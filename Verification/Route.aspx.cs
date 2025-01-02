using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;

public partial class Verification_Route : System.Web.UI.Page
{
    public int? setId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            setId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/Default.aspx");
       
        if (!IsPostBack)
        {
            DocSetDb docSetDb = new DocSetDb();
            if (!docSetDb.AllowVerificationSaveDate(setId.Value))
                Util.ShowUnauthorizedMessage();

            if (docSetDb.IsSetConfirmed(setId.Value))
                Util.ShowUnauthorizedMessage();

            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(setId.Value);
            DocSet.DocSetRow row = docSets[0];
            TreeviewDWMS.PopulateTreeView(RadTreeView1, setId.Value, true, true);
            RadTreeView1.ExpandAllNodes();
            populateUnit();
        }
    }


    protected void populateUnit()
    {
        RecipentDropDownList.Items.Clear();

        SectionDb sectionDb = new SectionDb();
        RecipentDropDownList.DataSource = sectionDb.GetSectionByOrder();
        RecipentDropDownList.DataTextField = "Name";
        RecipentDropDownList.DataValueField = "Id";
        RecipentDropDownList.DataBind();

        if (Request.Cookies["ROUTE_SECTION"] == null)
        {
            DocSetDb docSetDb = new DocSetDb();
            DataTable docSets = docSetDb.GetDocSetDocSectionDetail(setId.Value);
            RecipentDropDownList.SelectedValue = docSets.Rows[0]["SectionId"].ToString();

            HttpCookie userSelectionCookie = new HttpCookie("ROUTE_SECTION");
            userSelectionCookie.Values["SectionId"] = docSets.Rows[0]["SectionId"].ToString();

            userSelectionCookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(userSelectionCookie);
        }
        else
        {
            HttpCookie userSelectionCookie = Request.Cookies["ROUTE_SECTION"];
            RecipentDropDownList.SelectedValue = userSelectionCookie.Values["SectionId"].ToString();
            Response.AppendCookie(userSelectionCookie);
        }
    }
         

    protected void RadTreeView1_NodeCheck(object o, RadTreeNodeEventArgs e)
    {
        loadRemark();
    }

    protected void loadRemark()
    {
        DocSetDb docSetDb = new DocSetDb();
        DataTable docSets = docSetDb.GetDocSetDocSectionDetail(setId.Value);

        string fromUnit = docSets.Rows[0]["Name"].ToString();
        string toUnit = RecipentDropDownList.SelectedItem.Text;

        StringBuilder sb = new StringBuilder();
        StringBuilder sbSupport= new StringBuilder();
        sbSupport.Append(fromUnit);
        sbSupport.Append(" to ");
        sbSupport.Append(toUnit);
        sbSupport.Append(" on ");
        sbSupport.Append(DateTime.Now.ToString());
        sbSupport.Append("\n\n");

        if (!docSets.Rows[0]["sectionId"].ToString().Equals(RecipentDropDownList.SelectedValue.ToString()))
        {
            IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

            //if set node is selected
            if (checkedNodes.Count == TreeviewDWMS.GetTotalNodes(RadTreeView1))
            {
                string setNo = Format.RemoveHTMLTags(RadTreeView1.Nodes[0].Text);
                setNo = setNo.Substring(setNo.IndexOf(")")+1).Trim();

                sb.Append("The " + setNo + " have been routed from ");
                sb.Append(sbSupport.ToString());
                sb.Append("-" + setNo);
            }
            else
            {
                sb.Append("The following documents have been routed from ");
                sb.Append(sbSupport.ToString());

                List<int> uniqueNodes = new List<int>();
                foreach (RadTreeNode rNode in checkedNodes)
                {
                    if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
                    {
                        if (!uniqueNodes.Contains(int.Parse(rNode.Value))) // since one document can be in different folder, filter the repetition
                        {
                            uniqueNodes.Add(int.Parse(rNode.Value));
                            sb.Append("-" + Format.RemoveHTMLTags(rNode.Text) + "\n");
                        }
                    }
                }
            }
        }
        else
        {
            sb.Append(fromUnit + " is the current recipent. Please select a different receipent to route.");
        }

        RemarksTextBox.Text = sb.ToString();
    }

    protected void SendButton_onClick(object sender, EventArgs e)
    {
        DocSetDb docSetDb = new DocSetDb();
        if (!docSetDb.AllowVerificationSaveDate(setId.Value))
            Response.Redirect("view.aspx?id=" + setId.Value);

        if (docSetDb.IsSetConfirmed(setId.Value))
            Response.Redirect("view.aspx?id=" + setId.Value);

        DataTable docSets = docSetDb.GetDocSetDocSectionDetail(setId.Value);

        string fromUnit = docSets.Rows[0]["Name"].ToString();
        string toUnit = RecipentDropDownList.SelectedItem.Text;

        StringBuilder sb = new StringBuilder();

        //start the process only if the sectionid is different.
        if (!docSets.Rows[0]["sectionId"].ToString().Equals(RecipentDropDownList.SelectedValue.ToString()))
        {
            IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

            //if set node is selected
            if (checkedNodes.Count == TreeviewDWMS.GetTotalNodes(RadTreeView1))
            {
                //check if all the documents in the set are verified.
                DocDb docDb = new DocDb();
                if (docDb.IsDocumentsVerifiedForConfirmSet(setId.Value))
                {
                    //update set info
                    docSetDb.UpdateSectionForRoute(setId.Value, RemarksTextBox.Text, int.Parse(RecipentDropDownList.SelectedValue.ToString()));

                    SetVisibility(fromUnit, toUnit);
                }
                else
                {
                    RecipentCustomValidator.Text = Constants.UnableToRoute;
                    RecipentCustomValidator.IsValid = false;
                }
                urlTargetHiddenField.Value = "Default.aspx";
            }
            else
            {
                List<int> uniqueNodes = new List<int>();
                foreach (RadTreeNode rNode in checkedNodes)
                {
                    if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
                    {
                        if (!uniqueNodes.Contains(int.Parse(rNode.Value))) // since one document can be in different folder, filter the repetition
                        {
                            uniqueNodes.Add(int.Parse(rNode.Value));
                        }
                    }
                }

                if (uniqueNodes.Count > 0)
                {
                    AppDocRefDb appDocRefDb = new AppDocRefDb();
                    SetDocRefDb setDocRefDb = new SetDocRefDb();
                    RawFileDb rawFileDb = new RawFileDb();
                    RawPageDb rawPageDb = new RawPageDb();

                    // duplicate the docset
                    int newSetId = docSetDb.InsertFromOld(setId.Value, RemarksTextBox.Text, int.Parse(RecipentDropDownList.SelectedValue.ToString()));

                    // duplicate the setapp. this will attach the apppersonal records to the newset
                    SetAppDb setAppDb = new SetAppDb();
                    setAppDb.InsertFromOld(setId.Value, newSetId);

                    Dictionary<int, int> oldToNewRawFileId = new Dictionary<int, int>();

                    // duplicate the rawfile
                    oldToNewRawFileId = rawFileDb.InsertFromOldDocs(uniqueNodes, setId.Value, newSetId);

                    RawFile.RawFileDataTable rawFiles = rawFileDb.GetDataByDocSetId(setId.Value);
                    RawFile.RawFileRow rawFileRow = rawFiles[0];
                    int oldRawFileId = rawFileRow.Id;

                    //copy the documents
                    DocDb docDb = new DocDb();
                    DocTableAdapters.DocTableAdapter docTableAdapter = new DocTableAdapters.DocTableAdapter();

                    foreach (int docId in uniqueNodes)
                    {
                        Dictionary<int,int> oldToNewDocId = new Dictionary<int,int>();
                        Doc.DocDataTable doc = docDb.GetDocById(docId);
                        Doc.DocRow dr = doc[0];

                        DocStatusEnum docStatus = (DocStatusEnum) Enum.Parse(typeof(DocStatusEnum), dr.Status, true);
                        DocumentConditionEnum documentCondition = (DocumentConditionEnum) Enum.Parse(typeof(DocumentConditionEnum), dr.DocumentCondition, true);

                        //create new document
                        int newDocId = docDb.Insert(dr.DocTypeCode, newSetId, newSetId, docStatus, string.Empty, dr.ImageCondition, documentCondition);

                        //copy the apppersonal records related to the the document
                        appDocRefDb.CopyFromSource(docId, newDocId);

                        //copy the docpersonal related to the document
                        setDocRefDb.CopyFromSource(docId, newDocId, newSetId);

                        //duplicate the RawPage for a document to new rawFileId
                        rawPageDb.InsertFromOld(docId, newDocId, oldToNewRawFileId, true);

                        //log the action
                        AuditTrailDb auditTrailDb = new AuditTrailDb();
                        auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);

                        //update logAction
                        MembershipUser user = Membership.GetUser();

                        LogActionDb logActionDb = new LogActionDb();
                        Guid userId = (Guid)user.ProviderUserKey;

                        logActionDb.Insert(userId, LogActionEnum.Route_document, string.Empty, string.Empty, string.Empty, string.Empty, LogTypeEnum.D, docId);

                        //dettach and attach the documents to the route folder for the source set
                        AppDocRef.AppDocRefDataTable appDocRefs = appDocRefDb.GetAppDocRefByDocId(docId);
                        CustomPersonal customPersonal = new CustomPersonal();

                        foreach (AppDocRef.AppDocRefRow appDocRefRow in appDocRefs)
                        {
                            AppPersonalDb appPersonalDb = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalById(appDocRefRow.AppPersonalId);

                            foreach(AppPersonal.AppPersonalRow appPersonalRow in appPersonals)
                            {
                                customPersonal.AttachDocPersonalReference(docId, setId.Value, appPersonalRow.IsNricNull() ? "" : appPersonalRow.Nric, DocFolderEnum.Routed.ToString(), appPersonalRow.IsRelationshipNull() ? "" : appPersonalRow.Relationship);
                            }

                            appDocRefDb.Delete(appDocRefRow.Id);
                        }

                        SetDocRef.SetDocRefDataTable setDocRefs = setDocRefDb.GetSetDocRefByDocId(docId);

                        foreach (SetDocRef.SetDocRefRow setDocRefRow in setDocRefs)
                        {
                            DocPersonalDb docPersonalDb = new DocPersonalDb();
                            DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalById(setDocRefRow.DocPersonalId);

                            foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals)
                            {
                                customPersonal.AttachDocPersonalReference(docId, setId.Value, docPersonalRow.IsNricNull() ? "" : docPersonalRow.Nric, DocFolderEnum.Routed.ToString(), docPersonalRow.IsRelationshipNull() ? "" : docPersonalRow.Relationship);
                            }
                            setDocRefDb.Delete(setDocRefRow.Id);
                        }
                    }

                    //take the pagePath in an array which is used to create the main RawFile
                    ArrayList pagePath = new ArrayList();

                    foreach (var pair in oldToNewRawFileId)
                    {
                        int newRawFileid = pair.Value;

                        RawFile.RawFileDataTable newRawFiles = rawFileDb.GetRawFileById(pair.Value);
                        RawFile.RawFileRow newRawFileRows = newRawFiles[0];

                        string destPath = Util.GetIndividualRawFileOcrFolderPath(newRawFileRows.DocSetId, newRawFileRows.Id);

                        if (!Directory.Exists(destPath))
                            Directory.CreateDirectory(destPath);

                        RawPage.RawPageDataTable newRawPages = rawPageDb.GetRawPageByRawFileId(newRawFileid);

                        foreach (RawPage.RawPageRow rawPageRow in newRawPages)
                        {
                            String[] rawFilesArray;

                            //get destination path
                            string rawPagePath = Utility.GetIndividualRawPageOcrFolderPath(rawPageRow.Id);

                            rawFilesArray = Directory.GetFileSystemEntries(rawPagePath);

                            foreach (string rawFile in rawFilesArray)
                            {
                                if (Path.GetExtension(rawFile).ToLower().Trim().Equals(".pdf"))
                                    pagePath.Add(rawPagePath + "\\" + Path.GetFileName(rawFile));
                            }
                        }

                        //merge and save the file
                        Util.MergePdfFiles(pagePath, destPath + "\\" + newRawFileRows.FileName);
                        pagePath.Clear();
                    }

                    SetVisibility(fromUnit, toUnit);
                    urlTargetHiddenField.Value = "View.aspx?id=" + setId.ToString();
                }
                else
                {
                    RecipentCustomValidator.Text = "Please select a document to route.";
                    RecipentCustomValidator.IsValid = false;
                }
            }
        }
        else
        {
            RecipentCustomValidator.Text = fromUnit + " is the current recipent.<br/>Please select a different receipent to route.";
            RecipentCustomValidator.IsValid = false;
        }
    }

    protected void RecipentDropDownList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        if (Request.Cookies["ROUTE_SECTION"] == null)
        {
            HttpCookie userSelectionCookie = new HttpCookie("ROUTE_SECTION");
            userSelectionCookie.Values["SectionId"] = RecipentDropDownList.SelectedValue;
            userSelectionCookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(userSelectionCookie);
        }
        else
        {
            HttpCookie userSelectionCookie = Request.Cookies["ROUTE_SECTION"];
            userSelectionCookie.Values["SectionId"] = RecipentDropDownList.SelectedValue;
            Response.AppendCookie(userSelectionCookie);
        }

        loadRemark();
    }

    protected void SetVisibility(string fromUnit, string toUnit)
    {
        ConfirmLabel.Text = "The documents have been routed from <i>" + fromUnit + "</i> to <i>" + toUnit + "</i>.";
        ConfirmPanel.Visible = OKPanel.Visible = true;
        DisplayPanel.Visible = false;
    }

    protected void OkButton_onClick(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", String.Format("ResizeAndClose(700, 190, '{0}');", urlTargetHiddenField.Value.ToString()), true);
    }
}

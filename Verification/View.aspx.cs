using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.IO;
using System.Drawing;
using System.Text;
using Limilabs.Barcode;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class View_Set : System.Web.UI.Page
{
    #region Members
    public int? setId;
    private string currentDocType = string.Empty;
    private string selectedNodeGlobal = "0";
    private string selectedTabGlobal = string.Empty;
    public bool isSetConfirmed = false;
    public bool allowVerificationSaveDate = false;
    public bool allowVerificationSaveDateWithoutHouseholdCheck = false;        
    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        //string ocrDocStoragePath = Util.GetDocForOcrFolder();
        //DirectoryInfo ocrStorageDir = new DirectoryInfo(ocrDocStoragePath);

        //if (!string.IsNullOrEmpty(Request["id"]))
        //{
        //    setId = int.Parse(Request["id"]);
        //}
        //else
        //    Response.Redirect("~/Verification/");
        #endregion

        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        try
        {
            if (!string.IsNullOrEmpty(Request["id"]))
            {
                setId = int.Parse(Request["id"]);
            }
            else
                Response.Redirect("~/Verification/");
        }
        catch (Exception)
        {
            Response.Redirect("~/Verification/");
        }
                
        DataTable dt = DocSetDb.GetYearMonthDayForFolderStructure(setId.Value);
        DateTime datePath = new DateTime();
        bool isDate = false;
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            isDate = DateTime.TryParse(dr["VerificationDateIn"].ToString(), out datePath);
        }

        string ocrDocStoragePath = Util.GetDocForOcrFolder(setId.Value, datePath);
        DirectoryInfo ocrStorageDir = new DirectoryInfo(ocrDocStoragePath);
        #endregion


        #region Edited By Edward 2014.04.10 Optimize Slowness
        //DocSetDb docSetDb = new DocSetDb();
        //DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(setId.Value);

        DocSetDb docSetDb = new DocSetDb();
        DataTable docSets = docSetDb.GetStatusVerificationStaffUserIdById(setId.Value);

        // Check if set exit
        if (docSets.Rows.Count == 0)
            Response.Redirect("~/Verification/");
        else
        {
            //DocSet.DocSetRow docSetRow = docSets[0];            

            ////if(docSetRow.IsBeingProcessed)
            //if (docSetDb.CheckIfBeingProcessed(docSetRow.Id))
            if (docSetDb.CheckIfBeingProcessed(setId.Value))
                Response.Redirect("~/Verification/");

            isSetConfirmed = docSetDb.IsSetConfirmed(setId.Value);
        }

        #endregion
        

        allowVerificationSaveDate = docSetDb.AllowVerificationSaveDate(setId.Value);
        allowVerificationSaveDateWithoutHouseholdCheck = docSetDb.AllowVerificationSaveDateWithoutHouseholdCheck(setId.Value);

        //Added By Edward 12/3/2014 Sales and Resale Changes - Make ImageAccepted to Y by Default when sales or resale
        //UpdateImageAcceptedToYes();

        if (!IsPostBack)
        {
            SetUserSelectionCookie(string.Empty, string.Empty);
            PopulateTreeView();
            PopulateSetInfo();
            PopulateSplitType();

            //disable action buttons if the set is not assigned to the currentuser and set status is not verification_in_progress
            SubmitMetadataButton.Enabled = SubmitAndConfirmMDButton.Enabled = LeftConfirmButton.Enabled = LeftOptions.Enabled = allowVerificationSaveDate;
            AddReferenceButton.Enabled = ReferenceNoRadGrid.Columns[2].Visible = SaveSetInfoButton.Enabled = CancelSetInfoButton.Enabled = allowVerificationSaveDateWithoutHouseholdCheck;

            //disable action buttons if set is confirmed
            if (isSetConfirmed)
                SubmitMetadataButton.Enabled = SubmitAndConfirmMDButton.Enabled = LeftOptions.Enabled = ReferenceNoRadGrid.Columns[2].Visible = AddReferenceButton.Enabled = SaveSetInfoButton.Enabled = CancelSetInfoButton.Enabled = false;

            LoadContextContent(false);
        }

        //set currentDocType

        if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "doc")
        {
            DocDb docDb = new DocDb();
            Doc.DocDataTable docs = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));
            if (docs.Rows.Count > 0)
            {
                Doc.DocRow docRow = docs[0];
                currentDocType = docRow.DocTypeCode;
            }
        }

        //June 10, 2013 Added by Edward
        CheckAccessforAcceptance();        

        // Set Treeview and Thumbnail Panel Height
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SetHeightForControls", "javascript:SetHeightForControls();", true);
    }

    #region Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
    //protected void SetRightDocumentLabelText(int docId)
    //{
    //    DocDb docDb = new DocDb();
    //    Doc.DocDataTable docs = docDb.GetDocById(docId);

    //    if (docs.Rows.Count > 0)
    //    {
    //        Doc.DocRow docRow = docs[0];
    //        RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (docRow.Status.ToString().ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
    //    }
    //}

    protected void SetRightDocumentLabelText(string docId)
    {        
        try
        {            
            if (!string.IsNullOrEmpty(docId))
            {     
                int intDocId = int.Parse(docId);                
                DocDb docDb = new DocDb();
                Doc.DocDataTable docs = docDb.GetDocById(intDocId);

                if (docs.Rows.Count > 0)
                {                    
                    Doc.DocRow docRow = docs[0];
                    if(RadTreeView1.SelectedNode != null)
                        RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (docRow.Status.ToString().ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
                    else
                        RightDocumentLabel.Text = " (" + (docRow.Status.ToString().ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
                }
            }
        }
        catch (Exception ex)
        {
            RightDocumentLabel.Text = "Error in RightDocumentLabel: " + ex.Message;
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - SetRightDocumentLabelText", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex, " docId=" + docId);
        }
        
    }

    #endregion

    protected void DocTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Populate Meta fields
        int docId = int.Parse(RadTreeView1.SelectedValue);

        MetaDataDb metaDataDb = new MetaDataDb();

        //get the reference to the control
        UserControl userControl = AdditionalMetaDataPanel.FindControl(DocTypeDropDownList.SelectedValue.Trim() + "MetaDataUC") as UserControl;
        SetControlVisibility(userControl);
        LoadSaveControlData(DocTypeDropDownList.SelectedValue.Trim(), true, false, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());

        PopulateMetaData(DocTypeDropDownList.SelectedValue.Trim().ToUpper());
    }

    protected void MetaFieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            // Get the controls
            HiddenField IdHiddenFiled = e.Item.FindControl("IdHiddenFiled") as HiddenField;
        }
    }

    protected void ImageConditionDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"))
            SubmitAndConfirmMDButton.CausesValidation = false;
    }

    //Save button
    protected void SubmitMetadataButton_Click(object sender, EventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);
        RadTreeNode node = RadTreeView1.SelectedNode;
        if (node != null && Validation.IsInteger(node.Value))
        {
            //if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"))
            //{
            //    SaveMetaData(true, true);
            //}
            //else
            //{
            //Page.Validate();        //Added By Edward 08/01/2014    DWMS - Add UIN, FIN validation to Parent Image

            //if (Page.IsValid)       //Added By Edward 08/01/2014    DWMS - Add UIN, FIN validation to Parent Image
            //{
                SaveMetaData(false, false);
            //}
            //}
        }
    }

    #region Added by Edward Blank, Spam and Routed should not have same validation like Others 2015/10/16
    //Confirm button
    //protected void SubmitAndConfirmMDButton_Click(object sender, EventArgs e)
    //{
    //    RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

    //    RadTreeNode node = RadTreeView1.SelectedNode;

    //    DocDb docDb = new DocDb();
    //    Boolean isVerified = false;

    //    if (node != null && Validation.IsInteger(node.Value))
    //        isVerified = docDb.GetIsVerifiedByDocId(int.Parse(node.Value));

    //    if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
    //        Page.Validate();

    //    if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
    //    {
    //        if (Page.IsValid && node != null && Validation.IsInteger(node.Value))
    //            SaveMetaData(true, true);
    //    }
    //    else
    //    {
    //        if (node != null && Validation.IsInteger(node.Value))
    //            SaveMetaData(true, true);      //Edited by Edward validate date for blur/incomplete ; set to true for isValidate 2014/10/10
    //        //SaveMetaData(true, false);
    //    }
    //}

    protected void SubmitAndConfirmMDButton_Click(object sender, EventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

        RadTreeNode node = RadTreeView1.SelectedNode;

        DocDb docDb = new DocDb();
        Boolean isVerified = false;

        if (node != null && Validation.IsInteger(node.Value))
            isVerified = docDb.GetIsVerifiedByDocId(int.Parse(node.Value));

        //Added isPageValid by Edward 2015/10/15 
        bool isPageValid = true;

        if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
        {
            if (IsNeedValidation())
            {
                Page.Validate();
                isPageValid = Page.IsValid;
            }
        }

        if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
        {
            if (isPageValid && node != null && Validation.IsInteger(node.Value))
                SaveMetaData(true, true);
        }
        else
        {
            if (node != null && Validation.IsInteger(node.Value))
                SaveMetaData(true, true);      //Edited by Edward validate date for blur/incomplete ; set to true for isValidate 2014/10/10                              
        }
    }
    #endregion

    #region Added by Edward Blank, Spam and Routed should not have same validation like Others 2015/10/16
    private bool IsNeedValidation()
    {
        bool isNeed = false;
        RadTreeNode node = RadTreeView1.SelectedNode;

        if (node.ParentNode.Value.ToLower().Equals("blank") || node.ParentNode.Value.ToLower().Equals("spam"))
            isNeed = false;
        else if (node.ParentNode.ParentNode.Value.ToLower().Equals("blank") || node.ParentNode.ParentNode.Value.ToLower().Equals("spam"))
            isNeed = false;
        else
            isNeed = true;

        return isNeed;
    }

    #endregion

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        PopulateTreeView();
        #region Modified by Edward 2015/8/20 Fix Input string was not in a correct format in RadAjax
        //DisplayPdfDocument(int.Parse(RadTreeView1.SelectedValue));
        int intRadTreeValue;
        bool isInteger = int.TryParse(RadTreeView1.SelectedValue, out intRadTreeValue);
        if (isInteger)
            //DisplayPdfDocument(intRadTreeValue);       
            DisplayPdfDocument(intRadTreeValue.ToString());
        //else
        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "No selected Document. Please try again.", String.Format("javascript:alert('{0}');", Constants.UnableToConfirmSet), true);
        #endregion
    }

    private string GetHouseholdStructure(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        foreach (DataRow r in dt.Rows)
        {
            sb.Append(r["ApplicantType"].ToString());
            sb.Append(r["OrderNo"].ToString());
            sb.Append(": ");
            sb.Append(r["Name"].ToString());
            sb.Append("<br />");
        }
        return string.IsNullOrEmpty(sb.ToString()) ? "N.A." : sb.ToString();
    }

    private bool SendEmailToOic(int setId, bool hasPendingDoc)
    {
        //Get docapps based on the set id
        DocAppDb docAppDb = new DocAppDb();
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppByDocSetId(setId);

        DocSetDb docSetDb = new DocSetDb();
        DocSet.DocSetDataTable docSet = docSetDb.GetDocSetById(setId);

        if (docSet.Rows.Count > 0)
        {
            DocSet.DocSetRow docSetRow = docSet[0];
            string setNumber = docSetRow.SetNo;

            foreach (DocApp.DocAppRow docAppRow in docApps.Rows)
            {
                string peOIC = string.Empty;
                string caOIC = string.Empty;                                            
                //Get the RefNo from dbo.DocApp
                string refNo = docAppRow.RefNo.Trim();
                string hleStatus = HleInterfaceDb.GetHleStatusByRefNo(refNo);
                hleStatus = String.IsNullOrEmpty(hleStatus) ? "N/A" : hleStatus;
                //Added By Edward 2015/02/10 Added risk for email notification
                bool IsRiskLow = DocAppDb.CheckRefNoRiskIsL(docAppRow.Id);
                peOIC = docAppRow.IsPeOICNull() ? string.Empty : docAppRow.PeOIC.Trim(); // get PeOIC (null or not)
                caOIC = docAppRow.IsCaseOICNull() ? string.Empty : docAppRow.CaseOIC.Trim(); // get CaseOIC (null or not)
                SendEmail(hasPendingDoc, peOIC, caOIC, hleStatus, docAppRow.RefType.Trim(), refNo, setNumber, setId, docAppRow.Id.ToString(), IsRiskLow);
            }
            return true;
        }
        else        
            return false;        
    }
       
    protected void LeftConfirmButton_Click(object sender, EventArgs e)
    {
        // Check if user can click on the confirm the set

        DocSetDb docSetDb = new DocSetDb();
        DocAppDb docAppDb = new DocAppDb();
        //CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();

        if (docSetDb.CanUserConfirmSet(setId.Value, RadTreeView1))
        {
            
            //update set status
            docSetDb.UpdateSetStatus(setId.Value, SetStatusEnum.Verified, true, false, LogActionEnum.Confirmed_set);

            docSetDb.UpdateSendToCdbStatus(setId.Value, SendToCDBStatusEnum.Ready);

            //disable the action buttons.
            LeftConfirmButton.Visible = false;
            LeftOptions.Enabled = SubmitAndConfirmMDButton.Enabled = SubmitMetadataButton.Enabled = false;
            ReferenceNoRadGrid.Columns[2].Visible = AddReferenceButton.Enabled = SaveSetInfoButton.Enabled = CancelSetInfoButton.Enabled = RotateClockwiseButton.Enabled = ExtractButton.Enabled = false;

            //update reference no status
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppByDocSetId(setId.Value);
            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(setId.Value);
            //if (docSets.Count > 0)
            //{docSets[0].VerificationDateIn
            //}                                             
            foreach (DocApp.DocAppRow docAppRow in docApps.Rows)
            {
                // Update the status to "Verified" if the status if "Pending_Documents"

                #region Added By Edward 03/02/2014 Changes for Sales and Resale
                if (docAppRow.RefType.ToUpper().Equals(ReferenceTypeEnum.SALES.ToString()) || docAppRow.RefType.ToUpper().Equals(ReferenceTypeEnum.RESALE.ToString()))
                {
                    if (docAppRow.Status.Trim().Equals(AppStatusEnum.Pending_Documents.ToString()))
                        docAppDb.UpdateRefStatus(docAppRow.Id, AppStatusEnum.Verified, AssessmentStatusEnum.Verified, false, false, null);
                    IncomeDb.InsertMonthYearInIncomeTable(docAppRow.Id, IncomeMonthsSourceEnum.C);
                }
                else
                {
                    if (docAppRow.Status.Trim().Equals(AppStatusEnum.Pending_Documents.ToString()))
                        docAppDb.UpdateRefStatus(docAppRow.Id, AppStatusEnum.Verified, false, false, null);
                }
                #endregion
             
                // Update the Date In if it is null
                if (docAppRow.IsDateInNull())
                    docAppDb.UpdateDateIn(docAppRow.Id, docSetDb.GetEarliestVerificationDateInByDocAppId(docAppRow.Id));
                
                if (!docAppRow.IsDateAssignedNull())
                {
                    if (!docAppRow.IsSecondCANull())
                    {
                        #region Added By Edward 09.01.2014
                        if (docAppRow.SecondCA && Convert.ToDateTime(docAppRow.SecondCADate) < docSets[0].VerificationDateIn
                            && (docAppRow.PeOIC.Trim() == "" || !String.IsNullOrEmpty(docAppRow.PeOIC.Trim()) || docAppRow.PeOIC.Trim() == "-" || docAppRow.PeOIC.Trim().ToUpper() == "COS")
                            && docAppRow.DateAssigned < Convert.ToDateTime(docAppRow.SecondCADate))
                        {
                            docAppDb.UpdateSecondCA(docAppRow.Id, docSets[0].VerificationDateIn);
                        }
                        #endregion
                    }
                }
                //CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();
                //cdbPendingDoc.ProcessPendingDocs(docAppRow.RefNo.ToString(), "PAR", "LAT");
                //docAppDb.UpdatePendingDoc(docAppRow.RefNo);
               
                //send email to application OIC
                LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
                if (leasInterfaceDb.HasPendingDoc(docAppRow.RefNo.ToString()))
                    SendEmailToOic(setId.Value,true);
                else
                    SendEmailToOic(setId.Value, false);

            }

            //set confirm label text
            SetConfirmLabel.Text = Constants.SetVerified;

            //disable the tree node drag/drop
            PopulateTreeView();
            foreach (RadTreeNode node in RadTreeView1.GetAllNodes())
            {
                if (node.AllowDrag == true)
                    node.AllowDrag = false;

                if (node.AllowDrop == true)
                    node.AllowDrop = false;
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Unable to Confirm Set", String.Format("javascript:alert('{0}');", Constants.UnableToConfirmSet), true);

            PopulateTreeView();
        }
    }

    protected void SaveSetInfoButton_Click(object sender, EventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

        Page.Validate();

        if (Page.IsValid)
        {
            int docAppId = -1;
            DocSetDb docSetDb = new DocSetDb();
            docSetDb.Update(setId.Value, docAppId, string.Empty, -1, string.Empty, string.Empty, ChannelDropDownList.SelectedValue, RemarkTextBox.Text);
            ConfirmSetPanel.Visible = true;
        }
    }

    protected void RadToolBarMenu_ButtonClick(object sender, RadToolBarEventArgs e)
    {
        string buttonClicked = e.Item.Value.ToLower();
        int docId = int.Parse(RadTreeView1.SelectedValue);
        RawPageDb rawPageDb = new RawPageDb();

        if (buttonClicked.Equals("log"))
            LogButton_Click(sender, e);
        else if (buttonClicked.Equals("image"))
            ImgButton_Click(sender, e);
        else if (buttonClicked.Equals("thumbnails"))
            ThumbnailButton_Click(sender, e);
    }

    protected void LogButton_Click(object sender, EventArgs e)
    {
        MetaDataPanel.Visible = pdfframe.Visible = SetInfoPanel.Visible = RotateClockwiseButton.Visible = ExtractButton.Visible = ThumbnailPanel.Visible = false;
        LogPanel.Visible = ExportButton.Visible = true;
        PopulateLogAction();
        RadGridLog.DataBind();
        SetUserSelectionCookie("LOG", string.Empty);
        SetButtonClass("Log");
    }

    protected void ThumbnailButton_Click(object sender, EventArgs e)
    {
        int docId = int.Parse(RadTreeView1.SelectedValue);
        RawPageDb rawPageDb = new RawPageDb();
        MetaDataPanel.Visible = pdfframe.Visible = SetInfoPanel.Visible = ExportButton.Visible = LogPanel.Visible = false;
        ThumbnailPanel.Visible = RotateClockwiseButton.Visible = ExtractButton.Visible = true;
        RotateClockwiseButton.Enabled = ExtractButton.Enabled = allowVerificationSaveDate;
        if (isSetConfirmed)
            RotateClockwiseButton.Enabled = ExtractButton.Enabled = false;
        if (rawPageDb.CountRawPageByDocId(docId) <= 1)
            ExtractButton.Enabled = false;

        RadToolTipManager1.TargetControls.Clear();
        ThumbnailRadListView.Rebind();
        SetUserSelectionCookie("THUMBNAILS", string.Empty);
        SetButtonClass("Thumbnails");
    }

    protected void ImgButton_Click(object sender, EventArgs e)
    {
        ThumbnailPanel.Visible = RotateClockwiseButton.Visible = ExportButton.Visible = LogPanel.Visible = SetInfoPanel.Visible = false;
        MetaDataPanel.Visible = pdfframe.Visible = ExtractButton.Visible = true;
        ExtractButton.Enabled = allowVerificationSaveDate;
        if (isSetConfirmed)
            ExtractButton.Enabled = false;
        SetUserSelectionCookie("IMAGE", string.Empty);
        SetButtonClass("Image");

        if (!Object.Equals(RadToolTipManager1, null))
        {
            //Add the button (target) id to the tooltip manager                
            RadToolTipManager1.TargetControls.Add(ExtractButton.ClientID, true);
        }
    }

    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        this.UpdateToolTip(args.Value, args.UpdatePanel);
    }

    private void UpdateToolTip(string id, UpdatePanel panel)
    {
        Control ctrl = Page.LoadControl("~/Controls/SplitDocument.ascx");
        panel.ContentTemplateContainer.Controls.Add(ctrl);

        Controls_SplitDocument splitDocument = ctrl as Controls_SplitDocument;
        splitDocument.DocumentId = int.Parse(RadTreeView1.SelectedValue);
    }

    protected void SetButtonClass(string activeButton)
    {
        foreach (RadToolBarButton radToolBarButton in RadToolBarMenu.Items)
        {
            radToolBarButton.Font.Bold = false;
        }

        RadToolBarButton activeBarButton = RadToolBarMenu.FindItemByValue(activeButton) as RadToolBarButton;

        if (activeBarButton != null)
            activeBarButton.Font.Bold = true;
    }

    protected void ExportButton_Click(object sender, EventArgs e)
    {
        RadGridLog.ExportSettings.OpenInNewWindow = true;
        RadGridLog.ExportSettings.ExportOnlyData = true;
        RadGridLog.ExportSettings.IgnorePaging = true;

        DocSetDb docSetDb = new DocSetDb();
        DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(setId.Value);
        DocSet.DocSetRow docSetRow = docSets[0];

        RadGridLog.ExportSettings.FileName = docSetRow.SetNo;

        //code below to resolve the https download problem
        Page.Response.ClearHeaders();
        Page.Response.Cache.SetCacheability(HttpCacheability.Private);

        //RadGridLog.ExportSettings.ExportOnlyData = true;
        RadGridLog.MasterTableView.ExportToExcel();
    }

    protected void RadGridLog_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateLogAction();
    }

    protected void RadGridLog_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            Label ItemCountLabel = e.Item.FindControl("ItemCountLabel") as Label;
            int page = RadGridLog.MasterTableView.PagingManager.CurrentPageIndex;
            ItemCountLabel.Text = (page * RadGridLog.PageSize + e.Item.ItemIndex + 1).ToString();
        }
    }

    protected void LeftOptions_SelectedIndexChanged(object sender, EventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

        string option = LeftOptions.SelectedValue;

        switch (option)
        {
            case "Download":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Download", "loadExport();", true);
                break;
            case "Assign":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                    "AssignSet", "ShowWindow('Assign.aspx?id=" + setId + "', 500, 400);", true);
                break;
            case "Route":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                    "Route", "ShowWindow('Route.aspx?id=" + setId + "', 600, 600);", true);
                break;
            case "Email":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                    "Email", "ShowWindow('Email.aspx?id=" + setId + "', 600, 600);", true);
                break;
            case "Set Info":
                SetUserSelectionCookie(string.Empty, setId.Value.ToString());
                PopulateSetInfo();
                PopulateTreeView();
                break;
        }

        LeftOptions.SelectedIndex = 0;

    }

    protected void RotateClockwiseButton_Click(object sender, EventArgs e)
    {

        try
        {
            RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

            // Get selected ids
            List<int> pageIds = GetRawPageIds();
            int docId = int.Parse(RadTreeView1.SelectedValue);

            if (pageIds.Count >= 1)
            {
                ParameterDb parameterDb = new ParameterDb();
                int binarize = parameterDb.GetOcrBinarize();
                int bgFactor = parameterDb.GetOcrBackgroundFactor();
                int fgFactor = parameterDb.GetOcrForegroundFactor();
                int quality = parameterDb.GetOcrQuality();
                string morph = parameterDb.GetOcrMorph();
                bool dotMatrix = parameterDb.GetOcrDotMatrix();
                int despeckle = parameterDb.GetOcrDespeckle();
                foreach (int pageId in pageIds)
                {
                    DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(pageId);
                    if (rawPageDir.Exists)
                    {
                        FileInfo[] files = rawPageDir.GetFiles();
                        foreach (FileInfo file in files)
                        {
                            string fileName = file.Name.ToLower();
                            bool rotateSuccess = false;
                            bool ocrSuccess = false;
                            string ocrText = string.Empty;
                            string errorReason = string.Empty;
                            string errorException = string.Empty;

                            // If the PDF file is searcheable, retrieve the text of the PDF.  If the result is an empty string, do an
                            // OCR of the page to retrieve the text.
                            string tempContents = string.Empty;

                            if (!fileName.EndsWith("_s.pdf") && !fileName.EndsWith("_th.jpg")) // original file
                            {
                                //Rotating Original Image
                                rotateSuccess = rotateImage(file, 1);

                                #region Extracting Text from Original File
                                if (fileName.ToUpper().EndsWith(".PDF"))
                                { //create searchable PDF if possible, so no need to OCR
                                    int errorCode = Util.ExtractTextFromSearcheablePdf(file.FullName, null, false, out tempContents);
                                    if (errorCode < 0)
                                    {
                                        tempContents = string.Empty; //Cannot retrieve text, so let's let the OCR file extract the text
                                    }
                                }

                                if (String.IsNullOrEmpty(tempContents))
                                {
                                    OcrManager ocrManager = new OcrManager(file.FullName, null, binarize, bgFactor, fgFactor, quality, morph, dotMatrix, despeckle);
                                    ocrSuccess = ocrManager.GetOcrText(out ocrText, out errorReason, out errorException);
                                    ocrManager.Dispose();
                                }
                                else
                                {
                                    // Create the searcheable PDF copy
                                    FileInfo file2 = new FileInfo(file.FullName);
                                    string searcheablePdf = Path.Combine(Server.MapPath(file2.DirectoryName), file2.Name + "_s.pdf");
                                    try
                                    {
                                        file2.CopyTo(searcheablePdf);
                                    }
                                    catch (Exception) { }

                                    ocrText = tempContents;
                                    ocrSuccess = true;
                                }

                                if (ocrSuccess)
                                {
                                    RawPageDb rawPageDb2 = new RawPageDb();
                                    rawPageDb2.Update(pageId, ocrText, true, 0);
                                    break;
                                }
                                //else
                                //{
                                //    if (rotateSuccess)
                                //    {
                                //        rotateSuccess = rotateImage(file, 3);
                                //        break;
                                //    }
                                //}

                                // Check if the OCR was successful
                                // If not successful, create a PDF from the image
                                if (!ocrSuccess && String.IsNullOrEmpty(ocrText)) //something wrong with the OcrManager side
                                {
                                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", file.FullName), true);
                                    Util.CreateSearcheablePdfFile(file.FullName);
                                    string tempImagePath = string.Empty;
                                    string thumbNailPath = string.Empty;
                                    if (file.FullName.ToLower().EndsWith(".pdf"))
                                        if (!File.Exists(file.FullName.ToLower() + "_tmp.jpg_th.jpg"))
                                            File.Delete(file.FullName.ToLower() + "_tmp.jpg_th.jpg");
                                        else
                                            if (File.Exists(file.FullName.ToLower() + "_th.jpg"))
                                                File.Delete(file.FullName.ToLower() + "_th.jpg");

                                    try
                                    {
                                        // Create the thumbnail file
                                        //ImageManager.Resize(newRawPageTempPath, 113, 160);
                                        tempImagePath = Util.SaveAsJpegThumbnailImage(file.FullName.ToLower() + "_s.pdf");
                                        thumbNailPath = ImageManager.Resizes(tempImagePath);

                                        try
                                        {
                                            //Commented by Edward 2015/10/13
                                            if (File.Exists(tempImagePath)) File.Delete(tempImagePath);
                                        }
                                        catch { }
                                    }

                                    catch { }

                                    // Set the 'OcrFailed' flag to true
                                    RawPageDb rawPageDb2 = new RawPageDb();
                                    rawPageDb2.UpdateOcrFailed(pageId, true);
                                }
                                //if (File.Exists(Path.Combine(rawPageDir.FullName, "Thumbs.db_s.pdf"))) File.Delete(Path.Combine(rawPageDir.FullName, "Thumbs.db_s.pdf"));
                                #endregion
                            }
                        }
                    }
                }
            }

            // Reload the controls
            //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
            //SetRightDocumentLabelText(docId);
            SetRightDocumentLabelText(docId.ToString());
            PopulateTreeView();
            PopulateSetInfo();
            LoadContextContent(false);
            ThumbnailRadListView.Rebind();
            //RedirectToDefaultPage();
        }
        catch (Exception ex)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - RotateButton", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
        }
        
    }

    public bool rotateImage(FileInfo file, int rotateNum)
    {
        string fileName = file.Name.ToLower();
        if (fileName.EndsWith(".pdf"))
        { // rotate function for PDF
            string tempPdfPath = file.FullName + ".temp";
            PdfReader reader = new PdfReader(file.FullName);
            using (FileStream fs = new FileStream(tempPdfPath, FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);
                PdfDictionary pageDictionary = reader.GetPageN(1); //only get first page (raw page only 1 page)
                int desiredRot = rotateNum * 90; //parameter of how many degrees want to rotate clockwise
                PdfNumber rotation = pageDictionary.GetAsNumber(PdfName.ROTATE);
                if (rotation != null)
                {
                    desiredRot += rotation.IntValue;
                    desiredRot %= 360;
                }
                pageDictionary.Put(PdfName.ROTATE, new PdfNumber(desiredRot));
                stamper.Close();
            }
            File.Replace(tempPdfPath, file.FullName, file.FullName + ".backup");
            File.Delete(file.FullName + ".backup");

            return true;
        }
        else if (fileName.EndsWith(".png") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".gif") || fileName.EndsWith(".tif") || fileName.EndsWith(".tiff") || fileName.EndsWith(".bmp"))
        { //rotate function for images (PNG, JPEG, TIF, BMP, GIF)
            System.Drawing.Image image = null;
            #region 20170905 Updated By Edward Use FromStream for Out of memory
            //using (image = System.Drawing.Image.FromFile(file.FullName))            
            string strPhoto = (file.FullName);
            FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);
            using (image = System.Drawing.Image.FromStream(fs))            
            {
                switch (rotateNum)
                {
                    case 1:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 2:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 3:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                image.Save(file.FullName + ".temp");
                image.Dispose();
                File.Replace(file.FullName + ".temp", file.FullName, file.FullName + ".backup");
                File.Delete(file.FullName + ".backup");
            }
            #endregion
            return true;
        }
        return false;

    }


    protected void ExtractButton_Click(object sender, EventArgs e)
    {
        try //Added Try Catch ErrorLog by Edward 27.10.2013
        {
            RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

            // Get selected ids
            List<int> pageIds = GetRawPageIds();
            int docId = int.Parse(RadTreeView1.SelectedValue);

            if (pageIds.Count >= 1)
            {
                DocDb docDb = new DocDb();
                SplitTypeEnum splitType = (SplitTypeEnum)Enum.Parse(typeof(SplitTypeEnum), SplitTypeRadioButtonList.SelectedValue.Trim(), true);

                docDb.SplitDocumentForVerification(docId, pageIds, splitType);
            }

            // Reload the controls
            //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
            //SetRightDocumentLabelText(docId);
            SetRightDocumentLabelText(docId.ToString());
            PopulateTreeView();
            PopulateSetInfo();
            LoadContextContent(false);
        }
        catch (Exception ex)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - ExtractButton_Click", errorMessage);
        }       
    }

    #region Multiple Reference
    //protected void DocAppRadComboBox_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    //{
    //    if (!String.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()))
    //    {
    //        if (Validation.IsNric(RefNoRadComboBox.Text.Trim()) ||
    //            Validation.IsFin(RefNoRadComboBox.Text.Trim()))
    //        {
    //            ScriptManager.RegisterStartupScript(
    //                this.Page,
    //                this.GetType(),
    //                "ChooseReferenceScript",
    //                String.Format("ShowWindow('../Common/ChooseReference.aspx?nric={0}', 500, 500);", RefNoRadComboBox.Text.Trim()),
    //                true);
    //        }
    //        else
    //        {
    //            DocAppDb docAppDb = new DocAppDb();

    //            //2012-09-12
    //            DocApp.DocAppDataTable docApp = null;
    //            if (Validation.IsHLENumber(RefNoRadComboBox.Text.Trim())) //HLE
    //                docApp = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());
    //            else if (Validation.IsSersNumber(RefNoRadComboBox.Text.Trim())) //SERS
    //                docApp = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());
    //            else if (Validation.IsSalesNumber(RefNoRadComboBox.Text.Trim())) //SALES
    //                docApp = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());
    //            else if (Validation.IsResaleNumber(RefNoRadComboBox.Text.Trim())) //RESALE
    //                docApp = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

    //            if (docApp != null)
    //            {
    //                if (docApp.Rows.Count > 0)
    //                {
    //                    DocApp.DocAppRow row = docApp[0];
    //                    ReferenceTypeLabel.Text = row.RefType;
    //                    //ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

    //                    #region 2012-09-06 List the household structure
    //                    if (row.RefType.ToUpper().Trim() == "HLE")
    //                    {
    //                        //SersInterface
    //                        HleInterfaceDb db = new HleInterfaceDb();
    //                        DataTable dt = db.GetApplicantDetailsByRefNo(RefNoRadComboBox.Text.Trim());
    //                        householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                    }
    //                    else if (row.RefType.ToUpper().Trim() == "SERS")
    //                    {
    //                        //SersInterface
    //                        SersInterfaceDb db = new SersInterfaceDb();
    //                        DataTable dt = db.GetApplicantDetailsByRefNo(RefNoRadComboBox.Text);
    //                        householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                    }
    //                    else if (row.RefType.ToUpper().Trim() == "SALES")
    //                    {
    //                        //SalesInterface
    //                        SalesInterfaceDb db = new SalesInterfaceDb();
    //                        DataTable dt = db.GetApplicantDetailsByRefNo(RefNoRadComboBox.Text);
    //                        householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                    }
    //                    else if (row.RefType.ToUpper().Trim() == "RESALE")
    //                    {
    //                        //ResaleInterface
    //                        ResaleInterfaceDb db = new ResaleInterfaceDb();
    //                        DataTable dt = db.GetApplicantDetailsByRefNo(RefNoRadComboBox.Text);
    //                        householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                    }
    //                    else
    //                    {
    //                        householdStructureLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //                    }
    //                    #endregion 2012-09-06 List the household structure



    //                    ////update the address field based on the address for the docset sorted by submission date desc.
    //                    //DocSetDb docSetDb = new DocSetDb();
    //                    //DocSet.DocSetDataTable docSet = docSetDb.GetLatestDocSetByDocAppId(int.Parse(RefNoRadComboBox.SelectedValue));

    //                    //if (docSet.Rows.Count > 0)
    //                    //{
    //                    //    DocSet.DocSetRow docSetRow = docSet[0];
    //                    //    BlockTextBox.Text = docSetRow.IsBlockNull() ? string.Empty : docSetRow.Block;
    //                    //    StreetNameRadComboBox.SelectedValue = docSetRow.IsStreetIdNull() ? "1" : docSetRow.StreetId.ToString();
    //                    //    LevelTextBox.Text = docSetRow.IsFloorNull() ? string.Empty : docSetRow.Floor;
    //                    //    UnitNumberTextBox.Text = docSetRow.IsUnitNull() ? string.Empty : docSetRow.Unit;
    //                    //}

    //                    //update the address field based on the address for the sersinterface table.
    //                    //        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
    //                    //        SersInterface.SersInterfaceDataTable sersInterface = sersInterfaceDb.GetSersInterfaceByRefNo(RefNoRadComboBox.Text);

    //                    //        if (sersInterface.Rows.Count > 0)
    //                    //        {
    //                    //            SersInterface.SersInterfaceRow sersInterfaceRow = sersInterface[0];
    //                    //            BlockTextBox.Text = sersInterfaceRow.IsBlockNull() ? string.Empty : sersInterfaceRow.Block;
    //                    //            if (sersInterfaceRow.IsStreetNull())
    //                    //                StreetNameRadComboBox.SelectedIndex = 0;
    //                    //            else
    //                    //            {
    //                    //                StreetDb streetDb = new StreetDb();
    //                    //                Street.StreetDataTable streets = streetDb.GetStreetByName(sersInterfaceRow.Street.Trim());

    //                    //                if (streets.Rows.Count > 0)
    //                    //                {
    //                    //                    Street.StreetRow streetRow = streets[0];
    //                    //                    StreetNameRadComboBox.SelectedValue = streetRow.Id.ToString();
    //                    //                }
    //                    //                else
    //                    //                    StreetNameRadComboBox.SelectedIndex = 0;

    //                    //            }
    //                    //            LevelTextBox.Text = sersInterfaceRow.IsLevelNull() ? string.Empty : sersInterfaceRow.Level;
    //                    //            UnitNumberTextBox.Text = sersInterfaceRow.IsUnitNull() ? string.Empty : sersInterfaceRow.Unit;
    //                    //        }
    //                    //    }
    //                    //    else
    //                    //    {
    //                    //        ReferenceTypeLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //                    //        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
    //                    //        householdStructureLabel.Text = "N.A";
    //                    //    }
    //                    //}
    //                    //else
    //                    //{
    //                    //    ReferenceTypeLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //                    //    ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
    //                    //    householdStructureLabel.Text = "N.A";
    //                    //}


    //                    //ReferenceTypeLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //                    //ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

    //                    //CheckRefNoChanged();
    //                    //CheckRefNoExistence();

    //                    //if (Request.Url.ToString().ToLower().Contains("/import/scandocuments/"))
    //                    //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeTwainScript", "InitializeTwain();", true);
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        ReferenceTypeLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //        householdStructureLabel.Text = Util.GetReferenceType(RefNoRadComboBox.Text.Trim()).Replace("_", " ");
    //        householdStructureLabel.Text = "N.A";
    //    }
    //}


   

    protected void ReferenceNoRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        SetAppDb setAppDb = new SetAppDb();
        ReferenceNoRadGrid.DataSource = setAppDb.GetvSetAppByDocSetId(setId.Value);
    }

    protected void RawfileRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        RawFileDb rawFileDb = new RawFileDb();
        RawfileRadGrid.DataSource = rawFileDb.GetDataByDocSetId(setId.Value);
    }

    protected void ReferenceNoRadGrid_ItemCommand(object sender, GridCommandEventArgs e)
    {
        SetAppDb setAppDb = new SetAppDb();

        bool rePopulate = false;
        int id = int.Parse(e.CommandArgument.ToString());

        if (e.CommandName.Equals("Delete"))
        {
            RedirectIfSetConfirmedOrUserNotAllowedToUpdate(true);

            //delete the docref of the documents which are tied to this reference number of this set and place them under unidentified.
            //get the docAppId
            SetApp.SetAppDataTable setApp = setAppDb.GetSetAppById(id);
            if (setApp.Rows.Count > 0)
            {
                SetApp.SetAppRow setAppRow = setApp[0];

                AppDocRefDb appDocRefDb = new AppDocRefDb();
                AppDocRef.AppDocRefDataTable appDocRef = appDocRefDb.GetAppDocRefByDocSetIdAndDocAppId(setId.Value, setAppRow.DocAppId);

                CustomPersonal customPersonal = new CustomPersonal();

                foreach (AppDocRef.AppDocRefRow appDocRefRow in appDocRef.Rows)
                {
                    customPersonal.DettachAppPersonalReference(appDocRefRow.DocId);
                    customPersonal.AttachDocPersonalReference(appDocRefRow.DocId, setId.Value, string.Empty, DocFolderEnum.Unidentified.ToString(), string.Empty);
                }

                DocAppDb docAppDb = new DocAppDb();
                DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppById(setAppRow.DocAppId);

                if (docAppTable.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppRow = docAppTable[0];

                    // Log the action
                    LogActionDb logActionDb = new LogActionDb();
                    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_removed_as_reference_number, docAppRow.RefNo, string.Empty,
                        string.Empty, string.Empty, LogTypeEnum.S, setId.Value);
                }

                setAppDb.Delete(id);

                //check if we need to reset the application status?
            }

            rePopulate = true;
        }
        else if (e.CommandName.Equals("Retrieve"))
        {
            RedirectIfSetConfirmedOrUserNotAllowedToUpdate(true);

            int setAppId = int.Parse(e.CommandArgument.ToString());

            SetApp.SetAppDataTable dt = setAppDb.GetSetAppById(setAppId);

            if (dt.Rows.Count > 0)
            {
                SetApp.SetAppRow dr = dt[0];

                // Recreate the app personal record for the app
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                appPersonalDb.SavePersonalRecords(dr.DocAppId);
                rePopulate = true;
            }
        }
        else if (e.CommandName.Equals("PrintBarcode"))
        {
            #region using the sato printer
            //SetApp.SetAppDataTable setApp = setAppDb.GetSetAppById(id);
            //SetApp.SetAppRow setAppRow = setApp[0];
            //DocAppDb docAppDb = new DocAppDb();
            //DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(setAppRow.DocAppId);
            //DocApp.DocAppRow docAppRow = docApp[0];

            //int ID = -1;
            //Boolean Result = false;

            //LabelGalleryPlus3WR.LGApp LG = null;

            //try
            //{
            //    LG = new LGApp();
            //    string labelPath = Server.MapPath(Retrieve.GetBarcodePath());
            //    labelPath = Path.Combine(labelPath, "barcode.lbl");

            //    ID = LG.LabelOpen(labelPath);

            //    LG.LabelSetPrinter(ID, "\\\\HIEND-PC007\\sato cx400");

            //    Result = LG.LabelSetVar(ID, "barcodeValue", docAppRow.RefNo.Trim(), -9999, -9999);
            //    Result = LG.LabelSetVar(ID, "barcodeText", docAppRow.RefType.Trim() + " : " + docAppRow.RefNo.Trim(), -9999, -9999);
            //    Result = LG.LabelSetVar(ID, "dateValue", Format.FormatDateTime(DateTime.Now, DateTimeFormat.d__MMM__yyyy_C_h_Col_mm__tt), -9999, -9999);

            //    Result = LG.LabelPrint(ID, "1");
            //    LG.LabelClose(ID);
            //    LG.Quit();
            //}
            //catch (Exception exec)
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", Constants.BarcodeObjectInstantiationError), true);
            //}
            //finally
            //{
            //    if (LG != null)
            //        LG.Quit();
            //}

            //if (Result)
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", Constants.BarcodePrinted), true);
            //else
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", Constants.BarcodePrintedError), true);
            #endregion

            SetApp.SetAppDataTable setApp = setAppDb.GetSetAppById(id);

            if (setApp.Rows.Count > 0)
            {
                SetApp.SetAppRow setAppRow = setApp[0];

                DocAppDb docAppDb = new DocAppDb();
                DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(setAppRow.DocAppId);
                DocApp.DocAppRow docAppRow = docApp[0];

                //// save barcode image into your system
                string saveDir = Util.GetTempFolder();
                string imageFileName = docAppRow.RefNo + "_" + Guid.NewGuid() + ".png";
                string barcodeFileName = Path.Combine(saveDir, imageFileName);

                BaseBarcode barcode = BarcodeFactory.GetBarcode(Symbology.Code128);
                barcode.FontStyle = FontStyleType.Bold;
                barcode.ForeColor = System.Drawing.Color.Black;
                barcode.Number = docAppRow.RefNo;
                barcode.ChecksumAdd = false;
                barcode.ChecksumVisible = false;
                barcode.FontHeight = 0.27;
                barcode.CustomText = docAppRow.RefType + " : " + docAppRow.RefNo + "\n" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.d__MMM__yyyy_C_h_Col_mm__tt);
                barcode.Height = 120;

                //save it to file:
                barcode.Save(barcodeFileName, ImageType.Png, 300, 300);

                //crop the image. remove the watermark
                Bitmap cropBmp = null;

                #region 20170905 Updated By Edward Use FromStream for Out of memory
                //System.Drawing.Image image = System.Drawing.Image.FromFile(barcodeFileName);
                string strPhoto = (barcodeFileName);
                FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);
                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                #endregion

                System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(0, 50, image.Width, 70);

                try
                {
                    Bitmap bmp = image as Bitmap;
                    // Check if it is a bitmap:
                    if (bmp == null)
                        throw new ArgumentException("No valid bitmap");

                    // Crop the image:
                    cropBmp = bmp.Clone(cropRect, bmp.PixelFormat);

                    // Release the resources:
                    image.Dispose();

                    cropBmp.Save(barcodeFileName);

                    //Bitmap bm = new Bitmap(barcodeFileName);
                    //bm.Save(Response.OutputStream, ImageFormat.Png);

                    //bm.Dispose();
                    cropBmp.Dispose();

                    int setAppId = int.Parse(e.CommandArgument.ToString());

                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                            "Print", "OpenWindow('PrintBarcode.aspx?setappid=" + id + "&filename=" + imageFileName + "');", true);

                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", ex.Message), true);
                }
                finally
                {
                    if (image != null)
                        image.Dispose();

                    if (cropBmp != null)
                        cropBmp.Dispose();
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", "No SetApp with the id: " + id.ToString() + " exist"), true);
            }
        }

        DocSetDb docSetDb = new DocSetDb();
        LeftConfirmButton.Enabled = docSetDb.AllowVerificationSaveDate(setId.Value);

        if (rePopulate)
        {
            PopulateTreeView();
        }
    }

    //protected void AddReferenceButton_Click(object sender, EventArgs e)
    //{
    //    RedirectIfSetConfirmedOrUserNotAllowedToUpdate(true);

    //    Page.Validate();

    //    if (Page.IsValid)
    //    {
    //        int docAppId = -1;
    //        string refNo = string.Empty;

    //        if (!string.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()))
    //        {
    //            if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue))
    //            {
    //                docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
    //                refNo = RefNoRadComboBox.Text.Trim();
    //            }
    //            else
    //            {
    //                RadComboBoxItem selectedItem = RefNoRadComboBox.FindItemByText(RefNoRadComboBox.Text.Trim());

    //                if (selectedItem != null)
    //                {
    //                    docAppId = int.Parse(selectedItem.Value);
    //                }
    //                else
    //                {
    //                    DocAppDb docAppDb = new DocAppDb();
    //                    DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

    //                    if (docAppTable.Rows.Count > 0)
    //                    {
    //                        DocApp.DocAppRow docAppRow = docAppTable[0];
    //                        docAppId = docAppRow.Id;
    //                        refNo = docAppRow.RefNo;
    //                    }
    //                }
    //            }
    //        }

    //        if (docAppId != -1)
    //        {
    //            // Insert the reference record
    //            SetAppDb setAppDb = new SetAppDb();

    //            // Check if current link exists
    //            if (!setAppDb.DoesLinkExists(setId.Value, docAppId))
    //            {
    //                setAppDb.Insert(setId.Value, docAppId);

    //                // Log the action
    //                LogActionDb logActionDb = new LogActionDb();
    //                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_added_as_reference_number, refNo, string.Empty,
    //                    string.Empty, string.Empty, LogTypeEnum.S, setId.Value);

    //                ReferenceNoRadGrid.Rebind();
    //                //ConfirmRefNoPanel.Visible = true;

    //                PopulateTreeView();
    //            }
    //            else
    //            {
    //                ScriptManager.RegisterStartupScript(this.Page,
    //                    this.GetType(),
    //                    "SelectReferencDuplicateScript",
    //                    "alert('The reference number is already associated with the set.');",
    //                    true);
    //            }
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterStartupScript(this.Page,
    //                this.GetType(),
    //                "SelectReferencScript",
    //                "alert('Please select a reference number.');",
    //                true);
    //        }
    //    }

    //    DocSetDb docSetDb = new DocSetDb();
    //    LeftConfirmButton.Enabled = docSetDb.AllowVerificationSaveDate(setId.Value);
    //}

    protected void AddReferenceButton_Click(object sender, EventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(true);

        Page.Validate();

        if (Page.IsValid)
        {
            int docAppId = -1;
            string refNo = string.Empty;

            if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
            {
                if (!string.IsNullOrEmpty(DocAppHiddenValue.Value))
                {
                    docAppId = int.Parse(DocAppHiddenValue.Value);
                    refNo = DocAppRadTextBox.Text.Trim();
                }
                else
                {                  
                    DocAppDb docAppDb = new DocAppDb();
                    DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());

                    if (docAppTable.Rows.Count > 0)
                    {
                        DocApp.DocAppRow docAppRow = docAppTable[0];
                        docAppId = docAppRow.Id;
                        refNo = docAppRow.RefNo;
                    }                    
                }
            }

            if (docAppId != -1)
            {
                // Insert the reference record
                SetAppDb setAppDb = new SetAppDb();

                // Check if current link exists
                if (!setAppDb.DoesLinkExists(setId.Value, docAppId))
                {
                    setAppDb.Insert(setId.Value, docAppId);

                    // Log the action
                    LogActionDb logActionDb = new LogActionDb();
                    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_added_as_reference_number, refNo, string.Empty,
                        string.Empty, string.Empty, LogTypeEnum.S, setId.Value);

                    ReferenceNoRadGrid.Rebind();
                    //ConfirmRefNoPanel.Visible = true;

                    PopulateTreeView();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page,
                        this.GetType(),
                        "SelectReferencDuplicateScript",
                        "alert('The reference number is already associated with the set.');",
                        true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page,
                    this.GetType(),
                    "SelectReferencScript",
                    "alert('Please select a reference number.');",
                    true);
            }
        }

        DocSetDb docSetDb = new DocSetDb();
        LeftConfirmButton.Enabled = docSetDb.AllowVerificationSaveDate(setId.Value);
    }
    #endregion

    #region Tree View Events
    protected void RadTreeView1_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        //Added try catch by Edward 2015/8/21 Reduce Error Notifications Object reference not set to an instance of an object in TreeNodeClick        
        try
        {
            if (RadTreeView1.SelectedNode == null)      //redirect to view.aspx again to load the RadTreeView by Edward
                RedirectToDefaultPage();

            if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "set" || RadTreeView1.SelectedNode.Category.Trim().ToLower() == "doc")
            {                
                if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "set")
                {
                    PopulateSetInfo();
                    //set button font and enable
                    foreach (RadToolBarButton radToolBarButton in RadToolBarMenu.Items)
                    {
                        radToolBarButton.Font.Bold = false;
                        if (!radToolBarButton.Text.ToLower().Equals("log"))
                            radToolBarButton.Enabled = false;
                    }                    
                }
                else
                {                    
                    //set button font and enable
                    foreach (RadToolBarButton radToolBarButton in RadToolBarMenu.Items)
                    {
                        radToolBarButton.Enabled = true;
                    }

                    MetaDataPanel.Visible = RotateClockwiseButton.Enabled = ExtractButton.Enabled = true;
                    ExtractButton.Enabled = RotateClockwiseButton.Enabled = !isSetConfirmed;
                    ThumbnailPanel.Visible = true;
                    SetInfoPanel.Visible = LogPanel.Visible = false;                    
                    RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (RadTreeView1.SelectedNode.Attributes["Status"].ToString().ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
                    RightButtonsPanel.Visible = MetaDataButtonPanel.Visible = pdfframe.Visible = true;
                    LoadMetaData();                    
                }
                SetUserSelectionCookie(string.Empty, RadTreeView1.SelectedValue);
            }
            else
            {                
                RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text;
                MetaDataPanel.Visible = LogPanel.Visible = SetInfoPanel.Visible = false;
                RotateClockwiseButton.Enabled = ExtractButton.Enabled = false;
            }            
            ExportButton.Visible = ThumbnailPanel.Visible = false;
            ResetPageClosingFlags();
            selectedNodeGlobal = RadTreeView1.SelectedValue;            
            if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "set")
            {                
                if (GetUserSelectionSelectedTab().ToUpper().Equals("Log"))
                    LoadContextContent(true);
            }
            else if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "doc")
                LoadContextContent(true);
        }
        catch (System.Threading.ThreadAbortException)        //Added By Edward to take out Thread was being aborted 2016/04/12
        {
            // do nothing
        }
        catch (Exception ex)
        {            
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - RadTreeView1_NodeClick", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
        }        
    }

    protected void RadTreeView1_NodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
    {
        //Added By Try Catch Error Log by Edward 27.10.2013
        try
        {
            RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

            // Fetch event data 
            RadTreeNode sourceNode = e.SourceDragNode;
            RadTreeNode destNode = e.DestDragNode;

            string srcFolderName = e.SourceDragNode.ParentNode.Text;
            string desFolderName = e.DestDragNode.Text;
            #region modified by Edward 2017/11/10 optimize tree view
            //if (!destNode.Category.Trim().ToUpper().Equals("DOC"))
            //{
            //    string[] srcFolderNameArray = srcFolderName.Split(')');
            //    srcFolderName = srcFolderNameArray[1].Trim();
            //    string[] desFolderNameArray = desFolderName.Split(')');
            //    desFolderName = desFolderNameArray[1].Trim();
            //}

            if (!destNode.Category.Trim().ToUpper().Equals("DOC"))
            {
                string[] srcFolderNameArray = srcFolderName.Split(')');
                srcFolderName = srcFolderNameArray[1].Trim();
                string[] desFolderNameArray = desFolderName.Split(')');
                desFolderName = desFolderNameArray[1].Trim();
            }
            #endregion

            #region within the treenode
            //proceed only if dest node is different.
            if (sourceNode.ParentNode != destNode)
            {
                int docId = int.Parse(sourceNode.Value);
                DocDb docDb = new DocDb();
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                DocSetDb docSetDb = new DocSetDb();

                LogActionDb logActionDb = new LogActionDb();

                //if drop into DEFAULT FOLDER
                if (destNode.Category.Trim().ToUpper().Equals("DEFAULT FOLDER") || destNode.Category.Trim().ToUpper().Equals("UNIDENTIFIED"))
                {
                    #region Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    ////attach and dettach doc/app personal reference
                    //CustomPersonal customPersonal = new CustomPersonal();
                    //Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, true, docId, setId.Value, -1, destNode.Value);

                    ////log action
                    //if (isSuccess)
                    //{
                    //    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //    docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //}
                    #endregion

                    TreeView_NodeDropDefaultFolder(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("DEFAULT FOLDER NRIC")) // 
                {
                    #region Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    //if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                    //    && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    //{
                    //    string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

                    //    //attach and dettach doc/app personal reference
                    //    CustomPersonal customPersonal = new CustomPersonal();
                    //    Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, -1, destNode.ParentNode.Value.Trim());

                    //    //log action
                    //    if (isSuccess)
                    //    {
                    //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //    }
                    //}
                    #endregion

                    TreeView_NodeDropNRIC(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REFNO")) // if drop into REFNO Folder
                {
                    #region Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    //if (!(sourceNode.Attributes["DocTypeCode"].ToUpper().Trim().Equals(destNode.Attributes["DocTypeCode"].ToUpper().Trim())))
                    //{
                    //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", "This document cannot be put directly under the application folder."), true);
                    //}
                    //else
                    //{
                    //    //dettach doc/app personal reference
                    //    CustomPersonal customPersonal = new CustomPersonal();

                    //    customPersonal.AttachAppPersonalHouseholdReference(docId, int.Parse(destNode.Value), int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode);

                    //    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //    docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //}
                    #endregion

                    TreeView_NodeDropRefNo(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF NRIC")) // if drop into REFNO/NRIC Folder
                {
                    #region  Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    //if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                    //    && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    //{
                    //    string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

                    //    //attach and dettach doc/app personal reference
                    //    CustomPersonal customPersonal = new CustomPersonal();
                    //    Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Unidentified.ToString());

                    //    if (isSuccess)
                    //    {
                    //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //    }
                    //}
                    #endregion  

                    TreeView_NodeDropRefNoNRIC(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF OTHERS")) // if drop into REFNO/OTHERS Folder
                {
                    #region  Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    //if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                    //    && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    //{
                    //    //attach and dettach doc/app personal reference
                    //    CustomPersonal customPersonal = new CustomPersonal();

                    //    Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, false, docId, setId.Value, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Others.ToString());

                    //    if (isSuccess)
                    //    {
                    //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //    }
                    //}
                    #endregion

                    TreeView_NodeDropRefNoOthers(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF OTHERS NRIC")) // if drop into REFNO/OTHERS/NRIC Folder
                {
                    #region  Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    //if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                    //    && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    //{
                    //    string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

                    //    //attach and dettach doc/app personal reference
                    //    CustomPersonal customPersonal = new CustomPersonal();
                    //    Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, int.Parse(destNode.ParentNode.ParentNode.Value), DocFolderEnum.Others.ToString());

                    //    if (isSuccess)
                    //    {
                    //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                    //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    //    }
                    //}
                    #endregion

                    TreeView_NodeDropRefNoOtherNRIC(docId, sourceNode, destNode, srcFolderName, desFolderName);
                }
                else if (destNode.Category.Trim().ToUpper().Equals("DOC")) // if document is drop onto a document >> merge action
                {
                    #region Modified by Edward Separate the Treeview Node Drop to modules 2017/11/14
                    ////proceed only if both the source and destination documents still exist.
                    //int desctDocId = int.Parse(destNode.Value);
                    //Boolean isSourceAndDestinationDocumentExist = false;

                    //if (docDb.GetDocById(docId).Rows.Count > 0)
                    //    isSourceAndDestinationDocumentExist = true;

                    //if (docDb.GetDocById(desctDocId).Rows.Count > 0)
                    //    isSourceAndDestinationDocumentExist = true;

                    //if (isSourceAndDestinationDocumentExist)
                    //{
                    //    //copy all the source document raw pages to the destination document
                    //    RawPageDb rawPageDb = new RawPageDb();
                    //    //RawPage.RawPageDataTable rawPagesDest = rawPageDb.GetRawPageByDocId(desctDocId);

                    //    RawPage.RawPageDataTable rawPagesSrc = rawPageDb.GetRawPageByDocId(docId);

                    //    // Reorder the pages
                    //    int docPageNo = RawPageDb.ReorderRawPages(rawPagesSrc, desctDocId);

                    //    //int pageNo = rawPageDb.GetMaxDocPageNo(desctDocId) + 1;
                    //    //foreach (RawPage.RawPageRow rawPage in rawPagesSrcDt.Rows)
                    //    //{
                    //    //    rawPageDb.Update(rawPage.Id, desctDocId, pageNo);
                    //    //    pageNo++;
                    //    //}

                    //    rawPagesSrc = rawPageDb.GetRawPageByDocId(docId);

                    //    if (rawPagesSrc.Rows.Count <= 0)
                    //    {
                    //        // delete the old document.
                    //        docDb.Delete(docId);
                    //    }

                    //    SetUserSelectionCookie(string.Empty, desctDocId.ToString());

                    //    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_merged_from_REPLACE1_to_REPLACE2, sourceNode.Text, destNode.Text, string.Empty, string.Empty, LogTypeEnum.D, desctDocId);
                    //    docDb.UpdateDocStatus(desctDocId, DocStatusEnum.Pending_Verification);
                    //    docDb.UpdateCmDocId(desctDocId, null);
                    //    docDb.UpdateSendToCdbStatus(desctDocId, SendToCDBStatusEnum.Ready);
                    //    docDb.UpdateSendToCdbAccept(desctDocId, SendToCDBStatusEnum.NotReady);
                    //    docDb.UpdateIsVerified(desctDocId, false);
                    //}

                    TreeView_NodeDropMerge(destNode.Value, docId, sourceNode.Text, destNode.Text);
                    #endregion
                }

                if (destNode.Category.Trim().ToUpper().Equals("DOC"))
                {
                    //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
                    //SetRightDocumentLabelText(int.Parse(destNode.Value));
                    SetRightDocumentLabelText(!string.IsNullOrEmpty(destNode.Value) ? destNode.Value : string.Empty);
                    SetUserSelectionCookie(string.Empty, destNode.Value);
                }
                else
                {
                    //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
                    //SetRightDocumentLabelText(int.Parse(sourceNode.Value));
                    SetRightDocumentLabelText(!string.IsNullOrEmpty(sourceNode.Value) ? sourceNode.Value : string.Empty);
                    SetUserSelectionCookie(string.Empty, sourceNode.Value);
                }

                PopulateTreeView();
                LoadMetaData();
                LoadContextContent(false);
            }

            #endregion

            ResetPageClosingFlags();
        }
        catch (Exception ex)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - RadTreeView1_NodeDrop", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex,string.Empty);
        }        
    }
    #endregion

    #region Added by Edward Separate the Treeview Node Drop to modules 2017/11/14
    private void TreeView_NodeDropDefaultFolder(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        //attach and dettach doc/app personal reference
        CustomPersonal customPersonal = new CustomPersonal();
        Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, true, docId, setId.Value, -1, destNode.Value);

        //log action
        if (isSuccess)
        {
            DocDb docDb = new DocDb();
            LogActionDb logActionDb = new LogActionDb();
            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
            docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
        }
    }

    private void TreeView_NodeDropNRIC(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
        {
            string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

            //attach and dettach doc/app personal reference
            CustomPersonal customPersonal = new CustomPersonal();
            Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, -1, destNode.ParentNode.Value.Trim());

            //log action
            if (isSuccess)
            {
                DocDb docDb = new DocDb();
                LogActionDb logActionDb = new LogActionDb();
                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            }
        }
    }

    private void TreeView_NodeDropRefNo(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        if (!(sourceNode.Attributes["DocTypeCode"].ToUpper().Trim().Equals(destNode.Attributes["DocTypeCode"].ToUpper().Trim())))
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", "This document cannot be put directly under the application folder."), true);
        }
        else
        {
            //dettach doc/app personal reference
            CustomPersonal customPersonal = new CustomPersonal();

            customPersonal.AttachAppPersonalHouseholdReference(docId, int.Parse(destNode.Value), int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode);
            DocDb docDb = new DocDb();
            LogActionDb logActionDb = new LogActionDb();
            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
            docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
        }
    }

    private void TreeView_NodeDropRefNoNRIC(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
        {
            string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

            //attach and dettach doc/app personal reference
            CustomPersonal customPersonal = new CustomPersonal();
            Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Unidentified.ToString());

            if (isSuccess)
            {
                DocDb docDb = new DocDb();
                LogActionDb logActionDb = new LogActionDb();
                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            }
        }
    }

    private void TreeView_NodeDropRefNoOthers(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
        {
            //attach and dettach doc/app personal reference
            CustomPersonal customPersonal = new CustomPersonal();

            Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, false, docId, setId.Value, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Others.ToString());

            if (isSuccess)
            {
                DocDb docDb = new DocDb();
                LogActionDb logActionDb = new LogActionDb();
                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            }
        }
    }
    private void TreeView_NodeDropRefNoOtherNRIC(int docId, RadTreeNode sourceNode, RadTreeNode destNode, string srcFolderName, string desFolderName)
    {
        if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
        {
            string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

            //attach and dettach doc/app personal reference
            CustomPersonal customPersonal = new CustomPersonal();
            Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId.Value, int.Parse(destNode.ParentNode.ParentNode.Value), DocFolderEnum.Others.ToString());

            if (isSuccess)
            {
                DocDb docDb = new DocDb();
                LogActionDb logActionDb = new LogActionDb();
                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.D, docId);
                docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            }
        }
    }

    private void TreeView_NodeDropMerge(string strDestNode, int docId, string sourceNode, string destNode)
    {
        //proceed only if both the source and destination documents still exist.
        int desctDocId = int.Parse(strDestNode);
        Boolean isSourceAndDestinationDocumentExist = false;

        DocDb docDb = new DocDb();

        if (docDb.GetDocById(docId).Rows.Count > 0)
            isSourceAndDestinationDocumentExist = true;

        if (docDb.GetDocById(desctDocId).Rows.Count > 0)
            isSourceAndDestinationDocumentExist = true;

        if (isSourceAndDestinationDocumentExist)
        {
            //copy all the source document raw pages to the destination document
            RawPageDb rawPageDb = new RawPageDb();
            RawPage.RawPageDataTable rawPagesSrc = rawPageDb.GetRawPageByDocId(docId);

            // Reorder the pages
            int docPageNo = RawPageDb.ReorderRawPages(rawPagesSrc, desctDocId);

            rawPagesSrc = rawPageDb.GetRawPageByDocId(docId);

            if (rawPagesSrc.Rows.Count <= 0)
            {
                // delete the old document.
                docDb.Delete(docId);
            }

            SetUserSelectionCookie(string.Empty, desctDocId.ToString());
            LogActionDb logActionDb = new LogActionDb();
            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_merged_from_REPLACE1_to_REPLACE2, sourceNode, destNode, string.Empty, string.Empty, LogTypeEnum.D, desctDocId);
            DocDb.UpdateDocFromNodeDrop(desctDocId);
        }
    }
    #endregion

    #region List View Events
    protected void RadListView1_ItemDrop(object sender, RadListViewItemDragDropEventArgs e)
    {
        RedirectIfSetConfirmedOrUserNotAllowedToUpdate(false);

        RawPageDb rawPageDb = new RawPageDb();
        DocDb docDb = new DocDb();

        // Dragged to the thumbnails view for sorting
        if (e.DestinationHtmlElement.IndexOf("ThumbnailRadListView") > -1 && e.DraggedItem != null)
        {
            // RawPage id of the dragged image
            int srcRawPageId = (int)e.DraggedItem.GetDataKeyValue("Id");
            int docId = int.Parse(RadTreeView1.SelectedValue);

            // Get the RawPage id of the destination
            int destRawPageId = -1;
            foreach (RadListViewDataItem item in ThumbnailRadListView.Items)
            {
                bool hasMatch = false;
                foreach (Control control in item.Controls)
                {
                    if (control.ClientID.Equals(e.DestinationHtmlElement))
                    {
                        destRawPageId = (int)item.GetDataKeyValue("Id");
                        hasMatch = true;
                        break;
                    }
                }

                if (hasMatch)
                    break;
            }

            // Get the pages nos of the two pages
            int srcDocPageNo = rawPageDb.GetDocPageNo(srcRawPageId);
            int destDocPageNo = rawPageDb.GetDocPageNo(destRawPageId);

            bool result = false;
            // Update the page numbers of all the raw pages between the destination and source raw page (excluding the source)
            if (srcDocPageNo > destDocPageNo)
                result = rawPageDb.UpdateDocPageNos(docId, destDocPageNo, srcDocPageNo - 1, false);
            else if (srcDocPageNo < destDocPageNo)
                result = rawPageDb.UpdateDocPageNos(docId, srcDocPageNo + 1, destDocPageNo, true);

            // Update the doc page no of the dragged item
            if (result)
                rawPageDb.UpdateDocPageNo(srcRawPageId, destDocPageNo);

            //update doc status
            docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //update isverified
            docDb.UpdateIsVerified(docId, false);

            // Reload the controls
            //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
            //SetRightDocumentLabelText(docId);
            SetRightDocumentLabelText(docId.ToString());

            PopulateTreeView();
            SetUserSelectionCookie(string.Empty, docId.ToString());
            LoadContextContent(false);
        }
        // Dragged to tree view for moving the page
        else if (!string.IsNullOrEmpty(NodeValueHiddenField.Value) && e.DraggedItem != null)
        {
            //[[[[[[[[[[[[[[[OPTION DISABLED]]]]]]]]]]]]]]]]]]]]]]

            //// RawPage id of the dragged image
            //int rawPageId = (int)e.DraggedItem.GetDataKeyValue("Id");
            //int docId = int.Parse(RadTreeView1.SelectedValue);
            //string destFolder = NodeValueHiddenField.Value;
            ////string destCategory = NodeCategoryHiddenField.Value;

            //RadTreeNode destNode = RadTreeView1.FindNodeByValue(destFolder);

            //int pageCount = rawPageDb.CountRawPageByDocId(docId);

            //// Get the Doc data                
            //Doc.DocDataTable docs = docDb.GetDocById(docId);

            //if (docs.Rows.Count > 0)
            //{
            //    Doc.DocRow docRow = docs[0];

            //    DocPersonalDb docPersonalDb = new DocPersonalDb();
            //    string personalType = docPersonalDb.GetPersonalTypeByDocTypeCode(docRow.DocTypeCode);

            //    int docCopyId = -1;

            //    //if drop into DEFAULT FOLDER
            //    if (destNode.Category.Trim().ToUpper().Equals("DEFAULT FOLDER")) // if drop into DEFAULT FOLDER Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            destFolder, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("DFNRIC")) // if drop into DEFAULT FOLDER NRIC UNIDENTIFIED Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            destNode.ParentNode.Value, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //        string nric = destNode.Text.Trim();
            //        nric = Categorization.GetNricFromText(nric);

            //        string name = destNode.Text.Trim();
            //        if (name.IndexOf("-") >= 0)
            //            name = name.Substring(name.IndexOf("-") + 1, name.Length - name.IndexOf("-") - 4);
            //        else
            //            name = string.Empty;

            //        docPersonalDb.Insert(docCopyId, personalType, nric, name, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("UNIDENTIFIED")) // if drop into UNIDENTIFIED Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("INNRIC")) // if drop into REFNO/NRIC Folder
            //    {
            //        //string nric = destNode.Text.Trim();
            //        //nric = nric.Substring(nric.Length - 9, 9);

            //        string nric = destNode.Text.Trim();
            //        if (nric.IndexOf("-") >= 0)
            //            nric = nric.Substring(nric.IndexOf(":") + 1, nric.IndexOf("-") - nric.IndexOf(":") - 1).Trim();
            //        else
            //            nric = nric.Substring(nric.IndexOf(":") + 1, 9);

            //        string name = destNode.Text.Trim();
            //        if (name.IndexOf("-") >= 0)
            //            name = name.Substring(name.IndexOf("-") + 1, name.Length - name.IndexOf("-") - 4);
            //        else
            //            name = string.Empty;

            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, personalType, nric, name, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("REFNO")) // if drop into REFNO Folder
            //    {
            //        DocSetDb docSetDb = new DocSetDb();
            //        int docSetAppId = docSetDb.GetDocAppId(setId.Value);
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(activeDocType, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, "HA", string.Empty, string.Empty, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("DOC")) // if thumbnail is dropped onto a document >> sort of merge action
            //    {
            //        docCopyId = int.Parse(destNode.Value);
            //    }

            //    //only if a new document is created, proceed to the 
            //    if (docCopyId > 0 && docCopyId != -1)
            //    {
            //        // Get the RawPage data                        
            //        RawPage.RawPageDataTable rawPages = new RawPage.RawPageDataTable();
            //        rawPages = rawPageDb.GetRawPageById(rawPageId);

            //        // Reorder the pages
            //        int docPageNo = RawPageDb.ReorderRawPages(rawPages, docCopyId);

            //        rawPages = rawPageDb.GetRawPageByDocId(docId);

            //        int pageNo = 1;
            //        foreach (RawPage.RawPageRow rawPage in rawPages.Rows)
            //        {
            //            rawPageDb.Update(rawPage.Id, rawPage.DocId, pageNo);

            //            pageNo++;
            //        }

            //        rawPages = rawPageDb.GetRawPageByDocId(docId);

            //        // Delete the current doc record if it contains only 1 page
            //        if (rawPages.Rows.Count <= 0)
            //        {
            //            docDb.Delete(docId);
            //        }

            //        //update the status
            //        docDb.UpdateDocStatus(docCopyId, DocStatusEnum.Pending_Verification);

            //        if (destNode.Category.Trim().ToUpper().Equals("DOC"))
            //            docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //        //logaction
            //        LogActionDb logActionDb = new LogActionDb();
            //        DocTypeDb docTypeDb = new DocTypeDb();
            //        DocType.DocTypeDataTable docTypes = docTypeDb.GetDocType(docRow.DocTypeCode);
            //        DocType.DocTypeRow docTypeRow = docTypes[0];
            //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Classified_REPLACE1_in_REPLACE2_as_REPLACE3, "Page " + docPageNo, docTypeRow.Description + Dwms.Web.TreeviewDWMS.FormatId(docId.ToString()), destNode.Text, LogTypeEnum.D, docCopyId);

            //        // Reload the controls
            //        SetRightDocumentLabelText(docId);
            //        PopulateTreeView();
            //        SetUserSelectionCookie(string.Empty, docCopyId.ToString());
            //        LoadContextContent(false);
            //    }
            //}
        }
    }

    protected void RadListView1_NeedDataSource(object sender, Telerik.Web.UI.RadListViewNeedDataSourceEventArgs e)
    {
        if (RadTreeView1.SelectedNode.Category.ToLower().Equals("doc"))
        {
            int docId = int.Parse(RadTreeView1.SelectedValue);
            RawPageDb rawPageDb = new RawPageDb();
            ThumbnailRadListView.DataSource = rawPageDb.GetRawPageByDocId(docId);
        }
        else
        {
            RotateClockwiseButton.Visible = ThumbnailPanel.Visible = ExtractButton.Visible = false;
        }
    }

    protected void RadListView1_OnItemDataBound(object sender, Telerik.Web.UI.RadListViewItemEventArgs e)
    {
        RadListViewItemDragHandle RadListViewItemDragHandle1 = (RadListViewItemDragHandle)e.Item.FindControl("RadListViewItemDragHandle1");

        if (isSetConfirmed || !allowVerificationSaveDate)
        {
            RadListViewItemDragHandle1.Enabled = false;
            RadListViewItemDragHandle1.Visible = false;
        }

        try
        {
            RadListViewItem item = (RadListViewItem)e.Item;

            RadListViewDataItem dataItem = (RadListViewDataItem)item;

            RawPageDb rawPageDb = new RawPageDb();
            int rawPageId = int.Parse(dataItem.GetDataKeyValue("Id").ToString());

            #region New Implementation (RawPage folder)
            DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(rawPageId);

            if (rawPageDir.Exists)
            {
                HtmlImage ThumbnailImg = (HtmlImage)e.Item.FindControl("ThumbnailImg");

                if (ThumbnailImg != null)
                {
                    FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_th.jpg");
                    FileInfo[] rawPageFiles2 = rawPageDir.GetFiles("*_th.jpeg");

                    if (rawPageFiles.Length > 0 || rawPageFiles2.Length > 0)
                        ThumbnailImg.Src = "../Common/DownloadThumbnail.aspx?id=" + rawPageId.ToString();
                    else
                        ThumbnailImg.Src = "../Data/Images/Question.gif";
                }
            }
            #endregion
        }
        catch (Exception)
        {
        }
    }
    #endregion
    #endregion

    #region Private Methods

    protected void RedirectIfSetConfirmedOrUserNotAllowedToUpdate(Boolean checkWithoutHouseholdStructure)
    {
        DocSetDb docSetDb = new DocSetDb();
        isSetConfirmed = docSetDb.IsSetConfirmed(setId.Value);

        if (isSetConfirmed)
            RedirectToDefaultPage();

        if (checkWithoutHouseholdStructure)
        {
            allowVerificationSaveDateWithoutHouseholdCheck = docSetDb.AllowVerificationSaveDateWithoutHouseholdCheck(setId.Value);

            if (!allowVerificationSaveDateWithoutHouseholdCheck)
                RedirectToDefaultPage();
        }
        else
        {
            allowVerificationSaveDate = docSetDb.AllowVerificationSaveDate(setId.Value);

            if (!allowVerificationSaveDate)
                RedirectToDefaultPage();
        }
    }

    protected void RedirectToDefaultPage()
    {
        Response.Redirect("view.aspx?id=" + setId.Value);
        return;
    }

    private void LoadContextContent(Boolean fromTreeNode)
    {
        if (Request.Cookies["SET" + setId.ToString()] != null)
        {
            HttpCookie userSelectionCookie = Request.Cookies["SET" + setId.ToString()];

            if (!fromTreeNode)
                RadTreeView1_NodeClick(null, null);

            string selectedTab = userSelectionCookie["selectedTab"] == null ?
                string.Empty : userSelectionCookie["SelectedTab"].ToString().ToUpper().Trim();

            switch (selectedTab)
            {
                case "THUMBNAILS":
                    ThumbnailButton_Click(null, EventArgs.Empty);
                    break;
                case "IMAGE":
                    ImgButton_Click(null, EventArgs.Empty);
                    break;
                case "LOG":
                    LogButton_Click(null, EventArgs.Empty);
                    break;
                default:
                    //Added by if condition Edward 2015/8/21 Reduce Error Notifications Object reference not set to an instance of an object in TreeNodeClick
                    if (RadTreeView1.SelectedNode != null)
                    {
                        if (!(RadTreeView1.SelectedNode.Category.Trim().ToLower() == "set"))
                            ImgButton_Click(null, null);
                    }
                    break;
            }
        }
    }

    private string GetUserSelectionSelectedNode()
    {
        if (Request.Cookies["SET" + setId.ToString()] != null)
        {
            HttpCookie userSelectionCookie = Request.Cookies["SET" + setId.ToString()];
            return userSelectionCookie.Values["SelectedNode"].ToString();
        }
        else
            return "0";
    }

    private string GetUserSelectionSelectedTab()
    {
        if (Request.Cookies["SET" + setId.ToString()] != null)
        {
            HttpCookie userSelectionCookie = Request.Cookies["SET" + setId.ToString()];
            return userSelectionCookie.Values["SelectedTab"].ToString();
        }
        else
            return string.Empty;
    }

    private void SetUserSelectionCookie(string selectedTab, string selectedNode)
    {
        if (Request.Cookies["SET" + setId.ToString()] == null)
        {
            HttpCookie userSelectionCookie = new HttpCookie("SET" + setId.ToString());

            if (!string.IsNullOrEmpty(selectedTab))
                userSelectionCookie.Values["SelectedTab"] = selectedTab.ToUpper().Trim();
            else
                userSelectionCookie.Values["SelectedTab"] = string.Empty;

            if (!string.IsNullOrEmpty(selectedNode))
                userSelectionCookie.Values["SelectedNode"] = selectedNode;
            else
                userSelectionCookie.Values["SelectedNode"] = "0";

            userSelectionCookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(userSelectionCookie);
        }
        else
        {
            HttpCookie userSelectionCookie = Request.Cookies["SET" + setId.ToString()];
            if (!string.IsNullOrEmpty(selectedTab))
            {
                userSelectionCookie.Values["SelectedTab"] = selectedTab.ToUpper().Trim();
                selectedTabGlobal = selectedTab;
            }
            else
            {
                if (RadTreeView1.SelectedNode != null)
                {
                    if (RadTreeView1.SelectedNode.Category.ToLower().Trim().Equals("set"))
                    {
                        userSelectionCookie.Values["SelectedTab"] = string.Empty;
                        selectedTabGlobal = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrEmpty(selectedNode))
                userSelectionCookie.Values["SelectedNode"] = selectedNode;

            Response.AppendCookie(userSelectionCookie);
        }
    }

    private void PopulateTreeView()
    {
        //Added Try Catch Error Log By Edward 27.10.2013
        try
        {
            DocSetDb docSetDb = new DocSetDb();

            TreeviewDWMS.PopulateTreeView(RadTreeView1, setId.Value, false, false);

            //we need to set this for remember the treeview node expanded state
            // http://www.telerik.com/community/code-library/aspnet-ajax/treeview/save-the-expanded-state-of-the-treenodes-when-the-treeview-is-being-bound-upon-each-postback.aspx
            RadTreeView1.DataValueField = "TreeviewID";

            selectedNodeGlobal = GetUserSelectionSelectedNode();

            #region handle treeview node expansion state from cookie
            HttpCookie collapsedNodeCookie = Request.Cookies["set" + setId.Value + "collapsedNodes"];
            if (collapsedNodeCookie != null)
            {
                string[] collapseNodeValues = collapsedNodeCookie.Value.Split('*');
                foreach (string nodeValue in collapseNodeValues)
                {
                    RadTreeNode collapseNode = RadTreeView1.FindNodeByValue(HttpUtility.UrlDecode(nodeValue));
                    if (collapseNode != null)
                        collapseNode.Expanded = false;
                }
            }
            else
                RadTreeView1.ExpandAllNodes();
            #endregion

            #region handle selected node
            if (selectedNodeGlobal.Equals("0"))
                RadTreeView1.Nodes[0].Selected = true;
            else
            {
                RadTreeNode SelNode = RadTreeView1.FindNodeByValue(selectedNodeGlobal);

                if (SelNode != null)
                {
                    SelNode.Selected = true;
                    RadTreeView1.DataBind();
                    RadTreeView1.DataValueField = "TreeviewID";
                    SelNode.ExpandParentNodes();
                }
                else
                    RadTreeView1.Nodes[0].Selected = true;
            }
            #endregion

            #region lock node drag when set is verified or user do not have access to make changes to the set
            bool lockDrag = allowVerificationSaveDate;
            if (isSetConfirmed)
                lockDrag = false;
            foreach (RadTreeNode node in RadTreeView1.GetAllNodes())
            {
                if (node.AllowDrag == true)
                    node.AllowDrag = lockDrag;
            }
            #endregion
        }
        catch (Exception e)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = e.Message + "<br><br>" + e.InnerException + "<br><br>" + e.StackTrace;
            errorLogDb.Insert("Verification/View.aspx - PopulateTreeView", errorMessage);
        }        
    }

    protected void LoadMetaData()
    {
        // Modified By edward to check Input string was not in a correct format 2017 / 11 / 10
        //PopulateDocumentTypes(int.Parse(RadTreeView1.SelectedValue));
        //PopulateDocData(int.Parse(RadTreeView1.SelectedValue));
        //PopulateMetaDataByDocId(int.Parse(RadTreeView1.SelectedValue));
        //PopulateAdditionalMetaData(int.Parse(RadTreeView1.SelectedValue));
        //DisplayPdfDocument(int.Parse(RadTreeView1.SelectedValue));
        PopulateDocumentTypes(RadTreeView1.SelectedValue);        
        PopulateDocData(RadTreeView1.SelectedValue);
        PopulateMetaDataByDocId(RadTreeView1.SelectedValue);
        PopulateAdditionalMetaData(RadTreeView1.SelectedValue);
        DisplayPdfDocument(RadTreeView1.SelectedValue);
    }

    //            if (node.Attributes["DocTypeCode"] != null)
    //            {
    //                if (node.Attributes["DocTypeCode"].ToString().Trim().ToLower() == "unidentified")
    //                {
    //                    hasUnidentifiedFiles = true;
    //                    break;
    //                }
    //            }

    // private void PopulateAdditionalMetaData(int docId)
    private void PopulateAdditionalMetaData(string strDocId)
    {
        int docId = int.Parse(strDocId);
        DocDb docDb = new DocDb();
        Doc.DocDataTable doc = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

        if (doc.Rows.Count > 0)
        {
            Doc.DocRow docRow = doc[0];

            //get the reference to the control
            UserControl userControl = AdditionalMetaDataPanel.FindControl(docRow.DocTypeCode + "MetaDataUC") as UserControl;
            SetControlVisibility(userControl);
            LoadSaveControlData(docRow.DocTypeCode, true, false, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());
        }
    }

    private void SetControlVisibility(UserControl userControl)
    {
        foreach (Control item in AdditionalMetaDataPanel.Controls)
        {
            if (item == userControl)
                item.Visible = true;
            else
                item.Visible = false;
        }
    }

    private bool LoadSaveControlData(string docTypeCode, bool isLoad, bool isValidate, int docRefId, string referencePersonalTable)
    {
        bool validDate = true;
        int docId = int.Parse(RadTreeView1.SelectedValue);
        string currentDocType2 = docTypeCode.ToLower().Trim();

        bool IsBlurOrIncomplete = ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"); //Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16

        //set isUnder household structure nric
        Boolean isUnderHouseholdStructureNRIC = false;
        RadTreeNode docNode = RadTreeView1.FindNodeByValue(docId.ToString().Trim());

        if (docNode != null)
        {
            RadTreeNode docParentNode = docNode.ParentNode;
            if (docParentNode.Category.ToLower().Trim().Equals("ref nric"))
                isUnderHouseholdStructureNRIC = true;
        }

        switch (currentDocType2)
        {
            case "hle":
                HLEMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    HLEMetaDataUC.LoadData(docId);
                else
                    HLEMetaDataUC.SaveData(docId);
                break;
            case "sers":
                SERSMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    SERSMetaDataUC.LoadData(docId);
                else
                    SERSMetaDataUC.SaveData(docId);
                break;
            case "sales":
                SalesMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    SalesMetaDataUC.LoadData(docId);
                else
                    SalesMetaDataUC.SaveData(docId);
                break;
            case "resale":
                ResaleMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ResaleMetaDataUC.LoadData(docId);
                else
                    ResaleMetaDataUC.SaveData(docId);
                break;
            case "adoption":
                AdoptionMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    AdoptionMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    AdoptionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    //validDate = AdoptionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "bankstatement":
                BankStatementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BankStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = BankStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "baptism":
                BaptismMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BaptismMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //BaptismMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = BaptismMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "birthcertificate":
                BirthCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BirthCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    BirthCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "birthcertificatchild":
                BirthCertificatChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BirthCertificatChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    BirthCertificatChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "birthcertsibling":
                BirthCertSiblingMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BirthCertSiblingMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    BirthCertSiblingMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "businessprofile":
                BusinessProfileMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BusinessProfileMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //BusinessProfileMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = BusinessProfileMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "cbr":
                CBRMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CBRMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "commissionstatement":
                CommissionStatementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CommissionStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CommissionStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "cpfcontribution":
                CPFContributionMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFContributionMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CPFContributionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "cpfstatement":
                CPFStatementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CPFStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "cpfstatementrefund":
                CPFStatementRefundMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFStatementRefundMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    CPFStatementRefundMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = CPFStatementRefundMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificate":
                DeathCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificateexsp":
                DeathCertificateEXSPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateEXSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateEXSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificatefa":
                DeathCertificateFaMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateFaMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateFaMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificatemo":
                DeathCertificateMoMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateMoMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateMoMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #region Added by Edward 2017/03/30 New Document Types
            case "deathcertificatepa":
                DeathCertificatePAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificatePAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificatePAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificatelo":
                DeathCertificateLOMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateLOMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateLOMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #endregion
            case "deathcertificatenric":
                DeathCertificateNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deathcertificatesp":
                DeathCertificateSPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "declaraincomedetails":
                DeclaraIncomeDetailsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeclaraIncomeDetailsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DeclaraIncomeDetailsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeclaraIncomeDetailsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "declarationprivprop":
                DeclarationPrivPropMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeclarationPrivPropMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DeclarationPrivPropMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeclarationPrivPropMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deedpoll":
                DeedPollMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedPollMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedPollMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedPollMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "deedseparation":
                DeedSeparationMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeparationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #region Added by Edward 2017/03/30 New Document Types
            case "deedseparationnric":
                DeedSeparationNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeparationNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeparationNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #endregion
            case "deedseverance":
                DeedSeveranceMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeveranceMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeveranceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeveranceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertificate":
                DivorceCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertchild":
                DivorceCertChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertexspouse":
                DivorceCertExSpouseMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertExSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertExSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertExSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertfather":
                DivorceCertFatherMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertFatherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertFatherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertFatherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertmother":
                DivorceCertMotherMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertMotherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertMotherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertMotherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcecertnric":
                DivorceCertNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcedocinitial":
                DivorceDocInitialMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocInitialMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocInitialMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocInitialMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcedocinterim":
                DivorceDocInterimMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocInterimMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "divorcedocfinal":
                DivorceDocFinalMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocFinalMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocFinalMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocFinalMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "doceduinstitute":
                DocEduInstituteMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DocEduInstituteMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DocEduInstituteMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "docoffincommitment":  //Added By Edward 2015/05/18 New Document Types
                DocOfFinCommitmentMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DocOfFinCommitmentMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DocOfFinCommitmentMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "employmentpass":
                EmploymentPassMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EmploymentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = EmploymentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "employmentletter":
                EmploymentLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EmploymentLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //EmploymentLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = EmploymentLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "entrypermit":
                EntryPermitMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EntryPermitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = EntryPermitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "gla":
                GLAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    GLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    GLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = GLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "identitycard":
                IdentityCardMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IdentityCardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "identitycardchild":  //Added By Edward 2015/05/18   New Document Types
                IdentityCardChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #region Added by Edward 2017/03/30 New Document Types
            case "identitycardnric":  
                IdentityCardNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #endregion
            #region Added by Edward 2017/12/22 New Document Types Identity Card Father Mother
            case "identitycardfa":
                IdentityCardFaMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardFaMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else                    
                    validDate = IdentityCardFaMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "identitycardmo":
                IdentityCardMoMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardMoMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = IdentityCardMoMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #endregion
            case "irasassesement":
                IRASAssesementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IRASAssesementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IRASAssesementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IRASAssesementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "irasir8e":
                IRASIR8EMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IRASIR8EMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IRASIR8EMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IRASIR8EMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "lettersolicitorpoa":
                LettersolicitorPOAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LettersolicitorPOAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LettersolicitorPOAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LettersolicitorPOAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "licenseoftrade":
                LicenseofTradeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LicenseofTradeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LicenseofTradeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LicenseofTradeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "loanstatementsold":
                LoanStatementSoldMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LoanStatementSoldMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LoanStatementSoldMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LoanStatementSoldMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "marriagecertificate":
                MarriageCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "marriagecertchild":
                MarriageCertChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "marriagecertltspouse":
                MarriageCertLtSpouseMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertLtSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertLtSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "marriagecertparent":
                MarriageCertParentMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertParentMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertParentMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "marriagecertsibling": //Added By Edward 2015/05/18 New Document Types
                MarriageCertSiblingMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertSiblingMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertSiblingMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "meddocudoctorletters":  //Added By Edward 2015/05/25 New Document Types
                MedDocuDoctorLettersMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MedDocuDoctorLettersMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = MedDocuDoctorLettersMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "mortgageloanform":
                MortgageLoanFormMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MortgageLoanFormMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    MortgageLoanFormMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "noloannotification":
                NoLoanNotificationMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NoLoanNotificationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    NoLoanNotificationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    //validDate = NoLoanNotificationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "noticeoftransfer":
                NoticeofTransferMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NoticeofTransferMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //NoticeofTransferMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NoticeofTransferMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "notessyariahcourt":  //Added By Edward 2015/05/18   New Document Types
                NotesSyariahCourtMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NotesSyariahCourtMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NotesSyariahCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "nsenlistmentnotice":  //Added By Edward 2015/05/18   New Document Types
                NSEnlistmentNoticeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NSEnlistmentNoticeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NSEnlistmentNoticeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "nsordcertificate":  //Added By Edward 2015/05/18   New Document Types
                NSORDCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NSORDCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NSORDCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            //case "nsidcard":
            //    NSIDcardMetaDataUC.CurrentDocType = currentDocType2;
            //    if (isLoad)
            //        NSIDcardMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        NSIDcardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
            //    break;
            case "officialassignee":
                OfficialAssigneeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OfficialAssigneeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    OfficialAssigneeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    //validDate = OfficialAssigneeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "optionpurchase":  //Added By Edward 2015/05/18   New Document Types
                OptionPurchaseMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OptionPurchaseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = OptionPurchaseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "orderofcourt":
                OrderofCourtMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OrderofCourtMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //OrderofCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = OrderofCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "orderofcourtdivorce":
                OrderofCourtDivorceMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OrderofCourtDivorceMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //    DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = OrderofCourtDivorceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "overseasincome":
                OverseasIncomeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OverseasIncomeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = OverseasIncomeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "passport":
                PassportMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PassportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PassportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "payslip":
                PAYSLIPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PAYSLIPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PAYSLIPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "pensionerletter":
                PensionerLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PensionerLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PensionerLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PensionerLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "petitionforgla":
                PetitionforGLAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PetitionforGLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PetitionforGLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PetitionforGLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "powerattorney":
                PowerAttorneyMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PowerAttorneyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PowerAttorneyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PowerAttorneyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "prisonletter":
                PrisonLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PrisonLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PrisonLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PrisonLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "propertyquestionaire":
                PropertyQuestionaireMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyQuestionaireMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PropertyQuestionaireMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PropertyQuestionaireMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #region Added by Edward 2017/10/03 New Document Types Property Tax
            case "propertytax":
                PropertyTaxMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyTaxMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PropertyTaxMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "propertytaxnric":
                PropertyTaxNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyTaxNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PropertyTaxNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            #endregion

            case "purchaseagreement":
                PurchaseAgreementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PurchaseAgreementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PurchaseAgreementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PurchaseAgreementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "receiptsloanarrear":
                ReceiptsLoanArrearMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ReceiptsLoanArrearMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ReceiptsLoanArrearMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = ReceiptsLoanArrearMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "reconciliatundertakn":
                ReconciliatUndertaknMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ReconciliatUndertaknMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ReconciliatUndertaknMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = ReconciliatUndertaknMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "rentalarrears":
                RentalArrearsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    RentalArrearsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //RentalArrearsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = RentalArrearsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "socialvisit":
                SocialVisitMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    SocialVisitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = SocialVisitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "statementofaccounts":
                StatementofAccountsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatementofAccountsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatementofAccountsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "statementsale":
                StatementSaleMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatementSaleMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    StatementSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = StatementSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "statutorydeclaration":
                StatutoryDeclarationMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatutoryDeclarationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatutoryDeclarationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "statutorydeclgeneral":
                StatutoryDeclGeneralMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatutoryDeclGeneralMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatutoryDeclGeneralMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                    break;
            case "studentpass":
                StudentPassMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StudentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    StudentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = StudentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "valuationreport":
                ValuationReportMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ValuationReportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ValuationReportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = ValuationReportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            case "warranttoact":
                WarranttoActMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    WarranttoActMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    WarranttoActMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = WarranttoActMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate, IsBlurOrIncomplete);
                break;
            //case "spouseformsale":
            //    SpouseFormSaleMetaDataUC.CurrentDocType = currentDocType2;
            //    if (isLoad)
            //        SpouseFormSaleMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        SpouseFormSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
            //    break;
            //case "spouseformpurchase":
            //    SpouseFormPurchaseMetaDataUC.CurrentDocType = currentDocType2;
            //    if (isLoad)
            //        SpouseFormPurchaseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        SpouseFormPurchaseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
            //    break;
            default:
                EmptyMetaDataUC.Visible = true;
                EmptyMetaDataUC.CurrentDocType = currentDocType;
                DocDb docDb = new DocDb();
                Doc.DocDataTable docsEmpty = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));
                Doc.DocRow docEmptyRow = docsEmpty[0];
                EmptyMetaDataUC.ActualDocType = docEmptyRow.DocTypeCode.Trim();

                if (isLoad)
                    EmptyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    EmptyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
        }
        return validDate;
    }

    #region Modified by Edward Displaying Active DocTypes 2015/8/17   
    //private void PopulateDocumentTypes(int docId)
    //{
    //    DocDb docDb = new DocDb();
    //    string docTypeCode = string.Empty;

    //    Doc.DocDataTable doc = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

    //    if (doc.Rows.Count > 0)
    //    {
    //        Doc.DocRow docRow = doc[0];
    //        docTypeCode = docRow.DocTypeCode;

    //        DocTypeDb docTypeDb = new DocTypeDb();
    //        DocTypeDropDownList.DataSource = docTypeDb.GetDocType();

    //        DocTypeDropDownList.DataTextField = "Description";
    //        DocTypeDropDownList.DataValueField = "Code";
    //        DocTypeDropDownList.SelectedValue = docTypeCode;
    //        DocTypeDropDownList.DataBind();

    //        //remove the irrelavant application form
    //        List<string> applicationToKeep = new List<string>();
    //        AppPersonalDb appPersonalDb = new AppPersonalDb();
    //        DataTable appPersonals = appPersonalDb.GetAppPersonalReferenceDetailsByDocId(docRow.Id);

    //        //first check if the document is attached to any application [Docments inside the application folder]
    //        if (appPersonals.Rows.Count > 0)
    //            applicationToKeep.Add(appPersonals.Rows[0]["RefType"].ToString().ToUpper().Trim());
    //        else // if the document is not attached to any application, then get the applications related to the DocSet. [documents outside the application folder]
    //        {
    //            DocSetDb docSetDb = new DocSetDb();
    //            DataTable referenceDetails = docSetDb.GetReferenceDetailsById(setId.Value);

    //            foreach (DataRow referenceDetailsRow in referenceDetails.Rows)
    //            {
    //                applicationToKeep.Add(referenceDetailsRow["RefType"].ToString().Trim());
    //            }
    //        }

    //        foreach (ScanningTransactionTypeEnum ScanningTransactionType in ScanningTransactionTypeEnum.GetValues(typeof(ScanningTransactionTypeEnum)))
    //        {
    //            if (!applicationToKeep.Contains(ScanningTransactionType.ToString().ToUpper()))
    //                DocTypeDropDownList.Items.Remove(DocTypeDropDownList.Items.FindItemByValue(ScanningTransactionType.ToString()));
    //        }
    //    }
    //}

    // Modified By edward to check Input string was not in a correct format 2017 / 11 / 10
    //private void PopulateDocumentTypes(int docId)
    private void PopulateDocumentTypes(string strdocId)
    {
        int docId = int.Parse(strdocId);
        DocDb docDb = new DocDb();
        string docTypeCode = string.Empty;

        Doc.DocDataTable doc = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

        if (doc.Rows.Count > 0)
        {
            Doc.DocRow docRow = doc[0];
            docTypeCode = docRow.DocTypeCode;
            DocTypeDb docTypeDb = new DocTypeDb();
            

            #region Added by Edward 2015/8/26 To Address Selection Out Of Range. Checking if it is Active
            DocType.DocTypeDataTable docTypeDt;
            bool docTypeIsActive = true;
            string docTypeDesc = string.Empty;
            if (!string.IsNullOrEmpty(docTypeCode))
            {
                docTypeDt = docTypeDb.GetDocType(docTypeCode);
                if (docTypeDt.Rows.Count > 0)
                {
                    DocType.DocTypeRow docTypeR = docTypeDt[0];
                    docTypeIsActive = !docTypeR.IsIsActiveNull() ? docTypeR.IsActive : docTypeIsActive;
                    docTypeDesc = docTypeR.Description;
                }
            }


            if (docTypeIsActive)
            {
               DocTypeDropDownList.DataSource = docTypeDb.GetActiveDocTypes();
               DocTypeDropDownList.SelectedValue = docTypeCode;
            }
            else
            {

                if (!allowVerificationSaveDate)
                {
                    DocTypeDropDownList.DataSource = docTypeDb.GetDocType();
                    DocTypeDropDownList.Enabled = false;
                    DocTypeDropDownList.SelectedValue = docTypeCode;
                }
                else
                {
                    DocTypeDropDownList.DataSource = docTypeDb.GetActiveDocTypes();
                }
            }
            DocTypeDropDownList.DataTextField = "Description";
            DocTypeDropDownList.DataValueField = "Code";            
            DocTypeDropDownList.DataBind();
            #endregion      
            

            //remove the irrelavant application form
            List<string> applicationToKeep = new List<string>();
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            DataTable appPersonals = appPersonalDb.GetAppPersonalReferenceDetailsByDocId(docRow.Id);

            //first check if the document is attached to any application [Docments inside the application folder]
            if (appPersonals.Rows.Count > 0)
                applicationToKeep.Add(appPersonals.Rows[0]["RefType"].ToString().ToUpper().Trim());
            else // if the document is not attached to any application, then get the applications related to the DocSet. [documents outside the application folder]
            {
                DocSetDb docSetDb = new DocSetDb();
                DataTable referenceDetails = docSetDb.GetReferenceDetailsById(setId.Value);

                foreach (DataRow referenceDetailsRow in referenceDetails.Rows)
                {
                    applicationToKeep.Add(referenceDetailsRow["RefType"].ToString().Trim());
                }
            }

            foreach (ScanningTransactionTypeEnum ScanningTransactionType in ScanningTransactionTypeEnum.GetValues(typeof(ScanningTransactionTypeEnum)))
            {
                if (!applicationToKeep.Contains(ScanningTransactionType.ToString().ToUpper()))
                    DocTypeDropDownList.Items.Remove(DocTypeDropDownList.Items.FindItemByValue(ScanningTransactionType.ToString()));
            }
        }
    }

    #endregion

    //Modified By edward to check Input string was not in a correct format 2017/11/10
    //private void PopulateDocData(int docId)
    private void PopulateDocData(string strdocId)
    {
        int docId = int.Parse(strdocId);
        DocDb docDb = new DocDb();
        Doc.DocDataTable doc = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

        if (doc.Rows.Count > 0)
        {
            Doc.DocRow docRow = doc[0];

            currentDocType = docRow.DocTypeCode;

            ImageConditionDropDownList.Items.Clear();
            MasterListItemDb masterListItemDb = new MasterListItemDb();
            ImageConditionDropDownList.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Image_Condition.ToString().Replace("_", " "));
            ImageConditionDropDownList.DataTextField = "Name";
            ImageConditionDropDownList.DataValueField = "Name";
            ImageConditionDropDownList.DataBind();

            ImageConditionDropDownList.SelectedValue = docRow.ImageCondition.Trim();
            if (ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"))
                SubmitAndConfirmMDButton.CausesValidation = false;

            ImageConditionDropDownList.DataBind();

            string description = docRow.IsDescriptionNull() ? string.Empty : docRow.Description.Trim();

            if (string.IsNullOrEmpty(description))
            {
                TRImageDescription.Visible = false;
                ImageDescription.Text = string.Empty;
            }
            else
            {
                TRImageDescription.Visible = true;
                ImageDescription.Text = description;
            }
            
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            DataTable appPersonals = appPersonalDb.GetAppPersonalReferenceDetailsByDocId(docRow.Id);

            if (appPersonals.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(appPersonals.Rows[0]["RefType"].ToString().Trim()))
                {
                    RefNoHeadingLabel.Text = appPersonals.Rows[0]["RefType"].ToString() + " No";
                    RefNoValueLabel.Text = appPersonals.Rows[0]["RefNo"].ToString();
                    //June 10, 2013 Added by Edward
                    //if (docRow.IsImageAcceptedNull() || (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
                    if (docRow.IsImageAcceptedNull() || string.IsNullOrEmpty(docRow.ImageAccepted.Trim()) ||    //Added checking of empty 31/3/2014
                        (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
                    {
                        if (appPersonals.Rows[0]["RefType"].ToString().Trim() == "HLE")
                            AcceptanceRadioButtonList.ClearSelection();
                        else
                            AcceptanceRadioButtonList.SelectedValue = "Y";
                    }
                    else
                        AcceptanceRadioButtonList.SelectedValue = docRow.ImageAccepted.ToUpper();
                }
            }
            else
            {
                RefNoHeadingLabel.Text = "Reference No";
                RefNoValueLabel.Text = "NA ";
                if (docRow.IsImageAcceptedNull() || (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
                        AcceptanceRadioButtonList.ClearSelection();
                else
                    AcceptanceRadioButtonList.SelectedValue = docRow.ImageAccepted.ToUpper();
            }
            //June 10, 2013 Added by Edward
            //if (docRow.IsImageAcceptedNull() || (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
            if (docRow.IsImageAcceptedNull() || string.IsNullOrEmpty(docRow.ImageAccepted.Trim()) ||    //Added checking of empty 31/3/2014
                (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
            {
                #region Added By Edward 13/3/2014 Sales and Resale Changes
                DocAppDb docAppDb = new DocAppDb();
                DocApp.DocAppDataTable docApps = docAppDb.GetDocAppByDocSetId(setId.Value);
                if (docApps.Rows.Count > 0)
                {
                    DocApp.DocAppRow row = docApps[0];
                    if(row.RefType.ToUpper().Trim() == "HLE")
                        AcceptanceRadioButtonList.ClearSelection();
                    else
                        AcceptanceRadioButtonList.SelectedValue = "Y";
                }
                else
                    AcceptanceRadioButtonList.ClearSelection();
                #endregion

            }
            else
                AcceptanceRadioButtonList.SelectedValue = docRow.ImageAccepted.ToUpper();
        }
    }

    //private void PopulateMetaData(int docId)
    private void PopulateMetaDataByDocId(string strdocId)
    {
        int docId = int.Parse(strdocId);
        MetaDataDb metaDataDb = new MetaDataDb();
        MetaData.MetaDataDataTable metaData = metaDataDb.GetMetaDataByDocIdWithDummyFieldId(docId, false, true);
        MetaFieldRepeater.DataSource = metaData;
        MetaFieldRepeater.DataBind();

        MetaDataTable.Visible = false;

        if (metaData.Rows.Count > 0)
            MetaDataTable.Visible = true;
    }

    private void PopulateMetaData(string docTypeCode)
    {
        MetaFieldDb metaFieldDb = new MetaFieldDb();
        MetaField.MetaFieldDataTable metaFields = metaFieldDb.GetMetaFieldByDocTypeCodeWithDummyFieldValue(docTypeCode);

        MetaFieldRepeater.DataSource = metaFields;
        MetaFieldRepeater.DataBind();

        MetaDataTable.Visible = false;

        if (metaFields.Rows.Count > 0)
            MetaDataTable.Visible = true;
    }

    protected void SaveMetaData(Boolean isConfirm, Boolean isValidate)
    {
        bool validDate = true;
        int docId = int.Parse(RadTreeView1.SelectedValue);

        //save additional metadata
        //if(isValidate)
        validDate = LoadSaveControlData(DocTypeDropDownList.SelectedValue.Trim(), false, isValidate, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());
        //else
        //    validDate = LoadSaveControlData(DocTypeDropDownList.SelectedValue.Trim(), false, false, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());

        //save the meta data
        if (validDate)
        {
            Repeater metaFieldRepeater = MetaDataPanel.FindControl("MetaFieldRepeater") as Repeater;

            //proceed if atleast there is one metadata
            MetaDataDb metaDataDb = new MetaDataDb();
            if (metaFieldRepeater.Items.Count > 0)
            {
                if (currentDocType.ToLower().Trim().Equals(DocTypeDropDownList.SelectedValue.ToLower().Trim())) // if document type is not changed.
                {
                    foreach (RepeaterItem rItem in metaFieldRepeater.Items)
                    {
                        TextBox metaFieldTextBox = rItem.FindControl("MetaFieldTextBox") as TextBox;
                        HiddenField metaDataIdHiddenField = rItem.FindControl("MetadataIdHiddenField") as HiddenField;
                        metaDataDb.Update(int.Parse(metaDataIdHiddenField.Value), metaFieldTextBox.Text.Trim());
                    }
                }
                else //if doctype is changed
                {
                    //delete the old entried and add new metafileds.
                    metaDataDb.DeleteStampDataByDocID(docId);

                    foreach (RepeaterItem rItem in metaFieldRepeater.Items)
                    {
                        HiddenField metaDataIdHiddenField = rItem.FindControl("MetadataIdHiddenField") as HiddenField;
                        HiddenField MetaDataFieldNameHiddenField = rItem.FindControl("MetaDataFieldNameHiddenField") as HiddenField;
                        HiddenField MetaDataFieldIdHiddenField = rItem.FindControl("MetaDataFieldIdHiddenField") as HiddenField;
                        HiddenField MetaDataVerificationMandatoryHiddenField = rItem.FindControl("MetaDataVerificationMandatoryHiddenField") as HiddenField;
                        HiddenField MetaDataCompletenessMandatoryHiddenField = rItem.FindControl("MetaDataCompletenessMandatoryHiddenField") as HiddenField;
                        HiddenField MetaDataVerificationVisibleHiddenField = rItem.FindControl("MetaDataVerificationVisibleHiddenField") as HiddenField;
                        HiddenField MetaDataCompletenessVisibleHiddenField = rItem.FindControl("MetaDataCompletenessVisibleHiddenField") as HiddenField;
                        HiddenField MetaDataFixedHiddenField = rItem.FindControl("MetaDataFixedHiddenField") as HiddenField;
                        HiddenField MetaDataMaximumLengthHiddenField = rItem.FindControl("MetaDataMaximumLengthHiddenField") as HiddenField;

                        TextBox metaFieldTextBox = rItem.FindControl("MetaFieldTextBox") as TextBox;
                        if (string.IsNullOrEmpty(MetaDataFieldIdHiddenField.Value))
                            metaDataIdHiddenField.Value = MetaDataFieldIdHiddenField.Value = metaDataDb.Insert(docId, MetaDataFieldNameHiddenField.Value, metaFieldTextBox.Text, bool.Parse(MetaDataVerificationMandatoryHiddenField.Value), bool.Parse(MetaDataCompletenessMandatoryHiddenField.Value), bool.Parse(MetaDataVerificationVisibleHiddenField.Value), bool.Parse(MetaDataCompletenessVisibleHiddenField.Value), bool.Parse(MetaDataFixedHiddenField.Value), int.Parse(MetaDataMaximumLengthHiddenField.Value), true).ToString();
                        else
                            metaDataDb.Update(int.Parse(MetaDataFieldIdHiddenField.Value), metaFieldTextBox.Text);
                    }
                }
            }
            else
            {
                //delete the old entried and add new metafileds.
                // metaDataDb.DeleteStampDataByDocID(docId);
            }
        }

        DocDb docDb = new DocDb();

        //update doc details
        if (isConfirm && validDate)
        {
            docDb.UpdateDocFromMeta(docId, DocStatusEnum.Verified, ImageConditionDropDownList.SelectedValue, DocTypeDropDownList.SelectedValue.Trim(), true, LogTypeEnum.D);
            if (AcceptanceRadioButtonList.SelectedValue == "Y" || AcceptanceRadioButtonList.SelectedValue == "N" || AcceptanceRadioButtonList.SelectedValue == "X")
            {
                //June 10, 2013 Added by Edward
                docDb.UpdateDocDetails(docId, "", AcceptanceRadioButtonList.SelectedValue, DocStatusEnum.Completed);
                docDb.UpdateSendToCdbStatus(docId, SendToCDBStatusEnum.Ready);
                docDb.UpdateSendToCdbAccept(docId, SendToCDBStatusEnum.NotReady);
            }
            else
            {
                docDb.UpdateDocDetails(docId, "", AcceptanceRadioButtonList.SelectedValue, DocStatusEnum.Verified);
                docDb.UpdateSendToCdbStatus(docId, SendToCDBStatusEnum.Ready);
            }
        }
        else if (validDate)
        {
            docDb.UpdateDocFromMeta(docId, DocStatusEnum.Pending_Verification, ImageConditionDropDownList.SelectedValue, DocTypeDropDownList.SelectedValue.Trim(), false, LogTypeEnum.D);
            //June 10, 2013 Added by Edward
            docDb.UpdateDocDetails(docId, "", AcceptanceRadioButtonList.SelectedValue, DocStatusEnum.Pending_Verification);
        }

        currentDocType = DocTypeDropDownList.SelectedValue.Trim();

        //update set status to Verification_In_progress
        DocSetDb docSetDb = new DocSetDb();
        docSetDb.UpdateSetStatus(setId.Value, SetStatusEnum.Verification_In_Progress, false, false, LogActionEnum.None);

        //if the parent node is unidentified and if the nric is filled, update the default folder to OT (others folder)
        if (RadTreeView1.SelectedNode.ParentNode.Category.ToLower().Equals("unidentified"))
        {
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            DocPersonal.DocPersonalDataTable dt = docPersonalDb.GetDocPersonalByDocId(docId);

            bool hasNric = false;

            foreach (DocPersonal.DocPersonalRow dr in dt.Rows)
            {
                if (!dr.IsNricNull())
                {
                    if (dr.Nric.Trim().Length > 0)
                        hasNric = true;
                }
            }

            if (hasNric)
            {
                docDb.UpdateDocFolder(docId, "OT");
            }
        }

        selectedNodeGlobal = RadTreeView1.SelectedValue;
        if (validDate)
        {
            MetDataConfirmPanel.Visible = true;
            MetDataConfirmPanelLabel.Text = String.Format("The metadata has been {0} successfully.", (isConfirm ? "confirmed" : "saved"));
        }
        PopulateTreeView();
        ResetPageClosingFlags();
        //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
        //SetRightDocumentLabelText(docId);
        SetRightDocumentLabelText(docId.ToString());
        
    }

    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();
        RadTreeNode node = RadTreeView1.SelectedNode;

        if (Page.IsValid && node != null && Validation.IsInteger(node.Value))
        {
            int id = int.Parse(node.Value);

            PopulateTreeView();

            ResetPageClosingFlags();
        }
    }

    //private void DisplayPdfDocument(int docId)
    private void DisplayPdfDocument(string strdocId)
    {
        int docId = int.Parse(strdocId);
        int missingPages = 0;

        try
        {
            #region modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            //ArrayList pages = new ArrayList();

            //RawPageDb rawPageDb = new RawPageDb();
            //RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

            //for (int cnt = 0; cnt < rawPages.Count; cnt++)
            //{
            //    RawPage.RawPageRow rawPage = rawPages[cnt];

            //    #region New Implementation (RawPage folder)
            //    DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);

            //    if (rawPageDir.Exists)
            //    {
            //        FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_s.pdf");

            //        if (rawPageFiles.Length > 0)
            //        {
            //            pages.Add(rawPageFiles[0].FullName);
            //        }
            //    }
            //    #endregion
            //}

            ArrayList pages = RawPageDb.GetRawPagesToDisplayPDF(docId);

            #endregion

            //string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            string saveDir = Util.GetTempFolder();

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            string mergedFileName = Path.Combine(saveDir, docId.ToString() + "_" + Guid.NewGuid() + ".pdf");

            if (pages.Count > 0)
            {
                Util.MergePdfFiles(pages, mergedFileName);
            }
            else
            {
                try
                {
                    if (File.Exists(mergedFileName))
                    {
                        File.Delete(mergedFileName);
                    }
                }
                catch (Exception)
                {
                }

                pdfframe.Attributes["src"] = "NoFile.aspx";
            }

            if (File.Exists(mergedFileName))
            {
                pdfframe.Attributes["src"] = "../Common/DownloadMergedDocument.aspx?filePath=" + mergedFileName;

                PdfReader pdfReader = new PdfReader(mergedFileName);

                #region modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

                //if (rawPages.Count != pdfReader.NumberOfPages)
                //{
                //    missingPages = rawPages.Count - pdfReader.NumberOfPages;
                //    pdfReader.Close();
                //    throw new System.ArgumentException(Constants.ProblemLoadingPages);
                //}

                int totalPages = RawPageDb.GetRawPagesNoOfRecords(docId);

                if (totalPages != pdfReader.NumberOfPages)
                {
                    missingPages = totalPages - pdfReader.NumberOfPages;
                    pdfReader.Close();
                    throw new System.ArgumentException(Constants.ProblemLoadingPages);
                }
                else
                {
                    pdfReader.Close();   //Added by Edward for OOM 2017/09/21                    
                }
                #endregion

            }
            else //Added by Edward 28.10.2013
            {
                string msg = string.Format(Constants.ProblemLoadingPages, missingPages, missingPages == 1 ? string.Empty : "s", missingPages == 1 ? "is" : "are", missingPages == 1 ? "The" : "These", missingPages == 1 ? string.Empty : "s");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            }
            //
            //   throw new System.ArgumentException(Constants.ProblemLoadingPages);
        }
        catch (Exception e)
        {
            if (e.Message.Contains(Constants.ProblemLoadingPages))
            {
                string msg = string.Format(Constants.ProblemLoadingPages, missingPages, missingPages == 1 ? string.Empty : "s", missingPages == 1 ? "is" : "are", missingPages == 1 ? "The" : "These", missingPages == 1 ? string.Empty : "s");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            }
            else //Added By Edward 27.10.2013
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                string errorMessage = e.Message + "<br><br>" + e.InnerException + "<br><br>" + e.StackTrace;
                errorLogDb.Insert("Verification/View.aspx - DisplayPdfDocument", errorMessage);
                //#region Added By Edward 20140610 Fix displaying of error message for images   //Commented by Edward 2015/08/21 Reduce Error Notification
                //Session["ErrorFile"] = e.Message;
                //pdfframe.Attributes["src"] = "ErrorFile.aspx";
                //#endregion
            }            
        }
    }

    private void ResetPageClosingFlags()
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResetPageClosingFlags", "javascript:ResetFlags();", true);
    }

    protected void PopulateSetInfo()
    {
        //ThumbnailButton.Enabled = ImgButton.Enabled = 
        MetaDataPanel.Visible = RotateClockwiseButton.Enabled = ExtractButton.Enabled = false;
        LogPanel.Visible = ExportButton.Visible = ThumbnailPanel.Visible = false;
        RightButtonsPanel.Visible = true;
        SetInfoPanel.Visible = true;

        DocSetDb docSetDb = new DocSetDb();
        DataTable docSets = new DataTable();

        RadTabStrip1.SelectedIndex = 0;
        RadMultiPage1.SelectedIndex = 0;

        RawfileRadGrid.Rebind();
        ReferenceNoRadGrid.Rebind();

        #region Populate Page 1
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        DataTable allChannelMasterListItem = masterListItemDb.GetAllChannelMasterListItem();
        ChannelDropDownList.DataSource = allChannelMasterListItem;
        ChannelDropDownList.DataTextField = "Name";
        ChannelDropDownList.DataValueField = "Name";
        ChannelDropDownList.DataBind();
        #endregion

        docSets = docSetDb.GetDocSetById(setId.Value);

        if (docSets.Rows.Count > 0)
        {
            DataRow docSetRow = docSets.Rows[0];

            //set set information
            if (!string.IsNullOrEmpty(docSetRow["Channel"].ToString()))
                ChannelDropDownList.SelectedValue = docSetRow["Channel"].ToString();
            RemarkTextBox.Text = string.IsNullOrEmpty(docSetRow["Remark"].ToString()) ? string.Empty : docSetRow["Remark"].ToString();

            //set acknowledge
            AcknowledgeNumberLabel.Text = string.IsNullOrEmpty(docSetRow["AcknowledgeNumber"].ToString()) ? "-" : docSetRow["AcknowledgeNumber"].ToString();

            //set right label text
            RightDocumentLabel.Text = "SET: " + docSetRow["SetNo"].ToString();

            //set status text
            if (docSetRow["Status"].ToString().Equals("Verified"))
            {
                SetConfirmLabel.Text = Constants.SetVerified;
                LeftConfirmButton.Visible = false;
            }
            else if (docSetRow["Status"].ToString().Trim().Equals("Closed"))
            {
                SetConfirmLabel.Text = Constants.SetClosed;
                LeftConfirmButton.Visible = false;
            }
            else
            {
                SetConfirmLabel.Text = Constants.SetNotVerified;
                LeftConfirmButton.Visible = true;
            }

            //set asigned to 
            ProfileDb profilDb = new ProfileDb();
            if (!string.IsNullOrEmpty(docSetRow["VerificationStaffUserId"].ToString()))
            {
                Guid VerificationStaffUserId = new Guid(docSetRow["VerificationStaffUserId"].ToString());
                string assignedTo = profilDb.GetUserFullName(VerificationStaffUserId);
                SetVerificationOfficerLabel.Text = string.IsNullOrEmpty(assignedTo.Trim()) ? "Set not assigned" : "Assigned to: " + assignedTo;
            }
            else
                SetVerificationOfficerLabel.Text = "Set not assigned";

            SetButtonClass(string.Empty); // just passimg empty button
        }
        else
            Response.Redirect("~/Default.aspx");
    }

    protected void PopulateSplitType()
    {
        SplitTypeRadioButtonList.DataSource = Enum.GetValues(typeof(SplitTypeEnum));
        SplitTypeRadioButtonList.DataBind();
        SplitTypeRadioButtonList.SelectedIndex = 0;
    }

    private void PopulateLogAction()
    {
        LogActionDb logActionDb = new LogActionDb();

        if (RadTreeView1.SelectedNode.Category.ToLower().Equals("set"))
        {
            RadGridLog.Columns[3].Visible = true;
            RadGridLog.DataSource = logActionDb.GetLogActionBySetID(int.Parse(RadTreeView1.SelectedValue));
        }
        else
        {
            RadGridLog.Columns[3].Visible = false;
            RadGridLog.DataSource = logActionDb.GetLogActionByDocID(int.Parse(RadTreeView1.SelectedValue));
        }

        selectedNodeGlobal = RadTreeView1.SelectedValue;
    }

    //SortNodes is a recursive method enumerating and sorting all node levels 
    private void SortNodes(RadTreeNodeCollection collection)
    {
        Sort(collection);

        foreach (RadTreeNode node in collection)
        {
            if (node.Nodes.Count > 0)
            {
                SortNodes(node.Nodes);
            }
        }
    }

    //The Sort method is called for each node level sorting the child nodes 
    public void Sort(RadTreeNodeCollection collection)
    {
        RadTreeNode[] nodes = new RadTreeNode[collection.Count];
        collection.CopyTo(nodes, 0);
        Array.Sort(nodes, new TreeNodeComparer());
        collection.Clear();
        collection.AddRange(nodes);
    }

    /// <summary>
    /// Get the selected RawPages Ids
    /// </summary>
    /// <returns></returns>
    private List<int> GetRawPageIds()
    {
        List<int> rawPageIds = new List<int>();

        foreach (RadListViewDataItem item in ThumbnailRadListView.Items)
        {
            CheckBox SelectedCheckBox = item.FindControl("SelectedCheckBox") as CheckBox;

            // Get the id of the item
            int id = int.Parse(item.GetDataKeyValue("Id").ToString());
            

            // Add the id to the list if the checkbox is checked
            if (SelectedCheckBox.Checked)
                rawPageIds.Add(id);
        }

        return rawPageIds;
    }
    //June 10, 2013 Added By Edward
    /// <summary>
    /// Checks if Role has Access for Changing Acceptance 
    /// </summary>
    private void CheckAccessforAcceptance()
    {
        Guid? userId = null;
        Guid roleId = Guid.Empty;
        MembershipUser user = Membership.GetUser();
        userId = (Guid)user.ProviderUserKey;
        string userName = user.UserName;
        AccessControlDb accessControlDb = new AccessControlDb();

        UserDb userDb = new UserDb();
        roleId = userDb.GetRoleId(userId.Value);

        AccessControl.AccessControlDataTable accessControl = new AccessControl.AccessControlDataTable();
        accessControl = accessControlDb.GetAccessControl(roleId, true);
        if (accessControl.Rows.Count > 0)
        {
            foreach (AccessControl.AccessControlRow r in accessControl.Rows)
            {
                if (r.AccessRight.Contains("Change_Acceptance"))
                {
                    if (r.HasAccess == true)
                    {
                        AcceptancePlaceholder.Visible = true;
                        AcceptanceRadioButtonList.Visible = true;
                        AcceptanceRadioButtonList.Enabled = true;
                        AcceptanceRequiredFieldValidator.Enabled = true;
                    }
                    else
                    {
                        AcceptancePlaceholder.Visible = false;
                        AcceptanceRadioButtonList.Visible = false;
                        AcceptanceRadioButtonList.Enabled = false;
                        AcceptanceRequiredFieldValidator.Enabled = false;
                    }
                }
            }
        }
    }

    //#region Added By Edward Sales and Resale Changes 12/3/2014 - Update ImageAccepted to Yes by Default For sales and resale
    //private void UpdateImageAcceptedToYes()
    //{
    //    DocAppDb docAppDb = new DocAppDb();            
    //    DocApp.DocAppDataTable docApps = docAppDb.GetDocAppByDocSetId(setId.Value);

    //    if (docApps.Rows.Count > 0)
    //    {
    //        DocApp.DocAppRow docAppRow = docApps[0];
    //        if (docAppRow.RefType.ToUpper().Equals(ReferenceTypeEnum.SALES.ToString()) || docAppRow.RefType.ToUpper().Equals(ReferenceTypeEnum.RESALE.ToString()))
    //        {
    //            DocDb docDb = new DocDb();
    //            Doc.DocDataTable docDt = docDb.GetDocByDocSetId(setId.Value);
    //            foreach (Doc.DocRow row in docDt.Rows)
    //            {
    //                if (row.IsImageAcceptedNull())
    //                    docDb.UpdateImageAccepted(int.Parse(row["Id"].ToString()), "Y");
    //            }
    //        }
    //    }                
    //}
    //#endregion

    #endregion

    //The TreeNodeComparer class defines the sorting criteria 
    class TreeNodeComparer : IComparer
    {
        #region IComparer Members

        public int Compare(object x, object y)
        {
            RadTreeNode firstNode = (RadTreeNode)x;
            RadTreeNode secondNode = (RadTreeNode)y;

            // Not to sort folders
            if (firstNode.Nodes.Count > 0 || secondNode.Nodes.Count > 0)
            {
                return 0;
            }

            return firstNode.Text.CompareTo(secondNode.Text);
        }

        #endregion
    }

    //protected string GetImageUrl(object id)
    //{
    //    string imageUrl = String.Empty;
    //    string imageName = String.Empty;

    //    int rawPageId = Convert.ToInt32(id);

    //    DirectoryInfo[] rawPages = ocrStorageDir.GetDirectories(rawPageId.ToString(), SearchOption.AllDirectories);

    //    if (rawPages.Length > 0)
    //    {
    //        DirectoryInfo rawPageDir = rawPages[0];

    //        FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_th.jpg");

    //        if (rawPageFiles.Length > 0)
    //        {
    //            imageUrl = "../Common/DownloadThumbnail.aspx?filePath=" + rawPageFiles[0].FullName;
    //        }
    //    }

    //    return imageUrl;
    //}
    #region modified emailing by Edward 2015/03/09
    protected void SendEmail(bool hasPendingDoc, string peOIC, string caOIC, string hleStatus,
          string refType, string refNo, string setNumber, int setId, string docAppId, bool isRiskLow)
    {
        string AdditionalOrAll = string.Empty;
        string ReviewTheCase = string.Empty;
        string subject = string.Empty;
        string message = string.Empty;
        string recipientEmail = string.Empty;
        bool IsValidcaOIC = false;
        string pdfPath = string.Empty;
        string ccEmail = "MyDocErrLog@mailbox.ABCDE.com.ph";  //Added By Edward 2014/10/09 change to mailbox   

        //stop email notification if reference no is sales or sers
        if (refType.Trim().ToUpper().Equals("SALES") || refType.Trim().ToUpper().Equals("SERS"))        //Added By Edward Stop Email Notification for Sales and Sers 2015/03/9
            return;

        //Last sentence in the message body
        ReviewTheCase = "<br><br>You may view the image in DWMS using the link above and review the case, if necessary.";

        if (isRiskLow)
        {
            recipientEmail = "ABCDECredit@mailbox.ABCDE.com.ph";
            subject = string.Format("Documents (Set No. {0}) for {1} {2} (Status : System Approved) have been received", setNumber, refType, refNo);
            message = string.Format("Documents (Set No. <a href='{0}Verification/View.aspx?id={1}' target='_blank'>{2}</a>) for {3} <a href='{4}Completeness/View.aspx?id={5}' target=_blank>{6}</a> (Status : System Approved) have been received.",
                    Retrieve.GetDWMSDomain(), setId.ToString(), setNumber, refType, Retrieve.GetDWMSDomain(), docAppId, refNo);
            message = message + "<br><br>Documents received after approval." + ReviewTheCase;
        }
        else
        {
            //exit the function when invalid peOIC(-, COS or blank)
            if (String.IsNullOrEmpty(peOIC.Trim()) || peOIC.Trim() == "-" || peOIC.Trim().ToUpper() == "COS")
                return;

            //Check is peOIC and caOIC are on the list
            bool peOICfoundInUserList = ProfileDb.GetCountByEmailSetId(peOIC, setId);
            bool caOICfoundInUserList = ProfileDb.GetCountByEmailSetId(caOIC, setId);

            //if hasPendingDoc, adds the word "Additional", else "All" in the subjec and message
            if (hasPendingDoc)
                AdditionalOrAll = "Additional";
            else
                AdditionalOrAll = "All";

            //checks if caOIC is valid
            if (!string.IsNullOrEmpty(caOIC.Trim()) && caOIC.Trim() != "-" && caOIC.Trim().ToUpper() != "COS")
                IsValidcaOIC = true;

            //sets the default message of subject
            subject = string.Format("{0} documents (Set No. {1}) for {2} {3} (Status : {4}) have been received",
                AdditionalOrAll, setNumber, refType, refNo, hleStatus);

            //sets the default message of the message body
            message = string.Format("{0} documents (Set No. <a href='{1}Verification/View.aspx?id={2}' target='_blank'>{3}</a>) for {4} <a href='{5}Completeness/View.aspx?id={6}' target=_blank>{7}</a> (Status : {8}) have been received.",
                    AdditionalOrAll, Retrieve.GetDWMSDomain(), setId.ToString(), setNumber, refType, Retrieve.GetDWMSDomain(), docAppId, refNo, hleStatus);

            if (!peOICfoundInUserList)
            {
                recipientEmail = peOIC.Trim() + "@" + Retrieve.GetEmailDomain();
                subject = string.Format("{0} documents for {1} {2} have been received", AdditionalOrAll, refType, refNo);
                message = "Please review the case, if necessary.";
                string errorMsg = string.Empty;
                pdfPath = Util.GeneratePdfPathBySetId(setId, out errorMsg);
                if (!String.IsNullOrEmpty(errorMsg))
                    ReviewTheCase = " There is an error of attaching the files (" + errorMsg + ")<br/> Please contact DWMS Admin.";
                message = message + ReviewTheCase;
            }
            else if (hleStatus.Equals(HleStatusEnum.Approved.ToString()) || hleStatus.Equals(HleStatusEnum.Cancelled.ToString()) || hleStatus.Equals(HleStatusEnum.Expired.ToString()))
            {
                recipientEmail = !IsValidcaOIC ? string.Empty : caOIC.Trim() + "@" + Retrieve.GetEmailDomain();
                message = message + ReviewTheCase;
            }
            else if (hleStatus.Equals(HleStatusEnum.Pending_Pre_E.ToString()) || hleStatus.Equals(HleStatusEnum.Complete_Pre_E_Check.ToString()))
            {
                recipientEmail = peOIC.Trim() + "@" + Retrieve.GetEmailDomain();
                subject = "(Verified) " + subject;
                message = message + ReviewTheCase;
            }
            else if (caOICfoundInUserList && (hleStatus.Equals(HleStatusEnum.KIV_CA.ToString()) || hleStatus.Equals(HleStatusEnum.KIV_Pre_E.ToString())))
            {
                recipientEmail = !IsValidcaOIC ? string.Empty : caOIC.Trim() + "@" + Retrieve.GetEmailDomain();
                message = message + ReviewTheCase;
            }
            else if (caOICfoundInUserList && hleStatus.Equals(HleStatusEnum.Rejected.ToString()))
            {
                recipientEmail = !IsValidcaOIC ? string.Empty : caOIC.Trim() + "@" + Retrieve.GetEmailDomain();
                subject = string.Format("{0} documents for {1} {2} (Status : {3}) have been received",
                    AdditionalOrAll, refType, refNo, hleStatus);
                ReviewTheCase = "<br><br>Please look into the case.";
            }
            else
            {
                recipientEmail = "MyDocErrLog@mailbox.ABCDE.com.ph";
                subject = "(Not in Loop) " + subject + " for " + peOIC.Trim();
                message = "(Not in Loop) " + message + " for" + peOIC.Trim();
                message = message + ReviewTheCase;
            }
        }

        if (!String.IsNullOrEmpty(recipientEmail.Trim()))
        {
            ParameterDb parameterDb = new ParameterDb();
            bool emailSent = Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
            recipientEmail, ccEmail, string.Empty, string.Empty, subject, message, pdfPath);
        }
    }
    #endregion

    #region Added By Edward 2015/08/06 Reduce Errors in Verification to Reduce errors in CDB

    //protected void CustomValidator_Acceptance_ServerValidate(object sender, ServerValidateEventArgs e)
    //{
    //    if (string.IsNullOrEmpty(AcceptanceRadioButtonList.SelectedValue))
    //        e.IsValid = false;
    //    else
    //        e.IsValid = true;
    //}
    #endregion

    #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    protected void RawfileRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;
            HyperLink linkOrigDoc = e.Item.FindControl("linkOrigDoc") as HyperLink;
            linkOrigDoc.Text = data["FileName"].ToString();            
            string filePath = Util.GetIndividualRawFileOcrFolderPath(int.Parse(data["DocSetId"].ToString()), int.Parse(data["Id"].ToString())) + @"\" + data["FileName"].ToString();

            //linkOrigDoc.NavigateUrl = string.Format("../Common/DownloadMergedDocument.aspx?filePath={0}", filePath);
            linkOrigDoc.NavigateUrl = string.Format("{0}Common/DownloadMergedDocument.aspx?filePath={1}", Retrieve.GetDWMSDomain(), filePath);            
            linkOrigDoc.Target = "_blank";
        }
    }
    #endregion



    protected void DocAppRadTextBox_TextChanged(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {            
            if (Validation.IsNric(DocAppRadTextBox.Text.Trim()) ||
                Validation.IsFin(DocAppRadTextBox.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "ChooseReferenceScript",
                    String.Format("ShowWindow('../Common/ChooseReference.aspx?nric={0}', 500, 500);", DocAppRadTextBox.Text.Trim()),
                    true);
            }
            else
            {
                DocAppDb docAppDb = new DocAppDb();

                //2012-09-12
                DocApp.DocAppDataTable docApp = null;
                if (Validation.IsHLENumber(DocAppRadTextBox.Text.Trim())) //HLE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsSersNumber(DocAppRadTextBox.Text.Trim())) //SERS
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsSalesNumber(DocAppRadTextBox.Text.Trim())) //SALES
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsResaleNumber(DocAppRadTextBox.Text.Trim())) //RESALE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());

                if (docApp != null)
                {
                    if (docApp.Rows.Count > 0)
                    {
                        DocApp.DocAppRow row = docApp[0];
                        ReferenceTypeLabel.Text = row.RefType;
                        //ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                        DocAppHiddenValue.Value = row.Id.ToString();


                        #region 2012-09-06 List the household structure
                        if (row.RefType.ToUpper().Trim() == "HLE")
                        {
                            //SersInterface
                            HleInterfaceDb db = new HleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text.Trim());
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SERS")
                        {
                            //SersInterface
                            SersInterfaceDb db = new SersInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SALES")
                        {
                            //SalesInterface
                            SalesInterfaceDb db = new SalesInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "RESALE")
                        {
                            //ResaleInterface
                            ResaleInterfaceDb db = new ResaleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else
                        {
                            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
                        }
                        #endregion 2012-09-06 List the household structure
                    }
                }
            }
        }
        else
        {
            ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = "N.A";
        }
    }
}
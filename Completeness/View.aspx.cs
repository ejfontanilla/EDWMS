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
using iTextSharp.text.pdf;

public partial class Completeness_ViewApp : System.Web.UI.Page
{
    #region Members, Properties and Contructors
    public int? docAppId;        
    //private int summaryApplicantCnt;      //Commented by Edward to Reduce Error Notifications 2015/8/24
    //private int summaryOccupierCnt;
    //private int summaryBuyerCnt;
    //private int summarySellerCnt;
    //private int summaryMiscCnt;
    //string previousHleNumber = string.Empty;
    private string currentDocType = string.Empty;
    private string selectedNodeGlobal = "0";
    public bool isAppConfirmed = false;
    bool AllowCompletenessSaveDate = false;
    public string oldImageCondition = string.Empty;
    #endregion

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        RadToolTipManager1.TargetControls.Clear();

        if (!string.IsNullOrEmpty(Request["id"]))
        {
            docAppId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/Completeness/");

        // Check if refno exit
        DocAppDb docAppDb = new DocAppDb();
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

        if (docApps.Rows.Count == 0)
            Response.Redirect("~/Completeness/");

        isAppConfirmed = docAppDb.IsAppConfirmed(docAppId.Value);

        AllowCompletenessSaveDate = docAppDb.AllowCompletenessSaveDate(docAppId.Value);

        if (!IsPostBack)
        {
            DocApp.DocAppRow docAppRow = docApps[0];
            CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();
            cdbPendingDoc.ProcessPendingDocs(docAppRow.RefNo, "PAR", "LAT");
            docAppDb.UpdatePendingDoc(docAppRow.RefNo);
            SetUserSelectionCookie(string.Empty, string.Empty, string.Empty);
            PopulateTreeView();
            populateRefInfo();
            PopulateSplitType();            
            //PopulatePersonals(true);
            SetButtonClass("Summary");

            //disable action buttons if the app is not assigned to the currentuser and set status is not completeness_in_progress
            //SubmitMetadataButton.Enabled = SubmitAndConfirmMDButton.Enabled = LeftConfirmButton.Enabled = LeftOptions.Enabled = AllowCompletenessSaveDate;
            //Added By Edward 04.11.2013 Confirm All Acceptance
            SubmitMetadataButton.Enabled = SubmitAndConfirmMDButton.Enabled = LeftConfirmButton.Enabled = LeftOptions.Enabled = ConfirmAllAcceptanceButton.Enabled = AllowCompletenessSaveDate;
            //disable action buttons if app is confirmed
            if (isAppConfirmed)
                SubmitMetadataButton.Enabled = SubmitAndConfirmMDButton.Enabled = LeftOptions.Enabled = false;

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

        // Set Treeview, Summary Panel and Thumbnail Panel Height
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SetHeightForControls", "javascript:SetHeightForControls();", true);
    }

    protected void MetaFieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //{
        //    // Get the controls
        //    HiddenField IdHiddenFiled = e.Item.FindControl("IdHiddenFiled") as HiddenField;
        //}
    }    

    protected void DocTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Populate Meta fields
        int docId = int.Parse(RadTreeView1.SelectedValue);

        MetaDataDb metaDataDb = new MetaDataDb();

        //get the reference to the control
        UserControl userControl = (UserControl)AdditionalMetaDataPanel.FindControl(DocTypeDropDownList.SelectedValue.Trim() + "MetaDataUC");
        SetControlVisibility(userControl);
        LoadSaveControlData(DocTypeDropDownList.SelectedValue.Trim(), true, false, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());

        PopulateMetaData(DocTypeDropDownList.SelectedValue.Trim().ToUpper());
    }

    protected void SubmitMetadataButton_Click(object sender, EventArgs e)
    {
        RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

        RadTreeNode node = RadTreeView1.SelectedNode;
        if (node != null && Validation.IsInteger(node.Value))
        {
            //Page.Validate();        //Added By Edward 08/01/2014    DWMS - Add UIN, FIN validation to Parent Image
            //if (Page.IsValid)       //Added By Edward 08/01/2014    DWMS - Add UIN, FIN validation to Parent Image
            //{
                SaveMetaData(false, false);
            //}
        }
    }

    protected void ImageConditionDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"))
            SubmitAndConfirmMDButton.CausesValidation = false;
    }

    protected void SubmitAndConfirmMDButton_Click(object sender, EventArgs e)
    {
        RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

        RadTreeNode node = RadTreeView1.SelectedNode;

        DocDb docDb = new DocDb();
        Boolean isVerified = false;

        if (node != null && Validation.IsInteger(node.Value))
            isVerified = docDb.GetIsVerifiedByDocId(int.Parse(node.Value));

        if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
            Page.Validate();

        if (!ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete") && !isVerified)
        {
            if (Page.IsValid && node != null && Validation.IsInteger(node.Value))
                SaveMetaData(true, true);
        }
        else
        {
            if (node != null && Validation.IsInteger(node.Value))
                SaveMetaData(true, false);
        }
    }    

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        bool fromSummary = true;

        bool.TryParse(e.Argument, out fromSummary);
            
        RadToolTipManager1.TargetControls.Clear();
        PopulatePersonals(fromSummary);
        PopulateTreeView();
        //DisplayPdfDocument(int.Parse(RadTreeView1.SelectedValue));
    }

    protected void LeftConfirmButton_Click(object sender, EventArgs e)
    {
        ConfirmApplication();        //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
    }

    protected void RadToolBarMenuDoc_ButtonClick(object sender, RadToolBarEventArgs e)
    {
        string buttonClicked = e.Item.Value.ToLower();

        if (buttonClicked.Equals("logdoc"))
            LogDocButton_Click(sender, e);
        else if (buttonClicked.Equals("image"))
            ImgButton_Click(sender, e);
        else if (buttonClicked.Equals("thumbnails"))
            ThumbnailButton_Click(sender, e);
    }

    protected void RadToolBarMenuRef_ButtonClick(object sender, RadToolBarEventArgs e)
    {
        string buttonClicked = e.Item.Value.ToLower();

        if (buttonClicked.Equals("summary"))
            SummaryRadButton_Click(sender, e);
        else if (buttonClicked.Equals("outstanding"))
            OutstandingDocsRadButton_Click(sender, e);
        else if (buttonClicked.Equals("logref"))
            LogRefButton_Click(sender, e);
    }

    #region Green Panel Buttons
    protected void SummaryRadButton_Click(object sender, EventArgs e)
    {
        ThumbnailPanel.Visible = ExportButton.Visible = LogPanel.Visible = ExtractButton.Visible = MetaDataPanel.Visible = pdfframe.Visible = false;
        SummaryPanel.Visible = true;
        OutstandingPanel.Visible = !SummaryPanel.Visible;
        SetUserSelectionCookie("SUMMARY", string.Empty, string.Empty);
        PopulatePersonals(true);
        SetButtonClass("Summary");
    }

    protected void OutstandingDocsRadButton_Click(object sender, EventArgs e)
    {
        ThumbnailPanel.Visible = ExportButton.Visible = LogPanel.Visible = ExtractButton.Visible = MetaDataPanel.Visible = pdfframe.Visible = false;
        OutstandingPanel.Visible = true;
        SummaryPanel.Visible = !OutstandingPanel.Visible;
        SetUserSelectionCookie("OUTSTANDING", string.Empty, string.Empty);
        PopulatePersonals(false);
        SetButtonClass("Outstanding");
    }

    protected void LogRefButton_Click(object sender, EventArgs e)
    {
        MetaDataPanel.Visible = pdfframe.Visible = ExtractButton.Visible = ThumbnailPanel.Visible = false;
        LogPanel.Visible = ExportButton.Visible = true;
        SummaryPanel.Visible = OutstandingPanel.Visible = false;
        SetUserSelectionCookie("LOGREF", string.Empty, string.Empty);
        PopulateLogAction();
        RadGridLog.DataBind();
        SetButtonClass("LogRef");
    }

    protected void LogDocButton_Click(object sender, EventArgs e)
    {
        MetaDataPanel.Visible = pdfframe.Visible = ExtractButton.Visible = ThumbnailPanel.Visible = false;
        LogPanel.Visible = ExportButton.Visible = true;
        SummaryPanel.Visible = OutstandingPanel.Visible = false;
        SetUserSelectionCookie(string.Empty, "LOGDOC", string.Empty);
        PopulateLogAction();
        RadGridLog.DataBind();
        SetButtonClass("LogDoc");
    }

    protected void ThumbnailButton_Click(object sender, EventArgs e)
    {
        int docId = int.Parse(RadTreeView1.SelectedValue);
        RawPageDb rawPageDb = new RawPageDb();
        MetaDataPanel.Visible = pdfframe.Visible = ExportButton.Visible = LogPanel.Visible = false;
        ThumbnailPanel.Visible = ExtractButton.Visible = true;
        ExtractButton.Enabled = AllowCompletenessSaveDate;

        if (isAppConfirmed)
            ExtractButton.Enabled = false;
        if (rawPageDb.CountRawPageByDocId(docId) <= 1)
            ExtractButton.Enabled = false;

        ThumbnailRadListView.Rebind();
        SetUserSelectionCookie(string.Empty, "THUMBNAILS", string.Empty);
        SetButtonClass("Thumbnails");

        if (!Object.Equals(RadToolTipManager1, null))
        {
            //clear the tooltipcontrols
            RadToolTipManager1.TargetControls.Clear();
        }
    }

    protected void ImgButton_Click(object sender, EventArgs e)
    {
        ThumbnailPanel.Visible = ExportButton.Visible = LogPanel.Visible = false;
        MetaDataPanel.Visible = ExtractButton.Visible = pdfframe.Visible = true;
        ExtractButton.Enabled = AllowCompletenessSaveDate;

        if (isAppConfirmed)
            ExtractButton.Enabled = false;

        SetUserSelectionCookie(string.Empty, "IMAGE", string.Empty);
        SetButtonClass("Image");

        if (!Object.Equals(RadToolTipManager1, null))
        {
            //Add the button (target) id to the tooltip manager                
            RadToolTipManager1.TargetControls.Add(ExtractButton.ClientID, true);
        }
    }

    protected void ExportButton_Click(object sender, EventArgs e)
    {
        RadGridLog.ExportSettings.OpenInNewWindow = true;
        RadGridLog.ExportSettings.ExportOnlyData = true;
        RadGridLog.ExportSettings.IgnorePaging = true;

        DocSetDb docSetDb = new DocSetDb();
        DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(docAppId.Value);
        DocSet.DocSetRow docSetRow = docSets[0];

        RadGridLog.ExportSettings.FileName = docSetRow.SetNo;

        //code below to resolve the https download problem
        Page.Response.ClearHeaders();
        Page.Response.Cache.SetCacheability(HttpCacheability.Private);

        //RadGridLog.ExportSettings.ExportOnlyData = true;
        RadGridLog.MasterTableView.ExportToExcel();
    }

    protected void ExtractButton_Click(object sender, EventArgs e)
    {
        try //Added By Edward 27.10.2013
        {
            RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

            // Get selected ids
            List<int> ids = GetRawPageIds();

            // Get selected ids
            List<int> pageIds = GetRawPageIds();
            int docId = int.Parse(RadTreeView1.SelectedValue);

            if (pageIds.Count >= 1)
            {
                DocDb docDb = new DocDb();
                SplitTypeEnum splitType = (SplitTypeEnum)Enum.Parse(typeof(SplitTypeEnum), SplitTypeRadioButtonList.SelectedValue.Trim(), true);

                docDb.SplitDocumentForCompleteness(docId, pageIds, splitType);
            }

            // Reload the controls
            //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
            //SetRightDocumentLabelText(docId);
            SetRightDocumentLabelText(docId.ToString());
            SetButtonVisibility("doc");
            PopulateTreeView();
            //populateRefInfo();
            LoadContextContent(false);
        }
        catch (Exception ex)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Completeness/View.aspx - ExtractButton_Click", errorMessage);
        }
        

    }
    #endregion

    protected void Save(object sender, EventArgs e)
    {
        //Page.Validate();
        //RadTreeNode node = RadTreeView1.SelectedNode;

        //if (Page.IsValid && node != null && Validation.IsInteger(node.Value))
        //{
        //    int id = int.Parse(node.Value);

        //    PopulateTreeView();

        //    ResetPageClosingFlags();
        //}
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
        RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

        string option = LeftOptions.SelectedValue;

        switch (option)
        {
            case "To Cancel":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                    "CancelApplication", "ShowWindow('Cancel.aspx?id=" + docAppId + "', 500, 520);", true);
                break;
            case "Download":
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Download", "loadExport();", true);
                break;
        }

        LeftOptions.SelectedIndex = 0;
    }

    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        this.UpdateToolTip(args.Value, args.UpdatePanel);
    }

    #region RadTreeView
    protected void RadTreeView1_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        //Added try catch by Edward 2015/8/21 Reduce Error Notifications Object reference not set to an instance of an object in TreeNodeClick
        string ErrorPart = string.Empty;        // this will determine what part the error is occuring. Remove after tracing all errors
        try
        {
            if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "ref" || RadTreeView1.SelectedNode.Category.Trim().ToLower() == "doc")
            {
                ErrorPart = "1";
                if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "ref")
                {
                    populateRefInfo();
                    foreach (RadToolBarButton radToolBarButton in RadToolBarMenuRef.Items)
                    {
                        radToolBarButton.Enabled = true;
                    }
                }
                else
                {
                    ErrorPart = "2";
                    //set button font and enable
                    foreach (RadToolBarButton radToolBarButton in RadToolBarMenuDoc.Items)
                    {
                        radToolBarButton.Enabled = true;
                    }

                    MetaDataPanel.Visible = ExtractButton.Enabled = true;
                    ThumbnailPanel.Visible = pdfframe.Visible = true;
                    LogPanel.Visible = SummaryPanel.Visible = OutstandingPanel.Visible = false;
                    ErrorPart = "3";
                    ExtractButton.Enabled = !isAppConfirmed;
                    RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (RadTreeView1.SelectedNode.Attributes["Status"].ToString().ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
                    LoadMetaData();
                }
                ErrorPart = "4";
                SetUserSelectionCookie(string.Empty, string.Empty, RadTreeView1.SelectedValue);
                SetButtonVisibility(RadTreeView1.SelectedNode.Category.Trim().ToLower());
                LoadContextContent(true);
            }
            else
            {
                ErrorPart = "5";
                RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text;
                ExtractButton.Enabled = false;
                ThumbnailPanel.Visible = MetaDataPanel.Visible = SummaryPanel.Visible = OutstandingPanel.Visible = LogPanel.Visible = pdfframe.Visible = false;
                ExportButton.Visible = false;

                foreach (RadToolBarButton radToolBarButton in RadToolBarMenuDoc.Items)
                {
                    radToolBarButton.Enabled = false;
                }

                foreach (RadToolBarButton radToolBarButton in RadToolBarMenuRef.Items)
                {
                    radToolBarButton.Enabled = false;
                }
            }
            ErrorPart = "6";
            ResetPageClosingFlags();
            selectedNodeGlobal = RadTreeView1.SelectedValue;
        }
        catch (Exception ex)
        {            
            ErrorLogDb errorLogDb = new ErrorLogDb();
            string errorMessage = ErrorPart + ":" + ex.Message + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
            errorLogDb.Insert("Completeness/View.aspx - RadTreeView1_NodeClick", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex, ErrorPart);
        }        
    }

    protected void RadTreeView1_NodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
    {
        try // Added Try Catch ErrorLog by edward 27.10.2013
        {
            RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

            // Fetch event data 
            RadTreeNode sourceNode = e.SourceDragNode;
            RadTreeNode destNode = e.DestDragNode;

            string srcFolderName = e.SourceDragNode.ParentNode.Text;
            string desFolderName = e.DestDragNode.Text;

            if (!destNode.Category.Trim().ToUpper().Equals("DOC"))
            {
                string[] srcFolderNameArray = srcFolderName.Split(')');
                srcFolderName = srcFolderNameArray[1].Trim();
                string[] desFolderNameArray = desFolderName.Split(')');
                desFolderName = desFolderNameArray[1].Trim();
            }

            #region within the treenode
            //proceed only if dest node is different.
            if (sourceNode.ParentNode != destNode)
            {
                int docId = int.Parse(sourceNode.Value);
                DocDb docDb = new DocDb();
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DocSetDb docSetDb = new DocSetDb();
                LogActionDb logActionDb = new LogActionDb();

                int setId = docDb.GetSetIdByDocId(int.Parse(sourceNode.Value));

                //if drop into REFNO Folder
                if (destNode.Category.Trim().ToUpper().Equals("REFNO"))
                {
                    if (!(sourceNode.Attributes["DocTypeCode"].ToUpper().Trim().Equals(destNode.Attributes["DocTypeCode"].ToUpper().Trim())))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", "This document cannot be put directly under the application folder."), true);
                    }
                    else
                    {
                        //attach and dettach doc/app personal reference
                        CustomPersonal customPersonal = new CustomPersonal();
                        Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, false, docId, setId, int.Parse(destNode.Value), DocFolderEnum.Unidentified.ToString());

                        if (isSuccess)
                        {
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.C, docId);
                            docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
                            docDb.UpdateIsAccepted(docId, false);
                            //reset SendToCdb Flags
                            docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                            docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);
                        }
                    }
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF NRIC")) // if drop into REFNO/NRIC Folder
                {
                    if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    {
                        string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

                        //attach and dettach doc/app personal reference
                        CustomPersonal customPersonal = new CustomPersonal();

                        Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Unidentified.ToString());

                        if (isSuccess)
                        {
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.C, docId);
                            docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
                            docDb.UpdateIsAccepted(docId, false);
                            //reset SendToCdb Flags
                            docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                            docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);
                        }
                    }
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF OTHERS")) // if drop into REFNO/OTHERS Folder
                {
                    if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    {
                        //attach and dettach doc/app personal reference
                        CustomPersonal customPersonal = new CustomPersonal();

                        Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, false, docId, setId, int.Parse(destNode.ParentNode.Value), DocFolderEnum.Others.ToString());

                        if (isSuccess)
                        {
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.C, docId);
                            docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
                            docDb.UpdateIsAccepted(docId, false);
                            //reset SendToCdb Flags
                            docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                            docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);
                        }
                    }
                }
                else if (destNode.Category.Trim().ToUpper().Equals("REF OTHERS NRIC")) // if drop into REFNO/OTHERS/NRIC Folder
                {
                    if (sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "HLE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SALES"
                        && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "RESALE" && sourceNode.Attributes["DocTypeCode"].ToUpper().Trim() != "SERS")
                    {
                        string nric = destNode.Attributes["DocRefId"].ToUpper().Trim();

                        //attach and dettach doc/app personal reference
                        CustomPersonal customPersonal = new CustomPersonal();
                        Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, nric, false, docId, setId, int.Parse(destNode.ParentNode.ParentNode.Value), DocFolderEnum.Others.ToString());

                        if (isSuccess)
                        {
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_moved_from_REPLACE1_folder_to_REPLACE2_folder, srcFolderName, desFolderName, string.Empty, string.Empty, LogTypeEnum.C, docId);
                            docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
                            docDb.UpdateIsAccepted(docId, false);
                            //reset SendToCdb Flags
                            docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                            docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);
                        }
                    }
                }
                else if (destNode.Category.Trim().ToUpper().Equals("DOC")) // if document is drop onto a document >> merge action
                {
                    //proceed only if both the source and destination documents still exist.
                    int destDocId = int.Parse(destNode.Value);

                    Boolean isSourceAndDestinationDocumentExist = false;

                    if (docDb.GetDocById(docId).Rows.Count > 0)
                        isSourceAndDestinationDocumentExist = true;

                    if (docDb.GetDocById(destDocId).Rows.Count > 0)
                        isSourceAndDestinationDocumentExist = true;

                    if (isSourceAndDestinationDocumentExist)
                    {
                        //copy all the source document raw pages to the destination document
                        RawPageDb rawPageDb = new RawPageDb();
                        RawPage.RawPageDataTable rawPagesDestDt = rawPageDb.GetRawPageByDocId(destDocId);
                        RawPage.RawPageDataTable rawPagesSrcDt = rawPageDb.GetRawPageByDocId(docId);

                        // get setIds and document description 
                        //source
                        Doc.DocDataTable docs = docDb.GetDocById(docId);
                        Doc.DocRow docRow = docs[0];
                        int sourceSetId = docRow.DocSetId;

                        DocTypeDb docTypeDb = new DocTypeDb();
                        DocType.DocTypeDataTable sourceDocType = docTypeDb.GetDocType(docRow.DocTypeCode);
                        DocType.DocTypeRow sourceDocTypeRow = sourceDocType[0];
                        string sourceDescription = sourceDocTypeRow.Description;

                        //destination
                        Doc.DocDataTable desDocs = docDb.GetDocById(destDocId);
                        Doc.DocRow desDocRow = desDocs[0];
                        int destSetId = desDocRow.DocSetId;

                        DocType.DocTypeDataTable desDocType = docTypeDb.GetDocType(desDocRow.DocTypeCode);
                        DocType.DocTypeRow desDocTypeRow = desDocType[0];
                        string desDescription = desDocTypeRow.Description;

                        string destDocSetNo = docSetDb.GetSetNumber(destSetId);
                        string sourceDocSetNo = docSetDb.GetSetNumber(sourceSetId);

                        // Reorder the pages
                        int docPageNo = RawPageDb.ReorderRawPages(rawPagesSrcDt, destDocId);

                        rawPagesSrcDt = rawPageDb.GetRawPageByDocId(docId);

                        if (rawPagesSrcDt.Rows.Count <= 0)
                        {
                            // delete the old document.
                            docDb.Delete(docId);
                        }

                        SetUserSelectionCookie(string.Empty, string.Empty, destDocId.ToString());

                        // if documents merged from SAME set
                        if (destSetId.Equals(sourceSetId))
                        {
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Document_merged_from_REPLACE1_to_REPLACE2, sourceNode.Text, destNode.Text, string.Empty, string.Empty, LogTypeEnum.C, destDocId);
                        }
                        else // if documents merged from DIFFERENT set
                        {
                            //log the action for destination document for both completeness and verification
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_from_REPLACE2_merged_with_REPLACE3_from_REPLACE4, sourceDescription + ".pdf", "SET:" + sourceDocSetNo, desDescription + ".pdf", "SET:" + destDocSetNo, LogTypeEnum.C, destDocId);
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_from_REPLACE2_merged_with_REPLACE3_from_REPLACE4, sourceDescription + ".pdf", "SET:" + sourceDocSetNo, desDescription + ".pdf", "SET:" + destDocSetNo, LogTypeEnum.D, destDocId);

                            //log the action for destination and source set for verification
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_from_REPLACE2_merged_with_REPLACE3_from_REPLACE4, sourceDescription + ".pdf", "SET:" + sourceDocSetNo, desDescription + ".pdf", "SET:" + destDocSetNo, LogTypeEnum.S, destSetId);
                            logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_from_REPLACE2_merged_with_REPLACE3_from_REPLACE4, sourceDescription + ".pdf", "SET:" + sourceDocSetNo, desDescription + ".pdf", "SET:" + destDocSetNo, LogTypeEnum.S, sourceSetId);
                        }

                        docDb.UpdateDocStatus(destDocId, DocStatusEnum.Verified);
                        //reset SendToCdb Flags
                        docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                        docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);

                        docDb.UpdateIsAccepted(docId, false);
                        docDb.UpdateIsVerified(docId, false);
                    }
                }

                if (destNode.Category.Trim().ToUpper().Equals("DOC"))
                {
                    //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
                    //SetRightDocumentLabelText(int.Parse(destNode.Value));
                    SetRightDocumentLabelText(!string.IsNullOrEmpty(destNode.Value) ? destNode.Value : string.Empty);
                    SetUserSelectionCookie(string.Empty, string.Empty, destNode.Value);
                    selectedNodeGlobal = destNode.Value;
                }
                else
                {
                    //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
                    //SetRightDocumentLabelText(int.Parse(sourceNode.Value));
                    SetRightDocumentLabelText(!string.IsNullOrEmpty(sourceNode.Value) ? sourceNode.Value : string.Empty);
                    SetUserSelectionCookie(string.Empty, string.Empty, sourceNode.Value);
                    selectedNodeGlobal = sourceNode.Value;
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
            errorLogDb.Insert("Completeness/View.aspx - RadTreeView1_NodeDrop", errorMessage);
            ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
        }
    }
    #endregion


    #region Summary Repeaters
    #region HLE Repeater
    protected void HleSummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label DateJoinedLabel = (Label)e.Item.FindControl("DateJoinedLabel");
            Label EmploymentStatusLabel = (Label)e.Item.FindControl("EmploymentStatusLabel");
            Label CompanyNameLabel = (Label)e.Item.FindControl("CompanyNameLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(), 
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(), 
                    data["Name"].ToString(), data["Nric"].ToString());

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            DateJoinedLabel.Text = (data["DateJoined"].ToString().Trim().Equals(".") ? String.Empty : data["DateJoined"].ToString());
            EmploymentStatusLabel.Text = data["EmploymentType"].ToString();
            CompanyNameLabel.Text = data["EmployerName"].ToString();

            RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGrid");
            PopulateSummaryDocs(data["HleNumber"].ToString(), data["Nric"].ToString(), SummaryDocsRadGrid, personalType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.HLE.ToString());
        }
    }
    #endregion

    #region RESALE Repeater
    protected void ResaleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(ResalePersonalTypeEnum.BU.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Buyer " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.PR.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Parent " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.CH.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Child " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.SE.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.SP.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller Spouse " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.MISC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Miscellaneous " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "s":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller " + (++summarySellerCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "b":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Buyer " + (++summaryBuyerCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "o":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "misc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Miscellaneous " + (++summaryMiscCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGrid");
            PopulateSummaryDocs(data["CaseNo"].ToString(), data["Nric"].ToString(), SummaryDocsRadGrid, personalType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.Resale.ToString());
        }
    }
    #endregion

    #region SALES Repeater
    protected void SalesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label MaritalStatusLabel = (Label)e.Item.FindControl("MaritalStatusLabel");
            Label RelationshipLabel = (Label)e.Item.FindControl("RelationshipLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "ha":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + (++summaryApplicantCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "oc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            MaritalStatusLabel.Text = data["MaritalStatus"].ToString();
            RelationshipLabel.Text = data["Relationship"].ToString();

            RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGrid");
            PopulateSummaryDocs(data["RegistrationNo"].ToString(), data["Nric"].ToString(), SummaryDocsRadGrid, personalType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.Sales.ToString());
        }
    }
    #endregion

    #region SERS Repeater
    protected void SersRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label MaritalStatusLabel = (Label)e.Item.FindControl("MaritalStatusLabel");
            Label RelationshipLabel = (Label)e.Item.FindControl("RelationshipLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "ha":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + (++summaryApplicantCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "oc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            MaritalStatusLabel.Text = data["MaritalStatus"].ToString();
            RelationshipLabel.Text = data["Relationship"].ToString();

            RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGrid");
            PopulateSummaryDocs(data["SchAcc"].ToString(), data["Nric"].ToString(), SummaryDocsRadGrid, personalType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.SERS.ToString());
        }
    }    
    #endregion
    #endregion

    #region Outstanding Repeaters
    #region HLE
    protected void CosOutstandingPersonalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label DateJoinedLabel = (Label)e.Item.FindControl("DateJoinedLabel");
            Label EmploymentStatusLabel = (Label)e.Item.FindControl("EmploymentStatusLabel");
            Label CompanyNameLabel = (Label)e.Item.FindControl("CompanyNameLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "ha":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + (++summaryApplicantCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "oc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            DateJoinedLabel.Text = (data["DateJoined"].ToString().Trim().Equals(".") ? String.Empty : data["DateJoined"].ToString());
            EmploymentStatusLabel.Text = data["EmploymentType"].ToString();
            CompanyNameLabel.Text = data["EmployerName"].ToString();

            RadGrid PendingDocsRadGrid = (RadGrid)e.Item.FindControl("PendingDocsRadGrid");
            PopulatePendingDocs(data["HleNumber"].ToString(), data["Nric"].ToString(), PendingDocsRadGrid, ReferenceTypeEnum.HLE);
        }
    }
    #endregion

    #region RESALE
    protected void ResaleOutstandingPersonalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(ResalePersonalTypeEnum.BU.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Buyer " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.PR.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Parent " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.CH.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Child " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.SE.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.SP.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller Spouse " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(ResalePersonalTypeEnum.MISC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Miscellaneous " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "s":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller " + (++summarySellerCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "b":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Buyer " + (++summaryBuyerCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "o":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "misc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Miscellaneous " + (++summaryMiscCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            RadGrid PendingDocsRadGrid = (RadGrid)e.Item.FindControl("PendingDocsRadGrid");
            PopulatePendingDocs(data["CaseNo"].ToString(), data["Nric"].ToString(), PendingDocsRadGrid, ReferenceTypeEnum.RESALE);
        }
    }
    #endregion

    #region SALES
    protected void SocOutstandingPersonalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label MaritalStatusLabel = (Label)e.Item.FindControl("MaritalStatusLabel");
            Label RelationshipLabel = (Label)e.Item.FindControl("RelationshipLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "ha":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + (++summaryApplicantCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "oc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            MaritalStatusLabel.Text = data["MaritalStatus"].ToString();
            RelationshipLabel.Text = data["Relationship"].ToString();

            RadGrid PendingDocsRadGrid = (RadGrid)e.Item.FindControl("PendingDocsRadGrid");
            PopulatePendingDocs(data["RegistrationNo"].ToString(), data["Nric"].ToString(), PendingDocsRadGrid, ReferenceTypeEnum.SALES);
        }
    }
    #endregion

    #region SERS
    protected void SersOutstandingPersonalRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["ApplicantType"].ToString();
            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
            Label MaritalStatusLabel = (Label)e.Item.FindControl("MaritalStatusLabel");
            Label RelationshipLabel = (Label)e.Item.FindControl("RelationshipLabel");

            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            //switch (personalType.ToLower())
            //{
            //    case "ha":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + (++summaryApplicantCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    case "oc":
            //        ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + (++summaryOccupierCnt).ToString(), data["Name"].ToString(), data["Nric"].ToString());
            //        break;
            //    default:
            //        ApplicantLabel.Text = string.Empty;
            //        break;
            //}

            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? String.Empty : data["DateOfBirth"].ToString());
            MaritalStatusLabel.Text = data["MaritalStatus"].ToString();
            RelationshipLabel.Text = data["Relationship"].ToString();

            RadGrid PendingDocsRadGrid = (RadGrid)e.Item.FindControl("PendingDocsRadGrid");
            PopulatePendingDocs(data["SchAcc"].ToString(), data["Nric"].ToString(), PendingDocsRadGrid, ReferenceTypeEnum.SERS);
        }
    }
    #endregion
    #endregion

    #region Summary/Pending Doc Rad Grid
    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SummaryDocRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;

            GridDataItem dataBoundItem = e.Item as GridDataItem;

            DataRowView data = (DataRowView)e.Item.DataItem;

            Label Acceptance = (Label)e.Item.FindControl("ReceivedLabel");
            switch (data["ImageAccepted"].ToString().ToUpper())
            {
                case "Y":
                    Acceptance.Text = "Yes";
                    dataBoundItem.ForeColor = Color.Black;
                    break;
                case "N":
                    Acceptance.Text = "No";
                    dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                    break;
                case "X":
                    Acceptance.Text = "NA";
                    dataBoundItem.ForeColor = Color.Black;
                   break;
                case "U":
                    Acceptance.Text = "Unknown";
                    dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                    break;
                default:
                    Acceptance.Text = "Unknown";
                    dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                    break;
            }
            //string imageAccepted = "";
            //if (string.TryParse(data["ImageAccepted"].ToString(), out imageAccepted))
            //{
                //dataBoundItem.ForeColor = (data["ImageAccepted"] == "No" ? Color.FromArgb(204, 0, 0) : Color.Black);
            //}

            Control target = e.Item.FindControl("EditIconImage");
            ImageButton EditImageButton = (ImageButton)e.Item.FindControl("EditImageButton");

            EditImageButton.OnClientClick = String.Format("javascript:ShowWindow('Controls/UpdatePendingDoc.aspx?id={0}', 600, 500);", data["Id"]);

            if (!Object.Equals(RadToolTipManager1, null) && !isAppConfirmed)
            {
                string id = data["Id"].ToString();

                //Add the button (target) id to the tooltip manager                
                //RadToolTipManager1.TargetControls.Add(target.ClientID, id, true);
            }

            //if (AllowCompletenessSaveDate && !isAppConfirmed)
            //    target.Visible = true;
            //else
            //    target.Visible = false;

            if (AllowCompletenessSaveDate && !isAppConfirmed)
                EditImageButton.Visible = true;
            else
                EditImageButton.Visible = false;

        }
    }

    public void DocTypeCodeLinkButtonClick(object sender, EventArgs e)
    {
        LinkButton docTypeCodeLinkButton = (LinkButton)sender;

        //set the tab to image and selected document to the document
        SetButtonVisibility("doc");
        SetUserSelectionCookie("", "IMAGE", docTypeCodeLinkButton.CommandArgument);
        PopulateTreeView();
        LoadContextContent(false);
    } 

    protected void PendingDocsRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        //PopulatePersonals();
    }

    protected void PendingDocsRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;

            GridDataItem dataBoundItem = e.Item as GridDataItem;

            DataRowView data = (DataRowView)e.Item.DataItem;
        }
    }
    #endregion

    #region List View Events
    protected void RadListView1_ItemDrop(object sender, RadListViewItemDragDropEventArgs e)
    {
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
            docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //update isAcceptedStatus
            docDb.UpdateIsAccepted(docId, false);

            //update isVerified
            docDb.UpdateIsVerified(docId, false);

            // Reload the controls
            //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
            //SetRightDocumentLabelText(docId);
            SetRightDocumentLabelText(docId.ToString());

            PopulateTreeView();
            SetUserSelectionCookie(string.Empty, string.Empty, docId.ToString());
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
            //             destFolder, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("DFNRIC")) // if drop into DEFAULT FOLDER NRIC UNIDENTIFIED Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            destNode.ParentNode.Value, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);


            //        string nric = destNode.Text.Trim();
            //        nric = nric.Substring(0, 9);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, personalType, nric, string.Empty, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("UNIDENTIFIED")) // if drop into UNIDENTIFIED Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("OUTNRIC")) // if drop into OUTNRIC Folder
            //    {
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //        string nric = destNode.Text.Trim();
            //        nric = nric.Substring(0, 9);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, personalType, nric, string.Empty, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
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

            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, personalType, nric, string.Empty, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("REFNO")) // if drop into REFNO Folder
            //    {
            //        DocSetDb docSetDb = new DocSetDb();
            //        //gopi to work
            //        //int docSetAppId = docSetDb.GetDocAppId(docRow.DocSetId);
            //        // Create a copy of the current Doc data
            //        DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
            //        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
            //        docCopyId = docDb.Insert(ActiveDocType, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum,
            //            string.Empty, docRow.ImageCondition, documentConditionEnum);
            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //        //create a new docpersonal record to store the nric
            //        docPersonalDb.Insert(docCopyId, "HA", string.Empty, string.Empty, null, null, null,
            //            null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
            //            null, null, null, null, null, EmploymentStatusEnum.Employed);
            //    }
            //    else if (destNode.Category.Trim().ToUpper().Equals("DOC")) // if thumbnail is droped onto a document >> sort of merge action
            //    {
            //        docCopyId = int.Parse(destNode.Value);
            //    }

            //    //only if a new document is created, proceed to the 
            //    if (docCopyId > 0 && docCopyId != -1)
            //    {
            //        // Get the RawPage data                        
            //        RawPage.RawPageDataTable rawPageDt = rawPageDb.GetRawPageById(rawPageId);
            //        int docPageNo = 1;
            //        int pageOrder = rawPageDb.GetMaxDocPageNo(docCopyId);
            //        if (rawPageDt.Rows.Count > 0)
            //        {
            //            RawPage.RawPageRow rawPage = rawPageDt[0];

            //            // Update the RawPage to be assigned to the new Doc record
            //            rawPageDb.Update(rawPage.Id, docCopyId, pageOrder + 1);
            //            docPageNo = rawPage.DocPageNo;
            //        }

            //        RawPage.RawPageDataTable rawPagesDt = rawPageDb.GetRawPageByDocId(docId);

            //        int pageNo = 1;
            //        foreach (RawPage.RawPageRow rawPage in rawPagesDt.Rows)
            //        {
            //            rawPageDb.Update(rawPage.Id, rawPage.DocId, pageNo);

            //            pageNo++;
            //        }

            //        // Delete the current doc record if it contains only 1 page
            //        if (rawPagesDt.Rows.Count <= 0)
            //        {
            //            docDb.Delete(docId);
            //        }

            //        //update doc status
            //        docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
            //        docDb.UpdateIsAccepted(docId, false);

            //        if (destNode.Category.Trim().ToUpper().Equals("DOC"))
            //            docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);

            //        //logaction
            //        LogActionDb logActionDb = new LogActionDb();
            //        DocTypeDb docTypeDb = new DocTypeDb();
            //        DocType.DocTypeDataTable dtDocType = docTypeDb.GetDocType(docRow.DocTypeCode);
            //        DocType.DocTypeRow drDocType = dtDocType[0];
            //        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.Classified_REPLACE1_in_REPLACE2_as_REPLACE3, "Page " + docPageNo, drDocType.Description + Dwms.Web.TreeviewDWMS.FormatId(docId.ToString()), destNode.Text, LogTypeEnum.D, docCopyId);

            //        // Reload the controls
            //        selectedNodeGlobal = docCopyId.ToString();
            //        SetButtonVisibility("doc");
            //        SetRightDocumentLabelText(docId);
            //        PopulateTreeView();
            //        //populateRefInfo();
            //        SetUserSelectionCookie(string.Empty, string.Empty, docCopyId.ToString());
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
            ThumbnailPanel.Visible = ExtractButton.Visible = false;
        }
    }

    protected void RadListView1_OnItemDataBound(object sender, Telerik.Web.UI.RadListViewItemEventArgs e)
    {
        RadListViewItemDragHandle RadListViewItemDragHandle1 = (RadListViewItemDragHandle)e.Item.FindControl("RadListViewItemDragHandle1");

        if (isAppConfirmed || !AllowCompletenessSaveDate)
        {
            RadListViewItemDragHandle1.Enabled = false;
            RadListViewItemDragHandle1.Visible = false;
        }

        // Replaced by Lexin on 4 May 2012 to resolve thumbnail blur issue.
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

    #region Validation
    protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        if (AcceptanceRadioButtonList.SelectedValue == "N" && DocConditionRadioButtonList.SelectedValue.Trim().ToString().Equals(DocumentConditionEnum.Amended.ToString()))
        {
            if (!String.IsNullOrEmpty(ExceptionTextBox.Text.Trim()) && ExceptionTextBox.Text.Trim().Length <= 255)
            {
                result = true;
            }
        }
        else
            result = true;

        args.IsValid = result;
    }
    #endregion

    #region Private Methods

    protected void RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate()
    {
        DocAppDb docAppDb = new DocAppDb();
        isAppConfirmed = docAppDb.IsAppConfirmed(docAppId.Value);

        if (isAppConfirmed)
            RedirectToDefaultPage();

        if (!AllowCompletenessSaveDate)
            RedirectToDefaultPage();
    }

    protected void RedirectToDefaultPage()
    {
        Response.Redirect("view.aspx?id=" + docAppId.Value);
        return;
    }

    private void LoadContextContent(Boolean fromTreeNode)
    {
        if (Request.Cookies["REF" + docAppId.ToString()] != null)
        {
            HttpCookie userSelectionCookie = Request.Cookies["REF" + docAppId.ToString()];


            if (!fromTreeNode)
                RadTreeView1_NodeClick(null, null);

	        string selectedReferenceTab = userSelectionCookie["selectedReferenceTab"] == null ? 
		        string.Empty : userSelectionCookie["selectedReferenceTab"].ToString().ToUpper().Trim();

            string selectedDocumentTab = userSelectionCookie["selectedDocumentTab"] == null ?
                string.Empty : userSelectionCookie["selectedDocumentTab"].ToString().ToUpper().Trim();

            if (RadTreeView1.SelectedNode.Category.Trim().ToLower() == "ref")
            {
                switch (selectedReferenceTab)
                {
                    case "SUMMARY":
                        SummaryRadButton_Click(null, EventArgs.Empty);
                        break;
                    case "OUTSTANDING":
                        OutstandingDocsRadButton_Click(null, EventArgs.Empty);
                        break;
                    case "LOGREF":
                        LogRefButton_Click(null, EventArgs.Empty);
                        break;
                    default:
                        SummaryRadButton_Click(null, null);
                        break;
                }
            }
            else
            {
                switch (selectedDocumentTab)
                {
                    case "THUMBNAILS":
                        ThumbnailButton_Click(null, EventArgs.Empty);
                        break;
                    case "IMAGE":
                        ImgButton_Click(null, EventArgs.Empty);
                        break;
                    case "LOGDOC":
                        LogDocButton_Click(null, EventArgs.Empty);
                        break;
                    default:
                        ImgButton_Click(null, null);
                        break;
                }
            }
        }
    }

    private string GetUserSelectionSelectedNode()
    {
        if (Request.Cookies["REF" + docAppId.ToString()] != null)
        {
            HttpCookie userSelectionCookie = Request.Cookies["REF" + docAppId.ToString()];
            return userSelectionCookie.Values["SelectedNode"].ToString();
        }
        else
            return "0";
    }

    private void SetUserSelectionCookie(string selectedReferenceTab, string selectedDocumentTab, string selectedNode)
    {
        if (Request.Cookies["REF" + docAppId.ToString()] == null)
        {
            HttpCookie userSelectionCookie = new HttpCookie("REF" + docAppId.ToString());

            if (!string.IsNullOrEmpty(selectedReferenceTab))
                userSelectionCookie.Values["selectedReferenceTab"] = selectedReferenceTab.ToUpper().Trim();
            else
                userSelectionCookie.Values["selectedReferenceTab"] = string.Empty;

            if (!string.IsNullOrEmpty(selectedDocumentTab))
                userSelectionCookie.Values["selectedDocumentTab"] = selectedDocumentTab.ToUpper().Trim();
            else
                userSelectionCookie.Values["selectedDocumentTab"] = string.Empty;

            if (!string.IsNullOrEmpty(selectedNode))
                userSelectionCookie.Values["SelectedNode"] = selectedNode;
            else
                userSelectionCookie.Values["SelectedNode"] = "0";

            userSelectionCookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(userSelectionCookie);
        }
        else
        {
            HttpCookie userSelectionCookie = Request.Cookies["REF" + docAppId.ToString()];

            if (!string.IsNullOrEmpty(selectedReferenceTab))
                userSelectionCookie.Values["selectedReferenceTab"] = selectedReferenceTab.ToUpper().Trim();

            if (!string.IsNullOrEmpty(selectedDocumentTab))
                userSelectionCookie.Values["selectedDocumentTab"] = selectedDocumentTab.ToUpper().Trim();

            if (!string.IsNullOrEmpty(selectedNode))
                userSelectionCookie.Values["SelectedNode"] = selectedNode;

            Response.AppendCookie(userSelectionCookie);
        }
    }

    protected void populateRefInfo()
    {
        ThumbnailPanel.Visible = ExportButton.Visible = LogPanel.Visible = ExtractButton.Visible = MetaDataPanel.Visible = pdfframe.Visible = false;
        SummaryPanel.Visible = true;
        OutstandingPanel.Visible = !SummaryPanel.Visible;
        PopulatePersonals(true);
        SetButtonClass("Summary");

        DocAppDb docAppDb = new DocAppDb();
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

        if (docApps.Rows.Count > 0)
        {
            DocApp.DocAppRow docAppRow = docApps[0];

            SetAccessControl();     //Added By Edward 04.11.2013 Confirm All Acceptance

            if (docAppRow["Status"].ToString().Trim().Equals("Completeness_Checked"))
            {
                SetConfirmLabel.Text = Constants.RefVerified;
                LeftConfirmButton.Visible = false;
                ConfirmAllAcceptanceButton.Visible = false; //Added By Edward 04.11.2013 Confirm All Acceptance
                ConfirmExtractButton.Visible = false; 
            }
            else if (docAppRow["Status"].ToString().Trim().Equals("Closed"))
            {
                SetConfirmLabel.Text = Constants.RefClosed;
                LeftConfirmButton.Visible = false;
                ConfirmAllAcceptanceButton.Visible = false; //Added By Edward 04.11.2013 Confirm All Acceptance
                ConfirmExtractButton.Visible = false; 
            }
            else
            {
                SetConfirmLabel.Text = Constants.RefNotVerified;
                #region this part will hide the Left Confirm button when the Confrim Extract button is visible Added by Edward 2014/9/18
                if (!ConfirmExtractButton.Visible)
                    LeftConfirmButton.Visible = true;
                else
                {
                    LeftConfirmButton.Visible = false;
                    ConfirmAllAcceptanceButton.Visible = false;
                }
                    
                #endregion
                //ConfirmAllAcceptanceButton.Visible = true;  //Added By Edward 04.11.2013 Confirm All Acceptance
                //ConfirmExtractButton.Visible = true; 
            }            

            ProfileDb profilDb = new ProfileDb();

            if (!string.IsNullOrEmpty(docAppRow["CompletenessStaffUserId"].ToString()))
            {
                Guid VerificationStaffUserId = new Guid(docAppRow["CompletenessStaffUserId"].ToString());
                string assignedTo = profilDb.GetUserFullName(VerificationStaffUserId);
                AppVerificationOfficerLabel.Text = string.IsNullOrEmpty(assignedTo.Trim()) ? "App not assigned" : "Assigned to: " + assignedTo;
            }
            else
                AppVerificationOfficerLabel.Text = "App not assigned";


            RightDocumentLabel.Text = docAppRow.RefType + ": " + docAppRow.RefNo;

            //attach address for sers ref type
            if (docAppRow.RefType.ToLower().Equals("sers"))
            {
                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                RightDocumentLabel.Text += " (" + sersInterfaceDb.GetSersAddressByRefNo(docAppRow.RefNo) + ")";
            }

            //attach hlestatus for hle ref type

            if (docAppRow.RefType.ToLower().Equals("hle"))
            {
                HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                HleInterface.HleInterfaceDataTable hleInterface = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);
                if (hleInterface.Rows.Count > 0)
                {
                    HleInterface.HleInterfaceRow hleInterfaceRow = hleInterface[0];
                    RightDocumentLabel.Text += " (HLE Status: " + (string.IsNullOrEmpty(hleInterfaceRow.HleStatus.Trim()) ? " - " : hleInterfaceRow.HleStatus.Replace("_", " ")) + ")";
                }
                else
                    RightDocumentLabel.Text += " (HLE Status: - )";
            }

            //get the outstanding documents 
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            RadToolBarButton outstandingButton = RadToolBarMenuRef.FindItemByValue("Outstanding") as RadToolBarButton;

            if (outstandingButton != null)
                outstandingButton.Text = "Outstanding Documents (" + leasInterfaceDb.GetCountByHleNUmberGroupByCategoryInfo(docAppRow.RefNo).ToString() + ")";
        }
        else
            Response.Redirect("~/Default.aspx");

        DocButtonPanel.Visible = false;
        RefButtonPanel.Visible = true;

        // #########################################
        // Dummy (13/06/2012) - to be removed after DEMO on 13/06/2012
        if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.HLE.ToString()))
        {
            HleSummaryRepeater.Visible = true;
            ResaleRepeater.Visible = false;
            SersRepeater.Visible = false;
            SersInfoPanel.Visible = false;
        }
        else if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.RESALE.ToString()))
        {
            HleSummaryRepeater.Visible = false;
            ResaleRepeater.Visible = true;
            SersRepeater.Visible = false;
            SersInfoPanel.Visible = false;
        }
        else if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.SERS.ToString()))
        {
            HleSummaryRepeater.Visible = false;
            ResaleRepeater.Visible = false;
            SersRepeater.Visible = true;
            SersInfoPanel.Visible = true;
        }
        // #########################################
    }

    protected void PopulateSplitType()
    {
        SplitTypeRadioButtonList.DataSource = Enum.GetValues(typeof(SplitTypeEnum));
        SplitTypeRadioButtonList.DataBind();
        SplitTypeRadioButtonList.SelectedIndex = 0;
    }

    private void PopulateTreeView()
    {
        DocSetDb docSetDb = new DocSetDb();

        TreeviewDWMS.PopulateTreeViewCompleteness(RadTreeView1, docAppId.Value, false);

        //we need to set this for remember the treeview node expanded state
        // http://www.telerik.com/community/code-library/aspnet-ajax/treeview/save-the-expanded-state-of-the-treenodes-when-the-treeview-is-being-bound-upon-each-postback.aspx
        RadTreeView1.DataValueField = "TreeviewID";

        selectedNodeGlobal = GetUserSelectionSelectedNode();

        #region handle treeview node expansion state from cookie
        HttpCookie collapsedNodeCookie = Request.Cookies["ref" + docAppId.Value + "collapsedNodes"];
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
                SelNode.ExpandParentNodes();
            }
            else
                RadTreeView1.Nodes[0].Selected = true;
        }
        #endregion

        #region lock node drag when application is checked/verified or user do not have access to make changes to the application
        bool lockDrag = AllowCompletenessSaveDate;
        if (isAppConfirmed)
            lockDrag = false;
        foreach (RadTreeNode node in RadTreeView1.GetAllNodes())
        {
            if (node.AllowDrag == true)
                node.AllowDrag = lockDrag;
        }
        #endregion
    }

    protected void SetButtonVisibility(string category)
    {
        DocButtonPanel.Visible = !category.Equals("ref");
        RefButtonPanel.Visible = category.Equals("ref");
    }

    protected void LoadMetaData()
    {
        PopulateDocumentTypes(int.Parse(RadTreeView1.SelectedValue));
        PopulateDocData(int.Parse(RadTreeView1.SelectedValue));
        PopulateMetaData(int.Parse(RadTreeView1.SelectedValue));
        PopulateAdditionalMetaData(int.Parse(RadTreeView1.SelectedValue));

        DisplayPdfDocument(int.Parse(RadTreeView1.SelectedValue));
    }

    private void PopulateAdditionalMetaData(int docId)
    {
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));
        Doc.DocRow docRow = docs[0];

        //get the reference to the control
        UserControl userControl = (UserControl)AdditionalMetaDataPanel.FindControl(docRow.DocTypeCode + "MetaDataUC");
        SetControlVisibility(userControl);
        LoadSaveControlData(docRow.DocTypeCode, true, false, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());
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
        //DocDb docDb = new DocDb();

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
                    validDate = BankStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "baptism":
                BaptismMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    BaptismMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //BaptismMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = BaptismMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
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
                    validDate = BusinessProfileMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "cbr":
                CBRMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CBRMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "commissionstatement":
                CommissionStatementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CommissionStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CommissionStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "cpfcontribution":             
                CPFContributionMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFContributionMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CPFContributionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;           
            case "cpfstatement":
                CPFStatementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = CPFStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "cpfstatementrefund":
                CPFStatementRefundMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    CPFStatementRefundMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    CPFStatementRefundMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = CPFStatementRefundMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificate":
                DeathCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificateexsp":
                DeathCertificateEXSPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateEXSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateEXSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificatefa":
                DeathCertificateFaMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateFaMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateFaMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificatemo":
                DeathCertificateMoMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateMoMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateMoMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificatenric":
                DeathCertificateNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificatesp":
                DeathCertificateSPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "declaraincomedetails":
                DeclaraIncomeDetailsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeclaraIncomeDetailsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DeclaraIncomeDetailsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeclaraIncomeDetailsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "declarationprivprop":
                DeclarationPrivPropMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeclarationPrivPropMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DeclarationPrivPropMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeclarationPrivPropMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deedpoll":
                DeedPollMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedPollMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedPollMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedPollMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deedseparation":
                DeedSeparationMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeparationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deedseverance":
                DeedSeveranceMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeveranceMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeveranceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeveranceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertificate":
                DivorceCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertchild":
                DivorceCertChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertexspouse":
                DivorceCertExSpouseMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertExSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertExSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertExSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertfather":
                DivorceCertFatherMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertFatherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertFatherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertFatherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertmother":
                DivorceCertMotherMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertMotherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertMotherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertMotherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcecertnric":
                DivorceCertNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceCertNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceCertNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceCertNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcedocinitial":
                DivorceDocInitialMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocInitialMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocInitialMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocInitialMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcedocinterim":
                DivorceDocInterimMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocInterimMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "divorcedocfinal":
                DivorceDocFinalMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DivorceDocFinalMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    DivorceDocFinalMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DivorceDocFinalMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "doceduinstitute":
                DocEduInstituteMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DocEduInstituteMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DocEduInstituteMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "docoffincommitment":  //Added By Edward 2015/05/18 New Document Types
                DocOfFinCommitmentMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DocOfFinCommitmentMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DocOfFinCommitmentMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "employmentpass":
                EmploymentPassMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EmploymentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = EmploymentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "employmentletter":
                EmploymentLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EmploymentLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //EmploymentLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = EmploymentLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "entrypermit":
                EntryPermitMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    EntryPermitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = EntryPermitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "gla":
                GLAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    GLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    GLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = GLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "identitycard":
                IdentityCardMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IdentityCardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "identitycardchild":  //Added By Edward 2015/05/18   New Document Types
                IdentityCardChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "irasassesement":
                IRASAssesementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IRASAssesementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IRASAssesementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IRASAssesementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "irasir8e":
                IRASIR8EMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IRASIR8EMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    IRASIR8EMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IRASIR8EMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "lettersolicitorpoa":
                LettersolicitorPOAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LettersolicitorPOAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LettersolicitorPOAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LettersolicitorPOAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "licenseoftrade":
                LicenseofTradeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LicenseofTradeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LicenseofTradeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LicenseofTradeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "loanstatementsold":
                LoanStatementSoldMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    LoanStatementSoldMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    LoanStatementSoldMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = LoanStatementSoldMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "marriagecertificate":
                MarriageCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "marriagecertchild":
                MarriageCertChildMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "marriagecertltspouse":
                MarriageCertLtSpouseMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertLtSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertLtSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "marriagecertparent":
                MarriageCertParentMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertParentMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertParentMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "marriagecertsibling": //Added By Edward 2015/05/18 New Document Types
                MarriageCertSiblingMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MarriageCertSiblingMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = MarriageCertSiblingMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "mortgageloanform":
                MortgageLoanFormMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MortgageLoanFormMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    MortgageLoanFormMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "meddocudoctorletters":  //Added By Edward 2015/05/25 New Document Types
                MedDocuDoctorLettersMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    MedDocuDoctorLettersMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = MedDocuDoctorLettersMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
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
                    validDate = NoticeofTransferMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "notessyariahcourt":  //Added By Edward 2015/05/18   New Document Types
                NotesSyariahCourtMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NotesSyariahCourtMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NotesSyariahCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "nsenlistmentnotice":  //Added By Edward 2015/05/18   New Document Types
                NSEnlistmentNoticeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NSEnlistmentNoticeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NSEnlistmentNoticeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "nsordcertificate":  //Added By Edward 2015/05/18   New Document Types
                NSORDCertificateMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    NSORDCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = NSORDCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
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
                    validDate = OptionPurchaseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "orderofcourt":
                OrderofCourtMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OrderofCourtMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //OrderofCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = OrderofCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "orderofcourtdivorce":
                OrderofCourtDivorceMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OrderofCourtDivorceMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //    DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = OrderofCourtDivorceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "overseasincome":
                OverseasIncomeMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    OverseasIncomeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = OverseasIncomeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "passport":
                PassportMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PassportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PassportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "payslip":
                PAYSLIPMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PAYSLIPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PAYSLIPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "pensionerletter":
                PensionerLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PensionerLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PensionerLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PensionerLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "petitionforgla":
                PetitionforGLAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PetitionforGLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PetitionforGLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PetitionforGLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "powerattorney":
                PowerAttorneyMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PowerAttorneyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    PowerAttorneyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = PowerAttorneyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "prisonletter":
                PrisonLetterMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PrisonLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PrisonLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PrisonLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "propertyquestionaire":
                PropertyQuestionaireMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyQuestionaireMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PropertyQuestionaireMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PropertyQuestionaireMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            #region Added by Edward 2017/10/03 New Document Types Property Tax
            case "propertytax":
                PropertyTaxMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyTaxMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PropertyTaxMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "propertytaxnric":
                PropertyTaxNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PropertyTaxNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = PropertyTaxNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            #endregion
            case "purchaseagreement":
                PurchaseAgreementMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    PurchaseAgreementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //PurchaseAgreementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = PurchaseAgreementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "receiptsloanarrear":
                ReceiptsLoanArrearMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ReceiptsLoanArrearMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ReceiptsLoanArrearMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = ReceiptsLoanArrearMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "reconciliatundertakn":
                ReconciliatUndertaknMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ReconciliatUndertaknMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ReconciliatUndertaknMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = ReconciliatUndertaknMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "rentalarrears":
                RentalArrearsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    RentalArrearsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //RentalArrearsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = RentalArrearsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "socialvisit":
                SocialVisitMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    SocialVisitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = SocialVisitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "statementofaccounts":
                StatementofAccountsMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatementofAccountsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatementofAccountsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "statementsale":
                StatementSaleMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatementSaleMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    StatementSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = StatementSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "statutorydeclaration":
                StatutoryDeclarationMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatutoryDeclarationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatutoryDeclarationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "statutorydeclgeneral":
                StatutoryDeclGeneralMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StatutoryDeclGeneralMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = StatutoryDeclGeneralMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    break;
            case "studentpass":
                StudentPassMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    StudentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    StudentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                validDate = StudentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "valuationreport":
                ValuationReportMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    ValuationReportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //ValuationReportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = ValuationReportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "warranttoact":
                WarranttoActMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    WarranttoActMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                //    WarranttoActMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = WarranttoActMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            #region  Added by Edward 2017/03/30 New Document Types
            case "deathcertificatepa":
                DeathCertificatePAMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificatePAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificatePAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deathcertificatelo":
                DeathCertificateLOMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeathCertificateLOMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = DeathCertificateLOMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "deedseparationnric":
                DeedSeparationNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    DeedSeparationNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = DeedSeparationNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "identitycardnric":
                IdentityCardNRICMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    //CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                    validDate = IdentityCardNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            #endregion
            #region Added by Edward 2017/12/22 New Document Types Identity Card Father Mother
            case "identitycardfa":
                IdentityCardFaMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardFaMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else                    
                    validDate = IdentityCardFaMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            case "identitycardmo":
                IdentityCardMoMetaDataUC.CurrentDocType = currentDocType2;
                if (isLoad)
                    IdentityCardMoMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    validDate = IdentityCardMoMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
            #endregion
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
                EmptyMetaDataUC.ActualDocType = docEmptyRow.DocTypeCode;

                if (isLoad)
                    EmptyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
                else
                    EmptyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable, isValidate);
                break;
        }
        return validDate;
            //case "hle":
            //    HLEMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        HLEMetaDataUC.LoadData(docId);
            //    else
            //        HLEMetaDataUC.SaveData(docId);
            //    break;
            //case "birthcertificate":
            //    BirthCertificateMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        BirthCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        BirthCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "birthcertificatchild":
            //    BirthCertificatChildMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        BirthCertificatChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        BirthCertificatChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "cpfstatement":
            //    CPFStatementMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        CPFStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        CPFStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificate":
            //    DeathCertificateMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificatefa":
            //    DeathCertificateFaMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateFaMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateFaMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificatemo":
            //    DeathCertificateMoMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateMoMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateMoMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificatesp":
            //    DeathCertificateSPMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificateexsp":
            //    DeathCertificateEXSPMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateEXSPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateEXSPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deathcertificatenric":
            //    DeathCertificateNRICMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeathCertificateNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeathCertificateNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "marriagecertificate":
            //    MarriageCertificateMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        MarriageCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        MarriageCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "marriagecertparent":
            //    MarriageCertParentMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        MarriageCertParentMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        MarriageCertParentMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "marriagecertltspouse":
            //    MarriageCertLtSpouseMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        MarriageCertLtSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        MarriageCertLtSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "marriagecertchild":
            //    MarriageCertChildMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        MarriageCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        MarriageCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "cbr":
            //    CBRMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        CBRMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        CBRMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "irasir8e":
            //    IRASIR8EMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        IRASIR8EMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        IRASIR8EMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "irasassesement":
            //    IRASAssesementMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        IRASAssesementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        IRASAssesementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "cpfcontribution":
            //    CPFContributionMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        CPFContributionMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        CPFContributionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "statutorydeclaration":
            //    StatutoryDeclarationMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        StatutoryDeclarationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        StatutoryDeclarationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "statutorydeclgeneral":
            //    StatutoryDeclGeneralMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        StatutoryDeclGeneralMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        StatutoryDeclGeneralMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "declarationprivprop":
            //    DeclarationPrivPropMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeclarationPrivPropMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeclarationPrivPropMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "payslip":
            //    PAYSLIPMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PAYSLIPMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PAYSLIPMetaDataUC.SaveData(docId, docRefId, referencePersonalTable,true);
            //    break;
            //case "commissionstatement":
            //    CommissionStatementMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        CommissionStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        CommissionStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "overseasincome":
            //    OverseasIncomeMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        OverseasIncomeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        OverseasIncomeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "noloannotification":
            //    NoLoanNotificationMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        NoLoanNotificationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        NoLoanNotificationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "identitycard":
            //    IdentityCardMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        IdentityCardMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        IdentityCardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "nsidcard":
            //    NSIDcardMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        NSIDcardMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        NSIDcardMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "passport":
            //    PassportMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PassportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PassportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "entrypermit":
            //    EntryPermitMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        EntryPermitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        EntryPermitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "employmentpass":
            //    EmploymentPassMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        EmploymentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        EmploymentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "socialvisit":
            //    SocialVisitMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        SocialVisitMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        SocialVisitMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "studentpass":
            //    StudentPassMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        StudentPassMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        StudentPassMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "prisonletter":
            //    PrisonLetterMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PrisonLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PrisonLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            ////case "spouseformsale":
            ////    SpouseFormSaleMetaDataUC.CurrentDocType = currentDocType;
            ////    if (isLoad)
            ////        SpouseFormSaleMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            ////    else
            ////        SpouseFormSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            ////    break;
            //case "declaraincomedetails":
            //    DeclaraIncomeDetailsMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeclaraIncomeDetailsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeclaraIncomeDetailsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "lettersolicitorpoa":
            //    LettersolicitorPOAMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        LettersolicitorPOAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        LettersolicitorPOAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            ////case "spouseformpurchase":
            ////    SpouseFormPurchaseMetaDataUC.CurrentDocType = currentDocType;
            ////    if (isLoad)
            ////        SpouseFormPurchaseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            ////    else
            ////        SpouseFormPurchaseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            ////    break;
            //case "valuationreport":
            //    ValuationReportMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        ValuationReportMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        ValuationReportMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "reconciliatundertakn":
            //    ReconciliatUndertaknMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        ReconciliatUndertaknMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        ReconciliatUndertaknMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "receiptsloanarrear":
            //    ReceiptsLoanArrearMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        ReceiptsLoanArrearMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        ReceiptsLoanArrearMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "purchaseagreement":
            //    PurchaseAgreementMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PurchaseAgreementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PurchaseAgreementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "rentalarrears":
            //    RentalArrearsMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        RentalArrearsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        RentalArrearsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "petitionforgla":
            //    PetitionforGLAMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PetitionforGLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PetitionforGLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "orderofcourt":
            //    OrderofCourtMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        OrderofCourtMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        OrderofCourtMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "noticeoftransfer":
            //    NoticeofTransferMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        NoticeofTransferMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        NoticeofTransferMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "gla":
            //    GLAMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        GLAMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        GLAMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "warranttoact":
            //    WarranttoActMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        WarranttoActMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        WarranttoActMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "cpfstatementrefund":
            //    CPFStatementRefundMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        CPFStatementRefundMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        CPFStatementRefundMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "statementsale":
            //    StatementSaleMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        StatementSaleMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        StatementSaleMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "propertyquestionaire":
            //    PropertyQuestionaireMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PropertyQuestionaireMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PropertyQuestionaireMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "loanstatementsold":
            //    LoanStatementSoldMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        LoanStatementSoldMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        LoanStatementSoldMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "adoption":
            //    AdoptionMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        AdoptionMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        AdoptionMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deedpoll":
            //    DeedPollMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeedPollMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeedPollMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "powerattorney":
            //    PowerAttorneyMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PowerAttorneyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PowerAttorneyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "officialassignee":
            //    OfficialAssigneeMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        OfficialAssigneeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        OfficialAssigneeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "baptism":
            //    BaptismMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        BaptismMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        BaptismMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "employmentletter":
            //    EmploymentLetterMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        EmploymentLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        EmploymentLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "bankstatement":
            //    BankStatementMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        BankStatementMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        BankStatementMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "statementofaccounts":
            //    StatementofAccountsMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        StatementofAccountsMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        StatementofAccountsMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "pensionerletter":
            //    PensionerLetterMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        PensionerLetterMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        PensionerLetterMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "businessprofile":
            //    BusinessProfileMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        BusinessProfileMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        BusinessProfileMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "licenseoftrade":
            //    LicenseofTradeMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        LicenseofTradeMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        LicenseofTradeMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deedseparation":
            //    DeedSeparationMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeedSeparationMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeedSeparationMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "deedseverance":
            //    DeedSeveranceMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DeedSeveranceMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DeedSeveranceMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcedocinitial":
            //    DivorceDocInitialMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceDocInitialMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceDocInitialMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcedocinterim":
            //    DivorceDocInterimMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceDocInterimMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceDocInterimMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcedocfinal":
            //    DivorceDocFinalMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceDocFinalMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceDocFinalMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertificate":
            //    DivorceCertificateMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertificateMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertificateMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertfather":
            //    DivorceCertFatherMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertFatherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertFatherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertmother":
            //    DivorceCertMotherMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertMotherMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertMotherMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertexspouse":
            //    DivorceCertExSpouseMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertExSpouseMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertExSpouseMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertnric":
            //    DivorceCertNRICMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertNRICMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertNRICMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "divorcecertchild":
            //    DivorceCertChildMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        DivorceCertChildMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        DivorceCertChildMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //case "sers":
            //    SERSMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        SERSMetaDataUC.LoadData(docId);
            //    else
            //        SERSMetaDataUC.SaveData(docId);
            //    break;
            //case "sales":
            //    SalesMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        SalesMetaDataUC.LoadData(docId);
            //    else
            //        SalesMetaDataUC.SaveData(docId);
            //    break;
            //case "resale":
            //    ResaleMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        ResaleMetaDataUC.LoadData(docId);
            //    else
            //        ResaleMetaDataUC.SaveData(docId);
            //    break;
            //case "mortgageloanform":
            //    MortgageLoanFormMetaDataUC.CurrentDocType = currentDocType;
            //    if (isLoad)
            //        MortgageLoanFormMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        MortgageLoanFormMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
            //default:
            //    EmptyMetaDataUC.Visible = true;
            //    EmptyMetaDataUC.CurrentDocType = currentDocType;
            //    Doc.DocDataTable docsEmpty = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));
            //    Doc.DocRow docEmptyRow = docsEmpty[0];
            //    EmptyMetaDataUC.ActualDocType = docEmptyRow.DocTypeCode;

            //    if (isLoad)
            //        EmptyMetaDataUC.LoadData(docId, docRefId, referencePersonalTable, isUnderHouseholdStructureNRIC);
            //    else
            //        EmptyMetaDataUC.SaveData(docId, docRefId, referencePersonalTable);
            //    break;
        //}
    }
    #region Modified by Edward Displaying Active DocTypes 2015/8/17
    //private void PopulateDocumentTypes(int docId)
    //{
    //    DocDb docDb = new DocDb();
    //    string docTypeCode = string.Empty;
    //    Doc.DocDataTable docs = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

    //    Doc.DocRow docRow = docs[0];
    //    docTypeCode = docRow.DocTypeCode;
    //    DocTypeDb docTypeDb = new DocTypeDb();
    //    //Added by Edward Displaying Active DocTypes 2015/8/17
    //    //DocTypeDropDownList.DataSource = docTypeDb.GetDocType();
    //    DocTypeDropDownList.DataSource = docTypeDb.GetActiveDocTypes();

    //    //Added by Edward 2015/8/26 To Address Selection Out Of Range. Checking if it is Active
    //    DocType.DocTypeDataTable docTypeDt;
    //    bool docTypeIsActive = true;
    //    string docTypeDesc = string.Empty;
    //    if (!string.IsNullOrEmpty(docTypeCode))
    //    {
    //        docTypeDt = docTypeDb.GetDocType(docTypeCode);
    //        if (docTypeDt.Rows.Count > 0)
    //        {
    //            DocType.DocTypeRow docTypeR = docTypeDt[0];
    //            docTypeIsActive = !docTypeR.IsIsActiveNull() ? docTypeR.IsActive : docTypeIsActive;
    //            docTypeDesc = docTypeR.Description;
    //        }
    //    }

    //    DocTypeDropDownList.DataTextField = "Description";
    //    DocTypeDropDownList.DataValueField = "Code";
    //    if (docTypeIsActive)
    //        DocTypeDropDownList.SelectedValue = docTypeCode;
    //    else
    //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('This doctype {0} is already inactive');", docTypeDesc), true);
    //    DocTypeDropDownList.DataBind();

    //    //remove the irrelavant application form
    //    List<string> applicationToKeep = new List<string>();
    //    AppPersonalDb appPersonalDb = new AppPersonalDb();
    //    DataTable appPersonals = appPersonalDb.GetAppPersonalReferenceDetailsByDocId(docRow.Id);

    //    //first check if the document is attached to any application [Docments inside the application folder]
    //    if (appPersonals.Rows.Count > 0)
    //        applicationToKeep.Add(appPersonals.Rows[0]["RefType"].ToString().ToUpper().Trim());
    //    else // if the document is not attached to any application, then get the applications related to the DocSets. [documents outside the application folder]
    //    {
    //        DocSetDb docSetDb = new DocSetDb();
    //        DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId.Value);

    //        foreach (DocSet.DocSetRow docSetRow in docSets.Rows)
    //        {
    //            DataTable referenceDetails = docSetDb.GetReferenceDetailsById(docSetRow.Id);
    //            foreach (DataRow referenceDetailsRow in referenceDetails.Rows)
    //            {
    //                applicationToKeep.Add(referenceDetailsRow["RefType"].ToString().Trim());
    //            }
    //        }
    //    }

    //    foreach (ScanningTransactionTypeEnum ScanningTransactionType in ScanningTransactionTypeEnum.GetValues(typeof(ScanningTransactionTypeEnum)))
    //    {
    //        if (!applicationToKeep.Contains(ScanningTransactionType.ToString().ToUpper()))
    //            DocTypeDropDownList.Items.Remove(DocTypeDropDownList.Items.FindItemByValue(ScanningTransactionType.ToString()));
    //    } 
    //}

    private void PopulateDocumentTypes(int docId)
    {
        DocDb docDb = new DocDb();
        string docTypeCode = string.Empty;
        Doc.DocDataTable docs = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));

        Doc.DocRow docRow = docs[0];
        docTypeCode = docRow.DocTypeCode;
        DocTypeDb docTypeDb = new DocTypeDb();
        //Added by Edward Displaying Active DocTypes 2015/8/17
        //DocTypeDropDownList.DataSource = docTypeDb.GetDocType();
        DocTypeDropDownList.DataSource = docTypeDb.GetActiveDocTypes();

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

            if (!AllowCompletenessSaveDate)
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
        else // if the document is not attached to any application, then get the applications related to the DocSets. [documents outside the application folder]
        {
            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId.Value);

            foreach (DocSet.DocSetRow docSetRow in docSets.Rows)
            {
                DataTable referenceDetails = docSetDb.GetReferenceDetailsById(docSetRow.Id);
                foreach (DataRow referenceDetailsRow in referenceDetails.Rows)
                {
                    applicationToKeep.Add(referenceDetailsRow["RefType"].ToString().Trim());
                }
            }
        }

        foreach (ScanningTransactionTypeEnum ScanningTransactionType in ScanningTransactionTypeEnum.GetValues(typeof(ScanningTransactionTypeEnum)))
        {
            if (!applicationToKeep.Contains(ScanningTransactionType.ToString().ToUpper()))
                DocTypeDropDownList.Items.Remove(DocTypeDropDownList.Items.FindItemByValue(ScanningTransactionType.ToString()));
        }
    }

    #endregion

    private void PopulateDocData(int docId)
    {
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(int.Parse(RadTreeView1.SelectedValue));
        Doc.DocRow docRow = docs[0];

        currentDocType = docRow.DocTypeCode;

        MasterListItemDb masterListItemDb = new MasterListItemDb();
        ImageConditionDropDownList.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Image_Condition.ToString().Replace("_", " "));
        ImageConditionDropDownList.DataTextField = "Name";
        ImageConditionDropDownList.DataValueField = "Name";
        ImageConditionDropDownList.DataBind();

        ImageConditionDropDownList.SelectedValue = docRow.ImageCondition.Trim();
        if (ImageConditionDropDownList.SelectedValue.Equals("Blur/Incomplete"))
            SubmitAndConfirmMDButton.CausesValidation = false;

        ImageConditionDropDownList.DataBind();

        oldImageCondition = ImageConditionDropDownList.SelectedValue;

        AppPersonalDb appPersonalDb = new AppPersonalDb();
        DataTable appPersonals = appPersonalDb.GetAppPersonalReferenceDetailsByDocId(docRow.Id);

        if (appPersonals.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(appPersonals.Rows[0]["RefType"].ToString().Trim()))
            {
                RefNoHeadingLabel.Text = appPersonals.Rows[0]["RefType"].ToString() + " No";
                RefNoValueLabel.Text = appPersonals.Rows[0]["RefNo"].ToString();
            }
        }
        else
        {
            RefNoHeadingLabel.Text = "Reference No";
            RefNoValueLabel.Text = "NA ";
        }

        //set documentcondition, remark and acceptance
        DocConditionRadioButtonList.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Document_Condition.ToString().Replace("_", " "));
        DocConditionRadioButtonList.DataTextField = "Name";
        DocConditionRadioButtonList.DataValueField = "Name";
        DocConditionRadioButtonList.DataBind();
        DocConditionRadioButtonList.SelectedValue = (docRow.IsDocumentConditionNull() ?
            DocumentConditionEnum.NA.ToString() : docRow.DocumentCondition.Trim());

        ExceptionTextBox.Text = (docRow.IsExceptionNull() ? string.Empty : docRow.Exception);

        //AcceptanceRadioButtonList.SelectedValue = (!docRow.IsIsAcceptedNull() && docRow.IsAccepted ? "1" : "0");
        if (docRow.IsImageAcceptedNull() || (docRow.ImageAccepted.ToUpper() != "Y" && docRow.ImageAccepted.ToUpper() != "N" && docRow.ImageAccepted.ToUpper() != "X"))
            AcceptanceRadioButtonList.ClearSelection();
        else
            AcceptanceRadioButtonList.SelectedValue = docRow.ImageAccepted.ToUpper();

    }

    private void PopulateMetaData(int docId)
    {
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
        int docId = int.Parse(RadTreeView1.SelectedNode.Value);

        //save additional metadata
        validDate = LoadSaveControlData(DocTypeDropDownList.SelectedValue.Trim(), false, isValidate, int.Parse(RadTreeView1.SelectedNode.Attributes["DocRefId"].ToString().Trim()), RadTreeView1.SelectedNode.Attributes["referencePersonalTable"].ToString().Trim().ToLower());

        if (validDate)
        {
            //save the meta data
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
                    metaDataDb.DeleteStampDataByDocID(int.Parse(RadTreeView1.SelectedValue));

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
                //metaDataDb.DeleteStampDataByDocID(int.Parse(RadTreeView1.SelectedValue));
            }
        }

        DocDb docDb = new DocDb();
        //update doc details
        if (isConfirm && validDate)
        {
            docDb.UpdateDocFromMeta(docId, DocStatusEnum.Completed, ImageConditionDropDownList.SelectedValue, DocTypeDropDownList.SelectedValue.Trim(), true, LogTypeEnum.C);
            docDb.UpdateDocDetails(docId, ExceptionTextBox.Text, AcceptanceRadioButtonList.SelectedValue, DocStatusEnum.Completed);
            //docDb.UpdateSendToCdbStatus(docId, SendToCDBStatusEnum.Ready);
            docDb.UpdateSendToCdbAccept(docId, SendToCDBStatusEnum.Ready);

            //update the SendToCdbStatus if the imageCondition is changed from blur to NA
            if (ImageConditionDropDownList.SelectedValue.Equals("NA") && oldImageCondition.Equals("Blur/Incomplete"))
            {
                //reset SendToCdb Flags
                docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                int setId = docDb.GetSetIdByDocId(docId);
                DocSetDb docSetDb = new DocSetDb();
                docSetDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(setId);
            }
        }
        else if(validDate)
        {
            docDb.UpdateDocFromMeta(docId, DocStatusEnum.Verified, ImageConditionDropDownList.SelectedValue, DocTypeDropDownList.SelectedValue.Trim(), false, LogTypeEnum.C);
            docDb.UpdateDocDetails(docId, ExceptionTextBox.Text, AcceptanceRadioButtonList.SelectedValue, DocStatusEnum.Verified);
        }

        //update document condition
        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), DocConditionRadioButtonList.SelectedValue, true);
        docDb.UpdateDocumentCondition(docId, documentConditionEnum);

        currentDocType = DocTypeDropDownList.SelectedValue.Trim();

        //uppdate app status to completeness_In_progress
        DocAppDb docAppDb = new DocAppDb();
        docAppDb.UpdateRefStatus(docAppId.Value, AppStatusEnum.Completeness_In_Progress, false, false, null);

        selectedNodeGlobal = RadTreeView1.SelectedValue;
        if (validDate)
        {
            MetDataConfirmPanelLabel.Text = String.Format("The metadata has been {0} successfully.", (isConfirm ? "confirmed" : "saved"));
            MetDataConfirmPanel.Visible = true;
        }
        PopulateTreeView();
        ResetPageClosingFlags();
        //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
        //SetRightDocumentLabelText(docId);
        SetRightDocumentLabelText(docId.ToString());
    }






    private void PopulateGrid(int docDetailId)
    {
        //PersonalDb personalDb = new PersonalDb();
    }

    private void DisplayPdfDocument(int docId)
    {
        int missingPages = 0;
        try
        {
            if (docId > 0)
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
                    Util.MergePdfFiles(pages, mergedFileName);
                else
                {
                    try
                    {
                        if (File.Exists(mergedFileName))
                        {
                            File.Delete(mergedFileName);
                        }
                    }
                    catch (Exception ex)
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

                    #endregion

                }
                else
                    throw new System.ArgumentException(Constants.ProblemLoadingPages);
            }
        }
        catch (Exception e)
        {
            if (e.Message.Contains(Constants.ProblemLoadingPages))
            {
                string msg = string.Format(Constants.ProblemLoadingPages, missingPages, missingPages == 1 ? string.Empty : "s", missingPages == 1 ? "is" : "are", missingPages == 1 ? "The" : "These", missingPages == 1 ? string.Empty : "s");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            }
            else // Added By Edward 27.10.2013
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                string errorMessage = e.Message + "<br><br>" + e.InnerException + "<br><br>" + e.StackTrace;
                errorLogDb.Insert("Completeness/View.aspx - DisplayPdfDocument", errorMessage);
            } 
        }
    }

    private void ResetPageClosingFlags()
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResetPageClosingFlags", "javascript:ResetFlags();", true);
    }

    protected void SetButtonClass(string activeButton)
    {
        foreach (RadToolBarButton radToolBarButton in RadToolBarMenuDoc.Items)
        {
            radToolBarButton.Font.Bold = false;
        }

        RadToolBarButton activeBarButton = RadToolBarMenuDoc.FindItemByValue(activeButton) as RadToolBarButton;

        if (activeBarButton != null)
            activeBarButton.Font.Bold = true;

        foreach (RadToolBarButton radToolBarButton in RadToolBarMenuRef.Items)
        {
            radToolBarButton.Font.Bold = false;
        }

        activeBarButton = RadToolBarMenuRef.FindItemByValue(activeButton) as RadToolBarButton;

        if (activeBarButton != null)
            activeBarButton.Font.Bold = true;
    }

    private void PopulateLogAction()
    {
        LogActionDb logActionDb = new LogActionDb();

        if (RadTreeView1.SelectedNode.Category.ToLower().Equals("ref"))
        {
            RadGridLog.Columns[3].Visible = true;
            RadGridLog.DataSource = logActionDb.GetLogActionByAppID(int.Parse(RadTreeView1.SelectedValue));
        }
        else if (RadTreeView1.SelectedNode.Category.ToLower().Equals("doc"))
        {
            RadGridLog.Columns[3].Visible = false;
            RadGridLog.DataSource = logActionDb.GetLogActionByDocIDForCompleteness(int.Parse(RadTreeView1.SelectedValue));
        }

        selectedNodeGlobal = RadTreeView1.SelectedValue;
    }

    private void PopulatePersonalMetaDataList(int docId)
    {
        //DocPersonalDb docPersonalDb = new DocPersonalDb();
        //DocPersonal.DocPersonalDataTable docPersonal = docPersonalDb.GetDocPersonalByDocId(docId);
        //PersonalMetaDataRepeater.DataSource = docPersonal;
        //PersonalMetaDataRepeater.DataBind();
    }

    private void PopulatePersonals(bool fromSummary)
    {
        if (docAppId.HasValue)
        {
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

            docApps = docAppDb.GetDocAppById(docAppId.Value);

            if (docApps.Rows.Count > 0)
            {
                if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    #region HLE 
                    DocApp.DocAppRow docAppRow = docApps[0];

                    HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                    HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                    //summaryApplicantCnt = 0;
                    //summaryOccupierCnt = 0;

                    if (!fromSummary)
                    {
                        CosOutstandingPersonalRepeater.DataSource = hleInterfaceDt;
                        CosOutstandingPersonalRepeater.DataBind();

                        CosNoOutstandingPanel.Visible = (hleInterfaceDt.Rows.Count <= 0);

                        OutstandingMultiView.ActiveViewIndex = 0;
                    }
                    else
                    {
                        HleSummaryRepeater.DataSource = hleInterfaceDt;
                        HleSummaryRepeater.DataBind();

                        CosNoSummaryPanel.Visible = (hleInterfaceDt.Rows.Count <= 0);

                        SummaryMultiView.ActiveViewIndex = 0;
                    }
                    #endregion
                }
                else if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.RESALE.ToString()))
                {
                    #region RESALE
                    DocApp.DocAppRow docAppRow = docApps[0];

                    ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                    ResaleInterface.ResaleInterfaceDataTable resaleInterfaceDt = resaleInterfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);

                    //summaryOccupierCnt = 0;
                    //summarySellerCnt = 0;
                    //summaryBuyerCnt = 0;
                    //summaryMiscCnt = 0;

                    if (!fromSummary)
                    {
                        ResaleOutstandingPersonalRepeater.DataSource = resaleInterfaceDt;
                        ResaleOutstandingPersonalRepeater.DataBind();

                        ResaleNoOutstandingPanel.Visible = (resaleInterfaceDt.Rows.Count <= 0);

                        OutstandingMultiView.ActiveViewIndex = 1;
                    }
                    else
                    {
                        ResaleRepeater.DataSource = resaleInterfaceDt;
                        ResaleRepeater.DataBind();

                        ResaleNoSummaryPanel.Visible = (resaleInterfaceDt.Rows.Count <= 0);

                        SummaryMultiView.ActiveViewIndex = 1;
                    }
                    #endregion
                }
                else if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.SALES.ToString()))
                {
                    #region SALES
                    DocApp.DocAppRow docAppRow = docApps[0];

                    SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                    SalesInterface.SalesInterfaceDataTable salesInterfaceDt = salesInterfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);

                    //summaryApplicantCnt = 0;
                    //summaryOccupierCnt = 0;

                    if (!fromSummary)
                    {
                        SocOutstandingPersonalRepeater.DataSource = salesInterfaceDt;
                        SocOutstandingPersonalRepeater.DataBind();

                        SocNoOutstandingPanel.Visible = (salesInterfaceDt.Rows.Count <= 0);

                        OutstandingMultiView.ActiveViewIndex = 2;
                    }
                    else
                    {
                        SalesRepeater.DataSource = salesInterfaceDt;
                        SalesRepeater.DataBind();

                        SocNoSummaryPanel.Visible = (salesInterfaceDt.Rows.Count <= 0);

                        SummaryMultiView.ActiveViewIndex = 2;
                    }
                    #endregion
                }
                else if (RadTreeView1.SelectedNode.Text.ToUpper().Contains(ReferenceTypeEnum.SERS.ToString()))
                {
                    #region SERS
                    DocApp.DocAppRow docAppRow = docApps[0];

                    SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                    SersInterface.SersInterfaceDataTable sersInterfaceDt = sersInterfaceDb.GetSersInterfaceByRefNo(docAppRow.RefNo);

                    //summaryApplicantCnt = 0;
                    //summaryOccupierCnt = 0;

                    if (!fromSummary)
                    {
                        SersOutstandingPersonalRepeater.DataSource = sersInterfaceDt;
                        SersOutstandingPersonalRepeater.DataBind();

                        SersNoOutstandingPanel.Visible = (sersInterfaceDt.Rows.Count <= 0);

                        OutstandingMultiView.ActiveViewIndex = 3;
                    }
                    else
                    {
                        SersRepeater.DataSource = sersInterfaceDt;
                        SersRepeater.DataBind();

                        if (sersInterfaceDt.Rows.Count > 0)
                        {
                            SersInterface.SersInterfaceRow sersInterface = sersInterfaceDt[0];

                            SersAllocSchLabel.Text = (String.IsNullOrEmpty(sersInterface.Alloc.Trim()) ? "-" : sersInterface.Alloc.Trim());
                            SersElligSchLabel.Text = (String.IsNullOrEmpty(sersInterface.Elig.Trim()) ? "-" : sersInterface.Elig.Trim());
                        }

                        SersNoSummaryPanel.Visible = (sersInterfaceDt.Rows.Count <= 0);

                        SummaryMultiView.ActiveViewIndex = 3;
                    }
                    #endregion
                }
            }
        }
    }

    private void PopulatePendingDocs(string refNo, string nric, RadGrid pendingDocsRadGrid, ReferenceTypeEnum refType)
    {
        if (refType != ReferenceTypeEnum.SALES)
        {
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            pendingDocsRadGrid.DataSource = leasInterfaceDb.GetLeasInterfaceByHleNumberAndNricForPendingDocumentDisplay(refNo, nric);
        }
        else
        {
            SocInterfaceDb socInterfaceDb = new SocInterfaceDb();
            pendingDocsRadGrid.DataSource = socInterfaceDb.DeleteByRegistrationNoAndNric(refNo, nric);
        }
        pendingDocsRadGrid.DataBind();
    }

    private void PopulateSummaryDocs(string hleNumber, string nric, RadGrid summaryDocsRadGrid, bool isHa, string referenceType)
    {
        DocDb docDb = new DocDb();
        summaryDocsRadGrid.DataSource = docDb.GetDocSummaryByNric(docAppId.Value, nric, isHa, referenceType);
        summaryDocsRadGrid.DataBind();
    }

    private void UpdateToolTip(string id, UpdatePanel panel)
    {
        Control ctrl = Page.LoadControl("~/Controls/SplitDocument.ascx");
        panel.ContentTemplateContainer.Controls.Add(ctrl);

        Controls_SplitDocument splitDocument = (Controls_SplitDocument)ctrl;
        splitDocument.DocumentId = int.Parse(RadTreeView1.SelectedValue);
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

    #region  Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
    //protected void SetRightDocumentLabelText(int docId)
    //{
    //    DocDb docDb = new DocDb();
    //    Doc.DocDataTable docs = docDb.GetDocById(docId);

    //    if (docs.Rows.Count > 0)
    //    {
    //        Doc.DocRow docRow = docs[0];
    //        RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (docRow.Status.ToString().ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
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
                    RightDocumentLabel.Text = RadTreeView1.SelectedNode.Text + " (" + (docRow.Status.ToString().ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()) ? Constants.DocumentVerified : Constants.DocumentNotVerified) + ")";
                }
            }
        }
        catch (Exception ex)
        {
            RightDocumentLabel.Text = "Error in RightDocumentLabel: " + ex.Message;            
        }
        
    }

    #endregion

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
    /// <summary>
    /// Added By Edward 04.11.2013 Confirm All Acceptance
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ConfirmAllAcceptanceButton_Click(object sender, EventArgs e)
    {
        if (docAppId.HasValue)
        {
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

            docApps = docAppDb.GetDocAppById(docAppId.Value);

            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];

                HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                if (hleInterfaceDt.Rows.Count > 0)
                {
                    foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                    {
                        DocDb docDb = new DocDb();
                        DataTable docDt = docDb.GetDocSummaryByNric(docAppId.Value, hleInterfaceRow.Nric, hleInterfaceRow.ApplicantType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.HLE.ToString());
                        foreach (DataRow row in docDt.Rows)
                        {
                            if (row["ImageCondition"].ToString().Trim().Equals("NA"))                            
                                //docDb.UpdateImageAccepted(int.Parse(row["id"].ToString()), "Y");    
                                SaveMetaData(int.Parse(row["id"].ToString()), row["ImageCondition"].ToString().Trim(), row["DocTypeCode"].ToString(), 
                                    row["Exception"] == null ? string.Empty : row["Exception"].ToString(), "Y", row["DocumentCondition"].ToString().Trim());
                            else if (row["ImageCondition"].ToString().Trim().Equals("Blur/Incomplete"))                            
                                //docDb.UpdateImageAccepted(int.Parse(row["id"].ToString()), "N");  
                                SaveMetaData(int.Parse(row["id"].ToString()), row["ImageCondition"].ToString().Trim(), row["DocTypeCode"].ToString(),
                                    row["Exception"] == null ? string.Empty : row["Exception"].ToString(), "N", row["DocumentCondition"].ToString().Trim());                                                   
                        }
                    }
                }
            }

            PopulatePersonals(true);
        }
    }

    #region Added By Edward 04.11.2013 Confirm All Acceptance
    private void SetAccessControl()
    {
        ConfirmAllAcceptanceButton.Visible = Util.HasAccessRights(ModuleNameEnum.Completeness, AccessControlSettingEnum.Confirm_All_Acceptance);
        ConfirmExtractButton.Visible = Util.HasAccessRights(ModuleNameEnum.Completeness, AccessControlSettingEnum.Confirm_and_Extract);
        if (ConfirmExtractButton.Visible)
            ConfirmAllAcceptanceButton.Visible = false;
    }

    protected void SaveMetaData(int docId, string imageCondition, string docTypeCode, string exception, string acceptance, string documentCondition )
    {
        DocDb docDb = new DocDb();
        docDb.UpdateDocFromMeta(docId, DocStatusEnum.Completed, imageCondition, docTypeCode, true, LogTypeEnum.C);
        docDb.UpdateDocDetails(docId, exception, acceptance, DocStatusEnum.Completed);
        docDb.UpdateSendToCdbAccept(docId, SendToCDBStatusEnum.Ready);

        //update document condition
        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), documentCondition, true);
        docDb.UpdateDocumentCondition(docId, documentConditionEnum);

        currentDocType = DocTypeDropDownList.SelectedValue.Trim();

        //uppdate app status to completeness_In_progress
        DocAppDb docAppDb = new DocAppDb();
        docAppDb.UpdateRefStatus(docAppId.Value, AppStatusEnum.Completeness_In_Progress, false, false, null);

        selectedNodeGlobal = RadTreeView1.SelectedValue;

        PopulateTreeView();
        ResetPageClosingFlags();
        //Modified by Edward to Reduce Error Notification - Object reference not set to an instance of an object at SetRightDocumentLabelText 2015/8/18
        //SetRightDocumentLabelText(docId);
        SetRightDocumentLabelText(docId.ToString());
    }
    #endregion

    #region Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
    protected void ConfirmExtractButton_Click(object sender, EventArgs e)
    {
        DocAppDb docAppDb = new DocAppDb();

        if (docAppId.HasValue)
        {            
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

            docApps = docAppDb.GetDocAppById(docAppId.Value);

            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];

                HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                if (hleInterfaceDt.Rows.Count > 0)
                {
                    foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                    {
                        DocDb docDb = new DocDb();
                        DataTable docDt = docDb.GetDocSummaryByNric(docAppId.Value, hleInterfaceRow.Nric, hleInterfaceRow.ApplicantType.Equals(PersonalTypeEnum.HA.ToString()), ScanningTransactionTypeEnum.HLE.ToString());
                        foreach (DataRow row in docDt.Rows)
                        {
                            if (row["ImageCondition"].ToString().Trim().Equals("NA"))
                                //docDb.UpdateImageAccepted(int.Parse(row["id"].ToString()), "Y");    
                                SaveMetaData(int.Parse(row["id"].ToString()), row["ImageCondition"].ToString().Trim(), row["DocTypeCode"].ToString(),
                                    row["Exception"] == null ? string.Empty : row["Exception"].ToString(), "Y", row["DocumentCondition"].ToString().Trim());
                            else if (row["ImageCondition"].ToString().Trim().Equals("Blur/Incomplete"))
                                //docDb.UpdateImageAccepted(int.Parse(row["id"].ToString()), "N");  
                                SaveMetaData(int.Parse(row["id"].ToString()), row["ImageCondition"].ToString().Trim(), row["DocTypeCode"].ToString(),
                                    row["Exception"] == null ? string.Empty : row["Exception"].ToString(), "N", row["DocumentCondition"].ToString().Trim());
                        }
                    }
                }
            }

            PopulatePersonals(true);

            if (ConfirmExtractApplication())
            {
                string strAddress = String.Format("window.open('../IncomeAssessment/AssignUser.aspx?id={0}', 'word')", docAppId.Value);
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Opening Income Extraction Summary Page", string.Format("javascript:ShowWindow('~/IncomeAssessment/AssignUser.aspx?id={0}',800,500);return false;", docAppId.Value), true);
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", String.Format("javascript:{0};", strAddress), true);
            }      
        }
         
    }

    protected bool ConfirmApplication()
    {
        bool IsConfirm = false;
        RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

        DocAppDb docAppDb = new DocAppDb();

        if (docAppDb.CanUserConfirmApplication(docAppId.Value))
        {
            //Updated by Edward 11.07.2013
            docAppDb.UpdateRefStatus(docAppId.Value, AppStatusEnum.Completeness_Checked, AssessmentStatusEnum.Completeness_Checked, true, false, LogActionEnum.Confirmed_application);
            //Added by Edward 15.07.2013
            IncomeDb.InsertMonthYearInIncomeTable(docAppId.Value, IncomeMonthsSourceEnum.C);

            docAppDb.UpdateSendToCdbStatus(docAppId.Value, SendToCDBStatusEnum.Ready);

            SetConfirmLabel.Text = Constants.RefVerified;

            //disable the action buttons.
            LeftConfirmButton.Visible = false;
            LeftOptions.Enabled = SubmitAndConfirmMDButton.Enabled = SubmitMetadataButton.Enabled = false;
            ExtractButton.Enabled = false;
            ConfirmAllAcceptanceButton.Visible = ConfirmExtractButton.Visible = false;        //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
            
            //disable the tree node drag/drop
            foreach (RadTreeNode node in RadTreeView1.GetAllNodes())
            {
                if (node.AllowDrag == true)
                    node.AllowDrag = false;

                if (node.AllowDrop == true)
                    node.AllowDrop = false;
            }
            IsConfirm = true;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Unable to Confirm Set", String.Format("javascript:alert('{0}');", Constants.UnableToConfirmApplication), true);
        }
        return IsConfirm;
    }
    #region Added By Edward 2014/9/18 Confirm and Extract
    protected bool ConfirmExtractApplication()
    {
        bool IsConfirm = false;
        RedirectIfApplicationConfirmedAndUserNotAllowedToUpdate();

        DocAppDb docAppDb = new DocAppDb();

        if (docAppDb.CanUserConfirmExtractApplication(docAppId.Value))
        {
            //Updated by Edward 11.07.2013
            docAppDb.UpdateRefStatus(docAppId.Value, AppStatusEnum.Completeness_Checked, AssessmentStatusEnum.Completeness_Checked, true, false, LogActionEnum.Confirmed_application);
            //Added by Edward 15.07.2013
            IncomeDb.InsertMonthYearInIncomeTable(docAppId.Value, IncomeMonthsSourceEnum.C);

            docAppDb.UpdateSendToCdbStatus(docAppId.Value, SendToCDBStatusEnum.Ready);

            SetConfirmLabel.Text = Constants.RefVerified;

            //disable the action buttons.
            LeftConfirmButton.Visible = false;
            LeftOptions.Enabled = SubmitAndConfirmMDButton.Enabled = SubmitMetadataButton.Enabled = false;
            ExtractButton.Enabled = false;
            ConfirmAllAcceptanceButton.Visible = ConfirmExtractButton.Visible = false;        //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting

            //disable the tree node drag/drop
            foreach (RadTreeNode node in RadTreeView1.GetAllNodes())
            {
                if (node.AllowDrag == true)
                    node.AllowDrag = false;

                if (node.AllowDrop == true)
                    node.AllowDrop = false;
            }
            IsConfirm = true;
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Unable to Confirm Set", String.Format("javascript:alert('{0}');", Constants.UnableToConfirmExtractApplication), true);
        }
        return IsConfirm;
    }
    #endregion
    #endregion

}
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using System.IO;
using Dwms.Web;

public partial class Import_SearchSetsReadOnly_Default : System.Web.UI.Page
{
    int sectionId = -1;
    int departmentId = -1;

    #region Event Handlers
    /// <summary>
    /// Page Load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //get user departmentid
        MembershipUser user = Membership.GetUser();
        if (user == null)
            Response.Redirect("/Search/Default.aspx");

        DepartmentDb departmentDb = new DepartmentDb();
        departmentId = departmentDb.GetDepartmentIdByUserId((Guid)user.ProviderUserKey);

        if (!IsPostBack)
        {
            PopulateChannels();
            PopulateStatus();
            PopulateUsers();
        }
    }

    /// <summary>
    /// Rad Grid 1 need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            int id = int.Parse(DataBinder.Eval(e.Item.DataItem, "Id").ToString());
            HyperLink ViewSetHyperLink = e.Item.FindControl("ViewSetHyperLink") as HyperLink;
            Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
            Image UrgentImage = e.Item.FindControl("UrgentImage") as Image;
            Label WaitingTimeLabel = e.Item.FindControl("WaitingTimeLabel") as Label;
            Label ProcessingTimeLabel = e.Item.FindControl("ProcessingTimeLabel") as Label;
            ImageButton DocumentImageButton = e.Item.FindControl("DocumentImageButton") as ImageButton;

            DocSetDb docSetDb = new DocSetDb();
            string setStatus = docSetDb.GetSetStatus(id);
            SetStatusEnum setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), setStatus, true);

            //disable the set link if the categorization is in process
            if (docSetDb.CheckIfBeingProcessed(id))
            {
                ViewSetHyperLink.Enabled = false;
            }
            else if (setStatusEnum == SetStatusEnum.Categorization_Failed)
            {
                ViewSetHyperLink.Enabled = false;
            }

            // Compute the Waiting time
            DateTime processingEndDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingEndDate").ToString());
            DateTime verificationDateIn = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "VerificationDateIn").ToString());
            Util.DisplayWaitingTime(WaitingTimeLabel, processingEndDate, verificationDateIn, setStatusEnum);

            //compute Processing time
            DateTime processingStartDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingStartDate").ToString());
            Util.DisplayProcessingTime(ProcessingTimeLabel, processingStartDate, processingEndDate, setStatusEnum);

            //set Urgency indicator
            Util.DisplayUrgencyIndicator(setStatusEnum, UrgentImage, id, DataBinder.Eval(e.Item.DataItem, "IsUrgent").ToString().Equals("True"),
                DataBinder.Eval(e.Item.DataItem, "SkipCategorization").ToString().Equals("True"), null);

            //Applicattion Information
            //Util.DisplayApplicationInformationForSearch(AppInformationLabel, id);
            SetAppDb setAppDb = new SetAppDb();
            SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(id);

            if (setApps.Rows.Count > 0)
            {
                foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                {
                    Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                    Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    Util.DisplayHasPendingDocIndicatorSearch(NoPendingDoc, setAppRow.RefNo.ToString());
                }
            }
            else
                AppInformationLabel.Text = "-";

            char[] charsBreak = { '<', 'b', 'r', '>' };
            AppInformationLabel.Text = AppInformationLabel.Text.TrimEnd(charsBreak);


            //Display Document Information
            Util.DisplayDocumentInformation(DocumentImageButton, id);

            //Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
            //Util.DisplayHasPendingDocIndicatorSearch(NoPendingDoc, AppInformationLabel.Text.ToString());
        }
    }



    protected void RadGrid1OLD_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            int id = int.Parse(DataBinder.Eval(e.Item.DataItem, "Id").ToString());
            HyperLink ViewSetHyperLink = e.Item.FindControl("ViewSetHyperLink") as HyperLink;
            Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
            Image UrgentImage = e.Item.FindControl("UrgentImage") as Image;
            Label WaitingTimeLabel = e.Item.FindControl("WaitingTimeLabel") as Label;
            Label ProcessingTimeLabel = e.Item.FindControl("ProcessingTimeLabel") as Label;
            ImageButton DocumentImageButton = e.Item.FindControl("DocumentImageButton") as ImageButton;

            DocSetDb docSetDb = new DocSetDb();
            string setStatus = docSetDb.GetSetStatus(id);
            SetStatusEnum setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), setStatus, true);

            //disable the set link if the categorization is in process
            if (docSetDb.CheckIfBeingProcessed(id))
            {
                ViewSetHyperLink.Enabled = false;
            }
            else if (setStatusEnum == SetStatusEnum.Categorization_Failed)
            {
                ViewSetHyperLink.Enabled = false;
            }

            // Compute the Waiting time
            DateTime processingEndDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingEndDate").ToString());
            DateTime verificationDateIn = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "VerificationDateIn").ToString());
            Util.DisplayWaitingTime(WaitingTimeLabel, processingEndDate, verificationDateIn, setStatusEnum);

            //compute Processing time
            DateTime processingStartDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingStartDate").ToString());
            Util.DisplayProcessingTime(ProcessingTimeLabel, processingStartDate, processingEndDate, setStatusEnum);

            //set Urgency indicator
            Util.DisplayUrgencyIndicator(setStatusEnum, UrgentImage, id, DataBinder.Eval(e.Item.DataItem, "IsUrgent").ToString().Equals("True"),
                DataBinder.Eval(e.Item.DataItem, "SkipCategorization").ToString().Equals("True"), null);

            //Applicattion Information
            //Util.DisplayApplicationInformationForSearch(AppInformationLabel, id);
            SetAppDb setAppDb = new SetAppDb();
            SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(id);

            if (setApps.Rows.Count > 0)
            {
                foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                {
                    Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                    Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    Util.DisplayHasPendingDocIndicatorSearch(NoPendingDoc, setAppRow.RefNo.ToString());
                }
            }
            else
                AppInformationLabel.Text = "-";

            char[] charsBreak = { '<', 'b', 'r', '>' };
            AppInformationLabel.Text = AppInformationLabel.Text.TrimEnd(charsBreak);


            //Display Document Information
            Util.DisplayDocumentInformation(DocumentImageButton, id);

            //Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
            //Util.DisplayHasPendingDocIndicatorSearch(NoPendingDoc, AppInformationLabel.Text.ToString());
        }
    }

    /// <summary>
    /// Search button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        RadGrid1.Rebind();
    }

    /// <summary>
    /// Item command event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    //{
    //    DocSetDb docSetDb = new DocSetDb();

    //    if (e.CommandName.Equals("Delete"))
    //    {
    //        int id = int.Parse(e.CommandArgument.ToString());

    //        // Delete the set doc from the physical folder
    //        try
    //        {
    //            string mainDirPath = Util.GetDocForOcrFolder();

    //            DirectoryInfo mainDir = new DirectoryInfo(mainDirPath);

    //            DirectoryInfo[] subDirs = mainDir.GetDirectories();

    //            //foreach (DirectoryInfo subDir in subDirs)
    //            //{
    //            //DirectoryInfo[] docSetDirs = subDir.GetDirectories();

    //            //foreach (DirectoryInfo docSetDir in docSetDirs)
    //            foreach (DirectoryInfo docSetDir in subDirs)
    //            {
    //                if (docSetDir.Name.Equals(id.ToString()))
    //                    docSetDir.Delete(true);
    //            }
    //            //}
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //        finally
    //        {
    //            docSetDb.Delete(id);
    //        }
    //    }
    //    else if (e.CommandName.Equals("Recategorize"))
    //    {
    //        int id = int.Parse(e.CommandArgument.ToString());

    //        // Set the status to 'Pending Categorization' to recategorize the set
    //        docSetDb.UpdateSetStatus(id, SetStatusEnum.Pending_Categorization, true, false, LogActionEnum.REPLACE1_Recatogorized_the_set);

    //        RadGrid1.Rebind();
    //    }
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
        DocSetDb docSetDb = new DocSetDb();

        if (e.CommandName.Equals("Delete"))
        {
            int id = int.Parse(e.CommandArgument.ToString());
           
            // Delete the set doc from the physical folder
            DateTime datePath = new DateTime();
            if (Util.GetVerificationDateForOcrFolder(id, out datePath))
            {
                try
                {
                    string mainDirPath = Util.GetDocForOcrFolder(id, datePath);
                    DirectoryInfo mainDir = new DirectoryInfo(mainDirPath);

                    if (mainDir.Exists)
                    {
                        DirectoryInfo[] subDirs = mainDir.GetDirectories();


                        foreach (DirectoryInfo docSetDir in subDirs)
                        {
                            if (docSetDir.Name.Equals(id.ToString()))
                                docSetDir.Delete(true);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    docSetDb.Delete(id);
                }
            }
        }
        else if (e.CommandName.Equals("Recategorize"))
        {
            int id = int.Parse(e.CommandArgument.ToString());

            // Set the status to 'Pending Categorization' to recategorize the set
            docSetDb.UpdateSetStatus(id, SetStatusEnum.Pending_Categorization, true, false, LogActionEnum.REPLACE1_Recatogorized_the_set);

            RadGrid1.Rebind();
        }
    }
    #endregion

    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Search functions
    /// </summary>

    /// <summary>
    /// Ppopulat the status drop down list
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.EnumToDataTable(typeof(SetStatusEnum));
        StatusRadComboBox.DataTextField = "Text";
        StatusRadComboBox.DataValueField = "Value";
        StatusRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the channels
    /// </summary>
    private void PopulateChannels()
    {
        //ScanChannelDb scanChannelDb = new ScanChannelDb();
        //ChannelRadComboBox.DataSource = scanChannelDb.GetAllChannels();
        //ChannelRadComboBox.DataTextField = "Name";
        //ChannelRadComboBox.DataValueField = "Name";
        //ChannelRadComboBox.DataBind();

        DocSetDb docSetDb = new DocSetDb();
        ChannelRadComboBox.DataSource = docSetDb.GetChannelsForDropDown();
        ChannelRadComboBox.DataTextField = "Channel";
        ChannelRadComboBox.DataValueField = "Channel";
        ChannelRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate users drop down list
    /// </summary>
    private void PopulateUsers()
    {
        VerificationOICRadComboBox.Items.Clear();
        VerificationOICRadComboBox.DataSourceID = "GetUserByDepartmentObjectDataSource";
        VerificationOICRadComboBox.DataTextField = "Name";
        VerificationOICRadComboBox.DataValueField = "UserId";


        UserRadComboBox.Items.Clear();
        UserRadComboBox.DataSourceID = "GetUserByDepartmentObjectDataSource";
        UserRadComboBox.DataTextField = "Name";
        UserRadComboBox.DataValueField = "UserId";
    }

    protected void GetUserByDepartmentObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["department"] = departmentId;
    }

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        ChannelRadComboBox.SelectedIndex = 0;
        StatusRadComboBox.SelectedIndex = 0;
        //RefNoRadComboBox.Text = string.Empty;
        //RefNoRadComboBox.SelectedValue = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        IsUrgentCheckBox.Checked = false;
        SkipCategorizationCheckBox.Checked = false;  

        UserRadComboBox.SelectedIndex = 0;
        UserRadComboBox.SelectedValue = string.Empty;
        UserRadComboBox.Text = string.Empty;

        VerificationOICRadComboBox.SelectedIndex = 0;
        VerificationOICRadComboBox.SelectedValue = string.Empty;
        VerificationOICRadComboBox.Text = string.Empty;
        AcknowledgeNumberRadComboBox.Text = string.Empty;
        AcknowledgeNumberRadComboBox.SelectedValue = string.Empty;
        CmDocumentIdComboBox.Text = string.Empty;
        CmDocumentIdComboBox.SelectedValue = string.Empty;

        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;

        PopulateUsers();
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;

        PopulateList();
        RebindGrids();
    }

    private void RebindGrids()
    {
        RadGrid1.DataBind();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        //get nric
        string nric = string.Empty;
        //if (!string.IsNullOrEmpty(NricRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(NricRadComboBox.SelectedValue))
        //    {
        //        nric = NricRadComboBox.SelectedValue;
        //    }
        //    else
        //        nric = NricRadComboBox.Text.Trim();
        //}
        if (!string.IsNullOrEmpty(NricRadTextBox.Text.Trim()))
        {
            nric = NricRadTextBox.Text.Trim();
        }
        //get AcknowledgeNumber
        string acknowledgeNumber = string.Empty;
        if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.SelectedValue))
            {
                acknowledgeNumber = AcknowledgeNumberRadComboBox.SelectedValue;
            }
            else
                acknowledgeNumber = AcknowledgeNumberRadComboBox.Text.Trim();
        }

        //get CmDocumentId
        string cmDocumentId = string.Empty;
        if (!string.IsNullOrEmpty(CmDocumentIdComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(CmDocumentIdComboBox.SelectedValue))
            {
                cmDocumentId = CmDocumentIdComboBox.SelectedValue;
            }
            else
                cmDocumentId = CmDocumentIdComboBox.Text.Trim();
        }

        string channel = ChannelRadComboBox.SelectedValue;
        //string referenceNumber = RefNoTextBox.Text.Trim();

        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()) && !string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);

        //if (!string.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue))
        //    {
        //        docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
        //    }
        //    else
        //    {
        //        RadComboBoxItem selectedItem = RefNoRadComboBox.FindItemByText(RefNoRadComboBox.Text.Trim());

        //        if (selectedItem != null)
        //        {
        //            docAppId = int.Parse(selectedItem.Value);
        //        }
        //        else
        //        {
        //            #region Commented by Edward 08.11.2013 Optimize using Search Refno
        //            //DocAppDb docAppDb = new DocAppDb();
        //            //DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

        //            //if (docAppTable.Rows.Count > 0)
        //            //{
        //            //    DocApp.DocAppRow docAppRow = docAppTable[0];
        //            //    docAppId = docAppRow.Id;
        //            //}
        //            #endregion
        //            //Added By Edward 08.11.2013 Optimizie using Search Refno
        //            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(RefNoRadComboBox.Text.Trim());
        //        }
        //    }
        //}
        if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {
            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(DocAppRadTextBox.Text.Trim());
        }
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        string importedByOICUserIdStr = UserRadComboBox.SelectedValue;
        Guid? importedByOICUserId = null;

        if (!importedByOICUserIdStr.Equals("-1") && !string.IsNullOrEmpty(importedByOICUserIdStr.Trim()))
        {
            importedByOICUserId = new Guid(importedByOICUserIdStr);
        }

        string verificationOICUserIdStr = VerificationOICRadComboBox.SelectedValue;
        Guid? verificationOICUserId = null;

        if (!verificationOICUserIdStr.Equals("-1") && !string.IsNullOrEmpty(verificationOICUserIdStr.Trim()))
        {
            verificationOICUserId = new Guid(verificationOICUserIdStr);
        }

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        DocSetDb docsetDb = new DocSetDb();

        SetStatusEnum setStatusEnum = new SetStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), StatusRadComboBox.SelectedValue, true);
            RadGrid1.DataSource = docsetDb.GetSets(channel, setStatusEnum, string.Empty, departmentId, sectionId, importedByOICUserId, verificationOICUserId, dateInFrom, dateInTo, docAppId, nric, IsUrgentCheckBox.Checked, SkipCategorizationCheckBox.Checked, cmDocumentId, acknowledgeNumber);
        }
        else
            RadGrid1.DataSource = docsetDb.GetSets(channel, null, string.Empty, departmentId, sectionId, importedByOICUserId, verificationOICUserId, dateInFrom, dateInTo, docAppId, nric, IsUrgentCheckBox.Checked, SkipCategorizationCheckBox.Checked, cmDocumentId, acknowledgeNumber);
    }
    #endregion
}

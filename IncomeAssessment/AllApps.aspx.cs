using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class IncomeAssessment_AppList : System.Web.UI.Page
{
    //string user;          Commented by Edward 2016/02/04 take out unused variables

    int sectionId = -1;

    bool isAllAppsReadOnlyUser = false;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //show address if the user belong to SERS department
        UserDb userDb = new UserDb();
        sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);

        if (!IsPostBack)
        {
            PopulateUsers();
            PopulateStatus();

            NoGroupRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
    
            PopulateList();
            RebindGrids();
            GridMultiView.ActiveViewIndex = 0;
        }

        AccessControlDb accessControlDb = new AccessControlDb();
        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness);
        isAllAppsReadOnlyUser = (accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()));
        

    }

    /// <summary>
    /// Rad grid need data source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void NoGroupRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void NoGroupRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        //Added By Edward 17/3/2014 Sales and Resale Changes
        if (sectionId == 5 || sectionId == 6 || (sectionId == 3 || sectionId == 4))
        {
            NoGroupRadGrid.MasterTableView.Columns[6].HeaderText = "OIC 1";
            NoGroupRadGrid.MasterTableView.Columns[7].HeaderText = "OIC 2";
        }

        
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            Label AddressLabel = e.Item.FindControl("AddressLabel") as Label;
            LinkButton SetCountLinkButton = e.Item.FindControl("SetCountLinkButton") as LinkButton;
            DataRowView data = (DataRowView)e.Item.DataItem;

            // Compute the age value
            DateTime dateInDateTime;

            if (Validation.IsDate(data["AssessmentDateIn"].ToString()))
            {               
                #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                // Compute the age value
                //dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                //AgingLabel.Text = Format.GetAging(diff);
                dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["AssessmentDateOut"].ToString()) ? DateTime.Parse(data["AssessmentDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                #endregion
            }
            else
            {
                AgingLabel.Text = "? Day(s)";
            }

            string status = data["Status"].ToString().Trim(); // Get the status

            //if (status.Equals(SetStatusEnum.Verified.ToString()))
            //{
            //    // Source: http://www.telerik.com/community/code-library/aspnet-ajax/grid/gridclientselectcolumn-select-all-rows-with-enabled-check-boxes-only.aspx
            //    CheckBox checkBox = (CheckBox)dataBoundItem["ChildCheckBoxColumn"].Controls[0]; 
            //    checkBox.Visible = false; 
            //}

            //handle viewonly/verify users
            if (isAllAppsReadOnlyUser)
            {
                NoGroupRadGrid.Columns[1].Visible = false;
                RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
            }
            else
            {
                NoGroupRadGrid.Columns[1].Visible = true;
                RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
            }

            //show address only for SERS sections (3,4)
            if (sectionId == 3 || sectionId == 4)
            {
                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                AddressLabel.Text = sersInterfaceDb.GetSersAddressByRefNo(data["RefNo"].ToString());
            }

            //Set Information
            int docAppId = int.Parse(data["Id"].ToString());
            Label SetInformationLabel = e.Item.FindControl("SetInformationLabel") as Label;

            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId);

            int setCount = docSets.Rows.Count;
            Util.DisplaySetInformation(setCount, SetCountLinkButton, docAppId);

            //show ! and ? indicators
            if (!status.Equals(AppStatusEnum.Completeness_Cancelled.ToString()))
            {
                // check if the docapp have all sets verified.
                //set the NotVerifiedFiles Indicator
                Image SetsVerifiedIndicatorImage = e.Item.FindControl("SetsVerifiedIndicatorImage") as Image;
                Util.DisplaySetsNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId);

                //set the NotVerifiedFiles Indicator
                // check if the docapp is checked and have files which are not verified.
                Image FilesIndicatorImage = e.Item.FindControl("FilesIndicatorImage") as Image;
                Util.DisplayFilesNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId, status, setCount);
            }
                       
            #region Added by Edward Changes for Sales and Resale 2014/5/26
            string sentToStatus = string.Empty;
            if (sectionId == 1)
                sentToStatus = data["SentToLEASStatus"].ToString().ToUpper();
            else if (sectionId == 5)
                sentToStatus = data["SentToResaleStatus"].ToString().ToUpper();
            #endregion

            #region Added By Edward 24/02/2014  Add Icon and Action Log
            Image WorksheetImage = e.Item.FindControl("WorksheetImage") as Image;
            if (sentToStatus == SendToLEASStatusEnum.Ready.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application is ready for sending. ";
                WorksheetImage.ImageUrl = "~/Data/Images/ReadyWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.Sent.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has been sent. ";
                WorksheetImage.ImageUrl = "~/Data/Images/SentWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.FailedConnErr.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has encountered a connection error. ";
                WorksheetImage.ImageUrl = "~/Data/Images/FailedWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.FailedInpErr.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has encountered an input error. ";
                WorksheetImage.ImageUrl = "~/Data/Images/FailedWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.FailedOutErr.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has encountered an output error. ";
                WorksheetImage.ImageUrl = "~/Data/Images/FailedWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.FailedUnknown.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has not been sent. ";
                WorksheetImage.ImageUrl = "~/Data/Images/FailedWorksheet.png";
            }
            else if (sentToStatus == SendToLEASStatusEnum.SentButFailed.ToString().ToUpper())
            {
                WorksheetImage.Visible = true;
                WorksheetImage.ToolTip = "This application has been sent but failed. ";
                WorksheetImage.ImageUrl = "~/Data/Images/FailedWorksheet.png";
            }
            #endregion
        }
    }

    /// <summary>
    /// Search button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        PopulateList();
        RebindGrids();
    }

    /// <summary>
    /// Reset button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ResetButton_Click(object sender, EventArgs e)
    {
        StatusRadComboBox.SelectedIndex = 0;
        UserRadComboBox.Text = string.Empty;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        //AcknowledgeNumberRadComboBox.Text = string.Empty;
        //AcknowledgeNumberRadComboBox.SelectedValue = string.Empty;
        AcknowledgeNumberRadTextBox.Text = string.Empty;

        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;
        GroupByDateCheckBox.Checked = false;
        //GroupRadComboBox.SelectedIndex = 0;


        NoGroupRadGrid.Rebind();
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        NoGroupRadGrid.Rebind();
    }

    /// <summary>
    /// on ObjectDataSource Selecting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GetUserObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        UserDb userDb = new UserDb();
        e.InputParameters["section"] = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);
    }

    #endregion

    #region group by date

    protected void GroupByDateRadGrid_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
    {
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;

        e.DetailTableView.DataSource = GetDataForDetailTable((DateTime)dataItem.GetDataKeyValue("AssessmentDateIn"));
    }

    protected void GroupByDateRadGrid_PreRender(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();
        int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);

        foreach (GridDataItem item in GroupByDateRadGrid.MasterTableView.Items)
        {
            if (item.HasChildItems)
            {
                foreach (GridTableView innerDetailView in item.ChildItem.NestedTableViews)
                {
                    GridColumn unitOfMeasure = innerDetailView.GetColumn("Address");
                    unitOfMeasure.Visible = (sectionId == 3 || sectionId == 4); ;
                }
            }
        }
    }

    protected void GroupByDateRadGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (!e.IsFromDetailTable)
            PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GroupByDateRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {

        //Added By Edward 17/3/2014 Sales and Resale Changes
        if (sectionId == 5 || sectionId == 6)
        {
            GroupByDateRadGrid.MasterTableView.Columns[6].HeaderText = "OIC 1";
            GroupByDateRadGrid.MasterTableView.Columns[7].HeaderText = "OIC 2";
        }

        if (e.Item is GridDataItem)
        {
            if (e.Item.OwnerTableView.Name.Equals("GroupByDateDetailTable"))
            {
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                Label AddressLabel = e.Item.FindControl("AddressLabel") as Label;
                LinkButton SetCountLinkButton = e.Item.FindControl("SetCountLinkButton") as LinkButton;
              
                DataRowView data = (DataRowView)e.Item.DataItem;

                // Compute the age value
                DateTime dateInDateTime;

                if (Validation.IsDate(data["AssessmentDateIn"].ToString()))
                {
                    #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                    // Compute the age value
                    //dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                    //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                    //AgingLabel.Text = Format.GetAging(diff);
                    dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                    DateTime dateOutDateTime = !string.IsNullOrEmpty(data["AssessmentDateOut"].ToString()) ? DateTime.Parse(data["AssessmentDateOut"].ToString()) : DateTime.Now;
                    AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                    #endregion
                }
                else
                {
                    AgingLabel.Text = "? Day(s)";
                }

                string status = data["Status"].ToString().Trim(); // Get the status

                //if (status.Equals(SetStatusEnum.Verified.ToString()))
                //{
                //    // Source: http://www.telerik.com/community/code-library/aspnet-ajax/grid/gridclientselectcolumn-select-all-rows-with-enabled-check-boxes-only.aspx
                //    CheckBox checkBox = (CheckBox)dataBoundItem["ChildCheckBoxColumn"].Controls[0]; 
                //    checkBox.Visible = false; 
                //}

                //handle viewonly/verify users
                if (isAllAppsReadOnlyUser)
                {
                    NoGroupRadGrid.Columns[1].Visible = false;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
                }
                else
                {
                    NoGroupRadGrid.Columns[1].Visible = true;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
                }

                //show address only for SERS sections (3,4)
                if (sectionId == 3 || sectionId == 4)
                {
                    SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                    AddressLabel.Text = sersInterfaceDb.GetSersAddressByRefNo(data["RefNo"].ToString());
                }

                //Set Information
                int docAppId = int.Parse(data["Id"].ToString());
                Label SetInformationLabel = e.Item.FindControl("SetInformationLabel") as Label;

                DocSetDb docSetDb = new DocSetDb();
                DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId);

                int setCount = docSets.Rows.Count;
                Util.DisplaySetInformation(setCount, SetCountLinkButton, docAppId);

                //show ! and ? indicators
                if (!status.Equals(AppStatusEnum.Completeness_Cancelled.ToString()))
                {
                    // check if the docapp have all sets verified.
                    //set the NotVerifiedFiles Indicator
                    Image SetsVerifiedIndicatorImage = e.Item.FindControl("SetsVerifiedIndicatorImage") as Image;
                    Util.DisplaySetsNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId);

                    //set the NotVerifiedFiles Indicator
                    // check if the docapp is checked and have files which are not verified.
                    Image FilesIndicatorImage = e.Item.FindControl("FilesIndicatorImage") as Image;
                    Util.DisplayFilesNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId, status, setCount);
                }
            }
        }
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Populate users drop down list
    /// </summary>
    private void PopulateUsers()
    {
        UserRadComboBox.DataSourceID = "GetUserObjectDataSource";
        UserRadComboBox.DataTextField = "Name";
        UserRadComboBox.DataValueField = "UserId";
    }

    /// <summary>
    /// Populate the status
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.GetAssessmentStatus();
        StatusRadComboBox.DataTextField = "Text";
        StatusRadComboBox.DataValueField = "Value";
        StatusRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        //by Andrew (17/1/2013)
        //int groupCriteria = int.Parse(GroupRadComboBox.SelectedValue);
        int groupCriteria = GroupByDateCheckBox.Checked ? 1 : 0;

        if (groupCriteria == 0)
            PopulateDataWithoutGroup();
        else
            PopulateDataWithGroup();

        GridMultiView.ActiveViewIndex = groupCriteria;
    }

    /// <summary>
    /// Get Data
    /// </summary>
    private DataTable GetData(bool withGroup)
    {
        DocAppDb docAppDb = new DocAppDb();

        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
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
        //            //DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

        //            //if (docAppTable.Rows.Count > 0)
        //            //{
        //            //    DocApp.DocAppRow docAppRow = docAppTable[0];
        //            //    docAppId = docAppRow.Id;
        //            //}
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

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        //get completeness OIC
        Guid? userId = null;
        if (!string.IsNullOrEmpty(UserRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(UserRadComboBox.SelectedValue))
                userId = new Guid(UserRadComboBox.SelectedValue);
        }

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
        //if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.SelectedValue))
        //    {
        //        acknowledgeNumber = AcknowledgeNumberRadComboBox.SelectedValue;
        //    }
        //    else
        //        acknowledgeNumber = AcknowledgeNumberRadComboBox.Text.Trim();
        //}
        if (!string.IsNullOrEmpty(AcknowledgeNumberRadTextBox.Text.Trim()))
        {
            acknowledgeNumber = AcknowledgeNumberRadTextBox.Text.Trim();
        }

        // Get the Sets for the user's section

        AssessmentStatusEnum assessmentStatusEnum = new AssessmentStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        DataTable sets;

        if (!status.Equals("-1"))
        {
            assessmentStatusEnum = (AssessmentStatusEnum)Enum.Parse(typeof(AssessmentStatusEnum), StatusRadComboBox.SelectedValue, true);

            if (withGroup)
                sets = docAppDb.GetAllAppsVerificationDatesIA(assessmentStatusEnum, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, nric, acknowledgeNumber);
            else
                sets = docAppDb.GetAllAppsIA(assessmentStatusEnum, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, false, string.Empty, nric, acknowledgeNumber);
        }
        else
        {
            if (withGroup)
                sets = docAppDb.GetAllAppsVerificationDatesIA(null, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, nric, acknowledgeNumber);
            else
                sets = docAppDb.GetAllAppsIA(null, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, false, string.Empty, nric, acknowledgeNumber);
        }

        if (sets.Rows.Count == 0)
        {
            if (isAllAppsReadOnlyUser)
                RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
            else
                RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
        }

        return sets;
    }

    private DataTable GetDataForDetailTable(DateTime verificationDateIn)
    {
        DocAppDb docAppDb = new DocAppDb();

        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
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
        //            DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

        //            if (docAppTable.Rows.Count > 0)
        //            {
        //                DocApp.DocAppRow docAppRow = docAppTable[0];
        //                docAppId = docAppRow.Id;
        //            }
        //        }
        //    }
        //}

        if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {
            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(DocAppRadTextBox.Text.Trim());
        }

        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        //get completeness OIC
        Guid? userId = null;
        if (!string.IsNullOrEmpty(UserRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(UserRadComboBox.SelectedValue))
                userId = new Guid(UserRadComboBox.SelectedValue);
        }

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
        //if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.SelectedValue))
        //    {
        //        acknowledgeNumber = AcknowledgeNumberRadComboBox.SelectedValue;
        //    }
        //    else
        //        acknowledgeNumber = AcknowledgeNumberRadComboBox.Text.Trim();
        //}
        if (!string.IsNullOrEmpty(AcknowledgeNumberRadTextBox.Text.Trim()))
        {
            acknowledgeNumber = AcknowledgeNumberRadTextBox.Text.Trim();
        }
        // Get the Sets for the user's section

        AssessmentStatusEnum assessmentStatusEnum = new AssessmentStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            assessmentStatusEnum = (AssessmentStatusEnum)Enum.Parse(typeof(AssessmentStatusEnum), StatusRadComboBox.SelectedValue, true);
            return docAppDb.GetAllAppsByVerificationDateInIA(verificationDateIn, assessmentStatusEnum, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, nric, acknowledgeNumber);
        }
        else
            return docAppDb.GetAllAppsByVerificationDateInIA(verificationDateIn, null, userId, dateInFrom, dateInTo, sectionId, docAppId, string.Empty, nric, acknowledgeNumber);
    }

    /// <summary>
    /// Populate without group grid
    /// </summary>
    private void PopulateDataWithoutGroup()
    {
        NoGroupRadGrid.DataSource = GetData(false);
    }

    /// <summary>
    /// Populate with group grid
    /// </summary>
    private void PopulateDataWithGroup()
    {
        GroupByDateRadGrid.DataSource = GetData(true);
        //GroupByDateRadGrid.DataBind();
    }

    private void RebindGrids()
    {
        //by Andrew (17/1/2013)
        //int groupCriteria = int.Parse(GroupRadComboBox.SelectedValue);
        int groupCriteria = GroupByDateCheckBox.Checked ? 1 : 0;

        if (groupCriteria == 0)
            NoGroupRadGrid.DataBind();
        else
            GroupByDateRadGrid.DataBind();
    }

    #endregion
}

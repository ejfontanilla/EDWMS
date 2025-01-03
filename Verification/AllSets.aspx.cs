﻿using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Verification_SetList : System.Web.UI.Page
{
    bool isAllSetsReadOnlyUser = false;    
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();
        int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);        

        if (!IsPostBack)
        {
            PopulateChannels();
            PopulateUsers();
            PopulateStatus();

            //show address if the user belong to SERS department                        
            NoGroupRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
            //GroupByDateRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);

            PopulateList();
            RebindGrids();
            GridMultiView.ActiveViewIndex = 0;
        }

        AccessControlDb accessControlDb = new AccessControlDb();
        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Verification);
        isAllSetsReadOnlyUser = (accessControlList.Contains(AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()));
    }

    #region No Group
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
        //Added by Edward 2015/8/24 use Local Variables instead of Global Variable - Reduce Error Notification
        UserDb userDb = new UserDb();
        int sectionIdOfUser = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
        //Added By Edward 17/3/2014 Sales and Resale Changes        
        if (sectionIdOfUser == 5 || sectionIdOfUser == 6 || sectionIdOfUser == 3)
        {
            NoGroupRadGrid.MasterTableView.Columns[4].HeaderText = "Ref No (OIC1/OIC2)";
        }

        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            Label AddressLabel = new Label();
            Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;

            DataRowView data = e.Item.DataItem as DataRowView;

            // Compute the age value

            #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
            //DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
            //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
            //AgingLabel.Text = Format.GetAging(diff);
            DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
            DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
            AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
            #endregion

            string status = data["Status"].ToString(); // Get the status

            //if (status.Equals(SetStatusEnum.Verified.ToString()))
            //{
            //    // Source: http://www.telerik.com/community/code-library/aspnet-ajax/grid/gridclientselectcolumn-select-all-rows-with-enabled-check-boxes-only.aspx
            //    CheckBox checkBox = (CheckBox)dataBoundItem["ChildCheckBoxColumn"].Controls[0]; 
            //    checkBox.Visible = false; 
            //}

            //handle viewonly/verify users

            if (isAllSetsReadOnlyUser)
            {
                NoGroupRadGrid.Columns[1].Visible = false;
                RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
            }
            else
            {
                NoGroupRadGrid.Columns[1].Visible = true;
                RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
            }

            //Import OIC
            //commented by Andrew (17/1/2013) as already added into vDocSet and Dal/DocSetDs.cs
            /*Label ImportedOICLabel = e.Item.FindControl("ImportedOICLabel") as Label;
            UserDb userDb = new UserDb();
            User.UserDataTable users = userDb.GetUser(new Guid(data["ImportedBy"].ToString()));
            User.UserRow userRow = users[0];
            ImportedOICLabel.Text = userRow.Name;*/

            //Applicattion Information
            int docSetId = int.Parse(data["Id"].ToString());
            int sectionId = int.Parse(data["SectionId"].ToString()); // Get the sectionId

            SetAppDb setAppDb = new SetAppDb();
            SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(docSetId);

            if (setApps.Rows.Count > 0)
            {
                foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                {
                    Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                    //show address only for SERS sections (3,4)
                    if (sectionId == 3 || sectionId == 4)
                    {
                        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                        AddressLabel.Text += sersInterfaceDb.GetSersAddressByRefNo(setAppRow.RefNo) + "<br>";
                    }

                    //Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    //Util.DisplayHasPendingDocIndicator(NoPendingDoc, setAppRow.RefNo.ToString());

                    Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    if (Util.DisplayHasPendingDocIndicator(setAppRow.RefNo.ToString()))
                    {
                        NoPendingDoc.ImageUrl = "../Data/Images/Icons/Thumb_up.png";
                        NoPendingDoc.ToolTip = Constants.HasPendingDocInApplication;
                        NoPendingDoc.Visible = true;
                    }
                }
            }
            else
                AppInformationLabel.Text = "-";

            dataBoundItem["Address"].Text = string.IsNullOrEmpty(AddressLabel.Text.Trim()) ? "-" : AddressLabel.Text;
        }
    }


    protected void NoGroupRadGridOLD_ItemDataBound(object sender, GridItemEventArgs e)
    {
        //Added by Edward 2015/8/24 use Local Variables instead of Global Variable - Reduce Error Notification
        UserDb userDb = new UserDb();
        int sectionIdOfUser = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
        //Added By Edward 17/3/2014 Sales and Resale Changes        
        if (sectionIdOfUser == 5 || sectionIdOfUser == 6 || sectionIdOfUser == 3)
        {
            NoGroupRadGrid.MasterTableView.Columns[4].HeaderText = "Ref No (OIC1/OIC2)";
        }

        if (e.Item is GridDataItem)
        {
            DataRowView data = e.Item.DataItem as DataRowView;
            try
            {
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                Label AddressLabel = new Label();
                Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
                
                // Compute the age value

                #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                //DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                //AgingLabel.Text = Format.GetAging(diff);
                DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                #endregion

                string status = data["Status"].ToString(); // Get the status

                //if (status.Equals(SetStatusEnum.Verified.ToString()))
                //{
                //    // Source: http://www.telerik.com/community/code-library/aspnet-ajax/grid/gridclientselectcolumn-select-all-rows-with-enabled-check-boxes-only.aspx
                //    CheckBox checkBox = (CheckBox)dataBoundItem["ChildCheckBoxColumn"].Controls[0]; 
                //    checkBox.Visible = false; 
                //}

                //handle viewonly/verify users

                if (isAllSetsReadOnlyUser)
                {
                    NoGroupRadGrid.Columns[1].Visible = false;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
                }
                else
                {
                    NoGroupRadGrid.Columns[1].Visible = true;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
                }

                //Import OIC
                //commented by Andrew (17/1/2013) as already added into vDocSet and Dal/DocSetDs.cs
                /*Label ImportedOICLabel = e.Item.FindControl("ImportedOICLabel") as Label;
                UserDb userDb = new UserDb();
                User.UserDataTable users = userDb.GetUser(new Guid(data["ImportedBy"].ToString()));
                User.UserRow userRow = users[0];
                ImportedOICLabel.Text = userRow.Name;*/

                //Applicattion Information
                int docSetId = int.Parse(data["Id"].ToString());
                int sectionId = int.Parse(data["SectionId"].ToString()); // Get the sectionId

                SetAppDb setAppDb = new SetAppDb();
                SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(docSetId);

                if (setApps.Rows.Count > 0)
                {
                    foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                    {
                        Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                        //show address only for SERS sections (3,4)
                        if (sectionId == 3 || sectionId == 4)
                        {
                            SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                            AddressLabel.Text += sersInterfaceDb.GetSersAddressByRefNo(setAppRow.RefNo) + "<br>";
                        }
			

			//Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
              //      	Util.DisplayHasPendingDocIndicator(NoPendingDoc, setAppRow.RefNo.ToString());

                        Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                        if (Util.DisplayHasPendingDocIndicator(setAppRow.RefNo.ToString()))
                        {
                            NoPendingDoc.ImageUrl = "../Data/Images/Icons/Thumb_up.png";
                            NoPendingDoc.ToolTip = Constants.HasPendingDocInApplication;
                            NoPendingDoc.Visible = true;
                        }
                    }
                }
                else
                    AppInformationLabel.Text = "-";

                //Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                //Util.DisplayHasPendingDocIndicator(NoPendingDoc, data["RefNo"].ToString());

                AppInformationLabel.Text = data["RefNoWithOIC"].ToString();

                dataBoundItem["Address"].Text = string.IsNullOrEmpty(AddressLabel.Text.Trim()) ? "-" : AddressLabel.Text;
            }
            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                string errorMessage = ex.Message + " for DocSet:" + data["Id"].ToString() + "<br><br>" + ex.InnerException + "<br><br>" + ex.StackTrace;
                errorLogDb.Insert("AllSets_ItemDataBound", errorMessage);
                //ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
            }

        }
    }

    #endregion

    #region GroupByDate
    protected void GroupByDateRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        

        if (e.Item is GridHeaderItem)
        {
            GridHeaderItem header = (GridHeaderItem)e.Item;
            //CheckBox ChildCheckBoxColumn =  (CheckBox)header["ChildCheckBoxColumn"].Controls[0];
            //ChildCheckBoxColumn.Enabled = false;
        }
        else if (e.Item is GridDataItem)
        {
            if (e.Item.OwnerTableView.Name.Equals("GroupByDateDetailTable"))
            {
                ////Added By Edward 17/3/2014 Sales and Resale Changes
                //if (g_sectionId == 5 || g_sectionId == 6 || g_sectionId == 3)
                //{
                //    e.Item.OwnerTableView.Columns[5].HeaderText = "Ref No (OIC1/OIC2)";
                //}


                GridDataItem dataBoundItem = e.Item as GridDataItem;
                //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                Label AddressLabel = new Label();
                Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;

                DataRowView data = e.Item.DataItem as DataRowView;

                #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                // Compute the age value
                //DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                //AgingLabel.Text = Format.GetAging(diff);
                DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                #endregion
 
                string status = data["Status"].ToString(); // Get the status

                //if (status.Equals(SetStatusEnum.Verified.ToString()))
                //{
                //    // Source: http://www.telerik.com/community/code-library/aspnet-ajax/grid/gridclientselectcolumn-select-all-rows-with-enabled-check-boxes-only.aspx
                //    CheckBox checkBox = (CheckBox)dataBoundItem["ChildCheckBoxColumn"].Controls[0]; 
                //    checkBox.Visible = false; 
                //}

                //handle viewonly/verify users

                if (isAllSetsReadOnlyUser)
                {
                    //GroupByDateRadGrid.Columns.FindByUniqueName("ChildCheckBoxColumn").Visible = false;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
                }
                else
                {
                    //GroupByDateRadGrid.Columns.FindByUniqueName("ChildCheckBoxColumn").Visible = true;
                    RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
                }

                //Imported OIC
                //commented by Andrew (17/1/2013) as already added into vDocSet and Dal/DocSetDs.cs
                /*Label ImportedOICLabel = e.Item.FindControl("ImportedOICLabel") as Label;
                UserDb userDb = new UserDb();
                User.UserDataTable users = userDb.GetUser(new Guid(data["ImportedBy"].ToString()));
                User.UserRow userRow = users[0];
                ImportedOICLabel.Text = userRow.Name;*/

                //Applicattion Information
                int docSetId = int.Parse(data["Id"].ToString());
                int sectionId = int.Parse(data["SectionId"].ToString()); // Get the sectionId

                SetAppDb setAppDb = new SetAppDb();
                SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(docSetId);

                if (setApps.Rows.Count > 0)
                {
                    foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                    {
                        Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                        //show address only for SERS sections (3,4)
                        if (sectionId == 3 || sectionId == 4)
                        {
                            SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                            AddressLabel.Text += sersInterfaceDb.GetSersAddressByRefNo(setAppRow.RefNo) + "<br>";
                        }

                        Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                        Util.DisplayHasPendingDocIndicator(NoPendingDoc, setAppRow.RefNo.ToString());
                    }
                }
                else
                    AppInformationLabel.Text = "-";

                dataBoundItem["Address"].Text = string.IsNullOrEmpty(AddressLabel.Text.Trim()) ? "-" : AddressLabel.Text;
            }
            else
            {
                //GridDataItem header = (GridDataItem)e.Item;
                //CheckBox ChildCheckBoxColumn = (CheckBox)header["ChildCheckBoxColumn"].Controls[0];
                //ChildCheckBoxColumn.Enabled = false;
            }
        }
        else
        {
        }
    }

    protected void GroupByDateRadGrid_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
    {
        GridDataItem dataItem = e.DetailTableView.ParentItem as GridDataItem;

        e.DetailTableView.DataSource = GetDataForDetailTable((DateTime)dataItem.GetDataKeyValue("VerificationDateIn"));
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

    #endregion

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
        ChannelRadComboBox.SelectedIndex = 0;
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

        PopulateList();
        RebindGrids();
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        PopulateList();
        RebindGrids();
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
    /// Populate the channels
    /// </summary>
    private void PopulateChannels()
    {
        DocSetDb docSetDb = new DocSetDb();
        ChannelRadComboBox.DataSource = docSetDb.GetChannelsForDropDown();
        ChannelRadComboBox.DataTextField = "Channel";
        ChannelRadComboBox.DataValueField = "Channel";
        ChannelRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the status
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.GetAllSetStatus();
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
    /// get Data
    /// </summary>
    private DataTable GetData(bool withGroup)
    {
        string channel = ChannelRadComboBox.SelectedValue;

        //string refNo = RefNoTextBox.Text.Trim();
        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
        #region Update by Edward 2017/11/13
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
        //if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        //{
        //    docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(DocAppRadTextBox.Text.Trim());            
        //}
        #endregion

        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        UserDb userDb = new UserDb();
        int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        //get verification OIC
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
        #region Update by Edward 2017/11/13
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
        #endregion

        // Get the Sets for the user's section
        DocSetDb docSetDb = new DocSetDb();

        SetStatusEnum setStatusEnum = new SetStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        DataTable sets;

        if (!status.Equals("-1"))
        {
            setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), StatusRadComboBox.SelectedValue, true);
            if (withGroup)
                sets = docSetDb.GetAllSetsVerificationDates(channel, setStatusEnum, string.Empty, userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
            else
                //sets = docSetDb.GetAllSets(channel, setStatusEnum, string.Empty, userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
                sets = docSetDb.GetAllSets(channel, setStatusEnum, DocAppRadTextBox.Text.Trim(), userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
        }
        else
        {
            if (withGroup)
                sets = docSetDb.GetAllSetsVerificationDates(channel, null, string.Empty, userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
            else
                sets = docSetDb.GetAllSets(channel, null, DocAppRadTextBox.Text.Trim(), userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
        }

        if (sets.Rows.Count == 0)
        {
            if (isAllSetsReadOnlyUser)
                RadMenu1.Visible = RadMenuHomeComplete.Visible = false;
            else
                RadMenu1.Visible = RadMenuHomeComplete.Visible = true;
        }

        return sets;
    }

    private DataTable GetDataForDetailTable(DateTime verificationDateIn)
    {
        string channel = ChannelRadComboBox.SelectedValue;

        //string refNo = RefNoTextBox.Text.Trim();
        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);

        #region Update by Edward 2017/11/13 to address OOM
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
        //            DocAppDb docAppDb = new DocAppDb();
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
        #endregion

        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        UserDb userDb = new UserDb();
        int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        //get verification OIC
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
        #region Update by Edward 2017/11/13
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
        #endregion

        // Get the Sets for the user's section
        DocSetDb docSetDb = new DocSetDb();

        SetStatusEnum setStatusEnum = new SetStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), StatusRadComboBox.SelectedValue, true);
            return docSetDb.GetAllSetsByVerificationDateIn(verificationDateIn, channel, setStatusEnum, string.Empty, userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
        }
        else
            return docSetDb.GetAllSetsByVerificationDateIn(verificationDateIn, channel, null, string.Empty, userId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
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

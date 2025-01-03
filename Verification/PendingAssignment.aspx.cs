﻿using System;
using System.Collections;
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
    string user;

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PopulateChannels();

            //show address if the user belong to SERS department
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
            NoGroupRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
            //GroupByDateRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);

            PopulateList();
            RebindGrids();
            GridMultiView.ActiveViewIndex = 0;           
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
        ChannelRadComboBox.SelectedIndex = 0;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;
        GroupByDateCheckBox.Checked = false;
        //GroupRadComboBox.SelectedIndex = 0;
        //Added by Edward 30.10.2013 Optimizing Listings
        //AcknowledgeNumberRadComboBox.Text = string.Empty;
        AcknowledgeNumberRadTextBox.Text = string.Empty;
        PopulateList();
        RebindGrids();
    }

    #region NoGroupRadGrid
    /// <summary>
    /// Radgrid need data source event
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
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;

            DataRowView data = (DataRowView)e.Item.DataItem;

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
            Label AddressLabel = new Label();

            //Imported OIC
            //commented by Andrew (17/1/2013) as already added in vDocSet and Dal/DocSetDs.cs
            /*Label ImportedOICLabel = e.Item.FindControl("ImportedOICLabel") as Label;
            UserDb userDb = new UserDb();
            User.UserDataTable users = userDb.GetUser(new Guid(data["ImportedBy"].ToString()));
            User.UserRow userRow = users[0];
            ImportedOICLabel.Text = userRow.Name;*/

            #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
            // Compute the age value
            //DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
            //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
            //AgingLabel.Text = Format.GetAging(diff);
            DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
            DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
            AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
            #endregion

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

            dataBoundItem["Address"].Text = AddressLabel.Text;

        }
    }

    #endregion

    #region GroupByDateRadGrid
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

    protected void GroupByDateRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            if (e.Item.OwnerTableView.Name.Equals("GroupByDateDetailTable"))
            {
                GridDataItem dataBoundItem = e.Item as GridDataItem;

                DataRowView data = (DataRowView)e.Item.DataItem;
                
                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
                Label AddressLabel = new Label();

                //Imported OIC
                //commented by Andrew (17/1/2013) as already added in vDocSet and Dal/DocSetDs.cs
                /*Label ImportedOICLabel = e.Item.FindControl("ImportedOICLabel") as Label;
                UserDb userDb = new UserDb();
                User.UserDataTable users = userDb.GetUser(new Guid(data["ImportedBy"].ToString()));
                User.UserRow userRow = users[0];
                ImportedOICLabel.Text = userRow.Name;*/

                #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                // Compute the age value
                //DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                //AgingLabel.Text = Format.GetAging(diff);
                DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                #endregion
                
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

                dataBoundItem["Address"].Text = AddressLabel.Text;
            }
        }
    }
    
    protected void GroupByDateRadGrid_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
    {
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;

        //if (e.DetailTableView.Name.Equals("GroupByDateDetailTable"))
        //{
            e.DetailTableView.DataSource = GetDataForDetailTable((DateTime)dataItem.GetDataKeyValue("VerificationDateIn"));            
        //}
    }
    #endregion
    #endregion

    #region Private Methods
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
    /// Populate without group grid
    /// </summary>
    private void PopulateDataWithoutGroup()
    {
        NoGroupRadGrid.DataSource = GetData(false);
        //NoGroupRadGrid.DataBind();
    }

    /// <summary>
    /// Populate with group grid
    /// </summary>
    private void PopulateDataWithGroup()
    {
        GroupByDateRadGrid.DataSource = GetData(true);

        //GroupByDateRadGrid.DataBind();
    }
    
    /// <summary>
    /// Get data
    /// </summary>
    /// <returns></returns>
    private DataTable GetData(bool withGroup)
    {
        string channel = ChannelRadComboBox.SelectedValue;
        int docAppId = -1;

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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        UserDb userDb = new UserDb();
        int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

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
        // Get the Sets for the user's section
        DocSetDb docSetDb = new DocSetDb();

        if(!withGroup)
            return docSetDb.GetPendingAssignmentSets(channel, string.Empty, dateInFrom, dateInTo, sectionId, SetStatusEnum.New, docAppId, nric);
        else
            return docSetDb.GetPendingAssignmentSetsVerificationDates(channel, string.Empty, dateInFrom, dateInTo, sectionId, SetStatusEnum.New, docAppId, nric);
    }

    /// <summary>
    /// Get the data for the detail table
    /// </summary>
    /// <param name="verificationDate"></param>
    /// <returns></returns>
    private DataTable GetDataForDetailTable(DateTime verificationDateIn)
    {
        string channel = ChannelRadComboBox.SelectedValue;
        int docAppId = -1;

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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        UserDb userDb = new UserDb();
        int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

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
        // Get the Sets for the user's section
        DocSetDb docSetDb = new DocSetDb();

        return docSetDb.GetPendingAssignmentSetsByVerificationDateIn(verificationDateIn, channel, string.Empty, 
            dateInFrom, dateInTo, sectionId, SetStatusEnum.New, docAppId, nric);
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

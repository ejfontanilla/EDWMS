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

public partial class Filing : System.Web.UI.Page
{
    string user;
    int sectionId = -1;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PopulateUsers();
            PopulateStatus();
            PopulateDownloadStatus();
        }

        AccessControlDb accessControlDb = new AccessControlDb();
        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness);
    }

    /// <summary>
    /// Rad grid need data source
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

            DataRowView data = (DataRowView)e.Item.DataItem;

            //set status and status date
            //set status and status date 
            Label StatusLabel = e.Item.FindControl("StatusLabel") as Label;
            Label StatusDateLabel = e.Item.FindControl("StatusDateLabel") as Label;

            HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
            HleInterface.HleInterfaceDataTable hleInterface = hleInterfaceDb.GetHleInterfaceByRefNo(data["RefNo"].ToString());

            if (hleInterface.Rows.Count > 0)
            {
                HleInterface.HleInterfaceRow hleInterfaceRow = hleInterface[0];
                StatusLabel.Text = hleInterfaceRow.HleStatus.Replace("_", " ");

                if (hleInterfaceRow.HleStatus.ToString().Trim().Equals(HleStatusEnum.Approved.ToString()))
                    StatusDateLabel.Text = hleInterfaceRow.IsApprovedDateNull() ? "-" : hleInterfaceRow.ApprovedDate;
                else if (hleInterfaceRow.HleStatus.ToString().Trim().Equals(HleStatusEnum.Cancelled.ToString()))
                    StatusDateLabel.Text = hleInterfaceRow.IsCancelledDateNull() ? "-" : hleInterfaceRow.CancelledDate;
                else if (hleInterfaceRow.HleStatus.ToString().Trim().Equals(HleStatusEnum.Expired.ToString()))
                    StatusDateLabel.Text = hleInterfaceRow.IsExpiredDateNull() ? "-" : hleInterfaceRow.ExpiredDate;
                else if (hleInterfaceRow.HleStatus.ToString().Trim().Equals(HleStatusEnum.Rejected.ToString()))
                    StatusDateLabel.Text = hleInterfaceRow.IsRejectedDateNull() ? "-" : hleInterfaceRow.RejectedDate;
                else
                    StatusDateLabel.Text = "-";
            }
            else
            {
                StatusLabel.Text = StatusDateLabel.Text = "-";
            }

            //ClosedBy
            Label ClosedByLabel = e.Item.FindControl("ClosedByLabel") as Label;
            ClosedByLabel.Text = "-";

            if (data["downloadstatus"].ToString().Trim().ToLower().Equals(DownloadStatusEnum.Downloaded.ToString().ToLower()))
            {
                UserDb userDb = new UserDb();
                Guid userId = new Guid(data["DownloadedBy"].ToString());
                User.UserDataTable user = userDb.GetUser(userId);
                User.UserRow userRow = user[0];
                ClosedByLabel.Text = userRow.Name;
            }
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
    /// Reset button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ResetButton_Click(object sender, EventArgs e)
    {
        UserRadComboBox.SelectedIndex = 0;
        StatusRadComboBox.SelectedIndex = 0;
        RefNoRadComboBox.Text = string.Empty;
        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;

        RadGrid1.Rebind();
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        RadGrid1.Rebind();
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

    /// <summary>
    /// On Download Status selection change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DownloadStatusRadComboBox_OnSelectedIndexChanged(Object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (DownloadStatusRadComboBox.SelectedValue.Trim().ToLower().Equals(DownloadStatusEnum.Pending_Download.ToString().ToLower()))
        {
            RadGrid1.Columns.FindByUniqueName("DownloadedOn").Visible = false;
            RadGrid1.Columns.FindByUniqueName("ClosedBy").Visible = false;
        }
        else
        {
            RadGrid1.Columns.FindByUniqueName("DownloadedOn").Visible = true;
            RadGrid1.Columns.FindByUniqueName("ClosedBy").Visible = true;
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
    /// Populate Status
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.GetHleStatus();
        StatusRadComboBox.DataTextField = "Text";
        StatusRadComboBox.DataValueField = "Value";
        StatusRadComboBox.SelectedIndex = 0;
        StatusRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate Download Status
    /// </summary>
    private void PopulateDownloadStatus()
    {
        DownloadStatusRadComboBox.DataSource = EnumManager.GetDownloadStatus();
        DownloadStatusRadComboBox.DataTextField = "Text";
        DownloadStatusRadComboBox.DataValueField = "Value";
        DownloadStatusRadComboBox.SelectedIndex = 0;
        DownloadStatusRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        DocAppDb docAppDb = new DocAppDb();

        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);

        if (!string.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue))
            {
                docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
            }
            else
            {
                RadComboBoxItem selectedItem = RefNoRadComboBox.FindItemByText(RefNoRadComboBox.Text.Trim());

                if (selectedItem != null)
                {
                    docAppId = int.Parse(selectedItem.Value);
                }
                else
                {
                    DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

                    if (docAppTable.Rows.Count > 0)
                    {
                        DocApp.DocAppRow docAppRow = docAppTable[0];
                        docAppId = docAppRow.Id;
                    }
                }
            }
        }

        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        UserDb userDb = new UserDb();
        int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        string userIdStr = UserRadComboBox.SelectedValue;
        Guid? userId = null;

        if (!userIdStr.Equals("-1") && !string.IsNullOrEmpty(userIdStr.Trim()))
        {
            userId = new Guid(userIdStr);
        }

        //get nric
        string nric = string.Empty;
        if (!string.IsNullOrEmpty(NricRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(NricRadComboBox.SelectedValue))
            {
                nric = NricRadComboBox.SelectedValue;
            }
            else
                nric = NricRadComboBox.Text.Trim();
        }

        // Get the Sets for the user's section
        RadGrid1.DataSource = docAppDb.GetAllApps(AppStatusEnum.Completeness_Checked, userId, dateInFrom, dateInTo, sectionId, docAppId, DownloadStatusRadComboBox.SelectedValue.Equals("-1") ? string.Empty : DownloadStatusRadComboBox.SelectedValue.Trim(), true, StatusRadComboBox.SelectedValue.Equals("-1") ? string.Empty : StatusRadComboBox.SelectedValue, nric, string.Empty);
    }

    #endregion
}
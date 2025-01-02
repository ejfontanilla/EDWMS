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
using Dwms.Web;

public partial class IncomeAssessment_PendingAssignment : System.Web.UI.Page
{
    //string user;          Commented by Edward 2016/02/04 take out unused variables
    int sectionId = -1;

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();
        sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);

        if (!IsPostBack)
        {
            //show address if the user belong to SERS department
            NoGroupRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
            GroupByDateRadGrid.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);

            PopulateList();
            RebindGrids();
            GridMultiView.ActiveViewIndex = 0;           
        }
    }

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

        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;
        GroupByDateCheckBox.Checked = false;
        //GroupRadComboBox.SelectedIndex = 0;

        PopulateList();
        RebindGrids();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void NoGroupRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        //Added By Edward 17/3/2014 Sales and Resale Changes
        if (sectionId == 5 || sectionId == 6)
        {
            NoGroupRadGrid.MasterTableView.Columns[3].HeaderText = "OIC 1";
            NoGroupRadGrid.MasterTableView.Columns[4].HeaderText = "OIC 2";
        }

        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;
            
            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            //show address only for SERS sections (3,4)
            Label AddressLabel = e.Item.FindControl("AddressLabel") as Label;
            LinkButton SetCountLinkButton = e.Item.FindControl("SetCountLinkButton") as LinkButton;
            if (sectionId == 3 || sectionId == 4)
            {
                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                AddressLabel.Text = sersInterfaceDb.GetSersAddressByRefNo(data["RefNo"].ToString());
            }

            // Compute the age value
            DateTime dateInDateTime;

            if (Validation.IsDate(data["AssessmentDateIn"].ToString()))
            {
                dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                AgingLabel.Text = Format.GetAging(diff);
            }
            else
            {
                AgingLabel.Text = "? Day(s)";
            }

            //Set Information
            int docAppId = int.Parse(data["Id"].ToString());
            Label SetInformationLabel = e.Item.FindControl("SetInformationLabel") as Label;

            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId);

            Util.DisplaySetInformation(docSets.Rows.Count, SetCountLinkButton, docAppId);
        }
    }
    #endregion

    #region Group by Date

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

    protected void GroupByDateRadGrid_ItemCommand(object sender, GridCommandEventArgs e)
    {
        if (e.CommandName.Equals("Check"))
        {
            int id = int.Parse(e.CommandArgument.ToString());

            DocAppDb docAppDb = new DocAppDb();

            bool success = docAppDb.AssignUserAsCompletenessOfficer(id, (Guid)Membership.GetUser().ProviderUserKey);

            if (success)
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:window.open('{0}');", "View.aspx?id=" + id), true);
        }
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
            GroupByDateRadGrid.MasterTableView.Columns[3].HeaderText = "OIC 1";
            GroupByDateRadGrid.MasterTableView.Columns[4].HeaderText = "OIC 2";
        }

        if (e.Item is GridDataItem)
        {
            if (e.Item.OwnerTableView.Name.Equals("GroupByDateDetailTable"))
            {
                DataRowView data = (DataRowView)e.Item.DataItem;

                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                //show address only for SERS sections (3,4)
                Label AddressLabel = e.Item.FindControl("AddressLabel") as Label;
                if (sectionId == 3 || sectionId == 4)
                {
                    SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                    AddressLabel.Text = sersInterfaceDb.GetSersAddressByRefNo(data["RefNo"].ToString());
                }

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

                //Set Information
                int docAppId = int.Parse(data["Id"].ToString());
                Label SetInformationLabel = e.Item.FindControl("SetInformationLabel") as Label;

                DocSetDb docSetDb = new DocSetDb();
                DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId);

                if (docSets.Rows.Count > 0)
                {
                    foreach (DocSet.DocSetRow docSetRow in docSets.Rows)
                    {
                        SetInformationLabel.Text += docSetRow.SetNo + ": " + docSetRow.Status.Replace("_", " ") + "<br>";
                    }
                }
                else
                    SetInformationLabel.Text = "-";
            }
        }
    }

    protected void GroupByDateRadGrid_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
    {
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
        e.DetailTableView.DataSource = GetDataForDetailTable((DateTime)dataItem.GetDataKeyValue("AssessmentDateIn"));
    }
    #endregion

    #region Private Methods

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
    /// Populate the list
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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        // Get the Sets for the user's section
        #region Modified by Edward 2014/5/23   Changes in Sales and Resale
        //if (!withGroup)
        //    return docAppDb.GetPendingAssignmentAppsIA(docAppId, dateInFrom, dateInTo, AssessmentStatusEnum.Completeness_Checked, AssessmentStatusEnum.Completeness_Cancelled, sectionId, nric);
        //else
        //    return docAppDb.GetPendingAssignmentAppsVerificationDatesIA(docAppId, dateInFrom, dateInTo, AssessmentStatusEnum.Completeness_Checked, AssessmentStatusEnum.Completeness_Cancelled, sectionId, nric);
        AssessmentStatusEnum assessmentStatus = AssessmentStatusEnum.Completeness_Checked;
        if (sectionId == 5 || sectionId == 6)
            assessmentStatus = AssessmentStatusEnum.Verified;

        if (!withGroup)
            return docAppDb.GetPendingAssignmentAppsIA(docAppId, dateInFrom, dateInTo, assessmentStatus, AssessmentStatusEnum.Completeness_Cancelled, sectionId, nric);
        else
            return docAppDb.GetPendingAssignmentAppsVerificationDatesIA(docAppId, dateInFrom, dateInTo, assessmentStatus, AssessmentStatusEnum.Completeness_Cancelled, sectionId, nric);

        #endregion
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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        // Get the Sets for the user's section
        return docAppDb.GetPendingAssignmentSetsByVerificationDateInIA(verificationDateIn, docAppId, dateInFrom, dateInTo, AssessmentStatusEnum.Completeness_Checked, sectionId, nric);
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
    }
    

    #endregion
}

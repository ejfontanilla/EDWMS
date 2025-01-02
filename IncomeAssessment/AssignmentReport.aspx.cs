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
using Dwms.Dal;

public partial class IncomeAssessment_AssignmentReport : System.Web.UI.Page
{
    //string user;              Commented by Edward 2016/02/04 take out unused variables
    int sectionId = -1;
    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();
        sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);

        if (!IsPostBack)
        {
            DateInFromRadDateTimePicker.SelectedDate = DateTime.Today;
            DateInToRadDateTimePicker.SelectedDate = DateTime.Now;
            StatusRadComboBox.SelectedValue = AssessmentStatusEnum.Pending_Extraction.ToString(); ;
            PopulateUsers();
            PopulateStatus();
            PopulateList();
            RebindGrids();        
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
        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        StatusRadComboBox.SelectedValue = AppStatusEnum.Pending_Completeness.ToString();
        DateInFromRadDateTimePicker.SelectedDate = DateTime.Today; //null;
        DateInToRadDateTimePicker.SelectedDate = DateTime.Now; //null;
        UserRadComboBox.Text = string.Empty;

        //PopulateUsers();
        PopulateList();
        RebindGrids();
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
            GroupByDateRadGrid.MasterTableView.Columns[3].HeaderText = "OIC 1";
            GroupByDateRadGrid.MasterTableView.Columns[4].HeaderText = "OIC 2";
        }

        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            // Compute the age value
            DateTime dateInDateTime;

            if (Validation.IsDate(data["DateIn"].ToString()))
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

    /// <summary>
    /// on ObjectDataSource Selecting
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void GetUserObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
    }

    protected void UserRadComboBox_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (!String.IsNullOrEmpty(e.Value))
            CompletenessOicHiddenField.Value = e.Value;
        //else
        //    SelectTheFirstOic();
    }

    /// <summary>
    /// Export Link Button Click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ExportLinkButton_Click(object sender, EventArgs e)
    {
        if (GroupByDateRadGrid.Items.Count > 0)
        {
            // Set export settings
            GroupByDateRadGrid.ExportSettings.OpenInNewWindow = true;            
            GroupByDateRadGrid.ExportSettings.IgnorePaging = true;
            GroupByDateRadGrid.ExportSettings.ExportOnlyData = true;

            // Set export file name
            GroupByDateRadGrid.ExportSettings.FileName = String.Format("Assignment Report for {0}", UserRadComboBox.Text);

            Page.Response.ClearHeaders();
            Page.Response.Cache.SetCacheability(HttpCacheability.Private);

            // Export RadGrid data            
            GroupByDateRadGrid.MasterTableView.ExportToExcel();
        }
    }

    protected void RadGrid1_ExcelExportCellFormatting(object sender, ExcelExportCellFormattingEventArgs e)
    {
        //// Source: http://www.telerik.com/help/aspnet-ajax/grid-html-export.html
        //GridDataItem item = e.Cell.Parent as GridDataItem;
        //item.Style["border"] = Constants.ExcelExportCellStyle;
    }
    #endregion

    #region Private Methods

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
        GroupByDateRadGrid.DataSource = GetData();
    }

    /// <summary>
    /// Populate users drop down list
    /// </summary>
    private void PopulateUsers()
    {
        UserRadComboBox.DataSourceID = "GetUserObjectDataSource";
        UserRadComboBox.DataTextField = "Name";
        UserRadComboBox.DataValueField = "UserId";

        // Select the first OIC as default
        //SelectTheFirstOic();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private DataTable GetData()
    {
        DocAppDb docAppDb = new DocAppDb();

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

        Guid? completenessOic = null;

        if (!String.IsNullOrEmpty(UserRadComboBox.Text.Trim()))
        {            
            DataTable oicDt = docAppDb.GetCompletenessOfficers();

            foreach(DataRow oic in oicDt.Rows)
            {
                if (oic["UserId"].ToString().ToLower().Equals( UserRadComboBox.SelectedValue.ToLower()))
                {
                    completenessOic = new Guid(UserRadComboBox.SelectedValue);
                    break;
                }
            }
        }

        /*if (!completenessOic.HasValue && (DateInFromRadDateTimePicker.SelectedDate.HasValue || DateInToRadDateTimePicker.SelectedDate.HasValue))
        {
            GridSortExpression SortByCompletenessOIC = new GridSortExpression();
            SortByCompletenessOIC.FieldName = "CompletenessOIC";
            SortByCompletenessOIC.SortOrder = GridSortOrder.Ascending;
            GroupByDateRadGrid.MasterTableView.SortExpressions.AddSortExpression(SortByCompletenessOIC);
        }
        */
        AssessmentStatusEnum assessmentStatusEnum = new AssessmentStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            assessmentStatusEnum = (AssessmentStatusEnum)Enum.Parse(typeof(AssessmentStatusEnum), StatusRadComboBox.SelectedValue, true);
            //return docAppDb.GetAssessmentOfficersForAssignedCasesReportDetailsIA(completenessOic, docAppId, RefNoRadComboBox.Text.Trim(), assessmentStatusEnum, dateInFrom, dateInTo, nric);
            return docAppDb.GetAssessmentOfficersForAssignedCasesReportDetailsIA(completenessOic, docAppId, DocAppRadTextBox.Text.Trim(), assessmentStatusEnum, dateInFrom, dateInTo, nric);
        }
        else
            //return docAppDb.GetAssessmentOfficersForAssignedCasesReportDetailsIA(completenessOic, docAppId, RefNoRadComboBox.Text.Trim(), null, dateInFrom, dateInTo, nric);
            return docAppDb.GetAssessmentOfficersForAssignedCasesReportDetailsIA(completenessOic, docAppId, DocAppRadTextBox.Text.Trim(), null, dateInFrom, dateInTo, nric);

        /*if (completenessOic.HasValue)
            return docAppDb.GetCompletenessOfficersForAssignedCasesReportDetails(completenessOic, docAppId, RefNoRadComboBox.Text.Trim(), dateInFrom, dateInTo);
        else
            return new DataTable();*/
    }

    private void RebindGrids()
    {
        GroupByDateRadGrid.DataBind();
    }

    private void SelectTheFirstOic()
    {
        // Select the first OIC as default
        DocAppDb docAppDb = new DocAppDb();
        DataTable oicDt = docAppDb.GetCompletenessOfficers();

        if (oicDt.Rows.Count > 0)
        {
            UserRadComboBox.Text = oicDt.Rows[0]["Name"].ToString();
            CompletenessOicHiddenField.Value = oicDt.Rows[0]["UserId"].ToString();
        }
    }
    #endregion
}

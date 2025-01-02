﻿using System;
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

public partial class Verification_AssignmentReport : System.Web.UI.Page
{
    string user;

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateInFromRadDateTimePicker.SelectedDate = DateTime.Today;
            DateInToRadDateTimePicker.SelectedDate = DateTime.Now;
            
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
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            // Compute the age value
            DateTime dateInDateTime;

            if (Validation.IsDate(data["VerificationDateIn"].ToString()))
            {
                dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
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
        UserDb userDb = new UserDb();
        e.InputParameters["section"] = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);
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
        DocSetDb docSetDb = new DocSetDb();

        int docAppId = -1;

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

        Guid? verificationOIC = null;
          
        if (!string.IsNullOrEmpty(UserRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(UserRadComboBox.SelectedValue))
                verificationOIC = new Guid(UserRadComboBox.SelectedValue);
        }

        
        SetStatusEnum setStatusEnum = new SetStatusEnum();
        string status = StatusRadComboBox.SelectedValue;
        

        if (!status.Equals("-1"))
        {
            setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), StatusRadComboBox.SelectedValue, true);
            //return docSetDb.GetVerificationOICForReportDetails(verificationOIC, docAppId, RefNoRadComboBox.Text.Trim(), setStatusEnum, dateInFrom, dateInTo, nric);
            return docSetDb.GetVerificationOICForReportDetails(verificationOIC, docAppId, DocAppRadTextBox.Text.Trim(), setStatusEnum, dateInFrom, dateInTo, nric);
        }
        else
            //return docSetDb.GetVerificationOICForReportDetails(verificationOIC, docAppId, RefNoRadComboBox.Text.Trim(), null, dateInFrom, dateInTo, nric);
            return docSetDb.GetVerificationOICForReportDetails(verificationOIC, docAppId, DocAppRadTextBox.Text.Trim(), null, dateInFrom, dateInTo, nric);
    }

    private void RebindGrids()
    {
        GroupByDateRadGrid.DataBind();
    }
  
    #endregion
}
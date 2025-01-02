using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI.GridExcelBuilder;

public partial class Maintenance_AuditTrail_Default : System.Web.UI.Page
{
    bool isExport = false;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        RoleDb roleDb = new RoleDb();
        CustomFilteringColumn UserRoleCol =
            RadGrid1.MasterTableView.Columns.FindByUniqueName("UserRole") as CustomFilteringColumn;
        UserRoleCol.ListDataSource = roleDb.GetRole();
        UserRoleCol.DataTextField = "RoleNameText";
        UserRoleCol.DataValueField = "RoleName";

        CustomFilteringColumn OperationCol =
            RadGrid1.MasterTableView.Columns.FindByUniqueName("Operation") as CustomFilteringColumn;
        OperationCol.ListDataSource = EnumManager.EnumToDataTable(typeof(OperationTypeEnum));

        CustomFilteringColumn TableNameCol =
        RadGrid1.MasterTableView.Columns.FindByUniqueName("TableName") as CustomFilteringColumn;
        TableNameCol.ListDataSource = EnumManager.GetAuditTables();
    }

    /// <summary>
    /// Rad grid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = (GridDataItem)e.Item;

            string tableName = (string)DataBinder.Eval(e.Item.DataItem, "TableName");

            #region Commented out by Min on Friday, 24 June 2011

            // Using Hyperlinks to display the "Operation" to remove dead hyperlinks in the exported Excel file.

            //string id = (string)DataBinder.Eval(e.Item.DataItem, "RecordId");
            //string operation = (string)DataBinder.Eval(e.Item.DataItem, "Operation");
            //string operationId = DataBinder.Eval(e.Item.DataItem, "OperationId").ToString();
           
            //dataItem["Operation"].Text = "<a title='View audit detail' href=\"javascript:ShowWindow('View.aspx?table=" +
            //    tableName + "&id=" + id + "#" + operationId + "', 850, 600)\">" + operation + "</a>";

            #endregion

            dataItem["TableName"].Text = EnumManager.FormatTableName(tableName);
        }
    }

    /// <summary>
    /// Rad grid need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        bool isArchived = Request["v"] == "archived";
        AuditTrailDb auditTrailDb = new AuditTrailDb();

        if (isArchived)
        {
            TitleLabel.Text = "Archived Audit Trail";
            RadGrid1.DataSource = auditTrailDb.GetArchivedRecords();
            HyperLink1.NavigateUrl = "Default.aspx";
            HyperLink1.Text = "Back to Audit Trail";
        }
        else
        {
            TitleLabel.Text = "Audit Trail";
            RadGrid1.DataSource = auditTrailDb.GetAuditTrail();
            HyperLink1.NavigateUrl = "Default.aspx?v=archived";
            HyperLink1.Text = "View Archived Audit Trail";
        }
    }

    /// <summary>
    /// Export table to Microsoft Excel format
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ExportExcel(object sender, EventArgs e)
    {
        if (RadGrid1.Items.Count > 0)
        {
            isExport = true;

            RadGrid1.ExportSettings.OpenInNewWindow = true;
            RadGrid1.ExportSettings.ExportOnlyData = true;
            RadGrid1.ExportSettings.IgnorePaging = true;

            RadGrid1.ExportSettings.FileName = "Audit Trail " +
                Format.FormatDateTime(DateTime.Now, DateTimeFormat.dd__MMM__yyyy);

            try
            {
                RadGrid1.MasterTableView.ExportToExcel();
            }
            catch (Exception ex)
            {
                Response.Write(ex);
            }
        }
    }

    /// <summary>
    /// Rad grid Export Excel event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ExcelExportCellFormatting(object sender, ExcelExportCellFormattingEventArgs e)
    {
        // Source: http://www.telerik.com/help/aspnet-ajax/grid-html-export.html
        GridDataItem item = e.Cell.Parent as GridDataItem;
        item.Style["border"] = Constants.ExcelExportCellStyle;
    }

    /// <summary>
    /// Rad grid Item Created event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemCreated(object sender, GridItemEventArgs e)
    {
        // Source: http://www.telerik.com/help/aspnet-ajax/grid-html-export.html
        if ((e.Item is GridHeaderItem || e.Item is GridFilteringItem) && isExport)
            e.Item.Style["border"] = Constants.ExcelExportCellStyle;
    }

    #endregion

    #region Protected Methods
    /// <summary>
    /// Get the Audit Trail detail URL link
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="recordId"></param>
    /// <param name="operationId"></param>
    /// <returns></returns>
    protected string GetAuditDetailUrl(object tableName, object recordId, object operationId)
    {
        string detailUrl = "#";
       
        detailUrl = String.Format("javascript:ShowWindow('View.aspx?table={0}&id={1}#{2}', 850, 600)",
            tableName.ToString(), recordId.ToString(), operationId.ToString());

        return detailUrl;
    }

    #endregion
}
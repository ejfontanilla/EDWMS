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
using System.Data;

public partial class Maintenance_DocumentType_Default : System.Web.UI.Page
{
    bool hasManageAllAccess = false;
    bool hasManageDepartmentAccess = false;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the access control for the user
        SetAccessControl();

        
        CustomFilteringColumn AcquireNewSamples = RadGrid1.MasterTableView.Columns.FindByUniqueName("AcquireNewSamples") as CustomFilteringColumn;

        DataTable dt = new DataTable();
        DataColumn c = new DataColumn("AcquireNewSamples");
        dt.Columns.Add(c);
        DataRow r;
        r = dt.NewRow();
        r["AcquireNewSamples"] = true;
        dt.Rows.Add(r);
        r = dt.NewRow();
        r["AcquireNewSamples"] = false;
        dt.Rows.Add(r);
        

        AcquireNewSamples.ListDataSource =  dt;
        AcquireNewSamples.DataTextField = "AcquireNewSamples";
        AcquireNewSamples.DataValueField = "AcquireNewSamples";

        //Added by Edward 2015/8/26 Add IsActive in DocType
        CustomFilteringColumn IsActive = RadGrid1.MasterTableView.Columns.FindByUniqueName("IsActive") as CustomFilteringColumn;

        dt = new DataTable();
        c = new DataColumn("IsActive");
        dt.Columns.Add(c);        
        r = dt.NewRow();
        r["IsActive"] = true;
        dt.Rows.Add(r);
        r = dt.NewRow();
        r["IsActive"] = false;
        dt.Rows.Add(r);


        IsActive.ListDataSource = dt;
        IsActive.DataTextField = "IsActive";
        IsActive.DataValueField = "IsActive"; 

        if (!IsPostBack)
        {
            NewButton.Attributes["onclick"] = "location.href='Edit.aspx'";
            ToggleNewButtonVisibility();
        }
    }

    /// <summary>
    /// RadGrid need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateRadGrid();
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
            Label ItemCountLabel = e.Item.FindControl("ItemCountLabel") as Label;
            int page = RadGrid1.MasterTableView.PagingManager.CurrentPageIndex;
            ItemCountLabel.Text = (page * RadGrid1.PageSize + e.Item.ItemIndex + 1).ToString();
        }
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the RadGrid with data
    /// </summary>
    private void PopulateRadGrid()
    {
        DocTypeDb docTypeDb = new DocTypeDb();
        RadGrid1.DataSource = docTypeDb.GetDocType();
    }

    /// <summary>
    /// Toggle new button visibility
    /// </summary>
    private void ToggleNewButtonVisibility()
    {
        DocTypeDb docTypeDb = new DocTypeDb();
        NewButton.Visible = docTypeDb.HasDocTypeNotShown();
    }

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasManageAllAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);
        hasManageDepartmentAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_Department);

        bool hasAccess = (hasManageAllAccess || hasManageDepartmentAccess);

        // Set the visibility of the buttons
        NewButton.Visible = hasAccess;
    }
    #endregion
}

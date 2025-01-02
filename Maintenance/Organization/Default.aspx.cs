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

public partial class Maintenance_Organization : System.Web.UI.Page
{
    protected string role;

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
            NewButton.Attributes["onclick"] = "javascript:ShowWindow('Edit.aspx', 700, 400)";
        }

        // Set access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Item created event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemCreated(object sender, GridItemEventArgs e)
    {
        GridItem item = e.Item;

        if (e.Item is GridDataItem)
        {
            Label ItemCountLabel = item.FindControl("ItemCountLabel") as Label;
            int page = RadGrid1.MasterTableView.PagingManager.CurrentPageIndex;
            ItemCountLabel.Text = (page * RadGrid1.PageSize + e.Item.ItemIndex + 1).ToString();
        }
    }

    /// <summary>
    /// Need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        DepartmentDb DepartmentDb = new Dwms.Bll.DepartmentDb();
        RadGrid1.DataSource = DepartmentDb.GetDepartmentForDisplay();
    }

    /// <summary>
    /// DataTable bound
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
    {
        SectionDb sectionDb = new SectionDb(); 
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
        int id = int.Parse(dataItem.GetDataKeyValue("Id").ToString());
        string code = dataItem.GetDataKeyValue("Code").ToString();
        e.DetailTableView.DataSource = sectionDb.GetSectionByDepartmentWithMailingList(id);
    }


    /// <summary>
    /// Item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            string id = DataBinder.Eval(e.Item.DataItem, "Id").ToString();

            HyperLink edit = e.Item.FindControl("Edit") as HyperLink;
            edit.NavigateUrl = "javascript:ShowWindow('Edit.aspx?id=" + id + "', 700, 400)";

        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        RadGrid1.Rebind();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        //NewButton.Visible = hasAccess;
        //set the visibility to false as of now.
        NewButton.Visible = false;

    }
    #endregion
}

using System;
using System.Data;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Maintenance_UserAccounts_Default : System.Web.UI.Page
{
    protected string role;

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
        NewButton.Attributes["onclick"] = "location.href='Edit.aspx'";

        // Filtering dropdown list
        DepartmentDb departmentDb = new DepartmentDb();
        CustomFilteringColumn DepartmentCol = RadGrid1.MasterTableView.Columns.FindByUniqueName("DepartmentCode") as CustomFilteringColumn;

        if (hasManageDepartmentAccess)
            DepartmentCol.ListDataSource = (DataTable)departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
        else
            DepartmentCol.ListDataSource = (DataTable)departmentDb.GetDepartment();

        DepartmentCol.DataTextField = "Code";
        DepartmentCol.DataValueField = "Code"; 

        // Section dropdown list
        SectionDb sectionDb = new SectionDb();
        DataTable section  = (DataTable)sectionDb.GetSection();
        CustomFilteringColumn SectionCol = RadGrid1.MasterTableView.Columns.FindByUniqueName("SectionCode") as CustomFilteringColumn;
        SectionCol.ListDataSource = section;
        SectionCol.DataTextField = "Code";
        SectionCol.DataValueField = "Code";
        //SectionCol.FilterControlWidth = 10;

        MasterListItemDb masterListItemDb = new MasterListItemDb();
        CustomFilteringColumn TeamCol = RadGrid1.MasterTableView.Columns.FindByUniqueName("Team") as CustomFilteringColumn;
        TeamCol.ListDataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Teams.ToString());
        TeamCol.DataTextField = "Name";
        TeamCol.DataValueField = "Name";
        //TeamCol.FilterControlWidth=14;

        PopulateStatus();
        PopulateRoles();

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
        UserDb userDb = new UserDb();
        if (hasManageDepartmentAccess)
        {
            DepartmentDb departmentDb = new DepartmentDb();
            Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
            Department.DepartmentRow departmentRow = departments[0];
            RadGrid1.DataSource = userDb.GetUserByDepartment(departmentRow.Id);
        }
        else
            RadGrid1.DataSource = userDb.GetUser();
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
            GridDataItem dataItem = (GridDataItem)e.Item;

            string designation = DataBinder.Eval(e.Item.DataItem, "Designation").ToString();

            if (string.IsNullOrEmpty(designation))
            {
                dataItem["Designation"].Text = "N.A.";
            }
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate roles
    /// </summary>
    private void PopulateRoles()
    {
        RoleDb roleDb = new RoleDb();

        CustomFilteringColumn UserRoleCol =
            RadGrid1.MasterTableView.Columns.FindByUniqueName("RoleName") as CustomFilteringColumn;
        UserRoleCol.ListDataSource = roleDb.GetRole();
        UserRoleCol.DataTextField = "RoleNameText";
        UserRoleCol.DataValueField = "RoleName";
    }

    private void PopulateStatus()
    {
        CustomFilteringColumn TableNameCol =
        RadGrid1.MasterTableView.Columns.FindByUniqueName("IsApproved") as CustomFilteringColumn;
        TableNameCol.ListDataSource =  EnumManager.EnumToDataTable(typeof(UserActiveStatusEnum));
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

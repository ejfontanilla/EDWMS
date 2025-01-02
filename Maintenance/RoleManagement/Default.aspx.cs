using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Web;
using System.Configuration.Provider;
using Dwms.Bll;

public partial class Default : System.Web.UI.Page
{
    bool hasManageAllAccess = false;
    bool hasManageDepartmentAccess = false;

    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            NewButton.Attributes["onclick"] = "javascript:ShowWindow('Edit.aspx'," +
                Constants.WindowWidth + "," + Constants.WindowHeight + ")";
        }

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// RadGrid item created event
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
    /// RadGrid need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        RoleDb roleDb = new RoleDb();

        MembershipUser user = Membership.GetUser();
        DepartmentDb departmentDb = new DepartmentDb();
        Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)user.ProviderUserKey);
        Department.DepartmentRow departmentRow = departments[0];

        RadGrid1.DataSource = roleDb.GetRoleByDepartment(departmentRow.Id, Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All));
    }

    /// <summary>
    /// AjaxManager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        RadGrid1.Rebind();
    }

    /// <summary>
    /// RadGrid item command event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemCommand(object source, GridCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            string roleName = e.CommandArgument.ToString();
            bool success = false;

            try
            {
                RoleDb roleDb = new RoleDb();

                AuditTrailDb auditTrailDb = new AuditTrailDb();
                MembershipUser user = Membership.GetUser();
                Guid? operationId = auditTrailDb.Record(TableNameEnum.aspnet_Roles, roleDb.GetRoleGuid(roleName).ToString(), OperationTypeEnum.Delete);
               
                success = roleDb.Delete(roleName);

                if (!success)
                {
                    auditTrailDb.Delete(operationId);
                }
            }
            catch (Exception)
            {
                success = false;
            }

            if (success)
            {
                e.Item.OwnerTableView.Rebind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
                    "DeleteActionFailScript", "alert('" + Constants.UnableToDeleteRoleErrorMessage + "');", true);
            }
        }
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
            Guid roleId = new Guid(DataBinder.Eval(e.Item.DataItem, "RoleId").ToString());

            HyperLink EditHyperLink = e.Item.FindControl("Edit") as HyperLink;
            LinkButton DeleteLinkButton = e.Item.FindControl("DeleteLinkButton") as LinkButton;
            Label SeperatorLabel = e.Item.FindControl("SeperatorLabel") as Label;

            DataRowView data = (DataRowView)e.Item.DataItem;

            // Set access control for the 'Action' links
            bool hasAccess = hasManageAllAccess || hasManageDepartmentAccess;
            EditHyperLink.Visible = hasAccess;
            DeleteLinkButton.Visible = hasAccess;
            SeperatorLabel.Visible = hasAccess;

            if (hasAccess)
            {
                if (!data["LoweredRoleName"].ToString().Trim().Equals(RoleEnum.System_Administrator.ToString().Replace("_", " ").ToLower()))
                {
                    EditHyperLink.NavigateUrl = "javascript:ShowWindow('Edit.aspx?roleId=" + roleId + "'," +
                        Constants.WindowWidth + "," + Constants.WindowHeight + ")";
                    SeperatorLabel.Visible = true;
                }
                else
                {
                    EditHyperLink.Enabled = DeleteLinkButton.Enabled = SeperatorLabel.Enabled = false;
                }
            }
        }
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
}
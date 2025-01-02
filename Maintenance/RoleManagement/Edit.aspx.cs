using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Web;
using Dwms.Bll;

public partial class Edit : System.Web.UI.Page
{
    Guid? roleId;
    UserDb userDb = new UserDb();
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
        SetAccessControl();

        if (!String.IsNullOrEmpty(Request["roleId"]))
        {
            roleId = new Guid(Request["roleId"].ToString());
        }

        if (!IsPostBack)
        {
            RoleNameTextBox.Focus();

          

            if (roleId.HasValue)
            {
                TitleLabel.Text = "Edit Role";

                RoleDb roleDb = new RoleDb();
                RoleNameTextBox.Text = roleDb.GetRoleName(roleId.Value);

                RoleToDepartmentDb roleToDepartmentDb = new RoleToDepartmentDb();
                RoleToDepartment.RoleToDepartmentDataTable roleToDepartment = roleToDepartmentDb.GetRoleToDepartmentByRoleId(roleId.Value);
                RoleToDepartment.RoleToDepartmentRow roleToDepartmentRow = roleToDepartment[0];

                PopulateDepartment(roleToDepartmentRow.DepartmentId.ToString());
            }
            else
            {
                PopulateDepartment(string.Empty);
                DepartmentDb departmentDb = new DepartmentDb();
                Department.DepartmentDataTable department = departmentDb.GetDepartmentById(int.Parse(DepartmentRadComboBox.SelectedValue));
                Department.DepartmentRow departmentRow = department[0];
                RoleNameTextBox.Text = " - " + departmentRow.Code.Trim();
            }
        }
    }

    protected void DepartmentRadComboBox_OnSelectedIndexChanged(Object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        DepartmentDb departmentDb = new DepartmentDb();
        Department.DepartmentDataTable department = departmentDb.GetDepartmentById(int.Parse(DepartmentRadComboBox.SelectedValue));
        Department.DepartmentRow departmentRow = department[0];

        int index = RoleNameTextBox.Text.LastIndexOf("-");
        if (index > 0)
            RoleNameTextBox.Text = RoleNameTextBox.Text.Substring(0, index).Trim() + " - " + departmentRow.Code.Trim();
        else
            RoleNameTextBox.Text = " - " + departmentRow.Code.Trim();
    }

    /// <summary>
    /// Save button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string newRoleName = RoleNameTextBox.Text.Trim();
            bool success = false;
            RoleDb roleDb = new RoleDb();

            Guid newRoleId = Guid.Empty;

            RoleToDepartmentDb roleToDepartmentDb = new RoleToDepartmentDb();
            MembershipUser user = Membership.GetUser();

            if (!roleId.HasValue)
            {
                newRoleId = roleDb.Insert(newRoleName);

                AuditTrailDb auditTrailDb = new AuditTrailDb();
                Guid? operationId = auditTrailDb.Record(TableNameEnum.aspnet_Roles, roleDb.GetRoleGuid(newRoleName).ToString(), OperationTypeEnum.Insert);

                //insert into roletodepartment table
                roleToDepartmentDb.Insert(newRoleId, int.Parse(DepartmentRadComboBox.SelectedValue));

                success = true;
            }
            else
            {
                success = roleDb.Update(roleId.Value, newRoleName);
                if (success)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    Guid? operationId = auditTrailDb.Record(TableNameEnum.aspnet_Roles, roleDb.GetRoleGuid(newRoleName).ToString(), OperationTypeEnum.Update);
                }
                success = roleToDepartmentDb.UpdateDepartmetIdByRoleId(roleId.Value, int.Parse(DepartmentRadComboBox.SelectedValue));
            }

            if (success)
            {
                ConfirmPanel.Visible = true;
                FormPanel.Visible = false;
                SubmitPanel.Visible = false;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", "ResizeAndClose(" + Constants.WindowWidth + ", 190);", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "UnableToUpdateActionScript",
                    "javascript:alert('" + Constants.UnableToCreateUpdateErrorMessage + "');", true);
            }
        }
    }

    private void PopulateDepartment(string selectedDepartment)
    {
        DepartmentDb departmentDb = new DepartmentDb();

        if (hasManageDepartmentAccess)
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
        else
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartment();

        DepartmentRadComboBox.DataTextField = "Name";
        DepartmentRadComboBox.DataValueField = "Id";
        DepartmentRadComboBox.DataBind();

        if (!string.IsNullOrEmpty(selectedDepartment))
            DepartmentRadComboBox.SelectedValue = selectedDepartment;
    }

    #endregion

    #region Validation
    /// <summary>
    /// Description custom validator server validate event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void DescriptionCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string roleName = RoleNameTextBox.Text.Trim();

        if (roleId.HasValue)
        {
            RoleDb roleDb = new RoleDb();
            if (RoleNameTextBox.Text.Trim() == roleDb.GetRoleName(roleId.Value))
                args.IsValid = true;
            else
                args.IsValid = !Roles.RoleExists(roleName);
        }
        else
            args.IsValid = !Roles.RoleExists(roleName);
    }
    #endregion
    
    #region Private Methods

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasManageAllAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);
        hasManageDepartmentAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_Department);
    }

    #endregion
}

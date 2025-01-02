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

public partial class Maintenance_AccessControl_Default : System.Web.UI.Page
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
        ConfirmPanel.Visible = (Request["cfm"] == "1");

        // Set the access control for the user
        SetAccessControl();

        if (!IsPostBack)
        {
            PopulateDepartment();
            PopulateRoles();
        }
    }

    /// <summary>
    /// Save settings event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        SaveAccessControlSettings();

        Response.Redirect("Default.aspx?cfm=1&deptId=" + DepartmentRadComboBox.SelectedValue.Trim() );
    }
    
    /// <summary>
    /// Repeater item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RoleRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            // Get the controls
            HiddenField RoleIdHiddenField = e.Item.FindControl("RoleIdHiddenField") as HiddenField;

            CheckBoxList ImportRightsCheckBoxList = e.Item.FindControl("ImportRightsCheckBoxList") as CheckBoxList;
            CheckBoxList SearchRightsCheckBoxList = e.Item.FindControl("SearchRightsCheckBoxList") as CheckBoxList;
            CheckBoxList VerificationRightsCheckBoxList = e.Item.FindControl("VerificationRightsCheckBoxList") as CheckBoxList;
            CheckBoxList CompletenessRightsCheckBoxList = e.Item.FindControl("CompletenessRightsCheckBoxList") as CheckBoxList;
            CheckBoxList IncCompRightsCheckBoxList = e.Item.FindControl("IncCompRightsCheckBoxList") as CheckBoxList;
            CheckBoxList ReportsRightsCheckBoxList = e.Item.FindControl("ReportsRightsCheckBoxList") as CheckBoxList;
            CheckBoxList FileDocRightsCheckBoxList = e.Item.FindControl("FileDocRightsCheckBoxList") as CheckBoxList;
            CheckBoxList MaintenanceRightsCheckBoxList = e.Item.FindControl("MaintenanceRightsCheckBoxList") as CheckBoxList;

            AccessControlDb accessControlDb = new AccessControlDb();

            Guid roleId = new Guid(RoleIdHiddenField.Value);

            // Populate access controls
            ImportRightsCheckBoxList.DataSource = EnumManager.GetImportAccessRights();
            ImportRightsCheckBoxList.DataTextField = "Text";
            ImportRightsCheckBoxList.DataValueField = "Value";
            ImportRightsCheckBoxList.DataBind();

            SearchRightsCheckBoxList.DataSource = EnumManager.GetSearchAccessRights();
            SearchRightsCheckBoxList.DataTextField = "Text";
            SearchRightsCheckBoxList.DataValueField = "Value";
            SearchRightsCheckBoxList.DataBind();

            VerificationRightsCheckBoxList.DataSource = EnumManager.GetVerificationAccessRights();
            VerificationRightsCheckBoxList.DataTextField = "Text";
            VerificationRightsCheckBoxList.DataValueField = "Value";
            VerificationRightsCheckBoxList.DataBind();

            CompletenessRightsCheckBoxList.DataSource = EnumManager.GetCompletenessAccessRights();
            CompletenessRightsCheckBoxList.DataTextField = "Text";
            CompletenessRightsCheckBoxList.DataValueField = "Value";
            CompletenessRightsCheckBoxList.DataBind();

            IncCompRightsCheckBoxList.DataSource = EnumManager.GetIncomeComputationAccessRights();
            IncCompRightsCheckBoxList.DataTextField = "Text";
            IncCompRightsCheckBoxList.DataValueField = "Value";
            IncCompRightsCheckBoxList.DataBind();

            //ReportsRightsCheckBoxList.DataSource = EnumManager.GetReportsAccessRights();
            //ReportsRightsCheckBoxList.DataTextField = "Text";
            //ReportsRightsCheckBoxList.DataValueField = "Value";
            //ReportsRightsCheckBoxList.DataBind();

            FileDocRightsCheckBoxList.DataSource = EnumManager.GetFileDocAccessRights();
            FileDocRightsCheckBoxList.DataTextField = "Text";
            FileDocRightsCheckBoxList.DataValueField = "Value";
            FileDocRightsCheckBoxList.DataBind();

            MaintenanceRightsCheckBoxList.DataSource = EnumManager.GetMaintenanceAccessRights();
            MaintenanceRightsCheckBoxList.DataTextField = "Text";
            MaintenanceRightsCheckBoxList.DataValueField = "Value";
            MaintenanceRightsCheckBoxList.DataBind();

            //disable the manageall when 
            if (hasManageDepartmentAccess)
                MaintenanceRightsCheckBoxList.Items[0].Enabled = false;

            // Set access control settings
            SetAccessControlSettings(roleId, ModuleNameEnum.Import, ImportRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.Search, SearchRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.Verification, VerificationRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.Completeness, CompletenessRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.Income_Assessment, IncCompRightsCheckBoxList);
            //SetAccessControlSettings(roleId, ModuleNameEnum.Reports, ReportsRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.FileDoc, FileDocRightsCheckBoxList);
            SetAccessControlSettings(roleId, ModuleNameEnum.Maintenance, MaintenanceRightsCheckBoxList);
        }
    }

    protected void VerificationRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList VerificationRightsCheckBoxList = (CheckBoxList)repeater.FindControl("VerificationRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = VerificationRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("all_sets_read_only"))
        {
            VerificationRightsCheckBoxList.Items.FindByValue("All_Sets").Selected = false;
        }

        if (sel.ToLower().Equals("all_sets"))
        {
            VerificationRightsCheckBoxList.Items.FindByValue("All_Sets_Read_Only").Selected = false;
        }
    }

    protected void CompletenessRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList CompletenessRightsCheckBoxList = (CheckBoxList)repeater.FindControl("CompletenessRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = CompletenessRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("all_apps_read_only"))
        {
            CompletenessRightsCheckBoxList.Items.FindByValue("All_Apps").Selected = false;
        }

        if (sel.ToLower().Equals("all_apps"))
        {
            CompletenessRightsCheckBoxList.Items.FindByValue("All_Apps_Read_Only").Selected = false;
        }
    }


    protected void IncomComputationRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList IncCompRightsCheckBoxList = (CheckBoxList)repeater.FindControl("IncCompRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = IncCompRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("all_apps_read_only"))
        {
            IncCompRightsCheckBoxList.Items.FindByValue("All_Apps").Selected = false;
        }

        if (sel.ToLower().Equals("all_apps"))
        {
            IncCompRightsCheckBoxList.Items.FindByValue("All_Apps_Read_Only").Selected = false;
        }
    }
    

    protected void SearchRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList SearchRightsCheckBoxList = (CheckBoxList)repeater.FindControl("SearchRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = SearchRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("search_all_department_sets"))
        {
            SearchRightsCheckBoxList.Items.FindByValue("Search_All_Department_Sets_Read_Only").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_Own_Department_Sets").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_Own_Department_Sets_Read_Only").Selected = false;
        }

        if (sel.ToLower().Equals("search_all_department_sets_read_only"))
        {
            SearchRightsCheckBoxList.Items.FindByValue("Search_All_Department_Sets").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_Own_Department_Sets_Read_Only").Selected = false;
        }

        if (sel.ToLower().Equals("search_own_department_sets"))
        {
            SearchRightsCheckBoxList.Items.FindByValue("Search_All_Department_Sets").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_Own_Department_Sets_Read_Only").Selected = false;
        }

        if (sel.ToLower().Equals("search_own_department_sets_read_only"))
        {
            SearchRightsCheckBoxList.Items.FindByValue("Search_All_Department_Sets").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_All_Department_Sets_Read_Only").Selected = false;
            SearchRightsCheckBoxList.Items.FindByValue("Search_Own_Department_Sets").Selected = false;
        }
    }

    protected void MaintenanceRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList MaintenanceRightsCheckBoxList = (CheckBoxList)repeater.FindControl("MaintenanceRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = MaintenanceRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("view_only"))
            MaintenanceRightsCheckBoxList.Items.FindByValue("Manage_All").Selected = false;

        if (sel.ToLower().Equals("manage_all"))
            MaintenanceRightsCheckBoxList.Items.FindByValue("View_Only").Selected = MaintenanceRightsCheckBoxList.Items.FindByValue("Manage_Department").Selected = false;

        if (sel.ToLower().Equals("manage_department"))
            MaintenanceRightsCheckBoxList.Items.FindByValue("View_Only").Selected = MaintenanceRightsCheckBoxList.Items.FindByValue("Manage_All").Selected = false;
    }

    protected void FileDocRightsCheckBoxList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        RepeaterItem repeater = (RepeaterItem)(((Control)sender).NamingContainer);
        CheckBoxList FileDocRightsCheckBoxList = (CheckBoxList)repeater.FindControl("FileDocRightsCheckBoxList");

        string[] control = Request.Form.Get("__EVENTTARGET").Split('$');
        int idx = control.Length - 1;
        string sel = FileDocRightsCheckBoxList.Items[Int32.Parse(control[idx])].Value;

        if (sel.ToLower().Equals("view_only"))
            FileDocRightsCheckBoxList.Items.FindByValue("Download").Selected = false;

        if (sel.ToLower().Equals("download"))
            FileDocRightsCheckBoxList.Items.FindByValue("View_Only").Selected = false;
    }

    protected void DepartmentRadComboBox_OnSelectedIndexChanged(Object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        PopulateRoles();
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the roles
    /// </summary>
    //private void PopulateRoles()
    //{
    //    string[] roles = Roles.GetAllRoles();

    //    DataTable roleTable = new DataTable();
    //    roleTable.Columns.Add("Role");
    //    roleTable.Columns.Add("RoleId");

    //    foreach (string role in roles)
    //    {
    //        DataRow roleRow = roleTable.NewRow();
            
    //        // Get the role id of the role
    //        Guid? roleId = DalFunc.GetRoleId(role);

    //        if (roleId.HasValue)
    //        {
    //            roleRow["Role"] = Format.FormatEnumValue(role);
    //            roleRow["RoleId"] = roleId.Value;

    //            if (role.Equals(RoleEnum.System_Administrator.ToString()))
    //                roleTable.Rows.InsertAt(roleRow, 0);
    //            else
    //                roleTable.Rows.Add(roleRow);
    //        }
    //    }

    //    RoleRepeater.DataSource = roleTable;
    //    RoleRepeater.DataBind();
    //}

    private void PopulateRoles()
    {
        RoleDb roleDb = new RoleDb();

        RoleRepeater.DataSource = roleDb.GetRoleByDepartmentForAccessControl(int.Parse(DepartmentRadComboBox.SelectedValue), Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All));
        RoleRepeater.DataBind();
    }

    private void PopulateDepartment()
    {
        DepartmentDb departmentDb = new DepartmentDb();

        if (hasManageDepartmentAccess)
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
        else
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartment();

        DepartmentRadComboBox.DataTextField = "Name";
        DepartmentRadComboBox.DataValueField = "Id";
        DepartmentRadComboBox.DataBind();

        if (Request["deptId"] != null)
            DepartmentRadComboBox.SelectedValue = Request["deptId"].ToString();
    }
    

    /// <summary>
    /// Set the access control settings
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="module"></param>
    /// <param name="checkBoxList"></param>
    private void SetAccessControlSettings(Guid roleId, ModuleNameEnum module, CheckBoxList checkBoxList)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        foreach (ListItem checkBoxItem in checkBoxList.Items)
        {
            AccessControl.AccessControlDataTable dt = new AccessControl.AccessControlDataTable();
            dt = accessControlDb.GetAccessControl(roleId, module, checkBoxItem.Value);

            if (dt.Rows.Count > 0)
            {
                AccessControl.AccessControlRow dr = dt[0];
                checkBoxItem.Selected = dr.HasAccess;
            }
        }
    }

    /// <summary>
    /// Save the access control settings
    /// </summary>
    private void SaveAccessControlSettings()
    {
        foreach (RepeaterItem item in RoleRepeater.Items)
        {
            // Get the controls
            HiddenField RoleIdHiddenField = item.FindControl("RoleIdHiddenField") as HiddenField;

            CheckBoxList ImportRightsCheckBoxList = item.FindControl("ImportRightsCheckBoxList") as CheckBoxList;
            CheckBoxList SearchRightsCheckBoxList = item.FindControl("SearchRightsCheckBoxList") as CheckBoxList;
            CheckBoxList VerificationRightsCheckBoxList = item.FindControl("VerificationRightsCheckBoxList") as CheckBoxList;
            CheckBoxList CompletenessRightsCheckBoxList = item.FindControl("CompletenessRightsCheckBoxList") as CheckBoxList;
            CheckBoxList IncCompRightsCheckBoxList = item.FindControl("IncCompRightsCheckBoxList") as CheckBoxList;
            //CheckBoxList ReportsRightsCheckBoxList = item.FindControl("ReportsRightsCheckBoxList") as CheckBoxList;
            CheckBoxList FileDocRightsCheckBoxList = item.FindControl("FileDocRightsCheckBoxList") as CheckBoxList;
            CheckBoxList MaintenanceRightsCheckBoxList = item.FindControl("MaintenanceRightsCheckBoxList") as CheckBoxList;

            Guid roleId = new Guid(RoleIdHiddenField.Value);

            SaveSettings(roleId, ModuleNameEnum.Import, ImportRightsCheckBoxList);
            SaveSettings(roleId, ModuleNameEnum.Search, SearchRightsCheckBoxList);
            SaveSettings(roleId, ModuleNameEnum.Verification, VerificationRightsCheckBoxList);
            SaveSettings(roleId, ModuleNameEnum.Completeness, CompletenessRightsCheckBoxList);
            SaveSettings(roleId, ModuleNameEnum.Income_Assessment, IncCompRightsCheckBoxList);
            //SaveSettings(roleId, ModuleNameEnum.Reports, ReportsRightsCheckBoxList);
            SaveSettings(roleId, ModuleNameEnum.FileDoc, FileDocRightsCheckBoxList);

            RoleDb roleDb = new RoleDb();

            if (!RoleIdHiddenField.Value.ToString().Equals(roleDb.GetRoleGuid(RoleEnum.System_Administrator.ToString().Replace("_"," ")).ToString().Trim()))
                SaveSettings(roleId, ModuleNameEnum.Maintenance, MaintenanceRightsCheckBoxList);
        }
    }

    /// <summary>
    /// Save the individual settings of the check box list per module
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="module"></param>
    /// <param name="checkBoxList"></param>
    private void SaveSettings(Guid roleId, ModuleNameEnum module, CheckBoxList checkBoxList)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        foreach(ListItem checkBoxItem in checkBoxList.Items)
        {
            accessControlDb.Update(roleId, module, checkBoxItem.Value, checkBoxItem.Selected); 
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
        SubmitPanel.Visible = hasAccess;
    }
    #endregion


}
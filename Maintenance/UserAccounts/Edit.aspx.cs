using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Maintenance_UserAccounts_Edit : System.Web.UI.Page
{
    Guid? userId;

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
        if (!String.IsNullOrEmpty(Request["id"]))
        {
            userId = new Guid(Request["id"]);

            //is the current user and the user account belong to same person. redirect to listing page.
            MembershipUser user = Membership.GetUser();
            if (user == null)
                Response.Redirect("Default.aspx");

            if (userId == (Guid)user.ProviderUserKey)
            {
                Response.Redirect("Default.aspx");
            }
        }

        // Set the access control for the user
        SetAccessControl();

        if (!IsPostBack)
        {
            PopulateRoles();
            PopulateStatus();

            if (!userId.HasValue)
            {
                Name.Focus();
                PopulateTeam();
                PopulateDepartment(-1);
            }
            else
            {
                PopulateDetails();
                Nric.ReadOnly = true;
                Nric.Enabled = false;
            }
        }
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            if (userId.HasValue)// User exists
            {
                MembershipUser user = Membership.GetUser(userId.Value);
                user.Email = Email.Text.Trim();
                Membership.UpdateUser(user);

                string userName = user.UserName;

                UserDb userDb = new UserDb();
                string oldRole = userDb.GetRole(userId.Value);
                string newRole = RoleDropDownList.SelectedValue.Replace("_", " ");

                if (oldRole != newRole)
                {
                    Roles.RemoveUserFromRole(userName, oldRole);
                    Roles.AddUserToRole(userName, newRole);

                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.aspnet_UsersInRoles, userId.ToString(), OperationTypeEnum.Update);
                }

                // Update the profile of the user
                UpdateProfile(user, newRole);
                Response.Redirect("View.aspx?id=" + userId.ToString() + "&cfm=1");
            }
            else
            {
                //PassGen passGen = new PassGen();

                // ===============================================
                // Modified to allow Production users to do data migration
                // Date: 21/12/2011
                // ===============================================
                //string newPassword = passGen.GenPassWithCap(8);
                string newPassword = "111111";
                // ===============================================

                MembershipCreateStatus status;

                MembershipUser user = Membership.CreateUser(Nric.Text.Trim().ToUpper(), newPassword,
                    Email.Text.Trim(), null, null, true, out status);

                if (status != MembershipCreateStatus.Success)
                {
                    switch (status)
                    {
                        case MembershipCreateStatus.ProviderError:
                            InfoLabel.Text = "The authentication provider returned an error. " +
                                "Please verify your entry and try again. " +
                                "If the problem persists, please contact the system administrator.";
                            InfoLabel.Visible = true;
                            break;
                            
                        case MembershipCreateStatus.UserRejected:
                            InfoLabel.Text = "The user creation request has been canceled. " +
                                "Please verify your entry and try again. " +
                                "If the problem persists, please contact the system administrator.";
                            InfoLabel.Visible = true;
                            break;
                        case MembershipCreateStatus.DuplicateUserName:
                            InfoLabel.Text = "The user with same staff Id already exist. " +
                                "Please verify your entry and try again. " +
                                "If the problem persists, please contact the system administrator.";
                            InfoLabel.Visible = true;
                            break;
                        default:
                            InfoLabel.Text = "An unknown error occurred. " +
                                "Please verify your entry and try again. " +
                                "If the problem persists, please contact the system administrator.";
                            InfoLabel.Visible = true;
                            break;
                    }
                }

                if (status == MembershipCreateStatus.Success)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.aspnet_Membership, ((Guid)user.ProviderUserKey).ToString(), OperationTypeEnum.Insert);

                    UpdateProfile(user, RoleDropDownList.SelectedValue.Replace(" ", "_"));

                    Roles.AddUserToRole(user.UserName.Trim().ToUpper(), RoleDropDownList.SelectedValue);
                    auditTrailDb.Record(TableNameEnum.aspnet_UsersInRoles,  ((Guid)user.ProviderUserKey).ToString(), OperationTypeEnum.Insert);

                    Response.Redirect("View.aspx?id=" + user.ProviderUserKey.ToString() + "&cfm=1");
                }
            }
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// Validate email
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void ValidateEmail(object source, ServerValidateEventArgs args)
    {
        if (EmailRequiredValidator.IsValid)
        {
            string email = Email.Text.Trim();

            if (Validation.IsEmail(email))
            {
                string oldUserName = string.Empty;
                string newUserName = Membership.GetUserNameByEmail(email);

                if (userId.HasValue)
                {
                    MembershipUser user = Membership.GetUser(userId.Value);

                    if (user != null)
                        oldUserName = user.UserName;
                }

                if (string.IsNullOrEmpty(newUserName))
                {
                    args.IsValid = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(oldUserName)) // Existing user
                        if (!oldUserName.ToUpper().Equals(newUserName.ToUpper()))
                            args.IsValid = false;
                        else
                            args.IsValid = true;
                    else // New users
                        args.IsValid = false;
                }

                (source as CustomValidator).ErrorMessage = "<br />An account for the email already exists. Please use another one.";
            }
            else
            {
                (source as CustomValidator).ErrorMessage = "<br />Email is incorrect, it should be in a format like \"name@domain.com\".";
                args.IsValid = false;
            }
        }
    }

    /// <summary>
    /// Validate nric
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void ValidateNric(object source, ServerValidateEventArgs args)
    {
        if (RequiredFieldValidator7.IsValid)
        {
            string nric = Nric.Text.Trim();

            //if (Validation.IsNric(nric))
            //{
                MembershipUser user = Membership.GetUser(nric);

                if (user == null) //New User
                {
                    if (Membership.GetUser(nric) != null)
                        args.IsValid = false;
                    else
                        args.IsValid = true;
                }
                else//Existing User
                {
                    args.IsValid = true;
                }
            //}
            //else
            //    args.IsValid = false;
        }
    }
    #endregion  

    #region Private Methods
    /// <summary>
    /// Populate roles
    /// </summary>
    private void PopulateRoles()
    {
        MembershipUser user = Membership.GetUser();
        DepartmentDb departmentDb = new DepartmentDb();
        Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)user.ProviderUserKey);
        Department.DepartmentRow departmentRow = departments[0];

        RoleDb roleDb = new RoleDb();
        RoleDropDownList.DataSource = roleDb.GetRoleByDepartment(departmentRow.Id, Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All));
        RoleDropDownList.DataTextField = "RoleName";
        RoleDropDownList.DataValueField = "RoleName";
        RoleDropDownList.DataBind();
    }

    private void PopulateStatus()
    {
        StatusRadioButtonList.DataSource = Enum.GetValues(typeof(UserActiveStatusEnum));
        StatusRadioButtonList.DataBind();
        StatusRadioButtonList.SelectedIndex = 0;
    }

    /// <summary>
    /// Populate Team
    /// </summary>
    private void PopulateTeam()
    {
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        TeamRadComboBox.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Teams.ToString());
        TeamRadComboBox.DataTextField = "Name";
        TeamRadComboBox.DataValueField = "Name";
        TeamRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate sections
    /// </summary>
    private void PopulateSections()
    {
        SectionDb sectionDb = new SectionDb();
        SectionRadComboBox.DataSource = sectionDb.GetSectionByDepartment(int.Parse(DepartmentRadComboBox.SelectedValue));
        SectionRadComboBox.DataTextField = "Name";
        SectionRadComboBox.DataValueField = "Id";
        SectionRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate department
    /// </summary>
    private void PopulateDepartment(int UserGroupId)
    {
        DepartmentDb departmentDb = new DepartmentDb();

        if (hasManageDepartmentAccess)
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
        else
            DepartmentRadComboBox.DataSource = departmentDb.GetDepartment();

        DepartmentRadComboBox.DataTextField = "Name";
        DepartmentRadComboBox.DataValueField = "Id";
        DepartmentRadComboBox.DataBind();

        if (UserGroupId != -1)
            DepartmentRadComboBox.SelectedValue = UserGroupId.ToString();

        PopulateSections();
    }
    
    /// <summary>
    /// Populate user details
    /// </summary>
    private void PopulateDetails()
    {
        MembershipUser user = Membership.GetUser(userId.Value);

        if (user != null)
        {            
            //Nric.ReadOnly = true;
            //Nric.Enabled = false;

            DataTable dtProfile = new DataTable();
            dtProfile = ProfileDb.GetProfileInfo(userId.Value);

            UserDb userDb = new UserDb();
            RoleDropDownList.SelectedValue = userDb.GetRole(userId.Value);
            Nric.Text = user.UserName;
            Email.Text = user.Email;
            TitleLabel.Text = "User Accounts - " + dtProfile.Rows[0]["name"].ToString();
            Name.Text = dtProfile.Rows[0]["name"].ToString();
            Designation.Text = string.IsNullOrEmpty(dtProfile.Rows[0]["Designation"].ToString()) ? "N.A." : dtProfile.Rows[0]["Designation"].ToString();
            PopulateTeam();
            PopulateDepartment(int.Parse(dtProfile.Rows[0]["departmentId"].ToString()));
            SectionRadComboBox.SelectedValue = dtProfile.Rows[0]["sectionId"].ToString();
            TeamRadComboBox.SelectedValue = dtProfile.Rows[0]["TeamName"].ToString();
            StatusRadioButtonList.SelectedValue = Boolean.Parse(dtProfile.Rows[0]["IsApproved"].ToString()) ? UserActiveStatusEnum.Active.ToString() : UserActiveStatusEnum.Inactive.ToString();

            if (user.UserName.ToLower().Equals("system"))
            {
                RoleDropDownList.Enabled = false;
                Nric.Enabled = false;
                Email.Enabled = false;                
                Name.Enabled = false;
                Designation.Enabled = false;
                DepartmentRadComboBox.Enabled = false;
                SectionRadComboBox.Enabled = false;
                TeamRadComboBox.Enabled = false;
                StatusRadioButtonList.Enabled = false;

                SubmitButton.Enabled = false;
            }
        }        
    }

    protected void DepartmentRadComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Populate sections
        PopulateSections();
        PopulateDepartment(int.Parse(DepartmentRadComboBox.SelectedValue));
    }    

    /// <summary>
    /// Update the profile record
    /// </summary>
    /// <param name="user"></param>
    /// <param name="newRole"></param>
    private void UpdateProfile(MembershipUser user, string newRole)
    {
        ProfileDb profileDb = new ProfileDb();
        ProfileCustom userProfile = profileDb.GetProfile(user.UserName);

        UserDb userDb = new UserDb();
        int oldSectionId = userDb.GetSection(userProfile.UserId);

        if (userProfile == null)
            Response.Redirect("Default.aspx");

        userProfile.FullName = Format.ToTitleCase(Name.Text.Trim());
        userProfile.Email = Email.Text.Trim();
        userProfile.Designation = Designation.Text.Trim();
        userProfile.Section = int.Parse(SectionRadComboBox.SelectedValue);
        userProfile.Team = TeamRadComboBox.SelectedValue;

        userProfile.Save();
        
        //update status (isApproved status)

        userDb.UpdateIsApprovedStatus(userProfile.UserId, StatusRadioButtonList.SelectedValue.ToLower().Trim().Equals("active") ? true : false);

        int newSectionId = int.Parse(SectionRadComboBox.SelectedValue);

        //if the section is changed, update all the docset/application status to "New" which are assigned to the user and NOT verified/confirmed.
        if (oldSectionId != newSectionId)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocAppDb docAppDb = new DocAppDb();

            docSetDb.UpdateFromUserSectionChange(userProfile.UserId, oldSectionId, newSectionId);
            docAppDb.UpdateFromUserSectionChange(userProfile.UserId, oldSectionId, newSectionId);
        }
    }

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasManageAllAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);
        hasManageDepartmentAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_Department);

        // Set the visibility of the buttons
        SubmitPanel.Visible = (hasManageAllAccess || hasManageDepartmentAccess);
    }
    #endregion
}
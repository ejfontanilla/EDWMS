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
using Dwms.Bll;
using Dwms.Web;

public partial class Maintenance_UserAccounts_View : System.Web.UI.Page
{
    Guid? userId;
    bool isAdmin = false;             

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
            if (Validation.IsGuid(Request["id"]))
            {
                userId = new Guid(Request["id"]);
            }                
        }

        if (!String.IsNullOrEmpty(Request["admin"]))
        {
            if (Request["admin"] == "1")
            {
                isAdmin = true;
            }
        }

        //is the current user and the user account belong to same person. hide the edit button.
        MembershipUser user = Membership.GetUser();
        if (user == null)
            Response.Redirect("Default.aspx");

        if (userId == (Guid)user.ProviderUserKey)
        {
            EditButton.Visible = false;
        }

        if(!userId.HasValue)
            Response.Redirect("Default.aspx");

        ConfirmPanel.Visible = (Request["cfm"] == "1");
        WarningPanel.Visible = (Request["cfm"] == "0");

        EditButton.Attributes["onclick"] = "location.href='Edit.aspx?id=" + Request["id"] + "'";

        PopulateFields();

        // Set access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Delete event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Delete(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();

        if (userId.HasValue)
        {
            bool success = false;
            
            //if (isAdmin)
            //    success = userDb.Delete(userId.Value);
            //else
            //    success = userDb.UpdateIsApprovedStatus(userId.Value, false);

            success = userDb.Delete(userId.Value);

            if (success)
                Response.Redirect("Default.aspx");
            else
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "DeleteFailScript",
                    "alert('" + Constants.UnableToDeleteUserErrorMessage + "');", true);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate data
    /// </summary>
    private void PopulateFields()
    {
        if (userId.HasValue)
        {
            MembershipUser user = Membership.GetUser(userId.Value);

            if (user != null)
            {

                DataTable dtProfile = new DataTable();
                dtProfile = ProfileDb.GetProfileInfo(userId.Value);

                if (dtProfile.Rows.Count >0)
                {
                    UserDb userDb = new UserDb();
                    Role.Text = Format.FormatEnumValue(userDb.GetRole(userId.Value));
                    Nric.Text = user.UserName;
                    Email.Text = user.Email;
                    TitleLabel.Text = "User Accounts - " + dtProfile.Rows[0]["Name"].ToString();
                    Name.Text = dtProfile.Rows[0]["name"].ToString();
                    Designation.Text = string.IsNullOrEmpty(dtProfile.Rows[0]["Designation"].ToString()) ? "N.A." : dtProfile.Rows[0]["Designation"].ToString();
                    SectionLabel.Text = dtProfile.Rows[0]["SectionName"].ToString() + " - " + dtProfile.Rows[0]["SectionCode"].ToString();
                    TeamLabel.Text = dtProfile.Rows[0]["TeamName"].ToString();
                    GroupLabel.Text = dtProfile.Rows[0]["DepartmentName"].ToString() + " - " + dtProfile.Rows[0]["DepartmentCode"].ToString();
                    BusinessCodeLabel.Text = dtProfile.Rows[0]["BusinessCode"].ToString();
                    Status.Text = dtProfile.Rows[0]["IsApproved"].ToString().ToLower().Equals("true") ? UserActiveStatusEnum.Active.ToString() : UserActiveStatusEnum.Inactive.ToString();

                    if (user.UserName.ToLower().Equals("system"))
                    {
                        EditButton.Disabled = true;
                        DeleteButton.Enabled = false;
                    }
                }
                else
                    Response.Redirect("~/Default.aspx");
            }
            else
                Response.Redirect("Default.aspx");
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


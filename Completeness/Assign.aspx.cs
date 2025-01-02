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

public partial class Verification_AssignApp : System.Web.UI.Page
{
    int[] ids;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the list of App ids
        if (Request["id"] != null)
        {
            string[] idStrArray = Request["id"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ids = new int[idStrArray.Length];
            int count = 0;
            foreach (string idStr in idStrArray)
            {
                ids[count] = int.Parse(idStr);
                count++;
            }
        }

        if (!IsPostBack)
        {
            if (ids != null && ids.Length > 0)
            {
                PopulateApps();
                PopulateUsers();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    "javascript:CloseWindow();", true);
            }
        }
    }

    /// <summary>
    /// Assign button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AssignButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            DocAppDb docAppDb = new DocAppDb();

            Guid userId = new Guid(UserRadComboBox.SelectedValue);

            foreach (int id in ids)
            {
                docAppDb.AssignUserAsCompletenessOfficer(id, userId);

                //reset SendToCdbFlags
                docAppDb.ResetSendToCdbFlags(id);
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(700, 200);", true);
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

    #endregion

    #region Validation
    /// <summary>
    /// User custom valivador
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void UserCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        string userIdStr = UserRadComboBox.SelectedValue;
        args.IsValid = (!userIdStr.Equals("-1") && !string.IsNullOrEmpty(userIdStr.Trim()));
    }
    #endregion  

    #region Private Methods
    /// <summary>
    /// Populate the Apps
    /// </summary>
    private void PopulateApps()
    {
        if (ids != null && ids.Length > 0)
        {
            //NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to assign:", ids.Length);

            DocAppDb docAppDb = new DocAppDb();
            AppRepeater.DataSource = docAppDb.GetDocAppsByIdsForAssignment(ids);
            AppRepeater.DataBind();

            NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to assign:", AppRepeater.Items.Count);
        }
    }

    /// <summary>
    /// Populate users drop down list
    /// </summary>
    private void PopulateUsers()
    {
        UserRadComboBox.DataSourceID = "GetUserObjectDataSource";
        UserRadComboBox.DataTextField = "Name";
        UserRadComboBox.DataValueField = "UserId";
    }
    #endregion
}
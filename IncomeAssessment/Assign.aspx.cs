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

public partial class IncomeAssessment_AssignApp : System.Web.UI.Page
{
    int[] intArrIdsToAllow;
    int[] intArrIdsNotAllow;

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

            string strIdsAllow = string.Empty;
            string strIdsNotAllow = string.Empty;

            foreach (string idStr in idStrArray)
            {
                if (AllowToAssign(int.Parse(idStr)))
                    strIdsAllow = strIdsAllow + (string.IsNullOrEmpty(strIdsAllow) ? idStr : "," + idStr);
                else
                    strIdsNotAllow = strIdsNotAllow + (string.IsNullOrEmpty(strIdsNotAllow) ? idStr : "," + idStr);
            }

            string[] arrIdsAllow = strIdsAllow.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            intArrIdsToAllow = new int[arrIdsAllow.Length];
            int countAllow = 0;
            foreach (string idStr in arrIdsAllow)
            {
                    intArrIdsToAllow[countAllow] = int.Parse(idStr);
                    countAllow++;
            }

            string[] arrIdsNotAllow = strIdsNotAllow.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            intArrIdsNotAllow = new int[arrIdsNotAllow.Length];
            int countNotAllow = 0;
            foreach (string idStr in arrIdsNotAllow)
            {
                intArrIdsNotAllow[countNotAllow] = int.Parse(idStr);
                countNotAllow++;
            }
        }

        if (!IsPostBack)
        {
            if (intArrIdsToAllow != null && intArrIdsToAllow.Length > 0)
            {
                PopulateApps();
                PopulateNotApps();
                PopulateUsers();
            }
            else
            {
                PopulateNotApps();
                UserRadComboBox.Enabled = false;
                AssignButton.Enabled = false;
                tblAllow.Visible = false;
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

            foreach (int id in intArrIdsToAllow)
            {
                docAppDb.AssignUserAsAssessmentOfficer(id, userId);

                //reset SendToCdbFlags
                //docAppDb.ResetSendToCdbFlags(id);
                docAppDb.ResetSendToLeasFlags(id);      //Added By Edward 24/02/2014 Add Icon and Action Log
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
        if (intArrIdsToAllow != null && intArrIdsToAllow.Length > 0)
        {
            //NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to assign:", ids.Length);

            DocAppDb docAppDb = new DocAppDb();
            AppRepeater.DataSource = docAppDb.GetDocAppsByIdsForAssignment(intArrIdsToAllow);
            AppRepeater.DataBind();

            NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to assign:", AppRepeater.Items.Count);
        }
    }


    private void PopulateNotApps()
    {
        if (intArrIdsNotAllow != null && intArrIdsNotAllow.Length > 0)
        {
            NotAllowedeLabel.Visible = true;
            tblNotAllow.Visible = true;

            DocAppDb docAppDb = new DocAppDb();
            NotAppRepeater.DataSource = docAppDb.GetDocAppsByIdsForAssignment(intArrIdsNotAllow);
            NotAppRepeater.DataBind();

            NotNoOfAppsLabel.Text = string.Format("Apps not allowed:", AppRepeater.Items.Count);
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

    /// <summary>
    /// Do not allow to assign when HLEStatus is cancelled, approved, expired or rejected
    /// </summary>
    private bool AllowToAssign(int id)
    {
        string status = HleInterfaceDb.GetHleStatusByDocAppId(id);
        if (status.Equals("Rejected") || status.Equals("Approved") || status.Equals("Cancelled") || status.Equals("Expired"))
            return false;
        else
            return true;
    }
    #endregion
}
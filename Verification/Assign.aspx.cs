using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Verification_AssignSet : System.Web.UI.Page
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
        // Get the list of set ids
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
            //check for access rights
            if (ids != null && ids.Length ==1)
            {
                AccessControlDb accessControlDb = new AccessControlDb();
                List<string> accessControlList = new List<string>();
                accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Verification);

                //if not supervisor then check the access rights
                if (!accessControlList.Contains(AccessControlSettingEnum.All_Sets.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()))
                {
                    DocSetDb docSetDb = new DocSetDb();
                    if (!docSetDb.AllowVerificationSaveDate(int.Parse(ids[0].ToString())))
                        Util.ShowUnauthorizedMessage();
                }
            }

            if (ids != null && ids.Length > 0)
            {
                PopulateSets();
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
    protected void AssignButtonOLD_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocAppDb docAppDb = new DocAppDb();
            Guid userId = new Guid(UserRadComboBox.SelectedValue);

            foreach (int id in ids)
            {
                docSetDb.AssignUserAsVerificationOfficer(id, userId);

                //reset SendToCdbFlags
                docSetDb.ResetSendToCdbFlags(id);

                docAppDb.UpdateDocAppStatusOnDocSetAssign(id);
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(550, 200);", true);
        }
    }

    protected void AssignButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {

            Guid userId = new Guid(UserRadComboBox.SelectedValue);

            foreach (int id in ids)
            {
                DocSetDb.AssignDocSetToUser(id, userId);
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(550, 200);", true);
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
        args.IsValid =  (!userIdStr.Equals("-1") && !string.IsNullOrEmpty(userIdStr.Trim()));
    }


    #endregion  

    #region Private Methods
    /// <summary>
    /// Populate the sets
    /// </summary>
    private void PopulateSets()
    {
        if (ids != null && ids.Length > 0)
        {
            //NoOfSetsLabel.Text = string.Format("You have chosen {0} set(s) to assign:", ids.Length);

            #region Added By Edward 2017/11/03 To Optimize Assign might reduce OOM (improved from 20 to 1sec)
            //DocSetDb docSetDb = new DocSetDb();
            //SetRepeater.DataSource = docSetDb.GetMultipleDocSets(ids);
            //SetRepeater.DataBind();

            SetRepeater.DataSource = DocSetDb.GetDocSetsToAssign(Request["id"].ToString());
            SetRepeater.DataBind();
            #endregion

            NoOfSetsLabel.Text = string.Format("You have chosen {0} set(s) to assign:", SetRepeater.Items.Count);
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
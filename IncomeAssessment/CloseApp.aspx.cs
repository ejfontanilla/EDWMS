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

public partial class Completeness_CloseApp : System.Web.UI.Page
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
            if (ids != null && ids.Length == 1)
            {
                AccessControlDb accessControlDb = new AccessControlDb();
                List<string> accessControlList = new List<string>();
                accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness);

                //if not supervisor then check the access rights
                if (!accessControlList.Contains(AccessControlSettingEnum.All_Apps.ToString().ToLower()))
                {
                    DocAppDb docAppDb = new DocAppDb();
                    if (!docAppDb.AllowCompletenessSaveDate(int.Parse(ids[0].ToString())))
                        Util.ShowUnauthorizedMessage();
                }
            }

            if (ids != null && ids.Length > 0)
            {
                PopulateApps();
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
    protected void CloseButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            DocAppDb docAppDb = new DocAppDb();

            foreach (int id in ids)
            {
                Boolean success = false;
                success = docAppDb.UpdateRefStatusIA(id, AssessmentStatusEnum.Closed, true, false, LogActionEnum.Application_closed);

                //update completness status to AppStatusEnum.Pending_Documents if there is only one set for the application.
                //if (success)
                //{
                //    DocAppDb docAppDb = new DocAppDb();
                //    docAppDb.UpdateDocAppStatusOnDocSetClosed(id);
                    docAppDb.ResetDateOut(id);
                //}
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(550, 200);", true);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the sets
    /// </summary>
    private void PopulateApps()
    {
        if (ids != null && ids.Length > 0)
        {
            //NoOfSetsLabel.Text = string.Format("You have chosen {0} set(s) to assign:", ids.Length);

            DocAppDb docAppDb = new DocAppDb();
            AppRepeater.DataSource = docAppDb.GetMultipleDocApp(ids);
            AppRepeater.DataBind();

            NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to close:", AppRepeater.Items.Count);
        }
    }
    #endregion
}
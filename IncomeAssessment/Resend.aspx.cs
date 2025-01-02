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

public partial class IncomeAssessment_Resend : System.Web.UI.Page
{
    int[] intArrIdsToAllow;


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

            foreach (string idStr in idStrArray)
            {
                if (AllowToAssign(int.Parse(idStr)))
                {
                    strIdsAllow = strIdsAllow + (string.IsNullOrEmpty(strIdsAllow) ? idStr : "," + idStr);
                }
            }

            string[] arrIdsAllow = strIdsAllow.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            intArrIdsToAllow = new int[arrIdsAllow.Length];
            int countAllow = 0;
            foreach (string idStr in arrIdsAllow)
            {
                intArrIdsToAllow[countAllow] = int.Parse(idStr);
                countAllow++;
            }
            
        }

        if (!IsPostBack)
        {
            if (intArrIdsToAllow != null && intArrIdsToAllow.Length > 0)
            {
                PopulateApps();
            }
            else
            {
                NoOfAppsLabel.Text = string.Format("There are no application (with status Extracted) to resend Extraction Worksheet:", AppRepeater.Items.Count);
                AssignButton.Enabled = false;
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
            string refType = string.Empty;
            foreach (int id in intArrIdsToAllow)
            {
                DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(id);
                if (docApps.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppRow = docApps[0];
                    refType = docAppRow.RefType.ToUpper().Trim();
                }

                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    docAppDb.UpdateSendToLeasStatus(id, SendToLEASStatusEnum.Ready);
                    docAppDb.UpdateSendToLeasAttemptCount(id, 0);
                    #region Added By Edward 24/02/2014  Add Icon and Action Log
                    IncomeDb.InsertExtractionLog(id, string.Empty, string.Empty,
                        LogActionEnum.Resend_Application, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                    #endregion
                }
                else if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))     //Added By Edward Changes for Sales and Resale 2014/5/26
                {
                    //docAppDb.UpdateSendToResaleStatus(id, SendToLEASStatusEnum.Ready);
                    //docAppDb.UpdateSendToResaleAttemptCount(id, 0);
                    //IncomeDb.InsertExtractionLog(id, string.Empty, string.Empty,
                    //    LogActionEnum.Resend_Application, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                }
            
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

            NoOfAppsLabel.Text = string.Format("You have chosen {0} app(s) to resend Extraction Worksheet:", AppRepeater.Items.Count);
        }
    }

    private bool AllowToAssign(int id)
    {
        DocAppDb db = new DocAppDb();
        DataTable dt = db.GetDocAppById(id);
        if (dt.Rows.Count > 0)
        {
            string status = dt.Rows[0]["AssessmentStatus"].ToString();
            if (status.Trim().ToUpper().Equals(AssessmentStatusEnum.Extracted.ToString().ToUpper()))
                return true;
            else
                return false;
        }
        else
            return false;
    }
    #endregion
}
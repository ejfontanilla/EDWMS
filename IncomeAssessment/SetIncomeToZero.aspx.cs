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

public partial class IncomeAssessment_SetIncomeToZero : System.Web.UI.Page
{
    int appPersonalId;
    int docAppId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            appPersonalId = int.Parse(Request["id"]);
            docAppId = int.Parse(Request["docappid"]);       
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        #region Modified by edward changes from 20140604 Meeting : no more popup for setting zero
        IncomeDb.SetIncomeToZero(appPersonalId, docAppId);
        //lblConfirm.Text = "Set Income to Zero is successful.";
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    string.Format("javascript:ResizeAndClose(300, 120);"), true);
        #endregion
    }

    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            try
            {

                DataTable dt = IncomeDb.GetIncomeDataByAppPersonalId(appPersonalId);
                if (dt.Rows.Count > 0)
                {
                    string strIncomeComponent = "Income";
                    string strIncomeType = "Gross Income";
                    decimal decAmount = 0;
                    foreach (DataRow row in dt.Rows)
                    {                         
                        int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(row["Id"].ToString()), 1, (Guid)Membership.GetUser().ProviderUserKey);
                        int result = IncomeDb.InsertIncomeDetails(NewVersionId, strIncomeComponent, decAmount, false, false, true, false, false);
                        int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(row["Id"].ToString()), NewVersionId);
                    }
                    CreditAssessmentDb CAdb = new CreditAssessmentDb();
                    CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, strIncomeComponent, strIncomeType);
                    if (CADt.Rows.Count > 0)
                    {
                        CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                        CAdb.Update(appPersonalId, decAmount, (Guid)Membership.GetUser().ProviderUserKey,true);
                    }
                    else
                        CAdb.Insert(appPersonalId, strIncomeComponent, strIncomeType, decAmount, (Guid)Membership.GetUser().ProviderUserKey, true);                                        

                    #region Added By Edward 24/02/2014  Add Icon and Action Log
                    IncomeDb.InsertExtractionLog(docAppId, string.Empty, string.Empty,
                        LogActionEnum.Set_Income_To_Zero, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                    #endregion

                    ConfirmPanel.Visible = true;
                    FormPanel.Visible = false;
                }
                else
                {
                    InfoLabel.Text = "No Income Months. ";
                    WarningPanel.Visible = true;
                    FormPanel.Visible = false;
                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    string.Format("javascript:ResizeAndClose(400, 200);"), true);

            }
            catch (Exception ex)
            {
                InfoLabel.Text = ex.Message;
                WarningPanel.Visible = true;
                FormPanel.Visible = false;
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
    string.Format("javascript:ResizeAndClose(400, 200);"), true);
            }

        }
    }
}
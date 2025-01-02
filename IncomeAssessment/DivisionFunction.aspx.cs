using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class IncomeAssessment_DivisionFunction : System.Web.UI.Page
{
    int? docAppId;
    string nric;
    int? IncomeId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["docAppId"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            docAppId = int.Parse(Request["docAppId"]);
            nric = Request["nric"].ToString();
            IncomeId = int.Parse(Request["incomeId"]);
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        NoOfSetsLabel.Text = string.Format("Do you want to divide the income computation to all Months? (Please save all changes before confirming.)");
    }
    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        DataTable dtAllIncome = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, nric);
        DataTable dtIncome = IncomeDb.GetIncomeDataById(IncomeId.Value);
        DataTable dtDetails = IncomeDb.GetIncomeDetailsByIncomeVersion(int.Parse(dtIncome.Rows[0]["IncomeVersionId"].ToString()));

        foreach (DataRow rowAllIncome in dtAllIncome.Rows)
        {
            DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(rowAllIncome["Id"].ToString()));
            int VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;
            int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(rowAllIncome["Id"].ToString()), VersionNo, (Guid)Membership.GetUser().ProviderUserKey);

            foreach (DataRow rowDetails in dtDetails.Rows)
            {
                decimal NewIncomeAmount = Decimal.Parse(rowDetails["IncomeAmount"].ToString()) / dtAllIncome.Rows.Count;

                int result = IncomeDb.InsertIncomeDetails(NewVersionId, rowDetails["IncomeItem"].ToString(), NewIncomeAmount, 
                   bool.Parse(rowDetails["CAIncome"].ToString()), bool.Parse(rowDetails["CPFIncome"].ToString()), 
                   bool.Parse(rowDetails["HouseholdIncome"].ToString()), bool.Parse(rowDetails["AHGIncome"].ToString()), bool.Parse(rowDetails["Allowance"].ToString()));
            }
            int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(rowAllIncome["Id"].ToString()), NewVersionId);
        }
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:ResizeAndClose(550, 200);", true);
    }

 
}
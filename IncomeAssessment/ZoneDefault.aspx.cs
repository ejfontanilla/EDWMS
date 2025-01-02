using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class IncomeAssessment_ZoneDefault : System.Web.UI.Page
{
    int? ReqDocAppId;
    string ReqNRIC;
    int? ReqAppDocRefId;
    int? ReqAppPersonalId;
    int? ReqIncomeId;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            ReqDocAppId = int.Parse(Request["id"]);
            ReqNRIC = Request["nric"].ToString();
            ReqAppDocRefId = int.Parse(Request["appdocref"]);
            Session["ReqIncomeId"] = int.Parse(Request["incomeid"]);
            ReqIncomeId = int.Parse(Request["incomeid"]);
            ReqAppPersonalId = int.Parse(Request["apppersonal"]);
            Response.Redirect(string.Format("~/IncomeAssessment/ZoningPage.aspx?id={0}&nric={1}&appdocref={2}&apppersonal={3}&incomeid={4}",
                ReqDocAppId.Value, ReqNRIC, ReqAppDocRefId.Value, ReqAppPersonalId.Value,ReqIncomeId.Value));
        }
        else
            Response.Redirect("~/IncomeAssessment/");
    }
}
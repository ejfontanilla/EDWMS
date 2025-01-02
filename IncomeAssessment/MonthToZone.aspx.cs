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


public partial class IncomeAssessment_MonthToZone : System.Web.UI.Page
{
    int? ReqDocAppId;
    string ReqNRIC;
    int? ReqAppDocRefId;
    int? ReqAppPersonalId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["docappid"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            ReqDocAppId = int.Parse(Request["docappid"].ToString());
            ReqNRIC = Request["nric"].ToString();
            ReqAppDocRefId = int.Parse(Request["appdocrefid"].ToString());
            ReqAppPersonalId = int.Parse(Request["apppersonal"]);
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        if (!IsPostBack)
            PopulateMonthYear();

        #region Modified by edward changes from 20140604 Meeting no more popup for zone
        string str = string.Format("window.open('ZoneDefault.aspx?id={0}&nric={1}&appdocref={2}&incomeid={3}&apppersonal={4}')",
            ReqDocAppId.Value, ReqNRIC, ReqAppDocRefId.Value, MonthYearDropDownList.SelectedValue, ReqAppPersonalId.Value);       
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
            string.Format("javascript:{0};GetRadWindow().close();", str), true);
        #endregion
    }


    private void PopulateMonthYear()
    {
        try
        {
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    RadComboBoxItem item = new RadComboBoxItem(row["MonthYear"].ToString(), row["Id"].ToString());
                    int noOfNotAcceptedDocPerIncome = IncomeDb.GetNoofIncomeDetailsPerIncome(int.Parse(row["Id"].ToString()));
                    if (noOfNotAcceptedDocPerIncome > 0)
                        item.ImageUrl = "~/Data/Images/Green_Check.png";
                    //if (int.Parse(row["IncomeVersionId"].ToString()) > 0)
                    //    item.ImageUrl = "~/Data/Images/Green_Check.png";                    
                    MonthYearDropDownList.Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }


    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            try
            {

                #region Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                //string str = string.Format("window.open('ZoningPage.aspx?id={0}&nric={1}&appdocref={2}&incomeid={3}&apppersonal={4}')",
                //    ReqDocAppId.Value,ReqNRIC,ReqAppDocRefId.Value,MonthYearDropDownList.SelectedValue, ReqAppPersonalId.Value);
                string str = string.Format("window.open('ZoneDefault.aspx?id={0}&nric={1}&appdocref={2}&incomeid={3}&apppersonal={4}')",
                    ReqDocAppId.Value, ReqNRIC, ReqAppDocRefId.Value, MonthYearDropDownList.SelectedValue, ReqAppPersonalId.Value);
                #endregion

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    string.Format("javascript:{0};GetRadWindow().close();", str), true);

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
            }

        }
    }

}
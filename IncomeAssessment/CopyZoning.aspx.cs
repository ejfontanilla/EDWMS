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
using System.Text;

public partial class IncomeAssessment_CopyZoning : System.Web.UI.Page
{

    int? ReqDocAppId;
    string ReqNRIC;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["docAppId"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            ReqDocAppId = int.Parse(Request["docAppId"]);
            ReqNRIC = Request["nric"].ToString();
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        if (!IsPostBack)
            PopulateCheckBoxList();
    }

    protected void valMonths_ServerValidation(object source, ServerValidateEventArgs args)
    {
        args.IsValid = chkboxMonths.SelectedItem != null;
    }

    private void PopulateCheckBoxList()
    {
        try
        {
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem(row["MonthYear"].ToString(), row["Id"].ToString());
                    item.Selected = true;   //Modified By Edward 26/2/2014 February 21, 2014 Meeting
                    chkboxMonths.Items.Add(item);

                    //int noOfNotAcceptedDocPerIncome = IncomeDb.GetNoofIncomeDetailsPerIncome(int.Parse(row["Id"].ToString()));
                    //if (noOfNotAcceptedDocPerIncome > 0)
                    //    item.ImageUrl = "~/Data/Images/Green_Check.png";
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
                MembershipUser user = Membership.GetUser();
                Guid currentUserId = (Guid)user.ProviderUserKey;

                StringBuilder strBuild = new StringBuilder();
                StringBuilder strBuild2 = new StringBuilder();      //Added BY Edward 24/02/2014 Add Icon and Action Log
                

                foreach (ListItem item in chkboxMonths.Items)
                {
                    if (item.Selected == true)
                    {
                        strBuild.Append(item.Value + ";");
                        strBuild2.Append(item.Text + ";");          //Added BY Edward 24/02/2014 Add Icon and Action Log
                    }
                }
                Session["Copy"] = strBuild.ToString().Substring(0,strBuild.ToString().Length-1);
                Session["IncomeMonthYear"] = strBuild2.ToString().Substring(0, strBuild2.ToString().Length - 1);     //Added BY Edward 24/02/2014 Add Icon and Action Log
                Session["Figures"] = radIncludeAmount.SelectedValue == "y" ? "Y" : "N";   
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    "javascript:CloseWindow('copy');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
            }

        }
    }


    protected void radSelectAll_SelectedIndexChanged(object sender, EventArgs e)
    {
        RadioButtonList yes = (RadioButtonList)sender;
        if (yes.SelectedValue == "y")
        {
            foreach (ListItem item in chkboxMonths.Items)
            {
                item.Selected = true;
            }
        }
        else
        {
            foreach (ListItem item in chkboxMonths.Items)
            {
                item.Selected = false;
            }
        }
    }
    protected void radIncludeAmount_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void radSelectAll_TextChanged(object sender, EventArgs e)
    {

    }
}
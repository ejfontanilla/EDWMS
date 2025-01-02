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

public partial class Controls_CopyItems : System.Web.UI.UserControl
{
    int? docAppid;
    
    public int DocAppId
    {
        get { return (docAppid.HasValue ? docAppid.Value : -1); }
        set { docAppid = value; }
    }

    public string NRIC { get; set; }
    public string VersionNo { get; set; }
    public string MonthYearClientId { get; set; }
    public string Action { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        PopulateCheckBoxList();
    }

    private void PopulateCheckBoxList()
    {
        try
        {
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(DocAppId, NRIC);
            PopulateRadMonthFrom(dt);
            if(RadMonthFrom.Items.Count > 0)
                RadMonthFrom.SelectedIndex = 0;
            if (Action.Equals("copy"))
            {
                PopulatechkBoxMonths(dt);
                TitleLabel.Text = "Copy selected item(s) to all Months";
                lblFrom.Text = "Copy from:";
                tblSelectAllMonths.Visible = true;
                tblDestination.Visible = true;
                tblSource.Visible = true;
                lblDestination.Text = "Copy To:";
            }
            else if (Action.Equals("divide"))
            {
                TitleLabel.Text = "Divide from all items from selected Month to all Months";
                lblFrom.Text = "Divide from:";
                tblSelectAllMonths.Visible = false;
                tblDestination.Visible = false;
                tblSource.Visible = true;
            }
            else
            {
                PopulatechkBoxMonths(dt);
                TitleLabel.Text = "Set Income for Selected Months(s) as Blank ";
                lblDestination.Text = "Set:";
                tblSource.Visible = false;
                tblSelectAllMonths.Visible = false;
                tblDestination.Visible = true;
            }
            
            
            //PopulatechkBoxItems();            
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }
    }

    private void PopulateRadMonthFrom(DataTable dt)
    {       
        
        RadMonthFrom.Items.Clear();
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                ListItem item = new ListItem(row["MonthYear"].ToString(), row["Id"].ToString());                
                RadMonthFrom.Items.Add(item);
            }
        }
    }

    private void PopulatechkBoxMonths(DataTable dt)
    {
        if (chkboxMonths.Items.Count == 0)
        {
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem(row["MonthYear"].ToString(), row["Id"].ToString());
                    item.Selected = true;
                    chkboxMonths.Items.Add(item);
                }
            }
        }
    }

    //private void PopulatechkBoxItems()
    //{
    //    int intVersionNo = 0;
    //    if (!VersionNo.ToLower().Contains("new"))
    //    {
    //        string[] strVersionNo = VersionNo.Split('-');
    //        intVersionNo = int.Parse(strVersionNo[0].Trim());
    //    }

    //    chkboxItems.Items.Clear();
    //    DataTable dt = IncomeDb.GetIncomeItemsByDocAppIdNRICVersionNo(DocAppId, NRIC, intVersionNo);
    //    if (dt.Rows.Count > 0)
    //    {
    //        foreach (DataRow row in dt.Rows)
    //        {
    //            ListItem item = new ListItem(row["IncomeItem"].ToString(), row["IncomeItem"].ToString());
    //            item.Selected = true;   
    //            chkboxItems.Items.Add(item);
    //        }
    //    }

        
    //}

    protected void valMonths_ServerValidation(object source, ServerValidateEventArgs args)
    {
        args.IsValid = chkboxMonths.SelectedItem != null;
    }

    protected void valItems_ServerValidation(object source, ServerValidateEventArgs args)
    {
        args.IsValid = chkboxMonths.SelectedItem != null;
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

                string[] MonthsClientID = MonthYearClientId.ToString().Substring(0, MonthYearClientId.ToString().Length - 1).Split(',');

                StringBuilder strBuild = new StringBuilder();
                StringBuilder strBuild2 = new StringBuilder();      //Added BY Edward 24/02/2014 Add Icon and Action Log
                StringBuilder sbMonthsClientId = new StringBuilder(); 
                string strMonthFrom = MonthsClientID[RadMonthFrom.SelectedIndex] ;

                foreach (ListItem item in chkboxMonths.Items)
                {
                    if (item.Selected == true)
                    {
                        strBuild.Append(item.Value + ";");
                        strBuild2.Append(item.Text + ";");          //Added BY Edward 24/02/2014 Add Icon and Action Log
                        foreach (string str in MonthsClientID)
                        {
                            if (str.Contains(item.Value))
                                sbMonthsClientId.Append(str + ";");
                        }
                    }
                    
                }
                //Session["IncomeId"] = strBuild.ToString().Substring(0, strBuild.ToString().Length - 1);
                //Session["IncomeMonthYear"] = strBuild2.ToString().Substring(0, strBuild2.ToString().Length - 1);     //Added BY Edward 24/02/2014 Add Icon and Action Log
                //Session["MonthCopyFrom"] = strMonthFrom;               

                if (Action.Equals("copy"))
                {
                    IncomeDb.InsertExtractionLog(DocAppId, RadMonthFrom.SelectedItem.Text, strBuild2.ToString().Replace(';', ','),
                        LogActionEnum.Copied_Months_From_REPLACE2_To_REPLACE3, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SaveDocInfoSript", string.Format("CloseActiveToolTip('{0}','{1}','{2}');", strMonthFrom, 
                        sbMonthsClientId.ToString().Substring(0, sbMonthsClientId.ToString().Length - 1), radIncludeAmount.SelectedValue), true);
                }
                else if (Action.Equals("divide"))
                {
                    IncomeDb.InsertExtractionLog(DocAppId, RadMonthFrom.SelectedItem.Text, NRIC,
                        LogActionEnum.Divide_Income_From_REPLACE2_For_REPLACE3, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SaveDocInfoSript", string.Format("CloseActiveToolTipDivide('{0}');", strMonthFrom), true);
                }
                else
                {
                    //IncomeDb.InsertExtractionLog(DocAppId, RadMonthFrom.SelectedItem.Text, strBuild2.ToString().Replace(';', ','),
                    //    LogActionEnum.Divide_Income_From_REPLACE2_by_REPLACE3, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SaveDocInfoSript", string.Format("CloseActiveToolTipBlank('{0}');", 
                        sbMonthsClientId.ToString().Substring(0, sbMonthsClientId.ToString().Length - 1)), true);
                }

            }
            catch (Exception ex)
            {

                throw;
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

    //protected void RadItems_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    RadioButtonList yes = (RadioButtonList)sender;
    //    if (yes.SelectedValue == "y")
    //    {
    //        foreach (ListItem item in chkboxItems.Items)
    //        {
    //            item.Selected = true;
    //        }
    //    }
    //    else
    //    {
    //        foreach (ListItem item in chkboxItems.Items)
    //        {
    //            item.Selected = false;
    //        }
    //    }
    //}
}
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

public partial class Maintenance_IncomeTemplate_AddValues : System.Web.UI.Page
{
    //string docTypeCode;           Commented by Edward 2016/02/04 take out unused variables
    string mode;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]) && !string.IsNullOrEmpty(Request["mode"]))
        {
            if (Request["id"].ToString() != "0")
            {
                mode = Request["mode"].ToString();
                if (mode == "edit")
                {
                    HeaderLabel.Text = "Edit Income Component";

                }
            }
            else
            {
                HeaderLabel.Text = "Please select an Income Template";
                SubmitButton.Enabled = false;
            }
        }
        else
        {
            HeaderLabel.Text = "Please select an Income Template";
            SubmitButton.Enabled = false;
        }

        if (!IsPostBack)
        {

            if (mode == "edit")
            {
                PopulateValues();
            }
            else
            {
                DeleteButton.Visible = false;
            }
            ValueTextBox.Focus();
        }
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string value = ValueTextBox.Text.Trim();
            IncomeTemplateItemDb db = new IncomeTemplateItemDb();
            if (mode == "edit")
                db.Update(int.Parse(Request["id"].ToString()), value, chkGrossIncome.Checked, chkAHGIncome.Checked, chkCAIncome.Checked, chkAllowance.Checked, chkOvertime.Checked);
            else
                db.Insert(int.Parse(Request["id"].ToString()), value, 1, chkGrossIncome.Checked, chkAHGIncome.Checked, chkCAIncome.Checked, chkAllowance.Checked, chkOvertime.Checked);
            

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                "javascript:CloseWindow('" + value + "');", true);
        }
    }


    protected void Delete(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string value = ValueTextBox.Text.Trim();
            IncomeTemplateItemDb db = new IncomeTemplateItemDb();
            db.Delete(int.Parse(Request["id"].ToString()));


            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                "javascript:CloseWindow('" + value + "');", true);
        }
    }
    #endregion


    private void PopulateValues()
    {
        IncomeTemplateItemDb db = new IncomeTemplateItemDb();
        IncomeTemplateItem.IncomeTemplateItemsDataTable dt = db.GetItemsById(int.Parse(Request["id"].ToString()));
        if (dt.Rows.Count > 0)
        {
            IncomeTemplateItem.IncomeTemplateItemsRow row = dt[0];
            ValueTextBox.Text = row.ItemName;
            if (!row.IsGrossIncomeNull() ? row.GrossIncome : false)
                chkGrossIncome.Checked = true;
            if (!row.IsAHGIncomeNull() ? row.AHGIncome : false)
                chkAHGIncome.Checked = true;
            if (!row.IsCAIncomeNull() ? row.CAIncome : false)
                chkCAIncome.Checked = true;
            if (!row.IsAllowanceNull() ? row.Allowance: false)
                chkAllowance.Checked = true;
            if (!row.IsOvertimeNull() ? row.Overtime : false)
                chkOvertime.Checked = true;
        }
    }

}
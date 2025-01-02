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

public partial class IncomeAssessment_ChangeCurrency : System.Web.UI.Page
{
    int? IncomeId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["incomeId"]))
        {
            IncomeId = int.Parse(Request["incomeId"]);
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        PopulateCurrency();
    }

    private void PopulateCurrency()
    {
        List<string> listCurrencies = EnumManager.GetCurrencies();

        foreach (string str in listCurrencies)
        {
            RadComboBoxItem item = new RadComboBoxItem(str);
            CurrencyDropdownList.Items.Add(item);
        }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {

        try
        {
            Button s = (Button)sender;            
            ErrorLabel.Text = "";
            string currRate;
            string currency;
            if (s.Text.ToUpper().Trim() == "DEFAULT")
            {
                currency = "SGD";
                currRate = "1";
            }
            else
            {
                currency = CurrencyDropdownList.Text;
                currRate = CurrencyRateTextbox.Text;
            }
            decimal CurrencyRate = 0;
            if (decimal.TryParse(currRate, out CurrencyRate))
            {
                if (CurrencyRate != 0)//added by Edward 2014/5/21   divide by zero
                {
                    IncomeDb.UpdateIncome(IncomeId.Value, CurrencyRate, currency);
                    #region Added By Edward 24/02/2014  Add Icon and Action Log
                    IncomeDb.InsertExtractionLog(IncomeId.Value, CurrencyDropdownList.Text, CurrencyRateTextbox.Text,
                        LogActionEnum.Updated_Currency_to_REPLACE2_and_rate_to_REPLACE3, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
                    #endregion
                    ConfirmPanel.Visible = true;
                    FormPanel.Visible = false;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:ResizeAndClose(550, 200);", true);
                }
                else
                    ErrorLabel.Text = "Zero currency rate not allowed.";
            }
            else
            {
                ErrorLabel.Text = "Please enter a valid amount.";
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), ex.Source.ToString(), string.Format("javascript:alert('{0}');", ex.Message.ToString()), true); ;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:CloseWindow();", true);
        }

    }


}
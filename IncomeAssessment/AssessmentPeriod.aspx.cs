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

public partial class IncomeAssessment_AssessmentPeriod : System.Web.UI.Page
{
    int? docAppId;
    string nric;
    int m;
    string strRefType;
    protected void Page_Load(object sender, EventArgs e)
    {


        if (!string.IsNullOrEmpty(Request["docAppId"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            docAppId = int.Parse(Request["docAppId"]);
            nric = Request["nric"].ToString();
            m =  !string.IsNullOrEmpty(Request["m"]) ? int.Parse(Request["m"]) : 0;
            strRefType = Request["ref"].ToString();
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        
        if (!IsPostBack)
        {
            PopulateMonthDropdownList();
            PopulateYearDropdownList();
            LoadPeriod();
            if (strRefType.ToUpper() == "HLE")
                Label1.Text = "Months to pass to LEAS";
            else if (strRefType.ToUpper() == "RESALE")
                Label1.Text = "Months to pass";
            else if (strRefType.ToUpper() == "SALES")
                Label1.Text = "Months to pass";
        }
    }

    private void LoadPeriod()
    {
        DataTable dt = IncomeDb.GetAssessmentPeriod(docAppId.Value, nric);
        if (dt.Rows.Count > 0)
        {
            Month1.SelectedValue = dt.Rows[0]["IncomeMonth"].ToString();
            Month2.SelectedValue = dt.Rows[dt.Rows.Count - 1]["IncomeMonth"].ToString();
            Year1.SelectedValue = dt.Rows[0]["IncomeYear"].ToString();
            Year2.SelectedValue = dt.Rows[dt.Rows.Count - 1]["IncomeYear"].ToString();
            //TxtMonth.Text = dt.Rows[0]["MonthsToLeas"].ToString();
            lblMonth.Text = !string.IsNullOrEmpty(dt.Rows[0]["MonthsToLeas"].ToString()) ? dt.Rows[0]["MonthsToLeas"].ToString() : "0";
            ChknotApplicable.Checked = !string.IsNullOrEmpty(dt.Rows[0]["AssessmentNA"].ToString()) ? bool.Parse(dt.Rows[0]["AssessmentNA"].ToString()) : false;
            m = int.Parse(lblMonth.Text);
        }
        else
            lblMonth.Text = m.ToString();
    }

    private void CalculateLeas()
    {
        DateTime dt = new DateTime(int.Parse(Year2.SelectedValue), int.Parse(Month2.SelectedValue), 1);
        DateTime dtFrom = new DateTime(int.Parse(Year2.SelectedValue), int.Parse(Month2.SelectedValue), 1).AddMonths(m != 0 ? -(m-1) : 0);
        Month1.SelectedValue = dtFrom.Month.ToString();
        Year1.SelectedValue = dtFrom.Year.ToString();
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            try
            {
                if (!ChknotApplicable.Checked)
                {
                    #region Create Assessment Period
                    DateTime startDate = new DateTime(int.Parse(Year1.SelectedValue), int.Parse(Month1.SelectedValue), 1);
                    DateTime endDate = new DateTime(int.Parse(Year2.SelectedValue), int.Parse(Month2.SelectedValue), 1);
                    int NoOfMonths = ((endDate.Year - startDate.Year) * 12) + (endDate.Month - startDate.Month);
                    if (startDate.CompareTo(endDate) <= 0)
                    {
                        if (NoOfMonths <= 24)
                        {
                            Guid? userId = (Guid)Membership.GetUser().ProviderUserKey;
                            //DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, nric);
                            int appPersonalId = 0;
                            AppPersonalDb d = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable dt = d.GetAppPersonalByNricAndDocAppId(nric, docAppId.Value);
                            if (dt.Rows.Count > 0)
                            {
                                AppPersonal.AppPersonalRow r = dt[0];
                                appPersonalId = r.Id;
                            }

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    bool IsInAssessmentPeriodRange = false;
                                    for (var month = startDate.Date; month.Date <= endDate.Date; month = month.AddMonths(1))
                                    {
                                        if (int.Parse(IncomeRow["IncomeMonth"].ToString()) == month.Month &&
                                            int.Parse(IncomeRow["IncomeYear"].ToString()) == month.Year)
                                        {
                                            IsInAssessmentPeriodRange = true;
                                            break;
                                        }
                                    }
                                    if (IsInAssessmentPeriodRange == false)
                                    {
                                        DataTable IncomeVersionDt = IncomeDb.GetIncomeVersionByIncome(int.Parse(IncomeRow["id"].ToString()));
                                        if (IncomeVersionDt.Rows.Count > 0)
                                        {
                                            foreach (DataRow IncomeVersionRow in IncomeVersionDt.Rows)
                                            {
                                                IncomeDb.DeleteAllIncomeDetailsByVersionId(int.Parse(IncomeVersionRow["Id"].ToString()));
                                            }
                                        }
                                        IncomeDb.DeleteAllIncomeVersionsByIncomeId(int.Parse(IncomeRow["id"].ToString()));
                                        IncomeDb.DeleteIncomeByIncomeId(int.Parse(IncomeRow["id"].ToString()));
                                    }
                                }
                            }

                            for (var month = startDate.Date; month.Date <= endDate.Date; month = month.AddMonths(1))
                            {
                                DataTable DtByMonthAndYear = IncomeDb.GetIncomeByAppPersonalIdByMonthByYear(appPersonalId, month.Month, month.Year);
                                if (DtByMonthAndYear.Rows.Count == 0)
                                    IncomeDb.InsertIncome(0, month.Month, month.Year, "SGD", 1, userId.Value, appPersonalId);
                            }

                            UpdateMonthsToLEAS(appPersonalId, !string.IsNullOrEmpty(lblMonth.Text) ? int.Parse(lblMonth.Text) : 0);
                            #region Added By Edward 24/02/2014  Add Icon and Action Log
                            IncomeDb.InsertExtractionLog(docAppId.Value,nric, string.Empty,
                                LogActionEnum.Updated_Extraction_Period_of_REPLACE2, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                            #endregion

                            ConfirmPanel.Visible = true;
                            FormPanel.Visible = false;
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:ResizeAndClose(550, 200);", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invalid Date", string.Format("javascript:alert('{0}');", "Select a date range within 24 months."), true); ;
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Invalid Date", string.Format("javascript:alert('{0}');", "Begin Date must not be more than the End Date."), true); ;
                    }
                    #endregion
                }
                else
                {
                    AppPersonalDb d = new AppPersonalDb();
                    AppPersonal.AppPersonalDataTable dt = d.GetAppPersonalByNricAndDocAppId(nric, docAppId.Value);
                    if (dt.Rows.Count > 0)
                    {
                        AppPersonal.AppPersonalRow r = dt[0];
                        IncomeDb.DeleteAllExtractionByAppPersonalId(r.Id);
                        UpdateMonthsToLEAS(r.Id, 0);
                    }

                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), ex.Source.ToString(), string.Format("javascript:alert('{0}');", ex.Message.ToString()), true); ;
               
            }
            finally
            {
               ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:CloseWindow();", true);
            }
        }
    }


    private void PopulateMonthDropdownList()
    {
        string[] arrMonthName = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};

        for (int i = 0; i < 12; i++)
        {
            RadComboBoxItem li = new RadComboBoxItem(arrMonthName[i].ToString(), (i + 1).ToString());
            Month1.Items.Add(li);            
            
        }

        Month1.SelectedValue = (DateTime.Now.Month + 1).ToString();
        for (int i = 0; i < 12; i++)
        {
            RadComboBoxItem li = new RadComboBoxItem(arrMonthName[i].ToString(), (i + 1).ToString());            
            Month2.Items.Add(li);
        }
        Month2.SelectedValue = DateTime.Now.Month.ToString();
    }

    private void PopulateYearDropdownList()
    {
        for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 5; i--)
        {
            RadComboBoxItem li = new RadComboBoxItem(i.ToString(), i.ToString());
            Year1.Items.Add(li);

        }

        Year1.SelectedValue = (DateTime.Now.Year - 1).ToString();
        for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 5; i--)
        {
            RadComboBoxItem li = new RadComboBoxItem(i.ToString(), i.ToString());
            Year2.Items.Add(li);

        }
        Year2.SelectedValue = DateTime.Now.Year.ToString();
    }

    private void UpdateMonthsToLEAS(int appPersonalId, int months)
    {
        try
        {
            AppPersonalDb db = new AppPersonalDb();
            db.UpdateMonthToLEAS(appPersonalId, months, ChknotApplicable.Checked);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), ex.Source.ToString(), string.Format("javascript:alert('{0}');", ex.Message.ToString()), true); ;
        }        
    }

    protected void AdjustmentCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        //int d = 0;
        if (!ChknotApplicable.Checked)
        {
            //if (string.IsNullOrEmpty(lblMonth.Text))
            //{
            //    AdjustmentCustomValidator.ErrorMessage = "<br />Please enter number of months.";
            //    args.IsValid = false;
            //}
            //else if (!string.IsNullOrEmpty(lblMonth.Text) && !int.TryParse(lblMonth.Text, out d))
            //{
            //    AdjustmentCustomValidator.ErrorMessage = "<br />Please enter number of months in digits.";
            //    args.IsValid = false;
            //}
            //else 
            if ((!string.IsNullOrEmpty(lblMonth.Text) ? int.Parse(lblMonth.Text) : 0) > CheckPeriod())
            {
                AdjustmentCustomValidator.ErrorMessage = string.Format("<br />The Extraction Period cannot be shorter than the {0} months.", lblMonth.Text);
                args.IsValid = false;
            }
        }
    }

    private int CheckPeriod()
    {
        int i = 0;
        DateTime startDate = new DateTime(int.Parse(Year1.SelectedValue), int.Parse(Month1.SelectedValue), 1);
        DateTime endDate = new DateTime(int.Parse(Year2.SelectedValue), int.Parse(Month2.SelectedValue), 1);
        for (var month = startDate.Date; month.Date <= endDate.Date; month = month.AddMonths(1))
           i++;
        return i;
    }
    protected void ChknotApplicable_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox check = (CheckBox)sender;
        if (check.Checked)
        {
            Month1.Enabled = false;
            Month2.Enabled = false;
            Year1.Enabled = false;
            Year2.Enabled = false;
            //TxtMonth.Enabled = false;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Not Applicable", string.Format("javascript:alert('{0}');","This will delete all Income Extraction of this person once confirmed."), true); ;
        }
        else
        {
            Month1.Enabled = true;
            Month2.Enabled = true;
            Year1.Enabled = true;
            Year2.Enabled = true;
            //TxtMonth.Enabled = true;
        }

    }
    protected void Year2_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        CalculateLeas();
    }
    protected void Month2_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        CalculateLeas();
    }
}
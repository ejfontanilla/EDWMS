using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.IO;
using System.Drawing;
using iTextSharp.text.pdf;

public partial class IncomeAssessment_ViewLog : System.Web.UI.Page
{
    int? Id;
    int? DocAppId;
    string NRIC;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            Id = int.Parse(Request["id"]);
            DocAppId = int.Parse(Request["docAppId"]);
            NRIC = Request["nric"].ToString();
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        //GetIncomeMonthYear();
        PopulateApplicantDetails();
    }

    protected void ViewLogRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        PopulateViewLogGrid();
    }

    protected void ViewLogRadGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;
            Label Doclbl = e.Item.FindControl("lblDoc") as Label;
            //Doclbl.Text = data["Description"].ToString() + " | " + (bool.Parse(data["IsAccepted"].ToString()) == true ? "Accepted" : "Not Accepted");
            Doclbl.Text = string.Format("{0} | {1}",data["Description"].ToString(),!string.IsNullOrEmpty(data["CMDocumentId"].ToString()) ? data["CMDocumentId"].ToString() : " - ");

            Label LogDateLabel = e.Item.FindControl("LogDateLabel") as Label;
            string monthName = new DateTime(DateTime.Parse(data["LogDate"].ToString()).Year, DateTime.Parse(data["LogDate"].ToString()).Month, DateTime.Parse(data["LogDate"].ToString()).Day, 
                DateTime.Parse(data["LogDate"].ToString()).Hour, DateTime.Parse(data["LogDate"].ToString()).Minute, 
                DateTime.Parse(data["LogDate"].ToString()).Second).ToString("d MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            LogDateLabel.Text = monthName;

            DateTime result;

            Label lblStartDate = e.Item.FindControl("lblStartDate") as Label;
            string strStartDate = DateTime.TryParse(data["StartDate"].ToString(), out result) ?  new DateTime(DateTime.Parse(data["StartDate"].ToString()).Year, 
                DateTime.Parse(data["StartDate"].ToString()).Month, DateTime.Parse(data["StartDate"].ToString()).Day).ToString("d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) : " - ";
            lblStartDate.Text = strStartDate;

            Label lblEndDate = e.Item.FindControl("lblEndDate") as Label;
            string strEndDate = DateTime.TryParse(data["StartDate"].ToString(), out result) ? new DateTime(DateTime.Parse(data["StartDate"].ToString()).Year, 
                DateTime.Parse(data["EndDate"].ToString()).Month, DateTime.Parse(data["EndDate"].ToString()).Day).ToString("d MMM yyyy", System.Globalization.CultureInfo.InvariantCulture) : " - " ;
            lblEndDate.Text = strEndDate;
        }
    }

    private void PopulateViewLogGrid()
    {
        ViewLogDb viewLogDb = new ViewLogDb();
        ViewLog.ViewLogDataTable dt = viewLogDb.GetViewLogByDocAppIdAndNric(DocAppId.Value, NRIC);
        ViewLogRadGrid.DataSource = dt;

    }

    private void PopulateApplicantDetails()
    {
        DataTable dt = IncomeDb.GetApplicantDetails(DocAppId.Value, NRIC);
        if (dt.Rows.Count > 0)
        {
            DataRow row = dt.Rows[0];
            string personalType = row["PersonalType"].ToString();
            string applicantTypeFormat = "{0} - {1} ({2})";
            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                DescriptionLabel.Text = String.Format(applicantTypeFormat, "Applicant " + row["OrderNo"].ToString(), row["Name"].ToString(), row["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                DescriptionLabel.Text = String.Format(applicantTypeFormat, "Occupier " + row["OrderNo"].ToString(), row["Name"].ToString(), row["Nric"].ToString());
        }
    }
    private void GetIncomeMonthYear()
    {
        DataTable dt = IncomeDb.GetIncomeDataById(Id.Value);
        if (dt.Rows.Count > 0)
        {
            string month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(dt.Rows[0]["IncomeMonth"].ToString()));
            string year = dt.Rows[0]["IncomeYear"].ToString();
            TitleLabel.Text = string.Format("{0} for {1} {2}", TitleLabel.Text, month, year);
        }
    }
}
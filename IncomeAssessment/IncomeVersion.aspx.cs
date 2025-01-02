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

public partial class IncomeAssessment_IncomeVersion : System.Web.UI.Page
{
    int? Id;
    int? DocAppId;
    string NRIC;
    int IncomeVersionUsed;

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

        GetIncomeMonthYear();
        PopulateApplicantDetails();
    }

    protected void IncomeVersionRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        PopulateIncomeVersionGrid();
    }

    protected void IncomeVersionRadGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;

            //if (int.Parse(data["Id"].ToString()) == IncomeVersionUsed)
            //{
            //    System.Web.UI.WebControls.Image imgVersionUsed = e.Item.FindControl("imgVersionUsed") as System.Web.UI.WebControls.Image;
            //    imgVersionUsed.ImageUrl = "~/Data/Images/Green_Check.png";
            //    imgVersionUsed.ToolTip = "This version is currently being used.";
            //    imgVersionUsed.Visible = true;
            //}

            Label lbVersionNo = e.Item.FindControl("lbVersionNo") as Label;
            lbVersionNo.Text = data["VersionNo"].ToString();

            Label lblVersionName = e.Item.FindControl("lblVersionName") as Label;
            lblVersionName.Text = !string.IsNullOrEmpty(data["VersionName"].ToString()) ? data["VersionName"].ToString() : " - ";            

            Label lblLastSavedBy = e.Item.FindControl("lblLastSavedBy") as Label;            
            lblLastSavedBy.Text = data["Name"].ToString();

            Label lblLastSavedDate = e.Item.FindControl("lblLastSavedDate") as Label;
            string monthName = new DateTime(DateTime.Parse(data["DateEntered"].ToString()).Year, DateTime.Parse(data["DateEntered"].ToString()).Month, DateTime.Parse(data["DateEntered"].ToString()).Day,
                DateTime.Parse(data["DateEntered"].ToString()).Hour, DateTime.Parse(data["DateEntered"].ToString()).Minute,
                DateTime.Parse(data["DateEntered"].ToString()).Second).ToString("d MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            lblLastSavedDate.Text = monthName;

            LinkButton lbEdit = e.Item.FindControl("lbEdit") as LinkButton;
            lbEdit.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersionEditDelete.aspx?versionNo={0}&mode={1}&incomeid={2}&docappId={3}&nric={4}',550,200);return false;",
                data["VersionNo"].ToString(), "edit", Id.Value, DocAppId.Value, NRIC));

            LinkButton lbDelete = e.Item.FindControl("lbDelete") as LinkButton;
            lbDelete.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersionEditDelete.aspx?versionNo={0}&mode={1}&incomeid={2}&docappId={3}&nric={4}',550,200);return false;",
                data["VersionNo"].ToString(), "delete", Id.Value, DocAppId.Value, NRIC));
        }
    }

    private void PopulateIncomeVersionGrid()
    {
        //DataTable dt = IncomeDb.GetIncomeVersionByIncomeId(Id.Value);
        DataTable dt = IncomeDb.GetIncomeVersionByDocAppIdAndNricWithProfile(DocAppId.Value, NRIC);
        IncomeVersionRadGrid.DataSource = dt;        
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
            TitleLabel.Text = string.Format("{0}", TitleLabel.Text);
            IncomeVersionUsed = int.Parse(dt.Rows[0]["IncomeVersionId"].ToString());
        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        IncomeVersionRadGrid.Rebind();
    }
}
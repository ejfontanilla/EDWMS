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

public partial class IncomeAssessment_ActionLog : System.Web.UI.Page
{
    int? docAppId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            docAppId = int.Parse(Request["id"]);            
        }
        else
            Response.Redirect("~/IncomeAssessment/");
    }

    protected void ActionLogRadGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        PopulateLogAction();
    }

    protected void ActionLogRadGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;

            Label lblName = e.Item.FindControl("lblName") as Label;
            lblName.Text = data["Name"].ToString();

            Label lblDescription = e.Item.FindControl("lblDescription") as Label;
            lblDescription.Text = data["Description"].ToString();

            Label lblAction = e.Item.FindControl("lblAction") as Label;
            lblAction.Text = data["Action"].ToString();

            Label lblLogDate = e.Item.FindControl("lblLogDate") as Label;
            string monthName = new DateTime(DateTime.Parse(data["LogDate"].ToString()).Year, DateTime.Parse(data["LogDate"].ToString()).Month, DateTime.Parse(data["LogDate"].ToString()).Day,
                DateTime.Parse(data["LogDate"].ToString()).Hour, DateTime.Parse(data["LogDate"].ToString()).Minute,
                DateTime.Parse(data["LogDate"].ToString()).Second).ToString("d MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            lblLogDate.Text = monthName;
            
        }
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        
    }

    private void PopulateLogAction()
    {
        DataTable dt = LogActionDb.GetLogActionIncomeExtraction(docAppId.Value);
        ActionLogRadGrid.DataSource = dt;
    }

}
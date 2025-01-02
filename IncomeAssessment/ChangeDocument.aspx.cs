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

public partial class IncomeAssessment_ChangeDocument : System.Web.UI.Page
{
    int? docAppId;
    string nric;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["docAppId"]) && !string.IsNullOrEmpty(Request["nric"]))
        {
            docAppId = int.Parse(Request["docAppId"]);
            nric = Request["nric"].ToString();            
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        if (!IsPostBack)
        {
            PopulateIncome();
        }
    }

    private void PopulateIncome()
    {
        DataTable dt = IncomeDb.GetDataForIncomeAssessment(docAppId.Value,nric);
        IncomeRadGrid.DataSource = dt;
        IncomeRadGrid.DataBind();
    }


    protected void IncomeRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            DataRowView data = (DataRowView)e.Item.DataItem;

            DataTable dt = IncomeDb.GetDocsByIncomeId(docAppId.Value, nric, int.Parse(data["Id"].ToString()));

            if (dt.Rows.Count > 0)
            {               

                Repeater lblDocRepeater = (Repeater)e.Item.FindControl("lblDocRepeater");
                lblDocRepeater.DataSource = dt;
                lblDocRepeater.DataBind();
            }            
        }
    }

    protected void lblDocRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            HyperLink lbl = (HyperLink)e.Item.FindControl("lblDoc");
            //bool isAccepted = false;
            if (data["ImageAccepted"].ToString() == "Y")
            {
                lbl.Text = data["Description"].ToString() + " | " + (data["ImageAccepted"].ToString() == "Y" ? "Accepted" : "Not Accepted") + " | " + data["DocId"].ToString();
                lbl.NavigateUrl = string.Format("javascript:UpdatePDFAndClose({0})", data["AppDocRefId"].ToString());
            }
        }

    }
}
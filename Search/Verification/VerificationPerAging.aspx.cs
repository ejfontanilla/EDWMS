using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using System.IO;
using Dwms.Web;
using System.Data;

public partial class Search_Verification_VerificationPerAging : System.Web.UI.Page
{
    int sectionId = -1;
    int departmentId = -1;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    /// <summary>
    /// Rad Grid 1 need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid2_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid2_ItemDataBound(object sender, GridItemEventArgs e)
    {

        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            DataRowView data = (DataRowView)e.Item.DataItem;
            Label lblMonthYear = (Label)e.Item.FindControl("lblMonthYear");

            if (e.Item.OwnerTableView.Name != "Child")
                lblMonthYear.Text = string.Format("{0}", data["AgingRange"].ToString());

            if (e.Item.OwnerTableView.Name == "Child")
            {
                Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
                DateTime dateInDateTime = !string.IsNullOrEmpty(data["DateAssigned"].ToString()) ? DateTime.Parse(data["DateAssigned"].ToString()) : DateTime.Now; 
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["VerificationDateOut"].ToString()) ? DateTime.Parse(data["VerificationDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                
            }
        }
    }

    protected void RadGrid2_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
    {
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
        int aging = int.Parse(dataItem.GetDataKeyValue("AgingNo").ToString());
        e.DetailTableView.DataSource = DocSetDb.GetVerificationPerAging(sectionId, departmentId, aging);
    }

    /// <summary>
    /// Search button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        RadGrid2.Rebind();
    }

    /// <summary>
    /// Item command event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    protected void ResetButton_Click(object sender, EventArgs e)
    {

        RadTreeView tree = (RadTreeView)DepartmentSectionRadComboBox.Items[0].FindControl("RadTreeView1");
        tree.Nodes[0].Selected = true;
        DepartmentSectionRadComboBox.Text = "All Departments";

        PopulateList();
        RebindGrids();
    }

    private void RebindGrids()
    {
        RadGrid2.DataBind();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        RadTreeView tree = (RadTreeView)DepartmentSectionRadComboBox.Items[0].FindControl("RadTreeView1");

        if (tree.SelectedNode != null)
        {
            if (tree.SelectedNode.Value.ToString().ToLower().Contains("d"))
            {
                departmentId = int.Parse(tree.SelectedNode.Value.ToString().ToLower().Replace("d", string.Empty));
            }
            else if (tree.SelectedNode.Value.ToString().ToLower().Contains("s"))
            {
                sectionId = int.Parse(tree.SelectedNode.Value.ToString().ToLower().Replace("s", string.Empty));
                // get deptid based on section id
                SectionDb secttionDb = new SectionDb();
                Section.SectionDataTable section = secttionDb.GetSectionById(sectionId);
                Section.SectionRow sRow = section[0];
                departmentId = sRow.Department;
            }
        }        
        RadGrid2.DataSource = DocSetDb.GetAgingRange(sectionId, departmentId);
    }
}
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

public partial class Search_BatchUpload_Default : System.Web.UI.Page
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
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            DataRowView data = (DataRowView)e.Item.DataItem;

            DataTable dt = BatchUploadDb.GetBatchUploadDetailsById(int.Parse(data["BUId"].ToString()));
            if(dt.Rows.Count > 0)
            {
                Repeater lblDocRepeater = (Repeater)e.Item.FindControl("lblDocRepeater");
                lblDocRepeater.DataSource = dt;
                lblDocRepeater.DataBind();
            }
        }
    }

    protected void lblDocRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = (RepeaterItem)e.Item;
        DataRowView data = (DataRowView)item.DataItem;
        Label lbl = (Label)e.Item.FindControl("lblDoc");
        lbl.Text = data["RefNo"].ToString();
    }

    /// <summary>
    /// Search button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        RadGrid1.Rebind();
    }

    /// <summary>
    /// Item command event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
    }



    protected void ResetButton_Click(object sender, EventArgs e)
    {

        RadTreeView tree = (RadTreeView)DepartmentSectionRadComboBox.Items[0].FindControl("RadTreeView1");
        tree.Nodes[0].Selected = true;
        DepartmentSectionRadComboBox.Text = "All Departments";

        DateFromRadDateTimePicker.SelectedDate = null;
        DateToRadDateTimePicker.SelectedDate = null;

        PopulateList();
        RebindGrids();
    }

    private void RebindGrids()
    {
        RadGrid1.DataBind();
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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;
        if (DateFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateFromRadDateTimePicker.SelectedDate.Value;

        if (DateToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateToRadDateTimePicker.SelectedDate.Value;

        RadGrid1.DataSource = BatchUploadDb.GetBatchUpload(sectionId, departmentId, dateInFrom, dateInTo);
    }
}
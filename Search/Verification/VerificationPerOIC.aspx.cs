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


public partial class Search_Verification_VerificationPerOIC : System.Web.UI.Page
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
    //protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    //{
    //    PopulateList();
    //}

    protected void RadGrid2_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    //{
    //    if (e.Item is GridDataItem)
    //    {
    //        RadGrid grid = (RadGrid)sender;
    //        GridDataItem dataBoundItem = e.Item as GridDataItem;
    //        DataRowView data = (DataRowView)e.Item.DataItem;
    //        Label lblMonthYear = (Label)e.Item.FindControl("lblMonthYear");

    //        lblMonthYear.Text = string.Format("{0} {1}",
    //            System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(data["OcrMonth"].ToString())),
    //            data["OcrYear"].ToString());

    //        DataTable dt = ExceptionLogDb.GetOCRWebServiceError(sectionId, departmentId, int.Parse(data["OcrMonth"].ToString()), int.Parse(data["OcrYear"].ToString()));

    //        //Repeater lblDocRepeater = (Repeater)e.Item.FindControl("lblDocRepeater");
    //        //lblDocRepeater.DataSource = dt;
    //        //lblDocRepeater.DataBind();

    //        RadGrid radgridReason = (RadGrid)e.Item.FindControl("RadGridReason");
    //        radgridReason.DataSource = dt;
    //        radgridReason.DataBind();

    //        //Label lblCOunt = (Label)e.Item.FindControl("lblCount");

    //        //lblCOunt.Text = ExceptionLogDb.GetOCRWebCount(int.Parse(data["OcrMonth"].ToString()), int.Parse(data["OcrYear"].ToString())
    //        //    , data["Reason"].ToString()).ToString();

    //    }
    //}

    //protected void RadGrid2_ItemCreated(object sender, GridItemEventArgs e)
    //{
    //    GridItem item = e.Item;

    //    if (e.Item is GridDataItem)
    //    {
    //        Label ItemCountLabel = item.FindControl("ItemCountLabel") as Label;
    //        int page = RadGrid1.MasterTableView.PagingManager.CurrentPageIndex;
    //        ItemCountLabel.Text = (page * RadGrid1.PageSize + e.Item.ItemIndex + 1).ToString();
    //    }
    //}

    protected void RadGrid2_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            RadGrid grid = (RadGrid)sender;
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            DataRowView data = (DataRowView)e.Item.DataItem;
            Label lblMonthYear = (Label)e.Item.FindControl("lblMonthYear");

            if (e.Item.OwnerTableView.Name != "Child")
                lblMonthYear.Text = data["Name"].ToString();

            if (e.Item.OwnerTableView.Name == "Child")
            {
                Label lblCOunt = (Label)e.Item.FindControl("lblCount");
                lblCOunt.Text = DocSetDb.GetSetCountPerAgingPerOIC(sectionId, departmentId, (Guid)data["VerificationStaffUserId"], int.Parse(data["AgingNo"].ToString())).ToString();
            }
        }
    }

    //protected void lblDocRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    //{
    //    RepeaterItem item = (RepeaterItem)e.Item;
    //    DataRowView data = (DataRowView)item.DataItem;
    //    Label lbl = (Label)e.Item.FindControl("lblDoc");
    //    lbl.Text = data["Reason"].ToString();
    //}

    //protected void lblCountRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    //{
    //    RepeaterItem item = (RepeaterItem)e.Item;
    //    DataRowView data = (DataRowView)item.DataItem;
    //    Label lbl = (Label)e.Item.FindControl("lblCount");
    //    lbl.Text = data["Reason"].ToString();
    //}


    protected void RadGrid2_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
    {        
        GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
        string str = dataItem.GetDataKeyValue("Name").ToString();
        e.DetailTableView.DataSource = DocSetDb.GetAgingRange(sectionId, departmentId, (Guid) dataItem.GetDataKeyValue("UserId"));
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
    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {
    }

    protected void RadGrid2_ItemCommand(object sender, GridCommandEventArgs e)
    {
    }

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
        RadGrid2.DataSource = DocSetDb.GetVerificationOIC(sectionId, departmentId);
    }
}
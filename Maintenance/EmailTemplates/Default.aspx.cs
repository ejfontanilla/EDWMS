using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Maintenance_EmailTemplates_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    /// <summary>
    /// RadGrid need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateRadGrid();
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
            Label ItemCountLabel = e.Item.FindControl("ItemCountLabel") as Label;
            int page = RadGrid1.MasterTableView.PagingManager.CurrentPageIndex;
            ItemCountLabel.Text = (page * RadGrid1.PageSize + e.Item.ItemIndex + 1).ToString();
        }
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the RadGrid with data
    /// </summary>
    private void PopulateRadGrid()
    {
        EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
        RadGrid1.DataSource = emailTemplateDb.GetEmailTemplate();
    }
    #endregion
}

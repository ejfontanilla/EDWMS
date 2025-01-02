using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Common_ChooseReference : System.Web.UI.Page
{
    string nric = string.Empty;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(Request["nric"]))
        {
            nric = Request["nric"];
        }

        if (!IsPostBack)
        {
            if (!String.IsNullOrEmpty(nric))
            {

            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    "javascript:CloseWindow('');", true);
            }
        }
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        if (ValidatePage())
        {
            string refNo = string.Empty;

            GridItemCollection seletedItems = RefNoRadGrid.SelectedItems;

            if (seletedItems.Count > 0)
            {
                GridDataItem item = seletedItems[0] as GridDataItem;
                refNo = item.GetDataKeyValue("RefNo").ToString();
            }

            ScriptManager.RegisterStartupScript(
                this.Page, 
                this.GetType(), 
                "CloseWindow",
                String.Format("javascript:CloseWindow('{0}');", refNo), 
                true);
        }
    }

    protected void RefNoRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        if (!String.IsNullOrEmpty(nric))
        {
            DocAppDb docAppDb = new DocAppDb();
            RefNoRadGrid.DataSource = docAppDb.GetRefNosForNric(nric);
        }
    }
    #endregion

    #region Validation
    private bool ValidatePage()
    {
        bool result = false;
        GridItemCollection seletedItems = RefNoRadGrid.SelectedItems;

        if (seletedItems.Count == 0)
        {
            ErrorLabel.Visible = true;
        }
        else
        {
            ErrorLabel.Visible = false;
            result = true;
        }

        return result;
    }
    #endregion  

    #region Private Methods
    #endregion
}
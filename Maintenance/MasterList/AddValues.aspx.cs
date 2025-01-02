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

public partial class Maintenance_MasterList_AddValues : System.Web.UI.Page
{
    //string docTypeCode;           Commented by Edward 2016/02/04 take out unused variables

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
            ValueTextBox.Focus();
        }
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string value = ValueTextBox.Text.Trim();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                "javascript:CloseWindow('" + value + "');", true);
        }        
    }
    #endregion

    #region Validation
    #endregion  

    #region Private Methods
    #endregion
}
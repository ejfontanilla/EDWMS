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

public partial class Import_Controls_ChooseReference : System.Web.UI.Page
{
    string nric;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request["nric"]))
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
        //contains = false;

        //if (!String.IsNullOrEmpty(Request["contains"].ToString()))
        //    contains = (Request["contains"].ToString().Equals("1") ? true : false);

        //if (!IsPostBack)
        //{
        //    TitleLabel.Text = String.Format("Add Keywords {0}", (contains ? "(Contains)" : "(Not contains)"));

        //    DataTable initialDt = new DataTable();
        //    initialDt.Columns.Add("Keyword");

        //    DataRow initialRow = initialDt.NewRow();
        //    initialRow["Keyword"] = string.Empty;
        //    initialDt.Rows.Add(initialRow);

        //    SetRepeaterData(initialDt);
        //}
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        //string condition = string.Empty;
        //string keywordFormat = "{0}";
        //string keywordVariableFormat = "{0}";

        //// Compose the condition
        //foreach (RepeaterItem item in KeywordRepeater.Items)
        //{
        //    //string keyword = (item.FindControl("KeywordTextBox") as TextBox).Text;

        //    string keyword = (item.FindControl("KeywordRadComboBox") as RadComboBox).Text;

        //    if (!String.IsNullOrEmpty(keyword))
        //    {
        //        keyword = String.Format((Validation.IsKeywordVariable(keyword) ? keywordVariableFormat : keywordFormat), keyword);
        //        //condition = (String.IsNullOrEmpty(condition) ? keyword : condition +
        //        //    " " + CategorisationRuleOpeatorEnum.and.ToString() + " " + keyword);
        //        condition = (String.IsNullOrEmpty(condition) ? keyword : condition +
        //            Constants.KeywordSeperator + keyword);
        //    }
        //}

        //// Enclose in parenthesis
        //if (condition.Contains(" " + CategorisationRuleOpeatorEnum.and.ToString() + " "))
        //    condition = "(" + condition + ")";

        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
        //    "javascript:CloseWindow('" + condition + "," + contains.ToString() + "');", true);
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
    #endregion  

    #region Private Methods
    #endregion
}
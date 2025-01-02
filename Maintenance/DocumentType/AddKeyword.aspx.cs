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

public partial class Maintenance_DocumentType_AddKeyword : System.Web.UI.Page
{
    bool contains;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        contains = false;

        if (!String.IsNullOrEmpty(Request["contains"].ToString()))
            contains = (Request["contains"].ToString().Equals("1") ? true : false);

        if (!IsPostBack)
        {
            TitleLabel.Text = String.Format("Add Keywords {0}", (contains ? "(Contains)" : "(Not contains)"));

            DataTable initialDt = new DataTable();
            initialDt.Columns.Add("Keyword");

            DataRow initialRow = initialDt.NewRow();
            initialRow["Keyword"] = string.Empty;
            initialDt.Rows.Add(initialRow);

            SetRepeaterData(initialDt);
        }
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        string condition = string.Empty;
        string keywordFormat = "{0}";
        string keywordVariableFormat = "{0}";

        // Compose the condition
        foreach (RepeaterItem item in KeywordRepeater.Items)
        {
            //string keyword = (item.FindControl("KeywordTextBox") as TextBox).Text;

            string keyword = (item.FindControl("KeywordRadComboBox") as RadComboBox).Text;

            if (!String.IsNullOrEmpty(keyword))
            {
                keyword = String.Format((Validation.IsKeywordVariable(keyword) ? keywordVariableFormat : keywordFormat), keyword);
                //condition = (String.IsNullOrEmpty(condition) ? keyword : condition +
                //    " " + CategorisationRuleOpeatorEnum.and.ToString() + " " + keyword);
                condition = (String.IsNullOrEmpty(condition) ? keyword : condition +
                    Constants.KeywordSeperator + keyword);
            }
        }

        // Enclose in parenthesis
        if (condition.Contains(" " + CategorisationRuleOpeatorEnum.and.ToString() + " "))
            condition = "(" + condition + ")";

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
            "javascript:CloseWindow('" + condition + "," + contains.ToString() + "');", true);
    }

    /// <summary>
    /// Item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void KeywordRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = e.Item as RepeaterItem;

            //if (item.ItemIndex == 0 && KeywordRepeater.Items.Count == 1)               
            //    (item.FindControl("DeleteImageButton") as ImageButton).Visible = false;

            RadComboBox KeywordRadComboBox = (item.FindControl("KeywordRadComboBox") as RadComboBox);
            KeywordRadComboBox.DataSource = EnumManager.GetKeywordVariables();
            KeywordRadComboBox.DataTextField = "Text";
            KeywordRadComboBox.DataValueField = "Value";
            KeywordRadComboBox.DataBind();

            DataRowView drv = e.Item.DataItem as DataRowView;
            KeywordRadComboBox.Text = drv.Row["Keyword"].ToString();
        }
    }

    /// <summary>
    /// Item command event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void KeywordRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("Delete"))
        {
            // Get the current repeater data
            DataTable data = GetRepeaterData();

            // Get the data of the row to delete
            DataRow row = data.Rows[e.Item.ItemIndex];

            data.Rows[e.Item.ItemIndex].Delete();

            SetRepeaterData(data);
        }
    }

    /// <summary>
    /// More button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void MoreButton_Click(object sender, EventArgs e)
    {
        // Get the current data in the repeater
        DataTable data = GetRepeaterData();

        // Add a new row to the repeater
        data.Rows.Add(string.Empty);

        SetRepeaterData(data);
    }
    #endregion

    #region Validation
    #endregion  

    #region Private Methods
    /// <summary>
    /// Retrieve the repeater data
    /// </summary>
    /// <returns></returns>
    private DataTable GetRepeaterData()
    {
        DataTable data = new DataTable();
        data.Columns.Add("Keyword");

        // Loop through each repeater item to get the data
        foreach (RepeaterItem item in KeywordRepeater.Items)
        {
            //string keyword = (item.FindControl("KeywordTextBox") as TextBox).Text;
            string keyword = (item.FindControl("KeywordRadComboBox") as RadComboBox).Text;
            data.Rows.Add(keyword);
        }

        return data;
    }

    /// <summary>
    /// Set the repeater data
    /// </summary>
    /// <param name="data"></param>
    private void SetRepeaterData(DataTable data)
    {
        KeywordRepeater.DataSource = data;
        KeywordRepeater.DataBind();

        // Remove the "Delete" image button if there is only 1 item in the repeater
        (KeywordRepeater.Items[0].FindControl("DeleteImageButton") as ImageButton).Visible = !(KeywordRepeater.Items.Count == 1);
        (KeywordRepeater.Items[0].FindControl("AndLiteralLabel") as Label).Visible = !(KeywordRepeater.Items.Count == 1);
    }
    #endregion
}
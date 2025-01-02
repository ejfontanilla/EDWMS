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
using System.Collections.Generic;

public partial class Maintenance_DocumentType_EditKeyword : System.Web.UI.Page
{
    bool hasManageAllAccess = false;
    bool hasManageDepartmentAccess = false;

    string docTypeCode;

    string containsAddKeywordPageUrl = "javascript:ShowWindow('AddKeyword.aspx?contains=1'," +
            Constants.WindowWidth.ToString() + "," + Constants.WindowHeight.ToString() + ");";
    string notContainsAddKeywordPageUrl = "javascript:ShowWindow('AddKeyword.aspx?contains=0'," +
            Constants.WindowWidth.ToString() + "," + Constants.WindowHeight.ToString() + ");";

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");
        WarningPanel.Visible = (Request["cfm"] == "0");

        docTypeCode = string.Empty;

        if (!String.IsNullOrEmpty(Request["code"]))
        {
            docTypeCode = Request["code"];
        }
        else
            Response.Redirect("Default.aspx");

        if (!IsPostBack)
        {
            if (!String.IsNullOrEmpty(docTypeCode))
            {
                DocTypeDb docTypeDb = new DocTypeDb();
                DocType.DocTypeDataTable docTypeDt = docTypeDb.GetDocType(docTypeCode);

                if (docTypeDt.Rows.Count > 0)
                {
                    DocType.DocTypeRow docType = docTypeDt[0];
                    TitleLabel.Text = docType.Description + " Categorisation Rules";

                    DocumentIdLabel.Text = docType.DocumentId.ToString();
                    DocumentTypeLabel.Text = docType.Description;
                }

                PopulateRuleDetails();
            }
        }

        ContainsAddRadButton.Attributes.Add("onclick", containsAddKeywordPageUrl);
        NotContainsAddRadButton.Attributes.Add("onclick", notContainsAddKeywordPageUrl);

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            CategorisationRuleDb catRuleDb = new CategorisationRuleDb();
            CategorisationRuleKeywordDb catRuleKeywordDb = new CategorisationRuleKeywordDb();

            CategorisationRuleProcessingActionEnum processAction = (ProcessingCheckBox.Checked ? CategorisationRuleProcessingActionEnum.Stop :
                CategorisationRuleProcessingActionEnum.Continue);

            bool result = false;

            // Update
            if (!String.IsNullOrEmpty(docTypeCode))
            {
                bool tempResult = false;
                int ruleId = -1;                

                // Update the categorisation rule
                tempResult = catRuleDb.Update(docTypeCode, processAction, out ruleId);

                if (tempResult)
                    result = SaveKeywords(ruleId); // Save the keywords
            }

            if (result)
                Response.Redirect(String.Format("View.aspx?code={0}&cfm=1", docTypeCode));
            else
                Response.Redirect(String.Format("View.aspx?code={0}&cfm=0", docTypeCode));
        }
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {       
        if (!String.IsNullOrEmpty(e.Argument))
        {
            // Get the parameter values
            bool isContainsKeyword = Boolean.Parse(e.Argument.Substring(e.Argument.LastIndexOf(",") + 1)); // To determine if the keyword is for 'contains' or 'not contains'
            string condition = e.Argument.Substring(0, e.Argument.LastIndexOf(","));

            RadListBox control = (isContainsKeyword ? ContainsRadListBox : NotContainsRadListBox);

            if (control != null)
            {
                // Add "or" to the last item in the list box
                if (control.Items.Count > 0 && !control.Items[control.Items.Count - 1].Text.EndsWith(CategorisationRuleOpeatorEnum.or.ToString()))
                    control.Items[control.Items.Count - 1].Text += " " + CategorisationRuleOpeatorEnum.or.ToString();

                // Add the new item
                control.Items.Add(new RadListBoxItem(Format.FormatKeywords(condition), condition));
            }
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// Keyword list custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void ContainsListCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (ContainsRadListBox.Items.Count > 0);
    }
    #endregion  

    #region Private Methods
    /// <summary>
    /// Populate rule details
    /// </summary>
    private void PopulateRuleDetails()
    {
        CategorisationRuleDb categorisationRuleDb = new CategorisationRuleDb();
        CategorisationRuleKeywordDb catRuleDb = new CategorisationRuleKeywordDb();

        CategorisationRule.CategorisationRuleDataTable dt = categorisationRuleDb.GetCategorisationRule(docTypeCode);

        if (dt.Rows.Count > 0)
        {
            CategorisationRule.CategorisationRuleRow dr = dt[0];

            ProcessingCheckBox.Checked = (dr.ProcessingAction.Equals(CategorisationRuleProcessingActionEnum.Stop.ToString()));

            ContainsRadListBox.DataSource = catRuleDb.GetKeyword(dr.Id, true);
            ContainsRadListBox.DataValueField = "Value";
            ContainsRadListBox.DataTextField = "Text";
            ContainsRadListBox.DataBind();

            NotContainsRadListBox.DataSource = catRuleDb.GetKeyword(dr.Id, false);
            NotContainsRadListBox.DataValueField = "Value";
            NotContainsRadListBox.DataTextField = "Text";
            NotContainsRadListBox.DataBind();  
        }
    }

    /// <summary>
    /// Get the keywords from list box
    /// </summary>
    /// <returns></returns>
    private List<string> GetKeyWords(bool isContainsKeywords)
    {
        List<string> keywords = new List<string>();

        // Get the List Box control accordingly
        RadListBox control = (isContainsKeywords ? ContainsRadListBox : NotContainsRadListBox);

        // Loop through list box items
        foreach (RadListBoxItem keyword in control.Items)
            keywords.Add(keyword.Value);

        return keywords;
    }

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasManageAllAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);
        hasManageDepartmentAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_Department);

        bool hasAccess = (hasManageAllAccess || hasManageDepartmentAccess);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }

    /// <summary>
    /// Save the keywords
    /// </summary>
    /// <returns></returns>
    private bool SaveKeywords(int ruleId)
    {
        CategorisationRuleKeywordDb catRuleKeywordDb = new CategorisationRuleKeywordDb();

        bool result = false;
        bool tempResult = false;

        // Get the 'contains' keywords and insert into the database
        List<string> keywords = new List<string>();
        keywords = GetKeyWords(true);
        tempResult = catRuleKeywordDb.Insert(ruleId, keywords, true);

        result = tempResult;

        // Get the 'not contains' keywords 
        keywords = GetKeyWords(false);

        // Insert the keywords if any
        if (tempResult)
        {
            tempResult = catRuleKeywordDb.Insert(ruleId, keywords, false);
            result = tempResult;
        }

        return result;
    }
    #endregion
}
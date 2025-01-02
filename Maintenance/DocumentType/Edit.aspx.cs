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

public partial class Maintenance_DocumentType_Edit : System.Web.UI.Page
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

        AddRowButton.OnClientClick = String.Format("javascript:ShowWindow('AddMetaField.aspx?code={0}', 700, 500)", docTypeCode);

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
                }

                DisplaySampleDocuments();
                PopulateDocumentTypes(true);
                DocTypeDropDownList.SelectedValue = docTypeCode;
                DocTypeDropDownList.Enabled = false;

                PopulateRuleDetails();

                PopulateMeta();
            }
            else
            {
                PopulateDocumentTypes(false);
                DocTypeDropDownList.Focus();
                DocTypeDropDownList.Enabled = true;
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
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            SamplePageDb samplePageDb = new SamplePageDb();
            SampleDocDb sampleDocDb = new SampleDocDb();

            // Save the sample documents for the document type
            foreach (UploadedFile file in DocumentRadAsyncUpload.UploadedFiles)
            {
                byte[] bytes = new byte[file.ContentLength];
                file.InputStream.Read(bytes, 0, file.ContentLength);
                sampleDocDb.Insert((String.IsNullOrEmpty(docTypeCode) ? DocTypeDropDownList.SelectedValue : docTypeCode), file.FileName, bytes);
            }

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
            // Insert
            else
            {
                bool tempResult = false;
                int ruleId = -1;

                // Get the doc type
                docTypeCode = DocTypeDropDownList.SelectedValue;

                // Insert the categorisation rule
                tempResult = catRuleDb.Insert(docTypeCode, processAction, out ruleId);

                if (tempResult)
                    result = SaveKeywords(ruleId); // Save the keywords
            }

            //Update the Metadata Fields
            foreach (RepeaterItem ri in MetaFieldRepeater.Items)
            {
                HiddenField IdHidden = (ri.FindControl("IdHidden") as HiddenField);
                HiddenField DocTypeCodeHidden = (ri.FindControl("DocTypeCodeHidden") as HiddenField);
                Label FieldNameLabel = (ri.FindControl("FieldNameLabel") as Label);
                CheckBox MandatoryVerCheckBox = (ri.FindControl("MandatoryVerCheckBox") as CheckBox);
                CheckBox MandatoryComManCheckBox = (ri.FindControl("MandatoryComManCheckBox") as CheckBox);
                CheckBox VisibleVerCheckBox = (ri.FindControl("VisibleVerCheckBox") as CheckBox);
                CheckBox VisibleComCheckBox = (ri.FindControl("VisibleComCheckBox") as CheckBox);
                TextBox MaximumLengthTextBox = (ri.FindControl("MaximumLengthTextBox") as TextBox);

                MetaFieldDb metaFieldDb = new MetaFieldDb();

                try
                {
                    metaFieldDb.Update(int.Parse(IdHidden.Value), FieldNameLabel.Text.Trim(), int.Parse(MaximumLengthTextBox.Text.Trim()), MandatoryComManCheckBox.Checked, VisibleComCheckBox.Checked, MandatoryVerCheckBox.Checked, VisibleVerCheckBox.Checked, DocTypeCodeHidden.Value.Trim());
                }
                catch (Exception)
                {

                    Response.Write(IdHidden.Value);
                }

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
        else
            PopulateMeta();
    }

    protected void MetaFieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = e.Item as RepeaterItem;

            HyperLink EditHyperLink = (item.FindControl("EditHyperLink") as HyperLink);
            Label SeperatorLabel = (item.FindControl("SeperatorLabel") as Label);
            LinkButton DeleteLink = (item.FindControl("DeleteLink") as LinkButton);

            DataRowView drv = e.Item.DataItem as DataRowView;

            //set the fields
            HiddenField IdHidden = (item.FindControl("IdHidden") as HiddenField);
            HiddenField DocTypeHidden = (item.FindControl("DocTypeCodeHidden") as HiddenField);
            Label FieldNameLabel = (item.FindControl("FieldNameLabel") as Label);
            CheckBox MandatoryVerCheckBox = (item.FindControl("MandatoryVerCheckBox") as CheckBox);
            CheckBox MandatoryComManCheckBox = (item.FindControl("MandatoryComManCheckBox") as CheckBox);
            CheckBox VisibleVerCheckBox = (item.FindControl("VisibleVerCheckBox") as CheckBox);
            CheckBox VisibleComCheckBox = (item.FindControl("VisibleComCheckBox") as CheckBox);
            TextBox MaximumLengthTextBox = (item.FindControl("MaximumLengthTextBox") as TextBox);

            MandatoryVerCheckBox.Checked = Convert.ToBoolean(drv.Row["VerificationMandatory"].ToString().Trim());
            MandatoryComManCheckBox.Checked = Convert.ToBoolean(drv.Row["CompletenessMandatory"].ToString().Trim());
            VisibleVerCheckBox.Checked = Convert.ToBoolean(drv.Row["VerificationVisible"].ToString().Trim());
            VisibleComCheckBox.Checked = Convert.ToBoolean(drv.Row["CompletenessVisible"].ToString().Trim());
            MaximumLengthTextBox.Text = drv.Row["MaximumLength"].ToString();
            FieldNameLabel.Text = drv.Row["FieldName"].ToString().Trim();
            IdHidden.Value = drv.Row["Id"].ToString();
            DocTypeHidden.Value = drv.Row["DocTypeCode"].ToString().Trim();


            EditHyperLink.NavigateUrl = String.Format("javascript:ShowWindow('AddMetaField.aspx?id={0}', 700, 500)", drv.Row["Id"].ToString());

            //make edit and delet invisible for 
            EditHyperLink.Visible = MaximumLengthTextBox.Enabled = !(Convert.ToBoolean(drv.Row["Fixed"].ToString()));
            SeperatorLabel.Visible = EditHyperLink.Visible;
            DeleteLink.Visible = EditHyperLink.Visible;

        }
    }

    protected void MetaFieldRepeate_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        MetaFieldDb metaFieldDb = new MetaFieldDb();

        if (e.CommandName == "Delete")
        {
            int id = int.Parse(e.CommandArgument.ToString());
            bool success = metaFieldDb.Delete(id);
        }
        PopulateMeta();
    }

    /// <summary>
    /// Repeater item command event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        // Check type of item
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName.Equals("Delete"))
            {
                SampleDocDb sampleDocDb = new SampleDocDb();
                sampleDocDb.Delete(id);

                DisplaySampleDocuments();
            }
        }
    }

    protected void EditSampleDocsButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(String.Format("EditSampleDoc.aspx?code={0}", docTypeCode));
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
    /// Populate document types
    /// </summary>
    private void PopulateDocumentTypes(bool visibility)
    {
        DocTypeDb docTypeDb = new DocTypeDb();
        DocTypeDropDownList.DataSource = docTypeDb.GetDocTypeWithAbbreviation(visibility);
        DocTypeDropDownList.DataTextField = "Description";
        DocTypeDropDownList.DataValueField = "Code";
        DocTypeDropDownList.DataBind();
    }

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
        ContainsAddRadButton.Visible = hasAccess;
        NotContainsAddRadButton.Visible = hasAccess;
    }

    private void PopulateMeta()
    {
        MetaFieldDb metaFieldDb = new MetaFieldDb();
        MetaFieldRepeater.DataSource = metaFieldDb.GetMetaFieldByDocTypeCode(docTypeCode);
        MetaFieldRepeater.DataBind();
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

    /// <summary>
    /// Display the sample documents for the given document type
    /// </summary>
    /// <param name="version"></param>
    private void DisplaySampleDocuments()
    {
        SampleDocDb sampleDocDb = new SampleDocDb();

        if (!String.IsNullOrEmpty(docTypeCode))
        {
            DocumentRepeater.DataSource = sampleDocDb.GetDataByDocType(docTypeCode);
            DocumentRepeater.DataBind();
        }
    }
    #endregion   
}
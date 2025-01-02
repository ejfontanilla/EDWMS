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

public partial class Maintenance_DocumentType_View : System.Web.UI.Page
{
    bool hasManageAllAccess = false;
    bool hasManageDepartmentAccess = false;
    string docTypeCode;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");
        SampleDocsConfirmPanel.Visible = (Request["cfm"] == "2");
        MetaFieldConfirmPanel.Visible = (Request["cfm"] == "3");
        WarningPanel.Visible = (Request["cfm"] == "0");

        docTypeCode = string.Empty;

        if (!String.IsNullOrEmpty(Request["code"]))
        {
            docTypeCode = Request["code"];
        }

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
                    DocumentTypeLabel.Text = docType.Description;

                    DocumentIdLabel.Text = docType.DocumentId.ToString();

                    //Added By Edward 26/3/2014
                    lblAcquire.Text = !docType.IsAcquireNewSamplesNull() ? (docType.AcquireNewSamples ? "Yes" : "No") : "Yes";

                    //Added by Edward Displaying Active DocTypes 2015/8/17
                    lblIsActive.Text = !docType.IsIsActiveNull() ? (docType.IsActive ? "Yes" : "No") : "Yes";
                }

                DisplaySampleDocuments();
                PopulateRuleDetails();
                ToggleDeleteButton();
                PopulateMeta();
            }
        }

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Edit button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EditButton_Click(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(docTypeCode))
            Response.Redirect(String.Format("Edit.aspx?code={0}", docTypeCode));
    }

    /// <summary>
    /// Delete event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Delete(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(docTypeCode))
        {
            CategorisationRuleDb catRuleDb = new CategorisationRuleDb();
            catRuleDb.Delete(docTypeCode);

            Response.Redirect("Default.aspx");
        }
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
        SampleDocsSubmitPanel.Visible = hasAccess;
        KeywordSubmitPanel.Visible = hasAccess;
        MetaSubmitPanel.Visible = hasAccess;
    }

    /// <summary>
    /// Item Bound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void MetaFieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = e.Item as RepeaterItem;

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
        }
    }

    /// <summary>
    /// Repeater item command event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        //// Check type of item
        //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //{
        //    int submissionAttachmentId = int.Parse(e.CommandArgument.ToString());

        //    if (e.CommandName.Equals("Delete"))
        //    {
        //        ArfAttachmentDb approvalRequestAttachmentDb = new ArfAttachmentDb();
        //        approvalRequestAttachmentDb.Delete(submissionAttachmentId);

        //        DisplayAttachments();
        //    }
        //}
    }

    /// <summary>
    /// Edit sample docs button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EditSampleDocsButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(String.Format("EditSampleDoc.aspx?code={0}", docTypeCode));
    }

    /// <summary>
    /// Edit keyword button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EditKeywordButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(String.Format("EditKeyword.aspx?code={0}", docTypeCode));
    }

    /// <summary>
    /// Edit meta fields button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EditMetaButton_Click(object sender, EventArgs e)
    {
        Response.Redirect(String.Format("EditMetaField.aspx?code={0}", docTypeCode));
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

            //DocumentTypeLabel.Text = dr.DocTypeCode;

            ActionLabel.Text = (dr.ProcessingAction.Equals(CategorisationRuleProcessingActionEnum.Stop.ToString()) ? Constants.RulesProcessingActionStop : Constants.RulesProcessingActionContinue);

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
    /// Toggle the visibility of the delete button
    /// </summary>
    private void ToggleDeleteButton()
    {
        if (!String.IsNullOrEmpty(docTypeCode))
        {
            DocTypeDb docTypeDb = new DocTypeDb();
            //DeleteButton.Visible = docTypeDb.IsDocTypeEditable(docTypeCode);
        }
    }

    /// <summary>
    /// Pupulate Meta
    /// </summary>
    private void PopulateMeta()
    {
        MetaFieldDb metaFieldDb = new MetaFieldDb();
        MetaFieldRepeater.DataSource = metaFieldDb.GetMetaFieldByDocTypeCode(docTypeCode);
        MetaFieldRepeater.DataBind();
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

        NoSampleDocumentLabel.Visible = (DocumentRepeater.Items.Count <= 0);
    }
    #endregion
}


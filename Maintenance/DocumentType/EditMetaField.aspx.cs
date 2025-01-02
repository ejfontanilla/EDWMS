using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;


public partial class Maintenance_DocumentType_EditMetaFields : System.Web.UI.Page
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
                    TitleLabel.Text = docType.Description + " Metadata Fields";

                    DocumentIdLabel.Text = docType.DocumentId.ToString();
                    DocumentTypeLabel.Text = docType.Description;
                }

                PopulateMeta();
            }
        }

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

            Response.Redirect(String.Format("View.aspx?code={0}&cfm=3", docTypeCode));
        }
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {       
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
    #endregion

    #region Validation
    #endregion  

    #region Private Methods
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

    private void PopulateMeta()
    {
        MetaFieldDb metaFieldDb = new MetaFieldDb();
        MetaFieldRepeater.DataSource = metaFieldDb.GetMetaFieldByDocTypeCode(docTypeCode);
        MetaFieldRepeater.DataBind();
    }
    #endregion   
}
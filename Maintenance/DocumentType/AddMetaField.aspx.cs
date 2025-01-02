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

public partial class Maintenance_DocumentType_AddMetaField : System.Web.UI.Page
{
    int? id;
    string docTypeCode;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        docTypeCode = string.Empty;


        if (string.IsNullOrEmpty(Request["id"]))
        {
            id = null;

            //check for DocTypeCode
            if (!String.IsNullOrEmpty(Request["code"]))
                docTypeCode = Request["code"];
            else
                Response.Redirect("Default.aspx");
        }
        else
        {
            id = int.Parse(Request["id"]);
        }


        if (!IsPostBack)
        {
            if (id == null)
            {
                FieldNameTextBox.Focus();
                DocTypeCodeHidden.Value = docTypeCode;
            }
            else
            {
                TitleLabel.Text = "Edit Meta Field";

                MetaFieldDb metaFieldDb = new MetaFieldDb();

                MetaField.MetaFieldDataTable metaField = metaFieldDb.GetMetaFieldById(id.Value);

                if (metaField.Count == 0)
                {
                    Response.Redirect("~/Default.aspx");
                }

                MetaField.MetaFieldRow row = metaField[0];

                FieldNameTextBox.Text = row.FieldName.Trim();
                MandatoryVerCheckBox.Checked = row.VerificationMandatory;
                MandatoryVComCheckBox.Checked = row.CompletenessMandatory;
                VisibleVerCheckBox.Checked = row.VerificationVisible;
                VisibleComCheckBox.Checked = row.CompletenessVisible;
                MaximumLengthTextBox.Text = row.MaximumLength.ToString().Trim();
                DocTypeCodeHidden.Value = row.DocTypeCode.Trim();
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
        Page.Validate();

        if (Page.IsValid)
        {
            MetaFieldDb metaFieldDb = new MetaFieldDb();

            if (id == null)
                id = metaFieldDb.Insert(FieldNameTextBox.Text.Trim(), int.Parse(MaximumLengthTextBox.Text), MandatoryVComCheckBox.Checked, VisibleComCheckBox.Checked, MandatoryVerCheckBox.Checked, VisibleVerCheckBox.Checked, DocTypeCodeHidden.Value.Trim().ToUpper());
            else
                metaFieldDb.Update(id.Value, FieldNameTextBox.Text.Trim(), int.Parse(MaximumLengthTextBox.Text), MandatoryVComCheckBox.Checked, VisibleComCheckBox.Checked, MandatoryVerCheckBox.Checked, VisibleVerCheckBox.Checked, DocTypeCodeHidden.Value.Trim().ToUpper());

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;
            SubmitPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", "ResizeAndClose(600, 190);", true);
        }
    }

    #endregion
}
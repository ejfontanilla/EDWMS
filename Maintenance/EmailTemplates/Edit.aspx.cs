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
using Dwms.Web;

public partial class Hq_Administration_EmailTemplates_Edit : System.Web.UI.Page
{
    int? id;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(Request["id"]))
            id = int.Parse(Request["id"]);

        if (!IsPostBack)
        {
            if (id.HasValue)
            {
                PopulateTemplateDetails();
            }
        }

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Submit button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
            
            string description = DescriptionTextBox.Text.Trim();
            string subject = Subject.Text.Trim().Replace("<", "[%").Replace(">", "%]");
            string content = ContentTextBox.Text.Trim().Replace("<", "[%").Replace(">", "%]");

            if (id.HasValue)
                emailTemplateDb.Update(id.Value, string.Empty, description, subject, content);

            Response.Redirect("View.aspx?id=" + id.Value + "&cfm=1");
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// Validate content
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void ContentCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {        
        args.IsValid = (!String.IsNullOrEmpty(ContentTextBox.Text.Trim()));
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate template details
    /// </summary>
    private void PopulateTemplateDetails()
    {
        EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
        EmailTemplate.EmailTemplateDataTable dt = emailTemplateDb.GetEmailTemplate(id.Value);

        if (dt.Rows.Count > 0)
        {
            EmailTemplate.EmailTemplateRow dr = dt[0];

            TitleLabel.Text = dr.TemplateDescription;
            DescriptionTextBox.Text = dr.TemplateDescription;
            Subject.Text = dr.Subject;     
            ContentTextBox.Text = dr.Content;
        }
        else
            Response.Redirect("Default.aspx");
    }

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }
    #endregion
}

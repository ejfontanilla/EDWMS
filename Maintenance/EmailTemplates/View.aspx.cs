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

public partial class Maintenance_EmailTemplates_View : System.Web.UI.Page
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
        ConfirmPanel.Visible = (!String.IsNullOrEmpty(Request["cfm"]) && Request["cfm"].Equals("1"));

        if (!String.IsNullOrEmpty(Request["id"]))
            id = int.Parse(Request["id"]);

        if (!IsPostBack)
        {
            if (id.HasValue)
            {
                PopulateTemplateDetails();
            }
            else
                Response.Redirect("Default.aspx");
        }

        // Set the access control of the user
        SetAccessControl();
    }

    /// <summary>
    /// Delete button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DeleteButton_Click(object sender, EventArgs e)
    {
        //EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
        //bool success = emailTemplateDb.Delete(id.Value);

        //if (success)
        //    Response.Redirect("Default.aspx");
        //else
        //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
        //        "DeleteFailScript", "alert('" + Constants.OperationFailedErrorMessage + "');", true);
    }    

    /// <summary>
    /// Edit button click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EditButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Edit.aspx?id=" + id.Value.ToString());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the template details
    /// </summary>
    private void PopulateTemplateDetails()
    {
        EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
        EmailTemplate.EmailTemplateDataTable dt = emailTemplateDb.GetEmailTemplate(id.Value);

        if (dt.Rows.Count > 0)
        {
            EmailTemplate.EmailTemplateRow dr = dt[0];

            TitleLabel.Text = dr.TemplateDescription;
            DescriptionLabel.Text = dr.TemplateDescription;
            SubjectLabel.Text = dr.Subject;
            ContentLabel.Text = dr.Content.Trim().Replace(Environment.NewLine, "<br />");
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


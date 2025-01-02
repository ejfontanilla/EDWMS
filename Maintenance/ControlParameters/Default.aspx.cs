using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Web;
using Dwms.Bll;

public partial class Maintenance_ControlParameters_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");

        if (!IsPostBack)
        {
            ParameterDb parameterDb = new ParameterDb();

            ArchiveAudit.DataSource = EnumManager.EnumToDataTable(typeof(ArchiveAuditEnum));
            ArchiveAudit.DataBind();

            SystemEmail.Text = parameterDb.GetParameter(ParameterNameEnum.SystemEmail);
            ArchiveAudit.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.ArchiveAudit).Replace(" ","_");
            BatchJobMailingList.Text = parameterDb.GetParameter(ParameterNameEnum.BatchJobMailingList);
            TestMailingList.Text = parameterDb.GetParameter(ParameterNameEnum.TestMailingList);
            RedirectAllEmailsToTestMailingList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.RedirectAllEmailsToTestMailingList);
            AuthenticationModeRadioButtonList.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.AuthenticationMode);
            ErrorNotificationMailingList.Text = parameterDb.GetParameter(ParameterNameEnum.ErrorNotificationMailingList);
            EnableErrorNotification.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.EnableErrorNotification);
        }

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Save parameter event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            ParameterDb parameterDb = new ParameterDb();

            parameterDb.Update(ParameterNameEnum.SystemEmail, SystemEmail.Text.Trim());
            parameterDb.Update(ParameterNameEnum.ArchiveAudit, ArchiveAudit.SelectedValue.Replace("_", " "));
            parameterDb.Update(ParameterNameEnum.BatchJobMailingList, BatchJobMailingList.Text.Trim());
            parameterDb.Update(ParameterNameEnum.TestMailingList, TestMailingList.Text.Trim());
            parameterDb.Update(ParameterNameEnum.RedirectAllEmailsToTestMailingList, RedirectAllEmailsToTestMailingList.Text.Trim());
            parameterDb.Update(ParameterNameEnum.AuthenticationMode, AuthenticationModeRadioButtonList.SelectedValue);
            parameterDb.Update(ParameterNameEnum.ErrorNotificationMailingList, ErrorNotificationMailingList.Text.Trim());
            parameterDb.Update(ParameterNameEnum.EnableErrorNotification, EnableErrorNotification.SelectedValue);

            Response.Redirect(Retrieve.GetPageName() + "?cfm=1");
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// Batch job mailing list custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void BatchJobMailingListCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (BatchJobMailingListRequiredFieldValidator.IsValid)
        {
            args.IsValid = Validation.IsValidEmail(BatchJobMailingList.Text.Trim());
        }
    }

    /// <summary>
    /// Test mailing list custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void TestMailingListCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (TestMailingListRequiredFieldValidator.IsValid)
        {
            args.IsValid = Validation.IsValidEmail(TestMailingList.Text.Trim());
        }
    }

    /// <summary>
    /// System email custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void SystemEmailCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (EmailRequiredValidator.IsValid)
        {
            args.IsValid= Validation.IsEmail(SystemEmail.Text.Trim());
        }
    }

    protected void ErrorNotificationMailingListCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (ErrorNotificationMailingListRequiredFieldValidator.IsValid)
        {
            args.IsValid = Validation.IsValidEmail(ErrorNotificationMailingList.Text.Trim());
        }
    }
    #endregion

    #region Private Methods
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

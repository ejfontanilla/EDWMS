using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Controls_Email : System.Web.UI.UserControl
{
    public string RequiredFieldErrorMessage
    {
        get
        {
            return RequiredFieldValidatorEmail.ErrorMessage;
        }
        set
        {
            RequiredFieldValidatorEmail.ErrorMessage = value;
        }
    }

    public Boolean EnableRequiredFieldValidation
    {
        get
        {
            return RequiredFieldValidatorEmail.Enabled;
        }
        set
        {
            RequiredFieldValidatorEmail.Enabled = value;
        }
    }

    public Boolean EnableFormatValidation
    {
        get
        {
            return RegularExpressionValidatorEmail.Enabled;
        }
        set
        {
            RegularExpressionValidatorEmail.Enabled = value;
        }
    }

    public Unit Width
    {
        get
        {
            return Email.Width;
        }
        set
        {
            Email.Width = value;
        }
    }

    public string EmailValue
    {
        get
        {
            return Email.Text;
        }
        set
        {
            Email.Text = value;
        }
    }

    public string FormatValidationEmailErrorMessage
    {
        get
        {
            return RegularExpressionValidatorEmail.ErrorMessage;
        }
        set
        {
            RegularExpressionValidatorEmail.ErrorMessage = value;
        }
    }

    public TextBox EmailTextBox
    {
        get
        {
            return Email;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
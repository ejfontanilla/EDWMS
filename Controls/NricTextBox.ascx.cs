using System;
using System.Web.Security;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Controls_Nric : System.Web.UI.UserControl
{
    public string RequiredFieldErrorMessage
    {
        get
        {
            return RequiredFieldValidatorNric.ErrorMessage;
        }
        set
        {
            RequiredFieldValidatorNric.ErrorMessage = value;
        }
    }

    public string ValidationGroupNric
    {
        get
        {
            return Nric.ValidationGroup;
        }
        set
        {
            Nric.ValidationGroup = value;
        }
    }

    public Boolean NricEnabled
    {
        get
        {
            return Nric.Enabled;
        }
        set
        {
            Nric.Enabled = value;
        }
    }

    public Boolean EnableRequiredField
    {
        get
        {
            return RequiredFieldValidatorNric.Enabled;
        }
        set
        {
            RequiredFieldValidatorNric.Enabled = value;
        }
    }

    public string NricValue
    {
        get
        {
            return Nric.Text.ToUpper();
        }
        set
        {
            Nric.Text = value.ToUpper();
        }
    }

    public string CustomValidatorErrorMessage
    {
        get
        {
            return NricFinCustomValidator.ErrorMessage;
        }
        set
        {
            NricFinCustomValidator.ErrorMessage = value;
        }
    }

    public Boolean EnableCustomValidator
    {
        get
        {
            return NricFinCustomValidator.Enabled;
        }
        set
        {
            NricFinCustomValidator.Enabled = value;
        }
    }

    public string NricIdValue
    {
        get
        {
            return NricId.Value;
        }
        set
        {
            NricId.Value = value;
        }
    }

    public TextBox NricTextBox
    {
        get
        {
            return Nric;
        }
    }

    public HiddenField NricIdHiddenField
    {
        get
        {
            return NricId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {
        if ((args.Value.ToUpper().StartsWith("S") || args.Value.ToUpper().StartsWith("T") || args.Value.ToUpper().StartsWith("F") || args.Value.ToUpper().StartsWith("G")) && args.Value.Length == 9)
        {
            args.IsValid = (Validation.IsNric(args.Value.ToString().Trim()) || Validation.IsFin(args.Value.ToString().Trim()));
        }
    }
    protected void Nric_TextChanged(object sender, EventArgs e)
    {
        Nric.Text = Nric.Text.ToUpper();
    }
}
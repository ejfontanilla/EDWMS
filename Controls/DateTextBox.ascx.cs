using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Controls_Date : System.Web.UI.UserControl
{
    public string RequiredFieldErrorMessage
    {
        get
        {
            return RequiredFieldValidatorDate.ErrorMessage;
        }
        set
        {
            RequiredFieldValidatorDate.ErrorMessage = value;
        }
    }

    public Boolean EnableRequiredField
    {
        get
        {
            return RequiredFieldValidatorDate.Enabled;
        }
        set
        {
            RequiredFieldValidatorDate.Enabled = value;
        }
    }

    public string DateValue
    {
        get
        {
            return DateTextBox.Text.Trim();
        }
        set
        {
            DateTextBox.Text = value;
        }
    }

    public string DateIdValue
    {
        get
        {
            return DateId.Value.Trim();
        }
        set
        {
            DateId.Value = value;
        }
    }

    public HiddenField DateIdHiddenField
    {
        get
        {
            return DateId;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void CustomValidator_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        DateTime d;
        e.IsValid = DateTime.TryParseExact(e.Value.Trim(), Format.GetMetaDataDateFormat(), CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
        CustomValidator.ErrorMessage = Constants.InvalidDateFormat;

        if (e.IsValid)
        {
            e.IsValid = d.Year > 1900 && d.Year <= 2050;
            CustomValidator.ErrorMessage = Constants.InvalidDateFormatInYear;
        }

    }

    protected void DateTextBox_TextChanged(object sender, EventArgs e)
    {
        DateTextBox.Text = Format.GetMetaDataValueInMetaDataDateFormat(DateTextBox.Text);
        //DateTextBox.Focus();
    }
 }
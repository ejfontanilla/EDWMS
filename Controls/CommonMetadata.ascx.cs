using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Controls_CommonMedata : System.Web.UI.UserControl
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
            RequiredFieldValidatorName.Enabled = value;
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

    public string NricValue
    {
        get
        {
            return Nric.Text;
        }
        set
        {
            Nric.Text = value;
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

    public string NameValue
    {
        get
        {
            return Name.Text;
        }
        set
        {
            Name.Text = value;
        }
    }

    public string NameIdValue
    {
        get
        {
            return NameId.Value;
        }
        set
        {
            NameId.Value = value;
        }
    }

    public TextBox NameTextBox
    {
        get
        {
            return Name;
        }
    }

    public string IDTypeValue
    {
        get
        {
            return IDType.SelectedValue;
        }
        set
        {
            IDType.SelectedValue = value;
        }
    }

    public string IDTypeIdValue
    {
        get
        {
            return IDTypeId.Value;
        }
        set
        {
            IDTypeId.Value = value;
        }
    }

    public RadioButtonList IDTypeRadioButtonList
    {
        get
        {
            return IDType;
        }
    }

    public string CustomerSourceIdIdValue
    {
        get
        {
            return CustomerSourceId.Value;
        }
        set
        {
            CustomerSourceId.Value = value;
        }
    }

    public HiddenField CustomerSourceIdHiddenField
    {
        get
        {
            return CustomerSourceId;
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
        if (!IsPostBack)
        {
            PopulateIDType();
        }
    }

    protected void PopulateIDType()
    {
        IDType.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDType.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        
        IDType.DataBind();
    }

    public void Clear()
    {
        Nric.Text = Name.Text = string.Empty;
        PopulateIDType();
        IDType.ClearSelection();
        IDType.DataBind();
        NricId.Value = NameId.Value = IDTypeId.Value = string.Empty;
    }

    public void RestrictAccess(Boolean isUnderHouseholdStructure)
    {
        CustomerDetailsPanel.Visible = Name.Enabled = Nric.Enabled = IDType.Enabled =  NricFinCustomValidator.Enabled = !isUnderHouseholdStructure;
    }

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {
        //args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDType, args.Value.ToString().Trim());
        bool isValid = true;

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        Nric.Text = Validation.ReplaceSpecialCharacters(Nric.Text);        

        #region modified by Edward 2015/09/25 Investigate UIN Type Blank after deployment of NO UIN
        if (!string.IsNullOrEmpty(args.Value.ToString().Trim()) && string.IsNullOrEmpty(IDType.SelectedValue))
        {
            NricFinCustomValidator.ErrorMessage = "Please select an Id Type.";
            isValid = false;
        }
        else if (!Validation.ValidateUINFINXINBasedOnIDType(IDType.SelectedValue, args.Value.ToString().Trim()))
        {
            NricFinCustomValidator.ErrorMessage = "Please enter a valid Id No.";
            isValid = false;
        }

        args.IsValid = isValid;
        #endregion
    }

    protected void IDTypeCustomValidator(object source, ServerValidateEventArgs args)
    {
        bool isValid = true;
        if (!string.IsNullOrEmpty(args.Value.ToString().Trim()) && string.IsNullOrEmpty(Nric.Text.Trim()))
        {
            IDTypeValidator.ErrorMessage = "Please enter Id No.";
            isValid = false;
        }
        //else if (!Validation.ValidateUINFINXINBasedOnIDType(IDType, Nric.Text.Trim()))
        //    isValid = false;
        args.IsValid = isValid;
    }


    protected void Nric_TextChanged(object sender, EventArgs e)
    {
        Nric.Text = Nric.Text.ToUpper();                

        #region added by edward 09.01.2014
        CheckNRIC();
        #endregion
    }
    protected void IDType_SelectedIndexChanged(object sender, EventArgs e)
    {
        #region added by edward 09.01.2014
        CheckNRIC();
        #endregion
    }
    #region added by edward 09.01.2014
    private void CheckNRIC()
    {
        if (!Validation.ValidateUINFINXINBasedOnIDType(IDType.SelectedValue, Nric.Text.Trim()))
            lblInvalid.Visible = true;
        else
            lblInvalid.Visible = false;
    }
    #endregion

    //Added by Edward 2016/06/10 To Take out non letter characters for Name
    protected void Name_CustomValidator(object source, ServerValidateEventArgs args)
    {        
        bool isValid = true;
        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        Name.Text = Validation.ReplaceNonLetterCharacters(Name.Text);        
        args.IsValid = isValid;
    }
}
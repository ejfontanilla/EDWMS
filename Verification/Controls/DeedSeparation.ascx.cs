using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_DeedSeparation : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DeedSeparation";

    public string CurrentDocType
    {
        get
        {
            return _CurrentDocType;
        }
        set
        {
            _CurrentDocType = value;
        }
    }

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        lblDateRangeError.Visible = false;
        if (!IsPostBack)
        {
            PopulateIDType();
            PopulateTag();
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        DateOfSeperation.DateValue = string.Empty;
        DateOfSeperation.DateIdValue = string.Empty;
        NameOfSpouse.Text = NameOfRequestor.Text = IdentityNoRequestor.Text = IdentityNoSpouse.Text = string.Empty;
        IDTypeRequestor.ClearSelection();
        IDTypeSpouse.ClearSelection();
        NameOfSpouseId.Value = NameOfRequestorId.Value = IdentityNoRequestorId.Value = IdentityNoSpouseId.Value = string.Empty;
        IDTypeSpouseId.Value = IDTypeRequestorId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {

        ClearData();


        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();
            PopulateTag();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.DateOfSeperation.ToString().ToLower()))
                {
                    DateOfSeperation.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfSeperation.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.IdentityNoRequestor.ToString().ToLower()))
                {
                    IdentityNoRequestor.Text = metaDataRow.FieldValue;
                    IdentityNoRequestorId.Value= metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.IDTypeRequestor.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeRequestor.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeRequestor.SelectedValue = metaDataRow.FieldValue;
                    IDTypeRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.NameOfRequestor.ToString().ToLower()))
                {
                    NameOfRequestor.Text = metaDataRow.FieldValue;
                    NameOfRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeedSeparationEnum.NameOfSpouse.ToString().ToLower()))
                {
                    NameOfSpouse.Text = metaDataRow.FieldValue;
                    NameOfSpouseId.Value = metaDataRow.Id.ToString();
                }

            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public void PopulateIDType()
    {
        IDTypeRequestor.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeRequestor.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeRequestor.DataBind();

        IDTypeSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeSpouse.DataBind();

        //IDTypeRequestor.SelectedValue = IDTypeSpouse.SelectedValue = IDTypeEnum.UIN.ToString();
    }

    public void PopulateTag()
    {
        TagRadioButtonList.Items.Clear();
        foreach (TagEnum val in Enum.GetValues(typeof(TagEnum)))
        {
            TagRadioButtonList.Items.Add(new RadComboBoxItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        TagRadioButtonList.DataBind();

    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfSeperation.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorIdentityNoRequestor.Enabled = RequiredFieldValidatorIdentityNoSpouse.Enabled = RequiredFieldValidatorNameOfRequestor.Enabled = isEnabled;
    //    RequiredFieldValidatorNameOfSpouse.Enabled = NricFinCustomValidatorRequestor.Enabled = NricFinCustomValidatorSpouse.Enabled = isEnabled;
    //}
    #endregion

    //protected void SetIdType()
    //{
    //    if (IdentityNoRequestor.Text.Length > 0) RequiredFieldValidatorIDTypeRequestor.Enabled = true;
    //    else RequiredFieldValidatorIDTypeRequestor.Enabled = false;
    //    if (IdentityNoSpouse.Text.Length > 0) RequiredFieldValidatorIDTypeSpouse.Enabled = true;
    //    else RequiredFieldValidatorIDTypeSpouse.Enabled = false;
    //}
    protected void NricCustomValidatorRequestor(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeRequestor.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfSeperation.DateValue, DateTime.Now.ToString()) || !isValidate || DateOfSeperation.EnableRequiredField == false)
        {
            SaveDataDetails(docId, docRefId, referencePersonalTable, isValidate);
        }
        else
        {
            lblDateRangeError.Visible = true;
            validDate = false;
        }
        return validDate;
    }
    #endregion

    protected void IdentityNoRequestor_TextChanged(object sender, EventArgs e)
    {
        IdentityNoRequestor.Text = IdentityNoRequestor.Text.ToUpper();
        //SetIdType();
    }
    protected void IdentityNoSpouse_TextChanged(object sender, EventArgs e)
    {
        IdentityNoSpouse.Text = IdentityNoSpouse.Text.ToUpper();
        //SetIdType();
    }

    #region Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16
    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate, bool IsBlurOrIncomplete)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;

        string strDateNow = DateTime.Now.ToString();

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfSeperation.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfSeperation.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate || DateOfSeperation.EnableRequiredField == false)
        {
            SaveDataDetails(docId, docRefId, referencePersonalTable, isValidate);
        }
        else
        {
            lblDateRangeError.Visible = true;
            validDate = false;
        }
        return validDate;
    }
    #endregion

    #region Added by Edward 2016/06/15 to make SaveData maintanable
    private void SaveDataDetails(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        //Save Perosnal Data
        CustomPersonal customPersonal = new CustomPersonal();
        //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
        //Added by Edward 2016/03/23 Optimize Doctype Saving
        customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType,
            CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
        MetaDataDb metadataDb = new MetaDataDb();

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoRequestor.Text = Validation.ReplaceSpecialCharacters(IdentityNoRequestor.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfRequestor.Text = Validation.ReplaceNonLetterCharacters(NameOfRequestor.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (DateOfSeperation.DateIdValue.Length == 0)
            DateOfSeperation.DateIdValue = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.DateOfSeperation.ToString(), DateOfSeperation.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfSeperation.DateIdValue), DateOfSeperation.DateValue);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoRequestorId.Value.Length == 0)
            IdentityNoRequestorId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.IdentityNoRequestor.ToString(), IdentityNoRequestor.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoRequestorId.Value), IdentityNoRequestor.Text.ToUpper());

        if (IdentityNoSpouseId.Value.Length == 0)
            IdentityNoSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.IdentityNoSpouse.ToString(), IdentityNoSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoSpouseId.Value), IdentityNoSpouse.Text.ToUpper());

        if (NameOfRequestorId.Value.Length == 0)
            NameOfRequestorId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.NameOfRequestor.ToString(), NameOfRequestor.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfRequestorId.Value), NameOfRequestor.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeRequestorId.Value.Length == 0)
            IDTypeRequestorId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.IDTypeRequestor.ToString(), IDTypeRequestor.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeRequestorId.Value), IDTypeRequestor.SelectedValue);

        if (IDTypeSpouseId.Value.Length == 0)
            IDTypeSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDeedSeparationEnum.IDTypeSpouse.ToString(), IDTypeSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeSpouseId.Value), IDTypeSpouse.SelectedValue);
    }

    #endregion

}
using System;
using System.Web.UI.WebControls;
//using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_DeathCertificateMo : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DeathCertificateMo";

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
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        NameOfMother.Text = IdentityNoOfMother.Text = DateOfDeath.DateValue = string.Empty;
        IDType.ClearSelection();
        TagRadioButtonList.SelectedIndex = 0;
        IDTypeId.Value = TagRadioButtonListId.Value = DateOfDeath.DateIdValue = string.Empty;
        NameOfMotherId.Value = IdentityNoOfMotherId.Value = string.Empty;
        lblDateRangeError.Visible = false;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        CustomPersonal customPersonal = new CustomPersonal();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateMoEnum.DateOfDeath.ToString().ToLower()))
                {
                    DateOfDeath.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfDeath.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateMoEnum.Tag.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Equals(TagEnum.Local_Civil.ToString()) || metaDataRow.FieldValue.Equals(TagEnum.Local_Muslim.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Local.ToString();
                    else if (metaDataRow.FieldValue.Equals(TagGeneralEnum.Foreign.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Foreign.ToString();
                    //TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));   Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateMoEnum.IDType.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDType.SelectedValue = metaDataRow.FieldValue;
                    IDTypeId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateMoEnum.IdentityNoOfMother.ToString().ToLower()))
                {
                    IdentityNoOfMother.Text = metaDataRow.FieldValue;
                    IdentityNoOfMotherId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateMoEnum.NameOfMother.ToString().ToLower()))
                {
                    NameOfMother.Text = metaDataRow.FieldValue;
                    NameOfMotherId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Commented by Edward 2016/01/05 IdNo was not validated all doctype when foreign is selected
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfDeath.EnableRequiredField = RequiredFieldValidatorIdentityNoOfMother.Enabled = isEnabled;
    //    RequiredFieldValidatorNameOfMother.Enabled = NricFinCustomValidator.Enabled = isEnabled;
    //}
    #endregion

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDType.SelectedValue, args.Value.ToString().Trim());
    }

    public void PopulateIDType()
    {
        IDType.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDType.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDType.DataBind();

        //IDType.SelectedValue = IDTypeEnum.UIN.ToString(); //Added by Edward on 2015/07/31 for Investigating CBD Errors
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfDeath.DateValue, DateTime.Now.ToString()) || !isValidate || DateOfDeath.EnableRequiredField == false)
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

    #region Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16
    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate, bool IsBlurOrIncomplete)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;

        string strDateNow = DateTime.Now.ToString();

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfDeath.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfDeath.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate || DateOfDeath.EnableRequiredField == false)
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
        //save metadata fields
        MetaDataDb metadataDb = new MetaDataDb();

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoOfMother.Text = Validation.ReplaceSpecialCharacters(IdentityNoOfMother.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfMother.Text = Validation.ReplaceNonLetterCharacters(NameOfMother.Text);

        if (DateOfDeath.DateIdValue.Length == 0)
            DateOfDeath.DateIdValue = metadataDb.Insert(DocTypeMetaDataDeathCertificateMoEnum.DateOfDeath.ToString(), DateOfDeath.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfDeath.DateIdValue), DateOfDeath.DateValue);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataDeathCertificateMoEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IDTypeId.Value.Length == 0)
            IDTypeId.Value = metadataDb.Insert(DocTypeMetaDataDeathCertificateMoEnum.IDType.ToString(), IDType.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeId.Value), IDType.SelectedValue);

        if (IdentityNoOfMotherId.Value.Length == 0)
            IdentityNoOfMotherId.Value = metadataDb.Insert(DocTypeMetaDataDeathCertificateMoEnum.IdentityNoOfMother.ToString(), IdentityNoOfMother.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoOfMotherId.Value), IdentityNoOfMother.Text);

        if (NameOfMotherId.Value.Length == 0)
            NameOfMotherId.Value = metadataDb.Insert(DocTypeMetaDataDeathCertificateMoEnum.NameOfMother.ToString(), NameOfMother.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfMotherId.Value), NameOfMother.Text);
    }
    #endregion

    #endregion
}
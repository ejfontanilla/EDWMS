using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_DivorceCertNRIC : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DivorceCertNRIC";

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
        DateOfDivorce.DateValue = DivorceCaseNo.Text = string.Empty;
        DateOfDivorce.DateIdValue = DivorceCaseNoId.Value = string.Empty;
        NameOfSpouse.Text = NameOfNRIC.NricValue = IdentityNoNRIC.Text = IdentityNoSpouse.Text = string.Empty;
        IDTypeNRIC.ClearSelection();
        IDTypeSpouse.ClearSelection();
        NameOfSpouseId.Value = NameOfNRIC.NricIdValue = IdentityNoNRICId.Value = IdentityNoSpouseId.Value = string.Empty;
        IDTypeSpouseId.Value = IDTypeNRICId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.DateOfDivorce.ToString().ToLower()))
                {
                    DateOfDivorce.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfDivorce.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.DivorceCaseNo.ToString().ToLower()))
                {
                    DivorceCaseNo.Text = metaDataRow.FieldValue;
                    DivorceCaseNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.IdentityNoNRIC.ToString().ToLower()))
                {
                    IdentityNoNRIC.Text = metaDataRow.FieldValue;
                    IdentityNoNRICId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.IDTypeNRIC.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeNRIC.SelectedValue = metaDataRow.FieldValue;
                    IDTypeNRICId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.NameOfNRIC.ToString().ToLower()))
                {
                    NameOfNRIC.NricValue = metaDataRow.FieldValue;
                    NameOfNRIC.NricIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertNRICEnum.NameOfSpouse.ToString().ToLower()))
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
        IDTypeNRIC.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeNRIC.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeNRIC.DataBind();

        IDTypeSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeSpouse.DataBind();

        //IDTypeNRIC.SelectedValue = IDTypeSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
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

    protected void NricCustomValidatorNRIC(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeNRIC.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfDivorce.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorIdentityNoNRIC.Enabled = RequiredFieldValidatorIdentityNoSpouse.Enabled = NameOfNRIC.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorNameOfSpouse.Enabled = RequiredFieldValidatorDivorceCaseNo.Enabled = NricFinCustomValidatorNRIC.Enabled = NricFinCustomValidatorSpouse.Enabled = isEnabled;
    //}
    #endregion

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfDivorce.DateValue, DateTime.Now.ToString()) || !isValidate || DateOfDivorce.EnableRequiredField == false)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfDivorce.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfDivorce.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate || DateOfDivorce.EnableRequiredField == false)
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
            IdentityNoNRIC.Text = Validation.ReplaceSpecialCharacters(IdentityNoNRIC.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfNRIC.NricValue = Validation.ReplaceNonLetterCharacters(NameOfNRIC.NricValue);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (DateOfDivorce.DateIdValue.Length == 0)
            DateOfDivorce.DateIdValue = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.DateOfDivorce.ToString(), DateOfDivorce.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfDivorce.DateIdValue), DateOfDivorce.DateValue);

        if (DivorceCaseNoId.Value.Length == 0)
            DivorceCaseNoId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.DivorceCaseNo.ToString(), DivorceCaseNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DivorceCaseNoId.Value), DivorceCaseNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoNRICId.Value.Length == 0)
            IdentityNoNRICId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.IdentityNoNRIC.ToString(), IdentityNoNRIC.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoNRICId.Value), IdentityNoNRIC.Text.ToUpper());

        if (IdentityNoSpouseId.Value.Length == 0)
            IdentityNoSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.IdentityNoSpouse.ToString(), IdentityNoSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoSpouseId.Value), IdentityNoSpouse.Text.ToUpper());

        if (NameOfNRIC.NricIdValue.Length == 0)
            NameOfNRIC.NricIdValue = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.NameOfNRIC.ToString(), NameOfNRIC.NricValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfNRIC.NricIdValue), NameOfNRIC.NricValue);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeNRICId.Value.Length == 0)
            IDTypeNRICId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.IDTypeNRIC.ToString(), IDTypeNRIC.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeNRICId.Value), IDTypeNRIC.SelectedValue);

        if (IDTypeSpouseId.Value.Length == 0)
            IDTypeSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertNRICEnum.IDTypeSpouse.ToString(), IDTypeSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeSpouseId.Value), IDTypeSpouse.SelectedValue);
    }
    #endregion

    #endregion
}
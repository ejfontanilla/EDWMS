using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_DivorceCertChild : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DivorceCertChild";

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
        NameOfSpouse.Text = NameOfChild.Text = IdentityNoChild.Text = IdentityNoSpouse.Text = string.Empty;
        IDTypeChild.ClearSelection();
        IDTypeSpouse.ClearSelection();
        NameOfSpouseId.Value = NameOfChildId.Value = IdentityNoChildId.Value = IdentityNoSpouseId.Value = string.Empty;
        IDTypeSpouseId.Value = IDTypeChildId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.DateOfDivorce.ToString().ToLower()))
                {
                    DateOfDivorce.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfDivorce.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.DivorceCaseNo.ToString().ToLower()))
                {
                    DivorceCaseNo.Text = metaDataRow.FieldValue;
                    DivorceCaseNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.IdentityNoChild.ToString().ToLower()))
                {
                    IdentityNoChild.Text = metaDataRow.FieldValue;
                    IdentityNoChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.IDTypeChild.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeChild.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeChild.SelectedValue = metaDataRow.FieldValue;
                    IDTypeChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.NameOfChild.ToString().ToLower()))
                {
                    NameOfChild.Text = metaDataRow.FieldValue;
                    NameOfChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDivorceCertChildEnum.NameOfSpouse.ToString().ToLower()))
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
        IDTypeChild.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeChild.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeChild.DataBind();

        IDTypeSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeSpouse.DataBind();

        //IDTypeChild.SelectedValue = IDTypeSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
    }

    protected void NricCustomValidatorChild(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeChild.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeSpouse.SelectedValue, args.Value.ToString().Trim());
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
    //    CommonMetadata.EnableRequiredField = DateOfDivorce.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorIdentityNoChild.Enabled = RequiredFieldValidatorIdentityNoSpouse.Enabled = RequiredFieldValidatorNameOfChild.Enabled = isEnabled;
    //    RequiredFieldValidatorNameOfSpouse.Enabled = RequiredFieldValidatorDivorceCaseNo.Enabled = NricFinCustomValidatorRequestor.Enabled = NricFinCustomValidatorSpouse.Enabled = isEnabled;
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

        if (DateOfDivorce.DateIdValue.Length == 0)
            DateOfDivorce.DateIdValue = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.DateOfDivorce.ToString(), DateOfDivorce.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfDivorce.DateIdValue), DateOfDivorce.DateValue);

        if (DivorceCaseNoId.Value.Length == 0)
            DivorceCaseNoId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.DivorceCaseNo.ToString(), DivorceCaseNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DivorceCaseNoId.Value), DivorceCaseNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoChild.Text = Validation.ReplaceSpecialCharacters(IdentityNoChild.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfChild.Text = Validation.ReplaceNonLetterCharacters(NameOfChild.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (IdentityNoChildId.Value.Length == 0)
            IdentityNoChildId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.IdentityNoChild.ToString(), IdentityNoChild.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoChildId.Value), IdentityNoChild.Text.ToUpper());     

        if (IdentityNoSpouseId.Value.Length == 0)
            IdentityNoSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.IdentityNoSpouse.ToString(), IdentityNoSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoSpouseId.Value), IdentityNoSpouse.Text.ToUpper());

        if (NameOfChildId.Value.Length == 0)
            NameOfChildId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.NameOfChild.ToString(), NameOfChild.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfChildId.Value), NameOfChild.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeChildId.Value.Length == 0)
            IDTypeChildId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.IDTypeChild.ToString(), IDTypeChild.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeChildId.Value), IDTypeChild.SelectedValue);

        if (IDTypeSpouseId.Value.Length == 0)
            IDTypeSpouseId.Value = metadataDb.Insert(DocTypeMetaDataDivorceCertChildEnum.IDTypeSpouse.ToString(), IDTypeSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeSpouseId.Value), IDTypeSpouse.SelectedValue);
    }

    #endregion

    #endregion
}
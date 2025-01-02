using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;


public partial class Verification_Controls_MarriageCertSibling : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "MarriageCertSibling";

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
        lblDateRangeError.Visible = false;
        DateOfMarriage.DateValue = MarriageCertNo.Text = string.Empty;
        DateOfMarriage.DateIdValue = MarriageCertNoId.Value = string.Empty;

        NameOfSpouseId.Value = NameOfSiblingId.Value = string.Empty;
        NameOfSpouse.Text = NameOfSibling.Text = string.Empty;
        IdentityNoImageSibling.Text = IdentityNoImageSpouse.Text = string.Empty;

        IDTypeImageSibling.ClearSelection();
        IDTypeImageSpouse.ClearSelection();

        IdentityNoImageSiblingId.Value = IdentityNoImageSpouseId.Value = IDTypeImageSpouseId.Value = IDTypeImageSiblingId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();
            PopulateTag();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.DateOfMarriage.ToString().ToLower()))
                {
                    DateOfMarriage.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfMarriage.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.MarriageCertNo.ToString().ToLower()))
                {
                    MarriageCertNo.Text = metaDataRow.FieldValue;
                    MarriageCertNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local")); Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.IdentityNoSibling.ToString().ToLower()))
                {
                    IdentityNoImageSibling.Text = metaDataRow.FieldValue;
                    IdentityNoImageSiblingId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoImageSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.IDTypeSibling.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageSibling.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageSibling.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageSiblingId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.NameOfSibling.ToString().ToLower()))
                {
                    NameOfSibling.Text = metaDataRow.FieldValue;
                    NameOfSiblingId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertSiblingEnum.NameOfSpouse.ToString().ToLower()))
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
        IDTypeImageSibling.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageSibling.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageSibling.DataBind();

        IDTypeImageSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageSpouse.DataBind();

        //IDTypeImageSpouse.SelectedValue = IDTypeImageSibling.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
    }

    public void PopulateTag()
    {
        TagRadioButtonList.Items.Clear();
        foreach (TagEnum val in Enum.GetValues(typeof(TagEnum)))
        {
            TagRadioButtonList.Items.Add(new RadComboBoxItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        TagRadioButtonList.DataBind();

        //IDTypeImageSibling.SelectedValue = IDTypeImageSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
    }

    protected void NricCustomValidatorSibling(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageSibling.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfMarriage.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorNameOfSibling.Enabled = isEnabled;
    //    RequiredFieldValidatorNameOfSpouse.Enabled = RequiredFieldValidatorMarriageCertNo.Enabled = isEnabled;
    //}
    #endregion

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfMarriage.DateValue, DateTime.Now.ToString()) || !isValidate || DateOfMarriage.EnableRequiredField == false)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfMarriage.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfMarriage.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate || DateOfMarriage.EnableRequiredField == false)
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
            IdentityNoImageSibling.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageSibling.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoImageSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSibling.Text = Validation.ReplaceNonLetterCharacters(NameOfSibling.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (DateOfMarriage.DateIdValue.Length == 0)
            DateOfMarriage.DateIdValue = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.DateOfMarriage.ToString(), DateOfMarriage.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfMarriage.DateIdValue), DateOfMarriage.DateValue);

        if (MarriageCertNoId.Value.Length == 0)
            MarriageCertNoId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.MarriageCertNo.ToString(), MarriageCertNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(MarriageCertNoId.Value), MarriageCertNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoImageSiblingId.Value.Length == 0)
            IdentityNoImageSiblingId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.IdentityNoSibling.ToString(), IdentityNoImageSibling.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageSiblingId.Value), IdentityNoImageSibling.Text.ToUpper());

        if (IdentityNoImageSpouseId.Value.Length == 0)
            IdentityNoImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.IdentityNoSpouse.ToString(), IdentityNoImageSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageSpouseId.Value), IdentityNoImageSpouse.Text.ToUpper());

        if (NameOfSiblingId.Value.Length == 0)
            NameOfSiblingId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.NameOfSibling.ToString(), NameOfSibling.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSiblingId.Value), NameOfSibling.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeImageSiblingId.Value.Length == 0)
            IDTypeImageSiblingId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.IDTypeSibling.ToString(), IDTypeImageSibling.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageSiblingId.Value), IDTypeImageSibling.SelectedValue);

        if (IDTypeImageSpouseId.Value.Length == 0)
            IDTypeImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertSiblingEnum.IDTypeSpouse.ToString(), IDTypeImageSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageSpouseId.Value), IDTypeImageSpouse.SelectedValue);
    }
    #endregion

    #endregion
}
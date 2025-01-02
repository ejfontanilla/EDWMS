using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_MarriageCertParent : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "MarriageCertParent";

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

        NameOfSpouse.Text = NameOfParent.Text = string.Empty;
        NameOfSpouseId.Value = NameOfParentId.Value = string.Empty;
        IdentityNoImageParent.Text = IdentityNoImageSpouse.Text = string.Empty;

        IDTypeImageParent.ClearSelection();
        IDTypeImageSpouse.ClearSelection();

        IdentityNoImageParentId.Value = IdentityNoImageSpouseId.Value = IDTypeImageSpouseId.Value = IDTypeImageParentId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.DateOfMarriage.ToString().ToLower()))
                {
                    DateOfMarriage.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfMarriage.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.MarriageCertNo.ToString().ToLower()))
                {
                    MarriageCertNo.Text = metaDataRow.FieldValue;
                    MarriageCertNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local")); Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.IdentityNoParent.ToString().ToLower()))
                {
                    IdentityNoImageParent.Text = metaDataRow.FieldValue;
                    IdentityNoImageParentId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoImageSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.IDTypeParent.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageParent.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageParent.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageParentId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.NameOfParent.ToString().ToLower()))
                {
                    NameOfParent.Text = metaDataRow.FieldValue;
                    NameOfParentId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertParentEnum.NameOfSpouse.ToString().ToLower()))
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
        IDTypeImageParent.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageParent.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageParent.DataBind();
        
        //IDTypeImageParent.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors

        IDTypeImageSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageSpouse.DataBind();

        //IDTypeImageSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
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

    protected void NricCustomValidatorParent(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageParent.SelectedValue, args.Value.ToString().Trim());
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
    //    RequiredFieldValidatorNameOfParent.Enabled = RequiredFieldValidatorIdentityNoImageParent.Enabled = RequiredFieldValidatorIdentityNoImageSpouse.Enabled = isEnabled;
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
            IdentityNoImageParent.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageParent.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoImageSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfParent.Text = Validation.ReplaceNonLetterCharacters(NameOfParent.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (DateOfMarriage.DateIdValue.Length == 0)
            DateOfMarriage.DateIdValue = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.DateOfMarriage.ToString(), DateOfMarriage.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfMarriage.DateIdValue), DateOfMarriage.DateValue);

        if (MarriageCertNoId.Value.Length == 0)
            MarriageCertNoId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.MarriageCertNo.ToString(), MarriageCertNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(MarriageCertNoId.Value), MarriageCertNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoImageParentId.Value.Length == 0)
            IdentityNoImageParentId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.IdentityNoParent.ToString(), IdentityNoImageParent.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageParentId.Value), IdentityNoImageParent.Text.ToUpper());

        if (IdentityNoImageSpouseId.Value.Length == 0)
            IdentityNoImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.IdentityNoSpouse.ToString(), IdentityNoImageSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageSpouseId.Value), IdentityNoImageSpouse.Text.ToUpper());

        if (NameOfParentId.Value.Length == 0)
            NameOfParentId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.NameOfParent.ToString(), NameOfParent.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfParentId.Value), NameOfParent.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeImageParentId.Value.Length == 0)
            IDTypeImageParentId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.IDTypeParent.ToString(), IDTypeImageParent.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageParentId.Value), IDTypeImageParent.SelectedValue);

        if (IDTypeImageSpouseId.Value.Length == 0)
            IDTypeImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertParentEnum.IDTypeSpouse.ToString(), IDTypeImageSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageSpouseId.Value), IDTypeImageSpouse.SelectedValue);
    }
    #endregion

    #endregion
}
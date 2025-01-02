using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_MarriageCertChild : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "MarriageCertChild";

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

        NameOfSpouseId.Value = NameOfChildId.Value = string.Empty;
        NameOfSpouse.Text = NameOfChild.Text = string.Empty;
        IdentityNoImageChild.Text = IdentityNoImageSpouse.Text  = string.Empty;

        IDTypeImageChild.ClearSelection();
        IDTypeImageSpouse.ClearSelection();

        IdentityNoImageChildId.Value = IdentityNoImageSpouseId.Value = IDTypeImageSpouseId.Value = IDTypeImageChildId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.DateOfMarriage.ToString().ToLower()))
                {
                    DateOfMarriage.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfMarriage.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.MarriageCertNo.ToString().ToLower()))
                {
                    MarriageCertNo.Text = metaDataRow.FieldValue;
                    MarriageCertNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.IdentityNoChild.ToString().ToLower()))
                {
                    IdentityNoImageChild.Text = metaDataRow.FieldValue;
                    IdentityNoImageChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoImageSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.IDTypeChild.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageChild.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageChild.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.IDTypeSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.NameOfChild.ToString().ToLower()))
                {
                    NameOfChild.Text = metaDataRow.FieldValue;
                    NameOfChildId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertChildEnum.NameOfSpouse.ToString().ToLower()))
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
        IDTypeImageChild.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageChild.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageChild.DataBind();
        //IDTypeImageChild.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors

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

        //IDTypeImageChild.SelectedValue = IDTypeImageSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
    }

    protected void NricCustomValidatorChild(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageChild.SelectedValue, args.Value.ToString().Trim());
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
    //    RequiredFieldValidatorNameOfChild.Enabled = isEnabled;
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
            IdentityNoImageChild.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageChild.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoImageSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfChild.Text = Validation.ReplaceNonLetterCharacters(NameOfChild.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfSpouse.Text);

        if (DateOfMarriage.DateIdValue.Length == 0)
            DateOfMarriage.DateIdValue = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.DateOfMarriage.ToString(), DateOfMarriage.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfMarriage.DateIdValue), DateOfMarriage.DateValue);

        if (MarriageCertNoId.Value.Length == 0)
            MarriageCertNoId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.MarriageCertNo.ToString(), MarriageCertNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(MarriageCertNoId.Value), MarriageCertNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoImageChildId.Value.Length == 0)
            IdentityNoImageChildId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.IdentityNoChild.ToString(), IdentityNoImageChild.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageChildId.Value), IdentityNoImageChild.Text.ToUpper());

        if (IdentityNoImageSpouseId.Value.Length == 0)
            IdentityNoImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.IdentityNoSpouse.ToString(), IdentityNoImageSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageSpouseId.Value), IdentityNoImageSpouse.Text.ToUpper());

        if (NameOfChildId.Value.Length == 0)
            NameOfChildId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.NameOfChild.ToString(), NameOfChild.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfChildId.Value), NameOfChild.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeImageChildId.Value.Length == 0)
            IDTypeImageChildId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.IDTypeChild.ToString(), IDTypeImageChild.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageChildId.Value), IDTypeImageChild.SelectedValue);

        if (IDTypeImageSpouseId.Value.Length == 0)
            IDTypeImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertChildEnum.IDTypeSpouse.ToString(), IDTypeImageSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageSpouseId.Value), IDTypeImageSpouse.SelectedValue);
    }
    #endregion

    #endregion
}
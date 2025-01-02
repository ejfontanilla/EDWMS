using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_MarriageCertLtSpouse : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "MarriageCertLtSpouse";

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

        NameOfRequestor.Text = NameOfLateSpouse.Text = string.Empty;
        IdentityNoRequestor.Text = IdentityNoImageRequestor.Text = IdentityNoImageLateSpouse.Text = string.Empty;
        NameOfRequestorId.Value = NameOfLateSpouseId.Value = string.Empty;
        IdentityNoRequestorId.Value = IDTypeRequestorId.Value = string.Empty;

        IDTypeRequestor.ClearSelection();
        IDTypeImageRequestor.ClearSelection();
        IDTypeImageLateSpouse.ClearSelection();

        IdentityNoImageRequestorId.Value = IDTypeImageRequestorId.Value = string.Empty;
        IdentityNoImageLateSpouseId.Value = IDTypeImageLateSpouseId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.DateOfMarriage.ToString().ToLower()))
                {
                    DateOfMarriage.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfMarriage.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.MarriageCertNo.ToString().ToLower()))
                {
                    MarriageCertNo.Text = metaDataRow.FieldValue;
                    MarriageCertNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoRequestor.ToString().ToLower()))
                {
                    IdentityNoRequestor.Text = metaDataRow.FieldValue;
                    IdentityNoRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoImageRequestor.ToString().ToLower()))
                {
                    IdentityNoImageRequestor.Text = metaDataRow.FieldValue;
                    IdentityNoImageRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoLateSpouse.ToString().ToLower()))
                {
                    IdentityNoImageLateSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoImageLateSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeRequestor.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeRequestor.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeRequestor.SelectedValue = metaDataRow.FieldValue;
                    IDTypeRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeImageRequestor.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageRequestor.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageRequestor.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeLateSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageLateSpouse.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeImageLateSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageLateSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.NameOfRequestor.ToString().ToLower()))
                {
                    NameOfRequestor.Text = metaDataRow.FieldValue;
                    NameOfRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertLtSpouseEnum.NameOfLateSpouse.ToString().ToLower()))
                {
                    NameOfLateSpouse.Text = metaDataRow.FieldValue;
                    NameOfLateSpouseId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        //attached the Household Structure NRIC and tag
        if (isUnderHouseholdStructure)
        {
            IdentityNoRequestor.Text = CommonMetadata.NricValue;
            IDTypeRequestor.SelectedValue = Retrieve.GetIdTypeByNRIC(CommonMetadata.NricValue);
        }

        //enable the IdentityNo and Type is outside the household structure
        IdentityNoRequestor.Enabled = IDTypeRequestor.Enabled = !isUnderHouseholdStructure;

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }


    public void PopulateIDType()
    {
        IDTypeRequestor.Items.Clear();
        IDTypeImageRequestor.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeRequestor.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
            IDTypeImageRequestor.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeRequestor.DataBind();
        IDTypeImageRequestor.DataBind();
        //IDTypeRequestor.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
        //IDTypeImageRequestor.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors

        IDTypeImageLateSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeImageLateSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeImageLateSpouse.DataBind();

        //IDTypeImageLateSpouse.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
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

    protected void NricCustomValidatorRequestor(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeRequestor.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorImageRequestor(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageRequestor.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorLateSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageLateSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfMarriage.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorIdentityNoRequestor.Enabled =RequiredFieldValidatorIdentityNoImageLateSpouse.Enabled =  RequiredFieldValidatorNameOfLateSpouse.Enabled = RequiredFieldValidatorMarriageCertNo.Enabled = isEnabled;
    //    NricFinCustomValidatorRequestor.Enabled = NricFinCustomValidatorImageLateSpouse.Enabled = isEnabled;
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
            IdentityNoRequestor.Text = Validation.ReplaceSpecialCharacters(IdentityNoRequestor.Text);

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoImageLateSpouse.Text = Validation.ReplaceSpecialCharacters(IdentityNoImageLateSpouse.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfRequestor.Text = Validation.ReplaceNonLetterCharacters(NameOfRequestor.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfLateSpouse.Text = Validation.ReplaceNonLetterCharacters(NameOfLateSpouse.Text);

        if (DateOfMarriage.DateIdValue.Length == 0)
            DateOfMarriage.DateIdValue = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.DateOfMarriage.ToString(), DateOfMarriage.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfMarriage.DateIdValue), DateOfMarriage.DateValue);

        if (MarriageCertNoId.Value.Length == 0)
            MarriageCertNoId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.MarriageCertNo.ToString(), MarriageCertNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(MarriageCertNoId.Value), MarriageCertNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoRequestorId.Value.Length == 0)
            IdentityNoRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoRequestor.ToString(), IdentityNoRequestor.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoRequestorId.Value), IdentityNoRequestor.Text.ToUpper());

        if (IdentityNoImageRequestorId.Value.Length == 0)
            IdentityNoImageRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoImageRequestor.ToString(), IdentityNoImageRequestor.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageRequestorId.Value), IdentityNoImageRequestor.Text.ToUpper());

        if (IdentityNoImageLateSpouseId.Value.Length == 0)
            IdentityNoImageLateSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IdentityNoLateSpouse.ToString(), IdentityNoImageLateSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageLateSpouseId.Value), IdentityNoImageLateSpouse.Text.ToUpper());

        if (NameOfRequestorId.Value.Length == 0)
            NameOfRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.NameOfRequestor.ToString(), NameOfRequestor.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfRequestorId.Value), NameOfRequestor.Text);

        if (NameOfLateSpouseId.Value.Length == 0)
            NameOfLateSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.NameOfLateSpouse.ToString(), NameOfLateSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfLateSpouseId.Value), NameOfLateSpouse.Text);

        if (IDTypeRequestorId.Value.Length == 0)
            IDTypeRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeRequestor.ToString(), IDTypeRequestor.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeRequestorId.Value), IDTypeRequestor.SelectedValue);

        if (IDTypeImageRequestorId.Value.Length == 0)
            IDTypeImageRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeImageRequestor.ToString(), IDTypeImageRequestor.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageRequestorId.Value), IDTypeImageRequestor.SelectedValue);

        if (IDTypeImageLateSpouseId.Value.Length == 0)
            IDTypeImageLateSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertLtSpouseEnum.IDTypeLateSpouse.ToString(), IDTypeImageLateSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageLateSpouseId.Value), IDTypeImageLateSpouse.SelectedValue);
    }
    #endregion

    #endregion
}
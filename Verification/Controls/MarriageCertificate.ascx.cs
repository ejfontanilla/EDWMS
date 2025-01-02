using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_MarriageCertificate : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "MarriageCertificate";
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

        NameOfSpouse.Text = NameOfRequestor.Text = string.Empty;
        NameOfSpouseId.Value = NameOfRequestorId.Value = string.Empty;
        IdentityNoRequestor.Text = IdentityNoSpouse.Text = string.Empty;
        IdentityNoImageRequestor.Text = IdentityNoImageSpouse.Text = string.Empty;

        IDTypeRequestor.ClearSelection(); 
        IDTypeSpouse.ClearSelection();
        IDTypeImageRequestor.ClearSelection(); 
        IDTypeImageSpouse.ClearSelection();

        IdentityNoRequestorId.Value = IdentityNoSpouseId.Value = IDTypeSpouseId.Value = IDTypeRequestorId.Value = string.Empty;
        IdentityNoImageRequestorId.Value = IdentityNoImageSpouseId.Value = IDTypeImageSpouseId.Value = IDTypeImageRequestorId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.DateOfMarriage.ToString().ToLower()))
                {
                    DateOfMarriage.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfMarriage.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.MarriageCertNo.ToString().ToLower()))
                {
                    MarriageCertNo.Text = metaDataRow.FieldValue;
                    MarriageCertNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.Tag.ToString().ToLower()))
                {
                    TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));    Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IdentityNoRequestor.ToString().ToLower()))
                {
                    IdentityNoRequestor.Text = metaDataRow.FieldValue;
                    IdentityNoRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IdentityNoSpouse.ToString().ToLower()))
                {
                    IdentityNoSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IdentityNoImageRequestor.ToString().ToLower()))
                {
                    IdentityNoImageRequestor.Text = metaDataRow.FieldValue;
                    IdentityNoImageRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IdentityNoImageSpouse.ToString().ToLower()))
                {
                    IdentityNoImageSpouse.Text = metaDataRow.FieldValue;
                    IdentityNoImageSpouseId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IDTypeRequestor.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeRequestor.SelectedValue = metaDataRow.FieldValue;
                    IDTypeRequestorId.Value = metaDataRow.Id.ToString();
                    SetIdType();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IDTypeSpouse.ToString().ToLower()))
                {//#1
                    if (metaDataRow.FieldValue.Length > 0) IDTypeSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeSpouseId.Value = metaDataRow.Id.ToString();
                    SetIdType();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IDTypeImageRequestor.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageRequestor.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageRequestorId.Value = metaDataRow.Id.ToString();
                    SetIdType();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.IDTypeImageSpouse.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeImageSpouse.SelectedValue = metaDataRow.FieldValue;
                    IDTypeImageSpouseId.Value = metaDataRow.Id.ToString();
                    SetIdType();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.NameOfRequestor.ToString().ToLower()))
                {
                    NameOfRequestor.Text = metaDataRow.FieldValue;
                    NameOfRequestorId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataMarriageCertificateEnum.NameOfSpouse.ToString().ToLower()))
                {
                    NameOfSpouse.Text = metaDataRow.FieldValue;
                    NameOfSpouseId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        //attached the Household Structure NRIC and tag
        if (isUnderHouseholdStructure)
        {
            IdentityNoRequestor.Text = CommonMetadata.NricValue;
            string idType = Retrieve.GetIdTypeByNRIC(CommonMetadata.NricValue);
            IDTypeRequestor.SelectedValue = string.IsNullOrEmpty(idType) ? IDTypeEnum.UIN.ToString() : idType;
        }

        //enable the IdentityNo and Type is outside the household structure
        IdentityNoRequestor.Enabled = IDTypeRequestor.Enabled = !isUnderHouseholdStructure;

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);

        HiddenFieldIsUnderHouseholdStructure.Value = isUnderHouseholdStructure.ToString().ToLower();
        HiddenFieldDocRefId.Value = docRefId.ToString();
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

        IDTypeSpouse.Items.Clear();
        IDTypeImageSpouse.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
            IDTypeImageSpouse.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeSpouse.DataBind();
        IDTypeImageSpouse.DataBind();

        //IDTypeRequestor.SelectedValue = IDTypeImageRequestor.SelectedValue = IDTypeSpouse.SelectedValue = IDTypeImageSpouse.SelectedValue = IDTypeEnum.UIN.ToString();
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

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorImageRequestor(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageRequestor.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorImageSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeImageSpouse.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorSpouseHousehold(object source, ServerValidateEventArgs args)
    {
        string nric = args.Value.ToString().Trim();
        if (!string.IsNullOrEmpty(nric) && HiddenFieldIsUnderHouseholdStructure.Value.Equals("true"))
        {
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            int docAppId = appPersonalDb.GetDocAppIdByAppDocRefId(int.Parse(HiddenFieldDocRefId.Value));
            if (docAppId != -1)
            {
                args.IsValid = appPersonalDb.IsAppNric(nric, docAppId);
            }
            else
                args.IsValid = false;
        }
        else
            args.IsValid = true;
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Contains("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfMarriage.EnableRequiredField = isEnabled;
    //    RequiredFieldValidatorIdentityNoRequestor.Enabled = RequiredFieldValidatorIdentityNoImageSpouse.Enabled = isEnabled;
    //    RequiredFieldValidatorNameOfSpouse.Enabled = RequiredFieldValidatorMarriageCertNo.Enabled = isEnabled;
    //    NricFinCustomValidatorImageRequestor.Enabled = NricFinCustomValidatorImageSpouse.Enabled = NricFinCustomValidatorRequestor.Enabled = isEnabled;
    //    NricFinCustomValidatorSpouseHousehold.Enabled = NricFinCustomValidatorSpouse.Enabled = isEnabled;
    //}
    #endregion
    //#2
    protected void SetIdType()
    {
        if (IdentityNoRequestor.Text.Length > 0) RequiredFieldValidatorIDTypeRequestor.Enabled = true;
        else RequiredFieldValidatorIDTypeRequestor.Enabled = false;
        if (IdentityNoImageRequestor.Text.Length > 0) RequiredFieldValidatorIDTypeImageRequestor.Enabled = true;
        else RequiredFieldValidatorIDTypeImageRequestor.Enabled = false;
        if (IdentityNoSpouse.Text.Length > 0) RequiredFieldValidatorIDTypeSpouse.Enabled = true;
        else RequiredFieldValidatorIDTypeSpouse.Enabled = false;
        if (IdentityNoImageSpouse.Text.Length > 0) RequiredFieldValidatorIDTypeImageSpouse.Enabled = true;
        else RequiredFieldValidatorIDTypeImageSpouse.Enabled = false;
    }

    #region Added By Edward 10/01/2014 to address "Identity No should be part of household structure" Issue
    private int GetDocAppId(int docRefId)
    {
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByAppDocRefId(docRefId);

        if (appPersonals.Rows.Count > 0)
        {
            AppPersonal.AppPersonalRow appPersonalRow = appPersonals[0];
            return appPersonalRow.DocAppId;
        }
        return -1;
    }

    private int GetAppDocRefId(string nric, int docAppId, int docId)
    {
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        AppPersonal.AppPersonalDataTable appPersonalsByNricDocAppId = appPersonalDb.GetAppPersonalByNricAndDocAppId(nric, docAppId);
                
        if (appPersonalsByNricDocAppId.Rows.Count > 0)
        {
            AppPersonal.AppPersonalRow appPersonalsByNricSetAppIdRow = appPersonalsByNricDocAppId[0];

            AppDocRefDb appDocRefDb = new AppDocRefDb();
            AppDocRef.AppDocRefDataTable appDocRefdt = appDocRefDb.GetAppDocRefByAppPersonalIdAndDocId(appPersonalsByNricSetAppIdRow.Id,docId);

            if (appDocRefdt.Rows.Count > 0)
            {
                AppDocRef.AppDocRefRow appDocRefRow = appDocRefdt[0];
                return appDocRefRow.Id;
            }
        }
        return -1;
    }

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

    protected void IdentityNoRequestor_TextChanged(object sender, EventArgs e)
    {
        IdentityNoRequestor.Text = IdentityNoRequestor.Text.ToUpper();
        SetIdType();
    }
    protected void IdentityNoImageRequestor_TextChanged(object sender, EventArgs e)
    {
        IdentityNoImageRequestor.Text = IdentityNoImageRequestor.Text.ToUpper();
        SetIdType();
    }
    protected void IdentityNoSpouse_TextChanged(object sender, EventArgs e)
    {
        IdentityNoSpouse.Text = IdentityNoSpouse.Text.ToUpper();
        SetIdType();
    }
    protected void IdentityNoImageSpouse_TextChanged(object sender, EventArgs e)
    {
        IdentityNoImageSpouse.Text = IdentityNoImageSpouse.Text.ToUpper();
        SetIdType();
    }
    #endregion//#4

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
        #region Added By Edward 10/01/2014 to address "Identity No should be part of household structure" Issue
        int docAppId = GetDocAppId(docRefId);
        #endregion

        //Save Perosnal Data
        CustomPersonal customPersonal = new CustomPersonal();
        //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
        //Added by Edward 2016/03/23 Optimize Doctype Saving
        customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType,
            CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
        #region Added By Edward 10/01/2014 to address "Identity No should be part of household structure" Issue
        if (docAppId > 0)
            HiddenFieldDocRefId.Value = GetAppDocRefId(CommonMetadata.NricTextBox.Text.Trim(), docAppId, docId).ToString();
        #endregion

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

        if (DateOfMarriage.DateIdValue.Length == 0)
            DateOfMarriage.DateIdValue = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.DateOfMarriage.ToString(), DateOfMarriage.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfMarriage.DateIdValue), DateOfMarriage.DateValue);

        if (MarriageCertNoId.Value.Length == 0)
            MarriageCertNoId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.MarriageCertNo.ToString(), MarriageCertNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(MarriageCertNoId.Value), MarriageCertNo.Text);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

        if (IdentityNoRequestorId.Value.Length == 0)
            IdentityNoRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IdentityNoRequestor.ToString(), IdentityNoRequestor.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoRequestorId.Value), IdentityNoRequestor.Text.ToUpper());

        if (IdentityNoSpouseId.Value.Length == 0)
            IdentityNoSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IdentityNoSpouse.ToString(), IdentityNoSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoSpouseId.Value), IdentityNoSpouse.Text.ToUpper());

        if (IdentityNoImageRequestorId.Value.Length == 0)
            IdentityNoImageRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IdentityNoImageRequestor.ToString(), IdentityNoImageRequestor.Text.ToUpper(), docId, false).ToString();
        else//#3
            metadataDb.Update(int.Parse(IdentityNoImageRequestorId.Value), IdentityNoImageRequestor.Text.ToUpper());

        if (IdentityNoImageSpouseId.Value.Length == 0)
            IdentityNoImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IdentityNoImageSpouse.ToString(), IdentityNoImageSpouse.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoImageSpouseId.Value), IdentityNoImageSpouse.Text.ToUpper());

        if (NameOfRequestorId.Value.Length == 0)
            NameOfRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.NameOfRequestor.ToString(), NameOfRequestor.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfRequestorId.Value), NameOfRequestor.Text);

        if (NameOfSpouseId.Value.Length == 0)
            NameOfSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.NameOfSpouse.ToString(), NameOfSpouse.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfSpouseId.Value), NameOfSpouse.Text);

        if (IDTypeRequestorId.Value.Length == 0)
            IDTypeRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IDTypeRequestor.ToString(), IDTypeRequestor.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeRequestorId.Value), IDTypeRequestor.SelectedValue);

        if (IDTypeSpouseId.Value.Length == 0)
            IDTypeSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IDTypeSpouse.ToString(), IDTypeSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeSpouseId.Value), IDTypeSpouse.SelectedValue);

        if (IDTypeImageRequestorId.Value.Length == 0)
            IDTypeImageRequestorId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IDTypeImageRequestor.ToString(), IDTypeImageRequestor.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageRequestorId.Value), IDTypeImageRequestor.SelectedValue);

        if (IDTypeImageSpouseId.Value.Length == 0)
            IDTypeImageSpouseId.Value = metadataDb.Insert(DocTypeMetaDataMarriageCertificateEnum.IDTypeImageSpouse.ToString(), IDTypeImageSpouse.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeImageSpouseId.Value), IDTypeImageSpouse.SelectedValue);
    }
    #endregion
}
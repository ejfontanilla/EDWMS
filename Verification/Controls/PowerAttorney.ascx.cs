using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_PowerAttorney : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "PowerAttorney";

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
        NameDonor1.Text = NameDonor2.Text = NameDonor3.Text = NameDonor4.Text = string.Empty;
        IDTypeDonor1.ClearSelection();
        IDTypeDonor2.ClearSelection();
        IDTypeDonor3.ClearSelection();
        IDTypeDonor4.ClearSelection();
        IdentityNoDonor1.Text = IdentityNoDonor2.Text = IdentityNoDonor3.Text = IdentityNoDonor4.Text = string.Empty;
        NameDonor1Id.Value = NameDonor2Id.Value = NameDonor3Id.Value = NameDonor4Id.Value = string.Empty;
        IdentityNoDonor1Id.Value =  IdentityNoDonor2Id.Value = IdentityNoDonor3Id.Value = IdentityNoDonor4Id.Value = string.Empty;
        IDTypeDonor1Id.Value = IDTypeDonor2Id.Value = IDTypeDonor3Id.Value = IDTypeDonor4Id.Value = string.Empty;
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

            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.DateOfFiling.ToString().ToLower()))
                {
                    DateOfFiling.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfFiling.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor1.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeDonor1.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeDonor1.SelectedValue = metaDataRow.FieldValue;
                    IDTypeDonor1Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor2.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeDonor2.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeDonor2.SelectedValue = metaDataRow.FieldValue;
                    IDTypeDonor2Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor3.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeDonor3.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeDonor3.SelectedValue = metaDataRow.FieldValue;
                    IDTypeDonor3Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor4.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDTypeDonor4.SelectedValue = metaDataRow.FieldValue;
                    //IDTypeDonor4.SelectedValue = metaDataRow.FieldValue;
                    IDTypeDonor4Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.NameDonor1.ToString().ToLower()))
                {
                    NameDonor1.Text = metaDataRow.FieldValue;
                    NameDonor1Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.NameDonor2.ToString().ToLower()))
                {
                    NameDonor2.Text = metaDataRow.FieldValue;
                    NameDonor2Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.NameDonor3.ToString().ToLower()))
                {
                    NameDonor3.Text = metaDataRow.FieldValue;
                    NameDonor3Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.NameDonor4.ToString().ToLower()))
                {
                    NameDonor4.Text = metaDataRow.FieldValue;
                    NameDonor4Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor1.ToString().ToLower()))
                {
                    IdentityNoDonor1.Text = metaDataRow.FieldValue;
                    IdentityNoDonor1Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor2.ToString().ToLower()))
                {
                    IdentityNoDonor2.Text = metaDataRow.FieldValue;
                    IdentityNoDonor2Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor3.ToString().ToLower()))
                {
                    IdentityNoDonor3.Text = metaDataRow.FieldValue;
                    IdentityNoDonor3Id.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor4.ToString().ToLower()))
                {
                    IdentityNoDonor4.Text = metaDataRow.FieldValue;
                    IdentityNoDonor4Id.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    protected void NricCustomValidatorDonor1(object source, ServerValidateEventArgs args)
    {
        if (!string.IsNullOrEmpty(IdentityNoDonor1.Text.Trim()))
            args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeDonor1.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorDonor2(object source, ServerValidateEventArgs args)
    {
        if (!string.IsNullOrEmpty(IdentityNoDonor2.Text.Trim()))
            args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeDonor2.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorDonor3(object source, ServerValidateEventArgs args)
    {
        if (!string.IsNullOrEmpty(IdentityNoDonor3.Text.Trim()))
            args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeDonor3.SelectedValue, args.Value.ToString().Trim());
    }

    protected void NricCustomValidatorDonor4(object source, ServerValidateEventArgs args)
    {
        if (!string.IsNullOrEmpty(IdentityNoDonor4.Text.Trim()))
            args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDTypeDonor4.SelectedValue, args.Value.ToString().Trim());
    }

    public void PopulateIDType()
    {
        IDTypeDonor1.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeDonor1.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeDonor1.DataBind();

        IDTypeDonor2.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeDonor2.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeDonor2.DataBind();

        IDTypeDonor3.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeDonor3.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeDonor3.DataBind();

        IDTypeDonor4.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDTypeDonor4.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDTypeDonor4.DataBind();

        //IDTypeDonor1.SelectedValue = IDTypeDonor2.SelectedValue = IDTypeEnum.UIN.ToString();
        //IDTypeDonor3.SelectedValue = IDTypeDonor4.SelectedValue = IDTypeEnum.UIN.ToString();
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfFiling.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfFiling.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfFiling.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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

        //date of filing
        if (DateOfFiling.DateIdValue.Length == 0)
            DateOfFiling.DateIdValue = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.DateOfFiling.ToString(), DateOfFiling.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfFiling.DateIdValue), DateOfFiling.DateValue);

        //idtype
        if (IDTypeDonor1Id.Value.Length == 0)
            IDTypeDonor1Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor1.ToString(), IDTypeDonor1.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeDonor1Id.Value), IDTypeDonor1.SelectedValue);

        if (IDTypeDonor2Id.Value.Length == 0)
            IDTypeDonor2Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor2.ToString(), IDTypeDonor2.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeDonor2Id.Value), IDTypeDonor2.SelectedValue);

        if (IDTypeDonor3Id.Value.Length == 0)
            IDTypeDonor3Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor3.ToString(), IDTypeDonor3.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeDonor3Id.Value), IDTypeDonor3.SelectedValue);

        if (IDTypeDonor4Id.Value.Length == 0)
            IDTypeDonor4Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IDTypeDonor4.ToString(), IDTypeDonor4.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeDonor4Id.Value), IDTypeDonor4.SelectedValue);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameDonor1.Text = Validation.ReplaceNonLetterCharacters(NameDonor1.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameDonor2.Text = Validation.ReplaceNonLetterCharacters(NameDonor2.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameDonor3.Text = Validation.ReplaceNonLetterCharacters(NameDonor3.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameDonor4.Text = Validation.ReplaceNonLetterCharacters(NameDonor4.Text);

        //name
        if (NameDonor1Id.Value.Length == 0)
            NameDonor1Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.NameDonor1.ToString(), NameDonor1.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameDonor1Id.Value), NameDonor1.Text);

        if (NameDonor2Id.Value.Length == 0)
            NameDonor2Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.NameDonor2.ToString(), NameDonor2.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameDonor2Id.Value), NameDonor2.Text);

        if (NameDonor3Id.Value.Length == 0)
            NameDonor3Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.NameDonor3.ToString(), NameDonor3.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameDonor3Id.Value), NameDonor3.Text);

        if (NameDonor4Id.Value.Length == 0)
            NameDonor4Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.NameDonor4.ToString(), NameDonor4.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameDonor4Id.Value), NameDonor4.Text);

        //Identity no

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        if (isValidate)
            IdentityNoDonor1.Text = Validation.ReplaceSpecialCharacters(IdentityNoDonor1.Text);

        if (IdentityNoDonor1Id.Value.Length == 0)
            IdentityNoDonor1Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor1.ToString(), IdentityNoDonor1.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoDonor1Id.Value), IdentityNoDonor1.Text.ToUpper());

        if (isValidate)
            IdentityNoDonor2.Text = Validation.ReplaceSpecialCharacters(IdentityNoDonor2.Text);

        if (IdentityNoDonor2Id.Value.Length == 0)
            IdentityNoDonor2Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor2.ToString(), IdentityNoDonor2.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoDonor2Id.Value), IdentityNoDonor2.Text.ToUpper());

        if (isValidate)
            IdentityNoDonor3.Text = Validation.ReplaceSpecialCharacters(IdentityNoDonor3.Text);

        if (IdentityNoDonor3Id.Value.Length == 0)
            IdentityNoDonor3Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor3.ToString(), IdentityNoDonor3.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoDonor3Id.Value), IdentityNoDonor3.Text.ToUpper());

        if (isValidate)
            IdentityNoDonor4.Text = Validation.ReplaceSpecialCharacters(IdentityNoDonor4.Text);

        if (IdentityNoDonor4Id.Value.Length == 0)
            IdentityNoDonor4Id.Value = metadataDb.Insert(DocTypeMetaDataPowerAttorneyEnum.IdentityNoDonor4.ToString(), IdentityNoDonor4.Text.ToUpper(), docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoDonor4Id.Value), IdentityNoDonor4.Text.ToUpper());
    }
    #endregion

    #endregion
}
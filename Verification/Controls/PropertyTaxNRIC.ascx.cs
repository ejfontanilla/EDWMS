using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Controls_PropertyTaxNRIC : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "PropertyTaxNRIC";

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
        YearOfPropertyTax.Text = string.Empty;
        YearOfPropertyTaxId.Value = string.Empty;
        NameOfNRIC.Text = IdentityNo.Text = string.Empty;
        IDType.ClearSelection();
        IDTypeId.Value = string.Empty;
        NameOfNRICId.Value = IdentityNoId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        CustomPersonal customPersonal = new CustomPersonal();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPropertyTaxNRICEnum.YearOfPropertyTax.ToString().ToLower()))
                {
                    YearOfPropertyTax.Text = metaDataRow.FieldValue;
                    YearOfPropertyTaxId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPropertyTaxNRICEnum.IDType.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDType.SelectedValue = metaDataRow.FieldValue;
                    IDTypeId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPropertyTaxNRICEnum.IdentityNo.ToString().ToLower()))
                {
                    IdentityNo.Text = metaDataRow.FieldValue;
                    IdentityNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPropertyTaxNRICEnum.NameOfNRIC.ToString().ToLower()))
                {
                    NameOfNRIC.Text = metaDataRow.FieldValue;
                    NameOfNRICId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsInteger(YearOfPropertyTax.Text) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(YearOfPropertyTax.Text))
            strDateNow = string.Empty;

        if (Validation.IsInteger(YearOfPropertyTax.Text) || !isValidate)
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
            IdentityNo.Text = Validation.ReplaceSpecialCharacters(IdentityNo.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        if (isValidate)
            NameOfNRIC.Text = Validation.ReplaceNonLetterCharacters(NameOfNRIC.Text);

        if (YearOfPropertyTaxId.Value.Length == 0)
        {
            YearOfPropertyTaxId.Value = metadataDb.Insert(DocTypeMetaDataPropertyTaxEnum.YearOfPropertyTax.ToString(), YearOfPropertyTax.Text, docId, false).ToString();
            FromDateId.Value = metadataDb.Insert(DocTypeMetaDataPAYSLIPEnum.StartDate.ToString(), string.Format("01-01-{0}", YearOfPropertyTax.Text), docId, false).ToString();
            ToDateId.Value = metadataDb.Insert(DocTypeMetaDataPAYSLIPEnum.EndDate.ToString(), string.Format("31-12-{0}", YearOfPropertyTax.Text), docId, false).ToString();
        }
        else
        {
            metadataDb.Update(int.Parse(YearOfPropertyTaxId.Value), YearOfPropertyTax.Text);
            metadataDb.Update(int.Parse(FromDateId.Value), string.Format("01-01-{0}", YearOfPropertyTax.Text));
            metadataDb.Update(int.Parse(ToDateId.Value), string.Format("31-12-{0}", YearOfPropertyTax.Text));
        }

        if (IDTypeId.Value.Length == 0)
            IDTypeId.Value = metadataDb.Insert(DocTypeMetaDataPropertyTaxNRICEnum.IDType.ToString(), IDType.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeId.Value), IDType.SelectedValue);

        if (IdentityNoId.Value.Length == 0)
            IdentityNoId.Value = metadataDb.Insert(DocTypeMetaDataPropertyTaxNRICEnum.IdentityNo.ToString(), IdentityNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoId.Value), IdentityNo.Text);

        if (NameOfNRICId.Value.Length == 0)
            NameOfNRICId.Value = metadataDb.Insert(DocTypeMetaDataPropertyTaxNRICEnum.NameOfNRIC.ToString(), NameOfNRIC.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfNRICId.Value), NameOfNRIC.Text);
    }
    #endregion

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

        //IDType.SelectedValue = IDTypeEnum.UIN.ToString(); //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
    }

    protected void SetRequiredField(Boolean isEnabled)
    {
        CommonMetadata.EnableRequiredField = RequiredFieldValidatorIdentityNo.Enabled = RequiredFieldValidatorNameOfNRIC.Enabled = NricFinCustomValidator.Enabled = isEnabled;
    }
}
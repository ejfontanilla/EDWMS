using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Controls_IdentityCardChild : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "IdentityCardChild";

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
        DateIssue.DateValue = string.Empty;
        DateIssue.DateIdValue = string.Empty;
        NameOfChild.Text = IdentityNo.Text = string.Empty;
        IDType.ClearSelection();        
        IDTypeId.Value = string.Empty;
        NameOfChildId.Value = IdentityNoId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIdentityCardChildEnum.DateOfIssue.ToString().ToLower()))
                {
                    DateIssue.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateIssue.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIdentityCardChildEnum.IDType.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDType.SelectedValue = metaDataRow.FieldValue;
                    IDTypeId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIdentityCardChildEnum.IdentityNo.ToString().ToLower()))
                {
                    IdentityNo.Text = metaDataRow.FieldValue;
                    IdentityNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIdentityCardChildEnum.NameOfChild.ToString().ToLower()))
                {
                    NameOfChild.Text = metaDataRow.FieldValue;
                    NameOfChildId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateIssue.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateIssue.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateIssue.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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
            NameOfChild.Text = Validation.ReplaceNonLetterCharacters(NameOfChild.Text);

        if (DateIssue.DateIdValue.Length == 0)
            DateIssue.DateIdValue = metadataDb.Insert(DocTypeMetaDataIdentityCardChildEnum.DateOfIssue.ToString(), DateIssue.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateIssue.DateIdValue), DateIssue.DateValue);

        if (IDTypeId.Value.Length == 0)
            IDTypeId.Value = metadataDb.Insert(DocTypeMetaDataIdentityCardChildEnum.IDType.ToString(), IDType.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeId.Value), IDType.SelectedValue);

        if (IdentityNoId.Value.Length == 0)
            IdentityNoId.Value = metadataDb.Insert(DocTypeMetaDataIdentityCardChildEnum.IdentityNo.ToString(), IdentityNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IdentityNoId.Value), IdentityNo.Text);

        if (NameOfChildId.Value.Length == 0)
            NameOfChildId.Value = metadataDb.Insert(DocTypeMetaDataIdentityCardChildEnum.NameOfChild.ToString(), NameOfChild.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(NameOfChildId.Value), NameOfChild.Text);
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
        CommonMetadata.EnableRequiredField = RequiredFieldValidatorIdentityNo.Enabled = RequiredFieldValidatorNameOfChild.Enabled = NricFinCustomValidator.Enabled = isEnabled;
    }
}
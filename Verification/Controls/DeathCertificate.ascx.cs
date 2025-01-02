using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Control_DeathCertificate : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DeathCertificate";

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
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        DateOfDeath.DateValue = string.Empty;
        DateOfDeath.DateIdValue = string.Empty;

        TagRadioButtonList.SelectedIndex = 0;
        TagRadioButtonListId.Value = string.Empty;
        lblDateRangeError.Visible = false;
        lblDateRangeError.Visible = false;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateEnum.DateOfDeath.ToString().ToLower()))
                {
                    DateOfDeath.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfDeath.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDeathCertificateEnum.Tag.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Equals(TagEnum.Local_Civil.ToString()) || metaDataRow.FieldValue.Equals(TagEnum.Local_Muslim.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Local.ToString();
                    else if (metaDataRow.FieldValue.Equals(TagGeneralEnum.Foreign.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Foreign.ToString();
                    //TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));  Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = DateOfDeath.EnableRequiredField = isEnabled;
    //}
    #endregion


    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfDeath.DateValue, DateTime.Now.ToString()) || !isValidate || DateOfDeath.EnableRequiredField == false)
        {
            SaveDataDetails(docId, docRefId, referencePersonalTable);
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfDeath.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfDeath.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate || DateOfDeath.EnableRequiredField == false)
        {
            SaveDataDetails(docId, docRefId, referencePersonalTable);
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
    private void SaveDataDetails(int docId, int docRefId, string referencePersonalTable)
    {
        //Save Perosnal Data
        CustomPersonal customPersonal = new CustomPersonal();
        //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
        //Added by Edward 2016/03/23 Optimize Doctype Saving
        customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType,
            CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
        //save metadata fields
        MetaDataDb metadataDb = new MetaDataDb();

        if (DateOfDeath.DateIdValue.Length == 0)
            DateOfDeath.DateIdValue = metadataDb.Insert(DocTypeMetaDataDeathCertificateEnum.DateOfDeath.ToString(), DateOfDeath.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfDeath.DateIdValue), DateOfDeath.DateValue);

        if (TagRadioButtonListId.Value.Length == 0)
            TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificateEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);
    }
    #endregion


    #endregion
}
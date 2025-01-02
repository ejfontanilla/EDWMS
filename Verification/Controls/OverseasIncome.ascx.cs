using System;
using System.Web.UI.WebControls;
using Dwms.Bll;


public partial class Verification_Control_OverseasIncome : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "OverseasIncome";

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
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        FromDate.DateValue = ToDate.DateValue = string.Empty;
        FromDate.DateIdValue = ToDate.DateIdValue = string.Empty;
        lblDateRangeError.Visible = false;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();


        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataOverseasIncomeEnum.StartDate.ToString().ToLower()))
                {
                    FromDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    FromDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataOverseasIncomeEnum.EndDate.ToString().ToLower()))
                {
                    ToDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    ToDate.DateIdValue = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(FromDate.DateValue, ToDate.DateValue) || !isValidate)
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
        if (Validation.IsValidDateRange(FromDate.DateValue, ToDate.DateValue, IsBlurOrIncomplete) || !isValidate)
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
        if (FromDate.DateIdValue.Length == 0)
            FromDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataOverseasIncomeEnum.StartDate.ToString(), FromDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(FromDate.DateIdValue), FromDate.DateValue);

        if (ToDate.DateIdValue.Length == 0)
            ToDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataOverseasIncomeEnum.EndDate.ToString(), ToDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(ToDate.DateIdValue), ToDate.DateValue);
    }
    #endregion

    #endregion
}
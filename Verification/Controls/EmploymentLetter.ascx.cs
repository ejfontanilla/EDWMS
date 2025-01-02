using System;
using System.Web.UI.WebControls;
using Dwms.Bll;


public partial class Verification_Control_EmploymentLetter : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "EmploymentLetter";

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
        FromDate.DateValue = string.Empty;// = NameOfCompany.Text
        FromDate.DateIdValue = string.Empty;// = NameOfCompanyId.Value
        //AllceRadioButtonListId.Value = string.Empty;
        //AllceRadioButtonList.SelectedIndex = 1;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        // "name of the company" is only for COS
        //get the docsetId
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        //SectionDb sectionDb = new SectionDb();
        //AllceTR.Visible = NameOfCompanyTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

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

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataEmploymentLetterEnum.StartDate.ToString().ToLower()))
                {
                    FromDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    FromDate.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataEmploymentLetterEnum.NameOfCompany.ToString().ToLower()))
                //{
                //    NameOfCompany.Text = metaDataRow.FieldValue;
                //    NameOfCompanyId.Value = metaDataRow.Id.ToString();
                //}
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataEmploymentLetterEnum.Allowance.ToString().ToLower()))
                //{
                //    AllceRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                //    AllceRadioButtonListId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(FromDate.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(FromDate.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(FromDate.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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
            FromDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataEmploymentLetterEnum.StartDate.ToString(), FromDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(FromDate.DateIdValue), FromDate.DateValue);   
    }
    #endregion

    #endregion
}
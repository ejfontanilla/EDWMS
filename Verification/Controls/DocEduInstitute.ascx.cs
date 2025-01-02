using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Controls_DocEduInstitute : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "DocEduInstitute";

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
        lblDateRangeError2.Visible = false;
        if (!IsPostBack)
        {
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        FromDate.DateValue = ToDate.DateValue = string.Empty;//= NameOfCompany.Text
        FromDate.DateIdValue = ToDate.DateIdValue = string.Empty;//= NameOfCompanyId.Value
        lblDateRangeError.Visible = false;
        //AllceRadioButtonListId.Value = string.Empty;
        //AllceRadioButtonList.SelectedValue = DocTypeMetaDataValueYesNoEnum.No.ToString();
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        // restric "name of the company" and "Allowance" are for COS only
        //get the docsetId
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        // if the set is from AAD, 
        SectionDb sectionDb = new SectionDb();
        //AllowanceTR.Visible = NameOfCompanyTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1,docRow.DocSetId);

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

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDocEduInstituteEnum.StartDate.ToString().ToLower()))
                {
                    FromDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    FromDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataDocEduInstituteEnum.EndDate.ToString().ToLower()))
                {
                    ToDate.DateValue = Format.GetMetaDataValueInMetaDataEndDateFormat(metaDataRow.FieldValue);
                    ToDate.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPAYSLIPEnum.NameOfCompany.ToString().ToLower()))
                //{
                //    NameOfCompany.Text = metaDataRow.FieldValue;
                //    NameOfCompanyId.Value = metaDataRow.Id.ToString();
                //}
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPAYSLIPEnum.Allowance.ToString().ToLower()))
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
        lblDateRangeError2.Visible = false;
        if ((Validation.IsValidDateRangePayExp(FromDate.DateValue, ToDate.DateValue) && Validation.IsValidDateRangePayExp(FromDate.DateValue, DateTime.Now.ToString())) || !isValidate)
        {
            if (Validation.IsValidDateRangePayExp(ToDate.DateValue, DateTime.Now.AddMonths(1).ToString()) || !isValidate)
            {
                SaveDataDetails(docId, docRefId, referencePersonalTable);
            }
            else
            {
                lblDateRangeError2.Visible = true;
                validDate = false;
            }
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
        lblDateRangeError2.Visible = false;

        string strDateNow = DateTime.Now.ToString();

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(FromDate.DateValue))
            strDateNow = string.Empty;

        if ((Validation.IsValidDateRangePayExp(FromDate.DateValue, ToDate.DateValue, IsBlurOrIncomplete) && Validation.IsValidDateRangePayExp(FromDate.DateValue, strDateNow, IsBlurOrIncomplete)) || !isValidate)
        {
            string strDateNowPlusOne = DateTime.Now.AddMonths(1).ToString();

            if (IsBlurOrIncomplete && string.IsNullOrEmpty(ToDate.DateValue))
                strDateNowPlusOne = string.Empty;

            if (Validation.IsValidDateRangePayExp(ToDate.DateValue, strDateNowPlusOne, IsBlurOrIncomplete) || !isValidate)
            {
                SaveDataDetails(docId, docRefId, referencePersonalTable);
            }
            else
            {
                lblDateRangeError2.Visible = true;
                validDate = false;
            }
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
            FromDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataDocEduInstituteEnum.StartDate.ToString(), FromDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(FromDate.DateIdValue), FromDate.DateValue);

        if (ToDate.DateIdValue.Length == 0)
            ToDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataDocEduInstituteEnum.EndDate.ToString(), ToDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(ToDate.DateIdValue), ToDate.DateValue);
    }
    #endregion

    #endregion
}
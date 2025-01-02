using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Control_CPFContribution : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "CPFContribution";

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
        FromDate.DateValue = ToDate.DateValue = string.Empty;//= CompanyName1.Text = CompanyName2.Text 
        FromDate.DateIdValue = ToDate.DateIdValue = string.Empty;
        lblDateRangeError.Visible = false;
        //ConsistentContributionRadioButtonList.SelectedValue = DocTypeMetaDataValueCPFEnum.No.ToString();
        //ConsistentContributionRadioButtonListId.Value = string.Empty;
        //CompanyName1Id.Value = CompanyName2Id.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        //Business Type is only for COS
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        SectionDb sectionDb = new SectionDb();
        //ConsistentContributionTR.Visible = CompanyName1TR.Visible = CompanyName2TR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

        CustomPersonal customPersonal = new CustomPersonal();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataCPFContributionEnum.StartDate.ToString().ToLower()))
                {
                    FromDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    FromDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataCPFContributionEnum.EndDate.ToString().ToLower()))
                {
                    ToDate.DateValue = Format.GetMetaDataValueInMetaDataEndDateFormat(metaDataRow.FieldValue);
                    ToDate.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataCPFContributionEnum.CompanyName1.ToString().ToLower()))
                //{
                //    CompanyName1.Text = metaDataRow.FieldValue;
                //    CompanyName1Id.Value = metaDataRow.Id.ToString();
                //}
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataCPFContributionEnum.CompanyName2.ToString().ToLower()))
                //{
                //    CompanyName2.Text = metaDataRow.FieldValue;
                //    CompanyName2Id.Value = metaDataRow.Id.ToString();
                //}
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataCPFContributionEnum.ConsistentContribution.ToString().ToLower()))
                //{
                //    ConsistentContributionRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                //    ConsistentContributionRadioButtonListId.Value = metaDataRow.Id.ToString();
                //}
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
            FromDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataCPFContributionEnum.StartDate.ToString(), FromDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(FromDate.DateIdValue), FromDate.DateValue);

        if (ToDate.DateIdValue.Length == 0)
            ToDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataCPFContributionEnum.EndDate.ToString(), ToDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(ToDate.DateIdValue), ToDate.DateValue);
    }
    #endregion


    #endregion
}
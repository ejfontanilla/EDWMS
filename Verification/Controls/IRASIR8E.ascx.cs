using System;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_IRASIR8E : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "IRASIR8E";

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
        //if (!IsPostBack)
        //{
        //    PopulateTypeOfIncome();
        //}
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        YearOfAssessment.Text = DateOfFiling.DateValue = string.Empty;
        //TypeOfIncome.SelectedIndex = 0;
        YearOfAssessmentId.Value = DateOfFiling.DateIdValue  = string.Empty;//= TypeOfIncomeId.Value
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        CustomPersonal customPersonal = new CustomPersonal();

        //Business Type is only for COS
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        //SectionDb sectionDb = new SectionDb();
        //TypeOfIncomeTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            //PopulateTypeOfIncome();

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIRASIR8EEnum.YearOfAssessment.ToString().ToLower()))
                {
                    YearOfAssessment.Text = metaDataRow.FieldValue;
                    YearOfAssessmentId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIRASIR8EEnum.DateOfFiling.ToString().ToLower()))
                {
                    DateOfFiling.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfFiling.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataIRASIR8EEnum.TypeOfIncome.ToString().ToLower()))
                //{
                //    TypeOfIncome.SelectedValue = metaDataRow.FieldValue;
                //    TypeOfIncomeId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    //public void PopulateTypeOfIncome()
    //{
    //    TypeOfIncome.Items.Clear();
    //    foreach (DocTypeMetaDataValueIRASAssesementEnum val in Enum.GetValues(typeof(DocTypeMetaDataValueIRASAssesementEnum)))
    //    {
    //        TypeOfIncome.Items.Add(new RadComboBoxItem(val.ToString(), val.ToString()));
    //    }
    //    TypeOfIncome.DataBind();
    //}

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfFiling.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfFiling.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfFiling.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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
        if (YearOfAssessmentId.Value.Length == 0)
            YearOfAssessmentId.Value = metadataDb.Insert(DocTypeMetaDataIRASIR8EEnum.YearOfAssessment.ToString(), YearOfAssessment.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(YearOfAssessmentId.Value), YearOfAssessment.Text);

        if (DateOfFiling.DateIdValue.Length == 0)
            DateOfFiling.DateIdValue = metadataDb.Insert(DocTypeMetaDataIRASIR8EEnum.DateOfFiling.ToString(), DateOfFiling.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfFiling.DateIdValue), DateOfFiling.DateValue);

        //if (TypeOfIncomeId.Value.Length == 0)
        //    TypeOfIncomeId.Value = metadataDb.Insert(DocTypeMetaDataIRASIR8EEnum.TypeOfIncome.ToString(), TypeOfIncome.SelectedValue, docId, false).ToString();
        //else
        //    metadataDb.Update(int.Parse(TypeOfIncomeId.Value), TypeOfIncome.SelectedValue);
    }
    #endregion

    #endregion
}
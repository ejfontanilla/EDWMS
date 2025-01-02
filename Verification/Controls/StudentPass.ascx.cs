using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_StudentPass : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "StudentPass";

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
            PopulateEducationLevel();
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        DateOfIssue.DateValue = string.Empty;
        EducationLevel.SelectedIndex = 0;
        DateOfIssue.DateIdValue = EducationLevelId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        //Education Level is only for COS
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        SectionDb sectionDb = new SectionDb();
        EducationLevelTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateEducationLevel();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStudentPassEnum.DateOfIssue.ToString().ToLower()))
                {
                    DateOfIssue.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfIssue.DateIdValue = metaDataRow.Id.ToString();
                }
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStudentPassEnum.EducationLevel.ToString().ToLower()))
                {
                    EducationLevel.SelectedValue = metaDataRow.FieldValue;
                    EducationLevelId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public void PopulateEducationLevel()
    {
        EducationLevel.Items.Clear();
        foreach (DocTypeMetaDataValueStudentPassEnum val in Enum.GetValues(typeof(DocTypeMetaDataValueStudentPassEnum)))
        {
            EducationLevel.Items.Add(new RadComboBoxItem(val.ToString(), val.ToString()));
        }
        EducationLevel.DataBind();
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfIssue.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfIssue.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfIssue.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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

        if (DateOfIssue.DateIdValue.Length == 0)
            DateOfIssue.DateIdValue = metadataDb.Insert(DocTypeMetaDataStudentPassEnum.DateOfIssue.ToString(), DateOfIssue.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfIssue.DateIdValue), DateOfIssue.DateValue);

        if (EducationLevelId.Value.Length == 0)
            EducationLevelId.Value = metadataDb.Insert(DocTypeMetaDataStudentPassEnum.EducationLevel.ToString(), EducationLevel.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(EducationLevelId.Value), EducationLevel.SelectedValue);
    }
    #endregion

    #endregion
}
using System;
using System.Web.UI.WebControls;
//using Telerik.Web.UI;
using Dwms.Bll;


public partial class Verification_Control_StatutoryDeclaration : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "StatutoryDeclaration";

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
        //if (!IsPostBack)
        //{
        //    PopulateType();
        //}
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        FromDate.DateValue = ToDate.DateValue = DateOfDeclaration.DateValue = string.Empty;
        //Type.SelectedIndex = 0;
        FromDate.DateIdValue = ToDate.DateIdValue = DateOfDeclaration.DateIdValue = string.Empty;
        //TypeId.Value = string.Empty;
        lblDateRangeError.Visible = false;
        lblDateRangeError2.Visible = false;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        // restric "name of the company" required field based on docset department
        //get the docsetId
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        // if the set is from AAD, make Name of the company as compulsory. "1"
        //SectionDb sectionDb = new SectionDb();
        //TypeTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //PopulateType();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatutoryDeclarationEnum.StartDate.ToString().ToLower()))
                {
                    FromDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    FromDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatutoryDeclarationEnum.EndDate.ToString().ToLower()))
                {
                    ToDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    ToDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatutoryDeclarationEnum.DateOfDeclaration.ToString().ToLower()))
                {
                    DateOfDeclaration.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfDeclaration.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatutoryDeclarationEnum.Type.ToString().ToLower()))
                //{
                //    Type.SelectedValue = metaDataRow.FieldValue;
                //    TypeId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    //public void PopulateType()
    //{
    //    Type.Items.Clear();
    //    foreach (DocTypeMetaDataValueStatutoryDeclarationEnum val in Enum.GetValues(typeof(DocTypeMetaDataValueStatutoryDeclarationEnum)))
    //    {
    //        Type.Items.Add(new RadComboBoxItem(val.ToString().Replace("_", " "), val.ToString()));
    //    }
    //    Type.DataBind();
    //}

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        lblDateRangeError2.Visible = false;
        if (Validation.IsValidDateRange(FromDate.DateValue, ToDate.DateValue) || !isValidate)
        {
            if (Validation.IsValidDateRange(DateOfDeclaration.DateValue, DateTime.Now.ToString()) || !isValidate)
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
        if (Validation.IsValidDateRange(FromDate.DateValue, ToDate.DateValue, IsBlurOrIncomplete) || !isValidate)
        {
            string strDateNow = DateTime.Now.ToString();

            if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfDeclaration.DateValue))
                strDateNow = string.Empty;

            if (Validation.IsValidDateRange(DateOfDeclaration.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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
            FromDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataStatutoryDeclarationEnum.StartDate.ToString(), FromDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(FromDate.DateIdValue), FromDate.DateValue);

        if (ToDate.DateIdValue.Length == 0)
            ToDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataStatutoryDeclarationEnum.EndDate.ToString(), ToDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(ToDate.DateIdValue), ToDate.DateValue);

        if (DateOfDeclaration.DateIdValue.Length == 0)
            DateOfDeclaration.DateIdValue = metadataDb.Insert(DocTypeMetaDataStatutoryDeclarationEnum.DateOfDeclaration.ToString(), DateOfDeclaration.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfDeclaration.DateIdValue), DateOfDeclaration.DateValue);
    }
    #endregion

    #endregion
}
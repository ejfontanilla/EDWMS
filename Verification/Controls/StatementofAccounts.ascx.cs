using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;


public partial class Verification_Control_StatementofAccounts : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "StatementofAccounts";

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
        StartDate.DateValue = EndDate.DateValue = string.Empty;// = NameOfCompany.Text
        StartDate.DateIdValue = EndDate.DateIdValue = string.Empty;// = NameOfCompanyId.Value
        lblDateRangeError.Visible = false;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();


        //Business Type is only for COS
        DocDb docDb = new DocDb();
        Doc.DocDataTable docs = docDb.GetDocById(docId);
        Doc.DocRow docRow = docs[0];

        //SectionDb sectionDb = new SectionDb();
        //BusinessTypeTR.Visible = sectionDb.IsDocSetSectionFromDepartment(1, docRow.DocSetId);

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

                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatementOfAccountsEnum.StartDate.ToString().ToLower()))
                {
                    StartDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    StartDate.DateIdValue = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatementOfAccountsEnum.EndDate.ToString().ToLower()))
                {
                    EndDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    EndDate.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataStatementOfAccountsEnum.NameOfCompany.ToString().ToLower()))
                //{
                //    NameOfCompany.Text = metaDataRow.FieldValue;
                //    NameOfCompanyId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(StartDate.DateValue, EndDate.DateValue) || !isValidate)
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
        if (Validation.IsValidDateRange(StartDate.DateValue, EndDate.DateValue, IsBlurOrIncomplete) || !isValidate)
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

        if (StartDate.DateIdValue.Length == 0)
            StartDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataStatementOfAccountsEnum.StartDate.ToString(), StartDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(StartDate.DateIdValue), StartDate.DateValue);

        if (EndDate.DateIdValue.Length == 0)
            EndDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataStatementOfAccountsEnum.EndDate.ToString(), EndDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(EndDate.DateIdValue), EndDate.DateValue);

        //if (NameOfCompanyId.Value.Length == 0)
        //    NameOfCompanyId.Value = metadataDb.Insert(DocTypeMetaDataStatementOfAccountsEnum.NameOfCompany.ToString(), NameOfCompany.Text, docId, false).ToString();
        //else
        //    metadataDb.Update(int.Parse(NameOfCompanyId.Value), NameOfCompany.Text);
    }
    #endregion

    #endregion
}
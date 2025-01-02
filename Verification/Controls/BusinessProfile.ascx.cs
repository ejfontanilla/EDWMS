﻿using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_BusinessProfile : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "BusinessProfile";

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
        //    PopulateBusinessType();
        //}
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        DateOfRegistration.DateValue = UENNo.Text = string.Empty;
        DateOfRegistration.DateIdValue = string.Empty;
        UENNoId.Value = string.Empty;//BusinessTypeId.Value = 
        //BusinessType.SelectedIndex = 0;
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

            //PopulateBusinessType();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBusinessProfileEnum.UENNo.ToString().ToLower()))
                {
                    UENNo.Text = metaDataRow.FieldValue;
                    UENNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBusinessProfileEnum.DateOfRegistration.ToString().ToLower()))
                {
                    DateOfRegistration.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfRegistration.DateIdValue = metaDataRow.Id.ToString();
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBusinessProfileEnum.BusinessType.ToString().ToLower()))
                //{
                //    BusinessType.SelectedValue = metaDataRow.FieldValue;
                //    BusinessTypeId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    //public void PopulateBusinessType()
    //{
    //    BusinessType.Items.Clear();
    //    foreach (BusinessTypeEnum val in Enum.GetValues(typeof(BusinessTypeEnum)))
    //    {
    //        BusinessType.Items.Add(new RadComboBoxItem(val.ToString().Replace("_", " "), val.ToString()));
    //    }
    //    BusinessType.DataBind();
    //}

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfRegistration.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfRegistration.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfRegistration.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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
        if (DateOfRegistration.DateIdValue.Length == 0)
            DateOfRegistration.DateIdValue = metadataDb.Insert(DocTypeMetaDataBusinessProfileEnum.DateOfRegistration.ToString(), DateOfRegistration.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfRegistration.DateIdValue), DateOfRegistration.DateValue);

        if (UENNoId.Value.Length == 0)
            UENNoId.Value = metadataDb.Insert(DocTypeMetaDataBusinessProfileEnum.UENNo.ToString(), UENNo.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(UENNoId.Value), UENNo.Text);    
    }
    #endregion

    #endregion
}
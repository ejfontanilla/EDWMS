﻿using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Control_OrderofCourt : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "OrderofCourt";

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
        CourtOrderDate.DateValue = string.Empty;
        CourtOrderDate.DateIdValue = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataOrderofCourtEnum.CourtOrderDate.ToString().ToLower()))
                {
                    CourtOrderDate.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    CourtOrderDate.DateIdValue = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(CourtOrderDate.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(CourtOrderDate.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(CourtOrderDate.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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

        if (CourtOrderDate.DateIdValue.Length == 0)
            CourtOrderDate.DateIdValue = metadataDb.Insert(DocTypeMetaDataOrderofCourtEnum.CourtOrderDate.ToString(), CourtOrderDate.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(CourtOrderDate.DateIdValue), CourtOrderDate.DateValue);
    }
    #endregion

    #endregion
}
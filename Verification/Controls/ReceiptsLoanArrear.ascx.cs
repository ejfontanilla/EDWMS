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

public partial class Verification_Control_ReceiptsLoanArrear : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "ReceiptsLoanArrear";

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
        ABCDERef.Text = DateOfStatement.DateValue = string.Empty;
        ABCDERefId.Value = DateOfStatement.DateIdValue = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataReceiptsLoanArrearEnum.ABCDERef.ToString().ToLower()))
                {
                    ABCDERef.Text = metaDataRow.FieldValue;
                    ABCDERefId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataReceiptsLoanArrearEnum.DateOfStatement.ToString().ToLower()))
                {
                    DateOfStatement.DateValue = Format.GetMetaDataValueInMetaDataDateFormat(metaDataRow.FieldValue);
                    DateOfStatement.DateIdValue = metaDataRow.Id.ToString();
                }
            }
        }
        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsValidDateRange(DateOfStatement.DateValue, DateTime.Now.ToString()) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(DateOfStatement.DateValue))
            strDateNow = string.Empty;

        if (Validation.IsValidDateRange(DateOfStatement.DateValue, strDateNow, IsBlurOrIncomplete) || !isValidate)
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

        if (ABCDERefId.Value.Length == 0)
            ABCDERefId.Value = metadataDb.Insert(DocTypeMetaDataReceiptsLoanArrearEnum.ABCDERef.ToString(), ABCDERef.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(ABCDERefId.Value), ABCDERef.Text);

        if (DateOfStatement.DateIdValue.Length == 0)
            DateOfStatement.DateIdValue = metadataDb.Insert(DocTypeMetaDataReceiptsLoanArrearEnum.DateOfStatement.ToString(), DateOfStatement.DateValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(DateOfStatement.DateIdValue), DateOfStatement.DateValue);
    }
    #endregion

    #endregion
}
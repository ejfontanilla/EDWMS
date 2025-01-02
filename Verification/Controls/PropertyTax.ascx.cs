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

public partial class Verification_Controls_PropertyTax : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "PropertyTax";

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

    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        YearOfPropertyTax.Text = string.Empty;
        YearOfPropertyTaxId.Value = string.Empty;
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
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            //PopulateTypeOfIncome();

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataPropertyTaxEnum.YearOfPropertyTax.ToString().ToLower()))
                {
                    YearOfPropertyTax.Text = metaDataRow.FieldValue;
                    YearOfPropertyTaxId.Value = metaDataRow.Id.ToString();
                }                
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public bool SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        bool validDate = true;
        lblDateRangeError.Visible = false;
        if (Validation.IsInteger(YearOfPropertyTax.Text) || !isValidate)
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

        if (IsBlurOrIncomplete && string.IsNullOrEmpty(YearOfPropertyTax.Text))
            strDateNow = string.Empty;

        if (Validation.IsInteger(YearOfPropertyTax.Text) || !isValidate)
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
        if (YearOfPropertyTaxId.Value.Length == 0)
        {
            YearOfPropertyTaxId.Value = metadataDb.Insert(DocTypeMetaDataPropertyTaxEnum.YearOfPropertyTax.ToString(), YearOfPropertyTax.Text, docId, false).ToString();
            FromDateId.Value = metadataDb.Insert(DocTypeMetaDataPAYSLIPEnum.StartDate.ToString(), string.Format("01-01-{0}", YearOfPropertyTax.Text), docId, false).ToString();
            ToDateId.Value = metadataDb.Insert(DocTypeMetaDataPAYSLIPEnum.EndDate.ToString(), string.Format("31-12-{0}", YearOfPropertyTax.Text), docId, false).ToString();
        }
        else
        { 
            metadataDb.Update(int.Parse(YearOfPropertyTaxId.Value), YearOfPropertyTax.Text);
            metadataDb.Update(int.Parse(FromDateId.Value), string.Format("01-01-{0}", YearOfPropertyTax.Text));
            metadataDb.Update(int.Parse(ToDateId.Value), string.Format("31-12-{0}", YearOfPropertyTax.Text));
        }


    }
    #endregion

    

    #endregion
}
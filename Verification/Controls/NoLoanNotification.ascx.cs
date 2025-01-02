using System;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Verification_Control_NoLoanNotification : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "NoLoanNotification";

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
        if (!IsPostBack)
        {
            
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
        DateOfSignatureRadioButtonListId.Value = string.Empty;
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
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataNoLoanNotificationEnum.DateOfSignature.ToString().ToLower()) && metaDataRow.FieldValue.Trim().Length > 0)
                {
                    DateOfSignatureRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    DateOfSignatureRadioButtonListId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public void SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        //Modified by Edward, Commented out if(!isValidate) 2014/11/03 - No Loan Notification Cannot save date of signature when confirm click
        //if (!isValidate)
        //{
            //Save Perosnal Data
            CustomPersonal customPersonal = new CustomPersonal();
            //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
            //Added by Edward 2016/03/23 Optimize Doctype Saving
            customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType,
                CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
            //save metadata fields
            MetaDataDb metadataDb = new MetaDataDb();

            if (DateOfSignatureRadioButtonList.SelectedValue.Length > 0)
            {
                if (DateOfSignatureRadioButtonListId.Value.Length == 0)
                    DateOfSignatureRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataNoLoanNotificationEnum.DateOfSignature.ToString(), DateOfSignatureRadioButtonList.SelectedValue, docId, false).ToString();
                else
                    metadataDb.Update(int.Parse(DateOfSignatureRadioButtonListId.Value), DateOfSignatureRadioButtonList.SelectedValue);
            }
        //}
        //else
        //{
        //    return false;
        //}
        //return true;
    }   
    #endregion
}
using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_BirthCertificate : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "BirthCertificate"; 

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
        TagRadioButtonList.SelectedIndex = 0;
        TagRadioButtonListId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        CustomPersonal customPersonal = new CustomPersonal();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBirthCertificateEnum.Tag.ToString().ToLower()))
                {
                    //Added .toString() in all TagEnums by Edward 2014/08/19 BirthCertificate Tag reset to Local even Foreign was saved
                    if (metaDataRow.FieldValue.Equals(TagEnum.Local_Civil.ToString()) || metaDataRow.FieldValue.Equals(TagEnum.Local_Muslim.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Local.ToString();
                    else if (metaDataRow.FieldValue.Equals(TagGeneralEnum.Foreign.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Foreign.ToString();
                    //TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
    }

    protected void SetRequiredField(Boolean isEnabled)
    {
        CommonMetadata.EnableRequiredField = isEnabled;
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
            MetaDataDb metadataDb = new MetaDataDb();

            if (TagRadioButtonListId.Value.Length == 0)
                TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificateEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);
        //}
        //else
        //{
        //    return false;
        //}
        //return true;
    }

    #endregion
}
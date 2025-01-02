using System;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_Adoption : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "Adoption";

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
        //NameOfChild.Text = string.Empty;
        TagRadioButtonList.SelectedIndex = 0;
        //NameOfChildId.Value = TagRadioButtonListId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataAdoptionEnum.Tag.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Equals(TagEnum.Local_Civil.ToString()) || metaDataRow.FieldValue.Equals(TagEnum.Local_Muslim.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Local.ToString();
                    else if (metaDataRow.FieldValue.Equals(TagGeneralEnum.Foreign.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Foreign.ToString();
                    //TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
                }
                //else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataAdoptionEnum.NameOfChild.ToString().ToLower()))
                //{
                //    NameOfChild.Text = metaDataRow.FieldValue;
                //    NameOfChildId.Value = metaDataRow.Id.ToString();
                //}
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    //protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
    //}

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    RequiredFieldValidatorNameOfChild.Enabled = isEnabled;
    //}


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

            //save meta data
            MetaDataDb metadataDb = new MetaDataDb();

            if (TagRadioButtonListId.Value.Length == 0)
                TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataAdoptionEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

            //if (NameOfChildId.Value.Length == 0)
            //    NameOfChildId.Value = metadataDb.Insert(DocTypeMetaDataAdoptionEnum.NameOfChild.ToString(), NameOfChild.Text, docId, false).ToString();
            //else
            //    metadataDb.Update(int.Parse(NameOfChildId.Value), NameOfChild.Text);
        //}
        //else
        //{
        //    return false;
        //}
        //return true;
    }

    #endregion
}
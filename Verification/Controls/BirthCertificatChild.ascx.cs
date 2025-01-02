using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_BirthCertificatChild : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "BirthCertificatChild"; 

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
            PopulateIDType();
        }
    }

    public void ClearData()
    {
        CommonMetadata.Clear();       
        NameOfChild.Text = IdentityNo.Text = string.Empty;
        IDType.ClearSelection();
        TagRadioButtonList.SelectedIndex = 0;
        IDTypeId.Value = TagRadioButtonListId.Value = string.Empty;
        NameOfChildId.Value = IdentityNoId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();
        CustomPersonal customPersonal = new CustomPersonal();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();

            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBirthCertificatChildEnum.Tag.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Equals(TagEnum.Local_Civil.ToString()) || metaDataRow.FieldValue.Equals(TagEnum.Local_Muslim.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Local.ToString();
                    else if (metaDataRow.FieldValue.Equals(TagGeneralEnum.Foreign.ToString()))
                        TagRadioButtonList.SelectedValue = TagGeneralEnum.Foreign.ToString();
                    //TagRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                    TagRadioButtonListId.Value = metaDataRow.Id.ToString();
                    //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local")); Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBirthCertificatChildEnum.IDType.ToString().ToLower()))
                {
                    if (metaDataRow.FieldValue.Length > 0) IDType.SelectedValue = metaDataRow.FieldValue;
                    IDTypeId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBirthCertificatChildEnum.IdentityNo.ToString().ToLower()))
                {
                    IdentityNo.Text = metaDataRow.FieldValue;
                    IdentityNoId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataBirthCertificatChildEnum.NameOfChild.ToString().ToLower()))
                {
                    NameOfChild.Text = metaDataRow.FieldValue;
                    NameOfChildId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    #region Commented by Edward 2016/06/07 IdNo should be validated regardless of local or foreign for all Doc Types
    protected void TagRadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
    {
        //SetRequiredField(TagRadioButtonList.SelectedValue.ToString().ToLower().Equals("local"));
    }

    //protected void SetRequiredField(Boolean isEnabled)
    //{
    //    CommonMetadata.EnableRequiredField = RequiredFieldValidatorIdentityNo.Enabled = RequiredFieldValidatorNameOfChild.Enabled = NricFinCustomValidator.Enabled = isEnabled;
    //}
    #endregion

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {        
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDType.SelectedValue, args.Value.ToString().Trim());
    }

    public void PopulateIDType()
    {
        IDType.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDType.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDType.DataBind();

        //IDType.SelectedValue = IDTypeEnum.UIN.ToString();   //Uncommented by Edward on 2015/07/31 for Investigating CBD Errors
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

            //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
            if (isValidate)
                IdentityNo.Text = Validation.ReplaceSpecialCharacters(IdentityNo.Text);

            //Added by Edward 2016/06/10 To Take out non letter characters for Name
            if (isValidate)
                NameOfChild.Text = Validation.ReplaceNonLetterCharacters(NameOfChild.Text);

            if (TagRadioButtonListId.Value.Length == 0)
                TagRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificatChildEnum.Tag.ToString(), TagRadioButtonList.SelectedValue, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(TagRadioButtonListId.Value), TagRadioButtonList.SelectedValue);

            if (IDTypeId.Value.Length == 0)
                IDTypeId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificatChildEnum.IDType.ToString(), IDType.SelectedValue, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(IDTypeId.Value), IDType.SelectedValue);            

            if (IdentityNoId.Value.Length == 0)                            
                IdentityNoId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificatChildEnum.IdentityNo.ToString(), IdentityNo.Text, docId, false).ToString();            
            else
                metadataDb.Update(int.Parse(IdentityNoId.Value), IdentityNo.Text);

            if (NameOfChildId.Value.Length == 0)
                NameOfChildId.Value = metadataDb.Insert(DocTypeMetaDataBirthCertificatChildEnum.NameOfChild.ToString(), NameOfChild.Text, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(NameOfChildId.Value), NameOfChild.Text);
        //}
        //else
        //{
        //    return false;
        //}
        //return true;
    }

    #endregion
}
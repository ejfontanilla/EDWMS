using System;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_SpouseFormSale : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    protected string DocType = "SpouseFormSale";

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
        SpouseID.Text = SpouseName.Text = string.Empty;
        IDType.SelectedIndex = 0;
        SpouseIDId.Value = SpouseNameId.Value = IDTypeId.Value = string.Empty;
    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        if (CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {
            //load nric and name fields
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);

            PopulateIDType();

            //load metadata fields
            MetaDataDb metadataDb = new MetaDataDb();
            MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

            foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
            {
                if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataSpouseFormPurchaseEnum.IDType.ToString().ToLower()))
                {
                    IDType.SelectedValue = metaDataRow.FieldValue;
                    IDTypeId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataSpouseFormPurchaseEnum.SpouseID.ToString().ToLower()))
                {
                    SpouseID.Text = metaDataRow.FieldValue;
                    SpouseIDId.Value = metaDataRow.Id.ToString();
                }
                else if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataSpouseFormPurchaseEnum.SpouseName.ToString().ToLower()))
                {
                    SpouseName.Text = metaDataRow.FieldValue;
                    SpouseNameId.Value = metaDataRow.Id.ToString();
                }
            }
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public void PopulateIDType()
    {
        IDType.Items.Clear();
        foreach (IDTypeEnum val in Enum.GetValues(typeof(IDTypeEnum)))
        {
            IDType.Items.Add(new ListItem(val.ToString().Replace("_", " "), val.ToString()));
        }
        IDType.DataBind();

        //IDType.SelectedValue = IDTypeEnum.UIN.ToString();
    }

    protected void NricCustomValidatorSpouse(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.ValidateUINFINXINBasedOnIDType(IDType.SelectedValue, args.Value.ToString().Trim());
    }

    public void SaveData(int docId, int docRefId, string referencePersonalTable)
    {
        //Save Perosnal Data
        CustomPersonal customPersonal = new CustomPersonal();
        //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
        //Added by Edward 2016/03/23 Optimize Doctype Saving
        customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, DocType,
            CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
        //save metadata fields
        MetaDataDb metadataDb = new MetaDataDb();

        //Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected        
        SpouseID.Text = Validation.ReplaceSpecialCharacters(SpouseID.Text);

        //Added by Edward 2016/06/10 To Take out non letter characters for Name        
        SpouseName.Text = Validation.ReplaceNonLetterCharacters(SpouseName.Text);

        if (IDTypeId.Value.Length == 0)
            IDTypeId.Value = metadataDb.Insert(DocTypeMetaDataSpouseFormPurchaseEnum.IDType.ToString(), IDType.SelectedValue, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(IDTypeId.Value), IDType.SelectedValue);

        if (SpouseIDId.Value.Length == 0)
            SpouseIDId.Value = metadataDb.Insert(DocTypeMetaDataSpouseFormPurchaseEnum.SpouseID.ToString(), SpouseID.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(SpouseIDId.Value), SpouseID.Text);

        if (SpouseNameId.Value.Length == 0)
            SpouseNameId.Value = metadataDb.Insert(DocTypeMetaDataSpouseFormPurchaseEnum.SpouseName.ToString(), SpouseName.Text, docId, false).ToString();
        else
            metadataDb.Update(int.Parse(SpouseNameId.Value), SpouseName.Text);
    }

    #endregion
}
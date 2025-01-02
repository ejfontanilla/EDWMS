using System;
using Dwms.Bll;

public partial class Verification_Control_Empty : System.Web.UI.UserControl
{
    private String _CurrentDocType;
    private String _ActualDocType;

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

    public string ActualDocType
    {
        get
        {
            return _ActualDocType;
        }
        set
        {
            _ActualDocType = value;
        }
    }

    #region Event handlers
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void LoadData(int docId, int docRefId, string referencePersonalTable, Boolean isUnderHouseholdStructure)
    {
        ClearData();

        if (CurrentDocType.ToLower().Equals(ActualDocType.ToLower()))
        {
            //load nric
            CustomPersonal customPersonal = new CustomPersonal();
            customPersonal.Load(docId, referencePersonalTable, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, CommonMetadata.NricIdHiddenField,  CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
        }

        CommonMetadata.RestrictAccess(isUnderHouseholdStructure);
    }

    public void ClearData()
    {
        CommonMetadata.Clear();
    }

    public void SaveData(int docId, int docRefId, string referencePersonalTable, bool isValidate)
    {
        if (!isValidate)
        {
            //Save Perosnal Data
            CustomPersonal customPersonal = new CustomPersonal();
            //customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, ActualDocType, CommonMetadata.NricTextBox, CommonMetadata.NameTextBox, true, CommonMetadata.IDTypeRadioButtonList, CommonMetadata.CustomerSourceIdHiddenField);
            //Added by Edward 2016/03/23 Optimize Doctype Saving
            customPersonal.Save(docId, docRefId, referencePersonalTable, CurrentDocType, ActualDocType,
                CommonMetadata.NricTextBox.Text, CommonMetadata.NameTextBox.Text, true, CommonMetadata.IDTypeRadioButtonList.SelectedValue, CommonMetadata.CustomerSourceIdHiddenField.Value);
        }
        //else
        //{
        //    return false;
        //}
        //return true;
    }
    #endregion
}
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Verification_Control_Hle : System.Web.UI.UserControl
{
    private int applicantCnt = 0;
    private int occupierCnt = 0;

    private String _CurrentDocType;
    protected string DocType = "HLE";

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
        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');","hi"), true);
    }

    public void ClearData()
    {
        DateOfSignatureRadioButtonListId.Value = string.Empty;
        DateOfSignatureRadioButtonList.Items[0].Selected = false;
        DateOfSignatureRadioButtonList.Items[1].Selected = false;
    }

    public void LoadData(int docId)
    {
        ClearData();
        PopulatePersonalMetaDataList(docId);

        //load metadata fields
        MetaDataDb metadataDb = new MetaDataDb();
        MetaData.MetaDataDataTable metaDatas = metadataDb.GetMetaDataByDocId(docId, false, false);

        foreach (MetaData.MetaDataRow metaDataRow in metaDatas.Rows)
        {
            if (metaDataRow.FieldName.ToLower().Trim().Equals(DocTypeMetaDataHleEnum.DateOfSignature.ToString().ToLower()) && metaDataRow.FieldValue.Trim().Length>0)
            {
                DateOfSignatureRadioButtonList.SelectedValue = metaDataRow.FieldValue;
                DateOfSignatureRadioButtonListId.Value = metaDataRow.Id.ToString();
            }
        }
    }

    public void SaveData(int docId)
    {
        foreach (RepeaterItem rItem in PersonalMetaDataRepeater.Items)
        {
            //update nric, name, company name and date joined into appPersonal table
            HiddenField appPersonalIdHiddenField = (HiddenField)rItem.FindControl("AppPersonalIdHiddenField");
            Controls_Nric CustomNric = (Controls_Nric)rItem.FindControl("CustomNric");
            TextBox name = (TextBox)rItem.FindControl("Name");
            TextBox companyName = (TextBox)rItem.FindControl("Company");
            TextBox dateJoined = (TextBox)rItem.FindControl("DateJoined");

            AppPersonalDb appPersonalDb = new AppPersonalDb();
            appPersonalDb.Update(int.Parse(appPersonalIdHiddenField.Value.ToString().Trim()), CustomNric.NricValue.Trim(), name.Text.Trim(), "", dateJoined.Text.Trim(), companyName.Text.Trim());
            
            ////update salary information into AppPersonalSalary table
            //HiddenField idHiddenField = (HiddenField)rItem.FindControl("IdHiddenField");

            //TextBox Month1Name = (TextBox)rItem.FindControl("Month1Name");
            //TextBox Month2Name = (TextBox)rItem.FindControl("Month2Name");
            //TextBox Month3Name = (TextBox)rItem.FindControl("Month3Name");
            //TextBox Month4Name = (TextBox)rItem.FindControl("Month4Name");
            //TextBox Month5Name = (TextBox)rItem.FindControl("Month5Name");
            //TextBox Month6Name = (TextBox)rItem.FindControl("Month6Name");
            //TextBox Month7Name = (TextBox)rItem.FindControl("Month7Name");
            //TextBox Month8Name = (TextBox)rItem.FindControl("Month8Name");
            //TextBox Month9Name = (TextBox)rItem.FindControl("Month9Name");
            //TextBox Month10Name = (TextBox)rItem.FindControl("Month10Name");
            //TextBox Month11Name = (TextBox)rItem.FindControl("Month11Name");
            //TextBox Month12Name = (TextBox)rItem.FindControl("Month12Name");

            //TextBox Month1Value = (TextBox)rItem.FindControl("Month1Value");
            //TextBox Month2Value = (TextBox)rItem.FindControl("Month2Value");
            //TextBox Month3Value = (TextBox)rItem.FindControl("Month3Value");
            //TextBox Month4Value = (TextBox)rItem.FindControl("Month4Value");
            //TextBox Month5Value = (TextBox)rItem.FindControl("Month5Value");
            //TextBox Month6Value = (TextBox)rItem.FindControl("Month6Value");
            //TextBox Month7Value = (TextBox)rItem.FindControl("Month7Value");
            //TextBox Month8Value = (TextBox)rItem.FindControl("Month8Value");
            //TextBox Month9Value = (TextBox)rItem.FindControl("Month9Value");
            //TextBox Month10Value = (TextBox)rItem.FindControl("Month10Value");
            //TextBox Month11Value = (TextBox)rItem.FindControl("Month11Value");
            //TextBox Month12Value = (TextBox)rItem.FindControl("Month12Value");

            //AppPersonalSalaryDb appPersonalSalaryDb = new AppPersonalSalaryDb();
            //appPersonalSalaryDb.Update(int.Parse(idHiddenField.Value.ToString().Trim()), 
            //        Month1Name.Text.Trim(), string.IsNullOrEmpty(Month1Value.Text.ToString()) ? "0.0" : Month1Value.Text,
            //        Month2Name.Text.Trim(), string.IsNullOrEmpty(Month2Value.Text.ToString()) ? "0.0" : Month2Value.Text,
            //        Month3Name.Text.Trim(), string.IsNullOrEmpty(Month3Value.Text.ToString()) ? "0.0" : Month3Value.Text,
            //        Month4Name.Text.Trim(), string.IsNullOrEmpty(Month4Value.Text.ToString()) ? "0.0" : Month4Value.Text,
            //        Month5Name.Text.Trim(), string.IsNullOrEmpty(Month5Value.Text.ToString()) ? "0.0" : Month5Value.Text,
            //        Month6Name.Text.Trim(), string.IsNullOrEmpty(Month6Value.Text.ToString()) ? "0.0" : Month6Value.Text,
            //        Month7Name.Text.Trim(), string.IsNullOrEmpty(Month7Value.Text.ToString()) ? "0.0" : Month7Value.Text,
            //        Month8Name.Text.Trim(), string.IsNullOrEmpty(Month8Value.Text.ToString()) ? "0.0" : Month8Value.Text,
            //        Month9Name.Text.Trim(), string.IsNullOrEmpty(Month9Value.Text.ToString()) ? "0.0" : Month9Value.Text,
            //        Month10Name.Text.Trim(), string.IsNullOrEmpty(Month10Value.Text.ToString()) ? "0.0" : Month10Value.Text,
            //        Month11Name.Text.Trim(), string.IsNullOrEmpty(Month11Value.Text.ToString()) ? "0.0" : Month11Value.Text,
            //        Month12Name.Text.Trim(), string.IsNullOrEmpty(Month12Value.Text.ToString()) ? "0.0" : Month12Value.Text);
        }

        //save metadata fields
        MetaDataDb metadataDb = new MetaDataDb();

        if (DateOfSignatureRadioButtonList.SelectedValue.Length >0)
        {
            if (DateOfSignatureRadioButtonListId.Value.Length == 0)
                DateOfSignatureRadioButtonListId.Value = metadataDb.Insert(DocTypeMetaDataHleEnum.DateOfSignature.ToString(), DateOfSignatureRadioButtonList.SelectedValue, docId, false).ToString();
            else
                metadataDb.Update(int.Parse(DateOfSignatureRadioButtonListId.Value), DateOfSignatureRadioButtonList.SelectedValue);
        }

        if (!CurrentDocType.ToLower().Equals(DocType.ToLower()))
        {

        }
    }

    protected void PersonalMetaDataRepeater_OnItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        ClearData();

        if (e.Item is RepeaterItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            string personalType = data["PersonalType"].ToString();

            Label PersonalTypeLabel = item.FindControl("PersonalTypeLabel") as Label;

            switch (personalType.ToLower())
            {
                case "ha":
                    PersonalTypeLabel.Text = "Applicant " + (++applicantCnt).ToString();
                    break;
                case "oc":
                    PersonalTypeLabel.Text = "Occupier " + (++occupierCnt).ToString();
                    break;
                default:
                    PersonalTypeLabel.Text = string.Empty;
                    break;
            }

            Controls_Nric CustomNric = (Controls_Nric)e.Item.FindControl("CustomNric");
            TextBox Name = (TextBox)e.Item.FindControl("Name");
            //TextBox Company = (TextBox)e.Item.FindControl("Company");
            //TextBox DateJoined = (TextBox)e.Item.FindControl("DateJoined");

            //disable editing of nric and name for documents of personaltype ha and oc
            if (personalType.ToLower().Equals("ha") || personalType.ToLower().Equals("oc"))
                CustomNric.NricTextBox.Enabled = Name.Enabled = false;
            else
                CustomNric.NricTextBox.Enabled = Name.Enabled = true;

            //Label DateJoinedLabel = item.FindControl("DateJoinedLabel") as Label;
            //Label GrossIncomeLabel = item.FindControl("GrossIncomeLabel") as Label;
            //bool hasDateJoinedText = !String.IsNullOrEmpty(data["DateJoinedService"].ToString().Trim());
        }
    }

    private void PopulatePersonalMetaDataList(int docId)
    {
        applicantCnt = 0;
        occupierCnt = 0;
        AppPersonalSalaryDb appPersonalSalaryDb = new AppPersonalSalaryDb();
        PersonalMetaDataRepeater.DataSource = appPersonalSalaryDb.GetAppPersonalSalaryWithAppPersonalDetailsByDocId(docId);
        PersonalMetaDataRepeater.DataBind();
    }

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (Validation.IsNric(args.Value.ToString().Trim()) || Validation.IsFin(args.Value.ToString().Trim()));
    }

    #endregion
    protected void PersonalMetaDataRepeater_Init(object sender, EventArgs e)
    {
        applicantCnt = 0;
        occupierCnt = 0;
    }
}
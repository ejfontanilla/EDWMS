using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;

public partial class Verification_Control_Resale : System.Web.UI.UserControl
{
    private int applicantCnt;
    private int occupierCnt;

    private String _CurrentDocType;
    protected string DocType = "Resale";

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
    }

    public void LoadData(int docId)
    {
        PopulatePersonalMetaDataList(docId);
    }

    public void SaveData(int docId)
    {

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
            TextBox Company = (TextBox)e.Item.FindControl("Company");
            TextBox DateJoined = (TextBox)e.Item.FindControl("DateJoined");

            //disable editing of nric and name for documents of personaltype ha and oc
            if (personalType.ToLower().Equals("ha") || personalType.ToLower().Equals("oc"))
                CustomNric.NricTextBox.Enabled = Name.Enabled = false;
            else
                CustomNric.NricTextBox.Enabled = Name.Enabled = true;

            Label DateJoinedLabel = item.FindControl("DateJoinedLabel") as Label;
            bool hasDateJoinedText = !String.IsNullOrEmpty(data["DateJoinedService"].ToString().Trim());
        }
    }

    private void PopulatePersonalMetaDataList(int docId)
    {
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        PersonalMetaDataRepeater.DataSource = appPersonalDb.GetAppPersonalByDocId(docId);
        PersonalMetaDataRepeater.DataBind();
    }

    protected void NricCustomValidator(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (Validation.IsNric(args.Value.ToString().Trim()) || Validation.IsFin(args.Value.ToString().Trim()));
    }

    #endregion
}
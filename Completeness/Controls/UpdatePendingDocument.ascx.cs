using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Completeness_Controls_UpdatePendingDocument : System.Web.UI.UserControl
{
    int? id;
    string radGridClientId;
    string cookieName;
    bool firstLoad;

    public int PendingDocumentId
    {
        get { return (id.HasValue ? id.Value : -1); }
        set { id = value; }
    }

    public string RadGridClientId
    {
        get { return radGridClientId; }
        set { radGridClientId = value; }
    }

    public bool FirstLoad
    {
        get { return firstLoad; }
        set { firstLoad = value; }
    }

    protected override void OnPreRender(EventArgs e)
    //protected void Page_Load(object sender, EventArgs e)
    {
        base.OnPreRender(e);        

        if (PendingDocumentId > 0)
        {
            cookieName = "UpdatePendingDoc:" + PendingDocumentId.ToString();

            //LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            //LeasInterface.LeasInterfaceDataTable dt = leasInterfaceDb.GetLeasInterfaceById(PendingDocumentId);

            DocDb docDb = new DocDb();
            Doc.DocDataTable doc = docDb.GetDocById(PendingDocumentId);

            if (doc.Rows.Count > 0)
            {
                //LeasInterface.LeasInterfaceRow dr = dt[0];
                Doc.DocRow docRow = doc[0];

                if (firstLoad)
                {
                    PopulateDocumentCondition();

                    ExceptionTextBox.Text = string.Empty;

                    AcceptanceRadioButtonList.SelectedValue = (!docRow.IsIsAcceptedNull() && docRow.IsAccepted ? "1" : "0");
                    ExceptionTextBox.Text = (docRow.IsExceptionNull() ? string.Empty : docRow.Exception);
                    DocConditionRadioButtonList.SelectedValue = (docRow.IsDocumentConditionNull() ?
                        DocumentConditionEnum.NA.ToString() : docRow.DocumentCondition.Trim());

                    firstLoad = false;
                }
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        foreach (ListItem lItem in AcceptanceRadioButtonList.Items)
        {
            if (lItem.Value.Equals("1"))
                lItem.Attributes.Add("onclick", "document.getElementById('" + ExceptionTextBox.ClientID + "').value = '';document.getElementById('" + ExceptionTextBox.ClientID + "').disabled = true");
            else
                lItem.Attributes.Add("onclick", "document.getElementById('" + ExceptionTextBox.ClientID + "').disabled = false;");
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            if (PendingDocumentId > 0)
            {
                //LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
                //leasInterfaceDb.Update(PendingDocumentId, ExceptionTextBox.Text.Trim(), bool.Parse(AcceptanceRadioButtonList.SelectedValue));

                DocDb docDb = new DocDb();
                #region Modified by Edward 2014/06/12
                //docDb.UpdateDocDetails(PendingDocumentId, ExceptionTextBox.Text.Trim(), 
                //    (AcceptanceRadioButtonList.SelectedValue == "1"),
                //    ((AcceptanceRadioButtonList.SelectedValue == "1") ? DocStatusEnum.Completed : DocStatusEnum.Verified));
                docDb.UpdateDocDetails(PendingDocumentId, ExceptionTextBox.Text.Trim(),
                    (AcceptanceRadioButtonList.SelectedValue == "1" ? "Y" : "N"),
                    ((AcceptanceRadioButtonList.SelectedValue == "1") ? DocStatusEnum.Completed : DocStatusEnum.Verified));
                #endregion
                //update document condition
                DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), DocConditionRadioButtonList.SelectedValue, true);
                docDb.UpdateDocumentCondition(PendingDocumentId, documentConditionEnum);

                //capture log action
                ProfileDb profileDb = new ProfileDb();
                DocTypeDb docTypeDb = new DocTypeDb();
                LogActionDb logActionDb = new LogActionDb();
                LogActionEnum logAction = (AcceptanceRadioButtonList.SelectedValue == "1") ? LogActionEnum.REPLACE1_accepted_REPLACE2_at_REPLACE3 : LogActionEnum.REPLACE1_rejected_REPLACE2_at_REPLACE3;
                logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, logAction, 
                    profileDb.GetUserFullName((Guid)Membership.GetUser().ProviderUserKey), 
                    docTypeDb.GetDocType(docDb.GetDocById(PendingDocumentId).Rows[0]["DocTypeCode"].ToString()).Rows[0]["Description"].ToString(),
                    DateTime.Now.ToString(), string.Empty, LogTypeEnum.C, PendingDocumentId);

                // Close the active tooltip
                ScriptManager.RegisterClientScriptBlock(
                    this.Page,
                    this.GetType(),
                    "SaveDocInfoSript",
                    "CloseActiveToolTip();",
                    true);
            }
        }
    }

    protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        if (AcceptanceRadioButtonList.SelectedValue == "0")
        {
            if (!String.IsNullOrEmpty(ExceptionTextBox.Text.Trim()) && ExceptionTextBox.Text.Trim().Length <= 255)
            {
                result = true;
            }
        }
        else
            result = true;

        args.IsValid = result;
    }

    private void PopulateDocumentCondition()
    {
        DocConditionRadioButtonList.Items.Clear();
        foreach (DocumentConditionEnum val in Enum.GetValues(typeof(DocumentConditionEnum)))
        {
            DocConditionRadioButtonList.Items.Add(new ListItem (val.ToString(), val.ToString()));
        }
        DocConditionRadioButtonList.DataBind();
    }
}
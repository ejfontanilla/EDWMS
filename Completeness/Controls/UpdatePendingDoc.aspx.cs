using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Verification_AssignApp : System.Web.UI.Page
{
    int? id;
    string src = string.Empty;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the list of App ids
        if (Request["id"] != null)
        {
            id = int.Parse(Request["id"]);
        }

        if (Request["src"] != null)
        {
            src = Request["src"];
        }

        if (!IsPostBack)
        {
            if (id.HasValue)
            {
                if (String.IsNullOrEmpty(src))
                {
                    DocConditionTr.Visible = true;
                    PopulateSummaryDocs();
                }
                else
                {
                    DocConditionTr.Visible = false;
                    PopulateOutstandingDocs();
                }                
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    "javascript:CloseWindow();", true);
            }

        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            if (String.IsNullOrEmpty(src))
                SaveSummaryDocChanges();
            else
                SaveOutstandingDocChanges();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeAndClose",
                String.Format("javascript:ResizeAndClose({0},{1},'{2}');", 600, 180, (String.IsNullOrEmpty(src) ? true : false)), true);
        }
    }
    #endregion

    #region Validation
    protected void CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        if (AcceptanceRadioButtonList.SelectedValue == "N" && DocConditionRadioButtonList.SelectedValue.Trim().ToString().Equals(DocumentConditionEnum.Amended.ToString()))
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
    #endregion  

    #region Private Methods
    private void PopulateDocumentCondition()
    {
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        DocConditionRadioButtonList.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.Document_Condition.ToString().Replace("_", " "));
        DocConditionRadioButtonList.DataTextField = "Name";
        DocConditionRadioButtonList.DataValueField = "Name";
        DocConditionRadioButtonList.DataBind();
    }

    private void PopulateSummaryDocs()
    {
        DocDb docDb = new DocDb();
        Doc.DocDataTable dt = docDb.GetDocById(id.Value);

        if (dt.Rows.Count > 0)
        {

            Doc.DocRow dr = dt[0];

            PopulateDocumentCondition();

            ExceptionTextBox.Text = string.Empty;

            //AcceptanceRadioButtonList.SelectedValue = (!dr.IsIsAcceptedNull() && dr.IsAccepted ? "1" : "0");
            if (dr.IsImageAcceptedNull() || (dr.ImageAccepted.ToUpper() != "Y" && dr.ImageAccepted.ToUpper() != "N" && dr.ImageAccepted.ToUpper() != "X"))
                AcceptanceRadioButtonList.ClearSelection();
            else
                AcceptanceRadioButtonList.SelectedValue = dr.ImageAccepted.ToUpper();
            ExceptionTextBox.Text = (dr.IsExceptionNull() ? string.Empty : dr.Exception);
            DocConditionRadioButtonList.SelectedValue = (dr.IsDocumentConditionNull() ?
                DocumentConditionEnum.NA.ToString() : dr.DocumentCondition.Trim());
        }
        else
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
            //"javascript:CloseWindow();", true);
        }
    }

    private void PopulateOutstandingDocs()
    {
        if (src.Equals(ReferenceTypeEnum.HLE.ToString()))
        {
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            LeasInterface.LeasInterfaceDataTable dt = leasInterfaceDb.GetLeasInterfaceById(id.Value);

            if (dt.Rows.Count > 0)
            {

                LeasInterface.LeasInterfaceRow dr = dt[0];

                ExceptionTextBox.Text = string.Empty;

                AcceptanceRadioButtonList.SelectedValue = (dr.IsReceived ? "1" : "0");
                ExceptionTextBox.Text = (dr.IsExceptionNull() ? string.Empty : dr.Exception);
            }
            else
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                //"javascript:CloseWindow();", true);
            }
        }
        else
        {
            SocInterfaceDb socInterfaceDb = new SocInterfaceDb();
            SocInterface.SocInterfaceDataTable dt = socInterfaceDb.GetSocInterfaceById(id.Value);

            if (dt.Rows.Count > 0)
            {

                SocInterface.SocInterfaceRow dr = dt[0];

                ExceptionTextBox.Text = string.Empty;

                AcceptanceRadioButtonList.SelectedValue = (dr.IsReceived ? "1" : "0");
                ExceptionTextBox.Text = (dr.IsExceptionNull() ? string.Empty : dr.Exception);
            }
            else
            {
                //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                //"javascript:CloseWindow();", true);
            }
        }
    }

    private void SaveSummaryDocChanges()
    {
        DocDb docDb = new DocDb();
        docDb.UpdateDocDetails(id.Value, (ExceptionTextBox.Text.Trim()), (AcceptanceRadioButtonList.SelectedValue), DocStatusEnum.Completed);
        docDb.UpdateSendToCdbAccept(id.Value, SendToCDBStatusEnum.Ready);
        //update document condition
        DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), DocConditionRadioButtonList.SelectedValue, true);
        docDb.UpdateDocumentCondition(id.Value, documentConditionEnum);

        //capture log action
        ProfileDb profileDb = new ProfileDb();
        DocTypeDb docTypeDb = new DocTypeDb();
        LogActionDb logActionDb = new LogActionDb();
        LogActionEnum logAction = (AcceptanceRadioButtonList.SelectedValue == "Y") ? LogActionEnum.REPLACE1_accepted_REPLACE2_at_REPLACE3 : LogActionEnum.REPLACE1_rejected_REPLACE2_at_REPLACE3;
        logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, logAction,
            profileDb.GetUserFullName((Guid)Membership.GetUser().ProviderUserKey),
            docTypeDb.GetDocType(docDb.GetDocById(id.Value).Rows[0]["DocTypeCode"].ToString()).Rows[0]["Description"].ToString(),
            DateTime.Now.ToString(), string.Empty, LogTypeEnum.C, id.Value);

        FormPanel.Visible = false;
        ConfirmPanel.Visible = true;
    }

    private void SaveOutstandingDocChanges()
    {
        if (src.Equals(ReferenceTypeEnum.HLE.ToString()))
        {
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            leasInterfaceDb.Update(id.Value,
               (ExceptionTextBox.Text.Trim()),
               (AcceptanceRadioButtonList.SelectedValue == "Y"));

        }
        else
        {
            SocInterfaceDb socInterfaceDb = new SocInterfaceDb();
            socInterfaceDb.Update(id.Value,
               (ExceptionTextBox.Text.Trim()),
               (AcceptanceRadioButtonList.SelectedValue == "Y"));
        }
    }
    #endregion
}
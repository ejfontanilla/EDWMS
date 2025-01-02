using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;

public partial class Verification_Email : System.Web.UI.Page
{
    public int? setId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            setId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/Default.aspx");
       
        if (!IsPostBack)
        {
            DocSetDb docSetDb = new DocSetDb();
            if (!docSetDb.AllowVerificationSaveDate(setId.Value))
                Util.ShowUnauthorizedMessage();

            if (docSetDb.IsSetConfirmed(setId.Value))
                Util.ShowUnauthorizedMessage();

            if (!docSetDb.AllowVerificationSaveDate(setId.Value))
                Util.ShowUnauthorizedMessage();

            TreeviewDWMS.PopulateTreeView(RadTreeView1, setId.Value, true, true);

            docSetDb.GetDocSetById(setId.Value);
            DocSet.DocSetDataTable docset = docSetDb.GetDocSetById(setId.Value);
            DocSet.DocSetRow docSetRow = docset[0];
            SubjectTextBox.Text = "Documents from SET: " + docSetRow.SetNo;
            RadTreeView1.ExpandAllNodes();
        }
    }

    protected void SendButton_onClick(object sender, EventArgs e)
    {
        DocSetDb docSetDb = new DocSetDb();
        if (!docSetDb.AllowVerificationSaveDate(setId.Value))
            Response.Redirect("view.aspx?id=" + setId.Value);

        if (docSetDb.IsSetConfirmed(setId.Value))
            Response.Redirect("view.aspx?id=" + setId.Value);

        IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

        List<int> uniqueNodes = new List<int>();
        foreach (RadTreeNode rNode in checkedNodes)
        {
            if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
            {
                if (!uniqueNodes.Contains(int.Parse(rNode.Value))) // since one document can be in different folder, filter the repetition
                {
                    uniqueNodes.Add(int.Parse(rNode.Value));
                }
            }
        }

        if (uniqueNodes.Count > 0)
        {
            string FileName = string.Empty;
            string userName = string.Empty;
            string userEmail = string.Empty;
            foreach (int docId in uniqueNodes)
            {
                DocDb docDb = new DocDb();
                FileName += docDb.GetDownloadableDocumentFileName(uniqueNodes, setId.Value) + ";";
            }

            if (!string.IsNullOrEmpty(FileName))
            {
                //send email
                ParameterDb parameterDb = new ParameterDb();
                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    EmailRecipentsTextBox.EmailValue, string.Empty, string.Empty, string.Empty, SubjectTextBox.Text, RemarksTextBox.Text, FileName);
                SetVisibility(EmailRecipentsTextBox.EmailValue);
            }
            else
            {
                RecipentCustomValidator.Text = "There are no documents to email.";
                RecipentCustomValidator.IsValid = false;
            }
            urlTargetHiddenField.Value = "View.aspx?id=" + setId.ToString();
        }
        else
        {
            RecipentCustomValidator.Text = "Please select a document to email.";
            RecipentCustomValidator.IsValid = false;
        }
    }

    protected void SetVisibility(string reciepents)
    {
        ConfirmLabel.Text = "The documents have been emailed to " + reciepents;
        ConfirmPanel.Visible = OKPanel.Visible = true;
        DisplayPanel.Visible = false;
    }

    protected void OkButton_onClick(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", String.Format("ResizeAndClose(700, 190, '{0}');", urlTargetHiddenField.Value.ToString()), true);
    }
}

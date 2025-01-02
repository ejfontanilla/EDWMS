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
using System.Collections.Generic;
using System.IO;
using System.Threading;

public partial class Maintenance_DocumentType_EditSampleDoc : System.Web.UI.Page
{
    string docTypeCode;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");
        WarningPanel.Visible = (Request["cfm"] == "0");

        docTypeCode = string.Empty;

        if (!String.IsNullOrEmpty(Request["code"]))
        {
            docTypeCode = Request["code"];
        }
        else
            Response.Redirect("Default.aspx");

        if (!IsPostBack)
        {
            if (!String.IsNullOrEmpty(docTypeCode))
            {
                DocTypeDb docTypeDb = new DocTypeDb();
                DocType.DocTypeDataTable docTypeDt = docTypeDb.GetDocType(docTypeCode);

                if (docTypeDt.Rows.Count > 0)
                {
                    DocType.DocTypeRow docType = docTypeDt[0];
                    TitleLabel.Text = docType.Description + " Sample Documents";

                    DocumentIdLabel.Text = docType.DocumentId.ToString();
                    DocumentTypeLabel.Text = docType.Description;

                    //Added By Edward 26/3/2014
                    if (docType.IsAcquireNewSamplesNull())
                        rblAcquire.Items[0].Selected = true;
                    else if (docType.AcquireNewSamples)
                        rblAcquire.Items[0].Selected = true;
                    else
                        rblAcquire.Items[1].Selected = true;


                    //Added by Edward Displaying Active DocTypes 2015/8/17
                    if (docType.IsIsActiveNull())
                        rblIsActive.Items[0].Selected = true;
                    else if (docType.IsActive)
                        rblIsActive.Items[0].Selected = true;
                    else
                        rblIsActive.Items[1].Selected = true;

                }

                DisplaySampleDocuments();
            }
        }

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            bool result = SaveSampleDocs();

            if (result)
            {
                Response.Redirect(String.Format("View.aspx?code={0}&cfm={1}", docTypeCode, "2")); 
            }
        }
    }

    /// <summary>
    /// Repeater item command event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        // Check type of item
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName.Equals("Delete"))
            {
                // Delete the sample doc from the database
                SampleDocDb sampleDocDb = new SampleDocDb();
                sampleDocDb.Delete(id);

                // Delete the sample doc from the physical folder
                try
                {
                    string sampleDocPath = Util.GetUsersSampleDocTempFolder();

                    DirectoryInfo sampleDocMainDir = new DirectoryInfo(sampleDocPath);

                    DirectoryInfo[] sampleDocDirs = sampleDocMainDir.GetDirectories(id.ToString(), SearchOption.AllDirectories);

                    if (sampleDocDirs.Length > 0)
                    {
                        sampleDocDirs[0].Delete(true);
                    }
                }
                catch(Exception)
                {
                }

                DisplaySampleDocuments();
            }
        }
    }

    /// <summary>
    /// Repeater item data bound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DocumentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //HyperLink AttachmentHyperLink = e.Item.FindControl("AttachmentHyperLink") as HyperLink;
            LinkButton DeleteLink = e.Item.FindControl("DeleteLink") as LinkButton;
            
            DataRowView drv = e.Item.DataItem as DataRowView;

            bool isOcr = bool.Parse(drv.Row["IsOcr"].ToString());

            if (!isOcr)
            {
                //AttachmentHyperLink.NavigateUrl = "#";
                DeleteLink.OnClientClick = "alert(\"You cannot delete this sample document. It is pending for OCR.\"); return false;";
            }
        }
    }
    #endregion

    #region Validation
    #endregion  

    #region Private Methods
    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }

    /// <summary>
    /// Display the sample documents for the given document type
    /// </summary>
    /// <param name="version"></param>
    private void DisplaySampleDocuments()
    {
        SampleDocDb sampleDocDb = new SampleDocDb();

        if (!String.IsNullOrEmpty(docTypeCode))
        {
            DocumentRepeater.DataSource = sampleDocDb.GetDataByDocType(docTypeCode);
            DocumentRepeater.DataBind();
        }
    }

    /// <summary>
    /// SAve the sample documents
    /// </summary>
    /// <returns></returns>
    private bool SaveSampleDocs()
    {
        bool result = false;        

        SampleDocDb sampleDocDb = new SampleDocDb();

        string userTempPath = Util.GetUsersSampleDocTempFolder();

        // Save the sample documents for the document type
        foreach (UploadedFile file in DocumentRadAsyncUpload.UploadedFiles)
        {
            int sampleDocId = -1;
            byte[] bytes = new byte[0];
            sampleDocId = sampleDocDb.Insert(docTypeCode, file.FileName, bytes, false);

            if (sampleDocId > 0)
            {
                // Add escape characters to the path
                userTempPath = userTempPath.Replace(@"\", @"\\");

                // Set the folder the image is to be saved
                string uploadedDocsDir = Path.Combine(userTempPath, docTypeCode);
                uploadedDocsDir = Path.Combine(uploadedDocsDir, sampleDocId.ToString());

                // Create the directory if it does not exists
                if (!Directory.Exists(uploadedDocsDir))
                    Directory.CreateDirectory(uploadedDocsDir);

                // Create the file path
                string filePath = Path.Combine(uploadedDocsDir, file.FileName);

                // Save the file
                file.SaveAs(filePath, true);

                result = true;
            }
        }

        //Added By Edward 26/3/2014 Freeze Sample Documents
        DocTypeDb docTypeDb = new DocTypeDb();
        //Added by Edward Displaying Active DocTypes 2015/8/17
        //result = docTypeDb.UpdateAcquireNewSamples(docTypeCode, rblAcquire.Items[0].Selected);
        result = docTypeDb.UpdateAcquireNewSamples(docTypeCode, 
            rblAcquire.Items[0].Selected,rblIsActive.Items[0].Selected);
        return result;
    }
    #endregion
}
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
using System.IO;

public partial class Verification_IndicateUrgency : System.Web.UI.Page
{
    int[] ids;
    string src = string.Empty;
    int count = 0;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the list of set ids
        if (Request["id"] != null)
        {
            string[] idStrArray = Request["id"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ids = new int[idStrArray.Length];
            count = 0;
            foreach (string idStr in idStrArray)
            {
                ids[count] = int.Parse(idStr);
                count++;
            }
        }

        if (!IsPostBack)
        {
            PopulateExpediteReason();
            PopulateSets();

            if (count==1)
                PopulateList();
        }
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            SaveUrgencyDetails();

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeAndClose",
                String.Format("javascript:ResizeAndClose({0},{1},'{2}');", 600, 180, (String.IsNullOrEmpty(src) ? true : false)), true);
        }
    }

    protected void SkipCategorizationRadioButtonList_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        //if (SkipCategorizationRadioButtonList.SelectedValue.Equals("1"))
        //    IsUrgentPanel.Visible = false;
        //else
            IsUrgentPanel.Visible = true;
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Populate the sets
    /// </summary>
    private void PopulateSets()
    {
        if (ids != null && ids.Length > 0)
        {
            //NoOfSetsLabel.Text = string.Format("You have chosen {0} set(s) to assign:", ids.Length);

            DocSetDb docSetDb = new DocSetDb();
            SetRepeater.DataSource = docSetDb.GetMultipleDocSets(ids);
            SetRepeater.DataBind();

            NoOfSetsLabel.Text = string.Format("You have chosen {0} set(s) to update urgency:", SetRepeater.Items.Count);
        }
    }

    private void PopulateExpediteReason()
    {
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        ExpediteReasonDropDownList.DataSource = masterListItemDb.GetMasterListItemByMasterListName(MasterListEnum.ExpediteReason.ToString().Replace("_", " "));
        ExpediteReasonDropDownList.DataTextField = "Name";
        ExpediteReasonDropDownList.DataValueField = "Name";
        ExpediteReasonDropDownList.DataBind();
    }

    private void PopulateList()
    {
        DocSetDb docSetDb = new DocSetDb();
        DocSet.DocSetDataTable docSet = docSetDb.GetDocSetById(ids[0]);

        if (docSet.Rows.Count > 0)
        {
            DocSet.DocSetRow docSetRow = docSet[0];

            ExpediteRemarkTextBox.Text = docSetRow.IsExpediteRemarkNull() ? string.Empty : docSetRow.ExpediteRemark;
            UrgencyRadioButtonList.SelectedValue = docSetRow.IsUrgent ? "1" : "0";
            ExpediteReasonDropDownList.SelectedValue = (docSetRow.IsExpediteReasonNull() ?
                string.Empty : docSetRow.ExpediteReason);

            //SkipCategorizationRadioButtonList.SelectedValue = docSetRow.SkipCategorization ? "1" : "0";

            //if (docSetRow.SkipCategorization)
            //    IsUrgentPanel.Visible = false;
            //else
                IsUrgentPanel.Visible = true;
        }
        else
        {
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
            //"javascript:CloseWindow();", true);
        }
    }

    private void SaveUrgencyDetails()
    {
        Page.Validate();

        if (Page.IsValid)
        {
            DocSetDb docSetDb = new DocSetDb();

            foreach (int id in ids)
            {
                docSetDb.UpdateUrgencyDetails(id, ExpediteReasonDropDownList.SelectedValue, ExpediteRemarkTextBox.Text,
                    UrgencyRadioButtonList.SelectedValue.Equals("1") ? true : false);
                //,SkipCategorizationRadioButtonList.SelectedValue.Equals("1") ? true : false
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(550, 200);", true);
        }
    }

    private void SetDocumentsAsUnidentified(int docSetId)
    {
        RawFileDb rawFileDb = new RawFileDb();
        DocDb docDb = new DocDb();
        RawPageDb rawPageDb = new RawPageDb();
        LogActionDb logActionDb = new LogActionDb();
        DocSetDb docSetDb = new DocSetDb();
        ExceptionLogDb exceptionLogDb = new ExceptionLogDb();

        Guid importedBy = new Guid(Membership.GetUser().ProviderUserKey.ToString());

        // For OCR main Dir
        string mainDirPath = Util.GetDocForOcrFolder();
        string rawPageDirPath = Util.GetRawPageOcrFolder();
        string setDirPath = Path.Combine(mainDirPath, docSetId.ToString());
        
        RawFile.RawFileDataTable rawFileTable = rawFileDb.GetDataByDocSetId(docSetId);

        foreach(RawFile.RawFileRow rawFileRow in rawFileTable)
        {
            // Get the physical raw file
            string rawFilePath = Path.Combine(setDirPath, rawFileRow.Id.ToString());
            rawFilePath = Path.Combine(rawFilePath, rawFileRow.FileName);
            FileInfo rawFile = new FileInfo(rawFilePath);

            if (rawFile.Exists)
            {
                try
                {
                    // Process the raw file
                    if (rawFile.Extension.ToUpper().Equals(".PDF"))
                    {
                        PreparePdfForProcessing(docSetId, rawFile, new DirectoryInfo(rawPageDirPath), new DirectoryInfo(rawFile.DirectoryName), importedBy);
                    }
                    else if ((rawFile.Extension.ToUpper().Equals(".TIF") || rawFile.Extension.ToUpper().Equals(".TIFF")) &&
                        Util.CountTiffPages(rawFile.FullName) > 1)
                    {
                        PrepareMultiPageTiffForProcessing(docSetId, rawFile, new DirectoryInfo(rawPageDirPath), new DirectoryInfo(rawFile.DirectoryName), importedBy);
                    }
                    else
                    {
                        PrepareImagesForProcessing(docSetId, rawFile, new DirectoryInfo(rawPageDirPath), new DirectoryInfo(rawFile.DirectoryName), importedBy);
                    }
                }
                catch (Exception ex)
                {
                    string errorSummary = string.Format("File={0}, Message={1}", rawFile.Name,
                        ex.Message + (ex.Message.ToLower().Contains("the document has no pages") ? " File could be a secured document." : string.Empty));

                    // Log the error to show in the set action log
                    logActionDb.Insert(importedBy,
                        LogActionEnum.REPLACE1_COLON_REPLACE2,
                        LogActionEnum.File_Error.ToString(),
                        ex.Message,
                        string.Empty, string.Empty, LogTypeEnum.S, docSetId);

                    // Log the exception for the directory
                    string channel = string.Empty;
                    string refNo = string.Empty;
                    string reason = "Error processing the file(s).";
                    string errorMessage = ex.Message;

                    DocSet.vDocSetDataTable vDocSetTable = docSetDb.GetvDocSetById(docSetId);

                    if (vDocSetTable.Rows.Count > 0)
                    {
                        DocSet.vDocSetRow vDocSet = vDocSetTable[0];

                        channel = vDocSet.Channel;
                        refNo = vDocSet.RefNo;
                    }

                    exceptionLogDb.Insert(channel, refNo, DateTime.Now, reason, errorMessage, docSetId.ToString(), true);
                }
            }            
        }

        //// Create a document record of the raw file
        //// Save the document into the Doc table                
        //int docId = docDb.Insert(setId, DocTypeEnum.Unidentified.ToString(), setId, DocStatusEnum.New.ToString());

        //if (docId > 0)
        //{
        //    // Update the raw pages of the raw file
        //    RawPage.RawPageDataTable rawPageTable = rawPageDb.GetRawPageByRawFileId(rawFileRow.Id);

        //    foreach (RawPage.RawPageRow rawPageRow in rawPageTable)
        //    {
        //        rawPageDb.Update(rawPageRow.Id, docId, rawPageRow.RawPageNo);
        //    }

        //    // Update the status of the set
        //    // If there is a verification officer assigned to the set, the status will be Pending Verification.
        //    // Else, it will be New                    
        //    docSetDb.UpdateSetStatus(setId, (docSetDb.HasVerificationOfficerAssigned(setId) ? SetStatusEnum.Pending_Verification : SetStatusEnum.New));

        //    // Insert the doc personal record for the doc                    
        //    int docPersonalId = docPersonalDb.Insert(setId, string.Empty, string.Empty,
        //        DocFolderEnum.Unidentified.ToString(), string.Empty);

        //    // Insert the association of the doc and doc personal                    
        //    setDocRefDb.Insert(docId, docPersonalId);
        //}
    }

    private void PreparePdfForProcessing(int setId, FileInfo file, DirectoryInfo rawPageMainDirInfo, 
        DirectoryInfo rawFilesDir, Guid importedBy)
    {
        RawPageDb rawPageDb = new RawPageDb();
        LogActionDb logActionDb = new LogActionDb();

        ArrayList pdfArrayList = Util.PdfSplit(file.FullName, rawFilesDir.FullName);

        int pageNo = 1;

        // Add the PDF file paths to the array
        foreach (string pdfPagePath in pdfArrayList)
        {
            FileInfo pdfFile = new FileInfo(pdfPagePath);

            // Save the raw page                                
            int rawPageId = rawPageDb.Insert(int.Parse(rawFilesDir.Name), pageNo, new byte[0],
                string.Empty, new byte[0], new byte[0], false);

            if (rawPageId > 0)
            {
                string rawPageTempPath = Path.Combine(rawPageMainDirInfo.FullName, rawPageId.ToString());

                // If the folder does not exists, create one
                if (!Directory.Exists(rawPageTempPath))
                    Directory.CreateDirectory(rawPageTempPath);

                string newRawPageTempPath = Path.Combine(rawPageTempPath, pdfFile.Name);

                try
                {

                    // Move the file
                    pdfFile.CopyTo(newRawPageTempPath, true);

                    try
                    {
                        //// Create the thumbnail file
                        //ImageManager.Resize(newRawPageTempPath, 113, 160);
                    }
                    catch (Exception)
                    {
                        // Log the error to show in the set action log
                        logActionDb.Insert(importedBy,
                            LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_Unable_to_create_thumbnail_for_the_file_PERIOD_SEMICOLON_File_EQUALSSIGN_REPLACE2,
                            LogActionEnum.Thumbnail_Creation_Error.ToString(),
                            pdfFile.FullName.Contains("\\") ? pdfFile.FullName.Substring(pdfFile.FullName.LastIndexOf("\\") + 1) : pdfFile.FullName,
                            string.Empty, string.Empty, LogTypeEnum.S, setId);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        // Log to show in the set level
                        logActionDb.Insert(importedBy,
                            LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_REPLACE2_SEMICOLON_File_EQUALSSIGN_REPLACE,
                            LogActionEnum.File_Error.ToString(),
                            ex.Message,
                            pdfFile.FullName.Contains("\\") ? pdfFile.FullName.Substring(pdfFile.FullName.LastIndexOf("\\") + 1) : pdfFile.FullName,
                            string.Empty, LogTypeEnum.S, setId);

                        // Use the original PDF file for the View page
                        // Copy the file into a folder that has a name equal to the RawPage Id                                        
                        newRawPageTempPath = Path.Combine(rawPageTempPath, pdfFile.Name + "_s.pdf");

                        // Move the file
                        pdfFile.CopyTo(newRawPageTempPath);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // Delete the original
            try
            {
                pdfFile.Delete();
            }
            catch (Exception)
            {
            }

            pageNo++;
        }
    }

    private void PrepareMultiPageTiffForProcessing(int setId, FileInfo file, DirectoryInfo rawPageMainDirInfo,
        DirectoryInfo rawFilesDir, Guid importedBy)
    {
        RawPageDb rawPageDb = new RawPageDb();
        LogActionDb logActionDb = new LogActionDb();

        ArrayList tiffArrayList = Util.TiffSplit(file.FullName, rawFilesDir.FullName);

        int pageNo = 1;

        // Add the TIFF file paths to the array
        foreach (string tiffPagePath in tiffArrayList)
        {
            FileInfo tiffFile = new FileInfo(tiffPagePath);

            // Save the raw page                                
            int rawPageId = rawPageDb.Insert(int.Parse(rawFilesDir.Name), pageNo, new byte[0],
                string.Empty, new byte[0], new byte[0], false);

            if (rawPageId > 0)
            {
                string rawPageTempPath = Path.Combine(rawPageMainDirInfo.FullName, rawPageId.ToString());

                // If the folder does not exists, create one
                if (!Directory.Exists(rawPageTempPath))
                    Directory.CreateDirectory(rawPageTempPath);

                // Copy the file into a folder that has a name equal to the RawPage Id                                        
                string newRawPageTempPath = Path.Combine(rawPageTempPath, tiffFile.Name);

                // Move the file
                try
                {
                    tiffFile.CopyTo(newRawPageTempPath);

                    try
                    {
                        //// Create the thumbnail file
                        //ImageManager.Resize(newRawPageTempPath, 113, 160);
                    }
                    catch (Exception)
                    {
                        // Log the error to show in the set action log
                        logActionDb.Insert(importedBy,
                            LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_Unable_to_create_thumbnail_for_the_file_PERIOD_SEMICOLON_File_EQUALSSIGN_REPLACE2,
                            LogActionEnum.Thumbnail_Creation_Error.ToString(),
                            tiffFile.FullName.Contains("\\") ? tiffFile.FullName.Substring(tiffFile.FullName.LastIndexOf("\\") + 1) : tiffFile.FullName,
                            string.Empty, string.Empty, LogTypeEnum.S, setId);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        // Log to show in the set level
                        logActionDb.Insert(importedBy,
                            LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_REPLACE2_SEMICOLON_File_EQUALSSIGN_REPLACE,
                            LogActionEnum.File_Error.ToString(),
                            ex.Message,
                            tiffFile.FullName.Contains("\\") ? tiffFile.FullName.Substring(tiffFile.FullName.LastIndexOf("\\") + 1) : tiffFile.FullName,
                            string.Empty, LogTypeEnum.S, setId);

                        // Use the original TIFF file for the View page
                        Util.CreateSearcheablePdfFile(tiffFile.FullName);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // Delete the original
            try
            {
                tiffFile.Delete();
            }
            catch (Exception)
            {
            }

            pageNo++;
        }
    }

    private void PrepareImagesForProcessing(int setId, FileInfo file, DirectoryInfo rawPageMainDirInfo,
        DirectoryInfo rawFilesDir, Guid importedBy)
    {
        RawPageDb rawPageDb = new RawPageDb();
        LogActionDb logActionDb = new LogActionDb();

        // Save the raw page
        int rawPageId = rawPageDb.Insert(int.Parse(rawFilesDir.Name), 1, new byte[0],
            string.Empty, new byte[0], new byte[0], false);

        if (rawPageId > 0)
        {
            // Copy the file into a folder that has a name equal to the RawPage Id
            string rawPageTempPath = Path.Combine(rawPageMainDirInfo.FullName, rawPageId.ToString());
            string newRawPageTempPath = Path.Combine(rawPageTempPath, file.Name);

            // If the folder does not exists, create one
            if (!Directory.Exists(rawPageTempPath))
                Directory.CreateDirectory(rawPageTempPath);

            try
            {
                // Copy the file
                file.CopyTo(newRawPageTempPath);

                try
                {
                    // Create the thumbnail file
                    ImageManager.Resize(newRawPageTempPath, 113, 160);
                }
                catch (Exception)
                {
                    logActionDb.Insert(importedBy, 
                        LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_Unable_to_create_thumbnail_for_the_file_PERIOD_SEMICOLON_File_EQUALSSIGN_REPLACE2,
                        LogActionEnum.Thumbnail_Creation_Error.ToString(), 
                        file.FullName.Contains("\\") ? file.FullName.Substring(file.FullName.LastIndexOf("\\") + 1) : file.FullName, 
                        string.Empty, string.Empty, LogTypeEnum.S, setId);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    // Log to show in the set level
                    logActionDb.Insert(importedBy,
                        LogActionEnum.REPLACE1_COLON_Message_EQUALSSIGN_REPLACE2_SEMICOLON_File_EQUALSSIGN_REPLACE, 
                        LogActionEnum.File_Error.ToString(), 
                        ex.Message,
                        file.FullName.Contains("\\") ? file.FullName.Substring(file.FullName.LastIndexOf("\\") + 1) : file.FullName, 
                        string.Empty, LogTypeEnum.S, setId);

                    // Use the original image file for the View page
                    Util.CreateSearcheablePdfFile(file.FullName);
                }
                catch (Exception)
                {
                }
            }
        }
    }

    #endregion
}
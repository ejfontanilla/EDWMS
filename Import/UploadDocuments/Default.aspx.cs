﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;
using System.IO;
using System.Threading;

public partial class Import_UploadDocuments_Default : System.Web.UI.Page
{
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

        if (!IsPostBack)
        {
        }

        // Set access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Upload button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            // Save
            bool result = Save();
            
            Response.Redirect(String.Format("Default.aspx?cfm={0}", (result ? "1" : "0")));
        }
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        string argument = e.Argument;

        ScanUploadFields.SetComboBoxText(argument);
    }
    #endregion

    #region Validation
    /// <summary>
    /// Document validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void DocumentRadUploadCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (DocumentRadAsyncUpload.UploadedFiles.Count > 0);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Upload functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Import, AccessControlSettingEnum.Upload);

        // Set the visibility of the buttons
        SubmitButton.Visible = hasAccess;
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="setId"></param>
    /// <returns></returns>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //private bool Save()
    //{
    //    bool result = false;

    //    DocSetDb docSetDb = new DocSetDb();

    //    int departmentId = -1;
    //    int sectionid = -1;
    //    string setNo = Util.FormulateSetNumber(docSetDb.GetNextIdNo(), out departmentId, out sectionid);
    //    string referenceNo = ScanUploadFields.ReferenceNumber.Trim();
    //    int docAppId = ScanUploadFields.DocAppId;
    //    string referenceType = ScanUploadFields.ReferenceType;
    //    string block = ScanUploadFields.Block;
    //    int streetId = ScanUploadFields.StreetId;
    //    string level = ScanUploadFields.Level;
    //    string unitNo = ScanUploadFields.Unit;
    //    string channel = ScanUploadFields.Channel;
    //    SetStatusEnum status = SetStatusEnum.Pending_Categorization;
    //    Guid importedBy = (Guid)Membership.GetUser().ProviderUserKey;

    //    DocAppDb docAppDb = new DocAppDb();
    //    docAppDb.GetAppDetails(docAppId, referenceNo, referenceType, out docAppId);
    //    int setId = 0;

    //    // Save the set information
    //    for (int cnt = 0; cnt < 2; cnt++)
    //    {
    //        //Added by edward Try Catch for Reducing Error Notifications - Insert bug in UploadDocument 19/8/2015
    //        try
    //        {
    //            setId = docSetDb.Insert(setNo, DateTime.Now, status, block, streetId, level, unitNo, channel, importedBy,
    //            null, departmentId, sectionid, docAppId);
    //            if (setId > 0)
    //                break;
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorLogDb.NotifyErrorByEmail(ex,string.Empty);
    //            return false;
    //        }            
    //    }

    //    if (setId > 0)
    //    {
    //        result = true;

    //        // Save the uploaded documents
    //        result = SaveUploadedDocument(setId);

    //        if (result)
    //        {
    //            // Save the raw file
    //            result = SaveRawFile(setId);
    //        }

    //        if (!result)
    //        {
    //            // Delete the set record and all related files during saving error
    //            try
    //            {
    //                string userTempPath = Util.GetDocForOcrFolder();

    //                // Add escape characters to the path
    //                userTempPath = userTempPath.Replace(@"\", @"\\");

    //                string uploadedDocsDir = Path.Combine(userTempPath, setId.ToString());

    //                DirectoryInfo mainDir = new DirectoryInfo(uploadedDocsDir);
    //                //mainDir.Delete(true);
    //            }
    //            catch (Exception ex)
    //            {
    //                ErrorLogDb.NotifyErrorByEmail(ex,string.Empty);
    //            }
    //            finally
    //            {
    //                //docSetDb.Delete(setId);
    //            }
    //        }
    //        else
    //        {
    //            // Update the 'ReadyForOcr' flag
    //            docSetDb.SetReadyForOcr(setId, true);
    //        }
    //    }

    //    return result;
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    private bool Save()
    {
        bool result = false;
        DocSetDb docSetDb = new DocSetDb();
        int departmentId = -1;
        int sectionid = -1;
        string setNo = Util.FormulateSetNumber(docSetDb.GetNextIdNo(), out departmentId, out sectionid);
        string referenceNo = ScanUploadFields.ReferenceNumber.Trim();
        int docAppId = ScanUploadFields.DocAppId;
        string referenceType = ScanUploadFields.ReferenceType;
        string block = ScanUploadFields.Block;
        int streetId = ScanUploadFields.StreetId;
        string level = ScanUploadFields.Level;
        string unitNo = ScanUploadFields.Unit;
        string channel = ScanUploadFields.Channel;
        SetStatusEnum status = SetStatusEnum.Pending_Categorization;
        Guid importedBy = (Guid)Membership.GetUser().ProviderUserKey;

        DocAppDb docAppDb = new DocAppDb();
        docAppDb.GetAppDetails(docAppId, referenceNo, referenceType, out docAppId);
        int setId = 0;

        DateTime verificationDateIn = DateTime.Now;
        // Save the set information
        for (int cnt = 0; cnt < 2; cnt++)
        {
            //Added by edward Try Catch for Reducing Error Notifications - Insert bug in UploadDocument 19/8/2015
            try
            {
                setId = docSetDb.Insert(setNo, verificationDateIn, status, block, streetId, level, unitNo, channel, importedBy,
                null, departmentId, sectionid, docAppId);
                if (setId > 0)
                    break;
            }
            catch (Exception ex)
            {
                ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
                return false;
            }
        }

        if (setId > 0)
        {
            result = true;

            // Save the uploaded documents
            result = SaveUploadedDocument(setId,verificationDateIn);

            if (result)
            {
                // Save the raw file
                result = SaveRawFile(setId, verificationDateIn);
            }

            if (!result)
            {
                // Delete the set record and all related files during saving error
                try
                {
                    DirectoryInfo mainDir = new DirectoryInfo(Util.GetDocForOcrFolder(setId,verificationDateIn));
                    //mainDir.Delete(true);
                }
                catch (Exception ex)
                {
                    ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
                }
                finally
                {
                    //docSetDb.Delete(setId);
                }
            }
            else
            {
                // Update the 'ReadyForOcr' flag
                docSetDb.SetReadyForOcr(setId, true);
            }
        }

        return result;
    }
    #endregion

    /// <summary>
    /// Save the uploaded documents
    /// </summary>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //private bool SaveUploadedDocument(int setId)
    //{
    //    string userTempPath = Util.GetDocForOcrFolder();

    //    // Add escape characters to the path
    //    userTempPath = userTempPath.Replace(@"\", @"\\");

    //    string uploadedDocsDir = Path.Combine(userTempPath, setId.ToString());

    //    // If the folder does not exists, create one
    //    if (!Directory.Exists(uploadedDocsDir))
    //        Directory.CreateDirectory(uploadedDocsDir);
    //    else
    //    {
    //        Util.DeleteFolderContents(uploadedDocsDir);
    //    }

    //    // Save the uploaded files
    //    foreach (UploadedFile file in DocumentRadAsyncUpload.UploadedFiles)
    //    {
    //        try
    //        {
    //            string fileName = Path.Combine(uploadedDocsDir, file.FileName);
    //            if (file.FileName.Contains("%") || file.FileName.Contains("=") || file.FileName.Contains("+") || file.FileName.Contains("!") || file.FileName.Contains("^") || file.FileName.Contains("&") || file.FileName.Contains("#") || file.FileName.Contains("[") || file.FileName.Contains("]") || file.FileName.Contains("{") || file.FileName.Contains("}"))
    //                fileName = Path.Combine(uploadedDocsDir, file.FileName.Replace('%', '_').Replace('=', '_').Replace('+', '_').Replace('!', '_').Replace('^', '_').Replace('&', '_').Replace('#', '_').Replace('[', '_').Replace(']', '_').Replace('{', '_').Replace('}', '_'));
    //            else
    //                fileName = Path.Combine(uploadedDocsDir, file.FileName);

    //            // Save the image
    //            file.SaveAs(fileName, true);
    //        }
    //        catch (Exception ex)
    //        {
    //            ErrorLogDb.NotifyErrorByEmail(ex,string.Empty);
    //            return false;
    //        }
    //    }

    //    return true;
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    private bool SaveUploadedDocument(int setId, DateTime verificationDateIn)
    {
        //string userTempPath = Util.GetDocForOcrFolder(setId,verificationDateIn);

        //// Add escape characters to the path
        //userTempPath = userTempPath.Replace(@"\", @"\\");

        string uploadedDocsDir = Util.GetDocForOcrFolder(setId, verificationDateIn);

        // If the folder does not exists, create one
        if (!Directory.Exists(uploadedDocsDir))
            Directory.CreateDirectory(uploadedDocsDir);
        else
        {
            Util.DeleteFolderContents(uploadedDocsDir);
        }

        // Save the uploaded files
        foreach (UploadedFile file in DocumentRadAsyncUpload.UploadedFiles)
        {
            try
            {
                string fileName = Path.Combine(uploadedDocsDir, file.FileName);
                if (file.FileName.Contains("%") || file.FileName.Contains("=") || file.FileName.Contains("+") || file.FileName.Contains("!") || file.FileName.Contains("^") || file.FileName.Contains("&") || file.FileName.Contains("#") || file.FileName.Contains("[") || file.FileName.Contains("]") || file.FileName.Contains("{") || file.FileName.Contains("}"))
                    fileName = Path.Combine(uploadedDocsDir, file.FileName.Replace('%', '_').Replace('=', '_').Replace('+', '_').Replace('!', '_').Replace('^', '_').Replace('&', '_').Replace('#', '_').Replace('[', '_').Replace(']', '_').Replace('{', '_').Replace('}', '_'));
                else
                    fileName = Path.Combine(uploadedDocsDir, file.FileName);

                // Save the image
                file.SaveAs(fileName, true);
            }
            catch (Exception ex)
            {
                ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
                return false;
            }
        }

        return true;
    }
    #endregion


    /// <summary>
    /// Save the raw file(s)
    /// </summary>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //private bool SaveRawFile(int setId)
    //{
    //    string userTempPath = Util.GetDocForOcrFolder();

    //    // Add escape characters to the path
    //    userTempPath = userTempPath.Replace(@"\", @"\\");

    //    string uploadedDocsDir = Path.Combine(userTempPath, setId.ToString());

    //    RawFileDb rawFileDb = new RawFileDb();

    //    DirectoryInfo dirInfo = new DirectoryInfo(uploadedDocsDir);
    //    if (dirInfo.GetFiles().Length > 0)
    //    {
    //        FileInfo[] files = dirInfo.GetFiles("*.*");

    //        // Save the uploaded files
    //        foreach (FileInfo file in files)
    //        {
    //            try
    //            {
    //                if (!file.Extension.ToUpper().EndsWith(".DB"))
    //                {
    //                    // Insert the raw file
    //                    //int temp = rawFileDb.Insert(setId, file.Name, BllFunc.FileToBytes(file.FullName));
    //                    int temp = rawFileDb.Insert(setId, file.Name, new byte[0]);

    //                    // Create the folder for each file uploaded
    //                    string rawFileDir = Path.Combine(uploadedDocsDir, temp.ToString());

    //                    try
    //                    {
    //                        Directory.Delete(rawFileDir);
    //                    }
    //                    catch (Exception)
    //                    {
    //                    }

    //                    // Create the directory
    //                    if (!Directory.Exists(rawFileDir))
    //                        Directory.CreateDirectory(rawFileDir);

    //                    string newFileName = Path.Combine(rawFileDir, file.Name);

    //                    // Copy the file to its respective dir
    //                    file.MoveTo(newFileName);
    //                }
    //            }
    //            catch (Exception)
    //            {
    //                return false;
    //            }
    //        }
    //    }

    //    return true;
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    private bool SaveRawFile(int setId, DateTime verificationDateIn)
    {
        //string userTempPath = Util.GetDocForOcrFolder();

        //// Add escape characters to the path
        //userTempPath = userTempPath.Replace(@"\", @"\\");

        string uploadedDocsDir = Util.GetDocForOcrFolder(setId, verificationDateIn);

        RawFileDb rawFileDb = new RawFileDb();

        DirectoryInfo dirInfo = new DirectoryInfo(uploadedDocsDir);
        if (dirInfo.GetFiles().Length > 0)
        {
            FileInfo[] files = dirInfo.GetFiles("*.*");

            // Save the uploaded files
            foreach (FileInfo file in files)
            {
                try
                {
                    if (!file.Extension.ToUpper().EndsWith(".DB"))
                    {
                        // Insert the raw file
                        //int temp = rawFileDb.Insert(setId, file.Name, BllFunc.FileToBytes(file.FullName));
                        int temp = rawFileDb.Insert(setId, file.Name, new byte[0]);

                        // Create the folder for each file uploaded
                        string rawFileDir = Path.Combine(uploadedDocsDir, temp.ToString());

                        try
                        {
                            Directory.Delete(rawFileDir);
                        }
                        catch (Exception)
                        {
                        }

                        // Create the directory
                        if (!Directory.Exists(rawFileDir))
                            Directory.CreateDirectory(rawFileDir);

                        string newFileName = Path.Combine(rawFileDir, file.Name);

                        // Copy the file to its respective dir
                        file.MoveTo(newFileName);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

    #endregion
}

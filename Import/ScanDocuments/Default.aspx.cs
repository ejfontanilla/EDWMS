using System;
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
using System.Diagnostics;

public partial class Import_ScanDocuments_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ConfirmPanel.Visible = (Request["cfm"] == "1");
            WarningPanel.Visible = (Request["cfm"] == "0");

            // Assign the scan filename
            ScanFileName.Value = Membership.GetUser().UserName + "_" +
                Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt);

            // Assign the Ids of the scan file name hidden field
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AssignIdScript",
                String.Format("javascript:AssignIds('{0}', '{1}');", SaveButton.ClientID, ScanFileName.ClientID), true);
        }

        // Set access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        string[] parameters = e.Argument.Split(new char[] { ',' });

        if (parameters[0].ToUpper().Equals("SAVESET"))
        {
            // Save the set
            Save();
        }
        else if (parameters[0].ToUpper().Equals("UPDATEREFNO"))
        {
            ScanUploadFields.SetComboBoxText(parameters[1]);

            ScriptManager.RegisterStartupScript(
                this.Page, 
                this.GetType(), 
                "InitializeTwainScript", 
                "InitializeTwain();", 
                true);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Scan functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Import, AccessControlSettingEnum.Scan);

        // Set the visibility of the buttons
        ScanBtn.Visible = hasAccess;
    }

    /// <summary>
    /// Save 
    /// </summary>
    /// <param name="setId"></param>
    /// <returns></returns>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //private void Save()
    //{
    //    Page.Validate();

    //    if (Page.IsValid)
    //    {
    //        //bool result = false;

    //        DocSetDb docSetDb = new DocSetDb();

    //        int departmentId = -1;
    //        int sectionid = -1;
    //        string setNo = Util.FormulateSetNumber(docSetDb.GetNextIdNo(), out departmentId, out sectionid);
    //        string referenceNo = ScanUploadFields.ReferenceNumber.Trim();
    //        int docAppId = ScanUploadFields.DocAppId;
    //        string referenceType = ScanUploadFields.ReferenceType;
    //        string block = ScanUploadFields.Block;
    //        int streetId = ScanUploadFields.StreetId;
    //        string level = ScanUploadFields.Level;
    //        string unitNo = ScanUploadFields.Unit;
    //        string channel = ScanUploadFields.Channel;
    //        SetStatusEnum status = SetStatusEnum.Pending_Categorization;
    //        Guid importedBy = (Guid)Membership.GetUser().ProviderUserKey;

    //        HleInterfaceDb interfaceDb = new HleInterfaceDb();
    //        DocAppDb docAppDb = new DocAppDb();
    //        docAppDb.GetAppDetails(docAppId, referenceNo, referenceType, out docAppId);
    //        int setId = 0;

    //        // Save the set information
    //        for (int cnt = 0; cnt < 2; cnt++)
    //        {
    //            //Added by edward Try Catch for Reducing Error Notifications - Insert bug in UploadDocument 19/8/2015
    //            try
    //            {
    //                setId = docSetDb.Insert(setNo, DateTime.Now, status, block, streetId, level, unitNo, channel, importedBy,
    //                null, departmentId, sectionid, docAppId);
    //                if (setId > 0)
    //                    break;
    //            }
    //            catch (Exception ex)
    //            {
    //                ErrorLogDb.NotifyErrorByEmail(ex,string.Empty);
    //                WarningPanel.Visible = true;                    
    //            }                          
    //        }

    //        if (setId > 0)
    //        {
    //            // Save the raw file
    //            bool result = SaveRawFile(setId);

    //            if (!result)
    //            {
    //                // Delete the set record and all related files during saving error

    //                try
    //                {
    //                    string userTempPath = Util.GetDocForOcrFolder();

    //                    // Add escape characters to the path
    //                    userTempPath = userTempPath.Replace(@"\", @"\\");

    //                    string uploadedDocsDir = Path.Combine(userTempPath, setId.ToString());

    //                    DirectoryInfo mainDir = new DirectoryInfo(uploadedDocsDir);
    //                    //mainDir.Delete(true);
    //                }
    //                catch (Exception ex)
    //                {
    //                    ErrorLogDb.NotifyErrorByEmail(ex, string.Empty);
    //                }
    //                finally
    //                {
    //                    //docSetDb.Delete(setId);
    //                }
    //            }
    //            else
    //            {
    //                // Update the 'ReadyForOcr' flag
    //                docSetDb.SetReadyForOcr(setId, true);
    //            }

    //            // Clean the temporary folder
    //            try
    //            {
    //                string userTempPath = Util.GetUsersDocForOcrTempFolder();

    //                // Add escape characters to the path
    //                userTempPath = userTempPath.Replace(@"\", @"\\");
    //                DirectoryInfo mainDir = new DirectoryInfo(userTempPath);
    //                mainDir.Delete(true);
    //            }
    //            catch (Exception)
    //            {
    //            }

    //            Response.Redirect(String.Format("Default.aspx?cfm={0}", (result ? "1" : "0")));
    //        }
    //    }
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    private void Save()
    {
        Page.Validate();

        if (Page.IsValid)
        {
            //bool result = false;

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

            HleInterfaceDb interfaceDb = new HleInterfaceDb();
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
                    WarningPanel.Visible = true;
                }
            }

            if (setId > 0)
            {
                // Save the raw file
                bool result = SaveRawFile(setId, verificationDateIn);

                if (!result)
                {
                    // Delete the set record and all related files during saving error

                    try
                    {
                        DirectoryInfo mainDir = new DirectoryInfo(Util.GetDocForOcrFolder(setId, verificationDateIn));
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

                // Clean the temporary folder
                try
                {
                    string userTempPath = Util.GetUsersDocForOcrTempFolder();

                    // Add escape characters to the path
                    userTempPath = userTempPath.Replace(@"\", @"\\");
                    DirectoryInfo mainDir = new DirectoryInfo(userTempPath);
                    mainDir.Delete(true);
                }
                catch (Exception)
                {
                }

                Response.Redirect(String.Format("Default.aspx?cfm={0}", (result ? "1" : "0")));
            }
        }
    }
    #endregion

    /// <summary>
    /// Call the web service function to save the raw file via javascript
    /// </summary>
    #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //private bool SaveRawFile(int setId)
    //{
    //    string userTempPath = Util.GetUsersDocForOcrTempFolder();
    //    string setDirPath = Util.GetDocForOcrFolder();

    //    // Add escape characters to the path
    //    userTempPath = userTempPath.Replace(@"\", @"\\");
    //    setDirPath = setDirPath.Replace(@"\", @"\\");

    //    string uploadedDocsDir = Path.Combine(setDirPath, setId.ToString());

    //    // If the folder does not exists, create one
    //    try
    //    {
    //        if (!Directory.Exists(uploadedDocsDir))
    //            Directory.CreateDirectory(uploadedDocsDir);
    //        else
    //        {
    //            Util.DeleteFolderContents(uploadedDocsDir);
    //        }
    //    }
    //    catch
    //    {
    //        return false;
    //    }

    //    RawFileDb rawFileDb = new RawFileDb();

    //    DirectoryInfo dirInfo = new DirectoryInfo(userTempPath);
    //    if (dirInfo.GetFiles().Length > 0)
    //    {
    //        //FileInfo[] files = dirInfo.GetFiles("*.*");
    //        //FileInfo[] files = dirInfo.GetFiles(ScanFileName.Value + ".pdf");
    //        FileInfo[] files = dirInfo.GetFiles("*.pdf");

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
        string userTempPath = Util.GetUsersDocForOcrTempFolder();

        // Add escape characters to the path
        userTempPath = userTempPath.Replace(@"\", @"\\");

        string uploadedDocsDir = Util.GetDocForOcrFolder(setId, verificationDateIn);

        // If the folder does not exists, create one
        try
        {
            if (!Directory.Exists(uploadedDocsDir))
                Directory.CreateDirectory(uploadedDocsDir);
            else
            {
                Util.DeleteFolderContents(uploadedDocsDir);
            }
        }
        catch
        {
            return false;
        }

        RawFileDb rawFileDb = new RawFileDb();

        DirectoryInfo dirInfo = new DirectoryInfo(userTempPath);
        if (dirInfo.GetFiles().Length > 0)
        {
            //FileInfo[] files = dirInfo.GetFiles("*.*");
            //FileInfo[] files = dirInfo.GetFiles(ScanFileName.Value + ".pdf");
            FileInfo[] files = dirInfo.GetFiles("*.pdf");

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

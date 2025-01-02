using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Dwms.Web;
using Dwms.Bll;
using Ionic.Zip;
using System.IO;
using System.Collections;

public partial class Common_DownloadDocument : System.Web.UI.Page
{
    int appId = -1;

    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (String.IsNullOrEmpty(Request["id"]))
            {
                Response.Write("id is null");
                Response.End();
            }

            if (String.IsNullOrEmpty(Request["appId"]))
            {
                Response.Write("appid is null");
                Response.End();
            }
            else
                appId = int.Parse(Request["appId"].ToString());

            #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            //DocDb docDb = new DocDb();
            //RawPageDb rawPageDb = new RawPageDb();
            //DocTypeDb docTypeDb = new DocTypeDb();

            //// RawPage Folder
            //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
            //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);


            //string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");

            //string idsStr = Request["id"];

            //string[] ids = idsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //ArrayList docList = new ArrayList();

            //foreach (string idStr in ids)
            //{
            //    int id = int.Parse(idStr);

            //    Doc.DocDataTable docTable = docDb.GetDocById(id);

            //    if (docTable.Rows.Count > 0)
            //    {
            //        Doc.DocRow doc = docTable[0];

            //        ArrayList pageList = new ArrayList();

            //        RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(id);

            //        for (int cnt = 0; cnt < rawPages.Count; cnt++)
            //        {
            //            RawPage.RawPageRow rawPage = rawPages[cnt];
            //            DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

            //            if (rawPageDirs.Length > 0)
            //            {
            //                DirectoryInfo rawPageDir = rawPageDirs[0];

            //                // Get the raw page for download
            //                FileInfo[] rawPageFiles = rawPageDir.GetFiles();

            //                bool hasRawPage = false;
            //                foreach (FileInfo rawPageFile in rawPageFiles)
            //                {
            //                    if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
            //                        !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
            //                        !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
            //                    {
            //                        if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
            //                        {
            //                            //path = Util.CreatePdfFileFromImage(path);
            //                            pageList.Add(rawPageFile.FullName);
            //                            hasRawPage = true;
            //                        }
            //                    }
            //                }

            //                // If the raw page is not found, use the searcheable PDF
            //                if (!hasRawPage)
            //                {
            //                    FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

            //                    if (rawPagePdfFiles.Length > 0)
            //                        pageList.Add(rawPagePdfFiles[0].FullName);
            //                }
            //            }
            //        }

            //        if (!Directory.Exists(saveDir))
            //            Directory.CreateDirectory(saveDir);

            //        if (pageList.Count > 0)
            //        {
            //            string docTypeDesc = doc.DocTypeCode;

            //            DocType.DocTypeDataTable docTypeTable = docTypeDb.GetDocType(doc.DocTypeCode);

            //            if (docTypeTable.Rows.Count > 0)
            //            {
            //                DocType.DocTypeRow docType = docTypeTable[0];
            //                docTypeDesc = docType.Description;
            //            }

            //            string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + doc.Id.ToString() + ".pdf");

            //            try
            //            {
            //                if (File.Exists(mergedFileName))
            //                    File.Delete(mergedFileName);
            //            }
            //            catch (Exception)
            //            {
            //            }

            //            string errorMessage = string.Empty;

            //            Util.MergePdfFiles(pageList, mergedFileName, out errorMessage);

            //            docList.Add(mergedFileName);
            //        }
            //    }
            //}

            //if (docList.Count > 0)
            //{
            //    using (ZipFile zip = new ZipFile())
            //    {
            //        foreach (string docPath in docList)
            //        {
            //            FileInfo fi = new FileInfo(docPath);

            //            try
            //            {
            //                using (FileStream fileStream = fi.Open(FileMode.Open, FileAccess.Read))
            //                {
            //                    BinaryReader binaryReader = new BinaryReader(fileStream);
            //                    byte[] fileBytes = binaryReader.ReadBytes((Int32)fi.Length);

            //                    string rawFileName = fi.Name;

            //                    zip.AddEntry(rawFileName, fileBytes);
            //                }
            //            }
            //            catch (Exception)
            //            {
            //            }
            //        }

            //        DocAppDb docAppDb = new DocAppDb();
            //        DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(appId);
            //        DocApp.DocAppRow docAppRow = docApp[0];

            //        //update the docapp download status
            //        docAppDb.UpdateDownloadDetails(appId, DownloadStatusEnum.Downloaded, (Guid)user.ProviderUserKey);

            //        //send file for download
            //        string fileName = docAppRow.RefNo + ".zip";
            //        Response.Clear();
            //        Response.ContentType = "application/zip";
            //        Response.AddHeader("content-disposition", "filename=" + fileName);
            //        zip.Save(Response.OutputStream);

            //    }
            //}
            //else
            //{
            //    Response.Write("No documents were found.");
            //    Response.End();
            //}      
            #endregion

            #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            string idsStr = Request["id"];

            ArrayList docList = Util.MergeDocumentsToTemp(idsStr, saveDir);
            
            if (docList.Count > 0)
            {
                using (ZipFile zip = new ZipFile())
                {
                    foreach (string docPath in docList)
                    {
                        FileInfo fi = new FileInfo(docPath);

                        try
                        {
                            using (FileStream fileStream = fi.Open(FileMode.Open, FileAccess.Read))
                            {
                                BinaryReader binaryReader = new BinaryReader(fileStream);
                                byte[] fileBytes = binaryReader.ReadBytes((Int32)fi.Length);

                                string rawFileName = fi.Name;

                                zip.AddEntry(rawFileName, fileBytes);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    DocAppDb docAppDb = new DocAppDb();
                    DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(appId);
                    DocApp.DocAppRow docAppRow = docApp[0];

                    //update the docapp download status
                    docAppDb.UpdateDownloadDetails(appId, DownloadStatusEnum.Downloaded, (Guid)user.ProviderUserKey);

                    //send file for download
                    string fileName = docAppRow.RefNo + ".zip";
                    Response.Clear();
                    Response.ContentType = "application/zip";
                    Response.AddHeader("content-disposition", "filename=" + fileName);
                    zip.Save(Response.OutputStream);

                }
            }
            else
            {
                Response.Write("No documents were found.");
                Response.End();
            }
            #endregion
        }
        else
        {
            Response.Write(Constants.UnathorizedAccessErrorMessage);
            Response.End();
        }
    }
}
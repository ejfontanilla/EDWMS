﻿using System;
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

public partial class Common_ConsolidatedWorkSheetPDF : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (String.IsNullOrEmpty(Request["id"]))
            {
                Response.Write("Id is null");
                Response.End();
            }

            if (String.IsNullOrEmpty(Request["appId"]))
            {
                Response.Write("Application Id is null");
                Response.End();
            }

            #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            //DocDb docDb = new DocDb();
            //DocAppDb docAppDb = new DocAppDb();
            //RawPageDb rawPageDb = new RawPageDb();
            //DocTypeDb docTypeDb = new DocTypeDb();

            //// RawPage Folder
            //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
            //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);

            //string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");

            //string idsStr = Request["id"];
            //int appId = int.Parse(Request["appId"]);

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

            //            Util.MergePdfFiles(pageList, mergedFileName);

            //            docList.Add(mergedFileName);
            //        }
            //    }
            //}

            //#region Adding the Income Worksheet
            ////string strFileName = "Income_Extraction_Worksheet.pdf";
            ////MemoryStream pdfStream = HousingGrantGenerator.GeneratePDFHousingGrant(appId);

            ////DocApp.DocAppDataTable docApps = docAppDb.GetDocAppIncomeExtractionById(appId);
            ////if (docApps.Rows.Count > 0)
            ////{
            ////    DocApp.DocAppRow docAppRow = docApps[0];
            ////    strFileName = string.Format("{0}_{1}_{2}", docAppRow.RefNo, DateTime.Now.ToShortDateString().Replace("/", "_"), strFileName);
            ////    string workSheetPath = Path.Combine(saveDir, strFileName);
            ////    //http://stackoverflow.com/questions/2186817/itextsharp-filestream-corrupt-pdf-file
            ////    byte[] content = pdfStream.ToArray();
            ////    using (FileStream fs = File.Create(workSheetPath))
            ////    {
            ////        fs.Write(content, 0, (int)content.Length);
            ////    }
            ////    docList.Add(workSheetPath);
            ////}            
            //#endregion


            //if (docList.Count > 0)
            //{
            //    DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppById(appId);
            //    string refNo = string.Empty;

            //    if (docAppTable.Rows.Count > 0)
            //    {
            //        DocApp.DocAppRow docApp = docAppTable[0];
            //        refNo = docApp.RefNo;
            //    }

            //    string mergedFileName = Path.Combine(saveDir, refNo + "_" +
            //        Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt) + ".pdf");

            //    try
            //    {
            //        if (File.Exists(mergedFileName))
            //            File.Delete(mergedFileName);
            //    }
            //    catch (Exception)
            //    {
            //    }

            //    string errorMessage = string.Empty;
            //    Util.MergePdfFiles(docList, mergedFileName, out errorMessage);

            //    if (String.IsNullOrEmpty(errorMessage))
            //    {
            //        FileInfo mergedPdf = new FileInfo(mergedFileName);

            //        if (mergedPdf.Exists)
            //        {
            //            Response.Redirect("GenericFileHandler.ashx?filePath=" + mergedPdf.FullName);
            //        }
            //        else
            //        {
            //            Response.Write("PDF file cannot be found.");
            //            Response.End();
            //        }
            //    }
            //    else
            //    {
            //        Response.Write(errorMessage);
            //        Response.End();
            //    }
            //}
            //else
            //{
            //    Response.Write("No documents were found.");
            //    Response.End();
            //}
            #endregion

            #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            DocAppDb docAppDb = new DocAppDb();                        

            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            string idsStr = Request["id"];
            int appId = int.Parse(Request["appId"]);

            ArrayList docList = Util.MergeDocumentsToTemp(idsStr, saveDir);

            if (docList.Count > 0)
            {
                DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppById(appId);
                string refNo = string.Empty;

                if (docAppTable.Rows.Count > 0)
                {
                    DocApp.DocAppRow docApp = docAppTable[0];
                    refNo = docApp.RefNo;
                }

                string mergedFileName = Path.Combine(saveDir, refNo + "_" +
                    Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt) + ".pdf");

                try
                {
                    if (File.Exists(mergedFileName))
                        File.Delete(mergedFileName);
                }
                catch (Exception)
                {
                }

                string errorMessage = string.Empty;
                Util.MergePdfFiles(docList, mergedFileName, out errorMessage);

                if (String.IsNullOrEmpty(errorMessage))
                {
                    FileInfo mergedPdf = new FileInfo(mergedFileName);

                    if (mergedPdf.Exists)
                    {
                        Response.Redirect("GenericFileHandler.ashx?filePath=" + mergedPdf.FullName);
                    }
                    else
                    {
                        Response.Write("PDF file cannot be found.");
                        Response.End();
                    }
                }
                else
                {
                    Response.Write(errorMessage);
                    Response.End();
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
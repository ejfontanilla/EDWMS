using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using System.IO;
using System.Collections;
using Dwms.Web;


//<%--This page is used by the CBD Service (DWMS->CDB) to help to download the file--%>


public partial class Common_DownloadImage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        #region TestArea



        ////int appId = -1;

        //if (String.IsNullOrEmpty(Request["filepath"]))
        //{
        //    Response.Write("file path is null");
        //    Response.End();
        //}

        //DocDb docDb = new DocDb();
        //RawPageDb rawPageDb = new RawPageDb();
        //DocTypeDb docTypeDb = new DocTypeDb();

        //// RawPage Folder
        //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
        //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);

        //string saveDir = HttpContext.Current.Server.MapPath(Retrieve.GetTempDirPath());

        //string idsStr = Request["filepath"];

        //ArrayList docList = new ArrayList();

        ////foreach (string idStr in ids)
        ////{
        //    int id = int.Parse(Path.GetFileNameWithoutExtension(idsStr));

        //    Doc.DocDataTable docTable = docDb.GetDocById(id);

        //    if (docTable.Rows.Count > 0)
        //    {
        //        Doc.DocRow doc = docTable[0];

        //        ArrayList pageList = new ArrayList();

        //        RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(id);
        //        if (docTable.Rows.Count > 0)
        //        {
        //            for (int cnt = 0; cnt < rawPages.Count; cnt++)
        //            {

        //                RawPage.RawPageRow rawPage = rawPages[cnt];

        //                DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

        //                if (rawPageDirs.Length > 0)
        //                {
        //                    DirectoryInfo rawPageDir = rawPageDirs[0];

        //                    // Get the raw page for download
        //                    FileInfo[] rawPageFiles = rawPageDir.GetFiles();

        //                    bool hasRawPage = false;
        //                    foreach (FileInfo rawPageFile in rawPageFiles)
        //                    {
        //                        if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
        //                            !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
        //                            !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
        //                        {
        //                            if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
        //                            {
        //                                //path = Util.CreatePdfFileFromImage(path);
        //                                pageList.Add(rawPageFile.FullName);
        //                                hasRawPage = true;
        //                            }
        //                        }
        //                    }

        //                    // If the raw page is not found, use the searcheable PDF
        //                    if (!hasRawPage)
        //                    {
        //                        FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

        //                        if (rawPagePdfFiles.Length > 0)
        //                            pageList.Add(rawPagePdfFiles[0].FullName);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Response.Write("PDF file 3 cannot be found.");
        //            Response.End();
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

        //            //string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + doc.Id.ToString() + ".pdf");
        //            string mergedFileName = Path.Combine(saveDir, doc.Id.ToString() + ".pdf");

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

        //            //docList.Add(mergedFileName);


        //            if (String.IsNullOrEmpty(errorMessage))
        //            {
        //                FileInfo mergedPdf = new FileInfo(mergedFileName);

        //                if (mergedPdf.Exists)
        //                {
        //                    Response.Redirect("GenericFileHandler.ashx?filePath=" + mergedPdf.FullName);

        //                    //File.Delete(mergedFileName);
        //                }
        //                //else
        //                //{
        //                //    Response.Write("PDF file 4 cannot be found.");
        //                //    Response.End();
        //                //}
        //            }
        //            //else
        //            //{
        //            //    Response.Write(errorMessage);
        //            //    Response.End();
        //            //}


        //        }
        //        else
        //        {
        //            Response.Write("PDF file 5 cannot be found.");
        //            Response.End();
        //        }
        //    }
        //    else
        //    {
        //        Response.Write("PDF file 6 cannot be found.");
        //        Response.End();
            
        //    }
        ////}

        #endregion TestArea

        #region Code
            string saveDir = HttpContext.Current.Server.MapPath(Retrieve.GetTempDirPath());

            string fileName = Path.GetFileName(Request["file"]);

            //string mergedFileName = Path.Combine(saveDir, fileName + ".pdf");
            string mergedFileName = Path.Combine(saveDir, fileName);


            FileInfo mergedPdf = new FileInfo(mergedFileName);

            if (mergedPdf.Exists)
            {
                 Response.Redirect("GenericFileHandler.ashx?filePath=" + mergedPdf.FullName);
                 File.Delete(mergedPdf.FullName);

            }
        #endregion Code
    }
}
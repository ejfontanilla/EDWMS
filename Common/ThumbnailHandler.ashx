<%@ WebHandler Language="C#" Class="ThumbnailHandler" %>

using System;
using System.Web;
using Dwms.Bll;

public class ThumbnailHandler : IHttpHandler
{

    public bool IsReusable
    {
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        int id = int.Parse(context.Request["id"]);
        string filePath = string.Empty;

        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

        // RawPage Folder
        //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
        //System.IO.DirectoryInfo rawPageDirInfo = new System.IO.DirectoryInfo(rawPageDirPath);
        //System.IO.DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(id.ToString());

        //if (rawPageDirs.Length > 0)
        //{
        //    System.IO.DirectoryInfo rawPageDir = rawPageDirs[0];

        //    // Get the raw page for download
        //    System.IO.FileInfo[] rawPageFiles = rawPageDir.GetFiles();

        //    foreach (System.IO.FileInfo rawPageFile in rawPageFiles)
        //    {
        //        if (rawPageFile.Name.ToUpper().EndsWith("_TH.JPG") || rawPageFile.Name.ToUpper().EndsWith("_TH.JPEG"))
        //        {
        //            filePath = rawPageFile.FullName;
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    filePath = HttpContext.Current.Server.MapPath("~/Data/Images/Question.gif");
        //}

        System.IO.DirectoryInfo rawPageDirInfo = Dwms.Web.Util.GetIndividualRawPageOcrDirectoryInfo(id);        

        if (rawPageDirInfo.Exists)
        {
            // Get the raw page for download
            System.IO.FileInfo[] rawPageFiles = rawPageDirInfo.GetFiles();

            foreach (System.IO.FileInfo rawPageFile in rawPageFiles)
            {
                if (rawPageFile.Name.ToUpper().EndsWith("_TH.JPG") || rawPageFile.Name.ToUpper().EndsWith("_TH.JPEG"))
                {
                    filePath = rawPageFile.FullName;
                    break;
                }
            }
        }
        else
        {
            filePath = HttpContext.Current.Server.MapPath("~/Data/Images/Question.gif");
        }
        
        #endregion

        

        if (System.IO.File.Exists(filePath))
        {
            context.Response.ContentType = Retrieve.GetContentType(filePath);
            context.Response.BinaryWrite(System.IO.File.ReadAllBytes(filePath));
            context.Response.End();
        }
    }
}
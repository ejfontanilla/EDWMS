<%@ WebHandler Language="C#" Class="GenericFileHandler" %>

using System;
using System.Web;
using Dwms.Bll;

public class GenericFileHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string filePath = context.Request["filePath"];
        System.IO.FileInfo mergedPdf = new System.IO.FileInfo(filePath);

        GetFile(context, System.IO.File.ReadAllBytes(filePath), mergedPdf.Name);
    }

    private static void GetFile(HttpContext context, byte[] attachmentFile, string fileName)
    {
        context.Response.Clear();

        try
        {
            context.Response.ContentType = Retrieve.GetContentType(fileName);
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename=\"{0}\"", fileName));
            context.Response.BinaryWrite(attachmentFile);
            context.Response.End();
        }
        catch (Exception)
        {
        }
    }
    
    public bool IsReusable {
        get {
            return false;
        }
    }

}
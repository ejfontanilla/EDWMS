<%@ WebHandler Language="C#" Class="HousingGrant" %>

using System;
using System.IO;
using System.Web;
using Dwms.Bll;

public class HousingGrant : IHttpHandler {

    //public void ProcessRequest(HttpContext context)
    //{
    //    string filePath = context.Request["filePath"];
    //    System.IO.FileInfo mergedPdf = new System.IO.FileInfo(filePath);

    //    GetFile(context, System.IO.File.ReadAllBytes(filePath), mergedPdf.Name);
    //}

    //private static void GetFile(HttpContext context, byte[] attachmentFile, string fileName)
    //{
    //    context.Response.Clear();

    //    try
    //    {
    //        context.Response.ContentType = Retrieve.GetContentType(fileName);
    //        context.Response.AddHeader("content-disposition", String.Format("attachment;filename=\"{0}\"", fileName));
    //        context.Response.BinaryWrite(attachmentFile);
    //        context.Response.End();
    //    }
    //    catch (Exception ex)
    //    {
    //    }
    //}


    public void ProcessRequest(HttpContext context)
    {
        int DocAppId = int.Parse(context.Request.QueryString["docAppId"]);

        MemoryStream pdfStream = HousingGrantGenerator.GeneratePDFHousingGrant(DocAppId);

        if (pdfStream != null)
        {
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-disposition", "attachment;filename=Income_Extraction_Worksheet.pdf");
            context.Response.OutputStream.Write(pdfStream.GetBuffer(), 0, pdfStream.GetBuffer().Length);
        }
        else
        {
            context.Response.Write("Error generating PDF letter.");
        }
        //context.Response.OutputStream.Flush();
        //context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
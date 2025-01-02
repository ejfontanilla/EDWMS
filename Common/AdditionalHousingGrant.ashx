<%@ WebHandler Language="C#" Class="AdditionalHousingGrant" %>

using System;
using System.IO;
using System.Web;

public class AdditionalHousingGrant : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) 
    {
        int DocAppId = int.Parse(context.Request.QueryString["docAppId"]);

        MemoryStream pdfStream = AddHousingGrantGenerator.GeneratePDFHousingGrant(DocAppId);

        if (pdfStream != null)
        {
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("content-disposition", "attachment;filename=Additional_Housing_Grant.pdf");
            context.Response.OutputStream.Write(pdfStream.GetBuffer(), 0, pdfStream.GetBuffer().Length);
            context.Response.OutputStream.Flush();
        }
        else
        {
            context.Response.Write("Error generating PDF letter.");
        }
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
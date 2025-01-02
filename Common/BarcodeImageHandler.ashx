<%@ WebHandler Language="C#" Class="BarcodeImageHandler" %>

using System;
using System.Web;
using Dwms.Bll;
using Dwms.Web;

public class BarcodeImageHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string fileName = context.Request["filename"];
        GetImage(context, fileName);
    }

    private static void GetImage(HttpContext context, string photoName)
    {
        string filePath = Retrieve.GetTempDirPath() + photoName;
        context.Response.Buffer = true;
        context.Response.Clear();
        context.Response.ContentType = "image/jpeg";
        context.Response.WriteFile(filePath);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
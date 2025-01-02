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
        string filePath = context.Request["filePath"];

        if (System.IO.File.Exists(filePath))
        {
            try
            {
                context.Response.ContentType = Retrieve.GetContentType(filePath);
                context.Response.BinaryWrite(System.IO.File.ReadAllBytes(filePath));
                //context.Response.Flush();   //Added By Edward 2014/04/15
                
                context.ApplicationInstance.CompleteRequest();  //Added by Edward for OOM 2017/09/21
                //context.Response.End();    //Commented By Edward 2014/04/15    
                
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.ToString());
                context.Response.End();
            }
        }
    }

   
}
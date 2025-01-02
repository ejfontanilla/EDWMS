<%@ WebHandler Language="C#" Class="SampleDocumentFileHandler" %>

using System;
using System.Web;
using Dwms.Bll;
using Dwms.Web;
using System.Web.Security;

public class SampleDocumentFileHandler : IHttpHandler 
{
    
    public void ProcessRequest (HttpContext context) 
    {
        int? id;

        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (context.Request.QueryString["id"] != null)
            {
                id = int.Parse(context.Request.QueryString["id"]);
            }
            else
            {
                return;
            }

            // Delete the sample doc from the physical folder
            try
            {
                SampleDocDb sampleDocDb = new SampleDocDb();
                SampleDoc.SampleDocDataTable dt = sampleDocDb.GetDataById(id.Value);

                if (dt.Rows.Count > 0)
                {
                    SampleDoc.SampleDocRow dr = dt[0];

                    string sampleDocPath = Util.GetUsersSampleDocTempFolder();

                    string docTypeDir = System.IO.Path.Combine(sampleDocPath, dr.DocTypeCode);

                    System.IO.DirectoryInfo sampleDocMainDir = new System.IO.DirectoryInfo(docTypeDir);

                    string sampleDocFileDir = System.IO.Path.Combine(sampleDocMainDir.FullName, dr.Id.ToString());

                    System.IO.DirectoryInfo sampleDocDir = new System.IO.DirectoryInfo(sampleDocFileDir);

                    System.IO.FileInfo[] fileInfos = sampleDocDir.GetFiles(dr.FileName);

                    if (fileInfos.Length > 0)
                    {
                        System.IO.FileInfo file = fileInfos[0];

                        GetFile(context, file);
                    }
                    else
                    {
                        context.Response.Write("File cannot be found.");
                        context.Response.End();
                    }
                }
                else
                {
                    context.Response.Write("Sample document does not exists.");
                    context.Response.End();
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message.ToString());
                context.Response.End();
            }
        }
        else
        {
            context.Response.Write(Constants.UnathorizedAccessErrorMessage);
            context.Response.End();
        }
    }

    private static void GetFile(HttpContext context, System.IO.FileInfo file)
    {
        context.Response.Clear();

        try
        {
            context.Response.ContentType = Retrieve.GetContentType(file.Name);
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename=\"{0}\"", file.Name));
            //context.Response.BinaryWrite(attachmentFile);
            context.Response.WriteFile(file.FullName);
            context.Response.End();
        }
        catch (Exception)
        {
            return;
        }
    }
 
    public bool IsReusable 
    {
        get 
        {
            return false;
        }
    }

}
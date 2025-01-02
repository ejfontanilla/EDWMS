using System;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Dwms.Bll;

/// <summary>
/// Summary description for FunctionWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class FunctionWebService : System.Web.Services.WebService {

    public FunctionWebService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    /// <summary>
    /// Get Document File Size
    /// </summary>
    /// <param name="docId"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public long GetDocumentFileSize(int docId)
    {
        DocDb docDb = new DocDb();
        return docDb.GetDocumentFileSize(docId);
    }

    [WebMethod(EnableSession = true)]
    public long GetDocumentFileSizeByIds(string docIds)
    {
        DocDb docDb = new DocDb();
        return docDb.GetDocumentFileSizeByIds(docIds);
    }

    [WebMethod(EnableSession = true)]
    public string GetDocumentFileSizeByIdsWithFormatSize(string docIds)
    {
        DocDb docDb = new DocDb();
        return Format.FormatFileSize(Convert.ToDouble(docDb.GetDocumentFileSizeByIds(docIds)));
    }

    [WebMethod(EnableSession = true)]
    public string GetDocumentZippedFileSizeByIdsWithFormatSize(string docIds, string docAppId)
    {
        DocDb docDb = new DocDb();
        return Format.FormatFileSize(Convert.ToDouble(docDb.GetDocumentZippedFileSizeByIds(docIds, int.Parse(docAppId))));
    }

    


}

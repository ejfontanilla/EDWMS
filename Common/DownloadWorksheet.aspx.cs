using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Dwms.Bll;
public partial class Common_DownloadWorksheet : System.Web.UI.Page
{
    public int? docAppId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["docAppId"]))
        {
            docAppId = int.Parse(Request["docAppId"]);
            string strFileName = "Income_Extraction_Worksheet.pdf";
            MemoryStream pdfStream = HousingGrantGenerator.GeneratePDFHousingGrant(docAppId.Value);

            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppIncomeExtractionById(docAppId.Value);
            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                strFileName = string.Format("{0}_{1}_{2}", docAppRow.RefNo, DateTime.Now.ToShortDateString(), strFileName);
            }

            if (pdfStream != null)
            {
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", string.Format("inline;filename={0}", strFileName));
                Response.OutputStream.Write(pdfStream.GetBuffer(), 0, pdfStream.GetBuffer().Length);
            }
            else
            {
                Response.Write("Error generating PDF letter.");
            }



            //docAppId = int.Parse(Request["docAppId"]);



            //Response.Redirect(string.Format("HousingGrant.ashx?docAppId={0}", docAppId.Value));
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        
    }
}
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Bll;


public partial class Maintenance_TransferData_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        DataTable dtDocDetail;

        SqlCommand command = new SqlCommand();
        StringBuilder sqlStatement = new StringBuilder();

        sqlStatement.Append("Select Id, 'HIEND-SET00'+CONVERT(varchar, SetNo) AS SETNO,DOCTYPE,HLENO,NRIC,PAGENO,'HIEND-SET00'+CONVERT(varchar, SetNo)+'-'+CONVERT(varchar, Id)+'.pdf' AS FileName FROM [vDocDetail] WHERE HLENO>'' ORDER BY HLENO, PAGENO");

        using (SqlConnection connection = new SqlConnection(connString))
        {
            command.CommandText = sqlStatement.ToString();
            command.Connection = connection;
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(dataSet);
            dtDocDetail = dataSet.Tables[0];
        }

        grid1.DataSource = dtDocDetail;
        grid1.DataBind();
        DropDownList1.DataSource = dtDocDetail;
        DropDownList1.DataTextField = "FileName";
        DropDownList1.DataValueField = "Id";
        DropDownList1.DataBind();
    }
    protected void ButtonUpload_Click(object sender, EventArgs e)
    {
        LabelMsg.Text = string.Empty;
        if (DropDownList1.Items.Count == 0)
        {
            LabelMsg.Text = "no file selected.";
            return;
        }
        int id =int.Parse( DropDownList1.SelectedValue.ToString());
        string fileName = DropDownList1.Items[DropDownList1.SelectedIndex].Text;
        LabelMsg.Text = fileName;
        //DocDetailDb db=new DocDetailDb();
        //DocDetail.DocDetailDataTable dt = db.GetDataById(id);
        //DocDetail.DocDetailRow r = dt[0];
        //string br = "<br/>";
        //if (r.IsPdfFileDataNull())
        //{
        //    LabelMsg.Text += br + "file is null";
        //    return;
        //}
        

        //LabelMsg.Text +=br+ "file Name:"+fileName ;
        //LabelMsg.Text += br + "Document Type: " + r.DocType;
        //LabelMsg.Text += br + "HLE No:" + r.HleNo;
        //LabelMsg.Text += br + "NRIC:" + r.Nric;

        //string base64String0 = Convert.ToBase64String(r.PdfFileData) ;

        //ABCDEMF.BP27JCMFilerService mfService = new ABCDEMF.BP27JCMFilerService();
        //ABCDEMF.BP27JInputDocumentDTO inputDocs = new ABCDEMF.BP27JInputDocumentDTO();

        //int totalFile = 1;

        //ABCDEMF.file myfile = new ABCDEMF.file();

        //inputDocs.hleNo = r.HleNo;

        //inputDocs.files = new ABCDEMF.file[totalFile];  // if have multiple files

        //myfile.docType = r.DocType;
        //myfile.fileName = fileName;
        //myfile.nric = r.Nric;
        //myfile.mimeType = "application/pdf";
        //myfile.encodedFileBytes = base64String0;

        //inputDocs.files[0] = myfile;

        //int iReturn = mfService.fileDocument(inputDocs);

        //LabelMsg.Text += br + "Ressult: " + iReturn;

    }
}

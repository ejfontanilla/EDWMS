using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Dwms.Bll;

public partial class Maintenance_StreetTable_ExcelTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string filePath = MapPath(ConfigurationManager.AppSettings["excelFolder"] + "/" + Constants.StreetListExcelFileName);

        HttpContext context = HttpContext.Current;
        context.Response.Clear();

        try
        {
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=" + "StreeCodeTemplate.xls");
            Response.WriteFile(filePath);
            context.Response.End();
        }
        catch (Exception)
        {
        }
    }
}
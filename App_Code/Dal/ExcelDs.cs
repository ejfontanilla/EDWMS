using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class ExcelDs
    {
        public static DataTable GetDataTableFromExcel(string excelFilePath, string sqlSelect)
        {
            string sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                   + excelFilePath
                   + ";Extended Properties='Excel 8.0'";
            OleDbConnection objConn = new OleDbConnection(sConnectionString);
            OleDbCommand objCmdSelect = new OleDbCommand(sqlSelect, objConn);
            OleDbDataAdapter objAdapter = new OleDbDataAdapter();
            objAdapter.SelectCommand = objCmdSelect;
            DataSet ds = new DataSet();
            objAdapter.Fill(ds, "dtTables");
            return ds.Tables[0];
        }
    }
}

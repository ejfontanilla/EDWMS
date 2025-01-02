using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class ExcelDb
    {
        #region Retrieve Methods
        public DataTable GetDataTableFromExcel(string excelFilePath, string sqlSelect)
        {
            return ExcelDs.GetDataTableFromExcel(excelFilePath, sqlSelect);
        }
        #endregion
    }
}
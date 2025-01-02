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
    public class RawFileDs
    {
        public static string GetConnectionString()
        {
            ConnectionStringSettings cts = ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"];
            return cts.ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection objConnection = new SqlConnection(GetConnectionString());
            return objConnection;
        }

        #region Retrieve Methods

        /// <summary>
        /// Get RawPage and respective docApp Details
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDataByDocIds(string docIds)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Id, DocSetId, FileName, FileData ");
            sqlStatement.Append("FROM RawFile ");
            sqlStatement.Append("WHERE (Id IN ");
            sqlStatement.Append("(SELECT RawFileId ");
            sqlStatement.Append("FROM RawPage ");
            sqlStatement.Append("WHERE (DocId IN (" + docIds + "))))");

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                return dataSet.Tables[0];
            }
        }
        #endregion
    }
}

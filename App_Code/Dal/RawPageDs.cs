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
    public class RawPageDs
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
        public static int CountOcrPagesBySet(int docSetId)
        {
            int pageCount = 0;
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT COUNT(*) AS PageCount ");
            sqlStatement.Append("FROM RawPage INNER JOIN RawFile ON RawPage.RawFileId = RawFile.Id ");
            sqlStatement.Append("WHERE RawPage.IsOcr = 1 AND RawFile.DocSetId = @docSetId");

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    pageCount = int.Parse(dataSet.Tables[0].Rows[0]["PageCount"].ToString());
                }
            }

            return pageCount;
        }

        /// <summary>
        /// Get RawPage and respective docApp Details
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetRawPagesBySetId(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * ");
            sqlStatement.Append("FROM RawPage INNER JOIN RawFile ON RawPage.RawFileId = RawFile.Id ");
            sqlStatement.Append("WHERE RawFile.DocSetId = @docSetId");

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

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

        /// <summary>
        /// Get RawPage and respective docApp Details
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static int GetSetIdByRawPageId(int rawPageId)
        {
            int pageCount = 0;
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT RawFile.DocSetId FROM RawFile ");
            sqlStatement.Append("INNER JOIN RawPage ON RawFile.Id = RawPage.RawFileId ");
            sqlStatement.Append("WHERE RawPage.Id = @rawPageId");

            command.Parameters.Add("@rawPageId", SqlDbType.Int);
            command.Parameters["@rawPageId"].Value = rawPageId;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    pageCount = int.Parse(dataSet.Tables[0].Rows[0]["DocSetId"].ToString());
                }
            }

            return pageCount;
        }

        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

        public static DataTable GetRawPagesIdByDocId(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Id AS RawPageId ");
            sqlStatement.Append("FROM RawPage WITH (NOLOCK) ");
            sqlStatement.Append("WHERE (DocId = @DocId) ORDER BY DocPageNo ");

            command.Parameters.Add("@DocId", SqlDbType.Int);
            command.Parameters["@DocId"].Value = docId;

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

        public static DataTable GetYearMonthDayForFolderStructure(int rawPageId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT b.Id AS 'DocId', c.Id AS 'DocSetId', ");
            sqlStatement.Append("DATEPART(YEAR, c.VerificationDateIn) AS 'fYear', DATEPART(MONTH, c.VerificationDateIn) AS 'fMonth', DATEPART(DAY, c.VerificationDateIn) AS 'fDay'");
            sqlStatement.Append("FROM RawPage a INNER JOIN doc b ON a.DocId = b.id INNER JOIN Docset c ON b.DocSetId = c.Id WHERE a.id = @rawPageId");

            command.Parameters.Add("@rawPageId", SqlDbType.Int);
            command.Parameters["@rawPageId"].Value = rawPageId;

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

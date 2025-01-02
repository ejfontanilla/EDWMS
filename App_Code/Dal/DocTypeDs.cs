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

//This class is created by Edward 2015/11/30 To Optimize Leas Web Service
namespace Dwms.Dal
{
    /// <summary>
    /// Summary description for DocTypeDs
    /// </summary>
    public class DocTypeDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();


        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service

        public static string GetDocTypeCodeByDocumentIdAndDocumentSubIdAsNull(string documentId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT ISNULL(Code,'') AS 'DocTypeCode' FROM DocType  ");
            sqlStatement.Append("WHERE (DocumentId = @DocumentId) AND (DocumentSubId = '00' OR DocumentSubId IS NULL)");

            command.Parameters.Add("@DocumentId", SqlDbType.NVarChar);
            command.Parameters["@DocumentId"].Value = documentId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return dataSet.Tables[0].Rows[0]["DocTypeCode"].ToString();
                else
                    return string.Empty;
            }
        }

        public static string GetDocTypeCodeByDocumentIdAndDocumentSubId(string documentId, string documentSubId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT ISNULL(Code,'') AS 'DocTypeCode' FROM DocType  ");
            sqlStatement.Append("WHERE (DocumentId = @documentId) AND (DocumentSubId = @documentSubId)");

            command.Parameters.Add("@DocumentId", SqlDbType.NVarChar);
            command.Parameters["@DocumentId"].Value = documentId;

            command.Parameters.Add("@documentSubId", SqlDbType.NVarChar);
            command.Parameters["@documentSubId"].Value = documentSubId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return dataSet.Tables[0].Rows[0]["DocTypeCode"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion

    }
}
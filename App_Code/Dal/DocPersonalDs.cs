using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class DocPersonalDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods


        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get Documents for a set wih reference to docPersonal by docsetid
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        //public static DataTable GetDocPersonalDocumentByDocSetId(int docSetId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT DocPersonal.Id, DocTypeCode,Nric, DocPersonal.Name,Folder,DocPersonal.DocSetId, doc.Id AS DocId, Doc.Status AS DocStatus, Doc.SendToCDBStatus AS SendStatus, DocType.Description, SetDocRef.Id AS SetDocRefId "); 
        //    sqlStatement.Append("FROM Doc LEFT OUTER JOIN DocType ON ");
        //    sqlStatement.Append("Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN SetDocRef ON ");
        //    sqlStatement.Append("Doc.Id = SetDocRef.DocId LEFT OUTER JOIN DocPersonal ON ");
        //    sqlStatement.Append("SetDocRef.DocPersonalId = DocPersonal.Id WHERE DocPersonal.DocSetId = @docSetId ORDER BY DocType.Description, doc.Id");

        //    command.Parameters.Add("@docSetId", SqlDbType.Int);
        //    command.Parameters["@docSetId"].Value = docSetId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public static DataTable GetDocPersonalDocumentByDocSetId(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocPersonal_GetDocPersonalDocumentByDocSetId";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }
        #endregion
        #endregion
    }
}

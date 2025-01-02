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
    public class AppPersonalDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods


        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get Documents for an application wih reference to appPersonal by setappid
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        //public static DataTable GetAppPersonalDocumentByDocAppIdAndSetId(int docAppId, int docSetId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT AppPersonal.Id, DocTypeCode, Nric,AppPersonal.Name, Folder, DocAppId, Doc.Id AS DocId, AppDocRef.Id AS AppDocRefId, ");
        //    sqlStatement.Append("Doc.Status AS DocStatus, Doc.SendToCDBStatus AS SendStatus, DocType.Description, CASE Folder WHEN 'Others' THEN 1 ELSE 0 END AS FolderOrder, ");
        //    sqlStatement.Append("CASE WHEN Nric IS NULL OR LTRIM(RTRIM(Nric))='' THEN 0 ELSE 1 END AS NricOrder FROM Doc LEFT OUTER JOIN DocType ON ");
        //    sqlStatement.Append("Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN AppDocRef ON ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId LEFT OUTER JOIN AppPersonal ON ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id WHERE AppPersonal.DocAppId = @docAppId AND Doc.DocSetId=@docSetId ORDER BY FolderOrder, NricOrder, DocType.Description, Doc.Id");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

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

        public static DataTable GetAppPersonalDocumentByDocAppIdAndSetId(int docAppId, int docSetId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AppPersonal_GetAppPersonalDocumentByDocAppIdAndSetId";
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

        /// <summary>
        /// Get DocAppId from AppPersonal by AppDocRefId
        /// </summary>
        /// <param name="appDocRefId"></param>
        /// <returns></returns>
        public static int GetDocAppIdByAppDocRefId(int appDocRefId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT TOP 1 DocAppId FROM AppPersonal WHERE Id IN (SELECT AppPersonalId FROM AppDocRef WHERE Id=@appDocRefId)");

            command.Parameters.Add("@appDocRefId", SqlDbType.Int);
            command.Parameters["@appDocRefId"].Value = appDocRefId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                try
                {
                    return int.Parse(dataSet.Tables[0].Rows[0]["DocAppId"].ToString());
                }
                catch
                {
                    return -1;
                }
            }
        }


        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get Documents for an application wih reference to appPersonal in all the verified sets
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        //public static DataTable GetAppPersonalDocumentByDocAppId(int docAppId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT AppPersonal.Id, DocTypeCode, Nric,AppPersonal.Name, Folder, DocAppId, Doc.Id AS DocId, Doc.DocSetId, AppDocRef.Id AS AppDocRefId, ");
        //    sqlStatement.Append("Doc.Status AS DocStatus, Doc.SendToCDBStatus AS SendStatus, Doc.SendToCDBAccept AS SendAccept, DocType.Description, CASE Folder WHEN 'Others' THEN 1 ELSE 0 END AS FolderOrder, ");
        //    sqlStatement.Append("CASE WHEN Nric IS NULL OR LTRIM(RTRIM(Nric))='' THEN 0 ELSE 1 END AS NricOrder, ");
        //    sqlStatement.Append("(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN FieldValue ELSE NULL END  FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%')) AS StartDate, ");
        //    sqlStatement.Append("(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN FieldValue ELSE NULL END  FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%')) AS EndDate, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(dbo.FN_GETCONVERTEDDATE(FieldValue)) = 1 THEN CONVERT(datetime,dbo.FN_GETCONVERTEDDATE(FieldValue))  ELSE NULL END  FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%')),105) AS StartDateFormatted, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN CONVERT(datetime,FieldValue) ELSE NULL END  FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%')),105) AS EndDateFormatted ");
        //    sqlStatement.Append("FROM Doc LEFT OUTER JOIN DocType ON ");
        //    sqlStatement.Append("Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN AppDocRef ON ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId LEFT OUTER JOIN AppPersonal ON ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id WHERE AppPersonal.DocAppId = @docAppId ");
        //    sqlStatement.Append("AND Doc.DocSetId in (select DocSetId from SetApp where DocAppId=@docAppId and DocSetId in (select id from DocSet where Status='Verified')) ");
        //    sqlStatement.Append("ORDER BY FolderOrder, NricOrder, DocType.Description, StartDateFormatted, Doc.Id");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

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

        public static DataTable GetAppPersonalDocumentByDocAppId(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AppPersonal_GetAppPersonalDocumentByDocAppId";
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


        /// <summary>
        /// Gets the apppersonal records with ref no details
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static DataTable GetAppPersonalReferenceDetailsByDocId(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT TOP 1 RefNo, RefType FROM Doc INNER JOIN AppDocRef ON ");
            sqlStatement.Append("Doc.Id = AppDocRef.DocId INNER JOIN AppPersonal ON ");
            sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN DocApp ON ");
            sqlStatement.Append("AppPersonal.DocAppId = DocApp.Id ");
            sqlStatement.Append("WHERE Doc.Id = @docId");

            command.Parameters.Add("@docId", SqlDbType.Int);
            command.Parameters["@docId"].Value = docId;

            using (SqlConnection connection = new SqlConnection(connString))
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


        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get Documents for an application wih reference to appPersonal in all the verified sets
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        //public static DataTable GetAppPersonalDocumentByDocAppIdAndAppPersonalId(int docAppId, int appPersonalId) //Added by Edward 15.07.2013
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    //sqlStatement.Append("SELECT Doc.Id, DocTypeCode, Status, IsAccepted, IsVerified, Doctype.Description, MetaData.FieldValue AS 'EndDate', CONVERT(DATETIME,FieldValue,105) ");
        //    sqlStatement.Append("SELECT Doc.Id, DocTypeCode, Status, IsAccepted, IsVerified, Doctype.Description, MetaData.FieldValue AS 'EndDate' ");
        //    //sqlStatement.Append(",CASE WHEN DocTypeCode = 'PAYSLIP' THEN 100 WHEN DocTypeCode LIKE '%CPF%' THEN -1 ELSE 99 END ");  //Added By Edward 27/2/2014 February 21, 2014 Meeting : Less Priority in getting CPF Enddate
        //    sqlStatement.Append(",CASE WHEN DocTypeCode = 'PAYSLIP' THEN 100 ELSE 99 END ");  //Added by Edward ignore Cpf 2014/5/21
        //    sqlStatement.Append("FROM Doc LEFT OUTER JOIN DocType ON ");
        //    sqlStatement.Append("Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN AppDocRef ON ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId LEFT OUTER JOIN AppPersonal ON ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id LEFT OUTER JOIN MetaData ON ");
        //    sqlStatement.Append("Metadata.Doc = Doc.Id WHERE AppPersonal.DocAppId = @docAppId AND AppPersonal.Id = @appPersonalId ");
        //    sqlStatement.Append("AND Doc.DocSetId in (select DocSetId from SetApp where DocAppId=@docAppId and DocSetId in (select id from DocSet where Status='Verified')) ");
        //    //sqlStatement.Append("AND MetaData.FieldName LIKE 'EndDate' AND Doctype.DocumentId <> '000028' "); //Added Dont include IRASAssessment
        //    sqlStatement.Append("AND MetaData.FieldName LIKE 'EndDate' AND Doctype.DocumentId NOT IN ('000028', '000025') ");  // 28/2/2014 Enddate is NA cannot convert to date - Not Include Employment Letter
        //    sqlStatement.Append("AND DocTypeCode NOT LIKE '%CPF%'  "); //Added by Edward ignore Cpf 2014/5/21
        //    #region Added by Edward 21/01/2014 to Add IRAS Notice of Assessment
        //    sqlStatement.Append("UNION ");
        //    //sqlStatement.Append("SELECT Doc.Id, DocTypeCode, Status, IsAccepted, IsVerified, Doctype.Description, MetaData.FieldValue AS 'EndDate', CONVERT(DATETIME,FieldValue,105) ");
        //    sqlStatement.Append("SELECT Doc.Id, DocTypeCode, Status, IsAccepted, IsVerified, Doctype.Description, MetaData.FieldValue AS 'EndDate' ");
        //    sqlStatement.Append(",99 ");  //Added By Edward 27/2/2014 February 21, 2014 Meeting : Less Priority in getting CPF Enddate
        //    sqlStatement.Append("FROM Doc LEFT OUTER JOIN DocType ON ");
        //    sqlStatement.Append("Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN AppDocRef ON ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId LEFT OUTER JOIN AppPersonal ON ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id LEFT OUTER JOIN MetaData ON ");
        //    sqlStatement.Append("Metadata.Doc = Doc.Id WHERE AppPersonal.DocAppId = @docAppId AND AppPersonal.Id = @appPersonalId ");
        //    sqlStatement.Append("AND Doc.DocSetId in (select DocSetId from SetApp where DocAppId=@docAppId and DocSetId in (select id from DocSet where Status='Verified')) ");
        //    sqlStatement.Append("AND MetaData.FieldName LIKE 'DateOfFiling' AND DocType.DocumentId = '000028' ");
        //    #endregion

        //    //sqlStatement.Append(" ORDER BY CONVERT(DATETIME,FieldValue,105) DESC  "); //Commented and modified By Edward 27/2/2014 February 21, 2014 Meeting : Less Priority in getting CPF Enddate
        //    //sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode = 'PAYSLIP' THEN 100 WHEN DocTypeCode LIKE '%CPF%' THEN -1 ELSE 99 END DESC ,CONVERT(DATETIME,FieldValue,105) DESC ");
        //    //sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode = 'PAYSLIP' THEN 100 WHEN DocTypeCode LIKE '%CPF%' THEN -1 ELSE 99 END DESC ");
        //    sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode = 'PAYSLIP' THEN 100 ELSE 99 END DESC ");//Added by Edward ignore Cpf 2014/5/21
                       
        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@appPersonalId", SqlDbType.Int);
        //    command.Parameters["@appPersonalId"].Value = appPersonalId;

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

        public static DataTable GetAppPersonalDocumentByDocAppIdAndAppPersonalId(int docAppId, int appPersonalId) //Added by Edward 15.07.2013
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;
            command.Parameters.Add("@appPersonalId", SqlDbType.Int);
            command.Parameters["@appPersonalId"].Value = appPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "AppPersonal_GetAppPersonalDocumentByDocAppIdAndAppPersonalId";
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

        //Added By Edward 14.11.2013 For Income Extraction
        public static bool CheckAssessmentNotApplicable(string nric, int docAppId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = string.Empty;

            sqlStatement = "SELECT EmploymentType, AssessmentNA FROM AppPersonal WHERE Nric = @nric AND DocAppId = @docAppId ";

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                bool CanConfirm;
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                if (dataSet.Tables[0].Rows[0]["AssessmentNA"].ToString().ToUpper().Equals("UNEMPLOYED"))
                    CanConfirm = !string.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["AssessmentNA"].ToString()) ?
                        bool.Parse(dataSet.Tables[0].Rows[0]["AssessmentNA"].ToString()) : false;
                else
                    CanConfirm = false;

                return CanConfirm;
            }
        }


        #endregion

        /// <summary>
        /// Delete Orphan Records
        /// </summary>
        internal static void DeleteOrphanRecords()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM AppPersonal WHERE (Nric IS NULL OR Nric ='') AND (PersonalType IS NULL OR PersonalType = '')" );
            sqlStatement.Append("AND (Relationship IS NULL OR Relationship = '')" );
            sqlStatement.Append("AND Folder='Unidentified' AND Id NOT IN (SELECT AppPersonalId FROM AppDocRef)");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

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
    public class DocDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        /// <summary>
        /// Get Docs for treeview in Verification
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDocForTreeView(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT * FROM vDoc WHERE DocSetId=@docSetId ORDER BY PersonalType, Id, Description");

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

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

        /// <summary>
        /// Get distinct CmDocumentId from docSet for dropdown display
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCmDocumentIdForDropDown()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT(CmDocumentId) FROM Doc WHERE CmDocumentId IS NOT NULL AND LEN(CmDocumentId)>0 ORDER BY CmDocumentId");

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

        /// <summary>
        /// Get Documents by nric. search nric in both docpersonal and apppersonal
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDocByNric(string nric, DateTime? dateInFrom, DateTime? dateInTo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT Doc.Id, DocTypeCode, DocSet.SetNo, DocSet.Status, DocSet.VerificationDateIn, Doc.DocSetId FROM Doc ");
            sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId  ");
            sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id  ");
            sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric = @nric1  ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT DISTINCT Doc.Id, DocTypeCode, DocSet.SetNo, DocSet.Status, DocSet.VerificationDateIn, Doc.DocSetId FROM Doc  ");
            sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
            sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric=@nric2");

            command.Parameters.Add("@nric1", SqlDbType.VarChar);
            command.Parameters["@nric1"].Value = nric;

            command.Parameters.Add("@nric2", SqlDbType.VarChar);
            command.Parameters["@nric2"].Value = nric;

            // Add the date in and out query
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn = @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn = @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

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
        

        /// <summary>
        /// Get Docs for treeview in Completeness
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public static DataTable GetDocForTreeViewCompleteness(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT * FROM vDoc WHERE DocAppId=@docAppId AND DocSetStatus='Verified' ORDER BY PersonalType, Description");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

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
        /// get first level folderid
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        //public static DataTable GetParentFolderForTreeView(int docSetId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT * FROM (");
        //    sqlStatement.Append("SELECT TOP 1 code as FolderId, name AS FolderName, 1 AS FolderOrder, 'UNIDENTIFIED' AS TreeCategory, '' as RefType, '' as Refno FROM Docfolder WHERE FolderOrder<=1 ");
        //    sqlStatement.Append("UNION ");
        //    sqlStatement.Append("SELECT DISTINCT LTRIM(RTRIM(SetApp.DocAppId)) AS FolderId, RefType + ': ' + RefNo AS FolderName, 2 AS FolderOrder, 'REFNO' AS treeCategory, RefType, RefNo FROM DocSet INNER JOIN SetApp ON DocSet.Id = SetApp.DocSetId INNER JOIN DocApp ON SetApp.DocAppId = DocApp.Id WHERE DocSet.Id = @docSetId ");
        //    sqlStatement.Append("UNION ");
        //    sqlStatement.Append("SELECT code as FolderId, name AS FolderName, FolderOrder+200, 'DEFAULT FOLDER' AS treeCategory, '' as RefType, '' as Refno FROM DocFolder WHERE FolderOrder>1 and Name <> 'Others' ");
        //    sqlStatement.Append(") tbl ORDER BY FolderOrder");

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
        public static DataTable GetParentFolderForTreeView(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Doc_GetParentFolderForTreeView";
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
        /// get first level folderid for completeness 
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        public static DataTable GetParentFolderForTreeView_Completeness(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM (");
            sqlStatement.Append("SELECT Code AS FolderId, Name AS FolderName, 'DEFAULT FOLDER' AS TreeCategory, 2 AS FolderOrder FROM DocFolder WHERE Code='OT'");
            sqlStatement.Append(") tbl WHERE (FolderId <> '' OR FolderId IS NULL) ORDER BY FolderOrder");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

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
        /// Get the submitted documents for the user
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public static DataTable GetDocForSummary(int docAppId, string nric, bool isHa, string referenceType)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT DISTINCT Doc.*, (SELECT TOP 1 Description FROM DocType WHERE Code = Doc.DocTypeCode) AS DocTypeDescription, ");

        //    #region Modified By Edward To Get Start Date first before the other dates 2014/6/26
        //    //sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%') ORDER BY ModifiedDate DESC) As StartDate, ");
        //    //sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(dbo.FN_GETCONVERTEDDATE(FieldValue)) = 1 THEN CONVERT(datetime,dbo.FN_GETCONVERTEDDATE(FieldValue)) ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%') ORDER BY ModifiedDate DESC),105) As StartDateFormatted, ");
        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%')  ");
        //    sqlStatement.Append("ORDER BY CASE WHEN FieldName LIKE 'StartDate%' THEN 1 WHEN FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%' THEN 2 END) As StartDate, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(dbo.FN_GETCONVERTEDDATE(FieldValue)) = 1 THEN CONVERT(datetime,dbo.FN_GETCONVERTEDDATE(FieldValue)) ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%') ");
        //    sqlStatement.Append("ORDER BY CASE WHEN FieldName LIKE 'StartDate%' THEN 1 WHEN FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%' THEN 2 END),105) As StartDateFormatted,  ");
        //    #endregion

        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%') ORDER BY ModifiedDate DESC) As EndDate, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN CONVERT(datetime,FieldValue) ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%') ORDER BY ModifiedDate DESC),105) As EndDateFormatted, ");
        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Company%' AND FieldName LIKE '%2%')) As CompanyName ");
        //    sqlStatement.Append(", DocOrder = CASE WHEN DocTypeCode=@ReferenceType THEN 0 ELSE 1 END ");
        //    sqlStatement.Append(", AppPersonal.Nric, AppDocRef.Id AS AppDocRefID, AppPersonal.Id AS AppPersonalId    "); //Added by Edward 16.10.2013  //Added AppPersonal.Id As AppPersonalId
        //    sqlStatement.Append("FROM Doc INNER JOIN AppDocRef ON  ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId INNER JOIN AppPersonal ON  ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN SetApp ON ");
        //    sqlStatement.Append("AppPersonal.DocAppId = SetApp.DocAppId INNER JOIN DocSet ON ");
        //    sqlStatement.Append("SetApp.DocSetId = DocSet.id ");
        //    sqlStatement.Append("AND Doc.DocSetId = DocSet.id ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId=@docAppId  ");
        //    sqlStatement.Append("AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@nric))  ");
        //    sqlStatement.Append("and DocSet.Status='Verified' ");

        //    //sqlStatement.Append("SELECT DISTINCT Doc.*, (SELECT TOP 1 Description FROM DocType WHERE Code = Doc.DocTypeCode) AS DocTypeDescription, ");
        //    //sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%')) As StartDate, ");
        //    //sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN FieldValue ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%'))) As StartDateForOrder, ");
        //    //sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%')) As EndDate, ");
        //    //sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN FieldValue ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%'))) As EndDateForORder, ");
        //    //sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Company%' AND FieldName LIKE '%2%')) As CompanyName ");
        //    //sqlStatement.Append(", DocOrder = CASE WHEN DocTypeCode=@ReferenceType THEN 0 ELSE 1 END ");
        //    //sqlStatement.Append("FROM Doc INNER JOIN AppDocRef ON  ");
        //    //sqlStatement.Append("Doc.Id = AppDocRef.DocId INNER JOIN AppPersonal ON  ");
        //    //sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN SetApp ON ");
        //    //sqlStatement.Append("AppPersonal.DocAppId = SetApp.DocAppId INNER JOIN DocSet ON ");
        //    //sqlStatement.Append("SetApp.DocSetId = DocSet.id ");
        //    //sqlStatement.Append("AND Doc.DocSetId = DocSet.id ");
        //    //sqlStatement.Append("WHERE AppPersonal.DocAppId=@docAppId  ");
        //    //sqlStatement.Append("AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@nric))  ");
        //    //sqlStatement.Append("and DocSet.Status='Verified' ");

        //    if (!isHa)
        //    {
        //        sqlStatement.Append("AND Doc.DocTypeCode <> @ReferenceType ");
        //    }

        //    sqlStatement.Append("ORDER BY DocOrder, DocTypeDescription, StartDateFormatted, Doc.Id");

        //    command.Parameters.Add("@ReferenceType", SqlDbType.VarChar);
        //    command.Parameters["@ReferenceType"].Value = referenceType;

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

        //    command.Parameters.Add("@Status", SqlDbType.VarChar);
        //    command.Parameters["@Status"].Value = SetStatusEnum.Verified.ToString();

        //    //System.Web.HttpContext.Current.Response.Write(sqlStatement.ToString());
        //    //System.Web.HttpContext.Current.Response.End();

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

        public static DataTable GetDocForSummary(int docAppId, string nric, bool isHa, string referenceType)
        {
            SqlCommand command = new SqlCommand();
            if (!isHa)
            {
                command.Parameters.Add("@ReferenceType", SqlDbType.VarChar);
                command.Parameters["@ReferenceType"].Value = referenceType;
            }
            
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = SetStatusEnum.Verified.ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Doc_GetDocForSummary";
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
        /// Get Nric for dropdownlist
        /// </summary>
        /// <returns></returns>
        public static DataTable GetNricForDropDownListOLD()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM ");
            sqlStatement.Append("( ");
            sqlStatement.Append("SELECT Nric FROM DocPersonal WHERE Nric IS NOT NULL AND Nric <> '' AND len(LTRIM(RTRIM(Nric))) >0  ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT Nric FROM AppPersonal WHERE Nric IS NOT NULL AND Nric <> '' AND len(LTRIM(RTRIM(Nric))) >0  ");
            sqlStatement.Append(") Tbl ORDER BY Nric");

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
        #region Modified by Edward 2017/10/30  to convert to SP
        public static DataTable GetNricForDropDownList()
        {
            SqlCommand command = new SqlCommand();            
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Verification_GetNRIC";
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
        /// Get nric for completeness dropdown. since completeness only have records in appPersonal, exclude the nric in docPersonal
        /// </summary>
        /// <returns></returns>
        public static DataTable GetNricForCompletenessDropDownList()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Nric FROM AppPersonal WHERE Nric IS NOT NULL AND Nric <> '' AND len(LTRIM(RTRIM(Nric))) >0   ORDER BY Nric");

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

        #endregion

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM

        #region Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
        //public static DataTable GetDocForSummaryIncomeExtraction(int docAppId, string nric, bool isHa, string referenceType)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT DISTINCT Doc.*, (SELECT TOP 1 Description FROM DocType WHERE Code = Doc.DocTypeCode) AS DocTypeDescription, ");

        //    #region Modified By Edward To Get Start Date first before the other dates 2014/6/26
        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%')  ");
        //    sqlStatement.Append("ORDER BY CASE WHEN FieldName LIKE 'StartDate%' THEN 1 WHEN FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%' THEN 2 END) As StartDate, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(dbo.FN_GETCONVERTEDDATE(FieldValue)) = 1 THEN CONVERT(datetime,dbo.FN_GETCONVERTEDDATE(FieldValue)) ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%') ");
        //    sqlStatement.Append("ORDER BY CASE WHEN FieldName LIKE 'StartDate%' THEN 1 WHEN FieldName LIKE '%Date%' AND FieldName NOT LIKE '%End%' THEN 2 END),105) As StartDateFormatted,  ");
        //    #endregion

        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%') ORDER BY ModifiedDate DESC) As EndDate, ");
        //    sqlStatement.Append("CONVERT(datetime,(SELECT TOP 1 CASE WHEN ISDATE(FieldValue) = 1 THEN CONVERT(datetime,FieldValue) ELSE NULL END FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Date%' AND FieldName LIKE '%End%') ORDER BY ModifiedDate DESC),105) As EndDateFormatted, ");
        //    sqlStatement.Append("(SELECT TOP 1 FieldValue FROM MetaData WHERE Doc = Doc.Id AND (FieldName LIKE '%Company%' AND FieldName LIKE '%2%')) As CompanyName ");
        //    sqlStatement.Append(", DocOrder = CASE WHEN DocTypeCode=@ReferenceType THEN 0 ELSE 1 END ");
        //    sqlStatement.Append(", AppPersonal.Nric, AppDocRef.Id AS AppDocRefID, AppPersonal.Id AS AppPersonalId    "); //Added by Edward 16.10.2013  //Added AppPersonal.Id As AppPersonalId
        //    sqlStatement.Append("FROM Doc INNER JOIN AppDocRef ON  ");
        //    sqlStatement.Append("Doc.Id = AppDocRef.DocId INNER JOIN AppPersonal ON  ");
        //    sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN SetApp ON ");
        //    sqlStatement.Append("AppPersonal.DocAppId = SetApp.DocAppId INNER JOIN DocSet ON ");
        //    sqlStatement.Append("SetApp.DocSetId = DocSet.id ");
        //    sqlStatement.Append("AND Doc.DocSetId = DocSet.id ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId=@docAppId  ");
        //    sqlStatement.Append("AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@nric))  ");
        //    sqlStatement.Append("and DocSet.Status='Verified' ");

        //    if (!isHa)
        //    {
        //        sqlStatement.Append("AND Doc.DocTypeCode <> @ReferenceType ");
        //    }

        //    sqlStatement.Append("ORDER BY DocOrder, DocTypeDescription, StartDateFormatted, Doc.Id");

        //    command.Parameters.Add("@ReferenceType", SqlDbType.VarChar);
        //    command.Parameters["@ReferenceType"].Value = referenceType;

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

        //    command.Parameters.Add("@Status", SqlDbType.VarChar);
        //    command.Parameters["@Status"].Value = SetStatusEnum.Verified.ToString();

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

        public static DataTable GetDocForSummaryIncomeExtraction(int docAppId, string nric, bool isHa, string referenceType)
        {
            SqlCommand command = new SqlCommand();
            if (!isHa)
            {
                command.Parameters.Add("@ReferenceType", SqlDbType.VarChar);
                command.Parameters["@ReferenceType"].Value = referenceType;
            }

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = SetStatusEnum.Verified.ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Doc_GetDocForSummaryIncomeExtraction";
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


        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

        public static DataTable GetDocSetIdAndDateByDocId(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DocSetId, DATEPART(YEAR,VerificationDateIn) AS 'fYear',  DATEPART(MONTH,VerificationDateIn) AS 'fMonth', DATEPART(DAY,VerificationDateIn) AS 'fday' FROM Doc a ");
            sqlStatement.Append("INNER JOIN DocSet b ON a.DocSetId = b.Id WHERE a.Id = @docId ");

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

        #endregion

        #region Added by Edward Separate the Treeview Node Drop to modules 2017/11/14
        public static bool UpdateDocFromNodeDrop(int docId)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@DocId", SqlDbType.Int);
            command.Parameters["@DocId"].Value = docId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "VerificationView_MergeDocsOnNodeDrop";
                command.Connection = connection;
                connection.Open();
                var result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion
    }
}

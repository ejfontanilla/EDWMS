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
    public class DocAppDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private static void ExecuteNonQuery(string queryString)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        #region Retrieve Methods

        /// <summary>
        /// Get User DocApp for Section Change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetDocAppByUserIdForSectionChange(Guid userId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM DocApp WHERE CompletenessStaffUserId = @userId AND (Status=@status1 OR Status=@status2) ");

            command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
            command.Parameters.Add("@status1", SqlDbType.NChar);
            command.Parameters.Add("@status2", SqlDbType.NChar);
            command.Parameters["@userId"].Value = userId;
            command.Parameters["@status1"].Value = AppStatusEnum.Completeness_In_Progress.ToString();
            command.Parameters["@status2"].Value = AppStatusEnum.Pending_Completeness.ToString();

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

        public static DataTable GetDocAppForDropDown()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT - 1 AS Id, 'Unidentified' AS RefNo, 'Unidentified' AS RefType FROM DocApp ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT Id, RefNo, RefType FROM DocApp");

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
        /// Get not verified files for a checked application. checks accross the verified sets only.
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public static int GetNotVerifiedFilesForCheckedApplicationsCount(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM Doc INNER JOIN AppDocRef ON ");
            sqlStatement.Append("Doc.Id = AppDocRef.DocId INNER JOIN AppPersonal ON ");
            sqlStatement.Append("AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN DocApp ON ");
            sqlStatement.Append("AppPersonal.DocAppId = DocApp.Id  ");
            sqlStatement.Append("WHERE AppPersonal.DocAppId = @DocAppId ");
            sqlStatement.Append("AND Doc.DocSetId IN (SELECT Id FROM vDocSet WHERE DocAppId= @DocAppId AND Status=@SetStatus) ");
            sqlStatement.Append("AND Doc.Status <> @DocStatus ");
            sqlStatement.Append("AND LTRIM(RTRIM(DocApp.Status)) = @AppStatus1 AND LTRIM(RTRIM(DocApp.Status)) = @AppStatus2");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters.Add("@AppStatus1", SqlDbType.NChar);
            command.Parameters.Add("@AppStatus2", SqlDbType.NChar);
            command.Parameters.Add("@SetStatus", SqlDbType.NChar);
            command.Parameters.Add("@DocStatus", SqlDbType.NChar);
            command.Parameters["@DocAppId"].Value = docAppId;
            command.Parameters["@AppStatus1"].Value = AppStatusEnum.Completeness_Checked.ToString();
            command.Parameters["@AppStatus2"].Value = AppStatusEnum.Completeness_In_Progress.ToString();
            command.Parameters["@SetStatus"].Value = SetStatusEnum.Verified.ToString();
            command.Parameters["@DocStatus"].Value = DocStatusEnum.Completed.ToString();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0].Rows.Count;
            }
        }        

        public static DataTable GetStatusForDropDown(Guid userId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM DocApp WHERE CompletenessStaffUserId = @userId AND (Status=@status1 OR Status=@status2) ");

            command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
            command.Parameters.Add("@status1", SqlDbType.NChar);
            command.Parameters.Add("@status2", SqlDbType.NChar);
            command.Parameters["@userId"].Value = userId;
            command.Parameters["@status1"].Value = AppStatusEnum.Completeness_In_Progress.ToString();
            command.Parameters["@status2"].Value = AppStatusEnum.Pending_Completeness.ToString();

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
        /// Get Doc App Id of latest set for NRIC
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetRefNosForNric(string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DocApp.RefNo FROM DocApp ");
            sqlStatement.Append("WHERE DocApp.RefNo IN (SELECT HleNumber FROM HleInterface WHERE Nric=@nric) OR ");
            sqlStatement.Append("DocApp.RefNo IN (SELECT CaseNo FROM ResaleInterface WHERE Nric=@nric) OR ");
            sqlStatement.Append("DocApp.RefNo IN (SELECT SchAcc FROM SersInterface WHERE Nric=@nric)");

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

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

        public static DataTable GetCompletenessOfficerCurrentCases(Guid userId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND CompletenessStaffUserId = @userId AND Status NOT IN ('Pending_Documents', 'Completeness_Checked')) AS Total");

            command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
            command.Parameters["@userId"].Value = userId;

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

        public static DataTable GetCompletenessCurrentCasesTotal()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked') AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE RefType = 'HLE' AND Status NOT IN ('Pending_Documents', 'Completeness_Checked')) AS Total");

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
        /// Get pending assignment HLE applications count
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        #region Commented by Edward 2015/02/10
        //public static DataTable GetPendingHleCounts(DateTime dateIn, string status)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
        //    //Added by Edward 13/1/2014 ofr Batch Assignment Panel
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");
            
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) = CONVERT(DATE, @dateIn))) AS Total");


        //    command.Parameters.Add("@dateIn", SqlDbType.DateTime);
        //    command.Parameters["@dateIn"].Value = dateIn;

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

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
        #endregion

        // Added by Edward 2015/02/10 adding risk field and using stored procedure
        public static DataTable GetPendingHleCounts(DateTime dateIn, string status)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@DateIn", SqlDbType.DateTime);
            command.Parameters["@DateIn"].Value = dateIn;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Completeness_BatchAssignment_GetPendingHleCounts";
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

        /// <summary>
        /// Get pending assignment HLE applications count for Income Assessment //Added by Edward 05.08.2013
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static DataTable GetPendingHleIncomeAssessmentCounts(DateTime dateIn, string status)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  ");
            sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @dateIn))) AS Total");


            command.Parameters.Add("@dateIn", SqlDbType.DateTime);
            command.Parameters["@dateIn"].Value = dateIn;

            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

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
        /// Get pending assignment HLE applications count by lane
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        //public static DataTable GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT COUNT(*) AS LaneCount FROM DocApp WHERE RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = @lane AND (Risk <> 'L' OR Risk IS NULL)");

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@lane", SqlDbType.VarChar);
        //    command.Parameters["@lane"].Value = lane.ToString();

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

        public static DataTable GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

            command.Parameters.Add("@lane", SqlDbType.VarChar);
            command.Parameters["@lane"].Value = lane.ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetPendingHleCountsByLanes";
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

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get pending assignment HLE applications count by lane for Income Assessment Added by Edward 12.08.2013
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        //public static DataTable GetPendingHleCountsByLanesIA(string status, HleLanesEnum lane)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT COUNT(*) AS LaneCount FROM DocApp WHERE RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = @lane");

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@lane", SqlDbType.VarChar);
        //    command.Parameters["@lane"].Value = lane.ToString();

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

        public static DataTable GetPendingHleCountsByLanesIA(string status, HleLanesEnum lane)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

            command.Parameters.Add("@lane", SqlDbType.VarChar);
            command.Parameters["@lane"].Value = lane.ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetPendingHleCountsByLanesIA";
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
        /// Get pending assignment HLE applications no pending doc
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        #region Commented by Edward 2015/02/10
        //public static DataTable GetPendingHleNoPendingDoc(string status)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
        //    //Added by Edward 13/1/2014 ofr Batch Assignment Panel  
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status ) AS Total");


        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

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
        #endregion

        // Added by Edward 2015/02/10 adding risk field and using stored procedure
        public static DataTable GetPendingHleNoPendingDoc(string status)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Completeness_BatchAssignment_GetPendingHleNoPendingDoc";
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
        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get pending assignment HLE applications no pending doc for Income Assessment //Added by Edward 08.06.2013
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        //public static DataTable GetPendingHleNoPendingDocIncomeAssessment(string status)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 0 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND HasPendingDoc = 1 AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status ) AS Total");


        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

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

        public static DataTable GetPendingHleNoPendingDocIncomeAssessment(string status)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetPendingHleNoPendingDocIncomeAssessment";
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
        /// Get pending assignment HLE applications less than limi count
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        #region Commented by Edward 2015/02/10
        //public static DataTable GetPendingHleGreaterThanLimitCounts(DateTime dateIn, string status)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
        //    //Added by Edward 13/1/2014 ofr Batch Assignment Panel   
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE CompletenessStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND Status = @status AND (CONVERT(DATE, DateIn) < CONVERT(DATE, @dateIn))) AS Total");


        //    command.Parameters.Add("@dateIn", SqlDbType.DateTime);
        //    command.Parameters["@dateIn"].Value = dateIn;

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

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
        #endregion

        // Added by Edward 2015/02/10 adding risk field and using stored procedure
        public static DataTable GetPendingHleGreaterThanLimitCounts(DateTime dateIn, string status)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@DateIn", SqlDbType.DateTime);
            command.Parameters["@DateIn"].Value = dateIn;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Completeness_BatchAssignment_GetPendingHleGreaterThanLimitCounts";
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

        /// <summary>
        /// Get pending assignment HLE applications less than limit count for Income Assessment //added by Edward 06.08.2013
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static DataTable GetPendingHleGreaterThanLimitCountsIA(DateTime dateIn, string status)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  ");
            sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'A') AS LaneACount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'B') AS LaneBCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'C') AS LaneCCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'D') AS LaneDCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'E') AS LaneECount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'F') AS LaneFCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'H') AS LaneHCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'L') AS LaneLCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'N') AS LaneNCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'T') AS LaneTCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn)) AND SUBSTRING(RefNo, 4, 1) = 'X') AS LaneXCount, ");
            sqlStatement.Append("(SELECT COUNT(*) FROM DocApp WHERE AssessmentStaffUserId IS NULL AND ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND AssessmentStatus = @status AND (CONVERT(DATE, AssessmentDateIn) < CONVERT(DATE, @dateIn))) AS Total");


            command.Parameters.Add("@dateIn", SqlDbType.DateTime);
            command.Parameters["@dateIn"].Value = dateIn;

            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

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

        public static DataTable GetPendingAssignmentApps(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            
            //Added Top 500 by Edward 
            //sqlStatement.Append("SELECT * ");
            sqlStatement.Append("SELECT TOP 500 Id, RefNo, RefType, DateIn, DateOut, Status, CompletenessStaffUserId, CaseOIC,  PeOIC ");            
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (Status=@Status)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;
            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            //Added By Edward 08.10.2013
            sqlStatement.Append(" ORDER BY DateIn DESC ");

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
        /// Get pending assignments set by verification date in
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentSetsByVerificationDateIn(DateTime verificationDateIn, int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE  (CONVERT(DATE, DateIn) = CONVERT(DATE, @VerificationDateIn)) AND ");
            sqlStatement.Append("Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (Status=@Status)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = verificationDateIn;

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
        /// Get DocApp By address. the address is searched accross sersinterface table
        /// </summary>
        /// <param name="block"></param>
        /// <param name="streetName"></param>
        /// <param name="floor"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static DataTable GetDocAppForDropDownByAddress(string block, string streetName, string level, string unit)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM DocApp WHERE RefNo IN ");
            sqlStatement.Append("(SELECT SchAcc FROM SersInterface WHERE 1=1 ");

            if (!string.IsNullOrEmpty(streetName))
            {
                sqlStatement.Append("AND Street LIKE '%' + @Street + '%' ");

                command.Parameters.Add("@Street", SqlDbType.VarChar);
                command.Parameters["@Street"].Value = streetName;
            }

            if (!string.IsNullOrEmpty(block))
            {
                sqlStatement.Append("AND Block LIKE '%' + @block + '%' ");

                command.Parameters.Add("@block", SqlDbType.VarChar);
                command.Parameters["@block"].Value = block;
            }

            if (!string.IsNullOrEmpty(level))
            {
                sqlStatement.Append("AND Level LIKE '%' + @level + '%' ");

                command.Parameters.Add("@level", SqlDbType.VarChar);
                command.Parameters["@level"].Value = level;
            }

            if (!string.IsNullOrEmpty(unit))
            {
                sqlStatement.Append("AND Unit LIKE '%' + @unit + '%' ");

                command.Parameters.Add("@unit", SqlDbType.VarChar);
                command.Parameters["@unit"].Value = unit;
            }

            sqlStatement.Append(")");

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
        /// Get All Apps verification Dates
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public static DataTable GetAllAppsVerificationDates(AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, DateIn)) AS DateIn FROM DocApp ");
            sqlStatement.Append("WHERE DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocApp.Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND CompletenessStaffUserId=@CompletenessStaffUserId ");

                command.Parameters.Add("@CompletenessStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@CompletenessStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND DateIn IS NOT NULL ");
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
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
        /// getAllApps
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>

        public static DataTable GetAllAppsOLD(AppStatusEnum? status, Guid? verificationOicUserId,
        DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
           
            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;


            if (docAppId != -1)
            {                
                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {               
                command.Parameters.Add("@CompletenessStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@CompletenessStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {                
                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }            

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }            

            if (!string.IsNullOrEmpty(downloadStatus))
            {                
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {                
                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {                
                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (checkHleStatus)
            {
                if (!string.IsNullOrEmpty(hleStatus))
                {
                    command.Parameters.Add("@HleStatus", SqlDbType.VarChar);
                    command.Parameters["@HleStatus"].Value = hleStatus;
                }
                else
                {
                    command.Parameters.Add("@HleStatus", SqlDbType.VarChar);
                    command.Parameters["@HleStatus"].Value = "NA";                    
                }
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Completeness_GetAllApps";
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



        public static DataTable GetAllApps(AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            //Edited by Edward 30.10.2013 Optimizing Listings
            //Added TOP 500 by Edward 29.10.2013
            //sqlStatement.Append("SELECT DocApp.*, Profile.Name as CompletenessOIC FROM DocApp LEFT JOIN Profile ");
            //sqlStatement.Append("SELECT TOP 500 DocApp.*, Profile.Name as CompletenessOIC FROM DocApp LEFT JOIN Profile ");
            //sqlStatement.Append("ON DocApp.CompletenessStaffUserId=Profile.UserId ");
            //sqlStatement.Append("WHERE DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");
            sqlStatement.Append("SELECT TOP 500 * FROM vCompletenessGetAllApps ");
            sqlStatement.Append("WHERE Id IN (SELECT DocAppId FROM vVerificationGetAllSets WHERE SectionId=@SectionId) ");


            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;


            if (docAppId != -1)
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append("AND DocApp.Id=@DocAppId ");
                sqlStatement.Append("AND Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND CompletenessStaffUserId=@CompletenessStaffUserId ");

                command.Parameters.Add("@CompletenessStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@CompletenessStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                //sqlStatement.Append("AND (Status=@Status1 OR Status=@Status2 OR Status=@Status3 OR Status=@Status4 OR Status=@Status5 OR Status=@Status6 OR Status=@Status7) ");

                //command.Parameters.Add("@Status1", SqlDbType.VarChar);
                //command.Parameters.Add("@Status2", SqlDbType.VarChar);
                //command.Parameters.Add("@Status3", SqlDbType.VarChar);
                //command.Parameters.Add("@Status4", SqlDbType.VarChar);
                //command.Parameters.Add("@Status5", SqlDbType.VarChar);
                //command.Parameters.Add("@Status6", SqlDbType.VarChar);
                //command.Parameters.Add("@Status7", SqlDbType.VarChar);
                //command.Parameters["@Status1"].Value = AppStatusEnum.Closed.ToString();
                //command.Parameters["@Status2"].Value = AppStatusEnum.Completeness_Cancelled.ToString();
                //command.Parameters["@Status3"].Value = AppStatusEnum.Completeness_Checked.ToString();
                //command.Parameters["@Status4"].Value = AppStatusEnum.Completeness_In_Progress.ToString();
                //command.Parameters["@Status5"].Value = AppStatusEnum.Pending_Completeness.ToString();
                //command.Parameters["@Status6"].Value = AppStatusEnum.Pending_Documents.ToString();
                //command.Parameters["@Status7"].Value = AppStatusEnum.Verified.ToString();
                sqlStatement.Append("AND Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND DateIn IS NOT NULL ");
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                //Edited by Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                sqlStatement.Append("AND LTRIM(RTRIM(DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append(" AND Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append(" AND Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (checkHleStatus)
            {
                if (!string.IsNullOrEmpty(hleStatus))
                {
                    //Edited By Edward 30.10.2013 Optimizing Listings
                    //sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus=@HleStatus) ");
                    sqlStatement.Append(" AND RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus=@HleStatus) ");
                    command.Parameters.Add("@HleStatus", SqlDbType.VarChar);
                    command.Parameters["@HleStatus"].Value = hleStatus;
                }
                else
                {
                    //Edited By Edward 30.10.2013 Optimizing Listings
                    //sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface) ");
                    sqlStatement.Append(" AND RefNo IN (SELECT HleNumber FROM HleInterface) ");
                    //sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus in (");
                    //sqlStatement.Append("'" + HleStatusEnum.Approved + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Cancelled + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Rejected + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Expired + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Complete_Pre_E_Check + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.KIV_CA + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.KIV_Pre_E + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Cancellation + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Pre_E + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Rejection + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Route_To_CA_Officer + "'");
                    //sqlStatement.Append("))");
                }
            }
            //Added by Edward 08.10.2013
            sqlStatement.Append(" ORDER BY DateIn DESC ");

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


        #region Added By Edward 8.11.2013 For Optimizing the Search using RefNo. Only Applicable for Completeness AllApps.aspx      
        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        //public static DataTable GetDocAppIDOnly(string section)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append(" SELECT Id, RefNo FROM DocApp WHERE (RefType = @section) OR (RefType <> @section) ORDER BY CASE WHEN RefType = @section THEN 1 ELSE 100 END, Id DESC ");
        //    command.Parameters.Add("@section", SqlDbType.VarChar);
        //    command.Parameters["@section"].Value = section;

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

        public static DataTable GetDocAppIDOnly(string section)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@section", SqlDbType.VarChar);
            command.Parameters["@section"].Value = section;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetDocAppIDOnly";
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

        public static int GetDocAppIDOnlyByRefNo(string refno)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append(" SELECT Id FROM DocApp WHERE (RefNo = @RefNo) ");
            command.Parameters.Add("@RefNo", SqlDbType.VarChar);
            command.Parameters["@RefNo"].Value = refno;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return int.Parse(dataSet.Tables[0].Rows[0]["Id"].ToString());
                else
                    return -1;                
            }
        }

        #endregion


        /// <summary>
        /// Get All Apps By Verification Date in
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public static DataTable GetAllAppsByVerificationDateIn(DateTime verificationDateIn, AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DocApp.*, Profile.Name as CompletenessOIC FROM DocApp LEFT JOIN Profile ");
            sqlStatement.Append("ON DocApp.CompletenessStaffUserId=Profile.UserId ");
            sqlStatement.Append("WHERE (CONVERT(DATE, DateIn) = CONVERT(DATE, @DateIn)) AND ");
            sqlStatement.Append("DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;


            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocApp.Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND CompletenessStaffUserId=@CompletenessStaffUserId ");

                command.Parameters.Add("@CompletenessStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@CompletenessStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND DateIn IS NOT NULL ");
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }

            command.Parameters.Add("@DateIn", SqlDbType.DateTime);
            command.Parameters["@DateIn"].Value = verificationDateIn;      

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
        /// Get Apps Assigned to specific user
        /// </summary>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="currUserId"></param>
        /// <param name="status"></param>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static DataTable GetAppsAssignedToUser(DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, AppStatusEnum? status, int docAppId, string nric)
        {

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Added TOP 500 By Edward 29.10.2013
            //sqlStatement.Append("SELECT * ");
            sqlStatement.Append("SELECT TOP 500 Id, RefNo, RefType, DateIn, DateOut, Status, CompletenessStaffUserId, DateAssigned, CancellationOption, CancellationRemark, CaseOIC,  ");
            sqlStatement.Append("PeOIC, DownloadStatus, DownloadedBy, DownloadedOn, SendToCDBStatus, SendToCDBAttemptCount, SecondCADate, SecondCA, SecondCAFlag, HasPendingDoc, DocInOrderDate  ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE CompletenessStaffUserId=@VerificationStaffUserId ");

            command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@VerificationStaffUserId"].Value = currUserId;

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            if (docAppId != -1)
            {
                sqlStatement.Append("AND Id=@DocAppId");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            //Added by Edward 08.10.2013
            sqlStatement.Append(" ORDER BY DateIn DESC");
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
        /// Get pending assignments verificatondates
        /// </summary>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentAppsVerificationDates(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, DateIn)) AS DateIn ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (Status=@Status)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append("ORDER BY DateIn DESC");

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
        //public static DataTable GetCompletenessCurrentCasesTotal2()
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("Profile.UserId, Profile.Name, ISNULL(SUBSTRING(DocApp.RefNo, 4, 1), '') AS Lane, COUNT(DocApp.RefNo) AS Total ");
        //    sqlStatement.Append("FROM Profile ");
        //    sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId = aspnet_UsersInRoles.UserId ");
        //    sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId AND aspnet_Roles.RoleName = 'Completeness Officer - AAD' ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocApp ON DocApp.CompletenessStaffUserId= Profile.UserId and DocApp.RefType = 'HLE' ");
        //    sqlStatement.Append("Group by Profile.UserId, Profile.Name, SUBSTRING(DocApp.RefNo, 4, 1) ");
        //    sqlStatement.Append("Order by Profile.Name");

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

        public static DataTable GetCompletenessCurrentCasesTotal2()
        {
            SqlCommand command = new SqlCommand();
            
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetCompletenessCurrentCasesTotal2";
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

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Added by Edward 06.08.2013
        /// </summary>
        /// <returns></returns>
        //public static DataTable GetAssessmentCurrentCasesTotal2()
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("Profile.UserId, Profile.Name, ISNULL(SUBSTRING(DocApp.RefNo, 4, 1), '') AS Lane, COUNT(DocApp.RefNo) AS Total ");
        //    sqlStatement.Append("FROM Profile ");
        //    sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId = aspnet_UsersInRoles.UserId ");
        //    sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId AND aspnet_Roles.RoleName = 'Income Extraction Officer - AAD' ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocApp ON DocApp.AssessmentStaffUserId= Profile.UserId and DocApp.RefType = 'HLE' ");
        //    sqlStatement.Append("Group by Profile.UserId, Profile.Name, SUBSTRING(DocApp.RefNo, 4, 1) ");
        //    sqlStatement.Append("Order by Profile.Name");

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

        public static DataTable GetAssessmentCurrentCasesTotal2()
        {
            SqlCommand command = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetAssessmentCurrentCasesTotal2";
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

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        //public static DataTable GetCompletenessOfficers()
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ");
        //    sqlStatement.Append("Profile.UserId, Profile.Name ");
        //    sqlStatement.Append("FROM Profile ");
        //    sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId=aspnet_UsersInRoles.UserId ");
        //    sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId=aspnet_Roles.RoleId AND aspnet_Roles.RoleName='Completeness Officer - AAD' ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocApp ON DocApp.CompletenessStaffUserId=Profile.UserId AND DocApp.RefType='HLE' ");
        //    sqlStatement.Append("Group by Profile.UserId, Profile.Name ");
        //    sqlStatement.Append("Order by Profile.Name");

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

        public static DataTable GetCompletenessOfficers()
        {
            SqlCommand command = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetCompletenessOfficers";
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

        public static DataTable GetCompletenessOfficersForReport(int docAppId, string refNo, Guid? completenessOic, DateTime? dateInFrom, DateTime? dateInTo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT ");
            sqlStatement.Append("Profile.UserId, Profile.Name ");
            sqlStatement.Append("FROM Profile ");
            sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId=aspnet_UsersInRoles.UserId ");
            sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId=aspnet_Roles.RoleId AND aspnet_Roles.RoleName='Completeness Officer - AAD' ");

            if (docAppId <= 0 && String.IsNullOrEmpty(refNo) && !dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("LEFT OUTER JOIN DocApp ON DocApp.CompletenessStaffUserId=Profile.UserId AND DocApp.RefType='HLE' ");
            }
            else
            {
                sqlStatement.Append("INNER JOIN DocApp ON DocApp.CompletenessStaffUserId=Profile.UserId ");

                bool hasWhereClause = false;

                if (docAppId >= 0)
                {
                    sqlStatement.Append("WHERE DocApp.Id=@docAppId AND DocApp.RefType='HLE' ");

                    command.Parameters.Add("@docAppId", SqlDbType.Int);
                    command.Parameters["@docAppId"].Value = docAppId;

                    hasWhereClause = true;
                }
                else if (docAppId <= 0 && !String.IsNullOrEmpty(refNo))
                {
                    sqlStatement.Append("WHERE DocApp.RefNo=@refNo AND DocApp.RefType='HLE' ");

                    command.Parameters.Add("@refNo", SqlDbType.NVarChar);
                    command.Parameters["@refNo"].Value = refNo;

                    hasWhereClause = true;
                }

                if (dateInFrom.HasValue && dateInTo.HasValue)
                {
                    if (!hasWhereClause)
                        sqlStatement.Append("WHERE ");
                    else
                        sqlStatement.Append("AND ");

                    sqlStatement.Append("DocApp.DateIn BETWEEN @dateInFrom AND @dateInTo AND DocApp.RefType='HLE' ");

                    command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                    command.Parameters["@dateInFrom"].Value = dateInFrom.Value;

                    command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                    command.Parameters["@dateInTo"].Value = dateInTo.Value;
                }
                else if (dateInFrom.HasValue && !dateInTo.HasValue)
                {
                    if (!hasWhereClause)
                        sqlStatement.Append("WHERE ");
                    else
                        sqlStatement.Append("AND ");

                    sqlStatement.Append("DocApp.DateIn >= @dateInFrom AND DocApp.RefType='HLE' ");

                    command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                    command.Parameters["@dateInFrom"].Value = dateInFrom.Value;
                }
                else if (dateInTo.HasValue && !dateInFrom.HasValue)
                {
                    if (!hasWhereClause)
                        sqlStatement.Append("WHERE ");
                    else
                        sqlStatement.Append("AND ");

                    sqlStatement.Append("DocApp.DateIn <= @dateInTo AND DocApp.RefType='HLE' ");

                    command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                    command.Parameters["@dateInTo"].Value = dateInTo.Value;
                }
            }

            if (completenessOic.HasValue)
            {
                if (!sqlStatement.ToString().ToLower().Contains("where"))
                    sqlStatement.Append("WHERE ");
                else
                    sqlStatement.Append("AND ");
                
                sqlStatement.Append("Profile.UserId = @userId ");
                command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
                command.Parameters["@userId"].Value = completenessOic.Value;                
            }

            sqlStatement.Append("Group by Profile.UserId, Profile.Name ");
            sqlStatement.Append("Order by Profile.Name");

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

        public static DataTable GetCompletenessOfficersForReportDetails(Guid? completenessUserId, int docAppId, string refNo, AppStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Add Top 500 By Edward 29.10.2013
            //updated by Andrew (16/1/2013) to show Completeness OIC Name in Assignment Report          
            //sqlStatement.Append("SELECT DocApp.*, Profile.Name As CompletenessOIC FROM DocApp ");
            sqlStatement.Append("SELECT TOP 500 DocApp.*, Profile.Name As CompletenessOIC FROM DocApp ");
            sqlStatement.Append("INNER JOIN Profile ON DocApp.CompletenessStaffUserId = Profile.UserId ");
            sqlStatement.Append("WHERE RefType='HLE' ");

            if (completenessUserId.HasValue)
            {
                sqlStatement.Append("AND CompletenessStaffUserId=@userId ");
                command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
                command.Parameters["@userId"].Value = completenessUserId.Value;
            }


            if (docAppId >= 0)
            {
                //added by Andrew (16/1/2013)
                sqlStatement.Append("AND DocApp.Id=@docAppId ");

                command.Parameters.Add("@docAppId", SqlDbType.Int);
                command.Parameters["@docAppId"].Value = docAppId;
            }
            else if (docAppId <= 0 && !String.IsNullOrEmpty(refNo))
            {
                sqlStatement.Append("AND RefNo=@refNo ");

                command.Parameters.Add("@refNo", SqlDbType.NVarChar);
                command.Parameters["@refNo"].Value = refNo;
            }

            if (status.HasValue)
            {
                sqlStatement.Append("AND Status=@status ");

                command.Parameters.Add("@status", SqlDbType.NVarChar);
                command.Parameters["@status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateAssigned BETWEEN @dateInFrom AND @dateInTo ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateAssigned >= @dateInFrom ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateAssigned <= @dateInTo ");

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //Added Order by Edward 29.10.2013
            sqlStatement.Append(" ORDER BY DateIn DESC ");
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


        #region INCOME ASSESSMENT

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        //public static DataTable GetAssessmentOfficers()
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ");
        //    sqlStatement.Append("Profile.UserId, Profile.Name ");
        //    sqlStatement.Append("FROM Profile ");
        //    sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId=aspnet_UsersInRoles.UserId ");
        //    sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId=aspnet_Roles.RoleId AND aspnet_Roles.RoleName='Income Extraction Officer - AAD' ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocApp ON DocApp.AssessmentStaffUserId=Profile.UserId AND DocApp.RefType='HLE' ");
        //    sqlStatement.Append("Group by Profile.UserId, Profile.Name ");
        //    sqlStatement.Append("Order by Profile.Name");

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


        public static DataTable GetAssessmentOfficers()
        {
            SqlCommand command = new SqlCommand();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocApp_GetAssessmentOfficers";
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
        /// getAllApps of Income Assessment
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public static DataTable GetAllAppsIA(AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT TOP 500 DocApp.*, Profile.Name as AssessmentOIC FROM DocApp LEFT JOIN Profile ");             
            sqlStatement.Append("ON DocApp.AssessmentStaffUserId=Profile.UserId ");
            sqlStatement.Append("WHERE DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;


            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocApp.Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND AssessmentStaffUserId=@AssessmentStaffUserId ");

                command.Parameters.Add("@AssessmentStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@AssessmentStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                //Changed to AssessmentStatus June 3, 2013-Edward
                //sqlStatement.Append("AND Status=@Status ");
                sqlStatement.Append("AND AssessmentStatus=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                //Changed to AssessmentStatus June 3, 2013-Edward
                //sqlStatement.Append("AND (Status=@Status1 OR Status=@Status2 OR Status=@Status3 OR Status=@Status4 OR Status=@Status5 OR Status=@Status6) ");
                sqlStatement.Append("AND (AssessmentStatus IN (@Status1, @Status2, @Status3, @Status4, @Status5, @Status6, @Status7)) ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters.Add("@Status3", SqlDbType.VarChar);
                command.Parameters.Add("@Status4", SqlDbType.VarChar);
                command.Parameters.Add("@Status5", SqlDbType.VarChar);
                command.Parameters.Add("@Status6", SqlDbType.VarChar);
                command.Parameters.Add("@Status7", SqlDbType.VarChar);
                //command.Parameters["@Status1"].Value = AppStatusEnum.Verified.ToString();
                command.Parameters["@Status1"].Value = AssessmentStatusEnum.Extracted.ToString();
                command.Parameters["@Status2"].Value = AssessmentStatusEnum.Extraction_In_Progress.ToString();
                command.Parameters["@Status3"].Value = AssessmentStatusEnum.Closed.ToString();
                command.Parameters["@Status4"].Value = AssessmentStatusEnum.Extraction_Cancelled.ToString();
                command.Parameters["@Status5"].Value = AssessmentStatusEnum.Completeness_Checked.ToString();
                command.Parameters["@Status6"].Value = AssessmentStatusEnum.Pending_Extraction.ToString();
                command.Parameters["@Status7"].Value = AssessmentStatusEnum.Completeness_Cancelled.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND AssessmentDateIn IS NOT NULL ");
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (checkHleStatus)
            {
                if (!string.IsNullOrEmpty(hleStatus))
                {
                    sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus=@HleStatus) ");
                    command.Parameters.Add("@HleStatus", SqlDbType.VarChar);
                    command.Parameters["@HleStatus"].Value = hleStatus;
                }
                else
                {
                    sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface) ");
                    //sqlStatement.Append(" AND DocApp.RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus in (");
                    //sqlStatement.Append("'" + HleStatusEnum.Approved + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Cancelled + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Rejected + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Expired + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Complete_Pre_E_Check + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.KIV_CA + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.KIV_Pre_E + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Cancellation + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Pre_E + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Pending_Rejection + "',");
                    //sqlStatement.Append("'" + HleStatusEnum.Route_To_CA_Officer + "'");
                    //sqlStatement.Append("))");
                }
            }
            sqlStatement.Append(" ORDER BY AssessmentDateIn DESC ");

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
        /// Get All Apps verification Dates
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public static DataTable GetAllAppsVerificationDatesIA(AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, AssessmentDateIn)) AS AssessmentDateIn FROM DocApp ");
            sqlStatement.Append("WHERE DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocApp.Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND AssessmentStaffUserId=@AssessmentStaffUserId ");

                command.Parameters.Add("@AssessmentStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@AssessmentStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                //June 4, 2013-Edward
                //sqlStatement.Append("AND Status=@Status ");
                sqlStatement.Append("AND AssessmentStatus=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                //June 4, 2013-Edward
                //sqlStatement.Append("AND Status<>@Status1 ");
                sqlStatement.Append("AND AssessmentStatus<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AssessmentStatusEnum.Extracted.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND AssessmentDateIn IS NOT NULL ");
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
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
        /// Get All Apps By Verification Date in for Income Assessment
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public static DataTable GetAllAppsByVerificationDateInIA(DateTime verificationDateIn, AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DocApp.*, Profile.Name as AssessmentOIC FROM DocApp LEFT JOIN Profile ");
            sqlStatement.Append("ON DocApp.AssessmentStaffUserId=Profile.UserId ");
            sqlStatement.Append("WHERE (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @DateIn)) AND ");
            sqlStatement.Append("DocApp.Id IN (SELECT DocAppId FROM vDocSet WHERE SectionId=@SectionId) ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;


            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocApp.Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND AssessmentStaffUserId=@AssessmentStaffUserId ");

                command.Parameters.Add("@AssessmentStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@AssessmentStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND AssessmentStatus=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND AssessmentStatus<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AssessmentStatusEnum.Extracted.ToString();
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            //sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else
            {
                sqlStatement.Append("AND AssessmentDateIn IS NOT NULL ");
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }

            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DocApp.DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }

            command.Parameters.Add("@DateIn", SqlDbType.DateTime);
            command.Parameters["@DateIn"].Value = verificationDateIn;

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
        /// Get pending assignments for Income Assessment
        /// </summary>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentAppsIA(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, AssessmentStatusEnum? status1, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            
            sqlStatement.Append("SELECT TOP 500 * ");            
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (AssessmentStatus=@Status OR AssessmentStatus=@Status1)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;
            command.Parameters.Add("@Status1", SqlDbType.VarChar);
            command.Parameters["@Status1"].Value = status1;

            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append(" ORDER BY AssessmentDateIn DESC ");
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

        public static DataTable GetPendingAssignmentAppsVerificationDatesIA(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, AssessmentStatusEnum? status1, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, AssessmentDateIn)) AS AssessmentDateIn ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (AssessmentStatus=@Status OR AssessmentStatus=@Status1)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;
            command.Parameters.Add("@Status1", SqlDbType.VarChar);
            command.Parameters["@Status1"].Value = status1;

            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append("ORDER BY AssessmentDateIn DESC");

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
        /// Get Apps Assigned to specific user for Income Assessment 
        /// </summary>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="currUserId"></param>
        /// <param name="status"></param>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static DataTable GetAppsAssignedToUserIA(DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, AssessmentStatusEnum? status, int docAppId, string nric)
        {

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT TOP 500 * ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE AssessmentStaffUserId=@VerificationStaffUserId ");

            command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@VerificationStaffUserId"].Value = currUserId;

            if (status != null)
            {
                sqlStatement.Append("AND AssessmentStatus=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            //else
            //{
            //    sqlStatement.Append("AND AssessmentStatus<>@Status1 ");

            //    command.Parameters.Add("@Status1", SqlDbType.VarChar);
            //    command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            //}

            if (docAppId != -1)
            {
                sqlStatement.Append("AND Id=@DocAppId");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(")");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append(" ORDER BY AssessmentDateIn DESC ");

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
        /// Assignment Report for Income Assessment 
        /// </summary>
        /// <param name="completenessUserId"></param>
        /// <param name="docAppId"></param>
        /// <param name="refNo"></param>
        /// <param name="status"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static DataTable GetAssessmentOfficersForReportDetailsIA(Guid? UserId, int docAppId, string refNo, AssessmentStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //updated by Andrew (16/1/2013) to show Completeness OIC Name in Assignment Report
            sqlStatement.Append("SELECT TOP 500 DocApp.*, Profile.Name As AssessmentOIC FROM DocApp ");
            sqlStatement.Append("INNER JOIN Profile ON DocApp.AssessmentStaffUserId = Profile.UserId ");
            sqlStatement.Append("WHERE RefType='HLE' ");

            if (UserId.HasValue)
            {
                sqlStatement.Append("AND AssessmentStaffUserId=@userId ");
                command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
                command.Parameters["@userId"].Value = UserId.Value;
            }


            if (docAppId >= 0)
            {
                //added by Andrew (16/1/2013)
                sqlStatement.Append("AND DocApp.Id=@docAppId ");

                command.Parameters.Add("@docAppId", SqlDbType.Int);
                command.Parameters["@docAppId"].Value = docAppId;
            }
            else if (docAppId <= 0 && !String.IsNullOrEmpty(refNo))
            {
                sqlStatement.Append("AND RefNo=@refNo ");

                command.Parameters.Add("@refNo", SqlDbType.NVarChar);
                command.Parameters["@refNo"].Value = refNo;
            }

            if (status.HasValue)
            {
                sqlStatement.Append("AND AssessmentStatus=@status ");

                command.Parameters.Add("@status", SqlDbType.NVarChar);
                command.Parameters["@status"].Value = status;
            }
            //else
            //{
            //    sqlStatement.Append("AND AssessmentStatus<>@Status1 ");

            //    command.Parameters.Add("@Status1", SqlDbType.VarChar);
            //    command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            //}

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateAssigned BETWEEN @dateInFrom AND @dateInTo ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateAssigned >= @dateInFrom ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateAssigned <= @dateInTo ");

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            sqlStatement.Append(" ORDER BY AssessmentDateIn DESC ");
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
        /// used in Pending Assignment when group by date. Income Assessment
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="status"></param>
        /// <param name="sectionId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>

        public static DataTable GetPendingAssignmentSetsByVerificationDateInIA(DateTime verificationDateIn, int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, int sectionId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * ");
            sqlStatement.Append("FROM DocApp ");
            sqlStatement.Append("WHERE  (CONVERT(DATE, AssessmentDateIn) = CONVERT(DATE, @VerificationDateIn)) AND ");
            sqlStatement.Append("Id IN (SELECT docappid FROM vDocSet WHERE sectionid=@SectionId) AND (AssessmentStatus=@Status)");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (refID != -1)
            {
                sqlStatement.Append("AND Id=@Id ");

                command.Parameters.Add("@Id", SqlDbType.Int);
                command.Parameters["@Id"].Value = refID;
            }

            // Some applications with null DateIn were shown in All Applications page
            // Added by Lexin on 2 May 2012 as a temporary solution. To be removed when a permanant solution is found.
            sqlStatement.Append("AND LEN(DateIn) > 0 ");

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND AssessmentDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = verificationDateIn;

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


        #region UpdateMethods
        /// <summary>
        /// NOT IN USE
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="lane"></param>
        public static void UpdateTopPendingHleByLanes(string userId, int count, HleLanesEnum lane)
        {
            /// NOT IN USE MATHEW to confirm and remove or KIV
            /// 
            StringBuilder sqlStatement = new StringBuilder();

            //sqlStatement.Append("WITH TopDocApp AS _openParen_ SELECT TOP {0} * FROM DocApp WHERE Status = '{1}' AND CompletenessStaffUserId IS NULL AND SUBSTRING(RefNo, 4, 1) = '{2}' ORDER BY DateIn _closeParen_ ");
            //sqlStatement.Append("UPDATE TopDocApp SET CompletenessStaffUserId = '{3}', Status = '{4}'");
            
            sqlStatement.Append("UPDATE DocApp SET CompletenessStaffUserId = '{0}', Status = '{1}', DateAssigned = GETDATE() ");
            sqlStatement.Append("WHERE Id IN (SELECT TOP {2} Id FROM DocApp WHERE Status = '{3}' AND CompletenessStaffUserId IS NULL AND SUBSTRING(RefNo, 4, 1) = '{4}' ORDER BY DateIn) ");

            string query = String.Format(sqlStatement.ToString(), userId, AppStatusEnum.Pending_Completeness.ToString(), count.ToString(), AppStatusEnum.Verified.ToString(), lane.ToString());
            //query = query.Replace("_openParen_", "{").Replace("_closeParen_", "}");

            ExecuteNonQuery(query);
        }
        #endregion

        #region Added By Edward to address FileDoc Download for all status verified above 05/02/2014 Sales and Resales Changes
        public static DataTable GetAllAppsForFileDoc(AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT TOP 500 * FROM vCompletenessGetAllApps ");
            sqlStatement.Append("WHERE Id IN (SELECT DocAppId FROM vVerificationGetAllSets WHERE SectionId=@SectionId) ");

            #region Section Parameter
            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;
            #endregion

            #region DocAppId Parameter
            if (docAppId != -1)
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append("AND DocApp.Id=@DocAppId ");
                sqlStatement.Append("AND Id=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }
            #endregion

            #region CompletenessStaffUserId Parameter
            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND CompletenessStaffUserId=@CompletenessStaffUserId ");

                command.Parameters.Add("@CompletenessStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@CompletenessStaffUserId"].Value = verificationOicUserId.Value;
            }
            #endregion

            #region Status Parameter
            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND (Status=@Status1 OR Status=@Status2 OR Status=@Status3 OR Status=@Status4 OR Status=@Status5 OR Status=@Status6) ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters.Add("@Status3", SqlDbType.VarChar);
                command.Parameters.Add("@Status4", SqlDbType.VarChar);
                command.Parameters.Add("@Status5", SqlDbType.VarChar);
                command.Parameters.Add("@Status6", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Closed.ToString();
                command.Parameters["@Status2"].Value = AppStatusEnum.Completeness_Cancelled.ToString();
                command.Parameters["@Status3"].Value = AppStatusEnum.Completeness_Checked.ToString();
                command.Parameters["@Status4"].Value = AppStatusEnum.Completeness_In_Progress.ToString();
                command.Parameters["@Status5"].Value = AppStatusEnum.Pending_Completeness.ToString();
                command.Parameters["@Status6"].Value = AppStatusEnum.Verified.ToString();
            }
            #endregion

            #region DateIn Parameter
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            #endregion

            #region Download Status
            if (!string.IsNullOrEmpty(downloadStatus))
            {
                sqlStatement.Append("AND LTRIM(RTRIM(DownloadStatus)) = @DownloadStatus ");
                command.Parameters.Add("@DownloadStatus", SqlDbType.VarChar);
                command.Parameters["@DownloadStatus"].Value = downloadStatus;
            }
            #endregion

            #region Nric Parameter
            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            #endregion

            #region Acknowledge Paramter
            //acknowledgeNumber check
            acknowledgeNumber = acknowledgeNumber.Trim();
            if (!String.IsNullOrEmpty(acknowledgeNumber))
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append(" AND Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Id FROM DocSet WHERE AcknowledgeNumber LIKE '%' + @acknowledgeNumber + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@acknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@acknowledgeNumber"].Value = acknowledgeNumber;
            }
            #endregion

            #region hlestatus
            if (sectionId == 1)
            {
                if (checkHleStatus)
                {
                    if (!string.IsNullOrEmpty(hleStatus))
                    {
                        sqlStatement.Append(" AND RefNo IN (SELECT HleNumber FROM HleInterface WHERE HleStatus=@HleStatus) ");
                        command.Parameters.Add("@HleStatus", SqlDbType.VarChar);
                        command.Parameters["@HleStatus"].Value = hleStatus;
                    }
                    else
                    {
                        sqlStatement.Append(" AND RefNo IN (SELECT HleNumber FROM HleInterface) ");
                    }
                }
            }
            #endregion
            //Added by Edward 08.10.2013
            sqlStatement.Append(" ORDER BY DateIn DESC ");

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

        #region Added By Edward 2015/02/10 Added Risk Field in Email Notification
        public static string GetDocAppRiskOnlyById(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append(" SELECT ISNULL(Risk,'') AS Risk FROM DocApp WHERE (Id = @DocAppId) ");
            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    return !string.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["Risk"].ToString()) ? dataSet.Tables[0].Rows[0]["Risk"].ToString() : string.Empty;
                }
                else
                    return string.Empty;
            }
        }
        #endregion


        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        public static string GetAssessmentStatusByDocAppId(int id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT AssessmentStatus FROM DocApp WHERE (Id = @Id)  ");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = id;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return !string.IsNullOrEmpty(dataSet.Tables[0].Rows[0]["AssessmentStatus"].ToString()) ? dataSet.Tables[0].Rows[0]["AssessmentStatus"].ToString() : string.Empty;
                else
                    return string.Empty;
            }
        }

        #endregion


        #region  added by Edward 2017/11/01 To Optimize speed of Verification Listing 
        public static bool UpdateAndCheckHasPendingDoc(string hleNumber)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@RefNo", SqlDbType.NVarChar);
            command.Parameters["@RefNo"].Value = hleNumber;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Util_HasPendingDoc";
                command.Connection = connection;
                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString().Equals("1") ? true : false;
                }
                else
                    return false;
            }
        }
        #endregion
    }
}

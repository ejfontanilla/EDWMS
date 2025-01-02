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
    public class BatchUploadDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        public static int InsertBUHeader(int sectionId, string fileName, string oic, int noOfCases, int noOfFailed)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO BatchUpload (SectionId, XMLFileName, DateUploaded, OIC, NoOfCases, NoOfFailed) VALUES ");
            sqlStatement.Append("(@SectionID, @FileName, GETDATE(), @OIC, @NoOfCases, @NoOfCasesFailed);SELECT @@Identity");

            command.Parameters.Add("@SectionID", SqlDbType.Int);
            command.Parameters["@SectionID"].Value = sectionId;

            command.Parameters.Add("@FileName", SqlDbType.VarChar);
            command.Parameters["@FileName"].Value = fileName;

            command.Parameters.Add("@OIC", SqlDbType.VarChar);
            command.Parameters["@OIC"].Value = oic;

            command.Parameters.Add("@NoOfCases", SqlDbType.Int);
            command.Parameters["@NoOfCases"].Value = noOfCases;

            command.Parameters.Add("@NoOfCasesFailed", SqlDbType.Int);
            command.Parameters["@NoOfCasesFailed"].Value = noOfFailed;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }

        public static int UpdateBUHeaderNoOfCases(int id, int noOfCases, int noOfFailed)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE BatchUpload SET NoOfCases = @NoOfCases, NoOfFailed = @NoOfFailed, DateFinished = GETDATE() WHERE Id = @Id");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = id;

            command.Parameters.Add("@NoOfCases", SqlDbType.Int);
            command.Parameters["@NoOfCases"].Value = noOfCases;

            command.Parameters.Add("@NoOfFailed", SqlDbType.Int);
            command.Parameters["@NoOfFailed"].Value = noOfFailed;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int InsertBUDetails(int buId, string refNo, bool isSuccess, string errorMsg)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO BatchUploadDetails (BUId, RefNo, IsSuccess, Error) VALUES ");
            sqlStatement.Append("(@BUId, @RefNo, @IsSuccess, @Error);SELECT @@Identity");

            command.Parameters.Add("@BUId", SqlDbType.Int);
            command.Parameters["@BUId"].Value = buId;

            command.Parameters.Add("@RefNo", SqlDbType.VarChar);
            command.Parameters["@RefNo"].Value = refNo;

            command.Parameters.Add("@IsSuccess", SqlDbType.Bit);
            command.Parameters["@IsSuccess"].Value = isSuccess;

            command.Parameters.Add("@Error", SqlDbType.VarChar);
            command.Parameters["@Error"].Value = errorMsg;       

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }


        public static DataTable GetBatchUpload(int section, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT a.Id AS BUId, DateUploaded, b.Code AS Section, XMlFileName, OIC, NoOfCases, NoOfFailed FROM BatchUpload a ");
            sqlStatement.Append("INNER JOIN Section b ON a.SectionId = b.Id INNER JOIN Department c ON c.Id = b.Department ");

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND b.Department=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.Int);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.Int);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            #region date
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateUploaded BETWEEN @DateInFrom AND @DateInTo ");
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateUploaded >= @DateInFrom ");
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {

                sqlStatement.Append("AND DateUploaded <= @DateInTo ");
                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            #endregion

            sqlStatement.Append("ORDER BY DateUploaded DESC ");

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

        public static DataTable GetBatchUploadDetailsById(int buId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT RefNo FROM BatchUploadDetails WHERE BUId = @BUId AND IsSuccess = 0 ");
            command.Parameters.Add("@BUId", SqlDbType.Int);
            command.Parameters["@BUId"].Value = buId;
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
    }

    #region Added by Edward 2015/02/05 For Archival Report
    public class ArchiveDocumentsDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        public static DataTable GetArchivalReport(int section, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT NoOfArchive, DateArchived, RefNo, Division, PeriodOfMonths FROM vArchivalReport WHERE 1 = 1 ");

            #region department and section
            if (department != -1)
            {
                if (department == 1)
                    sqlStatement.Append("AND Division='COS' ");
                else if(department == 2)
                    sqlStatement.Append("AND Division='SERS' ");
                else if (department == 3)
                    sqlStatement.Append("AND Division='RESALE' ");
                else if (department == 4)
                    sqlStatement.Append("AND Division='SALES' ");
            }

            // Add the section query
            if (section != -1)
            {
                //sqlStatement.Append("AND SectionId=@SectionId ");
                //command.Parameters.Add("@SectionId", SqlDbType.Int);
                //command.Parameters["@SectionId"].Value = section;
            }
            #endregion


            #region date
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateArchived BETWEEN @DateInFrom AND @DateInTo ");
                command.Parameters.Add("@DateInFrom", SqlDbType.VarChar);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value.ToShortDateString();

                command.Parameters.Add("@DateInTo", SqlDbType.VarChar);
                command.Parameters["@DateInTo"].Value = dateInTo.Value.ToShortDateString();
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DateArchived >= @DateInFrom ");
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {

                sqlStatement.Append("AND DateArchived <= @DateInTo ");
                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            #endregion

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
    }
    #endregion
}
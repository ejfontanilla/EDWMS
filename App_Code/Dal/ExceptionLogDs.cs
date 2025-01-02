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
    public class ExceptionLogDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods
        
        /// <summary>
        /// Get Exception Log
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static DataTable GetLogForExceptionReport(string channel, string refNo, DateTime? fromDate, DateTime? toDate)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            
            sqlStatement.Append("SELECT * FROM ExceptionLog WHERE 1=1 ");

            if (!String.IsNullOrEmpty(channel))
            {
                sqlStatement.Append("AND Channel=@channel ");

                command.Parameters.Add("@channel", SqlDbType.VarChar);
                command.Parameters["@channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(refNo))
            {
                sqlStatement.Append("AND RefNo=@refNo ");

                command.Parameters.Add("@refNo", SqlDbType.VarChar);
                command.Parameters["@refNo"].Value = refNo;
            }

            if (fromDate.HasValue && toDate.HasValue)
            {
                sqlStatement.Append("AND DateOccurred BETWEEN @dateFrom AND @dateTo ");

                command.Parameters.Add("@dateFrom", SqlDbType.DateTime);
                command.Parameters["@dateFrom"].Value = fromDate.Value;

                command.Parameters.Add("@dateTo", SqlDbType.DateTime);
                command.Parameters["@dateTo"].Value = toDate.Value;
            }
            else if (fromDate.HasValue && !toDate.HasValue)
            {
                sqlStatement.Append("AND DateOccurred >= @dateFrom ");

                command.Parameters.Add("@dateFrom", SqlDbType.DateTime);
                command.Parameters["@dateFrom"].Value = fromDate.Value;
            }
            else if (!fromDate.HasValue && toDate.HasValue)
            {
                sqlStatement.Append("AND DateOccurred <= @dateTo ");

                command.Parameters.Add("@dateTo", SqlDbType.DateTime);
                command.Parameters["@dateTo"].Value = toDate.Value;
            }
            //Added By Edward 08.10.2013
            sqlStatement.Append(" ORDER BY DateOccurred DESC ");

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

        #region Added by Edward Development of reports 2014/06/09
        public static DataTable GetErrorSendingToCDB(int section, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT d.VerificationDateOut, a.RefNo, d.SetNo, a.ErrorMessage, ");
            sqlStatement.Append("(SELECT COUNT(Id) FROM Doc e WHERE e.DocSetId =  d.Id AND e.SendToCDBStatus LIKE '%failed%') AS NoOfDocuments ");
            sqlStatement.Append("FROM ExceptionLog a INNER JOIN DocApp b ON a.RefNo = b.RefNo INNER JOIN SetApp c ON c.DocAppId = b.Id ");
            sqlStatement.Append("INNER JOIN DocSet d ON d.Id = c.DocSetId WHERE d.SendToCDBStatus LIKE '%failed%' ");

            #region department and section
            if (department != -1)
            {               
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
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
                sqlStatement.Append("AND d.VerificationDateOut BETWEEN @DateInFrom AND @DateInTo ");
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND d.VerificationDateOut >= @DateInFrom ");
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {

                sqlStatement.Append("AND d.VerificationDateOut <= @DateInTo ");               
                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            #endregion

            sqlStatement.Append("ORDER BY d.VerificationDateOut DESC ");

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

        public static DataTable GetOCRWebServiceError(int section, int department)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DATEPART(MONTH,DateOccurred) AS OcrMonth, DATEPART(YEAR,DateOccurred) AS OcrYear, ");
            sqlStatement.Append("Reason FROM ExceptionLog a INNER JOIN DocApp b ON a.RefNo = b.RefNo WHERE 1=1  ");

            #region department and section
            if (department != -1)
            {                
                sqlStatement.Append("AND b.RefType=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                if (department == 1)
                    command.Parameters["@DepartmentId"].Value = "HLE";
                else if (department == 2)
                    command.Parameters["@DepartmentId"].Value = "SERS";
                else if (department == 3)
                    command.Parameters["@DepartmentId"].Value = "RESALE";
                else if (department == 4)
                    command.Parameters["@DepartmentId"].Value = "SALES";
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND b.RefType=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                if (section == 1)
                    command.Parameters["@SectionId"].Value = "HLE";
                else if (section == 3 || section == 4)
                    command.Parameters["@SectionId"].Value = "SERS";
                else if (section == 5)
                    command.Parameters["@SectionId"].Value = "RESALE";
                else if (section == 6)
                    command.Parameters["@SectionId"].Value = "SALES";
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


        public static DataTable GetOCRWebServiceError(int section, int department, int month, int year)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DATEPART(MONTH,DateOccurred) AS OcrMonth, DATEPART(YEAR,DateOccurred) AS OcrYear, ");
            sqlStatement.Append("Reason FROM ExceptionLog a INNER JOIN DocApp b ON a.RefNo = b.RefNo WHERE  ");
            sqlStatement.Append("DATEPART(MONTH,DateOccurred) = @Month AND DATEPART(YEAR,DateOccurred) = @Year  ");

            command.Parameters.Add("@Month", SqlDbType.Int);
            command.Parameters["@Month"].Value = month;
            command.Parameters.Add("@Year", SqlDbType.Int);
            command.Parameters["@Year"].Value = year;

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND b.RefType=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                if (department == 1)
                    command.Parameters["@DepartmentId"].Value = "HLE";
                else if (department == 2)
                    command.Parameters["@DepartmentId"].Value = "SERS";
                else if (department == 3)
                    command.Parameters["@DepartmentId"].Value = "RESALE";
                else if (department == 4)
                    command.Parameters["@DepartmentId"].Value = "SALES";
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND b.RefType=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                if (section == 1)
                    command.Parameters["@SectionId"].Value = "HLE";
                else if (section == 3 || section == 4)
                    command.Parameters["@SectionId"].Value = "SERS";
                else if (section == 5)
                    command.Parameters["@SectionId"].Value = "RESALE";
                else if (section == 6)
                    command.Parameters["@SectionId"].Value = "SALES";
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

        public static DataTable GetOCRWebServiceMonthYear(int section, int department)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DATEPART(MONTH,DateOccurred) AS OcrMonth, DATEPART(YEAR,DateOccurred) AS OcrYear ");
            sqlStatement.Append("FROM ExceptionLog a INNER JOIN DocApp b ON a.RefNo = b.RefNo WHERE 1=1 ");
                        
            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND b.RefType=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                if (department == 1)
                    command.Parameters["@DepartmentId"].Value = "HLE";
                else if (department == 2)
                    command.Parameters["@DepartmentId"].Value = "SERS";
                else if (department == 3)
                    command.Parameters["@DepartmentId"].Value = "RESALE";
                else if (department == 4)
                    command.Parameters["@DepartmentId"].Value = "SALES";
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND b.RefType=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                if (section == 1)
                    command.Parameters["@SectionId"].Value = "HLE";
                else if (section == 3 || section == 4)
                    command.Parameters["@SectionId"].Value = "SERS";
                else if (section == 5)
                    command.Parameters["@SectionId"].Value = "RESALE";
                else if (section == 6)
                    command.Parameters["@SectionId"].Value = "SALES";
            }
            #endregion

            sqlStatement.Append("ORDER BY DATEPART(YEAR,DateOccurred) DESC, DATEPART(MONTH,DateOccurred) DESC  ");

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


        public static int GetOCRWebCount(int section, int department, int month, int year, string reason)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT COUNT(*) AS OcrWebCount FROM ExceptionLog a INNER JOIN DocApp b ON a.RefNo = b.RefNo  ");
            sqlStatement.Append("WHERE DATEPART(MONTH,DateOccurred) = @Month AND DATEPART(YEAR,DateOccurred) = @Year  ");
            sqlStatement.Append("AND Reason = @Reason ");

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND b.RefType=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                if (department == 1)
                    command.Parameters["@DepartmentId"].Value = "HLE";
                else if (department == 2)
                    command.Parameters["@DepartmentId"].Value = "SERS";
                else if (department == 3)
                    command.Parameters["@DepartmentId"].Value = "RESALE";
                else if (department == 4)
                    command.Parameters["@DepartmentId"].Value = "SALES";
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND b.RefType=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                if (section == 1)
                    command.Parameters["@SectionId"].Value = "HLE";
                else if (section == 3 || section == 4)
                    command.Parameters["@SectionId"].Value = "SERS";
                else if (section == 5)
                    command.Parameters["@SectionId"].Value = "RESALE";
                else if (section == 6)
                    command.Parameters["@SectionId"].Value = "SALES";
            }
            #endregion

            command.Parameters.Add("@Month", SqlDbType.Int);
            command.Parameters["@Month"].Value = month;

            command.Parameters.Add("@Year", SqlDbType.Int);
            command.Parameters["@Year"].Value = year;

            command.Parameters.Add("@Reason", SqlDbType.VarChar);
            command.Parameters["@Reason"].Value = reason;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                return (int) command.ExecuteScalar();                
            }
        }
        #endregion
    }
}

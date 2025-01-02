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
    /// <summary>
    /// For the Income Table as well as other tables related to it
    /// </summary>
    public class IncomeDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Gets the Income records
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public static DataTable GetDataForIncomeAssessment(int docAppId, string nric)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    //sqlStatement.Append("SELECT DISTINCT Doc.*,  ");
        //    sqlStatement.Append("SELECT DISTINCT   ");
        //    sqlStatement.Append("Income.Id , ");
        //    sqlStatement.Append("DateName( month , DateAdd( month , Income.IncomeMonth , 0 ) - 1 ) + ' ' + CAST(Income.IncomeYear AS VARCHAR)  AS MonthYear,  ");
        //    sqlStatement.Append("Income.IncomeMonth, ");
        //    sqlStatement.Append("Income.IncomeYear, ");
        //    //sqlStatement.Append("Doctype.Description AS DoctypeDescription, ");
        //    #region Modified By Edward 24/3/2014    Show converted foreign Currency
        //    //sqlStatement.Append("(SELECT SUM(IncomeAmount) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND Allowance = 1) AS 'Allowance', ");
        //    //sqlStatement.Append("(SELECT SUM(IncomeAmount) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND CPFIncome = 1) AS 'CPFIncome', ");
        //    //sqlStatement.Append("(SELECT SUM(IncomeAmount) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND GrossIncome = 1) AS 'GrossIncome', ");
        //    //sqlStatement.Append("(SELECT SUM(IncomeAmount) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND AHGIncome = 1) AS 'AHGIncome', ");
        //    //sqlStatement.Append("(SELECT SUM(IncomeAmount) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND Overtime = 1) AS 'Overtime', ");
        //    sqlStatement.Append("(SELECT CAST(ROUND(SUM(IncomeAmount) / CurrencyRate,2) AS NUMERIC(36,2)) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND Allowance = 1) AS 'Allowance', ");
        //    sqlStatement.Append("(SELECT CAST(ROUND(SUM(IncomeAmount) / CurrencyRate,2) AS NUMERIC(36,2)) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND CPFIncome = 1) AS 'CPFIncome', ");
        //    sqlStatement.Append("(SELECT CAST(ROUND(SUM(IncomeAmount) / CurrencyRate,2) AS NUMERIC(36,2)) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND GrossIncome = 1) AS 'GrossIncome', ");
        //    sqlStatement.Append("(SELECT CAST(ROUND(SUM(IncomeAmount) / CurrencyRate,2) AS NUMERIC(36,2)) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND AHGIncome = 1) AS 'AHGIncome', ");
        //    sqlStatement.Append("(SELECT CAST(ROUND(SUM(IncomeAmount) / CurrencyRate,2) AS NUMERIC(36,2)) FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = Income.IncomeVersionId AND Overtime = 1) AS 'Overtime', ");
        //    #endregion
        //    sqlStatement.Append("Income.Currency, Income.CurrencyRate, AppPersonal.Nric, AppPersonal.DocAppId, Income.IncomeVersionId, AppPersonal.MonthsToLEAS, AppPersonal.Id AS AppPersonalId ");
        //    sqlStatement.Append("FROM Income ");
        //    sqlStatement.Append("LEFT JOIN IncomeVersion ON Income.Id = IncomeVersion.IncomeId ");
        //    sqlStatement.Append("LEFT JOIN IncomeDetails ON IncomeVersion.Id = IncomeDetails.IncomeVersionID ");
        //    sqlStatement.Append("LEFT JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
        //    sqlStatement.Append("LEFT JOIN Doc ON AppDocRef.DocId = Doc.Id ");            
        //    sqlStatement.Append("LEFT JOIN DocType ON Doc.DocTypeCode = Doctype.Code ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric))");
        //    //Added 22.10.2013
        //    sqlStatement.Append(" ORDER BY Income.IncomeYear ASC, Income.IncomeMonth ASC");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

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

        public static DataTable GetDataForIncomeAssessment(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_GetDataForIncomeAssessment";
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

        //public static DataTable GetIncomeMonthsFromHleInterface(string refno, string nric, int docAppId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT Inc1Date, Inc1, Inc2Date, Inc2, Inc3Date, Inc3, Inc4Date, Inc4, Inc5Date, Inc5, Inc6Date, Inc6, Inc7Date, Inc7, Inc8Date, Inc8, ");
        //    sqlStatement.Append("Inc9Date, Inc9, Inc10Date, Inc10, Inc11Date, Inc11, Inc12Date, Inc12 FROM HleInterface a INNER JOIN AppPersonal b ON a.nric = b.nric ");
        //    sqlStatement.Append("INNER JOIN DocApp c ON c.id = b.DocAppId WHERE a.nric = @Nric AND a.HleNumber = @RefNo AND b.DocAppId = @DocAppId ");

        //    command.Parameters.Add("@RefNo", SqlDbType.VarChar);
        //    command.Parameters["@RefNo"].Value = refno;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

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

        public static DataTable GetIncomeMonthsFromHleInterface(string refno, string nric, int docAppId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@RefNo", SqlDbType.VarChar);
            command.Parameters["@RefNo"].Value = refno;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_GetIncomeMonthsFromHleInterface";
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

        #region Added this method to accommodate Resale by Edward 29/01/2014 Sales and Resale Changes
        public static DataTable GetIncomeMonthsFromResaleInterface(string refno, string nric, int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Inc1Date, Inc1, Inc2Date, Inc2, Inc3Date, Inc3, Inc4Date, Inc4, Inc5Date, Inc5, Inc6Date, Inc6, Inc7Date, Inc7, Inc8Date, Inc8, ");
            sqlStatement.Append("Inc9Date, Inc9, Inc10Date, Inc10, Inc11Date, Inc11, Inc12Date, Inc12, Inc13Date, Inc13, Inc14Date, Inc14, Inc15Date, Inc15 FROM ResaleInterface a INNER JOIN AppPersonal b ON a.nric = b.nric ");
            sqlStatement.Append("INNER JOIN DocApp c ON c.id = b.DocAppId WHERE a.nric = @Nric AND a.CaseNo = @RefNo AND b.DocAppId = @DocAppId ");

            command.Parameters.Add("@RefNo", SqlDbType.VarChar);
            command.Parameters["@RefNo"].Value = refno;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

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
        #endregion

        #region Added by Edward 11/3/2014 Sales and Resale Changes
        public static DataTable GetIncomeMonthsFromSalesInterface(string refno, string nric, int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Inc1Date, Inc1, Inc2Date, Inc2, Inc3Date, Inc3, Inc4Date, Inc4, Inc5Date, Inc5, Inc6Date, Inc6, Inc7Date, Inc7, Inc8Date, Inc8, ");
            sqlStatement.Append("Inc9Date, Inc9, Inc10Date, Inc10, Inc11Date, Inc11, Inc12Date, Inc12 FROM SalesInterface a INNER JOIN AppPersonal b ON a.nric = b.nric ");
            sqlStatement.Append("INNER JOIN DocApp c ON c.id = b.DocAppId WHERE a.nric = @Nric AND a.RegistrationNo = @RefNo AND b.DocAppId = @DocAppId ");

            command.Parameters.Add("@RefNo", SqlDbType.VarChar);
            command.Parameters["@RefNo"].Value = refno;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

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
        #endregion

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Mostly used in Assessment Period to get only the Month and Year
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public static DataTable GetAssessmentPeriod(int docAppId, string nric)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();
  
        //    sqlStatement.Append("SELECT DISTINCT   ");
        //    sqlStatement.Append("Income.Id , ");
        //    sqlStatement.Append("DateName( month , DateAdd( month , Income.IncomeMonth , 0 ) - 1 ) + ' ' + CAST(Income.IncomeYear AS VARCHAR)  AS MonthYear,  ");
        //    sqlStatement.Append("Income.IncomeMonth, ");
        //    sqlStatement.Append("Income.IncomeYear, AppPersonal.MonthsToLEAS, AppPersonal.AssessmentNA ");            
        //    sqlStatement.Append("FROM Income ");
        //    sqlStatement.Append("LEFT JOIN IncomeVersion ON Income.Id = IncomeVersion.IncomeId ");
        //    sqlStatement.Append("LEFT JOIN IncomeDetails ON IncomeVersion.Id = IncomeDetails.IncomeVersionID ");
        //    sqlStatement.Append("LEFT JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric))");            
        //    sqlStatement.Append(" ORDER BY Income.IncomeYear ASC, Income.IncomeMonth ASC");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

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

        public static DataTable GetAssessmentPeriod(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_GetAssessmentPeriod";
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
        /// Get the records per Income. This is used to group the Documents in a single Income in the table
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <param name="incomeId"></param>
        /// <returns></returns>
        //public static DataTable GetDocsByIncomeId(int docAppId, string nric, int incomeId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT Doc.Id AS DocId, Income.Id AS 'IncomeId', Doc.ImageAccepted, DocType.Description, AppDocRef.Id AS AppDocRefId,  ");
        //    sqlStatement.Append("MetaData.FieldValue AS 'EndDate', AppPersonal.Nric, AppPersonal.Id AS AppPersonalId, CONVERT(DATETIME,FieldValue,105)  ");
        //    sqlStatement.Append("FROM Income LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");            
        //    sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");            
        //    sqlStatement.Append("LEFT OUTER JOIN DocSet ON DocSet.Id = Doc.DocSetId LEFT OUTER JOIN SetApp ON SetApp.DocSetId = DocSet.Id LEFT OUTER JOIN DocApp ON DocApp.Id = SetApp.DocAppId ");  //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) AND MetaData.FieldName LIKE 'EndDate' AND Doctype.DocumentId <> '000028' ");
        //    //sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeMonth AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeYear) ");
        //    sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeMonth FROM Income I WHERE I.Id = Income.Id) AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeYear FROM Income I WHERE I.Id = Income.Id)) ");          
        //    sqlStatement.Append("AND Income.Id = @IncomeId ");
        //    sqlStatement.Append("AND (CONVERT(DATETIME,DocSet.VerificationDateIn,105) > CASE WHEN ISDATE(DocApp.SecondCADate) = 1 THEN CONVERT(DATETIME,DocApp.SecondCADate,105) ELSE '1/1/1900' END) "); //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting

        //    #region Added by Edward 21/01/2014 to Add IRAS Notice of Assessment
        //    sqlStatement.Append("UNION ");
        //    sqlStatement.Append("SELECT Doc.Id AS DocId, Income.Id AS 'IncomeId', Doc.ImageAccepted, DocType.Description, AppDocRef.Id AS AppDocRefId,  ");
        //    sqlStatement.Append("MetaData.FieldValue AS 'EndDate', AppPersonal.Nric, AppPersonal.Id AS AppPersonalId, CONVERT(DATETIME,FieldValue,105)  ");
        //    sqlStatement.Append("FROM Income LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");            
        //    sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocSet ON DocSet.Id = Doc.DocSetId LEFT OUTER JOIN SetApp ON SetApp.DocSetId = DocSet.Id LEFT OUTER JOIN DocApp ON DocApp.Id = SetApp.DocAppId ");  //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) AND Doctype.DocumentId = '000028' AND MetaData.FieldName LIKE 'DateOfFiling' ");
        //    //sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeMonth AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeYear) ");
        //    sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeMonth FROM Income I WHERE I.Id = Income.Id) AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeYear FROM Income I WHERE I.Id = Income.Id)) ");
        //    sqlStatement.Append("AND Income.Id = @IncomeId ");
        //    sqlStatement.Append("AND (CONVERT(DATETIME,DocSet.VerificationDateIn,105) > CASE WHEN ISDATE(DocApp.SecondCADate) = 1 THEN CONVERT(DATETIME,DocApp.SecondCADate,105) ELSE '1/1/1900' END) "); //Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
        //    sqlStatement.Append("ORDER BY CONVERT(DATETIME,FieldValue,105) DESC ");
        //    //sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode LIKE 'Payslip%' THEN 100 WHEN DocTypeCode LIKE 'CPF%' THEN 99 ");
        //    //sqlStatement.Append("WHEN DocTypeCode LIKE 'BankStatement%' THEN 98 ELSE 0 END DESC, CONVERT(DATETIME,FieldValue,105) DESC ");
        //    #endregion
           
        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

        //    command.Parameters.Add("@IncomeId", SqlDbType.Int);
        //    command.Parameters["@IncomeId"].Value = incomeId;

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

        public static DataTable GetDocsByIncomeId(int docAppId, string nric, int incomeId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_GetDocsByIncomeId";
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
        /// This will get all Docs by DocAppId and Nric
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public static DataTable GetDocsByDocAppIdAndNric(int docAppId, string nric)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT Doc.Id AS DocId, Income.Id AS 'IncomeId',   ");
        //    sqlStatement.Append("Doc.ImageAccepted, ");
        //    sqlStatement.Append("DocType.Description, AppDocRef.Id AS AppDocRefId, ");
        //    sqlStatement.Append("MetaData.FieldValue AS 'EndDate', AppPersonal.Nric  ");
        //    sqlStatement.Append("FROM Income ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code ");
        //    sqlStatement.Append("LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) AND MetaData.FieldName LIKE 'EndDate' ");
        //    //sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeMonth AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeYear) ");            
        //    sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeMonth FROM Income I WHERE I.Id = Income.Id)  ");
        //    sqlStatement.Append("AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeYear FROM Income I WHERE I.Id = Income.Id))  ");
        //    sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode LIKE 'Payslip%' THEN 100 WHEN DocTypeCode LIKE 'CPF%' THEN 99 ");
        //    sqlStatement.Append("WHEN DocTypeCode LIKE 'BankStatement%' THEN 98 ELSE 0 END DESC, CONVERT(DATETIME,FieldValue,105) DESC ");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;


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

        public static DataTable GetDocsByDocAppIdAndNric(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_GetDocsByDocAppIdAndNric";
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

        public static DataTable GetDocsByIncomeId(int docAppId, string nric, int incomeId, int appDocrefId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Doc.Id AS DocId, Income.Id AS 'IncomeId',   ");
            sqlStatement.Append("Doc.ImageAccepted, ");
            sqlStatement.Append("DocType.Description, AppDocRef.Id AS AppDocRefId, ");
            sqlStatement.Append("MetaData.FieldValue AS 'EndDate', AppPersonal.Nric  ");
            sqlStatement.Append("FROM Income ");
            sqlStatement.Append("LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");
            sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code ");
            sqlStatement.Append("LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");
            sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) AND MetaData.FieldName LIKE 'EndDate' ");
            sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeMonth AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeYear) ");
            sqlStatement.Append("AND Income.Id = @IncomeId AND AppDocRef.Id = @AppDocRefId ");
            sqlStatement.Append("ORDER BY CASE WHEN DocTypeCode LIKE 'Payslip%' THEN 100 WHEN DocTypeCode LIKE 'CPF%' THEN 99 ");
            sqlStatement.Append("WHEN DocTypeCode LIKE 'BankStatement%' THEN 98 ELSE 0 END DESC, CONVERT(DATETIME,FieldValue,105) DESC ");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            command.Parameters.Add("@AppDocRefId", SqlDbType.Int);
            command.Parameters["@AppDocRefId"].Value = appDocrefId;

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
        /// Checks if Income is Complete per Nric
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public static bool CheckIncomeCompleteByNRIC(string nric, int appPersonalId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ISNULL(SUM(x.ctr),-1) AS 'SumOfIncome',   ");
        //    sqlStatement.Append("(SELECT COUNT(*) FROM Income a INNER JOIN AppPersonal b ON a.AppPersonalId = b.Id  ");
        //    sqlStatement.Append("WHERE b.Nric = @nric AND b.Id = @AppPersonalId) AS 'CountOfIncome'  ");
        //    sqlStatement.Append("FROM (SELECT CASE WHEN COUNT(c.Id) > 0 THEN 1  ");
        //    sqlStatement.Append("WHEN COUNT(c.Id) = 0 AND a.IsSetToBlank = 1 THEN 1  ");
        //    sqlStatement.Append("ELSE 0 END AS 'ctr', a.Id FROM Income a  ");
        //    sqlStatement.Append("LEFT JOIN IncomeVersion b ON a.IncomeVersionId = b.Id ");
        //    sqlStatement.Append("LEFT JOIN IncomeDetails c ON c.IncomeVersionID = b.Id ");
        //    sqlStatement.Append("LEFT JOIN AppPersonal d ON d.Id = a.AppPersonalId ");
        //    sqlStatement.Append("WHERE d.Nric = @nric AND d.Id = @AppPersonalId GROUP BY a.Id, a.IsSetToBlank) x ");

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

        //    command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
        //    command.Parameters["@AppPersonalId"].Value = appPersonalId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        bool CanConfirm = true;
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        //checks if the SumOfIncome is negative. means that it doesnt have any IncomeMonths. It wont allow the user to extract
        //        if (int.Parse(dataSet.Tables[0].Rows[0]["SumOfIncome"].ToString()) < 0)
        //            return false;

        //        if (int.Parse(dataSet.Tables[0].Rows[0]["SumOfIncome"].ToString()) != int.Parse(dataSet.Tables[0].Rows[0]["CountOfIncome"].ToString()))
        //            CanConfirm = false;
                
        //        return CanConfirm;
        //    }
        //}

        public static bool CheckIncomeCompleteByNRIC(string nric, int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                bool CanConfirm = true;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Income_CheckIncomeCompleteByNRIC";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                //checks if the SumOfIncome is negative. means that it doesnt have any IncomeMonths. It wont allow the user to extract
                if (int.Parse(dataSet.Tables[0].Rows[0]["SumOfIncome"].ToString()) < 0)
                    return false;

                if (int.Parse(dataSet.Tables[0].Rows[0]["SumOfIncome"].ToString()) != int.Parse(dataSet.Tables[0].Rows[0]["CountOfIncome"].ToString()))
                    CanConfirm = false;

                return CanConfirm;
            }
        }
        #endregion

        public static bool CheckCreditAssessmentByAppPersonalId(int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            DataTable dt = GetDistinctIncomeItemByAppPersonalId(appPersonalId);
            int dtRowsCount = dt.Rows.Count;

            sqlStatement.Append("SELECT DISTINCT IncomeItem, IncomeType FROM CreditAssessment WHERE AppPersonalID = @AppPersonalId  ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;
           
            using (SqlConnection connection = new SqlConnection(connString))
            {                
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                int ctr = 0;
  
                foreach (DataRow r in dt.Rows)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        if (r["IncomeItem"].ToString().Trim().ToLower() == row["IncomeItem"].ToString().Trim().ToLower() &&
                            r["IncomeType"].ToString().Trim().ToLower() == row["IncomeType"].ToString().Trim().ToLower())
                        {
                            ctr++;
                            break;
                        }
                    }
                }

                if (ctr >= dtRowsCount)
                    return true;                                    

                return false;
            }
        }

        public static int GetNoofNotAcceptedRecordsPerNRIC(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT COUNT(DISTINCT Doc.Id) AS 'NoOfNotAccepted' ");
            sqlStatement.Append("FROM Income ");
            sqlStatement.Append("LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");
            sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code ");
            sqlStatement.Append("LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");
            sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) AND MetaData.FieldName LIKE 'EndDate' ");
            //sqlStatement.Append(" AND LTRIM(RTRIM(MetaData.FieldValue)) <> ''   ");
            //sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeMonth FROM Income I WHERE I.Id = Income.Id) AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = (SELECT I.IncomeYear FROM Income I WHERE I.Id = Income.Id)) ");
            sqlStatement.Append("AND Doc.ImageAccepted <> 'Y'");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

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
                return Convert.ToInt16(dataSet.Tables[0].Rows[0]["NoOfNotAccepted"]);
            }
        }

        /// <summary>
        /// Gets the number of Income Details
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <param name="incomeId"></param>
        /// <returns></returns>
        public static int GetNoofIncomeDetailsPerIncome(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            #region Commented by Edward 06.01.2014
            //sqlStatement.Append("SELECT COUNT(a.Id) AS 'NoOfNotAccepted' ");
            //sqlStatement.Append("FROM IncomeDetails a ");
            //sqlStatement.Append("INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");
            //sqlStatement.Append("INNER JOIN Income c ON c.Id = b.IncomeId ");
            //sqlStatement.Append("WHERE c.Id = @IncomeId ");
            #endregion

            sqlStatement.Append("SELECT CASE  ");
            sqlStatement.Append("WHEN COUNT(c.Id) > 0 THEN COUNT(c.Id) ");
            sqlStatement.Append("WHEN COUNT(c.Id) = 0 AND IsSetToBlank = 1 THEN 1 ");
            sqlStatement.Append("ELSE 0 END AS 'NoOfNotAccepted'  ");
            sqlStatement.Append("FROM Income a ");
            sqlStatement.Append("LEFT JOIN IncomeVersion b ON a.IncomeVersionId = b.Id ");
            sqlStatement.Append("LEFT JOIN IncomeDetails c ON c.IncomeVersionID = b.Id  ");
            sqlStatement.Append("WHERE a.Id = @IncomeId GROUP BY IsSetToBlank ");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return Convert.ToInt16(dataSet.Tables[0].Rows[0]["NoOfNotAccepted"]);
            }

        }
        #region commented
        //public static int GetNoofNotAcceptedRecordsPerIncome(int docAppId, string nric, int incomeId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT COUNT(DISTINCT Doc.Id) AS 'NoOfNotAccepted' ");
        //    sqlStatement.Append("FROM Income ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id ");
        //    sqlStatement.Append("LEFT OUTER JOIN Doc ON Doc.Id = AppDocRef.DocId ");
        //    sqlStatement.Append("LEFT OUTER JOIN DocType ON Doc.DocTypeCode= DocType.Code ");
        //    sqlStatement.Append("LEFT OUTER JOIN MetaData ON Metadata.Doc = Doc.Id ");
        //    sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND MetaData.FieldName LIKE 'EndDate' ");
        //    sqlStatement.Append("AND (DATEPART(MONTH,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeMonth AND DATEPART(YEAR,CONVERT(DATETIME,FieldValue,105)) = Income.IncomeYear) ");
        //    sqlStatement.Append("AND Income.Id = @IncomeId AND Doc.IsAccepted = 0");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@nric", SqlDbType.VarChar);
        //    command.Parameters["@nric"].Value = nric;

        //    command.Parameters.Add("@IncomeId", SqlDbType.Int);
        //    command.Parameters["@IncomeId"].Value = incomeId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return Convert.ToInt16(dataSet.Tables[0].Rows[0]["NoOfNotAccepted"]);
        //    }

        //}
        #endregion

        public static DataTable GetApplicantDetails(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT  ");
            sqlStatement.Append("AppPersonal.Name,  AppPersonal.PersonalType, AppPersonal.OrderNo, AppPersonal.Nric, AppPersonal.DocAppId ");            
            sqlStatement.Append("FROM AppDocRef INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");            
            sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) ");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            //command.Parameters.Add("@appDocRefId", SqlDbType.Int);
            //command.Parameters["@appDocRefId"].Value = appDocRefId;

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

        public static DataTable GetIncomeDataById(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT Id, IncomeVersionId, IncomeMonth, IncomeYear, Currency, CurrencyRate FROM Income WHERE Id = @IncomeId";

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetIncomeDataByAppPersonalId(int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT Id, IncomeVersionId, IncomeMonth, IncomeYear, Currency, CurrencyRate FROM Income WHERE AppPersonalId = @AppPersonalId";

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetIncomeVersionByIncome(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT Id, IncomeId, VersionName, VersionNo FROM IncomeVersion WHERE IncomeId = @IncomeId ORDER BY VersionNo DESC";

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        #region Added By Edward 29/01/2014 Sales and Resales Changes
        public static DataTable GetIncomeVersionByIncomeId(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT a.Id, a.IncomeId, a.VersionName, a.VersionNo, a.DateEntered, b.Name ");
            sqlStatement.Append("FROM IncomeVersion a LEFT JOIN Profile b ON a.EnteredBy = b.UserId WHERE IncomeId = @IncomeId ORDER BY VersionNo DESC ");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

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

        public static DataTable GetIncomeByAppPersonalIdByMonthByYear(int AppPersonalId, int Month, int Year)
        {
            SqlCommand command = new SqlCommand();

            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT Id, IncomeVersionId, IncomeMonth, IncomeYear, Currency, CurrencyRate FROM Income b ");          
            sqlStatement.Append("WHERE b.AppPersonalId = @AppPersonalId AND b.IncomeMonth = @Month AND b.IncomeYear = @Year   ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = AppPersonalId;

            command.Parameters.Add("@Month", SqlDbType.Int);
            command.Parameters["@Month"].Value = Month;

            command.Parameters.Add("@Year", SqlDbType.Int);
            command.Parameters["@Year"].Value = Year;

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

        #region commented
        //public static DataTable GetIncomeVersionByIncome(int incomeId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    string sqlStatement = "SELECT Id, IncomeId, VersionName, VersionNo FROM IncomeVersion WHERE IncomeId = @IncomeId ORDER BY VersionNo DESC";

        //    command.Parameters.Add("@IncomeId", SqlDbType.Int);
        //    command.Parameters["@IncomeId"].Value = incomeId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement;
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}
        #endregion

        public static DataTable GetIncomeDetailsByIncomeVersion(int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT Id , IncomeVersionId  AS 'IncomeVersionId', IncomeItem,  ");
            sqlStatement.Append("LTRIM(RTRIM(CAST(IncomeAmount AS NVARCHAR(10)))) AS IncomeAmount, ");
            sqlStatement.Append("CASE WHEN Allowance = 1 THEN 'true' ELSE 'false' END AS 'Allowance',   ");
            sqlStatement.Append("CASE WHEN CPFIncome = 1 THEN 'true' ELSE 'false' END AS 'CPFIncome',  ");
            sqlStatement.Append("CASE WHEN GrossIncome = 1 THEN 'true' ELSE 'false' END AS 'GrossIncome',   ");
            sqlStatement.Append("CASE WHEN AHGIncome = 1 THEN 'true' ELSE 'false' END AS 'AHGIncome', ");
            sqlStatement.Append("CASE WHEN Overtime = 1 THEN 'true' ELSE 'false' END AS 'Overtime', ");
            sqlStatement.Append("OrderNo ");    //Added By Edward 31/01/2014    Sales and Resales Changes 
            sqlStatement.Append("FROM IncomeDetails WHERE IncomeVersionId = @IncomeVersionId  ");
            sqlStatement.Append("ORDER BY OrderNo  ");  //Added By Edward 31/01/2014    Sales and Resales Changes 

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = incomeVersionId;

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

        #region Gets the IncomeDetails by Order Nos and by Null Order No  Added By Edward 29/01/2014 Sales and Resales Changes
        public static DataTable GetIncomeDetailsOrderNos(int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT Id , OrderNo  ");
            sqlStatement.Append("FROM IncomeDetails WHERE IncomeVersionId = @IncomeVersionId ");
            sqlStatement.Append("AND OrderNo IS NOT NULL   ");
            sqlStatement.Append("UNION  ");
            sqlStatement.Append("SELECT Id , OrderNo   ");
            sqlStatement.Append("FROM IncomeDetails WHERE IncomeVersionId = @IncomeVersionId ");
            sqlStatement.Append("AND OrderNo IS NULL  ");

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = incomeVersionId;

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

        public static DataTable GetIncomeDetailTotals(int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            //ALL ColumnNames must be similiar to ColumnNames in the GetIncomeDetailsByIncomeVersion Method
            sqlStatement.Append("SELECT DISTINCT  ");
            sqlStatement.Append("1, Income.Currency,  LTRIM(RTRIM(CAST(Income.CurrencyRate AS NVARCHAR(10)))) AS 'IncomeItem', Income.Currency  + ' Total' AS 'IncomeAmount' ,");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / 1 FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND Allowance = 1) AS 'Allowance',  ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / 1 FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND CPFIncome = 1) AS 'CPFIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / 1 FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND GrossIncome = 1) AS 'GrossIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / 1 FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND AHGIncome = 1) AS 'AHGIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / 1 FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND Overtime = 1) AS 'Overtime' ");
            sqlStatement.Append("FROM IncomeDetails  ");
            sqlStatement.Append("INNER JOIN IncomeVersion ON IncomeDetails.IncomeVersionID = IncomeVersion.Id ");
            sqlStatement.Append("INNER JOIN Income ON IncomeVersion.IncomeId = Income.Id ");
            sqlStatement.Append("WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT DISTINCT  ");
            //"1 AS 'IncomeItem'" is really the currency rate of SGD to SGD
            sqlStatement.Append("2, 'SGD',  '1' AS 'IncomeItem' ,  'SGD' + ' / ' + LTRIM(RTRIM(CAST(Income.CurrencyRate AS NVARCHAR(10)))) + 'Total' AS 'IncomeAmount' , ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / Income.CurrencyRate FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND Allowance = 1) AS 'Allowance',  ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / Income.CurrencyRate FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND CPFIncome = 1) AS 'CPFIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / Income.CurrencyRate FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND GrossIncome = 1) AS 'GrossIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / Income.CurrencyRate FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND AHGIncome = 1) AS 'AHGIncome', ");
            sqlStatement.Append("(SELECT SUM(IncomeAmount) / Income.CurrencyRate FROM IncomeDetails WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId AND Overtime = 1) AS 'Overtime' ");
            sqlStatement.Append("FROM IncomeDetails  ");
            sqlStatement.Append("INNER JOIN IncomeVersion ON IncomeDetails.IncomeVersionID = IncomeVersion.Id ");
            sqlStatement.Append("INNER JOIN Income ON IncomeVersion.IncomeId = Income.Id ");
            sqlStatement.Append("WHERE IncomeDetails.IncomeVersionId = @IncomeVersionId ");

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = incomeVersionId;

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


        public static int DeleteIncomeItems(int[] ids)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM IncomeDetails WHERE Id IN (");
            foreach (int i in ids)
            {
                sqlStatement.Append(i + ",");
            }
            sqlStatement.Remove(sqlStatement.ToString().Length - 1, 1);
            sqlStatement.Append(")");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int InsertIncomeVersion(int incomeId, int versionNo, Guid enteredBy)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO IncomeVersion (IncomeId, VersionNo, EnteredBy, DateEntered) VALUES ");
            sqlStatement.Append("(@incomeId, @versionNo, @enteredBy,  GETDATE()); SELECT @@Identity");

            command.Parameters.Add("@incomeId", SqlDbType.Int);
            command.Parameters["@incomeId"].Value = incomeId;

            command.Parameters.Add("@versionNo", SqlDbType.Int);
            command.Parameters["@versionNo"].Value = versionNo;

            command.Parameters.Add("@enteredBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@enteredBy"].Value = enteredBy;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }

        public static int InsertIncomeDetails(int incomeVersionId, string incomeItem, decimal incomeAmount, bool Allowance, 
            bool CPFIncome, bool GrossIncome, bool AHGIncome, bool Overtime)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO IncomeDetails (IncomeVersionID, IncomeItem, IncomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime) VALUES ");
            sqlStatement.Append("(@IncomeVersionID, @IncomeItem, @IncomeAmount, @Allowance, @CPFIncome, @GrossIncome, @AHGIncome, @Overtime)");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeItem", SqlDbType.NVarChar);
            command.Parameters["@IncomeItem"].Value = incomeItem;

            command.Parameters.Add("@IncomeAmount", SqlDbType.Decimal);
            command.Parameters["@IncomeAmount"].Value = incomeAmount;

            command.Parameters.Add("@Allowance", SqlDbType.Bit);
            command.Parameters["@Allowance"].Value = Allowance;

            command.Parameters.Add("@CPFIncome", SqlDbType.Bit);
            command.Parameters["@CPFIncome"].Value = CPFIncome;

            command.Parameters.Add("@GrossIncome", SqlDbType.Bit);
            command.Parameters["@GrossIncome"].Value = GrossIncome;

            command.Parameters.Add("@AHGIncome", SqlDbType.Bit);
            command.Parameters["@AHGIncome"].Value = AHGIncome;

            command.Parameters.Add("@Overtime", SqlDbType.Bit);
            command.Parameters["@Overtime"].Value = Overtime;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncomeDetails(int IncomeDetailsId, int incomeVersionId, string incomeItem, decimal incomeAmount, bool Allowance,
            bool CPFIncome, bool GrossIncome, bool AHGIncome, bool Overtime)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE IncomeDetails SET IncomeVersionID = @IncomeVersionID, IncomeItem = @IncomeItem, IncomeAmount = @IncomeAmount, Allowance = @Allowance, ");
            sqlStatement.Append("CPFIncome = @CPFIncome, GrossIncome = @GrossIncome, AHGIncome = @AHGIncome, Overtime = @Overtime WHERE Id = @IncomeDetailsId");

            command.Parameters.Add("@IncomeDetailsId", SqlDbType.Int);
            command.Parameters["@IncomeDetailsId"].Value = IncomeDetailsId;

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeItem", SqlDbType.NVarChar);
            command.Parameters["@IncomeItem"].Value = incomeItem;

            command.Parameters.Add("@IncomeAmount", SqlDbType.Decimal);
            command.Parameters["@IncomeAmount"].Value = incomeAmount;

            command.Parameters.Add("@Allowance", SqlDbType.Bit);
            command.Parameters["@Allowance"].Value = Allowance;

            command.Parameters.Add("@CPFIncome", SqlDbType.Bit);
            command.Parameters["@CPFIncome"].Value = CPFIncome;

            command.Parameters.Add("@GrossIncome", SqlDbType.Bit);
            command.Parameters["@GrossIncome"].Value = GrossIncome;

            command.Parameters.Add("@AHGIncome", SqlDbType.Bit);
            command.Parameters["@AHGIncome"].Value = AHGIncome;

            command.Parameters.Add("@Overtime", SqlDbType.Bit);
            command.Parameters["@Overtime"].Value = Overtime;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncomeVersion(int incomeId, int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE Income SET IncomeVersionId = @IncomeVersionId WHERE Id = @IncomeId");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        #region UpdateIncomeVersion Added By Edward 29/01/2014 Sales and Resales Changes
        // this method updates the EnteredBy Field when user clicks the save button in the zoning page
        public static int UpdateIncomeVersion(int Id, Guid enteredBy)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE IncomeVersion SET EnteredBy  = @EnteredBy, DateEntered = GETDATE() WHERE Id = @IncomeVersionId");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = Id;

            command.Parameters.Add("@EnteredBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@EnteredBy"].Value = enteredBy;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncomeVersion(int Id, Guid enteredBy, string VersionName)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE IncomeVersion SET EnteredBy  = @EnteredBy, VersionName = @VersionName, DateEntered = GETDATE() WHERE Id = @IncomeVersionId");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = Id;

            command.Parameters.Add("@EnteredBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@EnteredBy"].Value = enteredBy;

            command.Parameters.Add("@VersionName", SqlDbType.VarChar);
            command.Parameters["@VersionName"].Value = VersionName;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        #endregion



        public static int UpdateIncomeSetToBlank(int incomeId, bool setToBlank)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE Income SET IsSetToBlank = @SetToBlank WHERE Id = @IncomeId");

            command.Parameters.Add("@SetToBlank", SqlDbType.Bit);
            command.Parameters["@SetToBlank"].Value = setToBlank;

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int InsertIncome(int incomeVersionId, int incomeMonth, int incomeYear,
            string currency, decimal currencyRate, Guid? enteredBy, int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO Income (IncomeVersionID, IncomeMonth, IncomeYear, Currency, CurrencyRate, EnteredBy, DateEntered, AppPersonalId) VALUES ");
            sqlStatement.Append("(@IncomeVersionID, @IncomeMonth, @IncomeYear, @Currency, @CurrencyRate, @EnteredBy, GETDATE(), @AppPersonalId);SELECT @@Identity");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeMonth", SqlDbType.Int);
            command.Parameters["@IncomeMonth"].Value = incomeMonth;

            command.Parameters.Add("@IncomeYear", SqlDbType.Int);
            command.Parameters["@IncomeYear"].Value = incomeYear;

            command.Parameters.Add("@Currency", SqlDbType.VarChar);
            command.Parameters["@Currency"].Value = currency;

            command.Parameters.Add("@CurrencyRate", SqlDbType.Decimal);
            command.Parameters["@CurrencyRate"].Value = currencyRate;

            command.Parameters.Add("@EnteredBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@EnteredBy"].Value = enteredBy;

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }


        public static int InsertIncome(int incomeVersionId, int incomeMonth, int incomeYear,
            string currency, decimal currencyRate, int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO Income (IncomeVersionID, IncomeMonth, IncomeYear, Currency, CurrencyRate, DateEntered, AppPersonalId) VALUES ");
            sqlStatement.Append("(@IncomeVersionID, @IncomeMonth, @IncomeYear, @Currency, @CurrencyRate, GETDATE(), @AppPersonalId);SELECT @@Identity");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeMonth", SqlDbType.Int);
            command.Parameters["@IncomeMonth"].Value = incomeMonth;

            command.Parameters.Add("@IncomeYear", SqlDbType.Int);
            command.Parameters["@IncomeYear"].Value = incomeYear;

            command.Parameters.Add("@Currency", SqlDbType.VarChar);
            command.Parameters["@Currency"].Value = currency;

            command.Parameters.Add("@CurrencyRate", SqlDbType.Decimal);
            command.Parameters["@CurrencyRate"].Value = currencyRate;

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return int.Parse(command.ExecuteScalar().ToString());
            }
        }

        public static int DeleteAllIncomeDetailsByVersionId(int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "DELETE IncomeDetails WHERE IncomeVersionId = @IncomeVersionId";
            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = incomeVersionId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }

        }

        public static int DeleteAllIncomeVersionsByIncomeId(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "DELETE IncomeVersion WHERE IncomeId = @IncomeId";
            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }


        public static int DeleteIncomeByIncomeId(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "DELETE Income WHERE Id = @IncomeId";
            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }


        public static int DeleteAllIncomeByDocAppIdAndNRIC(int docAppId, string Nric)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "DELETE a FROM Income a INNER JOIN AppPersonal b ON a.AppPersonalId = b.Id WHERE b.DocAppId = @DocAppId AND LTRIM(RTRIM(b.Nric)) = LTRIM(RTRIM(@Nric))";

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = Nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncome(int IncomeId, decimal CurrencyRate, string Currency)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE Income SET Currency = @Currency, CurrencyRate = @CurrencyRate WHERE Id = @IncomeId");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = IncomeId;

            command.Parameters.Add("@Currency", SqlDbType.VarChar);
            command.Parameters["@Currency"].Value = Currency;

            command.Parameters.Add("@CurrencyRate", SqlDbType.Decimal);
            command.Parameters["@CurrencyRate"].Value = CurrencyRate;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static DataTable GetAHGFromIncomeTableByAppPersonalId(int appPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT ISNULL(SUM(c.IncomeAmount),0) AS AHGIncomeTotal ,Count(Distinct(IncomeMonth)) AS NoOfMonths,  ");
            sqlStatement.Append("ISNULL(ROUND(SUM(c.IncomeAmount) / Count(Distinct(IncomeMonth)),2),0) AS AHGAverage ");
            sqlStatement.Append("FROM Income a   ");
            sqlStatement.Append("Inner join IncomeVersion b on a.IncomeVersionId = b.Id  ");
            sqlStatement.Append("inner join IncomeDetails c on c.IncomeVersionID = b.Id   ");
            sqlStatement.Append("where c.AHGIncome = 1 and a.AppPersonalId = @AppPersonalId ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;

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

        #region GetOrderByMonthYear
        /// <summary>
        /// Gets the Order of Month Year
        /// </summary>
        /// <param name="docAppId">DocAppId in AppPersonal</param>
        /// <param name="nric">NRIC of Person in AppPersonal</param>
        /// <param name="sortBy">DESC / ASC</param>
        /// <returns></returns>
        public static DataTable GetOrderByMonthYear(int docAppId, string nric, string sortBy)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT   ");
            sqlStatement.Append("Income.Id , DateName( month , DateAdd( month , Income.IncomeMonth , 0 ) - 1 ) + ' ' + CAST(Income.IncomeYear AS VARCHAR)  AS MonthYear, ");
            sqlStatement.Append("Income.IncomeMonth, Income.IncomeYear, Income.IncomeVersionId  ");
            sqlStatement.Append("FROM Income ");
            sqlStatement.Append("LEFT JOIN IncomeVersion ON Income.Id = IncomeVersion.IncomeId ");
            sqlStatement.Append("LEFT JOIN IncomeDetails ON IncomeVersion.Id = IncomeDetails.IncomeVersionID ");
            sqlStatement.Append("LEFT JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
            #region commented by Edward 06.01.2014
            //sqlStatement.Append("LEFT JOIN AppDocRef ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
            //sqlStatement.Append("LEFT JOIN Doc ON AppDocRef.DocId = Doc.Id ");
            //sqlStatement.Append("LEFT JOIN DocType ON Doc.DocTypeCode = Doctype.Code ");
            #endregion
            sqlStatement.Append("WHERE AppPersonal.DocAppId = @docAppId AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM(@Nric)) "); 

            if(sortBy == "DESC")
                sqlStatement.Append("ORDER BY Income.IncomeYear DESC, Income.IncomeMonth DESC ");
            else
                sqlStatement.Append("ORDER BY Income.IncomeYear , Income.IncomeMonth  ");

            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

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
        /// <summary>
        /// Gets the Order of Month Year
        /// </summary>
        /// <param name="appPersonalId">Id from AppPersonal Table</param>
        /// <param name="sortBy">DESC or ASC</param>
        /// <returns></returns>
        public static DataTable GetOrderByMonthYear(int appPersonalId, string sortBy)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT   ");
            sqlStatement.Append("Income.Id , ");
            sqlStatement.Append("DateName( month , DateAdd( month , Income.IncomeMonth , 0 ) - 1 ) + ' ' + CAST(Income.IncomeYear AS VARCHAR)  AS MonthYear,  ");
            sqlStatement.Append("Income.IncomeMonth, ");
            sqlStatement.Append("Income.IncomeYear ");
            sqlStatement.Append("FROM Income ");
            sqlStatement.Append("LEFT JOIN IncomeVersion ON Income.Id = IncomeVersion.IncomeId ");
            sqlStatement.Append("LEFT JOIN IncomeDetails ON IncomeVersion.Id = IncomeDetails.IncomeVersionID ");
            sqlStatement.Append("LEFT JOIN AppPersonal ON Income.AppPersonalId = AppPersonal.Id ");
            sqlStatement.Append("WHERE AppPersonal.Id = @AppPersonalId ");

            if (sortBy == "DESC")
                sqlStatement.Append("ORDER BY Income.IncomeYear DESC, Income.IncomeMonth DESC ");
            else
                sqlStatement.Append("ORDER BY Income.IncomeYear , Income.IncomeMonth  ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = appPersonalId;            

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

        public static DataTable GetDistinctIncomeItemByAppPersonalId(int id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT   DISTINCT IncomeItem, CASE WHEN Allowance = 1 THEN 'Allowance' WHEN GrossIncome = 1 THEN 'Gross Income' ");
            sqlStatement.Append("WHEN Overtime = 1 THEN 'Overtime' WHEN AHGIncome = 1 THEN 'AHGIncome'  END AS IncomeType  ");
            sqlStatement.Append("FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id  ");
            sqlStatement.Append("INNER JOIN Income c ON c.IncomeVersionId = b.Id WHERE  c.AppPersonalId = @AppPersonalId ORDER BY IncomeItem");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = id;

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

        #region GetIncomeDetailsByIncomeIdAndIncomeItem
        /// <summary>
        /// Gets the Income Details by Income and IncomeItem
        /// </summary>
        /// <param name="IncomeId">IncomeId from Income</param>
        /// <param name="IncomeItem">IncomeItem</param>
        /// <returns></returns>
        public static DataTable GetIncomeDetailsByIncomeIdAndIncomeItem(int IncomeId, string IncomeItem)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  a.Id AS IncomeDetailsId, b.Id AS IncomeVersionId, c.Id AS IncomeId, a.IncomeItem, a.IncomeAmount, ");
            sqlStatement.Append("CASE WHEN Allowance = 1 THEN 'Allowance' WHEN GrossIncome = 1 THEN 'Gross Income' WHEN Overtime = 1 THEN 'Overtime' ");
            sqlStatement.Append("ELSE ' - ' END AS IncomeType , c.CurrencyRate   ");
            sqlStatement.Append("FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");
            sqlStatement.Append("INNER JOIN Income c ON c.IncomeVersionId = b.Id WHERE c.Id = @IncomeId and IncomeItem = @IncomeItem ");
            sqlStatement.Append("AND (Allowance = 1 OR GrossIncome = 1 OR Overtime = 1) ");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = IncomeId;

            command.Parameters.Add("@IncomeItem", SqlDbType.VarChar);
            command.Parameters["@IncomeItem"].Value = IncomeItem;

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
        /// Get the Income Details by the IncomeId, IncomeItem and IncomeType
        /// </summary>
        /// <param name="IncomeId">IncomeId from Income</param>
        /// <param name="IncomeItem">IncomeItem</param>
        /// <param name="IncomeType">GrossIncome, Allowance or Overtime</param>
        /// <returns></returns>
        public static DataTable GetIncomeDetailsByIncomeIdAndIncomeItem(int IncomeId, string IncomeItem, string IncomeType)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT  a.Id AS IncomeDetailsId, b.Id AS IncomeVersionId, c.Id AS IncomeId, a.IncomeItem, a.IncomeAmount, ");
            sqlStatement.Append("CASE WHEN Allowance = 1 THEN 'Allowance' WHEN GrossIncome = 1 THEN 'Gross Income' WHEN Overtime = 1 THEN 'Overtime' WHEN AHGIncome = 1 THEN 'AHGIncome' ");
            sqlStatement.Append("ELSE ' - ' END AS IncomeType , c.CurrencyRate   ");
            sqlStatement.Append("FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");
            sqlStatement.Append("INNER JOIN Income c ON c.IncomeVersionId = b.Id WHERE c.Id = @IncomeId and IncomeItem = @IncomeItem ");
            if(IncomeType.ToUpper().Trim().Equals("OVERTIME"))
                sqlStatement.Append("AND Overtime = 1 ");
            else if(IncomeType.ToUpper().Trim().Equals("ALLOWANCE"))
                sqlStatement.Append("AND Allowance = 1 ");
            else if (IncomeType.ToUpper().Trim().Equals("AHGINCOME"))
                sqlStatement.Append("AND AHGIncome = 1 ");
            else 
                sqlStatement.Append("AND GrossIncome = 1 ");            

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = IncomeId;

            command.Parameters.Add("@IncomeItem", SqlDbType.VarChar);
            command.Parameters["@IncomeItem"].Value = IncomeItem;

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



        public static DataTable GetIncomeDetailsById(int id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM IncomeDetails WHERE Id = @Id");            

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
                return dataSet.Tables[0];
            }
        }

        #endregion

        public static int DeleteAllExtractionByAppPersonalId(int AppPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM CreditAssessment WHERE AppPersonalId = @AppPersonalId; ");

            sqlStatement.Append("DELETE FROM IncomeDetails WHERE IncomeVersionId IN ");
            sqlStatement.Append("(SELECT Id FROM IncomeVersion WHERE IncomeId IN  ");
            sqlStatement.Append("(SELECT Id FROM Income WHERE AppPersonalId = @AppPersonalId)); ");

            sqlStatement.Append("DELETE FROM IncomeVersion WHERE IncomeId IN ");
            sqlStatement.Append("(SELECT Id FROM Income WHERE AppPersonalId = @AppPersonalId); ");

            sqlStatement.Append("DELETE FROM Income WHERE AppPersonalId = @AppPersonalId ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = AppPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int DeleteCreditAssessment(int AppPersonalId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM CreditAssessment WHERE AppPersonalId = @AppPersonalId ");            

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = AppPersonalId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        #region Added By Edward 29/01/2014 Sales and Resales Changes
        public static int DeleteIncomeVersionById(int Id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM IncomeVersion WHERE Id = @Id ");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = Id;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        #endregion

        public static int DeleteCreditAssessment(int AppPersonalId, string IncomeItem, string IncomeType)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("DELETE FROM CreditAssessment WHERE AppPersonalId = @AppPersonalId AND LTRIM(RTRIM(IncomeItem)) = @IncomeItem AND LTRIM(RTRIM(IncomeType)) = @IncomeType ");

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = AppPersonalId;

            command.Parameters.Add("@IncomeItem", SqlDbType.VarChar);
            command.Parameters["@IncomeItem"].Value = IncomeItem;

            command.Parameters.Add("@IncomeType", SqlDbType.VarChar);
            command.Parameters["@IncomeType"].Value = IncomeType;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        #region Updates the Order No of Income Detail Added By Edward 31/01/2014 Sales and Resales Changes
        public static int UpdateIncomeDetailsOrderNo(int IncomeDetailsId, int OrderNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE IncomeDetails SET OrderNo = @OrderNo ");
            sqlStatement.Append("WHERE Id = @IncomeDetailsId");

            command.Parameters.Add("@IncomeDetailsId", SqlDbType.Int);
            command.Parameters["@IncomeDetailsId"].Value = IncomeDetailsId;

            command.Parameters.Add("@OrderNo", SqlDbType.Int);
            command.Parameters["@OrderNo"].Value = OrderNo;            

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }


        public static DataTable GetIncomeDetailOrderNoById(int Id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT Id , OrderNo  ");           
            sqlStatement.Append("FROM IncomeDetails WHERE Id = @Id  ");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = Id;

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

        //public static DataTable GetIncomeDetailOrderNoByIncomeVersionAndOrderNo(int IncomeVersionId, int OrderNo)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();
        //    sqlStatement.Append("SELECT Id , OrderNo  ");
        //    sqlStatement.Append("FROM IncomeDetails WHERE Id = @Id  ");

        //    command.Parameters.Add("@Id", SqlDbType.Int);
        //    command.Parameters["@Id"].Value = Id;

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

        public static int UpdateIncomeDetailsOrderNo(int IncomeDetailsId, string action, int IncomeVersionId, int? CurrentOrderNo )
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            int NewOrderNo = 0;
            int result = 0;
            if (action == "up")
            {
                if (CurrentOrderNo > 1)
                    NewOrderNo = CurrentOrderNo.Value - 1;
                else
                    NewOrderNo = CurrentOrderNo.Value;
            }
            else
            {
                DataTable dt = IncomeDs.GetIncomeDetailsByIncomeVersion(IncomeVersionId);
                if (CurrentOrderNo < dt.Rows.Count)
                    NewOrderNo = CurrentOrderNo.Value + 1;
                else
                    NewOrderNo = CurrentOrderNo.Value;
            }
            
            sqlStatement.Append("UPDATE IncomeDetails SET OrderNo = @CurrentOrderNo ");
            sqlStatement.Append("WHERE IncomeVersionId = @IncomeVersionId AND OrderNo = @NewOrderNo");

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = IncomeVersionId;

            command.Parameters.Add("@CurrentOrderNo", SqlDbType.Int);
            command.Parameters["@CurrentOrderNo"].Value = CurrentOrderNo;

            command.Parameters.Add("@NewOrderNo", SqlDbType.Int);
            command.Parameters["@NewOrderNo"].Value = NewOrderNo;
           
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                result = command.ExecuteNonQuery();
            }

            sqlStatement = new StringBuilder();
            command = new SqlCommand();

            sqlStatement.Append("UPDATE IncomeDetails SET OrderNo = @NewOrderNo ");
            sqlStatement.Append("WHERE Id = @Id");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = IncomeDetailsId;

            command.Parameters.Add("@NewOrderNo", SqlDbType.Int);
            command.Parameters["@NewOrderNo"].Value = NewOrderNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        #endregion

        #endregion

        #region Added By Edward Income Extraction Changes 2014/6/17     Add HLE Form in View Consolidated PDF in Income Extraction
        public static DataTable GetHLEFormByDocApp(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT a.Id AS DocId FROM Doc a INNER JOIN DocSet b ON a.DocSetId = b.Id ");
            sqlStatement.Append("INNER JOIN SetApp c ON c.DocSetId = b.Id  ");
            sqlStatement.Append("INNER JOIN DocApp d ON d.id = c.DocAppId  ");
            sqlStatement.Append("WHERE DocTypeCode = 'HLE' AND d.Id = @DocAppId ");

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
                return dataSet.Tables[0];
            }
        }
        #endregion

        #region Modifed by Edward 2014/07/22 Changes in Zoning Page
        public static DataTable GetIncomeDetailsByVersionNoAndIncomeId(int versionNo, int incomeId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT c.Id , b.Id  AS 'IncomeVersionId', IncomeItem, a.Id AS IncomeDetailsID,  ");
            sqlStatement.Append("LTRIM(RTRIM(CAST(IncomeAmount AS NVARCHAR(10)))) AS IncomeAmount, ");
            sqlStatement.Append("CASE WHEN Allowance = 1 THEN 'true' ELSE 'false' END AS 'Allowance',   ");
            sqlStatement.Append("CASE WHEN CPFIncome = 1 THEN 'true' ELSE 'false' END AS 'CPFIncome',  ");
            sqlStatement.Append("CASE WHEN GrossIncome = 1 THEN 'true' ELSE 'false' END AS 'GrossIncome',   ");
            sqlStatement.Append("CASE WHEN AHGIncome = 1 THEN 'true' ELSE 'false' END AS 'AHGIncome', ");
            sqlStatement.Append("CASE WHEN Overtime = 1 THEN 'true' ELSE 'false' END AS 'Overtime', ");
            sqlStatement.Append("OrderNo ");
            sqlStatement.Append("FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id INNER JOIN Income c ON b.IncomeId = c.Id ");
            sqlStatement.Append("WHERE b.VersionNo = @VersionNo AND c.Id = @IncomeId ");
            sqlStatement.Append("ORDER BY OrderNo  ");  

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

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

        public static DataTable GetIncomeVersionNoByDocAppIdAndNric(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();            
            StringBuilder sqlStatement = new StringBuilder();
            DataTable dt = GetSecondCAAndSecondCADate(docAppId);
            string secondCADate = string.Empty;
            bool secondCA = false;
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                secondCA = bool.Parse(r["SecondCA"].ToString());
                secondCADate = !r.IsNull("SecondCADate") ? r["SecondCADate"].ToString() : "01/01/0001";
            }

            sqlStatement.Append("SELECT DISTINCT a.VersionNo, ISNULL(a.VersionName,'') AS VersionName FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id ");
            sqlStatement.Append("WHERE d.Id = @DocAppId AND c.Nric = @Nric  ");
            if (secondCA)
            {
                sqlStatement.Append(" AND a.DateEntered > @SecondCADate  ");
                command.Parameters.Add("@SecondCADate", SqlDbType.DateTime);
                command.Parameters["@SecondCADate"].Value = secondCADate;
            }            

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

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

        private static DataTable GetSecondCAAndSecondCADate(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT SecondCA, SecondCADate FROM DocApp WHERE Id = @DocAppId");

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
                return dataSet.Tables[0];
            }
        }


        public static DataTable GetTopOneIncomeVersionByIncome(int incomeId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT TOP 1 Id, IncomeId, VersionName, VersionNo FROM IncomeVersion WHERE IncomeId = @IncomeId ORDER BY VersionNo DESC";

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static int InsertIncomeDetails(int incomeVersionId, string incomeItem, decimal incomeAmount, bool GrossIncome, bool Allowance,
            bool Overtime, bool AHGIncome, bool CPFIncome, int orderNo )
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("INSERT INTO IncomeDetails (IncomeVersionID, IncomeItem, IncomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime, OrderNo) VALUES ");
            sqlStatement.Append("(@IncomeVersionID, @IncomeItem, @IncomeAmount, @Allowance, @CPFIncome, @GrossIncome, @AHGIncome, @Overtime, @OrderNo)");

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeItem", SqlDbType.NVarChar);
            command.Parameters["@IncomeItem"].Value = incomeItem;

            command.Parameters.Add("@IncomeAmount", SqlDbType.Decimal);
            command.Parameters["@IncomeAmount"].Value = incomeAmount;

            command.Parameters.Add("@Allowance", SqlDbType.Bit);
            command.Parameters["@Allowance"].Value = Allowance;

            command.Parameters.Add("@CPFIncome", SqlDbType.Bit);
            command.Parameters["@CPFIncome"].Value = CPFIncome;

            command.Parameters.Add("@GrossIncome", SqlDbType.Bit);
            command.Parameters["@GrossIncome"].Value = GrossIncome;

            command.Parameters.Add("@AHGIncome", SqlDbType.Bit);
            command.Parameters["@AHGIncome"].Value = AHGIncome;

            command.Parameters.Add("@Overtime", SqlDbType.Bit);
            command.Parameters["@Overtime"].Value = Overtime;

            command.Parameters.Add("@OrderNo", SqlDbType.Int);
            command.Parameters["@OrderNo"].Value = orderNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncomeDetails(int IncomeDetailsId, int incomeVersionId, string incomeItem, decimal incomeAmount, bool GrossIncome, bool Allowance,
            bool Overtime, bool AHGIncome, bool CPFIncome, int OrderNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE IncomeDetails SET IncomeVersionID = @IncomeVersionID, IncomeItem = @IncomeItem, IncomeAmount = @IncomeAmount, Allowance = @Allowance, ");
            sqlStatement.Append("CPFIncome = @CPFIncome, GrossIncome = @GrossIncome, AHGIncome = @AHGIncome, Overtime = @Overtime, OrderNo = @OrderNo WHERE Id = @IncomeDetailsId");

            command.Parameters.Add("@IncomeDetailsId", SqlDbType.Int);
            command.Parameters["@IncomeDetailsId"].Value = IncomeDetailsId;

            command.Parameters.Add("@IncomeVersionID", SqlDbType.Int);
            command.Parameters["@IncomeVersionID"].Value = incomeVersionId;

            command.Parameters.Add("@IncomeItem", SqlDbType.NVarChar);
            command.Parameters["@IncomeItem"].Value = incomeItem;

            command.Parameters.Add("@IncomeAmount", SqlDbType.Decimal);
            command.Parameters["@IncomeAmount"].Value = incomeAmount;

            command.Parameters.Add("@Allowance", SqlDbType.Bit);
            command.Parameters["@Allowance"].Value = Allowance;

            command.Parameters.Add("@CPFIncome", SqlDbType.Bit);
            command.Parameters["@CPFIncome"].Value = CPFIncome;

            command.Parameters.Add("@GrossIncome", SqlDbType.Bit);
            command.Parameters["@GrossIncome"].Value = GrossIncome;

            command.Parameters.Add("@AHGIncome", SqlDbType.Bit);
            command.Parameters["@AHGIncome"].Value = AHGIncome;

            command.Parameters.Add("@Overtime", SqlDbType.Bit);
            command.Parameters["@Overtime"].Value = Overtime;

            command.Parameters.Add("@OrderNo", SqlDbType.Int);
            command.Parameters["@OrderNo"].Value = OrderNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static DataTable GetIncomeVersionByIncomeIdAndVersionNo(int incomeId, int versionNo)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT Id, IncomeId, VersionName, VersionNo FROM IncomeVersion WHERE IncomeId = @IncomeId AND VersionNo = @VersionNo";

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement;
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        //this will be used in the Copy Items function
        public static DataTable GetIncomeItemsByDocAppIdNricVersionNo(int docAppId, string nric, int versionNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT DISTINCT a.IncomeItem FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");
            sqlStatement.Append("INNER JOIN Income c ON b.IncomeId = c.Id INNER JOIN AppPersonal d ON d.id = c.AppPersonalId ");
            sqlStatement.Append("INNER JOIN DocApp e ON e.id = d.DocAppId WHERE e.Id = @DocAppId AND d.Nric = @Nric AND b.VersionNo = @VersionNo");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

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

        public static DataTable GetIncomeVersionById(int incomeVersionId)
        {
            SqlCommand command = new SqlCommand();
            string sqlStatement = "SELECT Id, IncomeId, VersionName, VersionNo FROM IncomeVersion WHERE Id = @IncomeVersionId";

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = incomeVersionId;

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

        public static DataTable GetIncomeVersionByDocAppIdAndNricWithProfile(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            //sqlStatement.Append("SELECT DISTINCT a.VersionName, a.VersionNo, CONVERT(VARCHAR,a.DateEntered,103) AS DateEntered, z.Name FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id ");
            //sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id ");
            //sqlStatement.Append("LEFT JOIN Profile z ON a.EnteredBy = z.UserId WHERE d.Id = @DocAppId AND c.Nric = @Nric  ");

            sqlStatement.Append("SELECT DISTINCT a.VersionName, a.VersionNo, (SELECT TOP 1 z.DateEntered AS DateEntered FROM IncomeVersion z INNER JOIN Income x ON z.IncomeId = x.Id ");
            sqlStatement.Append("INNER JOIN AppPersonal w ON x.AppPersonalId = w.Id INNER JOIN DocApp y ON w.DocAppId = y.Id  ");
            sqlStatement.Append("LEFT JOIN Profile p ON z.EnteredBy = p.UserId WHERE y.Id = @DocAppId AND w.Nric = @Nric  ");
            sqlStatement.Append("AND z.VersionNo = a.VersionNo ORDER BY z.DateEntered desc) AS DateEntered, n.Name FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id   ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id   ");
            sqlStatement.Append("LEFT JOIN Profile n ON a.EnteredBy = n.UserId WHERE d.Id = @DocAppId AND c.Nric = @Nric  ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

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

        public static int DeleteAllIncomeVersionsByDocAppIdAndNric(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("DELETE a FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id  ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id ");
            sqlStatement.Append("WHERE d.Id = @DocAppId AND c.Nric = @Nric  ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }


        public static int UpdateIncomeVersionIdByDocAppIdAndNric(int docAppId, string nric, bool isBlank)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE b SET b.IncomeVersionId = 0, b.IsSetToBlank = @SetToBlank  FROM  Income b INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id ");
            sqlStatement.Append("INNER JOIN DocApp d ON c.DocAppId = d.Id WHERE d.Id = @DocAppId AND c.Nric = @Nric  ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@SetToBlank", SqlDbType.Bit);
            command.Parameters["@SetToBlank"].Value = isBlank;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateIncomeVersionByDocAppIdAndNricAndVersionNo(Guid enteredBy, string VersionName, int docAppId, string nric, int versionNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE a SET a.EnteredBy = @EnteredBy, a.VersionName = @VersionName, DateEntered = GETDATE() ");
            sqlStatement.Append("FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id  ");
            sqlStatement.Append("WHERE d.Id = @DocAppId AND c.Nric = @Nric AND a.VersionNo = @VersionNo ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@EnteredBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@EnteredBy"].Value = enteredBy;

            command.Parameters.Add("@VersionName", SqlDbType.VarChar);
            command.Parameters["@VersionName"].Value = VersionName;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int DeleteIncomeVersionByDocAppIdAndNricAndVersionNo(int docAppId, string nric, int versionNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("DELETE a FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id  ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id ");
            sqlStatement.Append("WHERE d.Id = @DocAppId AND c.Nric = @Nric AND a.VersionNo = @VersionNo ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        
        public static int UpdateIncome(int IncomeId, int versionId, bool setToBlank)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE Income SET IncomeVersionId = @IncomeVersionId, IsSetToBlank = @IsSetToBlank WHERE Id = @IncomeId");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = IncomeId;

            command.Parameters.Add("@IncomeVersionId", SqlDbType.Int);
            command.Parameters["@IncomeVersionId"].Value = versionId;

            command.Parameters.Add("@IsSetToBlank", SqlDbType.Bit);
            command.Parameters["@IsSetToBlank"].Value = setToBlank;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int DeleteIncomeItemsByVersionNoAndDocAppIdAndNric(int versionNo, int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("DELETE a FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");
            sqlStatement.Append("INNER JOIN Income c ON b.IncomeId = c.id ");
            sqlStatement.Append("INNER JOIN AppPersonal d ON c.AppPersonalId = d.Id INNER JOIN DocApp e ON d.DocAppId = e.Id  ");
            sqlStatement.Append("WHERE e.Id = @DocAppId AND d.Nric = @Nric AND b.VersionNo = @VersionNo ");
            
            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static DataTable GetIncomeVersionByDocAppIdAndNricAndVersionNoAndIncomeId(int docAppId, string nric, int versionNo, int incomeId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT DISTINCT a.* FROM IncomeVersion a INNER JOIN Income b ON a.IncomeId = b.Id ");
            sqlStatement.Append("INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id INNER JOIN DocApp d ON c.DocAppId = d.Id ");
            sqlStatement.Append("WHERE d.Id = @DocAppId AND c.Nric = @Nric AND a.VersionNo = @VersionNo AND b.Id = @IncomeId ");

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;

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


        public static int DeleteIncomeItemsByIncomeIdAndVersionNo(int incomeId, int versionNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("DELETE a FROM IncomeDetails a INNER JOIN IncomeVersion b ON a.IncomeVersionID = b.Id ");            
            sqlStatement.Append("WHERE b.IncomeId = @IncomeId AND b.VersionNo = @VersionNo  ");

            command.Parameters.Add("@IncomeId", SqlDbType.Int);
            command.Parameters["@IncomeId"].Value = incomeId;
          
            command.Parameters.Add("@VersionNo", SqlDbType.Int);
            command.Parameters["@VersionNo"].Value = versionNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Gets the latest Income version no when the Income Id clicked doesn't return
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static DataTable GetIncomeVersionNoByDocAppIdAndNricAndIncomeVersionIsZero(int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append("SELECT * FROM IncomeVersion WHERE ID = ");
            sqlStatement.Append("(SELECT TOP 1 b.IncomeVersionId FROM Income b INNER JOIN AppPersonal c ON b.AppPersonalId = c.Id  ");
            sqlStatement.Append("INNER JOIN DocApp d ON c.DocAppId = d.Id WHERE d.Id = @DocAppId AND c.Nric = @Nric AND IncomeVersionId > 0) ");            

            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;

            command.Parameters.Add("@Nric", SqlDbType.VarChar);
            command.Parameters["@Nric"].Value = nric;

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
    }
}
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
    public class LogActionDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        /// <summary>
        /// Get LogAction by Set ID
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public static DataTable GetLogActionBySetID(int setId)
        {

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM " );
            sqlStatement.Append("( ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'SET: ' + SetNo AS Description FROM LogAction INNER JOIN  ");
            sqlStatement.Append("Profile ON LogAction.UserId=Profile.UserId INNER JOIN ");
            sqlStatement.Append("DocSet ON DocSet.Id=LogAction.TypeId WHERE LogAction.TypeId=@setId AND LogAction.Doctype='S' ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'DOC: ' + DocType.Description AS Description FROM LogAction INNER JOIN ");
            sqlStatement.Append("Profile ON LogAction.UserId = Profile.UserId INNER JOIN ");
            sqlStatement.Append("Doc ON LogAction.TypeId=Doc.Id INNER JOIN ");
            sqlStatement.Append("DocType ON Doc.DocTypeCode=DocType.Code WHERE LogAction.TypeId IN (SELECT Id FROM Doc WHERE DocSetId=@setId) ");
            sqlStatement.Append("AND LogAction.DocType = 'D' ");
            sqlStatement.Append(") tbl ORDER BY LogDate DESC");

            command.Parameters.Add("@setId", SqlDbType.Int);
            command.Parameters["@setId"].Value = setId;

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
        /// Get LogAction by App ID
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public static DataTable GetLogActionByAppID(int appId)
        {

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM ");
            sqlStatement.Append("( ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'REF: ' + RefNo AS Description FROM LogAction INNER JOIN  ");
            sqlStatement.Append("Profile ON LogAction.UserId=Profile.UserId INNER JOIN ");
            sqlStatement.Append("DocApp ON DocApp.Id=LogAction.TypeId WHERE LogAction.TypeId=@appId AND LogAction.Doctype='A' ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'DOC: ' + DocType.Description AS Description FROM LogAction INNER JOIN ");
            sqlStatement.Append("PROFILE ON LogAction.UserId = Profile.UserId INNER JOIN ");
            sqlStatement.Append("Doc ON LogAction.TypeId=Doc.Id INNER JOIN ");
            sqlStatement.Append("DocType ON Doc.DocTypeCode=DocType.Code WHERE LogAction.TypeId IN ");
            sqlStatement.Append("(SELECT Id FROM Doc WHERE DocSetId in (select id from DocSet where Status='Verified' and id in (select DocSetId from SetApp where DocAppId=@appId)))");
            sqlStatement.Append("AND LogAction.DocType = 'C' ");
            sqlStatement.Append(") tbl ORDER BY LogDate DESC");

            command.Parameters.Add("@appId", SqlDbType.Int);
            command.Parameters["@appId"].Value = appId;

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
        /// Get LogAction by Doc ID
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static DataTable GetLogActionByDocID(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT LogAction.*,Profile.Name, DocType.Description FROM LogAction INNER JOIN ");
            sqlStatement.Append("Profile ON LogAction.UserId = Profile.UserId INNER JOIN ");
            sqlStatement.Append("Doc ON LogAction.TypeId=Doc.Id INNER JOIN ");
            sqlStatement.Append("DocType ON Doc.DocTypeCode=DocType.Code WHERE LogAction.TypeId =@docId AND LogAction.DocType='D' ORDER BY LogDate DESC");

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


        /// <summary>
        /// Get Document logs for completeness
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static DataTable GetLogActionByDocIDForCompleteness(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT LogAction.*,Profile.Name, DocType.Description FROM LogAction INNER JOIN ");
            sqlStatement.Append("Profile ON LogAction.UserId = Profile.UserId INNER JOIN ");
            sqlStatement.Append("Doc ON LogAction.TypeId=Doc.Id INNER JOIN ");
            sqlStatement.Append("DocType ON Doc.DocTypeCode=DocType.Code WHERE LogAction.TypeId =@docId AND LogAction.DocType='C' ORDER BY LogDate DESC");

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

        #region Added By Edward 24/02/2014 Add Icon and Action Log
        public static DataTable GetLogActionIncomeExtraction(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM ( ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'REF: ' + RefNo AS Description FROM LogAction INNER JOIN  ");
            sqlStatement.Append("Profile ON LogAction.UserId=Profile.UserId INNER JOIN ");
            sqlStatement.Append("DocApp ON DocApp.Id=LogAction.TypeId WHERE LogAction.TypeId=@DocAppId AND LogAction.Doctype='E' ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT LogAction.*,Profile.Name, 'INCOME: ' + DATENAME(MONTH,DATEADD(Month,incomemonth,-1)) + ' ' + CAST(IncomeYear AS VARCHAR) AS Description FROM LogAction INNER JOIN ");
            sqlStatement.Append("PROFILE ON LogAction.UserId = Profile.UserId INNER JOIN ");
            sqlStatement.Append("Income ON LogAction.TypeId=Income.Id ");
            sqlStatement.Append("WHERE LogAction.TypeId IN  ");
            sqlStatement.Append("(SELECT Id FROM Income WHERE AppPersonalId in (select Id from AppPersonal where  DocAppId=@DocAppId))  ");
            sqlStatement.Append("AND LogAction.DocType = 'Z') tbl ORDER BY LogDate DESC  ");

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


        #region Added By Edward to Fix Audit Trail Filter Tables on 2015/06/29
        public static DataTable GetSetAction()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT LogAction.*,Profile.Name FROM LogAction INNER JOIN ");
            sqlStatement.Append("Profile ON LogAction.UserId = Profile.UserId LEFT JOIN ");
            sqlStatement.Append("Doc ON LogAction.TypeId=Doc.Id LEFT JOIN ");
            sqlStatement.Append("DocType ON Doc.DocTypeCode=DocType.Code WHERE LogAction.DocType='S' AND (action like '%Delete set%' OR action like '%Recategorize set%') ORDER BY LogDate DESC");

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

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
//using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class HleInterfceDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        /// <summary>
        /// 2012-08-31 : SP
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns>dataset</returns>
        public static DataTable GetApplicantDetailsByRefNo(string hleNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT Name, ApplicantType, OrderNo FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber ORDER BY ApplicantType, OrderNo");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = hleNumber;

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

        public static string GetHleStatusByRefNo(string hleNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT DISTINCT HleStatus FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = hleNumber;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    if (dataSet.Tables[0].Rows.Count > 1)
                        return "Mixed";
                    else
                        return dataSet.Tables[0].Rows[0][0].ToString();
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the status by DocAppId.      Added by Edward 17.10.2013
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public static string GetHleStatusByDocAppId(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT DISTINCT a.refno, b.HleStatus FROM DocApp a INNER  JOIN HleInterface b ON ");
            sqlStatement.Append("a.RefNo = b.HleNumber WHERE a.Id = @DocAppId");

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
                    if (dataSet.Tables[0].Rows.Count > 1)
                        return "Mixed";
                    else
                        return dataSet.Tables[0].Rows[0]["HleStatus"].ToString();
                else
                    return string.Empty;
            }
        }

        #endregion


       
        /// <summary>
        /// Delete Wrong Records
        /// </summary>
        internal static void DeleteWrongRecords()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //delete based on date length.
            sqlStatement.Append("Delete FROM HleInterface WHERE LEN(HleDate) <10");

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


        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        public static bool DoesPersonalExistsByRefNo(string refNo)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT COUNT(HleNumber) FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = refNo;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                    return true;
                else
                    return false;
            }
        }



        public static bool DoesPersonalExistsByRefNoNric(string refNo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT COUNT(Nric) FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber AND Nric = @Nric");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = refNo;

            command.Parameters.Add("@Nric", SqlDbType.NVarChar);
            command.Parameters["@Nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                    return true;
                else
                    return false;
            }
        }

        public static bool DoesPersonalExistsByRefNoName(string refNo, string name)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT COUNT(Nric) FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber AND Name = @Name");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = refNo;

            command.Parameters.Add("@Name", SqlDbType.NVarChar);
            command.Parameters["@Name"].Value = name;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                    return true;
                else
                    return false;
            }
        }


        public static int GetOrderNumberForNric(string refNo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT  OrderNo FROM HleInterface ");
            sqlStatement.Append("WHERE (HleNumber = @HleNumber) AND (Nric = @Nric)");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = refNo;

            command.Parameters.Add("@Nric", SqlDbType.NVarChar);
            command.Parameters["@Nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return (int) dataSet.Tables[0].Rows[0]["OrderNo"];
                else
                    return 0;
            }
        }

        public static bool IsMainApplicant(string refNo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT ISNULL(IsMainApplicant,'0') AS IsMainApplicant FROM HleInterface ");
            sqlStatement.Append("WHERE HleNumber= @HleNumber AND Nric = @Nric");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = refNo;

            command.Parameters.Add("@Nric", SqlDbType.NVarChar);
            command.Parameters["@Nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return (bool) dataSet.Tables[0].Rows[0]["IsMainApplicant"];
                else
                    return false;
            }
        }

        #endregion



    }
}

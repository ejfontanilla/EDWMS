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
    /// Summary description for AHGIncomeDs
    /// </summary>
    public class AHGIncomeDs
    {

        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        public static int UpdateClear1(AHGIncomeDb cls)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE AHGIncome SET AHGTotalAmount1 = NULL, AHGAvgAmount1 = NULL, NoOfMonths1 = NULL, ");
            sqlStatement.Append("UpdatedBy = @UpdatedBy, DateEntered = GETDATE() WHERE AppPersonalId = @AppPersonalId ");

            command.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@UpdatedBy"].Value = cls.UpdateBy.Value;

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = cls.AppPersonalId ;

       
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public static int UpdateClear2(AHGIncomeDb cls)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("UPDATE AHGIncome SET AHGTotalAmount2 = NULL, AHGAvgAmount2 = NULL, NoOfMonths2 = NULL, ");
            sqlStatement.Append("UpdatedBy = @UpdatedBy, DateEntered = GETDATE() WHERE AppPersonalId = @AppPersonalId ");

            command.Parameters.Add("@UpdatedBy", SqlDbType.UniqueIdentifier);
            command.Parameters["@UpdatedBy"].Value = cls.UpdateBy.Value;

            command.Parameters.Add("@AppPersonalId", SqlDbType.Int);
            command.Parameters["@AppPersonalId"].Value = cls.AppPersonalId;


            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }
}
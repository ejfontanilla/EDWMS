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
    public class AppPersonalSalaryDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods
        
        /// <summary>
        /// Get AppPersonalSalary with AppPersonalDetails by DocId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static DataTable GetAppPersonalSalaryWithAppPersonalDetailsByDocId(int docId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT *, CASE WHEN PersonalType = 'MISC' THEN 100 WHEN PersonalType = 'OC' THEN 99 ELSE 0 END AS PersonalTypeOrder FROM AppPersonalSalary, AppPersonal ");
            sqlStatement.Append("WHERE AppPersonal.Id= AppPersonalSalary.AppPersonalId AND AppPersonalId IN ");
            sqlStatement.Append("(SELECT AppPersonalId FROM AppDocRef WHERE DocId = @docId) " );
            sqlStatement.Append("ORDER BY PersonalTypeOrder, OrderNo, AppPersonal.Id ");

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
    }
}

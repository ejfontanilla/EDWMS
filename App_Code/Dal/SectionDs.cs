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
    public class SectionDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        /// <summary>
        /// Get Section and corresponding department details.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static DataTable GetSectionDepartmentDetail(int Id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Section.*,Department.Id AS DepartmentId, Department.Code AS DepartmentCode, Department.Name AS DepartmentName FROM Section , ");
            sqlStatement.Append("Department WHERE Section.Department = Department.Id AND Section.Id=@id");

            command.Parameters.Add("@id", SqlDbType.Int);
            command.Parameters["@id"].Value = Id;

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

        public static DataTable GetSectionByDepartmentWithMailingList(int Id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT *, '' as MailingList FROM Section WHERE Department=@id");

            command.Parameters.Add("@id", SqlDbType.Int);
            command.Parameters["@id"].Value = Id;

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

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
    public class DepartmentDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        /// <summary>
        ///  Get Department and Section for dropdown display
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDepartmentSectionForDropDown()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT 'all' as id, 'All Departments' as  name , null as parentid ");
            sqlStatement.Append("UNION ");
            sqlStatement.Append("SELECT 'd' + CAST(id AS varchar(5)) as id,  name , 'all' as parentid FROM Department ");
            sqlStatement.Append("UNION " );
            sqlStatement.Append("SELECT 's' + CAST(id AS varchar(5)) as id, name, 'd' + CAST(Department  AS varchar(5)) as parentid FROM Section");

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
        /// Get Department section details for dropdown grid
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDepartmentSectionForDropDownGrid()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Section.Id, Section.Name AS SectionName, Department.Code AS DepartmentCode FROM Section, Department ");
            sqlStatement.Append("WHERE Section.Department = Department.Id ORDER BY Department.Code, Section.Name");

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
        /// Get Department abd section details by section id
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public static DataTable GetDepartmentSectionForDropDownGridBySectionId(int sectionId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Section.Id, Section.Name AS SectionName, Department.Code AS DepartmentCode FROM Section, Department ");
            sqlStatement.Append("WHERE Section.Department = Department.Id and Section=@sectionId ORDER BY Department.Code, Section.Name");

            command.Parameters.Add("@sectionId", SqlDbType.Int);
            command.Parameters["@sectionId"].Value = sectionId;

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
        /// Get docset with section details
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDocSetDocSectionDetail(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DocSet.*,Section.Name,Section.code,Section.department,Section.businessCode FROM DocSet LEFT JOIN Section ON DocSet.SectionId = Section.Id ");
            sqlStatement.Append("WHERE DocSet.Id=@docSetId");

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

        #endregion
    }
}

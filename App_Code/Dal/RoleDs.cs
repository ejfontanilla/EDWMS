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
    public class RoleDs
    {
        public static string GetConnectionString()
        {
            ConnectionStringSettings cts = ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"];
            return cts.ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            SqlConnection objConnection = new SqlConnection(GetConnectionString());
            return objConnection;
        }

        #region Retrieve Methods

        /// <summary>
        /// Get RoleByDepartmentId. if hasManageAllRights get all the complete list.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="hasManageAllRights"></param>
        /// <returns></returns>
        public static DataTable GetRoleByDepartment(int departmentId, Boolean hasManageAllRights)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT aspnet_Roles.*, RoleToDepartment.DepartmentId, Department.Name as DepartmentName ");
            sqlStatement.Append("FROM aspnet_Roles INNER JOIN RoleToDepartment ");
            sqlStatement.Append("ON aspnet_Roles.RoleId=RoleToDepartment.RoleId ");
            sqlStatement.Append("INNER JOIN Department ");
            sqlStatement.Append("ON RoleToDepartment.DepartmentId=Department.Id ");

            if (!hasManageAllRights)
            {
                sqlStatement.Append(" AND RoleToDepartment.DepartmentId=@departmentId AND aspnet_Roles.RoleName <> @RoleName");
                
                command.Parameters.Add("@departmentId", SqlDbType.Int);
                command.Parameters["@departmentId"].Value = departmentId;

                command.Parameters.Add("@RoleName", SqlDbType.VarChar);
                command.Parameters["@RoleName"].Value = RoleEnum.System_Administrator.ToString().Replace("_", " ");
            }

            sqlStatement.Append(" ORDER BY RoleName");

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
        /// Get roles by department for access control
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="hasManageAllRights"></param>
        /// <returns></returns>
        public static DataTable GetRoleByDepartmentForAccessControl(int departmentId, bool hasManageAllRights)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT aspnet_Roles.*, RoleToDepartment.DepartmentId, Department.Name as DepartmentName ");
            sqlStatement.Append("FROM aspnet_Roles INNER JOIN RoleToDepartment ");
            sqlStatement.Append("ON aspnet_Roles.RoleId=RoleToDepartment.RoleId ");
            sqlStatement.Append("INNER JOIN Department ");
            sqlStatement.Append("ON RoleToDepartment.DepartmentId=Department.Id ");
            sqlStatement.Append(" AND RoleToDepartment.DepartmentId=@departmentId ");

            if (!hasManageAllRights)
            {
                sqlStatement.Append(" AND aspnet_Roles.RoleName <> @RoleName");

                command.Parameters.Add("@RoleName", SqlDbType.VarChar);
                command.Parameters["@RoleName"].Value = RoleEnum.System_Administrator.ToString().Replace("_", " ");
            }

            command.Parameters.Add("@departmentId", SqlDbType.Int);
            command.Parameters["@departmentId"].Value = departmentId;

            sqlStatement.Append(" ORDER BY RoleName");

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
        /// Get Role based on moduleName, accessName and hasAccess
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="accessName"></param>
        /// <param name="hasAccess"></param>
        /// <returns></returns>
        public static DataTable GetRoleNameByModuleAndAccessRight(string moduleName, string accessName, int hasAccess)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT aspnet_Roles.* FROM AccessControl, aspnet_Roles WHERE AccessControl.RoleId = aspnet_Roles.RoleId ");
            sqlStatement.Append("AND Module=@moduleName AND AccessRight=@accessName AND HasAccess=@hasAccess");

            command.Parameters.Add("@moduleName", SqlDbType.NVarChar);
            command.Parameters["@moduleName"].Value = moduleName;

            command.Parameters.Add("@accessName", SqlDbType.NVarChar);
            command.Parameters["@accessName"].Value = accessName;

            command.Parameters.Add("@hasAccess", SqlDbType.Int);
            command.Parameters["@hasAccess"].Value = hasAccess;

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Text;
using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class MasterListItemDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods
        public static DataTable GetAllChannelMasterListItem()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT Name FROM MasterListItem WHERE MasterListId IN  ");
            sqlStatement.Append("(SELECT Id FROM MasterList WHERE Name = 'Uploading Channels') order by ItemOrder ");
            sqlStatement.Append("SELECT Name FROM MasterListItem WHERE MasterListId IN  ");
            sqlStatement.Append("(SELECT Id FROM MasterList WHERE Name = 'Scanning Channels') order by ItemOrder ");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                foreach (DataRow dataRow in dataSet.Tables[1].Rows)
                {
                    DataRow[] filteredRows = dataSet.Tables[0].Select("Name = '" + dataRow["Name"].ToString().Trim() + "'");

                    if (filteredRows.Length == 0)
                    {
                        DataRow dr = dataSet.Tables[0].NewRow();
                        dr["Name"] = dataRow["Name"].ToString().Trim();
                        dataSet.Tables[0].Rows.Add(dr);
                    }
                }
                return dataSet.Tables[0];
            }
        }
        #endregion
    }
}

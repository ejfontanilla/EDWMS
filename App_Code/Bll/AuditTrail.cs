using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Data.SqlTypes;
using AuditTrailTableAdapters;
using System.Data.SqlClient;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class AuditTrailDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private AuditTrailTableAdapter _AuditTrailAdapter = null;

        protected AuditTrailTableAdapter Adapter
        {
            get
            {
                if (_AuditTrailAdapter == null)
                    _AuditTrailAdapter = new AuditTrailTableAdapter();

                return _AuditTrailAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get the audit trails
        /// </summary>
        /// <returns></returns>
        public DataTable GetAuditTrail()
        {
            DataTable dataTable;

            ParameterDb parameterDb = new ParameterDb();
            int month = int.Parse(parameterDb.GetParameter(ParameterNameEnum.ArchiveAudit));
            DateTime auditDate = DateTime.Now.AddMonths(-month);

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT AuditDate, IP, Operation, OperationId, ");
            sqlStatement.Append(" ISNULL((SELECT UserName FROM aspnet_Users WHERE(UserId = AuditTrail.UserId)), 'Anonymous User') AS FullName,");
            sqlStatement.Append(" RecordId, SystemInfo, TableName, UserId, UserRole ");
            sqlStatement.Append("FROM AuditTrail ");
            sqlStatement.Append("WHERE (AuditDate >= @AuditDate) AND TableName <>'aspnet_Membership' ");
            sqlStatement.Append(" AND TableName<> 'aspnet_UsersInRoles'");
            sqlStatement.Append("GROUP BY AuditDate, FullName, IP, Operation, OperationId, ");
            sqlStatement.Append("RecordId, SystemInfo, TableName, UserId, UserRole ");
            sqlStatement.Append("ORDER BY AuditDate DESC");

            command.Parameters.Add("@AuditDate", SqlDbType.DateTime);
            command.Parameters["@AuditDate"].Value = auditDate;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                dataTable = dataSet.Tables[0];
            }


            #region Added by Min on Tuesday, June 14, 2011

            // Added new columns which store only the date portion
            // to resolve the problem of filtering date with "Equals" filter function
            dataTable.Columns.Add("AuditDateFormatted", typeof(DateTime));
            dataTable.Columns.Add("AuditDateTimeFormatted", typeof(DateTime));

            DateTime result = DateTime.MinValue;

            foreach (DataRow row in dataTable.Rows)
            {
                if (DateTime.TryParse(row["AuditDate"].ToString(), out result))
                {
                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Second, 0);

                    row["AuditDateFormatted"] = result.Date;
                    row["AuditDateTimeFormatted"] = result;
                }
            }

            #endregion

            return dataTable;
        }

        /// <summary>
        /// Get the archived audit trail records
        /// </summary>
        /// <returns></returns>
        public DataTable GetArchivedRecords()
        {
            DataTable dataTable;

            ParameterDb parameterDb = new ParameterDb();
            int month = int.Parse(parameterDb.GetParameter(ParameterNameEnum.ArchiveAudit));
            DateTime auditDate = DateTime.Now.AddMonths(-month);

            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT AuditDate, IP, Operation, OperationId, ");
            sqlStatement.Append(" ISNULL((SELECT UserName FROM aspnet_Users WHERE(UserId = AuditTrail.UserId)), 'Anonymous User') AS FullName,");
            sqlStatement.Append(" RecordId, SystemInfo, TableName, UserId, UserRole ");
            sqlStatement.Append("FROM AuditTrail ");
            sqlStatement.Append("WHERE (AuditDate < @AuditDate) ");
            sqlStatement.Append("GROUP BY AuditDate, FullName, IP, Operation, OperationId, ");
            sqlStatement.Append("RecordId, SystemInfo, TableName, UserId, UserRole ");
            sqlStatement.Append("ORDER BY AuditDate DESC");

            command.Parameters.Add("@AuditDate", SqlDbType.DateTime);
            command.Parameters["@AuditDate"].Value = auditDate;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                dataTable = dataSet.Tables[0];
            }

            #region Added by Min on Tuesday, June 14, 2011

            // Added new columns which store only the date portion
            // to resolve the problem of filtering date with "Equals" filter function
            dataTable.Columns.Add("AuditDateFormatted", typeof(DateTime));
            dataTable.Columns.Add("AuditDateTimeFormatted", typeof(DateTime));

            DateTime result = DateTime.MinValue;

            foreach (DataRow row in dataTable.Rows)
            {
                if (DateTime.TryParse(row["AuditDate"].ToString(), out result))
                {
                    result = new DateTime(result.Year, result.Month, result.Day, result.Hour, result.Second, 0);

                    row["AuditDateFormatted"] = result.Date;
                    row["AuditDateTimeFormatted"] = result;
                }               
            }

            #endregion

            return dataTable;
        }

        /// <summary>
        /// Get a single transaction
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns> 
        public AuditTrail.AuditTrailDataTable GetTransactionByOperationId(Guid operationId)
        {
            return Adapter.GetTransactionByOperationId(operationId);
        }

        /// <summary>
        /// Get all transactions for a record
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="recordId"></param>
        /// <returns></returns> 
        public DataTable GetTransactionHistory(TableNameEnum tableName, string recordId)
        {
            if (!Validation.IsNaturalNumber(recordId) && !Validation.IsGuid(recordId))
                return new AuditTrail.AuditTrailDataTable();

           // return Adapter.GetTransactionHistory(tableName.ToString(), recordId);
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT AuditDate, Operation, OperationId, TableName, RecordId, UserId, UserRole, Ip,SystemInfo,");
            sqlStatement.Append(" ISNULL((SELECT UserName FROM aspnet_Users WHERE(UserId = AuditTrail.UserId)), 'Anonymous User') AS FullName");
            sqlStatement.Append(" FROM AuditTrail");
            sqlStatement.Append(" WHERE (TableName = @TableName) AND (RecordId = @RecordId)");
            sqlStatement.Append(" GROUP BY AuditDate, Operation, OperationId, TableName, RecordId, UserId, UserRole, IP, FullName,SystemInfo");
            sqlStatement.Append(" ORDER BY AuditDate DESC");

            command.Parameters.Add("@TableName", SqlDbType.VarChar);
            command.Parameters["@TableName"].Value = tableName;

            command.Parameters.Add("@RecordId", SqlDbType.VarChar);
            command.Parameters["@RecordId"].Value = recordId;

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
        /// Get records by table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        private DataTable GetRecordByTableAndRecordId(TableNameEnum tableName, string recordId)
        {
            SqlCommand command = new SqlCommand();
            string sql = null;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                if (Validation.IsNaturalNumber(recordId))
                {
                    sql = "SELECT * FROM [" + tableName.ToString() + "] WHERE Id = @Id";
                    command.Parameters.Add("@Id", SqlDbType.Int);
                    command.Parameters["@Id"].Value = int.Parse(recordId);
                }
                else
                {
                    if (tableName.Equals(TableNameEnum.aspnet_Roles))
                    {
                        sql = "SELECT * FROM [" + tableName + "] WHERE roleId = @Id";
                    }else
                    {
                        sql = "SELECT * FROM [" + tableName + "] WHERE UserId = @Id";
                    }
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier);
                    command.Parameters["@Id"].Value = new Guid(recordId);
                }

                command.CommandText = sql;
                command.Connection = connection;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get table columns
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable GetTableColumns(TableNameEnum tableName)
        {
            SqlCommand command = new SqlCommand();
            string sql = "SELECT column_name FROM information_schema.columns WHERE table_name = '" + tableName.ToString() + "'";

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sql;
                command.Connection = connection;
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }        

        /// <summary>
        /// Get users from audit trail
        /// </summary>
        /// <returns></returns>
        public AuditTrail.AuditTrailDataTable GetUsers()
        {
            return Adapter.GetUsers();
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Record the transaction
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="recordId"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public Guid? Record(TableNameEnum tableName, string recordId, OperationTypeEnum operation)
        {
            if (!Validation.IsNaturalNumber(recordId) && !Validation.IsGuid(recordId))
                return null;

            // Get record by table name and record ID
            DataTable record = GetRecordByTableAndRecordId(tableName, recordId);
            if (record.Rows.Count == 0)
                return null;

            // Get table columns
            DataTable columns = GetTableColumns(tableName);
            if (columns.Rows.Count == 0)
                return null;

            // Get audit trail parameters
            Guid? userId = null;
            string userRole = "Unknown";
            MembershipUser user = Membership.GetUser();

            if (user != null)
            {
                userId = (Guid)user.ProviderUserKey;
                string userName = user.UserName;

                UserDb userDb = new UserDb();
                userRole = userDb.GetRole(userId.Value);
            }

            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string systemInfo = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            DateTime auditDate = DateTime.Now;
            Guid operationId = Guid.NewGuid();
            int count = 0;

            // Inser column name and values
            foreach (DataRow r in columns.Rows)
            {
                string columnName = r["column_name"].ToString();
                string columnValue = record.Rows[0][columnName].ToString();

                count = count + Insert(auditDate, operation, operationId, tableName,
                    recordId, columnName, columnValue, userId, userRole, ip, systemInfo);
            }

            return operationId;
        }

        //Added by Edward 2014/05/05    Batch Upload
        public Guid? Record(TableNameEnum tableName, string recordId, OperationTypeEnum operation, string ip, string systemInfo)
        {
            if (!Validation.IsNaturalNumber(recordId) && !Validation.IsGuid(recordId))
                return null;

            // Get record by table name and record ID
            DataTable record = GetRecordByTableAndRecordId(tableName, recordId);
            if (record.Rows.Count == 0)
                return null;

            // Get table columns
            DataTable columns = GetTableColumns(tableName);
            if (columns.Rows.Count == 0)
                return null;

            // Get audit trail parameters
            Guid? userId = null;
            string userRole = "Unknown";
            MembershipUser user = Membership.GetUser();

            if (user != null)
            {
                userId = (Guid)user.ProviderUserKey;
                string userName = user.UserName;

                UserDb userDb = new UserDb();
                userRole = userDb.GetRole(userId.Value);
            }
     
            DateTime auditDate = DateTime.Now;
            Guid operationId = Guid.NewGuid();
            int count = 0;

            // Inser column name and values
            foreach (DataRow r in columns.Rows)
            {
                string columnName = r["column_name"].ToString();
                string columnValue = record.Rows[0][columnName].ToString();

                count = count + Insert(auditDate, operation, operationId, tableName,
                    recordId, columnName, columnValue, userId, userRole, ip, systemInfo);
            }

            return operationId;
        }

        /// <summary>
        /// Insert transaction
        /// </summary>
        /// <param name="auditDate"></param>
        /// <param name="operation"></param>
        /// <param name="operationId"></param>
        /// <param name="tableName"></param>
        /// <param name="recordId"></param>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <param name="userId"></param>
        /// <param name="userRole"></param>
        /// <param name="ip"></param>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        private int Insert(DateTime auditDate, OperationTypeEnum operation, Guid operationId,
            TableNameEnum tableName, string recordId, string columnName, string columnValue,
            Guid? userId, string userRole, string ip, string systemInfo)
        {
            // Insert

            if (userId == null)
            {
                return 0;
            }

            AuditTrail.AuditTrailDataTable auditTrails = new AuditTrail.AuditTrailDataTable();
            AuditTrail.AuditTrailRow auditTrail = auditTrails.NewAuditTrailRow();

            auditTrail.AuditDate = auditDate;
            auditTrail.Operation = operation.ToString();
            auditTrail.OperationId = operationId;
            auditTrail.TableName = tableName.ToString();
            auditTrail.RecordId = recordId;
            auditTrail.ColumnName = columnName;
            auditTrail.ColumnValue = columnValue;
            auditTrail.UserId = userId.Value;
            //auditTrail.FullName = NameManager.GetFullName(userId.Value);
            auditTrail.UserRole = userRole;
            auditTrail.IP = ip;
            auditTrail.SystemInfo = systemInfo;

            auditTrails.AddAuditTrailRow(auditTrail);
            int rowsAffected = Adapter.Update(auditTrails);

            return rowsAffected;
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public bool Delete(Guid? operationId)
        {
            if (operationId != null)
            {
                int rowsAffected = Adapter.DeleteByOperationId(operationId.Value);

                return (rowsAffected > 0);
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
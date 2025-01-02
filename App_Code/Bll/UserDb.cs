using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using UserTableAdapters;
using System.Data.SqlClient;
using Dwms.Web;

namespace Dwms.Bll
{
    [System.ComponentModel.DataObject]
    public class UserDb
    {
        private UserTableAdapter _UserAdapter = null;

        protected UserTableAdapter Adapter
        {
            get
            {
                if (_UserAdapter == null)
                    _UserAdapter = new UserTableAdapter();

                return _UserAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        public User.UserDataTable GetUser()
        {
            User.UserDataTable dataTable = Adapter.GetUsers();

            dataTable.Columns.Add("NameNricFormat", typeof(String));

            // Format the name and NRIC for display
            foreach (User.UserRow row in dataTable.Rows)
            {
                row["NameNricFormat"] = String.Format("{0} ({1})", row.Name, row.UserName);
            }

            return dataTable;
        }

        /// <summary>
        /// Get user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User.UserDataTable GetUser(Guid userId)
        {
            return Adapter.GetUserByUserId(userId);
        }

        /// <summary>
        /// Get admin users
        /// </summary>
        /// <returns></returns>
        public User.UserDataTable GetAdminUsers()
        {
            User.UserDataTable user = Adapter.GetUsers();

            User.UserDataTable table = new User.UserDataTable();

            if (user.Rows.Count == 0)
            {
                return new User.UserDataTable();
            }
            else
            {
                foreach (User.UserRow row in user.Rows)
                {
                    if (row.RoleName.Equals(RoleEnum.System_Administrator.ToString()))
                    {
                        User.UserRow r = table.NewUserRow();

                        r.UserId = row.UserId;
                        r.UserName = row.UserName;
                        r.RoleName = row.RoleName;
                        r.Email = row.Email;
                        r.Designation = row.Designation;
                        r.Name = row.Name;
                        r.Section = row.Section;

                        table.AddUserRow(r);
                    }
                }
            }

            return table;
        }        

        /// <summary>
        /// Get the nric of the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetNric(Guid userId)
        {
            string nric = string.Empty;

            User.UserDataTable dt = GetUser(userId);

            if (dt.Rows.Count > 0)
            {
                User.UserRow dr = dt[0];
                nric = dr.UserName;
            }

            return nric;
        }

        /// <summary>
        /// Get user by role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public User.UserDataTable GetUser(string role)
        {
            return Adapter.GetUsersByRole(role.ToString());
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserByUserName(string userName)
        {
            return Adapter.GetUserByUserName(userName);
        }

        /// <summary>
        /// Get admin emails
        /// </summary>
        /// <returns></returns>
        public string GetSystemAdminEmails()
        {
            string emails = string.Empty;

            User.UserDataTable users = GetUser(RoleEnum.System_Administrator.ToString());

            foreach (User.UserRow rUser in users.Rows)
            {
                emails = (String.IsNullOrEmpty(emails) ? rUser.Email : emails + ";" + rUser.Email);
            }

            return emails;
        }

        /// <summary>
        /// Get the user role
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetRole(Guid userId)
        {
            string role = string.Empty;

            User.UserDataTable dt = GetUser(userId);

            if (dt.Rows.Count > 0)
            {
                User.UserRow dr = dt[0];
                role = dr.RoleName;
            }

            return role;
        }

        /// <summary>
        /// Get RoleID by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Guid GetRoleId(Guid userId)
        {
            Guid role = Guid.Empty;

            User.UserDataTable dt = GetUser(userId);

            if (dt.Rows.Count > 0)
            {
                User.UserRow dr = dt[0];
                role = dr.RoleId;
            }

            return role;
        }

        /// <summary>
        /// Get Section
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetSection(Guid userId)
        {
            int section = -1;

            User.UserDataTable dt = GetUser(userId);

            if (dt.Rows.Count > 0)
            {
                User.UserRow dr = dt[0];
                section = dr.Section;
            }

            return section;
        }

        /// <summary>
        /// Get the users by section
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserBySection(int section)
        {
            return Adapter.GetDataBySection(section);
        }

        /// <summary>
        /// Get user list for assign in verification module
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserBySectionForVerification(int section)
        {
            return Adapter.GetUserBySectionForVerification(section);
        }

        /// <summary>
        /// Get user list for assign in completeness module
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserBySectionForCompleteness(int section)
        {
            return Adapter.GetUserBySectionForCompleteness(section);
        }

        //18.07.2013 Commented By Edward...'UserTableAdapters.UserTableAdapter' does not contain a definition for 'GetUserBySectionForAssessment' and no extension method 'GetUserBySectionForAssessment' accepting a first argument of type 'UserTableAdapters.UserTableAdapter' could be found 
        public User.UserDataTable GetUserBySectionForAssessment(int section)
        {
            return Adapter.GetUserBySectionForAssessment(section);
        }

        /// <summary>
        /// Get the users by section and department
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserBySectionAndDepartment(int section, int department)
        {
            return Adapter.GetDataBySectionAndDepartment(section, department);
        }

        /// <summary>
        /// Get the users by department
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public User.UserDataTable GetUserByDepartment(int department)
        {
            return Adapter.GetDataByDepartment(department);
        }

        /// <summary>
        /// Get the user section
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetUserSection(Guid userId)
        {
            int section = -1;

            User.UserDataTable dt = GetUser(userId);

            if (dt.Rows.Count > 0)
            {
                User.UserRow dr = dt[0];
                section = dr.Section;
            }

            return section;
        }

        #endregion

        #region Delete Methods
        public bool UpdateIsApprovedStatus(Guid userId, bool status)
        {
            MembershipUser user = Membership.GetUser(userId);

            if (user != null)
            {

                user.IsApproved = status;
                Membership.UpdateUser(user);

                AuditTrailDb auditTrailDb = new AuditTrailDb();
                Guid? operationId1 = auditTrailDb.Record(TableNameEnum.aspnet_Membership, userId.ToString(), OperationTypeEnum.Update);

                if (operationId1.HasValue)
                    return true;
                else
                    return false;


                // ######################## Keep this code as we might need it in future #####################

                //Guid? operationId1 = Util.RecordAudit(TableNameEnum.aspnet_Users, userId.ToString(), OperationTypeEnum.Delete);
                //Guid? operationId2 = Util.RecordAudit(TableNameEnum.aspnet_Membership, userId.ToString(), OperationTypeEnum.Delete);
                //Guid? operationId3 = Util.RecordAudit(TableNameEnum.aspnet_UsersInRoles, userId.ToString(), OperationTypeEnum.Delete);
                ////Guid? operationId4 = Util.RecordAudit(TableName.Profile, id.ToString(), OperationType.Delete);

                //bool success = Membership.DeleteUser(user.UserName, true);

                //if (!success)
                //{
                //    Util.DeleteAudit(operationId1);
                //    Util.DeleteAudit(operationId2);
                //    Util.DeleteAudit(operationId3);
                //    //Util.DeleteAudit(operationId4);
                //}

                // ######################## Keep this code as we might need it in future #####################
            }
            else
                return false;
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Delete(Guid userId)
        {
            MembershipUser user = Membership.GetUser(userId);

            if (user != null)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                Guid? operationId1 = auditTrailDb.Record(TableNameEnum.aspnet_Users, userId.ToString(), OperationTypeEnum.Delete);
                Guid? operationId2 = auditTrailDb.Record(TableNameEnum.aspnet_Membership, userId.ToString(), OperationTypeEnum.Delete);
                Guid? operationId3 = auditTrailDb.Record(TableNameEnum.aspnet_UsersInRoles, userId.ToString(), OperationTypeEnum.Delete);
                //Guid? operationId4 = Util.RecordAudit(TableName.Profile, id.ToString(), OperationType.Delete);

                bool success = false;

                try
                {
                    success = Membership.DeleteUser(user.UserName, true);
                }
                catch (Exception ex)
                {

                }

                if (!success)
                {
                    auditTrailDb.Delete(operationId1);
                    auditTrailDb.Delete(operationId2);
                    auditTrailDb.Delete(operationId3);
                }

                return success;
            }
            else
                return false;
        }

        #endregion

        #region Update Methods
        /// <summary>
        /// De-activate the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeactivateUser(Guid userId)
        {
            MembershipUser user = Membership.GetUser(userId);
            if (user.IsApproved)
                user.IsApproved = false;
            Membership.UpdateUser(user);
            return true;
        }
        #endregion
    }
}
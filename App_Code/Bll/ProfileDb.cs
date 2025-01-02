using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Security;
using System.IO;
using ProfileTableAdapters;
using Dwms.Web;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class ProfileCustom
    {
        private Guid userId;
        private string fullName;
        private string email;
        private string designation;
        private int section;
        private string team;

        public Guid UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Designation
        {
            get { return designation; }
            set { designation = value; }
        }

        public int Section
        {
            get { return section; }
            set { section = value; }
        }

        public string Team
        {
            get { return team; }
            set { team = value; }
        }

        public ProfileCustom(Guid userId, string fullName, string email, string designation, int section, string team)
        {
            this.userId = userId;
            FullName = fullName;
            Email = email;
            Designation = designation;
            Section = section;
            Team = team;
            UserId = userId;
        }

        public bool Save()
        {
            ProfileDb profileDb = new ProfileDb();
            return profileDb.Update(userId, FullName, Email, Designation, Section, Team);
        }
    }

    public class ProfileDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private ProfileTableAdapter _ProfileAdapter = null;

        protected ProfileTableAdapter Adapter
        {
            get
            {
                if (_ProfileAdapter == null)
                    _ProfileAdapter = new ProfileTableAdapter();

                return _ProfileAdapter;
            }
        }

        #region Retrieve Methods


        public ProfileCustom GetProfile(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            if (user == null)
                return null;
            
            Guid userId = (Guid)user.ProviderUserKey;
            Profile.ProfileDataTable profiles = Adapter.GetProfileByUserId(userId);

            // Insert profile
            if (profiles.Rows.Count == 0)
            {
                Insert(userId, string.Empty, string.Empty, string.Empty, 1, string.Empty);
                return new ProfileCustom(userId, string.Empty, string.Empty, string.Empty, 1, string.Empty);
            }
            else
            {
                Profile.ProfileRow profile = profiles[0];
                string team = profile.IsTeamNull() ? null : profile.Team;
                return new ProfileCustom(userId, profile.Name, profile.Email, profile.Designation, profile.Section, team);
            }
        }

        /// <summary>
        /// Get the profile information including the opration and group details
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetProfileInfo(Guid userId)
        {
            MembershipUser user = Membership.GetUser(userId);
            if (user == null)
                return null;

            return ProfileDs.GetProfileInfo(userId);
        }

        public ProfileCustom GetProfile(Guid userId)
        {
            MembershipUser user = Membership.GetUser(userId);
            if (user == null)
                return null;

            //Guid userId = (Guid)user.ProviderUserKey;
            Profile.ProfileDataTable profiles = Adapter.GetProfileByUserId(userId);

            // Insert profile
            if (profiles.Rows.Count == 0)
            {
                Insert(userId, string.Empty, string.Empty, string.Empty, 1, string.Empty);
                return new ProfileCustom(userId, string.Empty, string.Empty, string.Empty, 1, string.Empty);
            }
            else
            {
                Profile.ProfileRow profile = profiles[0];
                string team = profile.IsTeamNull()? null : profile.Team;
                return new ProfileCustom(userId, profile.Name, profile.Email, profile.Designation, profile.Section, team);
            }
        }

        public static string GetLoginEmail()
        {
            string email = string.Empty;

            MembershipUser user = Membership.GetUser();

            if (user == null)
            {
                return string.Empty;
            }
            ProfileDb profileDb = new ProfileDb();
            ProfileCustom userProfile = profileDb.GetProfile(user.UserName);

            if (userProfile == null)
            {
                return string.Empty;
            }

            return userProfile.Email;
        }

        public static string GetLoginName(string email)
        {
            string name = string.Empty;

            string  userName = Membership.GetUserNameByEmail(email);

            if (userName == null)
            {
                return string.Empty;
            }
            ProfileDb profileDb = new ProfileDb();
            ProfileCustom userProfile = profileDb.GetProfile(userName);

            if (userProfile == null)
            {
                return string.Empty;
            }

            return userProfile.FullName;
        }

        public static string GetNric(Guid? userId)
        {
            return Membership.GetUser(userId.Value).UserName;
        }

        public static string GetUserFullName(string email)
        {
            string name = string.Empty;

            string userName = Membership.GetUserNameByEmail(email);

            if (userName == null)
            {
                return string.Empty;
            }

            ProfileDb profileDb = new ProfileDb();
            ProfileCustom userProfile = profileDb.GetProfile(userName);

            if (userProfile == null)
            {
                return string.Empty;
            }

            return userProfile.FullName;
        }

        public string GetUserFullName(Guid userId)
        {
            ProfileCustom profile = GetProfile(userId);

            return (profile != null ? profile.FullName : " ");
        }

        public string GetUserDesignation(Guid userId)
        {
            ProfileCustom profile = GetProfile(userId);

            return profile != null ? profile.Designation : string.Empty;
        }

        public DataTable GetvProfile(Guid userId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM vProfile ");
            sqlStatement.Append("WHERE UserId=@UserId");

            command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@UserId"].Value = userId;

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
        /// To Check if Specific Email is inside the list, distincted by Set Id to know the Section ID
        /// </summary>
        /// <param name="username"></param>
        /// <param name="setId"></param>
        /// <returns></returns>
        public static bool GetCountByEmailSetId(string OIC, int setId)
        {
            return (ProfileDs.GetCountByEmailSetId(OIC, setId) > 0);
        }
        #endregion

        public bool Insert(Guid userId, string fullName, string email, string designation, int section, string team)
        {
            Profile.ProfileDataTable profiles = new Profile.ProfileDataTable();
            Profile.ProfileRow profile = profiles.NewProfileRow();

            profile.UserId = userId;
            profile.Name = fullName;
            profile.Email = email;
            profile.Designation = designation;
            profile.Section = section;
            profile.Team = team;

            profiles.AddProfileRow(profile);
            int rowsAffected = Adapter.Update(profiles);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Profile, userId.ToString(), OperationTypeEnum.Insert);
            }

            return rowsAffected > 0;
        }

        public bool Update(Guid userId, string fullName, string email, string designation, int section, string team)
        {
            Profile.ProfileDataTable profiles = Adapter.GetProfileByUserId(userId);
            if (profiles.Count == 0)
                return false;

            Profile.ProfileRow profile = profiles[0];

            profile.UserId = userId;
            profile.Name = fullName;
            profile.Email = email;
            profile.Designation = designation;
            profile.Section = section;
            profile.Team = team;
          
            int rowsAffected = Adapter.Update(profile);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Profile, userId.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected > 0;
        }

        public bool Delete(Guid userId)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            Guid? operation = auditTrailDb.Record(TableNameEnum.Profile, userId.ToString(), OperationTypeEnum.Delete);
            int rowsAffected = Adapter.DeleteByUserId(userId);

            if (rowsAffected == 0)
            {
                auditTrailDb.Record(TableNameEnum.Profile, userId.ToString(), OperationTypeEnum.Delete);
            }

            return rowsAffected > 0;
        }

        #region Added By Edward 2014/08/20  Stop Notification Email to Sales and Sers
        public static DataTable GetProfileInfoByUserInitials(string userInitials)
        {
            return ProfileDs.GetProfileInfoByUserInitials(userInitials);
        }
        #endregion
    }
}
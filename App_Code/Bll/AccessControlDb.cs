using System;
using System.Collections.Generic;
using System.Web.Security;
using AccessControlTableAdapters;

namespace Dwms.Bll
{
    public class AccessControlDb
    {
        private AccessControlTableAdapter _AccessControlAdapter = null;

        protected AccessControlTableAdapter Adapter
        {
            get
            {
                if (_AccessControlAdapter == null)
                    _AccessControlAdapter = new AccessControlTableAdapter();

                return _AccessControlAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all the access controls settings
        /// </summary>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl()
        {
            return Adapter.GetAccessControls();
        }

        /// <summary>
        /// Get access control setting by role
        /// </summary>
        /// <param name="AccessControlCode"></param>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl(Guid roleId)
        {
            return Adapter.GetAccessControlByRole(roleId);
        }

        /// <summary>
        /// Get access control setting by id
        /// </summary>
        /// <param name="AccessControlCode"></param>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl(int id)
        {
            return Adapter.GetAccessControlById(id);
        }

        /// <summary>
        /// Get access control setting by Role id, module and access rights
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl(Guid roleId, ModuleNameEnum module, string accessRight)
        {
            return Adapter.GetAccessRightByRoleIdModuleRights(roleId, module.ToString(), accessRight);
        }

        /// <summary>
        /// Get access control by role id and hasAccess status
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="hasAccess"></param>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl(Guid roleId, bool hasAccess)
        {
            return Adapter.GetStatusAccessControlByRole(roleId, hasAccess);
        }

        /// <summary>
        /// Get access control by role id, module and hasAccess status
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <param name="hasAccess"></param>
        /// <returns></returns>
        public AccessControl.AccessControlDataTable GetAccessControl(Guid roleId, ModuleNameEnum module, bool hasAccess)
        {
            return Adapter.GetStatusModuleAccessControlByRole(roleId, module.ToString(), hasAccess);
        }

        /// <summary>
        /// Check if the role has access to the module
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public bool HasAccess(Guid roleId, ModuleNameEnum module, string accessRight)
        {
            bool result = false;

            AccessControl.AccessControlDataTable tbl = GetAccessControl(roleId, module, accessRight);

            if (tbl.Rows.Count > 0)
            {
                AccessControl.AccessControlRow row = tbl[0];
                result = row.HasAccess;
            }

            return result;
        }

        /// <summary>
        /// Get user access control lits by module
        /// </summary>
        /// <param name="moduleNameEnum"></param>
        /// <returns></returns>
        public List<string> GetUserAccessControlList(ModuleNameEnum moduleNameEnum)
        {
            //redirect based on the access rights
            //get user role access
            Guid? userId = null;
            Guid roleId = Guid.Empty;
            MembershipUser user = Membership.GetUser();
            List<string> AccessControlList = new List<string>();

            if (user != null)
            {
                userId = (Guid)user.ProviderUserKey;
                return GetUserAccessControlList(moduleNameEnum, userId.Value);
            }

            return AccessControlList;
        }

        /// <summary>
        /// Get user access control lits by module and userid
        /// </summary>
        /// <param name="moduleNameEnum"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetUserAccessControlList(ModuleNameEnum moduleNameEnum, Guid userId)
        {
            Guid roleId = Guid.Empty;
            List<string> AccessControlList = new List<string>();

            UserDb userDb = new UserDb();
            roleId = userDb.GetRoleId(userId);

            AccessControlDb accessControlDb = new AccessControlDb();
            AccessControl.AccessControlDataTable accessControl = new AccessControl.AccessControlDataTable();

            accessControl = accessControlDb.GetAccessControl(roleId, moduleNameEnum, true);

            if (accessControl.Rows.Count > 0)
            {
                foreach (AccessControl.AccessControlRow r in accessControl.Rows)
                {
                    AccessControlList.Add(r.AccessRight.ToLower().Trim());
                }
            }
            return AccessControlList;
        }

        #endregion        

        #region Insert Methods
        /// <summary>
        /// Insert access control setting
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Insert(Guid roleId, ModuleNameEnum module, string accessRight, bool hasAccess)
        {
            AccessControl.AccessControlDataTable dt = new AccessControl.AccessControlDataTable();
            AccessControl.AccessControlRow r = dt.NewAccessControlRow();
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            r.RoleId = roleId;
            r.Module = module.ToString();
            r.AccessRight = accessRight;
            r.HasAccess = hasAccess;

            dt.AddAccessControlRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                auditTrailDb.Record(TableNameEnum.AccessControl, r.Id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update the access rights
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Update(int id, Guid roleId, ModuleNameEnum module, string accessRight, bool hasAccess)
        {
            AccessControl.AccessControlDataTable dt = GetAccessControl(id);
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            if (dt.Count == 0) return false;

            AccessControl.AccessControlRow r = dt[0];

            r.RoleId = roleId;
            r.Module = module.ToString();
            r.AccessRight = accessRight;
            r.HasAccess = hasAccess;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.AccessControl, r.Id.ToString(), OperationTypeEnum.Update);
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Update the access rights
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <param name="accessRight"></param>
        /// <param name="hasAccess"></param>
        /// <returns></returns>
        public bool Update(Guid roleId, ModuleNameEnum module, string accessRight, bool hasAccess)
        {
            AccessControl.AccessControlDataTable dt = GetAccessControl(roleId, module, accessRight);
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            if (dt.Count == 0)
            {
                int id = Insert(roleId, module, accessRight, hasAccess);
                return (id > 0);
            }

            AccessControl.AccessControlRow r = dt[0];
            Boolean initialAccess = r.HasAccess;
            r.RoleId = roleId;
            r.Module = module.ToString();
            r.AccessRight = accessRight;
            r.HasAccess = hasAccess;            
            
            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0 && initialAccess != hasAccess)
            {
                auditTrailDb.Record(TableNameEnum.AccessControl, r.Id.ToString(), OperationTypeEnum.Update);
            }

            return (rowsAffected > 0);
        }
        #endregion

        #region Miscellaneous
        /// <summary>
        /// Check if the user has access to the function
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <param name="accessRight"></param>
        /// <returns></returns>
        public bool HasAccessRights(Guid roleId, ModuleNameEnum module, AccessControlSettingEnum accessRight)
        {
            bool hasAccess = false;

            AccessControlDb accessControlDb = new AccessControlDb();
            hasAccess = accessControlDb.HasAccess(roleId, module, accessRight.ToString());

            return hasAccess;
        }
        #endregion
    }
}
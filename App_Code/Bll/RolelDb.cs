using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using RoleTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class RoleDb
    {
        private RoleTableAdapter _RoleAdapter = null;

        protected RoleTableAdapter Adapter
        {
            get
            {
                if (_RoleAdapter == null)
                    _RoleAdapter = new RoleTableAdapter();

                return _RoleAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all the roles
        /// </summary>
        /// <returns></returns>
        public Role.RoleDataTable GetRole()
        {
            Role.RoleDataTable dt = Adapter.GetRoles();
            Role.RoleDataTable finalDt = new Role.RoleDataTable();
            finalDt.Columns.Add("RoleNameText", typeof(string));

            int i = 0;

            foreach (Role.RoleRow dr in dt.Rows)
            {
                if (i <= 500)
                {
                    Role.RoleRow finalDr = finalDt.NewRoleRow();

                    finalDr.ApplicationId = dr.ApplicationId;
                    finalDr.RoleId = dr.RoleId;
                    finalDr.RoleName = dr.RoleName;
                    finalDr.LoweredRoleName = dr.LoweredRoleName;
                    //finalDr.Description = dr.Description;
                    finalDr["RoleNameText"] = Format.FormatEnumValue(dr.RoleName);

                    finalDt.Rows.Add(finalDr);
                }
                i++;
            }

            return finalDt;
        }

        /// <summary>
        /// Get Roles by Department and access rights
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="hasManageAllRights"></param>
        /// <returns></returns>
        public DataTable GetRoleByDepartment(int departmentId, Boolean hasManageAllRights)
        {
            return RoleDs.GetRoleByDepartment(departmentId, hasManageAllRights);
        }

        public DataTable GetRoleByDepartmentForAccessControl(int departmentId, Boolean hasManageAllRights)
        {
            return RoleDs.GetRoleByDepartmentForAccessControl(departmentId, hasManageAllRights);
        }

        /// <summary>
        /// Get role by role id
        /// </summary>
        /// <param name="RoleCode"></param>
        /// <returns></returns>
        public Role.RoleDataTable GetRole(Guid roleId)
        {
            return Adapter.GetRoleByRoleId(roleId);
        }

        /// <summary>
        /// Get role by role name
        /// </summary>
        /// <param name="RoleCode"></param>
        /// <returns></returns>
        public Role.RoleDataTable GetRole(string roleName)
        {
            return Adapter.GetRoleByRoleName(roleName.ToLower());
        }

        /// <summary>
        /// check if role exists
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public bool RoleExists(string roleName)
        {
            return Roles.RoleExists(roleName);
        }

        /// <summary>
        /// Get the role name
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string GetRoleName(Guid roleId)
        {
            string roleName = string.Empty;

            Role.RoleDataTable dt = GetRole(roleId);

            if (dt.Rows.Count > 0)
            {
                Role.RoleRow dr = dt[0];
                roleName = dr.RoleName;
            }

            return roleName;
        }

        /// <summary>
        /// Get the role id (GUID) of the role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Guid GetRoleGuid(string roleName)
        {
            Guid roleGuid = Guid.Empty;

            Role.RoleDataTable dt = GetRole(roleName);

            if (dt.Rows.Count > 0)
            {
                Role.RoleRow dr = dt[0];
                roleGuid = dr.RoleId;
            }

            return roleGuid;
        }

        /// <summary>
        /// Get Role based on moduleName, accessName and hasAccess
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="accessName"></param>
        /// <param name="hasAccess"></param>
        /// <returns></returns>
        public DataTable GetRoleNameByModuleAndAccessRight(ModuleNameEnum moduleName, AccessControlSettingEnum accessName, int hasAccess)
        {
            return RoleDs.GetRoleNameByModuleAndAccessRight(moduleName.ToString(), accessName.ToString(), hasAccess);
        }
        

        #endregion        

        #region Insert Methods
        /// <summary>
        /// Insert role
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public Guid Insert(string roleName)
        {
            Roles.CreateRole(roleName);
            return this.GetRoleGuid(roleName);
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update the role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Update(Guid roleId, string roleName)
        {
            Role.RoleDataTable dt = GetRole(roleId);

            if (dt.Count == 0) return false;

            Role.RoleRow r = dt[0];

            r.RoleName = roleName;
            r.LoweredRoleName = roleName.ToLower();

            int rowsAffected = Adapter.Update(dt);
            
            return (rowsAffected > 0);
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="roleName"></param>
        public bool Delete(string roleName)
        {
            return Roles.DeleteRole(roleName, true);
        }
        #endregion
    }
}
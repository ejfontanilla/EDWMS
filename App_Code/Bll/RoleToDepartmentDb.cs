using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using RoleToDepartmentTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for RoleToDepartmentDb
    /// </summary>
    public class RoleToDepartmentDb
    {
        private RoleToDepartmentTableAdapter _RoleToDepartmentAdapter = null;

        protected RoleToDepartmentTableAdapter Adapter
        {
            get
            {
                if (_RoleToDepartmentAdapter == null)
                    _RoleToDepartmentAdapter = new RoleToDepartmentTableAdapter();

                return _RoleToDepartmentAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the records
        /// </summary>
        /// <returns></returns>
        public RoleToDepartment.RoleToDepartmentDataTable GetAppDocRef()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get RoleToDepartment By RoleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public RoleToDepartment.RoleToDepartmentDataTable GetRoleToDepartmentByRoleId(Guid roleId)
        {
            return Adapter.GetRoleToDepartmentByRoleId(roleId);
        }

        #endregion

        #region Insert Methods


        /// <summary>
        /// Add RoleToDepartment record
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public int Insert(Guid roleId, int departmentId)
        {
            RoleToDepartment.RoleToDepartmentDataTable RoleToDepartmentExist = Adapter.GetRoleToDepartmentByRoleIdAndDepartmentId(roleId, departmentId);

            if (RoleToDepartmentExist.Rows.Count == 0)
            {
                RoleToDepartment.RoleToDepartmentDataTable dt = new RoleToDepartment.RoleToDepartmentDataTable();
                RoleToDepartment.RoleToDepartmentRow r = dt.NewRoleToDepartmentRow();

                r.RoleId = roleId;
                r.DepartmentId = departmentId;

                dt.AddRoleToDepartmentRow(r);
                Adapter.Update(dt);
                int id = r.Id;

                if (id > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.RoleToDepartment, id.ToString(), OperationTypeEnum.Insert);
                }

                return id;
            }
            else
                return -1;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update department by roleid
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public bool UpdateDepartmetIdByRoleId(Guid roleId, int departmentId)
        {
            RoleToDepartment.RoleToDepartmentDataTable roleToDepartment = Adapter.GetRoleToDepartmentByRoleId(roleId);

            if (roleToDepartment.Count == 0)
                return false;

            RoleToDepartment.RoleToDepartmentRow roleToDepartmentRow = roleToDepartment[0];

            roleToDepartmentRow.DepartmentId = departmentId;

            int rowsAffected = Adapter.Update(roleToDepartment);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.RoleToDepartment, roleToDepartmentRow.Id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete AppDocRef by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.RoleToDepartment, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }

        #endregion

    }
}
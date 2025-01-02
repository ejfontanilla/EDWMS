using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using DepartmentTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    [System.ComponentModel.DataObject]
    public class DepartmentDb
    {
        private DepartmentTableAdapter _DepartmentAdapter = null;

        protected DepartmentTableAdapter Adapter
        {
            get
            {
                if (_DepartmentAdapter == null)
                    _DepartmentAdapter = new DepartmentTableAdapter();

                return _DepartmentAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all Department
        /// </summary>
        /// <returns></returns>
        public Department.DepartmentDataTable GetDepartment()
        {
            Department.DepartmentDataTable dataTable = Adapter.GetDepartment();
            return dataTable;
        }

        /// <summary>
        /// Get all Department for display
        /// </summary>
        /// <returns></returns>
        public Department.DepartmentDataTable GetDepartmentForDisplay()
        {
            Department.DepartmentDataTable dataTable = Adapter.GetDepartmentForDisplay();
            return dataTable;
        }

        /// <summary>
        /// Get Department by id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Department.DepartmentDataTable GetDepartmentById(int Id)
        {
            return Adapter.GetDepartmentById(Id);
        }

        /// <summary>
        /// Get Department and Section for dropdown display
        /// </summary>
        /// <returns></returns>
        public DataTable GetDepartmentSectionForDropDown()
        {
            return DepartmentDs.GetDepartmentSectionForDropDown();
        }

        /// <summary>
        /// Get Department By UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Department.DepartmentDataTable GetDepartmentByUserId(Guid userId)
        {
            return Adapter.GetDepartmentByUserId(userId);
        }

        /// <summary>
        /// Get Department Id by UserGuid
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetDepartmentIdByUserId(Guid userId)
        {
            Department.DepartmentDataTable department = Adapter.GetDepartmentByUserId(userId);
            if (department.Rows.Count > 0)
            {
                Department.DepartmentRow departmentRow = department[0];
                return departmentRow.Id;
            }
            else
                return -1;
        }

        /// <summary>
        /// Get Department and Section for dropdown grid display
        /// </summary>
        /// <returns></returns>
        public DataTable GetDepartmentSectionForDropDownGrid()
        {
            return DepartmentDs.GetDepartmentSectionForDropDownGrid();
        }

        public string GetDepartmentMailingList(DepartmentCodeEnum department)
        {
            string email = string.Empty;

            Department.DepartmentDataTable dt = Adapter.GetDataByCode(department.ToString());

            if (dt.Rows.Count > 0)
            {
                Department.DepartmentRow dr = dt[0];
                email = dr.MailingList;
            }

            return email;
        }
        #endregion

        #region Insert Method

        /// <summary>
        /// Insert Department
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public int Insert(string name, string code, string mailingList)
        {
            Department.DepartmentDataTable department = new Department.DepartmentDataTable();
            Department.DepartmentRow r = department.NewDepartmentRow();
            r.Name = name;
            r.Code = code;
            r.MailingList = mailingList;

            department.AddDepartmentRow(r);
            Adapter.Update(department);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Department, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

       #endregion

        #region Update Method

        /// <summary>
        /// Update Department
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Update(int id, string name, string code, string mailingList)
        {
            Department.DepartmentDataTable department = Adapter.GetDepartmentById(id);

            if (department.Count == 0)
                return false;

            Department.DepartmentRow row = department[0];

            row.Name = name;
            row.Code = code;
            row.MailingList = mailingList;

            int rowsAffected = Adapter.Update(department);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Department, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        #endregion

        #region Checking Methods
        public bool NameExists(string name, int? id)
        {
            int count = (id == null) ?
                (int)Adapter.CountDepartmentByName(Format.ReplaceDoubleSpaceWithSingleSpace(name)) :
                (int)Adapter.CountDepartmentName(id.Value, Format.ReplaceDoubleSpaceWithSingleSpace(name));
            return (count > 0);
        }

        public bool CodeExists(string code, int? id)
        {
            int count = (id == null) ?
                (int)Adapter.CountDepartmentByCode(Format.ReplaceDoubleSpaceWithSingleSpace(code)) :
                (int)Adapter.CountDepartmentCode(id.Value, Format.ReplaceDoubleSpaceWithSingleSpace(code));
            return (count > 0);
        }
        #endregion
    }
}
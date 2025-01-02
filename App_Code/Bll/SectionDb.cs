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
using SectionTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    [System.ComponentModel.DataObject]
    public class SectionDb
    {
        private SectionTableAdapter _SectionAdapter = null;

        protected SectionTableAdapter Adapter
        {
            get
            {
                if (_SectionAdapter == null)
                    _SectionAdapter = new SectionTableAdapter();

                return _SectionAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all Operations
        /// </summary>
        /// <returns></returns>
        public Section.SectionDataTable GetSection()
        {
            Section.SectionDataTable dataTable = Adapter.GetSection();
            return dataTable;
        }

        /// <summary>
        /// Get Section By Order DepartmentId, Name
        /// </summary>
        /// <returns></returns>
        public Section.SectionDataTable GetSectionByOrder()
        {
            Section.SectionDataTable dataTable = Adapter.GetSectionByOrder();
            return dataTable;
        }        

        /// <summary>
        /// Get Section by Department
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Section.SectionDataTable GetSectionByDepartment(int id)
        {
            return Adapter.GetSectionByDepartment(id);
        }

        public DataTable GetSectionByDepartmentWithMailingList(int id)
        {
            return SectionDs.GetSectionByDepartmentWithMailingList(id);
        }

        /// <summary>
        /// Get Section for display by department
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Section.SectionDataTable GetSectionForDisplayByDepartment(int id)
        {
            return Adapter.GetSectionForDisplayByDepartment(id);
        }

        /// <summary>
        /// get Section by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Section.SectionDataTable GetSectionById(int id)
        {
            return Adapter.GetSectionById(id);
        }

        /// <summary>
        /// Get Section and corresponding department details.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataTable GetSectionDepartmentDetail(int id)
        {
            return SectionDs.GetSectionDepartmentDetail(id);
        }
        
        /// <summary>
        /// Check if the docset belong to a given department
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public Boolean IsDocSetSectionFromDepartment(int departmentId, int docSetId)
        {
            Section.SectionDataTable section = Adapter.GetDocSetSectionFromDepartmentId(departmentId, docSetId);
            return section.Rows.Count > 0 ;
        }

        #endregion

        #region Insert Methods

        /// <summary>
        /// Insert section
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="businessCode"></param>
        /// <returns></returns>
        public int Insert(string name, string code, string businessCode)
        {
            Section.SectionDataTable section = new Section.SectionDataTable();
            Section.SectionRow r = section.NewSectionRow();
            r.Name = name;
            r.Code = code;
            r.BusinessCode = businessCode;

            section.AddSectionRow(r);
            Adapter.Update(section);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Section, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

        #endregion

        #region Update Method

        /// <summary>
        /// Update Section
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="businessCode"></param>
        /// <returns></returns>
        public bool Update(int id, string name, string code, string businessCode)
        {
            Section.SectionDataTable section = Adapter.GetSectionById(id);

            if (section.Count == 0)
                return false;

            Section.SectionRow row = section[0];

            row.Name = name;
            row.Code = code;
            row.BusinessCode = businessCode;

            int rowsAffected = Adapter.Update(section);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Section, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        #endregion

        #region Checking Methods

        /// <summary>
        /// Check if same name exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool NameExists(string name, int? id)
        {
            int count = (id == null) ?
                (int)Adapter.CountSectionByName(Format.ReplaceDoubleSpaceWithSingleSpace(name)) :
                (int)Adapter.CountSectionName(id.Value, Format.ReplaceDoubleSpaceWithSingleSpace(name));
            return (count > 0);
        }

        /// <summary>
        /// Check if same code exist
        /// </summary>
        /// <param name="code"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CodeExists(string code, int? id)
        {
            int count = (id == null) ?
                (int)Adapter.CountSectionByCode(Format.ReplaceDoubleSpaceWithSingleSpace(code)) :
                (int)Adapter.CountSectionCode(id.Value, Format.ReplaceDoubleSpaceWithSingleSpace(code));
            return (count > 0);
        }

        /// <summary>
        /// Check if the user belong to certain BusinessCode
        /// </summary>
        /// <param name="sectionBusinessCodeEnum"></param>
        /// <returns></returns>
        public bool isUserBusinessCode(SectionBusinessCodeEnum sectionBusinessCodeEnum)
        {
            MembershipUser user = Membership.GetUser();

            if (user != null)
            {
                Guid userId = new Guid(user.ProviderUserKey.ToString());
                DataTable dtProfile = ProfileDs.GetProfileInfo(userId);
                if (dtProfile.Rows.Count > 0)
                    return (dtProfile.Rows[0]["BusinessCode"].ToString().ToLower().Trim().Equals(sectionBusinessCodeEnum.ToString().ToLower()));
                else
                    return false;
            }
            else
                return false;
        }

        #endregion
    }
}
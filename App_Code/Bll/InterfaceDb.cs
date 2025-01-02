using System;
using System.Collections.Generic;
using System.Web;
using InterfaceTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for InterfaceDb
    /// </summary>
    public class InterfaceDb
    {
        private InterfaceTableAdapter _InterfaceAdapter = null;

        protected InterfaceTableAdapter Adapter
        {
            get
            {
                if (_InterfaceAdapter == null)
                    _InterfaceAdapter = new InterfaceTableAdapter();

                return _InterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces
        /// </summary>
        /// <returns></returns>
        public Interface.InterfaceDataTable GetInterface()
        {
            return Adapter.GetData();
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="streetCode"></param>
        /// <param name="streetName"></param>
        /// <returns></returns>
        public int Insert(string section, string refNo, string refDate, string updateDate, string refStatus,
            string peOic, string caOic, string addressBlk, string address, string postal, string personalType,
            string nric, string name, string maritalStatusCode, string maritalStatus, string dateOfBirth,
            string relationshipCode, string relationship, string incYear, string incAverage, string employerName,
            string dateJoined)
        {
            Interface.InterfaceDataTable dt = new Interface.InterfaceDataTable();
            Interface.InterfaceRow r = dt.NewInterfaceRow();

            r.Section = section;
            r.RefNo = refNo;
            r.RefDate = refDate;
            r.UpdateDate = updateDate;
            r.RefStatus = refStatus;
            r.Peoic = peOic;
            r.Caoic = caOic;
            r.AddressBlk = addressBlk;
            r.Address = address;
            r.Postal = postal;
            r.PersonalType = personalType;
            r.Nric = nric;
            r.Name = name;
            r.MaritalStatusCode = maritalStatusCode;
            r.MaritalStatus = maritalStatus;
            r.DateOfBirth = dateOfBirth;
            r.RelationshipCode = relationshipCode;
            r.Relationship = relationship;
            r.IncYear = incYear;
            r.IncAverage = incAverage;
            r.EmployerName = employerName;
            r.DateJoined = dateJoined;

            dt.AddInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Interface, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion
    }
}
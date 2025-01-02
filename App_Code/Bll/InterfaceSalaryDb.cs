using System;
using System.Collections.Generic;
using System.Web;
using InterfaceSalaryTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for InterfaceSalaryDb
    /// </summary>
    public class InterfaceSalaryDb
    {
        private InterfaceSalaryTableAdapter _InterfaceSalaryAdapter = null;

        protected InterfaceSalaryTableAdapter Adapter
        {
            get
            {
                if (_InterfaceSalaryAdapter == null)
                    _InterfaceSalaryAdapter = new InterfaceSalaryTableAdapter();

                return _InterfaceSalaryAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces salary
        /// </summary>
        /// <returns></returns>
        public InterfaceSalary.InterfaceSalaryDataTable GetInterfaceSalary()
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
        public int Insert(int interfaceId, string month1Name, string month1Value, string month2Name, string month2Value, 
            string month3Name, string month3Value, string month4Name, string month4Value, string month5Name, string month5Value,
            string month6Name, string month6Value, string month7Name, string month7Value, string month8Name, string month8Value,
            string month9Name, string month9Value, string month10Name, string month10Value, string month11Name, string month11Value,
            string month12Name, string month12Value)
        {
            InterfaceSalary.InterfaceSalaryDataTable dt = new InterfaceSalary.InterfaceSalaryDataTable();
            InterfaceSalary.InterfaceSalaryRow r = dt.NewInterfaceSalaryRow();

            r.InterfaceId = interfaceId;
            r.Month1Name = month1Name;
            r.Month1Value = month1Value;
            r.Month2Name = month2Name;
            r.Month2Value = month2Value;
            r.Month3Name = month3Name;
            r.Month3Value = month3Value;
            r.Month4Name = month4Name;
            r.Month4Value = month4Value;
            r.Month5Name = month5Name;
            r.Month5Value = month5Value;
            r.Month6Name = month6Name;
            r.Month6Value = month6Value;
            r.Month7Name = month7Name;
            r.Month7Value = month7Value;
            r.Month8Name = month8Name;
            r.Month8Value = month8Value;
            r.Month9Name = month9Name;
            r.Month9Value = month9Value;
            r.Month10Name = month10Name;
            r.Month10Value = month10Value;
            r.Month11Name = month11Name;
            r.Month11Value = month11Value;
            r.Month12Name = month12Name;
            r.Month12Value = month12Value;

            dt.AddInterfaceSalaryRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.InterfaceSalary, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion
    }
}
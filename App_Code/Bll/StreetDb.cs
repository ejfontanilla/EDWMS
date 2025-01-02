using System;
using System.Collections.Generic;
using System.Web;
using StreetTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for StreetDb
    /// </summary>
    public class StreetDb
    {
        private StreetTableAdapter _StreetAdapter = null;

        protected StreetTableAdapter Adapter
        {
            get
            {
                if (_StreetAdapter == null)
                    _StreetAdapter = new StreetTableAdapter();

                return _StreetAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get the streets
        /// </summary>
        /// <returns></returns>
        public Street.StreetDataTable GetStreet()
        {
            return Adapter.GetStreets();
        }

        public Street.StreetDataTable GetStreetById(int id)
        {
            return Adapter.GetDataById(id);
        }

        public Street.StreetDataTable GetStreetByCode(string streetCode)
        {
            return Adapter.GetStreetByCode(streetCode);
        }

        /// <summary>
        /// Get Streets by Name
        /// </summary>
        /// <param name="streetName"></param>
        /// <returns></returns>
        public Street.StreetDataTable GetStreetByName(string streetName)
        {
            return Adapter.GetStreetByName(streetName);
        }
        
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="streetCode"></param>
        /// <param name="streetName"></param>
        /// <returns></returns>
        public int Insert(string streetCode, string streetName)
        {
            Street.StreetDataTable dt = new Street.StreetDataTable();
            Street.StreetRow r = dt.NewStreetRow();

            r.Code = streetCode;
            r.Name = streetName;

            dt.AddStreetRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                //AuditTrailDb auditTrailDb = new AuditTrailDb();
                //auditTrailDb.Record(TableNameEnum.Street, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion

        #region Update Methods
        public bool Update(int id, string streetCode, string streetName)
        {
            Street.StreetDataTable dt = GetStreetById(id);

            if (dt.Rows.Count == 0)
                return false;

            Street.StreetRow row = dt[0];
            row.Code = streetCode;
            row.Name = streetName;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                //AuditTrailDb auditTrailDb = new AuditTrailDb();
                //auditTrailDb.Record(TableNameEnum.Street, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }
        #endregion
    }
}
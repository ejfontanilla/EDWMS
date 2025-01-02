using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Runtime.InteropServices; 
using SocInterfaceTableAdapters;
using System.Web.Security;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class SocInterfaceDb
    {
        private SocInterfaceTableAdapter _SocInterfaceAdapter = null;

        protected SocInterfaceTableAdapter Adapter
        {
            get
            {
                if (_SocInterfaceAdapter == null)
                    _SocInterfaceAdapter = new SocInterfaceTableAdapter();

                return _SocInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public SocInterface.SocInterfaceDataTable GetSocInterface()
        {
            return Adapter.GetData();
        }

        public SocInterface.SocInterfaceDataTable GetSocInterfaceById(int id)
        {
            return Adapter.GetDataById(id);
        }

        public SocInterface.SocInterfaceDataTable GetSocInterfaceByRegistrationNoAndNric(string registrationNo, string nric)
        {
            return Adapter.GetDataByRegistrationNoAndNric(registrationNo, nric);
        }

        public bool SocInterfaceExists(string registrationNo, string nric)
        {
            return GetSocInterfaceByRegistrationNoAndNric(registrationNo, nric).Rows.Count > 0;
        }

        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert doc app
        /// </summary>
        /// <returns></returns>
        public int Insert(string registrationNo, string nric, string docTypeCode, string docType, string docStartDate, string docEndDate, 
            bool isRequired, bool isReceived, string exception, string remarks)
        {
            SocInterface.SocInterfaceDataTable dt = new SocInterface.SocInterfaceDataTable();
            SocInterface.SocInterfaceRow r = dt.NewSocInterfaceRow();

            r.RegistrationNo = registrationNo;
            r.Nric = nric;
            r.DocTypeCode = docTypeCode;
            r.DocType = docType;
            r.DocStartDate = (docStartDate.Equals(".") ? string.Empty : docStartDate);
            r.DocEndDate = (docEndDate.Equals(".") ? string.Empty : docEndDate);
            r.IsReceived = isReceived;
            r.IsRequired = isRequired;
            r.Exception = exception;
            r.Remarks = remarks;

            dt.AddSocInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            return id;
        }
        #endregion

        #region Update Methods
        public bool Update(int id, string exception, bool isReceived)
        {
            SocInterface.SocInterfaceDataTable dt = GetSocInterfaceById(id);

            if (dt.Rows.Count == 0) return false;

            SocInterface.SocInterfaceRow r = dt[0];

            r.Exception = exception;
            r.IsReceived = isReceived;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #region Delete Methods
        public bool DeleteByRegistrationNoAndNric(string registrationNo, string nric)
        {
            return Adapter.DeleteByRegistrationNoAndNric(registrationNo, nric) > 0;
        }
        #endregion
    }
}
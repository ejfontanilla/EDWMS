using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Runtime.InteropServices; 
using LeasInterfaceTableAdapters;
using System.Web.Security;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class LeasInterfaceDb
    {
        private LeasInterfaceTableAdapter _LeasInterfaceAdapter = null;

        protected LeasInterfaceTableAdapter Adapter
        {
            get
            {
                if (_LeasInterfaceAdapter == null)
                    _LeasInterfaceAdapter = new LeasInterfaceTableAdapter();

                return _LeasInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public LeasInterface.LeasInterfaceDataTable GetLeasInterface()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public LeasInterface.LeasInterfaceDataTable GetLeasInterfaceByByHleNumberAndNric(string hleNumber, string nric)
        {
            return Adapter.GetDataByHleNumberAndNric(hleNumber, nric);
        }

        /// <summary>
        /// Get data by HleNumber
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public LeasInterface.LeasInterfaceDataTable GetLeasInterfaceByByHleNumber(string hleNumber)
        {
            return Adapter.GetDataByHleNumber(hleNumber);
        }

        //check for given application if allthe sets are verified or closed
        public bool HasPendingDoc(string hleNumber)
        {
            LeasInterface.LeasInterfaceDataTable leasdt = GetLeasInterfaceByByHleNumber(hleNumber);

            //if ((hleNumber.Substring(0, 1) == "R" && hleNumber.Substring(3, 1) == "F") || (hleNumber.Substring(0, 1) == "R" && hleNumber.Substring(3, 1) == "N") || (hleNumber.Substring(0, 1) == "T" && hleNumber.Substring(3, 1) == "T"))//interim period only (for RxxF, RxxN, TxxT)
            //{
                if (leasdt.Rows.Count > 0)
                {
                    foreach (LeasInterface.LeasInterfaceRow leas in leasdt.Rows)
                    {
                        if ((leas.Status.Trim().Equals(leasInterfaceStatusEnum.PEN.ToString())))
                            return true;
                    }
                    return false;
                }
                else
                    return true;
            //}
            //else
            //    return true;
        }

        //public Boolean HasPendingDoc(string hleNumber)
        //{
        //    LeasInterface.LeasInterfaceDataTable leasdt = GetLeasInterfaceByByHleNumber(hleNumber);

        //    if (leasdt.Rows.Count > 0)
        //    {
        //        //foreach (LeasInterface.LeasInterfaceRow leas in leasdt.Rows)
        //        //{
        //            //LeasInterface.LeasInterfaceRow leas;
        //            //LeasInterface.LeasInterfaceRow leas2;
        //            for (int i = 0; i < leasdt.Rows.Count; i++)
        //            {
        //                LeasInterface.LeasInterfaceRow leas = ((LeasInterface.LeasInterfaceRow)leasdt.Rows[i]);
        //                if (leas.IsCategoryGroupNull())
        //                {
        //                    if ((leas.Status.Trim().Equals(leasInterfaceStatusEnum.PEN.ToString())))
        //                        return true;
        //                }
        //                else
        //                {
        //                    for (int j = i + 1; j < leasdt.Rows.Count; j++)
        //                    {
        //                        LeasInterface.LeasInterfaceRow leas2 = ((LeasInterface.LeasInterfaceRow)leasdt.Rows[j]);
        //                        if (leas.CategoryGroup == leas2.CategoryGroup && leas.Category == leas2.Category && leas.CategoryDate == leas2.CategoryDate)
        //                            if (leas.Status.Trim().Equals(leasInterfaceStatusEnum.PEN.ToString()) && leas2.Status.Trim().Equals(leasInterfaceStatusEnum.PEN.ToString()))
        //                                return true;
        //                            else
        //                                i = j + 1;
        //                    }
        //                }
        //            }
        //            //else
        //            //return false;
                
        //    }
        //    return false;
        //}

        public DataTable GetLeasInterfaceByHleNumberAndNricForPendingDocumentDisplay(string hleNumber, string nric)
        {
            return LeasInterfceDs.GetLeasInterfaceByHleNumberAndNricForPendingDocumentDisplay(hleNumber, nric);
        }

        /// <summary>
        /// Get Count by Hle number(reference no)
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public int GetCountByHleNumber(string hleNumber)
        {
            int? result = Adapter.GetCountByHleNumber(hleNumber);
            return (result.HasValue ? result.Value : 0);
        }

        /// <summary>
        /// Get Count by Hle number(reference no) group by CategoryInfo
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public int GetCountByHleNUmberGroupByCategoryInfo(string hleNumber, leasInterfaceStatusEnum status)
        {
            int? result = (int)Adapter.GetCountByHleNUmberGroupByCategoryInfo(hleNumber, status.ToString());
            return (result.HasValue ? result.Value : 0);
        }

        public int GetCountByHleNUmberGroupByCategoryInfo(string hleNumber)
        {
            //get unique nric for the reference number
            LeasInterface.LeasInterfaceDataTable leasInterface = GetLeasInterfaceByByHleNumber(hleNumber);
            LeasInterfceDs leasInterfceDs = new LeasInterfceDs();

            if (leasInterface.Rows.Count > 0)
            {
                string nricUnique = string.Empty;
                string nric = string.Empty;
                int totalCount = 0;
                foreach (LeasInterface.LeasInterfaceRow leasInterfaceRow in leasInterface.Rows)
                {
                    nric = leasInterfaceRow.Nric.ToString().Trim();
                    if (!nricUnique.Contains(nric + ";"))
                    {
                        totalCount += LeasInterfceDs.GetLeasInterfaceByHleNumberAndNricForPendingDocumentDisplay(hleNumber, nric).Rows.Count;
                        nricUnique += nric + ";";
                    }
                }
                return totalCount;
            }
            else
                return 0;
        }

        /// <summary>
        /// Get Count of Outstanding documents with no remarks
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public int GetCountOutstandingDocumentsWithNoRemarksByHleNumber(string hleNumber)
        {
            int? result = Adapter.GetCountOutstandingDocumentsWithNoRemarksByHleNumber(hleNumber);
            return (result.HasValue ? result.Value : 0);
        }

        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public LeasInterface.LeasInterfaceDataTable GetLeasInterfaceById(int id)
        {
            return Adapter.GetDataById(id);
        }

        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public bool LeasInterfaceExists(string hleNumber, string nric)
        {
            return Adapter.GetDataByHleNumberAndNric(hleNumber, nric).Rows.Count > 0;
        }

        public LeasInterface.LeasInterfaceDataTable GetLeasInterfaceByByHleNumberNricAndDocTypeCode(string hleNumber, string nric, string docTypeCode)
        {
            return Adapter.GetDataByHleNumberNricAndDocTypeCode(hleNumber, nric, docTypeCode);
        }

        public LeasInterface.LeasInterfaceDataTable GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(string hleNumber, string nric, string docTypeCode, string docStartDate)
        {
            return Adapter.GetDataByHleNumberNricDocTypeCodeAndDocStartDate(hleNumber, nric, docTypeCode, docStartDate);
        }

        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert doc app
        /// </summary>
        /// <returns></returns>
        public int Insert(string hleNumber, string nric, string docTypeCode, string docStartDate, string docEndDate,
            bool isRequired, bool isReceived, string exception, string remarks, string category, string categoryGroup, string categoryDate, string status)
        {
            LeasInterface.LeasInterfaceDataTable dt = new LeasInterface.LeasInterfaceDataTable();
            LeasInterface.LeasInterfaceRow r = dt.NewLeasInterfaceRow();

            categoryDate = categoryDate.Trim();
            docStartDate = docStartDate.Trim();
            docEndDate = docEndDate.Trim();

            r.HleNumber = hleNumber;
            r.Nric = nric;
            r.DocTypeCode = docTypeCode.Trim();
            r.DocStartDate = (docStartDate.Equals(".") || docStartDate.Equals(Constants.WebServiceNullDate) ? string.Empty : docStartDate);
            r.DocEndDate = (docEndDate.Equals(".") || docEndDate.Equals(Constants.WebServiceNullDate) ? string.Empty : docEndDate);
            r.IsReceived = isReceived;
            r.IsRequired = isRequired;
            r.Exception = exception.Trim();
            r.Remarks = remarks.Trim();
            r.DocTypeCode = docTypeCode.Trim();
            r.Category = category.Trim();
            r.CategoryGroup = categoryGroup.Trim();
            if (string.IsNullOrEmpty(categoryDate) || categoryDate.Equals(".") || categoryDate.Equals("0") || categoryDate.Equals(Constants.WebServiceNullDate))
                r.SetCategoryDateNull();
            else
                r.CategoryDate = DateTime.Parse(categoryDate); 

            r.Status = status.Trim();

            dt.AddLeasInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            return id;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ocrText"></param>
        /// <param name="isOcr"></param>
        /// <returns></returns>
        public bool Update(int id, string exception, bool isReceived)
        {
            LeasInterface.LeasInterfaceDataTable dt = GetLeasInterfaceById(id);

            if (dt.Rows.Count == 0) return false;

            LeasInterface.LeasInterfaceRow r = dt[0];

            r.Exception = exception;
            r.IsReceived = isReceived;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool Update(string hleNumber, string nric, string docTypeCode, string docStartDate, string docEndDate,
            bool isRequired, bool isReceived, string exception, string remarks, string category, string categoryGroup, string categoryDate, string status)
        {
            LeasInterface.LeasInterfaceDataTable dt = GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(hleNumber, nric, docTypeCode, docStartDate);

            if (dt.Rows.Count == 0) return false;

            categoryDate = categoryDate.Trim();
            docStartDate = docStartDate.Trim();
            docEndDate = docEndDate.Trim();

            LeasInterface.LeasInterfaceRow r = dt[0];

            r.DocStartDate = (docStartDate.Equals(".") || docStartDate.Equals(Constants.WebServiceNullDate) ? string.Empty : docStartDate);
            r.DocEndDate = (docEndDate.Equals(".") || docEndDate.Equals(Constants.WebServiceNullDate) ? string.Empty : docEndDate);
            r.IsReceived = isReceived;
            r.IsRequired = isRequired;
            r.Exception = exception.Trim();
            r.Remarks = remarks.Trim();
            r.DocTypeCode = docTypeCode.Trim();
            r.Category = category.Trim();
            r.CategoryGroup = categoryGroup.Trim();
            r.Status = status.Trim();

            if (string.IsNullOrEmpty(categoryDate) || categoryDate.Equals(".") || categoryDate.Equals(Constants.WebServiceNullDate))
                r.SetCategoryDateNull();
            else
                r.CategoryDate = DateTime.Parse(categoryDate);

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #region Delete Methods
        public bool Delete(string hleNumber, string nric)
        {
            return Adapter.DeleteByHleNumberAndNric(hleNumber, nric) > 0;
        }

        public bool Delete(string hleNumber)
        {
            return Adapter.DeleteByHleNumber(hleNumber) > 0;
        }
        #endregion
    }
}
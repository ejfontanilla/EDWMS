using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using DocTypeTableAdapters;
using Dwms.Dal;
namespace Dwms.Bll
{
    public class DocTypeDb
    {
        private DocTypeTableAdapter _DocTypeAdapter = null;

        protected DocTypeTableAdapter Adapter
        {
            get
            {
                if (_DocTypeAdapter == null)
                    _DocTypeAdapter = new DocTypeTableAdapter();

                return _DocTypeAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get DocType
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocType.DocTypeDataTable GetDocType()
        {
            return Adapter.GetDocTypes();
        }

        /// <summary>
        /// Get DocType by code
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocType.DocTypeDataTable GetDocType(string code)
        {
            return Adapter.GetDocTypeByCode(code);
        }

        /// <summary>
        /// Get DocType by code with abbreviation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocType.DocTypeDataTable GetDocTypeWithAbbreviation(string code)
        {
            DocType.DocTypeDataTable dt = GetDocType(code);
            return AddDocTypeAbbreviation(dt);
        }

        /// <summary>
        /// Get DocType by visibility
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocType.DocTypeDataTable GetDocType(bool show)
        {
            if (show)
                return Adapter.GetDocTypeWithRule();
            else
                return Adapter.GetDocTypeWithoutRule();
        }

        /// <summary>
        /// Get DocType by visibility with abbreviation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocType.DocTypeDataTable GetDocTypeWithAbbreviation(bool show)
        {
            return GetDocType(show);
            //return AddDocTypeAbbreviation(dt);
        }
        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        /// <summary>
        /// Get DocTypeCode by DocumentID and DocumentSubId
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="documentSubId"></param>
        /// <returns></returns>
        //public string GetDocTypeCodeByDocumentIdAndDocumentSubId(string documentId, string documentSubId)
        //{
        //    DocType.DocTypeDataTable docType = null;

        //    if (string.IsNullOrEmpty(documentSubId) || documentSubId.Trim() == "00")
        //    {
        //        //2013-01-28: modified to ensure 00 codes are also considered
        //       docType = Adapter.GetDataByDocumentIdAndDocumentSubIdAsNull(documentId);
        //    }
        //    else
        //        docType = Adapter.GetDataByDocumentIdAndDocumentSubId(documentId, documentSubId);

        //    if (docType.Rows.Count > 0)
        //    {
        //        DocType.DocTypeRow doctypeRow = docType[0];
        //        return doctypeRow.Code.Trim();
        //    }
        //    else
        //        return string.Empty;
        //}


        public string GetDocTypeCodeByDocumentIdAndDocumentSubId(string documentId, string documentSubId)
        {
            string docTypeCode = string.Empty;

            if (string.IsNullOrEmpty(documentSubId) || documentSubId.Trim() == "00")
            {
                //2013-01-28: modified to ensure 00 codes are also considered
                docTypeCode = DocTypeDs.GetDocTypeCodeByDocumentIdAndDocumentSubIdAsNull(documentId);
            }
            else                
                docTypeCode = DocTypeDs.GetDocTypeCodeByDocumentIdAndDocumentSubId(documentId, documentSubId);

            return docTypeCode;
        }

        #endregion

        #endregion

        #region Checking Methods

        /// <summary>
        /// Check if there are doc types not shown
        /// </summary>
        /// <returns></returns>
        public bool HasDocTypeNotShown()
        {
            bool result = false;

            DocType.DocTypeDataTable dt = GetDocType(false);
            result = dt.Rows.Count > 0;

            return result;
        }

        /// <summary>
        /// Get the visibility of the doc type
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsDocTypeEditable(string code)
        {
            bool editable = false;

            DocType.DocTypeDataTable dt = GetDocType(code);

            if (dt.Rows.Count > 0)
            {
                DocType.DocTypeRow dr = dt[0];
                editable = dr.IsEditable;
            }

            return editable;
        }    
        #endregion

        #region Update Methods
        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(string code, string value)
        {
            //int id = GetIdByName(name);
            //if (id <= 0)
            //{
            //    Adapter.Insert(name.ToString(), value);
            //}

            //int rowsAffected = Adapter.UpdateDocType(value, name.ToString());

            //return rowsAffected == 1;
            return true;
        }

        //Added By Edward 26/3/2014 Freeze Sample Documents
        //Added by Edward Displaying Active DocTypes 2015/8/17 Added New Parameter IsActive
        //public bool UpdateAcquireNewSamples(string code, bool acquireNewSamples)
        public bool UpdateAcquireNewSamples(string code, bool acquireNewSamples, bool isActive)
        {
            DocType.DocTypeDataTable dt = Adapter.GetDocTypeByCode(code);

            if (dt.Count == 0)
                return false;

            DocType.DocTypeRow row = dt[0];

            row.AcquireNewSamples = acquireNewSamples;
            row.IsActive = isActive;        //Added by Edward Displaying Active DocTypes 2015/8/17 Added New Parameter IsActive             

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocType, code, OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }


        #endregion

        #region Private Methods
        /// <summary>
        /// Add the doc type abbreviation
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DocType.DocTypeDataTable AddDocTypeAbbreviation(DocType.DocTypeDataTable dt)
        {
            DocType.DocTypeDataTable finalDt = new DocType.DocTypeDataTable();
            finalDt.Columns.Add("DocTypeAbbr", typeof(string));

            foreach (DocType.DocTypeRow dr in dt.Rows)
            {
                DocType.DocTypeRow finalDr = finalDt.NewDocTypeRow();
                finalDr.Code = dr.Code;
                finalDr.Description = dr.Description;
                finalDr.IsEditable = dr.IsEditable;
                finalDr["DocTypeAbbr"] = dr.Description + " (" + dr.Code + ")";

                finalDt.Rows.Add(finalDr);
            }

            return finalDt;
        }
        #endregion

        #region Added by Edward Displaying Active DocTypes 2015/8/17
        public DocType.DocTypeDataTable GetActiveDocTypes()
        {
            return Adapter.GetActiveDocTypes();
        }
        #endregion
    }
}
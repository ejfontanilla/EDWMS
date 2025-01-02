using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using AppDocRefTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AppAppDocRefRefDb
    /// </summary>
    public class AppDocRefDb
    {
        private AppDocRefTableAdapter _AppDocRefAdapter = null;

        protected AppDocRefTableAdapter Adapter
        {
            get
            {
                if (_AppDocRefAdapter == null)
                    _AppDocRefAdapter = new AppDocRefTableAdapter();

                return _AppDocRefAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public AppDocRef.AppDocRefDataTable GetAppDocRef()
        {
            return Adapter.GetData();
        }


        /// <summary>
        /// Get AppDocRef By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppDocRef.AppDocRefDataTable GetAppDocRefById(int id)
        {
            return Adapter.GetAppDocRefById(id);
        }

        /// <summary>
        /// Get AppDocRef by DocId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public AppDocRef.AppDocRefDataTable GetAppDocRefByDocId(int docId)
        {
            return Adapter.GetAppDocRefByDocId(docId);
        }

        public AppDocRef.AppDocRefDataTable GetAppDocRefByAppPersonalId(int appPersonalId)
        {
            return Adapter.GetDataByAppPersonalId(appPersonalId);
        }


        public AppDocRef.AppDocRefDataTable GetAppDocRefByDocSetIdAndDocAppId(int docSetId, int docAppId)
        {
            return Adapter.GetAppDocRefByDocSetIdAndDocAppId(docSetId, docAppId);
        }

        #region Added By Edward 10/01/2014 to address "Identity No should be part of household structure" Issue
        public AppDocRef.AppDocRefDataTable GetAppDocRefByAppPersonalIdAndDocId(int appPersonalId, int docId)
        {
            return Adapter.GetAppDocRefByAppPersonalIdAndDocId(appPersonalId, docId);
        }
        #endregion

        /// <summary>
        /// Get AppDocRef by DocId and RefId
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public AppDocRef.AppDocRefDataTable GetAppDocRefByDocIdAndRefId(int docId, int refId)
        {
            return Adapter.GetAppDocRefByDocIdAndRefId(docId, refId);
        }

        public bool DocHasMultiplePersonalRefBesideGivenPersonal(int docId, int appPersonalId)
        {
            int? count = Adapter.CountRefBesidesGivenAppPersonalId(docId, appPersonalId);

            return (count.HasValue && count.Value > 0);
        }

        public AppDocRef.AppDocRefDataTable GetAppDocRefByDocAppId(int docAppId)
        {
            return Adapter.GetAppDocRefByDocAppId(docAppId);
        }
        #endregion

        #region Insert Methods

        /// <summary>
        /// Add AppDocRef
        /// </summary>
        /// <param name="appPersonalId"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int Insert(int appPersonalId, int docId)
        {
            AppDocRef.AppDocRefDataTable AppDocRefExist = Adapter.GetAppDocRefByAppPersonalIdAndDocId(appPersonalId, docId);

            if (AppDocRefExist.Rows.Count == 0)
            {
                AppDocRef.AppDocRefDataTable dt = new AppDocRef.AppDocRefDataTable();
                AppDocRef.AppDocRefRow r = dt.NewAppDocRefRow();

                r.AppPersonalId = appPersonalId;
                r.DocId = docId;

                dt.AddAppDocRefRow(r);
                Adapter.Update(dt);
                int id = r.Id;
                return id;
            }
            else
                return -1;
        }

        /// <summary>
        /// Copy records form source DocId to des Doc Id
        /// </summary>
        /// <param name="sourceDocId"></param>
        /// <param name="desDocId"></param>
        public void CopyFromSource(int sourceDocId, int desDocId)
        {
            //get records from source DocId and duplicate them to des DocId
            AppDocRef.AppDocRefDataTable appDocRefs = Adapter.GetAppDocRefByDocId(sourceDocId);

            foreach (AppDocRef.AppDocRefRow appDocRefRow in appDocRefs.Rows)
            {
                AppDocRef.AppDocRefDataTable dt = new AppDocRef.AppDocRefDataTable();
                AppDocRef.AppDocRefRow r = dt.NewAppDocRefRow();

                r.AppPersonalId = appDocRefRow.AppPersonalId;
                r.DocId = desDocId;

                dt.AddAppDocRefRow(r);
                Adapter.Update(dt);
            }
        }

        #endregion

        #region Update Methods
        public bool UpdateAppPersonalId(int id, int appPersonalId)
        {
            AppDocRef.AppDocRefDataTable dt = GetAppDocRefById(id);

            if (dt.Count == 0)
                return false;

            AppDocRef.AppDocRefRow row = dt[0];

            row.AppPersonalId = appPersonalId;

            int rowsAffected = Adapter.Update(dt);

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
            Guid? operationId = auditTrailDb.Record(TableNameEnum.AppDocRef, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.AppDocRef, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Delete the appDocRef By docid
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public bool DeleteByDocId(int docId)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;
            AppDocRef.AppDocRefDataTable appDocRefs = GetAppDocRefByDocId(docId);

            foreach (AppDocRef.AppDocRefRow appDocRefRow in appDocRefs.Rows)
            {
                auditTrailDb.Record(TableNameEnum.AppDocRef, appDocRefRow.Id.ToString(), OperationTypeEnum.Delete);
                rowsAffected = Adapter.Delete(appDocRefRow.Id);
            }

            return (rowsAffected > 0);
        }

        #endregion

    }
}
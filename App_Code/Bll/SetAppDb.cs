using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using SetAppTableAdapters;
using System.Data;
using Dwms.Dal;
using System.Collections;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AppSetAppRefDb
    /// </summary>
    public class SetAppDb
    {
        private SetAppTableAdapter _SetAppAdapter = null;

        private vSetAppTableAdapter _vSetAppAdapter = null;

        protected SetAppTableAdapter Adapter
        {
            get
            {
                if (_SetAppAdapter == null)
                    _SetAppAdapter = new SetAppTableAdapter();

                return _SetAppAdapter;
            }
        }

        protected vSetAppTableAdapter vAdapter
        {
            get
            {
                if (_vSetAppAdapter == null)
                    _vSetAppAdapter = new vSetAppTableAdapter();

                return _vSetAppAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public SetApp.SetAppDataTable GetSetApp()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public SetApp.SetAppDataTable GetSetAppById(int id)
        {
            return Adapter.GetDataById(id);
        }

        /// <summary>
        /// Get SetApp by DocSetId
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public SetApp.SetAppDataTable GetSetAppByDocSetId(int docSetId)
        {
            return Adapter.GetSetAppByDocSetId(docSetId);
        }

        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public SetApp.vSetAppDataTable GetvSetAppByDocSetId(int docSetId)
        {
            return vAdapter.GetDataByDocSetId(docSetId);
        }

        /// <summary>
        /// Get by docAppId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public SetApp.SetAppDataTable GetSetAppByDocAppId(int docAppId)
        {
            return Adapter.GetSetAppByDocAppId(docAppId);
        }

        /// <summary>
        /// Check if current docset and docapp association exists
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public bool DoesLinkExists(int docSetId, int docAppId)
        {
            bool result = false;

            SetApp.vSetAppDataTable vDt = GetvSetAppByDocSetId(docSetId);
            
            foreach(SetApp.vSetAppRow vDr in vDt)
            {
                if (vDr.DocAppId == docAppId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="AppSetAppRefId"></param>
        /// <param name="docType"></param>
        /// <param name="originalSetId"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public int Insert(int docSetId, int docAppId)
        {
            SetApp.SetAppDataTable dt = new SetApp.SetAppDataTable();
            SetApp.SetAppRow r = dt.NewSetAppRow();

            r.DocSetId = docSetId;
            r.DocAppId = docAppId;

            dt.AddSetAppRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                // Save the personal records
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                appPersonalDb.SavePersonalRecords(docAppId);
            }

            return id;
        }

        /// <summary>
        /// Insert from Old Set Id
        /// </summary>
        /// <param name="oldSetId"></param>
        /// <param name="newSetId"></param>
        /// <returns></returns>
        public Boolean InsertFromOld(int oldSetId, int newSetId)
        {
            SetAppDb setAppDb  = new SetAppDb();
            SetApp.SetAppDataTable oldSetApps = setAppDb.GetSetAppByDocSetId(oldSetId);

            int id = -1;

            foreach (SetApp.SetAppRow oldSetAppRow in oldSetApps.Rows)
            {
                SetApp.SetAppDataTable newSetApps = new SetApp.SetAppDataTable();
                SetApp.SetAppRow newSetAppRow = newSetApps.NewSetAppRow();

                newSetAppRow.DocAppId = oldSetAppRow.DocAppId;
                newSetAppRow.DocSetId = newSetId;

                newSetApps.AddSetAppRow(newSetAppRow);
                Adapter.Update(newSetApps);

                id = newSetAppRow.Id;

                if (id > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Insert);
                }
            }

            return (id > 0);
        }
        #endregion

        #region Update Methods
        ///// <summary>
        ///// Update SetApp Status
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="status"></param>
        ///// <returns></returns>
        //public bool UpdateSetAppFromMeta(int id, SetAppStatusEnum status, string imageCondition, string docTypeCode, Boolean isConfirmed, LogTypeEnum logType)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.Status = status.ToString();
        //    row.ImageCondition = imageCondition;
        //    row.SetAppTypeCode = docTypeCode;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);

        //        //Insert LogAction
        //        MembershipUser user = Membership.GetUser();
        //        if (user == null)
        //            return false;

        //        LogActionDb logActionDb = new LogActionDb();
        //        Guid userId = (Guid)user.ProviderUserKey;

        //        if (isConfirmed)
        //            logActionDb.Insert(userId, LogActionEnum.Confirmed_metadata, string.Empty, string.Empty, string.Empty, logType, id);
        //        else
        //            logActionDb.Insert(userId, LogActionEnum.Saved_metadata_as_draft, string.Empty, string.Empty, string.Empty, logType, id);
        //    }
        //    return rowsAffected == 1;
        //}


        ///// <summary>
        ///// Update SetAppTypeCode
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docTypeCode"></param>
        ///// <returns></returns>
        //public bool UpdateSetApptype(int id, string docTypeCode)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.SetAppTypeCode = docTypeCode;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update SetAppFolder
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docFolder"></param>
        ///// <returns></returns>
        //public bool UpdateSetAppFolder(int id, string docFolder)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    if (string.IsNullOrEmpty(docFolder))
        //        row.SetSetAppFolderNull();
        //    else
        //        row.SetAppFolder = docFolder;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update SetAppument Status 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docStatus"></param>
        ///// <returns></returns>
        //public bool UpdateSetAppStatus(int id, SetAppStatusEnum docStatus)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.Status = docStatus.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}


        ///// <summary>
        ///// Update SetAppAppId
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docappid"></param>
        ///// <returns></returns>
        //public bool UpdateSetAppAppId(int id, int docappid)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    if (docappid == -1)
        //        row.SetSetAppAppIdNull();
        //    else
        //        row.SetAppAppId = docappid;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update IsAccepted
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="isAccepted"></param>
        ///// <returns></returns>
        //public bool UpdateIsAccepted(int id, Boolean isAccepted)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.IsAccepted = isAccepted;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update set id of all the old documents to new setid and default folder to Routed(RO)
        ///// </summary>
        ///// <param name="oldSetId"></param>
        ///// <param name="newSetId"></param>
        ///// <returns></returns>
        //public bool UpdateSetId(int oldSetId, int newSetId)
        //{
        //    SetAppDb docDb = new SetAppDb();
        //    SetApp.SetAppDataTable doc = docDb.GetSetAppByAppSetAppRefId(oldSetId);

        //    int updatedRowCount = 0;

        //    foreach (SetApp.SetAppRow dr in doc.Rows)
        //    {
        //        dr.AppSetAppRefId = newSetId;
        //        dr.SetAppFolder = "RO"; // set the default folder to routed. PK in docFolder table.

        //        int rowsAffected = Adapter.Update(doc);

        //        if (rowsAffected > 0)
        //        {
        //            updatedRowCount++;
        //            AuditTrailDb auditTrailDb = new AuditTrailDb();
        //            auditTrailDb.Record(TableNameEnum.SetApp, dr.Id.ToString(), OperationTypeEnum.Update);
        //        }
        //    }

        //    return (updatedRowCount == doc.Rows.Count);
        //}


        ///// <summary>
        ///// Update setid to new setid for all the docs send in list and default folder to Routed(RO)
        ///// </summary>
        ///// <param name="docIds"></param>
        ///// <param name="newSetId"></param>
        ///// <returns></returns>
        //public bool UpdateSetId(List<int> docIds, int newSetId)
        //{
        //    SetAppDb docDb = new SetAppDb();

        //    int updatedRowCount = 0;

        //    foreach (int docId in docIds)
        //    {
        //        SetApp.SetAppDataTable doc = docDb.GetSetAppById(docId);
        //        SetApp.SetAppRow dr = doc[0];

        //        dr.AppSetAppRefId = newSetId;
        //        dr.SetAppFolder = "RO"; // set the default folder to routed. PK in docFolder table.

        //        int rowsAffected = Adapter.Update(doc);

        //        if (rowsAffected > 0)
        //        {
        //            updatedRowCount++;
        //            AuditTrailDb auditTrailDb = new AuditTrailDb();
        //            auditTrailDb.Record(TableNameEnum.SetApp, dr.Id.ToString(), OperationTypeEnum.Update);

        //            //update logAction
        //            MembershipUser user = Membership.GetUser();
        //            if (user == null)
        //                return false;

        //            LogActionDb logActionDb = new LogActionDb();
        //            Guid userId = (Guid)user.ProviderUserKey;

        //            logActionDb.Insert(userId, LogActionEnum.Route_document, string.Empty, string.Empty, string.Empty, LogTypeEnum.D, docId);
        //        }

        //    }

        //    return (updatedRowCount == docIds.Count);
        //}


        //public bool UpdateSetAppDetails(int id, string exception, bool isAccepted, SetAppStatusEnum status)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.Exception = exception;
        //    row.IsAccepted = isAccepted;
        //    row.Status = status.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update SetAppument Condition
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="documentCondition"></param>
        ///// <returns></returns>
        //public bool UpdateSetAppumentCondition(int id, SetAppumentConditionEnum documentCondition)
        //{
        //    SetApp.SetAppDataTable doc = Adapter.GetSetAppById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    SetApp.SetAppRow row = doc[0];

        //    row.SetAppumentCondition = documentCondition.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.SetApp, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        #endregion

        #region Delete

        /// <summary>
        /// delete doc record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            return (Adapter.Delete(id) > 0);
        }
        #endregion

        #region Checking Methods

        ///// <summary>
        ///// returns true if the documents in the set are confirmed for Set Confirmation
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool IsSetAppumentsVerifiedForConfirmSet(int id)
        //{
        //    int count = (int)Adapter.IsSetAppumentsVerifiedForConfirmSet(id);
        //    return (count == 0);
        //}
        #endregion
    }
}
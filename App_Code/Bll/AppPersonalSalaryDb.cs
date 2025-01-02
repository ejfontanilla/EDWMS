using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using AppPersonalSalaryTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AppAppPersonalSalaryRefDb
    /// </summary>
    public class AppPersonalSalaryDb
    {
        private AppPersonalSalaryTableAdapter _AppPersonalSalaryAdapter = null;

        protected AppPersonalSalaryTableAdapter Adapter
        {
            get
            {
                if (_AppPersonalSalaryAdapter == null)
                    _AppPersonalSalaryAdapter = new AppPersonalSalaryTableAdapter();

                return _AppPersonalSalaryAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalary()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get AppPersonalSalary by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalaryById(int id)
        {
            return Adapter.GetAppPersonalSalaryById(id);
        }

        /// <summary>
        /// Get AppPersonalSalary with AppPersonalDetails by DocId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public DataTable GetAppPersonalSalaryWithAppPersonalDetailsByDocId(int docId)
        {
            return AppPersonalSalaryDs.GetAppPersonalSalaryWithAppPersonalDetailsByDocId(docId);
        }

        /// <summary>
        /// Get by AppPersonalId
        /// </summary>
        /// <param name="appPersonalId"></param>
        /// <returns></returns>
        public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalaryByAppPersonalId(int appPersonalId)
        {
            return Adapter.GetDataByAppPersonalId(appPersonalId);
        }

        ///// <summary>
        ///// Retrieve the documents by AppAppPersonalSalaryRefId
        ///// </summary>
        ///// <returns></returns>
        //public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalaryByAppAppPersonalSalaryRefId(int id)
        //{
        //    return Adapter.GetAppPersonalSalaryByAppAppPersonalSalaryRefId(id);
        //}

        ///// <summary>
        ///// Retrieve the documents by Id
        ///// </summary>
        ///// <returns></returns>
        //public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalaryById(int id)
        //{
        //    return Adapter.GetAppPersonalSalaryById(id);
        //}

        ///// <summary>
        ///// Get the document list of the personal for summary page
        ///// </summary>
        ///// <param name="docAppId"></param>
        ///// <param name="nric"></param>
        ///// <returns></returns>
        //public DataTable GetAppPersonalSalarySummaryByNric(int docAppId, string nric, bool isHa)
        //{
        //    return AppPersonalSalaryDs.GetAppPersonalSalaryForSummary(docAppId, nric, isHa);
        //}

        //public DataTable GetParentFolderForTreeView(int AppAppPersonalSalaryRefId, string docType)
        //{
        //    return AppPersonalSalaryDs.GetParentFolderForTreeView(AppAppPersonalSalaryRefId, docType);
        //}

        //public DataTable GetAppPersonalSalaryForTreeViewCompleteness(int docAppId)
        //{
        //    return AppPersonalSalaryDs.GetAppPersonalSalaryForTreeViewCompleteness(docAppId);
        //}

        //public DataTable GetAppPersonalSalaryForTreeView(int AppAppPersonalSalaryRefId)
        //{
        //    return AppPersonalSalaryDs.GetAppPersonalSalaryForTreeView(AppAppPersonalSalaryRefId);
        //}

        //public AppPersonalSalary.AppPersonalSalaryDataTable GetAppPersonalSalaryByAppPersonalSalaryTypeCode(string docTypeCode)
        //{
        //    return Adapter.GetDataByAppPersonalSalaryTypeCode(docTypeCode);
        //}

        //public int GetSetIdByAppPersonalSalaryId(int id)
        //{
        //    int setId = -1;

        //    AppPersonalSalary.AppPersonalSalaryDataTable dt = GetAppPersonalSalaryById(id);

        //    if (dt.Rows.Count > 0)
        //    {
        //        AppPersonalSalary.AppPersonalSalaryRow dr = dt[0];
        //        setId = dr.AppAppPersonalSalaryRefId;
        //    }

        //    return setId;
        //}
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="AppAppPersonalSalaryRefId"></param>
        /// <param name="docType"></param>
        /// <param name="originalSetId"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public int Insert(int appPersonalId, string month1Name, string month1Value, string month2Name, string month2Value,
            string month3Name, string month3Value, string month4Name, string month4Value, string month5Name, string month5Value,
            string month6Name, string month6Value, string month7Name, string month7Value, string month8Name, string month8Value,
            string month9Name, string month9Value, string month10Name, string month10Value, string month11Name, string month11Value,
            string month12Name, string month12Value)
        {
            AppPersonalSalary.AppPersonalSalaryDataTable dt = new AppPersonalSalary.AppPersonalSalaryDataTable();
            AppPersonalSalary.AppPersonalSalaryRow r = dt.NewAppPersonalSalaryRow();

            r.AppPersonalId = appPersonalId;
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

            dt.AddAppPersonalSalaryRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Update AppPersonalSalary
        /// </summary>
        /// <param name="id"></param>
        /// <param name="month1Name"></param>
        /// <param name="month1Value"></param>
        /// <param name="month2Name"></param>
        /// <param name="month2Value"></param>
        /// <param name="month3Name"></param>
        /// <param name="month3Value"></param>
        /// <param name="month4Name"></param>
        /// <param name="month4Value"></param>
        /// <param name="month5Name"></param>
        /// <param name="month5Value"></param>
        /// <param name="month6Name"></param>
        /// <param name="month6Value"></param>
        /// <param name="month7Name"></param>
        /// <param name="month7Value"></param>
        /// <param name="month8Name"></param>
        /// <param name="month8Value"></param>
        /// <param name="month9Name"></param>
        /// <param name="month9Value"></param>
        /// <param name="month10Name"></param>
        /// <param name="month10Value"></param>
        /// <param name="month11Name"></param>
        /// <param name="month11Value"></param>
        /// <param name="month12Name"></param>
        /// <param name="month12Value"></param>
        /// <returns></returns>
        public bool Update(int id, string month1Name, string month1Value, string month2Name, string month2Value,
            string month3Name, string month3Value, string month4Name, string month4Value, string month5Name, string month5Value,
            string month6Name, string month6Value, string month7Name, string month7Value, string month8Name, string month8Value,
            string month9Name, string month9Value, string month10Name, string month10Value, string month11Name, string month11Value,
            string month12Name, string month12Value)
        {
            AppPersonalSalary.AppPersonalSalaryDataTable appPersonalSalarys = GetAppPersonalSalaryById(id);

            if (appPersonalSalarys.Count == 0) return false;

            AppPersonalSalary.AppPersonalSalaryRow appPersonalSalaryRow = appPersonalSalarys[0];

            appPersonalSalaryRow.Month1Name = (String.IsNullOrEmpty(month1Name) ? " " : month1Name);
            appPersonalSalaryRow.Month2Name = (String.IsNullOrEmpty(month2Name) ? " " : month2Name);
            appPersonalSalaryRow.Month3Name = (String.IsNullOrEmpty(month3Name) ? " " : month3Name);
            appPersonalSalaryRow.Month4Name = (String.IsNullOrEmpty(month4Name) ? " " : month4Name);
            appPersonalSalaryRow.Month5Name = (String.IsNullOrEmpty(month5Name) ? " " : month5Name);
            appPersonalSalaryRow.Month6Name = (String.IsNullOrEmpty(month6Name) ? " " : month6Name);
            appPersonalSalaryRow.Month7Name = (String.IsNullOrEmpty(month7Name) ? " " : month7Name);
            appPersonalSalaryRow.Month8Name = (String.IsNullOrEmpty(month8Name) ? " " : month8Name);
            appPersonalSalaryRow.Month9Name = (String.IsNullOrEmpty(month9Name) ? " " : month9Name);
            appPersonalSalaryRow.Month10Name = (String.IsNullOrEmpty(month11Name) ? " " : month10Name);
            appPersonalSalaryRow.Month11Name = (String.IsNullOrEmpty(month11Name) ? " " : month11Name);
            appPersonalSalaryRow.Month12Name = (String.IsNullOrEmpty(month12Name) ? " " : month12Name);

            appPersonalSalaryRow.Month1Value = (String.IsNullOrEmpty(month1Value) ? " " : month1Value);
            appPersonalSalaryRow.Month2Value = (String.IsNullOrEmpty(month2Value) ? " " : month2Value);
            appPersonalSalaryRow.Month3Value = (String.IsNullOrEmpty(month3Value) ? " " : month3Value);
            appPersonalSalaryRow.Month4Value = (String.IsNullOrEmpty(month4Value) ? " " : month4Value);
            appPersonalSalaryRow.Month5Value = (String.IsNullOrEmpty(month5Value) ? " " : month5Value);
            appPersonalSalaryRow.Month6Value = (String.IsNullOrEmpty(month6Value) ? " " : month6Value);
            appPersonalSalaryRow.Month7Value = (String.IsNullOrEmpty(month7Value) ? " " : month7Value);
            appPersonalSalaryRow.Month8Value = (String.IsNullOrEmpty(month8Value) ? " " : month8Value);
            appPersonalSalaryRow.Month9Value = (String.IsNullOrEmpty(month9Value) ? " " : month9Value);
            appPersonalSalaryRow.Month10Value = (String.IsNullOrEmpty(month10Value) ? " " : month10Value);
            appPersonalSalaryRow.Month11Value = (String.IsNullOrEmpty(month11Value) ? " " : month11Value);
            appPersonalSalaryRow.Month12Value = (String.IsNullOrEmpty(month12Value) ? " " : month12Value);

            int affected = Adapter.Update(appPersonalSalarys);
            return affected > 0;
        }

        public bool Update(int id, int appPersonalId)
        {
            AppPersonalSalary.AppPersonalSalaryDataTable appPersonalSalarys = GetAppPersonalSalaryById(id);

            if (appPersonalSalarys.Count == 0) return false;

            AppPersonalSalary.AppPersonalSalaryRow appPersonalSalaryRow = appPersonalSalarys[0];

            appPersonalSalaryRow.AppPersonalId = appPersonalId;

            int affected = Adapter.Update(appPersonalSalarys);
            return affected > 0;
        }


        ///// <summary>
        ///// Update AppPersonalSalary Status
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="status"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalaryFromMeta(int id, AppPersonalSalaryStatusEnum status, string imageCondition, string docTypeCode, Boolean isConfirmed, LogTypeEnum logType)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.Status = status.ToString();
        //    row.ImageCondition = imageCondition;
        //    row.AppPersonalSalaryTypeCode = docTypeCode;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);

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
        ///// Update AppPersonalSalaryTypeCode
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docTypeCode"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalarytype(int id, string docTypeCode)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.AppPersonalSalaryTypeCode = docTypeCode;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update AppPersonalSalaryFolder
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docFolder"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalaryFolder(int id, string docFolder)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    if (string.IsNullOrEmpty(docFolder))
        //        row.SetAppPersonalSalaryFolderNull();
        //    else
        //        row.AppPersonalSalaryFolder = docFolder;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update AppPersonalSalaryument Status 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docStatus"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalaryStatus(int id, AppPersonalSalaryStatusEnum docStatus)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.Status = docStatus.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}


        ///// <summary>
        ///// Update AppPersonalSalaryAppId
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="docappid"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalaryAppId(int id, int docappid)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    if (docappid == -1)
        //        row.SetAppPersonalSalaryAppIdNull();
        //    else
        //        row.AppPersonalSalaryAppId = docappid;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
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
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.IsAccepted = isAccepted;

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
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
        //    AppPersonalSalaryDb docDb = new AppPersonalSalaryDb();
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = docDb.GetAppPersonalSalaryByAppAppPersonalSalaryRefId(oldSetId);

        //    int updatedRowCount = 0;

        //    foreach (AppPersonalSalary.AppPersonalSalaryRow dr in doc.Rows)
        //    {
        //        dr.AppAppPersonalSalaryRefId = newSetId;
        //        dr.AppPersonalSalaryFolder = "RO"; // set the default folder to routed. PK in docFolder table.

        //        int rowsAffected = Adapter.Update(doc);

        //        if (rowsAffected > 0)
        //        {
        //            updatedRowCount++;
        //            AuditTrailDb auditTrailDb = new AuditTrailDb();
        //            auditTrailDb.Record(TableNameEnum.AppPersonalSalary, dr.Id.ToString(), OperationTypeEnum.Update);
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
        //    AppPersonalSalaryDb docDb = new AppPersonalSalaryDb();

        //    int updatedRowCount = 0;

        //    foreach (int docId in docIds)
        //    {
        //        AppPersonalSalary.AppPersonalSalaryDataTable doc = docDb.GetAppPersonalSalaryById(docId);
        //        AppPersonalSalary.AppPersonalSalaryRow dr = doc[0];

        //        dr.AppAppPersonalSalaryRefId = newSetId;
        //        dr.AppPersonalSalaryFolder = "RO"; // set the default folder to routed. PK in docFolder table.

        //        int rowsAffected = Adapter.Update(doc);

        //        if (rowsAffected > 0)
        //        {
        //            updatedRowCount++;
        //            AuditTrailDb auditTrailDb = new AuditTrailDb();
        //            auditTrailDb.Record(TableNameEnum.AppPersonalSalary, dr.Id.ToString(), OperationTypeEnum.Update);

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


        //public bool UpdateAppPersonalSalaryDetails(int id, string exception, bool isAccepted, AppPersonalSalaryStatusEnum status)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.Exception = exception;
        //    row.IsAccepted = isAccepted;
        //    row.Status = status.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        ///// <summary>
        ///// Update AppPersonalSalaryument Condition
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="documentCondition"></param>
        ///// <returns></returns>
        //public bool UpdateAppPersonalSalaryumentCondition(int id, AppPersonalSalaryumentConditionEnum documentCondition)
        //{
        //    AppPersonalSalary.AppPersonalSalaryDataTable doc = Adapter.GetAppPersonalSalaryById(id);

        //    if (doc.Count == 0)
        //        return false;

        //    AppPersonalSalary.AppPersonalSalaryRow row = doc[0];

        //    row.AppPersonalSalaryumentCondition = documentCondition.ToString();

        //    int rowsAffected = Adapter.Update(doc);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Update);
        //    }
        //    return rowsAffected == 1;
        //}

        #endregion

        #region Delete

        ///// <summary>
        ///// Delete doc record.
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool Delete(int id)
        //{
        //    AuditTrailDb auditTrailDb = new AuditTrailDb();
        //    int rowsAffected = 0;
        //    Guid? operationId = auditTrailDb.Record(TableNameEnum.AppPersonalSalary, id.ToString(), OperationTypeEnum.Delete);

        //    rowsAffected = Adapter.Delete(id);

        //    if (rowsAffected > 0)
        //    {
        //        auditTrailDb.Delete(operationId);
        //    }

        //    return (rowsAffected > 0);
        //}
        #endregion

        #region Checking Methods

        ///// <summary>
        ///// returns true if the documents in the set are confirmed for Set Confirmation
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool IsAppPersonalSalaryumentsVerifiedForConfirmSet(int id)
        //{
        //    int count = (int)Adapter.IsAppPersonalSalaryumentsVerifiedForConfirmSet(id);
        //    return (count == 0);
        //}
        #endregion
    }
}
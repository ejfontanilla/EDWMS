using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.Security;
using DocTableAdapters;
using System.Data;
using System.IO;
using Ionic.Zip;
using Dwms.Dal;
using Dwms.Web;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class DocDb
    {
        private DocTableAdapter _DocAdapter = null;

        protected DocTableAdapter Adapter
        {
            get
            {
                if (_DocAdapter == null)
                    _DocAdapter = new DocTableAdapter();

                return _DocAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public Doc.DocDataTable GetDoc()
        {
            return Adapter.GetDoc();
        }

        /// <summary>
        /// Retrieve the documents by docSetId
        /// </summary>
        /// <returns></returns>
        public Doc.DocDataTable GetDocByDocSetId(int id)
        {
            return Adapter.GetDocByDocSetId(id);
        }

        public Doc.DocDataTable GetDocByDocSetIdOrderByDocTypeCode(int id)
        {
            return Adapter.GetDocByDocSetIdOrderByDocTypeCode(id);
        }

        /// <summary>
        /// Retrieve the documents by Id
        /// </summary>
        /// <returns></returns>
        public Doc.DocDataTable GetDocById(int id)
        {
            return Adapter.GetDocById(id);
        }

        /// <summary>
        /// Get the document list of the personal for summary page
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetDocSummaryByNric(int docAppId, string nric, bool isHa, string referenceType)
        {
            return DocDs.GetDocForSummary(docAppId, nric, isHa, referenceType);
        }

        public DataTable GetParentFolderForTreeView(int docSetId)
        {
            return DocDs.GetParentFolderForTreeView(docSetId);
        }

        public DataTable GetDocForTreeViewCompleteness(int docAppId)
        {
            return DocDs.GetDocForTreeViewCompleteness(docAppId);
        }

        public DataTable GetDocForTreeView(int docSetId)
        {
            return DocDs.GetDocForTreeView(docSetId);
        }

        public DataTable GetDocByNric(string nric, DateTime? dateInForm, DateTime? dateInTo)
        {
            return DocDs.GetDocByNric(nric, dateInForm, dateInTo);
        }

        /// <summary>
        /// Get distinct CmDocumentId from docSet for dropdown display
        /// </summary>
        /// <returns></returns>
        public DataTable GetCmDocumentIdForDropDown()
        {
            return DocDs.GetCmDocumentIdForDropDown();
        }


        public Doc.DocDataTable GetDocByDocTypeCode(string docTypeCode)
        {
            return Adapter.GetDataByDocTypeCode(docTypeCode);
        }

        /// <summary>
        /// Get verified document Count for completeness confirmation. this will only check for documents whos docsets are verified.
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public int GetCountNotVerifiedForAppConfirmation(int docAppId)
        {
            int? result = Adapter.GetCountNotVerifiedForAppConfirmation(docAppId);
            return (result.HasValue ? result.Value : 0);
        }
        

        /// <summary>
        /// Get SetId by docId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetSetIdByDocId(int id)
        {
            Doc.DocDataTable docs = Adapter.GetDocById(id);

            if (docs.Rows.Count > 0)
            {
                Doc.DocRow docRow = docs[0];
                return docRow.DocSetId;
            }
            else
                return -1;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="docType"></param>
        /// <param name="originalSetId"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public int Insert(string docType, int originalSetId, int docSetId, DocStatusEnum? status, string docFolder, 
            string imageCondition, DocumentConditionEnum?  docCondition)
        {
            Doc.DocDataTable dt = new Doc.DocDataTable();
            Doc.DocRow r = dt.NewDocRow();

            r.DocTypeCode = docType;
            r.OriginalSetId = originalSetId;
            r.DocSetId = docSetId;
            r.Status = status.ToString();
            r.ImageCondition = imageCondition;
            r.DocumentCondition = docCondition.ToString();
            r.SendToCDBStatus = SendToCDBStatusEnum.Ready.ToString();
            dt.AddDocRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        /// <summary>
        /// Insert method (new By Andrew) include the docChannel
        /// </summary>
        /// <param name="docType"></param>
        /// <param name="originalSetId"></param>
        /// <param name="docSetId"></param>
        /// <param name="status"></param>
        /// <param name="docFolder"></param>
        /// <param name="imageCondition"></param>
        /// <param name="docCondition"></param>
        /// <returns></returns>
        public int Insert(string docType, int originalSetId, int docSetId, DocStatusEnum? status, string docFolder,
            string imageCondition, DocumentConditionEnum? docCondition, bool IsVerified, string docChannel,
            string CmDocumentId, string OriginalCmDocumentId, string Description, string CustomerIdSubFromSource)
        {
            Doc.DocDataTable dt = new Doc.DocDataTable();
            Doc.DocRow r = dt.NewDocRow();

            r.DocTypeCode = docType;
            r.OriginalSetId = originalSetId;
            r.DocSetId = docSetId;
            r.Status = status.ToString();
            r.ImageCondition = imageCondition;
            r.DocumentCondition = docCondition.ToString();
            r.SendToCDBStatus = SendToCDBStatusEnum.Ready.ToString();
            r.IsVerified = IsVerified;
            r.DocChannel = docChannel;
            r.CmDocumentId = CmDocumentId;
            r.OriginalCmDocumentId = OriginalCmDocumentId;
            r.Description = Description;
            r.CustomerIdSubFromSource = CustomerIdSubFromSource;
            dt.AddDocRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Update Doc Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateDocFromMeta(int id, DocStatusEnum status, string imageCondition, string docTypeCode, Boolean isConfirmed, LogTypeEnum logType)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.Status = status.ToString();
            row.ImageCondition = imageCondition;
            row.DocTypeCode = docTypeCode;
            if (status == DocStatusEnum.Verified && isConfirmed)
                row.SendToCDBStatus = SendToCDBStatusEnum.Ready.ToString();
            if (status == DocStatusEnum.Completed && isConfirmed)
                row.SendToCDBAccept = SendToCDBStatusEnum.Ready.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);

                //Insert LogAction
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                LogActionDb logActionDb = new LogActionDb();
                Guid userId = (Guid)user.ProviderUserKey;

                if (isConfirmed)
                    logActionDb.Insert(userId, LogActionEnum.Confirmed_metadata, string.Empty, string.Empty, string.Empty, string.Empty, logType, id);
                else
                    logActionDb.Insert(userId, LogActionEnum.Saved_metadata_as_draft, string.Empty, string.Empty, string.Empty, string.Empty, logType, id);
            }
            return rowsAffected == 1;
        }


        /// <summary>
        /// Update DocTypeCode
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public bool UpdateDoctype(int id, string docTypeCode)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.DocTypeCode = docTypeCode;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update DocFolder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docFolder"></param>
        /// <returns></returns>
        public bool UpdateDocFolder(int id, string docFolder)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            //if (string.IsNullOrEmpty(docFolder))
            //    row.SetDocFolderNull();
            //else
            //    row.DocFolder = docFolder;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update Document Status 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docStatus"></param>
        /// <returns></returns>
        public bool UpdateDocStatus(int id, DocStatusEnum docStatus)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.Status = docStatus.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update Document Status 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docStatus"></param>
        /// <returns></returns>
        public bool UpdateCmDocId(int id, string cmDocID)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.CmDocumentId = cmDocID;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update DocAppId
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docappid"></param>
        /// <returns></returns>
        public bool UpdateDocAppId(int id, int docappid)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            //if (docappid == -1)
            //    row.SetDocAppIdNull();
            //else
            //    row.DocAppId = docappid;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update IsAccepted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isAccepted"></param>
        /// <returns></returns>
        public bool UpdateIsAccepted(int id, Boolean isAccepted)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.IsAccepted = isAccepted;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }
        //Added by Edward 04.11.2013 Confirm All Acceptance
        /// <summary>
        /// Updates the ImageAccepted.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isAccepted"></param>
        /// <returns></returns>
        public bool UpdateImageAccepted(int id, string isAccepted)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.ImageAccepted = isAccepted;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }


        /// <summary>
        /// Update IsVerified Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isVerified"></param>
        /// <returns></returns>
        public bool UpdateIsVerified(int id, Boolean isVerified)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.IsVerified = isVerified;

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update set id of all the old documents to new setid and default folder to Routed(RO)
        /// </summary>
        /// <param name="oldSetId"></param>
        /// <param name="newSetId"></param>
        /// <returns></returns>
        public bool UpdateSetId(int oldSetId, int newSetId)
        {
            DocDb docDb = new DocDb();
            Doc.DocDataTable doc = docDb.GetDocByDocSetId(oldSetId);

            int updatedRowCount = 0;

            foreach (Doc.DocRow dr in doc.Rows)
            {
                //dr.DocSetId = newSetId;
                //dr.DocFolder = "RO"; // set the default folder to routed. PK in docFolder table.

                int rowsAffected = Adapter.Update(doc);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);
                }
            }

            return (updatedRowCount == doc.Rows.Count);
        }


        /// <summary>
        /// Update setid to new setid for all the docs send in list and default folder to Routed(RO)
        /// </summary>
        /// <param name="docIds"></param>
        /// <param name="newSetId"></param>
        /// <returns></returns>
        public bool UpdateSetId(List<int> docIds, int newSetId)
        {
            DocDb docDb = new DocDb();

            int updatedRowCount = 0;

            foreach (int docId in docIds)
            {
                Doc.DocDataTable doc = docDb.GetDocById(docId);
                Doc.DocRow dr = doc[0];

                //dr.DocSetId = newSetId;
                //dr.DocFolder = "RO"; // set the default folder to routed. PK in docFolder table.

                int rowsAffected = Adapter.Update(doc);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);

                    //update logAction
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                        return false;

                    LogActionDb logActionDb = new LogActionDb();
                    Guid userId = (Guid)user.ProviderUserKey;

                    logActionDb.Insert(userId, LogActionEnum.Route_document, string.Empty, string.Empty, string.Empty, string.Empty, LogTypeEnum.D, docId);
                }

            }

            return (updatedRowCount == docIds.Count);
        }

        /// <summary>
        /// Update doc SendToCdbStatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbStatus(int id, SendToCDBStatusEnum status)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.SendToCDBStatus = status.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update doc SendToCdbStatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbAccept(int id, SendToCDBStatusEnum accept)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.SendToCDBAccept = accept.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// UpdateSendToCdbStatusUponDocModifiedUnderCompleteness
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(int id)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            if (row.SendToCDBStatus.ToString().ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                row.SendToCDBStatus = SendToCDBStatusEnum.ModifiedInCompleteness.ToString();
            else
                row.SendToCDBStatus = SendToCDBStatusEnum.Ready.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        public bool UpdateDocDetails(int id, string exception, string imageAccepted, DocStatusEnum status)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.Exception = exception;
            row.ImageAccepted = imageAccepted;
            row.Status = status.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Update Document Condition
        /// </summary>
        /// <param name="id"></param>
        /// <param name="documentCondition"></param>
        /// <returns></returns>
        public bool UpdateDocumentCondition(int id, DocumentConditionEnum documentCondition)
        {
            Doc.DocDataTable doc = Adapter.GetDocById(id);

            if (doc.Count == 0)
                return false;

            Doc.DocRow row = doc[0];

            row.DocumentCondition = documentCondition.ToString();

            int rowsAffected = Adapter.Update(doc);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        /// <summary>
        /// Set the converted to sample flag
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="isConverted"></param>
        /// <returns></returns>
        public bool SetConvertedToSampleDocFlag(int docSetId, bool isConverted)
        {
            DocDb docDb = new DocDb();
            Doc.DocDataTable doc = docDb.GetDocByDocSetId(docSetId);

            int updatedRowCount = 0;

            foreach (Doc.DocRow dr in doc.Rows)
            {
                dr.ConvertedToSampleDoc = isConverted;

                int rowsAffected = Adapter.Update(doc);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);
                }
            }

            return (updatedRowCount == doc.Rows.Count);
        }
        #endregion

        #region Delete

        /// <summary>
        /// Delete doc record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;
            Guid? operationId = auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.Doc, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }
        #endregion

        #region Checking Methods

        /// <summary>
        /// returns true if the documents in the set are confirmed for Set Confirmation
        /// </summary>
        /// <param name="docSteId"></param>
        /// <returns></returns>
        public bool IsDocumentsVerifiedForConfirmSet(int docSteId)
        {
            int count = (int)Adapter.IsDocumentsVerifiedForConfirmSet(docSteId);
            return (count == 0);
        }

        /// <summary>
        /// checks if there are any unidentified documents attached to  appDocRef based on DocSetId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasUnidentifiedAppDocByDocSetId(int id)
        {
            int count = (int)Adapter.GetUnidentifiedAppDocCountByDocSetId(id);
            return (count > 0);
        }

        /// <summary>
        /// Get Document IsVerified status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool GetIsVerifiedByDocId(int id)
        {
            int count = (int)Adapter.GetCountIsVerifiedByDocId(id);
            return (count > 0);
        }

        #endregion

        #region Support Methods

        public void SplitDocumentForVerification(int docId, List<int> pageIds, SplitTypeEnum splitType)
        {
            RawPageDb rawPageDb = new RawPageDb();

            if (rawPageDb.IsAllRawPagesBelongToDocument(pageIds, docId))
            {
                if (splitType.Equals(SplitTypeEnum.Group))
                {
                    SplitDocumentForVerification(docId, pageIds, -1);
                }
                else
                {
                    RawPage.RawPageDataTable rawPages = new RawPage.RawPageDataTable();

                    //this is to preserve the page number for log.
                    Dictionary<int, int> docPageNoDictionary = new Dictionary<int, int>();
                    foreach (int pageId in pageIds)
                    {
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(pageId);
                        RawPage.RawPageRow rawPageRow = rawPages[0];
                        docPageNoDictionary.Add(pageId, rawPageRow.DocPageNo);
                    }

                    foreach (int pageId in pageIds)
                    {
                        List<int> newPageIdList = new List<int>();
                        newPageIdList.Add(pageId);
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(pageId);
                        RawPage.RawPageRow rawPageRow = rawPages[0];

                        SplitDocumentForVerification(docId, newPageIdList, docPageNoDictionary[pageId]);
                    }
                }
            }
        }

        public void SplitDocumentForCompleteness(int docId, List<int> pageIds, SplitTypeEnum splitType)
        {
            RawPageDb rawPageDb = new RawPageDb();

            if (rawPageDb.IsAllRawPagesBelongToDocument(pageIds, docId))
            {
                if (splitType.Equals(SplitTypeEnum.Group))
                {
                    SplitDocumentForCompleteness(docId, pageIds, -1);
                }
                else
                {
                    RawPage.RawPageDataTable rawPages = new RawPage.RawPageDataTable();

                    //this is to preserve the page number for log.
                    Dictionary<int, int> docPageNoDictionary = new Dictionary<int, int>();
                    foreach (int pageId in pageIds)
                    {
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(pageId);
                        RawPage.RawPageRow rawPageRow = rawPages[0];
                        docPageNoDictionary.Add(pageId, rawPageRow.DocPageNo);
                    }

                    foreach (int pageId in pageIds)
                    {
                        List<int> newPageIdList = new List<int>();
                        newPageIdList.Add(pageId);
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(pageId);
                        RawPage.RawPageRow rawPageRow = rawPages[0];

                        SplitDocumentForCompleteness(docId, newPageIdList, docPageNoDictionary[pageId]);
                    }
                }
            }
        }

        protected void SplitDocumentForVerification(int docId, List<int> pageIds, int rawPageNo)
        {
            if (pageIds.Count == 0)
                return;

            RawPageDb rawPageDb = new RawPageDb();
            DocDb docDb = new DocDb();

            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();

            // Document id
            int pageCount = rawPageDb.CountRawPageByDocId(docId);

            // Get the Doc data                
            UpdateCmDocId(docId, null);
            UpdateSendToCdbStatus(docId, SendToCDBStatusEnum.Ready);
            UpdateSendToCdbAccept(docId, SendToCDBStatusEnum.NotReady);
            Doc.DocDataTable docs = docDb.GetDocById(docId);

            if (docs.Rows.Count > 0 && pageCount > 1)
            {
                Doc.DocRow docRow = docs[0];
                //docRow.CmDocumentId = null;

                //string Description;
                //if (docRow.Description == null)
                //    Description = "";
                //else
                //    Description = docRow.Description;
                // Create a copy of the current Doc data
                DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
                DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
                int docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum, "",
                    docRow.ImageCondition, documentConditionEnum, false, docRow.DocChannel,
                    docRow.IsCmDocumentIdNull() ? null : docRow.CmDocumentId, docRow.IsOriginalCmDocumentIdNull() ? null : docRow.OriginalCmDocumentId,
                    docRow.IsDescriptionNull() ? null : docRow.Description, docRow.IsCustomerIdSubFromSourceNull() ? null : docRow.CustomerIdSubFromSource);
        //public int Insert(string docType, int originalSetId, int docSetId, DocStatusEnum? status, string docFolder,
        //    string imageCondition, DocumentConditionEnum? docCondition, bool IsVerified, string docChannel,
        //    string CmDocumentId, string OriginalCmDocumentId, string Description, string CustomerIdSubFromSource)

                if (docCopyId > 0)
                {
                    int pageNo = 1;
                    string selectedPageNumbers = "Page" + ((pageIds.Count > 1) ? "s " : " ");

                    RawPage.RawPageDataTable rawPages = new RawPage.RawPageDataTable();

                    foreach (int rawPageId in pageIds)
                    {
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(rawPageId);

                        if (rawPages.Rows.Count > 0)
                        {
                            RawPage.RawPageRow rawPageRow = rawPages[0];

                            // Update the RawPage to be assigned to the new Doc record
                            rawPageDb.Update(rawPageRow.Id, docCopyId, pageNo);

                            if (pageNo == 1)
                                selectedPageNumbers += rawPageRow.DocPageNo;
                            else
                                selectedPageNumbers += "," + rawPageRow.DocPageNo;

                            pageNo++;
                        }
                    }

                    //this is to preserver the pageno when the split is of "individual" type
                    // for split type "group" rawPageNo paramter is set to -1
                    if (rawPageNo != -1)
                        selectedPageNumbers = "Page " + rawPageNo;

                    //add log
                    LogActionDb logActionDb = new LogActionDb();
                    DocTypeDb docTypeDb = new DocTypeDb();
                    DocType.DocTypeDataTable docTypes = docTypeDb.GetDocType(docRow.DocTypeCode);
                    string oldDocTypeName = string.Empty;

                    if (docTypes.Rows.Count > 0)
                    {
                        DocType.DocTypeRow docTypeRow = docTypes[0];
                        oldDocTypeName = docTypeRow.Description;
                    }

                    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_extracted_as_new_document_REPLACE2_from_REPLACE3, selectedPageNumbers, oldDocTypeName + Dwms.Web.TreeviewDWMS.FormatId(docCopyId.ToString()), oldDocTypeName + Dwms.Web.TreeviewDWMS.FormatId(docId.ToString()), string.Empty, LogTypeEnum.D, docId);

                    //copy the docpersonal/apppersonal records
                    setDocRefDb.CopyFromSource(docId, docCopyId);
                    appDocRefDb.CopyFromSource(docId, docCopyId);

                    // Rearrange the page numbers of the raw page
                    rawPages = rawPageDb.GetRawPageByDocId(docId);

                    int pageNo2 = 1;
                    foreach (RawPage.RawPageRow rawPage in rawPages.Rows)
                    {
                        rawPageDb.Update(rawPage.Id, rawPage.DocId, pageNo2);

                        pageNo2++;
                    }

                    // Delete the current doc record if it contains only 1 page
                    if (rawPages.Count <= 0)
                    {
                        docDb.Delete(docId);
                    }

                    //update doc status
                    docDb.UpdateDocStatus(docId, DocStatusEnum.Pending_Verification);
                    docDb.UpdateDocStatus(docCopyId, DocStatusEnum.Pending_Verification);

                    //update isVerifiedStatus
                    docDb.UpdateIsVerified(docId, false);
                }
            }
        }

        protected void SplitDocumentForCompleteness(int docId, List<int> pageIds, int rawPageNo)
        {
            if (pageIds.Count == 0)
                return;

            RawPageDb rawPageDb = new RawPageDb();
            DocDb docDb = new DocDb();

            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();
            DocSetDb docSetDb = new DocSetDb();

            // Document id
            int pageCount = rawPageDb.CountRawPageByDocId(docId);

            // Get the Doc data                
            UpdateCmDocId(docId, null);
            UpdateSendToCdbStatus(docId, SendToCDBStatusEnum.Ready);
            UpdateSendToCdbAccept(docId, SendToCDBStatusEnum.NotReady);
            Doc.DocDataTable docs = docDb.GetDocById(docId);

            if (docs.Rows.Count > 0 && pageCount > 1)
            {
                Doc.DocRow docRow = docs[0];
                //docRow.CmDocumentId = null;

                // Create a copy of the current Doc data
                DocStatusEnum docStatusEnum = (DocStatusEnum)Enum.Parse(typeof(DocStatusEnum), docRow.Status, true);
                DocumentConditionEnum documentConditionEnum = (DocumentConditionEnum)Enum.Parse(typeof(DocumentConditionEnum), docRow.DocumentCondition, true);
                int docCopyId = docDb.Insert(docRow.DocTypeCode, docRow.OriginalSetId, docRow.DocSetId, docStatusEnum, "",
                    docRow.ImageCondition, documentConditionEnum, false, docRow.DocChannel,
                    docRow.IsCmDocumentIdNull() ? null : docRow.CmDocumentId, docRow.IsOriginalCmDocumentIdNull() ? null : docRow.OriginalCmDocumentId,
                    docRow.IsDescriptionNull() ? null : docRow.Description, docRow.IsCustomerIdSubFromSourceNull() ? null : docRow.CustomerIdSubFromSource);
                //public int Insert(string docType, int originalSetId, int docSetId, DocStatusEnum? status, string docFolder,
                //    string imageCondition, DocumentConditionEnum? docCondition, bool IsVerified, string docChannel,
                //    string CmDocumentId, string OriginalCmDocumentId, string Description, string CustomerIdSubFromSource)
                
                if (docCopyId > 0)
                {
                    int pageNo = 1;
                    string selectedPageNumbers = "Page" + ((pageIds.Count > 1) ? "s " : " ");

                    RawPage.RawPageDataTable rawPages = new RawPage.RawPageDataTable();

                    foreach (int rawPageId in pageIds)
                    {
                        // Get the RawPage data                        
                        rawPages = rawPageDb.GetRawPageById(rawPageId);

                        if (rawPages.Rows.Count > 0)
                        {
                            RawPage.RawPageRow rawPageRow = rawPages[0];

                            // Update the RawPage to be assigned to the new Doc record
                            rawPageDb.Update(rawPageRow.Id, docCopyId, pageNo);

                            if (pageNo == 1)
                                selectedPageNumbers += rawPageRow.DocPageNo;
                            else
                                selectedPageNumbers += "," + rawPageRow.DocPageNo;

                            pageNo++;
                        }
                    }

                    //this is to preserver the pageno when the split is of "individual" type
                    // for split type "group" rawPageNo paramter is set to -1
                    if (rawPageNo != -1)
                        selectedPageNumbers = "Page " + rawPageNo;

                    //add log
                    LogActionDb logActionDb = new LogActionDb();
                    DocTypeDb docTypeDb = new DocTypeDb();
                    DocType.DocTypeDataTable docTypes = docTypeDb.GetDocType(docRow.DocTypeCode);
                    string oldDocTypeName = string.Empty;

                    if (docTypes.Rows.Count > 0)
                    {
                        DocType.DocTypeRow docTypeRow = docTypes[0];
                        oldDocTypeName = docTypeRow.Description;
                    }

                    logActionDb.Insert((Guid)Membership.GetUser().ProviderUserKey, LogActionEnum.REPLACE1_extracted_as_new_document_REPLACE2_from_REPLACE3, selectedPageNumbers, oldDocTypeName + Dwms.Web.TreeviewDWMS.FormatId(docCopyId.ToString()), oldDocTypeName + Dwms.Web.TreeviewDWMS.FormatId(docId.ToString()), string.Empty, LogTypeEnum.C, docId);

                    //copy the docpersonal/apppersonal records
                    setDocRefDb.CopyFromSource(docId, docCopyId);
                    appDocRefDb.CopyFromSource(docId, docCopyId);

                    // Rearrange the page numbers of the raw page
                    rawPages = rawPageDb.GetRawPageByDocId(docId);

                    int pageNo2 = 1;
                    foreach (RawPage.RawPageRow rawPage in rawPages.Rows)
                    {
                        rawPageDb.Update(rawPage.Id, rawPage.DocId, pageNo2);

                        pageNo2++;
                    }

                    // Delete the current doc record if it contains only 1 page
                    if (rawPages.Count <= 0)
                    {
                        docDb.Delete(docId);
                    }

                    //update doc status
                    docDb.UpdateDocStatus(docId, DocStatusEnum.Verified);
                    docDb.UpdateDocStatus(docCopyId, DocStatusEnum.Verified);

                    //update isVerifiedStatus
                    docDb.UpdateIsVerified(docId, false);

                    //update isAcceptedStatus
                    docDb.UpdateIsAccepted(docId, false);
                    docDb.UpdateIsAccepted(docCopyId, false);

                    //reset SendToCdb Flags
                    int setId = docDb.GetSetIdByDocId(docId);
                    docDb.UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(docId);
                    docSetDb.UpdateSendToCdbStatus(setId, SendToCDBStatusEnum.Ready);
                }
            }
        }

        /// <summary>
        /// Get Document file size
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        //public long GetDocumentFileSize(int docId)
        //{
        //    ArrayList pageList = new ArrayList();
        //    long fileSize = 0;

        //    // RawPage Folder
        //    string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
        //    DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);

        //    RawPageDb rawPageDb = new RawPageDb();
        //    RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

        //    for (int cnt = 0; cnt < rawPages.Count; cnt++)
        //    {
        //        RawPage.RawPageRow rawPage = rawPages[cnt];

        //        DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

        //        if (rawPageDirs.Length > 0)
        //        {
        //            DirectoryInfo rawPageDir = rawPageDirs[0];

        //            // Get the raw page for download
        //            FileInfo[] rawPageFiles = rawPageDir.GetFiles();

        //            bool hasRawPage = false;
        //            foreach (FileInfo rawPageFile in rawPageFiles)
        //            {
        //                if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
        //                    !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
        //                    !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
        //                {
        //                    if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
        //                    {
        //                        hasRawPage = true;
        //                        fileSize += rawPageFile.Length;
        //                    }
        //                }
        //            }

        //            // If the raw page is not found, use the searcheable PDF
        //            if (!hasRawPage)
        //            {
        //                FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

        //                foreach (FileInfo file in rawPagePdfFiles)
        //                {
        //                    fileSize += file.Length;
        //                }
        //            }
        //        }
        //    }

        //    return fileSize;
        //}

        public long GetDocumentFileSize(int docId)
        {
            ArrayList pageList = new ArrayList();
            long fileSize = 0;

            // RawPage Folder
            RawPageDb rawPageDb = new RawPageDb();
            RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

            for (int cnt = 0; cnt < rawPages.Count; cnt++)
            {
                RawPage.RawPageRow rawPage = rawPages[cnt];                

                DirectoryInfo rawPageDirs = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);

                if (rawPageDirs.Exists)
                {
                    // Get the raw page for download
                    FileInfo[] rawPageFiles = rawPageDirs.GetFiles();

                    bool hasRawPage = false;
                    foreach (FileInfo rawPageFile in rawPageFiles)
                    {
                        if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                            !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                            !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                        {
                            if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                            {
                                hasRawPage = true;
                                fileSize += rawPageFile.Length;
                            }
                        }
                    }

                    // If the raw page is not found, use the searcheable PDF
                    if (!hasRawPage)
                    {
                        FileInfo[] rawPagePdfFiles = rawPageDirs.GetFiles("*_s.pdf");

                        foreach (FileInfo file in rawPagePdfFiles)
                        {
                            fileSize += file.Length;
                        }
                    }
                }
            }

            return fileSize;
        }

        #endregion

        /// <summary>
        /// Get totalsize by document ids
        /// </summary>
        /// <param name="docIds"></param>
        /// <returns></returns>
        public long GetDocumentFileSizeByIds(string docIds)
        {
            long totalSize = 0;
            string[] ids = docIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string id in ids)
            {
                totalSize += GetDocumentFileSize(int.Parse(id));
            }
            return totalSize;
        }

        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        //public long GetDocumentZippedFileSizeByIds(string docIds, int docAppId)
        //{
        //    long totalSize = 0;
        //    ArrayList docList = new ArrayList();
        //    string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");

        //    string[] ids = docIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string id in ids)
        //    {
        //        DocDb docDb = new DocDb();
        //        RawPageDb rawPageDb = new RawPageDb();
        //        DocTypeDb docTypeDb = new DocTypeDb();

        //        int docId = int.Parse(id);


        //        Doc.DocDataTable docTable = docDb.GetDocById(docId);

        //        // RawPage Folder
        //        string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
        //        DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);

        //        if (docTable.Rows.Count > 0)
        //        {
        //            Doc.DocRow doc = docTable[0];

        //            ArrayList pageList = new ArrayList();

        //            RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

        //            for (int cnt = 0; cnt < rawPages.Count; cnt++)
        //            {

        //                RawPage.RawPageRow rawPage = rawPages[cnt];

        //                DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

        //                if (rawPageDirs.Length > 0)
        //                {
        //                    DirectoryInfo rawPageDir = rawPageDirs[0];

        //                    // Get the raw page for download
        //                    FileInfo[] rawPageFiles = rawPageDir.GetFiles();

        //                    bool hasRawPage = false;
        //                    foreach (FileInfo rawPageFile in rawPageFiles)
        //                    {
        //                        if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
        //                            !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
        //                            !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
        //                        {
        //                            if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
        //                            {
        //                                //path = Util.CreatePdfFileFromImage(path);
        //                                pageList.Add(rawPageFile.FullName);
        //                                hasRawPage = true;
        //                            }
        //                        }
        //                    }

        //                    // If the raw page is not found, use the searcheable PDF
        //                    if (!hasRawPage)
        //                    {
        //                        FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

        //                        if (rawPagePdfFiles.Length > 0)
        //                            pageList.Add(rawPagePdfFiles[0].FullName);
        //                    }
        //                }
        //            }

        //            if (!Directory.Exists(saveDir))
        //                Directory.CreateDirectory(saveDir);

        //            if (pageList.Count > 0)
        //            {
        //                string docTypeDesc = doc.DocTypeCode;

        //                DocType.DocTypeDataTable docTypeTable = docTypeDb.GetDocType(doc.DocTypeCode);

        //                if (docTypeTable.Rows.Count > 0)
        //                {
        //                    DocType.DocTypeRow docType = docTypeTable[0];
        //                    docTypeDesc = docType.Description;
        //                }

        //                string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + doc.Id.ToString() + ".pdf");

        //                try
        //                {
        //                    if (File.Exists(mergedFileName))
        //                        File.Delete(mergedFileName);
        //                }
        //                catch (Exception)
        //                {
        //                }

        //                string errorMessage = string.Empty;

        //                Util.MergePdfFiles(pageList, mergedFileName, out errorMessage);

        //                docList.Add(mergedFileName);
        //            }
        //        }
                
        //    }

        //    if (docList.Count > 0)
        //    {
        //        using (ZipFile zip = new ZipFile())
        //        {

        //            DocAppDb docAppDb = new DocAppDb();
        //            DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(docAppId);
        //            DocApp.DocAppRow docAppRow = docApp[0];

        //            string zipFileName = Path.Combine(saveDir, docAppRow.RefNo + "_" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt) + ".zip");

        //            foreach (string docPath in docList)
        //            {
        //                FileInfo fi = new FileInfo(docPath);

        //                try
        //                {
        //                    using (FileStream fileStream = fi.Open(FileMode.Open, FileAccess.Read))
        //                    {
        //                        BinaryReader binaryReader = new BinaryReader(fileStream);
        //                        byte[] fileBytes = binaryReader.ReadBytes((Int32)fi.Length);

        //                        string rawFileName = fi.Name;

        //                        zip.AddEntry(rawFileName, fileBytes);
        //                    }
        //                }
        //                catch (Exception)
        //                {
        //                }
        //            }
        //            zip.Save(zipFileName);

        //            FileInfo zipFileInfo = new FileInfo(zipFileName);
        //            totalSize = zipFileInfo.Length;
        //        }
        //    }
        //    return totalSize;
        //}
        #endregion

        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public long GetDocumentZippedFileSizeByIds(string docIds, int docAppId)
        {
            long totalSize = 0;            
            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");

            ArrayList docList = Util.MergeDocumentsToTemp(docIds, saveDir);
                        
            if (docList.Count > 0)
            {
                using (ZipFile zip = new ZipFile())
                {

                    DocAppDb docAppDb = new DocAppDb();
                    DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(docAppId);
                    DocApp.DocAppRow docAppRow = docApp[0];

                    string zipFileName = Path.Combine(saveDir, docAppRow.RefNo + "_" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt) + ".zip");

                    foreach (string docPath in docList)
                    {
                        FileInfo fi = new FileInfo(docPath);

                        try
                        {
                            using (FileStream fileStream = fi.Open(FileMode.Open, FileAccess.Read))
                            {
                                BinaryReader binaryReader = new BinaryReader(fileStream);
                                byte[] fileBytes = binaryReader.ReadBytes((Int32)fi.Length);

                                string rawFileName = fi.Name;

                                zip.AddEntry(rawFileName, fileBytes);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    zip.Save(zipFileName);

                    FileInfo zipFileInfo = new FileInfo(zipFileName);
                    totalSize = zipFileInfo.Length;
                }
            }
            return totalSize;
        }
        #endregion

        public string GetDownloadableDocumentFileName(List<int> docIds, int setId)
        {
            DocDb docDb = new DocDb();
            DocSetDb docSetDb = new DocSetDb();
            RawPageDb rawPageDb = new RawPageDb();
            DocTypeDb docTypeDb = new DocTypeDb();

            #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            // RawPage Folder
            //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
            //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);
            #endregion 

            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");

            ArrayList docList = new ArrayList();

            foreach (int id in docIds)
            {
                Doc.DocDataTable docTable = docDb.GetDocById(id);

                if (docTable.Rows.Count > 0)
                {
                    Doc.DocRow doc = docTable[0];

                    ArrayList pageList = new ArrayList();

                    RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(id);

                    for (int cnt = 0; cnt < rawPages.Count; cnt++)
                    {
                        RawPage.RawPageRow rawPage = rawPages[cnt];
                        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
                        //DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

                        //if (rawPageDirs.Length > 0)
                        //{
                        //    DirectoryInfo rawPageDir = rawPageDirs[0];

                        //    // Get the raw page for download
                        //    FileInfo[] rawPageFiles = rawPageDir.GetFiles();

                        //    bool hasRawPage = false;
                        //    foreach (FileInfo rawPageFile in rawPageFiles)
                        //    {
                        //        if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                        //            !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                        //            !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                        //        {
                        //            if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                        //            {
                        //                //path = Util.CreatePdfFileFromImage(path);
                        //                pageList.Add(rawPageFile.FullName);
                        //                hasRawPage = true;
                        //            }
                        //        }
                        //    }

                        //    // If the raw page is not found, use the searcheable PDF
                        //    if (!hasRawPage)
                        //    {
                        //        FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

                        //        if (rawPagePdfFiles.Length > 0)
                        //            pageList.Add(rawPagePdfFiles[0].FullName);
                        //    }
                        //}


                        #endregion

                        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
                        DirectoryInfo rawPageDirs = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);                                               
                        // Get the raw page for download
                        FileInfo[] rawPageFiles = rawPageDirs.GetFiles();

                        bool hasRawPage = false;
                        foreach (FileInfo rawPageFile in rawPageFiles)
                        {
                            if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                                !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                                !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                            {
                                if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                                {                                    
                                    pageList.Add(rawPageFile.FullName);
                                    hasRawPage = true;
                                }
                            }
                        }

                        // If the raw page is not found, use the searcheable PDF
                        if (!hasRawPage)
                        {
                            FileInfo[] rawPagePdfFiles = rawPageDirs.GetFiles("*_s.pdf");

                            if (rawPagePdfFiles.Length > 0)
                                pageList.Add(rawPagePdfFiles[0].FullName);
                        }                        
                        #endregion
                    }

                    if (!Directory.Exists(saveDir))
                        Directory.CreateDirectory(saveDir);

                    if (pageList.Count > 0)
                    {
                        string docTypeDesc = doc.DocTypeCode;

                        DocType.DocTypeDataTable docTypeTable = docTypeDb.GetDocType(doc.DocTypeCode);

                        if (docTypeTable.Rows.Count > 0)
                        {
                            DocType.DocTypeRow docType = docTypeTable[0];
                            docTypeDesc = docType.Description;
                        }

                        string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + doc.Id.ToString() + ".pdf");

                        try
                        {
                            if (File.Exists(mergedFileName))
                                File.Delete(mergedFileName);
                        }
                        catch (Exception)
                        {
                        }

                        Util.MergePdfFiles(pageList, mergedFileName);

                        docList.Add(mergedFileName);
                    }
                }
            }

            if (docList.Count > 0)
            {
                string setNo = docSetDb.GetSetNumber(setId);

                string mergedFileName = Path.Combine(saveDir, setNo + "_" +
                    Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt) + ".pdf");

                try
                {
                    if (File.Exists(mergedFileName))
                        File.Delete(mergedFileName);
                }
                catch (Exception)
                {
                }

                string errorMessage = string.Empty;
                Util.MergePdfFiles(docList, mergedFileName, out errorMessage);

                if (String.IsNullOrEmpty(errorMessage))
                {
                    return mergedFileName;
                }
            }

            return string.Empty;
        }

        #endregion

        #region Added By Edward 2014/07/04 Changes on July 1 2014 Meeting
        public DataTable GetDocForSummaryIncomeExtraction(int docAppId, string nric, bool isHa, string referenceType)
        {
            return DocDs.GetDocForSummaryIncomeExtraction(docAppId, nric, isHa, referenceType);
        }
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static DataTable GetDocSetIdAndDateByDocId(int docId)
        {
            return DocDs.GetDocSetIdAndDateByDocId(docId);
        }
        #endregion

        #region Added by Edward Separate the Treeview Node Drop to modules 2017/11/14
        public static bool UpdateDocFromNodeDrop(int docId)
        {

            return DocDs.UpdateDocFromNodeDrop(docId);
        }
        #endregion
    }
}
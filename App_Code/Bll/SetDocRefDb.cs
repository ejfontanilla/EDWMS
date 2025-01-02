using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using SetDocRefTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AppSetDocRefRefDb
    /// </summary>
    public class SetDocRefDb
    {
        private SetDocRefTableAdapter _SetDocRefAdapter = null;

        protected SetDocRefTableAdapter Adapter
        {
            get
            {
                if (_SetDocRefAdapter == null)
                    _SetDocRefAdapter = new SetDocRefTableAdapter();

                return _SetDocRefAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public SetDocRef.SetDocRefDataTable GetSetDocRef()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get SetDocRef by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SetDocRef.SetDocRefDataTable GetSetDocRefById(int id)
        {
            return Adapter.GetSetDocRefById(id);
        }

        /// <summary>
        /// Get SetDocRef by DocId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public SetDocRef.SetDocRefDataTable GetSetDocRefByDocId(int docId)
        {
            return Adapter.GetSetDocRefByDocId(docId);
        }

        /// <summary>
        /// Get SetDocRef by DocId and RefId
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public SetDocRef.SetDocRefDataTable GetSetDocRefByDocIdAndRefId(int docId, int refId)
        {
            return Adapter.GetSetDocRefByDocIdAndRefId(docId, refId);
        }
        

        #endregion

        #region Insert Methods

        /// <summary>
        /// Add SetDocRef
        /// </summary>
        /// <param name="docPersonalId"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int Insert(int docPersonalId, int docId)
        {
            SetDocRef.SetDocRefDataTable SetDocRefExist = Adapter.GetSetDocRefByDocPersonalIdAndDocId(docPersonalId, docId);

            if (SetDocRefExist.Rows.Count == 0)
            {
                SetDocRef.SetDocRefDataTable dt = new SetDocRef.SetDocRefDataTable();
                SetDocRef.SetDocRefRow r = dt.NewSetDocRefRow();

                r.DocPersonalId = docPersonalId;
                r.DocId = docId;

                dt.AddSetDocRefRow(r);
                Adapter.Update(dt);
                int id = r.Id;
                return id;
            }
            else
                return -1;
        }

        /// <summary>
        /// Copy records form source DocId to des Doc Id where the source and destination docsetid is same
        /// </summary>
        /// <param name="sourceDocId"></param>
        /// <param name="desDocId"></param>
        public void CopyFromSource(int sourceDocId, int desDocId)
        {
            //get records from source DocId and duplicate them to des DocId
            SetDocRef.SetDocRefDataTable setDocRefs = Adapter.GetSetDocRefByDocId(sourceDocId);

            foreach (SetDocRef.SetDocRefRow setDocRefRow in setDocRefs.Rows)
            {
                SetDocRef.SetDocRefDataTable dt = new SetDocRef.SetDocRefDataTable();
                SetDocRef.SetDocRefRow r = dt.NewSetDocRefRow();

                r.DocPersonalId = setDocRefRow.DocPersonalId;
                r.DocId = desDocId;

                dt.AddSetDocRefRow(r);
                Adapter.Update(dt);
            }
        }

        /// <summary>
        /// Copy records form source DocId to des Doc Id where the source and destination docsetid are different
        /// </summary>
        /// <param name="sourceDocId"></param>
        /// <param name="desDocId"></param>
        /// <param name="newDocSetId"></param>
        public void CopyFromSource(int sourceDocId, int desDocId, int newDocSetId)
        {
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalByDocId(sourceDocId);

            foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals)
            {
                int newDocPersonalId = -1;

                if (!docPersonalRow.IsRelationshipNull())
                {
                    RelationshipEnum relationship = new RelationshipEnum();
                    relationship = (RelationshipEnum)Enum.Parse(typeof(RelationshipEnum), docPersonalRow.Relationship, true);
                    newDocPersonalId = docPersonalDb.Insert(newDocSetId, docPersonalRow.IsNricNull() ? "" : docPersonalRow.Nric, docPersonalRow.IsNameNull() ? "" : docPersonalRow.Name, docPersonalRow.Folder, relationship);
                }
                else
                    newDocPersonalId = docPersonalDb.Insert(newDocSetId, docPersonalRow.IsNricNull() ? "" : docPersonalRow.Nric, docPersonalRow.IsNameNull() ? "" : docPersonalRow.Name, docPersonalRow.Folder, null);

                SetDocRef.SetDocRefDataTable dt = new SetDocRef.SetDocRefDataTable();
                SetDocRef.SetDocRefRow r = dt.NewSetDocRefRow();

                r.DocPersonalId = newDocPersonalId;
                r.DocId = desDocId;

                dt.AddSetDocRefRow(r);
                Adapter.Update(dt);
            }
        }

        #endregion

        #region Update Methods

        #endregion

        #region Delete

        /// <summary>
        /// Delete SetDocRef by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;
            Guid? operationId = auditTrailDb.Record(TableNameEnum.SetDocRef, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.SetDocRef, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Delete the setDocRef By docid
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public bool DeleteByDocId(int docId)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;
            SetDocRef.SetDocRefDataTable setDocRefs = GetSetDocRefByDocId(docId);

            foreach (SetDocRef.SetDocRefRow setDocRefRow in setDocRefs.Rows)
            {
                Guid? operationId = auditTrailDb.Record(TableNameEnum.SetDocRef, setDocRefRow.Id.ToString(), OperationTypeEnum.Delete);
                rowsAffected = Adapter.Delete(setDocRefRow.Id);
            }

            return (rowsAffected > 0);
        }
        
        #endregion
    }
}
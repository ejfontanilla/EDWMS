using System;
using System.Collections.Generic;
using System.Web;
using MetaDataTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for MetaDataDb
    /// </summary>
    public class MetaDataDb
    {
        private MetaDataTableAdapter _MetaDataAdapter = null;

        protected MetaDataTableAdapter Adapter
        {
            get
            {
                if (_MetaDataAdapter == null)
                    _MetaDataAdapter = new MetaDataTableAdapter();

                return _MetaDataAdapter;
            }
        }

        #region Retrieve Methods

        /// <summary>
        /// Get all MetaField
        /// </summary>
        /// <returns></returns>
        public MetaData.MetaDataDataTable GetMetaData()
        {
            MetaData.MetaDataDataTable dataTable = Adapter.GetMetaData();
            return dataTable;
        }

        /// <summary>
        /// Get MetaData by DocId
        /// </summary>
        /// <returns></returns>
        public MetaData.MetaDataDataTable GetMetaDataByDocId(int docId, Boolean isOldData, Boolean isStamp)
        {
            MetaData.MetaDataDataTable dataTable = Adapter.GetMetaDataByDocId(docId, isOldData, isStamp);
            return dataTable;
        }

        public MetaData.MetaDataDataTable GetMetaDataByDocIdWithDummyFieldId(int docId, Boolean isOldData, Boolean isStamp)
        {
            MetaData.MetaDataDataTable metaDatas = Adapter.GetMetaDataByDocId(docId, isOldData, isStamp);

            MetaData.MetaDataDataTable finalMetaDatas = new MetaData.MetaDataDataTable();

            finalMetaDatas.Columns.Add("FieldId", typeof(string));

            foreach (MetaData.MetaDataRow dr in metaDatas.Rows)
            {
                MetaData.MetaDataRow metaDataRow = finalMetaDatas.NewMetaDataRow();

                metaDataRow.Id = dr.Id;
                metaDataRow.FieldName = dr.FieldName.Trim();
                metaDataRow.FieldValue = dr.FieldValue.Trim();
                metaDataRow.Doc = dr.Doc;
                metaDataRow.VerificationMandatory = dr.VerificationMandatory;
                metaDataRow.CompletenessMandatory = dr.CompletenessMandatory;
                metaDataRow.VerificationVisible = dr.VerificationVisible;
                metaDataRow.CompletenessVisible = dr.CompletenessVisible;
                metaDataRow.Fixed = dr.Fixed;
                metaDataRow.MaximumLength = dr.MaximumLength;
                metaDataRow.isOldData = dr.isOldData;
                metaDataRow.isStamp = dr.isStamp;
                metaDataRow.CreatedDate = dr.CreatedDate;
                metaDataRow.ModifiedDate = dr.ModifiedDate;
                metaDataRow["FieldId"] = string.Empty;

                finalMetaDatas.Rows.Add(metaDataRow);
            }

            return finalMetaDatas;
        }

        public MetaData.MetaDataDataTable GetMetaDataIsOldByDocId(int docId, Boolean isOldData)
        {
            MetaData.MetaDataDataTable dataTable = Adapter.GetMetaDataIsOldByDocId(docId, isOldData);
            return dataTable;
        }
       
        #endregion

        #region Insert Methods

        /// <summary>
        /// Insert Metadata
        /// </summary>
        /// <param name="metaFieldValue"></param>
        /// <param name="metaFieldName"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public int Insert(string metaFieldName, string metaFieldValue, int doc, bool isStamp)
        {
            //check if record exist for the doc and fieldname, if exist do not enter the value

            MetaData.MetaDataDataTable metaDataExist = Adapter.GetMetaDataByDocIdFieldNameIsStamp(doc, metaFieldName, isStamp);

            if (metaDataExist.Rows.Count == 0)
            {
                MetaData.MetaDataDataTable metaData = new MetaData.MetaDataDataTable();
                MetaData.MetaDataRow r = metaData.NewMetaDataRow();

                r.FieldName = metaFieldName.Trim();
                r.FieldValue = metaFieldValue.Trim();
                r.Doc = doc;
                r.isOldData = false;
                r.isStamp = isStamp;
                r.CreatedDate = DateTime.Now;
                r.ModifiedDate = DateTime.Now;
                metaData.AddMetaDataRow(r);

                Adapter.Update(metaData);
                int id = r.Id;

                if (id > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Insert);
                }
                return id;
            }
            else
                return -1;
        }

        /// <summary>
        /// insert metadata of a document by docTypeCode
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public bool InsertByDocTypeCode(int docId, string docTypeCode)
        {
            MetaFieldDb metaFieldDb = new MetaFieldDb();
            MetaField.MetaFieldDataTable metaField = new MetaField.MetaFieldDataTable();
            metaField = metaFieldDb.GetMetaFieldByDocTypeCode(docTypeCode);

            if (metaField.Rows.Count > 0)
            {
                MetaData.MetaDataDataTable metaData = new MetaData.MetaDataDataTable();

                foreach (MetaField.MetaFieldRow rMF in metaField.Rows)
                {
                    MetaData.MetaDataRow r = metaData.NewMetaDataRow();

                    r.CompletenessMandatory = rMF.CompletenessMandatory;
                    r.CompletenessVisible = rMF.CompletenessVisible;
                    r.VerificationMandatory = rMF.VerificationMandatory;
                    r.VerificationVisible = rMF.VerificationVisible;
                    r.Doc = docId;
                    r.FieldName = rMF.FieldName.Trim();
                    r.MaximumLength = rMF.MaximumLength;
                    r.Fixed = rMF.Fixed;
                    r.FieldValue = string.Empty;
                    r.isOldData = false;
                    r.isStamp = true;
                    r.CreatedDate = DateTime.Now;
                    r.ModifiedDate = DateTime.Now;

                    metaData.AddMetaDataRow(r);

                    Adapter.Update(metaData);
                    int id = r.Id;

                    if (id > 0)
                    {
                        AuditTrailDb auditTrailDb = new AuditTrailDb();
                        auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Insert);
                    }
                }

                return true;
            }
            else
                return false;
        }

        public int Insert(int docId, string fieldName, string fieldValue, bool verMandatory, bool comMandatory,
            bool verVisible, bool comVisible, bool isFixed, int maximumLength, bool isStamp)
        {
            MetaData.MetaDataDataTable metaDataExist = Adapter.GetMetaDataByDocIdFieldNameIsStamp(docId, fieldName, isStamp);

            if (metaDataExist.Rows.Count == 0)
            {
                MetaData.MetaDataDataTable dt = new MetaData.MetaDataDataTable();
                MetaData.MetaDataRow r = dt.NewMetaDataRow();

                r.Doc = docId;
                r.FieldName = fieldName.Trim();

                if (String.IsNullOrEmpty(fieldValue))
                    fieldValue = " ";

                r.FieldValue = fieldValue.Trim();
                r.VerificationMandatory = verMandatory;
                r.CompletenessMandatory = comMandatory;
                r.VerificationVisible = verVisible;
                r.CompletenessVisible = comVisible;
                r.Fixed = isFixed;
                r.MaximumLength = maximumLength;
                r.isOldData = false;
                r.isStamp = isStamp;
                r.CreatedDate = DateTime.Now;
                r.ModifiedDate = DateTime.Now;

                dt.AddMetaDataRow(r);
                Adapter.Update(dt);
                int id = r.Id;
                return id;
            }
            else
                return -1;
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Update Metadata 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="metaFieldValue"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool Update(int id, string metaFieldValue)
        {
            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataById(id);

            if (metaData.Count == 0)
                return false;

            MetaData.MetaDataRow r = metaData[0];

            r.FieldValue = metaFieldValue.Trim();
            r.ModifiedDate = DateTime.Now;

            int rowsAffected = Adapter.Update(metaData);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }


        /// <summary>
        /// update is Old Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldDocTypeCode"></param>
        /// <param name="newDocTypeCode"></param>
        /// <returns></returns>
        public bool UpdateIsOldDataStatus(int id, Boolean isOldData)
        {
            //get new records, loop and upate with old values.

            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataIsOldByDocId(id, !isOldData);

            if (metaData.Count == 0)
                return false;

            int rowsAffected = -1;


            foreach (MetaData.MetaDataRow rMd in metaData.Rows)
            {
                try
                {
                    rMd.isOldData = isOldData;
                    rMd.ModifiedDate = DateTime.Now;
                }
                catch (Exception e)
                { }

                rowsAffected = Adapter.Update(metaData);

                if (rowsAffected > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Update);
                }
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update New Metadata with old metadata based on fieldname
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateNewData(int id)
        {
            //get new values and loop throuh and update with old values
            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataByDocId(id, false, true);

            if (metaData.Count == 0)
                return false;

            int rowsAffected = -1;


            foreach (MetaData.MetaDataRow rMd in metaData.Rows)
            {
                try
                {
                    rMd.FieldValue = Adapter.GetOldMetaDataFieldValue(id, rMd.FieldName).Rows[0]["FieldValue"].ToString().Trim();
                    rMd.ModifiedDate = DateTime.Now;
                }
                catch (Exception e)
                { }

                rowsAffected = Adapter.Update(metaData);

                if (rowsAffected > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Update);
                }
            }

            return rowsAffected == 1;
        }

        #endregion

        #region Delete Methods


        /// <summary>
        /// Delete Old data by docid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteOldDataByDocID(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;

            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataIsOldByDocId(id,true);

            foreach (MetaData.MetaDataRow row in metaData.Rows)
            {
                Id = row.Id;
                Guid? operationId = auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Delete);
                rowsAffected = Adapter.Delete(Id);
            }

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Delete the Stamo Data from MetaData
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteStampDataByDocID(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;

            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataByDocId(id, false, true);

            foreach (MetaData.MetaDataRow row in metaData.Rows)
            {
                Id = row.Id;
                Guid? operationId = auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Delete);
                rowsAffected = Adapter.Delete(Id);
            }

            return (rowsAffected > 0);
        }
        

        /// <summary>
        /// Delete all the metadata by Docid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteByDocID(int id, bool isOldData, bool isStamp)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;

            MetaData.MetaDataDataTable metaData = Adapter.GetMetaDataByDocId(id, isOldData, isStamp);

            foreach (MetaData.MetaDataRow row in metaData.Rows)
            {
                Id = row.Id;
                Guid? operationId = auditTrailDb.Record(TableNameEnum.MetaData, id.ToString(), OperationTypeEnum.Delete);
                rowsAffected = Adapter.Delete(Id);
            }

            return (rowsAffected > 0);
        }

        #endregion
    }
}
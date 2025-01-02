using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using MetaFieldTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for MetaFieldDb
    /// </summary>
    public class MetaFieldDb
    {
        private MetaFieldTableAdapter _MetaFieldAdapter = null;

        protected MetaFieldTableAdapter Adapter
        {
            get
            {
                if (_MetaFieldAdapter == null)
                    _MetaFieldAdapter = new MetaFieldTableAdapter();

                return _MetaFieldAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all MetaField
        /// </summary>
        /// <returns></returns>
        public MetaField.MetaFieldDataTable GetMetaField()
        {
            MetaField.MetaFieldDataTable dataTable = Adapter.GetMetaField();
            return dataTable;
        }


        /// <summary>
        /// Get all MetaFields by DocType
        /// </summary>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public MetaField.MetaFieldDataTable GetMetaFieldByDocTypeCode(string docTypeCode)
        {
            return Adapter.GetMetaFieldByDocTypeCode(docTypeCode);
        }

        /// <summary>
        /// Get all MetaFields by DocType
        /// </summary>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public MetaField.MetaFieldDataTable GetMetaFieldByDocTypeCodeWithDummyFieldValue(string docTypeCode)
        {
            MetaField.MetaFieldDataTable metaFields = Adapter.GetMetaFieldByDocTypeCode(docTypeCode);

            MetaField.MetaFieldDataTable finalMetaFields = new MetaField.MetaFieldDataTable();

            finalMetaFields.Columns.Add("FieldValue", typeof(string));
            finalMetaFields.Columns.Add("FieldId", typeof(string));

            foreach (MetaField.MetaFieldRow dr in metaFields.Rows)
            {
                MetaField.MetaFieldRow metaFieldRow = finalMetaFields.NewMetaFieldRow();

                metaFieldRow.Id = dr.Id;
                metaFieldRow.FieldName = dr.FieldName;
                metaFieldRow.FieldDescription = dr.FieldDescription;
                metaFieldRow.Remark = dr.IsRemarkNull() ? string.Empty : dr.Remark;
                metaFieldRow.DocTypeCode = dr.DocTypeCode;
                metaFieldRow.VerificationMandatory = dr.VerificationMandatory;
                metaFieldRow.CompletenessMandatory = dr.CompletenessMandatory;
                metaFieldRow.VerificationVisible = dr.VerificationVisible;
                metaFieldRow.CompletenessVisible = dr.CompletenessVisible;
                metaFieldRow.Fixed = dr.Fixed;
                metaFieldRow.MaximumLength = dr.MaximumLength;
                metaFieldRow["FieldValue"] = string.Empty;
                metaFieldRow["FieldId"] = string.Empty;

                finalMetaFields.Rows.Add(metaFieldRow);
            }

            return finalMetaFields;
        }

        /// <summary>
        /// Get Meta Field By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public MetaField.MetaFieldDataTable GetMetaFieldById(int Id)
        {
            MetaField.MetaFieldDataTable dataTable = Adapter.GetMetaFieldById(Id);
            return dataTable;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update MetaField
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldName"></param>
        /// <param name="maximumLength"></param>
        /// <param name="completenessMandatory"></param>
        /// <param name="completenessVisible"></param>
        /// <param name="verificationMandatory"></param>
        /// <param name="verificationVisible"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Update(int id, string fieldName, int maximumLength, bool completenessMandatory, bool completenessVisible, bool verificationMandatory, bool verificationVisible, string docTypeCode)
        {
            MetaField.MetaFieldDataTable metaField =  Adapter.GetMetaFieldById(id);

            if (metaField.Count == 0)
                return false;

            MetaField.MetaFieldRow row = metaField[0];

            row.CompletenessMandatory = completenessMandatory;
            row.CompletenessVisible = completenessVisible;
            row.VerificationMandatory = verificationMandatory;
            row.VerificationVisible = verificationVisible;
            row.FieldName = fieldName;
            row.MaximumLength = maximumLength;
            row.DocTypeCode = docTypeCode;

            int rowsAffected = Adapter.Update(metaField);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.MetaField, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        #endregion

        #region Insert Methods

        /// <summary>
        /// Insert a new Meta Field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="maximumLength"></param>
        /// <param name="completenessMandatory"></param>
        /// <param name="completenessVisible"></param>
        /// <param name="verificationMandatory"></param>
        /// <param name="verificationVisible"></param>
        /// <returns></returns>
        public int Insert(string fieldName, int maximumLength, bool completenessMandatory, bool completenessVisible, bool verificationMandatory, bool verificationVisible, string docTypeCode)
        {
            MetaField.MetaFieldDataTable metaField = new MetaField.MetaFieldDataTable();
            MetaField.MetaFieldRow row = metaField.NewMetaFieldRow();

            row.CompletenessMandatory = completenessMandatory;
            row.CompletenessVisible = completenessVisible;
            row.VerificationMandatory = verificationMandatory;
            row.VerificationVisible = verificationVisible;
            row.FieldName = fieldName;
            row.MaximumLength = maximumLength;
            row.DocTypeCode = docTypeCode;
            row.FieldDescription = string.Empty;
            row.Fixed = false;

            metaField.AddMetaFieldRow(row);

            Adapter.Update(metaField);
            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.MetaField, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }
        #endregion

        #region Delete Methods

        /// <summary>
        /// Delete MetaField by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;

            MetaField.MetaFieldDataTable metaField = Adapter.GetMetaFieldById(id);

            if (metaField.Count > 0)
            {
                MetaField.MetaFieldRow row = metaField[0];
                Id = row.Id;
            }
            //Note:- MetaData values will be deleted on cascade delete rule when a metafield is deleted

            Guid? operationId = auditTrailDb.Record(TableNameEnum.MetaField, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected == 0)
            {
                auditTrailDb.Record(TableNameEnum.MetaField, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }

        #endregion
    }
}
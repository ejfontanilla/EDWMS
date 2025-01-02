using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using SampleDocTableAdapters;

namespace Dwms.Bll
{
    public class SampleDocDb
    {
        private SampleDocTableAdapter _SampleDocAdapter = null;

        protected SampleDocTableAdapter Adapter
        {
            get
            {
                if (_SampleDocAdapter == null)
                    _SampleDocAdapter = new SampleDocTableAdapter();

                return _SampleDocAdapter;
            }
        }

        #region Retrieve Methods

        public SampleDoc.SampleDocDataTable GetDataById(int id)
        {
            return Adapter.GetDataById(id);
        }

        public SampleDoc.SampleDocDataTable GetDataByPageId(int samplePageId)
        {
            return Adapter.GetDataByPageId(samplePageId);
        }

        public SampleDoc.SampleDocDataTable GetDataByDocType(string docTypeCode)
        {
            return Adapter.GetDataByDocType(docTypeCode);
        }

        #endregion

        #region Insert Methods
        public int Insert(string docTypeCode, string fileName, byte[] fileData)
        {
            SampleDoc.SampleDocDataTable sampleDoc = new SampleDoc.SampleDocDataTable();
            SampleDoc.SampleDocRow row = sampleDoc.NewSampleDocRow();

            row.DocTypeCode = docTypeCode;
            row.FileName = fileName;
            row.FileData = fileData;
            row.IsOcr = false;
            row.DateIn = DateTime.Now;

            sampleDoc.AddSampleDocRow(row);
            Adapter.Update(sampleDoc);
            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SampleDoc, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }

        public int Insert(string docTypeCode, string fileName, byte[] fileData, bool isOcr)
        {
            SampleDoc.SampleDocDataTable sampleDoc = new SampleDoc.SampleDocDataTable();
            SampleDoc.SampleDocRow row = sampleDoc.NewSampleDocRow();

            row.DocTypeCode = docTypeCode;
            row.FileName = fileName;
            row.FileData = fileData;
            row.IsOcr = isOcr;
            row.DateIn = DateTime.Now;

            sampleDoc.AddSampleDocRow(row);
            Adapter.Update(sampleDoc);
            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SampleDoc, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion

        #region Update Methods
        #endregion

        #region Delete Methods

        public bool Delete(int id)
        {
            return (Adapter.Delete(id) > 0);
        }

        /// <summary>
        /// Delete by doc set id
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public bool DeleteByDocSetId(int docSetId)
        {
            return (Adapter.DeleteByDocSetId(docSetId.ToString()) > 0);
        }
        #endregion
    }
}
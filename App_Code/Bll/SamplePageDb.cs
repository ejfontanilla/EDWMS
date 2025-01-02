using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using SamplePageTableAdapters;

namespace Dwms.Bll
{
    public class SamplePageDb
    {
        private SamplePageTableAdapter _SamplePageAdapter = null;

        protected SamplePageTableAdapter Adapter
        {
            get
            {
                if (_SamplePageAdapter == null)
                    _SamplePageAdapter = new SamplePageTableAdapter();

                return _SamplePageAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get sample pages
        /// </summary>
        /// <returns></returns>
        public SamplePage.SamplePageDataTable GetSamplePages()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get sample page by id
        /// </summary>
        /// <returns></returns>
        public SamplePage.SamplePageDataTable GetSamplePageById(int id)
        {
            return Adapter.GetDataById(id);
        }

        public SamplePage.SamplePageDataTable GetSamplePageBySampleDocId(int sampleDocId)
        {
            return Adapter.GetDataBySampleDocId(sampleDocId);
        }
        #endregion

        #region Insert Methods

        public int Insert(int sampleDocId, string ocrText, bool isOcr)
        {
            SamplePage.SamplePageDataTable sampleDoc = new SamplePage.SamplePageDataTable();
            SamplePage.SamplePageRow row = sampleDoc.NewSamplePageRow();

            row.SampleDocId = sampleDocId;
            row.OcrText = ocrText;
            row.IsOcr = isOcr;

            sampleDoc.AddSamplePageRow(row);
            Adapter.Update(sampleDoc);
            int id = row.Id;

            //if (id > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.SamplePage, id.ToString(), OperationTypeEnum.Insert);
            //}

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
        public bool Update(int id, string ocrText, bool isOcr)
        {
            SamplePage.SamplePageDataTable dt = GetSamplePageById(id);

            if (dt.Rows.Count == 0) return false;

            SamplePage.SamplePageRow r = dt[0];

            r.OcrText = ocrText;
            r.IsOcr = isOcr;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ocrText"></param>
        /// <param name="isOcr"></param>
        /// <returns></returns>
        public bool Update(int id, bool isOcr)
        {
            SamplePage.SamplePageDataTable dt = GetSamplePageById(id);

            if (dt.Rows.Count == 0) return false;

            SamplePage.SamplePageRow r = dt[0];

            r.IsOcr = isOcr;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #region Delete Methods

        public bool Delete(int id)
        {
            return (Adapter.Delete(id) > 0);
        }

        #endregion
    }
}
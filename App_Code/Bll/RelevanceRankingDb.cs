using System;
using System.Collections.Generic;
using System.Web;
using RelevanceRankingTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for RelevanceRankingDb
    /// </summary>
    public class RelevanceRankingDb
    {
        private RelevanceRankingTableAdapter _RelevanceRankingAdapter = null;

        protected RelevanceRankingTableAdapter Adapter
        {
            get
            {
                if (_RelevanceRankingAdapter == null)
                    _RelevanceRankingAdapter = new RelevanceRankingTableAdapter();

                return _RelevanceRankingAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve relevance ranking
        /// </summary>
        /// <returns></returns>
        public RelevanceRanking.RelevanceRankingDataTable GetRelevanceRanking()
        {
            return Adapter.GetData();
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert doc app
        /// </summary>
        /// <returns></returns>
        public int Insert(int sampleDocId, DateTime categorizationDate, bool isMatch, int rawPageId)
        {
            RelevanceRanking.RelevanceRankingDataTable dt = new RelevanceRanking.RelevanceRankingDataTable();
            RelevanceRanking.RelevanceRankingRow r = dt.NewRelevanceRankingRow();

            r.SampleDocId = sampleDocId;
            r.CategorizationDate = categorizationDate;
            r.IsMatch = isMatch;
            r.RawPageId = rawPageId;

            dt.AddRelevanceRankingRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.RelevanceRanking, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update the Set number of the set
        /// </summary>
        /// <returns></returns>
        public bool UpdateSetNumber(int id)
        {
            //RelevanceRanking.RelevanceRankingDataTable dt = Adapter.GetRelevanceRankingById(id);

            //if (dt.Count == 0)
            //    return false;

            //RelevanceRanking.RelevanceRankingRow dr = dt[0];

            //int temp1 = -1;
            //int temp2 = -1;
            //dr.SetNo = BllFunc.FormulateSetNumber(id, out temp1, out temp2);

            //int rowsAffected = Adapter.Update(dt);

            //if (rowsAffected > 0)
            //{
            //    Util.RecordAudit(TableNameEnum.RelevanceRanking, id.ToString(), OperationTypeEnum.Update);
            //}

            //return rowsAffected == 1;
            return true;
        }
        #endregion

        #region Delete Methods
        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }
        #endregion
    }
}
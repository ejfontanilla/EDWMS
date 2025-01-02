using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Dwms.Dal;
using RawFileTableAdapters;

namespace Dwms.Bll
{
    public class RawFileDb
    {
        private RawFileTableAdapter _RawFileTableAdapter = null;

        protected RawFileTableAdapter Adapter
        {
            get
            {
                if (_RawFileTableAdapter == null)
                    _RawFileTableAdapter = new RawFileTableAdapter();

                return _RawFileTableAdapter;
            }
        }

        public RawFile.RawFileDataTable GetData()
        {
            return Adapter.GetData();
        }

        public RawFile.RawFileDataTable GetBySetIdRange(int startSetId, int endSetId)
        {
            return Adapter.GetBySetIdRange(startSetId, endSetId);
        }

        /// <summary>
        /// Get rawfiles for respective docsets
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public RawFile.RawFileDataTable GetDataByDocSetId(int docSetId)
        {
            return Adapter.GetDataByDocSetId(docSetId);
        }

        /// <summary>
        /// Get rawFile by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RawFile.RawFileDataTable GetRawFileById(int id)
        {
            return Adapter.GetRawFileById(id);
        }

        /// <summary>
        /// Get rawFileId by docId
        /// </summary>
        /// <param name="docIds"></param>
        /// <returns></returns>
        public DataTable GetDataByDocIds(string docIds)
        {
            return RawFileDs.GetDataByDocIds(docIds);
        }

        

        /// <summary>
        /// Get rawfiles for respective application
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public RawFile.RawFileDataTable GetDataByDocAppId(int docSetId)
        {
            return Adapter.GetDataByDocAppId(docSetId);
        }

        /// <summary>
        /// Insert the raw file
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="fileName"></param>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public int Insert(int docSetId, string fileName, byte[] fileData)
        {
            RawFile.RawFileDataTable dt = new RawFile.RawFileDataTable();
            RawFile.RawFileRow r = dt.NewRawFileRow();

            r.DocSetId = docSetId;
            r.FileName = fileName;
            r.FileData = fileData;
            r.SkipCategorization = false;
            dt.AddRawFileRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }

        /// <summary>
        /// Insert from old RawFile record
        /// </summary>
        /// <param name="oldSetId"></param>
        /// <param name="newSetId"></param>
        /// <returns></returns>
        public Dictionary<int, int> InsertFromOldDocs(List<int> docIds, int oldSetId, int newSetId)
        {
            Dictionary<int, int> oldToNewRawFileIds = new Dictionary<int, int>();

            System.Text.StringBuilder sqlDocIds = new System.Text.StringBuilder();
            sqlDocIds.Append("''");

            foreach (int i in docIds)
            {
                sqlDocIds.Append(",'" + i + "'");
            }

            RawFileDb rawFileDb = new RawFileDb();
            DataTable rawFiles = rawFileDb.GetDataByDocIds(sqlDocIds.ToString());

            foreach (DataRow rawFileRow in rawFiles.Rows)
            {
                RawFile.RawFileDataTable newRawFiles = new RawFile.RawFileDataTable();
                RawFile.RawFileRow newRawFileRow = newRawFiles.NewRawFileRow();

                newRawFileRow.DocSetId = newSetId;
                newRawFileRow.FileName = rawFileRow["FileName"].ToString();
                newRawFileRow.FileData = new byte[0];
                //newRawFileRow.FileData = System.Text.Encoding.ASCII.GetBytes(rawFileRow["FileData"].ToString());

                newRawFiles.AddRawFileRow(newRawFileRow);
                Adapter.Update(newRawFiles);

                int id = newRawFileRow.Id;

                if (id > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.RawFile, id.ToString(), OperationTypeEnum.Insert);
                    oldToNewRawFileIds.Add(int.Parse(rawFileRow["Id"].ToString().Trim()), id);
                }
            }
            
            return oldToNewRawFileIds;
        }
    }
}
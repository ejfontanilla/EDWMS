using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using ScanChannelTableAdapters;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public class ScanChannelDb
    {
        private ScanChannelTableAdapter _ScanChannelAdapter = null;

        protected ScanChannelTableAdapter Adapter
        {
            get
            {
                if (_ScanChannelAdapter == null)
                    _ScanChannelAdapter = new ScanChannelTableAdapter();

                return _ScanChannelAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get ScanChannels
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScanChannel.ScanChannelDataTable GetChannel()
        {
            return Adapter.GetChannels();
        }

        /// <summary>
        /// Get all the channels which includes scan channels and uploadchannels
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllChannels()
        {
            DataTable channels = new DataTable();
            channels.Columns.Add("Name");

            ScanChannelDb scanChannelDb = new ScanChannelDb();
            UploadChannelDb uploadChannelDb = new UploadChannelDb();

            // Add the Scan Channel
            ScanChannel.ScanChannelDataTable scanChannelDt = scanChannelDb.GetChannel();
            foreach (ScanChannel.ScanChannelRow scanChannel in scanChannelDt.Rows)
            {
                DataRow row = channels.NewRow();
                row["Name"] = scanChannel.Name;
                channels.Rows.Add(row);
            }

            // Add the Upload Channel
            UploadChannel.UploadChannelDataTable uploadChannelDt = uploadChannelDb.GetChannel();
            foreach (UploadChannel.UploadChannelRow uploadChannel in uploadChannelDt.Rows)
            {
                DataRow row = channels.NewRow();
                row["Name"] = uploadChannel.Name;
                channels.Rows.Add(row);
            }

            return channels;
        }

        /// <summary>
        /// Get Categorisation Rule keyword by rule id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public ScanChannel.ScanChannelDataTable GetScanChannel(int ruleId)
        //{
        //    return Adapter.GetDataByRuleId(ruleId);
        //}
        #endregion

        #region Checking Methods
        /// <summary>
        /// Check if there are ScanChannels
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool HasScanChannels()
        {
            return GetChannel().Rows.Count > 0;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Insert(List<string[]> channels)
        {
            bool success = false;

            try
            {
                foreach (string[] channel in channels)
                {
                    if (int.Parse(channel[0]) < 0)
                    {
                        string name = channel[1];

                        ScanChannel.ScanChannelDataTable dt = new ScanChannel.ScanChannelDataTable();
                        ScanChannel.ScanChannelRow dr = dt.NewScanChannelRow();

                        dr.Name = name;

                        dt.AddScanChannelRow(dr);

                        Adapter.Update(dt);
                        int rowsAffected = dr.Id;

                        if (rowsAffected > 0)
                        {
                            AuditTrailDb auditTrailDb = new AuditTrailDb();
                            auditTrailDb.Record(TableNameEnum.ScanChannel, rowsAffected.ToString(), OperationTypeEnum.Insert);
                        }
                    }
                }

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }

        public int Insert(string name)
        {
            ScanChannel.ScanChannelDataTable dt = new ScanChannel.ScanChannelDataTable();
            ScanChannel.ScanChannelRow dr = dt.NewScanChannelRow();

            dr.Name = name;

            dt.AddScanChannelRow(dr);

            Adapter.Update(dt);
            int rowsAffected = dr.Id;

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.ScanChannel, rowsAffected.ToString(), OperationTypeEnum.Insert);
            }

            return rowsAffected;
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete ScanChannel by rule id
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }

        /// <summary>
        /// Delete all ScanChannels
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool Delete()
        {
            return Adapter.DeleteChannels() > 0;
        }
        #endregion
    }
}
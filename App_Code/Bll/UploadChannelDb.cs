using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using UploadChannelTableAdapters;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public class UploadChannelDb
    {
        private UploadChannelTableAdapter _UploadChannelAdapter = null;

        protected UploadChannelTableAdapter Adapter
        {
            get
            {
                if (_UploadChannelAdapter == null)
                    _UploadChannelAdapter = new UploadChannelTableAdapter();

                return _UploadChannelAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get UploadChannels
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UploadChannel.UploadChannelDataTable GetChannel()
        {
            return Adapter.GetChannels();
        }

        /// <summary>
        /// Get Categorisation Rule keyword by rule id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public UploadChannel.UploadChannelDataTable GetUploadChannel(int ruleId)
        //{
        //    return Adapter.GetDataByRuleId(ruleId);
        //}
        #endregion

        #region Checking Methods
        /// <summary>
        /// Check if there are UploadChannels
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool HasUploadChannels()
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

                        UploadChannel.UploadChannelDataTable dt = new UploadChannel.UploadChannelDataTable();
                        UploadChannel.UploadChannelRow dr = dt.NewUploadChannelRow();

                        dr.Name = name;

                        dt.AddUploadChannelRow(dr);

                        Adapter.Update(dt);
                        int rowsAffected = dr.Id;

                        if (rowsAffected > 0)
                        {
                            AuditTrailDb auditTrailDb = new AuditTrailDb();
                            auditTrailDb.Record(TableNameEnum.UploadChannel, rowsAffected.ToString(), OperationTypeEnum.Insert);
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
            UploadChannel.UploadChannelDataTable dt = new UploadChannel.UploadChannelDataTable();
            UploadChannel.UploadChannelRow dr = dt.NewUploadChannelRow();

            dr.Name = name;

            dt.AddUploadChannelRow(dr);

            Adapter.Update(dt);
            int rowsAffected = dr.Id;

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.UploadChannel, rowsAffected.ToString(), OperationTypeEnum.Insert);
            }

            return rowsAffected;
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete UploadChannel by rule id
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }

        /// <summary>
        /// Delete all UploadChannels
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
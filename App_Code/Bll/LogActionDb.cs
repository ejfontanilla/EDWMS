using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using LogActionTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    [System.ComponentModel.DataObject]
    public class LogActionDb
    {
        private LogActionTableAdapter _LogActionAdapter = null;

        protected LogActionTableAdapter Adapter
        {
            get
            {
                if (_LogActionAdapter == null)
                    _LogActionAdapter = new LogActionTableAdapter();

                return _LogActionAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all LogAction
        /// </summary>
        /// <returns></returns>
        public LogAction.LogActionDataTable GetLogAction()
        {
            LogAction.LogActionDataTable dataTable = Adapter.GetLogAction();
            return dataTable;
        }

        /// <summary>
        /// Get LogAction by TypeId
        /// </summary>
        /// <param name="TypeId"></param>
        /// <returns></returns>
        public LogAction.LogActionDataTable GetLogActionByTypeId(int typeId)
        {
            return Adapter.GetLogActionByTypeId(typeId);
        }

        /// <summary>
        /// Get LogAction By SetId
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public DataTable GetLogActionBySetID(int setId)
        {
            return LogActionDs.GetLogActionBySetID(setId);
        }

        /// <summary>
        /// Get logaction by App Id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public DataTable GetLogActionByAppID(int appId)
        {
            return LogActionDs.GetLogActionByAppID(appId);
        }

        /// <summary>
        /// Get LogAction By DocId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public DataTable GetLogActionByDocID(int docId)
        {
            return LogActionDs.GetLogActionByDocID(docId);
        }

        /// <summary>
        /// Get logAction by DocId for completeness
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public DataTable GetLogActionByDocIDForCompleteness(int docId)
        {
            return LogActionDs.GetLogActionByDocIDForCompleteness(docId);
        }

        #endregion

        #region Insert Method

        /// <summary>
        /// Insert LogAction
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="action"></param>
        /// <param name="type"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public int Insert(Guid userId, LogActionEnum action, string actionReplaceValue1, string actionReplaceValue2, string actionReplaceValue3, 
            string actionReplaceValue4, LogTypeEnum logType, int typeId)
        {
            LogAction.LogActionDataTable logAction = new LogAction.LogActionDataTable();
            LogAction.LogActionRow r = logAction.NewLogActionRow();

            string act = action.ToString();

            //replace SEMICOLON            
            act = act.Replace("SEMICOLON", ";");
            act = act.Replace("COLON", ":");
            act = act.Replace("EQUALSSIGN", "=");
            act = act.Replace("PERIOD", ".");

            if (!string.IsNullOrEmpty(actionReplaceValue1))
                act = act.Replace(LogActionEnum.REPLACE1.ToString(), actionReplaceValue1);

            if (!string.IsNullOrEmpty(actionReplaceValue2))
                act = act.Replace(LogActionEnum.REPLACE2.ToString(), actionReplaceValue2);

            if (!string.IsNullOrEmpty(actionReplaceValue3))
                act = act.Replace(LogActionEnum.REPLACE3.ToString(), actionReplaceValue3);

            if (!string.IsNullOrEmpty(actionReplaceValue4))
                act = act.Replace(LogActionEnum.REPLACE4.ToString(), actionReplaceValue4);

            r.UserId = userId;
            r.Action = act.Replace('_',' ');
            r.DocType = logType.ToString();
            r.TypeId = typeId;
            r.LogDate = DateTime.Now;

            logAction.AddLogActionRow(r);
            Adapter.Update(logAction);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.LogAction, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

        #region Added By Edward 24/02/2014 Add Icon and Action Log
        public static DataTable GetLogActionIncomeExtraction(int docAppId)
        {
            return LogActionDs.GetLogActionIncomeExtraction(docAppId);
        }
        #endregion


        #region Added By Edward to Fix Audit Trail Filter Tables on 2015/06/29
        public DataTable GetSetAction()
        {
            return LogActionDs.GetSetAction();
        }
        #endregion

        #endregion
    }
}
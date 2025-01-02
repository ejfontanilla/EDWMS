using System;
using System.Collections.Generic;
using System.Web;
using Dwms.Dal;
using System.Data;
using System.Web;
using System.Web.Security;
using ViewLogTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class ViewLogDb
    {
        private ViewLogTableAdapter _Adapter = null;

        protected ViewLogTableAdapter Adapter
        {
            get
            {
                if (_Adapter == null)
                    _Adapter = new ViewLogTableAdapter();

                return _Adapter;
            }
        }

        public ViewLog.ViewLogDataTable GetViewLogByIncomeId(int incomeId)
        {
            return Adapter.GetViewLogByIncomeId(incomeId);
        }

        public ViewLog.ViewLogDataTable GetViewLogByDocAppIdAndNric(int docAppId, string nric)
        {
            return Adapter.GetViewLogByDocAppIdAndNric(nric, docAppId);
        }

        public int Insert(Guid userId, int appDocRefId, int incomeId)
        {
            ViewLog.ViewLogDataTable dt = new ViewLog.ViewLogDataTable();
            ViewLog.ViewLogRow r = dt.NewViewLogRow();
            r.UserID = userId;
            r.AppDocRefId = appDocRefId;
            r.IncomeId = incomeId;
            dt.AddViewLogRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }        
    }
}
using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using IncomeTemplateTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for IncomeTemplateDb
    /// </summary>
    public class IncomeTemplateDb
    {
        private IncomeTemplateTableAdapter _Adapter = null;

        protected IncomeTemplateTableAdapter Adapter
        {
            get
            {
                if (_Adapter == null)
                    _Adapter = new IncomeTemplateTableAdapter();

                return _Adapter;
            }
        }

        public IncomeTemplate.IncomeTemplateDataTable GetIncomeTemplates()
        {
            return Adapter.GetIncomeTemplates();
        }

        public IncomeTemplate.IncomeTemplateDataTable GetIncomeTemplateByName(string name)
        {
            return Adapter.GetIncomeTemplateByName(name);
        }

        public int Insert(string name, Guid currentUserId)
        {
            IncomeTemplate.IncomeTemplateDataTable dt = new IncomeTemplate.IncomeTemplateDataTable();
            IncomeTemplate.IncomeTemplateRow row = dt.NewIncomeTemplateRow();

            row.Name = name;
            row.CanDeleteItems = false;
            row.DateEntered = DateTime.Now;
            row.EnteredBy = currentUserId;

            dt.AddIncomeTemplateRow(row);

            Adapter.Update(dt);

            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.IncomeTemplate, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }
    }


        
}
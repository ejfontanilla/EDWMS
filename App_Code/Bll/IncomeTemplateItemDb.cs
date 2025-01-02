using System;
using System.Configuration;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using IncomeTemplateItemTableAdapters;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for IncomeTemplateItemDb
    /// </summary>
    public class IncomeTemplateItemDb
    {
        static string connString =
                    ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private IncomeTemplateItemsTableAdapter _Adapter = null;

        protected IncomeTemplateItemsTableAdapter Adapter
        {
            get
            {
                if (_Adapter == null)
                    _Adapter = new IncomeTemplateItemsTableAdapter();

                return _Adapter;
            }
        }

        public IncomeTemplateItem.IncomeTemplateItemsDataTable GetIncomeTemplateItems()
        {
            return Adapter.GetIncomeTemplateItem();
        }

        public IncomeTemplateItem.IncomeTemplateItemsDataTable GetItemsById(int id)
        {
            return Adapter.GetItemsById(id);
        }

        public IncomeTemplateItem.IncomeTemplateItemsDataTable GetItemsByIncomeTemplateId(int id)
        {
            return Adapter.GetItemsByIncomeTemplateId(id);
        }

        public IncomeTemplateItem.IncomeTemplateItemsDataTable GetItemsByIncomeTemplateName(string name)
        {
            return Adapter.GetItemsByIncomeTemplateName(name);
        }

        public bool UpdateOrder(int id, int itemOrder)
        {            
            IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItems = Adapter.GetItemsById(id);

            if (incomeTemplateItems.Count == 0)
                return false;

            IncomeTemplateItem.IncomeTemplateItemsRow incomeTemplateItemRow = incomeTemplateItems[0];
            
            incomeTemplateItemRow.ItemOrder = itemOrder;

            int rowsAffected = Adapter.Update(incomeTemplateItems);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();                
                auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        public bool Update(int id, string name, bool GrossIncome, bool AHGIncome, bool CAIncome, bool Allowance, bool Overtime)
        {
            IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItems = Adapter.GetItemsById(id);

            if (incomeTemplateItems.Count == 0)
                return false;

            IncomeTemplateItem.IncomeTemplateItemsRow incomeTemplateItemRow = incomeTemplateItems[0];

            incomeTemplateItemRow.ItemName = name;
            incomeTemplateItemRow.GrossIncome = GrossIncome;
            incomeTemplateItemRow.AHGIncome = AHGIncome;
            incomeTemplateItemRow.Allowance = Allowance;
            incomeTemplateItemRow.CAIncome = CAIncome;
            incomeTemplateItemRow.Overtime = Overtime;

            int rowsAffected = Adapter.Update(incomeTemplateItems);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        public int Insert(int incomeTemplateId, string name, int itemOrder)
        {
            IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItem = new IncomeTemplateItem.IncomeTemplateItemsDataTable();
            IncomeTemplateItem.IncomeTemplateItemsRow incomeTemplateItemRow = incomeTemplateItem.NewIncomeTemplateItemsRow();

            incomeTemplateItemRow.ItemName = name;
            incomeTemplateItemRow.IncomeTemplateId = incomeTemplateId;
            incomeTemplateItemRow.ItemOrder = itemOrder;


            incomeTemplateItem.AddIncomeTemplateItemsRow(incomeTemplateItemRow);

            Adapter.Update(incomeTemplateItem);

            int id = incomeTemplateItemRow.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

        public int Insert(int incomeTemplateId, string name, int itemOrder, bool GrossIncome, bool AHGIncome, bool CAIncome, bool Allowance, bool Overtime)
        {
            IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItem = new IncomeTemplateItem.IncomeTemplateItemsDataTable();
            IncomeTemplateItem.IncomeTemplateItemsRow incomeTemplateItemRow = incomeTemplateItem.NewIncomeTemplateItemsRow();

            incomeTemplateItemRow.ItemName = name;
            incomeTemplateItemRow.IncomeTemplateId = incomeTemplateId;
            incomeTemplateItemRow.ItemOrder = itemOrder;
            incomeTemplateItemRow.GrossIncome = GrossIncome;
            incomeTemplateItemRow.AHGIncome = AHGIncome;
            incomeTemplateItemRow.CAIncome = CAIncome;
            incomeTemplateItemRow.Allowance = Allowance;
            incomeTemplateItemRow.Overtime = Overtime;

            incomeTemplateItem.AddIncomeTemplateItemsRow(incomeTemplateItemRow);

            Adapter.Update(incomeTemplateItem);

            int id = incomeTemplateItemRow.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;            

            IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItems = Adapter.GetItemsById(id);

            if (incomeTemplateItems.Count > 0)
            {
                IncomeTemplateItem.IncomeTemplateItemsRow row = incomeTemplateItems[0];
                Id = row.Id;
            }

            Guid? operationId = auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected == 0)
            {
                auditTrailDb.Record(TableNameEnum.IncomeTemplateItems, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }
    }
}
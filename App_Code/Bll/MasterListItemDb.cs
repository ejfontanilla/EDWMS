using System;
using System.Configuration;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using MasterListItemTableAdapters;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class MasterListItemDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private MasterListItemTableAdapter _MasterListItemTableAdapter = null;

        protected MasterListItemTableAdapter Adapter
        {
            get
            {
                if (_MasterListItemTableAdapter == null)
                    _MasterListItemTableAdapter = new MasterListItemTableAdapter();

                return _MasterListItemTableAdapter;
            }
        }


        #region Retrieve Methods

        /// <summary>
        /// Get All Master List
        /// </summary>
        /// <returns></returns>
        public MasterListItem.MasterListItemDataTable GetMasterListItem()
        {
            return Adapter.GetMasterListItem();
        }

        /// <summary>
        /// Get MasterListItem By MasterlistId
        /// </summary>
        /// <returns></returns>
        public MasterListItem.MasterListItemDataTable GetMasterListItemByMasterListId(int id)
        {
            return Adapter.GetMasterListItemByMasterListId(id);
        }

        public MasterListItem.MasterListItemDataTable GetMasterListItemById(int id)
        {
            return Adapter.GetMasterListItemById(id);
        }

        public MasterListItem.MasterListItemDataTable GetMasterListItemByMasterListName(string name)
        {
            return Adapter.GetMasterListItemByMasterListName(name);
        }

        /// <summary>
        /// Gets all the unique items from scan channel and upload channel)
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllChannelMasterListItem()
        {
            return MasterListItemDs.GetAllChannelMasterListItem();
        }

        #endregion

        #region Insert Methods

        /// <summary>
        /// insert MasterListItem record
        /// </summary>
        /// <param name="masterListId"></param>
        /// <param name="name"></param>
        /// <param name="itemOrder"></param>
        /// <returns></returns>
        public int Insert(int masterListId, string name, int itemOrder)
        {
            MasterListItem.MasterListItemDataTable masterListItem = new MasterListItem.MasterListItemDataTable();
            MasterListItem.MasterListItemRow masterListItemRow = masterListItem.NewMasterListItemRow();

            masterListItemRow.Name = name;
            masterListItemRow.MasterListId = masterListId;
            masterListItemRow.ItemOrder = itemOrder;

            masterListItem.AddMasterListItemRow(masterListItemRow);

            Adapter.Update(masterListItem);

            int id = masterListItemRow.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.MasterListItem, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// update MasterListItem rcord
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemOrder"></param>
        /// <returns></returns>
        public bool UpdateOrder(int id, int itemOrder)
        {
            MasterListItem.MasterListItemDataTable masterListItems = Adapter.GetMasterListItemById(id);

            if (masterListItems.Count == 0)
                return false;

            MasterListItem.MasterListItemRow masterListItemRow = masterListItems[0];

            masterListItemRow.ItemOrder = itemOrder ;

            int rowsAffected = Adapter.Update(masterListItems);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.MasterListItem, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete MasterListItem record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();

            int rowsAffected = 0;
            int Id = 0;

            MasterListItem.MasterListItemDataTable masterListItems = Adapter.GetMasterListItemById(id);

            if (masterListItems.Count > 0)
            {
                MasterListItem.MasterListItemRow row = masterListItems[0];
                Id = row.Id;
            }

            Guid? operationId = auditTrailDb.Record(TableNameEnum.MasterListItem, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected == 0)
            {
                auditTrailDb.Record(TableNameEnum.MasterList, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }
        #endregion
    }
}
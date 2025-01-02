using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using MasterListTableAdapters;
using System.Data;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class MasterListDb
    {
        private MasterListTableAdapter _MasterListTableAdapter = null;

        protected MasterListTableAdapter Adapter
        {
            get
            {
                if (_MasterListTableAdapter == null)
                    _MasterListTableAdapter = new MasterListTableAdapter();

                return _MasterListTableAdapter;
            }
        }


        #region Retrieve Methods

        /// <summary>
        /// Get All Master List
        /// </summary>
        /// <returns></returns>
        public MasterList.MasterListDataTable GetMasterList()
        {
            return Adapter.GetMasterList();
        }

        /// <summary>
        /// Get Editable MasterList
        /// </summary>
        /// <returns></returns>
        public MasterList.MasterListDataTable GetEditableMasterList()
        {
            return Adapter.GetEditableMasterList();
        }

        /// <summary>
        /// Get MasterList
        /// </summary>
        /// <returns></returns>
        public MasterList.MasterListDataTable GetMasterListNyName(MasterListEnum item)
        {
            return Adapter.GetDataByName(item.ToString().Replace("_", " "));
        }
    
        #endregion

        #region Insert Methods
        
        #endregion

        #region Update Methods



        #endregion

        #region Delete

        #endregion
    }
}
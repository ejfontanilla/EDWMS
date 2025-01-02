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
using DocFolderTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    [System.ComponentModel.DataObject]
    public class DocFolderDb
    {
        private DocFolderTableAdapter _DocFolderAdapter = null;

        protected DocFolderTableAdapter Adapter
        {
            get
            {
                if (_DocFolderAdapter == null)
                    _DocFolderAdapter = new DocFolderTableAdapter();

                return _DocFolderAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all DocFolder
        /// </summary>
        /// <returns></returns>
        public DocFolder.DocFolderDataTable GetDocFolder()
        {
            DocFolder.DocFolderDataTable dataTable = Adapter.GetDocFolder();
            return dataTable;
        }

        public DocFolder.DocFolderDataTable GetDocFolderByCode(string code)
        {
            DocFolder.DocFolderDataTable dataTable = Adapter.GetDocFolderByCode(code);
            return dataTable;
        }

        #endregion

    }
}
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using CategorisationRuleTableAdapters;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public class CategorisationRuleDb
    {
        private CategorisationRuleTableAdapter _CategorisationRuleAdapter = null;

        protected CategorisationRuleTableAdapter Adapter
        {
            get
            {
                if (_CategorisationRuleAdapter == null)
                    _CategorisationRuleAdapter = new CategorisationRuleTableAdapter();

                return _CategorisationRuleAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get Categorisation Rule
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategorisationRule.CategorisationRuleDataTable GetCategorisationRule()
        {
            return Adapter.GetData();
        }

        public CategorisationRule.CategorisationRuleDataTable GetCategorisationRule(int id)
        {
            return Adapter.GetDataById(id);
        }

        /// <summary>
        /// Get Categorisation Rule by code
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategorisationRule.CategorisationRuleDataTable GetCategorisationRule(string code)
        {
            return Adapter.GetDataByDocTypeCode(code);
        }

        /// <summary>
        /// Get the categoriation rule row of the document type
        /// </summary>
        /// <param name="docType"></param>
        /// <returns></returns>
        public CategorisationRule.CategorisationRuleRow GetCategorisationRulesRow(string docType)
        {
            CategorisationRule.CategorisationRuleRow dr = null;

            CategorisationRule.CategorisationRuleDataTable dt = GetCategorisationRule(docType);

            if (dt.Rows.Count > 0)
                dr = dt[0];

            return dr;
        }

        /// <summary>
        /// Convert the keywords string into string array
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public string[] ConvertKeywordsToStringArray(string keywords)
        {
            if (!String.IsNullOrEmpty(keywords))
                return keywords.Split(new string[] { CategorisationRuleOpeatorEnum.or.ToString() }, StringSplitOptions.RemoveEmptyEntries);
            else
                return new string[0];
        }
        #endregion

        #region Checking Methods
        /// <summary>
        /// Check if CategorisationRule exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CategorisationRuleExist(string code)
        {
            return GetCategorisationRule(code).Rows.Count > 0;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="code"></param>
        /// <param name="action"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public bool Insert(string docTypeCode, CategorisationRuleProcessingActionEnum action)
        {
            int rowsAffected = InsertReturnId(docTypeCode, action);          
            return (rowsAffected > 0);
        }

        /// <summary>
        /// Insert method that returns the rule Id and operation result
        /// </summary>
        /// <param name="code"></param>
        /// <param name="action"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool Insert(string docTypeCode, CategorisationRuleProcessingActionEnum action, out int ruleId)
        {
            ruleId = -1;
            ruleId = InsertReturnId(docTypeCode, action);
            return (ruleId > 0);
        }

        /// <summary>
        /// Insert the keywords, returning the Id
        /// </summary>
        /// <param name="docTypecode"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int InsertReturnId(string docTypecode, CategorisationRuleProcessingActionEnum action)
        {
            CategorisationRule.CategorisationRuleDataTable dt = new CategorisationRule.CategorisationRuleDataTable();
            CategorisationRule.CategorisationRuleRow dr = dt.NewCategorisationRuleRow();

            dr.DocTypeCode = docTypecode;
            dr.ProcessingAction = action.ToString();

            dt.AddCategorisationRuleRow(dr);

            Adapter.Update(dt);
            int rowsAffected = dr.Id;

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.CategorisationRule, docTypecode.ToString(), OperationTypeEnum.Insert);
            }

            return rowsAffected;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(string code, CategorisationRuleProcessingActionEnum action)
        {
            int rowsAffected = UpdateReturnId(code, action);
            return rowsAffected > 0;
        }

        /// <summary>
        /// Update method that returns the rule Id and operation result
        /// </summary>
        /// <param name="code"></param>
        /// <param name="action"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool Update(string code, CategorisationRuleProcessingActionEnum action, out int ruleId)
        {
            ruleId = -1;
            ruleId = UpdateReturnId(code.ToString(), action);
            return ruleId > 0;
        }

        /// <summary>
        /// Update method that returns the rule Id
        /// </summary>
        /// <param name="code"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int UpdateReturnId(string code, CategorisationRuleProcessingActionEnum action)
        {
            int ruleId = -1;

            CategorisationRule.CategorisationRuleDataTable dt = GetCategorisationRule(code);

            if (dt.Rows.Count == 0)
            {
                ruleId = InsertReturnId(code, action);
            }
            else
            {
                CategorisationRule.CategorisationRuleRow dr = dt[0];

                dr.ProcessingAction = action.ToString();
                int temp = dr.Id;

                int rowsAffected = Adapter.Update(dt);

                if (rowsAffected > 0)
                {
                    ruleId = temp;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.CategorisationRule, code, OperationTypeEnum.Update);
                }
            }

            return ruleId;
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete by doc type code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Delete(string code)
        {
            return (Adapter.DeleteByDocTypeCode(code) > 0);
        }
        #endregion
    }
}
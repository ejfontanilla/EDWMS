using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using CategorisationRuleKeywordTableAdapters;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public class CategorisationRuleKeywordDb
    {
        private CategorisationRuleKeywordTableAdapter _CategorisationRuleKeywordAdapter = null;

        protected CategorisationRuleKeywordTableAdapter Adapter
        {
            get
            {
                if (_CategorisationRuleKeywordAdapter == null)
                    _CategorisationRuleKeywordAdapter = new CategorisationRuleKeywordTableAdapter();

                return _CategorisationRuleKeywordAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get Categorisation Rule
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordDataTable GetCategorisationRuleKeyword()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get Categorisation Rule keyword by rule id
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordDataTable GetCategorisationRuleKeyword(int ruleId)
        {
            return Adapter.GetDataByRuleId(ruleId);
        }

        /// <summary>
        /// Get Categorisation Rule
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordDataTable GetCategorisationRuleKeyword(bool isContains)
        {
            return Adapter.GetDataByContainsFilter(isContains);
        }

        /// <summary>
        /// Get Categorisation Rule keyword by rule id and contain filter
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="isContainsKeyword"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordDataTable GetCategorisationRuleKeyword(int ruleId, bool isContainsKeyword)
        {
            return Adapter.GetDataByRuleIdContainsFilter(ruleId, isContainsKeyword);
        }

        /// <summary>
        /// Get the keyword for the document type
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public DataTable GetKeyword(int ruleId, bool isContainsKeyword)
        {
            DataTable resultTbl = new DataTable();
            resultTbl.Columns.Add("Text");
            resultTbl.Columns.Add("Value");

            string composedKeywordFormat = " {0} ";

            CategorisationRuleKeyword.CategorisationRuleKeywordDataTable dt = GetCategorisationRuleKeyword(ruleId, isContainsKeyword);

            for (int cnt = 0; cnt < dt.Rows.Count; cnt++)
            {
                CategorisationRuleKeyword.CategorisationRuleKeywordRow dr = dt.Rows[cnt] as CategorisationRuleKeyword.CategorisationRuleKeywordRow;
                DataRow resultRow = resultTbl.NewRow();

                string temp = string.Empty;
                string keyword = dr.Keyword;

                // Check if the keyword is a composition (ex. cat and dog and tree)
                if (keyword.Contains(Constants.KeywordSeperator))
                {
                    string[] keywordArray = keyword.Split(new string[] { Constants.KeywordSeperator }, StringSplitOptions.RemoveEmptyEntries);
                    string temp2 = string.Empty;

                    foreach (string keywordItem in keywordArray)
                    {
                        string keywordTemp = keywordItem;

                        // If the keyword is not a variable, enclose in quotes (")
                        if (!Validation.IsKeywordVariable(keywordTemp))
                            keywordTemp = String.Format("\"{0}\"", keywordTemp);

                        temp2 = (String.IsNullOrEmpty(temp2) ? keywordTemp : temp2 +
                            String.Format(composedKeywordFormat, CategorisationRuleOpeatorEnum.and.ToString()) + keywordTemp);
                    }

                    keyword = temp2;
                }
                else
                {
                    // If the keyword is not a variable, enclose in quots (")
                    if (!Validation.IsKeywordVariable(keyword))
                        keyword = String.Format("\"{0}\"", keyword);
                }

                if (cnt == (dt.Rows.Count - 1))
                    temp = keyword;
                else
                    temp = keyword + " " + CategorisationRuleOpeatorEnum.or.ToString();

                // Add the row
                resultRow["Text"] = temp;
                resultRow["Value"] = dr.Keyword;

                resultTbl.Rows.Add(resultRow);
            }

            return resultTbl;
        }

        /// <summary>
        /// Get the categorisation rule keyword rows
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordRow[] GetCategorisationRuleKeywordsRows(int ruleId)
        {
            CategorisationRuleKeyword.CategorisationRuleKeywordRow[] drs = null;

            CategorisationRuleKeyword.CategorisationRuleKeywordDataTable dt = GetCategorisationRuleKeyword(ruleId);

            drs = new CategorisationRuleKeyword.CategorisationRuleKeywordRow[dt.Rows.Count];
            int count = 0;
            foreach (CategorisationRuleKeyword.CategorisationRuleKeywordRow dr in dt.Rows)
            {
                drs[count] = dr;
                count++;
            }

            return drs;
        }

        /// <summary>
        /// Get the categorisation rule keyword rows
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public CategorisationRuleKeyword.CategorisationRuleKeywordRow[] GetCategorisationRuleKeywordsRows(int ruleId, bool isInclusive)
        {
            CategorisationRuleKeyword.CategorisationRuleKeywordRow[] drs = null;

            CategorisationRuleKeyword.CategorisationRuleKeywordDataTable dt = GetCategorisationRuleKeyword(ruleId);

            drs = new CategorisationRuleKeyword.CategorisationRuleKeywordRow[dt.Rows.Count];

            int count = 0;
            foreach (CategorisationRuleKeyword.CategorisationRuleKeywordRow dr in dt.Rows)
            {
                if (dr.ContainsFilter == isInclusive)
                {
                    drs[count] = dr;
                    count++;
                }
            }

            return drs;
        }
        #endregion

        #region Checking Methods
        /// <summary>
        /// Check if the document type has a keyword
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool HasKeywords(int ruleId, bool isContainsKeyword)
        {
            return GetCategorisationRuleKeyword(ruleId, isContainsKeyword).Rows.Count > 0;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Insert(int ruleId, List<string> keywords, bool isContainsKeywords)
        {
            bool success = false;

            // Delete the current keywords for the rule
            success = (HasKeywords(ruleId, isContainsKeywords) ? Delete(ruleId, isContainsKeywords) : true);

            if (success)
            {
                try
                {
                    foreach (string keyword in keywords)
                    {
                        CategorisationRuleKeyword.CategorisationRuleKeywordDataTable dt = new CategorisationRuleKeyword.CategorisationRuleKeywordDataTable();
                        CategorisationRuleKeyword.CategorisationRuleKeywordRow dr = dt.NewCategorisationRuleKeywordRow();

                        dr.RuleId = ruleId;
                        dr.Keyword = keyword;
                        dr.ContainsFilter = isContainsKeywords;

                        dt.AddCategorisationRuleKeywordRow(dr);

                        Adapter.Update(dt);
                        int rowsAffected = dr.Id;

                        if (rowsAffected > 0)
                        {
                            AuditTrailDb auditTrailDb = new AuditTrailDb();
                            auditTrailDb.Record(TableNameEnum.CategorisationRuleKeyword, ruleId.ToString(), OperationTypeEnum.Insert);
                        }
                    }

                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }

            }

            return success;
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Delete keywords by rule id
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public bool DeleteByRuleId(int ruleId)
        {
            return Adapter.DeleteByRuleId(ruleId) > 0;
        }

        /// <summary>
        /// Delete keywords by 'contains' or 'not contains'
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="isContainsKeyword"></param>
        /// <returns></returns>
        public bool Delete(int ruleId, bool isContainsKeyword)
        {
            return Adapter.DeleteByContainsFilter(ruleId, isContainsKeyword) > 0;
        }

        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }
        #endregion
    }
}
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using ParameterTableAdapters;

namespace Dwms.Bll
{
    public class ParameterDb
    {
        private ParameterTableAdapter _ParameterAdapter = null;

        protected ParameterTableAdapter Adapter
        {
            get
            {
                if (_ParameterAdapter == null)
                    _ParameterAdapter = new ParameterTableAdapter();

                return _ParameterAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get the parameter by name.
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <returns>Parameter table</returns>
        private Parameter.ParameterDataTable GetParameterByName(ParameterNameEnum name)
        {
            return Adapter.GetParameterByName(name.ToString());
        }

        /// <summary>
        /// Get parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetParameter(ParameterNameEnum name)
        {
            Parameter.ParameterDataTable table = Adapter.GetParameterByName(name.ToString());

            if (table.Count > 0 && !table[0].IsValueNull())
                return table[0].Value;
            else
                return null;
        }

        /// <summary>
        /// Get parameter ids
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIdByName(ParameterNameEnum name)
        {
            Parameter.ParameterDataTable table = Adapter.GetParameterByName(name.ToString());

            if (table.Count > 0)
                return table[0].Id;
            else
                return -1;
        }
        #endregion

        #region Checking Methods
        /// <summary>
        /// Check if parameter exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ParameterExist(ParameterNameEnum name)
        {
            return GetIdByName(name) > 0;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(ParameterNameEnum name, string value)
        {
            int id = GetIdByName(name);
            if (id <= 0)
            {
                Adapter.Insert(name.ToString(), value);
            }

            int rowsAffected = Adapter.UpdateParameter(value, name.ToString());

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.Parameter, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

                /// <summary>
        /// Get the minimum age setting value for external files.
        /// </summary>
        /// <returns>Minimum age value</returns>
        public int GetMinimumAgeForExternalFiles()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumAgeExternalFiles);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 10;
        }

        /// <summary>
        /// Get the maximum number of ocr pages for ocr processing.
        /// </summary>
        /// <returns>Maximum number pages</returns>
        public int GetMaximumPagesForOcr()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MaximumOcrPages);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 50;
        }

        /// <summary>
        /// Get the maximum sample document setting value.
        /// </summary>
        /// <returns>Maximum sample document</returns>
        public int GetMaximumSampleDocsLimit()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MaxSampleDocs);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 100;
        }

        /// <summary>
        /// Get the maximum number of threads for ocr processing.
        /// </summary>
        /// <returns>Maximum threads for ocr</returns>
        public int GetMaximumThreadsForOcr()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MaximumThread);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 10;
        }

        /// <summary>
        /// Get the minimum age for temporary files.
        /// </summary>
        /// <returns>Minimum age for temporary files</returns>
        public int GetMinimumAgeForTemporaryFiles()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumAgeTempFiles);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 10;
        }

        /// <summary>
        /// Get the minimum english word count.
        /// </summary>
        /// <returns>Minimum english word count</returns>
        public int GetMinimumEnglishWordCount()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumEnglishWordCount);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 1;
        }

        /// <summary>
        /// Get the minimum english word percentage.
        /// </summary>
        /// <returns>Minimum english word percentage</returns>
        public decimal GetMinimumEnglishWordPercentage()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumEnglishWordPercentage);

            if (table.Count > 0)
                return Decimal.Divide(int.Parse(table[0].Value), 100);
            else
                return 0.01m;
        }

        /// <summary>
        /// Get the minimum sample score.
        /// </summary>
        /// <returns>Minimum sample score</returns>
        public decimal GetMinimumSampleScore()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumScore);

            if (table.Count > 0)
                return decimal.Parse(table[0].Value);
            else
                return 0.001m;
        }

        /// <summary>
        /// Get the minimum word length.
        /// </summary>
        /// <returns>Minimum word length</returns>
        public int GetMinimumWordLength()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.MinimumWordLength);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 1;
        }

        /// <summary>
        /// Get the keyword check scope.
        /// </summary>
        /// <returns>Keyword check scope</returns>
        public int GetKeywordCheckScope()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.KeywordCheckScope);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 1;
        }

        /// <summary>
        /// Get the top sample pages percentage.
        /// </summary>
        /// <returns>Top sample page percentage</returns>
        public double GetTopSamplePagesPercentage()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.TopRankedSamplePages);

            if (table.Count > 0)
                return double.Parse(table[0].Value);
            else
                return 0.50;
        }

        /// <summary>
        /// Get the OCR Binarize setting.
        /// </summary>
        /// <returns>OCR binarize</returns>
        public int GetOcrBinarize()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrBinarize);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return -1;
        }

        /// <summary>
        /// Get the OCR Background Factor.
        /// </summary>
        /// <returns>OCR background factor</returns>
        public int GetOcrBackgroundFactor()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrBackgroundFactor);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 1;
        }

        /// <summary>
        /// Get the OCR Foreground factor.
        /// </summary>
        /// <returns>OCR foreground factor</returns>
        public int GetOcrForegroundFactor()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrForegroundFactor);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 1;
        }

        /// <summary>
        /// Get the OCR Quality.
        /// </summary>
        /// <returns>OCR quality</returns>
        public int GetOcrQuality()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrQuality);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 75;
        }

        /// <summary>
        /// Get OCR Morph.
        /// </summary>
        /// <returns>OCR morph.</returns>
        public string GetOcrMorph()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrMorph);

            if (table.Count > 0)
                return table[0].Value;
            else
                return "d2.2";
        }

        /// <summary>
        /// Get OCR Dot Matrix
        /// </summary>
        /// <returns>OCR dot matrix</returns>
        public bool GetOcrDotMatrix()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrDotMatrix);

            if (table.Count > 0)
                return bool.Parse(table[0].Value);
            else
                return false;
        }

        /// <summary>
        /// Get OCR Despeckle.
        /// </summary>
        /// <returns>OCR despeckle</returns>
        public int GetOcrDespeckle()
        {
            Parameter.ParameterDataTable table = GetParameterByName(ParameterNameEnum.OcrDespeckle);

            if (table.Count > 0)
                return int.Parse(table[0].Value);
            else
                return 0;
        }

        #endregion
    }
}
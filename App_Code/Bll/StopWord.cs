using System.Data;
using StopWordTableAdapters;
using System.Collections;

namespace Dwms.Bll
{
    public class StopWordDb
    {
        private StopWordTableAdapter _StopWordAdapter = null;

        protected StopWordTableAdapter Adapter
        {
            get
            {
                if (_StopWordAdapter == null)
                    _StopWordAdapter = new StopWordTableAdapter();

                return _StopWordAdapter;
            }
        }


        #region Select Methods

        /// <summary>
        /// get stopword
        /// </summary>
        /// <param name="startedWord"></param>
        /// <param name="searchedWord"></param>
        /// <returns></returns>
        public DataTable GetSearchWord(string startedWord, string searchedWord)
        {
            if (startedWord == "All") startedWord = "";
            return Adapter.GetRecordByWordStartedAndWordSearched(startedWord, searchedWord);
        }

        /*public ArrayList GetStopWords()
        {
            ArrayList result = new ArrayList();

            StopWord.StopWordDataTable dt = Adapter.GetData();

            foreach(StopWord.StopWordRow dr in dt)
            {
                result.Add(dr.Word);
            }

            return result;
        }*/

        /// <summary>
        /// Get the document sets
        /// </summary>
        /// <returns></returns>
        public StopWord.StopWordDataTable GetStopWords()
        {
            return Adapter.GetData();
        }


        public ArrayList GetStopWordsToArrayList()
        {
            ArrayList result = new ArrayList();

            StopWord.StopWordDataTable stopWordsDt = GetStopWords();

            foreach (StopWord.StopWordRow stopWord in stopWordsDt)
            {
                result.Add(stopWord.Word);
            }

            return result;
        }

        #endregion 

        #region Insert Methods
        /// <summary>
        /// Insert stop word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int Insert(string word)
        {
            int rowsAffected = 0;

            if (string.IsNullOrEmpty(word)) return 0; // exist

            StopWord.StopWordDataTable stopWord = new StopWord.StopWordDataTable();
            StopWord.StopWordRow stopWordRow = stopWord.NewStopWordRow();

            stopWordRow.Word = word.ToLower();
            stopWord.AddStopWordRow(stopWordRow);
            Adapter.Update(stopWord);
            //int id = stopWordRow.Id;

            //if (id > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.StopWord, id.ToString(), OperationTypeEnum.Insert);
            //}

            return rowsAffected;
        }
        #endregion

        #region Checking Method

        public bool IsStopWord(string word)
        {
            if (word.ToLower().Substring(0, 1).CompareTo("0") < 0) return true;
            //word.IndexOfAny(
            int i = int.Parse( Adapter.GetWordCount(word).ToString());
            return i > 0;
        }
        #endregion

        #region Delete Method
        /// <summary>
        /// Delete by word
        /// </summary>
        /// <param name="systemId"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public int DeleteByWord(string word)
        {
            int rowsAffected = 0;
            rowsAffected = Adapter.DeleteByWord(word);
            //if (rowsAffected > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.StopWord, string.Empty , OperationTypeEnum.Delete);
            //}
            return rowsAffected;
        }
        #endregion
    }
}
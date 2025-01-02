using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Runtime.InteropServices; 
using DocSetTableAdapters;
using Dwms.Web;
using Dwms.Dal;
using System.Collections;
using Telerik.Web.UI;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class DocSetDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private DocSetTableAdapter _DocSetAdapter = null;

        private vDocSetTableAdapter _vDocSetAdapter = null;

        protected DocSetTableAdapter Adapter
        {
            get
            {
                if (_DocSetAdapter == null)
                    _DocSetAdapter = new DocSetTableAdapter();

                return _DocSetAdapter;
            }
        }

        protected vDocSetTableAdapter vAdapter
        {
            get
            {
                if (_vDocSetAdapter == null)
                    _vDocSetAdapter = new vDocSetTableAdapter();

                return _vDocSetAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the document sets
        /// </summary>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetDocSet()
        {
            return Adapter.GetDocSets();
        }

        /// <summary>
        /// Retrieve the documents
        /// </summary>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetDocSetById(int id)
        {
            return Adapter.GetDocSetById(id);
        }

        public DocSet.DocSetDataTable GetBySetIdRange(int startDocSetId, int endDocSetId)
        {
            return Adapter.GetBySetIdRange(startDocSetId, endDocSetId);
        }

        /// <summary>
        /// Get DocSet by DocAppId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetDocSetByDocAppId(int docAppId)
        {
            return Adapter.GetDocSetByDocAppId(docAppId);
        }

        /// <summary>
        /// Get Earliest VerificationDateIn By DocAppId 
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public DateTime GetEarliestVerificationDateInByDocAppId(int docAppId)
        {
            DateTime? verificationDateIn = Adapter.GetEarliestVerificationDateInByDocAppId(docAppId);
            return (verificationDateIn.HasValue ? verificationDateIn.Value : DateTime.Now);
        }

        /// <summary>
        /// Retrieve the documents
        /// </summary>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetDocSetBySetNo(string setNo)
        {
            return Adapter.GetDocSetBySetNo(setNo);
        }

        /// <summary>
        /// Get the last Id number
        /// </summary>
        /// <returns></returns>
        public int GetLastIdNo()
        {
            int? id = Adapter.GetLastIdNo();
            return (id.HasValue ? id.Value : -1);
        }


        /// <summary>
        /// Get Doc App Id. returns -1 if its null
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public int GetDocAppId(int docSetId)
        {
            int id = -1;

            SetAppDb setAppDb = new SetAppDb();
            SetApp.SetAppDataTable dt = setAppDb.GetSetAppByDocSetId(docSetId);

            if (dt.Rows.Count > 0)
            {
                SetApp.SetAppRow dr = dt[0];

                id = dr.DocAppId;
            }

            return id;
        }

        /// <summary>
        /// Get the next Id number
        /// </summary>
        /// <returns></returns>
        public int GetNextIdNo()
        {
            int id = GetLastIdNo();
            
            return (id == -1 ? 1 : id);
        }

        /// <summary>
        /// Retrieve the document sets
        /// </summary>
        /// <returns></returns>
        public DocSet.vDocSetDataTable GetvDocSet()
        {
            return vAdapter.GetvData();
        }

        //check for given application if allthe sets are verified or closed
        public Boolean AreAllDocSetsVerifiedOrClosed(int docAppId)
        {
            DocSet.vDocSetDataTable vDocSet = vAdapter.GetvDocSetByDocAppId(docAppId);

            foreach (DocSet.vDocSetRow vDocSetRow in vDocSet.Rows)
            {
                if (!(vDocSetRow.Status.Trim().Equals(SetStatusEnum.Verified.ToString()) || vDocSetRow.Status.Trim().Equals(SetStatusEnum.Closed.ToString())))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieve the documents
        /// </summary>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetDocSetsByIds(int[] ids)
        {
            DocSet.DocSetDataTable result = new DocSet.DocSetDataTable();

            foreach(int id in ids)
            {
                DocSet.DocSetDataTable dt = GetDocSetById(id);

                if (dt.Rows.Count > 0)
                {
                    DocSet.DocSetRow dr = dt[0];
                    DocSet.DocSetRow resultRow = result.NewDocSetRow();

                    resultRow.ItemArray = dr.ItemArray;
                    result.Rows.Add(resultRow);
                }
            }

            return result;
        }


        /// <summary>
        /// Retrieve the documents
        /// </summary>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetMultipleDocSets(int[] ids)
        {
            DocSet.DocSetDataTable result = new DocSet.DocSetDataTable();

            foreach (int id in ids)
            {
                DocSet.DocSetDataTable dt = GetDocSetById(id);

                if (dt.Rows.Count > 0)
                {                    
                    DocSet.DocSetRow dr = dt[0];
                    DocSet.DocSetRow resultRow = result.NewDocSetRow();

                    resultRow.ItemArray = dr.ItemArray;
                    result.Rows.Add(resultRow);
                }
            }

            return result;
        }

        #region  Added By Edward 2017/11/03 To Optimize Assign might reduce OOM
        public static DataTable GetDocSetsToAssign(string ids)
        {            
            return DocSetDs.GetMultipleDocSets(ids);
        }

        public static int AssignDocSetToUser(int id, Guid userId)
        {
            MembershipUser user = Membership.GetUser();           
            Guid currentUserId = (Guid)user.ProviderUserKey;
            return DocSetDs.AssignDocSetToUser(id, userId, currentUserId);
        }
        #endregion

        #region Commented By Edward, Not being used 2016/01/26 - Delete this after a few months only in Verification/Log.aspx
        ///// <summary>
        ///// Get DocSet and respective docApp Detail
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public DataTable GetDocSetDocAppDetail(int id)
        //{
        //    return DocSetDs.GetDocSetDocAppDetail(id);
        //}
        #endregion

        /// <summary>
        /// Get docset with section details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDocSetDocSectionDetail(int id)
        {
            return DocSetDs.GetDocSetDocSectionDetail(id);
        }

        /// <summary>
        /// Get User Docset for Section Change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetDocSetByUserIdForSectionChange(Guid userId)
        {
            return DocSetDs.GetDocSetByUserIdForSectionChange(userId);
        }

        /// <summary>
        /// Get Reference Details for a given docset
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DataTable GetReferenceDetailsById(int Id)
        {
            return DocSetDs.GetReferenceDetailsById(Id);
        }
        

        /// <summary>
        /// Get channles for dropdown list
        /// </summary>
        /// <returns></returns>
        public DataTable GetChannelsForDropDown()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT(Channel) FROM DocSet ORDER BY Channel");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get distinct AcknowledgeNumber from docSet for dropdown display
        /// </summary>
        /// <returns></returns>
        public DataTable GetAcknowledgeNumberForDropDown()
        {
            return DocSetDs.GetAcknowledgeNumberForDropDown();
        }

        /// <summary>
        /// Get Latest docset by docappid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocSet.DocSetDataTable GetLatestDocSetByDocAppId(int id)
        {
            return Adapter.GetLatestDocSetByDocAppId(id);
        }

        public int GetUnverifiedSetForAppCount(int docAppId)
        {
            int? result = Adapter.CountUnverifiedSetsForApp(docAppId);

            return (result.HasValue ? result.Value : -1);
        }

        
        /// <summary>
        /// Get sets
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="department"></param>
        /// <param name="section"></param>
        /// <param name="userId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetSets(string channel, SetStatusEnum? status, string referenceNumber, int department, int section,
            Guid? ImportedByOIC, Guid? VerificationByOIC, DateTime? dateInFrom, DateTime? dateInTo, int docAppId, string nric, Boolean isUrgent, Boolean isSkipCategorization, string cmDocumentId, string acknowledgeNumber)
        {
            return DocSetDs.GetSets(channel, status, referenceNumber, department, section, ImportedByOIC, VerificationByOIC, dateInFrom, dateInTo, docAppId, nric, isUrgent, isSkipCategorization, cmDocumentId, acknowledgeNumber);
        }


        /// <summary>
        /// Get sets assigned to user
        /// </summary>
        /// <param name="source"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="currUserId"></param>
        /// <param name="status"></param>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public DataTable GetSetsAssignedToUser(string source, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, SetStatusEnum? status, int docAppId, string nric, string acknowledgeNumber)
        {
            return DocSetDs.GetSetsAssignedToUser(source, referenceNumber,  dateInFrom, dateInTo, currUserId, status, docAppId, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get All Sets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <returns></returns>
        public DataTable GetAllSets(string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            return DocSetDs.GetAllSets(channel, status, referenceNumber, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);            
        }

        public DataTable GetvDocSetsByAppId(int docAppId)
        {
            return DocSetDs.GetvDocSetsByAppId(docAppId);
        }

        /// <summary>
        /// Get All Sets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <returns></returns>
        public DataTable GetAllSetsVerificationDates(string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            return DocSetDs.GetAllSetsVerificationDates(channel, status, referenceNumber, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
        }

        public DataTable GetAllSetsByVerificationDateIn(DateTime verificationDateIn, string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            return DocSetDs.GetAllSetsByVerificationDateIn(verificationDateIn, channel, status, referenceNumber, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get pending assignments sets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <returns></returns>
        public DataTable GetPendingAssignmentSets(string channel, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            return DocSetDs.GetPendingAssignmentSets(channel, referenceNumber, dateInFrom, dateInTo, sectionId, status, docAppId, nric);
        }

        /// <summary>
        /// Get Earliest DocSet for a DocApp with verified as status with a filtered docSetId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="filterDocSetId"></param>
        /// <returns></returns>
        public DataTable GetEarliestVerifiedDocSetByDocAppId(int docAppId, int filterDocSetId)
        {
            return DocSetDs.GetEarliestVerifiedDocSetByDocAppId(docAppId, filterDocSetId);
        }

        public DataTable GetPendingAssignmentSetsVerificationDates(string channel, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            return DocSetDs.GetPendingAssignmentSetsVerificationDates(channel, referenceNumber, dateInFrom, dateInTo, sectionId, status, docAppId, nric);
        }

        public int GetLatestDocAppIdForNric(string nric)
        {
            int result = -1;

            DataTable dt = DocSetDs.GetLatestSetForNric(nric);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                result = int.Parse(dr["DocAppId"].ToString());
            }

            return result;
        }

        #region Edited By Edward to Optimize Slowness 2014.04.10
        //public bool CheckIfBeingProcessed(int id)
        //{
        //    bool result = false;

        //    DocSet.DocSetDataTable dt = GetDocSetById(id);

        //    if (dt.Rows.Count > 0)
        //    {
        //        DocSet.DocSetRow dr = dt[0];
        //        //result = dr.IsBeingProcessed || dr.Status.Equals(SetStatusEnum.Pending_Categorization.ToString());
        //        result = dr.Status.Equals(SetStatusEnum.Pending_Categorization.ToString());
        //    }

        //    return result;
        //}

        public bool CheckIfBeingProcessed(int id)
        {
            bool result = false;

            DocSetDs ds = new DocSetDs();
            DataTable dt = ds.GetStatusVerificationStaffUserIdById(id);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];                
                result = dr["Status"].ToString().Trim().Equals(SetStatusEnum.Pending_Categorization.ToString());
            }

            return result;
        }
        #endregion

        public string GetSetStatus(int id)
        {
            string result = string.Empty;

            DocSet.DocSetDataTable dt = GetDocSetById(id);

            if (dt.Rows.Count > 0)
            {
                DocSet.DocSetRow dr = dt[0];
                result = dr.Status.Trim();
            }

            return result;
        }

        public string GetSetNumber(int id)
        {
            string result = string.Empty;

            DocSet.DocSetDataTable dt = GetDocSetById(id);

            if (dt.Rows.Count > 0)
            {
                DocSet.DocSetRow dr = dt[0];
                result = dr.SetNo;
            }

            return result;
        }

        public DataTable GetPendingAssignmentReportData(DateTime dateIn)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DateIn");
            dt.Columns.Add("Count");

            int limit = int.Parse(PendingAssignmentReportCountEnum._5.ToString().Replace("_", ""));

            for (int cnt = 0; cnt < limit; cnt++)
            {
                DateTime date = dateIn.AddDays(cnt * -1);

                DataTable tempDt = DocSetDs.GetPendingAssignmentCountByDate(date);

                if (tempDt.Rows.Count > 0)
                {
                    DataRow tempDr = tempDt.Rows[0];

                    DataRow dr = dt.NewRow();
                    dr["DateIn"] = Format.FormatDateTime(date, DateTimeFormat.dd__MMM__yyyy);
                    dr["Count"] = tempDr["Count"];

                    dt.Rows.InsertAt(dr, 0);
                }
            }

            DataTable beyondTempDt = DocSetDs.GetPendingAssignmentCountBeyondLastDate(dateIn.AddDays((limit - 1) * -1));

            if (beyondTempDt.Rows.Count > 0)
            {
                DataRow tempDr = beyondTempDt.Rows[0];

                DataRow beyondDate = dt.NewRow();
                beyondDate["DateIn"] = "> " + limit.ToString() + " days";
                beyondDate["Count"] = tempDr["Count"];

                dt.Rows.InsertAt(beyondDate, 0);
            }            

            return dt;
        }

        public DocSet.vDocSetDataTable GetvPendingAssignmentByVerificationDate(DateTime verificationDate)
        {
            return vAdapter.GetvDataByVerificationDate(verificationDate);
        }

        public DataTable GetPendingAssignmentSetsByVerificationDateIn(DateTime verificationDateIn, string channel, string referenceNumber, 
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            return DocSetDs.GetPendingAssignmentSetsByVerificationDateIn(verificationDateIn, channel, referenceNumber, 
                dateInFrom, dateInTo, sectionId, status, docAppId, nric);
        }

        public DocSet.vDocSetDataTable GetvDocSetById(int id)
        {
            return vAdapter.GetvDataById(id);
        }

        public DataTable GetVerificationOfficerForSetAssignment()
        {
            return DocSetDs.GetVerificationOfficerForSetAssignment();
        }

        //Added By Edward 2014/04/29 Assignment Report in Verification Module
        public DataTable GetVerificationOICForReportDetails(Guid? verificationUserId, int docAppId, string refNo,
            SetStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            return DocSetDs.GetVerificationOICForReportDetails(verificationUserId, docAppId, refNo, status, dateInFrom, dateInTo, nric);
        }

        //Added by Edward 2014/04/30 Batch Assignment in Verification Module
        public DataTable GetPendingHleCounts(DateTime dateIn, string status, int sectionId)
        {
            DataTable result = new DataTable();
            result.Columns.Add("DateInConverted");
            result.Columns.Add("LaneACount");
            result.Columns.Add("LaneBCount");
            result.Columns.Add("LaneCCount");
            result.Columns.Add("LaneDCount");       
            result.Columns.Add("LaneECount");
            result.Columns.Add("LaneFCount");
            result.Columns.Add("LaneHCount");
            result.Columns.Add("LaneLCount");
            result.Columns.Add("LaneNCount");
            result.Columns.Add("LaneTCount");
            result.Columns.Add("LaneXCount");
            result.Columns.Add("Total");

            int limit = int.Parse(PendingAssignmentReportCountEnum._5.ToString().Replace("_", ""));

            for (int cnt = 0; cnt < limit; cnt++)
            {
                DateTime dateInTemp = dateIn.AddDays(cnt * -1);                
                DataTable temp = DocSetDs.GetPendingHleCounts(dateInTemp, status, sectionId);

                if (temp.Rows.Count > 0)
                {
                    DataRow tempRow = temp.Rows[0];

                    DataRow newRow = result.NewRow();
                    newRow["DateInConverted"] = Format.FormatDateTime(tempRow["DateInConverted"], DateTimeFormat.dd__MMM__yyyy);
                    newRow["LaneACount"] = tempRow["LaneACount"];
                    newRow["LaneBCount"] = tempRow["LaneBCount"];
                    newRow["LaneCCount"] = tempRow["LaneCCount"];
                    newRow["LaneDCount"] = tempRow["LaneDCount"];       
                    newRow["LaneECount"] = tempRow["LaneECount"];
                    newRow["LaneFCount"] = tempRow["LaneFCount"];
                    newRow["LaneHCount"] = tempRow["LaneHCount"];
                    newRow["LaneLCount"] = tempRow["LaneLCount"];
                    newRow["LaneNCount"] = tempRow["LaneNCount"];
                    newRow["LaneTCount"] = tempRow["LaneTCount"];
                    newRow["LaneXCount"] = tempRow["LaneXCount"];
                    newRow["Total"] = tempRow["Total"];

                    result.Rows.InsertAt(newRow, 0);
                }
            }
            
            DataTable beyondTempDt = DocSetDs.GetPendingHleGreaterThanLimitCounts(dateIn.AddDays((limit - 1) * -1), status, sectionId);

            if (beyondTempDt.Rows.Count > 0)
            {
                DataRow tempRow = beyondTempDt.Rows[0];

                DataRow newRow = result.NewRow();
                newRow["DateInConverted"] = "> " + limit.ToString() + " days";
                newRow["LaneACount"] = tempRow["LaneACount"];
                newRow["LaneBCount"] = tempRow["LaneBCount"];
                newRow["LaneCCount"] = tempRow["LaneCCount"];
                newRow["LaneDCount"] = tempRow["LaneDCount"];           //Added by Edward 13/1/2014 ofr Batch Assignment Panel
                newRow["LaneECount"] = tempRow["LaneECount"];
                newRow["LaneFCount"] = tempRow["LaneFCount"];
                newRow["LaneHCount"] = tempRow["LaneHCount"];
                newRow["LaneLCount"] = tempRow["LaneLCount"];
                newRow["LaneNCount"] = tempRow["LaneNCount"];
                newRow["LaneTCount"] = tempRow["LaneTCount"];
                newRow["LaneXCount"] = tempRow["LaneXCount"];
                newRow["Total"] = tempRow["Total"];

                result.Rows.InsertAt(newRow, 0);
            }
            //Commented by Edward 6/5/2014  Batch Assignment Uncomment if this is needed
            DataTable pendingTempDt = DocSetDs.GetPendingHleNoPendingDoc(status, sectionId);

            if (pendingTempDt.Rows.Count > 0)
            {
                DataRow tempRow = pendingTempDt.Rows[0];

                DataRow newRow = result.NewRow();

                newRow["DateInConverted"] = "No Pending";
                newRow["LaneACount"] = tempRow["LaneACount"];
                newRow["LaneBCount"] = tempRow["LaneBCount"];
                newRow["LaneCCount"] = tempRow["LaneCCount"];
                newRow["LaneDCount"] = tempRow["LaneDCount"];
                newRow["LaneECount"] = tempRow["LaneECount"];
                newRow["LaneFCount"] = tempRow["LaneFCount"];
                newRow["LaneHCount"] = tempRow["LaneHCount"];
                newRow["LaneLCount"] = tempRow["LaneLCount"];
                newRow["LaneNCount"] = tempRow["LaneNCount"];
                newRow["LaneTCount"] = tempRow["LaneTCount"];
                newRow["LaneXCount"] = tempRow["LaneXCount"];
                newRow["Total"] = tempRow["Total"];

                result.Rows.InsertAt(newRow, 0);
            }

            return result;
        }

        //Added BY Edward 2014/05/02    Batch Assignment
        public void UpdateTopPendingHleByLanes(Guid userId, int count, HleLanesEnum lane, int sectionId)
        {
            if (count > 0)
            {
                DocSet.DocSetDataTable docSet = GetTopPendingHleByLanes(count, SetStatusEnum.New, lane, sectionId);

                foreach (DocSet.DocSetRow docSetRow in docSet.Rows)
                {
                    AssignUserAsVerificationOfficer(docSetRow.Id, userId);
                }
            }
        }

        //Added by Edward 6/5/2014  Batch Assignment
        public DocSet.DocSetDataTable GetTopPendingHleByLanes(int top, SetStatusEnum status, HleLanesEnum lane, int sectionId)
        {
            return Adapter.GetTopPendingHleByLanes(top, status.ToString(), lane.ToString(), sectionId);
        }
        

        //Added By Edward 6/5/2014 Batch Assignment
        public Dictionary<string, int> GetPendingHleCountsByAllLanes()
        {
            DataTable lanes = EnumManager.EnumToDataTable(typeof(HleLanesEnum));

            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (DataRow lane in lanes.Rows)
            {
                int value = 0;
                string laneValue = lane["Value"].ToString();

                switch (laneValue)
                {
                    case "A":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.A);
                        break;
                    case "B":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.B);
                        break;
                    case "C":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.C);
                        break;
                    case "D":    //Added by Edward 13/1/2014 ofr Batch Assignment Panel 
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.D);
                        break;
                    case "E":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.E);
                        break;
                    case "F":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.F);
                        break;
                    case "H":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.H);
                        break;
                    case "L":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.L);
                        break;
                    case "N":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.N);
                        break;
                    case "T":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.T);
                        break;
                    case "X":
                        value += GetPendingHleCountsByLanes(SetStatusEnum.New.ToString(), HleLanesEnum.X);
                        break;
                    default:
                        break;
                }

                dic.Add(laneValue, value);
            }

            return dic;
        }

        //Added By Edward 6/5/2014  Batch Assignment
        public int GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        {
            int result = -1;

            DataTable resultDt = DocSetDs.GetPendingHleCountsByLanes(status, lane);

            if (resultDt.Rows.Count > 0)
            {
                DataRow resultDr = resultDt.Rows[0];
                result = int.Parse(resultDr["LaneCount"].ToString());
            }

            return result;
        }

        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert Set
        /// </summary>
        /// <returns></returns>
        public int Insert(string setNo, DateTime dateInFrom, SetStatusEnum? status, string block, int streetId, string level, 
            string unit, string channel, Guid importedBy, Guid? verificationStaff, 
            int departmentId, int sectionId, int docAppId)
        {
            DocSet.DocSetDataTable dt = new DocSet.DocSetDataTable();
            DocSet.DocSetRow r = dt.NewDocSetRow();

            r.SetNo = setNo;
            r.VerificationDateIn = dateInFrom;            
            if (status != null)
                r.Status = status.ToString();

            if (!string.IsNullOrEmpty(channel))
                r.Channel = channel;

            if(!String.IsNullOrEmpty(block))
                r.Block = block;
            
            if(streetId > -1)
                r.StreetId = streetId;

            if(!String.IsNullOrEmpty(level))
                r.Floor = level;

            if(!String.IsNullOrEmpty(unit))
                r.Unit = unit;

            if (verificationStaff.HasValue)
                r.VerificationStaffUserId = verificationStaff.Value;

            r.ImportedBy = importedBy;
            r.ImportedOn = dateInFrom;
            r.DepartmentId = departmentId;
            r.SectionId = sectionId;
            r.ProcessingStartDate = DateTime.Now;
            r.ProcessingEndDate = DateTime.Now;
            r.SendToCDBStatus = SendToCDBStatusEnum.NotReady.ToString();
            r.SendToCDBAttemptCount = 0;

            dt.AddDocSetRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Insert);

                // Update the set number to reflect the id of the record
                UpdateSetNumber(id);

                if (docAppId != -1 && docAppId != 0)
                {
                    // Create SetApp records
                    SetAppDb setAppDb = new SetAppDb();
                    int setAppId = setAppDb.Insert(id, docAppId);

                    // if the reference number is already checked, update the status to Completeness_In_Progress
                    DocAppDb docAppDb = new DocAppDb();
                    docAppDb.UpdateDocAppStatusOnDocSetInsert(docAppId);
                }
            }

            return id;
        }

        /// <summary>
        /// Insert from old. used in route funtion.
        /// </summary>
        /// <param name="oldSetId"></param>
        /// <param name="remark"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public int InsertFromOld(int oldSetId, string remark, int sectionId)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable oldDocSet = docSetDb.GetDocSetById(oldSetId);
            DocSet.DocSetRow oldRow = oldDocSet[0];

            DocSet.DocSetDataTable dt = new DocSet.DocSetDataTable();
            DocSet.DocSetRow r = dt.NewDocSetRow();

            //if (!oldRow.IsDocAppIdNull())
            //    r.DocAppId = oldRow.DocAppId;

            r.SetNo = oldRow.SetNo;
            r.VerificationDateIn = oldRow.VerificationDateIn;
            r.Status = SetStatusEnum.New.ToString();

            if (!oldRow.IsBlockNull())
                r.Block = oldRow.Block;

            if (!oldRow.IsStreetIdNull())
                r.StreetId = oldRow.StreetId;

            if (!oldRow.IsFloorNull())
                r.Floor = oldRow.Floor;

            if (!oldRow.IsUnitNull())
                r.Unit = oldRow.Unit;

            if (!oldRow.IsChannelNull())
                r.Channel = oldRow.Channel;

            r.Remark = remark;

            r.ImportedBy = oldRow.ImportedBy;
            r.ImportedOn = oldRow.ImportedOn;
            r.SectionId = sectionId;
            r.ProcessingStartDate = DateTime.Now;
            r.ProcessingEndDate = DateTime.Now;
            r.SendToCDBStatus = SendToCDBStatusEnum.NotReady.ToString();
            r.SendToCDBAttemptCount = 0;

            //get department id for the new section.
            SectionDb sectionDb = new SectionDb();
            DataTable sectionTable = sectionDb.GetSectionDepartmentDetail(sectionId);

            r.DepartmentId = int.Parse(sectionTable.Rows[0]["DepartmentId"].ToString());

            dt.AddDocSetRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Insert);

                // Update the set number to reflect the id of the record
                UpdateSetNumber(id);
            }

            //update remark for old set
            UpdateRemark(oldSetId, remark);

            return id;
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Update section with logAction. used in route 
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="remark"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public bool UpdateSectionForRoute(int setId, string remark, int sectionId)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSet = docSetDb.GetDocSetById(setId);

            if (docSet.Count == 0)
                return false;

            DocSet.DocSetRow r = docSet[0];

            r.SectionId = sectionId;
            r.Remark = remark;
            r.Status = SetStatusEnum.New.ToString();

            //get department id for the new section.
            SectionDb sectionDb = new SectionDb();
            DataTable sectionTable = sectionDb.GetSectionDepartmentDetail(sectionId);

            r.DepartmentId = int.Parse(sectionTable.Rows[0]["DepartmentId"].ToString());

            int rowsAffected = Adapter.Update(docSet);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, setId.ToString(), OperationTypeEnum.Insert);

                //log the action.
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                Guid userId = (Guid)user.ProviderUserKey;
                LogActionDb logActionDb = new LogActionDb();
                logActionDb.Insert(userId, LogActionEnum.Route_set, string.Empty, string.Empty, string.Empty, string.Empty, LogTypeEnum.S, setId);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update the Set number of the set
        /// </summary>
        /// <returns></returns>
        public bool UpdateSetNumber(int id)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            int temp1 = -1;
            int temp2 = -1;
            dr.SetNo = Util.FormulateSetNumber(id, out temp1, out temp2);

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// //reset SendToCdbStatus and SendToCdbAttemptCount
        /// </summary>
        /// <param name="id"></param>
        public void ResetSendToCdbFlags(int id)
        {
            //reset SendToCdbStatus and SendToCdbAttemptCount
            UpdateSendToCdbStatus(id, SendToCDBStatusEnum.NotReady);
            UpdateSendToCdbAttemptCount(id, 0);
        }

        /// <summary>
        /// update SendToCdbStatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbStatus(int id, SendToCDBStatusEnum status)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            dr.SendToCDBStatus = status.ToString();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// UpdateSendToCdbStatusUponDocModifiedUnderCompleteness
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbStatusUponDocModifiedUnderCompleteness(int id)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            if (dr.SendToCDBStatus.ToString().ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                dr.SendToCDBStatus = SendToCDBStatusEnum.ModifiedInCompleteness.ToString();
            else
                dr.SendToCDBStatus = SendToCDBStatusEnum.Ready.ToString();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// update SendToCdbAttemptCount
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateSendToCdbAttemptCount(int id, int attemptCount)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            dr.SendToCDBAttemptCount = attemptCount;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update Urgency details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expediteReason"></param>
        /// <param name="expediteRemark"></param>
        /// <param name="isUrgent"></param>
        /// <returns></returns>
        public bool UpdateUrgencyDetails(int id, string expediteReason, string expediteRemark, Boolean isUrgent)
        {
            MembershipUser user = Membership.GetUser();
            if (user == null)
                return false;

            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            //return it set is not avialable
            if (dt.Count == 0)
                return false;

            //return is the set status is not Pending_Categorization
            DocSet.DocSetRow dr = dt[0];
            if (!dr.Status.Trim().Equals(SetStatusEnum.Pending_Categorization.ToString()))
                return false;

            dr.ExpediteReason = expediteReason;
            dr.ExpediteRemark = expediteRemark;
            dr.ExpediteRequestDate = DateTime.Now;
            dr.ExpediteRequester = (Guid)user.ProviderUserKey;
            //dr.SkipCategorization = SkipCategorization;
            
            //if (SkipCategorization)
            //    dr.IsUrgent = false;
            //else
                dr.IsUrgent = isUrgent;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update VerificationStaff
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateForSectionChange(int id, Guid userId)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            if (userId == Guid.Empty)
                dr.SetVerificationStaffUserIdNull();
            else
                dr.VerificationStaffUserId = userId;

            dr.SetVerificationDateOutNull();
            dr.SetDateAssignedNull();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update Remark
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UpdateRemark(int id, string remark)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            dr.Remark = remark;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update DocSet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docAppID"></param>
        /// <param name="refType"></param>
        /// <param name="block"></param>
        /// <param name="streetId"></param>
        /// <param name="level"></param>
        /// <param name="unit"></param>
        /// <param name="scanChannel"></param>
        /// <param name="remark"></param>
        /// <param name="isIdentified"></param>
        /// <param name="uploadChannel"></param>
        /// <returns></returns>
        public bool Update(int id, int docAppID, string block, int streetId, string level, 
            string unit, string channel, string remark)
        {
            DocSet.DocSetDataTable docSet = Adapter.GetDocSetById(id);

            if (docSet.Count == 0)
                return false;

            DocSet.DocSetRow r = docSet[0];

            r.Remark = remark;

            if (!string.IsNullOrEmpty(channel))
                r.Channel = channel;

            if (!String.IsNullOrEmpty(block))
                r.Block = block;

            if (streetId > -1)
                r.StreetId = streetId;

            if (!String.IsNullOrEmpty(level))
                r.Floor = level;

            if (!String.IsNullOrEmpty(unit))
                r.Unit = unit;

            int rowsAffected = Adapter.Update(docSet);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;

        }

        /// <summary>
        /// Update Set Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="setStatus"></param>
        /// <returns></returns>
        public bool UpdateSetStatus(int id, SetStatusEnum setStatus, Boolean isLogAction, Boolean isUserSectionChange, LogActionEnum logAction)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            dr.Status = setStatus.ToString();

            if (setStatus == SetStatusEnum.Pending_Categorization)
                dr.ReadyForOcr = true;

            if (setStatus.Equals(SetStatusEnum.Verified))
                dr.VerificationDateOut = DateTime.Now;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);

                if (isLogAction)
                {
                    string username = string.Empty;
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                        return false;

                    ProfileDb profileDb = new ProfileDb();

                    if (isUserSectionChange)
                    {                        
                        DocSetDb docSetDb = new DocSetDb();
                        DocSet.DocSetDataTable docSet = docSetDb.GetDocSetById(id);
                        DocSet.DocSetRow docset = docSet[0];
                        if (!docset.IsVerificationStaffUserIdNull())
                            username = profileDb.GetUserFullName(docset.VerificationStaffUserId);
                    }

                    LogActionDb logActionDb = new LogActionDb();
                    Guid userId = (Guid)user.ProviderUserKey;

                    if (logAction == LogActionEnum.REPLACE1_Recatogorized_the_set)
                    {
                        username = profileDb.GetUserFullName(userId);
                        isUserSectionChange = true;
                    }

                    logActionDb.Insert(userId, logAction, isUserSectionChange ? username : string.Empty, string.Empty, string.Empty, string.Empty, LogTypeEnum.S, id);
                }
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Assign the verification officer of the set
        /// </summary>
        /// <returns></returns>
        public bool AssignUserAsVerificationOfficer(int id, Guid userId)
        {
            DocSet.DocSetDataTable dt = Adapter.GetDocSetById(id);

            if (dt.Count == 0)
                return false;

            DocSet.DocSetRow dr = dt[0];

            dr.VerificationStaffUserId = userId;
            dr.Status = SetStatusEnum.Pending_Verification.ToString();
            dr.DateAssigned = DateTime.Now;

            // If the set is being re-assigned after it has been verified, the documents
            // for this set will be taken in as sample documents once again.
            dr.ConvertedToSampleDoc = false;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);

                //log the action
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                ProfileDb profileDb = new ProfileDb();
                
                string username = profileDb.GetUserFullName(userId);

                LogActionDb logActionDb = new LogActionDb();
                Guid currentUserId = (Guid)user.ProviderUserKey;
                logActionDb.Insert(currentUserId, LogActionEnum.Assigned_set_to_REPLACE1, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.S, id);  

                // Clear the sample documents for this set if it exists
                SampleDocDb sampleDocDb = new SampleDocDb();
                sampleDocDb.DeleteByDocSetId(id);

                // Set the documents for this to not yet taken in as a 
                // sample document
                DocDb docDb = new DocDb();
                docDb.SetConvertedToSampleDocFlag(id, false);
            }

            return rowsAffected == 1;

        }

        /// <summary>
        /// Update docset status upon user section change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateFromUserSectionChange(Guid userId, int oldSectionId, int newSectionId)
        {
            //get all the docsets which are to be updated.
            DataTable docSet = GetDocSetByUserIdForSectionChange(userId);

            int rowsAffected = -1;

            if (docSet.Rows.Count == 0)
                return false;
            else
            {
                UserDb userDb = new UserDb();
                User.UserDataTable users = userDb.GetUser(userId);
                User.UserRow userRow = users[0];

                MembershipUser currentUser = Membership.GetUser();

                //for log
                SectionDb sectionDb = new SectionDb();
                Section.SectionDataTable oldSection = sectionDb.GetSectionById(oldSectionId);
                Section.SectionRow oldSectionRow = oldSection[0];

                Section.SectionDataTable newSection = sectionDb.GetSectionById(newSectionId);
                Section.SectionRow newSectionRow = newSection[0];

                foreach (DataRow row in docSet.Rows)
                {
                    //update the status
                    UpdateSetStatus(int.Parse(row["id"].ToString()), SetStatusEnum.New, true, true, LogActionEnum.Release_set_from_REPLACE1);
                    UpdateForSectionChange(int.Parse(row["id"].ToString()), Guid.Empty);

                    //log the action
                    LogActionDb logActionDb = new LogActionDb();
                    rowsAffected = logActionDb.Insert((Guid)currentUser.ProviderUserKey, LogActionEnum.REPLACE1_change_section_from_REPLACE2_to_REPLACE3, userRow.Name + " (" + userRow.UserName + ")", oldSectionRow.Name, newSectionRow.Name, string.Empty, LogTypeEnum.S, int.Parse(row["id"].ToString()));
                }
            }

            return rowsAffected == 1;
        }

        public bool SetReadyForOcr(int id, bool readyForOcr)
        {
            DocSet.DocSetDataTable docSet = Adapter.GetDocSetById(id);

            if (docSet.Count == 0)
                return false;

            DocSet.DocSetRow r = docSet[0];

            r.ReadyForOcr = readyForOcr;

            int rowsAffected = Adapter.Update(docSet);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }
        #endregion

        #region Delete Methods
        public bool Delete(int id)
        {
            int rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocSet, id.ToString(), OperationTypeEnum.Delete);
            }

            return Adapter.Delete(id) > 0;
        }
        #endregion

        #region AccessRules

        /// <summary>
        /// check if user have access to save data for a given set.
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        #region commented by Edward 2014.04.08 AllowVerificationSaveDate
        //public bool AllowVerificationSaveDate(int setId)
        //{
        //    DocSet.DocSetDataTable docSet = Adapter.GetDocSetById(setId);

        //    if (docSet.Rows.Count > 0)
        //    {
        //        DocSet.DocSetRow docRow = docSet[0];
        //        MembershipUser user = Membership.GetUser();
        //        if (user == null)
        //            return false;

        //        AppPersonalDb appPersonalDb = new AppPersonalDb();

        //        Guid userId = (Guid)user.ProviderUserKey;

        //        if (!(docRow.Status.ToLower().Trim().Equals(SetStatusEnum.Verification_In_Progress.ToString().ToLower()) || docRow.Status.ToLower().Trim().Equals(SetStatusEnum.Pending_Verification.ToString().ToLower())) || !(userId.ToString().Equals(docRow.VerificationStaffUserId.ToString())) || appPersonalDb.GetAppPersonalForHouseholdStructureByDocSetId(setId).Rows.Count==0)
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //        return false;
        //}
        #endregion
        //Added by Edward 2014.04.08 Optimize Slowness
        public bool AllowVerificationSaveDate(int setId)
        {
            DocSetDs ds = new DocSetDs();
            DataTable dt = ds.GetStatusVerificationStaffUserIdById(setId);

            if (dt.Rows.Count > 0)
            {
                DataRow docRow = dt.Rows[0];
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                AppPersonalDb appPersonalDb = new AppPersonalDb();

                Guid userId = (Guid)user.ProviderUserKey;

                if (!(docRow["Status"].ToString().ToLower().Trim().Equals(SetStatusEnum.Verification_In_Progress.ToString().ToLower()) 
                    || docRow["Status"].ToString().ToLower().Trim().Equals(SetStatusEnum.Pending_Verification.ToString().ToLower())) 
                    || !(userId.ToString().Equals(docRow["VerificationStaffUserId"].ToString())) 
                    || appPersonalDb.GetAppPersonalForHouseholdStructureByDocSetId(setId).Rows.Count == 0)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if user have access to save data for a given set excluding the household structure check.
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        #region COmmented by Edward 2014.04.08  AllowVerificationSaveDateWithoutHouseholdCheck
        //public bool AllowVerificationSaveDateWithoutHouseholdCheck(int setId)
        //{
        //    DocSet.DocSetDataTable docSet = Adapter.GetDocSetById(setId);

        //    if (docSet.Rows.Count > 0)
        //    {
        //        DocSet.DocSetRow docRow = docSet[0];
        //        MembershipUser user = Membership.GetUser();
        //        if (user == null)
        //            return false;

        //        AppPersonalDb appPersonalDb = new AppPersonalDb();

        //        Guid userId = (Guid)user.ProviderUserKey;

        //        if (!(docRow.Status.ToLower().Trim().Equals(SetStatusEnum.Verification_In_Progress.ToString().ToLower()) || docRow.Status.ToLower().Trim().Equals(SetStatusEnum.Pending_Verification.ToString().ToLower())) || !(userId.ToString().Equals(docRow.VerificationStaffUserId.ToString())))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //        return false;
        //}
        #endregion
        //Added bY Edward 2014.04.08  Optimize Slowness
        public bool AllowVerificationSaveDateWithoutHouseholdCheck(int setId)
        {            
            DocSetDs ds = new DocSetDs();
            DataTable dt = ds.GetStatusVerificationStaffUserIdById(setId);

            if (dt.Rows.Count > 0)
            {
                DataRow docRow = dt.Rows[0];
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                AppPersonalDb appPersonalDb = new AppPersonalDb();

                Guid userId = (Guid)user.ProviderUserKey;

                if (!(docRow["Status"].ToString().ToLower().Trim().Equals(SetStatusEnum.Verification_In_Progress.ToString().ToLower()) 
                    || docRow["Status"].ToString().ToLower().Trim().Equals(SetStatusEnum.Pending_Verification.ToString().ToLower())) 
                    || !(userId.ToString().Equals(docRow["VerificationStaffUserId"].ToString())))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }


        /// <summary>
        /// check if user can verify the document set
        /// </summary>
        /// <param name="setId"></param>
        public bool CanUserConfirmSet(int setId, RadTreeView radTreeView)
        {

            //0. check if current user can confirm the set.
            if (!AllowVerificationSaveDate(setId))
                return false;

            //1. There should not be any documents inside the unidentified folder.
            int docCountInUnidentifiedFolder = 0;
            RadTreeNode DocumentNode = radTreeView.FindNodeByValue(DocFolderEnum.Unidentified.ToString().ToUpper());
            docCountInUnidentifiedFolder = DocumentNode.Nodes.Count;

            if (docCountInUnidentifiedFolder > 0)
                return false;

            //2. All the documents should be verified inside the HLE (Application folder).
            Boolean isAllDocumentsVerified = false;
            DocDb docDb = new DocDb();
            isAllDocumentsVerified = docDb.IsDocumentsVerifiedForConfirmSet(setId);

            if (!isAllDocumentsVerified)
                return false;

            //3. There should not be any documents with document type as "Unidentified" inside the HLE folder.
            Boolean hasUnidentifiedFiles = false;
            DocSetDb docSetDb = new DocSetDb();
            DocAppDb docAppDb = new DocAppDb();
            hasUnidentifiedFiles = docDb.HasUnidentifiedAppDocByDocSetId(setId);

            if (hasUnidentifiedFiles)
                return false;

            return true;
        }

        #endregion

        #region Checking
        /// <summary>
        /// check if the docset is verified
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        #region Commented by Edward 2014.04.08 IsSetConfirmed(int setId)
        //public bool IsSetConfirmed(int setId)
        //{
        //    DocSet.DocSetDataTable docSets = GetDocSetById(setId);
        //    if (docSets.Rows.Count == 0)
        //        return false;
        //    else
        //    {
        //        DocSet.DocSetRow docSetRow = docSets[0];
        //        return docSetRow.Status.ToLower().Equals(SetStatusEnum.Verified.ToString().ToLower());
        //    }
        //}
        #endregion
        //Added bY Edward 2014.04.08  Optimize Slowness
        public bool IsSetConfirmed(int setId)
        {
            //DocSet.DocSetDataTable docSets = GetDocSetById(setId);
            DocSetDs ds = new DocSetDs();
            DataTable dt = ds.GetStatusVerificationStaffUserIdById(setId);
            if (dt.Rows.Count == 0)
                return false;
            else
            {
                DataRow docSetRow = dt.Rows[0];
                return docSetRow["Status"].ToString().ToLower().Equals(SetStatusEnum.Verified.ToString().ToLower());
            }
        }

        public DataTable GetStatusVerificationStaffUserIdById(int setId)
        {
            DocSetDs ds = new DocSetDs();
            return ds.GetStatusVerificationStaffUserIdById(setId);
        }
        #endregion

        #region TestingPurpose
        public DataTable GetDataBySentToCDBStatusAndDocSetStatus(SendToCDBStatusEnum sendToCDBStatus, SetStatusEnum setStatus, string exclusion)
        {
            //return Adapter.GetDataBySentToCDBStatus(status.ToString(), SetStatusEnum.Verified.ToString());
            return DocSetDs.GetDataBySentToCDBStatusAndDocSetStatusAndNotSystem(sendToCDBStatus.ToString(), setStatus.ToString(), exclusion.ToString());
        }

        public DataTable GetDocAppAndDocData(int docSetId, DocStatusEnum status, string imageCondition, string docTypesToAvoid, SendToCDBStatusEnum sendToCDBStatusToAvoid)
        {
            return DocSetDs.GetDocAppAndDocData(docSetId, status.ToString(), imageCondition, docTypesToAvoid, sendToCDBStatusToAvoid.ToString());
        }

        #endregion

        #region Added By Edward Development of Reports 2014/06/09        
        public static DataTable GetVerificationReportMonthYear(int section, int department)
        {
            return DocSetDs.GetVerificationReportMonthYear(section, department);
        }

        public static DataTable GetVerificationReport(int section, int department, int month, int year)
        {
            return DocSetDs.GetVerificationReport(section, department, month, year);
        }

        public static int GetVerificationSetCount(int section, int department, int month, int year, string channel)
        {
            return DocSetDs.GetVerificationSetCount(section, department, month, year, channel);
        }

        public static DataTable GetAgingRange(int section, int department)
        {
            return DocSetDs.GetAgingRange(section, department);
        }

        public static DataTable GetAgingRange(int section, int department, Guid? oic)
        {
            return DocSetDs.GetAgingRange(section, department, oic);
        }

        public static DataTable GetVerificationPerAging(int section, int department, int aging)
        {
            return DocSetDs.GetVerificationPerAging(section, department, aging);
        }

        public static DataTable GetVerificationOIC(int section, int department)
        {
            return DocSetDs.GetVerificationOIC(section, department);
        }

        public static int GetSetCountPerAgingPerOIC(int section, int department, Guid oic, int aging)
        {
            return DocSetDs.GetSetCountPerAgingPerOIC(section, department, oic, aging);
        }
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static DataTable GetYearMonthDayForFolderStructure(int docSetId)
        {
            return DocSetDs.GetYearMonthDayForFolderStructure(docSetId);
        }
        #endregion
    }
}
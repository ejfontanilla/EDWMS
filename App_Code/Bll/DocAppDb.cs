using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Text;
using System.Web;
using System.Web.Security;
using System.Runtime.InteropServices; 
using DocAppTableAdapters;
using System.Web.Security;
using Dwms.Dal;
using System.Collections;

namespace Dwms.Bll
{
    //******************Log Changes/Fixes*********************
    //Added/Modified By                 Date                Description 
    //Edward                            2014/05/05          Added By Edward    Batch Upload, ip and systemInfo
    //Edward                            2015/02/04          Added Risk Field in Application
    //Edward                            2015/02/06          Enhancing Leas Web Service and Batch Upload


    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class DocAppDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private DocAppTableAdapter _DocAppAdapter = null;

        protected DocAppTableAdapter Adapter
        {
            get
            {
                if (_DocAppAdapter == null)
                    _DocAppAdapter = new DocAppTableAdapter();

                return _DocAppAdapter;
            }
        }

        #region Retrieve Methods

        /// <summary>
        /// Get Oic names, search for peOic or CaOic
        /// Created by Sandeep
        /// Date: 2012-12-04
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public string GetOICEmailRecipientByRefNo(string refNo)
        {
            string result = Adapter.GetOICEmailRecipientByRefNo(refNo).ToString();
            return (string.IsNullOrEmpty(result) ? string.Empty : result);
        }

        /// <summary>
        /// Retrieve the doc app
        /// </summary>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetDocApp()
        {
            string section = "HLE";
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
            if (sectionId == 1 || sectionId == 8)
                section = "HLE";
            else if (sectionId == 3 || sectionId == 4)
                section = "SERS";
            else if (sectionId == 5 || sectionId == 9)
                section = "RESALE";
            else if (sectionId == 6)
                section = "SALE";
            return Adapter.GetDocApp(section);
        }

        #region Added By Edward 8.11.2013 For Optimizing the Search using RefNo. Only Applicable for Completeness AllApps.aspx

        public DataTable GetDocAppIDOnly()
        {
            string section = "HLE";
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
            if (sectionId == 1 || sectionId == 8)
                section = "HLE";
            else if (sectionId == 3 || sectionId == 4)
                section = "SERS";
            else if (sectionId == 5 || sectionId == 9)
                section = "RESALE";
            else if (sectionId == 6)
                section = "SALE";
            return DocAppDs.GetDocAppIDOnly(section);
        }

        public static int GetDocAppIDOnlyByRefNo(string refno)
        {
            return DocAppDs.GetDocAppIDOnlyByRefNo(refno);
        }

        #endregion

        /// <summary>
        /// Retrieve the doc app for drop down which includes unidentified
        /// </summary>
        /// <returns></returns>
        public DataTable GetDocAppForDropDown()
        {
            //return DocAppDs.GetDocAppForDropDown();
            string section = "HLE";
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
            if (sectionId == 1 || sectionId == 8)
                section = "HLE";
            else if (sectionId == 3 || sectionId == 4)
                section = "SERS";
            else if (sectionId == 5 || sectionId == 9)
                section = "RESALE";
            else if (sectionId == 6)
                section = "SALE";
            return Adapter.GetDocAppForDropDown(section);
        }

        /// <summary>
        /// GetDocApp By Address
        /// </summary>
        /// <param name="block"></param>
        /// <param name="streetName"></param>
        /// <param name="floor"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public DataTable GetDocAppForDropDownByAddress(string block, string streetName, string level, string unit)
        {
            return DocAppDs.GetDocAppForDropDownByAddress(block, streetName, level, unit);
        }

        /// <summary>
        /// Retrieve the docapp by Id
        /// </summary>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetDocAppById(int id)
        {
            return Adapter.GetDocAppById(id);
        }

        //Added By Edward 14.11.2013 for Income Extraction
        public DocApp.DocAppDataTable GetDocAppIncomeExtractionById(int id)
        {
            return Adapter.GetDocAppIncomeExtractionById(id);
        }


        /// <summary>
        /// Get DocApp by DocSetId 
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetDocAppByDocSetId(int docSetId)
        {
            
            return Adapter.GetDocAppByDocSetId(docSetId);
        }




        public DocApp.DocAppDataTable GetTopPendingHleByLanes(int top, AppStatusEnum status, HleLanesEnum lane)
        {
            return Adapter.GetTopPendingHleByLanes(top, status.ToString(), lane.ToString());
        }
        /// <summary>
        /// Added by Edward 08.06.2013
        /// </summary>
        /// <param name="top"></param>
        /// <param name="status"></param>
        /// <param name="lane"></param>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetTopPendingHleByLanesIA(int top, AssessmentStatusEnum status, HleLanesEnum lane)
        {
            return Adapter.GetTopPendingHleByLanesIA(top, status.ToString(), lane.ToString());
        }

        /// <summary>
        /// Retrieve the docApp
        /// </summary>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetDocAppsByIdsForAssignment(int[] ids)
        {
            DocApp.DocAppDataTable result = new DocApp.DocAppDataTable();

            foreach (int id in ids)
            {
                DocApp.DocAppDataTable dt = GetDocAppById(id);

                if (dt.Rows.Count > 0)
                {
                    DocApp.DocAppRow dr = dt[0];
                    DocApp.DocAppRow resultRow = result.NewDocAppRow();

                    if (!dr.Status.Equals(DocStatusEnum.Verified.ToString()))
                    {
                        resultRow.ItemArray = dr.ItemArray;
                        result.Rows.Add(resultRow);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieve the docApp
        /// </summary>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetDocAppsByReferenceNo(string refNo)
        {
            return Adapter.GetDataByRefNo(refNo);
        }

        /// <summary>
        /// Check if the reference number exists
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public bool DoesRefNoExists(string refNo)
        {
            return (Adapter.GetDataByRefNo(refNo).Rows.Count > 0);
        }

        /// <summary>
        /// Get User DocApp for Section Change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public DataTable GetDocAppByUserIdForSectionChange(Guid userId)
        {
            return DocAppDs.GetDocAppByUserIdForSectionChange(userId);
        }

        /// <summary>
        /// Get not verified files for a checked application. checks accross the verified sets only.
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public int GetNotVerifiedFilesForCheckedApplicationsCount(int docAppId)
        {
            return DocAppDs.GetNotVerifiedFilesForCheckedApplicationsCount(docAppId);
        }
        

        /// <summary>
        /// Get AllApps verification Dates
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllAppsVerificationDates(AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllAppsVerificationDates(status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, nric, acknowledgeNumber);
            
        }

        /// <summary>
        /// Get AllApps verification Dates
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllAppsVerificationDatesIA(AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllAppsVerificationDatesIA(status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, nric, acknowledgeNumber);

        }

        /// <summary>
        /// Get All Apps
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="checkHleStatus"></param>
        /// <param name="hleStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllAppsIA(AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllAppsIA(status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, checkHleStatus, hleStatus, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get AllApps By Verification DateIn
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllAppsByVerificationDateInIA(DateTime verificationDateIn, AssessmentStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllAppsByVerificationDateInIA(verificationDateIn, status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get All Apps
        /// </summary>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="checkHleStatus"></param>
        /// <param name="hleStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllApps(AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, Boolean checkHleStatus, string hleStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllApps(status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, checkHleStatus, hleStatus, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get AllApps By Verification DateIn
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="status"></param>
        /// <param name="verificationOicUserId"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="docAppId"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="nric"></param>
        /// <param name="acknowledgeNumber"></param>
        /// <returns></returns>
        public DataTable GetAllAppsByVerificationDateIn(DateTime verificationDateIn, AppStatusEnum? status, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string downloadStatus, string nric, string acknowledgeNumber)
        {
            return DocAppDs.GetAllAppsByVerificationDateIn(verificationDateIn, status, verificationOicUserId, dateInFrom, dateInTo, sectionId, docAppId, downloadStatus, nric, acknowledgeNumber);
        }

        /// <summary>
        /// Get Apps Assigned To User
        /// </summary>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="currUserId"></param>
        /// <param name="status"></param>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
                
        public DataTable GetAppsAssignedToUser(DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, AppStatusEnum? status, int docAppId, string nric)
        {
            return DocAppDs.GetAppsAssignedToUser(dateInFrom, dateInTo, currUserId, status, docAppId, nric);
        }

        public DataTable GetAppsAssignedToUserIA(DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, AssessmentStatusEnum? status, int docAppId, string nric)
        {
            return DocAppDs.GetAppsAssignedToUserIA(dateInFrom, dateInTo, currUserId, status, docAppId, nric);
        }


        /// <summary>
        /// Get pending assignments
        /// </summary>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetPendingAssignmentApps(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentApps(refID, dateInFrom, dateInTo, status, sectionId, nric);
        }

        public DataTable GetPendingAssignmentAppsIA(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, AssessmentStatusEnum? status1, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentAppsIA(refID, dateInFrom, dateInTo, status, status1, sectionId, nric);
        }

        /// <summary>
        /// Get pending assignents by verification date
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetPendingAssignmentSetsByVerificationDateIn(DateTime verificationDateIn, int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentSetsByVerificationDateIn(verificationDateIn, refID, dateInFrom, dateInTo, status, sectionId, nric);
        }
        /// <summary>
        /// Get Pending assignments by verification date for Income Extraction/Assessment. 
        /// </summary>
        /// <param name="verificationDateIn"></param>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="status"></param>
        /// <param name="sectionId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetPendingAssignmentSetsByVerificationDateInIA(DateTime verificationDateIn, int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentSetsByVerificationDateInIA(verificationDateIn, refID, dateInFrom, dateInTo, status, sectionId, nric);
        }

        /// <summary>
        /// Get Pending assigment verification dates
        /// </summary>
        /// <param name="refID"></param>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="sectionId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetPendingAssignmentAppsVerificationDates(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AppStatusEnum? status, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentAppsVerificationDates(refID, dateInFrom, dateInTo, status, sectionId, nric);
        }

        public DataTable GetPendingAssignmentAppsVerificationDatesIA(int refID, DateTime? dateInFrom,
            DateTime? dateInTo, AssessmentStatusEnum? status, AssessmentStatusEnum? status1, int sectionId, string nric)
        {
            return DocAppDs.GetPendingAssignmentAppsVerificationDatesIA(refID, dateInFrom, dateInTo, status, status1, sectionId, nric);
        }

        /// <summary>
        /// Get the reference numbers by nric
        /// </summary>
        /// <param name="nric"></param>
        /// <returns></returns>
        public DataTable GetRefNosForNric(string nric)
        {
            return DocAppDs.GetRefNosForNric(nric);
        }

        /// <summary>
        /// Get app details for Scanning/Uploading
        /// </summary>
        /// <param name="selectedDocAppId"></param>
        /// <param name="referenceNo"></param>
        /// <param name="referenceType"></param>
        /// <param name="newDocAppId"></param>
        /// <param name="caseOic"></param>
        public void GetAppDetails(int selectedDocAppId, string referenceNo, string referenceType, out int newDocAppId)
        {
            newDocAppId = 0;

            DocSetDb docSetDb = new DocSetDb();
            HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
            ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();

            if (selectedDocAppId == 0 && !string.IsNullOrEmpty(referenceNo))
            {
                DocApp.DocAppDataTable docAppDt = GetDocAppsByReferenceNo(referenceNo);

                if (docAppDt.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppDr = docAppDt[0];
                    newDocAppId = docAppDr.Id;
                }
                else
                {
                    newDocAppId = Insert(referenceNo, referenceType, null, null, AppStatusEnum.Pending_Documents, null, string.Empty, string.Empty, null, 
                        false, string.Empty,false,string.Empty,string.Empty);
                }
            }
            else
            {
                newDocAppId = selectedDocAppId;
            }
        }

        /// <summary>
        /// Get the Id and CaseOic of an application
        /// </summary>
        /// <param name="referenceNo"></param>
        /// <param name="referenceType"></param>
        /// <param name="docAppId"></param>
        /// <param name="caseOic"></param>
        private string GetIdAndCaseOic(string referenceNo)
        {
            string caseOic = string.Empty;

            DocApp.DocAppDataTable dt = GetDocAppsByReferenceNo(referenceNo);

            if (dt.Rows.Count > 0)
            {
                DocApp.DocAppRow dr = dt[0];
                caseOic = (dr.IsCaseOICNull() ? string.Empty : dr.CaseOIC);
            }

            return caseOic;
        }               

        public DataTable GetPendingHleCounts(DateTime dateIn, string status)
        {
            DataTable result = new DataTable();
            result.Columns.Add("DateInConverted");
            result.Columns.Add("LaneACount");
            result.Columns.Add("LaneBCount");
            result.Columns.Add("LaneCCount");
            result.Columns.Add("LaneDCount");       //Added by Edward 13/1/2014 ofr Batch Assignment Panel
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
                DataTable temp = DocAppDs.GetPendingHleCounts(dateInTemp, status);

                if (temp.Rows.Count > 0)
                {
                    DataRow tempRow = temp.Rows[0];

                    DataRow newRow = result.NewRow();
                    newRow["DateInConverted"] = Format.FormatDateTime(tempRow["DateInConverted"], DateTimeFormat.dd__MMM__yyyy);
                    newRow["LaneACount"] = tempRow["LaneACount"];
                    newRow["LaneBCount"] = tempRow["LaneBCount"];
                    newRow["LaneCCount"] = tempRow["LaneCCount"];
                    newRow["LaneDCount"] = tempRow["LaneDCount"];       //Added by Edward 13/1/2014 ofr Batch Assignment Panel
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

            DataTable beyondTempDt = DocAppDs.GetPendingHleGreaterThanLimitCounts(dateIn.AddDays((limit - 1) * -1), status);

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

            DataTable pendingTempDt = DocAppDs.GetPendingHleNoPendingDoc(status);

            if (pendingTempDt.Rows.Count > 0)
            {
                DataRow tempRow = pendingTempDt.Rows[0];

                DataRow newRow = result.NewRow();
                //System.Web.UI.WebControls.Image indicatorImage = new System.Web.UI.WebControls.Image();
                //indicatorImage.ImageUrl = "../../Data/Images/Icons/Thumb_up.png";
                //indicatorImage.ToolTip = Constants.HasPendingDocInApplication;
                //indicatorImage.Visible = true;

                //newRow["DateInConverted"] = indicatorImage;
                newRow["DateInConverted"] = "No Pending";
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

            return result;
        }

        /// <summary>
        /// Added by Edward 06.08.2013 for Income Assessment
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public DataTable GetPendingHleCountsIncomeAssessment(DateTime dateIn, string status)
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
                DataTable temp = DocAppDs.GetPendingHleIncomeAssessmentCounts(dateInTemp, status);

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

            DataTable beyondTempDt = DocAppDs.GetPendingHleGreaterThanLimitCountsIA(dateIn.AddDays((limit - 1) * -1), status);

            if (beyondTempDt.Rows.Count > 0)
            {
                DataRow tempRow = beyondTempDt.Rows[0];

                DataRow newRow = result.NewRow();
                newRow["DateInConverted"] = "> " + limit.ToString() + " days";
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

            DataTable pendingTempDt = DocAppDs.GetPendingHleNoPendingDocIncomeAssessment(status);

            if (pendingTempDt.Rows.Count > 0)
            {
                DataRow tempRow = pendingTempDt.Rows[0];

                DataRow newRow = result.NewRow();
                //System.Web.UI.WebControls.Image indicatorImage = new System.Web.UI.WebControls.Image();
                //indicatorImage.ImageUrl = "../../Data/Images/Icons/Thumb_up.png";
                //indicatorImage.ToolTip = Constants.HasPendingDocInApplication;
                //indicatorImage.Visible = true;

                //newRow["DateInConverted"] = indicatorImage;
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


        public int GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        {
            int result = -1;

            DataTable resultDt = DocAppDs.GetPendingHleCountsByLanes(status, lane);

            if (resultDt.Rows.Count > 0)
            {
                DataRow resultDr = resultDt.Rows[0];
                result = int.Parse(resultDr["LaneCount"].ToString());
            }

            return result;
        }

        /// <summary>
        /// Added by Edward 12.08.2013 For Income Assessment
        /// </summary>
        /// <param name="status"></param>
        /// <param name="lane"></param>
        /// <returns></returns>
        public int GetPendingHleCountsByLanesIA(string status, HleLanesEnum lane)
        {
            int result = -1;

            DataTable resultDt = DocAppDs.GetPendingHleCountsByLanesIA(status, lane);

            if (resultDt.Rows.Count > 0)
            {
                DataRow resultDr = resultDt.Rows[0];
                result = int.Parse(resultDr["LaneCount"].ToString());
            }

            return result;
        }

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
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.A);
                        break;
                    case "B":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.B);
                        break;
                    case "C":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.C);
                        break;
                    case "D":    //Added by Edward 13/1/2014 ofr Batch Assignment Panel 
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.D);
                        break;
                    case "E":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.E);
                        break;
                    case "F":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.F);
                        break;
                    case "H":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.H);
                        break;
                    case "L":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.L);
                        break;
                    case "N":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.N);
                        break;
                    case "T":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.T);
                        break;
                    case "X":
                        value += GetPendingHleCountsByLanes(AppStatusEnum.Verified.ToString(), HleLanesEnum.X);
                        break;
                    default:
                        break;
                }

                dic.Add(laneValue, value);
            }

            return dic;
        }

        /// <summary>
        /// For Income Assessment : Added by Edward 12.08.2013
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetPendingHleCountsByAllLanesIA()
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
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.A);
                        break;
                    case "B":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.B);
                        break;
                    case "C":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.C);
                        break;
                    case "D":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.D);
                        break;
                    case "E":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.E);
                        break;
                    case "F":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.F);
                        break;
                    case "H":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.H);
                        break;
                    case "L":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.L);
                        break;
                    case "N":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.N);
                        break;
                    case "T":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.T);
                        break;
                    case "X":
                        value += GetPendingHleCountsByLanesIA(AssessmentStatusEnum.Completeness_Checked.ToString(), HleLanesEnum.X);
                        break;
                    default:
                        break;
                }

                dic.Add(laneValue, value);
            }

            return dic;
        }

        public DataTable GetCompletenessOfficerForCaseAssignment()
        {
            Dictionary<string, double> laneTotals = new Dictionary<string, double>();
            return GetCompletenessOfficerForCaseAssignment(ref laneTotals);
        }

        public DataTable GetCompletenessOfficerForCaseAssignment(ref  Dictionary<string, double> laneTotals)
        {
            DataTable result = new DataTable();
            result.Columns.Add("UserId");
            result.Columns.Add("CompletenessOic");
            result.Columns.Add("LaneACount", typeof(double));
            result.Columns.Add("LaneBCount", typeof(double));
            result.Columns.Add("LaneCCount", typeof(double));
            result.Columns.Add("LaneDCount", typeof(double));   //Added by Edward 13/1/2014 ofr Batch Assignment Panel                  
            result.Columns.Add("LaneECount", typeof(double));
            result.Columns.Add("LaneFCount", typeof(double));
            result.Columns.Add("LaneHCount", typeof(double));
            result.Columns.Add("LaneLCount", typeof(double));
            result.Columns.Add("LaneNCount", typeof(double));
            result.Columns.Add("LaneTCount", typeof(double));
            result.Columns.Add("LaneXCount", typeof(double));
            result.Columns.Add("Total");

            // Get the number of current cases of the officer
            DataTable tempDt = DocAppDs.GetCompletenessCurrentCasesTotal2();

            ArrayList userIdList = new ArrayList();
            double laneATotal = 0.00;
            double laneBTotal = 0.00;
            double laneCTotal = 0.00;
            double laneDTotal = 0.00;       //Added by Edward 13/1/2014 ofr Batch Assignment Panel        
            double laneETotal = 0.00;
            double laneFTotal = 0.00;
            double laneHTotal = 0.00;
            double laneLTotal = 0.00;
            double laneNTotal = 0.00;
            double laneTTotal = 0.00;
            double laneXTotal = 0.00;
            double laneTotal = 0.00;

            foreach (DataRow row in tempDt.Rows)
            {
                string userId = row["UserId"].ToString();

                if (userIdList.Contains(userId))
                    continue;
                else
                    userIdList.Add(userId);

                double laneACount = 0.00;
                double laneBCount = 0.00;
                double laneCCount = 0.00;
                double laneDCount = 0.00;       //Added by Edward 13/1/2014 ofr Batch Assignment Panel 
                double laneECount = 0.00;
                double laneFCount = 0.00;
                double laneHCount = 0.00;
                double laneLCount = 0.00;
                double laneNCount = 0.00;
                double laneTCount = 0.00;
                double laneXCount = 0.00;
                double totalCount = 0.00;

                if (tempDt.Rows.Count > 0)
                {
                    laneACount = GetTotal(tempDt, HleLanesEnum.A, userId);
                    laneBCount = GetTotal(tempDt, HleLanesEnum.B, userId);
                    laneCCount = GetTotal(tempDt, HleLanesEnum.C, userId);
                    laneDCount = GetTotal(tempDt, HleLanesEnum.D, userId);      //Added by Edward 13/1/2014 ofr Batch Assignment Panel
                    laneECount = GetTotal(tempDt, HleLanesEnum.E, userId);
                    laneFCount = GetTotal(tempDt, HleLanesEnum.F, userId);
                    laneHCount = GetTotal(tempDt, HleLanesEnum.H, userId);
                    laneLCount = GetTotal(tempDt, HleLanesEnum.L, userId);
                    laneNCount = GetTotal(tempDt, HleLanesEnum.N, userId);
                    laneTCount = GetTotal(tempDt, HleLanesEnum.T, userId);
                    laneXCount = GetTotal(tempDt, HleLanesEnum.X, userId);
                    totalCount = laneACount + laneBCount + laneCCount + laneDCount + laneECount + laneFCount + laneHCount + laneLCount + laneNCount + laneTCount + laneXCount;
                }

                DataRow newRow = result.NewRow();

                newRow["UserId"] = row["UserId"];
                newRow["CompletenessOic"] = row["Name"];
                newRow["LaneACount"] = laneACount;
                newRow["LaneBCount"] = laneBCount;
                newRow["LaneCCount"] = laneCCount;
                newRow["LaneDCount"] = laneDCount;          //Added by Edward 13/1/2014 ofr Batch Assignment Panel      
                newRow["LaneECount"] = laneECount;
                newRow["LaneFCount"] = laneFCount;
                newRow["LaneHCount"] = laneHCount;
                newRow["LaneLCount"] = laneLCount;
                newRow["LaneNCount"] = laneNCount;
                newRow["LaneTCount"] = laneTCount;
                newRow["LaneXCount"] = laneXCount;
                newRow["Total"] = totalCount;

                result.Rows.Add(newRow);
                result.AcceptChanges();

                laneATotal += laneACount;
                laneBTotal += laneBCount;
                laneCTotal += laneCCount;
                laneDTotal += laneDCount;
                laneETotal += laneECount;
                laneFTotal += laneFCount;
                laneHTotal += laneHCount;
                laneLTotal += laneLCount;
                laneNTotal += laneNCount;
                laneTTotal += laneTCount;
                laneXTotal += laneXCount;
                laneTotal += totalCount;
            }

            laneTotals.Add(HleLanesEnum.A.ToString(), laneATotal);
            laneTotals.Add(HleLanesEnum.B.ToString(), laneBTotal);
            laneTotals.Add(HleLanesEnum.C.ToString(), laneCTotal);
            laneTotals.Add(HleLanesEnum.D.ToString(), laneDTotal);
            laneTotals.Add(HleLanesEnum.E.ToString(), laneETotal);
            laneTotals.Add(HleLanesEnum.F.ToString(), laneFTotal);
            laneTotals.Add(HleLanesEnum.H.ToString(), laneHTotal);
            laneTotals.Add(HleLanesEnum.L.ToString(), laneLTotal);
            laneTotals.Add(HleLanesEnum.N.ToString(), laneNTotal);
            laneTotals.Add(HleLanesEnum.T.ToString(), laneTTotal);
            laneTotals.Add(HleLanesEnum.X.ToString(), laneXTotal);
            laneTotals.Add("LaneTotal", laneTotal);

            return result;
        }


        /// <summary>
        /// Added By Edward 06.08.2013 for Income Assessment
        /// </summary>
        /// <param name="laneTotals"></param>
        /// <returns></returns>
        public DataTable GetAssessmentOfficerForCaseAssignment(ref  Dictionary<string, double> laneTotals)
        {
            DataTable result = new DataTable();
            result.Columns.Add("UserId");
            result.Columns.Add("ExtractionOic");
            result.Columns.Add("LaneACount", typeof(double));
            result.Columns.Add("LaneBCount", typeof(double));
            result.Columns.Add("LaneCCount", typeof(double));
            result.Columns.Add("LaneDCount", typeof(double));
            result.Columns.Add("LaneECount", typeof(double));
            result.Columns.Add("LaneFCount", typeof(double));
            result.Columns.Add("LaneHCount", typeof(double));
            result.Columns.Add("LaneLCount", typeof(double));
            result.Columns.Add("LaneNCount", typeof(double));
            result.Columns.Add("LaneTCount", typeof(double));
            result.Columns.Add("LaneXCount", typeof(double));
            result.Columns.Add("Total");

            // Get the number of current cases of the officer
            DataTable tempDt = DocAppDs.GetAssessmentCurrentCasesTotal2();

            ArrayList userIdList = new ArrayList();
            double laneATotal = 0.00;
            double laneBTotal = 0.00;
            double laneCTotal = 0.00;
            double laneDTotal = 0.00;
            double laneETotal = 0.00;
            double laneFTotal = 0.00;
            double laneHTotal = 0.00;
            double laneLTotal = 0.00;
            double laneNTotal = 0.00;
            double laneTTotal = 0.00;
            double laneXTotal = 0.00;
            double laneTotal = 0.00;

            foreach (DataRow row in tempDt.Rows)
            {
                string userId = row["UserId"].ToString();

                if (userIdList.Contains(userId))
                    continue;
                else
                    userIdList.Add(userId);

                double laneACount = 0.00;
                double laneBCount = 0.00;
                double laneCCount = 0.00;
                double laneDCount = 0.00;
                double laneECount = 0.00;
                double laneFCount = 0.00;
                double laneHCount = 0.00;
                double laneLCount = 0.00;
                double laneNCount = 0.00;
                double laneTCount = 0.00;
                double laneXCount = 0.00;
                double totalCount = 0.00;

                if (tempDt.Rows.Count > 0)
                {
                    laneACount = GetTotal(tempDt, HleLanesEnum.A, userId);
                    laneBCount = GetTotal(tempDt, HleLanesEnum.B, userId);
                    laneCCount = GetTotal(tempDt, HleLanesEnum.C, userId);
                    laneDCount = GetTotal(tempDt, HleLanesEnum.D, userId);
                    laneECount = GetTotal(tempDt, HleLanesEnum.E, userId);
                    laneFCount = GetTotal(tempDt, HleLanesEnum.F, userId);
                    laneHCount = GetTotal(tempDt, HleLanesEnum.H, userId);
                    laneLCount = GetTotal(tempDt, HleLanesEnum.L, userId);
                    laneNCount = GetTotal(tempDt, HleLanesEnum.N, userId);
                    laneTCount = GetTotal(tempDt, HleLanesEnum.T, userId);
                    laneXCount = GetTotal(tempDt, HleLanesEnum.X, userId);
                    totalCount = laneACount + laneBCount + laneCCount + laneDCount + laneECount + laneFCount + laneHCount + laneLCount + laneNCount + laneTCount + laneXCount;
                }

                DataRow newRow = result.NewRow();

                newRow["UserId"] = row["UserId"];
                newRow["ExtractionOic"] = row["Name"];
                newRow["LaneACount"] = laneACount;
                newRow["LaneBCount"] = laneBCount;
                newRow["LaneCCount"] = laneCCount;
                newRow["LaneDCount"] = laneDCount;
                newRow["LaneECount"] = laneECount;
                newRow["LaneFCount"] = laneFCount;
                newRow["LaneHCount"] = laneHCount;
                newRow["LaneLCount"] = laneLCount;
                newRow["LaneNCount"] = laneNCount;
                newRow["LaneTCount"] = laneTCount;
                newRow["LaneXCount"] = laneXCount;
                newRow["Total"] = totalCount;

                result.Rows.Add(newRow);
                result.AcceptChanges();

                laneATotal += laneACount;
                laneBTotal += laneBCount;
                laneCTotal += laneCCount;
                laneDTotal += laneDCount;
                laneETotal += laneECount;
                laneFTotal += laneFCount;
                laneHTotal += laneHCount;
                laneLTotal += laneLCount;
                laneNTotal += laneNCount;
                laneTTotal += laneTCount;
                laneXTotal += laneXCount;
                laneTotal += totalCount;
            }

            laneTotals.Add(HleLanesEnum.A.ToString(), laneATotal);
            laneTotals.Add(HleLanesEnum.B.ToString(), laneBTotal);
            laneTotals.Add(HleLanesEnum.C.ToString(), laneCTotal);
            laneTotals.Add(HleLanesEnum.D.ToString(), laneDTotal);
            laneTotals.Add(HleLanesEnum.E.ToString(), laneETotal);
            laneTotals.Add(HleLanesEnum.F.ToString(), laneFTotal);
            laneTotals.Add(HleLanesEnum.H.ToString(), laneHTotal);
            laneTotals.Add(HleLanesEnum.L.ToString(), laneLTotal);
            laneTotals.Add(HleLanesEnum.N.ToString(), laneNTotal);
            laneTotals.Add(HleLanesEnum.T.ToString(), laneTTotal);
            laneTotals.Add(HleLanesEnum.X.ToString(), laneXTotal);
            laneTotals.Add("LaneTotal", laneTotal);

            return result;
        }

        private double GetTotal(DataTable table, HleLanesEnum lane, string userId)
        {
            double result = 0.00;

            string filter = String.Format("Lane = '{0}' AND UserId='{1}'", lane.ToString(), userId);

            DataRow[] resultRow = (DataRow[])table.Select(filter);

            foreach (DataRow row in resultRow)
            {
                result += double.Parse(row["Total"].ToString());
            }

            return result;
        }

        public DataTable GetCompletenessOfficersForAssignedCasesReport(int docAppId, string refNo, Guid? completenessOic, DateTime? dateInFrom, DateTime? dateInTo)
        {
            return DocAppDs.GetCompletenessOfficersForReport(docAppId, refNo, completenessOic, dateInFrom, dateInTo);
        }

        public DataTable GetCompletenessOfficersForAssignedCasesReportDetails(Guid? completenessUserId, int docAppId, string refNo, AppStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            return DocAppDs.GetCompletenessOfficersForReportDetails(completenessUserId, docAppId, refNo, status, dateInFrom, dateInTo, nric);
        }

        public DataTable GetAssessmentOfficersForAssignedCasesReportDetailsIA(Guid? UserId, int docAppId, string refNo, AssessmentStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            return DocAppDs.GetAssessmentOfficersForReportDetailsIA(UserId, docAppId, refNo, status, dateInFrom, dateInTo, nric);
        }

        public DataTable GetCompletenessOfficers()
        {
            return DocAppDs.GetCompletenessOfficers();
        }

        #region Commented By Edward 06/02/2014 Not being used anymore
        //public DataTable GetAssessmentOfficers()
        //{
        //    return DocAppDs.GetAssessmentOfficers();
        //}
        #endregion

        #endregion

        #region Insert Methods

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2015/02/04          Added Risk Field in Application

        /// <summary>
        /// Insert Application
        /// </summary>
        /// <param name="refNo"></param>
        /// <param name="refType"></param>
        /// <param name="dateIn"></param>
        /// <param name="dateOut"></param>
        /// <param name="status"></param>
        /// <param name="staffUserId"></param>
        /// <returns></returns>
        public int Insert(string refNo, string refType, DateTime? dateIn, DateTime? dateOut, AppStatusEnum status, Guid? staffUserId, string caseOic, 
            string peOic, string CADate, bool secondCA, string risk, bool IsBatchUpload, string ip, string systemInfo)
        {
            DocApp.DocAppDataTable dt = new DocApp.DocAppDataTable();
            DocApp.DocAppRow r = dt.NewDocAppRow();

            r.RefNo = refNo.ToUpper();
            r.RefType = refType;

            if (dateIn.HasValue)
                r.DateIn = dateIn.Value;

            if (dateOut.HasValue)
                r.DateOut = dateOut.Value;

            r.Status = status.ToString();

            if (!String.IsNullOrEmpty(caseOic))
                r.CaseOIC = caseOic;

            if (!String.IsNullOrEmpty(peOic))
                r.PeOIC = peOic;

            if (staffUserId.HasValue)
                r.CompletenessStaffUserId = staffUserId.Value;

            if (!String.IsNullOrEmpty(CADate))
                r.SecondCADate = CADate;

            if (r.IsSecondCANull())
                r.SecondCA = false;
            r.SecondCA = secondCA;

            if (r.IsSecondCAFlagNull())
                r.SecondCAFlag = false;
            r.SecondCAFlag = secondCA;      //Added By Edward 23/01/2014

            r.SendToCDBStatus = SendToCDBStatusEnum.NotReady.ToString();
            r.SendToCDBAttemptCount = 0;

            if (!string.IsNullOrEmpty(risk))    //Added by Edward 2015/02/03
                r.Risk = risk;
            

            #region Added By Edward 24/02/2014 Add Icon and Action Log
            r.SentToLEASStatus = SendToLEASStatusEnum.NotReady.ToString();
            r.SentToLeasAttemptCount = 0;
            #endregion

            dt.AddDocAppRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                if (!IsBatchUpload)                    
                    auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Insert);
                else //2014/05/05 Batch Upload
                    auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Insert, ip, systemInfo);

                // Update the set number to reflect the id of the record
                UpdateSetNumber(id);
            }

            return id;
        }





        #endregion

        #region Update Methods
        /// <summary>
        /// Update the Set number of the set
        /// </summary>
        /// <returns></returns>
        public bool UpdateSetNumber(int id)
        {
            //DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            //if (dt.Count == 0)
            //    return false;

            //DocApp.DocAppRow dr = dt[0];

            //int temp1 = -1;
            //int temp2 = -1;
            //dr.SetNo = BllFunc.FormulateSetNumber(id, out temp1, out temp2);

            //int rowsAffected = Adapter.Update(dt);

            //if (rowsAffected > 0)
            //{
            //    Util.RecordAudit(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            //}

            //return rowsAffected == 1;
            return true;
        }

        /// <summary>
        /// Update CompletenessStaff
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateForSectionChange(int id, Guid userId)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            if (userId == Guid.Empty)
                dr.SetCompletenessStaffUserIdNull();
            else
                dr.CompletenessStaffUserId = userId;

            dr.SetDateOutNull();
            dr.SetDateAssignedNull();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Assign completeness officer to the application
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AssignUserAsCompletenessOfficer(int id, Guid userId)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.CompletenessStaffUserId = userId;
            dr.Status = AppStatusEnum.Pending_Completeness.ToString();
            dr.DateAssigned = DateTime.Now;

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
                logActionDb.Insert(currentUserId, LogActionEnum.Assigned_application_to_REPLACE1, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.A, id);

            }

            return rowsAffected == 1;

        }
        // June 4, 2013
        /// <summary>
        /// Assign assessmemt officer to the application for Income Assessment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AssignUserAsAssessmentOfficer(int id, Guid userId)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.AssessmentStaffUserId = userId;
            dr.AssessmentStatus = AssessmentStatusEnum.Pending_Extraction.ToString(); 
            dr.AssessmentDateAssigned = DateTime.Now;

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
                logActionDb.Insert(currentUserId, LogActionEnum.Assigned_application_to_REPLACE1, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.E, id);

            }

            return rowsAffected == 1;

        }

        #region Edward - July 11, 2013 UpdateRefStatus
        /// <summary>
        /// When user clicks Confirm, updates the Status and AssessmentStatus in DocApp
        /// </summary>
        /// <param name="id">DocAppId</param>
        /// <param name="status">Status Column</param>
        /// <param name="AssessmentStatus">AssessmentStatusColumn</param>
        /// <param name="isLogAction"></param>
        /// <param name="isUserSectionChange"></param>
        /// <param name="logAction"></param>
        /// <returns></returns>
        public bool UpdateRefStatus(int id, AppStatusEnum status, AssessmentStatusEnum AssessmentStatus, Boolean isLogAction, Boolean isUserSectionChange, LogActionEnum? logAction)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.Status = status.ToString();
            dr.AssessmentStatus = AssessmentStatus.ToString();

            if (status.Equals(AppStatusEnum.Completeness_Checked))
                dr.DateOut = DateTime.Now;
            if (status.Equals(AppStatusEnum.Closed))
                dr.DateOut = DateTime.Now;
            //Added by Edward 12.08.2013 For Income Assessment
            //Added AssessmentStatus.Equals(AssessmentStatusEnum.Verified) By Edward 04/20/2014 For Sales and Resales Changes
            if (AssessmentStatus.Equals(AssessmentStatusEnum.Completeness_Checked) || AssessmentStatus.Equals(AssessmentStatusEnum.Verified))
            {
                dr.AssessmentDateIn = DateTime.Now;
                dr.SetAssessmentDateOutNull();          //Added By Edward 14.11.2013 Set AssessmentDateOut To Null when Confirm is clicked again.
            }

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);

                if (isLogAction && logAction != null)
                {
                    string username = string.Empty;
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                        return false;

                    if (isUserSectionChange)
                    {
                        ProfileDb profileDb = new ProfileDb();
                        DocAppDb docAppDb = new DocAppDb();
                        DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(id);
                        DocApp.DocAppRow docAppRow = docApp[0];
                        if (!docAppRow.IsCompletenessStaffUserIdNull())
                            username = profileDb.GetUserFullName(docAppRow.CompletenessStaffUserId);
                        if (!docAppRow.IsAssessmentStaffUserIdNull())
                            username = profileDb.GetUserFullName(docAppRow.AssessmentStaffUserId);
                    }

                    LogActionDb logActionDb = new LogActionDb();
                    Guid userId = (Guid)user.ProviderUserKey;
                    logActionDb.Insert(userId, logAction.Value, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.A, id);
                }
            }

            return rowsAffected == 1;
        }
        #endregion


        /// <summary>
        /// Update Ref Status for Income Assessment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="isLogAction"></param>
        /// <returns></returns>
        public bool UpdateRefStatusIA(int id, AssessmentStatusEnum status, Boolean isLogAction, Boolean isUserSectionChange, LogActionEnum? logAction)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.AssessmentStatus = status.ToString();

            if (status.Equals(AssessmentStatusEnum.Extracted))
            {
                dr.AssessmentDateOut = DateTime.Now;
                //dr.SentToLEASStatus = SendToLEASStatusEnum.Ready.ToString();
                //dr.SentToLeasAttemptCount = 0;
            }
            else if (status.Equals(AssessmentStatusEnum.Closed))
                dr.AssessmentDateOut = DateTime.Now;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);

                if (isLogAction && logAction != null)
                {
                    string username = string.Empty;
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                        return false;

                    if (isUserSectionChange)
                    {
                        ProfileDb profileDb = new ProfileDb();
                        DocAppDb docAppDb = new DocAppDb();
                        DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(id);
                        DocApp.DocAppRow docAppRow = docApp[0];
                        if (!docAppRow.IsAssessmentStaffUserIdNull())
                            username = profileDb.GetUserFullName(docAppRow.AssessmentStaffUserId);
                    }

                    LogActionDb logActionDb = new LogActionDb();
                    Guid userId = (Guid)user.ProviderUserKey;
                    logActionDb.Insert(userId, logAction.Value, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.A, id);
                }
            }

            return rowsAffected == 1;
        }

        #region Commented BY Edward 12/02/2014 not used anymore delete this if you want
        //public bool UpdateSentToLeasStatusIA(int id, string status, int attempt)
        //{
        //    DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

        //    if (dt.Count == 0)
        //        return false;

        //    DocApp.DocAppRow dr = dt[0];

        //    dr.SentToLEASStatus = status;
        //    dr.SentToLeasAttemptCount = attempt;

        //    int rowsAffected = Adapter.Update(dt);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);               
        //    }

        //    return rowsAffected == 1;
        //}
        #endregion


        /// <summary>
        /// Update Ref Status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="isLogAction"></param>
        /// <returns></returns>
        public bool UpdateRefStatus(int id, AppStatusEnum status, Boolean isLogAction, Boolean isUserSectionChange, LogActionEnum? logAction)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.Status = status.ToString();

            if (status.Equals(AppStatusEnum.Completeness_Checked))
                dr.DateOut = DateTime.Now;
            if (status.Equals(AppStatusEnum.Closed))
                dr.DateOut = DateTime.Now;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);

                if (isLogAction && logAction != null)
                {
                    string username = string.Empty;
                    MembershipUser user = Membership.GetUser();
                    if (user == null)
                        return false;

                    if (isUserSectionChange)
                    {
                        ProfileDb profileDb = new ProfileDb();
                        DocAppDb docAppDb = new DocAppDb();
                        DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(id);
                        DocApp.DocAppRow docAppRow = docApp[0];
                        if (!docAppRow.IsCompletenessStaffUserIdNull())
                            username = profileDb.GetUserFullName(docAppRow.CompletenessStaffUserId);
                    }

                    LogActionDb logActionDb = new LogActionDb();
                    Guid userId = (Guid)user.ProviderUserKey;
                    logActionDb.Insert(userId, logAction.Value, username, string.Empty, string.Empty, string.Empty, LogTypeEnum.A, id);
                }
            }

            return rowsAffected == 1;
        }

        #region Commented By Edward 15.11.2013 Uncommented On 09/01/2014 
        public bool UpdateSecondCA(int id, DateTime DateIn, bool secondCAFlag)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            if (!dr.SecondCAFlag)
                dr.Status = AppStatusEnum.Verified.ToString();
            dr.SecondCAFlag = secondCAFlag;
            dr.DateIn = DateIn;
            dr.SetCompletenessStaffUserIdNull();
            //dr.SetDateAssignedNull;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }
        #endregion

        #region Added By Edward 09.01.2014 
        public bool UpdateSecondCA(int id, DateTime DateIn)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];
            
            dr.Status = AppStatusEnum.Verified.ToString();
            dr.SecondCAFlag = true;                 //Added BY Edward 23/01/2014
            dr.DateIn = DateIn;
            dr.SetCompletenessStaffUserIdNull();
            dr.SetDateAssignedNull();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        #endregion


        //Updated by Edward 15.11.2013
        #region Updated by Edward 15.11.2013
        public bool UpdateSecondCA(int id, DateTime DateIn, bool secondCAFlag, bool UpdateOIC)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            if (!dr.SecondCAFlag && !UpdateOIC)
                dr.Status = AppStatusEnum.Verified.ToString();
            //dr.SecondCAFlag = secondCAFlag;
            dr.DateIn = DateIn;
            if (UpdateOIC)
                dr.SetCompletenessStaffUserIdNull();
            //dr.SetDateAssignedNull;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }
        #endregion
        /// <summary>
        /// Update Download Status details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="downloadStatus"></param>
        /// <param name="downloadedBy"></param>
        /// <returns></returns>
        public bool UpdateDownloadDetails(int id, DownloadStatusEnum downloadStatus, Guid? downloadedBy)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            if (downloadedBy != null)
                dr.DownloadedBy = downloadedBy.Value;
            else
                dr.SetDownloadedByNull();

            dr.DownloadStatus = downloadStatus.ToString();

            if (downloadStatus.ToString().Equals(DownloadStatusEnum.Pending_Download.ToString()))
                dr.SetDownloadedOnNull();
            else
                dr.DownloadedOn = DateTime.Now;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Retrieve the documents
        /// </summary>
        /// <returns></returns>
        public DocApp.DocAppDataTable GetMultipleDocApp(int[] ids)
        {
            DocApp.DocAppDataTable result = new DocApp.DocAppDataTable();

            foreach (int id in ids)
            {
                DocApp.DocAppDataTable dt = GetDocAppById(id);

                if (dt.Rows.Count > 0)
                {
                    DocApp.DocAppRow dr = dt[0];
                    DocApp.DocAppRow resultRow = result.NewDocAppRow();

                    resultRow.ItemArray = dr.ItemArray;
                    result.Rows.Add(resultRow);
                }
            }

            return result;
        }

        
       /// <summary>
        /// Update application status for application cancellation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateRefStatusFromCancellation(int id)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.Status = AppStatusEnum.Completeness_Cancelled.ToString();
            dr.DateOut = DateTime.Now;

            //BEGIN Added by Edward 08.05.2013
            dr.AssessmentStatus = AssessmentStatusEnum.Completeness_Cancelled.ToString();
            dr.AssessmentDateIn = DateTime.Now;
            //END Added by Edward 08.05.2013

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);

                LogActionDb logActionDb = new LogActionDb();
                MembershipUser user = Membership.GetUser();
                Guid userId = (Guid)user.ProviderUserKey;
                logActionDb.Insert(userId, LogActionEnum.Confirmed_application_COLON_To_Cancel, string.Empty, string.Empty, string.Empty, string.Empty, LogTypeEnum.A, id);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update DateIn
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateIn"></param>
        /// <returns></returns>
        public bool UpdateDateIn(int id, DateTime dateIn)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.DateIn = dateIn;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }


        /// <summary>
        /// Update from application cancellaion
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CancellationOption"></param>
        /// <param name="CancellationRemark"></param>
        /// <returns></returns>
        public bool UpdateForCancellation(int id, ApplicationCancellationOptionEnum CancellationOption, string CancellationRemark)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.CancellationOption = CancellationOption.ToString();
            dr.CancellationRemark = CancellationRemark;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Reset Dateout OIC
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ResetDateOut(int id)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.SetDateOutNull();

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        public bool UpdatePendingDoc(string hleNumber)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDataByRefNo(hleNumber);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            if (leasInterfaceDb.HasPendingDoc(hleNumber))
            {
                dr.HasPendingDoc = true;
                dr.SetDocInOrderDateNull();
            }
            else
            {
                dr.HasPendingDoc = false;
                if (dr.IsDocInOrderDateNull())
                    dr.DocInOrderDate = DateTime.Now;
            }

            int rowsAffected = Adapter.Update(dt);

            //if (rowsAffected > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            //}

            return rowsAffected == 1;
        }
        #region  added by Edward 2017/11/01 To Optimize speed of Verification Listing 
        public static bool UpdateAndCheckHasPendingDoc(string hleNumber)
        {
            return DocAppDs.UpdateAndCheckHasPendingDoc(hleNumber);
        }
        #endregion

        /// <summary>
        /// Update docApp status upon user section change
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldSectionId"></param>
        /// <param name="newSectionId"></param>
        /// <returns></returns>
        public bool UpdateFromUserSectionChange(Guid userId, int oldSectionId, int newSectionId)
        {
            //get all the docsets which are to be updated.
            DataTable docApp = GetDocAppByUserIdForSectionChange(userId);

            int rowsAffected = -1;

            if (docApp.Rows.Count == 0)
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

                foreach (DataRow row in docApp.Rows)
                {
                    //update the status
                    UpdateRefStatus(int.Parse(row["id"].ToString()), AppStatusEnum.Verified, true, true, LogActionEnum.Release_application_from_REPLACE1);
                    UpdateForSectionChange(int.Parse(row["id"].ToString()), Guid.Empty);

                    //log the action
                    LogActionDb logActionDb = new LogActionDb();
                    rowsAffected = logActionDb.Insert((Guid)currentUser.ProviderUserKey, LogActionEnum.REPLACE1_change_section_from_REPLACE2_to_REPLACE3, userRow.Name + " (" + userRow.UserName + ")", oldSectionRow.Name, newSectionRow.Name, string.Empty, LogTypeEnum.A, int.Parse(row["id"].ToString()));
                }
            }

            return rowsAffected == 1;
        }

        /// <summary>
        /// Update DocApp Status on DocSet deletion. If no docset is verified, then update the docapp status to "pending_document"
        /// or else update the docapp datein to the earliest docset
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public void UpdateDocAppStatusOnDocSetDelete(int docSetId)
        {
            DocSetDb docSetDb = new DocSetDb();
            SetAppDb setAppDb = new SetAppDb();
            SetApp.SetAppDataTable setApps = setAppDb.GetSetAppByDocSetId(docSetId);

            //get all the apps for the docset
            foreach (SetApp.SetAppRow setAppRows in setApps.Rows)
            {
                int docappId = setAppRows.DocAppId;

                //check the earliest docset for the docapp with verified status
                DataTable earliestDocSet = docSetDb.GetEarliestVerifiedDocSetByDocAppId(docappId, docSetId);

                //if there are no earliest records which means no verified docsets. in this case set the docapp status to pending_documents
                if (earliestDocSet.Rows.Count == 0)
                {
                    UpdateRefStatus(docappId, AppStatusEnum.Pending_Documents, false, false, null);
                    ResetDateOut(docappId);
                    ResetSendToCdbFlags(docappId);
                }
                else
                    UpdateDateIn(docappId, DateTime.Parse(earliestDocSet.Rows[0]["VerificationDateIn"].ToString()));
            }
        }

        public void UpdateDocAppStatusOnDocSetAssign(int docSetId)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocAppDb docAppDb = new DocAppDb();
            SetAppDb setAppDb = new SetAppDb();
            SetApp.SetAppDataTable setApps = setAppDb.GetSetAppByDocSetId(docSetId);

            //get all the apps for the docset
            foreach (SetApp.SetAppRow setAppRows in setApps.Rows)
            {
                int docappId = setAppRows.DocAppId;

                //check the earliest docset for the docapp with verified status
                DataTable earliestDocSet = docSetDb.GetEarliestVerifiedDocSetByDocAppId(docappId, docSetId);

                //if there are no earliest records which means no verified docsets. in this case set the docapp status to pending_documents
                if (earliestDocSet.Rows.Count == 0)
                {
                    UpdateRefStatus(docappId, AppStatusEnum.Pending_Documents, false, false, null);
                    ResetDateOut(docappId);
                    ResetSendToCdbFlags(docappId);
                }
                else
                {
                    DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(docappId);

                    if (docApp.Rows.Count > 0)
                    {
                        DocApp.DocAppRow docAppRow = docApp[0];
                        if (docAppRow.Status.Trim().Equals(AppStatusEnum.Completeness_Checked.ToString()))
                            UpdateRefStatus(docappId, AppStatusEnum.Completeness_In_Progress, false, false, null);

                        ResetSendToCdbFlags(docappId);
                    }
                    UpdateDateIn(docappId, DateTime.Parse(earliestDocSet.Rows[0]["VerificationDateIn"].ToString()));
                }
            }
        }

        /// <summary>
        /// Update DocApp Status on DocSet closed. If there is only one set for the application. update the status to Pending_Documents
        /// </summary>
        /// <param name="docSetId"></param>
        public void UpdateDocAppStatusOnDocSetClosed(int docSetId)
        {
            DocAppDb docAppDb = new DocAppDb();
            SetAppDb setAppDb = new SetAppDb();
            SetApp.SetAppDataTable setApps = setAppDb.GetSetAppByDocSetId(docSetId);

            //get all the apps for the docset
            foreach (SetApp.SetAppRow setAppRows in setApps.Rows)
            {
                int docappId = setAppRows.DocAppId;
                //if there is only one set update the status to Pending_Documents
                if (setAppDb.GetSetAppByDocAppId(docappId).Rows.Count==1)
                    UpdateRefStatus(docappId, AppStatusEnum.Pending_Documents, false, false, null);

                ResetSendToCdbFlags(docappId);
            }
        }

        /// <summary>
        /// if the reference number is already checked, update the status to Completeness_In_Progress
        /// </summary>
        /// <param name="docSetId"></param>
        public void UpdateDocAppStatusOnDocSetInsert(int docAppId)
        {
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(docAppId);

            if (docApp.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApp[0];

                if (docAppRow.Status.ToString().Trim().ToLower().Equals(AppStatusEnum.Completeness_Checked.ToString().ToLower()))
                {
                    UpdateRefStatus(docAppId, AppStatusEnum.Completeness_In_Progress, false, false, null);

                    //reset the download status.
                    UpdateDownloadDetails(docAppId, DownloadStatusEnum.Pending_Download, null);

                    ResetSendToCdbFlags(docAppId);
                }
            }
        }
        

        /// <summary>
        /// Update DocApp Status on DocSet Recategorize.
        /// </summary>
        /// <param name="docSetId"></param>
        public void UpdateDocAppStatusOnDocSetRecategorize(int docSetId)
        {
            DocSetDb docSetDb = new DocSetDb();
            SetAppDb setAppDb = new SetAppDb();
            SetApp.SetAppDataTable setApps = setAppDb.GetSetAppByDocSetId(docSetId);

            //get all the apps for the docset
            foreach (SetApp.SetAppRow setAppRows in setApps.Rows)
            {
                int docappId = setAppRows.DocAppId;

                //check the earliest docset for the docapp with verified status
                DataTable earliestDocSet = docSetDb.GetEarliestVerifiedDocSetByDocAppId(docappId, docSetId);

                //if there are no earliest records which means no verified docsets. in this case set the docapp status to pending_documents
                if (earliestDocSet.Rows.Count == 0)
                {
                    UpdateRefStatus(docappId, AppStatusEnum.Pending_Documents, true, false, null);
                }
            }
        }

        public bool UpdateCaseOic(string refNo, string caseOic)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDataByRefNo(refNo);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.CaseOIC = caseOic;

            int rowsAffected = Adapter.Update(dt);
            int id = dr.Id;

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        //Added new parameter Risk to update risk by Edward 2015/08/13 - Risk can be updated
        //public bool UpdateOic(string refNo, string caseOic, string peOic, string CADate, bool secondCA, bool IsBatchUpload, string ip, string systemInfo)
        public bool UpdateOic(string refNo, string caseOic, string peOic, string CADate, bool secondCA, bool IsBatchUpload, string ip, string systemInfo, string risk)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDataByRefNo(refNo);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.CaseOIC = caseOic;
            dr.PeOIC = peOic;

            if (!String.IsNullOrEmpty(CADate))
                dr.SecondCADate = CADate;

            if (dr.IsSecondCANull())
                dr.SecondCA = false;
            dr.SecondCA = secondCA;

            if (dr.IsSecondCAFlagNull())
                dr.SecondCAFlag = false;
            dr.SecondCAFlag = secondCA;         //Added by Edward 23/01/2014
            dr.Risk = risk;                      //Added by Edward 2015/08/13- Risk can be updated

            int rowsAffected = Adapter.Update(dt);
            int id = dr.Id;

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                if (!IsBatchUpload)
                    auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
                else //2014/05/05    Batch Upload
                    auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update, ip, systemInfo);
            }

            return rowsAffected == 1;
        }



        public void UpdateTopPendingHleByLanes(Guid userId, int count, HleLanesEnum lane)
        {
            if (count > 0)
            {
                DocApp.DocAppDataTable docApp = GetTopPendingHleByLanes(count, AppStatusEnum.Verified, lane);

                foreach (DocApp.DocAppRow docAppRow in docApp.Rows)
                {
                    AssignUserAsCompletenessOfficer(docAppRow.Id, userId);
                }
            }
        }
        /// <summary>
        /// Added by Edward 08.06.2013
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="count"></param>
        /// <param name="lane"></param>
        public void UpdateTopPendingHleByLanesIA(Guid userId, int count, HleLanesEnum lane)
        {
            if (count > 0)
            {
                DocApp.DocAppDataTable docApp = GetTopPendingHleByLanesIA(count, AssessmentStatusEnum.Completeness_Checked, lane);

                foreach (DocApp.DocAppRow docAppRow in docApp.Rows)
                {
                    AssignUserAsAssessmentOfficer(docAppRow.Id, userId);
                }
            }
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
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.SendToCDBStatus = status.ToString();
            if (status.ToString() == SendToCDBStatusEnum.Ready.ToString())
                dr.SendToCDBAttemptCount = 0;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
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
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.SendToCDBAttemptCount = attemptCount;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        #region Reset SentToLeas Added By Edward 24/02/2014 Add Icon and Action Log
        public void ResetSendToLeasFlags(int id)
        {            
            UpdateSendToLeasStatus(id, SendToLEASStatusEnum.NotReady);
            UpdateSendToLeasAttemptCount(id, 0);
        }

        public bool UpdateSendToLeasStatus(int id, SendToLEASStatusEnum status)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.SentToLEASStatus = status.ToString();
            if (status.ToString() == SendToLEASStatusEnum.Ready.ToString())
                dr.SentToLeasAttemptCount = 0;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }

        public bool UpdateSendToLeasAttemptCount(int id, int attemptCount)
        {
            DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

            if (dt.Count == 0)
                return false;

            DocApp.DocAppRow dr = dt[0];

            dr.SentToLeasAttemptCount = attemptCount;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
            }

            return rowsAffected == 1;
        }
        

        #endregion

        #region Added by Edward 2014/5/23   Changes for Sales and Resales
        //public void ResetSendToResaleFlags(int id)
        //{
        //    UpdateSendToResaleStatus(id, SendToLEASStatusEnum.NotReady);
        //    UpdateSendToResaleAttemptCount(id, 0);
        //}
        //public bool UpdateSendToResaleStatus(int id, SendToLEASStatusEnum status)
        //{
        //    DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

        //    if (dt.Count == 0)
        //        return false;

        //    DocApp.DocAppRow dr = dt[0];

        //    dr.SentToResaleStatus = status.ToString();
        //    if (status.ToString() == SendToLEASStatusEnum.Ready.ToString())
        //        dr.SentToResaleAttemptCount = 0;

        //    int rowsAffected = Adapter.Update(dt);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
        //    }

        //    return rowsAffected == 1;
        //}

        //public bool UpdateSendToResaleAttemptCount(int id, int attemptCount)
        //{
        //    DocApp.DocAppDataTable dt = Adapter.GetDocAppById(id);

        //    if (dt.Count == 0)
        //        return false;

        //    DocApp.DocAppRow dr = dt[0];

        //    dr.SentToResaleAttemptCount = attemptCount;

        //    int rowsAffected = Adapter.Update(dt);

        //    if (rowsAffected > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.DocApp, id.ToString(), OperationTypeEnum.Update);
        //    }

        //    return rowsAffected == 1;
        //}
        #endregion

        #endregion

        #region Delete Methods
        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }
        #endregion

        #region AccessRules

        /// <summary>
        /// check if user have access to save data for a given refno.
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public bool AllowCompletenessSaveDate(int docAppId)
        {
            DocApp.DocAppDataTable docApp = Adapter.GetDocAppById(docAppId);

            if (docApp.Rows.Count > 0)
            {
                DocApp.DocAppRow row = docApp[0];
                MembershipUser user = Membership.GetUser();
                if (user == null)
                    return false;

                Guid userId = (Guid)user.ProviderUserKey;

                if (row.IsCompletenessStaffUserIdNull())
                    return false;
                else if (!(row.Status.ToLower().Trim().Equals(AppStatusEnum.Completeness_In_Progress.ToString().ToLower()) ||
                    row.Status.ToLower().Trim().Equals(AppStatusEnum.Pending_Completeness.ToString().ToLower())) ||
                    !(userId.ToString().Equals(row.CompletenessStaffUserId.ToString())))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Check if user can confirm the application
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public bool CanUserConfirmApplication(int docAppId)
        {
            //1. check if there are any sets which are not verified for this application
            DocSetDb docSetDb = new DocSetDb();
            if (!docSetDb.AreAllDocSetsVerifiedOrClosed(docAppId))
                return false;

            //2 check if the documents are accepted for completeness. documents under aplication folder and unidentified folder must be verified.
            DocDb docDb = new DocDb();
            if (docDb.GetCountNotVerifiedForAppConfirmation(docAppId) != 0)
                return false;

            //3. NOT IN USE
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            // ignore this as we still need to confirm from ABCDE. hardcode to false
            //haveOutstandingDocumentsWithNoRemarks = leasInterfaceDb.GetCountOutstandingDocumentsWithNoRemarksByHleNumber(docAppRow.RefNo) > 0 ;

            //4. check if user is allowed to update the set. the docset must be assigned to the current user 
            //and the status shouldbe Completeness_In_Progress or Pending_Completeness.
            if (!AllowCompletenessSaveDate(docAppId))
                return false;

            return true;
        }
        #endregion

        /// <summary>
        /// Check if the application is confirmed.
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public bool IsAppConfirmed(int docAppId)
        {
            DocApp.DocAppDataTable docApps = GetDocAppById(docAppId);
            if (docApps.Rows.Count == 0)
                return false;
            else
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                return docAppRow.Status.ToLower().Equals(AppStatusEnum.Completeness_Checked.ToString().ToLower());
            }
        }

        #region Added by Edward 2014/9/18 Confirm and Extract Button
        public bool CanUserConfirmExtractApplication(int docAppId)
        {
            //1. check if there are any sets which are not verified for this application
            DocSetDb docSetDb = new DocSetDb();
            //if (!docSetDb.AreAllDocSetsVerifiedOrClosed(docAppId))
            //    return false;

            //2 check if the documents are accepted for completeness. documents under aplication folder and unidentified folder must be verified.
            DocDb docDb = new DocDb();
            if (docDb.GetCountNotVerifiedForAppConfirmation(docAppId) != 0)
                return false;

            //3. NOT IN USE
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            // ignore this as we still need to confirm from ABCDE. hardcode to false
            //haveOutstandingDocumentsWithNoRemarks = leasInterfaceDb.GetCountOutstandingDocumentsWithNoRemarksByHleNumber(docAppRow.RefNo) > 0 ;

            //4. check if user is allowed to update the set. the docset must be assigned to the current user 
            //and the status shouldbe Completeness_In_Progress or Pending_Completeness.
            if (!AllowCompletenessSaveDate(docAppId))
                return false;

            return true;
        }
        #endregion


        #region Modified By Edward 2015/02/06
        /// <summary>
        /// Insert or updates the Ref number in DocApp Table
        /// </summary>
        /// <param name="refNo"></param>
        /// <param name="refType"></param>
        /// <param name="cAOic"></param>
        /// <param name="pEOic"></param>
        /// <param name="CADate"></param>
        /// <param name="secondCA"></param>
        /// <param name="risk"></param>
        /// <param name="IsBatchUpload">True if it is batchupload, then input Ip and systeminfo, false then leave ip and systeminfo blank</param>
        /// <param name="ip"></param>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public int InsertRefNumber(string refNo, string refType, string cAOic, string pEOic, string CADate, bool secondCA, string risk, bool IsBatchUpload, string ip, string systemInfo)
        {
            int docAppId = -1;
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(refNo);

            if (docAppTable.Rows.Count <= 0)            
                // Insert the HLE number                
                    docAppId = docAppDb.Insert(refNo, refType, null, null,
                        AppStatusEnum.Pending_Documents, null, cAOic, pEOic, CADate, secondCA, risk, IsBatchUpload, ip, systemInfo);            
            else
            {
                DocApp.DocAppRow docAppRow = docAppTable[0];
                docAppId = docAppRow.Id;

                // Update the OIC                
                //docAppDb.UpdateOic(refNo, cAOic, pEOic, CADate, secondCA, IsBatchUpload, ip, systemInfo);
                //Added new parameter Risk to update risk by Edward 2015/08/13 - Risk can be updated    ---uncommented at 2016/03/15
                docAppDb.UpdateOic(refNo, cAOic, pEOic, CADate, secondCA, IsBatchUpload, ip, systemInfo, risk);
            }
            return docAppId;
        }

        #endregion


        #region Added By Edward 2015/02/10 Added Risk Field in Email Notification
        public static bool CheckRefNoRiskIsL(int docAppId)
        {
            bool IsResult = false;
            
            string strRisk = DocAppDs.GetDocAppRiskOnlyById(docAppId);
            if (strRisk.ToUpper().Equals("L"))
                IsResult = true;
            return IsResult;
        }
        #endregion

        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        public string GetAssessmentStatusByDocAppId(int id)
        {
            return DocAppDs.GetAssessmentStatusByDocAppId(id);
        }
        #endregion
    }
}
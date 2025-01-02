using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using HleInterfaceTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    public class HleInterfaceDb
    {
        private HleInterfaceTableAdapter _HleInterfaceAdapter = null;

        protected HleInterfaceTableAdapter Adapter
        {
            get
            {
                if (_HleInterfaceAdapter == null)
                    _HleInterfaceAdapter = new HleInterfaceTableAdapter();

                return _HleInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all data from HLE interface
        /// </summary>
        /// <returns></returns>
        public HleInterface.HleInterfaceDataTable GetHleInterface()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get all data from HLE interface
        /// </summary>
        /// <returns></returns>
        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNo(string refNo)
        {
            return Adapter.GetDataByHleNumber(refNo);
        }

        /// <summary>
        /// Get all data from HLE interface
        /// </summary>
        /// <returns></returns>
        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.GetDataByHleAndNric(refNo, nric);
        }

        /// <summary>
        /// Get all data from HLE interface
        /// </summary>
        /// <returns></returns>
        public HleInterface.HleInterfaceDataTable GetHleInterfaceByNric(string nric)
        {
            return Adapter.GetDataByNric(nric);
        }

        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNoAndCustomerId(string refNo, string CustomerId)
        {
            return Adapter.GetDataByHleAndCustomerId(refNo, CustomerId);
        }

        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNoAndName(string refNo, string name)
        {
            return Adapter.GetDataByHleAndName(name, refNo);
        }

        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service

        /// <summary>
        /// Check if the personal already exists in the system
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        //public bool DoesPersonalExistsByRefNo(string refNo)
        //{
        //    return (GetHleInterfaceByRefNo(refNo).Rows.Count > 0);
        //}

        public bool DoesPersonalExistsByRefNo(string refNo)
        {            
            return HleInterfceDs.DoesPersonalExistsByRefNo(refNo);
        }

              
        //public bool DoesPersonalExistsByRefNoNric(string refNo, string nric)
        //{
        //    return (GetHleInterfaceByRefNoAndNric(refNo, nric).Rows.Count > 0);
        //}

        public bool DoesPersonalExistsByRefNoNric(string refNo, string nric)
        {
            return HleInterfceDs.DoesPersonalExistsByRefNoNric(refNo, nric);            
        }
        #endregion

        public bool DoesPersonalExistsByRefNoCustomerId(string refNo, string CustomerId)
        {
            return (GetHleInterfaceByRefNoAndCustomerId(refNo, CustomerId).Rows.Count > 0);
        }

        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        //public bool DoesPersonalExistsByRefNoName(string refNo, string name)
        //{
        //    return (GetHleInterfaceByRefNoAndName(refNo, name).Rows.Count > 0);
        //}

        public bool DoesPersonalExistsByRefNoName(string refNo, string name)
        {
            return HleInterfceDs.DoesPersonalExistsByRefNoName(refNo, name);            
        }
        #endregion

        /// <summary>
        /// Get the Case OIC of the App
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public string GetCaseOIC(string refNo)
        {
            string result = string.Empty;

            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNo(refNo);

            if (dt.Rows.Count > 0)
            {
                HleInterface.HleInterfaceRow dr = dt[0];
                result = dr.Caoic;
            }

            return result;
        }


        /// <summary>
        /// Get Hle Status By Ref No From DocApp
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public static string GetHleStatusByRefNo(string refNo)
        {
            return HleInterfceDs.GetHleStatusByRefNo(refNo);
        }

        /// <summary>
        ///  Gets the status by DocAppId.      Added by Edward 17.10.2013
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public static string GetHleStatusByDocAppId(int docAppId)
        {
            return HleInterfceDs.GetHleStatusByDocAppId(docAppId);
        }

        /// <summary>
        /// Get the household structure
        /// Created by Sandeep
        /// Date: 2012-09-07
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public DataTable GetApplicantDetailsByRefNo(string hleNumber)
        {
            return HleInterfceDs.GetApplicantDetailsByRefNo(hleNumber);
        }

        /// <summary>
        /// Get Oic names, search for peOic or CaOic
        /// Created by Sandeep
        /// Date: 2012-12-04
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public string GetOICEmailRecipientByHleNumber(string hleNumber)
        {
            string result = Adapter.GetOICEmailRecipientByHleNumber(hleNumber);
            return (string.IsNullOrEmpty(result)? null: result);
        }

        public string GetApplicantType(string refNo, string nric)
        {
            string result = string.Empty;

            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                HleInterface.HleInterfaceRow dr = dt[0];
                result = dr.ApplicantType;
            }

            return result;
        }

        public int GetLastOrderNoByApplicantType(string refNo, string applicantType)
        {
            int? result = Adapter.GetLastOrderNoForHleByApplicantType(refNo, applicantType);

            if (result.HasValue)
                return result.Value;

            return 0;
        }

        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNoAndApplicantTypeSortByOrderNo(string refNo, string applicantType)
        {
            return Adapter.GetDataByHleNumberAndApplicantTypeSortByOrderNo(refNo, applicantType);
        }

        public HleInterface.HleInterfaceDataTable GetHleInterfaceByRefNoAndApplicantTypeSortByNric(string refNo, string applicantType)
        {
            return Adapter.GetDataByHleNumberAndApplicantTypeSortByNric(refNo, applicantType);
        }

        #region Added by Edward 2015/11/30 to Optimize LEAS Web Service
        //public int GetOrderNumberForNric(string refNo, string applicantType, string nric)
        //{
        //    HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(refNo, nric);

        //    if (dt.Rows.Count > 0)
        //    {
        //        HleInterface.HleInterfaceRow dr = dt[0];
        //        return dr.OrderNo;
        //    }

        //    return GetLastOrderNoByApplicantType(refNo, applicantType) + 1;
        //}

        public int GetOrderNumberForNric(string refNo, string applicantType, string nric)
        {
            int OrderNo = HleInterfceDs.GetOrderNumberForNric(refNo, nric);

            if (OrderNo > 0)
            {                
                return OrderNo;
            }

            return GetLastOrderNoByApplicantType(refNo, applicantType) + 1;
        }
        
        //public bool IsMainApplicant(string refNo, string nric)
        //{
        //    bool isMainApplicant = false;

        //    HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(refNo, nric);

        //    if (dt.Rows.Count > 0)
        //    {
        //        HleInterface.HleInterfaceRow dr = dt[0];
        //        isMainApplicant = dr.IsMainApplicant;                
        //    }

        //    return isMainApplicant;
        //}

        public bool IsMainApplicant(string refNo, string nric)
        {
            return HleInterfceDs.IsMainApplicant(refNo, nric); 
        }
        #endregion


        #endregion

        #region Insert Methods
        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2015/02/04          Added Risk Field in Application

        /// <summary>
        /// Insert personal record
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Insert(HlePersonal hlePersonal)
        {
            HleInterface.HleInterfaceDataTable dt = new HleInterface.HleInterfaceDataTable();
            HleInterface.HleInterfaceRow r = dt.NewHleInterfaceRow();

            r.HleNumber = hlePersonal.HleNumber;
            r.HleDate = hlePersonal.HleDate;
            r.HleStatus = hlePersonal.HleStatus;
            r.HouseHoldInc = hlePersonal.HhIncome;
            r.CaInc = hlePersonal.CaIncome;
            r.Peoic = (String.IsNullOrEmpty(hlePersonal.PeOic) ? string.Empty : hlePersonal.PeOic);
            r.Caoic = (String.IsNullOrEmpty(hlePersonal.CaOic) ? string.Empty : hlePersonal.CaOic);
            r.RejectedDate = hlePersonal.RejectedDate;
            r.CancelledDate = hlePersonal.CancelledDate;
            r.ApprovedDate = hlePersonal.ApprovedDate;
            r.ExpiredDate = hlePersonal.ExpiredDate;
            r.Risk = hlePersonal.Risk;  //Added By Edward 2014/02/04

            r.ApplicantType = (String.IsNullOrEmpty(hlePersonal.ApplicantType) ? string.Empty : hlePersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(hlePersonal.CustomerId) ? string.Empty : hlePersonal.CustomerId);
            r.Nric = hlePersonal.Nric;
            r.Name = hlePersonal.Name;
            r.MaritalStatusCode = hlePersonal.MaritalStatusCode;
            r.MaritalStatus = hlePersonal.MaritalStatus;
            r.RelationshipCode = hlePersonal.RelationshipCode;
            r.Relationship = hlePersonal.Relationship;
            r.CitizenshipCode = (String.IsNullOrEmpty(hlePersonal.CitizenshipCode) ? string.Empty : hlePersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(hlePersonal.Citizenship) ? string.Empty : hlePersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(hlePersonal.DateOfBirth) || hlePersonal.DateOfBirth.Equals(".") ? string.Empty : hlePersonal.DateOfBirth);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(hlePersonal.EmploymentTypeCode) ? string.Empty : hlePersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(hlePersonal.EmploymentType) ? string.Empty : hlePersonal.EmploymentType);
            r.Inc1Date = (String.IsNullOrEmpty(hlePersonal.Date1) ? string.Empty : hlePersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(hlePersonal.Inc1) ? string.Empty : hlePersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(hlePersonal.Date2) ? string.Empty : hlePersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(hlePersonal.Inc2) ? string.Empty : hlePersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(hlePersonal.Date3) ? string.Empty : hlePersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(hlePersonal.Inc3) ? string.Empty : hlePersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(hlePersonal.Date4) ? string.Empty : hlePersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(hlePersonal.Inc4) ? string.Empty : hlePersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(hlePersonal.Date5) ? string.Empty : hlePersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(hlePersonal.Inc5) ? string.Empty : hlePersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(hlePersonal.Date6) ? string.Empty : hlePersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(hlePersonal.Inc6) ? string.Empty : hlePersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(hlePersonal.Date7) ? string.Empty : hlePersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(hlePersonal.Inc7) ? string.Empty : hlePersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(hlePersonal.Date8) ? string.Empty : hlePersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(hlePersonal.Inc8) ? string.Empty : hlePersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(hlePersonal.Date9) ? string.Empty : hlePersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(hlePersonal.Inc9) ? string.Empty : hlePersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(hlePersonal.Date10) ? string.Empty : hlePersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(hlePersonal.Inc10) ? string.Empty : hlePersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(hlePersonal.Date11) ? string.Empty : hlePersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(hlePersonal.Inc11) ? string.Empty : hlePersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(hlePersonal.Date12) ? string.Empty : hlePersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(hlePersonal.Inc12) ? string.Empty : hlePersonal.Inc12);
            r.IncYear = (String.IsNullOrEmpty(hlePersonal.IncYear) ? string.Empty : hlePersonal.IncYear);
            r.IncAverage = (String.IsNullOrEmpty(hlePersonal.IncAve) ? string.Empty : hlePersonal.IncAve);
            r.EmployerName = (String.IsNullOrEmpty(hlePersonal.EmployerName) ? string.Empty : hlePersonal.EmployerName);
            r.DateJoined = (String.IsNullOrEmpty(hlePersonal.DateJoined) ? string.Empty : hlePersonal.DateJoined);
            r.WithAllowance = hlePersonal.WithAllowance;
            r.WithCpf = hlePersonal.WithCpf;
            //added By Edward 18/12/2013 
            r.NoOfIncomeMonths = hlePersonal.NoOfIncomeMonths;

            r.IsMainApplicant = hlePersonal.IsMainApplicant;
            r.OrderNo = hlePersonal.OrderNo;

            dt.AddHleInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.HleInterface, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }


        //Added By Edward 2014/05/05    Batch Upload
        public int Insert(HlePersonal hlePersonal, string ip, string systemInfo)
        {
            HleInterface.HleInterfaceDataTable dt = new HleInterface.HleInterfaceDataTable();
            HleInterface.HleInterfaceRow r = dt.NewHleInterfaceRow();

            r.HleNumber = hlePersonal.HleNumber;
            r.HleDate = hlePersonal.HleDate;
            r.HleStatus = hlePersonal.HleStatus;
            r.HouseHoldInc = hlePersonal.HhIncome;
            r.CaInc = hlePersonal.CaIncome;
            r.Peoic = (String.IsNullOrEmpty(hlePersonal.PeOic) ? string.Empty : hlePersonal.PeOic);
            r.Caoic = (String.IsNullOrEmpty(hlePersonal.CaOic) ? string.Empty : hlePersonal.CaOic);
            r.RejectedDate = hlePersonal.RejectedDate;
            r.CancelledDate = hlePersonal.CancelledDate;
            r.ApprovedDate = hlePersonal.ApprovedDate;
            r.ExpiredDate = hlePersonal.ExpiredDate;
            r.Risk = hlePersonal.Risk;  //Added By Edward 2014/02/04

            r.ApplicantType = (String.IsNullOrEmpty(hlePersonal.ApplicantType) ? string.Empty : hlePersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(hlePersonal.CustomerId) ? string.Empty : hlePersonal.CustomerId);
            r.Nric = hlePersonal.Nric;
            r.Name = hlePersonal.Name;
            r.MaritalStatusCode = hlePersonal.MaritalStatusCode;
            r.MaritalStatus = hlePersonal.MaritalStatus;
            r.RelationshipCode = hlePersonal.RelationshipCode;
            r.Relationship = hlePersonal.Relationship;
            r.CitizenshipCode = (String.IsNullOrEmpty(hlePersonal.CitizenshipCode) ? string.Empty : hlePersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(hlePersonal.Citizenship) ? string.Empty : hlePersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(hlePersonal.DateOfBirth) || hlePersonal.DateOfBirth.Equals(".") ? string.Empty : hlePersonal.DateOfBirth);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(hlePersonal.EmploymentTypeCode) ? string.Empty : hlePersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(hlePersonal.EmploymentType) ? string.Empty : hlePersonal.EmploymentType);
            r.Inc1Date = (String.IsNullOrEmpty(hlePersonal.Date1) ? string.Empty : hlePersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(hlePersonal.Inc1) ? string.Empty : hlePersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(hlePersonal.Date2) ? string.Empty : hlePersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(hlePersonal.Inc2) ? string.Empty : hlePersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(hlePersonal.Date3) ? string.Empty : hlePersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(hlePersonal.Inc3) ? string.Empty : hlePersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(hlePersonal.Date4) ? string.Empty : hlePersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(hlePersonal.Inc4) ? string.Empty : hlePersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(hlePersonal.Date5) ? string.Empty : hlePersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(hlePersonal.Inc5) ? string.Empty : hlePersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(hlePersonal.Date6) ? string.Empty : hlePersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(hlePersonal.Inc6) ? string.Empty : hlePersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(hlePersonal.Date7) ? string.Empty : hlePersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(hlePersonal.Inc7) ? string.Empty : hlePersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(hlePersonal.Date8) ? string.Empty : hlePersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(hlePersonal.Inc8) ? string.Empty : hlePersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(hlePersonal.Date9) ? string.Empty : hlePersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(hlePersonal.Inc9) ? string.Empty : hlePersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(hlePersonal.Date10) ? string.Empty : hlePersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(hlePersonal.Inc10) ? string.Empty : hlePersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(hlePersonal.Date11) ? string.Empty : hlePersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(hlePersonal.Inc11) ? string.Empty : hlePersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(hlePersonal.Date12) ? string.Empty : hlePersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(hlePersonal.Inc12) ? string.Empty : hlePersonal.Inc12);
            r.IncYear = (String.IsNullOrEmpty(hlePersonal.IncYear) ? string.Empty : hlePersonal.IncYear);
            r.IncAverage = (String.IsNullOrEmpty(hlePersonal.IncAve) ? string.Empty : hlePersonal.IncAve);
            r.EmployerName = (String.IsNullOrEmpty(hlePersonal.EmployerName) ? string.Empty : hlePersonal.EmployerName);
            r.DateJoined = (String.IsNullOrEmpty(hlePersonal.DateJoined) ? string.Empty : hlePersonal.DateJoined);
            r.WithAllowance = hlePersonal.WithAllowance;
            r.WithCpf = hlePersonal.WithCpf;
            //added By Edward 18/12/2013 
            r.NoOfIncomeMonths = hlePersonal.NoOfIncomeMonths;

            r.IsMainApplicant = hlePersonal.IsMainApplicant;
            r.OrderNo = hlePersonal.OrderNo;

            dt.AddHleInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.HleInterface, id.ToString(), OperationTypeEnum.Insert, ip, systemInfo);
            }
            return id;
        }

        #endregion

        #region Update Methods
        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2014/02/04          Added Risk Field in Application

        /// <summary>
        /// Update the personal record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Update(HlePersonal hlePersonal)
        {
            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(hlePersonal.HleNumber, hlePersonal.Nric);

            if (dt.Count == 0)
                dt = GetHleInterfaceByRefNoAndName(hlePersonal.HleNumber, hlePersonal.Name);

            if (dt.Count == 0) return false;

            HleInterface.HleInterfaceRow r = dt[0];

            r.HleNumber = hlePersonal.HleNumber;
            r.HleDate = hlePersonal.HleDate;
            r.HleStatus = hlePersonal.HleStatus;
            r.HouseHoldInc = hlePersonal.HhIncome;
            r.CaInc = hlePersonal.CaIncome;
            r.Peoic = hlePersonal.PeOic;
            r.Caoic = hlePersonal.CaOic;
            r.RejectedDate = hlePersonal.RejectedDate;
            r.CancelledDate = hlePersonal.CancelledDate;
            r.ApprovedDate = hlePersonal.ApprovedDate;
            r.ExpiredDate = hlePersonal.ExpiredDate;
            r.Risk = hlePersonal.Risk;  //Added By Edward 2014/02/04

            r.ApplicantType = hlePersonal.ApplicantType;
            r.CustomerId = hlePersonal.CustomerId;
            r.Nric = hlePersonal.Nric;
            r.Name = hlePersonal.Name;
            r.MaritalStatusCode = hlePersonal.MaritalStatusCode;
            r.MaritalStatus = hlePersonal.MaritalStatus;
            r.RelationshipCode = hlePersonal.RelationshipCode;
            r.Relationship = hlePersonal.Relationship;
            r.CitizenshipCode = (String.IsNullOrEmpty(hlePersonal.CitizenshipCode) ? string.Empty : hlePersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(hlePersonal.Citizenship) ? string.Empty : hlePersonal.Citizenship);
            r.DateOfBirth = (hlePersonal.DateOfBirth.Equals(".") ? string.Empty : hlePersonal.DateOfBirth);
            r.EmployerName = hlePersonal.EmployerName;
            r.DateJoined = hlePersonal.DateJoined;
            r.EmploymentTypeCode = hlePersonal.EmploymentTypeCode;
            r.EmploymentType = hlePersonal.EmploymentType;
            r.Inc1Date = hlePersonal.Date1;
            r.Inc1 = hlePersonal.Inc1;
            r.Inc2Date = hlePersonal.Date2;
            r.Inc2 = hlePersonal.Inc2;
            r.Inc3Date = hlePersonal.Date3;
            r.Inc3 = hlePersonal.Inc3;
            r.Inc4Date = hlePersonal.Date4;
            r.Inc4 = hlePersonal.Inc4;
            r.Inc5Date = hlePersonal.Date5;
            r.Inc5 = hlePersonal.Inc5;
            r.Inc6Date = hlePersonal.Date6;
            r.Inc6 = hlePersonal.Inc6;
            r.Inc7Date = hlePersonal.Date7;
            r.Inc7 = hlePersonal.Inc7;
            r.Inc8Date = hlePersonal.Date8;
            r.Inc8 = hlePersonal.Inc8;
            r.Inc9Date = hlePersonal.Date9;
            r.Inc9 = hlePersonal.Inc9;
            r.Inc10Date = hlePersonal.Date10;
            r.Inc10 = hlePersonal.Inc10;
            r.Inc11Date = hlePersonal.Date11;
            r.Inc11 = hlePersonal.Inc11;
            r.Inc12Date = hlePersonal.Date12;
            r.Inc12 = hlePersonal.Inc12;
            r.IncYear = hlePersonal.IncYear;
            r.IncAverage = hlePersonal.IncAve;
            r.WithAllowance = hlePersonal.WithAllowance;
            r.WithCpf = hlePersonal.WithCpf;
            //Added By Edward 18/12/2013
            r.NoOfIncomeMonths = hlePersonal.NoOfIncomeMonths;

            r.IsMainApplicant = hlePersonal.IsMainApplicant;
            r.OrderNo = hlePersonal.OrderNo;

            int rowsAffected = Adapter.Update(dt);
            
            return (rowsAffected > 0);
        }

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2014/02/04          Added Risk Field in Application

        public bool UpdateApplication(string refNo, string applDate, string appStatus, string pEOic, string cAOic, string hhInc,
            string cAInc, string rejDate, string canDate, string apprDate, string expDate, string risk)
        {
            return Adapter.UpdateHleInterfaceInfo(applDate, appStatus, hhInc, cAInc, pEOic, cAOic, rejDate, canDate, apprDate, expDate, risk, refNo) > 0;
        }

        public bool UpdateOrderNo(string refNo, string nric, int orderNo)
        {
            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Count == 0) return false;

            HleInterface.HleInterfaceRow r = dt[0];

            r.OrderNo = orderNo;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateOrderNos(string refNo, string applicantType, bool sortByOrderNo)
        {
            HleInterface.HleInterfaceDataTable dt = (sortByOrderNo ?
                GetHleInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, applicantType) :
                GetHleInterfaceByRefNoAndApplicantTypeSortByNric(refNo, applicantType));

            if (dt.Count == 0) return false;

            int orderNo = 1;
            foreach (HleInterface.HleInterfaceRow r in dt)
            {
                r.OrderNo = orderNo;

                int rowsAffected = Adapter.Update(dt);

                orderNo++;
            }

            return (orderNo > 1);
        }
        
        /// <summary>
        /// Update all the HA applicants order nos after the records are inserted using the LEAS web service
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public bool UpdateOrderNosOfHaApplicants(string refNo)
        {
            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, "HA");

            // Set the main applicant as the first personal in the order
            foreach (HleInterface.HleInterfaceRow r in dt)
            {
                if (r.IsMainApplicant)
                {
                    r.OrderNo = 1;

                    int rowsAffected = Adapter.Update(dt);
                    break;
                }
            }

            // Update all the other order nos by incrementing by 1
            foreach (HleInterface.HleInterfaceRow r in dt)
            {
                if (!r.IsMainApplicant)
                {
                    r.OrderNo = r.OrderNo++;

                    int rowsAffected = Adapter.Update(dt);
                }
            }

            // Update all the order numbers of all the HA applicants to make the numbers in sequence
            return true;
        }
               
        /// <summary>
        /// Update all the OC occupiers order nos after the records are inserted using the LEAS web service
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public bool UpdateOrderNosOfOcOccupiers(string refNo)
        {
            //return UpdateOrderNos(refNo, "OC", true); //Commented By Edward 19/12/2013 To Cater UAT Issue for Occupier Sorting by NRIC
            return UpdateOrderNos(refNo, "OC", false); //Added By Edward 19/12/2013 To Cater UAT Issue for Occupier Sorting by NRIC
        }

        #region Added By Edward Get Income Months from Household when NoOfIncomeMonths is Null
        public bool UpdateNoOfIncomeMonths(string refNo, string nric, int NoOfIncomeMonths)
        {
            HleInterface.HleInterfaceDataTable dt = GetHleInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Count == 0) return false;

            HleInterface.HleInterfaceRow r = dt[0];

            r.NoOfIncomeMonths = NoOfIncomeMonths;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #endregion

        #region Delete
        public void DeleteWrongRecords()
        {
            HleInterfceDs.DeleteWrongRecords();
        }

        public bool DeleteByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.DeleteByRefNoAndNric(refNo, nric) > 0;
        }

        public bool DeleteByRefNo(string refNo)
        {
            return Adapter.DeleteByHleNumber(refNo) > 0;
        }
        #endregion
    }
}
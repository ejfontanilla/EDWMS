using System;
using System.Collections.Generic;
using System.Web;
using SalesInterfaceTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResaleInterfaceDb
    /// </summary>
    public class SalesInterfaceDb
    {
        private SalesInterfaceTableAdapter _SalesInterfaceAdapter = null;

        protected SalesInterfaceTableAdapter Adapter
        {
            get
            {
                if (_SalesInterfaceAdapter == null)
                    _SalesInterfaceAdapter = new SalesInterfaceTableAdapter();

                return _SalesInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces
        /// </summary>
        /// <returns></returns>
        public SalesInterface.SalesInterfaceDataTable GetSalesInterface()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get data by case no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <returns></returns>
        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNo(string refNo)
        {
            return Adapter.GetDataByRegNo(refNo);
        }

        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.GetDataByRegNoAndNric(refNo, nric);
        }

        /// <summary>
        /// Get data by nric
        /// </summary>
        /// <param name="nric"></param>
        /// <returns></returns>
        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByNric(string nric)
        {
            return Adapter.GetDataByNric(nric);
        }

        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNoAndCustomerId(string refNo, string CustomerId)
        {
            return Adapter.GetDataByRegNoAndCustomerId(refNo, CustomerId);
        }

        /// <summary>
        /// Get data by ref no and nric
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNoAndName(string refNo, string name)
        {
            return Adapter.GetDataByRegNoAndName(refNo, name);
        }

        /// <summary>
        /// Check if personal exists in a ref no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public bool DoesPersonalExistsByRefNo(string refNo)
        {
            return (GetSalesInterfaceByRefNo(refNo).Rows.Count > 0);
        }

        public bool DoesPersonalExistsByRefNoNric(string refNo, string nric)
        {
            return GetSalesInterfaceByRefNoAndNric(refNo, nric).Rows.Count > 0;
        }

        public bool DoesPersonalExistsByRefNoCustomerId(string refNo, string CustomerId)
        {
            return (GetSalesInterfaceByRefNoAndCustomerId(refNo, CustomerId).Rows.Count > 0);
        }

        public bool DoesPersonalExistsByRefNoName(string refNo, string name)
        {
            return GetSalesInterfaceByRefNoAndName(refNo, name).Rows.Count > 0;
        }

        /// <summary>
        /// Get the household structure
        /// Created by Sandeep
        /// Date: 2012-09-07
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public SalesInterface.SalesInterfaceDataTable GetApplicantDetailsByRefNo(string refNo)
        {
            return Adapter.GetDataByRegNo(refNo);
        }

        public string GetApplicantType(string refNo, string nric)
        {
            string result = string.Empty;

            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SalesInterface.SalesInterfaceRow dr = dt[0];
                result = dr.ApplicantType;
            }

            return result;
        }

        public int GetLastOrderNoByApplicantType(string refNo, string applicantType)
        {
            int? result = Adapter.GetLastOrderNoForRegNoByApplicantType(refNo, applicantType);

            if (result.HasValue)
                return result.Value;

            return 0;
        }

        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNoAndApplicantTypeSortByOrderNo(string refNo, string applicantType)
        {
            return Adapter.GetDataByRegNoAndApplicantTypeSortByOrderNo(refNo, applicantType);
        }

        public SalesInterface.SalesInterfaceDataTable GetSalesInterfaceByRefNoAndApplicantTypeSortByNric(string refNo, string applicantType)
        {
            return Adapter.GetDataByRegNoAndApplicantTypeSortByNric(refNo, applicantType);
        }

        public int GetOrderNumberForNric(string refNo, string applicantType, string nric)
        {
            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SalesInterface.SalesInterfaceRow dr = dt[0];
                return dr.OrderNo;
            }

            return GetLastOrderNoByApplicantType(refNo, applicantType) + 1;
        }

        public bool IsMainApplicant(string refNo, string nric)
        {
            bool isMainApplicant = false;

            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SalesInterface.SalesInterfaceRow dr = dt[0];
                isMainApplicant = dr.IsMainApplicant;
            }

            return isMainApplicant;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="rosPersonal"></param>
        /// <returns></returns>
        public int Insert(SalesPersonal salesPersonal)
        {
            SalesInterface.SalesInterfaceDataTable dt = new SalesInterface.SalesInterfaceDataTable();
            SalesInterface.SalesInterfaceRow r = dt.NewSalesInterfaceRow();

            r.RegistrationNo = salesPersonal.RefNumber;
            r.Status = (String.IsNullOrEmpty(salesPersonal.Status) ? string.Empty : salesPersonal.Status);
            r.BallotQuarter = (String.IsNullOrEmpty(salesPersonal.BallotQuarter) ? string.Empty : salesPersonal.BallotQuarter);
            r.HouseholdType = (String.IsNullOrEmpty(salesPersonal.HouseholdType) ? string.Empty : salesPersonal.HouseholdType);
            r.AllocationMode = (String.IsNullOrEmpty(salesPersonal.AllocationMode) ? string.Empty : salesPersonal.AllocationMode);
            r.AllocationScheme = (String.IsNullOrEmpty(salesPersonal.AllocationScheme) ? string.Empty : salesPersonal.AllocationScheme);
            r.EligibilityScheme = (String.IsNullOrEmpty(salesPersonal.EligibilityScheme) ? string.Empty : salesPersonal.EligibilityScheme);
            r.FlatType = (String.IsNullOrEmpty(salesPersonal.FlatType) ? string.Empty : salesPersonal.FlatType);
            r.AHGStatus = (String.IsNullOrEmpty(salesPersonal.AhgStatus) ? string.Empty : salesPersonal.AhgStatus);
            r.SHGStatus = (String.IsNullOrEmpty(salesPersonal.ShgStatus) ? string.Empty : salesPersonal.ShgStatus);
            r.LoanTag = (String.IsNullOrEmpty(salesPersonal.LoanTag) ? string.Empty : salesPersonal.LoanTag);
            r.BookedAddress = (String.IsNullOrEmpty(salesPersonal.BookedAddress) ? string.Empty : salesPersonal.BookedAddress);
            r.FlatDesign = (String.IsNullOrEmpty(salesPersonal.FlatDesign) ? string.Empty : salesPersonal.FlatDesign);
            r.AffectedSERS = (String.IsNullOrEmpty(salesPersonal.AffectedSERS) ? string.Empty : salesPersonal.AffectedSERS);
            r.AcceptanceDate = (String.IsNullOrEmpty(salesPersonal.AcceptanceDate) ? string.Empty : salesPersonal.AcceptanceDate);
            #region Added By Edward 12/3/2014  Sales and Resale Changes
            //ForwardedDate is replaced to ApplicationDate by Edward 12/3/2014  Sales and Resale Changes
            //r.ForwardedDate = (String.IsNullOrEmpty(salesPersonal.ForwardedDate) ? string.Empty : salesPersonal.ForwardedDate); 
            r.ApplicationDate = (String.IsNullOrEmpty(salesPersonal.ApplicationDate) ? string.Empty : salesPersonal.ApplicationDate);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(salesPersonal.EmploymentTypeCode) ? string.Empty : salesPersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(salesPersonal.EmploymentType) ? string.Empty : salesPersonal.EmploymentType);
            #endregion

            r.ApplicantType = (String.IsNullOrEmpty(salesPersonal.ApplicantType) ? string.Empty : salesPersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(salesPersonal.ApplicantType) ? string.Empty : salesPersonal.CustomerId);
            r.Nric = salesPersonal.Nric;
            r.Name = salesPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(salesPersonal.MaritalStatusCode) ? string.Empty : salesPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(salesPersonal.MaritalStatus) ? string.Empty : salesPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(salesPersonal.RelationshipCode) ? string.Empty : salesPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(salesPersonal.Relationship) ? string.Empty : salesPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(salesPersonal.CitizenshipCode) ? string.Empty : salesPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(salesPersonal.Citizenship) ? string.Empty : salesPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(salesPersonal.DateOfBirth) || salesPersonal.DateOfBirth.Equals(".") ? string.Empty : salesPersonal.DateOfBirth);
            r.Income = (String.IsNullOrEmpty(salesPersonal.Income) ? string.Empty : salesPersonal.Income);
            r.IsMCPS = salesPersonal.IsMCPS;
            r.IsPPO = salesPersonal.IsPPO;
            r.NRICSOC = (String.IsNullOrEmpty(salesPersonal.nricSOC) ? string.Empty : salesPersonal.nricSOC);
            r.NameSOC = (String.IsNullOrEmpty(salesPersonal.nameSOC) ? string.Empty : salesPersonal.nameSOC);

            r.IsMainApplicant = salesPersonal.IsMainApplicant;
            r.OrderNo = salesPersonal.OrderNo;
            //added By Edward 19/2/2014 
            r.NoOfIncomeMonths = salesPersonal.NoOfIncomeMonths;
            r.KeyIssueDate = salesPersonal.KeyIssueDate;
            r.CancelledDate = salesPersonal.CancelledDate;
            r.ABCDERefNumber = salesPersonal.ABCDERefNumber;

            #region Added By Edward 11/3/2014 Sales and Resale Changes
            r.HouseHoldInc = (String.IsNullOrEmpty(salesPersonal.HouseholdIncome) ? string.Empty : salesPersonal.HouseholdIncome);
            r.OIC1 = (string.IsNullOrEmpty(salesPersonal.OIC1) ? string.Empty : salesPersonal.OIC1);
            r.OIC2 = (string.IsNullOrEmpty(salesPersonal.OIC2) ? string.Empty : salesPersonal.OIC2);
            r.Inc1Date = (String.IsNullOrEmpty(salesPersonal.Date1) ? string.Empty : salesPersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(salesPersonal.Inc1) ? string.Empty : salesPersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(salesPersonal.Date2) ? string.Empty : salesPersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(salesPersonal.Inc2) ? string.Empty : salesPersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(salesPersonal.Date3) ? string.Empty : salesPersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(salesPersonal.Inc3) ? string.Empty : salesPersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(salesPersonal.Date4) ? string.Empty : salesPersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(salesPersonal.Inc4) ? string.Empty : salesPersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(salesPersonal.Date5) ? string.Empty : salesPersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(salesPersonal.Inc5) ? string.Empty : salesPersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(salesPersonal.Date6) ? string.Empty : salesPersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(salesPersonal.Inc6) ? string.Empty : salesPersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(salesPersonal.Date7) ? string.Empty : salesPersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(salesPersonal.Inc7) ? string.Empty : salesPersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(salesPersonal.Date8) ? string.Empty : salesPersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(salesPersonal.Inc8) ? string.Empty : salesPersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(salesPersonal.Date9) ? string.Empty : salesPersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(salesPersonal.Inc9) ? string.Empty : salesPersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(salesPersonal.Date10) ? string.Empty : salesPersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(salesPersonal.Inc10) ? string.Empty : salesPersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(salesPersonal.Date11) ? string.Empty : salesPersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(salesPersonal.Inc11) ? string.Empty : salesPersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(salesPersonal.Date12) ? string.Empty : salesPersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(salesPersonal.Inc12) ? string.Empty : salesPersonal.Inc12);
            #endregion

            dt.AddSalesInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SalesInterface, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }

        //Added By Edward 2014/05/05    Batch Upload
        public int Insert(SalesPersonal salesPersonal, string ip, string systemInfo)
        {
            SalesInterface.SalesInterfaceDataTable dt = new SalesInterface.SalesInterfaceDataTable();
            SalesInterface.SalesInterfaceRow r = dt.NewSalesInterfaceRow();

            r.RegistrationNo = salesPersonal.RefNumber;
            r.Status = (String.IsNullOrEmpty(salesPersonal.Status) ? string.Empty : salesPersonal.Status);
            r.BallotQuarter = (String.IsNullOrEmpty(salesPersonal.BallotQuarter) ? string.Empty : salesPersonal.BallotQuarter);
            r.HouseholdType = (String.IsNullOrEmpty(salesPersonal.HouseholdType) ? string.Empty : salesPersonal.HouseholdType);
            r.AllocationMode = (String.IsNullOrEmpty(salesPersonal.AllocationMode) ? string.Empty : salesPersonal.AllocationMode);
            r.AllocationScheme = (String.IsNullOrEmpty(salesPersonal.AllocationScheme) ? string.Empty : salesPersonal.AllocationScheme);
            r.EligibilityScheme = (String.IsNullOrEmpty(salesPersonal.EligibilityScheme) ? string.Empty : salesPersonal.EligibilityScheme);
            r.FlatType = (String.IsNullOrEmpty(salesPersonal.FlatType) ? string.Empty : salesPersonal.FlatType);
            r.AHGStatus = (String.IsNullOrEmpty(salesPersonal.AhgStatus) ? string.Empty : salesPersonal.AhgStatus);
            r.SHGStatus = (String.IsNullOrEmpty(salesPersonal.ShgStatus) ? string.Empty : salesPersonal.ShgStatus);
            r.LoanTag = (String.IsNullOrEmpty(salesPersonal.LoanTag) ? string.Empty : salesPersonal.LoanTag);
            r.BookedAddress = (String.IsNullOrEmpty(salesPersonal.BookedAddress) ? string.Empty : salesPersonal.BookedAddress);
            r.FlatDesign = (String.IsNullOrEmpty(salesPersonal.FlatDesign) ? string.Empty : salesPersonal.FlatDesign);
            r.AffectedSERS = (String.IsNullOrEmpty(salesPersonal.AffectedSERS) ? string.Empty : salesPersonal.AffectedSERS);
            r.AcceptanceDate = (String.IsNullOrEmpty(salesPersonal.AcceptanceDate) ? string.Empty : salesPersonal.AcceptanceDate);
            #region Added By Edward 12/3/2014  Sales and Resale Changes
            //ForwardedDate is replaced to ApplicationDate by Edward 12/3/2014  Sales and Resale Changes
            //r.ForwardedDate = (String.IsNullOrEmpty(salesPersonal.ForwardedDate) ? string.Empty : salesPersonal.ForwardedDate); 
            r.ApplicationDate = (String.IsNullOrEmpty(salesPersonal.ApplicationDate) ? string.Empty : salesPersonal.ApplicationDate);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(salesPersonal.EmploymentTypeCode) ? string.Empty : salesPersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(salesPersonal.EmploymentType) ? string.Empty : salesPersonal.EmploymentType);
            #endregion
            r.ABCDERefNumber = salesPersonal.ABCDERefNumber;
            r.ApplicantType = (String.IsNullOrEmpty(salesPersonal.ApplicantType) ? string.Empty : salesPersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(salesPersonal.ApplicantType) ? string.Empty : salesPersonal.CustomerId);
            r.Nric = salesPersonal.Nric;
            r.Name = salesPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(salesPersonal.MaritalStatusCode) ? string.Empty : salesPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(salesPersonal.MaritalStatus) ? string.Empty : salesPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(salesPersonal.RelationshipCode) ? string.Empty : salesPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(salesPersonal.Relationship) ? string.Empty : salesPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(salesPersonal.CitizenshipCode) ? string.Empty : salesPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(salesPersonal.Citizenship) ? string.Empty : salesPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(salesPersonal.DateOfBirth) || salesPersonal.DateOfBirth.Equals(".") ? string.Empty : salesPersonal.DateOfBirth);
            r.Income = (String.IsNullOrEmpty(salesPersonal.Income) ? string.Empty : salesPersonal.Income);
            r.IsMCPS = salesPersonal.IsMCPS;
            r.IsPPO = salesPersonal.IsPPO;
            r.NRICSOC = (String.IsNullOrEmpty(salesPersonal.nricSOC) ? string.Empty : salesPersonal.nricSOC);
            r.NameSOC = (String.IsNullOrEmpty(salesPersonal.nameSOC) ? string.Empty : salesPersonal.nameSOC);

            r.IsMainApplicant = salesPersonal.IsMainApplicant;
            r.OrderNo = salesPersonal.OrderNo;
            //added By Edward 19/2/2014 
            r.NoOfIncomeMonths = salesPersonal.NoOfIncomeMonths;

            r.KeyIssueDate = salesPersonal.KeyIssueDate;
            r.CancelledDate = salesPersonal.CancelledDate;

            #region Added By Edward 11/3/2014 Sales and Resale Changes
            r.HouseHoldInc = (String.IsNullOrEmpty(salesPersonal.HouseholdIncome) ? string.Empty : salesPersonal.HouseholdIncome);
            r.OIC1 = (string.IsNullOrEmpty(salesPersonal.OIC1) ? string.Empty : salesPersonal.OIC1);
            r.OIC2 = (string.IsNullOrEmpty(salesPersonal.OIC2) ? string.Empty : salesPersonal.OIC2);
            r.Inc1Date = (String.IsNullOrEmpty(salesPersonal.Date1) ? string.Empty : salesPersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(salesPersonal.Inc1) ? string.Empty : salesPersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(salesPersonal.Date2) ? string.Empty : salesPersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(salesPersonal.Inc2) ? string.Empty : salesPersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(salesPersonal.Date3) ? string.Empty : salesPersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(salesPersonal.Inc3) ? string.Empty : salesPersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(salesPersonal.Date4) ? string.Empty : salesPersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(salesPersonal.Inc4) ? string.Empty : salesPersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(salesPersonal.Date5) ? string.Empty : salesPersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(salesPersonal.Inc5) ? string.Empty : salesPersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(salesPersonal.Date6) ? string.Empty : salesPersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(salesPersonal.Inc6) ? string.Empty : salesPersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(salesPersonal.Date7) ? string.Empty : salesPersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(salesPersonal.Inc7) ? string.Empty : salesPersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(salesPersonal.Date8) ? string.Empty : salesPersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(salesPersonal.Inc8) ? string.Empty : salesPersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(salesPersonal.Date9) ? string.Empty : salesPersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(salesPersonal.Inc9) ? string.Empty : salesPersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(salesPersonal.Date10) ? string.Empty : salesPersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(salesPersonal.Inc10) ? string.Empty : salesPersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(salesPersonal.Date11) ? string.Empty : salesPersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(salesPersonal.Inc11) ? string.Empty : salesPersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(salesPersonal.Date12) ? string.Empty : salesPersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(salesPersonal.Inc12) ? string.Empty : salesPersonal.Inc12);
            #endregion

            dt.AddSalesInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SalesInterface, id.ToString(), OperationTypeEnum.Insert, ip, systemInfo);
            }

            return id;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="rosPersonal"></param>
        /// <returns></returns>
        public bool Update(SalesPersonal salesPersonal)
        {
            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(salesPersonal.RefNumber, salesPersonal.Nric);

            if (dt.Count == 0)
                dt = GetSalesInterfaceByRefNoAndName(salesPersonal.RefNumber, salesPersonal.Name);

            if (dt.Count == 0) return false;

            SalesInterface.SalesInterfaceRow r = dt[0];

            r.RegistrationNo = salesPersonal.RefNumber;
            r.Status = salesPersonal.Status;
            r.BallotQuarter = salesPersonal.BallotQuarter;
            r.HouseholdType = salesPersonal.HouseholdType;
            r.AllocationMode = salesPersonal.AllocationMode;
            r.AllocationScheme = salesPersonal.AllocationScheme;
            r.EligibilityScheme = (String.IsNullOrEmpty(salesPersonal.EligibilityScheme) ? string.Empty : salesPersonal.EligibilityScheme);
            r.HouseHoldInc = salesPersonal.HouseholdIncome;
            r.FlatType = salesPersonal.FlatType;
            r.AHGStatus = (String.IsNullOrEmpty(salesPersonal.AhgStatus) ? string.Empty : salesPersonal.AhgStatus);
            r.SHGStatus = (String.IsNullOrEmpty(salesPersonal.ShgStatus) ? string.Empty : salesPersonal.ShgStatus);
            r.LoanTag = salesPersonal.LoanTag;
            r.BookedAddress = salesPersonal.BookedAddress;
            r.FlatDesign = salesPersonal.FlatDesign;
            r.AffectedSERS = salesPersonal.AffectedSERS;
            r.AcceptanceDate = salesPersonal.AcceptanceDate;
            //r.ForwardedDate = salesPersonal.ForwardedDate;    //ForwardedDate is replaced to ApplicationDate by Edward 12/3/2014  Sales and Resale Changes
            r.ApplicationDate = salesPersonal.ApplicationDate;

            r.ApplicantType = (String.IsNullOrEmpty(salesPersonal.ApplicantType) ? string.Empty : salesPersonal.ApplicantType);
            r.CustomerId = salesPersonal.CustomerId;
            r.Nric = salesPersonal.Nric;
            r.Name = salesPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(salesPersonal.MaritalStatusCode) ? string.Empty : salesPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(salesPersonal.MaritalStatus) ? string.Empty : salesPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(salesPersonal.RelationshipCode) ? string.Empty : salesPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(salesPersonal.Relationship) ? string.Empty : salesPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(salesPersonal.CitizenshipCode) ? string.Empty : salesPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(salesPersonal.Citizenship) ? string.Empty : salesPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(salesPersonal.DateOfBirth) || salesPersonal.DateOfBirth.Equals(".") ? string.Empty : salesPersonal.DateOfBirth);
            r.Income = (String.IsNullOrEmpty(salesPersonal.Income) ? string.Empty : salesPersonal.Income);
            r.IsMCPS = salesPersonal.IsMCPS;
            r.IsPPO = salesPersonal.IsPPO;
            r.NRICSOC = (String.IsNullOrEmpty(salesPersonal.nricSOC) ? string.Empty : salesPersonal.nricSOC);
            r.NameSOC = (String.IsNullOrEmpty(salesPersonal.nameSOC) ? string.Empty : salesPersonal.nameSOC);

            r.IsMainApplicant = salesPersonal.IsMainApplicant;
            r.OrderNo = salesPersonal.OrderNo;

            //Added By Edward 19/2/2014
            r.NoOfIncomeMonths = salesPersonal.NoOfIncomeMonths;

            #region Added By Edward 11/3/2014 Sales and Resale Changes 
            r.OIC1 = (String.IsNullOrEmpty(salesPersonal.OIC1) ? string.Empty : salesPersonal.OIC1);
            r.OIC2 = (String.IsNullOrEmpty(salesPersonal.OIC2) ? string.Empty : salesPersonal.OIC2);
            r.Inc1Date = salesPersonal.Date1;
            r.Inc1 = salesPersonal.Inc1;
            r.Inc2Date = salesPersonal.Date2;
            r.Inc2 = salesPersonal.Inc2;
            r.Inc3Date = salesPersonal.Date3;
            r.Inc3 = salesPersonal.Inc3;
            r.Inc4Date = salesPersonal.Date4;
            r.Inc4 = salesPersonal.Inc4;
            r.Inc5Date = salesPersonal.Date5;
            r.Inc5 = salesPersonal.Inc5;
            r.Inc6Date = salesPersonal.Date6;
            r.Inc6 = salesPersonal.Inc6;
            r.Inc7Date = salesPersonal.Date7;
            r.Inc7 = salesPersonal.Inc7;
            r.Inc8Date = salesPersonal.Date8;
            r.Inc8 = salesPersonal.Inc8;
            r.Inc9Date = salesPersonal.Date9;
            r.Inc9 = salesPersonal.Inc9;
            r.Inc10Date = salesPersonal.Date10;
            r.Inc10 = salesPersonal.Inc10;
            r.Inc11Date = salesPersonal.Date11;
            r.Inc11 = salesPersonal.Inc11;
            r.Inc12Date = salesPersonal.Date12;
            r.Inc12 = salesPersonal.Inc12;
            #endregion
            r.KeyIssueDate = salesPersonal.KeyIssueDate;
            r.CancelledDate = salesPersonal.CancelledDate;
            r.ABCDERefNumber = salesPersonal.ABCDERefNumber;    //Added By Edward 2015/02/04

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description        
        //Edward                            2015/02/04          Added ABCDERefNumber Field in Application
        public bool UpdateApplication(string refNumber, string refType, string status, string ballotQuarter, string householdType, string allocationMode,
            string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, 
            string bookedAddress,string flatDesign, string contactNumbers, string affectedSERS, 
            string acceptanceDate, string forwardedDate, string keyIssueDate, string cancelledDate, string ABCDERefNumber)
        {
            //return Adapter.UpdateSalesInterfaceInfo(householdType, AHGStatus, refNumber, refType, status, ballotQuarter, allocationMode,
            //        allocationScheme, eligibilityScheme, householdIncome, flatType, SHGStatus, loanTag, bookedAddress,
            //        flatDesign, contactNumbers, affectedSERS, acceptanceDate, forwardedDate) > 0;
            return Adapter.UpdateSalesInterfaceInfo(householdType, AHGStatus, allocationMode, allocationScheme, householdIncome, eligibilityScheme,
                ballotQuarter, status, flatType, SHGStatus, loanTag, bookedAddress, flatDesign, affectedSERS, 
                acceptanceDate, forwardedDate, keyIssueDate, cancelledDate, ABCDERefNumber, refNumber) > 0;
    
        }

        public bool UpdateOrderNo(string refNo, string nric, int orderNo)
        {
            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Count == 0) return false;

            SalesInterface.SalesInterfaceRow r = dt[0];

            r.OrderNo = orderNo;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateOrderNos(string refNo, string applicantType, bool sortByOrderNo)
        {
            SalesInterface.SalesInterfaceDataTable dt = (sortByOrderNo ?
                GetSalesInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, applicantType) :
                GetSalesInterfaceByRefNoAndApplicantTypeSortByNric(refNo, applicantType));

            if (dt.Count == 0) return false;

            int orderNo = 1;
            foreach (SalesInterface.SalesInterfaceRow r in dt)
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
            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, "HA");

            // Set the main applicant as the first personal in the order
            foreach (SalesInterface.SalesInterfaceRow r in dt)
            {
                if (r.IsMainApplicant)
                {
                    r.OrderNo = 1;

                    int rowsAffected = Adapter.Update(dt);
                    break;
                }
            }

            // Update all the other order nos by incrementing by 1
            foreach (SalesInterface.SalesInterfaceRow r in dt)
            {
                if (!r.IsMainApplicant)
                {
                    r.OrderNo = r.OrderNo++;

                    int rowsAffected = Adapter.Update(dt);
                }
            }

            // Update all the order numbers of all the HA applicants to make the numbers in sequence
            return UpdateOrderNos(refNo, "HA", true);
        }

        /// <summary>
        /// Update all the OC occupiers order nos after the records are inserted using the LEAS web service
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public bool UpdateOrderNosOfOcOccupiers(string refNo)
        {
            return UpdateOrderNos(refNo, "OC", true);
        }



        #region Added By Edward Sales and Resales Changes    29/01/2014
        public bool UpdateNoOfIncomeMonths(string refNo, string nric, int NoOfIncomeMonths)
        {
            SalesInterface.SalesInterfaceDataTable dt = GetSalesInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Count == 0) return false;

            SalesInterface.SalesInterfaceRow r = dt[0];

            r.NoOfIncomeMonths = NoOfIncomeMonths;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #endregion

        #region Delete
        public bool DeleteByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.DeleteByRefNoAndNric(refNo, nric) > 0;
        }

        public bool DeleteByRefNo(string refNo)
        {
            return Adapter.DeleteByRefNo(refNo) > 0;
        }
        #endregion
    }
}
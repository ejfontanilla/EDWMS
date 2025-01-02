using System;
using System.Collections.Generic;
using System.Web;
using ResaleInterfaceTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResaleInterfaceDb
    /// </summary>
    public class ResaleInterfaceDb
    {
        private ResaleInterfaceTableAdapter _ResaleInterfaceAdapter = null;

        protected ResaleInterfaceTableAdapter Adapter
        {
            get
            {
                if (_ResaleInterfaceAdapter == null)
                    _ResaleInterfaceAdapter = new ResaleInterfaceTableAdapter();

                return _ResaleInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces
        /// </summary>
        /// <returns></returns>
        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterface()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get data by case no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <returns></returns>
        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByRefNo(string refNo)
        {
            return Adapter.GetDataByCaseNo(refNo);
        }


        /// <summary>
        /// Get data by case no and nric
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByRefNoAndNric(string refNo, string nric, string applicantType)
        {
            return Adapter.GetDataByCaseNoAndNric(refNo, nric, applicantType);
        }

        /// <summary>
        /// Get data by nric
        /// </summary>
        /// <param name="nric"></param>
        /// <returns></returns>
        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByNric(string nric)
        {
            return Adapter.GetDataByNric(nric);
        }

        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByByRefNoAndCustomerId(string refNo, string CustomerId)
        {
            return Adapter.GetDataByCaseNoAndCustomerId(refNo, CustomerId);
        }


        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByCaseNoAndName(string refNo, string name, string applicantType)
        {
            return Adapter.GetDataByCaseNoAndName(name, refNo, applicantType);
        }

        /// <summary>
        /// Check if personal exists in a case no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public bool DoesPersonalExistsByRefNo(string refNo)
        {
            return (GetResaleInterfaceByRefNo(refNo).Rows.Count > 0);
        }

        public bool DoesPersonalExistsByCaseNoNric(string caseNo, string nric, string applicantType)
        {
            return GetResaleInterfaceByRefNoAndNric(caseNo, nric, applicantType).Rows.Count > 0;
        }

        public bool DoesPersonalExistsByRefNoCustomerId(string refNo, string CustomerId)
        {
            return (GetResaleInterfaceByByRefNoAndCustomerId(refNo, CustomerId).Rows.Count > 0);
        }

        public bool DoesPersonalExistsByCaseNoName(string caseNo, string name, string applicantType)
        {
            return GetResaleInterfaceByCaseNoAndName(caseNo, name, applicantType).Rows.Count > 0;
        }

        /// <summary>
        /// Get Oic names, search for peOic or CaOic
        /// Created by Sandeep
        /// Date: 2012-12-04
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public string GetOICEmailRecipientByHleNumber(string caseNo)
        {
            string result = Adapter.GetOICEmailRecipientByCaseNo(caseNo).ToString();
            return (string.IsNullOrEmpty(result) ? null : result);
        }

        /// <summary>
        /// Get the Case OIC of the App
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public string GetCaseOIC(string refNo)
        {
            string result = string.Empty;

            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNo(refNo);

            if (dt.Rows.Count > 0)
            {
                ResaleInterface.ResaleInterfaceRow dr = dt[0];
                result = dr.Oic;
            }

            return result;
        }


        //GetDataByCaseNo
        public ResaleInterface.ResaleInterfaceDataTable GetApplicantDetailsByRefNo(string refNo)
        {
            return Adapter.GetDataByCaseNo(refNo);
        }

        //public string GetApplicantType(string refNo, string nric)
        //{
        //    string result = string.Empty;

        //    ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(refNo, nric);

        //    if (dt.Rows.Count > 0)
        //    {
        //        ResaleInterface.ResaleInterfaceRow dr = dt[0];
        //        result = dr.ApplicantType;
        //    }

        //    return result;
        //}

        public int GetLastOrderNoByApplicantType(string refNo, string applicantType)
        {
            int? result = Adapter.GetLastOrderNoForCaseNoByApplicantType(refNo, applicantType);

            if (result.HasValue)
                return result.Value;

            return 0;
        }

        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByRefNoAndApplicantTypeSortByOrderNo(string refNo, string applicantType)
        {
            return Adapter.GetDataByCaseNoAndApplicantTypeSortByOrderNo(refNo, applicantType);
        }

        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByRefNoAndApplicantTypeSortByNric(string refNo, string applicantType)
        {
            return Adapter.GetDataByCaseNoAndApplicantTypeSortByNric(refNo, applicantType);
        }

        public int GetOrderNumberForNric(string refNo, string applicantType, string nric)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(refNo, nric, applicantType);

            if (dt.Rows.Count > 0)
            {
                ResaleInterface.ResaleInterfaceRow dr = dt[0];
                return dr.OrderNo;
            }

            return GetLastOrderNoByApplicantType(refNo, applicantType) + 1;
        }

        public bool IsMainApplicant(string refNo, string nric)
        {
            bool isMainApplicant = false;

            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                ResaleInterface.ResaleInterfaceRow dr = dt[0];
                isMainApplicant = dr.IsMainApplicant;
            }

            return isMainApplicant;
        }

        public ResaleInterface.ResaleInterfaceDataTable GetResaleInterfaceByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.GetDataByCaseNoNric(refNo, nric);
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="streetCode"></param>
        /// <param name="streetName"></param>
        /// <returns></returns>
        public int Insert(ResalePersonal resalePersonal)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = new ResaleInterface.ResaleInterfaceDataTable();
            ResaleInterface.ResaleInterfaceRow r = dt.NewResaleInterfaceRow();

            r.CaseNo = resalePersonal.CaseNo;
            r.NumSchAccnt = (String.IsNullOrEmpty(resalePersonal.NumSchAccnt) ? string.Empty : resalePersonal.NumSchAccnt);
            r.OldLesseeCode = (String.IsNullOrEmpty(resalePersonal.OldLesseeCode) ? string.Empty : resalePersonal.OldLesseeCode);
            r.NewLesseeCode = (String.IsNullOrEmpty(resalePersonal.NewLesseeCode) ? string.Empty : resalePersonal.NewLesseeCode);
            r.AllocBuyer = (String.IsNullOrEmpty(resalePersonal.AllocBuyer) ? string.Empty : resalePersonal.AllocBuyer);
            r.EligBuyer = (String.IsNullOrEmpty(resalePersonal.EligBuyer) ? string.Empty : resalePersonal.EligBuyer);
            r.CodeRecipient = (String.IsNullOrEmpty(resalePersonal.CodeRecipient) ? string.Empty : resalePersonal.CodeRecipient);
            r.Roic = (String.IsNullOrEmpty(resalePersonal.Roic) ? string.Empty : resalePersonal.Roic);
            r.Oic = (String.IsNullOrEmpty(resalePersonal.Oic) ? string.Empty : resalePersonal.Oic);
            r.EligibilityScheme = (String.IsNullOrEmpty(resalePersonal.EligibilityScheme) ? string.Empty : resalePersonal.EligibilityScheme);
            r.HouseholdType = (String.IsNullOrEmpty(resalePersonal.HouseholdType) ? string.Empty : resalePersonal.HouseholdType);
            r.AhgGrant = (String.IsNullOrEmpty(resalePersonal.AhgGrant) ? string.Empty : resalePersonal.AhgGrant);
            r.HHInc2Year = (String.IsNullOrEmpty(resalePersonal.HhInc2Year) ? string.Empty : resalePersonal.HhInc2Year);
            r.BankLoan = resalePersonal.BankLoan;
            r.ABCDELoan = resalePersonal.ABCDELoan;
            r.HouseHoldInc = (String.IsNullOrEmpty(resalePersonal.HouseHoldInc) ? string.Empty : resalePersonal.HouseHoldInc);
            r.CaInc = (String.IsNullOrEmpty(resalePersonal.CaInc) ? string.Empty : resalePersonal.CaInc);
            r.ApplicationDate = (String.IsNullOrEmpty(resalePersonal.ApplicationDate) ? string.Empty : resalePersonal.ApplicationDate);
            r.AppointmentDate = (String.IsNullOrEmpty(resalePersonal.AppointmentDate) ? string.Empty : resalePersonal.AppointmentDate);
            r.CompletionDate = (String.IsNullOrEmpty(resalePersonal.CompletionDate) ? string.Empty : resalePersonal.CompletionDate);
            r.CancellationDate = (String.IsNullOrEmpty(resalePersonal.CancellationDate) ? string.Empty : resalePersonal.CancellationDate);

            r.ApplicantType = (String.IsNullOrEmpty(resalePersonal.ApplicantType) ? string.Empty : resalePersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(resalePersonal.ApplicantType) ? string.Empty : resalePersonal.CustomerId);
            r.Nric = resalePersonal.Nric;
            r.Name = resalePersonal.Name;
            r.MaritalStatusCode = resalePersonal.MaritalStatusCode;
            r.MaritalStatus = resalePersonal.MaritalStatus;
            r.RelationshipCode = resalePersonal.RelationshipCode;
            r.Relationship = resalePersonal.Relationship;
            r.CitizenshipCode = (String.IsNullOrEmpty(resalePersonal.CitizenshipCode) ? string.Empty : resalePersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(resalePersonal.Citizenship) ? string.Empty : resalePersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(resalePersonal.DateOfBirth) || resalePersonal.DateOfBirth.Equals(".") ? string.Empty : resalePersonal.DateOfBirth);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(resalePersonal.EmploymentTypeCode) ? string.Empty : resalePersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(resalePersonal.EmploymentType) ? string.Empty : resalePersonal.EmploymentType);
            r.Inc1Date = (String.IsNullOrEmpty(resalePersonal.Date1) ? string.Empty : resalePersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(resalePersonal.Inc1) ? string.Empty : resalePersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(resalePersonal.Date2) ? string.Empty : resalePersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(resalePersonal.Inc2) ? string.Empty : resalePersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(resalePersonal.Date3) ? string.Empty : resalePersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(resalePersonal.Inc3) ? string.Empty : resalePersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(resalePersonal.Date4) ? string.Empty : resalePersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(resalePersonal.Inc4) ? string.Empty : resalePersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(resalePersonal.Date5) ? string.Empty : resalePersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(resalePersonal.Inc5) ? string.Empty : resalePersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(resalePersonal.Date6) ? string.Empty : resalePersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(resalePersonal.Inc6) ? string.Empty : resalePersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(resalePersonal.Date7) ? string.Empty : resalePersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(resalePersonal.Inc7) ? string.Empty : resalePersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(resalePersonal.Date8) ? string.Empty : resalePersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(resalePersonal.Inc8) ? string.Empty : resalePersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(resalePersonal.Date9) ? string.Empty : resalePersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(resalePersonal.Inc9) ? string.Empty : resalePersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(resalePersonal.Date10) ? string.Empty : resalePersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(resalePersonal.Inc10) ? string.Empty : resalePersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(resalePersonal.Date11) ? string.Empty : resalePersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(resalePersonal.Inc11) ? string.Empty : resalePersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(resalePersonal.Date12) ? string.Empty : resalePersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(resalePersonal.Inc12) ? string.Empty : resalePersonal.Inc12);
            r.Inc13Date = (String.IsNullOrEmpty(resalePersonal.Date13) ? string.Empty : resalePersonal.Date13);
            r.Inc13 = (String.IsNullOrEmpty(resalePersonal.Inc13) ? string.Empty : resalePersonal.Inc13);
            r.Inc14Date = (String.IsNullOrEmpty(resalePersonal.Date14) ? string.Empty : resalePersonal.Date14);
            r.Inc14 = (String.IsNullOrEmpty(resalePersonal.Inc14) ? string.Empty : resalePersonal.Inc14);
            r.Inc15Date = (String.IsNullOrEmpty(resalePersonal.Date15) ? string.Empty : resalePersonal.Date15);
            r.Inc15 = (String.IsNullOrEmpty(resalePersonal.Inc15) ? string.Empty : resalePersonal.Inc15);

            r.IsMainApplicant = resalePersonal.IsMainApplicant;
            r.OrderNo = resalePersonal.OrderNo;

            //added By Edward 19/2/2014 
            r.NoOfIncomeMonths = resalePersonal.NoOfIncomeMonths;

            dt.AddResaleInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.ResaleInterface, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }

        //Added By Edward 2014/05/05    Batch Upload
        public int Insert(ResalePersonal resalePersonal, string ip, string systemInfo)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = new ResaleInterface.ResaleInterfaceDataTable();
            ResaleInterface.ResaleInterfaceRow r = dt.NewResaleInterfaceRow();

            r.CaseNo = resalePersonal.CaseNo;
            r.NumSchAccnt = (String.IsNullOrEmpty(resalePersonal.NumSchAccnt) ? string.Empty : resalePersonal.NumSchAccnt);
            r.OldLesseeCode = (String.IsNullOrEmpty(resalePersonal.OldLesseeCode) ? string.Empty : resalePersonal.OldLesseeCode);
            r.NewLesseeCode = (String.IsNullOrEmpty(resalePersonal.NewLesseeCode) ? string.Empty : resalePersonal.NewLesseeCode);
            r.AllocBuyer = (String.IsNullOrEmpty(resalePersonal.AllocBuyer) ? string.Empty : resalePersonal.AllocBuyer);
            r.EligBuyer = (String.IsNullOrEmpty(resalePersonal.EligBuyer) ? string.Empty : resalePersonal.EligBuyer);
            r.CodeRecipient = (String.IsNullOrEmpty(resalePersonal.CodeRecipient) ? string.Empty : resalePersonal.CodeRecipient);
            r.Roic = (String.IsNullOrEmpty(resalePersonal.Roic) ? string.Empty : resalePersonal.Roic);
            r.Oic = (String.IsNullOrEmpty(resalePersonal.Oic) ? string.Empty : resalePersonal.Oic);
            r.EligibilityScheme = (String.IsNullOrEmpty(resalePersonal.EligibilityScheme) ? string.Empty : resalePersonal.EligibilityScheme);
            r.HouseholdType = (String.IsNullOrEmpty(resalePersonal.HouseholdType) ? string.Empty : resalePersonal.HouseholdType);
            r.AhgGrant = (String.IsNullOrEmpty(resalePersonal.AhgGrant) ? string.Empty : resalePersonal.AhgGrant);
            r.HHInc2Year = (String.IsNullOrEmpty(resalePersonal.HhInc2Year) ? string.Empty : resalePersonal.HhInc2Year);
            r.BankLoan = resalePersonal.BankLoan;
            r.ABCDELoan = resalePersonal.ABCDELoan;
            r.HouseHoldInc = (String.IsNullOrEmpty(resalePersonal.HouseHoldInc) ? string.Empty : resalePersonal.HouseHoldInc);
            r.CaInc = (String.IsNullOrEmpty(resalePersonal.CaInc) ? string.Empty : resalePersonal.CaInc);
            r.ApplicationDate = (String.IsNullOrEmpty(resalePersonal.ApplicationDate) ? string.Empty : resalePersonal.ApplicationDate);
            r.AppointmentDate = (String.IsNullOrEmpty(resalePersonal.AppointmentDate) ? string.Empty : resalePersonal.AppointmentDate);
            r.CompletionDate = (String.IsNullOrEmpty(resalePersonal.CompletionDate) ? string.Empty : resalePersonal.CompletionDate);
            r.CancellationDate = (String.IsNullOrEmpty(resalePersonal.CancellationDate) ? string.Empty : resalePersonal.CancellationDate);

            r.ApplicantType = (String.IsNullOrEmpty(resalePersonal.ApplicantType) ? string.Empty : resalePersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(resalePersonal.ApplicantType) ? string.Empty : resalePersonal.CustomerId);
            r.Nric = resalePersonal.Nric;
            r.Name = resalePersonal.Name;
            r.MaritalStatusCode = resalePersonal.MaritalStatusCode;
            r.MaritalStatus = resalePersonal.MaritalStatus;
            r.RelationshipCode = resalePersonal.RelationshipCode;
            r.Relationship = resalePersonal.Relationship;
            r.CitizenshipCode = (String.IsNullOrEmpty(resalePersonal.CitizenshipCode) ? string.Empty : resalePersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(resalePersonal.Citizenship) ? string.Empty : resalePersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(resalePersonal.DateOfBirth) || resalePersonal.DateOfBirth.Equals(".") ? string.Empty : resalePersonal.DateOfBirth);
            r.EmploymentTypeCode = (String.IsNullOrEmpty(resalePersonal.EmploymentTypeCode) ? string.Empty : resalePersonal.EmploymentTypeCode);
            r.EmploymentType = (String.IsNullOrEmpty(resalePersonal.EmploymentType) ? string.Empty : resalePersonal.EmploymentType);
            r.Inc1Date = (String.IsNullOrEmpty(resalePersonal.Date1) ? string.Empty : resalePersonal.Date1);
            r.Inc1 = (String.IsNullOrEmpty(resalePersonal.Inc1) ? string.Empty : resalePersonal.Inc1);
            r.Inc2Date = (String.IsNullOrEmpty(resalePersonal.Date2) ? string.Empty : resalePersonal.Date2);
            r.Inc2 = (String.IsNullOrEmpty(resalePersonal.Inc2) ? string.Empty : resalePersonal.Inc2);
            r.Inc3Date = (String.IsNullOrEmpty(resalePersonal.Date3) ? string.Empty : resalePersonal.Date3);
            r.Inc3 = (String.IsNullOrEmpty(resalePersonal.Inc3) ? string.Empty : resalePersonal.Inc3);
            r.Inc4Date = (String.IsNullOrEmpty(resalePersonal.Date4) ? string.Empty : resalePersonal.Date4);
            r.Inc4 = (String.IsNullOrEmpty(resalePersonal.Inc4) ? string.Empty : resalePersonal.Inc4);
            r.Inc5Date = (String.IsNullOrEmpty(resalePersonal.Date5) ? string.Empty : resalePersonal.Date5);
            r.Inc5 = (String.IsNullOrEmpty(resalePersonal.Inc5) ? string.Empty : resalePersonal.Inc5);
            r.Inc6Date = (String.IsNullOrEmpty(resalePersonal.Date6) ? string.Empty : resalePersonal.Date6);
            r.Inc6 = (String.IsNullOrEmpty(resalePersonal.Inc6) ? string.Empty : resalePersonal.Inc6);
            r.Inc7Date = (String.IsNullOrEmpty(resalePersonal.Date7) ? string.Empty : resalePersonal.Date7);
            r.Inc7 = (String.IsNullOrEmpty(resalePersonal.Inc7) ? string.Empty : resalePersonal.Inc7);
            r.Inc8Date = (String.IsNullOrEmpty(resalePersonal.Date8) ? string.Empty : resalePersonal.Date8);
            r.Inc8 = (String.IsNullOrEmpty(resalePersonal.Inc8) ? string.Empty : resalePersonal.Inc8);
            r.Inc9Date = (String.IsNullOrEmpty(resalePersonal.Date9) ? string.Empty : resalePersonal.Date9);
            r.Inc9 = (String.IsNullOrEmpty(resalePersonal.Inc9) ? string.Empty : resalePersonal.Inc9);
            r.Inc10Date = (String.IsNullOrEmpty(resalePersonal.Date10) ? string.Empty : resalePersonal.Date10);
            r.Inc10 = (String.IsNullOrEmpty(resalePersonal.Inc10) ? string.Empty : resalePersonal.Inc10);
            r.Inc11Date = (String.IsNullOrEmpty(resalePersonal.Date11) ? string.Empty : resalePersonal.Date11);
            r.Inc11 = (String.IsNullOrEmpty(resalePersonal.Inc11) ? string.Empty : resalePersonal.Inc11);
            r.Inc12Date = (String.IsNullOrEmpty(resalePersonal.Date12) ? string.Empty : resalePersonal.Date12);
            r.Inc12 = (String.IsNullOrEmpty(resalePersonal.Inc12) ? string.Empty : resalePersonal.Inc12);
            r.Inc13Date = (String.IsNullOrEmpty(resalePersonal.Date13) ? string.Empty : resalePersonal.Date13);
            r.Inc13 = (String.IsNullOrEmpty(resalePersonal.Inc13) ? string.Empty : resalePersonal.Inc13);
            r.Inc14Date = (String.IsNullOrEmpty(resalePersonal.Date14) ? string.Empty : resalePersonal.Date14);
            r.Inc14 = (String.IsNullOrEmpty(resalePersonal.Inc14) ? string.Empty : resalePersonal.Inc14);
            r.Inc15Date = (String.IsNullOrEmpty(resalePersonal.Date15) ? string.Empty : resalePersonal.Date15);
            r.Inc15 = (String.IsNullOrEmpty(resalePersonal.Inc15) ? string.Empty : resalePersonal.Inc15);

            r.IsMainApplicant = resalePersonal.IsMainApplicant;
            r.OrderNo = resalePersonal.OrderNo;

            //added By Edward 19/2/2014 
            r.NoOfIncomeMonths = resalePersonal.NoOfIncomeMonths;

            dt.AddResaleInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.ResaleInterface, id.ToString(), OperationTypeEnum.Insert, ip, systemInfo);
            }

            return id;
        }



        ///// <summary>
        ///// Insert
        ///// </summary>
        ///// <param name="rosPersonal"></param>
        ///// <returns></returns>
        //public int Insert(ResalePersonal rosPersonal)
        //{
        //    ResaleInterface.ResaleInterfaceDataTable dt = new ResaleInterface.ResaleInterfaceDataTable();
        //    ResaleInterface.ResaleInterfaceRow r = dt.NewResaleInterfaceRow();

        //    r.CaseNo = rosPersonal.CaseNo;
        //    r.Nric = rosPersonal.Nric;
        //    r.Name = rosPersonal.Name;
        //    r.MaritalStatusCode = (String.IsNullOrEmpty(rosPersonal.MaritalStatusCode) ? string.Empty : rosPersonal.MaritalStatusCode);
        //    r.MaritalStatus = (String.IsNullOrEmpty(rosPersonal.MaritalStatus) ? string.Empty : rosPersonal.MaritalStatus);
        //    r.DateOfBirth = (String.IsNullOrEmpty(rosPersonal.DateOfBirth) || rosPersonal.DateOfBirth.Equals(".") ? string.Empty : rosPersonal.DateOfBirth);
        //    r.RelationshipCode = (String.IsNullOrEmpty(rosPersonal.RelationshipCode) ? string.Empty : rosPersonal.RelationshipCode);
        //    r.Relationship = (String.IsNullOrEmpty(rosPersonal.Relationship) ? string.Empty : rosPersonal.Relationship);
        //    r.Roic = (String.IsNullOrEmpty(rosPersonal.Roic) ? string.Empty : rosPersonal.Roic);
        //    r.Oic = (String.IsNullOrEmpty(rosPersonal.Oic) ? string.Empty : rosPersonal.Oic);
        //    r.AhgGrant = (String.IsNullOrEmpty(rosPersonal.AhgGrant) ? string.Empty : rosPersonal.AhgGrant);
        //    r.HHInc2Year = (String.IsNullOrEmpty(rosPersonal.HhInc2Year) ? string.Empty : rosPersonal.HhInc2Year);
        //    r.NumSchAccnt = (String.IsNullOrEmpty(rosPersonal.NumSchAccnt) ? string.Empty : rosPersonal.NumSchAccnt);
        //    r.OldLesseeCode = (String.IsNullOrEmpty(rosPersonal.OldLesseeCode) ? string.Empty : rosPersonal.OldLesseeCode);
        //    r.NewLesseeCode = (String.IsNullOrEmpty(rosPersonal.NewLesseeCode) ? string.Empty : rosPersonal.NewLesseeCode);
        //    r.AllocBuyer = (String.IsNullOrEmpty(rosPersonal.AllocBuyer) ? string.Empty : rosPersonal.AllocBuyer);
        //    r.EligBuyer = (String.IsNullOrEmpty(rosPersonal.EligBuyer) ? string.Empty : rosPersonal.EligBuyer);
        //    r.BankLoan = rosPersonal.BankLoan;
        //    r.ABCDELoan = rosPersonal.ABCDELoan;
        //    r.HouseHoldInc = (String.IsNullOrEmpty(rosPersonal.HhInc) ? string.Empty : rosPersonal.HhInc);
        //    r.CaInc = (String.IsNullOrEmpty(rosPersonal.CaInc) ? string.Empty : rosPersonal.CaInc);
        //    r.ApplicantType = (String.IsNullOrEmpty(rosPersonal.ApplicantType) ? string.Empty : rosPersonal.ApplicantType);
        //    r.OrderNo = rosPersonal.OrderNo;

        //    dt.AddResaleInterfaceRow(r);
        //    Adapter.Update(dt);
        //    int id = r.Id;

        //    if (id > 0)
        //    {
        //        AuditTrailDb auditTrailDb = new AuditTrailDb();
        //        auditTrailDb.Record(TableNameEnum.ResaleInterface, id.ToString(), OperationTypeEnum.Insert);
        //    }

        //    return id;
        //}
        #endregion

        #region Update Methods
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="rosPersonal"></param>
        /// <returns></returns>
        public bool Update(ResalePersonal rosPersonal)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(rosPersonal.CaseNo, rosPersonal.Nric, rosPersonal.ApplicantType);

            if (dt.Count == 0)
                dt = GetResaleInterfaceByCaseNoAndName(rosPersonal.CaseNo, rosPersonal.Name, rosPersonal.ApplicantType);

            if (dt.Count == 0) return false;

            ResaleInterface.ResaleInterfaceRow r = dt[0];

            r.CaseNo = rosPersonal.CaseNo;
            r.NumSchAccnt = (String.IsNullOrEmpty(rosPersonal.NumSchAccnt) ? string.Empty : rosPersonal.NumSchAccnt);
            r.OldLesseeCode = (String.IsNullOrEmpty(rosPersonal.OldLesseeCode) ? string.Empty : rosPersonal.OldLesseeCode);
            r.NewLesseeCode = (String.IsNullOrEmpty(rosPersonal.NewLesseeCode) ? string.Empty : rosPersonal.NewLesseeCode);
            r.AllocBuyer = (String.IsNullOrEmpty(rosPersonal.AllocBuyer) ? string.Empty : rosPersonal.AllocBuyer);
            r.EligBuyer = (String.IsNullOrEmpty(rosPersonal.EligBuyer) ? string.Empty : rosPersonal.EligBuyer);
            r.CodeRecipient = (String.IsNullOrEmpty(rosPersonal.CodeRecipient) ? string.Empty : rosPersonal.CodeRecipient);
            r.Roic = (String.IsNullOrEmpty(rosPersonal.Roic) ? string.Empty : rosPersonal.Roic);
            r.Oic = (String.IsNullOrEmpty(rosPersonal.Oic) ? string.Empty : rosPersonal.Oic);
            r.EligibilityScheme = (String.IsNullOrEmpty(rosPersonal.EligibilityScheme) ? string.Empty : rosPersonal.EligibilityScheme);
            r.HouseholdType = (String.IsNullOrEmpty(rosPersonal.HouseholdType) ? string.Empty : rosPersonal.HouseholdType);
            r.AhgGrant = (String.IsNullOrEmpty(rosPersonal.AhgGrant) ? string.Empty : rosPersonal.AhgGrant);
            r.HHInc2Year = (String.IsNullOrEmpty(rosPersonal.HhInc2Year) ? string.Empty : rosPersonal.HhInc2Year);
            r.BankLoan = (String.IsNullOrEmpty(rosPersonal.BankLoan) ? string.Empty : rosPersonal.BankLoan);    //Uncommented by Edward 11/3/2014 Sales and Resale Changes
            r.ABCDELoan = (String.IsNullOrEmpty(rosPersonal.ABCDELoan) ? string.Empty : rosPersonal.ABCDELoan);       //Uncommented by Edward 11/3/2014 Sales and Resale Changes
            r.HouseHoldInc = (String.IsNullOrEmpty(rosPersonal.HouseHoldInc) ? string.Empty : rosPersonal.HouseHoldInc);
            r.CaInc = (String.IsNullOrEmpty(rosPersonal.CaInc) ? string.Empty : rosPersonal.CaInc);
            r.ApplicationDate = (String.IsNullOrEmpty(rosPersonal.ApplicationDate) ? string.Empty : rosPersonal.ApplicationDate);
            r.AppointmentDate = (String.IsNullOrEmpty(rosPersonal.AppointmentDate) ? string.Empty : rosPersonal.AppointmentDate);
            r.CancellationDate = (String.IsNullOrEmpty(rosPersonal.CancellationDate) ? string.Empty : rosPersonal.CancellationDate);

            r.ApplicantType = (String.IsNullOrEmpty(rosPersonal.ApplicantType) ? string.Empty : rosPersonal.ApplicantType);
            r.CustomerId = rosPersonal.CustomerId;
            r.Nric = rosPersonal.Nric;
            r.Name = rosPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(rosPersonal.MaritalStatusCode) ? string.Empty : rosPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(rosPersonal.MaritalStatus) ? string.Empty : rosPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(rosPersonal.RelationshipCode) ? string.Empty : rosPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(rosPersonal.Relationship) ? string.Empty : rosPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(rosPersonal.CitizenshipCode) ? string.Empty : rosPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(rosPersonal.Citizenship) ? string.Empty : rosPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(rosPersonal.DateOfBirth) || rosPersonal.DateOfBirth.Equals(".") ? string.Empty : rosPersonal.DateOfBirth);

            r.EmploymentTypeCode = rosPersonal.EmploymentTypeCode;
            r.EmploymentType = rosPersonal.EmploymentType;
            r.Inc1Date = rosPersonal.Date1;
            r.Inc1 = rosPersonal.Inc1;
            r.Inc2Date = rosPersonal.Date2;
            r.Inc2 = rosPersonal.Inc2;
            r.Inc3Date = rosPersonal.Date3;
            r.Inc3 = rosPersonal.Inc3;
            r.Inc4Date = rosPersonal.Date4;
            r.Inc4 = rosPersonal.Inc4;
            r.Inc5Date = rosPersonal.Date5;
            r.Inc5 = rosPersonal.Inc5;
            r.Inc6Date = rosPersonal.Date6;
            r.Inc6 = rosPersonal.Inc6;
            r.Inc7Date = rosPersonal.Date7;
            r.Inc7 = rosPersonal.Inc7;
            r.Inc8Date = rosPersonal.Date8;
            r.Inc8 = rosPersonal.Inc8;
            r.Inc9Date = rosPersonal.Date9;
            r.Inc9 = rosPersonal.Inc9;
            r.Inc10Date = rosPersonal.Date10;
            r.Inc10 = rosPersonal.Inc10;
            r.Inc11Date = rosPersonal.Date11;
            r.Inc11 = rosPersonal.Inc11;
            r.Inc12Date = rosPersonal.Date12;
            r.Inc12 = rosPersonal.Inc12;
            r.Inc13Date = rosPersonal.Date13;
            r.Inc13 = rosPersonal.Inc13;
            r.Inc14Date = rosPersonal.Date14;
            r.Inc14 = rosPersonal.Inc14;
            r.Inc15Date = rosPersonal.Date15;
            r.Inc15 = rosPersonal.Inc15;

            r.IsMainApplicant = rosPersonal.IsMainApplicant;
            r.OrderNo = rosPersonal.OrderNo;
            //Added By Edward 19/2/2014 Sales and Resale Changes
            r.NoOfIncomeMonths = rosPersonal.NoOfIncomeMonths;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateApplication(string refNo, string Roic, string Oic, string AhgGrant, string HHInc2Year, string NumSchAccnt, string OldLesseeCode, string NewLesseeCode, string AllocBuyer,
            string EligBuyer, string BankLoan, string ABCDELoan, string cAInc, string CodeRecipient, string EligibilityScheme, string HouseholdType, 
            string ApplicationDate, string AppointmentDate, string CompletionDate, string CancellationDate, string HouseHoldInc )
        {
            return Adapter.UpdateResaleInterfaceInfo(Roic, Oic, AhgGrant, HHInc2Year, NumSchAccnt, OldLesseeCode, NewLesseeCode, AllocBuyer, EligBuyer, BankLoan, ABCDELoan, cAInc, CodeRecipient,
                EligibilityScheme, HouseholdType, ApplicationDate, AppointmentDate, CompletionDate, CancellationDate, HouseHoldInc, refNo) > 0;
        }

        //public bool UpdateOrderNo(string refNo, string nric, int orderNo)
        //{
        //    ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(refNo, nric);

        //    if (dt.Count == 0) return false;

        //    ResaleInterface.ResaleInterfaceRow r = dt[0];

        //    r.OrderNo = orderNo;

        //    int rowsAffected = Adapter.Update(dt);

        //    return (rowsAffected > 0);
        //}

        public bool UpdateOrderNos(string refNo, string applicantType, bool sortByOrderNo)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = (sortByOrderNo ?
                GetResaleInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, applicantType) :
                GetResaleInterfaceByRefNoAndApplicantTypeSortByNric(refNo, applicantType));

            if (dt.Count == 0) return false;

            int orderNo = 1;
            foreach (ResaleInterface.ResaleInterfaceRow r in dt)
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
        public bool UpdateOrderNosOfHaApplicants(string refNo, string applicantType)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, applicantType);

            // Set the main applicant as the first personal in the order
            foreach (ResaleInterface.ResaleInterfaceRow r in dt)
            {
                if (r.IsMainApplicant)
                {
                    r.OrderNo = 1;

                    int rowsAffected = Adapter.Update(dt);
                    break;
                }
            }

            // Update all the other order nos by incrementing by 1
            foreach (ResaleInterface.ResaleInterfaceRow r in dt)
            {
                if (!r.IsMainApplicant)
                {
                    r.OrderNo = r.OrderNo++;

                    int rowsAffected = Adapter.Update(dt);
                }
            }

            // Update all the order numbers of all the HA applicants to make the numbers in sequence
            return UpdateOrderNos(refNo, "BU", true);
        }

        /// <summary>
        /// Update all the OC occupiers order nos after the records are inserted using the LEAS web service
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public bool UpdateOrderNosOfOcOccupiers(string refNo, string applicantType)
        {
            return UpdateOrderNos(refNo, applicantType, true);
        }


        #region Added By Edward Sales and Resales Changes    29/01/2014
        public bool UpdateNoOfIncomeMonths(string refNo, string nric, string applicantType, int NoOfIncomeMonths)
        {
            ResaleInterface.ResaleInterfaceDataTable dt = GetResaleInterfaceByRefNoAndNric(refNo, nric, applicantType);       

            if (dt.Count == 0) return false;

            ResaleInterface.ResaleInterfaceRow r = dt[0];

            r.NoOfIncomeMonths = NoOfIncomeMonths;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }
        #endregion

        #endregion

        #region Delete
        //public void DeleteWrongRecords()
        //{
        //    HleInterfceDs.DeleteWrongRecords();
        //}

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
using System;
using System.Collections.Generic;
using System.Web;
using SersInterfaceTableAdapters;
using System.Data;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResaleInterfaceDb
    /// </summary>
    public class SersInterfaceDb
    {
        private SersInterfaceTableAdapter _SersInterfaceAdapter = null;

        protected SersInterfaceTableAdapter Adapter
        {
            get
            {
                if (_SersInterfaceAdapter == null)
                    _SersInterfaceAdapter = new SersInterfaceTableAdapter();

                return _SersInterfaceAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces
        /// </summary>
        /// <returns></returns>
        public SersInterface.SersInterfaceDataTable GetSersInterface()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get address by reference number
        /// </summary>
        /// <param name="refNo"></param>
        /// <returns></returns>
        public string GetSersAddressByRefNo(string refNo)
        {
            System.Text.StringBuilder address = new System.Text.StringBuilder();
            SersInterface.SersInterfaceDataTable sersInterfaces = Adapter.GetDataBySchAcc(refNo);

            if (sersInterfaces.Rows.Count >0)
            {
                SersInterface.SersInterfaceRow sersInterfaceRow = sersInterfaces[0];
                return "Block " + sersInterfaceRow.Block + ", #" + sersInterfaceRow.Level + "-" + sersInterfaceRow.Unit + ", " + Format.ToTitleCase(sersInterfaceRow.Street.ToLower()) + ", S" + sersInterfaceRow.PostalCode;
            }
            else
                return "-";
        }

        /// <summary>
        /// Get data by ref no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <returns></returns>
        public SersInterface.SersInterfaceDataTable GetSersInterfaceByRefNo(string refNo)
        {
            return Adapter.GetDataBySchAcc(refNo);
        }

        /// <summary>
        /// Get the household structure
        /// Created by Sandeep
        /// Date: 2012-09-07
        /// </summary>
        /// <param name="hleNumber"></param>
        /// <returns></returns>
        public SersInterface.SersInterfaceDataTable GetApplicantDetailsByRefNo(string refNo)
        {
            return Adapter.GetDataBySchAcc(refNo);
        }

        /// <summary>
        /// Get data by ref no and nric
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public SersInterface.SersInterfaceDataTable GetSersInterfaceByRefNoAndNric(string refNo, string nric)
        {
            return Adapter.GetDataBySchAccAndNric(refNo, nric);
        }

        /// <summary>
        /// Get data by nric
        /// </summary>
        /// <param name="nric"></param>
        /// <returns></returns>
        public SersInterface.SersInterfaceDataTable GetSersInterfaceByNric(string nric)
        {
            return Adapter.GetDataByNric(nric);
        }

        public SersInterface.SersInterfaceDataTable GetSersInterfaceByByRefNoAndCustomerId(string refNo, string CustomerId)
        {
            return Adapter.GetDataBySchAccAndCustomerId(refNo, CustomerId);
        }


        public SersInterface.SersInterfaceDataTable GetSersInterfaceByRefNoAndName(string refNo, string name)
        {
            return Adapter.GetDataBySchAccAndName(refNo, name);
        }

        /// <summary>
        /// Check if personal exists in a ref no
        /// </summary>
        /// <param name="caseNo"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public bool DoesPersonalExistsByRefNo(string refNo)
        {
            return (GetSersInterfaceByRefNo(refNo).Rows.Count > 0);
        }

        public bool DoesPersonalExistsBySchAccNric(string refNo, string nric)
        {
            return GetSersInterfaceByRefNoAndNric(refNo, nric).Rows.Count > 0;
        }

        public bool DoesPersonalExistsByRefNoCustomerId(string refNo, string CustomerId)
        {
            return (GetSersInterfaceByByRefNoAndCustomerId(refNo, CustomerId).Rows.Count > 0);
        }

        public bool DoesPersonalExistsBySchAccName(string refNo, string name)
        {
            return GetSersInterfaceByRefNoAndName(refNo, name).Rows.Count > 0;
        }

        public string GetApplicantType(string refNo, string nric)
        {
            string result = string.Empty;

            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SersInterface.SersInterfaceRow dr = dt[0];
                result = dr.ApplicantType;
            }

            return result;
        }

        public int GetLastOrderNoByApplicantType(string refNo, string applicantType)
        {
            int? result = Adapter.GetLastOrderNoForSchAccByApplicantType(refNo, applicantType);

            if (result.HasValue)
                return result.Value;

            return 0;
        }

        public SersInterface.SersInterfaceDataTable GetSersInterfaceByRefNoAndApplicantTypeSortByOrderNo(string refNo, string applicantType)
        {
            return Adapter.GetDataBySchAccAndApplicantTypeSortByOrderNo(refNo, applicantType);
        }

        public SersInterface.SersInterfaceDataTable GetSersInterfaceByRefNoAndApplicantTypeSortByNric(string refNo, string applicantType)
        {
            return Adapter.GetDataBySchAccAndApplicantTypeSortByNric(refNo, applicantType);
        }

        public int GetOrderNumberForNric(string refNo, string applicantType, string nric)
        {
            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SersInterface.SersInterfaceRow dr = dt[0];
                return dr.OrderNo;
            }

            return GetLastOrderNoByApplicantType(refNo, applicantType) + 1;
        }

        public bool IsMainApplicant(string refNo, string nric)
        {
            bool isMainApplicant = false;

            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Rows.Count > 0)
            {
                SersInterface.SersInterfaceRow dr = dt[0];
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
        public int Insert(SersPersonal sersPersonal)
        {
            SersInterface.SersInterfaceDataTable dt = new SersInterface.SersInterfaceDataTable();
            SersInterface.SersInterfaceRow r = dt.NewSersInterfaceRow();

            r.SchAcc = sersPersonal.SchAcc;
            r.RegistrationNumber = sersPersonal.RegistrationNumber;

            r.Stage = (String.IsNullOrEmpty(sersPersonal.Stage) ? string.Empty : sersPersonal.Stage);
            r.Block = (String.IsNullOrEmpty(sersPersonal.Block) ? string.Empty : sersPersonal.Block);
            r.Street = (String.IsNullOrEmpty(sersPersonal.Street) ? string.Empty : sersPersonal.Street);
            r.Level = (String.IsNullOrEmpty(sersPersonal.Level) ? string.Empty : sersPersonal.Level);
            r.Unit = (String.IsNullOrEmpty(sersPersonal.Unit) ? string.Empty : sersPersonal.Unit);
            r.PostalCode = (String.IsNullOrEmpty(sersPersonal.Postal) ? string.Empty : sersPersonal.Postal);
            r.Alloc = (String.IsNullOrEmpty(sersPersonal.Alloc) ? string.Empty : sersPersonal.Alloc);
            r.Elig = (String.IsNullOrEmpty(sersPersonal.Ellig) ? string.Empty : sersPersonal.Ellig);

            r.ApplicantType = (String.IsNullOrEmpty(sersPersonal.ApplicantType) ? string.Empty : sersPersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(sersPersonal.CustomerId) ? string.Empty : sersPersonal.CustomerId);
            r.Nric = sersPersonal.Nric;
            r.Name = sersPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(sersPersonal.MaritalStatusCode) ? string.Empty : sersPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(sersPersonal.MaritalStatus) ? string.Empty : sersPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(sersPersonal.RelationshipCode) ? string.Empty : sersPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(sersPersonal.Relationship) ? string.Empty : sersPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(sersPersonal.CitizenshipCode) ? string.Empty : sersPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(sersPersonal.Citizenship) ? string.Empty : sersPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(sersPersonal.DateOfBirth) || sersPersonal.DateOfBirth.Equals(".") ? string.Empty : sersPersonal.DateOfBirth);
            r.PrivatePropertyOwner = sersPersonal.PrivatePropertyOwner;

            r.IsMainApplicant = sersPersonal.IsMainApplicant;
            r.OrderNo = sersPersonal.OrderNo;

            r.OIC1 = sersPersonal.OIC1;
            r.OIC2 = sersPersonal.OIC2;

            r.SurrenderDate = sersPersonal.SurrenderDate;

            dt.AddSersInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SersInterface, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }

        //Added By Edward 2014/05/05 Batch Upload 
        public int Insert(SersPersonal sersPersonal, string ip, string systemInfo)
        {
            SersInterface.SersInterfaceDataTable dt = new SersInterface.SersInterfaceDataTable();
            SersInterface.SersInterfaceRow r = dt.NewSersInterfaceRow();

            r.SchAcc = sersPersonal.SchAcc;
            r.RegistrationNumber = sersPersonal.RegistrationNumber;

            r.Stage = (String.IsNullOrEmpty(sersPersonal.Stage) ? string.Empty : sersPersonal.Stage);
            r.Block = (String.IsNullOrEmpty(sersPersonal.Block) ? string.Empty : sersPersonal.Block);
            r.Street = (String.IsNullOrEmpty(sersPersonal.Street) ? string.Empty : sersPersonal.Street);
            r.Level = (String.IsNullOrEmpty(sersPersonal.Level) ? string.Empty : sersPersonal.Level);
            r.Unit = (String.IsNullOrEmpty(sersPersonal.Unit) ? string.Empty : sersPersonal.Unit);
            r.PostalCode = (String.IsNullOrEmpty(sersPersonal.Postal) ? string.Empty : sersPersonal.Postal);
            r.Alloc = (String.IsNullOrEmpty(sersPersonal.Alloc) ? string.Empty : sersPersonal.Alloc);
            r.Elig = (String.IsNullOrEmpty(sersPersonal.Ellig) ? string.Empty : sersPersonal.Ellig);

            r.ApplicantType = (String.IsNullOrEmpty(sersPersonal.ApplicantType) ? string.Empty : sersPersonal.ApplicantType);
            r.CustomerId = (String.IsNullOrEmpty(sersPersonal.CustomerId) ? string.Empty : sersPersonal.CustomerId);
            r.Nric = sersPersonal.Nric;
            r.Name = sersPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(sersPersonal.MaritalStatusCode) ? string.Empty : sersPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(sersPersonal.MaritalStatus) ? string.Empty : sersPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(sersPersonal.RelationshipCode) ? string.Empty : sersPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(sersPersonal.Relationship) ? string.Empty : sersPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(sersPersonal.CitizenshipCode) ? string.Empty : sersPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(sersPersonal.Citizenship) ? string.Empty : sersPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(sersPersonal.DateOfBirth) || sersPersonal.DateOfBirth.Equals(".") ? string.Empty : sersPersonal.DateOfBirth);
            r.PrivatePropertyOwner = sersPersonal.PrivatePropertyOwner;

            r.IsMainApplicant = sersPersonal.IsMainApplicant;
            r.OrderNo = sersPersonal.OrderNo;

            r.OIC1 = sersPersonal.OIC1;
            r.OIC2 = sersPersonal.OIC2;
            r.SurrenderDate = sersPersonal.SurrenderDate;

            dt.AddSersInterfaceRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.SersInterface, id.ToString(), OperationTypeEnum.Insert,ip, systemInfo);
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
        public bool Update(SersPersonal sersPersonal)
        {
            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(sersPersonal.SchAcc, sersPersonal.Nric);

            if (dt.Count == 0)
                dt = GetSersInterfaceByRefNoAndName(sersPersonal.SchAcc, sersPersonal.Name);

            if (dt.Count == 0) return false;

            SersInterface.SersInterfaceRow r = dt[0];

            r.SchAcc = sersPersonal.SchAcc;
            r.RegistrationNumber = (String.IsNullOrEmpty(sersPersonal.RegistrationNumber) ? string.Empty : sersPersonal.RegistrationNumber);
            r.Stage = (String.IsNullOrEmpty(sersPersonal.Stage) ? string.Empty : sersPersonal.Stage);
            r.Block = (String.IsNullOrEmpty(sersPersonal.Block) ? string.Empty : sersPersonal.Block);
            r.Street = (String.IsNullOrEmpty(sersPersonal.Street) ? string.Empty : sersPersonal.Street);
            r.Level = (String.IsNullOrEmpty(sersPersonal.Level) ? string.Empty : sersPersonal.Level);
            r.Unit = (String.IsNullOrEmpty(sersPersonal.Unit) ? string.Empty : sersPersonal.Unit);
            r.PostalCode = (String.IsNullOrEmpty(sersPersonal.Postal) ? string.Empty : sersPersonal.Postal);
            r.Alloc = (String.IsNullOrEmpty(sersPersonal.Alloc) ? string.Empty : sersPersonal.Alloc);
            r.Elig = (String.IsNullOrEmpty(sersPersonal.Ellig) ? string.Empty : sersPersonal.Ellig);

            r.ApplicantType = (String.IsNullOrEmpty(sersPersonal.ApplicantType) ? string.Empty : sersPersonal.ApplicantType);
            r.CustomerId = sersPersonal.CustomerId;
            r.Nric = sersPersonal.Nric;
            r.Name = sersPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(sersPersonal.MaritalStatusCode) ? string.Empty : sersPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(sersPersonal.MaritalStatus) ? string.Empty : sersPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(sersPersonal.RelationshipCode) ? string.Empty : sersPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(sersPersonal.Relationship) ? string.Empty : sersPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(sersPersonal.CitizenshipCode) ? string.Empty : sersPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(sersPersonal.Citizenship) ? string.Empty : sersPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(sersPersonal.DateOfBirth) || sersPersonal.DateOfBirth.Equals(".") ? string.Empty : sersPersonal.DateOfBirth);
            r.PrivatePropertyOwner = sersPersonal.PrivatePropertyOwner;

            r.IsMainApplicant = sersPersonal.IsMainApplicant;
            r.OrderNo = sersPersonal.OrderNo;

            r.OIC1 = sersPersonal.OIC1;
            r.OIC2 = sersPersonal.OIC2;

            r.SurrenderDate = sersPersonal.SurrenderDate;
           

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateBySales(SersPersonal sersPersonal)
        {
            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(sersPersonal.SchAcc, sersPersonal.Nric);

            if (dt.Count == 0)
                dt = GetSersInterfaceByRefNoAndName(sersPersonal.SchAcc, sersPersonal.Name);

            if (dt.Count == 0) return false;

            SersInterface.SersInterfaceRow r = dt[0];

            r.ApplicantType = (String.IsNullOrEmpty(sersPersonal.ApplicantType) ? string.Empty : sersPersonal.ApplicantType);
            r.CustomerId = sersPersonal.CustomerId;
            r.Nric = sersPersonal.Nric;
            r.Name = sersPersonal.Name;
            r.MaritalStatusCode = (String.IsNullOrEmpty(sersPersonal.MaritalStatusCode) ? string.Empty : sersPersonal.MaritalStatusCode);
            r.MaritalStatus = (String.IsNullOrEmpty(sersPersonal.MaritalStatus) ? string.Empty : sersPersonal.MaritalStatus);
            r.RelationshipCode = (String.IsNullOrEmpty(sersPersonal.RelationshipCode) ? string.Empty : sersPersonal.RelationshipCode);
            r.Relationship = (String.IsNullOrEmpty(sersPersonal.Relationship) ? string.Empty : sersPersonal.Relationship);
            r.CitizenshipCode = (String.IsNullOrEmpty(sersPersonal.CitizenshipCode) ? string.Empty : sersPersonal.CitizenshipCode);
            r.Citizenship = (String.IsNullOrEmpty(sersPersonal.Citizenship) ? string.Empty : sersPersonal.Citizenship);
            r.DateOfBirth = (String.IsNullOrEmpty(sersPersonal.DateOfBirth) || sersPersonal.DateOfBirth.Equals(".") ? string.Empty : sersPersonal.DateOfBirth);

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateApplication(string stage, string block, string street, string level, string unitNumber,
            string postalCode, string allocationScheme, string eligibilityScheme, string registrationNumber, string SchAcc,
            string OIC1, string OIC2, string surrenderDate )
        {
            return Adapter.UpdateSersInterfaceInfo(stage, block, street, level, unitNumber, postalCode, allocationScheme, 
                eligibilityScheme, registrationNumber,OIC1,OIC2, surrenderDate,  SchAcc) > 0;
        }

        public bool UpdateOrderNo(string refNo, string nric, int orderNo)
        {
            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndNric(refNo, nric);

            if (dt.Count == 0) return false;

            SersInterface.SersInterfaceRow r = dt[0];

            r.OrderNo = orderNo;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool UpdateOrderNos(string refNo, string applicantType, bool sortByOrderNo)
        {
            SersInterface.SersInterfaceDataTable dt = (sortByOrderNo ?
                GetSersInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, applicantType) :
                GetSersInterfaceByRefNoAndApplicantTypeSortByNric(refNo, applicantType));

            if (dt.Count == 0) return false;

            int orderNo = 1;
            foreach (SersInterface.SersInterfaceRow r in dt)
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
            SersInterface.SersInterfaceDataTable dt = GetSersInterfaceByRefNoAndApplicantTypeSortByOrderNo(refNo, "HA");

            // Set the main applicant as the first personal in the order
            foreach (SersInterface.SersInterfaceRow r in dt)
            {
                if (r.IsMainApplicant)
                {
                    r.OrderNo = 1;

                    int rowsAffected = Adapter.Update(dt);
                    break;
                }
            }

            // Update all the other order nos by incrementing by 1
            foreach (SersInterface.SersInterfaceRow r in dt)
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
        #endregion

        #region Delete
        public void DeleteWrongRecords()
        {
            //SersInterfceDs.DeleteWrongRecords();
        }

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
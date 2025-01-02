using System;
using System.Collections.Generic;
using System.Web;
using Dwms.Bll;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResalePersonal
    /// </summary>
    public class SersPersonal
    {
        #region Sers info
        private string schAcc;
        private string registrationNumber;
        private string stage;
        private string block;
        private string street;
        private string level;
        private string unit;
        private string postal;
        private string alloc;
        private string ellig;
        #endregion

        #region Applicant info
        private string applicantType;
        private string customerId;
        private string nric;
        private string name;
        private string maritalStatusCode;
        private string maritalStatus;
        private string relationshipCode;
        private string relationship;
        private string citizenshipCode;
        private string citizenship;
        private string dateOfBirth;
        private bool privatePropertyOwner;

        private int orderNo;
        private bool isMainApplicant;
        private bool forDeletion;

        private string strOIC1;
        private string strOIC2;
        private string strSurrenderDate;
        #endregion        

        #region Public Members
        public string SchAcc
        {
            get { return schAcc; }
            set { schAcc = value; }
        }

        public string RegistrationNumber
        {
            get { return registrationNumber; }
            set { registrationNumber = value; }
        }

        public string Stage
        {
            get { return stage; }
            set { stage = value; }
        }

        public string Block
        {
            get { return block; }
            set { block = value; }
        }

        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        public string Level
        {
            get { return level; }
            set { level = value; }
        }

        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        public string Postal
        {
            get { return postal; }
            set { postal = value; }
        }

        public string Alloc
        {
            get { return alloc; }
            set { alloc = value; }
        }

        public string Ellig
        {
            get { return ellig; }
            set { ellig = value; }
        }

        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public string Nric
        {
            get { return nric; }
            set { nric = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string MaritalStatusCode
        {
            get { return maritalStatusCode; }
            set { maritalStatusCode = value; }
        }

        public string MaritalStatus
        {
            get { return maritalStatus; }
            set { maritalStatus = value; }
        }

        public string DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
        }

        public string CitizenshipCode
        {
            get { return citizenshipCode; }
            set { citizenshipCode = value; }
        }

        public string Citizenship
        {
            get { return citizenship; }
            set { citizenship = value; }
        }        

        public string RelationshipCode
        {
            get { return relationshipCode; }
            set { relationshipCode = value; }
        }

        public string Relationship
        {
            get { return relationship; }
            set { relationship = value; }
        }

        public bool PrivatePropertyOwner
        {
            get { return privatePropertyOwner; }
            set { privatePropertyOwner = value; }
        }

        public string ApplicantType
        {
            get { return applicantType; }
            set { applicantType = value; }
        }

        public bool IsMainApplicant
        {
            get { return isMainApplicant; }
            set { isMainApplicant = value; }
        }

        public int OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }

        public string OIC1
        {
            get { return strOIC1; }
            set { strOIC2 = value; }
        }

        public string OIC2
        {
            get { return strOIC1; }
            set { strOIC2 = value; }
        }

        public string SurrenderDate
        {
            get { return strSurrenderDate; }
            set { strSurrenderDate = value; }
        }
        #endregion

        #region Methods
        public SersPersonal()
        {
        }

        public void AddPersonalInfo(string schAcc, string stage, string blk, string street, string level, string unit, string postal,
            string alloc, string elig, string nric, string name, string maritalStatusCode, string maritalStatus, string dateOfBirth,
            string citizenshipCode, string citizenship, string relationshipCode, string relationship, bool privatePropertyOwner, 
            string applicantType, int orderNo)
        {
            this.schAcc = schAcc;
            this.stage = stage;
            this.block = blk;
            this.street = street;
            this.level = level;
            this.unit = unit;
            this.postal = postal;
            this.alloc = alloc;
            this.ellig = elig;
            this.nric = nric;
            this.name = name;
            this.maritalStatusCode = maritalStatusCode;
            this.maritalStatus = maritalStatus;
            this.dateOfBirth = dateOfBirth;
            this.citizenship = citizenship;
            this.citizenshipCode = citizenshipCode;
            this.relationshipCode = relationshipCode;
            this.relationship = relationship;
            this.privatePropertyOwner = privatePropertyOwner;
            this.applicantType = applicantType;
            this.orderNo = orderNo;
        }

        //sersPersonal.AddPersonalInfoFromWebService(refNo, stage, block, street, level, unitNumber, postalCode, allocationScheme, eligibilityScheme,
        //    ocOrderNo++, personal);
        public void AddPersonalInfoFromWebService(string schAcc, string stage, string blk, string street, string level, string unit, 
            string postal, string alloc, string elig, int orderNo, string oic1, string oic2, string surrenderDate, Sers.Personal personal)
        {
            this.schAcc = schAcc;
            this.stage = stage;
            this.block = blk;
            this.street = street;
            this.level = level;
            this.unit = unit;
            this.postal = postal;
            this.alloc = alloc;
            this.ellig = elig;
            this.orderNo = orderNo;

            this.forDeletion = personal.IsDeletion;
            this.applicantType = personal.PersonalType;
            this.isMainApplicant = (personal.IsMainApplicant == null ? false : personal.IsMainApplicant);
            this.nric = personal.Nric;
            this.name = personal.Name;
            this.maritalStatusCode = personal.MaritalStatus;
            this.maritalStatus = EnumManager.MapMaritalStatus(this.maritalStatusCode);
            this.dateOfBirth = (personal.DateOfBirth == null ? string.Empty : Format.FormatDateTime(personal.DateOfBirth, DateTimeFormat.dd_MM_yyyy));
            this.relationshipCode = personal.Relationship;
            this.relationship = EnumManager.MapRelationshipStatus(this.relationshipCode);
            this.citizenshipCode = personal.Citizenship.ToUpper();
            this.citizenship = EnumManager.MapCitizenship(this.citizenshipCode);
            this.privatePropertyOwner = (personal.PrivatePropertyOwner == null ? false : personal.PrivatePropertyOwner);
            this.customerId = (personal.CustomerId == null ? string.Empty : personal.CustomerId);
            this.strOIC1 = oic1;
            this.strOIC2 = oic2;
            this.strSurrenderDate = surrenderDate;
        }

        //sersPersonal.AddPersonalInfoFromWebService(refNo, stage, block, street, level, unitNumber, postalCode, allocationScheme, eligibilityScheme,
        //    ocOrderNo++, personal);

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2015/02/05          modifed for affected sers logic
        public void AddPersonalInfoFromSales(Sales.Personal personal, string affectedSers, int orderNo)
        {
            this.forDeletion = personal.IsDeletion;
            this.applicantType = personal.PersonalType;
            this.isMainApplicant = (personal.IsMainApplicant == null ? false : personal.IsMainApplicant);
            this.nric = personal.Nric;
            this.name = personal.Name;
            this.maritalStatusCode = personal.MaritalStatus;
            this.maritalStatus = EnumManager.MapMaritalStatus(this.maritalStatusCode);
            this.dateOfBirth = (personal.DateOfBirth == null ? string.Empty : Format.FormatDateTime(personal.DateOfBirth, DateTimeFormat.dd_MM_yyyy));
            this.relationshipCode = personal.Relationship;
            this.relationship = EnumManager.MapRelationshipStatus(this.relationshipCode);
            this.citizenshipCode = personal.Citizenship.ToUpper();
            this.citizenship = EnumManager.MapCitizenship(this.citizenshipCode);
            this.customerId = (personal.CustomerId == null ? string.Empty : personal.CustomerId);
            this.schAcc = affectedSers;
            this.orderNo = orderNo;             
        }
        #endregion
    }
}
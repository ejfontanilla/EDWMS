using System;
using System.Collections.Generic;
using System.Web;
using Dwms.Bll;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResalePersonal
    /// </summary>
    public class ResalePersonal
    {
        #region Resale info
        private string caseNo;
        private string numSchAccnt;
        private string oldLesseeCode;
        private string newLesseeCode;
        private string allocBuyer;
        private string eligBuyer;
        private string codeRecipient;
        private string roic;
        private string oic;
        private string eligibilityScheme;
        private string householdType;
        private string ahgGrant;
        private string hhInc2Year;
        private string bankLoan;
        private string ABCDELoan;
        private string houseHoldIncome;
        private string caInc;
        private string applicationDate;
        private string appointmentDate;
        private string completionDate;
        private string cancellationDate;
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
        private string employmentTypeCode;
        private string employmentType;
        private string date1;
        private string date2;
        private string date3;
        private string date4;
        private string date5;
        private string date6;
        private string date7;
        private string date8;
        private string date9;
        private string date10;
        private string date11;
        private string date12;
        private string date13;
        private string date14;
        private string date15;
        private string inc1;
        private string inc2;
        private string inc3;
        private string inc4;
        private string inc5;
        private string inc6;
        private string inc7;
        private string inc8;
        private string inc9;
        private string inc10;
        private string inc11;
        private string inc12;
        private string inc13;
        private string inc14;
        private string inc15;

        private int orderNo;
        private bool isMainApplicant;
        private bool forDeletion;

        private int noOfIncomeMonths; //Added by Edward 19/2/2014   Sales and Resale Changes
        #endregion

        #region Public Members
        public string CaseNo
        {
            get { return caseNo; }
            set { caseNo = value; }
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

        public string EmploymentTypeCode
        {
            get { return employmentTypeCode; }
            set { employmentTypeCode = value; }
        }

        public string EmploymentType
        {
            get { return employmentType; }
            set { employmentType = value; }
        }

        public string Date1
        {
            get { return date1; }
            set { date1 = value; }
        }

        public string Date2
        {
            get { return date2; }
            set { date2 = value; }
        }

        public string Date3
        {
            get { return date3; }
            set { date3 = value; }
        }

        public string Date4
        {
            get { return date4; }
            set { date4 = value; }
        }

        public string Date5
        {
            get { return date5; }
            set { date5 = value; }
        }

        public string Date6
        {
            get { return date6; }
            set { date6 = value; }
        }

        public string Date7
        {
            get { return date7; }
            set { date7 = value; }
        }

        public string Date8
        {
            get { return date8; }
            set { date8 = value; }
        }

        public string Date9
        {
            get { return date9; }
            set { date9 = value; }
        }

        public string Date10
        {
            get { return date10; }
            set { date10 = value; }
        }

        public string Date11
        {
            get { return date11; }
            set { date11 = value; }
        }

        public string Date12
        {
            get { return date12; }
            set { date12 = value; }
        }

        public string Date13
        {
            get { return date13; }
            set { date13 = value; }
        }

        public string Date14
        {
            get { return date14; }
            set { date14 = value; }
        }

        public string Date15
        {
            get { return date15; }
            set { date15 = value; }
        }

        public string Inc1
        {
            get { return inc1; }
            set { inc1 = value; }
        }

        public string Inc2
        {
            get { return inc2; }
            set { inc2 = value; }
        }

        public string Inc3
        {
            get { return inc3; }
            set { inc3 = value; }
        }

        public string Inc4
        {
            get { return inc4; }
            set { inc4 = value; }
        }

        public string Inc5
        {
            get { return inc5; }
            set { inc5 = value; }
        }

        public string Inc6
        {
            get { return inc6; }
            set { inc6 = value; }
        }

        public string Inc7
        {
            get { return inc7; }
            set { inc7 = value; }
        }

        public string Inc8
        {
            get { return inc8; }
            set { inc8 = value; }
        }

        public string Inc9
        {
            get { return inc9; }
            set { inc9 = value; }
        }

        public string Inc10
        {
            get { return inc10; }
            set { inc10 = value; }
        }

        public string Inc11
        {
            get { return inc11; }
            set { inc11 = value; }
        }

        public string Inc12
        {
            get { return inc12; }
            set { inc12 = value; }
        }

        public string Inc13
        {
            get { return inc13; }
            set { inc13 = value; }
        }

        public string Inc14
        {
            get { return inc14; }
            set { inc14 = value; }
        }

        public string Inc15
        {
            get { return inc15; }
            set { inc15 = value; }
        }

        public string Roic
        {
            get { return roic; }
            set { roic = value; }
        }

        public string Oic
        {
            get { return oic; }
            set { oic = value; }
        }

        public string AhgGrant
        {
            get { return ahgGrant; }
            set { ahgGrant = value; }
        }

        public string HhInc2Year
        {
            get { return hhInc2Year; }
            set { hhInc2Year = value; }
        }

        public string NumSchAccnt
        {
            get { return numSchAccnt; }
            set { numSchAccnt = value; }
        }

        public string OldLesseeCode
        {
            get { return oldLesseeCode; }
            set { oldLesseeCode = value; }
        }

        public string NewLesseeCode
        {
            get { return newLesseeCode; }
            set { newLesseeCode = value; }
        }

        public string AllocBuyer
        {
            get { return allocBuyer; }
            set { allocBuyer = value; }
        }

        public string EligBuyer
        {
            get { return eligBuyer; }
            set { eligBuyer = value; }
        }

        public string CodeRecipient
        {
            get { return codeRecipient; }
            set { codeRecipient = value; }
        }

        public string EligibilityScheme
        {
            get { return eligibilityScheme; }
            set { eligibilityScheme = value; }
        }
        public string HouseholdType
        {
            get { return householdType; }
            set { householdType = value; }
        }

        public string ApplicationDate
        {
            get { return applicationDate; }
            set { applicationDate = value; }
        }

        public string AppointmentDate
        {
            get { return appointmentDate; }
            set { appointmentDate = value; }
        }

        public string CompletionDate
        {
            get { return completionDate; }
            set { completionDate = value; }
        }

        public string CancellationDate
        {
            get { return cancellationDate; }
            set { cancellationDate = value; }
        }

        public string BankLoan
        {
            get { return bankLoan; }
            set { bankLoan = value; }
        }

        public string ABCDELoan
        {
            get { return ABCDELoan; }
            set { ABCDELoan = value; }
        }

        public string HouseHoldInc
        {
            get { return houseHoldIncome; }
            set { houseHoldIncome = value; }
        }

        public string CaInc
        {
            get { return caInc; }
            set { caInc = value; }
        }

        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
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

        //Added by Edward 19/2/2014 Sales and Resale Changes
        public int NoOfIncomeMonths
        {
            get { return noOfIncomeMonths; }
            set { noOfIncomeMonths = value; }
        }
        #endregion

        #region Methods
        public ResalePersonal()
        {
        }

        public void AddPersonalInfo(string caseNo, string nric, string name, string maritalStatusCode, string maritalStatus,
            string dateOfBirth, string relationshipCode, string relationship, string roic, string oic, string ahgGrant, string hhInc2Year,
            string numSchAccnt, string oldLesseeCode, string newLesseeCode, string allocBuyer, string eligBuyer, string bankLoan, string ABCDELoan,
            string houseHoldIncome, string caInc, string applicantType, int orderNo)
        {
            this.caseNo = caseNo;
            this.nric = nric;
            this.name = name;
            this.maritalStatusCode = maritalStatusCode;
            this.maritalStatus = maritalStatus;
            this.dateOfBirth = dateOfBirth;
            this.relationshipCode = relationshipCode;
            this.relationship = relationship;
            this.roic = roic;
            this.oic = oic;
            this.ahgGrant = ahgGrant;
            this.hhInc2Year = hhInc2Year;
            this.numSchAccnt = numSchAccnt;
            this.oldLesseeCode = oldLesseeCode;
            this.newLesseeCode = newLesseeCode;
            this.allocBuyer = allocBuyer;
            this.eligBuyer = eligBuyer;
            this.bankLoan = bankLoan;
            this.ABCDELoan = ABCDELoan;
            this.houseHoldIncome = houseHoldIncome;
            this.caInc = caInc;
            this.applicantType = applicantType;
            this.orderNo = orderNo;
        }

        public void AddPersonalInfoFromWebService(string caseNo, string numSchAccnt, string oldLesseeCode, string newLesseeCode, string buyerAllocationScheme,
        string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme, string householdType, string AHGGrant,
        string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome, string applicationDate,
        string appointmentDate, string completionDate, string cancellationDate, int orderNo, Resale.Personal personal)
        {
            this.caseNo = caseNo;
            this.numSchAccnt = numSchAccnt;
            this.oldLesseeCode = oldLesseeCode;
            this.newLesseeCode = newLesseeCode;
            this.allocBuyer = buyerAllocationScheme;
            this.eligBuyer = buyerEligibilityScheme;
            this.codeRecipient = codeRecipient;
            this.roic = ROIC;
            this.oic = caseOIC;
            this.eligibilityScheme = eligibilityScheme;
            this.householdType = householdType;
            this.ahgGrant = AHGGrant;
            this.hhInc2Year = HH2YINC;
            this.bankLoan = bankLoanAmount;
            this.ABCDELoan = ABCDELoanAmount;
            this.houseHoldIncome = householdIncome;
            this.caInc = CAIncome;
            this.applicationDate = (applicationDate == null ? string.Empty : Format.FormatDateTime(applicationDate, DateTimeFormat.dd_MM_yyyy));
            this.appointmentDate = (appointmentDate == null ? string.Empty : Format.FormatDateTime(appointmentDate, DateTimeFormat.dd_MM_yyyy));
            this.completionDate = (completionDate == null ? string.Empty : Format.FormatDateTime(completionDate, DateTimeFormat.dd_MM_yyyy));
            this.cancellationDate = (cancellationDate == null ? string.Empty : Format.FormatDateTime(cancellationDate, DateTimeFormat.dd_MM_yyyy));
            this.orderNo = orderNo;

            this.forDeletion = personal.IsDeletion;
            this.applicantType = personal.PersonalType;
            this.isMainApplicant = (personal.IsMainApplicant == null ? false : personal.IsMainApplicant);
            this.nric = personal.Nric;
            this.name = personal.Name;
            this.maritalStatusCode = personal.MaritalStatus;
            this.maritalStatus = EnumManager.MapMaritalStatus(this.maritalStatusCode);
            this.dateOfBirth = (personal.DateOfBirth == null ? string.Empty : Format.FormatDateTime(personal.DateOfBirth, DateTimeFormat.dd_MM_yyyy));
            this.relationshipCode = personal.Relationship.ToUpper();
            this.relationship = EnumManager.MapResaleRelationshipStatus(this.relationshipCode);
            this.citizenshipCode = personal.Citizenship.ToUpper();
            this.citizenship = EnumManager.MapCitizenship(this.citizenshipCode);
            this.employmentTypeCode = personal.EmploymentStatus;
            this.employmentType = EnumManager.MapResaleEmploymentStatus(this.employmentTypeCode);
            this.customerId = (personal.CustomerId == null ? string.Empty : personal.CustomerId);
            this.date1 = this.date2 = this.date3 = this.date4 = this.date5 = this.date6 = this.date7 = this.date8 = this.date9 = this.date10 = this.date11 = this.date12 = this.date13 = this.date14 = this.date15 = string.Empty;
            this.inc1 = this.inc2 = this.inc3 = this.inc4 = this.inc5 = this.inc6 = this.inc7 = this.inc8 = this.inc9 = this.inc10 = this.inc11 = this.inc12 = this.inc13 = this.inc14 = this.inc15 = "0";

            this.noOfIncomeMonths = (personal.NumberOfIncomeMonths == null ? 0 : personal.NumberOfIncomeMonths); //Added by Edward 19.02.2014   Sales and Resale Changes

            if (personal.MonthlyIncomeSet != null)
            {
                #region Income Details
                int startIndex = 15 - (personal.MonthlyIncomeSet.Length > 15 ? 15 : personal.MonthlyIncomeSet.Length);

                for (int cnt = startIndex, incomeStart = 0; cnt < 15; cnt++, incomeStart++)
                {
                    Resale.MonthlyIncome monthlyIncome = personal.MonthlyIncomeSet[incomeStart];

                    string yearMonth = monthlyIncome.YearMonth;
                    string amount = (monthlyIncome.Amount == null ? string.Empty : monthlyIncome.Amount.ToString());

                    switch (cnt + 1)
                    {
                        case 1:
                            this.date1 = yearMonth;
                            this.inc1 = amount;
                            break;
                        case 2:
                            this.date2 = yearMonth;
                            this.inc2 = amount;
                            break;
                        case 3:
                            this.date3 = yearMonth;
                            this.inc3 = amount;
                            break;
                        case 4:
                            this.date4 = yearMonth;
                            this.inc4 = amount;
                            break;
                        case 5:
                            this.date5 = yearMonth;
                            this.inc5 = amount;
                            break;
                        case 6:
                            this.date6 = yearMonth;
                            this.inc6 = amount;
                            break;
                        case 7:
                            this.date7 = yearMonth;
                            this.inc7 = amount;
                            break;
                        case 8:
                            this.date8 = yearMonth;
                            this.inc8 = amount;
                            break;
                        case 9:
                            this.date9 = yearMonth;
                            this.inc9 = amount;
                            break;
                        case 10:
                            this.date10 = yearMonth;
                            this.inc10 = amount;
                            break;
                        case 11:
                            this.date11 = yearMonth;
                            this.inc11 = amount;
                            break;
                        case 12:
                            this.date12 = yearMonth;
                            this.inc12 = amount;
                            break;
                        case 13:
                            this.date13 = yearMonth;
                            this.inc13 = amount;
                            break;
                        case 14:
                            this.date14 = yearMonth;
                            this.inc14 = amount;
                            break;
                        case 15:
                            this.date15 = yearMonth;
                            this.inc15 = amount;
                            break;
                        default:
                            break;
                    }
                }
                #endregion

            }
        }
        #endregion
    }
}

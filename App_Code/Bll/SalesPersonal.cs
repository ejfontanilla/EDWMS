using System;
using System.Collections.Generic;
using System.Web;
using Dwms.Bll;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for ResalePersonal
    /// </summary>
    public class SalesPersonal
    {
        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2014/3/12           All forwardedDate is replaced to applicationDate. applicationDate is added in DB Sales and Resale Changes DWMS_8
        //Edward                            2015/02/04          Added ABCDERefNumber Field in Application

        #region Properties
        private string refNumber;
        private string status;
        private string ballotQuarter;
        private string householdType;
        private string allocationMode;
        private string allocationScheme;
        private string eligibilityScheme;
        private string householdIncome;
        private string flatType;
        private string ahgStatus;
        private string shgStatus;
        private string loanTag;
        private string bookedAddress;
        private string flatDesign;
        private string contactNumbers;
        private string affectedSERS;
        private string acceptanceDate;
        #region Added By Edward 12/3/2014
        private string applicationDate; //Modified ForwardedDate to ApplicationDate by Edward 12/3/2014 Sales and Resale Changes
        #endregion
        #region Added by Edward 15/12/2014
        private string keyIssueDate;
        private string cancelledDate;
        #endregion
        private string ABCDERefNumber;

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
        private string income;
        private string NRICSOC;
        private string NameSOC;

        private int orderNo;
        private bool isMainApplicant;
        private bool forDeletion;
        private bool isMCPS;
        private bool isPPO;

        private string oic1;
        private string oic2;

        private int noOfIncomeMonths; //Added by Edward 19/2/2014   Sales and Resale Changes

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

        public string RefNumber
        {
            get { return refNumber; }
            set { refNumber = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string BallotQuarter
        {
            get { return ballotQuarter; }
            set { ballotQuarter = value; }
        }

        public string HouseholdType
        {
            get { return householdType; }
            set { householdType = value; }
        }

        public string AllocationMode
        {
            get { return allocationMode; }
            set { allocationMode = value; }
        }

        public string AllocationScheme
        {
            get { return allocationScheme; }
            set { allocationScheme = value; }
        }

        public string EligibilityScheme
        {
            get { return eligibilityScheme; }
            set { eligibilityScheme = value; }
        }

        public string HouseholdIncome
        {
            get { return householdIncome; }
            set { householdIncome = value; }
        }

        public string FlatType
        {
            get { return flatType; }
            set { flatType = value; }
        }

        public string AhgStatus
        {
            get { return ahgStatus; }
            set { ahgStatus = value; }
        }

        public string ShgStatus
        {
            get { return shgStatus; }
            set { shgStatus = value; }
        }

        public string LoanTag
        {
            get { return loanTag; }
            set { loanTag = value; }
        }

        public string BookedAddress
        {
            get { return bookedAddress; }
            set { bookedAddress = value; }
        }

        public string FlatDesign
        {
            get { return flatDesign; }
            set { flatDesign = value; }
        }

        public string ContactNumbers
        {
            get { return contactNumbers; }
            set { contactNumbers = value; }
        }

        public string AffectedSERS
        {
            get { return affectedSERS; }
            set { affectedSERS = value; }
        }

        public string AcceptanceDate
        {
            get { return acceptanceDate; }
            set { acceptanceDate = value; }
        }

        public string ApplicationDate
        {
            get { return applicationDate; }
            set { applicationDate = value; }
        }

        public string KeyIssueDate
        {
            get { return keyIssueDate; }
            set { keyIssueDate = value; }
        }

        public string CancelledDate
        {
            get { return cancelledDate; }
            set { cancelledDate = value; }
        }

        public string ABCDERefNumber
        {
            get { return ABCDERefNumber; }
            set { ABCDERefNumber = value; }
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

        public string Income
        {
            get { return income; }
            set { income = value; }
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

        public string nricSOC
        {
            get { return NRICSOC; }
            set { NRICSOC = value; }
        }

        public string nameSOC
        {
            get { return NameSOC; }
            set { NameSOC = value; }
        }

        public bool IsMainApplicant
        {
            get { return isMainApplicant; }
            set { isMainApplicant = value; }
        }

        public bool IsMCPS
        {
            get { return isMCPS; }
            set { isMCPS = value; }
        }

        public bool IsPPO
        {
            get { return isPPO; }
            set { isPPO = value; }
        }

        public int OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }

        #region Added By Edward 10/3/2014 Sales and Resale Changes
        public string OIC1
        {
            get { return oic1; }
            set { oic1 = value; }
        }

        public string OIC2
        {
            get { return oic2; }
            set { oic2 = value; }
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

        #endregion

        //Added by Edward 19/2/2014 Sales and Resale Changes
        public int NoOfIncomeMonths
        {
            get { return noOfIncomeMonths; }
            set { noOfIncomeMonths = value; }
        }
        
        #endregion

        #region Methods
        public SalesPersonal()
        {
        }

        public void AddPersonalInfoFromWebService(string refNumber, string status, string ballotQuarter, string householdType, string allocationMode,
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
        string flatDesign, string contactNumbers, string affectedSERS, string OIC1, string OIC2, string acceptanceDate, 
        string applicationDate, string keyIssueDate, string cancelledDate, string ABCDERefNumber, int orderNo, Sales.Personal personal)
        {
            this.refNumber = refNumber;
            this.status = status;
            this.ballotQuarter = ballotQuarter;
            this.householdType = householdType;
            this.allocationMode = allocationMode;
            this.allocationScheme = allocationScheme;
            this.eligibilityScheme = eligibilityScheme;
            this.householdIncome = householdIncome;
            this.flatType = flatType;
            this.ahgStatus = AHGStatus;
            this.shgStatus = SHGStatus;
            this.loanTag = loanTag;
            this.bookedAddress = bookedAddress;
            this.flatDesign = flatDesign;
            this.contactNumbers = contactNumbers;
            this.affectedSERS = affectedSERS;
            this.acceptanceDate = acceptanceDate;
            this.applicationDate = applicationDate;
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
            this.relationship = EnumManager.MapRelationshipStatus(this.relationshipCode);
            this.citizenshipCode = personal.Citizenship.ToUpper();
            this.citizenship = EnumManager.MapCitizenship(this.citizenshipCode);
            this.employmentTypeCode = personal.EmploymentStatus;
            this.employmentType = EnumManager.MapSalesEmploymentStatus(this.employmentTypeCode);
            this.customerId = (personal.CustomerId == null ? string.Empty : personal.CustomerId);
            //this.income = personal.Income;
            this.isMCPS = (personal.IsMCPS == null ? false : personal.IsMCPS);
            this.isPPO = (personal.IsPPO == null ? false : personal.IsPPO);
            this.NRICSOC = personal.NRICSOC;
            this.NameSOC = personal.NameSOC;

            this.oic1 = OIC1;
            this.oic2 = OIC2;
            this.keyIssueDate = keyIssueDate;
            this.cancelledDate = cancelledDate;
            this.ABCDERefNumber = ABCDERefNumber;   //Added BY Edward 2015/02/04
            
            this.noOfIncomeMonths = (personal.NumberOfIncomeMonths == null ? 0 : personal.NumberOfIncomeMonths); //Added by Edward 19.02.2014   Sales and Resale Changes

            //Added By Edward 11/3/2014 Sales and Resale Changes  To Cater Monthly Income Set

            if (personal.MonthlyIncomeSet != null)
            {
                #region Income Details
                int startIndex = 12 - (personal.MonthlyIncomeSet.Length > 12 ? 12 : personal.MonthlyIncomeSet.Length);

                for (int cnt = startIndex, incomeStart = 0; cnt < 12; cnt++, incomeStart++)
                {
                    Sales.MonthlyIncome monthlyIncome = personal.MonthlyIncomeSet[incomeStart];

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
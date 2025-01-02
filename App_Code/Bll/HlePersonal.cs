using System;
using System.Collections.Generic;
using System.Web;
using Dwms.Bll;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for HlePersonal
    /// </summary>
    public class HlePersonal
    {
        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2015/02/04          Added Risk Field in Application


        #region HLE info
        private string referenceNumber;
        private string applicationDate;
        private string status;
        private string houseHoldIncome;
        private string creditAssessmentIncome;
        private string preEligibilityOic;
        private string creditAssessmentOic;
        private string rejectedDate;
        private string cancelledDate;
        private string approvedDate;
        private string expiredDate;
        private string risk;
        #endregion

        #region Applicant info
        private string applicantType;
        private string nric;
        private string name;
        private string maritalStatus;
        private string maritalStatusCode;
        private string dateOfBirth;
        private string employmentTypeCode;
        private string employmentType;
        private string relationship;
        private string relationshipCode;
        private string citizenship;
        private string citizenshipCode;
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
        private string annualIncome;
        private string averageIncome;
        private string employerName;
        private string dateJoined;
        private bool withCpf;
        private bool withAllowance;
        private string customerId;
        private int noOfIncomeMonths; //Added by Edward 22.10.2013

        private int orderNo;
        private bool isMainApplicant;
        private bool forDeletion;
        #endregion        

        #region Public Members
        public string HleNumber
        {
            get { return referenceNumber; }
            set { referenceNumber = value; }
        }

        public string HleDate
        {
            get { return applicationDate; }
            set { applicationDate = value; }
        }

        public string HleStatus
        {
            get { return status; }
            set { status = value; }
        }

        public string HhIncome
        {
            get { return houseHoldIncome; }
            set { houseHoldIncome = value; }
        }
        
        public string CaIncome
        {
            get { return creditAssessmentIncome; }
            set { creditAssessmentIncome = value; }
        }
        
        public string PeOic
        {
            get { return preEligibilityOic; }
            set { preEligibilityOic = value; }
        }
        
        public string CaOic
        {
            get { return creditAssessmentOic; }
            set { creditAssessmentOic = value; }
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
        
        public string MaritalStatus
        {
            get { return maritalStatus; }
            set { maritalStatus = value; }
        }
        
        public string MaritalStatusCode
        {
            get { return maritalStatusCode; }
            set { maritalStatusCode = value; }
        }
        
        public string DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; }
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

        public string Relationship
        {
            get { return relationship; }
            set { relationship = value; }
        }
        
        public string RelationshipCode
        {
            get { return relationshipCode; }
            set { relationshipCode = value; }
        }

        public string Citizenship
        {
            get { return citizenship; }
            set { citizenship = value; }
        }

        public string CitizenshipCode
        {
            get { return citizenshipCode; }
            set { citizenshipCode = value; }
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
        
        public string IncYear
        {
            get { return annualIncome; }
            set { annualIncome = value; }
        }
        
        public string IncAve
        {
            get { return averageIncome; }
            set { averageIncome = value; }
        }
        
        public string EmployerName
        {
            get { return employerName; }
            set { employerName = value; }
        }
        
        public string DateJoined
        {
            get { return dateJoined; }
            set { dateJoined = value; }
        }
        
        public string ApplicantType
        {
            get { return applicantType; }
            set { applicantType = value; }
        }

        public string RejectedDate
        {
            get { return rejectedDate; }
            set { rejectedDate = value; }
        }

        public string CancelledDate
        {
            get { return cancelledDate; }
            set { cancelledDate = value; }
        }

        public string ApprovedDate
        {
            get { return approvedDate; }
            set { approvedDate = value; }
        }

        public string ExpiredDate
        {
            get { return expiredDate; }
            set { expiredDate = value; }
        }

        public int OrderNo
        {
            get { return orderNo; }
            set { orderNo = value; }
        }

        public bool IsMainApplicant
        {
            get { return isMainApplicant; }
            set { isMainApplicant = value; }
        }

        public bool WithCpf
        {
            get { return withCpf; }
            set { withCpf = value; }
        }

        public string CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public bool WithAllowance
        {
            get { return withAllowance; }
            set { withAllowance = value; }
        }

        public bool ForDeletion
        {
            get { return forDeletion; }
            set { forDeletion = value; }
        }
        //Added by Edward 22.10.2013
        public int NoOfIncomeMonths
        {
            get { return noOfIncomeMonths; }
            set { noOfIncomeMonths = value; }
        }

        public string Risk
        {
            get { return risk; }
            set { risk = value; }
        }
        #endregion

        public HlePersonal()
        {
        }

        public void AddPersonalInfo(string hleNo, string applicationDate, string hleStatus, string hhInc, string caInc, string peOic, 
            string caOic, string rejDate, string canDate, string aprDate, string expDate, string[] info, string applicantType,
            int orderNo)
        {
            this.referenceNumber = hleNo;
            this.applicationDate = applicationDate;
            this.status = hleStatus;
            this.houseHoldIncome = hhInc;
            this.creditAssessmentIncome = caInc;
            this.preEligibilityOic = peOic;
            this.creditAssessmentOic = caOic;
            this.applicantType = applicantType;
            nric = info[0].ToUpper();
            name = info[1];
            maritalStatusCode = info[2];
            maritalStatus = info[3];
            dateOfBirth = info[4];
            relationshipCode = info[5];
            relationship = info[6];
            employmentTypeCode = info[7];
            employmentType = info[8];
            date1 = info[9];
            inc1 = info[10];
            date2 = info[11];
            inc2 = info[12];
            date3 = info[13];
            inc3 = info[14];
            date4 = info[15];
            inc4 = info[16];
            date5 = info[17];
            inc5 = info[18];
            date6 = info[19];
            inc6 = info[20];
            date7 = info[21];
            inc7 = info[22];
            date8 = info[23];
            inc8 = info[24];
            date9 = info[25];
            inc9 = info[26];
            date10 = info[27];
            inc10 = info[28];
            date11 = info[29];
            inc11 = info[30];
            date12 = info[31];
            inc12 = info[32];
            annualIncome = info[33];
            averageIncome = info[34];
            employerName = info[35];
            dateJoined = info[36];
            this.rejectedDate = rejDate;
            this.cancelledDate = canDate;
            this.approvedDate = aprDate;
            this.expiredDate = expDate;
            this.orderNo = orderNo;
        }


        public void AddPersonalInfoFromWebService(string hleNo, string applicationDate, string hleStatus, string hhInc, string caInc, string peOic,
            string caOic, string rejDate, string canDate, string aprDate, string expDate, int orderNo, string risk,  Leas.Personal personal)
        {
            this.referenceNumber = hleNo;
            this.applicationDate = applicationDate;
            this.status = hleStatus;
            this.houseHoldIncome = hhInc;
            this.creditAssessmentIncome = caInc;
            this.preEligibilityOic = peOic;
            this.creditAssessmentOic = caOic;
            this.rejectedDate = rejDate;
            this.cancelledDate = canDate;
            this.approvedDate = aprDate;
            this.expiredDate = expDate;
            this.orderNo = orderNo;
            this.risk = risk;   //Added by Edward 2014/02/04

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
            this.annualIncome = (personal.AnnualIncome == null ? string.Empty : personal.AnnualIncome.ToString());
            this.averageIncome = (personal.AverageIncome == null ? string.Empty : personal.AverageIncome.ToString());
            this.employerName = personal.EmployerName;
            this.dateJoined = (personal.EmploymentDate == null ? string.Empty : Format.FormatDateTime(personal.EmploymentDate, DateTimeFormat.dd_MM_yyyy));
            this.employmentTypeCode = personal.EmploymentStatus;
            this.employmentType = EnumManager.MapEmploymentStatus(this.employmentTypeCode);
            this.withAllowance = (personal.WithAllowance == null ? false : personal.WithAllowance);
            this.withCpf = (personal.WithCpf == null ? false : personal.WithCpf);
            this.customerId = (personal.CustomerId == null ? string.Empty : personal.CustomerId);
            this.noOfIncomeMonths = (personal.NumberOfIncomeMonths == null ? 0 : personal.NumberOfIncomeMonths); //Added by Edward 22.10.2013                        

            this.date1 = this.date2 = this.date3 = this.date4 = this.date5 = this.date6 = this.date7 = this.date8 = this.date9 = this.date10 = this.date11 = this.date12 = string.Empty;
            this.inc1 = this.inc2 = this.inc3 = this.inc4 = this.inc5 = this.inc6 = this.inc7 = this.inc8 = this.inc9 = this.inc10 = this.inc11 = this.inc12 = "0";

            if (personal.MonthlyIncomeSet != null)
            {
                #region Income Details
                int startIndex = 12 - (personal.MonthlyIncomeSet.Length > 12 ? 12 : personal.MonthlyIncomeSet.Length);

                for (int cnt = startIndex, incomeStart = 0; cnt < 12; cnt++, incomeStart++)
                {
                    Leas.MonthlyIncome monthlyIncome = personal.MonthlyIncomeSet[incomeStart];

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


       
    }
}
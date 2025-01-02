using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for AppPersonal
/// </summary>
public class AppPersonalData
{
    #region Object Properties
    private string nric = string.Empty;
    private string name = string.Empty;
    private string dateJoinedService = string.Empty;
    private string companyName = string.Empty;
    private string personalType = string.Empty;
    private string employmentType = string.Empty;
    private string month1Name = string.Empty;
    private string month2Name = string.Empty;
    private string month3Name = string.Empty;
    private string month4Name = string.Empty;
    private string month5Name = string.Empty;
    private string month6Name = string.Empty;
    private string month7Name = string.Empty;
    private string month8Name = string.Empty;
    private string month9Name = string.Empty;
    private string month10Name = string.Empty;
    private string month11Name = string.Empty;
    private string month12Name = string.Empty;
    private string month1Value = string.Empty;
    private string month2Value = string.Empty;
    private string month3Value = string.Empty;
    private string month4Value = string.Empty;
    private string month5Value = string.Empty;
    private string month6Value = string.Empty;
    private string month7Value = string.Empty;
    private string month8Value = string.Empty;
    private string month9Value = string.Empty;
    private string month10Value = string.Empty;
    private string month11Value = string.Empty;
    private string month12Value = string.Empty;
    private bool hasSalary = false;
    private int orderNo = 0;
    private string customerId = string.Empty;

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

    public string DateJoinedService
    {
        get { return dateJoinedService; }
        set { dateJoinedService = value; }
    }

    public string CompanyName
    {
        get { return companyName; }
        set { companyName = value; }
    }

    public string PersonalType
    {
        get { return personalType; }
        set { personalType = value; }
    }

    public string EmploymentType
    {
        get { return employmentType; }
        set { employmentType = value; }
    }

    public int OrderNo
    {
        get { return orderNo; }
        set { orderNo = value; }
    }

    public string Month1Name
    {
        get { return month1Name; }
        set { month1Name = value; }
    }

    public string Month2Name
    {
        get { return month2Name; }
        set { month2Name = value; }
    }

    public string Month3Name
    {
        get { return month3Name; }
        set { month3Name = value; }
    }

    public string Month4Name
    {
        get { return month4Name; }
        set { month4Name = value; }
    }

    public string Month5Name
    {
        get { return month5Name; }
        set { month5Name = value; }
    }

    public string Month6Name
    {
        get { return month6Name; }
        set { month6Name = value; }
    }

    public string Month7Name
    {
        get { return month7Name; }
        set { month7Name = value; }
    }

    public string Month8Name
    {
        get { return month8Name; }
        set { month8Name = value; }
    }

    public string Month9Name
    {
        get { return month9Name; }
        set { month9Name = value; }
    }

    public string Month10Name
    {
        get { return month10Name; }
        set { month10Name = value; }
    }

    public string Month11Name
    {
        get { return month11Name; }
        set { month11Name = value; }
    }

    public string Month12Name
    {
        get { return month12Name; }
        set { month12Name = value; }
    }

    public string Month1Value
    {
        get { return month1Value; }
        set { month1Value = value; }
    }

    public string Month2Value
    {
        get { return month2Value; }
        set { month2Value = value; }
    }

    public string Month3Value
    {
        get { return month3Value; }
        set { month3Value = value; }
    }

    public string Month4Value
    {
        get { return month4Value; }
        set { month4Value = value; }
    }

    public string Month5Value
    {
        get { return month5Value; }
        set { month5Value = value; }
    }

    public string Month6Value
    {
        get { return month6Value; }
        set { month6Value = value; }
    }

    public string Month7Value
    {
        get { return month7Value; }
        set { month7Value = value; }
    }

    public string Month8Value
    {
        get { return month8Value; }
        set { month8Value = value; }
    }

    public string Month9Value
    {
        get { return month9Value; }
        set { month9Value = value; }
    }

    public string Month10Value
    {
        get { return month10Value; }
        set { month10Value = value; }
    }

    public string Month11Value
    {
        get { return month11Value; }
        set { month11Value = value; }
    }

    public string Month12Value
    {
        get { return month12Value; }
        set { month12Value = value; }
    }

    public string CustomerId
    {
        get { return customerId; }
        set { customerId = value; }
    }

    public bool HasSalary
    {
        get { return hasSalary; }
        set { hasSalary = value; }
    }

    #region added by Edward 18/12/2013 for Leas Service

    private int noOfIncomeMonths;

    public int NoOfIncomeMonths
    {
        get { return noOfIncomeMonths; }
        set { noOfIncomeMonths = value; }
    }
           
    #endregion

    public AppPersonalData(HleInterface.HleInterfaceRow hleInterface)
	{
        this.Nric = hleInterface.Nric;
        this.Name = hleInterface.Name;
        this.PersonalType = hleInterface.ApplicantType;
        this.DateJoinedService = hleInterface.DateJoined;
        this.CompanyName = hleInterface.EmployerName;
        this.EmploymentType = hleInterface.EmploymentType;
        this.Month1Name = hleInterface.Inc1Date;
        this.Month1Value = hleInterface.Inc1;
        this.Month2Name = hleInterface.Inc2Date;
        this.Month2Value = hleInterface.Inc2;
        this.Month3Name = hleInterface.Inc3Date;
        this.Month3Value = hleInterface.Inc3;
        this.Month4Name = hleInterface.Inc4Date;
        this.Month4Value = hleInterface.Inc4;
        this.Month5Name = hleInterface.Inc5Date;
        this.Month5Value = hleInterface.Inc5;
        this.Month6Name = hleInterface.Inc6Date;
        this.Month6Value = hleInterface.Inc6;
        this.Month7Name = hleInterface.Inc7Date;
        this.Month7Value = hleInterface.Inc7;
        this.Month8Name = hleInterface.Inc8Date;
        this.Month8Value = hleInterface.Inc8;
        this.Month9Name = hleInterface.Inc9Date;
        this.Month9Value = hleInterface.Inc9;
        this.Month10Name = hleInterface.Inc10Date;
        this.Month10Value = hleInterface.Inc10;
        this.Month11Name = hleInterface.Inc11Date;
        this.Month11Value = hleInterface.Inc11;
        this.Month12Name = hleInterface.Inc12Date;
        this.Month12Value = hleInterface.Inc12;
        this.hasSalary = true;
        this.orderNo = hleInterface.OrderNo;
        this.customerId = hleInterface.IsCustomerIdNull() ? string.Empty : hleInterface.CustomerId;
        //Added By Edward 18/12/2013 for Leas Service
        this.NoOfIncomeMonths = hleInterface.IsNoOfIncomeMonthsNull() ? 0 : hleInterface.NoOfIncomeMonths;
    }

    public AppPersonalData(ResaleInterface.ResaleInterfaceRow resaleInterface)
    {
        this.Nric = resaleInterface.Nric;
        this.Name = resaleInterface.Name;
        this.PersonalType = resaleInterface.ApplicantType;
        this.DateJoinedService = string.Empty;
        this.CompanyName = string.Empty;
        this.EmploymentType = resaleInterface.EmploymentType;
        this.Month1Name = string.Empty;
        this.Month1Value = string.Empty;
        this.Month2Name = string.Empty;
        this.Month2Value = string.Empty;
        this.Month3Name = string.Empty;
        this.Month3Value = string.Empty;
        this.Month4Name = string.Empty;
        this.Month4Value = string.Empty;
        this.Month5Name = string.Empty;
        this.Month5Value = string.Empty;
        this.Month6Name = string.Empty;
        this.Month6Value = string.Empty;
        this.Month7Name = string.Empty;
        this.Month7Value = string.Empty;
        this.Month8Name = string.Empty;
        this.Month8Value = string.Empty;
        this.Month9Name = string.Empty;
        this.Month9Value = string.Empty;
        this.Month10Name = string.Empty;
        this.Month10Value = string.Empty;
        this.Month11Name = string.Empty;
        this.Month11Value = string.Empty;
        this.Month12Name = string.Empty;
        this.Month12Value = string.Empty;
        this.HasSalary = false;
        this.orderNo = resaleInterface.OrderNo;
        this.customerId = resaleInterface.IsCustomerIdNull() ? string.Empty : resaleInterface.CustomerId;
        //Added By Edward 19/2/2014 for Sales and Resale Changes
        this.NoOfIncomeMonths = resaleInterface.IsNoOfIncomeMonthsNull() ? 0 : resaleInterface.NoOfIncomeMonths;
    }

    public AppPersonalData(SersInterface.SersInterfaceRow sersInterface)
    {
        this.Nric = sersInterface.Nric;
        this.Name = sersInterface.Name;
        this.PersonalType = sersInterface.ApplicantType;
        this.DateJoinedService = string.Empty;
        this.CompanyName = string.Empty;
        this.EmploymentType = string.Empty;
        this.Month1Name = string.Empty;
        this.Month1Value = string.Empty;
        this.Month2Name = string.Empty;
        this.Month2Value = string.Empty;
        this.Month3Name = string.Empty;
        this.Month3Value = string.Empty;
        this.Month4Name = string.Empty;
        this.Month4Value = string.Empty;
        this.Month5Name = string.Empty;
        this.Month5Value = string.Empty;
        this.Month6Name = string.Empty;
        this.Month6Value = string.Empty;
        this.Month7Name = string.Empty;
        this.Month7Value = string.Empty;
        this.Month8Name = string.Empty;
        this.Month8Value = string.Empty;
        this.Month9Name = string.Empty;
        this.Month9Value = string.Empty;
        this.Month10Name = string.Empty;
        this.Month10Value = string.Empty;
        this.Month11Name = string.Empty;
        this.Month11Value = string.Empty;
        this.Month12Name = string.Empty;
        this.Month12Value = string.Empty;
        this.HasSalary = false;
        this.orderNo = sersInterface.OrderNo;
        this.customerId = sersInterface.IsCustomerIdNull() ? string.Empty : sersInterface.CustomerId;
    }

    public AppPersonalData(SalesInterface.SalesInterfaceRow salesInterface)
    {
        this.Nric = salesInterface.Nric;
        this.Name = salesInterface.Name;
        this.PersonalType = salesInterface.ApplicantType;
        this.DateJoinedService = string.Empty;
        this.CompanyName = string.Empty;
        this.EmploymentType = salesInterface.EmploymentType;
        this.Month1Name = string.Empty;
        this.Month1Value = string.Empty;
        this.Month2Name = string.Empty;
        this.Month2Value = string.Empty;
        this.Month3Name = string.Empty;
        this.Month3Value = string.Empty;
        this.Month4Name = string.Empty;
        this.Month4Value = string.Empty;
        this.Month5Name = string.Empty;
        this.Month5Value = string.Empty;
        this.Month6Name = string.Empty;
        this.Month6Value = string.Empty;
        this.Month7Name = string.Empty;
        this.Month7Value = string.Empty;
        this.Month8Name = string.Empty;
        this.Month8Value = string.Empty;
        this.Month9Name = string.Empty;
        this.Month9Value = string.Empty;
        this.Month10Name = string.Empty;
        this.Month10Value = string.Empty;
        this.Month11Name = string.Empty;
        this.Month11Value = string.Empty;
        this.Month12Name = string.Empty;
        this.Month12Value = string.Empty;
        this.HasSalary = false;
        this.orderNo = salesInterface.OrderNo;
        this.customerId = salesInterface.IsCustomerIdNull() ? string.Empty : salesInterface.CustomerId;
        //Added By Edward 19/2/2014 for Sales and Resale Changes
        this.NoOfIncomeMonths = salesInterface.IsNoOfIncomeMonthsNull() ? 0 : salesInterface.NoOfIncomeMonths;
    }

    #endregion
}
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Dwms.Bll;
using System.Web.Services.Protocols;
using System.Configuration;
using System.IO;
using Dwms.Web;
using System.Collections;
using System.Threading;

/// <summary>
/// Summary description for Sales Household
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Sales : System.Web.Services.WebService
{
    //public AuthenticationSoapHeader soapHeader;
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED = -4;
    private const int INVALID_REFERENCE_NUMBER = -5;
    private const int PERSONAL_WITH_INVALID_NRIC_OR_NAME = -6;
    private const int INVALD_PERSONAL_TYPE = -7;

    public Sales()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    
    #region Log Changes/Fixes
    //******************Log Changes/Fixes*********************
    //Added/Modified By                 Date                Description
    //Edward                            2014/3/12           All forwardedDate is replaced to applicationDate. applicationDate is added in DB Sales and Resale Changes DWMS_8
    //Edward                            2015/02/04          Added ABCDERefNumber Field in Application
    #endregion
    [WebMethod(EnableSession = true)]
    public int SalesHouseholdInfo(AuthenticationClass authentication, string refNumber, string status, string ballotQuarter, string householdType, string allocationMode, 
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress, 
        string flatDesign, string contactNumbers, string affectedSERS, string OIC1, string OIC2, DateTime acceptanceDate, 
        DateTime applicationDate, DateTime keyIssueDate, DateTime cancelledDate, string ABCDERefNumber, bool refreshHouseholdStructure, Personal[] personals)   
    {
        int result = 0;

        // Create the text file
        string logFilePath = WriteInfoToTextFile(authentication, refNumber, status, ballotQuarter, householdType, allocationMode, 
        allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress, 
        flatDesign, contactNumbers, affectedSERS, OIC1, OIC2, acceptanceDate, applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber, refreshHouseholdStructure, personals);

        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Writing Info \r\n ");

        int errorCode;

        try
        {
            errorCode = Validate(authentication, refNumber, status, ballotQuarter, householdType, allocationMode,
                allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                flatDesign, contactNumbers, affectedSERS, acceptanceDate, applicationDate,
                keyIssueDate, cancelledDate, personals);

            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Done Validating: " + errorCode.ToString());
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Error while Validating: " + e.Message.ToString());
            errorCode = -10;
        }
        
        if (errorCode < 0)
        {
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Validation Check (Error Code): " + errorCode.ToString());
            WriteToFile(logFilePath, "===============================");

            try
            {
                File.Move(logFilePath, logFilePath + ".Err");
                logFilePath = logFilePath + ".Err";

                string subject = string.Empty;
                string message = string.Empty;

                subject = "Error importing from Sales web service";
                message = "There is an error code " + errorCode + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.SSD).Trim(), string.Empty, string.Empty, subject, message);

                #region Added by Edward 2014/06/13  OCR/Web Service Error Report
                ExceptionLogDb exceptionLogDb = new ExceptionLogDb();
                exceptionLogDb.Insert("WebService", refNumber, DateTime.Now, subject, message, string.Empty, false);
                #endregion
            }
            catch (Exception ex)
            {
                WriteToFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
            return errorCode;
        }

        #region Retrieve data about the reference number
        string strRefNo = refNumber;
        string strStatus = status;
        string strBallotQuarter = ballotQuarter;
        string strHouseholdType = householdType;
        string strAllocationMode = allocationMode;
        string strAllocationScheme = allocationScheme;
        string strEligibilityScheme = eligibilityScheme;
        string strHouseholdIncome = householdIncome;
        string strFlatType = flatType;
        string strAHGStatus = AHGStatus;
        string strSHGStatus = SHGStatus;
        string strLoanTag = loanTag;
        string strBookedAddress = bookedAddress;
        string strFlatDesign = flatDesign;
        string strContactNumbers = contactNumbers;
        string strAffectedSERS = affectedSERS;
        string strOIC1 = OIC1;
        string strOIC2 = OIC2;
        string accpDate = (acceptanceDate == null || acceptanceDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(acceptanceDate, DateTimeFormat.dd_MM_yyyy));
        string applDate = (applicationDate == null || applicationDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(applicationDate, DateTimeFormat.dd_MM_yyyy));
        string strKeyIssueDate = (keyIssueDate == null || keyIssueDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(keyIssueDate, DateTimeFormat.dd_MM_yyyy));
        string strCancelledDate = (cancelledDate == null || cancelledDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(cancelledDate, DateTimeFormat.dd_MM_yyyy));
        string strABCDERefNumber = ABCDERefNumber;
        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Retrieving Data \r\n ");
        #endregion

        // Insert
        try
        {
            // Insert household structure information
            string refType = Util.GetReferenceType(strRefNo);            
            if (refType.Equals(ReferenceTypeEnum.SALES.ToString()))
            {
                result = SaveSalesInterface(strRefNo, refType, strStatus, strBallotQuarter, strHouseholdType, strAllocationMode,
                    strAllocationScheme, strEligibilityScheme, strHouseholdIncome, strFlatType, strAHGStatus, strSHGStatus, strLoanTag, strBookedAddress,
                    strFlatDesign, strContactNumbers, strAffectedSERS, strOIC1, strOIC2, accpDate, applDate,
                    strKeyIssueDate, strCancelledDate, ABCDERefNumber, refreshHouseholdStructure,  personals, logFilePath);
            }
            else
            {
                result = INVALID_REFERENCE_NUMBER;
            }
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, e.Message);
            result = PROCESSING_ERROR;
        }

        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Process Result: " + result.ToString());
        WriteToFile(logFilePath, "===============================");

        if (result > 100)
            File.Move(logFilePath, logFilePath + ".Upd");

        if (result < 0)
        {
            try
            {
                File.Move(logFilePath, logFilePath + ".Err");

                string subject = string.Empty;
                string message = string.Empty;

                subject = "Error processing from Sales web service";
                message = "There is an error code " + result + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.SSD).Trim(), string.Empty, string.Empty, subject, message);

                #region Added by Edward 2014/06/13  OCR/Web Service Error Report
                ExceptionLogDb exceptionLogDb = new ExceptionLogDb();
                exceptionLogDb.Insert("WebService", refNumber, DateTime.Now, subject, message, string.Empty, false);
                #endregion

            }
            catch (Exception ex)
            {
                WriteToFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
        }

        return result;
    }

    private string WriteInfoToTextFile(AuthenticationClass authentication, string refNumber, string status, string ballotQuarter, string householdType, string allocationMode,
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
        string flatDesign, string contactNumbers, string affectedSERS, string OIC1, string OIC2,
        DateTime acceptanceDate, DateTime applicationDate, DateTime keyIssueDate, DateTime cancelledDate, string ABCDERefNumber,
        bool refreshHouseholdStructure, Personal[] personals)
    {
        // Temporary code to write input to text file
        //string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("SALE-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");
        //string filePath = Server.MapPath("~/App_Data/LogFiles/" + DateTime.Now.ToString("SALE-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        string filePath = Util.GetLogFilesFolder("SALE", refNumber, string.Empty);
        #endregion

        StreamWriter w;
        w = File.CreateText(filePath);

        w.Write("UserName: " + authentication.UserName + "\r\n");
        w.Write("Password: " + authentication.Password + "\r\n");
        w.Write("\r\n");

        w.Write("Ref Number: " + refNumber + "\r\n");
        w.Write("Status: " + status + "\r\n");
        w.Write("Ballot Quarter: " + ballotQuarter + "\r\n");
        w.Write("Household Type: " + householdType + "\r\n");
        w.Write("Allocation Mode: " + allocationMode + "\r\n");
        w.Write("Allocation Scheme: " + allocationScheme + "\r\n");
        w.Write("Eligibility Scheme: " + eligibilityScheme + "\r\n");
        w.Write("Household Income: " + householdIncome + "\r\n");
        w.Write("Flat Type: " + flatType + "\r\n");
        w.Write("AHG Status: " + AHGStatus + "\r\n");
        w.Write("SHG Status: " + SHGStatus + "\r\n");
        w.Write("Loan Tag: " + loanTag + "\r\n");
        w.Write("Booked Address: " + bookedAddress + "\r\n");
        w.Write("Flat Design: " + flatDesign + "\r\n");
        w.Write("contact Numbers: " + contactNumbers + "\r\n");
        w.Write("Affected SERS: " + affectedSERS + "\r\n");
        w.Write("OIC1: " + OIC1 + "\r\n");
        w.Write("OIC2: " + OIC2 + "\r\n");
        w.Write("Acceptance Date: " + acceptanceDate + "\r\n");
        w.Write("Application Date: " + applicationDate + "\r\n");
        w.Write("KeyIssue Date: " + keyIssueDate + "\r\n");
        w.Write("Cancelled Date: " + cancelledDate + "\r\n");
        w.Write("ABCDERefNumber: " + ABCDERefNumber + "\r\n");
        w.Write("RefreshHouseholdStructure: " + refreshHouseholdStructure.ToString().ToUpper() + "\r\n");
        w.Write("\r\n");

        foreach (Personal personal in personals)
        {
            if (personal != null)
            {
                w.Write(personal.IsDeletion == null ? "IsDeletion: " : "IsDeletion: " + personal.IsDeletion.ToString() + "\r\n");
                w.Write("PersonalType: " + personal.PersonalType + "\r\n");
                w.Write("CustomerId: " + personal.CustomerId + "\r\n");
                w.Write(personal.IsMainApplicant == null ? "IsMainApplicant: " : "IsMainApplicant: " + personal.IsMainApplicant.ToString() + "\r\n");
                w.Write("Nric: " + personal.Nric + "\r\n");
                w.Write("Name: " + personal.Name + "\r\n");
                //w.Write("Id Type: " + personal.IdType + "\r\n");
                w.Write("MaritalStatus: " + personal.MaritalStatus + "\r\n");
                w.Write(personal.DateOfBirth == null ? "DateOfBirth: " : "DateOfBirth: " + personal.DateOfBirth.ToString("dd MMM yyyy") + "\r\n");
                w.Write("Relationship: " + personal.Relationship + "\r\n");
                w.Write(personal.Citizenship == null ? "Citizenship: " : "Citizenship: " + personal.Citizenship.ToString() + "\r\n");
                //w.Write("Employment Status: " + personal.Income.ToString() + "\r\n");
                //w.Write("Income: " + personal.Income + "\r\n");
                w.Write(personal.IsMCPS == null ? "IsMCPS: " : "IsMCPS: " + personal.IsMCPS.ToString() + "\r\n");
                w.Write(personal.IsPPO == null ? "IsPPO: " : "IsPPO: " + personal.IsPPO.ToString() + "\r\n");
                w.Write("Nric SOC: " + personal.NRICSOC + "\r\n");
                w.Write("Name SOC: " + personal.NameSOC + "\r\n");
                w.Write(personal.NumberOfIncomeMonths == null ? "Number of Income Months: " : "Number of Income Months: " + personal.NumberOfIncomeMonths.ToString() + "\r\n");
                w.Write("\r\n");

                if (personal.MonthlyIncomeSet != null)
                {
                    foreach (MonthlyIncome monthlyIncome in personal.MonthlyIncomeSet)
                    {
                        w.Write("YearMonth: " + monthlyIncome.YearMonth + "\r\n");
                        w.Write(monthlyIncome.Amount == null ? "Amount: " : "Amount: " + monthlyIncome.Amount.ToString() + "\r\n");
                    }
                }

                w.Write("\r\n");

                if (personal.Documents != null)
                {
                    foreach (Document document in personal.Documents)
                    {
                        w.Write("DocumentTypeCode: " + document.DocumentTypeCode + "\r\n");
                        w.Write("DocumentTypeSubCode: " + document.DocumentTypeSubCode + "\r\n");
                        w.Write("Category: " + document.Category + "\r\n");
                        w.Write("CategoryGroup: " + document.CategoryGroup + "\r\n");
                        w.Write("CategoryDate: " + document.CategoryDate + "\r\n");
                        w.Write("Date: " + document.Date + "\r\n");
                        w.Write("Status: " + document.Status + "\r\n");
                    }
                }

                w.Write("\r\n");
            }
        }

        w.Flush();
        w.Close();

        return filePath;
    }

    private int Validate(AuthenticationClass authentication, string refNumber, string status, string ballotQuarter, string householdType, string allocationMode,
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
        string flatDesign, string contactNumbers, string affectedSERS, DateTime acceptanceDate, 
        DateTime applicationDate, DateTime keyIssueDate, DateTime cancelledDate, Personal[] personals)
    {
        if (!ValidateLogin(authentication))
            return AUTHENTICATION_ERROR; // -1

        if (personals != null && personals.Length > 0)
        {
            foreach (Personal personal in personals)
            {
                if (personal == null)
                    return WRONG_INPUT; // -2

                if (personal.IsDeletion && personal.IsMainApplicant)
                    return DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED; // -4

                //if (String.IsNullOrEmpty(personal.Nric) ||
                //    String.IsNullOrEmpty(personal.Name) ||
                //    (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                //    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (String.IsNullOrEmpty(personal.Nric))// edit by calvin temp remove for XIN, XIN was able to import before 5 feb || (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                //if (!personal.PersonalType.ToUpper().Equals("BU") && personal.IsMainApplicant)
                //    return WRONG_INPUT; // -2

                if (!string.IsNullOrEmpty(personal.CustomerId)) 
                {
                    if (personal.CustomerId.Contains("&#"))
                        return WRONG_INPUT; // -2
                }

                if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (!personal.PersonalType.ToUpper().Equals("HA") && !personal.PersonalType.ToUpper().Equals("OC"))
                    return INVALD_PERSONAL_TYPE; // -7

                if (personal.MonthlyIncomeSet != null)
                {
                    foreach (MonthlyIncome monthlyIncome in personal.MonthlyIncomeSet)
                    {
                        if (monthlyIncome == null)
                            return WRONG_INPUT; // -2
                    }
                }

                if (personal.Documents != null)
                {
                    foreach (Document document in personal.Documents)
                    {
                        if (document == null)
                            return WRONG_INPUT; // -2
                    }
                }
            }
        }

        return 0;
    }

    private int SaveSalesInterface(string refNumber, string refType, string status, string ballotQuarter, string householdType, string allocationMode,
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
        string flatDesign, string contactNumbers, string affectedSERS, string OIC1, string OIC2, string acceptanceDate, 
        string applicationDate, string keyIssueDate, string cancelledDate, string ABCDERefNumber, bool refreshHouseholdStructure, Personal[] personals, string logFilePath)
    {
        int result = 0;
        int resulttemp = 0;

        SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
        DocAppDb docAppDb = new DocAppDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
        DocTypeDb docTypeDb = new DocTypeDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();

        int docAppId = -1;

        #region Insert the Sales number
        try
        {
            docAppId = docAppDb.InsertRefNumber(refNumber, refType, OIC2, OIC1, null, false, string.Empty, false, string.Empty, string.Empty);
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Done Insert/Update Sales number\r\n ");
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, "===============================");
            WriteToFile(logFilePath, "Insert/Update OIC Exception: " + e.Message);
            result = PROCESSING_ERROR;
        }
        #endregion

        #region Check if Personal in RefNo
        resulttemp = result;
        try
        {
            if (salesInterfaceDb.DoesPersonalExistsByRefNo(refNumber))
            {
                for (int cnt = 0; cnt < 3; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts updating Sales: " + (cnt + 1).ToString());
                    try
                    {
                        //Update the application
                        if (salesInterfaceDb.UpdateApplication(refNumber, refType, status, ballotQuarter, householdType, allocationMode,
                        allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                        flatDesign, contactNumbers, affectedSERS, acceptanceDate, applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber))
                        {
                            result = resulttemp;
                            break;
                        }
                        else
                        {
                            result = INVALID_REFERENCE_NUMBER;//Ref no not found for updating
                            WriteToFile(logFilePath, " Failed\r\n");
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Update Sales Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }
            else if (personals == null || personals.Length <= 0)
            {
                WriteToFile(logFilePath, "\r\nAttempts updating Sales OIC: Failed\r\n");
                result = INVALID_REFERENCE_NUMBER;//Ref no not found for updating
            }
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, "===============================");
            WriteToFile(logFilePath, "Update Application Exception: " + e.Message);
            result = PROCESSING_ERROR;
        }
        #endregion

        if (personals != null && personals.Length > 0)
        {
            // If the refresh flag is set, remove the existing record for the reference number
            #region Delete Refno is RefreshHouseHoldStructure is True
            if (refreshHouseholdStructure && salesInterfaceDb.DoesPersonalExistsByRefNo(refNumber))
            {
                resulttemp = result;
                for (int cnt = 0; cnt < 2; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts deleting Sales: " + (cnt + 1).ToString());
                    try
                    {
                        if (salesInterfaceDb.DeleteByRefNo(refNumber))
                        {
                            result = resulttemp;
                            break;
                        }
                        else
                        {
                            result = resulttemp + 200;
                            WriteToFile(logFilePath, " Failed\r\n");//Ref no not found for deleting
                            break;

                            #region commented by unknown
                            //if (!logFilePath.EndsWith(".Upd"))
                            //{
                            //    File.Move(logFilePath, logFilePath + ".Upd");
                            //    logFilePath = logFilePath + ".Upd";
                            //}
                            #endregion
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Sales Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }
            #endregion

            int inserted = 0;
            int deleted = 0;

            #region Insert the OC personal info
            Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Personal personal in sortedOcPersonal)
            {
                #region Insert the Sales household structure data for OC
                SalesPersonal salesPersonal = new SalesPersonal();
                salesPersonal.AddPersonalInfoFromWebService(refNumber, status, ballotQuarter, householdType, allocationMode,
                    allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                    flatDesign, contactNumbers, affectedSERS, OIC1, OIC2, acceptanceDate, applicationDate, 
                    keyIssueDate, cancelledDate, ABCDERefNumber, ocOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Nric) ||
                            //SalesInterfaceDb.DoesPersonalExistsByRefNoCustomerId(salesPersonal.RefNumber, salesPersonal.CustomerId) ||
                            salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Name))
                            isSaved = salesInterfaceDb.Update(salesPersonal);
                        else
                            isSaved = salesInterfaceDb.Insert(salesPersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }

                    #region commented by unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, salesPersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, salesPersonal.RefNumber, salesPersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Sales(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(salesPersonal.RefNumber, salesPersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    #region commented by unknown
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(salesPersonal.RefNumber.Trim(),
                                    //    salesPersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(salesPersonal.RefNumber, salesPersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    #endregion
                                    leasInterfaceDb.Insert(salesPersonal.RefNumber, salesPersonal.Nric, docTypeCode, document.Date,
                                        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                }
                                else
                                {
                                    WriteToFile(logFilePath, "===============================");
                                    WriteToFile(logFilePath, "Exception: Invalid DocumentId: " + document.DocumentTypeCode + " AND DocumentSubId: " + document.DocumentTypeSubCode);
                                    result = WRONG_INPUT;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update (OC) Pending Doc Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion
            }
            #endregion

            #region Insert the HA personal info
            Personal[] sortedBuPersonal = RetrievePersonals(personals, "HA");

            foreach (Personal personal in sortedBuPersonal)
            {
                #region Insert the Sales household structure data for HA
                int buOrderNo = salesInterfaceDb.GetOrderNumberForNric(refNumber, personal.PersonalType, personal.Nric);

                SalesPersonal salesPersonal = new SalesPersonal();
                salesPersonal.AddPersonalInfoFromWebService(refNumber, status, ballotQuarter, householdType, allocationMode,
                    allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                    flatDesign, contactNumbers, affectedSERS, OIC1, OIC2, acceptanceDate, applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber, buOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Nric) ||
                            //(salesPersonal.CustomerId.Length > 0 && SalesInterfaceDb.DoesPersonalExistsByRefNoCustomerId(salesPersonal.RefNumber, salesPersonal.CustomerId)) ||
                            salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Name))
                            isSaved = salesInterfaceDb.Update(salesPersonal);
                        else
                            isSaved = salesInterfaceDb.Insert(salesPersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    #region commented by unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, salesPersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, salesPersonal.RefNumber, salesPersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Sales(HA) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(salesPersonal.RefNumber, salesPersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {

                                    #region commented by unknown
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(salesPersonal.RefNumber.Trim(),
                                    //    salesPersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(salesPersonal.RefNumber, salesPersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    #endregion

                                    leasInterfaceDb.Insert(salesPersonal.RefNumber, salesPersonal.Nric, docTypeCode, document.Date,
                                        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                }
                                else
                                {
                                    WriteToFile(logFilePath, "===============================");
                                    WriteToFile(logFilePath, "Exception: Invalid DocumentId: " + document.DocumentTypeCode + " AND DocumentSubId: " + document.DocumentTypeSubCode);
                                    result = WRONG_INPUT;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update (HA) Pending Doc Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion
            }
            #endregion  

            #region Delete all personal marked for deletion
            Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Personal personal in toBeDeletedPersonal)
            {
                if (salesInterfaceDb.IsMainApplicant(refNumber, personal.Nric))
                {
                    result = DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED;
                    hasMainApplicantForDelete = true;
                    break;
                }
            }

            if (!hasMainApplicantForDelete)
            {
                foreach (Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        // Check if the personal has a record in AppPersonal table with documents tied to it
                        if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        {
                        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNumber, personal.Nric);

                        if (isSaved)
                            result = (++deleted);
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Sales Exception: " + e.Message);
                        result = PROCESSING_ERROR;
                    }
                    #endregion
                }
            }
            #endregion

            // Record the number of transactions
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Records inserted/updated: " + inserted.ToString());
            WriteToFile(logFilePath, "Records deleted: " + deleted.ToString());
            
            #region Sort by NRIC
            // Update the Order Nos
            resulttemp = result;
            for (int cnt = 0; cnt < 3; cnt++)
            {
                try
                {
                    WriteToFile(logFilePath, "\r\nAttempts update Orders: " + (cnt + 1).ToString());
                    if (salesInterfaceDb.UpdateOrderNosOfOcOccupiers(refNumber) || salesInterfaceDb.UpdateOrderNosOfHaApplicants(refNumber))
                    {
                        result = resulttemp;
                        //break;
                    }
                    else
                    {
                        result = resulttemp + 200;
                        WriteToFile(logFilePath, " Failed\r\n");
                        #region commented by unknown
                        //if (!logFilePath.EndsWith(".Upd"))
                        //{
                        //    File.Move(logFilePath, logFilePath + ".Upd");
                        //    logFilePath = logFilePath + ".Upd";
                        //}
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Update Order Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                Thread.Sleep(1000);
            }
            #endregion

            // Refresh the household structure
            try
            {
                appPersonalDb.SavePersonalRecords(docAppId);
                #region Added By Edward 19/02/2014  Sales and Resale Changes
                DocApp.DocAppDataTable docAppdt = docAppDb.GetDocAppById(docAppId);
                if (docAppdt.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppRow = docAppdt[0];
                    if (!docAppRow.IsAssessmentStatusNull())
                        IncomeDb.InsertMonthYearInIncomeTable(docAppId, IncomeMonthsSourceEnum.W);
                    //IncomeDb.InsertMonthYearInIncomeTableFromLeas(docAppId); Commented By Edward 29/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
                }
                #endregion
            }
            catch (Exception e)
            {
                WriteToFile(logFilePath, "===============================");
                WriteToFile(logFilePath, "Update Personal Exception: " + e.Message);
                result = PROCESSING_ERROR;
            }
        }

        #region Attempt to update SERS Household is affectedSERS is not empty
        if (affectedSERS.Trim().Length > 0)
        {
            SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
            
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "SERS affected: " + affectedSERS);

            #region Delete Refno is RefreshHouseHoldStructure is True
            if (refreshHouseholdStructure)
            {
                resulttemp = result;
                for (int cnt = 0; cnt < 2; cnt++)
                {

                    WriteToFile(logFilePath, "\r\nAttempts deleting Sers affected: " + (cnt + 1).ToString());
                    try
                    {
                        if (sersInterfaceDb.DeleteByRefNo(affectedSERS))
                        {
                            result = resulttemp;
                            break;
                        }
                        else
                        {
                            result = resulttemp + 200;
                            WriteToFile(logFilePath, " Failed\r\n");//Ref no not found for deleting
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Sers Affected Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }
            #endregion

            int inserted = 0;
            int deleted = 0;

            #region Insert the OC personal info
            Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Personal personal in sortedOcPersonal)
            {
                #region Insert the Sers household structure data for OC
                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromSales(personal, affectedSERS, ocOrderNo++);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(affectedSERS, personal.Nric) ||
                            //sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            sersInterfaceDb.DoesPersonalExistsBySchAccName(affectedSERS, personal.Name))
                            isSaved = sersInterfaceDb.UpdateBySales(sersPersonal);
                        else
                            isSaved = sersInterfaceDb.Insert(sersPersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Sers Affected (OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion
            }
            #endregion

            #region Insert the HA personal info
            Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");

            
            foreach (Personal personal in sortedHaPersonal)
            {
                #region Insert the Sers household structure data for HA
                int orderNo = sersInterfaceDb.GetOrderNumberForNric(affectedSERS, personal.PersonalType, personal.Nric);

                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromSales(personal,affectedSERS, orderNo);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(affectedSERS, personal.Nric) ||
                            //sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            sersInterfaceDb.DoesPersonalExistsBySchAccName(affectedSERS, personal.Name))
                            isSaved = sersInterfaceDb.UpdateBySales(sersPersonal);
                        else
                            isSaved = sersInterfaceDb.Insert(sersPersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Sers Affected(HA) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion
            }
            #endregion

            #region Delete all personal marked for deletion
            Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Personal personal in toBeDeletedPersonal)
            {
                if (salesInterfaceDb.IsMainApplicant(refNumber, personal.Nric))
                {
                    result = DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED;
                    hasMainApplicantForDelete = true;
                    break;
                }
            }

            if (!hasMainApplicantForDelete)
            {
                foreach (Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        // Check if the personal has a record in AppPersonal table with documents tied to it
                        if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        {
                            isSaved = DeleteFromSersInterface(affectedSERS, personal.Nric);

                            if (isSaved)
                                result = (++deleted);
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Sers Affected Exception: " + e.Message);
                        result = PROCESSING_ERROR;
                    }
                    #endregion
                }
            }
            #endregion

            resulttemp = result;
            for (int cnt = 0; cnt < 3; cnt++)
            {
                try
                {
                    WriteToFile(logFilePath, "\r\nAttempts update Orders: " + (cnt + 1).ToString());
                    if (sersInterfaceDb.UpdateOrderNosOfOcOccupiers(affectedSERS) || sersInterfaceDb.UpdateOrderNosOfHaApplicants(affectedSERS))
                    {
                        result = resulttemp;
                        break;
                    }
                    else
                    {
                        result = resulttemp + 200;
                        WriteToFile(logFilePath, " Failed\r\n");
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Update Order Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                Thread.Sleep(1000);
            }
            
        }
        #endregion

        return result;
    }

    private bool CheckIfPersonalHasDocuments(int docAppId, string nric)
    {
        bool result = false;

        AppPersonalDb appPersonalDb = new AppPersonalDb();
        AppDocRefDb appDocRefDb = new AppDocRefDb();

        // Check if the personal exists in the AppPersonal table
        AppPersonal.AppPersonalDataTable appPersonalTable = appPersonalDb.GetAppPersonalByNricAndDocAppId(nric, docAppId);

        if (appPersonalTable.Rows.Count > 0)
        {
            AppPersonal.AppPersonalRow appPersonalRow = appPersonalTable[0];
            int appPersonalId = appPersonalRow.Id;

            // Check if the AppPersonal is being reference by a document
            result = (appDocRefDb.GetAppDocRefByAppPersonalId(appPersonalId).Rows.Count > 0);
        }

        return result;
    }

    private bool DeleteFromInterfaceAndAppPersonal(int docAppId, string refNo, string nric)
    {
        SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();

        // Delete from AppPersonal table
        appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);  //uncommented by Edward 11/3/2014 Sales and Resale Changes

        // Delete from SrsInterface table
        leasInterfaceDb.Delete(refNo, nric);

        //// Get the applicant type of the personal info
        //string applicantType = SalesInterfaceDb.GetApplicantType(refNo, nric);

        // Delete the applicant info for the application if marked for deletion
        return salesInterfaceDb.DeleteByRefNoAndNric(refNo, nric);

        //// Update the Order numbers of the rest of the personal data
        //SalesInterfaceDb.UpdateOrderNos(refNo, applicantType, (applicantType.ToUpper().Equals("HA")));

        //return true;
    }

    private bool DeleteFromSersInterface(string refNo, string nric)
    {
        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
        // Delete the applicant info for the application if marked for deletion
        return sersInterfaceDb.DeleteByRefNoAndNric(refNo, nric);
    }

    private string GetMainApplicantNric(Personal[] personals)
    {
        string mainApplicantNric = string.Empty;

        foreach (Personal personal in personals)
        {
            if (personal.IsMainApplicant)
            {
                mainApplicantNric = personal.Nric;
                break;
            }
        }

        return mainApplicantNric;
    }

    private Personal[] RetrievePersonals(Personal[] personals, string applicantType)
    {
        List<Personal> temp = new List<Personal>();

        // Sort the B personals
        if (applicantType.ToUpper().Equals("HA"))
        {
            #region Get all HA
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("HA"))
                    {
                        if (personal.IsMainApplicant)
                        {
                            mainApplicantNric = personal.Nric;
                            temp.Add(personal);
                            break;
                        }
                    }
                }
            }

            int count = (String.IsNullOrEmpty(mainApplicantNric) ? 0 : 1);

            // Get the rest of the HA personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("HA"))
                    {
                        if (!personal.Nric.Equals(mainApplicantNric))
                        {
                            temp.Add(personal);
                        }
                    }
                }
            }
            #endregion
        }
        else if (applicantType.ToUpper().Equals("OC"))
        {
            #region Get all OC
            int count = 0;

            // Get the OC personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("OC"))
                    {
                        temp.Add(personal);
                        count++;
                    }
                }
            }
            #endregion
        }
        else
        {
            #region Get all personal for deletion
            // Get the OC personals
            foreach (Personal personal in personals)
            {
                if (personal.IsDeletion)
                {
                    temp.Add(personal);
                }
            }
            #endregion
        }

        Personal[] newPersonals = new Personal[temp.Count];

        int cnt = 0;
        foreach (Personal personal in temp)
        {
            newPersonals[cnt] = personal;
            cnt++;
        }

        return newPersonals;
    }

    private void WriteToFile(string logFilePath, string contents)
    {
        try
        {
            StreamWriter w;
            w = File.AppendText(logFilePath);
            w.Write(contents + "\r\n");
            w.Flush();
            w.Close();
        }
        catch (Exception)
        {
        }
    }

    public class Personal
    {
        public bool IsDeletion;
        public string PersonalType;
        public string CustomerId;
        public bool IsMainApplicant;
        public string Nric;
        public string Name;
        //public string IdType;
        public string MaritalStatus;
        public DateTime DateOfBirth;
        public string Relationship;
        public string Citizenship;
        public string EmploymentStatus;
        //public string Income;
        public bool IsMCPS;
        public bool IsPPO;
        public string NRICSOC;
        public string NameSOC;
        public MonthlyIncome[] MonthlyIncomeSet;
        public Document[] Documents;
        public int NumberOfIncomeMonths;    //Added By Edward 28.10.2013
        
    }

    public class MonthlyIncome
    {
        public string YearMonth;
        public decimal Amount;
    }

    public class Document
    {
        public string DocumentTypeCode;
        public string DocumentTypeSubCode;
        public string Category;
        public string CategoryGroup;
        public string CategoryDate;
        public string Date;
        public string Status;
    }

    public class AuthenticationClass
    {
        public string UserName;
        public string Password;
    }

    private static bool ValidateLogin(AuthenticationClass authentication)
    {
        if (authentication == null)
        {
            return false;
        }

        return authentication.UserName == ConfigurationManager.AppSettings["WebServiceUserName"] &&
            authentication.Password == ConfigurationManager.AppSettings["WebServicePassword"];

    }

}

#region Commented for Old affectedSers Logic
//if (!refreshHouseholdStructure && affectedSERS.Trim().Length > 0)
//        {
//            SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
//            if (sersInterfaceDb.DoesPersonalExistsByRefNo(affectedSERS))
//            {
//                WriteToFile(logFilePath, "\r\n===============================");
//                WriteToFile(logFilePath, "SERS affected: " + affectedSERS);

//            #region Insert the OC personal info
//            Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

//            //int ocOrderNo = 1;
//            foreach (Personal personal in sortedOcPersonal)
//            {
//                #region Insert the Sers household structure data for OC
//                SersPersonal sersPersonal = new SersPersonal();
//                sersPersonal.AddPersonalInfoFromSales(personal);

//                bool isSaved = false;
//                try
//                {
//                    if (!personal.IsDeletion)
//                    {
//                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(affectedSERS, personal.Nric) ||
//                            //sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
//                            sersInterfaceDb.DoesPersonalExistsBySchAccName(affectedSERS, personal.Name))
//                            isSaved = sersInterfaceDb.UpdateBySales(sersPersonal);
//                        //else
//                        //    isSaved = sersInterfaceDb.Insert(personal) > 0;

//                        //if (isSaved)
//                        //    result = (++inserted);
//                    }
//                }
//                catch (Exception e)
//                {
//                    WriteToFile(logFilePath, "===============================");
//                    WriteToFile(logFilePath, "Insert/Update Sers(OC) Exception: " + e.Message);
//                    result = PROCESSING_ERROR;
//                }
//                #endregion
//            }
//            #endregion

//            #region Insert the HA personal info
//            Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");

//            int ocOrderNo = 1;
//            foreach (Personal personal in sortedHaPersonal)
//            {
//                #region Insert the Sers household structure data for HA
//                SersPersonal sersPersonal = new SersPersonal();
//                sersPersonal.AddPersonalInfoFromSales(personal);

//                bool isSaved = false;
//                try
//                {
//                    if (!personal.IsDeletion)
//                    {
//                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(affectedSERS, personal.Nric) ||
//                            //sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
//                            sersInterfaceDb.DoesPersonalExistsBySchAccName(affectedSERS, personal.Name))
//                            isSaved = sersInterfaceDb.UpdateBySales(sersPersonal);
//                        //else
//                        //    isSaved = sersInterfaceDb.Insert(personal) > 0;

//                        //if (isSaved)
//                        //    result = (++inserted);
//                    }
//                }
//                catch (Exception e)
//                {
//                    WriteToFile(logFilePath, "===============================");
//                    WriteToFile(logFilePath, "Insert/Update Sers(OC) Exception: " + e.Message);
//                    result = PROCESSING_ERROR;
//                }
//                #endregion
//            }
//            #endregion
//            }
//        }
#endregion
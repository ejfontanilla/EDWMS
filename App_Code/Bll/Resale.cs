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
/// Summary description for Resale Household
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Resale : System.Web.Services.WebService
{
    //public AuthenticationSoapHeader soapHeader;
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED = -4;
    private const int INVALID_REFERENCE_NUMBER = -5;
    private const int PERSONAL_WITH_INVALID_NRIC_OR_NAME = -6;
    private const int INVALD_PERSONAL_TYPE = -7;

    public Resale()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public int ResaleHouseholdInfo(AuthenticationClass authentication, string caseNumber, string schemeAccount, string oldLesseeCode, string newLesseeCode, 
        string buyerAllocationScheme, string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme,
        string householdType, string AHGGrant, string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome,
        DateTime applicationDate, DateTime appointmentDate, DateTime completionDate, DateTime cancellationDate, bool refreshHouseholdStructure, Personal[] personals)
    {
        int result = 0;

        // Create the text file
        string logFilePath = WriteInfoToTextFile(authentication, caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme, 
            buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount, 
            householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, refreshHouseholdStructure, personals);

        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Writing Info \r\n ");

        int errorCode;
        try
        {
            errorCode = Validate(authentication, caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, personals);
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

                subject = "Error importing from Resale web service";
                message = "There is an error code " + errorCode + " for reference no. " + caseNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.RSD).Trim(), string.Empty, string.Empty, subject, message);

                #region Added by Edward 2014/06/13  OCR/Web Service Error Report
                ExceptionLogDb exceptionLogDb = new ExceptionLogDb();
                exceptionLogDb.Insert("WebService", caseNumber, DateTime.Now, subject, message, string.Empty, false);
                #endregion

            }
            catch (Exception ex)
            {
                WriteToFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
            return errorCode;
        }

        #region Retrieve data about the reference number
        string strRefNo = caseNumber;
        string strSchemeAccount = schemeAccount;
        string strOldLesseeCode = oldLesseeCode;
        string strNewLesseeCode = newLesseeCode;
        string strBuyerAllocationScheme = buyerAllocationScheme;
        string strBuyerEligibilityScheme = buyerEligibilityScheme;
        string strCodeRecipient = codeRecipient;
        string strROIC = ROIC;
        string strCaseOIC = caseOIC;
        string strEligibilityScheme = eligibilityScheme;
        string strHouseholdType = householdType;
        string strAHGGrant = AHGGrant;
        string strHH2YINC = HH2YINC;
        string strBankLoanAmount = bankLoanAmount;
        string strABCDELoanAmount = ABCDELoanAmount;
        string strHouseholdIncome = householdIncome;
        string strCAIncome = CAIncome;
        string strApplicationDate = (applicationDate == null || applicationDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(applicationDate, DateTimeFormat.dd_MM_yyyy));
        string strAppointmentDate = (appointmentDate == null || appointmentDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(appointmentDate, DateTimeFormat.dd_MM_yyyy)); 
        string strCompletionDate = (completionDate == null || completionDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(completionDate, DateTimeFormat.dd_MM_yyyy)); 
        string strCancellationDate = (cancellationDate == null || cancellationDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(cancellationDate, DateTimeFormat.dd_MM_yyyy));
        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Retrieving Data \r\n ");
        #endregion

        // Insert
        try
        {
            // Insert household structure information
            string refType = Util.GetReferenceType(strRefNo);

            if (refType.Equals(ReferenceTypeEnum.RESALE.ToString()))
            {
                result = SaveResaleInterface(strRefNo, refType, strSchemeAccount, strOldLesseeCode, strNewLesseeCode, strBuyerAllocationScheme,
                    strBuyerEligibilityScheme, strCodeRecipient, strROIC, strCaseOIC, strEligibilityScheme, strHouseholdType, strAHGGrant, strHH2YINC, strBankLoanAmount, strABCDELoanAmount,
                    strHouseholdIncome, strCAIncome, strApplicationDate, strAppointmentDate, strCompletionDate, strCancellationDate, refreshHouseholdStructure, personals, logFilePath);
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

                subject = "Error processing from Resale web service";
                message = "There is an error code " + result + " for reference no. " + caseNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.RSD).Trim(), string.Empty, string.Empty, subject, message);

                #region Added by Edward 2014/06/13  OCR/Web Service Error Report
                ExceptionLogDb exceptionLogDb = new ExceptionLogDb();
                exceptionLogDb.Insert("WebService", caseNumber, DateTime.Now, subject, message, string.Empty, false);
                #endregion

            }
            catch (Exception ex)
            {
                WriteToFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
        }

        return result;
    }

    private string WriteInfoToTextFile(AuthenticationClass authentication, string caseNumber, string schemeAccount, string oldLesseeCode, string newLesseeCode,
        string buyerAllocationScheme, string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme,
        string householdType, string AHGGrant, string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome,
        DateTime applicationDate, DateTime appointmentDate, DateTime completionDate, DateTime cancellationDate, bool refreshHouseholdStructure, Personal[] personals)
    {
        // Temporary code to write input to text file
        #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
        //string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("RESALE-yyyyMMdd-HHmmss.fff") + "-" + caseNumber + ".txt");
        //string filePath = Server.MapPath("~/App_Data/LogFiles/" + DateTime.Now.ToString("RESALE-yyyyMMdd-HHmmss.fff") + "-" + caseNumber + ".txt");
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        string filePath = Util.GetLogFilesFolder("RESALE", caseNumber, string.Empty);
        #endregion

        StreamWriter w;
        w = File.CreateText(filePath);

        w.Write("UserName: " + authentication.UserName + "\r\n");
        w.Write("Password: " + authentication.Password + "\r\n");
        w.Write("\r\n");

        w.Write("Case Number: " + caseNumber + "\r\n");
        w.Write("scheme Account: " + schemeAccount + "\r\n");
        w.Write("Old Lessee Code: " + oldLesseeCode + "\r\n");
        w.Write("New Lessee Code: " + newLesseeCode + "\r\n");
        w.Write("Buyer Allocation Scheme: " + buyerAllocationScheme + "\r\n");
        w.Write("Buyer Eligibility Scheme: " + buyerEligibilityScheme + "\r\n");
        w.Write("Code Recipient: " + codeRecipient + "\r\n");
        w.Write("ROIC: " + ROIC + "\r\n");
        w.Write("CaseOIC: " + caseOIC + "\r\n");
        w.Write("Eligibility Scheme: " + eligibilityScheme + "\r\n");
        w.Write("Household Type: " + householdType + "\r\n");
        w.Write("AHGGrant: " + AHGGrant + "\r\n");
        w.Write("HH2YINC: " + HH2YINC + "\r\n");
        w.Write("Bank Loan Amount: " + bankLoanAmount + "\r\n");
        w.Write("ABCDE Loan Amount: " + ABCDELoanAmount + "\r\n");
        w.Write("Household Income: " + householdIncome + "\r\n");
        w.Write("CA Income: " + CAIncome + "\r\n");
        w.Write("Application Date: " + applicationDate + "\r\n");
        w.Write("Appointment Date: " + appointmentDate + "\r\n");
        w.Write("Completion Date: " + completionDate + "\r\n");
        w.Write("Cancellation Date: " + cancellationDate + "\r\n");
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
                w.Write("Id Type: " + personal.IdType + "\r\n");
                w.Write("MaritalStatus: " + personal.MaritalStatus + "\r\n");
                w.Write(personal.DateOfBirth == null ? "DateOfBirth: " : "DateOfBirth: " + personal.DateOfBirth.ToString("dd MMM yyyy") + "\r\n");
                w.Write("Relationship: " + personal.Relationship + "\r\n");
                w.Write(personal.Citizenship == null ? "Citizenship: " : "Citizenship: " + personal.Citizenship.ToString() + "\r\n");
                w.Write("Employment Status: " + personal.EmploymentStatus.ToString() + "\r\n");
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

    private int Validate(AuthenticationClass authentication, string caseNumber, string schemeAccount, string oldLesseeCode, string newLesseeCode,
        string buyerAllocationScheme, string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme,
        string householdType, string AHGGrant, string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome,
        DateTime applicationDate, DateTime appointmentDate, DateTime completionDate, DateTime cancellationDate, Personal[] personals)
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

                if (personal.CustomerId.Contains("&#"))
                    return WRONG_INPUT; // -2

                if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (!personal.PersonalType.ToUpper().Equals("BU") && !personal.PersonalType.ToUpper().Equals("OC") && 
                    !personal.PersonalType.ToUpper().Equals("PR") && !personal.PersonalType.ToUpper().Equals("CH") && 
                    !personal.PersonalType.ToUpper().Equals("SE") && !personal.PersonalType.ToUpper().Equals("SP"))
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

    private int SaveResaleInterface(string caseNumber, string refType, string schemeAccount, string oldLesseeCode, string newLesseeCode, string buyerAllocationScheme, 
        string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme, string householdType, string AHGGrant,
        string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome, string applicationDate,
        string appointmentDate, string completionDate, string cancellationDate, bool refreshHouseholdStructure, Personal[] personals, string logFilePath)
    {
        int result = 0;
        int resulttemp = 0;

        ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
        DocAppDb docAppDb = new DocAppDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
        DocTypeDb docTypeDb = new DocTypeDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();

        int docAppId = -1;

        #region Insert the Resale number
        try
        {
            docAppId = docAppDb.InsertRefNumber(caseNumber, refType, caseOIC, ROIC, null, false, string.Empty, false, string.Empty, string.Empty);
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
            if (resaleInterfaceDb.DoesPersonalExistsByRefNo(caseNumber))
            {
                for (int cnt = 0; cnt < 3; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts updating Resale: " + (cnt + 1).ToString());
                    try
                    {
                        // Update the application
                        if (resaleInterfaceDb.UpdateApplication(caseNumber,ROIC, caseOIC, AHGGrant,HH2YINC,schemeAccount, oldLesseeCode,
                        newLesseeCode,buyerAllocationScheme,buyerEligibilityScheme,bankLoanAmount,ABCDELoanAmount,CAIncome,codeRecipient,eligibilityScheme,householdType,
                        applicationDate, appointmentDate, completionDate,cancellationDate, householdIncome))
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
                        WriteToFile(logFilePath, "Update Resale Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }
            else if (personals == null || personals.Length <= 0)
            {
                WriteToFile(logFilePath, "\r\nAttempts updating Resale OIC: Failed\r\n");
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
            if (refreshHouseholdStructure && resaleInterfaceDb.DoesPersonalExistsByRefNo(caseNumber))
            {
                resulttemp = result;
                for (int cnt = 0; cnt < 2; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts deleting Resale: " + (cnt + 1).ToString());
                    try
                    {
                        if (resaleInterfaceDb.DeleteByRefNo(caseNumber))
                        {
                            result = resulttemp;
                            break;
                        }
                        else
                        {
                            result = resulttemp + 200;
                            WriteToFile(logFilePath, " Failed\r\n");//Ref no not found for deleting
                            break;
                            //if (!logFilePath.EndsWith(".Upd"))
                            //{
                            //    File.Move(logFilePath, logFilePath + ".Upd");
                            //    logFilePath = logFilePath + ".Upd";
                            //}
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Resale Exception: " + e.Message + "\r\n");
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
                #region Insert the Resale household structure data for OC
                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, ocOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "OC") ||
                            //ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "OC"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    #region commented by unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Insert the PR personal info
            Personal[] sortedPRPersonal = RetrievePersonals(personals, "PR");

            int prOrderNo = 1;
            foreach (Personal personal in sortedPRPersonal)
            {
                #region Insert the Resale household structure data for OC
                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, prOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "PR") ||
                            //ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "PR"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    #region Commented by Unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                        leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Insert the CH personal info
            Personal[] sortedPrPersonal = RetrievePersonals(personals, "CH");

            int chOrderNo = 1;
            foreach (Personal personal in sortedPrPersonal)
            {
                #region Insert the Resale household structure data for OC
                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, chOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "CH") ||
                            //ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "CH"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                        leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Insert the SE personal info
            Personal[] sortedSePersonal = RetrievePersonals(personals, "SE");

            int seOrderNo = 1;
            foreach (Personal personal in sortedSePersonal)
            {
                #region Insert the Resale household structure data for OC
                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, seOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "SE") ||
                            //ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "SE"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    #region commented by unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                        leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Insert the SP personal info
            Personal[] sortedSpPersonal = RetrievePersonals(personals, "SP");

            int spOrderNo = 1;
            foreach (Personal personal in sortedSpPersonal)
            {
                #region Insert the Resale household structure data for OC
                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, spOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "SP") ||
                            //ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "SP"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                        leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Insert the BU personal info
            Personal[] sortedBuPersonal = RetrievePersonals(personals, "BU");

            foreach (Personal personal in sortedBuPersonal)
            {
                #region Insert the Resale household structure data for HA
                int buOrderNo = resaleInterfaceDb.GetOrderNumberForNric(caseNumber, personal.PersonalType, personal.Nric);

                ResalePersonal resalePersonal = new ResalePersonal();
                resalePersonal.AddPersonalInfoFromWebService(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                    buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                    householdIncome, CAIncome, applicationDate, appointmentDate, completionDate, cancellationDate, buOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(resalePersonal.CaseNo, resalePersonal.Nric, "BU") ||
                            //(hlePersonal.CustomerId.Length > 0 && ResaleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId)) ||
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "BU"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    #region commented by unknown
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, hlePersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, hlePersonal.HleNumber, hlePersonal.Nric);

                    //        if (isSaved)
                    //            result = (++successCnt);
                    //    }
                    //}
                    #endregion
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Resale(HA) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(resalePersonal.CaseNo, resalePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(resalePersonal.CaseNo.Trim(),
                                    //    resalePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    leasInterfaceDb.Insert(resalePersonal.CaseNo, resalePersonal.Nric, docTypeCode, document.Date,
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

            #region Commented by unknown
            #region Delete all personal marked for deletion
            Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Personal personal in toBeDeletedPersonal)
            {
                if (resaleInterfaceDb.IsMainApplicant(caseNumber, personal.Nric))
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
                            isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, caseNumber, personal.Nric);

                            if (isSaved)
                                result = (++deleted);
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete Resale Exception: " + e.Message);
                        result = PROCESSING_ERROR;
                    }
                    #endregion
                }
            }
            #endregion
            #endregion

            #region Sort by NRIC
            // Record the number of transactions
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Records inserted/updated: " + inserted.ToString());
            WriteToFile(logFilePath, "Records deleted: " + deleted.ToString());

            // Update the Order Nos
            resulttemp = result;
            for (int cnt = 0; cnt < 3; cnt++)
            {
                try
                {
                    WriteToFile(logFilePath, "\r\nAttempts update Orders: " + (cnt + 1).ToString());
                    if (resaleInterfaceDb.UpdateOrderNosOfOcOccupiers(caseNumber, "CH") || resaleInterfaceDb.UpdateOrderNosOfOcOccupiers(caseNumber, "OC") || 
                        resaleInterfaceDb.UpdateOrderNosOfOcOccupiers(caseNumber, "SP") || resaleInterfaceDb.UpdateOrderNosOfOcOccupiers(caseNumber, "SE") || 
                        resaleInterfaceDb.UpdateOrderNosOfOcOccupiers(caseNumber, "PR") || resaleInterfaceDb.UpdateOrderNosOfHaApplicants(caseNumber, "BU"))
                    {
                        result = resulttemp;
                        break;
                    }
                    else
                    {
                        result = resulttemp + 200;
                        WriteToFile(logFilePath, " Failed\r\n");
                        //if (!logFilePath.EndsWith(".Upd"))
                        //{
                        //    File.Move(logFilePath, logFilePath + ".Upd");
                        //    logFilePath = logFilePath + ".Upd";
                        //}
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
                #region Added By Edward 18/02/2014  Sales and Resale Changes
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
        else    //Added By Edward 18/02/2014 to write on log
        {
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "No Personal ");
        }

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
        ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();

        // Delete from AppPersonal table
        appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);  //uncommented by Edward 11/3/2014 Sales and Resale Changes

        // Delete from SrsInterface table
        leasInterfaceDb.Delete(refNo, nric);

        //// Get the applicant type of the personal info
        //string applicantType = ResaleInterfaceDb.GetApplicantType(refNo, nric);

        // Delete the applicant info for the application if marked for deletion
        return resaleInterfaceDb.DeleteByRefNoAndNric(refNo, nric);

        //// Update the Order numbers of the rest of the personal data
        //ResaleInterfaceDb.UpdateOrderNos(refNo, applicantType, (applicantType.ToUpper().Equals("HA")));

        //return true;
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
        if (applicantType.ToUpper().Equals("BU"))
        {
            #region Get all B
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("BU"))
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
                    if (personal.PersonalType.ToUpper().Equals("BU"))
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
            #region Get all O
            int count = 0;

            // Get the O personals
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
        else if (applicantType.ToUpper().Equals("PR"))
        {
            #region Get all PR
            int count = 0;

            // Get the PR personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("PR"))
                    {
                        temp.Add(personal);
                        count++;
                    }
                }
            }
            #endregion
        }
        else if (applicantType.ToUpper().Equals("CH"))
        {
            #region Get all CH
            int count = 0;

            // Get the CH personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("CH"))
                    {
                        temp.Add(personal);
                        count++;
                    }
                }
            }
            #endregion
        }
        else if (applicantType.ToUpper().Equals("SE"))
        {
            #region Get all S
            int count = 0;

            // Get the S personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("SE"))
                    {
                        temp.Add(personal);
                        count++;
                    }
                }
            }
            #endregion
        }
        else if (applicantType.ToUpper().Equals("SP"))
        {
            #region Get all SP
            int count = 0;

            // Get the SP personals
            foreach (Personal personal in personals)
            {
                if (!personal.IsDeletion)
                {
                    if (personal.PersonalType.ToUpper().Equals("SP"))
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
        public string IdType;
        public string MaritalStatus;
        public DateTime DateOfBirth;
        public string Relationship;
        public string Citizenship;
        public string EmploymentStatus;
        public MonthlyIncome[] MonthlyIncomeSet;
        public Document[] Documents;
        public int NumberOfIncomeMonths; //Added By Edward 28.10.2013 
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

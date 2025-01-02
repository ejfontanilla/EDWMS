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
/// Summary description for Leas Household
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Leas : System.Web.Services.WebService
{
    //public AuthenticationSoapHeader soapHeader;
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED = -4;
    private const int INVALID_REFERENCE_NUMBER = -5;
    private const int PERSONAL_WITH_INVALID_NRIC_OR_NAME = -6;

    public Leas()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    #region Log Changes/Fixes
    //******************Log Changes/Fixes*********************
    //Added/Modified By                 Date                Description
    //Edward                            2015/02/04          Added Risk Field in Application
    #endregion

    [WebMethod(EnableSession = true)]
    public int SendHouseholdInfo(AuthenticationClass authentication, string refNumber, DateTime applicationDate, string status, string preEligibilityOic, 
        string creditAccessmentOic, decimal householdIncome, decimal creditAccessmentIncome, DateTime rejectionDate, DateTime cancellationDate, 
        DateTime approvalDate, DateTime expiryDate, DateTime secondCADate, bool secondCA, string risk, bool refreshHouseholdStructure, Personal[] personals)
    {
        int result = 0;

        // Create the text file
        string logFilePath = WriteInfoToTextFile(authentication, refNumber, applicationDate, status, preEligibilityOic, creditAccessmentOic, 
            householdIncome, creditAccessmentIncome, rejectionDate, cancellationDate, approvalDate, expiryDate, secondCADate, secondCA, risk, refreshHouseholdStructure, personals);

        //added by edward 23/01/2014
        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Writing Info \r\n ");

        //Added try catch by Edward 23/01/2014
        int errorCode;
        try
        {
            errorCode = Validate(authentication, refNumber, applicationDate, status, preEligibilityOic, creditAccessmentOic,
            householdIncome, creditAccessmentIncome, rejectionDate, cancellationDate, approvalDate, expiryDate, secondCADate, secondCA, personals, logFilePath);

            //added by edward 23/01/2014
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

                subject = "Error importing from LEAS web service";
                message = "There is an error code " + errorCode + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.AAD).Trim(), string.Empty, string.Empty, subject, message);

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
        string refNo = refNumber;
        string applDate = (applicationDate == null || applicationDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(applicationDate, DateTimeFormat.dd_MM_yyyy));
        string appStatus = EnumManager.MapHleStatus(status);
        string pEOic = preEligibilityOic;
        string cAOic = creditAccessmentOic;
        string hhInc = (householdIncome == null || householdIncome.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : householdIncome.ToString());
        string cAInc = (creditAccessmentIncome == null || creditAccessmentIncome.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : creditAccessmentIncome.ToString());
        string rejDate = (rejectionDate == null || rejectionDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(rejectionDate, DateTimeFormat.dd_MM_yyyy));
        string canDate = (cancellationDate == null || cancellationDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(cancellationDate, DateTimeFormat.dd_MM_yyyy));
        string apprDate = (approvalDate == null || approvalDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(approvalDate, DateTimeFormat.dd_MM_yyyy));
        string expDate = (expiryDate == null || expiryDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(expiryDate, DateTimeFormat.dd_MM_yyyy));
        string CADate = (secondCADate == null || secondCADate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(secondCADate, DateTimeFormat.dd_MM_yyyy));
        string strRisk = risk;

        //Added by Edward 23/01/2014
        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Retrieving Data \r\n ");
        #endregion

        // Insert
        try
        {
            // Insert household structure information
            string refType = Util.GetReferenceType(refNo);

            if (refType.Equals(ReferenceTypeEnum.HLE.ToString()))
                result = SaveHleInterface(refNo, refType, applDate, appStatus, pEOic, cAOic, hhInc, cAInc, rejDate,
                    canDate, apprDate, expDate, CADate, secondCA, risk, refreshHouseholdStructure, personals, logFilePath);
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
        WriteToFile(logFilePath, "\r\nEnd Processing at " + DateTime.Now.ToString() + "\r\n");  //Added by Edward 2015/11/30 to Optimize LEAS Web Service
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

                subject = "Error processing from LEAS web service";
                message = "There is an error code " + result + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.AAD).Trim(), string.Empty, string.Empty, subject, message);

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

    private string WriteInfoToTextFile(AuthenticationClass authentication, string refNumber, DateTime applicationDate, string status, string preEligibilityOic, 
        string creditAccessmentOic, decimal householdIncome, decimal creditAccessmentIncome, 
        DateTime rejectionDate, DateTime cancellationDate, DateTime approvalDate, DateTime expiryDate, DateTime secondCADate, 
        bool secondCA, string risk, bool refreshHouseholdStructure, Personal[] personals)
    {
        // Temporary code to write input to text file
        #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
        //string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("LEAS-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");
        //string filePath = Server.MapPath("~/App_Data/LogFiles/" + DateTime.Now.ToString("LEAS-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        string filePath = Util.GetLogFilesFolder("LEAS", refNumber, string.Empty);
        #endregion

        StreamWriter w;
        w = File.CreateText(filePath);

        w.Write("Start Processing at " + DateTime.Now.ToString() + "\r\n"); //Added by Edward 2015/11/30 to Optimize LEAS Web Service 
        w.Write("UserName: " + authentication.UserName + "\r\n");
        w.Write("Password: " + authentication.Password + "\r\n");
        w.Write("\r\n");

        w.Write("RefNumber: " + refNumber + "\r\n");
        w.Write(applicationDate == null ? "ApplicationDate: " : "ApplicationDate: " + applicationDate.ToString() + "\r\n");
        w.Write("Status: " + status + "\r\n");
        w.Write("PreEligibilityOic: " + preEligibilityOic + "\r\n");
        w.Write("CreditAccessmentOic: " + creditAccessmentOic + "\r\n");
        w.Write(householdIncome == null ? "HouseholdIncome: " : "HouseholdIncome: " + householdIncome.ToString() + "\r\n");
        w.Write(creditAccessmentIncome == null ? "CreditAccessmentIncome: " : "CreditAccessmentIncome: " + creditAccessmentIncome.ToString() + "\r\n");
        w.Write(rejectionDate == null ? "RejectionDate: " : "RejectionDate: " + rejectionDate.ToString() + "\r\n");
        w.Write(cancellationDate == null ? "CancellationDate: " : "CancellationDate: " + cancellationDate.ToString() + "\r\n");
        w.Write(approvalDate == null ? "ApprovalDate: " : "ApprovalDate: " + approvalDate.ToString() + "\r\n");
        w.Write(expiryDate == null ? "ExpiryDate: " : "ExpiryDate: " + expiryDate.ToString() + "\r\n");
        w.Write(secondCADate == null ? "second CA Date: " : "second CA Date: " + secondCADate.ToString() + "\r\n");
        w.Write(secondCA == null ? "second CA: " : "second CA: " + secondCA.ToString() + "\r\n");
        w.Write(risk == null ? "risk: " : "risk : " + risk + "\r\n");
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
                w.Write("MaritalStatus: " + personal.MaritalStatus + "\r\n");
                w.Write(personal.DateOfBirth == null ? "DateOfBirth: " : "DateOfBirth: " + personal.DateOfBirth.ToString("dd MMM yyyy") + "\r\n");
                w.Write("Relationship: " + personal.Relationship + "\r\n");
                w.Write(personal.AnnualIncome == null ? "AnnualIncome: " : "AnnualIncome: " + personal.AnnualIncome.ToString() + "\r\n");
                w.Write(personal.AverageIncome == null ? "AverageIncome: " : "AverageIncome: " + personal.AverageIncome.ToString() + "\r\n");
                w.Write("EmployerName: " + personal.EmployerName + "\r\n");
                w.Write(personal.EmploymentDate == null ? "EmploymentDate: " : "EmploymentDate: " + personal.EmploymentDate.ToString("dd MMM yyyy") + "\r\n");
                w.Write("EmploymentStatus: " + personal.EmploymentStatus + "\r\n");
                w.Write(personal.WithAllowance == null ? "WithAllowance: " : "WithAllowance: " + personal.WithAllowance.ToString() + "\r\n");
                w.Write(personal.WithCpf == null ? "WithCpf: " : "WithCpf: " + personal.WithCpf.ToString() + "\r\n");
                w.Write(personal.Citizenship == null ? "Citizenship: \r\n" : "Citizenship: " + personal.Citizenship.ToString() + "\r\n");
                w.Write(personal.NumberOfIncomeMonths == null ? "Number of Income Months: " : "Number of Income Months: " + personal.NumberOfIncomeMonths.ToString() + "\r\n"); //Added by Edward for Income Extraction/ Assessment
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

    private int Validate(AuthenticationClass authentication, string refNumber, DateTime applicationDate, string status, 
        string preEligibilityOic, string creditAccessmentOic, decimal householdIncome, decimal creditAccessmentIncome,
        DateTime rejectionDate, DateTime cancellationDate, DateTime approvalDate, DateTime expiryDate, DateTime secondCADate, bool secondCA,
        Personal[] personals, string logFilePath)
    {
        
            if (!ValidateLogin(authentication))
                return AUTHENTICATION_ERROR; // -1

            //added by edward 23/01/2014
            //WriteToFile(logFilePath, "\r\n===============================");
            //WriteToFile(logFilePath, "Done Validating Authentication ");

            if (personals != null && personals.Length > 0)
            {
                foreach (Personal personal in personals)
                {
                    if (personal == null)
                        return WRONG_INPUT; // -2

                    //added by edward 23/01/2014
                    //WriteToFile(logFilePath, "\r\n===============================");
                    //WriteToFile(logFilePath, "Done Validating personal ");

                    if (personal.IsDeletion && personal.IsMainApplicant)
                        return DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED; // -4

                    //added by edward 23/01/2014
                    //WriteToFile(logFilePath, "\r\n===============================");
                    //WriteToFile(logFilePath, "Done Validating Main Applicant ");

                    //if (String.IsNullOrEmpty(personal.Nric) ||
                    //    String.IsNullOrEmpty(personal.Name) ||
                    //    (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                    //    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                    if (String.IsNullOrEmpty(personal.Nric))// edit by calvin temp remove for XIN, XIN was able to import before 5 feb || (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                        return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                    //added by edward 23/01/2014
                    //WriteToFile(logFilePath, "\r\n===============================");
                    //WriteToFile(logFilePath, "Done Validating Nric ");

                    if (!string.IsNullOrEmpty(personal.CustomerId)) //Added By Edward 23/01/2014
                    {
                        if (personal.CustomerId.Contains("&#"))
                             return WRONG_INPUT; // -2
                    }

                    //added by edward 23/01/2014
                    //WriteToFile(logFilePath, "\r\n===============================");
                    //WriteToFile(logFilePath, "Done Validating CustomerId ");

                    if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                        return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                    //added by edward 23/01/2014
                    //WriteToFile(logFilePath, "\r\n===============================");
                    //WriteToFile(logFilePath, "Done Validating Deletion and Name ");

                    if (personal.MonthlyIncomeSet != null)
                    {
                        foreach (MonthlyIncome monthlyIncome in personal.MonthlyIncomeSet)
                        {
                            if (monthlyIncome == null)
                                return WRONG_INPUT; // -2
                        }
                        //added by edward 23/01/2014
                        //WriteToFile(logFilePath, "\r\n===============================");
                        //WriteToFile(logFilePath, "Done Validating Monthly Income");
                    }
                    //else
                    //{
                    //    //added by edward 23/01/2014
                    //    //WriteToFile(logFilePath, "\r\n===============================");
                    //    //WriteToFile(logFilePath, "Done Validating Monthly Income 0");
                    //}
                                

                    if (personal.Documents != null)
                    {
                        foreach (Document document in personal.Documents)
                        {
                            if (document == null)
                                return WRONG_INPUT; // -2
                        }
                        //added by edward 23/01/2014
                        //WriteToFile(logFilePath, "\r\n===============================");
                        //WriteToFile(logFilePath, "Done Validating Documents");
                    }
                    //else
                    //{
                    //    //added by edward 23/01/2014
                    //    WriteToFile(logFilePath, "\r\n===============================");
                    //    WriteToFile(logFilePath, "Done Validating Documents 0");
                    //}
                }
            }
            else
            {
                //added by edward 23/01/2014
                WriteToFile(logFilePath, "\r\n===============================");
                WriteToFile(logFilePath, "No Personal to Validate ");
            }

            return 0;
    }

    private int SaveHleInterface(string refNo, string refType, string applDate, string appStatus, string pEOic, string cAOic, string hhInc, string cAInc, 
        string rejDate, string canDate, string apprDate, string expDate, string CADate, bool secondCA, string risk, bool refreshHouseholdStructure,
        Personal[] personals, string logFilePath)
    {
        int result = 0;
        int resulttemp = 0;
        int HACount = 0; int OCCount = 0;

        HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
        DocAppDb docAppDb = new DocAppDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
        DocTypeDb docTypeDb = new DocTypeDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();

        int docAppId = -1;

        #region Insert the HLE number
        try
        {
            docAppId = docAppDb.InsertRefNumber(refNo, refType, cAOic, pEOic, CADate, secondCA, risk, false, string.Empty, string.Empty);

            //Added By Edward 23/01/2014
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Done Insert/Update HLE number\r\n ");
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
        try  //Added By Try Catch By Edward 23/01/2014
        {
            if (hleInterfaceDb.DoesPersonalExistsByRefNo(refNo))
            {
                for (int cnt = 0; cnt < 3; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts updating HLE OIC: " + (cnt + 1).ToString());
                    try
                    {
                        // Update the application
                        if (hleInterfaceDb.UpdateApplication(refNo, applDate, appStatus, pEOic, cAOic, hhInc,
                            cAInc, rejDate, canDate, apprDate, expDate, risk))
                        {
                            result = resulttemp;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Update HLE Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
                //Added By Edward 23/01/2014
                WriteToFile(logFilePath, "\r\n===============================");
                WriteToFile(logFilePath, "Done Update Application ");
            }
            else if (personals == null || personals.Length <= 0)
            {
                WriteToFile(logFilePath, "\r\nAttempts updating HLE OIC: Failed\r\n");
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
            if (refreshHouseholdStructure && hleInterfaceDb.DoesPersonalExistsByRefNo(refNo))
            {
                resulttemp = result;
                for (int cnt = 0; cnt < 2; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts deleting HLE: " + (cnt + 1).ToString());
                    try
                    {
                        if (hleInterfaceDb.DeleteByRefNo(refNo))
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
                        WriteToFile(logFilePath, "Delete HLE Exception: " + e.Message + "\r\n");
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
                #region Insert the HLE household structure data for OC
                OCCount++;

                #region Commented by Edward 18/12/2013 OCCUPIER ISSUE
                HlePersonal hlePersonal = new HlePersonal();
                hlePersonal.AddPersonalInfoFromWebService(refNo, applDate, appStatus, hhInc, cAInc, pEOic, cAOic, rejDate, canDate, apprDate, expDate,
                    ocOrderNo++, risk, personal);
                #endregion

                #region Modified by Edward 18/12/2013 FOR OCCUPIER ISSUE
                //int orderNo = hleInterfaceDb.GetOrderNumberForNric(refNo, personal.PersonalType, personal.Nric);
                //HlePersonal hlePersonal = new HlePersonal();
                //hlePersonal.AddPersonalInfoFromWebService(refNo, applDate, appStatus, hhInc, cAInc, pEOic, cAOic, rejDate, canDate, apprDate, expDate,
                //    orderNo, personal);
                #endregion



                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (hleInterfaceDb.DoesPersonalExistsByRefNoNric(hlePersonal.HleNumber, hlePersonal.Nric) ||
                            //hleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            hleInterfaceDb.DoesPersonalExistsByRefNoName(hlePersonal.HleNumber, hlePersonal.Name))
                            isSaved = hleInterfaceDb.Update(hlePersonal);
                        else
                            isSaved = hleInterfaceDb.Insert(hlePersonal) > 0;

                        if (isSaved && result >= 0)
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
                    WriteToFile(logFilePath, "Insert/Update HLE(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(hlePersonal.HleNumber, hlePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(hlePersonal.HleNumber.Trim(),
                                    //    hlePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(hlePersonal.HleNumber, hlePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    leasInterfaceDb.Insert(hlePersonal.HleNumber, hlePersonal.Nric, docTypeCode, document.Date,
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
            Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");

            foreach (Personal personal in sortedHaPersonal)
            {
                #region Insert the HLE household structure data for HA
                HACount++;
                int orderNo = hleInterfaceDb.GetOrderNumberForNric(refNo, personal.PersonalType, personal.Nric);

                HlePersonal hlePersonal = new HlePersonal();
                hlePersonal.AddPersonalInfoFromWebService(refNo, applDate, appStatus, hhInc, cAInc, pEOic, cAOic, rejDate, canDate, apprDate, expDate,
                    orderNo, risk, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (hleInterfaceDb.DoesPersonalExistsByRefNoNric(hlePersonal.HleNumber, hlePersonal.Nric) ||
                            //(hlePersonal.CustomerId.Length > 0 && hleInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId)) ||
                            hleInterfaceDb.DoesPersonalExistsByRefNoName(hlePersonal.HleNumber, hlePersonal.Name))
                            isSaved = hleInterfaceDb.Update(hlePersonal);
                        else
                            isSaved = hleInterfaceDb.Insert(hlePersonal) > 0;

                        if (isSaved && result >= 0)
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
                    WriteToFile(logFilePath, "Insert/Update HLE(HA) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(hlePersonal.HleNumber, hlePersonal.Nric);
                        foreach (Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(hlePersonal.HleNumber.Trim(),
                                    //    hlePersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(hlePersonal.HleNumber, hlePersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
                                    leasInterfaceDb.Insert(hlePersonal.HleNumber, hlePersonal.Nric, docTypeCode, document.Date,
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
                        //DocAppDb docAppDb = new DocAppDb();
                        docAppDb.UpdatePendingDoc(hlePersonal.HleNumber);
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

            WriteToFile(logFilePath, "\r\n===============================");

            WriteToFile(logFilePath, "Records HA/OC: " + HACount.ToString() + " / " + OCCount.ToString());
            WriteToFile(logFilePath, "Records inserted/updated: " + inserted.ToString());

            #region Delete all personal marked for deletion
            Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Personal personal in toBeDeletedPersonal)
            {
                if (hleInterfaceDb.IsMainApplicant(refNo, personal.Nric))
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
                        //if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        //{
                        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNo, personal.Nric);

                        if (isSaved)
                            result = (++deleted);
                        //}
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Delete HLE Exception: " + e.Message);
                        result = PROCESSING_ERROR;
                    }
                    #endregion
                }
            }
            #endregion

            // Record the number of transactions
            //WriteToFile(logFilePath, "\r\n===============================");

            //WriteToFile(logFilePath, "Records HA/OC: " + HACount.ToString() + " / " + OCCount.ToString());
            //WriteToFile(logFilePath, "Records inserted/updated: " + inserted.ToString());
            WriteToFile(logFilePath, "Records deleted: " + deleted.ToString());

            #region Sort by NRIC
            // Update the Order Nos
            resulttemp = result;
            for (int cnt = 0; cnt < 3; cnt++)
            {
                try
                {
                    WriteToFile(logFilePath, "\r\nAttempts update Orders: " + (cnt + 1).ToString());
                    if (HACount > 0)
                    {
                        if (hleInterfaceDb.UpdateOrderNosOfHaApplicants(refNo))
                        {
                            result = resulttemp;
                            //break;        Commented By Edward 19/12/2013 FOR UAT Issue to Cater the Occupier Sorting by NRIC
                        }
                        else
                        {
                            result = resulttemp + 200;
                            WriteToFile(logFilePath, " Failed\r\n");
                        }
                    }
                    if (OCCount > 0)
                    {
                        if (hleInterfaceDb.UpdateOrderNosOfOcOccupiers(refNo))
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

            #region Add to AppPersonal and add Income Months
            // Refresh the household structure
            try
            {
                appPersonalDb.SavePersonalRecords(docAppId);
                #region Added By Edward 22/01/2014  Add or Update InsertMonthYearIncomeTable when Use LEas service

                #region Added by Edward 2015/11/30 to Optimize LEAS Web Service

                //DocApp.DocAppDataTable docAppdt = docAppDb.GetDocAppById(docAppId);
                //if (docAppdt.Rows.Count > 0)
                //{
                //    DocApp.DocAppRow docAppRow = docAppdt[0];
                //    if (!docAppRow.IsAssessmentStatusNull())
                //        IncomeDb.InsertMonthYearInIncomeTable(docAppId, IncomeMonthsSourceEnum.W);
                //        //IncomeDb.InsertMonthYearInIncomeTableFromLeas(docAppId); Commented By Edward 29/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
                //}

                string AssessmentStatus = docAppDb.GetAssessmentStatusByDocAppId(docAppId);
                if(!string.IsNullOrEmpty(AssessmentStatus))
                    IncomeDb.InsertMonthYearInIncomeTable(docAppId, IncomeMonthsSourceEnum.W);

                #endregion

                #endregion
            }
            catch (Exception e)
            {
                WriteToFile(logFilePath, "===============================");
                WriteToFile(logFilePath, "Update Personal Exception: " + e.Message);
                result = PROCESSING_ERROR;
            }
            #endregion
        }
        else    //Added By Edward 23/01/2014 to write on log
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
        HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();

        // Delete from AppPersonal table
        //appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);
        //Uncommented by Edward 17/12/2013
        appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);

        // Delete from LeasInterface table
        leasInterfaceDb.Delete(refNo, nric);

        //// Get the applicant type of the personal info
        //string applicantType = hleInterfaceDb.GetApplicantType(refNo, nric);

        // Delete the applicant info for the application if marked for deletion
        return hleInterfaceDb.DeleteByRefNoAndNric(refNo, nric);

        //// Update the Order numbers of the rest of the personal data
        //hleInterfaceDb.UpdateOrderNos(refNo, applicantType, (applicantType.ToUpper().Equals("HA")));

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

        // Sort the HA personals
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
        public string MaritalStatus;
        public DateTime DateOfBirth;
        public string Relationship;
        public decimal AnnualIncome;
        public decimal AverageIncome;
        public string EmployerName;
        public DateTime EmploymentDate;
        public string EmploymentStatus;
        public bool WithAllowance;
        public bool WithCpf;
        public string Citizenship;
        public MonthlyIncome[] MonthlyIncomeSet;
        public Document[] Documents;
        public int NumberOfIncomeMonths;        //Added by Edward 19.08.2013 for Income Extraction/Assessment
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

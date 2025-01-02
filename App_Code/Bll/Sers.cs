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
/// Summary description for Sers Household
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Sers : System.Web.Services.WebService
{
    //public AuthenticationSoapHeader soapHeader;
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED = -4;
    private const int INVALID_REFERENCE_NUMBER = -5;
    private const int PERSONAL_WITH_INVALID_NRIC_OR_NAME = -6;

    public Sers()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public int SersHouseholdInfo(AuthenticationClass authentication, string refNumber, string registrationNumber, string stage, string block, string street, 
        string level, string unitNumber, string postalCode, string allocationScheme, string eligibilityScheme, 
        string OIC1, string OIC2, DateTime surrenderDate, bool refreshHouseholdStructure, Personal[] personals)
    {
        int result = 0;

        #region Creating the TextFile
        // Create the text file
        string logFilePath = WriteInfoToTextFile(authentication, refNumber, registrationNumber, stage, block, street,
            level, unitNumber, postalCode, allocationScheme, eligibilityScheme, OIC1, OIC2, surrenderDate, refreshHouseholdStructure, personals);

        WriteToFile(logFilePath, "\r\n===============================");
        WriteToFile(logFilePath, "Done Writing Info \r\n ");
        #endregion

        #region Validating...
        int errorCode;

        try
        {
            errorCode = Validate(authentication, refNumber, registrationNumber, stage, block, street,
               level, unitNumber, postalCode, allocationScheme, eligibilityScheme, surrenderDate, personals);

            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Done Validating: " + errorCode.ToString());
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, "\r\n===============================");
            WriteToFile(logFilePath, "Error while Validating: " + e.Message.ToString());
            errorCode = -10;
        }
        #endregion

        #region If Validation has error
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

                subject = "Error importing from Srs web service";
                message = "There is an error code " + errorCode + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.PRD).Trim(), string.Empty, string.Empty, subject, message);

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
        #endregion

        #region Retrieve data about the reference number
        string strRefNo = refNumber;
        string strRegistrationNumber = registrationNumber;
        string strStage = stage;
        string strBlock = block;
        string strStreet = street;
        string strLevel = level;
        string strUnitNumber = unitNumber;
        string strPostalCode = postalCode;
        string strAllocationScheme = allocationScheme;
        string strEligibilityScheme = eligibilityScheme;
        string strOIC1 = OIC1;
        string strOIC2 = OIC2;
        string strSurrenderDate = (surrenderDate == null || surrenderDate.ToString().Equals(Constants.WebServiceNullDate) ? string.Empty : Format.FormatDateTime(surrenderDate, DateTimeFormat.dd_MM_yyyy));
        #endregion

        #region Inserting in the Interface
        // Insert
        try
        {
            // Insert household structure information
            string refNo = refNumber;
            string refType = Util.GetReferenceType(refNo);
            if (refType.Equals(ReferenceTypeEnum.SERS.ToString()))
            {
                result = SaveSersInterface(refNumber, refType, registrationNumber, stage, block, street, level,
                    unitNumber, postalCode, allocationScheme, eligibilityScheme, OIC1, OIC2, strSurrenderDate, refreshHouseholdStructure, personals, logFilePath);
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

        #endregion
                
        #region Check result and Email when there is error in inserting
        if (result > 100)
            File.Move(logFilePath, logFilePath + ".Upd");

        if (result < 0)
        {
            try
            {
                File.Move(logFilePath, logFilePath + ".Err");

                string subject = string.Empty;
                string message = string.Empty;

                subject = "Error processing from Sers web service";
                message = "There is an error code " + result + " for reference no. " + refNumber;

                ParameterDb parameterDb = new ParameterDb();
                DepartmentDb departmentDb = new DepartmentDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    departmentDb.GetDepartmentMailingList(DepartmentCodeEnum.PRD).Trim(), string.Empty, string.Empty, subject, message);

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
        #endregion

        return result;
    }

    private string WriteInfoToTextFile(AuthenticationClass authentication, string refNumber, string registrationNumber, string stage,
        string block, string street, string level, string unitNumber, string postalCode, string allocationScheme,
        string eligibilityScheme, string OIC1, string OIC2, DateTime surrenderDate, bool refreshHouseholdStructure, Personal[] personals)
    {
        // Temporary code to write input to text file
        //string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("SERS-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");
        //string filePath = Server.MapPath("~/App_Data/LogFiles/" + DateTime.Now.ToString("SERS-yyyyMMdd-HHmmss.fff") + "-" + refNumber + ".txt");

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        string filePath = Util.GetLogFilesFolder("SERS", refNumber, string.Empty);
        #endregion

        StreamWriter w;
        w = File.CreateText(filePath);

        w.Write("UserName: " + authentication.UserName + "\r\n");
        w.Write("Password: " + authentication.Password + "\r\n");
        w.Write("\r\n");

        w.Write("RefNumber: " + refNumber + "\r\n");
        w.Write("Registration Number: " + registrationNumber + "\r\n");
        w.Write("Stage: " + stage + "\r\n");
        w.Write("Block: " + block + "\r\n");
        w.Write("Street: " + street + "\r\n");
        w.Write("Level: " + level + "\r\n");
        w.Write("Unit Number: " + unitNumber + "\r\n");
        w.Write("Postal Code: " + postalCode + "\r\n");
        w.Write("Allocation Scheme: " + allocationScheme + "\r\n");
        w.Write("Eligibility Scheme: " + eligibilityScheme + "\r\n");
        w.Write("OIC1: " + OIC1 + "\r\n");
        w.Write("OIC2: " + OIC2 + "\r\n");
        w.Write("Surrender Date: " + surrenderDate + "\r\n");
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
                w.Write(personal.Citizenship == null ? "Citizenship: " : "Citizenship: " + personal.Citizenship.ToString() + "\r\n");
                w.Write("PrivatePropertyOwner: " + personal.PrivatePropertyOwner.ToString() + "\r\n");
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

    private int Validate(AuthenticationClass authentication, string refNumber, string registrationNumber, string stage,
        string block, string street, string level, string unitNumber, string postalCode, string allocationScheme,
        string eligibilityScheme, DateTime surrenderDate, Personal[] personals)
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
                
                if (!string.IsNullOrEmpty(personal.CustomerId))
                {
                    if (personal.CustomerId.Contains("&#"))
                        return WRONG_INPUT; // -2
                }

                if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

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

    private int SaveSersInterface(string refNo, string refType, string registrationNumber, string stage, string block, string street, string level,
        string unitNumber, string postalCode, string allocationScheme, string eligibilityScheme, string OIC1, string OIC2,
        string surrenderDate, bool refreshHouseholdStructure, Personal[] personals, string logFilePath)
    {
        int result = 0;
        int resulttemp = 0;

        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
        DocAppDb docAppDb = new DocAppDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
        DocTypeDb docTypeDb = new DocTypeDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();

        int docAppId = -1;

        #region Insert the Sers number
        try
        {
            docAppId = docAppDb.InsertRefNumber(refNo, refType, OIC2, OIC1, null, false, string.Empty, false, string.Empty, string.Empty);
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
            if (sersInterfaceDb.DoesPersonalExistsByRefNo(refNo))
            {
                for (int cnt = 0; cnt < 3; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts updating Sers: " + (cnt + 1).ToString());
                    try
                    {
                        //Update the application
                        if (sersInterfaceDb.UpdateApplication(stage, block, street, level, unitNumber, postalCode, allocationScheme, 
                            eligibilityScheme, registrationNumber, refNo, OIC1, OIC2, surrenderDate))
                        {
                            result = resulttemp;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        WriteToFile(logFilePath, "===============================");
                        WriteToFile(logFilePath, "Update Sers Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }
            else if (personals == null || personals.Length <= 0)
            {
                WriteToFile(logFilePath, "\r\nAttempts updating Sers OIC: Failed\r\n");
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
            if (refreshHouseholdStructure && sersInterfaceDb.DoesPersonalExistsByRefNo(refNo))
            {
                resulttemp = result;
                for (int cnt = 0; cnt < 2; cnt++)
                {
                    WriteToFile(logFilePath, "\r\nAttempts deleting Sers: " + (cnt + 1).ToString());
                    try
                    {
                        if (sersInterfaceDb.DeleteByRefNo(refNo))
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
                        WriteToFile(logFilePath, "Delete Sers Exception: " + e.Message + "\r\n");
                        result = PROCESSING_ERROR;
                    }
                    Thread.Sleep(1000);
                }
            }

            int inserted = 0;
            int deleted = 0;

            #region Insert the OC personal info
            Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Personal personal in sortedOcPersonal)
            {
                #region Insert the Sers household structure data for OC
                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromWebService(refNo, stage, block, street, level, unitNumber, postalCode, allocationScheme, 
                    eligibilityScheme, ocOrderNo++, OIC1, OIC2, surrenderDate, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(sersPersonal.SchAcc, sersPersonal.Nric) ||
                            //sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId) ||
                            sersInterfaceDb.DoesPersonalExistsBySchAccName(sersPersonal.SchAcc, sersPersonal.Name))
                            isSaved = sersInterfaceDb.Update(sersPersonal);
                        else
                            isSaved = sersInterfaceDb.Insert(sersPersonal) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
                    //else
                    //{
                    //    // Check if the personal has a record in AppPersonal table with documents tied to it
                    //    if (!CheckIfPersonalHasDocuments(docAppId, sersPersonal.Nric))
                    //    {
                    //        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, sersPersonal.SchAcc, sersPersonal.Nric);

                    //        //if (isSaved)
                    //        //    result = (++successCnt);
                    //    }
                    //}
                }
                catch (Exception e)
                {
                    WriteToFile(logFilePath, "===============================");
                    WriteToFile(logFilePath, "Insert/Update Sers(OC) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(sersPersonal.SchAcc, sersPersonal.Nric);
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
                                    leasInterfaceDb.Insert(sersPersonal.SchAcc, sersPersonal.Nric, docTypeCode, document.Date,
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
                #region Insert the Sers household structure data for HA
                int orderNo = sersInterfaceDb.GetOrderNumberForNric(refNo, personal.PersonalType, personal.Nric);

                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromWebService(refNo, stage, block, street, level, unitNumber, postalCode, allocationScheme, 
                    eligibilityScheme, orderNo, OIC1, OIC2,surrenderDate, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(sersPersonal.SchAcc, sersPersonal.Nric) ||
                            //(hlePersonal.CustomerId.Length > 0 && sersInterfaceDb.DoesPersonalExistsByRefNoCustomerId(hlePersonal.HleNumber, hlePersonal.CustomerId)) ||
                            sersInterfaceDb.DoesPersonalExistsBySchAccName(sersPersonal.SchAcc, sersPersonal.Name))
                            isSaved = sersInterfaceDb.Update(sersPersonal);
                        else
                            isSaved = sersInterfaceDb.Insert(sersPersonal) > 0;

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
                    WriteToFile(logFilePath, "Insert/Update Sers(HA) Exception: " + e.Message);
                    result = PROCESSING_ERROR;
                }
                #endregion

                #region Insert the Pending Documents
                try
                {
                    if (personal.Documents != null && isSaved)
                    {
                        leasInterfaceDb.Delete(sersPersonal.SchAcc, sersPersonal.Nric);
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
                                    leasInterfaceDb.Insert(sersPersonal.SchAcc, sersPersonal.Nric, docTypeCode, document.Date,
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
                if (sersInterfaceDb.IsMainApplicant(refNo, personal.Nric))
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
                        WriteToFile(logFilePath, "Delete Sers Exception: " + e.Message);
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

            // Update the Order Nos
            resulttemp = result;
            for (int cnt = 0; cnt < 3; cnt++)
            {
                try
                {
                    WriteToFile(logFilePath, "\r\nAttempts update Orders: " + (cnt + 1).ToString());
                    if (sersInterfaceDb.UpdateOrderNosOfOcOccupiers(refNo) || sersInterfaceDb.UpdateOrderNosOfHaApplicants(refNo))
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

            // Refresh the household structure
            try
            {
                appPersonalDb.SavePersonalRecords(docAppId);
            }
            catch (Exception e)
            {
                WriteToFile(logFilePath, "===============================");
                WriteToFile(logFilePath, "Update Personal Exception: " + e.Message);
                result = PROCESSING_ERROR;
            }
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
        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();

        // Delete from AppPersonal table
        //appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);

        // Delete from SrsInterface table
        leasInterfaceDb.Delete(refNo, nric);

        //// Get the applicant type of the personal info
        //string applicantType = sersInterfaceDb.GetApplicantType(refNo, nric);

        // Delete the applicant info for the application if marked for deletion
        return sersInterfaceDb.DeleteByRefNoAndNric(refNo, nric);

        //// Update the Order numbers of the rest of the personal data
        //sersInterfaceDb.UpdateOrderNos(refNo, applicantType, (applicantType.ToUpper().Equals("HA")));

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
        public string Citizenship;
        public bool PrivatePropertyOwner;
        public Document[] Documents;
        //public int NumberOfIncomeMonths; //Added By Edward 28.10.2013
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

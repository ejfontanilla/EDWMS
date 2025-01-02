using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Threading;


#region Log Changes/Fixes
//******************Log Changes/Fixes*********************
//Added/Modified By                 Date                Description
//Edward                            2015/02/04          Added Risk Field in Application
//Edward                            2015/02/04          Added ABCDERefNumber Field in Application
#endregion


public partial class Maintenance_BatchUpload_Default : System.Web.UI.Page
{
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED = -4;
    private const int INVALID_REFERENCE_NUMBER = -5;
    private const int PERSONAL_WITH_INVALID_NRIC_OR_NAME = -6;

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
            TreeviewDWMS.PopulateTreeViewXML(RadTreeView1);
        //RadTreeView1.ExpandAllNodes();
    }

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        
    }

    protected void RadTreeView1CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (RadTreeView1.CheckedNodes.Count > 0);
    }

    protected void RadTreeView1_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        //FileSizeLabel.Text = "120MB";
    }

    protected void Save(object sender, EventArgs e)
    {
        
        Page.Validate();        
        if (Page.IsValid)
        {
            WaitPanel.Visible = true;
            IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;
            string strLEAS = @"LEAS_FTP/";
            string strRESALE = @"RESL_FTP/";
            string strSALES = @"SOC_FTP/";
            string strSERS = @"SRS_FTP/";
            string division = string.Empty;
            string strHomeFolder = @"household_ftp/localuser/";

            string filePath = Server.MapPath("~/App_Data/WebService/" + strHomeFolder);
            CreateImportedFolder(filePath);

            #region  Added By Edward 2014/05/05 for addressing the Thread
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string systemInfo = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            Guid userId;
            MembershipUser user = Membership.GetUser();
            userId = (Guid)user.ProviderUserKey;
            string recipientEmail = user.Email;

            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection(userId);

            #endregion

            foreach (RadTreeNode rNode in checkedNodes)
            {
                if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
                {
                    FileInfo file;
                    if (rNode.FullPath.Substring(0, strLEAS.Length - 1).ToUpper().Equals("LEAS_FTP"))                    
                        division = strLEAS;
                    else if (rNode.FullPath.Substring(0, strRESALE.Length - 1).ToUpper().Equals("RESL_FTP"))                    
                        division = strRESALE;
                    else if (rNode.FullPath.Substring(0, strSALES.Length - 1).ToUpper().Equals("SOC_FTP"))                    
                        division = strSALES;
                    else if (rNode.FullPath.Substring(0, strSERS.Length - 1).ToUpper().Equals("SRS_FTP"))                    
                        division = strSERS;
                    
                    file = new FileInfo(filePath + division + rNode.Text);                    
                    //ReadXML(file, division, filePath);
                    ThreadStart starter = delegate { ReadXML(file, division, filePath, ip, systemInfo, recipientEmail, sectionId, user.UserName); };
                    //ThreadStart starter = delegate { ReadXML(file, division, filePath, ip, systemInfo, recipientEmail); };
                    Thread thread = new Thread(starter);
                    thread.Start();
                    
                }
            }
            TreeviewDWMS.PopulateTreeViewXML(RadTreeView1);

        }
    }

    private void CopyAndDeleteFile(string path, string division, FileInfo file)
    {
        string strImported = @"Imported/";
        file.CopyTo(path + division + strImported + file.Name, true);
        file.Delete();
    }


    private void CreateImportedFolder(string path)
    {
        string strLEAS = path + @"LEAS_FTP/Imported/";
        string strRESALE = path + @"RESL_FTP/Imported/";
        string strSALES = path + @"SOC_FTP/Imported/";
        string strSERS = path + @"SRS_FTP/Imported/";
        DirectoryInfo info = new DirectoryInfo(path);

        if (!Directory.Exists(strLEAS))
            Directory.CreateDirectory(strLEAS);
        if (!Directory.Exists(strRESALE))
            Directory.CreateDirectory(strRESALE);
        if (!Directory.Exists(strSALES))
            Directory.CreateDirectory(strSALES);
        if (!Directory.Exists(strSERS))
            Directory.CreateDirectory(strSERS);
    }

    private void ReadXML(FileInfo file, string division, string filePath, string ip, string systemInfo, string recipientEmail,
        int sectionId, string oic)
    {
        int NoOfCases = 0;
        int TotalNoOfFailed = 0;
        int NoOfFailed = 0;
        int BUId = 0;

        if (file.Exists)
        {
            string filePathOfLogFile = filePath + division + file.Name + DateTime.Now.ToString("-yyyyMMdd-HHmmss.fff") + ".txt";
            
            BUId = BatchUploadDb.InsertBUHeader(sectionId, file.Name, oic, 0, 0);

            XDocument doc = XDocument.Load(file.FullName);
            XElement elementApplication = new XElement("new");
            if (division.Contains("LEAS_FTP"))
            {                
                foreach (XElement element in doc.Element("ROOT").Elements())
                {
                    if (element.Name == "Application")
                    {                        
                        InsertLEAS(element, filePathOfLogFile, ip, systemInfo, recipientEmail, BUId, out NoOfFailed);                        
                        NoOfCases++;
                        TotalNoOfFailed = TotalNoOfFailed + NoOfFailed;
                    }
                }                
            }
            else if (division.Contains("RESL_FTP"))
            {                
                foreach (XElement element in doc.Element("ROOT").Elements())
                {
                    if (element.Name == "Application")
                    {
                        InsertRESALE(element, filePathOfLogFile, ip, systemInfo, recipientEmail, BUId, out NoOfFailed);
                        NoOfCases++;
                        TotalNoOfFailed = TotalNoOfFailed + NoOfFailed;
                    }
                }
            }
            else if (division.Contains("SOC_FTP"))
            {
                foreach (XElement element in doc.Element("ROOT").Elements())
                {
                    if (element.Name == "Application")
                    {
                        InsertSALES(element, filePathOfLogFile, ip, systemInfo, recipientEmail, BUId, out NoOfFailed);
                        NoOfCases++;
                        TotalNoOfFailed = TotalNoOfFailed + NoOfFailed;
                    }
                }
            }
            else if (division.Contains("SRS_FTP"))
            {
                foreach (XElement element in doc.Element("ROOT").Elements())
                {
                    if (element.Name == "Application")
                    {
                        InsertSERS(element, filePathOfLogFile, ip, systemInfo, recipientEmail, BUId, out NoOfFailed);
                        NoOfCases++;
                        TotalNoOfFailed = TotalNoOfFailed + NoOfFailed;
                    }
                }
            }
            BatchUploadDb.UpdateBUHeaderNoOfCases(BUId, NoOfCases, TotalNoOfFailed);

            SendEmailAfterUploading(recipientEmail, file.Name, filePathOfLogFile);                
            CopyAndDeleteFile(filePath, division, file);
        }
    }

    private  string WriteInfoToTextFile(string refNumber, string filePathOfLogFile)
    {
        // Temporary code to write input to text file        

        StreamWriter w;
        if (!File.Exists(filePathOfLogFile))
            w = File.CreateText(filePathOfLogFile);
        else
            w = File.AppendText(filePathOfLogFile);

        w.Write("\r\n");
        w.Write("Date/Time Started: " + DateTime.Now.ToString() + "\r\n");

        w.Write("RefNumber: " + refNumber + "\r\n");

        w.Flush();
        w.Close();

        return filePathOfLogFile;
    }

    private  void WriteToFile(string logFilePath, string contents)
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

    private  void WriteToFileWhenError(string logFilePath, string contents, int source)
    {
        WriteToFile(logFilePath, "\r\n===============================");
        if (source == 1) //Validate
            WriteToFile(logFilePath, "Validation Check (Error Code): " + contents.ToString());
        else if(source == 2) //Save
            WriteToFile(logFilePath, "Error in Saving (Error Code): " + contents.ToString());  
        else
            WriteToFile(logFilePath, "Error in Reading XML (Error Code): " + contents.ToString());  
        WriteToFile(logFilePath, "===============================");
    }

    #region LEAS

    private  void InsertLEAS(XElement element, string filePathOfLogFile, string ip, string systemInfo, string recipientEmail, 
        int BUId, out int NoOfFailed)
    {
        NoOfFailed = 0;
        string refNo = string.Empty;
        try
        {
            refNo = element.Element("RefNumber").Value;
            string applDate = Format.CheckDateTime(element.Element("ApplicationDate").Value);
            string appStatus = element.Element("Status").Value;
            string pEOic = element.Element("PreEligibilityOic").Value;
            string cAOic = element.Element("CreditAccessmentOic").Value;
            string hhInc = element.Element("HouseholdIncome").Value;
            string cAInc = element.Element("CreditAccessmentIncome").Value;
            string rejDate = Format.CheckDateTime(element.Element("RejectionDate").Value);
            string canDate = Format.CheckDateTime(element.Element("CancellationDate").Value);
            string apprDate = Format.CheckDateTime(element.Element("ApprovalDate").Value);
            string expDate = Format.CheckDateTime(element.Element("ExpiryDate").Value);
            string CADate = Format.CheckDateTime(element.Element("SecondCADate").Value);
            bool secondCA = Format.CheckBoolean(element.Element("SecondCA").Value);
            string risk = element.Element("Risk").Value;    //Added By Edward 2015/02/06
            bool refreshHouseholdStructure = Format.CheckBoolean(element.Element("RefreshHouseholdStructure").Value);
            List<Leas.Personal> listPersonal = new List<Leas.Personal>();
            foreach (XElement xePersonal in element.Elements())
            {

                if (xePersonal.Name == "Personal")
                {
                    Leas.Personal personal = new Leas.Personal();
                    personal.IsDeletion = Format.CheckBoolean(xePersonal.Element("IsDeletion").Value);
                    personal.PersonalType = xePersonal.Element("PersonalType").Value;
                    personal.CustomerId = xePersonal.Element("CustomerId").Value;
                    personal.IsMainApplicant = Format.CheckBoolean(xePersonal.Element("IsMainApplicant").Value);
                    personal.Nric = xePersonal.Element("Nric").Value;
                    personal.Name = xePersonal.Element("Name").Value;
                    personal.MaritalStatus = xePersonal.Element("MaritalStatus").Value;
                    personal.DateOfBirth = DateTime.Parse(Format.CheckDateTime(xePersonal.Element("DateOfBirth").Value));
                    personal.Relationship = xePersonal.Element("Relationship").Value;
                    personal.AnnualIncome = decimal.Parse(xePersonal.Element("AnnualIncome").Value);
                    personal.EmployerName = xePersonal.Element("EmployerName").Value;
                    personal.EmploymentDate = DateTime.Parse(Format.CheckDateTime(xePersonal.Element("EmploymentDate").Value));
                    personal.EmploymentStatus = xePersonal.Element("EmploymentStatus").Value;
                    personal.WithAllowance = Format.CheckBoolean(xePersonal.Element("WithAllowance").Value);
                    personal.WithCpf = Format.CheckBoolean(xePersonal.Element("WithCpf").Value);
                    personal.Citizenship = xePersonal.Element("Citizenship").Value;
                    personal.NumberOfIncomeMonths = int.Parse(xePersonal.Element("NumberOfIncomeMonths").Value);

                    #region MonthlyIncome Elements
                    List<Leas.MonthlyIncome> listMonthlyIncome = new List<Leas.MonthlyIncome>();
                    foreach (XElement xeMonthlyIncome in xePersonal.Elements())
                    {
                        if (xeMonthlyIncome.Name == "MonthlyIncome")
                        {
                            string strYrMnth = string.Empty;
                            foreach (XElement xeMonthYear in xeMonthlyIncome.Descendants())
                            {
                                if (xeMonthYear.Name == "YearMonth")
                                    strYrMnth = xeMonthYear.Value;
                                else if (xeMonthYear.Name == "Amount")
                                {
                                    Leas.MonthlyIncome monthlyIncome = new Leas.MonthlyIncome();
                                    monthlyIncome.YearMonth = strYrMnth;
                                    monthlyIncome.Amount = decimal.Parse(xeMonthYear.Value);
                                    listMonthlyIncome.Add(monthlyIncome);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Document Elements
                    List<Leas.Document> listDocument = new List<Leas.Document>();
                    foreach (XElement xeDocument in xePersonal.Elements())
                    {
                        if (xeDocument.Name == "Document")
                        {
                            string strDocumentTypeCode, strDocumentTypeSubCode, strCategory, strCategoryGroup, strCategoryDate, strDate;
                            strDocumentTypeCode = strDocumentTypeSubCode = strCategory = strCategoryGroup = strCategoryDate = strDate = string.Empty;
                            foreach (XElement xeElement in xeDocument.Descendants())
                            {
                                if (xeElement.Name == "DocumentTypeCode")
                                    strDocumentTypeCode = xeElement.Value;
                                else if (xeElement.Name == "DocumentTypeSubCode")
                                    strDocumentTypeSubCode = xeElement.Value;
                                else if (xeElement.Name == "Category")
                                    strCategory = xeElement.Value;
                                else if (xeElement.Name == "CategoryGroup")
                                    strCategoryGroup = xeElement.Value;
                                else if (xeElement.Name == "CategoryDate")
                                    strCategoryDate = xeElement.Value;
                                else if (xeElement.Name == "Date")
                                    strDate = xeElement.Value;
                                else if (xeElement.Name == "Status")
                                {
                                    Leas.Document document = new Leas.Document();
                                    document.DocumentTypeCode = strDocumentTypeCode;
                                    document.DocumentTypeSubCode = strDocumentTypeSubCode;
                                    document.Category = strCategory;
                                    document.CategoryGroup = strCategoryGroup;
                                    document.CategoryDate = strCategoryDate;
                                    document.Date = strDate;
                                    document.Status = xeDocument.Element("Status").Value;
                                    listDocument.Add(document);
                                }
                            }
                        }
                    }
                    #endregion

                    personal.MonthlyIncomeSet = listMonthlyIncome.ToArray();
                    personal.Documents = listDocument.ToArray();
                    listPersonal.Add(personal);
                }
            }

            string logFilePath = WriteInfoToTextFile(refNo, filePathOfLogFile);

            int errorCode = Validate(refNo, DateTime.Parse(applDate), appStatus, pEOic, cAOic,
                Decimal.Parse(hhInc), Decimal.Parse(cAInc), DateTime.Parse(rejDate), DateTime.Parse(canDate), DateTime.Parse(apprDate),
                DateTime.Parse(expDate), DateTime.Parse(CADate), secondCA, listPersonal.ToArray());

            if (errorCode < 0)
            {
                NoOfFailed++;
                WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 1);
                BatchUploadDb.InsertBUDetails(BUId, refNo, false, GetErrorMessage(errorCode));
            }
            else
            {
                string refType = Util.GetReferenceType(refNo);

                if (refType.Equals(ReferenceTypeEnum.HLE.ToString()))
                {
                    errorCode = SaveHleInterface(refNo, refType, applDate, appStatus, pEOic, cAOic, hhInc, cAInc, rejDate, canDate, apprDate, expDate,
                         CADate, secondCA, risk, refreshHouseholdStructure, listPersonal.ToArray(), filePathOfLogFile, ip, systemInfo);

                    if (errorCode < 0)
                    {
                        NoOfFailed++;
                        WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 2);
                        BatchUploadDb.InsertBUDetails(BUId, refNo, false, GetErrorMessage(errorCode));
                    }
                    else                    
                        BatchUploadDb.InsertBUDetails(BUId, refNo, true, string.Empty);                    
                }
                else
                {
                    NoOfFailed++;
                    WriteToFileWhenError(filePathOfLogFile, "Reference number is not valid for HLE", 2);
                    BatchUploadDb.InsertBUDetails(BUId, refNo, false, "Reference number is not valid for HLE");
                }
            }

        }
        catch (Exception ex)
        {
            NoOfFailed++;
            WriteToFileWhenError(filePathOfLogFile, ex.Message, 3);
            BatchUploadDb.InsertBUDetails(BUId, refNo, false, ex.Message);
        }
        
    }

    private int SaveHleInterface(string refNo, string refType, string applDate, string appStatus, string pEOic, string cAOic, string hhInc, string cAInc,
      string rejDate, string canDate, string apprDate, string expDate, string CADate, bool secondCA, string risk, bool refreshHouseholdStructure,
      Leas.Personal[] personals, string logFilePath, string ip, string systemInfo)
    
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
            docAppId = docAppDb.InsertRefNumber(refNo, refType, cAOic, pEOic, CADate, secondCA, risk, true, ip , systemInfo);
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
        if (hleInterfaceDb.DoesPersonalExistsByRefNo(refNo))
        {
            for (int cnt = 0; cnt < 3; cnt++)
            {
                WriteToFile(logFilePath, "\r\nAttempts updating HLE OIC: " + (cnt + 1).ToString());
                try
                {
                    // Update the application
                    if (hleInterfaceDb.UpdateApplication(refNo, applDate, appStatus, pEOic, cAOic, hhInc,
                        cAInc, rejDate, canDate, apprDate, expDate,risk))
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
        }
        else if (personals == null || personals.Length <= 0)
        {
            WriteToFile(logFilePath, "\r\nAttempts updating HLE OIC: Failed\r\n");
            result = INVALID_REFERENCE_NUMBER;//Ref no not found for updating
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
            Leas.Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Leas.Personal personal in sortedOcPersonal)
            {
                #region Insert the HLE household structure data for OC
                OCCount++;
                HlePersonal hlePersonal = new HlePersonal();
                hlePersonal.AddPersonalInfoFromWebService(refNo, applDate, appStatus, hhInc, cAInc, pEOic, cAOic, rejDate, canDate, apprDate, expDate,
                    ocOrderNo++, risk, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (hleInterfaceDb.DoesPersonalExistsByRefNoNric(hlePersonal.HleNumber, hlePersonal.Nric) ||
                            hleInterfaceDb.DoesPersonalExistsByRefNoName(hlePersonal.HleNumber, hlePersonal.Name))
                            isSaved = hleInterfaceDb.Update(hlePersonal);
                        else
                            isSaved = hleInterfaceDb.Insert(hlePersonal,ip,systemInfo) > 0;

                        if (isSaved && result >= 0)
                            result = (++inserted);
                    }
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
                        foreach (Leas.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
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
            Leas.Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");

            foreach (Leas.Personal personal in sortedHaPersonal)
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
                            hleInterfaceDb.DoesPersonalExistsByRefNoName(hlePersonal.HleNumber, hlePersonal.Name))
                            isSaved = hleInterfaceDb.Update(hlePersonal);
                        else
                            isSaved = hleInterfaceDb.Insert(hlePersonal,ip,systemInfo) > 0;

                        if (isSaved && result >= 0)
                            result = (++inserted);
                    }
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
                        foreach (Leas.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {           
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

            #region Delete all personal marked for deletion
            Leas.Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Leas.Personal personal in toBeDeletedPersonal)
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
                foreach (Leas.Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNo, personal.Nric, "HLE");

                        if (isSaved)
                            result = (++deleted);
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

            // Refresh the household structure
            try
            {
                appPersonalDb.SavePersonalRecords(docAppId);
                #region Added By Edward 10/3/2014  Sales and Resale Changes
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

        WriteToFile(logFilePath, "===============================");
        WriteToFile(logFilePath, "Date/Time Ended: " + DateTime.Now.ToString());
        return result;
    }

    private  Leas.Personal[] RetrievePersonals(Leas.Personal[] personals, string applicantType)
    {
        List<Leas.Personal> temp = new List<Leas.Personal>();

        // Sort the HA personals
        if (applicantType.ToUpper().Equals("HA"))
        {
            #region Get all HA
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Leas.Personal personal in personals)
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
            foreach (Leas.Personal personal in personals)
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
            foreach (Leas.Personal personal in personals)
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
            foreach (Leas.Personal personal in personals)
            {
                if (personal.IsDeletion)
                {
                    temp.Add(personal);
                }
            }
            #endregion
        }

        Leas.Personal[] newPersonals = new Leas.Personal[temp.Count];

        int cnt = 0;
        foreach (Leas.Personal personal in temp)
        {
            newPersonals[cnt] = personal;
            cnt++;
        }

        return newPersonals;
    }

    private  bool DeleteFromInterfaceAndAppPersonal(int docAppId, string refNo, string nric, string div)
    {
        HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
        SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
        ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();

        // Delete from AppPersonal table
        //appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);
        //Uncommented by Edward 17/12/2013
        appPersonalDb.DeleteByDocAppIdAndNric(docAppId, nric);

        // Delete from LeasInterface table
        leasInterfaceDb.Delete(refNo, nric);

        // Delete the applicant info for the application if marked for deletion
        if (div.ToUpper() == "HLE")
            return hleInterfaceDb.DeleteByRefNoAndNric(refNo, nric);
        else if (div.ToUpper() == "SALES")
            return salesInterfaceDb.DeleteByRefNoAndNric(refNo, nric);
        else if (div.ToUpper() == "RESALE")
            return resaleInterfaceDb.DeleteByRefNoAndNric(refNo, nric);
        else
            return true;
    }

    private  int Validate(string refNumber, DateTime applicationDate, string status,
        string preEligibilityOic, string creditAccessmentOic, decimal householdIncome, decimal creditAccessmentIncome,
        DateTime rejectionDate, DateTime cancellationDate, DateTime approvalDate, DateTime expiryDate, DateTime secondCADate, bool secondCA, Leas.Personal[] personals)
    {

        if (personals != null && personals.Length > 0)
        {
            foreach (Leas.Personal personal in personals)
            {
                if (personal == null)
                    return WRONG_INPUT; // -2

                if (personal.IsDeletion && personal.IsMainApplicant)
                    return DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED; // -4

                if (String.IsNullOrEmpty(personal.Nric))// edit by calvin temp remove for XIN, XIN was able to import before 5 feb || (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (!string.IsNullOrEmpty(personal.CustomerId))
                {
                    if (personal.CustomerId.Contains("&#"))
                        return WRONG_INPUT; // -2
                }
                if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (personal.MonthlyIncomeSet != null)
                {
                    foreach (Leas.MonthlyIncome monthlyIncome in personal.MonthlyIncomeSet)
                    {
                        if (monthlyIncome == null)
                            return WRONG_INPUT; // -2
                    }
                }

                if (personal.Documents != null)
                {
                    foreach (Leas.Document document in personal.Documents)
                    {
                        if (document == null)
                            return WRONG_INPUT; // -2
                    }
                }
            }
        }

        return 0;
    }

    #endregion

    #region RESALE

    private void InsertRESALE(XElement element, string filePathOfLogFile, string ip, string systemInfo, string recipientEmail,
        int BUId, out int NoOfFailed)
    {
        NoOfFailed = 0;
        string caseNumber = string.Empty;
        try
        {
            #region Application Elements
            caseNumber = element.Element("CaseNumber").Value;
            string schemeAccount = element.Element("SchemeAccount").Value;
            string oldLesseeCode = element.Element("OldLesseeCode").Value;
            string newLesseeCode = element.Element("NewLesseeCode").Value;
            string buyerAllocationScheme = element.Element("BuyerAllocationScheme").Value;
            string buyerEligibilityScheme = element.Element("BuyerEligibilityScheme").Value;
            string codeRecipient = element.Element("CodeRecipient").Value;
            string ROIC = element.Element("ROIC").Value;
            string caseOIC = element.Element("CaseOIC").Value;
            string eligibilityScheme = element.Element("EligibilityScheme").Value;
            string householdType = element.Element("HouseholdType").Value;
            string AHGGrant = element.Element("AHGGrant").Value;
            string HH2YINC = element.Element("HH2YINC").Value;
            string bankLoanAmount = element.Element("BankLoanAmount").Value;
            string ABCDELoanAmount = element.Element("ABCDELoanAmount").Value;
            string householdIncome = element.Element("HouseholdIncome").Value;
            string CAIncome = element.Element("CAIncome").Value;
            string applicationDate = Format.CheckDateTime(element.Element("ApplicationDate").Value);
            string appointmentDate = Format.CheckDateTimeWithTimeFormat(element.Element("AppointmentDate").Value);
            string completionDate = Format.CheckDateTime(element.Element("CompletionDate").Value);
            string cancellationDate = Format.CheckDateTime(element.Element("CancellationDate").Value);
            bool refreshHouseholdStructure = Format.CheckBoolean(element.Element("RefreshHouseholdStructure").Value);
            #endregion

            List<Resale.Personal> listPersonal = new List<Resale.Personal>();
            foreach (XElement xePersonal in element.Elements())
            {

                if (xePersonal.Name == "Personal")
                {
                    Resale.Personal personal = new Resale.Personal();
                    personal.IsDeletion = Format.CheckBoolean(xePersonal.Element("IsDeletion").Value);
                    personal.PersonalType = xePersonal.Element("PersonalType").Value;
                    personal.CustomerId = xePersonal.Element("CustomerId").Value;
                    personal.IsMainApplicant = Format.CheckBoolean(xePersonal.Element("IsMainApplicant").Value);
                    personal.Nric = xePersonal.Element("Nric").Value;
                    personal.Name = xePersonal.Element("Name").Value;
                    personal.MaritalStatus = xePersonal.Element("MaritalStatus").Value;
                    personal.DateOfBirth = DateTime.Parse(Format.CheckDateTime(xePersonal.Element("DateOfBirth").Value));
                    personal.Relationship = xePersonal.Element("Relationship").Value;
                    personal.EmploymentStatus = xePersonal.Element("EmploymentStatus").Value;
                    personal.Citizenship = xePersonal.Element("Citizenship").Value;
                    personal.NumberOfIncomeMonths = int.Parse(xePersonal.Element("NumberOfIncomeMonths").Value);

                    #region MonthlyIncome Elements
                    List<Resale.MonthlyIncome> listMonthlyIncome = new List<Resale.MonthlyIncome>();
                    foreach (XElement xeMonthlyIncome in xePersonal.Elements())
                    {
                        if (xeMonthlyIncome.Name == "MonthlyIncome")
                        {
                            string strYrMnth = string.Empty;
                            foreach (XElement xeMonthYear in xeMonthlyIncome.Descendants())
                            {
                                if (xeMonthYear.Name == "YearMonth")                                
                                    strYrMnth = xeMonthYear.Value;                                                                                                  
                                else if (xeMonthYear.Name == "Amount")
                                {
                                    Resale.MonthlyIncome monthlyIncome = new Resale.MonthlyIncome();
                                    monthlyIncome.YearMonth = strYrMnth;
                                    monthlyIncome.Amount = decimal.Parse(xeMonthYear.Value);
                                    listMonthlyIncome.Add(monthlyIncome);
                                }                      
                            }
                        }
                    }
                    #endregion

                    #region Document Elements
                    List<Resale.Document> listDocument = new List<Resale.Document>();
                    foreach (XElement xeDocument in xePersonal.Elements())
                    {
                        if (xeDocument.Name == "Document")
                        {
                            string strDocumentTypeCode, strDocumentTypeSubCode, strCategory, strCategoryGroup, strCategoryDate, strDate;
                            strDocumentTypeCode = strDocumentTypeSubCode = strCategory = strCategoryGroup = strCategoryDate=strDate = string.Empty;
                            foreach (XElement xeElement in xeDocument.Descendants())
                            {
                                if (xeElement.Name == "DocumentTypeCode")
                                    strDocumentTypeCode = xeElement.Value;
                                else if (xeElement.Name == "DocumentTypeSubCode")
                                    strDocumentTypeSubCode = xeElement.Value;
                                else if (xeElement.Name == "Category")
                                    strCategory = xeElement.Value;
                                else if (xeElement.Name == "CategoryGroup")
                                    strCategoryGroup = xeElement.Value;
                                else if (xeElement.Name == "CategoryDate")
                                    strCategoryDate = xeElement.Value;
                                else if (xeElement.Name == "Date")
                                    strDate = xeElement.Value;
                                else if (xeElement.Name == "Status")
                                {
                                    Resale.Document document = new Resale.Document();
                                    document.DocumentTypeCode = strDocumentTypeCode;
                                    document.DocumentTypeSubCode = strDocumentTypeSubCode;
                                    document.Category = strCategory;
                                    document.CategoryGroup = strCategoryGroup;
                                    document.CategoryDate = strCategoryDate;
                                    document.Date = strDate;
                                    document.Status = xeDocument.Element("Status").Value;
                                    listDocument.Add(document);
                                }                                                                
                            }
                        }
                    }
                    #endregion

                    personal.MonthlyIncomeSet = listMonthlyIncome.ToArray();
                    personal.Documents = listDocument.ToArray();
                    listPersonal.Add(personal);
                }
            }

            string logFilePath = WriteInfoToTextFile(caseNumber, filePathOfLogFile);

            int errorCode = Validate(caseNumber, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                householdIncome, CAIncome, DateTime.Parse(applicationDate),
                DateTime.Parse(appointmentDate), DateTime.Parse(completionDate), DateTime.Parse(cancellationDate), listPersonal.ToArray());

            if (errorCode < 0)
            {
                NoOfFailed++;
                WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 1);
                BatchUploadDb.InsertBUDetails(BUId, caseNumber, false, GetErrorMessage(errorCode));
            }
            else
            {

                string refType = Util.GetReferenceType(caseNumber);

                if (refType.Equals(ReferenceTypeEnum.RESALE.ToString()))
                {
                    errorCode = SaveResaleInterface(caseNumber, refType, schemeAccount, oldLesseeCode, newLesseeCode, buyerAllocationScheme,
                       buyerEligibilityScheme, codeRecipient, ROIC, caseOIC, eligibilityScheme, householdType, AHGGrant, HH2YINC, bankLoanAmount, ABCDELoanAmount,
                       householdIncome, CAIncome, applicationDate, appointmentDate, completionDate,
                       cancellationDate, refreshHouseholdStructure, listPersonal.ToArray(), logFilePath, ip, systemInfo);

                    if (errorCode < 0)
                    {
                        NoOfFailed++;
                        WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 2);
                        BatchUploadDb.InsertBUDetails(BUId, caseNumber, false, GetErrorMessage(errorCode));
                    }
                    else
                        BatchUploadDb.InsertBUDetails(BUId, caseNumber, true, string.Empty);
                }
                else
                {
                    NoOfFailed++;
                    WriteToFileWhenError(filePathOfLogFile, "Reference number is not valid for Resale", 2);
                    BatchUploadDb.InsertBUDetails(BUId, caseNumber, false, "Reference number is not valid for Resale");
                }
            }
        }
        catch (Exception ex)
        {
            NoOfFailed++;
            WriteToFileWhenError(filePathOfLogFile, ex.Message, 3);
            BatchUploadDb.InsertBUDetails(BUId, caseNumber, false, ex.Message);
        }
        
    }

    private  int SaveResaleInterface(string caseNumber, string refType, string schemeAccount, string oldLesseeCode, string newLesseeCode, string buyerAllocationScheme,
        string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme, string householdType, string AHGGrant,
        string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome, string applicationDate,
        string appointmentDate, string completionDate, string cancellationDate, 
        bool refreshHouseholdStructure, Resale.Personal[] personals, string logFilePath, string ip, string systemInfo)
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
            docAppId = docAppDb.InsertRefNumber(caseNumber, refType, caseOIC, ROIC, null, false, string.Empty, true, ip, systemInfo);
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
                        if (resaleInterfaceDb.UpdateApplication(caseNumber, ROIC, caseOIC, AHGGrant, HH2YINC, schemeAccount, oldLesseeCode,
                        newLesseeCode, buyerAllocationScheme, buyerEligibilityScheme, bankLoanAmount, ABCDELoanAmount, CAIncome, codeRecipient, eligibilityScheme, householdType,
                        applicationDate, appointmentDate, completionDate, cancellationDate, householdIncome))
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

            int inserted = 0;
            int deleted = 0;

            #region Insert the OC personal info
            Resale.Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Resale.Personal personal in sortedOcPersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "OC"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Resale.Document document in personal.Documents)
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
            Resale.Personal[] sortedPRPersonal = RetrievePersonals(personals, "PR");

            int prOrderNo = 1;
            foreach (Resale.Personal personal in sortedPRPersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "PR"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal,ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Resale.Document document in personal.Documents)
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
            Resale.Personal[] sortedPrPersonal = RetrievePersonals(personals, "CH");

            int chOrderNo = 1;
            foreach (Resale.Personal personal in sortedPrPersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "CH"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Resale.Document document in personal.Documents)
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
            Resale.Personal[] sortedSePersonal = RetrievePersonals(personals, "SE");

            int seOrderNo = 1;
            foreach (Resale.Personal personal in sortedSePersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "SE"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Resale.Document document in personal.Documents)
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
            Resale.Personal[] sortedSpPersonal = RetrievePersonals(personals, "SP");

            int spOrderNo = 1;
            foreach (Resale.Personal personal in sortedSpPersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "SP"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }                    
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
                        foreach (Resale.Document document in personal.Documents)
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
            Resale.Personal[] sortedBuPersonal = RetrievePersonals(personals, "BU");

            foreach (Resale.Personal personal in sortedBuPersonal)
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
                            resaleInterfaceDb.DoesPersonalExistsByCaseNoName(resalePersonal.CaseNo, resalePersonal.Name, "BU"))
                            isSaved = resaleInterfaceDb.Update(resalePersonal);
                        else
                            isSaved = resaleInterfaceDb.Insert(resalePersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Resale.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
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

            #region Delete all personal marked for deletion
            Resale.Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Resale.Personal personal in toBeDeletedPersonal)
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
                foreach (Resale.Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        // Check if the personal has a record in AppPersonal table with documents tied to it
                        if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        {
                            isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, caseNumber, personal.Nric, "RESALE");

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
                #region Added By Edward 10/3/2014  Sales and Resale Changes
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
        WriteToFile(logFilePath, "===============================");
        WriteToFile(logFilePath, "Date/Time Ended: " + DateTime.Now.ToString());
        return result;
    }

    private  Resale.Personal[] RetrievePersonals(Resale.Personal[] personals, string applicantType)
    {
        List<Resale.Personal> temp = new List<Resale.Personal>();

        // Sort the B personals
        if (applicantType.ToUpper().Equals("BU"))
        {
            #region Get all B
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
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
            foreach (Resale.Personal personal in personals)
            {
                if (personal.IsDeletion)
                {
                    temp.Add(personal);
                }
            }
            #endregion
        }

        Resale.Personal[] newPersonals = new Resale.Personal[temp.Count];

        int cnt = 0;
        foreach (Resale.Personal personal in temp)
        {
            newPersonals[cnt] = personal;
            cnt++;
        }

        return newPersonals;
    }

    private  int Validate(string caseNumber, string schemeAccount, string oldLesseeCode, string newLesseeCode,
        string buyerAllocationScheme, string buyerEligibilityScheme, string codeRecipient, string ROIC, string caseOIC, string eligibilityScheme,
        string householdType, string AHGGrant, string HH2YINC, string bankLoanAmount, string ABCDELoanAmount, string householdIncome, string CAIncome,
        DateTime applicationDate, DateTime appointmentDate, DateTime completionDate, DateTime cancellationDate, Resale.Personal[] personals)
    {        
        if (personals != null && personals.Length > 0)
        {
            foreach (Resale.Personal personal in personals)
            {
                if (personal == null)
                    return WRONG_INPUT; // -2

                if (personal.IsDeletion && personal.IsMainApplicant)
                    return DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED; // -4

                if (String.IsNullOrEmpty(personal.Nric))// edit by calvin temp remove for XIN, XIN was able to import before 5 feb || (!Validation.IsNric(personal.Nric) && !Validation.IsFin(personal.Nric)))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (!string.IsNullOrEmpty(personal.CustomerId))
                {
                    if (personal.CustomerId.Contains("&#"))
                        return WRONG_INPUT; // -2
                }

                if (!personal.IsDeletion && String.IsNullOrEmpty(personal.Name))
                    return PERSONAL_WITH_INVALID_NRIC_OR_NAME; // -6

                if (!personal.PersonalType.ToUpper().Equals("BU") && !personal.PersonalType.ToUpper().Equals("OC") &&
                    !personal.PersonalType.ToUpper().Equals("PR") && !personal.PersonalType.ToUpper().Equals("CH") &&
                    !personal.PersonalType.ToUpper().Equals("SE") && !personal.PersonalType.ToUpper().Equals("SP"))
                    return -7; // -7

                if (personal.MonthlyIncomeSet != null)
                {
                    foreach (Resale.MonthlyIncome monthlyIncome in personal.MonthlyIncomeSet)
                    {
                        if (monthlyIncome == null)
                            return WRONG_INPUT; // -2
                    }
                }

                if (personal.Documents != null)
                {
                    foreach (Resale.Document document in personal.Documents)
                    {
                        if (document == null)
                            return WRONG_INPUT; // -2
                    }
                }
            }
        }

        return 0;
    }

    #endregion

    #region SALES

    private void InsertSALES(XElement element, string filePathOfLogFile, string ip, string systemInfo, string recipientEmail,
        int BUId, out int NoOfFailed)
    {
        NoOfFailed = 0;
        string refNumber = string.Empty;
        try
        {
            refNumber = element.Element("RefNumber").Value;
            string status = element.Element("Status").Value;
            string ballotQuarter = element.Element("BallotQuarter").Value;
            string householdType = element.Element("HouseholdType").Value;
            string allocationMode = element.Element("AllocationMode").Value;
            string allocationScheme = element.Element("AllocationScheme").Value;
            string eligibilityScheme = element.Element("EligibilityScheme").Value;
            string householdIncome = element.Element("HouseholdIncome").Value;
            string flatType = element.Element("FlatType").Value;
            string AHGStatus = element.Element("AHGStatus").Value;
            string SHGStatus = element.Element("SHGStatus").Value;
            string loanTag = element.Element("LoanTag").Value;
            string bookedAddress = element.Element("BookedAddress").Value;
            string flatDesign = element.Element("FlatDesign").Value;
            string contactNumbers = element.Element("ContactNumbers").Value;
            string affectedSERS = element.Element("AffectedSERS").Value;
            string acceptanceDate = Format.CheckDateTime(element.Element("AcceptanceDate").Value);
            string applicationDate = Format.CheckDateTime(element.Element("ApplicationDate").Value);
            string keyIssueDate = Format.CheckDateTime(element.Element("KeyIssueDate").Value);
            string cancelledDate = Format.CheckDateTime(element.Element("CancelledDate").Value);
            bool refreshHouseholdStructure = Format.CheckBoolean(element.Element("RefreshHouseholdStructure").Value);
            string oic1 = element.Element("OIC1").Value;
            string oic2 = element.Element("OIC2").Value;
            string ABCDERefNumber = element.Element("ABCDERefNumber").Value;
            List<Sales.Personal> listPersonal = new List<Sales.Personal>();
            foreach (XElement xePersonal in element.Elements())
            {

                if (xePersonal.Name == "Personal")
                {
                    Sales.Personal personal = new Sales.Personal();
                    personal.IsDeletion = Format.CheckBoolean(xePersonal.Element("IsDeletion").Value);
                    personal.PersonalType = xePersonal.Element("PersonalType").Value;
                    personal.CustomerId = xePersonal.Element("CustomerId").Value;
                    personal.IsMainApplicant = Format.CheckBoolean(xePersonal.Element("IsMainApplicant").Value);
                    personal.Nric = xePersonal.Element("Nric").Value;
                    personal.Name = xePersonal.Element("Name").Value;
                    personal.MaritalStatus = xePersonal.Element("MaritalStatus").Value;
                    personal.DateOfBirth = DateTime.Parse(Format.CheckDateTime(xePersonal.Element("DateOfBirth").Value));
                    personal.Relationship = xePersonal.Element("Relationship").Value;
                    personal.Citizenship = xePersonal.Element("Citizenship").Value;
                    //personal.Income = xePersonal.Element("Income").Value;
                    personal.EmploymentStatus = xePersonal.Element("EmploymentStatus").Value;
                    personal.IsMCPS = Format.CheckBoolean(xePersonal.Element("IsMCPS").Value);
                    personal.IsPPO = Format.CheckBoolean(xePersonal.Element("IsPPO").Value);
                    personal.NRICSOC = xePersonal.Element("NRICSOC").Value;
                    personal.NameSOC = xePersonal.Element("NameSOC").Value;
                    personal.NumberOfIncomeMonths = int.Parse(xePersonal.Element("NumberOfIncomeMonths").Value);

                    #region MonthlyIncome Elements
                    List<Sales.MonthlyIncome> listMonthlyIncome = new List<Sales.MonthlyIncome>();
                    foreach (XElement xeMonthlyIncome in xePersonal.Elements())
                    {
                        if (xeMonthlyIncome.Name == "MonthlyIncome")
                        {
                            string strYrMnth = string.Empty;
                            foreach (XElement xeMonthYear in xeMonthlyIncome.Descendants())
                            {
                                if (xeMonthYear.Name == "YearMonth")
                                    strYrMnth = xeMonthYear.Value;
                                else if (xeMonthYear.Name == "Amount")
                                {
                                    Sales.MonthlyIncome monthlyIncome = new Sales.MonthlyIncome();
                                    monthlyIncome.YearMonth = strYrMnth;
                                    monthlyIncome.Amount = decimal.Parse(xeMonthYear.Value);
                                    listMonthlyIncome.Add(monthlyIncome);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Document Elements
                    List<Sales.Document> listDocument = new List<Sales.Document>();
                    foreach (XElement xeDocument in xePersonal.Elements())
                    {
                        if (xeDocument.Name == "Document")
                        {
                            string strDocumentTypeCode, strDocumentTypeSubCode, strCategory, strCategoryGroup, strCategoryDate, strDate;
                            strDocumentTypeCode = strDocumentTypeSubCode = strCategory = strCategoryGroup = strCategoryDate = strDate = string.Empty;
                            foreach (XElement xeElement in xeDocument.Descendants())
                            {
                                if (xeElement.Name == "DocumentTypeCode")
                                    strDocumentTypeCode = xeElement.Value;
                                else if (xeElement.Name == "DocumentTypeSubCode")
                                    strDocumentTypeSubCode = xeElement.Value;
                                else if (xeElement.Name == "Category")
                                    strCategory = xeElement.Value;
                                else if (xeElement.Name == "CategoryGroup")
                                    strCategoryGroup = xeElement.Value;
                                else if (xeElement.Name == "CategoryDate")
                                    strCategoryDate = xeElement.Value;
                                else if (xeElement.Name == "Date")
                                    strDate = xeElement.Value;
                                else if (xeElement.Name == "Status")
                                {
                                    Sales.Document document = new Sales.Document();
                                    document.DocumentTypeCode = strDocumentTypeCode;
                                    document.DocumentTypeSubCode = strDocumentTypeSubCode;
                                    document.Category = strCategory;
                                    document.CategoryGroup = strCategoryGroup;
                                    document.CategoryDate = strCategoryDate;
                                    document.Date = strDate;
                                    document.Status = xeDocument.Element("Status").Value;
                                    listDocument.Add(document);
                                }
                            }
                        }
                    }
                    #endregion

                    personal.MonthlyIncomeSet = listMonthlyIncome.ToArray();
                    personal.Documents = listDocument.ToArray();
                    listPersonal.Add(personal);
                }
            }

            string logFilePath = WriteInfoToTextFile(refNumber, filePathOfLogFile);

            int errorCode = Validate(refNumber, status, ballotQuarter, householdType, allocationMode,
                    allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                    flatDesign, contactNumbers, affectedSERS, DateTime.Parse(acceptanceDate), DateTime.Parse(applicationDate), 
                    listPersonal.ToArray(),DateTime.Parse(keyIssueDate), DateTime.Parse(cancelledDate));

            if (errorCode < 0)
            {
                NoOfFailed++;
                WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 1);
                BatchUploadDb.InsertBUDetails(BUId, refNumber, false, GetErrorMessage(errorCode));
            }
            else
            {
                string refType = Util.GetReferenceType(refNumber);

                if (refType.Equals(ReferenceTypeEnum.SALES.ToString()))
                {
                    errorCode = SaveSalesInterface(refNumber, refType, status, ballotQuarter, householdType, allocationMode,
                       allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                       flatDesign, contactNumbers, affectedSERS, oic1, oic2, acceptanceDate, 
                       applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber, refreshHouseholdStructure,
                       listPersonal.ToArray(), logFilePath, ip, systemInfo);

                    if (errorCode < 0)
                    {
                        NoOfFailed++;
                        WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 2);
                        BatchUploadDb.InsertBUDetails(BUId, refNumber, false, GetErrorMessage(errorCode));
                    }
                    else
                        BatchUploadDb.InsertBUDetails(BUId, refNumber, true, string.Empty);
                }
                else
                {
                    NoOfFailed++;
                    WriteToFileWhenError(filePathOfLogFile, "Reference number is not valid for Sales", 2);
                    BatchUploadDb.InsertBUDetails(BUId, refNumber, false, "Reference number is not valid for Sales");
                }
            }
        }
        catch (Exception ex)
        {
            NoOfFailed++;
            WriteToFileWhenError(filePathOfLogFile, ex.Message, 3);
            BatchUploadDb.InsertBUDetails(BUId, refNumber, false, ex.Message);
        }
        
    }

    private  int Validate(string refNumber, string status, string ballotQuarter, string householdType, string allocationMode,
        string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
        string flatDesign, string contactNumbers, string affectedSERS, DateTime acceptanceDate, DateTime applicationDate,
        Sales.Personal[] personals, DateTime keyIssueDate, DateTime cancelledDate)
    {       

        if (personals != null && personals.Length > 0)
        {
            foreach (Sales.Personal personal in personals)
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

                if (!personal.PersonalType.ToUpper().Equals("HA") && !personal.PersonalType.ToUpper().Equals("OC"))
                    return -7; // -7

                if (personal.Documents != null)
                {
                    foreach (Sales.Document document in personal.Documents)
                    {
                        if (document == null)
                            return WRONG_INPUT; // -2
                    }
                }
            }
        }

        return 0;
    }

    private  int SaveSalesInterface(string refNumber, string refType, string status, string ballotQuarter, string householdType, string allocationMode,
       string allocationScheme, string eligibilityScheme, string householdIncome, string flatType, string AHGStatus, string SHGStatus, string loanTag, string bookedAddress,
       string flatDesign, string contactNumbers, string affectedSERS, string OIC1, string OIC2, string acceptanceDate,
        string applicationDate, string keyIssueDate, string cancelledDate, string ABCDERefNumber, bool refreshHouseholdStructure, 
        Sales.Personal[] personals, string logFilePath, string ip, string systemInfo)
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
            docAppId = docAppDb.InsertRefNumber(refNumber, refType, OIC2, OIC1, null, false, string.Empty, true, ip, systemInfo);
        }
        catch (Exception e)
        {
            WriteToFile(logFilePath, "===============================");
            WriteToFile(logFilePath, "Insert/Update OIC Exception: " + e.Message);
            result = PROCESSING_ERROR;
        }
        #endregion

        resulttemp = result;
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

        if (personals != null && personals.Length > 0)
        {
            // If the refresh flag is set, remove the existing record for the reference number
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

            int inserted = 0;
            int deleted = 0;

            #region Insert the OC personal info
            Sales.Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Sales.Personal personal in sortedOcPersonal)
            {
                #region Insert the Sales household structure data for OC
                SalesPersonal salesPersonal = new SalesPersonal();
                salesPersonal.AddPersonalInfoFromWebService(refNumber, status, ballotQuarter, householdType, allocationMode,
                    allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                    flatDesign, contactNumbers, affectedSERS, OIC1, OIC2, acceptanceDate, applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber, ocOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Nric) ||
                            salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Name))
                            isSaved = salesInterfaceDb.Update(salesPersonal);
                        else
                            isSaved = salesInterfaceDb.Insert(salesPersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Sales.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
                                    //if (leasInterfaceDb.GetLeasInterfaceByByHleNumberNricDocTypeCodeAandDocStartDate(salesPersonal.RefNumber.Trim(),
                                    //    salesPersonal.Nric.Trim(), docTypeCode, document.Date.Trim()).Rows.Count > 0)
                                    //    leasInterfaceDb.Update(salesPersonal.RefNumber, salesPersonal.Nric, docTypeCode, document.Date,
                                    //        string.Empty, true, true, string.Empty, string.Empty, document.Category, document.CategoryGroup, document.CategoryDate, document.Status);
                                    //else
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
            Sales.Personal[] sortedBuPersonal = RetrievePersonals(personals, "HA");

            foreach (Sales.Personal personal in sortedBuPersonal)
            {
                #region Insert the Sales household structure data for HA
                int buOrderNo = salesInterfaceDb.GetOrderNumberForNric(refNumber, personal.PersonalType, personal.Nric);

                SalesPersonal salesPersonal = new SalesPersonal();
                salesPersonal.AddPersonalInfoFromWebService(refNumber, status, ballotQuarter, householdType, allocationMode,
                    allocationScheme, eligibilityScheme, householdIncome, flatType, AHGStatus, SHGStatus, loanTag, bookedAddress,
                    flatDesign, contactNumbers, affectedSERS,OIC1, OIC2, acceptanceDate,  applicationDate, keyIssueDate, cancelledDate, ABCDERefNumber, buOrderNo++, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Nric) ||                           
                            salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RefNumber, salesPersonal.Name))
                            isSaved = salesInterfaceDb.Update(salesPersonal);
                        else
                            isSaved = salesInterfaceDb.Insert(salesPersonal,ip,systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }
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
                        foreach (Sales.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
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
            Sales.Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Sales.Personal personal in toBeDeletedPersonal)
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
                foreach (Sales.Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        // Check if the personal has a record in AppPersonal table with documents tied to it
                        if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        {
                            isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNumber, personal.Nric, "SALES");

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

            // Refresh the household structure
            try
            {
                appPersonalDb.SavePersonalRecords(docAppId);
                #region Added By Edward 10/3/2014  Sales and Resale Changes
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
            Sales.Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Sales.Personal personal in sortedOcPersonal)
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
            Sales.Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");
            
            foreach (Sales.Personal personal in sortedHaPersonal)
            {
                #region Insert the Sers household structure data for HA
                int orderNo = sersInterfaceDb.GetOrderNumberForNric(affectedSERS, personal.PersonalType, personal.Nric);
                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromSales(personal, affectedSERS, orderNo);

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
            Sales.Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Sales.Personal personal in toBeDeletedPersonal)
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
                foreach (Sales.Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        // Check if the personal has a record in AppPersonal table with documents tied to it
                        if (!CheckIfPersonalHasDocuments(docAppId, personal.Nric))
                        {                            
                            isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNumber, personal.Nric, "SERS");

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

        WriteToFile(logFilePath, "===============================");
        WriteToFile(logFilePath, "Date/Time Ended: " + DateTime.Now.ToString());
        return result;
    }

    private  Sales.Personal[] RetrievePersonals(Sales.Personal[] personals, string applicantType)
    {
        List<Sales.Personal> temp = new List<Sales.Personal>();

        // Sort the B personals
        if (applicantType.ToUpper().Equals("HA"))
        {
            #region Get all HA
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Sales.Personal personal in personals)
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
            foreach (Sales.Personal personal in personals)
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
            foreach (Sales.Personal personal in personals)
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
            foreach (Sales.Personal personal in personals)
            {
                if (personal.IsDeletion)
                {
                    temp.Add(personal);
                }
            }
            #endregion
        }

        Sales.Personal[] newPersonals = new Sales.Personal[temp.Count];

        int cnt = 0;
        foreach (Sales.Personal personal in temp)
        {
            newPersonals[cnt] = personal;
            cnt++;
        }

        return newPersonals;
    }

    private  bool CheckIfPersonalHasDocuments(int docAppId, string nric)
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

    #endregion

    #region SERS

    private void InsertSERS(XElement element, string filePathOfLogFile, string ip, string systemInfo, string recipientEmail,
        int BUId, out int NoOfFailed)
    {
        NoOfFailed = 0;
        string refNo = string.Empty;
        try
        {
            refNo = element.Element("RefNumber").Value;
            string registrationNumber = element.Element("RegistrationNumber").Value;
            string stage = element.Element("Stage").Value;
            string block = element.Element("Block").Value;
            string street = element.Element("Street").Value;
            string level = element.Element("Level").Value;
            string unitNumber = element.Element("UnitNumber").Value;
            string postalCode = element.Element("PostalCode").Value;
            string allocationScheme = element.Element("AllocationScheme").Value;
            string eligibilityScheme = element.Element("EligibilityScheme").Value;
            bool refreshHouseholdStructure = Format.CheckBoolean(element.Element("RefreshHouseholdStructure").Value);
            string oic1 = element.Element("OIC1").Value;
            string oic2 = element.Element("OIC2").Value;
            string surrenderDate = element.Element("surrenderDate").Value;  //From SurrenderDate to surrenderDate 2015/06/25 by Edward
            List<Sers.Personal> listPersonal = new List<Sers.Personal>();
            foreach (XElement xePersonal in element.Elements())
            {

                if (xePersonal.Name == "Personal")
                {
                    Sers.Personal personal = new Sers.Personal();
                    personal.IsDeletion = Format.CheckBoolean(xePersonal.Element("IsDeletion").Value);
                    personal.PersonalType = xePersonal.Element("PersonalType").Value;
                    personal.CustomerId = xePersonal.Element("CustomerId").Value;
                    personal.IsMainApplicant = Format.CheckBoolean(xePersonal.Element("IsMainApplicant").Value);
                    personal.Nric = xePersonal.Element("Nric").Value;
                    personal.Name = xePersonal.Element("Name").Value;
                    personal.MaritalStatus = xePersonal.Element("MaritalStatus").Value;
                    personal.DateOfBirth = DateTime.Parse(Format.CheckDateTime(xePersonal.Element("DateOfBirth").Value));
                    personal.Relationship = xePersonal.Element("Relationship").Value;
                    personal.PrivatePropertyOwner = Format.CheckBoolean(xePersonal.Element("PrivatePropertyOwner").Value);
                    personal.Citizenship = xePersonal.Element("Citizenship").Value;
                    List<Sers.MonthlyIncome> listMonthlyIncome = new List<Sers.MonthlyIncome>();
                    foreach (XElement xeMonthlyIncome in xePersonal.Elements())
                    {
                        if (xeMonthlyIncome.Name == "MonthlyIncome")
                        {
                            Sers.MonthlyIncome monthlyIncome = new Sers.MonthlyIncome();
                            monthlyIncome.YearMonth = xeMonthlyIncome.Element("YearMonth").Value;
                            monthlyIncome.Amount = decimal.Parse(xeMonthlyIncome.Element("Amount").Value);
                            listMonthlyIncome.Add(monthlyIncome);
                        }
                    }
                    List<Sers.Document> listDocument = new List<Sers.Document>();
                    foreach (XElement xeDocument in xePersonal.Elements())
                    {
                        if (xeDocument.Name == "Document")
                        {
                            Sers.Document document = new Sers.Document();
                            document.DocumentTypeCode = xeDocument.Element("DocumentTypeCode").Value;
                            document.DocumentTypeSubCode = xeDocument.Element("DocumentTypeSubCode").Value;
                            document.Category = xeDocument.Element("Category").Value;
                            document.CategoryGroup = xeDocument.Element("CategoryGroup").Value;
                            document.CategoryDate = xeDocument.Element("CategoryDate").Value;
                            document.Date = xeDocument.Element("Date").Value;
                            document.Status = xeDocument.Element("Status").Value;
                            listDocument.Add(document);
                        }
                    }

                    personal.Documents = listDocument.ToArray();
                    listPersonal.Add(personal);
                }
            }

            string logFilePath = WriteInfoToTextFile(refNo, filePathOfLogFile);

            int errorCode = Validate(refNo, registrationNumber, stage, block, street,
                 level, unitNumber, postalCode, allocationScheme, eligibilityScheme, listPersonal.ToArray());

            if (errorCode < 0)
            {
                NoOfFailed++;
                WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 1);
                BatchUploadDb.InsertBUDetails(BUId, refNo, false, GetErrorMessage(errorCode));
            }
            else
            {
                string refType = Util.GetReferenceType(refNo);

                SaveSersInterface(refNo, refType, registrationNumber, stage, block, street, level,
                            unitNumber, postalCode, allocationScheme, eligibilityScheme, oic1, oic2, surrenderDate, refreshHouseholdStructure,
                            listPersonal.ToArray(), logFilePath, ip, systemInfo);

                if (errorCode < 0)
                {
                    NoOfFailed++;
                    WriteToFileWhenError(filePathOfLogFile, errorCode.ToString(), 2);
                    BatchUploadDb.InsertBUDetails(BUId, refNo, false, GetErrorMessage(errorCode));
                }
                else
                    BatchUploadDb.InsertBUDetails(BUId, refNo, true, string.Empty);
            }
        }
        catch (Exception ex)
        {
            NoOfFailed++;
            WriteToFileWhenError(filePathOfLogFile, ex.Message, 3);
            BatchUploadDb.InsertBUDetails(BUId, refNo, false, ex.Message);
        }
        
    }

    private  int Validate( string refNumber, string registrationNumber, string stage,
       string block, string street, string level, string unitNumber, string postalCode, string allocationScheme,
       string eligibilityScheme,  Sers.Personal[] personals)
    {   

        if (personals != null && personals.Length > 0)
        {
            foreach (Sers.Personal personal in personals)
            {
                if (personal == null)
                    return WRONG_INPUT; // -2

                if (personal.IsDeletion && personal.IsMainApplicant)
                    return DELETING_OF_MAIN_APPLICANT_NOT_ALLOWED; // -4
 
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
                    foreach (Sers.Document document in personal.Documents)
                    {
                        if (document == null)
                            return WRONG_INPUT; // -2
                    }
                }
            }
        }

        return 0;
    }

    private  int SaveSersInterface(string refNo, string refType, string registrationNumber, string stage, string block, string street, string level,
        string unitNumber, string postalCode, string allocationScheme, string eligibilityScheme, string OIC1, string OIC2, string surrenderDate, bool refreshHouseholdStructure,
        Sers.Personal[] personals, string logFilePath, string ip, string systemInfo)
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
            docAppId = docAppDb.InsertRefNumber(refNo, refType, OIC2, OIC1, null, false, string.Empty, true, ip, systemInfo);
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
                            eligibilityScheme, registrationNumber, refNo, OIC1, OIC2,surrenderDate))
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
            Sers.Personal[] sortedOcPersonal = RetrievePersonals(personals, "OC");

            int ocOrderNo = 1;
            foreach (Sers.Personal personal in sortedOcPersonal)
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
                            isSaved = sersInterfaceDb.Insert(sersPersonal, ip, systemInfo) > 0;

                        if (isSaved)
                            result = (++inserted);
                    }  
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
                        foreach (Sers.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
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
            Sers.Personal[] sortedHaPersonal = RetrievePersonals(personals, "HA");

            foreach (Sers.Personal personal in sortedHaPersonal)
            {
                #region Insert the Sers household structure data for HA
                int orderNo = sersInterfaceDb.GetOrderNumberForNric(refNo, personal.PersonalType, personal.Nric);

                SersPersonal sersPersonal = new SersPersonal();
                sersPersonal.AddPersonalInfoFromWebService(refNo, stage, block, street, level, unitNumber, postalCode, allocationScheme,
                    eligibilityScheme, orderNo, OIC1, OIC2, surrenderDate, personal);

                bool isSaved = false;
                try
                {
                    if (!personal.IsDeletion)
                    {
                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(sersPersonal.SchAcc, sersPersonal.Nric) ||                            
                            sersInterfaceDb.DoesPersonalExistsBySchAccName(sersPersonal.SchAcc, sersPersonal.Name))
                            isSaved = sersInterfaceDb.Update(sersPersonal);
                        else
                            isSaved = sersInterfaceDb.Insert(sersPersonal, ip, systemInfo) > 0;

                        if (isSaved && result >= 0)
                            result = (++inserted);
                    }
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
                        foreach (Sers.Document document in personal.Documents)
                        {
                            if (!String.IsNullOrEmpty(document.DocumentTypeCode))
                            {
                                string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(document.DocumentTypeCode, document.DocumentTypeSubCode);

                                if (!string.IsNullOrEmpty(docTypeCode))
                                {
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
            Sers.Personal[] toBeDeletedPersonal = RetrievePersonals(personals, string.Empty);

            bool hasMainApplicantForDelete = false;
            foreach (Sers.Personal personal in toBeDeletedPersonal)
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
                foreach (Sers.Personal personal in toBeDeletedPersonal)
                {
                    #region Delete the personal record
                    bool isSaved = false;
                    try
                    {
                        isSaved = DeleteFromInterfaceAndAppPersonal(docAppId, refNo, personal.Nric, "SERS");

                        if (isSaved)
                            result = (++deleted);                        
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
        WriteToFile(logFilePath, "===============================");
        WriteToFile(logFilePath, "Date/Time Ended: " + DateTime.Now.ToString());
        return result;
    }

    private  Sers.Personal[] RetrievePersonals(Sers.Personal[] personals, string applicantType)
    {
        List<Sers.Personal> temp = new List<Sers.Personal>();

        // Sort the HA personals
        if (applicantType.ToUpper().Equals("HA"))
        {
            #region Get all HA
            string mainApplicantNric = string.Empty;

            // Get the main applicant an assign as the first item in the array
            foreach (Sers.Personal personal in personals)
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
            foreach (Sers.Personal personal in personals)
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
            foreach (Sers.Personal personal in personals)
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
            foreach (Sers.Personal personal in personals)
            {
                if (personal.IsDeletion)
                {
                    temp.Add(personal);
                }
            }
            #endregion
        }

        Sers.Personal[] newPersonals = new Sers.Personal[temp.Count];

        int cnt = 0;
        foreach (Sers.Personal personal in temp)
        {
            newPersonals[cnt] = personal;
            cnt++;
        }

        return newPersonals;
    }

    

    #endregion

    private void SendEmailAfterUploading(string recipientEmail, string file, string filePathOfLogFile)
    {
        bool emailSent;
        try
        {
            string ccEmail = "MyDocErrLog@mailbox.ABCDE.com.ph";
            //recipientEmail = "edward.fontanilla@hiend.com";
            //string ccEmail = "dwms@hiend.com";

            string subject = "DWMS - Batch Upload Notification : " + file;
            string message = file + " has been uploaded.";

            ParameterDb parameterDb = new ParameterDb();

            emailSent = Util.SendMailBU(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                            recipientEmail, ccEmail, string.Empty, string.Empty, subject, message, filePathOfLogFile);
            if (emailSent)
                WriteInfoToTextFile("Batch Upload notification email sent to  " + recipientEmail, filePathOfLogFile);
            else
                WriteInfoToTextFile("Failure sending Batch Upload notification email to " + recipientEmail, filePathOfLogFile);
        }
        catch (Exception ex)
        {
            WriteInfoToTextFile("Failure Sending Batch Upload Notification Email to " + recipientEmail + " error: " + ex.Message, filePathOfLogFile);
        }

    }

    private string GetErrorMessage(int errorCode)
    {
        string msg = string.Empty;
        switch (errorCode)
        {
            case -1:
                msg = "Authentication Error";
                break;
            case -2:
                msg = "Wrong Input";
                break;
            case -3:
                msg = "Processing Error";
                break;
            case -4:
                msg = "Deleting of Main Applicant not allowed"; 
                break;
            case -5:                
                msg = "Invalid Reference Number"; 
                break;
            case -6:
                msg = "Personal with invalid nric or name";
                break;
            default:
                msg = "Error found";
                break;
        }
        return msg;
    }
}
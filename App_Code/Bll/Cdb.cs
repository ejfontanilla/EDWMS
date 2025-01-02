using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Dwms.Bll;
using System.Web.Services.Protocols;
using System.Configuration;
using Dwms.Web;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for Cdb
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]


/*
 * 2012-12-07
 * Previous known as CBD2
 * 
 * 2012-12-10
 * added IsVerified to ImageInfoClass
 * 
 */


public class Cdb : System.Web.Services.WebService
{
    //public AuthenticationSoapHeader soapHeader;
    private const int AUTHENTICATION_ERROR = -1;
    private const int WRONG_INPUT = -2;
    private const int PROCESSING_ERROR = -3;
    private const int UNABLE_TO_DOWNLOAD_FILE = -4;
    private const int FILE_DOES_NOT_EXISTS = -5;
    private const int FILE_SIZE_DOES_NOT_MATCH = -6;
    private const int DOCUMENT_CLASS_ERROR = -7;
    //private const int CUSTOMER_HAS_NO_DOCUMENTS = -8;
    private const int DOCUMENT_HAS_NO_IMAGE_INFORMATION = -8;
    private const int DOCUMENT_HAS_NO_BUSINESS_INFORMATION = -9;
    private const int DOCUMENT_HAS_NO_DOC_CAHNNEL = -10;
    private const int ERROR_IN_VALIDATE = -11;                  //Added BY Edward 2014/09/01 To address The process cannot access the file -- Added Try Catch 

    #region Change Log
    //     2017/06/19   Edward   Add error description in Validate Method
    #endregion

    public Cdb()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public int SendDocumentInfo(AuthenticationClass authentication, AcknowledgementInfoClass acknowledgementInfo, CustomerClass[] customers)
    {
        string logFilePath = WriteInfoToTextFile(authentication, (acknowledgementInfo == null ? new AcknowledgementInfoClass() : acknowledgementInfo), customers);

        string errorDesc = string.Empty; //2017/06/19

        int errorCode = Validate(authentication, (acknowledgementInfo == null ? new AcknowledgementInfoClass() : acknowledgementInfo), customers);

        if (errorCode < 0)
        {
           
            WriteToLogFile(logFilePath, "===============================");
            WriteToLogFile(logFilePath, "Validation Check (Error Code): " + errorCode.ToString());
            WriteToLogFile(logFilePath, "===============================");

            //Rename the file to an Error file
            try
            {
                File.Move(logFilePath, logFilePath + ".Err");

                string subject = string.Empty;
                string message = string.Empty;

                subject = "Error importing from CDB web service";
                message = "There is an error code " + errorCode;

                ParameterDb parameterDb = new ParameterDb();

                Util.SendMail(parameterDb.GetParameter(ParameterNameEnum.SystemName).Trim(), parameterDb.GetParameter(ParameterNameEnum.SystemEmail).Trim(),
                    parameterDb.GetParameter(ParameterNameEnum.ErrorNotificationMailingList).Trim(), string.Empty, string.Empty, subject, message);

            }
            catch (Exception ex)
            {
                WriteToLogFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
            


            return errorCode;
        }

        List<string> referenceNumbersList = new List<string>();

        foreach (CustomerClass customer in customers)
        {
            foreach (DocumentClass document in customer.Documents)
            {
                foreach (BusinessInfoClass businessInfo in document.BusinessInfoSet)
                {
                    string refNo = businessInfo.BusinessRefNumber.ToUpper().Trim();

                    // Add the reference number to the list
                    if (!referenceNumbersList.Contains(refNo))
                        referenceNumbersList.Add(refNo);
                }
            }
        }


        try
        {
            string webServiceDir = Util.GetWebServiceDocsFolder();
            string allFileNames = string.Empty;
            int fileCount = 1;

            foreach (string refNo in referenceNumbersList)
            {
                // Append the contents of the customer class
                bool hasDocIdTemp = false;
                bool isVerified = true;
                bool hasVerified = false;
                bool hasImageInfo = false;

                string setDir = Path.Combine(webServiceDir, (acknowledgementInfo == null ?
                    DateTime.Now.ToString("CDB-yyyyMMdd-HHmmss.fff") :
                    (String.IsNullOrEmpty(acknowledgementInfo.AcknowledgementNo) ? DateTime.Now.ToString("CDB-yyyyMMdd-HHmmss.fff") : acknowledgementInfo.AcknowledgementNo)));
                //amended by calvin directory will become ref-ref when there is no ack no. Remove the ref in the first part.

                //2012-12-17
                /*
                 * 	Multiple BusinessInfoSet under a DocumentClass
                 *   To be split into separate sets and duplicate the document between the sets, if necessary.
                 * 
                 * 
                 */
                setDir = setDir + "-" + refNo.ToUpper();


                //// Delete the directory if it exists
                if (Directory.Exists(setDir))
                    setDir = setDir + Guid.NewGuid().ToString().Substring(0, 8);

                // Create the directory to save the documents
                if (!Directory.Exists(setDir))
                    Directory.CreateDirectory(setDir);

                //bool xmlCreated = false;
                string channel = string.Empty;
                StringBuilder contents = new StringBuilder();
                bool hasDocId = false;

                foreach (CustomerClass customer in customers)
                {
                    foreach (DocumentClass document in customer.Documents)
                    {
                        bool addToSet = false;

                        foreach (BusinessInfoClass businessInfo in document.BusinessInfoSet)
                        {
                            if (refNo.ToUpper().Equals(businessInfo.BusinessRefNumber.Trim().ToUpper()))
                            {
                                addToSet = true;
                                break;
                            }
                        }

                        if (addToSet)
                        {
                            // Update the AppPersonal and/or DocPersonal records
                            // #################### is IdentityNo the NRIC?
                            UpdatePersonalInfo(customer.IdentityNo, refNo, customer.CustomerIdFromSource, customer.CustomerType, customer.IdentityType);

                            string fileName = string.Empty;

                            if (document.ImageInfo != null)
                            {

                                foreach (ImageInfoClass imageInfo in document.ImageInfo)
                                {
                                    if (allFileNames.Contains(imageInfo.ImageName + ";"))
                                    {
                                        fileName = Path.GetFileNameWithoutExtension(imageInfo.ImageName) + fileCount + Path.GetExtension(imageInfo.ImageName);
                                        allFileNames += fileName + ";";
                                        imageInfo.ImageName = fileName;
                                    }
                                    else
                                    {
                                        fileName = imageInfo.ImageName;
                                        allFileNames += fileName + ";";
                                        imageInfo.ImageName = fileName;
                                    }

                                    string destinationFile = string.Empty;

                                    // Download the files
                                    for (int cnt = 0; cnt < 3; cnt++)
                                    {
                                        WriteToLogFile(logFilePath, "Attempts download (" + (cnt + 1).ToString() + "): " + fileName + "\r\n");
                                        try
                                        {
                                            if (Util.DownloadFileFromUrl(imageInfo.ImageUrl, fileName, setDir, out destinationFile))
                                            {
                                                //result = resulttemp;
                                                break;
                                            }
                                            else
                                            {
                                                WriteToLogFile(logFilePath, "===============================");
                                                WriteToLogFile(logFilePath, "Process Result: " + UNABLE_TO_DOWNLOAD_FILE.ToString());
                                                WriteToLogFile(logFilePath, "===============================");

                                                //Rename the file to an Error file
                                                try
                                                {
                                                    File.Move(logFilePath, logFilePath + ".Err");
                                                }
                                                catch (Exception ex)
                                                {
                                                    WriteToLogFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
                                                }
                                                return UNABLE_TO_DOWNLOAD_FILE; // -4
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            WriteToLogFile(logFilePath, "===============================");
                                            WriteToLogFile(logFilePath, "Download Failed: " + e.Message + "\r\n");
                                            //result = PROCESSING_ERROR;
                                        }
                                    }

                                    // Get the size of the downloaded file
                                    FileInfo file = new FileInfo(destinationFile);

                                    if (!file.Exists)
                                    {
                                        WriteToLogFile(logFilePath, "===============================");
                                        WriteToLogFile(logFilePath, "Process Result: " + FILE_DOES_NOT_EXISTS.ToString());
                                        WriteToLogFile(logFilePath, "===============================");

                                        //Rename the file to an Error file
                                        try
                                        {
                                            File.Move(logFilePath, logFilePath + ".Err");
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteToLogFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
                                        }



                                        return FILE_DOES_NOT_EXISTS; // -5
                                    }

                                    decimal fileSize = Convert.ToDecimal(file.Length) / 1024;

                                    //if (fileSize != imageInfo.ImageSize)
                                    //{
                                    //    WriteToLogFile(logFilePath, "===============================");
                                    //    WriteToLogFile(logFilePath, "Process Result: " + FILE_SIZE_DOES_NOT_MATCH.ToString());
                                    //    WriteToLogFile(logFilePath, "===============================");



                                    //    //Rename the file to an Error file
                                    //    try
                                    //    {
                                    //        File.Move(logFilePath, logFilePath + ".Err");
                                    //    }
                                    //    catch (Exception ex)
                                    //    {
                                    //        WriteToLogFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
                                    //    }



                                    //    return FILE_SIZE_DOES_NOT_MATCH; // -6
                                    //}
                                    fileCount++;

                                    hasImageInfo = true;
                                    string currDocChannel = EnumManager.MapCdbDocChannel(imageInfo.DocChannel);
                                    if (!String.IsNullOrEmpty(channel) && !channel.Equals(currDocChannel))
                                        channel = CdbDocChannelEnum.Mixed.ToString();
                                    else
                                        channel = currDocChannel;
                                    //
                                    if (!imageInfo.IsVerified)
                                        isVerified = false;
                                    if (imageInfo.IsVerified)
                                    {
                                        hasVerified = true;
                                        channel = CdbDocChannelEnum.Mixed.ToString();
                                    }
                                }
                            }
                            contents.Append(CreateCustomerInfo(customer.CustomerIdFromSource, customer.CustomerName, customer.IdentityNo, customer.IdentityType, document, out hasDocIdTemp, customer.CustomerType));
                        }
                    }
                }
                if (hasVerified && hasImageInfo)
                {
                    channel = CdbDocChannelEnum.CDB.ToString();
                    CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();
                    cdbPendingDoc.ProcessPendingDocs(refNo, "PAR", "LAT");
                }
                if (channel == CdbDocChannelEnum.MyABCDEPage.ToString() || channel == CdbDocChannelEnum.MyABCDEPage_Common_Panel.ToString().Replace("_"," ")) // Added MyABCDEPage_Common_Panel by Edward 30.10.2013 011 New Channel     // || hasVerified)// && hasImageInfo && channel != CdbDocChannelEnum.Mixed.ToString())
                {
                    //channel = CdbDocChannelEnum.CDB.ToString();
                    CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();
                    cdbPendingDoc.ProcessPendingDocs(refNo, "PAR", "LAT");
                }

                // Commented out 2012-12-11, DocChannel moved to ImageInfo
                //string currDocChannel = EnumManager.MapCdbDocChannel(document.DocChannel);
                //if (!String.IsNullOrEmpty(channel) && !channel.Equals(currDocChannel))
                //    channel = "MIXED";
                //else
                //    channel = currDocChannel;


                //2012-12-11
                if (hasDocIdTemp && !hasDocId)
                    hasDocId = hasDocIdTemp;

                //// Create the XML file
                //if (!xmlCreated)
                //{
                //    CreateXmlFile2(setDir, refNo, EnumManager.MapCdbDocChannel(document.DocChannel), customers);

                //    xmlCreated = true;
                //}

                // Create the contents of the XML file
                AddContentToXmlFile(setDir, String.Format("<ROOT><SET><REFNO>{0}</REFNO><CHANNEL>{1}</CHANNEL><HASDOCID>{2}</HASDOCID>{3}</SET></ROOT>", refNo, channel, hasDocId.ToString(), contents.ToString()));
            }

            WriteToLogFile(logFilePath, "===============================");
            WriteToLogFile(logFilePath, "Process Result: " + customers.Length.ToString());
            WriteToLogFile(logFilePath, "===============================");

            return customers.Length;
        }
        catch (Exception e)
        {
            WriteToLogFile(logFilePath, "===============================");
            WriteToLogFile(logFilePath, "Exception: " + e.Message);
            WriteToLogFile(logFilePath, "Process Result: " + PROCESSING_ERROR.ToString());
            WriteToLogFile(logFilePath, "===============================");

            //Rename the file to an Error file
            try
            {
                File.Move(logFilePath, logFilePath + ".Err");
            }
            catch (Exception ex)
            {
                WriteToLogFile(logFilePath + ".RenErr", "Failed at renaming the Error file, Message details " + ex.Message);
            }
            


            return PROCESSING_ERROR;
        }
    }

    private int Validate(AuthenticationClass authentication, AcknowledgementInfoClass acknowledgementInfo, CustomerClass[] customers)
    {
        //Added BY Edward 2014/09/01 To address The process cannot access the file -- Added Try Catch 
        try
        {
           // errorDescription = string.Empty;

            if (!ValidateLogin(authentication))
                return AUTHENTICATION_ERROR; // -1

            if (customers == null || customers.Length <= 0)
                return WRONG_INPUT; // -2

            foreach (CustomerClass customer in customers)
            {
                if (customer == null)
                    return WRONG_INPUT; // -2

                if ((customer.Documents == null) || (customer.Documents.Length <= 0) || (customer.DocCounter != customer.Documents.Length))
                    return DOCUMENT_CLASS_ERROR; // -7 The Document object is null or lenght does not tally with the length of Documents array

                foreach (DocumentClass document in customer.Documents)
                {
                    if (document == null)
                        return DOCUMENT_CLASS_ERROR; // -7

                    if (document.DocStartDate == null)
                        return WRONG_INPUT; // -2

                    if ((document.ImageInfo == null) || (document.ImageInfo.Length <= 0))
                        return DOCUMENT_HAS_NO_IMAGE_INFORMATION; // -8 Document.ImageInfo is null or list is empty 

                    foreach (ImageInfoClass imageInfo in document.ImageInfo)
                    {
                        if (imageInfo == null)
                            return DOCUMENT_HAS_NO_IMAGE_INFORMATION; // -8

                        if (imageInfo.IsVerified)
                        {
                            if (imageInfo.IsAccepted == null)
                                return WRONG_INPUT; // -2
                            if (imageInfo.IsMatchedWithExternalOrg == null)
                                return WRONG_INPUT; // -2
                            if (imageInfo.DateFiled == null)
                                return WRONG_INPUT; // -2
                            if (imageInfo.CertificateDate == null)
                                return WRONG_INPUT; // -2
                            //if (imageInfo.CertificateNumber == null)
                            //    return WRONG_INPUT; // -2
                            //if (imageInfo.LocalForeign == null)
                            //    return WRONG_INPUT; // -2
                            //if (imageInfo.MarriageType == null)
                            //    return WRONG_INPUT; // -2
                        }

                        string destinationFile = string.Empty;

                        // Download the files
                        bool downloadSuccessful = Util.DownloadFileFromUrl(imageInfo.ImageUrl, imageInfo.ImageName,
                            Util.GetTempFolder() + "\\" + Guid.NewGuid().ToString().Substring(0, 8), out destinationFile);

                        if (!downloadSuccessful)
                            return UNABLE_TO_DOWNLOAD_FILE; // -4

                        // Get the size of the downloaded file
                        FileInfo file = new FileInfo(destinationFile);

                        if (imageInfo.DocChannel == null || imageInfo.DocChannel == string.Empty)
                            return DOCUMENT_HAS_NO_DOC_CAHNNEL; // -10

                        if (!file.Exists)
                            return FILE_DOES_NOT_EXISTS; // -5

                        decimal fileSize = Convert.ToDecimal(file.Length) / 1024;

                        //if (fileSize != imageInfo.ImageSize)
                        //    return FILE_SIZE_DOES_NOT_MATCH; // -6
                    }

                    if (document.BusinessInfoSet == null)
                        return DOCUMENT_HAS_NO_BUSINESS_INFORMATION; // -9

                    foreach (BusinessInfoClass businessInfo in document.BusinessInfoSet)
                    {
                        string refNo = businessInfo.BusinessRefNumber.Trim();

                        // BusinessRefNumber must be a HLE, Sales, Resale (Case) or SERS application number
                        if (!Validation.IsHLENumber(refNo) &&
                            !Validation.IsSalesNumber(refNo) &&
                            !Validation.IsResaleNumber(refNo) &&
                            !Validation.IsSersNumber(refNo))
                        {
                            return WRONG_INPUT; // -2
                        }
                    }
                }
            }

            return 0;
        }
        catch (Exception)
        {
            return ERROR_IN_VALIDATE;
        }
        
    }

    private string WriteInfoToTextFile(AuthenticationClass authentication, AcknowledgementInfoClass acknowledgementInfo, CustomerClass[] customers)
    {
        #region Write to text file

        string ackNo = string.Empty;
        string ackDate = string.Empty;
        string appIdentityNo = string.Empty;

        if (acknowledgementInfo != null)
        {
            ackNo = acknowledgementInfo.AcknowledgementNo;
            ackDate = acknowledgementInfo.AcknowledgementDate.ToString("dd MMM yyyy");
            appIdentityNo = acknowledgementInfo.ApplicantIdentityNo;
        }

        //amended by calvin change date formating yyyyMMdd-HHmmss
        // Temporary code to write input to text file
        #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
        //string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("CDB-yyyyMMdd-HHmmss.fff") + (String.IsNullOrEmpty(ackNo) ? string.Empty : "-" + ackNo) + ".txt");
        //string filePath = Server.MapPath("~/App_Data/LogFiles/" + DateTime.Now.ToString("CDB-yyyyMMdd-HHmmss.fff") + (String.IsNullOrEmpty(ackNo) ? string.Empty : "-" + ackNo) + ".txt");
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY        
        string filePath = Util.GetLogFilesFolder("CDB", ackNo, string.Empty);
        #endregion

        StreamWriter w;
        //Added BY Edward 2014/09/01 To address The process cannot access the file -- Added if File.Exists Condition        
        if (!File.Exists(filePath))
            w = File.CreateText(filePath);
        else
            w = new StreamWriter(filePath);

        w.Write("AcknowledgementNo: " + ackNo + "\r\n");
        w.Write("AcknowledgementDate: " + ackDate + "\r\n");
        w.Write("ApplicantIdentityNo: " + appIdentityNo + "\r\n\r\n");

        foreach (CustomerClass customer in customers)
        {
            w.Write("CustomerIdFromSource: " + customer.CustomerIdFromSource + "\r\n");
            w.Write("IdentityNo: " + customer.IdentityNo + "\r\n");
            w.Write("IdentityType: " + customer.IdentityType + "\r\n");
            w.Write("CustomerName: " + customer.CustomerName + "\r\n");
            w.Write(customer.DocCounter == null ? "DocCounter: " : "DocCounter: " + customer.DocCounter.ToString() + "\r\n");
            w.Write("CustomerType: " + customer.CustomerType + "\r\n");
            w.Write("\r\n");

            if (customer.Documents != null)
            {
                foreach (DocumentClass document in customer.Documents)
                {
                    w.Write("DocId: " + document.DocId + "\r\n");
                    w.Write("DocIdSub: " + document.DocIdSub + "\r\n");
                    w.Write("DocDescription: " + (document.DocDescription.Length > 50 ? document.DocDescription.Substring(0, 50) : document.DocDescription) + "\r\n");
                    w.Write(document.DocStartDate == null ? "DocStartDate: " : "DocStartDate: " + document.DocStartDate.ToString("dd MMM yyyy") + "\r\n");
                    w.Write(document.DocEndDate == null ? "DocEndDate: " : "DocEndDate: " + document.DocEndDate.ToString("dd MMM yyyy") + "\r\n");
                    w.Write("IdentityNoSub: " + document.IdentityNoSub + "\r\n");
                    w.Write("CustomerIdSubFromSource: " + document.CustomerIdSubFromSource + "\r\n");
                    w.Write("RequesterNickname: " + document.RequesterNickname + "\r\n");
                    //w.Write(document.DateReceived == null ? "DateReceived: " : "DateReceived: " + document.DateReceived.ToString("dd MMM yyyy") + "\r\n");
                    w.Write("\r\n");

                    if (document.ImageInfo != null)
                    {
                        foreach (ImageInfoClass imageInfo in document.ImageInfo)
                        {
                            w.Write("ImageUrl: " + imageInfo.ImageUrl + "\r\n");
                            w.Write("ImageName: " + imageInfo.ImageName + "\r\n");
                            w.Write(imageInfo.ImageSize == null ? "ImageSize: " : "ImageSize: " + imageInfo.ImageSize.ToString() + "\r\n");
                            w.Write(imageInfo.DateReceivedFromSource == null ? "DateReceivedFromSource: " : "DateReceivedFromSource: " + imageInfo.DateReceivedFromSource.ToString("dd MMM yyyy") + "\r\n");
                            w.Write("DocChannel: " + imageInfo.DocChannel + "\r\n");

                            // For verified documents
                            w.Write("CmDocumentId: " + imageInfo.CmDocumentId + "\r\n");
                            w.Write("IsAccepted: " + imageInfo.IsAccepted.ToString().ToUpper() + "\r\n");
                            w.Write("IsMatchedWithExternalOrg: " + imageInfo.IsMatchedWithExternalOrg.ToString().ToUpper() + "\r\n");
                            w.Write(imageInfo.DateFiled == null ? "DateFiled: " : "DateFiled: " + imageInfo.DateFiled.ToString("dd MMM yyyy") + "\r\n");

                            // Metadata for verified documents
                            w.Write("CertificateNumber: " + imageInfo.CertificateNumber + "\r\n");
                            w.Write(imageInfo.CertificateDate == null ? "CertificateDate: " : "CertificateDate: " + imageInfo.CertificateDate.ToString("dd MMM yyyy") + "\r\n");
                            //w.Write("IsForeign: " + imageInfo.IsForeign.ToString().ToUpper() + "\r\n");
                            //w.Write("IsMuslim: " + imageInfo.IsMuslim.ToString().ToUpper() + "\r\n");

                            w.Write("LocalForeign: " + (imageInfo.LocalForeign == null ? string.Empty : imageInfo.LocalForeign.Trim()) + "\r\n");
                            w.Write("MarriageType: " + (imageInfo.MarriageType == null ? string.Empty : imageInfo.MarriageType.Trim()) + "\r\n");


                            w.Write("IsVerified: " + imageInfo.IsVerified.ToString().ToUpper() + "\r\n");
                            w.Write("\r\n");
                        }
                    }

                    if (document.BusinessInfoSet != null)
                    {
                        foreach (BusinessInfoClass businessInfo in document.BusinessInfoSet)
                        {
                            w.Write("BusinessTransactionNumber: " + businessInfo.BusinessTransactionNumber + "\r\n");
                            w.Write("BusinessRefNumber: " + businessInfo.BusinessRefNumber + "\r\n");
                            w.Write("\r\n");
                        }
                    }
                }
            }
        }

        w.Flush();
        w.Close();

        return filePath;
        #endregion
    }

    private void AddContentToXmlFile(string setDir, string contents)
    {
        try
        {
            // Temporary code to write input to text file
            string filePath = Path.Combine(setDir, "set.xml");
            if (!File.Exists(filePath))
                File.AppendAllText(filePath, contents);
            else
                File.AppendAllText(filePath+"err", contents);
        }
        catch (Exception)
        {
        }
    }

    // Not used
    private void CreateXmlFile(string setDir, string refNo, string docChannel)
    {
        // Temporary code to write input to text file
        string filePath = Path.Combine(setDir, "set.xml");
        StreamWriter w;
        w = File.CreateText(filePath);
        w.Write("<SET>");
        w.Write("<REFNO>");
        w.Write(refNo);
        w.Write("</REFNO>");
        w.Write("<CHANNEL>");
        w.Write(docChannel);
        w.Write("</CHANNEL>");
        w.Write("</SET>");
        w.Flush();
        w.Close();
    }

    // Not used
    private void CreateXmlFile2(string setDir, string refNo, string docChannel, CustomerClass[] customers)
    {
        // Temporary code to write input to text file
        string filePath = Path.Combine(setDir, "set.xml");
        StreamWriter w;
        w = File.CreateText(filePath);

        w.Write("<ROOT>");
        w.Write("<SET>");
        w.Write("<REFNO>");
        w.Write(refNo);
        w.Write("</REFNO>");
        w.Write("<CHANNEL>");
        w.Write(docChannel);
        w.Write("</CHANNEL>");

        // Customers
        int custCnt = 1;
        foreach (CustomerClass customer in customers)
        {
            w.Write(String.Format("<CUSTOMER NO='{0}'>", custCnt++));

            w.Write("<NRIC>");
            w.Write(customer.IdentityNo);
            w.Write("</NRIC>");

            int docCnt = 1;
            foreach (DocumentClass document in customer.Documents)
            {
                w.Write(String.Format("<DOC NO='{0}'>", docCnt++));

                // Doc Id and SubId
                w.Write("<DOCID>");
                w.Write(document.DocId);
                w.Write("</DOCID>");
                w.Write("<DOCSUBID>");
                w.Write(document.DocIdSub);
                w.Write("</DOCSUBID>");

                // Doc files
                int fileCnt = 1;
                foreach (ImageInfoClass imageInfo in document.ImageInfo)
                {
                    w.Write(String.Format("<FILE NO='{0}'>", fileCnt++));

                    // Image name
                    w.Write("<NAME>");
                    w.Write(imageInfo.ImageName);
                    w.Write("</NAME>");

                    // Doc Metadata
                    w.Write("<METADATA>");
                    w.Write("<CERTIFICATENO>");
                    w.Write(imageInfo.CertificateNumber);
                    w.Write("</CERTIFICATENO>");
                    w.Write("<CERTIFICATEDATE>");
                    w.Write(imageInfo.CertificateDate.Equals(DateTime.Parse(Constants.WebServiceNullDate)) ? string.Empty : imageInfo.CertificateDate.ToString());
                    w.Write("</CERTIFICATEDATE>");
                    
                    w.Write("<LOCALFOREIGN>");
                    w.Write(imageInfo.LocalForeign);
                    w.Write("</LOCALFOREIGN>");

                    w.Write("<MARRIAGETYPE>");
                    w.Write(imageInfo.MarriageType);
                    w.Write("</MARRIAGETYPE>");
                    //2012-12-11
                    w.Write("<ISVERIFIED>");
                    w.Write(imageInfo.IsVerified);
                    w.Write("</ISVERIFIED>");
                    


                    w.Write("</METADATA>");

                    w.Write("</FILE>");
                }

                w.Write("</DOC>");
            }

            w.Write("</CUSTOMER>");
        }

        w.Write("</SET>");
        w.Write("</ROOT>");
        w.Flush();
        w.Close();
        //root
        // set
        //  refno
        //  customer
        //   nric
        //   doc
        //    docid
        //    docsubid
        //     file
        //      name
        //      metadata
        //       certificateno
        //       certificatedate
        //       isforeign
        //       ismuslim 
    }

    //private string CreateCustomerInfo(string nric, DocumentClass document, out bool hasDocId)
    //{
    //    hasDocId = !String.IsNullOrEmpty(document.DocId);

    //    StringBuilder contents = new StringBuilder();

    //    // Customers
    //    contents.Append("<CUSTOMER>");

    //    contents.Append("<NRIC>");
    //    contents.Append(nric);
    //    contents.Append("</NRIC>");

    //    contents.Append("<DOC>");

    //    // Doc Id and SubId
    //    contents.Append("<DOCID>");
    //    contents.Append(document.DocId);
    //    contents.Append("</DOCID>");
    //    contents.Append("<DOCSUBID>");
    //    contents.Append(document.DocIdSub);
    //    contents.Append("</DOCSUBID>");

    //    // Doc files
    //    foreach (ImageInfoClass imageInfo in document.ImageInfo)
    //    {
    //        contents.Append("<FILE>");

    //        // Image name
    //        contents.Append("<NAME>");
    //        contents.Append(imageInfo.ImageName);
    //        contents.Append("</NAME>");

    //        // Doc Metadata
    //        contents.Append("<METADATA>");
    //        contents.Append("<CERTIFICATENO>");
    //        contents.Append(imageInfo.CertificateNumber);
    //        contents.Append("</CERTIFICATENO>");
    //        contents.Append("<CERTIFICATEDATE>");
    //        contents.Append(imageInfo.CertificateDate.Equals(DateTime.Parse(Constants.WebServiceNullDate)) ? string.Empty : imageInfo.CertificateDate.ToString());
    //        contents.Append("</CERTIFICATEDATE>");
    //        contents.Append("<ISFOREIGN>");
    //        contents.Append(imageInfo.IsForeign);
    //        contents.Append("</ISFOREIGN>");
    //        contents.Append("<ISMUSLIM>");
    //        contents.Append(imageInfo.IsMuslim);
    //        contents.Append("</ISMUSLIM>");

    //        //2012-12-11
    //        contents.Append("<ISVERIFIED>");
    //        contents.Append(imageInfo.IsVerified);
    //        contents.Append("</ISVERIFIED>");

    //        contents.Append("</METADATA>");

    //        contents.Append("</FILE>");
    //    }

    //    contents.Append("</DOC>");
    //    contents.Append("</CUSTOMER>");

    //    return contents.ToString();
    //}




    //2012-12-17,
    // If there are multiple ImageInfoClass,
    //each Image info is created with a new <Customer></Customer>
    private string CreateCustomerInfo(string customerIdFromSource, string name, string nric, string identityType, DocumentClass document, out bool hasDocId, string customerType)
    {
        hasDocId = !String.IsNullOrEmpty(document.DocId);

        StringBuilder contents = new StringBuilder();

        //int fileCount = 1;
        string allFileName = string.Empty;
        string fileName = string.Empty;

        // Doc files
        foreach (ImageInfoClass imageInfo in document.ImageInfo)
        {
            //if (allFileName.Contains(imageInfo.ImageName + ";"))
            //{
            //    fileName = Path.GetFileNameWithoutExtension(imageInfo.ImageName) + fileCount + Path.GetExtension(imageInfo.ImageName);
            //}
            //else
            //{
                fileName = imageInfo.ImageName;
                allFileName += imageInfo.ImageName + ";";
            //}

            // Customers
            contents.Append("<CUSTOMER>");

            //CustomerIdFromSource: 
            contents.Append("<CUSTOMERIDFROMSOURCE>");
            contents.Append(customerIdFromSource);
            contents.Append("</CUSTOMERIDFROMSOURCE>");

            contents.Append("<CUSTNAME>");
            contents.Append(name);
            contents.Append("</CUSTNAME>");

            contents.Append("<NRIC>");
            contents.Append(nric);
            contents.Append("</NRIC>");

            //IdentityType: 
            contents.Append("<IDENTITYTYPE>");
            contents.Append(identityType);
            contents.Append("</IDENTITYTYPE>");

            //CustomerType
            contents.Append("<CUSTOMERTYPE>");
            contents.Append(customerType);
            contents.Append("</CUSTOMERTYPE>");

            contents.Append("<DOC>");

          
            contents.Append("<DOCID>");
            contents.Append(document.DocId);
            contents.Append("</DOCID>");
            
            contents.Append("<DOCSUBID>");
            contents.Append(document.DocIdSub);
            contents.Append("</DOCSUBID>");

            contents.Append("<IDENTITYNOSUB>");
            contents.Append(document.IdentityNoSub);
            contents.Append("</IDENTITYNOSUB>");
          
            contents.Append("<CUSTOMERIDSUBFROMSOURCE>");
            contents.Append(document.CustomerIdSubFromSource);
            contents.Append("</CUSTOMERIDSUBFROMSOURCE>");

            contents.Append("<DOCDESCRIPTION>");
            contents.Append(document.DocDescription.Length > 50 ? document.DocDescription.Substring(0, 50) : document.DocDescription);
            contents.Append("</DOCDESCRIPTION>");

            contents.Append("<DOCSTARTDATE>");
            contents.Append(document.DocStartDate == null ? string.Empty : document.DocStartDate.ToString());
            contents.Append("</DOCSTARTDATE>");

            contents.Append("<DOCENDDATE>");
            contents.Append(document.DocEndDate == null ? string.Empty : document.DocEndDate.ToString());
            contents.Append("</DOCENDDATE>");

            contents.Append("<FILE>");

            // Image name
            contents.Append("<NAME>");
            contents.Append(fileName);
            contents.Append("</NAME>");

            // Doc Metadata
            contents.Append("<METADATA>");

           
            contents.Append("<DOCCHANNEL>");
            contents.Append(imageInfo.DocChannel);
            contents.Append("</DOCCHANNEL>");
           
            contents.Append("<CMDOCUMENTID>");
            contents.Append(imageInfo.CmDocumentId);
            contents.Append("</CMDOCUMENTID>");

            contents.Append("<ISACCEPTED>");
            contents.Append(imageInfo.IsAccepted);
            contents.Append("</ISACCEPTED>");

            contents.Append("<CERTIFICATENO>");
            contents.Append(imageInfo.CertificateNumber);
            contents.Append("</CERTIFICATENO>");
            
            contents.Append("<CERTIFICATEDATE>");
            contents.Append(imageInfo.CertificateDate.Equals(DateTime.Parse(Constants.WebServiceNullDate)) ? string.Empty : imageInfo.CertificateDate.ToString());
            contents.Append("</CERTIFICATEDATE>");

            contents.Append("<LOCALFOREIGN>");
            contents.Append(imageInfo.LocalForeign);
            contents.Append("</LOCALFOREIGN>");

            contents.Append("<MARRIAGETYPE>");
            contents.Append(imageInfo.MarriageType);
            contents.Append("</MARRIAGETYPE>");

            contents.Append("<ISVERIFIED>");
            contents.Append(imageInfo.IsVerified);
            contents.Append("</ISVERIFIED>");

            contents.Append("</METADATA>");

            contents.Append("</FILE>");


            contents.Append("</DOC>");
            contents.Append("</CUSTOMER>");

            //fileCount++;
        }
        return contents.ToString();
    }



    private void WriteToLogFile(string logFilePath, string contents)
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

    private void UpdatePersonalInfo(string nric, string refNo, string customerIdFromSource, string customerType, string identityType)
    {
        AppPersonalDb appPersonalDb = new AppPersonalDb();
        DocPersonalDb docPersonalDb = new DocPersonalDb();

        // Update the AppPersonal data for the personal
        appPersonalDb.UpdateFromCdbWebService(refNo, nric, customerIdFromSource, customerType, identityType);

        // Update the DocPersonal data for the personal
        docPersonalDb.UpdateFromCdbWebService(nric, customerIdFromSource, customerType, identityType);
    }




    public class AcknowledgementInfoClass
    {
        public string AcknowledgementNo;
        public DateTime AcknowledgementDate;
        public string ApplicantIdentityNo;
    }




    public class CustomerClass
    {
        public string CustomerIdFromSource;
        public string IdentityNo;
        public string IdentityType;
        public string CustomerName;
        public int DocCounter;
        public string CustomerType;
        public DocumentClass[] Documents;
    }





    public class DocumentClass
    {
        public string DocId; // If this has a value, categorization for the set is no longer needed (no DocId and DocIdSub means unverified)
        public string DocIdSub; // If DocId has a value and/or this has a value, categorization for the set is no longer needed
        public string DocDescription;
        public DateTime DocStartDate;
        public DateTime DocEndDate;
        public string IdentityNoSub;
        public string CustomerIdSubFromSource;

        //2012-12-20 Commented out
        //public string DocChannel; // Use 'MIXED' if there are more than one (1) channels found

        public string RequesterNickname;
        public DateTime DateReceived;
        public ImageInfoClass[] ImageInfo;
        public BusinessInfoClass[] BusinessInfoSet;
    }



    public class ImageInfoClass
    {
        public string ImageUrl;
        public string ImageName;
        public decimal ImageSize;
        public DateTime DateReceivedFromSource;

        public string DocChannel; // Use 'MIXED' if there are more than one (1) channels found


        // Only for verified documents
        public string CmDocumentId; // If this has a value, then the documents passed have been verified
        public bool IsAccepted;
        public bool IsMatchedWithExternalOrg;
        public DateTime DateFiled;

        // Metadata for verified documents
        public string CertificateNumber;
        public DateTime CertificateDate;
        //public bool IsForeign;
        //public bool IsMuslim;
        public bool IsVerified;

        //2013-03-06
        public string LocalForeign;
        public string MarriageType;


        public PersonIdentityClass[] PersonIdentityInfoSet;

    }

    public class PersonIdentityClass
    {
        public string customerIdFromSource;
        public string identityNo;
        public string identityType;
        public string customerName;
        public PersonInfoClass[] PersonInfoSet;


    }

    public class PersonInfoClass
    {
        public string identityNo;
        public string identityType;
        public string customerName;
    }




    public class BusinessInfoClass
    {
        public string BusinessTransactionNumber;
        public string BusinessRefNumber;
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

        return authentication.UserName == ConfigurationSettings.AppSettings["WebServiceUserName"] &&
            authentication.Password == ConfigurationSettings.AppSettings["WebServicePassword"];

    }

}

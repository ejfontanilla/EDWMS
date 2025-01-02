using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.IO;
using Dwms.Web;
using CDBPendingDocWebRef;

/// <summary>
/// Summary description for CDBPendingDoc
/// 
/// To send information to CDB
/// 
/// </summary>


namespace Dwms.Bll
{

    public class CDBPendingDoc
    {
        public CDBPendingDoc()
        {
        }

      

        /*
         *  Feel free to rename the method
         *  
         * it can be called other classes or areas in code eg.
         * 
         *  CDBPendingDoc cdbPendingDoc = new CDBPendingDoc();
         *  cdbPendingDoc.ProcessPendingDocs();
         * 
         * 
         */

        // Triggering Method
        public void ProcessPendingDocs(string RefNumber, string endStatus, string documentStatus)
        {
            BE01BCstmrDocRQSTService customerDocRequestService = new BE01BCstmrDocRQSTService();
            BE01JAuthenticationDTO authentication = new BE01JAuthenticationDTO();
            BE01JEnquiryInput input = new BE01JEnquiryInput();
            BE01JEnquiryOutput output1 = new BE01JEnquiryOutput();
            BE01JEnquiryResult result = new BE01JEnquiryResult();
            LeasInterfaceDb leasInterfaceDb = new LeasInterfaceDb();
            DocTypeDb docTypeDb = new DocTypeDb();
            HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
            //HleInterfaceDataTable hleInterfaceDataTable = new HleInterfaceDataTable();
            string filePath = string.Empty;
            int attemptCount = 0;
            try
            {
                // declare the web reference
                for (attemptCount = 1; attemptCount < 3; attemptCount++)
                {
                    authentication.userName = "CDBDWMSPhase3";
                    authentication.passWord = "CDBDWMSINTERFACE";
                    input.bsnsTxNumber = "HLE";
                    input.bsnsRefNumber = RefNumber;
                    input.endStatus = endStatus;
                    input.documentStatus = documentStatus;
                    result = customerDocRequestService.enquireCstmrDocRQST(authentication, input);

                    #region Insert the Pending Documents
                    try
                    {
                        if (result.resultFlag)
                        {
                            //string errorMessage = String.Format("GenerateXmlOutput() Message={0}, StackTrace={1}", ex.Message, ex.StackTrace);
                            leasInterfaceDb.Delete(RefNumber);
                            foreach (BE01JEnquiryOutput output in result.outputList)
                            {
                                //output = result.outputList.Count();
                                HleInterface.HleInterfaceDataTable hleTable = hleInterfaceDb.GetHleInterfaceByRefNoAndCustomerId(output.bsnsRefNumber, output.customerIdFromSource);
                                //convert custid to nric
                                if (hleTable.Count > 0)
                                {
                                    string Nric = hleTable[0].Nric;
                                    string docTypeCode = docTypeDb.GetDocTypeCodeByDocumentIdAndDocumentSubId(output.docId, output.docIdSub);
                                    leasInterfaceDb.Insert(output.bsnsRefNumber, Nric, docTypeCode, output.docStartDate, output.docEndDate, true, true,
                                        string.Empty, string.Empty, output.bsnsTxCategory, output.bsnsTxCategoryGroup, output.bsnsTxCategoryDate, output.endDocStatus);
                                }
                            }
                        }
                        filePath = GetFilePathAndName(input, attemptCount);
                        if (!result.resultFlag)
                            filePath = filePath + "Err";

                        ErrorLogDb errorLogDb = new ErrorLogDb();
                        //errorLogDb.Insert(ErrorLogFunctionName.PendingDoc, "Pend doc update");    Commented by Edward to Reduce Error Notifications 2015/8/24
                        if (GenerateXmlOutput(filePath, result, input))
                            break;
                    }
                    catch (Exception ex)
                    {
                        ErrorLogDb errorLogDb = new ErrorLogDb();
                        //string errorMessage = String.Format("GenerateXmlOutput() Message={0}, StackTrace={1}", ex.Message, ex.StackTrace);
                        errorLogDb.Insert(ErrorLogFunctionName.PendingDoc, ex.Message + "---1InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
                        //filePath = filePath + "Err";
                        //GenerateXmlOutput(filePath, result, input);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                //string errorMessage = String.Format("GenerateXmlOutput() Message={0}, StackTrace={1}", ex.Message, ex.StackTrace);
                errorLogDb.Insert(ErrorLogFunctionName.PendingDoc, ex.Message + "---2InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
                //filePath = filePath + "Err";
                //GenerateXmlOutput(filePath, result, input);
            }
        }


        private string GetFilePathAndName(BE01JEnquiryInput input, int attemptCount)
        {
            #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
            //return Path.Combine(Util.GetWebServiceDocsFolder(), string.Format("PenDoc-{0}-{1}_{2}", input.bsnsRefNumber.ToUpper(), Format.FormatDateTime(DateTime.Now, DateTimeFormat.yyyyMMdd_dash_HHmmss), attemptCount));
            //return Path.Combine(Util.GetLogFilesFolder(), string.Format("PenDoc-{0}-{1}_{2}", input.bsnsRefNumber.ToUpper(), Format.FormatDateTime(DateTime.Now, DateTimeFormat.yyyyMMdd_dash_HHmmss), attemptCount));
            return Util.GetLogFilesFolder("PENDOC", input.bsnsRefNumber.ToUpper(), attemptCount.ToString());
            #endregion
        }

        protected bool GenerateXmlOutput(string filePath, BE01JEnquiryResult result, BE01JEnquiryInput input)
        {
            try
            {
                CDBPendingDocs CDBPendingDoc = new CDBPendingDocs();
                CDBPendingDoc.BE01JEnquiryInput = input;
                CDBPendingDoc.BE01JEnquiryResult = result;


                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(CDBPendingDocs));

                //Util.CDBDetailLog(string.Empty, String.Format("Start writing XML file: " + filePath + ".xml"), EventLogEntryType.Information);

                System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".xml");
                writer.Serialize(file, CDBPendingDoc);

                //Util.CDBDetailLog(string.Empty, String.Format("End writing XML file"), EventLogEntryType.Information);

                return true;
            }
            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                //string errorMessage = String.Format("GenerateXmlOutput() Message={0}, StackTrace={1}", ex.Message, ex.StackTrace);
                errorLogDb.Insert(ErrorLogFunctionName.PendingDoc, ex.Message + "---InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
                //Util.CDBLog("DWMS_CDB_Service.GenerateXmlOutput()", errorMessage, EventLogEntryType.Error);
                return false;
            }
        }

        //Maybe write private methods here for the moment and later move to alternate location where suitable
        private void GetData()
        { 
        
        }

    }
     public class CDBPendingDocs
    {
        public BE01JEnquiryInput BE01JEnquiryInput;
        public BE01JEnquiryResult BE01JEnquiryResult;
    }
}
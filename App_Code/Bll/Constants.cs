using System;
using System.Collections.Generic;

namespace Dwms.Bll
{    
    public static class Constants
    {
        public static readonly int WindowHeight = 400;
        public static readonly int WindowWidth = 700;

        public static readonly string UnableToDeleteRoleErrorMessage = "We are unable to delete the role because it is being referenced by a user account.  Please make sure no reference to this role exists before deleting.";
        public static readonly string UnableToCreateUpdateErrorMessage = "We are unable to create/update the role.  Please try again later.";
        public static readonly string UnableToDeleteOperationErrorMessage = "We are unable to delete the Operation because it is being referenced by a user account.  Please make sure no reference to this operation exists before deleting.";
        public static readonly string UnableToDeleteGroupErrorMessage = "We are unable to delete the Group because it is being referenced by a Operation.  Please make sure no reference to this Group exists before deleting.";
        public static readonly string UnableToDeleteUserErrorMessage = "We are unable to delete the user because it is being referenced in the system (example: Application OIC, Imported OIC etc).\\nPlease make sure no reference to this user exists before deleting.";
        public static readonly string MetaDataRequiredFieldErrorMessage = "Value for the meta field is required.";
        public static readonly string UnableToConfirmSet = "To confirm set,\\n1. Please clear all the documents inside the unidentified folder.\\n2. Make sure there are no unidentified documents under the application folders.\\n3. Confirm all the documents in application folders.";
        public static readonly string UnableToConfirmApplication = "To confirm application,\\n1. Please make sure all the documents have been confirmed.\\n2. Make sure all the sets are verified.";
        public static readonly string UnableToSplit = "Unable to process the split action as some of the pages are already split.\\nPlease click OK to view the updated document pages.";
        public static readonly string UnableToRoute = "To route the entire set, please verify all the documents in the set.";

        public static readonly string ProblemLoadingPages = "{0} page{1} in this document {2} missing.\\n{3} page{4} could be corrupted.\\nPlease try re-categorizing the set.";

        public static readonly string InvalidDateFormat = "Date was in incorrect format";
        public static readonly string InvalidDateFormatInYear = "Year must be between 1901 to 2050";

        public static readonly string ViewDocuments = "View Documents";

        public static readonly string BarcodePrinted = "Barcode is printed.";
        public static readonly string BarcodePrintedError = "Barcode is not printed. There is an error while printing.";
        public static readonly string BarcodeObjectInstantiationError = "Error while instantiating barcode object.";
        
        public static readonly string AndSign = "&&";
        public static readonly string OrSign = "||";

        public static readonly string ExcelExportCellStyle = "1px #c2c5d3 solid";

        public static readonly string StreetListExcelFileName = "StreetListTemplate.xls";

        public static readonly string RulesProcessingActionStop = "Stop processing other rules on match.";
        public static readonly string RulesProcessingActionContinue = "Continue processing other rules even on matches.";
        public static readonly string KeywordSeperator = "_&&_";
        public static readonly string ContainsFormat = "Contains({0})";
        public static readonly string NotContainsFormat = "NotContains({0})";

        public static readonly string SessionFolderNameVariable = "[Session_Folder_Name]";

        public static readonly string SetNumberFormat = "{0}{1}{2}-{3}"; // Where {0}=Group Code; {1}=Operation Code; {2}=Date In and {3}=Sequential Number

        public static readonly string UnathorizedAccessErrorMessage = "Unauthorized: Access is denied due to invalid credentials";
        public static readonly string UnathorizedPageAccessErrorMessage = "Unauthorized: You do not have access rights to view this page.";
        public static readonly string UnathorizedEmptyAccessErrorMessage = "Unauthorized: You do not have access rights to any of the modules. <br>Please approach your administrator to obtain access rights.";

        public static readonly string SetVerified = "Set was verified.";
        public static readonly string SetNotVerified = "Set has not been verified.";
        public static readonly string SetClosed = "Set has been closed.";

        public static readonly string RefVerified = "Application has been checked.";
        public static readonly string RefNotVerified = "Application has not been checked.";
        public static readonly string RefClosed = "Application has been closed.";        

        public static readonly string MarkedUrgentSet = "Marked as urgent for processing";
        public static readonly string MarkedUrgentAndSkipCategoriztion = "Marked as urgent for processing and skip categorization";
        public static readonly string NotMarkedUrgentSet = "Not marked as urgent for processing";

        public static readonly string DocumentVerified = "Confirmed";
        public static readonly string DocumentNotVerified = "Pending Confirmation";

        public static readonly string HasFileNotVerified = "There are some files that have not been viewed for this application.";

        public static readonly string HasSetsNotVerifiedInApplication = "There are some sets that have not been verified for this application.";

        //public static readonly string HasPendingDocInSet = "The set has no outstanding documents.";
        public static readonly string HasPendingDocInApplication = "The application has no outstanding documents.";
        public static readonly string isSecondCA = "This application is second CA.";

        public static readonly string HleNumberRefPrefix = "Ref";

        public static readonly char[] OcrTextLineSeperators = new char[] { '\r', '\n', ':', ' ', ',' };
        public static readonly char[] NewLineSeperators = new char[] { '\r', '\n' };

        public static readonly string MetadataHeader = "Metadata";

        public static readonly string TagMandatoryMsg = "* - Mandatory when Tag is Local";
        public static readonly string MandatoryMsg = "* - Mandatory when document is Local";

        public static readonly string WebServiceNullDate = "0001-01-01";

        public static readonly string AADContactEmail1 = "ylh3@ABCDE.com.ph";
        public static readonly string AADContactEmail2 = "tjm1@ABCDE.com.ph";

        public static readonly string PRDContactEmail1 = "psp1@ABCDE.com.ph";

        public static readonly string RSDContactEmail1 = "ckh4@ABCDE.com.ph";
        public static readonly string RSDContactEmail2 = "cwl1@ABCDE.com.ph";

        public static readonly string SSDContactEmail1 = "wgt1@ABCDE.com.ph";
        //Added By Edward 14.11.2013 When Confirm is clicked in Income Extraction
        public static readonly string RefExtracted = "Application has been extracted.";

        public static readonly string UnableToConfirmExtractApplication = "To confirm and extract application,\\n1. Please make sure all the documents have been confirmed.";
    }
}
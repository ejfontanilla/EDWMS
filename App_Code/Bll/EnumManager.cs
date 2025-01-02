using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public enum CdbDocChannelEnum
    {
        MyDoc,
        MyABCDEPage,
        Scan,
        Fax,
        Email,
        Deposit_Box,
        Hardcopy_Mail,
        Counter,
        Mixed,
        CDB, 
        MyABCDEPage_Common_Panel      //Added By Edward 30.10.2013 011 New Channel
    }

    public enum EmailTemplateVariablesEnum
    {
        Remark,
        SystemName,
        Source
    }

    public enum EmailTemplateCodeEnum
    {
        Main_Template,
        Exception_Log_Template
    }

    public enum leasInterfaceStatusEnum
    {
        PEN
    }

    public enum DepartmentCodeEnum
    {
        AAD,
        PRD,
        RSD,
        SSD
    }

    public enum HleStatusEnum
    {
        Approved,
        Cancelled,
        Complete_Pre_E_Check,
        Expired,
        KIV_CA,
        KIV_Pre_E,
        Pending_Cancellation,
        Pending_Pre_E,
        Pending_Rejection,
        Rejected,
        Route_To_CA_Officer,
    }
   
    public enum WebStringEnum
    {
        Empty
    }

    public enum ApplicantTypeEnum
    {
        Applicant,
        Occupier
    }

    public enum HleLanesEnum
    {
        A,
        B,
        C,
        D,  //Added by Edward 13/1/2014 ofr Batch Assignment Panel
        E,
        F,
        H,
        L,
        N,
        T,
        X
    }

    public enum PendingAssignmentReportCountEnum
    {
        _5
    }

    public enum ApplicationCancellationOptionEnum
    {
        CustomerRequest,
        Overdue,
        Others,
        None
    }

    public enum MasterListEnum
    {
        Scanning_Channels,
        Uploading_Channels,
        Image_Condition,
        Document_Condition,
        ExpediteReason,
        Teams
    }

    public enum DocFolderEnum
    {
        Blank, 
        Others, 
        Routed,
        Spam,
        Unidentified
    }

    public enum DownloadStatusEnum
    {
        Pending_Download, // docApp not downloaded by any user
        Downloaded, // docApp downloaded by a user
    }

    public enum CitizenshipEnum
    {
        Others, // 00
        Singapore_Citizen, // 10
        Singapore_Permanent_Resident, // 20
        Malaysian_Citizen, // 30

        Malaysian_Permanent_Resident,
        Unknown,
        Indonesia_Citizen, 
        Thailand_Citizen,
        India_Citizen,
        Sri_Lanka_Citizen,
        Bangladesh_Citizen,
        Philippines_Citizen,
        Taiwan_Citizen,
        Sabah_Citizen,
        Sarawak_Citizen,
        Special_Pass_Holder,
        Non_Citizen
    }

    public enum EmploymentStatusEnum
    {
        Unemployed, // LEAS 1 Resale 3
        Employed, // LEAS 3 Resale 1
        Self_Employed, // 5
        Employed_opa_Comission_sl_Incentive_Based_cpa_, // 7
        Odd_Job_sl_Part_Time_Worker, // 9
        Self_Employed_sl_Part_Time, //Resale 2
        Employed_without_monthly_CPF_contributions,  //Sales 4  //Added By Edward 13/3/2014 Sales and Resales Changes
        Employed_with_monthly_CPF_contributions,     //Sales 3   //Added By Edward 13/3/2014
        Retired_with_Pension                    //Leas and Sales 2 : Added By Edward Add New Employment Status for LEAS and SALES 2014/9/16
    }

    public enum MaritalStatusEnum
    {
        Single, // S
        Single_Orphan, // O
        Married, // M
        Divorced, // D
        Widowed, // W
        Seperated, // P
        Unknown
    }

    public enum RelationshipEnum
    {
        Self, // 00
        GrandParents, // 01
        Father, // 02
        Uncle_sl_Aunt, // 03
        Cousin, // 05
        Brother_sl_Sister, // 06
        Brother_sl_Sister_in_Law, // 07
        Nephew_sl_Niece, // 08
        Fiance_sl_Fiancee, // 09
        Mother, // 12
        Husband_sl_Wife, // 20
        GrandParents_in_Law, // 21
        Father_in_Law, // 22
        Mother_in_Law, // 32
        Adoptive_Father_sl_Mother, // 38
        Adoptive_Son_sl_Daughter, // 39
        Son_sl_Daughter, // 41
        Son_sl_Daughter_in_Law, // 42
        GrandChild, // 43
        Unrelated, // 47
        Requestor, // Used in save metadata function in viewset and viewapp
        Spouse, // Used in save metadata function in viewset and viewapp

        Spouse_Of_Uncle_sl_Aunt,
        Second_Wife,
        Parent,
        Parent_in_Law,
        Second_Mother,
        Uncle_sl_Aunt_in_Law,
        Nephew_sl_Niece_in_Law,
        Unrelated_Spinster,
        Spouse_Of_Grandchild,
        Great_Grandchild,
        Other_Relative,
        In_Laws,
        Unknown,
        Wife,        //Added By Edward 2014/08/21 Drag document to Person disappears
        Husband         //Added By Edward 2014/08/21 Drag document to Person disappears
    }

    public enum ResaleRelationshipEnum
    {
        Brother_sl_Sister_in_Law, // BI
        Brother_sl_Sister, // BS
        Cousin, // CO
        Daughter, // DA
        Ex_Husband, // EH
        Ex_Wife, // EW
        Father, // FA
        Father_in_Law, // FI
        Fiance_sl_Fiancee, // FN
        Grandparent_in_Law, // GI
        Grandparent, // GR
        Grandson_sl_Grandaughter, //GS
        Husband, // HU
        Mother, //MO
        Nephew_sl_Niece, // NN
        Others, // OT
        Senior_Citizen, // SC
        Self, // SE
        Son_sl_Daughter_in_Law, // SI
        Spouse_of_Joint_Lessee, // SJ
        Spouse_of_Lessee, // SL        
        Step_Mother_sl_Father, // SM
        Son, // SO        
        Second_Wife, // SW
        Uncle_sl_Aunt, // UA
        Wife, // WI
        Spouse_of_Seller, // SS
        Parent, // PR
        Child // CH
    }

    public enum AppStatusEnum
    {
        Pending_Documents, // When the application is imported from interface file
        Verified, // when Set is verified, the application is set to Verified.
        Pending_Completeness, // when user is assigned a application
        Completeness_In_Progress, // After user start to save or confirm any document in the application
        Completeness_Cancelled, // when a appplication is cancelled.
        Completeness_Checked, // when a appplication is confirmed.
        Closed // when an application is closed with no further action.
    }

    public enum AssessmentStatusEnum
    {
        Extraction_Cancelled, // when a appplication is cancelled.
        Completeness_Checked, // when a appplication is confirmed in the Completeness module.
        Completeness_Cancelled,
        Pending_Extraction, // after user clicks on the “Assess” link in the listing page to open an application for income assessment; or an application has been assigned to an assessment officer.
        Extraction_In_Progress, // after user start to save or confirm any document in the application
        Extracted, // after the “Send” button is clicked, DWMS will start sending the income information to LEAS. Users are still allowed to change the information at this stage.
        Closed, // after LEAS send “Approve/Cancelled/Rejected” status of the application to DWMS.
        Verified //This Status is used only for Sales and Resales
    }

    public enum CategorizationModeEnum
    {
        Relevance_Ranking,
        Keywords
    }

    public enum SendToCDBStatusEnum
    {
        NotReady,
        Ready,
        Sent,
        SentButFailed,
        ModifiedSetSentButFailed,
        ModifiedInCompleteness,
        NA
    }


    #region Added By Edward 24/02/2014 Add Icon and Action Log
    public enum SendToLEASStatusEnum
    {
        NotReady,
        Ready,
        Sent,
        SentButFailed,
        ModifiedSetSentButFailed,
        ModifiedInCompleteness,
        NA,
        FailedConnErr,
        FailedInpErr,
        FailedOutErr,
        FailedUnknown
    }
    #endregion

    public enum SetStatusEnum
    {
        Pending_Categorization, // Before being categorized
        Categorization_Failed, // Categorization has failed
        New, // After the set is categorized
        Pending_Verification, // After the set is assigned
        Verification_In_Progress, // After user start to save or confirm any document in the set
        Verified, // when set is confirmed.
        Closed // when a set is closed with no further action.
    }

    public enum DocStatusEnum
    {
        New, // initial state
        Pending_Verification,
        Verified, // when user click on confirm in verification module for a given document
        Completed // when user click on confirm in completeness module for a given document
    }

    public enum ImageAcceptedEnum
    {
        Yes,
        No,
        Unknown,
        NA
    }

    public enum AuthenticationModeEnum
    {
        Local,
        AD
    }

    public enum UserActiveStatusEnum
    {
        Active,
        Inactive
    }

    public enum IcNumberPrefixedEnum
    {
        S,
        T,
        F,
        G,
        X
    }

    public enum SectionBusinessCodeEnum
    {
        CO,
        RO,
        SO,
        SR
    }

    public enum ScanningTransactionTypeEnum
    {
        HLE,
        Sales,
        Resale,
        SERS
    }

    public enum ReferenceTypeEnum
    {
        HLE,
        RESALE,
        SALES,
        SERS,
        NRIC,
        Others
    }

    //public enum KeywordVariableEnum
    //{
    //    HLE_Number,
    //    NRIC
    //}

    public enum KeywordVariableEnum
    {
        HLE_Number,
        RESALE_Number,
        SALES_Number,
        SERS_Number,
        NRIC
    }

    public enum PersonalTypeEnum
    {
        OC,
        HA
    }

    public enum ResalePersonalTypeEnum
    {
        BU,
        OC,
        PR,
        CH,
        SP,
        SE,
        MISC
    }

    public enum PersonalTypeTableEnum
    {
        APPPERSONAL,
        DOCPERSONAL
    }

    public enum DocumentConditionEnum
    {
        NA,
        Amended,
        Duplicate
    }

    public enum BusinessTypeEnum
    {
        Partnership,
        Pte_Ltd,
        Sole_Proprietor
    }

    public enum LicenseofTradeBusinessTypeEnum
    {
        Hawker,
        Taxi
    }

    

    public enum CategorisationRuleProcessingActionEnum
    {
        Stop,
        Continue
    }

    public enum CategorisationRuleOpeatorEnum
    {
        or,
        and
    }

    public enum ParameterNameEnum
    {
        SenderName,
        SystemEmail,
        SystemName,
        OcrEngine,
        ArchiveAudit,
        BatchJobMailingList,
        TestMailingList,
        RedirectAllEmailsToTestMailingList,
        AuthenticationMode,
        MaximumThread,
        MaxSampleDocs,
        EnableErrorNotification,
        ErrorNotificationMailingList,
        MaximumOcrPages,
        MinimumAgeExternalFiles,
        MinimumAgeTempFiles,
        MinimumEnglishWordCount,
        MinimumEnglishWordPercentage,
        MinimumScore,
        MinimumWordLength,
        KeywordCheckScope,
        TopRankedSamplePages,
        OcrBinarize,
        OcrMorph,
        OcrBackgroundFactor,
        OcrForegroundFactor,
        OcrQuality,
        OcrDotMatrix,
        OcrDespeckle,
        #region Added By Edward 2014/12/09 Archiving Program
        ArchiveCOS,
        ArchiveResale,
        ArchiveSales,
        ArchiveSers,
        ArchiveServer,
        ArchiveTimeFormat,
        ArchiveTimeStart
        #endregion
    }

    public enum SplitTypeEnum
    {
        Group,
        Individual
    }

    public enum ArchiveAuditEnum
    {
        _1,
        _3,
        _6,
        _12,
        _24
    }

    #region Added By Edward on 2014/11/17 Archive Documents
    public enum ArchiveDocumentsEnum
    {      
        _48,
        _36,
        _29,
        _24,
        _18,
        _12,
        _6,
        _3
    }

    public enum FolderFilesEnum
    {
        _WebService,
        _ForOcr,
        _RawPage,        
    }
    #endregion

    public enum AccessControlSettingEnum
    {
        Assign,
        Verify,
        Delete,
        
        Scan,        
        Upload,
        Search_All_Department_Sets,
        Search_All_Department_Sets_Read_Only,
        Search_Own_Department_Sets,
        Search_Own_Department_Sets_Read_Only,
        Exception_Report,

        All_Sets,
        All_Sets_Read_Only,
        All_Apps,
        All_Apps_Read_Only,
        Pending_Assignment, // used both in verification and completeness
        Assigned_To_Me,
        Imported_By_Me,
        Batch_Assignment,
        Assignment_Report,

        Generate_Scanning_Overview_Report,
        Manage_All,
        Manage_Department,
        View_Only,

        Download,
        Change_Acceptance,   //June 10, 2013 Added by Edward use in verification only
        Confirm_All_Acceptance,   //04.11.2013    Added By Edward Confirm All Acceptance

        #region Added By Edward Development of Reports 2014/06/09 
        Error_Sending_to_CDB,
        OCR_and_Web_Service_Errors,
        Batch_Upload,
        Verification,
        Verification_per_Aging,
        Verification_per_OIC,
        #endregion
        Confirm_and_Extract, //Added by Edward 2014/09/18 Confirm and Extract
        Archival_Report, //Added by Edward 2015/02/05 For Archival

        #region Added By Edward Delete and Recategorize access controls 2016/01/25
        Delete_Set,
        Recategorize_Set
        #endregion
    }

    public enum ModuleNameEnum
    {
        Import,
        Verification,
        Completeness,
        //Income_Computation,
        Income_Assessment,
        Maintenance,
        Reports,
        Search,
        FileDoc
    }

    public enum RoleEnum
    {
        System_Administrator,
        System
    }

    public enum OperationTypeEnum
    {
        Insert,
        Update,
        Delete,
        Append,
        Overwrite
    }

    public enum TableNameEnum
    {
        AppDocRef,
        AppPersonal,
        aspnet_Applications,
        aspnet_Membership,
        aspnet_Paths,
        aspnet_PersonalizationAllUsers,
        aspnet_PersonalizationPerUser,
        aspnet_Profile,
        aspnet_Roles,
        aspnet_SchemaVersions,
        aspnet_Users,
        aspnet_UsersInRoles,
        aspnet_WebEvent_Events,
        AccessControl,
        AuditTrail,
        CategorisationRule,
        CategorisationRuleKeyword,
        ScanChannel,
        Department,
        Doc,
        DocApp,
        DocDetail,
        DocMaster,
        DocPersonal,
        DocSet,
        DocType,
        DocumentDetail,
        DocumentMaster,
        EmailTemplate,
        HleInterface,
        Interface,
        InterfaceSalary,
        InterfaceIncomeComputation,
        LogAction,
        MasterList,
        MasterListItem,
        MetaData,
        MetaField,
        Operation,
        Parameter,
        Personal,
        Profile,
        RawFile,
        RawPage,
        RawText,
        RelevanceRanking,
        ResaleInterface,
        RoleToDepartment,
        SalesInterface,
        Section,
        SersInterface,
        SetApp,
        SetDocRef,
        StopWord,
        Street,
        Unit,
        UploadChannel,
        UserGroup,
        SampleDoc,
        SamplePage,
        IncomeTemplateItems,         //Added by Edward 26.08.2013 for Income Extraction / Assessment
        IncomeTemplate,             //Added by Edward 11.08.2013 for Income Extraction / Assessment
        CreditAssessment,           //Added by Edward 10.10.2013 for Income Extraction / Assessment
    }

    public enum AuditTableNameEnum
    {
        #region Commented By Edward to Fix Audit Trail Filter Tables on 2015/06/29
        //AccessControl,
        //AuditTrail,
        //CategorisationRule,
        //CategorisationRuleKeyword,
        //ScanChannel,
        //DocDetail,
        //DocMaster,
        //DocType,
        //EmailTemplate,
        //Parameter,
        //Personal,
        //Profile,
        //RawText,
        //StopWord,
        //UploadChannel
        #endregion
        #region Added By Edward to Fix Audit Trail Filter Tables on 2015/06/29
        AppDocRef,
        AppPersonal,
        aspnet_Applications,
        aspnet_Membership,
        aspnet_Paths,
        aspnet_PersonalizationAllUsers,
        aspnet_PersonalizationPerUser,
        aspnet_Profile,
        aspnet_Roles,
        aspnet_SchemaVersions,
        aspnet_Users,
        aspnet_UsersInRoles,
        aspnet_WebEvent_Events,
        AccessControl,
        AuditTrail,
        CategorisationRule,
        CategorisationRuleKeyword,
        ScanChannel,
        Department,
        Doc,
        DocApp,
        DocDetail,
        DocMaster,
        DocPersonal,
        DocSet,
        DocType,
        DocumentDetail,
        DocumentMaster,
        EmailTemplate,
        HleInterface,
        Interface,
        InterfaceSalary,
        InterfaceIncomeComputation,
        LogAction,
        MasterList,
        MasterListItem,
        MetaData,
        MetaField,
        Operation,
        Parameter,
        Personal,
        Profile,
        RawFile,
        RawPage,
        RawText,
        RelevanceRanking,
        ResaleInterface,
        RoleToDepartment,
        SalesInterface,
        Section,
        SersInterface,
        SetApp,
        SetDocRef,
        StopWord,
        Street,
        Unit,
        UploadChannel,
        UserGroup,
        SampleDoc,
        SamplePage,
        IncomeTemplateItems,         
        IncomeTemplate,             
        CreditAssessment,           
        #endregion
    }

    public enum LogTypeEnum
    {
        D, // used for document log under verification
        S, // used for updates related to set/verification
        A, // used for updates related to application/completeness
        C, // used for document log under completeness
        E, // used for updates related to application in Income Extraction      Added by Edward 28/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
        Z, // used for updates for the Income Month in Zoning Page              Added by Edward 11/02/2014 Sales and Resales Changes
    }

    #region Added by Edward 28/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
    public enum IncomeMonthsSourceEnum
    {
        C, //Completeness when user clicks confirm button
        W, // from Leas, Resale, Sales Or Sers WebService       
    }
    #endregion

    public enum ErrorLogFunctionName
    {
        MergePDFDocument,
        UnableToOpenPDFDocument,
        PendingDoc
    }

    public enum LogActionEnum
    {
        Assigned_set_to_REPLACE1,
        Assigned_application_to_REPLACE1,
        Classified_REPLACE1_in_REPLACE2_as_REPLACE3,
        Confirmed_metadata,
        Confirmed_set,
        Confirmed_application,
        Confirmed_application_COLON_To_Cancel,
        Document_moved_from_REPLACE1_folder_to_REPLACE2_folder,
        Document_merged_from_REPLACE1_to_REPLACE2,
        Document_REPLACE1_metadata_name_changed_from_REPLACE2_to_REPLACE3,
        REPLACE1_from_REPLACE2_merged_with_REPLACE3_from_REPLACE4,
        REPLACE1_Recatogorized_the_set,
        Release_application_from_REPLACE1,
        Release_set_from_REPLACE1,
        REPLACE1_accepted_REPLACE2_at_REPLACE3,
        REPLACE1_rejected_REPLACE2_at_REPLACE3,
        REPLACE1_extracted_as_new_document_REPLACE2_from_REPLACE3,
        REPLACE1_change_section_from_REPLACE2_to_REPLACE3,
        REPLACE1, // used only for formatting
        REPLACE2, // used only for formatting
        REPLACE3, // used only for formatting
        REPLACE4, // used only for formatting
        Route_set,
        Route_document,
        Saved_metadata_as_draft,
        Set_closed,
        Application_closed,
        None, // used for null reference,
        REPLACE1_added_as_reference_number,
        REPLACE1_removed_as_reference_number,
        Thumbnail_Creation_Error,
        File_Error,
        REPLACE1_COLON_Message_EQUALSSIGN_Unable_to_create_thumbnail_for_the_file_PERIOD_SEMICOLON_File_EQUALSSIGN_REPLACE2,
        REPLACE1_COLON_Message_EQUALSSIGN_REPLACE2_SEMICOLON_File_EQUALSSIGN_REPLACE,
        REPLACE1_COLON_REPLACE2,
        #region Added BY Edward 24/02/2014 Add Icon and Action Log        
        Copied_Months_From_REPLACE2_To_REPLACE3, 
        Divide_Income_From_REPLACE2_For_REPLACE3,          //User Clicking Divide Button in Zoning Page
        Delete_Income_Items,           //User deletes income records in zoning page
        Set_Income_to_Blank_For_REPLACE2,        //User clicks set income to blank button
        Update_Version_Name_to_REPLACE2,    //Update version name
        Delete_Version,                     //Delete version name
        Save_Income_For_REPLACE2,                //Save button in zoning page
        Confirmed_Income_Extraction,                            //Clicked Confirm button
        Updated_Extraction_Period_of_REPLACE2,       //update assessment period
        Updated_Currency_to_REPLACE2_and_rate_to_REPLACE3,                        //Update Currency
        Resend_Application,
        Set_Income_To_Zero,
        Update_Credit_Assessment,
        #endregion
        #region Added by Edward on 2015/06/18 Add Log for deleting/recategorizing sets - Source: Meeting on 2015/06/17
        Delete_Set_REPLACE1_by_REPLACE2,
        Recategorize_Set_REPLACE1_by_REPLACE2,
        #endregion
    }

    //Added By Edward 14.11.2013 for Adding Top 10 Currencies in xe.com
    public enum CurrencyEnum
    {
        CPF,        //for CPF
        USD,        //US Dollar
        EUR,        //Euro
        GBP,        //British Pound
        INR,        //Indian Rupee
        AUD,        //Australian Dollar
        CAD,        //Canadian Dollar
        AED,        //Emirati Dirham
        MYR,        //Malaysian Ringgit
        CHF,        //Swiss Franc
        CNY,        //Chinese Yuan Renminbi
    }

    public sealed class EnumManager
    {
        /// <summary>
        /// Convert enum to datatable
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static DataTable EnumToDataTable(Type enumType)
        {
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();

                r["Text"] = item.Replace("_", " ");
                r["Value"] = item;

                table.Rows.Add(r);
            }

            return table;
        }

        /// <summary>
        /// Get the audit table names
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAuditTables()
        {
            Type enumType = typeof(AuditTableNameEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();
                AuditTableNameEnum name = (AuditTableNameEnum)Enum.Parse(typeof(AuditTableNameEnum), item, false);
                r["Value"] = item;
                r["Text"] = FormatAuditTableName(item);
                table.Rows.Add(r);
            }

            return table;
        }

        /// <summary>
        /// Format the audit table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string FormatAuditTableName(string tableName)
        {
            AuditTableNameEnum name = (AuditTableNameEnum)Enum.Parse(typeof(AuditTableNameEnum), tableName, false);
            return tableName;
        }

        /// <summary>
        /// Format the table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string FormatTableName(string tableName)
        {
            TableNameEnum name = (TableNameEnum)Enum.Parse(typeof(TableNameEnum), tableName, false);

            switch (name)
            {
                case TableNameEnum.aspnet_Applications:
                    tableName = "Applications";
                    break;
                case TableNameEnum.aspnet_Membership:
                    tableName = "Membership";
                    break;
                case TableNameEnum.aspnet_Paths:
                    tableName = "Paths";
                    break;
                case TableNameEnum.aspnet_PersonalizationAllUsers:
                    tableName = "PersonalizationAllUsers";
                    break;
                case TableNameEnum.aspnet_PersonalizationPerUser:
                    tableName = "PersonalizationPerUser";
                    break;
                case TableNameEnum.aspnet_Profile:
                    tableName = "Profile";
                    break;
                case TableNameEnum.aspnet_Roles:
                    tableName = "Roles";
                    break;
                case TableNameEnum.aspnet_SchemaVersions:
                    tableName = "SchemaVersions";
                    break;
                case TableNameEnum.aspnet_Users:
                    tableName = "Users";
                    break;
                case TableNameEnum.aspnet_UsersInRoles:
                    tableName = "UsersInRoles";
                    break;
                case TableNameEnum.aspnet_WebEvent_Events:
                    tableName = "WebEvent_Events";
                    break;
            }

            return tableName;
        }

        /// <summary>
        /// Get the operation type for the street table names
        /// </summary>
        /// <returns></returns>
        public static DataTable GetImportStreetOperationTypes()
        {
            Type enumType = typeof(OperationTypeEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("Append") || item.Equals("Overwrite"))
                {
                    DataRow r = table.NewRow();
                    AuditTableNameEnum name = (AuditTableNameEnum)Enum.Parse(typeof(AuditTableNameEnum), item, false);
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the access rights for the Scanning Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetImportAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("Scan") || item.Equals("Upload"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2014/06/09          Development of Reports (Error_Sending_to_CDB,Batch_Upload,OCR_and_Web_Service_Errors,Verification,Verification_per_Aging,Verification_per_OIC)
        //Edward                            2015/02/05          Added Archival Report
        //Edward                            2016/01/25          Edward Delete and Recategorize access controls 2016/01/25

        /// <summary>
        /// Get the access rights for the Search Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSearchAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("Search_All_Department_Sets") || item.Equals("Search_All_Department_Sets_Read_Only") || item.Equals("Search_Own_Department_Sets") 
                    || item.Equals("Search_Own_Department_Sets_Read_Only") || item.Equals("Exception_Report") || 
                    item.Equals("Error_Sending_to_CDB") || item.Equals("Batch_Upload") || item.Equals("OCR_and_Web_Service_Errors")
                    || item.Equals("Verification") || item.Equals("Verification_per_Aging") || item.Equals("Verification_per_OIC")
                    || item.Equals("Archival_Report")
                    //Added By Edward Delete and Recategorize access controls 2016/01/25    
                    || item.Equals("Delete_Set") || item.Equals("Recategorize_Set"))   
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        //******************Log Changes/Fixes*********************
        //Added/Modified By                 Date                Description
        //Edward                            2013/06/10          Added Change_Acceptance
        //Edward                            2014/04/30          Added Batch_Assignment
        /// <summary>
        /// Get the access rights for the Verification Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetVerificationAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("All_Sets") || item.Equals("All_Sets_Read_Only") || item.Equals("Pending_Assignment")
                    || item.Equals("Assigned_To_Me") || item.Equals("Imported_By_Me") || item.Equals("Assignment_Report")
                    || item.Equals("Change_Acceptance") || item.Equals("Batch_Assignment"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the access rights for the Completeness Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetCompletenessAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("All_Apps_Read_Only") || item.Equals("All_Apps") || item.Equals("Pending_Assignment")
                    || item.Equals("Assigned_To_Me") || item.Equals("Batch_Assignment") || item.Equals("Assignment_Report")
                    || item.Equals("Confirm_All_Acceptance") || item.Equals("Confirm_and_Extract")) //04.11.2013 Added By Edward Confirm All Acceptance, 2014/09/18 Confirm and Extract
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the access rights for the Income Computation Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetIncomeComputationAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("All_Apps_Read_Only") || item.Equals("All_Apps") || item.Equals("Pending_Assignment")
                    || item.Equals("Assigned_To_Me") || item.Equals("Assignment_Report") || item.Equals("Batch_Assignment"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the access rights for the Maintenance Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMaintenanceAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("Manage_All") || item.Equals("Manage_Department") || item.Equals("View_Only"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        ///  Get the access rights for the File Doc Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetFileDocAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("View_Only") || item.Equals("Download"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the access rights for the Reports Module
        /// </summary>
        /// <returns></returns>
        public static DataTable GetReportsAccessRights()
        {
            Type enumType = typeof(AccessControlSettingEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (item.Equals("Generate_Scanning_Overview_Report"))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the keyword variables
        /// </summary>
        /// <returns></returns>
        public static DataTable GetKeywordVariables()
        {
            Type enumType = typeof(KeywordVariableEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();

                string value = "[" + item + "]";

                r["Value"] = value;
                r["Text"] = value;
                table.Rows.Add(r);
            }

            return table;
        }

        /// <summary>
        /// Get the set status datatable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSetStatus()
        {
            Type enumType = typeof(SetStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (!item.Equals(SetStatusEnum.Pending_Categorization.ToString()))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Get the set status datatable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllSetStatus()
        {
            Type enumType = typeof(SetStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (!item.Equals(SetStatusEnum.Pending_Categorization.ToString()) && 
                    !item.Equals(SetStatusEnum.Categorization_Failed.ToString()))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        public static DataTable GetAppStatus()
        {
            Type enumType = typeof(AppStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                if (!item.Equals(AppStatusEnum.Pending_Documents.ToString()))
                {
                    DataRow r = table.NewRow();
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        public static DataTable GetAssessmentStatus()
        {
            Type enumType = typeof(AssessmentStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();
                r["Value"] = item;
                r["Text"] = item.Replace("_", " ");
                table.Rows.Add(r);
            }

            return table;
        }


        //Added By Edward 14.11.2013 for Income Extraction
        /// <summary>
        /// Get the Currencies using the CurrencyEnum
        /// </summary>
        /// <returns></returns>
        public static List<string>  GetCurrencies()
        {
            Type enumType = typeof(CurrencyEnum);
            List<string> listCurrencies = new List<string>();

            foreach (string item in Enum.GetNames(enumType))
            {
                listCurrencies.Add(item);
            }

            return listCurrencies;
        }

        /// <summary>
        /// Get the DownloadStatusEnum datatable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDownloadStatus()
        {
            Type enumType = typeof(DownloadStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();
                r["Value"] = item;
                r["Text"] = item.Replace("_", " ");
                table.Rows.Add(r);
            }

            return table;
        }

        /// <summary>
        /// Get HleStatus
        /// </summary>
        /// <returns></returns>
        public static DataTable GetHleStatus()
        {
            Type enumType = typeof(HleStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();
                r["Value"] = item;
                r["Text"] = item.Replace("_", " ");
                table.Rows.Add(r);
            }

            return table;
        }


        public static DataTable GetAssignedToMeSetStatus()
        {
            Type enumType = typeof(SetStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();

                if (!item.Equals(SetStatusEnum.New.ToString()) &&
                    !item.Equals(SetStatusEnum.Pending_Categorization.ToString()) &&
                    !item.Equals(SetStatusEnum.Categorization_Failed.ToString()))
                {
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        public static DataTable GetAssignedToMeAppStatus()
        {
            Type enumType = typeof(AppStatusEnum);
            DataTable table = new DataTable();
            DataColumn c;

            c = new DataColumn("Text");
            table.Columns.Add(c);

            c = new DataColumn("Value");
            table.Columns.Add(c);

            foreach (string item in Enum.GetNames(enumType))
            {
                DataRow r = table.NewRow();

                if (!item.Equals(AppStatusEnum.Pending_Documents.ToString()))
                {
                    r["Value"] = item;
                    r["Text"] = item.Replace("_", " ");
                    table.Rows.Add(r);
                }
            }

            return table;
        }

        /// <summary>
        /// Map the employment status code to the corresponding value for LEAS
        /// </summary>
        /// <param name="employmentStatusCode"></param>
        /// <returns></returns>
        public static string MapEmploymentStatus(string employmentStatusCode)
        {
            string result = string.Empty;

            switch (employmentStatusCode)
            {
                case "1":
                    result = EmploymentStatusEnum.Unemployed.ToString();
                    break;
                #region Added By Edward Add New Employment Status for LEAS and SALES 2014/9/16
                case "2":
                    result = EmploymentStatusEnum.Retired_with_Pension.ToString().Replace("_", "-");
                    break;
                #endregion
                case "3":
                    result = EmploymentStatusEnum.Employed.ToString();
                    break;
                case "5":
                    result = EmploymentStatusEnum.Self_Employed.ToString().Replace("_", "-");
                    break;
                case "7":
                    result = EmploymentStatusEnum.Employed_opa_Comission_sl_Incentive_Based_cpa_.ToString().Replace("_opa_", "(").Replace("_cpa_", ")").Replace("_sl_", "/").Replace("_", "-");
                    break;
                case "9":
                    result = EmploymentStatusEnum.Odd_Job_sl_Part_Time_Worker.ToString().Replace("_sl_", "/").Replace("_", "-");
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Map the employment status code to the corresponding value for Resale
        /// </summary>
        /// <param name="employmentStatusCode"></param>
        /// <returns></returns>
        public static string MapResaleEmploymentStatus(string employmentStatusCode)
        {
            string result = string.Empty;

            switch (employmentStatusCode)
            {
                case "1":
                    result = EmploymentStatusEnum.Employed.ToString();
                    break;
                case "2":
                    result = EmploymentStatusEnum.Self_Employed_sl_Part_Time.ToString().Replace("_sl_", "/").Replace("_", "-");
                    break;
                case "3":
                    result = EmploymentStatusEnum.Unemployed.ToString();
                    break;
                default:
                    break;
            }

            return result;
        }
        //Added By Edward 12/3/2014 Sales and Resale Changes
        public static string MapSalesEmploymentStatus(string employmentStatusCode)
        {
            string result = string.Empty;

            switch (employmentStatusCode)
            {
                case "1":
                    result = EmploymentStatusEnum.Unemployed.ToString();
                    break;
                #region Added By Edward Add New Employment Status for LEAS and SALES 2014/9/16
                case "2":
                    result = EmploymentStatusEnum.Retired_with_Pension.ToString().Replace("_", "-");
                    break;
                #endregion
                case "3":
                    result = EmploymentStatusEnum.Employed_with_monthly_CPF_contributions.ToString().Replace("_"," ");
                    break;
                case "4":
                    result = EmploymentStatusEnum.Employed_without_monthly_CPF_contributions.ToString().Replace("_"," ");
                    break;
                case "5":
                    result = EmploymentStatusEnum.Self_Employed.ToString().Replace("_"," ");
                    break;
                case "7":
                    result = EmploymentStatusEnum.Employed_opa_Comission_sl_Incentive_Based_cpa_.ToString().Replace("_opa_", "(").Replace("_cpa_", ")").Replace("_sl_", "/").Replace("_", "-");
                    break;
                case "9":
                    result = EmploymentStatusEnum.Odd_Job_sl_Part_Time_Worker.ToString().Replace("_sl_", "/").Replace("_", "-");
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Map the marital status code to the corresponding value
        /// </summary>
        /// <param name="employmentStatusCode"></param>
        /// <returns></returns>
        public static string MapMaritalStatus(string maritalStatusCode)
        {
            string result = string.Empty;

            switch (maritalStatusCode)
            {
                case "S":
                    result = MaritalStatusEnum.Single.ToString();
                    break;
                case "O":
                    result = MaritalStatusEnum.Single_Orphan.ToString().Replace("_", " ");
                    break;
                case "M":
                    result = MaritalStatusEnum.Married.ToString();
                    break;
                case "D":
                    result = MaritalStatusEnum.Divorced.ToString();
                    break;
                case "W":
                    result = MaritalStatusEnum.Widowed.ToString();
                    break;
                case "P":
                    result = MaritalStatusEnum.Seperated.ToString();
                    break;
                case "U":
                    result = MaritalStatusEnum.Unknown.ToString();
                    break;
                default:
                    break;
            }

            return result;
        }

        public static string MapSersMaritalStatus(string maritalStatusCode)
        {
            string result = string.Empty;

            switch (maritalStatusCode)
            {
                case "1":
                    result = MaritalStatusEnum.Single.ToString();
                    break;
                case "2":
                    result = MaritalStatusEnum.Married.ToString();
                    break;
                case "3":
                    result = MaritalStatusEnum.Widowed.ToString();
                    break;
                case "4":
                    result = MaritalStatusEnum.Divorced.ToString();
                    break;
                case "5":
                    result = MaritalStatusEnum.Seperated.ToString();
                    break;
                //case "U":
                //    result = MaritalStatusEnum.Unknown.ToString();
                //    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Map the relationship status code to the corresponding value
        /// </summary>
        /// <param name="employmentStatusCode"></param>
        /// <returns></returns>
        public static string MapRelationshipStatus(string relationshipStatusCode)
        {
            string result = string.Empty;

            switch (relationshipStatusCode)
            {
                case "0":
                case "00":
                    result = RelationshipEnum.Self.ToString();
                    break;
                case "01":
                case "11":
                    result = RelationshipEnum.GrandParents.ToString();
                    break;
                case "02":
                    result = RelationshipEnum.Father.ToString();
                    break;
                case "03":
                case "13":
                    result = RelationshipEnum.Uncle_sl_Aunt.ToString();
                    break;
                case "05":
                case "15":
                case "25":
                case "35":
                   result = RelationshipEnum.Cousin.ToString();
                    break;
                case "06":
                    result = RelationshipEnum.Brother_sl_Sister.ToString();
                    break;
                case "07":
                case "26":
                case "27":
                    result = RelationshipEnum.Brother_sl_Sister_in_Law.ToString();
                    break;
                case "08":
                    result = RelationshipEnum.Nephew_sl_Niece.ToString();
                    break;
                case "09":
                    result = RelationshipEnum.Fiance_sl_Fiancee.ToString();
                    break;
                case "12":
                    result = RelationshipEnum.Mother.ToString();
                    break;
                case "20":
                    result = RelationshipEnum.Husband_sl_Wife.ToString();
                    break;
                case "21":
                case "31":
                    result = RelationshipEnum.GrandParents_in_Law.ToString();
                    break;
                case "22":
                    result = RelationshipEnum.Father_in_Law.ToString();
                    break;
                case "32":
                    result = RelationshipEnum.Mother_in_Law.ToString();
                    break;
                case "38":
                    result = RelationshipEnum.Adoptive_Father_sl_Mother.ToString();
                    break;
                case "39":
                    result = RelationshipEnum.Adoptive_Son_sl_Daughter.ToString();
                    break;
                case "41":
                    result = RelationshipEnum.Son_sl_Daughter.ToString();
                    break;
                case "42":
                    result = RelationshipEnum.Son_sl_Daughter_in_Law.ToString();
                    break;
                case "43":
                    result = RelationshipEnum.GrandChild.ToString();
                    break;

                case "04":
                case "14":
                    result = RelationshipEnum.Spouse_Of_Uncle_sl_Aunt.ToString();
                    break;
                case "10":
                    result = RelationshipEnum.Second_Wife.ToString();
                    break;
                case "16":
                case "98":
                    result = RelationshipEnum.Parent.ToString();
                    break;
                case "18":
                    result = RelationshipEnum.Parent_in_Law.ToString();
                    break;
                case "19":
                    result = RelationshipEnum.Second_Mother.ToString();
                    break;
                case "23":
                case "24":
                case "33":
                case "34":
                    result = RelationshipEnum.Uncle_sl_Aunt_in_Law.ToString();
                    break;
                case "28":
                    result = RelationshipEnum.Nephew_sl_Niece_in_Law.ToString();
                    break;
                case "40":
                    result = RelationshipEnum.Unrelated_Spinster.ToString();
                    break;
                case "44":
                    result = RelationshipEnum.Spouse_Of_Grandchild.ToString();;
                    break;
                case "45":
                    result = RelationshipEnum.Great_Grandchild.ToString();
                    break;
                case "46":
                    result = RelationshipEnum.Other_Relative.ToString();
                    break;
                case "97":
                    result = RelationshipEnum.In_Laws.ToString();
                    break;
                case "99":
                    result = RelationshipEnum.Unknown.ToString();
                    break;
                case "47":
                case "48":
                case "49":
                    result = RelationshipEnum.Unrelated.ToString();
                    break;
                default:
                    break;
            }

            return result.Replace("_sl_", "/").Replace("_", " ");
        }

        /// <summary>
        /// Map the relationship status code to the corresponding value
        /// </summary>
        /// <param name="employmentStatusCode"></param>
        /// <returns></returns>
        public static string MapResaleRelationshipStatus(string relationshipStatusCode)
        {
            string result = string.Empty;

            switch (relationshipStatusCode)
            {
                case "BI":
                    result = ResaleRelationshipEnum.Brother_sl_Sister_in_Law.ToString();
                    break;
                case "BS":
                    result = ResaleRelationshipEnum.Brother_sl_Sister.ToString();
                    break;
                case "CO":
                    result = ResaleRelationshipEnum.Cousin.ToString();
                    break;
                case "DA":
                    result = ResaleRelationshipEnum.Daughter.ToString();
                    break;
                case "EH":
                    result = ResaleRelationshipEnum.Ex_Husband.ToString();
                    break;
                case "EW":
                    result = ResaleRelationshipEnum.Ex_Wife.ToString();
                    break;
                case "FA":
                    result = ResaleRelationshipEnum.Father.ToString();
                    break;
                case "FI":
                    result = ResaleRelationshipEnum.Father_in_Law.ToString();
                    break;
                case "FN":
                    result = ResaleRelationshipEnum.Fiance_sl_Fiancee.ToString();
                    break;
                case "GI":
                    result = ResaleRelationshipEnum.Grandparent_in_Law.ToString();
                    break;
                case "GR":
                    result = ResaleRelationshipEnum.Grandparent.ToString();
                    break;
                case "GS":
                    result = ResaleRelationshipEnum.Grandson_sl_Grandaughter.ToString();
                    break;
                case "HU":
                    result = ResaleRelationshipEnum.Husband.ToString();
                    break;
                case "MO":
                    result = ResaleRelationshipEnum.Mother.ToString();
                    break;
                case "NN":
                    result = ResaleRelationshipEnum.Nephew_sl_Niece.ToString();
                    break;
                case "OT":
                    result = ResaleRelationshipEnum.Others.ToString();
                    break;
                case "SC":
                    result = ResaleRelationshipEnum.Senior_Citizen.ToString();
                    break;
                case "SE":
                    result = ResaleRelationshipEnum.Self.ToString();
                    break;
                case "SI":
                    result = ResaleRelationshipEnum.Son_sl_Daughter_in_Law.ToString();
                    break;
                case "SJ":
                    result = ResaleRelationshipEnum.Spouse_of_Joint_Lessee.ToString();
                    break;
                case "SL":
                    result = ResaleRelationshipEnum.Spouse_of_Lessee.ToString();
                    break;
                case "SM":
                    result = ResaleRelationshipEnum.Step_Mother_sl_Father.ToString();
                    break;
                case "SO":
                    result = ResaleRelationshipEnum.Son.ToString();
                    break;
                case "SW":
                    result = ResaleRelationshipEnum.Second_Wife.ToString();
                    break;
                case "UA":
                    result = ResaleRelationshipEnum.Uncle_sl_Aunt.ToString();
                    break;
                case "WI":
                    result = ResaleRelationshipEnum.Wife.ToString();
                    break;
                case "PR":
                    result = ResaleRelationshipEnum.Parent.ToString();
                    break;
                case "CH":
                    result = ResaleRelationshipEnum.Child.ToString();
                    break;
                case "SS":
                    result = ResaleRelationshipEnum.Spouse_of_Seller.ToString();
                    break;
                default:
                    break;
            }

            return result.Replace("_sl_", "/").Replace("_", " ");
        }

        /// <summary>
        /// Map the citizenship status code to the corresponding value
        /// </summary>
        /// <param name="citizenshipCode"></param>
        /// <returns></returns>
        public static string MapCitizenship(string citizenshipCode)
        {
            string result = string.Empty;

            switch (citizenshipCode.ToUpper())
            {
                case "00":
                case "OC":
                case "OT":
                    result = CitizenshipEnum.Others.ToString();
                    break;
                case "10":
                case "SC":
                    result = CitizenshipEnum.Singapore_Citizen.ToString();
                    break;
                case "20":
                case "SP":
                case "PR":
                    result = CitizenshipEnum.Singapore_Permanent_Resident.ToString();
                    break;
                case "30":
                case "MC":
                    result = CitizenshipEnum.Malaysian_Citizen.ToString();
                    break;
                case "40":
                case "MP":
                    result = CitizenshipEnum.Malaysian_Permanent_Resident.ToString();
                    break;
                case "99":
                case "NI":
                    result = CitizenshipEnum.Unknown.ToString();
                    break;
                case "01":
                case "ID":
                    result = CitizenshipEnum.Indonesia_Citizen.ToString();
                    break;
                case "02":
                case "TH":
                    result = CitizenshipEnum.Thailand_Citizen.ToString();
                    break;
                case "03":
                case "IN":
                    result = CitizenshipEnum.India_Citizen.ToString();
                    break;
                case "04":
                case "SL":
                    result = CitizenshipEnum.Sri_Lanka_Citizen.ToString();
                    break;
                case "05":
                case "BD":
                    result = CitizenshipEnum.Bangladesh_Citizen.ToString();
                    break;
                case "06":
                case "PH":
                    result = CitizenshipEnum.Philippines_Citizen.ToString();
                    break;
                case "07":
                case "TN":
                    result = CitizenshipEnum.Taiwan_Citizen.ToString();
                    break;
                case "21":
                case "PS":
                    result = CitizenshipEnum.Special_Pass_Holder.ToString();
                    break;
                case "60":
                case "SB":
                    result = CitizenshipEnum.Sabah_Citizen.ToString();
                    break;
                case "80":
                case "SR":
                    result = CitizenshipEnum.Sarawak_Citizen.ToString();
                    break;
                case "NC":
                    result = CitizenshipEnum.Non_Citizen.ToString();
                    break;
                default:
                    break;
            }

            return result.Replace("_", " ");
        }
        
        /// <summary>
        /// Map the hle status code to the corresponding value
        /// </summary>
        /// <param name="hleStatusCode"></param>
        /// <returns></returns>
        public static string MapHleStatus(string hleStatusCode)
        {
            string result = string.Empty;

            switch (hleStatusCode.ToUpper())
            {
                case "AC":
                    result = HleStatusEnum.Approved.ToString();
                    break;
                case "REJ":
                    result = HleStatusEnum.Rejected.ToString();
                    break;
                case "CAN":
                    result = HleStatusEnum.Cancelled.ToString();
                    break;
                case "EXP":
                    result = HleStatusEnum.Expired.ToString();
                    break;
                case "PP":
                    result = HleStatusEnum.Pending_Pre_E.ToString();
                    break;
                case "KE":
                    result = HleStatusEnum.KIV_Pre_E.ToString();
                    break;
                case "PE":
                    result = HleStatusEnum.Route_To_CA_Officer.ToString();
                    break;
                case "AE":
                    result = HleStatusEnum.Complete_Pre_E_Check.ToString();
                    break;
                case "KC":
                    result = HleStatusEnum.KIV_CA.ToString();
                    break;
                case "PRJ":
                    result = HleStatusEnum.Pending_Rejection.ToString();
                    break;
                case "PCN":
                    result = HleStatusEnum.Pending_Cancellation.ToString();
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Map the CDB Doc Channel
        /// </summary>
        /// <param name="cdbDocChannel"></param>
        /// <returns></returns>
        public static string MapCdbDocChannel(string cdbDocChannel)
        {
            string result = string.Empty;

            switch (cdbDocChannel)
            {
                case "001":
                    result = CdbDocChannelEnum.MyDoc.ToString();
                    break;
                case "002":
                    result = CdbDocChannelEnum.MyABCDEPage.ToString();
                    break;
                case "003":
                    result = CdbDocChannelEnum.Scan.ToString();
                    break;
                case "004":
                    result = CdbDocChannelEnum.Fax.ToString();
                    break;
                case "005":
                    result = CdbDocChannelEnum.Email.ToString();
                    break;
                case "006":
                    result = CdbDocChannelEnum.Deposit_Box.ToString();
                    break;
                case "007":
                    result = CdbDocChannelEnum.Hardcopy_Mail.ToString();
                    break;
                case "008":
                    result = CdbDocChannelEnum.Counter.ToString();
                    break;
                case "009":
                    result = CdbDocChannelEnum.Mixed.ToString();
                    break;
                #region Added by Edward There was no channel in Docset 2017/09/26
                case "010":
                    result = CdbDocChannelEnum.CDB.ToString();
                    break;
                #endregion
                case "011": // Added By Edward 30.10.2013 011 New Channel
                    result = CdbDocChannelEnum.MyABCDEPage_Common_Panel.ToString();
                    break;
                default:
                    break;
            }

            return result.Replace("_", " ");
        }
    }

    #region DOCTYPE METADATA FIELD ENUM

    public enum DocTypeMetaDataBirthCertificateEnum
    {
        Tag
    }

    public enum DocTypeMetaDataBirthCertificatChildEnum
    {
        Tag,
        IDType,
        IdentityNo,
        NameOfChild
    }

    #region Added By Edward 2015/05/18 New Document Types
    public enum DocTypeMetaDataBirthCertSiblingEnum
    {
        Tag,
        IDType,
        IdentityNo,
        NameOfSibling
    }
    #endregion

    public enum DocTypeMetaDataAdoptionEnum
    {
        Tag,
        NameOfChild
    }

    public enum DocTypeMetaDataDeedPollEnum
    {
        DateOfDeedPoll
    }

    public enum DocTypeMetaDataPowerAttorneyEnum
    {
        DateOfFiling,
        IdentityNoDonor1,
        IdentityNoDonor2,
        IdentityNoDonor3,
        IdentityNoDonor4,
        IDTypeDonor1,
        IDTypeDonor2,
        IDTypeDonor3,
        IDTypeDonor4,
        NameDonor1,
        NameDonor2,
        NameDonor3,
        NameDonor4
    }

    public enum DocTypeMetaDataCBREnum
    {
        DateOfReport
    }

    #region Added by Edward 2015/05/18  New Document Types
    public enum DocTypeMetaDataDocEduInstituteEnum
    {
        StartDate,
        EndDate,
    }
    
    public enum DocTypeMetaDataDocOfFinCommitmentEnum
    {
        DateOfDocument
    }

    public enum DocTypeMetaDataOptionPurchaseEnum
    {
        DateOfOption
    }

    public enum DocTypeMetaDataNSEnlistmentNoticeEnum
    {
        DateOfFrom
    }

    public enum DocTypeMetaDataNSORDCertificateEnum
    {
        DateOfORD
    }

    public enum DocTypeMetaDataIdentityCardChildEnum
    {
        DateOfIssue,
        IDType,
        IdentityNo,
        NameOfChild
    }

    #region added by edward 3/30/2017 new document types
    public enum DocTypeMetaDataIdentityCardNRICEnum
    {
        DateOfIssue,
        IDType,
        IdentityNo,
        NameOfNRIC
    }
    #endregion

    #region Added by Edward 2017/12/22 New Document Types Identity Card Father Mother
    public enum DocTypeMetaDataIdentityCardFaEnum
    {
        DateOfIssue,
        IDType,
        IdentityNo,
        NameOfFather
    }

    public enum DocTypeMetaDataIdentityCardMoEnum
    {
        DateOfIssue,
        IDType,
        IdentityNo,
        NameOfMother
    }

    #endregion

    public enum DocTypeMetaDataNotesSyariahCourtEnum
    {
        DateOfDocument
    }

    public enum DocTypeMetaDataMedDocuDoctorLettersEnum
    {
        DateOfDocument
    }

    public enum DocTypeMetaDataOrderofCourtDivorceEnum
    {
        DateOfDocument,        
        IdentityNoRequestor,
        IdentityNoSpouse,
        IDTypeRequestor,
        IDTypeSpouse,
        NameOfRequestor,
        NameOfSpouse
    }
    #endregion

    public enum DocTypeMetaDataIdentityCardEnum
    {
        DateOfIssue,
        Address
    }

    public enum DocTypeMetaDataPassportEnum
    {
        DateOfExpiry
    }

    public enum DocTypeMetaDataStudentPassEnum
    {
        EducationLevel,
        DateOfIssue
    }

    public enum DocTypeMetaDataValueStudentPassEnum
    {
        Primary,
        Secondary,
        Tertiary
    }
    
    public enum DocTypeMetaDataSpouseFormPurchaseEnum
    {
        IDType,
        SpouseName,
        SpouseID
    }

    public enum DocTypeMetaDataPurchaseAgreementEnum
    {
        DateOfDocument,
    }
    

    public enum DocTypeMetaDataValuationReportEnum
    {
        DateOfReport,
    }

    public enum DocTypeMetaDataReceiptsLoanArrearEnum
    {
        ABCDERef,
        DateOfStatement
    }

    public enum DocTypeMetaDataRentalArrearsEnum
    {
        ABCDERef,
        DateOfStatement
    }

    public enum DocTypeMetaDataPetitionforGLAEnum
    {
        DateOfIssue,
        IdNo,
        IDType,
        NameDeceased
    }

    public enum DocTypeMetaDataOrderofCourtEnum
    {
        CourtOrderDate
    }

    public enum DocTypeMetaDataOfficialAssigneeEnum
    {
        BankruptcyNo
    }

    public enum DocTypeMetaDataBaptismEnum
    {
        DateOfBaptism
    }   
    

    public enum DocTypeMetaDataNoticeofTransferEnum
    {
        DateOfTransfer
    }

    public enum DocTypeMetaDataHleEnum
    {
        DateOfSignature
    }

    public enum DocTypeMetaDataLicenseofTradeEnum
    {
        StartDate,
        BusinessType
    }    

    public enum DocTypeMetaDataWarranttoActEnum
    {
        DateOfDocument
    }

    public enum DocTypeMetaDataLoanStatementSoldEnum
    {
        DateOfStatement
    }
    

    public enum DocTypeMetaDataCPFStatementRefundEnum
    {
        DateOfStatement,
        CR
    }

    public enum DocTypeMetaDataStatementSaleEnum
    {
        DateOfStatement
    }

    public enum DocTypeMetaDataPropertyQuestionaireEnum
    {
        DateOfDocument
    }

    #region Added by Edward 2017/10/03 New Document Types Property Tax
    public enum DocTypeMetaDataPropertyTaxEnum
    {                
        YearOfPropertyTax
    }
    public enum DocTypeMetaDataPropertyTaxNRICEnum
    {
        YearOfPropertyTax,
        IdentityNo,
        IDType,
        NameOfNRIC
    }
    #endregion


    public enum DocTypeMetaDataDeclaraIncomeDetailsEnum
    {
        DateOfDeclaration
    }

    public enum DocTypeMetaDataLettersolicitorPOAEnum
    {
        DateOfDocument
    }
    
    
    public enum DocTypeMetaDataCPFContributionEnum
    {
        StartDate,
        EndDate,
        ConsistentContribution,
        CompanyName1,
        CompanyName2
    }

    public enum DocTypeMetaDataMortgageLoanFormEnum
    {
        DateOfSignature
    }

    public enum DocTypeMetaDataStatutoryDeclarationEnum
    {
        StartDate,
        EndDate,
        DateOfDeclaration,
        Type
    }

    public enum DocTypeMetaDataStatutoryDeclGeneralEnum
    {
        DateOfDeclaration,
    }

    public enum DocTypeMetaDataDeclarationPrivPropEnum
    {
        DateOfDeclaration,
    }
    

    public enum DocTypeMetaDataPAYSLIPEnum
    {
        StartDate,
        EndDate,
        NameOfCompany,
        Allowance,
    }

    public enum DocTypeMetaDataEmploymentLetterEnum
    {
        StartDate,
        NameOfCompany,
        Allowance,
    }

    public enum DocTypeMetaDataCommissionStatementEnum
    {
        StartDate,
        EndDate,
        NameOfCompany,
    }

    public enum DocTypeMetaDataOverseasIncomeEnum
    {
        StartDate,
        EndDate,
    }

    public enum DocTypeMetaDataStatementOfAccountsEnum
    {
        StartDate,
        EndDate,
        NameOfCompany,
    }

    public enum DocTypeMetaDataPensionerLetterEnum
    {
        DateOfDocument
    }

    public enum DocTypeMetaDataBusinessProfileEnum
    {
        UENNo,
        DateOfRegistration,
        BusinessType
    }

    public enum DocTypeMetaDataBankStatementEnum
    {
        StartDate,
        EndDate,
    }    

    public enum DocTypeMetaDataValueStatutoryDeclarationEnum
    {
        Self_employed,
        Unemployed
    }

    public enum DocTypeMetaDataValueYesNoEnum
    {
        Yes,
        No
    }


    public enum DocTypeMetaDataValueCPFEnum
    {
        Yes,
        No
    }

    public enum DocTypeMetaDataCPFStatementEnum
    {
        StartDate,
        EndDate
    }

    public enum DocTypeMetaDataDeathCertificateEnum
    {
        IdentityNo,
        DateOfDeath,
        Tag
    }

    public enum DocTypeMetaDataDeathCertificateFaEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfFather,
        IdentityNoOfFather
    }

    public enum DocTypeMetaDataDeathCertificateMoEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfMother,
        IdentityNoOfMother
    }

    #region Added by Edward 2017/03/30 New Document Types
    public enum DocTypeMetaDataDeathCertificatePAEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfParent,
        IdentityNoOfParent
    }

    public enum DocTypeMetaDataDeathCertificateLOEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfLateOwner,
        IdentityNoOfLateOwner
    }
    #endregion

    public enum DocTypeMetaDataDeathCertificateSPEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfSpouse,
        IdentityNoOfSpouse
    }

    public enum DocTypeMetaDataDeathCertificateEXSPEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameOfEXSpouse,
        IdentityNoOfEXSpouse
    }

    public enum DocTypeMetaDataDeathCertificateNRICEnum
    {
        DateOfDeath,
        Tag,
        IDType,
        NameNRIC,
        IdentityNoNRIC
    }

    public enum DocTypeMetaDataNoLoanNotificationEnum
    {
        DateOfSignature,
        Type
    }

    public enum DocTypeMetaDataValueNoLoanNotificationEnum
    {
        Loan,
        Bank_Loan,
        No_Loan
    }

    public enum DocTypeMetaDataIRASAssesementEnum
    {
        DateOfFiling,
        TypeOfIncome,
        YearOfAssessment
    }

    public enum DocTypeMetaDataValueIRASAssesementEnum
    {
        Trade,
        Employment,
        Both
    }

    public enum DocTypeMetaDataIRASIR8EEnum
    {
        DateOfFiling,
        TypeOfIncome,
        YearOfAssessment
    }

    public enum DocTypeMetaDataValueIRASIR8EEnum
    {
        Trade,
        Employment,
        Both
    }

    public enum DocTypeMetaDataMarriageCertificateEnum
    {
        DateOfMarriage,
        MarriageCertNo,
        Tag,
        IdentityNoRequestor,
        IdentityNoImageRequestor,
        IdentityNoSpouse,
        IdentityNoImageSpouse,
        IDTypeRequestor,
        IDTypeImageRequestor,
        IDTypeSpouse,
        IDTypeImageSpouse,
        NameOfRequestor,
        NameOfImageRequestor,
        NameOfSpouse,
        NameOfImageSpouse,
    }

    public enum DocTypeMetaDataMarriageCertParentEnum
    {
        DateOfMarriage,
        MarriageCertNo,
        Tag,
        IdentityNoParent,
        IdentityNoSpouse,
        IDTypeParent,
        IDTypeSpouse,
        NameOfParent,
        NameOfSpouse
    }

    public enum DocTypeMetaDataMarriageCertLtSpouseEnum
    {
        DateOfMarriage,
        MarriageCertNo,
        Tag,
        IdentityNoRequestor,
        IdentityNoImageRequestor,
        IdentityNoLateSpouse,
        IDTypeRequestor,
        IDTypeImageRequestor,
        IDTypeLateSpouse,
        NameOfRequestor,
        NameOfLateSpouse
    }

    public enum DocTypeMetaDataMarriageCertChildEnum
    {
        DateOfMarriage,
        MarriageCertNo,
        Tag,
        IdentityNoChild,
        IdentityNoSpouse,
        IDTypeChild,
        IDTypeSpouse,
        NameOfChild,
        NameOfSpouse
    }

    #region added by edward 2015/05/18 New Document Types
    public enum DocTypeMetaDataMarriageCertSiblingEnum
    {
        DateOfMarriage,
        MarriageCertNo,
        Tag,
        IdentityNoSibling,
        IdentityNoSpouse,
        IDTypeSibling,
        IDTypeSpouse,
        NameOfSibling,
        NameOfSpouse
    }

    #endregion



    public enum DocTypeMetaDataDeedSeparationEnum
    {
        Tag,
        DateOfSeperation,
        IDTypeRequestor,
        IDTypeSpouse,
        IdentityNoRequestor,
        IdentityNoSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    #region #region Added by Edward 2017/03/30 New Document Types
    public enum DocTypeMetaDataDeedSeparationNRICEnum
    {
        Tag,
        DateOfSeperation,
        IDTypeNRIC,
        IDTypeSpouse,
        IdentityNoNRIC,
        IdentityNoSpouse,
        NameOfNRIC,
        NameOfSpouse
    }
    #endregion

    public enum DocTypeMetaDataDeedSeveranceEnum
    {
        Tag,
        DateOfSeverance,
        IDTypeRequestor,
        IDTypeSpouse,
        IdentityNoRequestor,
        IdentityNoSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceCertificateEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoRequestor,
        IdentityNoSpouse,
        IDTypeRequestor,
        IDTypeSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceCertFatherEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoFather,
        IdentityNoSpouse,
        IDTypeFather,
        IDTypeSpouse,
        NameOfFather,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceCertMotherEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoMother,
        IdentityNoSpouse,
        IDTypeMother,
        IDTypeSpouse,
        NameOfMother,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceCertExSpouseEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoRequestor,
        IdentityNoExSpouse,
        IDTypeRequestor,
        IDTypeExSpouse,
        NameOfRequestor,
        NameOfExSpouse
    }

    public enum DocTypeMetaDataDivorceCertChildEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoChild,
        IdentityNoSpouse,
        IDTypeChild,
        IDTypeSpouse,
        NameOfChild,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceCertNRICEnum
    {
        DateOfDivorce,
        DivorceCaseNo,
        Tag,
        IdentityNoNRIC,
        IdentityNoSpouse,
        IDTypeNRIC,
        IDTypeSpouse,
        NameOfNRIC,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceDocInitialEnum
    {
        DateOfOrder,
        Tag,
        IdentityNoRequestor,
        IdentityNoSpouse,
        IDTypeRequestor,
        IDTypeSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceDocInterimEnum
    {
        DateOfOrder,
        Tag,
        IdentityNoRequestor,
        IdentityNoSpouse,
        IDTypeRequestor,
        IDTypeSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    public enum DocTypeMetaDataDivorceDocFinalEnum
    {
        DateOfFinalJudgement,
        DivorceCaseNo,
        Tag,
        IdentityNoRequestor,
        IdentityNoSpouse,
        IDTypeRequestor,
        IDTypeSpouse,
        NameOfRequestor,
        NameOfSpouse
    }

    public enum CustomerTypeEnum
    {
        P,
        E
    }

    public enum IDTypeEnum
    {
        UIN,
        FIN,
        XIN
    }

    public enum TagEnum
    {
        Local_Civil,
        Local_Muslim,
        Foreign
    }

    public enum TagGeneralEnum
    {
        Local,
        Foreign
    }

    #endregion
}

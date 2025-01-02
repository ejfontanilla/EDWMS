using System;
using System.Web.Security;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;

public partial class Controls_MainMenu : System.Web.UI.UserControl
{
    protected void RadMainMenu_PreRender(object sender, EventArgs e)
    {
        RadMenu RadMainMenu = sender as RadMenu;

        foreach (RadMenuItem rootItem in RadMainMenu.Items)
        {
            rootItem.Attributes.Add("style", "cursor:pointer");// for root tems  

            rootItem.Font.Bold = true;

            foreach (RadMenuItem childItem in rootItem.Items)
            {
                childItem.Attributes.Add("style", "cursor:pointer");//for child items  
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        //June 11, 2013 Added !IsPostBack Condition by Edward
        if (!IsPostBack)
        {
        //get user role access
        Guid? userId = null;
        Guid roleId = Guid.Empty;
        MembershipUser user = Membership.GetUser();

        if (user == null)
            return;

        userId = (Guid)user.ProviderUserKey;
        string userName = user.UserName;

        AccessControlDb accessControlDb = new AccessControlDb();

        UserDb userDb = new UserDb();
        roleId = userDb.GetRoleId(userId.Value);

        AccessControl.AccessControlDataTable accessControl = new AccessControl.AccessControlDataTable();
        accessControl = accessControlDb.GetAccessControl(roleId, true);

        if (accessControl.Rows.Count > 0)
        {
            List<string> AccessControlList = new List<string>();
            foreach (AccessControl.AccessControlRow r in accessControl.Rows)
            {
                AccessControlList.Add(r.Module.ToLower().Trim());
                AccessControlList.Add(r.Module.ToLower().Trim() + r.AccessRight.ToLower().Trim());
            }

            #region IMPORT

            if (AccessControlList.Contains(ModuleNameEnum.Import.ToString().ToLower()))
            {
                RadMenuItem Import = new RadMenuItem("Import");
                Import.NavigateUrl = "~/Import";

                if (AccessControlList.Contains(ModuleNameEnum.Import.ToString().ToLower() + AccessControlSettingEnum.Scan.ToString().ToLower()))
                {
                    RadMenuItem ScanDocuments = new RadMenuItem("Scan Documents");
                    ScanDocuments.NavigateUrl = "~/Import/ScanDocuments/Default.aspx";
                    Import.Items.Add(ScanDocuments);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Import.ToString().ToLower() + AccessControlSettingEnum.Upload.ToString().ToLower()))
                {
                    RadMenuItem UploadDocuments = new RadMenuItem("Upload Documents");
                    UploadDocuments.NavigateUrl = "~/Import/UploadDocuments/Default.aspx";
                    Import.Items.Add(UploadDocuments);
                }

                RadMainMenu.Items.Add(Import);
            }

            #endregion

            #region VERIFICATION

            if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower()))
            {
                RadMenuItem Verification = new RadMenuItem("Verification");
                Verification.NavigateUrl = "~/Verification";

                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem AllSetsReadOnlyVerification = new RadMenuItem("All Sets Read Only");
                    AllSetsReadOnlyVerification.NavigateUrl = "~/Verification/AllSetsReadOnly.aspx";
                    Verification.Items.Add(AllSetsReadOnlyVerification);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.All_Sets.ToString().ToLower()) && !AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem AllSetsVerification = new RadMenuItem("All Sets");
                    AllSetsVerification.NavigateUrl = "~/Verification/AllSets.aspx";
                    Verification.Items.Add(AllSetsVerification);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()))
                {
                    RadMenuItem PendingAssignmentVerification = new RadMenuItem("Pending Assignment");
                    PendingAssignmentVerification.NavigateUrl = "~/Verification/PendingAssignment.aspx";
                    Verification.Items.Add(PendingAssignmentVerification);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()))
                {
                    RadMenuItem AssignedToMeVerification = new RadMenuItem("Assigned To Me");
                    AssignedToMeVerification.NavigateUrl = "~/Verification/AssignedToMe.aspx";
                    Verification.Items.Add(AssignedToMeVerification);
                }
                //Added by Edward 2014/04/30  Batch Assignment Panel 
                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.Batch_Assignment.ToString().ToLower()))
                {
                    RadMenuItem AssignmentHleVerification = new RadMenuItem("Batch Assignment");
                    AssignmentHleVerification.NavigateUrl = "~/Verification/AssignmentHLE.aspx";
                    Verification.Items.Add(AssignmentHleVerification);
                }


                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.Imported_By_Me.ToString().ToLower()))
                {
                    RadMenuItem ImportedByMeVerification = new RadMenuItem("Imported By Me");
                    ImportedByMeVerification.NavigateUrl = "~/Verification/ImportedByMe.aspx";
                    Verification.Items.Add(ImportedByMeVerification);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Verification.ToString().ToLower() + AccessControlSettingEnum.Assignment_Report.ToString().ToLower()))
                {
                    RadMenuItem AssignmentReportVerification = new RadMenuItem("Assignment Report");
                    AssignmentReportVerification.NavigateUrl = "~/Verification/AssignmentReport.aspx";
                    Verification.Items.Add(AssignmentReportVerification);
                }

                RadMainMenu.Items.Add(Verification);
            }

            #endregion

            #region COMPLETENESS

            if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower()))
            {
                RadMenuItem Completeness = new RadMenuItem("Completeness");
                Completeness.NavigateUrl = "~/Completeness";

                if ((AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower())))
                {
                    RadMenuItem AllAppsReadOnlyCompleteness = new RadMenuItem("All Applications Read Only");
                    AllAppsReadOnlyCompleteness.NavigateUrl = "~/Completeness/AllAppsReadOnly.aspx";
                    Completeness.Items.Add(AllAppsReadOnlyCompleteness);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.All_Apps.ToString().ToLower()) && !AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem AllAppsCompleteness = new RadMenuItem("All Applications");
                    AllAppsCompleteness.NavigateUrl = "~/Completeness/AllApps.aspx";
                    Completeness.Items.Add(AllAppsCompleteness);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()))
                {
                    RadMenuItem PendingAssignmentCompleteness = new RadMenuItem("Pending Assignment");
                    PendingAssignmentCompleteness.NavigateUrl = "~/Completeness/PendingAssignment.aspx";
                    Completeness.Items.Add(PendingAssignmentCompleteness);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()))
                {
                    RadMenuItem AssignedToMeCompleteness = new RadMenuItem("Assigned To Me");
                    AssignedToMeCompleteness.NavigateUrl = "~/Completeness/AssignedToMe.aspx";
                    Completeness.Items.Add(AssignedToMeCompleteness);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.Batch_Assignment.ToString().ToLower()))
                {
                    RadMenuItem AssignmentHleCompleteness = new RadMenuItem("Batch Assignment");
                    AssignmentHleCompleteness.NavigateUrl = "~/Completeness/AssignmentHle.aspx";
                    Completeness.Items.Add(AssignmentHleCompleteness);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Completeness.ToString().ToLower() + AccessControlSettingEnum.Assignment_Report.ToString().ToLower()))
                {
                    RadMenuItem AssignmentReportCompleteness = new RadMenuItem("Assignment Report");
                    AssignmentReportCompleteness.NavigateUrl = "~/Completeness/AssignmentReport.aspx";
                    Completeness.Items.Add(AssignmentReportCompleteness);
                }

                RadMainMenu.Items.Add(Completeness);
            }

            #endregion

            #region INCOME ASSESSMENT

            if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower()))
            {
                RadMenuItem IncomeAssessment = new RadMenuItem("Income Extraction");
                IncomeAssessment.NavigateUrl = "~/IncomeAssessment";

                if ((AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower())))
                {
                    RadMenuItem AllAppsReadOnlyIncomeAssessment = new RadMenuItem("All Applications Read Only");
                    AllAppsReadOnlyIncomeAssessment.NavigateUrl = "~/IncomeAssessment/AllAppsReadOnly.aspx";
                    IncomeAssessment.Items.Add(AllAppsReadOnlyIncomeAssessment);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.All_Apps.ToString().ToLower()) && !AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem AllAppsIncomeAssessment = new RadMenuItem("All Applications");
                    AllAppsIncomeAssessment.NavigateUrl = "~/IncomeAssessment/AllApps.aspx";
                    IncomeAssessment.Items.Add(AllAppsIncomeAssessment);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()))
                {
                    RadMenuItem PendingAssignmentIncomeAssessment = new RadMenuItem("Pending Assignment");
                    PendingAssignmentIncomeAssessment.NavigateUrl = "~/IncomeAssessment/PendingAssignment.aspx";
                    IncomeAssessment.Items.Add(PendingAssignmentIncomeAssessment);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()))
                {
                    RadMenuItem AssignedToMeIncomeAssessment = new RadMenuItem("Assigned To Me");
                    AssignedToMeIncomeAssessment.NavigateUrl = "~/IncomeAssessment/AssignedToMe.aspx";
                    IncomeAssessment.Items.Add(AssignedToMeIncomeAssessment);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.Batch_Assignment.ToString().ToLower()))
                {
                    RadMenuItem AssignmentHleIncomeAssessment = new RadMenuItem("Batch Assignment");
                    AssignmentHleIncomeAssessment.NavigateUrl = "~/IncomeAssessment/AssignmentHle.aspx";
                    IncomeAssessment.Items.Add(AssignmentHleIncomeAssessment);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Income_Assessment.ToString().ToLower() + AccessControlSettingEnum.Assignment_Report.ToString().ToLower()))
                {
                    RadMenuItem AssignmentReportIncomeAssessment = new RadMenuItem("Assignment Report");
                    AssignmentReportIncomeAssessment.NavigateUrl = "~/IncomeAssessment/AssignmentReport.aspx";
                    IncomeAssessment.Items.Add(AssignmentReportIncomeAssessment);
                }

                RadMainMenu.Items.Add(IncomeAssessment);
            }

            #endregion

            #region MAINTENANCE

            if (AccessControlList.Contains(ModuleNameEnum.Maintenance.ToString().ToLower()))
            {
                RadMenuItem Maintenance = new RadMenuItem("Maintenance");
                Maintenance.NavigateUrl = "~/Maintenance";

                RadMenuItem UserAccountsMaintenance = new RadMenuItem("User Accounts");
                UserAccountsMaintenance.NavigateUrl = "~/Maintenance/UserAccounts/Default.aspx";
                Maintenance.Items.Add(UserAccountsMaintenance);

                RadMenuItem RoleManagementMaintenance = new RadMenuItem("Roles Management");
                RoleManagementMaintenance.NavigateUrl = "~/Maintenance/RoleManagement/Default.aspx";
                Maintenance.Items.Add(RoleManagementMaintenance);

                RadMenuItem AccessControlMaintenance = new RadMenuItem("Access Control");
                AccessControlMaintenance.NavigateUrl = "~/Maintenance/AccessControl/Default.aspx";
                Maintenance.Items.Add(AccessControlMaintenance);

                RadMenuItem ControlParametersMaintenance = new RadMenuItem("Control Parameters");
                ControlParametersMaintenance.NavigateUrl = "~/Maintenance/ControlParameters/Default.aspx";
                Maintenance.Items.Add(ControlParametersMaintenance);

                RadMenuItem OcrParametersMaintenance = new RadMenuItem("OCR Parameters");
                OcrParametersMaintenance.NavigateUrl = "~/Maintenance/OcrParameters/Default.aspx";
                Maintenance.Items.Add(OcrParametersMaintenance);

                RadMenuItem DocumentTypeMaintenance = new RadMenuItem("Document Types");
                DocumentTypeMaintenance.NavigateUrl = "~/Maintenance/DocumentType/Default.aspx";
                Maintenance.Items.Add(DocumentTypeMaintenance);

                RadMenuItem StreetTableMaintenance = new RadMenuItem("Street Name");
                StreetTableMaintenance.NavigateUrl = "~/Maintenance/StreetTable/Default.aspx";
                Maintenance.Items.Add(StreetTableMaintenance);

                RadMenuItem MasterListMaintenance = new RadMenuItem("Master List");
                MasterListMaintenance.NavigateUrl = "~/Maintenance/MasterList/Default.aspx";
                Maintenance.Items.Add(MasterListMaintenance);

                RadMenuItem OrganizationMaintenance = new RadMenuItem("Organization");
                OrganizationMaintenance.NavigateUrl = "~/Maintenance/Organization/Default.aspx";
                Maintenance.Items.Add(OrganizationMaintenance);

                //RadMenuItem ImportInterfaceMaintenance = new RadMenuItem("Import Interface Files");
                //ImportInterfaceMaintenance.NavigateUrl = "~/Maintenance/ImportInterface/Default.aspx";
                //Maintenance.Items.Add(ImportInterfaceMaintenance);

                RadMenuItem StopWordsMaintenance = new RadMenuItem("Stop Words");
                StopWordsMaintenance.NavigateUrl = "~/Maintenance/StopWords/Default.aspx";
                Maintenance.Items.Add(StopWordsMaintenance);

                RadMenuItem BatchUploadMaintenance = new RadMenuItem("Batch Upload");
                BatchUploadMaintenance.NavigateUrl = "~/Maintenance/BatchUpload/Default.aspx";
                Maintenance.Items.Add(BatchUploadMaintenance);

                RadMenuItem AuditTrailMaintenance = new RadMenuItem("Audit Trail");
                AuditTrailMaintenance.NavigateUrl = "~/Maintenance/AuditTrail/Default.aspx";
                Maintenance.Items.Add(AuditTrailMaintenance);

                #region Added By Edward to Fix Audit Trail Filter Tables on 2015/06/29
                RadMenuItem SetActionMaintenance = new RadMenuItem("Set Action");
                SetActionMaintenance.NavigateUrl = "~/Maintenance/AuditTrail/SetAction.aspx";
                Maintenance.Items.Add(SetActionMaintenance);
                #endregion

                RadMenuItem IncomeTemplateMaintenance = new RadMenuItem("Income Template");
                IncomeTemplateMaintenance.NavigateUrl = "~/Maintenance/IncomeTemplate/Default.aspx";
                Maintenance.Items.Add(IncomeTemplateMaintenance);

                RadMenuItem ArchiveMaintenance = new RadMenuItem("Archive Parameters");
                ArchiveMaintenance.NavigateUrl = "~/Maintenance/ArchiveDocuments/Default.aspx";
                Maintenance.Items.Add(ArchiveMaintenance);

                RadMainMenu.Items.Add(Maintenance);
            }

            #endregion

            #region SEARCH

            if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower()))
            {
                RadMenuItem Search = new RadMenuItem("Search");
                Search.NavigateUrl = "~/Search";

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Search_All_Department_Sets.ToString().ToLower()))
                {
                    RadMenuItem SearchAllDepartmentSets = new RadMenuItem("Search Documents for All Departments");
                    SearchAllDepartmentSets.NavigateUrl = "~/Search/SearchAllDepartmentSets/Default.aspx";
                    Search.Items.Add(SearchAllDepartmentSets);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Search_All_Department_Sets_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem SearchAllDepartmentSetsReadOnly = new RadMenuItem("Search Documents for All Departments (Read Only)");
                    SearchAllDepartmentSetsReadOnly.NavigateUrl = "~/Search/SearchAllDepartmentSetsReadOnly/Default.aspx";
                    Search.Items.Add(SearchAllDepartmentSetsReadOnly);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Search_Own_Department_Sets.ToString().ToLower()))
                {
                    RadMenuItem SearchOwnDepartmentSets = new RadMenuItem("Search Documents for Own Department");
                    SearchOwnDepartmentSets.NavigateUrl = "~/Search/SearchOwnDepartmentSets/Default.aspx";
                    Search.Items.Add(SearchOwnDepartmentSets);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Search_Own_Department_Sets_Read_Only.ToString().ToLower()))
                {
                    RadMenuItem SearchOwnDepartmentSetsReadOnly = new RadMenuItem("Search Documents for Own Department (Read Only)");
                    SearchOwnDepartmentSetsReadOnly.NavigateUrl = "~/Search/SearchOwnDepartmentSetsReadOnly/Default.aspx";
                    Search.Items.Add(SearchOwnDepartmentSetsReadOnly);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Exception_Report.ToString().ToLower()))
                {
                    RadMenuItem ExceptionReport = new RadMenuItem("Exception Report");
                    ExceptionReport.NavigateUrl = "~/Search/ExceptionReport/Default.aspx";
                    Search.Items.Add(ExceptionReport);
                }
                #region Added by Edward Development of Reports 2014/06/09
                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Error_Sending_to_CDB.ToString().ToLower()))
                {
                    RadMenuItem ErrorSendingToCDBReport = new RadMenuItem("Error Sending to CDB Report");
                    ErrorSendingToCDBReport.NavigateUrl = "~/Search/ErrorSendingToCDB/Default.aspx";
                    Search.Items.Add(ErrorSendingToCDBReport);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Batch_Upload.ToString().ToLower()))
                {
                    RadMenuItem BatchUploadReport = new RadMenuItem("Batch Upload Report");
                    BatchUploadReport.NavigateUrl = "~/Search/BatchUpload/Default.aspx";
                    Search.Items.Add(BatchUploadReport);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.OCR_and_Web_Service_Errors.ToString().ToLower()))
                {
                    RadMenuItem OCRWebServiceReport = new RadMenuItem("OCR / Web Service Error Report");
                    OCRWebServiceReport.NavigateUrl = "~/Search/OcrWebService/Default.aspx";
                    Search.Items.Add(OCRWebServiceReport);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Verification.ToString().ToLower()))
                {
                    RadMenuItem VerificationReport = new RadMenuItem("Verification");
                    VerificationReport.NavigateUrl = "~/Search/Verification/Verification.aspx";
                    Search.Items.Add(VerificationReport);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Verification.ToString().ToLower()))
                {
                    RadMenuItem VerificationReport = new RadMenuItem("Verification Per Aging");
                    VerificationReport.NavigateUrl = "~/Search/Verification/VerificationPerAging.aspx";
                    Search.Items.Add(VerificationReport);
                }

                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Verification.ToString().ToLower()))
                {
                    RadMenuItem VerificationReport = new RadMenuItem("Verification Per OIC");
                    VerificationReport.NavigateUrl = "~/Search/Verification/VerificationPerOIC.aspx";
                    Search.Items.Add(VerificationReport);
                }
                #endregion

                #region Added By Edward 2015/02/05 For Archival Report
                if (AccessControlList.Contains(ModuleNameEnum.Search.ToString().ToLower() + AccessControlSettingEnum.Archival_Report.ToString().ToLower()))
                {
                    RadMenuItem ArchivalReport = new RadMenuItem("Archival Report");
                    ArchivalReport.NavigateUrl = "~/Search/ArchivalReport/Default.aspx";
                    Search.Items.Add(ArchivalReport);
                }
                #endregion

                RadMainMenu.Items.Add(Search);
            }

            #endregion

            #region FILEDOC

            if (AccessControlList.Contains(ModuleNameEnum.FileDoc.ToString().ToLower()))
            {
                RadMenuItem FileDoc = new RadMenuItem("File Doc");
                FileDoc.NavigateUrl = "~/FileDoc";

                if (AccessControlList.Contains(ModuleNameEnum.FileDoc.ToString().ToLower() + AccessControlSettingEnum.View_Only.ToString().ToLower()))
                {
                    RadMenuItem ViewOnly = new RadMenuItem("View Only");
                    ViewOnly.NavigateUrl = "~/FileDoc/ViewOnly.aspx";
                    FileDoc.Items.Add(ViewOnly);
                }

                if (AccessControlList.Contains(ModuleNameEnum.FileDoc.ToString().ToLower() + AccessControlSettingEnum.Download.ToString().ToLower()))
                {
                    RadMenuItem Download = new RadMenuItem("Download");
                    Download.NavigateUrl = "~/FileDoc/Download.aspx";
                    FileDoc.Items.Add(Download);
                }

                RadMainMenu.Items.Add(FileDoc);
            }

            #endregion

            #region HELP

            if (AccessControlList.Contains(ModuleNameEnum.FileDoc.ToString().ToLower()))
            {
                RadMenuItem Help = new RadMenuItem("Help");
                Help.NavigateUrl = "~/Help";
                RadMainMenu.Items.Add(Help);
            }

            #endregion

            #region Highlight the selected navigation tab

            string url = Request.Url.ToString().ToLower();
            System.Drawing.Color color = System.Drawing.Color.White;
            System.Drawing.Color backColor = System.Drawing.ColorTranslator.FromHtml("#7c84af");

            if (url.Contains("/import/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Import").Selected = true;
                RadMainMenu.Items.FindItemByText("Import").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Import").BackColor = backColor;
            }
            else if (url.Contains("/search/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Search").Selected = true;
                RadMainMenu.Items.FindItemByText("Search").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Search").BackColor = backColor;
            }
            else if (url.Contains("/verification/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Verification").Selected = true;
                RadMainMenu.Items.FindItemByText("Verification").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Verification").BackColor = backColor;
            }
            else if (url.Contains("/completeness/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Completeness").Selected = true;
                RadMainMenu.Items.FindItemByText("Completeness").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Completeness").BackColor = backColor;
            }
            else if (url.Contains("/incomeassessment/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Income Extraction").Selected = true;
                RadMainMenu.Items.FindItemByText("Income Extraction").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Income Extraction").BackColor = backColor;
            }
            else if (url.Contains("/reports/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Reports").Selected = true;
                RadMainMenu.Items.FindItemByText("Reports").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Reports").BackColor = backColor;
            }
            else if (url.Contains("/filedoc/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("File Doc").Selected = true;
                RadMainMenu.Items.FindItemByText("File Doc").ForeColor = color;
                RadMainMenu.Items.FindItemByText("File Doc").BackColor = backColor;
            }
            else if (url.Contains("/maintenance/") && !url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Maintenance").Selected = true;
                RadMainMenu.Items.FindItemByText("Maintenance").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Maintenance").BackColor = backColor;
            }
            else if (url.Contains("/help/"))
            {
                RadMainMenu.Items.FindItemByText("Help").Selected = true;
                RadMainMenu.Items.FindItemByText("Help").ForeColor = color;
                RadMainMenu.Items.FindItemByText("Help").BackColor = backColor;
            }

            #endregion
        }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.IO;
using TallComponents.Web.Storage;
using System.Drawing;

public partial class IncomeAssessment_ViewApp : System.Web.UI.Page
{
    #region Members, Properties and Contructors
    public int? docAppId;
    //private int applicantCnt;         Commented by Edward 2016/02/04 take out unused variables
    //private int occupierCnt;          Commented by Edward 2016/02/04 take out unused variables
    //private int summaryApplicantCnt;  Commented by Edward 2016/02/04 take out unused variables
    //private int summaryOccupierCnt;   Commented by Edward 2016/02/04 take out unused variables
    //private int summaryBuyerCnt;      Commented by Edward 2016/02/04 take out unused variables
    //private int summarySellerCnt;     Commented by Edward 2016/02/04 take out unused variables
    //private int summaryMiscCnt;       Commented by Edward 2016/02/04 take out unused variables
    string previousHleNumber = string.Empty;
    private string currentDocType = string.Empty;
    //private string selectedNodeGlobal = "0";  Commented by Edward 2016/02/04 take out unused variables
    #region Modified by Edward 2015/11/18 Change condition of access of updating doc acceptance
    //public bool isAppConfirmed = false;
    //bool AllowCompletenessSaveDate = false;
    #endregion
    bool ExtractionPeriodValid = true;
    #endregion

    bool HasAccess = true;  
    HiddenField ShowHideTextBox;
    LinkButton ShowHideNotAcceptedLinkBtn;
   
    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            docAppId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        // Check if refno exit
        DocAppDb docAppDb = new DocAppDb();
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

        if (docApps.Rows.Count == 0)
            Response.Redirect("~/IncomeAssessment/");

        #region Modified by Edward 2015/11/18 Change condition of access of updating doc acceptance
        //isAppConfirmed = docAppDb.IsAppConfirmed(docAppId.Value);
        //AllowCompletenessSaveDate = docAppDb.AllowCompletenessSaveDate(docAppId.Value);
        #endregion

        if (!IsPostBack)
        {
            PopulateRefInfo();
            Session.Remove("HLESummaryRepeater");
        }
        Session["r"] = "no";
        PopulatePersonals();
        ActionLogButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ActionLog.aspx?id={0}',800,500);return false;", docAppId.Value)); //Added BY Edward 24/02/2014 Add Icon and Action Log
    }

    #region Summary Repeater ItemCommand Events
    protected void HleSummaryRepeater_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
    {
        PopulatePersonals();
    }

    protected void ResaleSummaryRepeater_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
    {
        PopulatePersonals();
    }

    protected void SalesSummaryRepeater_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
    {
        PopulatePersonals();
    }
    #endregion

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        bool fromSummary = true;
        bool.TryParse(e.Argument, out fromSummary);
        
        if (e.Argument == "null" || e.Argument == string.Empty)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        //else if(e.Argument.ToString().Contains("capture"))
        //{
        //    InsertAHGIncome(e.Argument.ToString());
        //    PopulatePersonals();
        //}
        //else if (e.Argument.ToString().Contains("clear"))
        //{
        //    ClearAHGIncome(e.Argument.ToString());
        //    PopulatePersonals();
        //}
        else if (e.Argument.ToString().Contains("credit"))
        {
            Session["Tab"] = e.Argument.ToString().Substring(6);
            PopulatePersonals();
        }
        else if (e.Argument.ToString().Contains("ShowHide"))
        {
        }
        else if (e.Argument.ToString().Contains("zero"))
        {
            //IncomeDb.SetIncomeToZero(docAppId
        }

    }

    #region AHGINCOME Methods
    //Whenever you click Capture link to get the AHG Income Average
    //private void InsertAHGIncome(string capture)
    //{
    //    MembershipUser user = Membership.GetUser();
    //    Guid currentUserId = (Guid)user.ProviderUserKey;
    //    DataTable dt = IncomeDb.GetAHGFromIncomeTableByAppPersonalId(int.Parse(capture.Substring(8)));
    //    if (dt.Rows.Count > 0)
    //    {
    //        AHGIncomeDb db = new AHGIncomeDb();
    //        AHGIncome.AHGIncomeDataTable aHGIncomeDt = db.GetAHGIncomeByAppPersonalId(int.Parse(capture.Substring(8)));
    //        if (aHGIncomeDt.Rows.Count > 0)
    //        {
    //            if (capture.Substring(0, 8) == "capture1")
    //            {
    //                db.AHGTotalAmount1 = decimal.Parse(dt.Rows[0]["AHGIncomeTotal"].ToString());
    //                db.AHGAvgAmount1 = decimal.Parse(dt.Rows[0]["AHGAverage"].ToString());
    //                db.NoOfMonths1 =  int.Parse(dt.Rows[0]["NoOfMonths"].ToString());
    //                db.AppPersonalId = int.Parse(capture.Substring(8));
    //                db.UpdateBy = currentUserId;
    //                db.UpdateCapture1(db);
    //            }
    //            else
    //            {
    //                db.AHGTotalAmount2 = decimal.Parse(dt.Rows[0]["AHGIncomeTotal"].ToString());
    //                db.AHGAvgAmount2 = decimal.Parse(dt.Rows[0]["AHGAverage"].ToString());
    //                db.NoOfMonths2 =  int.Parse(dt.Rows[0]["NoOfMonths"].ToString());
    //                db.AppPersonalId = int.Parse(capture.Substring(8));
    //                db.UpdateBy = currentUserId;
    //                db.UpdateCapture2(db);
    //            }
    //        }
    //        else
    //        {
    //            if (capture.Substring(0, 8) == "capture1")
    //            {
    //                db.AHGTotalAmount1 = decimal.Parse(dt.Rows[0]["AHGIncomeTotal"].ToString());
    //                db.AHGAvgAmount1 = decimal.Parse(dt.Rows[0]["AHGAverage"].ToString());
    //                db.NoOfMonths1 = int.Parse(dt.Rows[0]["NoOfMonths"].ToString());
    //            }
    //            else if (capture.Substring(0, 8) == "capture2")
    //            {
    //                db.AHGTotalAmount2 = decimal.Parse(dt.Rows[0]["AHGIncomeTotal"].ToString());
    //                db.AHGAvgAmount2 = decimal.Parse(dt.Rows[0]["AHGAverage"].ToString());
    //                db.NoOfMonths2 = int.Parse(dt.Rows[0]["NoOfMonths"].ToString());
    //            }
    //            db.UpdateBy = currentUserId;
    //            db.AppPersonalId = int.Parse(capture.Substring(8));
    //            db.Insert(db, capture.Substring(0, 8));

    //        }
    //    }
    //}

    ////Whenever you click Clear link to update AHG Income to Null
    //private void ClearAHGIncome(string clear)
    //{
    //    MembershipUser user = Membership.GetUser();
    //    Guid currentUserId = (Guid)user.ProviderUserKey;
    //    DataTable dt = IncomeDb.GetAHGFromIncomeTableByAppPersonalId(int.Parse(clear.Substring(6)));
    //    if (dt.Rows.Count > 0)
    //    {
    //        AHGIncomeDb db = new AHGIncomeDb();
    //        AHGIncome.AHGIncomeDataTable aHGIncomeDt = db.GetAHGIncomeByAppPersonalId(int.Parse(clear.Substring(6)));
    //        if (aHGIncomeDt.Rows.Count > 0)
    //        {
    //            if (clear.Substring(0, 6) == "clear1")
    //            { 
    //                db.AppPersonalId = int.Parse(clear.Substring(6));
    //                db.UpdateBy = currentUserId;
    //                AHGIncomeDb.UpdateClear1(db);
    //            }
    //            else
    //            {
    //                db.AppPersonalId = int.Parse(clear.Substring(6));
    //                db.UpdateBy = currentUserId;
    //                AHGIncomeDb.UpdateClear2(db);
    //            }
    //        }            
    //    }
    //}
    #endregion

    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {

    }

    public void DocTypeCodeLinkButtonClick(object sender, EventArgs e)
    {
        LinkButton docTypeCodeLinkButton = (LinkButton)sender;
    }

    protected void HousingGrant(object sender, EventArgs e)
    {
        string strAddress;
        Button button = (Button)sender;

        if (button.Text.ToLower().Contains("additional"))
            strAddress = String.Format("window.open('../Common/AdditionalHousingGrant.ashx?docAppId={0}', 'word','width=500,height=500, left=0, top=0')", docAppId.Value);        
        else if (button.Text.ToLower().Contains("excel")) // Added By Edward Excel Worksheet on 2015/4/24
            strAddress = String.Format("window.open('../Common/DownloadExcelWorksheet.aspx?docAppId={0}', 'word','width={1},height={2}, left=0, top=0, resizable=1')",
                docAppId.Value, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 200, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 150);                  
        else
            strAddress = String.Format("window.open('../Common/DownloadWorksheet.aspx?docAppId={0}', 'word','width={1},height={2}, left=0, top=0, resizable=1')",
                docAppId.Value, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 200, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 150);                  

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", String.Format("javascript:{0};", strAddress), true);
    }

    #endregion

    private void SetAccessControl(Guid? assessmentStaffUserId, string status)
    {
        MembershipUser user = Membership.GetUser();
        Guid currentUserId = (Guid)user.ProviderUserKey;

        if (assessmentStaffUserId == Guid.Empty || currentUserId != assessmentStaffUserId)
        {
            ConfirmButton.Enabled = false;
            HousingGrantButton.Enabled = false;            
            HasAccess = false;            
        }

        #region Check if Extracted or Closed then ConfirmButton is invisible. Also sets the Label of the Worksheet Button to Download
        if (status.Equals(AssessmentStatusEnum.Extracted.ToString()) ||
            status.Equals(AssessmentStatusEnum.Closed.ToString()))
        {
            ConfirmButton.Visible = false;
            HousingGrantButton.Text = "Extraction Worksheet";            
            HasAccess = false;
        }
        #endregion
    }

    private void PopulatePersonals()
    {
        try
        {
            if (docAppId.HasValue)
            {
                DocAppDb docAppDb = new DocAppDb();
                DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

                if (docApps.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppRow = docApps[0];
                    string refType = docAppRow.RefType.ToUpper().Trim();

                    SetAccessControl(docAppRow.IsAssessmentStaffUserIdNull() ? Guid.Empty : docAppRow.AssessmentStaffUserId, docAppRow.AssessmentStatus.Trim());

                    #region Checks the Section if COS , ROS or SOS and hides the corresponding Worksheet Button
                    DepartmentDb departmentDb = new DepartmentDb();
                    Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
                    Department.DepartmentRow departmentRow = departments[0];

                    SectionDb sectionDb = new SectionDb();
                    Section.SectionDataTable sectionDt = sectionDb.GetSectionByDepartment(departmentRow.Id);
                    Section.SectionRow sectionRow = sectionDt[0];
                    #endregion

                    if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                    {
                        #region HLE
                        HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                        HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);
                        HleSummaryRepeater.DataSource = hleInterfaceDt;
                        HleSummaryRepeater.DataBind();
                        Session["HLESummaryRepeater"] = HleSummaryRepeater;
                        Object obj = HleSummaryRepeater;
                        CosNoSummaryPanel.Visible = (hleInterfaceDt.Rows.Count <= 0);
                        SummaryMultiView.ActiveViewIndex = 0;
                        #endregion
                    }
                    else if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))
                    {
                        #region RESALE
                        ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                        ResaleInterface.ResaleInterfaceDataTable resaleInterfaceDt = resaleInterfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);     
                        ResaleSummaryRepeater.DataSource = resaleInterfaceDt;
                        ResaleSummaryRepeater.DataBind();
                        Session["ResaleSummaryRepeater"] = ResaleSummaryRepeater;
                        SummaryMultiView.ActiveViewIndex = 1;
                        #endregion
                    }
                    else if (refType.Contains(ReferenceTypeEnum.SALES.ToString()))
                    {
                        #region SALES
                        SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                        SalesInterface.SalesInterfaceDataTable salesInterfaceDt = salesInterfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);
                        SalesSummaryRepeater.DataSource = salesInterfaceDt;
                        SalesSummaryRepeater.DataBind();
                        Session["SalesSummaryRepeater"] = SalesSummaryRepeater;
                        SummaryMultiView.ActiveViewIndex = 2;
                        #endregion
                    }                    
                }
            }
        }
        catch (Exception ex)
        {
            InfoLabel.Text = ex.Message + " (PopulatePersonals) ";
            WarningPanel.Visible = true;            
        }
        
    }

    #region FOR HLE

    #region HLE Repeater ItemDataBound
    protected void HleSummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        string msgIncCaseOfError = string.Empty;
        try
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem item = (RepeaterItem)e.Item;
                DataRowView data = (DataRowView)item.DataItem;
                
                #region Personal Type, Order No, Date Of Birth, Date Joined, Employment Status, Company Name, CitizenShip
                msgIncCaseOfError = "Personal Type, Order No, Date Of Birth, Date Joined, Employment Status, Company Name, CitizenShip";
                string personalType = data["ApplicantType"].ToString();

                int orderNo = int.Parse(data["OrderNo"].ToString());

                Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabel");
                string applicantTypeFormat = "{0} - {1} ({2})";
                if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                    ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                        data["Name"].ToString(), data["Nric"].ToString());
                else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                    ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                        data["Name"].ToString(), data["Nric"].ToString());
                else
                    ApplicantLabel.Text = String.Format(applicantTypeFormat, orderNo.ToString(),
                        data["Name"].ToString(), data["Nric"].ToString());

                Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabel");
                DateOfBirthLabel.Text = (string.IsNullOrEmpty(data["DateOfBirth"].ToString()) || data["DateOfBirth"].ToString().Trim().Equals(".") ? "-" :
                   Format.FormatDateTime(data["DateOfBirth"].ToString(), DateTimeFormat.dd__MMM__yyyy));

                Label DateJoinedLabel = (Label)e.Item.FindControl("DateJoinedLabel");
                DateJoinedLabel.Text = (data["DateJoined"].ToString().Trim().Equals(".") ? "-" :
                    Format.FormatDateTime(data["DateJoined"].ToString(), DateTimeFormat.dd__MMM__yyyy));


                Label EmploymentStatusLabel = (Label)e.Item.FindControl("EmploymentStatusLabel");
                EmploymentStatusLabel.Text = data["EmploymentType"].ToString();

                Label CompanyNameLabel = (Label)e.Item.FindControl("CompanyNameLabel");
                CompanyNameLabel.Text = (!string.IsNullOrEmpty(data["EmployerName"].ToString()) ? data["EmployerName"].ToString() : "-");

                Label CitizenshipLabel = (Label)e.Item.FindControl("CitizenshipLabel");
                CitizenshipLabel.Text = (!string.IsNullOrEmpty(data["Citizenship"].ToString()) ? data["Citizenship"].ToString() : "-");
                #endregion

                #region Display Not accepted documents and the ApplicantImage
                msgIncCaseOfError = "Displaying Not Accepted Documents";
                int noOfNotAccepted = IncomeDb.GetNoofNotAcceptedRecordsPerNRIC(docAppId.Value, data["Nric"].ToString());
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(data["Nric"].ToString(), docAppId.Value);
                bool noOfIncome = false;
                int appPersonalId = 0;
                if (appPersonaldt.Rows.Count > 0)
                {
                    AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                    noOfIncome = IncomeDb.CheckIncomeCompleteByNRIC(data["Nric"].ToString(), appPersonalRow.Id);
                    //if (noOfIncome)
                    //    noOfIncome = IncomeDb.CheckCreditAssessmentByAppPersonalId(appPersonalRow.Id);
                    appPersonalId = appPersonalRow.Id;
                }

                if (HasAccess)
                {
                    if (data["EmploymentType"].ToString().Trim().ToUpper() == EmploymentStatusEnum.Unemployed.ToString().ToUpper())
                    {
                        LinkButton linkIncomeToZero = (LinkButton)e.Item.FindControl("linkIncomeToZero");
                        linkIncomeToZero.Visible = true;
                        #region Modified by edward changes from 20140604 Meeting : no more popup for setting zero
                        //linkIncomeToZero.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('SetIncomeToZero.aspx?id={0}&docappid={1}',460,200);return false;", appPersonalId.ToString(),docAppId.Value.ToString()));
                        linkIncomeToZero.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('SetIncomeToZero.aspx?id={0}&docappid={1}',300,120);return false;", appPersonalId.ToString(), docAppId.Value.ToString()));
                        #endregion
                    }
                }


                ShowHideTextBox = (HiddenField)e.Item.FindControl("ShowHideText");

                ShowHideNotAcceptedLinkBtn = (LinkButton)e.Item.FindControl("ShowHideNotAcceptedLinkBtn");
                ShowHideNotAcceptedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:onClicking('{0}');", ShowHideTextBox.ClientID));
                ShowHideNotAcceptedLinkBtn.ID = "ShowHideBtn" + data["Nric"].ToString();

                System.Web.UI.WebControls.Image ApplicantImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("ApplicantImage");
                ApplicantImage.Visible = noOfIncome;

                string r = "no";

                if (Session["HLESummaryRepeater"] != null)
                {
                    Repeater repeater = new Repeater();
                    repeater = (Repeater)Session["HLESummaryRepeater"];
                    foreach (Control ctrl in item.Controls)
                    {
                        if (ctrl is Panel)
                        {
                            foreach (Control ctrlInPanel in ctrl.Controls)
                            {
                                if (ctrlInPanel is HiddenField)
                                {
                                    r = (Request.Form[((HiddenField)ctrlInPanel).UniqueID]);
                                    Session["r"] = r;
                                }
                            }
                        }
                    }
                }

                ShowHideTextBox.Value = !string.IsNullOrEmpty(r) ? r : "no";
                if (ShowHideTextBox.Value == "no")
                    ShowHideNotAcceptedLinkBtn.Text = "Show Not Accepted List (" + noOfNotAccepted.ToString() + ")";
                else
                    ShowHideNotAcceptedLinkBtn.Text = "Hide Not Accepted List (" + noOfNotAccepted.ToString() + ")";
                #endregion

                #region View Consolidated PDF
                msgIncCaseOfError = "View Consolidated PDF";
                DataTable docsDt = IncomeDb.GetDocsByDocAppIdAndNric(docAppId.Value, data["Nric"].ToString());
                string ids = string.Empty;
                if (docsDt.Rows.Count > 0)
                {
                    foreach (DataRow row in docsDt.Rows)
                    {
                        if (r == "yes") //&& (bool.Parse(string.IsNullOrEmpty(row["IsAccepted"].ToString()) ? false.ToString() : row["IsAccepted"].ToString())))
                        {
                            ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                        }
                        else
                        {
                            //edited by calvin change to acceptance field
                            if (string.IsNullOrEmpty(row["ImageAccepted"].ToString()) ? false : row["ImageAccepted"].ToString().Trim() == "Y")
                                ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                        }
                    }
                }
                #region Added By Edward Income Extraction Changes 2014/6/17     Add HLE Form in View Consolidated PDF in Income Extraction
                DataTable HLEDt = IncomeDb.GetHLEFormByDocApp(docAppId.Value);
                if (HLEDt.Rows.Count > 0)
                {
                    foreach (DataRow row in HLEDt.Rows)
                        ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());                                                               
                }
                #endregion
                LinkButton ViewConsolidatedLinkBtn = (LinkButton)e.Item.FindControl("ViewConsolidatedLinkBtn");
                if (!string.IsNullOrEmpty(ids))
                    ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("window.open('../Common/ConsolidatedWorkSheetPDF.aspx?id={0}&appId={1}', 'word','width=1,height=1, left=0, top=0')", ids, docAppId.Value));
                else
                    ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:alert('No PDF to view')"));
                #endregion

                #region Populating the SummaryDocsRadGrid and DocumentsRadGrid
                msgIncCaseOfError = "Populate MonthYear";
                RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGrid");
                SummaryDocsRadGrid.ID = "SummaryDocsRadGrid" + data["Nric"].ToString();
                DataTable table = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, data["Nric"].ToString());

                PopulateSummaryDocs(table, SummaryDocsRadGrid);

                msgIncCaseOfError = "Populate Documents";
                RadGrid DocumentsRadGrid = (RadGrid)e.Item.FindControl("DocumentsRadGrid");
                PopulateDocumentsRadGrid(data["Nric"].ToString(), DocumentsRadGrid, ScanningTransactionTypeEnum.HLE.ToString());
                if (!HasAccess)
                    DocumentsRadGrid.MasterTableView.Columns[3].Visible = false;

                Table EmployedTable = (Table)e.Item.FindControl("EmployedTable");

                RadMultiPage radMultiPage = (RadMultiPage)e.Item.FindControl("RadMultiPage1");
                RadTabStrip tabstrip = (RadTabStrip)e.Item.FindControl("TabStrip");
                if (Session["Tab"] != null)
                {
                    if (radMultiPage.ClientID == Session["Tab"].ToString())
                    {
                        radMultiPage.SelectedIndex = 2;
                        tabstrip.SelectedIndex = 2;
                        Session["Tab"] = null;
                    }
                }
                msgIncCaseOfError = "Populate IncomeData";
                PopulateIncomeDataTab(data["Nric"].ToString(), EmployedTable, radMultiPage.ClientID);

                #endregion

                #region Setting the Assessment Period Link Button
                msgIncCaseOfError = "Assessment Period";
                Label AssessmentPeriod = (Label)e.Item.FindControl("AssessmentPeriodLabel");
                Label MonthsToLeas = (Label)e.Item.FindControl("MonthsToLeasLabel");
                int intMonthToLeas;
                if (table.Rows.Count > 0)
                {
                    AssessmentPeriod.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[0]["MonthYear"].ToString(),
                        table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), table.Rows.Count.ToString(), table.Rows.Count > 1 ? "Months" : "Month");

                    intMonthToLeas = !string.IsNullOrEmpty(table.Rows[0]["MonthsToLeas"].ToString()) ? int.Parse(table.Rows[0]["MonthsToLeas"].ToString()) : 0;
                    if (intMonthToLeas > 0 && (intMonthToLeas <= table.Rows.Count))
                        MonthsToLeas.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[(table.Rows.Count) - intMonthToLeas]["MonthYear"].ToString(),
                            table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), intMonthToLeas, intMonthToLeas > 1 ? "Months" : "Month");
                    else if (intMonthToLeas > table.Rows.Count)
                    {
                        MonthsToLeas.Text = string.Format("({0} Months)", intMonthToLeas);
                        System.Web.UI.WebControls.Image exImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("exImage");
                        exImage.ToolTip = string.Format("Extraction Period cannot be shorter than {0} Months", intMonthToLeas);
                        exImage.Visible = true;
                        ExtractionPeriodValid = false;
                    }
                    else if (intMonthToLeas == 0)
                        MonthsToLeas.Text = string.Format("({0} Months)", intMonthToLeas);
                    else
                        MonthsToLeas.Text = " - ";
                }
                else
                {
                    AssessmentPeriod.Text = " - ";
                    intMonthToLeas = !string.IsNullOrEmpty(data["NoOfIncomeMonths"].ToString()) ? int.Parse(data["NoOfIncomeMonths"].ToString()) : 0;
                    MonthsToLeas.Text = string.Format("({0}) Months", intMonthToLeas);
                }
                LinkButton AssessmentPeriodLinkBtn = (LinkButton)e.Item.FindControl("AssessmentPeriodLinkBtn");
                AssessmentPeriodLinkBtn.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('AssessmentPeriod.aspx?docAppId={0}&nric={1}&m={2}&ref={3}',580,350);return false;", 
                    docAppId.Value.ToString(), data["Nric"].ToString(), intMonthToLeas.ToString(),"hle"));
                if (!HasAccess)
                    AssessmentPeriodLinkBtn.Visible = false;
                #endregion

                msgIncCaseOfError = string.Empty;
            }
        }
        catch (Exception ex)
        {
            Label ShowPendingDocLabel = (Label)e.Item.FindControl("ShowPendingDocLabel");
            ShowPendingDocLabel.Text = string.Format("Error: {0} (HleSummaryRepeater_ItemDataBound {1}) ",ex.Message, msgIncCaseOfError);
            ShowPendingDocLabel.CssClass = "form-error";
        }

    }


    #endregion

    #region HLE MonthYearRadgrid ItemDataBound
    protected void SummaryDocRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        string nricError = string.Empty;
        string msgIncCaseOfError = string.Empty;
        try
        {
            if (e.Item is GridDataItem)
            {
                RadGrid grid = (RadGrid)sender;
                GridDataItem dataBoundItem = e.Item as GridDataItem;
                DataRowView data = (DataRowView)e.Item.DataItem;

                #region Documents Link
                nricError = data["Nric"].ToString();
                msgIncCaseOfError = "Displaying Documents Link";
                DataTable dt = IncomeDb.GetDocsByIncomeId(docAppId.Value, data["Nric"].ToString(), int.Parse(data["Id"].ToString()));

                if (dt.Rows.Count > 0)
                {
                    if (Session["r"] == null)
                        Session["r"] = "no";

                    if (Session["r"].ToString() == "no")
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //edited by calvin change to acceptance field
                            if (dt.Rows[i]["ImageAccepted"].ToString().Trim() != "Y")
                            {
                                dt.Rows.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    Repeater lblDocRepeater = (Repeater)e.Item.FindControl("lblDocRepeater");
                    lblDocRepeater.DataSource = dt;
                    lblDocRepeater.DataBind();
                }
                #endregion

                #region Setting the MonthYear Link
                msgIncCaseOfError = "MonthYear Link";
                if (HasAccess)
                {
                    HyperLink MonthYearLink = (HyperLink)e.Item.FindControl("MonthYearLink");
                    MonthYearLink.Text = data["MonthYear"].ToString();
                    string appdocrefid = dt.Rows.Count > 0 ? dt.Rows[0]["AppDocRefId"].ToString() : "0";
                    #region Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                    //MonthYearLink.NavigateUrl = string.Format("~/IncomeAssessment/ZoningPage.aspx?id={0}&nric={1}&appdocref={2}&incomeid={3}&apppersonal={4}", docAppId.Value.ToString(), data["Nric"].ToString(),
                    //    appdocrefid, data["Id"].ToString(), data["AppPersonalId"].ToString());
                    MonthYearLink.NavigateUrl = string.Format("~/IncomeAssessment/ZoneDefault.aspx?id={0}&nric={1}&appdocref={2}&incomeid={3}&apppersonal={4}", docAppId.Value.ToString(), data["Nric"].ToString(),
                        appdocrefid, data["Id"].ToString(), data["AppPersonalId"].ToString());
                    #endregion
                    MonthYearLink.Visible = true;
                }
                else
                {
                    Label MonthYearLabel = (Label)e.Item.FindControl("MonthYearLabel");
                    MonthYearLabel.Text = data["MonthYear"].ToString();
                    MonthYearLabel.Visible = true;
                }
                #endregion

                //int noOfNotAcceptedDocPerIncome = IncomeDb.GetNoofNotAcceptedRecordsPerIncome(docAppId.Value, data["Nric"].ToString(), Convert.ToInt16(data["Id"]));
                int noOfNotAcceptedDocPerIncome = IncomeDb.GetNoofIncomeDetailsPerIncome(int.Parse(data["Id"].ToString()));

                if (noOfNotAcceptedDocPerIncome > 0)
                {
                    System.Web.UI.WebControls.Image AcceptedImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("AcceptedImage");
                    AcceptedImage.Visible = true;
                }

                #region Setting the Currency Link
                msgIncCaseOfError = "Currency Link";
                if (HasAccess)
                {
                    LinkButton CurrencyLink = (LinkButton)e.Item.FindControl("CurrencyLink");
                    CurrencyLink.Text = string.Format("{0}@{1}", !string.IsNullOrEmpty(data["Currency"].ToString()) ? data["Currency"].ToString() : "SGD", data["CurrencyRate"].ToString());
                    CurrencyLink.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ChangeCurrency.aspx?incomeid={0}',500,200);return false;", data["Id"].ToString()));
                    CurrencyLink.Visible = true;
                }
                else
                {
                    Label Currencylabel = (Label)e.Item.FindControl("CurrencyLabel");
                    Currencylabel.Text = string.Format("{0}@{1}", !string.IsNullOrEmpty(data["Currency"].ToString()) ? data["Currency"].ToString() : "SGD", data["CurrencyRate"].ToString());
                    Currencylabel.Visible = true;
                }
                #endregion
                msgIncCaseOfError = string.Empty;
            }
        }
        catch (Exception ex)
        {            
            InfoLabel.Text = string.Format("{0} (SummaryDocRadGrid_ItemDataBound) {1} {2}", ex.Message, nricError, msgIncCaseOfError);
            WarningPanel.Visible = true;
        }
    }
    #endregion

    #region HLE DocumentsRadGrid ItemDataBound
    protected void DocumentsRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        try
        {
            if (e.Item is GridDataItem)
            {
                RadGrid grid = (RadGrid)sender;

                GridDataItem dataBoundItem = e.Item as GridDataItem;

                DataRowView data = (DataRowView)e.Item.DataItem;

                Label Acceptance = (Label)e.Item.FindControl("ReceivedLabel");
                switch (data["ImageAccepted"].ToString().ToUpper())
                {
                    case "Y":
                        Acceptance.Text = "Yes";
                        dataBoundItem.ForeColor = Color.Black;
                        break;
                    case "N":
                        Acceptance.Text = "No";
                        dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                        break;
                    case "X":
                        Acceptance.Text = "NA";
                        dataBoundItem.ForeColor = Color.Black;
                        break;
                    case "U":
                        Acceptance.Text = "Unknown";
                        dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                        break;
                    default:
                        Acceptance.Text = "Unknown";
                        dataBoundItem.ForeColor = Color.FromArgb(204, 0, 0);
                        break;
                }
                Control target = e.Item.FindControl("EditIconImage");
                ImageButton EditImageButton = (ImageButton)e.Item.FindControl("EditImageButton");

                //Modified by Edward 2015/09/17 Fix UpdatePendingDoc Error
                //EditImageButton.OnClientClick = String.Format("javascript:ShowWindow('Controls/UpdatePendingDoc.aspx?id={0}', 600, 500);", data["Id"]);
                EditImageButton.OnClientClick = String.Format("javascript:ShowWindow('../Completeness/Controls/UpdatePendingDoc.aspx?id={0}', 600, 500);", data["Id"]);
                
                LinkButton ZoneLinkButton = (LinkButton)e.Item.FindControl("ZoneLinkButton");                
                ZoneLinkButton.Text = "Zone";
                ZoneLinkButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('MonthToZone.aspx?docappid={0}&nric={1}&appdocrefid={2}&apppersonal={3}',0,0);return false;",
                    docAppId.Value.ToString(), data["Nric"].ToString(), data["AppDocRefId"].ToString(), data["AppPersonalId"].ToString()));

                #region  Modified by Edward 2015/11/18 Change condition of access of updating doc acceptance
                //if (AllowCompletenessSaveDate && !isAppConfirmed)
                //    EditImageButton.Visible = true;
                //else
                //    EditImageButton.Visible = false;

                EditImageButton.Visible = HasAccess;
                #endregion

            }
        }
        catch (Exception ex)
        {
            InfoLabel.Text = ex.Message + " (DocumentsRadGrid_ItemDataBound) ";
            WarningPanel.Visible = true;
        }
    }
    #endregion

    #endregion

    #region FOR RESALE

    #region Resale Repeater ItemDataBound 16/01/2014 By Edward BTDWMS20140114 - Changes for Sales and Resales
    protected void ResaleSummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;

            #region Applicant's Personal Details

            string personalType = data["ApplicantType"].ToString();

            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabelRS");
            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToUpper().Equals("BU"))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Buyer " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToUpper().Equals("SE"))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Seller " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());


            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabelRS");
            DateOfBirthLabel.Text = (string.Format(string.IsNullOrEmpty(data["DateOfBirth"].ToString()) || data["DateOfBirth"].ToString().Trim().Equals(".") ? "-" :
               Format.FormatDateTime(data["DateOfBirth"].ToString(), DateTimeFormat.dd__MMM__yyyy)));

            Label DateJoinedLabel = (Label)e.Item.FindControl("DateJoinedLabelRS");
            DateJoinedLabel.Text = " - ";   //No DateJoined


            Label EmploymentStatusLabel = (Label)e.Item.FindControl("EmploymentStatusLabelRS");
            EmploymentStatusLabel.Text = data["EmploymentType"].ToString();

            Label CompanyNameLabel = (Label)e.Item.FindControl("CompanyNameLabelRS");
            CompanyNameLabel.Text = " - "; //no CompanyName

            Label CitizenshipLabel = (Label)e.Item.FindControl("CitizenshipLabelRS");
            CitizenshipLabel.Text = (!string.IsNullOrEmpty(data["Citizenship"].ToString()) ? data["Citizenship"].ToString() : "-");
            #endregion

            #region Display Not accepted documents and the ApplicantImage
            int noOfNotAccepted = IncomeDb.GetNoofNotAcceptedRecordsPerNRIC(docAppId.Value, data["Nric"].ToString());
            AppPersonalDb appPersonalDb1 = new AppPersonalDb();
            AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb1.GetAppPersonalByNricAndDocAppId(data["Nric"].ToString(), docAppId.Value);
            bool noOfIncome = false;
            if (appPersonaldt.Rows.Count > 0)
            {
                AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                noOfIncome = IncomeDb.CheckIncomeCompleteByNRIC(data["Nric"].ToString(), appPersonalRow.Id);
            }

            ShowHideTextBox = (HiddenField)e.Item.FindControl("ShowHideTextRS");

            ShowHideNotAcceptedLinkBtn = (LinkButton)e.Item.FindControl("ShowHideNotAcceptedLinkBtnRS");
            ShowHideNotAcceptedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:onClicking('{0}');", ShowHideTextBox.ClientID));
            ShowHideNotAcceptedLinkBtn.ID = "ShowHideBtn" + data["Nric"].ToString();

            System.Web.UI.WebControls.Image ApplicantImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("ApplicantImage");
            ApplicantImage.Visible = noOfIncome;

            string r = "no";
            if (Session["ResaleSummaryRepeater"] != null)
            {
                Repeater repeater = new Repeater();
                repeater = (Repeater)Session["ResaleSummaryRepeater"];
                foreach (Control ctrl in item.Controls)
                {
                    if (ctrl is Panel)
                    {
                        foreach (Control ctrlInPanel in ctrl.Controls)
                        {
                            if (ctrlInPanel is HiddenField)
                            {
                                r = (Request.Form[((HiddenField)ctrlInPanel).UniqueID]);
                                Session["r"] = r;
                            }
                        }
                    }
                }
            }
            ShowHideTextBox.Value = !string.IsNullOrEmpty(r) ? r : "no";
            if (ShowHideTextBox.Value == "no")
                ShowHideNotAcceptedLinkBtn.Text = "Show Not Accepted List (" + noOfNotAccepted.ToString() + ")";
            else
                ShowHideNotAcceptedLinkBtn.Text = "Hide Not Accepted List (" + noOfNotAccepted.ToString() + ")";
            #endregion

            #region View Consolidated PDF
            DataTable docsDt = IncomeDb.GetDocsByDocAppIdAndNric(docAppId.Value, data["Nric"].ToString());
            string ids = string.Empty;
            if (docsDt.Rows.Count > 0)
            {
                foreach (DataRow row in docsDt.Rows)
                {
                    if (r == "yes") //&& (bool.Parse(string.IsNullOrEmpty(row["IsAccepted"].ToString()) ? false.ToString() : row["IsAccepted"].ToString())))
                    {
                        ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                    }
                    else
                    {
                        //edited by calvin change to acceptance field
                        if (string.IsNullOrEmpty(row["ImageAccepted"].ToString()) ? false : row["ImageAccepted"].ToString().Trim() == "Y")
                            ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                    }
                }
            }
            LinkButton ViewConsolidatedLinkBtn = (LinkButton)e.Item.FindControl("ViewConsolidatedLinkBtnRS");
            if (!string.IsNullOrEmpty(ids))
                ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("window.open('../Common/ConsolidatedWorkSheetPDF.aspx?id={0}&appId={1}', 'word','width=1,height=1, left=0, top=0')", ids, docAppId.Value));
            else
                ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:alert('No PDF to view')"));
            #endregion

            #region AHGIncome
            //Label AvgAHGIncome1label = (Label)e.Item.FindControl("AvgAHGIncome1labelRS");
            //Label AvgAHGIncome2label = (Label)e.Item.FindControl("AvgAHGIncome2labelRS");

            //int appPersonalId = 0;
            //AppPersonalDb appPersonalDb = new AppPersonalDb();
            //AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(data["Nric"].ToString(), docAppId.Value);
            //if (appPersonalDt.Rows.Count > 0)
            //{
            //    AppPersonal.AppPersonalRow appPersonalRow = appPersonalDt[0];
            //    appPersonalId = appPersonalRow.Id;
            //    AHGIncomeDb aHGIncomeDb = new AHGIncomeDb();
            //    AHGIncome.AHGIncomeDataTable aHGIncomeDt = aHGIncomeDb.GetAHGIncomeByAppPersonalId(appPersonalRow.Id);
            //    if (aHGIncomeDt.Rows.Count > 0)
            //    {
            //        AHGIncome.AHGIncomeRow ahgIncomeRow = aHGIncomeDt[0];

            //        AvgAHGIncome1label.Text = string.Format("{0} / {1} Months",
            //            !ahgIncomeRow.IsAHGAvgAmount1Null() ? ahgIncomeRow.AHGAvgAmount1.ToString() : " - ",
            //            !ahgIncomeRow.IsNoOfMonths1Null() ? ahgIncomeRow.NoOfMonths1.ToString() : " - ");

            //        AvgAHGIncome2label.Text = string.Format("{0} / {1} Months",
            //            !ahgIncomeRow.IsAHGAvgAmount2Null() ? ahgIncomeRow.AHGAvgAmount2.ToString() : " - ",
            //            !ahgIncomeRow.IsNoOfMonths2Null() ? ahgIncomeRow.NoOfMonths2.ToString() : " - ");
            //    }
            //    else
            //    {
            //        AvgAHGIncome1label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //        AvgAHGIncome2label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //    }
            //}
            //else
            //{
            //    AvgAHGIncome1label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //    AvgAHGIncome2label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //}


            //#region avgAHGIncomeButton
            //LinkButton AvgAHGIncome1CaptureLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome1CaptureLinkBtnRS");
            //LinkButton AvgAHGIncome2CaptureLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome2CaptureLinkBtnRS");

            //AvgAHGIncome1CaptureLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "capture1", appPersonalId);
            //AvgAHGIncome2CaptureLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "capture2", appPersonalId);

            //LinkButton AvgAHGIncome1ClearLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome1ClearLinkBtnRS");
            //LinkButton AvgAHGIncome2ClearLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome2ClearLinkBtnRS");

            //AvgAHGIncome1ClearLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "clear1", appPersonalId);
            //AvgAHGIncome2ClearLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "clear2", appPersonalId);
            //#endregion



            #endregion

            #region Populating the SummaryDocsRadGrid and DocumentsRadGrid
            RadGrid SummaryDocsRadGrid = (RadGrid)e.Item.FindControl("SummaryDocsRadGridRS");
            SummaryDocsRadGrid.ID = "SummaryDocsRadGrid" + data["Nric"].ToString();
            DataTable table = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, data["Nric"].ToString());
  
            PopulateSummaryDocs(table, SummaryDocsRadGrid);

            RadGrid DocumentsRadGrid = (RadGrid)e.Item.FindControl("DocumentsRadGridRS");
            PopulateDocumentsRadGrid(data["Nric"].ToString(), DocumentsRadGrid, ScanningTransactionTypeEnum.HLE.ToString());
            if (!HasAccess)
                DocumentsRadGrid.MasterTableView.Columns[3].Visible = false;

            Table EmployedTable = (Table)e.Item.FindControl("EmployedTableRS");

            RadMultiPage radMultiPage = (RadMultiPage)e.Item.FindControl("RadMultiPage2");
            RadTabStrip tabstrip = (RadTabStrip)e.Item.FindControl("TabStripRS");
            if (Session["Tab"] != null)
            {
                if (radMultiPage.ClientID == Session["Tab"].ToString())
                {
                    radMultiPage.SelectedIndex = 2;
                    tabstrip.SelectedIndex = 2;
                    Session["Tab"] = null;
                }

            }

            PopulateIncomeDataTab(data["Nric"].ToString(), EmployedTable, radMultiPage.ClientID);
            #endregion

            #region Setting the Assessment Period Link Button
            Label AssessmentPeriod = (Label)e.Item.FindControl("AssessmentPeriodLabelRS");
            Label MonthsToResale = (Label)e.Item.FindControl("MonthsToResaleLabel");
            int intMonthToLeas;
            if (table.Rows.Count > 0)
            {
                AssessmentPeriod.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[0]["MonthYear"].ToString(),
                    table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), table.Rows.Count.ToString(), table.Rows.Count > 1 ? "Months" : "Month");

                intMonthToLeas = !string.IsNullOrEmpty(table.Rows[0]["MonthsToLeas"].ToString()) ? int.Parse(table.Rows[0]["MonthsToLeas"].ToString()) : 0;
                if (intMonthToLeas > 0 && (intMonthToLeas <= table.Rows.Count))
                    MonthsToResale.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[(table.Rows.Count) - intMonthToLeas]["MonthYear"].ToString(),
                        table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), intMonthToLeas, intMonthToLeas > 1 ? "Months" : "Month");
                else if (intMonthToLeas > table.Rows.Count)
                {
                    MonthsToResale.Text = string.Format("({0} Months)", intMonthToLeas);
                    System.Web.UI.WebControls.Image exImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("exImage");
                    exImage.ToolTip = string.Format("Extraction Period cannot be shorter than {0} Months", intMonthToLeas);
                    exImage.Visible = true;
                    ExtractionPeriodValid = false;
                }
                else if (intMonthToLeas == 0)
                    MonthsToResale.Text = string.Format("({0} Months)", intMonthToLeas);
                else
                    MonthsToResale.Text = " - ";
            }
            else
            {
                AssessmentPeriod.Text = " - ";
                intMonthToLeas = !string.IsNullOrEmpty(data["NoOfIncomeMonths"].ToString()) ? int.Parse(data["NoOfIncomeMonths"].ToString()) : 0;
                MonthsToResale.Text = string.Format("({0}) Months", intMonthToLeas);
            }
            LinkButton AssessmentPeriodLinkBtn = (LinkButton)e.Item.FindControl("AssessmentPeriodLinkBtnRS");
            AssessmentPeriodLinkBtn.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('AssessmentPeriod.aspx?docAppId={0}&nric={1}&m={2}&ref={3}',580,350);return false;", 
                docAppId.Value.ToString(), data["Nric"].ToString(), intMonthToLeas.ToString(),"resale"));
            if (!HasAccess)
                AssessmentPeriodLinkBtn.Visible = false;
            #endregion


        }
    }
    #endregion

    #endregion

    #region FOR SALES

    #region Sales Repeater ItemDataBound 16/01/2014 By Edward BTDWMS20140114 - Changes for Sales and Resales
    protected void SalesSummaryRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;

            #region Applicant's Personal Details
            string personalType = data["ApplicantType"].ToString();

            int orderNo = int.Parse(data["OrderNo"].ToString());

            Label ApplicantLabel = (Label)e.Item.FindControl("ApplicantLabelSales");
            string applicantTypeFormat = "{0} - {1} ({2})";

            if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());
            else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + orderNo.ToString(),
                    data["Name"].ToString(), data["Nric"].ToString());

            Label DateOfBirthLabel = (Label)e.Item.FindControl("DateOfBirthLabelSales");
            DateOfBirthLabel.Text = (data["DateOfBirth"].ToString().Trim().Equals(".") ? "-" :
               Format.FormatDateTime(data["DateOfBirth"].ToString(), DateTimeFormat.dd__MMM__yyyy));

            Label EmploymentStatusLabelSales = (Label)e.Item.FindControl("EmploymentStatusLabelSales");
            EmploymentStatusLabelSales.Text = data["EmploymentType"].ToString();

            Label CitizenshipLabel = (Label)e.Item.FindControl("CitizenshipLabelSales");
            CitizenshipLabel.Text = (!string.IsNullOrEmpty(data["Citizenship"].ToString()) ? data["Citizenship"].ToString() : "-");
            #endregion

            #region Display Not accepted documents and the ApplicantImage
            int noOfNotAccepted = IncomeDb.GetNoofNotAcceptedRecordsPerNRIC(docAppId.Value, data["Nric"].ToString());
            AppPersonalDb appPersonalDb1 = new AppPersonalDb();
            AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb1.GetAppPersonalByNricAndDocAppId(data["Nric"].ToString(), docAppId.Value);
            bool noOfIncome = false;
            if (appPersonaldt.Rows.Count > 0)
            {
                AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                noOfIncome = IncomeDb.CheckIncomeCompleteByNRIC(data["Nric"].ToString(), appPersonalRow.Id);
            }

            ShowHideTextBox = (HiddenField)e.Item.FindControl("ShowHideTextSales");

            ShowHideNotAcceptedLinkBtn = (LinkButton)e.Item.FindControl("ShowHideNotAcceptedLinkBtnSales");
            ShowHideNotAcceptedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:onClicking('{0}');", ShowHideTextBox.ClientID));
            ShowHideNotAcceptedLinkBtn.ID = "ShowHideBtn" + data["Nric"].ToString();

            System.Web.UI.WebControls.Image ApplicantImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("ApplicantImage");
            ApplicantImage.Visible = noOfIncome;

            string r = "no";
            if (Session["SalesSummaryRepeater"] != null)
            {
                Repeater repeater = new Repeater();
                repeater = (Repeater)Session["SalesSummaryRepeater"];
                foreach (Control ctrl in item.Controls)
                {
                    if (ctrl is Panel)
                    {
                        foreach (Control ctrlInPanel in ctrl.Controls)
                        {
                            if (ctrlInPanel is HiddenField)
                            {
                                r = (Request.Form[((HiddenField)ctrlInPanel).UniqueID]);
                                Session["r"] = r;
                            }
                        }
                    }
                }
            }
            ShowHideTextBox.Value = !string.IsNullOrEmpty(r) ? r : "no";
            if (ShowHideTextBox.Value == "no")
                ShowHideNotAcceptedLinkBtn.Text = "Show Not Accepted List (" + noOfNotAccepted.ToString() + ")";
            else
                ShowHideNotAcceptedLinkBtn.Text = "Hide Not Accepted List (" + noOfNotAccepted.ToString() + ")";
            #endregion

            #region View Consolidated PDF
            DataTable docsDt = IncomeDb.GetDocsByDocAppIdAndNric(docAppId.Value, data["Nric"].ToString());
            string ids = string.Empty;
            if (docsDt.Rows.Count > 0)
            {
                foreach (DataRow row in docsDt.Rows)
                {
                    if (r == "yes") //&& (bool.Parse(string.IsNullOrEmpty(row["IsAccepted"].ToString()) ? false.ToString() : row["IsAccepted"].ToString())))
                    {
                        ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                    }
                    else
                    {
                        //edited by calvin change to acceptance field
                        if (string.IsNullOrEmpty(row["ImageAccepted"].ToString()) ? false : row["ImageAccepted"].ToString().Trim() == "Y")
                            ids = (String.IsNullOrEmpty(ids) ? row["DocId"].ToString() : ids + "," + row["DocId"].ToString());
                    }
                }
            }
            LinkButton ViewConsolidatedLinkBtn = (LinkButton)e.Item.FindControl("ViewConsolidatedLinkBtnSales");
            if (!string.IsNullOrEmpty(ids))
                ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("window.open('../Common/ConsolidatedWorkSheetPDF.aspx?id={0}&appId={1}', 'word','width=1,height=1, left=0, top=0')", ids, docAppId.Value));
            else
                ViewConsolidatedLinkBtn.Attributes.Add("OnClick", string.Format("javascript:alert('No PDF to view')"));
            #endregion

            #region AHGIncome
            //Label AvgAHGIncome1label = (Label)e.Item.FindControl("AvgAHGIncome1labelSales");
            //Label AvgAHGIncome2label = (Label)e.Item.FindControl("AvgAHGIncome2labelSales");

            //int appPersonalId = 0;
            //AppPersonalDb appPersonalDb = new AppPersonalDb();
            //AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(data["Nric"].ToString(), docAppId.Value);
            //if (appPersonalDt.Rows.Count > 0)
            //{
            //    AppPersonal.AppPersonalRow appPersonalRow = appPersonalDt[0];
            //    appPersonalId = appPersonalRow.Id;
            //    AHGIncomeDb aHGIncomeDb = new AHGIncomeDb();
            //    AHGIncome.AHGIncomeDataTable aHGIncomeDt = aHGIncomeDb.GetAHGIncomeByAppPersonalId(appPersonalRow.Id);
            //    if (aHGIncomeDt.Rows.Count > 0)
            //    {
            //        AHGIncome.AHGIncomeRow ahgIncomeRow = aHGIncomeDt[0];

            //        AvgAHGIncome1label.Text = string.Format("{0} / {1} Months",
            //            !ahgIncomeRow.IsAHGAvgAmount1Null() ? ahgIncomeRow.AHGAvgAmount1.ToString() : " - ",
            //            !ahgIncomeRow.IsNoOfMonths1Null() ? ahgIncomeRow.NoOfMonths1.ToString() : " - ");

            //        AvgAHGIncome2label.Text = string.Format("{0} / {1} Months",
            //            !ahgIncomeRow.IsAHGAvgAmount2Null() ? ahgIncomeRow.AHGAvgAmount2.ToString() : " - ",
            //            !ahgIncomeRow.IsNoOfMonths2Null() ? ahgIncomeRow.NoOfMonths2.ToString() : " - ");
            //    }
            //    else
            //    {
            //        AvgAHGIncome1label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //        AvgAHGIncome2label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //    }
            //}
            //else
            //{
            //    AvgAHGIncome1label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //    AvgAHGIncome2label.Text = string.Format("{0} / {1} Months", " - ", " - ");
            //}


            //#region avgAHGIncomeButton
            //LinkButton AvgAHGIncome1CaptureLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome1CaptureLinkBtnSales");
            //LinkButton AvgAHGIncome2CaptureLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome2CaptureLinkBtnSales");

            //AvgAHGIncome1CaptureLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "capture1", appPersonalId);
            //AvgAHGIncome2CaptureLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "capture2", appPersonalId);

            //LinkButton AvgAHGIncome1ClearLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome1ClearLinkBtnSales");
            //LinkButton AvgAHGIncome2ClearLinkBtn = (LinkButton)e.Item.FindControl("AvgAHGIncome2ClearLinkBtnSales");

            //AvgAHGIncome1ClearLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "clear1", appPersonalId);
            //AvgAHGIncome2ClearLinkBtn.OnClientClick = string.Format("javascript:UpdateParentPage1('{0}{1}'); return false;", "clear2", appPersonalId);
            //#endregion



            #endregion

            #region Populating the SummaryDocsRadGrid and DocumentsRadGrid
            RadGrid SummaryDocsRadGridSales = (RadGrid)e.Item.FindControl("SummaryDocsRadGridSales");
            SummaryDocsRadGridSales.ID = "SummaryDocsRadGrid" + data["Nric"].ToString();
            DataTable table = IncomeDb.GetDataForIncomeAssessment(docAppId.Value, data["Nric"].ToString());

            PopulateSummaryDocs(table, SummaryDocsRadGridSales);

            RadGrid DocumentsRadGrid = (RadGrid)e.Item.FindControl("DocumentsRadGridSales");
            PopulateDocumentsRadGrid(data["Nric"].ToString(), DocumentsRadGrid, ScanningTransactionTypeEnum.HLE.ToString());
            if (!HasAccess)
                DocumentsRadGrid.MasterTableView.Columns[3].Visible = false;

            Table EmployedTable = (Table)e.Item.FindControl("EmployedTableSales");

            RadMultiPage radMultiPage = (RadMultiPage)e.Item.FindControl("RadMultiPage3");
            RadTabStrip tabstrip = (RadTabStrip)e.Item.FindControl("TabStripSales");
            if (Session["Tab"] != null)
            {
                if (radMultiPage.ClientID == Session["Tab"].ToString())
                {
                    radMultiPage.SelectedIndex = 2;
                    tabstrip.SelectedIndex = 2;
                    Session["Tab"] = null;
                }

            }

            PopulateIncomeDataTab(data["Nric"].ToString(), EmployedTable, radMultiPage.ClientID);
            #endregion

            #region Setting the Assessment Period Link Button
            Label AssessmentPeriod = (Label)e.Item.FindControl("AssessmentPeriodLabelSales");
            Label MonthsToSalesLabel = (Label)e.Item.FindControl("MonthsToSalesLabel");
            int intMonthToLeas;
            if (table.Rows.Count > 0)
            {
                AssessmentPeriod.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[0]["MonthYear"].ToString(),
                    table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), table.Rows.Count.ToString(), table.Rows.Count > 1 ? "Months" : "Month");

                intMonthToLeas = !string.IsNullOrEmpty(table.Rows[0]["MonthsToLeas"].ToString()) ? int.Parse(table.Rows[0]["MonthsToLeas"].ToString()) : 0;
                if (intMonthToLeas > 0 && (intMonthToLeas <= table.Rows.Count))
                    MonthsToSalesLabel.Text = string.Format("{0} to {1} ({2} {3})", table.Rows[(table.Rows.Count) - intMonthToLeas]["MonthYear"].ToString(),
                        table.Rows[table.Rows.Count - 1]["MonthYear"].ToString(), intMonthToLeas, intMonthToLeas > 1 ? "Months" : "Month");
                else if (intMonthToLeas > table.Rows.Count)
                {
                    MonthsToSalesLabel.Text = string.Format("({0} Months)", intMonthToLeas);
                    System.Web.UI.WebControls.Image exImage = (System.Web.UI.WebControls.Image)e.Item.FindControl("exImage");
                    exImage.ToolTip = string.Format("Extraction Period cannot be shorter than {0} Months", intMonthToLeas);
                    exImage.Visible = true;
                    ExtractionPeriodValid = false;
                }
                else if (intMonthToLeas == 0)
                    MonthsToSalesLabel.Text = string.Format("({0} Months)", intMonthToLeas);
                else
                    MonthsToSalesLabel.Text = " - ";
            }
            else
            {
                AssessmentPeriod.Text = " - ";
                intMonthToLeas = !string.IsNullOrEmpty(data["NoOfIncomeMonths"].ToString()) ? int.Parse(data["NoOfIncomeMonths"].ToString()) : 0;
                MonthsToSalesLabel.Text = string.Format("({0}) Months", intMonthToLeas);
            }
            LinkButton AssessmentPeriodLinkBtn = (LinkButton)e.Item.FindControl("AssessmentPeriodLinkBtnSales");
            AssessmentPeriodLinkBtn.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('AssessmentPeriod.aspx?docAppId={0}&nric={1}&m={2}&ref={3}',580,350);return false;",
                docAppId.Value.ToString(), data["Nric"].ToString(), intMonthToLeas.ToString(), "sales"));
            if (!HasAccess)
                AssessmentPeriodLinkBtn.Visible = false;
            #endregion
        }

    }

    #endregion
    
    #endregion

    private void PopulateSummaryDocs(DataTable table, RadGrid summaryDocsRadGrid)
    {                               
        summaryDocsRadGrid.DataSource = table;
        summaryDocsRadGrid.DataBind();
    }

    #region Summary/Pending Doc Rad Grid
    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    protected void lblDocRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            RepeaterItem item = (RepeaterItem)e.Item;
            DataRowView data = (DataRowView)item.DataItem;
            HyperLink lbl = (HyperLink)e.Item.FindControl("lblDoc");
            //edited by calvin change to acceptance field
            //bool isAccepted = false;
            //if (data["ImageAccepted"].ToString() == "Y")
            //{
                lbl.Text = data["Description"].ToString() + " | " + (data["ImageAccepted"].ToString().Trim() == "Y" ? "Accepted" : "Not Accepted") + " | " + data["DocId"].ToString();
                if (data["ImageAccepted"].ToString().Trim() == "Y")
                    #region Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                    //lbl.NavigateUrl = string.Format("~/IncomeAssessment/ZoningPage.aspx?id={0}&nric={1}&apppersonal={2}&incomeid={3}&appdocref={4}", docAppId.Value.ToString(), data["Nric"].ToString(), 
                    //    data["AppPersonalId"].ToString(), data["IncomeId"].ToString(), data["AppDocRefId"].ToString());
                    lbl.NavigateUrl = string.Format("~/IncomeAssessment/ZoneDefault.aspx?id={0}&nric={1}&apppersonal={2}&incomeid={3}&appdocref={4}", docAppId.Value.ToString(), data["Nric"].ToString(), 
                        data["AppPersonalId"].ToString(), data["IncomeId"].ToString(), data["AppDocRefId"].ToString());
                    #endregion
                else
                    lbl.NavigateUrl = String.Format("~/Common/DownloadDocument.aspx?id={0}&appId={1}", data["DocId"].ToString(), docAppId.Value);
            //}
        }
    }

    #endregion

    #region Private Methods

    protected void PopulateDateDropDown (DropDownList dropDownList)
    {
        dropDownList.Items.Clear();

        DateTime runnningDate = new DateTime();
        runnningDate = DateTime.Now.AddMonths(-24);

        while (!(runnningDate.Year == DateTime.Now.Year && runnningDate.Month == DateTime.Now.Month))
        {
            dropDownList.Items.Add(string.Format("{0:MMM}",runnningDate) + " " + runnningDate.Year.ToString());
            runnningDate = runnningDate.AddMonths(1);
        }
        
        dropDownList.DataBind();
    }

    protected void PopulateRefInfo()
    {
        DocAppDb docAppDb = new DocAppDb();
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);

        if (docApps.Rows.Count > 0)
        {
            DocApp.DocAppRow docAppRow = docApps[0];    
            string refType = docAppRow.RefType;
            string status = docAppRow.Status;
            TitleLabel.Text = string.Format("{0} : {1} ({2})",   refType , docAppRow.RefNo, docAppRow.AssessmentStatus.Trim().Replace("_"," "));
            CompletenessHyperLink.NavigateUrl = "~/Completeness/View.aspx?id=" + docAppId.Value.ToString();            
            
            #region Added By Edward 13/3/2014 Sales and Resale Changes
            if (docAppRow["RefType"].ToString().ToUpper().Trim() == "HLE")
                PopulateHLERefInfo(docAppRow["RefNo"].ToString(), Format.FormatDateTime(docAppRow.AssessmentDateIn, DateTimeFormat.ddd_C_d__MMM__yyyy),
                    !docAppRow.IsNull("SecondCA") ? (bool.Parse(docAppRow["SecondCA"].ToString()) == true ? "Yes" : "No") : "No");
            else if (docAppRow["RefType"].ToString().ToUpper().Trim() == "SALES")
                PopulateSalesRefInfo(docAppRow["RefNo"].ToString());
            else if (docAppRow["RefType"].ToString().ToUpper().Trim() == "RESALE")
                PopulateResaleRefInfo(docAppRow["RefNo"].ToString());
            #endregion
        }
    }

    #region Added By Edward 13/3/2014 To Populate Info in Application level Sales and Resale Changes
    protected void PopulateHLERefInfo(string refNo, string applicationDate, string secondCA)
    {
        HleInterfaceDb db = new HleInterfaceDb();
        HleInterface.HleInterfaceDataTable dt = db.GetHleInterfaceByRefNo(refNo);
        if (dt.Rows.Count > 0)
        {
            TableRow tRow;
            TableCell tCell;
            Label lbl;
            HleInterface.HleInterfaceRow r = dt[0];

            #region Row 1 Date of Application , SecondCA
            tRow = new TableRow();
            tCell = new TableCell();
            lbl = new Label();
            lbl.ID = "lblDateOfApplication";
            lbl.Text = "Date of Application: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "DateOfApplicationLabel";
            lbl.Text = applicationDate;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            
            tCell = new TableCell();
            lbl = new Label();
            lbl.ID = "lblSecondCA";
            lbl.Text = "Second CA: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "SecondCALabel";
            lbl.Text = secondCA;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tblRefInfo.Rows.Add(tRow);
            #endregion

        }
    }

    protected void PopulateSalesRefInfo(string refNo)
    {
        SalesInterfaceDb db = new SalesInterfaceDb();
        SalesInterface.SalesInterfaceDataTable dt = db.GetSalesInterfaceByRefNo(refNo);
        if (dt.Rows.Count > 0)
        {
            TableRow tRow;
            TableCell tCell;
            Label lbl;
            SalesInterface.SalesInterfaceRow r = dt[0];

            #region Row 1 Date of Application , Ballot Quarter
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblDateOfApplication";
            lbl.Text = "Date of Application: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "DateOfApplicationLabel";
            lbl.Text = !r.IsApplicationDateNull() ? Format.FormatDateTime(r.ApplicationDate, DateTimeFormat.ddd_C_d__MMM__yyyy) : string.Empty;            
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblBallotQuarter";
            lbl.Text = "Ballot Quarter: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "BallotQuarterLabel";
            lbl.Text = !r.IsBallotQuarterNull() ? r.BallotQuarter : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tblRefInfo.Rows.Add(tRow);
            #endregion 

            #region Row 2 Eligibility Scheme , Allocation Scheme
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblEligibilityScheme";
            lbl.Text = "Eligibility Scheme: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "EligibilitySchemeLabel";
            lbl.Text = !r.IsEligibilitySchemeNull() ? r.EligibilityScheme : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblAllocationScheme";
            lbl.Text = "Allocation Scheme: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "AllocationSchemeLabel";
            lbl.Text = !r.IsAllocationSchemeNull() ? r.AllocationScheme : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tblRefInfo.Rows.Add(tRow);
            #endregion 

            #region Row 3 Household Type , Flat Type
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblHouseholdType";
            lbl.Text = "Household Type: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "HouseholdTypeLabel";
            lbl.Text = !r.IsHouseholdTypeNull() ? r.HouseholdType : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblFlatType";
            lbl.Text = "Flat Type: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "FlatTypeLabel";
            lbl.Text = !r.IsFlatTypeNull() ? r.FlatType : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tblRefInfo.Rows.Add(tRow);
            #endregion 

        }
    }

    protected void PopulateResaleRefInfo(string refNo)
    {
        ResaleInterfaceDb db = new ResaleInterfaceDb();
        ResaleInterface.ResaleInterfaceDataTable dt = db.GetResaleInterfaceByRefNo(refNo);
        if (dt.Rows.Count > 0)
        {
            TableRow tRow;
            TableCell tCell;
            Label lbl;
            ResaleInterface.ResaleInterfaceRow r = dt[0];

            #region Row 1 Date of Application , Eligibility Scheme, Household Type
            tRow = new TableRow();
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblDateOfApplication";
            lbl.Text = "Date of Application: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "DateOfApplicationLabel";
            lbl.Text = !r.IsApplicationDateNull() ? Format.FormatDateTime(r.ApplicationDate, DateTimeFormat.ddd_C_d__MMM__yyyy) : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblHouseholdType";
            lbl.Text = "Household Type: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "HouseholdTypeLabel";
            lbl.Text = !r.IsHouseholdTypeNull() ? r.HouseholdType : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tblRefInfo.Rows.Add(tRow);

            tRow = new TableRow();
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblEligibilityScheme";
            lbl.Text = "Eligibility Scheme: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "EligibilitySchemeLabel";
            lbl.Text = !r.IsEligBuyerNull() ? r.EligBuyer : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "lblAllocationScheme";
            lbl.Text = "Allocation Scheme: ";
            lbl.CssClass = "label";
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);
            tCell = new TableCell();
            tCell.Width = new Unit(200);
            lbl = new Label();
            lbl.ID = "AllocationSchemeLabel";
            lbl.Text = !r.IsAllocBuyerNull() ? r.AllocBuyer : string.Empty;
            tCell.Controls.Add(lbl);
            tRow.Cells.Add(tCell);

            tblRefInfo.Rows.Add(tRow);

            
            #endregion 
        }
    }

    #endregion
    #endregion


    private void PopulateDocumentsRadGrid(string nric, RadGrid DocumentsRadGrid, string referenceType)
    {
        DocDb docDb = new DocDb();
        DocumentsRadGrid.DataSource = docDb.GetDocForSummaryIncomeExtraction(docAppId.Value, nric, true, referenceType);
        DocumentsRadGrid.DataBind();
    }


    private void PopulateIncomeDataTab(string nric, Table EmployedTable, string tab)
    {

        try
        {                        
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(nric, docAppId.Value);            
            if (appPersonalDt.Rows.Count > 0)
            {
                AppPersonal.AppPersonalRow appPersonalRow = appPersonalDt[0];
                
                DataTable test = IncomeDb.CreateIncomeDataTable(appPersonalDt, docAppId.Value, nric);

                TableRow EmployedRow = new TableRow();
                foreach (DataColumn testc in test.Columns)
                {
                    CreateHeaderCell(testc.ColumnName, EmployedRow, tab, appPersonalRow.Id);
                }
                EmployedTable.Rows.Add(EmployedRow);

                foreach (DataRow testr in test.Rows)
                {
                    EmployedRow = new TableRow();
                    for (int i = 0; i < test.Columns.Count; i++)
                    {
                        CreateCell(testr[i].ToString(), EmployedRow);
                    }
                    EmployedTable.Rows.Add(EmployedRow);
                }
            }            
        }
        catch (Exception ex)
        {
            InfoLabel.Text = string.Format("{0} (PopulateEmployedAllowanceTable for {1})", ex.Message, nric);
            WarningPanel.Visible = true;            
        }
           
    }


    private void CreateCreditAssessmentCell(string text, TableRow row, int appPersonalId, string component, string typeofItem, int id, string tab)
    {
        TableCell cell = new TableCell();
        cell.Text = text;
        if (text.Contains("NA"))
            cell.ForeColor = Color.Red;
        cell.BorderWidth = new Unit(1);
        cell.Height = new Unit(40);
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        row.Cells.Add(cell);
    }

    private void CreateCell(string text, TableRow row)
    {
        TableCell cell = new TableCell();
        cell.Text = text;
        cell.BorderWidth = new Unit(1);
        cell.Height = new Unit(40);        
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        row.Cells.Add(cell);
    }

    private void CreateHeaders(TableRow EmployedTableRowHeading, string tab, int appPersonalId, string nric, int intMonthToLEAS)
    {
        //DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId.Value, nric, "DESC");

        //CreateCreditAssessmentHeaderCell("Credit Assessment", EmployedTableRowHeading, tab, appPersonalId);        
        //CreateHeaderCell(string.Format("Avg in past {0} mths",IncomeDt.Rows.Count < 3 ? IncomeDt.Rows.Count.ToString() : "3"), EmployedTableRowHeading);
        ////CreateHeaderCell(string.Format("Avg in past {0} mths", IncomeDt.Rows.Count < 12 ? IncomeDt.Rows.Count.ToString() : "12"), EmployedTableRowHeading);
        ////CreateHeaderCell(string.Format("Lowest in past {0} mths",IncomeDt.Rows.Count < 12 ? IncomeDt.Rows.Count.ToString() : "12"), EmployedTableRowHeading);
        //CreateHeaderCell(string.Format("Avg in past {0} mths", intMonthToLEAS), EmployedTableRowHeading);
        //CreateHeaderCell(string.Format("Lowest in past {0} mths",intMonthToLEAS), EmployedTableRowHeading);
        //CreateHeaderCell("Yes/No", EmployedTableRowHeading);
    }


    private void CreateHeaderCell(string text, TableRow row, string tab, int appPersonalId)
    {
        TableCell cell = new TableCell();
        if (text.Contains("IncomeComponent"))
            cell.Text = "Income Component";
        else if (text.Contains("TypeOfIncome"))
            cell.Text = "Type of Income";
        else if (text.Contains("CreditAssessment"))
            CreateCreditAssessmentHeaderCell("Credit Assessment",cell,  tab, appPersonalId);            
        else if (text.Contains("AvgMonths"))
            cell.Text = string.Format("Avg in {0} mnths", text.Substring(text.IndexOf("_") + 1));
        else if (text.Contains("MonthsToPass"))
            cell.Text = string.Format("Avg in {0} mnths", text.Substring(text.IndexOf("_") + 1));
        else if (text.Contains("Lowest"))
            cell.Text = string.Format("Lowest in past {0} mnths", text.Substring(text.IndexOf("_") + 1));
        else if (text.Contains("YesNo"))
            cell.Text = "Yes/No";
        else
            cell.Text = text;        
        cell.BorderWidth = new Unit(1);
        cell.Height = new Unit(40);
        cell.BackColor = Color.LightGray;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        //cell.Width = new Unit(100);
        cell.HorizontalAlign = HorizontalAlign.Center;
        row.Cells.Add(cell);
    }

    private void CreateCreditAssessmentHeaderCell(string text, TableCell cell,  string tab, int appPersonalId)
    {        
        Label lbl = new Label();
        lbl.Text = text + " ";
        cell.Controls.Add(lbl);

        #region Setting the Edit Credit Assessment Link
        if (HasAccess)
        {
            LinkButton hp = new LinkButton();
            hp.Text = "(Edit)";
            hp.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('CreditAssessment.aspx?tab={0}&appPersonalId={1}&docappid={2}',600,400);return false;", tab, appPersonalId,docAppId));
            hp.Attributes.Add("cursor", "pointer");
            cell.Controls.Add(hp);
        }
        #endregion

        cell.BorderWidth = new Unit(1);
        cell.Height = new Unit(40);
        cell.BackColor = Color.LightGray;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        //cell.Width = new Unit(100);
        cell.HorizontalAlign = HorizontalAlign.Center;
        
    }

    #region ConfirmButton Click
    protected void ConfirmButton_Click(object sender, EventArgs e)
    {
        try
        {
            string errormsg = string.Empty;
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);
            bool CanConfirm = true;
            int intAppPersonalId = 0;
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                string refType = docAppRow.RefType.ToUpper().Trim();
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    #region HLE
                    HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                    HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                    if (hleInterfaceDt.Rows.Count > 0)
                    {
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {                            
                            AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(hleInterfaceRow.Nric, docAppId.Value);                        
                            if (appPersonaldt.Rows.Count > 0)
                            {
                                AppPersonal.AppPersonalRow r = appPersonaldt[0]; 
                                intAppPersonalId = r.Id;
                            }
                            //if (!hleInterfaceRow.EmploymentType.ToUpper().Trim().Equals("UNEMPLOYED"))
                            //{
                                CanConfirm = IncomeDb.CheckIncomeCompleteByNRIC(hleInterfaceRow.Nric, intAppPersonalId);
                                if (!CanConfirm)
                                {
                                    errormsg = string.Format("Please complete Income for {0}.", hleInterfaceRow.Nric);
                                    break;
                                }
                                //CanConfirm = IncomeDb.CheckCreditAssessmentByAppPersonalId(intAppPersonalId);
                                //if (!CanConfirm)
                                //{
                                //    errormsg = string.Format("Please complete Credit Assessment for {0} .", hleInterfaceRow.Nric);
                                //    break;
                                //}
                            //}
                        }
                        if (!ExtractionPeriodValid)
                        {
                            errormsg = string.Format("Please check all extraction periods if they are valid.");
                            CanConfirm = false;
                        }
                        if (CanConfirm)
                            ExtractApplication();
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Confirm", string.Format("javascript:alert('{0} {1}');", "Please complete Income Extraction for all roles.", errormsg), true);
                    }
                    #endregion
                }
                else if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))
                {
                    #region RESALE
                    ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                    ResaleInterface.ResaleInterfaceDataTable resaleInterfaceDt = resaleInterfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);

                    if (resaleInterfaceDt.Rows.Count > 0)
                    {
                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {
                            AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(resaleInterfaceRow.Nric, docAppId.Value);
                            if (appPersonaldt.Rows.Count > 0)
                            {
                                AppPersonal.AppPersonalRow r = appPersonaldt[0];
                                intAppPersonalId = r.Id;
                            }                            
                            CanConfirm = IncomeDb.CheckIncomeCompleteByNRIC(resaleInterfaceRow.Nric, intAppPersonalId);
                            if (!CanConfirm)
                            {
                                errormsg = string.Format("Please complete Income for {0}.", resaleInterfaceRow.Nric);
                                break;
                            }                                                            
                        }                        
                        if (CanConfirm)
                            ExtractApplication();
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Confirm", string.Format("javascript:alert('{0}');", "Please enter all Income Extraction to continue."), true);
                    }
                    #endregion
                }
                else if (refType.Contains(ReferenceTypeEnum.SALES.ToString()))
                {
                    #region SALES
                    SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                    SalesInterface.SalesInterfaceDataTable salesInterfaceDt = salesInterfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);

                    if (salesInterfaceDt.Rows.Count > 0)
                    {
                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {
                            AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(salesInterfaceRow.Nric, docAppId.Value);
                            if (appPersonaldt.Rows.Count > 0)
                            {
                                AppPersonal.AppPersonalRow r = appPersonaldt[0];
                                intAppPersonalId = r.Id;
                            }

                            CanConfirm = IncomeDb.CheckIncomeCompleteByNRIC(salesInterfaceRow.Nric, intAppPersonalId);
                            if (!CanConfirm)
                            {
                                errormsg = string.Format("Please complete Income for {0}.", salesInterfaceRow.Nric);
                                break;
                            }                   
                        }
                        if (CanConfirm)
                            ExtractApplication();
                        else
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Confirm", string.Format("javascript:alert('{0}');", "Please enter all Income Extraction to continue."), true);
                    }
                    #endregion
                }
            }
        }
        catch (Exception ex)
        {
            InfoLabel.Text = ex.Message + " (ConfirmButton_Click) ";
            WarningPanel.Visible = true;  
        }
        
    }

    private void ExtractApplication()
    {
        DocAppDb docAppDb = new DocAppDb();                
        docAppDb.UpdateRefStatusIA(docAppId.Value, AssessmentStatusEnum.Extracted, true, false, LogActionEnum.Confirmed_application);

        #region Added By Edward  24/02/2014 Add Icon and Action Log
        DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId.Value);
        string RefType = string.Empty;
        if (docApps.Rows.Count > 0)
        {
            DocApp.DocAppRow docAppRow = docApps[0];
            RefType = docAppRow.RefType.ToUpper().Trim();
        }
        if(RefType.Contains(ReferenceTypeEnum.HLE.ToString()))          //Added Condition by Edward 24/02/2014 Add Icon and Action Log
        {
            docAppDb.UpdateSendToLeasStatus(docAppId.Value, SendToLEASStatusEnum.Ready);
            docAppDb.UpdateSendToLeasAttemptCount(docAppId.Value, 0);
        }
        else if (RefType.Contains(ReferenceTypeEnum.RESALE.ToString())) //Added by Edward 2014/5/23 Changes for Sales and Resale
        {
            //docAppDb.UpdateSendToResaleStatus(docAppId.Value, SendToLEASStatusEnum.Ready);
            //docAppDb.UpdateSendToResaleAttemptCount(docAppId.Value, 0);
        }
        #endregion

        HousingGrantButton.Text = "Extraction Worksheet";        
        SetConfirmLabel.Text = Constants.RefExtracted;
        ConfirmButton.Visible = false;
        ConfirmPanel.Visible = true;
        #region Added By Edward 24/02/2014 Add Icon and Action Log
        IncomeDb.InsertExtractionLog(docAppId.Value,string.Empty,string.Empty,
                LogActionEnum.Confirmed_Income_Extraction, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
        #endregion
        PopulateRefInfo();
        PopulatePersonals();    //Modified by Edward 2015/11/18 Change condition of access of updating doc acceptance
    }
    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Drawing;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Text;

public partial class IncomeAssessment_ZoningPage : System.Web.UI.Page
{
    int? ReqDocAppId;
    string ReqNRIC;
    int? ReqAppDocRefId;
    int? ReqAppPersonalId;
    /// <summary>
    /// Variable that contains the Request["incomeid"]
    /// </summary>
    int? ReqIncomeId;
    int incomeVersionId;
    int docId;
    
    List<int> listDocId;
    int DocIdCount;
    string str_Section;

    List<string> arrClientIdofMonthTable;
    List<string> arrClientIdForex;
    List<string> arrIncomeId;

    protected void Page_Load(object sender, EventArgs e)
    {              
        if (!string.IsNullOrEmpty(Request["id"]) && !string.IsNullOrEmpty(Request["nric"]) )
        {
            ReqDocAppId = int.Parse(Request["id"]);
            ReqNRIC = Request["nric"].ToString();
            ReqAppDocRefId = int.Parse(Request["appdocref"]);

            #region Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
            //ReqIncomeId = int.Parse(Request["incomeid"]);

            if (Session["ReqIncomeId"] == null)
            {
                ReqIncomeId = int.Parse(Request["incomeid"]);
                Session["ReqIncomeId"] = ReqIncomeId;
            }
            else
                ReqIncomeId = int.Parse(Session["ReqIncomeId"].ToString());            

            if (string.IsNullOrEmpty(StoreIncomeId.Value))
                StoreIncomeId.Value = ReqIncomeId.Value.ToString();
            #endregion

            ReqAppPersonalId = int.Parse(Request["apppersonal"]);            
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        DocIdCount = PopulateTraversing();
        if (DocIdCount == 0)
        {
            PreviousButton.Enabled = false;
            NextButton.Enabled = false;
        }

        if (!IsPostBack)
        {            
            ChangeDocAppAssessmentStatus();
            PopulateApplicantDetails();
            PopulateVersion();
            GetIncomeVersion();
            //GetIncomeIdIncomeVersion();
            PopulateTemplate();
            GetDocIdByAppDocRefId();        
            DisplayPdfDocument();
            ViewState["CurrentDocId"] = null;
            if (ViewState["CurrentDocId"] == null)
                ViewState["CurrentDocId"] = listDocId.IndexOf(docId);                  
        }
        CheckUserSection();   
        DisableZoning();
        arrIncomeId = new List<string>();
        arrClientIdofMonthTable = new List<string>();
        arrClientIdForex = new List<string>();
        PopulateTableMonthRepeater();
        AddAttributesForButtons();
        PopulateHiddenFields();       
    }
    
    #region Zoning Page Permissions
    private void DisableZoning()
    {
        try
        {
            if (ReqDocAppId.HasValue)
            {
                DocAppDb docAppDb = new DocAppDb();
                DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(ReqDocAppId.Value);

                if (docApps.Rows.Count > 0)
                {
                    DocApp.DocAppRow docAppRow = docApps[0];
                    MembershipUser user = Membership.GetUser();
                    Guid currentUserId = (Guid)user.ProviderUserKey;
                    SetAccessControl(docAppRow.IsAssessmentStaffUserIdNull() ? Guid.Empty : docAppRow.AssessmentStaffUserId, docAppRow.AssessmentStatus.Trim());
                }
            }
        }
        catch (Exception ex)
        {            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in disable zoning page", ex.Message), true);
        }
        
    }

    private void SetAccessControl(Guid? assessmentStaffUserId, string status)
    {
        MembershipUser user = Membership.GetUser();
        Guid currentUserId = (Guid)user.ProviderUserKey;

        if (assessmentStaffUserId == Guid.Empty || currentUserId != assessmentStaffUserId)        
            DisableButtons();        

        #region Check if Extracted or Closed then ConfirmButton is invisible. Also sets the Label of the Worksheet Button to Download
        if (status.Equals(AssessmentStatusEnum.Extracted.ToString()) ||
            status.Equals(AssessmentStatusEnum.Closed.ToString()))
        {
            DisableButtons();
        }
        #endregion
    }

    private void DisableButtons()
    {
        CopyButton.Enabled = false;
        CopyButton.ToolTip = "Not allowed to zone.";
        CopyButton2.Enabled = false;
        CopyButton2.ToolTip = "Not allowed to zone.";
        ArrowDivideButton.Enabled = false;
        ArrowDivideButton.ToolTip = "Not allowed to zone.";
        ArrowDivideButton2.Enabled = false;
        ArrowDivideButton2.ToolTip = "Not allowed to zone.";
        DeleteTemp.Enabled = false;
        DeleteTemp.ToolTip = "Not allowed to zone.";
        DeleteTemp2.Enabled = false;
        DeleteTemp2.ToolTip = "Not allowed to zone.";
        IncomeBlank.Enabled = false;
        IncomeBlank.ToolTip = "Not allowed to zone.";
        IncomeBlank2.Enabled = false;
        IncomeBlank2.ToolTip = "Not allowed to zone.";
        HistoryButton.Enabled = false;
        HistoryButton.ToolTip = "Not allowed to zone.";
        HistoryButton2.Enabled = false;
        HistoryButton2.ToolTip = "Not allowed to zone.";
        SaveButton.Enabled = false;
        SaveButton.ToolTip = "Not allowed to zone.";
        SaveButton2.Enabled = false;
        SaveButton2.ToolTip = "Not allowed to zone.";
        VersionButton.Enabled = false;
        VersionButton.ToolTip = "Not allowed to zone.";
        VersionButton2.Enabled = false;
        VersionButton2.ToolTip = "Not allowed to zone.";
    }

    #endregion
   


    #region Populates the Applicant details
    /// <summary>
    /// Populates the Applicant Format, the Name, NRIC and Order No.
    /// </summary>
    private void PopulateApplicantDetails()
    {
        try
        {
            DataTable dt = IncomeDb.GetApplicantDetails(ReqDocAppId.Value, ReqNRIC);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string personalType = row["PersonalType"].ToString();
                string applicantTypeFormat = "{0} - {1} ({2})";
                if (personalType.ToLower().Equals(PersonalTypeEnum.HA.ToString().ToLower()))
                    ApplicantLabel.Text = String.Format(applicantTypeFormat, "Applicant " + row["OrderNo"].ToString(), row["Name"].ToString(), row["Nric"].ToString());
                else if (personalType.ToLower().Equals(PersonalTypeEnum.OC.ToString().ToLower()))
                    ApplicantLabel.Text = String.Format(applicantTypeFormat, "Occupier " + row["OrderNo"].ToString(), row["Name"].ToString(), row["Nric"].ToString());
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Getting Applicant Details", ex.Message), true);           
        }
        
    }
    #endregion
    

    #region Populates the Version Combo Box based on the Selected MonthYear combo box
    
    private void PopulateTemplate()
    {
        try
        {
            TemplateDropdownList.Items.Clear();            
            RadComboBoxItem blank = new RadComboBoxItem("- Template - ", "tem");
            TemplateDropdownList.Items.Add(blank);

            IncomeTemplateDb incomeTemplateDb = new IncomeTemplateDb();
            IncomeTemplate.IncomeTemplateDataTable incomeTemplateDataTable = incomeTemplateDb.GetIncomeTemplates();

            if (incomeTemplateDataTable.Rows.Count > 0)
            {
                foreach (IncomeTemplate.IncomeTemplateRow row in incomeTemplateDataTable)
                {
                    RadComboBoxItem newTemplate = new RadComboBoxItem(string.Format("- {0} -", row.Name), row.Name);
                    TemplateDropdownList.Items.Add(newTemplate);
                }
            }            
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in getting templates", ex.Message), true);
        }

    }

    #endregion

    private DataTable AddDataTableColumns(DataTable dt)
    {
        dt.Columns.Add("Id");
        dt.Columns.Add("IncomeVersionId");
        dt.Columns.Add("IncomeItem");
        dt.Columns.Add("IncomeAmount");
        dt.Columns.Add("Allowance");
        dt.Columns.Add("CPFIncome");
        dt.Columns.Add("GrossIncome");
        dt.Columns.Add("AHGIncome");
        dt.Columns.Add("Overtime");
        return dt;
    }

    private DataTable SelectIncomeTemplate(string template)
    {
        DataTable dt = new DataTable();
        try
        {
            dt = AddDataTableColumns(dt);
            dt = IncomeDb.CreateIncomeTemplate(dt, template);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }
        return dt;
    }
 
    #region Inserts in to the ViewLog Table whenever DisplayPDFDocument Method triggers
    /// <summary>
    /// Inserts into View Log table each time the DisplayPdfDocument is called.
    /// </summary>   

    private void InsertIntoViewLog(int IncomeId)
    {
        Guid? userId = null;
        Guid roleId = Guid.Empty;
        MembershipUser user = Membership.GetUser();
        userId = (Guid)user.ProviderUserKey;

        ViewLogDb viewLogDb = new ViewLogDb();
        viewLogDb.Insert(userId.Value, ReqAppDocRefId.Value, IncomeId);
    }
    #endregion

    /// <summary>
    /// Gets the docId to be used in the DisplayPdfDocument Method.
    /// </summary>
    private void GetDocIdByAppDocRefId()
    {
        try
        {
            AppDocRefDb e = new AppDocRefDb();
            AppDocRef.AppDocRefDataTable dt = e.GetAppDocRefById(ReqAppDocRefId.Value);
            if (dt.Rows.Count > 0)
            {
                AppDocRef.AppDocRefRow row = dt[0];
                docId = row.DocId;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Getting Docs", ex.Message), true);
        }
        

        
    }

    #region Displays the PDF Document Method
    /// <summary>
    /// Displas the PDF Document depending on the docID. 
    /// </summary>
    private void DisplayPdfDocument()
    {
        //GetDocIdByAppDocRefId();
        //InsertIntoViewLog(ReqIncomeId.Value);        Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
        InsertIntoViewLog(int.Parse(StoreIncomeId.Value));  
        int missingPages = 0;
        try
        {
            #region modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

            //ArrayList pages = new ArrayList();

            //RawPageDb rawPageDb = new RawPageDb();
            //RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

            //for (int cnt = 0; cnt < rawPages.Count; cnt++)
            //{
            //    RawPage.RawPageRow rawPage = rawPages[cnt];

            //    #region New Implementation (RawPage folder)
            //    DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);

            //    if (rawPageDir.Exists)
            //    {
            //        FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_s.pdf");

            //        if (rawPageFiles.Length > 0)
            //        {
            //            pages.Add(rawPageFiles[0].FullName);
            //        }
            //    }
            //    #endregion
            //}

            ArrayList pages = RawPageDb.GetRawPagesToDisplayPDF(docId);

            #endregion

            //string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            string saveDir = Util.GetTempFolder();

            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            string mergedFileName = Path.Combine(saveDir, docId.ToString() + "_" + Guid.NewGuid() + ".pdf");

            if (pages.Count > 0)
                Util.MergePdfFiles(pages, mergedFileName);
            else
            {
                try
                {
                    if (File.Exists(mergedFileName))
                    {
                        File.Delete(mergedFileName);
                    }
                }
                catch (Exception)
                {
                }

                pdfframe.Attributes["src"] = "NoFile.aspx";

            }

            if (File.Exists(mergedFileName))
            {
                pdfframe.Attributes["src"] = "../Common/DownloadMergedDocument.aspx?filePath=" + mergedFileName;


                PdfReader pdfReader = new PdfReader(mergedFileName);

                #region modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

                //if (rawPages.Count != pdfReader.NumberOfPages)
                //{
                //    missingPages = rawPages.Count - pdfReader.NumberOfPages;
                //    pdfReader.Close();
                //    throw new System.ArgumentException(Constants.ProblemLoadingPages);
                //}

                int totalPages = RawPageDb.GetRawPagesNoOfRecords(docId);

                if (totalPages != pdfReader.NumberOfPages)
                {
                    missingPages = totalPages - pdfReader.NumberOfPages;
                    pdfReader.Close();
                    throw new System.ArgumentException(Constants.ProblemLoadingPages);
                }

                #endregion

            }
            else
                throw new System.ArgumentException(Constants.ProblemLoadingPages);
        }
        catch (Exception)
        {
            //if (e.Message.Contains(Constants.ProblemLoadingPages))
            //{
            //    string msg = string.Format(Constants.ProblemLoadingPages, missingPages, missingPages == 1 ? string.Empty : "s", missingPages == 1 ? "is" : "are", missingPages == 1 ? "The" : "These", missingPages == 1 ? string.Empty : "s");
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            //}
        }
    }
    #endregion


    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {

        if (e.Argument == "null" || e.Argument == string.Empty)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        else if (e.Argument.ToLower().Contains("blank"))
        {
            string[] arrBlank = e.Argument.Split(new string[] { ";" }, StringSplitOptions.None);
            SetIncomeToBlank(arrBlank[1], arrBlank[2]);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        }
        #region Added By Edward 30/01/2014 Sales and Resales Changes
        else if (e.Argument.ToLower().Contains("up_") || e.Argument.ToLower().Contains("down_"))
        {
            Navigation(e.Argument.ToString());
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
        else if (e.Argument.ToLower() == "version")            
        {
            string versionId = VersionDropdownList.SelectedValue;
            PopulateVersion();
            VersionDropdownList.SelectedValue = versionId;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
        else if (e.Argument.ToLower().Contains("versionno"))
        {
            PopulateVersion();
            if (e.Argument.ToLower().Contains("new"))
                VersionDropdownList.SelectedIndex = VersionDropdownList.Items.Count - 1;
            else
                VersionDropdownList.SelectedIndex = VersionDropdownList.FindItemIndexByText(e.Argument.Substring(9));
            TemplateDropdownList.SelectedIndex = 0;
            CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            PopulateTableMonthRepeater();            
            MetDataConfirmPanel.Text = "Saved at " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            MetDataConfirmPanel2.Text = "Saved at " + DateTime.Now.ToString("dd MMM yyyy, hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            IncomeDb.InsertExtractionLog(ReqDocAppId.Value, ReqNRIC, string.Empty,
                LogActionEnum.Save_Income_For_REPLACE2, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);            
        }
        #endregion
        else
        {
            LoadingPanel1.Visible = false;
            ReqAppDocRefId = int.Parse(e.Argument.ToString());
            GetDocIdByAppDocRefId();
            DisplayPdfDocument();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
    }

    private void Navigation(string action)
    {
        string[] arrAction = action.Split(new string[] {"_"},StringSplitOptions.None);
        DataTable dt = IncomeDb.GetIncomeDetailsById(int.Parse(arrAction[1]));
        if (dt.Rows.Count > 0)
        {
            DataRow r = dt.Rows[0];
            IncomeDb.UpdateIncomeDetailsOrderNo(int.Parse(r["Id"].ToString()), arrAction[0], int.Parse(r["IncomeVersionId"].ToString()), int.Parse(r["OrderNo"].ToString()));
            PopulateTableMonthRepeater();            
        }        
    }

    /// <summary>
    /// This will update the Assessment Status to Extraction in Progress when the Assessment Status is Pending_Extraction
    /// </summary>
    private void ChangeDocAppAssessmentStatus()
    {
        try
        {
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppDt = docAppDb.GetDocAppById(ReqDocAppId.Value);
            if (docAppDt.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docAppDt[0];
                if (docAppRow.AssessmentStatus.Trim().Equals(AssessmentStatusEnum.Pending_Extraction.ToString()))
                {
                    docAppDb.UpdateRefStatus(ReqDocAppId.Value, AppStatusEnum.Completeness_Checked, AssessmentStatusEnum.Extraction_In_Progress, true, false, LogActionEnum.Confirmed_application);
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Changing Status", ex.Message), true);
        }        
    }
    
    #region Added By Edward Traverse 2014/6/23
    private int PopulateTraversing()
    {
        listDocId = IncomeDb.GetDocIdForTraverse(ReqDocAppId.Value, ReqNRIC);
        return listDocId.Count;
    }

    protected void PreviousButton_Click(object sender, EventArgs e)
    {
        if (ViewState["CurrentDocId"] != null)
        {
            if (int.Parse(ViewState["CurrentDocId"].ToString()) <= 0)
                ViewState["CurrentDocId"] = DocIdCount - 1;
            else
                ViewState["CurrentDocId"] = int.Parse(ViewState["CurrentDocId"].ToString()) - 1;
        }
        else
            ViewState["CurrentDocId"] = 0;
        docId = listDocId[int.Parse(ViewState["CurrentDocId"].ToString())];
        DisplayPdfDocument();
        
    }

    protected void NextButton_Click(object sender, EventArgs e)
    {
        if (ViewState["CurrentDocId"] != null)
        {

            if (int.Parse(ViewState["CurrentDocId"].ToString()) + 1 >= DocIdCount)
                ViewState["CurrentDocId"] = 0;
            else
                ViewState["CurrentDocId"] = int.Parse(ViewState["CurrentDocId"].ToString()) + 1;
        }
        else
            ViewState["CurrentDocId"] = 0;
        docId = listDocId[int.Parse(ViewState["CurrentDocId"].ToString())];
        DisplayPdfDocument();
    }

    #endregion

    #region Modifed by Edward 2014/07/22 Changes in Zoning Page 
    protected void TableMonthRepeater_ItemCommand(Object Sender, RepeaterCommandEventArgs e)
    {
        PopulateTableMonthRepeater();   
    }

    protected void TableMonthRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = (RepeaterItem)e.Item;
        DataRowView data = (DataRowView)item.DataItem;

        Table MonthTable = (Table)e.Item.FindControl("MonthTable");        
        MonthTable.ID = "MonthTable" + data["Id"].ToString();

        #region Set alternate colors
        string cssClass = string.Empty;
        if ((item.ItemIndex % 2) == 0)            
            cssClass = "green-header";
        else
            cssClass = "gray-header";
        #endregion


        #region Create Headers
        TableRow trHeader = new TableRow();
        trHeader.CssClass = cssClass;
        CreateHeaders(trHeader, data["MonthYear"].ToString(), MonthTable.ClientID, data["Id"].ToString());
        MonthTable.Rows.Add(trHeader);
        #endregion

       

        #region Create Rows

        if (TemplateDropdownList.SelectedValue.ToLower().Contains("tem"))
        {
            if (!VersionDropdownList.SelectedValue.ToLower().Contains("new"))
            {
                DataTable dt = IncomeDb.GetIncomeDetailsByVersionNoAndIncomeId(int.Parse(VersionDropdownList.SelectedValue), int.Parse(data["Id"].ToString()));
                if (dt.Rows.Count > 0)
                {
                    CreateIncomeDetailsRowCOS(data["Id"].ToString(), dt, MonthTable, MonthTable.ClientID, data["CurrencyRate"].ToString());
                    DataTable dtTotals = IncomeDb.GetIncomeDetailTotals(int.Parse(dt.Rows[0]["IncomeVersionId"].ToString()));
                    CreateIncomeTotals(data["Id"].ToString(), dtTotals, MonthTable, data["CurrencyRate"].ToString());
                }
                else
                {
                    //CreateDefaultIncomeDetails(data["Id"].ToString(), MonthTable, MonthTable.ClientID, data["CurrencyRate"].ToString());
                    CreateIncomeTotals(data["Id"].ToString(), null, MonthTable, data["CurrencyRate"].ToString());
                }
            }
            else
            {
                if (item.ItemIndex == 0)                
                    CreateDefaultIncomeDetails(data["Id"].ToString(), MonthTable, MonthTable.ClientID, data["CurrencyRate"].ToString());
                CreateIncomeTotals(data["Id"].ToString(), null, MonthTable, data["CurrencyRate"].ToString());
                
            }
        }
        else //if a template was selected
        {
            DataTable dt = new DataTable();
            dt = SelectIncomeTemplate(TemplateDropdownList.SelectedValue.ToLower());
            if (dt.Rows.Count > 0)
            {
                if (item.ItemIndex == 0)
                {
                    CreateTemplate(data["Id"].ToString(), dt, MonthTable, MonthTable.ClientID, data["CurrencyRate"].ToString());
                    DataTable dtTotals = IncomeDb.GetIncomeDetailTotals(int.Parse(dt.Rows[0]["IncomeVersionId"].ToString()));
                    CreateIncomeTotals(data["Id"].ToString(), dtTotals, MonthTable, data["CurrencyRate"].ToString());
                }
                else
                    CreateIncomeTotals(data["Id"].ToString(), null, MonthTable, data["CurrencyRate"].ToString());
            }
            else
            {
                //CreateDefaultIncomeDetails(data["Id"].ToString(), MonthTable, MonthTable.ClientID, data["CurrencyRate"].ToString());
                CreateIncomeTotals(data["Id"].ToString(), null, MonthTable, data["CurrencyRate"].ToString());
            }
        }
        #endregion

        #region Create Add Row
        TableRow trAddRow = new TableRow();
        CreateAddRow(trAddRow, data["Id"].ToString(), MonthTable.ClientID, data["CurrencyRate"].ToString());
        MonthTable.Rows.Add(trAddRow);
        #endregion

        arrIncomeId.Add(data["Id"].ToString());
        arrClientIdofMonthTable.Add(MonthTable.ClientID);
        arrClientIdForex.Add(data["CurrencyRate"].ToString());
    }

    private void PopulateTableMonthRepeater()
    {
        try
        {
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());
            TableMonthRepeater.DataSource = dt;
            TableMonthRepeater.DataBind();
        }
        catch (Exception ex)
        {            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in populating month table", ex.Message), true);
        }        
    }

    #region Create Headers
    private void CreateHeaders(TableRow tr, string strMonthYear, string MonthTableClientId, string incomeId)
    {
        FillInCheckBoxForHeaders(incomeId, tr, MonthTableClientId, "5%");
        FillInCellsForHeaders(strMonthYear, "30%", tr, incomeId);
        FillInCellsForHeaders("Amount", "10%", tr, false);
        if (str_Section.Equals("COS"))
        {
            FillInCellsForHeaders("Gross", "9%", tr, true);
            FillInCellsForHeaders("Allce", "9%", tr, true);
            FillInCellsForHeaders("OT", "9%", tr, true);
        }
        else
        {
            FillInCellsForHeaders("Gross", "9%", tr, true);
            FillInCellsForHeaders("AHG", "9%", tr, true);         
        }
        FillInCellsForHeaders(string.Empty, "10%", tr, false);
    }

    private void FillInCellsForHeaders(string str, string strUnit, TableRow tr, bool isCenter)
    {
        TableCell tc = new TableCell();
        tc.Text = str;           
        tc.Width = new Unit(strUnit);
        if (isCenter)
            tc.HorizontalAlign = HorizontalAlign.Center;
        tr.Cells.Add(tc);
    }

    private void FillInCellsForHeaders(string str, string strUnit, TableRow tr, string incomeId)
    {
        TableCell tc = new TableCell();
        tc.Text = str;
        tc.Width = new Unit(strUnit);
        Label lbl = new Label();
        lbl.Text = str;
        tc.Controls.Add(lbl);
        HiddenField hf = new HiddenField();
        hf.Value = incomeId;                
        tc.Controls.Add(hf);
        tr.Cells.Add(tc);
    }

    private void FillInCheckBoxForHeaders(string id, TableRow tr, string MonthTableId, string width)
    {
        TableCell tc = new TableCell();
        tc.HorizontalAlign = HorizontalAlign.Center;
        tc.Width = new Unit(width);
        CheckBox cb = new CheckBox();
        cb.ID = id;
        cb.Attributes.Add("OnClick", string.Format("javascript:SelectCheckBoxes('{0}')", MonthTableId));
        tc.Controls.Add(cb);
        tr.Cells.Add(tc);
    }
    #endregion

    #region Creating Default Income Details eg: Basic 0
    private void CreateDefaultIncomeDetails(string incomeId, Table tbl, string monthTableClientId, string forex)
    {
        TableRow tr;
        tr = new TableRow();        
        TableCell tc = new TableCell();
        int index = 1;
        FillInCheckBoxForIncomeDetailRows("RowNumberCheckBox" + incomeId, false, tr, monthTableClientId);
        FillInTextBoxForIncomeDetailRows("RowItem" + incomeId, "Basic", tr, false, monthTableClientId, forex);
        FillInTextBoxForIncomeDetailRows("RowAmount" + incomeId, "0", tr, true, monthTableClientId, forex);
        FillInCheckBoxForIncomeDetailRows("RowGrossCheckBox" + incomeId, false, tr, monthTableClientId, forex, index, "gross");
        if (str_Section.Equals("COS"))
        {        
            FillInCheckBoxForIncomeDetailRows("RowAllceCheckBox" + incomeId, false, tr, monthTableClientId, forex, index, "allce");
            FillInCheckBoxForIncomeDetailRows("RowOTCheckBox" + incomeId, false, tr, monthTableClientId, forex, index,  "ot");
        }
        else
            FillInCheckBoxForIncomeDetailRows("RowAHGCheckBox" + incomeId, false, tr, monthTableClientId, forex, index, "ahg");        
        tc = new TableCell();
        LinkButton lb = new LinkButton();
        System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
        image.ImageUrl = "~/Data/Images/navUpD.png";
        //image.Attributes.Add("OnClick", string.Format("javascript:Navigating('up_{0}');return false;", monthTableClientId));
        //image.Style.Add("cursor", "pointer");
        lb.Controls.Add(image);
        tc.Controls.Add(image);
        lb = new LinkButton();
        image = new System.Web.UI.WebControls.Image();
        image.ImageUrl = "~/Data/Images/navDownD.png";
        //image.Attributes.Add("OnClick", string.Format("javascript:Navigating('down_{0}');return false;", monthTableClientId));
        //image.Style.Add("cursor", "pointer");
        lb.Controls.Add(image);
        tc.Controls.Add(image);
        Button b = new Button();
        b.Text = "X";
        b.CssClass = "button-zone";
        b.Attributes.Add("OnClick", string.Format("javascript:DeleteCurrentRow('{0}','{1}');return false;", monthTableClientId, forex));
        tc.Controls.Add(b);
        HiddenField hf = new HiddenField();
        hf.Value = "0";
        tc.Controls.Add(hf);
        tr.Cells.Add(tc);        
        tbl.Rows.Add(tr);
    }

    private void CreateTemplate(string incomeId, DataTable dt, Table tbl, string monthTableClientId, string forex)
    {
        TableRow tr;
        int index = 1;
        foreach (DataRow r in dt.Rows)
        {
            tr = new TableRow();
            TableCell tc = new TableCell();
            FillInCheckBoxForIncomeDetailRows("RowNumberCheckBox" + incomeId + index.ToString(), false, tr, monthTableClientId);
            FillInTextBoxForIncomeDetailRows("RowItem" + incomeId + index.ToString(), r["IncomeItem"].ToString(), tr, false, monthTableClientId, forex);
            FillInTextBoxForIncomeDetailRows("RowAmount" + incomeId + index.ToString(), r["IncomeAmount"].ToString(), tr, true, monthTableClientId, forex);
            FillInCheckBoxForIncomeDetailRows("RowGrossCheckBox" + incomeId + index.ToString(), bool.Parse(r["GrossIncome"].ToString()), tr, monthTableClientId, forex, index, "gross");
            if (str_Section.Equals("COS"))
            {                
                FillInCheckBoxForIncomeDetailRows("RowAllceCheckBox" + incomeId + index.ToString(), bool.Parse(r["Allowance"].ToString()), tr, monthTableClientId, forex, index, "allce");
                FillInCheckBoxForIncomeDetailRows("RowOTCheckBox" + incomeId + index.ToString(), bool.Parse(r["Overtime"].ToString()), tr, monthTableClientId, forex, index, "ot");
            }
            else                            
                FillInCheckBoxForIncomeDetailRows("RowAHGCheckBox" + incomeId + index.ToString(), bool.Parse(r["AHGIncome"].ToString()), tr, monthTableClientId, forex, index, "ahg");            
            tc = new TableCell();
            LinkButton lb = new LinkButton();
            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            image.ImageUrl = "~/Data/Images/navUpD.png";
            lb.Controls.Add(image);
            tc.Controls.Add(image);
            lb = new LinkButton();
            image = new System.Web.UI.WebControls.Image();
            image.ImageUrl = "~/Data/Images/navDownD.png";
            lb.Controls.Add(image);
            tc.Controls.Add(image);
            Button b = new Button();
            b.Text = "X";
            b.CssClass = "button-zone";
            b.Attributes.Add("OnClick", string.Format("javascript:DeleteCurrentRow('{0}','{1}');return false;", monthTableClientId, forex));
            tc.Controls.Add(b);
            HiddenField hf = new HiddenField();
            hf.Value = "0";
            tc.Controls.Add(hf);
            tr.Cells.Add(tc);
            tbl.Rows.Add(tr);
            index++;
        }
    }

    private void CreateIncomeDetailsRowCOS(string incomeId, DataTable dt, Table tbl, string monthTableClientId, string forex)
    {
        int index = 1;
        TableRow tr;
        foreach (DataRow r in dt.Rows)
        {
            tr = new TableRow();
            TableCell tc = new TableCell();
            FillInCheckBoxForIncomeDetailRows("RowNumberCheckBox" + incomeId + "_" + r["IncomeDetailsID"].ToString(), false, tr, monthTableClientId);
            FillInTextBoxForIncomeDetailRows("RowItem" + incomeId + "_" + r["IncomeDetailsID"].ToString(), r["IncomeItem"].ToString(), tr, false, monthTableClientId, forex);
            FillInTextBoxForIncomeDetailRows("RowAmount" + incomeId + "_" + r["IncomeDetailsID"].ToString(), r["IncomeAmount"].ToString(), tr, true, monthTableClientId, forex);
            FillInCheckBoxForIncomeDetailRows("RowGrossCheckBox" + incomeId + "_" + r["IncomeDetailsID"].ToString(), bool.Parse(r["GrossIncome"].ToString()), tr, monthTableClientId, forex, index, "gross");
            if (str_Section.Equals("COS"))
            {                
                FillInCheckBoxForIncomeDetailRows("RowAllceCheckBox" + incomeId + "_" + r["IncomeDetailsID"].ToString(), bool.Parse(r["Allowance"].ToString()), tr, monthTableClientId, forex, index, "allce");
                FillInCheckBoxForIncomeDetailRows("RowOTCheckBox" + incomeId + "_" + r["IncomeDetailsID"].ToString(), bool.Parse(r["Overtime"].ToString()), tr, monthTableClientId, forex, index, "ot");
            }
            else                            
                FillInCheckBoxForIncomeDetailRows("RowAHGCheckBox" + incomeId + "_" + r["IncomeDetailsID"].ToString(), bool.Parse(r["AHGIncome"].ToString()), tr, monthTableClientId, forex, index, "ahg");            
            tc = new TableCell();
            LinkButton lb = new LinkButton();
            System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
            image.ImageUrl = "~/Data/Images/navUp.png";
            lb.Controls.Add(image);
            lb.ID = "RowUp" + incomeId + "_" + r["IncomeDetailsID"].ToString();
            lb.Attributes.Add("OnClick", string.Format("javascript:Navigating('up_{0}');return false;", r["IncomeDetailsID"].ToString()));
            tc.Controls.Add(lb);
            lb = new LinkButton();
            image = new System.Web.UI.WebControls.Image();
            image.ImageUrl = "~/Data/Images/navDown.png";
            lb.Controls.Add(image);
            lb.ID = "RowDown" + incomeId + "_" + r["IncomeDetailsID"].ToString();
            lb.Attributes.Add("OnClick", string.Format("javascript:Navigating('down_{0}');return false;", r["IncomeDetailsID"].ToString()));
            tc.Controls.Add(lb);
            Button b = new Button();
            b.Text = "X";
            b.CssClass = "button-zone";
            b.Attributes.Add("OnClick", string.Format("javascript:DeleteCurrentRow('{0}','{1}');return false;", monthTableClientId, forex));
            b.ID = "RowDelete" + incomeId + "_" + r["IncomeDetailsID"].ToString();
            tc.Controls.Add(b);
            HiddenField hf = new HiddenField();
            hf.Value = r["IncomeDetailsId"].ToString();
            hf.ID = "RowIncomeDetailsId" + incomeId + "_" + r["IncomeDetailsID"].ToString();
            tc.Controls.Add(hf);
            tr.Cells.Add(tc);
            index++;
            tbl.Rows.Add(tr);
        }
    }
    #endregion


    private void CreateAddRow(TableRow tr, string incomeId, string monthTableClientId, string forex)
    {
        TableCell tc;
        tc = new TableCell();
        tc.Text = "Add";
        tc.Font.Bold = true;
        tr.Cells.Add(tc);
        FillInTextBoxForAddRow("AddItem" + incomeId, tr, false, monthTableClientId);
        FillInTextBoxForAddRow("AddAmount" + incomeId, tr, true, monthTableClientId);
        FillInCheckBoxForAddRow("AddGross" + incomeId, tr, monthTableClientId, "gross");
        if (str_Section.Equals("COS"))
        {            
            FillInCheckBoxForAddRow("AddAllce" + incomeId, tr, monthTableClientId, "allce");
            FillInCheckBoxForAddRow("AddOT" + incomeId, tr, monthTableClientId, "ot");
        }
        else                 
            FillInCheckBoxForAddRow("AddAHG" + incomeId, tr, monthTableClientId, "ahg");                    
        tc = new TableCell();
        LinkButton lb = new LinkButton();
        lb.ID = "AddRow" + incomeId;
        lb.Attributes.Add("OnClick", string.Format("javascript:AddingRows({0},'{1}','{2}');return false;", incomeId, monthTableClientId, forex));            
        lb.Text = "Add";
        lb.CssClass = "button-zone";        
        tc.Controls.Add(lb);
        tr.Cells.Add(tc);
    }

    private void FillInTextBoxForAddRow(string id, TableRow tr, bool HasAmount, string monthTableClientId)
    {
        TableCell tc = new TableCell();
        TextBox tb = new TextBox();
        tb.ID = id;
        tb.Width = new Unit("100%");
        if (HasAmount)
        {
            tb.Text = "0";
            tb.Style.Add("text-align", "right");
        }
        else
        {
            tb.Attributes.Add("OnBlur", string.Format("javascript:SplitItemAndAmount('{0}')", monthTableClientId));
        }
        tc.Controls.Add(tb);
        tr.Cells.Add(tc);        
    }

    private void FillInCheckBoxForAddRow(string id, TableRow tr, string clientIdOfMonthTable, string chkBoxType)
    {
        TableCell tc = new TableCell();
        CheckBox cb = new CheckBox();
        cb.ID = id;       
        tc.HorizontalAlign = HorizontalAlign.Center;
        cb.Attributes.Add("OnClick", string.Format("javascript:ValidateCheckboxesForAddRow('{0}','{1}')", clientIdOfMonthTable, chkBoxType));
        tc.Controls.Add(cb);
        tr.Cells.Add(tc);
    }

    

    private void FillInCheckBoxForIncomeDetailRows(string id, bool value, TableRow tr, string MonthTableId, string forex, int chkBoxIndex, string chkBoxType)
    {
        TableCell tc = new TableCell();
        tc.HorizontalAlign = HorizontalAlign.Center;
        CheckBox cb = new CheckBox();
        cb.ID = id;
        cb.Checked = value;
        cb.Attributes.Add("OnClick", string.Format("javascript:CalculateCheckboxes('{0}','{1}')", MonthTableId, forex));
        cb.Attributes.Add("OnClick", string.Format("javascript:ValidateCheckboxes('{0}','{1}','{2}','{3}')", MonthTableId, chkBoxIndex, forex, chkBoxType));
        tc.Controls.Add(cb);
        tr.Cells.Add(tc);
    }

    private void FillInCheckBoxForIncomeDetailRows(string id, bool value, TableRow tr, string MonthTableId)
    {
        TableCell tc = new TableCell();
        tc.HorizontalAlign = HorizontalAlign.Center;
        CheckBox cb = new CheckBox();
        cb.ID = id;
        cb.Checked = value;        
        tc.Controls.Add(cb);
        tr.Cells.Add(tc);
    }

    private void FillInTextBoxForIncomeDetailRows(string id, string value, TableRow tr, bool IsAmount, string MonthTableId, string forex)
    {
        TableCell tc = new TableCell();
        TextBox tb = new TextBox();
        tb.ID = id;
        tb.Text = value;
        tb.Width = new Unit("100%");
        if (IsAmount)
        {
            tb.Style.Add("text-align", "right");            
            tb.Attributes.Add("OnBlur", string.Format("javascript:CalculateCheckboxes('{0}','{1}')", MonthTableId, forex));
        }
        tc.Controls.Add(tb);
        tr.Cells.Add(tc);
    }

    private void CreateIncomeTotals(string incomeId, DataTable dt, Table tbl, string forex)
    {
        decimal dc0;
        decimal dc1;
        TableRow tr = new TableRow();
        TableCell tc = new TableCell();        
        tr.Cells.Add(tc);
        tc = new TableCell();
        HtmlTextArea hta = new HtmlTextArea();
        hta.ID = "hta" + incomeId;
        hta.Attributes.Add("class", "append-items");              
        hta.Attributes.Add("OnBlur", string.Format("javascript:ClearHTA('{0}','{1}','{2}');", tbl.ClientID,forex,incomeId));
        tc.Controls.Add(hta);       
        tr.Cells.Add(tc);
        if (dt != null && dt.Rows.Count > 0)
        {
            FillInLabelForTotal(dt.Rows[0]["IncomeAmount"].ToString() + "<br>" + dt.Rows[1]["IncomeAmount"].ToString(), tr);
            dc0 = !string.IsNullOrEmpty(dt.Rows[0]["GrossIncome"].ToString()) ? decimal.Parse(dt.Rows[0]["GrossIncome"].ToString()) : 0;
            dc1 = !string.IsNullOrEmpty(dt.Rows[1]["GrossIncome"].ToString()) ? decimal.Parse(dt.Rows[1]["GrossIncome"].ToString()) : 0;
            FillInLabelForTotal(Format.GetDecimalPlacesWithoutRounding(dc0).ToString() + "<br>" + Format.GetDecimalPlacesWithoutRounding(dc1).ToString(), tr);
            if (str_Section.Equals("COS"))
            {                                
                dc0 = !string.IsNullOrEmpty(dt.Rows[0]["Allowance"].ToString()) ? decimal.Parse(dt.Rows[0]["Allowance"].ToString()) : 0;
                dc1 = !string.IsNullOrEmpty(dt.Rows[1]["Allowance"].ToString()) ? decimal.Parse(dt.Rows[1]["Allowance"].ToString()) : 0;
                FillInLabelForTotal(Format.GetDecimalPlacesWithoutRounding(dc0).ToString() + "<br>" + Format.GetDecimalPlacesWithoutRounding(dc1).ToString(), tr);
                dc0 = !string.IsNullOrEmpty(dt.Rows[0]["Overtime"].ToString()) ? decimal.Parse(dt.Rows[0]["Overtime"].ToString()) : 0;
                dc1 = !string.IsNullOrEmpty(dt.Rows[1]["Overtime"].ToString()) ? decimal.Parse(dt.Rows[1]["Overtime"].ToString()) : 0;
                FillInLabelForTotal(Format.GetDecimalPlacesWithoutRounding(dc0).ToString() + "<br>" + Format.GetDecimalPlacesWithoutRounding(dc1).ToString(), tr);
            }
            else
            {                
                dc0 = !string.IsNullOrEmpty(dt.Rows[0]["AHGIncome"].ToString()) ? decimal.Parse(dt.Rows[0]["AHGIncome"].ToString()) : 0;
                dc1 = !string.IsNullOrEmpty(dt.Rows[1]["AHGIncome"].ToString()) ? decimal.Parse(dt.Rows[1]["AHGIncome"].ToString()) : 0;
                FillInLabelForTotal(Format.GetDecimalPlacesWithoutRounding(dc0).ToString() + "<br>" + Format.GetDecimalPlacesWithoutRounding(dc1).ToString(), tr);                
            }
        }
        else
        {
            FillInLabelForTotal("SGD Total<br>SGD / 1 Total", tr);            
            FillInLabelForTotal("0<br>0", tr);
            FillInLabelForTotal("0<br>0", tr);
            if (str_Section.Equals("COS"))
                FillInLabelForTotal("0<br>0", tr);
        }
        tc = new TableCell();
        tr.Cells.Add(tc);
        tbl.Rows.Add(tr);
    }

    private void FillInLabelForTotal(string str, TableRow tr)
    {
        TableCell tc = new TableCell();
        Label lbl1 = new Label();
        lbl1.Font.Bold = true;
        lbl1.Text = str;
        tc.Controls.Add(lbl1);
        tr.Cells.Add(tc);
    }

    #region Version Dropdownlist 
    private void PopulateVersion()
    {
        try
        {
            VersionDropdownList.Items.Clear();
            DataTable dt = IncomeDb.GetIncomeVersionNoByDocAppIdAndNric(ReqDocAppId.Value, ReqNRIC);
            RadComboBoxItem blank = new RadComboBoxItem("- New -", "new");
            VersionDropdownList.Items.Add(blank);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    RadComboBoxItem item = new RadComboBoxItem(string.Format("{0} - {1}", row["VersionNo"].ToString(), row["VersionName"].ToString()), row["VersionNo"].ToString());
                    VersionDropdownList.Items.Add(item);
                }                
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Populating Versions", ex.Message), true); 
        }

    }
    //Initializes the Version No
    private void GetIncomeVersion()
    {
        try
        {
            DataTable dt = IncomeDb.GetIncomeDataById(ReqIncomeId.Value);
            if (dt.Rows.Count > 0)
            {
                incomeVersionId = int.Parse(dt.Rows[0]["IncomeVersionId"].ToString());
                if (incomeVersionId > 0)
                {
                    dt = IncomeDb.GetIncomeVersionById(incomeVersionId);
                    if (dt.Rows.Count > 0)
                        VersionDropdownList.SelectedValue = dt.Rows[0]["VersionNo"].ToString();
                }
                else
                {
                    dt = IncomeDb.GetIncomeVersionNoByDocAppIdAndNricAndIncomeVersionIsZero(ReqDocAppId.Value, ReqNRIC);
                    if (dt.Rows.Count > 0)
                        VersionDropdownList.SelectedValue = dt.Rows[0]["VersionNo"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Getting Income Version", ex.Message), true);
        }
        
    }
    
    protected void VersionDropdownList_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        int a = 0;
        if (e.Value.ToLower().Contains("new"))                    
            TemplateDropdownList.SelectedIndex = 0;        
        else if (int.TryParse(e.Value, out a))                    
            TemplateDropdownList.SelectedIndex = 0;        
        else                                             
            VersionDropdownList.SelectedIndex = 0;                    
        PopulateTableMonthRepeater();
    }    
    #endregion  

    private void UpdateToolTip(string id, UpdatePanel panel, string value)
    {          
            Control ctrl = Page.LoadControl("~/Controls/CopyItems.ascx");
            Controls_CopyItems ctrlItems = (Controls_CopyItems)ctrl;
            ctrlItems.NRIC = Request["nric"].ToString();
            ctrlItems.DocAppId = int.Parse(Request["id"]);
            ctrlItems.VersionNo = VersionDropdownList.SelectedValue;
            ctrlItems.MonthYearClientId = HiddenClientId.Value;
            ctrlItems.Action = value;
            panel.ContentTemplateContainer.Controls.Add(ctrl);        
    }

    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        this.UpdateToolTip(args.TargetControlID, args.UpdatePanel, args.Value);
    }


    private void AddAttributesForButtons()
    {
        try
        {
            //PopulateCopyToolTip();
            if (!Object.Equals(RadToolTipManager1, null))
            {
                RadToolTipManager1.TargetControls.Add(CopyButton.ClientID,"copy", true);
                RadToolTipManager1.TargetControls.Add(CopyButton2.ClientID, "copy", true);
                RadToolTipManager1.TargetControls.Add(ArrowDivideButton.ClientID, "divide", true);
                RadToolTipManager1.TargetControls.Add(ArrowDivideButton2.ClientID, "divide", true);
                RadToolTipManager1.TargetControls.Add(IncomeBlank.ClientID, "blank", true);
            }
            MetDataConfirmPanel.Text = string.Empty;
            MetDataConfirmPanel2.Text = string.Empty;
            HistoryButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ViewLog.aspx?Id={0}&docAppId={1}&nric={2}',500,500);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));
            HistoryButton2.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ViewLog.aspx?Id={0}&docAppId={1}&nric={2}',500,500);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));
            ChangeButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ChangeDocument.aspx?docAppId={0}&nric={1}',500,500);return false;", ReqDocAppId, ReqNRIC));
            VersionButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersion.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));
            VersionButton2.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersion.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));
            DeleteTemp.Attributes.Add("OnClick", string.Format("javascript:DeleteAllSelectedRows();return false;"));
            DeleteTemp2.Attributes.Add("OnClick", string.Format("javascript:DeleteAllSelectedRows();return false;"));
        }
        catch (Exception ex)
        {            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in adding attributes for controls", ex.Message), true);
        }        
    }

    private void PopulateHiddenFields()
    {
        try
        {
            HiddenClientId.Value = string.Empty;                            // Contains the ClientIds of Each Table
            HiddenForex.Value = string.Empty;                               // Contains the Forex Values of Each Month
            HiddenToDeleteIncomeDetails.Value = string.Empty;               // Contains the IncomeDetailsId to be deleted
            HiddenIncomeId.Value = string.Empty;                            // Contains the IncomeId 
            foreach (string str in arrClientIdofMonthTable)
                HiddenClientId.Value = HiddenClientId.Value + str + ",";
            foreach (string str in arrClientIdForex)
                HiddenForex.Value = HiddenForex.Value + str + ",";
            foreach (string str in arrIncomeId)
                HiddenIncomeId.Value = HiddenIncomeId.Value + str + ",";
        }
        catch (Exception ex)
        {
            
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in hidden fields", ex.Message), true);
        }        
    }

    private void SetIncomeToBlank(string IdsToBeBlanked, string AllIds)
    {
        try
        {
            if (!VersionDropdownList.SelectedValue.ToLower().Contains("new"))
            {
                List<string> listIds = new List<string>(IdsToBeBlanked.Split(new string[] { "," }, StringSplitOptions.None));
                List<string> listAllIds = new List<string>(AllIds.Split(new string[] { "," }, StringSplitOptions.None));
                foreach (string AllId in listAllIds)
                {
                    int intIncomeId = int.Parse(AllId);
                    bool IsSetToBlank = false;
                    foreach (string id in listIds)
                    {                                                
                        if(AllId.Equals(id))
                        {
                            IncomeDb.DeleteIncomeItemsByIncomeIdAndVersionNo(intIncomeId, int.Parse(VersionDropdownList.SelectedValue));
                            IsSetToBlank = true;                            
                        }                                                
                    }
                    DataTable dt = IncomeDb.GetIncomeVersionByIncomeIdAndVersionNo(intIncomeId, int.Parse(VersionDropdownList.SelectedValue));
                    if (dt.Rows.Count > 0)
                    {
                        DataRow rVersion = dt.Rows[0];
                        IncomeDb.UpdateIncome(intIncomeId, int.Parse(rVersion["id"].ToString()), IsSetToBlank);
                    }
                }
                IncomeDb.DeleteCreditAssessment(ReqAppPersonalId.Value);
                CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
                #region Added By Edward  24/02/2014 Add Icon and Action Log                
                IncomeDb.InsertExtractionLog(ReqDocAppId.Value, ReqNRIC, string.Empty,
                    LogActionEnum.Set_Income_to_Blank_For_REPLACE2, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                #endregion
            }
            else
            {
                string msg = string.Format("Please select a version.");
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            }
            //int NewVersionNo = 0;
            //int NewVersionId = 0;
            //int intIncomeId = 0;                       
            //#region Set Income to Blank Functionality 2014/08/25 
            //// Clicking Set Income to Blank will create new version that has no Income Component but it will be acceptable to Extract
            //// It will always create a new version and make that version the latest version.  
            //// It will create a new version and save into the database immediately
            //// It will not delete or "set to blank" the current version or the selected version.
            //// It will not delete other previous versions
            //DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());
            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow r in dt.Rows)
            //    {
            //        intIncomeId = int.Parse(r["id"].ToString());
            //        DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(intIncomeId);
            //        NewVersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;
            //        NewVersionId = IncomeDb.InsertIncomeVersion(intIncomeId, NewVersionNo, (Guid)Membership.GetUser().ProviderUserKey);
            //        IncomeDb.UpdateIncome(intIncomeId, NewVersionId, true);
            //    }                
            //    CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            //    #region Added By Edward  24/02/2014 Add Icon and Action Log
            //    IncomeDb.InsertExtractionLog(ReqAppPersonalId.Value, string.Empty, string.Empty,
            //        LogActionEnum.Set_Income_to_Blank_by_REPLACE1, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            //    #endregion
            //}
            //#endregion
            //Delete ONLY the Selected Version and make it the current version

            //int intIncomeId = 0; 
            //if (!VersionDropdownList.SelectedValue.ToLower().Contains("new"))
            //{
            //    IncomeDb.DeleteIncomeItemsByVersionNoAndDocAppIdAndNric(int.Parse(VersionDropdownList.SelectedValue), ReqDocAppId.Value, ReqNRIC);
            //    DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());
            //    if (dt.Rows.Count > 0)
            //    {
            //        foreach (DataRow r in dt.Rows)
            //        {
            //            intIncomeId = int.Parse(r["id"].ToString());
            //            DataTable dtVersion = IncomeDb.GetIncomeVersionByDocAppIdAndNricAndVersionNoAndIncomeId(ReqDocAppId.Value, ReqNRIC, int.Parse(VersionDropdownList.SelectedValue), intIncomeId);
            //            if (dtVersion.Rows.Count > 0)
            //            {
            //                DataRow rVersion = dtVersion.Rows[0];
            //                IncomeDb.UpdateIncome(intIncomeId, int.Parse(rVersion["id"].ToString()), true);                            
            //            }
            //        }
            //        IncomeDb.DeleteCreditAssessment(ReqAppPersonalId.Value);
            //        CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            //    }                                                                
            //    #region Added By Edward  24/02/2014 Add Icon and Action Log                
            //    IncomeDb.InsertExtractionLog(ReqDocAppId.Value, ReqNRIC, string.Empty,
            //        LogActionEnum.Set_Income_to_Blank_For_REPLACE2_by_REPLACE1, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
            //    #endregion
            //}
            //else
            //{
            //    string msg = string.Format("Please select a version.");
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            //}
        }
        catch (Exception ex)
        {
            string msg = string.Format("There's a problem in Setting the Income to blank. {0}", ex.Message);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
        }
    }


    private void CheckUserSection()
    {
        try
        {
            MembershipUser user = Membership.GetUser();
            Guid currentUserId = (Guid)user.ProviderUserKey;

            DepartmentDb departmentDb = new DepartmentDb();
            Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
            Department.DepartmentRow departmentRow = departments[0];

            SectionDb sectionDb = new SectionDb();
            Section.SectionDataTable sectionDt = sectionDb.GetSectionByDepartment(departmentRow.Id);
            Section.SectionRow sectionRow = sectionDt[0];

            str_Section = sectionRow.Code.ToUpper().Trim();
            HiddenSection.Value = str_Section;
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0} : {1}');", "Error in Checking user rights", ex.Message), true);
        }        
    }

    

    #endregion



}



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

public partial class IncomeAssessment_NewZoningPage : System.Web.UI.Page
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

    #region Income Total Variables
    decimal AllowanceTotal;
    decimal CPFIncomeTotal;
    decimal GrossIncomeTotal;
    decimal AHGIncomeTotal;
    decimal OvertimeTotal;
    #endregion

    List<int> listDocId;
    int DocIdCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]) && !string.IsNullOrEmpty(Request["nric"]))
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

            //if (string.IsNullOrEmpty(StoreIncomeId.Value))
            //    StoreIncomeId.Value = ReqIncomeId.Value.ToString();
            #endregion

            ReqAppPersonalId = int.Parse(Request["apppersonal"]);
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        DocIdCount = PopulateTraversing();

        if (!IsPostBack)
        {
            EditFlag = 0; //always zero when postback
            VersionIdToDeleteItems = 0; //always zero when postback
            ChangeDocAppAssessmentStatus();
            PopulateApplicantDetails();
            PopulateMonthYear();
            GetIncomeIdIncomeVersion();
            PopulateTemplate();
            //if (MonthYearDropDownList.Items.Count > 0)
            //    PopulateVersion();
            GetDocIdByAppDocRefId();
            DisplayPdfDocument();
            ViewState["CurrentDocId"] = null;
            if (ViewState["CurrentDocId"] == null)
                ViewState["CurrentDocId"] = listDocId.IndexOf(docId);
        }

        DisableZoning();

        CopyButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('CopyZoning.aspx?docAppId={0}&nric={1}',550,500);return false;", ReqDocAppId, ReqNRIC));
        ChangeButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ChangeDocument.aspx?docAppId={0}&nric={1}',500,500);return false;", ReqDocAppId, ReqNRIC));
        //ArrowDivideButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('DivisionFunction.aspx?docAppId={0}&nric={1}&incomeId={2}',550,200);return false;", ReqDocAppId, ReqNRIC, MonthYearDropDownList.SelectedValue));
        //VersionButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersion.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));


    }

    #region Zoning Page Permissions
    private void DisableZoning()
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


    private void SetAccessControl(Guid? assessmentStaffUserId, string status)
    {
        MembershipUser user = Membership.GetUser();
        Guid currentUserId = (Guid)user.ProviderUserKey;

        if (assessmentStaffUserId == Guid.Empty || currentUserId != assessmentStaffUserId)
        {
            DisableButtons();
        }

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
        ArrowDivideButton.Enabled = false;
        ArrowDivideButton.ToolTip = "Not allowed to zone.";
        DeleteTemp.Enabled = false;
        DeleteTemp.ToolTip = "Not allowed to zone.";
        IncomeBlank.Enabled = false;
        IncomeBlank.ToolTip = "Not allowed to zone.";
        HistoryButton.Enabled = false;
        HistoryButton.ToolTip = "Not allowed to zone.";
        //SaveButton.Enabled = false;
        //SaveButton.ToolTip = "Not allowed to zone.";
    }

    #endregion

    #region DROPDOWN EVENTS

    protected void MonthYearDropDownList_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        DragAndDropAction = string.Empty;
        EditFlag = 0;
        //ClearAllDictionaries();     

        GetIncomeVersion();
        //VersionDropdownList.Text = string.Empty;
        PopulateVersion();
        int VersionDDLSelectedValue;
        //if (!int.TryParse(VersionDropdownList.SelectedValue, out VersionDDLSelectedValue))
        //    VersionDDLSelectedValue = 0;
        CurrencyTotalDic.Clear();
       // CreateCurrencyTotalDic(VersionDDLSelectedValue);
        //IncomeDetailsRadGrid.Rebind();
        //StoreIncomeId.Value = MonthYearDropDownList.SelectedValue;
        //InsertIntoViewLog(int.Parse(StoreIncomeId.Value));
        GetIncomeIdIncomeVersion();
        //VersionButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('IncomeVersion.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));
    }

    #region Version Dropdown SelectIndexChanged Event
    protected void VersionDropdownList_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        DragAndDropAction = string.Empty;
        EditFlag = 0;
        int a = 0;
        //ClearAllDictionaries();  
        if (e.Value.ToLower().Contains("new"))
        {
            DataTable dt = new DataTable();
            int NumberOfItems = 0;
            dt = AddDataTableColumns(dt);
            dt = GetItemsFromRadGrid(dt, out NumberOfItems);
            //IncomeDetailsRadGrid.DataSource = dt;
            //IncomeDetailsRadGrid.DataBind();
            DragAndDropAction = "append";
        }
        else if (int.TryParse(e.Value, out a))
        {
            //IncomeDetailsRadGrid.Rebind();
        }
        else
        {
            DataTable dt = new DataTable();
            dt = SelectIncomeTemplate(e.Value.ToLower());
            //IncomeDetailsRadGrid.DataSource = dt;
            //IncomeDetailsRadGrid.DataBind();
            DragAndDropAction = "append";
            DeleteTemp.Enabled = false;
            DeleteTemp.ToolTip = "Cannot delete from templates.";
        }

    }
    #endregion

    #endregion

    #region RADGRID Events

    #region RadGrid NeedDataSource Event
    protected void IncomeDetailsRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        GetItemsAndPayTextBoxFromRadGrid();
        PopulateIncomeItems();
    }
    #endregion

    #region RadGrid ItemDataBound Event
    protected void IncomeDetailsRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;

            #region Added by Edward 30/01/2014 Navigation icons for Sales and Resales Changes
            LinkButton btnUp = e.Item.FindControl("btnUp") as LinkButton;
            LinkButton btnDown = e.Item.FindControl("btnDown") as LinkButton;
            if (int.Parse(data["Id"].ToString()) <= 0)
            {
                System.Web.UI.WebControls.Image imgUp = e.Item.FindControl("imgUp") as System.Web.UI.WebControls.Image;
                System.Web.UI.WebControls.Image imgDown = e.Item.FindControl("imgDown") as System.Web.UI.WebControls.Image;
                imgUp.ImageUrl = "~/Data/Images/navUpD.png";
                imgDown.ImageUrl = "~/Data/Images/navDownD.png";
                imgUp.ToolTip = "Please save the Income Month to enable this feature.";
                imgDown.ToolTip = "Please save the Income Month to enable this feature.";
                btnUp.Enabled = false;
                btnDown.Enabled = false;
            }
            else
            {
                btnUp.Attributes.Add("OnClick", string.Format("javascript:Navigating('up_{0}');return false;", data["Id"].ToString()));
                btnDown.Attributes.Add("OnClick", string.Format("javascript:Navigating('down_{0}');return false;", data["Id"].ToString()));
                btnUp.ToolTip = "Move the item up. ";
                btnDown.ToolTip = "Move the item down.";
            }
            #endregion

            #region IncomeItem and IncomeAmount - Modified by Edward 27/2/2014 February 21, 2014 Meeting : Edit Mode the Whole Zoning Page
            TextBox IncomeItemTextBox = (TextBox)e.Item.FindControl("IncomeItemTextBox");
            TextBox IncomeAmountTextBox = (TextBox)e.Item.FindControl("AmountTextBox");
            IncomeItemTextBox.Text = data["IncomeItem"].ToString();
            IncomeAmountTextBox.Text = data["IncomeAmount"].ToString();
            #endregion

            #region GrossIncomeCheckbox
            CheckBox GrossIncomeCheckBox = e.Item.FindControl("GrossIncomeCheckBox") as CheckBox;
            GrossIncomeCheckBox.Checked = bool.Parse(data["GrossIncome"].ToString());
            GrossIncomeTotal = (GrossIncomeTotal + (GrossIncomeCheckBox.Checked ? decimal.Parse(data["IncomeAmount"].ToString()) : 0));
            #endregion

            #region AllowanceCheckBox
            CheckBox AllowanceCheckBox = e.Item.FindControl("AllowanceCheckBox") as CheckBox;
            AllowanceCheckBox.Checked = bool.Parse(data["Allowance"].ToString());
            AllowanceTotal = (AllowanceTotal + (AllowanceCheckBox.Checked ? decimal.Parse(data["IncomeAmount"].ToString()) : 0));
            #endregion

            #region CPFIncomeCheckbox
            CheckBox CPFIncomeCheckBox = e.Item.FindControl("CPFIncomeCheckBox") as CheckBox;
            CPFIncomeCheckBox.Checked = bool.Parse(data["CPFIncome"].ToString());
            CPFIncomeTotal = (CPFIncomeTotal + (CPFIncomeCheckBox.Checked ? decimal.Parse(data["IncomeAmount"].ToString()) : e.Item.IsInEditMode ? decimal.Parse(IncomeAmountTextBox.Text) : 0));
            #endregion

            #region AHGIncomeCheckBox
            CheckBox AHGIncomeCheckBox = e.Item.FindControl("AHGIncomeCheckBox") as CheckBox;
            AHGIncomeCheckBox.Checked = bool.Parse(data["AHGIncome"].ToString());
            AHGIncomeTotal = (AHGIncomeTotal + (AHGIncomeCheckBox.Checked ? decimal.Parse(data["IncomeAmount"].ToString()) : 0));
            #endregion

            #region OvertimeCheckbox
            CheckBox OvertimeCheckBox = e.Item.FindControl("OvertimeCheckBox") as CheckBox;
            OvertimeCheckBox.Checked = bool.Parse(data["Overtime"].ToString());
            OvertimeTotal = (OvertimeTotal + (OvertimeCheckBox.Checked ? decimal.Parse(data["IncomeAmount"].ToString()) : 0));
            #endregion
        }
        else if (e.Item is GridFooterItem)
        {
            #region Footer Totals
            Label CurrencyNONSGD = e.Item.FindControl("CurrencyNONSGD") as Label;
            CurrencyNONSGD.Text = CurrencyTotalDic.ContainsKey("CurrencyTotals1") ? CurrencyTotalDic["CurrencyTotals1"].Currency + " Total" : "Total";
            Label CurrencySGD = e.Item.FindControl("CurrencySGD") as Label;
            CurrencySGD.Text = CurrencyTotalDic.ContainsKey("CurrencyTotals2") ? CurrencyTotalDic["CurrencyTotals2"].Currency + " / " + CurrencyTotalDic["CurrencyTotals1"].CurrencyRate + " Total" : "Total";

            decimal NONSGDRate = CurrencyTotalDic.ContainsKey("CurrencyTotals1") ? CurrencyTotalDic["CurrencyTotals1"].CurrencyRate : 1;
            decimal SGDRate = CurrencyTotalDic.ContainsKey("CurrencyTotals2") ? CurrencyTotalDic["CurrencyTotals2"].CurrencyRate : 1;

            Label AllowanceLabel = e.Item.FindControl("AllowanceLabel") as Label;
            AllowanceLabel.Text = Format.GetDecimalPlacesWithoutRounding(AllowanceTotal).ToString();
            Label AllowanceLabelSGD = e.Item.FindControl("AllowanceLabelSGD") as Label;
            AllowanceLabelSGD.Text = Format.GetDecimalPlacesWithoutRounding(AllowanceTotal / NONSGDRate).ToString();

            Label CPFIncomeLabel = e.Item.FindControl("CPFIncomeLabel") as Label;
            CPFIncomeLabel.Text = Format.GetDecimalPlacesWithoutRounding(CPFIncomeTotal).ToString();
            Label CPFIncomeLabelSGD = e.Item.FindControl("CPFIncomeLabelSGD") as Label;
            CPFIncomeLabelSGD.Text = Format.GetDecimalPlacesWithoutRounding(CPFIncomeTotal / NONSGDRate).ToString();

            Label GrossIncomeLabel = e.Item.FindControl("GrossIncomeLabel") as Label;
            GrossIncomeLabel.Text = Format.GetDecimalPlacesWithoutRounding(GrossIncomeTotal).ToString();
            Label GrossIncomeLabelSGD = e.Item.FindControl("GrossIncomeLabelSGD") as Label;
            GrossIncomeLabelSGD.Text = Format.GetDecimalPlacesWithoutRounding(GrossIncomeTotal / NONSGDRate).ToString();

            Label AHGIncomeLabel = e.Item.FindControl("AHGIncomeLabel") as Label;
            AHGIncomeLabel.Text = Format.GetDecimalPlacesWithoutRounding(AHGIncomeTotal).ToString();
            Label AHGIncomeLabelSGD = e.Item.FindControl("AHGIncomeLabelSGD") as Label;
            AHGIncomeLabelSGD.Text = Format.GetDecimalPlacesWithoutRounding(AHGIncomeTotal / NONSGDRate).ToString();

            Label OvertimeLabel = e.Item.FindControl("OvertimeLabel") as Label;
            OvertimeLabel.Text = Format.GetDecimalPlacesWithoutRounding(OvertimeTotal).ToString();
            Label OvertimeLabelSGD = e.Item.FindControl("OvertimeLabelSGD") as Label;
            OvertimeLabelSGD.Text = Format.GetDecimalPlacesWithoutRounding(OvertimeTotal / NONSGDRate).ToString();
            #endregion
        }
    }
    #endregion

    #endregion

    #region Retrieve Methods

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
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }
    #endregion
    /// <summary>
    /// Gets the IncomeVersionId of the Selected MonthYear. Also Adds attributes for the HistoryButton.
    /// </summary>
    private void GetIncomeIdIncomeVersion()
    {
        try
        {
            DataTable dt;
            //dt = IncomeDb.GetIncomeDataById(int.Parse(MonthYearDropDownList.SelectedValue));
            //DataRow incomeRow = dt.Rows[0];
            //incomeVersionId = int.Parse(incomeRow["IncomeVersionId"].ToString());
            //HistoryButton.Attributes.Remove("OnClick");
            //Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
            //HistoryButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ViewLog.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", ReqIncomeId,ReqDocAppId,ReqNRIC));
            //HistoryButton.Attributes.Add("OnClick", string.Format("javascript:ShowWindow('ViewLog.aspx?Id={0}&docAppId={1}&nric={2}',600,600);return false;", StoreIncomeId.Value, ReqDocAppId, ReqNRIC));

            //dt = IncomeDb.GetDocsByIncomeId(ReqDocAppId.Value, ReqNRIC, ReqIncomeId.Value, ReqAppDocRefId.Value);
            //docId = dt.Rows.Count > 0 ? int.Parse(dt.Rows[0]["DocId"].ToString()) : 0;
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }

    #region Populates the Version Combo Box based on the Selected MonthYear combo box
    /// <summary>
    /// Populates the Version DropdownList
    /// </summary>
    private void PopulateVersion()
    {
        try
        {
            //VersionDropdownList.Items.Clear();
           // DataTable dt = IncomeDb.GetIncomeVersionByIncome(int.Parse(MonthYearDropDownList.SelectedValue));
            RadComboBoxItem blank = new RadComboBoxItem("- New -", "new");
            //VersionDropdownList.Items.Add(blank);

            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow row in dt.Rows)
            //    {
            //        RadComboBoxItem item = new RadComboBoxItem(string.Format("{0} - {1}", row["VersionNo"].ToString(), row["VersionName"].ToString()), row["Id"].ToString());
            //        //VersionDropdownList.Items.Add(item);
            //    }
            //   // VersionDropdownList.SelectedValue = incomeVersionId.ToString();
            //}
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }

    private void PopulateTemplate()
    {
        try
        {
            //TemplateDropdownList.Items.Clear();
            RadComboBoxItem blank = new RadComboBoxItem("- Template - ", "tem");
            //TemplateDropdownList.Items.Add(blank);

            IncomeTemplateDb incomeTemplateDb = new IncomeTemplateDb();
            IncomeTemplate.IncomeTemplateDataTable incomeTemplateDataTable = incomeTemplateDb.GetIncomeTemplates();

            if (incomeTemplateDataTable.Rows.Count > 0)
            {
                foreach (IncomeTemplate.IncomeTemplateRow row in incomeTemplateDataTable)
                {
                    RadComboBoxItem newTemplate = new RadComboBoxItem(string.Format("- {0} -", row.Name), row.Name);
                    //TemplateDropdownList.Items.Add(newTemplate);
                }

            }

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }

    #endregion

    #region Populates the MonthYear Combo box
    /// <summary>
    /// Populates the MonthYear Combobox. 
    /// </summary>
    private void PopulateMonthYear()
    {
        try
        {
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC.ToString());

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    RadComboBoxItem item = new RadComboBoxItem(row["MonthYear"].ToString(), row["Id"].ToString());
                    int noOfNotAcceptedDocPerIncome = IncomeDb.GetNoofIncomeDetailsPerIncome(int.Parse(row["Id"].ToString()));
                    if (noOfNotAcceptedDocPerIncome > 0)
                        item.ImageUrl = "~/Data/Images/Green_Check.png";
                    //if (int.Parse(row["IncomeVersionId"].ToString()) > 0)
                    //    item.ImageUrl = "~/Data/Images/Green_Check.png";                    
                    //MonthYearDropDownList.Items.Add(item);
                }
                //MonthYearDropDownList.SelectedValue = ReqIncomeId.Value.ToString();     Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
               // MonthYearDropDownList.SelectedValue = StoreIncomeId.Value;
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }
    #endregion

    #region Populates the IncomeDetailsRadGrid
    private void PopulateIncomeItems()
    {
        try
        {
            CheckUserSection();

            #region Checks Dictionaries if Null then creates a new one
            //if (ChkAllowanceDic == null)
            //    ChkAllowanceDic = new Dictionary<string, bool>();
            //if (ChkCPFIncomeDic == null)
            //    ChkCPFIncomeDic = new Dictionary<string, bool>();
            //if (ChkGrossIncomeDic == null)
            //    ChkGrossIncomeDic = new Dictionary<string, bool>();
            //if (ChkAHGIncomeDic == null)
            //    ChkAHGIncomeDic = new Dictionary<string, bool>();
            //if (ChkOvertimeDic == null)
            //    ChkOvertimeDic = new Dictionary<string, bool>();
            #endregion

            #region Populate Footer

            //int VersionDDLSelectedValue = !VersionDropdownList.SelectedValue.ToLower().Contains("new") ? int.Parse(VersionDropdownList.SelectedValue) : 0;
            int VersionDDLSelectedValue;
            //if (!int.TryParse(VersionDropdownList.SelectedValue, out VersionDDLSelectedValue))
            //    VersionDDLSelectedValue = 0;

            //CreateCurrencyTotalDic(VersionDDLSelectedValue);

            #endregion

            #region Determines if the RadGrid is in Edit Mode or New and will Add Rows from the drag and Drop
            int a = 0;
            //if (int.TryParse(VersionDropdownList.SelectedValue, out a) && TemplateDropdownList.SelectedValue.Contains("tem")) // this is from the database
            //{
            //    if (DragAndDropAction == "replace")
            //    {
            //        //ClearAllDictionaries();
            //        CreateCurrencyTotalDic(VersionDDLSelectedValue);
            //    }
            //    DataTable dtIncomeItems = new DataTable();
            //    if (EditFlag == 0)
            //    {
            //        IncomeDb.UpdateIncomeDetailsOrderNos(VersionDDLSelectedValue);
            //        dtIncomeItems = IncomeDb.GetIncomeDetailsByIncomeVersion(VersionDDLSelectedValue);
            //        EditFlag = 1;
            //    }
            //    else
            //        dtIncomeItems = AddRowsToExistingVersionsByDragAndDrop();
            //    IncomeDetailsRadGrid.DataSource = dtIncomeItems;
            //    IncomeDetailsRadGridCount = dtIncomeItems.Rows.Count;
            //}
            //else // this is for new and other templates
            //{
            //    //if (DragAndDropAction == "replace")
            //    //    ClearAllDictionaries();
            //    DataTable dtIncomeItems = AddRowsToNewVersionByDragAndDrop();
            //    IncomeDetailsRadGrid.DataSource = dtIncomeItems;
            //    IncomeDetailsRadGridCount = dtIncomeItems.Rows.Count;
            //}
            //if (string.IsNullOrEmpty(DragAndDropAction))
            //    DragAndDropAction = "append";
            #endregion
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true);
        }

    }
    #endregion

    #region Checks the section of user to hide the AHGIncome Column
    private void CheckUserSection()
    {
        MembershipUser user = Membership.GetUser();
        Guid currentUserId = (Guid)user.ProviderUserKey;

        DepartmentDb departmentDb = new DepartmentDb();
        Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)Membership.GetUser().ProviderUserKey);
        Department.DepartmentRow departmentRow = departments[0];

        SectionDb sectionDb = new SectionDb();
        Section.SectionDataTable sectionDt = sectionDb.GetSectionByDepartment(departmentRow.Id);
        Section.SectionRow sectionRow = sectionDt[0];

        if (sectionRow.Code.ToUpper().Trim() == "ROS" || sectionRow.Code.ToUpper().Trim() == "SOS")
        {
            //IncomeDetailsRadGrid.MasterTableView.Columns[9].Visible = true;
            //IncomeDetailsRadGrid.MasterTableView.Columns[8].Visible = false;
            //IncomeDetailsRadGrid.MasterTableView.Columns[7].Visible = false;
        }
        else if (sectionRow.Code.ToUpper().Trim() == "COS")
        {
            //IncomeDetailsRadGrid.MasterTableView.Columns[9].Visible = false;
            //IncomeDetailsRadGrid.MasterTableView.Columns[8].Visible = true;
            //IncomeDetailsRadGrid.MasterTableView.Columns[7].Visible = true;
        }
    }
    #endregion

    #region DRAG AND DROP METHODS

    #region Add Rows when in Edit Mode By Drag and Drop

    private DataTable AddRowsToExistingVersionsByDragAndDrop()
    {
        DataTable dt = new DataTable();
        int NumberOfItems = 0;
        dt = AddDataTableColumns(dt);
        if (DragAndDropAction == "append")
        {
            dt = GetItemsFromRadGrid(dt, out NumberOfItems);
        }
        dt = AddRowsWhenDragDropTriggers(dt, NumberOfItems);
        if (DragAndDropAction == "replace")
        {
            //VersionIdToDeleteItems = int.Parse(!VersionDropdownList.SelectedValue.ToLower().Contains("new") ? VersionDropdownList.SelectedValue : "0");
            int VersionIdToDeleteItems;
            //if (!int.TryParse(VersionDropdownList.SelectedValue, out VersionIdToDeleteItems))
            //    VersionIdToDeleteItems = 0;
        }
        DragAndDropAction = "append";
        return dt;
    }
    #endregion

    #region Add rows by Drag Drop when the Version Combobox is "NEW"
    private DataTable AddRowsToNewVersionByDragAndDrop()
    {
        DataTable dt = new DataTable();
        int NumberOfItems = 0;
        dt = AddDataTableColumns(dt);

        if (DragAndDropAction == "append")
        {
            dt = GetItemsFromRadGrid(dt, out NumberOfItems);
        }
        dt = AddRowsWhenDragDropTriggers(dt, NumberOfItems);
        DragAndDropAction = "append";
        return dt;
    }
    #endregion

    #region Miscellaneous Functions

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

    private void CreateCurrencyTotalDic(int VersionDDLSelectedValue)
    {
        DataTable dtTotals = IncomeDb.GetIncomeDetailTotals(VersionDDLSelectedValue);

        if (CurrencyTotalDic == null)
            CurrencyTotalDic = new Dictionary<string, CurrencyTotals>();

        if (dtTotals.Rows.Count > 0)
        {
            foreach (DataRow row in dtTotals.Rows)
            {
                CurrencyTotals clsCurrencyTotals = new CurrencyTotals();
                clsCurrencyTotals.Currency = row["Currency"].ToString();
                clsCurrencyTotals.CurrencyRate = decimal.Parse(row["IncomeItem"].ToString());
                if (!CurrencyTotalDic.ContainsKey("CurrencyTotals" + row[0].ToString()))
                    CurrencyTotalDic.Add("CurrencyTotals" + row[0].ToString(), clsCurrencyTotals);

            }
        }
        else
        {
            //dtTotals = IncomeDb.GetIncomeDataById(int.Parse(MonthYearDropDownList.SelectedValue));
            if (dtTotals.Rows.Count > 0)
            {

                DataRow row = dtTotals.Rows[0];
                CurrencyTotals clsCurrencyTotals = new CurrencyTotals();
                clsCurrencyTotals.Currency = row["Currency"].ToString();
                clsCurrencyTotals.CurrencyRate = decimal.Parse(row["CurrencyRate"].ToString());
                if (!CurrencyTotalDic.ContainsKey("CurrencyTotals1"))
                    CurrencyTotalDic.Add("CurrencyTotals1", clsCurrencyTotals);
                clsCurrencyTotals = new CurrencyTotals();
                clsCurrencyTotals.Currency = "SGD";
                clsCurrencyTotals.CurrencyRate = 1;
                if (!CurrencyTotalDic.ContainsKey("CurrencyTotals2"))
                    CurrencyTotalDic.Add("CurrencyTotals2", clsCurrencyTotals);
            }
        }
    }

    private DataTable GetItemsFromRadGrid(DataTable dt, out int NumberOfItems)
    {
        DataRow row;
        int i = 0;
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    row = dt.NewRow();
        //    Label ContainerLabel = (Label)item.FindControl("ContainerLabel");
        //    Label IdLabel = (Label)item.FindControl("IdLabel");
        //    row["Id"] = IdLabel.Text;
        //    row["IncomeVersionId"] = "0";
        //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
        //    if (IncomeItemTextBox.Visible == true)
        //        row["IncomeItem"] = IncomeItemTextBox.Text;
        //    else
        //        row["IncomeItem"] = IncomeItemLabel.Text;
        //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //    if (AmountTextBox.Visible == true)
        //        row["IncomeAmount"] = decimal.Parse(AmountTextBox.Text);
        //    else
        //        row["IncomeAmount"] = decimal.Parse(AmountLabel.Text);
        //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
        //    row["Allowance"] = AllowanceCheckBox.Checked;
        //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
        //    row["CPFIncome"] = CPFIncomeCheckBox.Checked;
        //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
        //    row["GrossIncome"] = GrossIncomeCheckBox.Checked;
        //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
        //    row["AHGIncome"] = AHGIncomeCheckBox.Checked;
        //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
        //    row["Overtime"] = OvertimeCheckBox.Checked;
        //    dt.Rows.Add(row);
        //    i = !string.IsNullOrEmpty(ContainerLabel.Text) ? int.Parse(ContainerLabel.Text) : 0;
        //}
        NumberOfItems = i;
        return dt;
    }

    private DataTable GetItemsFromRadGrid(DataTable dt, CheckBox chk, string strTrigger)
    {
        DataRow row;
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    row = dt.NewRow();
        //    Label ContainerLabel = (Label)item.FindControl("ContainerLabel");
        //    Label IdLabel = (Label)item.FindControl("IdLabel");
        //    row["Id"] = IdLabel.Text;
        //    row["IncomeVersionId"] = "0";
        //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
        //    if (IncomeItemTextBox.Visible == true)
        //        row["IncomeItem"] = IncomeItemTextBox.Text;
        //    else
        //        row["IncomeItem"] = IncomeItemLabel.Text;
        //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //    if (AmountTextBox.Visible == true)
        //        row["IncomeAmount"] = decimal.Parse(AmountTextBox.Text);
        //    else
        //        row["IncomeAmount"] = decimal.Parse(AmountLabel.Text);


        //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
        //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
        //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
        //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
        //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
        //    if (strTrigger == "GROSSINCOME" && GrossIncomeCheckBox.UniqueID == chk.UniqueID)
        //    {
        //        row["GrossIncome"] = chk.Checked;
        //        row["Allowance"] = !chk.Checked ? false : AllowanceCheckBox.Checked;
        //        row["Overtime"] = chk.Checked ? false : OvertimeCheckBox.Checked;
        //    }
        //    else if (strTrigger == "ALLOWANCE" && AllowanceCheckBox.UniqueID == chk.UniqueID)
        //    {
        //        row["Allowance"] = chk.Checked;
        //        row["GrossIncome"] = chk.Checked ? true : GrossIncomeCheckBox.Checked;
        //        row["Overtime"] = chk.Checked ? false : OvertimeCheckBox.Checked;
        //    }
        //    else if (strTrigger == "OVERTIME" && OvertimeCheckBox.UniqueID == chk.UniqueID)
        //    {
        //        row["Overtime"] = chk.Checked;
        //        row["GrossIncome"] = chk.Checked ? false : GrossIncomeCheckBox.Checked;
        //        row["Allowance"] = chk.Checked ? false : AllowanceCheckBox.Checked;
        //    }
        //    else
        //    {
        //        row["GrossIncome"] = GrossIncomeCheckBox.Checked;
        //        row["Allowance"] = AllowanceCheckBox.Checked;
        //        row["Overtime"] = OvertimeCheckBox.Checked;
        //    }
        //    row["CPFIncome"] = CPFIncomeCheckBox.Checked;
        //    row["AHGIncome"] = AHGIncomeCheckBox.Checked;
        //    dt.Rows.Add(row);
        //}
        return dt;
    }

    private DataTable AddRowsWhenDragDropTriggers(DataTable dt, int i)
    {
        //if ((DragAndDropAction == "append" && !string.IsNullOrEmpty(AppendItems.InnerText))
        //    || (DragAndDropAction == "replace" && !string.IsNullOrEmpty(ReplaceItems.InnerText)))
        //{
        //    DataRow row;

        //    string[] arrRows = DragAndDropAction == "replace" ? ReplaceItems.InnerText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None) : AppendItems.InnerText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        //    foreach (string strRows in arrRows)
        //    {
        //        decimal IncomeAmount = 0;
        //        string IncomeItem = string.Empty;
        //        string[] arrReplaceItems = strRows.Trim().Split(' ');
        //        foreach (string str in arrReplaceItems)
        //        {
        //            if (!decimal.TryParse(str.Trim(), NumberStyles.Currency | NumberStyles.AllowCurrencySymbol, CultureInfo.GetCultureInfo("en-US"), out IncomeAmount))
        //            {
        //                IncomeItem = IncomeItem + str + " ";
        //            }
        //        }
        //        row = dt.NewRow();
        //        row["Id"] = 0;
        //        row["IncomeVersionId"] = "0";
        //        row["IncomeItem"] = IncomeItem;
        //        row["IncomeAmount"] = IncomeAmount;
        //        row["Allowance"] = false;
        //        row["CPFIncome"] = false;
        //        row["GrossIncome"] = false;
        //        row["AHGIncome"] = false;
        //        row["Overtime"] = false;
        //        dt.Rows.Add(row);
        //    }

        //    ReplaceItems.InnerText = string.Empty;
        //    AppendItems.InnerText = string.Empty;
        //}
        return dt;
    }

    private void ClearAllDictionaries()
    {
        ChkAllowanceDic.Clear();
        ChkCPFIncomeDic.Clear();
        ChkGrossIncomeDic.Clear();
        ChkAHGIncomeDic.Clear();
        ChkOvertimeDic.Clear();
        CurrencyTotalDic.Clear();
        IncomeItemDic.Clear();
        IncomeAmountDic.Clear();
    }

    #endregion

    #endregion

    #region Gets the current IncomeVersion of the selected Income in the MonthYear Dropdown List
    private void GetIncomeVersion()
    {
        //DataTable dt = IncomeDb.GetIncomeDataById(int.Parse(MonthYearDropDownList.SelectedValue));
        //if (dt.Rows.Count > 0)
        //{
        //    incomeVersionId = int.Parse(dt.Rows[0]["IncomeVersionId"].ToString());
        //    #region This condition will clear the RadGrid when there is no IncomeVersion for the specific Income
        //    if (incomeVersionId == 0)
        //        IncomeDetailsRadGrid.DataSource = new DataTable();
        //    #endregion
        //}
    }
    #endregion

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
        AppDocRefDb e = new AppDocRefDb();
        AppDocRef.AppDocRefDataTable dt = e.GetAppDocRefById(ReqAppDocRefId.Value);
        if (dt.Rows.Count > 0)
        {
            AppDocRef.AppDocRefRow row = dt[0];
            docId = row.DocId;
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
        //InsertIntoViewLog(int.Parse(StoreIncomeId.Value));
        int missingPages = 0;
        try
        {
            ArrayList pages = new ArrayList();

            RawPageDb rawPageDb = new RawPageDb();
            RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(docId);

            for (int cnt = 0; cnt < rawPages.Count; cnt++)
            {
                RawPage.RawPageRow rawPage = rawPages[cnt];

                #region New Implementation (RawPage folder)
                DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);

                if (rawPageDir.Exists)
                {
                    FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_s.pdf");

                    if (rawPageFiles.Length > 0)
                    {
                        pages.Add(rawPageFiles[0].FullName);
                    }
                }
                #endregion
            }

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
                catch (Exception ex)
                {
                }

                pdfframe.Attributes["src"] = "NoFile.aspx";

            }

            if (File.Exists(mergedFileName))
            {
                pdfframe.Attributes["src"] = "../Common/DownloadMergedDocument.aspx?filePath=" + mergedFileName;


                PdfReader pdfReader = new PdfReader(mergedFileName);

                if (rawPages.Count != pdfReader.NumberOfPages)
                {
                    missingPages = rawPages.Count - pdfReader.NumberOfPages;
                    pdfReader.Close();
                    throw new System.ArgumentException(Constants.ProblemLoadingPages);
                }

            }
            else
                throw new System.ArgumentException(Constants.ProblemLoadingPages);
        }
        catch (Exception e)
        {
            //if (e.Message.Contains(Constants.ProblemLoadingPages))
            //{
            //    string msg = string.Format(Constants.ProblemLoadingPages, missingPages, missingPages == 1 ? string.Empty : "s", missingPages == 1 ? "is" : "are", missingPages == 1 ? "The" : "These", missingPages == 1 ? string.Empty : "s");
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
            //}
        }
    }
    #endregion

    #region This Method is used to Get the values from the text box when it is in editMode
    /// <summary>
    /// This Method is used to Get the values from the text box of the Item and Pay Columns when it is in editMode
    /// </summary>    
    private void GetItemsAndPayTextBoxFromRadGrid()
    {

        if (IncomeItemDic == null)
            IncomeItemDic = new Dictionary<string, string>();
        if (IncomeAmountDic == null)
            IncomeAmountDic = new Dictionary<string, decimal>();
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //    TextBox IncomeAmountTextBox = (TextBox)item.FindControl("AmountTextBox");
        //    if (item.IsInEditMode)
        //    {
        //        if (!IncomeItemDic.ContainsKey(IncomeItemTextBox.UniqueID))
        //        {
        //            IncomeItemDic.Add(IncomeItemTextBox.UniqueID, IncomeItemTextBox.Text);
        //            IncomeAmountDic.Add(IncomeAmountTextBox.UniqueID, decimal.Parse(IncomeAmountTextBox.Text));
        //        }
        //        else
        //        {
        //            IncomeItemDic[IncomeItemTextBox.UniqueID] = IncomeItemTextBox.Text;
        //            IncomeAmountDic[IncomeAmountTextBox.UniqueID] = decimal.Parse(IncomeAmountTextBox.Text);
        //        }
        //    }
        //}
    }
    #endregion

    #endregion

    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {

        if (e.Argument == "null" || e.Argument == string.Empty)
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        else if (e.Argument == "d")
        {
            //IncomeDetailsRadGrid.Rebind();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
        else if (e.Argument.ToLower() == "replace" || e.Argument.ToLower() == "append")
        {
            DragAndDropAction = e.Argument.ToLower();
            //IncomeDetailsRadGrid.Rebind();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
        else if (e.Argument.ToLower() == "delete")
        {
            DeleteItems();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
        }
        else if (e.Argument.ToLower() == "divide")
        {
            DivideAssessment();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        }
        else if (e.Argument.ToLower() == "copy")
        {
            CopyAssessment();
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
        }
        else if (e.Argument.ToLower() == "blank")
        {
            SetIncomeToBlank();
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
            //string versionId = VersionDropdownList.SelectedValue;
            //PopulateVersion();
            //VersionDropdownList.SelectedValue = versionId;
            //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:SetPDFSize('98','92');", true);
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
        string[] arrAction = action.Split(new string[] { "_" }, StringSplitOptions.None);
        DataTable dt = IncomeDb.GetIncomeDetailsById(int.Parse(arrAction[1]));
        if (dt.Rows.Count > 0)
        {
            DataRow r = dt.Rows[0];
            IncomeDb.UpdateIncomeDetailsOrderNo(int.Parse(r["Id"].ToString()), arrAction[0], int.Parse(r["IncomeVersionId"].ToString()), int.Parse(r["OrderNo"].ToString()));
            dt = IncomeDb.GetIncomeDetailsByIncomeVersion(int.Parse(r["IncomeVersionId"].ToString()));
            #region Adds the temporary records to a temporary table then will merge to permanent table after ordering
            DataTable dtTemp = new DataTable();
            DataRow row;
            dtTemp.Columns.Add("Id");
            dtTemp.Columns.Add("IncomeVersionId");
            dtTemp.Columns.Add("IncomeItem");
            dtTemp.Columns.Add("IncomeAmount");
            dtTemp.Columns.Add("Allowance");
            dtTemp.Columns.Add("CPFIncome");
            dtTemp.Columns.Add("GrossIncome");
            dtTemp.Columns.Add("AHGIncome");
            dtTemp.Columns.Add("Overtime");
            dtTemp.Columns.Add("OrderNo");

            //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
            //{
            //    Label IdLabel = (Label)item.FindControl("IdLabel");

            //    if (IdLabel.Text == "0")
            //    {
            //        row = dt.NewRow();
            //        row["Id"] = IdLabel.Text;
            //        row["IncomeVersionId"] = "0";
            //        TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
            //        Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
            //        if (IncomeItemTextBox.Visible == true)
            //            row["IncomeItem"] = IncomeItemTextBox.Text;
            //        else
            //            row["IncomeItem"] = IncomeItemLabel.Text;
            //        TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
            //        Label AmountLabel = item.FindControl("AmountLabel") as Label;
            //        if (AmountTextBox.Visible == true)
            //            row["IncomeAmount"] = decimal.Parse(AmountTextBox.Text);
            //        else
            //            row["IncomeAmount"] = decimal.Parse(AmountLabel.Text);

            //        #region Assign the Checkboxes
            //        CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
            //        CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
            //        CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
            //        CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
            //        CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;


            //        if (GrossIncomeCheckBox.Visible == true)
            //            row["GrossIncome"] = GrossIncomeCheckBox.Checked;
            //        else
            //            row["GrossIncome"] = false;

            //        if (AllowanceCheckBox.Visible == true)
            //            row["Allowance"] = AllowanceCheckBox.Checked;
            //        else
            //            row["Allowance"] = false;

            //        if (OvertimeCheckBox.Visible == true)
            //            row["Overtime"] = OvertimeCheckBox.Checked;
            //        else
            //            row["Overtime"] = false;

            //        if (CPFIncomeCheckBox.Visible == true)
            //            row["CPFIncome"] = CPFIncomeCheckBox.Checked;
            //        else
            //            row["CPFIncome"] = false;

            //        if (AHGIncomeCheckBox.Visible == true)
            //            row["AHGIncome"] = AHGIncomeCheckBox.Checked;
            //        else
            //            row["AHGIncome"] = false;
            //        #endregion
            //        dt.Rows.Add(row);
            //    }
            //}
            //IncomeDetailsRadGrid.DataSource = dt;
            //IncomeDetailsRadGrid.Rebind();
            //dt.Merge(dtTemp);
            #endregion
        }
    }


    #region Divide Assessment
    private void DivideAssessment()
    {
        try
        {
            DataTable dtAllIncome = IncomeDb.GetDataForIncomeAssessment(ReqDocAppId.Value, ReqNRIC);

            foreach (DataRow rowAllIncome in dtAllIncome.Rows)
            {
                DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(rowAllIncome["Id"].ToString()));
                int VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;
                int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(rowAllIncome["Id"].ToString()), VersionNo, (Guid)Membership.GetUser().ProviderUserKey);

                //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
                //{
                //    Label IdLabel = (Label)item.FindControl("IdLabel");
                //    string IncomeItem = string.Empty;
                //    decimal IncomeAmount = 0;
                //    bool Allowance = false;
                //    bool CPFIncome = false;
                //    bool GrossIncome = false;
                //    bool AHGIncome = false;
                //    bool Overtime = false;

                //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
                //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
                //    if (IncomeItemTextBox.Visible == true)
                //        IncomeItem = IncomeItemTextBox.Text;
                //    else
                //        IncomeItem = IncomeItemLabel.Text;

                //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
                //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
                //    if (AmountTextBox.Visible == true)
                //        IncomeAmount = decimal.Parse(AmountTextBox.Text);
                //    else
                //        IncomeAmount = decimal.Parse(AmountLabel.Text);

                //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
                //    if (AllowanceCheckBox.Visible == true)
                //        Allowance = AllowanceCheckBox.Checked;

                //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
                //    if (CPFIncomeCheckBox.Visible == true)
                //        CPFIncome = CPFIncomeCheckBox.Checked;

                //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
                //    if (GrossIncomeCheckBox.Visible == true)
                //        GrossIncome = GrossIncomeCheckBox.Checked;

                //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
                //    if (AHGIncomeCheckBox.Visible == true)
                //        AHGIncome = AHGIncomeCheckBox.Checked;

                //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
                //    if (OvertimeCheckBox.Visible == true)
                //        Overtime = OvertimeCheckBox.Checked;

                //    decimal NewIncomeAmount = IncomeAmount / dtAllIncome.Rows.Count;

                //    int result = IncomeDb.InsertIncomeDetails(NewVersionId, IncomeItem, NewIncomeAmount,
                //       Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);

                //}
                int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(rowAllIncome["Id"].ToString()), NewVersionId);
                InsertIntoViewLog(int.Parse(rowAllIncome["Id"].ToString()));
            }
            CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            #region Added By Edward  24/02/2014 Add Icon and Action Log
            //IncomeDb.InsertExtractionLog(int.Parse(MonthYearDropDownList.SelectedValue), string.Empty, string.Empty,
            //    LogActionEnum.Divide_Amount_by_REPLACE1, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            #endregion
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true); ;
        }
    }
    #endregion

    #region Copy Assessment
    private void CopyAssessment()
    {
        try
        {
            string[] strArray = Session["Copy"].ToString().Split(';');
            string IncludeFigures = Session["Figures"].ToString();
            string[] strIncomeMonthYear = Session["IncomeMonthYear"].ToString().Split(';');     //Added BY Edward 24/02/2014 Add Icon and Action Log
            Session["Copy"] = null;
            Session["Figures"] = null;
            Session["IncomeMonthYear"] = null;          //Added BY Edward 24/02/2014 Add Icon and Action Log

            foreach (string str in strArray)
            {
                DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(str));
                int VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;
                int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(str), VersionNo, (Guid)Membership.GetUser().ProviderUserKey);

                //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
                //{
                //    Label IdLabel = (Label)item.FindControl("IdLabel");
                //    string IncomeItem = string.Empty;
                //    decimal IncomeAmount = 0;
                //    bool Allowance = false;
                //    bool CPFIncome = false;
                //    bool GrossIncome = false;
                //    bool AHGIncome = false;
                //    bool Overtime = false;

                //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
                //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
                //    if (IncomeItemTextBox.Visible == true)
                //        IncomeItem = IncomeItemTextBox.Text;
                //    else
                //        IncomeItem = IncomeItemLabel.Text;

                //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
                //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
                //    if (AmountTextBox.Visible == true)
                //        IncomeAmount = decimal.Parse(AmountTextBox.Text);
                //    else
                //        IncomeAmount = decimal.Parse(AmountLabel.Text);

                //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
                //    if (AllowanceCheckBox.Visible == true)
                //        Allowance = AllowanceCheckBox.Checked;

                //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
                //    if (CPFIncomeCheckBox.Visible == true)
                //        CPFIncome = CPFIncomeCheckBox.Checked;

                //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
                //    if (GrossIncomeCheckBox.Visible == true)
                //        GrossIncome = GrossIncomeCheckBox.Checked;

                //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
                //    if (AHGIncomeCheckBox.Visible == true)
                //        AHGIncome = AHGIncomeCheckBox.Checked;

                //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
                //    if (OvertimeCheckBox.Visible == true)
                //        Overtime = OvertimeCheckBox.Checked;

                //    int result = IncomeDb.InsertIncomeDetails(NewVersionId, IncomeItem, IncludeFigures == "Y" ? IncomeAmount : 0,
                //       Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);

                //}
                int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(str), NewVersionId);
                InsertIntoViewLog(int.Parse(str));
            }
            CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);

            #region Added By Edward  24/02/2014 Add Icon and Action Log
            System.Text.StringBuilder strB = new System.Text.StringBuilder();

            foreach (string str in strIncomeMonthYear)
            {
                strB.Append(str + " ");
            }
            //IncomeDb.InsertExtractionLog(int.Parse(MonthYearDropDownList.SelectedValue), strB.ToString().Substring(0, strB.Length > 280 ? 280 : strB.Length), string.Empty,
            //    LogActionEnum.REPLACE1_copied_to_REPLACE2, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            #endregion

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", ex.Message), true); ;
        }
    }
    #endregion

    #region Set Income To Blank

    private void SetIncomeToBlank()
    {
        try
        {
            //DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(MonthYearDropDownList.SelectedValue));
            //if (dtIncomeVersion.Rows.Count > 0)
            //{
            //    foreach (DataRow IncomeVersionRow in dtIncomeVersion.Rows)
            //        IncomeDb.DeleteAllIncomeDetailsByVersionId(int.Parse(IncomeVersionRow["Id"].ToString()));
            //    IncomeDb.DeleteAllIncomeVersionsByIncomeId(int.Parse(MonthYearDropDownList.SelectedValue));
            //    int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(MonthYearDropDownList.SelectedValue), 0);
            //}
            //IncomeDb.UpdateIncomeSetToBlank(int.Parse(MonthYearDropDownList.SelectedValue), true);
            //IncomeDb.DeleteCreditAssessment(ReqAppPersonalId.Value);
            //CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            //#region Added By Edward  24/02/2014 Add Icon and Action Log
            //IncomeDb.InsertExtractionLog(int.Parse(MonthYearDropDownList.SelectedValue), string.Empty, string.Empty,
            //    LogActionEnum.Set_Income_to_Blank_by_REPLACE1, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            //#endregion

        }
        catch (Exception ex)
        {
            string msg = string.Format("There's a problem in Setting the Income to blank. {0}", ex.Message);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "test", String.Format("javascript:alert('{0}');", msg), true);
        }

    }
    #endregion

    #region CHECKBOX EVENTS

    protected void AllowanceCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        //if (ChkAllowanceDic.ContainsKey(chk.UniqueID))
        //    ChkAllowanceDic[chk.UniqueID] = chk.Checked;
        //string GrossIncomeString = "GrossIncomeCheckBox";
        //string AllowanceString = "AllowanceCheckBox";
        //string OvertimeString = "OvertimeCheckBox";
        //string NewUniqueId = chk.UniqueID.Substring(0, chk.UniqueID.Length - AllowanceString.Length);
        //if (!chk.Checked)
        //{
        //    ChkOvertimeDic[NewUniqueId + OvertimeString] = false;
        //}
        //else
        //{
        //    ChkGrossIncomeDic[NewUniqueId + GrossIncomeString] = true;
        //    ChkOvertimeDic[NewUniqueId + OvertimeString] = false;
        //}
        DataTable dt = new DataTable();
        dt = AddDataTableColumns(dt);
        //IncomeDetailsRadGrid.DataSource = GetItemsFromRadGrid(dt, chk, "ALLOWANCE");
        //IncomeDetailsRadGrid.Rebind();
    }

    protected void CPFIncomeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        if (ChkCPFIncomeDic.ContainsKey(chk.UniqueID))
            ChkCPFIncomeDic[chk.UniqueID] = chk.Checked;
        //IncomeDetailsRadGrid.Rebind();
    }

    protected void GrossIncomeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        //if (ChkGrossIncomeDic.ContainsKey(chk.UniqueID))
        //    ChkGrossIncomeDic[chk.UniqueID] = chk.Checked;
        //string GrossIncomeString = "GrossIncomeCheckBox";
        //string AllowanceString = "AllowanceCheckBox";
        //string OvertimeString = "OvertimeCheckBox";
        //string NewUniqueId = chk.UniqueID.Substring(0, chk.UniqueID.Length - GrossIncomeString.Length);
        //if (!chk.Checked)
        //{            
        //    ChkAllowanceDic[NewUniqueId + AllowanceString] = false;
        //}
        //else
        //{
        //    ChkOvertimeDic[NewUniqueId + OvertimeString] = false;
        //}
        DataTable dt = new DataTable();
        dt = AddDataTableColumns(dt);
        //IncomeDetailsRadGrid.DataSource = GetItemsFromRadGrid(dt, chk, "GROSSINCOME");
        //IncomeDetailsRadGrid.Rebind();


    }

    protected void AHGIncomeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        //if (ChkAHGIncomeDic.ContainsKey(chk.UniqueID))
        //    ChkAHGIncomeDic[chk.UniqueID] = chk.Checked;
        DataTable dt = new DataTable();
        dt = AddDataTableColumns(dt);
        //IncomeDetailsRadGrid.DataSource = GetItemsFromRadGrid(dt, chk, "AHGINCOME");
        //IncomeDetailsRadGrid.Rebind();
    }

    protected void OvertimeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        //if (ChkOvertimeDic.ContainsKey(chk.UniqueID))
        //    ChkOvertimeDic[chk.UniqueID] = chk.Checked;
        //string GrossIncomeString = "GrossIncomeCheckBox";
        //string AllowanceString = "AllowanceCheckBox";
        //string OvertimeString = "OvertimeCheckBox";
        //string NewUniqueId = chk.UniqueID.Substring(0, chk.UniqueID.Length - OvertimeString.Length);
        //if (chk.Checked)
        //{
        //    ChkGrossIncomeDic[NewUniqueId + GrossIncomeString] = false;
        //    ChkAllowanceDic[NewUniqueId + AllowanceString] = false;
        //}
        DataTable dt = new DataTable();
        dt = AddDataTableColumns(dt);
        //IncomeDetailsRadGrid.DataSource = GetItemsFromRadGrid(dt, chk, "OVERTIME");
        //IncomeDetailsRadGrid.Rebind();
    }



    #endregion

    #region DICTIONARIES
    /// <summary>
    /// Stores the UniqueIds of the Allowance Checkboxes
    /// </summary>
    private Dictionary<string, bool> ChkAllowanceDic
    {
        get { return (Dictionary<string, bool>)ViewState["ChkAllowanceDic"]; }
        set { ViewState["ChkAllowanceDic"] = value; }
    }
    /// <summary>
    /// Stores the UniqueIds of the CPFIncome Checkboxes
    /// </summary>
    private Dictionary<string, bool> ChkCPFIncomeDic
    {
        get { return (Dictionary<string, bool>)ViewState["ChkCPFIncomeDic"]; }
        set { ViewState["ChkCPFIncomeDic"] = value; }
    }
    /// <summary>
    /// Stores the UniqueIds of the GrossIncome Checkboxes
    /// </summary>
    private Dictionary<string, bool> ChkGrossIncomeDic
    {
        get { return (Dictionary<string, bool>)ViewState["ChkGrossIncomeDic"]; }
        set { ViewState["ChkGrossIncomeDic"] = value; }
    }
    /// <summary>
    /// Stores the UniqueIds of the GrossIncome Checkboxes
    /// </summary>
    private Dictionary<string, bool> ChkAHGIncomeDic
    {
        get { return (Dictionary<string, bool>)ViewState["ChkAHGIncomeDic"]; }
        set { ViewState["ChkAHGIncomeDic"] = value; }
    }

    private Dictionary<string, bool> ChkOvertimeDic
    {
        get { return (Dictionary<string, bool>)ViewState["ChkOvertimeDic"]; }
        set { ViewState["ChkOvertimeDic"] = value; }
    }

    private Dictionary<string, CurrencyTotals> CurrencyTotalDic
    {
        get { return (Dictionary<string, CurrencyTotals>)ViewState["CurrencyTotalDic"]; }
        set { ViewState["CurrencyTotalDic"] = value; }
    }

    private Dictionary<string, string> IncomeItemDic
    {
        get { return (Dictionary<string, string>)ViewState["IncomeItemDic"]; }
        set { ViewState["IncomeItemDic"] = value; }
    }

    private Dictionary<string, decimal> IncomeAmountDic
    {
        get { return (Dictionary<string, decimal>)ViewState["IncomeAmountDic"]; }
        set { ViewState["IncomeAmountDic"] = value; }
    }

    #endregion

    #region OTHER VIEWSTATE VARIABLES
    /// <summary>
    /// Stores the IncomeId to be used particularly for the History Log
    /// </summary>
    private int IncomeId
    {
        get { return (int)ViewState["IncomeId"]; }
        set { ViewState["IncomeId"] = value; }
    }
    /// <summary>
    /// Number of Items in the IncomeDetailsRadgrid excluding the Totals
    /// </summary>
    private int IncomeDetailsRadGridCount
    {
        get { return (int)ViewState["IncomeDetailsRadGridCount"]; }
        set { ViewState["IncomeDetailsRadGridCount"] = value; }
    }
    /// <summary>
    /// Determines if its to append or replace items when Drag and Drop triggers
    /// </summary>
    private string DragAndDropAction
    {
        get { return (string)ViewState["DragAndDropAction"]; }
        set { ViewState["DragAndDropAction"] = value; }
    }
    /// <summary>
    /// Determines if the flag is 1 it will not load the Income
    /// </summary>
    private int EditFlag
    {
        get { return (int)ViewState["EditFlag"]; }
        set { ViewState["EditFlag"] = value; }
    }

    private int VersionIdToDeleteItems
    {
        get { return (int)ViewState["VersionIdToDeleteItems"]; }
        set { ViewState["VersionIdToDeleteItems"] = value; }
    }
    #endregion

    #region Insert, Update, Delete Methods and Events

    #region SAVE BUTTON
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (ValidateZoningTable())      //Added By Edward February 21, 2014 Meeting
        {
            int a = 0;
            //if (TemplateDropdownList.SelectedValue != "new")
            //{
            //    if (int.TryParse(VersionDropdownList.SelectedValue, out a) && TemplateDropdownList.SelectedValue.Contains("tem"))      //from the database
            //        UpdateIncomeVersionAndIncomeDetails();
            //    else        //new and other templates
            //        InsertNewIncomeVersionAndIncomeDetails();
            //}
            //else
            //    InsertNewIncomeVersionAndIncomeDetails();
            //int IncomeVersionResult = IncomeDb.UpdateIncomeSetToBlank(int.Parse(MonthYearDropDownList.SelectedValue), false);

            //CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            //#region Added By Edward  24/02/2014 Add Icon and Action Log
            //IncomeDb.InsertExtractionLog(int.Parse(MonthYearDropDownList.SelectedValue), MonthYearDropDownList.Text, string.Empty,
            //    LogActionEnum.Save_Income_REPLACE2_by_REPLACE1, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            //#endregion
        }
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Error", String.Format("javascript:alert('{0}');", "Please enter a correct amount."), true);
        }
        //Session["ReqIncomeId"] = StoreIncomeId.Value;
    }
    #endregion

    #region INSERT INCOMEVERSION AND INCOMEDETAILS
    private void InsertNewIncomeVersionAndIncomeDetails()
    {
        //DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(MonthYearDropDownList.SelectedValue));
        //int VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;

        //int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(MonthYearDropDownList.SelectedValue), VersionNo, (Guid)Membership.GetUser().ProviderUserKey);
        //int i = 0;
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    //if (i < IncomeDetailsRadGridCount)
        //    //{
        //    string IncomeItem = string.Empty;
        //    decimal IncomeAmount = 0;
        //    bool Allowance = false;
        //    bool CPFIncome = false;
        //    bool GrossIncome = false;
        //    bool AHGIncome = false;
        //    bool Overtime = false;

        //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
        //    if (IncomeItemTextBox.Visible == true)
        //        IncomeItem = IncomeItemTextBox.Text;
        //    else
        //        IncomeItem = IncomeItemLabel.Text;

        //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //    if (AmountTextBox.Visible == true)
        //        IncomeAmount = decimal.Parse(AmountTextBox.Text);
        //    else
        //        IncomeAmount = decimal.Parse(AmountLabel.Text);

        //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
        //    if (AllowanceCheckBox.Visible == true)
        //        Allowance = AllowanceCheckBox.Checked;

        //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
        //    if (CPFIncomeCheckBox.Visible == true)
        //        CPFIncome = CPFIncomeCheckBox.Checked;

        //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
        //    if (GrossIncomeCheckBox.Visible == true)
        //        GrossIncome = GrossIncomeCheckBox.Checked;

        //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
        //    if (AHGIncomeCheckBox.Visible == true)
        //        AHGIncome = AHGIncomeCheckBox.Checked;

        //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
        //    if (OvertimeCheckBox.Visible == true)
        //        Overtime = OvertimeCheckBox.Checked;

        //    int result = IncomeDb.InsertIncomeDetails(NewVersionId, IncomeItem, IncomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);
        //    //}
        //    i++;
        //}
        //int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(MonthYearDropDownList.SelectedValue), NewVersionId);

        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
    }
    #endregion

    #region UPDATE INCOMEVERSION AND INCOMEDETAILS
    private void UpdateIncomeVersionAndIncomeDetails()
    {
        if (VersionIdToDeleteItems > 0)
            IncomeDb.DeleteAllIncomeDetailsByVersionId(VersionIdToDeleteItems);

        //int i = 0;
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{

        //    Label IdLabel = (Label)item.FindControl("IdLabel");

        //    string IncomeItem = string.Empty;
        //    decimal IncomeAmount = 0;
        //    bool Allowance = false;
        //    bool CPFIncome = false;
        //    bool GrossIncome = false;
        //    bool AHGIncome = false;
        //    bool Overtime = false;

        //    TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //    Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
        //    if (IncomeItemTextBox.Visible == true)
        //        IncomeItem = IncomeItemTextBox.Text;
        //    else
        //        IncomeItem = IncomeItemLabel.Text;

        //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //    if (AmountTextBox.Visible == true)
        //        IncomeAmount = decimal.Parse(AmountTextBox.Text);
        //    else
        //        IncomeAmount = decimal.Parse(AmountLabel.Text);

        //    CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
        //    if (AllowanceCheckBox.Visible == true)
        //        Allowance = AllowanceCheckBox.Checked;

        //    CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
        //    if (CPFIncomeCheckBox.Visible == true)
        //        CPFIncome = CPFIncomeCheckBox.Checked;

        //    CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
        //    if (GrossIncomeCheckBox.Visible == true)
        //        GrossIncome = GrossIncomeCheckBox.Checked;

        //    CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
        //    if (AHGIncomeCheckBox.Visible == true)
        //        AHGIncome = AHGIncomeCheckBox.Checked;

        //    CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
        //    if (OvertimeCheckBox.Visible == true)
        //        Overtime = OvertimeCheckBox.Checked;

        //    int result = IncomeDb.UpdateIncomeDetails(int.Parse(IdLabel.Text), int.Parse(VersionDropdownList.SelectedValue), IncomeItem, IncomeAmount,
        //        Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);

        //    if (result == 0)
        //    {
        //        int Newresult = IncomeDb.InsertIncomeDetails(int.Parse(VersionDropdownList.SelectedValue), IncomeItem, IncomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);
        //    }
        //}
        //IncomeDb.UpdateIncomeVersion(int.Parse(VersionDropdownList.SelectedValue), (Guid)Membership.GetUser().ProviderUserKey);
        //int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(MonthYearDropDownList.SelectedValue), int.Parse(VersionDropdownList.SelectedValue));
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "RefreshPage", "javascript:reloadPage();", true);
    }
    #endregion

    #region DELETE ROWS IN NEW VERSION
    //protected void DeleteTemp_Click(object sender, ImageClickEventArgs e)
    //{
    //    DataTable dt = new DataTable();
    //    DataRow row;
    //    dt.Columns.Add("Id");
    //    dt.Columns.Add("IncomeVersionId");
    //    dt.Columns.Add("IncomeItem");
    //    dt.Columns.Add("IncomeAmount");
    //    dt.Columns.Add("Allowance");
    //    dt.Columns.Add("CPFIncome");
    //    dt.Columns.Add("GrossIncome");
    //    dt.Columns.Add("AHGIncome");
    //    dt.Columns.Add("Overtime");
    //    int i = 0;
    //    foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
    //    {
    //        if (!item.Selected)
    //        {
    //            row = dt.NewRow();
    //            Label ContainerLabel = (Label)item.FindControl("ContainerLabel");
    //            row["Id"] = ContainerLabel.Text;
    //            row["IncomeVersionId"] = "0";
    //            TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
    //            Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
    //            if (IncomeItemTextBox.Visible == true)
    //                row["IncomeItem"] = IncomeItemTextBox.Text;
    //            else
    //                row["IncomeItem"] = IncomeItemLabel.Text;
    //            TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
    //            Label AmountLabel = item.FindControl("AmountLabel") as Label;
    //            if (AmountTextBox.Visible == true)
    //                row["IncomeAmount"] = decimal.Parse(AmountTextBox.Text);
    //            else
    //                row["IncomeAmount"] = decimal.Parse(AmountLabel.Text);
    //            CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
    //            if (AllowanceCheckBox.Visible == true)
    //                row["Allowance"] = AllowanceCheckBox.Checked;
    //            CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
    //            if (CPFIncomeCheckBox.Visible == true)
    //                row["CPFIncome"] = CPFIncomeCheckBox.Checked;
    //            CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
    //            if (GrossIncomeCheckBox.Visible == true)
    //                row["GrossIncome"] = GrossIncomeCheckBox.Checked;
    //            CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;
    //            if (AHGIncomeCheckBox.Visible == true)
    //                row["AHGIncome"] = AHGIncomeCheckBox.Checked;
    //            CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
    //            if (OvertimeCheckBox.Visible == true)
    //                row["Overtime"] = AHGIncomeCheckBox.Checked;
    //            dt.Rows.Add(row);
    //            i = !string.IsNullOrEmpty(ContainerLabel.Text) ? int.Parse(ContainerLabel.Text) : 0;
    //        }
    //    }
    //    IncomeDetailsRadGrid.DataSource = dt;
    //    IncomeDetailsRadGrid.Rebind();
    //}

    private void DeleteItems()
    {
        DataTable dt = new DataTable();
        DataRow row;
        dt.Columns.Add("Id");
        dt.Columns.Add("IncomeVersionId");
        dt.Columns.Add("IncomeItem");
        dt.Columns.Add("IncomeAmount");
        dt.Columns.Add("Allowance");
        dt.Columns.Add("CPFIncome");
        dt.Columns.Add("GrossIncome");
        dt.Columns.Add("AHGIncome");
        dt.Columns.Add("Overtime");
        //int i = 0;
        int[] ids;
        string idsToDelete = string.Empty;
        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    Label IdLabel = (Label)item.FindControl("IdLabel");

        //    if (!item.Selected)
        //    {
        //        row = dt.NewRow();
        //        row["Id"] = IdLabel.Text;
        //        row["IncomeVersionId"] = "0";
        //        TextBox IncomeItemTextBox = (TextBox)item.FindControl("IncomeItemTextBox");
        //        Label IncomeItemLabel = (Label)item.FindControl("IncomeItemLabel");
        //        if (IncomeItemTextBox.Visible == true)
        //            row["IncomeItem"] = IncomeItemTextBox.Text;
        //        else
        //            row["IncomeItem"] = IncomeItemLabel.Text;
        //        TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //        Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //        if (AmountTextBox.Visible == true)
        //            row["IncomeAmount"] = decimal.Parse(AmountTextBox.Text);
        //        else
        //            row["IncomeAmount"] = decimal.Parse(AmountLabel.Text);

        //        #region Assign the Checkboxes
        //        CheckBox GrossIncomeCheckBox = item.FindControl("GrossIncomeCheckBox") as CheckBox;
        //        CheckBox AllowanceCheckBox = item.FindControl("AllowanceCheckBox") as CheckBox;
        //        CheckBox OvertimeCheckBox = item.FindControl("OvertimeCheckBox") as CheckBox;
        //        CheckBox CPFIncomeCheckBox = item.FindControl("CPFIncomeCheckBox") as CheckBox;
        //        CheckBox AHGIncomeCheckBox = item.FindControl("AHGIncomeCheckBox") as CheckBox;


        //        if (GrossIncomeCheckBox.Visible == true)
        //            row["GrossIncome"] = GrossIncomeCheckBox.Checked;
        //        else
        //            row["GrossIncome"] = false;

        //        if (AllowanceCheckBox.Visible == true)
        //            row["Allowance"] = AllowanceCheckBox.Checked;
        //        else
        //            row["Allowance"] = false;

        //        if (OvertimeCheckBox.Visible == true)
        //            row["Overtime"] = OvertimeCheckBox.Checked;
        //        else
        //            row["Overtime"] = false;

        //        if (CPFIncomeCheckBox.Visible == true)
        //            row["CPFIncome"] = CPFIncomeCheckBox.Checked;
        //        else
        //            row["CPFIncome"] = false;

        //        if (AHGIncomeCheckBox.Visible == true)
        //            row["AHGIncome"] = AHGIncomeCheckBox.Checked;
        //        else
        //            row["AHGIncome"] = false;
        //        #endregion

        //        dt.Rows.Add(row);
        //        //i = !string.IsNullOrEmpty(ContainerLabel.Text) ? int.Parse(ContainerLabel.Text) : 0;
        //    }
        //    else
        //    {
        //        if (int.Parse(IdLabel.Text) != 0)
        //        {
        //            idsToDelete = string.IsNullOrEmpty(idsToDelete) ? IdLabel.Text : idsToDelete + ", " + IdLabel.Text;
        //        }
        //    }
        //}
        if (!string.IsNullOrEmpty(idsToDelete))
        {
            string[] idStrArray = idsToDelete.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ids = new int[idStrArray.Length];
            int count = 0;
            foreach (string idStr in idStrArray)
            {
                ids[count] = int.Parse(idStr);
                count++;
            }

            //foreach (int id in ids)
            //{
            //    DataTable itemToDeleteDT = IncomeDb.GetIncomeDetailsById(id);
            //    if (itemToDeleteDT.Rows.Count > 0)
            //    {
            //        DataRow r = itemToDeleteDT.Rows[0];
            //        IncomeDb.DeleteCreditAssessment(ReqAppPersonalId.Value,r["IncomeItem"].ToString(),
            //    }
            //}

            int result = IncomeDb.DeleteIncomeItems(ids);
            IncomeDb.DeleteCreditAssessment(ReqAppPersonalId.Value);
            CreditAssessmentDb.AutoGenerateCreditAssessment(ReqAppPersonalId.Value);
            #region Added By Edward  24/02/2014 Add Icon and Action Log
            //IncomeDb.InsertExtractionLog(int.Parse(MonthYearDropDownList.SelectedValue), string.Empty, string.Empty,
            //    LogActionEnum.Delete_Income_Items_by_REPLACE1, LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            #endregion
        }
        //IncomeDetailsRadGrid.DataSource = dt;
        //IncomeDetailsRadGrid.Rebind();
    }
    #endregion

    #endregion

    #region Class for Currency Totals to be stored in the dictionary
    [Serializable]
    private class CurrencyTotals
    {
        private decimal _CurrencyRate;

        public decimal CurrencyRate
        {
            get { return _CurrencyRate; }
            set { _CurrencyRate = value; }
        }

        private string _Currency;

        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }
    }
    #endregion

    protected void ConfirmButton_Click(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// This will update the Assessment Status to Extraction in Progress when the Assessment Status is Pending_Extraction
    /// </summary>
    private void ChangeDocAppAssessmentStatus()
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

    #region Added By Edward 27/2/2014 February 21, 2014 Meeting
    private bool ValidateZoningTable()
    {
        bool CanSave = true;

        //foreach (GridDataItem item in IncomeDetailsRadGrid.MasterTableView.Items)
        //{
        //    Label IdLabel = (Label)item.FindControl("IdLabel");

        //    decimal IncomeAmount = 0;
        //    bool Validate = false;

        //    TextBox AmountTextBox = item.FindControl("AmountTextBox") as TextBox;
        //    Label AmountLabel = item.FindControl("AmountLabel") as Label;
        //    Validate = decimal.TryParse(AmountTextBox.Text, out IncomeAmount);

        //    if (Validate == false)
        //    {
        //        AmountTextBox.BackColor = Color.Yellow;
        //        CanSave = false;
        //    }
        //    else
        //        AmountTextBox.BackColor = Color.White;
        //}

        return CanSave;
    }

    #endregion

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
}
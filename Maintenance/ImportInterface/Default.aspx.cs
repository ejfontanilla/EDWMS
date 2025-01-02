using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;
using Dwms.Web;
using Dwms.Bll;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Text;

public partial class Maintenance_ImportInterface_Default : System.Web.UI.Page
{
    bool hasManageAllAccess = false;
    bool hasManageDepartmentAccess = false;

    //string br = "<br/>";          Commented by Edward 2016/02/04 take out unused variables

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        //ConfirmPanel.Visible = (Request["cfm"] == "1");

        //// Set the access control for the user
        //SetAccessControl();

        ////Get User department and show only those tabs related to the user department
        //if (hasManageDepartmentAccess)
        //{
        //    MembershipUser user = Membership.GetUser();
        //    DepartmentDb departmentDb = new DepartmentDb();
        //    Department.DepartmentDataTable departments = departmentDb.GetDepartmentByUserId((Guid)user.ProviderUserKey);
        //    Department.DepartmentRow departmentRow = departments[0];

        //    switch (departmentRow.Code.ToUpper().Trim())
        //    {
        //        case "AAD": // Admin & Accounting Department 
        //            CosRadPageView.Selected = true;
        //            RadTabStrip1.Tabs[0].Selected = true;

        //            RadTabStrip1.Tabs[1].Enabled = false;
        //            RadTabStrip1.Tabs[2].Enabled = false;
        //            RadTabStrip1.Tabs[3].Enabled = false;
        //            RadTabStrip1.DataBind();
        //            break;
        //        case "PRD": //Projects & Redevelopment Department 
        //            SersRadPageView.Selected = true;
        //            RadTabStrip1.Tabs[2].Selected = true;

        //            RadTabStrip1.Tabs[0].Enabled = false;
        //            RadTabStrip1.Tabs[1].Enabled = false;
        //            RadTabStrip1.Tabs[3].Enabled = false;
        //            RadTabStrip1.Tabs[4].Enabled = false;
        //            RadTabStrip1.DataBind();
        //            break;
        //        case "RSD": // Resale Department 
        //            ResaleRadPageView.Selected = true;
        //            RadTabStrip1.Tabs[1].Selected = true;

        //            RadTabStrip1.Tabs[0].Enabled = false;
        //            RadTabStrip1.Tabs[2].Enabled = false;
        //            RadTabStrip1.Tabs[3].Enabled = false;
        //            RadTabStrip1.DataBind();
        //            break;
        //        case "SSD": //Sales Department 
        //            SalesRadPageView.Selected = true;
        //            RadTabStrip1.Tabs[3].Selected = true;

        //            RadTabStrip1.Tabs[0].Enabled = false;
        //            RadTabStrip1.Tabs[1].Enabled = false;
        //            RadTabStrip1.Tabs[2].Enabled = false;
        //            RadTabStrip1.Tabs[4].Enabled = false;
        //            RadTabStrip1.DataBind();
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    /// <summary>
    /// Import button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ImportButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string result = string.Empty;
            int uniqueRefNoCnt = 0;
            int successCnt = 0;
            int failedCnt = 0;
            string error = string.Empty;

            if (RadTabStrip1.SelectedTab.Text.ToUpper().Equals("COS"))
            {
                if (HouseholdRadUpload.UploadedFiles.Count > 0)
                    ImportCosHouseholdInterfaceFile(out successCnt, out failedCnt, out error, out uniqueRefNoCnt);
                else
                    return;

                ////delete wrong records uploaded. [******************** DO NOT RUN THIS *********************]
                //HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                //hleInterfaceDb.DeleteWrongRecords();

                if (!String.IsNullOrEmpty(error))
                    result += error;
                else
                    result += "COS import finished!";
            }
            else if (RadTabStrip1.SelectedTab.Text.ToUpper().Equals("RESALE"))
            {
                if (RosRadUpload.UploadedFiles.Count > 0)
                    ImportRosHouseholdInterfaceFile(out successCnt, out failedCnt, out error, out uniqueRefNoCnt);
                else
                    return;

                if (!String.IsNullOrEmpty(error))
                    result += error;
                else
                    result += "RESALE import finished!";
            }
            else if (RadTabStrip1.SelectedTab.Text.ToUpper().Equals("SERS"))
            {
                if (SersRadUpload.UploadedFiles.Count > 0)
                    ImportSersHouseholdInterfaceFile(out successCnt, out failedCnt, out error, out uniqueRefNoCnt);
                else
                    return;

                if (!String.IsNullOrEmpty(error))
                    result += error;
                else
                    result += "SERS import finished!";
            }
            else if (RadTabStrip1.SelectedTab.Text.ToUpper().Equals("SALES"))
            {
                if (SalesRadUpload.UploadedFiles.Count > 0)
                    ImportSalesHouseholdInterfaceFile(out successCnt, out failedCnt, out error, out uniqueRefNoCnt);
                else
                    return;

                if (!String.IsNullOrEmpty(error))
                    result += error;
                else
                    result += "SALES import finished!";
            }


            if (failedCnt != 0 || !String.IsNullOrEmpty(error))
            {
                WarningPanel.Visible = true;
                ConfirmPanel.Visible = false;

                WarningLabel.Text = result;
            }
            else
            {
                WarningPanel.Visible = false;
                ConfirmPanel.Visible = true;

                SummaryLabel.Text = result;
            }

            //Response.Redirect("Default.aspx?cfm=1");
        }
    }    
    #endregion

    #region Validation
    /// <summary>
    /// Household upload validation
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void HouseholdRadUpload_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        result = !(HouseholdRadUpload.InvalidFiles.Count > 0 && RadTabStrip1.SelectedTab.Text.ToUpper().Equals("COS"));
            
        args.IsValid = result;
    }

    /// <summary>
    /// ROS Household upload validation
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void RosCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        result = !(RosRadUpload.InvalidFiles.Count > 0 && RadTabStrip1.SelectedTab.Text.ToUpper().Equals("RESALE"));

        args.IsValid = result;
    }

    /// <summary>
    /// SERS Household upload validation
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void SersCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        result = !(SersRadUpload.InvalidFiles.Count > 0 && RadTabStrip1.SelectedTab.Text.ToUpper().Equals("SERS"));

        args.IsValid = result;
    }

    /// <summary>
    /// SALES Household upload validation
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void SalesCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool result = false;

        result = !(SalesRadUpload.InvalidFiles.Count > 0 && RadTabStrip1.SelectedTab.Text.ToUpper().Equals("SALES"));

        args.IsValid = result;
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasManageAllAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);
        hasManageDepartmentAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_Department);

        bool hasAccess = (hasManageAllAccess || hasManageDepartmentAccess);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }

    #region COS
    /// <summary>
    /// Insert the household interface data
    /// </summary>
    private void ImportCosHouseholdInterfaceFile(out int successCnt, out int failedCnt, out string error, out int uniqueRefNoCnt)
    {
        uniqueRefNoCnt = 0;
        successCnt = 0;
        failedCnt = 0;
        error = string.Empty;

        if (HouseholdRadUpload.UploadedFiles.Count > 0)
        {
            UploadedFile uploadedFile = HouseholdRadUpload.UploadedFiles[0];
            StreamReader stream = new StreamReader(uploadedFile.InputStream);

            string s;
            ArrayList hlePersonalList = new ArrayList();
            //ArrayList hleNumberList = new ArrayList();
            Dictionary<string, string> hleNumberList = new Dictionary<string, string>();
            string caOic = string.Empty;

            while ((s = stream.ReadLine()) != null)
            {
                string[] dataItems = s.Split(new string[] { "*" }, StringSplitOptions.None);

                if (dataItems.Length != 352)
                {
                    error = "Please upload a correct COS interface file.";
                    return;
                }

                // Get the HLE data                    
                string hleNumber = dataItems[0].Trim().ToUpper();

                if (!String.IsNullOrEmpty(hleNumber) && Validation.IsHLENumber(hleNumber))
                {
                    string applicationDate = dataItems[1].Trim();
                    string hleStatus = dataItems[2].Trim();
                    hleStatus = EnumManager.MapHleStatus(hleStatus);
                    string hhIncome = dataItems[3].Trim();
                    string caIncome = dataItems[4].Trim();
                    string peOic = dataItems[5].Trim();
                    caOic = dataItems[6].Trim();
                    string rejDate = dataItems[348].Trim();
                    string canDate = dataItems[349].Trim();
                    string aprDate = dataItems[350].Trim();
                    string expDate = dataItems[351].Trim();

                    // In the Household interface file, there will be a maximum of 4 Applications followed by 6 Occupiers
                    #region Get the personal info of HA
                    GetCosPersonalData(dataItems, hleNumber, applicationDate, hleStatus, hhIncome, caIncome, peOic, caOic, rejDate, canDate, aprDate, expDate,
                        ref hlePersonalList, PersonalTypeEnum.HA);
                    #endregion

                    #region Get the personal info of OC
                    GetCosPersonalData(dataItems, hleNumber, applicationDate, hleStatus, hhIncome, caIncome, peOic, caOic, rejDate, canDate, aprDate, expDate,
                        ref hlePersonalList, PersonalTypeEnum.OC);
                    #endregion

                    // Create the unique list of HLE numbers
                    if (!hleNumberList.ContainsKey(hleNumber))
                    {
                        hleNumberList.Add(hleNumber, caOic);
                    }
                }
            }


            // Insert the unique HLE numbers
            if (hleNumberList.Count > 0)
            {
                DocAppDb docAppDb = new DocAppDb();

                foreach (KeyValuePair<string, string> hleNumber in hleNumberList)
                {
                    //if (!docAppDb.DoesRefNoExists(hleNumber.Key))
                    //{
                    //    docAppDb.Insert(hleNumber.Key, Util.GetReferenceType(hleNumber.Key), null, null,
                    //        AppStatusEnum.Pending_Documents, null, caOic, string.Empty, null, false);
                    //}
                    //else
                    //{
                    //    // Update the Case OIC
                    //    docAppDb.UpdateCaseOic(hleNumber.Key, hleNumber.Value);
                    //}

                    //uniqueRefNoCnt++;
                }
            }

            // Insert the HLE household structure data
            HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
            if (hlePersonalList.Count > 0)
            {
                foreach (HlePersonal hlePersonal in hlePersonalList)
                {
                    try
                    {
                        bool success = false;
                        if (hleInterfaceDb.DoesPersonalExistsByRefNoNric(hlePersonal.HleNumber, hlePersonal.Nric))
                            success = hleInterfaceDb.Update(hlePersonal);
                        else if (hleInterfaceDb.DoesPersonalExistsByRefNoName(hlePersonal.HleNumber, hlePersonal.Name))
                            success = hleInterfaceDb.Update(hlePersonal);
                        else
                            success = hleInterfaceDb.Insert(hlePersonal) > 0;

                        if (success)
                            successCnt++;
                        else
                            failedCnt++;
                    }
                    catch (Exception)
                    {
                        //Response.Write(ex.Message);
                        failedCnt++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the personal data
    /// </summary>
    /// <param name="startColumnIndex"></param>
    /// <param name="endColumnIndex"></param>
    /// <param name="dataItems"></param>
    /// <param name="hleNumber"></param>
    /// <param name="hleDate"></param>
    /// <param name="hleStatus"></param>
    /// <param name="hhIncome"></param>
    /// <param name="caIncome"></param>
    /// <param name="peOic"></param>
    /// <param name="caOic"></param>
    /// <param name="hlePersonalList"></param>
    /// <param name="personalType"></param>
    private void GetCosPersonalData(string[] dataItems, string hleNumber, string hleDate, string hleStatus,
        string hhIncome, string caIncome, string peOic, string caOic, string rejDate, string canDate, string aprDate, string expDate,
        ref ArrayList hlePersonalList, PersonalTypeEnum personalType)
    {
        if (personalType == PersonalTypeEnum.HA)
        {
            int startIndex = 7;

            #region HA's Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 4; cnt++)
            {
                string[] info = new string[37];
                
                info[0] = dataItems[startIndex++].Trim().ToUpper();
                info[1] = dataItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(info[0]) && !String.IsNullOrEmpty(info[1]))
                {
                    if (Validation.IsNric(info[0]) || Validation.IsFin(info[0]))
                    {
                        #region Get the HA info
                        info[2] = dataItems[startIndex++].Trim().ToUpper();
                        info[3] = EnumManager.MapMaritalStatus(info[2]);
                        info[4] = dataItems[startIndex++].Trim().ToUpper();
                        info[5] = dataItems[startIndex++].Trim().ToUpper();
                        info[6] = EnumManager.MapRelationshipStatus(info[5]);
                        info[7] = dataItems[startIndex++].Trim().ToUpper();
                        info[8] = EnumManager.MapEmploymentStatus(info[7]);
                        info[9] = dataItems[startIndex++].Trim().ToUpper();
                        info[10] = dataItems[startIndex++].Trim().ToUpper();
                        info[11] = dataItems[startIndex++].Trim().ToUpper();
                        info[12] = dataItems[startIndex++].Trim().ToUpper();
                        info[13] = dataItems[startIndex++].Trim().ToUpper();
                        info[14] = dataItems[startIndex++].Trim().ToUpper();
                        info[15] = dataItems[startIndex++].Trim().ToUpper();
                        info[16] = dataItems[startIndex++].Trim().ToUpper();
                        info[17] = dataItems[startIndex++].Trim().ToUpper();
                        info[18] = dataItems[startIndex++].Trim().ToUpper();
                        info[19] = dataItems[startIndex++].Trim().ToUpper();
                        info[20] = dataItems[startIndex++].Trim().ToUpper();
                        info[21] = dataItems[startIndex++].Trim().ToUpper();
                        info[22] = dataItems[startIndex++].Trim().ToUpper();
                        info[23] = dataItems[startIndex++].Trim().ToUpper();
                        info[24] = dataItems[startIndex++].Trim().ToUpper();
                        info[25] = dataItems[startIndex++].Trim().ToUpper();
                        info[26] = dataItems[startIndex++].Trim().ToUpper();
                        info[27] = dataItems[startIndex++].Trim().ToUpper();
                        info[28] = dataItems[startIndex++].Trim().ToUpper();
                        info[29] = dataItems[startIndex++].Trim().ToUpper();
                        info[30] = dataItems[startIndex++].Trim().ToUpper();
                        info[31] = dataItems[startIndex++].Trim().ToUpper();
                        info[32] = dataItems[startIndex++].Trim().ToUpper();
                        info[33] = dataItems[startIndex++].Trim().ToUpper();
                        info[34] = dataItems[startIndex++].Trim().ToUpper();
                        info[35] = dataItems[startIndex++].Trim().ToUpper();
                        info[36] = dataItems[startIndex++].Trim().ToUpper();
                        #endregion

                        //if (!String.IsNullOrEmpty(info[0]))
                        //{
                            // Add the HLE personal
                            SaveCosPersonalRecords(hleNumber, hleDate, hleStatus, hhIncome, caIncome, peOic, caOic, rejDate, canDate, aprDate, expDate, info,
                                ref hlePersonalList, personalType.ToString(), personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
        else if (personalType == PersonalTypeEnum.OC)
        {
            int startIndex = 143;

            #region OC's Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 6; cnt++)
            {
                string[] info = new string[37];
                
                info[0] = dataItems[startIndex++].Trim().ToUpper();
                info[1] = dataItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(info[0]) && !String.IsNullOrEmpty(info[1]))
                {
                    if (Validation.IsNric(info[0]) || Validation.IsFin(info[0]))
                    {
                        #region Get the OC info
                        info[2] = dataItems[startIndex++].Trim().ToUpper();
                        info[3] = EnumManager.MapMaritalStatus(info[2]);
                        info[4] = dataItems[startIndex++].Trim().ToUpper();
                        info[5] = dataItems[startIndex++].Trim().ToUpper();
                        info[6] = EnumManager.MapRelationshipStatus(info[5]);
                        info[7] = dataItems[startIndex++].Trim().ToUpper();
                        info[8] = EnumManager.MapEmploymentStatus(info[7]);
                        info[9] = dataItems[startIndex++].Trim().ToUpper();
                        info[10] = dataItems[startIndex++].Trim().ToUpper();
                        info[11] = dataItems[startIndex++].Trim().ToUpper();
                        info[12] = dataItems[startIndex++].Trim().ToUpper();
                        info[13] = dataItems[startIndex++].Trim().ToUpper();
                        info[14] = dataItems[startIndex++].Trim().ToUpper();
                        info[15] = dataItems[startIndex++].Trim().ToUpper();
                        info[16] = dataItems[startIndex++].Trim().ToUpper();
                        info[17] = dataItems[startIndex++].Trim().ToUpper();
                        info[18] = dataItems[startIndex++].Trim().ToUpper();
                        info[19] = dataItems[startIndex++].Trim().ToUpper();
                        info[20] = dataItems[startIndex++].Trim().ToUpper();
                        info[21] = dataItems[startIndex++].Trim().ToUpper();
                        info[22] = dataItems[startIndex++].Trim().ToUpper();
                        info[23] = dataItems[startIndex++].Trim().ToUpper();
                        info[24] = dataItems[startIndex++].Trim().ToUpper();
                        info[25] = dataItems[startIndex++].Trim().ToUpper();
                        info[26] = dataItems[startIndex++].Trim().ToUpper();
                        info[27] = dataItems[startIndex++].Trim().ToUpper();
                        info[28] = dataItems[startIndex++].Trim().ToUpper();
                        info[29] = dataItems[startIndex++].Trim().ToUpper();
                        info[30] = dataItems[startIndex++].Trim().ToUpper();
                        info[31] = dataItems[startIndex++].Trim().ToUpper();
                        info[32] = dataItems[startIndex++].Trim().ToUpper();
                        info[33] = dataItems[startIndex++].Trim().ToUpper();
                        info[34] = dataItems[startIndex++].Trim().ToUpper();
                        info[35] = dataItems[startIndex++].Trim().ToUpper();
                        info[36] = dataItems[startIndex++].Trim().ToUpper();
                        #endregion

                        //if (!String.IsNullOrEmpty(info[0]))
                        //{
                            // Add the HLE personal
                            SaveCosPersonalRecords(hleNumber, hleDate, hleStatus, hhIncome, caIncome, peOic, caOic, rejDate, canDate, aprDate, expDate, info,
                                ref hlePersonalList, personalType.ToString(), personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Create Personal records
    /// </summary>
    /// <param name="hleNo"></param>
    /// <param name="hleDate"></param>
    /// <param name="hleStatus"></param>
    /// <param name="hhInc"></param>
    /// <param name="caInc"></param>
    /// <param name="peOic"></param>
    /// <param name="caOic"></param>
    /// <param name="dataItems"></param>
    /// <param name="hlePersonalList"></param>
    /// <param name="applicantType"></param>
    private void SaveCosPersonalRecords(string hleNo, string hleDate, string hleStatus, string hhInc,
        string caInc, string peOic, string caOic, string rejDate, string canDate, string aprDate, string expDate,
        string[] dataItems, ref ArrayList hlePersonalList, string applicantType, int orderNo)
    {        
        // Create the HLE Personal records of HA1
        HlePersonal hlePersonal = new HlePersonal();

        hlePersonal.AddPersonalInfo(hleNo, hleDate, hleStatus, hhInc, caInc, peOic, caOic, rejDate, canDate, aprDate, expDate, dataItems, applicantType, orderNo);

        hlePersonalList.Add(hlePersonal);
    }
    #endregion    

    #region RESALE
    /// <summary>
    /// Import the ROS household interface file
    /// </summary>
    private void ImportRosHouseholdInterfaceFile(out int successCnt, out int failedCnt, out string error, out int uniqueRefCnt)
    {
        uniqueRefCnt = 0;
        successCnt = 0;
        failedCnt = 0;
        error = string.Empty;

        if (RosRadUpload.UploadedFiles.Count > 0)
        {
            UploadedFile uploadedFile = RosRadUpload.UploadedFiles[0];
            StreamReader stream = new StreamReader(uploadedFile.InputStream);

            string s;
            ArrayList rosPersonalList = new ArrayList();
            Dictionary<string, resaleOIC> caseNumberList = new Dictionary<string, resaleOIC>();

            while ((s = stream.ReadLine()) != null)
            {
                string[] dataItems = s.Split(new string[] { "*" }, StringSplitOptions.None);

                if (dataItems.Length != 110)
                {
                    error = "Please upload a correct RESALE interface file.";
                    return;
                }

                // Get the ROS data                    
                string caseNo = dataItems[0].Trim().ToUpper();

                if (!String.IsNullOrEmpty(caseNo) && Validation.IsResaleNumber(caseNo))
                {
                    string roic = dataItems[26].Trim().ToUpper();
                    string oic = dataItems[27].Trim().ToUpper();
                    string ahgGrant = dataItems[28].Trim().ToUpper();
                    string hhInc2Yr = dataItems[29].Trim();
                    string numSchAccnt = dataItems[30].Trim();
                    string oldLesseeCode = dataItems[31].Trim();
                    string newLesseeCode = dataItems[32].Trim();
                    string allocBuyer = dataItems[33].Trim();
                    string eligBuyer = dataItems[34].Trim();
                    bool bankLoan = (dataItems[35].Trim().Equals("Y") ? true : false);
                    bool ABCDELoan = (dataItems[36].Trim().Equals("Y") ? true : false);
                    string hhInc = dataItems[37].Trim();
                    string caInc = dataItems[38].Trim();

                    #region Get the sellers info
                    // Get the Sellers' info
                    GetRosPersonalData(dataItems, caseNo, roic, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode,
                        allocBuyer, eligBuyer, bankLoan, ABCDELoan, hhInc, caInc, ref rosPersonalList,
                        ResalePersonalTypeEnum.SE, string.Empty);
                    #endregion

                    #region Get the miscellaneous info
                    // Get the miscellaneous info
                    GetRosPersonalData(dataItems, caseNo, roic, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode,
                        allocBuyer, eligBuyer, bankLoan, ABCDELoan, hhInc, caInc, ref rosPersonalList,
                        ResalePersonalTypeEnum.MISC, "SS");
                    #endregion

                    #region Get the Buyers' info
                    // Get the Buyers' Info
                    GetRosPersonalData(dataItems, caseNo, roic, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode,
                        allocBuyer, eligBuyer, bankLoan, ABCDELoan, hhInc, caInc, ref rosPersonalList,
                        ResalePersonalTypeEnum.BU, string.Empty);
                    #endregion

                    #region Get the Occupiers' info
                    // Get the Occupiers' Info
                    GetRosPersonalData(dataItems, caseNo, roic, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode,
                        allocBuyer, eligBuyer, bankLoan, ABCDELoan, hhInc, caInc, ref rosPersonalList,
                        ResalePersonalTypeEnum.OC, string.Empty);
                    #endregion

                    // Create the unique list of case numbers
                    if (!caseNumberList.ContainsKey(caseNo))
                    {
                        resaleOIC resaleOIC = new Maintenance_ImportInterface_Default.resaleOIC();
                        resaleOIC.oic = oic;
                        resaleOIC.roic = roic;
                        caseNumberList.Add(caseNo, resaleOIC);
                    }
                }
            }

            // Insert the unique case numbers
            if (caseNumberList.Count > 0)
            {
                DocAppDb docAppDb = new DocAppDb();

                foreach (KeyValuePair<string, resaleOIC> caseNumber in caseNumberList)
                {
                    //if (!docAppDb.DoesRefNoExists(caseNumber.Key))
                    //{
                    //    docAppDb.Insert(caseNumber.Key, Util.GetReferenceType(caseNumber.Key), null, null,
                    //        AppStatusEnum.Pending_Documents, null, caseNumber.Value.oic, caseNumber.Value.roic, null, false);
                    //}
                    //else
                    //{
                    //    // Update the Case OIC
                    //    docAppDb.UpdateOic(caseNumber.Key, caseNumber.Value.oic, caseNumber.Value.roic, null, false);
                    //}

                    uniqueRefCnt++;
                }
            }

            // Insert the ROS household structure data            
            ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
            if (rosPersonalList.Count > 0)
            {
                foreach (ResalePersonal rosPersonal in rosPersonalList)
                {
                    //try
                    //{
                    //    bool success = false;
                    //    if (resaleInterfaceDb.DoesPersonalExistsByCaseNoNric(rosPersonal.CaseNo, rosPersonal.Nric))
                    //        success = resaleInterfaceDb.Update(rosPersonal);
                    //    else if (resaleInterfaceDb.DoesPersonalExistsByCaseNoName(rosPersonal.CaseNo, rosPersonal.Name))
                    //        success = resaleInterfaceDb.Update(rosPersonal);
                    //    else
                    //        success = resaleInterfaceDb.Insert(rosPersonal) > 0;

                    //    if (success)
                    //        successCnt++;
                    //    else
                    //        failedCnt++;
                    //}
                    //catch (Exception)
                    //{
                    //    failedCnt++;
                    //}
                }
            }
        }
    }

    public struct resaleOIC
    {
        public string roic;
        public string oic;
    }

    /// <summary>
    /// Get the Resale personal data
    /// </summary>
    /// <param name="dateItems"></param>
    /// <param name="caseNo"></param>
    /// <param name="roIc"></param>
    /// <param name="oic"></param>
    /// <param name="ahgGrant"></param>
    /// <param name="hhInc2Yr"></param>
    /// <param name="numSchAccnt"></param>
    /// <param name="oldLesseeCode"></param>
    /// <param name="newLesseeCode"></param>
    /// <param name="allocBuyer"></param>
    /// <param name="eligBuyer"></param>
    /// <param name="bankLoan"></param>
    /// <param name="ABCDELoan"></param>
    /// <param name="hhInc"></param>
    /// <param name="caInc"></param>
    /// <param name="rosPersonalList"></param>
    /// <param name="personalType"></param>
    /// <param name="relationship"></param>
    private void GetRosPersonalData(string[] dateItems, string caseNo, string roIc, string oic, string ahgGrant, string hhInc2Yr, string numSchAccnt,
        string oldLesseeCode, string newLesseeCode, string allocBuyer, string eligBuyer, bool bankLoan, bool ABCDELoan,
        string hhInc, string caInc, ref ArrayList rosPersonalList, ResalePersonalTypeEnum personalType, string relationshipCode)
    {
        string nric = string.Empty;
        string name = string.Empty;
        string maritalStatusCode = string.Empty;
        string maritalStatus = string.Empty;
        string dateOfBirth = string.Empty;
        string relationship = string.Empty;

        if (personalType == ResalePersonalTypeEnum.SE)
        {
            #region Sellers' Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 4; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;

                // Get the NRIC and Name of the seller
                if (cnt == 1)
                {
                    nric = dateItems[1].Trim().ToUpper();
                    name = dateItems[5].Trim().ToUpper();
                }
                else if (cnt == 2)
                {
                    nric = dateItems[2].Trim().ToUpper();
                    name = dateItems[6].Trim().ToUpper();
                }
                else if (cnt == 3)
                {
                    nric = dateItems[3].Trim().ToUpper();
                    name = dateItems[7].Trim().ToUpper();
                }
                else if (cnt == 4)
                {
                    nric = dateItems[4].Trim().ToUpper();
                    name = dateItems[8].Trim().ToUpper();
                }

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        //if (!String.IsNullOrEmpty(nric))
                        //{
                            // Add the resale personal
                            SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, relationshipCode,
                                relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                                bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
        else if (personalType == ResalePersonalTypeEnum.BU)
        {
            int startIndex = 39;

            #region Buyers' Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 4; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;
                maritalStatusCode = string.Empty;
                maritalStatus = string.Empty;
                dateOfBirth = string.Empty;
                relationshipCode = string.Empty;
                relationship = string.Empty;

                /// Get the occupier info
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        maritalStatusCode = dateItems[startIndex++].Trim().ToUpper();
                        maritalStatus = EnumManager.MapMaritalStatus(maritalStatusCode);
                        dateOfBirth = dateItems[startIndex++].Trim().ToUpper();
                        relationshipCode = dateItems[startIndex++].Trim().ToUpper();
                        relationship = EnumManager.MapResaleRelationshipStatus(relationshipCode);

                        //if (!String.IsNullOrEmpty(nric))
                        //{
                            // Add the resale personal
                            SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, relationshipCode,
                                relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                                bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
        else if (personalType == ResalePersonalTypeEnum.OC)
        {
            int startIndex = 59;

            #region Occupiers' Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 10; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;
                maritalStatusCode = string.Empty;
                maritalStatus = string.Empty;
                dateOfBirth = string.Empty;
                relationshipCode = string.Empty;
                relationship = string.Empty;

                // Get the occupier info
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        maritalStatusCode = dateItems[startIndex++].Trim().ToUpper();
                        maritalStatus = EnumManager.MapMaritalStatus(maritalStatusCode);
                        dateOfBirth = dateItems[startIndex++].Trim().ToUpper();
                        relationshipCode = dateItems[startIndex++].Trim().ToUpper();
                        relationship = EnumManager.MapResaleRelationshipStatus(relationshipCode);

                        //if (!String.IsNullOrEmpty(nric))
                        //{
                        // Add the resale personal
                        SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, relationshipCode,
                            relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                            bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
        else if (personalType == ResalePersonalTypeEnum.MISC)
        {
            int personalCnt = 1;

            int startIndex = 9;
            relationship = EnumManager.MapResaleRelationshipStatus("SS");

            #region Sellers' Spouse Info
            for (int cnt = 1; cnt <= 4; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;

                // Get the NRIC and Name of the sellers' spouse
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        //if (!String.IsNullOrEmpty(nric))
                        //{
                        // Add the resale personal
                        SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, "SS",
                            relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                            bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion

            startIndex = 18;
            relationship = EnumManager.MapResaleRelationshipStatus("PR");

            #region Parents' Info
            for (int cnt = 1; cnt <= 2; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;

                // Get the NRIC and Name of the parent
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        //if (!String.IsNullOrEmpty(nric))
                        //{
                        // Add the resale personal
                        SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, "PR",
                            relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                            bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion

            startIndex = 22;
            relationship = EnumManager.MapResaleRelationshipStatus("CH");

            #region Children Info
            for (int cnt = 1; cnt <= 2; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;

                // Get the NRIC and Name of the child
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {
                        //if (!String.IsNullOrEmpty(nric))
                        //{
                        // Add the resale personal
                        SaveResalePersonalRecords(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, "CH",
                            relationship, roIc, oic, ahgGrant, hhInc2Yr, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
                            bankLoan, ABCDELoan, hhInc, caInc, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Save the ROS personal data
    /// </summary>
    /// <param name="caseNo"></param>
    /// <param name="nric"></param>
    /// <param name="name"></param>
    /// <param name="maritalStatusCode"></param>
    /// <param name="maritalStatus"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="relationshipCode"></param>
    /// <param name="relationship"></param>
    /// <param name="roic"></param>
    /// <param name="oic"></param>
    /// <param name="ahgGrant"></param>
    /// <param name="hhInc2Year"></param>
    /// <param name="numSchAccnt"></param>
    /// <param name="oldLesseeCode"></param>
    /// <param name="newLesseeCode"></param>
    /// <param name="allocBuyer"></param>
    /// <param name="eligBuyer"></param>
    /// <param name="bankLoan"></param>
    /// <param name="ABCDELoan"></param>
    /// <param name="hhInc"></param>
    /// <param name="caInc"></param>
    /// <param name="applicantType"></param>
    /// <param name="resalePersonalList"></param>
    private void SaveResalePersonalRecords(string caseNo, string nric, string name, string maritalStatusCode, string maritalStatus,
        string dateOfBirth, string relationshipCode, string relationship, string roic, string oic, string ahgGrant, string hhInc2Year,
        string numSchAccnt, string oldLesseeCode, string newLesseeCode, string allocBuyer, string eligBuyer, bool bankLoan, bool ABCDELoan,
        string hhInc, string caInc, string applicantType, ref ArrayList resalePersonalList, int orderNo)
    {
        //// Create the Personal record
        //ResalePersonal resalePersonal = new ResalePersonal();

        //resalePersonal.AddPersonalInfo(caseNo, nric, name, maritalStatusCode, maritalStatus, dateOfBirth, relationshipCode,
        //    relationship, roic, oic, ahgGrant, hhInc2Year, numSchAccnt, oldLesseeCode, newLesseeCode, allocBuyer, eligBuyer,
        //    bankLoan, ABCDELoan, hhInc, caInc, applicantType, orderNo);

        //resalePersonalList.Add(resalePersonal);
    }
    #endregion

    #region SERS
    /// <summary>
    /// Import the SERS household interface file
    /// </summary>
    private void ImportSersHouseholdInterfaceFile(out int successCnt, out int failedCnt, out string error, out int uniqueRefCnt)
    {
        uniqueRefCnt = 0;
        successCnt = 0;
        failedCnt = 0;
        error = string.Empty;

        if (SersRadUpload.UploadedFiles.Count > 0)
        {
            UploadedFile uploadedFile = SersRadUpload.UploadedFiles[0];
            StreamReader stream = new StreamReader(uploadedFile.InputStream);

            string s;
            ArrayList sersPersonalList = new ArrayList();
            List<string> caseNumberList = new List<string>();

            while ((s = stream.ReadLine()) != null)
            {
                string[] dataItems = s.Split(new string[] { "*" }, StringSplitOptions.None);

                if (dataItems.Length != 108)
                {
                    error = "Please upload a correct SERS interface file.";
                    return;
                }

                // Get the SERS data                    
                string schAcc = dataItems[0].Trim().ToUpper();

                if (!String.IsNullOrEmpty(schAcc) && Validation.IsSersNumber(schAcc))
                {
                    string stage = dataItems[1].Trim().ToUpper();
                    string block = dataItems[2].Trim().ToUpper();
                    string street = dataItems[3].Trim().ToUpper();
                    string level = dataItems[4].Trim();
                    string unit = dataItems[5].Trim();
                    string postal = dataItems[6].Trim();
                    string alloc = dataItems[7].Trim();
                    string elig = dataItems[8].Trim();

                    #region Get the HA info
                    // Get the HA' info
                    GetSersPersonalData(dataItems, schAcc, stage, block, street, level, unit, postal, alloc, elig, ref sersPersonalList,
                        PersonalTypeEnum.HA);
                    #endregion

                    #region Get the OC info
                    // Get the OC' Spouse info
                    GetSersPersonalData(dataItems, schAcc, stage, block, street, level, unit, postal, alloc, elig, ref sersPersonalList,
                        PersonalTypeEnum.OC);
                    #endregion

                    // Create the unique list of case numbers
                    if (!caseNumberList.Contains(schAcc))
                    {
                        caseNumberList.Add(schAcc);
                    }
                }
            }

            // Insert the unique case numbers
            if (caseNumberList.Count > 0)
            {
                DocAppDb docAppDb = new DocAppDb();

                foreach (string caseNumber in caseNumberList)
                {
                    //if (!docAppDb.DoesRefNoExists(caseNumber))
                    //{
                    //    docAppDb.Insert(caseNumber, Util.GetReferenceType(caseNumber), null, null,
                    //        AppStatusEnum.Pending_Documents, null, string.Empty, string.Empty, null, false);
                    //}

                    //uniqueRefCnt++;
                }
            }

            // Insert the SERS household structure data            
            SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
            if (sersPersonalList.Count > 0)
            {
                foreach (SersPersonal sersPersonal in sersPersonalList)
                {
                    try
                    {
                        bool success = false;

                        if (sersInterfaceDb.DoesPersonalExistsBySchAccNric(sersPersonal.SchAcc, sersPersonal.Nric))
                            success = sersInterfaceDb.Update(sersPersonal);
                        else if (sersInterfaceDb.DoesPersonalExistsBySchAccName(sersPersonal.SchAcc, sersPersonal.Name))
                            success = sersInterfaceDb.Update(sersPersonal);
                        else
                            success = sersInterfaceDb.Insert(sersPersonal) > 0;

                        if (success)
                            successCnt++;
                        else
                            failedCnt++;
                    }
                    catch (Exception)
                    {
                        failedCnt++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the SERS personal data
    /// </summary>
    /// <param name="dateItems"></param>
    /// <param name="schAcc"></param>
    /// <param name="stage"></param>
    /// <param name="block"></param>
    /// <param name="street"></param>
    /// <param name="level"></param>
    /// <param name="unit"></param>
    /// <param name="postal"></param>
    /// <param name="alloc"></param>
    /// <param name="elig"></param>
    /// <param name="rosPersonalList"></param>
    /// <param name="personalType"></param>
    /// <param name="relationshipCode"></param>
    private void GetSersPersonalData(string[] dateItems, string schAcc, string stage, string block, string street, string level, string unit,
        string postal, string alloc, string elig, ref ArrayList rosPersonalList, PersonalTypeEnum personalType)
    {
        string nric = string.Empty;
        string name = string.Empty;
        string maritalStatusCode = string.Empty;
        string maritalStatus = string.Empty;
        string dateOfBirth = string.Empty;
        string citizenshipCode = string.Empty;
        string citizenship = string.Empty;
        string relationshipCode = string.Empty;
        string relationship = string.Empty;
        bool privatePropertyOwner = false;

        if (personalType == PersonalTypeEnum.HA)
        {
            int startIndex = 9;

            #region HA' Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 4; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;
                maritalStatusCode = string.Empty;
                maritalStatus = string.Empty;
                dateOfBirth = string.Empty;
                relationshipCode = string.Empty;
                relationship = string.Empty;

                /// Get the occupier info
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {

                        maritalStatusCode = dateItems[startIndex++].Trim().ToUpper();
                        maritalStatus = EnumManager.MapSersMaritalStatus(maritalStatusCode);
                        dateOfBirth = dateItems[startIndex++].Trim().ToUpper();
                        citizenshipCode = dateItems[startIndex++].Trim().ToUpper();
                        citizenship = EnumManager.MapCitizenship(citizenshipCode);
                        relationshipCode = dateItems[startIndex++].Trim().ToUpper();
                        relationship = EnumManager.MapRelationshipStatus(relationshipCode);
                        privatePropertyOwner = (dateItems[startIndex++].Trim().ToUpper().Equals("Y") ? true : false);

                        //if (!String.IsNullOrEmpty(nric))
                        //{
                            // Add the resale personal
                            SaveSersPersonalRecords(schAcc, stage, block, street, level, unit, postal, alloc, elig, nric, name,
                                maritalStatusCode, maritalStatus, dateOfBirth, citizenshipCode, citizenship, relationshipCode, relationship,
                                privatePropertyOwner, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
        else if (personalType == PersonalTypeEnum.OC)
        {
            int startIndex = 37;

            #region OC' Info
            int personalCnt = 1;
            for (int cnt = 1; cnt <= 10; cnt++)
            {
                nric = string.Empty;
                name = string.Empty;
                maritalStatusCode = string.Empty;
                maritalStatus = string.Empty;
                dateOfBirth = string.Empty;
                relationshipCode = string.Empty;
                relationship = string.Empty;

                /// Get the occupier info
                nric = dateItems[startIndex++].Trim().ToUpper();
                name = dateItems[startIndex++].Trim().ToUpper();

                if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
                {
                    if (Validation.IsNric(nric) || Validation.IsFin(nric))
                    {

                        maritalStatusCode = dateItems[startIndex++].Trim().ToUpper();
                        maritalStatus = EnumManager.MapSersMaritalStatus(maritalStatusCode);
                        dateOfBirth = dateItems[startIndex++].Trim().ToUpper();
                        citizenshipCode = dateItems[startIndex++].Trim().ToUpper();
                        citizenship = EnumManager.MapCitizenship(citizenshipCode);
                        relationshipCode = dateItems[startIndex++].Trim().ToUpper();
                        relationship = EnumManager.MapRelationshipStatus(relationshipCode);
                        privatePropertyOwner = (dateItems[startIndex++].Trim().ToUpper().Equals("Y") ? true : false);

                        //if (!String.IsNullOrEmpty(nric))
                        //{
                            // Add the resale personal
                            SaveSersPersonalRecords(schAcc, stage, block, street, level, unit, postal, alloc, elig, nric, name,
                                maritalStatusCode, maritalStatus, dateOfBirth, citizenshipCode, citizenship, relationshipCode, relationship,
                                privatePropertyOwner, personalType.ToString(), ref rosPersonalList, personalCnt++);
                        //}
                    }
                }
            }
            #endregion
        }
    }

    /// <summary>
    /// Save the SERS personal data
    /// </summary>
    /// <param name="schAcc"></param>
    /// <param name="stage"></param>
    /// <param name="block"></param>
    /// <param name="street"></param>
    /// <param name="level"></param>
    /// <param name="unit"></param>
    /// <param name="postal"></param>
    /// <param name="alloc"></param>
    /// <param name="elig"></param>
    /// <param name="nric"></param>
    /// <param name="name"></param>
    /// <param name="maritalStatusCode"></param>
    /// <param name="maritalStatus"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="citizenshipCode"></param>
    /// <param name="citizenship"></param>
    /// <param name="relationshipCode"></param>
    /// <param name="relationship"></param>
    /// <param name="privatePropertyOwner"></param>
    /// <param name="applicantType"></param>
    /// <param name="sersPersonalList"></param>
    private void SaveSersPersonalRecords(string schAcc, string stage, string block, string street, string level, string unit,
        string postal, string alloc, string elig, string nric, string name, string maritalStatusCode, string maritalStatus,
        string dateOfBirth, string citizenshipCode, string citizenship, string relationshipCode, string relationship,
        bool privatePropertyOwner, string applicantType, ref ArrayList sersPersonalList, int orderNo)
    {
        // Create the Personal record
        SersPersonal sersPersonal = new SersPersonal();

        sersPersonal.AddPersonalInfo(schAcc, stage, block, street, level, unit, postal, alloc, elig, nric, name, maritalStatusCode, maritalStatus,
            dateOfBirth, citizenshipCode, citizenship, relationshipCode, relationship, privatePropertyOwner, applicantType, orderNo);

        sersPersonalList.Add(sersPersonal);
    }
    #endregion

    #region SALES
    /// <summary>
    /// Import the SALES household interface file
    /// </summary>
    private void ImportSalesHouseholdInterfaceFile(out int successCnt, out int failedCnt, out string error, out int uniqueRefCnt)
    {
        uniqueRefCnt = 0;
        successCnt = 0;
        failedCnt = 0;
        error = string.Empty;

        if (SalesRadUpload.UploadedFiles.Count > 0)
        {
            UploadedFile uploadedFile = SalesRadUpload.UploadedFiles[0];
            StreamReader stream = new StreamReader(uploadedFile.InputStream);

            string s;
            ArrayList salesPersonalList = new ArrayList();
            List<string> caseNumberList = new List<string>();

            while ((s = stream.ReadLine()) != null)
            {
                string[] dataItems = s.Split(new string[] { "*" }, StringSplitOptions.None);

                if (dataItems.Length != 250)
                {
                    error = "Please upload a correct SALES interface file.";
                    return;
                }

                // Get the SERS data                    
                string registrationNo = dataItems[0].Trim().ToUpper();

                if (!String.IsNullOrEmpty(registrationNo) && Validation.IsSalesNumber(registrationNo))
                {
                    string applicationDate = dataItems[1].Trim().ToUpper();
                    string householdType = dataItems[2].Trim().ToUpper();
                    string alloc = dataItems[3].Trim().ToUpper();
                    string ellig = dataItems[4].Trim();
                    string hhInc = dataItems[5].Trim();
                    string ahgStatus = dataItems[6].Trim();

                    #region Get the HA info
                    // Get the HA' info
                    GetSalesPersonalData(dataItems, registrationNo, applicationDate, householdType, alloc, ellig, hhInc, ahgStatus,
                        ref salesPersonalList, PersonalTypeEnum.HA);
                    #endregion

                    #region Get the OC info
                    // Get the OC' Spouse info
                    GetSalesPersonalData(dataItems, registrationNo, applicationDate, householdType, alloc, ellig, hhInc, ahgStatus,
                        ref salesPersonalList, PersonalTypeEnum.OC);
                    #endregion

                    // Create the unique list of case numbers
                    if (!caseNumberList.Contains(registrationNo))
                    {
                        caseNumberList.Add(registrationNo);
                    }
                }
            }

            // Insert the unique case numbers
            if (caseNumberList.Count > 0)
            {
                DocAppDb docAppDb = new DocAppDb();

                foreach (string caseNumber in caseNumberList)
                {
                    //if (!docAppDb.DoesRefNoExists(caseNumber))
                    //{
                    //    docAppDb.Insert(caseNumber, Util.GetReferenceType(caseNumber), null, null,
                    //        AppStatusEnum.Pending_Documents, null, string.Empty, string.Empty, null, false);
                    //}

                    //uniqueRefCnt++;
                }
            }

            // Insert the SALES household structure data            
            SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
            if (salesPersonalList.Count > 0)
            {
                foreach (SalesPersonal salesPersonal in salesPersonalList)
                {
                    //try
                    //{
                    //    bool success = false;

                    //    if (salesInterfaceDb.DoesPersonalExistsByRefNoNric(salesPersonal.RegistrationNo, salesPersonal.Nric))
                    //        success = salesInterfaceDb.Update(salesPersonal);
                    //    else if (salesInterfaceDb.DoesPersonalExistsByRefNoName(salesPersonal.RegistrationNo, salesPersonal.Name))
                    //        success = salesInterfaceDb.Update(salesPersonal);
                    //    else
                    //        success = salesInterfaceDb.Insert(salesPersonal) > 0;

                    //    if (success)
                    //        successCnt++;
                    //    else
                    //        failedCnt++;
                    //}
                    //catch (Exception)
                    //{
                    //    failedCnt++;
                    //}
                }
            }
        }
    }

    /// <summary>
    /// Get the SALES personal data
    /// </summary>
    /// <param name="dateItems"></param>
    /// <param name="registrationNo"></param>
    /// <param name="applicationDate"></param>
    /// <param name="householdType"></param>
    /// <param name="hhInc"></param>
    /// <param name="ahgStatus"></param>
    /// <param name="unit"></param>
    /// <param name="postal"></param>
    /// <param name="alloc"></param>
    /// <param name="elig"></param>
    /// <param name="salesPersonalList"></param>
    /// <param name="personalType"></param>
    /// <param name="relationshipCode"></param>
    private void GetSalesPersonalData(string[] dateItems, string registrationNo, string applicationDate, string householdType, string alloc, string elig, 
        string hhInc, string ahgStatus, ref ArrayList salesPersonalList, PersonalTypeEnum personalType)
    {
        string nric = string.Empty;
        string name = string.Empty;
        string maritalStatusCode = string.Empty;
        string maritalStatus = string.Empty;
        string dateOfBirth = string.Empty;
        string citizenshipCode = string.Empty;
        string citizenship = string.Empty;
        string relationshipCode = string.Empty;
        string relationship = string.Empty;
        string income = string.Empty;

        int startIndex = 0;
        int counter = 0;

        if (personalType == PersonalTypeEnum.HA)
        {
            startIndex = 7;
            counter = 4;
        }
        else if (personalType == PersonalTypeEnum.OC)
        {
            startIndex = 35;
            counter = 6;
        }

        #region Get Data
        int personalCnt = 1;
        for (int cnt = 1; cnt <= counter; cnt++)
        {
            nric = string.Empty;
            name = string.Empty;
            maritalStatusCode = string.Empty;
            maritalStatus = string.Empty;
            dateOfBirth = string.Empty;
            relationshipCode = string.Empty;
            relationship = string.Empty;
            citizenshipCode = string.Empty;
            citizenship = string.Empty;
            income = string.Empty;

            // Get the applicant info
            nric = dateItems[startIndex++].Trim().ToUpper();
            name = dateItems[startIndex++].Trim().ToUpper();

            if (!String.IsNullOrEmpty(nric) && !String.IsNullOrEmpty(name))
            {
                if (Validation.IsNric(nric) || Validation.IsFin(nric))
                {
                    maritalStatusCode = dateItems[startIndex++].Trim().ToUpper();
                    maritalStatus = EnumManager.MapMaritalStatus(maritalStatusCode);
                    dateOfBirth = dateItems[startIndex++].Trim().ToUpper();
                    relationshipCode = dateItems[startIndex++].Trim().ToUpper();
                    relationship = EnumManager.MapRelationshipStatus(relationshipCode);
                    citizenshipCode = dateItems[startIndex++].Trim().ToUpper();
                    citizenship = EnumManager.MapCitizenship(citizenshipCode);
                    income = dateItems[startIndex++].Trim().ToUpper();

                    //if (!String.IsNullOrEmpty(nric))
                    //{
                        // Add the resale personal
                        SaveSalesPersonalRecords(registrationNo, applicationDate, householdType, alloc, elig, hhInc, ahgStatus, nric, name,
                            maritalStatusCode, maritalStatus, dateOfBirth, relationshipCode, relationship, citizenshipCode, citizenship,
                            income, personalType.ToString(), ref salesPersonalList, personalCnt++);
                    //}
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Save the SALES personal data
    /// </summary>
    /// <param name="registrationNo"></param>
    /// <param name="applicationDate"></param>
    /// <param name="householdType"></param>
    /// <param name="alloc"></param>
    /// <param name="elig"></param>
    /// <param name="nric"></param>
    /// <param name="name"></param>
    /// <param name="maritalStatusCode"></param>
    /// <param name="maritalStatus"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="citizenshipCode"></param>
    /// <param name="citizenship"></param>
    /// <param name="relationshipCode"></param>
    /// <param name="relationship"></param>
    /// <param name="applicantType"></param>
    /// <param name="sersPersonalList"></param>
    private void SaveSalesPersonalRecords(string registrationNo, string applicationDate, string householdType, string alloc, string elig, 
        string hhInc, string ahgStatus, string nric, string name, string maritalStatusCode, string maritalStatus,
        string dateOfBirth, string relationshipCode, string relationship, string citizenshipCode, string citizenship, 
        string income, string applicantType, ref ArrayList salesPersonalList, int orderNo)
    {
        //// Create the Personal record
        //SalesPersonal salesPersonal = new SalesPersonal();

        //salesPersonal.AddPersonalInfo(registrationNo, applicationDate, householdType, alloc, elig, hhInc, ahgStatus,
        //    nric, name, maritalStatusCode, maritalStatus, dateOfBirth, citizenshipCode, citizenship, 
        //    relationshipCode, relationship, income, applicantType, orderNo);

        //salesPersonalList.Add(salesPersonal);
    }
    #endregion

    #endregion
}
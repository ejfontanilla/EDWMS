using System;
using System.Data;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;
using System.Text;

public partial class Import_Control_ScanUploadFields : System.Web.UI.UserControl
{
    private string url;
    private int _currentDocAppId = -6;

    #region Private Members
    //public string ReferenceNumber
    //{
    //    get
    //    {
    //        return DocAppRadComboBox.Text.Trim();
    //    }
    //    set
    //    {
    //        DocAppRadComboBox.Text = value;
    //    }
    //}

    public string ReferenceNumber
    {
        get
        {
            if (DocAppRadTextBox.Visible)
                return DocAppRadTextBox.Text.Trim();
            else
                return DocAppRadComboBox.Text.Trim();
            //return DocAppRadTextBox.Text.Trim();
        }
        set
        {
            if (DocAppRadTextBox.Visible)
                DocAppRadTextBox.Text = value;
            else
                DocAppRadComboBox.Text = value;
            //DocAppRadTextBox.Text = value;
        }
    }

    public string ReferenceType
    {
        get
        {
            return ReferenceTypeHiddenField.Value;
        }
        set
        {
            ReferenceTypeHiddenField.Value = value;
        }
    }

    public string ReferenceLabel
    {
        get
        {
            return ReferenceTypeLabel.Text;
        }
        set
        {
            ReferenceTypeLabel.Text = value;
        }
    }

    public string Block
    {
        get
        {
            return BlockTextBox.Text.Trim();
        }
        set
        {
            BlockTextBox.Text = value;
        }
    }

    public int StreetId
    {
        get
        {
            int result = -1;
            Int32.TryParse(StreetNameRadComboBox.SelectedValue, out result);
            return result;
        }
        set
        {
            int result = -1;
            Int32.TryParse(value.ToString(), out result);
            StreetNameRadComboBox.SelectedValue = result.ToString();
        }
    }

    //public int DocAppId
    //{
    //    get
    //    {
    //        int result = -1;
    //        Int32.TryParse(DocAppRadComboBox.SelectedValue, out result);
    //        return result;
    //    }
    //    set
    //    {
    //        int result = -1;
    //        Int32.TryParse(value.ToString(), out result);
    //        DocAppRadComboBox.SelectedValue = result.ToString();
    //    }
    //}

    public int DocAppId
    {
        get
        {
            int result = -1;
            Int32.TryParse(DocAppHiddenValue.Value, out result);
            return result;            
        }
        set
        {
            int result = -1;
            Int32.TryParse(value.ToString(), out result);
            DocAppHiddenValue.Value = result.ToString();
        }
    }

    public string Level
    {
        get
        {
            return LevelTextBox.Text.Trim();
        }
        set
        {
            LevelTextBox.Text = value;
        }
    }

    public string Unit
    {
        get
        {
            return UnitNumberTextBox.Text.Trim();
        }
        set
        {
            UnitNumberTextBox.Text = value;
        }
    }

    public string Channel
    {
        get
        {
            return ChannelDropDownList.SelectedValue;
        }
        set
        {
            ChannelDropDownList.SelectedValue = value;
        }
    }

    public string Remark
    {
        get
        {
            return RemarkTextBox.Text.Trim();
        }
        set
        {
            RemarkTextBox.Text = value;
        }
    }

    public string CurrentDocAppId
    {
        get
        {
            return docAppIdHiddenField.Value;
        }
        set
        {
            docAppIdHiddenField.Value = value;
        }
    }
    
    #endregion

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        url = Request.Url.ToString().ToLower();

        if (!IsPostBack)
        {
            PopulateStreetNames();
            PopulateChannels();

            ////hide the Block, street name and Unit Number for users who belong to operation "SR"
            //SectionDb sectionDb = new SectionDb();

            //if (sectionDb.isUserBusinessCode(SectionBusinessCodeEnum.SR))
            //    AddressPanel.Visible = true;
        }
    }

    //protected void GetRefNoButton_OnClick(object sender, EventArgs e)
    //{
    //    DocAppRadComboBox.Text = string.Empty;
    //    DocAppRadComboBox.Items.Clear();
    //    DocAppRadComboBox.DataSourceID = "GetDocAppByAddressObjectDataSource";

    //    //set the ref no dropdown to the first occurance 
    //    DocAppDb docAppDb = new DocAppDb();
    //    DataTable docApp1 = docAppDb.GetDocAppForDropDownByAddress(BlockTextBox.Text.Trim(), StreetNameRadComboBox.Text.Equals("- Select Street -") ? string.Empty : StreetNameRadComboBox.Text.Trim(), LevelTextBox.Text.Trim(), UnitNumberTextBox.Text.Trim());
    //    //DocAppDb docAppDb = new DocAppDb();

    //    if (docApp1.Rows.Count > 0)
    //    {
    //        foreach (DataRow r in docApp1.Rows)
    //            DocAppRadComboBox.Items.Add(new RadComboBoxItem(r["RefNo"].ToString(),string.Empty));
    //        //DocAppRadComboBox.Text = docApp.Rows[0]["RefNo"].ToString();
    //        //DocAppRadComboBox.it;
    //        DocAppRadComboBox.SelectedIndex = 0;

    //        //2012-09-12
    //        DocApp.DocAppDataTable docApp = null;
    //        if (Validation.IsHLENumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //HLE
    //            docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //        else if (Validation.IsSersNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //SERS
    //            docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //        else if (Validation.IsSalesNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //SALES
    //            docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //        else if (Validation.IsResaleNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //RESALE
    //            docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());

    //        if (docApp != null)
    //        {
    //            if (docApp.Rows.Count > 0)
    //            {
    //                DocApp.DocAppRow row = docApp[0];
    //                ReferenceTypeLabel.Text = row.RefType;
    //                ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

    //                #region 2012-09-06 List the household structure
    //                if (row.RefType.ToUpper().Trim() == "HLE")
    //                {
    //                    //SersInterface
    //                    HleInterfaceDb db = new HleInterfaceDb();
    //                    DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //                    householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                }
    //                else if (row.RefType.ToUpper().Trim() == "SERS")
    //                {
    //                    //SersInterface
    //                    SersInterfaceDb db = new SersInterfaceDb();
    //                    DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //                    householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                }
    //                else if (row.RefType.ToUpper().Trim() == "SALES")
    //                {
    //                    //SalesInterface
    //                    SalesInterfaceDb db = new SalesInterfaceDb();
    //                    DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //                    householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                }
    //                else if (row.RefType.ToUpper().Trim() == "RESALE")
    //                {
    //                    //ResaleInterface
    //                    ResaleInterfaceDb db = new ResaleInterfaceDb();
    //                    DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
    //                    householdStructureLabel.Text = GetHouseholdStructure(dt);
    //                }
    //                else
    //                {
    //                    householdStructureLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
    //                }
    //                #endregion 2012-09-06 List the household structure



    //                ////update the address field based on the address for the docset sorted by submission date desc.
    //                //DocSetDb docSetDb = new DocSetDb();
    //                //DocSet.DocSetDataTable docSet = docSetDb.GetLatestDocSetByDocAppId(int.Parse(DocAppRadComboBox.SelectedValue));

    //                //if (docSet.Rows.Count > 0)
    //                //{
    //                //    DocSet.DocSetRow docSetRow = docSet[0];
    //                //    BlockTextBox.Text = docSetRow.IsBlockNull() ? string.Empty : docSetRow.Block;
    //                //    StreetNameRadComboBox.SelectedValue = docSetRow.IsStreetIdNull() ? "1" : docSetRow.StreetId.ToString();
    //                //    LevelTextBox.Text = docSetRow.IsFloorNull() ? string.Empty : docSetRow.Floor;
    //                //    UnitNumberTextBox.Text = docSetRow.IsUnitNull() ? string.Empty : docSetRow.Unit;
    //                //}

    //                //update the address field based on the address for the sersinterface table.
    //                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
    //                SersInterface.SersInterfaceDataTable sersInterface = sersInterfaceDb.GetSersInterfaceByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());

    //                if (sersInterface.Rows.Count > 0)
    //                {
    //                    SersInterface.SersInterfaceRow sersInterfaceRow = sersInterface[0];
    //                    BlockTextBox.Text = sersInterfaceRow.IsBlockNull() ? string.Empty : sersInterfaceRow.Block;
    //                    if (sersInterfaceRow.IsStreetNull())
    //                        StreetNameRadComboBox.SelectedIndex = 0;
    //                    else
    //                    {
    //                        StreetDb streetDb = new StreetDb();
    //                        Street.StreetDataTable streets = streetDb.GetStreetByName(sersInterfaceRow.Street.Trim());

    //                        if (streets.Rows.Count > 0)
    //                        {
    //                            Street.StreetRow streetRow = streets[0];
    //                            StreetNameRadComboBox.SelectedValue = streetRow.Id.ToString();
    //                        }
    //                        else
    //                            StreetNameRadComboBox.SelectedIndex = 0;

    //                    }
    //                    LevelTextBox.Text = sersInterfaceRow.IsLevelNull() ? string.Empty : sersInterfaceRow.Level;
    //                    UnitNumberTextBox.Text = sersInterfaceRow.IsUnitNull() ? string.Empty : sersInterfaceRow.Unit;
    //                }
    //            }
    //            else
    //            {
    //                ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
    //                ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
    //                householdStructureLabel.Text = "N.A";
    //            }
    //        }
    //        else
    //        {
    //            ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
    //            ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
    //            householdStructureLabel.Text = "N.A";
    //        }


    //        ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
    //        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

    //        CheckRefNoChanged();
    //        CheckRefNoExistence();

    //        if (Request.Url.ToString().ToLower().Contains("/import/scandocuments/"))
    //            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeTwainScript", "InitializeTwain();", true);
    //    }
    //}

    protected void GetRefNoButton_OnClick(object sender, EventArgs e)
    {
        DocAppRadComboBox.Visible = true;
        DocAppRadComboBox.Text = string.Empty;
        DocAppRadComboBox.Items.Clear();
        DocAppRadComboBox.DataSourceID = "GetDocAppByAddressObjectDataSource";

        DocAppRadTextBox.Text = string.Empty;
        DocAppHiddenValue.Value = string.Empty;
        DocAppRadTextBox.Visible = false;

        //set the ref no dropdown to the first occurance 
        DocAppDb docAppDb = new DocAppDb();
        DataTable docApp1 = docAppDb.GetDocAppForDropDownByAddress(BlockTextBox.Text.Trim(), StreetNameRadComboBox.Text.Equals("- Select Street -") ? string.Empty : StreetNameRadComboBox.Text.Trim(), LevelTextBox.Text.Trim(), UnitNumberTextBox.Text.Trim());
        //DocAppDb docAppDb = new DocAppDb();

        if (docApp1.Rows.Count > 0)
        {
            foreach (DataRow r in docApp1.Rows)
                DocAppRadComboBox.Items.Add(new RadComboBoxItem(r["RefNo"].ToString(), string.Empty));
            //DocAppRadComboBox.Text = docApp.Rows[0]["RefNo"].ToString();
            //DocAppRadComboBox.it;
            DocAppRadComboBox.SelectedIndex = 0;

            //2012-09-12
            DocApp.DocAppDataTable docApp = null;
            if (Validation.IsHLENumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //HLE
                docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
            else if (Validation.IsSersNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //SERS
                docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
            else if (Validation.IsSalesNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //SALES
                docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());
            else if (Validation.IsResaleNumber(DocAppRadComboBox.SelectedItem.Text.Trim())) //RESALE
                docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.SelectedItem.Text.Trim());

            if (docApp != null)
            {
                if (docApp.Rows.Count > 0)
                {
                    DocApp.DocAppRow row = docApp[0];
                    ReferenceTypeLabel.Text = row.RefType;
                    ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                    #region 2012-09-06 List the household structure
                    if (row.RefType.ToUpper().Trim() == "HLE")
                    {
                        //SersInterface
                        HleInterfaceDb db = new HleInterfaceDb();
                        DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
                        householdStructureLabel.Text = GetHouseholdStructure(dt);
                    }
                    else if (row.RefType.ToUpper().Trim() == "SERS")
                    {
                        //SersInterface
                        SersInterfaceDb db = new SersInterfaceDb();
                        DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
                        householdStructureLabel.Text = GetHouseholdStructure(dt);
                    }
                    else if (row.RefType.ToUpper().Trim() == "SALES")
                    {
                        //SalesInterface
                        SalesInterfaceDb db = new SalesInterfaceDb();
                        DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
                        householdStructureLabel.Text = GetHouseholdStructure(dt);
                    }
                    else if (row.RefType.ToUpper().Trim() == "RESALE")
                    {
                        //ResaleInterface
                        ResaleInterfaceDb db = new ResaleInterfaceDb();
                        DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());
                        householdStructureLabel.Text = GetHouseholdStructure(dt);
                    }
                    else
                    {
                        householdStructureLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
                    }
                    #endregion 2012-09-06 List the household structure



                    ////update the address field based on the address for the docset sorted by submission date desc.
                    //DocSetDb docSetDb = new DocSetDb();
                    //DocSet.DocSetDataTable docSet = docSetDb.GetLatestDocSetByDocAppId(int.Parse(DocAppRadComboBox.SelectedValue));

                    //if (docSet.Rows.Count > 0)
                    //{
                    //    DocSet.DocSetRow docSetRow = docSet[0];
                    //    BlockTextBox.Text = docSetRow.IsBlockNull() ? string.Empty : docSetRow.Block;
                    //    StreetNameRadComboBox.SelectedValue = docSetRow.IsStreetIdNull() ? "1" : docSetRow.StreetId.ToString();
                    //    LevelTextBox.Text = docSetRow.IsFloorNull() ? string.Empty : docSetRow.Floor;
                    //    UnitNumberTextBox.Text = docSetRow.IsUnitNull() ? string.Empty : docSetRow.Unit;
                    //}

                    //update the address field based on the address for the sersinterface table.
                    SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                    SersInterface.SersInterfaceDataTable sersInterface = sersInterfaceDb.GetSersInterfaceByRefNo(DocAppRadComboBox.SelectedItem.Text.Trim());

                    if (sersInterface.Rows.Count > 0)
                    {
                        SersInterface.SersInterfaceRow sersInterfaceRow = sersInterface[0];
                        BlockTextBox.Text = sersInterfaceRow.IsBlockNull() ? string.Empty : sersInterfaceRow.Block;
                        if (sersInterfaceRow.IsStreetNull())
                            StreetNameRadComboBox.SelectedIndex = 0;
                        else
                        {
                            StreetDb streetDb = new StreetDb();
                            Street.StreetDataTable streets = streetDb.GetStreetByName(sersInterfaceRow.Street.Trim());

                            if (streets.Rows.Count > 0)
                            {
                                Street.StreetRow streetRow = streets[0];
                                StreetNameRadComboBox.SelectedValue = streetRow.Id.ToString();
                            }
                            else
                                StreetNameRadComboBox.SelectedIndex = 0;

                        }
                        LevelTextBox.Text = sersInterfaceRow.IsLevelNull() ? string.Empty : sersInterfaceRow.Level;
                        UnitNumberTextBox.Text = sersInterfaceRow.IsUnitNull() ? string.Empty : sersInterfaceRow.Unit;
                    }
                }
                else
                {
                    ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
                    ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                    householdStructureLabel.Text = "N.A";
                }
            }
            else
            {
                ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
                ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                householdStructureLabel.Text = "N.A";
            }


            ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.SelectedItem.Text.Trim()).Replace("_", " ");
            ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

            CheckRefNoChanged();
            CheckRefNoExistence();

            if (Request.Url.ToString().ToLower().Contains("/import/scandocuments/"))
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeTwainScript", "InitializeTwain();", true);
        }
    }

    protected void ClearButton_OnClick(object sender, EventArgs e)
    {
        BlockTextBox.Text = string.Empty;
        LevelTextBox.Text = string.Empty;
        UnitNumberTextBox.Text = string.Empty;
        StreetNameRadComboBox.SelectedIndex = 0;
    }

    protected void GetDocAppByAddressObjectDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["block"] = BlockTextBox.Text.Trim();
        e.InputParameters["streetName"] = StreetNameRadComboBox.Text.Equals("- Select Street -") ? string.Empty : StreetNameRadComboBox.Text.Trim();
        e.InputParameters["level"] = LevelTextBox.Text.Trim();
        e.InputParameters["unit"] = UnitNumberTextBox.Text.Trim();
    }

    protected void DocAppRadComboBox_OnSelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (!String.IsNullOrEmpty(DocAppRadComboBox.Text.Trim()))
        {
            if (Validation.IsNric(DocAppRadComboBox.Text.Trim()) ||
                Validation.IsFin(DocAppRadComboBox.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "ChooseReferenceScript",
                    String.Format("ShowWindow('../../Common/ChooseReference.aspx?nric={0}', 500, 500);", DocAppRadComboBox.Text.Trim()),
                    true);
            }
            else
            {
                DocAppDb docAppDb = new DocAppDb();

                //2012-09-12
                DocApp.DocAppDataTable docApp = null;
                if (Validation.IsHLENumber(DocAppRadComboBox.Text.Trim())) //HLE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.Text.Trim());
                else if (Validation.IsSersNumber(DocAppRadComboBox.Text.Trim())) //SERS
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.Text.Trim());
                else if (Validation.IsSalesNumber(DocAppRadComboBox.Text.Trim())) //SALES
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.Text.Trim());
                else if (Validation.IsResaleNumber(DocAppRadComboBox.Text.Trim())) //RESALE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadComboBox.Text.Trim());

                if (docApp != null)
                {
                    if (docApp.Rows.Count > 0)
                    {
                        DocApp.DocAppRow row = docApp[0];
                        ReferenceTypeLabel.Text = row.RefType;
                        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                        DocAppHiddenValue.Value = row.Id.ToString();
                        DocAppRadTextBox.Text = row.RefNo;


                        #region 2012-09-06 List the household structure
                        if (row.RefType.ToUpper().Trim() == "HLE")
                        {
                            //SersInterface
                            HleInterfaceDb db = new HleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.Text.Trim());
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SERS")
                        {
                            //SersInterface
                            SersInterfaceDb db = new SersInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SALES")
                        {
                            //SalesInterface
                            SalesInterfaceDb db = new SalesInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "RESALE")
                        {
                            //ResaleInterface
                            ResaleInterfaceDb db = new ResaleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadComboBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else
                        {
                            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
                        }
                        #endregion 2012-09-06 List the household structure



                        ////update the address field based on the address for the docset sorted by submission date desc.
                        //DocSetDb docSetDb = new DocSetDb();
                        //DocSet.DocSetDataTable docSet = docSetDb.GetLatestDocSetByDocAppId(int.Parse(DocAppRadComboBox.SelectedValue));

                        //if (docSet.Rows.Count > 0)
                        //{
                        //    DocSet.DocSetRow docSetRow = docSet[0];
                        //    BlockTextBox.Text = docSetRow.IsBlockNull() ? string.Empty : docSetRow.Block;
                        //    StreetNameRadComboBox.SelectedValue = docSetRow.IsStreetIdNull() ? "1" : docSetRow.StreetId.ToString();
                        //    LevelTextBox.Text = docSetRow.IsFloorNull() ? string.Empty : docSetRow.Floor;
                        //    UnitNumberTextBox.Text = docSetRow.IsUnitNull() ? string.Empty : docSetRow.Unit;
                        //}

                        //update the address field based on the address for the sersinterface table.
                        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                        SersInterface.SersInterfaceDataTable sersInterface = sersInterfaceDb.GetSersInterfaceByRefNo(DocAppRadComboBox.Text);

                        if (sersInterface.Rows.Count > 0)
                        {
                            SersInterface.SersInterfaceRow sersInterfaceRow = sersInterface[0];
                            BlockTextBox.Text = sersInterfaceRow.IsBlockNull() ? string.Empty : sersInterfaceRow.Block;
                            if (sersInterfaceRow.IsStreetNull())
                                StreetNameRadComboBox.SelectedIndex = 0;
                            else
                            {
                                StreetDb streetDb = new StreetDb();
                                Street.StreetDataTable streets = streetDb.GetStreetByName(sersInterfaceRow.Street.Trim());

                                if (streets.Rows.Count > 0)
                                {
                                    Street.StreetRow streetRow = streets[0];
                                    StreetNameRadComboBox.SelectedValue = streetRow.Id.ToString();
                                }
                                else
                                    StreetNameRadComboBox.SelectedIndex = 0;

                            }
                            LevelTextBox.Text = sersInterfaceRow.IsLevelNull() ? string.Empty : sersInterfaceRow.Level;
                            UnitNumberTextBox.Text = sersInterfaceRow.IsUnitNull() ? string.Empty : sersInterfaceRow.Unit;
                        }
                    }
                    else
                    {
                        ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
                        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                        householdStructureLabel.Text = "N.A";
                    }
                }
                else
                {
                    ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
                    ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                    householdStructureLabel.Text = "N.A";
                }


                ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
                ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                CheckRefNoChanged();
                CheckRefNoExistence();

                if (Request.Url.ToString().ToLower().Contains("/import/scandocuments/"))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeTwainScript", "InitializeTwain();", true);
            }
        }
        else
        {
            ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadComboBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = "N.A";
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// Block number custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void BlockCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.IsBlockNumber(BlockTextBox.Text.Trim());
    }

    /// <summary>
    /// Level custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void LevelCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.IsLevelNumber(LevelTextBox.Text.Trim());
    }

    /// <summary>
    /// Unit number custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void UnitNoCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = Validation.IsUnitNumber(UnitNumberTextBox.Text.Trim());
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate the streetNames
    /// </summary>
    private void PopulateStreetNames()
    {
        StreetDb streetDb = new StreetDb();
        StreetNameRadComboBox.DataSource = streetDb.GetStreet();
        StreetNameRadComboBox.DataTextField = "Name";
        StreetNameRadComboBox.DataValueField = "Id";
        StreetNameRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate Channels
    /// </summary>
    private void PopulateChannels()
    {
        MasterListDb masterListDb = new MasterListDb();
        MasterListItemDb masterListItemDb = new MasterListItemDb();

        MasterList.MasterListDataTable masterListTable = new MasterList.MasterListDataTable();
        MasterListItem.MasterListItemDataTable masterListItemTable = new MasterListItem.MasterListItemDataTable();
        DataTable allChannelMasterListItem = new DataTable();

        if (String.IsNullOrEmpty(url))
            url = Request.Url.ToString().ToLower();

        if (url.Contains("/scandocuments/"))
            masterListTable = masterListDb.GetMasterListNyName(MasterListEnum.Scanning_Channels);
        else if (url.Contains("/uploaddocuments/"))
            masterListTable = masterListDb.GetMasterListNyName(MasterListEnum.Uploading_Channels);
        else if (url.Contains("/verification/"))
        {
            allChannelMasterListItem = masterListItemDb.GetAllChannelMasterListItem();
            RemarkPanel.Visible = true;
        }

        if (masterListTable.Rows.Count > 0)
        {
            MasterList.MasterListRow masterList = masterListTable[0];
            masterListItemTable = masterListItemDb.GetMasterListItemByMasterListId(masterList.Id);
            ChannelDropDownList.DataSource = masterListItemTable;
        }
        else
            ChannelDropDownList.DataSource = allChannelMasterListItem;

        ChannelDropDownList.DataTextField = "Name";
        ChannelDropDownList.DataValueField = "Name";
        ChannelDropDownList.DataBind();
    }

    private string GetHouseholdStructure(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        foreach (DataRow r in dt.Rows)
        {
                sb.Append(r["ApplicantType"].ToString());
                sb.Append(r["OrderNo"].ToString());
                sb.Append(": ");
                sb.Append(r["Name"].ToString());
                sb.Append("<br />");
        }
        return string.IsNullOrEmpty(sb.ToString()) ? "N.A." : sb.ToString();
    }


    #endregion

    #region Public Methods
    /// <summary>
    /// Clear the inputs
    /// </summary>
    public void ClearInputs()
    {
        ReferenceTypeLabel.Text = "N.A.";
        BlockTextBox.Text = string.Empty;
        StreetNameRadComboBox.SelectedIndex = 0;
        LevelTextBox.Text = string.Empty;
        UnitNumberTextBox.Text = string.Empty;
        //DocAppRadComboBox.SelectedIndex = 0;
        DocAppRadTextBox.Text = string.Empty;
        DocAppHiddenValue.Value = string.Empty;
    }


    //private void CheckRefNoChanged()
    //{
    //    if (!Request.Url.ToString().ToLower().Contains("/import/") && !Request.Url.ToString().ToLower().Contains("/upload/"))
    //    {
    //        if (int.Parse(CurrentDocAppId) != int.Parse(DocAppRadComboBox.SelectedValue))
    //            RefNoChangedLabel.Visible = true;
    //        else
    //            RefNoChangedLabel.Visible = false;
    //    }
    //}

    private void CheckRefNoChanged()
    {
        if (!Request.Url.ToString().ToLower().Contains("/import/") && !Request.Url.ToString().ToLower().Contains("/upload/"))
        {
            if (int.Parse(CurrentDocAppId) != int.Parse(DocAppHiddenValue.Value))
                RefNoChangedLabel.Visible = true;
            else
                RefNoChangedLabel.Visible = false;
        }
    }

    private void CheckRefNoExistence()
    {
        int docAppId = DocAppId;
        string referenceNo = ReferenceNumber;

        NewRefWarningLabel.Visible = false;

        // Insert a Doc App record if no reference number is keyed in
        if (docAppId == 0 && !string.IsNullOrEmpty(referenceNo))
        {
            DocAppDb docAppDb = new DocAppDb();

            // If the reference number keyed in is a NRIC,
            // search the reference number of the NRIC from the interface file.
            // The retrieved reference number will be used to compare to the DocApp table.
            // If found, use the id of the DocApp. Else, create a new DocApp record.
            if (Validation.IsNric(referenceNo) || Validation.IsFin(referenceNo))
            {
                HleInterfaceDb interfaceDb = new HleInterfaceDb();
                HleInterface.HleInterfaceDataTable dt = interfaceDb.GetHleInterfaceByNric(referenceNo);

                if (dt.Rows.Count > 0)
                {
                    HleInterface.HleInterfaceRow dr = dt[0];

                    DocApp.DocAppDataTable docAppDt = docAppDb.GetDocAppsByReferenceNo(dr.HleNumber.Trim());

                    if (docAppDt.Rows.Count <= 0)
                    {
                        NewRefWarningLabel.Visible = true;
                    }
                }
                else
                {
                    NewRefWarningLabel.Visible = true;
                }
            }
            else
            {
                DocApp.DocAppDataTable docAppDt = docAppDb.GetDocAppsByReferenceNo(referenceNo);

                if (docAppDt.Rows.Count <= 0)
                {
                    NewRefWarningLabel.Visible = true;
                }
            }
        }
    }

    public void SetComboBoxText(string refNo)
    {
        DocAppRadTextBox.Text = refNo;
    }
    #endregion

    protected void DocAppRadTextBox_TextChanged(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {
            if (Validation.IsNric(DocAppRadTextBox.Text.Trim()) ||
                Validation.IsFin(DocAppRadTextBox.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(
                    this.Page,
                    this.GetType(),
                    "ChooseReferenceScript",
                    String.Format("ShowWindow('../../Common/ChooseReference.aspx?nric={0}', 500, 500);", DocAppRadTextBox.Text.Trim()),
                    true);
            }
            else
            {
                DocAppDb docAppDb = new DocAppDb();

                //2012-09-12
                DocApp.DocAppDataTable docApp = null;
                if (Validation.IsHLENumber(DocAppRadTextBox.Text.Trim())) //HLE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsSersNumber(DocAppRadTextBox.Text.Trim())) //SERS
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsSalesNumber(DocAppRadTextBox.Text.Trim())) //SALES
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());
                else if (Validation.IsResaleNumber(DocAppRadTextBox.Text.Trim())) //RESALE
                    docApp = docAppDb.GetDocAppsByReferenceNo(DocAppRadTextBox.Text.Trim());

                if (docApp != null)
                {
                    if (docApp.Rows.Count > 0)
                    {
                        DocApp.DocAppRow row = docApp[0];
                        ReferenceTypeLabel.Text = row.RefType;
                        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                        DocAppHiddenValue.Value = row.Id.ToString();


                        #region 2012-09-06 List the household structure
                        if (row.RefType.ToUpper().Trim() == "HLE")
                        {
                            //SersInterface
                            HleInterfaceDb db = new HleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text.Trim());
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SERS")
                        {
                            //SersInterface
                            SersInterfaceDb db = new SersInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "SALES")
                        {
                            //SalesInterface
                            SalesInterfaceDb db = new SalesInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else if (row.RefType.ToUpper().Trim() == "RESALE")
                        {
                            //ResaleInterface
                            ResaleInterfaceDb db = new ResaleInterfaceDb();
                            DataTable dt = db.GetApplicantDetailsByRefNo(DocAppRadTextBox.Text);
                            householdStructureLabel.Text = GetHouseholdStructure(dt);
                        }
                        else
                        {
                            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
                        }
                        #endregion 2012-09-06 List the household structure



                        ////update the address field based on the address for the docset sorted by submission date desc.
                        //DocSetDb docSetDb = new DocSetDb();
                        //DocSet.DocSetDataTable docSet = docSetDb.GetLatestDocSetByDocAppId(int.Parse(DocAppRadTextBox.SelectedValue));

                        //if (docSet.Rows.Count > 0)
                        //{
                        //    DocSet.DocSetRow docSetRow = docSet[0];
                        //    BlockTextBox.Text = docSetRow.IsBlockNull() ? string.Empty : docSetRow.Block;
                        //    StreetNameRadComboBox.SelectedValue = docSetRow.IsStreetIdNull() ? "1" : docSetRow.StreetId.ToString();
                        //    LevelTextBox.Text = docSetRow.IsFloorNull() ? string.Empty : docSetRow.Floor;
                        //    UnitNumberTextBox.Text = docSetRow.IsUnitNull() ? string.Empty : docSetRow.Unit;
                        //}

                        //update the address field based on the address for the sersinterface table.
                        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                        SersInterface.SersInterfaceDataTable sersInterface = sersInterfaceDb.GetSersInterfaceByRefNo(DocAppRadTextBox.Text);

                        if (sersInterface.Rows.Count > 0)
                        {
                            SersInterface.SersInterfaceRow sersInterfaceRow = sersInterface[0];
                            BlockTextBox.Text = sersInterfaceRow.IsBlockNull() ? string.Empty : sersInterfaceRow.Block;
                            if (sersInterfaceRow.IsStreetNull())
                                StreetNameRadComboBox.SelectedIndex = 0;
                            else
                            {
                                StreetDb streetDb = new StreetDb();
                                Street.StreetDataTable streets = streetDb.GetStreetByName(sersInterfaceRow.Street.Trim());

                                if (streets.Rows.Count > 0)
                                {
                                    Street.StreetRow streetRow = streets[0];
                                    StreetNameRadComboBox.SelectedValue = streetRow.Id.ToString();
                                }
                                else
                                    StreetNameRadComboBox.SelectedIndex = 0;

                            }
                            LevelTextBox.Text = sersInterfaceRow.IsLevelNull() ? string.Empty : sersInterfaceRow.Level;
                            UnitNumberTextBox.Text = sersInterfaceRow.IsUnitNull() ? string.Empty : sersInterfaceRow.Unit;
                        }
                    }
                    else
                    {
                        ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
                        ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                        householdStructureLabel.Text = "N.A";
                    }
                }
                else
                {
                    ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
                    ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;
                    householdStructureLabel.Text = "N.A";
                }


                ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
                ReferenceTypeHiddenField.Value = ReferenceTypeLabel.Text;

                CheckRefNoChanged();
                CheckRefNoExistence();

                if (Request.Url.ToString().ToLower().Contains("/import/scandocuments/"))
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "InitializeTwainScript", "InitializeTwain();", true);
            }
        }
        else
        {
            ReferenceTypeLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = Util.GetReferenceType(DocAppRadTextBox.Text.Trim()).Replace("_", " ");
            householdStructureLabel.Text = "N.A";
        }
    }
}
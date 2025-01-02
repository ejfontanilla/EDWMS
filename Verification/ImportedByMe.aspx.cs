using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Verification_ImportedByMe : System.Web.UI.Page
{
    int userSectionId = -1;

    #region Event Handlers
    /// <summary>
    /// Page Load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            PopulateChannels();
            PopulateStatus();
            PopulateReferenceNo();
        }

        //show address if the user belong to SERS department
        UserDb userDb = new UserDb();
        userSectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
        RadGrid1.Columns.FindByUniqueName("Address").Visible = (userSectionId == 3 || userSectionId == 4);
    }

    /// <summary>
    /// Rad Grid 1 need data source event
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        PopulateList();
    }

    /// <summary>
    /// RadGrid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataBoundItem = e.Item as GridDataItem;
            string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

            HyperLink VerifyHyperLink = e.Item.FindControl("VerifyHyperLink") as HyperLink;
            HyperLink ViewSetHyperLink = e.Item.FindControl("ViewSetHyperLink") as HyperLink;
            Label AppInformationLabel = e.Item.FindControl("AppInformationLabel") as Label;
            Image UrgentImage = e.Item.FindControl("UrgentImage") as Image;
            Label WaitingTimeLabel = e.Item.FindControl("WaitingTimeLabel") as Label;
            Label ProcessingTimeLabel = e.Item.FindControl("ProcessingTimeLabel") as Label;
            
            Label AddressLabel = new Label();

            DataRowView data = (DataRowView)e.Item.DataItem;
            int sectionId = int.Parse(data["SectionId"].ToString()); // Get the sectionId
            int id = int.Parse(DataBinder.Eval(e.Item.DataItem, "Id").ToString()); //set setid

            //VerifyLinkButton.Enabled = ViewSetHyperLink.Enabled = (data["Status"].ToString().Equals("New"));
            VerifyHyperLink.Enabled = (data["Status"].ToString().Equals(SetStatusEnum.New.ToString()) && (userSectionId == sectionId));
            ViewSetHyperLink.Enabled = !(data["Status"].ToString().Equals(SetStatusEnum.Pending_Categorization.ToString()) ||
                data["Status"].ToString().Equals(SetStatusEnum.Categorization_Failed.ToString()));


            VerifyHyperLink.Text = VerifyHyperLink.Enabled ? VerifyHyperLink.Text : "-";
            ViewSetHyperLink.Text = ViewSetHyperLink.Enabled ? ViewSetHyperLink.Text : "-";

            DocSetDb docSetDb = new DocSetDb();
            string setStatus = docSetDb.GetSetStatus(id);
            SetStatusEnum setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), setStatus, true);

            // Compute the Waiting time
            DateTime processingEndDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingEndDate").ToString());
            DateTime verificationDateIn = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "VerificationDateIn").ToString());
            Util.DisplayWaitingTime(WaitingTimeLabel, processingEndDate, verificationDateIn, setStatusEnum);

            //compute Processing time
            DateTime processingStartDate = DateTime.Parse(DataBinder.Eval(e.Item.DataItem, "ProcessingStartDate").ToString());
            Util.DisplayProcessingTime(ProcessingTimeLabel, processingStartDate, processingEndDate, setStatusEnum);

            //set Urgency indicator
            Util.DisplayUrgencyIndicator(setStatusEnum, UrgentImage, id, DataBinder.Eval(e.Item.DataItem, "IsUrgent").ToString().Equals("True"),
                DataBinder.Eval(e.Item.DataItem, "SkipCategorization").ToString().Equals("True"), null);

            //Applicattion Information
            int docSetId = int.Parse(data["Id"].ToString());

            SetAppDb setAppDb = new SetAppDb();
            SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(docSetId);

            if (setApps.Rows.Count > 0)
            {
                foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                {
                    Util.DisplayApplicationInformation(AppInformationLabel, setAppRow);

                    //show address only for SERS sections (3,4)
                    if (sectionId == 3 || sectionId == 4)
                    {
                        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                        AddressLabel.Text += sersInterfaceDb.GetSersAddressByRefNo(setAppRow.RefNo) + "<br>";
                    }

                    //Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    //Util.DisplayHasPendingDocIndicator(NoPendingDoc, setAppRow.RefNo.ToString());

                    Image NoPendingDoc = e.Item.FindControl("NoPendingDoc") as Image;
                    if (Util.DisplayHasPendingDocIndicator(setAppRow.RefNo.ToString()))
                    {
                        NoPendingDoc.ImageUrl = "../Data/Images/Icons/Thumb_up.png";
                        NoPendingDoc.ToolTip = Constants.HasPendingDocInApplication;
                        NoPendingDoc.Visible = true;
                    }
                }
            }
            else
                AppInformationLabel.Text = "-";

            dataBoundItem["Address"].Text = string.IsNullOrEmpty(AddressLabel.Text.Trim()) ? "-" : AddressLabel.Text;

        }
    }


    /// <summary>
    /// Search button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        RadGrid1.Rebind();
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Search functions
    /// </summary>

    /// <summary>
    /// Ppopulat the status drop down list
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.EnumToDataTable(typeof(SetStatusEnum));
        StatusRadComboBox.DataTextField = "Text";
        StatusRadComboBox.DataValueField = "Value";
        StatusRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the channels
    /// </summary>
    private void PopulateChannels()
    {
        DocSetDb docSetDb = new DocSetDb();
        ChannelRadComboBox.DataSource = docSetDb.GetChannelsForDropDown();
        ChannelRadComboBox.DataTextField = "Channel";
        ChannelRadComboBox.DataValueField = "Channel";
        ChannelRadComboBox.DataBind();
    }

    /// <summary>
    /// Populate the reference number
    /// </summary>
    private void PopulateReferenceNo()
    {
        //DocAppDb docAppDb = new DocAppDb();
        //RefNoRadComboBox.DataSource = docAppDb.GetDocApp();
        //RefNoRadComboBox.DataValueField = "Id";
        //RefNoRadComboBox.DataTextField = "RefNo";
        //RefNoRadComboBox.DataBind();
    }

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        ChannelRadComboBox.SelectedIndex = 0;
        StatusRadComboBox.SelectedIndex = 0;
        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;
        IsUrgentCheckBox.Checked = false;
        SkipCategorizationCheckBox.Checked = false;

        /*AcknowledgeNumberRadComboBox.Text = string.Empty;
        AcknowledgeNumberRadComboBox.SelectedValue = string.Empty;
        CmDocumentIdComboBox.Text = string.Empty;
        CmDocumentIdComboBox.SelectedValue = string.Empty;*/ 

        PopulateList();
        RebindGrids();
    }

    private void RebindGrids()
    {
        RadGrid1.DataBind();
    }

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        string channel = ChannelRadComboBox.SelectedValue;
        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
        #region Update by Edward 2017/11/13 to address OOM
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue))
        //    {
        //        docAppId = int.Parse(RefNoRadComboBox.SelectedValue);
        //    }
        //    else
        //    {
        //        RadComboBoxItem selectedItem = RefNoRadComboBox.FindItemByText(RefNoRadComboBox.Text.Trim());

        //        if (selectedItem != null)
        //        {
        //            docAppId = int.Parse(selectedItem.Value);
        //        }
        //        else
        //        {
        //            DocAppDb docAppDb = new DocAppDb();
        //            DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

        //            if (docAppTable.Rows.Count > 0)
        //            {
        //                DocApp.DocAppRow docAppRow = docAppTable[0];
        //                docAppId = docAppRow.Id;
        //            }
        //        }
        //    }
        //}
        if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {
            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(DocAppRadTextBox.Text.Trim());
        }
        #endregion

        //get nric
        string nric = string.Empty;
        //if (!string.IsNullOrEmpty(NricRadComboBox.Text.Trim()))
        //{
        //    if (!string.IsNullOrEmpty(NricRadComboBox.SelectedValue))
        //    {
        //        nric = NricRadComboBox.SelectedValue;
        //    }
        //    else
        //        nric = NricRadComboBox.Text.Trim();
        //}
        if (!string.IsNullOrEmpty(NricRadTextBox.Text.Trim()))
        {
            nric = NricRadTextBox.Text.Trim();
        }

        //get AcknowledgeNumber
        string acknowledgeNumber = string.Empty;
        /*if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(AcknowledgeNumberRadComboBox.SelectedValue))
            {
                acknowledgeNumber = AcknowledgeNumberRadComboBox.SelectedValue;
            }
            else
                acknowledgeNumber = AcknowledgeNumberRadComboBox.Text.Trim();
        }*/

        //get CmDocumentId
        string cmDocumentId = string.Empty;
        /*if (!string.IsNullOrEmpty(CmDocumentIdComboBox.Text.Trim()))
        {
            if (!string.IsNullOrEmpty(CmDocumentIdComboBox.SelectedValue))
            {
                cmDocumentId = CmDocumentIdComboBox.SelectedValue;
            }
            else
                cmDocumentId = CmDocumentIdComboBox.Text.Trim();
        }*/

        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        DocSetDb docsetDb = new DocSetDb();

        SetStatusEnum setStatusEnum = new SetStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            setStatusEnum = (SetStatusEnum)Enum.Parse(typeof(SetStatusEnum), StatusRadComboBox.SelectedValue, true);
            RadGrid1.DataSource = docsetDb.GetSets(channel, setStatusEnum, string.Empty, -1, -1, (Guid)Membership.GetUser().ProviderUserKey, null, dateInFrom, dateInTo, docAppId, nric, IsUrgentCheckBox.Checked, SkipCategorizationCheckBox.Checked, cmDocumentId, acknowledgeNumber);
        }
        else
            RadGrid1.DataSource = docsetDb.GetSets(channel, null, string.Empty, -1, -1, (Guid)Membership.GetUser().ProviderUserKey, null, dateInFrom, dateInTo, docAppId, nric, IsUrgentCheckBox.Checked, SkipCategorizationCheckBox.Checked, cmDocumentId, acknowledgeNumber);

    }
    #endregion
}

using System;
using System.Collections;
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

public partial class IncomeAssessment_AssignedToMe : System.Web.UI.Page
{
    int sectionId = -1;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        UserDb userDb = new UserDb();
        sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);

        if (!IsPostBack)
        {
            PopulateStatus();

            //show address if the user belong to SERS department
            RadGrid1.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
            //StatusRadComboBox.SelectedValue = AppStatusEnum.Completeness_In_Progress.ToString();
        }
    }    

    /// <summary>
    /// Rad grid need data source
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
        //Added By Edward 17/3/2014 Sales and Resale Changes
        if (sectionId == 5 || sectionId == 6)
        {
            RadGrid1.MasterTableView.Columns[4].HeaderText = "OIC 1";
            RadGrid1.MasterTableView.Columns[5].HeaderText = "OIC 2";
        }

        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;
            
            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            //show address only for SERS sections (3,4)
            Label AddressLabel = e.Item.FindControl("AddressLabel") as Label;
            LinkButton SetCountLinkButton = e.Item.FindControl("SetCountLinkButton") as LinkButton;
            if (sectionId == 3 || sectionId == 4)
            {
                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                AddressLabel.Text = sersInterfaceDb.GetSersAddressByRefNo(data["RefNo"].ToString());
            }

            // Compute the age value
            DateTime dateInDateTime;

            if (Validation.IsDate(data["AssessmentDateIn"].ToString()))
            {
                #region Modified By Eward 30/6/2014 Get the Aging subtract to DateOut
                // Compute the age value
                //dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                //TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
                //AgingLabel.Text = Format.GetAging(diff);
                dateInDateTime = DateTime.Parse(data["AssessmentDateIn"].ToString());
                DateTime dateOutDateTime = !string.IsNullOrEmpty(data["AssessmentDateOut"].ToString()) ? DateTime.Parse(data["AssessmentDateOut"].ToString()) : DateTime.Now;
                AgingLabel.Text = Format.CalculateAging(dateInDateTime, dateOutDateTime);
                #endregion
            }
            else
            {
                AgingLabel.Text = "? Day(s)";
            }


            //Set Information
            int docAppId = int.Parse(data["Id"].ToString());
            Label SetInformationLabel = e.Item.FindControl("SetInformationLabel") as Label;

            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSets = docSetDb.GetDocSetByDocAppId(docAppId);

            int setCount = docSets.Rows.Count;
            Util.DisplaySetInformation(setCount, SetCountLinkButton, docAppId);

            //show ! and ? indicators
            string status = data["Status"].ToString().Trim(); // Get the status
            if (!status.Equals(AppStatusEnum.Completeness_Cancelled.ToString()))
            {
                // check if the docapp have all sets verified.
                //set the NotVerifiedFiles Indicator
                Image SetsVerifiedIndicatorImage = e.Item.FindControl("SetsVerifiedIndicatorImage") as Image;
                Util.DisplaySetsNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId);

                //set the NotVerifiedFiles Indicator
                // check if the docapp is checked and have files which are not verified.
                Image FilesIndicatorImage = e.Item.FindControl("FilesIndicatorImage") as Image;
                Util.DisplayFilesNotVerifiedIndicator(SetsVerifiedIndicatorImage, docAppId, status, setCount);
            }
        }
    }

    /// <summary>
    /// Search button clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        RadGrid1.Rebind();
    }

    /// <summary>
    /// Reset the search query
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ResetButton_Click(object sender, EventArgs e)
    {
        StatusRadComboBox.SelectedIndex = 0;
        //RefNoRadComboBox.Text = string.Empty;
        DocAppRadTextBox.Text = string.Empty;
        //NricRadComboBox.Text = string.Empty;
        NricRadTextBox.Text = string.Empty;
        DateInFromRadDateTimePicker.SelectedDate = null;
        DateInToRadDateTimePicker.SelectedDate = null;

        RadGrid1.Rebind();
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Populate the list
    /// </summary>
    private void PopulateList()
    {
        DocAppDb docAppDb = new DocAppDb();

        int docAppId = -1;
        //if (!string.IsNullOrEmpty(RefNoRadComboBox.SelectedValue.Trim()))
        //    docAppId = int.Parse(RefNoRadComboBox.SelectedValue);

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
        //            //DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppsByReferenceNo(RefNoRadComboBox.Text.Trim());

        //            //if (docAppTable.Rows.Count > 0)
        //            //{
        //            //    DocApp.DocAppRow docAppRow = docAppTable[0];
        //            //    docAppId = docAppRow.Id;
        //            //}
        //            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(RefNoRadComboBox.Text.Trim());
        //        }
        //    }
        //}

        if (!string.IsNullOrEmpty(DocAppRadTextBox.Text.Trim()))
        {
            docAppId = DocAppDb.GetDocAppIDOnlyByRefNo(DocAppRadTextBox.Text.Trim());
        }


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
        DateTime? dateInFrom = null;
        DateTime? dateInTo = null;
        Guid currUserId = (Guid)Membership.GetUser().ProviderUserKey;

        if (DateInFromRadDateTimePicker.SelectedDate.HasValue)
            dateInFrom = DateInFromRadDateTimePicker.SelectedDate.Value;

        if (DateInToRadDateTimePicker.SelectedDate.HasValue)
            dateInTo = DateInToRadDateTimePicker.SelectedDate.Value;

        // Get the sets for the current logged-in user only

        AssessmentStatusEnum assessmentStatusEnum = new AssessmentStatusEnum();
        string status = StatusRadComboBox.SelectedValue;

        if (!status.Equals("-1"))
        {
            assessmentStatusEnum = (AssessmentStatusEnum)Enum.Parse(typeof(AssessmentStatusEnum), StatusRadComboBox.SelectedValue, true);
            RadGrid1.DataSource = docAppDb.GetAppsAssignedToUserIA(dateInFrom, dateInTo, currUserId, assessmentStatusEnum, docAppId, nric);
        }
        else
            RadGrid1.DataSource = docAppDb.GetAppsAssignedToUserIA(dateInFrom, dateInTo, currUserId, null, docAppId, nric);
    }

    /// <summary>
    /// Populate the status
    /// </summary>
    private void PopulateStatus()
    {
        StatusRadComboBox.DataSource = EnumManager.GetAssessmentStatus();
        StatusRadComboBox.DataTextField = "Text";
        StatusRadComboBox.DataValueField = "Value";
        StatusRadComboBox.DataBind();
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
    #endregion
}

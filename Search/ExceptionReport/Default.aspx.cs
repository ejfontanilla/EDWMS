using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using System.IO;
using Dwms.Web;

public partial class Search_ExceptionReport_Default : System.Web.UI.Page
{
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
        }
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

    /// <summary>
    /// Item command event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemCommand(object sender, GridCommandEventArgs e)
    {        
    }
    #endregion

    #region Private Methods
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

    protected void ResetButton_Click(object sender, EventArgs e)
    {
        ChannelRadComboBox.SelectedIndex = 0;
        RefNoRadComboBox.Text = string.Empty;
        RefNoRadComboBox.SelectedIndex = 0;

        DateFromRadDateTimePicker.SelectedDate = null;

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
        string channel = (ChannelRadComboBox.SelectedValue == "-1" ? string.Empty : ChannelRadComboBox.SelectedValue);
        string refNo = RefNoRadComboBox.Text.Trim();

        DateTime? dateFrom = null;
        DateTime? dateTo = null;

        if (DateFromRadDateTimePicker.SelectedDate.HasValue)
            dateFrom = DateFromRadDateTimePicker.SelectedDate.Value;

        if (DateToRadDateTimePicker.SelectedDate.HasValue)
            dateTo = DateToRadDateTimePicker.SelectedDate.Value;

        ExceptionLogDb exceptionLogDb = new ExceptionLogDb();
        RadGrid1.DataSource = exceptionLogDb.GetExceptionLogForExceptionReport(channel, refNo, dateFrom, dateTo);
    }
    #endregion
}

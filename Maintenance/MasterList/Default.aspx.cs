using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.Collections.Generic;

public partial class Maintenance_MasterList_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");
        WarningPanel.Visible = (Request["cfm"] == "0");

        if (!IsPostBack)
        {
            PopulateList();
            PopulateListDetails();
        }

        AddRadButton.Attributes.Add("onclick", "javascript:ShowWindow('AddValues.aspx'," + Constants.WindowWidth + "," + Constants.WindowHeight + ");");

        // Set the access control for the user
        SetAccessControl();
    }

    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            bool result = SaveListDetails();

            Response.Redirect(String.Format("Default.aspx?cfm=1", (result ? "1" : "0")));
        }
    }

    /// <summary>
    /// List selected index changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ListDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateListDetails();
    }

    /// <summary>
    /// Rad ajax manager ajax request event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        // Add the new item
        ValuesRadListBox.Items.Add(new RadListBoxItem(e.Argument, (-1).ToString()));
    }
    #endregion

    #region Validation
    /// <summary>
    /// Keyword list custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void ValuesRadListBoxCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (ValuesRadListBox.Items.Count > 0);
    }
    #endregion  

    #region Private Methods
    /// <summary>
    /// Populate list types
    /// </summary>
    private void PopulateList()
    {
        MasterListDb masterListDb = new MasterListDb();
        ListDropDownList.DataSource = masterListDb.GetEditableMasterList();
        ListDropDownList.DataTextField = "Name";
        ListDropDownList.DataValueField = "Id";
        ListDropDownList.DataBind();
    }

    /// <summary>
    /// Populate list details
    /// </summary>
    private void PopulateListDetails()
    {
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        ValuesRadListBox.DataSource = masterListItemDb.GetMasterListItemByMasterListId(int.Parse(ListDropDownList.SelectedValue));
        ValuesRadListBox.DataTextField = "Name";
        ValuesRadListBox.DataValueField = "Id";
        ValuesRadListBox.DataBind();

    }


    /// <summary>
    /// Get the values from list box
    /// </summary>
    /// <returns></returns>
    private List<string[]> GetValues()
    {
        List<string[]> keywords = new List<string[]>();

        // Loop through list box items
        foreach (RadListBoxItem keyword in ValuesRadListBox.Items)
        {
            string[] data = new string[2];
            data[0] = keyword.Value;
            data[1] = keyword.Text;

            keywords.Add(data);
        }

        return keywords;
    }

    /// <summary>
    /// Save the list details
    /// </summary>
    /// <returns></returns>
    private bool SaveListDetails()
    {
        MasterListItemDb masterListItemDb = new MasterListItemDb();
        ArrayList currentChannelIds = new ArrayList();
        ArrayList channelToDelete = new ArrayList();
        List<string[]> values = GetValues();

        MasterListItem.MasterListItemDataTable masterListItems = masterListItemDb.GetMasterListItemByMasterListId(int.Parse(ListDropDownList.SelectedValue));

        // Get the current channels
        foreach (MasterListItem.MasterListItemRow masterListItemRow in masterListItems.Rows)
        {
            if (!currentChannelIds.Contains(masterListItemRow.Id))
                currentChannelIds.Add(masterListItemRow.Id);
        }

        // Get the channels to be deleted
        foreach (int currentId in currentChannelIds)
        {
            bool toBeDeleted = true;

            foreach (string[] data in values)
            {
                int id = int.Parse(data[0]);

                if (id == currentId)
                {
                    toBeDeleted = false;
                    break;
                }
            }

            if (toBeDeleted)
            {
                if (!channelToDelete.Contains(currentId))
                    channelToDelete.Add(currentId);
            }
        }

        int itemOrder = 1;

        // Get the channels to update
        foreach (string[] data in values)
        {
            int id = int.Parse(data[0]);
            string name = data[1];

            if (id > 0)
            {
                masterListItemDb.UpdateOrder(id, itemOrder);
            }
            else
            {
                masterListItemDb.Insert(int.Parse(ListDropDownList.SelectedValue), name, itemOrder);
            }
            itemOrder++;
        }

        // Delete channels
        foreach (int id in channelToDelete)
        {
            masterListItemDb.Delete(id);
        }


        return true;
    }

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
        AddRadButton.Visible = hasAccess;
    }
    #endregion
}
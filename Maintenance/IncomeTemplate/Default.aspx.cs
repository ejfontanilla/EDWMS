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

public partial class Maintenance_IncomeTemplate_Default : System.Web.UI.Page
{
    bool hasAccess;
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");
        WarningPanel.Visible = (Request["cfm"] == "0");

        if (!IsPostBack)
        {
            PopulateList();
            PopulateListDetails();
        }

        // Set the access control for the user
        SetAccessControl();
        CreateButtons();               
    }

    private void CreateButtons()
    {
        AddRadButton.Attributes.Add("onclick", string.Format("javascript:ShowWindow('AddValues.aspx?id={0}&mode={1}',{2},{3})",
           ListDropDownList.Items.Count > 0 ? int.Parse(ListDropDownList.SelectedValue) : 0, "add", Constants.WindowWidth, Constants.WindowHeight));
        if (!hasAccess)
            AddTemplateButton.Visible = false;
        else
            AddTemplateButton.Attributes.Add("onclick", "javascript:ShowWindow('AddTemplate.aspx'," + Constants.WindowWidth + "," + Constants.WindowHeight + ");");
    }


    /// <summary>
    /// Save user account
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //protected void Save(object sender, EventArgs e)
    //{
    //    Page.Validate();

    //    if (Page.IsValid)
    //    {
    //        bool result = SaveListDetails();

    //        Response.Redirect(String.Format("Default.aspx?cfm=1", (result ? "1" : "0")));
    //    }
    //}

    /// <summary>
    /// List selected index changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ListDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        IncomeDetailsRadGrid.Rebind();
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
        if (e.Argument.ToString() == "fromtemp")
        {
            PopulateList();
            CreateButtons();
        }
        else
            IncomeDetailsRadGrid.Rebind();
        //else        
        //    ValuesRadListBox.Items.Add(new RadListBoxItem(e.Argument, (-1).ToString()));
        
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
        //args.IsValid = (ValuesRadListBox.Items.Count > 0);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Populate list types
    /// </summary>
    private void PopulateList()
    {        
        IncomeTemplateDb incomeTemplateDb = new IncomeTemplateDb();
        ListDropDownList.DataSource = incomeTemplateDb.GetIncomeTemplates();
        ListDropDownList.DataTextField = "Name";
        ListDropDownList.DataValueField = "Id";
        ListDropDownList.DataBind();

    }

    /// <summary>
    /// Populate list details
    /// </summary>
    private void PopulateListDetails()
    {
        //IncomeTemplateItemDb incomeTemplateItemDb = new IncomeTemplateItemDb();
        //ValuesRadListBox.DataSource = incomeTemplateItemDb.GetItemsByIncomeTemplateId(int.Parse(!string.IsNullOrEmpty( ListDropDownList.SelectedValue) ? ListDropDownList.SelectedValue : "0"));
        //ValuesRadListBox.DataTextField = "ItemName";
        //ValuesRadListBox.DataValueField = "Id";
        //ValuesRadListBox.DataBind();
    }


    /// <summary>
    /// Get the values from list box
    /// </summary>
    /// <returns></returns>
    //private List<string[]> GetValues()
    //{
    //    List<string[]> keywords = new List<string[]>();

    //    // Loop through list box items
    //    foreach (RadListBoxItem keyword in ValuesRadListBox.Items)
    //    {
    //        string[] data = new string[2];
    //        data[0] = keyword.Value;
    //        data[1] = keyword.Text;

    //        keywords.Add(data);
    //    }

    //    return keywords;
    //}

    /// <summary>
    /// Save the list details
    /// </summary>
    /// <returns></returns>
    //private bool SaveListDetails()
    //{        
    //    IncomeTemplateItemDb incomeTemplateItemDb = new IncomeTemplateItemDb();
    //    ArrayList currentChannelIds = new ArrayList();
    //    ArrayList channelToDelete = new ArrayList();
    //    List<string[]> values = GetValues();     
    //    IncomeTemplateItem.IncomeTemplateItemsDataTable incomeTemplateItems = incomeTemplateItemDb.GetItemsByIncomeTemplateId(int.Parse(ListDropDownList.SelectedValue));

    //    // Get the current channels        

    //    foreach (IncomeTemplateItem.IncomeTemplateItemsRow incomeTemplateItemRow in incomeTemplateItems.Rows)
    //    {
    //        if (!currentChannelIds.Contains(incomeTemplateItemRow.Id))
    //            currentChannelIds.Add(incomeTemplateItemRow.Id);
    //    }

    //    // Get the channels to be deleted
    //    foreach (int currentId in currentChannelIds)
    //    {
    //        bool toBeDeleted = true;

    //        foreach (string[] data in values)
    //        {
    //            int id = int.Parse(data[0]);

    //            if (id == currentId)
    //            {
    //                toBeDeleted = false;
    //                break;
    //            }
    //        }

    //        if (toBeDeleted)
    //        {
    //            if (!channelToDelete.Contains(currentId))
    //                channelToDelete.Add(currentId);
    //        }
    //    }

    //    int itemOrder = 1;

    //    // Get the channels to update
    //    foreach (string[] data in values)
    //    {
    //        int id = int.Parse(data[0]);
    //        string name = data[1];

    //        if (id > 0)
    //        {             
    //            incomeTemplateItemDb.UpdateOrder(id, itemOrder);                
    //        }
    //        else
    //        {                
    //            incomeTemplateItemDb.Insert(int.Parse(ListDropDownList.SelectedValue), name, itemOrder);

    //        }
    //        itemOrder++;
    //    }

    //    // Delete channels
    //    foreach (int id in channelToDelete)
    //    {            
    //        incomeTemplateItemDb.Delete(id);
    //    }


    //    return true;
    //}

    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
        AddRadButton.Visible = hasAccess;
    }
    #endregion


     #region RadGrid NeedDataSource Event
    protected void IncomeDetailsRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        IncomeTemplateItemDb incomeTemplateItemDb = new IncomeTemplateItemDb();
        IncomeDetailsRadGrid.DataSource = incomeTemplateItemDb.GetItemsByIncomeTemplateId(int.Parse(!string.IsNullOrEmpty(ListDropDownList.SelectedValue) ? ListDropDownList.SelectedValue : "0"));
        
    }
    #endregion

    
    protected void IncomeDetailsRadGrid_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            DataRowView data = (DataRowView)e.Item.DataItem;
            HyperLink IncomeItemLabel = (HyperLink)e.Item.FindControl("IncomeItemHyperLink");
            IncomeItemLabel.Text = data["ItemName"].ToString();
            if (hasAccess)
            {
                IncomeItemLabel.Attributes.Add("onclick", string.Format("javascript:ShowWindow('AddValues.aspx?id={0}&mode={1}',{2},{3})",
                    int.Parse(data["Id"].ToString()), "edit", Constants.WindowWidth, Constants.WindowHeight));
                IncomeItemLabel.Attributes.Add("cursor", "pointer");
            }            
            CheckBox GrossIncomeCheckBox = (CheckBox)e.Item.FindControl("GrossIncomeCheckBox");
            if (bool.Parse(!string.IsNullOrEmpty(data["GrossIncome"].ToString()) ? data["GrossIncome"].ToString() : "false"))
                GrossIncomeCheckBox.Checked = true;
            else
                GrossIncomeCheckBox.Checked = false;
            CheckBox AllowanceCheckBox = (CheckBox)e.Item.FindControl("AllowanceCheckBox");
            if (bool.Parse(!string.IsNullOrEmpty(data["Allowance"].ToString()) ? data["Allowance"].ToString() : "false"))
                AllowanceCheckBox.Checked = true;
            else
                AllowanceCheckBox.Checked = false;
            CheckBox OvertimeCheckBox = (CheckBox)e.Item.FindControl("OvertimeCheckBox");
            if (bool.Parse(!string.IsNullOrEmpty(data["Overtime"].ToString()) ? data["Overtime"].ToString() : "false"))
                OvertimeCheckBox.Checked = true;
            else
                OvertimeCheckBox.Checked = false;
            CheckBox AHGIncomeCheckBox = (CheckBox)e.Item.FindControl("AHGIncomeCheckBox");
            if (bool.Parse(!string.IsNullOrEmpty(data["AHGIncome"].ToString()) ? data["AHGIncome"].ToString() : "false"))
                AHGIncomeCheckBox.Checked = true;
            else
                AHGIncomeCheckBox.Checked = false;
            CheckBox CAIncomeCheckBox = (CheckBox)e.Item.FindControl("CAIncomeCheckBox");
            if (bool.Parse(!string.IsNullOrEmpty(data["CAIncome"].ToString()) ? data["CAIncome"].ToString() : "false"))
                CAIncomeCheckBox.Checked = true;
            else
                CAIncomeCheckBox.Checked = false;
        }
    }
}
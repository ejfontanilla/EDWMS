using System;
using System.Text.RegularExpressions;
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

public partial class Verification_SetList : System.Web.UI.Page
{
    //string user;                          Commented by Edward 2016/02/04 take out unused variables
    //bool isAllSetsReadOnlyUser = false;   Commented by Edward 2016/02/04 take out unused variables
    public int? docAppId;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            docAppId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/Completeness/");

        if (!IsPostBack)
        {
            //show address if the user belong to SERS department
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetSection((Guid)Membership.GetUser().ProviderUserKey);
            RadGrid1.Columns.FindByUniqueName("Address").Visible = (sectionId == 3 || sectionId == 4);
            PopulateData();

            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(docAppId.Value);
            DocApp.DocAppRow docAppRow = docApp[0];
            TitleLabel.Text = "All Sets for " + docAppRow.RefType + ": "+ docAppRow.RefNo;
            this.Page.Title += TitleLabel.Text;
        }
    }

    #region No Group
    /// <summary>
    /// Rad grid need data source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        GetData();
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
            //string status = DataBinder.Eval(e.Item.DataItem, "Status").ToString();

            Label AgingLabel = e.Item.FindControl("AgingLabel") as Label;
            Label AddressLabel = new Label();

            DataRowView data = (DataRowView)e.Item.DataItem;

            // Compute the age value
            DateTime dateInDateTime = DateTime.Parse(data["VerificationDateIn"].ToString());
            TimeSpan diff = DateTime.Now.Subtract(dateInDateTime);
            AgingLabel.Text = Format.GetAging(diff);

            dataBoundItem["Address"].Text = string.IsNullOrEmpty(AddressLabel.Text.Trim()) ? "-" : AddressLabel.Text;
        }
    }

    #endregion

    #endregion

    #region Private Methods

    /// <summary>
    /// get Data
    /// </summary>
    private DataTable GetData()
    {
        DocSetDb docSetDb = new DocSetDb();
        return docSetDb.GetvDocSetsByAppId(docAppId.Value);
    }

    /// <summary>
    /// Populate with group grid
    /// </summary>
    private void PopulateData()
    {
        RadGrid1.DataSource = GetData();
        RadGrid1.DataBind();
    }

    #endregion
}

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
    //string user;                              Commented by Edward 2016/02/04 take out unused variables
    //bool isAllSetsReadOnlyUser = false;       Commented by Edward 2016/02/04 take out unused variables
    public int? docSetId;

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
            docSetId = int.Parse(Request["id"]);
        }
        else
            docSetId = -1;

        if (!IsPostBack)
        {
            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSet = docSetDb.GetDocSetById(docSetId.Value);
            DocSet.DocSetRow docSetRow = docSet[0];
            TitleLabel.Text = "Documents for Set: " + docSetRow.SetNo;
            this.Page.Title += TitleLabel.Text;
        }
    }

        /// <summary>
    /// Rad grid need data source
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void RadGrid1_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        DocDb docDb = new DocDb();
        RadGrid1.DataSource = docDb.GetDocByDocSetIdOrderByDocTypeCode(docSetId.Value);
    }

   
    #endregion
}

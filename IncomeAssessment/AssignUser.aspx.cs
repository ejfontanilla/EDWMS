using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Completeness_AssignSet : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the list of set ids
        if (Request["id"] != null)
        {
            DocAppDb docAppDb = new DocAppDb();
            bool success = docAppDb.AssignUserAsAssessmentOfficer(int.Parse(Request["id"]), (Guid)Membership.GetUser().ProviderUserKey);

            if (success)
                Response.Redirect("View.aspx?id=" + int.Parse(Request["id"]));
        }
    }

    #endregion
}
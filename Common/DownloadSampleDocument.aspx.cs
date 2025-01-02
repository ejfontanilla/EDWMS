using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Dwms.Bll;

public partial class Common_DownloadSampleDocument : System.Web.UI.Page
{
    int? id;

    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (String.IsNullOrEmpty(Request["id"]))
            {
                Response.Write("id is null");
                Response.End();
            }

            id = int.Parse(Request["id"]);

            Response.Redirect("SampleDocumentFileHandler.ashx?id=" + id.Value);
        }
        else
        {
            Response.Write(Constants.UnathorizedAccessErrorMessage);
            Response.End();
        }
    }
}
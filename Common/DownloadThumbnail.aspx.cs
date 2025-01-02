using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Dwms.Web;
using Dwms.Bll;
using Ionic.Zip;

public partial class Common_DownloadAppDocument : System.Web.UI.Page
{
    int? id;

    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (String.IsNullOrEmpty(Request["id"]))
            {
                Response.Write("Id is null");
                Response.End();
            }

            id = int.Parse(Request["id"].ToString());

            Response.Redirect("ThumbnailHandler.ashx?id=" + id.Value.ToString());
        }
        else
        {
            Response.Write(Constants.UnathorizedAccessErrorMessage);
            Response.End();
        }
    }
}
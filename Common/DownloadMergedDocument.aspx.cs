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
    string filePath;

    protected void Page_Load(object sender, EventArgs e)
    {
        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            if (String.IsNullOrEmpty(Request["filePath"]))
            {
                Response.Write("file path is null");
                Response.End();
            }

            filePath = Request["filePath"].ToString();

            Response.Redirect("MergedDocumentHandler.ashx?filePath=" + filePath);
        }
        else
        {
            Response.Write(Constants.UnathorizedAccessErrorMessage);
            Response.End();
        }
    }
}
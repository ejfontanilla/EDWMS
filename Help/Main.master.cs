using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Help_Main : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string url = Request.Url.ToString().ToLower();

        if (url.Contains("/help/guides/"))
        {
            GuidesHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/import/"))
        {
            ImportHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/verification/"))
        {
            VerificationHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/completeness/"))
        {
            CompletenessHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/filedoc/"))
        {
            FileDocHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/contact/"))
        {
            ContactHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/login/"))
        {
            LoginHyperLink.CssClass = "on";
        }
        else if (url.Contains("/help/maintenance/"))
        {
            MaintenanceHyperLink.CssClass = "on";
        }
        
    }
}
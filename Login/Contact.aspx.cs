using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.DirectoryServices.AccountManagement;
using Dwms.Bll;
using System.Web.Configuration;

public partial class Login_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AADHyperLink1.NavigateUrl = "mailto:" + Constants.AADContactEmail1;
            AADHyperLink1.Text = Constants.AADContactEmail1;
            AADHyperLink2.NavigateUrl = "mailto:" + Constants.AADContactEmail2;
            AADHyperLink2.Text = Constants.AADContactEmail2;
            SSDHyperLink1.NavigateUrl = "mailto:" + Constants.SSDContactEmail1;
            SSDHyperLink1.Text = Constants.SSDContactEmail1;
            PRDHyperLink1.NavigateUrl = "mailto:" + Constants.PRDContactEmail1;
            PRDHyperLink1.Text = Constants.PRDContactEmail1;
            RSDHyperLink1.NavigateUrl = "mailto:" + Constants.RSDContactEmail1;
            RSDHyperLink1.Text = Constants.RSDContactEmail1;
            RSDHyperLink2.NavigateUrl = "mailto:" + Constants.RSDContactEmail2;
            RSDHyperLink2.Text = Constants.RSDContactEmail2;
        }
    }

    protected void SetLink(HyperLink link)
    {

    }

}

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
using Dwms.Bll;

public partial class Filing_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.FileDoc);

        if (accessControlList.Count > 0)
        {
            if (accessControlList.Contains(AccessControlSettingEnum.View_Only.ToString().ToLower()))
                Response.Redirect("~/FileDoc/ViewOnly.aspx");
            else if (accessControlList.Contains(AccessControlSettingEnum.Download.ToString().ToLower()))
                Response.Redirect("~/FileDoc/Download.aspx");
        }
        else
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}

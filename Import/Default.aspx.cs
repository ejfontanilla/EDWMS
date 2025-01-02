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

public partial class Import_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Import);

        if (accessControlList.Count > 0)
        {
            if (accessControlList.Contains(AccessControlSettingEnum.Scan.ToString().ToLower()))
                Response.Redirect("~/Import/ScanDocuments");
            else if (accessControlList.Contains(AccessControlSettingEnum.Upload.ToString().ToLower()))
                Response.Redirect("~/Import/UploadDocuments");
        }
        else
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}

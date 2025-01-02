using System;
using System.Collections.Generic;
using Dwms.Bll;

public partial class Completeness_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AccessControlDb accessControlDb = new AccessControlDb();

            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness);

            if (accessControlList.Count > 0)
            {
                if (accessControlList.Contains(AccessControlSettingEnum.All_Apps.ToString().ToLower()))
                    Response.Redirect("AllApps.aspx");
                else if (accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()))
                    Response.Redirect("AllAppsReadOnly.aspx");
                else if (accessControlList.Contains(AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()))
                    Response.Redirect("AssignedToMe.aspx");
                else
                    Response.Redirect("AssignmentHle.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}
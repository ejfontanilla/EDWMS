using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;

public partial class IncomeComputation_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Income_Assessment);

        if (accessControlList.Count > 0)
        {
            if (accessControlList.Contains(AccessControlSettingEnum.All_Apps.ToString().ToLower()))
                Response.Redirect("~/IncomeAssessment/AllApps.aspx");
            if (accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()))
                Response.Redirect("~/IncomeAssessment/AllAppsReadOnly.aspx");
            else if (accessControlList.Contains(AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()))
                Response.Redirect("~/IncomeAssessment/AssignedToMe.aspx");
            else
                Response.Redirect("~/IncomeAssessment/AssignmentHle.aspx");
        }
        else
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}

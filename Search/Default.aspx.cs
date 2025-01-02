using System;
using System.Collections.Generic;
using Dwms.Bll;

public partial class Search_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AccessControlDb accessControlDb = new AccessControlDb();

        List<string> accessControlList = new List<string>();
        accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Search);

        if (accessControlList.Count > 0)
        {
            if (accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets.ToString().ToLower()) && !accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets_Read_Only.ToString().ToLower()))
                Response.Redirect("~/Search/SearchAllDepartmentSets");
            else if (accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets_Read_Only.ToString().ToLower()))
                Response.Redirect("~/Search/SearchAllDepartmentSetsReadOnly");
            else if (accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets.ToString().ToLower()) && !accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets_Read_Only.ToString().ToLower()))
                Response.Redirect("~/Search/SearchOwnDepartmentSets");
            else if (accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets_Read_Only.ToString().ToLower()))
                Response.Redirect("~/Search/SearchOwnDepartmentSetsReadOnly");
            else if (accessControlList.Contains(AccessControlSettingEnum.Exception_Report.ToString().ToLower()))
                Response.Redirect("~/Search/ExceptionReport");
        }
        else
        {
            Response.Redirect("~/Default.aspx");
        }
    }
}

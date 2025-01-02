using System;
using System.Collections.Generic;
using Dwms.Bll;

public partial class Verification_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            AccessControlDb accessControlDb = new AccessControlDb();

            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Verification);

            if (accessControlList.Count > 0)
            {
                if (accessControlList.Contains(AccessControlSettingEnum.All_Sets.ToString().ToLower()))
                    Response.Redirect("AllSets.aspx");
                else if (accessControlList.Contains(AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()))
                    Response.Redirect("AllSetsReadOnly.aspx");
                else
                    Response.Redirect("AssignedToMe.aspx");
            }
            else
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}
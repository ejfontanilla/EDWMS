using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class MainNo : System.Web.UI.MasterPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        #region Added by Edward to redirect to new web application 2017/10/05
        string redirectURL = Retrieve.GetDWMSUrl(HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.ApplicationPath,
            HttpContext.Current.Request.Url.Segments, HttpContext.Current.Request.Url.Query);
        if (!redirectURL.Equals("NA"))
            Response.Redirect(redirectURL);
        #endregion
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Prevent cache, works for IE, FireFox and Safari
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();


        //set the user name
        

        Page.ClientScript.RegisterClientScriptInclude("somescript", ResolveUrl("~/Data/JavaScript/tools.js"));
    }


    protected void checkPageAccessRights()
    {
        string url = Request.Url.ToString().ToLower().Trim();

        AccessControlDb accessControlDb = new AccessControlDb();

        //handle import module
        if (url.Contains("/import/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Import);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.Scan.ToString().ToLower()) && url.Contains("/import/scandocuments"))
                    Util.ShowUnauthorizedMessage();
                else if (!accessControlList.Contains(AccessControlSettingEnum.Upload.ToString().ToLower()) && url.Contains("/import/uploaddocuments"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }

        }
        else if (url.Contains("/search/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Search);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets_Read_Only.ToString().ToLower()) && url.Contains("/search/searchalldepartmentsetsreadonly"))
                    Util.ShowUnauthorizedMessage();
                else if ((!accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.Search_All_Department_Sets_Read_Only.ToString().ToLower())) && url.Contains("/search/searchalldepartmentsets/"))
                    Util.ShowUnauthorizedMessage();
                else if (!accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets_Read_Only.ToString().ToLower()) && url.Contains("/search/searchowndepartmentsetsreadonly"))
                    Util.ShowUnauthorizedMessage();
                else if ((!accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.Search_Own_Department_Sets_Read_Only.ToString().ToLower())) && url.Contains("/search/searchowndepartmentsets/"))
                    Util.ShowUnauthorizedMessage();
                else if (!accessControlList.Contains(AccessControlSettingEnum.Exception_Report.ToString().ToLower()) && url.Contains("/search/exceptionreport/"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }

        }
        else if (url.Contains("/verification/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Verification);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower()) && url.Contains("/verification/allsetsreadonly.aspx"))
                    Util.ShowUnauthorizedMessage();

                if ((!accessControlList.Contains(AccessControlSettingEnum.All_Sets.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.All_Sets_Read_Only.ToString().ToLower())) && url.Contains("/verification/allsets.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Imported_By_Me.ToString().ToLower()) && url.Contains("/verification/importedbyme.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()) && url.Contains("/verification/assignedtome.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()) && url.Contains("/verification/pendingassignment.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assignment_Report.ToString().ToLower()) && url.Contains("/verification/assignmentreport.aspx"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }

        }
        else if (url.Contains("/completeness/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()) && url.Contains("/completeness/allappsreadonly.aspx"))
                    Util.ShowUnauthorizedMessage();

                if ((!accessControlList.Contains(AccessControlSettingEnum.All_Apps.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower())) && url.Contains("/completeness/allapps.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()) && url.Contains("/completeness/assignedtome.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()) && url.Contains("/completeness/pendingassignment.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Batch_Assignment.ToString().ToLower()) && url.Contains("/completeness/assignmenthle.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assignment_Report.ToString().ToLower().Trim()) && url.Contains("/completeness/assignmentreport.aspx"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }
        }
        else if (url.Contains("/incomeassessment/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Income_Assessment);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower()) && url.Contains("/incomeassessment/allappsreadonly.aspx"))
                    Util.ShowUnauthorizedMessage();

                if ((!accessControlList.Contains(AccessControlSettingEnum.All_Apps.ToString().ToLower()) || accessControlList.Contains(AccessControlSettingEnum.All_Apps_Read_Only.ToString().ToLower())) && url.Contains("/incomeassessment/allapps.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assigned_To_Me.ToString().ToLower()) && url.Contains("/incomeassessment/assignedtome.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Pending_Assignment.ToString().ToLower()) && url.Contains("/incomeassessment/pendingassignment.aspx"))
                    Util.ShowUnauthorizedMessage();

                if (!accessControlList.Contains(AccessControlSettingEnum.Assignment_Report.ToString().ToLower()) && url.Contains("/incomeassessment/assignmentreport.aspx"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }
        }
        else if (url.Contains("/filedoc/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.FileDoc);

            if (accessControlList.Count > 0)
            {
                if (!accessControlList.Contains(AccessControlSettingEnum.View_Only.ToString().ToLower()) && url.Contains("/filedoc/viewonly"))
                    Util.ShowUnauthorizedMessage();
                else if (!accessControlList.Contains(AccessControlSettingEnum.Download.ToString().ToLower()) && url.Contains("/filedoc/download"))
                    Util.ShowUnauthorizedMessage();
            }
            else
            {
                Util.ShowUnauthorizedMessage();
            }

        }
        else if (url.Contains("/maintenance/"))
        {
            List<string> accessControlList = new List<string>();
            accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Maintenance);

            if (accessControlList.Count == 0)
                Util.ShowUnauthorizedMessage();
        }
    }
}

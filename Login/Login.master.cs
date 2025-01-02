using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Web;
using Dwms.Bll;

public partial class Login : System.Web.UI.MasterPage
{
    protected void RadMenu1_PreRender(object sender, EventArgs e)
    {
        RadMenu radMenu = sender as RadMenu;

        foreach (RadMenuItem rootItem in radMenu.Items)
        {
            rootItem.Attributes.Add("style", "cursor:pointer");// for root tems  

            foreach (RadMenuItem childItem in rootItem.Items)
            {
                childItem.Attributes.Add("style", "cursor:pointer");//for child items  
            }
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string url = Request.Url.ToString().ToLower();
        System.Drawing.Color color = System.Drawing.Color.White;

        #region Added by Edward to redirect to new web application 2017/10/05        
                                
        string redirectURL = Retrieve.GetDWMSUrl(HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.ApplicationPath,
            HttpContext.Current.Request.Url.Segments, HttpContext.Current.Request.Url.Query);
        if (!redirectURL.Equals("NA"))
            Response.Redirect(redirectURL);
        
        #endregion

        //if (url.Contains("/search/"))
        //{
        //    Search.CssClass = "on";
        //}
        //else if (url.Contains("/translations/"))
        //{
        //    Translations.CssClass = "on";
        //}
        //else if (url.Contains("/categories/"))
        //{
        //    Categories.CssClass = "on";
        //}
        //else if (url.Contains("/sources/"))
        //{
        //    Sources.CssClass = "on";
        //}
        //else if (url.Contains("/upload/"))
        //{
        //    Upload.CssClass = "on";
        //}

        Page.ClientScript.RegisterClientScriptInclude("somescript", ResolveUrl("~/Data/JavaScript/tools.js"));
    }
}

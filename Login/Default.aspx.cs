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
            FormsAuthentication.SignOut();
            UserNameTextBox.Focus();
        }
    }

    protected void DoLogin(object sender, EventArgs e)
    {
        Page.Validate();
        if (!Page.IsValid)
            return;

        string userName = UserNameTextBox.Text.ToUpper().Trim();
        string passWord = PasswordTextBox.Text.Trim();

        MembershipUser user = Membership.GetUser(userName);

        if (user != null)
        {
            ParameterDb parameterDb = new ParameterDb();
            //string adServer = "ABCDEaddev.ABCDE.com.ph"; // Replace this for DWMS
            string adServer = "ABCDEadprod.ABCDE.com.ph"; // Replace this for DWMS
            //string adServer = "localhost"; // Replace this for DWMS
            bool isValid = false;

            // Authenticate the user: Local OR Active Directory
            if (parameterDb.GetParameter(ParameterNameEnum.AuthenticationMode).Trim().Equals(AuthenticationModeEnum.Local.ToString()))
                isValid = Membership.ValidateUser(userName, passWord);
            else
                isValid = GetADLoginResult(adServer, userName, passWord);

            if (isValid)
            {
                // Getting the authentication section of the web.config file
                // Source: http://sandblogaspnet.blogspot.com/2008/09/tags-in-webconfig.html
                AuthenticationSection authenticationSection = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

                // Retrieving the minutes from the timeout attribute of forms tag of web.config
                int timeOut = authenticationSection.Forms.Timeout.Minutes;

                // Generate the folder name that will be used to save the scanned images (when needed)
                //string sessionFolderName = user.ProviderUserKey.ToString().ToLower() + "_" + Guid.NewGuid().ToString().ToLower();
                string sessionFolderName = user.ProviderUserKey.ToString().ToLower();

                // Set the authentication cookie.  This is the cookie that will be used by the scanning module when saving the image
                // handle remember me http://forums.asp.net/t/1518081.aspx/1
                // if the remember me option is selected, set the cookie expiry to 1 year. if not, follow the timeout value from forms authentication.
                FormsAuthenticationTicket formsAuthTix = new FormsAuthenticationTicket(1, userName, DateTime.Now, 
                    //RememberMeCheckBox.Checked ? DateTime.Now.AddYears(timeOut) : DateTime.Now.AddMinutes(timeOut), --Deleted on 29 Apr 2014 by Edwin
                    DateTime.Now.AddYears(1),   //Added on 29 Apr 2014 by Edwin to change cookie timout from 30 mins to 1 year (to solve scanner error)
                    RememberMeCheckBox.Checked, sessionFolderName);
                string cookieStr = FormsAuthentication.Encrypt(formsAuthTix);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieStr);
                cookie.Expires = formsAuthTix.Expiration;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                Response.Cookies.Add(cookie);
                //FormsAuthentication.RedirectFromLoginPage(userName, RememberMeCheckBox.Checked); 

                if (Request["ReturnUrl"] != null)
                {
                    //FormsAuthentication.RedirectFromLogin wPage(userName, false);
                    Response.Redirect(Request["ReturnUrl"]);
                }
                else
                {
                    //FormsAuthentication.SetAuthCookie(userName, false);

                    //redirect to the respective access page
                    AccessControlDb accessControlDb = new AccessControlDb();
                    List<string> accessControlList = new List<string>();

                    AccessControlDb accessControlDb1 = new AccessControlDb();
                    Guid? userId = (Guid)user.ProviderUserKey;

                    accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Verification, userId.Value);

                    if (accessControlList.Count > 0)
                    {
                        Response.Redirect("~/Verification/");
                    }

                    accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Completeness, userId.Value);
                    if (accessControlList.Count > 0)
                    {
                        Response.Redirect("~/Completeness/");
                    }

                    accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Search, userId.Value);
                    if (accessControlList.Count > 0)
                    {
                        Response.Redirect("~/Search/");
                    }

                    accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Import, userId.Value);

                    if (accessControlList.Count > 0)
                    {
                        Response.Redirect("~/Import/");
                    }

                    accessControlList = accessControlDb.GetUserAccessControlList(ModuleNameEnum.Maintenance, userId.Value);
                    if (accessControlList.Count > 0)
                    {
                        Response.Redirect("~/Maintenance/");
                    }
                    else
                    {
                        Response.Redirect("~/EmptyAccess/");
                    }
                }                
            }
            else
                ErrorPanel.Visible = true;
        }
        else
            ErrorPanel.Visible = true;        
    }

    private bool GetADLoginResult(string adServer, string userName, string passWord)
    {
        bool isValid = false;

        // AdServer- the server name or IP address , "192.168.1.111", "HIEND_Server001"
        // MICA AdServer - "MITAHQ" or "10.23.1.241"
        try
        {
            PrincipalContext pc = new PrincipalContext(ContextType.Domain, adServer);
            isValid = pc.ValidateCredentials(userName, passWord);
        }
        catch (Exception)
        {
            //errorMessage = "Fail to Login, Reason:" + ex.Message;
        }

        return isValid;
    }
}

<%@ Application Language="C#" %>
<%@ Import Namespace="Dwms.Bll" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        // Code that runs when an unhandled error occurs
        Exception error = Server.GetLastError().GetBaseException();

        if (error == null)
        {
            return;
        }

        //Cps2.Bll.EmailManager.SendErrorNotificationEmail("Hiend Software", "info@hiend.com", "minthant.sin@hiend.com", String.Empty, String.Empty, "CPS II - Error Notification", "There is an error in the page.");

        // Log it in the database
        LogErrorInDatabase(error);


            // Email notification
            NotifyErrorByEmail(error);
        

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }


    public static void NotifyErrorByEmail(Exception error)
    {
        MembershipUser user = Membership.GetUser();
        string userName = String.Empty;

        if (user != null)
        {
            userName = user.UserName;
        }

        // Construct email content        

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("<html><head><title></title>");
        builder.AppendLine("</head><body>");
        builder.AppendLine("<div style='padding: 2px;'>");
        builder.AppendLine("<table cellpadding='0' cellspacing='0' width='100%' border='1'>");

        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "Message"));
        builder.AppendLine(String.Format("<td>{0}</td>", error.Message));
        builder.AppendLine("</tr>");

        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "Login ID"));
        builder.AppendLine(String.Format("<td>{0}</td>", userName));
        builder.AppendLine("</tr>");

        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "URL"));
        builder.AppendLine(String.Format("<td>{0}</td>", HttpContext.Current.Request.Url.AbsoluteUri));
        builder.AppendLine("</tr>");
        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "Source"));
        builder.AppendLine(String.Format("<td>{0}</td>", error.Source));
        builder.AppendLine("</tr>");
        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "Target Site"));
        builder.AppendLine(String.Format("<td>{0}</td>", error.TargetSite));
        builder.AppendLine("</tr>");

        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td nowrap='nowrap'>{0}</td>", "Date Time"));
        builder.AppendLine(String.Format("<td>{0}</td>", DateTime.Now.ToString("yyyy-MMM-dd hh:mm:ss tt")));
        builder.AppendLine("</tr>");


        builder.AppendLine("<tr>");
        builder.AppendLine(String.Format("<td>{0}</td>", "Detail"));
        builder.AppendLine(String.Format("<td>{0} Inner Exception: {1}</td>", error.ToString(), error.InnerException));
        builder.AppendLine("</tr>");

        builder.AppendLine("</table>");
        builder.AppendLine("</div></body></html>");


        string senderName = "Hiend Software Pte Ltd";
        string senderEmail = "Lay_Har_OH/EAPG/ABCDE/SG@ABCDE.com.ph";
        string recipientEmail = "dwms@hiend.com";
        string ccEmail = String.Empty;
        string replyToEmail = String.Empty;
        string subject = "DWMS - Error Notification";
        string message = builder.ToString();

        EmailTemplateDb.SendErrorNotificationEmail(senderName, senderEmail, recipientEmail, ccEmail, replyToEmail, subject, message);
    }


    private void LogErrorInDatabase(Exception error)
    {
        try
        {
            MembershipUser user = Membership.GetUser();
            Guid userId = Guid.Empty;

            if (user != null)
            {
                userId = (Guid)user.ProviderUserKey;
            }


            ErrorLogDb log = new ErrorLogDb();
            log.Insert("Global.asax", error.Message + " " + HttpContext.Current.Request.Url.AbsoluteUri + ", Inner Exception: " + error.InnerException + ", StackTrace:" + error.StackTrace);
        }
        catch (Exception ex)
        {
            // Catch and ignore to allow other tasks to continue
        }
    }
</script>

using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using ErrorLogTableAdapters;
using Dwms.Dal;
using System.Text;
namespace Dwms.Bll
{
    public class ErrorLogDb
    {
        private ErrorLogTableAdapter _ErrorLogAdapter = null;

        protected ErrorLogTableAdapter Adapter
        {
            get
            {
                if (_ErrorLogAdapter == null)
                    _ErrorLogAdapter = new ErrorLogTableAdapter();

                return _ErrorLogAdapter;
            }
        }

        #region Insert Methods

        /// <summary>
        /// Insert ErrorLog
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Insert(ErrorLogFunctionName functionName, string message)
        {
            ErrorLog.ErrorLogDataTable dt = new ErrorLog.ErrorLogDataTable();
            ErrorLog.ErrorLogRow r = dt.NewErrorLogRow();

            r.FunctionName = functionName.ToString();
            r.Message = message;
            r.Date = DateTime.Now;

            dt.AddErrorLogRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        //Added by Edward 26.10.2013
        public int Insert(string functionName, string message)
        {
            ErrorLog.ErrorLogDataTable dt = new ErrorLog.ErrorLogDataTable();
            ErrorLog.ErrorLogRow r = dt.NewErrorLogRow();

            r.FunctionName = functionName;
            r.Message = message;
            r.Date = DateTime.Now;

            dt.AddErrorLogRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        #endregion

        #region Added by Edward 2015/8/26 Reduce Error Notifications and OOM
        public static void NotifyErrorByEmail(Exception error, string addInfo)
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
            builder.AppendLine(String.Format("<td>{0}</td>", "Captured Message"));
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
            builder.AppendLine(String.Format("<td>{0}<br>{1}</td>", error.TargetSite,addInfo));
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
        #endregion

    }

    #region Added By edward 2014/10/10 for Error Custom page
    public static class ApplicationErrorDb
    {
        public static string ErrorMessage { get; set; }
        public static string ErrorSource { get; set; }
        public static string ErrorStackStrace { get; set; }        
    }
    #endregion

}
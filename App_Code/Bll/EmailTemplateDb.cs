using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using EmailTemplateTableAdapters;
using System.Net.Mail;

namespace Dwms.Bll
{
    public class EmailTemplateDb
    {
        private EmailTemplateTableAdapter _EmailTemplateAdapter = null;

        protected EmailTemplateTableAdapter Adapter
        {
            get
            {
                if (_EmailTemplateAdapter == null)
                    _EmailTemplateAdapter = new EmailTemplateTableAdapter();

                return _EmailTemplateAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all the email templates
        /// </summary>
        /// <returns></returns>
        public EmailTemplate.EmailTemplateDataTable GetEmailTemplate()
        {
            return Adapter.GetEmailTemplates();
        }

        /// <summary>
        /// Get email template by template code
        /// </summary>
        /// <param name="emailTemplateCode"></param>
        /// <returns></returns>
        public EmailTemplate.EmailTemplateDataTable GetEmailTemplate(EmailTemplateCodeEnum emailTemplateCode)
        {
            return Adapter.GetEmailTemplateByCode(emailTemplateCode.ToString());
        }

        /// <summary>
        /// Get email template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmailTemplate.EmailTemplateDataTable GetEmailTemplate(int id)
        {
            return Adapter.GetEmailTemplateById(id);
        }
        #endregion        

        #region Insert Methods
        /// <summary>
        /// Insert email template
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int Insert(string templateCode, string templateDescription, string subject, string content)
        {
            EmailTemplate.EmailTemplateDataTable dt = new EmailTemplate.EmailTemplateDataTable();
            EmailTemplate.EmailTemplateRow r = dt.NewEmailTemplateRow();

            r.EmailTemplateCode = templateCode;
            r.TemplateDescription = templateDescription;
            r.Subject = subject;
            r.Content = content;

            dt.AddEmailTemplateRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Update the email template
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Update(int id, string templateCode, string templateDescription, string subject, string content)
        {
            EmailTemplate.EmailTemplateDataTable dt = GetEmailTemplate(id);

            if (dt.Count == 0) return false;

            EmailTemplate.EmailTemplateRow r = dt[0];

            if(!String.IsNullOrEmpty(templateCode))
                r.EmailTemplateCode = templateCode;

            r.TemplateDescription = templateDescription;
            r.Subject = subject;
            r.Content = content;

            int rowsAffected = Adapter.Update(dt);
            
            return (rowsAffected > 0);
        }
        #endregion


        public static bool SendErrorNotificationEmail(string senderName, string senderEmail,
           string recipientEmail, string ccEmail, string replyToEmail, string subject, string message)
        {
            char[] delimiterChars = { ';', ',', ':' };
            bool sent = false;
            string from = String.Format("{0} <{1}>", senderName, senderEmail);
            string cr = Environment.NewLine;

            message = message.Replace(cr, "<br />");
            message = message.Replace("\n", "<br />");

            try
            {
                MailMessage m = new MailMessage();

                m.From = new MailAddress(from);

                //// To
                //string[] toEmails = recipientEmail.Split(delimiterChars);

                //foreach (string s in toEmails)
                //{
                //    string email = s.Trim();

                //    if (!String.IsNullOrEmpty(email) && Validation.IsEmail(email))
                //    {
                //        m.To.Add(new MailAddress(email));
                //    }
                //}

                //// CC
                //if (!String.IsNullOrEmpty(ccEmail))
                //{
                //    string[] ccEmails = ccEmail.Split(delimiterChars);

                //    foreach (string s in ccEmails)
                //    {
                //        string email = s.Trim();

                //        if (!String.IsNullOrEmpty(email) && Validation.IsEmail(email))
                //        {
                //            m.CC.Add(new MailAddress(email));
                //        }
                //    }
                //}

                m.To.Add(new MailAddress(recipientEmail));
                m.IsBodyHtml = true;
                m.Subject = subject;
                m.Body = message;

                SmtpClient client = new SmtpClient();
                client.Send(m);
                sent = true;
            }
            catch (Exception ex)
            {
                sent = false;
            }

            return sent;
        }
    }
}
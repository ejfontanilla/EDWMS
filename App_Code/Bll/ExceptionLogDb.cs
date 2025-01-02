using System;
using System.Collections.Generic;
using System.Web.Security;
using ExceptionLogTableAdapters;
using System.Data;
using Dwms.Dal;
using Dwms.Web;

namespace Dwms.Bll
{
    public class ExceptionLogDb
    {
        private ExceptionLogTableAdapter _ExceptionLogAdapter = null;

        protected ExceptionLogTableAdapter Adapter
        {
            get
            {
                if (_ExceptionLogAdapter == null)
                    _ExceptionLogAdapter = new ExceptionLogTableAdapter();

                return _ExceptionLogAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get all the exception data
        /// </summary>
        /// <returns></returns>
        public ExceptionLog.ExceptionLogDataTable GetExceptionLog()
        {
            return Adapter.GetData();
        }        

        public DataTable GetExceptionLogForExceptionReport(string channel, string refNo, DateTime? fromDate, DateTime? toDate)
        {
            return ExceptionLogDs.GetLogForExceptionReport(channel, refNo, fromDate, toDate);
        }
        #endregion        

        #region Insert Methods
        public int Insert(string channel, string refNo, DateTime date, string reason, string errorMessage, string sourceName, bool sendEmail)
        {
            ExceptionLog.ExceptionLogDataTable dt = new ExceptionLog.ExceptionLogDataTable();
            ExceptionLog.ExceptionLogRow r = dt.NewExceptionLogRow();

            r.Channel = channel;
            r.RefNo = refNo;
            r.DateOccurred = date;
            r.Reason = reason;
            r.ErrorMessage = errorMessage;

            dt.AddExceptionLogRow(r);
            Adapter.Update(dt);
            int rowAffected = r.Id;

            if (rowAffected > 0)
            {
                ParameterDb parameterDb = new ParameterDb();

                // Send email notifications
                string recipient = string.Empty;
                string subject = string.Empty;
                string message = string.Empty;

                string systemEmail = parameterDb.GetParameter(ParameterNameEnum.SystemEmail);
                string systemName = parameterDb.GetParameter(ParameterNameEnum.SystemName);

                if (!String.IsNullOrEmpty(refNo))
                {
                    string refType = Util.GetReferenceType(refNo);

                    string departmentCode = string.Empty;

                    if (refType.Equals(ReferenceTypeEnum.HLE.ToString()))
                        departmentCode = DepartmentCodeEnum.AAD.ToString();
                    else if (refType.Equals(ReferenceTypeEnum.RESALE.ToString()))
                        departmentCode = DepartmentCodeEnum.RSD.ToString();
                    else if (refType.Equals(ReferenceTypeEnum.SALES.ToString()))
                        departmentCode = DepartmentCodeEnum.SSD.ToString();
                    else if (refType.Equals(ReferenceTypeEnum.SERS.ToString()))
                        departmentCode = DepartmentCodeEnum.PRD.ToString();

                    if (!String.IsNullOrEmpty(departmentCode))
                    {
                        DepartmentDb departmentDb = new DepartmentDb();
                        recipient = departmentDb.GetDepartmentMailingList((DepartmentCodeEnum)Enum.Parse(typeof(DepartmentCodeEnum), departmentCode));
                    }
                    else
                    {
                        recipient = parameterDb.GetParameter(ParameterNameEnum.ErrorNotificationMailingList);
                    }
                }
                else
                {
                    recipient = parameterDb.GetParameter(ParameterNameEnum.ErrorNotificationMailingList);
                }

                if (!String.IsNullOrEmpty(recipient) && sendEmail)
                {
                    // Compose the email subject and body
                    EmailTemplateDb emailTemplateDb = new EmailTemplateDb();
                    EmailTemplate.EmailTemplateDataTable emailTemplateDt = emailTemplateDb.GetEmailTemplate(EmailTemplateCodeEnum.Exception_Log_Template);

                    if (emailTemplateDt.Rows.Count > 0)
                    {
                        string content = string.Empty;
                        content += String.Format("Error: {0}", reason) + Environment.NewLine;
                        content += String.Format("Details: {0}", errorMessage) + Environment.NewLine;

                        EmailTemplate.EmailTemplateRow emailTemplate = emailTemplateDt[0];
                        subject = emailTemplate.Subject.Replace("[" + EmailTemplateVariablesEnum.Source.ToString() + "]", sourceName);
                        message = emailTemplate.Content;
                        message = message.Replace("[" + EmailTemplateVariablesEnum.Remark.ToString() + "]", content);
                        message = message.Replace("[" + EmailTemplateVariablesEnum.SystemName.ToString() + "]", systemName);
                    }

                    Util.SendMail(systemName, systemEmail, recipient, string.Empty, string.Empty, subject, message);
                }
            }

            return rowAffected;
        }
        #endregion

        #region Added By Edward Development of Reports 2014/06/09
        public static DataTable GetErrorSendingToCDB(int section, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            return ExceptionLogDs.GetErrorSendingToCDB(section, department, dateInFrom, dateInTo);
        }

        public static DataTable GetOCRWebServiceError(int section, int department)
        {
            return ExceptionLogDs.GetOCRWebServiceError(section, department);
        }

        public static DataTable GetOCRWebServiceError(int section, int department, int month, int year)
        {
            return ExceptionLogDs.GetOCRWebServiceError(section, department, month, year);
        }

        public static DataTable GetOCRWebServiceMonthYear(int section, int department)
        {
            return ExceptionLogDs.GetOCRWebServiceMonthYear(section, department);
        }

        public static int GetOCRWebCount(int section, int department, int month, int year, string reason)
        {
            return ExceptionLogDs.GetOCRWebCount(section, department, month, year, reason);
        }
        #endregion
    }
}
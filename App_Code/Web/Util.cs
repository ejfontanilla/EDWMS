using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Net.Mail;
using System.Xml;
using System.IO;
using System.Collections;
using System.Net.Mime;
using System.Text;
using Dwms.Bll;
using WebSupergoo.ABCpdf8;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Dwms.Dal;
using Telerik.Web.UI;
using System.Net;
using System.Drawing.Imaging;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System.Text.RegularExpressions;
using Cyotek.GhostScript.PdfConversion;
using NHunspell;
using Ionic.Zip;

namespace Dwms.Web
{
    public sealed class Util
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        public static bool SendMail(string senderName, string senderEmail,
            string recipientEmail, string ccEmail, string replyToEmail,
            string subject, string message)
        {
            ParameterDb parameterDb = new ParameterDb();

            char[] delimiterChars = { ';', ',', ':' };
            bool sent = false;
            string from = String.Format("{0} <{1}>", senderName, senderEmail);

            string hostUrl = GetHostUrl();
            string cr = Environment.NewLine;            

            message = message.Replace(cr, "<br />");
            message = message.Replace("\n\r", "<br />");
            message = message.Replace("\r\n", "<br />");
            message = message.Replace("\n", "<br />");
            message = message.Replace("\r", "<br />");

            if (parameterDb.GetParameter(ParameterNameEnum.RedirectAllEmailsToTestMailingList).Trim().ToUpper() == "YES")
            {
                subject = subject + " (UAT Email)";
                message += cr + cr + "<br /><br />-----<br /><br />This is a UAT email from " + Util.GetHostUrl() + ". The original recipients are: " 
                    + recipientEmail;

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    message = message + ", CC: " + ccEmail;
                }

                //recipientEmail = "lexin.pan@hiend.com;wintwah.toe@hiend.com;peter.zhou@hiend.com;matthew.narca@hiend.com;ns.subashini@hiend.com";
                recipientEmail = parameterDb.GetParameter(ParameterNameEnum.TestMailingList).Trim();
                ccEmail = string.Empty;
            }

            try
            {
                MailMessage m = new MailMessage();

                // From
                m.From = new MailAddress(from);

                // To
                string[] toEmails = recipientEmail.Split(delimiterChars);
                foreach (string s in toEmails)
                    if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                        m.To.Add(new MailAddress(s.Trim()));

                // CC
                if (ccEmail != null && ccEmail.Trim() != string.Empty)
                {
                    string[] ccEmails = ccEmail.Split(delimiterChars);
                    foreach (string s in ccEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.CC.Add(new MailAddress(s.Trim()));
                }

                // Reply
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    m.ReplyTo = new MailAddress(replyToEmail);
                }

                message = FormatBody(message);

                m.IsBodyHtml = true;
                m.Subject = subject;
                m.Body = message;
                SmtpClient client = new SmtpClient();

                //added by Sandeep 2012-07-20, can be removed once IIS can send the emails
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //

                client.Send(m);
                sent = true;
            }
            catch (Exception ex)
            {
                // Consider customizing the message for the EmailNotSentPanel in the ShowAds page.
                //HttpContext.Current.Response.Write(ex.Message);
                sent = false;
            }
            return sent;
        }
       
        public static bool SendMail(string senderName, string senderEmail,
            string recipientEmail, string ccEmail, string bCCEmail, string replyToEmail,
            string subject, string message, string attachments)
        {
            ParameterDb parameterDb = new ParameterDb();

            char[] delimiterChars = { ';', ',', ':' };
            bool sent = false;
            string from = String.Format("{0} <{1}>", senderName, senderEmail);

            string hostUrl = GetHostUrl();
            string cr = Environment.NewLine;            

            message = message.Replace(cr, "<br />");
            message = message.Replace("\n\r", "<br />");
            message = message.Replace("\r\n", "<br />");
            message = message.Replace("\n", "<br />");
            message = message.Replace("\r", "<br />");

            if (parameterDb.GetParameter(ParameterNameEnum.RedirectAllEmailsToTestMailingList).Trim().ToUpper() == "YES")
            {
                subject = subject + " (UAT Email)";
                message += cr + cr + "<br /><br />-----<br /><br />This is a UAT email from " + Util.GetHostUrl() + ". The original recipients are: " 
                    + recipientEmail;

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    message = message + ", CC: " + ccEmail;
                }

                //recipientEmail = "lexin.pan@hiend.com;wintwah.toe@hiend.com;peter.zhou@hiend.com;matthew.narca@hiend.com;ns.subashini@hiend.com";
                recipientEmail = parameterDb.GetParameter(ParameterNameEnum.TestMailingList).Trim();
                ccEmail = string.Empty;
            }

            try
            {
                MailMessage m = new MailMessage();

                // From
                m.From = new MailAddress(from);

                // To
                string[] toEmails = recipientEmail.Split(delimiterChars);
                foreach (string s in toEmails)
                    if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                        m.To.Add(new MailAddress(s.Trim()));

                // CC
                if (ccEmail != null && ccEmail.Trim() != string.Empty)
                {
                    string[] ccEmails = ccEmail.Split(delimiterChars);
                    foreach (string s in ccEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.CC.Add(new MailAddress(s.Trim()));
                }

                // BCC

                if (bCCEmail != null && bCCEmail.Trim() != string.Empty)
                {
                    string[] bCCEmails = bCCEmail.Split(delimiterChars);
                    foreach (string s in bCCEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.Bcc.Add(new MailAddress(s.Trim()));
                }

                // Reply
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    m.ReplyTo = new MailAddress(replyToEmail);
                }

                //Attachment

                m.Attachments.Clear();

                if (attachments != null && attachments.Trim() != string.Empty)
                {
                    string[] attachmentArray = attachments.Split(';');
                    foreach (string s in attachmentArray)
                        if (s != null && s.Trim() != string.Empty)
                        {
                            if (File.Exists(s))
                            {
                                Attachment attached = new Attachment(s);
                                m.Attachments.Add(attached);
                            }
                        }
                }

                message = FormatBody(message);

                m.IsBodyHtml = true;
                m.Subject = subject;
                m.Body = message;
                SmtpClient client = new SmtpClient();
                client.Timeout = 360000;
                client.Send(m);
                sent = true;
                m.Dispose();
            }
            catch (Exception ex)
            {
                sent = false;
            }
            finally
            {
                if (attachments != null && attachments.Trim() != string.Empty)
                {
                    string[] attachmentArray = attachments.Split(';');
                    foreach (string s in attachmentArray)
                        if (s != null && s.Trim() != string.Empty)
                        {
                            try
                            {
                                File.Delete(s);
                            }
                            catch (Exception)
                            {
                            }
                        }
                }
            }

            return sent;
        }

        private static string FormatBody(string message)
        {
            //return "<p style=\"Arial, Helvetica, sans-serif; font-size: 12px;\">" + message + "</p>";
            return "<span style=\"font-family: arial,sans-serif; font-size: 10pt;\">" + message + "</span>";
        }

        public static string GetFixedFooter()
        {
            #region Modified by Edward Use StringBuilder rather than string in Reduce Error Notification and Improve Out Of memory 2015/8/24
            StringBuilder s = new StringBuilder();
            string cr = Environment.NewLine;
            s.Append(cr + "========================================");
            s.Append(cr + "THIS IS A SYSTEM GENERATED MESSAGE");
            s.Append(cr + "PLEASE DO NOT REPLY TO THIS EMAIL");
            s.Append(cr + "NO REPLY WILL BE ACTED UPON");
            s.Append(cr + "========================================");
            
            //string s = string.Empty;
            //string cr = Environment.NewLine;
            //s += cr + "========================================";
            //s += cr + "THIS IS A SYSTEM GENERATED MESSAGE";
            //s += cr + "PLEASE DO NOT REPLY TO THIS EMAIL";
            //s += cr + "NO REPLY WILL BE ACTED UPON";
            //s += cr + "========================================";
            return s.ToString();
            #endregion
        }

        public static bool SendBatchNotificationEmail(string subject, string emailContent)
        {
            ParameterDb paraDb = new ParameterDb();
            string senderName = paraDb.GetParameter(ParameterNameEnum.SenderName);
            string senderEmail = paraDb.GetParameter(ParameterNameEnum.SystemEmail);
            string batchEmailList = paraDb.GetParameter(ParameterNameEnum.BatchJobMailingList).Trim();
            emailContent += GetFixedFooter();
            return SendMail(senderName, senderEmail, batchEmailList, string.Empty, string.Empty, subject, emailContent);
        }

        public static void AuthenticateUser()
        {
            MembershipUser user = Membership.GetUser();

            if (user == null)
            {
                string returnUrl = HttpContext.Current.Request.Url.ToString();
                HttpContext.Current.Response.Redirect("Login.aspx?ReturnUrl=" + returnUrl);
            }
        }

        public static string GetHostUrl()
        {
            string url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath;
            if(url.EndsWith("/"))
            {
                url=url.Substring(0,url.Length-1);
            }
            //MOE launch http://localhost:82 for autosys job of SendMail reminder
            string dbInstance = GetDatabaseServer();
            if (dbInstance.StartsWith("VMNTDP01"))
            {
                url = "http://olts.moe.com.ph";
            }
            else if (dbInstance.StartsWith("VMNTAU01"))
            {
                url = "http://u-olts.moe.com.ph";
            }

            return url.ToLower();
        }

        private static string GetDatabaseServer()
        {
            string dbInstance = string.Empty;
            try
            {
                System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(connString);
                dbInstance = cn.DataSource;
            }
            catch (Exception ex)
            {
                dbInstance = string.Empty;
            }
            return dbInstance.Trim().ToUpper();
        }

        public static string SaveAsImage(string filePath)
        {
            string imagePath = string.Empty;

            WebSupergoo.ABCpdf8.Doc doc = new WebSupergoo.ABCpdf8.Doc();
            doc.Read(filePath);

            // set up the rendering parameters
            doc.Rendering.ColorSpace = XRendering.ColorSpaceType.Gray;
            doc.Rendering.SaveCompression = XRendering.Compression.LZW;
            doc.Rendering.BitsPerChannel = 8;

            long fileSize = 0;
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Exists)
            {
                // Get file size in KB
                fileSize = fileInfo.Length / 1024;
            }

            // Set DPI based on file size
            if (fileSize <= 256)
            {
                doc.Rendering.DotsPerInch = 600;
            }
            else if (fileSize > 256 && fileSize <= 512)
            {
                doc.Rendering.DotsPerInch = 300;
            }
            else
            {
                doc.Rendering.DotsPerInch = 200;
            }

            doc.PageNumber = 1;
            doc.Rect.String = doc.CropBox.String;
            doc.Rendering.SaveAppend = false;
            //doc.SetInfo(0, "ImageCompression", "4");
            
            imagePath = filePath + "_.tiff";           
            doc.Rendering.Save(imagePath);

            //// loop through the pages
            //int n = doc.PageCount;

            //for (int i = 1; i <= n; i++)
            //{
            //    doc.PageNumber = i;
            //    doc.Rect.String = doc.CropBox.String;
            //    doc.Rendering.SaveAppend = (i != 1);
            //    doc.SetInfo(0, "ImageCompression", "4");
            //    doc.Rendering.Save(Server.MapPath(Guid.NewGuid().ToString().ToUpper().Substring(0, 10) + ".png"));
            //}

            doc.Clear();

            return imagePath;
        }

        public static string SaveAsImage_TestMultiThread(string filePath)
        {
            string imagePath = string.Empty;

            WebSupergoo.ABCpdf8.Doc doc = new WebSupergoo.ABCpdf8.Doc();
            doc.Read(filePath);

            // set up the rendering parameters
            doc.Rendering.ColorSpace = XRendering.ColorSpaceType.Gray;
            doc.Rendering.BitsPerChannel = 1;
            doc.Rendering.DotsPerInch = 300;
            doc.Rendering.AutoRotate = true;

            doc.PageNumber = 1;
            doc.Rect.String = doc.CropBox.String;
            doc.Rendering.SaveAppend = false;
            doc.SetInfo(0, "ImageCompression", "4");

            imagePath = filePath + "_.tif";
            doc.Rendering.Save(imagePath);

            doc.Clear();

            return imagePath;
        }

        /// <summary>
        /// show Unauthorized Message
        /// </summary>
        public static void ShowUnauthorizedMessage()
        {
            HttpContext.Current.Response.Write(Constants.UnathorizedPageAccessErrorMessage);
            HttpContext.Current.Response.End();
        }

        public static void MergePdfFiles(ArrayList inputPdfFiles, string destinationFile)
        {
            string errorMessage = string.Empty;
            MergePdfFiles(inputPdfFiles, destinationFile, out errorMessage);
        }

        #region Added by Edward 03.10.2013  Commented By Edward 2014/04/2015
        //public static void MergePdfFiles(ArrayList inputPdfFiles, string destinationFile, out string errorMessage)
        //{
        //    ErrorLogDb errorLogDb = new ErrorLogDb();
        //    errorMessage = string.Empty;

        //    if (inputPdfFiles.Count == 1)
        //    {
        //        FileInfo pdfFile = new FileInfo(inputPdfFiles[0].ToString());
        //        pdfFile.CopyTo(destinationFile, true);

        //    }
        //    else
        //    {
        //        Document document = new Document();
        //        PdfCopy copy = new PdfCopy(document, new FileStream(destinationFile, FileMode.Create));
        //        document.Open();
        //        PdfImportedPage page;
        //        // this object **REQUIRED** to add content when concatenating PDFs
        //        PdfCopy.PageStamp stamp;

        //        try
        //        {
        //            foreach (string p in inputPdfFiles)
        //            {
        //                PdfReader reader = null;
        //                try
        //                {
        //                    reader = new PdfReader(new RandomAccessFileOrArray(p), null);
        //                    errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, "TryOpenPDFDocument---FilePath: " + p.ToString());
        //                }
        //                catch (Exception exception)
        //                {
        //                    reader = new PdfReader(new RandomAccessFileOrArray(p.Replace("_.tiff_s.pdf", string.Empty)), null);
        //                    errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, exception.Message + "---InnerException: " + exception.InnerException + "---StackTrace: " + exception.StackTrace + "---FilePath: " + p.ToString());
        //                }
        //                int pages = reader.NumberOfPages;

        //                //loop over document pages
        //                for (int i = 0; i < pages; )
        //                {
        //                    page = copy.GetImportedPage(reader, ++i);
        //                    stamp = copy.CreatePageStamp(page);
        //                    //resulting in Auto Rotate the PDF Merged
        //                    //PdfContentByte cb = stamp.GetUnderContent();
        //                    //cb.SaveState();
        //                    stamp.AlterContents();
        //                    copy.AddPage(page);
        //                }
        //                reader.Close();
        //            }
        //            document.Close();
        //        }

        //        catch (Exception exce)
        //        {
        //            errorMessage = exce.Message + "<br><br>" + exce.InnerException + "<br><br>" + exce.StackTrace;
        //            errorLogDb.Insert(ErrorLogFunctionName.MergePDFDocument, errorMessage);

        //            if (document != null)
        //                document.Close();

        //            if (copy != null)
        //                copy.Close();
        //        }
        //        finally
        //        {
        //            if (document != null)
        //                document.Close();

        //            if (copy != null)
        //                copy.Close();
        //        }

        //    }

        //    //code copied from h ttp://kuujinbo.info/cs/itext_cat.aspx
        //}
        #endregion

        #region MergePdfFiles commented by Edward 03.10.2013  Uncommented by Edward 2014/04/15
        //public static void MergePdfFiles(ArrayList inputPdfFiles, string destinationFile, out string errorMessage)
        //{
        //    ErrorLogDb errorLogDb = new ErrorLogDb();
        //    errorMessage = string.Empty;
        //    Document document = new Document();
        //    PdfCopy copy = new PdfCopy(document, new FileStream(destinationFile, FileMode.Create));

        //    document.Open();
        //    PdfImportedPage page;
        //    // this object **REQUIRED** to add content when concatenating PDFs
        //    PdfCopy.PageStamp stamp;

        //    try
        //    {
        //        foreach (string p in inputPdfFiles)
        //        {
        //            PdfReader reader = null;
        //            try
        //            {
        //                reader = new PdfReader(new RandomAccessFileOrArray(p), null);
        //                //errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, "TryOpenPDFDocument---FilePath: " + p.ToString());    Commented by Edward to Reduce Error Notifications 2015/8/24
        //            }
        //            catch (Exception exception)
        //            {
        //                reader = new PdfReader(new RandomAccessFileOrArray(p.Replace("_.tiff_s.pdf", string.Empty)), null);
        //                errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, exception.Message + "---InnerException: " + exception.InnerException + "---StackTrace: " + exception.StackTrace + "---FilePath: " + p.ToString());
        //            }
        //            int pages = reader.NumberOfPages;

        //            // loop over document pages
        //            for (int i = 0; i < pages; )
        //            {
        //                page = copy.GetImportedPage(reader, ++i);
        //                stamp = copy.CreatePageStamp(page);
        //                //resulting in Auto Rotate the PDF Merged
        //                //PdfContentByte cb = stamp.GetUnderContent();
        //                //cb.SaveState();
        //                stamp.AlterContents();
        //                copy.AddPage(page);
        //            }
        //        }

        //        document.Close();
        //    }
        //    catch (Exception exce)
        //    {
        //        errorMessage = exce.Message + "<br><br>" + exce.InnerException + "<br><br>" + exce.StackTrace;
        //        errorLogDb.Insert(ErrorLogFunctionName.MergePDFDocument, errorMessage);

        //        if (document != null)
        //            document.Close();

        //        if (copy != null)
        //            copy.Close();
        //    }
        //    
        //    finally
        //    {
        //        if (document != null)
        //            document.Close();
        //
        //        if (copy != null)
        //            copy.Close();
        //    }

        //    //code copied from h ttp://kuujinbo.info/cs/itext_cat.aspx
        //}

        //Modified by Edward 2017/10/25 to address
        //The document has no pages
        //The process cannot access the file 'E:\Websites\Go-DWMS\App_Data\Temp\Payslip_Income Letter - 5629027.pdf' because it is being used by another process.
        public static void MergePdfFiles(ArrayList inputPdfFiles, string destinationFile, out string errorMessage)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            errorMessage = string.Empty;
            Document document = new Document();

            using (FileStream stream = new FileStream(destinationFile, FileMode.Create))
            {
                PdfCopy copy = new PdfCopy(document, stream);
                document.Open();
                PdfImportedPage page;
                // this object **REQUIRED** to add content when concatenating PDFs
                PdfCopy.PageStamp stamp;

                try
                {
                    foreach (string p in inputPdfFiles)
                    {
                        PdfReader reader = null;
                        try
                        {
                            reader = new PdfReader(new RandomAccessFileOrArray(p), null);
                            //errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, "TryOpenPDFDocument---FilePath: " + p.ToString());    Commented by Edward to Reduce Error Notifications 2015/8/24
                        }
                        catch (Exception exception)
                        {
                            reader = new PdfReader(new RandomAccessFileOrArray(p.Replace("_.tiff_s.pdf", string.Empty)), null);
                            errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, exception.Message + "---InnerException: " + exception.InnerException + "---StackTrace: " + exception.StackTrace + "---FilePath: " + p.ToString());
                        }
                        int pages = reader.NumberOfPages;

                        // loop over document pages
                        for (int i = 0; i < pages; )
                        {
                            page = copy.GetImportedPage(reader, ++i);
                            stamp = copy.CreatePageStamp(page);
                            //resulting in Auto Rotate the PDF Merged
                            //PdfContentByte cb = stamp.GetUnderContent();
                            //cb.SaveState();
                            stamp.AlterContents();
                            copy.AddPage(page);
                        }
                    }

                    //document.Close();
                    //Added by Edward 2017/10/25 to address ERROR The document has no pages.
                    if (document != null)
                        document.Close();

                    if (copy != null)
                        copy.Close();
                }
                catch (Exception exce)
                {
                    errorMessage = exce.Message + "<br><br>" + exce.InnerException + "<br><br>" + exce.StackTrace;
                    errorLogDb.Insert(ErrorLogFunctionName.MergePDFDocument, errorMessage);

                    if (document != null)
                        document.Close();

                    if (copy != null)
                        copy.Close();
                }
            }                        
            //code copied from h ttp://kuujinbo.info/cs/itext_cat.aspx
        }
        #endregion

        public static void MergePdfFilesOLD(ArrayList inputPdfFiles, string destinationFile, out string errorMessage)
        {
            ErrorLogDb errorLogDb = new ErrorLogDb();
            errorMessage = string.Empty;

            PdfReader reader = null;
            Document document = null;
            PdfWriter writer = null;

            try
            {
                int f = 0;

                // if _.tiff_s.pdf is corrupted and for some reason not able to open, then get the main pdf file.
                try
                {
                    reader = new PdfReader(inputPdfFiles[f].ToString());
                }
                catch (Exception exception)
                {
                    reader = new PdfReader(inputPdfFiles[f].ToString().Replace("_.tiff_s.pdf", string.Empty));
                    errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, exception.Message + "---InnerException: " + exception.InnerException + "---StackTrace: " + exception.StackTrace + "---FilePath: " + inputPdfFiles[f].ToString());
                }

                // we retrieve the total number of pages
                int n = reader.NumberOfPages;
                //Console.WriteLine("There are " + n + " pages in the original file.");
                // step 1: creation of a document-object
                document = new Document(reader.GetPageSizeWithRotation(1));
                // step 2: we create a writer that listens to the document
                writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.Create));
                // step 3: we open the document
                document.Open();
                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage page;
                int rotation;
                // step 4: we add content
                while (f < inputPdfFiles.Count)
                {
                    int i = 0;
                    while (i < n)
                    {
                        i++;
                        page = writer.GetImportedPage(reader, i);
                        rotation = reader.GetPageRotation(i);
                        document.SetPageSize(reader.GetPageSizeWithRotation(i));
                        //document.SetPageSize(new iTextSharp.text.Rectangle(0.0F, 0.0F, page.Width, page.Height));
                        document.NewPage();

                        if (rotation == 90 || rotation == 270)
                        {
                            if (rotation == 90)
                                cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                            else
                                cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, reader.GetPageSizeWithRotation(i).Width, 0);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }

                    }
                    f++;
                    if (f < inputPdfFiles.Count)
                    {
                        // if _.tiff_s.pdf is corrupted and for some reason not able to open, then get the main pdf file.
                        try
                        {
                            reader = new PdfReader(inputPdfFiles[f].ToString());
                        }
                        catch (Exception exception)
                        {
                            reader = new PdfReader(inputPdfFiles[f].ToString().Replace("_.tiff_s.pdf", string.Empty));
                            errorLogDb.Insert(ErrorLogFunctionName.UnableToOpenPDFDocument, exception.Message + "---InnerException: " + exception.InnerException + "---StackTrace: " + exception.StackTrace + "---FilePath: " + inputPdfFiles[f].ToString());
                        }

                        // we retrieve the total number of pages
                        n = reader.NumberOfPages;
                        //Console.WriteLine("There are " + n + " pages in the original file.");
                    }
                }
                // step 5: we close the document, writer and reader

                if (document != null)
                    document.Close();
                if (writer != null)
                    writer.Close();
                if (reader != null)
                    reader.Close();
            }
            catch (Exception e)
            {
                errorMessage = e.Message + "<br><br>" + e.InnerException + "<br><br>" + e.StackTrace;
                errorLogDb.Insert(ErrorLogFunctionName.MergePDFDocument, errorMessage);
            }
            finally
            {
                if (document != null)
                    document.Close();
                if (writer != null)
                    writer.Close();
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Create the Set number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FormulateSetNumber(int id, out int departmentId, out int sectionId)
        {
            departmentId = -1;
            sectionId = -1;
            string setNumber = string.Empty;

            MembershipUser user = Membership.GetUser();

            if (user != null)
            {
                ProfileDb profileDb = new ProfileDb();
                DataTable profileDt = profileDb.GetvProfile((Guid)user.ProviderUserKey);

                if (profileDt.Rows.Count > 0)
                {
                    DataRow profile = profileDt.Rows[0];
                    setNumber = Format.FormatSetNumber(profile["DepartmentCode"].ToString().Trim(), profile["BusinessCode"].ToString().Trim(), DateTime.Now, id);

                    departmentId = int.Parse(profile["DepartmentId"].ToString());
                    sectionId = int.Parse(profile["SectionId"].ToString());
                }
            }

            return setNumber;
        }

        /// <summary>
        /// Get the temp folder (to save the sample documents) of the user
        /// </summary>
        /// <returns></returns>
        public static string GetUsersSampleDocTempFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetSampleDocsMainDirPath());

            // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
            // Save each file for each user in a folder.  Folder name is the GUID of the user
            //return userTempPath + Dwms.Web.Retrieve.GetFormsAuthenticationUserData();
        }

        /// <summary>
        /// Get the temp folder (to save the scanned/uploaded documents) of the user
        /// </summary>
        /// <returns></returns>
        public static string GetUsersDocForOcrTempFolder()
        {
            //string userTempPath = System.Web.HttpContext.Current.Request.MapPath(Dwms.Web.Retrieve.GetDocsForOcrDirPath());
            //string userTempPath = Path.GetTempPath();

            // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
            // Save each file for each user in a folder.  Folder name is the GUID of the user
            //return userTempPath + Dwms.Web.Retrieve.GetFormsAuthenticationUserData();
            //return Path.Combine(userTempPath, Dwms.Web.Retrieve.GetFormsAuthenticationUserData());
            //return Path.Combine(GetTempFolder(), (Retrieve.GetFormsAuthenticationUserData()+Format.FormatDateTime(DateTime.Now, DateTimeFormat.yyyyMMdd_dash_HHmmss)));
            return Path.Combine(GetTempFolder(), Retrieve.GetFormsAuthenticationUserData());
        }

        /// <summary>
        /// Get the temp folder (to save the scanned/uploaded documents) of the user
        /// </summary>
        /// <returns></returns>
        #region Not being used To Delete by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static string GetDocForOcrFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetDocsForOcrDirPath());
        }
        #endregion

        #region Not being used To Delete by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static string GetRawPageOcrFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetRawPageOcrDirPath());
        }
        #endregion


        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY GetIndividualRawPageOcrDirectoryInfo(int rawPageId)
        //public static DirectoryInfo GetIndividualRawPageOcrDirectoryInfo(int rawPageId)
        //{
        //    string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetRawPageOcrDirPath());
        //    mainDir = Path.Combine(mainDir, rawPageId.ToString());
        //    DirectoryInfo mainDirInfo = new DirectoryInfo(mainDir);
        //    return mainDirInfo;
        //}
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static DirectoryInfo GetIndividualRawPageOcrDirectoryInfo(int rawPageId)
        {            
            DataTable dt = RawPageDb.GetYearMonthDayForFolderStructure(rawPageId);
            string datePath = string.Empty;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                datePath = dr["fYear"].ToString() + @"\" + dr["fMonth"].ToString() + @"\" + dr["fDay"].ToString();
            }
            
            string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetDocDirPath());
            string rawPagePath = datePath + @"\" + Retrieve.GetRawPageOcrDirPath() + @"\" + rawPageId.ToString();
            mainDir = Path.Combine(mainDir, rawPagePath);
            DirectoryInfo mainDirInfo = new DirectoryInfo(mainDir);
            return mainDirInfo;
        }
        #endregion       

        
        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY GetIndividualRawFileOcrFolderPath(int docSetId, int rawFileId)
        //public static string GetIndividualRawFileOcrFolderPath(int docSetId, int rawFileId)
        //{
        //    string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetDocsForOcrDirPath());
        //    mainDir = Path.Combine(mainDir, docSetId.ToString());
        //    mainDir = Path.Combine(mainDir, rawFileId.ToString());
        //    return mainDir;
        //}
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static string GetIndividualRawFileOcrFolderPath(int docSetId, int rawFileId)
        {
            DataTable dt = DocSetDs.GetYearMonthDayForFolderStructure(docSetId);
            string datePath = string.Empty;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                datePath = dr["fYear"].ToString() + @"\" + dr["fMonth"].ToString() + @"\" + dr["fDay"].ToString();
            }

            string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetDocDirPath());
            string forOCRPath = datePath + @"\" + Retrieve.GetDocsForOcrDirPath() + @"\" + docSetId.ToString();
            mainDir = Path.Combine(mainDir, forOCRPath);
            mainDir = Path.Combine(mainDir, rawFileId.ToString());
            return mainDir;
        }

        /// <summary>
        /// Returns the DOC/YEAR/MONTH/DAY/FOROCR/SETID/ path
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="verificationDateIn"></param>
        /// <returns></returns>
        public static string GetDocForOcrFolder(int docSetId, DateTime verificationDateIn)
        {
            string datePath = string.Empty;
            datePath = verificationDateIn.Year.ToString() + @"\" + verificationDateIn.Month.ToString() + @"\" + verificationDateIn.Day.ToString();
            string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetDocDirPath());
            string forOCRPath = datePath + @"\" + Retrieve.GetDocsForOcrDirPath() + @"\" + docSetId.ToString();
            mainDir = Path.Combine(mainDir, forOCRPath);
            return mainDir;
        }

        public static bool GetVerificationDateForOcrFolder(int docSetId, out DateTime datePath)
        {
            DataTable dt = DocSetDb.GetYearMonthDayForFolderStructure(docSetId);
            //DateTime datePath = new DateTime();
            bool isDate = false;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                isDate = DateTime.TryParse(dr["VerificationDateIn"].ToString(), out datePath);
            }
            else
                datePath = new DateTime();
            return isDate;
        }

        public static string GetLogFilesFolder(string div, string strOthers, string strOthers2)
        {
            string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetLogFilesFolderDirPath());
            string datePath = DateTime.Now.Year.ToString() + @"\" + DateTime.Now.Month.ToString() + @"\" + DateTime.Now.Day.ToString();
            string fileName = string.Empty;

            if (div.Equals("CDB"))
                fileName = DateTime.Now.ToString("CDB-yyyyMMdd-HHmmss.fff") + (String.IsNullOrEmpty(strOthers) ? string.Empty : "-" + strOthers) + ".txt";
            else if (div.Equals("LEAS"))
                fileName = DateTime.Now.ToString("LEAS-yyyyMMdd-HHmmss.fff") + "-" + strOthers + ".txt";
            else if (div.Equals("SALE"))
                fileName = DateTime.Now.ToString("SALE-yyyyMMdd-HHmmss.fff") + "-" + strOthers + ".txt";
            else if (div.Equals("RESALE"))
                fileName = DateTime.Now.ToString("RESALE-yyyyMMdd-HHmmss.fff") + "-" + strOthers + ".txt";
            else if (div.Equals("SERS"))
                fileName = DateTime.Now.ToString("SERS-yyyyMMdd-HHmmss.fff") + "-" + strOthers + ".txt";
            else if (div.Equals("PENDOC"))
                fileName = string.Format("PenDoc-{0}-{1}_{2}", strOthers, Format.FormatDateTime(DateTime.Now, DateTimeFormat.yyyyMMdd_dash_HHmmss), strOthers2);

            mainDir = Path.Combine(mainDir, datePath);

            //Added by Edward to prevent DirectoryNotFound Error
            if (!Directory.Exists(mainDir))
                Directory.CreateDirectory(mainDir);

            mainDir = Path.Combine(mainDir, fileName);
            return mainDir;
        }

        #endregion

        /// <summary>
        /// Get Reference Type
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public static string GetReferenceType(string referenceNumber)
        {
            if (String.IsNullOrEmpty(referenceNumber))
                return "N.A.";

            string refType = ReferenceTypeEnum.Others.ToString();

            if (Validation.IsHLENumber(referenceNumber))
                refType = ReferenceTypeEnum.HLE.ToString();
            else if (Validation.IsResaleNumber(referenceNumber))
                refType = ReferenceTypeEnum.RESALE.ToString();
            else if (Validation.IsSalesNumber(referenceNumber))
                refType = ReferenceTypeEnum.SALES.ToString();
            else if (Validation.IsSersNumber(referenceNumber))
                refType = ReferenceTypeEnum.SERS.ToString();
            //else if (Validation.IsNric(referenceNumber))
            //    refType = ScanningReferenceTypeEnum.NRIC.ToString();
            else if (Validation.IsNricFormat(referenceNumber))
                refType = ReferenceTypeEnum.NRIC.ToString();

            return refType;
        }

        /// <summary>
        /// Check if the user has access to the function
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="module"></param>
        /// <param name="accessRight"></param>
        /// <returns></returns>
        public static bool HasAccessRights(ModuleNameEnum module, AccessControlSettingEnum accessRight)
        {
            bool hasAccess = false;

            // Get the roles
            string[] roles = Roles.GetRolesForUser();

            if (roles.Length > 0)
            {
                string role = roles[0];

                RoleDb roleDb = new RoleDb();
                Guid roleId = roleDb.GetRoleGuid(role);

                if (!roleId.Equals(Guid.Empty))
                {
                    AccessControlDb accessControlDb = new AccessControlDb();
                    hasAccess = accessControlDb.HasAccessRights(roleId, module, accessRight);
                }
            }

            return hasAccess;
        }

        public static void LogException(Exception error, string methodName)
        {
            StringBuilder builder = new StringBuilder();

            try
            {
                ParameterDb parameterDb = new ParameterDb();
                string enableErrorNotification = parameterDb.GetParameter(ParameterNameEnum.EnableErrorNotification);

                if (enableErrorNotification.ToLower().Equals("no"))
                {
                    return;
                }

                if (error != null)
                {
                    builder.AppendLine("<html><head><title></title>");
                    builder.AppendLine("</head><body>");
                    builder.AppendLine("<div style='padding: 2px;'>");
                    builder.AppendLine("<table cellpadding='0' cellspacing='0' border='1' width='100%'>");
                    builder.AppendLine("<tr>");
                    builder.AppendLine(String.Format("<td>{0}</td>", "Date Time"));
                    builder.AppendLine(String.Format("<td>{0}</td>", Format.FormatDateTime(DateTime.Now, DateTimeFormat.d__MMM__yyyy_C_h_Col_mm__tt)));
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr>");
                    builder.AppendLine(String.Format("<td>{0}</td>", "Login ID"));
                    builder.AppendLine(String.Format("<td>{0}</td>", Retrieve.GetLoggedInUserName()));
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr>");
                    builder.AppendLine(String.Format("<td>{0}</td>", "Method Name"));
                    builder.AppendLine(String.Format("<td>{0}</td>", methodName));
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
                    builder.AppendLine(String.Format("<td>{0}</td>", "Error Message"));
                    builder.AppendLine(String.Format("<td>{0}</td>", error.Message));
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr>");
                    builder.AppendLine(String.Format("<td>{0}</td>", "Stack Trace"));
                    builder.AppendLine(String.Format("<td>{0}</td>", error.StackTrace));
                    builder.AppendLine("</tr>");
                    builder.AppendLine("</table>");
                    builder.AppendLine("</div></body></html>");
                }

                string senderName = "Hiend Software Pte Ltd";
                string senderEmail = "info@hiend.com";
                string recipientEmails = parameterDb.GetParameter(ParameterNameEnum.ErrorNotificationMailingList);

                if (String.IsNullOrEmpty(recipientEmails))
                {
                    return;
                }

                string subject = String.Concat("DWMS - Error in ", methodName);
                string message = builder.ToString();

                SendMailOnError(senderName, senderEmail, recipientEmails, subject, message);
            }
            catch (Exception ex)
            {

            }
        }

        public static bool SendMailOnError(string senderName, string senderEmail, string recipientEmails, string subject, string message)
        {
            bool sent = false;

            char[] delimiterChars = { ';', ',', ':' };
            string from = String.Format("{0} <{1}>", senderName, senderEmail);
            string cr = Environment.NewLine;

            message = message.Replace(cr, "<br />");
            message = message.Replace("\n", "<br />");

            try
            {
                MailMessage m = new MailMessage();

                m.From = new MailAddress(from);

                // To
                string[] toEmailList = recipientEmails.Split(delimiterChars);

                foreach (string s in toEmailList)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        string email = s.Trim();

                        if (Validation.IsEmail(email))
                        {
                            m.To.Add(new MailAddress(email));
                        }
                    }
                }

                m.IsBodyHtml = true;
                m.Subject = subject;
                m.Body = message;

                SmtpClient client = new SmtpClient();
                client.Send(m);
                sent = true;
            }
            catch (Exception ex)
            {

            }

            return sent;
        }

        public static bool DeleteFolderContents(string mainDirPath)
        {
            bool result = false;

            try
            {
                DirectoryInfo mainDir = new DirectoryInfo(mainDirPath);

                // Delete the sub-directories
                DirectoryInfo[] subDirs = mainDir.GetDirectories();

                foreach (DirectoryInfo subDir in subDirs)
                {
                    try
                    {
                        subDir.Delete(true);
                    }
                    catch (Exception)
                    {
                    }
                }

                // Delete the files under the main dir
                FileInfo[] files = mainDir.GetFiles();

                foreach(FileInfo file in files)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception)
                    {
                    }
                }

                result = true;
            }
            catch (Exception)
            {
            }        

            return result;
        }

        public static string CreatePdfFileFromImage(string sourceFilePath)
        {
            Document doc = new Document();
            string destPath = string.Empty;

            try
            {
                destPath = sourceFilePath + ".pdf";

                PdfWriter.GetInstance(doc, new FileStream(destPath, FileMode.OpenOrCreate));
                doc.Open();
                iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(destPath);
                png.ScaleToFit(475f, 1500f);
                doc.Add(png);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                doc.Close();
            }

            return destPath;
        }

        public static string CreatePdfFileFromOriginalImage (string sourceFilePath)
        {
            string pdfPath = sourceFilePath + "_s.pdf";
            Document doc = new Document(PageSize.A4);
            try{
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfPath, FileMode.Create));
                doc.Open();
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(sourceFilePath);
                img.ScaleToFit(PageSize.A4.Width, PageSize.A4.Height);
                img.SetAbsolutePosition(0, PageSize.A4.Top - img.ScaledHeight);
                doc.Add(img);
            }
            catch { return "Error Creating PDF File"; }
            finally
            {
                if (doc != null)
                {
                    if (doc.IsOpen())
                        doc.Close();

                    doc = null;
                }
            }
            return pdfPath;
        }

        public static string GetTempFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetTempDirPath());
        }

        /// <summary>
        /// set the setinformation into the label
        /// </summary>
        /// <param name="docSets"></param>
        /// <param name="displayLabel"></param>
        /// <param name="userSectionId"></param>
        public static void DisplaySetInformation(int noOfSets, LinkButton displayLabel, int docAppId)
        {
            if (noOfSets == 0)
                displayLabel.Text = "(No Sets)";
            else
            {
                displayLabel.Text = "(" + noOfSets + " Set" + (noOfSets > 1 ? "s" : "") + ")";

                displayLabel.OnClientClick = String.Format("javascript:ShowWindow('../Common/ViewSetsByApplication.aspx?id={0}', 650, 300);", docAppId);
            }
        }

        /// <summary>
        /// Display link to view documents listing for a given docset
        /// </summary>
        /// <param name="displayLabel"></param>
        /// <param name="docSetId"></param>
        public static void DisplayDocumentInformation(ImageButton displayImage, int docSetId)
        {
            displayImage.ImageUrl = "~/Data/Images/Icons/grid.png";
            displayImage.ToolTip = Constants.ViewDocuments;
            displayImage.OnClientClick = String.Format("javascript:ShowWindow('../../Common/ViewDocument.aspx?id={0}', 650, 500);", docSetId);
        }

        public static void DisplaySetsNotVerifiedIndicator(System.Web.UI.WebControls.Image indicatorImage, int docAppId)
        {
            DocSetDb docSetDb = new DocSetDb();
            if (!docSetDb.AreAllDocSetsVerifiedOrClosed(docAppId))
            {
                indicatorImage.ImageUrl = "../Data/Images/Icons/Exclamation.jpg";
                indicatorImage.ToolTip = Constants.HasSetsNotVerifiedInApplication;
                indicatorImage.Visible = true;
            }
        }

        public static void DisplayHasPendingDocIndicator(System.Web.UI.WebControls.Image indicatorImage, string hleNumber)
        {
            DocAppDb docAppDb = new DocAppDb();
            docAppDb.UpdatePendingDoc(hleNumber);
            if (docAppDb.GetDocAppsByReferenceNo(hleNumber).Count > 0)
            {
                if (docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].IsHasPendingDocNull() ? false : !docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].HasPendingDoc)
                {
                    indicatorImage.ImageUrl = "../Data/Images/Icons/Thumb_up.png";
                    indicatorImage.ToolTip = Constants.HasPendingDocInApplication;
                    indicatorImage.Visible = true;
                }
                //}
            }
        }

        #region  added by Edward 2017/11/01 To Optimize speed of Verification Listing 
        public static bool DisplayHasPendingDocIndicator(string hleNumber)
        {
            return DocAppDb.UpdateAndCheckHasPendingDoc(hleNumber);
        }
        #endregion


        public static void DisplayHasPendingDocIndicatorSearch(System.Web.UI.WebControls.Image indicatorImage, string hleNumber)
        {
            DocAppDb docAppDb = new DocAppDb();
            docAppDb.UpdatePendingDoc(hleNumber);
            if (docAppDb.GetDocAppsByReferenceNo(hleNumber).Count > 0)
            {
                if (docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].IsHasPendingDocNull() ? false : !docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].HasPendingDoc)
                {//path is not the same as above
                    indicatorImage.ImageUrl = "../../Data/Images/Icons/Thumb_up.png";
                    indicatorImage.ToolTip = Constants.HasPendingDocInApplication;
                    indicatorImage.Visible = true;
                }
                //}
            }
        }

        public static void DisplaySecondCA(System.Web.UI.WebControls.Image indicatorImage, string hleNumber)
        {
            DocAppDb docAppDb = new DocAppDb();
            if (docAppDb.GetDocAppsByReferenceNo(hleNumber).Count > 0)
            {
                if (docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].IsSecondCAFlagNull()? false : docAppDb.GetDocAppsByReferenceNo(hleNumber)[0].SecondCAFlag)
                {
                    indicatorImage.ImageUrl = "../Data/Images/Icons/CopyHS.png";
                    indicatorImage.ToolTip = Constants.isSecondCA;
                    indicatorImage.Visible = true;
                }
            }
        }

        /// <summary>
        /// Display Application details in search pages
        /// </summary>
        /// <param name="displayLabel"></param>
        /// <param name="docSetId"></param>
        public static void DisplayApplicationInformationForSearch(Label displayLabel, int docSetId)
        {
            SetAppDb setAppDb = new SetAppDb();
            SetApp.vSetAppDataTable setApps = setAppDb.GetvSetAppByDocSetId(docSetId);

            if (setApps.Rows.Count > 0)
            {
                foreach (SetApp.vSetAppRow setAppRow in setApps.Rows)
                {
                    displayLabel.Text += setAppRow.RefNo + "<br>";
                }
            }
            else
                displayLabel.Text = "-";

            char[] charsBreak = { '<', 'b', 'r', '>' };

            displayLabel.Text = displayLabel.Text.TrimEnd(charsBreak);
        }

        

        /// <summary>
        /// Check for the file Not Verified indicator if (the appstatus is Completeness_Checked) or if (appsstus is Completeness_Checked and setCount ir more than 1)
        /// </summary>
        /// <param name="IndicatorImage"></param>
        /// <param name="docAppId"></param>
        /// <param name="appStatus"></param>
        /// <param name="setCount"></param>
        public static void DisplayFilesNotVerifiedIndicator(System.Web.UI.WebControls.Image IndicatorImage, int docAppId, string appStatus, int setCount)
        {
            if (appStatus.ToLower().Equals(AppStatusEnum.Completeness_Checked.ToString().ToLower()) || (setCount > 1 && appStatus.ToLower().Equals(AppStatusEnum.Completeness_In_Progress.ToString().ToLower())))
            {
                DocAppDb docAppDb = new DocAppDb();
                int notVerifedFiles = docAppDb.GetNotVerifiedFilesForCheckedApplicationsCount(docAppId);
                if (notVerifedFiles > 0)
                {
                    IndicatorImage.ImageUrl = "../Data/Images/QuestionMark.jpg";
                    IndicatorImage.ToolTip = Constants.HasFileNotVerified;
                    IndicatorImage.Visible = true;
                }
            }
        }

        public static void DisplaySetInformationOld(DocSet.DocSetDataTable docSets, Label DisplayLabel, int userSectionId)
        {
            foreach (DocSet.DocSetRow docSetRow in docSets.Rows)
            {
                int setId = docSetRow.Id;
                int setSectionId = docSetRow.SectionId;

                string destinationSection = string.Empty;
                if (setSectionId != userSectionId)
                {
                    SectionDb sectionDb = new SectionDb();
                    Section.SectionDataTable sections = sectionDb.GetSectionById(setSectionId);
                    Section.SectionRow sectionRow = sections[0];
                    destinationSection = sectionRow.Name.Trim() + " (" + sectionRow.Code.Trim() + ")";
                }
                DisplayLabel.Text += (string.IsNullOrEmpty(DisplayLabel.Text.Trim()) ? string.Empty : ", ") + "<a href=../Verification/view.aspx?id=" + setId +
                    " title='" + docSetRow.SetNo + ": " + docSetRow.Status.Replace("_", " ") + (setSectionId != userSectionId ? "\nRouted to " + destinationSection + "." : string.Empty) + "' target='_blank'>" + setId.ToString() + "</a>";
            }
        }

        public static void DisplayUrgencyIndicator(SetStatusEnum setStatus, System.Web.UI.WebControls.Image UrgentImage, 
            int setId, Boolean isUrgent, Boolean isSkipCategorization, CheckBox ChildCheckBox)
        {
            if (setStatus.Equals(SetStatusEnum.Pending_Categorization))
            {
                if (isUrgent && isSkipCategorization)
                {
                    UrgentImage.ImageUrl = "~/Data/Images/Icons/RocketRed.png";
                    UrgentImage.ToolTip = Constants.MarkedUrgentAndSkipCategoriztion;
                }
                else if (isUrgent && !isSkipCategorization)
                {
                    UrgentImage.ImageUrl = "~/Data/Images/Icons/RocketGreen.png";
                    UrgentImage.ToolTip = Constants.MarkedUrgentSet;
                }
                else
                {
                    UrgentImage.ImageUrl = "~/Data/Images/Icons/Car.png";
                    UrgentImage.ToolTip = Constants.NotMarkedUrgentSet;
                }
            }
            else
            {
                if (ChildCheckBox != null)
                {
                    ChildCheckBox.Enabled = ChildCheckBox.Visible = false;
                }
                UrgentImage.Visible = false;
            }
        }

        public static void DisplayApplicationInformation(Label appInformationLabel, SetApp.vSetAppRow setAppRow)
        {
            string caseOIC = setAppRow.IsCaseOICNull() ? "-" : (setAppRow.CaseOIC.Trim().Length == 0 ? "-" : setAppRow.CaseOIC);
            string peOIC = setAppRow.IsPeOICNull() ? "-" : (setAppRow.PeOIC.Trim().Length == 0 ? "-" : setAppRow.PeOIC);
            appInformationLabel.Text += setAppRow.RefNo + " (" + peOIC + "/" + caseOIC + ")" + "<br>";
        }


        public static void DisplayWaitingTime(Label WaitingTimeLabel, DateTime processingEndDate, DateTime verificationDateTime, SetStatusEnum setStatus)
        {
            if (!setStatus.Equals(SetStatusEnum.Pending_Categorization))
            {
                TimeSpan diff = processingEndDate.Subtract(verificationDateTime);
                WaitingTimeLabel.Text = Format.GetWaitingTime(diff);
            }
            else
            {
                WaitingTimeLabel.Text = "-";
            }
        }

        public static void DisplayProcessingTime(Label ProcessingTimeLabel, DateTime processingStartDate, DateTime processingEndDate, SetStatusEnum setStatus)
        {
            if (!setStatus.Equals(SetStatusEnum.Pending_Categorization))
            {
                TimeSpan diff = processingEndDate.Subtract(processingStartDate);
                ProcessingTimeLabel.Text = Format.GetWaitingTime(diff);
            }
            else
            {
                ProcessingTimeLabel.Text = "-";
            }
        }

        public static string GetWebServiceDocsFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetWebServiceDocsDirPath());
        }

        #region Not being used To Delete by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
        public static string GetLogFilesFolder()
        {
            return System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetLogFilesFolderDirPath());
        }
        #endregion
        #endregion

        public static bool DownloadFileFromUrl(string url, string fileName, string destinationDir, out string destinationFile)
        {
            bool result = false;
            destinationFile = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                using (WebClient wc = new WebClient())
                {
                    if (!Directory.Exists(destinationDir))
                        Directory.CreateDirectory(destinationDir);

                    destinationFile = Path.Combine(destinationDir, fileName);
                    wc.DownloadFile(url, destinationFile);

                    //start download
                    WebClient client = new WebClient();
                    client.DownloadFile(url, destinationFile);

                    result = File.Exists(destinationFile);
                }
            }

            return result;
        }

        public static ArrayList PdfSplit(string sourcePdf, string destFolder)
        {
            ArrayList aPdfFileList = new ArrayList();
            FileInfo file = new FileInfo(sourcePdf);
            string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

            PdfReader reader1 = new PdfReader(sourcePdf);

            try
            {
                int pageCount = 0;
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

                reader1.RemoveUnusedObjects();
                pageCount = reader1.NumberOfPages;

                for (int pageNo = 1; pageNo <= pageCount; pageNo++)
                {
                    string outfile = Path.Combine(destFolder, name + "_" + pageNo.ToString() + ".pdf");

                    if (!File.Exists(outfile))
                    {
                        Document doc = new Document(reader1.GetPageSizeWithRotation(pageNo));

                        PdfCopy pdfCpy = new PdfCopy(doc, new System.IO.FileStream(outfile, System.IO.FileMode.Create));

                        try
                        {
                            doc.Open();

                            PdfImportedPage page = pdfCpy.GetImportedPage(reader1, pageNo);  //first page start from 1, NOT 0
                            pdfCpy.AddPage(page);

                            aPdfFileList.Add(outfile);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            doc.Close();
                            pdfCpy.CloseStream = true;
                            pdfCpy.Close();
                        }
                    }
                    else
                    {
                        aPdfFileList.Add(outfile);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader1.Close();
            }

            return aPdfFileList;
        }

        public static ArrayList TiffSplit(string sourceFile, string destinationFolder)
        {
            ArrayList tiffFileList = new ArrayList();

            // Get the frame dimension list from the image of the file and 
            #region 20170905 Updated By Edward Use FromStream for Out of memory
            string strPhoto = (sourceFile);
            FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);
            //using (System.Drawing.Image tiffImage = System.Drawing.Image.FromFile(sourceFile))
            using (System.Drawing.Image tiffImage = System.Drawing.Image.FromStream(fs))
            #endregion            
            {
                //get the globally unique identifier (GUID) 
                Guid objGuid = tiffImage.FrameDimensionsList[0];
                //create the frame dimension 
                FrameDimension dimension = new FrameDimension(objGuid);
                //Gets the total number of frames in the .tiff file 
                int noOfPages = tiffImage.GetFrameCount(dimension);

                ImageCodecInfo encodeInfo = null;
                ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
                for (int j = 0; j < imageEncoders.Length; j++)
                {
                    if (imageEncoders[j].MimeType == "image/tiff")
                    {
                        encodeInfo = imageEncoders[j];
                        break;
                    }
                }

                // Save the tiff file in the output directory. 
                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                foreach (Guid guid in tiffImage.FrameDimensionsList)
                {
                    for (int index = 0; index < noOfPages; index++)
                    {
                        FrameDimension currentFrame = new FrameDimension(guid);
                        tiffImage.SelectActiveFrame(currentFrame, index);
                        string fileName = string.Concat(destinationFolder, @"\", index, ".TIF");
                        tiffFileList.Add(fileName);
                        tiffImage.Save(fileName, encodeInfo, null);
                    }
                }
            }

            return tiffFileList;
        }

        public static int CountTiffPages(string sourceFile)
        {
            int pageCount = 0;

            //Get the frame dimension list from the image of the file and 
            #region 20170905 Updated By Edward Use FromStream for Out of memory
            //System.Drawing.Image tiffImage = System.Drawing.Image.FromFile(sourceFile);
            string strPhoto = (sourceFile);
            FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);                                                                                
            #endregion
            System.Drawing.Image tiffImage = System.Drawing.Image.FromStream(fs);            
            //get the globally unique identifier (GUID) 
            Guid objGuid = tiffImage.FrameDimensionsList[0];
            //create the frame dimension 
            FrameDimension dimension = new FrameDimension(objGuid);
            //Gets the total number of frames in the .tiff file 
            pageCount = tiffImage.GetFrameCount(dimension);

            return pageCount;
        }

        public static string[] SplitString(string text, bool removeNumbers, bool removeEmptyEntries)
        {
            string[] arr;

            #region Old Implementation - Special characters are hard coded
            //char[] delim = { ' ', '\n', '\t', '\r', ',', '.', ';', ':', 
            //'+', '=', '!', '$', '%', '&', '#', '@', '*', '(', ')', '[', ']', 
            //'{', '}', '-', '~','\'', '"', '<', '>', '/', '_', ' ' };

            //char[] delimWithNumbers = { ' ', '\n', '\t', '\r', ',', '.', ';', ':', 
            //'+', '=', '!', '$', '%', '&', '#', '@', '*', '(', ')', '[', ']', 
            //'{', '}', '-', '~','\'', '"', '<', '>', '/', '_', ' ',
            //'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

            //if (removeNumbers)
            //{
            //    if (removeEmptyEntries)
            //        arr = text.Split(delimWithNumbers, StringSplitOptions.RemoveEmptyEntries);
            //    else
            //        arr = text.Split(delimWithNumbers);
            //}
            //else
            //{
            //    if (removeEmptyEntries)
            //        arr = text.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            //    else
            //        arr = text.Split(delim);
            //}
            #endregion

            #region New Implementation - Using regular expression to replace special characters
            string pattern = string.Empty;

            if (removeNumbers)
                pattern = "[^a-zA-Z]";
            else
                pattern = "[^a-zA-Z0-9]";

            Regex regex = new Regex(pattern);
            string modText = regex.Replace(text, " ");

            if (removeEmptyEntries)
                arr = modText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            else
                arr = modText.Split(new char[] { ' ' });
            #endregion

            return arr;
        }

        public static void CreateSearcheablePdfFile(string sourceFilePath)
        {
            FileInfo file = new FileInfo(sourceFilePath);

            if (file.Exists)
            {
                if (file.Extension.ToLower().Equals(".pdf"))
                {
                    // Copy the PDF file
                    string newRawPageTempPath = file.FullName + "_s.pdf";
                    file.CopyTo(newRawPageTempPath);
                }
                else
                {
                    // Create a PDF file from the images
                    CreateSearcheablePdfFileFromImage(file.FullName);
                }
            }
        }

        private static void CreateSearcheablePdfFileFromImage(string sourceFilePath)
        {
            Document doc = new Document();
            try
            {
                string pdfPath = sourceFilePath + "_s.pdf";
                
                PdfWriter.GetInstance(doc, new FileStream(pdfPath, FileMode.Create));
                doc.Open();
                iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(sourceFilePath);
                png.ScaleToFit(475f, 1500f);
                doc.Add(png);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (doc != null)
                {
                    if (doc.IsOpen())
                    {
                        try { doc.Close(); }
                        catch { }
                    }
                    doc = null;
                }
            }
        }

        public static int ExtractTextFromSearcheablePdf(string filePath, int? docSetId, bool isSampleDoc, out string result)
        {
            int errorCode = -1;
            result = string.Empty;

            if (File.Exists(filePath))
            {
                PDDocument doc = null;

                try
                {
                    doc = PDDocument.load(filePath);
                    PDFTextStripper stripper = new PDFTextStripper();

                    result = stripper.getText(doc).Trim();

                    errorCode = (!String.IsNullOrEmpty(result) && !CategorizationHelpers.IsValidTextForRelevanceRanking(result)
                        ? -1 : 0);
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (doc != null)
                        doc.close();
                }
            }

            return errorCode;
        }

        public static string GetOcrLicense()
        {
            return ConfigurationManager.AppSettings["OcrLicense"];
        }

        public static string SaveAsJpegThumbnailImage(string filePath)
        {
            Pdf2ImageSettings pdfSettings = new Pdf2ImageSettings();

            pdfSettings.AntiAliasMode = Cyotek.GhostScript.AntiAliasMode.High;
            pdfSettings.GridFitMode = Cyotek.GhostScript.GridFitMode.None;
            pdfSettings.ImageFormat = Cyotek.GhostScript.ImageFormat.Png8;

            Pdf2Image pdf2Img = new Pdf2Image(filePath);

            pdf2Img.PdfFileName = filePath;
            pdf2Img.Settings = pdfSettings;

            string imagePath = filePath + "_temp.jpeg";
            pdf2Img.ConvertPdfPageToImage(imagePath, 1, 72);

            return imagePath;
        }

        /// <summary>
        /// Generate PDF Path By Set ID To Be Used for Email Attachments (Not Used)
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static string GeneratePdfPathBySetId(int setId, out string errorMsg)
        {
            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            errorMsg = string.Empty;

            DocDb docDb = new DocDb();
            DocAppDb docAppDb = new DocAppDb();
            DocSetDb docSetDb = new DocSetDb();
            RawPageDb rawPageDb = new RawPageDb();
            DocTypeDb docTypeDb = new DocTypeDb();

            #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
            //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);
            #endregion

            Doc.DocDataTable docTable = docDb.GetDocByDocSetId(setId);

            ArrayList docList = new ArrayList();
            if (docTable.Rows.Count > 0)
            {
                foreach (Doc.DocRow r in docTable.Rows)
                {
                    ArrayList pageList = new ArrayList();

                    RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(r.Id);
                    for (int cnt = 0; cnt < rawPages.Count; cnt++)
                    {
                        RawPage.RawPageRow rawPage = rawPages[cnt];
                        #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
                        //DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

                        //if (rawPageDirs.Length > 0)
                        //{
                        //    DirectoryInfo rawPageDir = rawPageDirs[0];

                        //    FileInfo[] rawPageFiles = rawPageDir.GetFiles();

                        //    bool hasRawPage = false;
                        //    foreach (FileInfo rawPageFile in rawPageFiles)
                        //    {
                        //        if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                        //            !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                        //            !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                        //        {
                        //            if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                        //            {
                        //                //path = Util.CreatePdfFileFromImage(path);
                        //                pageList.Add(rawPageFile.FullName);
                        //                hasRawPage = true;
                        //            }
                        //        }
                        //    }

                        //    if (!hasRawPage)
                        //    {
                        //        FileInfo[] rawPagePdfFiles = rawPageDir.GetFiles("*_s.pdf");

                        //        if (rawPagePdfFiles.Length > 0)
                        //            pageList.Add(rawPagePdfFiles[0].FullName);
                        //    }
                        //}
                        #endregion

                        #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
                        DirectoryInfo rawPageDirs = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);
                        FileInfo[] rawPageFiles = rawPageDirs.GetFiles();

                        bool hasRawPage = false;
                        foreach (FileInfo rawPageFile in rawPageFiles)
                        {
                            if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                                !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                                !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                            {
                                if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                                {
                                    //path = Util.CreatePdfFileFromImage(path);
                                    pageList.Add(rawPageFile.FullName);
                                    hasRawPage = true;
                                }
                            }
                        }

                        if (!hasRawPage)
                        {
                            FileInfo[] rawPagePdfFiles = rawPageDirs.GetFiles("*_s.pdf");

                            if (rawPagePdfFiles.Length > 0)
                                pageList.Add(rawPagePdfFiles[0].FullName);
                        }                        
                        #endregion
                    }

                    if (!Directory.Exists(saveDir))
                        Directory.CreateDirectory(saveDir);

                    if (pageList.Count > 0)
                    {
                        string docTypeDesc = r.DocTypeCode;

                        DocType.DocTypeDataTable docTypeTable = docTypeDb.GetDocType(r.DocTypeCode);

                        if (docTypeTable.Rows.Count > 0)
                        {
                            DocType.DocTypeRow docType = docTypeTable[0];
                            docTypeDesc = docType.Description;
                        }

                        string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + r.Id.ToString() + ".pdf");

                        try
                        {
                            if (File.Exists(mergedFileName))
                                File.Delete(mergedFileName);
                        }
                        catch (Exception)
                        {
                        }

                        Util.MergePdfFiles(pageList, mergedFileName);

                        docList.Add(mergedFileName);
                    }
                }
            }

            if (docList.Count > 0)
            {
                string setNo = docSetDb.GetSetNumber(setId);

                string mergedFileName = Path.Combine(saveDir, setNo + "_" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.yyMMdd) + ".pdf");

                try { if (File.Exists(mergedFileName)) File.Delete(mergedFileName); }
                catch (Exception) { }

                string errorMessage = string.Empty;
                Util.MergePdfFiles(docList, mergedFileName, out errorMessage);

                if (String.IsNullOrEmpty(errorMessage))
                {
                    FileInfo mergedPdf = new FileInfo(mergedFileName);
                    if (mergedPdf.Exists)
                        return mergedPdf.FullName;
                    else
                    {
                        errorMsg = "Compiled PDF File Cannot Be Found.";
                        return null;
                    }
                }
                else
                {
                    errorMsg = errorMessage;
                    return null;
                }
            }
            else
            {
                errorMsg = "No documents were found.";
                return null;
            }
        }

        public static string GenerateZipPathBySetId(int setId, out string errorMsg)
        {
            string saveDir = HttpContext.Current.Server.MapPath("~/App_Data/Temp/");
            errorMsg = string.Empty;

            //DocDb docDb = new DocDb();
            //DocAppDb docAppDb = new DocAppDb();
            //DocSetDb docSetDb = new DocSetDb();
            //RawPageDb rawPageDb = new RawPageDb();
            //DocTypeDb docTypeDb = new DocTypeDb();

            //string rawPageDirPath = HttpContext.Current.Server.MapPath(Retrieve.GetRawPageOcrDirPath());
            //DirectoryInfo rawPageDirInfo = new DirectoryInfo(rawPageDirPath);
            //string setNo = docSetDb.GetSetNumber(setId);
            //string dirName = saveDir + setNo + "_" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt);
            ;
            string zipFullName = Util.GeneratePdfPathBySetId(setId, out errorMsg);
            //saveDir + string.Format("{0}_{1}.zip", setNo, Format.FormatDateTime(DateTime.Now, DateTimeFormat.dMMMyyyyhmmtt));
            //Doc.DocDataTable docTable = docDb.GetDocByDocSetId(setId);

            //ArrayList docList = new ArrayList();
            //if (docTable.Rows.Count > 0)
            //{
            //    if (Directory.Exists(dirName)) Directory.Delete(dirName, true);
            //    Directory.CreateDirectory(dirName);
            //    foreach (Doc.DocRow r in docTable.Rows)
            //    {
            //        ArrayList pageList = new ArrayList();

            //        RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(r.Id);
            //        for (int cnt = 0; cnt < rawPages.Count; cnt++)
            //        {
            //            RawPage.RawPageRow rawPage = rawPages[cnt];
            //            DirectoryInfo[] rawPageDirs = rawPageDirInfo.GetDirectories(rawPage.Id.ToString());

            //            if (rawPageDirs.Length > 0)
            //            {
            //                DirectoryInfo rawPageDir = rawPageDirs[0];

            //                FileInfo[] rawPageFiles = rawPageDir.GetFiles();

            //                foreach (FileInfo rawPageFile in rawPageFiles)
            //                {
            //                    if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
            //                        !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
            //                        !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
            //                    {
            //                        pageList.Add(rawPageFile.FullName);
            //                        File.Copy(rawPageFile.FullName, dirName + "/(" + r.Id + ") " + r.DocTypeCode + "-" + rawPageFile.Name);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //Directory.Delete(dirName, true);

            FileInfo zippedFile = new FileInfo(zipFullName);
            if (zippedFile.Exists)
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFile(zipFullName);
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    //zip.Comment = "This zip was created programmatically at " + System.DateTime.Now.ToString("G");
                    zip.Save(zipFullName + ".zip");
                    //Response.Clear();
                    //Response.ContentType = "application/zip";
                    //Response.AddHeader("content-disposition", "filename=" + zippedFile.Name + ".zip");
                    //zip.Save(Response.OutputStream);
                }
                return zippedFile.FullName;
            }
            else
            {
                errorMsg = "Zipped File Cannot Be Found.";
                return null;
            }
        }


        public static bool SendMailWithCreatedAttachment(string senderName, string senderEmail,
            string recipientEmail, string ccEmail, string bCCEmail, string replyToEmail,
            string subject, string message, string attachmentPath)
        {
            ParameterDb parameterDb = new ParameterDb();

            char[] delimiterChars = { ';', ',', ':' };
            bool sent = false;
            string from = String.Format("{0} <{1}>", senderName, senderEmail);

            string hostUrl = GetHostUrl();
            string cr = Environment.NewLine;            
            
            message = message.Replace(cr, "<br />");
            message = message.Replace("\n\r", "<br />");
            message = message.Replace("\r\n", "<br />");
            message = message.Replace("\n", "<br />");
            message = message.Replace("\r", "<br />");

            if (parameterDb.GetParameter(ParameterNameEnum.RedirectAllEmailsToTestMailingList).Trim().ToUpper() == "YES")
            {
                subject = subject + " (UAT Email)";
                message += cr + cr + "<br /><br />-----<br /><br />This is a UAT email from " + Util.GetHostUrl() + ". The original recipients are: " 
                    + recipientEmail;

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    message = message + ", CC: " + ccEmail;
                }

                //recipientEmail = "lexin.pan@hiend.com;wintwah.toe@hiend.com;peter.zhou@hiend.com;matthew.narca@hiend.com;ns.subashini@hiend.com";
                recipientEmail = parameterDb.GetParameter(ParameterNameEnum.TestMailingList).Trim();
                ccEmail = string.Empty;
            }

            try
            {
                MailMessage m = new MailMessage();

                // From
                m.From = new MailAddress(from);

                // To
                string[] toEmails = recipientEmail.Split(delimiterChars);
                foreach (string s in toEmails)
                    if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                        m.To.Add(new MailAddress(s.Trim()));

                // CC
                if (ccEmail != null && ccEmail.Trim() != string.Empty)
                {
                    string[] ccEmails = ccEmail.Split(delimiterChars);
                    foreach (string s in ccEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.CC.Add(new MailAddress(s.Trim()));
                }

                // BCC

                if (bCCEmail != null && bCCEmail.Trim() != string.Empty)
                {
                    string[] bCCEmails = bCCEmail.Split(delimiterChars);
                    foreach (string s in bCCEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.Bcc.Add(new MailAddress(s.Trim()));
                }

                // Reply
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    m.ReplyTo = new MailAddress(replyToEmail);
                }

                //Attachment

                m.Attachments.Clear();

                if (!String.IsNullOrEmpty(attachmentPath.Trim()))
                {
                    if (File.Exists(attachmentPath))
                    {
                        MemoryStream attachmentStream = new MemoryStream(File.ReadAllBytes(attachmentPath));
                        if (attachmentStream.Length > 0)
                        {
                            m.Attachments.Add(new Attachment(attachmentPath));
                        }
                    }
                }

                message = FormatBody(message);

                m.IsBodyHtml = true;
                m.Subject = subject;
                m.Body = message;
                SmtpClient client = new SmtpClient();
                client.Timeout = 360000;
                client.Send(m);
                sent = true;
                m.Dispose();
            }
            catch (Exception ex)
            {
                sent = false;
            }
            finally
            {
                if (!String.IsNullOrEmpty(attachmentPath.Trim()))
                {
                    if (File.Exists(attachmentPath))
                       File.Delete(attachmentPath);
                }
            }
            return sent;
        }

        #region Added By Edward Batch Upload
        public static bool SendMailBU(string senderName, string senderEmail,
            string recipientEmail, string ccEmail, string bCCEmail, string replyToEmail,
            string subject, string message, string filePathOfLogFile)
        {
            ParameterDb parameterDb = new ParameterDb();

            char[] delimiterChars = { ';', ',', ':' };
            bool sent = false;
            string from = String.Format("{0} <{1}>", senderName, senderEmail);

            //string hostUrl = GetHostUrl();
            string cr = Environment.NewLine;

            message = message.Replace(cr, "<br />");
            message = message.Replace("\n\r", "<br />");
            message = message.Replace("\r\n", "<br />");
            message = message.Replace("\n", "<br />");
            message = message.Replace("\r", "<br />");

            if (parameterDb.GetParameter(ParameterNameEnum.RedirectAllEmailsToTestMailingList).Trim().ToUpper() == "YES")
            {
                subject = subject + " (UAT Email)";
                message += cr + cr + "<br /><br />-----<br /><br />This is a UAT email. The original recipients are: "
                    + recipientEmail;

                if (!string.IsNullOrEmpty(ccEmail))
                {
                    message = message + ", CC: " + ccEmail;
                }

                //recipientEmail = "lexin.pan@hiend.com;wintwah.toe@hiend.com;peter.zhou@hiend.com;matthew.narca@hiend.com;ns.subashini@hiend.com";
                recipientEmail = parameterDb.GetParameter(ParameterNameEnum.TestMailingList).Trim();
                ccEmail = string.Empty;
            }

            try
            {
                MailMessage m = new MailMessage();

                // From
                m.From = new MailAddress(from);

                // To
                string[] toEmails = recipientEmail.Split(delimiterChars);
                foreach (string s in toEmails)
                    if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                        m.To.Add(new MailAddress(s.Trim()));

                // CC
                if (ccEmail != null && ccEmail.Trim() != string.Empty)
                {
                    string[] ccEmails = ccEmail.Split(delimiterChars);
                    foreach (string s in ccEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.CC.Add(new MailAddress(s.Trim()));
                }

                // BCC

                if (bCCEmail != null && bCCEmail.Trim() != string.Empty)
                {
                    string[] bCCEmails = bCCEmail.Split(delimiterChars);
                    foreach (string s in bCCEmails)
                        if (s != null && s.Trim() != string.Empty && Validation.IsEmail(s.Trim()))
                            m.Bcc.Add(new MailAddress(s.Trim()));
                }

                // Reply
                if (!string.IsNullOrEmpty(replyToEmail))
                {
                    m.ReplyTo = new MailAddress(replyToEmail);
                }                            

                message = FormatBody(message);

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
        #endregion

        #region Added By Edward 2014/08/20  Stop Notification Email to Sales and Sers
        /// <summary>
        /// Gets the section using the initials of the user email, eg: olh@ABCDE.gov
        /// </summary>
        /// <param name="userInitials"></param>
        /// <returns></returns>
        public static string GetUserSectionByEmail(string userInitials)
        {
            string BusinessCode = string.Empty;
            DataTable dt = ProfileDb.GetProfileInfoByUserInitials(userInitials);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                BusinessCode = row["BusinessCode"].ToString();
            }
            return BusinessCode;
        }
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static ArrayList MergeDocumentsToTemp(string idsStr, string saveDir)
        {
            DocDb docDb = new DocDb();
            RawPageDb rawPageDb = new RawPageDb();
            DocTypeDb docTypeDb = new DocTypeDb();

            string[] ids = idsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ArrayList docList = new ArrayList();

            foreach (string idStr in ids)
            {
                int id = int.Parse(idStr);

                Doc.DocDataTable docTable = docDb.GetDocById(id);

                if (docTable.Rows.Count > 0)
                {
                    Doc.DocRow doc = docTable[0];

                    ArrayList pageList = new ArrayList();

                    RawPage.RawPageDataTable rawPages = rawPageDb.GetRawPageByDocId(id);

                    for (int cnt = 0; cnt < rawPages.Count; cnt++)
                    {

                        RawPage.RawPageRow rawPage = rawPages[cnt];   
                        DirectoryInfo rawPageDirs = Util.GetIndividualRawPageOcrDirectoryInfo(rawPage.Id);

                        //Added if (rawPageDirs.Exists) by Edward 2017/10/25 
                        //to address ERROR Could not find a part of the path 'E:\Websites\Go-DWMS\App_Data\Doc\2013\10\1\RawPage\2249441'
                        if (rawPageDirs.Exists)
                        {

                            // Get the raw page for download
                            FileInfo[] rawPageFiles = rawPageDirs.GetFiles();

                            bool hasRawPage = false;
                            foreach (FileInfo rawPageFile in rawPageFiles)
                            {
                                if (!rawPageFile.Extension.ToUpper().Equals(".DB") &&
                                    !rawPageFile.Name.ToUpper().EndsWith("_S.PDF") &&
                                    !rawPageFile.Name.ToUpper().EndsWith("_TH.JPG"))
                                {
                                    if (rawPageFile.Extension.ToUpper().Equals(".PDF"))
                                    {
                                        //path = Util.CreatePdfFileFromImage(path);
                                        pageList.Add(rawPageFile.FullName);
                                        hasRawPage = true;
                                    }
                                }
                            }

                            // If the raw page is not found, use the searcheable PDF
                            if (!hasRawPage)
                            {
                                FileInfo[] rawPagePdfFiles = rawPageDirs.GetFiles("*_s.pdf");

                                if (rawPagePdfFiles.Length > 0)
                                    pageList.Add(rawPagePdfFiles[0].FullName);
                            }
                        }                        
                    }

                    if (!Directory.Exists(saveDir))
                        Directory.CreateDirectory(saveDir);

                    if (pageList.Count > 0)
                    {
                        string docTypeDesc = doc.DocTypeCode;

                        DocType.DocTypeDataTable docTypeTable = docTypeDb.GetDocType(doc.DocTypeCode);

                        if (docTypeTable.Rows.Count > 0)
                        {
                            DocType.DocTypeRow docType = docTypeTable[0];
                            docTypeDesc = docType.Description;
                        }

                        string mergedFileName = Path.Combine(saveDir, docTypeDesc.Replace("/", "_") + " - " + doc.Id.ToString() + ".pdf");

                        try
                        {
                            if (File.Exists(mergedFileName))
                                File.Delete(mergedFileName);
                        }
                        catch (Exception)
                        {
                        }

                        string errorMessage = string.Empty;

                        Util.MergePdfFiles(pageList, mergedFileName, out errorMessage);

                        docList.Add(mergedFileName);
                    }
                }
            }
            return docList;
                
        }

        #endregion
    }    
}
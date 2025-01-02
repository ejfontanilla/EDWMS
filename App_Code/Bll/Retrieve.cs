using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Collections.Generic;

namespace Dwms.Bll
{
    public sealed class Retrieve
    {
        public static string GetPageName()
        {
            return System.IO.Path.GetFileName(
                HttpContext.Current.Request.PhysicalPath);
        }

        public static string GetFileType(string fileName)
        {
            string fileType = "File";

            switch (Path.GetExtension(fileName))
            {
                case ".cs":
                    fileType = "Visual C# Source File";
                    break;

                case ".css":
                    fileType = "Cascading Style Sheet Document";
                    break;

                case ".html":
                    fileType = "HTML Document";
                    break;

                case ".resx":
                    fileType = "ASP.NET Resource File";
                    break;

                case ".vb":
                    fileType = "Visual Basic Source File";
                    break;

                case ".xml":
                    fileType = "XML Document";
                    break;

                case ".ascx":
                    fileType = "ASP.NET User Control";
                    break;

                case ".aspx":
                    fileType = "ASP.NET Server Page";
                    break;

                case ".gif":
                    fileType = "GIF Image";
                    break;

                case ".jpg":
                    fileType = "JPEG Image";
                    break;

                case ".js":
                    fileType = "JScript Script File";
                    break;

                case ".master":
                    fileType = "ASP.NET Master Page";
                    break;

                case ".zip":
                    fileType = "Compressed (zipped) Folder";
                    break;

            }

            return fileType;
        }

        public static string GetContentType(string fileName)
        {
            string contentType = "File";

            switch (Path.GetExtension(fileName))
            {
                case ".htm":
                case ".html":
                    contentType = "text/html";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".png":
                    contentType = "image/x-png";
                    break;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                    contentType = "image/jpeg";
                    break;
                case ".tiff":
                case ".tif":
                    contentType = "image/tiff";
                    break;
                case ".bmp":
                    contentType = "image/x-ms-bmp";
                    break;
                case ".wav":
                    contentType = "audio/x-wav";
                    break;
                case ".mpeg":
                case ".mpg":
                case ".mpe":
                    contentType = "video/mpeg";
                    break;
                case ".mpv2":
                case ".mp2v":
                    contentType = "  video/mpeg-2";
                    break;
                case ".qt":
                case ".mov":
                    contentType = "video/quicktime";
                    break;
                case ".avi":
                    contentType = "video/x-msvideo";
                    break;
                case ".movie":
                    contentType = "video/x-sgi-movie";
                    break;
                case ".ai":
                case ".eps":
                case ".ps":
                    contentType = "application/postscript";
                    break;
                case ".rtf":
                    contentType = "application/rtf";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".dvi":
                    contentType = "application/x-dvi";
                    break;
                case ".tar":
                    contentType = "application/x-tar";
                    break;
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".js":
                    contentType = "text/javascript";
                    break;
                case ".ppt":
                case ".pptx":
                    contentType = "application/vnd.ms-powerpoint";
                    break;
                case ".doc":
                case ".docx":
                    contentType = "application/msword";
                    break;
                case ".xls":
                case ".xlsx":
                    contentType = "application/vnd.ms-excel";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            return contentType;
        }

        public static string GetFileExtension(string filePath)
        {
            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
            string format = "UnknownFormat";

            if (fileInfo.Exists)
            {
                string ext = fileInfo.Extension.ToUpper();
                ext = ext.Replace(".", string.Empty);

                switch (ext)
                {
                    case ("DOC"):
                        format = ext;
                        break;
                    case ("PDF"):
                        format = ext;
                        break;
                    case ("XLS"):
                        format = ext;
                        break;
                    case ("PPT"):
                        format = ext;
                        break;
                    case ("RTF"):
                        format = ext;
                        break;
                    case ("TXT"):
                        format = ext;
                        break;
                    case ("JPG"):
                        format = ext;
                        break;
                    case ("JPEG"):
                        format = ext;
                        break;
                    case ("GIF"):
                        format = ext;
                        break;
                    case ("BMP"):
                        format = ext;
                        break;
                }
            }

            return format;
        }

        public static string GetCountryName(string countryCode)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("~/App_Data/country-list.xml"));
            XmlNodeList nodeList = doc.SelectNodes("countrylist/country");

            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes["value"].Value == countryCode)
                    return node.Attributes["text"].Value;
            }

            return null;
        }

        public static string GetEmailName(string email, int maxLen)
        {
            if (String.IsNullOrEmpty(email) || email.Length <= maxLen)
                return email;
            else
            {
                if (email.Contains("@"))
                    email = email.Substring(0, email.IndexOf("@"));

                if (email.Length > maxLen)
                    email = email.Substring(0, maxLen);

                return email;
            }
        }

        public static string GetFilesFolder()
        {
            return ConfigurationManager.AppSettings["filesFolder"];
        }

        public static string GetArchiveAuditFolder()
        {
            return ConfigurationManager.AppSettings["archiveAuditFolder"];
        }

        public static HttpCookie GetCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            return (cookie == null) ? new HttpCookie(cookieName) : cookie;
        }

        public static int GetCookieLifespan()
        {
            return int.Parse(ConfigurationManager.AppSettings["cookieExpires"]);
        }

        public static string GetImageForExtension(string fileName)
        {
            string folder = "~/Data/Images/Files/";
            string ext = Path.GetExtension(fileName).ToLower();
            string image = folder + "file.gif";

            if (string.IsNullOrEmpty(ext))
                image = folder + "folder.gif";
            else
            {
                ext = ext.Substring(1);
                image = folder + ext + ".gif";

                if (!File.Exists(HttpContext.Current.Server.MapPath(image)))
                    image = folder + "file.gif";
            }

            return image;
        }

        /// <summary>
        /// Retrieve the UserData from the FormsAuthenticationTicket
        /// </summary>
        /// <returns></returns>
        public static string GetFormsAuthenticationUserData()
        {
            // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
            // Source: http://www.dotnetfunda.com/articles/article141.aspx
            FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = identity.Ticket;

            return ticket.UserData;
        }

        /// <summary>
        /// Return the path of the Scanned Images main dir
        /// </summary>
        /// <returns></returns>
        public static string GetScannedDocsMainDirPath()
        {
            return ConfigurationManager.AppSettings["ScannedDocsFolder"].ToString();
        }

        /// <summary>
        /// Return the path of the Uploaded Images main dir
        /// </summary>
        /// <returns></returns>
        public static string GetUploadedDocsMainDirPath()
        {
            return ConfigurationManager.AppSettings["UploadedDocsFolder"].ToString();
        }

        /// <summary>
        /// Return the path of the Images to be OCR'ed
        /// </summary>
        /// <returns></returns>
        public static string GetDocsForOcrDirPath()
        {
            return ConfigurationManager.AppSettings["DocsForOcrFolder"].ToString();
        }

        public static string GetBarcodePath()
        {
            return ConfigurationManager.AppSettings["BarcodeFolder"].ToString();
        }

        public static string GetRawPageOcrDirPath()
        {
            return ConfigurationManager.AppSettings["RawPageOcrFolder"].ToString();
        }

        /// <summary>
        /// Return the path of the Sample docs folder
        /// </summary>
        /// <returns></returns>
        public static string GetSampleDocsMainDirPath()
        {
            return ConfigurationManager.AppSettings["SampleForOcrDocsFolder"].ToString();
        }

        /// <summary>
        /// Return the path of the OCR result folder for the sample docs
        /// </summary>
        /// <returns></returns>
        public static string GetSampleDocsOcrResultMainDirPath()
        {
            return ConfigurationManager.AppSettings["SampleOcrResultDocsFolder"].ToString();
        }

        /// <summary>
        /// Get the OCR License
        /// </summary>
        /// <returns></returns>
        public static string GetOcrLicense()
        {
            return ConfigurationManager.AppSettings["OcrLicense"].ToString();
        }

        public static string GetFileName(string filePath)
        {
            FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
            return fileInfo.Name;
        }

        public static string GetFileName2(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Name;
        }

        /// <summary>
        /// Get the Download Set folder
        /// </summary>
        /// <returns></returns>
        public static string GetDownloadSetDirPath()
        {
            return ConfigurationManager.AppSettings["DownloadSetFolder"].ToString();
        }

        public static string GetLoggedInUserName()
        {
            string userName = String.Empty;
            MembershipUser user = Membership.GetUser();

            if (user != null)
            {
                userName = user.UserName;
            }

            return userName;
        }

        public static string GetTempDirPath()
        {
            return ConfigurationManager.AppSettings["TempFolder"].ToString();
        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public static string GetWebServiceDocsDirPath()
        {
            return ConfigurationManager.AppSettings["WebServiceDocsFolder"].ToString();
        }

        #region Modified by Edward 2015/3/25 Logfiles to be saved in LogFiles Folder instead to WebService
        public static string GetLogFilesFolderDirPath()
        {
            return ConfigurationManager.AppSettings["LogFilesFolder"].ToString();
        }
        #endregion

        public static string GetEmailDomain()
        {
            return ConfigurationManager.AppSettings["EmailDomain"].ToString();
        }

        public static string GetDWMSDomain()
        {
            return ConfigurationManager.AppSettings["DWMSDomain"].ToString();
        }

        #region Added by Edward to redirect to new web application 2017/10/05
        public static string GetDWMSRedirectToURLDefault()
        {
            return ConfigurationManager.AppSettings["DWMSRedirectToURLDefault"].ToString();
        }
        #endregion

        public static string GetABCDECreditEmailAddress()
        {
            return ConfigurationManager.AppSettings["ABCDECreditEmailAddress"].ToString();
        }

        /// <summary>
        /// Get IdType by nric
        /// </summary>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static string GetIdTypeByNRIC(string nric)
        {
            if (string.IsNullOrEmpty(nric))
                return string.Empty;
            else if (Validation.IsNric(nric))
                return IDTypeEnum.UIN.ToString();
            else if (Validation.IsFin(nric))
                return IDTypeEnum.FIN.ToString();
            else
                return IDTypeEnum.XIN.ToString();
        }

        /// <summary>
        /// Return the path of the Images to be OCR'ed
        /// </summary>
        /// <returns></returns>
        public static string GetSpellCheckerDirPath()
        {
            return ConfigurationManager.AppSettings["SpellCheckLibraryFolder"].ToString();
        }


        public static void GetHunspellResourcesPath(out string libAffPath, out string libDicPath)
        {
            libAffPath = Path.Combine(GetSpellCheckerDirPath(), "en_US.aff");
            libDicPath = Path.Combine(GetSpellCheckerDirPath(), "en_US.dic");
        }

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static string GetDocDirPath()
        {
            return ConfigurationManager.AppSettings["docFolder"].ToString();
        }

        #endregion

        #region Added by Edward 2017/10/23 Fix Redirection to different app pools per group
        //http://blog.ginzburgconsulting.com/getting-current-page-url-or-its-parts-in-c/
        
        //http://localhost:12345/site/page.aspx?q1=1&q2=2

        //Value of HttpContext.Current.Request.Url.Host
        //localhost

        //Value of HttpContext.Current.Request.Url.Authority
        //localhost:12345

        // Value of HttpContext.Current.Request.Url.AbsolutePath
        // /site/page.aspx

        // Value of HttpContext.Current.Request.ApplicationPath
        // /site

        //Value of HttpContext.Current.Request.Url.AbsoluteUri
        //http://localhost:12345/site/page.aspx?q1=1&q2=2

        // Value of HttpContext.Current.Request.RawUrl
        // /site/page.aspx?q1=1&q2=2

        // Value of HttpContext.Current.Request.Url.PathAndQuery
        // /site/page.aspx?q1=1&q2=2
        
        public static string GetDWMSUrl(string strHost, string strApplicationPath, string[] strSegments, string strQuery)
        {
            string result = "NA";            
            //For Group users like GO-DWMSG11 group of Susanna
            if (strApplicationPath.ToLower().Contains("go-dwmsg"))
                return result;

            //For Sales Group users like GO-DWMSSS1 group
            if (strApplicationPath.ToLower().Contains("go-dwmss"))
                return result;

            //For VPN Users
            if (!strHost.ToLower().Contains(GetDWMSServerName()))
                return result;


            string PageAndQuery = string.Empty;
            if (strSegments.Length > 0)
            {
                for (int i = 2; i < strSegments.Length; i++)
                {
                    PageAndQuery = PageAndQuery + strSegments[i];
                }

                PageAndQuery = PageAndQuery + strQuery;
            }

            //checks if host and application path are not the same, then redirect
            string defaultURL = GetDWMSRedirectToURLDefault().ToLower();

            if (defaultURL.Equals("na"))
                return result;

            if (!defaultURL.Equals("http://" + strHost.ToLower() + strApplicationPath.ToLower() + "/"))
                result = GetDWMSRedirectToURLDefault() + PageAndQuery;

            return result;
        }

        public static string GetDWMSServerName()
        {
            return ConfigurationManager.AppSettings["DWMSServerName"].ToString();
        }
        #endregion
    }
}

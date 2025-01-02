using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Dwms.Bll
{
    public enum DateTimeFormat
    {
        yyMMdd,
        dd_MM_yy,
        dd_Hyp_MM_Hyp_yyyy,
        dd_MM_yyyy,
        dd__MMM__yyyy,
        dd__MMM__yy,
        ddd_C_d__MMM__yyyy,
        dMMMyyyyhmmtt,
        d__MMM__yyyy_C_h_Col_mm__tt,
        yyyyMMdd_dash_HHmmss
    }

    public sealed class Format
    {
        public static string FormatEnumValue(object enumValue)
        {
            if (enumValue == null)
                return string.Empty;
            else
                return enumValue.ToString().Replace("_", " ");
        }

        public static string HTMLBreakEncode(string text)
        {
            if (text == null || text.Length == 0)
                return text;
            else
            {
                text = text.Replace("\r\n", "<br />");
                text = text.Replace("\n\r", "<br />");
                text = text.Replace("\r", "<br />");
                text = text.Replace("\n", "<br />");
                return text;
            }
        }

        public static string FormatFileSize(long bytes)
        {
            decimal b = 1024m;
            decimal kb = 1048576m;
            decimal mb = 1073741824m;

            if (bytes == 0)
                return "0 KB";
            else if (bytes < b)
                return "1 KB";
            else if (bytes < kb)
                return Math.Ceiling(bytes / b).ToString() + " KB";
            else if (bytes < mb)
                return Math.Ceiling(bytes / kb).ToString() + " MB";
            else
                return Math.Ceiling(bytes / mb).ToString() + " GB";
        }

        public static string FormatFileSize(double bytes)
        {
            string[] Suffix = { "Bytes", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = 0;
            for (i = 0; (int)(bytes / 1024) > 0; i++, bytes /= 1024)
                dblSByte = bytes / 1024.0;
            return String.Format("{0:0.00} {1}", dblSByte, Suffix[i]);
        }

        public static string RemoveHTMLTags(string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }

        public static string FormatDateTime(object date, DateTimeFormat type)
        {
            string typeStr = type.ToString();

            if (string.IsNullOrEmpty(typeStr))
                return null;
            else
            {
                typeStr = typeStr.Replace("_dash_", "-");
                typeStr = typeStr.Replace("_C_", ", ");
                typeStr = typeStr.Replace("_Col_", ":");
                typeStr = typeStr.Replace("_Hyp_", "-");
                typeStr = typeStr.Replace("__", " ");
                typeStr = typeStr.Replace("_", "/");
            }

            string dateString = String.Format("{0:" + typeStr + "}", date);
            return dateString;

            //switch (type)
            //{
            //    case ("TinyDate"):
            //        dateString = String.Format("{0:dd/MM/yy}", date);
            //        break;
            //    case ("TinyDate"):
            //        dateString = String.Format("{0:dd/MM/yy}", date);
            //        break;
            //    case ("TinyDateTime"):
            //        dateString = String.Format("{0:dd/MM/yy h:mm tt}", date);
            //        break;
            //    case ("ShortDate"):
            //        dateString = String.Format("{0:d MMM yyyy}", date);
            //        break;
            //    case ("ShortDateEqualLen"):
            //        dateString = String.Format("{0:dd MMM yyyy}", date);
            //        break;
            //    case ("LongDate"):
            //        dateString = String.Format("{0:d MMMM yyyy}", date);
            //        break;
            //    case ("LongDateTime"):
            //        dateString = String.Format("{0:d MMMM yyyy, h:mm tt}", date);
            //        break;
            //    case ("LongDateTimeEqualLen"):
            //        dateString = String.Format("{0:dd MMMM yyyy, h:mm tt}", date);
            //        break;
            //    case ("MediumDate"):
            //        dateString = String.Format("{0:d MMM yyyy}", date);
            //        break;
            //    case ("MediumDateTime"):
            //        dateString = String.Format("{0:d MMM yyyy, h:mm tt}", date);
            //        break;
            //    case ("ShortDateTime"):
            //        dateString = String.Format("{0:d MMM yy, h:mm tt}", date);
            //        break;
            //    case ("ShortDateTimeEqualLen"):
            //        dateString = String.Format("{0:dd MMM yy, h:mm tt}", date);
            //        break;
            //    case ("AMPMTime"):
            //        dateString = String.Format("{0:h:mm tt}", date);
            //        break;
            //    case ("MonthYear"):
            //        dateString = String.Format("{0:MMMM yyyy}", date);
            //        break;
            //    case ("WeekdayLongDate"):
            //        dateString = String.Format("{0:dddd, d MMMM yyyy}", date);
            //        break;
            //    case ("WeekdayShortDate"):
            //        dateString = String.Format("{0:ddd, d MMM yyyy}", date);
            //        break;
            //    case ("WeekdayTinyDate"):
            //        dateString = String.Format("{0:ddd, d MMM yy}", date);
            //        break;
            //    case ("WeekdayShortDateTime"):
            //        dateString = String.Format("{0:ddd, d MMM yyyy, h:mm tt}", date);
            //        break;
            //    case ("WeekdayLongDateTime"):
            //        dateString = String.Format("{0:dddd, d MMMM yyyy, h:mm tt}", date);
            //        break;
            //    case ("WeekdayMediumDateTime"):
            //        dateString = String.Format("{0:ddd, d MMM yyyy, h:mm tt}", date);
            //        break;
            //    default:
            //        dateString = date.ToString();
            //        break;
            //}

        }

        public static DateTime GetDatePart(DateTime date)
        {
            return DateTime.Parse(FormatDateTime(date, DateTimeFormat.dd__MMM__yy));
        }

        public static string ToAbsoluteUrl(string url)
        {
            if (url != null)
            {
                url = url.ToLower();

                if (!url.StartsWith("http://"))
                {
                    if (url.StartsWith("www."))
                        url = "http://" + url;
                    else if (url.StartsWith("/"))
                        url = "~" + url;
                    else
                        url = "~/" + url;
                }
            }

            return url;
        }

        public static string Truncate(string s, int maxLen)
        {
            if (s == null || s.Length <= maxLen)
                return s;
            else
                return s.Substring(0, maxLen) + "...";
        }

        public static string HighlightKeywordInResult(string keyword, string result)
        {
            if (String.IsNullOrEmpty(keyword) || String.IsNullOrEmpty(result))
                return result;

            int start = 0;
            int end = 0;
            string s = null;

            while (end >= 0)
            {
                end = result.IndexOf(keyword, start, StringComparison.OrdinalIgnoreCase);

                if (end >= 0)
                {
                    s = s + result.Substring(start, end - start) + "<b>" + result.Substring(end, keyword.Length) + "</b>";
                    start = end + keyword.Length;
                }
            }

            s = s + result.Substring(start, result.Length - start);
            return s;
        }

        public static string FormatCurrenty(object amount)
        {
            return String.Format("S${0:n}", amount);
        }

        public static string TrimZeros(object o)
        {
            return o.ToString().TrimEnd('0', CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]);
        }

        public static string FormatPageTitle(string pageTitle)
        {
            string crisisSiteName = "OTS";

            if (string.IsNullOrEmpty(pageTitle))
                return crisisSiteName;
            else
                return pageTitle + " - " + crisisSiteName;
        }

        public static string FormatSetNumber(int setNumber)
        {
            string setNumberStr = setNumber.ToString("D5");
            string date = DateTime.Now.ToString("yyMMdd");
            setNumberStr = "EACO" + date + "-" + setNumberStr;
            return setNumberStr;
        }

        public static string DocTypeShortToLong(string docType)
        {
            string docTypeName;

            switch (docType)
            {
                case "HLE":
                    docTypeName = "HLE Application Form";
                    break;
                case "PAYSLIP":
                    docTypeName = "Payslip";
                    break;
                case "TAX":
                    docTypeName = "Notice of Assessment";
                    break;
                case "ROC":
                    docTypeName = "Registration of Company/Business";
                    break;
                case "CPF":
                    docTypeName = "CPF Contribution History Statement";
                    break;
                default:
                    docTypeName = "Unidentified";
                    break;
            }

            return docTypeName;
        }

        public static string DocTypeLongToShort(string docType)
        {
            string docTypeName;

            switch (docType)
            {
                case "HLE Application Form":
                    docTypeName = "HLE";
                    break;
                case "Payslip":
                    docTypeName = "PAYSLIP";
                    break;
                case "Notice of Assessment":
                    docTypeName = "TAX";
                    break;
                case "Registration of Company/Business":
                    docTypeName = "ROC";
                    break;
                case "CPF Contribution History Statement":
                    docTypeName = "CPF";
                    break;
                default:
                    docTypeName = "Unidentified";
                    break;
            }

            return docTypeName;
        }

        /// <summary>
        /// Format the keywords for the Categorisation Rules
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static string FormatKeywords(string keywords)
        {
            string result = string.Empty;

            string[] keywordsArray = keywords.Split(new string[] { Constants.KeywordSeperator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string keyword in keywordsArray)
            {
                // If the keyword is a variable, do not surround with quotes (")
                string keywordFormatted = (Validation.IsKeywordVariable(keyword) ? keyword : String.Format("\"{0}\"", keyword));

                result = (String.IsNullOrEmpty(result) ? keywordFormatted : 
                    result + " " + CategorisationRuleOpeatorEnum.and.ToString() + " " + keywordFormatted);
            }

            return result;
        }

        /// <summary>
        /// Replace double space with single space
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceDoubleSpaceWithSingleSpace(string input)
        {
            string formatted = input.Trim();

            string oneSpace = " ";
            string twoSpace = "  ";

            while (formatted.IndexOf(twoSpace) > -1)
            {
                formatted = formatted.Replace(twoSpace, oneSpace);
            }

            return formatted;
        }

        /// <summary>
        /// Create the Set number
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="operationCode"></param>
        /// <param name="dateIn"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static string FormatSetNumber(string groupCode, string operationCode, DateTime dateIn, int sequence)
        {
            //return String.Format(Constants.SetNumberFormat, groupCode.ToUpper(), operationCode.ToUpper(), FormatDateTime(dateIn, DateTimeFormat.yyMMdd), sequence.ToString().PadLeft(5, '0'));
            return String.Format(Constants.SetNumberFormat, "EA", operationCode.ToUpper(), FormatDateTime(dateIn, DateTimeFormat.yyMMdd), sequence.ToString().PadLeft(5, '0'));
        }

        public static string RemoveNonAlphanumericCharacters(string input)
        {
            string temp = input.Trim();

            // Source: http://stackoverflow.com/questions/3210393/how-to-remove-all-non-alphanumeric-characters-from-a-string-except-dash
            Regex regex = new Regex("[^\\w]*");
            temp = regex.Replace(temp, string.Empty);

            return temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        public static string GetMetaDataDateFormat()
        {
            return DateTimeFormat.dd_Hyp_MM_Hyp_yyyy.ToString().Replace("_Hyp_", "-");
        }

        public static string GetMetaDataValueInMetaDataDateFormat(string dateValue)
        {
            try
            {
                DateTime result = new DateTime();
                if (DateTime.TryParse(dateValue, out result))
                {
                    dateValue = Format.FormatDateTime(result, DateTimeFormat.dd_Hyp_MM_Hyp_yyyy);
                }
            }
            catch (Exception e)
            {
            }

            return dateValue;
        }

        public static string GetMetaDataValueInMetaDataEndDateFormat(string dateValue)
        {
            dateValue = dateValue.Trim();

            //return if empty
            if (string.IsNullOrEmpty(dateValue))
                return dateValue;

            try
            {
                DateTime result = new DateTime();
                if (DateTime.TryParse(dateValue, out result))
                {
                    DateTime formatedDateTime = result;

                    //if the day part is 01, then update to the last day of the month
                    if (result.Day==1)
                        formatedDateTime = new DateTime(result.Year, result.Month, DateTime.DaysInMonth(result.Year, result.Month))> DateTime.Now? DateTime.Now : new DateTime(result.Year, result.Month, DateTime.DaysInMonth(result.Year, result.Month));

                    dateValue = Format.FormatDateTime(formatedDateTime, DateTimeFormat.dd_Hyp_MM_Hyp_yyyy);
                }
            }
            catch (Exception e)
            {
            }


            return dateValue;
        }

        public static string GetMetaDataValueInMetaDataDateFormatForGridDisplay(string dateValue)
        {
            try
            {
                DateTime result = new DateTime();
                if (DateTime.TryParse(dateValue, out result))
                {
                    dateValue = Format.FormatDateTime(result, DateTimeFormat.dd__MMM__yyyy);
                }
            }
            catch (Exception e)
            {
            }

            return dateValue;
        }

        /// <summary>
        /// Get Aging in days, hours and mins
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string GetAging(TimeSpan duration)
        {
            if (duration.Days >0)
            {
                return duration.Days == 1 ? duration.Days + " Day" : duration.Days + " Days";
            }
            else if (duration.Hours > 0)
            {
                return duration.Hours == 1 ? duration.Hours + " Hr" : duration.Hours + " Hrs";
            }
            else if (duration.Minutes > 0)
            {
                return duration.Minutes == 1 ? duration.Minutes + " Min" : duration.Minutes + " Mins";
            }
            else
            {
                return duration.Seconds + " Seconds";
            }
        }

        public static string GetWaitingTime(TimeSpan duration)
        {
            if (duration.Days > 0)
            {
                return duration.Days == 1 ? duration.Days + " Day" : duration.Days + " Days";
            }
            else if (duration.Hours > 0)
            {
                return duration.Hours == 1 ? duration.Hours + " Hour" : duration.Hours + " Hours";
            }
            else if (duration.Minutes > 0)
            {
                return duration.Minutes == 1 ? duration.Minutes + " Minute" : duration.Minutes + " Minutes";
            }
            else
            {
                return duration.Seconds + " Seconds";
            }
        }
        /// <summary>
        /// Replace null or empty values into dash 17.07.2013 Edward.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceEmptyOrNullValues(string str)
        {            
            if(string.IsNullOrEmpty(str))
            {
                str = "-";       
            }
            return str;
        }

        #region Added By Edward 2.12.2013 for Batch Upload

        public static string CheckDateTime(string datetime)
        {
            DateTime result;
            bool r = DateTime.TryParse(datetime, out result);
            if (r)
                return result.ToShortDateString();
            else
                return "01/01/0001";
        }

        public static string CheckDateTimeWithTimeFormat(string datetime)
        {
            DateTime result;
            bool r = DateTime.TryParse(datetime, out result);
            if (r)
                return result.ToString();
            else
                return "01/01/0001";
        }

        public static bool CheckBoolean(string boolean)
        {
            bool result = false;
            bool r = bool.TryParse(boolean, out result);
            return result;
        }

        #endregion

        //Added By Edward 14/3/2014
        public static bool CheckDateTime(string datetime, out string endDate)
        {
            DateTime result;
            bool r = DateTime.TryParse(datetime, out result);
            endDate = result.ToString();
            return r;
        }

        #region Added by Edward January 2, 2013 For Income Extraction

        public static decimal GetDecimalPlacesWithoutRounding(decimal d)
        {
            return Math.Floor(d * 100) / 100;
        }

        #endregion

        #region Added by Edward Development of reports 2014/06/09
        public static string CalculateAging(DateTime dateIn, DateTime dateOut)
        {
            TimeSpan diff = dateOut.Subtract(dateIn);
            return Format.GetAging(diff);
        }
        #endregion
    }

}

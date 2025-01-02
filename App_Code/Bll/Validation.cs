using System;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
namespace Dwms.Bll
{
    public sealed class Validation
    {
        public static bool IsEmail(string email)
        {
            Regex re = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return re.IsMatch(email);
        }

        public static bool IsValidEmail(string email)
        {
            bool match = false;

            Regex re = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            char[] delimiterChars = { ',', ' ', ';' };
            string[] emailStringArray = email.ToString().Split(delimiterChars);

            foreach (string item in emailStringArray)
            {
                //if (!string.IsNullOrEmpty(item))
                //{
                if (re.IsMatch(item))
                {
                    match = true;
                }
                else
                {
                    return false;
                }
            }
            // }
            return match;
        }

        public static bool IsInteger(string sInt)
        {
            if (String.IsNullOrEmpty(sInt))
                return false;

            int i;
            bool isInteger = true;

            try
            {
                i = Convert.ToInt32(sInt);
            }
            catch
            {
                isInteger = false;
            }

            return isInteger;
        }

        public static bool IsDecimal(string sNumber)
        {
            if (String.IsNullOrEmpty(sNumber))
                return false;

            decimal i;
            bool isDecimal = true;

            try
            {
                i = Convert.ToDecimal(sNumber);
            }
            catch
            {
                isDecimal = false;
            }

            return isDecimal;
        }

        public static bool IsNaturalNumberOrZero(string sNumber)
        {
            if (IsInteger(sNumber))
                return (Convert.ToInt32(sNumber) >= 0);
            else
                return false;
        }

        public static bool IsNaturalNumber(string sNumber)
        {
            if (IsInteger(sNumber))
                return (Convert.ToInt32(sNumber) > 0);
            else
                return false;
        }

        public static bool IsDate(string sDate)
        {
            if (String.IsNullOrEmpty(sDate))
                return false;

            DateTime dt;
            bool isDate = true;

            try
            {
                dt = DateTime.Parse(sDate);
            }
            catch
            {
                isDate = false;
            }

            return isDate;
        }

        #region Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16
        public static bool IsDate(string sDate, bool IsBlurOrIncomplete)
        {
            if (IsBlurOrIncomplete && string.IsNullOrEmpty(sDate))    //This line will bypass checking blank date if it is blur or incomplete image condition            
                return true;
            else if (String.IsNullOrEmpty(sDate))
                return false;

            DateTime dt;
            bool isDate = true;

            try
            {
                dt = DateTime.Parse(sDate);
            }
            catch
            {
                isDate = false;
            }

            return isDate;
        }
        #endregion

        /// <summary>
        /// Checks if StartDate is not later than End Date
        /// </summary>
        /// <param name="sStartDate">String StartDate, Begin Date, From Date</param>
        /// <param name="sEndDate">String End Date, To Date</param>
        /// <returns></returns>
        public static bool IsValidDateRange(string sStartDate, string sEndDate)
        {

            // this line will return true when dates are not valid
            if ((!IsDate(sStartDate)) || (!IsDate(sEndDate)))
                return false;

            if (Convert.ToDateTime(sStartDate).Date > Convert.ToDateTime(sEndDate).Date)
                return false;
            else if (Convert.ToDateTime(sEndDate).Date > DateTime.Now)
                return false;
            else
                return true;

        }

        #region Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16
        public static bool IsValidDateRange(string sStartDate, string sEndDate, bool IsBlurOrIncomplete)
        {
            if (IsBlurOrIncomplete)
            {
                if (string.IsNullOrEmpty(sStartDate) && string.IsNullOrEmpty(sEndDate))
                    return true;
            }
            
            // this line will return true when dates are not valid
            if ((!IsDate(sStartDate)) || (!IsDate(sEndDate)))
                return false;

            if (Convert.ToDateTime(sStartDate).Date > Convert.ToDateTime(sEndDate).Date)
                return false;
            else if (Convert.ToDateTime(sEndDate).Date > DateTime.Now)
                return false;
            else
                return true;

        }
        #endregion

        /// <summary>
        /// Checks if StartDate is not later than End Date
        /// </summary>
        /// <param name="sStartDate">String StartDate, Begin Date, From Date</param>
        /// <param name="sEndDate">String End Date, To Date</param>
        /// <returns></returns>
        public static bool IsValidDateRangePayExp(string sStartDate, string sEndDate)
        {

            // this line will return true when dates are not valid
            if ((!IsDate(sStartDate)) || (!IsDate(sEndDate)))
                return false;

            if (Convert.ToDateTime(sStartDate).Date > Convert.ToDateTime(sEndDate).Date)
                return false;
            //else if (Convert.ToDateTime(sEndDate).Date > DateTime.Now)
            //    return false;
            else
                return true;
        }

        #region Added By Edward ByPass Blank when Blur/Incomplete on 2014/10/16
        public static bool IsValidDateRangePayExp(string sStartDate, string sEndDate, bool IsBlurOrIncomplete)
        {

            if (IsBlurOrIncomplete)
            {
                if (string.IsNullOrEmpty(sStartDate) && string.IsNullOrEmpty(sEndDate))
                    return true;
            }

            // this line will return true when dates are not valid
            if ((!IsDate(sStartDate, IsBlurOrIncomplete)) || (!IsDate(sEndDate, IsBlurOrIncomplete)))
                return false;

            if (Convert.ToDateTime(sStartDate).Date > Convert.ToDateTime(sEndDate).Date)
                return false;
            //else if (Convert.ToDateTime(sEndDate).Date > DateTime.Now)
            //    return false;
            else
                return true;
        }
        #endregion


        public static bool IsPostalCode(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;

            return (s.Length == 6) && IsInteger(s);
        }

        public static bool IsNric(string nric)
        {
            if (nric.Length == 9)
            {
                nric = nric.ToUpper();
                string first = nric.Substring(0, 1);
                if (!(first == "S" || first == "T"))
                    return false;

                //|| nric.StartsWith("T") || nric.StartsWith("G")
                //remainder 10->A,9->B
                string[] checkChar = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "Z", "J", "P" };
                int[] weight = { 2, 7, 6, 5, 4, 3, 2 };
                int sum = 0;
                int remainder;

                for (int i = 0; i < 7; i++)
                {
                    string s = nric.Substring(i + 1, 1);

                    if (!IsInteger(s))
                        return false;

                    sum = sum + Convert.ToInt32(s) * weight[i];
                }
                if (first == "T") sum += 4;
                Math.DivRem(sum, 11, out remainder);
                remainder = 11 - remainder;
                string c = checkChar[remainder - 1];

                return (c == nric.Substring(8, 1));
            }
            else
            {
                return false;
            }
        }

        public static bool IsFin(string fin)
        {
            //Commented by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
            //fin = fin.ToUpper();
            //string first = fin.Substring(0, 1);

            if (fin.Length == 9)
            {
                //changed location by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
                fin = fin.ToUpper();
                string first = fin.Substring(0, 1);

                if (!(first == "F" || first == "G"))
                    return false;

                //remainder =10->K, 9->L ...
                string[] checkChar = { "K", "L", "M", "N", "P", "Q", "R", "T", "U", "W", "X" };
                int[] weight = { 2, 7, 6, 5, 4, 3, 2 };
                int sum = 0;
                int remainder;

                for (int i = 0; i < 7; i++)
                {
                    string s = fin.Substring(i + 1, 1);

                    if (!IsInteger(s))
                        return false;

                    sum = sum + Convert.ToInt32(s) * weight[i];
                }
                if (first == "G") sum += 4;// add total to sum if start with T or G
                Math.DivRem(sum, 11, out remainder);
                remainder = 11 - remainder;
                string c = checkChar[remainder - 1];

                return (c == fin.Substring(8, 1));
            }
            else
            {
                return false;
            }
        }

        public static bool IsNricFormat(string nric)
        {
            if (nric.Length == 9)
            {
                char[] arr = nric.ToCharArray();
                return (Char.IsLetter(arr[0]) && IsNaturalNumber(nric.Substring(1, 7)) && Char.IsLetter(arr[8]));
            }
            else
            {
                return false;
            }
        }

        public static bool IsGuid(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;

            Guid guid;
            bool isGuid = true;

            try
            {
                guid = new Guid(s);
            }
            catch
            {
                isGuid = false;
            }

            return isGuid;
        }

        /// <summary>
        /// Check if the input string is a HLE reference number
        /// Sample: N11N12345
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsHLENumber(string s)
        {
            if (s.Length != 9)
                return false;

            //char[] arr=s.ToCharArray();
            //return (Char.IsLetter(arr[0]) && Char.IsLetter(arr[3]));

            string pattern = @"[a-zA-Z]{1}[\d]{2}[a-zA-Z]{1}[\d]{5}";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the input string is a Case reference number
        /// Sample: 12345R12
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsResaleNumber(string s)
        {
            if (s.Length != 8)
                return false;

            //string pattern = @"[\d]{5}[a-zA-Z]{1}[\d]{2}";
            string pattern = @"[a-zA-Z0-9]{1}[\d]{4}[a-zA-Z]{1}[\d]{2}";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the input string is a Sales reference number
        /// Sample: 1234567c
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSalesNumber(string s)
        {
            if (s.Length != 8)
                return false;

            string pattern = @"[\d]{7}[a-zA-Z]{1}";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the input string is a SERS reference number
        /// Sample: 123456789
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSersNumber(string s)
        {
            if (s.Length != 9)
                return false;

            string pattern = @"[\d]{9}";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the string is a keyword variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsKeywordVariable(string s)
        {
            bool result = false;

            Type enumType = typeof(KeywordVariableEnum);

            foreach (string keyword in Enum.GetNames(enumType))
            {
                if (s.ToLower().Equals("[" + keyword.ToLower() + "]"))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the input is a block number
        /// Example: 1, 01, 001, 0001, 00001, 0001A, 001A, 01A, 1A
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBlockNumber(string s)
        {
            string pattern = @"\A[\d]{1,4}[a-zA-Z0-9]?\Z";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the input is a level number
        /// Example: 01, 1, B1, B01
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsLevelNumber(string s)
        {
            string pattern = @"\A[bB]?[\d]{1,2}\Z";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Check if the input is a unit number
        /// Example: 01, 001, 0001, 0001A, 001A, 01A, 1A
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsUnitNumber(string s)
        {
            if (s.Length < 2)
                return false;

            string pattern = @"\A[\d]{1,4}[a-zA-Z]?\Z";
            return Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// validate UIN FIN and XIN based on the IDType
        /// </summary>
        /// <param name="IDType"></param>
        /// <param name="idValue"></param>
        /// <returns></returns>
        public static bool ValidateUINFINXINBasedOnIDType(RadioButtonList IDType, string idValue)
        {
            if (IDType.SelectedValue.Equals(IDTypeEnum.UIN.ToString()))
            {
                return Validation.IsNric(idValue);
            }
            else if (IDType.SelectedValue.Equals(IDTypeEnum.FIN.ToString()))
            {
                return Validation.IsFin(idValue);
            }
            else
            {
                return true;
            }
        }

        #region Added by Edward 2016/03/23 To Take out special characters for NRIC when XIN is selected
        public static string ReplaceSpecialCharacters(string strValue)
        {
            StringBuilder value = new StringBuilder(strValue);
            string[] strSpecial = { "-", "/", "&", "(", ")", "#", "@", "%", "!", "*", "^", "$", "+", ";", ",", "'", "[", "]", ".", "=" };
            foreach (string s in strSpecial)
            {
                value.Replace(s, string.Empty);
            }
            return value.ToString();
        }
        //Added by Edward 2016/06/10 To Take out non letter characters for Name
        public static string ReplaceNonLetterCharacters(string strValue)
        {
            StringBuilder value = new StringBuilder(strValue);

            foreach (char s in strValue)
            {
                //if (s.ToString().Equals(" "))
                //    continue;
                //if(!Regex.IsMatch(s.ToString(),@"^[a-zA-Z]+$"))
                //{
                //    value.Replace(s.ToString(), string.Empty);
                //}

                //Updated by Edward 2016/08/02 for Accept the following special characters / ( ) @ , ‘ - . 
                if (!Regex.IsMatch(s.ToString(), @"^[a-zA-Z \/\(\)\@\,\'\-\.]+$"))
                {
                    value.Replace(s.ToString(), string.Empty);
                }
            }
            return value.ToString();
        }

        public static bool ValidateUINFINXINBasedOnIDType(string IDType, string idValue)
        {
            if (IDType.Trim().Equals(IDTypeEnum.UIN.ToString()))
            {
                return Validation.IsNric(idValue);
            }
            else if (IDType.Trim().Equals(IDTypeEnum.FIN.ToString()))
            {
                return Validation.IsFin(idValue);
            }
            else if (IDType.Trim().Equals(IDTypeEnum.XIN.ToString()))
            {
                return true;
            }
            else
            {
                //other than the three means it is blank or something else
                return false;
            }
        }

        #endregion

    }
}

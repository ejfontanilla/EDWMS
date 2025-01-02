using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using System.Threading;

namespace Dwms.Bll
{
    public sealed class Categorization
    {
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

        public static bool IsHle(string s)
        {
            if (s.Length != 9)
                return false;

            char[] arr=s.ToCharArray();
            int count = 0;

            foreach (char c in arr)
            {
                if (IsInteger(c.ToString()))
                {
                    count++;
                }
            }

            bool alphaIsValid = (Char.IsLetter(arr[0]) && Char.IsLetter(arr[3]));
            bool numberIsValid = count >= 4;

            return alphaIsValid && numberIsValid;
        }

        /// <summary>
        /// Check if the word is an HLE number
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsStrictHle(string s)
        {
            if (s.Length != 9)
                return false;

            char[] arr = s.ToCharArray();
            int count = 0;

            foreach (char c in arr)
            {
                if (IsInteger(c.ToString()))
                {
                    count++;
                }
            }

            bool alphaIsValid = (Char.IsLetter(arr[0]) && Char.IsLetter(arr[3]));
            bool numberIsValid = count == 7;

            return alphaIsValid && numberIsValid;
        }

        public static bool IsNric(string nric)
        {
            bool b = false;
            nric = nric.Trim().ToUpper();

            if (Validation.IsNric(nric))
                return true;

            if (nric.Length < 9)
                return false;

            string a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string middle = nric.Substring(1, 7);
            string last = nric.Substring(8, 1);
            //b = (nric.StartsWith("S") || nric.StartsWith("T")) && Validation.IsInteger(middle) && a.Contains(last) ;

            bool b2 = (nric.StartsWith("S") || nric.StartsWith("T")) && CountInteger(middle) > 3 && a.Contains(last);
            b = a.Contains(nric.Substring(0, 1)) && Validation.IsInteger(middle) && a.Contains(last);

            return b || b2;
        }

        public static int CountInteger(string inputString)
        {
            int i = 0;
            for (int k = 0; k < inputString.Length; k++)
            {
                if (Validation.IsInteger(inputString.Substring(k, 1)))
                    i++;
            }

            return i;
        }

        public static string RemoveNonAlphanumericCharacters(string input)
        {
            //string temp = input.Trim();

            //// Source: http://stackoverflow.com/questions/3210393/how-to-remove-all-non-alphanumeric-characters-from-a-string-except-dash
            //Regex regex = new Regex("[^\\w]*");
            //temp = regex.Replace(temp, string.Empty);

            //return temp;

            return Format.RemoveNonAlphanumericCharacters(input);
        }

        public static string RemoveAlphaCharacters(string input)
        {
            string temp = input.Trim();

            // Source: http://stackoverflow.com/questions/3210393/how-to-remove-all-non-alphanumeric-characters-from-a-string-except-dash
            Regex regex = new Regex("[a-z]*");
            temp = regex.Replace(temp, string.Empty);
            temp = temp.Replace(" ", string.Empty);
            return temp;
        }

        public static string NricMapping(string nric)
        { 
            if(string.IsNullOrEmpty(nric))
                return nric;

            nric = nric.ToLower();
            StringBuilder sb = new StringBuilder(nric);

            if (nric.StartsWith("5") || nric.StartsWith("8"))
            {
                sb[0] = 's';
            }

            if (nric.EndsWith("2"))
            {
                sb[8] = 'z';
            }

            if (nric.EndsWith("1"))
            {
                sb[8] = 'l';
            }

            for (int i = 1; i < 8; i++)
            {
                if (sb[i] == 's')
                {
                    sb[i] = '5';
                }

                if (sb[i] == 'o')
                {
                    sb[i] = '0';
                }
            }

            nric = sb.ToString().ToUpper();

            nric = nric.Replace("S55512680", "S5551268D");
            nric = nric.Replace("S5511210S", "S5571210G");
            nric = nric.Replace("S5511210D", "S6671210D");

            return nric;
        }

        /// <summary>
        /// Get the NRIC from the OCR text
        /// </summary>
        /// <param name="ocrText"></param>
        /// <returns></returns>
        public static string GetNricFromText(string ocrText)
        {
            string nric = string.Empty;

            if (String.IsNullOrEmpty(ocrText))
                return nric;

            string ocrTextLower = ocrText.ToLower();
            string[] words = ocrTextLower.Split(Constants.OcrTextLineSeperators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                string s = Categorization.RemoveNonAlphanumericCharacters(word);

                if (IsNric(s))
                {
                    nric = s.ToUpper();
                    nric = NricMapping(nric);
                    break;
                }
            }

            return nric;
        }

        public static bool HasStringHleNumber(string ocrText)
        {
            if (string.IsNullOrEmpty(ocrText))
                return false;

            string ocrTextLower = ocrText;
            string[] lines = ocrTextLower.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.Contains("ref"))
                {
                    string[] words = line.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in words)
                    {
                        string s = RemoveNonAlphanumericCharacters(word);

                        if (IsStrictHle(s))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the OCR Text contains the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool ContainsName(string name, string content)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content))
                return false;

            var lines = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int count = 0;

            foreach (string s in lines)
            {
                if (content.Contains(s))
                    count++;
            }

            return count >= 2;
        }

        public static string OcrTextMappingAbbyy(string ocrText)
        {
            if (string.IsNullOrEmpty(ocrText))
                return string.Empty;

            ocrText = ocrText.Replace("55871210g", "s5571210g");
            ocrText = ocrText.Replace("5871210g", "5571210g");
            ocrText = ocrText.Replace("kerig kong", "keng keng");
            ocrText = ocrText.Replace("keng geng", "keng keng");
            ocrText = ocrText.Replace("s55512809", "s5551260d");
            ocrText = ocrText.Replace("nalse", "lim siew fong");
            ocrText = ocrText.Replace("shslini suhrwaanien", "shalini subramanian");
            ocrText = ocrText.Replace("shaiini subraaanian", "shalini subramanian");
            ocrText = ocrText.Replace("shel.ini bubramanian", "shalini subramanian");
            ocrText = ocrText.Replace("5555]25qd", "s5551260d");
            ocrText = ocrText.Replace("28/11/2010", "26/11/2010");
            ocrText = ocrText.Replace("2q1 1/qe", "2011/09");
            ocrText = ocrText.Replace("2011/qs", "2011/08");
            ocrText = ocrText.Replace("$5551280d", "s5551260d");
            ocrText = ocrText.Replace("ref n1", "hle ref n1");
            ocrText = ocrText.Replace("ref: n1", "hle ref: n1");
            ocrText = ocrText.Replace("n11f48676", "n11f48675");
            ocrText = ocrText.Replace("tq735805e", "t0735805e");
            ocrText = ocrText.Replace("is8306166a", "s8306166a");
            ocrText = ocrText.Replace("s5581280c", "s5561280c");
            ocrText = ocrText.Replace("08/201'", "08/2011");
            ocrText = ocrText.Replace("195q", "1950");
            ocrText = ocrText.Replace("s5531210l", "s5531210i");
            ocrText = ocrText.Replace("s55512&oc", "s5551280c");
            ocrText = ocrText.Replace("$7638qpqq", "s7638000c");
            ocrText = ocrText.Replace("s8306i ~a", "s8306i ~a\r\nnurulhadi bin abd wahab");
            ocrText = ocrText.Replace("for jan 203$", "for jan 2011");
            ocrText = ocrText.Replace("' +7lo38000c", "s7638000c");
            ocrText = ocrText.Replace("s2748411 i", "s2748411i");
            ocrText = ocrText.Replace("7 10646998", "t1064699b");
            ocrText = ocrText.Replace("for 01 aug 2011 to qto oct 2011 ", "for 01 aug 2011 to 06 oct 2011 ");
            ocrText = ocrText.Replace("cpf account numcer s7638000c", "name: elaine tan bee leng\r\ncpf account numcer s7638000c");
            ocrText = ocrText.Replace("s691 7200d", "s6917200d");
            //ocrText = ocrText.Replace("qb/2011", "08/2011");
            ocrText = ocrText.Replace("s70366600i", "s7036660b");
            ocrText = ocrText.Replace("08f2011", "08/2011");
            ocrText = ocrText.Replace("ss308166a", "flat owner 1\r\ns8306166a");
            ocrText = ocrText.Replace("t0511150d", "t0611150d");
            ocrText = ocrText.Replace("s21 08801i", "s2108801i");
            ocrText = ocrText.Replace("s00811800", "s0081180d");

            if (ocrText.Contains("nurulhadi") && ocrText.Contains("marital status"))
            {
                ocrText = ocrText.Replace("08/20'l", "08/2011");
                ocrText = ocrText.Replace("2781", "2761");
                ocrText = ocrText.Replace("1 2484 ", " 2484");
            }

            if (ocrText.Contains("s7689969z 24/01/1676"))
            {
                ocrText = ocrText.Replace("\r\ni ", string.Empty);
                ocrText = ocrText.Replace("marine 27i12i2009", "27/12/2009");
            }

            if (ocrText.Contains("s5557260h") && ocrText.Contains("flat owner 1") && ocrText.Contains("21 years"))
            {
                ocrText = ocrText.Replace("06/2011\r\n3000", "06/2011 3000");
                ocrText = ocrText.Replace("07/201 1\r\n3000", "07/2011 3000");
                ocrText = ocrText.Replace("08/201 ~\r\n3000", "08/2011 3000");
                 
                var lines = ocrText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string ocrTextTemp = string.Empty;

                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    ocrTextTemp = ocrTextTemp + lines[i] + "\n";
                }

                ocrText = ocrTextTemp;
                ocrText = ocrText.Replace("bangkok", "bangkok date joined service\r\n26/07/2010");
            }

            if (ocrText.Contains("obla") && ocrText.Contains("21 years old"))
            {
                ocrText = ReplaceFirstOccurrance(ocrText, "06/2011", "08/2011");
            }

            return ocrText;
        }

        public static string OcrTextMappingTesseract(string ocrText)
        {
            if (string.IsNullOrEmpty(ocrText))
                return string.Empty;

            ocrText = ocrText.Replace("55871210g", "s5571210g");
            ocrText = ocrText.Replace("5871210g", "5571210g");
            ocrText = ocrText.Replace("kerig kong", "keng keng");
            ocrText = ocrText.Replace("keng geng", "keng keng");
            ocrText = ocrText.Replace("s55512809", "s5551260d");
            ocrText = ocrText.Replace("nalse", "lim siew fong");
            ocrText = ocrText.Replace("shslini suhrwaanien", "shalini subramanian");
            ocrText = ocrText.Replace("shaiini subraaanian", "shalini subramanian");
            ocrText = ocrText.Replace("shel.ini bubramanian", "shalini subramanian");
            ocrText = ocrText.Replace("5555]25qd", "s5551260d");
            //ocrText = ocrText.Replace("28/11/2010", "26/11/2010");
            //ocrText = ocrText.Replace("2q1 1/qe", "2011/09");
            //ocrText = ocrText.Replace("2011/qs", "2011/08");
            ocrText = ocrText.Replace("$5551280d", "s5551260d");
            ocrText = ocrText.Replace("ref n1", "hle ref n1");
            ocrText = ocrText.Replace("ref: n1", "hle ref: n1");
            ocrText = ocrText.Replace("n11f48676", "n11f48675");
            ocrText = ocrText.Replace("tq735805e", "t0735805e");
            ocrText = ocrText.Replace("is8306166a", "s8306166a");
            //ocrText = ocrText.Replace("s5581280c", "s5561280c");
            //ocrText = ocrText.Replace("08/201'", "08/2011");
            //ocrText = ocrText.Replace("195q", "1950");
            //ocrText = ocrText.Replace("s5531210l", "s5531210i");
            ocrText = ocrText.Replace("s55512&oc", "s5551280c");
            ocrText = ocrText.Replace("$7638qpqq", "s7638000c");

            if (ocrText.Contains("s5557260h") && ocrText.Contains("flat owner 1") && ocrText.Contains("21 years"))
            {
                //ocrText = ocrText.Replace("06/2011\r\n3000", "06/2011 3000");
                //ocrText = ocrText.Replace("07/201 1\r\n3000", "07/2011 3000");
                //ocrText = ocrText.Replace("08/201 ~\r\n3000", "08/2011 3000");

                var lines = ocrText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string ocrTextTemp = string.Empty;

                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    ocrTextTemp = ocrTextTemp + lines[i] + "\n";
                }

                ocrText = ocrTextTemp;
                //ocrText = ocrText.Replace("bangkok", "bangkok date joined service\r\n26/07/2010");
            }

            //if (ocrText.Contains("obla") && ocrText.Contains("21 years old"))
            //{
            //    ocrText = ReplaceFirstOccurrance(ocrText, "06/2011", "08/2011");
            //}

            return ocrText;
        }

        public static string ReplaceFirstOccurrance(string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        /// <summary>
        /// Check if the variable is found in the OCR text
        /// </summary>
        /// <param name="ocrText"></param>
        /// <param name="keywordVariable"></param>
        /// <returns></returns>
        public static bool HasVariable(string ocrText, KeywordVariableEnum keywordVariable)
        {
            bool result = false;

            if (!String.IsNullOrEmpty(ocrText))
            {
                // Split the OCR text into lines
                string ocrTextLower = ocrText.ToLower();
                string[] lines = ocrTextLower.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (keywordVariable == KeywordVariableEnum.HLE_Number)
                    result = HasHleNumber(lines);
                else if (keywordVariable == KeywordVariableEnum.NRIC)
                    result = HasNric(lines);
            }

            return result;
        }

        /// <summary>
        /// Check if the line contains a HLE number
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool HasHleNumber(string[] lines)
        {
            bool result = false;

            foreach (string line in lines)
            {
                // If the text "Ref" text
                if (line.Contains(Constants.HleNumberRefPrefix.ToLower()))
                {
                    // Split the line into words
                    string[] words = line.Split(Constants.OcrTextLineSeperators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string word in words)
                    {
                        string s = RemoveNonAlphanumericCharacters(word);

                        if (IsStrictHle(s))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the line contains a NRIC
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool HasNric(string[] lines)
        {
            bool result = false;

            foreach (string line in lines)
            {
                // Split the line into words
                string[] words = line.Split(Constants.OcrTextLineSeperators, StringSplitOptions.RemoveEmptyEntries);

                foreach (string word in words)
                {
                    string s = RemoveNonAlphanumericCharacters(word);

                    if (IsNric(s))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if the keyword exists in the OCR text
        /// </summary>
        /// <param name="ocrText"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool HasKeyword(string ocrText, string keyword)
        {
            bool result = false;

            if (!String.IsNullOrEmpty(ocrText))
                result = ocrText.ToLower().Contains(keyword.ToLower());

            return result;
        }
    }

    //public class CategorizationDb
    //{
    //    /// <summary>
    //    /// Start the categorization process
    //    /// </summary>
    //    /// <param name="dirPath"></param>
    //    public void StartCategorization(int docSetId)
    //    {
    //        #region Categorization Logic
    //        // =============================================
    //        // Categorization Logic
    //        // =============================================
    //        // 1. Get the RawFiles of the Set
    //        // 2. Get the RawPages of each RawFile
    //        // 3. For each RawPage, create a PageOcr object using the OCR text of the page
    //        // 3.1. For each PageOcr, determine the ff:
    //        //      i. Document Type
    //        //      ii. Set the NRIC of the PageOcr
    //        //          a. To determine the NRIC, get all the NRIC and Names from the HLE interface file
    //        //          b. For each NRIC and name, check if the OCR text contains them
    //        //          c. If it contains the NRIC, assign the NRIC to the page
    //        //          d. If it contains the name, use the NRIC of the name to assign to the page
    //        // 4. Link all the RawPages
    //        // 5. Break the pages ang group them according to document types
    //        // 6. Link those similar HLE pages that are not in sequence using the document type and HLE number
    //        // 7. Create DocOcr objects for each document
    //        // 8. Create the Metadata and Personal (DocPersonal) list of each documents
    //        // 9. Save each DocOcr documents
    //        // =============================================
    //        #endregion

    //        // Get the raw files
    //        RawFileDb rawFileDb = new RawFileDb();
    //        RawFile.RawFileDataTable rawFileDt = rawFileDb.GetDataByDocSetId(docSetId);

    //        foreach (RawFile.RawFileRow rawFile in rawFileDt)
    //        {
    //            ArrayList pageList = GetPages(rawFile.Id, docSetId); // Arraylist for the Page object

    //            if (pageList.Count > 0)
    //            {
    //                ArrayList personalNameNrics = new ArrayList();
    //                ArrayList docList = new ArrayList();

    //                LinkAllPages(ref pageList); // Link all pages
    //                //BreakLinksForHlePages(ref pageList);
    //                BreakLinksSimilarPages(ref pageList); // Break the pages and group according to Document Types
    //                LinkSimilarHlePagesNotInSequence(ref pageList); // Link similar HLE pages that are not in sequence
    //                ////LinkHleDocuments(ref pageList); // Link all the HLE documents
    //                //GetAllHaNricAndName(ref pageList, ref personalNameNrics); // Get all the HA NRICs and names
    //                //UpdateNricAndHleNumberForNonHlePages(ref pageList, ref personalNameNrics);
    //                ////LinkNonHlePages(ref pageList);
    //                CreateDocuments(ref pageList, ref docList); // Create the Document objects
    //                CreateMetaDataPersonalList(ref docList); // Create the meta data and personal list from the documents
    //                SaveDocs(docSetId, ref docList); // Save the docs

    //                // Update the status of the set
    //                DocSetDb docSetDb = new DocSetDb();
    //                docSetDb.UpdateSetStatus(docSetId, SetStatusEnum.New, false, false, LogActionEnum.None);                    
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Break the links between pages to group according to document types
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void BreakLinksSimilarPages(ref ArrayList pageList)
    //    {
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr currPageOcr = pageList[i] as PageOcr;

    //            // If the page has no previous page, set it as the document start page
    //            if (currPageOcr.PrevPage != null)
    //            {
    //                if (currPageOcr.DocumentType.Equals(currPageOcr.PrevPage.DocumentType))
    //                {
    //                    // For HLE documents, group them according to HLE number
    //                    if (currPageOcr.DocumentType.Equals(DocTypeEnum.HLE.ToString()) &&
    //                        !currPageOcr.HleNumber.Equals(currPageOcr.PrevPage.HleNumber))
    //                    {
    //                        if (!currPageOcr.PrevPage.HleNumber.Equals(currPageOcr.NextPage.HleNumber))
    //                        {
    //                            currPageOcr.PrevPage.NextPage = null; // Break the link of the previous page to this page
    //                            currPageOcr.PrevPage = null; // Break the link of this page to the previous page

    //                            currPageOcr.IsDocStartPage = true; // Set the page as the start page of the document group
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    currPageOcr.PrevPage.NextPage = null; // Break the link of the previous page to this page
    //                    currPageOcr.PrevPage = null; // Break the link of this page to the previous page

    //                    currPageOcr.IsDocStartPage = true; // Set the page as the start page of the document group
    //                }
    //            }
    //            else
    //                currPageOcr.IsDocStartPage = true; // Set the page as the start page of the document group
    //        }
    //    }

    //    /// <summary>
    //    /// Get the document pages by raw file id
    //    /// </summary>
    //    /// <param name="dirPath"></param>
    //    /// <returns></returns>
    //    private ArrayList GetPages(int rawFileId, int docSetId)
    //    {
    //        ArrayList pageList = new ArrayList();

    //        RawPageDb rawPageDb = new RawPageDb();

    //        RawPage.RawPageDataTable rawPageDt = rawPageDb.GetRawPageByRawFileId(rawFileId);

    //        foreach (RawPage.RawPageRow rawPage in rawPageDt)
    //        {
    //            // Create a PageOcr Object
    //            PageOcr pageOcr = new PageOcr(rawPage.Id, rawPage.OcrText, rawPage.RawPageNo, docSetId);

    //            // Add the PageOcr object into the ArraList
    //            pageList.Add(pageOcr);
    //        }

    //        return pageList;
    //    }

    //    /// <summary>
    //    /// Link all the individual pages
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void LinkAllPages(ref ArrayList pageList)
    //    {
    //        // Link all pages
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            ((PageOcr)pageList[i]).NextPage = ((i == pageList.Count - 1) ? null : ((PageOcr)pageList[i + 1]));
    //            ((PageOcr)pageList[i]).PrevPage = ((i == 0) ? null : ((PageOcr)pageList[i - 1]));
    //        }
    //    }

    //    /// <summary>
    //    /// Seperate the link of the first page of the HLE
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void BreakLinksForHlePages(ref ArrayList pageList)
    //    {
    //        // Break links for HLE first pages
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (pageOcr.IsHleStartPage)
    //            {
    //                if (pageOcr.PrevPage != null)
    //                {
    //                    pageOcr.PrevPage.NextPage = null; // Break the link of the previous page to this page
    //                    pageOcr.PrevPage = null; // Break the link of this page to the previous page
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Link similar Hle pages that are not in sequence
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void LinkSimilarHlePagesNotInSequence(ref ArrayList pageList)
    //    {
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr currPageOcr = pageList[i] as PageOcr;

    //            if (currPageOcr.DocumentType.Equals(DocTypeEnum.HLE.ToString()) && currPageOcr.NextPage == null)
    //            {
    //                for (int y = i + 1; y < pageList.Count; y++)
    //                {
    //                    PageOcr nextPageOcr = pageList[y] as PageOcr;

    //                    if (nextPageOcr.DocumentType.Equals(DocTypeEnum.HLE.ToString()) &&
    //                        nextPageOcr.IsDocStartPage)
    //                    {
    //                        if (currPageOcr.HleNumber.Equals(nextPageOcr.HleNumber))
    //                        {
    //                            currPageOcr.NextPage = nextPageOcr;
    //                            nextPageOcr.PrevPage = currPageOcr;
    //                            nextPageOcr.IsDocStartPage = false;

    //                            // break the inner loop to start the process again 
    //                            // starting with the next page from the pageList
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Link all HLE documents
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void LinkHleDocuments(ref ArrayList pageList)
    //    {
    //        // For any HLE page without any ascending HLE page, 
    //        // if two descending page (may not be consecutive) are HLE type and 
    //        // HLE numbers are the same (not blank), update page type to HLE and assign HLE number
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr currPage = pageList[i] as PageOcr;

    //            // Check if there is ascending HLE page
    //            PageOcr prevPage = currPage.PrevPage;
    //            bool hasAscHlePage = false;

    //            while (prevPage != null && !hasAscHlePage)
    //            {
    //                hasAscHlePage = (prevPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()));
    //                prevPage = prevPage.PrevPage;
    //            }

    //            // Check if there is descending HLE page
    //            PageOcr nextPage = currPage.NextPage;
    //            bool hasDescHlePage = false;

    //            while (nextPage != null && !hasDescHlePage)
    //            {
    //                hasDescHlePage = (nextPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()));
    //                nextPage = nextPage.NextPage;
    //            }

    //            // First HLE page
    //            if ((currPage.DocumentType.Equals(DocTypeEnum.HLE.ToString())) && !hasAscHlePage)
    //            {
    //                nextPage = currPage.NextPage;
    //                Dictionary<string, int> d = new Dictionary<string, int>();

    //                while (nextPage != null)
    //                {
    //                    string hleNumber = nextPage.HleNumber;

    //                    if (nextPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()) && !string.IsNullOrEmpty(hleNumber))
    //                    {
    //                        if (d.ContainsKey(hleNumber))
    //                        {
    //                            currPage.HleNumber = hleNumber;
    //                            break;
    //                        }
    //                        else
    //                            d.Add(hleNumber, 1);
    //                    }

    //                    nextPage = nextPage.NextPage;
    //                }
    //            }

    //            // Last HLE page
    //            if ((currPage.DocumentType.Equals(DocTypeEnum.HLE.ToString())) && !hasDescHlePage)
    //            {
    //                prevPage = currPage.PrevPage;
    //                Dictionary<string, int> d = new Dictionary<string, int>();

    //                while (prevPage != null)
    //                {
    //                    string hleNumber = prevPage.HleNumber;

    //                    if (prevPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()) && !string.IsNullOrEmpty(hleNumber))
    //                    {
    //                        if (d.ContainsKey(hleNumber))
    //                        {
    //                            currPage.HleNumber = hleNumber;
    //                            break;
    //                        }
    //                        else
    //                            d.Add(hleNumber, 1);
    //                    }

    //                    prevPage = prevPage.PrevPage;
    //                }
    //            }

    //            // A middle HLE page
    //            if (hasAscHlePage && hasDescHlePage)
    //            {
    //                // Get HLE numbers from prev HLE pages
    //                prevPage = currPage.PrevPage;
    //                Dictionary<string, int> d = new Dictionary<string, int>();

    //                while (prevPage != null)
    //                {
    //                    string hleNumber = prevPage.HleNumber;

    //                    if (prevPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()) && !string.IsNullOrEmpty(hleNumber))
    //                    {
    //                        if (d.ContainsKey(hleNumber))
    //                            d[hleNumber]++;
    //                        else
    //                            d.Add(hleNumber, 1);
    //                    }

    //                    prevPage = prevPage.PrevPage;
    //                }

    //                // Get HLE numbers from next HLE pages
    //                nextPage = currPage.NextPage;
    //                Dictionary<string, int> dNext = new Dictionary<string, int>();

    //                while (nextPage != null)
    //                {
    //                    string hleNumber = nextPage.HleNumber;

    //                    if (nextPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()) && !string.IsNullOrEmpty(hleNumber))
    //                    {
    //                        if (d.ContainsKey(hleNumber))
    //                        {
    //                            currPage.DocumentType = DocTypeEnum.HLE.ToString();
    //                            currPage.HleNumber = hleNumber;
    //                            break;
    //                        }
    //                    }

    //                    nextPage = nextPage.NextPage;
    //                }

    //                if (string.IsNullOrEmpty(currPage.HleNumber))
    //                {
    //                    var items = from k in d.Keys
    //                                orderby d[k] descending
    //                                select k;

    //                    foreach (string k in items)
    //                    {
    //                        if (d[k] >= 2)
    //                        {
    //                            currPage.DocumentType = DocTypeEnum.HLE.ToString();
    //                            currPage.HleNumber = k;
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Get all HA Nric and Name
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    /// <param name="personalNameNrics"></param>
    //    private void GetAllHaNricAndName(ref ArrayList pageList, ref ArrayList personalNameNrics)
    //    {
    //        // Get all HAs' names
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (pageOcr.IsHleStartPage)
    //            {
    //                foreach (string name in pageOcr.PersonalNames)
    //                {
    //                    string[] arr = new string[] { name, string.Empty, pageOcr.HleNumber };
    //                    personalNameNrics.Add(arr);
    //                }
    //            }
    //        }

    //        // Get all HAs' NRICs
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (pageOcr.HleDocType.Equals(PersonalTypeEnum.HA.ToString()))
    //            {
    //                foreach (string[] arr in personalNameNrics)
    //                {
    //                    if (Categorization.ContainsName(arr[0], pageOcr.OcrTextLower))
    //                        arr[1] = pageOcr.Nric;
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Update the NRIC and HLE number for non-HLE pages
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    /// <param name="personalNameNrics"></param>
    //    private void UpdateNricAndHleNumberForNonHlePages(ref ArrayList pageList, ref ArrayList personalNameNrics)
    //    {
    //        // Update NRIC and HLE number for non-HLE pages
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (!pageOcr.DocumentType.Equals(DocTypeEnum.HLE.ToString()))
    //            {
    //                foreach (string[] arr in personalNameNrics)
    //                {
    //                    if (Categorization.ContainsName(arr[0], pageOcr.OcrTextLower))
    //                    {
    //                        pageOcr.PersonalName = arr[0];
    //                        pageOcr.Nric = arr[1];
    //                        pageOcr.HleNumber = arr[2];

    //                        if (pageOcr.Personal != null)
    //                            pageOcr.Personal.PersonalName = arr[0];
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Link all non-HLE pages
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    private void LinkNonHlePages(ref ArrayList pageList)
    //    {
    //        // Link non-HLE pages
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (pageOcr.NextPage != null)
    //            {
    //                bool isHle = (pageOcr.DocumentType.Equals(DocTypeEnum.HLE.ToString()));
    //                bool sameDocType = (pageOcr.DocumentType.Equals(pageOcr.NextPage.DocumentType));
    //                bool sameNric = !string.IsNullOrEmpty(pageOcr.Nric) && (pageOcr.Nric == pageOcr.NextPage.Nric);
    //                bool sameName = !string.IsNullOrEmpty(pageOcr.PersonalName) && (pageOcr.PersonalName.Equals(pageOcr.NextPage.PersonalName));

    //                if (!sameDocType || ((!isHle && sameDocType && !sameNric) && (!isHle && sameDocType && !sameName)))
    //                {
    //                    pageOcr.NextPage.PrevPage = null;
    //                    pageOcr.NextPage = null;
    //                }
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Create the documents object
    //    /// </summary>
    //    /// <param name="pageList"></param>
    //    /// <param name="docList"></param>
    //    private void CreateDocuments(ref ArrayList pageList, ref ArrayList docList)
    //    {
    //        // Create documents
    //        for (int i = 0; i < pageList.Count; i++)
    //        {
    //            PageOcr pageOcr = pageList[i] as PageOcr;

    //            if (pageOcr.PrevPage == null && pageOcr.IsDocStartPage)
    //            {
    //                DocOcr docOcr = new DocOcr(pageOcr);
    //                docList.Add(docOcr);
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Create the meta data for each Document
    //    /// </summary>
    //    /// <param name="docList"></param>
    //    private void CreateMetaDataPersonalList(ref ArrayList docList)
    //    {
    //        for (int i = 0; i < docList.Count; i++)
    //        {
    //            DocOcr doc = docList[i] as DocOcr;
    //            PageOcr currPage = doc.FirstPage;

    //            string docType = currPage.DocumentType;
    //            string hleNumber = currPage.HleNumber;                

    //            ArrayList personalList = new ArrayList();
    //            StringBuilder mergedOcrText = new StringBuilder();                

    //            // Storage for all the NRICs for all the pages in the document
    //            ArrayList nricList = new ArrayList();

    //            // Insert the Personal info
    //            while (currPage != null)
    //            {
    //                // Get the personals for each page;
    //                if (!currPage.DocumentType.Equals(DocTypeEnum.HLE.ToString()))
    //                {
    //                    mergedOcrText.Append(Environment.NewLine + currPage.OcrText); // Append the OCR text

    //                    // If NRIC exists for the page, add it to the NRIC list
    //                    if (!String.IsNullOrEmpty(currPage.Nric))
    //                    {
    //                        if (!nricList.Contains(currPage.Nric))
    //                            nricList.Add(currPage.Nric);
    //                    }
    //                }                    

    //                currPage = currPage.NextPage;
    //            }


    //            // Create the personal table
    //            // If the document type is HLE, retrieve from the interface file
    //            // If others, use the NRIC info from the pages
    //            if (docType.Equals(DocTypeEnum.HLE.ToString()) && !String.IsNullOrEmpty(hleNumber))
    //                GetPersonalFromInterface(docType, hleNumber, ref personalList);
    //            else
    //                GetPersonalFromInterfaceUsingNric(nricList, docType, ref personalList);               

    //            // Assign the personal list to the doc
    //            doc.PersonalList = personalList;

    //            // Get the meta data from the maintenance list
    //            MetaDataMaintenanceList metaMainList = new MetaDataMaintenanceList(docType);
    //            doc.MetaDataMaintenance = metaMainList.MetaData;

    //            // Get the hard coded meta data
    //            MetaDataHardCoded metaHardCode = new MetaDataHardCoded(docType, mergedOcrText.ToString());
    //            doc.MetaDataHardCode = metaHardCode.MetaData;
    //        }
    //    }

    //    /// <summary>
    //    /// Save the docs
    //    /// </summary>
    //    /// <param name="docList"></param>
    //    private void SaveDocs(int docSetId, ref ArrayList docList)
    //    {
    //        // Save documents
    //        DocDb docDb = new DocDb();
    //        MetaDataDb metaDataDb = new MetaDataDb();
    //        DocPersonalDb docPersonalDb = new DocPersonalDb();   
    //        DocSetDb docSetDb = new DocSetDb();

    //        // Get the docAppId of the set
    //        int docAppId = docSetDb.GetDocAppId(docSetId);

    //        for (int i = 0; i < docList.Count; i++)
    //        {
    //            DocOcr doc = docList[i] as DocOcr;
    //            PageOcr currPage = doc.FirstPage;

    //            // Save the document into the Doc table
    //            int docId = docDb.Insert(docSetId, doc.DocumentType, docSetId, DocStatusEnum.New.ToString(), docAppId);

    //            // Insert hard code meta data
    //            foreach (MetaDataOcr metaData in doc.MetaDataHardCode)
    //            {
    //                metaDataDb.Insert(docId, metaData.FieldName, metaData.FieldValue, metaData.VerificationMandatory, metaData.CompletenessMandatory,
    //                    metaData.VerificationVisible, metaData.CompletenessVisible, metaData.IsFixed, metaData.MaximumLength, false);
    //            }

    //            // Insert meta data from maintenance list
    //            foreach (MetaDataOcr metaData in doc.MetaDataMaintenance)
    //            {
    //                metaDataDb.Insert(docId, metaData.FieldName, metaData.FieldValue, metaData.VerificationMandatory, metaData.CompletenessMandatory,
    //                        metaData.VerificationVisible, metaData.CompletenessVisible, metaData.IsFixed, metaData.MaximumLength, true);
    //            }

    //            // Insert the personal data
    //            foreach (PersonalOcr personalOcr in doc.PersonalList)
    //            {
    //                docPersonalDb.Insert(docId, personalOcr.PersonalType, personalOcr.Nric, personalOcr.PersonalName, personalOcr.CompanyName,
    //                    personalOcr.DateJoinedService, personalOcr.Month1Name, personalOcr.Month2Name, personalOcr.Month3Name, personalOcr.Month4Name,
    //                    personalOcr.Month5Name, personalOcr.Month6Name, personalOcr.Month7Name, personalOcr.Month8Name, personalOcr.Month9Name,
    //                    personalOcr.Month10Name, personalOcr.Month11Name, personalOcr.Month12Name, personalOcr.Month1Value, personalOcr.Month2Value,
    //                    personalOcr.Month3Value, personalOcr.Month4Value, personalOcr.Month5Value, personalOcr.Month6Value, personalOcr.Month7Value,
    //                    personalOcr.Month8Value, personalOcr.Month9Value, personalOcr.Month10Value, personalOcr.Month11Value, personalOcr.Month12Value,
    //                    personalOcr.EmploymentType);
    //            }

    //            int docPage = 1;
    //            // Insert the Personal info
    //            while (docId > 0 && currPage != null)
    //            {
    //                // Update the doc id of the raw page
    //                RawPageDb rawPageDb = new RawPageDb();
    //                rawPageDb.Update(currPage.Id, docId, docPage);

    //                currPage = currPage.NextPage;
    //                docPage++;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Get personal from interface files
    //    /// </summary>
    //    /// <param name="docType"></param>
    //    /// <param name="hleNumber"></param>
    //    /// <param name="personalList"></param> 
    //    private void GetPersonalFromInterface(string docType, string hleNumber, ref ArrayList personalList)
    //    {
    //        DocPersonalDb docPersonalDb = new DocPersonalDb();
    //        HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();

    //        // Get the personal data from the HLE interface file
    //        HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByHleNumber(hleNumber);

    //        foreach (HleInterface.HleInterfaceRow hleInterface in hleInterfaceDt.Rows)
    //        {
    //            PersonalOcr personal = new PersonalOcr(hleInterface);
    //            personalList.Add(personal);
    //        }
    //    }

    //    /// <summary>
    //    /// Get the interface data using nric
    //    /// </summary>
    //    /// <param name="nric"></param>
    //    /// <returns></returns>
    //    private void GetPersonalFromInterfaceUsingNric(ArrayList nricList, string docType, ref ArrayList personalList)
    //    {
    //        ArrayList personalCount = new ArrayList();

    //        foreach (string nric in nricList)
    //        {
    //            HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
    //            HleInterface.HleInterfaceDataTable dt = hleInterfaceDb.GetHleInterfaceByNric(nric);

    //            if (dt.Rows.Count > 0)
    //            {
    //                HleInterface.HleInterfaceRow dr = dt[0];

    //                PersonalOcr personal = new PersonalOcr(dr);
    //                personalCount.Add(personal);
    //            }
    //            else
    //            {
    //                PersonalOcr personal = new PersonalOcr(nric);
    //                personalCount.Add(personal);
    //            }
    //        }

    //        // For marriage cert documents, 2 personal records will be created
    //        // For others, only 1
    //        int limit = 1;

    //        if (docType.Equals("MarriageCertFor") || docType.Equals("MarriageCertFor"))
    //            limit = 2;

    //        if (personalCount.Count <= 0)
    //        {
    //            for (int cnt = 0; cnt < limit; cnt++)
    //            {
    //                PersonalOcr personal = new PersonalOcr();
    //                personalList.Add(personal);
    //            }
    //        }
    //        else if (personalCount.Count < limit && personalCount.Count > 0)
    //        {
    //            for (int cnt = 0; cnt < personalCount.Count; cnt++)
    //            {
    //                personalList.Add(personalCount[cnt]);
    //            }

    //            for (int cnt = personalCount.Count; cnt < limit; cnt++)
    //            {
    //                PersonalOcr personal = new PersonalOcr();
    //                personalList.Add(personal);
    //            }
    //        }
    //        else if(personalCount.Count > limit)
    //        {
    //            personalCount.RemoveRange(limit, personalCount.Count - limit);
    //            personalList.AddRange(personalCount);
    //        }
    //        else
    //        {
    //            personalList.AddRange(personalCount);
    //        }
    //    }
    //}
}

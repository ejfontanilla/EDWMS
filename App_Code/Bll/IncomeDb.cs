using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.Security;
using DocTableAdapters;
using System.Data;
using System.IO;
using Ionic.Zip;
using Dwms.Dal;
using Dwms.Web;

namespace Dwms.Bll
{
    /// <summary>
    /// Class for Income, IncomeVersion, IncomeDetails and IncomeDoc Tables and all related Income Assessment functions
    /// </summary>
    public class IncomeDb
    {
        #region Retrieve Methods

        public static DataTable GetDataForIncomeAssessment(int docAppId, string nric)
        {
            return IncomeDs.GetDataForIncomeAssessment(docAppId, nric);
        }

         /// <summary>
        /// Mostly used in Assessment Period to get only the Month and Year
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public static DataTable GetAssessmentPeriod(int docAppId, string nric)
        {
            return IncomeDs.GetAssessmentPeriod(docAppId, nric);
        }

        public static int GetNoofNotAcceptedRecordsPerNRIC(int docAppId, string nric)
        {
            return IncomeDs.GetNoofNotAcceptedRecordsPerNRIC(docAppId, nric);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nric"></param>
        /// <returns>int 1 = not complete; 0 = complete</returns>
        public static bool CheckIncomeCompleteByNRIC(string nric, int appPersonalId)
        {
            return IncomeDs.CheckIncomeCompleteByNRIC(nric, appPersonalId);
        }

        public static bool CheckCreditAssessmentByAppPersonalId(int appPersonalId)
        {
            return IncomeDs.CheckCreditAssessmentByAppPersonalId(appPersonalId);
        }


        //public static int GetNoofNotAcceptedRecordsPerIncome(int docAppId, string nric, int incomeId)
        //{
        //    return IncomeDs.GetNoofNotAcceptedRecordsPerIncome(docAppId, nric, incomeId);
        //}

        public static int GetNoofIncomeDetailsPerIncome(int incomeId)
        {
            return IncomeDs.GetNoofIncomeDetailsPerIncome(incomeId);
        }

        public static DataTable GetDocsByIncomeId(int docAppId, string nric, int incomeId)
        {
            return IncomeDs.GetDocsByIncomeId(docAppId, nric, incomeId);
        }

        public static DataTable GetDocsByIncomeId(int docAppId, string nric, int incomeId, int appDocRefId)
        {
            return IncomeDs.GetDocsByIncomeId(docAppId, nric, incomeId,appDocRefId);
        }

        public static DataTable GetApplicantDetails(int docAppId, string nric)
        {
            return IncomeDs.GetApplicantDetails(docAppId, nric);
        }

        public static DataTable GetIncomeVersionByIncome(int incomeId)
        {
            return IncomeDs.GetIncomeVersionByIncome(incomeId);
        }

        #region Added By Edward 29/01/2014 Sales and Resales Changes
        public static DataTable GetIncomeVersionByIncomeId(int incomeId)
        {
            return IncomeDs.GetIncomeVersionByIncomeId(incomeId);
        }
        #endregion

        public static DataTable GetIncomeByAppPersonalIdByMonthByYear(int AppPersonalId, int Month, int Year)
        {
            return IncomeDs.GetIncomeByAppPersonalIdByMonthByYear(AppPersonalId, Month, Year);
        }

        public static DataTable GetIncomeDetailsByIncomeVersion(int incomeVersionId)
        {
            return IncomeDs.GetIncomeDetailsByIncomeVersion(incomeVersionId);
        }
        #region Added By Edward 29/01/2014 Sales and Resales Changes
        public static void UpdateIncomeDetailsOrderNos(int incomeVersionId)
        {
            DataTable dt = IncomeDs.GetIncomeDetailsOrderNos(incomeVersionId);
            int orderNo = 1;
            foreach (DataRow r in dt.Rows)
            {
                if (r.IsNull("OrderNo"))
                {
                    IncomeDs.UpdateIncomeDetailsOrderNo(int.Parse(r["Id"].ToString()), orderNo);
                }
                orderNo++;
            }
        }
        #endregion 

        public static DataTable GetIncomeDataById(int incomeId)
        {                        
            return IncomeDs.GetIncomeDataById(incomeId);
        }

        public static int DeleteIncomeItems(int[] ids)
        {
            return IncomeDs.DeleteIncomeItems(ids);
        }

        public static DataTable GetIncomeDetailTotals(int incomeVersionId)
        {
            return IncomeDs.GetIncomeDetailTotals(incomeVersionId);
        }

        public static DataTable GetIncomeDataByAppPersonalId(int appPersonalId)
        {
            return IncomeDs.GetIncomeDataByAppPersonalId(appPersonalId);
        }

        public static int InsertIncomeVersion(int incomeId, int versionNo, Guid enteredBy)
        {
            return IncomeDs.InsertIncomeVersion(incomeId, versionNo, enteredBy);
        }

        public static int InsertIncomeDetails(int incomeVersionId, string incomeItem, decimal incomeAmount, bool Allowance,
            bool CPFIncome, bool GrossIncome, bool AHGIncome, bool Overtime)
        {
            return IncomeDs.InsertIncomeDetails(incomeVersionId, incomeItem, incomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);
        }

        public static int UpdateIncomeDetails(int incomeDetailsId, int incomeVersionId, string incomeItem, decimal incomeAmount, bool Allowance,
            bool CPFIncome, bool GrossIncome, bool AHGIncome, bool Overtime)
        {
            return IncomeDs.UpdateIncomeDetails(incomeDetailsId, incomeVersionId, incomeItem, incomeAmount, Allowance, CPFIncome, GrossIncome, AHGIncome, Overtime);
        }

        public static int UpdateIncomeVersion(int incomeId, int incomeVersionId)
        {
            return IncomeDs.UpdateIncomeVersion(incomeId, incomeVersionId);
        }

        #region UpdateIncomeVersion Added By Edward 29/01/2014 Sales and Resales Changes
        public static int UpdateIncomeVersion(int Id, Guid enteredBy)
        {
            return IncomeDs.UpdateIncomeVersion(Id, enteredBy);
        }

        public static int UpdateIncomeVersion(int Id, Guid enteredBy, string VersionName)
        {
            return IncomeDs.UpdateIncomeVersion(Id, enteredBy, VersionName);
        }

        public static int UpdateIncomeDetailsOrderNo(int IncomeDetailsId, string action, int IncomeVersionId, int? CurrentOrderNo)
        {
            return IncomeDs.UpdateIncomeDetailsOrderNo(IncomeDetailsId, action, IncomeVersionId, CurrentOrderNo);
        }
        #endregion

        public static int UpdateIncomeSetToBlank(int incomeId, bool setToBlank)
        {
            return IncomeDs.UpdateIncomeSetToBlank(incomeId, setToBlank);
        }

        public static DataTable GetAHGFromIncomeTableByAppPersonalId(int appPersonalId)
        {
            return IncomeDs.GetAHGFromIncomeTableByAppPersonalId(appPersonalId);
        }


        #region InsertMonthYearInIncomeTable Modified By Edward 29/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
        public static void InsertMonthYearInIncomeTable(int docAppId, IncomeMonthsSourceEnum source)
        {
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppdt = docAppDb.GetDocAppById(docAppId);
            int noOfIncomeMonths = 0;
            string refType = string.Empty;
            if (docAppdt.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docAppdt[0];
                refType = docAppRow.RefType.ToUpper().Trim();

                #region FOR COS
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                    HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);
                    
                    foreach(HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                    {                             
                        AppPersonalDb appPersonalDb = new AppPersonalDb();
                        AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(hleInterfaceRow.Nric, docAppId);
                        string personalType = string.Empty;
                        AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                        if (!appPersonalRow.Folder.ToString().ToUpper().Equals("OTHERS") && !appPersonalRow.PersonalType.Trim().Equals(string.Empty))
                        {
                            #region Get NoOfIncomeMonths
                            if (hleInterfaceRow.IsNoOfIncomeMonthsNull())
                            {
                                GetIncomeMonthsFromHousehold(docAppRow.RefNo, appPersonalRow.Nric, noOfIncomeMonths, appPersonalRow.Id, docAppId, source, out noOfIncomeMonths);
                                appPersonalDb.UpdateMonthToLEAS(appPersonalRow.Id, noOfIncomeMonths);
                                hleInterfaceDb.UpdateNoOfIncomeMonths(docAppRow.RefNo, appPersonalRow.Nric, noOfIncomeMonths);

                            }                          
                            else
                                noOfIncomeMonths = hleInterfaceRow.NoOfIncomeMonths;
                            #endregion

                            #region Insert Income Months

                            InsertIncomeMonths(docAppId, appPersonalRow.Id, appPersonalRow.Nric, noOfIncomeMonths, docAppRow.RefNo, source, "HLE");                            

                            #endregion
                        }                        
                    }
                }
                #endregion

                #region FOR RESALE
                else if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))
                {
                    ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                    ResaleInterface.ResaleInterfaceDataTable resaleInterfaceDt = resaleInterfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);
                    foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                    {
                        AppPersonalDb appPersonalDb = new AppPersonalDb();
                        AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(resaleInterfaceRow.Nric, docAppId);
                        string personalType = string.Empty;
                        AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                        if (!appPersonalRow.Folder.ToString().ToUpper().Equals("OTHERS") && !appPersonalRow.PersonalType.Trim().Equals(string.Empty))
                        {
                            #region Get NoOfIncomeMonths
                            if (resaleInterfaceRow.IsNoOfIncomeMonthsNull())
                            {
                                GetIncomeMonthsFromResale(docAppRow.RefNo, appPersonalRow.Nric, noOfIncomeMonths, appPersonalRow.Id, docAppId, source, out noOfIncomeMonths);
                                appPersonalDb.UpdateMonthToLEAS(appPersonalRow.Id, noOfIncomeMonths);
                                resaleInterfaceDb.UpdateNoOfIncomeMonths(docAppRow.RefNo, appPersonalRow.Nric, resaleInterfaceRow.ApplicantType, noOfIncomeMonths);
                            }
                            else
                                noOfIncomeMonths = resaleInterfaceRow.NoOfIncomeMonths;

                            #endregion

                            #region Insert IncomeMonths

                            InsertIncomeMonths(docAppId, appPersonalRow.Id, appPersonalRow.Nric, noOfIncomeMonths, docAppRow.RefNo, source, "RESALE");                            

                            #endregion

                        }                    
                    }
                }
                #endregion

                #region FOR SALES
                else if (refType.Contains(ReferenceTypeEnum.SALES.ToString()))
                {
                    SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                    SalesInterface.SalesInterfaceDataTable salesInterfaceDt = salesInterfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);
                    foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                    {
                        AppPersonalDb appPersonalDb = new AppPersonalDb();
                        AppPersonal.AppPersonalDataTable appPersonaldt = appPersonalDb.GetAppPersonalByNricAndDocAppId(salesInterfaceRow.Nric, docAppId);
                        string personalType = string.Empty;
                        AppPersonal.AppPersonalRow appPersonalRow = appPersonaldt[0];
                        if (!appPersonalRow.Folder.ToString().ToUpper().Equals("OTHERS") && !appPersonalRow.PersonalType.Trim().Equals(string.Empty))
                        {
                            #region Get NoOfIncomeMonths
                            if (salesInterfaceRow.IsNoOfIncomeMonthsNull())
                            {
                                GetIncomeMonthsFromSales(docAppRow.RefNo, appPersonalRow.Nric, noOfIncomeMonths, appPersonalRow.Id, docAppId, source, out noOfIncomeMonths);
                                appPersonalDb.UpdateMonthToLEAS(appPersonalRow.Id, noOfIncomeMonths);
                                salesInterfaceDb.UpdateNoOfIncomeMonths(docAppRow.RefNo, appPersonalRow.Nric, noOfIncomeMonths);
                            }
                            else
                                noOfIncomeMonths = salesInterfaceRow.NoOfIncomeMonths;

                            #endregion

                            #region Insert Income Months

                            InsertIncomeMonths(docAppId, appPersonalRow.Id, appPersonalRow.Nric, noOfIncomeMonths, docAppRow.RefNo, source, "SALES");
                           
                            #endregion
                        }
                    }
                }
                #endregion
            }                      
        }
        #endregion

        //Added By Edward 17/3/2014
        private static void InsertIncomeMonths(int docAppId, int appPersonalId, string nric, int noOfIncomeMonths, string refNo, IncomeMonthsSourceEnum source, string division)
        {
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            DataTable dtAppPersonal = appPersonalDb.GetAppPersonalDocumentByDocAppIdAndAppPersonalId(docAppId, appPersonalId);
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;

            if (dtAppPersonal.Rows.Count > 0)
            {
                #region If AppPersonal have documents

                //Modified //DataRow rowAppPersonal = dtAppPersonal.Rows[0]; to Foreach by Edward on 14/3/2014
                string appPersonalEndDate = string.Empty;
                string tempStartDate = string.Empty;
                foreach (DataRow rowAppPersonal in dtAppPersonal.Rows)
                {
                    if (Format.CheckDateTime(rowAppPersonal["EndDate"].ToString(), out appPersonalEndDate))
                    {
                        if (string.IsNullOrEmpty(tempStartDate))
                            tempStartDate = appPersonalEndDate;
                        else if (DateTime.Parse(appPersonalEndDate) > DateTime.Parse(tempStartDate))
                            tempStartDate = appPersonalEndDate;
                    }
                        
                }

                DataTable dti = IncomeDb.GetDataForIncomeAssessment(docAppId, nric);
                //if it is not zero it will not insert anymore
                if (dti.Rows.Count == 0)
                {
                    //modified by Edward changing rowAppPersonal["EndDate"].ToString() to appPersonalEndDate 14/3/2014
                    startDate = DateTime.Parse(!string.IsNullOrEmpty(tempStartDate) ? tempStartDate : DateTime.Now.ToShortDateString()).AddMonths(-(noOfIncomeMonths - 1));
                    endDate = DateTime.Parse(!string.IsNullOrEmpty(tempStartDate) ? tempStartDate : DateTime.Now.ToShortDateString());

                    int IncomeId = 0;

                    for (var month = startDate.Date; month.Date <= endDate.Date; month = month.AddMonths(1))
                    {
                        if (source == IncomeMonthsSourceEnum.C)
                        {
                            Guid? userId = (Guid)Membership.GetUser().ProviderUserKey;
                            IncomeId = InsertIncome(0, month.Month, month.Year, "SGD", 1, userId.Value, appPersonalId);
                        }
                        else
                            IncomeId = InsertIncome(0, month.Month, month.Year, "SGD", 1, appPersonalId);
                    }
                }
                #endregion
            }
            else
            {
                if(division.ToUpper() == "HLE")
                    #region if there are no documents, get the Income Months from the Household Structure
                    GetIncomeMonthsFromHousehold(refNo, nric, noOfIncomeMonths, appPersonalId, docAppId, source, out noOfIncomeMonths);
                    #endregion
                else if (division.ToUpper() == "RESALE")
                    #region if there are no documents, get the Income Months from the Resale Structure
                    GetIncomeMonthsFromResale(refNo, nric, noOfIncomeMonths, appPersonalId, docAppId, source, out noOfIncomeMonths);
                    #endregion
                else if (division.ToUpper() == "SALES")
                    #region if there are no documents, get the Income Months from the Resale Structure
                    GetIncomeMonthsFromSales(refNo, nric, noOfIncomeMonths, appPersonalId, docAppId, source, out noOfIncomeMonths);
                    #endregion
            }                     

        }

        #region GetIncomeMonthsFromHousehold Modified By Edward 29/01/2014 Get Income Months from Household when NoOfIncomeMonths is Null
        //Separate the Inserting of Income Months to another method by Edward 29/01/2014 Sales and Resales Changes
        private static void GetIncomeMonthsFromHousehold(string docAppRefNo, string appPersonalNric, int hleInterfaceNoOfIncomeMonths, int appPersonalId, int docAppId, IncomeMonthsSourceEnum source, out int NoOfIncomeMonths)
        {

            NoOfIncomeMonths = 0;

            #region this part will get the MonthYears from HLEInterface table and add to List
            DataTable dt = IncomeDs.GetIncomeMonthsFromHleInterface(docAppRefNo, appPersonalNric, docAppId);
            List<string> strYearMonth = new List<string>();
            if (dt.Rows.Count > 0)
            {
                string IncDate = string.Empty;
                string Inc = string.Empty;
                foreach (DataRow rowHleInterface in dt.Rows)
                {
                    for (int i = 12; i > 0; i--)
                    {
                        if (CheckIncDateIsValidIncomeMonth(rowHleInterface["Inc" + i + "Date"].ToString().Trim()))
                        {
                            strYearMonth.Add(rowHleInterface["Inc" + i + "Date"].ToString());
                        }
                    }
                }
            NoOfIncomeMonths = strYearMonth.Count;
            }
            #endregion

            #region this part will insert the MonthYears from the list to Income table if there are still no records
            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, appPersonalNric);
            if (IncomeDt.Rows.Count == 0)            
                InsertIncomeMonthsFromHousehold(strYearMonth, docAppId, appPersonalNric, hleInterfaceNoOfIncomeMonths, source, appPersonalId);            
            #endregion
        }
        #endregion 

        #region Added this method to accommodate Resale by Edward 29/01/2014 Sales and Resales Changes
        private static void GetIncomeMonthsFromResale(string docAppRefNo, string appPersonalNric, int ResaleInterfaceNoOfIncomeMonths, int appPersonalId, int docAppId, IncomeMonthsSourceEnum source, out int NoOfIncomeMonths)
        {

            NoOfIncomeMonths = 0;

            #region this part will get the MonthYears from ResaleInterface table and add to List
            DataTable dt = IncomeDs.GetIncomeMonthsFromResaleInterface(docAppRefNo, appPersonalNric, docAppId);
            List<string> strYearMonth = new List<string>();
            if (dt.Rows.Count > 0)
            {
                string IncDate = string.Empty;
                string Inc = string.Empty;
                foreach (DataRow rowResaleInterface in dt.Rows)
                {
                    for (int i = 15; i > 0; i--)
                    {
                        if (CheckIncDateIsValidIncomeMonth(rowResaleInterface["Inc" + i + "Date"].ToString().Trim()))
                        {
                            strYearMonth.Add(rowResaleInterface["Inc" + i + "Date"].ToString());
                        }
                    }
                }
                NoOfIncomeMonths = strYearMonth.Count;
            }
            #endregion

            #region this part will insert the MonthYears from the list to Income table if there are still no records
            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, appPersonalNric);
            if (IncomeDt.Rows.Count == 0)
                InsertIncomeMonthsFromHousehold(strYearMonth, docAppId, appPersonalNric, ResaleInterfaceNoOfIncomeMonths, source, appPersonalId);
            #endregion
        }
        #endregion

        #region Added this method to accommodate Sales by Edward 03/02/2014 Sales and Resales Changes
        private static void GetIncomeMonthsFromSales(string docAppRefNo, string appPersonalNric, int SalesInterfaceNoOfIncomeMonths, int appPersonalId, int docAppId, IncomeMonthsSourceEnum source, out int NoOfIncomeMonths)
        {

            NoOfIncomeMonths = 0;

            #region this part will get the MonthYears from SalesInterface table and add to List           
            DataTable dt = IncomeDs.GetIncomeMonthsFromSalesInterface(docAppRefNo, appPersonalNric, docAppId);
            List<string> strYearMonth = new List<string>();
            if (dt.Rows.Count > 0)
            {
                string IncDate = string.Empty;
                string Inc = string.Empty;
                foreach (DataRow rowResaleInterface in dt.Rows)
                {
                    for (int i = 12; i > 0; i--)
                    {
                        if (CheckIncDateIsValidIncomeMonth(rowResaleInterface["Inc" + i + "Date"].ToString().Trim()))
                        {
                            strYearMonth.Add(rowResaleInterface["Inc" + i + "Date"].ToString());
                        }
                    }
                }
                NoOfIncomeMonths = strYearMonth.Count;
            }
            #endregion

            #region this part will insert the MonthYears from the list to Income table if there are still no records
            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, appPersonalNric);
            if (IncomeDt.Rows.Count == 0)
                InsertIncomeMonthsFromHousehold(strYearMonth, docAppId, appPersonalNric, SalesInterfaceNoOfIncomeMonths, source, appPersonalId);
            #endregion
        }
        #endregion

        #region Created a separated method from GetIncomeMonthsFromHousehold by Edward 29/01/2014 Sales and Resales Changes
        private static void InsertIncomeMonthsFromHousehold(List<string> strYearMonth, int docAppId, string appPersonalNric, int NoOfIncomeMonths, IncomeMonthsSourceEnum source, int appPersonalId)
        {                        
            if (strYearMonth.Count > 0)
            {
                int j = 0;
                foreach (string str in strYearMonth)
                {
                    if (j < NoOfIncomeMonths)
                    {
                        string stryear = str.Substring(0, 4);
                        string strmonth = str.Substring(4, 2);
                        int IncomeId = 0;
                        if (source == IncomeMonthsSourceEnum.W)
                            IncomeId = InsertIncome(0, int.Parse(strmonth), int.Parse(stryear), "SGD", 1, appPersonalId);
                        else
                        {
                            MembershipUser user = Membership.GetUser();
                            Guid currentUserId = (Guid)user.ProviderUserKey;
                            IncomeId = InsertIncome(0, int.Parse(strmonth), int.Parse(stryear), "SGD", 1, currentUserId, appPersonalId);
                        }
                    }
                    else
                        break;
                    j++;
                }
            }                       
        }
        #endregion 


        private static bool CheckIncDateIsValidIncomeMonth(string YearMonth)
        {
            bool result = false;
            try
            {
                if (string.IsNullOrEmpty(YearMonth))
                    return result;
                if (YearMonth.Length != 6)
                    return result;
                int test;
                if (!int.TryParse(YearMonth, out test))
                    return result;
                if (int.Parse(YearMonth.Substring(0, 4)) < 2000)
                    return result;
                if (int.Parse(YearMonth.Substring(4, 2)) > 12)
                    return result;

                result = true;
                return result;
            }
            catch (Exception)
            {
                result = false;
                return result;
            }                        
        }

        public static int InsertIncome(int incomeVersionId, int incomeMonth, int incomeYear,
            string currency, decimal currencyRate, Guid? enteredBy, int appPersonalId)
        {
            return IncomeDs.InsertIncome(incomeVersionId, incomeMonth, incomeYear, currency, currencyRate, enteredBy, appPersonalId);
        }

        public static int InsertIncome(int incomeVersionId, int incomeMonth, int incomeYear,
            string currency, decimal currencyRate, int appPersonalId)
        {
            return IncomeDs.InsertIncome(incomeVersionId, incomeMonth, incomeYear, currency, currencyRate, appPersonalId);
        }
        //public static int InsertIncomeDocs(int incomeId, int AppDocRefId)
        //{
        //    return IncomeDs.InsertIncomeDocs(incomeId, AppDocRefId);
        //}

        public static int DeleteAllIncomeDetailsByVersionId(int incomeVersionId)
        {
            return IncomeDs.DeleteAllIncomeDetailsByVersionId(incomeVersionId);
        }

        public static int DeleteAllIncomeVersionsByIncomeId(int incomeId)
        {
            return IncomeDs.DeleteAllIncomeVersionsByIncomeId(incomeId);
        }

        public static int DeleteAllIncomeByDocAppIdAndNRIC(int docAppId, string Nric)
        {
            return IncomeDs.DeleteAllIncomeByDocAppIdAndNRIC(docAppId, Nric);
        }

        public static int DeleteIncomeByIncomeId(int incomeId)
        {
            return IncomeDs.DeleteIncomeByIncomeId(incomeId);
        }

        public static DataTable GetDocsByDocAppIdAndNric(int docAppId, string nric)
        {
            return IncomeDs.GetDocsByDocAppIdAndNric(docAppId, nric);
        }

        public static int UpdateIncome(int IncomeId, decimal CurrencyRate, string Currency)
        {
            return IncomeDs.UpdateIncome(IncomeId, CurrencyRate, Currency);
        }

        public static int DeleteAllExtractionByAppPersonalId(int AppPersonalId)
        {
            return IncomeDs.DeleteAllExtractionByAppPersonalId(AppPersonalId);
        }

        public static int DeleteCreditAssessment(int AppPersonalId)
        {
            return IncomeDs.DeleteCreditAssessment(AppPersonalId);
        }

        public static int DeleteCreditAssessment(int AppPersonalId, string IncomeItem, string IncomeType)
        {
            return IncomeDs.DeleteCreditAssessment(AppPersonalId, IncomeItem, IncomeType);
        }

        public static int DeleteIncomeVersionById(int Id)
        {
            return IncomeDs.DeleteIncomeVersionById(Id);
        }

        #endregion  
        
        #region GetOrderByMonthYear
        public static DataTable GetOrderByMonthYear(int docAppId, string nric, string sortBy)
        {
            return IncomeDs.GetOrderByMonthYear(docAppId, nric, sortBy);
        }

        public static DataTable GetOrderByMonthYear(int appPersonalId, string sortBy)
        {
            return IncomeDs.GetOrderByMonthYear(appPersonalId, sortBy);
        }
        #endregion

        public static DataTable CreateIncomeTemplate(DataTable dt, string name)
        {
            IncomeTemplateItemDb incomeTemplateItemDb = new IncomeTemplateItemDb();
            IncomeTemplateItem.IncomeTemplateItemsDataTable itiDT = incomeTemplateItemDb.GetItemsByIncomeTemplateName(name);
            if (itiDT.Rows.Count > 0)
            {
                DataRow newrow;
                foreach (IncomeTemplateItem.IncomeTemplateItemsRow row in itiDT)
                {
                    newrow = dt.NewRow();
                    newrow["Id"] = 0;
                    newrow["IncomeVersionId"] = "0";
                    newrow["IncomeItem"] = row.ItemName;
                    newrow["IncomeAmount"] = 0;
                    newrow["Allowance"] = !row.IsAllowanceNull() ? row.Allowance : false;
                    newrow["CPFIncome"] = !row.IsCAIncomeNull() ? row.CAIncome : false;
                    newrow["GrossIncome"] = !row.IsGrossIncomeNull() ? row.GrossIncome : false;
                    newrow["AHGIncome"] = !row.IsAHGIncomeNull() ? row.AHGIncome : false;
                    newrow["Overtime"] = !row.IsOvertimeNull() ? row.Overtime : false;
                    dt.Rows.Add(newrow);
                }
            }
            return dt;
        }
        /// <summary>
        /// Get the IncomeItems to be used in the Housing Grant Worksheet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataTable GetDistinctIncomeItemByAppPersonalId(int id)
        {
            return IncomeDs.GetDistinctIncomeItemByAppPersonalId(id);
        }

        #region GetIncomeDetailsByIncomeIdAndIncomeItem
        /// <summary>
        /// Get the IncomeDetails to be used the Housing grant WORKSHEET
        /// </summary>
        /// <param name="IncomeId"></param>
        /// <param name="IncomeItem"></param>
        /// <returns></returns>
        public static DataTable GetIncomeDetailsByIncomeIdAndIncomeItem(int IncomeId, string IncomeItem)
        {
            return IncomeDs.GetIncomeDetailsByIncomeIdAndIncomeItem(IncomeId, IncomeItem);
        }

        public static DataTable GetIncomeDetailsByIncomeIdAndIncomeItem(int IncomeId, string IncomeItem, string IncomeType)
        {
            return IncomeDs.GetIncomeDetailsByIncomeIdAndIncomeItem(IncomeId, IncomeItem, IncomeType);
        }

        public static DataTable GetIncomeDetailsById(int id)
        {
            return IncomeDs.GetIncomeDetailsById(id);
        }

        #region Insert Log for Extraction added by Edward 24/02/2014 Add Icon and Action Log

        public static int InsertExtractionLog(int id, string message,string message2, LogActionEnum logAction, LogTypeEnum logType, Guid userId)
        {
            MembershipUser user = Membership.GetUser();
            ProfileDb profileDb = new ProfileDb();
            
            string username = profileDb.GetUserFullName(userId);

            LogActionDb logActionDb = new LogActionDb();
            Guid currentUserId = (Guid)user.ProviderUserKey;
            logActionDb.Insert(currentUserId, logAction, username, message, message2, string.Empty, logType, id);

            return 1;
        }


        #region Created By Edward February 21, 2014 Meeting : Method will create a datatable for Income Data Tab
        public static DataTable CreateIncomeDataTable(AppPersonal.AppPersonalDataTable dtAppPersonal, int intDocAppId, string strNRIC)
        {
            DataTable dtIncomeData = new DataTable();
            List<int> listIncomeId;

            listIncomeId = new List<int>();
            AppPersonal.AppPersonalRow rAppPersonal = dtAppPersonal[0];
            decimal decIncomeAmount = 0;
            string strLowestIncomeAmount = string.Empty;
            int intYes = 0;
            int intNo = 0;
            int intMonthsToPass = !rAppPersonal.IsMonthsToLEASNull() ? rAppPersonal.MonthsToLEAS : 0;
            decimal CATotal = 0;
            DataTable dtIncome = IncomeDb.GetOrderByMonthYear(intDocAppId, strNRIC, "DESC");
            #region Add Income Component and Type Of Income Heading, avg headers
            CreateDataColumnForHeaders(dtIncomeData, "IncomeComponent");
            CreateDataColumnForHeaders(dtIncomeData, "TypeOfIncome");
            CreateDataColumnForHeaders(dtIncomeData, "CreditAssessment");
            CreateDataColumnForHeaders(dtIncomeData, string.Format("AvgMonths_{0}", dtIncome.Rows.Count < 3 ? dtIncome.Rows.Count.ToString() : "3"));
            CreateDataColumnForHeaders(dtIncomeData, string.Format("MonthsToPass_{0}", intMonthsToPass));
            CreateDataColumnForHeaders(dtIncomeData, string.Format("Lowest_{0}", intMonthsToPass));
            CreateDataColumnForHeaders(dtIncomeData, "YesNo");
            #endregion

            #region Add Header MonthYearCells, then store the Income Id in a List to be used to get the values for each month
            if (dtIncome.Rows.Count > 0)
            {
                foreach (DataRow row in dtIncome.Rows)
                {
                    System.Globalization.DateTimeFormatInfo info = new System.Globalization.DateTimeFormatInfo();
                    CreateDataColumnForHeaders(dtIncomeData, string.Format("{0} {1}", info.GetAbbreviatedMonthName(int.Parse(row["IncomeMonth"].ToString())), row["IncomeYear"].ToString()));
                    listIncomeId.Add(int.Parse(row["Id"].ToString()));
                }
            }
            #endregion

            int intMonthsToLeas = !rAppPersonal.IsMonthsToLEASNull() ? rAppPersonal.MonthsToLEAS : 0;            
            DataTable IncomeItemsDt = IncomeDb.GetDistinctIncomeItemByAppPersonalId(rAppPersonal.Id);
            foreach (DataRow IncomeItemsRow in IncomeItemsDt.Rows)
            {
                List<string> listRow = new List<string>();
                decimal past3 = 0;
                decimal past12 = 0;
                int i = 0;

                if (!string.IsNullOrEmpty(IncomeItemsRow["IncomeType"].ToString())) //This will check if the IncomeItem does not have any IncomeType, that is the user did not check anything
                {
                    #region Creates the Cells of the Name of IncomeItem and IncomeType
                    listRow.Add(IncomeItemsRow["IncomeItem"].ToString());
                    listRow.Add(IncomeItemsRow["IncomeType"].ToString());
                    #endregion

                    #region Get the sum of past3 months, past12 months, Yes/No, Lowest Income
                    foreach (int intIncomeId in listIncomeId)
                    {
                        DataTable IncomeItemDt = IncomeDb.GetIncomeDetailsByIncomeIdAndIncomeItem(intIncomeId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
                        if (IncomeItemDt.Rows.Count > 0)
                        {
                            decimal decTotalAmountOfIncomeItem = 0;
                            foreach (DataRow r in IncomeItemDt.Rows)
                            {
                                decTotalAmountOfIncomeItem = decTotalAmountOfIncomeItem + decimal.Parse(r["IncomeAmount"].ToString());
                            }
                            DataRow IncomeItemRow = IncomeItemDt.Rows[0];
                            i++;
                            decIncomeAmount = decIncomeAmount + (decTotalAmountOfIncomeItem / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString()));
                            if (i == 3)
                                past3 = decIncomeAmount;

                            if (i == intMonthsToLeas)   //Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                                past12 = decIncomeAmount;
                            if (i <= intMonthsToLeas)
                            {
                                if (string.IsNullOrEmpty(strLowestIncomeAmount))
                                    strLowestIncomeAmount = (decimal.Parse(IncomeItemRow["IncomeAmount"].ToString()) / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString();
                                else if (decimal.Parse(strLowestIncomeAmount) > decimal.Parse(IncomeItemRow["IncomeAmount"].ToString()) / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString()))
                                    strLowestIncomeAmount = (decimal.Parse(IncomeItemRow["IncomeAmount"].ToString()) / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString();
                            }
                            intYes = intYes + 1;
                        }
                        else
                        {
                            i++;
                            if (i == 3)
                                past3 = decIncomeAmount;
                            if (i == intMonthsToLeas)   //Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                                past12 = decIncomeAmount;
                            intNo = intNo + 1;
                        }
                    }
                    #endregion

                    #region Gets the IncomeType and IncomeItem
                    CreditAssessmentDb CAdb = new CreditAssessmentDb();
                    CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(rAppPersonal.Id, IncomeItemsRow["IncomeItem"].ToString(),
                        IncomeItemsRow["IncomeType"].ToString());
                    if (CADt.Rows.Count > 0)
                    {
                        CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                        listRow.Add(Format.GetDecimalPlacesWithoutRounding(decimal.Parse(CARow.CreditAssessmentAmount.ToString())).ToString());
                        CATotal = CATotal + CARow.CreditAssessmentAmount;
                    }
                    else
                        listRow.Add(" NA ");
                    #endregion

                    #region Add Average cells, Lowest, Yes/No Values
                    listRow.Add(string.Format("{0}", i < 3 ? Format.GetDecimalPlacesWithoutRounding(decIncomeAmount / i).ToString() : Format.GetDecimalPlacesWithoutRounding(past3 / 3).ToString()));                       
                    listRow.Add(string.Format("{0}", intMonthsToLeas > 0 ? Format.GetDecimalPlacesWithoutRounding(past12 / intMonthsToLeas).ToString() : " - "));
                    listRow.Add(!string.IsNullOrEmpty(strLowestIncomeAmount) ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(strLowestIncomeAmount)).ToString() : " - ");
                    listRow.Add(string.Format("{0}/{1}", intYes, intNo));
                    #endregion

                    #region Gets the values for each Month using the IncomeId in the List
                    foreach (int intIncomeId in listIncomeId)
                    {
                        DataTable IncomeItemDt = IncomeDb.GetIncomeDetailsByIncomeIdAndIncomeItem(intIncomeId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
                        if (IncomeItemDt.Rows.Count > 0)
                        {
                            decimal decTotalAmountOfIncomeItem = 0;
                            foreach (DataRow r in IncomeItemDt.Rows)
                            {
                                decTotalAmountOfIncomeItem = decTotalAmountOfIncomeItem + decimal.Parse(r["IncomeAmount"].ToString());
                            }
                            DataRow IncomeItemRow = IncomeItemDt.Rows[0];
                            listRow.Add(Format.GetDecimalPlacesWithoutRounding(decTotalAmountOfIncomeItem / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString());
                        }
                        else
                            listRow.Add(" - ");
                    }
                    #endregion
                }

                #region Reseting values to 0 or empty
                strLowestIncomeAmount = string.Empty;
                intYes = 0;
                intNo = 0;
                decIncomeAmount = 0;
                CreateDataRow(dtIncomeData, listRow);
                listRow.Clear();  
                #endregion                              
            }
            #region Sorting the Income Types
            DataView dv = dtIncomeData.DefaultView;
            dv.RowFilter = "TypeOfIncome = 'Gross Income'";
            DataTable dtOrdered = dv.ToTable();
            dv.RowFilter = "TypeOfIncome = 'Allowance'";
            dtOrdered.Merge(dv.ToTable());
            dv.RowFilter = "TypeOfIncome = 'Overtime'";
            dtOrdered.Merge(dv.ToTable());
            dv.RowFilter = "TypeOfIncome = 'AHG Income'";
            dtOrdered.Merge(dv.ToTable());
            #endregion
            CreateDataRowCA(dtOrdered, CATotal);

            return dtOrdered;
        }

        private static void CreateDataColumnForHeaders(DataTable dtIncomeData, string strHeaderName)
        {
            DataColumn dtColumn;
            dtColumn = new DataColumn();
            dtColumn.ColumnName = strHeaderName;
            dtIncomeData.Columns.Add(dtColumn);
        }

        private static void CreateDataRow(DataTable dtIncomeData, List<string> listRow)
        {
            DataRow row = dtIncomeData.NewRow();
            for(int i = 0 ; i < listRow.Count ; i++)
            {
                row[i] = listRow[i];
            }
            dtIncomeData.Rows.Add(row);
        }

        private static void CreateDataRowCA(DataTable dtOrdered, decimal total)
        {
            DataRow row = dtOrdered.NewRow();
            row[1] = "CA Total";
            row[2] = total;
            dtOrdered.Rows.Add(row);
        }

        #endregion

        #endregion
        #endregion

        #region Modified by edward changes from 20140604 Meeting
        public static void SetIncomeToZero(int appPersonalId, int docAppId)
        {
            DataTable dt = IncomeDb.GetIncomeDataByAppPersonalId(appPersonalId);
            if (dt.Rows.Count > 0)
            {
                string strIncomeComponent = "Income";
                string strIncomeType = "Gross Income";
                decimal decAmount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    int NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(row["Id"].ToString()), 1, (Guid)Membership.GetUser().ProviderUserKey);
                    int result = IncomeDb.InsertIncomeDetails(NewVersionId, strIncomeComponent, decAmount, false, false, true, false, false);
                    int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(int.Parse(row["Id"].ToString()), NewVersionId);
                }
                CreditAssessmentDb CAdb = new CreditAssessmentDb();
                CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, strIncomeComponent, strIncomeType);
                if (CADt.Rows.Count > 0)
                {
                    CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                    CAdb.Update(appPersonalId, decAmount, (Guid)Membership.GetUser().ProviderUserKey, true);
                }
                else
                    CAdb.Insert(appPersonalId, strIncomeComponent, strIncomeType, decAmount, (Guid)Membership.GetUser().ProviderUserKey, true);

                #region Added By Edward 24/02/2014  Add Icon and Action Log
                IncomeDb.InsertExtractionLog(docAppId, string.Empty, string.Empty,
                    LogActionEnum.Set_Income_To_Zero, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
                #endregion
            }            
        }

        #endregion

        #region Added By Edward Income Extraction Changes 2014/6/17     Add HLE Form in View Consolidated PDF in Income Extraction
        public static DataTable GetHLEFormByDocApp(int docAppId)
        {
            return IncomeDs.GetHLEFormByDocApp(docAppId);
        }

        public static List<int> GetDocIdForTraverse(int docAppId, string nric)
        {
            List<int> listDocId = new List<int>();
            DataTable dt = IncomeDb.GetDataForIncomeAssessment(docAppId, nric);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow r in dt.Rows)
                {
                    DataTable dt1 = IncomeDb.GetDocsByIncomeId(docAppId, nric, int.Parse(r["Id"].ToString()));
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (DataRow r1 in dt1.Rows)
                        {
                            if (r1["ImageAccepted"].ToString() == "Y")
                                listDocId.Add(int.Parse(r1["DocId"].ToString()));
                        }
                    }
                }
            }
            return listDocId;
        }
        #endregion

        #region Modifed by Edward 2014/07/22 Changes in Zoning Page
        public static DataTable GetIncomeDetailsByVersionNoAndIncomeId(int versionNo, int incomeId)
        {
            return IncomeDs.GetIncomeDetailsByVersionNoAndIncomeId(versionNo, incomeId);
        }
        public static DataTable GetIncomeVersionNoByDocAppIdAndNric(int docAppId, string nric)
        {
            return IncomeDs.GetIncomeVersionNoByDocAppIdAndNric(docAppId, nric);
        }

        public static int InsertIncomeDetails(int incomeVersionId, string incomeItem, decimal incomeAmount, bool GrossIncome, bool Allowance,
            bool Overtime, bool AHGIncome, bool CPFIncome, int orderNo)
        {
            return IncomeDs.InsertIncomeDetails(incomeVersionId, incomeItem, incomeAmount, GrossIncome, Allowance, Overtime, AHGIncome, CPFIncome, orderNo);
        }

        public static int UpdateIncomeDetails(int IncomeDetailsId, int incomeVersionId, string incomeItem, decimal incomeAmount, bool GrossIncome, bool Allowance,
            bool Overtime, bool AHGIncome, bool CPFIncome, int OrderNo)
        {
            return IncomeDs.UpdateIncomeDetails(IncomeDetailsId, incomeVersionId, incomeItem, incomeAmount, GrossIncome, Allowance, Overtime, AHGIncome, CPFIncome, OrderNo);
        }

        public static DataTable GetIncomeVersionByIncomeIdAndVersionNo(int incomeId, int versionNo)
        {
            return IncomeDs.GetIncomeVersionByIncomeIdAndVersionNo(incomeId, versionNo);
        }

        public static DataTable GetIncomeItemsByDocAppIdNRICVersionNo(int docAppId, string nric, int VersionNo)
        {
            return IncomeDs.GetIncomeItemsByDocAppIdNricVersionNo(docAppId, nric, VersionNo);
        }

        public static DataTable GetIncomeVersionById(int incomeVersionId)
        {
            return IncomeDs.GetIncomeVersionById(incomeVersionId);
        }

        public static DataTable GetIncomeVersionByDocAppIdAndNricWithProfile(int docAppId, string nric)
        {
            return IncomeDs.GetIncomeVersionByDocAppIdAndNricWithProfile(docAppId, nric);
        }

        public static int DeleteAllIncomeVersionsByDocAppIdAndNric(int docAppId, string nric)
        {
            return IncomeDs.DeleteAllIncomeVersionsByDocAppIdAndNric(docAppId, nric);
        }
        /// <summary>
        /// Updates the IncomeVersion to 0 and IsSetToBlank to True
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="nric"></param>
        /// <param name="isBlank"></param>
        /// <returns></returns>
        public static int UpdateIncomeVersionIdByDocAppIdAndNric(int docAppId, string nric, bool isBlank)
        {
            return IncomeDs.UpdateIncomeVersionIdByDocAppIdAndNric(docAppId, nric, isBlank);
        }

        public static int UpdateIncomeVersionByDocAppIdAndNricAndVersionNo(Guid enteredBy, string VersionName, int docAppId, string nric, int versionNo)
        {
            return IncomeDs.UpdateIncomeVersionByDocAppIdAndNricAndVersionNo(enteredBy, VersionName, docAppId, nric, versionNo);
        }

        public static int DeleteIncomeVersionByDocAppIdAndNricAndVersionNo(int docAppId, string nric, int versionNo)
        {
            return IncomeDs.DeleteIncomeVersionByDocAppIdAndNricAndVersionNo(docAppId, nric, versionNo);
        }

        public static int UpdateIncome(int IncomeId, int versionId, bool setToBlank)
        {
            return IncomeDs.UpdateIncome(IncomeId, versionId, setToBlank);
        }

        public static int DeleteIncomeItemsByVersionNoAndDocAppIdAndNric(int versionNo, int docAppId, string nric)
        {
            return IncomeDs.DeleteIncomeItemsByVersionNoAndDocAppIdAndNric(versionNo, docAppId, nric);
        }

        public static DataTable GetIncomeVersionByDocAppIdAndNricAndVersionNoAndIncomeId(int docAppId, string nric, int versionNo, int incomeId)
        {
            return IncomeDs.GetIncomeVersionByDocAppIdAndNricAndVersionNoAndIncomeId(docAppId, nric, versionNo, incomeId);
        }

        public static int DeleteIncomeItemsByIncomeIdAndVersionNo(int incomeId, int versionNo)
        {
            return IncomeDs.DeleteIncomeItemsByIncomeIdAndVersionNo(incomeId, versionNo);
        }

        public static DataTable GetIncomeVersionNoByDocAppIdAndNricAndIncomeVersionIsZero(int docAppId, string nric)
        {
            return IncomeDs.GetIncomeVersionNoByDocAppIdAndNricAndIncomeVersionIsZero(docAppId, nric);
        }

        #endregion
    }
}
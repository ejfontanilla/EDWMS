using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using CreditAssessmentTableAdapters;
using System.Data;
using Dwms.Dal;


namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for CreditAssessmentDb
    /// </summary>
    public class CreditAssessmentDb
    {
        private CreditAssessmentTableAdapter _Adapter = null;

        protected CreditAssessmentTableAdapter Adapter
        {
            get
            {
                if (_Adapter == null)
                    _Adapter = new CreditAssessmentTableAdapter();

                return _Adapter;
            }
        }


        public CreditAssessment.CreditAssessmentDataTable GetCAByAppPersonalIdByIncomeItemType(int appPersonalId, string component, string type)
        {
            return Adapter.GetCAByAppPersonalIdByIncomeItemType(appPersonalId,component,type);
        }

        public CreditAssessment.CreditAssessmentDataTable GetCreditAssessmentById(int Id)
        {
            return Adapter.GetCreditAssessmentById(Id);
        }

        public static void AutoGenerateCreditAssessment(int appPersonalId)
        {
            string strIncomeItem = string.Empty;
            string strIncomeType = string.Empty;
            decimal decLatestAmount = 0;            
            
            List<int> listIncomeId = new List<int>();

            #region Gets the MonthToLeas
            int intMonthsToLeas = 0;
            string applicantType = string.Empty;
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalById(appPersonalId);
            if (appPersonalDt.Rows.Count > 0)
            {
                listIncomeId = new List<int>();
                AppPersonal.AppPersonalRow appPersonalRow = appPersonalDt[0];
                intMonthsToLeas = !appPersonalRow.IsMonthsToLEASNull() ? appPersonalRow.MonthsToLEAS : 0;
                applicantType = appPersonalRow.PersonalType;
            }
            
            #endregion

            if (!applicantType.Contains(PersonalTypeEnum.OC.ToString()))
            {

                #region Gets the currentUserId
                MembershipUser user = Membership.GetUser();
                Guid currentUserId = (Guid)user.ProviderUserKey;
                #endregion

                #region Stores all the IncomeId for each Month in Descending Order to be used later in getting the amounts
                DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(appPersonalId, "DESC");
                if (IncomeDt.Rows.Count > 0)
                {
                    foreach (DataRow IncomeRow in IncomeDt.Rows)
                    {
                        listIncomeId.Add(int.Parse(IncomeRow["Id"].ToString()));
                    }
                }
                #endregion

                //Get the Items
                DataTable IncomeItemsDt = IncomeDb.GetDistinctIncomeItemByAppPersonalId(appPersonalId);
                foreach (DataRow IncomeItemsRow in IncomeItemsDt.Rows)
                {
                    decimal decAverageXmonths = 0;
                    decimal decAmountToBeUsedForCA = 0;
                    int i = 0;
                    int intLatestCtr = 0;
                    decimal decIncomeAmount = 0;

                    if (!string.IsNullOrEmpty(IncomeItemsRow["IncomeType"].ToString()))         //This will check if the IncomeItem does not have any IncomeType, that is the user did not check anything
                    {
                        strIncomeItem = IncomeItemsRow["IncomeItem"].ToString();
                        strIncomeType = IncomeItemsRow["IncomeType"].ToString();

                        foreach (int intIncomeId in listIncomeId)
                        {
                            DataTable IncomeItemDt = IncomeDb.GetIncomeDetailsByIncomeIdAndIncomeItem(intIncomeId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
                            if (IncomeItemDt.Rows.Count > 0)
                            {
                                #region this segment sums the Amount for each record with the same IncomeItem and IncomeType per MonthYear regardless of Case Sensitive
                                decimal decTotalAmountOfIncomeItem = 0;
                                foreach (DataRow r in IncomeItemDt.Rows)
                                {
                                    decTotalAmountOfIncomeItem = decTotalAmountOfIncomeItem + decimal.Parse(r["IncomeAmount"].ToString());
                                }
                                #endregion

                                #region this segment will get the latest amount to be compared to the average amount later
                                if (intLatestCtr == 0)
                                {
                                    decLatestAmount = Format.GetDecimalPlacesWithoutRounding(decTotalAmountOfIncomeItem);
                                    intLatestCtr++;
                                }
                                #endregion

                                #region this segment will sum up the Income Amount for all MonthYear of intMonthToLeas
                                i++;
                                if (i <= intMonthsToLeas)
                                {
                                    DataRow IncomeItemRow = IncomeItemDt.Rows[0];
                                    decIncomeAmount = decIncomeAmount + (decTotalAmountOfIncomeItem / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString()));
                                }
                                #endregion
                            }
                            else
                            {
                                i++;
                                #region if First Month has no value default of Latest amount is 0
                                if (intLatestCtr == 0)
                                {
                                    decLatestAmount = 0;
                                    intLatestCtr++;
                                }
                                #endregion
                            }
                        }

                        #region this segment gets the Average Amount / intMonthToLeas (formerly i) which i is the number of months and will compare to the Latest Amount to be used for CA
                        //decAverageXmonths = Format.GetDecimalPlacesWithoutRounding(decIncomeAmount / i);
                        decAverageXmonths = intMonthsToLeas > 0 ? Format.GetDecimalPlacesWithoutRounding(decIncomeAmount / intMonthsToLeas) : 0;
                        if (decLatestAmount <= decAverageXmonths)
                            decAmountToBeUsedForCA = Format.GetDecimalPlacesWithoutRounding(decLatestAmount);
                        else
                            decAmountToBeUsedForCA = Format.GetDecimalPlacesWithoutRounding(decAverageXmonths);
                        #endregion

                        //if (!applicantType.Contains(PersonalTypeEnum.OC.ToString()))
                        //    decAmountToBeUsedForCA = 0;

                        #region this segment Updates or Inserts the CA in the database
                        CreditAssessmentDb CAdb = new CreditAssessmentDb();
                        CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, strIncomeItem, strIncomeType);
                        if (CADt.Rows.Count > 0)
                        {
                            CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                            CAdb.Update(int.Parse(CARow["Id"].ToString()), Format.GetDecimalPlacesWithoutRounding(decAmountToBeUsedForCA), currentUserId, true);
                        }
                        else
                            CAdb.Insert(appPersonalId, strIncomeItem, strIncomeType, Format.GetDecimalPlacesWithoutRounding(decAmountToBeUsedForCA), currentUserId, true);
                        #endregion
                    }
                }
            } 
        }

        public int Insert(int AppPersonalId, string IncomeItem, string IncomeType, decimal Amount, Guid? EnteredBy, bool IsGenerated)
        {
            CreditAssessment.CreditAssessmentDataTable dt = new CreditAssessment.CreditAssessmentDataTable();
            CreditAssessment.CreditAssessmentRow row = dt.NewCreditAssessmentRow();

            row.AppPersonalId = AppPersonalId;
            row.IncomeItem = IncomeItem;
            row.IncomeType = IncomeType;
            row.CreditAssessmentAmount = Amount;
            row.EnteredBy = EnteredBy.Value;
            row.DateEntered = DateTime.Now;
            row.IsGenerated = IsGenerated;

            dt.AddCreditAssessmentRow(row);

            Adapter.Update(dt);

            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.CreditAssessment, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }


        public bool Update(int id, decimal Amount, Guid? EnteredBy, bool IsGenerated)
        {
            CreditAssessment.CreditAssessmentDataTable dt = Adapter.GetCreditAssessmentById(id);

            if (dt.Count == 0)
                return false;

            CreditAssessment.CreditAssessmentRow row = dt[0];

            row.CreditAssessmentAmount = Amount;
            row.EnteredBy = EnteredBy.Value;
            row.DateEntered = DateTime.Now;
            row.IsGenerated = IsGenerated;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.CreditAssessment, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }




        public int Insert(int AppPersonalId, string IncomeItem, string IncomeType, decimal Amount, string strEnteredBy, bool IsGenerated)
        {
            MembershipUser user = Membership.GetUser(strEnteredBy);
            Guid currentUserId = (Guid)user.ProviderUserKey;

            CreditAssessment.CreditAssessmentDataTable dt = new CreditAssessment.CreditAssessmentDataTable();
            CreditAssessment.CreditAssessmentRow row = dt.NewCreditAssessmentRow();

            row.AppPersonalId = AppPersonalId;
            row.IncomeItem = IncomeItem;
            row.IncomeType = IncomeType;
            row.CreditAssessmentAmount = Amount;
            row.EnteredBy = currentUserId;
            row.DateEntered = DateTime.Now;
            row.IsGenerated = IsGenerated;

            dt.AddCreditAssessmentRow(row);

            Adapter.Update(dt);

            int id = row.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.CreditAssessment, id.ToString(), OperationTypeEnum.Insert);
            }
            return id;
        }


        public bool Update(int id, decimal Amount, string strEnteredBy, bool IsGenerated)
        {
            MembershipUser user = Membership.GetUser(strEnteredBy);
            Guid currentUserId = (Guid)user.ProviderUserKey;

            CreditAssessment.CreditAssessmentDataTable dt = Adapter.GetCreditAssessmentById(id);

            if (dt.Count == 0)
                return false;

            CreditAssessment.CreditAssessmentRow row = dt[0];

            row.CreditAssessmentAmount = Amount;
            row.EnteredBy = currentUserId;
            row.DateEntered = DateTime.Now;
            row.IsGenerated = IsGenerated;

            int rowsAffected = Adapter.Update(dt);

            if (rowsAffected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.CreditAssessment, id.ToString(), OperationTypeEnum.Update);
            }
            return rowsAffected == 1;
        }


    }
}
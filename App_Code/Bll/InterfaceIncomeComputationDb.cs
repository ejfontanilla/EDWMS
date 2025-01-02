using System;
using System.Collections.Generic;
using System.Web;
using InterfaceIncomeComputationTableAdapters;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for InterfaceIncomeComputationDb
    /// </summary>
    public class InterfaceIncomeComputationDb
    {
        private InterfaceIncomeComputationTableAdapter _InterfaceIncomeComputationAdapter = null;

        protected InterfaceIncomeComputationTableAdapter Adapter
        {
            get
            {
                if (_InterfaceIncomeComputationAdapter == null)
                    _InterfaceIncomeComputationAdapter = new InterfaceIncomeComputationTableAdapter();

                return _InterfaceIncomeComputationAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get interfaces income computation
        /// </summary>
        /// <returns></returns>
        public InterfaceIncomeComputation.InterfaceIncomeComputationDataTable GetInterfaceIncomeComputation()
        {
            return Adapter.GetData();
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="streetCode"></param>
        /// <param name="streetName"></param>
        /// <returns></returns>
        public int Insert(int interfaceId, string houseHoldInc, string caInc, string houseHoldInc2Yr, string schAccnt, 
            string codeLessee, string codeLesseeNew, string codeAhgGrant, string allocBuyer, string eligBuyer,
            string bankLoan, string ABCDELoan, string recipientTag)
        {
            InterfaceIncomeComputation.InterfaceIncomeComputationDataTable dt = new InterfaceIncomeComputation.InterfaceIncomeComputationDataTable();
            InterfaceIncomeComputation.InterfaceIncomeComputationRow r = dt.NewInterfaceIncomeComputationRow();

            r.InterfaceId = interfaceId;
            r.HouseHoldInc = houseHoldInc;
            r.CaInc = caInc;
            r.HouseHoldInc2Yr = houseHoldInc2Yr;
            r.SchAccnt = schAccnt;
            r.CodeLessee = codeLessee;
            r.CodeLesseeNew = codeLesseeNew;
            r.CodeAhgGrant = codeAhgGrant;
            r.AllocBuyer = allocBuyer;
            r.EligBuyer = eligBuyer;
            r.BankLoan = bankLoan;
            r.ABCDELoan = ABCDELoan;
            r.RecipientTag = recipientTag;

            dt.AddInterfaceIncomeComputationRow(r);
            Adapter.Update(dt);
            int id = r.Id;

            if (id > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.InterfaceIncomeComputation, id.ToString(), OperationTypeEnum.Insert);
            }

            return id;
        }
        #endregion
    }
}
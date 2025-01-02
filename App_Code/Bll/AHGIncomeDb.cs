using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Text;
using System.Web;
using System.Web.Security;
using System.Runtime.InteropServices;
using AHGIncomeTableAdapters;
using System.Web.Security;
using Dwms.Dal;
using System.Collections;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AHGIncomeDb
    /// </summary>
    public class AHGIncomeDb
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        private AHGIncomeTableAdapter _AHGIncomeTableAdapter = null;

        protected AHGIncomeTableAdapter Adapter
        {
            get
            {
                if (_AHGIncomeTableAdapter == null)
                    _AHGIncomeTableAdapter = new AHGIncomeTableAdapter();

                return _AHGIncomeTableAdapter;
            }
        }


        public AHGIncome.AHGIncomeDataTable GetAHGIncomeByAppPersonalId(int AppPersonalId)
        {
            return Adapter.GetAHGIncomeByAppPersonalId(AppPersonalId);
        }


        public int Insert(AHGIncomeDb cls, string capture)
        {
            // Inser

            AHGIncome.AHGIncomeDataTable dt = new AHGIncome.AHGIncomeDataTable();
            AHGIncome.AHGIncomeRow row = dt.NewAHGIncomeRow();

            if (capture == "capture1")
            {
                row.AHGAvgAmount1 = cls.AHGAvgAmount1;
                row.AHGTotalAmount1 = cls.AHGTotalAmount1;
                row.NoOfMonths1 = cls.NoOfMonths1;
            }
            else
            {
                row.AHGAvgAmount2 = cls.AHGAvgAmount2;
                row.AHGTotalAmount2 = cls.AHGTotalAmount2;
                row.NoOfMonths2 = cls.NoOfMonths2;
            }                        
            row.DateEntered = DateTime.Now;
            row.UpdatedBy = cls.UpdateBy.Value;
            row.AppPersonalId = cls.AppPersonalId;
            dt.AddAHGIncomeRow(row);
            int rowsAffected = Adapter.Update(dt);
                       

            return rowsAffected;
        }


        public bool UpdateCapture1(AHGIncomeDb cls)
        {            

            AHGIncome.AHGIncomeDataTable dt = GetAHGIncomeByAppPersonalId(cls.AppPersonalId);

            if (dt.Count == 0)
                return false;

            AHGIncome.AHGIncomeRow row = dt[0];

            row.AHGAvgAmount1 = cls.AHGAvgAmount1;
            row.AHGTotalAmount1 = cls.AHGTotalAmount1;
            row.NoOfMonths1 = cls.NoOfMonths1;
            row.UpdatedBy = cls.UpdateBy.Value;
            row.DateEntered = DateTime.Now;
            
            int rowsAffected = Adapter.Update(dt);

            return rowsAffected == 1;
        }

        public bool UpdateCapture2(AHGIncomeDb cls)
        {

            AHGIncome.AHGIncomeDataTable dt = GetAHGIncomeByAppPersonalId(cls.AppPersonalId);

            if (dt.Count == 0)
                return false;

            AHGIncome.AHGIncomeRow row = dt[0];

            row.AHGAvgAmount2 = cls.AHGAvgAmount2;
            row.AHGTotalAmount2 = cls.AHGTotalAmount2;
            row.NoOfMonths2 = cls.NoOfMonths2;
            row.UpdatedBy = cls.UpdateBy.Value;
            row.DateEntered = DateTime.Now;

            int rowsAffected = Adapter.Update(dt);

            return rowsAffected == 1;
        }


        public static int UpdateClear1(AHGIncomeDb cls)
        {
            return AHGIncomeDs.UpdateClear1(cls);
        }

        public static int UpdateClear2(AHGIncomeDb cls)
        {
            return AHGIncomeDs.UpdateClear2(cls);
        }

        private int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private decimal _AHGTotalAmount1;

        public decimal AHGTotalAmount1
        {
            get { return _AHGTotalAmount1; }
            set { _AHGTotalAmount1 = value; }
        }

        private decimal _AHGAvgAmount1;

        public decimal AHGAvgAmount1
        {
            get { return _AHGAvgAmount1; }
            set { _AHGAvgAmount1 = value; }
        }

        private int _NoOfMonths1;

        public int NoOfMonths1
        {
            get { return _NoOfMonths1; }
            set { _NoOfMonths1 = value; }
        }

        private decimal _AHGTotalAmount2;

        public decimal AHGTotalAmount2
        {
            get { return _AHGTotalAmount2; }
            set { _AHGTotalAmount2 = value; }
        }

        private decimal _AHGAvgAmount2;

        public decimal AHGAvgAmount2
        {
            get { return _AHGAvgAmount2; }
            set { _AHGAvgAmount2 = value; }
        }

        private int _NoOfMonths2;

        public int NoOfMonths2
        {
            get { return _NoOfMonths2; }
            set { _NoOfMonths2 = value; }
        }

        private Guid? _UpdatedBy;

        public Guid? UpdateBy
        {
            get { return _UpdatedBy; }
            set { _UpdatedBy = value; }
        }

        private DateTime _DateEntered;

        public DateTime DateEntered
        {
            get { return _DateEntered; }
            set { _DateEntered = value; }
        }

        private int _AppPersonalId;

        public int AppPersonalId
        {
            get { return _AppPersonalId; }
            set { _AppPersonalId = value; }
        }
        
        
        
    }


    /// <summary>
    /// AHGIncomeDetails contains the AHGIncomeDetails table
    /// </summary>
    public class AHGIncomeDetailsDb
    {
        private int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private int _AHGIncomeId;

        public int AHGIncomeId
        {
            get { return _AHGIncomeId; }
            set { _AHGIncomeId = value; }
        }

        private int _IncomeId;

        public int IncomeId
        {
            get { return _IncomeId; }
            set { _IncomeId = value; }
        }

        private decimal _IncomeTotal;

        public decimal IncomeTotal
        {
            get { return _IncomeTotal; }
            set { _IncomeTotal = value; }
        }
        
        
        
        
    }
}
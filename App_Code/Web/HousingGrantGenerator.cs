using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Globalization;

using its = iTextSharp;
using itsImage = iTextSharp.text.Image;
using itsFont = iTextSharp.text.Font;
using itsRectangle = iTextSharp.text.Rectangle;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Dwms.Bll;

/// <summary>
/// Summary description for HousingGrantGenerator
/// </summary>
public static class HousingGrantGenerator
{
    private static string strRefNo = string.Empty;
    private static string strAssessmentDateOut = string.Empty;    

    public static MemoryStream GeneratePDFHousingGrant(int docAppId)
    {
        MemoryStream pdfStream = new MemoryStream();
        Document pdfDoc = new Document(PageSize.A4.Rotate());
        pdfDoc.SetMargins(pdfDoc.LeftMargin, pdfDoc.RightMargin, 80, 80);
        

        Dictionary<string, IncomeWorksheet> dicIncomeWorksheet = null;
        IncomeWorksheet clsIncomeWorksheet;
        List<string> listMonthYear = new List<string>();
        List<int> listIncomeId = new List<int>();
        decimal decIncomeAmount = 0;
        string strLowestIncomeAmount = string.Empty;
        int intYes = 0;
        int intNo = 0;
        

        try
        {
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);
            PdfPTable pdfpTable = null;            
            itsFont bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            

            #region Get RefNo From DocApp to get the number of columns to create for the PDF table.    
        

            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppIncomeExtractionById(docAppId);
            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                strRefNo = docAppRow.RefNo;
                strAssessmentDateOut = !docAppRow.IsAssessmentDateOutNull() ? docAppRow.AssessmentDateOut.ToShortDateString() : " - ";
                pdfWriter.PageEvent = new PDFFooter();
                pdfDoc.Open();

                #region Income Extraction Worksheet Label
                Paragraph TitleParagraph = new Paragraph(new Chunk("Income Extraction Worksheet", bold));
                TitleParagraph.Font.Size = 16;
                pdfDoc.Add(TitleParagraph);
                #endregion

                string refType = docAppRow.RefType.ToUpper().Trim();
                #region HLE
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                    HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                    ApplicationNumberLabel(docAppRow.RefNo, TitleParagraph, pdfDoc, bold);
                    
                    if (hleInterfaceDt.Rows.Count > 0)
                    {
                        #region CreditAssessment Table
                        
                        //this is the new enhancement from the excelsheet
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            int intMonthsToLeas = 0;
                            decimal decCA = 0;
                            decimal decCATotal = 0;
                            AppPersonalDb appPersonalDb = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(hleInterfaceRow.Nric, docAppId);
                            AppPersonal.AppPersonalRow appPersonalRow;
                            if (appPersonalDt.Rows.Count > 0)
                            {
                                appPersonalRow = appPersonalDt[0];

                                intMonthsToLeas = !appPersonalRow.IsMonthsToLEASNull() ? appPersonalRow.MonthsToLEAS : 0;

                                DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, hleInterfaceRow.Nric, "DESC");
                                pdfpTable = new PdfPTable(7 + IncomeDt.Rows.Count);

                                CreditAssessmentApplicantAndHeader(hleInterfaceRow.ApplicantType, hleInterfaceRow.OrderNo, hleInterfaceRow.Name, hleInterfaceRow.Nric, docAppId,
                                    bold, pdfDoc, pdfpTable);   

                                CreditAssessmentPopulateMonthYear(listIncomeId, IncomeDt, pdfpTable);

                                CreditAssessmentSetHeaders(docAppId, hleInterfaceRow.Nric, intMonthsToLeas, pdfpTable);

                                DataTable dtOrdered = new DataTable();
                                CreditAssessmentSortIncomeType(appPersonalRow.Id, out dtOrdered);                                
                                foreach (DataRow IncomeItemsRow in dtOrdered.Rows)
                                {
                                    CreditAssessmentGetValues(IncomeItemsRow, pdfpTable, listIncomeId, intMonthsToLeas, appPersonalRow.Id, decIncomeAmount, strLowestIncomeAmount, intYes, intNo, out decCA);
                                    decCATotal = decCATotal + decCA;
                                }
                            }
                            CreditAssessmentAddCATotalRow(decCATotal, pdfpTable, listIncomeId.Count);
                            CreditAssessmentAddAverageHeaders(intMonthsToLeas, pdfpTable, pdfDoc);                            
                        }

                        #endregion

                        #region This is the Summary Tables for each person
                        if (dicIncomeWorksheet == null)
                            dicIncomeWorksheet = new Dictionary<string, IncomeWorksheet>();
                        foreach (HleInterface.HleInterfaceRow interfaceRow in hleInterfaceDt.Rows)
                        {
                            pdfpTable = new PdfPTable(4);
                            SummaryForEachPersonalHeader(pdfpTable, interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, ReferenceTypeEnum.HLE.ToString());

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal TotalGrossIncome = 0;
                                decimal TotalAllowance = 0;
                                decimal TotalOT = 0;
                                int i = 0;
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = 0;
                                    decimal b = 0;
                                    decimal c = 0;
                                    SummaryForEachPersonalComputation(ReferenceTypeEnum.HLE.ToString(), IncomeRow, listMonthYear, pdfpTable, out a, out b, out c);                                    
                                    #region Adding GrossIncome, Allowance and OT for each Person and storing it in a class
                                    TotalGrossIncome = TotalGrossIncome + a;
                                    TotalAllowance = TotalAllowance + b;
                                    TotalOT = TotalOT + c;
                                    clsIncomeWorksheet = new IncomeWorksheet();
                                    SummaryForEachPersonalComputationInClass(IncomeRow, clsIncomeWorksheet, dicIncomeWorksheet, interfaceRow.Nric, a, b, c, ReferenceTypeEnum.HLE.ToString());
                                    #endregion
                                    i++;
                                }
                                SummaryForEachPersonalAverageAndTotal(TotalGrossIncome, TotalAllowance, TotalOT, 0, IncomeDt.Rows.Count, ReferenceTypeEnum.HLE.ToString(), pdfpTable);
                            }
                            pdfDoc.Add(pdfpTable);
                        }
                        #endregion

                        decimal TotalAverageGrossIncome = 0;
                        listMonthYear = SortMonthYear(listMonthYear);

                        #region Application Summary Table

                        pdfpTable = new PdfPTable(hleInterfaceDt.Rows.Count + 1); //Sets the number of Columns for the Application Summary Table                              
                        Phrase p1 = new Phrase();   
                        PdfPCell cell1 = new PdfPCell(p1);
                        ApplicationSummaryTableSettings(pdfpTable, p1, cell1, hleInterfaceDt.Rows.Count, bold, docAppRow.RefNo);

                        #region Creation of the PersonName and NRIC Header
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(string.Format("{0} / {1}",hleInterfaceRow.Name,hleInterfaceRow.Nric), true, false));
                        }
                        #endregion

                        #region Getting the Gross Amount per Month Year for each Person
                        foreach (string str in listMonthYear)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(str, false, false));
                            foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                            {

                                if (dicIncomeWorksheet.ContainsKey(hleInterfaceRow.Nric + str))
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding
                                        (dicIncomeWorksheet[hleInterfaceRow.Nric + str].AverageGrossIncome).ToString(), false, false));
                                }
                                else
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                                }
                            }
                        }
                        #endregion

                        #region Getting the Average Gross Income for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average Gross Income", false, false));
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, hleInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgGrossIncome = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ?
                                        IncomeRow["GrossIncome"].ToString() : "0");
                                    avgGrossIncome = avgGrossIncome + a;

                                }
                                avgGrossIncome = avgGrossIncome / IncomeDt.Rows.Count;
                                TotalAverageGrossIncome = TotalAverageGrossIncome + avgGrossIncome;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgGrossIncome).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        #region Getting the Average Allowance for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average Allowance", false, false));
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, hleInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgAllowance = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal b = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["Allowance"].ToString()) ?
                                        IncomeRow["Allowance"].ToString() : "0");
                                    avgAllowance = avgAllowance + b;
                                }
                                avgAllowance = avgAllowance / IncomeDt.Rows.Count;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgAllowance).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        #region Getting the Average OT for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average Overtime", false, false));
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, hleInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgOT = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal c = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["Overtime"].ToString()) ?
                                        IncomeRow["Overtime"].ToString() : "0");
                                    avgOT = avgOT + c;

                                }
                                avgOT = avgOT / IncomeDt.Rows.Count;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgOT).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        pdfDoc.Add(pdfpTable);

                        ApplicationSummaryTotalGrossIncome(TotalAverageGrossIncome, pdfDoc, bold);

                        #endregion
                        
                        #region Monthly Zone Table per Person

                        MonthlyZoneTitle(pdfDoc, bold);

                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            MonthlyZonePersonalName(hleInterfaceRow.ApplicantType, hleInterfaceRow.OrderNo, hleInterfaceRow.Name, hleInterfaceRow.Nric, bold, pdfDoc);
                            DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, hleInterfaceRow.Nric, "DESC");
                            listIncomeId.Clear();
                            MonthlyZoneGetValues(docAppId, IncomeDt, pdfpTable, pdfDoc, listIncomeId, refType);                                               
                        }
                        #endregion

                    }
                }
                #endregion
                #region Resale Added By Edward Changes for Sales and Resale 2014/5/26
                else if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))
                {
                    ResaleInterfaceDb interfaceDb = new ResaleInterfaceDb();
                    ResaleInterface.ResaleInterfaceDataTable interfaceDt = interfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);

                    ApplicationNumberLabel(docAppRow.RefNo, TitleParagraph, pdfDoc, bold);

                    if (interfaceDt.Rows.Count > 0)
                    {
                        #region This is the Summary Tables for each person
                        if (dicIncomeWorksheet == null)
                            dicIncomeWorksheet = new Dictionary<string, IncomeWorksheet>();
                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            pdfpTable = new PdfPTable(3);
                            SummaryForEachPersonalHeader(pdfpTable, interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, ReferenceTypeEnum.RESALE.ToString());
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal TotalGrossIncome = 0;
                                decimal TotalAHGIncome = 0;                                
                                int i = 0;
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = 0;
                                    decimal b = 0;
                                    decimal c = 0;
                                    SummaryForEachPersonalComputation(ReferenceTypeEnum.RESALE.ToString(), IncomeRow, listMonthYear, pdfpTable, out a, out b, out c);        
                                    #region Adding GrossIncome, Allowance and OT for each Person and storing it in a class
                                    TotalGrossIncome = TotalGrossIncome + a;
                                    TotalAHGIncome = TotalAHGIncome + b;                                    
                                    clsIncomeWorksheet = new IncomeWorksheet();
                                    SummaryForEachPersonalComputationInClass(IncomeRow, clsIncomeWorksheet, dicIncomeWorksheet, interfaceRow.Nric, a, b, c, ReferenceTypeEnum.RESALE.ToString());
                                    #endregion
                                    i++;
                                }
                                SummaryForEachPersonalAverageAndTotal(TotalGrossIncome, 0, 0, TotalAHGIncome, IncomeDt.Rows.Count, ReferenceTypeEnum.RESALE.ToString(), pdfpTable);                                
                            }
                            pdfDoc.Add(pdfpTable);
                        }
                        #endregion

                        decimal TotalAverageGrossIncome = 0;
                        listMonthYear = SortMonthYear(listMonthYear);

                        #region Application Summary Table

                        pdfpTable = new PdfPTable(interfaceDt.Rows.Count + 1); //Sets the number of Columns for the Application Summary Table                        
                        Phrase p1 = new Phrase();
                        PdfPCell cell1 = new PdfPCell(p1);
                        ApplicationSummaryTableSettings(pdfpTable, p1, cell1, interfaceDt.Rows.Count, bold, docAppRow.RefNo);

                        #region Creation of the PersonName and NRIC Header
                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(string.Format("{0} / {1}", interfaceRow.Name, interfaceRow.Nric), true, false));
                        }
                        #endregion

                        #region Getting the Gross Amount per Month Year for each Person
                        foreach (string str in listMonthYear)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(str, false, false));
                            foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                            {

                                if (dicIncomeWorksheet.ContainsKey(interfaceRow.Nric + str))
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding
                                        (dicIncomeWorksheet[interfaceRow.Nric + str].AverageGrossIncome).ToString(), false, false));
                                }
                                else
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                                }
                            }
                        }
                        #endregion

                        #region Getting the Average Gross Income for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average Gross Income", false, false));
                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgGrossIncome = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ?
                                        IncomeRow["GrossIncome"].ToString() : "0");

                                    avgGrossIncome = avgGrossIncome + a;

                                }
                                avgGrossIncome = avgGrossIncome / IncomeDt.Rows.Count;
                                TotalAverageGrossIncome = TotalAverageGrossIncome + avgGrossIncome;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgGrossIncome).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        #region Getting the Average Allowance for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average AHGIncome", false, false));
                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgAllowance = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {

                                    decimal b = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ?
                                        IncomeRow["AHGIncome"].ToString() : "0");
                                    avgAllowance = avgAllowance + b;
                                }
                                avgAllowance = avgAllowance / IncomeDt.Rows.Count;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgAllowance).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        pdfDoc.Add(pdfpTable);

                        ApplicationSummaryTotalGrossIncome(TotalAverageGrossIncome, pdfDoc, bold);

                        #endregion

                        #region CreditAssessment Table

                        //this is the new enhancement from the excelsheet
                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {    

                            int intMonthsToLeas = 0;
                            AppPersonalDb appPersonalDb = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(interfaceRow.Nric, docAppId);
                            AppPersonal.AppPersonalRow appPersonalRow;
                            if (appPersonalDt.Rows.Count > 0)
                            {                                
                                appPersonalRow = appPersonalDt[0];
                                intMonthsToLeas = !appPersonalRow.IsMonthsToLEASNull() ? appPersonalRow.MonthsToLEAS : 0;
                                DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, interfaceRow.Nric, "DESC");
                                pdfpTable = new PdfPTable(7 + IncomeDt.Rows.Count);
                                CreditAssessmentApplicantAndHeader(interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, docAppId,
                                    bold, pdfDoc, pdfpTable);

                                CreditAssessmentPopulateMonthYear(listIncomeId, IncomeDt, pdfpTable);

                                CreditAssessmentSetHeaders(docAppId, interfaceRow.Nric, intMonthsToLeas, pdfpTable);

                                DataTable dtOrdered = new DataTable();
                                CreditAssessmentSortIncomeType(appPersonalRow.Id, out dtOrdered);
                                decimal decCATotal = 0;
                                foreach (DataRow IncomeItemsRow in dtOrdered.Rows)
                                {
                                    CreditAssessmentGetValues(IncomeItemsRow, pdfpTable, listIncomeId, intMonthsToLeas, appPersonalRow.Id, decIncomeAmount, strLowestIncomeAmount, intYes, intNo, out decCATotal);
                                }
                            }

                            CreditAssessmentAddAverageHeaders(intMonthsToLeas, pdfpTable, pdfDoc);
                        }

                        #endregion

                        #region Monthly Zone Table per Person

                        MonthlyZoneTitle(pdfDoc, bold);

                        foreach (ResaleInterface.ResaleInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            MonthlyZonePersonalName(interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, bold, pdfDoc);
                            DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, interfaceRow.Nric, "DESC");
                            listIncomeId.Clear();
                            MonthlyZoneGetValues(docAppId, IncomeDt, pdfpTable, pdfDoc, listIncomeId, refType);                                
                        }
                        #endregion

                    }
                }
                else if (refType.Contains(ReferenceTypeEnum.SALES.ToString()))
                {
                    SalesInterfaceDb interfaceDb = new SalesInterfaceDb();
                    SalesInterface.SalesInterfaceDataTable interfaceDt = interfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);

                    ApplicationNumberLabel(docAppRow.RefNo, TitleParagraph, pdfDoc, bold);

                    if (interfaceDt.Rows.Count > 0)
                    {
                        #region This is the Summary Tables for each person
                        if (dicIncomeWorksheet == null)
                            dicIncomeWorksheet = new Dictionary<string, IncomeWorksheet>();
                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            pdfpTable = new PdfPTable(3);
                            SummaryForEachPersonalHeader(pdfpTable, interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, ReferenceTypeEnum.SALES.ToString());
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal TotalGrossIncome = 0;
                                decimal TotalAHGIncome = 0;
                                int i = 0;
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = 0;
                                    decimal b = 0;
                                    decimal c = 0;
                                    SummaryForEachPersonalComputation(ReferenceTypeEnum.SALES.ToString(), IncomeRow, listMonthYear, pdfpTable, out a, out b, out c);
                                    #region Adding GrossIncome, Allowance and OT for each Person and storing it in a class
                                    TotalGrossIncome = TotalGrossIncome + a;
                                    TotalAHGIncome = TotalAHGIncome + b;
                                    clsIncomeWorksheet = new IncomeWorksheet();
                                    SummaryForEachPersonalComputationInClass(IncomeRow, clsIncomeWorksheet, dicIncomeWorksheet, interfaceRow.Nric, a, b, c, ReferenceTypeEnum.RESALE.ToString());
                                    #endregion                                    
                                    i++;
                                }
                                SummaryForEachPersonalAverageAndTotal(TotalGrossIncome, 0, 0, TotalAHGIncome, IncomeDt.Rows.Count, ReferenceTypeEnum.SALES.ToString(), pdfpTable);                                
                            }
                            pdfDoc.Add(pdfpTable);
                        }
                        #endregion

                        decimal TotalAverageGrossIncome = 0;
                        listMonthYear = SortMonthYear(listMonthYear);

                        #region Application Summary Table
                        
                        pdfpTable = new PdfPTable(interfaceDt.Rows.Count + 1); //Sets the number of Columns for the Application Summary Table                                                                        
                        Phrase p1 = new Phrase();                        
                        PdfPCell cell1 = new PdfPCell(p1);
                        ApplicationSummaryTableSettings(pdfpTable, p1, cell1, interfaceDt.Rows.Count, bold, docAppRow.RefNo);

                        #region Creation of the PersonName and NRIC Header
                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(string.Format("{0} / {1}", interfaceRow.Name, interfaceRow.Nric), true, false));
                        }
                        #endregion

                        #region Getting the Gross Amount per Month Year for each Person
                        foreach (string str in listMonthYear)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(str, false, false));
                            foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                            {

                                if (dicIncomeWorksheet.ContainsKey(interfaceRow.Nric + str))
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding
                                        (dicIncomeWorksheet[interfaceRow.Nric + str].AverageGrossIncome).ToString(), false, false));
                                }
                                else
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                                }
                            }
                        }
                        #endregion

                        #region Getting the Average Gross Income for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average Gross Income", false, false));
                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgGrossIncome = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    decimal a = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ?
                                        IncomeRow["GrossIncome"].ToString() : "0");

                                    avgGrossIncome = avgGrossIncome + a;

                                }
                                avgGrossIncome = avgGrossIncome / IncomeDt.Rows.Count;
                                TotalAverageGrossIncome = TotalAverageGrossIncome + avgGrossIncome;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgGrossIncome).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        #region Getting the Average Allowance for each Person
                        pdfpTable.AddCell(PopulatePDFCell("Average AHGIncome", false, false));
                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, interfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                decimal avgAllowance = 0;

                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {

                                    decimal b = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ?
                                        IncomeRow["AHGIncome"].ToString() : "0");
                                    avgAllowance = avgAllowance + b;
                                }
                                avgAllowance = avgAllowance / IncomeDt.Rows.Count;
                                pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(avgAllowance).ToString(), false, false));
                            }
                            else
                                pdfpTable.AddCell(PopulatePDFCell(" - ", false, false));
                        }
                        #endregion

                        pdfDoc.Add(pdfpTable);

                        ApplicationSummaryTotalGrossIncome(TotalAverageGrossIncome, pdfDoc, bold);

                        #endregion

                        #region CreditAssessment Table

                        //this is the new enhancement from the excelsheet
                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            int intMonthsToLeas = 0;
                            AppPersonalDb appPersonalDb = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(interfaceRow.Nric, docAppId);
                            AppPersonal.AppPersonalRow appPersonalRow;
                            if (appPersonalDt.Rows.Count > 0)
                            {
                                appPersonalRow = appPersonalDt[0];
                                intMonthsToLeas = !appPersonalRow.IsMonthsToLEASNull() ? appPersonalRow.MonthsToLEAS : 0;
                                DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, interfaceRow.Nric, "DESC");
                                pdfpTable = new PdfPTable(7 + IncomeDt.Rows.Count);
                                CreditAssessmentApplicantAndHeader(interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, docAppId,
                                    bold, pdfDoc, pdfpTable);

                                CreditAssessmentPopulateMonthYear(listIncomeId, IncomeDt, pdfpTable);

                                CreditAssessmentSetHeaders(docAppId, interfaceRow.Nric, intMonthsToLeas, pdfpTable);

                                DataTable dtOrdered = new DataTable();
                                CreditAssessmentSortIncomeType(appPersonalRow.Id, out dtOrdered);
                                decimal decCATotal = 0;
                                foreach (DataRow IncomeItemsRow in dtOrdered.Rows)
                                {
                                    CreditAssessmentGetValues(IncomeItemsRow, pdfpTable, listIncomeId, intMonthsToLeas, appPersonalRow.Id, decIncomeAmount, strLowestIncomeAmount, intYes, intNo, out decCATotal);
                                }
                            }

                            CreditAssessmentAddAverageHeaders(intMonthsToLeas, pdfpTable, pdfDoc);
                        }

                        #endregion

                        #region Monthly Zone Table per Person

                        MonthlyZoneTitle(pdfDoc, bold);

                        foreach (SalesInterface.SalesInterfaceRow interfaceRow in interfaceDt.Rows)
                        {
                            MonthlyZonePersonalName(interfaceRow.ApplicantType, interfaceRow.OrderNo, interfaceRow.Name, interfaceRow.Nric, bold, pdfDoc);
                            DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId, interfaceRow.Nric, "DESC");
                            listIncomeId.Clear();
                            MonthlyZoneGetValues(docAppId, IncomeDt, pdfpTable, pdfDoc, listIncomeId, refType);     
                        }
                        #endregion

                    }
                }
                #endregion
            }            
           
            #endregion                       
            pdfDoc.Close();
        }
        catch (Exception ex)
        {

        }

        return pdfStream;
    }

    private static PdfPCell PopulatePDFCell(string value, bool IsBold, bool IsGray)
    {
        itsFont bold = !IsBold ?  FontFactory.GetFont(FontFactory.HELVETICA, 12) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        PdfPCell pdfpCell = new PdfPCell(new Phrase(new Chunk(value,bold)));
        pdfpCell.BackgroundColor = !IsGray ? its.text.Color.WHITE : its.text.Color.LIGHT_GRAY;
        return pdfpCell;
    }

    private static PdfPCell PopulatePDFCellFont10(string value, bool IsBold, bool IsGray)
    {
        itsFont bold = !IsBold ? FontFactory.GetFont(FontFactory.HELVETICA, 9) : FontFactory.GetFont(FontFactory.HELVETICA, 9);
        PdfPCell pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
        pdfpCell.BackgroundColor = !IsGray ? its.text.Color.WHITE : its.text.Color.LIGHT_GRAY;
        pdfpCell.HorizontalAlignment = Cell.ALIGN_CENTER;
        return pdfpCell;
    }

    private static PdfPCell PopulatePDFCA(string value, bool IsBold, bool IsGray, string IsGenerated)
    {
        itsFont bold;
        PdfPCell pdfpCell;
        if (!IsGray)
        {
            if (IsGenerated == "Y")
            {
                bold = FontFactory.GetFont(FontFactory.HELVETICA, 9, its.text.Color.WHITE);
                pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
                pdfpCell.BackgroundColor = its.text.Color.RED;
            }
            else if (IsGenerated == "N")
            {
                bold = FontFactory.GetFont(FontFactory.HELVETICA, 9, its.text.Color.BLACK);
                pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
                pdfpCell.BackgroundColor = its.text.Color.CYAN;
            }
            else
            {
                bold = FontFactory.GetFont(FontFactory.HELVETICA, 9, its.text.Color.BLACK);
                pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
                pdfpCell.BackgroundColor = its.text.Color.WHITE;
            }
        }
        else
        {
            bold = !IsBold ? FontFactory.GetFont(FontFactory.HELVETICA, 9) : FontFactory.GetFont(FontFactory.HELVETICA, 9);
            pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
            pdfpCell.BackgroundColor = its.text.Color.LIGHT_GRAY;
        }
        pdfpCell.HorizontalAlignment = Cell.ALIGN_CENTER;
        return pdfpCell;
    }

    private static List<string> SortMonthYear(List<string> li)
    {
        string[] mth = {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
        List<string> newList = new List<string>();
        List<int> YearList = new List<int>();
        List<string> tempList;
        foreach (string listr in li)
        {
            int a = int.Parse(listr.Substring(listr.Length - 4));
            if (YearList.Count == 0)
                YearList.Add(a);
            else
            {
                bool unique = true;
                foreach (int year in YearList)
                {
                   
                    if (a == year)
                    {
                        unique = false;
                        break;
                    }
                }
                if(unique)
                    YearList.Add(a);
            }
        }
        YearList.Sort();

        foreach (int i in YearList)
        {
            tempList = new List<string>();
            foreach (string str1 in li)
            {
                if (i == int.Parse(str1.Substring(str1.Length - 4)))                
                    tempList.Add(str1);                                                        
            }
            foreach (string str in mth)
            {
                foreach (string temp in tempList)
                {
                    if (str == temp.Substring(0, temp.Length - 5))
                        newList.Add(temp);
                }
            }
        }                    
        return newList;
    }

    //http://www.codeproject.com/Tips/573907/Generating-PDF-using-ItextSharp-with-Footer-in-Csh
    public class PDFFooter : PdfPageEventHelper
    {
        // write on top of document
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            //base.OnOpenDocument(writer, document);
            //PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            //tabFot.SpacingAfter = 10F;
            //PdfPCell cell;
            //tabFot.TotalWidth = 300F;
            //cell = new PdfPCell(new Phrase("Header"));
            //tabFot.AddCell(cell);
            //tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);
        }

        // write on start of each page
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        // write on end of each page
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });
            PdfPCell cell;
            tabFot.TotalWidth = document.PageSize.Width - document.RightMargin;            
            cell = new PdfPCell(new Phrase(string.Format("{0} / {1}", strRefNo, strAssessmentDateOut)));
            cell.Border = 0;
            cell.HorizontalAlignment = PdfCell.ALIGN_RIGHT;
            tabFot.AddCell(cell);
            tabFot.WriteSelectedRows(0, -50, 0,document.Bottom - 50, writer.DirectContent);
        }

        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }

    private static void ApplicationNumberLabel(string RefNo, Paragraph TitleParagraph, Document pdfDoc, itsFont bold)
    {
        TitleParagraph = new Paragraph(new Chunk("Application Number: " + RefNo, bold));
        TitleParagraph.Font.Size = 14;
        TitleParagraph.SpacingAfter = 10f;
        pdfDoc.Add(TitleParagraph);
    }

    private static void SummaryForEachPersonalHeader(PdfPTable pdfpTable, string applicantType, int orderNo, string name, string nric, string refType)
    {
        
        pdfpTable.SpacingBefore = 30f;
        pdfpTable.WidthPercentage = 100;

        Phrase p = new Phrase();
        string strSummary = string.Empty;
        if (applicantType.Equals(PersonalTypeEnum.HA.ToString()))
            strSummary = string.Format("Summary for Applicant {0} - {1} ({2})", orderNo, name, nric);
        else if (applicantType.Equals(PersonalTypeEnum.OC.ToString()))
            strSummary = string.Format("Summary for Occupier {0} - {1} ({2})", orderNo, name, nric);
        else
            strSummary = string.Format("Summary for {0} ({1})", name, nric);
        p.Add(new Chunk(strSummary));
        PdfPCell cell = new PdfPCell(p);
        cell.Colspan = 4;
        cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        cell.BackgroundColor = its.text.Color.CYAN;
        pdfpTable.AddCell(cell);

        pdfpTable.AddCell(PopulatePDFCell("Month", true, true));
        pdfpTable.AddCell(PopulatePDFCell("Gross Income", true, true));

        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
        {
            pdfpTable.AddCell(PopulatePDFCell("Allce", true, true));
            pdfpTable.AddCell(PopulatePDFCell("OT", true, true));
        }
        else        
            pdfpTable.AddCell(PopulatePDFCell("AHGIncome", true, true));                        
    }

    private static void SummaryForEachPersonalComputation(string refType, DataRow IncomeRow, List<string> listMonthYear, PdfPTable pdfpTable, out decimal a, out decimal b, out decimal c)
    {
        a = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ?
                IncomeRow["GrossIncome"].ToString() : "0");
        #region Getting GrossIncome, Allowance and Overtime, AHG Income values divided by the CurrencyRate
        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
        {
            b = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["Allowance"].ToString()) ?
                IncomeRow["Allowance"].ToString() : "0");
            c = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["Overtime"].ToString()) ?
                IncomeRow["Overtime"].ToString() : "0");
        }
        else
        {
            b = decimal.Parse(!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ?
                IncomeRow["AHGIncome"].ToString() : "0");
            c = 0;
        }
        #endregion

        #region Gets the Income Month and Year and stores it in a list which will be used in the Summary of the Application table
        pdfpTable.AddCell(PopulatePDFCell(IncomeRow["MonthYear"].ToString(), false, false));
        bool IsExists = false;
        foreach (string str in listMonthYear)
        {
            if (str == IncomeRow["MonthYear"].ToString())
            {
                IsExists = true;
                break;
            }
        }
        if (IsExists == false)
            listMonthYear.Add(IncomeRow["MonthYear"].ToString());
        #endregion

        #region Creates the cells for GrossIncome, Overtie and Allowance values
        pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(a).ToString(), false, false)); //GrossIncome
        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
        {                                                
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(b).ToString(), false, false)); //Allowance
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(c).ToString(), false, false)); //Overtime
        }
        else                 
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(b).ToString(), false, false)); //AHGIncome                                            
        #endregion
    }

    private static void SummaryForEachPersonalComputationInClass(DataRow IncomeRow, IncomeWorksheet clsIncomeWorksheet, Dictionary<string, IncomeWorksheet> dicIncomeWorksheet,
        string nric, decimal a, decimal b, decimal c, string refType)
    {
        #region Adding GrossIncome, Allowance and OT for each Person and storing it in a class
        clsIncomeWorksheet.AverageGrossIncome = a;
        clsIncomeWorksheet.AverageAllowance = b;        //also ahgincome for non hle
        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))        
            clsIncomeWorksheet.AverageOT = c;            
                
        clsIncomeWorksheet.MonthYear = IncomeRow["MonthYear"].ToString();
        clsIncomeWorksheet.MonthYearId = int.Parse(IncomeRow["Id"].ToString());
        if (!dicIncomeWorksheet.ContainsKey(nric + IncomeRow["MonthYear"].ToString()))
            dicIncomeWorksheet.Add(nric + IncomeRow["MonthYear"].ToString(), clsIncomeWorksheet);
        #endregion
    }

    private static void SummaryForEachPersonalAverageAndTotal(decimal TotalGrossIncome, decimal TotalAllowance, decimal TotalOT, decimal TotalAHGIncome,
        int noOfRows, string refType, PdfPTable pdfpTable)
    {
        #region Average for each Person
        pdfpTable.AddCell(PopulatePDFCell("Average", true, false));
        pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalGrossIncome / noOfRows).ToString(), false, false));
        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
        {
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalAllowance / noOfRows).ToString(), false, false));
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalOT / noOfRows).ToString(), false, false));
        }
        else
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalAHGIncome / noOfRows).ToString(), false, false));                
        #endregion

        #region Total for each Person
        pdfpTable.AddCell(PopulatePDFCell("Total", true, false));
        pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalGrossIncome).ToString(), false, false));
        if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
        {
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalAllowance).ToString(), false, false));
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalOT).ToString(), false, false));
        }
        else
            pdfpTable.AddCell(PopulatePDFCell(Format.GetDecimalPlacesWithoutRounding(TotalAHGIncome).ToString(), false, false));
        #endregion
    }

    private static void ApplicationSummaryTableSettings(PdfPTable pdfpTable, Phrase p1, PdfPCell cell1, int noOfColumns, itsFont bold, string refNo)
    {
        #region Settings for the Application Summary Table        
        pdfpTable.SpacingBefore = 30f;
        pdfpTable.SpacingAfter = 30f;
        pdfpTable.WidthPercentage = 100;
        #endregion

        #region Creation of the Application Summary Header and the Item Header        
        p1.Add(new Chunk(string.Format("Summary for {0}", refNo, bold)));        
        cell1.Colspan = noOfColumns + 1;
        cell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        cell1.BackgroundColor = its.text.Color.CYAN;
        pdfpTable.AddCell(cell1);
        p1 = new Phrase();
        p1.Add(new Chunk("Items", bold));
        cell1 = new PdfPCell(p1);
        cell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
        pdfpTable.AddCell(cell1);
        #endregion
    }

    private static void ApplicationSummaryTotalGrossIncome(decimal TotalAverageGrossIncome, Document pdfDoc, itsFont bold)
    {
        Paragraph TitleParagraph = new Paragraph(new Chunk("Total Average Gross Income: " + Format.GetDecimalPlacesWithoutRounding(TotalAverageGrossIncome).ToString(), bold));
        TitleParagraph.Font.Size = 13;
        TitleParagraph.SpacingAfter = 20f;
        pdfDoc.Add(TitleParagraph);
    }

    private static void CreditAssessmentApplicantAndHeader(string applicantType, int orderNo, string name, string nric, int docAppId, itsFont bold, Document pdfDoc, PdfPTable pdfpTable)
    {
        #region Applicant Name and NRIC Label for each Person
        string strSummary = string.Empty;
        if (applicantType.Equals(PersonalTypeEnum.HA.ToString()))
            strSummary = string.Format("Applicant {0} Name: {1} ({2})", orderNo, name, nric);
        else if (applicantType.Equals(PersonalTypeEnum.OC.ToString()))
            strSummary = string.Format("Occupier {0} Name: {1} ({2})", orderNo, name, nric);
        else
            strSummary = string.Format("{0} Name: {1} ({2})", orderNo, name, nric);

        Paragraph TitleParagraph = new Paragraph(new Chunk(strSummary, bold));
        TitleParagraph.Font.Size = 14;
        pdfDoc.Add(TitleParagraph);
        #endregion
        pdfpTable.SpacingBefore = 10f;
        pdfpTable.WidthPercentage = 100;
        #region Income Component and Type of Income Column Headers
        pdfpTable.AddCell(PopulatePDFCellFont10("Income Component", true, true));
        pdfpTable.AddCell(PopulatePDFCellFont10("Type of Income", true, true));
        #endregion
    }

    private static void CreditAssessmentPopulateMonthYear(List<int> listIncomeId, DataTable IncomeDt, PdfPTable pdfpTable)
    {
        listIncomeId.Clear();
        if (IncomeDt.Rows.Count > 0)
        {
            foreach (DataRow IncomeRow in IncomeDt.Rows)
            {
                DateTimeFormatInfo info = new DateTimeFormatInfo();
                pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("{0} {1}",
                    info.GetAbbreviatedMonthName(int.Parse(IncomeRow["IncomeMonth"].ToString())), IncomeRow["IncomeYear"].ToString()), true, true));
                listIncomeId.Add(int.Parse(IncomeRow["Id"].ToString()));
            }
        }
    }

    private static void CreditAssessmentSetHeaders(int docAppId, string nric, int intMonthsToLeas, PdfPTable pdfpTable)
    {
        #region Set the Avg in Past 3 months, Avg in Past 12 Months, Lowest, Yes/No and Credit Assessment Column Headers
        DataTable NumberOfMonthsDt = IncomeDb.GetOrderByMonthYear(docAppId, nric, "DESC");
        pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("Avg in past {0} mths", NumberOfMonthsDt.Rows.Count < 3 ? NumberOfMonthsDt.Rows.Count.ToString() : "3"), true, true));        
        pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("Avg in past {0} mths", intMonthsToLeas), true, true));        
        pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("Lowest in past {0} mths", intMonthsToLeas), true, true));
        pdfpTable.AddCell(PopulatePDFCellFont10("Yes/No", true, true));
        pdfpTable.AddCell(PopulatePDFCellFont10("Credit Assessment", true, true));
        #endregion
    }

    private static void CreditAssessmentSortIncomeType(int appPersonalId, out DataTable dtOrdered)
    {
        DataTable IncomeItemsDt = IncomeDb.GetDistinctIncomeItemByAppPersonalId(appPersonalId);        
        DataView dv = IncomeItemsDt.DefaultView;
        dv.RowFilter = "IncomeType = 'Gross Income'";
        dtOrdered = dv.ToTable();
        dv.RowFilter = "IncomeType = 'Allowance'";
        dtOrdered.Merge(dv.ToTable());
        dv.RowFilter = "IncomeType = 'Overtime'";
        dtOrdered.Merge(dv.ToTable());
        dv.RowFilter = "IncomeType = 'AHG Income'";
        dtOrdered.Merge(dv.ToTable());
    }

    private static void CreditAssessmentGetValues(DataRow IncomeItemsRow, PdfPTable pdfpTable, List<int> listIncomeId, int intMonthsToLeas, int appPersonalId,
        decimal decIncomeAmount, string strLowestIncomeAmount, int intYes, int intNo, out decimal decCATotal)
    {
        decCATotal = 0;
        decimal past3 = 0;
        decimal past12 = 0;
        int i = 0;
        if (!string.IsNullOrEmpty(IncomeItemsRow["IncomeType"].ToString()))
        {
            #region Get the IncomeItem and IncomeType for each Person
            pdfpTable.AddCell(PopulatePDFCellFont10(IncomeItemsRow["IncomeItem"].ToString(), false, false));
            pdfpTable.AddCell(PopulatePDFCellFont10(IncomeItemsRow["IncomeType"].ToString(), false, false));
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
                        else if (decimal.Parse(strLowestIncomeAmount) > (decimal.Parse(IncomeItemRow["IncomeAmount"].ToString()) / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())))
                            strLowestIncomeAmount = (decimal.Parse(IncomeItemRow["IncomeAmount"].ToString()) / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString();
                    }
                    intYes = intYes + 1;
                    pdfpTable.AddCell(PopulatePDFCellFont10(Format.GetDecimalPlacesWithoutRounding(decTotalAmountOfIncomeItem / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString(), false, false));
                }
                else
                {
                    i++;
                    if (i == 3)
                        past3 = decIncomeAmount;                    
                    if (i == intMonthsToLeas)   //Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                        past12 = decIncomeAmount;
                    intNo = intNo + 1;
                    pdfpTable.AddCell(PopulatePDFCellFont10(" - ", false, false));
                }
            }
            #endregion

            #region  Add Average cells, Lowest, Yes/No Values
            pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("{0}", i < 3 ? Format.GetDecimalPlacesWithoutRounding(decIncomeAmount / i).ToString()
                : Format.GetDecimalPlacesWithoutRounding(past3 / 3).ToString()), false, false));
            pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("{0}", intMonthsToLeas > 0 ? Format.GetDecimalPlacesWithoutRounding(past12 / intMonthsToLeas).ToString() : " - "), false, false));
            pdfpTable.AddCell(PopulatePDFCellFont10(!string.IsNullOrEmpty(strLowestIncomeAmount) ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(strLowestIncomeAmount)).ToString() : " - ", false, false));
            pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("{0}/{1}", intYes, intNo), false, false));
            #endregion

            #region Get the Credit Assessment
            CreditAssessmentDb CAdb = new CreditAssessmentDb();
            CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
            string IsGenerated = string.Empty;
            if (CADt.Rows.Count > 0)
            {
                CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                IsGenerated = !CARow.IsIsGeneratedNull() ? (CARow.IsGenerated ? "Y" : "N") : string.Empty;
                pdfpTable.AddCell(PopulatePDFCA(string.Format(Format.GetDecimalPlacesWithoutRounding(CARow.CreditAssessmentAmount).ToString()), false, false, IsGenerated));
                decCATotal = decCATotal + CARow.CreditAssessmentAmount;
            }
            else                            
                pdfpTable.AddCell(PopulatePDFCA(string.Format(""), false, false, IsGenerated));
            
            #endregion
        }
        #region Reseting values to 0 or empty
        strLowestIncomeAmount = string.Empty;
        intYes = 0;
        intNo = 0;
        decIncomeAmount = 0;
        #endregion
    }

    private static void CreditAssessmentAddAverageHeaders(int intMonthsToLeas, PdfPTable pdfpTable, Document pdfDoc)
    {
        //pdfpTable.AddCell(PopulatePDFCell(string.Format("Avg in past {0} mths", intMonthsToLeas), true, true));
        //pdfpTable.AddCell(PopulatePDFCell("Average in past 3 mths", true, true));
        //pdfpTable.AddCell(PopulatePDFCell(string.Format("Lowest in past mths"), true, true));
        //pdfpTable.AddCell(PopulatePDFCell("Yes/No", true, true));
        //pdfpTable.AddCell(PopulatePDFCell("Credit Assessment", true, true));
        pdfpTable.SpacingBefore = 10f;
        pdfpTable.SpacingAfter = 10f;
        pdfDoc.Add(pdfpTable);
    }

    private static void CreditAssessmentAddCATotalRow(decimal decCATotal, PdfPTable pdfpTable, int columnCount)
    {
        columnCount = columnCount + 7;
        for (int i = 0; i < columnCount - 2; i++)
        {
            pdfpTable.AddCell(PopulatePDFCellFont10(string.Empty, false, false));
        }
        pdfpTable.AddCell(PopulatePDFCellFont10("CA Total", false, false));
        pdfpTable.AddCell(PopulatePDFCellFont10(Format.GetDecimalPlacesWithoutRounding(decCATotal).ToString(),false,false));
    }

    private static void MonthlyZoneTitle(Document pdfDoc, itsFont bold)
    {
        #region Monthly Zone Label
        Paragraph TitleParagraph = new Paragraph(new Chunk("Monthly Zone", bold));
        TitleParagraph.Font.Size = 14;
        TitleParagraph.SpacingBefore = 30f;
        TitleParagraph.SpacingAfter = 20f;
        pdfDoc.Add(TitleParagraph);
        #endregion
    }

    private static void MonthlyZonePersonalName(string applicantType, int orderNo, string name, string nric, itsFont bold, Document pdfDoc)
    {
        #region Applicant Name and NRIC Label for each Person
        string strSummary = string.Empty;
        if (applicantType.Equals(PersonalTypeEnum.HA.ToString()))
            strSummary = string.Format("Applicant {0} Name: {1} ({2})", orderNo, name, nric);
        else if (applicantType.Equals(PersonalTypeEnum.OC.ToString()))
            strSummary = string.Format("Occupier {0} Name: {1} ({2})", orderNo, name, nric);
        else
            strSummary = string.Format("{0} Name: {1} ({2})", orderNo, name, nric);

        Paragraph TitleParagraph = new Paragraph(new Chunk(strSummary, bold));
        TitleParagraph.Font.Size = 14;
        pdfDoc.Add(TitleParagraph);
        #endregion
    }

    private static void MonthlyZoneGetValues(int docAppId, DataTable IncomeDt, PdfPTable pdfpTable, Document pdfDoc, List<int> listIncomeId, string refType)
    {        
        if (IncomeDt.Rows.Count > 0)
        {
            foreach (DataRow IncomeRow in IncomeDt.Rows)
            {
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                    pdfpTable = new PdfPTable(5);
                else
                    pdfpTable = new PdfPTable(4);
                pdfpTable.SpacingBefore = 10f;
                pdfpTable.SpacingAfter = 10f;
                pdfpTable.WidthPercentage = 100;

                DateTimeFormatInfo info = new DateTimeFormatInfo();

                Phrase p1 = new Phrase();
                p1.Add(new Chunk(string.Format(string.Format("{0} {1}",
                    info.GetAbbreviatedMonthName(int.Parse(IncomeRow["IncomeMonth"].ToString())), IncomeRow["IncomeYear"].ToString()), true, true)));
                PdfPCell cell1 = new PdfPCell(p1);
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                    cell1.Colspan = 5;
                else
                    cell1.Colspan = 4;
                cell1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                cell1.BackgroundColor = its.text.Color.CYAN;
                pdfpTable.AddCell(cell1);

                pdfpTable.AddCell(PopulatePDFCell("Item", false, true));
                pdfpTable.AddCell(PopulatePDFCell("Amount", false, true));
                pdfpTable.AddCell(PopulatePDFCell("Gross Income", false, true));
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    pdfpTable.AddCell(PopulatePDFCell("Allowance", false, true));
                    pdfpTable.AddCell(PopulatePDFCell("Overtime", false, true));
                }
                else
                    pdfpTable.AddCell(PopulatePDFCell("AHGIncome", false, true));

                DataTable dtIncomeItems = IncomeDb.GetIncomeDetailsByIncomeVersion(int.Parse(IncomeRow["IncomeVersionId"].ToString()));
                foreach (DataRow r in dtIncomeItems.Rows)
                {
                    pdfpTable.AddCell(PopulatePDFCell(r["IncomeItem"].ToString(), false, false));
                    pdfpTable.AddCell(PopulatePDFCell(r["IncomeAmount"].ToString(), false, false));
                    pdfpTable.AddCell(PopulatePDFCell(!bool.Parse(r["GrossIncome"].ToString()) ? "No" : "Yes", false, false));
                    if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                    {
                        pdfpTable.AddCell(PopulatePDFCell(!bool.Parse(r["Allowance"].ToString()) ? "No" : "Yes", false, false));
                        pdfpTable.AddCell(PopulatePDFCell(!bool.Parse(r["Overtime"].ToString()) ? "No" : "Yes", false, false));
                    }
                    else
                        pdfpTable.AddCell(PopulatePDFCell(!bool.Parse(r["AHGIncome"].ToString()) ? "No" : "Yes", false, false));
                }

                DataTable dtTotals = IncomeDb.GetIncomeDetailTotals(int.Parse(IncomeRow["IncomeVersionId"].ToString()));

                foreach (DataRow r in dtTotals.Rows)
                {
                    pdfpTable.AddCell(PopulatePDFCell(string.Empty, false, false));
                    pdfpTable.AddCell(PopulatePDFCell(r["IncomeAmount"].ToString(), true, false));
                    pdfpTable.AddCell(PopulatePDFCell(!r.IsNull("GrossIncome") ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(r["GrossIncome"].ToString())).ToString()
                        : "0.00", false, false));
                    if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                    {
                        pdfpTable.AddCell(PopulatePDFCell(!r.IsNull("Allowance") ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(r["Allowance"].ToString())).ToString()
                            : "0.00", false, false));
                        pdfpTable.AddCell(PopulatePDFCell(!r.IsNull("Overtime") ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(r["Overtime"].ToString())).ToString()
                            : "0.00", false, false));
                    }
                    else
                    {
                        pdfpTable.AddCell(PopulatePDFCell(!r.IsNull("AHGIncome") ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(r["AHGIncome"].ToString())).ToString()
                            : "0.00", false, false));
                    }
                }


                pdfDoc.Add(pdfpTable);

                pdfpTable = new PdfPTable(3);
                pdfpTable.SpacingBefore = 10f;
                pdfpTable.SpacingAfter = 10f;
                pdfpTable.WidthPercentage = 100;

                ViewLogDb viewLogDb = new ViewLogDb();
                ViewLog.ViewLogDataTable dt = viewLogDb.GetViewLogByIncomeId(int.Parse(IncomeRow["Id"].ToString()));

                pdfpTable.AddCell(PopulatePDFCellFont10("Viewed On", false, true));
                pdfpTable.AddCell(PopulatePDFCellFont10("Viewed By", false, true));
                pdfpTable.AddCell(PopulatePDFCellFont10("Income Document | CMDocumentId", false, true));

                if (dt.Rows.Count > 0)
                {
                    foreach (ViewLog.ViewLogRow r in dt.Rows)
                    {
                        string monthName = new DateTime(DateTime.Parse(r.LogDate.ToString()).Year, DateTime.Parse(r.LogDate.ToString()).Month,
                            DateTime.Parse(r.LogDate.ToString()).Day, DateTime.Parse(r.LogDate.ToString()).Hour, DateTime.Parse(r.LogDate.ToString()).Minute,
                            DateTime.Parse(r.LogDate.ToString()).Second).ToString("d MMMM yyyy hh mm tt", System.Globalization.CultureInfo.InvariantCulture);


                        pdfpTable.AddCell(PopulatePDFCellFont10(monthName, false, false));
                        pdfpTable.AddCell(PopulatePDFCellFont10(r["Name"].ToString(), false, false));
                        pdfpTable.AddCell(PopulatePDFCellFont10(string.Format("{0} | {1}", r["Description"].ToString(),
                            !r.IsNull("CMDocumentId") ? r["CMDocumentId"].ToString() : " - "), false, false));
                    }
                }

                pdfDoc.Add(pdfpTable);

                listIncomeId.Add(int.Parse(IncomeRow["Id"].ToString()));
            }
        }
    }

    private class IncomeWorksheet
    {
        private int _MonthYearId;

        public int MonthYearId
        {
            get { return _MonthYearId; }
            set { _MonthYearId = value; }
        }

        private string _MonthYear;

        public string MonthYear
        {
            get { return _MonthYear; }
            set { _MonthYear = value; }
        }

        private decimal _AverageGrossIncome;

        public decimal AverageGrossIncome
        {
            get { return _AverageGrossIncome; }
            set { _AverageGrossIncome = value; }
        }

        private decimal _AverageAllowance;

        public decimal AverageAllowance
        {
            get { return _AverageAllowance; }
            set { _AverageAllowance = value; }
        }

        private decimal _AverageOT;

        public decimal AverageOT
        {
            get { return _AverageOT; }
            set { _AverageOT = value; }
        }
        

    }
}




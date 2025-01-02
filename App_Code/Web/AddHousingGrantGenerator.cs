using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Web.Security;

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
/// Summary description for AddHousingGrantGenerator
/// </summary>
public class AddHousingGrantGenerator
{
    static decimal TotalAmount;

    public static MemoryStream GeneratePDFHousingGrant(int docAppId)
    {
        MemoryStream pdfStream = new MemoryStream();
        Document pdfDoc = new Document(PageSize.A4.Rotate());
        
        pdfDoc.SetMargins(pdfDoc.LeftMargin, pdfDoc.RightMargin, 80, 80);
        List<string> listMonthYear = new List<string>();
        Dictionary<string, IncomeWorksheet> dicIncomeWorksheet = null;
        IncomeWorksheet clsIncomeWorksheet;

        try
        {
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);
            pdfWriter.AddViewerPreference(PdfName.PICKTRAYBYPDFSIZE, PdfBoolean.PDFTRUE);
            PdfPTable pdfpTable = null;

            itsFont bold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

            pdfDoc.Open();

            #region Computation of Average Houshold Income for AHG
            Paragraph TitleParagraph = new Paragraph(new Chunk("Computation of Average Gross Income for AHG", bold));
            TitleParagraph.Font.Size = 16;
            pdfDoc.Add(TitleParagraph);
            #endregion 

            #region Get RefNo From DocApp to get the number of columns to create for the PDF table.
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppById(docAppId);
            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                string refType = docAppRow.RefType.ToUpper().Trim();

                if (dicIncomeWorksheet == null)
                    dicIncomeWorksheet = new Dictionary<string, IncomeWorksheet>();

                if (refType.Contains(ReferenceTypeEnum.RESALE.ToString()))
                {
                    ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                    ResaleInterface.ResaleInterfaceDataTable resaleInterfaceDt = resaleInterfaceDb.GetResaleInterfaceByRefNo(docAppRow.RefNo);

                    #region The Refno or Application Number and Summary Phrase
                    TitleParagraph = new Paragraph(new Chunk("Application Number: " + docAppRow.RefNo, bold));
                    TitleParagraph.Font.Size = 12;
                    pdfDoc.Add(TitleParagraph);                    
                    TitleParagraph = new Paragraph(new Chunk("Summary", bold));
                    TitleParagraph.Font.Size = 14;
                    pdfDoc.Add(TitleParagraph);
                    #endregion

                    if (resaleInterfaceDt.Rows.Count > 0)
                    {
                        
                        pdfpTable = new PdfPTable(resaleInterfaceDt.Rows.Count + 1);
                        pdfpTable.WidthPercentage = 100;

                        #region Loop all the NRIC for the refno from the DocApp Table
                        pdfpTable.AddCell(PopulatePDFCell("Monthly Income ", true, true, false));
                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {

                            Phrase p = new Phrase();
                            if (resaleInterfaceRow.ApplicantType.ToUpper().Equals("BU"))
                                p.Add(new Chunk(string.Format("Buyer {0}{1}", resaleInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            else if (resaleInterfaceRow.ApplicantType.ToUpper().Equals("SE"))
                                p.Add(new Chunk(string.Format("Seller {0}{1}", resaleInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            else
                                p.Add(new Chunk(string.Format("{0}{1}", resaleInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            p.Add(new Chunk(string.Format("•    Name: {0}{1}", resaleInterfaceRow.Name, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    Date of Birth: {0}{1}", resaleInterfaceRow.DateOfBirth, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    NRIC: {0}{1}", resaleInterfaceRow.Nric, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    Citizenship: {0}{1}", !resaleInterfaceRow.IsCitizenshipNull() ? resaleInterfaceRow.Citizenship : "-", Environment.NewLine)));
                            pdfpTable.AddCell(p);


                        }
                        //The Code below will put it to a Dictionary
                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, resaleInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    
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
                                    clsIncomeWorksheet = new IncomeWorksheet();
                                    clsIncomeWorksheet.IncomeAmount = decimal.Parse(IncomeRow["GrossIncome"].ToString());
                                    if (!dicIncomeWorksheet.ContainsKey(resaleInterfaceRow.Nric + IncomeRow["MonthYear"].ToString()))
                                        dicIncomeWorksheet.Add(resaleInterfaceRow.Nric + IncomeRow["MonthYear"].ToString(), clsIncomeWorksheet);
                                }
                            }
                        }

                        foreach (string str in listMonthYear)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(str, false, false, false));
                            foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                            {
                                if (dicIncomeWorksheet.ContainsKey(resaleInterfaceRow.Nric + str))
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(dicIncomeWorksheet[resaleInterfaceRow.Nric + str].IncomeAmount.ToString("N0"), false, false,false));
                                }
                                else
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(" - ", false, false, false));
                                }
                            }
                        }
                        pdfpTable.SpacingBefore = 20f;
                        pdfpTable.SpacingAfter = 30f;
                        


                        //foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        //{
                        //    if(resaleInterfaceRow.ApplicantType.ToUpper().Equals("BU"))
                        //        pdfpTable.AddCell(PopulatePDFCell("Buyer " + resaleInterfaceRow.OrderNo.ToString(), true, true, false));
                        //    else if (resaleInterfaceRow.ApplicantType.ToUpper().Equals("SE"))
                        //        pdfpTable.AddCell(PopulatePDFCell("Seller " + resaleInterfaceRow.OrderNo.ToString(), true, true, false));
                        //    else
                        //        pdfpTable.AddCell(PopulatePDFCell(resaleInterfaceRow.OrderNo.ToString(), true, true, false));

                        //    pdfpTable.AddCell(PopulatePDFCell(resaleInterfaceRow.Name, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell("UIN", true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(resaleInterfaceRow.Nric, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell("Date of Birth", true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(resaleInterfaceRow.DateOfBirth, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(string.Empty, true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(string.Empty, false, true, false));

                        //    #region Get Income Assessment for each NRIC
                        //    DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, resaleInterfaceRow.Nric);
                        //    if (IncomeDt.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow IncomeRow in IncomeDt.Rows)
                        //        {
                        //            pdfpTable.AddCell(PopulatePDFCell(IncomeRow["MonthYear"].ToString(), false, false, false));
                        //            pdfpTable.AddCell(PopulatePDFCell("Income", true, false, false));
                        //            decimal amount = 0;
                        //            //amount = (!string.IsNullOrEmpty(IncomeRow["Allowance"].ToString()) ? decimal.Parse(IncomeRow["Allowance"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["CPFIncome"].ToString()) ? decimal.Parse(IncomeRow["CPFIncome"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? decimal.Parse(IncomeRow["GrossIncome"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ? decimal.Parse(IncomeRow["AHGIncome"].ToString()) : 0);
                        //            amount = (!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? decimal.Parse(IncomeRow["GrossIncome"].ToString()) : 0);
                        //            TotalAmount = TotalAmount + amount;
                        //            pdfpTable.AddCell(PopulatePDFCell("$ " + amount.ToString("N2"), false, false, false));

                        //            pdfpTable.AddCell(PopulatePDFCell("", false, false, false));
                        //        }
                        //    }
                        //    #endregion
                        //}
                        #endregion

                        #region Cells that contain the Total Income and Average Income
                        pdfpTable.AddCell(PopulatePDFCell("Total Income", true, true, false));
                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {
                            decimal totalAmount = 0;
                            foreach (string str in listMonthYear)
                            {                               
                                if (dicIncomeWorksheet.ContainsKey(resaleInterfaceRow.Nric + str))
                                {                                    
                                    totalAmount = totalAmount + dicIncomeWorksheet[resaleInterfaceRow.Nric + str].IncomeAmount;
                                }
                            }
                            pdfpTable.AddCell(PopulatePDFCell(totalAmount.ToString(), true, true, false));
                        }

                        pdfpTable.AddCell(PopulatePDFCell("Average Income", true, true, false));
                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {
                            decimal totalAmount = 0;
                            int count = 0;
                            foreach (string str in listMonthYear)
                            {
                                if (dicIncomeWorksheet.ContainsKey(resaleInterfaceRow.Nric + str))
                                {
                                    count = count + 1;
                                    totalAmount = totalAmount + dicIncomeWorksheet[resaleInterfaceRow.Nric + str].IncomeAmount;
                                }
                            }
                            if (count > 0)
                                pdfpTable.AddCell(PopulatePDFCell((totalAmount / count).ToString("N2"), true, true, false));
                        }
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //
                        //pdfpTable.AddCell(PopulatePDFCell("$ " + TotalAmount.ToString("N2"), true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("Average Income", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("$ " + (TotalAmount / 12).ToString("N2"), true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.SpacingBefore = 20f;
                        //pdfpTable.SpacingAfter = 30f;
                        pdfDoc.Add(pdfpTable);
                        #endregion

                        #region Average Gross Income computed, computed by and checked by fields.
                        TitleParagraph = new Paragraph(new Chunk("Average Gross Income computed:", bold));
                        TitleParagraph.Font.Size = 12;
                        TitleParagraph.SpacingAfter = 30f;
                        pdfDoc.Add(TitleParagraph);

                        ProfileDb profileDb = new ProfileDb();
                        MembershipUser user = Membership.GetUser();
                        Guid currentUserId = (Guid)user.ProviderUserKey;
                        string username = profileDb.GetUserFullName(currentUserId);

                        pdfpTable = new PdfPTable(2);
                        pdfpTable.WidthPercentage = 100;

                        pdfpTable.AddCell(PopulatePDFCell("____________________________", false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("____________________________", false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("Computed by: " + username, false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("Checked by: ", false, false, true));
                        pdfDoc.Add(pdfpTable);
                        #endregion

                        #region Notes
                        pdfDoc.NewPage();
                        TitleParagraph = new Paragraph(new Chunk("Note", bold));
                        TitleParagraph.Font.Size = 12;
                        pdfDoc.Add(TitleParagraph);
                        pdfDoc.Add(PopulateNotesSection("1. This form is applicable for Enhanced AHG implemented wef 15 Aug 11. (Income revised to $10,000)."));
                        pdfDoc.Add(PopulateNotesSection("2. The computed Average Income is rounded down to the nearest $1."));
                        pdfDoc.Add(PopulateNotesSection("3. Age is calculated based on 1st day of the month."));
                        pdfDoc.Add(PopulateNotesSection("4. CPF Contribution Rate Table:"));

                        pdfpTable = new PdfPTable(3);
                        float[] columnwidths = { 1f, 1f, 3f };
                        pdfpTable.SetWidths(columnwidths);
                        pdfpTable.SpacingBefore = 20f;
                        pdfpTable.WidthPercentage = 60;
                        pdfpTable.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfpTable.AddCell(PopulateNotesSection("Option", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Rate", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Description", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("1.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("36.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Total CPF Contribution (wef 1 Sep 11)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("2.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("35.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Total CPF Contribution (wef 1 Sep 11)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("3.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("sd/ue", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("4.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Income certified by employer", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("5.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Self-employed (income based on NOA)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("6.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Monthly Income (based on payslip)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("7.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("9.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("1st yr SPR", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("8.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("24.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("2nd yr SPR", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("9.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Negative Trade", false, false, false));
                        pdfDoc.Add(pdfpTable);

                        TitleParagraph = new Paragraph(new Chunk("*Footnote:", bold));
                        TitleParagraph.Font.Size = 12;
                        TitleParagraph.SpacingBefore = 30f;
                        pdfDoc.Add(TitleParagraph);
                        pdfDoc.Add(PopulateNotesSection("a) Total CPF contribution rate (wef 1 Jul 07) is 34.5%."));
                        pdfDoc.Add(PopulateNotesSection("b) Total CPF contribution rate (wef 1 Sep 10) is 35.0%."));
                        #endregion

                        #region Get Details for each NRIC

                        foreach (ResaleInterface.ResaleInterfaceRow resaleInterfaceRow in resaleInterfaceDt.Rows)
                        {
                            pdfDoc.NewPage();                
                            if (resaleInterfaceRow.ApplicantType.ToUpper().Equals("BU"))
                                pdfDoc.Add(new Paragraph(new Chunk("Buyer " + resaleInterfaceRow.OrderNo.ToString(), bold)));
                            else if (resaleInterfaceRow.ApplicantType.ToUpper().Equals("SE"))
                                pdfDoc.Add(new Paragraph(new Chunk("Seller " + resaleInterfaceRow.OrderNo.ToString(), bold)));
                            else
                                pdfDoc.Add(new Paragraph(new Chunk("" + resaleInterfaceRow.OrderNo.ToString(), bold)));                         
                            List list = new List(false);
                            list.SetListSymbol("•");
                            Phrase p = new Phrase();
                            ListItem listItem;
                            p.Add(new Chunk("     Name: ", bold));
                            p.Add(new Chunk(resaleInterfaceRow.Name));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     Date of Birth: ", bold));
                            p.Add(new Chunk(resaleInterfaceRow.DateOfBirth));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     NRIC: ", bold));
                            p.Add(new Chunk(resaleInterfaceRow.Nric));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     Citizenship: ", bold));
                            p.Add(new Chunk(!resaleInterfaceRow.IsCitizenshipNull() ? resaleInterfaceRow.Citizenship : "-"));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            pdfDoc.Add(list);

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, resaleInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    pdfpTable = new PdfPTable(4);
                                    pdfpTable.SpacingBefore = 30f;
                                    pdfpTable.WidthPercentage = 100;
                                    p.Clear();
                                    p.Add(new Chunk(IncomeRow["MonthYear"].ToString(), bold));
                                    PdfPCell cell = new PdfPCell(p);
                                    cell.Colspan = 4;
                                    cell.BackgroundColor = its.text.Color.CYAN;
                                    pdfpTable.AddCell(cell);
                                    DataTable IncomeDetailsDt = new DataTable();
                                    if (int.Parse(IncomeRow["IncomeVersionId"].ToString()) > 0)
                                        IncomeDetailsDt = IncomeDb.GetIncomeDetailsByIncomeVersion(int.Parse(IncomeRow["IncomeVersionId"].ToString()));
                                    pdfpTable.AddCell(PopulatePDFCell("Payslip/Income Letter Item", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Pay", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Gross Income", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("AHG Income", true, true, false));
                                    if (IncomeDetailsDt.Rows.Count > 0)
                                    {
                                        foreach (DataRow IncomeDetailsRow in IncomeDetailsDt.Rows)
                                        {
                                            pdfpTable.AddCell(PopulatePDFCell(IncomeDetailsRow["IncomeItem"].ToString(), false, false, false));
                                            pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(IncomeDetailsRow["IncomeAmount"].ToString()).ToString("N2"), false, false, false));
                                            bool resultTryParse = false;
                                            if (bool.TryParse(IncomeDetailsRow["GrossIncome"].ToString(), out resultTryParse))
                                            {
                                                pdfpTable.AddCell(PopulatePDFCell(resultTryParse ? "[x]" : "[ ]"));
                                            }
                                            if (bool.TryParse(IncomeDetailsRow["AHGIncome"].ToString(), out resultTryParse))
                                            {
                                                pdfpTable.AddCell(PopulatePDFCell(resultTryParse ? "[x]" : "[ ]"));
                                            }
                                        }
                                    }
                                    pdfpTable.AddCell(PopulatePDFCell("", true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Total", true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? IncomeRow["GrossIncome"].ToString() : "0").ToString("N2"), true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ? IncomeRow["AHGIncome"].ToString() : "0.00").ToString("N2"), true, false, false));
                                    pdfDoc.Add(pdfpTable);

                                }

                            }

                        }

                        #endregion

                    }
                }
                else if (refType.Contains(ReferenceTypeEnum.SALES.ToString()))
                {

                    SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                    SalesInterface.SalesInterfaceDataTable salesInterfaceDt = salesInterfaceDb.GetSalesInterfaceByRefNo(docAppRow.RefNo);

                    #region The Refno or Application Number and Summary Phrase
                    TitleParagraph = new Paragraph(new Chunk("Application Number: " + docAppRow.RefNo, bold));
                    TitleParagraph.Font.Size = 12;
                    pdfDoc.Add(TitleParagraph);
                    TitleParagraph = new Paragraph(new Chunk("Summary", bold));
                    TitleParagraph.Font.Size = 14;
                    pdfDoc.Add(TitleParagraph);
                    #endregion

                    if (salesInterfaceDt.Rows.Count > 0)
                    {

                        pdfpTable = new PdfPTable(salesInterfaceDt.Rows.Count + 1);
                        pdfpTable.WidthPercentage = 100;

                        #region Loop all the NRIC for the refno from the DocApp Table
                        pdfpTable.AddCell(PopulatePDFCell("Monthly Income ", true, true, false));
                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {

                            Phrase p = new Phrase();
                            if (salesInterfaceRow.ApplicantType.ToUpper().Equals("HA"))
                                p.Add(new Chunk(string.Format("Applicant {0}{1}", salesInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            else if (salesInterfaceRow.ApplicantType.ToUpper().Equals("OC"))
                                p.Add(new Chunk(string.Format("Occupier {0}{1}", salesInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            else
                                p.Add(new Chunk(string.Format("{0}{1}", salesInterfaceRow.OrderNo.ToString(), Environment.NewLine), bold));
                            p.Add(new Chunk(string.Format("•    Name: {0}{1}", salesInterfaceRow.Name, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    Date of Birth: {0}{1}", salesInterfaceRow.DateOfBirth, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    NRIC: {0}{1}", salesInterfaceRow.Nric, Environment.NewLine)));
                            p.Add(new Chunk(string.Format("•    Citizenship: {0}{1}", !salesInterfaceRow.IsCitizenshipNull() ? salesInterfaceRow.Citizenship : "-", Environment.NewLine)));
                            pdfpTable.AddCell(p);


                        }
                        //The Code below will put it to a Dictionary
                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {
                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, salesInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {

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
                                    clsIncomeWorksheet = new IncomeWorksheet();
                                    clsIncomeWorksheet.IncomeAmount = decimal.Parse(IncomeRow["GrossIncome"].ToString());
                                    if (!dicIncomeWorksheet.ContainsKey(salesInterfaceRow.Nric + IncomeRow["MonthYear"].ToString()))
                                        dicIncomeWorksheet.Add(salesInterfaceRow.Nric + IncomeRow["MonthYear"].ToString(), clsIncomeWorksheet);
                                }
                            }
                        }

                        foreach (string str in listMonthYear)
                        {
                            pdfpTable.AddCell(PopulatePDFCell(str, false, false, false));
                            foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                            {
                                if (dicIncomeWorksheet.ContainsKey(salesInterfaceRow.Nric + str))
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(dicIncomeWorksheet[salesInterfaceRow.Nric + str].IncomeAmount.ToString("N0"), false, false, false));
                                }
                                else
                                {
                                    pdfpTable.AddCell(PopulatePDFCell(" - ", false, false, false));
                                }
                            }
                        }
                        pdfpTable.SpacingBefore = 20f;
                        pdfpTable.SpacingAfter = 30f;
                        //pdfDoc.Add(pdfpTable);


                        //pdfpTable = new PdfPTable(4);
                        //pdfpTable.WidthPercentage = 100;

                        //#region Loop all the NRIC for the refno from the DocApp Table

                        //foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        //{
                        //    if (salesInterfaceRow.ApplicantType.ToUpper().Equals((PersonalTypeEnum.HA.ToString().ToUpper())))
                        //        pdfpTable.AddCell(PopulatePDFCell("Applcant " + salesInterfaceRow.OrderNo.ToString(), true, true, false));
                        //    else if (salesInterfaceRow.ApplicantType.ToUpper().Equals((PersonalTypeEnum.OC.ToString().ToUpper())))
                        //        pdfpTable.AddCell(PopulatePDFCell("Occupier " + salesInterfaceRow.OrderNo.ToString(), true, true, false));
                        //    else
                        //        pdfpTable.AddCell(PopulatePDFCell(salesInterfaceRow.OrderNo.ToString(), true, true, false));

                        //    pdfpTable.AddCell(PopulatePDFCell(salesInterfaceRow.Name, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell("UIN", true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(salesInterfaceRow.Nric, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell("Date of Birth", true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(salesInterfaceRow.DateOfBirth, false, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(string.Empty, true, true, false));
                        //    pdfpTable.AddCell(PopulatePDFCell(string.Empty, false, true, false));

                        //    #region Get Income Assessment for each NRIC
                        //    DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, salesInterfaceRow.Nric);
                        //    if (IncomeDt.Rows.Count > 0)
                        //    {
                        //        foreach (DataRow IncomeRow in IncomeDt.Rows)
                        //        {
                        //            pdfpTable.AddCell(PopulatePDFCell(IncomeRow["MonthYear"].ToString(), false, false, false));
                        //            pdfpTable.AddCell(PopulatePDFCell("Income", true, false, false));
                        //            decimal amount = 0;
                        //            //amount = (!string.IsNullOrEmpty(IncomeRow["Allowance"].ToString()) ? decimal.Parse(IncomeRow["Allowance"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["CPFIncome"].ToString()) ? decimal.Parse(IncomeRow["CPFIncome"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? decimal.Parse(IncomeRow["GrossIncome"].ToString()) : 0) +
                        //            //        (!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ? decimal.Parse(IncomeRow["AHGIncome"].ToString()) : 0);
                        //            amount = (!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? decimal.Parse(IncomeRow["GrossIncome"].ToString()) : 0);
                        //            TotalAmount = TotalAmount + amount;
                        //            pdfpTable.AddCell(PopulatePDFCell("$ " + amount.ToString("N2"), false, false, false));

                        //            pdfpTable.AddCell(PopulatePDFCell("", false, false, false));
                        //        }
                        //    }
                        //    #endregion
                        //}
                        #endregion

                        #region Cells that contain the Total Income and Average Income
                        pdfpTable.AddCell(PopulatePDFCell("Total Income", true, true, false));
                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {
                            decimal totalAmount = 0;
                            foreach (string str in listMonthYear)
                            {
                                if (dicIncomeWorksheet.ContainsKey(salesInterfaceRow.Nric + str))
                                {
                                    totalAmount = totalAmount + dicIncomeWorksheet[salesInterfaceRow.Nric + str].IncomeAmount;
                                }
                            }
                            pdfpTable.AddCell(PopulatePDFCell(totalAmount.ToString(), true, true, false));
                        }

                        pdfpTable.AddCell(PopulatePDFCell("Average Income", true, true, false));
                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {
                            decimal totalAmount = 0;
                            int count = 0;
                            foreach (string str in listMonthYear)
                            {
                                if (dicIncomeWorksheet.ContainsKey(salesInterfaceRow.Nric + str))
                                {
                                    count = count + 1;
                                    totalAmount = totalAmount + dicIncomeWorksheet[salesInterfaceRow.Nric + str].IncomeAmount;
                                }
                            }
                            if (count > 0)
                                pdfpTable.AddCell(PopulatePDFCell((totalAmount / count).ToString("N2"), true, true, false));
                        }
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("Total Income", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("$ " + TotalAmount.ToString("N2"), true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("Average Income", true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("$ " + (TotalAmount / 12).ToString("N2"), true, true, false));
                        //pdfpTable.AddCell(PopulatePDFCell("", true, true, false));
                        //pdfpTable.SpacingBefore = 20f;
                        //pdfpTable.SpacingAfter = 30f;
                        pdfDoc.Add(pdfpTable);
                        #endregion

                        #region Average Gross Income computed, computed by and checked by fields.
                        TitleParagraph = new Paragraph(new Chunk("Average Gross Income computed:", bold));
                        TitleParagraph.Font.Size = 12;
                        TitleParagraph.SpacingAfter = 30f;
                        pdfDoc.Add(TitleParagraph);

                        ProfileDb profileDb = new ProfileDb();
                        MembershipUser user = Membership.GetUser();
                        Guid currentUserId = (Guid)user.ProviderUserKey;
                        string username = profileDb.GetUserFullName(currentUserId);

                        pdfpTable = new PdfPTable(2);
                        pdfpTable.WidthPercentage = 100;

                        pdfpTable.AddCell(PopulatePDFCell("____________________________", false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("____________________________", false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("Computed by: " + username, false, false, true));
                        pdfpTable.AddCell(PopulatePDFCell("Checked by: ", false, false, true));
                        pdfDoc.Add(pdfpTable);
                        #endregion

                        #region Notes
                        pdfDoc.NewPage();
                        TitleParagraph = new Paragraph(new Chunk("Note", bold));
                        TitleParagraph.Font.Size = 12;
                        pdfDoc.Add(TitleParagraph);
                        pdfDoc.Add(PopulateNotesSection("1. This form is applicable for Enhanced AHG implemented wef 15 Aug 11. (Income revised to $10,000)."));
                        pdfDoc.Add(PopulateNotesSection("2. The computed Average Income is rounded down to the nearest $1."));
                        pdfDoc.Add(PopulateNotesSection("3. Age is calculated based on 1st day of the month."));
                        pdfDoc.Add(PopulateNotesSection("4. CPF Contribution Rate Table:"));

                        pdfpTable = new PdfPTable(3);
                        float[] columnwidths = { 1f, 1f, 3f };
                        pdfpTable.SetWidths(columnwidths);
                        pdfpTable.SpacingBefore = 20f;
                        pdfpTable.WidthPercentage = 60;
                        pdfpTable.HorizontalAlignment = Element.ALIGN_LEFT;
                        pdfpTable.AddCell(PopulateNotesSection("Option", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Rate", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Description", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("1.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("36.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Total CPF Contribution (wef 1 Sep 11)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("2.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("35.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Total CPF Contribution (wef 1 Sep 11)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("3.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("sd/ue", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("4.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Income certified by employer", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("5.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Self-employed (income based on NOA)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("6.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("0.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Monthly Income (based on payslip)", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("7.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("9.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("1st yr SPR", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("8.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("24.000%", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("2nd yr SPR", false, false, false));
                        pdfpTable.AddCell(PopulateNotesSection("9.", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("", true, true, true));
                        pdfpTable.AddCell(PopulateNotesSection("Negative Trade", false, false, false));
                        pdfDoc.Add(pdfpTable);

                        TitleParagraph = new Paragraph(new Chunk("*Footnote:", bold));
                        TitleParagraph.Font.Size = 12;
                        TitleParagraph.SpacingBefore = 30f;
                        pdfDoc.Add(TitleParagraph);
                        pdfDoc.Add(PopulateNotesSection("a) Total CPF contribution rate (wef 1 Jul 07) is 34.5%."));
                        pdfDoc.Add(PopulateNotesSection("b) Total CPF contribution rate (wef 1 Sep 10) is 35.0%."));
                        #endregion

                        #region Get Details for each NRIC

                        foreach (SalesInterface.SalesInterfaceRow salesInterfaceRow in salesInterfaceDt.Rows)
                        {
                            pdfDoc.NewPage();                            
                            if (salesInterfaceRow.ApplicantType.ToUpper().Equals("HA"))
                                pdfDoc.Add(new Paragraph(new Chunk("Applicant " + salesInterfaceRow.OrderNo.ToString(), bold)));
                            else if (salesInterfaceRow.ApplicantType.ToUpper().Equals("OC"))
                                pdfDoc.Add(new Paragraph(new Chunk("Occupier " + salesInterfaceRow.OrderNo.ToString(), bold)));
                            else
                                pdfDoc.Add(new Paragraph(new Chunk("" + salesInterfaceRow.OrderNo.ToString(), bold)));                            
                            List list = new List(false);
                            list.SetListSymbol("•");
                            Phrase p = new Phrase();
                            ListItem listItem;
                            p.Add(new Chunk("     Name: ", bold));
                            p.Add(new Chunk(salesInterfaceRow.Name));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     Date of Birth: ", bold));
                            p.Add(new Chunk(salesInterfaceRow.DateOfBirth));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     NRIC: ", bold));
                            p.Add(new Chunk(salesInterfaceRow.Nric));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            p.Clear();
                            p.Add(new Chunk("     Citizenship: ", bold));
                            p.Add(new Chunk(!salesInterfaceRow.IsCitizenshipNull() ? salesInterfaceRow.Citizenship : "-"));
                            listItem = new ListItem(p);
                            list.Add(listItem);
                            pdfDoc.Add(list);

                            DataTable IncomeDt = IncomeDb.GetDataForIncomeAssessment(docAppId, salesInterfaceRow.Nric);
                            if (IncomeDt.Rows.Count > 0)
                            {
                                foreach (DataRow IncomeRow in IncomeDt.Rows)
                                {
                                    pdfpTable = new PdfPTable(4);
                                    pdfpTable.SpacingBefore = 30f;
                                    pdfpTable.WidthPercentage = 100;
                                    p.Clear();
                                    p.Add(new Chunk(IncomeRow["MonthYear"].ToString(), bold));
                                    PdfPCell cell = new PdfPCell(p);
                                    cell.Colspan = 4;
                                    cell.BackgroundColor = its.text.Color.CYAN;
                                    pdfpTable.AddCell(cell);
                                    DataTable IncomeDetailsDt = new DataTable();
                                    if (int.Parse(IncomeRow["IncomeVersionId"].ToString()) > 0)
                                        IncomeDetailsDt = IncomeDb.GetIncomeDetailsByIncomeVersion(int.Parse(IncomeRow["IncomeVersionId"].ToString()));
                                    pdfpTable.AddCell(PopulatePDFCell("Payslip/Income Letter Item", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Pay", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Gross Income", true, true, false));
                                    pdfpTable.AddCell(PopulatePDFCell("AHG Income", true, true, false));
                                    if (IncomeDetailsDt.Rows.Count > 0)
                                    {
                                        foreach (DataRow IncomeDetailsRow in IncomeDetailsDt.Rows)
                                        {
                                            pdfpTable.AddCell(PopulatePDFCell(IncomeDetailsRow["IncomeItem"].ToString(), false, false, false));
                                            pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(IncomeDetailsRow["IncomeAmount"].ToString()).ToString("N2"), false, false, false));
                                            bool resultTryParse = false;
                                            if (bool.TryParse(IncomeDetailsRow["GrossIncome"].ToString(), out resultTryParse))
                                            {
                                                pdfpTable.AddCell(PopulatePDFCell(resultTryParse ? "[x]" : "[ ]"));
                                            }
                                            if (bool.TryParse(IncomeDetailsRow["AHGIncome"].ToString(), out resultTryParse))
                                            {
                                                pdfpTable.AddCell(PopulatePDFCell(resultTryParse ? "[x]" : "[ ]"));
                                            }
                                        }
                                    }
                                    pdfpTable.AddCell(PopulatePDFCell("", true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("Total", true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(!string.IsNullOrEmpty(IncomeRow["GrossIncome"].ToString()) ? IncomeRow["GrossIncome"].ToString() : "0").ToString("N2"), true, false, false));
                                    pdfpTable.AddCell(PopulatePDFCell("$ " + decimal.Parse(!string.IsNullOrEmpty(IncomeRow["AHGIncome"].ToString()) ? IncomeRow["AHGIncome"].ToString() : "0.00").ToString("N2"), true, false, false));
                                    pdfDoc.Add(pdfpTable);

                                }

                            }

                        }

                        #endregion

                    }
                }
            }

            #endregion



            pdfDoc.Close();
        }
        catch (Exception ex)
        {

        }

        return pdfStream;
    }

    private static PdfPCell PopulatePDFCell(string value, bool IsBold, bool IsGray, bool NoBorder)
    {
        itsFont bold = !IsBold ? FontFactory.GetFont(FontFactory.HELVETICA, 12) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        PdfPCell pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
        if (NoBorder)
            pdfpCell.Border = itsRectangle.NO_BORDER;
        pdfpCell.BackgroundColor = !IsGray ? its.text.Color.WHITE : its.text.Color.LIGHT_GRAY;
        return pdfpCell;
    }

    private static PdfPCell PopulatePDFCell(string value)
    {
        itsFont bold = FontFactory.GetFont(FontFactory.COURIER_BOLD, 12);
        PdfPCell pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));

        return pdfpCell;
    }

    private static Paragraph PopulateNotesSection(string value)
    {
        Paragraph paragraph = new Paragraph(new Chunk(value));
        paragraph.Font.Size = 12;
        return paragraph;
    }

    private static PdfPCell PopulateNotesSection(string value, bool IsBold, bool IsGray, bool IsCenter)
    {
        itsFont bold = !IsBold ? FontFactory.GetFont(FontFactory.HELVETICA, 12) : FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        PdfPCell pdfpCell = new PdfPCell(new Phrase(new Chunk(value, bold)));
        
        if (IsCenter)
            pdfpCell.HorizontalAlignment = Element.ALIGN_CENTER;        
        pdfpCell.BackgroundColor = !IsGray ? its.text.Color.WHITE : its.text.Color.LIGHT_GRAY;
        return pdfpCell;
    }


    private class IncomeWorksheet
    {
        private int _IncomeId;

        public int IncomeId
        {
            get { return _IncomeId; }
            set { _IncomeId = value; }
        }

        private string _IncomeMonthYear;

        public string IncomeMonthYear
        {
            get { return _IncomeMonthYear; }
            set { _IncomeMonthYear = value; }
        }

        private decimal _IncomeAmount;

        public decimal IncomeAmount
        {
            get { return _IncomeAmount; }
            set { _IncomeAmount = value; }
        }
        
        
        
    }
   
}
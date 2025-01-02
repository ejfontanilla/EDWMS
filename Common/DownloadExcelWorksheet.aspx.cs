
// Added By Edward Excel Worksheet on 2015/4/24
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Data;
using System.Globalization;
using Dwms.Bll;
public partial class Common_DownloadExcelWorksheet : System.Web.UI.Page
{
    public int? docAppId;
    private static string strRefNo = string.Empty;
    private static string strAssessmentDateOut = string.Empty;    

    protected void Page_Load(object sender, EventArgs e)
    {
        List<int> listIncomeId = new List<int>();
        decimal decIncomeAmount = 0;
        string strLowestIncomeAmount = string.Empty;
        int intYes = 0;
        int intNo = 0;

        if (!string.IsNullOrEmpty(Request["docAppId"]))
        {
            docAppId = int.Parse(Request["docAppId"]);
            string strFileName = "Income_Extraction_Worksheet";


            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppT = docAppDb.GetDocAppIncomeExtractionById(docAppId.Value);
            if (docAppT.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docAppT[0];
                strFileName = string.Format("{0}_{1}_{2}", docAppRow.RefNo, DateTime.Now.ToShortDateString(), strFileName);
            }

            
            DocApp.DocAppDataTable docApps = docAppDb.GetDocAppIncomeExtractionById(docAppId.Value);
            if (docApps.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppRow = docApps[0];
                strRefNo = docAppRow.RefNo;
                strAssessmentDateOut = !docAppRow.IsAssessmentDateOutNull() ? docAppRow.AssessmentDateOut.ToShortDateString() : " - ";
                Response.Write("<TABLE><TR><TD>Income Worksheet</TD></TR>");

                string refType = docAppRow.RefType.ToUpper().Trim();
                
                if (refType.Contains(ReferenceTypeEnum.HLE.ToString()))
                {
                    HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                    HleInterface.HleInterfaceDataTable hleInterfaceDt = hleInterfaceDb.GetHleInterfaceByRefNo(docAppRow.RefNo);

                    Response.Write("<TR><TD>Application Number: </TD><TD>" + docAppRow.RefNo + "</TD></TR></TABLE><BR><BR>");

                    if (hleInterfaceDt.Rows.Count > 0)
                    {
                        foreach (HleInterface.HleInterfaceRow hleInterfaceRow in hleInterfaceDt.Rows)
                        {
                            Response.Write("<TABLE border=1>");
                            int intMonthsToLeas = 0;
                            decimal decCA = 0;
                            decimal decCATotal = 0;
                            AppPersonalDb appPersonalDb = new AppPersonalDb();
                            AppPersonal.AppPersonalDataTable appPersonalDt = appPersonalDb.GetAppPersonalByNricAndDocAppId(hleInterfaceRow.Nric, docAppId.Value);
                            AppPersonal.AppPersonalRow appPersonalRow;

                            if (appPersonalDt.Rows.Count > 0)
                            {
                                appPersonalRow = appPersonalDt[0];

                                intMonthsToLeas = !appPersonalRow.IsMonthsToLEASNull() ? appPersonalRow.MonthsToLEAS : 0;

                                DataTable IncomeDt = IncomeDb.GetOrderByMonthYear(docAppId.Value, hleInterfaceRow.Nric, "DESC");
                                CreditAssessmentApplicantAndHeader(hleInterfaceRow.ApplicantType, hleInterfaceRow.OrderNo, hleInterfaceRow.Name, hleInterfaceRow.Nric, docAppId.Value);

                                CreditAssessmentPopulateMonthYear(listIncomeId, IncomeDt);

                                CreditAssessmentSetHeaders(docAppId.Value, hleInterfaceRow.Nric, intMonthsToLeas);

                                DataTable dtOrdered = new DataTable();
                                CreditAssessmentSortIncomeType(appPersonalRow.Id, out dtOrdered);
                                foreach (DataRow IncomeItemsRow in dtOrdered.Rows)
                                {
                                    Response.Write("<TR>");
                                    CreditAssessmentGetValues(IncomeItemsRow, listIncomeId, intMonthsToLeas, appPersonalRow.Id, decIncomeAmount, strLowestIncomeAmount, intYes, intNo, out decCA);
                                    decCATotal = decCATotal + decCA;
                                    Response.Write("</TR>");
                                }
                            }
                            CreditAssessmentAddCATotalRow(decCATotal, listIncomeId.Count);
                            Response.Write("</TABLE><BR><BR>");
                        }
                    }

                    

                }
            }
            
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=" + strFileName + ".xls");
        }                   
    }

    private void CreditAssessmentApplicantAndHeader(string applicantType, int orderNo, string name, string nric, int docAppId)
    {
        string strSummary = string.Empty;
        if (applicantType.Equals(PersonalTypeEnum.HA.ToString()))
            strSummary = string.Format("Applicant {0} Name: {1} ({2})", orderNo, name, nric);
        else if (applicantType.Equals(PersonalTypeEnum.OC.ToString()))
            strSummary = string.Format("Occupier {0} Name: {1} ({2})", orderNo, name, nric);
        else
            strSummary = string.Format("{0} Name: {1} ({2})", orderNo, name, nric);

        Response.Write("<TR><TD>" + strSummary + "</TD></TR>");
        
        #region Income Component and Type of Income Column Headers
        Response.Write("<TR><TD bgcolor=#AAAAAA>Income Component</TD><TD bgcolor=#AAAAAA>Type of Income</TD>");         
        #endregion

    }

    private void CreditAssessmentPopulateMonthYear(List<int> listIncomeId, DataTable IncomeDt)
    {
        listIncomeId.Clear();
        if (IncomeDt.Rows.Count > 0)
        {            
            foreach (DataRow IncomeRow in IncomeDt.Rows)
            {
                DateTimeFormatInfo info = new DateTimeFormatInfo();
                Response.Write("<TD bgcolor=#AAAAAA>" + string.Format("{0} {1}",
                    info.GetAbbreviatedMonthName(int.Parse(IncomeRow["IncomeMonth"].ToString())), IncomeRow["IncomeYear"].ToString()) + "</TD>");
                listIncomeId.Add(int.Parse(IncomeRow["Id"].ToString()));
            }
            
        }
    }

    private void CreditAssessmentSetHeaders(int docAppId, string nric, int intMonthsToLeas)
    {
        #region Set the Avg in Past 3 months, Avg in Past 12 Months, Lowest, Yes/No and Credit Assessment Column Headers
        DataTable NumberOfMonthsDt = IncomeDb.GetOrderByMonthYear(docAppId, nric, "DESC");
        Response.Write("<TD bgcolor=#AAAAAA>" + string.Format("Avg in past {0} mths", NumberOfMonthsDt.Rows.Count < 3 ? NumberOfMonthsDt.Rows.Count.ToString() : "3") + "</TD>");
        Response.Write("<TD bgcolor=#AAAAAA>" + string.Format("Avg in past {0} mths", intMonthsToLeas) + "</TD>");
        Response.Write("<TD bgcolor=#AAAAAA>" + string.Format("Lowest in past {0} mths", intMonthsToLeas) + "</TD>");
        Response.Write("<TD bgcolor=#AAAAAA>" + "Yes/No" + "</TD>");
        Response.Write("<TD bgcolor=#AAAAAA>" + "Credit Assessment" + "</TD></TR>"); 
        
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


    private  void CreditAssessmentGetValues(DataRow IncomeItemsRow, List<int> listIncomeId, int intMonthsToLeas, int appPersonalId,
        decimal decIncomeAmount, string strLowestIncomeAmount, int intYes, int intNo, out decimal decCATotal)
    {
        decCATotal = 0;
        decimal past3 = 0;
        decimal past12 = 0;
        int i = 0;
        if (!string.IsNullOrEmpty(IncomeItemsRow["IncomeType"].ToString()))
        {
            #region Get the IncomeItem and IncomeType for each Person
            Response.Write("<TD>" + IncomeItemsRow["IncomeItem"].ToString() + "</TD>");
            Response.Write("<TD>" + IncomeItemsRow["IncomeType"].ToString() + "</TD>");
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
                    Response.Write("<TD>" + Format.GetDecimalPlacesWithoutRounding(decTotalAmountOfIncomeItem / decimal.Parse(IncomeItemRow["CurrencyRate"].ToString())).ToString() + "</TD>");
                }
                else
                {
                    i++;
                    if (i == 3)
                        past3 = decIncomeAmount;
                    if (i == intMonthsToLeas)   //Modified by Edward 20/1/2014 5 Outstanding Changes. Freeze IncomeId When Zoning
                        past12 = decIncomeAmount;
                    intNo = intNo + 1;                    
                    Response.Write("<TD> - </TD>"); 
                }
            }
            #endregion

            #region  Add Average cells, Lowest, Yes/No Values

            Response.Write("<TD>" + string.Format("{0}", i < 3 ? Format.GetDecimalPlacesWithoutRounding(decIncomeAmount / i).ToString()
                : Format.GetDecimalPlacesWithoutRounding(past3 / 3).ToString()) + "</TD>");            
            Response.Write("<TD>" + string.Format("{0}", intMonthsToLeas > 0 ? Format.GetDecimalPlacesWithoutRounding(past12 / intMonthsToLeas).ToString() : " - ") + "</TD>");            
            Response.Write("<TD>" + (!string.IsNullOrEmpty(strLowestIncomeAmount) ? Format.GetDecimalPlacesWithoutRounding(decimal.Parse(strLowestIncomeAmount)).ToString() : " - ") + "</TD>");            
            Response.Write("<TD>" + string.Format("{0}/{1}", intYes, intNo) + "</TD>");
            #endregion

            #region Get the Credit Assessment
            CreditAssessmentDb CAdb = new CreditAssessmentDb();
            CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
            string IsGenerated = string.Empty;
            if (CADt.Rows.Count > 0)
            {
                CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                IsGenerated = !CARow.IsIsGeneratedNull() ? (CARow.IsGenerated ? "Y" : "N") : string.Empty;                
                PopulateCA(string.Format(Format.GetDecimalPlacesWithoutRounding(CARow.CreditAssessmentAmount).ToString()), IsGenerated);                
                decCATotal = decCATotal + CARow.CreditAssessmentAmount;
            }
            else
                //pdfpTable.AddCell(PopulatePDFCA(string.Format(""), false, false, IsGenerated));
                Response.Write("<TD>" + string.Format("") + "</TD>");

            #endregion
        }
        #region Reseting values to 0 or empty
        strLowestIncomeAmount = string.Empty;
        intYes = 0;
        intNo = 0;
        decIncomeAmount = 0;
        #endregion
    }

    private void CreditAssessmentAddCATotalRow(decimal decCATotal,  int columnCount)
    {
        columnCount = columnCount + 7;
        Response.Write("<TR>");
        for (int i = 0; i < columnCount - 2; i++)
        {            
            Response.Write("<TD></TD>");
        }        
        Response.Write("<TD>CA Total</TD>");
        Response.Write("<TD>" + Format.GetDecimalPlacesWithoutRounding(decCATotal).ToString() + "</TD>");
        Response.Write("</TR>");
    }

    private void PopulateCA(string value, string IsGenerated)
    {
        if (IsGenerated == "Y")
            Response.Write("<TD  bgcolor=#FF0000><FONT Color=#FFFFFF>" + value + "</FONT></TD>");        
        else if (IsGenerated == "N")
            Response.Write("<TD  bgcolor='CYAN'>" + value + "</TD>");        
        else        
           Response.Write("<TD></TD>");             
    }
}
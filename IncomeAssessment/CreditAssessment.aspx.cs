using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.Drawing;

public partial class IncomeAssessment_CreditAssessment : System.Web.UI.Page
{
    int appPersonalId;
    int docAppId;
    string tab = string.Empty;
    Table InfoTable = new Table();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["appPersonalId"]))
        {
            appPersonalId = int.Parse(Request["appPersonalId"].ToString());
            docAppId = int.Parse(Request["docappid"].ToString());
        }
        if (!string.IsNullOrEmpty(Request["tab"]))
            tab = Request["tab"].ToString();

      
       
            PopulateTableControl();
       
    }


    protected void Save(object sender, EventArgs e)
    {
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                    string.Format("javascript:CloseWindow('credit{0}');", tab), true);
        //Page.Validate();

        //if (Page.IsValid)
        //{
        //    try
        //    {
        //        CreditAssessmentDb db = new CreditAssessmentDb();
        //        MembershipUser user = Membership.GetUser();
        //        Guid currentUserId = (Guid)user.ProviderUserKey;
        //        InfoTable = new Table();
        //        InfoTable = (Table)Session["atbl"];
        //        foreach (TableRow tr in InfoTable.Rows)
        //        {
        //            if (tr.ID.Contains("EmployedTableRowHeading"))
        //            {
        //                string strIncomeComponent = string.Empty;
        //                string strIncomeType = string.Empty;
        //                foreach (TableCell cl in tr.Cells)
        //                {
                            
        //                    if (cl.ID.Contains("cellCA"))
        //                    {
        //                        foreach (Control ctrl in cl.Controls)
        //                        {
        //                            if (ctrl is TextBox)
        //                            {
        //                                CreditAssessmentDb CAdb = new CreditAssessmentDb();
        //                                CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, strIncomeComponent, strIncomeType);
        //                                if (CADt.Rows.Count > 0)
        //                                {
        //                                    CreditAssessment.CreditAssessmentRow CARow = CADt[0];                                            
        //                                    CAdb.Update(int.Parse(CARow["Id"].ToString()), decimal.Parse(Request.Form[((TextBox)ctrl).UniqueID].ToString()), currentUserId);
        //                                }
        //                                else                                        
        //                                    db.Insert(appPersonalId, strIncomeComponent, strIncomeType, decimal.Parse(Request.Form[((TextBox)ctrl).UniqueID].ToString()), currentUserId);                                        
        //                            }
        //                        }
        //                    }
        //                    else if (cl.ID.Contains("cellIncome"))
        //                        strIncomeComponent = cl.Text;
        //                    else if(cl.ID.Contains("cellType"))
        //                        strIncomeType = cl.Text;
        //                }
        //            }
        //        }
        //        #region Added By Edward 24/02/2014  Add Icon and Action Log
        //        IncomeDb.InsertExtractionLog(docAppId, string.Empty, string.Empty,
        //            LogActionEnum.Update_Credit_Assessment_by_REPLACE1, LogTypeEnum.E, (Guid)Membership.GetUser().ProviderUserKey);
        //        #endregion
        //        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
        //            string.Format("javascript:CloseWindow('credit{0}');", tab), true);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
            
        //}
    }

    private void PopulateTableControl()
    {
        TableRow EmployedTableRowHeading = new TableRow();
        CreateHeaderCell("Income Component", EmployedTableRowHeading);
        CreateHeaderCell("Type of Income", EmployedTableRowHeading);
        CreateHeaderCell("Credit Assessment", EmployedTableRowHeading);
        EmployedTableRowHeading.ID = "HeaderCA";
        InfoTable.Rows.Add(EmployedTableRowHeading);

       
        CreditAssessmentDb db = new CreditAssessmentDb();
        MembershipUser user = Membership.GetUser();
        Guid currentUserId = (Guid)user.ProviderUserKey;
        DataTable IncomeItemsDt = IncomeDb.GetDistinctIncomeItemByAppPersonalId(appPersonalId);

        #region Sorting the Income Types
        DataView dv = IncomeItemsDt.DefaultView;
        dv.RowFilter = "IncomeType = 'Gross Income'";
        DataTable dtOrdered = dv.ToTable();
        dv.RowFilter = "IncomeType = 'Allowance'";
        dtOrdered.Merge(dv.ToTable());
        dv.RowFilter = "IncomeType = 'Overtime'";
        dtOrdered.Merge(dv.ToTable());
        dv.RowFilter = "IncomeType = 'AHG Income'";
        dtOrdered.Merge(dv.ToTable());
        #endregion

        foreach (DataRow IncomeItemsRow in dtOrdered.Rows)
        {
            if (!string.IsNullOrEmpty(IncomeItemsRow["IncomeType"].ToString()))
            {
                EmployedTableRowHeading = new TableRow();
                EmployedTableRowHeading.ID = "EmployedTableRowHeading" + dtOrdered.Rows.IndexOf(IncomeItemsRow);
                CreateIncomeCell(IncomeItemsRow["IncomeItem"].ToString(), EmployedTableRowHeading, dtOrdered.Rows.IndexOf(IncomeItemsRow));
                CreateTypeCell(IncomeItemsRow["IncomeType"].ToString(), EmployedTableRowHeading, dtOrdered.Rows.IndexOf(IncomeItemsRow));
                CreditAssessmentDb CAdb = new CreditAssessmentDb();
                CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(appPersonalId, IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
                if (CADt.Rows.Count > 0)
                {
                    CreditAssessment.CreditAssessmentRow CARow = CADt[0];
                    CreateCreditAssessmentCell(CARow.CreditAssessmentAmount.ToString(), EmployedTableRowHeading, dtOrdered.Rows.IndexOf(IncomeItemsRow),
                        IncomeItemsRow["IncomeItem"].ToString(),IncomeItemsRow["IncomeType"].ToString());
                }
                else
                    CreateCreditAssessmentCell(string.Format(""), EmployedTableRowHeading, dtOrdered.Rows.IndexOf(IncomeItemsRow),
                        IncomeItemsRow["IncomeItem"].ToString(), IncomeItemsRow["IncomeType"].ToString());
                InfoTable.Rows.Add(EmployedTableRowHeading);
            }
        }

        InfoPanel.Controls.Add(InfoTable);
        Session["atbl"] = InfoTable;
    }

    private void CreateHeaderCell(string text, TableRow row)
    {
        TableCell cell = new TableCell();
        cell.Text = text;
        cell.BorderWidth = new Unit(1);
        //cell.Height = new Unit(20);
        cell.BackColor = Color.LightGray;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        //cell.Width = new Unit(100);
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.VerticalAlign = VerticalAlign.Middle;
        row.Cells.Add(cell);        
    }

    private void CreateTypeCell(string text, TableRow row, int rownumber)
    {
        TableCell cell = new TableCell();
        cell.ID = "cellType" + rownumber;
        cell.Text = text;
        cell.BorderWidth = new Unit(1);
        //cell.Height = new Unit(20);
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.VerticalAlign = VerticalAlign.Middle;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        row.Cells.Add(cell);
    }

    private void CreateIncomeCell(string text, TableRow row, int rownumber)
    {
        TableCell cell = new TableCell();
        cell.ID = "cellIncome" + rownumber;
        cell.Text = text;
        cell.BorderWidth = new Unit(1);
        //cell.Height = new Unit(20);
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.VerticalAlign = VerticalAlign.Middle;
        cell.BorderColor = Color.FromArgb(221, 221, 221);
        row.Cells.Add(cell);
    }

    private void CreateCreditAssessmentCell(string text, TableRow row, int rownumber, string strIncomeComponent, string strIncomeType)
    {
        MembershipUser user = Membership.GetUser();
        string currentUserId = user.UserName;        

        TableCell cell = new TableCell();
        cell.ID = "cellCA" + rownumber;
        TextBox txt = new TextBox();
        if (!IsPostBack)
            txt.Text =  text ;
        txt.ID = "txtCA" + rownumber;
        txt.Attributes.Add("onblur", string.Format("javascript:UpdateCreditAssessment(this,'{0}','{1}','{2}','{3}')", appPersonalId, 
            strIncomeComponent, strIncomeType, currentUserId));
        txt.Width = new Unit(70);
        cell.Controls.Add(txt);
        cell.BorderWidth = new Unit(1);
        //cell.Height = new Unit(20);
        cell.HorizontalAlign = HorizontalAlign.Center;
        cell.VerticalAlign = VerticalAlign.Middle;
        cell.BorderColor = Color.FromArgb(221, 221, 221);

        CustomValidator cv = new CustomValidator();
        cv.Display = ValidatorDisplay.Dynamic;
        cv.ID = "cv" + Convert.ToString(rownumber);
        cv.ControlToValidate = "txtCA" + rownumber;
        cv.ErrorMessage = "<br />Enter valid amount";
        cv.ServerValidate += AmountValidator_ServerValidate;
        cv.CssClass = "form-error";
        cell.Controls.Add(cv);

        RequiredFieldValidator rv = new RequiredFieldValidator();
        rv.ID = "rv" + rownumber;
        rv.ControlToValidate = "txtCA" + rownumber;
        rv.ErrorMessage = "<br />Enter an amount.";
        cell.Controls.Add(rv);

        row.Cells.Add(cell);
    }


    protected void AmountValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        //-------WEBsource http://stackoverflow.com/questions/5887050/how-to-extract-a-property-value-from-a-object-sender
        string name = Convert.ToString(source.GetType().GetProperty("ControlToValidate").GetValue(source, null));

        System.Reflection.PropertyInfo property = source.GetType().GetProperty("ErrorMessage");
        if (name.Contains("txtCA"))
        {
            decimal dec = 0;
            if (!decimal.TryParse(args.Value,out dec))
            {
                //--------WEBSource http://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
                property.SetValue(source, Convert.ChangeType("<br />Enter a valid amount.", property.PropertyType), null);
                args.IsValid = false;
            }
        }
    }
}
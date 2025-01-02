using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using Dwms.Bll;
using Dwms.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Generic;

/// <summary>
/// Summary description for Order_Dish
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class IncomeExtractionWebSvr : System.Web.Services.WebService
{

    public IncomeExtractionWebSvr()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }


    [WebMethod]
    public int UpdateCreditAssessment(string value, string appPersonalId, string strIncomeComponent, string strIncomeType, string strEnteredBy)
    {
        
        CreditAssessmentDb CAdb = new CreditAssessmentDb();
        CreditAssessment.CreditAssessmentDataTable CADt = CAdb.GetCAByAppPersonalIdByIncomeItemType(int.Parse(appPersonalId), strIncomeComponent, strIncomeType);
        if (CADt.Rows.Count > 0)
        {
            CreditAssessment.CreditAssessmentRow CARow = CADt[0];
            CAdb.Update(int.Parse(CARow["Id"].ToString()), decimal.Parse(value), strEnteredBy, false);
        }
        else
            CAdb.Insert(int.Parse(appPersonalId), strIncomeComponent, strIncomeType, decimal.Parse(value), strEnteredBy, false);         

        return 0;
    }


    #region Added BY Edward 2014/08/06 Changes in Zoning Page
    [WebMethod]
    public int InsertUpdateIncomeDetails(string incomeId, string incomeDetails, string versionNo, string toDeleteIncomeDetails)
    {
        #region variables
        int NewVersionNo = 0;
        int NewVersionId = 0;
        string item = string.Empty;
        decimal amount = 0;
        int incomeDetailsId = 0;
        bool gross, allowance, ot, cpf, ahg = false;
        int intIncomeId = int.Parse(incomeId);        
        #endregion

        if (!string.IsNullOrEmpty(toDeleteIncomeDetails))
        {
            string[] strToDeleteIncomeDetails = toDeleteIncomeDetails.Substring(0, toDeleteIncomeDetails.Length - 1).Split(';');
            int[] intToDeleteIncomeDetails = Array.ConvertAll<string, int>(strToDeleteIncomeDetails, int.Parse);
            IncomeDb.DeleteIncomeItems(intToDeleteIncomeDetails);
        }

        if (string.IsNullOrEmpty(incomeDetails))
            return 0;    
        string[] listAllIncomeDetails = incomeDetails.Substring(0, incomeDetails.Length - 1).Split(';');

        if (versionNo.ToUpper().Contains("NEW"))
        {
            DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(intIncomeId);
            NewVersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;            
            NewVersionId = IncomeDb.InsertIncomeVersion(intIncomeId, NewVersionNo, (Guid)Membership.GetUser().ProviderUserKey);                    
        }
        else
        {
            string[] strVersionNo = versionNo.Split('-');
            NewVersionNo = int.Parse(strVersionNo[0].Trim());
            DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncomeIdAndVersionNo(intIncomeId, NewVersionNo);
            if (dtIncomeVersion.Rows.Count > 0)
            {
                NewVersionId = int.Parse(dtIncomeVersion.Rows[0]["Id"].ToString());
                IncomeDb.UpdateIncomeVersion(NewVersionId, (Guid)Membership.GetUser().ProviderUserKey);
            }
            else
                NewVersionId = IncomeDb.InsertIncomeVersion(intIncomeId, NewVersionNo, (Guid)Membership.GetUser().ProviderUserKey);            
            //IncomeDb.DeleteAllIncomeDetailsByVersionId(NewVersionId);                   
        }
        int orderNo = 1;
        foreach (string strEachIncomeDetail in listAllIncomeDetails)
        {
            #region Assignments
            string[] str = strEachIncomeDetail.Split('|');
            item = str[0];
            amount = decimal.Parse(str[1]);
            gross = str[2] == "T" ? true : false;
            allowance = str[3] == "T" ? true : false;
            ot = str[4] == "T" ? true : false;
            ahg = str[6] == "T" ? true : false;
            cpf = str[5] == "T" ? true : false;
            incomeDetailsId = int.Parse(str[7]);
            #endregion
            if (incomeDetailsId == 0)
                IncomeDb.InsertIncomeDetails(NewVersionId, item, amount, gross, allowance, ot, ahg, cpf, orderNo);
            else
                IncomeDb.UpdateIncomeDetails(incomeDetailsId, NewVersionId, item, amount, gross, allowance, ot, ahg, cpf, orderNo);
            orderNo++;
        }     
        int IncomeVersionResult = IncomeDb.UpdateIncomeVersion(intIncomeId, NewVersionId);
        
        return 0;
    }

    //[WebMethod]
    //public int InsertIncomeVersion(string incomeId, string versionNo)
    //{
    //    int VersionNo = 0;
    //    int NewVersionId = 0;
    //    if (versionNo.ToUpper().Contains("NEW"))
    //    {
    //        DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(incomeId));
    //        VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) + 1 : 1;
    //        if (dtIncomeVersion.Rows.Count == 0)
    //            NewVersionId = IncomeDb.InsertIncomeVersion(int.Parse(incomeId), VersionNo, (Guid)Membership.GetUser().ProviderUserKey);
    //        else
    //            NewVersionId = int.Parse(dtIncomeVersion.Rows[0]["Id"].ToString());
    //    }
    //    else
    //    {

    //    }
    //    return NewVersionId;
    //}

    //[WebMethod]
    //public int GetVersionNo(string incomeId)
    //{
    //    int VersionNo = 0;
    //    DataTable dtIncomeVersion = IncomeDb.GetIncomeVersionByIncome(int.Parse(incomeId));        
    //    VersionNo = dtIncomeVersion.Rows.Count > 0 ? int.Parse(dtIncomeVersion.Rows[0]["VersionNo"].ToString()) : 1;
    //    return VersionNo;
    //}
    #endregion
}


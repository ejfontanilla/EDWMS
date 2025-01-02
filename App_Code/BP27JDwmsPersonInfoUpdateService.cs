using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.IO;
/// <summary>
/// Summary description for BP27JDwmsPersonInfoUpdateService
/// </summary>

[WebService(Namespace = "http://service.dwms.bp27.ABCDE.com.ph")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class BP27JDwmsPersonInfoUpdateService : System.Web.Services.WebService {

    public BP27JDwmsPersonInfoUpdateService () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public BP27JDwmsResultDto updatePersonInfo(BP27JDwmsAuthenticationDTO auth, BP27JDwmsCaseDTO CaseInfo)
    {
        WriteInfoToTextFile(CaseInfo);
        BP27JDwmsResultDto Results = new BP27JDwmsResultDto();
        Results.errorCode = "222";
        Results.errorMessage = "DEELTE";
        return Results;
    }


    private string WriteInfoToTextFile(BP27JDwmsCaseDTO CaseInfo)
    {
        // Temporary code to write input to text file
        string filePath = Server.MapPath("~/App_Data/WebService/" + DateTime.Now.ToString("TEMPORARY-yyyyMMdd-HHmmss.fff") + "-" + CaseInfo.numHla + ".txt");
        StreamWriter w;
        w = File.CreateText(filePath);



        w.Write("RefNumber: " + CaseInfo.numHla + "\r\n");
        w.Write("url: " + CaseInfo.docImage.url + "\r\n");
        
        w.Write("\r\n");

        foreach (BP27JDwmsPersonInfoDTO personal in CaseInfo.personDetail)
        {
            if (personal != null)
            {                
                w.Write("NRIC: " + personal.numNric + "\r\n");
                w.Write("Allowance: " + personal.amtAvgAllowance + "\r\n");                
                w.Write("Overtime: " + personal.amtAvgOvertime + "\r\n");
                w.Write("CAIncome: " + personal.amtCaIncome + "\r\n");               
                w.Write("\r\n");

                if (personal.monthlyIncome != null)
                {
                    foreach (BP27JDwmsMonthlyIncomeDTO monthlyIncome in personal.monthlyIncome)
                    {
                        w.Write("YearMonth: " + monthlyIncome.dteIncome + "\r\n");
                        w.Write(monthlyIncome.amtIncome == null ? "Amount: " : "Amount: " + monthlyIncome.amtIncome.ToString() + "\r\n");
                    }
                }

                w.Write("\r\n");

                
            }
        }

        w.Flush();
        w.Close();

        return filePath;
    }

    public class BP27JDwmsCaseDTO
    {

        public BP27JDwmsCaseDTO()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public BP27JDwmsPersonInfoDTO[] personDetail { get; set; }
        public string numHla { get; set; }
        public BP27JDwmsDocumentImageDTO docImage { get; set; }
        public string numUserId { get; set; }


    }


    public class BP27JDwmsPersonInfoDTO
    {

        public BP27JDwmsPersonInfoDTO()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public string numNric { get; set; }
        public string amtAvgAllowance { get; set; }
        public string amtAvgOvertime { get; set; }
        public string amtCaIncome { get; set; }
        public BP27JDwmsMonthlyIncomeDTO[] monthlyIncome { get; set; }

    }


    public class BP27JDwmsMonthlyIncomeDTO
    {

        public BP27JDwmsMonthlyIncomeDTO()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public string amtIncome { get; set; }
        public DateTime dteIncome { get; set; }

    }


    public class BP27JDwmsDocumentImageDTO
    {

        public BP27JDwmsDocumentImageDTO()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public string url { get; set; }

    }


    public class BP27JDwmsResultDto
    {

        public BP27JDwmsResultDto()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public string errorCode { get; set; }
        public string errorMessage { get; set; }

    }


   

    public class BP27JDwmsAuthenticationDTO
    {

        public BP27JDwmsAuthenticationDTO()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        public string password { get; set; }
        public string userName { get; set; }

    }

   

    

    
    
}




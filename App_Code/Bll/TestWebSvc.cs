using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

using Dwms.Bll;


/// <summary>
/// Summary description for TestWebSvc
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class TestWebSvc : System.Web.Services.WebService
{

    public TestWebSvc()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //[WebMethod]
    //public string HelloWorld() {
    //    return "Hello World";
    //}

    [WebMethod(EnableSession = true)]
    public bool ValidateHle(string hleNumber)
    {
        try
        {
            return Validation.IsHLENumber(hleNumber);

        }
        catch (Exception)
        {
            return false;
        }
    }

    [WebMethod(EnableSession = true)]
    public int Sum(int[] numbers)
    {
        try
        {
            int sum = 0;

            foreach (int i in numbers)
            {
                sum = sum + i;
            }

            return sum;

        }
        catch (Exception)
        {
            return 0;
        }
    }
}

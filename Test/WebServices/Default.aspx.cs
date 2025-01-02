using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Test_WebServices_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        char[] delim = { ',' };
        string[] arr = TextBox1.Text.Split(delim, StringSplitOptions.RemoveEmptyEntries);
        int[] numbers = new int[arr.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            numbers[i] = int.Parse(arr[i]);
        }

        TestWebSvcRef.TestWebSvc webService = new TestWebSvcRef.TestWebSvc();
        int sum = webService.Sum(numbers);
        Label1.Text = sum.ToString();
    }
}
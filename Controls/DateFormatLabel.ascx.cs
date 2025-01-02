using System;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;

public partial class Controls_Date : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        DateFormat.Text = "(" + Format.GetMetaDataDateFormat() + ")";
    }
}
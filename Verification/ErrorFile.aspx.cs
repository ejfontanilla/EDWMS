using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Verification_ErrorFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ErrorFile"] != null)
        {
            lbl.Text = lbl.Text + Session["ErrorFile"].ToString();
            Session["ErrorFile"] = null;
        }
    }
}
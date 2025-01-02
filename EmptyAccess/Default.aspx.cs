using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.IO;
using TallComponents.Web.Storage;
using System.Drawing;

public partial class EmptyAccess_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            EmptyAccessLabel.Text = Constants.UnathorizedEmptyAccessErrorMessage;
        }
    }
}
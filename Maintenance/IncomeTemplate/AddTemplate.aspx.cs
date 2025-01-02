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

public partial class Maintenance_IncomeTemplate_AddTemplate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ValueTextBox.Focus();
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            IncomeTemplateDb db = new IncomeTemplateDb();
             MembershipUser user = Membership.GetUser();
            Guid currentUserId = (Guid)user.ProviderUserKey;
            db.Insert(ValueTextBox.Text, currentUserId);

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                "javascript:CloseWindow('fromtemp');", true);
        }
    }
}
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

public partial class IncomeAssessment_ConfirmDeleteItems : System.Web.UI.Page
{

    int[] ids;
    //string action;            Commented by Edward 2016/02/04 take out unused variables
    protected void Page_Load(object sender, EventArgs e)
    {
        

        if (!string.IsNullOrEmpty(Request["id"].ToString()) )
        {
            string[] idStrArray = Request["id"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            ids = new int[idStrArray.Length];
            int count = 0;
            foreach (string idStr in idStrArray)
            {
                ids[count] = int.Parse(idStr);
                count++;
            }
        }

        NoOfSetsLabel.Text = string.Format("You have selected {0} items to be deleted. Please click the Confirm button to delete the items", ids.Length.ToString());
    }


    protected void btnConfirm_Click(object sender, EventArgs e)
    {

        try
        {

            int result = IncomeDb.DeleteIncomeItems(ids);

            if (result >= 0)
            {
                ConfirmPanel.Visible = true;
                FormPanel.Visible = false;
            }

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:ResizeAndClose(550, 200);", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), ex.Source.ToString(), string.Format("javascript:alert('{0}');", ex.Message.ToString()), true); ;
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:CloseWindow();", true);
        }

    }
}
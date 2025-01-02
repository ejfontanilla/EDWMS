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

public partial class Verification_AssignApp : System.Web.UI.Page
{
    int id;

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the list of App ids
        if (Request["id"] != null)
            id = int.Parse(Request["id"].ToString());
        else
        {
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "CloseWindow",
                "javascript:CloseWindow();", true);
        }
    }

    /// <summary>
    /// Assign button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void AssignButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            //update the cancellation option and remark
            DocAppDb docAppDb = new DocAppDb();
            ApplicationCancellationOptionEnum cancellationOption = (ApplicationCancellationOptionEnum)Enum.Parse(typeof(ApplicationCancellationOptionEnum), CancellationReasonList.SelectedValue, true);
            docAppDb.UpdateForCancellation(id, cancellationOption, CancellationRemark.Text);

            //update the docapp status
            docAppDb.UpdateRefStatusFromCancellation(id);

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow",
                "javascript:ResizeAndClose(700, 200);", true);
        }
    }

    #endregion
}
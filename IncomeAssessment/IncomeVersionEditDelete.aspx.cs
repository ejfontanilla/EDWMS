using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Drawing;
using iTextSharp.text.pdf;

public partial class IncomeAssessment_IncomeVersionEditDelete : System.Web.UI.Page
{
    int? VersionNo;
    string strMode;
    int? IncomeId;
    int? DocAppId;
    string NRIC;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["versionNo"]))
        {
            VersionNo = int.Parse(Request["versionNo"]);
            strMode = Request["mode"].ToString();
            IncomeId = int.Parse(Request["incomeid"]);
            DocAppId = int.Parse(Request["docAppId"]);
            NRIC = Request["nric"].ToString();
        }
        else
            Response.Redirect("~/IncomeAssessment/");

        if (strMode.ToUpper() == "EDIT")
        {
            lblDelete.Visible = false;
            lblEdit.Visible = true;
            txtVersionName.Visible = true;
            btnConfirm.Text = "Update";
            lblConfirm.Text = "Update Version successful.";
            
        }
        else if (strMode.ToUpper() == "DELETE")
        {
            lblDelete.Visible = true;
            lblEdit.Visible = false;
            txtVersionName.Visible = false;
            btnConfirm.Text = "Confirm";
            lblConfirm.Text = "Delete Version successful.";
            Page.Title = Page.Title.Replace("Edit", "Delete");
        }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        if (strMode.ToUpper() == "EDIT")
        {
            //IncomeDb.UpdateIncomeVersion(Id.Value, (Guid)Membership.GetUser().ProviderUserKey, txtVersionName.Text);
            IncomeDb.UpdateIncomeVersionByDocAppIdAndNricAndVersionNo((Guid)Membership.GetUser().ProviderUserKey, txtVersionName.Text, DocAppId.Value, NRIC, VersionNo.Value);

            #region Added By Edward  10/02/2014 Sales and Resales Changes
            IncomeDb.InsertExtractionLog(IncomeId.Value, txtVersionName.Text,string.Empty,
                LogActionEnum.Update_Version_Name_to_REPLACE2,LogTypeEnum.Z, (Guid)Membership.GetUser().ProviderUserKey);
            #endregion
        }
        else if (strMode.ToUpper() == "DELETE")
        {
            //IncomeDb.DeleteIncomeVersionById(VersionNo.Value);
            IncomeDb.DeleteIncomeVersionByDocAppIdAndNricAndVersionNo(DocAppId.Value, NRIC, VersionNo.Value);
            #region Added By Edward  10/02/2014 Sales and Resales Changes
            IncomeDb.InsertExtractionLog(IncomeId.Value, string.Empty,string.Empty,
                LogActionEnum.Delete_Version,LogTypeEnum.Z,(Guid)Membership.GetUser().ProviderUserKey);
            #endregion
        }
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeWindow", "javascript:ResizeAndClose(550, 200);", true);
    }
}
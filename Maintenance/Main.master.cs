using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Maintenance_Main : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string url = Request.Url.ToString().ToLower();

        if (url.Contains("/maintenance/ocrengine/"))
        {
            OcrHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/useraccounts/"))
        {
            UserAccountsHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/rolemanagement/"))
        {
            RolesHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/accesscontrol/"))
        {
            AccessControlHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/controlparameters/"))
        {
            ParametersHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/emailtemplates/"))
        {
            EmailTemplatesHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/imagerouting/"))
        {
            ImageRoutingHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/documenttype/"))
        {
            DocTypeHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/streettable/"))
        {
            StreetTableHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/incomeworksheet/"))
        {
            IncomeWorksheetHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/oic/"))
        {
            OicHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/reportrouting/"))
        {
            ReportRoutingHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/transferdata/"))
        {
            TransferDataHyperlink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/masterlist/"))
        {
            MasterListHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/organization/"))
        {
            OrganizationHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/importinterface/"))
        {
            ImportInterfaceHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/ocrparameters/"))
        {
            OcrParametersHyperLink.CssClass = "on";
        }
        #region Added By Edward to Fix Audit Trail Filter Tables on 2015/06/29
        else if (url.Contains("/maintenance/audittrail/setaction"))
        {
            SetActionHyperLink.CssClass = "on";
        }
        #endregion
        else if (url.Contains("/maintenance/audittrail/"))
        {
            AuditTrailHyperLink.CssClass = "on";
        }        
        else if (url.Contains("/maintenance/stopwords/"))
        {
            StopWordsHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/batchupload/"))
        {
            BatchUploadHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/incometemplate/"))
        {
            IncomeTemplateHyperLink.CssClass = "on";
        }
        else if (url.Contains("/maintenance/archivedocuments/"))
        {
            ArchiveDocumentsHyperLink.CssClass = "on";
        }
    }
}
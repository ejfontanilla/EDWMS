using System;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Dwms.Web;
using Dwms.Bll;
using System.IO;


//      How it works
//  - Get the VerificationDateIn of the Docset <= Selected date
//  - Get All documents  from the folder App_Data/ForOcr
//  - Get All documents from the folder App_Data/RawPage
//  - Get All documents from the following using acknowledgementno field
//      * if channel is email check only in App_Data/Email
//      * if channel is scan check only in App_Data/Scan
//      * if channel is fax check only in App_Data/Fax
//      * if channel is MyDoc check in App_Data/MyDoc AND App_Data/WebService
//      * if channel is MyABCDEPage check in App_Data/WebService
//      * if channel is MyABCDEPage Common Panel check in App_Data/WebService
public partial class Maintenance_ArchiveDocuments_Default : System.Web.UI.Page
{
    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ParameterDb parameterDb = new ParameterDb();
            ddlArchiveCOS.DataSource = EnumManager.EnumToDataTable(typeof(ArchiveDocumentsEnum));
            ddlArchiveCOS.DataBind();

            ddlArchiveSales.DataSource = EnumManager.EnumToDataTable(typeof(ArchiveDocumentsEnum));
            ddlArchiveSales.DataBind();

            ddlArchiveResale.DataSource = EnumManager.EnumToDataTable(typeof(ArchiveDocumentsEnum));
            ddlArchiveResale.DataBind();

            ddlArchiveSERS.DataSource = EnumManager.EnumToDataTable(typeof(ArchiveDocumentsEnum));
            ddlArchiveSERS.DataBind();

            ddlArchiveCOS.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.ArchiveCOS).Replace(" ", "_");
            ddlArchiveResale.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.ArchiveResale).Replace(" ", "_");
            ddlArchiveSales.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.ArchiveSales).Replace(" ", "_");
            ddlArchiveSERS.SelectedValue = parameterDb.GetParameter(ParameterNameEnum.ArchiveSers).Replace(" ", "_");
            ArchiveServer.Text = parameterDb.GetParameter(ParameterNameEnum.ArchiveServer);
            ArchiveTimeStart.Text = parameterDb.GetParameter(ParameterNameEnum.ArchiveTimeStart);
            ArchiveTimeFormat.Text = parameterDb.GetParameter(ParameterNameEnum.ArchiveTimeFormat);
            
        }
    }

    /// <summary>
    /// Save parameter event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Save(object sender, EventArgs e)
    {
        ParameterDb parameterDb = new ParameterDb();

        parameterDb.Update(ParameterNameEnum.ArchiveCOS, ddlArchiveCOS.SelectedValue.Replace("_", " "));
        parameterDb.Update(ParameterNameEnum.ArchiveResale, ddlArchiveResale.SelectedValue.Replace("_", " "));
        parameterDb.Update(ParameterNameEnum.ArchiveSales, ddlArchiveSales.SelectedValue.Replace("_", " "));
        parameterDb.Update(ParameterNameEnum.ArchiveSers, ddlArchiveSERS.SelectedValue.Replace("_", " "));
        parameterDb.Update(ParameterNameEnum.ArchiveServer, ArchiveServer.Text.Trim());
        parameterDb.Update(ParameterNameEnum.ArchiveTimeFormat, ArchiveTimeFormat.Text.Trim());
        parameterDb.Update(ParameterNameEnum.ArchiveTimeStart, ArchiveTimeStart.Text.Trim());
    }


    #endregion

    
}
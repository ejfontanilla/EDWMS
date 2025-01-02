using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.IO;
using Dwms.Web;
using Dwms.Bll;
using Telerik.Web.UI;

public partial class Maintenance_StreetTable_Default : System.Web.UI.Page
{
    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");

        if (System.IO.File.Exists(Server.MapPath(ConfigurationManager.AppSettings["excelFolder"] + "/" + Constants.StreetListExcelFileName)))
        {
            ExcelHyperLink.NavigateUrl = "~/Maintenance/StreetTable/ExcelTemplate.aspx";
        }

        // Set the access control for the user
        SetAccessControl();
    }

    protected void ImportButton_Click(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            string excelFilePath = SaveUploadedFile();

            if (!String.IsNullOrEmpty(excelFilePath))
            {
                // Retrieve the data inside the Excel file
                string sqlSelect = "SELECT * FROM [Sheet1$]";
                ExcelDb excelDb = new ExcelDb();
                DataTable dt = new DataTable();

                try
                {
                    dt = excelDb.GetDataTableFromExcel(excelFilePath, sqlSelect);
                }
                catch (Exception)
                {
                    Error.Visible = true;
                    Error.Text = "<br />Format of the file is invalid.  Please use the template file provided.";
                }

                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        StreetDb streetDb = new StreetDb();
                        foreach (DataRow dr in dt.Rows)
                        {
                            // Check if the current street exists
                            Street.StreetDataTable streetDt = streetDb.GetStreetByCode(dr["StreetCode"].ToString().ToUpper());

                            if (streetDt.Rows.Count <= 0)
                                streetDt = streetDb.GetStreetByName(dr["StreetDescription"].ToString().ToUpper());

                            if (streetDt.Rows.Count > 0)
                            {
                                // Update the street
                                Street.StreetRow streetDr = streetDt[0];
                                streetDb.Update(streetDr.Id, dr["StreetCode"].ToString().ToUpper(), dr["StreetDescription"].ToString().ToUpper());
                            }
                            else
                            {
                                // Append the street
                                streetDb.Insert(dr["StreetCode"].ToString().ToUpper(), dr["StreetDescription"].ToString().ToUpper());
                            }
                        }

                        Response.Redirect("Default.aspx?cfm=1");
                    }
                    catch (Exception)
                    {
                        Error.Visible = true;
                        Error.Text = "<br />Format of the file is invalid.  Please use the template file provided.";
                    }
                }
            }
        }
    }
    #endregion

    #region Validation
    protected void EmptyFileValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (ExcelRadUpload.InvalidFiles.Count == 0);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set the access control to the Maintenance functions
    /// </summary>
    private void SetAccessControl()
    {
        bool hasAccess = Util.HasAccessRights(ModuleNameEnum.Maintenance, AccessControlSettingEnum.Manage_All);

        // Set the visibility of the buttons
        SubmitPanel.Visible = hasAccess;
    }

    private string SaveUploadedFile()
    {
        string filePath = string.Empty;
        string userTempPath = Util.GetTempFolder();

        // Add escape characters to the path
        userTempPath = userTempPath.Replace(@"\", @"\\");

        // Save the uploaded files
        foreach (UploadedFile file in ExcelRadUpload.UploadedFiles)
        {
            string fileName = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8) + file.GetName();
            fileName = Path.Combine(userTempPath, fileName);
            try
            {
                // Save the image
                file.SaveAs(fileName, true);
                filePath = fileName;
                break;
            }
            catch (Exception)
            {
                filePath = string.Empty;
            }
        }

        return filePath;
    }
    #endregion
}

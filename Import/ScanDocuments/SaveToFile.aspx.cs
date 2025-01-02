using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Security;
using System.Security.AccessControl;
using Dwms.Bll;
using Dwms.Web;

public partial class Import_ScanDocuments_SaveToFile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HttpFileCollection files = HttpContext.Current.Request.Files;
        HttpPostedFile uploadfile = files["RemoteFile"];      

        MembershipUser user = Membership.GetUser();

        if (user != null)
        {
            // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
            // Save each file for each user in a folder.  Folder name is the GUID of the user
            string userTempPath = Util.GetUsersDocForOcrTempFolder();

            try
            {
                // Delete the directory to remove current files
                if (Directory.Exists(userTempPath))
                    Directory.Delete(userTempPath, true);
                else
                    throw new Exception("Requested path does not exist");
            }
            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                errorLogDb.Insert("Page_Load - After Delete Directory", ex.Message + "---1InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
                
            }

            // If the folder does not exists, create one
            try
            {
                if (!Directory.Exists(userTempPath))
                    Directory.CreateDirectory(userTempPath);
              
            }
            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                errorLogDb.Insert("Page_Load - After Create Directory", ex.Message + "---1InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
            }

            try
            {
                string fileName = "";

                // Create the filename of the PDF document 
                if (uploadfile != null)
                    fileName = Path.Combine(userTempPath, uploadfile.FileName);
                else
                    throw new Exception("File uploaded is missing.");

                // Save the image
                uploadfile.SaveAs(fileName);
            }

            catch (Exception ex)
            {
                ErrorLogDb errorLogDb = new ErrorLogDb();
                errorLogDb.Insert("Page_Load - After Upload File", ex.Message + "---1InnerException: " + ex.InnerException + "---StackTrace: " + ex.StackTrace);
            }
        }
    }
}
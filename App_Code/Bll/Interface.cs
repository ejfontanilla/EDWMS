using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Dwms.Bll;
using System.Web.Security;
using System.IO;

/// <summary>
/// Summary description for BllFuncWebSvr
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class BllFuncWebSvr : System.Web.Services.WebService {

    public BllFuncWebSvr () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(EnableSession = true)]
    public string HelloWorld() {
        return "Hello World";
    }

    /// <summary>
    /// Delete a scanned image for the current user
    /// </summary>
    /// <param name="referenceNumber"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public bool DeleteCurrentImage(int imageIndex)
    {
        try
        {
            MembershipUser user = Membership.GetUser();
            string scanTempPath = System.Web.HttpContext.Current.Request.MapPath("~/App_Data") + "/ScannedDocs/";

            if (user != null)
            {
                // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
                // Save each file for each user in a folder.  Folder name is the GUID of the user
                string userTempPath = scanTempPath + Retrieve.GetFormsAuthenticationUserData();

                // If the folder does not exists, create one
                if (Directory.Exists(userTempPath))
                {
                    string filePath = userTempPath + "/" + imageIndex + ".jpg";

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    /// <summary>
    /// UpdateSetStatus
    /// </summary>
    /// <param name="setId"></param>
    /// <param name="status"></param>
    /// <param name="logAction"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public bool UpdateSetStatusBySetIds(string setIds, string status, string logAction)
    {
        try
        {
            string[] idStrArray = setIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] ids;
            ids = new int[idStrArray.Length];
            int count = 0;
            foreach (string idStr in idStrArray)
            {
                ids[count] = int.Parse(idStr);
                count++;
            }

            DocSetDb docSetDb = new DocSetDb();

            SetStatusEnum setStatusEnum = SetStatusEnum.New;
            LogActionEnum logActionEnum = LogActionEnum.None;

            switch (status.ToLower())
            {
                case "closed" :
                    setStatusEnum = SetStatusEnum.Closed;
                    break;
                default:
                    break;
            }

            switch (logAction.ToLower())
	        {
                case "set_closed" :
                    logActionEnum = LogActionEnum.Set_closed;
                    break;
                default:
                    break;
	        }

            foreach (int id in ids)
                docSetDb.UpdateSetStatus(id, setStatusEnum, true, false, logActionEnum);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Delete all the scanned images for the current user
    /// </summary>
    /// <param name="referenceNumber"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public bool DeleteAllImages()
    {
        try
        {
            MembershipUser user = Membership.GetUser();
            string scanTempPath = System.Web.HttpContext.Current.Request.MapPath("~/App_Data") + "/ScannedDocs/";

            if (user != null)
            {
                // Get the Forms Authentication Ticket of the user to retrieve the UserData (folderName)
                // Save each file for each user in a folder.  Folder name is the GUID of the user
                string userTempPath = scanTempPath + Retrieve.GetFormsAuthenticationUserData();

                // If the folder does not exists, create one
                if (Directory.Exists(userTempPath))
                {
                    // Delete all the files inside the Directory
                    // Source: http://www.csharp-examples.net/delete-all-files/
                    string[] filePaths = Directory.GetFiles(userTempPath);

                    foreach (string filePath in filePaths)
                        File.Delete(filePath);
                }
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

      
}

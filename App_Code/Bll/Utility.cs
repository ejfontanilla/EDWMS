using System;
using System.Web;
using System.IO;

namespace Dwms.Bll
{
    public sealed class Utility
    {
        public static string GetIndividualRawPageOcrFolderPath(int rawPageId)
        {
            #region Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
            //string mainDir = System.Web.HttpContext.Current.Request.MapPath(Retrieve.GetRawPageOcrDirPath());
            //mainDir = Path.Combine(mainDir, rawPageId.ToString());            
            //return mainDir;
            #endregion

            #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY            
            DirectoryInfo dir = Dwms.Web.Util.GetIndividualRawPageOcrDirectoryInfo(rawPageId);            
            return dir.FullName;
            #endregion
        }

        public static string FormatId(string id)
        {
            return " - " + id;
        }
    }
}

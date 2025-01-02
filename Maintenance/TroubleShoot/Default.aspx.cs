using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;

public partial class Maintenance_TroubleShooting_Default : System.Web.UI.Page
{

    RawFileDb rawFileDb = new RawFileDb();

    #region Event Handlers
    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    #endregion
    protected void Submit_Click(object sender, EventArgs e)
    {
        int startSetId = int.Parse(startSetIdTextBox.Text.Trim());
        int endSetId = int.Parse(endSetIdTextBox.Text.Trim());
        DocSetDb docsetDb = new DocSetDb();
        RadGrid1.DataSource = docsetDb.GetBySetIdRange(startSetId, endSetId);
        RadGrid1.DataBind();
    }

    #region commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    //protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    //{
    //    if (e.Item is GridDataItem)
    //    {
    //        GridDataItem dataItem = (GridDataItem)e.Item;
    //        Label FileExistLabel = dataItem.FindControl("FileExistLabel") as Label;
    //        int docSetId = int.Parse(DataBinder.Eval(e.Item.DataItem, "Id").ToString());

    //        string folderPath = Path.Combine(Util.GetDocForOcrFolder(), docSetId.ToString());

    //        if (Directory.Exists(folderPath))
    //        {
    //            foreach (string rawFilePath in Directory.GetDirectories(folderPath))
    //            {
    //                string rawFileId = Path.GetFileName(rawFilePath);
    //                try
    //                {
    //                    RawFile.RawFileDataTable rawFile = rawFileDb.GetRawFileById(int.Parse(rawFileId));
    //                    FileExistLabel.Text += "RawFileFolder : " + rawFileId + " | " + (rawFile.Rows.Count > 0 ? "Exist" : "Not Exist");
    //                    FileExistLabel.Text += "<br>";
    //                }
    //                catch (Exception ex)
    //                {
    //                    FileExistLabel.Text += rawFileId;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            FileExistLabel.Text += "DocSetFolder : " + docSetId + " | " + "Not Exist";
    //            FileExistLabel.Text += "<br>";
    //        }
    //    }
    //}
    #endregion

    #region Modified by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = (GridDataItem)e.Item;
            Label FileExistLabel = dataItem.FindControl("FileExistLabel") as Label;
            int docSetId = int.Parse(DataBinder.Eval(e.Item.DataItem, "Id").ToString());

            string folderPath = string.Empty;

            DateTime datePath = new DateTime();
            if (Util.GetVerificationDateForOcrFolder(docSetId, out datePath))
                folderPath = Util.GetDocForOcrFolder(docSetId, datePath);

            if (Directory.Exists(folderPath))
            {
                foreach (string rawFilePath in Directory.GetDirectories(folderPath))
                {
                    string rawFileId = Path.GetFileName(rawFilePath);
                    try
                    {
                        RawFile.RawFileDataTable rawFile = rawFileDb.GetRawFileById(int.Parse(rawFileId));
                        FileExistLabel.Text += "RawFileFolder : " + rawFileId + " | " + (rawFile.Rows.Count > 0 ? "Exist" : "Not Exist");
                        FileExistLabel.Text += "<br>";
                    }
                    catch (Exception)
                    {
                        FileExistLabel.Text += rawFileId;
                    }
                }
            }
            else
            {
                FileExistLabel.Text += "DocSetFolder : " + docSetId + " | " + "Not Exist";
                FileExistLabel.Text += "<br>";
            }
        }
    }
    #endregion 
}

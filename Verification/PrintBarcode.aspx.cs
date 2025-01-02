using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;
using Limilabs.Barcode;

public partial class Verification_Barcode : System.Web.UI.Page
{
    public string barcodeParam = string.Empty;
    public string filename = string.Empty;
    public int? setAppId;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["setappid"]))
        {
            setAppId = int.Parse(Request["setappid"]);
        }
        else
            Response.Redirect("~/Verification/");

        if (!string.IsNullOrEmpty(Request["filename"]))
        {
            filename = Request["filename"];
        }
        else
            Response.Redirect("~/Verification/");
        

        if (!IsPostBack)
        {
            string saveDir = Util.GetTempFolder();
            BacodeImage.ImageUrl = "../Common/BarcodeImageHandler.ashx?filename=" + filename;

            PrintHelper.PrintWebControl(BacodeImage);

            #region old code not to delete
            //SetAppDb setAppDb = new SetAppDb();
            //SetApp.SetAppDataTable setApp = setAppDb.GetSetAppById(setAppId.Value);

            //if (setApp.Rows.Count > 0)
            //{
            //    SetApp.SetAppRow setAppRow = setApp[0];

            //    DocAppDb docAppDb = new DocAppDb();
            //    DocApp.DocAppDataTable docApp = docAppDb.GetDocAppById(setAppRow.DocAppId);
            //    DocApp.DocAppRow docAppRow = docApp[0];

            //    //// save barcode image into your system
            //    string saveDir = Util.GetTempFolder();
            //    string barcodeFileName = Path.Combine(saveDir, docAppRow.RefNo + ".png");

            //    BaseBarcode barcode = BarcodeFactory.GetBarcode(Symbology.Code128);
            //    barcode.FontStyle = FontStyleType.Bold;
            //    barcode.ForeColor = Color.Black;
            //    barcode.Number = docAppRow.RefNo;
            //    barcode.ChecksumAdd = false;
            //    barcode.ChecksumVisible = false;
            //    barcode.FontHeight = 0.27;
            //    barcode.CustomText = docAppRow.RefType + " : " + docAppRow.RefNo + "\n" + Format.FormatDateTime(DateTime.Now, DateTimeFormat.d__MMM__yyyy_C_h_Col_mm__tt);
            //    barcode.Height = 120;

            //    //save it to file:
            //    barcode.Save(barcodeFileName, ImageType.Png);

            //    //crop the image. remove the watermark
            //    Bitmap cropBmp = null;
            //    System.Drawing.Image image = System.Drawing.Image.FromFile(barcodeFileName);
            //    System.Drawing.Rectangle cropRect = new System.Drawing.Rectangle(0, 50, image.Width, 70);

            //    try
            //    {
            //        Bitmap bmp = image as Bitmap;
            //        // Check if it is a bitmap:
            //        if (bmp == null)
            //            throw new ArgumentException("No valid bitmap");

            //        // Crop the image:
            //        cropBmp = bmp.Clone(cropRect, bmp.PixelFormat);

            //        // Release the resources:
            //        image.Dispose();

            //        cropBmp.Save(barcodeFileName);
                    
            //        //Bitmap bm = new Bitmap(barcodeFileName);
            //        //bm.Save(Response.OutputStream, ImageFormat.Png);
                    
            //        //bm.Dispose();
            //        cropBmp.Dispose();
            //    }
            //    catch (Exception ex)
            //    {
            //        Response.Write(ex.Message);
            //    }
            //    finally
            //    {
            //        if (image != null)
            //            image.Dispose();

            //        if (cropBmp != null)
            //            cropBmp.Dispose();
            //    }
            //}
            //else
            //{
            //    Response.Write("No SetApp with the id: " + setAppId.Value.ToString() + " exist");
            //}

            #endregion
        }
    }
}
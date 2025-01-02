using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Dwms.Bll
{
    public class ImageManager
    {
        const double THUMB_WIDTH = 160;
        const double THUMB_HEIGHT = 160;

        public static byte[] Resize(byte[] fileBytes, int maxEdgeLength)
        {
            Stream imageStream = new MemoryStream(fileBytes);
            ImageInfo imageInfo = new ImageInfo();
            imageInfo.Load(imageStream);
            int imageWidth = imageInfo.Width;
            int imageHeight = imageInfo.Height;

            decimal ratio;

            if (imageWidth > imageHeight)
            {
                ratio = imageWidth < maxEdgeLength ? 1 : maxEdgeLength / (decimal)imageWidth;
            }
            else
            {
                ratio = imageHeight < maxEdgeLength ? 1 : maxEdgeLength / (decimal)imageHeight;
            }

            int thumbnailWidth = (int)((decimal)imageWidth * ratio);
            int thumbnailHeight = (int)((decimal)imageHeight * ratio);


            byte[] thumbnailBytes = null;

            try
            {
                System.Drawing.Image fullSizeImg = System.Drawing.Image.FromStream(imageStream);
                Bitmap bitmap = new Bitmap(fullSizeImg, thumbnailWidth, thumbnailHeight);
                ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters Params = new EncoderParameters(1);
                Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, Info[1], Params);
                fullSizeImg.Dispose();
                bitmap.Dispose();
                thumbnailBytes = stream.ToArray();
            }
            catch (Exception)
            {
                // Console.Write("An error occurred - " + ex.ToString());
            }

            return thumbnailBytes;
        }

        public static byte[] Resize(byte[] fileBytes)
        {
            Stream imageStream = new MemoryStream(fileBytes);
            ImageInfo imageInfo = new ImageInfo();
            imageInfo.Load(imageStream);
            int imageWidth = imageInfo.Width;
            int imageHeight = imageInfo.Height;

            double ratio;

            if (imageWidth > imageHeight)
            {
                ratio = imageWidth < THUMB_WIDTH ? 1 : THUMB_WIDTH / (double)imageWidth;
            }
            else
            {
                ratio = imageHeight < THUMB_HEIGHT ? 1 : THUMB_HEIGHT / (double)imageHeight;
            }

            int thumbnailWidth = (int)((double)imageWidth * ratio);
            int thumbnailHeight = (int)((double)imageHeight * ratio);


            byte[] thumbnailBytes = null;

            try
            {
                System.Drawing.Image fullSizeImg = System.Drawing.Image.FromStream(imageStream);
                Bitmap bitmap = new Bitmap(fullSizeImg, thumbnailWidth, thumbnailHeight);
                ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                EncoderParameters Params = new EncoderParameters(1);
                Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, Info[1], Params);
                fullSizeImg.Dispose();
                bitmap.Dispose();
                thumbnailBytes = stream.ToArray();
            }
            catch (Exception)
            {
                // Console.Write("An error occurred - " + ex.ToString());
            }

            return thumbnailBytes;
        }

        public static byte[] Resize(string filePath)
        {
            ImageInfo imageInfo = new ImageInfo();
            imageInfo.Load(filePath);
            int imageWidth = imageInfo.Width;
            int imageHeight = imageInfo.Height;

            double ratio;

            if (imageWidth > imageHeight)
            {
                ratio = imageWidth < THUMB_WIDTH ? 1 : THUMB_WIDTH / (double)imageWidth;
            }
            else
            {
                ratio = imageHeight < THUMB_HEIGHT ? 1 : THUMB_HEIGHT / (double)imageHeight;
            }

            int thumbnailWidth = (int)((double)imageWidth * ratio);
            int thumbnailHeight = (int)((double)imageHeight * ratio);


            byte[] thumbnailBytes = null;

            try
            {
                //using (System.Drawing.Image fullSizeImg = System.Drawing.Image.FromFile(filePath))
                #region 20170905 Updated By Edward Use FromStream for Out of memory                     
                string strPhoto = (filePath);
                FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);
                using (System.Drawing.Image fullSizeImg = System.Drawing.Image.FromStream(fs))
                #endregion
                {
                    Bitmap bitmap = new Bitmap(fullSizeImg, thumbnailWidth, thumbnailHeight);
                    ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();
                    EncoderParameters Params = new EncoderParameters(1);
                    Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                    MemoryStream stream = new MemoryStream();
                    bitmap.Save(stream, Info[1], Params);
                    fullSizeImg.Dispose();
                    bitmap.Dispose();
                    thumbnailBytes = stream.ToArray();
                }
            }
            catch (Exception)
            {
                // Console.Write("An error occurred - " + ex.ToString());
            }

            return thumbnailBytes;
        }

        public static string Resizes(string filePath)
        {
            ImageInfo imageInfo = new ImageInfo();
            imageInfo.Load(filePath);
            int imageWidth = imageInfo.Width;
            int imageHeight = imageInfo.Height;

            double ratio;

            if (imageWidth > imageHeight)
            {
                ratio = imageWidth < THUMB_WIDTH ? 1 : THUMB_WIDTH / (double)imageWidth;
            }
            else
            {
                ratio = imageHeight < THUMB_HEIGHT ? 1 : THUMB_HEIGHT / (double)imageHeight;
            }

            int thumbnailWidth = (int)((double)imageWidth * ratio);
            int thumbnailHeight = (int)((double)imageHeight * ratio);

            string thumbnailFilePath = string.Empty;

            try
            {
                //using (System.Drawing.Image fullSizeImg = System.Drawing.Image.FromFile(filePath))
                #region 20170905 Updated By Edward Use FromStream for Out of memory
                string strPhoto = (filePath);
                FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);
                using (System.Drawing.Image fullSizeImg = System.Drawing.Image.FromStream(fs))
                #endregion
                {
                    using (Bitmap bitmap = new Bitmap(fullSizeImg, thumbnailWidth, thumbnailHeight))
                    {
                        ImageCodecInfo[] Info = ImageCodecInfo.GetImageEncoders();

                        using (EncoderParameters Params = new EncoderParameters(1))
                        {
                            Params.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);

                            using (MemoryStream stream = new MemoryStream())
                            {
                                bitmap.Save(stream, Info[1], Params);
                                thumbnailFilePath = filePath + "_th.jpg";
                                bitmap.Save(thumbnailFilePath, Info[1], Params);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                imageInfo.Dispose();
            }

            return thumbnailFilePath;
        }

        public static void Resize(string filePath, int width, int height)
        {
            ThumbnailManager thumbnailManager = new ThumbnailManager();
            System.Drawing.Bitmap bm = thumbnailManager.GetThumbnail(filePath, 113, 160);
            bm.Save(filePath + "_th.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
    }
}
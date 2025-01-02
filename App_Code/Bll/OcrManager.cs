using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquaforest.OCR.Api;
using System.IO;
using System.Drawing;
using System.Web;
using Dwms.Web;
using System.Diagnostics;
using Aquaforest.OCR.Definitions;
using Dwms.Dal;
using iTextSharp.text.pdf;

namespace Dwms.Bll
{
    public class OcrManager
    {
        /// <summary>
        /// Members
        /// </summary>
        private PreProcessor preProcessor;
        private Ocr ocr;

        private string sourceFilePath;
        private int setId;
        private int binarize;
        private int bgFactor;
        private int fgFactor;
        private int quality;
        private string morph;
        private bool dotMatrix;
        private int despeckle;

        //private bool ocrDone;
        private byte ocrRotate;

        private string ocrText;

        public string OcrText
        {
            get { return ocrText; }
            set { ocrText = value; }
        }

        public byte GetRotation()
        {
            return ocrRotate;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventLog"></param>
        public OcrManager(string sourceFilePath, int? setId, int binarize, int bgFactor, int fgFactor, int quality, string morph, bool dotMatrix, int despeckle)
        {
            preProcessor = new PreProcessor();
            ocr = new Ocr();

            this.sourceFilePath = sourceFilePath;
            if (setId != null) this.setId = (int)setId;
            this.binarize = binarize;
            this.bgFactor = bgFactor;
            this.fgFactor = fgFactor;
            this.quality = quality;
            this.morph = morph;
            this.dotMatrix = dotMatrix;
            this.despeckle = despeckle;
            //this.ocrDone = false;

            InitiateOcrEngine();
        }

        /// <summary>
        /// Initiative OCR Engine
        /// </summary>
        private void InitiateOcrEngine()
        {
            // Settings
            preProcessor.Autorotate = false; //true (in OCR service)
            //preProcessor.Deskew = true;
            preProcessor.NoPictures = true;
            preProcessor.Tables = true;
            preProcessor.RemoveLines = true;
            //preProcessor.MRC = true;
            preProcessor.Despeckle = despeckle;
            //preProcessor.Binarize = binarize;
            //preProcessor.Morph = morph;
            preProcessor.MRCBackgroundFactor = bgFactor;
            preProcessor.MRCForegroundFactor = fgFactor;
            //preProcessor.MRCQuality = quality;

            string resourceFolder = @"C:\Aquaforest\OCRSDK\bin";

            // Add the resource folder to the environment variable.
            // But only add if the variable has not yet been added.
            string environmentPathVariable = System.Environment.GetEnvironmentVariable("PATH");

            if (!environmentPathVariable.ToUpper().Contains(resourceFolder.ToUpper()))
                System.Environment.SetEnvironmentVariable("PATH", System.Environment.GetEnvironmentVariable("PATH") + ";" + resourceFolder);

            //ocr.Dotmatrix = dotMatrix;
            ocr.ResourceFolder = resourceFolder;
            ocr.License = Util.GetOcrLicense();
            ocr.EnablePdfOutput = true;
            ocr.EnableTextOutput = true;
            ocr.Language = SupportedLanguages.English;
            ocr.RemoveExistingPDFText = false;
            //ocr.OptimiseOcr = true;
            //ocr.StatusUpdate += OcrStatusUpdate;

            string tempFolder = Path.Combine(HttpContext.Current.Server.MapPath(Retrieve.GetTempDirPath()), Guid.NewGuid().ToString()); // multiple thread need a different temporary folder for each thread
            ocr.TempFolder = tempFolder;
            

            //ocr.PageCompleted += OcrPageCompleted;
        }

        public bool GetOcrText(out string ocrText, out string errorReason, out string errorException)
        {
            bool result = false;

            ocrText = string.Empty;
            errorReason = string.Empty;
            errorException = string.Empty;

            string fileName = sourceFilePath.ToLower();
            Image image = null;
            try
            {
                result = true;

                // Set up the OCR engine
                if (fileName.EndsWith(".pdf")) 
                    ocr.ReadPDFSource(sourceFilePath);
                else if (fileName.EndsWith(".tif") || fileName.EndsWith(".tiff")) 
                    ocr.ReadTIFFSource(sourceFilePath);
                else if (fileName.EndsWith(".bmp")) 
                    ocr.ReadBMPSource(sourceFilePath);
                else if (fileName.EndsWith(".png") || fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".gif"))
                {

                    #region 20170905 Updated By Edward Use FromStream for Out of memory
                    //using (image = Image.FromFile(fileName))//causing out of memory issue
                    string strPhoto = (fileName);
                    FileStream fs = new FileStream(strPhoto, FileMode.Open, FileAccess.ReadWrite);                                                            
                    using (image = Image.FromStream(fs))
                    #endregion
                    { 
                        ocr.ReadImageSource(image);
                    }
                }
                else
                    result = false;

                if (result)
                {
                    // Carry out the OCR processing
                    if (ocr.Recognize(preProcessor))
                    {
                        //while (!this.ocrDone)
                        //    continue;

                        //if (this.ocrDone)
                        //{
                        // Return the OCR text
                        if (ocr.NumberPages > 0)
                        {
                            try
                            {
                                ocrText = ocr.ReadPageString(1);
                            }
                            catch (Exception)
                            {
                                ocrText = string.Empty;
                                errorReason += "<br/>" + "OCR Read Page String Error";
                            }
                        }

                        // Save the searchable PDF file
                        string pdfPath = sourceFilePath + "_s.pdf";
                        ocr.SavePDFOutput(pdfPath, true);
                        if (fileName.EndsWith(".pdf"))
                            if (File.Exists(sourceFilePath + "_tmp.jpg_th.jpg"))
                                File.Delete(sourceFilePath + "_tmp.jpg_th.jpg");
                        else
                            if (File.Exists(sourceFilePath + "_th.jpg"))
                                File.Delete(sourceFilePath + "_th.jpg");
                        try
                        {
                            // Create the thumbnail file
                            //ImageManager.Resize(newRawPageTempPath, 113, 160);
                            string tempImagePath = Util.SaveAsJpegThumbnailImage(pdfPath);
                            string thumbNailPath = ImageManager.Resizes(tempImagePath);

                            try
                            {
                                File.Delete(tempImagePath);
                            }
                            catch
                            {
                                errorReason += "<br/>" + "Delete Temp Image Path Error";
                            }
                        }
                        catch (Exception)
                        {
                            errorReason += "<br/>" + "Creating Thumbnail Error";
                        }

                        //}
                    }
                }
            }
            catch (Exception e)
            {
                // Log the error in the windows service log
                string errorSummary = string.Format("Warning (OcrManager.GetOcrText): File={0}, Message={1}, StackTrace={2}"
                    , sourceFilePath, e.Message, e.StackTrace);

                errorReason = "OCR of file failed.";
                errorException = String.Format("File={0}, Message={1}",
                    (sourceFilePath.Contains("\\") ? sourceFilePath.Substring(sourceFilePath.LastIndexOf("\\") + 1) : sourceFilePath),
                    e.Message);

                result = false;
            }
            finally
            {
                if (image != null)
                    image.Dispose();

                // Delete the temporary files
                ocr.DeleteTemporaryFiles();
            }

            return result;
        }

        public void Dispose()
        {
            if (ocr != null)
            {
                ocr.Dispose();
                ocr = null;
            }
        }

        private void OcrStatusUpdate(object sender, StatusUpdateEventArgs statusUpdateEventArgs)
        {
            //this.ocrDone = true;
            this.ocrRotate = (byte)statusUpdateEventArgs.Rotation;
        }

        private void OcrPageCompleted(int pageNumber, bool textAvailable, bool imageAvailable, bool blankPage)
        {
            //this.ocrDone = true;
        }
    }
}

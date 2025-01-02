using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.IO;
using Dwms.Dal;
using RawPageTableAdapters;
using System.Collections;
using Dwms.Web;

namespace Dwms.Bll
{
    public class RawPageDb
    {
        private RawPageTableAdapter _RawPageTableAdapter = null;

        protected RawPageTableAdapter Adapter
        {
            get
            {
                if (_RawPageTableAdapter == null)
                    _RawPageTableAdapter = new RawPageTableAdapter();

                return _RawPageTableAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Get the document sets
        /// </summary>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPages()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get the raw pages by raw file id
        /// </summary>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPage(int id)
        {
            return Adapter.GetDataById(id);
        }

        /// <summary>
        /// Get the raw pages by id
        /// </summary>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPageById(int id)
        {
            return Adapter.GetDataById(id);
        }

        /// <summary>
        /// Get pages of the raw file
        /// </summary>
        /// <param name="rawFileId"></param>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPageByRawFileId(int rawFileId)
        {
            return Adapter.GetDataByRawFileId(rawFileId);
        }


        public int CountOcrPagesBySet(int docSetId)
        {
            return RawPageDs.CountOcrPagesBySet(docSetId);
        }

        /// <summary>
        /// Get pages by set id
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        public DataTable GetRawPagesBySetId(int setId)
        {
            return RawPageDs.GetRawPagesBySetId(setId);
        }

        /// <summary>
        /// Get pages by doc id
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPageByDocId(int docId)
        {
            return Adapter.GetDataByDocId(docId);
        }

        public int CountRawPageByDocId(int docId)
        {
            return GetRawPageByDocId(docId).Rows.Count;
        }

        /// <summary>
        /// Get Max DocPageNo for a given document Id
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int GetMaxDocPageNo(int docId)
        {
            return (int)Adapter.GetMaxDocPageNo(docId);
        }

        /// <summary>
        /// Get the minimum doc page no
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int GetMinDocPageNo(int docId)
        {
            return (int)Adapter.GetMinDocPageNo(docId);
        }

        /// <summary>
        /// Get the maximum raw page no of the document
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int GetMaxRawPageNoOfDoc(int docId)
        {
            return (int)Adapter.GetMaxRawPageNoOfDoc(docId);
        }

        /// <summary>
        /// Get the minimum raw page no of the document
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public int GetMinRawPageNoOfDoc(int docId)
        {
            return (int)Adapter.GetMinRawPageNoOfDoc(docId);
        }

        /// <summary>
        /// Get doc page no of the raw page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetDocPageNo(int id)
        {
            int result = -1;

            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count > 0)
            {
                RawPage.RawPageRow dr = dt[0];
                result = dr.DocPageNo;
            }

            return result;
        }

        /// <summary>
        /// Get the raw pages sort by raw page no
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public RawPage.RawPageDataTable GetRawPageByDocIdSortByRawPageNo(int docId)
        {
            return Adapter.GetDataByDocIdRawPageNoSort(docId);
        }

        /// <summary>
        /// Get the raw file ids of the document
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public ArrayList GetRawFileIds(int docId)
        {
            ArrayList result = new ArrayList();

            RawPage.RawPageDataTable dt = GetRawPageByDocId(docId);

            foreach (RawPage.RawPageRow dr in dt.Rows)
            {
                if (!result.Contains(dr.RawFileId))
                    result.Add(dr.RawFileId);
            }

            return result;
        }

        public int GetSetIdByRawPageId(int id)
        {
            return RawPageDs.GetSetIdByRawPageId(id);
        }

        public int GetRawFileIdByRawPageId(int id)
        {
            int result = -1;

            RawPage.RawPageDataTable dt = GetRawPage(id);

            if (dt.Rows.Count > 0)
            {
                RawPage.RawPageRow dr = dt[0];
                result = dr.RawFileId;
            }

            return result;
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="RawPageId"></param>
        /// <param name="rawPageNo"></param>
        /// <param name="pageData"></param>
        /// <param name="ocrText"></param>
        /// <returns></returns>
        public int Insert(int rawFileId, int rawPageNo, byte[] pageData, string ocrText, byte[] imagePageData, byte[] searchablePdfData, bool isOcr)
        {
            RawPage.RawPageDataTable dt = new RawPage.RawPageDataTable();
            RawPage.RawPageRow r = dt.NewRawPageRow();

            r.RawFileId = rawFileId;
            r.RawPageNo = rawPageNo;
            r.PageData = pageData;
            r.OcrText = ocrText;
            r.DocPageNo = 0;
            r.ImagePageData = imagePageData;
            r.SearchablePdf = searchablePdfData;
            r.IsOcr = isOcr;
            
            dt.AddRawPageRow(r);
            Adapter.Update(dt);
            int rowAffected = r.Id;
            return rowAffected;
        }

        /// <summary>
        /// Copy from old doc to new doc with new rawfileid 
        /// </summary>
        /// <param name="oldDocId"></param>
        /// <param name="newDocId"></param>
        /// <param name="newRawFileId"></param>
        /// <returns></returns>
        public Boolean InsertFromOld(int oldDocId, int newDocId, Dictionary<int, int> oldToNewRawFileId, Boolean copyPhysicalFiles)
        {
            RawPageDb rawPageDb = new RawPageDb();
            RawPage.RawPageDataTable oldRawpages = rawPageDb.GetRawPageByDocId(oldDocId);

            int id = -1;

            foreach (RawPage.RawPageRow oldRawPageRow in oldRawpages)
            {
                RawPage.RawPageDataTable newRawPages = new RawPage.RawPageDataTable();
                RawPage.RawPageRow newRawPageRow = newRawPages.NewRawPageRow();

                newRawPageRow.RawFileId = oldToNewRawFileId[oldRawPageRow.RawFileId];
                newRawPageRow.RawPageNo = oldRawPageRow.RawPageNo;
                newRawPageRow.PageData = oldRawPageRow.PageData;
                newRawPageRow.OcrText = oldRawPageRow.OcrText;
                newRawPageRow.DocId = newDocId;
                newRawPageRow.DocPageNo = oldRawPageRow.DocPageNo;

                if (!oldRawPageRow.IsImagePageDataNull())
                    newRawPageRow.ImagePageData = oldRawPageRow.ImagePageData;

                if (!oldRawPageRow.IsSearchablePdfNull())
                    newRawPageRow.SearchablePdf = oldRawPageRow.SearchablePdf;

                if (!oldRawPageRow.IsIsOcrNull())
                    newRawPageRow.IsOcr = oldRawPageRow.IsOcr;

                newRawPageRow.OcrFailed = oldRawPageRow.OcrFailed;

                newRawPages.AddRawPageRow(newRawPageRow);
                Adapter.Update(newRawPages);

                id = newRawPageRow.Id;

                if (id > 0)
                {
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.RawPage, id.ToString(), OperationTypeEnum.Insert);
                }

                if (copyPhysicalFiles)
                {
                    //get source path
                    string sourcePath = Utility.GetIndividualRawPageOcrFolderPath(oldRawPageRow.Id);

                    //get destination path
                    string desPath = Utility.GetIndividualRawPageOcrFolderPath(id);

                    //copy the folder contents.
                    //create destination directory  
                    if (!Directory.Exists(desPath))
                        Directory.CreateDirectory(desPath);

                    String[] rawFiles;
                    rawFiles = Directory.GetFileSystemEntries(sourcePath);

                    foreach (string rawFile in rawFiles)
                    {
                        File.Copy(rawFile, desPath + "\\" + Path.GetFileName(rawFile), true);
                    }
                }
            }

            return id > 0;
        }


        #endregion

        #region Update Methods

        /// <summary>
        /// Update the raw page with autorotated value
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ocrText"></param>
        /// <param name="isOcr"></param>
        /// <param name="autoRotated"></param>
        /// <returns></returns>
        public bool Update(int id, string ocrText, bool isOcr, byte autoRotated)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.OcrText = ocrText;
            r.IsOcr = isOcr;
            r.AutoRotated = autoRotated;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateCode"></param>
        /// <param name="templateDescription"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Update(int id, int docId, int docPageNo)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.DocId = docId;
            r.DocPageNo = docPageNo;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Update the raw page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imagePageData"></param>
        /// <param name="searchablePdfData"></param>
        /// <param name="isOcr"></param>
        /// <returns></returns>
        public bool Update(int id, string ocrText, byte[] imagePageData, byte[] searchablePdfData, bool isOcr)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.OcrText = ocrText;
            r.ImagePageData = imagePageData;
            r.SearchablePdf = searchablePdfData;
            r.IsOcr = isOcr;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        public bool Update(int id, bool isOcr)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.IsOcr = isOcr;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Update set OcrFailed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ocrFailed"></param>
        /// <returns></returns>
        public bool UpdateOcrFailed(int id, bool ocrFailed)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.OcrFailed = ocrFailed;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }


        /// <summary>
        /// Update the doc page no of the raw page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docPageNo"></param>
        /// <returns></returns>
        public bool UpdateDocPageNo(int id, int docPageNo)
        {
            RawPage.RawPageDataTable dt = GetRawPageById(id);

            if (dt.Rows.Count == 0) return false;

            RawPage.RawPageRow r = dt[0];

            r.DocPageNo = docPageNo;

            int rowsAffected = Adapter.Update(dt);

            return (rowsAffected > 0);
        }

        /// <summary>
        /// Update the doc page nos of the document between the start and end page nos
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="startPageNo"></param>
        /// <param name="endPageNo"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool UpdateDocPageNos(int docId, int startPageNo, int endPageNo, bool asc)
        {
            if (asc)
                return Adapter.UpdatePageNoOnReorderAsc(docId, startPageNo, endPageNo) > 0;
            else
                return Adapter.UpdatePageNoOnReorderDesc(startPageNo, endPageNo, docId) > 0;
        }

        /// <summary>
        /// Update the doc page nos of the document
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="startPageNo"></param>
        /// <param name="endPageNo"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool UpdateDocPageNosIncrement(int docId)
        {
            return Adapter.UpdateIncrementDocPageNos(docId) > 0;
        }

        /// <summary>
        /// Update the doc pages nos of the pages with doc pange nos greater than the startPageNo
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="startPageNo"></param>
        /// <returns></returns>
        public bool UpdateDocPageNosFromStartPageNo(int docId, int startPageNo)
        {
            return Adapter.UpdateDocPageNosFromStartPage(startPageNo, docId) > 0;
        }
        #endregion

        #region Delete Methods
        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }
        #endregion

        #region support
        /// <summary>
        /// Reorder doc page numbers
        /// </summary>
        /// <param name="rawPagesToMoveDt"></param>
        /// <param name="docCopyId"></param>
        /// <returns></returns>
        public static int ReorderRawPages(RawPage.RawPageDataTable rawPagesToMoveDt, int desDocId)
        {
            RawPageDb rawPageDb = new RawPageDb();
            int docPageNo = 1;
            ArrayList rawFileIds = rawPageDb.GetRawFileIds(desDocId);

            foreach (RawPage.RawPageRow rawPageToMove in rawPagesToMoveDt.Rows)
            {
                int maxRawPageNo = rawPageDb.GetMaxRawPageNoOfDoc(desDocId);
                int minRawPageNo = rawPageDb.GetMinRawPageNoOfDoc(desDocId);

                int minPageOrder = rawPageDb.GetMinDocPageNo(desDocId);
                int maxPageOrder = rawPageDb.GetMaxDocPageNo(desDocId);

                // If the destination and the source have the same raw files,
                // use the raw page no to sort.
                // Else, add the page as the last page of the document
                //if(rawFileIds.Count > 0)
                //{
                //if (rawFileIds.Contains(rawPageToMove.RawFileId))
                //{
                //int rawPageId = rawPageToMove.Id;

                //// Get the RawPage data                        
                //RawPage.RawPageDataTable rawPageDt = rawPageDb.GetRawPageById(rawPageId);
                //
                //if (rawPageDt.Rows.Count > 0)
                //{
                //    RawPage.RawPageRow rawPageToMove = rawPageDt[0];

                    if (maxRawPageNo == 0 && minRawPageNo == 0)
                    {
                        // Update the RawPage to be assigned to the new Doc record
                        rawPageDb.Update(rawPageToMove.Id, desDocId, 1);
                        docPageNo = rawPageToMove.DocPageNo;
                    }
                    else if (rawPageToMove.RawPageNo < minRawPageNo)
                    {
                        // Update the current raw page of the document by
                        // setting the doc page to + 1
                        bool result = rawPageDb.UpdateDocPageNosIncrement(desDocId);

                        if (result)
                        {
                            // Update the RawPage to be assigned to the new Doc record
                            rawPageDb.Update(rawPageToMove.Id, desDocId, minPageOrder);
                            docPageNo = rawPageToMove.DocPageNo;
                        }
                    }
                    else if (rawPageToMove.RawPageNo > maxRawPageNo)
                    {
                        // Update the RawPage to be assigned to the new Doc record
                        rawPageDb.Update(rawPageToMove.Id, desDocId, maxPageOrder + 1);
                        docPageNo = rawPageToMove.DocPageNo;
                    }
                    else
                    {
                        // Get the raw pages of the document, order by raw page no
                        RawPage.RawPageDataTable rawPagesSortByRawPageNoDt = rawPageDb.GetRawPageByDocIdSortByRawPageNo(desDocId);

                        int pageToReplaceDocPageNo = -1;

                        // Find the page to replace with regard to the order
                        foreach (RawPage.RawPageRow rawPageSortByRawPageNo in rawPagesSortByRawPageNoDt.Rows)
                        {
                            if (rawPageToMove.RawPageNo <= rawPageSortByRawPageNo.RawPageNo)
                            {
                                pageToReplaceDocPageNo = rawPageSortByRawPageNo.DocPageNo;
                                break;
                            }
                        }

                        if (pageToReplaceDocPageNo > 0)
                        {
                            // Update the pages (set doc page no = +1) with doc page no greater than the page no
                            // of the page found in the loop above
                            bool result = rawPageDb.UpdateDocPageNosFromStartPageNo(desDocId, pageToReplaceDocPageNo);

                            if (result)
                            {
                                // Update the RawPage to be assigned to the new Doc record
                                rawPageDb.Update(rawPageToMove.Id, desDocId, pageToReplaceDocPageNo);
                                docPageNo = rawPageToMove.DocPageNo;
                            }
                        }
                    }
                //}
                //}
                //else
                //{
                //    // Update the RawPage to be assigned to the new Doc record
                //    rawPageDb.Update(rawPageToMove.Id, docCopyId, maxPageOrder + 1);
                //    docPageNo = rawPageToMove.DocPageNo;
                //}
            }

            return docPageNo;
        }


        /// <summary>
        /// Checks if all the rawfiles are still attached to a particular document.
        /// </summary>
        /// <param name="rawPageIds"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public bool IsAllRawPagesBelongToDocument(List<int> rawPageIds, int docId)
        {
            if (rawPageIds.Count == 0)
                return false;

            foreach (int rawPageId in rawPageIds)
            {
                Boolean doesFileExist = Adapter.GetDataByDocIdAndRawPageId(rawPageId, docId).Rows.Count > 0;

                if (!doesFileExist)
                    return false;
            }

            return true;
        }

        ///// <summary>
        ///// Checks if all the rawfiles are still attached to a particular document.
        ///// </summary>
        ///// <param name="rawPageIds"></param>
        ///// <param name="docId"></param>
        ///// <returns></returns>
        //public bool IsAllRawPagesBelongToDocument(List<int> rawPageIds, int docId)
        //{
        //    if (rawPageIds.Count == 0)
        //        return false;

        //    foreach (int rawPageId in rawPageIds)
        //    {
        //        Boolean doesFileExist = Adapter.GetDataByDocIdAndRawPageId(rawPageId, docId).Rows.Count > 0;

        //        if (!doesFileExist)
        //            return false;
        //    }

        //    return true;
        //}
        #endregion

        #region Added by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY
        public static ArrayList GetRawPagesToDisplayPDF(int docId)
        {
            ArrayList pages = new ArrayList();

            RawPageDb rawPageDb = new RawPageDb();
            DataTable rawPages = rawPageDb.GetRawPagesIdByDocId(docId);
            

            for (int cnt = 0; cnt < rawPages.Rows.Count; cnt++)
            {
                DataRow rawPage = rawPages.Rows[cnt];

                int rawPageId = 0;
                if (int.TryParse(rawPage["RawPageId"].ToString(), out rawPageId))
                {
                    DirectoryInfo rawPageDir = Util.GetIndividualRawPageOcrDirectoryInfo(rawPageId);

                    if (rawPageDir.Exists)
                    {
                        FileInfo[] rawPageFiles = rawPageDir.GetFiles("*_s.pdf");

                        if (rawPageFiles.Length > 0)
                        {
                            pages.Add(rawPageFiles[0].FullName);
                        }
                    }
                }                                                
            }

            return pages;
        }

        public static int GetRawPagesNoOfRecords(int docId)
        {
            RawPageDb rawPageDb = new RawPageDb();
            DataTable rawPages = rawPageDb.GetRawPagesIdByDocId(docId);
            return rawPages.Rows.Count;
        }

        public DataTable GetRawPagesIdByDocId(int docId)
        {
            return RawPageDs.GetRawPagesIdByDocId(docId);
        }

        public static DataTable GetYearMonthDayForFolderStructure(int rawPageId)
        {
            return RawPageDs.GetYearMonthDayForFolderStructure(rawPageId);
        }

        #endregion
    }
}
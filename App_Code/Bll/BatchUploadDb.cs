using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.Security;
using System.Data;
using System.IO;
using Dwms.Dal;
using Dwms.Web;
/// <summary>
/// Summary description for BatchUploadDb
/// </summary>
namespace Dwms.Bll
{
    public class BatchUploadDb
    {
        public static int InsertBUHeader(int sectionId, string fileName, string oic, int noOfCases, int noOfFailed)
        {            
            return BatchUploadDs.InsertBUHeader(sectionId, fileName, oic, noOfCases, noOfFailed);
        }

        public static int InsertBUDetails(int buId, string refNo, bool isSuccess, string errorMsg)
        {
            return BatchUploadDs.InsertBUDetails(buId, refNo, isSuccess, errorMsg);
        }

        public static int UpdateBUHeaderNoOfCases(int id, int noOfCases, int noOfFailed)
        {
            return BatchUploadDs.UpdateBUHeaderNoOfCases(id, noOfCases, noOfFailed);
        }

        public static DataTable GetBatchUpload(int sectionId, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            return BatchUploadDs.GetBatchUpload(sectionId, department, dateInFrom, dateInTo);
        }

        public static DataTable GetBatchUploadDetailsById(int buId)
        {
            return BatchUploadDs.GetBatchUploadDetailsById(buId);
        }
    }

    #region Added By Edward 2015/02/05 For Archival Report
    public class ArchiveDocumentsDb
    {
        public static DataTable GetArchivalReport(int section, int department, DateTime? dateInFrom, DateTime? dateInTo)
        {
            return ArchiveDocumentsDs.GetArchivalReport(section, department, dateInFrom, dateInTo);
        }
    }
    #endregion
}
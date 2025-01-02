using System;
using System.Collections.Generic;

using System.Web;
using System.Web.Security;
using AppPersonalTableAdapters;
using System.Data;
using Dwms.Dal;
using System.Collections;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for AppAppPersonalRefDb
    /// </summary>
    public class AppPersonalDb
    {
        private AppPersonalTableAdapter _AppPersonalAdapter = null;

        protected AppPersonalTableAdapter Adapter
        {
            get
            {
                if (_AppPersonalAdapter == null)
                    _AppPersonalAdapter = new AppPersonalTableAdapter();

                return _AppPersonalAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the documents 
        /// </summary>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonal()
        {
            return Adapter.GetData();
        }

        /// <summary>
        /// Get App Personal by DocAppID
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByDocAppId(int docAppId)
        {
            return Adapter.GetAppPersonalByDocAppId(docAppId);
        }

        /// <summary>
        /// Get AppPersonal by AppDocRefId
        /// </summary>
        /// <param name="setAppId"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByAppDocRefId(int setAppId)
        {
            return Adapter.GetAppPersonalByAppDocRefId(setAppId);
        }


        /// <summary>
        /// Get AppPersonal by Nric, Folder and Set AppId
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="setAppId"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByNricFolderAndDocAppId(string nric, int docAppId, string folder)
        {
            return Adapter.GetAppPersonalByNricFolderAndDocAppId(nric, folder, docAppId);
        }

        /// <summary>
        /// Get AppPersonal by Nric, Relationship, Folder and Set AppId
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="setAppId"></param>
        /// <param name="folder"></param>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByNricRelationshipFolderAndDocAppId(string nric, int docAppId, string folder, RelationshipEnum relationship)
        {
            return Adapter.GetAppPersonalByNricRelationshipFolderAndDocAppId(nric, folder, relationship.ToString(), docAppId);
        }

        /// <summary>
        /// Get AppPersonal by Nric and SetAppId
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="setAppId"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByNricAndDocAppId(string nric, int docAppId)
        {
            return Adapter.GetAppPersonalByNricAndDocAppId(nric, docAppId);
        }             

        /// <summary>
        /// Get Documents for an application wih reference to appPersonal by setappid
        /// </summary>
        /// <param name="setAppId"></param>
        /// <returns></returns>
        public DataTable GetAppPersonalDocumentByDocAppIdAndSetId(int docAppId, int setId)
        {
            return AppPersonalDs.GetAppPersonalDocumentByDocAppIdAndSetId(docAppId, setId);
        }

        /// <summary>
        /// Get DocAppId from AppPersonal by AppDocRefId
        /// </summary>
        /// <param name="appDocRefId"></param>
        /// <returns></returns>
        public int GetDocAppIdByAppDocRefId(int docAppRefId)
        {
            return AppPersonalDs.GetDocAppIdByAppDocRefId(docAppRefId);
        }

        /// <summary>
        /// Get Documents for an application wih reference to appPersonal in all the verified sets
        /// </summary>
        /// <param name="setAppId"></param>
        /// <returns></returns>
        public DataTable GetAppPersonalDocumentByDocAppId(int docAppId)
        {
            return AppPersonalDs.GetAppPersonalDocumentByDocAppId(docAppId);
        }
        /// <summary>
        /// Gets the documents of the selected AppPersonal and DocAppId 
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="appPersonalId"></param>
        /// <returns></returns>
        public DataTable GetAppPersonalDocumentByDocAppIdAndAppPersonalId(int docAppId, int appPersonalId)
        {
            return AppPersonalDs.GetAppPersonalDocumentByDocAppIdAndAppPersonalId(docAppId,appPersonalId);
        }

        /// <summary>
        /// Gets the apppersonal records with ref no details
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public DataTable GetAppPersonalReferenceDetailsByDocId(int docId)
        {
            return AppPersonalDs.GetAppPersonalReferenceDetailsByDocId(docId);
        }

        /// <summary>
        /// Get AppPersonal by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalById(int id)
        {
            return Adapter.GetAppPersonalById(id);
        }

        /// <summary>
        /// Get App Personal by DocId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalByDocId(int docId)
        {
            return Adapter.GetAppPersonalByDocId(docId);
        }

        /// <summary>
        /// Get AppPersonal for household structure by docappid
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalForHouseholdStructureByDocAppId(int docAppId)
        {
            return Adapter.GetAppPersonalForHouseholdStructureByDocAppId(docAppId);
        }

        /// <summary>
        /// Get AppPersonal for household structure by docsetId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public AppPersonal.AppPersonalDataTable GetAppPersonalForHouseholdStructureByDocSetId(int docSetId)
        {
            return Adapter.GetAppPersonalForHouseholdStructureByDocSetId(docSetId);
        }

        /// <summary>
        /// Get AppPersonal for household structure by docsetId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <returns></returns>
        public string GetNameByNricAndDocAppId(string Nric, int docAppId)
        {
            AppPersonal.AppPersonalDataTable ap = GetAppPersonalByNricAndDocAppId(Nric, docAppId);
            if(ap.Rows.Count>0)
                foreach (AppPersonal.AppPersonalRow personal in ap.Rows)
                {
                    return personal.Name;
                }
            return string.Empty;
        }

        //Added By Edward 14.11.2013 for Income Extraction Not Applicable for Unemployed
        /// <summary>
        /// Checks if UNEMPLOYED. If UNEMPLOYED, checks if Assessment is not applicable.  
        /// </summary>
        /// <param name="nric"></param>
        /// <returns>Returns False automatically when Not Unemployed, Else returns Not Applicable = True, Applicable = False</returns>
        public static bool CheckAssessmentNotApplicable(string nric, int docAppId)
        {
            return AppPersonalDs.CheckAssessmentNotApplicable(nric, docAppId);
        }
        #endregion

        #region Insert Methods
        /// <summary>
        /// Insert method
        /// </summary>
        /// <param name="AppAppPersonalRefId"></param>
        /// <param name="docType"></param>
        /// <param name="originalSetId"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public int Insert(int docAppId, string nric, string name, string personalType, 
            string dateJoinedService, string companyName, string employmentType, 
            string folder, RelationshipEnum? relationship, int orderNo, string customerId)
        {
            AppPersonal.AppPersonalDataTable dt = new AppPersonal.AppPersonalDataTable();
            AppPersonal.AppPersonalRow r = dt.NewAppPersonalRow();

            r.DocAppId = docAppId;
            r.Nric = nric;
            r.Name = name;
            r.PersonalType = personalType;
            r.DateJoinedService = dateJoinedService;
            r.CompanyName = companyName;
            r.EmploymentType = employmentType;
            r.Folder = folder;
            r.OrderNo = orderNo;
            if (!string.IsNullOrEmpty(customerId))
                r.CustomerSourceId = customerId;

            if (relationship != null && (relationship.Value == RelationshipEnum.Requestor || relationship.Value == RelationshipEnum.Spouse))
                r.Relationship = relationship.ToString();

            r.CustomerType = CustomerTypeEnum.P.ToString(); // as of now the CustomerType is defaulted to P.

            r.IdType = Retrieve.GetIdTypeByNRIC(nric);

            dt.AddAppPersonalRow(r);
            int rowsAffected = Adapter.Update(dt);

            //if (rowsAffected > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.AppPersonal, r.Id.ToString(), OperationTypeEnum.Insert);
            //}

            return r.Id;
        }

        //Created By Edward 18/12/201 to accommodate noOfIncomeMonths
        public int Insert(int docAppId, string nric, string name, string personalType,
           string dateJoinedService, string companyName, string employmentType,
           string folder, RelationshipEnum? relationship, int orderNo, string customerId, int noOfIncomeMonths)
        {
            AppPersonal.AppPersonalDataTable dt = new AppPersonal.AppPersonalDataTable();
            AppPersonal.AppPersonalRow r = dt.NewAppPersonalRow();

            r.DocAppId = docAppId;
            r.Nric = nric;
            r.Name = name;
            r.PersonalType = personalType;
            r.DateJoinedService = dateJoinedService;
            r.CompanyName = companyName;
            r.EmploymentType = employmentType;
            r.Folder = folder;
            r.OrderNo = orderNo;
            if (!string.IsNullOrEmpty(customerId))
                r.CustomerSourceId = customerId;
            r.MonthsToLEAS = noOfIncomeMonths;

            if (relationship != null && (relationship.Value == RelationshipEnum.Requestor || relationship.Value == RelationshipEnum.Spouse))
                r.Relationship = relationship.ToString();

            r.CustomerType = CustomerTypeEnum.P.ToString(); // as of now the CustomerType is defaulted to P.

            r.IdType = Retrieve.GetIdTypeByNRIC(nric);

            dt.AddAppPersonalRow(r);
            int rowsAffected = Adapter.Update(dt);

            //if (rowsAffected > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.AppPersonal, r.Id.ToString(), OperationTypeEnum.Insert);
            //}

            return r.Id;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update AppPersonal Details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nric"></param>
        /// <param name="name"></param>
        /// <param name="relationship"></param>
        /// <param name="dateJoinedService"></param>
        /// <returns></returns>
        public bool Update(int id, string nric, string name,  string relationship, string dateJoinedService, string companyName)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            if (!String.IsNullOrEmpty(nric))
                appPersonalRow.Nric = nric.ToUpper().Trim();

            if (!String.IsNullOrEmpty(name))
                appPersonalRow.Name = name.ToUpper().Trim();
            else
                appPersonalRow.Name = string.Empty;

            if (!String.IsNullOrEmpty(relationship) && (relationship.Equals(RelationshipEnum.Requestor.ToString()) ||
                relationship.Equals(RelationshipEnum.Spouse.ToString())))
                appPersonalRow.Relationship = relationship.Trim();

            if (!String.IsNullOrEmpty(dateJoinedService))
                appPersonalRow.DateJoinedService = dateJoinedService.ToUpper().Trim();

            if (!String.IsNullOrEmpty(companyName))
                appPersonalRow.CompanyName = companyName.ToUpper().Trim();

            int affected = Adapter.Update(appPersonal);

            appPersonalRow.IdType = Retrieve.GetIdTypeByNRIC(nric);

            //if name is null, take the log
            if (affected > 0 && String.IsNullOrEmpty(name))
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.AppPersonal, id.ToString(), OperationTypeEnum.Update);
            }

            return affected > 0;
        }

        public bool Update(int id, string idType, string customerSourceId)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            appPersonalRow.IdType = idType.ToUpper().Trim();
            appPersonalRow.CustomerSourceId = customerSourceId.ToUpper().Trim();
            int affected = Adapter.Update(appPersonal);

            //if name is null, take the log
            if (affected > 0 )
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.AppPersonal, id.ToString(), OperationTypeEnum.Update);
            }

            return affected > 0;
        }

        /// <summary>
        /// Update Folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docFolder"></param>
        /// <returns></returns>
        public bool UpdateNricRelationship(int id, string nric, RelationshipEnum relationship)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            if (!String.IsNullOrEmpty(nric))
                appPersonalRow.Nric = nric.ToUpper().Trim();

            appPersonalRow.IdType = Retrieve.GetIdTypeByNRIC(nric);

            appPersonalRow.Relationship = relationship.ToString().Trim();

            int affected = Adapter.Update(appPersonal);
            return affected > 0;
        }

        /// <summary>
        /// Update Folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="docFolder"></param>
        /// <returns></returns>
        public bool UpdateFolder(int id, DocFolderEnum docFolder)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            if (docFolder != null)
                appPersonalRow.Folder = docFolder.ToString();

            int affected = Adapter.Update(appPersonal);
            return affected > 0;
        }

        /// <summary>
        /// Update personal
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nric"></param>
        /// <param name="name"></param>
        /// <param name="personalType"></param>
        /// <param name="dateJoinedService"></param>
        /// <param name="companyName"></param>
        /// <param name="employmentType"></param>
        /// <param name="folder"></param>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public bool Update(int id, string nric, string name, string personalType,
            string dateJoinedService, string companyName, string employmentType, 
            string folder, RelationshipEnum? relationship, int orderNo, string customerId, int noOfIncomeMonths) //Added noOfIncomeMonths By Edward 18/12/2013
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow r = appPersonal[0];

            r.Nric = nric;
            r.Name = name;
            r.PersonalType = personalType;
            r.DateJoinedService = dateJoinedService;
            r.CompanyName = companyName;
            r.EmploymentType = employmentType;
            r.Folder = folder;
            r.OrderNo = orderNo;
            r.CustomerType = CustomerTypeEnum.P.ToString();
            if (!string.IsNullOrEmpty(customerId))
                r.CustomerSourceId = customerId;
            r.MonthsToLEAS = noOfIncomeMonths;  //Added By Edward 18/12/2013

            r.IdType = Retrieve.GetIdTypeByNRIC(nric);

            if (relationship != null && (relationship.Value == RelationshipEnum.Requestor || relationship.Value == RelationshipEnum.Spouse))
                r.Relationship = relationship.ToString();

            int affected = Adapter.Update(appPersonal);
            return affected > 0;
        }

        public bool UpdateFromCdbWebService(string refNo, string nric, string customerSourceId, string customerType, string idType)
        {
            bool result = false;

            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppDt = docAppDb.GetDocAppsByReferenceNo(refNo);

            if (docAppDt.Rows.Count > 0)
            {
                DocApp.DocAppRow docAppDr = docAppDt[0];

                int docAppId = docAppDr.Id;

                AppPersonal.AppPersonalDataTable appPersonalDt = GetAppPersonalByNricAndDocAppId(nric, docAppId);

                foreach (AppPersonal.AppPersonalRow appPersonalDr in appPersonalDt)
                {
                    appPersonalDr.Nric = nric;
                    appPersonalDr.CustomerSourceId = customerSourceId;
                    //appPersonalDr.CustomerType = customerType;
                    appPersonalDr.CustomerType = CustomerTypeEnum.P.ToString(); // as of now the CustomerType is defaulted to P.
                    appPersonalDr.IdType = Retrieve.GetIdTypeByNRIC(nric); //(idType == string.Empty) ? null : idType;

                    result = Adapter.Update(appPersonalDt) > 0;
                }
            }

            return result;
        }


        public bool UpdateMonthToLEAS(int id, int MonthToLEAS, bool assessmentNA)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            //appPersonalRow.MonthsToLEAS = MonthToLEAS;
            appPersonalRow.AssessmentNA = assessmentNA;

            int affected = Adapter.Update(appPersonal);
            return affected > 0;
        }

        #region Added By Edward Get Income Months from Household when NoOfIncomeMonths is Null
        public bool UpdateMonthToLEAS(int id, int MonthToLEAS)
        {
            AppPersonal.AppPersonalDataTable appPersonal = GetAppPersonalById(id);
            if (appPersonal.Count == 0) return false;

            AppPersonal.AppPersonalRow appPersonalRow = appPersonal[0];

            appPersonalRow.MonthsToLEAS = MonthToLEAS;            

            int affected = Adapter.Update(appPersonal);
            //if (affected > 0)
            //{
            //    AuditTrailDb auditTrailDb = new AuditTrailDb();
            //    auditTrailDb.Record(TableNameEnum.AppPersonal, id.ToString(), OperationTypeEnum.Update);
            //}

            return affected > 0;
        }
        #endregion

        #endregion

        #region Delete
        public bool DeleteByDocAppId(int docAppId)
        {
            return Adapter.DeleteByDocAppId(docAppId) > 0;
        }

        /// <summary>
        /// Delete Orpahn Records
        /// </summary>
        /// <returns></returns>
        public void DeleteOrphanRecords()
        {
            AppPersonalDs.DeleteOrphanRecords();
        }

        public bool Delete(int id)
        {
            return Adapter.Delete(id) > 0;
        }

        public bool DeleteByDocAppIdAndNric(int docAppId, string nric)
        {
            return Adapter.DeleteByDocAppIdAndNric(docAppId, nric) > 0;
        }
        #endregion

        #region Checking Methods

        /// <summary>
        /// Check if the nric is an App Nric for a given SetAppId
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="setAppId"></param>
        /// <returns></returns>
        public Boolean IsAppNric(string nric, int setAppId)
        {
            return (Adapter.IsAppNric(nric, setAppId) > 0);
        }

        #endregion

        #region Miscellaneous
        /// <summary>
        /// SAve the personal records
        /// </summary>
        /// <param name="docSetId"></param>
        /// <param name="docAppId"></param>
        public void SavePersonalRecords(int docAppId)
        {
            AppPersonalSalaryDb appPersonalSalaryDb = new AppPersonalSalaryDb();
            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docAppTable = docAppDb.GetDocAppById(docAppId);

            if (docAppTable.Rows.Count > 0)
            {
                DocApp.DocAppRow docApp = docAppTable[0];
                string refType = docApp.RefType.Trim();
                string refNo = docApp.RefNo.Trim();

                // Get the personal info basing on the reference number
                ArrayList newPersonals = GetPersonalDataFromInterface(refNo, refType);                

                if (newPersonals.Count > 0)
                {
                    AppPersonal.AppPersonalDataTable currPersonalTable = GetAppPersonalByDocAppId(docAppId);

                    // Loop through the new list and check if it exist in the current list.
                    // If it exists, update the personal record. Else, insert.                        
                    foreach (AppPersonalData appPersonal in newPersonals)
                    {
                        bool isNew = true;

                        foreach (AppPersonal.AppPersonalRow currPersonalRow in currPersonalTable)
                        {
                            if (currPersonalRow.Nric.ToLower().Equals(appPersonal.Nric.ToLower()))
                            {
                                // Update personal records
                                Update(currPersonalRow.Id, appPersonal.Nric, appPersonal.Name, appPersonal.PersonalType,
                                    appPersonal.DateJoinedService, appPersonal.CompanyName, appPersonal.EmploymentType,
                                    DocFolderEnum.Unidentified.ToString(), null, appPersonal.OrderNo, appPersonal.CustomerId, appPersonal.NoOfIncomeMonths); //Added by Edward 18/12/2013

                                AppPersonalSalary.AppPersonalSalaryDataTable personalSalaryTable = appPersonalSalaryDb.GetAppPersonalSalaryByAppPersonalId(currPersonalRow.Id);

                                if (personalSalaryTable.Rows.Count > 0)
                                {
                                    AppPersonalSalary.AppPersonalSalaryRow personalSalaryRow = personalSalaryTable[0];

                                    // Update AppPersonalSalary records
                                    appPersonalSalaryDb.Update(personalSalaryRow.Id, appPersonal.Month1Name, appPersonal.Month1Value, appPersonal.Month2Name, appPersonal.Month2Value,
                                        appPersonal.Month3Name, appPersonal.Month3Value, appPersonal.Month4Name, appPersonal.Month4Value, appPersonal.Month5Name, appPersonal.Month5Value,
                                        appPersonal.Month6Name, appPersonal.Month6Value, appPersonal.Month7Name, appPersonal.Month7Value, appPersonal.Month8Name, appPersonal.Month8Value,
                                        appPersonal.Month9Name, appPersonal.Month9Value, appPersonal.Month10Name, appPersonal.Month10Value, appPersonal.Month11Name, appPersonal.Month11Value,
                                        appPersonal.Month12Name, appPersonal.Month12Value);
                                }
                                else
                                {
                                    if (appPersonal.HasSalary)
                                    {
                                        // Create AppPersonalSalary records
                                        appPersonalSalaryDb.Insert(currPersonalRow.Id, appPersonal.Month1Name, appPersonal.Month1Value, appPersonal.Month2Name, appPersonal.Month2Value,
                                            appPersonal.Month3Name, appPersonal.Month3Value, appPersonal.Month4Name, appPersonal.Month4Value, appPersonal.Month5Name, appPersonal.Month5Value,
                                            appPersonal.Month6Name, appPersonal.Month6Value, appPersonal.Month7Name, appPersonal.Month7Value, appPersonal.Month8Name, appPersonal.Month8Value,
                                            appPersonal.Month9Name, appPersonal.Month9Value, appPersonal.Month10Name, appPersonal.Month10Value, appPersonal.Month11Name, appPersonal.Month11Value,
                                            appPersonal.Month12Name, appPersonal.Month12Value);
                                    }
                                }

                                isNew = false;
                                break;
                            }
                        }

                        if (isNew)
                        {
                            // Create AppPersonal records
                            int appPersonalId = Insert(docAppId, appPersonal.Nric, appPersonal.Name, appPersonal.PersonalType,
                                appPersonal.DateJoinedService, appPersonal.CompanyName, appPersonal.EmploymentType,
                                DocFolderEnum.Unidentified.ToString(), null, appPersonal.OrderNo, appPersonal.CustomerId, appPersonal.NoOfIncomeMonths);

                            if (appPersonal.HasSalary)
                            {
                                // Create AppPersonalSalary records
                                appPersonalSalaryDb.Insert(appPersonalId, appPersonal.Month1Name, appPersonal.Month1Value, appPersonal.Month2Name, appPersonal.Month2Value,
                                    appPersonal.Month3Name, appPersonal.Month3Value, appPersonal.Month4Name, appPersonal.Month4Value, appPersonal.Month5Name, appPersonal.Month5Value,
                                    appPersonal.Month6Name, appPersonal.Month6Value, appPersonal.Month7Name, appPersonal.Month7Value, appPersonal.Month8Name, appPersonal.Month8Value,
                                    appPersonal.Month9Name, appPersonal.Month9Value, appPersonal.Month10Name, appPersonal.Month10Value, appPersonal.Month11Name, appPersonal.Month11Value,
                                    appPersonal.Month12Name, appPersonal.Month12Value);
                            }
                        }
                    }

                    // Remove all the AppPersonal records if they have been removed from the household structure
                    RemovedDeletedPersonalFromAppPersonal(newPersonals, currPersonalTable, docAppId);

                    //for loop through appper and docper for match
                    //add doc to appper and delete from docper
                    //delete from docper
                    //DocPersonalDb docPersonalDb = new DocPersonalDb();
                    //DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalBySetDocRefId(refNo);
                    ////attach and dettach doc/app personal reference
                    //CustomPersonal customPersonal = new CustomPersonal();
                    //Boolean isSuccess = customPersonal.AttachAndDettachPersonalReference(int.Parse(sourceNode.Attributes["DocRefId"].ToString()), sourceNode, destNode, string.Empty, true, docId, setId.Value, -1, destNode.Value);
                }
            }
        }

        /// <summary>
        /// Get the personal data from the interface files
        /// </summary>
        /// <param name="refNo"></param>
        /// <param name="refType"></param>
        /// <returns></returns>
        private ArrayList GetPersonalDataFromInterface(string refNo, string refType)
        {
            ArrayList result = new ArrayList();

            if (refType.Equals(ReferenceTypeEnum.HLE.ToString()))
            {
                #region Get personal data from HLE Interface
                HleInterfaceDb hleInterfaceDb = new HleInterfaceDb();
                HleInterface.HleInterfaceDataTable hleTable = hleInterfaceDb.GetHleInterfaceByRefNo(refNo);

                foreach (HleInterface.HleInterfaceRow hleRow in hleTable.Rows)
                {
                    AppPersonalData appPersonalData = new AppPersonalData(hleRow);
                    result.Add(appPersonalData);
                }
                #endregion
            }
            else if (refType.Equals(ReferenceTypeEnum.SALES.ToString()))
            {
                #region Get personal data from SALES interface
                SalesInterfaceDb salesInterfaceDb = new SalesInterfaceDb();
                SalesInterface.SalesInterfaceDataTable salesTable = salesInterfaceDb.GetSalesInterfaceByRefNo(refNo);

                foreach (SalesInterface.SalesInterfaceRow salesRow in salesTable.Rows)
                {
                    AppPersonalData appPersonalData = new AppPersonalData(salesRow);
                    result.Add(appPersonalData);
                }
                #endregion
            }
            else if (refType.Equals(ReferenceTypeEnum.RESALE.ToString()))
            {
                #region Get personal data from RESALE interface
                ResaleInterfaceDb resaleInterfaceDb = new ResaleInterfaceDb();
                ResaleInterface.ResaleInterfaceDataTable resaleTable = resaleInterfaceDb.GetResaleInterfaceByRefNo(refNo);

                foreach (ResaleInterface.ResaleInterfaceRow resaleRow in resaleTable.Rows)
                {
                    AppPersonalData appPersonalData = new AppPersonalData(resaleRow);
                    result.Add(appPersonalData);
                }
                #endregion
            }
            else if (refType.Equals(ReferenceTypeEnum.SERS.ToString()))
            {
                #region Get personal data from SERS interface
                SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                SersInterface.SersInterfaceDataTable sersTable = sersInterfaceDb.GetSersInterfaceByRefNo(refNo);

                foreach (SersInterface.SersInterfaceRow sersRow in sersTable.Rows)
                {
                    AppPersonalData appPersonalData = new AppPersonalData(sersRow);
                    result.Add(appPersonalData);
                }
                #endregion
            }
            else
            {
            }

            return result;
        }

        /// <summary>
        /// Remove from the AppPersonal table all those personal records that have been removed from the interface tables
        /// </summary>
        /// <param name="appPersonalDataFromInterface"></param>
        /// <param name="docAppId"></param>
        private void RemovedDeletedPersonalFromAppPersonal(ArrayList appPersonalDataFromInterface, AppPersonal.AppPersonalDataTable currPersonalTable, int docAppId)
        {            
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();
            DocDb docDb = new DocDb();

            foreach (AppPersonal.AppPersonalRow appPersonal in currPersonalTable)
            {
                // Check if the AppPersonal is removed from the Household structure
                bool removedFromHouseholdStructure = true;

                foreach (AppPersonalData appPersonalData in appPersonalDataFromInterface)
                {
                    if (appPersonal.Nric.ToUpper().Equals(appPersonalData.Nric.ToUpper()) || appPersonal.Folder==DocFolderEnum.Others.ToString())
                    {
                        removedFromHouseholdStructure = false;
                        break;
                    }
                }

                if (removedFromHouseholdStructure)
                {
                    // Get all the documents tied to the AppPersonal                    
                    AppDocRef.AppDocRefDataTable appDocRefDt = appDocRefDb.GetAppDocRefByAppPersonalId(appPersonal.Id);

                    ArrayList docIdList = new ArrayList();

                    foreach (AppDocRef.AppDocRefRow appDocRef in appDocRefDt)
                    {
                        // Check if the document has multiple references by other AppPersonal records.  If it has
                        // multiple references, do not edit the document. Instead delete directly the AppPersonal record,
                        // which will automatically remove the AppDocRef record.
                        if (!appDocRefDb.DocHasMultiplePersonalRefBesideGivenPersonal(appDocRef.DocId, appPersonal.Id))
                        {
                            if (!docIdList.Contains(appDocRef.Id))
                                docIdList.Add(appDocRef.DocId);
                        }
                    }

                    // Assign DocPersonal to each document and assign the value 'Unidentified' to the folder
                    foreach (int docId in docIdList)
                    {
                        Doc.DocDataTable docDt = docDb.GetDocById(docId);

                        if (docDt.Rows.Count > 0)
                        {
                            Doc.DocRow doc = docDt[0];

                            int appPersonalId = -1;
                            AppPersonal.AppPersonalDataTable currAppDt = appPersonalDb.GetAppPersonalByNricFolderAndDocAppId(appPersonal.Nric, docAppId, 
                                DocFolderEnum.Others.ToString());

                            if (currAppDt.Rows.Count > 0)
                            {
                                AppPersonal.AppPersonalRow currAppDr = currAppDt[0];
                                appPersonalId = currAppDr.Id;
                            }
                            else
                            {
                                // Insert the doc personal record for the doc
                                appPersonalId = appPersonalDb.Insert(docAppId, appPersonal.Nric, appPersonal.Name, appPersonal.PersonalType, string.Empty, 
                                    string.Empty, string.Empty, DocFolderEnum.Others.ToString(), null, appPersonal.OrderNo, appPersonal.IsCustomerSourceIdNull() ? string.Empty : appPersonal.CustomerSourceId, 
                                    appPersonal.IsMonthsToLEASNull() ? 0 : appPersonal.MonthsToLEAS);
                            }

                            // Insert the association of the doc and doc personal
                            appDocRefDb.Insert(appPersonalId, docId);
                        }
                    }

                    // Remove the AppPersonal record
                    Delete(appPersonal.Id);
                }
            }
        }
        #endregion
    }
}
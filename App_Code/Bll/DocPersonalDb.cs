using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using DocPersonalTableAdapters;
using Dwms.Dal;

namespace Dwms.Bll
{
    /// <summary>
    /// Summary description for DocSetDb
    /// </summary>
    public class DocPersonalDb
    {
        private DocPersonalTableAdapter _DocPersonalAdapter = null;

        protected DocPersonalTableAdapter Adapter
        {
            get
            {
                if (_DocPersonalAdapter == null)
                    _DocPersonalAdapter = new DocPersonalTableAdapter();

                return _DocPersonalAdapter;
            }
        }

        #region Retrieve Methods
        /// <summary>
        /// Retrieve the DocPersonals 
        /// </summary>
        /// <returns></returns>
        public DocPersonal.DocPersonalDataTable GetDocPersonal()
        {
            return Adapter.GetDocPersonal();
        }

        /// <summary>
        /// Get Doc Personal by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DocPersonal.DocPersonalDataTable GetDocPersonalById(int id)
        {
            return Adapter.GetDocPersonalById(id);
        }

        /// <summary>
        /// Retrieve the DocPersonal by docId
        /// </summary>
        /// <returns></returns>
        public DocPersonal.DocPersonalDataTable GetDocPersonalByDocId(int id)
        {
            return Adapter.GetDocPersonalByDocId(id);
        }

        public DocPersonal.DocPersonalDataTable GetDocPersonalBySetDocRefId(int id)
        {
            return Adapter.GetDocPersonalBySetDocRefId(id);
        }

        /// <summary>
        /// Get Documents for a set wih reference to docPersonal by docsetid
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        
        public DataTable GetDocPersonalDocumentByDocSetId(int docSetId)
        {
            return DocPersonalDs.GetDocPersonalDocumentByDocSetId(docSetId);
        }

        /// <summary>
        /// Get Doc Personal by nric, folder and docsetid
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="docSetId"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public DocPersonal.DocPersonalDataTable GetDocPersonalByNricFolderAndDocSetId(string nric, int docSetId, string folder)
        {
            return Adapter.GetDocPersonalByNricFolderAndDocSetId(nric, docSetId, folder);
        }

        /// <summary>
        /// Get Doc Personal by nric, relationship and docsetid
        /// </summary>
        /// <param name="nric"></param>
        /// <param name="docSetId"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public DocPersonal.DocPersonalDataTable GetDocPersonalByNricRelationshipFolderAndDocSetId(string nric, RelationshipEnum relationship, int docSetId, string folder)
        {
            return Adapter.GetDocPersonalByNricRelationshipFolderAndDocSetId(nric, relationship.ToString(), docSetId, folder);
        }

        /// <summary>
        /// Get the PersonalType based on the doctype
        /// </summary>
        /// <param name="docTypeCode"></param>
        /// <returns></returns>
        public string GetPersonalTypeByDocTypeCode(string docTypeCode)
        {
            //switch (docTypeCode.ToLower().Trim())
            //{
            //    case "hle":
            //        return "HA";
            //    case "birthcertificateloc":
            //        return DocTypeMetaDataBirthCertificateLocEnum.CH.ToString();
            //    case "birthcertificatefor":
            //        return DocTypeMetaDataBirthCertificateForEnum.CH.ToString();
            //    case "cpfstatement":
            //        return DocTypeMetaDataCPFStatementEnum.CP.ToString();
            //    case "deathcertificateloc":
            //        return DocTypeMetaDataDeathCertificateLocEnum.DC.ToString();
            //    case "deathcertificatefor":
            //        return DocTypeMetaDataDeathCertificateForEnum.DC.ToString();
            //    case "marriagecertloc":
            //        return DocTypeMetaDataMarriageCertificateLocEnum.Husband.ToString();
            //    case "marriagecertfor":
            //        return DocTypeMetaDataMarriageCertificateForEnum.Wife.ToString();
            //    case "cbr":
            //        return DocTypeMetaDataCBREnum.CB.ToString();
            //    case "irasir8e":
            //        return DocTypeMetaDataIRASIR8EEnum.IR.ToString();
            //    case "irasassesement":
            //        return string.Empty;
            //    case "cpfcontribution":
            //        return DocTypeMetaDataCPFContributionEnum.CP.ToString();
            //    case "statutorydeclaration":
            //        return DocTypeMetaDataStatutoryDeclarationEnum.SD.ToString();
            //    case "payslip":
            //        return DocTypeMetaDataPAYSLIPEnum.PS.ToString();
            //    default:
                    return string.Empty;
            //}
        }

        #endregion

        #region Insert Methods


        public int Insert(int docSetId, string nric, string name, string folder, RelationshipEnum? relationship)
        {
            DocPersonal.DocPersonalDataTable dt = new DocPersonal.DocPersonalDataTable();
            DocPersonal.DocPersonalRow r = dt.NewDocPersonalRow();

            r.DocSetId = docSetId;
            r.Nric = nric;
            r.Name = name;
            r.Folder = folder;
            r.CustomerType = CustomerTypeEnum.P.ToString(); // as of now the CustomerType is defaulted to P.
            r.IdType = Retrieve.GetIdTypeByNRIC(nric);

            if (relationship != null)
                r.Relationship = relationship.ToString();

            dt.AddDocPersonalRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }


        // The below 2 Insert functions are used in test pages, check and remove them when not required.

        public int Insert(int docId, string personalType, string nric, string name, string companyName, string dateJoinedService, EmploymentStatusEnum employmentStatus)
        {
            return Insert(docId, personalType, nric, name,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, employmentStatus);
        }

        public int Insert(int docId, string personalType, string nric, string name, string companyName, string dateJoinedService,
            string month1Name, string month2Name, string month3Name, string month4Name, string month5Name, string month6Name,
            string month7Name, string month8Name, string month9Name, string month10Name, string month11Name, string month12Name, 
            string month1Value, string month2Value, string month3Value,string month4Value, string month5Value, string month6Value,
            string month7Value, string month8Value, string month9Value,string month10Value, string month11Value, string month12Value,
            EmploymentStatusEnum employmentStatus)
        {
            DocPersonal.DocPersonalDataTable dt = new DocPersonal.DocPersonalDataTable();
            DocPersonal.DocPersonalRow r = dt.NewDocPersonalRow();

            //r.DocId = docId;

            //if (!String.IsNullOrEmpty(personalType))
            //    r.PersonalType = personalType;

            //r.PersonalType = (String.IsNullOrEmpty(personalType) ? " " : personalType);

            r.Nric = (String.IsNullOrEmpty(nric) ? " " : nric.ToUpper().Trim());
            r.Name = (String.IsNullOrEmpty(name) ? " " : name.ToUpper().Trim());

            //if (!String.IsNullOrEmpty(companyName))
            //    r.CompanyName = companyName;

            //r.CompanyName = (String.IsNullOrEmpty(companyName) ? " " : companyName);

            //r.DateJoinedService = (String.IsNullOrEmpty(dateJoinedService) ? " " : dateJoinedService);
            //r.Month1Name = (String.IsNullOrEmpty(month1Name) ? " " : month1Name);
            //r.Month2Name = (String.IsNullOrEmpty(month2Name) ? " " : month2Name);
            //r.Month3Name = (String.IsNullOrEmpty(month3Name) ? " " : month3Name);
            //r.Month4Name = (String.IsNullOrEmpty(month4Name) ? " " : month4Name);
            //r.Month5Name = (String.IsNullOrEmpty(month5Name) ? " " : month5Name);
            //r.Month6Name = (String.IsNullOrEmpty(month6Name) ? " " : month6Name);
            //r.Month7Name = (String.IsNullOrEmpty(month7Name) ? " " : month7Name);
            //r.Month8Name = (String.IsNullOrEmpty(month8Name) ? " " : month8Name);
            //r.Month9Name = (String.IsNullOrEmpty(month9Name) ? " " : month9Name);
            //r.Month10Name = (String.IsNullOrEmpty(month11Name) ? " " : month10Name);
            //r.Month11Name = (String.IsNullOrEmpty(month11Name) ? " " : month11Name);
            //r.Month12Name = (String.IsNullOrEmpty(month12Name) ? " " : month12Name);

            //r.Month1Value = (String.IsNullOrEmpty(month1Value) ? " " : month1Value);
            //r.Month2Value = (String.IsNullOrEmpty(month2Value) ? " " : month2Value);
            //r.Month3Value = (String.IsNullOrEmpty(month3Value) ? " " : month3Value);
            //r.Month4Value = (String.IsNullOrEmpty(month4Value) ? " " : month4Value);
            //r.Month5Value = (String.IsNullOrEmpty(month5Value) ? " " : month5Value);
            //r.Month6Value = (String.IsNullOrEmpty(month6Value) ? " " : month6Value);
            //r.Month7Value = (String.IsNullOrEmpty(month7Value) ? " " : month7Value);
            //r.Month8Value = (String.IsNullOrEmpty(month8Value) ? " " : month8Value);
            //r.Month9Value = (String.IsNullOrEmpty(month9Value) ? " " : month9Value);
            //r.Month10Value = (String.IsNullOrEmpty(month10Value) ? " " : month10Value);
            //r.Month11Value = (String.IsNullOrEmpty(month11Value) ? " " : month11Value);
            //r.Month12Value = (String.IsNullOrEmpty(month12Value) ? " " : month12Value);
            //if (employmentStatus != null)
            //    r.EmployementType = employmentStatus.ToString();

            dt.AddDocPersonalRow(r);
            Adapter.Update(dt);
            int id = r.Id;
            return id;
        }


        #endregion

        #region Update Methods

        public bool Update(int id, int docId, string relationship, string nric, string name, string companyName, string dateJoinedService)
        {
            return Update(id, docId, relationship, nric, name);
        }

        public bool Update(int id, int docId, string relationship, string nric, string name)
        {
            DocPersonal.DocPersonalDataTable dt = GetDocPersonalById(id);

            if (dt.Count == 0) return false;

            DocPersonal.DocPersonalRow r = dt[0];

            r.Relationship = (String.IsNullOrEmpty(relationship) ? " " : relationship.ToUpper().Trim());
            r.Nric = (String.IsNullOrEmpty(nric) ? " " : nric.ToUpper().Trim());
            r.Name = (String.IsNullOrEmpty(name) ? " " : name.ToUpper().Trim());

            r.IdType = Retrieve.GetIdTypeByNRIC(nric);

            int affected = Adapter.Update(dt);
            return affected > 0;
        }

        /// <summary>
        /// update DocPersonal record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nric"></param>
        /// <param name="name"></param>
        /// <param name="folder"></param>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public bool Update(int id, string nric, string name, string folder, string relationship)
        {
            DocPersonal.DocPersonalDataTable docPersonal = GetDocPersonalById(id);
            if (docPersonal.Count == 0) return false;

            DocPersonal.DocPersonalRow docPersonalRow = docPersonal[0];

            if (!String.IsNullOrEmpty(nric))
                docPersonalRow.Nric = nric.ToUpper().Trim();

            docPersonalRow.IdType = Retrieve.GetIdTypeByNRIC(nric);

            if (!String.IsNullOrEmpty(name))
                docPersonalRow.Name = name.ToUpper().Trim();
            else
                docPersonalRow.SetNameNull();

            if (!String.IsNullOrEmpty(relationship))
                docPersonalRow.Relationship = relationship.ToUpper().Trim();

            if (!String.IsNullOrEmpty(folder))
                docPersonalRow.Folder = folder.ToUpper().Trim();

            if (!String.IsNullOrEmpty(relationship))
                docPersonalRow.Relationship = relationship.Trim();



            int affected = Adapter.Update(docPersonalRow);
            return affected > 0;
        }

        /// <summary>
        /// Update nric and relationship
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nric"></param>
        /// <param name="relationship"></param>
        /// <returns></returns>
        public bool UpdateNricRelationship(int id, string nric, RelationshipEnum relationship)
        {
            return Update(id, nric, String.Empty, string.Empty, relationship.ToString());
        }

        /// <summary>
        /// Update Name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateName(int id, string name)
        {
            return Update(id, String.Empty, name, string.Empty, String.Empty);
        }

        /// <summary>
        /// Update foldername
        /// </summary>
        /// <param name="id"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public bool UpdateFolder(int id, string folderName)
        {
            return Update(id, String.Empty, string.Empty, folderName, String.Empty);
        }

        public bool Update(int id, string idType, string customerSourceId)
        {
            DocPersonal.DocPersonalDataTable docPersonal = GetDocPersonalById(id);
            if (docPersonal.Count == 0) return false;

            DocPersonal.DocPersonalRow docPersonalRow = docPersonal[0];

            docPersonalRow.IdType = idType.ToUpper().Trim();
            docPersonalRow.CustomerSourceId = customerSourceId.ToUpper().Trim();
            
            int affected = Adapter.Update(docPersonal);

            //if name is null, take the log
            if (affected > 0)
            {
                AuditTrailDb auditTrailDb = new AuditTrailDb();
                auditTrailDb.Record(TableNameEnum.DocPersonal, id.ToString(), OperationTypeEnum.Update);
            }

            return affected > 0;
        }

        /// <summary>
        /// Update nric for first docpersonal records for a given docid
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public bool UpdateNricByDocId(int docId, string nric)
        {
            DocPersonal.DocPersonalDataTable dt = GetDocPersonalByDocId(docId);

            int updatedRowCount = 0;

            foreach (DocPersonal.DocPersonalRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(nric))
                    dr.SetNricNull();
                else
                    dr.Nric = nric.ToUpper().Trim();

                dr.IdType = Retrieve.GetIdTypeByNRIC(nric);

                int rowsAffected = Adapter.Update(dt);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);
                }
            }

            return (updatedRowCount == dt.Rows.Count);
        }

        /// <summary>
        /// Update first nric in docpersonal for a given docid
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="nric"></param>
        /// <returns></returns>
        public bool UpdateFirstNricByDocId(int docId, string nric)
        {
            DocPersonal.DocPersonalDataTable dt = GetDocPersonalByDocId(docId);

            int updatedRowCount = 0;

            if (dt.Rows.Count > 0)
            {
                DocPersonal.DocPersonalRow dr = dt[0];

                if (string.IsNullOrEmpty(nric))
                    dr.SetNricNull();
                else
                    dr.Nric = nric.ToUpper().Trim();

                dr.IdType = Retrieve.GetIdTypeByNRIC(nric);

                int rowsAffected = Adapter.Update(dt);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);
                }
            }

            return (updatedRowCount == 1);
        }

        /// <summary>
        /// Update name in docpersonal record for a given docid.
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateNameByDocId(int docId, string name)
        {
            DocPersonal.DocPersonalDataTable dt = GetDocPersonalByDocId(docId);

            int updatedRowCount = 0;

            foreach (DocPersonal.DocPersonalRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(name))
                    dr.SetNameNull();
                else
                    dr.Name = name.ToUpper().Trim();

                int rowsAffected = Adapter.Update(dt);

                if (rowsAffected > 0)
                {
                    updatedRowCount++;
                    AuditTrailDb auditTrailDb = new AuditTrailDb();
                    auditTrailDb.Record(TableNameEnum.Doc, dr.Id.ToString(), OperationTypeEnum.Update);
                }
            }

            return (updatedRowCount == dt.Rows.Count);
        }

        public bool UpdateFromCdbWebService(string nric, string customerSourceId, string customerType, string idType)
        {
            bool result = false;

            DocPersonal.DocPersonalDataTable docPersonalDt = Adapter.GetDocPersonalByNric(nric);

            foreach (DocPersonal.DocPersonalRow docPersonalDr in docPersonalDt)
            {
                docPersonalDr.CustomerSourceId = customerSourceId;
                docPersonalDr.CustomerType = CustomerTypeEnum.P.ToString(); // as of now the CustomerType is defaulted to P.
                docPersonalDr.IdType = Retrieve.GetIdTypeByNRIC(nric);//(idType == string.Empty) ? null : idType;

                result = Adapter.Update(docPersonalDt) > 0;
            }

            return result;
        }
        #endregion

        #region Delete

        /// <summary>
        /// delete by docId
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public bool DeletebyDocId(int docId)
        {
            Boolean deleteStatus = false;
            DocPersonal.DocPersonalDataTable docPersonal = GetDocPersonalByDocId(docId);

            foreach (DocPersonal.DocPersonalRow dpRow in docPersonal.Rows)
            {
                deleteStatus = Delete(dpRow.Id);     
            }

            return deleteStatus;
        }

        /// <summary>
        /// Delete by docpersonal id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            int rowsAffected = 0;
            Guid? operationId = auditTrailDb.Record(TableNameEnum.DocPersonal, id.ToString(), OperationTypeEnum.Delete);

            rowsAffected = Adapter.Delete(id);

            if (rowsAffected > 0)
            {
                auditTrailDb.Record(TableNameEnum.DocPersonal, id.ToString(), OperationTypeEnum.Delete);
            }

            return (rowsAffected > 0);
        }

        #endregion

    }
}
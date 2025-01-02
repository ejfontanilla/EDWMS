using System;
using System.Collections.Generic;

using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class DocSetDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods

        #region Commented By Edward, Not being used 2016/01/26 - Delete this after a few months only in Verification/Log.aspx
        ///// <summary>
        ///// Get DocSet and respective docApp Details
        ///// </summary>
        ///// <param name="docSetId"></param>
        ///// <returns></returns>
        //public static DataTable GetDocSetDocAppDetail(int docSetId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT * FROM DocSet LEFT JOIN DocApp ON DocSet.DocAppId = DocApp.Id ");
        //    sqlStatement.Append("WHERE DocSet.Id=@docSetId");

        //    command.Parameters.Add("@docSetId", SqlDbType.Int);
        //    command.Parameters["@docSetId"].Value = docSetId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        #endregion

        /// <summary>
        /// Get distinct AcknowledgeNumber from docSet for dropdown display
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAcknowledgeNumberForDropDown()
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT(AcknowledgeNumber) FROM DocSet WHERE AcknowledgeNumber IS NOT NULL AND LEN(AcknowledgeNumber)>0 ORDER BY AcknowledgeNumber");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get docset with section details
        /// </summary>
        /// <param name="docSetId"></param>
        /// <returns></returns>
        public static DataTable GetDocSetDocSectionDetail(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DocSet.*,Section.Name,Section.Code,Section.Department,Section.BusinessCode FROM DocSet LEFT JOIN Section ON DocSet.SectionId = Section.Id ");
            sqlStatement.Append("WHERE DocSet.Id=@docSetId");

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get User Docset for Section Change
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetDocSetByUserIdForSectionChange(Guid userId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT * FROM DocSet WHERE VerificationStaffUserId = @userId AND (Status=@status1 OR Status=@status2)");

            command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
            command.Parameters.Add("@status1", SqlDbType.NChar);
            command.Parameters.Add("@status2", SqlDbType.NChar);
            command.Parameters["@userId"].Value = userId;
            command.Parameters["@status1"].Value = SetStatusEnum.Verification_In_Progress.ToString();
            command.Parameters["@status2"].Value = SetStatusEnum.Pending_Verification.ToString();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get Doc App Id of latest set for NRIC
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataTable GetLatestSetForNric(string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT TOP 1 DocSet.* FROM DocSet WHERE DocAppId IN ");
            sqlStatement.Append("(SELECT Id FROM DocApp WHERE RefNo IN (SELECT HleNumber FROM HleInterface WHERE Nric=@nric)) ");
            sqlStatement.Append("ORDER BY VerificationDateIn DESC");

            command.Parameters.Add("@nric", SqlDbType.VarChar);
            command.Parameters["@nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }
 
        /// <summary>
        /// Get Reference Details for a given docset
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static DataTable GetReferenceDetailsById(int Id)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT RefType,RefNo FROM DocSet INNER JOIN SetApp  ");
            sqlStatement.Append("ON DocSet.Id = SetApp.DocSetId INNER JOIN DocApp  ");
            sqlStatement.Append("ON SetApp.DocAppId = DocApp.Id  ");
            sqlStatement.Append("WHERE DocSet.Id = @Id ");

            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = Id;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }


        public static DataTable GetSetsOLD(string channel, SetStatusEnum? status, string referenceNumber, int department, int section,
            Guid? ImportedByOIC, Guid? VerificationByOIC, DateTime? dateInFrom, DateTime? dateInTo, int docAppId, string nric, Boolean isUrgent, Boolean isSkipCategorization, string cmDocumentId, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
                       
            // Add the source query
            if (!channel.Equals("-1"))
            {               
                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;               
            }

            if (docAppId != -1)
            {                
                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            // Add the status query
            if (status != null)
            {                
                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status.ToString();
            }

            // Add the AcknowledgeNumber query
            if (acknowledgeNumber != null)
            {
                if (!string.IsNullOrEmpty(acknowledgeNumber))
                {                    
                    command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                    command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
                }
            }            

            // Add the department query
            if (department != -1)
            {                
                command.Parameters.Add("@DepartmentId", SqlDbType.Int);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {                
                command.Parameters.Add("@SectionId", SqlDbType.Int);
                command.Parameters["@SectionId"].Value = section;
            }

            // Add the Imported By OIC query
            if (ImportedByOIC.HasValue)
            {                
                command.Parameters.Add("@ImportedBy", SqlDbType.UniqueIdentifier);
                command.Parameters["@ImportedBy"].Value = ImportedByOIC.Value;
            }

            // Add the Verification OIC query
            if (VerificationByOIC.HasValue)
            {                
                command.Parameters.Add("@VerificationStaffUserID", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserID"].Value = VerificationByOIC.Value;
            }

            // Add the reference number query
            if (!String.IsNullOrEmpty(referenceNumber))
            {                
                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            // Add the date in and out query
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {                
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {                
                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {                
                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //isUrgent
            if (isUrgent)
            {                
                command.Parameters.Add("@IsUrgent", SqlDbType.Bit);
                command.Parameters["@IsUrgent"].Value = isUrgent;                
            }

            //isUrgent
            if (isSkipCategorization)
            {                
                command.Parameters.Add("@IsSkipCategorization", SqlDbType.Bit);
                command.Parameters["@IsSkipCategorization"].Value = isSkipCategorization;                
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {                
                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //CmDocumentId
            cmDocumentId = cmDocumentId.Trim();
            if (!String.IsNullOrEmpty(cmDocumentId))
            {                
                command.Parameters.Add("@cmDocumentId", SqlDbType.VarChar);
                command.Parameters["@cmDocumentId"].Value = cmDocumentId;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Search_GetAllSearchSets";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        public static DataTable GetSets(string channel, SetStatusEnum? status, string referenceNumber, int department, int section,
            Guid? ImportedByOIC, Guid? VerificationByOIC, DateTime? dateInFrom, DateTime? dateInTo, int docAppId, string nric, Boolean isUrgent, Boolean isSkipCategorization, string cmDocumentId, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Added Top 2000 by Edward 29.10.2013
            //sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedBy, ImportedOn, Code, DepartmentCode,IsUrgent, SkipCategorization, ProcessingStartDate, ProcessingEndDate ");            
            sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedBy, ImportedOn, Code, DepartmentCode,IsUrgent, SkipCategorization, ProcessingStartDate, ProcessingEndDate ");            
            sqlStatement.Append("FROM vDocSet ");

            bool hasWhere = false;
            // Add the source query
            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("WHERE Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
                hasWhere = true;
            }

            if (docAppId != -1)
            {
                if (hasWhere)
                    sqlStatement.Append("AND DocAppId=@DocAppId ");
                else
                {
                    sqlStatement.Append("WHERE DocAppId=@DocAppId ");
                    hasWhere = true;
                }

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            // Add the status query
            if (status != null)
            {
                if (hasWhere)
                    sqlStatement.Append("AND Status=@Status ");
                else
                {
                    sqlStatement.Append("WHERE Status=@Status ");
                    hasWhere = true;
                }

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status.ToString();
            }

            // Add the AcknowledgeNumber query
            if (acknowledgeNumber != null)
            {
                if (!string.IsNullOrEmpty(acknowledgeNumber))
                {
                    if (hasWhere)
                        sqlStatement.Append("AND AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");
                    else
                    {
                        sqlStatement.Append("WHERE AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");
                        hasWhere = true;
                    }

                    command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                    command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
                }
            }

            //else
            //{
            //    if (hasWhere)
            //        sqlStatement.Append("AND Status<>@Status ");
            //    else
            //    {
            //        sqlStatement.Append("WHERE Status<>@Status ");
            //        hasWhere = true;
            //    }

            //    command.Parameters.Add("@Status", SqlDbType.VarChar);
            //    command.Parameters["@Status"].Value = SetStatusEnum.Pending_Categorization.ToString();
            //}

            // Add the department query
            if (department != -1)
            {
                if (hasWhere)
                    sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                else
                {
                    sqlStatement.Append("WHERE DepartmentId=@DepartmentId ");
                    hasWhere = true;
                }

                command.Parameters.Add("@DepartmentId", SqlDbType.Int);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                if (hasWhere)
                    sqlStatement.Append("AND SectionId=@SectionId ");
                else
                {
                    sqlStatement.Append("WHERE SectionId=@SectionId ");
                    hasWhere = true;
                }

                command.Parameters.Add("@SectionId", SqlDbType.Int);
                command.Parameters["@SectionId"].Value = section;
            }

            // Add the Imported By OIC query
            if (ImportedByOIC.HasValue)
            {
                if (hasWhere)
                    sqlStatement.Append("AND ImportedBy=@ImportedBy ");
                else
                {
                    sqlStatement.Append("WHERE ImportedBy=@ImportedBy ");
                    hasWhere = true;
                }

                command.Parameters.Add("@ImportedBy", SqlDbType.UniqueIdentifier);
                command.Parameters["@ImportedBy"].Value = ImportedByOIC.Value;
            }

            // Add the Verification OIC query
            if (VerificationByOIC.HasValue)
            {
                if (hasWhere)
                    sqlStatement.Append("AND VerificationStaffUserID=@VerificationStaffUserID ");
                else
                {
                    sqlStatement.Append("WHERE VerificationStaffUserID=@VerificationStaffUserID ");
                    hasWhere = true;
                }

                command.Parameters.Add("@VerificationStaffUserID", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserID"].Value = VerificationByOIC.Value;
            }

            // Add the reference number query
            if (!String.IsNullOrEmpty(referenceNumber))
            {
                if (hasWhere)
                    sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");
                else
                {
                    sqlStatement.Append("WHERE RefNo LIKE '%' + @RefNo + '%' ");
                    hasWhere = true;
                }

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            // Add the date in and out query
            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                if (hasWhere)
                    sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");
                else
                {
                    sqlStatement.Append("WHERE VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");
                    hasWhere = true;
                }

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                if (hasWhere)
                    sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");
                else
                {
                    sqlStatement.Append("WHERE VerificationDateIn >= @DateInFrom ");
                    hasWhere = true;
                }

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                if (hasWhere)
                    sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");
                else
                {
                    sqlStatement.Append("WHERE VerificationDateIn <= @DateInTo ");
                    hasWhere = true;
                }

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //isUrgent
            if (isUrgent)
            {
                if (hasWhere)
                    sqlStatement.Append("AND IsUrgent = @IsUrgent AND Status=@StatusForUrgent ");
                else
                {
                    sqlStatement.Append("WHERE IsUrgent = @IsUrgent AND Status=@StatusForUrgent ");
                    hasWhere = true;
                }

                command.Parameters.Add("@IsUrgent", SqlDbType.Bit);
                command.Parameters["@IsUrgent"].Value = isUrgent;
                command.Parameters.Add("@StatusForUrgent", SqlDbType.VarChar);
                command.Parameters["@StatusForUrgent"].Value = SetStatusEnum.Pending_Categorization.ToString();
            }

            //isUrgent
            if (isSkipCategorization)
            {
                if (hasWhere)
                    sqlStatement.Append("AND SkipCategorization = @IsSkipCategorization AND Status=@StatusForSkip ");
                else
                {
                    sqlStatement.Append("WHERE SkipCategorization = @IsSkipCategorization AND Status=@StatusForSkip ");
                    hasWhere = true;
                }

                command.Parameters.Add("@IsSkipCategorization", SqlDbType.Bit);
                command.Parameters["@IsSkipCategorization"].Value = isSkipCategorization;
                command.Parameters.Add("@StatusForSkip", SqlDbType.VarChar);
                command.Parameters["@StatusForSkip"].Value = SetStatusEnum.Pending_Categorization.ToString();
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                if (hasWhere)
                    sqlStatement.Append("AND ID IN (");
                else
                {
                    sqlStatement.Append("WHERE ID IN (");
                    hasWhere = true;
                }

                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //CmDocumentId
            cmDocumentId = cmDocumentId.Trim();
            if (!String.IsNullOrEmpty(cmDocumentId))
            {
                if (hasWhere)
                    sqlStatement.Append("AND ID IN (");
                else
                {
                    sqlStatement.Append("WHERE ID IN (");
                    hasWhere = true;
                }

                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc WHERE CmDocumentId like '%' + cmDocumentId + '%')");

                command.Parameters.Add("@cmDocumentId", SqlDbType.VarChar);
                command.Parameters["@cmDocumentId"].Value = cmDocumentId;
            }
            //Edit by Edward 08.10.2013
            //sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedBy, ImportedOn, Code, DepartmentCode,IsUrgent, SkipCategorization, ProcessingStartDate, ProcessingEndDate ");
            sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedBy, ImportedOn, Code, DepartmentCode,IsUrgent, SkipCategorization, ProcessingStartDate, ProcessingEndDate ORDER BY VerificationDateIn DESC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetSetsAssignedToUser(string source, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, Guid currUserId, SetStatusEnum? status, int docAppId, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Edited By Edward 30.10.2013 Optimizing Listings
            //Added top 2000 by Edward 29.10.2013
            //sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedOIC ");
            //sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedOIC ");
            //sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut,  Address,  Status, VerificationOIC, SetNo, SectionId, ImportedOIC, DateAssigned ");
            sqlStatement.Append("FROM vVerificationGetAllSets ");
            sqlStatement.Append("WHERE VerificationStaffUserId=@VerificationStaffUserId ");

            command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@VerificationStaffUserId"].Value = currUserId;

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status.ToString();
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 AND Status<>@Status2 AND Status<>@Status3 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = SetStatusEnum.Pending_Categorization.ToString();

                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters["@Status2"].Value = SetStatusEnum.New.ToString();

                command.Parameters.Add("@Status3", SqlDbType.VarChar);
                command.Parameters["@Status3"].Value = SetStatusEnum.Categorization_Failed.ToString();
            }

            if (!source.Equals("-1"))
            {
                //Edited By Edward 30.10.2013 Optimizing Listings
                //sqlStatement.Append("AND Source=@Source ");
                sqlStatement.Append("AND Channel=@Source ");

                command.Parameters.Add("@Source", SqlDbType.VarChar);
                command.Parameters["@Source"].Value = source;
            }

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(acknowledgeNumber.Trim()))
            {
                sqlStatement.Append("AND AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");

                command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //Edited By Edward 30.10.2013 Optimzing Listings
            //Added Order by Edward 29.10.2013
            //sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedOIC");
            //sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedOIC ORDER BY VerificationDateIn DESC");
            sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned,  Address,  Status, VerificationOIC, SetNo,  SectionId,  Id, ImportedOIC ORDER BY VerificationDateIn DESC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM

        //public static DataTable GetvDocSetsByAppId(int docAppId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel ");
        //    sqlStatement.Append("FROM vDocSet ");
        //    sqlStatement.Append("WHERE DocAppId=@DocAppId");

        //    command.Parameters.Add("@DocAppId", SqlDbType.Int);
        //    command.Parameters["@DocAppId"].Value = docAppId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public static DataTable GetvDocSetsByAppId(int docAppId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@DocAppId", SqlDbType.Int);
            command.Parameters["@DocAppId"].Value = docAppId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSet_GetvDocSetsByAppId";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        #endregion

        #region Modified by Edward 2017/10/30  to minimize retrieval or records
        //Added by Edward 2017/10/30 to speed up the verification list
        public static DataTable GetAllSets(string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {               
                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {                
                command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {                
                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }

            if (!channel.Equals("-1"))
            {
                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {              
                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(acknowledgeNumber.Trim()))
            {                
                command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {                
                command.Parameters.Add("@VerificationDateInFrom", SqlDbType.DateTime);
                command.Parameters["@VerificationDateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@VerificationDateInTo", SqlDbType.DateTime);
                command.Parameters["@VerificationDateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {                
                command.Parameters.Add("@VerificationDateInFrom", SqlDbType.DateTime);
                command.Parameters["@VerificationDateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {                
                command.Parameters.Add("@VerificationDateInTo", SqlDbType.DateTime);
                command.Parameters["@VerificationDateInTo"].Value = dateInTo.Value;
            }

            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                command.Parameters.Add("@NRIC", SqlDbType.VarChar);
                command.Parameters["@NRIC"].Value = nric;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Verification_GetAllSets";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        //original method to be deleted if not used within 1year
        public static DataTable GetAllSetsOLD(string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Edit to vVerificationGetAllSets by Edward 30.10.2013 Optimizing Listings
            //Added Top 2000 By Edward 29.10.2013
            //sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ");
            //sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ");
            //sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("SELECT TOP 2000 ID, SetNo, VerificationDateIn, VerificationDateOut,  Status,  Channel, VerificationStaffUserId, DateAssigned, Address, VerificationOIC, SectionId, ImportedOIC ");
            //sqlStatement.Append("SELECT TOP 2000 ID, SetNo, VerificationDateIn, VerificationDateOut,  Status,  Channel, VerificationStaffUserId, DateAssigned, VerificationOIC, SectionId, ImportedOIC ");
            sqlStatement.Append("FROM vVerificationGetAllSets ");
            sqlStatement.Append("WHERE SectionId=@SectionId ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND VerificationStaffUserId=@VerificationStaffUserId ");

                command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 AND Status<>@Status2 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = SetStatusEnum.Pending_Categorization.ToString();

                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters["@Status2"].Value = SetStatusEnum.Categorization_Failed.ToString();
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(acknowledgeNumber.Trim()))
            {
                sqlStatement.Append("AND AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");

                command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            //Edit By Edward, 30.10.2013 Optimizing Listings
            //Edit By Edward, Added Order By 08.10.2013
            //sqlStatement.Append(" GROUP BY Id, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ");
            //sqlStatement.Append(" GROUP BY Id, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ORDER BY VerificationDateIn DESC");
            sqlStatement.Append(" GROUP BY Id, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, Status, VerificationOIC, SetNo, SectionId, Channel, ImportedOIC ORDER BY VerificationDateIn DESC");
            //sqlStatement.Append(" GROUP BY Id, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Status, VerificationOIC, SetNo, SectionId, Channel, ImportedOIC ORDER BY VerificationDateIn DESC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }
        #endregion

        public static DataTable GetAllSetsByVerificationDateIn(DateTime verificationDateIn, string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ");
            sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("WHERE (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @VerificationDateIn)) AND ");
            sqlStatement.Append("SectionId=@SectionId ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND VerificationStaffUserId=@VerificationStaffUserId ");

                command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 AND Status<>@Status2 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = SetStatusEnum.Pending_Categorization.ToString();

                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters["@Status2"].Value = SetStatusEnum.Categorization_Failed.ToString();
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(acknowledgeNumber.Trim()))
            {
                sqlStatement.Append("AND AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");

                command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = verificationDateIn;

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append(" GROUP BY ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, DateAssigned, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ProcessingStartDate, ProcessingEndDate, ImportedOIC ");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }
        

        public static DataTable GetAllSetsVerificationDates(string channel, SetStatusEnum? status, string referenceNumber, Guid? verificationOicUserId,
            DateTime? dateInFrom, DateTime? dateInTo, int sectionId, int docAppId, string nric, string acknowledgeNumber)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, VerificationDateIn)) AS VerificationDateIn ");
            sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("WHERE SectionId=@SectionId ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (verificationOicUserId.HasValue)
            {
                sqlStatement.Append("AND VerificationStaffUserId=@VerificationStaffUserId ");

                command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
                command.Parameters["@VerificationStaffUserId"].Value = verificationOicUserId.Value;
            }

            if (status != null)
            {
                sqlStatement.Append("AND Status=@Status ");

                command.Parameters.Add("@Status", SqlDbType.VarChar);
                command.Parameters["@Status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND Status<>@Status1 AND Status<>@Status2 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = SetStatusEnum.Pending_Categorization.ToString();

                command.Parameters.Add("@Status2", SqlDbType.VarChar);
                command.Parameters["@Status2"].Value = SetStatusEnum.Categorization_Failed.ToString();
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(acknowledgeNumber.Trim()))
            {
                sqlStatement.Append("AND AcknowledgeNumber LIKE '%' + @AcknowledgeNumber + '%' ");
                    
                command.Parameters.Add("@AcknowledgeNumber", SqlDbType.VarChar);
                command.Parameters["@AcknowledgeNumber"].Value = acknowledgeNumber.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append(" ORDER BY VerificationDateIn DESC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get Pending Assignment Report data
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentCountByDate(DateTime dateIn)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //sqlStatement.Append("SELECT  ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) = DATEADD(DD, 0, Convert(DATE, @date))) AS EndDateCount, ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) = DATEADD(DD, -1, Convert(DATE, @date))) AS EndDateMinusOneCount, ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) = DATEADD(DD, -2, Convert(DATE, @date))) AS EndDateMinusTwoCount, ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) = DATEADD(DD, -3, Convert(DATE, @date))) AS EndDateMinusThreeCount, ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) = DATEADD(DD, -4, Convert(DATE, @date))) AS EndDateMinusFourCount, ");
            //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE Convert(DATE, ImportedOn) < DATEADD(DD, -4, Convert(DATE, @date))) AS EndDateBeforeMinusFourCount");

            sqlStatement.Append("SELECT COUNT(*) AS Count FROM DocSet WHERE Status='New' AND Convert(DATE, ImportedOn) = Convert(DATE, @date) ");

            command.Parameters.Add("@date", SqlDbType.DateTime);
            command.Parameters["@date"].Value = dateIn;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetPendingAssignmentCountBeyondLastDate(DateTime dateIn)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT COUNT(*) AS Count FROM DocSet WHERE Status='New' AND Convert(DATE, ImportedOn) < Convert(DATE, @date) ");

            command.Parameters.Add("@date", SqlDbType.DateTime);
            command.Parameters["@date"].Value = dateIn;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get pending assignments sets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentSets(string channel, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            
            //Edit by Edward 30.10.2013 use vVerificationGetAllSets Optimizing Listings
            //Added Top 2000 by Edward 29.10.2013
            //sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedOIC ");
            //sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedOIC ");
            //sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("SELECT TOP 2000 ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut,  Address,  Status, VerificationOIC, SetNo, SectionId, ImportedOIC ");
            sqlStatement.Append("FROM vVerificationGetAllSets ");
            sqlStatement.Append("WHERE SectionId=@SectionId AND Status=@Status ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }
            //Edited By Edward 30.10.2013 Optimizing Listings
            //Edited By Edward 08.10.2013
            //sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedOIC ");
            //sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedOIC ORDER BY VerificationDateIn DESC");
            sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address,  Status, VerificationOIC, SetNo,  SectionId,  Id, ImportedOIC ORDER BY VerificationDateIn DESC");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }
        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Get Earliest DocSet for a DocApp with verified as status with a filtered docSetId
        /// </summary>
        /// <param name="docAppId"></param>
        /// <param name="filterDocSetId"></param>
        /// <returns></returns>
        //public static DataTable GetEarliestVerifiedDocSetByDocAppId(int docAppId, int filterDocSetId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT TOP 1 * FROM DocSet WHERE ID IN ");
        //    sqlStatement.Append("(SELECT DocSetId FROM SetApp WHERE DocAppId=@docAppId AND DocSetId <> @filterDocSetId) AND Status = 'Verified' ");
        //    sqlStatement.Append("ORDER BY VerificationDateIn ");

        //    command.Parameters.Add("@docAppId", SqlDbType.Int);
        //    command.Parameters["@docAppId"].Value = docAppId;

        //    command.Parameters.Add("@filterDocSetId", SqlDbType.Int);
        //    command.Parameters["@filterDocSetId"].Value = filterDocSetId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public static DataTable GetEarliestVerifiedDocSetByDocAppId(int docAppId, int filterDocSetId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@docAppId", SqlDbType.Int);
            command.Parameters["@docAppId"].Value = docAppId;

            command.Parameters.Add("@filterDocSetId", SqlDbType.Int);
            command.Parameters["@filterDocSetId"].Value = filterDocSetId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSet_GetEarliestVerifiedDocSetByDocAppId";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }
        #endregion

        public static DataTable GetPendingAssignmentSetsVerificationDates(string channel, string referenceNumber, DateTime? dateInFrom,
            DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT (CONVERT(DATE, VerificationDateIn)) AS VerificationDateIn ");
            sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("WHERE SectionId=@SectionId AND Status=@Status ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append("ORDER BY VerificationDateIn DESC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        /// <summary>
        /// Get pending assignments sets
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        /// <param name="referenceNumber"></param>
        /// <param name="dateInFrom"></param>
        /// <returns></returns>
        public static DataTable GetPendingAssignmentSetsByVerificationDateIn(DateTime verificationDateIn, string channel,
            string referenceNumber, DateTime? dateInFrom, DateTime? dateInTo, int sectionId, SetStatusEnum? status, int docAppId, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT ID, VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, ImportedOIC ");
            sqlStatement.Append("FROM vDocSet ");
            sqlStatement.Append("WHERE (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @VerificationDateIn)) AND ");
            sqlStatement.Append("SectionId=@SectionId AND Status=@Status ");

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            if (docAppId != -1)
            {
                sqlStatement.Append("AND DocAppId=@DocAppId ");

                command.Parameters.Add("@DocAppId", SqlDbType.Int);
                command.Parameters["@DocAppId"].Value = docAppId;
            }

            if (!channel.Equals("-1"))
            {
                sqlStatement.Append("AND Channel=@Channel ");

                command.Parameters.Add("@Channel", SqlDbType.VarChar);
                command.Parameters["@Channel"].Value = channel;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (!String.IsNullOrEmpty(referenceNumber))
            {
                sqlStatement.Append("AND RefNo LIKE '%' + @RefNo + '%' ");

                command.Parameters.Add("@RefNo", SqlDbType.VarChar);
                command.Parameters["@RefNo"].Value = referenceNumber;
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn BETWEEN @DateInFrom AND @DateInTo ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn >= @DateInFrom ");

                command.Parameters.Add("@DateInFrom", SqlDbType.DateTime);
                command.Parameters["@DateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND VerificationDateIn <= @DateInTo ");

                command.Parameters.Add("@DateInTo", SqlDbType.DateTime);
                command.Parameters["@DateInTo"].Value = dateInTo.Value;
            }

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = verificationDateIn;

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append("AND ID IN (");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN SetDocRef ON Doc.Id = SetDocRef.DocId ");
                sqlStatement.Append("INNER JOIN DocPersonal ON SetDocRef.DocPersonalId = DocPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON DocPersonal.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append("UNION  ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");

                sqlStatement.Append(")");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            sqlStatement.Append(" GROUP BY VerificationStaffUserId, VerificationDateIn, VerificationDateOut, Address, ImportedBy, Status, VerificationOIC, SetNo, Source, SectionId, Channel, Id, ImportedOIC");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM

        //public static DataTable GetVerificationOfficerForSetAssignment()
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT Profile.UserId, Profile.Name ");
        //    sqlStatement.Append("FROM Profile ");
        //    sqlStatement.Append("INNER JOIN aspnet_UsersInRoles ON Profile.UserId = aspnet_UsersInRoles.UserId ");
        //    sqlStatement.Append("INNER JOIN aspnet_Roles ON aspnet_UsersInRoles.RoleId = aspnet_Roles.RoleId AND aspnet_Roles.RoleName = 'Verification Officer - AAD' ");
        //    sqlStatement.Append("ORDER BY Profile.Name");

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public static DataTable GetVerificationOfficerForSetAssignment()
        {
            SqlCommand command = new SqlCommand();            
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSet_GetVerificationOfficerForSetAssignment";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        #endregion

        #endregion

        #region Added By Edward 2014.04.08 Optimize Slowness

        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        /// <summary>
        /// Used in DocSetDb IsSetConfirmed, AllowVerificationSaveDateWithoutHouseholdCheck
        /// </summary>
        /// <param name="setId"></param>
        /// <returns></returns>
        //public DataTable GetStatusVerificationStaffUserIdById(int setId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT Status, VerificationStaffUserId ");
        //    sqlStatement.Append("FROM DocSet ");
        //    sqlStatement.Append("WHERE Id = @Id ");

        //    command.Parameters.Add("@Id", SqlDbType.Int);
        //    command.Parameters["@Id"].Value = setId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public DataTable GetStatusVerificationStaffUserIdById(int setId)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@Id", SqlDbType.Int);
            command.Parameters["@Id"].Value = setId;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSet_GetStatusVerificationStaffUserIdById";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }
        #endregion


        #region Added By Edward 2014/04/29
        public static DataTable GetVerificationOICForReportDetails(Guid? verificationUserId, int docAppId, string refNo, 
            SetStatusEnum? status, DateTime? dateInFrom, DateTime? dateInTo, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //Add Top 500 By Edward 29.10.2013
            //updated by Andrew (16/1/2013) to show Completeness OIC Name in Assignment Report          
            //sqlStatement.Append("SELECT DocApp.*, Profile.Name As CompletenessOIC FROM DocApp ");
            sqlStatement.Append("SELECT TOP 500 DocSet.SetNo, DocSet.Id, DocSet.VerificationDateIn, DocSet.VerificationStaffUserId, DocSet.Status, DocSet.DateAssigned, ");
            sqlStatement.Append("Profile.Name As VerificationOIC, DocApp.RefNo, DocApp.PeOIC, DocApp.CaseOIC FROM DocSet ");
            sqlStatement.Append("INNER JOIN Profile ON DocSet.VerificationStaffUserId = Profile.UserId ");
            sqlStatement.Append("INNER JOIN SetApp ON SetApp.DocSetId = DocSet.Id  ");
            sqlStatement.Append("INNER JOIN DocApp ON DocApp.Id = setapp.DocAppId ");
            sqlStatement.Append("WHERE RefType='HLE' ");

            if (verificationUserId.HasValue)
            {
                sqlStatement.Append("AND VerificationStaffUserId=@userId ");
                command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier);
                command.Parameters["@userId"].Value = verificationUserId.Value;
            }


            if (docAppId >= 0)
            {
                //added by Andrew (16/1/2013)
                sqlStatement.Append("AND DocApp.Id=@docAppId ");

                command.Parameters.Add("@docAppId", SqlDbType.Int);
                command.Parameters["@docAppId"].Value = docAppId;
            }
            else if (docAppId <= 0 && !String.IsNullOrEmpty(refNo))
            {
                sqlStatement.Append("AND RefNo=@refNo ");

                command.Parameters.Add("@refNo", SqlDbType.NVarChar);
                command.Parameters["@refNo"].Value = refNo;
            }

            if (status.HasValue)
            {
                sqlStatement.Append("AND DocSet.Status=@status ");

                command.Parameters.Add("@status", SqlDbType.NVarChar);
                command.Parameters["@status"].Value = status;
            }
            else
            {
                sqlStatement.Append("AND DocSet.Status<>@Status1 ");

                command.Parameters.Add("@Status1", SqlDbType.VarChar);
                command.Parameters["@Status1"].Value = AppStatusEnum.Pending_Documents.ToString();
            }

            if (dateInFrom.HasValue && dateInTo.HasValue)
            {
                sqlStatement.Append("AND DocSet.DateAssigned BETWEEN @dateInFrom AND @dateInTo ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }
            else if (dateInFrom.HasValue && !dateInTo.HasValue)
            {
                sqlStatement.Append("AND DocSet.DateAssigned >= @dateInFrom ");

                command.Parameters.Add("@dateInFrom", SqlDbType.DateTime);
                command.Parameters["@dateInFrom"].Value = dateInFrom.Value;
            }
            else if (dateInTo.HasValue && !dateInFrom.HasValue)
            {
                sqlStatement.Append("AND DocSet.DateAssigned <= @dateInTo ");

                command.Parameters.Add("@dateInTo", SqlDbType.DateTime);
                command.Parameters["@dateInTo"].Value = dateInTo.Value;
            }

            //nric check
            nric = nric.Trim();
            if (!String.IsNullOrEmpty(nric))
            {
                sqlStatement.Append(" AND DocApp.Id IN (");
                sqlStatement.Append("SELECT DocAppId FROM SetApp WHERE DocSetId IN ");
                sqlStatement.Append("( ");
                sqlStatement.Append("SELECT Doc.DocSetId FROM Doc  ");
                sqlStatement.Append("INNER JOIN AppDocRef ON doc.Id = AppDocRef.DocId  ");
                sqlStatement.Append("INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id  ");
                sqlStatement.Append("INNER JOIN DocSet ON Doc.DocSetId = DocSet.Id WHERE Nric LIKE '%' + @nric + '%' ");
                sqlStatement.Append(") ");

                sqlStatement.Append(") ");

                command.Parameters.Add("@nric", SqlDbType.VarChar);
                command.Parameters["@nric"].Value = nric;
            }

            //Added Order by Edward 29.10.2013
            sqlStatement.Append(" ORDER BY VerificationDateIn DESC ");
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        #region Batch Assignment

        #region commented by edward 2015/02/09 
        //Added By Edward 2014/04/30    Batch ASsignment
        //public static DataTable GetPendingHleGreaterThanLimitCounts(DateTime dateIn, string status, int sectionId )
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");

        //    //sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN  ");
        //    //sqlStatement.Append("(SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'A'  ");
        //    //sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneACount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'A'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneACount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'B'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneBCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'C'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneCCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'D'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneDCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'E'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneECount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'F'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneFCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'H'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneHCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'L'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneLCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'N'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneNCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'T'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneTCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND SUBSTRING(RefNo, 4, 1) = 'X'  ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS LaneXCount, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) < CONVERT(DATE, @dateIn)) AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) ");
        //    sqlStatement.Append("AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS Total");

        //    command.Parameters.Add("@dateIn", SqlDbType.DateTime);
        //    command.Parameters["@dateIn"].Value = dateIn;

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@SectionId", SqlDbType.Int);
        //    command.Parameters["@SectionId"].Value = sectionId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}
        #endregion

        //Modified by Edward, adding risk field and using stored procedure
        public static DataTable GetPendingHleGreaterThanLimitCounts(DateTime dateIn, string status, int sectionId)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = dateIn;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Verification_BatchAssignment_GetPendingHleGreaterThanLimitCounts";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        //Added By Edward 2014/04/30    Batch ASsignment       
        #region commented by Edward 2015/02/09
        //public static DataTable GetPendingHleNoPendingDoc(string status, int sectionId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'A')))) AS LaneACount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'B')))) AS LaneBCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'C')))) AS LaneCCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'D')))) AS LaneDCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'E')))) AS LaneECount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'F')))) AS LaneFCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'H')))) AS LaneHCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'L')))) AS LaneLCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'N')))) AS LaneNCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'T')))) AS LaneTCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE HasPendingDoc = 0 ");
        //    sqlStatement.Append("AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-') AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'X')))) AS LaneXCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND SectionId = @SectionId AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ");
        //    sqlStatement.Append("(SELECT Id FROM DocApp WHERE HasPendingDoc = 0 AND RefType = 'HLE' AND (PeOIC IS NULL OR PeOIC = '' OR PeOIC = 'COS' OR PeOIC = '-')))) AS Total");


        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@SectionId", SqlDbType.Int);
        //    command.Parameters["@SectionId"].Value = sectionId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}
        #endregion

        //Modified by Edward 2015/02/09 For adding risk logic and using stored procedures
        public static DataTable GetPendingHleNoPendingDoc(string status, int sectionId)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Verification_BatchAssignment_GetPendingHleNoPendingDoc";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        //Added By Edward 6/5/2014 Batch Assignment 
        #region Commented by Edward
        //This method gets per date
        //public static DataTable GetPendingHleCounts(DateTime dateIn, string status, int sectionId)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT  ");
        //    sqlStatement.Append("(SELECT (CONVERT(DATE, @dateIn))) AS DateInConverted, ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'A')))) AS LaneACount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'B')))) AS LaneBCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'C')))) AS LaneCCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'D')))) AS LaneDCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'E')))) AS LaneECount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'F')))) AS LaneFCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'H')))) AS LaneHCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'L')))) AS LaneLCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'N')))) AS LaneNCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'T')))) AS LaneTCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN ((SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND  ");
        //    sqlStatement.Append("((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE' AND SUBSTRING(RefNo, 4, 1) = 'X')))) AS LaneXCount,  ");

        //    sqlStatement.Append("(SELECT COUNT(*) FROM DocSet WHERE VerificationStaffUserId IS NULL AND Status = @status AND (CONVERT(DATE, VerificationDateIn) = CONVERT(DATE, @dateIn)) AND SectionID = @SectionId ");
        //    sqlStatement.Append("AND Id IN (SELECT DocSetId FROM SetApp WHERE DocAppId IN (SELECT Id FROM DocApp WHERE ((HasPendingDoc IS NULL) OR (HasPendingDoc = 1)) AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND RefType = 'HLE'))) AS Total");           

        //    command.Parameters.Add("@dateIn", SqlDbType.DateTime);
        //    command.Parameters["@dateIn"].Value = dateIn;

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@SectionId", SqlDbType.Int);
        //    command.Parameters["@SectionId"].Value = sectionId;

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}
        #endregion

        //Modified by Edward 2015/02/09 For adding risk logic and using stored procedures
        public static DataTable GetPendingHleCounts(DateTime dateIn, string status, int sectionId)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@VerificationDateIn", SqlDbType.DateTime);
            command.Parameters["@VerificationDateIn"].Value = dateIn;

            command.Parameters.Add("@Status", SqlDbType.VarChar);
            command.Parameters["@Status"].Value = status;

            command.Parameters.Add("@SectionId", SqlDbType.Int);
            command.Parameters["@SectionId"].Value = sectionId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Verification_BatchAssignment_GetPendingHleCounts";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }
        #region  Modified by Edward 2015/8/25 Convert to SP for simple queries to reduce Error Notifications and OOM
        //Added By Edward 6/5/2014  Batch Assignment
        //public static DataTable GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        //{
        //    SqlCommand command = new SqlCommand();
        //    StringBuilder sqlStatement = new StringBuilder();

        //    sqlStatement.Append("SELECT COUNT(*) AS LaneCount FROM DocSet WHERE Status = @status AND VerificationStaffUserId IS NULL AND Id IN ");
        //    sqlStatement.Append("(SELECT DocSetId FROM SetApp WHERE DocAppId IN ");
        //    sqlStatement.Append("(SELECT Id FROM DocApp WHERE RefType = 'HLE' AND ((PeOIC IS NULL) OR (PeOIC = '') OR (PeOIC = 'COS') OR (PeOIC = '-')) AND SUBSTRING(RefNo, 4, 1) = @lane AND (Risk <> 'L' OR Risk IS NULL)))");

        //    command.Parameters.Add("@status", SqlDbType.VarChar);
        //    command.Parameters["@status"].Value = status;

        //    command.Parameters.Add("@lane", SqlDbType.VarChar);
        //    command.Parameters["@lane"].Value = lane.ToString();

        //    using (SqlConnection connection = new SqlConnection(connString))
        //    {
        //        command.CommandText = sqlStatement.ToString();
        //        command.Connection = connection;
        //        DataSet dataSet = new DataSet();
        //        SqlDataAdapter adapter = new SqlDataAdapter(command);
        //        connection.Open();
        //        adapter.Fill(dataSet);
        //        return dataSet.Tables[0];
        //    }
        //}

        public static DataTable GetPendingHleCountsByLanes(string status, HleLanesEnum lane)
        {
            SqlCommand command = new SqlCommand();
            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

            command.Parameters.Add("@lane", SqlDbType.VarChar);
            command.Parameters["@lane"].Value = lane.ToString();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DocSet_GetPendingHleCountsByLanes";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        #endregion


        #endregion

        #endregion


        #endregion

        #region TestingPurpose
        public static DataTable GetDataBySentToCDBStatusAndDocSetStatusAndNotSystem(string status, string docSetStatus, string exclusion)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            sqlStatement.Append(" SELECT AcknowledgeNumber, Block, Channel, ConvertedToSampleDoc, DateAssigned, DepartmentId, ExpediteReason, ExpediteRemark, ExpediteRequestDate, ");
            sqlStatement.Append(" ExpediteRequester, Floor, DocSet.Id, ImportedBy, ImportedOn, IsBeingProcessed, IsUrgent, ProcessingEndDate, ProcessingStartDate, ReadyForOcr, ");
            sqlStatement.Append(" Remark, SectionId, SendToCDBAttemptCount, SendToCDBStatus, SetNo, SkipCategorization, Status, StreetId, Unit, VerificationDateIn, VerificationDateOut, ");
            sqlStatement.Append(" VerificationStaffUserId, WebServXmlContent ");
            sqlStatement.Append(" FROM DocSet INNER JOIN Profile ON DocSet.VerificationStaffUserId = Profile.UserId");
            sqlStatement.Append(" WHERE (SendToCDBStatus = @status) AND (Status = @docSetStatus)");
            sqlStatement.Append(" AND (Profile.Name <> @exclusion)");

            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

            command.Parameters.Add("@docSetStatus", SqlDbType.VarChar);
            command.Parameters["@docSetStatus"].Value = docSetStatus;

            command.Parameters.Add("@exclusion", SqlDbType.VarChar);
            command.Parameters["@exclusion"].Value = exclusion;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetDocAppAndDocData(int docSetId, string status, string imageCondition, string docTypes, string sendToCDBStatus)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //sqlStatement.Append(" SELECT DISTINCT sa.docappid as DocAppId ");
            sqlStatement.Append(" SELECT DISTINCT sa.DocAppId, ap.Nric, ap.IdType, ap.Folder, ap.Name, d.Id as DocId, ap.CustomerSourceId, ap.CustomerType");
            sqlStatement.Append(" FROM docset ds ");
            sqlStatement.Append(" JOIN setapp sa ON ds.id=sa.docsetid ");
            sqlStatement.Append(" JOIN AppPersonal ap ON sa.DocAppid=ap.DocAppId ");
            sqlStatement.Append(" JOIN AppDocRef adr ON ap.id=adr.AppPersonalId ");
            sqlStatement.Append(" JOIN Doc d ON  adr.DocId=d.Id ");
            sqlStatement.Append(" WHERE d.status=@status and d.ImageCondition=@imageCondition and d.Doctypecode <>@docTypes and d.SendtoCDBStatus <>@sendToCDBStatus");
            sqlStatement.Append(" and d.DocSetId=@docSetId ORDER BY sa.docappid ");


            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

            command.Parameters.Add("@status", SqlDbType.VarChar);
            command.Parameters["@status"].Value = status;

            command.Parameters.Add("@imageCondition", SqlDbType.VarChar);
            command.Parameters["@imageCondition"].Value = imageCondition;

            command.Parameters.Add("@docTypes", SqlDbType.VarChar);
            command.Parameters["@docTypes"].Value = docTypes;


            command.Parameters.Add("@sendToCDBStatus", SqlDbType.VarChar);
            command.Parameters["@sendToCDBStatus"].Value = sendToCDBStatus;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }


        #endregion

        #region Added by Edward Development of reports 2014/06/09
        public static DataTable GetVerificationReportMonthYear(int section, int department)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DATEPART(MONTH,VerificationDateIn) AS VMonth, DATEPART(YEAR,VerificationDateIn) AS VYear ");
            sqlStatement.Append("FROM DocSet WHERE 1=1 ");
            
            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion
            sqlStatement.Append("ORDER BY DATEPART(YEAR,VerificationDateIn) DESC, DATEPART(MONTH,VerificationDateIn) DESC  ");            

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetVerificationReport(int section, int department, int month, int year)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT DATEPART(MONTH,VerificationDateIn) AS VMonth, DATEPART(YEAR,VerificationDateIn) AS VYear, ");
            sqlStatement.Append("Channel FROM DocSet WHERE ");
            sqlStatement.Append("DATEPART(MONTH,VerificationDateIn) = @Month AND DATEPART(YEAR,VerificationDateIn) = @Year ");

            command.Parameters.Add("@Month", SqlDbType.Int);
            command.Parameters["@Month"].Value = month;
            command.Parameters.Add("@Year", SqlDbType.Int);
            command.Parameters["@Year"].Value = year;

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static int GetVerificationSetCount(int section, int department, int month, int year, string channel)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT COUNT(*) AS SetCount FROM DocSet ");
            sqlStatement.Append("WHERE DATEPART(MONTH,VerificationDateIn) = @Month AND DATEPART(YEAR,VerificationDateIn) = @Year ");
            sqlStatement.Append("AND Channel = @Channel ");

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            command.Parameters.Add("@Month", SqlDbType.Int);
            command.Parameters["@Month"].Value = month;

            command.Parameters.Add("@Year", SqlDbType.Int);
            command.Parameters["@Year"].Value = year;

            command.Parameters.Add("@Channel", SqlDbType.VarChar);
            command.Parameters["@Channel"].Value = channel;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }

        public static DataTable GetAgingRange(int section, int department)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            string strWHEN = "WHEN DATEDIFF(DAY, ISNULL(a.DateAssigned, GETDATE()),ISNULL(VerificationDateOut, GETDATE())) ";

            sqlStatement.Append("SELECT DISTINCT CASE ");
            sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 THEN 2 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 THEN 4 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 THEN 6 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 THEN 8 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 THEN 10 ", strWHEN));
            sqlStatement.Append(string.Format("{0} > 10 THEN 11 ", strWHEN));
            sqlStatement.Append("ELSE 0 END AS AgingNo, CASE ");
            sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 THEN '0 - 2' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 THEN '3 - 4' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 THEN '5 - 6' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 THEN '7 - 8' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 THEN '9 - 10' ", strWHEN));
            sqlStatement.Append(string.Format("{0} > 10 THEN '> 10' ", strWHEN));            
            sqlStatement.Append("ELSE ' - ' END AS AgingRange ");
            sqlStatement.Append("FROM DocSet a INNER JOIN SetApp b ON a.id = b.DocSetId INNER JOIN DocApp c ON c.Id = b.DocAppId WHERE 1=1 ");

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            sqlStatement.Append("ORDER BY AgingNo DESC ");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetAgingRange(int section, int department, Guid? oic)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            string strWHEN = "WHEN DATEDIFF(DAY, ISNULL(a.DateAssigned, GETDATE()),ISNULL(VerificationDateOut, GETDATE())) ";

            sqlStatement.Append("SELECT DISTINCT a.VerificationStaffUserId, CASE  ");
            sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 THEN 2 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 THEN 4 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 THEN 6 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 THEN 8 ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 THEN 10 ", strWHEN));
            sqlStatement.Append(string.Format("{0} > 10 THEN 11 ", strWHEN));
            sqlStatement.Append("ELSE 0 END AS AgingNo, CASE ");
            sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 THEN '0 - 2' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 THEN '3 - 4' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 THEN '5 - 6' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 THEN '7 - 8' ", strWHEN));
            sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 THEN '9 - 10' ", strWHEN));
            sqlStatement.Append(string.Format("{0} > 10 THEN '> 10' ", strWHEN));
            sqlStatement.Append("ELSE ' - ' END AS AgingRange ");
            sqlStatement.Append("FROM DocSet a INNER JOIN SetApp b ON a.id = b.DocSetId INNER JOIN DocApp c ON c.Id = b.DocAppId AND a.VerificationStaffUserId = @oic ");

            command.Parameters.Add("@oic", SqlDbType.UniqueIdentifier);
            command.Parameters["@oic"].Value = oic;

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            sqlStatement.Append("ORDER BY AgingNo DESC ");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetVerificationPerAging(int section, int department, int aging)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            string strWHEN = " AND DATEDIFF(DAY, ISNULL(a.DateAssigned, GETDATE()),ISNULL(VerificationDateOut, GETDATE())) ";

            sqlStatement.Append("SELECT TOP 500 VerificationDateIn, a.DateAssigned, VerificationDateOut, ISNULL(d.Name,' - ') AS VOIC, RefNo, SetNo ");
//            sqlStatement.Append("DATEDIFF(DAY,  ISNULL(a.DateAssigned, GETDATE()),ISNULL(VerificationDateOut, GETDATE())) AS Aging " );
            sqlStatement.Append("FROM DocSet a INNER JOIN SetApp b ON a.id = b.DocSetId INNER JOIN DocApp c ON c.Id = b.DocAppId ");
            sqlStatement.Append("LEFT JOIN Profile d on a.VerificationStaffUserId = d.UserId WHERE 1=1 ");
            switch (aging)
            {
                case 11:
                    sqlStatement.Append(string.Format("{0} > 10 ",strWHEN));                    
                    break;
                case 10:
                    sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 ", strWHEN));                    
                    break;
                case 8:
                    sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 ", strWHEN));                       
                    break;
                case 6:
                    sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 ", strWHEN));                         
                    break;
                case 4:
                    sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 ", strWHEN));                         
                    break;
                case 2:
                    sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 ", strWHEN));                         
                    break;                                    
                default:
                    sqlStatement.Append(string.Format("{0} < 0 ", strWHEN));                         
                    break;
            }
            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion

            sqlStatement.Append("ORDER BY VerificationDateIn DESC ");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static DataTable GetVerificationOIC(int section, int department)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            sqlStatement.Append("SELECT DISTINCT b.Name, b.UserId FROM DocSet a ");
            sqlStatement.Append("INNER JOIN Profile b ON a.VerificationStaffUserId = b.UserId ");
            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        public static int GetSetCountPerAgingPerOIC(int section, int department, Guid oic, int aging)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            string strWHEN = " AND DATEDIFF(DAY, ISNULL(ds.DateAssigned, GETDATE()),ISNULL(ds.VerificationDateOut, GETDATE())) ";

            sqlStatement.Append("SELECT COUNT(ds.Id) AS SetCount FROM DocSet ds INNER JOIN SetApp sa ON ds.id = sa.DocSetId INNER JOIN DocApp da ON da.Id = sa.DocAppId  ");
            sqlStatement.Append("WHERE ds.VerificationStaffUserId = @oic ");

            command.Parameters.Add("@oic", SqlDbType.UniqueIdentifier);
            command.Parameters["@oic"].Value = oic;

            switch (aging)
            {
                case 11:
                    sqlStatement.Append(string.Format("{0} > 10 ", strWHEN));
                    break;
                case 10:
                    sqlStatement.Append(string.Format("{0} BETWEEN 9 AND 10 ", strWHEN));
                    break;
                case 8:
                    sqlStatement.Append(string.Format("{0} BETWEEN 7 AND 8 ", strWHEN));
                    break;
                case 6:
                    sqlStatement.Append(string.Format("{0} BETWEEN 5 AND 6 ", strWHEN));
                    break;
                case 4:
                    sqlStatement.Append(string.Format("{0} BETWEEN 3 AND 4 ", strWHEN));
                    break;
                case 2:
                    sqlStatement.Append(string.Format("{0} BETWEEN 0 AND 2 ", strWHEN));
                    break;
                default:
                    sqlStatement.Append(string.Format("{0} < 0 ", strWHEN));
                    break;
            }

            #region department and section
            if (department != -1)
            {
                sqlStatement.Append("AND DepartmentId=@DepartmentId ");
                command.Parameters.Add("@DepartmentId", SqlDbType.VarChar);
                command.Parameters["@DepartmentId"].Value = department;
            }

            // Add the section query
            if (section != -1)
            {
                sqlStatement.Append("AND SectionId=@SectionId ");
                command.Parameters.Add("@SectionId", SqlDbType.VarChar);
                command.Parameters["@SectionId"].Value = section;
            }
            #endregion
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
        #endregion

        #region Added by Edward 2015/12/04 to change Folder Structure of documents to YEAR/MONTH/DAY
        public static DataTable GetYearMonthDayForFolderStructure(int docSetId)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();
            
            sqlStatement.Append(" SELECT DATEPART(YEAR, VerificationDateIn) AS fYear, DATEPART(MONTH, VerificationDateIn) AS fMonth,");
            sqlStatement.Append(" DATEPART(DAY, VerificationDateIn) AS fDay, VerificationDateIn FROM DocSet WHERE Id = @docSetId ");

            command.Parameters.Add("@docSetId", SqlDbType.Int);
            command.Parameters["@docSetId"].Value = docSetId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                return dataSet.Tables[0];
            }
        }

        #endregion

        #region  Added By Edward 2017/11/03 To Optimize Assign might reduce OOM
        public static DataTable GetMultipleDocSets(string ids)
        {
            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@DocSetIds", SqlDbType.VarChar);
            command.Parameters["@DocSetIds"].Value = ids;
            
            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Assign_GetMultipleDocSets";
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);
                if (dataSet.Tables.Count > 0)
                    return dataSet.Tables[0];
                else
                    return new DataTable();
            }
        }

        public static int AssignDocSetToUser(int id, Guid userId, Guid currentUserId)
        {

            SqlCommand command = new SqlCommand();

            command.Parameters.Add("@DocSetId", SqlDbType.Int);
            command.Parameters["@DocSetId"].Value = id;

            command.Parameters.Add("@VerificationStaffUserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@VerificationStaffUserId"].Value = userId;

            command.Parameters.Add("@CurrentUserId", SqlDbType.UniqueIdentifier);
            command.Parameters["@CurrentUserId"].Value = currentUserId;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Assign_AssignDocSetToUsers";
                command.Connection = connection;
                connection.Open();
                int result = command.ExecuteNonQuery();
                return result;
            }
        }
        #endregion
    }
}

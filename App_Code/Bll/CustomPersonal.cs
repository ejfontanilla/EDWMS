using System;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Dal;

namespace Dwms.Bll
{
    public sealed class CustomPersonal
    {
        //this Save function is for single nric doctypes
        public Boolean Save(int docId, int docRefId, string referencePersonalTable, string currentDocType, string docType, TextBox nric, TextBox name, Boolean logNameChange, RadioButtonList idType, HiddenField customerSourceId)
        {
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();

            Boolean isDocStillAttachedToOldRefId = false;

            //check if the docid is still attached to refId before the dettach and attach action.
            if (referencePersonalTable.ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                isDocStillAttachedToOldRefId = setDocRefDb.GetSetDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;
            else
                isDocStillAttachedToOldRefId = appDocRefDb.GetAppDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;

            if (!isDocStillAttachedToOldRefId)
                return false;

            if (!currentDocType.ToLower().Equals(docType.ToLower()))
            {
                //delete old metadata
                MetaDataDb metaDataDb = new MetaDataDb();
                metaDataDb.DeleteByDocID(docId, false, false);
            }

            if (referencePersonalTable.ToUpper().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByAppDocRefId(docRefId);

                if (appPersonals.Rows.Count > 0)
                {
                    AppPersonal.AppPersonalRow appPersonalRow = appPersonals[0];

                    //detach the old link to old appPersonal record
                    AppDocRef.AppDocRefDataTable appDocRefs = appDocRefDb.GetAppDocRefById(docRefId);
                    AppDocRef.AppDocRefRow appDocRefRow = appDocRefs[0];
                    appDocRefDb.DeleteByDocId(appDocRefRow.DocId);

                    AppPersonal.AppPersonalDataTable appPersonalsByNricDocAppId = appPersonalDb.GetAppPersonalByNricAndDocAppId(nric.Text.Trim(), appPersonalRow.DocAppId);

                    // if record exits, attach it to the appPersonal.
                    if (appPersonalsByNricDocAppId.Rows.Count > 0)
                    {
                        AppPersonal.AppPersonalRow appPersonalsByNricSetAppIdRow = appPersonalsByNricDocAppId[0];
                        appDocRefDb.Insert(appPersonalsByNricSetAppIdRow.Id, docId);

                        //update Name only if the nric is not part of householdstructure
                        if (!appPersonalDb.IsAppNric(nric.Text.Trim(), appPersonalsByNricSetAppIdRow.DocAppId))
                        {
                            appPersonalDb.Update(appPersonalsByNricSetAppIdRow.Id, string.Empty, name.Text.Trim(), string.Empty, string.Empty, string.Empty);

                            //logname change action
                            if (logNameChange && IsAppPersonalNameChanged(appPersonalsByNricSetAppIdRow, name))
                                LogDocumentNameChange(docId, name.Text.Trim(), appPersonalsByNricSetAppIdRow.IsNameNull() ? string.Empty : appPersonalsByNricSetAppIdRow.Name);
                        }

                        #region Modified by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
                        //update customertype, customersourceid and idtype
                        //appPersonalDb.Update(appPersonalsByNricSetAppIdRow.Id, idType.SelectedValue, customerSourceId.Value);
                        string strIdType = string.Empty;
                        if (!string.IsNullOrEmpty(nric.Text.Trim()))
                            strIdType = idType.SelectedValue;
                        appPersonalDb.Update(appPersonalsByNricSetAppIdRow.Id, strIdType, customerSourceId.Value);
                        #endregion
                    }
                    else
                    {
                        //create new AppPersonal record
                        int newAppPersonalId = appPersonalDb.Insert(appPersonalRow.DocAppId, nric.Text.Trim().ToUpper(), name.Text.Trim(), string.Empty, string.Empty, string.Empty, string.Empty, DocFolderEnum.Others.ToString(), null, 0, string.Empty);

                        //attach to the document
                        appDocRefDb.Insert(newAppPersonalId, docId);

                        //logname change action
                        if (logNameChange)
                            LogDocumentNameChange(docId, name.Text.Trim(), WebStringEnum.Empty.ToString());

                        #region Modified by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
                        //update customertype, customersourceid and idtype
                        //appPersonalDb.Update(newAppPersonalId, idType.SelectedValue, customerSourceId.Value);
                        string strIdType = string.Empty;
                        if (!string.IsNullOrEmpty(nric.Text.Trim()))
                            strIdType = idType.SelectedValue;
                        appPersonalDb.Update(newAppPersonalId, strIdType, customerSourceId.Value);
                        #endregion
                    }
                }
            }
            else
            {
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalBySetDocRefId(docRefId);

                if (docPersonals.Rows.Count > 0)
                {
                    DocPersonal.DocPersonalRow docPersonalRow = docPersonals[0];

                    //detach the old link to old DocPersonal record
                    SetDocRef.SetDocRefDataTable setDocRefs = setDocRefDb.GetSetDocRefById(docRefId);
                    SetDocRef.SetDocRefRow setDocRefRow = setDocRefs[0];
                    setDocRefDb.DeleteByDocId(setDocRefRow.DocId);

                    DocPersonal.DocPersonalDataTable docPersonalsByNricDocSetId = docPersonalDb.GetDocPersonalByNricFolderAndDocSetId(nric.Text.Trim(), docPersonalRow.DocSetId, docPersonalRow.Folder);

                    // if record exits, attach it to the DocPersonal.
                    if (docPersonalsByNricDocSetId.Rows.Count > 0)
                    {
                        DocPersonal.DocPersonalRow docPersonalsByNricDocSetIdRow = docPersonalsByNricDocSetId[0];
                        setDocRefDb.Insert(docPersonalsByNricDocSetIdRow.Id, docId);

                        //updateName
                        docPersonalDb.UpdateName(docPersonalsByNricDocSetIdRow.Id, name.Text.Trim());

                        //logname change action
                        if (logNameChange && IsDocPersonalNameChanged(docPersonalsByNricDocSetIdRow, name))
                            LogDocumentNameChange(docId, name.Text.Trim(), docPersonalsByNricDocSetIdRow.IsNameNull() ? string.Empty : docPersonalsByNricDocSetIdRow.Name);

                        //update customersourceid and idtype
                        docPersonalDb.Update(docPersonalsByNricDocSetIdRow.Id, idType.SelectedValue, customerSourceId.Value);
                    }
                    else
                    {
                        //create new DocPersonal record
                        int newDocPersonalId = docPersonalDb.Insert(docPersonalRow.DocSetId, nric.Text.Trim().ToUpper(), name.Text.Trim(), docPersonalRow.Folder, null);

                        //attach to the document
                        setDocRefDb.Insert(newDocPersonalId, docId);

                        //logname change action
                        if (logNameChange)
                            LogDocumentNameChange(docId, name.Text.Trim(), string.Empty);

                        //update customersourceid and idtype
                        docPersonalDb.Update(newDocPersonalId, idType.SelectedValue, customerSourceId.Value);
                    }
                }
            }

            return true;
        }

        protected void LogDocumentNameChange(int docId, string newName, string prevName)
        {
            if (prevName == null)
                prevName = WebStringEnum.Empty.ToString();
            else if (string.IsNullOrEmpty(prevName.Trim()))
                prevName = WebStringEnum.Empty.ToString();
            else
                prevName = prevName.Trim();

            if (newName == null)
                newName = WebStringEnum.Empty.ToString();
            else if (string.IsNullOrEmpty(newName.Trim()))
                newName = WebStringEnum.Empty.ToString();
            else
                newName = newName.Trim();

            DocDb docDb = new DocDb();
            Doc.DocDataTable doc = docDb.GetDocById(docId);
            Doc.DocRow docRow = doc[0];

            string url = HttpContext.Current.Request.Url.ToString().ToLower().Trim();
            LogTypeEnum logTypeEnum = new LogTypeEnum();

            if (url.Contains("/verification/"))
                logTypeEnum = LogTypeEnum.D;
            else
                logTypeEnum = LogTypeEnum.C;

            MembershipUser user = Membership.GetUser();
            LogActionDb logActionDb = new LogActionDb();

            if (!prevName.Equals(newName))
                logActionDb.Insert((Guid)user.ProviderUserKey, LogActionEnum.Document_REPLACE1_metadata_name_changed_from_REPLACE2_to_REPLACE3, docRow.DocTypeCode + Utility.FormatId(docId.ToString()), prevName, newName, string.Empty, logTypeEnum, docId);
        }

        protected Boolean IsAppPersonalNameChanged(AppPersonal.AppPersonalRow appPersonalRow, TextBox name)
        {
            Boolean isNameChanged = false;
            if (!appPersonalRow.IsNameNull())
                isNameChanged = !appPersonalRow.Name.Trim().Equals(name.Text.Trim());

            return ((appPersonalRow.IsNameNull() && name.Text.Trim().Length > 0) || isNameChanged);
        }

        protected Boolean IsDocPersonalNameChanged(DocPersonal.DocPersonalRow docPersonalRow, TextBox name)
        {
            Boolean isNameChanged = false;
            if (!docPersonalRow.IsNameNull())
                isNameChanged = !docPersonalRow.Name.Trim().Equals(name.Text.Trim());

            return ((docPersonalRow.IsNameNull() && name.Text.Trim().Length > 0) || isNameChanged);
        }

        //this Load function is for single nric doctypes
        public void Load(int docId, string referencePersonalTable, TextBox nric, TextBox name, HiddenField nricId)
        {
            //check if the referencePersonaltable is AppPersonal
            if (referencePersonalTable.ToUpper().Trim().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocId(docId);

                foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                {
                    nric.Text = appPersonalRow.IsNricNull() ? string.Empty : appPersonalRow.Nric.Trim();
                    name.Text = appPersonalRow.IsNameNull() ? string.Empty : appPersonalRow.Name.Trim();
                    nricId.Value = appPersonalRow.Id.ToString();
                }
            }
            else // if DocPersonal
            {
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalByDocId(docId);

                foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals.Rows)
                {
                    nric.Text = docPersonalRow.IsNricNull() ? string.Empty : docPersonalRow.Nric.Trim();
                    name.Text = docPersonalRow.IsNameNull() ? string.Empty : docPersonalRow.Name.Trim();
                    nricId.Value = docPersonalRow.Id.ToString();
                }
            }
        }

        //this Load function is for single nric doctypes
        public void Load(int docId, string referencePersonalTable, TextBox nric, TextBox name, HiddenField nricId, RadioButtonList idType, HiddenField customerSourceId)
        {
            //check if the referencePersonaltable is AppPersonal
            if (referencePersonalTable.ToUpper().Trim().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocId(docId);

                foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                {
                    nric.Text = appPersonalRow.IsNricNull() ? string.Empty : appPersonalRow.Nric.Trim();
                    name.Text = appPersonalRow.IsNameNull() ? string.Empty : appPersonalRow.Name.Trim();

                    #region Modified by Edward at 2015/8/6 if NRIC is blank, load idtype blank - Reducing CDB Errors
                    //idType.SelectedValue = appPersonalRow.IsIdTypeNull() ? IDTypeEnum.UIN.ToString() : string.IsNullOrEmpty(appPersonalRow.IdType.Trim()) ? IDTypeEnum.UIN.ToString() : appPersonalRow.IdType.Trim();
                    //if (!string.IsNullOrEmpty(nric.Text))
                    //    idType.SelectedValue = appPersonalRow.IsIdTypeNull() ? IDTypeEnum.UIN.ToString() : string.IsNullOrEmpty(appPersonalRow.IdType.Trim()) ? IDTypeEnum.UIN.ToString() : appPersonalRow.IdType.Trim();
                    if (!string.IsNullOrEmpty(appPersonalRow.IsIdTypeNull() ? string.Empty : appPersonalRow.IdType.Trim()))   //modified by edward 2-15
                        idType.SelectedValue = appPersonalRow.IdType.Trim();
                    #endregion
                    
                    customerSourceId.Value = appPersonalRow.IsCustomerSourceIdNull() ? string.Empty : appPersonalRow.CustomerSourceId.Trim();
                    nricId.Value = appPersonalRow.Id.ToString();
                }
            }
            else // if DocPersonal
            {
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalByDocId(docId);

                foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals.Rows)
                {
                    nric.Text = docPersonalRow.IsNricNull() ? string.Empty : docPersonalRow.Nric.Trim();
                    name.Text = docPersonalRow.IsNameNull() ? string.Empty : docPersonalRow.Name.Trim();
                    idType.SelectedValue = docPersonalRow.IsIdTypeNull() ? IDTypeEnum.UIN.ToString() : string.IsNullOrEmpty(docPersonalRow.IdType.Trim()) ? IDTypeEnum.UIN.ToString() :  docPersonalRow.IdType.Trim();
                    customerSourceId.Value = docPersonalRow.IsCustomerSourceIdNull() ? string.Empty : docPersonalRow.CustomerSourceId.Trim();
                    nricId.Value = docPersonalRow.Id.ToString();
                }
            }
        }

        //this Load function is for single nric doctypes
        public void LoadName(int docId, string referencePersonalTable, TextBox name, HiddenField nricId)
        {
            //check if the referencePersonaltable is AppPersonal
            if (referencePersonalTable.ToUpper().Trim().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocId(docId);

                foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                {
                    name.Text = appPersonalRow.IsNameNull() ? string.Empty : appPersonalRow.Name.Trim();
                }
            }
            else // if DocPersonal
            {
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalByDocId(docId);

                foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals.Rows)
                {
                    name.Text = docPersonalRow.IsNameNull() ? string.Empty : docPersonalRow.Name.Trim();
                }
            }
        }

        //this Load function is for double nric doctypes
        public void Load(int docId, string referencePersonalTable, TextBox nricRequestor, TextBox nricSpouse, TextBox nameRequestor, TextBox nameSpouse, HiddenField nricRequestorId, HiddenField nricSpouseId)
        {
            //check if the referencePersonaltable is AppPersonal
            if (referencePersonalTable.ToUpper().Trim().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonalDb appPersonalDb = new AppPersonalDb();
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocId(docId);

                foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                {
                    if (!appPersonalRow.IsRelationshipNull())
                    {
                        if (appPersonalRow.Relationship.ToLower().Trim().Equals(RelationshipEnum.Requestor.ToString().ToLower().Trim()))
                        {
                            nricRequestor.Text = appPersonalRow.IsNricNull() ? string.Empty : appPersonalRow.Nric.Trim();
                            nameRequestor.Text = appPersonalRow.IsNameNull() ? string.Empty : appPersonalRow.Name.Trim();
                            nricRequestorId.Value = appPersonalRow.Id.ToString();
                        }
                        else if (appPersonalRow.Relationship.ToLower().Trim().Equals(RelationshipEnum.Spouse.ToString().ToLower().Trim()))
                        {
                            nricSpouse.Text = appPersonalRow.IsNricNull() ? string.Empty : appPersonalRow.Nric.Trim();
                            nameSpouse.Text = appPersonalRow.IsNameNull() ? string.Empty : appPersonalRow.Name.Trim();
                            nricSpouseId.Value = appPersonalRow.Id.ToString();
                        }
                    }
                }
            }
            else // if DocPersonal
            {
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalByDocId(docId);

                foreach (DocPersonal.DocPersonalRow docPersonalRow in docPersonals.Rows)
                {
                    if (!docPersonalRow.IsRelationshipNull())
                    {
                        if (docPersonalRow.Relationship.ToLower().Trim().Equals(RelationshipEnum.Requestor.ToString().ToLower().Trim()))
                        {
                            nricRequestor.Text = docPersonalRow.IsNricNull() ? string.Empty : docPersonalRow.Nric.Trim();
                            nameRequestor.Text = docPersonalRow.IsNameNull() ? string.Empty : docPersonalRow.Name.Trim();
                            nricRequestorId.Value = docPersonalRow.Id.ToString();
                        }
                        else if (docPersonalRow.Relationship.ToLower().Trim().Equals(RelationshipEnum.Spouse.ToString().ToLower().Trim()))
                        {
                            nricSpouse.Text = docPersonalRow.IsNricNull() ? string.Empty : docPersonalRow.Nric.Trim();
                            nameSpouse.Text = docPersonalRow.IsNameNull() ? string.Empty : docPersonalRow.Name.Trim();
                            nricSpouseId.Value = docPersonalRow.Id.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attach and dettach the docpersonal/apppersonal reference
        /// </summary>
        /// <param name="RadTreeView1"></param>
        /// <param name="sourceNode"></param>
        /// <param name="destNode"></param>
        /// <param name="docId"></param>
        public Boolean AttachAndDettachPersonalReference(int docRefId, RadTreeNode sourceNode, RadTreeNode destNode, string nric, Boolean preserveNric, int docId, int docSetId, int docAppId, string destinationFolder)
        {
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            string relationship = string.Empty;

            DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalBySetDocRefId(docRefId);
            AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByAppDocRefId(docRefId);

            Boolean isDocStillAttachedToOldRefId = false;

            //check if the docid is still attached to refId before the dettach and attach action.
            if (sourceNode.Attributes["referencePersonalTable"].ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                isDocStillAttachedToOldRefId = setDocRefDb.GetSetDocRefByDocIdAndRefId(docId, docRefId).Rows.Count >0;
            else
                isDocStillAttachedToOldRefId = appDocRefDb.GetAppDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;

            if (!isDocStillAttachedToOldRefId)
                return false;

            //dettach the doc reference
            if (sourceNode.Attributes["referencePersonalTable"].ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                setDocRefDb.DeleteByDocId(docId);
            else
                appDocRefDb.DeleteByDocId(docId);

            //if destination is DOCPERSONAL
            if (destNode.Category.Trim().ToUpper().Equals("DEFAULT FOLDER") || destNode.Category.Trim().ToUpper().Equals("UNIDENTIFIED") || destNode.Category.Trim().ToUpper().Equals("DEFAULT FOLDER NRIC"))
            {
                if (sourceNode.Attributes["referencePersonalTable"].ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                {
                    if (docPersonals.Rows.Count > 0)
                    {
                        DocPersonal.DocPersonalRow docPersonalRow = docPersonals[0];

                        //handle nric and relationship
                        if (string.IsNullOrEmpty(nric) && preserveNric)
                            nric = docPersonalRow.IsNricNull() ? "" : docPersonalRow.Nric.Trim();

                        relationship = docPersonalRow.IsRelationshipNull() ? "" : docPersonalRow.Relationship.Trim();

                        AttachDocPersonalReference(docId, docSetId, nric, destinationFolder, relationship);
                    }
                }
                else
                {
                    if (appPersonals.Rows.Count > 0)
                    {
                        AppPersonal.AppPersonalRow appPersonalRow = appPersonals[0];

                        //handle nric and relationship
                        if (string.IsNullOrEmpty(nric) && preserveNric)
                            nric = appPersonalRow.IsNricNull() ? "" : appPersonalRow.Nric.Trim();

                        relationship = appPersonalRow.IsRelationshipNull() ? "" : appPersonalRow.Relationship.Trim();

                        AttachDocPersonalReference(docId, docSetId, nric, destinationFolder, relationship);
                    }
                }

            }
            else //if destination is APPPERSONAL
            {
                if (sourceNode.Attributes["referencePersonalTable"].ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                {
                    if (docPersonals.Rows.Count > 0)
                    {
                        DocPersonal.DocPersonalRow docPersonalRow = docPersonals[0];

                        //handle nric and relationship
                        if (string.IsNullOrEmpty(nric) && preserveNric)
                            nric = docPersonalRow.IsNricNull() ? "" : docPersonalRow.Nric.Trim();

                        relationship = docPersonalRow.IsRelationshipNull() ? "" : docPersonalRow.Relationship.Trim();

                        AttachAppPersonalReference(docId, docAppId, nric, destinationFolder, relationship);
                    }
                }
                else
                {
                    if (appPersonals.Rows.Count > 0)
                    {
                        AppPersonal.AppPersonalRow appPersonalRow = appPersonals[0];

                        //handle nric and relationship
                        if (string.IsNullOrEmpty(nric) && preserveNric)
                            nric = appPersonalRow.IsNricNull() ? "" : appPersonalRow.Nric.Trim();

                        relationship = appPersonalRow.IsRelationshipNull() ? "" : appPersonalRow.Relationship.Trim();

                        AttachAppPersonalReference(docId, docAppId, nric, destinationFolder, relationship);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Attach DocPersonal reference
        /// </summary>
        /// <param name="docRefId"></param>
        /// <param name="docId"></param>
        /// <param name="nric"></param>
        /// <param name="destinationFolder"></param>
        public Boolean AttachDocPersonalReference(int docId, int docSetId, string nric, string destinationFolder, string relationship)
        {
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            DocPersonalDb docPersonalDb = new DocPersonalDb();

            //for single nric records
            if (string.IsNullOrEmpty(relationship))
            {
                //search and attach the new Personal record. 
                DocPersonal.DocPersonalDataTable docPersonalsByNricDocSetId = docPersonalDb.GetDocPersonalByNricFolderAndDocSetId(nric, docSetId, destinationFolder);

                // if record exits, attach it to the DocPersonal.
                if (docPersonalsByNricDocSetId.Rows.Count > 0)
                {
                    DocPersonal.DocPersonalRow docPersonalsByNricDocSetIdRow = docPersonalsByNricDocSetId[0];
                    return (setDocRefDb.Insert(docPersonalsByNricDocSetIdRow.Id, docId) != -1);
                }
                else
                {
                    //create new DocPersonal record
                    int newDocPersonalId = docPersonalDb.Insert(docSetId, nric, string.Empty, destinationFolder, null);

                    //attach to the document
                    return (setDocRefDb.Insert(newDocPersonalId, docId) != -1);
                }
            }
            else //for requestor and spouse nric records
            {
                //search and attach the new Personal record. 
                RelationshipEnum relationshipEnum = (RelationshipEnum)Enum.Parse(typeof(RelationshipEnum), relationship, true);

                DocPersonal.DocPersonalDataTable docPersonalsByNricDocSetId = docPersonalDb.GetDocPersonalByNricRelationshipFolderAndDocSetId(nric, relationshipEnum, docSetId, destinationFolder);

                // if record exits, attach it to the DocPersonal.
                if (docPersonalsByNricDocSetId.Rows.Count > 0)
                {
                    DocPersonal.DocPersonalRow docPersonalsByNricDocSetIdRow = docPersonalsByNricDocSetId[0];
                    return (setDocRefDb.Insert(docPersonalsByNricDocSetIdRow.Id, docId) != -1);
                }
                else
                {
                    //create new DocPersonal record
                    int newDocPersonalId = docPersonalDb.Insert(docSetId, nric, string.Empty, destinationFolder, relationshipEnum);

                    //attach to the document
                    return (setDocRefDb.Insert(newDocPersonalId, docId) != -1);
                }
            }
        }

        /// <summary>
        /// attach App Personal reference
        /// </summary>
        /// <param name="docRefId"></param>
        /// <param name="docId"></param>
        /// <param name="nric"></param>
        /// <param name="destinationFolder"></param>
        public Boolean AttachAppPersonalReference(int docId, int docAppId, string nric, string destinationFolder, string relationship)
        {
            AppDocRefDb appDocRefDb = new AppDocRefDb();
            AppPersonalDb appPersonalDb = new AppPersonalDb();

            //for single nric records
            if (string.IsNullOrEmpty(relationship))
            {
                //search and attach the new personal record
                AppPersonal.AppPersonalDataTable appPersonalsByNricSetAppId = appPersonalDb.GetAppPersonalByNricFolderAndDocAppId(nric, docAppId, destinationFolder);

                // if record exits, attach it to the appPersonal.
                if (appPersonalsByNricSetAppId.Rows.Count > 0)
                {
                    AppPersonal.AppPersonalRow appPersonalsByNricSetAppIdRow = appPersonalsByNricSetAppId[0];
                    return (appDocRefDb.Insert(appPersonalsByNricSetAppIdRow.Id, docId) != -1);
                }
                else
                {
                    //create new AppPersonal record
                    int newAppPersonalId = appPersonalDb.Insert(docAppId, nric, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, destinationFolder, null, 0, string.Empty);

                    //attach to the document
                    return (appDocRefDb.Insert(newAppPersonalId, docId) != -1);
                }
            }
            else //for requestor and spouse nric records
            {
                //search and attach the new Personal record. 
                RelationshipEnum relationshipEnum = (RelationshipEnum)Enum.Parse(typeof(RelationshipEnum), relationship, true);

                //search and attach the new personal record
                AppPersonal.AppPersonalDataTable appPersonalsByNricSetAppId = appPersonalDb.GetAppPersonalByNricRelationshipFolderAndDocAppId(nric, docAppId, destinationFolder, relationshipEnum);

                // if record exits, attach it to the appPersonal.
                if (appPersonalsByNricSetAppId.Rows.Count > 0)
                {
                    AppPersonal.AppPersonalRow appPersonalsByNricSetAppIdRow = appPersonalsByNricSetAppId[0];
                    return (appDocRefDb.Insert(appPersonalsByNricSetAppIdRow.Id, docId) !=-1);
                }
                else
                {
                    //create new AppPersonal record
                    int newAppPersonalId = appPersonalDb.Insert(docAppId, nric, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, destinationFolder, relationshipEnum, 0, string.Empty);

                    //attach to the document
                    return (appDocRefDb.Insert(newAppPersonalId, docId) != -1);
                }
            }
        }

        /// <summary>
        /// Attach the apppersonal household structure to a docid
        /// </summary>
        /// <param name="docId"></param>
        /// <param name="docAppId"></param>
        public Boolean AttachAppPersonalHouseholdReference(int docId, int docAppId, int docRefId, RadTreeNode sourceNode)
        {
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();

            Boolean isDocStillAttachedToOldRefId = false;

            //check if the docid is still attached to refId before the dettach and attach action.
            if (sourceNode.Attributes["referencePersonalTable"].ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                isDocStillAttachedToOldRefId = setDocRefDb.GetSetDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;
            else
                isDocStillAttachedToOldRefId = appDocRefDb.GetAppDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;

            if (!isDocStillAttachedToOldRefId)
                return false;

            //dettach the document from docPersonal and appPersonal
            DettachAppPersonalReference(docId);
            DettachDocPersonalReference(docId);

            AppPersonalDb appPersonalDb = new AppPersonalDb();
            AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalForHouseholdStructureByDocAppId(docAppId);

            foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
            {
                appDocRefDb.Insert(appPersonalRow.Id, docId);
            }

            return true;
        }

        /// <summary>
        /// Dettach AppPersonal records for a docid
        /// </summary>
        /// <param name="docId"></param>
        public void DettachAppPersonalReference(int docId)
        {
            AppDocRefDb appDocRefDb = new AppDocRefDb();
            appDocRefDb.DeleteByDocId(docId);
        }

        /// <summary>
        /// Dettach DocPersonal records for a docid
        /// </summary>
        /// <param name="docId"></param>
        public void DettachDocPersonalReference(int docId)
        {
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            setDocRefDb.DeleteByDocId(docId);
        }


        /// <summary>
        /// Get Nric for dropdownlist
        /// </summary>
        /// <returns></returns>
        public DataTable GetNricForDropDownList()
        {
            return DocDs.GetNricForDropDownList();
        }

        /// <summary>
        /// Get nric for completeness dropdown. since completeness only have records in appPersonal, exclude the nric in docPersonal
        /// </summary>
        /// <returns></returns>
        public DataTable GetNricForCompletenessDropDownList()
        {
            return DocDs.GetNricForCompletenessDropDownList();
        }

        #region Added by Edward 2016/03/23 Optimize Doctype Saving
        public Boolean Save(int docId, int docRefId, string referencePersonalTable, string currentDocType, string docType, string nric, string name, Boolean logNameChange, string idType, string customerSourceId)
        {
            DocPersonalDb docPersonalDb = new DocPersonalDb();
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            SetDocRefDb setDocRefDb = new SetDocRefDb();
            AppDocRefDb appDocRefDb = new AppDocRefDb();

            nric = nric.Trim();
            name = name.Trim();

            Boolean isDocStillAttachedToOldRefId = false;

            //check if the docid is still attached to refId before the dettach and attach action.
            if (referencePersonalTable.ToString().Trim().ToUpper().Equals(PersonalTypeTableEnum.DOCPERSONAL.ToString()))
                isDocStillAttachedToOldRefId = setDocRefDb.GetSetDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;
            else
                isDocStillAttachedToOldRefId = appDocRefDb.GetAppDocRefByDocIdAndRefId(docId, docRefId).Rows.Count > 0;

            if (!isDocStillAttachedToOldRefId)
                return false;

            if (!currentDocType.ToLower().Equals(docType.ToLower()))
            {
                //delete old metadata
                MetaDataDb metaDataDb = new MetaDataDb();
                metaDataDb.DeleteByDocID(docId, false, false);
            }

            if (referencePersonalTable.ToUpper().Equals(PersonalTypeTableEnum.APPPERSONAL.ToString()))
            {
                AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByAppDocRefId(docRefId);

                if (appPersonals.Rows.Count > 0)
                {
                    AppPersonal.AppPersonalRow appPersonalRow = appPersonals[0];

                    //detach the old link to old appPersonal record
                    AppDocRef.AppDocRefDataTable appDocRefs = appDocRefDb.GetAppDocRefById(docRefId);
                    AppDocRef.AppDocRefRow appDocRefRow = appDocRefs[0];
                    appDocRefDb.DeleteByDocId(appDocRefRow.DocId);

                    AppPersonal.AppPersonalDataTable appPersonalsByNricDocAppId = appPersonalDb.GetAppPersonalByNricAndDocAppId(nric, appPersonalRow.DocAppId);

                    // if record exits, attach it to the appPersonal.
                    if (appPersonalsByNricDocAppId.Rows.Count > 0)
                    {
                        AppPersonal.AppPersonalRow appPersonalsByNricSetAppIdRow = appPersonalsByNricDocAppId[0];
                        appDocRefDb.Insert(appPersonalsByNricSetAppIdRow.Id, docId);

                        //update Name only if the nric is not part of householdstructure
                        if (!appPersonalDb.IsAppNric(nric, appPersonalsByNricSetAppIdRow.DocAppId))
                        {
                            appPersonalDb.Update(appPersonalsByNricSetAppIdRow.Id, string.Empty, name, string.Empty, string.Empty, string.Empty);

                            //logname change action
                            if (logNameChange && IsAppPersonalNameChanged(appPersonalsByNricSetAppIdRow, name))
                                LogDocumentNameChange(docId, name, appPersonalsByNricSetAppIdRow.IsNameNull() ? string.Empty : appPersonalsByNricSetAppIdRow.Name);
                        }

                        #region Modified by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
                        //update customertype, customersourceid and idtype
                        appPersonalDb.Update(appPersonalsByNricSetAppIdRow.Id, !string.IsNullOrEmpty(nric) ? idType : string.Empty, customerSourceId);
                        #endregion
                    }
                    else
                    {
                        //create new AppPersonal record
                        int newAppPersonalId = appPersonalDb.Insert(appPersonalRow.DocAppId, nric.ToUpper(), name, string.Empty, string.Empty, string.Empty, string.Empty, DocFolderEnum.Others.ToString(), null, 0, string.Empty);

                        //attach to the document
                        appDocRefDb.Insert(newAppPersonalId, docId);

                        //logname change action
                        if (logNameChange)
                            LogDocumentNameChange(docId, name, WebStringEnum.Empty.ToString());

                        #region Modified by Edward at 2015/8/6 to Avoid saving IDType without NRIC - Reducing CDB Errors
                        //update customertype, customersourceid and idtype
                        appPersonalDb.Update(newAppPersonalId, !string.IsNullOrEmpty(nric) ? idType : string.Empty, customerSourceId);
                        #endregion
                    }
                }
            }
            else
            {
                DocPersonal.DocPersonalDataTable docPersonals = docPersonalDb.GetDocPersonalBySetDocRefId(docRefId);

                if (docPersonals.Rows.Count > 0)
                {
                    DocPersonal.DocPersonalRow docPersonalRow = docPersonals[0];

                    //detach the old link to old DocPersonal record
                    SetDocRef.SetDocRefDataTable setDocRefs = setDocRefDb.GetSetDocRefById(docRefId);
                    SetDocRef.SetDocRefRow setDocRefRow = setDocRefs[0];
                    setDocRefDb.DeleteByDocId(setDocRefRow.DocId);

                    DocPersonal.DocPersonalDataTable docPersonalsByNricDocSetId = docPersonalDb.GetDocPersonalByNricFolderAndDocSetId(nric, docPersonalRow.DocSetId, docPersonalRow.Folder);

                    // if record exits, attach it to the DocPersonal.
                    if (docPersonalsByNricDocSetId.Rows.Count > 0)
                    {
                        DocPersonal.DocPersonalRow docPersonalsByNricDocSetIdRow = docPersonalsByNricDocSetId[0];
                        setDocRefDb.Insert(docPersonalsByNricDocSetIdRow.Id, docId);

                        //updateName
                        docPersonalDb.UpdateName(docPersonalsByNricDocSetIdRow.Id, name);

                        //logname change action
                        if (logNameChange && IsDocPersonalNameChanged(docPersonalsByNricDocSetIdRow, name))
                            LogDocumentNameChange(docId, name, docPersonalsByNricDocSetIdRow.IsNameNull() ? string.Empty : docPersonalsByNricDocSetIdRow.Name);

                        //update customersourceid and idtype
                        docPersonalDb.Update(docPersonalsByNricDocSetIdRow.Id, idType, customerSourceId);
                    }
                    else
                    {
                        //create new DocPersonal record
                        int newDocPersonalId = docPersonalDb.Insert(docPersonalRow.DocSetId, nric.ToUpper(), name, docPersonalRow.Folder, null);

                        //attach to the document
                        setDocRefDb.Insert(newDocPersonalId, docId);

                        //logname change action
                        if (logNameChange)
                            LogDocumentNameChange(docId, name, string.Empty);

                        //update customersourceid and idtype
                        docPersonalDb.Update(newDocPersonalId, idType, customerSourceId);
                    }
                }
            }

            return true;
        }

        protected Boolean IsAppPersonalNameChanged(AppPersonal.AppPersonalRow appPersonalRow, string name)
        {
            Boolean isNameChanged = false;
            if (!appPersonalRow.IsNameNull())
                isNameChanged = !appPersonalRow.Name.Trim().Equals(name.Trim());

            return ((appPersonalRow.IsNameNull() && name.Trim().Length > 0) || isNameChanged);
        }

        protected Boolean IsDocPersonalNameChanged(DocPersonal.DocPersonalRow docPersonalRow, string name)
        {
            Boolean isNameChanged = false;
            if (!docPersonalRow.IsNameNull())
                isNameChanged = !docPersonalRow.Name.Trim().Equals(name.Trim());

            return ((docPersonalRow.IsNameNull() && name.Trim().Length > 0) || isNameChanged);
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using Dwms.Bll;
using Telerik.Web.UI;
using System.Xml;       //Added By Edward 27.11.2013 Batch XML
using System.Linq;
using System.Xml.Linq;

namespace Dwms.Web
{
    public sealed class TreeviewDWMS
    {
        public static readonly string ImgIconFolder = "~/Data/Images/Files/folder.gif";
        public static readonly string ImgIconConfirm = "~/Data/Images/Files/pdf_confirmed.gif";
        public static readonly string ImgIconNotConfirm = "~/Data/Images/Files/pdf.gif";
        public static readonly string ImgIconVerify = "~/Data/Images/Files/pdfVerifiy.gif";
        public static readonly string ImgIconAccept = "~/Data/Images/Files/pdfAccept.gif";

        public static void PopulateTreeView(RadTreeView radTreeView, int setId, Boolean showCheckBox, Boolean showOnlyVerifiedDocuments)
        {
            radTreeView.Nodes.Clear();

            //delete all the orphan records in apppersonal
            AppPersonalDb appPersonalDb = new AppPersonalDb();
            appPersonalDb.DeleteOrphanRecords();

            DocDb docDb = new DocDb();

            //get DocSet
            DocSetDb docSetDb = new DocSetDb();
            DocSet.DocSetDataTable docSets = new DocSet.DocSetDataTable();

            docSets = docSetDb.GetDocSetById(setId);

            foreach (DocSet.DocSetRow docSetRow in docSets)
            {
                bool isSetConfirmed = false;
                isSetConfirmed = docSetRow.Status.ToString().ToLower().Equals(SetStatusEnum.Verified.ToString().ToLower());
                RadTreeNode setNode = new RadTreeNode();
                setNode.ImageUrl = "~/Data/Images/Action/OrganizerHS.png";
                setNode.Value = docSetRow.Id.ToString();
                setNode.Text = "SET: " + docSetRow.SetNo;
                setNode.Category = "Set";
                setNode.ContextMenuID = "CategoryContextMenu";
                setNode.CssClass = "hand";
                setNode.Font.Bold = true;
                setNode.Checkable = showCheckBox;
                setNode.ContextMenuID = "PageContextMenu";
                setNode.Height = Unit.Percentage(100);
                setNode.AllowDrop = false;
                setNode.AllowDrag = false;

                // Expand/collapse set node
                setNode.ExpandMode = TreeNodeExpandMode.ServerSide;
                setNode.Expanded = true;

                radTreeView.Nodes.Add(setNode);

                //populate FIRST LEVEL FOLDERS - UNIDENTIFIED, [[[[APPLICATION >> NRIC/OTHERS >> DOCUMENTS]]]], BLANK, ROUTED, SPAM folders
                DataTable docParentFolder = docDb.GetParentFolderForTreeView(setId);

                foreach (DataRow docParentFolderRow in docParentFolder.Rows)
                {
                    //set folder Name
                    string folderName = string.Empty;
                    folderName = docParentFolderRow["FolderName"].ToString().Trim();

                    //for sers app show the address next to the folder
                    if (docParentFolderRow["RefType"].ToString().Trim().ToLower().Equals("sers"))
                    {
                        SersInterfaceDb sersInterfaceDb = new SersInterfaceDb();
                        folderName += " (" + sersInterfaceDb.GetSersAddressByRefNo(docParentFolderRow["RefNo"].ToString().Trim()) + ")";
                    }

                    RadTreeNode firstLevelNode = CreateNode(setNode, folderName, TreeviewDWMS.ImgIconFolder, docParentFolderRow["treeCategory"].ToString().ToLower().Trim(), docParentFolderRow["FolderId"].ToString().ToUpper().Trim(), showCheckBox, false, true, null, -1, docParentFolderRow["RefType"].ToString().Trim().ToLower(), null, setId.ToString(), null, PersonalTypeTableEnum.DOCPERSONAL.ToString(), isSetConfirmed);

                    // if appllication folder, fill the ha,oc, etc folders
                    if (docParentFolderRow["treeCategory"].ToString().Trim().ToUpper().Equals("REFNO"))
                    {
                        AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocAppId(int.Parse(docParentFolderRow["FolderId"].ToString().Trim()));

                        string personalType = "";
                        int i = 1;
                        foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                        {
                            if (!appPersonalRow.Folder.ToString().ToUpper().Equals("OTHERS") && !appPersonalRow.PersonalType.Trim().Equals(string.Empty))
                            {
                                //create the ha,oc etc node
                                RadTreeNode appPersonalNricNode = firstLevelNode.Nodes.FindNodeByValue(appPersonalRow.Nric.ToString().ToUpper());

                                if (appPersonalNricNode == null)
                                {
                                    if (personalType != appPersonalRow.PersonalType)
                                        i = 1;
                                    else
                                        i++;

                                    //send nric in docrefid attribute
                                    CreateNode(firstLevelNode, appPersonalRow.PersonalType.ToString().ToUpper() + i + ": " + appPersonalRow.Nric.ToString().ToUpper() + (appPersonalRow.IsNameNull() ? string.Empty : " - " + appPersonalRow.Name.Trim()), TreeviewDWMS.ImgIconFolder, "REF NRIC", appPersonalRow.Nric.ToString().Trim(), showCheckBox, false, true, null, -1, null, appPersonalRow.Nric.ToString().Trim(), setId.ToString(), null, string.Empty, isSetConfirmed);
                                    personalType = appPersonalRow.PersonalType;
                                }
                            }
                        }

                        //Create Others Folder
                        RadTreeNode othersNode = CreateNode(firstLevelNode, "Others", TreeviewDWMS.ImgIconFolder, "REF OTHERS", "Others", showCheckBox, false, true, null, -1, null, null, setId.ToString(), null, string.Empty, isSetConfirmed);

                        //Get all the files from AppPersonal and fill the Application folder
                        // during the process create the nric folders in the others folder
                        DataTable appPersonalDocs = new DataTable();
                        appPersonalDocs = appPersonalDb.GetAppPersonalDocumentByDocAppIdAndSetId(int.Parse(docParentFolderRow["FolderId"].ToString().Trim()), setId);

                        foreach (DataRow appPersonalDocRow in appPersonalDocs.Rows)
                        {
                            if (!appPersonalDocRow["Folder"].ToString().ToLower().Equals("others"))
                            {
                                RadTreeNode appPersonalNode = firstLevelNode.Nodes.FindNodeByValue(appPersonalDocRow["Nric"].ToString().ToUpper().Trim());

                                if (appPersonalNode != null)
                                {
                                    if (!appPersonalDocRow["DocTypeCode"].ToString().ToUpper().Trim().Equals(docParentFolderRow["RefType"].ToString().ToUpper().Trim()))
                                    {
                                        RadTreeNode documentNode = appPersonalNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                        if (documentNode == null)
                                        {
                                            if (showOnlyVerifiedDocuments && appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                                CreateNode(appPersonalNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                            else if (!showOnlyVerifiedDocuments)
                                                CreateNode(appPersonalNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                        }
                                    }
                                    else
                                    {
                                        RadTreeNode ApplicationFormNode = firstLevelNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                        if (ApplicationFormNode == null)
                                        {
                                            if (showOnlyVerifiedDocuments && appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                                CreateNode(firstLevelNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, 0, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                            else if (!showOnlyVerifiedDocuments)
                                                CreateNode(firstLevelNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, 0, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                        }
                                    }
                                }
                            }
                            else //fill documents for [Others] folder under a application folder
                            {
                                if (string.IsNullOrEmpty(appPersonalDocRow["Nric"].ToString().Trim())) //if no nric add the doc under other
                                {
                                    RadTreeNode documentNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                    if (documentNode == null)
                                    {
                                        if (showOnlyVerifiedDocuments && appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                            CreateNode(othersNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                        else if (!showOnlyVerifiedDocuments)
                                            CreateNode(othersNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                    }
                                }
                                else // if nric exist, create nric node and add the document to the nric folder
                                {
                                    RadTreeNode otherNricNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["Nric"].ToString().ToUpper().Trim());
                                    if (otherNricNode == null) // //send nric in docrefid attribute
                                        otherNricNode = CreateNode(othersNode, appPersonalDocRow["Nric"].ToString().ToUpper().Trim() + (string.IsNullOrEmpty(appPersonalDocRow["Name"].ToString().Trim()) ? string.Empty : " - " + appPersonalDocRow["Name"].ToString().Trim()), TreeviewDWMS.ImgIconFolder, "REF OTHERS NRIC", appPersonalDocRow["Nric"].ToString().ToUpper().Trim(), showCheckBox, false, true, null, -1, null, appPersonalDocRow["Nric"].ToString().ToUpper().Trim(), setId.ToString(), null, string.Empty, isSetConfirmed);

                                    RadTreeNode documentNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                    if (documentNode == null)
                                    {
                                        if (showOnlyVerifiedDocuments && appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || appPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                            CreateNode(otherNricNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                        else if (!showOnlyVerifiedDocuments)
                                            CreateNode(otherNricNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), setId.ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                    }
                                }
                            }
                        }
                    }
                }

                //fill all the files which are outside the application folder [ UNIDENTIFIED, BLANK, ROUTED, SPAM ]
                // during the process create the nric folders in the respective folder
                DocPersonalDb docPersonalDb = new DocPersonalDb();
                DataTable docPersonalDocs = new DataTable();
                docPersonalDocs = docPersonalDb.GetDocPersonalDocumentByDocSetId(setId);

                foreach (DataRow docPersonalDocRow in docPersonalDocs.Rows)
                {
                    if (!string.IsNullOrEmpty(docPersonalDocRow["Folder"].ToString().Trim())) // proceed if there is a folder info
                    {
                        RadTreeNode defaultFolderNode = setNode.Nodes.FindNodeByValue(docPersonalDocRow["Folder"].ToString().ToUpper().Trim());

                        if (defaultFolderNode != null)
                        {
                            if (string.IsNullOrEmpty(docPersonalDocRow["Nric"].ToString().Trim())) //if no nric add the doc under folder directly
                            {
                                RadTreeNode documentNode = defaultFolderNode.Nodes.FindNodeByValue(docPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                if (documentNode == null)
                                {
                                    if (showOnlyVerifiedDocuments && docPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || docPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                        CreateNode(defaultFolderNode, docPersonalDocRow["Description"].ToString().Trim() + FormatId(docPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(docPersonalDocRow["DocStatus"].ToString().Trim(), docPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", docPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, docPersonalDocRow["DocTypeCode"].ToString().Trim(), docPersonalDocRow["SetDocRefId"].ToString().Trim(), setId.ToString(), "DocPersonal", docPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                    else if (!showOnlyVerifiedDocuments)
                                        CreateNode(defaultFolderNode, docPersonalDocRow["Description"].ToString().Trim() + FormatId(docPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(docPersonalDocRow["DocStatus"].ToString().Trim(), docPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", docPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, docPersonalDocRow["DocTypeCode"].ToString().Trim(), docPersonalDocRow["SetDocRefId"].ToString().Trim(), setId.ToString(), "DocPersonal", docPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                }
                            }
                            else // if nric exist, create nric node and add the document to the nric folder
                            {
                                RadTreeNode defaultFolderNricNode = defaultFolderNode.Nodes.FindNodeByValue(docPersonalDocRow["Nric"].ToString().ToUpper().Trim());
                                if (defaultFolderNricNode == null) ////send nric in docrefid attribute
                                    defaultFolderNricNode = CreateNode(defaultFolderNode, docPersonalDocRow["Nric"].ToString().ToUpper().Trim() + (string.IsNullOrEmpty(docPersonalDocRow["Name"].ToString().Trim()) ? string.Empty : " - " + docPersonalDocRow["Name"].ToString().Trim()), TreeviewDWMS.ImgIconFolder, "DEFAULT FOLDER NRIC", docPersonalDocRow["Nric"].ToString().ToUpper().Trim(), showCheckBox, false, true, null, -1, null, docPersonalDocRow["Nric"].ToString().ToUpper().Trim(), setId.ToString(), null, string.Empty, isSetConfirmed);

                                RadTreeNode documentNode = defaultFolderNricNode.Nodes.FindNodeByValue(docPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                if (documentNode == null)
                                {
                                    if (showOnlyVerifiedDocuments && docPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Verified.ToString().ToLower()) || docPersonalDocRow["DocStatus"].ToString().ToLower().Trim().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                                        CreateNode(defaultFolderNricNode, docPersonalDocRow["Description"].ToString().Trim() + FormatId(docPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(docPersonalDocRow["DocStatus"].ToString().Trim(), docPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", docPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, docPersonalDocRow["DocTypeCode"].ToString().Trim(), docPersonalDocRow["SetDocRefId"].ToString().Trim(), setId.ToString(), "DocPersonal", docPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                    else if (!showOnlyVerifiedDocuments)
                                        CreateNode(defaultFolderNricNode, docPersonalDocRow["Description"].ToString().Trim() + FormatId(docPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForVerification(docPersonalDocRow["DocStatus"].ToString().Trim(), docPersonalDocRow["SendStatus"].ToString().Trim()), "Doc", docPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, docPersonalDocRow["DocTypeCode"].ToString().Trim(), docPersonalDocRow["SetDocRefId"].ToString().Trim(), setId.ToString(), "DocPersonal", docPersonalDocRow["DocStatus"].ToString().Trim(), isSetConfirmed);
                                }
                            }
                        }
                    }
                }
            }

            //add the node count
            AddNodeCount(radTreeView);

            //set tooltip
            SetToolTip(radTreeView);

        }

        public static void SetToolTip(RadTreeView radTreeView)
        {
            foreach (RadTreeNode node in radTreeView.GetAllNodes())
            {
                node.ToolTip = Format.RemoveHTMLTags(node.Text);
            }
        }

        public static void AddNodeCount(RadTreeView radTreeView)
        {
            if (radTreeView.Nodes.Count > 0)
            {
                GetChildNodeCount(radTreeView.Nodes[0]);
            }
        }

        public static int GetChildNodeCount(RadTreeNode node)
        {
            int count = 0;

            foreach (RadTreeNode childNode in node.Nodes)
            {
                if (childNode.Category.ToLower().Equals("doc"))
                {
                    count++;
                }
                else if (childNode.Nodes.Count > 0)
                {
                    count = count + GetChildNodeCount(childNode);
                }
                else
                {
                    childNode.Text = "<font class=treeCount>(0)</font> " + childNode.Text;
                }
            }

            node.Text = "<font class=treeCount>(" + count.ToString() + ")</font> " + node.Text;
            return count;
        }

        public static string GetFileIconForVerification(string status, string sendstatus)
        {
            if ((status.ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) || status.ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower())) && sendstatus.ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                return TreeviewDWMS.ImgIconVerify;
            else if (status.ToLower().Equals(DocStatusEnum.Verified.ToString().ToLower()) || status.ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                return TreeviewDWMS.ImgIconConfirm;
            else
                return TreeviewDWMS.ImgIconNotConfirm;
        }

        public static string GetFileIconForCompleteness(string status, string sendstatus, string sendaccept)
        {
            if (status.ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()) && sendstatus.ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()) && sendaccept.ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                return TreeviewDWMS.ImgIconAccept;
            if (status.ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()) && sendstatus.ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                return TreeviewDWMS.ImgIconConfirm;
            else if (status.ToLower().Equals(DocStatusEnum.Completed.ToString().ToLower()))
                return TreeviewDWMS.ImgIconConfirm;
            else if (sendstatus.ToLower().Equals(SendToCDBStatusEnum.Sent.ToString().ToLower()))
                return TreeviewDWMS.ImgIconVerify;
            else
                return TreeviewDWMS.ImgIconNotConfirm;
        }

        public static void PopulateTreeViewCompleteness(RadTreeView radTreeView, int docAppId, Boolean showCheckBox)
        {
            radTreeView.Nodes.Clear();

            //get DocApp

            DocAppDb docAppDb = new DocAppDb();
            DocApp.DocAppDataTable docApps = new DocApp.DocAppDataTable();

            docApps = docAppDb.GetDocAppById(docAppId);

            foreach (DocApp.DocAppRow docAppRow in docApps)
            {
                bool isAppConfirmed = false;
                isAppConfirmed = docAppRow.Status.ToString().ToLower().Equals(AppStatusEnum.Completeness_Checked.ToString().ToLower());
                RadTreeNode appNode = new RadTreeNode();
                appNode.ImageUrl = "~/Data/Images/Action/OrganizerHS.png";
                appNode.Value = docAppRow.Id.ToString();
                appNode.Text = docAppRow.RefType + ":" + docAppRow.RefNo;
                appNode.Category = "REF";
                appNode.ContextMenuID = "CategoryContextMenu";
                appNode.CssClass = "hand";
                appNode.Font.Bold = true;
                appNode.Checkable = showCheckBox;
                appNode.ContextMenuID = "PageContextMenu";
                appNode.Height = Unit.Percentage(100);
                appNode.AllowDrop = false;
                appNode.AllowDrag = false;
                appNode.Attributes.Add("DocTypeCode", docAppRow.RefType);
                // Expand/collapse set node
                appNode.ExpandMode = TreeNodeExpandMode.ServerSide;
                appNode.Expanded = true;

                radTreeView.Nodes.Add(appNode);

                SetAppDb setAppDb = new SetAppDb();
                SetApp.SetAppDataTable setApps = setAppDb.GetSetAppByDocAppId(docAppId);
                AppPersonalDb appPersonalDb = new AppPersonalDb();

                //first create the folder structure.
                foreach (SetApp.SetAppRow setAppRows in setApps)
                {
                    DocSetDb docSetDb = new DocSetDb();
                    DocSet.DocSetDataTable docSets = docSetDb.GetDocSetById(setAppRows.DocSetId);
                    DocSet.DocSetRow docSetRow = docSets[0];

                    //proceed only if the set is verified
                    if (docSetRow.Status.ToLower().Equals(SetStatusEnum.Verified.ToString().ToLower()))
                    {
                        AppPersonal.AppPersonalDataTable appPersonals = appPersonalDb.GetAppPersonalByDocAppId(setAppRows.DocAppId);

                        string personalType = "";
                        int i = 1;
                        foreach (AppPersonal.AppPersonalRow appPersonalRow in appPersonals.Rows)
                        {
                            if (!appPersonalRow.Folder.ToString().ToUpper().Equals("OTHERS") && !appPersonalRow.PersonalType.Trim().Equals(string.Empty))
                            {
                                //create the ha,oc etc node
                                RadTreeNode appPersonalNricNode = appNode.Nodes.FindNodeByValue(appPersonalRow.Nric.ToString().ToUpper());

                                if (appPersonalNricNode == null)
                                {
                                    if (personalType != appPersonalRow.PersonalType)
                                        i = 1;
                                    else
                                        i++;

                                    //send nric in docrefid attribute
                                    CreateNode(appNode, appPersonalRow.PersonalType.ToString().ToUpper() + i + ": " + appPersonalRow.Nric.ToString().ToUpper() + (appPersonalRow.IsNameNull() ? string.Empty : " - " + appPersonalRow.Name.Trim()), TreeviewDWMS.ImgIconFolder, "REF NRIC", appPersonalRow.Nric.ToString().Trim(), showCheckBox, false, true, null, -1, null, appPersonalRow.Nric.ToString().ToUpper(), setAppRows.DocSetId.ToString(), null, string.Empty, isAppConfirmed);
                                    personalType = appPersonalRow.PersonalType;
                                }
                            }
                        }

                        //Create Others Folder
                        RadTreeNode othersNode = appNode.Nodes.FindNodeByValue("Others");
                        if (othersNode == null)
                            othersNode = CreateNode(appNode, "Others", TreeviewDWMS.ImgIconFolder, "REF OTHERS", "Others", showCheckBox, false, true, null, -1, null, null, setAppRows.DocSetId.ToString(), null, string.Empty, isAppConfirmed);
                    }
                }

                //Get all the files from AppPersonal and fill the Application folder
                // during the process create the nric folders in the others folder
                DataTable appPersonalDocs = new DataTable();
                appPersonalDocs = appPersonalDb.GetAppPersonalDocumentByDocAppId(docAppId);

                foreach (DataRow appPersonalDocRow in appPersonalDocs.Rows)
                {
                    if (!appPersonalDocRow["Folder"].ToString().ToLower().Equals("others"))
                    {
                        RadTreeNode appPersonalNode = appNode.Nodes.FindNodeByValue(appPersonalDocRow["Nric"].ToString().ToUpper().Trim());

                        if (appPersonalNode != null)
                        {
                            if (!(appPersonalDocRow["DocTypeCode"].ToString().ToUpper().Trim().Equals("HLE") || appPersonalDocRow["DocTypeCode"].ToString().ToUpper().Trim().Equals("SALES") || appPersonalDocRow["DocTypeCode"].ToString().ToUpper().Trim().Equals("RESALE") || appPersonalDocRow["DocTypeCode"].ToString().ToUpper().Trim().Equals("SERS")))
                            {
                                RadTreeNode documentNode = appPersonalNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                if (documentNode == null)
                                    CreateNode(appPersonalNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForCompleteness(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim(), appPersonalDocRow["SendAccept"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), appPersonalDocRow["DocSetId"].ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isAppConfirmed);
                            }
                            else
                            {
                                RadTreeNode ApplicationFormNode = appNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                                if (ApplicationFormNode == null)
                                    CreateNode(appNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForCompleteness(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim(), appPersonalDocRow["SendAccept"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, 0, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), appPersonalDocRow["DocSetId"].ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isAppConfirmed);
                            }
                        }
                    }
                    else //fill documents for [Others] folder under a application folder
                    {
                        RadTreeNode othersNode = appNode.Nodes.FindNodeByValue("Others");
                        if (othersNode == null)
                            othersNode = CreateNode(appNode, "Others", TreeviewDWMS.ImgIconFolder, "REF OTHERS", "Others", showCheckBox, false, true, null, -1, null, null, appPersonalDocRow["DocSetIdId"].ToString().Trim(), null, string.Empty, isAppConfirmed);

                        if (string.IsNullOrEmpty(appPersonalDocRow["Nric"].ToString().Trim())) //if no nric add the doc under other
                        {
                            RadTreeNode documentNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                            if (documentNode == null)
                                CreateNode(othersNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForCompleteness(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim(), appPersonalDocRow["SendAccept"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), appPersonalDocRow["DocSetId"].ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isAppConfirmed);
                        }
                        else // if nric exist, create nric node and add the document to the nric folder
                        {
                            RadTreeNode otherNricNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["Nric"].ToString().ToUpper().Trim());
                            if (otherNricNode == null) //send nric in docrefid attribute
                                otherNricNode = CreateNode(othersNode, appPersonalDocRow["Nric"].ToString().ToUpper().Trim() + (string.IsNullOrEmpty(appPersonalDocRow["Name"].ToString().Trim()) ? string.Empty : " - " + appPersonalDocRow["Name"].ToString().Trim()), TreeviewDWMS.ImgIconFolder, "REF OTHERS NRIC", appPersonalDocRow["Nric"].ToString().ToUpper().Trim(), showCheckBox, false, true, null, -1, null, appPersonalDocRow["Nric"].ToString().ToUpper().Trim(), appPersonalDocRow["DocSetId"].ToString(), null, string.Empty, isAppConfirmed);

                            RadTreeNode documentNode = othersNode.Nodes.FindNodeByValue(appPersonalDocRow["DocId"].ToString().ToUpper().Trim());
                            if (documentNode == null)
                                CreateNode(otherNricNode, appPersonalDocRow["Description"].ToString().Trim() + FormatId(appPersonalDocRow["DocId"].ToString().Trim()), GetFileIconForCompleteness(appPersonalDocRow["DocStatus"].ToString().Trim(), appPersonalDocRow["SendStatus"].ToString().Trim(), appPersonalDocRow["SendAccept"].ToString().Trim()), "Doc", appPersonalDocRow["DocId"].ToString().Trim(), showCheckBox, true, true, null, -1, appPersonalDocRow["DocTypeCode"].ToString().Trim(), appPersonalDocRow["AppDocRefId"].ToString().Trim(), appPersonalDocRow["DocSetId"].ToString(), "AppPersonal", appPersonalDocRow["DocStatus"].ToString().Trim(), isAppConfirmed);
                        }
                    }
                }
            }

            //add the node count
            AddNodeCount(radTreeView);

            //set tooltip
            SetToolTip(radTreeView);
        }

        public static string FormatId(string id)
        {
            return Utility.FormatId(id);
        }

        public static string DisplayIdForCompleteness(string id, string doctype, string monthNumber)
        {
            string displayId = string.Empty;
            switch (doctype.ToLower())
            {
                case "payslip":
                case "cpfcontribution":
                case "cpfstatement":
                case "cpfstatementrefund":
                case "commissionstatement":
                    displayId = monthNumber;
                    break;
                default:
                    displayId = id;
                    break;
            }

            return " - " + displayId;
        }
        

        public static RadTreeNode CreateNode(RadTreeNode parentNode, string title, string imageUrl, string category, string value, Boolean showCheckBox, bool allowDrag, bool allowDrop, RadTreeNode insertBeforeNode, int insertAt, string attributeDocTypeCode, string attributeDocRefId, string attributeDocSetId,  string referencePersonalTable, string status, bool isSetConfirmed)
        {
            if (isSetConfirmed)
            {
                allowDrag = false;
                allowDrop = false;
            }

            RadTreeNode childNode = new RadTreeNode(title);
            childNode.ImageUrl = imageUrl;
            childNode.Category = category;
            childNode.Value = value;
            childNode.Checkable = showCheckBox;
            childNode.AllowDrag = allowDrag;
            childNode.AllowDrop = allowDrop;
            childNode.ToolTip = title;
            childNode.Expanded = true;
            childNode.Attributes["Status"] = status;

            if (!string.IsNullOrEmpty(attributeDocTypeCode))
                childNode.Attributes["DocTypeCode"] = attributeDocTypeCode;

            // DocRefId takes the reference id for "doc" nodes and nric for "nric" nodes.
            // using the same variable for multiple purpose.
            if (!string.IsNullOrEmpty(attributeDocRefId))
                childNode.Attributes["DocRefId"] = attributeDocRefId;

            if (!string.IsNullOrEmpty(attributeDocSetId))
                childNode.Attributes["DocSetId"] = attributeDocSetId;

            if (!string.IsNullOrEmpty(referencePersonalTable))
                childNode.Attributes["ReferencePersonalTable"] = referencePersonalTable;

            if (insertAt != -1)
                parentNode.Nodes.Insert(insertAt, childNode);
            else if (insertBeforeNode != null)
                insertBeforeNode.InsertBefore(childNode);
            else
                parentNode.Nodes.Add(childNode);

            return childNode;
        }

        public static int GetTotalNodes(RadTreeView treeView)
        {
            return GetTotalNodes(treeView.Nodes);
        }

        public static int GetTotalNodes(RadTreeNodeCollection nodes)
        {
            int rootNodes = nodes.Count;

            foreach (RadTreeNode node in nodes)
            {
                rootNodes += GetTotalNodes(node.Nodes);
            }

            return rootNodes;
        }

        #region Added By Edward for Batch XML
        //Added By Edward 26.11.2013 for Batch XML
        public static void PopulateTreeViewXML(RadTreeView radTreeView)      
        {
            radTreeView.Nodes.Clear();
            string[] arrWebServices = { "LEAS_FTP", "RESL_FTP", "SOC_FTP", "SRS_FTP" };
            string strHomeFolder = @"household_ftp\localuser";

            #region Commented By Edward 05/02/2014
            //foreach (string strWebServices in arrWebServices)
            //{
            //    RadTreeNode setNode = new RadTreeNode();

            //    setNode.ImageUrl = ImgIconFolder;

            //    DirectoryInfo WebServicesDir = new DirectoryInfo(Util.GetWebServiceDocsFolder() +  @"\" + strHomeFolder + @"\" + strWebServices);

            //    if (!WebServicesDir.Exists)
            //        WebServicesDir.Create();

            //    FileInfo[] WebServicesFileInfo = WebServicesDir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
            //    setNode.Text = strWebServices + " (" + WebServicesFileInfo.Count() + " Files)";
            //    foreach (FileInfo file in WebServicesFileInfo)
            //    {
            //        RadTreeNode fileNode = new RadTreeNode();
            //        fileNode.Text = file.Name;
            //        if (ReadXML(fileNode, file))
            //        {
            //            setNode.Expanded = true;
            //            setNode.Nodes.Add(fileNode);
            //        }
            //        else
            //        {
            //            fileNode.Text = file.Name + "_Error in Loading XML";
            //            fileNode.Enabled = false;                        
            //        }
            //    }

            //    radTreeView.Nodes.Add(setNode);
            //}
            #endregion

            #region Added BY Edward 05/02/2014
            UserDb userDb = new UserDb();
            int sectionId = userDb.GetUserSection((Guid)Membership.GetUser().ProviderUserKey);
            string strWebServices = string.Empty;
            if (sectionId == 1)
                strWebServices = "LEAS_FTP";
            else if (sectionId == 3)
                strWebServices = "SRS_FTP";
            else if (sectionId == 5)
                strWebServices = "RESL_FTP";
            else if (sectionId == 6)
                strWebServices = "SOC_FTP";


            if (!string.IsNullOrEmpty(strWebServices))
            {
                RadTreeNode setNode = new RadTreeNode();

                setNode.ImageUrl = ImgIconFolder;

                DirectoryInfo WebServicesDir = new DirectoryInfo(Util.GetWebServiceDocsFolder() + @"\" + strHomeFolder + @"\" + strWebServices);

                if (!WebServicesDir.Exists)
                    WebServicesDir.Create();

                FileInfo[] WebServicesFileInfo = WebServicesDir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                setNode.Text = strWebServices + " (" + WebServicesFileInfo.Count() + " Files)";
                foreach (FileInfo file in WebServicesFileInfo)
                {
                    RadTreeNode fileNode = new RadTreeNode();
                    fileNode.Text = file.Name;
                    //if (ReadXML(fileNode, file))
                    //{
                    setNode.Expanded = true;
                    setNode.Nodes.Add(fileNode);
                    //}
                    //else
                    //{
                    //    fileNode.Text = file.Name + "_Error in Loading XML";
                    //    fileNode.Enabled = false;
                    //}
                }

                radTreeView.Nodes.Add(setNode);
            }
            #endregion


        }
        private static bool ReadXML(RadTreeNode fileNode, FileInfo file)
        {
            bool result = true;
            try
            {
                if (file.Exists)
                {

                    XDocument doc = XDocument.Load(file.FullName);
                    XElement elementApplication = new XElement("new");
                    foreach (XElement element in doc.Element("ROOT").Descendants())
                    {
                        if (element.Name == "Application")
                        {
                            elementApplication = new XElement(element);
                        }

                        if (element.Name == "RefNumber" || element.Name == "CaseNumber")
                        {
                            RadTreeNode node = new RadTreeNode();
                            node.Text = element.Name + ": " + element.Value;
                            GetPersonalXML(node, elementApplication);
                            node.Checkable = false;
                            fileNode.Nodes.Add(node);
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                result = false;
                return result;
            }            
        }

        private static void GetPersonalXML(RadTreeNode node, XElement element)
        {
            foreach (XElement e in element.Elements())
            {
                if (e.Name == "Personal")
                {
                    foreach (XElement xePersonal in e.Elements())
                    {
                        if (xePersonal.Name == "Nric")
                        {
                            RadTreeNode nodeP = new RadTreeNode();
                            nodeP.Text = xePersonal.Name + ": " + xePersonal.Value;
                            nodeP.Checkable = false;
                            node.Nodes.Add(nodeP);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

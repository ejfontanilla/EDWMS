using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using Dwms.Web;
using Telerik.Web.UI;

public partial class Completeness_ShowAppDocument : System.Web.UI.Page
{
    public int? docAppId;
    //private string ActiveDocType = "HLE"; Commented by Edward 2015/12/04 to Change Folder Structure for documents to YEAR/MONTH/DAY

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            docAppId = int.Parse(Request["id"]);
        }
        else
            Response.Redirect("~/Default.aspx");
       
        if (!IsPostBack)
        {
            TreeviewDWMS.PopulateTreeViewCompleteness(RadTreeView1, docAppId.Value, true);
            RadTreeView1.ExpandAllNodes();
        }

        string referer = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];

        //hide the download pdf and zip files based on the caller.
        if (referer.ToLower().Contains("/completeness/"))
            DownloadZipButton.Visible = false;
        else
            DownloadPdfButton.Visible = false;

    }

    protected void DownloadPdfButton_onClick(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
                IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

                List<int> uniqueNodes = new List<int>();
                foreach (RadTreeNode rNode in checkedNodes)
                {
                    //Commented and Modified the If Condition by Edward on 04/02/2014 Investigate Download PDF
                    //if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
                    if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set") 
                        || rNode.ImageUrl.ToLower().Contains("organizerhs")))              
                    {
                        if (!uniqueNodes.Contains(int.Parse(rNode.Value))) // since one document can be in different folder, filter the repetition
                        {
                            uniqueNodes.Add(int.Parse(rNode.Value));
                        }
                    }
                }

                if (uniqueNodes.Count > 0)
                {
                    string ids = string.Empty;

                    foreach(int docId in uniqueNodes)
                    {
                        ids = (String.IsNullOrEmpty(ids) ? docId.ToString() : ids + "," + docId.ToString());
                    }

                    Response.Redirect(String.Format("DownloadAppDocument.aspx?id={0}&appId={1}", ids, docAppId.Value));
                }
                else
                {
                    RadTreeView1CustomValidator.Text = "<br />Please select at least 1 document to download.";
                    RadTreeView1CustomValidator.IsValid = false;
                }
        }        
    }

    protected void DownloadZipButton_onClick(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

            List<int> uniqueNodes = new List<int>();
            foreach (RadTreeNode rNode in checkedNodes)
            {
                //Commented and Modified the If Condition by Edward on 03/06/2014 Investigate Download PDF
                //if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
                if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")
                        || rNode.ImageUrl.ToLower().Contains("organizerhs")))          
                {
                    if (!uniqueNodes.Contains(int.Parse(rNode.Value))) // since one document can be in different folder, filter the repetition
                    {
                        uniqueNodes.Add(int.Parse(rNode.Value));
                    }
                }
            }

            if (uniqueNodes.Count > 0)
            {
                string ids = string.Empty;

                foreach (int docId in uniqueNodes)
                {
                    ids = (String.IsNullOrEmpty(ids) ? docId.ToString() : ids + "," + docId.ToString());
                }

                Response.Redirect(String.Format("DownloadDocument.aspx?id={0}&appId={1}", ids, docAppId.Value));
            }
            else
            {
                RadTreeView1CustomValidator.Text = "<br />Please select at least 1 document to download.";
                RadTreeView1CustomValidator.IsValid = false;
            }
        }
    }
    #endregion

    #region Validation
    /// <summary>
    /// document custom validator
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    protected void RadTreeView1CustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (RadTreeView1.CheckedNodes.Count > 0);
    }
    #endregion  
}

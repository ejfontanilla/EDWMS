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

public partial class Common_ShowSetDocument : System.Web.UI.Page
{
    public int? setId;

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(Request["id"]))
        {
            setId = int.Parse(Request["id"]);
        }

        if (!IsPostBack)
        {
            TreeviewDWMS.PopulateTreeView(RadTreeView1, setId.Value, true, false);
            RadTreeView1.ExpandAllNodes();
        }
    }

    protected void SendButton_onClick(object sender, EventArgs e)
    {
        Page.Validate();

        if (Page.IsValid)
        {
            IList<RadTreeNode> checkedNodes = RadTreeView1.CheckedNodes;

            List<int> uniqueNodes = new List<int>();
            foreach (RadTreeNode rNode in checkedNodes)
            {
                if (!(rNode.ImageUrl.ToLower().Contains("folder") || rNode.Category.ToLower().Trim().Equals("set")))
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

                Response.Redirect(String.Format("DownloadSetDocument.aspx?id={0}&setId={1}", ids, setId.Value));
            }
            else
            {
                RadTreeView1CustomValidator.Text = "<br />Please select at least 1 document to download.";
                RadTreeView1CustomValidator.IsValid = false;
            }
        }
    }

    protected void RadTreeView1_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        FileSizeLabel.Text = "120MB";
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

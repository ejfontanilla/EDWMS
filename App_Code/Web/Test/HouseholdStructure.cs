using System;
using System.Collections.Generic;
using System.Web;
using Telerik.Web.UI;

namespace Dwms.Web
{
    public class HouseholdStructure
    {
        public static void Build(int setId, RadTreeView HouseholdStructure)
        {
            RadTreeNode rootNode = new RadTreeNode(DateTime.Now.ToString("SET: EACO120523-00888"), "Root");
            HouseholdStructure.Nodes.Clear();
            HouseholdStructure.Nodes.Add(rootNode);
            rootNode.Expanded = true;
            rootNode.NavigateUrl = string.Format("ViewRedirect.aspx?id={0}", setId);
            rootNode.Target = "ViewFrame";

            for (int i = 1; i <= 20; i++)
            {
                int docId = i; // Dummy document ID
                RadTreeNode childNode = new RadTreeNode(string.Format("Document {0}", i));
                rootNode.Nodes.Add(childNode);
                childNode.NavigateUrl = string.Format("ViewRedirect.aspx?id={0}&docId={1}", setId, docId);
                childNode.Target = "ViewFrame";
            }
        }
    }
}
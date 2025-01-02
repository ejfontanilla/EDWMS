using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;

public partial class Maintenance_AuditTrail_View : System.Web.UI.Page
{
    int count;

    /// <summary>
    /// Page load event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        TableNameEnum table = (TableNameEnum)Enum.Parse(typeof(TableNameEnum), Request["table"], true);
        string id = Request["id"];
        TitleLabel.Text = "Transaction History for Record " + id + " in Table " + table;

        AuditTrailDb auditTrailDb = new AuditTrailDb();
        DataTable auditTrails = auditTrailDb.GetTransactionHistory(table, id);
        count = auditTrails.Rows.Count;
        OuterRepeater.DataSource = auditTrails;
        OuterRepeater.DataBind();

        RadGrid1.DataSource = auditTrails;
        RadGrid1.DataBind();
    }

    /// <summary>
    /// Repeater item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OuterRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;

        if ((item.ItemType == ListItemType.Item) ||
            (item.ItemType == ListItemType.AlternatingItem))
        {
            Repeater InnerRepeater = item.FindControl("InnerRepeater") as Repeater;
            Guid operationId = new Guid(((DataRowView)item.DataItem)["OperationId"].ToString());
            AuditTrailDb auditTrailDb = new AuditTrailDb();
            InnerRepeater.DataSource = auditTrailDb.GetTransactionByOperationId(operationId);
            InnerRepeater.DataBind();

            string version = "Version " + (count - item.ItemIndex);
            Label VersionLabel = item.FindControl("VersionLabel") as Label;
            VersionLabel.Text = version;

            OperationTypeEnum operation = (OperationTypeEnum)Enum.Parse(typeof(OperationTypeEnum),
                ((DataRowView)item.DataItem)["Operation"].ToString(), true);
            string operationStr = null;
            Panel OperationPanel = item.FindControl("OperationPanel") as Panel;

            switch (operation)
            {
                case OperationTypeEnum.Delete:
                    operationStr = "Deleted";
                    OperationPanel.CssClass = "reminder-red";
                    break;
                case OperationTypeEnum.Insert:
                    operationStr = "Inserted";
                    OperationPanel.CssClass = "reminder-green";
                    break;
                case OperationTypeEnum.Update:
                    operationStr = "Updated";
                    OperationPanel.CssClass = "reminder-green";
                    break;
            }

            string auditDate = Format.FormatDateTime(((DataRowView)item.DataItem)["AuditDate"], DateTimeFormat.d__MMM__yyyy_C_h_Col_mm__tt);
            Guid userId = new Guid(((DataRowView)item.DataItem)["UserId"].ToString());
            
            ProfileDb profileDb = new ProfileDb();
            string fullName = profileDb.GetUserFullName(userId);

            UserDb userDb = new UserDb();
            string role = userDb.GetRole(userId);

            if (role != null)
            {
                role = role.ToString().Replace("_", " ");
            }
            else
            {
                role = "Anonymous User";
            }

            string ip = ((DataRowView)item.DataItem)["Ip"].ToString();
            string systemInfo = ((DataRowView)item.DataItem)["SystemInfo"].ToString();

            Label OperationLabel = item.FindControl("OperationLabel") as Label;
            OperationLabel.Text = operationStr + " by " + fullName + " (" + role + ") on " + auditDate + " from " + ip;

            Label SystemInfoLabel = item.FindControl("SystemInfoLabel") as Label;
            SystemInfoLabel.Text = systemInfo;
        }
    }

    /// <summary>
    /// Rad grid item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = (GridDataItem)e.Item;

            string version = "Version " + (count - e.Item.ItemIndex);
            string operation = (string)DataBinder.Eval(e.Item.DataItem, "Operation");
            string operationId = DataBinder.Eval(e.Item.DataItem, "OperationId").ToString();

            dataItem["Version"].Text = "<a title='View record detail' href=\"#" + operationId + "\">" + version + "</a>";
            dataItem["Operation"].Text = "<a title='View record detail' href=\"#" + operationId + "\">" + operation + "</a>";
        }
    }
}

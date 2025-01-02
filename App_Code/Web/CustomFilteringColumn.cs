using System.Data;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI;
using System;
using System.Collections;

namespace Dwms.Web
{
    public class CustomFilteringColumn : GridTemplateColumn
    {
        private object listDataSource = null;
        private string dataTextField = null;
        private string dataValueField = null;
        //RadGrid calls this method when it initializes the controls inside the filtering item cells
        protected override void SetupFilterControls(TableCell cell)
        {
            base.SetupFilterControls(cell);
            cell.Controls.RemoveAt(0);
            DropDownList list = new DropDownList();
            list.ID = "list" + this.DataField;

            list.AutoPostBack = true;
            list.SelectedIndexChanged += new EventHandler(list_SelectedIndexChanged);
            list.DataBound += new EventHandler(list_DataBound);
            cell.Controls.AddAt(0, list);
            cell.Controls.RemoveAt(1);
            list.DataTextField = DataTextField;
            list.DataValueField = DataValueField;
            list.DataSource = this.ListDataSource;
            list.DataBind();
        }
        void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridFilteringItem filterItem = (sender as DropDownList).NamingContainer as GridFilteringItem;

            if ((sender as DropDownList).SelectedIndex > 0)
            {
                if (this.DataType == System.Type.GetType("System.Int32") ||
                    this.DataType == System.Type.GetType("System.Int16") ||
                    this.DataType == System.Type.GetType("System.Int64") ||
                    this.DataType == System.Type.GetType("System.Boolean") ||
                    this.DataType == System.Type.GetType("System.String") ||
                    this.DataType == System.Type.GetType("System.Decimal") ||
                    this.DataType == System.Type.GetType("System.Double"))
                {
                    filterItem.FireCommandEvent("Filter", new Pair("EqualTo", this.UniqueName));
                }
                else // treat everything else like a string
                    filterItem.FireCommandEvent("Filter", new Pair("Contains", this.UniqueName));
            }
            else
            {
                filterItem.FireCommandEvent("Filter", new Pair("NoFilter", this.UniqueName));
            }
        }
        void list_DataBound(object sender, EventArgs e)
        {
            ListItem item = new ListItem("- All -");
            (sender as DropDownList).Items.Insert(0, item);
        }
        public object ListDataSource
        {
            get { return this.listDataSource; }
            set { listDataSource = value; }
        }

        public string DataTextField
        {
            get { return dataTextField; }
            set { dataTextField = value; }
        }

        public string DataValueField
        {
            get { return dataValueField; }
            set { dataValueField = value; }
        }

        //RadGrid calls this method when the value should be set to the filtering input control(s)
        protected override void SetCurrentFilterValueToControl(TableCell cell)
        {
            base.SetCurrentFilterValueToControl(cell);
            DropDownList list = (DropDownList)cell.Controls[0];
            if (this.CurrentFilterValue != string.Empty)
            {
                list.SelectedValue = this.CurrentFilterValue;
            }
        }
        //RadGrid calls this method to extract the filtering value from the filtering input control(s)
        protected override string GetCurrentFilterValueFromControl(TableCell cell)
        {
            DropDownList list = (DropDownList)cell.Controls[0];
            return list.SelectedValue;
        }
        protected override string GetFilterDataField()
        {
            return this.DataField;
        }
    }
}
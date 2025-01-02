using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentTableAdapters;
using Telerik.Web.UI;

using Dwms.Bll;
using Dwms.Web;

public partial class Edit_Department : System.Web.UI.Page
{
    int? id;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Request["id"]))
        {
            id = null;
        }
        else
        {
            id = int.Parse(Request["id"]);
        }

        if (!IsPostBack)
        {
            if (id == null)
            {
                NameTextBox.Focus();
            }
            else
            {
                TitleLabel.Text = "Edit Department";

                DepartmentDb departmentDb = new DepartmentDb();

                Department.DepartmentDataTable department = departmentDb.GetDepartmentById(id.Value);

                if (department.Count == 0)
                {
                    Response.Redirect("~/Default.aspx");
                }

                Department.DepartmentRow row = department[0];

                NameTextBox.Text = row.Name.Trim();
                CodeTextBox.Text = row.Code.Trim();
                MailingListTextBox.EmailValue = row.IsMailingListNull() ? string.Empty : row.MailingList.Trim();
            }
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

       if (Page.IsValid)
        {
            DepartmentDb departmentDb = new DepartmentDb();

            if (id == null)
            {
                id = departmentDb.Insert(NameTextBox.Text.Trim(), CodeTextBox.Text.Trim(), MailingListTextBox.EmailValue.Trim());
            }
            else
            {
                departmentDb.Update(id.Value, NameTextBox.Text.Trim(), CodeTextBox.Text.Trim(), MailingListTextBox.EmailValue.Trim());
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;
            SubmitPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", "ResizeAndClose(600, 190);", true);
        }
    }

    protected void DuplicateNameCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (NameRequiredFieldValidator.IsValid)
        {
            DepartmentDb departmentDb = new DepartmentDb();
            args.IsValid = !departmentDb.NameExists(NameTextBox.Text.Trim(), id);
        }
    }

    protected void DuplicateCodeCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CodeRequiredFieldValidator.IsValid)
        {
            DepartmentDb departmentDb = new DepartmentDb();
            args.IsValid = !departmentDb.CodeExists(CodeTextBox.Text, id);
        }
    }
}
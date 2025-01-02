using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SectionTableAdapters;
using Telerik.Web.UI;

using Dwms.Bll;
using Dwms.Web;

public partial class Edit_Section : System.Web.UI.Page
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
                TitleLabel.Text = "Edit Section";

                SectionDb sectionDb = new SectionDb();

                Section.SectionDataTable section = sectionDb.GetSectionById(id.Value);

                if (section.Count == 0)
                {
                    Response.Redirect("~/Default.aspx");
                }

                Section.SectionRow row = section[0];

                NameTextBox.Text = row.Name.Trim();
                CodeTextBox.Text = row.Code.Trim();
                BusinessCodeTextBox.Text = row.BusinessCode.Trim();

                //set the department label
                DepartmentDb departmentDb = new DepartmentDb();
                Department.DepartmentDataTable department = departmentDb.GetDepartmentById(row.Department);

                Department.DepartmentRow  dRow = department[0];

                DepartmentLabel.Text = dRow.Code + " - " + dRow.Name; 
            }
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        Page.Validate();

       if (Page.IsValid)
        {
            SectionDb sectionDb = new SectionDb();

            if (id == null)
            {
                id = sectionDb.Insert(NameTextBox.Text.Trim(), CodeTextBox.Text.Trim(), BusinessCodeTextBox.Text.Trim());
            }
            else
            {
                sectionDb.Update(id.Value, NameTextBox.Text.Trim(), CodeTextBox.Text.Trim(), BusinessCodeTextBox.Text.Trim());
            }

            ConfirmPanel.Visible = true;
            FormPanel.Visible = false;
            SubmitPanel.Visible = false;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ResizeScript", "ResizeAndClose(600, 190);", true);
        }
    }

    protected void DuplicateCodeCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (CodeRequiredFieldValidator.IsValid)
        {
            SectionDb sectionDb = new SectionDb();
            args.IsValid = !sectionDb.CodeExists(CodeTextBox.Text, id);
        }
    }
}
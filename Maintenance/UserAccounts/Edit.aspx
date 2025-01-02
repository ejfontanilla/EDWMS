<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="Maintenance_UserAccounts_Edit" Title="DWMS - User Accounts" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="New User" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Account Information</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr runat="server">
                    <td valign="top">
                        <span class="label">Role </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="RoleDropDownList" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value" AutoPostBack="false">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Name <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="Name" Columns="30" MaxLength="50" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Name"
                            Display="Dynamic" ErrorMessage="<br />Name is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Staff ID <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="Nric" Columns="30" MaxLength="65" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="Nric"
                            Display="Dynamic" ErrorMessage="<br />Staff ID is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="NricCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                            ForeColor="" CssClass="form-error" ErrorMessage='<br/>Staff ID is incorrect, or an account with the same Staff ID already exists.'
                            OnServerValidate="ValidateNric"></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Email <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="Email" Columns="30" MaxLength="50" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" ControlToValidate="Email"
                            Display="Dynamic" ErrorMessage="<br />Email is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="EmailRegularExpressionValidator" runat="server"
                            ControlToValidate="Email" Display="Dynamic" ErrorMessage='<br />Email is incorrect, it should be in a format like "name@domain.com".'
                            ValidationExpression="(\s)*\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*(\s)*" ForeColor=""
                            CssClass="form-error" EnableClientScript="false" Enabled="false">
                        </asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="EmailCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                            ErrorMessage='<br />An account for the email already exists. Please use another one.'
                            OnServerValidate="ValidateEmail" ForeColor="" CssClass="form-error" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Designation <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="Designation" Columns="30" MaxLength="50" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Designation"
                            Display="Dynamic" ErrorMessage="<br />Designation is required." ForeColor=""
                            CssClass="form-error"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Status <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="StatusRadioButtonList" RepeatDirection="Horizontal" Width="150px" CssClass="list-item-table hand"></asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Department <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="DepartmentRadComboBox" runat="server" ZIndex="30000" Height="200"
                            Width="400" DropDownWidth="400" AppendDataBoundItems="false" 
                            ExpandAnimation-Duration="100" AutoPostBack="true" CausesValidation="false"
                            CollapseAnimation-Duration="200" OnSelectedIndexChanged="DepartmentRadComboBox_SelectedIndexChanged">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Section <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="SectionRadComboBox" runat="server" ZIndex="30000" Height="200"
                            Width="400" DropDownWidth="400" AppendDataBoundItems="false" 
                            ExpandAnimation-Duration="100" AutoPostBack="true" CausesValidation="false"
                            CollapseAnimation-Duration="200">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Team <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="TeamRadComboBox" runat="server" ZIndex="30000" Height="200"
                            Width="400" DropDownWidth="400" AppendDataBoundItems="false" 
                            ExpandAnimation-Duration="100" AutoPostBack="true" CausesValidation="false"
                            CollapseAnimation-Duration="200">
                        </telerik:RadComboBox>
                    </td>
                </tr>
           </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:history.back(1);">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false'>
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function ShowContactWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="Edit" Title="DWMS - Role Management" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:Label ID="TitleLabel" runat="server" CssClass="title" Text="New Role"></asp:Label>
    <div class="bottom10">
        <!---->
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10" Visible="false">
        The role has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SaveButton">
        <div class="header">
            <div class="left">
                Role Information</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td width="25%">
                        <span class="label">Role Name <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="RoleNameTextBox" Columns="30" MaxLength="256" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RoleNameRequiredFieldValidator" runat="server" ControlToValidate="RoleNameTextBox"
                            Display="Dynamic" ErrorMessage="<br />Role name is required." CssClass="form-error" Enabled="true">
                        </asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="DuplicateNameCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage="<br/>Duplicate role name exists." CssClass="form-error"
                            OnServerValidate="DescriptionCustomValidator_ServerValidate" >
                        </asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Department<span class="form-error">*</span></span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="DepartmentRadComboBox" runat="server" ZIndex="30000" Height="100"
                            Width="300" DropDownWidth="300" AppendDataBoundItems="false" 
                            ExpandAnimation-Duration="100" AutoPostBack="true" CausesValidation="false"
                            CollapseAnimation-Duration="200" OnSelectedIndexChanged="DepartmentRadComboBox_OnSelectedIndexChanged">
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </table>             
        </div>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
        <asp:Button ID="SaveButton" runat="server" Text="Save" 
            CssClass="button-large right20" onclick="SaveButton_Click"/>
        <a href="javascript:GetRadWindow().close();">Cancel</a>
    </asp:Panel>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false'>
    </telerik:RadWindow>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SaveButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                    <telerik:AjaxUpdatedControl ControlID="SubmitPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>

           
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

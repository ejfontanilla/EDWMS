<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="View.aspx.cs" Inherits="Maintenance_UserAccounts_View" Title="DWMS - User Accounts" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable to save this account. You may not have permission to perform this
        operation.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The account information has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
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
                        <span class="label">Role </span>
                    </td>
                    <td>
                        <asp:Label ID="Role" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Name </span>
                    </td>
                    <td>
                        <asp:Label ID="Name" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Staff ID </span>
                    </td>
                    <td>
                        <asp:Label ID="Nric" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Email</span>
                    </td>
                    <td>
                        <asp:Label ID="Email" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Designation</span>
                    </td>
                    <td>
                        <asp:Label ID="Designation" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Status</span>
                    </td>
                    <td>
                        <asp:Label ID="Status" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Department</span>
                    </td>
                    <td>
                        <asp:Label ID="GroupLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Section</span>
                    </td>
                    <td>
                        <asp:Label ID="SectionLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Team</span>
                    </td>
                    <td>
                        <asp:Label ID="TeamLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Business Code</span>
                    </td>
                    <td>
                        <asp:Label ID="BusinessCodeLabel" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <input id="EditButton" type="button" value="Edit" class="button-large right20" runat="server" />
            <asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button-large right20"
                UseSubmitBehavior="false" OnClick="Delete" OnClientClick="if(confirm('Are you sure you want to delete this user?') == false) return false;" Visible="true" />
            <a href="javascript:history.back(1);">Cancel</a>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SubmitPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="WarningPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="View.aspx.cs" Inherits="Maintenance_EmailTemplates_View" Title="DWMS - Email Templates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The email template has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Email Template Information</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr runat="server">
                    <td valign="top">
                        <span class="label">Template Description </span>
                    </td>
                    <td>
                        <asp:Label ID="DescriptionLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Subject </span>
                    </td>
                    <td>
                        <asp:Label ID="SubjectLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="25%" valign="top">
                        <span class="label">Content </span>
                    </td>
                    <td>
                        <asp:Label ID="ContentLabel" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">        
            <asp:Button ID="EditButton" runat="server" CssClass="button-large right20" 
                Text="Edit" onclick="EditButton_Click" />
            <asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button-large right20"                
                OnClientClick="if(confirm('Are you sure you want to delete this template?') == false) return false;" 
                onclick="DeleteButton_Click" Visible="false" />
            <a href="javascript:history.back(1);">Cancel</a>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

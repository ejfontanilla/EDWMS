<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_MasterList_Default" Title="DWMS - Master List" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Master List" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable to save the list. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The list has been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                List Details</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr runat="server">
                    <td valign="top">
                        <span class="label">List </span>
                    </td>
                    <td colspan="2">
                        <asp:DropDownList ID="ListDropDownList" runat="server" CssClass="form-field" 
                            AutoPostBack="true" Width="300px" 
                            onselectedindexchanged="ListDropDownList_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td valign="top">
                        <telerik:RadListBox ID="ValuesRadListBox" runat="server" AllowDelete="True" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="True">
                        </telerik:RadListBox><br /><br />
                        <telerik:RadButton ID="AddRadButton" runat="server" Skin="Windows7" Text="Add" 
                            Width="50px" AutoPostBack="false">
                        </telerik:RadButton>
                        <asp:CustomValidator ID="ValuesRadListBoxCustomValidator" runat="server" 
                            ErrorMessage="<br />Please add at least 1 value." CssClass="form-error" 
                            Display="Dynamic" onservervalidate="ValuesRadListBoxCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <%--<a href="javascript:history.back(1);">Cancel</a> --%>       
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
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
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(condition) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(condition);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

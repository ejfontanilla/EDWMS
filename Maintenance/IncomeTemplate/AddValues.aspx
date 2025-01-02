<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddValues.aspx.cs" Inherits="Maintenance_IncomeTemplate_AddValues"
    MasterPageFile="~/Blank.master" Title="DWMS - Income Template Item" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Income Component" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                <asp:Label runat="server" ID="HeaderLabel" Text="Add Income Component"></asp:Label></div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr id="Tr1" runat="server">
                    <td valign="top">
                        <span class="label">Name</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="middle">
                        <asp:TextBox ID="ValueTextBox" runat="server" Text='<%# Eval("Keyword") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="KeywordRequiredFieldValidator" runat="server" ErrorMessage="<br />Please enter a value."
                            Display="Dynamic" CssClass="form-error" ControlToValidate="ValueTextBox">
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr id="Tr2" runat="server">
                    <td valign="top">
                        <span class="label">Check the Income Type</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="middle">
                        <asp:CheckBox runat="server" ID="chkGrossIncome" Text="Gross Income" />
                        <asp:CheckBox runat="server" ID="chkAllowance" Text="Allowance" />
                        <asp:CheckBox runat="server" ID="chkOvertime" Text="Overtime" />
                        <asp:CheckBox runat="server" ID="chkAHGIncome" Text="AHG Income" />
                        <asp:CheckBox runat="server" ID="chkCAIncome" Text="CA Income" />
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <asp:Button ID="DeleteButton" runat="server" Text="Delete" CssClass="button-large right20"
                OnClick="Delete" />
            <a href="javascript:GetRadWindow().close();">Cancel</a>
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
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(condition) {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(condition);
            }

            function DeleteThenCloseWindow(condition) {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(condition);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

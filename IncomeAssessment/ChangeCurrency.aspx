<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeCurrency.aspx.cs" Inherits="IncomeAssessment_ChangeCurrency"
    Title="DWMS - Change Currency(Income Extraction)" MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Change Currency" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        <asp:Label ID="lblConfirm" runat="server" CssClass="list" Text="Change of Currency Successful."></asp:Label>
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="btnConfirm">
        <table>
            <tr>
                <td width="75">
                    <center>
                        <asp:Label runat="server" ID="Label1" Text="Currency"></asp:Label></center>
                </td>
                <td>
                    <telerik:RadComboBox runat="server" ID="CurrencyDropdownList" Skin="Windows7" AutoPostBack="true"
                        Width="100px" DropDownWidth="100px" Height="60px" MarkFirstMatch="true" CausesValidation="false"
                        Visible="true" />
                </td>
                <td width="50">
                    <center>
                        <asp:Label runat="server" ID="toLabel" Text="  /  Value"></asp:Label></center>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="CurrencyRateTextbox" Width="100"></asp:TextBox>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <span class="form-error">
                        <asp:Label ID="ErrorLabel" runat="server"></asp:Label></span>
                </td>
            </tr>
        </table>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button-large right20"
                OnClick="btnConfirm_Click" />
            <asp:Button ID="btnDefault" runat="server" Text="Default" CssClass="button-large right20"
                OnClick="btnConfirm_Click" ToolTip="Set the Currency to SGD." />
            <a href="javascript:GetRadWindow().close();">Cancel</a>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
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
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(null);

            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage();

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

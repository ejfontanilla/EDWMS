<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IncomeVersionEditDelete.aspx.cs"
    Inherits="IncomeAssessment_IncomeVersionEditDelete" Title="DWMS - Edit Income Version(Income Assessment)"
    MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Income Version" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        <asp:Label ID="lblConfirm" runat="server" CssClass="list" Text=""></asp:Label>
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="btnConfirm">
        <table>
            <tr>
                <td width="50%">
                    <asp:Label runat="server" ID="lblDelete" Text="Do you want to delete this version?" CssClass="label" Visible="false"></asp:Label>
                    <asp:Label runat="server" ID="lblEdit" Text="Version Name" CssClass="label"></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtVersionName" Width="200" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
        </table>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CssClass="button-large right20"
                OnClick="btnConfirm_Click" />
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
                setTimeout('GetRadWindow().close()', 1);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SetIncomeToZero.aspx.cs"
    Inherits="IncomeAssessment_SetIncomeToZero" Title="DWMS - Set Income to Zero"
    MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Set Income to Zero" visible="false"/>
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
         EnableViewState="false">
        <asp:Label ID="lblConfirm" runat="server" CssClass="list" Text="Setting income to Zero..."></asp:Label>
    </asp:Panel>
       <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="reminder-red top10 bottom15">
            <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" />
        </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton" Visible="false">
        <span class="label">This will set all the income to zero. Click the Set button to conitnue.</span>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Set" CssClass="button-large right20"
                OnClick="Save" />
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
                    <telerik:AjaxUpdatedControl ControlID="WarningPanel" />
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
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 2000);
                GetRadWindow().BrowserWindow.UpdateParentPage();

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

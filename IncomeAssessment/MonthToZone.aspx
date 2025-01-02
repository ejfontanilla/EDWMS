<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MonthToZone.aspx.cs" Inherits="IncomeAssessment_MonthToZone"
Title="DWMS - Month to Zone" MasterPageFile="~/Blank.master" %>


<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Month to Zone" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                <asp:Label runat="server" ID="labelComponent"></asp:Label></div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr id="Tr1" runat="server">
                    <td valign="top">
                        <span class="label">Month</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                    <telerik:RadComboBox runat="server" ID="MonthYearDropDownList" Skin="Windows7" AutoPostBack="true"
                        Width="150px" DropDownWidth="150px"  Height="75px" MarkFirstMatch="true" CausesValidation="false"
                        Visible="true" />
                </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Zone" CssClass="button-large right20"
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
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

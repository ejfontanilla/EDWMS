<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CopyZoning.aspx.cs" Inherits="IncomeAssessment_CopyZoning"
    Title="DWMS - Copy" MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Copy to all Months" /></div>
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
            <table >
                <tr>
                    <td valign="top" width="100">
                        <span class="label">Include Amount </span>
                    </td>
                    <td valign="middle" width="100">
                        <asp:RadioButtonList runat="server" ID="radIncludeAmount" OnSelectedIndexChanged="radIncludeAmount_SelectedIndexChanged"
                            RepeatDirection="Horizontal">
                            <asp:ListItem Text="Yes" Value="y" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="n" ></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Select All Months </span>
                    </td>
                    <td valign="middle">
                        <asp:RadioButtonList runat="server" ID="radSelectAll" OnSelectedIndexChanged="radSelectAll_SelectedIndexChanged"
                            AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Yes" Value="y" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="No" Value="n" ></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td></td>
                </tr>                
            </table>
            <table>
            <tr>
                    <td valign="top">
                        <span class="label">Months</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <asp:CheckBoxList runat="server" ID="chkboxMonths" RepeatColumns="3">
                        </asp:CheckBoxList>
                        <asp:CustomValidator OnServerValidate="valMonths_ServerValidation" ID="valMonths"
                            EnableClientScript="true" ClientValidationFunction="verifyCheckboxList" ErrorMessage="At least 1 Month is required."
                            runat="server" CssClass="form-error" ForeColor="">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Copy" CssClass="button-large right20"
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

            function verifyCheckboxList(source, arguments) {
                var val = document.getElementById("<%= chkboxMonths.ClientID %>");
                var col = val.getElementsByTagName("*");
                if (val.style.display != 'none') {
                    if (col != null) {
                        for (i = 0; i < col.length; i++) {
                            if (col.item(i).tagName == "INPUT") {
                                if (col.item(i).checked) {
                                    arguments.IsValid = true;
                                    return;
                                }
                            }
                        }
                    }
                }
                arguments.IsValid = false;
            }


            function CheckAll() {


                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest('all');

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

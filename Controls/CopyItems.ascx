<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CopyItems.ascx.cs" Inherits="Controls_CopyItems" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Copy to all Months" /></div>
    <div class="header">
        <div class="left">
        </div>
        <div class="right">
            <!---->
        </div>
    </div>
    <div class="area">
        <asp:Table runat="server" ID="tblSource">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top">
                    <asp:Label runat="server" ID="lblFrom" Text="Copy From:" CssClass="label"></asp:Label>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Middle">
                    <asp:RadioButtonList runat="server" ID="RadioButtonList1" AutoPostBack="true" RepeatDirection="Horizontal"
                        Visible="false">
                    </asp:RadioButtonList>
                    <asp:RadioButtonList runat="server" ID="RadMonthFrom" RepeatDirection="Horizontal"
                        RepeatColumns="3">
                    </asp:RadioButtonList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Table runat="server" ID="tblSelectAllMonths" Visible="false">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top">
                    <asp:Label runat="server" ID="lblIncludeAmount" Text="Include Amount:" CssClass="label"></asp:Label>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Middle">
                    <asp:RadioButtonList runat="server" ID="radIncludeAmount"  RepeatDirection="Horizontal">
                        <asp:ListItem Text="Yes" Value="y" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="n"></asp:ListItem>
                    </asp:RadioButtonList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top">
                    <asp:Label runat="server" ID="lblSelectAll" Text="Select All Months:" CssClass="label"></asp:Label>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Middle">
                    <asp:RadioButtonList runat="server" ID="radSelectAll" AutoPostBack="true" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="radSelectAll_SelectedIndexChanged">
                        <asp:ListItem Text="Yes" Value="y" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="n"></asp:ListItem>
                    </asp:RadioButtonList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Table runat="server" ID="tblDestination" Visible="false">
            <asp:TableRow>
                <asp:TableCell VerticalAlign="Top">
                    <asp:Label runat="server" ID="lblDestination" Text="Copy To:" CssClass="label"><span class="form-error">&nbsp;*</span></asp:Label>
                </asp:TableCell>
                <asp:TableCell VerticalAlign="Middle">
                    <asp:CheckBoxList runat="server" ID="chkboxMonths" RepeatColumns="3">
                    </asp:CheckBoxList>
                    <asp:CustomValidator ID="valMonths" EnableClientScript="true" ClientValidationFunction="verifyCheckboxList"
                        ErrorMessage="At least 1 month is required." runat="server" CssClass="form-error"
                        ForeColor="" OnServerValidate="valMonths_ServerValidation">
                    </asp:CustomValidator>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <%--<table>
            <tr>
                <td valign="top">
                    <span class="label">Select All Items </span>
                </td>
                <td valign="middle">
                    <asp:RadioButtonList runat="server" ID="RadItems" AutoPostBack="true" RepeatDirection="Horizontal"
                        OnSelectedIndexChanged="RadItems_SelectedIndexChanged">
                        <asp:ListItem Text="Yes" Value="y" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="No" Value="n"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td valign="top">
                    <span class="label">Items</span><span class="form-error">&nbsp;*</span>
                </td>
                <td>
                    <asp:CheckBoxList runat="server" ID="chkboxItems" RepeatColumns="3">
                    </asp:CheckBoxList>
                    <asp:CustomValidator ID="CustomValidator1" EnableClientScript="true" ClientValidationFunction="verifyCheckboxListItems"
                        ErrorMessage="At least 1 item is required." runat="server" CssClass="form-error"
                        ForeColor="" OnServerValidate="valItems_ServerValidation">
                    </asp:CustomValidator>
                </td>
            </tr>
        </table>--%>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:CancelActiveToolTip();">Cancel</a>
        </asp:Panel>
    </div>
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

            function verifyCheckboxListItems(source, arguments) {
                
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

            function CheckUncheckMonths() {
                var months = document.getElementById("<%= chkboxMonths.ClientID %>");
                var monthsYesNo = document.getElementById("<%= radSelectAll.ClientID %>");
                for (int x = 0; x < listBox1.Items.Count; x++)
                {
                    if (monthsYesNo.value == "Yes")                   
                        months.SetItemChecked(x, true);
                    else
                        months.SetItemChecked(x, false);
                }
            }


            function CheckAll() {


                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest('all');

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Panel>

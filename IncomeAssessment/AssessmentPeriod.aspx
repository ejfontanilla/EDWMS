<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AssessmentPeriod.aspx.cs"
    Inherits="IncomeAssessment_AssessmentPeriod" Title="DWMS - Change Extraction Period (Income Extraction)"
    MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Extraction Period" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        <asp:Label ID="lblConfirm" runat="server" CssClass="list" Text="Change of Extraction Period Successful."></asp:Label>
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="btnConfirm">
        <div class="header">
            <div class="left">
                <asp:Label runat="server" ID="labelComponent">Change of Extraction Period</asp:Label></div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table >
            <tr><td>
            <asp:Label runat="server" ID="Label3" Text="Not Applicable" CssClass="label"></asp:Label>
                <asp:CheckBox runat="server" ID="ChknotApplicable"  CssClass="label" 
                    oncheckedchanged="ChknotApplicable_CheckedChanged" AutoPostBack="true"  />
            </td></tr>
            </table>
            <table width="100%">
                <tr>
                    <td>
                        
                            <asp:Label runat="server" ID="Label2" Text="Period" CssClass="label"></asp:Label>
                    </td>
                    <td width="190">
                        <telerik:RadComboBox runat="server" ID="Month1" Skin="Windows7" AutoPostBack="false"
                            Width="90px" DropDownWidth="100px" Height="75px" MarkFirstMatch="true" CausesValidation="false"
                            Visible="true" />
                        <telerik:RadComboBox runat="server" ID="Year1" Skin="Windows7" AutoPostBack="false"
                            Width="80px" DropDownWidth="80px" Height="75px" MarkFirstMatch="true" CausesValidation="false"
                            Visible="true" />
                    </td>
                    <td width="10">
                        <center>
                            <asp:Label runat="server" ID="toLabel" Text="to"></asp:Label></center>
                    </td>
                    <td>
                        <telerik:RadComboBox runat="server" ID="Month2" Skin="Windows7" AutoPostBack="false"
                            Width="90px" DropDownWidth="100px" Height="75px" MarkFirstMatch="true" CausesValidation="false"
                            Visible="true"  />
                        <telerik:RadComboBox runat="server" ID="Year2" Skin="Windows7" AutoPostBack="false"
                            Width="80px" DropDownWidth="80px" Height="75px" MarkFirstMatch="true" CausesValidation="false"
                            Visible="true"  />
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td width="150" valign="bottom">
                        <asp:Label runat="server" ID="Label1" Text="Months to pass to LEAS " CssClass="label"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblMonth"></asp:Label>
                       <%-- <asp:TextBox runat="server" ID="TxtMonth" Width="50" Text="12"></asp:TextBox>--%>
                        <asp:CustomValidator ID="AdjustmentCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage="" OnServerValidate="AdjustmentCustomValidator_ServerValidate" />
                    </td>
                </tr>
            </table>
        </div>
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
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage();

            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

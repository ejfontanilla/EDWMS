<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreditAssessment.aspx.cs"
    Inherits="IncomeAssessment_CreditAssessment" Title="DWMS - Credit Assessment"
    MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/WebServices/IncomeExtractionWebSvr.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Credit Assessment" /></div>
    
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                <asp:Label runat="server" ID="labelComponent" ></asp:Label></div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <asp:Panel ID="InfoPanel" runat="server">   
            <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" style="display: none;" Text="Enter valid amount. " />         
            </asp:Panel>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save and Close" CssClass="button-large right20"
                OnClick="Save" Width="150" />
           <%-- <a href="javascript:GetRadWindow().close();">Cancel</a>--%>
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


            function UpdateCreditAssessment(textbox, appPersonalId, incomeComponent, incomeType, enteredBy) {
                //if (Page_ClientValidate("")) {
                    var docType = document.getElementById("<%= SubmitButton.ClientID %>");
                    docType.disabled = false;
                    var ca = textbox.value;
                    //alert(value + " || " + appPersonalId + " || " + incomeComponent + " || " + incomeType);
                    IncomeExtractionWebSvr.UpdateCreditAssessment(ca, appPersonalId, incomeComponent, incomeType, enteredBy, SucceededCallback, FailedCallback);
                //}
                //else {
                //    var docType = document.getElementById("<%= SubmitButton.ClientID %>");
                //    docType.disabled = true;
                //}
            }


            function SucceededCallback(count) {
                var a = document.getElementById("<%= InfoLabel.ClientID %>");
                a.style.display = 'none';
                var docType = document.getElementById("<%= SubmitButton.ClientID %>");
                docType.disabled = false;    
                if (count == "1")
                    alert("Operation failed. Please try again.");
            }

            function FailedCallback(error) {
                var a = document.getElementById("<%= InfoLabel.ClientID %>");
                a.style.display = '';
                var docType = document.getElementById("<%= SubmitButton.ClientID %>");
                docType.disabled = true;              
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

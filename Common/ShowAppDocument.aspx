<%@ Page Title="DWMS - Download Documents (Completeness)" Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="ShowAppDocument.aspx.cs" Inherits="Completeness_ShowAppDocument" ValidateRequest="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server" >
<div class="title">
<asp:Label ID="TitleLabel" runat="server" Text="Download Document" />
</div>
<asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
    Visible="false" EnableViewState="false">
        <asp:label runat="server" ID="ConfirmLabel" Font-Bold="false"></asp:label>
</asp:Panel>
<asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="DownloadPdfButton">
    <div class="header">
        <div class="left">
            Choose documents to download</div>
        <div class="right">
            <!---->
        </div>
    </div>
    <div class="area">
        <table style="width:100%;">
            <tr>
                <td>
                    <telerik:RadTreeView ID="RadTreeView1" runat="server" Skin="Windows7" AccessKey="T" 
                        SingleExpandPath="False" Width="100%" Height="350px" 
                        CausesValidation="false" TriStateCheckBoxes="True" CheckBoxes="true" BorderWidth="1"  OnClientNodeChecked="ClientNodeChecked" >
                    </telerik:RadTreeView>
                    <asp:CustomValidator ID="RadTreeView1CustomValidator" runat="server" Enabled="true" 
                        ErrorMessage="<br />Please select at least 1 document to download." CssClass="form-error" 
                        Display="Dynamic" onservervalidate="RadTreeView1CustomValidator_ServerValidate">
                    </asp:CustomValidator>
                </td>
            </tr>
        </table>
        <table style="width:210px;" cellpadding="0" cellspacing="0">
            <tr>
                <td align="left" style="width:120px;"><asp:Label runat="server" ID="Label1" Visible="true" Text="Selected File Size :"></asp:Label></td>
                <td align="left" style="width:90px;"><asp:Label runat="server" ID="FileSizeLabel" Visible="true" Text="0.00 Bytes"></asp:Label></td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button runat="server" ID="DownloadPdfButton" CssClass="button-large2" Text="Download as Pdf" OnClick="DownloadPdfButton_onClick"/>
            <asp:Button runat="server" ID="DownloadZipButton" CssClass="button-large2" Text="Download as Zip" OnClick="DownloadZipButton_onClick"/>
            &nbsp;&nbsp;&nbsp;<a href="javascript:GetRadWindow().close();">Close</a>        
    </asp:Panel>    
    <asp:HiddenField runat="server" ID="urlTargetHiddenField" />
</asp:Panel>
<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="FormPanel">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="FormPanel"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
</telerik:RadAjaxLoadingPanel>
<asp:ScriptManagerProxy ID="ScriptManager1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/WebServices/FunctionWebService.asmx" />
    </Services>
</asp:ScriptManagerProxy>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ClientNodeChecked(sender, args) {
                var tree = $find("<%= RadTreeView1.ClientID %>");
                var selectedIndeces = "";
                var TotalSize = 0;

                //loop through all the nodes
                for (var i = 0; i < tree.get_allNodes().length; i++) {

                    var node = tree.get_allNodes()[i];

                    //if document
                    if (node.get_category() == 'Doc') {

                        if (node.get_checked() == true) {
                            selectedIndeces = selectedIndeces + node.get_value() + ",";
                        }
                    }
                }
                selectedIndeces = selectedIndeces.substr(0, selectedIndeces.lastIndexOf(','));
                if (document.URL.indexOf("/completeness/") == -1) {
                    FunctionWebService.GetDocumentZippedFileSizeByIdsWithFormatSize(selectedIndeces, "<%= docAppId %>", CallSuccess, CallFailed);
                }
                else {
                    FunctionWebService.GetDocumentFileSizeByIdsWithFormatSize(selectedIndeces, CallSuccess, CallFailed);
                }
            }

            function CallSuccess(result) {
                var fileSizeLabel = document.getElementById('<%=FileSizeLabel.ClientID %>');
                fileSizeLabel.innerHTML = result;
            }

            function CallFailed(error) {
                //alert("Operation failed. Please try again.");
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ResizeAndClose(width, height, url) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.RefreshUrl(url);
            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Import_UploadDocuments_Default" Title="DWMS - Upload Documents" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Bll" TagPrefix="DwmsBll" %>
<%@ Register Src="~/Import/Controls/ScanUploadFields.ascx" TagName="ScanUploadFields" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">Upload Documents</div>
    <asp:Panel ID="ConfirmPanel" runat="server" Visible="false" CssClass="reminder-green top10 bottom15" >
        The set has been saved.
    </asp:Panel>
    <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="reminder-red top10 bottom15" >
        There was an error while saving the set.&nbsp;&nbsp;Please try again.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Upload Documents</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <uc:ScanUploadFields ID="ScanUploadFields" runat="server" />
            <table>
                <tr>
                    <td class="import-left">
                        <span class="label">Documents</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <telerik:RadAsyncUpload ID="DocumentRadAsyncUpload" runat="server" Skin="Default" 
                            Localization-Select="Browse" ControlObjectsVisibility="Default" Enabled="True" 
                            MultipleFileSelection="Automatic" UploadedFilesRendering="BelowFileInput" 
                            TemporaryFileExpiration="01:00:00"
                            AllowedFileExtensions=".PDF,.pdf,.JPG,.jpg,.JPEG,.jpeg,.PNG,.png,.TIF,.tif,.TIFF,.tiff">
                        </telerik:RadAsyncUpload>
                        <span class="form-note">Multiple files can be selected at once.</span>
                        <asp:CustomValidator ID="DocumentRadUploadCustomValidator" runat="server" 
                            ErrorMessage="<br />Please choose file(s) to upload." CssClass="form-error" Display="Dynamic" 
                            OnServerValidate="DocumentRadUploadCustomValidator_ServerValidate">
                        </asp:CustomValidator>

                        <div id="ProgressPanel" style="display:none; width: 240px;" class="top20">
                            <div id="ProgressbarOuter" class="progressbar">
                                <div id="ProgressbarInner"><!----></div>
                            </div>
                            <label id="ProgressLabel">Preparing pages for categorization...</label><br />
                            <span>Elapsed Time:&nbsp;</span><span id="ElapsedTimeHour">00</span>:<span id="ElapsedTimeMinute">00</span>:<span id="ElapsedTimeSecond">00</span>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="Panel2" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Upload" CssClass="button-large right20" style="width: 90px"
                UseSubmitBehavior="false" onclick="SubmitButton_Click" />
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="True" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">                
    </telerik:RadAjaxLoadingPanel>
    <asp:HiddenField ID="ReferenceTypeHiddenField" runat="server" />
    <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
        <script type="text/javascript">
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateReferenceParentPage(parameter) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(parameter);
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
</asp:Content>

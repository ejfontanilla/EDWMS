<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="EditSampleDoc.aspx.cs" Inherits="Maintenance_DocumentType_EditSampleDoc" Title="DWMS - Edit Sample Documents" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="New Categorisation Rules" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable to save all/some of the rules. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The categorisation rule has been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Sample Documents</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Document ID</span>
                    </td>
                    <td>
                        <asp:Label ID="DocumentIdLabel" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Document Type</span>
                    </td>
                    <td>
                        <asp:Label ID="DocumentTypeLabel" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Sample Documents</span>
                    </td>
                    <td>
                        <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="150px" CssClass="area">                        
                            <asp:Repeater ID="DocumentRepeater" runat="server" 
                                OnItemCommand="Repeater_ItemCommand" 
                                onitemdatabound="DocumentRepeater_ItemDataBound">
                                <ItemTemplate>
                                    <asp:HyperLink ID="AttachmentHyperLink" runat="server" CssClass="list"
                                        NavigateUrl='<%# "~/Common/DownloadSampleDocument.aspx?id=" + Eval("Id") %>'>
                                        <%# Eval("FileName") %>
                                    </asp:HyperLink>
                                    <asp:LinkButton ID="DeleteLink" CausesValidation="false" runat="server" CommandName="Delete" CssClass="link-del"
                                        OnClientClick='<%# "if(confirm(\"Are you sure you want to delete this sample document?\") == false) return false;" %>'
                                        CommandArgument='<%# Eval("Id") %>'>
                                        Delete
                                    </asp:LinkButton>
                                    <br />
                                </ItemTemplate>
                            </asp:Repeater>
                        </asp:Panel>
                        <br />
                        <telerik:RadProgressManager ID="UploadRadProgressManager" runat="server" />
                        <telerik:RadAsyncUpload ID="DocumentRadAsyncUpload" runat="server" Skin="Default" 
                            Localization-Select="Browse" ControlObjectsVisibility="Default" Enabled="True" 
                            MultipleFileSelection="Automatic" UploadedFilesRendering="BelowFileInput" 
                            TemporaryFileExpiration="01:00:00" 
                            AllowedFileExtensions=".PDF,.pdf,.JPG,.jpg,.JPEG,.jpeg,.PNG,.png,.TIF,.tif,.TIFF,.tiff">
                        </telerik:RadAsyncUpload>
                        <span class="form-note">Multiple files can be selected at once.</span>
                        <telerik:RadProgressArea ID="UploadRadProgressArea" runat="server">
                        </telerik:RadProgressArea>
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Acquire New Sample Documents</span>
                    </td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblAcquire" RepeatDirection="Horizontal" Width="100">
                        <asp:ListItem Text="Yes"></asp:ListItem>
                        <asp:ListItem Text="No"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Is Active</span>
                    </td>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblIsActive" RepeatDirection="Horizontal" Width="100">
                        <asp:ListItem Text="Yes"></asp:ListItem>
                        <asp:ListItem Text="No"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <%--<a href="javascript:history.back(1);">Cancel</a> --%>      
            <asp:LinkButton ID="CancelLinkButton" runat="server" OnClientClick="javascript:history.back(1);">Cancel</asp:LinkButton>
        </asp:Panel>
    </asp:Panel>
    <asp:ScriptManagerProxy ID="ScriptManager1" runat="server">
<%--        <Services>
            <asp:ServiceReference Path="~/Import/Controls/MultiThreadOcr.asmx" />
        </Services>--%>
    </asp:ScriptManagerProxy>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">            
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

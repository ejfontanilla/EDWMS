<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="Maintenance_DocumentType_Edit" Title="DWMS - Document Types" %>

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
                        <span class="label">Sample Documents</span>
                    </td>
                    <td>
                        <asp:Repeater ID="DocumentRepeater" runat="server" OnItemCommand="Repeater_ItemCommand">
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
                        <br />
                        <telerik:RadProgressManager ID="UploadRadProgressManager" runat="server" />
                        <telerik:RadAsyncUpload ID="DocumentRadAsyncUpload" runat="server" Skin="Default" 
                            Localization-Select="Browse" ControlObjectsVisibility="Default" Enabled="True" 
                            MultipleFileSelection="Automatic" UploadedFilesRendering="BelowFileInput" 
                            TemporaryFileExpiration="01:00:00" AllowedFileExtensions=".PDF,.pdf">
                        </telerik:RadAsyncUpload>
                        <span class="form-note">Multiple files can be selected at once.</span>
                        <telerik:RadProgressArea ID="UploadRadProgressArea" runat="server">
                        </telerik:RadProgressArea>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <asp:Panel ID="SampleDocsSubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="EditSampleDocsButton" runat="server" 
                Text="Edit Sample Documents" CssClass="button-large right20" 
                onclick="EditSampleDocsButton_Click" />        
        </asp:Panel>
        <div class="header">
            <div class="left">
                Categorisation Rules Details</div>
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
                    <td valign="top">
                        <span class="label">Document Type </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <asp:DropDownList ID="DocTypeDropDownList" runat="server" CssClass="form-field" 
                            AutoPostBack="false" Width="300px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Conditions <span class="form-error">*</span></span>
                    </td>
                    <td valign="top">
                        <span>Apply this rule when the document contains the following words/variables:</span>
                        <div class="bottom5">
                        </div>
                        <telerik:RadListBox ID="ContainsRadListBox" runat="server" AllowDelete="True" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="True">
                        </telerik:RadListBox><br /><br />
                        <telerik:RadButton ID="ContainsAddRadButton" runat="server" Skin="Windows7" Text="Add" 
                            Width="50px" AutoPostBack="false">
                        </telerik:RadButton>
                        <asp:CustomValidator ID="ContainsListCustomValidator" runat="server" 
                            ErrorMessage="<br />Please add at least 1 keyword." CssClass="form-error" 
                            Display="Dynamic" onservervalidate="ContainsListCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td width="25%" valign="top">
                        <%--   --%>
                    </td>
                    <td valign="top">
                        <span>Except if the following words/variables are found: (optional)</span>
                        <div class="bottom5">
                        </div>
                        <telerik:RadListBox ID="NotContainsRadListBox" runat="server" AllowDelete="True" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="True">
                        </telerik:RadListBox><br /><br />
                        <telerik:RadButton ID="NotContainsAddRadButton" runat="server" Skin="Windows7" Text="Add" 
                            Width="50px" AutoPostBack="false">
                        </telerik:RadButton>
                    </td>
                </tr>
                <tr>
                    <td width="25%">
                        <span class="label">Action <span class="form-error">*</span></span>
                    </td>
                    <td>  
                        <asp:CheckBox ID="ProcessingCheckBox" runat="server" Text="Stop executing other rules once this rule is executed" />                     
                    </td>
                </tr>
            </table>            
        </div>        
        <asp:Panel ID="MetaPanel" runat="server" Visible="true">
            <div class="header">
                <div class="left">
                    Metadata
                </div>
                <div class="right">
                    <!---->
                </div>
            </div>
            <div class="area">
                <table class="table-blue" width="100%">                    
                    <tr class="bg-blue">
                        <td rowspan="2">
                            Meta Field Name
                        </td>
                        <td colspan="2" style="text-align: center;">
                            Mandatory
                        </td>
                        <td colspan="2" style="text-align: center;">
                            Visible
                        </td>
                        <td rowspan="2" style="text-align: center;">
                            Maximum Length
                            <br />
                            <asp:Label runat="server" ID="MaxNoteLabel" Font-Size="X-Small" Font-Bold="false" Text="(Enter zero for Max)" />
                        </td>
                        <td rowspan="2" style="text-align: center;">
                            Action
                        </td>
                    </tr>
                    <tr class="bg-blue">
                        <%--<td>
                        </td>--%>
                        <td style="width: 15%; text-align: center;">
                            Verification
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Completeness
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Verification
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Completeness
                        </td>
                        <%--<td style="width: 15%; text-align: center;">
                            Action
                        </td>--%>
                    </tr>
                    <asp:Repeater ID="MetaFieldRepeater" runat="server" 
                        onitemdatabound="MetaFieldRepeater_ItemDataBound" OnItemCommand="MetaFieldRepeate_ItemCommand" >
                        <ItemTemplate>
                            <tr>                            
                                <td>
                                    <asp:HiddenField runat="server" ID="IdHidden" />
                                    <asp:HiddenField runat="server" ID="DocTypeCodeHidden" />
                                    <asp:Label runat="server" ID="FieldNameLabel"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryVerCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryComManCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleVerCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleComCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox runat="server" ID="MaximumLengthTextBox" MaxLength="4" Columns="1"/>
                                    <asp:RequiredFieldValidator ID="MaximumLengthRequiredFieldValidator" runat="server" ControlToValidate="MaximumLengthTextBox"
                                        Display="Dynamic" ErrorMessage="<br />Maximum Length is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="MaximumLengthCompareValidator" runat="server" ControlToValidate="MaximumLengthTextBox" Type="Integer" 
                                        Operator="DataTypeCheck" ErrorMessage="Please Enter an integer." Display="Dynamic" ForeColor="" CssClass="form-error"/> 
                                </td>
                                <td style="text-align: center;">
                                    <asp:HyperLink ID="EditHyperLink" runat="server" CssClass="form-field hand" 
                                        NavigateUrl="#">Edit</asp:HyperLink>
                                    <asp:Label ID="SeperatorLabel" runat="server">&nbsp;&nbsp;|&nbsp;</asp:Label>                                    
                                    <asp:LinkButton ID="DeleteLink" CausesValidation="false" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick='<%# "if(confirm(\"Deleting meta field will also clear the associated meta data.\\nAre you sure you want to delete this meta field and associated meta data?\") == false) return false;" %>'>
                                        Delete
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td>
                            <asp:Button ID="AddRowButton" runat="server" Text="Add Meta Field" CssClass="button-large right20" Width="120px"
                                CausesValidation="false"/>                                                               
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:history.back(1);">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SubmitPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="MetaPanel" />
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
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(condition, isContains) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(condition, isContains);
            }

            function UpdateParentPageMeta() {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

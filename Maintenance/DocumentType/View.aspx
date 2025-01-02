<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="View.aspx.cs" Inherits="Maintenance_DocumentType_View" Title="DWMS - Document Types" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Categorisation Rules" />
    </div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
         We were unable to save some/all of the changes. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The categorisation rule has been saved.
    </asp:Panel>
    <asp:Panel ID="SampleDocsConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The sample document(s) have been saved.
    </asp:Panel>
    <asp:Panel ID="MetaFieldConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The metadata field(s) have been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Sample Documents</div>
            <div class="right">                
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Sample Documents</span>
                    </td>
                    <td>
                        <asp:Panel ID="DocumentsPanel" runat="server" ScrollBars="Vertical" Height="150px" CssClass="area">
                            <asp:Label ID="NoSampleDocumentLabel" runat="server" Text="No sample documents uploaded" Visible="false"></asp:Label>
                            <asp:Repeater ID="DocumentRepeater" runat="server" OnItemCommand="Repeater_ItemCommand">
                                <ItemTemplate>
                                    <asp:HyperLink ID="AttachmentHyperLink" runat="server" CssClass="list"
                                        NavigateUrl='<%# "~/Common/DownloadSampleDocument.aspx?id=" + Eval("Id") %>'>
                                        <%# Eval("FileName") %>
                                    </asp:HyperLink>
                                    <asp:LinkButton ID="DeleteLink" CausesValidation="false" runat="server" CommandName="Delete" CssClass="link-del"
                                        OnClientClick='<%# "if(confirm(\"Are you sure you want to delete this sample document?\") == false) return false;" %>'
                                        CommandArgument='<%# Eval("Id") %>' Visible="false">
                                        Delete
                                    </asp:LinkButton>
                                    <br />
                                </ItemTemplate>
                            </asp:Repeater>
                            </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Acquire New Sample Documents</span>
                    </td>
                    <td>
                        <asp:Label ID="lblAcquire" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Is Active</span>
                    </td>
                    <td>
                        <asp:Label ID="lblIsActive" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <asp:Panel ID="SampleDocsSubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="EditSampleDocsButton" runat="server" 
                Text="Edit Sample Documents" CssClass="button-large right20" 
                onclick="EditSampleDocsButton_Click" Width="180" />        
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
                <tr id="DocIdTr" runat="server" visible="false">
                    <td valign="top" width="25%">
                        <span class="label">Document ID</span>
                    </td>
                    <td>
                        <asp:Label ID="DocumentIdLabel" runat="server" />
                    </td>
                </tr>
                <tr id="DocTypeTr" runat="server" visible="false">
                    <td valign="top">
                        <span class="label">Document Type </span>
                    </td>
                    <td>
                        <asp:Label ID="DocumentTypeLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Contains</span>
                    </td>
                    <td>
                        <span>Apply this rule when the document contains the following words/variables. Partial
                            match applies, i.e., it does not require matching the whole word.</span>
                        <div class="bottom5">
                        </div>
                        <telerik:RadListBox ID="ContainsRadListBox" runat="server" AllowDelete="False" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="False" Enabled="false">
                        </telerik:RadListBox>
                    </td>
                </tr>
                <tr>
                    <td width="25%" valign="top">
                        <span class="label">Not-Contains</span>
                    </td>
                    <td valign="top">
                        <span>Except if the following words/variables are found. Partial match applies, i.e.,
                            it does not require matching the whole word.</span>
                        <div class="bottom5">
                        </div>
                        <telerik:RadListBox ID="NotContainsRadListBox" runat="server" AllowDelete="False" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="False" Enabled="false">
                        </telerik:RadListBox>
                    </td>
                </tr>
                <tr id="ActionTr" runat="server" visible="false">
                    <td width="25%">
                        <span class="label">Action </span>
                    </td>
                    <td>
                        <asp:Label ID="ActionLabel" runat="server" Text="N.A."></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <asp:Panel ID="KeywordSubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="EditKeywordButton" runat="server" Text="Edit Keywords" 
                CssClass="button-large right20" onclick="EditKeywordButton_Click" Width="180"  />        
        </asp:Panel>
        <asp:Panel ID="MetaPanel" runat="server" Visible="true">
            <div class="header">
                <div class="left">
                    Metadata Fields
                </div>
                <div class="right">
                    <!---->
                </div>
            </div>
            <div class="area">
                <table class="table-blue" width="100%">                    
                    <tr class="bg-blue">
                        <td rowspan="2">
                            Metadata Field Name
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
                    </tr>
                    <tr class="bg-blue">
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
                    </tr>
                    <asp:Repeater ID="MetaFieldRepeater" runat="server" 
                        onitemdatabound="MetaFieldRepeater_ItemDataBound">
                        <ItemTemplate>
                            <tr>                            
                                <td>
                                    <asp:HiddenField runat="server" ID="IdHidden" />
                                    <asp:HiddenField runat="server" ID="DocTypeCodeHidden" />
                                    <asp:Label runat="server" ID="FieldNameLabel"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryVerCheckBox" runat="server" Enabled="false" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryComManCheckBox" runat="server" Enabled="false" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleVerCheckBox" runat="server" Enabled="false" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleComCheckBox" runat="server"  Enabled="false"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox runat="server" ID="MaximumLengthTextBox" MaxLength="4" Columns="1" Enabled="false"/>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td colspan="6">
                                    <asp:Label ID="lblEmpty" Text="No metadata available for this document type." runat="server" Visible='<%#bool.Parse((MetaFieldRepeater.Items.Count==0).ToString())%>'></asp:Label>
                                </td>
                            </tr>
                        </FooterTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </asp:Panel>
        <br />
        <asp:Panel ID="MetaSubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="EditMetaButton" runat="server" 
                Text="Edit Fields" CssClass="button-large right20" 
                onclick="EditMetaButton_Click" Width="180"  />        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SubmitPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="WarningPanel" />
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
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

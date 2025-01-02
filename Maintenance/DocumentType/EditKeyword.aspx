<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="EditKeyword.aspx.cs" Inherits="Maintenance_DocumentType_EditKeyword" Title="DWMS - Edit Keywords" %>

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
                        <span class="label">Document Type </span>
                    </td>
                    <td>
                        <asp:Label ID="DocumentTypeLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Conditions <span class="form-error" style="display:none;">*</span></span>
                    </td>
                    <td valign="top">
                        <span>Apply this rule when the document contains the following words/variables (Contains):</span>
                        <div class="bottom5">
                        </div>
                        <telerik:RadListBox ID="ContainsRadListBox" runat="server" AllowDelete="True" 
                            Height="200px" Width="350px" ButtonSettings-AreaWidth="50px" 
                            Skin="Windows7" ButtonSettings-Position="Right" AllowReorder="True">
                        </telerik:RadListBox><br /><br />
                        <telerik:RadButton ID="ContainsAddRadButton" runat="server" Skin="Windows7" Text="Add" 
                            Width="50px" AutoPostBack="false">
                        </telerik:RadButton>
                        <asp:CustomValidator ID="ContainsListCustomValidator" runat="server" Enabled="false" 
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
                        <span>Except if the following words/variables are found (Not contains): (optional)</span>
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
                <tr id="ActionTr" runat="server" visible="false">
                    <td width="25%">
                        <span class="label">Action <span class="form-error">*</span></span>
                    </td>
                    <td>  
                        <asp:CheckBox ID="ProcessingCheckBox" runat="server" Text="Stop executing other rules once this rule is executed" />                     
                    </td>
                </tr>
            </table>            
        </div>        
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" 
                CssClass="button-large right20" onclick="SubmitButton_Click" />
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
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

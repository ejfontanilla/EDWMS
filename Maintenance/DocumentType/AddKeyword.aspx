<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="AddKeyword.aspx.cs" Inherits="Maintenance_DocumentType_AddKeyword" Title="DWMS - Document Types" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Add Keywords" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Add Keywords</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <asp:Repeater ID="KeywordRepeater" runat="server" 
                    onitemcommand="KeywordRepeater_ItemCommand" 
                    onitemdatabound="KeywordRepeater_ItemDataBound">
                    <ItemTemplate>
                        <tr id="Tr1" runat="server">
                            <td valign="top" style="width: 20%;">
                                <span class="label"><%# "Keyword " + (Container.ItemIndex + 1) %></span><span class="form-error">&nbsp;*</span>
                            </td>
                            <td valign="middle">
                                <telerik:RadComboBox ID="KeywordRadComboBox" runat="server" AllowCustomText="True" Skin="Windows7" 
                                    ShowToggleImage="False">
                                    <HeaderTemplate>
                                        <table cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    Variables
                                                </td>
                                            </tr>
                                        </table>
                                    </HeaderTemplate>
                                </telerik:RadComboBox><asp:Label ID="AndLiteralLabel" runat="server">&nbsp;&nbsp;and&nbsp;&nbsp;</asp:Label>
                                <asp:ImageButton ID="DeleteImageButton" runat="server" ImageUrl="~/Data/Images/Icons/Cancel.gif" 
                                    CommandArgument='<%# Container.ItemIndex %>' CommandName="Delete" CausesValidation="false" />
                                <asp:RequiredFieldValidator ID="ComboBoxRequiredFieldValidator" runat="server" CssClass="form-error" 
                                    ErrorMessage="<br />Please enter a keyword or choose a variable." ControlToValidate="KeywordRadComboBox">
                                </asp:RequiredFieldValidator>                                
                                <asp:TextBox ID="KeywordTextBox" runat="server" Text='<%# Eval("Keyword") %>' Visible="false"></asp:TextBox>
                                <%--<asp:ImageButton ID="DeleteImageButton" runat="server" ImageUrl="~/Data/Images/Icons/Cancel.gif" 
                                    CommandArgument='<%# Container.ItemIndex %>' CommandName="Delete" CausesValidation="false" />--%>
                                <asp:RequiredFieldValidator ID="KeywordRequiredFieldValidator" runat="server" 
                                    ErrorMessage="<br />Please enter a keyword." Display="Dynamic" 
                                    CssClass="form-error" ControlToValidate="KeywordTextBox" Enabled="false">                                
                                </asp:RequiredFieldValidator>                        
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td></td>       
                    <td>
                        <asp:Button ID="MoreButton" runat="server" Text="More" CausesValidation="false" 
                            CssClass="button-large right20" onclick="MoreButton_Click" />                    
                    </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Add" CssClass="button-large right20"
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
            <telerik:AjaxSetting AjaxControlID="MoreButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="KeywordRepeater" LoadingPanelID="LoadingPanel1" />
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

            function CloseWindow(condition, isContains) {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(condition, isContains);
            }

            function HideTooltip() {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();
            }

            function BuildAddress() {
                var tooltip = $find("RadToolTip2");
                //API: hide the tooltip
                tooltip.hide();
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

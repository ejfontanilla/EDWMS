<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="Assign.aspx.cs" Inherits="Verification_AssignApp" Title="DWMS - Assign Application" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Assign Application" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The Reference Number(s) have been assigned to the user.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="AssignButton">
        <div class="header">
            <div class="left">
                Assign Application</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td valign="top" style="width: 30%;">
                        <span class="label">Reference Numbers to Assign</span>                        
                    </td>
                    <td>       
                        <asp:Label ID="NoOfAppsLabel" runat="server"></asp:Label><br /><br />            
                        <asp:Repeater ID="AppRepeater" runat="server">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" CssClass="list"><%# Eval("RefNo") %></asp:Label>
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr>                    
                    <td valign="top">
                        <span class="label">Completeness OIC</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true"
                                AutoPostBack="true" CausesValidation="false" 
                                EnableAutomaticLoadOnDemand="true" datatextfield="Name" MarkFirstMatch="True" datavaluefield="UserId" Filter="Contains"  
                                EmptyMessage="Type a Completeness OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
                        </telerik:RadComboBox>
                        <asp:CustomValidator ID="UserCustomValidator" runat="server" Display="Dynamic" CssClass="form-error" 
                            ErrorMessage="<br />Please select a completeness officer." 
                            onservervalidate="UserCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="AssignButton" runat="server" Text="Assign" 
                CssClass="button-large right20" onclick="AssignButton_Click" />
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

            function CloseWindow(){
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }
        </script>
    </telerik:RadCodeBlock>
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server"
        SelectMethod="GetUserBySectionForCompleteness" OnSelecting="GetUserObjectDataSource_Selecting"
        TypeName="Dwms.Bll.userDb">
    </asp:ObjectDataSource>
</asp:Content>

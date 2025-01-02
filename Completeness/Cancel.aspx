<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="Cancel.aspx.cs" Inherits="Verification_AssignApp" Title="DWMS - Cancel Application" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Cancel Application" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The Application has been cancelled.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="AssignButton">
        <div class="header">
            <div class="left">
                Cancel Application</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table cellpadding="0" cellspacing="0" style="margin:0px;">
                <tr>
                    <td valign="top" class="label">
                        <span class="label">The Application can be flagged for cancellation for the following reasons:</span>                        
                    </td>
                </tr>
                <tr>
                    <td class="left">
                        <asp:RadioButtonList runat="server" ID="CancellationReasonList" CssClass="RadComboBox" >
                            <asp:ListItem Text="At the customer’s request" Value="CustomerRequest"></asp:ListItem>
                            <asp:ListItem Text="Overdue (Partial / Non- response from customer<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;pertaining to ABCDE’s request(s) for documents.)" Value="Overdue"></asp:ListItem>
                            <asp:ListItem Text="Others" Value="Others" Selected></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:TextBox runat="server" ID="CancellationRemark" TextMode="MultiLine" Columns="40" Rows="5"></asp:TextBox>
                    </td>
                </tr>

            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="AssignButton" runat="server" Text="Confirm" 
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
                GetRadWindow().BrowserWindow.location.reload();
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

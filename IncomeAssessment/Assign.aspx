<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true" CodeFile="Assign.aspx.cs"
    Inherits="IncomeAssessment_AssignApp" Title="DWMS - Assign Application" %>

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
                    <td>
                        <asp:Table runat="server" ID="tblAllow">
                            <asp:TableRow>
                                <asp:TableCell><span class="label">Reference Numbers to Assign</span></asp:TableCell>
                                <asp:TableCell>
                                    <asp:Label ID="NoOfAppsLabel" runat="server"></asp:Label><br />
                                    <br />
                                    <asp:Repeater ID="AppRepeater" runat="server">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" CssClass="list"><%# Eval("RefNo") %></asp:Label>
                                            <br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                    <td>
                        <asp:Table runat="server" ID="tblNotAllow" Visible="false">
                            <asp:TableRow>
                                <asp:TableCell>
                                    <asp:Label runat="server" ID="NotAllowedeLabel" Text="Reference Numbers Not Allowed to Assign"
                                        Visible="false" CssClass="label"></asp:Label></asp:TableCell>
                                <asp:TableCell>
                                    <asp:Label ID="NotNoOfAppsLabel" runat="server"></asp:Label><br />
                                    <br />
                                    <asp:Repeater ID="NotAppRepeater" runat="server">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" CssClass="list"><%# Eval("RefNo") %></asp:Label>
                                            <br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
                </table>
                <table>
                <tr>
                    <td valign="top">
                        <span class="label">Income Assessment OIC</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true" AutoPostBack="true"
                            CausesValidation="false" EnableAutomaticLoadOnDemand="true" DataTextField="Name"
                            MarkFirstMatch="True" DataValueField="UserId" Filter="Contains" EmptyMessage="Type a Income Assessment OIC..."
                            Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
                        </telerik:RadComboBox>
                        <asp:CustomValidator ID="UserCustomValidator" runat="server" Display="Dynamic" CssClass="form-error"
                            ErrorMessage="<br />Please select a completeness officer." OnServerValidate="UserCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="AssignButton" runat="server" Text="Assign" CssClass="button-large right20"
                OnClick="AssignButton_Click" />
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

            function CloseWindow() {
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
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server" SelectMethod="GetUserBySectionForAssessment"
        OnSelecting="GetUserObjectDataSource_Selecting" TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="IndicateUrgency.aspx.cs" Inherits="Verification_IndicateUrgency" Title="DWMS - Indicate Urgency" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Indicate Urgency" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        Urgency details saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="UpdateButton">
        <div class="header">
            <div class="left">
                Indicate Urgency</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table width="100%">
                <tr>
                    <td valign="top" style="width: 30%;">
                        <span class="label">Sets to set urgency</span>                        
                    </td>
                    <td>       
                        <asp:Label ID="NoOfSetsLabel" runat="server"></asp:Label><br /><br />            
                        <asp:Repeater ID="SetRepeater" runat="server">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" CssClass="list"><%# Eval("SetNo") %></asp:Label>
                                <br />
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
<%--                <tr>
                    <td width="30%" valign="top">
                        <span class="label">Skip Categorization <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="SkipCategorizationRadioButtonList" runat="server" CssClass="list-item hand" AutoPostBack="true"
                            RepeatDirection="Horizontal" Width="100px" OnSelectedIndexChanged="SkipCategorizationRadioButtonList_OnSelectedIndexChanged">
                            <asp:ListItem Value="1">Yes</asp:ListItem>
                            <asp:ListItem Value="0" Selected>No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
--%>                <asp:Panel runat="server" ID="IsUrgentPanel">
                <tr>
                    <td width="30%" valign="top">
                        <span class="label">Is Urgent <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="UrgencyRadioButtonList" runat="server" CssClass="list-item hand"
                            RepeatDirection="Horizontal" Width="100px">
                            <asp:ListItem Value="1" Selected>Yes</asp:ListItem>
                            <asp:ListItem Value="0">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                </asp:Panel>
                <tr id="DocConditionTr" runat="server" visible="true">
                    <td width="30%" valign="top">
                        <span class="label">Expedite Reason <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ExpediteReasonDropDownList" runat="server" CssClass="list-item hand"
                            RepeatDirection="Horizontal" Width="220px"/>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Expedite Remark</span>
                    </td>
                    <td>
                        <asp:TextBox ID="ExpediteRemarkTextBox" runat="server" Rows="5" Width="300px" TextMode="MultiLine"
                            CssClass="form-field" Columns="5"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="UpdateButton" runat="server" Text="Update" CssClass="button-large right20"
                OnClick="UpdateButton_Click" />
            <a href="javascript:GetRadWindow().close();">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>            
            <telerik:AjaxSetting AjaxControlID="SkipCategorizationRadioButtonList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="IsUrgentPanel" LoadingPanelID="LoadingPanel1" />
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

            function ResizeAndClose(width, height, parameter) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(parameter);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

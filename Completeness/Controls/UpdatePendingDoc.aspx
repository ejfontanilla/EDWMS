<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="UpdatePendingDoc.aspx.cs" Inherits="Verification_AssignApp" Title="DWMS - Completeness" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Indicate Acceptance" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        Acceptance has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="UpdateButton">
        <div class="header">
            <div class="left">
                Indicate Acceptance</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table width="100%">
                <tr>
                    <td width="30%" valign="top">
                        <span class="label">Acceptance <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="AcceptanceRadioButtonList" runat="server" CssClass="list-item hand" 
                            RepeatDirection="Horizontal" Width="220px">
                            <asp:ListItem Value="Y">Yes</asp:ListItem>
                            <asp:ListItem Value="N">No</asp:ListItem>
                            <asp:ListItem Value="X">NA</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr id="DocConditionTr" runat="server" visible="true">
                    <td width="30%" valign="top">
                        <span class="label">Document Condition <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="DocConditionRadioButtonList" runat="server" CssClass="list-item hand"
                            RepeatDirection="Horizontal" Width="220px">
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Remark</span>
                    </td>
                    <td>
                        <asp:TextBox ID="ExceptionTextBox" runat="server" Rows="5" Width="300px" TextMode="MultiLine"
                            CssClass="form-field" Columns="5"></asp:TextBox>
                        <asp:CustomValidator ID="CustomValidator" runat="server" Display="Dynamic" CssClass="form-error" Enabled="true"
                            ErrorMessage="<br />Please enter a remark with less than 255 characters." OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
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

            function ResizeAndClose(width, height, parameter) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(parameter);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

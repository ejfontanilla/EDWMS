<%@ Page Title="DWMS - Email Documents (Verification)" Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="Email.aspx.cs" Inherits="Verification_Email" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/EmailTextBox.ascx" TagName="EmailTextBox" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server" >
<div class="title">
<asp:Label ID="TitleLabel" runat="server" Text="Email Documents" />
</div>
<asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
    Visible="false" EnableViewState="false">
        <asp:label runat="server" ID="ConfirmLabel" Font-Bold="false"></asp:label>
</asp:Panel>
<asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SendButton">
<table style="width:100%;" cellspacing="2">
    <asp:Panel runat="server" ID="DisplayPanel" Visible="true">
    <tr>
        <td colspan="2">
            <telerik:RadTreeView ID="RadTreeView1" runat="server" Skin="Windows7" AccessKey="T" SingleExpandPath="False" Width="100%" 
                CausesValidation="false" TriStateCheckBoxes="True" CheckBoxes="true" BorderWidth="1">
            </telerik:RadTreeView>
        </td>
    </tr>
    <tr>
        <td><span runat="server" id="Span1" class="label">Subject</span></td>
        <td valign="bottom">
            <asp:TextBox runat="server" ID="SubjectTextBox" Width="300"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td><span runat="server" id="RecipentLabel" class="label">Recipent</span></td>
        <td valign="bottom">
        <uc:EmailTextBox ID="EmailRecipentsTextBox" runat="server" Width="300"/>
        </td>
    </tr>
    <tr>
        <td valign="top"><span runat="server" class="label" id="RemarkLabel">Remarks</span></td>
        <td><asp:TextBox runat="server" ID="RemarksTextBox" TextMode="MultiLine" Columns="40" Rows="9"></asp:TextBox>
            <br />
            <asp:CustomValidator runat="server" ID="RecipentCustomValidator" ControlToValidate="RemarksTextBox"></asp:CustomValidator>
        </td>
    </tr>
    <tr>
        <td>
        </td>
        <td>
            <asp:Button runat="server" ID="SendButton" CssClass="button-large right20" Text="Send" OnClick="SendButton_onClick"/>&nbsp;
            <asp:Button runat="server" ID="CancelButton" CssClass="button-large right20" OnClientClick="javascript:GetRadWindow().close();" Text="Cancel" />
        </td>
    </tr>
    </asp:Panel>
    <asp:Panel runat="server" ID="OKPanel" Visible="false">
    <tr>
        <td colspan="2">
            <asp:Button runat="server" ID="OkButton" CssClass="button-large right20" Text="Ok" OnClick="OkButton_onClick" CausesValidation="false"/>&nbsp;
        </td>
    </tr>
    </asp:Panel>
</table>
<asp:HiddenField runat="server" ID="urlTargetHiddenField" />
</asp:Panel>
<telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="RadTreeView1">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="SendButton">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                <telerik:AjaxUpdatedControl ControlID="ConfirmPanel"/>
                <telerik:AjaxUpdatedControl ControlID="newSetIDHiddenField"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
</telerik:RadAjaxLoadingPanel>
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

            function ResizeAndClose(width, height, url) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.RefreshUrl(url);
            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

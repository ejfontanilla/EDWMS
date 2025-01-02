<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="EditSection.aspx.cs" Inherits="Edit_Section" Title="DWMS - Section Information" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:Label ID="TitleLabel" runat="server" CssClass="title" Text="New Section"></asp:Label>
    <div class="bottom10">
        <!---->
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10" Visible="false"
        EnableViewState="false">
        The Section has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SaveButton">
        <div class="header">
            <div class="left">
                Section Information</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        <b>Note:</b> Asterisk (<span class="form-error">*</span>) is mandatory field.<br /><br />
            <table>
                <tr>
                    <td>
                        <span class="label">Department</span>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="DepartmentLabel"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="label">Section Code <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="CodeTextBox" Columns="30" MaxLength="10" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CodeRequiredFieldValidator" runat="server" ControlToValidate="CodeTextBox"
                            Display="Dynamic" ErrorMessage="<br />Section Code is required." ForeColor=""
                            CssClass="form-error"></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CodeCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage="<br/>Section Code is a duplicate. Please verify." ForeColor="" CssClass="form-error"
                            OnServerValidate="DuplicateCodeCustomValidator_ServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td width="25%" nowrap="nowrap">
                        <span class="label">Section Name <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="NameTextBox" Columns="30" MaxLength="200" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NameRequiredFieldValidator" runat="server" ControlToValidate="NameTextBox"
                            Display="Dynamic" ErrorMessage="<br />Section Name is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap">
                        <span class="label">Business Code <span class="form-error">*</span></span>
                    </td>
                    <td>
                        <asp:TextBox ID="BusinessCodeTextBox" Columns="30" MaxLength="10" runat="server" CssClass="form-field" Enabled="false"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="BusinessCodeRequiredFieldValidator" runat="server" ControlToValidate="BusinessCodeTextBox"
                            Display="Dynamic" ErrorMessage="<br />Business Code is required." ForeColor=""
                            CssClass="form-error"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
             
        </div>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <center>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="200">
            <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:GetRadWindow().close();">Cancel</a>
        </asp:Panel>
    </center>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SaveButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
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

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }     
        
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

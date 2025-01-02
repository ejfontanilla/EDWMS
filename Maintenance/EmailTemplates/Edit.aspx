<%@ Page Language="C#" ValidateRequest="false" MasterPageFile="~/Maintenance/Main.master"
    AutoEventWireup="true" CodeFile="Edit.aspx.cs" Inherits="Hq_Administration_EmailTemplates_Edit" Title="DWMS - Email Templates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" />
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Email Template Information
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">Template Description </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="DescriptionTextBox" runat="server" MaxLength="200" CssClass="form-field" Columns="70" Enabled="false"></asp:TextBox>                        
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Subject </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="Subject" runat="server" CssClass="form-field" MaxLength="500" Columns="70">
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Subject"
                            ErrorMessage="<br />Subject is required." ForeColor="" CssClass="form-error"
                            Display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Content </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="ContentTextBox" runat="server" CssClass="form-field" Width="100%" Rows="20"
                            TextMode="MultiLine" >
                        </asp:TextBox>
                        <asp:CustomValidator ID="ContentCustomValidator" runat="server" CssClass="form-error" Display="Dynamic" 
                            ErrorMessage="<br />Content is required." OnServerValidate="ContentCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>
             
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%"> 
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="SubmitButton_Click" />
            <a href="javascript:history.back(1);">Cancel</a>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" ReloadOnShow="true" VisibleStatusbar="false" Modal="true">
    </telerik:RadWindow>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SubmitButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

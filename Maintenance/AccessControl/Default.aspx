<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_AccessControl_Default" Title="DWMS - Access Control" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <table width="100%">
        <tr>
            <td>
                <div class="title">
                    Access Control </div>
            </td>
        </tr>
    </table>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green bottom15" Visible="false"
        EnableViewState="false">
        The access control settings have been saved.
    </asp:Panel>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red bottom15" Visible="false"
        EnableViewState="false">
        Failed to save access control settings.  Please try again later.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Access Control</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <span class="label">Department</span>&nbsp
            <telerik:RadComboBox ID="DepartmentRadComboBox" runat="server" ZIndex="30000" Height="100"
                Width="300" DropDownWidth="300" AppendDataBoundItems="false" 
                ExpandAnimation-Duration="100" AutoPostBack="true" CausesValidation="false" 
                CollapseAnimation-Duration="200" OnSelectedIndexChanged="DepartmentRadComboBox_OnSelectedIndexChanged">
            </telerik:RadComboBox>
            <br /><br />
            <table class="table-blue">
                <asp:Repeater ID="RoleRepeater" runat="server" 
                    onitemdatabound="RoleRepeater_ItemDataBound">
                    <ItemTemplate>
                        <tr class="bg-blue">
                            <td rowspan="4">
                                <span class="label"><%# Eval("RoleName") %></span>
                                <asp:HiddenField ID="RoleIdHiddenField" runat="server" Value='<%# Eval("RoleId") %>' />
                            </td>
                            <td align="left">
                                <span class="label">Import</span>
                            </td>
                            <td align="left">
                                <span class="label">Search</span>
                            </td>
                            <td align="left">
                                <span class="label">Verification</span>
                            </td>
                            <td align="left">
                                <span class="label">Completeness</span>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:CheckBoxList ID="ImportRightsCheckBoxList" runat="server" RepeatDirection="Vertical"  
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true">
                                </asp:CheckBoxList>
                            </td>
                            <td align="left">
                                <asp:CheckBoxList ID="SearchRightsCheckBoxList" runat="server" RepeatDirection="Vertical"  
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="SearchRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                            <td align="left" valign="top">
                                <asp:CheckBoxList ID="VerificationRightsCheckBoxList" runat="server" RepeatDirection="Vertical" 
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="VerificationRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                            <td align="left">
                                <asp:CheckBoxList ID="CompletenessRightsCheckBoxList" runat="server" RepeatDirection="Vertical" 
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="CompletenessRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr class="bg-blue">
                            <td align="left">
                                <span class="label">Income Extraction</span>
                            </td>
                            <td align="left">
                                <span class="label">File Doc</span>
                            </td>
                            <td align="left">
                                <span class="label">Maintenance</span>
                            </td>
                            <td align="left">
                                <span class="label">&nbsp;</span>
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:CheckBoxList ID="IncCompRightsCheckBoxList" runat="server" RepeatDirection="Vertical" 
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="IncomComputationRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                            <td align="left">
                                <asp:CheckBoxList ID="FileDocRightsCheckBoxList" runat="server" RepeatDirection="Vertical" 
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="FileDocRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                            <td align="left">
                                <asp:CheckBoxList ID="MaintenanceRightsCheckBoxList" runat="server" RepeatDirection="Vertical" 
                                    RepeatLayout="Flow" CssClass="hand" AutoPostBack="true" OnSelectedIndexChanged="MaintenanceRightsCheckBoxList_OnSelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                            <td>&nbsp;</td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
        <asp:UpdatePanel ID="SubmitUpdatePanel" runat="server">
            <ContentTemplate>
                <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                    OnClick="Save" />
                <%--<a href="javascript:history.back(1);">Cancel</a>--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SubmitButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>

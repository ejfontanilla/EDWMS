<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_ArchiveDocuments_Default" 
MasterPageFile="~/Maintenance/Main.master" Title="DWMS - Archive Documents"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Archive Parameters</div>
        <span>This page is the settings for archiving documents and images from production server to archive server.</span><br />
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The parameter values have been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Parameter Values</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>                
                <tr>
                    <td valign="top">
                        <span class="label">COS<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlArchiveCOS" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value">
                        </asp:DropDownList>
                        &nbsp;Month (s)
                        <br />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">RESALE<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlArchiveResale" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value">
                        </asp:DropDownList>
                        &nbsp;Month (s)
                        <br />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">SALES<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlArchiveSales" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value">
                        </asp:DropDownList>
                        &nbsp;Month (s)
                        <br />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">SERS<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlArchiveSERS" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value">
                        </asp:DropDownList>
                        &nbsp;Month (s)
                        <br />
                    </td>
                </tr>

                <tr>
                    <td valign="top">
                        <span class="label">Archive Time Start<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:TextBox ID="ArchiveTimeStart" runat="server" CssClass="form-field"/>
                        <span class="form-note">Input time based on the format set in the Time Format Below. Eg: 13:30:05 is Military format
                        and will execute during 1:30:05 PM. </span>
                    </td>
                </tr>

                <tr>
                    <td valign="top">
                        <span class="label">Archive Time Format<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:TextBox ID="ArchiveTimeFormat" runat="server" CssClass="form-field"/>
                        <span class="form-note">Military Format (HH:mm:ss), Time with AM/PM (T) or with seconds (t)  </span>
                    </td>

                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Archive Server<span class="form-error"> &nbsp;*</span>
                        </span>
                    </td>
                    <td>
                        <asp:TextBox ID="ArchiveServer" runat="server" CssClass="form-field"/>
                        <span class="form-note">Shared path from the archive server </span>
                    </td>

                </tr>
            </table>
        </div>
        <div class="bottom10">
            <!---->
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <%--<a href="javascript:history.back(1);">Cancel</a>--%>
            <%--<input type="button" value="Cancel" class="button-large" onclick="javascript:history.back(1);" />--%>
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
            <telerik:AjaxSetting AjaxControlID="FilesDDL">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />                    
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>

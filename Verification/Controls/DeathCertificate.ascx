﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeathCertificate.ascx.cs" Inherits="Verification_Control_DeathCertificate" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="true" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Death must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Tag</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="TagRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="TagRadioButtonList_SelectedIndexChanged">
                <asp:ListItem Value="Local" Text="Local"></asp:ListItem>
                <asp:ListItem Value="Foreign" Text="Foreign" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:HiddenField runat="server" ID="TagRadioButtonListId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Date of Death</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfDeath" RequiredFieldErrorMessage="Date of Death is required." EnableRequiredField="true" Visible="true"/>
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeclarationPrivProp.ascx.cs" Inherits="Verification_Control_DeclarationPrivProp" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Declaration must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Declaration</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel2"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfDeclaration" RequiredFieldErrorMessage="Date of Declaration is required." Visible="true"/>
        </td>
    </tr>
</table>
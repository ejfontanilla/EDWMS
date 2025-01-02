<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DocEduInstitute.ascx.cs" Inherits="Verification_Controls_DocEduInstitute" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="From Date must be before To Date & Today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError2" Text="To Date should not be after this month" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">From Date </span><span class="form-error" runat="server" id="Span1" >&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="FromDate" RequiredFieldErrorMessage="From Date is required." Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">To Date </span><span class="form-error" runat="server" id="Span5" >&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel1"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="ToDate" RequiredFieldErrorMessage="To Date is required."/>
        </td>
    </tr>
</table>
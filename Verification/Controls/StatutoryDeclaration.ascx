<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StatutoryDeclaration.ascx.cs" Inherits="Verification_Control_StatutoryDeclaration" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="To Date must be later than From Date" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError2" Text="Date of Declaration must be before today" Visible="false" CssClass="form-error"></asp:Label>
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
            <uc:DateTextBox runat="server" ID="ToDate" RequiredFieldErrorMessage="To Date is required." Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Date of Declaration</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel2"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfDeclaration" RequiredFieldErrorMessage="Date of declaration is required." Visible="true"/>
        </td>
    </tr>
<%--    <tr runat="server" id="TypeTR">
        <td>
            <span class="label">Type</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <telerik:RadComboBox runat="server" ID="Type" />
            <asp:HiddenField runat="server" ID="TypeId" />
        </td>
    </tr>
--%></table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PropertyTaxNRIC.ascx.cs" Inherits="Verification_Controls_PropertyTaxNRIC" %>

<!--Added by Edward 2017/10/03 New Document Types Property Tax -->

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true"
    EnableRequiredField="false" />
<div class="grey-header">
    <%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Year of Property tax is invalid"
                Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Year of Property Tax </span><span class="form-error" runat="server" id="IdentityNoSpan">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="YearOfPropertyTax"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorYearOfPropertyTax" runat="server" CssClass="form-error" ControlToValidate="YearOfPropertyTax" 
                ErrorMessage="<br />Year of property tax is required." Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="YearOfPropertyTaxId" />
            <asp:HiddenField runat="server" ID="ToDateId" />
            <asp:HiddenField runat="server" ID="FromDateId" />
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNo" runat="server"
                CssClass="form-error" ControlToValidate="IdentityNo" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidator" runat="server" ControlToValidate="IdentityNo"
                Display="Dynamic" OnServerValidate="NricCustomValidator" ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDType" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name of NRIC</span><span class="form-error" runat="server" id="Span3">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfNRIC"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfNRIC" runat="server"
                CssClass="form-error" ControlToValidate="NameOfNRIC" ErrorMessage="<br />Name of NRIC is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfNRICId" />
        </td>
    </tr>
</table>

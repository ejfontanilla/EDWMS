<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PropertyTax.ascx.cs" Inherits="Verification_Controls_PropertyTax" %>

<!--Added by Edward 2017/10/03 New Document Types Property Tax -->

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Year of Property tax is invalid" Visible="false" CssClass="form-error"></asp:Label>
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
    
</table>
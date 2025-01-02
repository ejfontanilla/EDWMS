<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BusinessProfile.ascx.cs" Inherits="Verification_Control_BusinessProfile" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Registration must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Registration/ Incorporation</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfRegistration" Visible="true" EnableRequiredField="true" RequiredFieldErrorMessage="Date of Registration/ Incorporation is required."/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">UEN No.</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="UENNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorUENNo" runat="server" CssClass="form-error" ControlToValidate="UENNo" 
                ErrorMessage="<br />UENNo is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="UENNoId" />
        </td>
    </tr>
<%--    <tr runat="server" id="BusinessTypeTR">
        <td>
            <span class="label">Business Type</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <telerik:RadComboBox runat="server" ID="BusinessType" AutoPostBack="true" CausesValidation="false" />
            <asp:HiddenField runat="server" ID="BusinessTypeId" />
        </td>
    </tr>
--%></table>
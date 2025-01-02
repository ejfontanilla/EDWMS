<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmploymentLetter.ascx.cs" Inherits="Verification_Control_EmploymentLetter" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Document must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
<%--    <tr runat="server" id="AllceTR">
        <td class="left">
            <span class="label">Allce</span><span class="form-error" runat="server" id="Span7">&nbsp;*</span>
        </td>
        <td style="width:100px;">
            <asp:RadioButtonList runat="server" ID="AllceRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                <asp:ListItem Value="No" Text="No" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:HiddenField runat="server" ID="AllceRadioButtonListId" />
        </td>
    </tr>
--%>    <tr>
        <td class="left">
            <span class="label">Date of Document</span><span class="form-error" runat="server" id="Span1" >&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="FromDate" RequiredFieldErrorMessage="Date of Document is required." Visible="true"/>
        </td>
    </tr>
<%--    <tr runat="server" id="NameOfCompanyTR">
        <td>
            <span class="label">Name of Company</span><span class="form-error" runat="server" id="NameOfCompanySpan" >&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfCompany"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NameOfCompanyRequiredFieldValidator" runat="server" CssClass="form-error" ControlToValidate="NameOfCompany" 
                ErrorMessage="<br />Name of company is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfCompanyId" />
        </td>
    </tr>
--%></table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CPFContribution.ascx.cs" Inherits="Verification_Control_CPFContribution" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="To date must be later than From Date" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">From Date </span><span class="form-error" runat="server" id="Span1" >&nbsp;*</span><br />
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="FromDate" RequiredFieldErrorMessage="From Date is required." Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">To Date </span><span class="form-error" runat="server" id="Span2" >&nbsp;*</span><br />
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel1"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="ToDate" RequiredFieldErrorMessage="To Date is required."/>
        </td>
    </tr>
	
<%--    <tr runat="server" id="ConsistentContributionTR">
        <td>
            <span class="label">Consistent Contribution </span><span class="form-error" runat="server" id="TypeSpan" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="ConsistentContributionRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                <asp:ListItem Value="No" Text="No" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:HiddenField runat="server" ID="ConsistentContributionRadioButtonListId" />
        </td>
    </tr>
    <tr runat="server" id="CompanyName1TR">
        <td>
            <span class="label">Name of Company 1 </span><span class="form-error" runat="server" id="CompanyName1Span" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="CompanyName1"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorCompanyName1" runat="server" CssClass="form-error" ControlToValidate="CompanyName1" 
                ErrorMessage="<br />Company name 1 is required." Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="CompanyName1Id" />
        </td>
    </tr>
    <tr runat="server" id="CompanyName2TR">
        <td>
            <span class="label">Name of Company 2 </span><span class="form-error" runat="server" id="CompanyName2Span" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="CompanyName2"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorCompanyName2" runat="server" CssClass="form-error" ControlToValidate="CompanyName2" 
                ErrorMessage="<br />Company Name 2 is required." Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="CompanyName2Id" />
        </td>
    </tr>
--%></table>
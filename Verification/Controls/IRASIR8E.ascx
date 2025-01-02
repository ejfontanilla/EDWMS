<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IRASIR8E.ascx.cs" Inherits="Verification_Control_IRASIR8E" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Filing must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Year of Assessment </span><span class="form-error" runat="server" id="IdentityNoSpan">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="YearOfAssessment"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorYearOfAssessment" runat="server" CssClass="form-error" ControlToValidate="YearOfAssessment" 
                ErrorMessage="<br />Year of assessment is required." Display="Dynamic" Enabled="true"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="YearOfAssessmentId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Date of Filing </span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfFiling" RequiredFieldErrorMessage="Date of Filing is required." Visible="true"/>
        </td>
    </tr>
<%--    <tr runat="server" id="TypeOfIncomeTR">
        <td>
            <span class="label">Type of Income</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <telerik:RadComboBox runat="server" ID="TypeOfIncome"/>
            <asp:HiddenField runat="server" ID="TypeOfIncomeId" />
        </td>
    </tr>
--%></table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OfficialAssignee.ascx.cs" Inherits="Verification_Control_OfficialAssignee" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td class="left">
            <span class="label">Bankruptcy No. &<br />Year of Bankruptcy</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="BankruptcyNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfChild" runat="server" CssClass="form-error" ControlToValidate="BankruptcyNo" 
                ErrorMessage="Bankruptcy No. & Year of Bankruptcy is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="BankruptcyNoId" />
        </td>
    </tr>
</table>
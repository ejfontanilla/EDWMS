<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IdentityCard.ascx.cs" Inherits="Verification_Control_IdentityCard" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Issue must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Issue</span><span class="form-error" runat="server" id="Span5">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfIssue" RequiredFieldErrorMessage="Date of Issue is required." Visible="true"/>
        </td>
    </tr>
    <tr runat="server" id="AddressTR">
        <td>
            <span class="label">NRIC Address same as Sers Address</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="AddressRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                <asp:ListItem Value="No" Text="No" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>                
            <asp:HiddenField runat="server" ID="AddressRadioButtonListId" />
        </td>
    </tr>

</table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NoLoanNotification.ascx.cs" Inherits="Verification_Control_NoLoanNotification" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td class="left">
            <span class="label">Date of Signature</span><span class="form-error" runat="server" id="Span5">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="DateOfSignatureRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                <asp:ListItem Value="No" Text="No"></asp:ListItem>
            </asp:RadioButtonList>                
            <asp:RequiredFieldValidator ID="ReqiredFieldValidator1" runat="server" ControlToValidate="DateOfSignatureRadioButtonList" 
            CssClass="form-error" ErrorMessage="Date of Signature is required."/> 
            <asp:HiddenField runat="server" ID="DateOfSignatureRadioButtonListId" />
        </td>
    </tr>
</table>
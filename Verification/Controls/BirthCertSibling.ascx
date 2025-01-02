<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BirthCertSibling.ascx.cs" Inherits="Verification_Controls_BirthCertSibling" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="true" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td class="left">
            <span class="label">Tag</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="TagRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="TagRadioButtonList_SelectedIndexChanged">
                <asp:ListItem Value="Local" Text="Local"></asp:ListItem>
                <asp:ListItem Value="Foreign" Text="Foreign" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:HiddenField runat="server" ID="TagRadioButtonListId" />
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNo" runat="server" CssClass="form-error" ControlToValidate="IdentityNo" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CustomValidator id="NricFinCustomValidator" runat="server" 
                    ControlToValidate="IdentityNo" Display="Dynamic"  
                    OnServerValidate="NricCustomValidator" 
                    ErrorMessage="Enter a valid ID no. ex(S1234567A)." CssClass="form-error">
                </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDType" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name of Sibling</span><span class="form-error" runat="server" id="Span3">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfSibling"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfSibling" runat="server" CssClass="form-error" ControlToValidate="NameOfSibling" 
                ErrorMessage="<br />Name of Sibling is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfSiblingId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Adoption.ascx.cs" Inherits="Verification_Control_Adoption" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="true" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td class="left">
            <span class="label">Tag</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="TagRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal"><%-- OnSelectedIndexChanged="TagRadioButtonList_SelectedIndexChanged">--%>
                <asp:ListItem Value="Local" Text="Local"></asp:ListItem>
                <asp:ListItem Value="Foreign" Text="Foreign" Selected="True"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:HiddenField runat="server" ID="TagRadioButtonListId" />
        </td>
    </tr>
<%--    <tr>
        <td>
            <span class="label">Name of Child</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfChild"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfChild" runat="server" CssClass="form-error" ControlToValidate="NameOfChild" 
                ErrorMessage="<br />Name of child is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfChildId" />
        </td>
    </tr>
--%>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
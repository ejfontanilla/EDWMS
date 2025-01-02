<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DivorceDocInitial.ascx.cs"
    Inherits="Verification_Control_DivorceDocInitial" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>
<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true"
    EnableRequiredField="true" />
<div class="grey-header">
    <%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Order must be before today"
                Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Tag</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <telerik:RadComboBox runat="server" ID="TagRadioButtonList" AutoPostBack="true" CausesValidation="false"
                OnSelectedIndexChanged="TagRadioButtonList_SelectedIndexChanged" />
            <asp:HiddenField runat="server" ID="TagRadioButtonListId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Date of Order </span><span class="form-error" runat="server"
                id="DateOfOrderSpan" visible="false">&nbsp;*</span>
            <br />
            <span class="note">(<%= Dwms.Bll.Format.GetMetaDataDateFormat()%>)</span><!--Modified by Edward 2016/05/12 Take out DateFormatLabel UC-->
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfOrder" RequiredFieldErrorMessage="Date of Order is required."
                EnableRequiredField="true" Visible="true" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Requestor</div>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No (Requestor) </span><span class="form-error" runat="server"
                id="Span1" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoRequestor" runat="server"
                CssClass="form-error" ControlToValidate="IdentityNoRequestor" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorRequestor" runat="server" ControlToValidate="IdentityNoRequestor"
                Display="Dynamic" OnServerValidate="NricCustomValidatorRequestor" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type (Requestor)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeRequestor" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Requestor)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfRequestor" runat="server"
                CssClass="form-error" ControlToValidate="NameOfRequestor" ErrorMessage="<br />Name is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfRequestorId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Spouse</div>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity No (Spouse)</span><span class="form-error" runat="server"
                id="Span2" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoSpouse" runat="server"
                CssClass="form-error" ControlToValidate="IdentityNoSpouse" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorSpouse" runat="server" ControlToValidate="IdentityNoSpouse"
                Display="Dynamic" OnServerValidate="NricCustomValidatorSpouse" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type (Spouse)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeSpouse" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Spouse)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfSpouse" runat="server"
                CssClass="form-error" ControlToValidate="NameOfSpouse" ErrorMessage="<br />Name is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error">
            <%= Dwms.Bll.Constants.TagMandatoryMsg%>
        </td>
    </tr>
</table>

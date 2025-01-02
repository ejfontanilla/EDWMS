<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarriageCertLtSpouse.ascx.cs"
    Inherits="Verification_Control_MarriageCertLtSpouse" %>
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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Marriage must be before today"
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
            <span class="label">Date of Marriage </span><span class="form-error" runat="server"
                id="DateOfMarriageSpan" visible="false">&nbsp;*</span>
            <br />
            <span class="note">(<%= Dwms.Bll.Format.GetMetaDataDateFormat()%>)</span><!--Modified by Edward 2016/05/12 Take out DateFormatLabel UC-->
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfMarriage" RequiredFieldErrorMessage="Date of Marriage is required."
                EnableRequiredField="true" Visible="true" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Marriage Cert No </span><span class="form-error" runat="server"
                id="MarriageCertNoSpan" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="MarriageCertNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorMarriageCertNo" runat="server"
                CssClass="form-error" ControlToValidate="MarriageCertNo" ErrorMessage="<br />Marriage Cert No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="MarriageCertNoId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Requestor - Household</div>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span><span id="Span1" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoRequestor" runat="server"
                CssClass="form-error" ControlToValidate="IdentityNoRequestor" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorRequestor" runat="server" ControlToValidate="IdentityNoRequestor"
                Display="Dynamic" OnServerValidate="NricCustomValidatorRequestor" class="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeRequestor" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeRequestorId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Requestor - Image</div>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="form-error"
                ControlToValidate="IdentityNoImageRequestor" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorImageRequestor" runat="server" ControlToValidate="IdentityNoImageRequestor"
                Display="Dynamic" OnServerValidate="NricCustomValidatorImageRequestor" class="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoImageRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeImageRequestor" AutoPostBack="true"
                CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeImageRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="form-error"
                ControlToValidate="NameOfRequestor" ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfRequestorId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Ex-Spouse - Image</div>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity No</span><span id="Span4" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageLateSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoImageLateSpouse"
                runat="server" CssClass="form-error" ControlToValidate="IdentityNoImageLateSpouse"
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorImageLateSpouse" runat="server" ControlToValidate="IdentityNoImageLateSpouse"
                Display="Dynamic" OnServerValidate="NricCustomValidatorLateSpouse" class="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoImageLateSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeImageLateSpouse" AutoPostBack="true"
                CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeImageLateSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfLateSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfLateSpouse" runat="server"
                CssClass="form-error" ControlToValidate="NameOfLateSpouse" ErrorMessage="<br />Name is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfLateSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error">
            <%= Dwms.Bll.Constants.TagMandatoryMsg%>
        </td>
    </tr>
</table>

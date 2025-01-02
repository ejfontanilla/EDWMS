<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarriageCertificate.ascx.cs"
    Inherits="Verification_Control_MarriageCertificate" %>
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
            <span class="NameOfRequestorId">Marriage Cert No </span><span class="form-error"
                runat="server" id="MarriageCertNoSpan" visible="false">&nbsp;*</span>
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
            <asp:TextBox runat="server" ID="IdentityNoRequestor" OnTextChanged="IdentityNoRequestor_TextChanged"></asp:TextBox>
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
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeRequestor" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIDTypeRequestor" runat="server"
                CssClass="form-error" ControlToValidate="IDTypeRequestor" ErrorMessage="<br />Identity Type is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
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
            <span class="label">Identity No</span><span id="Span3" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageRequestor" OnTextChanged="IdentityNoImageRequestor_TextChanged"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="form-error"
                ControlToValidate="IdentityNoImageRequestor" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorImageRequestor" runat="server" ControlToValidate="IdentityNoImageRequestor"
                Display="Dynamic" OnServerValidate="NricCustomValidatorImageRequestor" CssClass="form-error"
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
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIDTypeImageRequestor" runat="server"
                CssClass="form-error" ControlToValidate="IDTypeImageRequestor" ErrorMessage="<br />Identity Type is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="IDTypeImageRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="form-error"
                ControlToValidate="NameOfRequestor" ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfRequestorId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Spouse - Household</div>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoSpouse" OnTextChanged="IdentityNoSpouse_TextChanged"></asp:TextBox>
            <asp:CustomValidator ID="NricFinCustomValidatorSpouse" runat="server" ControlToValidate="IdentityNoSpouse"
                Display="Dynamic" OnServerValidate="NricCustomValidatorSpouse" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorSpouseHousehold" runat="server" ControlToValidate="IdentityNoSpouse"
                Display="Dynamic" OnServerValidate="NricCustomValidatorSpouseHousehold" CssClass="form-error"
                ErrorMessage="Identity No should be part of household structure.">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeSpouse" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIDTypeSpouse" runat="server"
                CssClass="form-error" ControlToValidate="IDTypeSpouse" ErrorMessage="<br />Identity Type is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="IDTypeSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <div class="grey-header-light">
                Spouse - Image</div>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span><span id="Span4" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageSpouse" OnTextChanged="IdentityNoImageSpouse_TextChanged"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoImageSpouse" runat="server"
                CssClass="form-error" ControlToValidate="IdentityNoImageSpouse" ErrorMessage="<br />Identity No is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator ID="NricFinCustomValidatorImageSpouse" runat="server" ControlToValidate="IdentityNoImageSpouse"
                Display="Dynamic" OnServerValidate="NricCustomValidatorImageSpouse" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoImageSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeImageSpouse" AutoPostBack="true" CausesValidation="false"
                RepeatDirection="Horizontal" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIDTypeImageSpouse" runat="server"
                CssClass="form-error" ControlToValidate="IDTypeImageSpouse" ErrorMessage="<br />Identity Type is required."
                Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="IDTypeImageSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
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
            <asp:HiddenField runat="server" ID="HiddenFieldIsUnderHouseholdStructure" />
            <asp:HiddenField runat="server" ID="HiddenFieldDocRefId" />
        </td>
    </tr>
</table>

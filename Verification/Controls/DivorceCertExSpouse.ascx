<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DivorceCertExSpouse.ascx.cs" Inherits="Verification_Control_DivorceCertExSpouse" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="true" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Divorce must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Tag</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <telerik:RadComboBox runat="server" ID="TagRadioButtonList" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="TagRadioButtonList_SelectedIndexChanged"/>
            <asp:HiddenField runat="server" ID="TagRadioButtonListId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Date of Divorce </span><span class="form-error" runat="server" id="DateOfDivorceSpan" visible="false">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfDivorce" RequiredFieldErrorMessage="Date of Divorce is required." EnableRequiredField="true" Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Divorce Case No </span><span class="form-error" runat="server" id="DivorceCaseSpan" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="DivorceCaseNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorDivorceCaseNo" runat="server" CssClass="form-error" ControlToValidate="DivorceCaseNo" 
                ErrorMessage="<br />Divorce Case No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="DivorceCaseNoId" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Requestor</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No (Requestor) </span><span class="form-error" runat="server" id="Span1" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoRequestor" runat="server" CssClass="form-error" ControlToValidate="IdentityNoRequestor" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorRequestor" runat="server" 
                ControlToValidate="IdentityNoRequestor" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorRequestor" CssClass="form-error"
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
            <asp:RadioButtonList runat="server" ID="IDTypeRequestor" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeRequestorId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Requestor)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfRequestor"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfRequestor" runat="server" CssClass="form-error" ControlToValidate="NameOfRequestor" 
                ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfRequestorId" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Ex-Spouse</div></td></tr>
    <tr>
        <td>
            <span class="label">Identity No (Ex-Spouse)</span><span class="form-error" runat="server" id="Span2" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoExSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoExSpouse" runat="server" CssClass="form-error" ControlToValidate="IdentityNoExSpouse" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorExSpouse" runat="server" 
                ControlToValidate="IdentityNoExSpouse" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorExSpouse" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoExSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type (Ex-Spouse)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeExSpouse" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal"/>
            <asp:HiddenField runat="server" ID="IDTypeExSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Ex-Spouse)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfExSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfExSpouse" runat="server" CssClass="form-error" ControlToValidate="NameOfExSpouse" 
                ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfExSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
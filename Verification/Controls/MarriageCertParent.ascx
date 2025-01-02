<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MarriageCertParent.ascx.cs" Inherits="Verification_Control_MarriageCertParent" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Marriage must be before today" Visible="false" CssClass="form-error"></asp:Label>
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
            <span class="label">Date of Marriage</span><span class="form-error" runat="server" id="DateOfMarriageSpan" visible="false">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfMarriage" RequiredFieldErrorMessage="Date of Marriage is required." EnableRequiredField="true" Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Marriage Cert No </span><span class="form-error" runat="server" id="MarriageCertNoSpan" visible="false">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="MarriageCertNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorMarriageCertNo" runat="server" CssClass="form-error" ControlToValidate="MarriageCertNo" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="MarriageCertNoId" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Parent - Image</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No</span><span id="Span3" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageParent"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoImageParent" runat="server" CssClass="form-error" ControlToValidate="IdentityNoImageParent" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="CustomValidator1" runat="server" 
                ControlToValidate="IdentityNoImageParent" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorParent" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoImageParentId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeImageParent" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeImageParentId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfParent"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfParent" runat="server" CssClass="form-error" ControlToValidate="NameOfParent" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfParentId" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Spouse - Image</div></td></tr>
    <tr>
        <td>
            <span class="label">Identity No</span><span id="Span4" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoImageSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoImageSpouse" runat="server" CssClass="form-error" ControlToValidate="IdentityNoImageSpouse" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="CustomValidator2" runat="server" 
                ControlToValidate="IdentityNoImageSpouse" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorSpouse" CssClass="form-error"
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
            <asp:RadioButtonList runat="server" ID="IDTypeImageSpouse" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeImageSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfSpouse" runat="server" CssClass="form-error" ControlToValidate="NameOfSpouse" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DivorceCertMother.ascx.cs" Inherits="Verification_Control_DivorceCertMother" %>

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
    <tr><td colspan="2"><div class="grey-header-light">Mother</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No (Mother) </span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoMother"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoMother" runat="server" CssClass="form-error" ControlToValidate="IdentityNoMother" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorMother" runat="server" 
                ControlToValidate="IdentityNoMother" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorMother" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoMotherId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type (Mother)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeMother" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeMotherId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Mother)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfMother"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfMother" runat="server" CssClass="form-error" ControlToValidate="NameOfMother" 
                ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfMotherId" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Spouse</div></td></tr>
    <tr>
        <td>
            <span class="label">Identity No (Spouse)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoSpouse" runat="server" CssClass="form-error" ControlToValidate="IdentityNoSpouse" 
                ErrorMessage="<br />Identity No is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorSpouse" runat="server" 
                ControlToValidate="IdentityNoSpouse" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorSpouse" CssClass="form-error"
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
            <asp:RadioButtonList runat="server" ID="IDTypeSpouse" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal"/>
            <asp:HiddenField runat="server" ID="IDTypeSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name (Spouse)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfSpouse" runat="server" CssClass="form-error" ControlToValidate="NameOfSpouse" 
                ErrorMessage="<br />Name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
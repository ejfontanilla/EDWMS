﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DeathCertificateSP.ascx.cs" Inherits="Verification_Control_DeathCertificateSP" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Death must be before today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
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
        <td>
            <span class="label">Date of Death </span><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfDeath" RequiredFieldErrorMessage="Date of Death is required." EnableRequiredField="true" Visible="true"/>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Identity No of Spouse</span><span id="Span1" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoOfSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorIdentityNoOfSpouse" runat="server" CssClass="form-error" ControlToValidate="IdentityNoOfSpouse" 
                ErrorMessage="<br />Identity No of spouse is required." Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:CustomValidator id="NricFinCustomValidator" runat="server" 
                    ControlToValidate="IdentityNoOfSpouse" Display="Dynamic"  
                    OnServerValidate="NricCustomValidator" CssClass="form-error"
                    ErrorMessage="Enter a valid ID no. ex(S1234567A).">
                </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoOfSpouseId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ID Type of Spouse</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDType" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal"/>
            <asp:HiddenField runat="server" ID="IDTypeId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name of Spouse</span><span class="form-error" runat="server" id="Span3">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameOfSpouse"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfSpouse" runat="server" CssClass="form-error" ControlToValidate="NameOfSpouse" 
                ErrorMessage="<br />Name of spouse is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="NameOfSpouseId" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="form-error"><%= Dwms.Bll.Constants.TagMandatoryMsg%></td>
    </tr>
</table>
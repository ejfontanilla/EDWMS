<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SpouseFormPurchase.ascx.cs" Inherits="Verification_Control_SpouseFormPurchase" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
    <tr>
        <td class="left">
            <span class="label">Spouse ID</span><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="SpouseID"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="form-error" ControlToValidate="SpouseID" 
                ErrorMessage="<br />Spouse Id is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorSpouse" runat="server" 
                ControlToValidate="SpouseID" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorSpouse" 
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="SpouseIDId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Spouse  ID Type</span><span id="Span6" class="form-error" runat="server">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDType" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeId" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Spouse Name</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="SpouseName"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorNameOfChild" runat="server" CssClass="form-error" ControlToValidate="SpouseName" 
                ErrorMessage="<br />Spouse name is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="SpouseNameId" />
        </td>
    </tr>
</table>
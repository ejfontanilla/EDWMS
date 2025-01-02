<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PetitionforGLA.ascx.cs" Inherits="Verification_Control_PetitionforGLA" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Issue must be after today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Issue<%--<br />(Grant of Probate)--%></span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfIssue" RequiredFieldErrorMessage="Date of Issue is required." Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Id No<br />(Deceased)</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdNo"></asp:TextBox>
            <asp:RequiredFieldValidator ID="IdNoRequiredFieldValidator" runat="server" CssClass="form-error" ControlToValidate="IdNo" 
                ErrorMessage="<br />IdNo is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:CustomValidator id="NricFinCustomValidatorRequestor" runat="server" 
                ControlToValidate="IdNo" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorRequestor" 
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdNoId" />
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

</table>
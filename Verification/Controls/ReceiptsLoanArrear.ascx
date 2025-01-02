<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReceiptsLoanArrear.ascx.cs" Inherits="Verification_Control_ReceiptsLoanArrear" %>

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
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Statement must be after today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Statement</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfStatement" RequiredFieldErrorMessage="Date of Statement is required." Visible="true"/>
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">ABCDE Ref</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="ABCDERef"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" CssClass="form-error" ControlToValidate="ABCDERef" 
                ErrorMessage="<br />ABCDE ref is required." Display="Dynamic"></asp:RequiredFieldValidator>
            <asp:HiddenField runat="server" ID="ABCDERefId" />
        </td>
    </tr>
</table>
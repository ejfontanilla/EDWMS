<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateTextBox.ascx.cs" Inherits="Controls_Date" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:TextBox runat="server" ID="DateTextBox" OnTextChanged="DateTextBox_TextChanged" AutoPostBack="true"></asp:TextBox>
<asp:RequiredFieldValidator ID="RequiredFieldValidatorDate" runat="server" CssClass="form-error" ControlToValidate="DateTextBox" 
    Display="Dynamic" ErrorMessage="Date is required."></asp:RequiredFieldValidator>
<asp:CustomValidator ID="CustomValidator" runat="server" 
    ControlToValidate="DateTextBox" 
    OnServerValidate="CustomValidator_ServerValidate" 
    ErrorMessage="Incorrect date format">
</asp:CustomValidator>
<asp:HiddenField runat="server" ID="DateId" />
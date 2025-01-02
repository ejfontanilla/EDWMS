<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EmailTextBox.ascx.cs" Inherits="Controls_Email" %>
<asp:TextBox runat="server" ID="Email"></asp:TextBox>
<asp:RequiredFieldValidator ID="RequiredFieldValidatorEmail" runat="server" CssClass="form-error" ControlToValidate="Email" 
    Display="Dynamic" ErrorMessage="<br />Email  is required."></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidatorEmail" CssClass="form-error" ControlToValidate="Email" 
    Display="Dynamic" ErrorMessage="<br />Email  is in wrong format." ValidationExpression="^(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\b([,;]\s?)?)*$"/>
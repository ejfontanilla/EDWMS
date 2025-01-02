<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NricTextBox.ascx.cs" Inherits="Controls_Nric" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:TextBox runat="server" ID="Nric" ontextchanged="Nric_TextChanged"></asp:TextBox>
<asp:RequiredFieldValidator ID="RequiredFieldValidatorNric" runat="server" CssClass="form-error" ControlToValidate="Nric" 
    Display="Dynamic" ErrorMessage="<br />NRIC  is required."></asp:RequiredFieldValidator>
<asp:CustomValidator id="NricFinCustomValidator" runat="server" 
    ControlToValidate="Nric" Display="Dynamic"  
    OnServerValidate="NricCustomValidator" 
    ErrorMessage="Enter a valid NRIC. ex(S1234567A).">
</asp:CustomValidator>
<asp:HiddenField runat="server" ID="NricId" />
<%@ Page Language="C#" MasterPageFile="~/Login/Login.master" AutoEventWireup="true"
    CodeFile="Contact.aspx.cs" Inherits="Login_Default" Title="DWMS - Login" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="wrapper20" style="padding-left: 40px; text-align: left;">
        Please use an email below to contact the system administrator in charge.<br />
        Clicking on an email link will launch your client mail application:<br />
        <br />
        <table width="90%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td colspan="4" bgcolor="#EEEEEE" height="5">
                </td>
            </tr>
            <tr>
                <td colspan="4" height="15">
                </td>
            </tr>   
            <tr>
                <td width="25%" valign="top">
                    Admin & Accounting<br />
                    <asp:HyperLink ID="AADHyperLink1" runat="server"/><br />
                    <asp:HyperLink ID="AADHyperLink2" runat="server"/>
                </td>
                <td width="30%" valign="top">
                    Projects & Redevelopment<br />
                    <asp:HyperLink ID="PRDHyperLink1" runat="server" />
                </td>
                <td width="20%" valign="top">
                    Resale<br />
                    <asp:HyperLink ID="RSDHyperLink1" runat="server" /><br />
                    <asp:HyperLink ID="RSDHyperLink2" runat="server" />
                </td>
                <td width="25%" valign="top">
                    Sales<br />
                    <asp:HyperLink ID="SSDHyperLink1" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="4" height="15">
                </td>
            </tr>
            <tr>
                <td colspan="4" bgcolor="#EEEEEE" height="5">
                </td>
            </tr>
        </table>
        <br />
        <a href="Default.aspx">Return to the login page</a>
    </div>
</asp:Content>

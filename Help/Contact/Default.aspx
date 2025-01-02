<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Contact System Administrator
    </div>
    <br />
    Please use an email below to contact the system administrator in charge.<br />
    Clicking on an email link will launch your client mail application:<br /><br />
    <table width="100%" >
        <tr>
            <td width="25%" valign="top">
                <b>Admin & Accounting</b><br />
                <asp:HyperLink ID="AADHyperLink1" runat="server"/><br />
                <asp:HyperLink ID="AADHyperLink2" runat="server"/>
            </td>
            <td width="30%" valign="top">
                <b>Projects & Redevelopment</b><br />
                <asp:HyperLink ID="PRDHyperLink1" runat="server" />
            </td>
            <td width="20%" valign="top">
                <b>Resale</b><br />
                <asp:HyperLink ID="RSDHyperLink1" runat="server" /><br />
                <asp:HyperLink ID="RSDHyperLink2" runat="server" />
            </td>
            <td width="25%" valign="top">
                <b>Sales</b><br />
                <asp:HyperLink ID="SSDHyperLink1" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

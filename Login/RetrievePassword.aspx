<%@ Page Language="C#" MasterPageFile="~/Login/Login.master" AutoEventWireup="true" CodeFile="RetrievePassword.aspx.cs" Inherits="Login_Default" Title="DWMS - Login" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="wrapper20" style="padding-left: 40px; text-align: left;">
        Your login User ID and Password should be the same as your Active Directory (AD) credentials.<br />
        If you forgot your password, please <a href="http://his95/ISSC003p.nsf/PwRequestWeb?OpenForm" target="_blank">
            contact a ABCDE system administrator</a>.<br />
        <br />
        <br />
        <a href="Default.aspx">Return to the login page</a>
    </div>
</asp:Content>

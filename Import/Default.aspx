<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" 
    CodeFile="Default.aspx.cs" Inherits="Import_Default" Title="User Accounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" Runat="Server">
    <span class="title">User Accounts</span>
    <asp:Button ID="SubmitButtonCpf" runat="server" Text="New" CssClass="button-large right20"
        UseSubmitBehavior="false" />
    <img src="../Data/Images/UserAccounts.gif" alt="UserAccounts"/>
</asp:Content>


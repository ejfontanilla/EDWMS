<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MainMenu.ascx.cs" Inherits="Controls_MainMenu" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
    <telerik:RadMenu ID="RadMainMenu" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif" 
        Font-Size="16px" CssClass="TopMenu" Width="100%" Style="z-index: 2000" onprerender="RadMainMenu_PreRender">
</telerik:RadMenu>
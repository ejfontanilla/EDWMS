<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
Please click 
<asp:HyperLink ID="HyperLink2" runat="server" Text="here" Target="_blank" NavigateUrl="~/Data/Help/DWMS User Guide for Verification and Completeness Officers v1.0.pdf" /> to access the help guide.
</asp:Content>
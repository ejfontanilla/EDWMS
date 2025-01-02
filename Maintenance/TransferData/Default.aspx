<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_TransferData_Default" Title="Untitled Page" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" Runat="Server">
    <div class="title">Transfer Data to Mainframe System</div>
   
    
<telerik:RadGrid ID="grid1" runat="server">

</telerik:RadGrid>
<br />
Select File to Upload:
    <asp:DropDownList ID="DropDownList1" runat="server">
    </asp:DropDownList>

<br />
    <asp:Button ID="ButtonUpload" runat="server" Text="Upload to Main Frame" 
        onclick="ButtonUpload_Click" />
        <br />
        <br />
         <asp:Label ID="LabelMsg" runat="server" Text=""></asp:Label>
    
</asp:Content>


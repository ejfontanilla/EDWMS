<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_EmailTemplates_Default" Title="Email Templates" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" Runat="Server">
    <div class="title">Email Templates</div>
    <img src="../Data/Images/EmailTemplates.gif" />
        <center>
            <asp:Panel ID="Panel2" runat="server" CssClass="form-submit">
                <asp:Button ID="SubmitButtonCpf" runat="server" Text="Save" CssClass="button-large right20"
                    UseSubmitBehavior="false" />
            </asp:Panel>
        </center>
</asp:Content>


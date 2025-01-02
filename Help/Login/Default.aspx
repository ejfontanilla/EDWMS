<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Login
    </div>
    <br />
    Please click
    <asp:HyperLink ID="HyperLink2" runat="server" Text="here" Target="_blank" NavigateUrl="~/Data/Help/DWMS User Guide for Verification and Completeness Officers v1.0.pdf" />
    to access the help guide in PDF format.
    <br />
    <br />
    
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Login into the System
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Open a web browser and go to http://heasq2/go-dwms/</li>
                <br />
                <br />
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/Login_Login Page.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Enter your Active Directory ID (Username) and password. The credential should be
                    the same as the one you use to log into LEAS.</li>
                <li>Click Login. You will be redirected to the Scan Documents page upon successful login.</li>
                <li>If you have Click Login. You will be redirected to the Scan Documents page upon successful login.</li>
            </ol>
        </div>
    </asp:Panel>
</asp:Content>

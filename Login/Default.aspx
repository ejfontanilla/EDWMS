<%@ Page Language="C#" MasterPageFile="~/Login/Login.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Login_Default" Title="DWMS - Login" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" Runat="Server">
    <table width="100%" style="height: 100%">
        <tr>
            <td valign="top" width="60%" align="left" class="login">
                <div class="wrapper20" style="padding-left: 40px;">
                    <div class="subtitle">
                        About
                    </div>
                    <div align="justify">
                        The Document Workflow Management System (DWMS) is a versatile application built to digitize, process and manage the documents 
                        submitted by customers. The system provides complete workflow management capabilities from document polling, categorization, 
                        processing to maintenance. It offers comprehensive administrative functions for user account and access control management, 
                        query and reporting, audit trail, data archival and system configuration, with the capability to seamlessly interface with 
                        other ABCDE systems. Its integrated modules delivers useful features in a single, easy-to-use platform.
                    </div>
                </div>
            </td>
            <td width="5" bgcolor="#EEEEEE">
            </td>
            <td valign="top" align="left">
                <div class="wrapper20">
                    <div class="subtitle">
                        Login
                    </div>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td width="30%" class="login" nowrap="nowrap">
                                Active Directory <br /> User Name*
                            </td>
                            <td width="70%">
                                <asp:TextBox ID="UserNameTextBox" runat="server" CssClass="formField" Width="120"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="NricRequired" runat="server" ControlToValidate="UserNameTextBox"
                                    ErrorMessage="*" ForeColor="" CssClass="form-error" EnableClientScript="false"
                                    AutoUpdateAfterCallBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td class="login">
                                Password*
                            </td>
                            <td>
                                <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" CssClass="formField" Width="120"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="PasswordTextBox"
                                    ErrorMessage="*" ForeColor="" CssClass="form-error" EnableClientScript="false"
                                    AutoUpdateAfterCallBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="top20" /><asp:Label ID="ErrorLabel" CssClass="form-error" runat="server" Visible="false" />
                            </td>
                        </tr>                                        
                        <tr runat="server" visible="false">
                            <td></td>
                            <td align="left"><asp:CheckBox Text="Remember Me" runat="server" ID="RememberMeCheckBox" TextAlign="Right" Visible="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Panel ID="ErrorPanel" runat="server" CssClass="form-error" Visible="false" EnableViewState="false">
                                The username and password combination is incorrect.<br />Please try again.</asp:Panel>
                                <div class="top10">
                                    <asp:ImageButton ID="LoginButton" runat="server" OnClick="DoLogin"
                                    ImageUrl="~/Data/ImagesLogin/BtnLogin.gif" ToolTip="Login" />
                                </div>
                            </td>
                        </tr>
                    </table>
                    <div class="top10" />
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="RetrievePassword.aspx" Text="Forgot your password?" /><br />                         
                        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="Contact.aspx?t=problem" Text="Problems logging in?" />                      
                    </div>
            </td>
        </tr>
    </table>
</asp:Content>
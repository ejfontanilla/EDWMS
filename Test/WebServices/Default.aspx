<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Test_WebServices_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="TextBox1" runat="server" value="1,3,5"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" Text="Send" onclick="Button1_Click" /><br /><br />
        Returned by Web Services: <asp:Label ID="Label1" runat="server" Text="NA"></asp:Label>
    </div>
    </form>
</body>
</html>

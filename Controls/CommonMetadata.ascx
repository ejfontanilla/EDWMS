<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommonMetadata.ascx.cs" Inherits="Controls_CommonMedata" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<div class="grey-header">
    Document Owner
</div>
<table>
    <tr>
        <td class="left">
            <asp:Label runat="server" ID="NRICLabel" class="label">ID no</asp:Label><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
        </td>
        <td>
                <asp:TextBox runat="server" ID="Nric" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorNric" runat="server" CssClass="form-error" ControlToValidate="Nric" 
                    Display="Dynamic" ErrorMessage="<br />ID no  is required."></asp:RequiredFieldValidator>
                <asp:CustomValidator id="NricFinCustomValidator" runat="server" 
                    ControlToValidate="Nric" Display="Dynamic"  
                    OnServerValidate="NricCustomValidator" CssClass="form-error"
                    ErrorMessage="Enter a valid ID no. ex(S1234567A).">
                </asp:CustomValidator>
                <asp:HiddenField runat="server" ID="NricId" />
                <asp:Label runat="server" ID="lblInvalid" Text="Enter a valid ID no. ex(S1234567A)" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <asp:Label runat="server" ID="NameLabel" class="label">Name</asp:Label><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
                <asp:TextBox runat="server" ID="Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidatorName" runat="server" CssClass="form-error" ControlToValidate="Name" 
                    Display="Dynamic" ErrorMessage="<br />Name  is required."></asp:RequiredFieldValidator>
                    <asp:CustomValidator id="NameCustomValidator" runat="server" 
                    ControlToValidate="Name" Display="Dynamic"  
                    OnServerValidate="Name_CustomValidator" CssClass="form-error"
                    ErrorMessage="Enter a valid Name"></asp:CustomValidator>
                <asp:HiddenField runat="server" ID="NameId" />
        </td>
    </tr>
    <tr>
        <td>
            <asp:Label runat="server" ID="IdTypeLabel" class="label">ID Type</asp:Label><span class="form-error" runat="server" id="Span3">&nbsp;*</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDType" AutoPostBack="true" 
                CausesValidation="false" RepeatDirection="Horizontal" />

                <asp:CustomValidator id="IDTypeValidator" runat="server" 
                    ControlToValidate="IDType" Display="Dynamic"  
                    OnServerValidate="IDTypeCustomValidator" CssClass="form-error"
                    ErrorMessage="Please enter Id No.">
                </asp:CustomValidator>
                
            <asp:HiddenField runat="server" ID="IDTypeId" />
            <asp:HiddenField runat="server" ID="CustomerSourceId" />
        </td>
    </tr>
    <asp:Panel runat="server" ID="CustomerDetailsPanel">
        <!--Currenlty this section is not in use. KIV-->
    </asp:Panel>
</table>

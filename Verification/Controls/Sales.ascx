<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Sales.ascx.cs" Inherits="Verification_Control_Sales" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
    <asp:Repeater runat="server" ID="PersonalMetaDataRepeater" OnItemDataBound="PersonalMetaDataRepeater_OnItemDataBound">
        <ItemTemplate>
            <div class="grey-header">
                <asp:Label ID="PersonalTypeLabel" runat="server" Text='<%@# Eval("PersonalType") %>'
                    Font-Bold="true"></asp:Label>
            </div>
            <table>
                <tr>
                    <td class="left">
                        <span class="label">NRIC</span><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
                    </td>
                    <td>
                        <uc:NricTextBox runat="server" ID="CustomNric" EnableCustomValidator="true" EnableRequiredField="true" 
                        NricValue='<%# Eval("Nric").ToString() %>' ValidationGroupNric="MetaDataValidationGroup"/>
                        <asp:HiddenField runat="server" ID="AppPersonalIdHiddenField" Value='<%# Eval("Id").ToString() %>' />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label CssClass="label" runat="server" ID="NameLabel">Name</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="Name" Text='<%# Eval("Name").ToString() %>'
                            CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NameRequiredFieldValidator" runat="server"
                            CssClass="form-error" ControlToValidate="Name" ErrorMessage="<br />Name is required."
                            Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:Repeater>

    <table>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary runat="server" ID="MetaDataValidationSummary" ValidationGroup="MetaDataValidationGroup" />
            </td>
        </tr>
    </table>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SplitDocument.ascx.cs"
    Inherits="Controls_SplitDocument" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
    <div class="header">
        <div class="left">
            Split Document</div>
        <div class="right">
            <!---->
        </div>
    </div>
    <div class="area">
        <asp:Label runat="server" ID="MessageLabel" Text="There should be more than one page to split"></asp:Label>
        <asp:CheckBoxList runat="server" ID="PagesCheckBoxList"></asp:CheckBoxList>
        <asp:Label CssClass="label" runat="server" ID="SplitTypeLabel" Text="Split Type"></asp:Label>
        <asp:RadioButtonList runat="server" ID="SplitTypeRadioButtonList" RepeatDirection="Horizontal"/>
    </div>
    <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
        <asp:Button ID="SplitDocumentButton" runat="server" Text="Split Document" CssClass="button-large2"
            OnClick="SplitDocumentButton_Click" CausesValidation="false" />
    </asp:Panel>
</asp:Panel>
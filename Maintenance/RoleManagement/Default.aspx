<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Default" Title="DWMS - Role Management" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <table width="100%">
        <tr>
            <td>
                <asp:Panel ID="TitlePanel" runat="server" CssClass="title">
                    Roles Management
                    <input type="button" value="New" class="button-small" id="NewButton" runat="server" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
        AllowFilteringByColumn="True" EnableLinqExpressions="False" OnItemCreated="RadGrid1_ItemCreated"
        OnNeedDataSource="RadGrid1_NeedDataSource" OnItemCommand="RadGrid1_ItemCommand"
        OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
        <MasterTableView AllowFilteringByColumn="false" Width="100%">
            <SortExpressions>
                <telerik:GridSortExpression FieldName="RoleName" SortOrder="Ascending"></telerik:GridSortExpression>
            </SortExpressions>
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No records were found.
                </div>
            </NoRecordsTemplate>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <Columns>
                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                    HeaderStyle-Width="30px">
                    <ItemTemplate>
                        <asp:Label ID="ItemCountLabel" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Role" UniqueName="RoleName" DataField="RoleName"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RoleName">
                    <ItemTemplate>
                        <%# Eval("RoleName")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Department" UniqueName="DepartmentName" DataField="DepartmentName"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DepartmentName">
                    <ItemTemplate>
                        <%# Eval("DepartmentName")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" AllowFiltering="false">
                    <ItemTemplate>
                        <asp:HyperLink ID="Edit" runat="server">Edit</asp:HyperLink><asp:Label ID="SeperatorLabel" runat="server" Visible="false">&nbsp;|&nbsp;</asp:Label><asp:LinkButton 
                        ID="DeleteLinkButton" runat="server" CommandName="Delete" CommandArgument='<%#Eval("RoleName")%>'
                            OnClientClick='<%# "if(confirm(\"Are you sure you want to delete " + Eval("RoleName") + " role?\") == false) return false;" %>'>
                            Delete
                        </asp:LinkButton><br />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
            <Resizing AllowColumnResize="True" />
        </ClientSettings>
        <FilterMenu OnClientShown="MenuShowing">
        </FilterMenu>
    </telerik:RadGrid>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" ReloadOnShow="true" VisibleStatusbar="false" Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
        <script type="text/javascript">
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(command) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(command);
            }
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

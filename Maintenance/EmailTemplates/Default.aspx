<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_EmailTemplates_Default" Title="DWMS - Email Templates" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
     <table style="width: 100%;">
        <tr>
            <td width="50%">
                <asp:Panel ID="TitlePanel" runat="server" CssClass="title">
                    Email Templates&nbsp;
                </asp:Panel>
            </td>
            <td align="right">
                &nbsp;</td>
        </tr>
    </table>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
        AllowFilteringByColumn="True" EnableLinqExpressions="False"
        OnNeedDataSource="RadGrid1_NeedDataSource" OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" />       
        <MasterTableView>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <SortExpressions>
                <telerik:GridSortExpression FieldName="Id" SortOrder="Ascending"></telerik:GridSortExpression>
            </SortExpressions>
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No email templates were found.
                </div>
            </NoRecordsTemplate>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                    HeaderStyle-Width="40">
                    <ItemTemplate>
                        <asp:Label ID="ItemCountLabel" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Template Description" UniqueName="TemplateDescription" DataField="TemplateDescription"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="TemplateDescription">
                    <ItemTemplate>
                        <a href='<%# "View.aspx?id=" + Eval("Id") %>' title="View template detail">
                            <%# Eval("TemplateDescription")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
            <Resizing AllowColumnResize="True" />
            <ClientEvents OnFilterMenuShowing="filterMenuShowing" />
            <Scrolling SaveScrollPosition="false" />
        </ClientSettings>
        <FilterMenu OnClientShown="MenuShowing">
        </FilterMenu>
    </telerik:RadGrid>
    <uc:FilteringControl ID="FilteringControl" runat="server" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar="false" Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">        
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

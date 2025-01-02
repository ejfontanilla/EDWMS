<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="View.aspx.cs" Inherits="Maintenance_AuditTrail_View" Title="DWMS - Audit Trail Information" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" />
        <span style="position: absolute; padding-left: 10px;">
            <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" InitialDelayTime="0"
                IsSticky="true">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Data/Images/LoadingDots.gif" />
            </telerik:RadAjaxLoadingPanel>
        </span>
    </div>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False" PageSize="20"
        AllowFilteringByColumn="false" EnableLinqExpressions="false" OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" />
        <MasterTableView ClientDataKeyNames="RecordId,TableName,UserId,Operation,OperationId">
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No audit records were found.
                </div>
            </NoRecordsTemplate>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Version" UniqueName="Version">
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Operation" UniqueName="Operation" DataField="Operation"
                    AutoPostBackOnFilter="true" DataType="System.String" />
                <telerik:GridTemplateColumn HeaderText="User" UniqueName="FullName" DataField="FullName"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="FullName">
                    <ItemTemplate>
                        <%--<a href='<%# "../Accounts/View.aspx?id=" + Eval("UserId") %>' title="View user account">
                            <%# Eval("FullName")%></a>--%>
                        <%#Eval("FullName") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Role" UniqueName="UserRole" DataField="UserRole"
                    AutoPostBackOnFilter="true" DataType="System.String" />
                <telerik:GridDateTimeColumn DataField="AuditDate" HeaderText="Date" UniqueName="AuditDate"
                    AllowFiltering="true" DataType="System.DateTime" FilterControlWidth="120px" />
                <telerik:GridBoundColumn HeaderText="IP Address" UniqueName="IP" DataField="IP" AutoPostBackOnFilter="true"
                    DataType="System.String" />
            </Columns>
        </MasterTableView>
        <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
            <Resizing AllowColumnResize="True" />
            <ClientEvents OnRowDblClick="RowDoubleClick" OnFilterMenuShowing="filterMenuShowing" />
            <Scrolling SaveScrollPosition="false" />
        </ClientSettings>
        <FilterMenu OnClientShown="MenuShowing">
        </FilterMenu>
    </telerik:RadGrid>
    <uc:FilteringControl ID="FilteringControl" runat="server" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <asp:Repeater ID="OuterRepeater" runat="server" OnItemDataBound="OuterRepeater_ItemDataBound">
            <ItemTemplate>
                <a name='<%# Eval("OperationId") %>' style="visibility: hidden;">
                    <%# Eval("OperationId")%></a>
                <div class="header">
                    <div class="left">
                        <asp:Label ID="VersionLabel" runat="server" /></div>
                    <div class="right">
                        <!---->
                    </div>
                </div>
                <asp:Panel ID="OperationPanel" runat="server">
                    <asp:Label ID="OperationLabel" runat="server" />
                    <asp:Label ID="SystemInfoLabel" runat="server" />
                </asp:Panel>
                <div class="area">
                    <table>
                        <asp:Repeater ID="InnerRepeater" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="label" valign="top">
                                        <%# Eval("ColumnName")%>
                                    </td>
                                    <td valign="top">
                                        <%# Server.HtmlEncode(Eval("ColumnValue").ToString())%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </div>
                <div class="top5" style="text-align: right;">
                    <a href="#top">Go to Top</a>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <div class="bottom20">
        <!---->
    </div>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">

            function RowDoubleClick(sender, eventArgs) {
                var index = eventArgs.get_itemIndexHierarchical();
                var masterTable = sender.get_masterTableView();

                var operationId = masterTable.get_dataItems()[index].getDataKeyValue("OperationId");

                location.href = "#" + operationId;
            } 
                
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

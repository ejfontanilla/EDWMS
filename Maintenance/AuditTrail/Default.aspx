<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_AuditTrail_Default" Title="DWMS - Audit Trail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td>
                <div class="title">
                    <asp:Label ID="TitleLabel" runat="server" Visible="true" />
                </div>
            </td>
            <td align="right">
                <a href="javascript: ShowWindow('Tables.aspx', 650, 470);"><b>Table Information</b></a>&nbsp;&nbsp;|&nbsp;
                <asp:HyperLink ID="HyperLink1" runat="server" Font-Bold="true" />&nbsp;&nbsp;|&nbsp;
                <asp:LinkButton ID="ExportLinkButton" runat="server" class="link-excel" OnClick="ExportExcel">
               Export to Excel</asp:LinkButton>
            </td>
        </tr>
    </table>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" Skin="Windows7"
        AllowSorting="True" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True"
        PageSize="20" AllowFilteringByColumn="true" EnableLinqExpressions="false" OnItemDataBound="RadGrid1_ItemDataBound"
        OnNeedDataSource="RadGrid1_NeedDataSource" 
        onexcelexportcellformatting="RadGrid1_ExcelExportCellFormatting" 
        onitemcreated="RadGrid1_ItemCreated">
        <PagerStyle Mode="NextPrevAndNumeric" />
        <ExportSettings IgnorePaging="true">
        </ExportSettings>
        <MasterTableView ClientDataKeyNames="RecordId,TableName,UserId,Operation,OperationId">
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No audit records were found.
                </div>
            </NoRecordsTemplate>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <Columns>
                <%--<telerik:GridDateTimeColumn DataField="AuditDateFormatted" HeaderText="Date" UniqueName="AuditDate"
                    AllowFiltering="true" DataType="System.DateTime" />--%>
                <telerik:GridDateTimeColumn DataField="AuditDateTimeFormatted" HeaderText="Date"
                    UniqueName="AuditDate" EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime"
                    DataFormatString="{0:dd MMM yyyy HH:mm}" AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                <DwmsWeb:CustomFilteringColumn HeaderText="Action" UniqueName="Operation" DataTextField="Text"
                    DataValueField="Value" DataField="Operation" DataType="System.String" SortExpression="Operation">
                    <ItemTemplate>
                        <asp:HyperLink ID="OperationHyperLink" runat="server"
                            NavigateUrl='<%# GetAuditDetailUrl(Eval("TableName"), Eval("RecordId"), Eval("OperationId")) %>' 
                            Text='<%# Eval("Operation") %>' ToolTip="View audit detail">
                        </asp:HyperLink>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="Table" UniqueName="TableName" DataTextField="Text"
                    DataValueField="Value" DataField="TableName" DataType="System.String" SortExpression="TableName">
                    <ItemTemplate>
                        <%#Eval("TableName")%>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
                <telerik:GridTemplateColumn HeaderText="User Name" UniqueName="FullName" DataField="FullName"
                    AllowFiltering="true" DataType="System.String" SortExpression="FullName" AutoPostBackOnFilter="true">
                    <ItemTemplate>
                        <%#Eval("FullName")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="User Role" UniqueName="UserRole" DataField="UserRole" 
                    DataType="System.String" SortExpression="UserRole">
                    <ItemTemplate>
                        <%# Eval("UserRole").ToString().Replace("_", " ") %>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
            <Resizing AllowColumnResize="True" />
            <ClientEvents OnRowContextMenu="RowContextMenu" OnRowDblClick="RowDoubleClick" OnFilterMenuShowing="filterMenuShowing" />
            <Scrolling SaveScrollPosition="false" />
        </ClientSettings>
        <FilterMenu OnClientShown="MenuShowing">
        </FilterMenu>
    </telerik:RadGrid>
    <uc:FilteringControl ID="FilteringControl" runat="server" />
    <telerik:RadContextMenu ID="RadMenu1" runat="server" OnClientItemClicked="RowClick">
        <Items>
            <telerik:RadMenuItem Value="ViewRecoredDetail" Text="View Record Detail" PostBack="false">
            </telerik:RadMenuItem>
            <telerik:RadMenuItem Value="ViewUserAccount" Text="View User Account" PostBack="false">
            </telerik:RadMenuItem>
        </Items>
    </telerik:RadContextMenu>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="850px" Height="600px" VisibleStatusbar="False" Modal="true">
    </telerik:RadWindow>
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">

            function RowContextMenu(sender, eventArgs) {
                var evt = eventArgs.get_domEvent();

                if (evt.target.tagName == "INPUT" || evt.target.tagName == "A") {
                    return;
                }

                var index = eventArgs.get_itemIndexHierarchical();
                document.getElementById("radGridClickedRowIndex").value = index;

                var masterTable = sender.get_masterTableView();
                masterTable.selectItem(masterTable.get_dataItems()[index].get_element(), true);

                var menu = $find("<%=RadMenu1.ClientID %>");
                menu.show(evt);

                evt.cancelBubble = true;
                evt.returnValue = false;

                if (evt.stopPropagation) {
                    evt.stopPropagation();
                    evt.preventDefault();
                }
            }

            function RowClick(sender, args) {
                var index = document.getElementById("radGridClickedRowIndex").value;
                var masterTable = $find("<%= RadGrid1.ClientID %>").get_masterTableView();
                var itemValue = args.get_item().get_value();

                if (itemValue == "ViewRecoredDetail") {
                    var tableName = masterTable.get_dataItems()[index].getDataKeyValue("TableName");
                    var recordId = masterTable.get_dataItems()[index].getDataKeyValue("RecordId");

                    if (recordId != '0') {
                        var oWnd = $find("<%=RadWindow1.ClientID%>");
                        oWnd.setUrl('View.aspx?table=' + tableName + "&id=" + recordId);
                        oWnd.show();
                    }
                }
                else if (itemValue == "ViewUserAccount") {
                    var userId = masterTable.get_dataItems()[index].getDataKeyValue("UserId");
                    location.href = '../Accounts/View.aspx?id=' + userId;
                }
            }

            function RowDoubleClick(sender, eventArgs) {
                var index = eventArgs.get_itemIndexHierarchical();
                var masterTable = sender.get_masterTableView();

                var tableName = masterTable.get_dataItems()[index].getDataKeyValue("TableName");
                var recordId = masterTable.get_dataItems()[index].getDataKeyValue("RecordId");
                var operation = masterTable.get_dataItems()[index].getDataKeyValue("Operation");
                var operationId = masterTable.get_dataItems()[index].getDataKeyValue("OperationId");

                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.set_title(tableName);
                oWnd.setUrl('View.aspx?table=' + tableName + "&id=" + recordId + "#" + operationId);
                oWnd.show();
            }

            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }   
        
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

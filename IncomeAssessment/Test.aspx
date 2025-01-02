<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="IncomeAssessment_Test" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml'>
<head>
    <title>ASP.NET Drag-and-drop DataGrid items | RadGrid demo</title>
    <%--<link href="Common/example.css" rel="stylesheet" type="text/css" />--%>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadFormDecorator ID="RadFormDecorator1" DecoratedControls="All" runat="server"
        Skin="Metro" EnableRoundedCorners="false"></telerik:RadFormDecorator>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager runat="server" ID="radAjax" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdPendingOrders">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPendingOrders"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="grdShippedOrders"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="msg"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="grdShippedOrders">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdShippedOrders"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="msg"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="UseDragColumnCheckBox">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdPendingOrders"></telerik:AjaxUpdatedControl>
                    <telerik:AjaxUpdatedControl ControlID="grdShippedOrders"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadScriptBlock runat="server" ID="scriptBlock">
        <script type="text/javascript">
        //<![CDATA[
            function onRowDropping(sender, args) {
                if (sender.get_id() == "<%=grdPendingOrders.ClientID %>") {
                    var node = args.get_destinationHtmlElement();
                    if (!isChildOf('<%=grdShippedOrders.ClientID %>', node) && !isChildOf('<%=grdPendingOrders.ClientID %>', node)) {
                        args.set_cancel(true);
                    }
                }
                else {
                    var node = args.get_destinationHtmlElement();
                    if (!isChildOf('trashCan', node)) {
                        args.set_cancel(true);
                    }
                    else {
                        if (confirm("Are you sure you want to delete this order?"))
                            args.set_destinationHtmlElement($get('trashCan'));
                        else
                            args.set_cancel(true);
                    }
                }
            }

            function isChildOf(parentId, element) {
                while (element) {
                    if (element.id && element.id.indexOf(parentId) > -1) {
                        return true;
                    }
                    element = element.parentNode;
                }
                return false;
            }
            //]]>
        </script>
    </telerik:RadScriptBlock>
    <div class="exWrap">
        <div class="msgTop">
            <asp:CheckBox ID="UseDragColumnCheckBox" runat="server" OnCheckedChanged="UseDragColumnCheckBox_CheckedChanged"
                AutoPostBack="true" Text="Use GridDragDropColumn"></asp:CheckBox>
        </div>
        <p class="howto">
            Select and drag orders from pending to shipped when dispatched<br />
            Reorder pending orders on priority by drag and drop<br />
            Drop a shipped order over the recycle bin to delete it</p>
        <div style="float: left; padding: 0 6px 0 10px">
            <h2 style="color: #9c3608">
                Pending Orders</h2>
            <telerik:RadGrid runat="server" ID="grdPendingOrders" OnNeedDataSource="grdPendingOrders_NeedDataSource"
                AllowPaging="True" Width="350px" OnRowDrop="grdPendingOrders_RowDrop" AllowMultiRowSelection="true"
                PageSize="30">
                <MasterTableView DataKeyNames="OrderId" Width="100%" TableLayout="Fixed">
                    <Columns>
                        <telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="false">
                        </telerik:GridDragDropColumn>
                    </Columns>
                </MasterTableView>
                <ClientSettings AllowRowsDragDrop="True" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                    <ClientEvents OnRowDropping="onRowDropping"></ClientEvents>
                    <Scrolling AllowScroll="true" UseStaticHeaders="true"></Scrolling>
                </ClientSettings>
                <PagerStyle Mode="NumericPages" PageButtonCount="4"></PagerStyle>
            </telerik:RadGrid>
        </div>
        <div style="float: right; padding: 0 10px 0 6px">
            <h2 style="color: #3c8b04">
                Shipped Orders</h2>
            <telerik:RadGrid runat="server" AllowPaging="True" ID="grdShippedOrders" OnNeedDataSource="grdShippedOrders_NeedDataSource"
                Width="350px" OnRowDrop="grdShippedOrders_RowDrop" AllowMultiRowSelection="true">
                <MasterTableView DataKeyNames="OrderId" Width="100%">
                    <Columns>
                        <telerik:GridDragDropColumn HeaderStyle-Width="18px" Visible="false">
                        </telerik:GridDragDropColumn>
                    </Columns>
                    <NoRecordsTemplate>
                        <div style="height: 30px; cursor: pointer;">
                            No items to view</div>
                    </NoRecordsTemplate>
                    <PagerStyle Mode="NumericPages" PageButtonCount="4"></PagerStyle>
                </MasterTableView>
                <ClientSettings AllowRowsDragDrop="True">
                    <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                    <ClientEvents OnRowDropping="onRowDropping"></ClientEvents>
                </ClientSettings>
            </telerik:RadGrid>
        </div>
        <div style="clear: both;">
        </div>
        <div class="exFooter">
            <a id="trashCan" href="#" onclick="return false;">Recycle Bin</a>
            <div class="exMessage" runat="server" id="msg" visible="false" enableviewstate="false">
                Order(s) successfully deleted!
            </div>
        </div>
    </div>
    </form>
</body>
</html>
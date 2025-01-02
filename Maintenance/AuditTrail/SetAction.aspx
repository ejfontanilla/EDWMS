<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SetAction.aspx.cs" Inherits="Maintenance_AuditTrail_SetAction" MasterPageFile="~/Maintenance/Main.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>





<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td>
                <div class="title">
                    <asp:Label ID="TitleLabel" runat="server" Visible="true" />
                </div>
            </td>
            <td align="right">
                <%--<a href="javascript: ShowWindow('Tables.aspx', 650, 470);"><b>Table Information</b></a>&nbsp;&nbsp;|&nbsp;
                <asp:HyperLink ID="HyperLink1" runat="server" Font-Bold="true" />&nbsp;&nbsp;|&nbsp;--%>
                <%--<asp:LinkButton ID="ExportLinkButton" runat="server" class="link-excel" OnClick="ExportExcel">
               Export to Excel</asp:LinkButton>--%>
            </td>
        </tr>
    </table>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
                <telerik:RadGrid ID="RadGridLog" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                    AllowFilteringByColumn="False" EnableLinqExpressions="False" OnNeedDataSource="RadGridLog_NeedDataSource"
                    OnItemDataBound="RadGridLog_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <MasterTableView ClientDataKeyNames="id">
                        <ItemStyle CssClass="pointer" />
                        <AlternatingItemStyle CssClass="pointer" />
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="LogDate" SortOrder="Descending"></telerik:GridSortExpression>
                        </SortExpressions>
                        <NoRecordsTemplate>
                            <div class="wrapper10">
                                No records were found.
                            </div>
                        </NoRecordsTemplate>
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <asp:Label ID="ItemCountLabel" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="User" UniqueName="UserID" DataField="userID"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="UserID" HeaderStyle-Width="170">
                                <ItemTemplate>
                                    <%# Eval("Name")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Date" UniqueName="LogDate" DataField="LogDate"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LogDate" HeaderStyle-Width="140">
                                <ItemTemplate>
                                    <%# Eval("LogDate")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Set" UniqueName="TypeId" DataField="TypeId"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="TypeId" HeaderStyle-Width="140">
                                <ItemTemplate>
                                    <%# Eval("TypeId")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" DataField="Action"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Action">
                                <ItemTemplate>
                                    <%# Eval("Action")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                        <Selecting AllowRowSelect="True" />
                        <Resizing AllowColumnResize="True" />
                        <Scrolling SaveScrollPosition="false" />
                    </ClientSettings>
                    <FilterMenu OnClientShown="MenuShowing">
                    </FilterMenu>
                </telerik:RadGrid>



    
 
    
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
<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_Organization" Title="DWMS - Organization" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Organization
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
            <input type="button" value="New" class="button-small" runat="server" id="NewButton" />
        </telerik:RadCodeBlock>
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Estate Administration & Property Group (EAPG)</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True" Skin="Windows7"
        BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20" AllowFilteringByColumn="True"
        EnableLinqExpressions="False" OnDetailTableDataBind="RadGrid1_DetailTableDataBind"
        OnNeedDataSource="RadGrid1_NeedDataSource">
        <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
        <MasterTableView DataKeyNames="Id,Code" AllowFilteringByColumn="false" Width="100%"
            HierarchyDefaultExpanded="true" HierarchyLoadMode="Client">
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No records were found.
                </div>
            </NoRecordsTemplate>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <DetailTables>
                <telerik:GridTableView Width="100%" runat="server" AllowFilteringByColumn="false"
                    ShowHeader="false" BorderColor="#EEEEEE" ClientDataKeyNames="Id,Code" TableLayout="Auto">
                    <NoRecordsTemplate>
                        <div class="wrapper10">
                            No branches were found.
                        </div>
                    </NoRecordsTemplate>
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Branch Code" UniqueName="Department"
                            DataField="Code" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Code">
                            <ItemTemplate>
                                <%# Eval("Code")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Name" UniqueName="Name" DataField="Name"
                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Name">
                            <ItemTemplate>
                                <%# Eval("Name")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Business Code" UniqueName="BusinessCode" DataField="BusinessCode" AutoPostBackOnFilter="true"
                            DataType="System.String" SortExpression="">
                            <ItemTemplate>
                                <%# Eval("BusinessCode")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Business Code" UniqueName="MailingList" DataField="MailingList" AutoPostBackOnFilter="true"
                            DataType="System.String" SortExpression="">
                            <ItemTemplate>
                                <%# Eval("MailingList")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" AllowFiltering="false" >
                            <ItemTemplate>
                                <a href="javascript:ShowWindow('EditSection.aspx?Id=<%#Eval("Id")%>', 600, 430);"
                                    title="Edit">Edit</a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Code" UniqueName="Division" DataField="Code"
                    AutoPostBackOnFilter="true" DataType="System.String">
                    <ItemTemplate>
                        <b>
                            <%# Eval("Code")%></b>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Name" UniqueName="Name" DataField="Name"
                    AutoPostBackOnFilter="true" DataType="System.String">
                    <ItemTemplate>
                        <b><%# Eval("Name")%></b>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Business Code" UniqueName="" DataField=""
                    AutoPostBackOnFilter="true" DataType="System.String">
                    <ItemTemplate>
                        &nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Mailing List" UniqueName="MailingList" DataField="MailingList"
                    AutoPostBackOnFilter="true" DataType="System.String">
                    <ItemTemplate>
                        <%# Eval("MailingList")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" AllowFiltering="false" >
                    <ItemTemplate>
                        <a href="javascript:ShowWindow('EditDepartment.aspx?id=<%#Eval("Id")%>', 600, 400);" title="Edit">Edit</a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
            <Resizing AllowColumnResize="True" />
            <Scrolling SaveScrollPosition="false" />
        </ClientSettings>
</telerik:RadGrid>

    </asp:Panel>
    <uc:FilteringControl ID="FilteringControl" runat="server" />
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
        Width="600px" Height="510px" VisibleStatusbar="false" Modal="True">
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

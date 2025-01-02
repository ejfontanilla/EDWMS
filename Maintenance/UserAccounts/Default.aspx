<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_UserAccounts_Default" Title="DWMS - User Accounts" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        User Accounts
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
            <input type="button" value="New" class="button-small" runat="server" id="NewButton" />
        </telerik:RadCodeBlock>
    </div>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
        AllowFilteringByColumn="True" EnableLinqExpressions="False" OnItemCreated="RadGrid1_ItemCreated"
        OnNeedDataSource="RadGrid1_NeedDataSource" OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" />
       
        <MasterTableView ClientDataKeyNames="UserName, UserId, RoleName, DepartmentName">
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <SortExpressions>
                <telerik:GridSortExpression FieldName="Name" SortOrder="Ascending"></telerik:GridSortExpression>
            </SortExpressions>
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No records were found.
                </div>
            </NoRecordsTemplate>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false" HeaderStyle-Width="40">
                    <ItemTemplate>
                        <asp:Label ID="ItemCountLabel" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Name" UniqueName="c" DataField="Name"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Name">
                    <ItemTemplate>
                        <a href='<%# "../UserAccounts/View.aspx?id=" + Eval("UserId") %>' title="View user account detail">
                            <%# Eval("Name")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Designation" UniqueName="Designation" DataField="Designation"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Designation" Visible="false">
                    <ItemTemplate>
                        <%# String.IsNullOrEmpty(Eval("Designation").ToString()) ? "-" : Eval("Designation")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Secondary_Appointment" UniqueName="SecondaryAppointment"
                    DataField="SecondaryAppointment" AutoPostBackOnFilter="true" DataType="System.String"
                    SortExpression="Designation" Visible="false">
                    <ItemTemplate>
                        <%#Eval("Designation")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Email" UniqueName="Email" DataField="Email"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Email" HeaderStyle-Width="180">
                    <ItemTemplate>
                        <%# String.IsNullOrEmpty(Eval("Email").ToString()) ? "-" : Eval("Email")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="Department" UniqueName="DepartmentCode" DataField="DepartmentCode"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DepartmentCode" HeaderStyle-Width="80">
                    <ItemTemplate>
                         <%# Eval("DepartmentCode").ToString().Trim() %> 
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="Section" UniqueName="SectionCode" DataField="SectionCode"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SectionCode" HeaderStyle-Width="80">
                    <ItemTemplate>
                         <%# Eval("SectionCode").ToString().Trim() %>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="Team" UniqueName="Team" DataField="Team" HeaderStyle-Width="160"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Team">
                    <ItemTemplate>
                         <%# Eval("Team").ToString().Trim()%>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>
                <DwmsWeb:CustomFilteringColumn HeaderText="Role" UniqueName="RoleName" DataTextField="Text" HeaderStyle-Width="200"
                    DataValueField="Value" DataField="RoleName" DataType="System.String" SortExpression="RoleName">
                    <ItemTemplate>
                        <%# Eval("RoleName").ToString().Replace("_"," ")%>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>

                <DwmsWeb:CustomFilteringColumn HeaderText="Status" UniqueName="IsApproved" DataTextField="Text"
                    DataValueField="Value" DataField="Text" DataType="System.String" SortExpression="IsApproved" Visible="false" AllowFiltering="false">
                    <ItemTemplate>
                        <%# Eval("IsApproved").ToString().ToLower().Equals("true") ? "Active" : "InActive" %>
                    </ItemTemplate>
                </DwmsWeb:CustomFilteringColumn>

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
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar="false">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript"> 
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

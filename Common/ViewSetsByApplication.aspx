<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewSetsByApplication.aspx.cs" MasterPageFile="~/Blank.master" 
Inherits="Verification_SetList" Title="DWMS - " %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="" />
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False" 
            onitemdatabound="RadGrid1_ItemDataBound" 
            onneeddatasource="RadGrid1_NeedDataSource"
            PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="Id,Status" ClientDataKeyNames="Id,Status">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="VerificationDateIn" SortOrder="Descending"></telerik:GridSortExpression>
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
                            <%--<asp:Label ID="ItemCountLabel" runat="server"></asp:Label>--%>
                            <%# Container.ItemIndex + 1 %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="DateIn"
                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                    <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField="Address"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Address">
                        <ItemTemplate>
                           <%# (String.IsNullOrEmpty(Eval("Address").ToString()) ? "-" : Eval("Address"))%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                        <ItemTemplate>
                            <%# Eval("Status").ToString().Replace("_", " ") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Verification OIC" UniqueName="VerificationOIC"
                        DataField="FullName2" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VerificationOIC">
                        <ItemTemplate>
                            <%# (String.IsNullOrEmpty(Eval("VerificationOIC").ToString()) ? "-" : Eval("VerificationOIC"))%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Set Number" UniqueName="SetNo" DataField="SetNo"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo">
                        <ItemTemplate>
                            <asp:HyperLink ID="ViewSetHyperLink" runat="server" 
                                NavigateUrl='<%# String.Format("~/Verification/View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                <%# Eval("SetNo")%>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="Aging"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Aging">
                        <ItemTemplate>
                            <asp:Label ID="AgingLabel" runat="server"></asp:Label>
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
    </asp:Panel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar="false" Modal="True" ReloadOnShow="true">
    </telerik:RadWindow>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function UpdateParentPage() {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(null);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

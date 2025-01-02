<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewDocument.aspx.cs" MasterPageFile="~/Blank.master" 
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
            
            onneeddatasource="RadGrid1_NeedDataSource"
            PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="Id" ClientDataKeyNames="Id">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="Id" SortOrder="Ascending"></telerik:GridSortExpression>
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
                    <telerik:GridTemplateColumn HeaderText="File" UniqueName="File" DataField="Id"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Id">
                        <ItemTemplate>
                            <%# Eval("DocTypeCode").ToString() + ".pdf - " + Eval("Id").ToString()%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Original CM Document ID" UniqueName="OriginalCmDocumentId" DataField="OriginalCmDocumentId"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="OriginalCmDocumentId">
                        <ItemTemplate>
                            <%# Eval("OriginalCmDocumentId").ToString().Replace("_", " ")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="CM Document ID" UniqueName="CmDocumentId" DataField="CmDocumentId"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CmDocumentId">
                        <ItemTemplate>
                            <%# Eval("CmDocumentId").ToString().Replace("_", " ")%>
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

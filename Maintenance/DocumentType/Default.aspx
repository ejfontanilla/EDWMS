<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_DocumentType_Default" Title="DWMS - Document Types" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Web" TagPrefix="DwmsWeb" %>
<%@ Register TagPrefix="uc" TagName="FilteringControl" Src="~/Controls/FilteringUserControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Document Types
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server" Visible="false">
            <input type="button" value="New" class="button-small" runat="server" id="NewButton" />
        </telerik:RadCodeBlock>
    </div>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
        AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="RadGrid1_NeedDataSource"
        OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" />
        <MasterTableView>
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <SortExpressions>
                <telerik:GridSortExpression FieldName="Code" SortOrder="Ascending"></telerik:GridSortExpression>
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
                <telerik:GridTemplateColumn HeaderText="Document ID (CDB Code)" UniqueName="DocumentID"
                    DataField="DocumentId" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DocumentID"
                    Visible="true">
                    <ItemTemplate>
                        <a href='<%# "View.aspx?code=" + Eval("Code") %>' title="Edit rule detail">
                            <%# Eval("DocumentID")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Document Sub ID (CDB Sub Code)" UniqueName="DocumentSubId"
                    DataField="DocumentSubId" AutoPostBackOnFilter="true" DataType="System.String"
                    SortExpression="DocumentSubId" Visible="true">
                    <ItemTemplate>
                        <a href='<%# "View.aspx?code=" + Eval("Code") %>' title="Edit rule detail">
                            <%# String.IsNullOrEmpty(Eval("DocumentSubId").ToString()) ? "-" : Eval("DocumentSubId")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Document Type" UniqueName="Description" DataField="Description"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Description">
                    <ItemTemplate>
                        <a href='<%# "View.aspx?code=" + Eval("Code") %>' title="Edit rule detail">
                            <%# Eval("Description")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
          <%--      <telerik:GridTemplateColumn HeaderText="AcquireNewSamples" UniqueName="AcquireNewSamples" DataField="AcquireNewSamples"
                    AutoPostBackOnFilter="false" DataType="System.String" SortExpression="AcquireNewSamples" >
                    <ItemTemplate>
                        <a href='<%# "View.aspx?code=" + Eval("Code") %>' title="Edit rule detail">
                            <%# Eval("AcquireNewSamples")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>--%>
                <dwmsweb:customfilteringcolumn headertext="Acquire New Samples" uniquename="AcquireNewSamples"
                    datafield="AcquireNewSamples" autopostbackonfilter="true" datatype="System.String"
                    sortexpression="AcquireNewSamples" headerstyle-width="80">
                    <ItemTemplate>
                         <%# Eval("AcquireNewSamples").ToString().Trim() %> 
                    </ItemTemplate>
                </dwmsweb:customfilteringcolumn>
                <dwmsweb:customfilteringcolumn headertext="Active" uniquename="IsActive"
                    datafield="IsActive" autopostbackonfilter="true" datatype="System.String"
                    sortexpression="AcquireNewSamples" headerstyle-width="80">
                    <ItemTemplate>
                         <%# Eval("IsActive").ToString().Trim() %> 
                    </ItemTemplate>
                </dwmsweb:customfilteringcolumn>
                <telerik:GridTemplateColumn HeaderText="Document Code" UniqueName="Code" DataField="Code"
                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Code" Visible="false">
                    <ItemTemplate>
                        <%# Eval("Code") %>
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

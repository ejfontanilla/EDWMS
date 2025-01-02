<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Search_ArchivalReport_Default"
    MasterPageFile="~/OneColumn.master" Title="DWMS - Archival Report" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Archival Report</div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SearchButton">
        <telerik:RadComboBox ID="DepartmentSectionRadComboBox" DropDownWidth="336px" Width="336px"
            Height="350px" AllowCustomText="true" OnClientDropDownOpened="OnClientDropDownOpenedHandler"
            Text="All Departments" runat="server">
            <ItemTemplate>
                <div onclick="StopPropagation(event)">
                    <telerik:RadTreeView ID="RadTreeView1" runat="server" DataSourceID="ObjectDataSource1"
                        DataTextField="name" DataValueField="id" DataFieldID="id" DataFieldParentID="parentid"
                        OnClientNodeClicking="OnClientNodeClickingHandler">
                        <DataBindings>
                            <telerik:RadTreeNodeBinding Expanded="true" />
                        </DataBindings>
                    </telerik:RadTreeView>
                </div>
            </ItemTemplate>
            <Items>
                <telerik:RadComboBoxItem />
            </Items>
        </telerik:RadComboBox>
        <div class="top5">
            <!---->
        </div>
        <span class="label">Date Archived From:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateFromRadDateTimePicker" runat="server" Skin="Windows7">
        </telerik:RadDateTimePicker>
        &nbsp;&nbsp; <span class="label">Date Archived To:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateToRadDateTimePicker" runat="server" Skin="Windows7">
            <DateInput ID="DateInput1" runat="server">
                <ClientEvents OnValueChanged="inputValueChanged" />
            </DateInput>
        </telerik:RadDateTimePicker>
        &nbsp;
        <asp:Button ID="SearchButton" runat="server" Text="Search" CssClass="button-large right20"
            OnClick="SearchButton_Click" />
        <asp:Button ID="ResetButton" runat="server" Text="Reset" CssClass="button-large right10"
            OnClick="ResetButton_Click" />
        <div class="top5">
            <!---->
        </div>
        &nbsp;&nbsp;
        <div class="top5">
            <!---->
        </div>
        <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="RadGrid1_NeedDataSource"
            OnItemDataBound="RadGrid1_ItemDataBound" OnItemCommand="RadGrid1_ItemCommand"
            PagerStyle-Position="TopAndBottom">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="DateArchived" SortOrder="Descending"></telerik:GridSortExpression>
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
                            <%# Container.ItemIndex + 1 %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Date Archived" UniqueName="DateArchived"
                        DataField="DateArchived" AutoPostBackOnFilter="true" DataType="System.String"
                        SortExpression="DateArchived" HeaderStyle-Width="120">
                        <ItemTemplate>
                            <%# Eval("DateArchived")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="RefNo" UniqueName="RefNo" DataField="RefNo"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo" Visible="true">
                        <ItemTemplate>
                            <%# Eval("RefNo")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Division" UniqueName="Division" DataField="Division"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Division"
                        HeaderStyle-Width="100">
                        <ItemTemplate>
                            <%# Eval("Division")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="PeriodOfMonths" UniqueName="PeriodOfMonths" DataField="PeriodOfMonths"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="PeriodOfMonths"
                        HeaderStyle-Width="200">
                        <ItemTemplate>
                            <%# Eval("PeriodOfMonths")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>                   
                    <telerik:GridTemplateColumn HeaderText="No of Archive" UniqueName="NoOfArchive" DataField="NoOfArchive"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="NoOfArchive"
                        HeaderStyle-Width="200">
                        <ItemTemplate>
                            <%# Eval("NoOfArchive")%>
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
            <telerik:AjaxSetting AjaxControlID="ResetButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ChannelRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="RefNoRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="DateFromRadDateTimePicker" UpdatePanelRenderMode="Inline" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SearchButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
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
            var cancelDropDownClosing = false;

            //-- RadUpload client-side events
            // On remove button click
            function confirmDeletion(radUpload, eventArgs) {
                var fileInputs = radUpload.getFileInputs();

                if (fileInputs.length == 1)
                    eventArgs.set_cancel(true);
            }

            // On delete button click
            function confirmDeletions(radUpload, eventArgs) {
                var fileInputs = radUpload.getFileInputs();
                var inputs = eventArgs.get_fileInputFields();

                if (fileInputs.length == inputs.length) {
                    radUpload.addFileInput();
                }
            }
            //--

            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function StopPropagation(e) {
                //cancel bubbling
                e.cancelBubble = true;
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }

            function OnClientDropDownClosingHandler(sender, e) {
                //do not close the second combo if 
                //a checkbox from the first is clicked
                e.set_cancel(cancelDropDownClosing);
            }

            function OnClientDropDownOpenedHandler(sender, eventArgs) {
                var tree = sender.get_items().getItem(0).findControl("RadTreeView1");
                var selectedNode = tree.get_selectedNode();
                if (selectedNode) {
                    selectedNode.scrollIntoView();
                }
            }

            var currentLoadingPanel = null;
            var currentUpdatedControl = null;
            function RequestStart(sender, args) {
                currentLoadingPanel = $find("LoadingPanel1");
                currentUpdatedControl = "TableLayout";
                currentLoadingPanel.show(currentUpdatedControl);
            }
            function ResponseEnd() {
                //hide the loading panel and clean up the global variables
                if (currentLoadingPanel != null)
                    currentLoadingPanel.hide(currentUpdatedControl);
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }

            function DocAppRadComboBox_TextChange(sender, args) {
                sender.set_text(sender.get_text().toString().trim());
            }

            function ShowDownloadDocWindow(id) {
                ShowWindow("~/Common/DownloadDocuments.aspx?id=" + id, 500, 500);
            }

            function inputValueChanged(sender, args) {
                var newValue = args.get_newValue();
                var dateFormat = sender.get_dateFormat();
                var date = args.get_newDate();

                if (date.getHours() == 0 && date.getMinutes() == 0 && date.getSeconds() == 0) {
                    date.setHours(23);
                    date.setMinutes(59);
                    sender.set_value(date.format(dateFormat));
                }
            }

            function OnClientNodeClickingHandler(sender, e) {
                var node = e.get_node();
                //find the combobox, set its text and close it
                var combo = $find("<%= DepartmentSectionRadComboBox.ClientID %>");
                combo.set_text(node.get_text());
                cancelDropDownClosing = false;
                combo.hideDropDown();
            }
        </script>
    </telerik:RadCodeBlock>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetDepartmentSectionForDropDown"
        TypeName="Dwms.Bll.DepartmentDb"></asp:ObjectDataSource>
</asp:Content>

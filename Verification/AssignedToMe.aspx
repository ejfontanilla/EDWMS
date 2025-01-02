<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="AssignedToMe.aspx.cs" Inherits="Verification_AssignedToMe" Title="DWMS - Assigned To Me (Verification)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Assigned To Me
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <telerik:RadComboBox ID="ChannelRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="120" DropDownWidth="120" AppendDataBoundItems="true" Skin="Windows7"
            ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Channels -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <telerik:RadComboBox ID="StatusRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="150" DropDownWidth="150" AppendDataBoundItems="true" Skin="Windows7"
            ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Status -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
                AutoPostBack="false" CausesValidation="false" OnClientTextChange="DocAppRadComboBox_TextChange"
                EnableAutomaticLoadOnDemand="true" datatextfield="RefNo" MarkFirstMatch="True" datavaluefield="Id" Filter="Contains" Skin="Windows7"
                EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource1" Width="150" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;
        <%--<telerik:RadComboBox runat="server" ID="AcknowledgeNumberRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false" Skin="Windows7"
            EnableAutomaticLoadOnDemand="true" datatextfield="AcknowledgeNumber" MarkFirstMatch="True" datavaluefield="AcknowledgeNumber" Filter="Contains"  
            EmptyMessage="Type a Acknowledge Number..." DataSourceID="GetAcknowledgeNumberObjectDataSource" Width="180" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="AcknowledgeNumberRadTextBox" Width="180" AutoPostBack="false"
            EmptyMessage="Type an Acknowledge Number..." Skin="Windows7">
        </telerik:RadTextBox>&nbsp;    
        <asp:CheckBox runat="server" ID="GroupByDateCheckBox" Text="Group By Date" />
        <div class="top5">
            <!---->
        </div>
        <%--<telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false" Skin="Windows7"
            EnableAutomaticLoadOnDemand="true" DataTextField="Nric" MarkFirstMatch="True" DataValueField="Nric" Filter="Contains"
            EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="120" EnableVirtualScrolling="true" ItemsPerRequest="20"
            OnClientTextChange="DocAppRadComboBox_TextChange">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="NricRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a NRIC..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;
        <span class="label">Date In From:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateInFromRadDateTimePicker" runat="server" Skin="Windows7">
        </telerik:RadDateTimePicker>
        &nbsp;&nbsp; <span class="label">Date In To:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateInToRadDateTimePicker" runat="server" Skin="Windows7">
            <DateInput ID="DateInput1" runat="server">
                <ClientEvents OnValueChanged="inputValueChanged" />
            </DateInput>
        </telerik:RadDateTimePicker>
        &nbsp;&nbsp;
        <asp:Button ID="SearchButton" runat="server" Text="Search"
            CssClass="button-large right10" OnClick="SearchButton_Click" />
        <asp:Button ID="ResetButton" runat="server" Text="Reset"
            CssClass="button-large right10" OnClick="ResetButton_Click" />
        <div class="top5">
            <!---->
        </div>
        <asp:MultiView ID="GridMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="NoGroupingView" runat="server">
                <telerik:RadGrid ID="NoGroupRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None"
                    AllowPaging="True" PageSize="20" AllowFilteringByColumn="True" EnableLinqExpressions="False"
                    OnItemDataBound="NoGroupRadGrid_ItemDataBound" OnNeedDataSource="NoGroupRadGrid_NeedDataSource"
                    PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="Id,Status"
                        ClientDataKeyNames="Id,Status">
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
                                    <%# Container.ItemIndex + 1 %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <%--<telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                            </telerik:GridClientSelectColumn>--%>
                            <%--Commented By Edward 30.10.2013 Optimizing Listings --%>
                            <%--<telerik:GridTemplateColumn HeaderText="Source" UniqueName="Source" DataField="Source"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Source"
                                Visible="false">
                                <ItemTemplate>
                                    <%# Eval("Source")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>--%>
                            <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="DateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridDateTimeColumn DataField="DateAssigned" HeaderText="Date Assigned"
                                UniqueName="DateAssigned" EmptyDataText="-" AllowFiltering="true"
                                DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true"
                                PickerType="DateTimePicker" />
                            <telerik:GridTemplateColumn HeaderText="Ref No (PeOIC/CaseOIC)" UniqueName="" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="AppInformationLabel"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                                <ItemTemplate>
                                    <%# Eval("Status").ToString().Replace("_", " ") %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField="Address"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Address">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("Address").ToString()) ? "-" : Eval("Address"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Import OIC" UniqueName="ImportedOIC" DataField="ImportedOIC"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="ImportedOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("ImportedOIC").ToString()) ? "-" : Eval("ImportedOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <%--<telerik:GridTemplateColumn HeaderText="Verification OIC" UniqueName="VerificationOIC"
                                DataField="FullName2" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VerificationOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("VerificationOIC").ToString()) ? "-" : Eval("VerificationOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>--%>
                            <telerik:GridTemplateColumn HeaderText="Set Number" UniqueName="SetNo" DataField="SetNo"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ViewSetHyperLink" runat="server" NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>'
                                        Target="_blank">
                                <%# Eval("SetNo")%>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Icon" UniqueName="" DataField="" AutoPostBackOnFilter="true" HeaderStyle-Width="50px"
                                DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="VerificationDateIn" HeaderStyle-Width="62px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VerificationDateIn">
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
            </asp:View>
            <asp:View ID="GroupByDateView" runat="server">
                <telerik:RadGrid ID="GroupByDateRadGrid" runat="server" AutoGenerateColumns="False"
                    AllowSorting="True" Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE"
                    GridLines="None" AllowPaging="True" PageSize="20" AllowFilteringByColumn="True"
                    EnableLinqExpressions="False" OnDetailTableDataBind="GroupByDateRadGrid_DetailTableDataBind"
                    OnPreRender="GroupByDateRadGrid_PreRender" OnNeedDataSource="GroupByDateRadGrid_NeedDataSource"
                    OnItemDataBound="GroupByDateRadGrid_ItemDataBound" PagerStyle-Position="TopAndBottom"
                    AllowMultiRowSelection="True">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="VerificationDateIn"
                        ClientDataKeyNames="VerificationDateIn" GroupLoadMode="Client" Name="GroupByDatemasterTable">
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
                            <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="DateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd MMM yyyy}"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                        </Columns>
                        <DetailTables>
                            <telerik:GridTableView Name="GroupByDateDetailTable" Width="100%" runat="server"
                                AllowFilteringByColumn="false" ShowHeader="true" AllowPaging="True" PageSize="20"
                                DataKeyNames="Id" ClientDataKeyNames="Id">
                                <SortExpressions>
                                    <telerik:GridSortExpression FieldName="VerificationDateIn" SortOrder="Descending"></telerik:GridSortExpression>
                                </SortExpressions>
                                <NoRecordsTemplate>
                                    <div class="wrapper10">
                                        No records were found.
                                    </div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                        HeaderStyle-Width="30px">
                                        <ItemTemplate>
                                            <%# Container.ItemIndex + 1 %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <%--<telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                                    </telerik:GridClientSelectColumn>--%>
                                    <telerik:GridTemplateColumn HeaderText="Source" UniqueName="Source" DataField="Source"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Source"
                                        Visible="false">
                                        <ItemTemplate>
                                            <%# Eval("Source")%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="DateIn"
                                        HeaderStyle-Width="110px" EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime"
                                        DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                                    <telerik:GridDateTimeColumn DataField="DateAssigned" HeaderText="Date Assigned"
                                        UniqueName="DateAssigned" HeaderStyle-Width="110px" EmptyDataText="-" AllowFiltering="true"
                                        DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true"
                                        PickerType="DateTimePicker" />
                                    <telerik:GridTemplateColumn HeaderText="Ref No (PeOIC/CaseOIC)" UniqueName="" DataField="" HeaderStyle-Width="150px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="AppInformationLabel"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status" HeaderStyle-Width="150px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                                        <ItemTemplate>
                                            <%# Eval("Status").ToString().Replace("_", " ") %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField="Address"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Address">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("Address").ToString()) ? "-" : Eval("Address"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Import OIC" UniqueName="ImportedOIC" DataField="ImportedOIC" HeaderStyle-Width="150px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="ImportedOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("ImportedOIC").ToString()) ? "-" : Eval("ImportedOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <%-- <telerik:GridTemplateColumn HeaderText="Verification OIC" UniqueName="VerificationOIC"
                                        DataField="FullName2" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VerificationOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("VerificationOIC").ToString()) ? "-" : Eval("VerificationOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>--%>
                                    <telerik:GridTemplateColumn HeaderText="Set Number" UniqueName="SetNo" DataField="SetNo" HeaderStyle-Width="125px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="ViewSetHyperLink" runat="server" NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>'
                                                Target="_blank">
                                <%# Eval("SetNo")%>
                                            </asp:HyperLink>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Icon" UniqueName="" DataField="" AutoPostBackOnFilter="true"
                                        DataType="System.String" SortExpression="">
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="VerificationDateIn"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VerificationDateIn">
                                        <ItemTemplate>
                                            <asp:Label ID="AgingLabel" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </telerik:GridTableView>
                        </DetailTables>
                    </MasterTableView>
                    <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                        <Selecting AllowRowSelect="True" />
                        <Resizing AllowColumnResize="True" />
                        <Scrolling SaveScrollPosition="false" />
                        <ClientEvents OnRowDeselected="OnRowDeselected" OnRowSelected="OnRowSelected" />
                    </ClientSettings>
                    <FilterMenu OnClientShown="MenuShowing">
                    </FilterMenu>
                </telerik:RadGrid>
            </asp:View>
        </asp:MultiView>
    </asp:Panel>
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server" SelectMethod="GetNricForDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
    <%--<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetDocApp"
        TypeName="Dwms.Bll.DocAppDb"></asp:ObjectDataSource>--%>
    <%--<asp:ObjectDataSource ID="GetAcknowledgeNumberObjectDataSource" runat="server" SelectMethod="GetAcknowledgeNumberForDropDown"
        TypeName="Dwms.Bll.DocSetDb"></asp:ObjectDataSource>--%>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
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
            //-- RadUpload client-side events
            // On remove button click
            function onClicking(sender, eventArgs) {
                var masterTable;
                var gridSelectedItems;
                var item = eventArgs.get_item();
                var navigateUrl = item.get_navigateUrl();
                var text = item.get_text();

                var grid = $find('<%= NoGroupRadGrid.ClientID %>');

                if (grid == null) {
                    grid = $find('<%= GroupByDateRadGrid.ClientID %>');
                    masterTable = grid.get_masterTableView();
                    var detailTables = grid.get_detailTables();
                    var detailTableCount = detailTables.length;
                    gridSelectedItems = masterTable.get_selectedItems();

                    //if there are any datatables expanded
                    if (detailTableCount > 0) {
                        var selectedIndeces = "";
                        //loop through all the open tables
                        for (var i = 0; i < detailTableCount; i++) {
                            var items = detailTables[i].get_selectedItems();

                            if (items.length > 0) {
                                for (var j = 0; j < items.length; j++) {
                                    setId = items[j].getDataKeyValue("Id");
                                    selectedIndeces = selectedIndeces + setId + ",";
                                }
                            }
                        }

                        // if atleast one set exixt
                        if (selectedIndeces.lastIndexOf(',')) {
                            selectedIndeces = selectedIndeces.substr(0, selectedIndeces.lastIndexOf(','));
                            if (navigateUrl == "#") {
                                if (text == "Assign") {
                                    ShowWindow('Assign.aspx?id=' + selectedIndeces, 550, 400);
                                }
                                else if (text == "Download") {
                                    if (selectedIndeces.indexOf(',') > -1) {
                                        alert("Please choose only 1 set to download.");
                                    }
                                    else {
                                        //window.location = "../Common/DownloadSetDocument.aspx?id=" + selectedIndeces;
                                        ShowWindow('../Common/ShowSetDocument.aspx?id=' + selectedIndeces, 600, 600);
                                    }
                                }
                                else if (text == "Close") {
                                    //BllFuncWebSvr.UpdateSetStatusBySetIds(selectedIndeces, "Closed" , "Set_closed"  , CallSuccessUpdateSetStatus, CallFailedUpdateSetStatus);
                                    ShowWindow('CloseSet.aspx?id=' + selectedIndeces, 550, 400);
                                }

                                eventArgs.set_cancel(true);
                            }
                        }
                        else { // if no st exist
                            alert("Please select at least 1 set.");
                        }

                    }
                }
                else { // for no group
                    masterTable = grid.get_masterTableView();
                    gridSelectedItems = masterTable.get_selectedItems();

                    //Please select at least 1 set if clause
                    if (gridSelectedItems.length > 0) {
                        var selectedIndeces = "";

                        for (var i = 0; i < gridSelectedItems.length; i++) {
                            var setId = gridSelectedItems[i].getDataKeyValue("Id");
                            var status = gridSelectedItems[i].getDataKeyValue("Status");

                            //if(status != "Verified")
                            selectedIndeces = selectedIndeces + setId + ",";
                        }

                        selectedIndeces = selectedIndeces.substr(0, selectedIndeces.lastIndexOf(','));

                        if (navigateUrl == "#") {
                            if (text == "Assign") {
                                ShowWindow('Assign.aspx?id=' + selectedIndeces, 550, 400);
                            }
                            else if (text == "Download") {
                                if (selectedIndeces.indexOf(',') > -1) {
                                    alert("Please choose only 1 set to download.");
                                }
                                else {
                                    //window.location = "../Common/DownloadSetDocument.aspx?id=" + selectedIndeces;
                                    ShowWindow('../Common/ShowSetDocument.aspx?id=' + selectedIndeces, 600, 600);
                                }
                            }
                            else if (text == "Close") {
                                //BllFuncWebSvr.UpdateSetStatusBySetIds(selectedIndeces, "Closed" , "Set_closed"  , CallSuccessUpdateSetStatus, CallFailedUpdateSetStatus);
                                ShowWindow('CloseSet.aspx?id=' + selectedIndeces, 550, 400);
                            }

                            eventArgs.set_cancel(true);
                        }
                    }
                    else {
                        alert("Please select at least 1 set.");
                    }
                }
            }

            function OnRowSelected(sender, eventArgs) {
                var tableView = eventArgs.get_tableView();
                var tableName = tableView.get_name();

                if (tableName == "GroupByDatemasterTable") {
                    var selectedRow = eventArgs.get_item();
                    var nestedView = selectedRow.get_nestedViews()[0];

                    if (nestedView != null) {
                        nestedView.selectAllItems();
                    }
                }
                else if (tableName == "GroupByDateDetailTable") {
                    var masterTable = tableView.get_parentView();
                    var selectedItemCount = tableView.get_selectedItems().length;
                    var itemCount = tableView.get_dataItems().length;
                    var index = eventArgs.get_itemIndexHierarchical().split(":", 1);
                    var parentItem = masterTable.get_dataItems()[index];
                    parentItem.set_selected(selectedItemCount == itemCount);
                }
            }

            function OnRowDeselected(sender, eventArgs) {
                var tableView = eventArgs.get_tableView();
                var tableName = tableView.get_name();

                if (tableName == "GroupByDatemasterTable") {
                    var selectedRow = eventArgs.get_item();
                    var nestedView = selectedRow.get_nestedViews()[0];

                    if (nestedView != null) {
                        var itemCount = nestedView.get_dataItems().length;
                        var selectedChildItemCount = nestedView.get_selectedItems().length;

                        if (selectedChildItemCount == itemCount) {
                            nestedView.clearSelectedItems();
                        }
                    }
                }
                else if (tableName == "GroupByDateDetailTable") {
                    var masterTable = tableView.get_parentView();
                    var selectedItemCount = tableView.get_selectedItems().length;
                    var itemCount = tableView.get_dataItems().length;
                    var index = eventArgs.get_itemIndexHierarchical().split(":", 1);
                    var parentItem = masterTable.get_dataItems()[index];
                    parentItem.set_selected(selectedItemCount == itemCount);
                }
            }

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

            function DocAppRadComboBox_TextChange(sender, args) {
                sender.set_text(sender.get_text().toString().trim());
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
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

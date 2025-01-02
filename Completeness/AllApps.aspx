﻿<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="AllApps.aspx.cs" Inherits="Completeness_AppList" Title="DWMS - All Applications (Completeness)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        All Applications
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <telerik:RadComboBox ID="StatusRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="150" DropDownWidth="150" AppendDataBoundItems="true" Skin="Windows7"
            ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Status -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;           
            <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true" Skin="Windows7"
                    AutoPostBack="false" CausesValidation="false" OnClientTextChange="DocAppRadComboBox_TextChange"
                    EnableAutomaticLoadOnDemand="true" datatextfield="RefNo" MarkFirstMatch="True" datavaluefield="Id" Filter="Contains"  
                    EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource1" Width="150" EnableVirtualScrolling="true" ItemsPerRequest="20">
            </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>&nbsp;
            <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true"
                AutoPostBack="true" CausesValidation="false" Skin="Windows7"
                EnableAutomaticLoadOnDemand="true" DataTextField="Name" MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
                EmptyMessage="Type a Completeness OIC..." Width="180" EnableVirtualScrolling="true" ItemsPerRequest="20">
            </telerik:RadComboBox>
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
        <%--<telerik:RadComboBox runat="server" ID="GroupRadComboBox" AllowCustomText="false"
                AutoPostBack="false" CausesValidation="false" MarkFirstMatch="True" Width="185">
                <Items>
                    <telerik:RadComboBoxItem Text="No Grouping" Value="0" Selected="true" />
                    <telerik:RadComboBoxItem Text="Group By Date" Value="1" />
                </Items>
            </telerik:RadComboBox>
            <span class="label">Ref/IC Number:&nbsp;</span>&nbsp;
            <asp:TextBox ID="RefNoTextBox" runat="server"></asp:TextBox>--%>
        <div class="top5">
            <!---->
        </div>
        <%--<telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false" Skin="Windows7"
            EnableAutomaticLoadOnDemand="true" DataTextField="Nric" MarkFirstMatch="True" DataValueField="Nric" Filter="Contains"
            EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="150" EnableVirtualScrolling="true" ItemsPerRequest="20"
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

        <telerik:RadMenu ID="RadMenuHomeComplete" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif"
            Font-Size="12px" Style="z-index: 2000" Visible="true" Width="100%" OnClientItemClicking="onClicking">
            <Items>
                <telerik:RadMenuItem Text="Assign" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RightArrowHS.png">
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Download" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/GoToNextMessage.png">
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Close" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RecordHS.png">
                </telerik:RadMenuItem>
            </Items>
        </telerik:RadMenu>
        <div class="top5">
            <!---->
        </div>

        <asp:MultiView ID="GridMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="NoGroupingView" runat="server">
                <telerik:RadGrid ID="NoGroupRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                    AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="NoGroupRadGrid_NeedDataSource"
                    OnItemDataBound="NoGroupRadGrid_ItemDataBound" PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="Id,Status" ClientDataKeyNames="Id,Status">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="DateIn" SortOrder="Descending"></telerik:GridSortExpression>
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
                            <telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                            </telerik:GridClientSelectColumn>
                            <telerik:GridDateTimeColumn DataField="DateIn" HeaderText="Date In" UniqueName="DateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="110px"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridDateTimeColumn DataField="DateOut" HeaderText="Date Out" UniqueName="DateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="110px"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ViewRefHyperLink" runat="server"
                                        NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    <%# Eval("RefNo")%>
                                    </asp:HyperLink>
                                    &nbsp;<asp:LinkButton runat="server" ID="SetCountLinkButton"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="PeOIC" UniqueName="PeOIC" DataField="PeOIC" HeaderStyle-Width="48px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="PeOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("PeOIC").ToString()) ? "-" : Eval("PeOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Case OIC" UniqueName="CaseOIC" DataField="CaseOIC" HeaderStyle-Width="64px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CaseOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("CaseOIC").ToString()) ? "-" : Eval("CaseOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                                <ItemTemplate>
                                    <%# Eval("Status").ToString().Replace("_", " ") %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="AddressLabel"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Completeness OIC" UniqueName="VerificationOIC"
                                DataField="FullName2" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CompletenessOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("CompletenessOIC").ToString()) ? "-" : Eval("CompletenessOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Icon" UniqueName="" DataField="" AutoPostBackOnFilter="true"
                                DataType="System.String" SortExpression="" HeaderStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="SecondCA" Visible="false" />
                                    <asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                                    <asp:Image runat="server" ID="SetsVerifiedIndicatorImage" Visible="false" />
                                    <asp:Image runat="server" ID="FilesIndicatorImage" Visible="false" />
                                    <asp:Image runat="server" ID="SentStatusImage" Visible="false" ImageUrl="~/Data/Images/SentStatus.png" />
                                    <asp:Image runat="server" ID="SentButFailedStatusImage" Visible="false" ImageUrl="~/Data/Images/SentButFailedStatus.png" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="DateIn" HeaderStyle-Width="64px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DateIn">
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
                <telerik:RadGrid ID="GroupByDateRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                    AllowFilteringByColumn="True" EnableLinqExpressions="False"
                    OnDetailTableDataBind="GroupByDateRadGrid_DetailTableDataBind"
                    OnPreRender="GroupByDateRadGrid_PreRender"
                    OnNeedDataSource="GroupByDateRadGrid_NeedDataSource"
                    OnItemDataBound="GroupByDateRadGrid_ItemDataBound"
                    PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="DateIn" ClientDataKeyNames="DateIn" GroupLoadMode="Client" Name="GroupByDatemasterTable">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="DateIn" SortOrder="Descending"></telerik:GridSortExpression>
                        </SortExpressions>
                        <NoRecordsTemplate>
                            <div class="wrapper10">
                                No records were found.
                            </div>
                        </NoRecordsTemplate>
                        <ItemStyle CssClass="pointer" />
                        <AlternatingItemStyle CssClass="pointer" />
                        <Columns>
                            <%--<telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                            HeaderStyle-Width="30px">
                            <ItemTemplate>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>--%>
                            <%--<telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                        </telerik:GridClientSelectColumn>--%>
                            <telerik:GridDateTimeColumn DataField="DateIn" HeaderText="Date In" UniqueName="DateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd MMM yyyy}"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                        </Columns>
                        <DetailTables>
                            <telerik:GridTableView Name="GroupByDateDetailTable" Width="100%" runat="server" AllowFilteringByColumn="false"
                                ShowHeader="true" AllowPaging="True" PageSize="20" DataKeyNames="Id" ClientDataKeyNames="Id">
                                <NoRecordsTemplate>
                                    <div class="wrapper10">
                                        No records were found.
                                    </div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                        HeaderStyle-Width="30px">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="ItemCountLabel" runat="server"></asp:Label>--%>
                                            <%# Container.ItemIndex + 1 %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridDateTimeColumn DataField="DateIn" HeaderText="Date In" UniqueName="DateIn"
                                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="110px"
                                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                                    <telerik:GridDateTimeColumn DataField="DateOut" HeaderText="Date Out" UniqueName="DateIn"
                                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderStyle-Width="110px"
                                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                                    <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="ViewRefHyperLink" runat="server"
                                                NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    <%# Eval("RefNo")%>
                                            </asp:HyperLink>
                                            &nbsp;<asp:LinkButton runat="server" ID="SetCountLinkButton"></asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="PeOIC" UniqueName="PeOIC" DataField="PeOIC" HeaderStyle-Width="48px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="PeOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("PeOIC").ToString()) ? "-" : Eval("PeOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Case OIC" UniqueName="CaseOIC" DataField="CaseOIC" HeaderStyle-Width="64px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CaseOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("CaseOIC").ToString()) ? "-" : Eval("CaseOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                                        <ItemTemplate>
                                            <%# Eval("Status").ToString().Replace("_", " ") %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField=""
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="AddressLabel"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Completeness OIC" UniqueName="VerificationOIC"
                                        DataField="FullName2" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CompletenessOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("CompletenessOIC").ToString()) ? "-" : Eval("CompletenessOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Icon" UniqueName="" DataField="" AutoPostBackOnFilter="true"
                                        DataType="System.String" SortExpression="" HeaderStyle-Width="50px">
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="SecondCA" Visible="false" />
                                            <asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                                            <asp:Image runat="server" ID="SetsVerifiedIndicatorImage" Visible="false" />
                                            <asp:Image runat="server" ID="FilesIndicatorImage" Visible="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="DateIn" HeaderStyle-Width="64px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DateIn">
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

        <telerik:RadMenu ID="RadMenu1" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif"
            Font-Size="12px" Style="z-index: 2000" Visible="true" Width="100%" OnClientItemClicking="onClicking">
            <Items>
                <telerik:RadMenuItem Text="Assign" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RightArrowHS.png">
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Download" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/GoToNextMessage.png">
                </telerik:RadMenuItem>
                <telerik:RadMenuItem Text="Close" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RecordHS.png">
                </telerik:RadMenuItem>
            </Items>
        </telerik:RadMenu>
    </asp:Panel>
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
        SelectMethod="GetNricForCompletenessDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
    <%--SelectMethod="GetDocAppIDOnly" Commented by Edward 08.11.2013--%>
    <%--        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
            SelectMethod="GetDocAppIDOnly"
            TypeName="Dwms.Bll.DocAppDb">
        </asp:ObjectDataSource>   --%>
    <%--        <asp:ObjectDataSource ID="GetAcknowledgeNumberObjectDataSource" runat="server" 
            SelectMethod="GetAcknowledgeNumberForDropDown"
            TypeName="Dwms.Bll.DocSetDb">
        </asp:ObjectDataSource>--%>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest" EnableAJAX="true">
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
                                        ShowWindow('Assign.aspx?id=' + selectedIndeces, 600, 350);
                                    }
                                    else if (text == "Download") {
                                        if (selectedIndeces.indexOf(',') > -1) {
                                            alert("Please choose only 1 set to download.");
                                        }
                                        else {
                                            ShowWindow('../Common/ShowAppDocument.aspx?id=' + selectedIndeces, 600, 700);
                                        }
                                    }
                                    else if (text == "Close") {
                                        ShowWindow('CloseApp.aspx?id=' + selectedIndeces, 600, 300);
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
                                    ShowWindow('Assign.aspx?id=' + selectedIndeces, 600, 350);
                                }
                                else if (text == "Download") {
                                    if (selectedIndeces.indexOf(',') > -1) {
                                        alert("Please choose only 1 application to download.");
                                    }
                                    else {
                                        ShowWindow("../Common/ShowAppDocument.aspx?id=" + selectedIndeces, 600, 700);
                                    }
                                }
                                else if (text == "Close") {
                                    ShowWindow('CloseApp.aspx?id=' + selectedIndeces, 600, 300);
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

                function ShowWindow(url, width, height) {
                    var oWnd = $find("<%=RadWindow1.ClientID%>");
                    oWnd.setUrl(url);
                    oWnd.setSize(width, height);
                    oWnd.center();
                    oWnd.show();
                }

                function UpdateParentPage() {
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest(null);
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
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server"
        SelectMethod="GetUserBySection" OnSelecting="GetUserObjectDataSource_Selecting"
        TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
</asp:Content>

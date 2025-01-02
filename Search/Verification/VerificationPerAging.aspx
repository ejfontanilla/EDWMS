<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VerificationPerAging.aspx.cs"
    Inherits="Search_Verification_VerificationPerAging" MasterPageFile="~/OneColumn.master"
    Title="DWMS - Verification Per Aging Report" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Verification Per Aging</div>
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
        <telerik:RadGrid ID="RadGrid2" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False" OnDetailTableDataBind="RadGrid2_DetailTableDataBind"
            OnNeedDataSource="RadGrid2_NeedDataSource" OnItemDataBound="RadGrid2_ItemDataBound">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView DataKeyNames="AgingNo" AllowFilteringByColumn="false" Width="100%"
                HierarchyDefaultExpanded="false" HierarchyLoadMode="Client">
                <NoRecordsTemplate>
                    <div class="wrapper10">
                        No records were found.
                    </div>
                </NoRecordsTemplate>
                <ItemStyle CssClass="pointer" />
                <AlternatingItemStyle CssClass="pointer" />
                <DetailTables>
                    <telerik:GridTableView Width="100%" runat="server" AllowFilteringByColumn="false"
                        ShowHeader="true" BorderColor="#EEEEEE" ClientDataKeyNames="SetNo" Name="Child">
                        <NoRecordsTemplate>
                            <div class="wrapper10">
                                No branches were found.
                            </div>
                        </NoRecordsTemplate>
                        <Columns>
                            <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="DateIn"
                                HeaderStyle-Width="110px" EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridDateTimeColumn DataField="DateAssigned" HeaderText="DateAssigned" UniqueName="DateAssigned"
                                HeaderStyle-Width="110px" EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridDateTimeColumn DataField="VerificationDateOut" HeaderText="Date Out"
                                UniqueName="DateIn" HeaderStyle-Width="110px" EmptyDataText="-" AllowFiltering="true"
                                DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}" AutoPostBackOnFilter="true"
                                PickerType="DateTimePicker" />
                            <telerik:GridTemplateColumn HeaderText="Verification OIC" UniqueName="VOIC" DataField="VOIC"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="VOIC">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblOIC" Text=""></asp:Label>
                                    <%# Eval("VOIC")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="RefNo" UniqueName="RefNo" DataField="RefNo"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                <ItemTemplate>
                                    <%# Eval("RefNo")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="SetNo" UniqueName="SetNo" DataField="SetNo"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo">
                                <ItemTemplate>
                                    <%# Eval("SetNo")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="Aging"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Aging">
                                <ItemTemplate>                                    
                                    <asp:Label ID="AgingLabel" runat="server"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Aging Range" UniqueName="AgingRange" DataField="AgingRange"
                        AutoPostBackOnFilter="true" DataType="System.String">
                        <ItemTemplate>
                            <b>
                                <asp:Label runat="server" ID="lblMonthYear" Text=""></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="" UniqueName="AgingRange" DataField="AgingRange"
                        AutoPostBackOnFilter="true" DataType="System.String">
                        <ItemTemplate>
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

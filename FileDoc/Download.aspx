<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="Download.aspx.cs" Inherits="Filing" Title="DWMS - Filing" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
        <div class="title">
            Download</div>
        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
            <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true"
                    AutoPostBack="true" CausesValidation="false" 
                    EnableAutomaticLoadOnDemand="true" datatextfield="Name" MarkFirstMatch="True" datavaluefield="UserId" Filter="Contains"  
                    EmptyMessage="Type a Completeness OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
            </telerik:RadComboBox>&nbsp;
            <telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
                    AutoPostBack="false" CausesValidation="false" OnClientTextChange="DocAppRadComboBox_TextChange"
                    EnableAutomaticLoadOnDemand="true" datatextfield="RefNo" MarkFirstMatch="True" datavaluefield="Id" Filter="Contains"  
                    EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource1" Width="185" EnableVirtualScrolling="true" ItemsPerRequest="20">
            </telerik:RadComboBox>&nbsp;
            <telerik:RadComboBox ID="StatusRadComboBox" runat="server" ZIndex="30000" Height="200"
                Width="150" DropDownWidth="150" AppendDataBoundItems="true" 
                ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
                CollapseAnimation-Duration="200">
                <Items>
                    <telerik:RadComboBoxItem Text="- All Status -" Value="-1" Selected="true" />
                </Items>
            </telerik:RadComboBox>&nbsp;
            <telerik:RadComboBox runat="server" ID="DownloadStatusRadComboBox" AllowCustomText="true"  AppendDataBoundItems="true" 
                    AutoPostBack="false" CausesValidation="false" OnSelectedIndexChanged="DownloadStatusRadComboBox_OnSelectedIndexChanged">
                <Items>
                    <telerik:RadComboBoxItem Text="- All Download Status -" Value="-1" Selected="true" />
                </Items>
            </telerik:RadComboBox>&nbsp;
            <%--<span class="label">Ref/IC Number:&nbsp;</span>&nbsp;
            <asp:TextBox ID="RefNoTextBox" runat="server"></asp:TextBox>--%>
        <div class="top5"><!----></div>
            <telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
                AutoPostBack="false" CausesValidation="false"
                EnableAutomaticLoadOnDemand="true" datatextfield="Nric" MarkFirstMatch="True" datavaluefield="Nric" Filter="Contains"  
                EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="150" EnableVirtualScrolling="true" ItemsPerRequest="20"
                OnClientTextChange="DocAppRadComboBox_TextChange">
            </telerik:RadComboBox>&nbsp;
            <span class="label">Date In From:</span>&nbsp;
            <telerik:RadDateTimePicker ID="DateInFromRadDateTimePicker" runat="server" Skin="Windows7">
            </telerik:RadDateTimePicker>
            &nbsp;&nbsp; <span class="label">Date In To:</span>&nbsp;
            <telerik:RadDateTimePicker ID="DateInToRadDateTimePicker" runat="server" Skin="Windows7">
            </telerik:RadDateTimePicker>
            &nbsp;&nbsp;
            <asp:Button ID="SearchButton" runat="server" Text="Search" 
                CssClass="button-large right10" onclick="SearchButton_Click" />
            <asp:Button ID="ResetButton" runat="server" Text="Reset" 
                CssClass="button-large right10" onclick="ResetButton_Click" />
        <div class="top5"><!----></div>
            <telerik:RadMenu ID="RadMenuHomeComplete" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif"
                Font-Size="12px" Style="z-index: 2000" Visible="true" Width="100%" OnClientItemClicking="onClicking">
                <Items>
                    <telerik:RadMenuItem Text="Download" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/GoToNextMessage.png">
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>
        <div class="top5"><!----></div>
            <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="RadGrid1_NeedDataSource"
                OnItemDataBound="RadGrid1_ItemDataBound" PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="True">
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
                        <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                            <ItemTemplate>
                                <asp:HyperLink ID="ViewRefHyperLink" runat="server" 
                                    NavigateUrl='<%# String.Format("../Completeness/View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    <%# Eval("RefNo")%>
                                </asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField=""
                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="StatusLabel"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Status Date" UniqueName="StatusDate" DataField=""
                            AutoPostBackOnFilter="true" DataType="System.DateTime" SortExpression="">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="StatusDateLabel"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Download Status" UniqueName="DownloadStatus" DataField="DownloadStatus"
                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DownloadStatus">
                            <ItemTemplate>
                                    <%# Eval("DownloadStatus").ToString().Replace("_"," ")%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridDateTimeColumn DataField="DownloadedOn" HeaderText="Closed On" UniqueName="DownloadedOn"
                            EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                            AutoPostBackOnFilter="true" PickerType="DateTimePicker"/>
                        <telerik:GridTemplateColumn HeaderText="Closed By" UniqueName="ClosedBy" DataField=""
                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="ClosedByLabel"></asp:Label>
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
            <telerik:RadMenu ID="RadMenu1" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif"
                Font-Size="12px" Style="z-index: 2000" Visible="true" Width="100%" OnClientItemClicking="onClicking">
                <Items>
                    <telerik:RadMenuItem Text="Download" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/GoToNextMessage.png">
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>
        </asp:Panel>
        <asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
            SelectMethod="GetNricForCompletenessDropDownList"
            TypeName="Dwms.Bll.CustomPersonal">
        </asp:ObjectDataSource>
 <%--SelectMethod="GetDocApp" Commented by Edward 08.11.2013--%>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
            SelectMethod="GetDocAppIDOnly"
            TypeName="Dwms.Bll.DocAppDb">
        </asp:ObjectDataSource>    
        <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
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
                    var grid = $find('<%=RadGrid1.ClientID %>');
                    var masterTable = grid.get_masterTableView();

                    var item = eventArgs.get_item();
                    var navigateUrl = item.get_navigateUrl();
                    var text = item.get_text();

                    var gridSelectedItems = masterTable.get_selectedItems();

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
                                ShowWindow('Assign.aspx?id=' + selectedIndeces, 520, 400);
                            } else if (text == "Download") {
                                if (selectedIndeces.indexOf(',') > -1) {
                                    alert("Please choose only 1 application to download.");
                                }
                                else {
                                    ShowWindow("../Common/ShowAppDocument.aspx?id=" + selectedIndeces, 600, 600);
                                }
                            }

                            eventArgs.set_cancel(true);
                        }
                    }
                    else {
                        alert("Please select at least 1 reference number.");
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
            </script>
        </telerik:RadCodeBlock>
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server"
        SelectMethod="GetUserBySection" OnSelecting="GetUserObjectDataSource_Selecting"
        TypeName="Dwms.Bll.userDb">
    </asp:ObjectDataSource>
</asp:Content>

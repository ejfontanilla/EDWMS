<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Import_SearchSets_Default" Title="DWMS - Search Sets" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Search Documents for Own Department Sets
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SearchButton">
        <telerik:RadComboBox ID="ChannelRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="150" DropDownWidth="150" AppendDataBoundItems="true"
            ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Channels -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <telerik:RadComboBox ID="StatusRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="180" DropDownWidth="180" AppendDataBoundItems="true"
            ExpandAnimation-Duration="100" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Status -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" datatextfield="RefNo" MarkFirstMatch="True" datavaluefield="Id" Filter="Contains"  
            EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource2" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20"
            OnClientTextChange="DocAppRadComboBox_TextChange">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>&nbsp;
        <%--<telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Nric" MarkFirstMatch="True" DataValueField="Nric" Filter="Contains"
            EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20"
            OnClientTextChange="DocAppRadComboBox_TextChange">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="NricRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a NRIC..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;
        <telerik:RadComboBox runat="server" ID="CmDocumentIdComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="CmDocumentId" MarkFirstMatch="True" DataValueField="CmDocumentId" Filter="Contains"
            EmptyMessage="Type a CM Document ID..." DataSourceID="GetCmDocumentIdObjectDataSource" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>
        &nbsp;&nbsp;
        <div class="top5">
            <!---->
        </div>
        <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true"
            AutoPostBack="true" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Name" MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
            EmptyMessage="Type a Imported OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>
        &nbsp;&nbsp;
        <telerik:RadComboBox ID="VerificationOICRadComboBox" runat="server" AllowCustomText="true"
            AutoPostBack="true" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Name" MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
            EmptyMessage="Type a Verification OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>
        &nbsp;&nbsp;
        <%--<telerik:RadComboBox runat="server" ID="AcknowledgeNumberRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" datatextfield="AcknowledgeNumber" MarkFirstMatch="True" datavaluefield="AcknowledgeNumber" Filter="Contains"  
            EmptyMessage="Type a Acknowledge Number..." DataSourceID="GetAcknowledgeNumberObjectDataSource" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="AcknowledgeNumberRadTextBox" Width="180" AutoPostBack="false"
            EmptyMessage="Type an Acknowledge Number..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;&nbsp;
        <asp:CheckBox runat="server" ID="IsUrgentCheckBox" Text="Is Urgent" />
        &nbsp;
        <asp:CheckBox runat="server" ID="SkipCategorizationCheckBox" Text="Skip Categorization" />
        <div class="top5">
            <!---->
        </div>
        <%--<span class="label">Ref Number:&nbsp;</span><asp:TextBox ID="RefNoTextBox" runat="server" Columns="15"></asp:TextBox>--%>
        <%--&nbsp;&nbsp; --%>
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
            CssClass="button-large right20" OnClick="SearchButton_Click" />
        <asp:Button ID="ResetButton" runat="server" Text="Reset"
            CssClass="button-large right10" OnClick="ResetButton_Click" />

        <div class="top5">
            <!---->
        </div>
        <telerik:RadMenu ID="RadMenuHomeComplete" runat="server" Skin="Sitefinity" Font-Names="Arial, Helvetica, sans-serif"
            Font-Size="12px" Style="z-index: 1;" Visible="true" Width="100%" OnClientItemClicking="onClicking" EnableOverlay="False">
            <Items>
                <telerik:RadMenuItem Text="Set Urgency" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RightArrowHS.png">
                </telerik:RadMenuItem>
            </Items>
        </telerik:RadMenu>
        <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="RadGrid1_NeedDataSource"
            OnItemDataBound="RadGrid1_ItemDataBound"
            OnItemCommand="RadGrid1_ItemCommand"
            PagerStyle-Position="TopAndBottom" AllowMultiRowSelection="true">
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
                    <telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                    </telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn HeaderText="Set Number" UniqueName="SetNo" DataField="SetNo"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo" HeaderStyle-Width="140px">
                        <ItemTemplate>
                            <asp:HyperLink ID="ViewSetHyperLink" runat="server"
                                NavigateUrl='<%# (!Eval("Status").ToString().Equals("Pending_Categorization") && !Eval("Status").ToString().Equals("Categorization_Failed")? String.Format("../../Verification/View.aspx?id={0}", Eval("Id")) : "#") %>' Target="_blank">
                                <%# Eval("SetNo")%>
                            </asp:HyperLink>
                            <asp:Image ID="UrgentImage" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn DataField="VerificationDateIn" HeaderText="Date In" UniqueName="VerificationDateIn" HeaderStyle-Width="130px"
                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                    <telerik:GridTemplateColumn HeaderText="Channel" UniqueName="Source" DataField="Source" HeaderStyle-Width="130px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Source">
                        <ItemTemplate>
                            <%# Eval("Source")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Ref No (PeOIC/CaseOIC)" UniqueName="" DataField=""
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="" HeaderStyle-Width="160px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="AppInformationLabel"></asp:Label>&nbsp;<asp:ImageButton runat="server" ID="DocumentImageButton" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%--                    <telerik:GridTemplateColumn HeaderText="Department" UniqueName="DepartmentName" DataField="DepartmentName"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DepartmentName">
                        <ItemTemplate>
                            <%# Eval("DepartmentCode").ToString().Replace("_", " ")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Section" UniqueName="Name" DataField="Name"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Name">
                        <ItemTemplate>
                            <%# Eval("Code").ToString().Replace("_", " ")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>--%>
                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                        <ItemTemplate>
                            <%# Eval("Status").ToString().Replace("_", " ")%>
                            &nbsp;<asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Waiting" UniqueName="WaitingTime" DataField="" HeaderStyle-Width="100px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                        <ItemTemplate>
                            <asp:Label ID="WaitingTimeLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Processing" UniqueName="ProcessingTime" DataField="" HeaderStyle-Width="100px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                        <ItemTemplate>
                            <asp:Label ID="ProcessingTimeLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false" HeaderStyle-Width="220px">
                        <ItemTemplate>
                            <asp:Panel ID="OptionsPanel" runat="server">
                                <asp:HyperLink ID="DownloadHyperLink" runat="server" CssClass="hand" NavigateUrl="#"
                                    onclick='<%# String.Format("ShowWindow(\"../../Common/ShowSetDocument.aspx?id={0}\", 600, 600)", Eval("Id")) %>'>
                                    Download
                                </asp:HyperLink>
                                <asp:Label runat="server" ID="DeleteSeperatorLabel" Text="&nbsp;&nbsp;&nbsp;"></asp:Label>
                                <asp:LinkButton ID="DeleteLinkButton" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick='<%# String.Format("if(confirm(\"Are you sure you want to delete this set ({0})?\") == false) return false;", Eval("Id")) %>'>
                                    Delete
                                </asp:LinkButton>&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="RecategorizeLinkButton" runat="server" CommandName="Recategorize" CommandArgument='<%# Eval("Id") %>'
                                    OnClientClick='<%# String.Format("if(confirm(\"Are you sure you want to recategorize this set ({0})?\") == false) return false;", Eval("Id")) %>'>
                                    Recategorize
                                </asp:LinkButton>
                            </asp:Panel>
                            <asp:Label ID="NaLabel" runat="server" Text="NA" Visible="false"></asp:Label>
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
            Font-Size="12px" Style="z-index: 1;" Visible="true" Width="100%" OnClientItemClicking="onClicking" EnableOverlay="False">
            <Items>
                <telerik:RadMenuItem Text="Set Urgency" NavigateUrl="#" Font-Bold="true" ImageUrl="~/Data/Images/Action/RightArrowHS.png">
                </telerik:RadMenuItem>
            </Items>
        </telerik:RadMenu>
    </asp:Panel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ResetButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ChannelRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="StatusRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="RefNoRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="NricRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="UserRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="VerificationOICRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="DateInFromRadDateTimePicker" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="DateInToRadDateTimePicker" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="IsUrgentCheckBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="SkipCategorizationCheckBox" UpdatePanelRenderMode="Inline" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SearchButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
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
            function onClicking(sender, eventArgs) {
                var masterTable;
                var gridSelectedItems;
                var item = eventArgs.get_item();
                var navigateUrl = item.get_navigateUrl();
                var text = item.get_text();

                var grid = $find('<%=  RadGrid1.ClientID %>');

                masterTable = grid.get_masterTableView();
                gridSelectedItems = masterTable.get_selectedItems();

                //Please select at least 1 set if clause
                if (gridSelectedItems.length > 0) {
                    var selectedIndeces = "";

                    for (var i = 0; i < gridSelectedItems.length; i++) {
                        var setId = gridSelectedItems[i].getDataKeyValue("Id");
                        var status = gridSelectedItems[i].getDataKeyValue("Status");

                        if (status == "Pending_Categorization")
                            selectedIndeces = selectedIndeces + setId + ",";
                    }

                    if (selectedIndeces.length != 0) {

                        selectedIndeces = selectedIndeces.substr(0, selectedIndeces.lastIndexOf(','));

                        if (navigateUrl == "#") {
                            if (text == "Set Urgency") {
                                ShowWindow('../IndicateUrgency.aspx?id=' + selectedIndeces, 550, 500);
                            }
                            eventArgs.set_cancel(true);
                        }
                    }
                    else {
                        alert("Please select at least 1 set.");
                    }
                }
                else {
                    alert("Please select at least 1 set.");
                }

            }
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

            function UpdateParentPage(command) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(command);
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
        </script>

    </telerik:RadCodeBlock>
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
        SelectMethod="GetNricForDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
        SelectMethod="GetDepartmentSectionForDropDown"
        TypeName="Dwms.Bll.DepartmentDb"></asp:ObjectDataSource>
    <%--SelectMethod="GetDocApp" Commented by Edward 08.11.2013--%>
    <%--<asp:ObjectDataSource ID="ObjectDataSource2" runat="server"
        SelectMethod="GetDocAppIDOnly"
        TypeName="Dwms.Bll.DocAppDb">
    </asp:ObjectDataSource>--%>
    <asp:ObjectDataSource ID="GetUserByDepartmentObjectDataSource" runat="server" OnSelecting="GetUserByDepartmentObjectDataSource_Selecting"
        SelectMethod="GetUserByDepartment"
        TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetAcknowledgeNumberObjectDataSource" runat="server"
        SelectMethod="GetAcknowledgeNumberForDropDown"
        TypeName="Dwms.Bll.DocSetDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetCmDocumentIdObjectDataSource" runat="server"
        SelectMethod="GetCmDocumentIdForDropDown"
        TypeName="Dwms.Bll.DocDb"></asp:ObjectDataSource>
</asp:Content>

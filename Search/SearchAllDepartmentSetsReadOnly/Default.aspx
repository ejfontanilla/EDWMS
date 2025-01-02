<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Import_SearchSetsReadOnly_Default" Title="DWMS - Search Sets Read Only" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Search Documents for All Department Sets (Read Only)
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
            EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource2" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>&nbsp;
        <%--<telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Nric" MarkFirstMatch="True" DataValueField="Nric" Filter="Contains"
            EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
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
        &nbsp;
        <asp:CheckBox runat="server" ID="IsUrgentCheckBox" Text="Is Urgent" />
        <div class="top5">
            <!---->
        </div>

        <table>
            <tr>
                <td style="width: 342px;">
                    <telerik:RadComboBox ID="DepartmentSectionRadComboBox" DropDownWidth="336px" Width="336px" Height="350px" AllowCustomText="true"
                        OnClientDropDownOpened="OnClientDropDownOpenedHandler" Text="All Departments"
                        runat="server">
                        <ItemTemplate>
                            <div onclick="StopPropagation(event)">
                                <telerik:RadTreeView ID="RadTreeView1" runat="server" DataSourceID="ObjectDataSource1"
                                    DataTextField="name" DataValueField="id" DataFieldID="id" DataFieldParentID="parentid"
                                    OnClientNodeClicking="OnClientNodeClickingHandler" OnNodeClick="RadTreeView1_OnTextChanged">
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
                </td>
                <td style="width: 216px;">
                    <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="true"
                        AutoPostBack="true" CausesValidation="false"
                        EnableAutomaticLoadOnDemand="true" DataTextField="Name" MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
                        EmptyMessage="Type a Imported OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
                    </telerik:RadComboBox>
                </td>
                <td>
                    <telerik:RadComboBox ID="VerificationOICRadComboBox" runat="server" AllowCustomText="true"
                        AutoPostBack="true" CausesValidation="false"
                        EnableAutomaticLoadOnDemand="true" DataTextField="Name" MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
                        EmptyMessage="Type a Verification OIC..." Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
                    </telerik:RadComboBox>
                    &nbsp;&nbsp;
                </td>
                <td>
                    <telerik:RadComboBox runat="server" ID="AcknowledgeNumberRadComboBox" AllowCustomText="true"
                        AutoPostBack="false" CausesValidation="false"
                        EnableAutomaticLoadOnDemand="true" DataTextField="AcknowledgeNumber" MarkFirstMatch="True" DataValueField="AcknowledgeNumber" Filter="Contains"
                        EmptyMessage="Type a Acknowledge Number..." DataSourceID="GetAcknowledgeNumberObjectDataSource" Width="210" EnableVirtualScrolling="true" ItemsPerRequest="20">
                    </telerik:RadComboBox>
                    &nbsp;
                <asp:CheckBox runat="server" ID="SkipCategorizationCheckBox" Text="Skip Categorization" />
                </td>

            </tr>
        </table>
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

        <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="RadGrid1_NeedDataSource"
            OnItemDataBound="RadGrid1_ItemDataBound"
            OnItemCommand="RadGrid1_ItemCommand"
            PagerStyle-Position="TopAndBottom">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%">
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
                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                        <ItemTemplate>
                            <%# Eval("Status").ToString().Replace("_", " ")%>
                            &nbsp;<asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Department" UniqueName="DepartmentCode" DataField="DepartmentCode" HeaderStyle-Width="100px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DepartmentCode">
                        <ItemTemplate>
                            <%# Eval("DepartmentCode").ToString().Replace("_", " ")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Section" UniqueName="Code" DataField="Code" HeaderStyle-Width="100px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Code">
                        <ItemTemplate>
                            <%# Eval("Code").ToString().Replace("_", " ")%>
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
                    <telerik:AjaxUpdatedControl ControlID="StatusRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="RefNoRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="NricRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="DepartmentSectionRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="UserRadComboBox" />
                    <telerik:AjaxUpdatedControl ControlID="VerificationOICRadComboBox" />
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
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DepartmentSectionRadComboBox">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="UserRadComboBox" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="VerificationOICRadComboBox" UpdatePanelRenderMode="Inline" />
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

            function OnClientNodeClickingHandler(sender, e) {
                var node = e.get_node();
                //find the combobox, set its text and close it
                var combo = $find("<%= DepartmentSectionRadComboBox.ClientID %>");
                combo.set_text(node.get_text());
                cancelDropDownClosing = false;
                combo.hideDropDown();
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
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server"
        SelectMethod="GetUser"
        TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetUserByDepartmentObjectDataSource" runat="server" OnSelecting="GetUserByDepartmentObjectDataSource_Selecting"
        SelectMethod="GetUserByDepartment"
        TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetUserBySectionAndDepartmentObjectDataSource" runat="server" OnSelecting="GetUserBySectionAndDepartmentObjectDataSource_Selecting"
        SelectMethod="GetUserBySectionAndDepartment"
        TypeName="Dwms.Bll.userDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetAcknowledgeNumberObjectDataSource" runat="server"
        SelectMethod="GetAcknowledgeNumberForDropDown"
        TypeName="Dwms.Bll.DocSetDb"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetCmDocumentIdObjectDataSource" runat="server"
        SelectMethod="GetCmDocumentIdForDropDown"
        TypeName="Dwms.Bll.DocDb"></asp:ObjectDataSource>

</asp:Content>

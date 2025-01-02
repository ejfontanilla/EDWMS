<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="ImportedByMe.aspx.cs" Inherits="Verification_ImportedByMe" Title="DWMS - Imported By Me (Verification)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Imported By Me
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SearchButton">
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
        </telerik:RadTextBox>&nbsp;
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
        <asp:CheckBox runat="server" ID="IsUrgentCheckBox" Text="Urgent" />
        &nbsp;
        <asp:CheckBox runat="server" ID="SkipCategorizationCheckBox" Text="Skip Categorization" />
        <div class="top5">
            <!---->
        </div>
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
            PagerStyle-Position="TopAndBottom">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="ImportedOn" SortOrder="Descending"></telerik:GridSortExpression>
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
                    <telerik:GridDateTimeColumn DataField="ImportedOn" HeaderText="Imported On" UniqueName="ImportedOn" HeaderStyle-Width="110px"
                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                    <telerik:GridTemplateColumn HeaderText="Channel" UniqueName="Source" DataField="Source"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Source">
                        <ItemTemplate>
                            <%# Eval("Source")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Ref No (PeOIC/CaseOIC)" UniqueName="" DataField=""
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="" HeaderStyle-Width="160px">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="AppInformationLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status">
                        <ItemTemplate>
                            <%# Eval("Status").ToString().Replace("_", " ")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField="Address"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Address">
                        <ItemTemplate>
                            <%# (String.IsNullOrEmpty(Eval("Address").ToString()) ? "-" : Eval("Address"))%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Set Number" UniqueName="SetNo" DataField="SetNo" HeaderStyle-Width="125px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="SetNo">
                        <ItemTemplate>
                            <asp:HyperLink ID="ViewSetHyperLink" runat="server"
                                NavigateUrl='<%# (!Eval("Status").ToString().Equals("Pending_Categorization") ? String.Format("View.aspx?id={0}", Eval("Id")) : "#") %>' Text='<%# Eval("SetNo")%>' Target="_blank">
                            </asp:HyperLink>
                            <asp:Image ID="UrgentImage" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false" HeaderStyle-Width="70px">
                        <ItemTemplate>
                            <asp:HyperLink ID="VerifyHyperLink" runat="server"
                                NavigateUrl='<%# String.Format("AssignUser.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                Verify
                            </asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Icon" UniqueName="" DataField="" AutoPostBackOnFilter="true"
                        DataType="System.String" SortExpression="" HeaderStyle-Width="36px">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="NoPendingDoc" Visible="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Waiting" UniqueName="WaitingTime" DataField="" HeaderStyle-Width="82px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                        <ItemTemplate>
                            <asp:Label ID="WaitingTimeLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Processing" UniqueName="ProcessingTime" DataField="" HeaderStyle-Width="82px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                        <ItemTemplate>
                            <asp:Label ID="ProcessingTimeLabel" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Section" UniqueName="Source" DataField="Source" HeaderStyle-Width="50px"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Source">
                        <ItemTemplate>
                            <%# Eval("Code")%>
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
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
        SelectMethod="GetNricForDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
    <%--<asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
        SelectMethod="GetDocApp"
        TypeName="Dwms.Bll.DocAppDb">
    </asp:ObjectDataSource> --%>
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
    <%--<asp:ObjectDataSource ID="GetAcknowledgeNumberObjectDataSource" runat="server" 
        SelectMethod="GetAcknowledgeNumberForDropDown"
        TypeName="Dwms.Bll.DocSetDb">
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetCmDocumentIdObjectDataSource" runat="server" 
        SelectMethod="GetCmDocumentIdForDropDown"
        TypeName="Dwms.Bll.DocDb">
    </asp:ObjectDataSource> --%>
</asp:Content>

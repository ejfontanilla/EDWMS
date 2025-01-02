<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="PendingAssignment.aspx.cs" Inherits="IncomeAssessment_PendingAssignment" Title="DWMS - Pending Assignment (Income Assessment)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Pending Assignment
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
                AutoPostBack="false" CausesValidation="false" OnClientTextChange="DocAppRadComboBox_TextChange"
                EnableAutomaticLoadOnDemand="true" datatextfield="RefNo" MarkFirstMatch="True" datavaluefield="Id" Filter="Contains"  
                EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource1" Width="185" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>&nbsp;
        <%--<telerik:RadComboBox runat="server" ID="NricRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Nric" MarkFirstMatch="True" DataValueField="Nric" Filter="Contains"
            EmptyMessage="Type a NRIC No..." DataSourceID="GetNricObjectDataSource" Width="150" EnableVirtualScrolling="true" ItemsPerRequest="20"
            OnClientTextChange="DocAppRadComboBox_TextChange">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="NricRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a NRIC..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;
        <asp:CheckBox runat="server" ID="GroupByDateCheckBox" Text="Group By Date" />
        <%--<telerik:RadComboBox runat="server" ID="GroupRadComboBox" AllowCustomText="false"
            AutoPostBack="false" CausesValidation="false" MarkFirstMatch="True" Width="185">
            <Items>
                <telerik:RadComboBoxItem Text="No Grouping" Value="0" Selected="true" />
                <telerik:RadComboBoxItem Text="Group By Date" Value="1" />
            </Items>
        </telerik:RadComboBox>
        <span class="label">Ref/IC Number:</span>&nbsp;
        <asp:TextBox ID="RefNoTextBox" runat="server" Columns="9" MaxLength="9"></asp:TextBox>--%>
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
            CssClass="button-large right10" OnClick="SearchButton_Click" />
        <asp:Button ID="ResetButton" runat="server" Text="Reset"
            CssClass="button-large right10" OnClick="ResetButton_Click" />
        <div class="top5">
            <!---->
        </div>
        <asp:MultiView ID="GridMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="NoGroupingView" runat="server">
                <telerik:RadGrid ID="NoGroupRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                    AllowFilteringByColumn="True" EnableLinqExpressions="False" OnNeedDataSource="NoGroupRadGrid_NeedDataSource"
                    PagerStyle-Position="TopAndBottom" OnItemDataBound="NoGroupRadGrid_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="AssessmentDateIn" SortOrder="Descending"></telerik:GridSortExpression>
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
                            <telerik:GridDateTimeColumn DataField="AssessmentDateIn" HeaderText="Date In" UniqueName="AssessmentDateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status"
                                Visible="false">
                                <ItemTemplate>
                                    <%# Eval("Status").ToString().Replace("_", " ") %>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="PeOIC" UniqueName="PeOIC" DataField="PeOIC" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="PeOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("PeOIC").ToString()) ? "-" : Eval("PeOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Case OIC" UniqueName="CaseOIC" DataField="CaseOIC" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CaseOIC">
                                <ItemTemplate>
                                    <%# (String.IsNullOrEmpty(Eval("CaseOIC").ToString()) ? "-" : Eval("CaseOIC"))%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                <ItemTemplate>
                                    <asp:HyperLink ID="ViewSetHyperLink" runat="server"
                                        NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    <%# Eval("Refno")%>
                                    </asp:HyperLink>
                                    &nbsp;<asp:LinkButton runat="server" ID="SetCountLinkButton"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="AddressLabel"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false">
                                <ItemTemplate>
                                    <asp:HyperLink ID="VerifyHyperLink" runat="server"
                                        NavigateUrl='<%# String.Format("AssignUser.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    Extract
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="AssessmentDateIn" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AssessmentDateIn">
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
                    PagerStyle-Position="TopAndBottom"
                    OnDetailTableDataBind="GroupByDateRadGrid_DetailTableDataBind"
                    OnPreRender="GroupByDateRadGrid_PreRender"
                    OnNeedDataSource="GroupByDateRadGrid_NeedDataSource"
                    OnItemCommand="GroupByDateRadGrid_ItemCommand"
                    OnItemDataBound="GroupByDateRadGrid_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                    <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="AssessmentDateIn" ClientDataKeyNames="AssessmentDateIn">
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="AssessmentDateIn" SortOrder="Descending"></telerik:GridSortExpression>
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
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridDateTimeColumn DataField="AssessmentDateIn" HeaderText="Date In" UniqueName="AssessmentDateIn"
                                EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd MMM yyyy}"
                                AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                            <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression=""
                                Visible="false">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="PeOIC" UniqueName="PeOIC" DataField="" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Case OIC" UniqueName="CaseOIC" DataField="" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Set Information" UniqueName="" DataField=""
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="" DataField="" HeaderStyle-Width="70px"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                <ItemTemplate>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <DetailTables>
                            <telerik:GridTableView Name="GroupByDateDetailTable" Width="100%" runat="server" AllowFilteringByColumn="false"
                                ShowHeader="true" AllowPaging="True" PageSize="20">
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
                                    <telerik:GridDateTimeColumn DataField="AssessmentDateIn" HeaderText="Date In" UniqueName="AssessmentDateIn"
                                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy HH:mm}"
                                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" />
                                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status"
                                        Visible="false">
                                        <ItemTemplate>
                                            <%# Eval("Status").ToString().Replace("_", " ") %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="PeOIC" UniqueName="PeOIC" DataField="PeOIC" HeaderStyle-Width="70px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="PeOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("PeOIC").ToString()) ? "-" : Eval("PeOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Case OIC" UniqueName="CaseOIC" DataField="CaseOIC" HeaderStyle-Width="70px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CaseOIC">
                                        <ItemTemplate>
                                            <%# (String.IsNullOrEmpty(Eval("CaseOIC").ToString()) ? "-" : Eval("CaseOIC"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="ViewSetHyperLink" runat="server"
                                                NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                    <%# Eval("Refno")%>
                                            </asp:HyperLink>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Address" UniqueName="Address" DataField=""
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="AddressLabel"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Set Information" UniqueName="" DataField=""
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="SetInformationLabel"></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="CheckLinkButton" runat="server"
                                                CommandName="Check" CommandArgument='<%# Eval("Id") %>'>
                                    Check
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="AssessmentDateIn" HeaderStyle-Width="70px"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AssessmentDateIn">
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
                    </ClientSettings>
                    <FilterMenu OnClientShown="MenuShowing">
                    </FilterMenu>
                </telerik:RadGrid>
            </asp:View>
        </asp:MultiView>
    </asp:Panel>
    <%-- <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
            SelectMethod="GetDocAppIDOnly"
            TypeName="Dwms.Bll.DocAppDb">
        </asp:ObjectDataSource> --%>
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
        SelectMethod="GetNricForCompletenessDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
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

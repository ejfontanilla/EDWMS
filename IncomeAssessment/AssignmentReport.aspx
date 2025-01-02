<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="AssignmentReport.aspx.cs" Inherits="IncomeAssessment_AssignmentReport" Title="DWMS - Assignment Report (Income Assessment)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td>
                <div class="title">Assignment Report</div>
            </td>
            <td align="right">
                <asp:LinkButton ID="ExportLinkButton" runat="server" class="link-excel" OnClick="ExportLinkButton_Click">
                    Export to Excel
                </asp:LinkButton>
            </td>
        </tr>
    </table>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <telerik:RadComboBox ID="UserRadComboBox" runat="server" AllowCustomText="True"
            AutoPostBack="true" CausesValidation="false"
            EnableAutomaticLoadOnDemand="true" DataTextField="Name"
            MarkFirstMatch="True" DataValueField="UserId" Filter="Contains"
            EmptyMessage="Type a Income Extraction OIC..." Width="210"
            EnableVirtualScrolling="true" ItemsPerRequest="20" Enabled="True"
            OnSelectedIndexChanged="UserRadComboBox_SelectedIndexChanged">
        </telerik:RadComboBox>
        &nbsp;
        <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
            AutoPostBack="false" CausesValidation="false" OnClientTextChange="DocAppRadComboBox_TextChange"
            EnableAutomaticLoadOnDemand="true" DataTextField="RefNo" MarkFirstMatch="True" DataValueField="Id" Filter="Contains"
            EmptyMessage="Type a Ref No..." DataSourceID="ObjectDataSource1" Width="185" EnableVirtualScrolling="true" ItemsPerRequest="20">
        </telerik:RadComboBox>--%>
        <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="false"
            EmptyMessage="Type a Ref No..." Skin="Windows7">
        </telerik:RadTextBox>
        &nbsp;
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
        <telerik:RadComboBox ID="StatusRadComboBox" runat="server" ZIndex="30000" Height="200"
            Width="180" DropDownWidth="180" AppendDataBoundItems="true"
            ExpandAnimation-Duration="100" AutoPostBack="false" CausesValidation="false"
            CollapseAnimation-Duration="200">
            <Items>
                <telerik:RadComboBoxItem Text="- All Status -" Value="-1" Selected="true" />
            </Items>
        </telerik:RadComboBox>
        &nbsp;
        <div class="top5">
            <!---->
        </div>
        <span class="label">Date Assigned From:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateInFromRadDateTimePicker" runat="server" Skin="Windows7">
        </telerik:RadDateTimePicker>
        &nbsp;&nbsp; <span class="label">Date Assigned To:</span>&nbsp;
        <telerik:RadDateTimePicker ID="DateInToRadDateTimePicker" runat="server" Skin="Windows7">
            <DateInput runat="server">
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
        <telerik:RadGrid ID="GroupByDateRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
            Skin="Windows7" EnableEmbeddedSkins="false" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
            AllowFilteringByColumn="True" EnableLinqExpressions="False"
            PagerStyle-Position="TopAndBottom"
            OnExcelExportCellFormatting="RadGrid1_ExcelExportCellFormatting"
            OnNeedDataSource="GroupByDateRadGrid_NeedDataSource"
            OnItemDataBound="GroupByDateRadGrid_ItemDataBound">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%">
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="DateAssigned" SortOrder="Ascending"></telerik:GridSortExpression>
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
                        HeaderStyle-Width="40px">
                        <ItemTemplate>
                            <%--<asp:Label ID="ItemCountLabel" runat="server"></asp:Label>--%>
                            <%# Container.ItemIndex + 1 %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Income Extraction OIC" UniqueName="AssessmentOIC"
                        DataField="AssessmentOIC" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AssessmentOIC" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <%# (String.IsNullOrEmpty(Eval("AssessmentOIC").ToString()) ? "-" : Eval("AssessmentOIC"))%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Ref No" UniqueName="RefNo" DataField="RefNo"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo" HeaderStyle-Width="80px">
                        <ItemTemplate>
                            <asp:HyperLink ID="ViewSetHyperLink" runat="server" Text='<%# Eval("Refno")%>'
                                NavigateUrl='<%# String.Format("View.aspx?id={0}", Eval("Id")) %>' Target="_blank">
                                <%# Eval("Refno")%>
                            </asp:HyperLink>
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
                    <telerik:GridDateTimeColumn DataField="DateAssigned" HeaderText="Date Assigned" UniqueName="DateAssigned"
                        EmptyDataText="-" AllowFiltering="true" DataType="System.DateTime" DataFormatString="{0:dd MMM yyyy, hh:mm tt}"
                        AutoPostBackOnFilter="true" PickerType="DateTimePicker" HeaderStyle-Width="150px" />
                    <telerik:GridTemplateColumn HeaderText="Status" UniqueName="Status" DataField="Status"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Status" HeaderStyle-Width="180px">
                        <ItemTemplate>
                            <%# Eval("Status").ToString().Replace("_", " ") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Set Information" UniqueName="" DataField=""
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="" Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="SetInformationLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Aging" UniqueName="Aging" DataField="DateIn" HeaderStyle-Width="70px"
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
    </asp:Panel>
    <asp:HiddenField ID="CompletenessOicHiddenField" runat="server" />
    <%--<asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
        SelectMethod="GetDocAppIDOnly"
        TypeName="Dwms.Bll.DocAppDb"></asp:ObjectDataSource>--%>
    <%--<asp:ObjectDataSource ID="GetNricObjectDataSource" runat="server"
        SelectMethod="GetNricForCompletenessDropDownList"
        TypeName="Dwms.Bll.CustomPersonal"></asp:ObjectDataSource>--%>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="CompletenessOicHiddenField" />
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
    <asp:ObjectDataSource ID="GetUserObjectDataSource" runat="server"
        SelectMethod="GetCompletenessOfficers" OnSelecting="GetUserObjectDataSource_Selecting"
        TypeName="Dwms.Bll.DocAppDb"></asp:ObjectDataSource>
</asp:Content>

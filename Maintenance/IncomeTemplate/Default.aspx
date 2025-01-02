<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_IncomeTemplate_Default"
    MasterPageFile="~/Maintenance/Main.master" Title="DWMS - Income Template" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Income Template" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable to save the template. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The template has been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" >
        <div class="header">
            <div class="left">
                Income Template Details</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr id="Tr1" runat="server">
                    <td valign="top" width="100">
                        <span class="label">Income Template</span>
                    </td>
                    <td colspan="2">
                        <asp:DropDownList ID="ListDropDownList" runat="server" CssClass="form-field" AutoPostBack="true"
                            Width="300px" OnSelectedIndexChanged="ListDropDownList_SelectedIndexChanged">
                        </asp:DropDownList>
                        <telerik:RadButton ID="AddTemplateButton" runat="server" Skin="Windows7" Text="Add"
                            Width="50px" AutoPostBack="false">
                        </telerik:RadButton>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td valign="top">
                        <telerik:RadGrid ID="IncomeDetailsRadGrid" runat="server" AutoGenerateColumns="False"
                            AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                            PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" PagerStyle-Position="TopAndBottom"
                            OnNeedDataSource="IncomeDetailsRadGrid_NeedDataSource" OnItemDataBound="IncomeDetailsRadGrid_ItemDataBound"
                            AllowMultiRowSelection="True" AllowAutomaticUpdates="false">
                            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                            <MasterTableView AllowFilteringByColumn="false" Width="100%" DataKeyNames="Id" ClientDataKeyNames="Id"
                                TableLayout="Fixed" EditMode="InPlace" ShowFooter="true">
                                <NoRecordsTemplate>
                                    <div class="wrapper10">
                                        No records were found.
                                    </div>
                                </NoRecordsTemplate>
                                <ItemStyle CssClass="pointer" />
                                <AlternatingItemStyle CssClass="pointer" />
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                        DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="150px"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="IdLabel" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="30px"
                                        AllowFiltering="false">
                                        <ItemTemplate>
                                            <asp:Label ID="ContainerLabel" runat="server" Text='<%# Container.DataSetIndex + 1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Item" UniqueName="IncomeItem" DataField="IncomeItem"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="IncomeItem"
                                        AllowFiltering="false" HeaderStyle-Width="150px">
                                        <ItemTemplate>
                                            
                                            <asp:HyperLink runat="server" ID="IncomeItemHyperLink"></asp:HyperLink>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Gross Income" UniqueName="GrossIncome" DataField="GrossIncome"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="GrossIncome"
                                        AllowFiltering="false" HeaderStyle-Width="70">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="GrossIncomeCheckBox" runat="server" AutoPostBack="true" Enabled="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Allowance" UniqueName="Allowance" DataField="Allowance"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Allowance"
                                        AllowFiltering="false" HeaderStyle-Width="70">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="AllowanceCheckBox" runat="server" AutoPostBack="true" Enabled="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Overtime" UniqueName="Overtime" DataField="Overtime"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Overtime"
                                        AllowFiltering="false" HeaderStyle-Width="70">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="OvertimeCheckBox" runat="server" AutoPostBack="true" Enabled="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="AHG Income" UniqueName="AHGIncome" DataField="AHGIncome"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AHGIncome"
                                        AllowFiltering="false" HeaderStyle-Width="100px" >
                                        <ItemTemplate>
                                            <asp:CheckBox ID="AHGIncomeCheckBox" runat="server" AutoPostBack="true" Enabled="false" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="CA Income" UniqueName="CAIncome" DataField="CAIncome"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CAIncome"
                                        AllowFiltering="false" HeaderStyle-Width="70" >
                                        <ItemTemplate>
                                            <asp:CheckBox ID="CAIncomeCheckBox" runat="server" AutoPostBack="true" Enabled="false"/>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true" AllowRowsDragDrop="true">
                                <Selecting AllowRowSelect="True" />
                                <Resizing AllowColumnResize="True" />
                                <Scrolling SaveScrollPosition="false" />
                                
                            </ClientSettings>
                        </telerik:RadGrid>

                        <br />
                        <%--<br />
                        <telerik:RadListBox ID="ValuesRadListBox" runat="server" AllowDelete="True" Height="200px"
                            Width="350px" ButtonSettings-AreaWidth="50px" Skin="Windows7" ButtonSettings-Position="Right"
                            AllowReorder="True">
                        </telerik:RadListBox>
                        <br />
                        <br />--%>
                        <telerik:RadButton ID="AddRadButton" runat="server" Skin="Windows7" Text="Add Income Component" Width="170px"
                            AutoPostBack="false">
                        </telerik:RadButton>
                        <asp:CustomValidator ID="ValuesRadListBoxCustomValidator" runat="server" ErrorMessage="<br />Please add at least 1 value."
                            CssClass="form-error" Display="Dynamic" OnServerValidate="ValuesRadListBoxCustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <%--<asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />--%>
            <%--<a href="javascript:history.back(1);">Cancel</a> --%>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(condition) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(condition);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewLog.aspx.cs" Inherits="IncomeAssessment_ViewLog"
    Title="DWMS - View Log (Income Extraction)" MasterPageFile="~/Blank.master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="View Log" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The Reference Number(s) have been assigned to the user.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <table width="100%">
            <tr>
                <td valign="top">
                    <b><asp:Label runat="server" ID="DescriptionLabel"></asp:Label></b>
                    <br /><br />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <telerik:RadGrid ID="ViewLogRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="False"
                        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False" PageSize="20"
                        AllowFilteringByColumn="False" EnableLinqExpressions="False" PagerStyle-Position="TopAndBottom"
                        OnItemDataBound="ViewLogRadGrid_ItemDataBound" OnNeedDataSource="ViewLogRadGrid_NeedDataSource">
                        <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                        <MasterTableView AllowFilteringByColumn="false" Width="100%" >
                            <NoRecordsTemplate>
                                <div class="wrapper10">
                                    No records were found.
                                </div>
                            </NoRecordsTemplate>
                            <ItemStyle CssClass="pointer" />
                            <AlternatingItemStyle CssClass="pointer" />
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                    AllowFiltering="false">
                                    <ItemTemplate>
                                        <%# Container.DataSetIndex + 1 %>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Viewed On" UniqueName="LogDate" DataField="LogDate"
                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LogDate"
                                    AllowFiltering="false" HeaderStyle-Width="150px">
                                    <ItemTemplate>
                                        <asp:Label ID="LogDateLabel" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Viewed By" UniqueName="Name" DataField="Name"
                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Name" AllowFiltering="false"
                                    HeaderStyle-Width="150px">
                                    <ItemTemplate>
                                        <%# Eval("Name")%>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Income Document | CMDocument Id" UniqueName="Description"
                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:label runat="server" ID="lblDoc" Text="" ></asp:label><br />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Start Date" UniqueName="StartDate"
                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:label runat="server" ID="lblStartDate" Text="" ></asp:label><br />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="End Date" UniqueName="EndDate"
                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:label runat="server" ID="lblEndDate" Text="" ></asp:label><br />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                            <Selecting AllowRowSelect="True" />
                            <Resizing AllowColumnResize="False" />
                            <Scrolling SaveScrollPosition="false" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

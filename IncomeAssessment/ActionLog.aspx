<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ActionLog.aspx.cs" Inherits="IncomeAssessment_ActionLog" 
Title="DWMS - Income Version (Income Extraction)" MasterPageFile="~/Blank.master"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Action Log" />
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The Reference Number(s) have been assigned to the user.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <table width="100%">
            <tr>
                <td valign="top">
                    <b>
                        <asp:Label runat="server" ID="DescriptionLabel"></asp:Label></b>
                    <br />
                    <br />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <telerik:RadGrid ID="ActionLogRadGrid" runat="server" AutoGenerateColumns="False"
                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" PagerStyle-Position="TopAndBottom"
                        OnItemDataBound="ActionLogRadGrid_ItemDataBound" OnNeedDataSource="ActionLogRadGrid_NeedDataSource">
                        <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
                        <MasterTableView AllowFilteringByColumn="false" Width="100%">
                            <NoRecordsTemplate>
                                <div class="wrapper10">
                                    No records were found.
                                </div>
                            </NoRecordsTemplate>
                            <ItemStyle CssClass="pointer" />
                            <AlternatingItemStyle CssClass="pointer" />
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="15px"
                                    AllowFiltering="false" >
                                    <ItemTemplate>
                                        <%# Container.DataSetIndex + 1 %>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Name" UniqueName="Name" DataField="Name"
                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Name"
                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblName" Text=""></asp:Label>                                             
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Description" UniqueName="Description" DataField="Description"
                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Description"
                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblDescription" Text=""></asp:Label>                                                                                                                   
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" DataField="Action"
                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Action"
                                    AllowFiltering="false" HeaderStyle-Width="200px">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblAction" Text=""></asp:Label>                                                                                                                   
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Log Date" UniqueName="LogDate"
                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="100px">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblLogDate" Text=""></asp:Label><br />
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
        <a href="javascript:GetRadWindow().close();">Close</a>   
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
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

            function CloseWindow(command) {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateParentPage(command);
            }

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPage(null);
            }

            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(command) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(command);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

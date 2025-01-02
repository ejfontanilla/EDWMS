<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="ChooseReference.aspx.cs" Inherits="Common_ChooseReference" Title="DWMS - Reference Numbers" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Reference Numbers" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Choose Reference Number</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="ErrorLabel" runat="server" Visible="false" CssClass="form-error">Please select a reference number.</asp:Label>
                        <telerik:RadGrid ID="RefNoRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                            Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                            AllowFilteringByColumn="False" EnableLinqExpressions="False" AllowMultiRowSelection="false" 
                            onneeddatasource="RefNoRadGrid_NeedDataSource">
                            <PagerStyle Mode="NextPrevAndNumeric" />
                            <MasterTableView DataKeyNames="RefNo">
                                <ItemStyle CssClass="pointer" />
                                <AlternatingItemStyle CssClass="pointer" />
                                <NoRecordsTemplate>
                                    <div class="wrapper10">
                                        No records were found.
                                    </div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="ChildCheckBoxColumn" HeaderStyle-Width="30px">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                        HeaderStyle-Width="40">
                                        <ItemTemplate>
                                            <%# Container.ItemIndex + 1 %>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Reference Number" UniqueName="RefNo" DataField="RefNo"
                                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                        <ItemTemplate>
                                            <%# Eval("RefNo")%>
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
                    </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Select" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:CloseWindow('');">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
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

            function CloseWindow(condition) {
                GetRadWindow().close();
                GetRadWindow().BrowserWindow.UpdateReferenceParentPage(condition);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="View.aspx.cs" Inherits="IncomeAssessment_ViewApp" Title="DWMS - View Application (Income Extraction)"
    EnableEventValidation="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label runat="server" ID="TitleLabel"></asp:Label>
    </div>
    <div class="bottom10">
        <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="reminder-red top10 bottom15">
            <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" />
        </asp:Panel>
        <asp:Panel ID="ConfirmPanel" runat="server" Visible="false" CssClass="reminder-green top10 bottom15">
            <asp:Label ID="SetConfirmLabel" runat="server" Font-Bold="true" /><br />
        </asp:Panel>
        <asp:Table ID="tblRefInfo" runat="server">
        </asp:Table>
        <br />
        <asp:HyperLink ID="CompletenessHyperLink" runat="server" Font-Bold="true" Target="_blank">View Household Structure and All Documents (Completeness Module)</asp:HyperLink>
        <asp:Button runat="server" ID="ActionLogButton" Text="Log" CssClass="button-large2"
            Width="50" OnClick="ConfirmButton_Click" />&nbsp;
    </div>
    <asp:Panel ID="ThePanel" runat="server">
        <asp:MultiView ID="SummaryMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="CosSummaryView" runat="server">
                <asp:Panel ID="CosNoSummaryPanel" runat="server" Visible="false">
                    <div class="bottom10">
                        <!---->
                    </div>
                    <table class="table-blue" width="100%">
                        <tr class="bg-blue">
                            <td>
                                No records were found.
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Repeater ID="HleSummaryRepeater" runat="server" OnItemDataBound="HleSummaryRepeater_ItemDataBound"
                    OnItemCommand="HleSummaryRepeater_ItemCommand">
                    <ItemTemplate>
                        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
                            <div class="header">
                                <div class="left" id='div_hle_<%# Eval("ApplicantType") %>'>
                                    <asp:Label ID="ApplicantLabel" runat="server" Font-Bold="true"></asp:Label>
                                    <asp:Image runat="server" ID="ApplicantImage" ImageUrl="~/Data/Images/Green_Check.png"
                                        Visible="false" />
                                    - <a href='javascript:toggle("div_<%# Container.ItemIndex %>");'>
                                        <asp:Label ID="ShowPendingDocLabel" runat="server" Text="Collapse/Expand"></asp:Label></a>
                                </div>
                                <div class="right">
                                    <!---->
                                </div>
                            </div>
                            <div class="area">
                                <asp:Panel ID="PersonalInfoPanel" runat="server">
                                    <table>
                                        <tr>
                                            <td width="10%">
                                                <span class="label">Date of Birth</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="DateOfBirthLabel" runat="server"></asp:Label>
                                            </td>
                                            <td width="10%">
                                                <span class="label">Employment Status</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="EmploymentStatusLabel" runat="server"></asp:Label>
                                            </td>
                                            <%--<td width="10%">
                                                <asp:Label ID="AHGLabel1" runat="server" Text="Avg AHG Income 1" class="label"></asp:Label>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="AvgAHGIncome1label" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1CaptureLinkBtn" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1ClearLinkBtn" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Date Joined Service</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="DateJoinedLabel" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Company Name</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CompanyNameLabel" runat="server"></asp:Label>
                                            </td>
                                            <%--<td>
                                                <asp:Label ID="AHGLabel2" runat="server" Text="Avg AHG Income 2" class="label"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="AvgAHGIncome2label" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2CaptureLinkBtn" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2ClearLinkBtn" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Citizenship</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CitizenshipLabel" runat="server">Singaporean Citizen</asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Extraction Period</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="AssessmentPeriodLabel" runat="server"></asp:Label>&nbsp
                                                <asp:LinkButton ID="AssessmentPeriodLinkBtn" runat="server" Text="Change"></asp:LinkButton>
                                                <asp:Image ID="exImage" runat="server" ImageUrl="~/Data/Images/Exclamation.gif" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <span class="label">Months to pass to LEAS</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="MonthsToLeasLabel" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="linkIncomeToZero" runat="server" Font-Bold="true" Visible="false">Set Income to 0</asp:LinkButton>
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:LinkButton ID="ViewConsolidatedLinkBtn" runat="server" Font-Bold="true">View Consolidated PDF</asp:LinkButton>
                                &nbsp|&nbsp
                                <asp:LinkButton ID="ShowHideNotAcceptedLinkBtn" runat="server" Font-Bold="true" CommandName="buy"
                                    UseSubmitBehavior="False"></asp:LinkButton>
                                <asp:HiddenField ID="ShowHideText" runat="server"></asp:HiddenField>
                            </div>
                        </asp:Panel>
                        <div id='div_<%# Container.ItemIndex %>'>
                            <telerik:RadTabStrip runat="server" ID="TabStrip" MultiPageID="RadMultiPage1" SelectedIndex="0"
                                CausesValidation="False">
                                <%--OnTabClick="TabStrip_OnTabClick"--%>
                                <Tabs>
                                    <telerik:RadTab runat="server" Text="Documents" PageViewID="PageViewDocuments" />
                                    <telerik:RadTab runat="server" Text="Months" PageViewID="PageViewMonths" />
                                    <telerik:RadTab runat="server" Text="Income Data" PageViewID="PageViewEmployed" />
                                </Tabs>
                            </telerik:RadTabStrip>
                            <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
                                <telerik:RadPageView ID="PageViewDocuments" runat="server">
                                    <telerik:RadGrid ID="DocumentsRadGrid" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="DocumentsRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.ItemIndex + 1 %>
                                                        <%--<%# Eval("Id") %>--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Doc Type" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="docTypeCodeLabel" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'></asp:Label>
                                                        <%--         <asp:LinkButton runat="server" ID="docTypeCodeLinkButton" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'
                                                            CommandArgument='<%#Eval("Id")%>' CausesValidation="false" OnClick="DocTypeCodeLinkButtonClick" />--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Zone" UniqueName="Zone" DataField="Zone"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Zone" AllowFiltering="false"
                                                    HeaderStyle-Width="20px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="ZoneLinkButton"></asp:LinkButton>                                                        
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Start Date/Document Date" UniqueName="DocStartDate"
                                                    DataField="StartDate" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="StartDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("StartDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("StartDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="End Date" UniqueName="DocEndDate" DataField="EndDate"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="EndDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("EndDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("EndDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Company Name" UniqueName="CompanyName" AutoPostBackOnFilter="true"
                                                    DataType="System.String" AllowFiltering="false" HeaderStyle-Width="100px" Visible="false">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("CompanyName").ToString()) ? "-" : Eval("CompanyName") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Image Condition" UniqueName="ImageCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("ImageCondition").ToString()) ? "-" : Eval("ImageCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Document Condition" UniqueName="DocumentCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("DocumentCondition").ToString()) ? "-" : Eval("DocumentCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Remark" UniqueName="Exception" AllowFiltering="false"
                                                    HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("Exception").ToString()) ? "-" : Eval("Exception")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Acceptance" UniqueName="IsAccepted" AllowFiltering="false"
                                                    HeaderStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="ReceivedLabel" runat="server"><%# string.IsNullOrEmpty(Eval("IsAccepted").ToString()) || !bool.Parse(Eval("IsAccepted").ToString()) ? "No" : "Yes"%></asp:Label>
                                                        <asp:Image ID="EditIconImage" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            Visible="false" />
                                                        <asp:ImageButton ID="EditImageButton" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            ToolTip="Click to indicate acceptance" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewMonths" runat="server">
                                    <telerik:RadGrid ID="SummaryDocsRadGrid" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="SummaryDocRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataSetIndex + 1 %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Month" UniqueName="MonthYear" DataField="MonthYear"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="MonthYear"
                                                    AllowFiltering="false" HeaderStyle-Width="120px">
                                                    <ItemTemplate>
                                                        <asp:HyperLink runat="server" ID="MonthYearLink" Target="_blank" Text='<%# Eval("MonthYear")%>'
                                                            Visible="false"></asp:HyperLink>
                                                        <asp:Label runat="server" ID="MonthYearLabel" Text='<%# Eval("MonthYear")%>' Visible="false"></asp:Label>
                                                        <asp:Image runat="server" ID="AcceptedImage" ImageUrl="~/Data/Images/Green_Check.png"
                                                            Visible="false" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Income Document | Status" UniqueName="Document"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Repeater runat="server" ID="lblDocRepeater" OnItemDataBound="lblDocRepeater_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:HyperLink runat="server" ID="lblDoc" Text="" Target="_blank"></asp:HyperLink><br />
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="CPF Income" UniqueName="CPFIncome" DataField="CPFIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="CPFIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px" Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("CPFIncome") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Gross Income" UniqueName="GrossIncome" DataField="GrossIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="GrossIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("GrossIncome") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Allowance" UniqueName="Allowance" DataField="Allowance"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Allowance"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("Allowance") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Overtime" UniqueName="Overtime" DataField="Overtime"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Overtime"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("Overtime")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Currency" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="CurrencyLabel" Text="" Visible="false"></asp:Label>
                                                        <asp:LinkButton runat="server" ID="CurrencyLink" Text="" Visible="false"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewEmployed" runat="server">
                                    <div class="horizontalScroll">
                                        <asp:Table runat="server" ID="EmployedTable" BorderWidth="0" Width="150%" BorderColor="#dddddd">
                                        </asp:Table>
                                    </div>
                                </telerik:RadPageView>
                            </telerik:RadMultiPage>
                        </div>
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View ID="ResalesSummaryView" runat="server">
                <asp:Panel ID="ResalesSummaryPanel" runat="server" Visible="false">
                    <div class="bottom10">
                        <!---->
                    </div>
                    <table class="table-blue" width="100%">
                        <tr class="bg-blue">
                            <td>
                                No records were found.
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Repeater ID="ResaleSummaryRepeater" runat="server" OnItemDataBound="ResaleSummaryRepeater_ItemDataBound"
                    OnItemCommand="ResaleSummaryRepeater_ItemCommand">
                    <ItemTemplate>
                        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
                            <div class="header">
                                <div class="left" id='div_hle_<%# Eval("ApplicantType") %>'>
                                    <asp:Label ID="ApplicantLabelRS" runat="server" Font-Bold="true"></asp:Label>
                                    <asp:Image runat="server" ID="ApplicantImage" ImageUrl="~/Data/Images/Green_Check.png"
                                        Visible="false" />
                                    - <a href='javascript:toggle("div_<%# Container.ItemIndex %>");'>
                                        <asp:Label ID="ShowPendingDocLabel" runat="server" Text="Collapse/Expand"></asp:Label></a>
                                </div>
                                <div class="right">
                                    <!---->
                                </div>
                            </div>
                            <div class="area">
                                <asp:Panel ID="PersonalInfoPanel" runat="server">
                                    <table>
                                        <tr>
                                            <td width="10%">
                                                <span class="label">Date of Birth</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="DateOfBirthLabelRS" runat="server"></asp:Label>
                                            </td>
                                            <td width="10%">
                                                <span class="label">Employment Status</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="EmploymentStatusLabelRS" runat="server"></asp:Label>
                                            </td>
                                            <%-- <td width="10%">
                                                <asp:Label ID="AHGLabel1RS" runat="server" Text="Avg AHG Income 1" class="label" Visible="false"></asp:Label>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="AvgAHGIncome1labelRS" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1CaptureLinkBtnRS" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1ClearLinkBtnRS" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Date Joined Service</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="DateJoinedLabelRS" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Company Name</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CompanyNameLabelRS" runat="server"></asp:Label>
                                            </td>
                                            <%--<td>
                                                <asp:Label ID="AHGLabel2" runat="server" Text="Avg AHG Income 2" class="label"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="AvgAHGIncome2labelRS" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2CaptureLinkBtnRS" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2ClearLinkBtnRS" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Citizenship</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CitizenshipLabelRS" runat="server">Singaporean Citizen</asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Extraction Period</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="AssessmentPeriodLabelRS" runat="server"></asp:Label>&nbsp
                                                <asp:LinkButton ID="AssessmentPeriodLinkBtnRS" runat="server" Text="Change"></asp:LinkButton>
                                                <asp:Image ID="exImage" runat="server" ImageUrl="~/Data/Images/Exclamation.gif" Visible="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <span class="label">Months to pass</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="MonthsToResaleLabel" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:LinkButton ID="ViewConsolidatedLinkBtnRS" runat="server" Font-Bold="true">View Consolidated PDF</asp:LinkButton>
                                &nbsp|&nbsp
                                <asp:LinkButton ID="ShowHideNotAcceptedLinkBtnRS" runat="server" Font-Bold="true"
                                    CommandName="buy" UseSubmitBehavior="False"></asp:LinkButton>
                                <asp:HiddenField ID="ShowHideTextRS" runat="server"></asp:HiddenField>
                            </div>
                        </asp:Panel>
                        <div id='div_<%# Container.ItemIndex %>'>
                            <telerik:RadTabStrip runat="server" ID="TabStripRS" MultiPageID="RadMultiPage2" SelectedIndex="0"
                                CausesValidation="False">
                                <%--OnTabClick="TabStrip_OnTabClick"--%>
                                <Tabs>
                                    <telerik:RadTab runat="server" Text="Documents" PageViewID="PageViewDocumentsRS" />
                                    <telerik:RadTab runat="server" Text="Months" PageViewID="PageViewMonthsRS" />
                                    <telerik:RadTab runat="server" Text="Income Data" PageViewID="PageViewEmployedRS" />
                                </Tabs>
                            </telerik:RadTabStrip>
                            <telerik:RadMultiPage ID="RadMultiPage2" runat="server" SelectedIndex="0">
                                <telerik:RadPageView ID="PageViewDocumentsRS" runat="server">
                                    <telerik:RadGrid ID="DocumentsRadGridRS" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="DocumentsRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.ItemIndex + 1 %>
                                                        <%--<%# Eval("Id") %>--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Doc Type" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="docTypeCodeLabel" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'></asp:Label>
                                                        <%--         <asp:LinkButton runat="server" ID="docTypeCodeLinkButton" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'
                                                            CommandArgument='<%#Eval("Id")%>' CausesValidation="false" OnClick="DocTypeCodeLinkButtonClick" />--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Zone" UniqueName="Zone" DataField="Zone"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Zone" AllowFiltering="false"
                                                    HeaderStyle-Width="20px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="ZoneLinkButton"></asp:LinkButton>                                                        
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Start Date/Document Date" UniqueName="DocStartDate"
                                                    DataField="StartDate" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="StartDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("StartDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("StartDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="End Date" UniqueName="DocEndDate" DataField="EndDate"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="EndDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("EndDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("EndDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Company Name" UniqueName="CompanyName" AutoPostBackOnFilter="true"
                                                    DataType="System.String" AllowFiltering="false" HeaderStyle-Width="100px" Visible="false">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("CompanyName").ToString()) ? "-" : Eval("CompanyName") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Image Condition" UniqueName="ImageCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("ImageCondition").ToString()) ? "-" : Eval("ImageCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Document Condition" UniqueName="DocumentCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("DocumentCondition").ToString()) ? "-" : Eval("DocumentCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Remark" UniqueName="Exception" AllowFiltering="false"
                                                    HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("Exception").ToString()) ? "-" : Eval("Exception")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Acceptance" UniqueName="IsAccepted" AllowFiltering="false"
                                                    HeaderStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="ReceivedLabel" runat="server"><%# string.IsNullOrEmpty(Eval("IsAccepted").ToString()) || !bool.Parse(Eval("IsAccepted").ToString()) ? "No" : "Yes"%></asp:Label>
                                                        <asp:Image ID="EditIconImage" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            Visible="false" />
                                                        <asp:ImageButton ID="EditImageButton" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            ToolTip="Click to indicate acceptance" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewMonthsRS" runat="server">
                                    <telerik:RadGrid ID="SummaryDocsRadGridRS" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="SummaryDocRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataSetIndex + 1 %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Month" UniqueName="MonthYear" DataField="MonthYear"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="MonthYear"
                                                    AllowFiltering="false" HeaderStyle-Width="120px">
                                                    <ItemTemplate>
                                                        <asp:HyperLink runat="server" ID="MonthYearLink" Target="_blank" Text='<%# Eval("MonthYear")%>'
                                                            Visible="false"></asp:HyperLink>
                                                        <asp:Label runat="server" ID="MonthYearLabel" Text='<%# Eval("MonthYear")%>' Visible="false"></asp:Label>
                                                        <asp:Image runat="server" ID="AcceptedImage" ImageUrl="~/Data/Images/Green_Check.png"
                                                            Visible="false" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Income Document | Status" UniqueName="Document"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Repeater runat="server" ID="lblDocRepeater" OnItemDataBound="lblDocRepeater_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:HyperLink runat="server" ID="lblDoc" Text="" Target="_blank"></asp:HyperLink><br />
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Gross Income" UniqueName="GrossIncome" DataField="GrossIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="GrossIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("GrossIncome") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="AHG Income" UniqueName="AHGIncome" DataField="AHGIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AHGIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("AHGIncome")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Currency" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="CurrencyLabel" Text="" Visible="false"></asp:Label>
                                                        <asp:LinkButton runat="server" ID="CurrencyLink" Text="" Target="_blank"></asp:LinkButton><br />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewEmployedRS" runat="server">
                                    <div class="horizontalScroll">
                                        <asp:Table runat="server" ID="EmployedTableRS" BorderWidth="0" Width="150%" BorderColor="#dddddd">
                                        </asp:Table>
                                    </div>
                                </telerik:RadPageView>
                            </telerik:RadMultiPage>
                        </div>
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </asp:View>
            <asp:View ID="SalesSummaryView" runat="server">
                <asp:Panel ID="SalesSummaryPanel" runat="server" Visible="false">
                    <div class="bottom10">
                        <!---->
                    </div>
                    <table class="table-blue" width="100%">
                        <tr class="bg-blue">
                            <td>
                                No records were found.
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Repeater ID="SalesSummaryRepeater" runat="server" OnItemDataBound="SalesSummaryRepeater_ItemDataBound"
                    OnItemCommand="SalesSummaryRepeater_ItemCommand">
                    <ItemTemplate>
                        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
                            <div class="header">
                                <div class="left" id='div_hle_<%# Eval("ApplicantType") %>'>
                                    <asp:Label ID="ApplicantLabelSales" runat="server" Font-Bold="true"></asp:Label>
                                    <asp:Image runat="server" ID="ApplicantImage" ImageUrl="~/Data/Images/Green_Check.png"
                                        Visible="false" />
                                    - <a href='javascript:toggle("div_<%# Container.ItemIndex %>");'>
                                        <asp:Label ID="ShowPendingDocLabel" runat="server" Text="Collapse/Expand"></asp:Label></a>
                                </div>
                                <div class="right">
                                    <!---->
                                </div>
                            </div>
                            <div class="area">
                                <asp:Panel ID="PersonalInfoPanel" runat="server">
                                    <table>
                                        <tr>
                                            <td width="10%">
                                                <span class="label">Date of Birth</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="DateOfBirthLabelSales" runat="server"></asp:Label>
                                            </td>
                                            <td width="10%">
                                                <span class="label">Employment Status</span>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="EmploymentStatusLabelSales" runat="server"></asp:Label>
                                            </td>
                                            <%--<td width="10%">
                                                <asp:Label ID="AHGLabel1Sales" runat="server" Text="Avg AHG Income 1" class="label"></asp:Label>
                                            </td>
                                            <td width="20%">
                                                <asp:Label ID="AvgAHGIncome1labelSales" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1CaptureLinkBtnSales" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome1ClearLinkBtnSales" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Date Joined Service</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="DateJoinedLabelSales" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Company Name</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CompanyNameLabelSales" runat="server"></asp:Label>
                                            </td>
                                            <%--<td>
                                                <asp:Label ID="AHGLabelSales" runat="server" Text="Avg AHG Income 2" class="label"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="AvgAHGIncome2labelSales" runat="server">4,850 / 12 Months</asp:Label>&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2CaptureLinkBtnSales" runat="server" Text="Capture"></asp:LinkButton>&nbsp|&nbsp
                                                <asp:LinkButton ID="AvgAHGIncome2ClearLinkBtnSales" runat="server" Text="Clear"></asp:LinkButton>
                                            </td>--%>
                                        </tr>
                                        <tr>
                                            <td>
                                                <span class="label">Citizenship</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="CitizenshipLabelSales" runat="server">Singaporean Citizen</asp:Label>
                                            </td>
                                            <td>
                                                <span class="label">Extraction Period</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="AssessmentPeriodLabelSales" runat="server"></asp:Label>&nbsp
                                                <asp:LinkButton ID="AssessmentPeriodLinkBtnSales" runat="server" Text="Change"></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                            </td>
                                            <td>
                                                <span class="label">Months to pass</span>
                                            </td>
                                            <td>
                                                <asp:Label ID="MonthsToSalesLabel" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:LinkButton ID="ViewConsolidatedLinkBtnSales" runat="server" Font-Bold="true">View Consolidated PDF</asp:LinkButton>
                                &nbsp|&nbsp
                                <asp:LinkButton ID="ShowHideNotAcceptedLinkBtnSales" runat="server" Font-Bold="true"
                                    CommandName="buy" UseSubmitBehavior="False"></asp:LinkButton>
                                <asp:HiddenField ID="ShowHideTextSales" runat="server"></asp:HiddenField>
                            </div>
                        </asp:Panel>
                        <div id='div_<%# Container.ItemIndex %>'>
                            <telerik:RadTabStrip runat="server" ID="TabStripSales" MultiPageID="RadMultiPage3"
                                SelectedIndex="0" CausesValidation="False">
                                <%--OnTabClick="TabStrip_OnTabClick"--%>
                                <Tabs>
                                    <telerik:RadTab runat="server" Text="Documents" PageViewID="PageViewDocumentsSales" />
                                    <telerik:RadTab runat="server" Text="Months" PageViewID="PageViewMonthsSales" />
                                    <telerik:RadTab runat="server" Text="Income Data" PageViewID="PageViewEmployedSales" />
                                </Tabs>
                            </telerik:RadTabStrip>
                            <telerik:RadMultiPage ID="RadMultiPage3" runat="server" SelectedIndex="0">
                                <telerik:RadPageView ID="PageViewDocumentsSales" runat="server">
                                    <telerik:RadGrid ID="DocumentsRadGridSales" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="DocumentsRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.ItemIndex + 1 %>
                                                        <%--<%# Eval("Id") %>--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Doc Type" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="docTypeCodeLabel" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'></asp:Label>
                                                        <%--         <asp:LinkButton runat="server" ID="docTypeCodeLinkButton" Text='<%# Eval("DocTypeDescription") + Dwms.Web.TreeviewDWMS.FormatId(Eval("Id").ToString())%>'
                                                            CommandArgument='<%#Eval("Id")%>' CausesValidation="false" OnClick="DocTypeCodeLinkButtonClick" />--%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Zone" UniqueName="Zone" DataField="Zone"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Zone" AllowFiltering="false"
                                                    HeaderStyle-Width="20px">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="ZoneLinkButton"></asp:LinkButton>                                                        
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Start Date/Document Date" UniqueName="DocStartDate"
                                                    DataField="StartDate" AutoPostBackOnFilter="true" DataType="System.String" SortExpression="StartDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("StartDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("StartDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="End Date" UniqueName="DocEndDate" DataField="EndDate"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="EndDate"
                                                    AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("EndDate").ToString()) ? "-" : Dwms.Bll.Format.GetMetaDataValueInMetaDataDateFormatForGridDisplay(Eval("EndDate").ToString())%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Company Name" UniqueName="CompanyName" AutoPostBackOnFilter="true"
                                                    DataType="System.String" AllowFiltering="false" HeaderStyle-Width="100px" Visible="false">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("CompanyName").ToString()) ? "-" : Eval("CompanyName") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Image Condition" UniqueName="ImageCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("ImageCondition").ToString()) ? "-" : Eval("ImageCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Document Condition" UniqueName="DocumentCondition"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="80px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("DocumentCondition").ToString()) ? "-" : Eval("DocumentCondition")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Remark" UniqueName="Exception" AllowFiltering="false"
                                                    HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <%# String.IsNullOrEmpty(Eval("Exception").ToString()) ? "-" : Eval("Exception")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Acceptance" UniqueName="IsAccepted" AllowFiltering="false"
                                                    HeaderStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:Label ID="ReceivedLabel" runat="server"><%# string.IsNullOrEmpty(Eval("IsAccepted").ToString()) || !bool.Parse(Eval("IsAccepted").ToString()) ? "No" : "Yes"%></asp:Label>
                                                        <asp:Image ID="EditIconImage" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            Visible="false" />
                                                        <asp:ImageButton ID="EditImageButton" runat="server" ImageUrl="~/Data/Images/Icons/Edit.gif"
                                                            ToolTip="Click to indicate acceptance" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewMonthsSales" runat="server">
                                    <telerik:RadGrid ID="SummaryDocsRadGridSales" runat="server" AutoGenerateColumns="False"
                                        AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="False"
                                        PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemDataBound="SummaryDocRadGrid_ItemDataBound"
                                        PagerStyle-Position="TopAndBottom">
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
                                                <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" HeaderStyle-Width="20px"
                                                    AllowFiltering="false">
                                                    <ItemTemplate>
                                                        <%# Container.DataSetIndex + 1 %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                    DataType="System.String" SortExpression="Id" AllowFiltering="false" HeaderStyle-Width="20px"
                                                    Visible="false">
                                                    <ItemTemplate>
                                                        <%# Eval("Id") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Month" UniqueName="MonthYear" DataField="MonthYear"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="MonthYear"
                                                    AllowFiltering="false" HeaderStyle-Width="120px">
                                                    <ItemTemplate>
                                                        <asp:HyperLink runat="server" ID="MonthYearLink" Target="_blank" Text='<%# Eval("MonthYear")%>'
                                                            Visible="false"></asp:HyperLink>
                                                        <asp:Label runat="server" ID="MonthYearLabel" Text='<%# Eval("MonthYear")%>' Visible="false"></asp:Label>
                                                        <asp:Image runat="server" ID="AcceptedImage" ImageUrl="~/Data/Images/Green_Check.png"
                                                            Visible="false" />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Income Document | Status" UniqueName="Document"
                                                    AutoPostBackOnFilter="true" DataType="System.String" AllowFiltering="false" HeaderStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <asp:Repeater runat="server" ID="lblDocRepeater" OnItemDataBound="lblDocRepeater_ItemDataBound">
                                                            <ItemTemplate>
                                                                <asp:HyperLink runat="server" ID="lblDoc" Text="" Target="_blank"></asp:HyperLink><br />
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Gross Income" UniqueName="GrossIncome" DataField="GrossIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="GrossIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("GrossIncome") %>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="AHG Income" UniqueName="AHGIncome" DataField="AHGIncome"
                                                    AutoPostBackOnFilter="true" DataType="System.String" SortExpression="AHGIncome"
                                                    AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <%# Eval("AHGIncome")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Currency" UniqueName="DocTypeDescription"
                                                    DataField="DocTypeDescription" AutoPostBackOnFilter="true" DataType="System.String"
                                                    SortExpression="DocTypeDescription" AllowFiltering="false" HeaderStyle-Width="100px">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="CurrencyLabel" Text="" Visible="false"></asp:Label>
                                                        <asp:LinkButton runat="server" ID="CurrencyLink" Text="" Target="_blank"></asp:LinkButton><br />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings ReorderColumnsOnClient="False" EnableRowHoverStyle="true">
                                            <Selecting AllowRowSelect="False" />
                                            <Resizing AllowColumnResize="False" />
                                            <Scrolling SaveScrollPosition="false" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadPageView>
                                <telerik:RadPageView ID="PageViewEmployedSales" runat="server">
                                    <div class="horizontalScroll">
                                        <asp:Table runat="server" ID="EmployedTableSales" BorderWidth="0" Width="150%" BorderColor="#dddddd">
                                        </asp:Table>
                                    </div>
                                </telerik:RadPageView>
                            </telerik:RadMultiPage>
                        </div>
                        <br />
                    </ItemTemplate>
                </asp:Repeater>
            </asp:View>
        </asp:MultiView>
    </asp:Panel>
    <asp:Panel runat="server" ID="ButtonPanel" CssClass="form-submit" Width="100%">
        <asp:Button runat="server" ID="HousingGrantButton" Text="Extraction Worksheet" CssClass="button-large2"
            Width="200" OnClick="HousingGrant" />&nbsp;
            <asp:Button runat="server" ID="ExcelExtractionButton" Text="Extraction Worksheet to Excel" CssClass="button-large2"
            Width="200" OnClick="HousingGrant" />&nbsp;
        <%--<asp:Button runat="server" ID="AdditionalHousingGrantButton" Text="Extraction Worksheet"
            CssClass="button-large2" Width="260" OnClick="HousingGrant" Visible="false" />&nbsp;--%>
        <asp:Button runat="server" ID="ConfirmButton" Text="Confirm" CssClass="button-large-green"
            Width="100" OnClick="ConfirmButton_Click" />&nbsp;
        <br />
        <br />
    </asp:Panel>
    <asp:HiddenField runat="server" ID="NodeValueHiddenField" EnableViewState="false"
        Value="-1" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        ClientEvents-OnRequestStart="RequestStarted" EnableAJAX="true" DefaultLoadingPanelID="LoadingPanel1">
        <AjaxSettings>
            <%--           <telerik:AjaxSetting AjaxControlID="ShowHideNotAcceptedLinkBtn">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SummaryDocsRadGrid" />
                </UpdatedControls>
            </telerik:AjaxSetting>--%>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ThePanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="SummaryMultiView" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ConfirmButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ThePanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="SummaryMultiView" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ButtonPanel" />
                    <telerik:AjaxUpdatedControl ControlID="TitleLabel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" IsSticky="true"
        Style="position: absolute; top: 0; left: 0; height: 100%; width: 100%;">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var myHeight = getClientHeight();

            // Window Closing, Page Redirect events (Start)
            var hook = true;
            var edited = false;

            window.onbeforeunload = function () {
                if (hook && edited) {
                    return "You have modifications that have not been saved."
                }
            }

            function UnHook() {
                hook = false;
            }

            function Edited() {
                edited = true;
            }

            function ResetFlags() {
                hook = true;
                edited = false;
            }
            // Window Closing, Page Redirect events (End)

            function OnNodeExpanded(sender, args) {
                var category = args.get_node().get_category();
                var expandedNode = args.get_node();

                if (category == "Set") {
                    //CollapseSiblings(expandedNode);
                }
            }

            function rtvExplore_OnNodeExpandedCollapsed(sender, eventArgs)
             {
                var allNodes = eventArgs._node.get_treeView().get_allNodes();
                var i;
                var unSelectedNodes = "";
  
                for (i = 0; i < allNodes.length; i++)
                {
                     if (!allNodes[i].get_expanded())
                        unSelectedNodes += allNodes[i].get_value() + "*";
                }
                setCookie("ref" + <%= docAppId %> + "collapsedNodes", unSelectedNodes, 30);
            }

            function RefreshUrl(url)
            {
                window.location = url;
            }

            function loadExport() {
                //window.location = "../common/DownloadSetDocument.aspx?id=" + <//%= setId %>;
            }

            function RequestStarted(ajaxManager, eventArgs) {

                if (eventArgs.get_eventTarget().indexOf("ExportButton") != -1) {
                    eventArgs.set_enableAjax(false);
                }
                else {
                    eventArgs.set_enableAjax(true);
                }
            }

            function toggle(strID) {
                if (document.getElementById(strID).style.display == 'none') {
                    document.getElementById(strID).style.display = 'block';
                } else {
                    document.getElementById(strID).style.display = 'none';
                }
            }

            function CloseActiveToolTip() {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();

                UpdateParentPage(null);
                //RefreshUrl(window.location.href);
            }

            // Summary Docs
            function ToggleCosDiv(value) {
                var strID = "div_hle_" + value;

                var divs = document.getElementsByTagName("DIV");

                for (var i = 0; i < divs.length; i++) {
                    var div = divs[i];

                    if (div.id.toString().indexOf('div_hle_') > -1) {
                        if (strID.toLowerCase().indexOf('all') > -1) {
                            div.style.display = 'block';
                        }
                        else if (div.id.toString() != strID) {
                            div.style.display = 'none';
                        }
                        else {
                            div.style.display = 'block';
                        }                    
                    }
                }
            }

            function ToggleResaleDiv(value) {
                var strID = "div_resale_" + value;

                var divs = document.getElementsByTagName("DIV");

                for (var i = 0; i < divs.length; i++) {
                    var div = divs[i];

                    if (div.id.toString().indexOf('div_resale_') > -1) {
                        if (strID.toLowerCase().indexOf('all') > -1) {
                            div.style.display = 'block';
                        }
                        else if (div.id.toString() != strID) {
                            div.style.display = 'none';
                        }
                        else {
                            div.style.display = 'block';
                        }                    
                    }
                }
            }

            function ToggleSalesDiv(value) {
                var strID = "div_sales_" + value;

                var divs = document.getElementsByTagName("DIV");

                for (var i = 0; i < divs.length; i++) {
                    var div = divs[i];

                    if (div.id.toString().indexOf('div_sales_') > -1) {
                        if (strID.toLowerCase().indexOf('all') > -1) {
                            div.style.display = 'block';
                        }
                        else if (div.id.toString() != strID) {
                            div.style.display = 'none';
                        }
                        else {
                            div.style.display = 'block';
                        }                    
                    }
                }
            }

            function ToggleSersDiv(value) {
                var strID = "div_sers_" + value;

                var divs = document.getElementsByTagName("DIV");

                for (var i = 0; i < divs.length; i++) {
                    var div = divs[i];

                    if (div.id.toString().indexOf('div_sers_') > -1) {
                        if (strID.toLowerCase().indexOf('all') > -1) {
                            div.style.display = 'block';
                        }
                        else if (div.id.toString() != strID) {
                            div.style.display = 'none';
                        }
                        else {
                            div.style.display = 'block';
                        }                    
                    }
                }
            }

           function UpdateParentPage(page) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(page);
            }

            function onClicking(nric) {
                if(document.getElementById(nric).value == 'no')
                    document.getElementById(nric).value = 'yes'
                else if(document.getElementById(nric).value == 'yes')
                    document.getElementById(nric).value = 'no'      
                else
                    document.getElementById(nric).value = 'no'         
                    
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest('ShowHide');
            }

                        function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

             function reloadPage() {
                window.location.reload()
            }


             function UpdateParentPage1(page) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(page);
            }

            function SetIncomeToZero(page) {
                
                alert('ddwdw');                
                 var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(page);
            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

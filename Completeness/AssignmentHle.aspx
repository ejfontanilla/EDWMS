<%@ Page Language="C#" MasterPageFile="~/OneColumn.master" AutoEventWireup="true"
    CodeFile="AssignmentHle.aspx.cs" Inherits="Completeness_AssignmentHle" Title="DWMS - HLE Pending Assignment (Completeness)" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Bll" TagPrefix="DwmsBll" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Batch Assignment
    </div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The HLE applications have been assigned.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="top5">
            <!---->
        </div>
        <div class="header">
            <div class="left">
                Pending Assignment</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <telerik:RadGrid ID="PendingAssignmentRadGrid" runat="server" AutoGenerateColumns="False"
            AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True"
            PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" AllowMultiRowSelection="False"
            OnNeedDataSource="PendingAssignmentRadGrid_NeedDataSource">
            <PagerStyle Mode="NextPrevAndNumeric" Width="100%" />
            <MasterTableView AllowFilteringByColumn="false" Width="100%">
                <SortExpressions>
                </SortExpressions>
                <NoRecordsTemplate>
                    <div class="wrapper10">
                        No records were found.
                    </div>
                </NoRecordsTemplate>
                <ItemStyle CssClass="pointer" />
                <AlternatingItemStyle CssClass="pointer" />
                <Columns>
                    <telerik:GridTemplateColumn HeaderStyle-Width="40px">
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Date In" UniqueName="DateInConverted" DataField="DateInConverted"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="DateInConverted">
                        <ItemTemplate>
                            <%#Eval("DateInConverted")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="A" UniqueName="LaneACount" DataField="LaneACount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneACount">
                        <ItemTemplate>
                            <%#Eval("LaneACount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="B" UniqueName="LaneBCount" DataField="LaneBCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneBCount">
                        <ItemTemplate>
                            <%#Eval("LaneBCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="C" UniqueName="LaneCCount" DataField="LaneCCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneCCount">
                        <ItemTemplate>
                            <%#Eval("LaneCCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="D" UniqueName="LaneDCount" DataField="LaneDCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneDCount">
                        <ItemTemplate>
                            <%#Eval("LaneDCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="E" UniqueName="LaneECount" DataField="LaneECount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneECount">
                        <ItemTemplate>
                            <%#Eval("LaneECount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="F" UniqueName="LaneFCount" DataField="LaneFCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneFCount">
                        <ItemTemplate>
                            <%#Eval("LaneFCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="H" UniqueName="LaneHCount" DataField="LaneHCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneHCount">
                        <ItemTemplate>
                            <%#Eval("LaneHCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="L" UniqueName="LaneLCount" DataField="LaneLCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneLCount">
                        <ItemTemplate>
                            <%#Eval("LaneLCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="N" UniqueName="LaneNCount" DataField="LaneNCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneNCount">
                        <ItemTemplate>
                            <%#Eval("LaneNCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="T" UniqueName="LaneTCount" DataField="LaneTCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneTCount">
                        <ItemTemplate>
                            <%#Eval("LaneTCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="X" UniqueName="LaneXCount" DataField="LaneXCount"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LaneXCount">
                        <ItemTemplate>
                            <%#Eval("LaneXCount")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Total" UniqueName="Total" DataField="Total"
                        AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Total">
                        <ItemTemplate>
                            <%#Eval("Total")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="False" />
                <Resizing AllowColumnResize="True" />
                <Scrolling SaveScrollPosition="false" />
            </ClientSettings>
        </telerik:RadGrid>
        <%--<div class="top10"><!----></div>--%>
        <asp:Panel ID="SubmitPanel1" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="HeaderAssignButton" runat="server" Text="Assign" CssClass="button-large right20"
                OnClick="AssignButton_Click" />
        </asp:Panel>
        <%--<div class="top5"><!----></div>--%>
        <div class="header">
            <div class="left">
                Assigned Cases for Completeness Officers</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div id="RepeaterDiv">
            <asp:Repeater ID="Repeater1" runat="server" OnItemCreated="Repeater1_ItemCreated"
                OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                    <table class="table-blue" width="100%">
                        <tr class="bg-blue">
                            <td valign="top" width="150px">
                                Completeness OIC
                            </td>
                            <td valign="top">
                                A
                            </td>
                            <td valign="top">
                                B
                            </td>
                            <td valign="top">
                                C
                            </td>
                            <td valign="top">
                                D
                            </td>
                            <td valign="top">
                                E
                            </td>
                            <td valign="top">
                                F
                            </td>
                            <td valign="top">
                                H
                            </td>
                            <td valign="top">
                                L
                            </td>
                            <td valign="top">
                                N
                            </td>
                            <td valign="top">
                                T
                            </td>
                            <td valign="top">
                                X
                            </td>
                            <td valign="top">
                                Total
                            </td>
                        </tr>
                        <tr class="bg-blue">
                            <td valign="top" width="150px">
                                Total
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderATotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderBTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderCTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                             <td valign="top">
                                <asp:Label ID="HeaderDTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderETotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderFTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderHTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderLTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderNTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderTTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderXTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                            <td valign="top">
                                <asp:Label ID="HeaderTotalLabel" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td valign="top" width="150px">
                            <asp:HiddenField ID="UserHiddenField" runat="server" Value='<%#Eval("UserId")%>' />
                            <%#Eval("CompletenessOic")%>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="ATextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="BTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="CTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="DTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="ETextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="FTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="HTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="LTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="NTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="TTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:TextBox ID="XTextBox" runat="server" Width="40px" Text="0"></asp:TextBox>
                        </td>
                        <td valign="top">
                            <asp:Label ID="IndividualTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr class="bg-blue">
                        <td valign="top" width="150px">
                            Total
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterATotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterBTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterCTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterDTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterETotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterFTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterHTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterLTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterNTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterTTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterXTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                        <td valign="top">
                            <asp:Label ID="FooterTotalLabel" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                    <tr class="bg-blue">
                        <td valign="top" width="150px">
                            Completeness OIC
                        </td>
                        <td valign="top">
                            A
                        </td>
                        <td valign="top">
                            B
                        </td>
                        <td valign="top">
                            C
                        </td>
                        <td valign="top">
                            D
                        </td>
                        <td valign="top">
                            E
                        </td>
                        <td valign="top">
                            F
                        </td>
                        <td valign="top">
                            H
                        </td>
                        <td valign="top">
                            L
                        </td>
                        <td valign="top">
                            N
                        </td>
                        <td valign="top">
                            T
                        </td>
                        <td valign="top">
                            X
                        </td>
                        <td valign="top">
                            Total
                        </td>
                    </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="SubmitPanel2" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="FooterAssignButton" runat="server" Text="Assign" OnClick="AssignButton_Click"
                CssClass="button-large right20" />
        </asp:Panel>
    </asp:Panel>
    <%-- Pending assigned hidden values --%>
    <asp:HiddenField ID="LaneAUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneBUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneCUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneDUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneEUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneFUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneHUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneLUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneNUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneTUnassignedHiddenField" runat="server" />
    <asp:HiddenField ID="LaneXUnassignedHiddenField" runat="server" />
    <%-- Maximum assignment hidden values --%>
    <asp:HiddenField ID="LaneAMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneBMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneCMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneDMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneEMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneFMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneHMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneLMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneNMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneTMaxHiddenField" runat="server" />
    <asp:HiddenField ID="LaneXMaxHiddenField" runat="server" />
    <%-- Assigned hidden values --%>
    <asp:HiddenField ID="LaneATotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneBTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneCTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneDTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneETotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneFTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneHTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneLTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneNTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneTTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LaneXTotalHiddenField" runat="server" />
    <asp:HiddenField ID="LanesTotalHiddenField" runat="server" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="false">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var HeaderTotalLabelA;
            var HeaderTotalLabelB;
            var HeaderTotalLabelC;
            var HeaderTotalLabelD;
            var HeaderTotalLabelE;
            var HeaderTotalLabelF;
            var HeaderTotalLabelH;
            var HeaderTotalLabelL;
            var HeaderTotalLabelN;
            var HeaderTotalLabelT;
            var HeaderTotalLabelX;
            var HeaderTotalLabel;

            var FooterTotalLabelA;
            var FooterTotalLabelB;
            var FooterTotalLabelC;
            var FooterTotalLabelD;
            var FooterTotalLabelE;
            var FooterTotalLabelF;
            var FooterTotalLabelH;
            var FooterTotalLabelL;
            var FooterTotalLabelN;
            var FooterTotalLabelT;
            var FooterTotalLabelX;
            var FooterTotalLabel;

            var exceedMessage = "The entered value is greater than the unassigned number for the lane.";
            var notIntegerMessage = "The input is not an integer.  Please input whole numbers only.";

            function Debug(value) {
                console.log(value);
            }

            function SetFooterTotalLabelsId(clientIdA, clientIdB, clientIdC, clientIdD, clientIdE, clientIdF, clientIdH, clientIdL, clientIdN, clientIdT, clientIdX, clientIdTotal) {
                FooterTotalLabelA = clientIdA;
                FooterTotalLabelB = clientIdB;
                FooterTotalLabelC = clientIdC;
                FooterTotalLabelD = clientIdD;
                FooterTotalLabelE = clientIdE;
                FooterTotalLabelF = clientIdF;
                FooterTotalLabelH = clientIdH;
                FooterTotalLabelL = clientIdL;
                FooterTotalLabelN = clientIdN;
                FooterTotalLabelT = clientIdT;
                FooterTotalLabelX = clientIdX;
                FooterTotalLabel = clientIdTotal;
            }

            function SetHeaderTotalLabelsId(clientIdA, clientIdB, clientIdC, clientIdD, clientIdE, clientIdF, clientIdH, clientIdL, clientIdN, clientIdT, clientIdX, clientIdTotal) {
                HeaderTotalLabelA = clientIdA;
                HeaderTotalLabelB = clientIdB;
                HeaderTotalLabelC = clientIdC;
                HeaderTotalLabelD = clientIdD;
                HeaderTotalLabelE = clientIdE;
                HeaderTotalLabelF = clientIdF;
                HeaderTotalLabelH = clientIdH;
                HeaderTotalLabelL = clientIdL;
                HeaderTotalLabelN = clientIdN;
                HeaderTotalLabelT = clientIdT;
                HeaderTotalLabelX = clientIdX;
                HeaderTotalLabel = clientIdTotal;
            }

            function OnValueChanging(sender, args) {
                var senderId = sender.get_id();

                var unAssignedControl;
                var maximum = 0;
                var headerTotalControl;
                var footerTotalControl;

                if (senderId.indexOf("ARadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneAUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneAMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelA);
                    footerTotalControl = document.getElementById(FooterTotalLabelA);
                }
                else if (senderId.indexOf("BRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneBUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneBMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelB);
                    footerTotalControl = document.getElementById(FooterTotalLabelB);
                }
                else if (senderId.indexOf("CRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneCUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneCMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelC);
                    footerTotalControl = document.getElementById(FooterTotalLabelC);
                }
                else if (senderId.indexOf("DRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneDUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneDMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelD);
                    footerTotalControl = document.getElementById(FooterTotalLabelD);
                }
                else if (senderId.indexOf("ERadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneEUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneEMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelE);
                    footerTotalControl = document.getElementById(FooterTotalLabelE);
                }
                else if (senderId.indexOf("FRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneFUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneFMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelF);
                    footerTotalControl = document.getElementById(FooterTotalLabelF);
                }
                else if (senderId.indexOf("HRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneHUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneHMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelH);
                    footerTotalControl = document.getElementById(FooterTotalLabelH);
                }
                else if (senderId.indexOf("LRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneLUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneLMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelL);
                    footerTotalControl = document.getElementById(FooterTotalLabelL);
                }
                else if (senderId.indexOf("NRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneNUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneNMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelN);
                    footerTotalControl = document.getElementById(FooterTotalLabelN);
                }
                else if (senderId.indexOf("TRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneTUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneTMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelT);
                    footerTotalControl = document.getElementById(FooterTotalLabelT);
                }
                else if (senderId.indexOf("XRadNumericTextBox") > -1) {
                    unAssignedControl = document.getElementById("<%= LaneXUnassignedHiddenField.ClientID %>");
                    maximum = parseInt(document.getElementById("<%= LaneXMaxHiddenField.ClientID %>").value);
                    headerTotalControl = document.getElementById(HeaderTotalLabelX);
                    footerTotalControl = document.getElementById(FooterTotalLabelX);
                }

                var unAssigned = parseInt(unAssignedControl.value);
                var newValue = parseInt(args.get_newValue());
                var oldValue = parseInt(args.get_oldValue());

                var tempTotal = 0;

                if (newValue >= 0) {
                    if (newValue < oldValue) {
                        tempTotal = unAssigned + (oldValue - newValue);
                        Debug("Adding:" + tempTotal);
                    }
                    else if (newValue > oldValue) {
                        tempTotal = unAssigned - (newValue - oldValue);
                        Debug("Subtracting:" + tempTotal);
                    }

                    if (tempTotal < 0) {
                        alert(exceedMessage);
                        args.set_cancel(true);
                    }
                    else {
                        unAssignedControl.value = tempTotal;
                        headerTotalControl.innerHTML = (maximum - tempTotal);
                        footerTotalControl.innerHTML = (maximum - tempTotal);

                        SetTotalAllAssigned();
                    }
                }
            }

            function Validate(control) {
                var value = document.getElementById(control).value;

                if (/^\d+$/.test(value)) {
                    document.getElementById(control).value = parseInt(value);

                    // Check if less than maximum
                    if (!ValidateTotalPerLane(control)) {
                        alert(exceedMessage);
                        document.getElementById(control).value = "0";
                    }
                }
                else {
                    alert(notIntegerMessage);
                    document.getElementById(control).value = "0";
                }

                CalculateSumPerOfficer(control);
                CalculateSum();
                SetTotalAllAssigned();
            }

            function ValidateTotalPerLane(lane) {
                var maximum = 0;
                var currentTotal = 0;

                CalculateSum();

                if (lane.indexOf("ATextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneAMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelA).innerHTML);
                }
                else if (lane.indexOf("BTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneBMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelB).innerHTML);
                }
                else if (lane.indexOf("CTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneCMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelC).innerHTML);
                }
                else if (lane.indexOf("DTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneDMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelD).innerHTML);
                }
                else if (lane.indexOf("ETextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneEMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelE).innerHTML);
                }
                else if (lane.indexOf("FTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneFMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelF).innerHTML);
                }
                else if (lane.indexOf("HTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneHMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelH).innerHTML);
                }
                else if (lane.indexOf("LTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneLMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelL).innerHTML);
                }
                else if (lane.indexOf("NTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneNMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelN).innerHTML);
                }
                else if (lane.indexOf("TTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneTMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelT).innerHTML);
                }
                else if (lane.indexOf("XTextBox") > -1) {
                    maximum = parseInt(document.getElementById("<%= LaneXMaxHiddenField.ClientID %>").value);
                    currentTotal = parseInt(document.getElementById(FooterTotalLabelX).innerHTML);
                }

                if (currentTotal > maximum) {
                    return false;
                }

                return true;
            }

            function SetTotalAllAssigned() {
                headerTotalControl = document.getElementById(HeaderTotalLabel);

                headerTotalControl.innerHTML = parseInt(document.getElementById(HeaderTotalLabelA).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelB).innerHTML) +
                        parseInt(document.getElementById(HeaderTotalLabelC).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelD).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelE).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelF).innerHTML) +
                        parseInt(document.getElementById(HeaderTotalLabelH).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelL).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelN).innerHTML) +
                        parseInt(document.getElementById(HeaderTotalLabelT).innerHTML) + parseInt(document.getElementById(HeaderTotalLabelX).innerHTML);

                footerTotalControl = document.getElementById(FooterTotalLabel);

                footerTotalControl.innerHTML = parseInt(document.getElementById(FooterTotalLabelA).innerHTML) + parseInt(document.getElementById(FooterTotalLabelB).innerHTML) +
                        parseInt(document.getElementById(FooterTotalLabelC).innerHTML) + parseInt(document.getElementById(FooterTotalLabelD).innerHTML) + parseInt(document.getElementById(FooterTotalLabelE).innerHTML) + parseInt(document.getElementById(FooterTotalLabelF).innerHTML) +
                        parseInt(document.getElementById(FooterTotalLabelH).innerHTML) + parseInt(document.getElementById(FooterTotalLabelL).innerHTML) + parseInt(document.getElementById(FooterTotalLabelN).innerHTML) +
                        parseInt(document.getElementById(FooterTotalLabelT).innerHTML) + parseInt(document.getElementById(FooterTotalLabelX).innerHTML);
            }

            function CalculateSum() {
                var sumLaneA = CalculateSumPerLane("ATextBox");
                var sumLaneB = CalculateSumPerLane("BTextBox");
                var sumLaneC = CalculateSumPerLane("CTextBox");
                var sumLaneD = CalculateSumPerLane("DTextBox");
                var sumLaneE = CalculateSumPerLane("ETextBox");
                var sumLaneF = CalculateSumPerLane("FTextBox");
                var sumLaneH = CalculateSumPerLane("HTextBox");
                var sumLaneL = CalculateSumPerLane("LTextBox");
                var sumLaneN = CalculateSumPerLane("NTextBox");
                var sumLaneT = CalculateSumPerLane("TTextBox");
                var sumLaneX = CalculateSumPerLane("XTextBox");

                // Header error only Windows Server 2003 but okay on Windows Server 2008
                document.getElementById(HeaderTotalLabelA).innerHTML = sumLaneA;
                document.getElementById(HeaderTotalLabelB).innerHTML = sumLaneB;
                document.getElementById(HeaderTotalLabelC).innerHTML = sumLaneC;
                document.getElementById(HeaderTotalLabelD).innerHTML = sumLaneD;
                document.getElementById(HeaderTotalLabelE).innerHTML = sumLaneE;
                document.getElementById(HeaderTotalLabelF).innerHTML = sumLaneF;
                document.getElementById(HeaderTotalLabelH).innerHTML = sumLaneH;
                document.getElementById(HeaderTotalLabelL).innerHTML = sumLaneL;
                document.getElementById(HeaderTotalLabelN).innerHTML = sumLaneN;
                document.getElementById(HeaderTotalLabelT).innerHTML = sumLaneT;
                document.getElementById(HeaderTotalLabelX).innerHTML = sumLaneX;

                // Footer
                document.getElementById(FooterTotalLabelA).innerHTML = sumLaneA;
                document.getElementById(FooterTotalLabelB).innerHTML = sumLaneB;
                document.getElementById(FooterTotalLabelC).innerHTML = sumLaneC;
                document.getElementById(FooterTotalLabelD).innerHTML = sumLaneD;
                document.getElementById(FooterTotalLabelE).innerHTML = sumLaneE;
                document.getElementById(FooterTotalLabelF).innerHTML = sumLaneF;
                document.getElementById(FooterTotalLabelH).innerHTML = sumLaneH;
                document.getElementById(FooterTotalLabelL).innerHTML = sumLaneL;
                document.getElementById(FooterTotalLabelN).innerHTML = sumLaneN;
                document.getElementById(FooterTotalLabelT).innerHTML = sumLaneT;
                document.getElementById(FooterTotalLabelX).innerHTML = sumLaneX;

            }

            function CalculateSumPerLane(laneTextBox) {
                var container = document.getElementById("RepeaterDiv");
                var inputs = container.getElementsByTagName("input");

                var sum = 0;

                for (var i = 0; i < inputs.length; i++) {
                    if (inputs[i].type === "text") {
                        if (inputs[i].id.indexOf(laneTextBox) > -1) {
                            sum += parseInt(inputs[i].value);
                        }
                    }
                }

                return sum;
            }

            function CalculateSumPerOfficer(control) {
                var labelName = "IndividualTotalLabel";
                var lastIndexOfUnderscore = control.lastIndexOf("_");
                var controlPrefix = control.substring(0, lastIndexOfUnderscore + 1);
                labelName = controlPrefix + labelName;

                var totalForOfficer = document.getElementById(labelName);

                if (totalForOfficer != null) {
                    var sum = 0;

                    sum += parseInt(document.getElementById(controlPrefix + "ATextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "BTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "CTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "DTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "ETextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "FTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "HTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "LTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "NTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "TTextBox").value);
                    sum += parseInt(document.getElementById(controlPrefix + "XTextBox").value);

                    totalForOfficer.innerHTML = sum;
                }
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ZoningPage.aspx.cs" Inherits="IncomeAssessment_ZoningPage"
    MasterPageFile="~/Main.master" Title="DWMS - Zoning Panel (Income Extraction)"
    EnableEventValidation="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="TallComponents.Web.PdfViewer" TagPrefix="tc" Namespace="TallComponents.Web.Pdf" %>
<%@ Register Src="~/Import/Controls/ScanUploadFields.ascx" TagName="ScanUploadFields"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/CopyItems.ascx" TagName="CopyItems" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/WebServices/IncomeExtractionWebSvr.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Skin="Windows7" Width="100%"
        Height="100%" OnClientLoaded="SetSplitterHeight" OnClientResized="SetSplitterHeight">
        <telerik:RadPane ID="TreeRadPane" runat="server" Width="100%" Height="100%" Scrolling="Both">
            <asp:Panel ID="LeftSubmitPanel" runat="server" Width="100%" Height="100%">
                <div class="subHeadingRight">
                    <div class="top10">
                    </div>
                    <asp:Label ID="ApplicantLabel" runat="server" Font-Bold="true" Text=""></asp:Label>&nbsp&nbsp
                    <asp:Button ID="ChangeButton" runat="server" Text="Change" CssClass="button-large2"
                        EnableViewState="true" CausesValidation="false" /><br />
                    <asp:Button ID="PreviousButton" runat="server" Text="Previous" CssClass="button-small"
                        CausesValidation="false" EnableViewState="true" OnClick="PreviousButton_Click" />&nbsp&nbsp
                    <asp:Button ID="NextButton" runat="server" Text="Next" CssClass="button-small" CausesValidation="false"
                        EnableViewState="true" OnClick="NextButton_Click" />&nbsp&nbsp
                </div>
                <iframe id="pdfframe" runat="server" width="100%" frameborder="0" height="100%">
                </iframe>
                <div class="top5">
                </div>
            </asp:Panel>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" CollapseMode="Both" Height="100%"
            Skin="Windows7">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PreviewRadPane" runat="server" Height="100%" Scrolling="Both">
            <asp:Panel ID="PanelRight" runat="server" Width="100%" BorderWidth="0">
                <div class="subHeadingLeft">
                    <table>
                        <tr>
                            <td>
                                <asp:Image ID="CopyButton" runat="server" ImageUrl="~/Data/Images/Copy.png" />
                            </td>
                            <td>
                                <asp:Image ID="ArrowDivideButton" runat="server" ImageUrl="~/Data/Images/arrow_divide.png" />
                            </td>
                            <td>
                                <asp:ImageButton ID="DeleteTemp" runat="server" ImageUrl="~/Data/Images/deletered.png"
                                    ToolTip="Delete All Selected Items" />
                            </td>
                            <td>
                                <%--<asp:ImageButton ID="IncomeBlank" runat="server" ImageUrl="~/Data/Images/IncomeBlank.png"
                                    ToolTip="Set Income to Blank" OnClientClick="SetIncomeToBlank();return false;" />--%>
                                    <asp:Image ID="IncomeBlank" runat="server" ImageUrl="~/Data/Images/IncomeBlank.png" ToolTip="Set Income to Blank" />
                            </td>
                            <td>
                                <asp:ImageButton ID="HistoryButton" runat="server" ImageUrl="~/Data/Images/ViewLog32.png"
                                    ToolTip="View Log" />
                            </td>
                            <td>
                                <asp:ImageButton ID="VersionButton" runat="server" ImageUrl="~/Data/Images/VersionName32.png"
                                    ToolTip="View/Edit Versions" />
                            </td>
                            <td>
                                <telerik:RadComboBox runat="server" ID="VersionDropdownList" Skin="Windows7" AutoPostBack="true"
                                    Width="70px" DropDownWidth="150px" Height="300px" MarkFirstMatch="true" CausesValidation="false"
                                    Visible="true" OnSelectedIndexChanged="VersionDropdownList_SelectedIndexChanged" />
                            </td>
                            <td>
                                <telerik:RadComboBox runat="server" ID="TemplateDropdownList" Skin="Windows7" AutoPostBack="true"
                                    Width="100px" DropDownWidth="100px" Height="300px" MarkFirstMatch="true" CausesValidation="false"
                                    Visible="true" OnSelectedIndexChanged="VersionDropdownList_SelectedIndexChanged" />
                            </td>
                            <td>
                                <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="button-large2" EnableViewState="true"
                                    CausesValidation="false" OnClientClick="javascript:InsertOrUpdateIncomeDetails(); return false;" />
                            </td>
                            <td>
                                <%--   <asp:Panel ID="MetDataConfirmPanel" runat="server" CssClass="reminder-green top5 bottom10"
                                        Visible="false" EnableViewState="false">--%>
                                <asp:Label ID="MetDataConfirmPanel" runat="server" Text="" CssClass="reminder-transparent"></asp:Label>
                                <%--                                  </asp:Panel>--%>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <asp:Repeater runat="server" ID="TableMonthRepeater" OnItemDataBound="TableMonthRepeater_ItemDataBound"
                        OnItemCommand="TableMonthRepeater_ItemCommand">
                        <ItemTemplate>
                            <asp:Table runat="server" ID="MonthTable" Width="100%" CssClass="metaform-zone" ondragover="dragoveHandler(event)">
                            </asp:Table>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:HiddenField runat="server" ID="HiddenClientId" />
                    <asp:HiddenField runat="server" ID="HiddenForex" />
                    <asp:HiddenField ID="StoreIncomeId" runat="server" />
                    <asp:HiddenField ID="HiddenIncomeId" runat="server" />
                    <asp:HiddenField ID="HiddenToDeleteIncomeDetails" runat="server" />
                    <asp:HiddenField ID="HiddenMonthFrom" runat="server" />
                    <asp:HiddenField ID="HiddenSection" runat="server" />
                </div>
                <div class="subHeadingLeft">
                    <table>
                        <tr>
                            <td>
                                <asp:Image ID="CopyButton2" runat="server" ImageUrl="~/Data/Images/Copy.png" />
                            </td>
                            <td>
                                <asp:Image ID="ArrowDivideButton2" runat="server" ImageUrl="~/Data/Images/arrow_divide.png" />
                            </td>
                            <td>
                                <asp:ImageButton ID="DeleteTemp2" runat="server" ImageUrl="~/Data/Images/deletered.png"
                                    ToolTip="Delete All Selected Items" />
                            </td>
                            <td>
                                <asp:ImageButton ID="IncomeBlank2" runat="server" ImageUrl="~/Data/Images/IncomeBlank.png"
                                    ToolTip="Set Income to Blank" OnClientClick="SetIncomeToBlank();return false;" />
                            </td>
                            <td>
                                <asp:ImageButton ID="HistoryButton2" runat="server" ImageUrl="~/Data/Images/ViewLog32.png"
                                    ToolTip="View Log" />
                            </td>
                            <td>
                                <asp:ImageButton ID="VersionButton2" runat="server" ImageUrl="~/Data/Images/VersionName32.png"
                                    ToolTip="View/Edit Versions" />
                            </td>                            
                            <td>
                                <asp:Button ID="SaveButton2" runat="server" Text="Save" CssClass="button-large2" EnableViewState="true"
                                    CausesValidation="false" OnClientClick="javascript:InsertOrUpdateIncomeDetails(); return false;" />
                            </td>
                            <td>
                                <%--   <asp:Panel ID="MetDataConfirmPanel" runat="server" CssClass="reminder-green top5 bottom10"
                                        Visible="false" EnableViewState="false">--%>
                                <asp:Label ID="MetDataConfirmPanel2" runat="server" Text="" CssClass="reminder-transparent"></asp:Label>
                                <%--                                  </asp:Panel>--%>
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>
    <asp:HiddenField runat="server" ID="NodeValueHiddenField" EnableViewState="false"
        Value="-1" />
    <asp:HiddenField runat="server" ID="NodeCategoryHiddenField" EnableViewState="false"
        Value="-1" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" ClientEvents-OnRequestStart="RequestStarted"
        EnableAJAX="true" DefaultLoadingPanelID="LoadingPanel1" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipYear" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="VersionDropdownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipYear" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="MonthYearDropDownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="IncomeDetailsRadGrid">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="IncomeDetailsRadGrid" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="PreviousButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipYear" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
    <telerik:RadToolTipManager ID="RadToolTipManager1" HideEvent="ManualClose" Width="500"
        Height="300" runat="server" RelativeTo="Mouse" Position="BottomRight" Skin="Windows7"
        Animation="None" Modal="True" OnAjaxUpdate="OnAjaxUpdate">
    </telerik:RadToolTipManager>
    
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var myHeight = getClientHeight();

            function SetSplitterHeight(splitter, args) {
                var myHeight = getClientHeight();
                var height = myHeight - 97;

                var PreviewRadPane = splitter.getPaneById("<%= PreviewRadPane.ClientID %>");
                PreviewRadPane.set_height(height);

                //            

                var TreeRadPane = splitter.getPaneById("<%= TreeRadPane.ClientID %>");
                TreeRadPane.set_height(height);

                //Added By Edward 2014/5/21 for Auto width of PDF pane
                var myWidth = getClientWidth();
                TreeRadPane.set_width(myWidth * .5);
                PreviewRadPane.set_width(myWidth * .5);

                splitter.set_height(height);
                SetHeightForControls()
            }

            function SetTreeRadPaneWidth(splitter) {

            }

            function SetInnerSplitterHeight(splitter, args) {
                var myHeight = getClientHeight();
                var height = myHeight - 167;
                var frame = document.getElementById("<%= pdfframe.ClientID %>");
                if (frame) {
                    frame.style.height = height + "px";
                }

                splitter.set_height(height);
            }

            function SetHeightForControls() {
                var myHeight = getClientHeight();
                var height = (myHeight - 172) + "px";
            }

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

            function SetPDFSize(w, h) {
                var splitter = $find("<%= RadSplitter1.ClientID %>");
                splitter.set_height(h);
            }

            function OnNodeExpanded(sender, args) {
                var category = args.get_node().get_category();
                var expandedNode = args.get_node();

                if (category == "Set") {
                    //CollapseSiblings(expandedNode);
                }
            }

            function rtvExplore_OnNodeExpandedCollapsed(sender, eventArgs) {
                var allNodes = eventArgs._node.get_treeView().get_allNodes();
                var i;
                var unSelectedNodes = "";

                for (i = 0; i < allNodes.length; i++) {
                    if (!allNodes[i].get_expanded())
                        unSelectedNodes += allNodes[i].get_value() + "*";
                }

            }

            function RefreshUrl(url) {
                window.location = url;
            }

            function CollapseSiblings(expandedNode) {

                var nodes = tree.get_nodes();
                var siblingsCount = nodes.get_count();

                for (var nodeIndex = 0; nodeIndex < siblingsCount; nodeIndex++) {
                    var siblingNode = nodes.getNode(nodeIndex);

                    if (siblingNode.get_category() == "Set") {
                        if ((siblingNode != expandedNode) && (siblingNode.get_expanded())) {
                            siblingNode.collapse();
                        }
                    }
                }
            }



            function RequestStarted(ajaxManager, eventArgs) {

                if (eventArgs.get_eventTarget().indexOf("ExportButton") != -1) {
                    eventArgs.set_enableAjax(false);
                }
                else {
                    eventArgs.set_enableAjax(true);
                }
            }

            var _currentNode;
            function treeView_MouseOut(sender, args) {
                _currentNode = null;
            }

            function treeView_MouseOver(sender, args) {
                _currentNode = args.get_node();
            }

            function reloadPage() {
                window.location.reload()
            }

            var hasChanges, inputs, dropdowns, editedRow;

            function GridCommand(sender, args) {
                alert("grid command");
                if (args.get_commandName() != "Edit") {
                    editedRow = null;
                }
            }

            function GridCreated(sender, eventArgs) {

                var gridElement = sender.get_element();
                var elementsToUse = [];
                inputs = gridElement.getElementsByTagName("input");
                for (var i = 0; i < inputs.length; i++) {
                    var lowerType = inputs[i].type.toLowerCase();
                    if (lowerType == "hidden" || lowerType == "button") {
                        continue;
                    }
                    if (inputs[i].id.indexOf("PageSizeComboBox") == -1 && inputs[i].type.toLowerCase() != "checkbox") {
                        Array.add(elementsToUse, inputs[i]);
                    }
                    inputs[i].onchange = TrackChanges;
                }

                dropdowns = gridElement.getElementsByTagName("select");
                for (var i = 0; i < dropdowns.length; i++) {
                    dropdowns[i].onchange = TrackChanges;
                }

                setTimeout(function () { if (elementsToUse[0]) elementsToUse[0].focus(); }, 100);
            }

            function TrackChanges(e) {
                hasChanges = true;
            }

            function SetText(option) {
                if (option == 'replace') {
                    if (confirm("Replace all items?")) {
                        var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                        ajaxManager.ajaxRequest(option);
                    }
                }
                else {
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest(option);
                }
            }

            function DivideAssessment() {
                if (confirm("This will divide the items. Do you want to continue?")) {

                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest('divide');
                }
            }

            //Added by Edward 30/01/2014 Navigation icons for Sales and Resales Changes
            function Navigating(action) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(action);

            }

            function SetIncomeToBlank(IdsToBeBlanked, AllIds) {
                if (confirm("Do you want to set the Income to blank?")) {
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest('blank;' + IdsToBeBlanked + ";" + AllIds);
                }
            }

            function AddingRows(incomeId, clientIdOfMonthTable, forex) {
                var table = document.getElementById(clientIdOfMonthTable);

                var rowCount = table.rows.length;
                var row = table.insertRow(rowCount - 2);

                var colCount = table.rows[0].cells.length;
                var html;
                if (table.rows[rowCount].cells[1].childNodes[0].value == "") {
                    alert("No Item");
                    table.deleteRow(rowCount - 2);
                    return;
                }
                if (table.rows[rowCount].cells[2].childNodes[0].value == "") {
                    alert("No Amount");
                    table.deleteRow(rowCount - 2);
                    return;
                }
                if (isNaN(table.rows[rowCount].cells[2].childNodes[0].value.replace(',',''))) {
                    alert("Invalid Amount");
                    table.deleteRow(rowCount - 2);
                    return;
                }


                for (var i = 0; i < colCount; i++) {

                    var newcell = row.insertCell(i);

                    if (i == 0) {
                        var chkbox = document.createElement('input');
                        chkbox.type = "checkbox";
                        chkbox.id = "RowNumberCheckBox" + incomeId;
                        newcell.appendChild(chkbox);
                        newcell.align = "center";
                    }
                    else if (i == 1) {
                        newcell.innerHTML = table.rows[rowCount].cells[i].innerHTML;
                        newcell.childNodes[0].value = table.rows[rowCount].cells[i].childNodes[0].value;
                        table.rows[rowCount].cells[i].childNodes[0].value = "";
                    }
                    else if (i == 2) {
                        html = "<INPUT Id=\"RowAmount" + incomeId + "\" style=\"WIDTH: 100%\" ";
                        html = html + "onBlur=\"CalculateCheckboxes('" + clientIdOfMonthTable + "','" + forex + "')\" ";
                        html = html + "value=\"" + table.rows[rowCount].cells[i].childNodes[0].value + "\" >";
                        newcell.innerHTML = html;
                        newcell.childNodes[0].value = table.rows[rowCount].cells[i].childNodes[0].value.replace(',', '');
                        newcell.childNodes[0].style.textAlign = "right";
                        table.rows[rowCount].cells[i].childNodes[0].value = "";
                    }
                    else if (i == 3) {
                        newcell.innerHTML = ReturnHTMLForCheckBox("RowGrossCheckBox" + incomeId, clientIdOfMonthTable, forex, rowCount - 2, "gross", table.rows[rowCount].cells[i].childNodes[0].checked);
                        newcell.align = "center";
                        table.rows[rowCount].cells[i].childNodes[0].checked = false;
                    }
                    else if (i == 4) {
                        newcell.innerHTML = ReturnHTMLForCheckBox("RowAllceCheckBox" + incomeId, clientIdOfMonthTable, forex, rowCount - 2, "allce", table.rows[rowCount].cells[i].childNodes[0].checked);
                        newcell.align = "center";
                        table.rows[rowCount].cells[i].childNodes[0].checked = false;
                    }
                    else if (i == 5) {
                        newcell.innerHTML = ReturnHTMLForCheckBox("RowOTCheckBox" + incomeId, clientIdOfMonthTable, forex, rowCount - 2, "ot", table.rows[rowCount].cells[i].childNodes[0].checked);
                        newcell.align = "center";
                        table.rows[rowCount].cells[i].childNodes[0].checked = false;
                    }
                    else if (i == 6) {
                        var strHtmlUp = "<img src=\"../Data/Images/navUpD.png\" style=\"border-width:0px;\" Id=\"RowNavUp" + incomeId + "\" />";
                        var strHtmlDown = "<img src=\"../Data/Images/navDownD.png\" style=\"border-width:0px;\" Id=\"RowNavDown" + incomeId + "\" />";
                        var strHtml5 = strHtmlUp + strHtmlDown + "<INPUT TYPE=\"Button\" CLASS=\"button-zone\" onClick=\"DeleteCurrentRow('" + clientIdOfMonthTable + "','" + forex + "')\" VALUE=\"X\" Id=\"RowDelete" + incomeId + "\" />";
                        var strHtmlIncomeDetailsId = strHtml5 + "<input type=\"hidden\" value=\"0\" Id=\"RowHiddenIncomeDetailId" + incomeId + "\" />";
                        newcell.innerHTML = strHtmlIncomeDetailsId;
                    }
                }
                CalculateCheckboxes(clientIdOfMonthTable, forex);
                table.rows[rowCount].cells[2].childNodes[0].value = 0;
            }

            function CalculateCheckboxes(clientIdOfMonthTable, forex) {
                var strSection = document.getElementById("<%= HiddenSection.ClientID %>").value;
                var table = document.getElementById(clientIdOfMonthTable);
                var rows = table.rows, rowcount = rows.length, r, cells, cellcount, c, cell;
                var gross = parseFloat(0), allce = parseFloat(0), OT = parseFloat(0);
                var grossEx = parseFloat(0), allceEx = parseFloat(0), OTEx = parseFloat(0);
                var num = "";
                for (r = 1; r < rowcount; r++) {
                    cells = rows[r].cells;                    
                    if (cells[3].childNodes[0].checked == true) {                        
                        gross = gross + parseFloat(cells[2].childNodes[0].value.replace(',', ''));
                    }
                    if (cells[4].childNodes[0].checked == true) {
                        allce = allce + parseFloat(cells[2].childNodes[0].value.replace(',', ''));
                    }
                    if (strSection == "COS") {
                        if (cells[5].childNodes[0].checked == true) {
                            OT = OT + parseFloat(cells[2].childNodes[0].value.replace(',', ''));
                        }
                    }
                }
                grossEx = gross / forex;
                allceEx = allce / forex;
                if (strSection == "COS")
                    OTEx = OT / forex;
                var rowCount = table.rows.length;
                rows[rowcount - 2].cells[3].innerHTML = "<b>" + gross.toFixed(2) + "</b><br><b>" + grossEx.toFixed(2) + "</b>";
                rows[rowcount - 2].cells[4].innerHTML = "<b>" + allce.toFixed(2) + "</b><br><b>" + allceEx.toFixed(2) + "</b>";
                if (strSection == "COS")
                    rows[rowcount - 2].cells[5].innerHTML = "<b>" + OT.toFixed(2) + "</b><br><b>" + OTEx.toFixed(2) + "</b>";
            }

            function SelectCheckBoxes(clientIdOfMonthTable) {
                var table = document.getElementById(clientIdOfMonthTable);
                var rows = table.rows;
                var rowCount = table.rows.length - 2;
                for (r = 1; r < rowCount; r++) {
                    cells = rows[r].cells;
                    cells[0].childNodes[0].checked = rows[0].cells[0].childNodes[0].checked;
                }
            }

            function DeleteCurrentRow(clientIdOfMonthTable, forex) {
                var table = document.getElementById(clientIdOfMonthTable);                
                var strToDelete = document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>");
                var current = window.event.srcElement;                
                //here we will delete the line
                while ((current = current.parentElement) && current.tagName != "TR");
                document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>").value = document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>").value + current.cells[6].childNodes[3].value + ";";
                current.parentElement.removeChild(current);                
                ResetTableAfterDelete(clientIdOfMonthTable, forex);
                CalculateCheckboxes(clientIdOfMonthTable, forex);
            }

            function DeleteAllSelectedRows() {
                //if (confirm("Do you want to delete the selected items?")) {
                var table, rows, current;
                var rowCount;
                var strClient = document.getElementById("<%= HiddenClientId.ClientID %>").value;
                var splitClient = strClient.split(",");
                var strForex = document.getElementById("<%= HiddenForex.ClientID %>").value;
                var splitForex = strForex.split(",");                
                for (i = 0; i < splitClient.length - 1; i++) {
                    table = document.getElementById(splitClient[i]);
                    rows = table.rows;
                    rowCount = table.rows.length - 2;
                    for (r = 1; r < rowCount; r++) {                        
                        cells = rows[r].cells;
                        if (cells[0].childNodes[0].checked == true) {
                            document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>").value = document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>").value + cells[6].childNodes[3].value + ";";
                            table.deleteRow(r);
                            rowCount--;
                            r = 0;
                            CalculateCheckboxes(splitClient[i], splitForex[i]);
                        }
                    }
                    ResetTableAfterDelete(splitClient[i], splitForex[i]);
                }
                //}
            }

            //this function resets the table after each transaction like copy, divide and delete
            function ResetTableAfterDelete(clientIdOfMonthTable, forex) {
                var table = document.getElementById(clientIdOfMonthTable);
                var rows = table.rows;
                var rowCount = table.rows.length - 2;
                var cells;
                var colCount;
                var CHECK;
                for (r = 1; r < rowCount; r++) {
                    cells = rows[r].cells;                    
                    CHECK = cells[3].childNodes[0].checked;
                    cells[3].innerHTML = ReturnHTMLForCheckBox("RowGrossCheckBox" + r, clientIdOfMonthTable, forex, r, "gross", CHECK);
                    cells[3].childNodes[0].checked = CHECK;                    

                    CHECK = cells[4].childNodes[0].checked;
                    cells[4].innerHTML = ReturnHTMLForCheckBox("RowAllceCheckBox" + r, clientIdOfMonthTable, forex, r, "allce", CHECK);
                    cells[4].childNodes[0].checked = CHECK;

                    CHECK = cells[5].childNodes[0].checked;
                    cells[5].innerHTML = ReturnHTMLForCheckBox("RowOTCheckBox" + r, clientIdOfMonthTable, forex, r, "ot", CHECK); ;
                    cells[5].childNodes[0].checked = CHECK;
                }
            }

            //This is another approach, trying to do all one time
            function ReturnHTMLForCheckBox(checkBoxId, clientIdOfMonthTable, forex, indexNo, checkBoxType, checkBoxValue) {
                var html;
                html = "<INPUT TYPE=\"Checkbox\" Id=\"" + checkBoxId + "\" ";
                html = html + "onClick=\"CalculateCheckboxes('" + clientIdOfMonthTable + "','" + forex + "');";
                html = html + "ValidateCheckboxes('" + clientIdOfMonthTable + "','" + indexNo + "','" + forex + "','" + checkBoxType + "')\" ";
                html = html + (checkBoxValue === true ? " checked " : "") + ">";
                return html;
            }

            function ReturnHTMLForNavigation(incomeId, clientId, forex) {
                var html;
                var strHtmlUp = "<img src=\"../Data/Images/navUpD.png\" style=\"border-width:0px;\ Id=\"RowNavUp" + incomeId + "\" />";
                var strHtmlDown = "<img src=\"../Data/Images/navDownD.png\" style=\"border-width:0px;\ Id=\"RowNavDown" + incomeId + "\" />";
                var strHtml5 = strHtmlUp + strHtmlDown + "<INPUT TYPE=\"Button\" CLASS=\"button-zone\" onClick=\"DeleteCurrentRow('" + clientId + "','" + forex + "')\" VALUE=\"X\" Id=\"RowDelete" + incomeId + "\" />";
                html = strHtml5 + "<input type=\"hidden\" value=\"0\"  Id=\"RowHiddenIncomeDetailId" + incomeId + "\" />";
                return html;
            }

            function InsertOrUpdateIncomeDetails() {
                var table, rows, rowCount;
                var versionNoFromDropDown = document.getElementById("<%= VersionDropdownList.ClientID %>").value;
                var strClient = document.getElementById("<%= HiddenClientId.ClientID %>").value;
                var strToDelete = document.getElementById("<%= HiddenToDeleteIncomeDetails.ClientID %>").value;
                var splitClient = strClient.split(",");
                var item, amount, incomeDetailsId, gross, allowance, OT, CPF, AHG;
                var strIncomeId = "";
                var strIncomeDetails;
                var strArrayIncomeDetails;
                var strSaving = document.getElementById("<%= MetDataConfirmPanel.ClientID %>");
                var strSaving2 = document.getElementById("<%= MetDataConfirmPanel2.ClientID %>");
                var strSection = document.getElementById("<%= HiddenSection.ClientID %>").value;
                strSaving.innerHTML = "Saving...";
                strSaving2.innerHTML = "Saving...";
                //Concatenate all IncomeIds
                for (i = 0; i < splitClient.length - 1; i++) {
                    table = document.getElementById(splitClient[i]);
                    rows = table.rows;
                    rowCount = table.rows.length - 2;
                    strIncomeId = strIncomeId + rows[0].cells[1].childNodes[1].value + ",";           //This child contains the HiddenField of IncomeId for each Month //Concatenate into a string all the IncomeIds
                    strArrayIncomeDetails = "";
                    for (r = 1; r < rowCount; r++) {
                        strIncomeDetails = "";
                        cells = rows[r].cells;
                        item = cells[1].childNodes[0].value;
                        amount = cells[2].childNodes[0].value;
                        gross = (cells[3].childNodes[0].checked == true ? "T" : "F");
                        allowance = (cells[4].childNodes[0].checked == true ? "T" : "F");
                        if (strSection == "COS") {
                            allowance = (cells[4].childNodes[0].checked == true ? "T" : "F");
                            OT = (cells[5].childNodes[0].checked == true ? "T" : "F");
                            AHG = "F";      //False for COS
                        }
                        else {
                            allowance = "F";
                            OT = "F";
                            AHG = (cells[4].childNodes[0].checked == true ? "T" : "F");
                        }
                        CPF = "F";      //False for COS
                        if (strSection == "COS") 
                            incomeDetailsId = cells[6].childNodes[3].value;                        
                        else 
                            incomeDetailsId = cells[5].childNodes[3].value;                        
                        strIncomeDetails = item + "|" + amount + "|" + gross + "|" + allowance + "|" + OT + "|" + CPF + "|" + AHG + "|" + incomeDetailsId;
                        strArrayIncomeDetails = strArrayIncomeDetails + strIncomeDetails + ";";
                    }
                    //Call Webservice
                    IncomeExtractionWebSvr.InsertUpdateIncomeDetails(rows[0].cells[1].childNodes[1].value, strArrayIncomeDetails, versionNoFromDropDown, strToDelete, SucceededInsertUpdateIncomeDetailsCallback, FailedInsertUpdateIncomeDetailsCallback);
                }
            }

            function SucceededInsertUpdateIncomeDetailsCallback(count) {
                if (count == "1")
                    alert("Operation failed. Please try again.");
                else {
                    var versionNoFromDropDown = document.getElementById("<%= VersionDropdownList.ClientID %>").value;
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest('versionNo' + versionNoFromDropDown);
                }
            }

            function FailedInsertUpdateIncomeDetailsCallback(error) {
                alert("Error in saving zoning page.");
            }

            function swapRows(MonthTableClientId, swap_index, with_index) {
                // this method wont work with nested tables - which are bad form anyway
                table = document.getElementById(MonthTableClientId)
                var trs = table.getElementsByTagName("tr");
                if (swap_index >= trs.length || with_index >= trs.length) return false;
                var old_rows = new Array();
                var new_rows = new Array();
                for (var i = 0; i < trs.length; i++) {
                    old_rows.push(trs[i]);
                    new_rows.push(trs[i]);
                }
                var row1 = trs[swap_index];
                var row2 = trs[with_index];
                new_rows.splice(swap_index, 1, row2);
                new_rows.splice(with_index, 1, row1);
                var parent = trs[0].parentNode;
                for (var i = 0; i < old_rows.length; i++) parent.removeChild(old_rows[i]);
                for (var i = 0; i < old_rows.length; i++) parent.appendChild(new_rows[i]);
            }

            function CancelActiveToolTip(clientIdOfMonthTable, clientIdToBeCopied) {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();
            }

            function CloseActiveToolTip(clientIdOfMonthTable, clientIdToBeCopied, includeAmount) {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();
                CopyItems(clientIdOfMonthTable, clientIdToBeCopied, includeAmount);
            }

            function CloseActiveToolTipDivide(clientIdOfMonthTable) {
                if (confirm("This will divide the items. Do you want to continue?")) {
                    var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                    if (tooltip) tooltip.hide();
                    DivideItems(clientIdOfMonthTable);
                }
            }

            function CloseActiveToolTipBlank(clientIdToBeBlank) {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();
                SetSelectedIncomeToBlank(clientIdToBeBlank);
            }

            function ClearCheckBoxes() {
                var table, rows, rowCount;
                var strClient = document.getElementById("<%= HiddenClientId.ClientID %>").value;
                var splitClient = strClient.split(",");
                for (i = 0; i < splitClient.length - 1; i++) {
                    table = document.getElementById(splitClient[i]);
                    rows = table.rows;
                    rowCount = table.rows.length - 2;                                        
                    for (r = 0; r < rowCount; r++) {
                        rows[r].cells[0].childNodes[0].checked = false;                                              
                    }                    
                }
            }


            function CopyItems(clientIdOfMonthTable, clientIdToBeCopied, includeAmount) {
                var table, rowFromCount, row, colCount, rowTableCount, newcell;
                var tableFrom = document.getElementById(clientIdOfMonthTable);
                var strForex = document.getElementById("<%= HiddenForex.ClientID %>").value;
                var splitForex = strForex.split(",");
                var strIncomeId = document.getElementById("<%= HiddenIncomeId.ClientID %>").value;
                var splitIncomeId = strIncomeId.split(",");
                var splitClientIdToBeCopied = clientIdToBeCopied.split(";");
                var strSection = document.getElementById("<%= HiddenSection.ClientID %>").value;
                rowFromCount = tableFrom.rows.length;                
                for (x = 0; x < splitClientIdToBeCopied.length; x++) {                    
                    if (splitClientIdToBeCopied[x] != clientIdOfMonthTable) {
                        table = document.getElementById(splitClientIdToBeCopied[x]);
                        rowTableCount = table.rows.length;
                        for (j = 1; j < tableFrom.rows.length - 2; j++) {
                            if (tableFrom.rows[j].cells[0].childNodes[0].checked == true) {
                                row = table.insertRow(rowTableCount - 2);

                                rowTableCount++;        //When a row is inserted, increase the rowCount for that table
                                if (strSection == "COS")
                                    colCount = 7;           //Number of Columns in a table
                                else
                                    colCount = 6;

                                LoopThroughColumns("copy", colCount, newcell, row, splitIncomeId[x], splitClientIdToBeCopied[x], splitForex[x], rowTableCount, tableFrom, includeAmount, strSection, 0, item);
                            }
                        }
                        ResetTableAfterDelete(splitClientIdToBeCopied[x], splitForex[x]);
                        CalculateCheckboxes(splitClientIdToBeCopied[x], splitForex[x]);
                    }
                }
                ClearCheckBoxes();          //Clears checkboxes for selection after copying
                InsertOrUpdateIncomeDetails();
            }


            function DivideItems(clientIdOfMonthTable) {                
                var table, rowFromCount, row, colCount, rowTableCount, newcell;
                var tableFrom = document.getElementById(clientIdOfMonthTable);
                var strForex = document.getElementById("<%= HiddenForex.ClientID %>").value;
                var splitForex = strForex.split(",");
                var strIncomeId = document.getElementById("<%= HiddenIncomeId.ClientID %>").value;
                var splitIncomeId = strIncomeId.split(",");
                var strClient = document.getElementById("<%= HiddenClientId.ClientID %>").value;
                var splitClient = strClient.trim().substring(0, strClient.length - 1).split(",");
                var strSection = document.getElementById("<%= HiddenSection.ClientID %>").value;
                var forexFrom;
                rowFromCount = tableFrom.rows.length;
                for (x = 0; x < splitClient.length; x++) {

                    if (splitClient[x] != clientIdOfMonthTable) {
                        table = document.getElementById(splitClient[x]);
                        rowTableCount = table.rows.length;
                        for (j = 1; j < tableFrom.rows.length - 2; j++) {
                            row = table.insertRow(rowTableCount - 2);

                            rowTableCount++;        //When a row is inserted, increase the rowCount for that table
                            if (strSection == "COS")
                                colCount = 7;           //Number of Columns in a table
                            else
                                colCount = 6;

                            LoopThroughColumns("divide", colCount, newcell, row, splitIncomeId[x], splitClient[x], splitForex[x], rowTableCount, tableFrom, "", strSection, splitClient.length, item);
                        }
                        ResetTableAfterDelete(splitClient[x], splitForex[x]);
                        CalculateCheckboxes(splitClient[x], splitForex[x]);
                    }
                    else
                        forexFrom = splitForex[x];
                }
                for (j = 1; j < tableFrom.rows.length - 2; j++) {
                    tableFrom.rows[j].cells[2].childNodes[0].value = (parseFloat(tableFrom.rows[j].cells[2].childNodes[0].value.replace(',', '')) / splitClient.length).toFixed(2);
                }
                CalculateCheckboxes(clientIdOfMonthTable, forexFrom);
                InsertOrUpdateIncomeDetails();                
            }            
            //this function is triggered when Textarea is out of focus
            function ClearHTA(clientIdOfMonthTable, forex, incomeId) {
                var table = document.getElementById(clientIdOfMonthTable);
                var splitLineBreak = table.rows[table.rows.length - 2].cells[1].childNodes[0].value.trim().split("\n");
                var newcell;
                var strSection = document.getElementById("<%= HiddenSection.ClientID %>").value;
                if (table.rows[table.rows.length - 2].cells[1].childNodes[0].value.trim().length > 0) {
                    for (index = 0; index < splitLineBreak.length; index++) {
                        var splitSpace = splitLineBreak[index].trim().split(" ");                        
                        var parsed = parseFloat(splitSpace[splitSpace.length - 1].replace(',',''));
                        if (isNaN(parsed) == false) {
                            var item = "";
                            for (j = 0; j < splitSpace.length - 1; j++) {
                                item = item + splitSpace[j] + " ";
                            }                            
                        }
                        else {
                            var item = "";
                            for (j = 0; j < splitSpace.length; j++) {
                                item = item + splitSpace[j] + " ";
                            }
                            parsed = 0;                            
                        }
                        var table1 = document.getElementById(clientIdOfMonthTable);
                        var rowCount = table1.rows.length;
                        var row = table1.insertRow(rowCount - 2);
                        var colCount = table.rows[0].cells.length;
                        //alert(item);
                        LoopThroughColumns("clear", colCount, newcell, row, incomeId, clientIdOfMonthTable, forex, rowCount, table1, "", strSection, parsed, item);
                        
                        CalculateCheckboxes(clientIdOfMonthTable, forex);
                    }
                    table.rows[table.rows.length - 2].cells[1].childNodes[0].value = "";
                }
            }

            function SetSelectedIncomeToBlank(clientIdToBeCopied) {                
                var allClientId = document.getElementById("<%= HiddenClientId.ClientID %>").value;
                var splitAllClientId = allClientId.substring(0, allClientId.length - 1).split(",");
                var table, rowFromCount, row, colCount, rowTableCount, newcell;
                var strIncomeId = document.getElementById("<%= HiddenIncomeId.ClientID %>").value;
                var splitIncomeId = strIncomeId.substring(0, strIncomeId.length - 1).split(",");
                var splitClientIdToBeCopied = clientIdToBeCopied.split(";");
                var IdsToBeBlanked = "";
                for (x = 0; x < splitAllClientId.length; x++) {
                    for (y = 0; y < splitClientIdToBeCopied.length; y++) {
                        if (splitClientIdToBeCopied[y] == splitAllClientId[x]) {
                            IdsToBeBlanked = IdsToBeBlanked + splitIncomeId[x] + ",";
                            break;
                        }
                    }
                }
                SetIncomeToBlank(IdsToBeBlanked.substring(0, IdsToBeBlanked.length - 1), splitIncomeId);                
            }

            //mode=copy,divide,clear ; colcount=column count; newcell; row=
            function LoopThroughColumns(mode, colCount, newcell, row, incomeId, clientId, forex, rowTableCount, tableFrom, includeAmount, strSection, anyNumber, anyString) {
                var tableRowCheckBox; //true or false
              
                for (i = 0; i < colCount; i++) {
                    newcell = row.insertCell(i);
                    if (mode != "clear")
                        tableRowCheckBox = tableFrom.rows[j].cells[i].childNodes[0].checked;
                    else
                        tableRowCheckBox = tableFrom.rows[rowTableCount].cells[i].childNodes[0].checked;
                    if (i == 0) {
                        var chkbox = document.createElement('input');
                        chkbox.type = "checkbox";
                        chkbox.id = "RowNumberCheckBox" + incomeId;
                        newcell.appendChild(chkbox);
                        newcell.align = "center";
                    }
                    else if (i == 1) {
                        if (mode != "clear") {
                            newcell.innerHTML = tableFrom.rows[j].cells[i].innerHTML;
                            newcell.childNodes[0].value = tableFrom.rows[j].cells[i].childNodes[0].value;
                        }
                        else {
                            newcell.innerHTML = tableFrom.rows[rowTableCount].cells[i].innerHTML;
                            newcell.childNodes[0].value = anyString;    //anyString 
                        }
                    }
                    else if (i == 2) {
                        if (mode == "copy") {
                            newcell.innerHTML = "<INPUT Id=\"RowAmount" + incomeId + "\" onBlur=\"CalculateCheckboxes('" + clientId + "','" + forex + "')\" style=\"WIDTH: 100%\" value=\"" + tableFrom.rows[j].cells[i].childNodes[0].value + "\" >";
                            if (includeAmount == "y")
                                newcell.childNodes[0].value = tableFrom.rows[j].cells[i].childNodes[0].value;
                            else if (includeAmount != "y")
                                newcell.childNodes[0].value = "0";
                        }
                        else if (mode == "divide") {
                            newcell.innerHTML = "<INPUT Id=\"RowAmount" + incomeId + "\" onBlur=\"CalculateCheckboxes('" + clientId + "','" + forex + "')\" style=\"WIDTH: 100%\" value=\"" + tableFrom.rows[j].cells[i].childNodes[0].value + "\" >";
                            newcell.childNodes[0].value = (parseFloat(tableFrom.rows[j].cells[i].childNodes[0].value.replace(',', '')) / anyNumber).toFixed(2);   //anyNumber is number of months to divide
                        }
                        else if (mode == "clear") {
                            newcell.innerHTML = "<INPUT Id=\"RowAmount" + incomeId + "\" onBlur=\"CalculateCheckboxes('" + clientId + "','" + forex + "')\" style=\"WIDTH: 100%\" value=\"" + tableFrom.rows[rowTableCount].cells[i].childNodes[0].value + "\" >";
                            newcell.childNodes[0].value = anyNumber;  //anyNumber is the parsed amount                            
                        }
                        newcell.childNodes[0].style.textAlign = "right";
                    }
                    else if (i == 3) {
                        newcell.innerHTML = ReturnHTMLForCheckBox("RowGrossCheckBox" + incomeId, clientId, forex, rowTableCount - 2, "gross", tableRowCheckBox);
                        newcell.align = "center";
                    }
                    else if (i == 4) {
                        if (strSection == "COS") {
                            newcell.innerHTML = ReturnHTMLForCheckBox("RowAllceCheckBox" + incomeId, clientId, forex, rowTableCount - 2, "allce", tableRowCheckBox);
                            newcell.align = "center";
                        }
                        else {
                            newcell.innerHTML = ReturnHTMLForCheckBox("RowAHGCheckBox" + incomeId, clientId, forex, rowTableCount - 2, "ahg", tableRowCheckBox);
                            newcell.align = "center";
                        }
                    }
                    else if (i == 5) {
                        if (strSection == "COS") {
                            newcell.innerHTML = ReturnHTMLForCheckBox("RowOTCheckBox" + incomeId, clientId, forex, rowTableCount - 2, "ot", tableRowCheckBox);
                            newcell.align = "center";
                        }
                        else
                            newcell.innerHTML = ReturnHTMLForNavigation(incomeId, clientId, forex);
                    }
                    else if (i == 6)
                        newcell.innerHTML = ReturnHTMLForNavigation(incomeId, clientId, forex);                      
                }	
            }

            function SplitItemAndAmount(clientIdOfMonthTable) {                
                var table = document.getElementById(clientIdOfMonthTable);
                var splitAddItemAmount = table.rows[table.rows.length - 1].cells[1].childNodes[0].value.trim().split(" ");
                var item = "";
                if (splitAddItemAmount.length > 0) {                    
                    var parsed = parseFloat(splitAddItemAmount[splitAddItemAmount.length - 1]);
                    if (isNaN(parsed) == false) {
                        table.rows[table.rows.length - 1].cells[2].childNodes[0].value = splitAddItemAmount[splitAddItemAmount.length - 1];
                        for (j = 0; j < splitAddItemAmount.length - 1; j++) {
                            item = item + splitAddItemAmount[j] + " ";
                        }
                    }
                    else {
                        for (j = 0; j < splitAddItemAmount.length; j++) {
                            item = item + splitAddItemAmount[j] + " ";
                        }
                    }
                    table.rows[table.rows.length - 1].cells[1].childNodes[0].value = item;
                }
            }


            function ValidateCheckboxes(clientIdOfMonthTable, chkBoxIndex, forexfrom, chkBoxType) {
                var table = document.getElementById(clientIdOfMonthTable);
                var rows = table.rows;
                var cells = rows[chkBoxIndex].cells;                
                if (chkBoxType == "gross") {
                    cells[4].childNodes[0].checked = false;
                    cells[5].childNodes[0].checked = false;
                }
                else if (chkBoxType == "allce") {
                    cells[3].childNodes[0].checked = true;
                    cells[5].childNodes[0].checked = false;
                }
                else if (chkBoxType == "ot") {
                    cells[3].childNodes[0].checked = false;
                    cells[4].childNodes[0].checked = false;
                }
                CalculateCheckboxes(clientIdOfMonthTable, forexfrom);
            }

            function ValidateCheckboxesForAddRow(clientIdOfMonthTable, chkBoxType) {
                var table = document.getElementById(clientIdOfMonthTable);
                var rows = table.rows;
                var chkBoxIndex = table.rows.length - 1;
                var cells = rows[chkBoxIndex].cells;
                if (chkBoxType == "gross") {
                    cells[4].childNodes[0].checked = false;
                    cells[5].childNodes[0].checked = false;
                }
                else if (chkBoxType == "allce") {
                    cells[3].childNodes[0].checked = true;
                    cells[5].childNodes[0].checked = false;
                }
                else if (chkBoxType == "ot") {
                    cells[3].childNodes[0].checked = false;
                    cells[4].childNodes[0].checked = false;
                }
            }
           
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

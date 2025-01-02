<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Import_ScanDocuments_Default" Title="DWMS - Scan Documents" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Namespace="Dwms.Bll" TagPrefix="DwmsBll" %>
<%@ Register Src="~/Import/Controls/ScanUploadFields.ascx" TagName="ScanUploadFields" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <table>
            <tr>
                <td bgcolor="#efefef" style="width: 260px;" class="wrapper10" valign="top">
                    <asp:Panel ID="Panel1" runat="server" CssClass="area area-left">
                        <uc:ScanUploadFields ID="ScanUploadFields" runat="server" />
                        <table>
                            <tr>
                                <td class="import-left">
                                    <span class="label">Scanner</span><span class="form-error">&nbsp;*</span>
                                </td>
                                <td>
                                    <select size="1" id="source" onchange="SetScannerCookie();" style="width: 135px; display: none;">
                                        <option value = ""></option>
                                    </select>
                                    <telerik:RadComboBox ID="ScannerRadComboBox" runat="server" Width="135px" DropDownWidth="270px" 
                                        CausesValidation="False" RenderingMode="Full" EmptyMessage="Choose a scanner..." 
                                        Height="200px" AllowCustomText="False" AutoPostBack="False" OnClientSelectedIndexChanged="ScannerRadComboBox_TextChange">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <span class="label">Auto Feeder</span>
                                    <asp:Image ID="AdfImage" runat="server" ImageUrl="~/Data/Images/Action/Help.png" />
                                    <telerik:RadToolTip runat="server" ID="AdfRadToolTip" ShowEvent="OnMouseOver" 
                                        HideEvent="LeaveTargetAndToolTip" Position="MiddleRight" 
                                        RelativeTo="Element" TargetControlID="AdfImage" Skin="Forest">                        
                                            Automatic Document Feeder
                                    </telerik:RadToolTip>
                                </td>
                                <td>
                                <!-- Default to checked by Lexin on 3 May 2012 -->
                                    <input type="checkbox" id="ADF" onclick="SetADFCookie();" class="hand" checked="checked" /><label for="ADF" class="hand">Yes</label>
                                </td>
                            </tr>
<%--                            <tr>
                                <td>
                                    <span class="label">For Oki only</span>
                                    <asp:Image ID="BPTImage" runat="server" ImageUrl="~/Data/Images/Action/Help.png" />
                                    <telerik:RadToolTip runat="server" ID="RadToolTip3" ShowEvent="OnMouseOver" 
                                        HideEvent="LeaveTargetAndToolTip" Position="MiddleRight" 
                                        RelativeTo="Element" TargetControlID="BPTImage" Skin="Forest">                        
                                            Check if using Oki
                                    </telerik:RadToolTip>
                                </td>
                                <td>
                                    <input type="checkbox" id="BPT" onclick="SetBPTCookie();" class="hand" /><label for="BPT" class="hand">Yes</label>
                                </td>
                            </tr>
--%>                            <tr>
                                <td>
                                    <span class="label">Show UI</span>
                                    <asp:Image ID="UIImage" runat="server" ImageUrl="~/Data/Images/Action/Help.png" />
                                    <telerik:RadToolTip runat="server" ID="RadToolTip1" ShowEvent="OnMouseOver" 
                                        HideEvent="LeaveTargetAndToolTip" Position="MiddleRight" 
                                        RelativeTo="Element" TargetControlID="UIImage" Skin="Forest">                        
                                            Default option: 300dpi<br />Black/White ADF Duplex
                                    </telerik:RadToolTip>
                                </td>
                                <td>
                                    <input type="checkbox" id="ShowUI" onclick="SetUICookie();" class="hand" /><label for="ShowUI" class="hand">Yes</label>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <div id="ProgressPanel" style="display:none; width: 240px;" class="top20">
                        <div id="ProgressbarOuter" class="progressbar">
                            <div id="ProgressbarInner"><!----></div>
                        </div>
                        <label id="ProgressLabel">Preparing pages for categorization...</label><br />
                        <span>Elapsed Time:&nbsp;</span><span id="ElapsedTimeHour">00</span>:<span id="ElapsedTimeMinute">00</span>:<span id="ElapsedTimeSecond">00</span>
                    </div>

                    <asp:Panel ID="Panel2" runat="server" CssClass="form-submit" Width="100%" style="text-align: left">
                        <input id="ScanBtn" class="button-large right10" type="button" value="Scan" runat="server" onclick ="ScanBtn_onclick();" style="width: 90px" />
                        <input id="SaveButton" class="button-large right10" type="button" value="Save" runat="server" 
                            onclick ="SaveImages(false);" style="width: 90px" />
                    </asp:Panel>
                </td>
                <td bgcolor="#ffffff" style="padding: 20px">
                    <asp:Panel ID="ConfirmPanel" runat="server" Visible="false" CssClass="reminder-green top10 bottom15" >
                        The set has been saved.
                    </asp:Panel>
                    <asp:Panel ID="WarningPanel" runat="server" Visible="false" CssClass="reminder-red top10 bottom15" >
                        There was an error while saving the set.&nbsp;&nbsp;Please try again.
                    </asp:Panel>
                    <asp:Panel ID="Panel3" runat="server" CssClass="inputform">
                        <div class="title">
                            Scan Documents
                        </div>
                        <div class="area">
                        <div id="DWTcontainer" style="height:680px; border:0;">
                            <div id="DwtControlContainer" style="width: 100%;">
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <div id="DwtControl">
                                                <div style="text-align: center;">
                                                    <input id="RemoveCurrentImageBtn" class="button-small" style="width: 150px;" onclick="return RemoveCurrentImageBtn_onclick()" type="button" value="Remove Selected"/>
                                                    <input id="RemoveAllImagesBtn" class="button-small" style="width: 100px;" onclick="return RemoveAllImagesBtn_onclick()" type="button" value="Remove All"/>
                                                    <span class="label">&nbsp;Preview Mode&nbsp;</span>
                                                    <select size="1" class="inputform" id="PreviewMode" onchange ="PreviewModeChanged();">
                                                        <option value="0">1X1</option>
                                                    </select>
                                                    <input id="FitBtn" class="button-small" style="width: 100px;" onclick="return FitBtn_onclick()" type="button" value="Fit Window"/>
                                                    <input id="ZoomInBtn" class="button-small" style="width: 20px;" onclick="return ZoomInBtn_onclick()" type="button" value="+"/>
                                                    <input id="ZoomOutBtn" class="button-small" style="width: 20px;" onclick="return ZoomOutBtn_onclick()" type="button" value="-"/>
                                                </div>
                                                <div class="bottom10">
                                                </div>
                                                <div id="MainDivPlugin">
                                                    <div id="MainControlInstalled">
                                                    </div>
                                                </div>
                                                <div id="MainDivIE">
                                                    <object classid="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" style="display:none;">
                                                        <param name="LPKPath" value="../../Data/DynamicWebTWAIN/DynamicWebTwain.lpk" />
                                                    </object>
                                                    <div id="MainDivIEx86">                                                                                           
                                                    </div>
                                                    <div id="MainDivIEx64">                              
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="extraInfo" style="font-size: 11px; color: #222222; font-family: verdana sans-serif; background-color:#f0f0f0; text-align: center; width:580px; text-align: center;" >
                                            </div>

                                            <div class="divinput" style="text-align: center; width:580px; background-color:#FFFFFF; vertical-align: middle;">
                                                <div>
                                                    <table>
                                                        <tr>
                                                            <td valign="middle">
                                                                <br />
                                                                <input id="FirstImageBtn" class="button-small" onclick="return FirstImageBtn_onclick()" type="button" value=" |&lt; "/>&nbsp;
                                                                <input id="PreImageBtn" class="button-small" onclick="return PreImageBtn_onclick()" type="button" value=" &lt; "/>&nbsp;&nbsp;
                                                                <input type="text" class="inputform" size="2" id="CurrentImage" readonly="readonly"/>&nbsp;<span class="label">/</span>
                                                                <input type="text" class="inputform" size="2" id="TotalImage" readonly="readonly"/>&nbsp;&nbsp;
                                                                <input id="NextImageBtn" class="button-small" onclick="return NextImageBtn_onclick()" type="button" value=" &gt; "/>&nbsp;
                                                                <input id="LastImageBtn" class="button-small" onclick="return LastImageBtn_onclick()" type="button" value=" &gt;| "/>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </table>                    
                            </div>
                        </div>
                    </div>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ScanFileName" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>  
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
    </telerik:RadAjaxLoadingPanel>
    <asp:HiddenField ID="ReferenceTypeHiddenField" runat="server" />
    <asp:HiddenField ID="ScanFileName" runat="server" />
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript" src="../../Data/JavaScript/webtwain.js"></script>
        <script type="text/javascript">

            RestoreSelections();

            function RestoreSelections() {
                // Scanner
                //var Scanner = document.getElementById("Scanner");
                //var ScannerCookie = getCookie("Scanner");

                //if (ScannerCookie != null && ScannerCookie != "") {
                //    for (var i = 0; i < Scanner.options.length; i++) {
                //        if (Scanner.options[i].value == ScannerCookie)
                //            Scanner.options[i].selected = true;
                //    }
                //}

                // ADF
                var ADF = document.getElementById("ADF");
//                var BPT = document.getElementById("BPT");
                var ShowUI = document.getElementById("ShowUI");
                var ADFCookie = getCookie("ADF");
//                var BPTCookie = getCookie("BPT");
                var UICookie = getCookie("ShowUI");

                if (ADFCookie != null && ADFCookie != "") {
                    if(ADFCookie == "Yes")
                        ADF.checked = true;
                    else
                        ADF.checked = false;
                }
//                if (BPTCookie != null && BPTCookie != "") {
//                    if (BPTCookie == "Yes")
//                        BPT.checked = true;
//                    else
//                        BPT.checked = false;
//                }
                if (UICookie != null && UICookie != "") {
                    if (UICookie == "Yes")
                        ShowUI.checked = true;
                    else
                        ShowUI.checked = false;
                }
            }

            function ScannerRadComboBox_TextChange(sender, args) {
                SetScannerCookie();
            }

            function SetScannerCookie() {
                //var Scanner = document.getElementById("Scanner");
                //setCookie("Scanner", Scanner.options[Scanner.selectedIndex].value, 365)

                var selectedIndex = GetComboBoxSelectedIndex();

                if (selectedIndex >= 0) {
                    var combo = $find("<%= ScannerRadComboBox.ClientID %>");

                    if (combo) {
                        setCookie("Scanner", combo.get_items().getItem(selectedIndex).get_value(), 365)
                    }
                }
            }

            function SetADFCookie() {
                var ADF = document.getElementById("ADF");

                if (ADF.checked == true)
                    setCookie("ADF", "Yes", 365);
                else
                    setCookie("ADF", "No", 365);
            }

//            function SetBPTCookie() {
//                var BPT = document.getElementById("BPT");

//                if (BPT.checked == true)
//                    setCookie("BPT", "Yes", 365);
//                else
//                    setCookie("BPT", "No", 365);
//            }

            function SetUICookie() {
                var ShowUI = document.getElementById("ShowUI");

                if (ShowUI.checked == true)
                    setCookie("ShowUI", "Yes", 365);
                else
                    setCookie("ShowUI", "No", 365);
            }

            function AssignIds(saveBtnId, scanFileNameId) {
                AssignButtonIdsImport(saveBtnId);
                AssignScanFileNameIdImport(scanFileNameId);
                AssignComboBoxIdImport();

                RestoreSelections();
            }

            // Assign the ids of the save buttons
            function AssignButtonIdsImport(saveBtnId) {
                try {
                    AssignButtonIds(saveBtnId);
                } catch (e) {

                }
            }

            // Assign the id of the scan file name hidden field
            function AssignScanFileNameIdImport(scanFileNameId) {
                try {
                    AssignScanFileNameId(scanFileNameId);
                } catch (e) {

                }
            }

            function AssignComboBoxIdImport() {
                try {
                    AssignComboBoxId("<%= ScannerRadComboBox.ClientID %>");                    
                } catch (e) {

                }
            }

            // ================= Helper Functions ====================
            function SaveImages() {
                if (SaveBtn_onclick()) {
                    saveFileSuccess = true;
                    var command = "FROMSAVE:";
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest("SaveSet");
                }
                else {
                    alert('Failed to save the scanned images. Please try again later.');
                }
            }

            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateReferenceParentPage(parameter) {
                var parameter = "UpdateRefNo," + parameter;
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(parameter);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

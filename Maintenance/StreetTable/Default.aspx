<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_StreetTable_Default" Title="DWMS - Street Name" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Street Name
    </div>
    <div class="bottom20">
        <!---->
    </div>
    <telerik:radajaxpanel id="RadAjaxPanel1" runat="server" clientevents-onrequeststart="OnRequestStart">
        <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
            Visible="false" EnableViewState="false">
            The street information has been saved.
        </asp:Panel>
        <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
            <div class="header">
                <div class="left">
                    MS Excel Templates</div>
                <div class="right">
                    <!---->
                </div>
            </div>
            <div class="area">
                <table>
                    <tr>
                        <td valign="top" width="25%">
                            <span class="label">Instruction for Import</span>
                        </td>
                        <td valign="top">
                            <ol>
                                <li>This is for the batch upload of street details.</li>
                                <li>Use only the attached MS Excel file formatted below</li>
                                <li>While entering the data, please follow the data format as suggested on the header
                                    in the MS Excel file. </li>
                                <li>Ensure that all mandatory fields are entered. </li>
                            </ol>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <span class="label">MS Excel Template File</span>
                        </td>
                        <td>
                            <asp:HyperLink ID="ExcelHyperLink" runat="server" ImageUrl="~/Data/Images/Icon_XlsLarge.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <span class="label">MS Excel Data File <span class="form-error">&nbsp;*</span></span>
                        </td>
                        <td>
                            <telerik:RadUpload runat="server" ID="ExcelRadUpload" ControlObjectsVisibility="None"
                                AllowedFileExtensions=".xls,.xlsx" InputSize="42" MaxFileInputsCount="1">
                                <Localization Select="Browse" Remove="Delete" />
                            </telerik:RadUpload>
                            <span class="form-note">Only XLS/XLSX files are accepted.</span>
                            <asp:Label ID="Error" runat="server" CssClass="form-error" ForeColor="" Visible="false"></asp:Label>
                            <asp:CustomValidator ID="EmptyFileValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                                ErrorMessage='<br />Please upload a valid file using Browse button.' ForeColor="" CssClass="form-error"
                                OnServerValidate="EmptyFileValidator_ServerValidate" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="bottom10">
                <!---->
            </div>
            <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
                <asp:Button ID="ImportButton" runat="server" Text="Import" CssClass="button-large right20"
                    OnClick="ImportButton_Click" />
                <%--<input type="button" value="Cancel" class="button-large" onclick="javascript:history.back(1);" />--%>
            </asp:Panel>
         </asp:Panel>
     </telerik:radajaxpanel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:radcodeblock id="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            //On insert and update buttons click temporarily disables ajax to perform upload actions
            function OnRequestStart(sender, eventArgs) {
                if (eventArgs.get_eventTarget() == "<%= ImportButton.UniqueID %>") {
                    eventArgs.set_enableAjax(false);
                }
            }
         </script>
    </telerik:radcodeblock>
</asp:Content>

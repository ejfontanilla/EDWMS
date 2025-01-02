<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_ImportInterface_Default" Title="DWMS - Import Interface Files" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Import Interface Files
    </div>
    <div class="bottom20">
        <!---->
    </div>
    <telerik:radajaxpanel id="RadAjaxPanel1" runat="server" clientevents-onrequeststart="OnRequestStart">
        <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
            Visible="false" EnableViewState="false">
            <asp:Label ID="WarningLabel" runat="server" ></asp:Label>
        </asp:Panel>
        <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
            Visible="false" EnableViewState="false">
            <asp:Label ID="SummaryLabel" runat="server">The interface file(s) have been saved.</asp:Label>
        </asp:Panel>
        <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
            <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
                SelectedIndex="0" Align="Left" Width="100%">
                <Tabs>
                    <telerik:RadTab Text="COS"  >
                    </telerik:RadTab>
                    <telerik:RadTab Text="RESALE" >
                    </telerik:RadTab>
                    <telerik:RadTab Text="SERS" >
                    </telerik:RadTab>
                    <telerik:RadTab Text="SALES" >
                    </telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
                <telerik:RadPageView ID="CosRadPageView" runat="server">
                    <div class="header">
                        <div class="left">
                            COS Interface Files</div>
                        <div class="right">
                            <!---->
                        </div>
                    </div>
                    <div class="area">
                        <table>
                            <tr>
                                <td valign="top">
                                    <span class="label">Household Structure (COS)</span>
                                </td>
                                <td>
                                    <telerik:RadUpload runat="server" ID="HouseholdRadUpload" ControlObjectsVisibility="None"
                                        AllowedFileExtensions=".TXT,.txt" InputSize="42" MaxFileInputsCount="1">
                                        <Localization Select="Browse" Remove="Delete" />
                                    </telerik:RadUpload>
                                    <asp:CustomValidator ID="HouseholdCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                                        ErrorMessage='Please upload a valid file using the Browse button.' ForeColor="" CssClass="form-error" Enabled="true"
                                        OnServerValidate="HouseholdRadUpload_ServerValidate" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView ID="ResaleRadPageView" runat="server">
                    <div class="header">
                        <div class="left">
                            RESALE Interface Files</div>
                        <div class="right">
                            <!---->
                        </div>
                    </div>
                    <div class="area">
                        <table>
                            <tr>
                                <td valign="top">
                                    <span class="label">Household Structure (RESALE)</span>
                                </td>
                                <td>
                                    <telerik:RadUpload runat="server" ID="RosRadUpload" ControlObjectsVisibility="None"
                                        AllowedFileExtensions=".TXT,.txt" InputSize="42" MaxFileInputsCount="1">
                                        <Localization Select="Browse" Remove="Delete" />
                                    </telerik:RadUpload>
                                    <asp:CustomValidator ID="RosCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                                        ErrorMessage='Please upload a valid file using the Browse button.' ForeColor="" CssClass="form-error" Enabled="true"
                                        OnServerValidate="RosCustomValidator_ServerValidate" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView ID="SersRadPageView" runat="server">
                    <div class="header">
                        <div class="left">
                            SERS Interface Files</div>
                        <div class="right">
                            <!---->
                        </div>
                    </div>
                    <div class="area">
                        <table>
                            <tr>
                                <td valign="top">
                                    <span class="label">Household Structure (SERS)</span>
                                </td>
                                <td>
                                    <telerik:RadUpload runat="server" ID="SersRadUpload" ControlObjectsVisibility="None"
                                        AllowedFileExtensions=".TXT,.txt" InputSize="42" MaxFileInputsCount="1">
                                        <Localization Select="Browse" Remove="Delete" />
                                    </telerik:RadUpload>
                                    <asp:CustomValidator ID="SersCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                                        ErrorMessage='Please upload a valid file using the Browse button.' ForeColor="" CssClass="form-error" Enabled="true"
                                        OnServerValidate="SersCustomValidator_ServerValidate" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </telerik:RadPageView>
                <telerik:RadPageView ID="SalesRadPageView" runat="server">
                    <div class="header">
                        <div class="left">
                            SALES Interface Files</div>
                        <div class="right">
                            <!---->
                        </div>
                    </div>
                    <div class="area">
                        <table>
                            <tr>
                                <td valign="top">
                                    <span class="label">Household Structure (SALES)</span>
                                </td>
                                <td>
                                    <telerik:RadUpload runat="server" ID="SalesRadUpload" ControlObjectsVisibility="None"
                                        AllowedFileExtensions=".TXT,.txt" InputSize="42" MaxFileInputsCount="1">
                                        <Localization Select="Browse" Remove="Delete" />
                                    </telerik:RadUpload>
                                    <asp:CustomValidator ID="SalesCustomValidator" runat="server" Display="Dynamic" EnableClientScript="false"
                                        ErrorMessage='Please upload a valid file using the Browse button.' ForeColor="" CssClass="form-error" Enabled="true"
                                        OnServerValidate="SalesCustomValidator_ServerValidate" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
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
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                    <telerik:AjaxUpdatedControl ControlID="WarningPanel" />
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

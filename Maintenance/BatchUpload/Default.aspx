<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Maintenance_BatchUpload_Default"
    MasterPageFile="~/Maintenance/Main.master" Title="DWMS - Batch Upload" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="Batch Upload" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable upload the batch. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        Batch Upload Successful.
    </asp:Panel>
        <asp:Panel ID="WaitPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        Uploading is on process. You will receive an email notification after the upload. 
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
               Select files to be uploaded</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table style="width: 100%;">
                <tr>
                    <td>
                        <telerik:RadTreeView ID="RadTreeView1" runat="server" Skin="Windows7" AccessKey="T"
                            SingleExpandPath="False" Width="100%" Height="350px" CausesValidation="false"
                            TriStateCheckBoxes="true" CheckBoxes="true" BorderWidth="1" OnClientNodeChecked="ClientNodeChecked">
                        </telerik:RadTreeView>
                        <asp:CustomValidator ID="RadTreeView1CustomValidator" runat="server" Enabled="true"
                            ErrorMessage="<br />Please select at least 1 document to download." CssClass="form-error"
                            Display="Dynamic" OnServerValidate="RadTreeView1CustomValidator_ServerValidate">
                        </asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Upload" CssClass="button-large right20"
                OnClick="Save" />
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest" AsyncPostBackTimeout="600">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SubmitButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="WaitPanel" />
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


            function ClientNodeChecked(sender, args) {
                var tree = $find("<%= RadTreeView1.ClientID %>");
                var selectedIndeces = "";
                var TotalSize = 0;

                //loop through all the nodes
                for (var i = 0; i < tree.get_allNodes().length; i++) {

                    var node = tree.get_allNodes()[i];

                    //if document
                    if (node.get_category() == 'Doc') {

                        if (node.get_checked() == true) {
                            selectedIndeces = selectedIndeces + node.get_value() + ",";
                        }
                    }
                }
                selectedIndeces = selectedIndeces.substr(0, selectedIndeces.lastIndexOf(','));
                //FunctionWebService.GetDocumentFileSizeByIdsWithFormatSize(selectedIndeces, CallSuccess, CallFailed);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

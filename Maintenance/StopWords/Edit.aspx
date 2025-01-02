<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="StopWords_Edit" Title="DWMS - Stop Words" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="GoButton">
        <table width="100%">
            <tr>
                <td>
                    <div class="title">
                        Stop Words</div>
                </td>
                <td align="right">
                    <span class="label"><b>Search:&nbsp;</b></span>
                    <asp:TextBox ID="SearchTextBox" runat="server" CssClass="form-field" Columns="10"></asp:TextBox>
                    <asp:Button ID="GoButton" runat="server" Width="30" Text="Go" CssClass="button-large" onclick="SearchButton_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <table class="alphabet bottom10" width="100%">
        <tr class="header">
            <td>
                <asp:PlaceHolder ID="PlaceHolder1" runat="server" EnableViewState="false"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Stop Words</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            Please separate each word or phrase by comma (,).<br /><br />
            <asp:TextBox ID="StopWords" runat="server" Width="100%" Rows="12" TextMode="MultiLine"
                CssClass="form-field" />
        </div>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <center>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="200">
            <asp:UpdatePanel ID="SubmitUpdatePanel" runat="server">
                <ContentTemplate>
                    <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                        OnClick="Save" />
                    <a href="javascript:history.back(1);">Cancel</a>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </center>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="SubmitButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SubmitPanel"/>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <asp:HiddenField ID="HiddenOrigin" runat="server" />
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize" Width="600px" Height="510px" ReloadOnShow="true" VisibleStatusbar="false">
    </telerik:RadWindow>  

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
                       
        function ShowWindow(url, width, height)
        {
            var oWnd = $find("<%=RadWindow1.ClientID%>");
            oWnd.setUrl(url);
            oWnd.setSize(width, height);
            oWnd.center();
            oWnd.show();    
        }   
        
        function UpdateParentPage(command)
        {
            window.location.reload( false ); 
        }
        
        </script>

    </telerik:RadCodeBlock>
</asp:Content>

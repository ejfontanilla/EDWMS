<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="AddMetaField.aspx.cs" Inherits="Maintenance_DocumentType_AddMetaField" Title="DWMS - Document Types" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title"><asp:Label ID="TitleLabel" runat="server" Text="Add Metadata Field" /></div>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10" Visible="false"
        EnableViewState="false">
        The Metadata Field has been saved.
    </asp:Panel>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Metadata Field Name</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="label" Text="Metadata Field Name"></asp:Label><span class="form-error">&nbsp;*</span>
                    </td>
                    <td>
                        <asp:HiddenField runat="server" ID="DocTypeCodeHidden" />
                        <asp:TextBox ID="FieldNameTextBox" runat="server" CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="MetaFieldRequiredFieldValidator" runat="server" ControlToValidate="FieldNameTextBox"
                            Display="Dynamic" ErrorMessage="<br />Metadata Field Name is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" CssClass="label" Text="Mandatory Verfication"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="MandatoryVerCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" CssClass="label" Text="Mandatory Completeness"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="MandatoryVComCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" CssClass="label" Text="Visible Verfication"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="VisibleVerCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label5" runat="server" CssClass="label" Text="Visible Completeness"></asp:Label>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="VisibleComCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label7" runat="server" CssClass="label" Text="Maximum Length"></asp:Label>
                        <br />
                        <asp:Label runat="server" ID="MaxNoteLabel" Font-Size="X-Small" Font-Bold="false" Text="(Enter zero for Max)" />
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="MaximumLengthTextBox" MaxLength="4" Columns="1"/>
                        <asp:RequiredFieldValidator ID="MaximumLengthRequiredFieldValidator" runat="server" ControlToValidate="MaximumLengthTextBox"
                            Display="Dynamic" ErrorMessage="<br />Maximum Length is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="MaximumLengthCompareValidator" runat="server" ControlToValidate="MaximumLengthTextBox" Type="Integer" 
                            Operator="DataTypeCheck" ErrorMessage="Please Enter an integer." Display="Dynamic" ForeColor="" CssClass="form-error"/> 
                    </td>
                </tr>
            </table>            
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Add" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:GetRadWindow().close();">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="ConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="MoreButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="KeywordRepeater" LoadingPanelID="SubmitPanel" />
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

            function ResizeAndClose(width, height) {
                GetRadWindow().setSize(width, height);
                setTimeout('GetRadWindow().close()', 1200);
                GetRadWindow().BrowserWindow.UpdateParentPageMeta(null);
            }
              
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

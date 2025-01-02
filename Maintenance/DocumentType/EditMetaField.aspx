<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="EditMetaField.aspx.cs" Inherits="Maintenance_DocumentType_EditMetaFields" Title="DWMS - Edit Metadata Fields" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        <asp:Label ID="TitleLabel" runat="server" Text="New Categorisation Rules" /></div>
    <asp:Panel ID="WarningPanel" runat="server" CssClass="reminder-red top10 bottom10"
        Visible="false" EnableViewState="false">
        We were unable to save all/some of the rules. Please try again later.
    </asp:Panel>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The categorisation rule has been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">        
        <asp:Panel ID="MetaPanel" runat="server" Visible="true">
            <div class="header">
                <div class="left">
                    Metadata Fields
                </div>
                <div class="right">
                    <!---->
                </div>
            </div>
            <div class="area">
                <table>
                    <tr>
                        <td valign="top" width="25%">
                            <span class="label">Document ID</span>
                        </td>
                        <td>
                            <asp:Label ID="DocumentIdLabel" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <span class="label">Document Type </span>
                        </td>
                        <td>
                            <asp:Label ID="DocumentTypeLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table class="table-blue" width="100%">                                     
                    <tr class="bg-blue">
                        <td rowspan="2">
                            Metadata Field Name
                        </td>
                        <td colspan="2" style="text-align: center;">
                            Mandatory
                        </td>
                        <td colspan="2" style="text-align: center;">
                            Visible
                        </td>
                        <td rowspan="2" style="text-align: center;">
                            Maximum Length
                            <br />
                            <asp:Label runat="server" ID="MaxNoteLabel" Font-Size="X-Small" Font-Bold="false" Text="(Enter zero for Max)" />
                        </td>
                        <td rowspan="2" style="text-align: center;">
                            Action
                        </td>
                    </tr>
                    <tr class="bg-blue">
                        <%--<td>
                        </td>--%>
                        <td style="width: 15%; text-align: center;">
                            Verification
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Completeness
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Verification
                        </td>
                        <td style="width: 15%; text-align: center;">
                            Completeness
                        </td>
                        <%--<td style="width: 15%; text-align: center;">
                            Action
                        </td>--%>
                    </tr>
                    <asp:Repeater ID="MetaFieldRepeater" runat="server" 
                        onitemdatabound="MetaFieldRepeater_ItemDataBound" OnItemCommand="MetaFieldRepeate_ItemCommand" >
                        <ItemTemplate>
                            <tr>                            
                                <td>
                                    <asp:HiddenField runat="server" ID="IdHidden" />
                                    <asp:HiddenField runat="server" ID="DocTypeCodeHidden" />
                                    <asp:Label runat="server" ID="FieldNameLabel"/>
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryVerCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="MandatoryComManCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleVerCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:CheckBox ID="VisibleComCheckBox" runat="server" />
                                </td>
                                <td style="text-align: center;">
                                    <asp:TextBox runat="server" ID="MaximumLengthTextBox" MaxLength="4" Columns="1"/>
                                    <asp:RequiredFieldValidator ID="MaximumLengthRequiredFieldValidator" runat="server" ControlToValidate="MaximumLengthTextBox"
                                        Display="Dynamic" ErrorMessage="<br />Maximum Length is required." ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="MaximumLengthCompareValidator" runat="server" ControlToValidate="MaximumLengthTextBox" Type="Integer" 
                                        Operator="DataTypeCheck" ErrorMessage="Please Enter an integer." Display="Dynamic" ForeColor="" CssClass="form-error"/> 
                                </td>
                                <td style="text-align: center;">
                                    <asp:HyperLink ID="EditHyperLink" runat="server" CssClass="form-field hand" 
                                        NavigateUrl="#">Edit</asp:HyperLink>
                                    <asp:Label ID="SeperatorLabel" runat="server">&nbsp;&nbsp;|&nbsp;</asp:Label>                                    
                                    <asp:LinkButton ID="DeleteLink" CausesValidation="false" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick='<%# "if(confirm(\"Deleting metadata field will also clear the associated metadata.\\nAre you sure you want to delete this field and associated metadata?\") == false) return false;" %>'>
                                        Delete
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td>
                            <asp:Button ID="AddRowButton" runat="server" Text="Add Field" CssClass="button-large right20" Width="120px"
                                CausesValidation="false"/>                                                               
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <a href="javascript:history.back(1);">Cancel</a>        
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
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

            function UpdateParentPage(condition, isContains) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(condition, isContains);
            }

            function UpdateParentPageMeta() {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest("");
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

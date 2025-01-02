<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true" ValidateRequest="false"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_ControlParameters_Default" Title="DWMS - Control Parameters" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Control Parameters</div>
    <asp:Panel ID="ConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
        Visible="false" EnableViewState="false">
        The parameter values have been saved.
    </asp:Panel>
    <asp:Label ID="InfoLabel" runat="server" CssClass="form-error" Visible="false" />
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform" DefaultButton="SubmitButton">
        <div class="header">
            <div class="left">
                Parameter Values</div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <table>
                <tr>
                    <td valign="top" width="25%">
                        <span class="label">System Email</span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="SystemEmail" runat="server" Columns="50" CssClass="form-field" MaxLength="60" />
                        <asp:RequiredFieldValidator ID="EmailRequiredValidator" runat="server" ControlToValidate="SystemEmail"
                            Display="Dynamic" ErrorMessage="<br />System email is required." Visible="true"
                            ForeColor="" CssClass="form-error"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="EmailRegularExpressionValidator" runat="server"
                            ControlToValidate="SystemEmail" Display="Dynamic" ErrorMessage='<br />Email is incorrect, it should be in a format like "name@domain.com".'
                            ValidationExpression="(\s)*\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*(\s)*" ForeColor=""
                            CssClass="form-error" EnableClientScript="false" Enabled="false">
                        </asp:RegularExpressionValidator>
                        <asp:CustomValidator ID="SystemEmailCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage='<br />System email is incorrect, it should be in a format like "name@domain.com".'
                            OnServerValidate="SystemEmailCustomValidator_ServerValidate" CssClass="form-error" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Authentication Mode </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:RadioButtonList ID="AuthenticationModeRadioButtonList" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Table" CssClass="list-item-table hand" Width="300">  
                            <asp:ListItem Value="Local" Selected="True">Local</asp:ListItem>                          
                            <asp:ListItem Value="AD">Active Directory</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Archive Audit Trail Records Older Than<span class="form-error">
                            &nbsp;*</span> </span>
                    </td>
                    <td>
                        <asp:DropDownList ID="ArchiveAudit" runat="server" CssClass="form-field" DataTextField="Text"
                            DataValueField="Value">
                        </asp:DropDownList>
                        &nbsp;Month (s)
                        <br />
                        <span class="form-note">The audit trail will be archived automatically from time to
                            time.</span>
                    </td>
                </tr>                
                <tr>
                    <td valign="top">
                        <span class="label">Batch Job Mailing List </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="BatchJobMailingList" runat="server" CssClass="form-field" Columns="60"
                            TextMode="MultiLine" Rows="3" />
                        <asp:RequiredFieldValidator ID="BatchJobMailingListRequiredFieldValidator" runat="server"
                            ControlToValidate="BatchJobMailingList" ErrorMessage="<br />Batch job mailing list is required."
                            ForeColor="" CssClass="form-error" Display="Dynamic" />
                        <asp:CustomValidator ID="BatchJobMailingListCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage='<br/>Email is incorrect, it should be in a format like "name@domain.com".'
                            ForeColor="" CssClass="form-error" OnServerValidate="BatchJobMailingListCustomValidator_ServerValidate" />
                        <br />
                        <span class="form-note">Notifications sent by batch jobs will be forwarded to this mailing
                            list.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Test Mailing List </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="TestMailingList" runat="server" CssClass="form-field" Columns="60"
                            TextMode="MultiLine" Rows="3" />
                        <asp:RequiredFieldValidator ID="TestMailingListRequiredFieldValidator" runat="server"
                            ControlToValidate="TestMailingList" ErrorMessage="<br />Test mailing list is required."
                            ForeColor="" CssClass="form-error" Display="Dynamic" />
                        <asp:CustomValidator ID="TestMailingListCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage='<br/>Eamil is incorrect, it should be in a format like "name@domain.com".'
                            ForeColor="" CssClass="form-error" OnServerValidate="TestMailingListCustomValidator_ServerValidate" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Redirect All Emails to Test Mailing List </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:RadioButtonList ID="RedirectAllEmailsToTestMailingList" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Table" CssClass="list-item-table hand" Width="100">
                            <asp:ListItem Selected="True" Value="Yes">Yes</asp:ListItem>
                            <asp:ListItem Value="No">No</asp:ListItem>
                        </asp:RadioButtonList>
                        <span class="form-note">If "Yes" is selected, all emails (including batch job emails)
                            sent by the system will be redirected to this mailing list.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Error Notification Mailing List </span><span class="form-error">&nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:TextBox ID="ErrorNotificationMailingList" runat="server" CssClass="form-field" Columns="60"
                            TextMode="MultiLine" Rows="3" />
                        <asp:RequiredFieldValidator ID="ErrorNotificationMailingListRequiredFieldValidator" runat="server"
                            ControlToValidate="ErrorNotificationMailingList" ErrorMessage="<br />Error Notification Mailing List is required."
                            ForeColor="" CssClass="form-error" Display="Dynamic" />
                        <asp:CustomValidator ID="ErrorNotificationMailingListCustomValidator" runat="server" Display="Dynamic"
                            ErrorMessage='<br/>Eamil is incorrect, it should be in a format like "name@domain.com".'
                            ForeColor="" CssClass="form-error" OnServerValidate="ErrorNotificationMailingListCustomValidator_ServerValidate" />
                    </td>
                </tr>

                 <tr>
                    <td valign="top">
                        <span class="label">Enable Error Notification </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:RadioButtonList ID="EnableErrorNotification" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Table" CssClass="list-item-table hand" Width="100">
                            <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                            <asp:ListItem Value="No">No</asp:ListItem>
                        </asp:RadioButtonList>
                        <span class="form-note">If "Yes" is selected, whenever an error occurs, 
                            a notification email will be sent to Error Notification Mailing List.</span>
                    </td>
                </tr>
            </table>
            
        </div>
        <div class="bottom10">
            <!---->
        </div>
        <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">            
            <asp:Button ID="SubmitButton" runat="server" Text="Save" CssClass="button-large right20"
                OnClick="Save" />
            <%--<a href="javascript:history.back(1);">Cancel</a>--%>
            <%--<input type="button" value="Cancel" class="button-large" onclick="javascript:history.back(1);" />--%>
        </asp:Panel>
    </asp:Panel>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="FormPanel">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="FormPanel" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>

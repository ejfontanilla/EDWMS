<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScanUploadFields.ascx.cs"
    Inherits="Import_Control_ScanUploadFields" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="Panel1" runat="server">
    <table>
        <tr>
            <td class="import-left">
                <asp:Label ID="ReferenceNumberLabel" runat="server" Text="Ref No" CssClass="label"></asp:Label>
                <asp:Image ID="ReferenceNumberImage" runat="server" ImageUrl="~/Data/Images/Action/Help.png" />
                <telerik:RadToolTip runat="server" ID="ContractNumberRadToolTip" ShowEvent="OnMouseOver"
                    HideEvent="LeaveTargetAndToolTip" Position="MiddleRight" Width="450px" RelativeTo="Element"
                    TargetControlID="ReferenceNumberImage" Skin="Forest">
                    <asp:BulletedList ID="BulletedList1" runat="server" BulletStyle="Numbered" Font-Bold="True"
                        CssClass="olist">
                        <asp:ListItem>HLE Number (for COU users / LEAS system): N11N12345</asp:ListItem>
                        <asp:ListItem>Case Number (for Resale users / RESL system): 12345R12</asp:ListItem>
                        <asp:ListItem>Sales Number (for Sales users / SOC system): 1234567C</asp:ListItem>
                        <asp:ListItem>ABCDE Ref (for SERS unit / SRS system): 123456789</asp:ListItem>
                        <asp:ListItem>NRIC: S1234567A</asp:ListItem>
                        <asp:ListItem>Others</asp:ListItem>
                    </asp:BulletedList>
                </telerik:RadToolTip>
            </td>
            <td>
               <%-- OnClientTextChange="DocAppRadComboBox_TextChange"--%>
                <telerik:RadComboBox runat="server" ID="DocAppRadComboBox" AllowCustomText="true"
                    AutoPostBack="true"                     
                    CausesValidation="false" EnableAutomaticLoadOnDemand="true" DataTextField="RefNo" ShowDropDownOnTextboxClick="false"
                    MarkFirstMatch="True" DataValueField="Id" EmptyMessage="Type a Ref No..."
                    Width="135" EnableVirtualScrolling="false" ItemsPerRequest="20"
                      OnSelectedIndexChanged="DocAppRadComboBox_OnSelectedIndexChanged" Visible="false" >
                </telerik:RadComboBox>
                <%--<telerik:RadComboBox runat="server" ID="DocAppRadComboBox" AllowCustomText="true"
                    AutoPostBack="false"                     
                    CausesValidation="false" EnableAutomaticLoadOnDemand="true" DataTextField="RefNo" ShowDropDownOnTextboxClick="true"
                    MarkFirstMatch="true" DataValueField="Id" Filter="Contains" EmptyMessage="Type a Ref No..."
                    DataSourceID="ObjectDataSource1" Width="135" EnableVirtualScrolling="false" ItemsPerRequest="20"
                    >
                </telerik:RadComboBox>--%>
                <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="true" OnTextChanged="DocAppRadTextBox_TextChanged">

                </telerik:RadTextBox>
                <asp:HiddenField runat="server" ID="DocAppHiddenValue" />
                <br />
                <asp:Label ID="NewRefWarningLabel" runat="server" CssClass="form-error" Visible="false"
                    Text="The reference number you entered is not in record. You may still proceed if you are sure it is correct.">
                </asp:Label>
                <asp:Label ID="RefNoChangedLabel" runat="server" CssClass="form-error" Visible="false"
                    Text="The current set will be recategorized if ref no is changed.">
                </asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span class="label">Ref Type</span>
            </td>
            <td>
                <asp:Label ID="ReferenceTypeLabel" runat="server" CssClass="form-field" Text="N.A."></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <span class="label">Household<br />
                    Structure</span>
            </td>
            <td>
                <asp:Label ID="householdStructureLabel" runat="server" CssClass="form-field" Text="N.A."></asp:Label>
            </td>
        </tr>

        <asp:Panel runat="server" ID="AddressPanel" Visible="true">
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    <span class="label">Block</span>
                </td>
                <td>
                    <asp:TextBox ID="BlockTextBox" runat="server" MaxLength="5" Columns="5" CssClass="form-field"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="BlockRequiredFieldValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ErrorMessage="<br />Block is required." ControlToValidate="BlockTextBox"
                        Enabled="false">
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="BlockCustomValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ErrorMessage="<br />Block format is invalid." OnServerValidate="BlockCustomValidator_ServerValidate"
                        ControlToValidate="BlockTextBox" Enabled="false"></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td>
                    <span class="label">Street Name</span>
                </td>
                <td>
                    <telerik:RadComboBox ID="StreetNameRadComboBox" runat="server" MarkFirstMatch="True"
                        CssClass="form-field hand" Height="200" AppendDataBoundItems="true">
                        <Items>
                            <telerik:RadComboBoxItem Text="- Select Street -" Value="-1" Selected="true" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>
                    <span class="label">Unit Number</span>
                </td>
                <td>
                    #&nbsp;<asp:TextBox ID="LevelTextBox" runat="server" MaxLength="3" Columns="3" CssClass="form-field"></asp:TextBox>&nbsp;-&nbsp;
                    <asp:TextBox ID="UnitNumberTextBox" runat="server" MaxLength="5" Columns="5" CssClass="form-field"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="LevelRequiredFieldValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ControlToValidate="LevelTextBox" ErrorMessage="<br />Level is required."
                        Enabled="false">
                    </asp:RequiredFieldValidator>
                    <asp:RequiredFieldValidator ID="UnitNoRequiredFieldValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ControlToValidate="UnitNumberTextBox" ErrorMessage="<br />Unit number is required."
                        Enabled="false">
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="LevelCustomValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ErrorMessage="<br />Level format is invalid." OnServerValidate="LevelCustomValidator_ServerValidate"
                        ControlToValidate="LevelTextBox" Enabled="false">
                    </asp:CustomValidator>
                    <asp:CustomValidator ID="UnitNoCustomValidator" runat="server" CssClass="form-error"
                        Display="Dynamic" ErrorMessage="<br />Unit number format is invalid." OnServerValidate="UnitNoCustomValidator_ServerValidate"
                        ControlToValidate="UnitNumberTextBox" Enabled="false">
                    </asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button runat="server" ID="GetRefNoButton" Text="Get Ref No"
                        CssClass="button-large" Width="90" CausesValidation="false" OnClick="GetRefNoButton_OnClick" />&nbsp;&nbsp;&nbsp;
                    <asp:Button runat="server" ID="Clear" Text="Clear"
                        CssClass="button-large" Width="90" CausesValidation="false" OnClick="ClearButton_OnClick" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label runat="server" ID="addressLabel" CssClass="note" Text="Address applicable for SERS application only."></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td>
                <span class="label">Channel</span><span class="form-error">&nbsp;*</span>
            </td>
            <td>
                <asp:DropDownList ID="ChannelDropDownList" runat="server" CssClass="form-field" onchange="SetChannelCookie();"
                    Width="135">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Panel runat="server" ID="RemarkPanel" Visible="false">
            <tr>
                <td>
                    <span class="label">Remark</span>
                </td>
                <td>
                    <asp:TextBox ID="RemarkTextBox" runat="server" CssClass="form-field" TextMode="MultiLine"
                        Columns="70" Rows="6"></asp:TextBox>
                    <asp:HiddenField runat="server" ID="docAppIdHiddenField" />
                </td>
            </tr>
        </asp:Panel>
    </table>
    <%--<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetDocAppForDropDown"
        TypeName="Dwms.Bll.DocAppDb" CacheDuration="20" EnableCaching="true">--%>

    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetDocAppIDOnly"
        TypeName="Dwms.Bll.DocAppDb" CacheDuration="20" EnableCaching="true">

    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="GetDocAppByAddressObjectDataSource" runat="server" SelectMethod="GetDocAppForDropDownByAddress"
        TypeName="Dwms.Bll.DocAppDb" OnSelecting="GetDocAppByAddressObjectDataSource_Selecting">
    </asp:ObjectDataSource>
    <asp:HiddenField ID="ReferenceTypeHiddenField" runat="server" />
    <%--<telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
    Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
</telerik:RadWindow>--%>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            RestoreSelections();

            function RestoreSelections() {
                // Channel
                var Channel = document.getElementById('<%=ChannelDropDownList.ClientID %>');

                var ChannelCookie;
                var url = document.location.href.toLowerCase();
                var i = url.indexOf("scandocuments");

                if (i >= 0)
                    ChannelCookie = getCookie("ScanChannel");
                else
                    ChannelCookie = getCookie("UploadChannel");

                if (ChannelCookie != null && ChannelCookie != "") {
                    for (var i = 0; i < Channel.options.length; i++) {
                        if (Channel.options[i].value == ChannelCookie)
                            Channel.options[i].selected = true;
                    }
                }
            }

            function SetChannelCookie() {
                var Channel = document.getElementById('<%=ChannelDropDownList.ClientID %>');
                var selectedValue = Channel.options[Channel.selectedIndex].value;
                var url = document.location.href.toLowerCase();
                var i = url.indexOf("scandocuments");

                if (i >= 0)
                    setCookie("ScanChannel", selectedValue, 365)
                else
                    setCookie("UploadChannel", selectedValue, 365)
            }

            function InitializeTwain() {
                PageOnLoad();
            }

            function DocAppRadComboBox_TextChange(sender, args) {
                sender.set_text(sender.get_text().toString().trim());
            }

            function ShowChildWindow(url, width, height) {
                ShowWindow(url, width, height);
            }

            function UpdateParentPage(command) {
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Panel>

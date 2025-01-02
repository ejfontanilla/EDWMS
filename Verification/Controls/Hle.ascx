<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Hle.ascx.cs" Inherits="Verification_Control_Hle" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>

<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
    <table>
        <tr>
            <td class="left">
                <span class="label">Date of Signature</span><span class="form-error" runat="server" id="Span5">&nbsp;*</span>
            </td>
            <td>
                <asp:RadioButtonList runat="server" ID="DateOfSignatureRadioButtonList" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                    <asp:ListItem Value="No" Text="No"></asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="ReqiredFieldValidator1" runat="server" ControlToValidate="DateOfSignatureRadioButtonList" ErrorMessage="Date of Signature is required." CssClass="form-error"/> 
                <asp:HiddenField runat="server" ID="DateOfSignatureRadioButtonListId" />
            </td>
        </tr>
    </table>

    <asp:Repeater runat="server" ID="PersonalMetaDataRepeater" OnItemDataBound="PersonalMetaDataRepeater_OnItemDataBound">
        <ItemTemplate>
            <div class="grey-header">
                <asp:Label ID="PersonalTypeLabel" runat="server" Text='<%@# Eval("PersonalType") %>'
                    Font-Bold="true"></asp:Label>
            </div>
            <table>
                <tr>
                    <td class="left">
                        <span class="label">NRIC</span><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
                    </td>
                    <td>
                        <uc:NricTextBox runat="server" ID="CustomNric" EnableCustomValidator="true" EnableRequiredField="true" 
                        NricValue='<%# Eval("Nric").ToString() %>' ValidationGroupNric="MetaDataValidationGroup"/>
                        <asp:HiddenField runat="server" ID="IdHiddenField" Value='<%# Eval("Id").ToString() %>' />
                        <asp:HiddenField runat="server" ID="AppPersonalIdHiddenField" Value='<%# Eval("AppPersonalId").ToString() %>' />
                    </td>
                </tr>
                <tr>
                    <td class="left">
                        <asp:Label CssClass="label" runat="server" ID="NameLabel">Name</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="Name" Text='<%# Eval("Name").ToString() %>'
                            CssClass="form-field"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NameRequiredFieldValidator" runat="server"
                            CssClass="form-error" ControlToValidate="Name" ErrorMessage="<br />Name is required."
                            Display="Dynamic" Enabled="false"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <asp:Panel runat="server" ID="PersonalMetaDataRestricted" Visible="false">
                    <tr>
                        <td>
                            <span class="label">Company Name</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="Company" Text='<%# Eval("CompanyName").ToString() %>'
                                CssClass="form-field"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="DateJoinedLabel" runat="server" Text="Date Joined" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="DateJoined" Text='<%# Eval("DateJoinedService").ToString() %>'
                                CssClass="form-field"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="GrossIncomeLabel" runat="server" Text="Gross Income" CssClass="label"></asp:Label>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td width="50%">
                                        <span class="label">Month</span>
                                    </td>
                                    <td class="right">
                                        <span class="label">Income (nearest S$)</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <table id="PresonalDataMonthTable" runat="server">
                                <tr>
                                    <td width="50%">
                                        <asp:TextBox runat="server" ID="Month1Name" Text='<%# Eval("Month1Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month1Value" Text='<%# Eval("Month1Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month2Name" Text='<%# Eval("Month2Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox MinValue="0" runat="server" ID="Month2Value" Text='<%# Eval("Month2Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month3Name" Text='<%# Eval("Month3Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month3Value" Text='<%# Eval("Month3Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month4Name" Text='<%# Eval("Month4Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month4Value" Text='<%# Eval("Month4Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month5Name" Text='<%# Eval("Month5Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month5Value" Text='<%# Eval("Month5Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month6Name" Text='<%# Eval("Month6Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month6Value" Text='<%# Eval("Month6Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month7Name" Text='<%# Eval("Month7Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month7Value" Text='<%# Eval("Month7Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month8Name" Text='<%# Eval("Month8Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month8Value" Text='<%# Eval("Month8Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month9Name" Text='<%# Eval("Month9Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month9Value" Text='<%# Eval("Month9Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month10Name" Text='<%# Eval("Month10Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month10Value" Text='<%# Eval("Month10Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month11Name" Text='<%# Eval("Month11Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month11Value" Text='<%# Eval("Month11Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox runat="server" ID="Month12Name" Text='<%# Eval("Month12Name").ToString() %>'
                                            CssClass="metaSmall"></asp:TextBox>
                                    </td>
                                    <td class="right">
                                        <asp:TextBox runat="server" ID="Month12Value" Text='<%# Eval("Month12Value").ToString() %>'
                                            CssClass="metaSmall" />
                                    </td>
                                </tr>
                            </table>
                            <div>
                            </div>
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </ItemTemplate>
    </asp:Repeater>

    <table>
        <tr>
            <td colspan="2">
                <asp:ValidationSummary runat="server" ID="MetaDataValidationSummary" ValidationGroup="MetaDataValidationGroup" />
            </td>
        </tr>
    </table>

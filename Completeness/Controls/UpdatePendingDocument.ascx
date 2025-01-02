<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UpdatePendingDocument.ascx.cs"
    Inherits="Completeness_Controls_UpdatePendingDocument" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
    <div class="header">
        <div class="left">
            Indicate Acceptance</div>
        <div class="right">
            <!---->
        </div>
    </div>
    <div class="area">
        <table width="100%">
            <tr>
                <td width="30%" valign="top">
                    <span class="label">Acceptance <span class="form-error">*</span></span>
                </td>
                <td>
                    <asp:RadioButtonList ID="AcceptanceRadioButtonList" runat="server" CssClass="list-item hand"
                        RepeatDirection="Horizontal" Width="100px">
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="0">No</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td width="30%" valign="top">
                    <span class="label">Document Condition <span class="form-error">*</span></span>
                </td>
                <td>
                    <asp:RadioButtonList ID="DocConditionRadioButtonList" runat="server" CssClass="list-item hand"
                        RepeatDirection="Horizontal" Width="220px">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td valign="top">
                    <span class="label">Remark</span>
                </td>
                <td>
                    <asp:TextBox ID="ExceptionTextBox" runat="server" Rows="5" Width="100%" TextMode="MultiLine"
                        CssClass="form-field"></asp:TextBox>
                    <asp:CustomValidator ID="CustomValidator" runat="server" Display="Dynamic" CssClass="form-error" Enabled="true"
                        ErrorMessage="<br />Please enter an exception." OnServerValidate="CustomValidator_ServerValidate"></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
        <asp:Button ID="UpdateButton" runat="server" Text="Update" CssClass="button-large"
            OnClick="UpdateButton_Click" />
    </asp:Panel>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function Validate() {
                alert("test");
                return false;
            //            var listItemArray = document.getElementsByName("<%= AcceptanceRadioButtonList.ClientID %>");
            //            var isItemChecked = false;

            //            for (var i = 0; i < listItemArray.length; i++) {
            //                var listItem = listItemArray[i];

            //                if (listItem.checked) {
            //                    alert(listItem.value);
            //                    isItemChecked = true;
            //                }
            //            }

            //            if (isItemChecked == false) {
            //                alert('Nothing is checked!');

            //                return false;
            //            }

            //            return true;
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Panel>
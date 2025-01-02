<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PowerAttorney.ascx.cs" Inherits="Verification_Control_PowerAttorney" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/NricTextBox.ascx" TagName="NricTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateTextBox.ascx" TagName="DateTextBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/DateFormatLabel.ascx" TagName="DateFormatLabel" TagPrefix="uc" %>
<%@ Register Src="~/Controls/CommonMetadata.ascx" TagName="CommonMetadata" TagPrefix="uc" %>

<uc:CommonMetadata runat="server" ID="CommonMetadata" EnableCustomValidator="true" EnableRequiredField="false" />
<div class="grey-header"><%= Dwms.Bll.Constants.MetadataHeader%></div>
<table>
     <tr>
        <td colspan="2">
            <asp:Label runat="server" ID="lblDateRangeError" Text="Date of Filing must be after today" Visible="false" CssClass="form-error"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="left">
            <span class="label">Date of Filing</span><span class="form-error" runat="server" id="Span2">&nbsp;*</span>
            <br/><uc:DateFormatLabel runat="server" ID="DateFormatLabel"/>
        </td>
        <td>
            <uc:DateTextBox runat="server" ID="DateOfFiling" RequiredFieldErrorMessage="Date of Filing is required." Visible="true"/>
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Donor 1</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No<br />(Donor 1)</span><span class="form-error" runat="server" id="Span1">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoDonor1"></asp:TextBox>
            <asp:CustomValidator id="NricFinCustomValidatorDonor1" runat="server" 
                ControlToValidate="IdentityNoDonor1" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorDonor1" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoDonor1Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity Type<br />(Donor 1)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeDonor1" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeDonor1Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name<br />(Donor 1)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameDonor1"></asp:TextBox>
            <asp:HiddenField runat="server" ID="NameDonor1Id" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Donor 2</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No<br />(Donor 2)</span><span class="form-error" runat="server" id="Span3">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoDonor2"></asp:TextBox>
            <asp:CustomValidator id="NricFinCustomValidatorDonor2" runat="server" 
                ControlToValidate="IdentityNoDonor2" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorDonor2" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoDonor2Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity Type<br />(Donor 2)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeDonor2" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeDonor2Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name<br />(Donor 2)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameDonor2"></asp:TextBox>
            <asp:HiddenField runat="server" ID="NameDonor2Id" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Donor 3</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No<br />(Donor 3)</span><span class="form-error" runat="server" id="Span4">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoDonor3"></asp:TextBox>
            <asp:CustomValidator id="NricFinCustomValidatorDonor3" runat="server" 
                ControlToValidate="IdentityNoDonor2" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorDonor2"  CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoDonor3Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity Type<br />(Donor 3)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeDonor3" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeDonor3Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name<br />(Donor 3)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameDonor3"></asp:TextBox>
            <asp:HiddenField runat="server" ID="NameDonor3Id" />
        </td>
    </tr>
    <tr><td colspan="2"><div class="grey-header-light">Donor 4</div></td></tr>
    <tr>
        <td class="left">
            <span class="label">Identity No<br />(Donor 4)</span><span class="form-error" runat="server" id="Span5">&nbsp;*</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="IdentityNoDonor4"></asp:TextBox>
            <asp:CustomValidator id="NricFinCustomValidatorDonor4" runat="server" 
                ControlToValidate="IdentityNoDonor2" Display="Dynamic"  
                OnServerValidate="NricCustomValidatorDonor2" CssClass="form-error"
                ErrorMessage="Enter a valid ID no. ex(S1234567A).">
            </asp:CustomValidator>
            <asp:HiddenField runat="server" ID="IdentityNoDonor4Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Identity Type<br />(Donor 4)</span>
        </td>
        <td>
            <asp:RadioButtonList runat="server" ID="IDTypeDonor4" AutoPostBack="true" CausesValidation="false" RepeatDirection="Horizontal" />
            <asp:HiddenField runat="server" ID="IDTypeDonor4Id" />
        </td>
    </tr>
    <tr>
        <td>
            <span class="label">Name<br />(Donor 4)</span>
        </td>
        <td>
            <asp:TextBox runat="server" ID="NameDonor4"></asp:TextBox>
            <asp:HiddenField runat="server" ID="NameDonor4Id" />
        </td>
    </tr>

</table>
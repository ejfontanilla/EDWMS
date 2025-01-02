<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="View.aspx.cs"
    Inherits="View_Set" Title="DWMS - View Set (Verification)" Trace="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="TallComponents.Web.PdfViewer" TagPrefix="tc" Namespace="TallComponents.Web.Pdf" %>
<%@ Register Src="~/Import/Controls/ScanUploadFields.ascx" TagName="ScanUploadFields"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/SplitDocument.ascx" TagName="SplitDocument" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Adoption.ascx" TagName="Adoption" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/BankStatement.ascx" TagName="BankStatement"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Baptism.ascx" TagName="Baptism" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/BirthCertificate.ascx" TagName="BirthCertificate"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/BirthCertificatChild.ascx" TagName="BirthCertificatChild"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/BirthCertSibling.ascx" TagName="BirthCertSibling"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/BusinessProfile.ascx" TagName="BusinessProfile"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/CBR.ascx" TagName="CBR" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/CommissionStatement.ascx" TagName="CommissionStatement"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/CPFContribution.ascx" TagName="CPFContribution"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/CPFStatementRefund.ascx" TagName="CPFStatementRefund"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/CPFStatement.ascx" TagName="CPFStatement"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeedSeparation.ascx" TagName="DeedSeparation"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeedSeverance.ascx" TagName="DeedSeverance"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificate.ascx" TagName="DeathCertificate"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateFa.ascx" TagName="DeathCertificateFa"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateMo.ascx" TagName="DeathCertificateMo"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateSP.ascx" TagName="DeathCertificateSP"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateEXSP.ascx" TagName="DeathCertificateEXSP"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateNRIC.ascx" TagName="DeathCertificateNRIC"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeedPoll.ascx" TagName="DeedPoll" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeclarationPrivProp.ascx" TagName="DeclarationPrivProp"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeclaraIncomeDetails.ascx" TagName="DeclaraIncomeDetails"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceDocFinal.ascx" TagName="DivorceDocFinal"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceDocInitial.ascx" TagName="DivorceDocInitial"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceDocInterim.ascx" TagName="DivorceDocInterim"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertificate.ascx" TagName="DivorceCertificate"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertFather.ascx" TagName="DivorceCertFather"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertMother.ascx" TagName="DivorceCertMother"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertExSpouse.ascx" TagName="DivorceCertExSpouse"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertNRIC.ascx" TagName="DivorceCertNRIC"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DivorceCertChild.ascx" TagName="DivorceCertChild"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DocOfFinCommitment.ascx" TagName="DocOfFinCommitment"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/EmploymentLetter.ascx" TagName="EmploymentLetter"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/EmploymentPass.ascx" TagName="EmploymentPass"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/EntryPermit.ascx" TagName="EntryPermit"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Empty.ascx" TagName="Empty" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/GLA.ascx" TagName="GLA" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Hle.ascx" TagName="HLE" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IdentityCard.ascx" TagName="IdentityCard"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IRASAssesement.ascx" TagName="IRASAssesement"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IRASIR8E.ascx" TagName="IRASIR8E" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/LettersolicitorPOA.ascx" TagName="LettersolicitorPOA"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/LicenseofTrade.ascx" TagName="LicenseofTrade"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/LoanStatementSold.ascx" TagName="LoanStatementSold"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MarriageCertificate.ascx" TagName="MarriageCertificate"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MarriageCertParent.ascx" TagName="MarriageCertParent"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MarriageCertLtSpouse.ascx" TagName="MarriageCertLtSpouse"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MarriageCertChild.ascx" TagName="MarriageCertChild"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MarriageCertSibling.ascx" TagName="MarriageCertSibling"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MortgageLoanForm.ascx" TagName="MortgageLoanForm"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/NoLoanNotification.ascx" TagName="NoLoanNotification"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/NoticeofTransfer.ascx" TagName="NoticeofTransfer"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/MedDocuDoctorLetters.ascx" TagName="MedDocuDoctorLetters"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/OrderofCourtDivorce.ascx" TagName="OrderofCourtDivorce"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/NotesSyariahCourt.ascx" TagName="NotesSyariahCourt"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/NSORDCertificate.ascx" TagName="NSORDCertificate"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/NSEnlistmentNotice.ascx" TagName="NSEnlistmentNotice"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IdentityCardChild.ascx" TagName="IdentityCardChild"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DocEduInstitute.ascx" TagName="DocEduInstitute"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/OfficialAssignee.ascx" TagName="OfficialAssignee"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/OptionPurchase.ascx" TagName="OptionPurchase"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/OrderofCourt.ascx" TagName="OrderofCourt"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/OverseasIncome.ascx" TagName="OverseasIncome"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Passport.ascx" TagName="Passport" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PAYSLIP.ascx" TagName="PAYSLIP" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PensionerLetter.ascx" TagName="PensionerLetter"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PetitionforGLA.ascx" TagName="PetitionforGLA"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PowerAttorney.ascx" TagName="PowerAttorney"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PrisonLetter.ascx" TagName="PrisonLetter"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PropertyQuestionaire.ascx" TagName="PropertyQuestionaire"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PurchaseAgreement.ascx" TagName="PurchaseAgreement"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/ReceiptsLoanArrear.ascx" TagName="ReceiptsLoanArrear"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/ReconciliatUndertakn.ascx" TagName="ReconciliatUndertakn"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/RentalArrears.ascx" TagName="RentalArrears"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Resale.ascx" TagName="Resale" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/Sales.ascx" TagName="Sales" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/SERS.ascx" TagName="SERS" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/SocialVisit.ascx" TagName="SocialVisit"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/StatementofAccounts.ascx" TagName="StatementofAccounts"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/StatementSale.ascx" TagName="StatementSale"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/StatutoryDeclaration.ascx" TagName="StatutoryDeclaration"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/StatutoryDeclGeneral.ascx" TagName="StatutoryDeclGeneral"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/StudentPass.ascx" TagName="StudentPass"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/ValuationReport.ascx" TagName="ValuationReport"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/WarranttoAct.ascx" TagName="WarranttoAct"
    TagPrefix="uc" %>
<%--Added by Edward 2017/03/30 New Document Types --%>
<%@ Register Src="~/Verification/Controls/DeathCertificatePA.ascx" TagName="DeathCertificatePA"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeathCertificateLO.ascx" TagName="DeathCertificateLO"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IdentityCardNRIC.ascx" TagName="IdentityCardNRIC"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/DeedSeparationNRIC.ascx" TagName="DeedSeparationNRIC"
    TagPrefix="uc" %>
<%--Added by Edward 2017/10/03 New Document Types Property Tax --%>
<%@ Register Src="~/Verification/Controls/PropertyTax.ascx" TagName="PropertyTax"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/PropertyTaxNRIC.ascx" TagName="PropertyTaxNRIC"
    TagPrefix="uc" %>
<%-- Added by Edward 2017/12/22 New Document Types Identity Card Father Mother --%>
<%@ Register Src="~/Verification/Controls/IdentityCardFa.ascx" TagName="IdentityCardFa"
    TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/IdentityCardMo.ascx" TagName="IdentityCardMo"
    TagPrefix="uc" %>

<%--<%@ Register Src="~/Verification/Controls/NSIDcard.ascx" TagName="NSIDcard" TagPrefix="uc" %>--%>
<%--<%@ Register Src="~/Verification/Controls/SpouseFormPurchase.ascx" TagName="SpouseFormPurchase" TagPrefix="uc" %>
<%@ Register Src="~/Verification/Controls/SpouseFormSale.ascx" TagName="SpouseFormSale" TagPrefix="uc" %>--%>
<%--<%@ Register Src="~/Verification/Controls/Empty2.ascx" TagName="Empty2" TagPrefix="uc" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Skin="Windows7" Width="100%"
        Height="100%" OnClientLoaded="SetSplitterHeight" OnClientResized="SetSplitterHeight"
        ResizeWithBrowserWindow="true" LiveResize="true">
        <telerik:RadPane ID="TreeRadPane" runat="server" Width="250px" Height="100%">
            <div class="subHeadingLeft">
                <asp:Label ID="SetConfirmLabel" runat="server" Font-Bold="true" />
                <asp:Panel ID="LeftSubmitPanel" runat="server" Width="100%" CssClass="top5">
                    <asp:Button ID="LeftConfirmButton" runat="server" Text="Confirm" CssClass="button-large2"
                        OnClick="LeftConfirmButton_Click" EnableViewState="true" CausesValidation="false" />
                    <asp:DropDownList ID="LeftOptions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="LeftOptions_SelectedIndexChanged"
                        CausesValidation="false">
                        <asp:ListItem>- Options -</asp:ListItem>
                        <asp:ListItem>Download</asp:ListItem>
                        <asp:ListItem>Assign</asp:ListItem>
                        <asp:ListItem>Route</asp:ListItem>
                        <asp:ListItem>Email</asp:ListItem>
                        <asp:ListItem>Set Info</asp:ListItem>
                    </asp:DropDownList>
                </asp:Panel>
                <asp:Label ID="SetVerificationOfficerLabel" runat="server" Font-Size="XX-Small" />
            </div>
            <div class="top5">
                <telerik:RadTreeView ID="RadTreeView1" runat="server" Skin="Windows7" AccessKey="T"
                    EnableDragAndDrop="True" OnNodeDrop="RadTreeView1_NodeDrop" OnClientNodeExpanded="rtvExplore_OnNodeExpandedCollapsed"
                    OnClientNodeCollapsed="rtvExplore_OnNodeExpandedCollapsed" OnNodeClick="RadTreeView1_NodeClick"
                    CheckBoxes="False" OnClientMouseOver="treeView_MouseOver" OnClientMouseOut="treeView_MouseOut"
                    SingleExpandPath="False" Width="100%" Height="100%" CausesValidation="false"
                    TriStateCheckBoxes="False">
                </telerik:RadTreeView>
            </div>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" CollapseMode="Both" Height="100%"
            Skin="Windows7">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PreviewRadPane" runat="server" Height="100%" Width="100%" Scrolling="None">
            <div class="subHeadingRight">
                <asp:Label ID="RightDocumentLabel" runat="server" Font-Bold="True"></asp:Label>
                <table style="width: 100%;" class="top5">
                    <tr runat="server" id="RightButtonsPanel" visible="false">
                        <td align="left" style="width: 50%;">
                            <table class="right-nav">
                                <tr>
                                    <telerik:RadToolBar ID="RadToolBarMenuSet" runat="server" Style="z-index: 90001"
                                        Skin="Default" EnableRoundedCorners="true" EnableShadows="true" OnButtonClick="RadToolBarMenu_ButtonClick"
                                        Visible="false">
                                        <Items>
                                            <telerik:RadToolBarButton Text="Set Information" CausesValidation="false" Value="SetInformation" />
                                            <telerik:RadToolBarButton IsSeparator="true" />
                                            <telerik:RadToolBarButton Text="Set Reference Number" CausesValidation="false" Value="SetReferenceNumber" />
                                            <telerik:RadToolBarButton IsSeparator="true" />
                                            <telerik:RadToolBarButton Text="Set Log" CausesValidation="false" Value="SetLog" />
                                        </Items>
                                    </telerik:RadToolBar>
                                    <telerik:RadToolBar ID="RadToolBarMenu" runat="server" Style="z-index: 90001" Skin="Default"
                                        EnableRoundedCorners="true" EnableShadows="true" OnButtonClick="RadToolBarMenu_ButtonClick"
                                        Visible="true">
                                        <Items>
                                            <telerik:RadToolBarButton Text="Thumbnails" CausesValidation="false" Value="Thumbnails" />
                                            <telerik:RadToolBarButton IsSeparator="true" />
                                            <telerik:RadToolBarButton Text="Image" CausesValidation="false" Value="Image" />
                                            <telerik:RadToolBarButton IsSeparator="true" />
                                            <telerik:RadToolBarButton Text="Log" CausesValidation="false" Value="Log" />
                                        </Items>
                                    </telerik:RadToolBar>
                                </tr>
                            </table>
                        </td>
                        <td align="right" style="width: 50%;">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="ExportButton" runat="server" Text="Export Logs" CssClass="button-large2"
                                            Visible="false" OnClick="ExportButton_Click" />
                                    </td>
                                    <td>
                                        <asp:Button ID="RotateClockwiseButton" runat="server" Text="Rotate " CssClass="button-large2"
                                            Visible="true" OnClick="RotateClockwiseButton_Click" />
                                    </td>
                                    <td>
                                        <asp:Button ID="ExtractButton" runat="server" Text="Split Document" CssClass="button-large2"
                                            Visible="true" OnClick="ExtractButton_Click" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Panel runat="server" ID="MetaDataPanel" Visible="false" Height="100%" Width="100%">
                <%--Scrolling="None">--%>
                <telerik:RadSplitter ID="RadSplitter2" runat="server" Skin="Windows7" Width="100%"
                    Height="100%" LiveResize="true" OnClientLoaded="SetInnerSplitterHeight" OnClientResized="SetInnerSplitterHeight">
                    <telerik:RadPane ID="RadPane1" runat="server" Height="100%" Scrolling="None">
                        <iframe id="pdfframe" runat="server" width="100%" frameborder="0" height="100%"></iframe>
                    </telerik:RadPane>
                    <telerik:RadSplitBar ID="RadSplitBar2" runat="server" CollapseMode="Both" Height="100%"
                        Skin="Windows7">
                    </telerik:RadSplitBar>
                    <telerik:RadPane ID="MetaDataRadPane" runat="server" Width="250px" Height="100%"
                        CssClass="metaform" Scrolling="Both">
                        <div class="grey-header">
                            Document Properties
                        </div>
                        <div class="area">
                            <table>
                                <tr>
                                    <td class="left">
                                        <asp:Label runat="server" ID="RefNoHeadingLabel" Text="" CssClass="label"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="RefNoValueLabel"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="label">Document Type</span>
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ID="DocTypeDropDownList" Skin="Windows7" AutoPostBack="true"
                                            OnSelectedIndexChanged="DocTypeDropDownList_SelectedIndexChanged" DropDownWidth="300px"
                                            Height="300px" MarkFirstMatch="true" CausesValidation="false" Visible="true" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="label">Image Condition</span>
                                    </td>
                                    <td>
                                        <telerik:RadComboBox runat="server" ID="ImageConditionDropDownList" Skin="Windows7"
                                            Visible="true" AutoPostBack="true" OnSelectedIndexChanged="ImageConditionDropDownList_SelectedIndexChanged"
                                            CausesValidation="false">
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <%--June 10, 2013 Added by Edward--%>
                                <asp:PlaceHolder runat="server" ID="AcceptancePlaceholder" Visible="false">
                                    <tr>
                                        <td>
                                            <span class="">Acceptance <span class="form-error">*</span></span>
                                        </td>
                                        <td>
                                            <asp:RadioButtonList ID="AcceptanceRadioButtonList" runat="server" CssClass="list-item hand"
                                                RepeatDirection="Horizontal" Enabled="false" Visible="false">
                                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                                                <asp:ListItem Value="N">No</asp:ListItem>
                                                <asp:ListItem Value="X">NA</asp:ListItem>
                                            </asp:RadioButtonList>
                                            <asp:RequiredFieldValidator ID="AcceptanceRequiredFieldValidator" runat="server"
                                                ControlToValidate="AcceptanceRadioButtonList" ErrorMessage='Please choose an option'
                                                Display="Dynamic" Enabled='false'></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <tr runat="server" id="TRImageDescription" visible="false">
                                    <td>
                                        <span class="label">Description</span>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="ImageDescription"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel runat="server" ID="AdditionalMetaDataPanel" Width="100%">
                                <uc:HLE runat="server" ID="HLEMetaDataUC" Visible="false" />
                                <uc:BirthCertificate runat="server" ID="BirthCertificateMetaDataUC" Visible="false" />
                                <uc:BirthCertificatChild runat="server" ID="BirthCertificatChildMetaDataUC" Visible="false" />
                                <uc:BirthCertSibling runat="server" ID="BirthCertSiblingMetaDataUC" Visible="false" />
                                <uc:CPFStatement runat="server" ID="CPFStatementMetaDataUC" Visible="false" />
                                <uc:DeathCertificate runat="server" ID="DeathCertificateMetaDataUC" Visible="false" />
                                <uc:DeathCertificateFa runat="server" ID="DeathCertificateFaMetaDataUC" Visible="false" />
                                <uc:DeathCertificateMo runat="server" ID="DeathCertificateMoMetaDataUC" Visible="false" />
                                <uc:DeathCertificateSP runat="server" ID="DeathCertificateSPMetaDataUC" Visible="false" />
                                <uc:DeathCertificateEXSP runat="server" ID="DeathCertificateEXSPMetaDataUC" Visible="false" />
                                <uc:DeathCertificateNRIC runat="server" ID="DeathCertificateNRICMetaDataUC" Visible="false" />
                                <uc:MarriageCertificate runat="server" ID="MarriageCertificateMetaDataUC" Visible="false" />
                                <uc:MarriageCertParent runat="server" ID="MarriageCertParentMetaDataUC" Visible="false" />
                                <uc:MarriageCertLtSpouse runat="server" ID="MarriageCertLtSpouseMetaDataUC" Visible="false" />
                                <uc:MarriageCertChild runat="server" ID="MarriageCertChildMetaDataUC" Visible="false" />
                                <uc:MarriageCertSibling runat="server" ID="MarriageCertSiblingMetaDataUC" Visible="false" />
                                <uc:CBR runat="server" ID="CBRMetaDataUC" Visible="false" />
                                <uc:IRASIR8E runat="server" ID="IRASIR8EMetaDataUC" Visible="false" />
                                <uc:IRASAssesement runat="server" ID="IRASAssesementMetaDataUC" Visible="false" />
                                <uc:CPFContribution runat="server" ID="CPFContributionMetaDataUC" Visible="false" />
                                <uc:StatutoryDeclaration runat="server" ID="StatutoryDeclarationMetaDataUC" Visible="false" />
                                <uc:PAYSLIP runat="server" ID="PAYSLIPMetaDataUC" Visible="false" />
                                <uc:PurchaseAgreement runat="server" ID="PurchaseAgreementMetaDataUC" Visible="false" />
                                <uc:NoLoanNotification runat="server" ID="NoLoanNotificationMetaDataUC" Visible="false" />
                                <uc:IdentityCard runat="server" ID="IdentityCardMetaDataUC" Visible="false" />
                                <uc:Passport runat="server" ID="PassportMetaDataUC" Visible="false" />
                                <uc:StudentPass runat="server" ID="StudentPassMetaDataUC" Visible="false" />
                                <uc:DeclaraIncomeDetails runat="server" ID="DeclaraIncomeDetailsMetaDataUC" Visible="false" />
                                <uc:LettersolicitorPOA runat="server" ID="LettersolicitorPOAMetaDataUC" Visible="false" />
                                <uc:ValuationReport runat="server" ID="ValuationReportMetaDataUC" Visible="false" />
                                <uc:ReceiptsLoanArrear runat="server" ID="ReceiptsLoanArrearMetaDataUC" Visible="false" />
                                <uc:RentalArrears runat="server" ID="RentalArrearsMetaDataUC" Visible="false" />
                                <uc:PetitionforGLA runat="server" ID="PetitionforGLAMetaDataUC" Visible="false" />
                                <uc:OrderofCourt runat="server" ID="OrderofCourtMetaDataUC" Visible="false" />
                                <uc:NoticeofTransfer runat="server" ID="NoticeofTransferMetaDataUC" Visible="false" />
                                <uc:WarranttoAct runat="server" ID="WarranttoActMetaDataUC" Visible="false" />
                                <uc:CPFStatementRefund runat="server" ID="CPFStatementRefundMetaDataUC" Visible="false" />
                                <uc:StatementSale runat="server" ID="StatementSaleMetaDataUC" Visible="false" />
                                <uc:PropertyQuestionaire runat="server" ID="PropertyQuestionaireMetaDataUC" Visible="false" />
                                <uc:LoanStatementSold runat="server" ID="LoanStatementSoldMetaDataUC" Visible="false" />
                                <uc:Adoption runat="server" ID="AdoptionMetaDataUC" Visible="false" />
                                <uc:DeedPoll runat="server" ID="DeedPollMetaDataUC" Visible="false" />
                                <uc:PowerAttorney runat="server" ID="PowerAttorneyMetaDataUC" Visible="false" />
                                <uc:OfficialAssignee runat="server" ID="OfficialAssigneeMetaDataUC" Visible="false" />
                                <uc:Baptism runat="server" ID="BaptismMetaDataUC" Visible="false" />
                                <uc:EmploymentLetter runat="server" ID="EmploymentLetterMetaDataUC" Visible="false" />
                                <uc:BankStatement runat="server" ID="BankStatementMetaDataUC" Visible="false" />
                                <uc:StatementofAccounts runat="server" ID="StatementofAccountsMetaDataUC" Visible="false" />
                                <uc:PensionerLetter runat="server" ID="PensionerLetterMetaDataUC" Visible="false" />
                                <uc:BusinessProfile runat="server" ID="BusinessProfileMetaDataUC" Visible="false" />
                                <uc:EntryPermit runat="server" ID="EntryPermitMetaDataUC" Visible="false" />
                                <uc:EmploymentPass runat="server" ID="EmploymentPassMetaDataUC" Visible="false" />
                                <uc:SocialVisit runat="server" ID="SocialVisitMetaDataUC" Visible="false" />
                                <uc:ReconciliatUndertakn runat="server" ID="ReconciliatUndertaknMetaDataUC" Visible="false" />
                                <uc:PrisonLetter runat="server" ID="PrisonLetterMetaDataUC" Visible="false" />
                                <uc:GLA runat="server" ID="GLAMetaDataUC" Visible="false" />
                                <uc:CommissionStatement runat="server" ID="CommissionStatementMetaDataUC" Visible="false" />
                                <uc:OverseasIncome runat="server" ID="OverseasIncomeMetaDataUC" Visible="false" />
                                <uc:LicenseofTrade runat="server" ID="LicenseofTradeMetaDataUC" Visible="false" />
                                <uc:DeedSeparation runat="server" ID="DeedSeparationMetaDataUC" Visible="false" />
                                <uc:DeedSeverance runat="server" ID="DeedSeveranceMetaDataUC" Visible="false" />
                                <uc:DivorceDocInitial runat="server" ID="DivorceDocInitialMetaDataUC" Visible="false" />
                                <uc:DivorceDocInterim runat="server" ID="DivorceDocInterimMetaDataUC" Visible="false" />
                                <uc:DivorceDocFinal runat="server" ID="DivorceDocFinalMetaDataUC" Visible="false" />
                                <uc:DivorceCertificate runat="server" ID="DivorceCertificateMetaDataUC" Visible="false" />
                                <uc:DivorceCertFather runat="server" ID="DivorceCertFatherMetaDataUC" Visible="false" />
                                <uc:DivorceCertMother runat="server" ID="DivorceCertMotherMetaDataUC" Visible="false" />
                                <uc:DivorceCertExSpouse runat="server" ID="DivorceCertExSpouseMetaDataUC" Visible="false" />
                                <uc:DivorceCertNRIC runat="server" ID="DivorceCertNRICMetaDataUC" Visible="false" />
                                <uc:DivorceCertChild runat="server" ID="DivorceCertChildMetaDataUC" Visible="false" />
                                <uc:DocOfFinCommitment runat="server" ID="DocOfFinCommitmentMetaDataUC" Visible="false" />
                                <uc:OptionPurchase runat="server" ID="OptionPurchaseMetaDataUC" Visible="false" />
                                <uc:NSEnlistmentNotice runat="server" ID="NSEnlistmentNoticeMetaDataUC" Visible="false" />
                                <uc:NSORDCertificate runat="server" ID="NSORDCertificateMetaDataUC" Visible="false" />
                                <uc:IdentityCardChild runat="server" ID="IdentityCardChildMetaDataUC" Visible="false" />
                                <uc:DocEduInstitute runat="server" ID="DocEduInstituteMetaDataUC" Visible="false" />
                                <uc:NotesSyariahCourt runat="server" ID="NotesSyariahCourtMetaDataUC" Visible="false" />
                                <uc:MedDocuDoctorLetters runat="server" ID="MedDocuDoctorLettersMetaDataUC" Visible="false" />
                                <uc:OrderofCourtDivorce runat="server" ID="OrderofCourtDivorceMetaDataUC" Visible="false" />
                                <uc:Empty runat="server" ID="EmptyMetaDataUC" Visible="false" />
                                <uc:SERS runat="server" ID="SERSMetaDataUC" Visible="false" />
                                <uc:Sales runat="server" ID="SalesMetaDataUC" Visible="false" />
                                <uc:Resale runat="server" ID="ResaleMetaDataUC" Visible="false" />
                                <uc:StatutoryDeclGeneral runat="server" ID="StatutoryDeclGeneralMetaDataUC" Visible="false" />
                                <uc:DeclarationPrivProp runat="server" ID="DeclarationPrivPropMetaDataUC" Visible="false" />
                                <uc:MortgageLoanForm runat="server" ID="MortgageLoanFormMetaDataUC" Visible="false" />
                                <%--                                <uc:NSIDcard runat="server" ID="NSIDcardMetaDataUC" Visible="false" />--%>
                                <%--                            <uc:SpouseFormPurchase runat="server" ID="SpouseFormPurchaseMetaDataUC" Visible="false" />
                                <uc:SpouseFormSale runat="server" ID="SpouseFormSaleMetaDataUC" Visible="false" />--%>
                                <%--                            <uc:Empty2 runat="server" ID="Empty2MetaDataUC" Visible="false" />--%>
                                <!--Added By Edward 2017/03/30 New Document Types-->
                                <uc:DeathCertificatePA runat="server" ID="DeathCertificatePAMetaDataUC" Visible="false" />
                                <uc:DeathCertificateLO runat="server" ID="DeathCertificateLOMetaDataUC" Visible="false" />
                                <uc:IdentityCardNRIC runat="server" ID="IdentityCardNRICMetaDataUC" Visible="false" />
                                <uc:DeedSeparationNRIC runat="server" ID="DeedSeparationNRICMetaDataUC" Visible="false" />
                                <!--Added by Edward 2017/10/03 New Document Types Property Tax-->
                                <uc:PropertyTax runat="server" ID="PropertyTaxMetaDataUC" Visible="false" />
                                <uc:PropertyTaxNRIC runat="server" ID="PropertyTaxNRICMetaDataUC" Visible="false" />
                                <!--Added by Edward 2017/12/22 New Document Types Identity Card Father Mother-->
                                <uc:IdentityCardFa runat="server" ID="IdentityCardFaMetaDataUC" Visible="false" />
                                <uc:IdentityCardMo runat="server" ID="IdentityCardMoMetaDataUC" Visible="false" />
                            </asp:Panel>
                            <asp:PlaceHolder ID="MetaDataTable" runat="server" Visible="false">
                                <asp:Repeater runat="server" ID="MetaFieldRepeater" OnItemDataBound="MetaFieldRepeater_ItemDataBound">
                                    <HeaderTemplate>
                                        <div class="grey-header">
                                            Other Information
                                        </div>
                                        <table>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="left">
                                                <asp:HiddenField runat="server" ID="MetaDataIdHiddenField" Value='<%# Eval("Id") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataFieldIdHiddenField" Value='<%# Eval("FieldId") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataFieldNameHiddenField" Value='<%# Eval("FieldName") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataVerificationMandatoryHiddenField" Value='<%# Eval("VerificationMandatory") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataCompletenessMandatoryHiddenField" Value='<%# Eval("CompletenessMandatory") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataVerificationVisibleHiddenField" Value='<%# Eval("VerificationVisible") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataCompletenessVisibleHiddenField" Value='<%# Eval("CompletenessVisible") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataFixedHiddenField" Value='<%# Eval("Fixed") %>' />
                                                <asp:HiddenField runat="server" ID="MetaDataMaximumLengthHiddenField" Value='<%# Eval("MaximumLength") %>' />
                                                <asp:Label runat="server" ID="MetaFieldHeaderLabel" Text='<%# Eval("FieldName") %>'
                                                    CssClass="label"></asp:Label>
                                                <span runat="server" id="noteSpan" visible='<%# Eval("VerificationMandatory") %>'
                                                    class="form-error">*</span>
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="MetaFieldTextBox" MaxLength='<%# Eval("MaximumLength") %>'
                                                    Text='<%# Eval("FieldValue") %>'></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="MetaFieldTextBox"
                                                    ErrorMessage='<%# Dwms.Bll.Constants.MetaDataRequiredFieldErrorMessage %>' Display="Dynamic"
                                                    Enabled='<%# Eval("VerificationMandatory") %>'></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </asp:PlaceHolder>
                            <asp:Panel ID="MetDataConfirmPanel" runat="server" CssClass="reminder-green top10 bottom15"
                                Visible="false" EnableViewState="false">
                                <asp:Label ID="MetDataConfirmPanelLabel" runat="server" Text="The metadata has been saved successfully."></asp:Label>
                            </asp:Panel>
                        </div>
                        <asp:Panel ID="MetaDataButtonPanel" runat="server" CssClass="form-submit" Width="100%"
                            Visible="false">
                            <asp:Button ID="SubmitMetadataButton" runat="server" Text="Save" CssClass="button-large right20"
                                CausesValidation="false" OnClick="SubmitMetadataButton_Click" Style="width: 70px;" />
                            <asp:Button ID="SubmitAndConfirmMDButton" runat="server" Text="Confirm" CssClass="button-large"
                                CausesValidation="false" UseSubmitBehavior="false" OnClick="SubmitAndConfirmMDButton_Click"
                                Style="width: 70px;" />
                        </asp:Panel>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </asp:Panel>
            <asp:Panel runat="server" ID="SetInfoPanel" Visible="true" CssClass="inputform" Height="100%">
                <div class="wrapper10">
                    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
                        SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab Text="Set Information" Value="0">
                            </telerik:RadTab>
                            <telerik:RadTab Text="Reference Numbers" Value="1">
                            </telerik:RadTab>
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" Height="100%">
                        <telerik:RadPageView ID="RadPageView1" runat="server" Height="100%">
                            <div class="area" style="height: 100%">
                                <asp:Panel ID="ConfirmSetPanel" runat="server" CssClass="reminder-green top10 bottom15"
                                    Visible="false" EnableViewState="false" Height="100%">
                                    The set information has been saved successfully.
                                </asp:Panel>
                                <table>
                                    <tr>
                                        <td>
                                            <span class="label">Channel</span><span class="form-error">&nbsp;*</span>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ChannelDropDownList" runat="server" CssClass="form-field" Width="135"
                                                Enabled="false">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="label">Remark</span>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="RemarkTextBox" runat="server" CssClass="form-field" TextMode="MultiLine"
                                                Columns="70" Rows="6"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left">
                                            <asp:Label runat="server" ID="Label2" Text="Acknowledge Number" CssClass="label"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="AcknowledgeNumberLabel" Text="NA"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left">
                                            <asp:Label runat="server" ID="Label4" Text="Original file info" CssClass="label"></asp:Label>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                                <telerik:RadGrid ID="RawfileRadGrid" runat="server" AutoGenerateColumns="False" AllowSorting="False"
                                    Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                                    OnItemDataBound="RawfileRadGrid_ItemDataBound" AllowFilteringByColumn="False"
                                    EnableLinqExpressions="False" OnNeedDataSource="RawfileRadGrid_NeedDataSource"
                                    Height="100%">
                                    <PagerStyle Mode="NextPrevAndNumeric" />
                                    <MasterTableView>
                                        <ItemStyle CssClass="pointer" />
                                        <AlternatingItemStyle CssClass="pointer" />
                                        <SortExpressions>
                                        </SortExpressions>
                                        <NoRecordsTemplate>
                                            <div class="wrapper10">
                                                No records were found.
                                            </div>
                                        </NoRecordsTemplate>
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderText="File Id" UniqueName="Id" DataField="Id" AutoPostBackOnFilter="true"
                                                DataType="System.String" SortExpression="Id">
                                                <ItemTemplate>
                                                    <%# Eval("Id")%>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="File Name" UniqueName="FileName" DataField="FileName"
                                                AutoPostBackOnFilter="true" DataType="System.String">
                                                <ItemTemplate>
                                                    <asp:HyperLink runat="server" ID="linkOrigDoc" Text="test"></asp:HyperLink>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                                        <Selecting AllowRowSelect="True" />
                                        <Resizing AllowColumnResize="True" />
                                        <Scrolling SaveScrollPosition="false" />
                                    </ClientSettings>
                                </telerik:RadGrid>
                            </div>
                            <div>
                                <asp:Panel ID="SubmitPanel" runat="server" CssClass="form-submit" Width="100%">
                                    <asp:Button ID="SaveSetInfoButton" runat="server" Text="Save" CssClass="button-large right20"
                                        OnClick="SaveSetInfoButton_Click" />
                                    <asp:Button ID="CancelSetInfoButton" runat="server" Text="Cancel" CssClass="button-large right20" />
                                </asp:Panel>
                            </div>
                        </telerik:RadPageView>
                        <telerik:RadPageView ID="RadPageView2" runat="server">
                            <div class="area">
                                <asp:Panel ID="ConfirmRefNoPanel" runat="server" CssClass="reminder-green top10 bottom15"
                                    Visible="false" EnableViewState="false">
                                    Modifications to the reference number have been saved successfully.
                                </asp:Panel>
                                <table>
                                    <tr>
                                        <td valign="top" colspan="3">
                                            <%--<telerik:RadComboBox runat="server" ID="RefNoRadComboBox" AllowCustomText="true"
                                                OnSelectedIndexChanged="DocAppRadComboBox_OnSelectedIndexChanged" AutoPostBack="true"
                                                DataSourceID="ObjectDataSource1" CausesValidation="false" EnableAutomaticLoadOnDemand="true"
                                                DataTextField="RefNo" MarkFirstMatch="True" DataValueField="Id" Filter="Contains"
                                                EmptyMessage="Type a Ref No..." Width="135" EnableVirtualScrolling="true" ItemsPerRequest="20"
                                                OnClientTextChange="DocAppRadComboBox_TextChange">
                                            </telerik:RadComboBox>--%>
                                            <telerik:RadTextBox runat="server" ID="DocAppRadTextBox" Width="135" AutoPostBack="true" OnTextChanged="DocAppRadTextBox_TextChanged">
                                            </telerik:RadTextBox>
                                            <asp:HiddenField runat="server" ID="DocAppHiddenValue" />
                                            &nbsp;
                                            <asp:Button ID="AddReferenceButton" runat="server" Text="Add" CssClass="button-large"
                                                OnClick="AddReferenceButton_Click" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 80px">
                                            <span class="label">Ref Type</span>
                                        </td>
                                        <td style="width: 260px">
                                            <asp:Label ID="ReferenceTypeLabel" runat="server" CssClass="form-field" Text="N.A."></asp:Label>
                                        </td>
                                        <td style="width: 50%"></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span class="label">Household<br />
                                                Structure</span>
                                        </td>
                                        <td>
                                            <asp:Label ID="householdStructureLabel" runat="server" CssClass="form-field" Text="N.A."></asp:Label>
                                        </td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <telerik:RadGrid ID="ReferenceNoRadGrid" runat="server" AutoGenerateColumns="False"
                                                AllowSorting="False" Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True"
                                                PageSize="20" AllowFilteringByColumn="False" EnableLinqExpressions="False" OnItemCommand="ReferenceNoRadGrid_ItemCommand"
                                                OnNeedDataSource="ReferenceNoRadGrid_NeedDataSource">
                                                <PagerStyle Mode="NextPrevAndNumeric" />
                                                <MasterTableView>
                                                    <ItemStyle CssClass="pointer" />
                                                    <AlternatingItemStyle CssClass="pointer" />
                                                    <SortExpressions>
                                                    </SortExpressions>
                                                    <NoRecordsTemplate>
                                                        <div class="wrapper10">
                                                            No records were found.
                                                        </div>
                                                    </NoRecordsTemplate>
                                                    <Columns>
                                                        <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                                            HeaderStyle-Width="40">
                                                            <ItemTemplate>
                                                                <%# Container.ItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn HeaderText="Reference Number" UniqueName="RefNo" DataField="RefNo"
                                                            AutoPostBackOnFilter="true" DataType="System.String" SortExpression="RefNo">
                                                            <ItemTemplate>
                                                                <%# Eval("RefNo") %>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn HeaderText="Options" UniqueName="Options" AllowFiltering="false"
                                                            HeaderStyle-Width="110px">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="RetrieveLinkButton" runat="server" CommandName="Retrieve" CommandArgument='<%# Eval("Id") %>'>
                                                                    Retrieve
                                                                </asp:LinkButton>&nbsp;&nbsp;|&nbsp;
                                                                <asp:LinkButton ID="DeleteLinkButton" runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                                                    OnClientClick='if(confirm("Are you sure you want to delete this reference number association?") == false) return false;'>
                                                                    Delete
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn HeaderText="Barcode" UniqueName="barcode" AllowFiltering="false"
                                                            HeaderStyle-Width="100px" AutoPostBackOnFilter="true" DataType="System.String"
                                                            SortExpression="RefNo">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="BarcodeLinkButton" runat="server" CommandName="PrintBarcode"
                                                                    CommandArgument='<%# Eval("Id") %>'>
                                                                    Print Barcode
                                                                </asp:LinkButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                    </Columns>
                                                </MasterTableView>
                                                <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                                                    <Selecting AllowRowSelect="True" />
                                                    <Resizing AllowColumnResize="True" />
                                                    <Scrolling SaveScrollPosition="false" />
                                                </ClientSettings>
                                            </telerik:RadGrid>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </div>
            </asp:Panel>
            <asp:Panel ID="LogPanel" runat="server" Visible="false" CssClass="wrapper10" ScrollBars="Vertical">
                <telerik:RadGrid ID="RadGridLog" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                    Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="20"
                    AllowFilteringByColumn="False" EnableLinqExpressions="False" OnNeedDataSource="RadGridLog_NeedDataSource"
                    OnItemDataBound="RadGridLog_ItemDataBound">
                    <PagerStyle Mode="NextPrevAndNumeric" />
                    <MasterTableView ClientDataKeyNames="id">
                        <ItemStyle CssClass="pointer" />
                        <AlternatingItemStyle CssClass="pointer" />
                        <SortExpressions>
                            <telerik:GridSortExpression FieldName="LogDate" SortOrder="Descending"></telerik:GridSortExpression>
                        </SortExpressions>
                        <NoRecordsTemplate>
                            <div class="wrapper10">
                                No records were found.
                            </div>
                        </NoRecordsTemplate>
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="TemplateColumn" HeaderText="S/N" AllowFiltering="false"
                                HeaderStyle-Width="40">
                                <ItemTemplate>
                                    <asp:Label ID="ItemCountLabel" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="User" UniqueName="UserID" DataField="userID"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="UserID"
                                HeaderStyle-Width="170">
                                <ItemTemplate>
                                    <%# Eval("Name")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Date" UniqueName="LogDate" DataField="LogDate"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="LogDate"
                                HeaderStyle-Width="140">
                                <ItemTemplate>
                                    <%# Eval("LogDate")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Document" UniqueName="TypeId" DataField="TypeId"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="TypeId">
                                <ItemTemplate>
                                    <%# Eval("Description")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Action" UniqueName="Action" DataField="Action"
                                AutoPostBackOnFilter="true" DataType="System.String" SortExpression="Action">
                                <ItemTemplate>
                                    <%# Eval("Action")%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings ReorderColumnsOnClient="True" EnableRowHoverStyle="true">
                        <Selecting AllowRowSelect="True" />
                        <Resizing AllowColumnResize="True" />
                        <Scrolling SaveScrollPosition="false" />
                    </ClientSettings>
                    <FilterMenu OnClientShown="MenuShowing">
                    </FilterMenu>
                </telerik:RadGrid>
            </asp:Panel>
            <asp:Panel ID="ThumbnailPanel" runat="server" CssClass="wrapper10" Height="100%"
                Width="98%" ScrollBars="Vertical">
                <div style="height: 100%">
                    <telerik:RadListView runat="server" ID="ThumbnailRadListView" OnNeedDataSource="RadListView1_NeedDataSource"
                        OnItemDrop="RadListView1_ItemDrop" DataKeyNames="Id" BorderStyle="Solid" BorderWidth="1px"
                        OnItemDataBound="RadListView1_OnItemDataBound" AllowPaging="False" PageSize="800"
                        AllowMultiItemSelection="False" ClientSettings-AllowItemsDragDrop="True">
                        <LayoutTemplate>
                            <div class="RadListView RadListViewFloated RadListView_<%# Container.Skin %>">
                                <div class="rlvFloated rlvAutoScroll">
                                    <div id="itemPlaceholder" runat="server">
                                    </div>
                                </div>
                                <br />
                                <telerik:RadDataPager ID="RadDataPager1" runat="server" PageSize="800" Visible="false">
                                    <Fields>
                                        <telerik:RadDataPagerButtonField FieldType="Numeric" />
                                    </Fields>
                                </telerik:RadDataPager>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="rlvI" style="height: 260px; width: 170px; margin-top: 5px; margin-left: 5px; margin-right: 5px; margin-bottom: 5px; background-color: White; text-align: center;"
                                onmouseover="containerMouseover(this)" onmouseout="containerMouseout(this)">
                                <telerik:RadListViewItemDragHandle ID="RadListViewItemDragHandle1" runat="server"
                                    ToolTip="Drag to move" /><br />
                                <img id="ThumbnailImg" alt='<%# "Page " + Eval("DocPageNo") %>' runat="server" height="200"
                                    width="150" src="#" />
                                <br />
                                <div style="text-align: center;">
                                    <asp:CheckBox ID="SelectedCheckBox" runat="server" />
                                    <asp:Label ID="Label1" runat="server" Text='<%# "Page " + Eval("DocPageNo") %>'></asp:Label>
                                    <br />
                                    <asp:Label ID="Label3" runat="server" Text='<%# "Orig page " + Eval("RawPageNo") + " of File ID "+ Eval("RawFileId") %>'></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="RadListView RadListView_<%# Container.Skin %>">
                                <div class="rlvEmpty">
                                    There are no items to be displayed.
                                </div>
                            </div>
                        </EmptyDataTemplate>
                        <ClientSettings AllowItemsDragDrop="true">
                            <ClientEvents OnItemDropping="radListView_ItemDropping" />
                        </ClientSettings>
                    </telerik:RadListView>
                    <br />
                    <asp:Label CssClass="label" runat="server" ID="SplitTypeLabel" Text="Split Type"></asp:Label>
                    <asp:RadioButtonList runat="server" ID="SplitTypeRadioButtonList" RepeatDirection="Horizontal" />
                    <br />
                    <br />
                </div>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetDocAppForDropDown"
        TypeName="Dwms.Bll.DocAppDb"></asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="NodeValueHiddenField" EnableViewState="false"
        Value="-1" />
    <asp:HiddenField runat="server" ID="NodeCategoryHiddenField" EnableViewState="false"
        Value="-1" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" OnAjaxRequest="RadAjaxManager1_AjaxRequest"
        ClientEvents-OnRequestStart="RequestStarted" EnableAJAX="true" DefaultLoadingPanelID="LoadingPanel1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PersonalDataList" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="pdfframe" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadTreeView1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ExportButton" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolBarMenu" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="DocTypeDropDownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetaFieldRepeater" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SubmitMetadataButton">
                <UpdatedControls>
                    <%--<telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />--%>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <%--<telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />--%>
                    <telerik:AjaxUpdatedControl ControlID="AdditionalMetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetDataConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SubmitAndConfirmMDButton">
                <UpdatedControls>
                    <%--<telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />--%>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <%--  <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />--%>
                    <telerik:AjaxUpdatedControl ControlID="AdditionalMetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetDataConfirmPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadToolBarMenu">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ExportButton" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="LeftConfirmButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SetConfirmLabel" />
                    <telerik:AjaxUpdatedControl ControlID="LeftConfirmButton" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="LeftOptions" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="SubmitAndConfirmMDButton" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="SubmitMetadataButton" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="SaveSetInfoButton" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="CancelSetInfoButton" UpdatePanelRenderMode="Inline" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="AddReferenceButton" UpdatePanelRenderMode="Inline" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="SaveSetInfoButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ConfirmSetPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="LeftOptions">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ExportButton" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="LeftOptions" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolBarMenu" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RotateClockwiseButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ExportButton" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailRadListView" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolBarMenu" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ExtractButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                    <telerik:AjaxUpdatedControl ControlID="MetaDataPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                    <telerik:AjaxUpdatedControl ControlID="ExportButton" />
                    <telerik:AjaxUpdatedControl ControlID="RotateClockwiseButton" />
                    <telerik:AjaxUpdatedControl ControlID="ExtractButton" />
                    <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolBarMenu" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ImageConditionDropDownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ImageConditionDropDownList" />
                    <telerik:AjaxUpdatedControl ControlID="SubmitAndConfirmMDButton" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ExportButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LogPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ThumbnailRadListView">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="RightDocumentLabel" />
                    <telerik:AjaxUpdatedControl ControlID="ThumbnailPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ReferenceNoRadGrid">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="LeftConfirmButton" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="AddReferenceButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                    <telerik:AjaxUpdatedControl ControlID="LeftConfirmButton" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RefNoRadComboBox">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="SetInfoPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RadTreeView1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset" IsSticky="true"
        Style="position: absolute; top: 0; left: 0; height: 100%; width: 100%;">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
    <telerik:RadToolTipManager ID="RadToolTipManager1" OffsetX="-190" HideEvent="ManualClose"
        Width="255" Height="500" runat="server" OnAjaxUpdate="OnAjaxUpdate" RelativeTo="Element"
        Position="BottomRight" Skin="Windows7" Animation="None" Modal="true" ShowDelay="100">
    </telerik:RadToolTipManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var myHeight = getClientHeight();

            function SetSplitterHeight(splitter, args) 
            {
                var myHeight = getClientHeight();
                var height = myHeight - 97;

                var PreviewRadPane = splitter.getPaneById("<%= PreviewRadPane.ClientID %>"); 
                PreviewRadPane.set_height(height);

                var TreeRadPane = splitter.getPaneById("<%= TreeRadPane.ClientID %>"); 
                TreeRadPane.set_height(height);

                //                var RadTreeView1 = document.getElementById("<%= RadTreeView1.ClientID %>");

                //                if(RadTreeView1)
                //                {
                //                    RadTreeView1.style.height = myHeight - 174;
                //                }
                ////                var RadTreeView1 = splitter.getPaneById("<%= RadTreeView1.ClientID %>"); 
                //                RadTreeView1.set_height(myHeight - 167);

                splitter.set_height(height);
                SetHeightForControls()
            }
        
            function SetInnerSplitterHeight(splitter, args) 
            {
                var myHeight = getClientHeight();
                var height = myHeight - 167;

                var MetaDataRadPane = splitter.getPaneById("<%= MetaDataRadPane.ClientID %>"); 
                MetaDataRadPane.set_height(height);

                var RadPane1 = splitter.getPaneById("<%= RadPane1.ClientID %>"); 
                RadPane1.set_height(height);

                var frame = document.getElementById("<%= pdfframe.ClientID %>"); 
                if(frame)
                {
                    frame.style.height = height + "px";    
                }

                splitter.set_height(height);
            }

            function CloseActiveToolTip() {
                var tooltip = Telerik.Web.UI.RadToolTip.getCurrent();
                if (tooltip) tooltip.hide();

                UpdateParentPage(null);
                //RefreshUrl(window.location.href);
            }

            function SetHeightForControls() {
                var myHeight = getClientHeight();
                var height = (myHeight - 172) + "px";

                var RadTreeView1 = document.getElementById("<%= RadTreeView1.ClientID %>");

                if(RadTreeView1)
                {
                    RadTreeView1.style.height = height;
                }

                var ThumbnailPanel = document.getElementById("<%= ThumbnailPanel.ClientID %>");

                if (ThumbnailPanel) {
                    ThumbnailPanel.style.height = height;
                }

                var LogPanel = document.getElementById("<%= LogPanel.ClientID %>");

                if (LogPanel) {
                    LogPanel.style.height = height;
                }
            }

            // Window Closing, Page Redirect events (Start)
            var hook = true;
            var edited = false;

            window.onbeforeunload = function () {
                if (hook && edited) {
                    return "You have modifications that have not been saved."
                }
            }

            function UnHook() {
                hook = false;
            }

            function Edited() {
                edited = true;
            }

            function ResetFlags() {
                hook = true;
                edited = false;
            }
            // Window Closing, Page Redirect events (End)

            function ShowWindow(url, width, height) {
                var oWnd = $find("<%=RadWindow1.ClientID%>");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.center();
                oWnd.show();
            }

            function UpdateParentPage(command) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(command);
            }

            function OnNodeExpanded(sender, args) {
                var category = args.get_node().get_category();
                var expandedNode = args.get_node();

                if (category == "Set") {
                    //CollapseSiblings(expandedNode);
                }
            }

            function rtvExplore_OnNodeExpandedCollapsed(sender, eventArgs)
            {
                var allNodes = eventArgs._node.get_treeView().get_allNodes();
                var i;
                var unSelectedNodes = "";
  
                for (i = 0; i < allNodes.length; i++)
                {
                    if (!allNodes[i].get_expanded())
                        unSelectedNodes += allNodes[i].get_value() + "*";
                }
                setCookie("set" + <%= setId %> + "collapsedNodes", unSelectedNodes, 30);
            }

            function RefreshUrl(url)
            {
                window.location = url;
            }

            function CollapseSiblings(expandedNode) {
                var tree = $find("<%= RadTreeView1.ClientID %>");
                var nodes = tree.get_nodes();
                var siblingsCount = nodes.get_count();

                for (var nodeIndex = 0; nodeIndex < siblingsCount; nodeIndex++) {
                    var siblingNode = nodes.getNode(nodeIndex);

                    if (siblingNode.get_category() == "Set") {
                        if ((siblingNode != expandedNode) && (siblingNode.get_expanded())) {
                            siblingNode.collapse();                            
                        }
                    }
                }
            }

            function loadExport() {
                //window.location = "../common/DownloadSetDocument.aspx?id=" + <%= setId %>;
                ShowWindow("../Common/ShowSetDocument.aspx?id=" + <%= setId %>, 600, 600);
            }

            function RequestStarted(ajaxManager, eventArgs) {
                if (eventArgs.get_eventTarget().indexOf("ExportButton") != -1) {              
                    eventArgs.set_enableAjax(false);
                }
                else {
                    eventArgs.set_enableAjax(true);
                }
            }

            var _currentNode;
            function treeView_MouseOut(sender, args) {
                _currentNode = null;
            }

            function treeView_MouseOver(sender, args) {
                _currentNode = args.get_node();
            }

            function radListView_ItemDropping(sender, args) {                
                if (_currentNode) { // dropping on a treeview node                   
                    //save node value and node category for later retrival
                    $get("<%=NodeValueHiddenField.ClientID %>").value = _currentNode.get_value();
                    //$get("<%=NodeCategoryHiddenField.ClientID %>").value = _currentNode.get_category();
                }
                else if(args.get_destinationElement().id.indexOf("ThumbnailRadListView") < 0 && !_currentNode){
                    args.set_cancel(true);
                    return;
                }
            }

            function containerMouseover(sender)
            {
                //sender.getElementsByTagName("div")[0].style.display = "";
            }

            function containerMouseout(sender)
            {
                //sender.getElementsByTagName("div")[0].style.display = "none";
            }

            function DocAppRadComboBox_TextChange(sender, args) {
                sender.set_text(sender.get_text().toString().trim());
            }

            // ############ Reference Actions
            function UpdateReferenceParentPage(parameter) {
                var parameter = "UpdateRefNo," + parameter;
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(parameter);
            }

            function OpenWindow(url)
            {
                window.open(url, 'winname', "directories=0,titlebar=0,toolbar=0,location=0,status=0, menubar=0,scrollbars=no,resizable=no,width=250,height=120, resize=no");
            }

        </script>
    </telerik:RadCodeBlock>
</asp:Content>

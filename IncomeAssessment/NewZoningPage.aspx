<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewZoningPage.aspx.cs" Inherits="IncomeAssessment_NewZoningPage"
    MasterPageFile="~/Main.master" Title="DWMS - Zoning Panel (Income Extraction)"
    EnableEventValidation="false" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="TallComponents.Web.PdfViewer" TagPrefix="tc" Namespace="TallComponents.Web.Pdf" %>
<%@ Register Src="~/Import/Controls/ScanUploadFields.ascx" TagName="ScanUploadFields"
    TagPrefix="uc" %>
<%@ Register Src="~/Controls/SplitDocument.ascx" TagName="SplitDocument" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Skin="Windows7" Width="100%"
        Height="100%" OnClientLoaded="SetSplitterHeight" OnClientResized="SetSplitterHeight">
        <telerik:RadPane ID="TreeRadPane" runat="server" Width="100%" Height="100%" Scrolling="Both">
            <asp:Panel ID="LeftSubmitPanel" runat="server" Width="100%" Height="100%">
                <div class="subHeadingRight">
                    <div class="top10">
                    </div>
                    <asp:Label ID="ApplicantLabel" runat="server" Font-Bold="true" Text=""></asp:Label>&nbsp&nbsp
                    <asp:Button ID="ChangeButton" runat="server" Text="Change" CssClass="button-large2"
                        EnableViewState="true" CausesValidation="false" /><br />
                    <asp:Button ID="PreviousButton" runat="server" Text="Previous" CssClass="button-small"
                        CausesValidation="false" EnableViewState="true" OnClick="PreviousButton_Click" />&nbsp&nbsp
                    <asp:Button ID="NextButton" runat="server" Text="Next" CssClass="button-small" CausesValidation="false"
                        EnableViewState="true" OnClick="NextButton_Click" />&nbsp&nbsp
                </div>
                <iframe id="pdfframe" runat="server" width="100%" frameborder="0" height="100%">
                </iframe>
                <div class="top5">
                </div>
            </asp:Panel>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitBar1" runat="server" CollapseMode="Both" Height="100%"
            Skin="Windows7">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="PreviewRadPane" runat="server" Height="100%" Scrolling="Both">
            <asp:Panel ID="PanelRight" runat="server" Width="100%" BorderWidth="0">
                <div class="subHeadingLeft">
                    <table>
                        <tr>
                            <td>
                                <%--<asp:ImageButton ID="TextPaddingLeftButton" runat="server" ImageUrl="~/Data/Images/text_padding_left.png" />--%>
                            </td>
                            <td>
                                <%--<asp:ImageButton ID="ArrowDivideButton" runat="server" ImageUrl="~/Data/Images/arrow_divide.png" />--%>
                                <asp:ImageButton ID="CopyButton" runat="server" ImageUrl="~/Data/Images/Copy.png"
                                    ToolTip="Copy to all or selected months" />
                            </td>
                            <td>
                                <%--<asp:ImageButton ID="ArrowDivideButton" runat="server" ImageUrl="~/Data/Images/arrow_divide.png" />--%>
                                <asp:ImageButton ID="ArrowDivideButton" runat="server" ImageUrl="~/Data/Images/arrow_divide.png"
                                    OnClientClick="DivideAssessment();return false;" ToolTip="Divide to each Month" />
                            </td>
                            <td>
                                <asp:ImageButton ID="DeleteTemp" runat="server" ImageUrl="~/Data/Images/deletered.png"
                                    OnClientClick="DeleteItems();return false;" ToolTip="Delete Item" />
                            </td>
                            <td>
                                <asp:ImageButton ID="IncomeBlank" runat="server" ImageUrl="~/Data/Images/IncomeBlank.png"
                                    ToolTip="Set Income to Blank" OnClientClick="SetIncomeToBlank();return false;" />
                            </td>
                            <td>
                                <asp:ImageButton ID="HistoryButton" runat="server" ImageUrl="~/Data/Images/history.png"
                                    ToolTip="View Log" />
                            </td>
                            <td>
                                <asp:ImageButton ID="VersionButton" runat="server" ImageUrl="~/Data/Images/Version.png"
                                    ToolTip="View/Edit Versions" />
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="DropDownList1">
                                    <asp:ListItem>-Version-</asp:ListItem>
                                    <asp:ListItem>1</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="templateddl">
                                    <asp:ListItem>-Template-</asp:ListItem>
                                    <asp:ListItem>Basic</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="button-large2" EnableViewState="true"
                                    CausesValidation="false" OnClick="SaveButton_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <table width="100%" id="ZoningPerMonthTable" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>January 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='pp'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="text1" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox36" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBoxGrossIncome1" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBoxAllowance1" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="chkboxOT1" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="btnUp" runat="server">
                                <asp:Image runat="server" ID="imgUp" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="btnDown" runat="server">
                                <asp:Image runat="server" ID="imgDown" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton70" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr1'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox1" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox37" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox1" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox2" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox3" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton1" runat="server">
                                <asp:Image runat="server" ID="Image1" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton2" runat="server">
                                <asp:Image runat="server" ID="Image2" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton69" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label5" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label6" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label7" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox4" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox38" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox10" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox11" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox12" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="btn" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table1" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>February 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr2'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox2" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox3" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox4" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox5" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox6" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton3" runat="server">
                                <asp:Image runat="server" ID="Image3" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton4" runat="server">
                                <asp:Image runat="server" ID="Image4" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton5" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr3'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox5" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox6" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox7" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox8" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox9" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton6" runat="server">
                                <asp:Image runat="server" ID="Image5" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton7" runat="server">
                                <asp:Image runat="server" ID="Image6" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton8" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label9" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label10" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label11" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label12" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox7" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox8" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox13" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox14" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox15" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton9" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table2" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>March 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr4'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox9" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox10" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox16" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox17" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox18" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton10" runat="server">
                                <asp:Image runat="server" ID="Image7" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton11" runat="server">
                                <asp:Image runat="server" ID="Image8" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton12" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr5'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox11" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox12" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox19" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox20" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox21" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton13" runat="server">
                                <asp:Image runat="server" ID="Image9" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton14" runat="server">
                                <asp:Image runat="server" ID="Image10" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton15" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label13" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label14" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label15" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label16" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label17" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label18" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox13" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox14" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox22" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox23" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox24" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton16" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table3" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>April 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr6'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox15" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox16" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox25" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox26" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox27" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton17" runat="server">
                                <asp:Image runat="server" ID="Image11" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton18" runat="server">
                                <asp:Image runat="server" ID="Image12" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton19" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr7'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox17" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox18" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox28" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox29" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox30" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton20" runat="server">
                                <asp:Image runat="server" ID="Image13" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton21" runat="server">
                                <asp:Image runat="server" ID="Image14" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton22" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label19" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label20" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label21" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label22" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label23" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label24" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox19" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox20" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox31" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox32" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox33" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton23" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table4" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>May 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr8'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox21" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox22" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox34" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox35" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox36" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton24" runat="server">
                                <asp:Image runat="server" ID="Image15" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton25" runat="server">
                                <asp:Image runat="server" ID="Image16" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton26" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr9'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox23" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox24" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox37" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox38" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox39" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton27" runat="server">
                                <asp:Image runat="server" ID="Image17" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton28" runat="server">
                                <asp:Image runat="server" ID="Image18" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton29" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label25" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label26" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label27" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label28" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label29" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label30" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox25" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox26" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox40" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox41" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox42" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton30" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table5" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>June 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr10'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox27" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox28" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox43" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox44" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox45" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton31" runat="server">
                                <asp:Image runat="server" ID="Image19" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton32" runat="server">
                                <asp:Image runat="server" ID="Image20" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton33" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr11'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox29" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox30" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox46" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox47" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox48" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton34" runat="server">
                                <asp:Image runat="server" ID="Image21" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton35" runat="server">
                                <asp:Image runat="server" ID="Image22" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton36" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label31" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label32" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label33" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label34" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label35" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label36" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox31" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox32" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox49" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox50" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox51" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton37" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table6" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>July 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr12'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox33" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox34" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox52" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox53" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox54" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton38" runat="server">
                                <asp:Image runat="server" ID="Image23" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton39" runat="server">
                                <asp:Image runat="server" ID="Image24" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton40" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr13'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox35" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox39" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox55" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox56" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox57" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton41" runat="server">
                                <asp:Image runat="server" ID="Image25" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton42" runat="server">
                                <asp:Image runat="server" ID="Image26" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton43" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label37" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label38" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label39" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label40" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label41" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label42" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox40" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox41" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox58" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox59" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox60" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton44" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table7" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>August 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr14'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox42" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox43" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox61" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox62" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox63" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton45" runat="server">
                                <asp:Image runat="server" ID="Image27" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton46" runat="server">
                                <asp:Image runat="server" ID="Image28" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton47" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr15'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox44" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox45" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox64" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox65" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox66" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton48" runat="server">
                                <asp:Image runat="server" ID="Image29" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton49" runat="server">
                                <asp:Image runat="server" ID="Image30" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton50" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label43" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label44" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label45" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label46" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label47" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label48" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox46" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox47" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox67" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox68" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox69" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton51" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table8" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>September 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr16'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox48" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox49" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox70" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox71" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox72" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton52" runat="server">
                                <asp:Image runat="server" ID="Image31" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton53" runat="server">
                                <asp:Image runat="server" ID="Image32" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton54" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr17'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox50" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox51" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox73" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox74" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox75" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton55" runat="server">
                                <asp:Image runat="server" ID="Image33" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton56" runat="server">
                                <asp:Image runat="server" ID="Image34" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton57" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label49" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label50" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label51" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label52" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label53" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label54" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox52" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox53" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox76" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox77" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox78" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton58" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table9" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>October 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr18'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox54" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox55" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox79" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox80" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox81" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton59" runat="server">
                                <asp:Image runat="server" ID="Image35" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton60" runat="server">
                                <asp:Image runat="server" ID="Image36" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton61" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr19'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox56" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox57" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox82" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox83" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox84" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton62" runat="server">
                                <asp:Image runat="server" ID="Image37" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton63" runat="server">
                                <asp:Image runat="server" ID="Image38" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton64" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label55" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label56" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label57" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label58" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label59" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label60" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox58" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox59" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox85" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox86" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox87" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton65" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table10" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>November 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr20'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox60" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox61" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox88" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox89" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox90" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton66" runat="server">
                                <asp:Image runat="server" ID="Image39" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton67" runat="server">
                                <asp:Image runat="server" ID="Image40" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton68" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr21'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox62" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox63" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox91" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox92" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox93" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton71" runat="server">
                                <asp:Image runat="server" ID="Image41" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton72" runat="server">
                                <asp:Image runat="server" ID="Image42" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton73" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label61" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label62" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label63" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label64" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label65" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label66" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox64" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox65" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox94" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox95" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox96" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton74" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" id="Table11" cellspacing="0" cellpadding="0" class="metaform-zone">
                    <tr class="gray-header">
                        <td width="2%" class="grey-header">
                            <b>#</b>
                        </td>
                        <td width="30%">
                            <b>December 2014</b>
                        </td>
                        <td width="9%">
                            <b>Amount</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Gross</b>
                        </td>
                        <td width="9%" align="center">
                            <b>Allce</b>
                        </td>
                        <td width="9%" align="center">
                            <b>OT</b>
                        </td>
                        <td width="10%">
                            <b></b>
                        </td>
                    </tr>
                    <tr id='Tr22'>
                        <td>
                            1
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox66" runat="server" Text="Basic" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox67" runat="server" Text="2800" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox97" runat="server" Checked="true" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox98" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox99" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton75" runat="server">
                                <asp:Image runat="server" ID="Image43" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton76" runat="server">
                                <asp:Image runat="server" ID="Image44" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton77" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr id='Tr23'>
                        <td>
                            2
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox68" runat="server" Text="OT" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox69" runat="server" Text="1200" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox100" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox101" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox102" runat="server" Checked="true" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton78" runat="server">
                                <asp:Image runat="server" ID="Image45" ImageUrl="~/Data/Images/navUp.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton79" runat="server">
                                <asp:Image runat="server" ID="Image46" ImageUrl="~/Data/Images/navDown.png" /></asp:LinkButton>
                            <asp:LinkButton ID="LinkButton80" runat="server" Text="X" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Total:</b>
                        </td>
                        <td>
                            <asp:Label ID="Label67" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:Label ID="Label68" runat="server" Font-Bold="true" Text="SGD Total" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label69" runat="server" Font-Bold="true" Text="2800" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label70" runat="server" Font-Bold="true" Text="0" />
                        </td>
                        <td align="center">
                            <asp:Label ID="Label71" runat="server" Font-Bold="true" Text="1200" />
                        </td>
                        <td>
                            <asp:Label ID="Label72" runat="server" Font-Bold="true" Text="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <b>Add:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox70" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox71" runat="server" Text="" Width="100%"></asp:TextBox>
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox103" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox104" runat="server" Checked="false" />
                        </td>
                        <td align="center">
                            <asp:CheckBox ID="CheckBox105" runat="server" Checked="false" />
                        </td>
                        <td>
                            <asp:LinkButton ID="LinkButton81" runat="server" Text="Add" CssClass="button-zone"></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>
    <asp:HiddenField runat="server" ID="NodeValueHiddenField" EnableViewState="false"
        Value="-1" />
    <asp:HiddenField runat="server" ID="NodeCategoryHiddenField" EnableViewState="false"
        Value="-1" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" ClientEvents-OnRequestStart="RequestStarted"
        EnableAJAX="true" DefaultLoadingPanelID="LoadingPanel1" OnAjaxRequest="RadAjaxManager1_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadAjaxManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="VersionDropdownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="MonthYearDropDownList">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelRight" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="IncomeDetailsRadGrid">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="IncomeDetailsRadGrid" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="PreviousButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="LeftSubmitPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" Skin="Sunset">
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadWindow ID="RadWindow1" runat="server" Behaviors="Close,Move,Resize,Maximize"
        Width="600px" Height="510px" VisibleStatusbar='false' Modal="true" Overlay="true">
    </telerik:RadWindow>
    <telerik:RadToolTipManager ID="RadToolTipManager1" OffsetX="-190" HideEvent="ManualClose"
        Width="255" Height="500" runat="server" RelativeTo="Element" Position="BottomRight"
        Skin="Windows7" Animation="None" Modal="True">
    </telerik:RadToolTipManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            var myHeight = getClientHeight();

            function SetSplitterHeight(splitter, args) {
                var myHeight = getClientHeight();
                var height = myHeight - 97;

                var PreviewRadPane = splitter.getPaneById("<%= PreviewRadPane.ClientID %>");
                PreviewRadPane.set_height(height);

                //            

                var TreeRadPane = splitter.getPaneById("<%= TreeRadPane.ClientID %>");
                TreeRadPane.set_height(height);

                //Added By Edward 2014/5/21 for Auto width of PDF pane
                var myWidth = getClientWidth();
                TreeRadPane.set_width(myWidth * .5);
                PreviewRadPane.set_width(myWidth * .5);

                splitter.set_height(height);
                SetHeightForControls()
            }

            function SetTreeRadPaneWidth(splitter) {

            }

            function SetInnerSplitterHeight(splitter, args) {
                var myHeight = getClientHeight();
                var height = myHeight - 167;





                var frame = document.getElementById("<%= pdfframe.ClientID %>");
                if (frame) {
                    frame.style.height = height + "px";
                }

                splitter.set_height(height);
            }

            function SetHeightForControls() {
                var myHeight = getClientHeight();
                var height = (myHeight - 172) + "px";

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

            function SetPDFSize(w, h) {
                var splitter = $find("<%= RadSplitter1.ClientID %>");
                splitter.set_height(h);
            }

            function OnNodeExpanded(sender, args) {
                var category = args.get_node().get_category();
                var expandedNode = args.get_node();

                if (category == "Set") {
                    //CollapseSiblings(expandedNode);
                }
            }

            function rtvExplore_OnNodeExpandedCollapsed(sender, eventArgs) {
                var allNodes = eventArgs._node.get_treeView().get_allNodes();
                var i;
                var unSelectedNodes = "";

                for (i = 0; i < allNodes.length; i++) {
                    if (!allNodes[i].get_expanded())
                        unSelectedNodes += allNodes[i].get_value() + "*";
                }

            }

            function RefreshUrl(url) {
                window.location = url;
            }

            function CollapseSiblings(expandedNode) {

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



            function reloadPage() {
                window.location.reload()
            }




            var hasChanges, inputs, dropdowns, editedRow;

            function RowClick(sender, eventArgs) {

                if (editedRow && hasChanges) {
                    hasChanges = false;
                    //                    if (confirm("Update changes?")) {


                }
                // }
            }

            function RowDblClick(sender, eventArgs) {
                editedRow = eventArgs.get_itemIndexHierarchical();

            }

            function GridCommand(sender, args) {
                alert("grid command");
                if (args.get_commandName() != "Edit") {
                    editedRow = null;
                }
            }

            function GridCreated(sender, eventArgs) {

                var gridElement = sender.get_element();
                var elementsToUse = [];
                inputs = gridElement.getElementsByTagName("input");
                for (var i = 0; i < inputs.length; i++) {
                    var lowerType = inputs[i].type.toLowerCase();
                    if (lowerType == "hidden" || lowerType == "button") {
                        continue;
                    }
                    if (inputs[i].id.indexOf("PageSizeComboBox") == -1 && inputs[i].type.toLowerCase() != "checkbox") {
                        Array.add(elementsToUse, inputs[i]);
                    }
                    inputs[i].onchange = TrackChanges;
                }

                dropdowns = gridElement.getElementsByTagName("select");
                for (var i = 0; i < dropdowns.length; i++) {
                    dropdowns[i].onchange = TrackChanges;
                }

                setTimeout(function () { if (elementsToUse[0]) elementsToUse[0].focus(); }, 100);
            }

            function TrackChanges(e) {
                hasChanges = true;
            }

            function SetText(option) {
                if (option == 'replace') {
                    if (confirm("Replace all items?")) {
                        var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                        ajaxManager.ajaxRequest(option);
                    }
                }
                else {
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest(option);
                }
            }

            function DeleteItems() {

                //                
            }

            function DivideAssessment() {
                if (confirm("This will divide the items. Do you want to continue?")) {

                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest('divide');
                }
            }

            //Added by Edward 30/01/2014 Navigation icons for Sales and Resales Changes
            function Navigating(action) {
                var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                ajaxManager.ajaxRequest(action);

            }

            function SetIncomeToBlank() {
                if (confirm("Do you want to set the Income to blank?")) {
                    var ajaxManager = $find("<%= RadAjaxManager1.ClientID %>");
                    ajaxManager.ajaxRequest('blank');
                }
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

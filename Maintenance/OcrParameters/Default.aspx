<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true" ValidateRequest="false"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_OcrParameters_Default" Title="DWMS - OCR Parameters" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        OCR Parameters</div>
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
                        <span class="label">Maximum No. of Sample <br />Documents per Document Type </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MaxSampleDocsDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Maximum Threads (OCR Engine) </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="ThreadDropDownList" runat="server" CssClass="form-field" Width="75px">
                            <asp:ListItem Value="1">1</asp:ListItem>
                            <asp:ListItem Value="2">2</asp:ListItem>
                            <asp:ListItem Value="3">3</asp:ListItem>
                            <asp:ListItem Value="4">4</asp:ListItem>
                            <asp:ListItem Value="5">5</asp:ListItem>
                            <asp:ListItem Value="6">6</asp:ListItem>
                            <asp:ListItem Value="7">7</asp:ListItem>
                            <asp:ListItem Value="8">8</asp:ListItem>
                            <asp:ListItem Value="9">9</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="16">16</asp:ListItem>
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                            <asp:ListItem Value="24">24</asp:ListItem>
                            <asp:ListItem Value="25">25</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;Thread (s)
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Maximum Document Pages </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MaxPagesDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        &nbsp;Page (s)
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum Age to Process External Files </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="ExtFileAgeDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        &nbsp;Min (s)
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum Age to Remove Temporary Files </span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="TempFileAgeDropDownList" runat="server" CssClass="form-field" Width="75px">
                            <asp:ListItem Value="1">1</asp:ListItem>
                            <asp:ListItem Value="2">2</asp:ListItem>
                            <asp:ListItem Value="3">3</asp:ListItem>
                            <asp:ListItem Value="4">4</asp:ListItem>
                            <asp:ListItem Value="5">5</asp:ListItem>
                            <asp:ListItem Value="6">6</asp:ListItem>
                            <asp:ListItem Value="7">7</asp:ListItem>
                            <asp:ListItem Value="8">8</asp:ListItem>
                            <asp:ListItem Value="9">9</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;Day (s)
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum English Word Count</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MinimumEnglishWordCountDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        <span class="form-note">The English word count in source and sample OCR text must be >= this to qualify for comparison.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum English Word Percentage</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MinimumEnglishWordPercentageDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        <span class="form-note">The English word percentage in source and sample OCR text must be >= this to qualify for comparison.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum Score</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MinimumScoreDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        <span class="form-note">Samples with score < this will be ignored.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Minimum Word Length</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MinimumWordLengthDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        <span class="form-note">A word in source and sample OCR text must be >= this length to qualify for comparison.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Keyword Check Scope</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="KeywordCheckScopeDropDownList" runat="server" CssClass="form-field" Width="75px">
                        </asp:DropDownList>
                        <span class="form-note">Perform keyword condition check for top N document types with highest score.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Percentage of Sample Documents<br />For Document Type Determination</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="SampleDocPercentDropDownList" runat="server" CssClass="form-field" Width="75px">
                            <asp:ListItem Value="0.10">10</asp:ListItem>
                            <asp:ListItem Value="0.20">20</asp:ListItem>
                            <asp:ListItem Value="0.30">30</asp:ListItem>
                            <asp:ListItem Value="0.40">40</asp:ListItem>
                            <asp:ListItem Value="0.50">50</asp:ListItem>
                            <asp:ListItem Value="0.60">60</asp:ListItem>
                            <asp:ListItem Value="0.70">70</asp:ListItem>
                            <asp:ListItem Value="0.80">80</asp:ListItem>
                            <asp:ListItem Value="0.90">90</asp:ListItem>
                            <asp:ListItem Value="1">100</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;%
                        <span class="form-note">Percentage of sample pages to use for determining the document type.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Binarize</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="BinarizeDropDownList" runat="server" CssClass="form-field" Width="75px">                            
                        </asp:DropDownList>
                        <span class="form-note">It can control the way that color images are processed and force binarization with a particular threshold. A value of 200 has been shown to generally give good results in testing.
                        By setting this to -1 an alternative method is used which will attempt to separate the text from any background images or colors. This can give improved OCR results for certain documents such as newspaper and magazine pages.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Morph</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="MorphDropDownList" runat="server" CssClass="form-field" Width="75px">                            
                            <asp:ListItem Value="d2.2">d2.2</asp:ListItem>
                            <asp:ListItem Value="e2.2">e2.2</asp:ListItem>
                            <asp:ListItem Value="c2.2">c2.2</asp:ListItem>
                        </asp:DropDownList>
                        <span class="form-note">Morphological options that will be applied to the binarized image before OCR.<br />d2.2 – 2x2 dilation applied to all black pixel areas, useful for faint prints.
                        <br />e2.2 – 2x2 erosion applied to all black pixel areas, useful for heavy prints.
                        <br />c2.2 – closing process that performs a 2x2 dilation followed by a 2x2 erosion with the result that holes and gaps in the characters are filled.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">MRC Background Factor</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="BackgroundFactorDropDownList" runat="server" CssClass="form-field" Width="75px">                            
                            <asp:ListItem Value="1">1</asp:ListItem>
                            <asp:ListItem Value="2">2</asp:ListItem>
                            <asp:ListItem Value="3">3</asp:ListItem>
                            <asp:ListItem Value="4">4</asp:ListItem>
                            <asp:ListItem Value="5">5</asp:ListItem>
                        </asp:DropDownList>
                        <span class="form-note">Sampling size for the background portion of the image. The higher the number, the larger the size of the image blocks used for averaging which will result in a reduction in size but also quality. Default value is 3.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">MRC Foreground Factor</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="ForegroundFactorDropDownList" runat="server" CssClass="form-field" Width="75px">                            
                            <asp:ListItem Value="1">1</asp:ListItem>
                            <asp:ListItem Value="2">2</asp:ListItem>
                            <asp:ListItem Value="3">3</asp:ListItem>
                            <asp:ListItem Value="4">4</asp:ListItem>
                            <asp:ListItem Value="5">5</asp:ListItem>
                        </asp:DropDownList>
                        <span class="form-note">Sampling size for the foreground portion of the image. The higher the number, the larger the size of the image blocks used for averaging which will result in a reduction in size but also quality. Default value is 3.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">MRC Quality</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:DropDownList ID="QualityDropDownList" runat="server" CssClass="form-field" Width="75px">                            
                        </asp:DropDownList>
                        <span class="form-note">JPEG quality setting (percentage value 1 - 100) for use in saving the background and foreground images. Default value is 75.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Dot Matrix</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                        <asp:RadioButtonList ID="DotMatrixList" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Table" CssClass="list-item-table hand" Width="100">
                            <asp:ListItem Selected="True" Value="True">Yes</asp:ListItem>
                            <asp:ListItem Value="False">No</asp:ListItem>
                        </asp:RadioButtonList>
                        <span class="form-note">Set this to "Yes" to improve recognition of dot-matrix fonts. Default value is "No". If set to "Yes" for non dot-matrix fonts then the recognition can be poor.</span>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <span class="label">Despeckle</span><span class="form-error">
                            &nbsp;*</span>
                    </td>
                    <td valign="top">
                         <asp:DropDownList ID="DespeckleDropDownList" runat="server" CssClass="form-field" Width="75px">
                            <asp:ListItem Value="0">0</asp:ListItem>
                            <asp:ListItem Value="1">1</asp:ListItem>
                            <asp:ListItem Value="2">2</asp:ListItem>
                            <asp:ListItem Value="3">3</asp:ListItem>
                            <asp:ListItem Value="4">4</asp:ListItem>
                            <asp:ListItem Value="5">5</asp:ListItem>
                            <asp:ListItem Value="6">6</asp:ListItem>
                            <asp:ListItem Value="7">7</asp:ListItem>
                            <asp:ListItem Value="8">8</asp:ListItem>
                            <asp:ListItem Value="9">9</asp:ListItem>
                        </asp:DropDownList>
                        <span class="form-note">Removes all disconnected elements within the image that have height or width in pixels less than the specified figure. The maximum value is 9 and the default value is 0.</span>
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

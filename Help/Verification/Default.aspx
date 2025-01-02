<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Verifying Document Sets
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Picking Up a Document Set to Perform Verification
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Verification in main navigation menu and click Pending Assignment.</li>
                <li>Click the Verify link of a set. You will be brought to the Verification page.</li>
            </ol>
            <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/Verification_Pending_Assignment.JPG"
                BorderWidth="1" CssClass="helpImage" />
            <br />
            <br />
            <div class="helpnote">
                If document sets are supposed to be assigned to you to perform verification by your
                supervisor, you do not need to pick the sets yourself. Instead, you may go to the
                Assigned to Me page to view the sets assigned to you. To go to this page, mouse
                over Verification in main navigation menu and click Assigned to Me.
            </div>
        </div>
        <div class="header">
            <div class="left">
                Verifying a Document Set
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <asp:Image ID="Image3" runat="server" ImageUrl="~/Data/Images/Help/Verification_Page.JPG"
                BorderWidth="1" CssClass="helpImage" />
            <br />
            <br />
            <ol type="a">
                <li>If the Verification page, if there are documents under the Unidentified folder in
                    the Household Structure, click on the first PDF document, identify the document
                    type by examining its content in the PDF View, then drag and drop it to the appropriate
                    folder. </li>
                <li>Repeat step a) until all documents in the Unidentified folder are cleared.</li>
                <li>Click on the first PDF document, scroll down page by page to verify that the sequence
                    is in order. Otherwise, click on the Thumbnail button to open the Thumbnail View,
                    and then use drag-and-drop to arrange the page sequence.</li>
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Data/Images/Help/Verification_Page_Details_View.JPG"
                    CssClass="helpImage" />
                <li>Click on the Image button, fill in the fields in the Metadata panel and click Confirm.
                    If you are unable to provide all the mandatory metadata (i.e., fields marked with
                    an asterisk), fill in partial data and click Save button. </li>
                <li>Once you have cleared all documents in the Unidentified folder and confirmed all
                    documents in the HLE and Others folder, you may confirm the set by clicking the
                    Confirm button on the top left of the page. </li>
            </ol>
            <br />
        </div>
        <div class="header">
            <div class="left">
                View Document Sets Assigned to You
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Verification in main navigation menu and click Assigned to Me. </li>
                <li>Click the Set Number link of a set. You will be brought to the Verification page.</li>
            </ol>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Data/Images/Help/Verification_View Document Sets Assigned to You.JPG"
                BorderWidth="1" CssClass="helpImage" />
            <br />
            <br />
            <div class="helpnote">
                By default, the Assigned to Me page displays document sets with Verification In
                Progress status. To view sets assigned to you in another status, select another
                option in the status dropdown list and click Search.
            </div>
        </div>
    </asp:Panel>
</asp:Content>

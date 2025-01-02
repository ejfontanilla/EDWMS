<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Checking Applications for Completeness
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Picking Up an Application to Perform Completeness Check
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Completeness in main navigation menu and click Pending Assignment.</li>
                <li>Click the Check link of a set. You will be brought to the Completeness Check page.</li>
            </ol>
            <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Checking_Pending_Assignment.JPG"
                BorderWidth="1" CssClass="helpImage" />
            <br />
            <br />
            <div class="helpnote">
                If document sets are supposed to be assigned to you to perform completeness check
                by your supervisor, you do not need to pick the applications yourself. Instead,
                you may go to the Assigned to Me page to view the applications assigned to you.
                To go to this page. mouse over Completeness in main navigation menu and click Assigned
                to Me.
            </div>
        </div>
        <div class="header">
            <div class="left">
                Indicate Acceptance of Documents
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>In the Completeness Check page, mouse over the Yes or No link in the Acceptance
                    column of the Summary Table. Enter the Exception reason if you specify not to accept
                    the document, and then click Save. </li>
                <li>To examine the content and metadata of a document, click it in the Household Structure.
                    Note that you may edit the metadata if the information captured earlier is incorrect.
                    To return to the Summary Table, click the Reference Number in the Household Structure.
                </li>
                <li>Repeat step a) and b) for all documents. </li>
                <li>Confirm the application by clicking the Confirm button on the top left of the page.
                </li>
            </ol>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Indicate Acceptance of Documents.JPG"
                BorderWidth="1" CssClass="helpImage" />
        </div>
        <div class="header">
            <div class="left">
                View Applications Assigned to You
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Completeness in main navigation menu and click Assigned to Me. </li>
                <li>Click the Ref No link of a set. You will be brought to the Completeness Check page.
                </li>
            </ol>
            <asp:Image ID="Image3" runat="server" ImageUrl="~/Data/Images/Help/Completeness_View Applications Assigned to You.JPG"
                BorderWidth="1" CssClass="helpImage" />
            <br />
            <br />
            <div class="helpnote">
                By default, the Assigned to Me page displays applications with Completeness In Progress
                status. To view applications assigned to you in another status, select another option
                in the status dropdown list and click Search.
            </div>
        </div>
        <div class="header">
            <div class="left">
                Search and View the HLE case by criteria (e.g. Ref No.)
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Search for a HLE by typing in the Ref. No. </li>
                <br />
                <br />
                <asp:Image ID="Image6" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Search for a HLE.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>To look at the case, click on the Ref. No.'s link</li>
                <br />
                <br />
                <asp:Image ID="Image7" runat="server" ImageUrl="~/Data/Images/Help/Completeness_View HLE case.JPG"
                    BorderWidth="1" CssClass="helpImage" />
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Edit the HLE Case
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Edit the fields by either clicking on the buttons, selecting from the dropdown list
                    or filling in the field </li>
                <br />
                <br />
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Editing HLE case 1.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Changing the fields in the HLE case changes the respective fields in the Summary
                    Table after confirmation</li>
                <br />
                <br />
                <asp:Image ID="Image5" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Editing HLE case 2.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>e.g. Changing Acceptance ‘Yes’ to ‘No’</li>
                <br />
                <br />
                <asp:Image ID="Image8" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Editing HLE case 3.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Document Condition, Remark, & Acceptance can also be changed in Summary Table view
                    via
                    <asp:Image ID="Image9" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Editing pencil.JPG"
                        BorderWidth="1" CssClass="helpImage" />
                </li>
                <br />
                <br />
                <asp:Image ID="Image10" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Editing HLE case 4.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
            </ol>
        </div>
        <div class="header">
            <div class="left">
                View Details
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Click on the desired HLE from the Household Structure</li>
                <br />
                <br />
                <asp:Image ID="Image11" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Overview HLE case 1.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Click on the Summary button</li>
                <br />
                <br />
                <asp:Image ID="Image12" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Overview HLE case 2.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Show/Hide Documents (Shows/hides documents under the particular Applicant/Occupier)</li>
                <br />
                <br />
                <asp:Image ID="Image13" runat="server" ImageUrl="~/Data/Images/Help/Completeness_Overview HLE case 3.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
            </ol>
        </div>
    </asp:Panel>
</asp:Content>

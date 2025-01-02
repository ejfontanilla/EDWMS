<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Maintenance
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                View Maintenance
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Maintenance in main navigation menu and click any item.</li>
                <br />
                <br />
                <div class="helpnote">
                    Items can be clicked from the left navigation panel also.
                </div>
                <br />
                <br />
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/Maintenance_View List.JPG"
                    BorderWidth="1" CssClass="helpImage" />
            </ol>
        </div>
        <div class="header">
            <div class="left">
                User Accounts
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        View
            <ol type="a">
                <li>Click the Name link to view the details.</li>
            </ol>
        Edit
            <ol type="a">
                <li>Click the Edit button to edit the details.</li>
                <li>Once you are done editing details, click Save button.</li>
            </ol>
        New
            <ol type="a">
                <li>Click the New button to add a new user.</li>
               <li>Once you are done entering details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Role Management
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>Click Edit link to edit the details from the Action column.</li>
                 <li>Once you are done editing details, click Save button.</li>
            </ol>
        Delete
            <ol type="a">
                <li>Click Delete from the Action column.</li>
                <li>Click OK to confirm deletion.</li>
            </ol>
        New
            <ol type="a">
                <li>Click New to add a new role.</li>
                  <li>Once you are done entering details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Access Control
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        View
            <ol type="a">
                <li>You may use the dropdown list to select the desired Department.</li>
            </ol>
        Edit 
            <ol type="a">
                <li>Check mark to grant access control to respective Users and Functions as you wish.</li>
                 <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Control Parameters
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>You can enter information you wish to provide and change in the textboxes, and select
                    relevant options you wish to set.</li>
                 <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Document Types
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        View
            <ol type="a">
                <li>You may use the filter options at the top of the list to filter out the list of
                    document types.</li>
                <li>You may use the pagination controls at the bottom of the list to browse through
                    the list of document types.</li>
            </ol>
        Edit
            <ol type="a">
                <li>Click the Document ID or Document Type link to view the details.</li>
                <li>If you wish to edit information, you can click the respectve Edit buttons to edit
                    the details.</li>
                 <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Street Names
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>You may fill the neccessary details.</li>
                 <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Master List
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>You may fill the neccessary details.</li>
               <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Organization
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>Click Edit in the edit column to edit the details.</li>
                  <li>Once you are done editing details, click Save button.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Import Interface Files
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
        Edit
            <ol type="a">
                <li>You may select required tabs to add information.</li>
               <li>Once you are done editing details, click Import button.</li>
            </ol>
        </div>
    </asp:Panel>
</asp:Content>

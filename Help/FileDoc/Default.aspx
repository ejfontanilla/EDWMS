<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Download File Docs
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                View Files for Download
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over File Doc in main navigation menu and click Download</li>
            </ol>
            <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/FileDoc_View Download List.JPG"
                BorderWidth="1" CssClass="helpImage" />
        </div>
        <div class="header">
            <div class="left">
                Apply Filters to the Download List
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Use the dropdown lists to select filter criteria in the list, you can also type
                    in the dropdown box.</li>
                <li>Click Search button to apply the filter.</li>
            </ol>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Data/Images/Help/FileDoc_Filter Download List.JPG"
                BorderWidth="1" CssClass="helpImage" />
        </div>
        <div class="header">
            <div class="left">
                Download File(s)
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Check mark the the items to download</li>
                <br />
                <br />
                <div class="helpnote">
                    Please check mark only one item at a time to download.
                </div>
                <br />
                <br />
                <li>Click Download button</li>
                <br />
                <br />
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Data/Images/Help/FileDoc_Download Item.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>Check mark the the items to download</li>
                <li>Click Download as Zip button</li>
                <br />
                <br />
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Data/Images/Help/FileDoc_Download Item Details.JPG"
                    BorderWidth="1" CssClass="helpImage" />
            </ol>
        </div>
    </asp:Panel>
</asp:Content>

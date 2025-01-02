<%@ Page Language="C#" MasterPageFile="~/Help/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Help_Guides_Default" Title="DWMS - Help" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Importing Documents
    </div>
    <asp:Panel ID="FormPanel" runat="server" CssClass="inputform">
        <div class="header">
            <div class="left">
                Installing Scanning Program
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            Before you can scan documents, the scanning program must be installed on your computer.
            Please follow the steps below to install the program.
            <ol type="a">
                <li>Plug in scanner.</li>
                <li>Mouse over Import in main navigation menu and click Scan Documents.</li>
                <li>Internet Explorer (IE) prompts to install Microsoft License Manager DLL *. Click
                    Install.</li>
                <li>Refresh the page by pressing F5.</li>
                <li>IE prompts to install Dynamic Web Twain. Click Install.</li>
                <li>Close, re-launch IE and log into the system.</li>
            </ol>
            <div class="helpnote">
                * If there is no prompt, follow the steps below:
                <ol type="a">
                    <li>Follow IE’s notification to download signed ActiveX controls.</li>
                    <li>Run ActiveX Controls and plug-ins.</li>
                    <li>Internet Explorer (IE) prompts to install Microsoft License Manager DLL *. Click
                        Install.</li>
                    <li>Script ActiveX controls marked safe for scripting.</li>
                </ol>
            </div>
        </div>
        <div class="header">
            <div class="left">
                Scanning Documents
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Import in main navigation menu and click Scan Documents.</li>
                <br />
                <br />
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Data/Images/Help/Importing_Scan Documents Page.JPG"
                    BorderWidth="1" CssClass="helpImage" />
                <br />
                <br />
                <li>In the Ref No field, enter an application reference number in the format N11N12345;
                    or click on the dropdown list to select an existing reference number.</li>
                <li>Select Channel and Scanner.</li>
                <li>Check ADF if you are scanning multiple pages in one go using the scanning’s Automatic
                    Document Feeder.</li>
                <li>Click Scan, and follow screen instructions if you are prompted to specify the scanning
                    options. Once all pages are scanned, they will appear in the preview panel below
                    the “Scan Documents” title.</li>
                <li>Repeat step a) to e) if you need to scan another batch of documents.</li>
                <li>Click Save. You will be notified when the scanned documents are saved. These documents
                    will be processed at the backend and will be ready for verification shortly. To
                    check if the documents are ready for verification, mouse over Verification in main
                    navigation menu and click Imported by Me.</li>
            </ol>
        </div>
        <div class="header">
            <div class="left">
                Uploading Documents
            </div>
            <div class="right">
                <!---->
            </div>
        </div>
        <div class="area">
            <ol type="a">
                <li>Mouse over Import in main navigation menu and click Upload Documents.</li>
                <li>In the Ref No field, enter an application reference number in the format N11N12345;
                    or click on the dropdown list to select an existing reference number.</li>
                <li>Select Channel.</li>
                <li>Click Browse to select the documents to upload. You may use the Ctrl or Shift key
                    to select multiple documents at a time.</li>
                <li>Click Upload. You will be notified when the scanned documents are saved. These documents
                    will be processed at the backend and will be ready for verification shortly. To
                    check if the documents are ready for verification, mouse over Verification in main
                    navigation menu and click Imported by Me.</li>
            </ol>
            <asp:Image ID="Image1" runat="server" ImageUrl="~/Data/Images/Help/Importing_Upload Documents.JPG"
                BorderWidth="1" CssClass="helpImage" />
        </div>
    </asp:Panel>
</asp:Content>

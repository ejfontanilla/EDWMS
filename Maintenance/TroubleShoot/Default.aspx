<%@ Page Language="C#" MasterPageFile="~/Maintenance/Main.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Maintenance_TroubleShooting_Default" Title="DWMS - Trouble Shoot" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
<b>Check Raw Files</b><br /><br />

Start SetId <asp:TextBox runat="server" ID="startSetIdTextBox"></asp:TextBox>
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="startSetIdTextBox"
 ErrorMessage="Please Enter Only Numbers" ValidationExpression="^\d+$"></asp:RegularExpressionValidator>
 <asp:RequiredFieldValidator runat="server" ID="r1" ControlToValidate="startSetIdTextBox" 
 ErrorMessage="Please enter docset id."></asp:RequiredFieldValidator>
 <br />
End Set Id <asp:TextBox runat="server" ID="endSetIdTextBox"></asp:TextBox>
<asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="endSetIdTextBox"
 ErrorMessage="Please Enter Only Numbers" ValidationExpression="^\d+$" ValidationGroup="check"></asp:RegularExpressionValidator>
 <asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="endSetIdTextBox" 
 ErrorMessage="Please enter docset id."></asp:RequiredFieldValidator>
 <br /><br />
&nbsp;&nbsp;<asp:Button runat="server" Text="Submit" ID="Submit" onclick="Submit_Click" />
<br /><br />
<asp:Label runat="server" ID="InfoLabel"></asp:Label>
    <telerik:RadGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" AllowSorting="True"
        Skin="Windows7" BorderColor="#EEEEEE" GridLines="None" AllowPaging="True" PageSize="200000"
         EnableLinqExpressions="False" 
         OnItemDataBound="RadGrid1_ItemDataBound">
        <PagerStyle Mode="NextPrevAndNumeric" />
       
        <MasterTableView ClientDataKeyNames="Id">
            <ItemStyle CssClass="pointer" />
            <AlternatingItemStyle CssClass="pointer" />
            <SortExpressions>
                <telerik:GridSortExpression FieldName="Id" SortOrder="Ascending"></telerik:GridSortExpression>
            </SortExpressions>
            <NoRecordsTemplate>
                <div class="wrapper10">
                    No records were found.
                </div>
            </NoRecordsTemplate>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="DocSetId" UniqueName="Id" DataField="Id"
                    AutoPostBackOnFilter="true" DataType="System.Int32" SortExpression="Id">
                    <ItemTemplate>
                        <%#Eval("Id")%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="RawFileId | Status" UniqueName="FileExist"
                     AutoPostBackOnFilter="true" DataType="System.String"
                    SortExpression="FileName" AllowFiltering="false">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="FileExistLabel"></asp:Label>
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
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="Submit">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>

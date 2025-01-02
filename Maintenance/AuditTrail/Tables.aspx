<%@ Page Language="C#" MasterPageFile="~/Blank.master" AutoEventWireup="true"
    CodeFile="Tables.aspx.cs" Inherits="Maintenance_AuditTrail_Tables" Title="DWMS - Table Information" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainColumn" runat="Server">
    <div class="title">
        Table Information</div>
    <div id="table-info">
        <table>
            <tr>
                <td class="header">
                    <b>Table</b>
                </td>
                <td class="header">
                    <b>Description</b>
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    AccessControl
                </td>
                <td>
                    Stores information about the users' permissions to modules and functions
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    AppDocRef
                </td>
                <td>
                    Maps information about the documents and applications
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    AppPersonal
                </td>
                <td>
                    Keeps track of applicant, occupier and other details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    AppPersonalSalary
                </td>
                <td>
                    Keeps track of HLE applicants' salary information
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    CategorisationRule
                </td>
                <td>
                    Stores rules for categorisation
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    CategorisationRuleKeyword
                </td>
                <td>
                    Store keywords used in categorisation
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    Department
                </td>
                <td>
                   Store information about the department
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    DocApp
                </td>
                <td>
                    Keeps track of applications
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    Doc
                </td>
                <td>
                   Keeps track of documents
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    DocPersonal
                </td>
                <td>
                    Keeps track of NRIC of documents
                </td>
            </tr>
             <tr>
                <td class="form-note">
                    DocSet
                </td>
                <td>
                    Keeps track of document set information
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    Interface
                </td>
                <td>
                    Stores information about interface details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    InterfaceIncomeComputation
                </td>
                <td>
                     Stores information about Income computation details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    InterfaceSalary
                </td>
                <td>
                    Stores information about salary details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    LogAction
                </td>
                <td>
                    Logs all user actions
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    MasterListItem
                </td>
                <td>
                    Keeps track of master list data such as upload, scanning channels, and etc
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    MetaData
                </td>
                <td>
                    Store information about property values of documents
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    MetaField
                </td>
                <td>
                    Stores information about protperty items of documents
                </td>
            </tr>
             <tr>
                <td class="form-note">
                    Parameter
                </td>
                <td>
                    Keeps track of parameter values
                </td>

            </tr>
            <tr>
                <td class="form-note">
                    Profile
                </td>
                <td>
                    Keeps track of information about users
                </td>

            </tr>
            <tr>
                <td class="form-note">
                    RawFile
                </td>
                <td>
                    Keeps track of files uploaded
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    RelevanceRanking
                </td>
                <td>
                    Keeps track of relevance ranking details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    ResaleInterface
                </td>
                <td>
                    Keeps track of interface details from Resale
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    RoleToDepartment
                </td>
                <td>
                    Maps the role information to departments
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    SalesInterface
                </td>
                <td>
                    Keeps track of interface details from Sales
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    SampleDoc
                </td>
                <td>
                    Keeps track of sample documents for each doc types
                </td>
            </tr>
            
            <tr>
                <td class="form-note">
                    Section
                </td>
                <td>
                    Keeps track of sections under each department
                </td>
            </tr>
            <tr>
                <td class="form-note">
                    SersInterface
                </td>
                <td>
                     Keeps track of interface details from Sers
                </td>
            </tr>

            <tr>
                <td class="form-note">
                    SetApp
                </td>
                <td>
                    Maps information document sets and applications
                </td>
            </tr>
            <tr>
                <td class="form-note">
                  SetDocRef
                </td>
                <td>
                    Maps information document documents with personal details
                </td>
            </tr>
            <tr>
                <td class="form-note">
                  Street
                </td>
                <td>
                    Stores information about street details
                </td>
            </tr>
           
           
          

         
        </table>
    </div>
</asp:Content>

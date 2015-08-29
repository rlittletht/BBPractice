<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Resources.aspx.cs" Inherits="WebCal.Resources" MasterPageFile="~/WebCal.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
	<script language="javascript">
	</script>
	
	<script type="text/javascript" src="Common.js"/></script>
    <script type="text/javascript" src="Resources.js"/></script>
    <title></title>
 </asp:Content>

<asp:Content ID="LocalContent" ContentPlaceholderID="LocalContent" Runat="Server">
	<p class="NoMargins">Add/Edit Field Resources</p>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <table ID="TableData" class="DataTable" style="display: none">
        <thead>
            <tr>
                <td class="IconColumn"></td>
                <td class="IDColumn">ID</td>
                <td class="NameColumn"> Field Name</td>
                <td class="DescriptionColumn">Description</td>
                <td class="ActionColumn"> </td>
            </tr>
        </thead>
        <tr ID=TemplateRow class="HiddenRow">   
            <td class="IconColumn">&nbsp;</td>
            <td class="IDColumn">&nbsp;</td>
            <td class="NameColumn">&nbsp;</td>
            <td class="DescriptionColumn"> &nbsp;</td>
            <td class="ActionColumn">&nbsp;</td>
        </tr>
        <tr>
            <td class="IconColumn">
                *
            </td>
            <td class="IDColumn">
                <input type="text" id="AddID" class="FormControl"/>
            </td>
            <td class="NameColumn">
                <input type="text" id="AddName" class="FormControl" />
            </td>
            <td  class="DescriptionColumn">
                <input type="text" id="AddDescription" class="FormControl"/>
            </td>
            <td class="ActionColumn"> 
                <button id="Button2" onclick="AddResources(); return false;">Add</button> 
            </td>
        </tr>
    </table>
</asp:Content>

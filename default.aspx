<%@ Page Title="" Language="C#" MasterPageFile="~/WebCal.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WebCal.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript">
    function pageLoadLocal()
    {
        ShowUser();
    }

    function ShowHideAdmin()
    {
        var oDiv = document.getElementById("AdminMastHead");
        oDiv.style.display = "none";
    }

    function AfterLogin()
    {
    }

    </script>
    <script type="text/javascript" src="Common.js"/>
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="LocalContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="AdminContent" runat="server">
</asp:Content>

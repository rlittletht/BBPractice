<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Summary.aspx.cs" Inherits="WebCal.Summary" MasterPageFile="~/WebCal.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%
// Session["Debug"] = "1";
// Session["DebugLevel"] = "1"; 
%>

    <link rel="Stylesheet" href="Summary.css"/>
    <title></title>

    <script type="text/javascript" src="common.js"/>
    </script>
    <script type="text/javascript" src="SummaryEditSlots.js"/>
    </script>
    <script type="text/javascript" src="SlotsCommon.js"/>
    </script>
    <script type="text/javascript" src="Summary.js"/>
    </script>

    <style type="text/css">
        .style1
        {
            height: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="LocalContent" ContentPlaceholderID="LocalContent" Runat="Server">
	<p class="NoMargins">Slot Summary Report</p>
</asp:Content>

<asp:Content ID="AdminContent" ContentPlaceholderID="AdminContent" Runat="Server">
	<button id="btnUpload" runat="server" onclick="DoUploadSlots();return false;" >
		Upload
	</button>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <table class="OuterPanelContainer">
        <tr>
            <td>
                <p class="NoMargins" align="right">
                    <input type="checkbox" ID="chkGroupItems" CHECKED
                    onclick='ClickGroupItems()'>Group Items
                </p>
                <table class="PanelContainer">
                    <tr>
                        <td>
                            <table id="tblEditSlots_panelData" class="PanelData">
									<col/> <!--expando-->
									<col/> <!--location-->
									<col/> <!--date-->
									<col/> <!--start-->
									<col/> <!--length-->
									<col/> <!--owner-->
									<col/> <!--reserved-->
									<col/> <!--restriction-->
									<col/> <!--actions-->
                                <tr id="rowEditSlots_Headings">
                                    <td class="SlotHeading Expando">
                                        <p class="NoMargins">&nbsp;&nbsp;</p>
                                    </td>
                                    <td class="SlotHeading SortOrder SlotLocation" onclick="UpdateSortOrder(1)">
                                        <p class="NoMargins">Location</p>
                                    </td>
                                    <td class="SlotHeading SlotDate" onclick="UpdateSortOrder(2)">
                                        <p class="NoMargins">Date</p>
                                    </td>
                                    <td class="SlotHeading SlotStart" onclick="UpdateSortOrder(3)">
                                        <p class="NoMargins">Start</p>
                                    </td>
                                    <td class="SlotHeading SlotLength" onclick="UpdateSortOrder(4)">
                                        <p class="NoMargins">Length</p>
                                    </td>
                                    <td class="SlotHeading SlotOwner" onclick="UpdateSortOrder(5)">
                                        <p class="NoMargins">Owner</p>
                                    </td>
                                    <td class="SlotHeading SlotReserved" onclick="UpdateSortOrder(6)">
                                        <p class="NoMargins">Reserved</p>
                                    </td>
                                    <td class="SlotHeading SlotRestrictions" onclick="UpdateSortOrder(7)">
                                        <p class="NoMargins">Restrictions</p>
                                    </td>
                                    <td class="SlotHeading SlotActions">
                                        <p class="NoMargins">&nbsp;</p>
                                    </td>
                                </tr>
                                <tr class="HiddenRow">
                                    <td class="SlotData">
                                        <p class="NoMargins">&nbsp;</p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                    <td class="SlotData">
                                        <p class="NoMargins"></p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>


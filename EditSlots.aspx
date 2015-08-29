<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSlots.aspx.cs" Inherits="WebCal.EditSlots" MasterPageFile="~/WebCal.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<%
//	Session["Debug"] = "1";
//	Session["DebugLevel"] = "1"; 
%>

    <link rel="Stylesheet" href="EditSlots.css"/>
    <title></title>

    <script type="text/javascript" src="Common.js"/>
    </script>
    <script type="text/javascript" src="SummaryEditSlots.js"/>
    </script>
    <script type="text/javascript" src="SlotsCommon.js"/>
    </script>
    <script type="text/javascript" src="EditSlots.js"/>
    </script>

    <style type="text/css">
        .style1
        {
            height: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="LocalContent" ContentPlaceholderID="LocalContent" Runat="Server">
	<p class="NoMargins">Reservations</p>
</asp:Content>

<asp:Content ID="AdminContent" ContentPlaceholderID="AdminContent" Runat="Server">
	<button id="btnUpload" runat="server" onclick="DoUploadSlots();return false;" >
		Upload
	</button>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <table>
        <tr>
            <td>
                <p class="NoMargins" align="right">
                    <input type="checkbox" ID="chkGroupItems" CHECKED
                    onclick='ClickGroupItems()'>Group Items
                    <input type="checkbox" ID="chkShowAll" 
                    onclick='ClickShowAll()'>Show All slots
                </p>
                <table class="PanelContainer">
                    <tr>
                        <td>
                            <table class="PanelTable">
                                <tr id="rowEditSlots_Weeks">
                                    <td id="PanelActionFirstWeek" class="PanelAction">
                                        <button id="weekPrev2" class="ActionButton" onclick="SetFilterNextWeek2(-1);return false;">&lt;&lt;</button>
                                    </td>
                                    <td id="PanelActionPrevWeek" class="PanelAction">
                                        <button id="weekPrev" class="ActionButton" onclick="SetFilterNextWeek(-1);return false;">&lt;</button>
                                    </td>
                                    <td class="PanelInactiveWeek" onclick='ActivateWeek(2)'>
                                        <p class="NoMargins">1/3/2010</p>
                                    </td>
                                    <td class="PanelActiveWeek" onclick='ActivateWeek(3)'>
                                        <p class="NoMargins">1/10/2010</p>
                                    </td>
                                    <td class="PanelInactiveWeek" onclick='ActivateWeek(4)'>
                                        <p class="NoMargins">1/17/2010</p>
                                    </td>
                                    <td class="PanelInactiveWeek" onclick='ActivateWeek(5)'>
                                        <p class="NoMargins">1/24/2010</p>
                                    </td>
                                    <td class="PanelInactiveWeek" onclick='ActivateWeek(6)'>
                                        <p class="NoMargins">1/31/2010</p>
                                    </td>
                                    <td id="PanelActionNextWeek" class="PanelAction">
                                        <button id="weekNext" class="ActionButton" onclick="SetFilterNextWeek(1);return false;">&gt;</button>
                                    </td>
                                    <td id="PanelActionPrevWeek" class="PanelAction">
                                        <button id="weekNext2" class="ActionButton" onclick="SetFilterNextWeek2(1);return false;">&gt;&gt;</button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="PanelTable">
                                <tr id="rowEditSlots_Days">
                                    <td class="PanelInactiveDay" onclick='ActivateDay(0)'>
                                        <p class="NoMargins">Sunday</p>
                                    </td>
                                    <td class="PanelActiveDay" onclick='ActivateDay(1)'>
                                        <p class="NoMargins">Monday</p>
                                    </td>
                                    <td class="PanelInactiveDay" onclick='ActivateDay(2)'>
                                        <p class="NoMargins">Tuesday</p>
                                    </td>
                                    <td class="PanelInactiveDay" onclick='ActivateDay(3)'>
                                        <p class="NoMargins">Wednesday</p>
                                    </td>
                                    <td class="PanelInactiveDay" onclick='ActivateDay(4)'>
                                        <p class="NoMargins">Thursday</p>
                                    </td>
                                    <td class="PanelInactiveDay" onclick='ActivateDay(5)'>
                                        <p class="NoMargins">Friday</p>
                                    </td>
                                    <td class="PanelInactiveDay" onclick='ActivateDay(6)'>
                                        <p class="NoMargins">Saturday</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table id="tblEditSlots_panelData" class="PanelData">
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



var _iCellResourceID = 1;
var _iCellSlotStartDate = 2;
var _iCellSlotStartTime = 3;
var _iCellLength = 4;
var _iCellOwner = 5;
var _iCellReserved = 6;
var _iCellRestrict = 7;
var _iCellAction = 8;

var vsCurSortOrder = "";
var viCurSortOrder = 0;
var vmpExpanded = null;

var vfGroupItems = true;

/* S E T  T I T L E */
/*----------------------------------------------------------------------------
	%%Function: SetTitle
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function SetTitle(oCell, iCell, rgVals)
{
    if (iCell == _iCellResourceID)
        {
        oCell.title = rgVals[0].wcrs.sDescription;
        }

    if (iCell == _iCellRestrict)
        {
        oCell.title = DecodeRestrict(rgVals[_iCellRestrict]);
        }
}


/* C R E A T E  R E S E R V E  B L O C K */
/*----------------------------------------------------------------------------
	%%Function: CreateReserveBlock
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateReserveBlock(sSlotID)
{
    return CreateButtonCore("ReserveSlot('" + sSlotID + "')", "Reserve");
}

/* C R E A T E  R E L E A S E  B L O C K */
/*----------------------------------------------------------------------------
	%%Function: CreateReleaseBlock
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateReleaseBlock(sSlotID)
{
    return CreateButtonCore("ReleaseSlot('" + sSlotID + "')", "Release");
}

/* C R E A T E  E D I T  B L O C K */
/*----------------------------------------------------------------------------
	%%Function: CreateEditBlock
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateEditBlock(sSlotID)
{
    return CreateButtonCore("EditSlot('" + sSlotID + "')", "Edit");
}


/* A D D  W C S S  T O  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: AddWcssToTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function AddWcssToTable(wcss, rgVals, fNewHeader, fCollapsed)
{
    var oTable = tblEditSlots_panelData;
    var iRow = oTable.rows.length - 1;
    var oRowTemplate = oTable.rows[iRow];

    if (rgVals == null)
        rgVals = RgvalsFromWcss(wcss);

    oTable.insertRow(iRow);

    var oRowNew = oTable.rows[iRow];
    var cells = oRowTemplate.cells;

    oRowNew.className = "";

    if (fNewHeader)
        {
        // ooh, new heading line
        oRowNew.insertCell(0);
        oRowNew.cells[0].className = cells[0].className;
        if (fCollapsed)
            oRowNew.cells[0].innerHTML = CreateValueBlock("+");
        else
            oRowNew.cells[0].innerHTML = CreateValueBlock("-");

        var iT = 1;
        oRowNew.insertCell(iT);

        if (iT < viCurSortOrder)
            {
            oRowNew.cells[iT - 1].colSpan = viCurSortOrder - iT;
            oRowNew.insertCell(++iT);
            }

        oRowNew.cells[iT].innerHTML = CreateValueBlock(rgVals[viCurSortOrder]);
        SetTitle(oRowNew.cells[iT], viCurSortOrder, rgVals);
        oRowNew.className = "ExpandoHeader";
        oRowNew.onclick = ExpandoRow;
        iRow++;
        oTable.insertRow(iRow);
        oRowNew = oTable.rows[iRow];
        oRowNew.className = "";
        }

    var iCell;

    for (iCell = 0; iCell < cells.length; iCell++)
        {
        oRowNew.insertCell(iCell);

        var cellTemplate = cells[iCell];
        var cellNew = oRowNew.cells[iCell];

        cellNew.className = cellTemplate.className;

        if (iCell <= _iCellRestrict && iCell > 0)
            {
            if (iCell != _iCellRestrict)
                cellNew.innerHTML = CreateValueBlock(rgVals[iCell]);
            if (iCell == _iCellRestrict)
                {
                if (vsCurUserGroup == "Admin")
                    {
                    cellNew.innerHTML = CreateValueBlock(rgVals[iCell]);
                    }
                else
                    {
                    cellNew.style.visibility = "collapse";
                    cellNew.style.display = "none";
                    }
                }

            }

        if (iCell == _iCellAction)
            {
            var sReserve = "";
            var sRelease = "";
            var sEdit = "";

            // figure out what actions we have here
            if (vcCurUserLimitLeft > 0 && rgVals[_iCellOwner] == "")
                sReserve = CreateReserveBlock(rgVals[iCell]);
            if (rgVals[_iCellOwner] == vsCurUserID)
                sRelease = CreateReleaseBlock(rgVals[iCell]);
//            if (vsCurUserGroup == "Admin")
//                sEdit = CreateEditBlock(rgVals[iCell]);

            var sInner = CreateValueBlock(sReserve + sRelease + sEdit);
            cellNew.innerHTML = sInner
            }


        SetTitle(cellNew, iCell, rgVals);
        }

    if (fCollapsed)
        {
        oRowNew.className = "HiddenRow";
        }
//    if (((iRow - 1) % 2) == 0)
//        oRowNew.className = "OddRow";
//    else
//        oRowNew.className = "EvenRow";
}

/* R G V A L S  F R O M  W C S S */
/*----------------------------------------------------------------------------
	%%Function: RgvalsFromWcss
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function RgvalsFromWcss(wcss)
{
    var rgVals = new Array();

    rgVals[0] = wcss;
    rgVals[_iCellResourceID] = wcss.wcrs.sName;
    var dttm = ToLocalDate(wcss.dttmSlotStart);
    rgVals[_iCellSlotStartDate] = DttmToString(dttm, true);
    rgVals[_iCellSlotStartTime] = TimeToString(dttm, true);
    rgVals[_iCellLength] = TimeFromMinutes(wcss.nMinutes);
    rgVals[_iCellOwner] = wcss.sOwner;
    if (wcss.dttmReserved == null)
        rgVals[_iCellReserved] = "";
    else
        {
        var dttm = ToLocalDate(wcss.dttmReserved);
        var dttmCheck = dttm;
        var dttmToday = new Date();

        dttmCheck.setHours(dttmCheck.getHours() - 8);
        dttmToday.setHours(dttmToday.getHours() - 8);

        if (dttmCheck.getDate() == dttmToday.getDate() 
            && dttmCheck.getMonth() == dttmToday.getMonth() 
            && dttmCheck.getYear() == dttmToday.getYear())
            {
            rgVals[_iCellReserved] = "Today";
            }
        else
            {
            rgVals[_iCellReserved] = DttmToString(dttm);
            }
        }

    rgVals[_iCellRestrict] = wcss.wcts.grfLevel;
    rgVals[_iCellAction] = wcss.sSlotID;

    return rgVals;
}

/* F I L L  W C S S  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: FillWcssTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function FillWcssTable(rgwcss)
{
    if (rgwcss == null)
        return;

    var oTable = tblEditSlots_panelData;

    var iwcss;
    var sSortCur = "";
    var rgVals;
    var rgValsNext = null;

    var fNewHeader = false;
    var fCollapsed = false;

    for (iwcss = 0; iwcss < rgwcss.length; iwcss++)
        {
//        if (iwcss < 8)
//            DebugWrite("Processing " + iwcss + ": " + rgwcss[iwcss].wcrs.sResourceID);

        if (rgValsNext == null)
            rgVals = RgvalsFromWcss(rgwcss[iwcss]);
        else
            rgVals = rgValsNext;

        if (iwcss + 1 < rgwcss.length)
            rgValsNext = RgvalsFromWcss(rgwcss[iwcss + 1]);
        else
            rgValsNext = null;

        if (sSortCur != rgVals[viCurSortOrder] && vfGroupItems)
            {
            if (rgValsNext != null && rgValsNext[viCurSortOrder] == rgVals[viCurSortOrder])
                {
                if (vmpExpanded != null && vmpExpanded[rgVals[viCurSortOrder]] == 1)
                    fCollapsed = false;
                else
                    fCollapsed = true;

                fNewHeader = true;
                }
            else
                {
                fCollapsed = false;
                }

            sSortCur = rgVals[viCurSortOrder];
            }
        else
            {
            fNewHeader = false;
            }

//        if (iwcss < 8)
//            DebugWrite("Adding " + iwcss + ": " + rgwcss[iwcss].wcrs.sResourceID);

        AddWcssToTable(rgwcss[iwcss], rgVals, fNewHeader, fCollapsed);
        }
}


/* E X P A N D O  C E L L */
/*----------------------------------------------------------------------------
	%%Function: ExpandoCell
	%%Qualified: ExpandoCell.ExpandoCell
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ExpandoCell()
{
	var oRow = this.parentNode;
	ExpandoCore(oRow);
}

/* E X P A N D O  R O W */
/*----------------------------------------------------------------------------
	%%Function: ExpandoRow
	%%Qualified: ExpandoRow.ExpandoRow
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ExpandoRow()
{
	ExpandoCore(this);
}

/* E X P A N D O  C O R E */
/*----------------------------------------------------------------------------
	%%Function: ExpandoCore
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ExpandoCore(oRow)
{
	var iRow = oRow.rowIndex;
	var fUnhide = false;
	var sHiding = null;

	var oTable = oRow.parentNode;

	iRow++;

	if (oTable.rows[iRow].className.indexOf("HiddenRow") != -1)
		{
		oRow.cells[0].innerText = "-";
		fUnhide = true;
		}
	else
		{
		oRow.cells[0].innerText = "+";
		sUnhide = oTable.rows[iRow].cells[viCurSortOrder].innerText;
		}

	while (iRow < oTable.rows.length - 1)
		{
		var oRowCur = oTable.rows[iRow];

		if (fUnhide)
			{
			// unhide till you gen an unhidden row
			if (oRowCur.className.indexOf("HiddenRow") != -1)
				{
				// this is a hidden row
				oRowCur.className = oRowCur.className.replace("HiddenRow", "");
				}
			else
				{
				break;
				}
			}
		else
			{
			// hide until you get a "different" row or another header
			if (oRowCur.className.indexOf("ExpandoHeader") == -1 && sUnhide == oRowCur.cells[viCurSortOrder].innerText)
				{
				oRowCur.className += " HiddenRow";
				}
			else
				{
				break;
				}   
			}
		iRow++;
		}
}



/* S H O W  H I D E  A D M I N */
/*----------------------------------------------------------------------------
	%%Function: ShowHideAdmin
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ShowHideAdmin()
{
    var oRow = document.getElementById("rowEditSlots_Headings");
    var oDiv = document.getElementById("AdminMastHead");

    if (vsCurUserGroup == "Admin")
        {
        oRow.cells[_iCellRestrict].style.visibility = "visible";
        oRow.cells[_iCellRestrict].style.display = "block";
        oDiv.style.display = "block";
        }
    else
        {
        oRow.cells[_iCellRestrict].style.visibility = "collapse";
        oRow.cells[_iCellRestrict].style.display = "none";
        oDiv.style.display = "none";
        }
}

/* G E T  S O R T  O R D E R */
/*----------------------------------------------------------------------------
	%%Function: GetSortOrder
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function GetSortOrder()
{
	var i = 0;
	var oRow = rowEditSlots_Headings;

	while (i < oRow.cells.length)
		{
		if (oRow.cells[i].className.indexOf("SortOrder") != -1)
			{
			viCurSortOrder = i;
			vsCurSortOrder = oRow.cells[i].innerText;
			return vsCurSortOrder;
			}
		i++;
		}
}

/* U P D A T E  S O R T  O R D E R */
/*----------------------------------------------------------------------------
	%%Function: UpdateSortOrder
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function UpdateSortOrder(iNew)
{
	var i = 0;
	var oRow = rowEditSlots_Headings;

	while (i < oRow.cells.length)
		{
		if (i == iNew)
			{
			if (oRow.cells[i].className.indexOf("SortOrder") == -1)
				oRow.cells[i].className += " SortOrder";
			}
		else
			{
			if (oRow.cells[i].className.indexOf("SortOrder") != -1)
				oRow.cells[i].className = oRow.cells[i].className.replace("SortOrder", "");
			}
		i++;
		}
	DoSlotsQuery();
}

/* C L E A R  D A T A  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: ClearDataTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ClearDataTable()
{
	// first, remember our old settings
	PersistQuerySettings(); // implemented in local page js
    vmpExpanded = new Array();

	var oTable = tblEditSlots_panelData;

	while (oTable.rows.length > 2)
		{
		var oRow = oTable.rows[1];

		if (oRow.cells[0].innerText == "-")
			{
			var s = oRow.cells[oRow.cells.length - 1].innerText;
			vmpExpanded[s] = 1;
			}
		oTable.deleteRow(1);
		}
}

/* U P D A T E  S L O T S */
/*----------------------------------------------------------------------------
	%%Function: UpdateSlots
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function UpdateSlots()
{
	if (vcCurUserLimitLeft == -9999 && vsCurUserID != "")
		{
		// have to refresh the cur user limit first
		UpdateDailyLimitRemain();
		vcCurUserLimitLeft = 0;	// in case they don't succeed, don't want infinite loop
		return;
		}

	DoSlotsQuery();
}


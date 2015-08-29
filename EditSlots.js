

var fQueryCalled = false;

/* P A G E  L O A D  L O C A L */
/*----------------------------------------------------------------------------
	%%Function: pageLoadLocal
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function pageLoadLocal()
{
	if (!fQueryCalled)
		{
		fQueryCalled = true;

        var dttm = new Date();
		
		while (dttm.getDay() != 0)
			{
			dttm.setDate(dttm.getDate() - 1);
			}
		
        ShowWeeksBeginning(dttm);
        document.getElementById("idCurrentUser").innerText = vsCurUser;
        ShowUser();
		}
}

/* C L I C K  G R O U P  I T E M S */
/*----------------------------------------------------------------------------
	%%Function: ClickGroupItems
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ClickGroupItems()
{
	var cb = document.getElementById("chkGroupItems");

    vfGroupItems = cb.checked;
	vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
	UpdateSlots();
}


/* C L I C K  S H O W  A L L */
/*----------------------------------------------------------------------------
	%%Function: ClickShowAll
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ClickShowAll()
{
	var cb = document.getElementById("chkShowAll");

	vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
	UpdateSlots();
}

/* S H O W  W E E K S  B E G I N N I N G */
/*----------------------------------------------------------------------------
	%%Function: ShowWeeksBeginning
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ShowWeeksBeginning(dttmFirstSunday)
{
	var oRow = document.getElementById("rowEditSlots_Weeks");

	var iCellFirst = 2;
	var cCells = 5;
	var iCellMac = iCellFirst + cCells;

	while (iCellFirst < iCellMac)
		{
		oRow.cells[iCellFirst].innerText = DttmToString(dttmFirstSunday, false);

		dttmFirstSunday.setDate(dttmFirstSunday.getDate() + 7);
		iCellFirst++;
		}
}

/* S E T  F I L T E R  N E X T  W E E K */
/*----------------------------------------------------------------------------
	%%Function: SetFilterNextWeek
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function SetFilterNextWeek(iMul)
{
	var oRow = rowEditSlots_Weeks;
	var oCells = oRow.cells;

	var i = 2;
	while (i < oCells.length)
		{
		if (oCells[i].className == "PanelActiveWeek")
			break;
		i++;
		}

	if (i >= oCells.length)
		return null;

	// see if we just need to move to the next week
	if ((iMul == 1 && (i + 1) < oCells.length - 2)
		|| (iMul == -1 && (i - 1) >= 2))
		{
		oCells[i].className = "PanelInactiveWeek";
		oCells[i + iMul].className = "PanelActiveWeek";
		}
	else
		{
		// need to shift
		var sDttmSunday = oRow.cells[2].innerText;
		var dttm = DttmFromString(sDttmSunday);
		dttm.setDate(dttm.getDate() + 7 * iMul);

		ShowWeeksBeginning(dttm);
		}
    UpdateSlots();
}

/* S E T  F I L T E R  N E X T  W E E K  2 */
/*----------------------------------------------------------------------------
	%%Function: SetFilterNextWeek2
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function SetFilterNextWeek2(iMul)
{
	var oRow = rowEditSlots_Weeks;

	var sDttmSunday = oRow.cells[2].innerText;
	var dttm = DttmFromString(sDttmSunday);

	dttm.setDate(dttm.getDate() + 7 * 5 * iMul);
	ShowWeeksBeginning(dttm);
    UpdateSlots();
}

var iTestCur = 0;

/* W C S S  T E S T */
/*----------------------------------------------------------------------------
	%%Function: WcssTest
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function WcssTest()
{
	var wcss = wcssTest[iTestCur];

	iTestCur++;
	if (iTestCur >= rgwcssTest.length)
		iTestCur = 0;

	return wcss;
}

/* G E T  D A T E  F I L T E R */
/*----------------------------------------------------------------------------
	%%Function: GetDateFilter
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function GetDateFilter()
{
	var oRow = rowEditSlots_Weeks;
	var oCells = oRow.cells;

	var i = 2;
	while (i < oCells.length)
		{
		if (oCells[i].className == "PanelActiveWeek")
			break;
		i++;
		}

	if (i >= oCells.length)
		return null;

	var sWeek = oCells[i].innerText;
	var dttmWeek = DttmFromString(sWeek); // Date.parse(sWeek);
	
	i = 0;

	oRow = rowEditSlots_Days;
	oCells = oRow.cells;

	while (i < oCells.length)
		{
		if (oCells[i].className == "PanelActiveDay")
			break;
		i++;
		}

	if (i >= oCells.length)
		return null;

	dttmWeek.setDate(dttmWeek.getDate() + i);
	return dttmWeek;
}


/* D O  U P L O A D  S L O T S */
/*----------------------------------------------------------------------------
	%%Function: DoUploadSlots
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DoUploadSlots()
{
    var win = window.open("UploadSlots.aspx", "_blank", "width=128, height=96,location=no,menubar=no,status=no, toolbar=no,scrollbars=no,resizable=no");
}

/* P E R S I S T  Q U E R Y  S E T T I N G S */
/*----------------------------------------------------------------------------
	%%Function: PersistQuerySettings
	%%Contact: rlittle
 
 	persist any query settings this local page wants to remember (like the
 	date we're on)
----------------------------------------------------------------------------*/
function PersistQuerySettings()
{
	vdttmLastQuery = GetDateFilter();
}

var vdttmLastQuery = null;

/* D O  S L O T S  Q U E R Y */
/*----------------------------------------------------------------------------
	%%Function: DoSlotsQuery
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DoSlotsQuery()
{
	var dttmStart = GetDateFilter();
	var dttmEnd = new Date(dttmStart);
	var fShowAll = document.getElementById("chkShowAll").checked;

	dttmStart.setHours(0);
	dttmEnd.setHours(23);
	dttmEnd.setMinutes(59);
	
    // convert start and end to UTC which is what the database works in
    dttmStart = ToUTCDate(dttmStart);
    dttmEnd = ToUTCDate(dttmEnd);	
	WebCal.WebCal.QuerySlots(dttmStart, dttmEnd, null, null, !fShowAll, true, vsCurUserID, null, GetSortOrder(), fillSlotsTable, OnFailed);
}

/* A F T E R  L O G I N */
/*----------------------------------------------------------------------------
	%%Function: AfterLogin
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function AfterLogin()
{
    vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
    UpdateSlots();
}

/* A C T I V A T E  W E E K */
/*----------------------------------------------------------------------------
	%%Function: ActivateWeek
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ActivateWeek(iWeek)
{
	ActivateCol(iWeek, rowEditSlots_Weeks, "Week");
	UpdateSlots();
}


/* A C T I V A T E  D A Y */
/*----------------------------------------------------------------------------
	%%Function: ActivateDay
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ActivateDay(iDay)
{
	ActivateCol(iDay, rowEditSlots_Days, "Day");
	UpdateSlots();
}

/* A C T I V A T E  C O L */
/*----------------------------------------------------------------------------
	%%Function: ActivateCol
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ActivateCol(iCol, oRow, sSuffix)
{
	// first, find the active day and deactivate it
	var oCells = oRow.cells;

	var i = 0;
	while (i < oCells.length)
		{
		var s = oCells[i].className;
		
		if (s == "PanelActive" + sSuffix
			|| s == "PanelInactive" + sSuffix)
			{
			if (i == iCol)
				{
				oCells[i].className = "PanelActive" + sSuffix;
				}
			else
				{
				oCells[i].className = "PanelInactive" + sSuffix;
				}
			}
		i++;
		}
}



var rgwcssTest = 
[
{
	"wcrs" : 
	{
		"sResourceID" : "H5",
		"sName" : "Hartman Park 5",
		"sDescription" : "Dirt field, 60', dirt mound, enclosed",
	},
	"dttmSlotStart" : new Date(),
	"nMinutes" : 90,
	"sOwner" : "",
	"wcts" : 
	{
		"grfLevel" : 511,
	}
},
{
	"wcrs" : 
	{
		"sResourceID" : "H5",
		"sName" : "Hartman Park 5",
		"sDescription" : "Dirt field, 60', dirt mound, enclosed",
	},
	"dttmSlotStart" : new Date(),
	"nMinutes" : 90,
	"sOwner" : "",
	"wcts" : 
	{
		"grfLevel" : 511,
	}
}
];



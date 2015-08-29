
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
        ShowUser();
		}
}

/* D O  S L O T S  Q U E R Y */
/*----------------------------------------------------------------------------
	%%Function: DoSlotsQuery
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DoSlotsQuery()
{
	WebCal.WebCal.QuerySlots(null, null, vsCurUserID, null, false, false, vsCurUserID, null, GetSortOrder(), fillSlotsTable, OnFailed);
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


/* P E R S I S T  Q U E R Y  S E T T I N G S */
/*----------------------------------------------------------------------------
	%%Function: PersistQuerySettings
	%%Contact: rlittle
 
 	our query is always the same...whoever the logged in user is...
----------------------------------------------------------------------------*/
function PersistQuerySettings()
{
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


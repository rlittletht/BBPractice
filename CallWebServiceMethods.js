
var vpfnFill;

/* P A G E  L O A D */
/*----------------------------------------------------------------------------
	%%Function: pageLoad
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function pageLoad() 
{
    pageLoadLocal();
}

/* O N  S U C C E E D E D  U P D A T E  D A I L Y  L I M I T */
/*----------------------------------------------------------------------------
	%%Function: OnSucceededUpdateDailyLimit
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function OnSucceededUpdateDailyLimit(result, userContext, methodName)
{
    if (result != null)
        vcCurUserLimitLeft = result;

    UpdateSlots();
}

/* U P D A T E  D A I L Y  L I M I T  R E M A I N */
/*----------------------------------------------------------------------------
	%%Function: UpdateDailyLimitRemain
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function UpdateDailyLimitRemain()
{
    WebCal.WebCal.GetDailyLimitRemain(vsCurUserID, OnSucceededUpdateDailyLimit, OnFailed);
}

/* O N  S U C C E S S  R E F R E S H  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: OnSuccessRefreshTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function OnSuccessRefreshTable(result, userContext, methodName)
{
    vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
    if (result == null)
        {
        UpdateSlots();
        }
    else
        {
        alert("Failed to reserve slot: " + result);
        UpdateSlots();
        }
}

/* R E S E R V E  S L O T */
/*----------------------------------------------------------------------------
	%%Function: ReserveSlot
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ReserveSlot(sSlotID)
{
    vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
    WebCal.WebCal.ReserveSlot(sSlotID, vsCurUserID, null, OnSuccessRefreshTable, OnFailed);
}

/* R E L E A S E  S L O T */
/*----------------------------------------------------------------------------
	%%Function: ReleaseSlot
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ReleaseSlot(sSlotID)
{
    vcCurUserLimitLeft = -9999;    // we want to update this, so mark it as needing update
    WebCal.WebCal.ReleaseSlot(sSlotID, vsCurUserID, null, OnSuccessRefreshTable, OnFailed);
}


/* F I L L  S L O T S  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: fillSlotsTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function fillSlotsTable(result, userContext, methodName)
{
    ClearDataTable();
    FillWcssTable(result);
}

/* F I L L  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: fillTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function fillTable(result, userContext, methodName) 
{
    var rgwcrs = result;

    var i;
  
    for (i = 0; i < rgwcrs.length; i++)
        {
        AddWcrsToTable(rgwcrs[i]);
        }
}

/* A D D  O N E  R E S O U R C E */
/*----------------------------------------------------------------------------
	%%Function: addOneResource
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function addOneResource(result, userContext, methodName) 
{
    AddWcrsToTable(result);
}

/* D E L E T E  O N E  R E S O U R C E */
/*----------------------------------------------------------------------------
	%%Function: deleteOneResource
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function deleteOneResource(result, userContext, methodName) 
{
    var oRowTemplate = TemplateRow;
    var oTable = TableData;

    var oRow;
    var iRow = 1; // start after the title row

    var fRebanding = false;

    while (iRow < oTable.rows.length - 2) // don't include the template row or the add row
        {
        oRow = oTable.rows[iRow];
        if (!fRebanding)
            {
            if (oRow.cells[1].innerText == result)
                {
                oTable.deleteRow(iRow);
                oRow = oTable.rows[iRow];
                fRebanding = true;
                }
            }
        if (fRebanding)
            {
            if (oRow.className == "EvenRow")
                oRow.className = "OddRow";
            else if (oRow.className == "OddRow")
                oRow.className = "EvenRow";
            }

        iRow++;
        }
}


/* G E T  R E S O U R C E S */
/*----------------------------------------------------------------------------
	%%Function: GetResources
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function GetResources(pfnFill) 
{
    WebCal.WebCal.GetResources(fillTable, OnFailed);
}

function AddResources()
{
    var sID = theForm.elements["AddID"].value;
    var sName = theForm.elements["AddName"].value;
    var sDescription = theForm.elements["AddDescription"].value;

    theForm.elements["AddID"].value = "";
    theForm.elements["AddName"].value = "";
    theForm.elements["AddDescription"].value = "";

    WebCal.WebCal.AddResource(sID, sName, sDescription, addOneResource, OnFailed);
}

function DeleteResource(sID)
{
    WebCal.WebCal.DeleteResource(sID, deleteOneResource, OnFailed);
}

// Callback function invoked on successful
// completion of the page method.


/* O N  S U C C E E D E D */
/*----------------------------------------------------------------------------
	%%Function: OnSucceeded
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function OnSucceeded(result, userContext, methodName) 
{
    var sObjDump = AddObjectHtml(result, "result", 0, 0);
}

// Callback function invoked on failure 
// of the page method.
function OnFailed(error, userContext, methodName) 
{
    DebugWrite("Failed: " + AddObjectHtml(error, "error", 0, 0), 0);
}


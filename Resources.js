
var fQueryCalled = false;

function pageLoadLocal()
{
    if (!fQueryCalled)
        {
        fQueryCalled = true;

        var oDiv = document.getElementById("AdminMastHead");
        oDiv.style.display = "none";

        ShowUser();
        }
}

/* S H O W  H I D E  A D M I N */
/*----------------------------------------------------------------------------
	%%Function: ShowHideAdmin
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function ShowHideAdmin()
{
    var oTable = document.getElementById("TableData");

    if (vsCurUserGroup == "Admin")
        {
        oTable.style.display = "block";
        GetResources(fillTable);
        }
    else
        {
        oTable.style.display = "none";
        }
}

/* A F T E R  L O G I N */
/*----------------------------------------------------------------------------
	%%Function: AfterLogin
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function AfterLogin()
{
}
	
/* A D D  W C R S  T O  T A B L E */
/*----------------------------------------------------------------------------
	%%Function: AddWcrsToTable
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function AddWcrsToTable(wcrs)
{
    // get the table
    var oRowTemplate = TemplateRow;
    var oTable = TableData;

    oTable.insertRow(oTable.rows.length - 2);

    var iRow = oTable.rows.length - 3;

    var oRowNew = oTable.rows[iRow];
    oRowNew.className = oRowTemplate.className;

    var cells = oRowTemplate.cells;

    var iCell;

    for (iCell = 0; iCell < cells.length; iCell++)
        {
        var cell = cells[iCell];

        oRowNew.insertCell(iCell);
        var cellNew = oRowNew.cells[iCell];
        
        cellNew.className = cell.className;
        if (iCell == 1)
            cellNew.innerHTML = CreateValueBlock(wcrs.sResourceID);
        else if (iCell == 2)
            cellNew.innerHTML = CreateValueBlock(wcrs.sName);
        else if (iCell == 3)
            cellNew.innerHTML = CreateValueBlock(wcrs.sDescription);
        else if (iCell == 4)
            cellNew.innerHTML = CreateActionContent(wcrs.sResourceID);

        }

    if (((iRow - 1) % 2) == 0)
        {
        oRowNew.className = "OddRow";
        }
    else
        {
        oRowNew.className = "EvenRow";
        }

}


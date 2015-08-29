
function ShowUser()
{
    DebugWrite("UserID: " + vsCurUserID + ", User: " + vsCurUser + ", UserGroup: " + vsCurUserGroup + ", Limit: " + vcCurUserLimitLeft);

    var btnLogin = document.getElementById(vsDoLoginBtnID);

    if (vsCurUser == "" || vsCurUser == "undefined" || vsCurUser == "Not logged in")
        {
        btnLogin.innerText = "Login";
        vsCurUser = "Not logged in";
        }
    else
        {
        btnLogin.innerText = "Logout";
        AfterLogin();
        }

    var oLogin = document.getElementById("idCurrentUser");
    oLogin.innerText = vsCurUser;

    ShowHideAdmin();
}


/* N A V I G A T E */
/*----------------------------------------------------------------------------
	%%Function: Navigate
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function Navigate(s)
{
    window.location.href = s;
    return false;
}

/* D T T M  F R O M  S T R I N G */
/*----------------------------------------------------------------------------
	%%Function: DttmFromString
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DttmFromString(s)
{
	var dttm = new Date(s);

	return dttm;
}

/* C R E A T E  V A L U E  B L O C K */
/*----------------------------------------------------------------------------
	%%Function: CreateValueBlock
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateValueBlock(val)
{
    return "<p class='NoMargins'>" + val + "</p>";
}

/* C R E A T E  A C T I O N  C O N T E N T */
/*----------------------------------------------------------------------------
	%%Function: CreateActionContent
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateActionContent(sID)
{
    return CreateValueBlock("<button onclick='DeleteResource(\"" + sID + "\");return false;'>X</button>");
}

var _nUtcOffsetMinutes = 0;

function ToLocalDate(dttmUtc)
{
    if (_nUtcOffsetMinutes == 0)
        {
        // figure out what the local time offset is for UTC
        var dttmCur = new Date();
        _nUtcOffsetMinutes = -dttmCur.getTimezoneOffset();
        }

    dttmUtc.setMinutes(dttmUtc.getMinutes() + _nUtcOffsetMinutes);
    return dttmUtc;
}

function ToUTCDate(dttmLocal)
{
    if (_nUtcOffsetMinutes == 0)
        {
        // figure out what the local time offset is for UTC
        var dttmCur = new Date();
        _nUtcOffsetMinutes = -dttmCur.getTimezoneOffset();
        }

    dttmLocal.setMinutes(dttmLocal.getMinutes() - _nUtcOffsetMinutes);
    return dttmLocal;
}

/* T O  2  D I G I T S */
/*----------------------------------------------------------------------------
	%%Function: To2Digits
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function To2Digits(n)
{
    if (n < 10)
        return "0"+n;
    else
        return ""+n;
}


/* D T T M  T O  S T R I N G */
/*----------------------------------------------------------------------------
	%%Function: DttmToString
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DttmToString(dttm, fShort)
{
    var s = "" + (dttm.getMonth() + 1)
            + "/" + To2Digits(dttm.getDate());

    if (!fShort)
        s += "/" + dttm.getYear();

    return s;
}

/* T I M E  T O  S T R I N G */
/*----------------------------------------------------------------------------
	%%Function: TimeToString
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function TimeToString(dttm, fPM)
{
    if (fPM)
        {
        var sPM = "am";

        var nHours = dttm.getHours();
        if (nHours >= 12)
            {
            sPM = "pm";
            nHours -= 12;
            if (nHours == 0)
                nHours = 12;

            }
        return nHours + ":" + To2Digits(dttm.getMinutes()) + " " + sPM;
        }
    else
        return dttm.getHours() + ":" + To2Digits(dttm.getMinutes());
}


var dnsRestrict = new Array( "Baseball", "Softball", "TBall", "Sluggers", "AAA", "Coast", "Majors", "Juniors", "Seniors" );

/* D E C O D E  R E S T R I C T */
/*----------------------------------------------------------------------------
	%%Function: DecodeRestrict
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function DecodeRestrict(nRestrict)
{
    var s = "";
    var fFirst = true;

    // first, how many bits are we talking about?
    var cBitsOn = 0;
    var n = nRestrict;

    while (n != 0)
        {
        cBitsOn++;
        n &= (n -1);
        }

    if (cBitsOn == dnsRestrict.length)
        {
        return "No restrictions";
        }

    var i = 0;
    n = 1;

    var iMac = dnsRestrict.length;
    var fFlip = false;
    if (cBitsOn >= 5)
        {
        // shorter to report what's NOT allowed
        fFlip = true;
        s = "NOT ";
        }

    while (i < iMac)
        {
        if (!fFlip && (nRestrict & n) == n)
            {
            if (!fFirst)
                {
                s += ", ";
                }
            else
                {
                fFirst = false;
                }

            s += dnsRestrict[i];
            }
        if (fFlip && (nRestrict & n) == 0)
            {
            if (!fFirst)
                {
                s += ", ";
                }
            else
                {
                fFirst = false;
                }
            s += dnsRestrict[i];
            }
        i++;
        n = n << 1;
        }

    return s;
}

/* C R E A T E  B U T T O N  C O R E */
/*----------------------------------------------------------------------------
	%%Function: CreateButtonCore
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function CreateButtonCore(sPfn, sText)
{
    var s = "<button onclick=\"" + sPfn + ";return false;\" class=\"DataActionButton\">" + sText + "</button>";

    return s;
}

/* T I M E  F R O M  M I N U T E S */
/*----------------------------------------------------------------------------
	%%Function: TimeFromMinutes
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function TimeFromMinutes(nLen)
{
    if (nLen < 60)
        {
        return "0:" + To2Digits(nLen);
        }
    else
        {
        var nHours = Math.floor(nLen / 60);
        var nMin = nLen - nHours * 60;

        return nHours + ":" + To2Digits(nMin);
        }
}

/* B U I L D  I N D E N T  B L O C K  O P E N */
/*----------------------------------------------------------------------------
	%%Function: BuildIndentBlockOpen
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function BuildIndentBlockOpen(nIndent)
{
    return "<div style='margin-left: 0.25in;'>";
}

/* B U I L D  I N D E N T  B L O C K  C L O S E */
/*----------------------------------------------------------------------------
	%%Function: BuildIndentBlockClose
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function BuildIndentBlockClose()
{
    return "</div>";
}


/* A D D  O B J E C T  H T M L */
/*----------------------------------------------------------------------------
	%%Function: AddObjectHtml
	%%Contact: rlittle

----------------------------------------------------------------------------*/
function AddObjectHtml(obj, sName, nIndent, nDepth)
{
    if (nDepth > 10)
        return BuildIndentBlockOpen(nIndent) + "Depth exceeded" + BuildIndentBlockClose();
    
    var sOut = BuildIndentBlockOpen(nIndent) + sName + ": ";
    if (typeof obj == "object")
        {
        var child = null;

        for (var item in obj)
            {
            try
                {      
                child = obj[item];
                }
            catch (e)
                {
                child = "Can't evaluate child '"+item+"'("+e+")";
                }
            if (typeof obj == "object")
                sOut += AddObjectHtml(child, item, nIndent + 1, nDepth + 1);
            else
                sOut += item + ": " + child + "<br/>";

            }
        sOut += BuildIndentBlockClose();
        return sOut;
        }
    sOut += obj + BuildIndentBlockClose();
    return sOut;
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;


namespace WebCal
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// W E B  C A L 
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    public partial class WebCal : System.Web.Services.WebService
    {
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  O W N E R  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[Serializable]
		public class WebCalOwnerSerial
		{
			public string sOwnerID;
			public string sName;
			public string sGroup;
			public WebCalRestrictSerial wcts;
			public string sPassword;
            public int cDailyLimit;
            public int cDailyRemainGuess;
		}
	}
}

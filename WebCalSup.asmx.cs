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
    [WebService(Namespace = "http://www.thetasoft.com/schemas/webcal/2010")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
	// W E B  C A L 
	// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    public partial class WebCal : System.Web.Services.WebService
    {
		public const int _nUTCDayStart = 16;
		static string sResourceConnString = "<<secret>>"; // Data Source=<foo.bar.com>;Database=<somedb>;User ID=<someuserid>;Password=<secret>;";
//        static string sResourceConnString = "Data Source=ix;Database=db0902;Trusted_Connection=Yes";

        public static string Sqlify(string s)
        {
            return s.Replace("'", "''");
        }

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  O W N E R  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//		[Serializable]
//		public class WebCalOwnerSerial
//		{
//			public string sOwnerID;
//			public string sName;
//			public string sGroup;
//			public WebCalRestrictSerial wcts;
//			public string sPassword;
//            public int cDailyLimit;
//            public int cDailyRemainGuess;
//		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  R E S O U R C E  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[Serializable]
		public class WebCalResourceSerial
		{
			public string sResourceID;
			public string sName;
			public string sDescription;

			public WebCalResourceSerial()
			{
				sResourceID = sDescription = sName = "";
			}
		}


		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  R E S T R I C T  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[Serializable]
		public class WebCalRestrictSerial
		{
			public int grfLevel;
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  W I N D O W  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[Serializable]
		public class WebCalWindowSerial
		{
			public WebCalRestrictSerial wcts;
			public DateTime dttmStart;
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  S L O T  S E R I A L 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		[Serializable]
		public class WebCalSlotSerial
		{
			public string sSlotID;
			public DateTime dttmSlotStart;
			public int nMinutes;
			public string sOwner;
			public WebCalRestrictSerial wcts;
			public List<WebCalWindowSerial> plwcws;
			public WebCalResourceSerial wcrs;
			public DateTime? dttmReserved;

			public WebCalSlotSerial()
			{
			}
		}

		/* O P E N  C O N N E C T I O N */
		/*----------------------------------------------------------------------------
			%%Function: OpenConnection
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		static SqlConnection OpenConnection(out string sFailed)
		{
			SqlConnection sqlc = new SqlConnection(sResourceConnString);
            sFailed = "";
			try	
				{
				sqlc.Open(); 
				}
			catch (Exception exc) 
				{ 
				sFailed = exc.ToString(); 
				return null;
				}

			return sqlc;
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  O W N E R 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		public class WebCalOwner
		{
			WebCalOwnerSerial m_wcosCache = null;
			
			string m_sOwnerID;
			string m_sName;
			string m_sGroup;
			WebCalRestrict m_wct;
			string m_sPassword;
            int m_cDailyRemainGuess;
            int m_cDailyLimit;

			public string Name { get { return m_sName; } }
			public string OwnerID { get { return m_sOwnerID; } }

			public static string SqlQueryString = "select OwnerID, Name, Restrictions, Password, [Group], DailyLimit from WebCalOwners";

			/* L O G I N */
			/*----------------------------------------------------------------------------
				%%Function: Login
				%%Qualified: WebCal.WebCal:WebCalOwner.Login
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public static WebCalOwner Login(string sOwner, string sPassword)
			{
				WebCalOwner wco = null;

				string sQuery = SqlQueryString;

				if (sPassword == null)
					sQuery += String.Format(" WHERE OwnerID = '{0}'", sOwner);
				else
					sQuery += String.Format(" WHERE OwnerID = '{0}' AND Password = '{1}'", sOwner, sPassword);

				string sFailed;
				SqlConnection sqlc = OpenConnection(out sFailed);

				if (sqlc == null)
					return null;

				SqlCommand sqlcmd = sqlc.CreateCommand();
				sqlcmd.CommandText = sQuery;

				SqlDataReader sqlr = sqlcmd.ExecuteReader();

				if (sqlr.Read())
					{
					wco = new WebCalOwner(sqlr);
					}
				sqlr.Close();
				sqlcmd = null;
				sqlc.Close();

                if (wco != null)
                    {
                    if (wco.m_cDailyLimit > 1000)
                        {
                        wco.m_cDailyRemainGuess = wco.m_cDailyLimit;
                        }
                    else
                        {
                        wco.m_cDailyRemainGuess = wco.m_cDailyLimit - WebCal.WebCalSlot.GetSlotCount(WebCal._nUTCDayStart, wco.m_sOwnerID);
                        }
                    }

				return wco;
			}

			/* W E B  C A L  O W N E R */
			/*----------------------------------------------------------------------------
				%%Function: WebCalOwner
				%%Qualified: WebCal.WebCal:WebCalOwner.WebCalOwner
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public WebCalOwner(WebCalOwnerSerial wcos)
			{
				m_wcosCache = wcos;
				m_sOwnerID = wcos.sOwnerID;
				m_sName = wcos.sName;
				m_sPassword = wcos.sPassword;
				m_wct = new WebCalRestrict(wcos.wcts);
                m_sGroup = wcos.sGroup;
                m_cDailyLimit = wcos.cDailyLimit;
                m_cDailyRemainGuess = wcos.cDailyRemainGuess;
			}

			/* W E B  C A L  O W N E R */
			/*----------------------------------------------------------------------------
				%%Function: WebCalOwner
				%%Qualified: WebCal.WebCal:WebCalOwner.WebCalOwner
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public WebCalOwner(string sOwner, string sName, string sPassword, WebCalRestrict wct, string sGroup, int cDailyLimit, int cDailyRemainGuess)
			{
				m_sOwnerID = sOwner;
				m_sName = sName;
				m_wct = wct;
				m_sPassword = sPassword;
                m_cDailyLimit = cDailyLimit;
                m_cDailyRemainGuess = cDailyRemainGuess;
                m_sGroup = sGroup;
			}

			/* W E B  C A L  O W N E R */
			/*----------------------------------------------------------------------------
				%%Function: WebCalOwner
				%%Qualified: WebCal.WebCal:WebCalOwner.WebCalOwner
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public WebCalOwner(SqlDataReader sqlr)
			{
				m_sOwnerID = sqlr.GetString(0);
				m_sName = sqlr.GetString(1);
				m_sPassword = sqlr.GetString(3);
				m_wct = new WebCalRestrict(sqlr.GetInt32(2));
                m_sGroup = sqlr.GetString(4);
                m_cDailyLimit = sqlr.GetInt32(5);
                m_cDailyRemainGuess = 0;
			}

			/* W C O S */
			/*----------------------------------------------------------------------------
				%%Function: Wcos
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public WebCalOwnerSerial Wcos
			{
				get
					{
					if (m_wcosCache == null)
						{
						m_wcosCache = new WebCalOwnerSerial();
						m_wcosCache.sOwnerID = m_sOwnerID;
						m_wcosCache.sName = m_sName;
						m_wcosCache.sPassword = m_sPassword;
                        m_wcosCache.cDailyLimit = m_cDailyLimit;
                        m_wcosCache.cDailyRemainGuess = m_cDailyRemainGuess;
						m_wcosCache.wcts = m_wct.Wcts;
                        m_wcosCache.sGroup = m_sGroup;
						}
					return m_wcosCache;
					}
			}
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		// W E B  C A L  R E S O U R C E 
		// - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
		public class WebCalResource
        {
            WebCalResourceSerial m_wcrs;

            public static string SqlQueryString = "select ResourceID, Name, Description from WebCalResources";

            public bool Valid { get { return true/* m_wcrs.sName != null && m_wcrs.sName != ""*/; } }

			/* W E B  C A L  R E S O U R C E */
			/*----------------------------------------------------------------------------
				%%Function: WebCalResource
				%%Qualified: WebCal.WebCal:WebCalResource.WebCalResource
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public WebCalResource(SqlDataReader sqlr)
			{
                m_wcrs = new WebCalResourceSerial();
                m_wcrs.sResourceID = sqlr.GetString(0);
                m_wcrs.sName = sqlr.GetString(1);
                m_wcrs.sDescription = sqlr.GetString(2);
			}

            /* W E B  C A L  R E S O U R C E */
            /*----------------------------------------------------------------------------
            	%%Function: WebCalResource
            	%%Qualified: WebCal.WebCal:WebCalResource.WebCalResource
            	%%Contact: rlittle

            ----------------------------------------------------------------------------*/
            public WebCalResource(string sID, string sName, string sDescription)
            {
                m_wcrs = new WebCalResourceSerial();
                m_wcrs.sDescription = sDescription;
                m_wcrs.sResourceID = sID;
                m_wcrs.sName = sName;
            }

            /* W E B  C A L  R E S O U R C E */
            /*----------------------------------------------------------------------------
            	%%Function: WebCalResource
            	%%Qualified: WebCal.WebCal:WebCalResource.WebCalResource
            	%%Contact: rlittle

            ----------------------------------------------------------------------------*/
            public WebCalResource(string sID)
            {
                m_wcrs = new WebCalResourceSerial();
                m_wcrs.sResourceID = sID;
                m_wcrs.sDescription = m_wcrs.sName = null;
            }

            public WebCalResourceSerial Wcrs { get { return m_wcrs; } }

            public string ResourceID { get { return m_wcrs.sResourceID; } }

            public string BuildInsertQueryString()
            {
                return String.Format("INSERT INTO WebCalResources (ResourceID, Name, Description) VALUES ('{0}', '{1}', '{2}')",
                                     Sqlify(m_wcrs.sResourceID), Sqlify(m_wcrs.sName), Sqlify(m_wcrs.sDescription));
            }

            public string BuildDeleteQueryString()
            {
                return String.Format("DELETE FROM WebCalResources WHERE ResourceID = '{0}'", m_wcrs.sResourceID);
            }

        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // W E B  C A L  R E S T R I C T 
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public class WebCalRestrict
        {
            WebCalRestrictSerial m_wctsCache;

            public enum Level
            {
                TBall = 0,
                Sluggers = 1,
                AAA = 2,
                Coast = 3,
                Majors = 4,
                Juniors = 5,
                Seniors = 6,
                Last = 6,
            };

            bool []m_rgfLevel = new bool[7];
            bool m_fSoftball;
            bool m_fBaseball;

            public WebCalRestrict()
            {
                for(int i = 0; i <= (int)Level.Last; i++)
                    m_rgfLevel[i] = false;
                m_fSoftball = false;
                m_fBaseball = false;
                m_wctsCache = null;
            }

			public WebCalRestrict(WebCalRestrictSerial wcts)
			{
				m_wctsCache = wcts;
				FromGrf(wcts.grfLevel);
			}

            public bool Baseball { get { return m_fBaseball; } set { m_fBaseball = value; m_wctsCache = null; } }
            public bool Softball { get { return m_fSoftball; } set { m_fSoftball = value; m_wctsCache = null; } }

            public bool GetLevel(Level lvl)
            {
                return m_rgfLevel[(int)lvl];
            }

            public void SetLevel(Level lvl, bool f)
            {
                m_rgfLevel[(int)lvl] = f;
                m_wctsCache = null;
            }

            public bool FUpdateFromString(string s)
            {
                if (String.Compare(s, "Baseball", true) == 0)
                    m_fBaseball = true;
                else if (String.Compare(s, "Softball", true) == 0)
                    m_fSoftball = true;
                else if (String.Compare(s, "TBall", true) == 0)
                    SetLevel(Level.TBall, true);
                else if (String.Compare(s, "Sluggers", true) == 0)
                    SetLevel(Level.Sluggers, true);
                else if (String.Compare(s, "AAA", true) == 0)
                    SetLevel(Level.AAA, true);
                else if (String.Compare(s, "Coast", true) == 0)
                    SetLevel(Level.Coast, true);
                else if (String.Compare(s, "Majors", true) == 0)
                    SetLevel(Level.Majors, true);
                else if (String.Compare(s, "Juniors", true) == 0)
                    SetLevel(Level.Juniors, true);
                else if (String.Compare(s, "Seniors", true) == 0)
                    SetLevel(Level.Seniors, true);
                else
                    return false;

                return true;
            }

			void FromGrf(int grf)
			{
				if ((grf & 0x01) == 1)
					m_fBaseball = true;
				grf >>= 1;

				if ((grf & 0x01) == 1)
					m_fSoftball = true;

				grf >>= 1;

				int i = 0;
				while (i <= (int)Level.Last)
					{
					if ((grf & 0x01) == 1)
						m_rgfLevel[i] = true;
					grf >>= 1;
					i++;
					}
			}

            // bits are Seniors, Juniors, ..., TBall, Softball, Baseball
            public WebCalRestrict(int grf)
            {
				FromGrf(grf);
                m_wctsCache = new WebCalRestrictSerial();
                m_wctsCache.grfLevel = grf;
            }

            public int GrfLevel()
            {
                int grf = 0;

                int i = (int)Level.Last;
                while (i >= 0)
                    {
                    if (m_rgfLevel[i])
                        grf |= 0x1;

                    grf <<= 1;
                    i--;
                    }

                if (m_fSoftball)
                    grf |= 0x1;

                grf <<= 1;

                if (m_fBaseball)
                    grf |= 0x1;

                return grf;
            }

            public WebCalRestrictSerial Wcts
            {
                get
                    {
                    if (m_wctsCache == null)
                        {
                        m_wctsCache = new WebCalRestrictSerial();
                        m_wctsCache.grfLevel = GrfLevel();
                        }
                    return m_wctsCache;
                    }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // W E B  C A L  W I N D O W 
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public class WebCalWindow
        {
            WebCalWindowSerial m_wcwsCache;
            WebCalRestrict m_wct;
            DateTime m_dttmStart;

            public WebCalWindow(WebCalRestrict wct, DateTime dttmStart)
            {
                m_wcwsCache = null;
                m_wct = wct;
                m_dttmStart = dttmStart;
            }

            public WebCalWindowSerial Wcws
            {
                get
                    {
                    if (m_wcwsCache == null)
                        {
                        m_wcwsCache = new WebCalWindowSerial();
                        m_wcwsCache.dttmStart = m_dttmStart;
                        m_wcwsCache.wcts = m_wct.Wcts;
                        }
                    return m_wcwsCache;
                    }
            }

			public string BuildInsertString(string sSlotID)
			{
				return String.Format("INSERT INTO WebCalTimeWindows (SlotID, Restriction, WindowStart) VALUES ('{0}', {1}, '{2}')", sSlotID, m_wct.GrfLevel(), m_dttmStart.ToString("M/d/yy"));
			}

            public string ToHTML()
            {
                string sHtml = "<tr>";

                sHtml += String.Format("<td>{0}</td>", m_wct.GrfLevel());
                sHtml += String.Format("<td>{0}</td>", m_dttmStart);

                return sHtml;
            }

        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // W E B  C A L  S L O T 
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public class WebCalSlot
        {
            WebCalSlotSerial m_wcssCache;
            WebCalResource m_wcr;
            string m_sSlotID;
            DateTime m_dttmSlotStart;
			DateTime? m_dttmReserved;
            int m_nMinutes;
            string m_sOwner;
            WebCalRestrict m_wct;
            List<WebCalWindow> m_plwcw = new List<WebCalWindow>();

			static string SqlQueryString = "select WCS.SlotID, WCS.ResourceID, WCR.Name, WCS.SlotStart, WCS.SlotLength, WCS.Restrictions, WCS.Owner, WCR.Description, WCS.SlotTaken FROM WebCalSlots WCS INNER JOIN WebCalResources WCR ON WCR.ResourceID = WCS.ResourceID ";

            /* G E T  S L O T  C O U N T */
            /*----------------------------------------------------------------------------
            	%%Function: GetSlotCount
            	%%Qualified: WebCal.WebCal:WebCalSlot.GetSlotCount
            	%%Contact: rlittle

            ----------------------------------------------------------------------------*/
            public static int GetSlotCount(int nUTCHourDayBegin, string sOwner)
            {
                string s;

                s = String.Format("sp_getslotcount '{0}', {1}", sOwner, nUTCHourDayBegin);
                string sFailed;
				SqlConnection sqlc = OpenConnection(out sFailed);

				SqlCommand sqlcmd = sqlc.CreateCommand();
				sqlcmd.CommandText = s;
                object o = sqlcmd.ExecuteScalar();
                int i = (int)o;

				sqlcmd = null;
				sqlc.Close();

				return i;
            }

			/* B U I L D  Q U E R Y  S T R I N G */
			/*----------------------------------------------------------------------------
				%%Function: BuildQueryString
				%%Qualified: WebCal.WebCal:WebCalSlot.BuildQueryString
				%%Contact: rlittle

			----------------------------------------------------------------------------*/
			public static string BuildQueryString(DateTime ?dttmFirst, 
				DateTime ?dttmLast, 
				string sOwner,
				string sResourceID,
				bool fOnlyOpen,
				bool fHideRestricted,
				string sSortOrder,
				WebCalOwner wco)
			{
				string sQuery = WebCalSlot.SqlQueryString;
				string sQueryAdd = "";

				if (dttmFirst != null)
					sQueryAdd += String.Format(" AND WCS.SlotStart >= '{0}'", dttmFirst.Value.ToString("M/d/yyyy HH:mm"));

				if (dttmLast != null)
					sQueryAdd += String.Format(" AND WCS.SlotStart <= '{0}'", dttmLast.Value.ToString("M/d/yyyy HH:mm"));

				if (sOwner != null)
					sQueryAdd += String.Format(" AND WCS.Owner = '{0}'", sOwner);

				if (sResourceID != null)
					sQueryAdd += String.Format(" AND WCS.ResourceID = '{0}'", sResourceID);

				if (fOnlyOpen)
					sQueryAdd += String.Format(" AND (WCS.Owner = '' Or WCS.Owner = '{0}')", wco.OwnerID);

				if (fHideRestricted)
					sQueryAdd += String.Format(" AND (WCS.Restrictions & {0}) = {0}", wco.Wcos.wcts.grfLevel);

				sQuery += " WHERE " + sQueryAdd.Substring(4); // skip the leading AND

				if (sSortOrder != null && sSortOrder != "")
					{
					if (String.Compare(sSortOrder, "Location", true/*ignoreCase*/) == 0)
						sSortOrder = "WCS.ResourceID, WCS.SlotStart";
					else if (String.Compare(sSortOrder, "Date", true/*ignoreCase*/) == 0
                             || String.Compare(sSortOrder, "Start", true/*ignoreCase*/) == 0)
						sSortOrder = "WCS.SlotStart";
					else if (String.Compare(sSortOrder, "Owner", true/*ignoreCase*/) == 0)
						sSortOrder = "WCS.Owner, WCS.ResourceID, WCS.SlotStart";
                    else if (String.Compare(sSortOrder, "Length", true/*ignoreCase*/) == 0)
                        sSortOrder = "WCS.SlotLength, WCS.ResourceID, WCS.SlotStart";
                    else if (String.Compare(sSortOrder, "Restrictions", true/*ignoreCase*/) == 0)
                        sSortOrder = "WCS.Restrictions, WCS.ResourceID, WCS.SlotStart";
					else if (String.Compare(sSortOrder, "Reserved", true/*ignoreCase*/) == 0)
						sSortOrder = "WCS.SlotTaken, WCS.ResourceID, WCS.SlotStart";

					sQuery += " ORDER BY " + sSortOrder;
					}
				else
					{
					sQuery += " ORDER BY WCS.SlotStart";
					}

                return sQuery;
			}


            public WebCalSlot(string sSlotID, WebCalResource wcr, DateTime dttmSlotStart, int nMinutes, string sOwner, WebCalRestrict wct, List<WebCalWindow> plwcw, DateTime? dttmSlotTaken)
            {
                m_sSlotID = sSlotID;
                m_wcr = wcr;
                m_dttmSlotStart = dttmSlotStart;
                m_nMinutes = nMinutes;
                m_sOwner = sOwner;
                m_wct = wct;
                m_plwcw = plwcw;
				m_dttmReserved = dttmSlotTaken;
            }

			public string SlotID { get { return m_sSlotID; } }

			public List<WebCalWindow> Windows { get { return m_plwcw; } }

			public string BuildCleanupWindows()
			{
				return String.Format("DELETE FROM WebCalTimeWindows WHERE SlotID = '{0}'", m_sSlotID);
			}

			public string BuildInsertQueryString()
			{
				if (m_sSlotID == null || m_sSlotID == "")
					{
					m_sSlotID = System.Guid.NewGuid().ToString();
					}

				return String.Format(
					"INSERT INTO WebCalSlots (SlotID, ResourceID, SlotStart, SlotLength, Restrictions, Owner) "+
					"VALUES ('{0}', '{1}', '{2}', {3}, {4}, '{5}')",
					m_sSlotID, Sqlify(m_wcr.ResourceID), 
					m_dttmSlotStart.ToString("M/d/yyyy HH:mm"), m_nMinutes,
					m_wct.GrfLevel(), m_sOwner);
			}

            public static WebCalSlot WcsFromCsv(string []rgCsv, string []mpicol, int nUTCOffset, bool fDST, out string sFailed)
            {
                string sID = null, sOwner = null;
                int nMinutes = 0;
                DateTime dttmSlotStart = new DateTime();
                bool fDateSet = false;
                WebCalRestrict wct = new WebCalRestrict();
                List<WebCalWindow> plwcw = null;
                WebCalResource wcr = null;
                int iMac = rgCsv.Length;

                for (int i = 0; i < rgCsv.Length; i++)
                    {
                    // we're done if we get to the last column
                    if (mpicol[i] == null)
                        break;
                    if (String.Compare(mpicol[i], "ID", true/*ignoreCase*/) == 0)
						{
						sID = rgCsv[i];
						if (sID != "")
							{
							try
								{
								System.Guid guid = new System.Guid(sID);
								}
							catch (Exception exc)
								{
								sFailed = String.Format("Failed to parse {0} as GUID: {1}", sID, exc.ToString());
								return null;
								}
							}
						}
                    else if (String.Compare(mpicol[i], "ResourceID", true/*ignoreCase*/) == 0)
                        {
                        wcr = new WebCalResource(rgCsv[i]);
                        if (!wcr.Valid)
                            {
                            sFailed = String.Format("Unknown resource ID {0} ", rgCsv[i]);
                            return null;
                            }
                        }
                    else if (String.Compare(mpicol[i], "ID", true/*ignoreCase*/) == 0)
                        sID = rgCsv[i];
                    else if (String.Compare(mpicol[i], "SlotDate", true/*ignoreCase*/) == 0)
                        {
                        try
                            {
                            dttmSlotStart = DateTime.Parse(rgCsv[i]);
                            fDateSet = true;
                            }
                        catch (Exception e)
                            {
                            sFailed = String.Format("Failed to convert string '{0}' to dttm", rgCsv[i]);
                            return null;
                            }
                        }
					else if (String.Compare(mpicol[i], "SlotTime", true/*ignoreCase*/) == 0)
						{
						try
							{
                            DateTime dttmHour = DateTime.Parse(rgCsv[i]);
                            TimeSpan ts = dttmHour.Subtract(DateTime.Parse("0:00"));
                            dttmSlotStart = dttmSlotStart.Add(ts);

							fDateSet = true;
							}
						catch (Exception e)
							{
							sFailed = String.Format("Failed to convert string '{0}' to dttm", rgCsv[i]);
							return null;
							}
						}
                    else if (String.Compare(mpicol[i], "SlotLength", true/*ignoreCase*/) == 0)
                        {
                        try
                            {
                            DateTime dttm = DateTime.Parse(rgCsv[i]);
                            TimeSpan ts = dttm.Subtract(DateTime.Parse("0:00"));

                            nMinutes = (int)ts.TotalMinutes;
                            }
                        catch (Exception e)
                            {
                            sFailed = String.Format("Failed to convert string '{0}' to SlotLength", rgCsv[i]);
                            return null;
                            }
                        }
                    else if (String.Compare(mpicol[i], "Owner", true/*ignoreCase*/) == 0)
                        sOwner = rgCsv[i];
                    else if (String.Compare(mpicol[i], 0, "Slot-", 0, 5, true/*ignoreCase*/) == 0)
                        {
                        // this is a slot
                        if (rgCsv[i].Length > 0 && rgCsv[i] != "0" && rgCsv[i].Substring(0, 1).ToUpper() != "N")
                            {
                            if (!wct.FUpdateFromString(mpicol[i].Substring(5)))
                                {
                                sFailed = String.Format("Failed to converted expected slot {0} with value {1}", mpicol[i], rgCsv[i]);
                                return null;
                                }
                            }
                        }
                    else if ((String.Compare(mpicol[i], 0, "WindowBB-", 0, 9, true/*ignoreCase*/) == 0
                         || String.Compare(mpicol[i], 0, "WindowSB-", 0, 9, true/*ignoreCase*/) == 0))
                        {
                        if (rgCsv[i].Length > 0)
                            {
                            WebCalRestrict wctWindow = new WebCalRestrict();

                            bool fBaseball = String.Compare(mpicol[i], 0, "WindowBB-", 0, 9, true/*ignoreCase*/) == 0;

                            wctWindow.Baseball = fBaseball;
                            wctWindow.Softball = !fBaseball;

                            // this is a baseball signup window
                            if (!wctWindow.FUpdateFromString(mpicol[i].Substring(9)))
                                {
                                sFailed = String.Format("Failed to converted expected Window {0} with value {1}", mpicol[i], rgCsv[i]);
                                return null;
                                }

                            DateTime dttm;
                            try
                                {
                                dttm = DateTime.Parse(rgCsv[i]);
                                }
                            catch (Exception e)
                                {
                                sFailed = String.Format("Failed to convert string '{0}' to dttm for Window {1}", rgCsv[i], mpicol[i]);
                                return null;
                                }

                            WebCalWindow wcw = new WebCalWindow(wctWindow, dttm);
							if (plwcw == null)
								plwcw = new List<WebCalWindow>();

                            plwcw.Add(wcw);
                            }
                        }
                    else
                        {
                        // oh crap, we didn't match!
                        sFailed = String.Format("Unknown column header name {0}!", mpicol[i]);
                        return null;
                        }
                    }

                // did we get enough data?

                if (!fDateSet)
                    {
                    sFailed = "No date specified for slot!";
                    return null;
                    }
                if (nMinutes <= 0 || nMinutes > 240)
                    {
                    sFailed = String.Format("Slot length ({0}) is not between 1 and 240", nMinutes);
                    return null;
                    }

                if (wcr == null)
                    {
                    sFailed = "No resource specified for slot!";
                    return null;
                    }

                sFailed = null;
                WebCalSlot wcs = new WebCalSlot(sID, wcr, dttmSlotStart, nMinutes, sOwner, wct, plwcw, null);
				wcs.ConvertToUTC(nUTCOffset, fDST);
				return wcs;
            }

            public static WebCalSlot WcsFromSqlr(SqlDataReader sqlr)
            {
				WebCalResource wcr = new WebCalResource(sqlr.GetString(1), sqlr.GetString(2), sqlr.GetString(7));
				WebCalRestrict wct = new WebCalRestrict(sqlr.GetInt32(5));

                WebCalSlot wcs = new WebCalSlot(sqlr.GetGuid(0).ToString(), wcr, sqlr.GetDateTime(3), sqlr.GetInt32(4), sqlr.GetString(6), wct, null, sqlr.IsDBNull(8) ? (DateTime?)null : (DateTime?)sqlr.GetDateTime(8));
                return wcs;
            }

			DateTime DttmUTCFromDttm(DateTime dttm, int nUTCOffset, bool fDST)
			{
				int[] rgdstBeginMonth = { 3, 3, 3, 3, 3, 3};
				int[] rgdstBeginDay = { 14, 13, 11, 10, 9, 8};
				int[] rgdstEndMonth = { 11, 11, 11, 11, 11, 11};
				int[] rgdstEndDay = { 7, 6, 4, 3, 2, 1 };

				if (fDST)
					{
					int i = 2010 - dttm.Year;

					if (i < 0 || i > rgdstBeginMonth.Length)
						throw new Exception(String.Format("DST NOT IMPLEMENTED FOR YEAR {0}", m_dttmSlotStart.Year));

					DateTime dttmBegin = new DateTime(2010 + i, rgdstBeginMonth[i], rgdstBeginDay[i], 3, 0, 0);
					DateTime dttmEnd =  new DateTime(2010 + i, rgdstEndMonth[i], rgdstEndDay[i], 2, 0, 0);

					if (dttm.CompareTo(dttmBegin) > 0
						&& dttm.CompareTo(dttmEnd) < 0)
						{
						// we fall within DST, so this is a DST adjusted time. this means that we are 1 less hour away
						// from UTC
						nUTCOffset += 60;
						}
					}
				return dttm.AddMinutes(-nUTCOffset);
			}


			/* C O N V E R T  T O  U  T  C */
			/*----------------------------------------------------------------------------
				%%Function: ConvertToUTC
				%%Qualified: WebCal.WebCal.ConvertToUTC
				%%Contact: rlittle

			 	NOTE:  fDST doesn't mean the time *IS* daylight savings time, it just
			 	means that it MIGHT be.  We still have to look at the dates for DST to
			 	figure out whether it really is or not.
			----------------------------------------------------------------------------*/
			public void ConvertToUTC(int nUTCOffset, bool fDST)
			{
                m_dttmSlotStart = DttmUTCFromDttm(m_dttmSlotStart, nUTCOffset, fDST);
				
				// adjust slot taken time too if its there
				if (m_dttmReserved != null)
					m_dttmReserved = DttmUTCFromDttm(m_dttmReserved.Value, nUTCOffset, fDST);
			}

            public WebCalSlot()
            {
            }

            public WebCalSlotSerial Wcss
            {
                get
                    {   
                    if (m_wcssCache == null)
                        {
                        m_wcssCache = new WebCalSlotSerial();
                        m_wcssCache.sSlotID = m_sSlotID;
                        m_wcssCache.dttmSlotStart = m_dttmSlotStart;
                        m_wcssCache.nMinutes = m_nMinutes;
                        m_wcssCache.sOwner = m_sOwner;
                        m_wcssCache.wcts = m_wct.Wcts;
                        m_wcssCache.wcrs = m_wcr.Wcrs;
						m_wcssCache.dttmReserved = m_dttmReserved;

                        if (m_plwcw != null)
                            {
                            m_wcssCache.plwcws = new List<WebCalWindowSerial>();
                            foreach(WebCalWindow wcw in m_plwcw)
                                m_wcssCache.plwcws.Add(wcw.Wcws);
                            }
                        else
                            {
                            m_wcssCache.plwcws = null;
                            }
                        }
                    return m_wcssCache;
                    }
            }

            public string ToHTML()
            {
                string sHtml = "<tr>";

                sHtml += String.Format("<td>{0}</td>", m_sSlotID);
                sHtml += String.Format("<td>{0}</td>", m_dttmSlotStart.ToString("M/d/yyyy"));
                sHtml += String.Format("<td>{0}</td>", m_nMinutes);
                sHtml += String.Format("<td>{0}</td>", m_sOwner);
                sHtml += String.Format("<td>{0}</td>", m_wct.GrfLevel());
                sHtml += "<td>";
                if (m_plwcw != null)
                    {
                    sHtml += "<table>";
                    foreach(WebCalWindow wcw in m_plwcw)
                        sHtml += wcw.ToHTML();
                    }
                else
                    {
                    sHtml += "&nbsp;</td>";
                    }
                sHtml += "</tr>";
                return sHtml;
            }
        }


        class Csv
        {
            public static string[] LineToArray(string line)
            {
                String pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                Regex r = new Regex(pattern);

                string[] rgs = r.Split(line);

                for (int i = 0; i < rgs.Length; i++)
                    {
                    if (rgs[i].Length > 0 && rgs[i][0] == '"')
                        rgs[i] = rgs[i].Substring(1, rgs[i].Length - 2);
                    }
                return rgs;
           }
        }

        /* F  U P L O A D  S L O T S */
        /*----------------------------------------------------------------------------
        	%%Function: FUploadSlots
        	%%Contact: rlittle
		 
		 	NOTE:  All dates and times are assumed to be in local time, as defined
		 	by nUTCOffset and fDST.  We will insert into the database as UTC.
        ----------------------------------------------------------------------------*/
        public bool FUploadSlots(string sFile, int nUTCOffset, bool fDST, out string sFailed, out List<WebCalSlot> plwcsSucceeded, out List<WebCalSlot> plwcsFailed)
        {
            // build up slots to add and delete
            TextReader tr = new StreamReader(sFile);
            string sLine;

            plwcsSucceeded = new List<WebCalSlot>();
            plwcsFailed = new List<WebCalSlot>();

            bool fHeadingRead = false;
            string []mpicol = new string[64]; // at most 64 columns
            Dictionary<string, int> mpcolName = new Dictionary<string, int>();
            int iLine = 0;

            while ((sLine = tr.ReadLine()) != null)
                {
                iLine++;
                string []rg = Csv.LineToArray(sLine);

                if (rg.Length < 2)
                    continue;

                if (!fHeadingRead)
                    {
                    // create the mapping of columns
                    for (int i = 0; i < rg.Length; i++)
                        {
                        // if we hit an empty column, we're done!
                        if (rg[i] == "")
                            break;
                        mpicol[i] = rg[i];
                        if (mpcolName.ContainsKey(rg[i]))
                            {
                            sFailed = String.Format("fatal error in file {0}, line {1}: {2}", sFile, iLine, "heading line contained duplication column headings: "+ rg[i]);
                            return false;
                            }
                        mpcolName.Add(rg[i], i);
                        }
                    fHeadingRead = true;
                    continue;
                    }

                WebCalSlot wcs = WebCalSlot.WcsFromCsv(rg, mpicol, nUTCOffset, true, out sFailed);

                if (wcs == null)
                    {
                    sFailed = String.Format("fatal error in file {0}, line {1}: {2}", sFile, iLine, sFailed);
                    return false;
                    }

				plwcsSucceeded.Add(wcs);
                }

			// at this point, we have a bunch in plwcsSucceeded.  now we need to update or
			// create them

			SqlConnection sqlc = OpenConnection(out sFailed);

			if (sqlc == null)
				{
				sFailed = "All uploads failed: "+sFailed; 
				plwcsSucceeded = null; 
				return false; 
				}

			SqlTransaction sqlt = sqlc.BeginTransaction();
			SqlCommand sqlcmd = sqlc.CreateCommand();

			sqlcmd.Transaction = sqlt;

			try
				{
				for (int i = 0; i < plwcsSucceeded.Count; i++)
					{
					WebCalSlot wcs = plwcsSucceeded[i];

					string sCmd;
					int cRows;

					// actually insert the slot.
					sCmd= wcs.BuildInsertQueryString();

					sqlcmd.CommandText = sCmd;

					cRows = sqlcmd.ExecuteNonQuery();
					if (cRows != 1)
						throw new Exception(String.Format("insert query for wcs <{0}> failed", sCmd));

					// first, delete any old timewindows laying around
					sCmd = wcs.BuildCleanupWindows();
					sqlcmd.CommandText = sCmd;

					sqlcmd.ExecuteNonQuery();	// answer here is unimportant

					List<WebCalWindow> plwcw = wcs.Windows;

					if (plwcw != null)
						{
						foreach (WebCalWindow wcw in plwcw)
							{
							sCmd = wcw.BuildInsertString(wcs.SlotID);
							sqlcmd.CommandText = sCmd;

							cRows = sqlcmd.ExecuteNonQuery();
							if (cRows != 1)
								throw new Exception(String.Format("insert query for wcw <{0}> failed", sCmd));
							}
						}
				    }
				sFailed = "";
			    }
			catch (Exception exc)
				{
				sqlt.Rollback();
				sqlt = null;
				sFailed = exc.ToString();
				}

			// if this is null, then we rollad back
			if (sqlt != null)
				sqlt.Commit();

			sqlcmd = null;
			sqlc.Close();

            return (sFailed == "");
        }

		[WebMethod]
		/* L O G I N */
		/*----------------------------------------------------------------------------
			%%Function: Login
			%%Qualified: WebCal.WebCal.Login
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public WebCalOwnerSerial Login(string sID, string sPassword)
		{
			WebCalOwner wco = WebCalOwner.Login(sID, sPassword);

            if (wco == null)
                return null;

			return wco.Wcos;
		}

		[WebMethod]
		/* R E L E A S E  S L O T */
		/*----------------------------------------------------------------------------
			%%Function: ReleaseSlot
			%%Qualified: WebCal.ReleaseSlot
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public string ReleaseSlot(string sSlotID, string sOwnerID, string sPassword)
		{
			string sFailed;
			WebCalOwnerSerial wcos = Login(sOwnerID, null);

			if (wcos == null)
				return String.Format("Login for {0} failed", sOwnerID);


			string sCmd = String.Format("UPDATE WebCalSlots SET Owner='', SlotTaken=NULL WHERE SlotID='{0}' AND IsNull(Owner, '')='{1}'",
										sSlotID, sOwnerID);


			SqlConnection sqlc = OpenConnection(out sFailed);

			if (sqlc == null)
				return sFailed;

			sFailed = null;

			SqlCommand sqlcmd = sqlc.CreateCommand();

			sqlcmd.CommandText = sCmd;

			if (sqlcmd.ExecuteNonQuery() != 1)
				{
				// failed to update; try to figure out why
				sqlcmd.CommandText = String.Format("SELECT IsNull(Owner, '') FROM WebCalSlots WHERE SlotID='{0}'", sSlotID);
				string sResult = (string) sqlcmd.ExecuteScalar();

				if (sResult == "")
					{
					sqlcmd.CommandText = String.Format("SELECT SlotID From WebCalSlots where SlotID='{0}'", sSlotID);
					sResult = (string)sqlcmd.ExecuteScalar();

					if (sResult != sSlotID)
						sFailed = String.Format("Slot {0} no longer exists!", sSlotID);
					else
						sFailed = String.Format("Cannot reserve slot {0}: unknown failure", sSlotID);
					}
				else
					{
					sFailed = String.Format("Slot {0} is owned by {1}", sSlotID, sResult);
					}
				}
			sqlcmd = null;
			sqlc.Close();

			return sFailed;
		}

		[WebMethod]
		/* R E S E R V E  S L O T */
		/*----------------------------------------------------------------------------
			%%Function: ReserveSlot
			%%Qualified: WebCal.ReserveSlot
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public string ReserveSlot(string sSlotID, string sOwnerID, string sPassword)
		{
			string sFailed;
			WebCalOwnerSerial wcos = Login(sOwnerID, null);

			if (wcos == null)
				return String.Format("Login for {0} failed", sOwnerID);

			if (wcos.cDailyRemainGuess == 0)
				return String.Format("Account '{0}' cannot reserve any more slots today", sOwnerID);

			string sCmd = String.Format("UPDATE WebCalSlots SET Owner='{0}', SlotTaken=GetUTCDate() WHERE SlotID='{1}' AND IsNull(Owner, '')='' AND (Restrictions & {2}) = {2}",
										sOwnerID, sSlotID, wcos.wcts.grfLevel);


			SqlConnection sqlc = OpenConnection(out sFailed);

			if (sqlc == null)
				return sFailed;

			sFailed = null;

			SqlCommand sqlcmd = sqlc.CreateCommand();

			sqlcmd.CommandText = sCmd;

			if (sqlcmd.ExecuteNonQuery() != 1)
				{
				sqlcmd.CommandText = String.Format("SELECT Restrictions FROM WebCalSlots WHERE SlotID='{0}'", sSlotID);
				object o = sqlcmd.ExecuteScalar();

				if (o != null && (((int)o) & wcos.wcts.grfLevel) != wcos.wcts.grfLevel)
					{
					sFailed = String.Format("Restriction failed for {0}: ({1} & {2}) != {2}", sOwnerID, (int)o, wcos.wcts.grfLevel);
					}
				else
					{
					// failed to update; try to figure out why
					sqlcmd.CommandText = String.Format("SELECT IsNull(Owner, '') FROM WebCalSlots WHERE SlotID='{0}'", sSlotID);
					string sResult = (string) sqlcmd.ExecuteScalar();
	
					if (sResult == "")
						{
						sqlcmd.CommandText = String.Format("SELECT SlotID From WebCalSlots where SlotID='{0}'", sSlotID);
						sResult = (string)sqlcmd.ExecuteScalar();
	
						if (sResult != sSlotID)
							sFailed = String.Format("Slot {0} no longer exists!", sSlotID);
						else
							sFailed = String.Format("Cannot reserve slot {0}: unknown failure", sSlotID);
						}
					else
						{
						sFailed = String.Format("Slot {0} is already taken by {1}", sSlotID, sResult);
						}
					}
				}
			sqlcmd = null;
			sqlc.Close();

			return sFailed;
		}

		[WebMethod]
		/* Q U E R Y  S L O T S */
		/*----------------------------------------------------------------------------
			%%Function: QuerySlots
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public List<WebCalSlotSerial> QuerySlots(
			DateTime ?dttmFirst, 
			DateTime ?dttmLast, 
			string sOwner,
			string sResourceID,
			bool fOnlyOpen,
			bool fHideRestricted,
			string sOwnerID,
			string sPassword,
            string sSortOrder)
		{
			WebCalOwnerSerial wcos = Login(sOwnerID, sPassword);

			if (wcos == null)
				return null;

			WebCalOwner wco = new WebCalOwner(wcos);

			string sQuery = WebCalSlot.BuildQueryString(dttmFirst, dttmLast, sOwner, sResourceID, fOnlyOpen, fHideRestricted, sSortOrder, wco);
            string sFailed;
			SqlConnection sqlc = OpenConnection(out sFailed);

			if (sqlc == null)
				return null;

			SqlCommand sqlcmd = sqlc.CreateCommand();

			sqlcmd.CommandText = sQuery;
			SqlDataReader sqlr = sqlcmd.ExecuteReader();

			List<WebCalSlotSerial> plwcss = new List<WebCalSlotSerial>();

			while (sqlr.Read())
				{
				WebCalSlot wcs = WebCalSlot.WcsFromSqlr(sqlr);

				if (wcs != null)
                    {
                    WebCalSlotSerial wcss = wcs.Wcss;
					plwcss.Add(wcss);
                    }
				}

			sqlr.Close();
			sqlcmd = null;
			sqlc.Close();

			return plwcss;
		}


        [WebMethod]
        /* G E T  R E S O U R C E S */
        /*----------------------------------------------------------------------------
        	%%Function: GetResources
        	%%Qualified: WebCal.GetResources
        	%%Contact: rlittle

        ----------------------------------------------------------------------------*/
        public List<WebCalResourceSerial> GetResources()
		{
			List<WebCalResourceSerial> plwcrs = new List<WebCalResourceSerial>();

			try
				{
				string sFailed;

                SqlConnection sqlc = OpenConnection(out sFailed);

                SqlCommand sqlcmd = sqlc.CreateCommand();
				string sCmd = WebCalResource.SqlQueryString;
				sqlcmd.CommandText = sCmd;
                SqlDataReader oReader = sqlcmd.ExecuteReader(); 

				while (oReader.Read())
					{
					WebCalResource wcr = new WebCalResource(oReader);

					plwcrs.Add(wcr.Wcrs);
					}

				oReader.Close();
				sqlcmd = null;
				sqlc.Close();
				}
            catch (Exception exc) { };

			return plwcrs;
		}

		[WebMethod]
		/* G E T  D A I L Y  L I M I T  R E M A I N */
		/*----------------------------------------------------------------------------
			%%Function: GetDailyLimitRemain
			%%Qualified: WebCal.GetDailyLimitRemain
			%%Contact: rlittle

		----------------------------------------------------------------------------*/
		public int GetDailyLimitRemain(string sOwner)
		{
			WebCalOwnerSerial wcos = Login(sOwner, null);

			return wcos.cDailyRemainGuess;
		}

        [WebMethod]
        /* A D D  R E S O U R C E */
        /*----------------------------------------------------------------------------
        	%%Function: AddResource
        	%%Qualified: WebCal.AddResource
        	%%Contact: rlittle

        ----------------------------------------------------------------------------*/
        public WebCalResourceSerial AddResource(string sID, string sName, string sDescription)
        {
            WebCalResource wcr = new WebCalResource(sID, sName, sDescription);

            try
                {
				string sFailed;
                SqlConnection sqlc = OpenConnection(out sFailed);

                SqlCommand sqlcmd = sqlc.CreateCommand();
                string sCmd = wcr.BuildInsertQueryString();

                sqlcmd.CommandText = sCmd;
                int i = sqlcmd.ExecuteNonQuery(); 

                sqlcmd = null;
                sqlc.Close();

                if (i != 1)
                    return null;

                return wcr.Wcrs;
                }
            catch (Exception exc) 
                { 
                return null; 
                }

            return null;
        }

        [WebMethod]
        /* D E L E T E  R E S O U R C E */
        /*----------------------------------------------------------------------------
        	%%Function: DeleteResource
        	%%Qualified: WebCal.DeleteResource
        	%%Contact: rlittle

        ----------------------------------------------------------------------------*/
        public string DeleteResource(string sID)
        {
            WebCalResource wcr = new WebCalResource(sID, null, null);

            try
                {
				string sFailed;
                SqlConnection sqlc = OpenConnection(out sFailed);

//                m_page.Trace.Write("Connect=" + oConn.ConnectionString);

                SqlCommand sqlcmd = sqlc.CreateCommand();
                string sCmd = wcr.BuildDeleteQueryString();

                sqlcmd.CommandText = sCmd;
                int i = sqlcmd.ExecuteNonQuery(); 

                sqlcmd = null;
                sqlc.Close();

                if (i != 1)
                    return null;

                return sID;
                }
            catch (Exception exc) 
                { 
                return null; 
                }

            return null;
        }

    }
}

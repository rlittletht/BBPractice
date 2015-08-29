using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebCal
{
    public partial class WebCal1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LoginCommand(object sender, EventArgs e)
        {
            WebCal.WebCalOwner wco = WebCal.WebCalOwner.Login(txtUserID.Text, txtPassword.Text);

            if (wco != null)
                {
                WebCal.WebCalOwnerSerial wcos = wco.Wcos;

                Session["CurrentUserName"] = wcos.sName;
                Session["CurrentUserID"] = wcos.sOwnerID;
                Session["CurrentUserGroup"] = wcos.sGroup;
                Session["CurrentUserDailyLimitLeft"] = wcos.cDailyRemainGuess;
                }
            else
                {
                Session["CurrentUserName"] = "Not logged in";
                Session["CurrentUserID"] = null;
                Session["CurrentUserGroup"] = null;
                Session["CurrentUserDailyLimitLeft"] = 0;
                }

        }

    }
}

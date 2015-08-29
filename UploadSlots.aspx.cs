using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebCal
{
    public partial class UploadSlots : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        override protected void OnInit(EventArgs e)
        {
            // 
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            // 
            InitializeComponent();
            base.OnInit(e);
        }

		private void InitializeComponent()
		{    
			this.Submit1.ServerClick += new System.EventHandler(this.Submit1_ServerClick);
			this.Load += new System.EventHandler(this.Page_Load);

		}
        private void Submit1_ServerClick(object sender, System.EventArgs e)
        {
            if ((File1.PostedFile != null) && (File1.PostedFile.ContentLength > 0))
				{
                System.Guid guid = System.Guid.NewGuid();

                string sAsPosted = System.IO.Path.GetFileName(File1.PostedFile.FileName);
                string sUpload = Server.MapPath("\\Data") + "\\" + guid.ToString();
                try
					{
                    File1.PostedFile.SaveAs(sUpload);
					WebCal wc = new WebCal();

					List<WebCal.WebCalSlot> plwcsFailed;
                    List<WebCal.WebCalSlot> plwcsSucceeded;
					string sFailed;

					// for now, force GMT - 8 with DST
					if (wc.FUploadSlots(sUpload, -8 * 60, true, out sFailed, out plwcsSucceeded, out plwcsFailed))
						{
						Response.Write("All slots uploaded.");
						}
					else
						{
						Response.Write(sFailed + "<br>");
						}

					if (plwcsSucceeded != null && plwcsSucceeded.Count > 0)
						{
						Response.Write("<br><br>Failed items follow:<br>");
						Response.Write("<table>");
						foreach (WebCal.WebCalSlot wcs in plwcsSucceeded)
							{
							Response.Write(wcs.ToHTML());
							}
						Response.Write("</table>");
						}

					if (plwcsFailed != null && plwcsFailed.Count > 0)
						{
						Response.Write("<br><br>Failed items follow:<br>");
						Response.Write("<table>");
						foreach (WebCal.WebCalSlot wcs in plwcsFailed)
							{
							Response.Write(wcs.ToHTML());
							}
						Response.Write("</table>");
						}

					}
                catch (Exception ex)
					{
                    Response.Write("Error: " + ex.Message);
                    //Note: Exception.Message returns a detailed message that describes the current exception. 
                    //For security reasons, we do not recommend that you return Exception.Message to end users in 
                    //production environments. It would be better to return a generic error message. 
                    }
				}
            else
            {
                Response.Write("Please select a file to upload.");
            }
        }


    }
}

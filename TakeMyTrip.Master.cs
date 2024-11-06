using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class TakeMyTrip : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["account_name"] != null && userDropdownName != null)
                {
                    userDropdownName.Text = Session["account_name"].ToString();
                }

             
            }

        }
        protected void LoginStatus2_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            // Sign the user out and clear session
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();

            // Optionally, clear the authentication cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(authCookie);

        }
    }
}
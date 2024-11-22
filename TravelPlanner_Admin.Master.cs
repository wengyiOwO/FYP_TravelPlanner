using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class TravelPlanner_Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["account_name"] != null)
                {
                    
                        userDropdownName.Text = Session["account_name"].ToString();
                    
                }
              
                string profileId;
                if (!string.IsNullOrEmpty(Request.QueryString["u"]))
                {
                    profileId = Request.QueryString["u"];
                }
                else
                {
                    profileId = Convert.ToString(Session["account_id"]);
                }
                LoadProfile(profileId);
            }
        }

        protected void LoginStatus3_LoggingOut(object sender, LoginCancelEventArgs e)
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

        private void LoadProfile(string accountId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT account_name FROM Account WHERE account_id = @AccountId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Set account name
                        Session["account_name"] = reader["account_name"].ToString();

                        // Load profile image, use unknown.jpg if image not found
                        string imagePath = Server.MapPath("~/Uploads/Profile/") + accountId + ".jpg";
                        if (System.IO.File.Exists(imagePath))
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/" + accountId + ".jpg";
                        }
                        else
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
                        }
                    }
                    con.Close();
                }
            }
        }
    }
}
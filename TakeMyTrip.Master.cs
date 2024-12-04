using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class TakeMyTrip : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var adminDashboardLink = FindControl("adminDashboardLink") as HtmlGenericControl;

                if (adminDashboardLink == null)
                {
                    return;
                }
               
                else
                {
                    adminDashboardLink.Visible = false;
                }
            }
            if (Session["account_id"] != null)
            {
                string role = Session["account_role"] as string;

                // Show or hide admin dashboard link based on role
                adminDashboardLink.Visible = (!string.IsNullOrEmpty(role) && role == "Admin");

                string accountId = Session["account_id"] as string;
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT account_name, profile_image FROM Account WHERE account_id = @AccountId";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@AccountId", accountId);
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            userDropdownName.Text = reader["account_name"].ToString();

                            string profileImage = reader["profile_image"].ToString();
                            imgProfile.ImageUrl = !string.IsNullOrEmpty(profileImage)
                                ? "~/Uploads/Profile/" + profileImage
                                : "~/Uploads/Profile/unknown.jpg";
                        }
                        con.Close();
                    }
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
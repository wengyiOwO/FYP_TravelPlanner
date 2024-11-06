using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if the session exists
                //if (Session["Account_ID"] != null)
                //{
                    //string accountId = Session["Account_ID"].ToString();
                    string accountId = "AC0521";
                    LoadProfile(accountId);
                    LoadPosts(accountId);
                //}
                //else
                //{
                    // Redirect to login if session is missing
                    //Response.Redirect("Login.aspx");
                //}
            }
        }

        // Method to load profile data
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
                        lblAccountName.Text = reader["account_name"].ToString();

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

        // Method to load posts of the user
        private void LoadPosts(string accountId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT post_id, post_title FROM Posts WHERE account_id = @AccountId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind the data to the Repeater
                    PostsRepeater.DataSource = dt;
                    PostsRepeater.DataBind();

                    con.Close();
                }
            }
        }
    }
}
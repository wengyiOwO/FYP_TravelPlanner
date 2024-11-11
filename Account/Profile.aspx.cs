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
                string profileId = Request.QueryString["u"] != null ? Request.QueryString["u"] : "AC0521"; // Replace with Session["account_id"] if session is available
                string currentUserId = "AC0521"; // Replace with Session["account_id"] if session is available

                LoadProfile(profileId);
                LoadPosts(profileId);
                ConfigureFriendButton(profileId, currentUserId);
            }
        }

        // Method to configure the friend button based on the friendship status
        private void ConfigureFriendButton(string profileId, string currentUserId)
        {
            if (profileId == currentUserId)
            {
                btnEdit.Visible = true;
                friendButtonContainer.Visible = false;
            }
            else
            {
                btnEdit.Visible = false;
                friendButtonContainer.Visible = true;

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT account1_id, account2_id, friend_status
                             FROM Friends 
                             WHERE (account1_id = @CurrentUser AND account2_id = @ProfileUser) 
                                OR (account1_id = @ProfileUser AND account2_id = @CurrentUser)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CurrentUser", currentUserId);
                        cmd.Parameters.AddWithValue("@ProfileUser", profileId);
                        con.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string account1Id = reader["account1_id"].ToString();
                            string account2Id = reader["account2_id"].ToString();
                            string friendStatus = reader["friend_status"].ToString();

                            if (friendStatus == "Request" && account1Id == currentUserId)
                            {
                                ConfigureButton(btnSent, "Friend Request Sent", true, true);
                            }
                            else if (friendStatus == "Request" && account2Id == currentUserId)
                            {
                                ConfigureButton(btnAccept, "Accept", false,true);
                                btnReject.Visible = true;
                            }
                            else if (friendStatus == "Accepted")
                            {
                                ConfigureButton(btnUnfriend, "Unfriend", false, true);
                            }
                        }
                        else
                        {
                            ConfigureButton(btnAdd, "Add Friend", false, true);
                        }
                    }
                }
            }
        }

        // Helper method to configure the button
        private void ConfigureButton(Button button, string text, bool disable, bool visible)
        {
            button.Text = text;
            button.Enabled = !disable;
            button.Visible = visible;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/EditProfile.aspx");
        }


        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string account2_id = Request.QueryString["u"];
            string account1_id = "AC0521"; // Replace with Session["account_id"] if session is available

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "INSERT INTO Friends (account1_id, account2_id, friend_date, friend_status) VALUES (@account1_id, @account2_id, @friend_date, 'Request')";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account1_id", account1_id);
                cmd.Parameters.AddWithValue("@account2_id", account2_id);
                cmd.Parameters.AddWithValue("@friend_date", DateTime.Now);
                cmd.ExecuteNonQuery();
            }
            ConfigureButton(btnAdd, "Add", false, false);
            ConfigureButton(btnSent, "Friend Request Sent", true, true);
        }

        // Event handler for Accept Friend button
        protected void btnAccept_Click(object sender, EventArgs e)
        {
            string friendId = Request.QueryString["u"];
            string accountId = "AC0521"; // Replace with Session["account_id"] if session is available

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "UPDATE Friends SET friend_status = 'Accepted' WHERE account1_id = @friendId AND account2_id = @account_id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@friendId", friendId);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.ExecuteNonQuery();
            }
            ConfigureButton(btnAccept, "Accept", false, false);
            ConfigureButton(btnReject, "Reject", false, false);
            ConfigureButton(btnUnfriend, "Unfriend", false, true);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            // Assume the current user's account ID is stored in the session
            string accountId = "AC0521";
            //string accountId = Convert.ToString(Session["account_id"]);
            // The profile user's ID from the query string
            string friendId = Request.QueryString["u"];

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(friendId))
            {
                // Ensure both account IDs are available
                return;
            }

            // Connection string to your database
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                // SQL query to delete the friend request
                string query = @"
            DELETE FROM Friends 
            WHERE 
                (account1_id = @FriendId AND account2_id = @AccountId AND friend_status = 'Request')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters to the SQL command
                    cmd.Parameters.AddWithValue("@FriendId", friendId);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    // Execute the command
                    cmd.ExecuteNonQuery();
                }
            }
            ConfigureButton(btnAccept, "Accept", false, false);
            ConfigureButton(btnReject, "Reject", false, false);
            ConfigureButton(btnAdd, "Add", false, true);
        }

        // Event handler for Unfriend button
        protected void btnUnfriend_Click(object sender, EventArgs e)
        { 
            string friendId = Request.QueryString["u"];
            string accountId = "AC0521";
            //string accountId = Convert.ToString(Session["account_id"]);

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "DELETE FROM Friends WHERE (account1_id = @account_id AND account2_id = @friendId) OR (account1_id = @friendId AND account2_id = @account_id)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@friendId", friendId);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.ExecuteNonQuery();
            }
            ConfigureButton(btnUnfriend, "Unfriend", false, false);
            ConfigureButton(btnAdd, "Add Friend", false, true);
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
                string query = "SELECT post_id, post_title FROM Posts WHERE account_id = @AccountId AND post_status = 'Posted'";
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
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
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
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
                string currentUserId = Convert.ToString(Session["account_id"]);
                LoadProfile(profileId);
                LoadPosts(profileId, currentUserId);
                ConfigureFriendButton(profileId, currentUserId);
            
        }

        private void ConfigureFriendButton(string profileId, string currentUserId)
        {
            if (!string.IsNullOrEmpty(profileId) && !string.IsNullOrEmpty(currentUserId) && profileId == currentUserId)
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
            string account1_id = Session["account_id"] as string;

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

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            string friendId = Request.QueryString["u"];
            string accountId = Session["account_id"] as string;

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
            string accountId = Convert.ToString(Session["account_id"]);
            string friendId = Request.QueryString["u"];

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(friendId))
            {
                // Ensure both account IDs are available
                return;
            }

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"
            DELETE FROM Friends 
            WHERE 
                (account1_id = @FriendId AND account2_id = @AccountId AND friend_status = 'Request')";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FriendId", friendId);
                    cmd.Parameters.AddWithValue("@AccountId", accountId);

                    cmd.ExecuteNonQuery();
                }
            }
            ConfigureButton(btnAccept, "Accept", false, false);
            ConfigureButton(btnReject, "Reject", false, false);
            ConfigureButton(btnAdd, "Add", false, true);
        }

        protected void btnUnfriend_Click(object sender, EventArgs e)
        { 
            string friendId = Request.QueryString["u"];
            string accountId = Convert.ToString(Session["account_id"]);

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

        private void LoadProfile(string accountId)
        {
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
                        // Set account name
                        lblAccountName.Text = reader["account_name"].ToString();

                        string profileImage = reader["profile_image"].ToString();

                        if (!string.IsNullOrEmpty(profileImage))
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/" + profileImage;
                        }
                        else
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
                        }
                    }
                    pnlDeletedMessage.Visible = !reader.HasRows; 
                    pnlProfileDetails.Visible = reader.HasRows;
                    con.Close();
                }
            }
        }

        private void LoadPosts(string profileId, string currentUserId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        post_id, 
                        post_title, 
                        file_type 
                    FROM 
                        Posts 
                    WHERE 
                        post_status = 'Posted'
                        AND
                        account_id = @profileId
                        AND (
                            post_permission = 'Public' 
                            OR (post_permission = 'Friend' AND EXISTS (
                                SELECT 1 
                                FROM Friends 
                                WHERE 
                                    (account1_id = Posts.account_id AND account2_id = @currentAccountId AND friend_status = 'Accepted') 
                                    OR 
                                    (account2_id = Posts.account_id AND account1_id = @currentAccountId AND friend_status = 'Accepted')
                                    OR
                                    (Posts.account_id = @currentAccountId)
                            ))
                            OR (post_permission = 'Owner' AND account_id = @currentAccountId)
                        )";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@profileId", profileId);
                    cmd.Parameters.AddWithValue("@currentAccountId", currentUserId);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    PostsRepeater.DataSource = dt;
                    PostsRepeater.DataBind();

                    con.Close();
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace FYP_TravelPlanner.Traveller
{
    public partial class Friends : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string accountId;
            if (!IsPostBack)
            {
                if (Session["account_id"] != null)
                {
                    accountId = Session["account_id"].ToString();
                    BindFriends();
                    BindFriendRequests();
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        private void BindFriends()
        {
            string accountId = Convert.ToString(Session["account_id"]);

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT a.account_id, a.account_name
            FROM Account a
            INNER JOIN Friends f ON 
                 (f.account1_id = @account_id AND f.account2_id = a.account_id OR 
                  f.account2_id = @account_id AND f.account1_id = a.account_id)
            WHERE f.friend_status = 'Accepted'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("profile_image_url", typeof(string));
               

                rptFriendsList.DataSource = dt;
                rptFriendsList.DataBind();
            }
        }

        protected void rptFriendsList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectFriend")
            {
                string selectedFriendId = e.CommandArgument.ToString();
                Response.Redirect("~/Account/Profile.aspx?account_id=" + selectedFriendId);
            }
        }

        private void BindFriendRequests()
        {
            string accountId = Convert.ToString(Session["account_id"]);

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT a.account_id, a.account_name
            FROM Account a
            INNER JOIN Friends f ON f.account1_id = a.account_id
            WHERE f.account2_id = @account_id AND f.friend_status = 'Request'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("profile_image_url", typeof(string));

                rptFriendRequests.DataSource = dt;
                rptFriendRequests.DataBind();
            }
        }

        protected void rptFriendRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            string accountId = Convert.ToString(Session["account_id"]);

            string friendId = Convert.ToString(e.CommandArgument);
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                if (e.CommandName == "Accept")
                {
                    string query = "UPDATE Friends SET friend_status = 'Accepted' WHERE account1_id = @friendId AND account2_id = @account_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@friendId", friendId);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    cmd.ExecuteNonQuery();
                }
                else if (e.CommandName == "Reject")
                {
                    string query = "DELETE FROM Friends WHERE account1_id = @friendId AND account2_id = @account_id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@friendId", friendId);
                    cmd.Parameters.AddWithValue("@account_id", accountId);
                    cmd.ExecuteNonQuery();
                }
            }

            // Rebind lists after action
            BindFriendRequests();
            BindFriends();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string accountId = Convert.ToString(Session["account_id"]);
            string searchTerm = txtSearch.Text.Trim(); 
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT a.account_id, a.account_name
            FROM Account a
            INNER JOIN Friends f ON 
                 (f.account1_id = @account_id AND f.account2_id = a.account_id OR 
                  f.account2_id = @account_id AND f.account1_id = a.account_id)
            WHERE f.status = 'Accepted' AND a.account_name LIKE @searchTerm";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);


                rptFriendsList.DataSource = dt;
                rptFriendsList.DataBind();
            }
        }

        protected void imgProfile_DataBinding(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            string accountId = DataBinder.Eval(((RepeaterItem)img.NamingContainer).DataItem, "account_id").ToString();
            BindProfileImage(img, accountId);
        }

        protected void BindProfileImage(Image img, string accountId)
        {
            string imagePath = Server.MapPath("~/Uploads/Profile/" + accountId + ".jpg");

            if (System.IO.File.Exists(imagePath))
            {
                img.ImageUrl = "~/Uploads/Profile/" + accountId + ".jpg";
            }
            else
            {
                img.ImageUrl = "~/Uploads/Profile/unknown.jpg"; // Fallback image
            }
        }
    }
}
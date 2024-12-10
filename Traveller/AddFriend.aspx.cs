using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class AddFriend : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["account_id"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindRepeater(); 
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string account1_id = Session["account_id"].ToString(); // Logged in user's ID
            string account2_id = Convert.ToString(e.CommandArgument); // Selected user's ID

            if (e.CommandName == "AddFriend")
            {
                AddNewFriend(account1_id, account2_id); 
            }
            else if (e.CommandName == "Unfriend")
            {
                RemoveFriend(account1_id, account2_id);
            }

            BindRepeater();
        }

        private void BindRepeater()
        {
            string searchQuery = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            account_id, 
                            account_name, 
                            (SELECT friend_status 
                             FROM Friends 
                             WHERE (account1_id = @currentAccount AND account2_id = Account.account_id) 
                                OR (account2_id = @currentAccount AND account1_id = Account.account_id)) AS friend_status
                        FROM Account
                        WHERE account_name LIKE @search AND account_id != @currentAccount
                        ORDER BY account_name";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");
                    cmd.Parameters.AddWithValue("@currentAccount", Session["account_id"]);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptResults.DataSource = dt;
                    rptResults.DataBind();

                    lblNoResults.Visible = dt.Rows.Count == 0;
                }
            }
        }

        private void AddNewFriend(string account1_id, string account2_id)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"INSERT INTO Friends (account1_id, account2_id, friend_date, friend_status) 
                                 VALUES (@account1_id, @account2_id, @friend_date, @friend_status)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@account1_id", account1_id);
                cmd.Parameters.AddWithValue("@account2_id", account2_id);
                cmd.Parameters.AddWithValue("@friend_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@friend_status", "Request");

                cmd.ExecuteNonQuery();
            }
        }

        private void RemoveFriend(string account1_id, string account2_id)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                string query = @"DELETE FROM Friends 
                                 WHERE (account1_id = @account1_id AND account2_id = @account2_id)
                                    OR (account2_id = @account1_id AND account1_id = @account2_id)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@account1_id", account1_id);
                cmd.Parameters.AddWithValue("@account2_id", account2_id);

                cmd.ExecuteNonQuery();
            }
        }

        protected void imgProfile_DataBinding(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            string accountId = DataBinder.Eval(((RepeaterItem)img.NamingContainer).DataItem, "account_id").ToString();
            string imagePath = Server.MapPath("~/Uploads/Profile/" + accountId + ".jpg");

            if (System.IO.File.Exists(imagePath))
            {
                img.ImageUrl = "~/Uploads/Profile/" + accountId + ".jpg";
            }
            else
            {
                img.ImageUrl = "~/Uploads/Profile/unknown.jpg"; 
            }
        }

        protected string GetButtonText(object friendStatus)
        {
            if (friendStatus == DBNull.Value || friendStatus == null)
                return "Add";
            else if (friendStatus.ToString() == "Request")
                return "Sent";
            else if (friendStatus.ToString() == "Accepted")
                return "Unfriend";
            else
                return "Add";
        }

        protected string GetCommandName(object friendStatus)
        {
            if (friendStatus == DBNull.Value || friendStatus == null)
                return "AddFriend";
            else if (friendStatus.ToString() == "Accepted")
                return "Unfriend";
            else
                return ""; 
        }

        protected bool IsButtonEnabled(object friendStatus)
        {
            return friendStatus == DBNull.Value || friendStatus == null || friendStatus.ToString() == "Accepted";
        }
    }
}
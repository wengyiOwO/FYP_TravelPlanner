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

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchQuery))
            {
                string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT account_id, account_name FROM Account WHERE account_name LIKE @search";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptResults.DataSource = dt;
                    rptResults.DataBind();

                    lblNoResults.Visible = dt.Rows.Count == 0; // Display "No user found" if no results
                }
            }
        }

        protected void rptResults_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddFriend")
            {
                string account2_id = Convert.ToString(e.CommandArgument);
                string account1_id = Convert.ToString("AC0521"); // Assuming session holds current user's ID
                //string account1_id = Convert.ToString(Session["account_id"]);
                // Insert friend request into Friends table
                string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "INSERT INTO Friends (account1_id, account2_id, friend_date, friend_status) VALUES (@account1_id, @account2_id, @friend_date, @friend_status)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@account1_id", account1_id);
                    cmd.Parameters.AddWithValue("@account2_id", account2_id);
                    cmd.Parameters.AddWithValue("@friend_date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@friend_status", "Request");

                    cmd.ExecuteNonQuery();
                }

                // Display pop-up message
                ScriptManager.RegisterStartupScript(this, GetType(), "Popup", "alert('Friend request sent!');", true);
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
                img.ImageUrl = "~/Uploads/Profile/unknown.jpg"; // Fallback image
            }
        }
    }
}
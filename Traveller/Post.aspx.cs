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
    public partial class Post : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                BindPosts(null); 
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            BindPosts(searchQuery);
        }

        private void BindPosts(string searchQuery)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        post_id, 
                        post_title, 
                        file_type 
                    FROM 
                        Posts 
                    WHERE 
                        post_status = 'Posted' 
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

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    query += " AND post_title LIKE @searchQuery";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currentAccountId", Session["account_id"]);

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    cmd.Parameters.AddWithValue("@searchQuery", "%" + searchQuery + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                PostsRepeater.DataSource = dt;
                PostsRepeater.DataBind();
            }
        }
    }
}
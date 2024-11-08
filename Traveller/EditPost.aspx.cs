using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected string postId;

        protected void Page_Load(object sender, EventArgs e)
        {
            postId = Request.QueryString["p"];
            if (!IsPostBack)
            {
                LoadPostData(postId);
            }
        }

        private void LoadPostData(string postId)
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT post_title, post_content, post_permission, file_type FROM Posts WHERE post_id = @PostID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtPostTitle.Text = reader["post_title"].ToString();
                            txtPostContent.Text = reader["post_content"].ToString();
                            rblPostPermission.SelectedValue = reader["post_permission"].ToString();

                            LoadExistingFiles(postId, reader["file_type"].ToString());
                        }
                    }
                }
            }
        }

        private void LoadExistingFiles(string postId, string fileType)
        {
            // Display existing images or video based on fileType
            if (fileType == "image")
            {
                // Load images
                var images = Directory.GetFiles(Server.MapPath("~/Uploads/Images"), $"{postId}_*.jpg");
                foreach (var image in images)
                {
                    // Render image preview
                }
            }
            else if (fileType == "video")
            {
                // Load video
                string videoPath = Server.MapPath("~/Uploads/Videos/" + postId + ".mp4");
                if (File.Exists(videoPath))
                {
                    // Render video preview
                }
            }
        }

        protected void btnUpdatePost_Click(object sender, EventArgs e)
        {
            // Handle file upload, update database with new images or video
            // Similar to btnCreatePost_Click in CreatePost.aspx.cs
            // ...
            Response.Redirect("~/Traveller/PostDetails.aspx?post_id=" + postId);
        }
    }
}
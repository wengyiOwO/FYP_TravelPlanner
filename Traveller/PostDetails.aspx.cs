using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class PostDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string postId = Request.QueryString["post_id"];
                if (!string.IsNullOrEmpty(postId))
                {
                    LoadPostDetails(postId);
                }
                else
                {
                    Response.Write("Invalid Post ID");
                }
            }
        }

        protected void LoadPostDetails(string postId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT P.post_title, P.post_content, P.post_date, P.file_type, P.num_image, A.account_id, A.account_name 
                         FROM Posts P
                         INNER JOIN Account A ON P.account_id = A.account_id
                         WHERE P.post_id = @PostID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblPostTitle.Text = reader["post_title"].ToString();
                        ltPostContent.Text = reader["post_content"].ToString();
                        lblAuthorName.Text = reader["account_name"].ToString();
                        lblPostDate.Text = Convert.ToDateTime(reader["post_date"]).ToString("dd/MM/yyyy HH:mm:ss");

                        // Load the profile image
                        string accountId = reader["account_id"].ToString();
                        string imagePath = Server.MapPath("~/Uploads/Profile/") + accountId + ".jpg";

                        if (System.IO.File.Exists(imagePath))
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/" + accountId + ".jpg";
                        }
                        else
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
                        }

                        // Check the file type and load content accordingly
                        string fileType = reader["file_type"].ToString();
                        int numImages = Convert.ToInt32(reader["num_image"]);
                        if (fileType == "video")
                        {
                            // Load video if file type is video
                            LoadPostVideo(postId);
                            carouselControls.Visible = false;
                        }
                        else if (numImages > 0)
                        {
                            // Load images into carousel if there are images
                            LoadPostImages(postId, numImages);
                            carouselControls.Visible = numImages > 1;
                        }
                        else
                        {
                            carouselControls.Visible = false;
                        }
                    }
                    con.Close();
                }
            }
        }

        private void LoadPostVideo(string postId)
        {
            string videoFileName = $"{postId}.mp4";
            string videoPath = ResolveUrl($"~/Uploads/Videos/{videoFileName}");

            // Generate HTML for video player with controls
            videoLiteral.Text = $@"
        <div class='carousel-item active'>
            <div class='image-container'>
                <video controls>
                    <source src='{videoPath}' type='video/mp4'>
                    Your browser does not support the video tag.
                </video>
            </div>
        </div>";
        }

        private void LoadPostImages(string postId, int numImages)
        {
            carouselInner.Controls.Clear();

            for (int i = 1; i <= numImages; i++)
            {
                string imageFileName = $"{postId}_{i}.jpg";
                string imagePath = ResolveUrl($"~/Uploads/Images/{imageFileName}");

                var itemDiv = new LiteralControl();
                string activeClass = (i == 1) ? "active" : "";
                itemDiv.Text = $@"
            <div class='carousel-item {activeClass}'>
                <div class='image-container'>
                    <img src='{imagePath}' alt='Image {i}'>
                </div>
            </div>";

                carouselInner.Controls.Add(itemDiv);
            }
        }



        protected void btnEdit_Click(object sender, EventArgs e)
        {

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string postId = Request.QueryString["PostID"];

            if (!string.IsNullOrEmpty(postId))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE Posts SET post_status = 'Deleted' WHERE PostID = @PostID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PostID", postId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
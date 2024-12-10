using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class PostDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                string postId = Request.QueryString["p"];
                if (!string.IsNullOrEmpty(postId))
                {
                    LoadPostDetails(postId);
                    BindFriends();
                }
                else
                {
                    lblDeletedMessage.Text = "Invalid Post";
                    pnlDeletedMessage.Visible = true;
                    pnlPostDetails.Visible = false;
                }
            }
            
        }


        protected void LoadPostDetails(string postId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string currentAccountId = Session["account_id"]?.ToString(); // Get current user's account ID

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                P.post_title, P.post_content, P.post_date, P.post_status, P.file_type, 
                P.num_image, A.account_id, A.account_name, A.profile_image, P.post_permission
            FROM 
                Posts P
            INNER JOIN Account A ON P.account_id = A.account_id
            WHERE 
                P.post_id = @PostID
                AND (
                    P.post_permission = 'Public' 
                    OR (P.post_permission = 'Friend' AND EXISTS (
                        SELECT 1 
                        FROM Friends 
                        WHERE 
                            (account1_id = P.account_id AND account2_id = @currentAccountId AND friend_status = 'Accepted') 
                            OR (account2_id = P.account_id AND account1_id = @currentAccountId AND friend_status = 'Accepted')
                        ))
                    OR (P.post_permission = 'Owner' AND P.account_id = @currentAccountId)
                )";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    cmd.Parameters.AddWithValue("@currentAccountId", currentAccountId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string postStatus = reader["post_status"].ToString();

                        if (postStatus == "Deleted")
                        {
                            lblDeletedMessage.Text = "This post has been deleted.";
                            pnlDeletedMessage.Visible = true;
                            pnlPostDetails.Visible = false;
                            return;
                        }

                        lblPostTitle.Text = reader["post_title"].ToString();
                        string postContent = reader["post_content"].ToString();
                        postContent = postContent.Replace(Environment.NewLine, "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
                        ltPostContent.Text = postContent;
                        lblAuthorName.Text = reader["account_name"].ToString();
                        lblPostDate.Text = Convert.ToDateTime(reader["post_date"]).ToString("dd/MM/yyyy");

                        string authorId = reader["account_id"].ToString();
                        string profileImage = reader["profile_image"].ToString();

                        if (!string.IsNullOrEmpty(profileImage))
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/" + profileImage;
                        }
                        else
                        {
                            imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
                        }

                        string fileType = reader["file_type"].ToString();
                        int numImages = Convert.ToInt32(reader["num_image"]);
                        if (fileType == "video")
                        {
                            LoadPostVideo(postId);
                            carouselControls.Visible = false;
                        }
                        else if (numImages > 0)
                        {
                            LoadPostImages(postId, numImages);
                            carouselControls.Visible = numImages > 1;
                        }
                        else
                        {
                            carouselControls.Visible = false;
                        }

                        if (currentAccountId == authorId)
                        {
                            btnEdit.Visible = true;
                            btnDelete.Visible = true;
                        }
                        else
                        {
                            btnEdit.Visible = false;
                            btnDelete.Visible = false;
                        }
                    }
                    else
                    {
                        lblDeletedMessage.Text = "You do not have permission to view the post.";
                        pnlDeletedMessage.Visible = true;
                        pnlPostDetails.Visible = false;
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
            string postId = Request.QueryString["p"];
            Response.Redirect("~/Traveller/EditPost.aspx?p=" + postId);
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string postId = Request.QueryString["p"];

            if (!string.IsNullOrEmpty(postId))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string updateQuery = "UPDATE Posts SET post_status = 'Deleted' WHERE post_id = @PostID";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@PostID", postId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }


            successPanel.Visible = true;

            // Inject JavaScript to redirect after 2 seconds
            string redirectScript = $@"
            <script type='text/javascript'>
                setTimeout(function() {{
                    window.location.href = 'Post.aspx';
                }}, 2000);
            </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "RedirectScript", redirectScript);
        }

        protected void btnShare_Click(object sender, EventArgs e)
        {
            BindFriends();
        }
        private void BindFriends()
        {
            string accountId = Convert.ToString(Session["account_id"]);

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT a.account_id, a.account_name, a.profile_image
            FROM Account a
            INNER JOIN Friends f ON 
                 (f.account1_id = @account_id AND f.account2_id = a.account_id OR 
                  f.account2_id = @account_id AND f.account1_id = a.account_id)
            WHERE f.friend_status = 'Accepted'
            ORDER BY a.account_name";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptFriendsList.DataSource = dt;
                rptFriendsList.DataBind();
            }
        }

        protected void rptFriendsList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string accountId = Convert.ToString(Session["account_id"]);
            string friendId = Convert.ToString(e.CommandArgument);
            string newChatId = GenerateChatId(accountId, DateTime.Now);
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string messageContent = Request.Url.AbsoluteUri;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                if (e.CommandName == "Send")
                {
                    string query = "INSERT INTO Chat (chat_id, sender_id, receiver_id, chat_datetime, chat_message, message_type) " +
                               "VALUES (@ChatId, @SenderId, @ReceiverId, @DateTime, @Message, @MessageType)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ChatId", newChatId);
                    cmd.Parameters.AddWithValue("@SenderId", accountId);
                    cmd.Parameters.AddWithValue("@ReceiverId", friendId);
                    cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Message", messageContent);
                    cmd.Parameters.AddWithValue("@MessageType", "link");
                    cmd.ExecuteNonQuery();
                }
            }
            Button btnSend = (Button)e.CommandSource;
            btnSend.Text = "Sent";
            btnSend.Enabled = false;
            pnlFriend.Visible = true;

            BindFriends();

            string postId = Request.QueryString["p"];
            LoadPostDetails(postId);
            lblNotification.Text = "Message sent successfully!";
            notification.Style["display"] = "block";

            ScriptManager.RegisterStartupScript(this, GetType(), "HideNotification",
                "setTimeout(function() { document.getElementById('" + notification.ClientID + "').style.display='none'; }, 2000);", true);

        }

        
        private string GenerateChatId(string accountId, DateTime dateTime)
        {
            // Append a GUID to make it more unique
            return $"C{accountId}{dateTime:yyyyMMddHHmmss}_{Guid.NewGuid()}";
        }

    }
}
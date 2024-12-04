using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.Services;
using System.IO;

namespace FYP_TravelPlanner.Traveller
{
    public partial class Chat : System.Web.UI.Page
    {
        private string accountId;
        private string selectedFriendId;
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["account_id"] != null)
                {
                    accountId = Convert.ToString(Session["account_id"]);
                    LoadFriends();
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                }

            }
            else
            {
                accountId = Convert.ToString(Session["account_id"]);
                LoadFriends();
            }

            if (ViewState["SelectedFriendId"] != null)
            {
                selectedFriendId = ViewState["SelectedFriendId"].ToString();
                LoadSelectedFriendDetails();
            }
            else
            {
                selectedFriendId = null;
            }
        }

        protected void rptFriendsList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectFriend")
            {
                selectedFriendId = e.CommandArgument.ToString();
                chatPanel.Visible = true;
                LoadChatMessages();
                LoadSelectedFriendDetails();
            }
        }

        private void LoadSelectedFriendDetails()
        {
            if (string.IsNullOrEmpty(selectedFriendId))
                return;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT account_name, profile_image FROM Account WHERE account_id = @FriendId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FriendId", selectedFriendId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    litSelectedFriendName.Text = reader["account_name"].ToString();

                    string profileImage = reader["profile_image"].ToString();

                    if (!string.IsNullOrEmpty(profileImage))
                    {
                        imgProfile.ImageUrl = "~/Uploads/Profile/" + profileImage;
                    }
                    else
                    {
                        imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
                    }


                    ViewState["SelectedFriendId"] = selectedFriendId;
                }
                reader.Close();
            }
        }

        private void LoadFriends()
        {
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
                    WHERE f.friend_status = 'Accepted'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptFriendsList.DataSource = dt;
                rptFriendsList.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchQuery = txtSearch.Text.Trim();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT a.account_id, a.account_name, a.profile_image
                    FROM Account a
                    INNER JOIN Friends f ON 
                        (f.account1_id = @account_id AND f.account2_id = a.account_id OR 
                         f.account2_id = @account_id AND f.account1_id = a.account_id)
                    WHERE f.friend_status = 'Accepted' AND a.account_name LIKE @searchTerm";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.Parameters.AddWithValue("@searchTerm", "%" + searchQuery + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptFriendsList.DataSource = dt;
                rptFriendsList.DataBind();
            }
        }


        private void LoadChatMessages()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Chat.sender_id, Chat.chat_message, Chat.message_type, Chat.chat_datetime, Account.account_name AS sender_name, Account.profile_image
            FROM Chat
            INNER JOIN Account ON Chat.sender_id = Account.account_id
            WHERE (Chat.sender_id = @account_id AND Chat.receiver_id = @friend_id) 
               OR (Chat.sender_id = @friend_id AND Chat.receiver_id = @account_id)
            ORDER BY Chat.chat_datetime";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.Parameters.AddWithValue("@friend_id", selectedFriendId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptChatMessages.DataSource = dt;
                rptChatMessages.DataBind();
            }
        }




        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMessage.Text) || fileUpload.HasFiles)
            {
                string newChatId = GenerateChatId(accountId, DateTime.Now);

                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    string messageType = "text";
                    string messageContent = txtMessage.Text.Trim();
                    SaveMessage(messageContent, messageType);
                }

                if (fileUpload.HasFiles)
                {
                    foreach (HttpPostedFile file in fileUpload.PostedFiles)
                    {
                        string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string fileName = $"{newChatId}_{Guid.NewGuid()}"; 

                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            string filePath = Server.MapPath($"~/Uploads/Chat/{fileName}.jpg");
                            file.SaveAs(filePath);
                            string messageContent = fileName + ".jpg"; 
                            string messageType = "image"; 
                                                          
                            SaveMessage(messageContent, messageType);
                        }
                        else if (fileExtension == ".mp4" || fileExtension == ".mov" || fileExtension == ".avi")
                        {
                            string filePath = Server.MapPath($"~/Uploads/Chat/{fileName}.mp4");
                            file.SaveAs(filePath);
                            string messageContent = fileName + ".mp4"; 
                            string messageType = "video"; 
                                                         
                            SaveMessage(messageContent, messageType);
                        }
                    }
                }

                txtMessage.Text = "";
                fileUpload.Attributes.Clear();

                LoadChatMessages();
            }
        }

        private void SaveMessage(string messageContent, string messageType)
        {
            string newChatId = GenerateChatId(accountId, DateTime.Now);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Chat (chat_id, sender_id, receiver_id, chat_datetime, chat_message, message_type) " +
                               "VALUES (@ChatId, @SenderId, @ReceiverId, @DateTime, @Message, @MessageType)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ChatId", newChatId);
                cmd.Parameters.AddWithValue("@SenderId", accountId);
                cmd.Parameters.AddWithValue("@ReceiverId", selectedFriendId);
                cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@Message", messageContent);
                cmd.Parameters.AddWithValue("@MessageType", messageType);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private string GenerateChatId(string accountId, DateTime dateTime)
        {
            // Append a GUID to make it more unique
            return $"C{accountId}{dateTime:yyyyMMddHHmmss}_{Guid.NewGuid()}";
        }

        [WebMethod]
        public static string GetChatMessages(string accountId, string friendId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            Chat.sender_id, 
            Chat.chat_message, Chat.message_type,
            FORMAT(Chat.chat_datetime, 'hh:mm tt') AS formatted_time, 
            Account.account_name AS sender_name, 
            Account.profile_image
        FROM Chat
        INNER JOIN Account ON Chat.sender_id = Account.account_id
        WHERE (Chat.sender_id = @account_id AND Chat.receiver_id = @friend_id) 
           OR (Chat.sender_id = @friend_id AND Chat.receiver_id = @account_id)
        ORDER BY Chat.chat_datetime";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);
                cmd.Parameters.AddWithValue("@friend_id", friendId);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return JsonConvert.SerializeObject(dt);
            }
        }

        protected void rptChatMessages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (DataRowView)e.Item.DataItem;

                // Profile image
                var imgProfile = (Image)e.Item.FindControl("imgMessageProfile");
                var profileImage = dataItem["profile_image"].ToString();
                imgProfile.ImageUrl = !string.IsNullOrEmpty(profileImage)
                    ? ResolveUrl("~/Uploads/Profile/" + profileImage)
                    : ResolveUrl("~/Uploads/Profile/unknown.jpg");

                // Timestamp
                var litTime = (Literal)e.Item.FindControl("litTime");
                litTime.Text = Convert.ToDateTime(dataItem["chat_datetime"]).ToString("hh:mm tt");

                // Sender name
                var litSender = (Literal)e.Item.FindControl("litSender");
                var senderId = dataItem["sender_id"].ToString();
                litSender.Text = senderId == Session["account_id"].ToString() ? "You" : dataItem["sender_name"].ToString();

                // Message content
                var phMessageContent = (PlaceHolder)e.Item.FindControl("phMessageContent");
                var messageType = dataItem["message_type"].ToString();
                var message = dataItem["chat_message"].ToString();

                if (messageType == "text")
                {
                    phMessageContent.Controls.Add(new Literal { Text = message });
                }
                else if (messageType == "image")
                {
                    phMessageContent.Controls.Add(new Literal
                    {
                        Text = $"<img src='{ResolveUrl("~/Uploads/Chat/" + message)}' class='img-fluid' />"
                    });
                }
                else if (messageType == "video")
                {
                    phMessageContent.Controls.Add(new Literal
                    {
                        Text = $"<video src='{ResolveUrl("~/Uploads/Chat/" + message)}' class='w-100' controls></video>"
                    });
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Web.Services;


namespace FYP_TravelPlanner.Traveller
{
    public partial class Chat : System.Web.UI.Page
    {
        private string _accountId;
        private string _selectedFriendId;

        public string accountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        public string selectedFriendId
        {
            get { return _selectedFriendId; }
            set { _selectedFriendId = value; }
        }
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
                string query = "SELECT account_name FROM Account WHERE account_id = @FriendId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FriendId", selectedFriendId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    litSelectedFriendName.Text = reader["account_name"].ToString();

                    ViewState["SelectedFriendId"] = selectedFriendId;

                    imgProfile.DataBind();
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
                    SELECT a.account_id, a.account_name
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
            SELECT Chat.sender_id, Chat.chat_message, Chat.chat_datetime, Account.account_name AS sender_name
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
            if (!string.IsNullOrWhiteSpace(txtMessage.Text) && !string.IsNullOrEmpty(selectedFriendId))
            {
                string newChatId = GenerateChatId(accountId, DateTime.Now); 

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Chat (chat_id, sender_id, receiver_id, chat_datetime, chat_message) VALUES (@ChatId, @SenderId, @ReceiverId, @DateTime, @Message)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ChatId", newChatId);
                    cmd.Parameters.AddWithValue("@SenderId", accountId);
                    cmd.Parameters.AddWithValue("@ReceiverId", selectedFriendId);
                    cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Message", txtMessage.Text.Trim());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                txtMessage.Text = "";
                LoadChatMessages();
            }
        }

        private string GenerateChatId(string accountId, DateTime dateTime)
        {
            return $"C{accountId}{dateTime:yyyyMMddHHmmss}";
        }

        protected void imgProfile_DataBinding(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            string accountId = null;

            if (img.NamingContainer is RepeaterItem repeaterItem)
            {
                if (repeaterItem.Parent == rptFriendsList)
                {
                    accountId = DataBinder.Eval(repeaterItem.DataItem, "account_id")?.ToString();
                }
                else if (repeaterItem.Parent == rptChatMessages)
                {
                    accountId = DataBinder.Eval(repeaterItem.DataItem, "sender_id")?.ToString();
                }
            }
            else if (ViewState["SelectedFriendId"] != null)
            {
                accountId = ViewState["SelectedFriendId"].ToString();
            }

            if (!string.IsNullOrEmpty(accountId))
            {
                BindProfileImage(img, accountId);
            }
            else
            {
                img.ImageUrl = "~/Uploads/Profile/unknown.jpg"; 
            }
        }

        protected void BindProfileImage(Image img, string accountId)
        {
            string imagePath = Server.MapPath($"~/Uploads/Profile/{accountId}.jpg");

            if (System.IO.File.Exists(imagePath))
            {
                img.ImageUrl = $"~/Uploads/Profile/{accountId}.jpg";
            }
            else
            {
                img.ImageUrl = "~/Uploads/Profile/unknown.jpg"; 
            }
        }

        [WebMethod]
        public static string GetChatMessages(string accountId, string friendId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Chat.sender_id, Chat.chat_message, Chat.chat_datetime, Account.account_name AS sender_name
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

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Serialize(dt);
            }
        }
    }
}
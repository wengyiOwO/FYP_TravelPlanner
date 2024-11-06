using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class EditProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string accountId = Session["Account_ID"].ToString();
                string accountId = "AC0521";
                LoadAccountDetails(accountId);
            }
        }

        private void LoadAccountDetails(string accountId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT account_name, account_email, account_phoneNo FROM Account WHERE account_id = @Account_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Account_ID", accountId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    inputName.Text = reader["account_name"].ToString();
                    inputEmailAddress.Text = reader["account_email"].ToString();
                    inputPhone.Text = reader["account_phoneNo"].ToString();
                }
            }

            string profileImagePath = Server.MapPath("~/Uploads/Profile/") + accountId + ".jpg";
            if (System.IO.File.Exists(profileImagePath))
            {
                imgProfile.ImageUrl = "~/Uploads/Profile/" + accountId + ".jpg";
            }
            else
            {
                imgProfile.ImageUrl = "~/Uploads/Profile/unknown.jpg";
            }
        }
        protected void SaveChanges_Click(object sender, EventArgs e)
        {
            //string accountId = Session["Account_ID"].ToString();
            string accountId = "AC0521";

            string accountName = inputName.Text;
            string phone = inputPhone.Text;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string updateQuery = "UPDATE Account SET account_name = @Name, account_phoneNo = @Phone WHERE account_id = @Account_ID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Name", accountName);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(phone) ? DBNull.Value : (object)phone);
                cmd.Parameters.AddWithValue("@Account_ID", accountId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            StatusMessage.Text = "Your changes have been saved successfully!";
            StatusMessage.ForeColor = System.Drawing.Color.Green;
            StatusMessage.Visible = true; 
        }

        protected void UploadProfileImage_Click(object sender, EventArgs e)
        {
            if (ProfileImageUpload.HasFile)
            {
                string accountId = "AC0521"; 
                string filePath = Server.MapPath($"~/Uploads/Profile/{accountId}.jpg");

                ProfileImageUpload.SaveAs(filePath);

                imgProfile.ImageUrl = $"~/Uploads/Profile/{accountId}.jpg?t={DateTime.Now.Ticks}"; 

                StatusMessage.Text = "Profile image uploaded successfully!";
                StatusMessage.ForeColor = System.Drawing.Color.Green;
                StatusMessage.Visible = true; 
            }
            else
            {
                StatusMessage.Text = "Please select a file to upload.";
                StatusMessage.ForeColor = System.Drawing.Color.Red; 
                StatusMessage.Visible = true; 
            }
        }
    }
}
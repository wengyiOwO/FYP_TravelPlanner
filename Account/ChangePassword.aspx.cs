using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string accountId = Session["account_id"].ToString();
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;

            // Retrieve current password hash and salt from the database
            (string storedSalt, string storedHash) = GetCurrentPassword(accountId);

            if (storedSalt != null && storedHash != null)
            {
                //  Verify the entered current password
                string hashedCurrentPassword = HashPassword(currentPassword, storedSalt);

                if (hashedCurrentPassword == storedHash)
                {
                    //  Generate a new salt and hash the new password
                    string newSalt = GenerateSalt();
                    string newHashedPassword = HashPassword(newPassword, newSalt);

                    //  Update the password in the database
                    if (UpdatePassword(accountId, newSalt, newHashedPassword))
                    {
                        lblMessage.Text = "Password changed successfully.";
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblMessage.Text = "Error updating password. Please try again later.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
                else
                {
                    lblMessage.Text = "Current password is incorrect.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblMessage.Text = "Error retrieving password details.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        private (string salt, string hash) GetCurrentPassword(string accountId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT account_salt, account_password FROM Account WHERE account_id = @AccountId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string salt = reader["account_salt"].ToString();
                            string hash = reader["account_password"].ToString();
                            return (salt, hash);
                        }
                    }
                }
            }
            return (null, null);
        }

        private bool UpdatePassword(string accountId, string newSalt, string newHashedPassword)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "UPDATE Account SET account_salt = @Salt, account_password = @Password WHERE account_id = @AccountId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Salt", newSalt);
                    command.Parameters.AddWithValue("@Password", newHashedPassword);
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        private string HashPassword(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }
    }
}
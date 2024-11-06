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
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string token = Request.QueryString["token"];
                if (string.IsNullOrEmpty(token) || !IsTokenValid(token))
                {
                    lblMessage.Text = "Invalid or expired reset link.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    btnResetPassword.Enabled = false;
                }
            }
        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];
            string newPassword = txtNewPassword.Text;
            if (IsTokenValid(token))
            {
                // Hash the new password with a salt 
                string salt = GenerateSalt();

                string hashedPassword = HashPassword(newPassword, salt);

                if (UpdatePasswordWithToken(token, hashedPassword,salt))
                {
                    lblMessage.Text = "Your password has been reset successfully!";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblMessage.Text = "Failed to reset password. Please try again.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblMessage.Text = "Invalid or expired reset link.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                btnResetPassword.Enabled = false;

            }
        }

        private bool IsTokenValid(string token)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) FROM Account WHERE reset_token = @Token AND token_expiry > @Now";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);
                    command.Parameters.AddWithValue("@Now", DateTime.Now);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool UpdatePasswordWithToken(string token, string hashedPassword, string salt)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "UPDATE Account SET account_password = @Password, account_salt = @Salt, reset_token = NULL, token_expiry = NULL WHERE reset_token = @Token";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Salt", salt);
                    command.Parameters.AddWithValue("@Token", token);
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
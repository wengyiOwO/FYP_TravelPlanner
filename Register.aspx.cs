using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        public string GenerateNextID()
        {
            // Connect to your database
            SqlConnection conn;
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(strCon);
            conn.Open();

            // Retrieve the last ID from the database
            string lastID = GetLastIDFromDatabase(conn);

            // Generate the next ID
            string nextID = IncrementID(lastID);


            return nextID;

        }
        private string GetLastIDFromDatabase(SqlConnection connection)
        {
            string query = "SELECT TOP 1 account_id FROM Account ORDER BY account_id DESC";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        private string IncrementID(string lastID)
        {
            if (string.IsNullOrEmpty(lastID))
            {
                return "AC000001";
            }

            int numericPart = int.Parse(lastID.Substring(2)) + 1;
            string nextID = $"AC{numericPart:D6}";

            while (IDExistsInDatabase(nextID))
            {
                numericPart++;
                nextID = $"AC{numericPart:D6}";
            }

            return nextID;
        }

        private bool IDExistsInDatabase(string idToCheck)
        {
            SqlConnection conn;
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(strCon);
            conn.Open();

            string query = "SELECT COUNT(*) FROM Account WHERE account_id = @ID";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ID", idToCheck);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }

              }
        private void AddAccIntoDatabase()
        {
            SqlConnection conn;
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(strCon);
            conn.Open();

            string strInsert = "Insert Into Account(account_id,account_name,account_phoneNo,account_password,account_salt,account_email,account_status,account_role) VALUES(@AccountID,@Name,@Phone,@Password,@Salt,@Email,@Status,@Role) ";

            SqlCommand cmdInsert;
            cmdInsert = new SqlCommand(strInsert, conn);

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(txtPassword.Text.ToString(), salt);

            cmdInsert.Parameters.AddWithValue("@AccountID", txtAccID.Text.ToString());
            cmdInsert.Parameters.AddWithValue("@Name", txtName.Text.ToString());
            cmdInsert.Parameters.AddWithValue("@Phone", txtPhoneNum.Text.ToString());
            cmdInsert.Parameters.AddWithValue("@Password", hashedPassword);
            cmdInsert.Parameters.AddWithValue("@Salt", salt);
            cmdInsert.Parameters.AddWithValue("@Email", txtEmail.Text.ToString());
            cmdInsert.Parameters.AddWithValue("@Status", "Active");
            cmdInsert.Parameters.AddWithValue("@Role", "Traveller");



            cmdInsert.ExecuteNonQuery();
            conn.Close();
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
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
        private bool EmailExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Account WHERE account_email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.ToString();

            // Check if the email already exists
            if (EmailExists(email))
            {
                lblMessage.Text = "This email is already registered. Please use a different email.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string accID = GenerateNextID();
            txtAccID.Text = accID; 
            AddAccIntoDatabase();
            lblMessage.Text = "An Account is created successfully!";
            lblMessage.ForeColor = System.Drawing.Color.Green;

        }
    }
}
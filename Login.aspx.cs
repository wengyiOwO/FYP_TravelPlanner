using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace FYP_TravelPlanner
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private (string salt, string hash, string role,string id,string username) GetUserDetails(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT account_id, account_name, account_salt, account_password, account_role FROM Account WHERE account_email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string id = reader["account_id"].ToString();
                            string username = reader["account_name"].ToString();
                            string salt = reader["account_salt"].ToString();
                            string hash = reader["account_password"].ToString();
                            string role = reader["account_role"].ToString();
                            return (salt, hash, role, id,username);
                        }
                    }
                }
            }
            return (null, null, null,null,null);
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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string email = inputEmail.Text.ToString();
                string password = inputPassword.Text.ToString();

                // Retrieve the stored salt and hashed password from the database
                (string storedSalt, string storedHash, string userRole,string accID,string username) = GetUserDetails(email);

                if (!string.IsNullOrEmpty(storedHash) && !string.IsNullOrEmpty(storedSalt))
                {
                    // Hash the input password using the stored salt
                    string hashedInputPassword = HashPassword(password, storedSalt);

                    if (hashedInputPassword == storedHash)
                    {
                        // Set the authentication cookie
                        FormsAuthentication.SetAuthCookie(email, customCheck.Checked);
                        Session["account_id"] = accID;
                        Session["account_name"] = username;
                        Session["account_role"] = userRole;
                        Session["account_email"] = email;
                        // Redirect based on user role
                        if (string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            Response.Redirect("~/Admin/Dashboard.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else if (string.Equals(userRole, "Traveller", StringComparison.OrdinalIgnoreCase))
                        {
                            if (Session["LocationsToSave"] != null)
                            {
                                // If locations to save exist, redirect to TravelPlan.aspx
                                Response.Redirect("~/Traveller/TravelPlan.aspx", false);
                            }
                            else
                            {
                                // Otherwise, redirect to CreateTravelPlan.aspx
                                Response.Redirect("~/Traveller/CreateTravelPlan.aspx", false);
                            }

                            Context.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            lblMessage.Text = $"User role '{userRole}' not recognized. Please contact support.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Visible = true;
                        }
                    }
                    else
                    {
                        lblMessage.Text = "Invalid email or password.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        lblMessage.Visible = true;
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid email or password.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Visible = true;
                }

                // Handle Remember Me functionality
                if (customCheck.Checked)
                {
                    HttpCookie rememberMeCookie = new HttpCookie("RememberMeCookie")
                    {
                        Values = { ["Username"] = email },
                        Expires = DateTime.Now.AddDays(30)
                    };
                    Response.Cookies.Add(rememberMeCookie);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"An error occurred: {ex.Message}";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
            }
        }

    }
}

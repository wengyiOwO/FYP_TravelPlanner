using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            string userEmail = txtEmail.Text;

            if (IsEmailRegistered(userEmail))
            {
                // Generate a unique reset token and set the expiry time to 5 minutes
                string resetToken = Guid.NewGuid().ToString();
                DateTime expiryTime = DateTime.Now.AddMinutes(5);

                // Store the token in the database
                if (SaveResetTokenToDatabase(userEmail, resetToken, expiryTime))
                {
                    // Send reset email with tokenized link
                    string resetLink = $"https://localhost:44387/ResetPassword.aspx?token={resetToken}";
                    bool emailSent = SendResetEmail(userEmail, resetLink);

                    if (emailSent)
                    {
                        lblMessage.Text = "A password reset link has been sent to your email.";
                        lblMessage.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lblMessage.Text = "There was an error sending the email. Please try again later.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else
            {
                lblMessage.Text = "The email address is not registered.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }


        private bool IsEmailRegistered(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) FROM Account WHERE account_email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }



        private bool SendResetEmail(string toEmail, string resetLink)
        {
            try
            {
                

                string fromEmail = "puajq-wm21@student.tarc.edu.my";
                string subject = "Password Reset Request";
                string body = $"<p>To reset your password, please click the following link:</p><p><a href='{resetLink}'>Reset Password</a></p>" +
                    $"<p>The Reset Password Link will be expired in 5 minutes</p>";

                // Set up and send the email
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, "mkhm ibkq tijr svds"),
                    EnableSsl = true
                };

                smtp.Send(mail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                lblMessage.Text = "SMTP error occurred: " + smtpEx.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("SMTP Error: " + smtpEx.ToString());
                return false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while sending the email: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("General Error: " + ex.ToString());
                return false;
            }
        }

        private bool SaveResetTokenToDatabase(string email, string resetToken, DateTime expiryTime)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "UPDATE Account SET reset_token = @Token, token_expiry = @Expiry WHERE account_email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", resetToken);
                    command.Parameters.AddWithValue("@Expiry",expiryTime); 
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

    }
}
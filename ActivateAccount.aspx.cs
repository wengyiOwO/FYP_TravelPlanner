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
    public partial class ActivateAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];

            if (!string.IsNullOrEmpty(token))
            {
                if (ActivateAccountByToken(token))
                {
                    lblMessage.Text = "Your account has been successfully activated. You can now log in.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblMessage.Text = "Invalid or expired activation link.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        private bool ActivateAccountByToken(string token)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "UPDATE Account SET account_status = 'Active', activate_token = NULL WHERE activate_token = @Token";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", token);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
 
        

        protected void btnSendEmail_Click(object sender, EventArgs e)
        {
            string userEmail = txtEmail.Text;

            if (IsEmailRegistered(userEmail))
            {
                string activeToken = Guid.NewGuid().ToString();

                // Store the token in the database
                if (SaveTokenToDatabase(userEmail, activeToken))
                {
                    // Send reset email with tokenized link
                    string activeLink = $"https://localhost:44387/ActivateAccount.aspx?token={activeToken}";
                    bool emailSent = SendActivationEmail(userEmail, activeLink);

                    if (emailSent)
                    {
                        lblMessage.Text = "An account activation link has been sent to your email.";
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



        private bool SendActivationEmail(string toEmail, string activeLink)
        {
            try
            {


                string fromEmail = "puajq-wm21@student.tarc.edu.my";
                string subject = "Account Activation Request";
                string body = $"<p>To active your account, please click the following link:</p><p><a href='{activeLink}'>Active Account</a></p>"
                    ;

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

       
        private bool SaveTokenToDatabase(string email, string activeToken)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "UPDATE Account SET activate_token = @Token WHERE account_email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Token", activeToken);
                    command.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
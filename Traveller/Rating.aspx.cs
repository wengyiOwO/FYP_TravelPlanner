using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace FYP_TravelPlanner.Traveller
{
    public partial class Rating : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session["account_id"] == null)
            //{
            //    Response.Redirect("~/Login.aspx");
            //}


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
            string query = "SELECT TOP 1 rating_id FROM Rating ORDER BY rating_id DESC";

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
                return "R000001";
            }

            int numericPart = int.Parse(lastID.Substring(2)) + 1;
            string nextID = $"R{numericPart:D6}";

            while (IDExistsInDatabase(nextID))
            {
                numericPart++;
                nextID = $"R{numericPart:D6}";
            }

            return nextID;
        }

        private bool IDExistsInDatabase(string idToCheck)
        {
            SqlConnection conn;
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            conn = new SqlConnection(strCon);
            conn.Open();

            string query = "SELECT COUNT(*) FROM Rating WHERE rating_id = @ID";
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@ID", idToCheck);

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ratingID = GenerateNextID();
            //string accountId = Session["account_id"]?.ToString();

            //if (accountId == null)
            //{
            //    lblMessage.Text = "Invalid account. Please log in again.";
            //    lblMessage.Visible = true;
            //    lblMessage.ForeColor = System.Drawing.Color.Red;
            //    return;
            //}

            // Get satisfaction rating and review values
            int satisfactionRating = GetSelectedRating("satisfaction_rating");
            string review = txtReview.Text;

            if (satisfactionRating == 0) // Check if satisfaction rating is selected
            {
                lblMessage.Text = "Please select a satisfaction rating.";
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if(review == "")
            {
                lblMessage.Text = "Failed to submit rating and review.";
                lblMessage.Visible = true;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            } 

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();

                string strInsert = "INSERT INTO Rating (rating_id, account_id, rating, rating_date, review) VALUES (@RatingID, @AccID, @Rating, @Date, @Review)";

                using (SqlCommand cmdInsert = new SqlCommand(strInsert, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@RatingID", ratingID);
                    cmdInsert.Parameters.AddWithValue("@AccID", "AC000524");
                    cmdInsert.Parameters.AddWithValue("@Rating", satisfactionRating);
                    cmdInsert.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmdInsert.Parameters.AddWithValue("@Review", review);

                    cmdInsert.ExecuteNonQuery();
                    conn.Close();
                    Response.Write("<script>alert('Your Rating and Review have been submitted successfully!'); window.location='Feedback.aspx';</script>");
            
               
                }
            }
        }


        private int GetSelectedRating(string inputName)
        {
            string ratingValue = Request.Form[inputName];
            return int.TryParse(ratingValue, out int rating) ? rating : 0;
        }
    }
}
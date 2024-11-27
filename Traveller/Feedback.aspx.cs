using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class Feedback : System.Web.UI.Page
    {
        protected double overallRating = 0.0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptFeedback.ItemDataBound += rptFeedback_ItemDataBound;
                BindFeedbackData();
            }

        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["account_id"] != null)
            {
                MasterPageFile = "~/TakeMyTrip.Master";
            }
            else
            {
                MasterPageFile = "~/TakeMyTrip_Anonymous.Master";
            }
        }
        protected void ratingFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFeedbackData();
            DisplayOverallRating();

        }
        protected void rptFeedback_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var imgFeedback = (Image)e.Item.FindControl("imgFeedback");

                string accountId = DataBinder.Eval(e.Item.DataItem, "account_id") as string;
                string profileImage = GetProfileImage(accountId);
                    imgFeedback.ImageUrl = string.IsNullOrEmpty(profileImage)
                        ? "~/Uploads/Profile/unknown.jpg"
                        : "~/Uploads/Profile/" + profileImage;
                }
            }
        

        private string GetProfileImage(string accountId)
        {
            string profileImage = null;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT profile_image FROM Account WHERE account_id = @AccountId";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@AccountId", accountId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    profileImage = result != DBNull.Value ? result.ToString() : null;
                }
            }

            return profileImage;
        }


        private void BindFeedbackData()
        {
            string accountRole = Session["account_role"] as string;
            string selectedRating = ratingFilter.SelectedValue;
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query;

            if (selectedRating == "all")
            {
                query = @"
            SELECT R.rating, R.review, R.rating_date, A.account_name, A.profile_image, R.account_id 
            FROM Rating R 
            INNER JOIN Account A ON R.account_id = A.account_id 
            ORDER BY R.rating_date DESC";
            }
            else
            {
                query = @"
            SELECT R.rating, R.review, R.rating_date, A.account_name, A.profile_image, R.account_id   
            FROM Rating R 
            INNER JOIN Account A ON R.account_id = A.account_id 
            WHERE R.rating = @rating 
            ORDER BY R.rating_date DESC";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                if (selectedRating != "all")
                {
                    cmd.Parameters.AddWithValue("@rating", int.Parse(selectedRating));
                }

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                // Modify account_name based on role
                if (accountRole != "Admin")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        row["account_name"] = "***";
                    }
                }

                if (dt.Rows.Count == 0)
                {
                    lblMessage.Visible = true;
                    rptFeedback.Visible = false;
                }
                else
                {
                    lblMessage.Visible = false;
                    rptFeedback.Visible = true;
                    rptFeedback.DataSource = dt;
                    rptFeedback.DataBind();
                }
            }

        }



        private void DisplayOverallRating()
        {
            // Calculate overall rating
            string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT AVG(CAST(rating AS FLOAT)) AS AverageRating FROM Rating";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                object result = cmd.ExecuteScalar();
                overallRating = result != DBNull.Value ? Convert.ToDouble(result) : 0.0;
            }

            // Display overall rating
            lblOverallRating.Text = $"Overall Rating: {overallRating:F1} / 5";
            overallRatingStars.InnerHtml = GenerateStars((int)Math.Round(overallRating));
        }
        protected string GenerateStars(int rating)
        {
            string starsHtml = "";
            for (int i = 1; i <= rating; i++)
            {
                starsHtml += "<i class='fas fa-star'></i>";
            }
            return starsHtml;
        }
    }
}
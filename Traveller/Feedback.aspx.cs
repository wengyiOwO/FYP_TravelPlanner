using System;
using System.Collections.Generic;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindFeedbackData();
            }
        }

        protected void ratingFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindFeedbackData();
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
            SELECT R.rating, R.review, R.rating_date, A.account_name 
            FROM Rating R 
            INNER JOIN Account A ON R.account_id = A.account_id 
            ORDER BY R.rating_date DESC";
            }
            else
            {
                query = @"
            SELECT R.rating, R.review, R.rating_date, A.account_name 
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
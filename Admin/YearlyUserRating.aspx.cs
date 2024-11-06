using Newtonsoft.Json;
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
    public partial class YearlyUserRating : System.Web.UI.Page
    {
        protected string RatingDataJson = "[]"; 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRatingData(DateTime.Now.Year);
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            // Load data for the selected month and year
            LoadRatingData(selectedYear);
        }

        private void LoadRatingData(int year)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int[] ratingsCount = new int[5];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT rating, COUNT(*) 
                    FROM Rating 
                    WHERE YEAR(rating_date) = @Year
                    GROUP BY rating";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rating = reader.GetInt32(0);
                            int count = reader.GetInt32(1);
                            ratingsCount[rating - 1] = count;
                        }
                    }
                }

                // Convert ratingsCount to JSON
                RatingDataJson = JsonConvert.SerializeObject(ratingsCount);
            }

            // Load summary information (total, avg, 5-star percentage)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string summaryQuery = @"
                    SELECT COUNT(*) AS TotalCount,
                           AVG(CAST(Rating AS FLOAT)) AS AvgRating,
                           SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS FiveStarPercent
                    FROM Rating   
                    WHERE YEAR(rating_date) = @Year";

                using (SqlCommand cmd = new SqlCommand(summaryQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalRating.Text = reader["TotalCount"].ToString();

                        // Check for DBNull before converting AvgRating
                        lblAvgRating.Text = reader["AvgRating"] != DBNull.Value
                            ? Convert.ToDouble(reader["AvgRating"]).ToString("0.0")
                            : "0.0";

                        // Check for DBNull before converting FiveStarPercent
                        lblPercentage.Text = reader["FiveStarPercent"] != DBNull.Value
                            ? Convert.ToDouble(reader["FiveStarPercent"]).ToString("0.0") + "%"
                            : "0.0%";
                    }

                }
            }
        }
    }
}
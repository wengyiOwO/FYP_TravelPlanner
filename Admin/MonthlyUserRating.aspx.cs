using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Newtonsoft.Json;

namespace FYP_TravelPlanner
{
    public partial class MonthlyUserRating : Page
    {
        protected string RatingDataJson = "[]"; // JSON string for chart data

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRatingData(DateTime.Now.Month, DateTime.Now.Year);
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(DropDownList1.SelectedValue);
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            // Load data for the selected month and year
            LoadRatingData(selectedMonth, selectedYear);
        }

        private void LoadRatingData(int month, int year)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int[] ratingsCount = new int[5];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT rating, COUNT(*) 
                    FROM Rating 
                    WHERE MONTH(rating_date) = @Month AND YEAR(rating_date) = @Year
                    GROUP BY rating";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
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
                    WHERE MONTH(rating_date) = @Month AND YEAR(rating_date) = @Year";

                using (SqlCommand cmd = new SqlCommand(summaryQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Month", month);
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

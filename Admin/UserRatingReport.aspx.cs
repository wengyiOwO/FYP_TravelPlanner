using System;
using System.Configuration;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
using Newtonsoft.Json;


namespace FYP_TravelPlanner
{
    public partial class UserRatingReport : System.Web.UI.Page
    {
        protected string RatingDataJson = "[]"; 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRatingData();
                LoadRatingData();

             
            }
        }

        private void GetRatingData()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int[] ratingsCount = new int[5];

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT rating, COUNT(*) FROM Rating GROUP BY rating";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
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
            }

            // Convert the ratings data array to a JavaScript array string
            RatingDataJson = JsonConvert.SerializeObject(ratingsCount);
            System.Diagnostics.Debug.WriteLine("RatingDataJson: " + RatingDataJson);

        }

        private void LoadRatingData()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) AS TotalCount,
                           AVG(CAST(Rating AS FLOAT)) AS AvgRating,
                           SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS FiveStarPercent
                    FROM Rating";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Populate the labels with data
                        lblTotalRating.Text = reader["TotalCount"].ToString();
                        lblAvgRating.Text = Convert.ToDouble(reader["AvgRating"]).ToString("0.0");
                        lblPercentage.Text = Convert.ToDouble(reader["FiveStarPercent"]).ToString("0.0") + "%";
                    }
                }
            }
        }

    

    }

}
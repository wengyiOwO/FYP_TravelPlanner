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
    public partial class Dashboard : System.Web.UI.Page
    {
        protected string RatingDataJson = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var data = GetPopularDestinations();
                var jsonData = JsonConvert.SerializeObject(data);
                // Register JSON data to populate the chart on page load
                ClientScript.RegisterStartupScript(this.GetType(), "populateChart", $"populateChart({jsonData});", true);
                GetRatingData();
                GetAvgRating();
                GetMonthlyTravelPlan(DateTime.Now.Month);
                GetAnnualTravelPlan(DateTime.Now.Year);
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
        private void GetAvgRating()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT AVG(CAST(Rating AS FLOAT)) AS AvgRating
                    FROM Rating";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblAvgRating.Text = Convert.ToDouble(reader["AvgRating"]).ToString("0.0");
                    }
                }
            }
        }

        private void GetMonthlyTravelPlan(int month)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) AS TotalCount
                         FROM Travel_Plan
                WHERE MONTH(plan_date) = @MONTH";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Month", month);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblMonthlyTP.Text = Convert.ToInt32(reader["TotalCount"]).ToString();
                    }
                }
            }
        }
        private void GetAnnualTravelPlan(int year)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) AS TotalCount
                         FROM Travel_Plan
                WHERE YEAR(plan_date) = @YEAR";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@YEAR", year);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblAnnualTP.Text = Convert.ToInt32(reader["TotalCount"]).ToString();
                    }
                }
            }
        }
        private List<PopularDestination> GetPopularDestinations()
        {
            var popularDestinations = new List<PopularDestination>();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = @"
                SELECT TOP 8
                    l.location_id,
                    l.place_name AS location_name,
                    COUNT(tp.plan_id) AS visit_count
                FROM 
                    Travel_Plan tp
                JOIN 
                    Area a ON tp.area_id = a.area_id
                JOIN 
                    Location l ON l.area_id = a.area_id
                GROUP BY 
                    l.location_id, l.place_name
                ORDER BY 
                    visit_count DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        popularDestinations.Add(new PopularDestination
                        {
                            Name = reader["location_name"].ToString(),
                            Visits = Convert.ToInt32(reader["visit_count"])
                        });
                    }
                }
            }

            return popularDestinations;
        }

        public class PopularDestination
        {
            public string Name { get; set; }
            public int Visits { get; set; }
        }

    }
}
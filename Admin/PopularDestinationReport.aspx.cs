using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace FYP_TravelPlanner
{
    public partial class PopularDestinationReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Fetch popular destination data
                var data = GetPopularDestinations();
                // Serialize data to JSON for JavaScript
                var jsonData = JsonConvert.SerializeObject(data);
                // Register JSON data to populate the chart on page load
                ClientScript.RegisterStartupScript(this.GetType(), "populateChart", $"populateChart({jsonData});", true);
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

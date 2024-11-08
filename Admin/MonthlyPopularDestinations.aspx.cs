using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.js.demo
{
    public partial class MonthlyPopularDestinations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Get the current month and year
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Fetch popular destinations for the current month and year
                var initialData = GetPopularDestinations(currentMonth, currentYear);
                var jsonData = JsonConvert.SerializeObject(initialData);

                // Register the initial data as a JavaScript function call to populateChart
                ScriptManager.RegisterStartupScript(this, GetType(), "populateChart", $"populateChart({jsonData});", true);
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedMonth = int.Parse(DropDownList1.SelectedValue);
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            var selectedData = GetPopularDestinations(selectedMonth, selectedYear);
            var jsonData = JsonConvert.SerializeObject(selectedData);

            // Register the updated data as a JavaScript function call to populateChart
            ScriptManager.RegisterStartupScript(this, GetType(), "populateChart", $"populateChart({jsonData});", true);
        }

        private List<PopularDestination> GetPopularDestinations(int month, int year)
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
                WHERE 
                    MONTH(plan_date) = @Month AND YEAR(plan_date) = @Year

                GROUP BY 
                    l.location_id, l.place_name
                ORDER BY 
                    visit_count DESC;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.Parameters.AddWithValue("@Month", month);
                command.Parameters.AddWithValue("@Year", year);

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

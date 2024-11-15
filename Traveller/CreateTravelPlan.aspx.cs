using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Configuration;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;


namespace FYP_TravelPlanner.Traveller
{
    public partial class CreateTravelPlan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateAreaDropdown();
            }
        }

        private void PopulateAreaDropdown()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT area_id, area_name FROM Area";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlState.DataSource = reader;
                    ddlState.DataTextField = "area_name";
                    ddlState.DataValueField = "area_id";
                    ddlState.DataBind();
                }
            }

            ddlState.Items.Insert(0, new ListItem("--Select an Area--", ""));
        }

        protected void calendarStartDate_SelectionChanged(object sender, EventArgs e)
        {
            // Get the selected date
            DateTime selectedDate = calendarStartDate.SelectedDate;

            // Set the TextBox with the selected date
            txtStartDate.Text = selectedDate.ToString("dd-MM-yyyy");
        }

        protected void btnPlan_Click(object sender, EventArgs e)
        {
            string selectedAreaId = ddlState.SelectedValue;
            if (string.IsNullOrEmpty(selectedAreaId))
            {
                testError.Text = "Please select an area.";
                return;
            }

            DateTime startDate;
            if (!DateTime.TryParse(txtStartDate.Text, out startDate))
            {
                testError.Text = "Please select a valid start date.";
                return;
            }

            int duration = int.Parse(ddlDuration.SelectedValue);
            int budget = int.Parse(rblBudget.SelectedValue);
            var selectedInterests = cblActivities.Items.Cast<ListItem>()
    .Where(item => item.Selected)
    .Select(item => item.Value)
    .ToList();

            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Location> locations = new List<Location>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT location_id, place_name, latitude, longitude FROM Location WHERE area_id = @areaId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@areaId", selectedAreaId);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            locations.Add(new Location
                            {
                                id = reader["location_id"].ToString(),
                                name = reader["place_name"].ToString(),
                                address = string.Empty,
                                lat = Convert.ToDouble(reader["latitude"]),
                                lng = Convert.ToDouble(reader["longitude"])
                            });
                        }
                    }
                }
                List<Location> filteredLocations = FilterLocationsByInterest(locations, selectedInterests);
                // Determine the number of locations based on the budget
                int locationCount;
                switch (budget)
                {
                    case 500:
                        locationCount = 4;
                        break;
                    case 1000:
                        locationCount = 8;
                        break;
                    case 1500:
                        locationCount = 12;
                        break;
                    case 2000:
                        locationCount = 16;
                        break;
                    default:
                        locationCount = 4;
                        break;
                }


                // If filtered locations are less than needed, add random locations to meet the required count
                var random = new Random();
                if (filteredLocations.Count < locationCount)
                {
                    // Add random locations from the unfiltered list, excluding duplicates
                    var additionalLocations = locations.Except(filteredLocations)
                                                       .OrderBy(x => random.Next())
                                                       .Take(locationCount - filteredLocations.Count)
                                                       .ToList();
                    filteredLocations.AddRange(additionalLocations);
                }

                // Limit to the number of locations based on the budget
                var selectedLocations = filteredLocations.Take(locationCount).ToList();

                // Sort locations by latitude for easier routing
                selectedLocations = selectedLocations.OrderBy(l => l.lat).ToList();

                // Divide the locations by duration (days)
                int locationsPerDay = locationCount / duration;
                for (int i = 0; i < selectedLocations.Count; i++)
                {
                    selectedLocations[i].day = (i / locationsPerDay) + 1;
                }

                // Store the travel plan with day-wise locations in the session
                Session["SelectedLocations"] = JsonConvert.SerializeObject(selectedLocations);
                Session["AreaID"] = selectedAreaId;
                Session["StartDate"] = startDate;
                Session["Duration"] = duration;
                Session["Budget"] = budget;
                Response.Redirect("~/Traveller/TravelPlan.aspx");
            }
            catch (Exception ex)
            {
                testError.Text = $"Error: {ex.Message}";
            }
        }

        // Method to filter locations by activity interest keywords
        private List<Location> FilterLocationsByInterest(List<Location> locations, List<string> selectedInterests)
        {
            var filteredLocations = new List<Location>();

            // Keywords for each category
            var interestKeywords = new Dictionary<string, List<string>>
    {
        { "beaches", new List<string> { "Pantai", "Island", "Beach", "Pulau" } },
        { "citySightseeing", new List<string> { "Heritage", "Museum", "City", "Historical", "Monument", "Tower" } },
        { "foodExploration", new List<string> { "Food", "Restaurant", "Cafe", "Market", "Cuisine", "Street Food" } },
        { "shopping", new List<string> { "Mall", "Shopping", "Market", "Bazaar", "Souvenir" } },
        { "outdoorAdventures", new List<string> { "Taman", "Park", "Theme Park", "Escape Park", "Waterfall", "Nature", "Hill", "Hiking" } }
    };

            foreach (var location in locations)
            {
                bool matchesInterest = false;

                // Check if location name contains any keyword for each selected interest
                foreach (var interest in selectedInterests)
                {
                    if (interestKeywords.ContainsKey(interest))
                    {
                        var keywords = interestKeywords[interest];
                        if (keywords.Any(keyword => location.name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            matchesInterest = true;
                            break;
                        }
                    }
                }

                if (matchesInterest)
                    filteredLocations.Add(location);
            }

            // If no interests are selected, return all locations
            return filteredLocations.Count > 0 ? filteredLocations : locations;
        }

        public class Location
        {
            public string id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public int day { get; set; } 
        }
    }
}
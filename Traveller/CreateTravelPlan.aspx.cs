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
            DateTime selectedDate = calendarStartDate.SelectedDate;

            txtStartDate.Text = selectedDate.ToString("dd-MM-yyyy");
        }

        protected void ValidateStartDate(object source, ServerValidateEventArgs args)
        {
            if (DateTime.TryParse(args.Value, out DateTime selectedDate))
            {
                args.IsValid = selectedDate >= DateTime.Today;
            }
            else
            {
                args.IsValid = false;
            }
        }


        protected void CheckBoxRequired_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = cblActivities.SelectedIndex != -1;
        }

        protected void btnPlan_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {

                string locationJson = Request.Form[hfSelectedLocations.UniqueID];
               
                var frontendLocations = JsonConvert.DeserializeObject<List<Location>>(locationJson);

                List<Location> mustLocations = GetMustLocations(frontendLocations);

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



                //string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                //List<Location> locations = new List<Location>();
                List<Location> locations = GetAllLocations(mustLocations, selectedAreaId);

                try
                {
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

                    List<Location> allSelectedLocations = mustLocations.Concat(filteredLocations).ToList();


                    var random = new Random();
                    if (allSelectedLocations.Count < locationCount)
                    {
                        var additionalLocations = locations.Except(allSelectedLocations)
                                                           .OrderBy(x => random.Next())
                                                           .Take(locationCount - allSelectedLocations.Count)
                                                           .ToList();
                        allSelectedLocations.AddRange(additionalLocations);
                    }

                    var finalSelectedLocations = allSelectedLocations.Take(locationCount).ToList();

                    finalSelectedLocations = finalSelectedLocations.OrderBy(l => l.lat).ToList();

                    int locationsPerDay = locationCount / duration;
                    for (int i = 0; i < finalSelectedLocations.Count; i++)
                    {
                        finalSelectedLocations[i].day = (i / locationsPerDay) + 1;
                    }

                    Session["SelectedLocations"] = JsonConvert.SerializeObject(finalSelectedLocations);
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
            else
            {
                testError.Text = "Please correct the highlighted errors.";
            }
        }

        private List<Location> GetMustLocations(List<Location> frontendLocations)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Location> mustLocations = new List<Location>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                foreach (var loc in frontendLocations)
                {
                    string selectQuery = "SELECT location_id, latitude, longitude FROM Location WHERE place_name LIKE @placeName";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@placeName", loc.name);
                        SqlDataReader reader = selectCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            mustLocations.Add(new Location
                            {
                                id = reader["location_id"].ToString(),
                                name = loc.name,
                                address = loc.address,
                                lat = Convert.ToDouble(reader["latitude"]),
                                lng = Convert.ToDouble(reader["longitude"])
                            });
                            reader.Close();
                        }
                        else
                        {
                            reader.Close();

                            string maxIdQuery = "SELECT ISNULL(MAX(location_id), 'P0000000') FROM Location";
                            using (SqlCommand maxIdCmd = new SqlCommand(maxIdQuery, conn))
                            {
                                string maxId = maxIdCmd.ExecuteScalar().ToString();
                                string newId = "P" + (int.Parse(maxId.Substring(1)) + 1).ToString("D7");

                                string insertQuery = "INSERT INTO Location (location_id, place_name, area_id, latitude, longitude) VALUES (@id, @name, @areaId, @lat, @lng)";
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@id", newId);
                                    insertCmd.Parameters.AddWithValue("@name", loc.name);
                                    insertCmd.Parameters.AddWithValue("@areaId", loc.area_id); 
                                    insertCmd.Parameters.AddWithValue("@lat", loc.lat);
                                    insertCmd.Parameters.AddWithValue("@lng", loc.lng);

                                    insertCmd.ExecuteNonQuery();
                                }

                                mustLocations.Add(new Location
                                {
                                    id = newId,
                                    name = loc.name,
                                    address = loc.address,
                                    lat = loc.lat,
                                    lng = loc.lng
                                });
                            }
                        }
                    }
                }
            }

            return mustLocations;
        }

        private List<Location> GetAllLocations(List<Location> mustLocations, string selectedAreaId)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<Location> locations = new List<Location>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var mustLocationIds = mustLocations.Select(loc => loc.id).ToList();

                string query = "SELECT location_id, place_name, area_id, latitude, longitude FROM Location WHERE area_id = @areaId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@areaId", selectedAreaId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string locationId = reader["location_id"].ToString();

                        if (!mustLocationIds.Contains(locationId))
                        {
                            locations.Add(new Location
                            {
                                id = locationId,
                                name = reader["place_name"].ToString(),
                                lat = Convert.ToDouble(reader["latitude"]),
                                lng = Convert.ToDouble(reader["longitude"])
                            });
                        }
                    }
                }
            }

            return locations;
        }



        private List<Location> FilterLocationsByInterest(List<Location> locations, List<string> selectedInterests)
        {
            var filteredLocations = new List<Location>();

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

            public string area { get; set; }
            public string area_id { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public int day { get; set; }
        }
    }
}
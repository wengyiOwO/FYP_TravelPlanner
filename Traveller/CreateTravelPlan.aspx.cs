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

            // Retrieve the start date and duration
            int duration = int.Parse(ddlDuration.SelectedValue);
            int budget = int.Parse(rblBudget.SelectedValue);
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

                // Randomly select locations and distribute them over the travel days
                var random = new Random();
                var selectedLocations = locations.OrderBy(x => random.Next()).Take(duration * 3).ToList();
                int locationsPerDay = 3;

                // Assign locations to each travel day
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
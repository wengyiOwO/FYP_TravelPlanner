using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class MapMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string csvFilePath = Server.MapPath("JohorPerak.csv");
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<LocationData> locations = new List<LocationData>();

            // Read CSV and filter for State = "Johor"
            var lines = File.ReadAllLines(csvFilePath);

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var columns = line.Split(',');

                if (columns[2].Trim().Equals("Perak", StringComparison.OrdinalIgnoreCase))
                {
                    // Add to list of locations
                    locations.Add(new LocationData
                    {
                        Location = columns[1].Trim(),
                        State = columns[2].Trim(),
                        Latitude = double.Parse(columns[3].Trim(), CultureInfo.InvariantCulture),
                        Longitude = double.Parse(columns[4].Trim(), CultureInfo.InvariantCulture)
                    });
                }
            }

            // Insert into the database
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                int locationCounter = 57;

                foreach (var location in locations)
                {
                    string locationId = $"P{locationCounter:D7}"; // Generates IDs like P0000001, P0000002, etc.
                    string areaId = location.State == "Perak" ? "A0000001" : "A0000002"; // Assign based on state

                    string query = "INSERT INTO Location (location_id, area_id, place_name, latitude, longitude) " +
                                   "VALUES (@locationId, @areaId, @place_name, @latitude, @longitude)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@locationId", locationId);
                        cmd.Parameters.AddWithValue("@areaId", areaId);
                        cmd.Parameters.AddWithValue("@place_name", location.Location);
                        cmd.Parameters.AddWithValue("@latitude", location.Latitude);
                        cmd.Parameters.AddWithValue("@longitude", location.Longitude);

                        cmd.ExecuteNonQuery();
                    }

                    locationCounter++;
                }
            }
        }

        public class LocationData
        {
            public string Location { get; set; }
            public string State { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        //public class CSVToDatabaseInserter
        //{
        //    string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //    public void InsertDataFromCSV(string csvFilePath)
        //    {
        //        List<LocationData> locations = new List<LocationData>();

        //        // Read CSV and filter for State = "Johor"
        //        var lines = File.ReadAllLines(csvFilePath);

        //        foreach (var line in lines.Skip(1)) // Skip header
        //        {
        //            var columns = line.Split(',');

        //            if (columns[2].Trim().Equals("Johor", StringComparison.OrdinalIgnoreCase))
        //            {
        //                // Add to list of locations
        //                locations.Add(new LocationData
        //                {
        //                    Location = columns[1].Trim(),
        //                    State = columns[2].Trim(),
        //                    Latitude = double.Parse(columns[3].Trim(), CultureInfo.InvariantCulture),
        //                    Longitude = double.Parse(columns[4].Trim(), CultureInfo.InvariantCulture)
        //                });
        //            }
        //        }

        //        // Insert into the database
        //        using (SqlConnection conn = new SqlConnection(ConnectionString))
        //        {
        //            conn.Open();
        //            int locationCounter = 1;

        //            foreach (var location in locations)
        //            {
        //                string locationId = $"area-id-{locationCounter}";
        //                string areaId = location.State == "Perak" ? "A0000001" : "A0000002"; // Assign based on state

        //                string query = "INSERT INTO Location (location_id, area_id, latitude, longitude) " +
        //                               "VALUES (@locationId, @areaId, @latitude, @longitude)";

        //                using (SqlCommand cmd = new SqlCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@locationId", locationId);
        //                    cmd.Parameters.AddWithValue("@areaId", areaId);
        //                    cmd.Parameters.AddWithValue("@latitude", location.Latitude);
        //                    cmd.Parameters.AddWithValue("@longitude", location.Longitude);

        //                    cmd.ExecuteNonQuery();
        //                }

        //                locationCounter++;
        //            }
        //        }
        //    }
        //}
    }
}
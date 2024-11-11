using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace FYP_TravelPlanner
{
    public partial class PopularDestinationReport : System.Web.UI.Page
    {
        protected string DestinationDataJson = "[]"; // JSON for chart data

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DestinationDataJson = GetTopDestinationsJson();
                ClientScript.RegisterStartupScript(this.GetType(), "setDestinationData", $"var DestinationDataJson = {DestinationDataJson};", true);
            }
        }

        private string GetTopDestinationsJson()
        {
            DataTable destinations = new DataTable();
            destinations.Columns.Add("location_name", typeof(string));
            destinations.Columns.Add("visit_count", typeof(int));

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = @"
                SELECT TOP 8
                    l.place_name AS location_name,
                    COUNT(tp.plan_id) AS visit_count
                FROM 
                    Travel_Plan tp
                JOIN 
                    Area a ON tp.area_id = a.area_id
                JOIN 
                    Location l ON l.area_id = a.area_id
                GROUP BY 
                    l.place_name
                ORDER BY 
                    visit_count DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        destinations.Load(reader);
                    }
                }
            }

            return JsonConvert.SerializeObject(destinations);
        }
    }
}

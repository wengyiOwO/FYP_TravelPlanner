using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;

namespace FYP_TravelPlanner.Traveller
{
    public partial class TravelPlan : System.Web.UI.Page
    {
        protected string LocationsJson
        {
            get { return ViewState["LocationsJson"] as string ?? "[]"; }
            set { ViewState["LocationsJson"] = value; }
        }
        protected string AllLocationsJson { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            string planId = Request.QueryString["tp"];

            if (!IsPostBack)
            {
                // Fetch and load all locations for nearby search functionality
                LoadAllLocations();

                if (!string.IsNullOrEmpty(planId))
                {
                    // Load the travel plan details from the database
                    LoadTravelPlanData(planId);


                }
                else if (Session["SelectedLocations"] != null)
                {
                    // Deserialize or keep data in session if it's already List<Location>
                    if (Session["SelectedLocations"] is string selectedLocationsJson)
                    {
                        Session["SelectedLocations"] = JsonConvert.DeserializeObject<List<Location>>(selectedLocationsJson);
                    }
                }
                else
                {
                    Response.Redirect("~/Login.aspx");
                    //LocationsJson = "[]"; // Initialize as empty if session is null
                }
            }

            // Ensure data is serialized for JavaScript on every load
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LocationsJson = serializer.Serialize((List<Location>)Session["SelectedLocations"]);
            //List<Location> allLocations = Session["AllLocations"] as List<Location>;
            AllLocationsJson = serializer.Serialize((List<Location>)Session["AllLocations"]);
        }

        private void LoadAllLocations()
        {
            List<Location> allLocations = new List<Location>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"SELECT location_id, place_name, place_address, latitude, longitude 
                         FROM Location";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Location location = new Location
                            {
                                id = reader["location_id"].ToString(),
                                name = reader["place_name"].ToString(),
                                address = reader["place_address"].ToString(),
                                lat = Convert.ToDouble(reader["latitude"]),
                                lng = Convert.ToDouble(reader["longitude"])
                            };
                            allLocations.Add(location);
                        }
                    }
                }
            }

            // Store all locations in session
            Session["AllLocations"] = allLocations;
        }

        private void LoadTravelPlanData(string planId)
        {
            List<Location> locations = new List<Location>();
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
            SELECT loc.location_id, loc.place_name, loc.place_address, loc.latitude, loc.longitude, di.day_number
            FROM Travel_Plan tp
            INNER JOIN Daily_Itinerary di ON tp.plan_id = di.plan_id
            INNER JOIN Travel_Activity ta ON di.itinerary_id = ta.itinerary_id
            INNER JOIN Location loc ON ta.location_id = loc.location_id
            WHERE tp.plan_id = @plan_id
            ORDER BY di.day_number";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@plan_id", planId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Location location = new Location
                            {
                                id = reader["location_id"].ToString(),
                                name = reader["place_name"].ToString(),
                                address = reader["place_address"].ToString(),
                                lat = Convert.ToDouble(reader["latitude"]),
                                lng = Convert.ToDouble(reader["longitude"]),
                                day = Convert.ToInt32(reader["day_number"])
                            };
                            locations.Add(location);
                        }
                    }
                }
            }

            // Store locations in session and prepare JSON for JavaScript
            Session["SelectedLocations"] = locations;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            LocationsJson = serializer.Serialize(locations);
        }
        private bool TravelPlanExists(string areaId, DateTime planDate)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"SELECT COUNT(*) FROM Travel_Plan WHERE area_id = @area_id AND plan_date = @plan_date";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@area_id", areaId);
                    cmd.Parameters.AddWithValue("@plan_date", planDate);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                var locations = (List<Location>)Session["SelectedLocations"];
                Session["LocationsToSave"] = JsonConvert.SerializeObject(locations);
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Session["SavePlan"] != null && Session["SavePlan"].ToString() == "saved")
            {
                Response.Write("<script>alert('Travel Plan Saved Successfully!'); window.location='Rating.aspx';</script>");
                return;
            }
            string accountId = Session["account_id"].ToString();
            
            string existingPlanId = Request.QueryString["tp"];
            string planId,ownerId, areaId;
            DateTime startDate;
            int duration, budget;

            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                //check tp exist
                if (!string.IsNullOrEmpty(existingPlanId))
                {

                    // Delete associated activities and itineraries for the provided plan_id
                    string deleteActivitiesQuery = @"DELETE FROM Travel_Activity 
                                             WHERE itinerary_id IN 
                                             (SELECT itinerary_id FROM Daily_Itinerary WHERE plan_id = @plan_id)";
                    string deleteItinerariesQuery = @"DELETE FROM Daily_Itinerary WHERE plan_id = @plan_id";

                    using (SqlCommand cmd = new SqlCommand(deleteActivitiesQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", existingPlanId);
                        cmd.ExecuteNonQuery();
                    }
                    using (SqlCommand cmd = new SqlCommand(deleteItinerariesQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", existingPlanId);
                        cmd.ExecuteNonQuery();
                    }

                    // Use the existing plan_id
                    string planQuery = @"SELECT account_id, area_id, plan_date, duration, budget 
                         FROM Travel_Plan 
                         WHERE plan_id = @plan_id";
                    using (SqlCommand cmd = new SqlCommand(planQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", existingPlanId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ownerId = reader["account_id"].ToString();
                                areaId = reader["area_id"].ToString();
                                startDate = DateTime.Parse(reader["plan_date"].ToString());
                                duration = Convert.ToInt32(reader["duration"]);
                                budget = Convert.ToInt32(reader["budget"]);
                            }
                            else
                            {
                                throw new Exception("Travel Plan not found for the provided plan_id.");
                            }
                        }
                    }

                    planId = existingPlanId;


                }
                else
                {
                    // Generate a new plan_id if none exists
                    planId = GeneratePlanId();
                    areaId = Session["AreaID"].ToString();
                    startDate = DateTime.Parse(Session["StartDate"].ToString());
                    duration = Convert.ToInt32(Session["Duration"]);
                    budget = Convert.ToInt32(Session["Budget"]);
                    string travelPlanQuery = @"INSERT INTO Travel_Plan (plan_id, account_id, area_id, plan_date, duration, budget) 
                                   VALUES (@plan_id, @account_id, @area_id, @plan_date, @duration, @budget)";
                    using (SqlCommand cmd = new SqlCommand(travelPlanQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", planId);
                        cmd.Parameters.AddWithValue("@account_id", accountId);
                        cmd.Parameters.AddWithValue("@area_id", areaId);
                        cmd.Parameters.AddWithValue("@plan_date", startDate);
                        cmd.Parameters.AddWithValue("@duration", duration);
                        cmd.Parameters.AddWithValue("@budget", budget);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Get the list of locations from session
                var selectedLocations = (List<Location>)Session["SelectedLocations"];

                // Insert each day into Daily_Itinerary and locations into Travel_Activity
                for (int day = 1; day <= duration; day++)
                {
                    string itineraryId = GenerateItineraryId();

                    // Insert into Daily_Itinerary table
                    string dailyItineraryQuery = @"INSERT INTO Daily_Itinerary (itinerary_id, plan_id, day_number)
                                           VALUES (@itinerary_id, @plan_id, @day_number)";
                    using (SqlCommand cmd = new SqlCommand(dailyItineraryQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@itinerary_id", itineraryId);
                        cmd.Parameters.AddWithValue("@plan_id", planId);
                        cmd.Parameters.AddWithValue("@day_number", day);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert associated locations for the current day into Travel_Activity
                    foreach (var location in selectedLocations.Where(loc => loc.day == day))
                    {
                        string activityId = GenerateActivityId();

                        string travelActivityQuery = @"INSERT INTO Travel_Activity (activity_id, itinerary_id, location_id) 
                                               VALUES (@activity_id, @itinerary_id, @location_id)";
                        using (SqlCommand cmd = new SqlCommand(travelActivityQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@activity_id", activityId);
                            cmd.Parameters.AddWithValue("@itinerary_id", itineraryId);
                            cmd.Parameters.AddWithValue("@location_id", location.id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                conn.Close();
            }

            Session["SavePlan"] = "saved";

            string email = !string.IsNullOrEmpty(Session["account_email"] as string)
                ? Session["account_email"] as string
                : "takemytrip2024@gmail.com";

            if (Session["NotifyEmailSent"] == null)
            {
                  SendNotifyEmail(email, planId);
                Session["NotifyEmailSent"] = true;
            }

            ScheduleItineraryEmails(planId, startDate);
            if (Session["SavePlan"] != null && Session["SavePlan"].ToString() == "saved")
            {
                Response.Write("<script>alert('Travel Plan Saved Successfully!'); window.location='Rating.aspx';</script>");
                return;
            }
        }


        private bool SendNotifyEmail(string toEmail, string planId)
        {
            try
            {
                string fromEmail = "puajq-wm21@student.tarc.edu.my";
                string subject = "New Travel Plan is Ready!";
                string accountName = "";
                string areaName = "";
                DateTime planDate = DateTime.Now;
                int duration = 0;
                int budget = 0;

                // Retrieve travel plan details from the database
                string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT acc.account_name, area.area_name, tp.plan_date, tp.duration, tp.budget
                FROM Travel_Plan tp
                INNER JOIN Account acc ON tp.account_id = acc.account_id
                INNER JOIN Area area ON tp.area_id = area.area_id
                WHERE tp.plan_id = @plan_id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", planId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                accountName = reader["account_name"].ToString();
                                areaName = reader["area_name"].ToString();
                                planDate = Convert.ToDateTime(reader["plan_date"]);
                                duration = Convert.ToInt32(reader["duration"]);
                                budget = Convert.ToInt32(reader["budget"]);

                            }
                        }
                    }
                }

                string itineraryQuery = @"
            SELECT di.day_number, loc.place_name
            FROM Daily_Itinerary di
            INNER JOIN Travel_Activity ta ON di.itinerary_id = ta.itinerary_id
            INNER JOIN Location loc ON ta.location_id = loc.location_id
            WHERE di.plan_id = @plan_id
            ORDER BY di.day_number";

                string itineraryDetails = "";

                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand itineraryCmd = new SqlCommand(itineraryQuery, conn))
                    {
                        itineraryCmd.Parameters.AddWithValue("@plan_id", planId);

                        using (SqlDataReader reader = itineraryCmd.ExecuteReader())
                        {
                            int currentDay = 0;
                            while (reader.Read())
                            {
                                int dayNumber = Convert.ToInt32(reader["day_number"]);
                                string placeName = reader["place_name"].ToString();

                                // Check if a new day has started to structure the output
                                if (dayNumber != currentDay)
                                {
                                    if (currentDay != 0) itineraryDetails += "</ul>";
                                    itineraryDetails += $"<p><b>Day {dayNumber}</b>:</p><ul>";
                                    currentDay = dayNumber;
                                }

                                itineraryDetails += $"<li>{placeName}</li>";
                            }

                            if (currentDay != 0) itineraryDetails += "</ul>";
                        }
                    }
                }

                // Construct the email body with the retrieved details
                string body = $@"
            <p><b>Dear {accountName}</b>,</p>
            <p>We're excited to inform you that your new travel plan has been successfully generated! Here are the details of your upcoming adventure:</p>
            <p><b>Travel Plan Summary:</b></p>
            <p><b>1. Destination</b>: {areaName}</p>
            <p><b>2. Date</b>: {planDate.ToString("yyyy-MM-dd")}</p>
            <p><b>3. Duration</b>: {duration} day(s)</p>
            <p><b>4. Budget</b>: RM {budget} </p>

            <p><b>Itinerary Highlights:</b></p>           
            {itineraryDetails}
            <p>You can view the full details of your travel plan by logging into your account on the Take My Trip website. Don't forget to check your packing list, and make sure that you have all your travel documents ready.</p>
            <p>Safe Travels,</p>
            <p><b>Take My Trip</b></p>";

                // Set up and send the email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, "mkhm ibkq tijr svds"),
                    EnableSsl = true
                };

                smtp.Send(mail);
                return true;
            }
            catch (SmtpException smtpEx)
            {
                lblMessage.Text = "SMTP error occurred: " + smtpEx.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("SMTP Error: " + smtpEx.ToString());
                return false;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while sending the email: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                Console.WriteLine("General Error: " + ex.ToString());
                return false;
            }
        }

        private void ScheduleItineraryEmails(string planId, DateTime startDate)
        {
            DateTime today = DateTime.Today;

            if (startDate.AddDays(-1) == today)
            {
                if (Session["ItineraryEmail_1DayBefore"] == null)
                {
                    SendDailyItineraryEmail(planId, 1);
                    Session["ItineraryEmail_1DayBefore"] = true;
                }
            }

        }

        private bool SendDailyItineraryEmail(string planId, int dayNumber)
        {
            try
            {
                string fromEmail = "puajq-wm21@student.tarc.edu.my";
                string toEmail;

                if (!string.IsNullOrEmpty(Session["account_email"] as string))
                {
                    toEmail = Session["account_email"] as string;
                }
                else
                {
                    toEmail = "takemytrip2024@gmail.com";
                }
                string subject = $"Day {dayNumber} Itinerary Reminder for Your Travel Plan!";

                string accountName = "";
                string areaName = "";
                DateTime planDate;
                int duration;
                string itineraryDetails = "";

                string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Get account and plan details
                    string planQuery = @"
                SELECT acc.account_name, area.area_name, tp.plan_date, tp.duration
                FROM Travel_Plan tp
                INNER JOIN Account acc ON tp.account_id = acc.account_id
                INNER JOIN Area area ON tp.area_id = area.area_id
                WHERE tp.plan_id = @plan_id";

                    using (SqlCommand cmd = new SqlCommand(planQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@plan_id", planId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                accountName = reader["account_name"].ToString();
                                areaName = reader["area_name"].ToString();
                                planDate = Convert.ToDateTime(reader["plan_date"]);
                                duration = Convert.ToInt32(reader["duration"]);
                            }
                        }
                    }

                    // Retrieve day-specific itinerary details
                    string itineraryQuery = @"
                SELECT loc.place_name
                FROM Daily_Itinerary di
                INNER JOIN Travel_Activity ta ON di.itinerary_id = ta.itinerary_id
                INNER JOIN Location loc ON ta.location_id = loc.location_id
                WHERE di.plan_id = @plan_id AND di.day_number = @day_number";

                    itineraryDetails = $"<p><b>Day {dayNumber} Itinerary:</b></p><ul>";

                    using (SqlCommand itineraryCmd = new SqlCommand(itineraryQuery, conn))
                    {
                        itineraryCmd.Parameters.AddWithValue("@plan_id", planId);
                        itineraryCmd.Parameters.AddWithValue("@day_number", dayNumber);

                        using (SqlDataReader reader = itineraryCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string placeName = reader["place_name"].ToString();
                                itineraryDetails += $"<li>{placeName}</li>";
                            }
                        }
                    }
                    itineraryDetails += "</ul>";
                }

                // Construct the email body
                string body = $@"
            <p><b>Dear {accountName}</b>,</p>
            <p>This is your itinerary for Day {dayNumber} of your travel plan in {areaName}.</p>
            {itineraryDetails}
            <p>Have a great day ahead!</p>
            <p><b>Take My Trip</b></p>";

                // Set up and send the email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, "mkhm ibkq tijr svds"),
                    EnableSsl = true
                };

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while sending the daily itinerary email: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return false;
            }
        }

        private string GeneratePlanId()
        {
            string newPlanId = "TP000001";
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 plan_id FROM Travel_Plan ORDER BY plan_id DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var lastId = cmd.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith("TP"))
                    {
                        int lastNum = int.Parse(lastId.Substring(2));
                        newPlanId = "TP" + (lastNum + 1).ToString("D6");
                    }
                }
                conn.Close();
            }
            return newPlanId;
        }

        private string GenerateItineraryId()
        {
            string newItineraryId = "D0000001";
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 itinerary_id FROM Daily_Itinerary ORDER BY itinerary_id DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var lastId = cmd.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith("D"))
                    {
                        int lastNum = int.Parse(lastId.Substring(1));
                        newItineraryId = "D" + (lastNum + 1).ToString("D7");
                    }
                }
                conn.Close();
            }
            return newItineraryId;
        }

        private string GenerateActivityId()
        {
            string newActivityId = "TA000001";
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 activity_id FROM Travel_Activity ORDER BY activity_id DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    var lastId = cmd.ExecuteScalar()?.ToString();
                    if (!string.IsNullOrEmpty(lastId) && lastId.StartsWith("TA"))
                    {
                        int lastNum = int.Parse(lastId.Substring(2));
                        newActivityId = "TA" + (lastNum + 1).ToString("D6");
                    }
                }
                conn.Close();
            }
            return newActivityId;
        }

        [System.Web.Services.WebMethod]
        public static void UpdateSelectedLocations(string updatedLocations)
        {
            // Deserialize the JSON list and store it in session
            var locationsList = JsonConvert.DeserializeObject<List<Location>>(updatedLocations);
            HttpContext.Current.Session["SelectedLocations"] = locationsList;
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
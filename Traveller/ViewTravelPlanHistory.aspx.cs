using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner.Traveller
{
    public partial class ViewTravelPlanHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTravelPlans();
            }
        }

        private void BindTravelPlans()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TP.plan_id, TP.plan_date, TP.duration, TP.budget, 
                           A.area_name, 
                           CASE WHEN DATEADD(day, TP.duration, TP.plan_date) < GETDATE() THEN 'Finished' ELSE 'Upcoming' END AS status
                    FROM Travel_Plan TP
                    INNER JOIN Area A ON TP.area_id = A.area_id
                    WHERE TP.account_id = @account_id";

                SqlCommand cmd = new SqlCommand(query, conn);
                //cmd.Parameters.AddWithValue("@account_id", Session["AccountID"]);
                cmd.Parameters.AddWithValue("@account_id","AC0521");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptTravelPlans.DataSource = dt;
                rptTravelPlans.DataBind();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // Get the Repeater item from the button that was clicked
            Button btnDelete = (Button)sender;
            RepeaterItem item = (RepeaterItem)btnDelete.NamingContainer;

            // Retrieve the plan_id from the hidden field
            HiddenField hfPlanId = (HiddenField)item.FindControl("hfPlanId");
            string planId = hfPlanId.Value;

            // Proceed with deletion
            if (DeleteTravelPlan(planId))
            {
                // Refresh the repeater after deletion
                BindTravelPlans();
            }
            else
            {
                // Handle delete failure (optional)
                Response.Write("<script>alert('Failed to delete the travel plan.');</script>");
            }
        }

        private bool DeleteTravelPlan(string planId)
        {
            bool success = false;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string deleteActivitiesQuery = "DELETE FROM Travel_Activity WHERE itinerary_id IN (SELECT itinerary_id FROM Daily_Itinerary WHERE plan_id = @plan_id)";
                string deleteItinerariesQuery = "DELETE FROM Daily_Itinerary WHERE plan_id = @plan_id";
                string deletePlanQuery = "DELETE FROM Travel_Plan WHERE plan_id = @plan_id";

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(deleteActivitiesQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@plan_id", planId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(deleteItinerariesQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@plan_id", planId);
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(deletePlanQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@plan_id", planId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error deleting travel plan: " + ex.Message);
                    }
                }
            }
            return success;
        }
    }
}
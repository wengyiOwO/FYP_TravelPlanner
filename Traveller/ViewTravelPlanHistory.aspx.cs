using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Numerics;

namespace FYP_TravelPlanner.Traveller
{
    public partial class ViewTravelPlanHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                LoadAreaDropdown();
                BindTravelPlans();
            }
        }

        private void LoadAreaDropdown()
        {
            string accountId = Session["account_id"] as string;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT DISTINCT A.area_id, A.area_name FROM Travel_Plan TP JOIN Area A ON TP.area_id = A.area_id AND TP.account_id = @accountId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@accountId", accountId);

                SqlDataReader reader = cmd.ExecuteReader();

                ddlArea.Items.Clear();
                ddlArea.Items.Add(new ListItem("All Areas", "")); 

                while (reader.Read())
                {
                    ddlArea.Items.Add(new ListItem(reader["area_name"].ToString(), reader["area_id"].ToString()));
                }

                conn.Close();
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
                    WHERE TP.account_id = @account_id AND TP.plan_status != 'Deleted'";

                if (!string.IsNullOrEmpty(ddlArea.SelectedValue))
                {
                    query += " AND A.area_id = @area_id";
                }

                query += " ORDER BY TP.plan_date DESC";

                string accountId = Session["account_id"] as string;
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@account_id", accountId);

                if (!string.IsNullOrEmpty(ddlArea.SelectedValue))
                {
                    cmd.Parameters.AddWithValue("@area_id", ddlArea.SelectedValue);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                pnlNoData.Visible = dt.Rows.Count == 0;

                rptTravelPlans.DataSource = dt;
                rptTravelPlans.DataBind();
            }
        }

        protected void Filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTravelPlans();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btnDelete = (Button)sender;

            RepeaterItem item = (RepeaterItem)btnDelete.NamingContainer;

            HiddenField hfPlanId = (HiddenField)item.FindControl("hfPlanId");
            string planId = hfPlanId.Value;
            
            hfSelectedPlanId.Value = planId.ToString();
                pnlConfirmDelete.Style["display"] = "block";

           
        }

        protected void btnConfirmDelete_Click(object sender, EventArgs e)
        {
            string planId = hfSelectedPlanId.Value;

            if (DeleteTravelPlan(planId))
            {
                BindTravelPlans();
                successPanel.Visible = true;
            }
            else
            {
                Response.Write("<script>alert('Failed to delete the travel plan.');</script>");
            }

            pnlConfirmDelete.Style["display"] = "none";
        }


        private bool DeleteTravelPlan(string planId)
        {
            bool success = false;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE Travel_Plan SET plan_status = 'Deleted' WHERE plan_id = @plan_id";

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
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

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class ViewUserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            SqlDataSource1.SelectCommand = @"
                SELECT * 
                FROM [Account] 
                WHERE ([account_id] LIKE @SearchTerm 
                       OR [account_name] LIKE @SearchTerm 
                       OR [account_email] LIKE @SearchTerm 
                       OR [account_phoneNo] LIKE @SearchTerm)
                  AND [account_status] <> 'Deleted'";

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("SearchTerm", DbType.String, "%" + searchTerm + "%");

            try
            {
                GridView1.DataBind();

                if (GridView1.Rows.Count == 0)
                {
                    lblErrorMessage.Text = "Oops, no results found.";
                    lblErrorMessage.Visible = true;
                }
                else
                {
                    lblErrorMessage.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Oops, an error occurred: {ex.Message}";
                lblErrorMessage.Visible = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string accountId = GridView1.DataKeys[e.RowIndex]?.Value.ToString();

            if (string.IsNullOrEmpty(accountId))
            {
                lblErrorMessage.Text = "Account ID not found!";
                lblErrorMessage.Visible = true;
                e.Cancel = true;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check current status
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT account_status FROM Account WHERE account_id = @accountId", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@accountId", accountId);
                        string accountStatus = checkCmd.ExecuteScalar()?.ToString();

                        if (accountStatus == "Active")
                        {
                            lblErrorMessage.Text = "Active accounts cannot be deleted!";
                            lblErrorMessage.Visible = true;
                            e.Cancel = true;
                            return;
                        }
                    }

                    // Update status to 'Deleted'
                    using (SqlCommand updateCmd = new SqlCommand(
                        "UPDATE Account SET account_status = 'Deleted' WHERE account_id = @accountId", conn))
                    {
                        updateCmd.Parameters.AddWithValue("@accountId", accountId);
                        updateCmd.ExecuteNonQuery();
                    }
                }

                lblErrorMessage.Text = "Account successfully deleted.";
                lblErrorMessage.ForeColor = System.Drawing.Color.Green;
                lblErrorMessage.Visible = true;

                // Rebind the GridView
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = $"Error: {ex.Message}";
                lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                lblErrorMessage.Visible = true;
            }
        }
    }
}

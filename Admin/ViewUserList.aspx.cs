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
            AND [account_role] <> 'Admin'      ";

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("SearchTerm", DbType.String, "%" + searchTerm + "%");

            try
            {
                GridView1.DataBind();

                if (GridView1.Rows.Count == 0)
                {
                    lblMessage.Text = "Oops, no results found.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Visible = true;
                }
                else
                {
                    lblMessage.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Oops, an error occurred: {ex.Message}";
                lblMessage.Visible = true;
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string accountId = GridView1.DataKeys[e.RowIndex]?.Value.ToString();

            if (string.IsNullOrEmpty(accountId))
            {
                lblMessage.Text = "Account ID not found!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
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
                            lblMessage.Text = "Active accounts cannot be deleted!";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Visible = true;
                            e.Cancel = true;
                            return;
                        }
                    }
                    // Attempt to delete the account
                    using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM Account WHERE account_id = @accountId", conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@accountId", accountId);

                        try
                        {
                            int rowsAffected = deleteCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                lblMessage.Text = "Account does not exist or cannot be deleted.";
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                                lblMessage.Visible = true;
                                e.Cancel = true;
                                return;
                            }
                        }
                        catch (SqlException ex) when (ex.Number == 547) // Foreign key violation
                        {
                            lblMessage.Text = "Cannot delete account as it is linked to other records.";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            lblMessage.Visible = true;
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                lblMessage.Text = "Account successfully deleted.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Visible = true;

                // Rebind the GridView
                GridView1.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.Text = $"Error: {ex.Message}";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Visible = true;
            }

        }

        protected void SqlDataSource1_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            // Check if the update was successful
            if (e.AffectedRows > 0)
            {
                lblMessage.Text = "Details updated successfully.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lblMessage.Text = "Failed to update details. Please try again.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            lblMessage.Visible = true;
        }
    }
}

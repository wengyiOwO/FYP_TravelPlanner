using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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

            // Perform the search using your data source (SqlDataSource1)
            SqlDataSource1.SelectCommand = "SELECT * FROM [Account] WHERE [account_id] LIKE @SearchTerm OR [account_name] LIKE @SearchTerm OR [account_email] LIKE @SearchTerm OR [account_phoneNo] LIKE @SearchTerm";

            SqlDataSource1.SelectParameters.Clear();
            SqlDataSource1.SelectParameters.Add("SearchTerm", DbType.String, "%" + searchTerm + "%");

            try
            {
                GridView1.DataBind();

                // Check if any rows are returned
                if (GridView1.Rows.Count == 0)
                {
                    lblErrorMessage.Text = "Oops, No results found.";
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
            string accountId = GridView1.DataKeys[e.RowIndex].Value.ToString();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                // Check account status
                SqlCommand checkStatusCmd = new SqlCommand("SELECT account_status FROM Account WHERE account_id = @accountId", conn);
                checkStatusCmd.Parameters.AddWithValue("@accountId", accountId);


                string accountStatus = checkStatusCmd.ExecuteScalar() as string;



                // Check account_status value
                if (accountStatus == "Active")
                {
                    lblErrorMessage.Visible = true;
                    lblErrorMessage.Text = "Active accounts cannot be deleted!";
                    e.Cancel = true;  // Cancel the delete operation
                }
                else
                {
                    lblErrorMessage.Visible = false;
                    // Update account status to "Deleted"
                    SqlCommand deleteCmd = new SqlCommand("UPDATE Account SET account_status = 'Deleted' WHERE account_id = @accountId", conn);
                    deleteCmd.Parameters.AddWithValue("@accountId", accountId);
                    deleteCmd.ExecuteNonQuery();

                    // Rebind GridView to reflect changes
                    GridView1.DataBind();
                }
            }

        }



    }
}
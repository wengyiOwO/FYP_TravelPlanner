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

            // Retrieve account_status from database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT account_status FROM Account WHERE account_id = @accountId", conn);
                cmd.Parameters.AddWithValue("@accountId", GridView1.DataKeys[e.RowIndex].Value);


                string accountStatus = cmd.ExecuteScalar() as string;



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
            }
            }
        }



    }
}
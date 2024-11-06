using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FYP_TravelPlanner
{
    public partial class ViewPostList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void ValidateDate(object source, ServerValidateEventArgs args)
        {
            DateTime parsedDate;
            // Try to parse the date in dd/MM/yyyy format
            bool isValid = DateTime.TryParseExact(args.Value, "dd/MM/yyyy",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.None,
                                                   out parsedDate);

            if (!isValid)
            {
                args.IsValid = false;
            }
            else if (parsedDate > DateTime.Now)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void GridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            TextBox txtPostDate = (TextBox)GridView1.Rows[e.RowIndex].FindControl("TextBox3");

            DateTime parsedDate;
            if (DateTime.TryParseExact(txtPostDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                e.NewValues["post_date"] = parsedDate;
            }
            else
            {
                e.Cancel = true;
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            SqlDataSource1.SelectCommand = "SELECT * FROM [Posts] WHERE [post_id] LIKE @SearchTerm OR [post_title] LIKE @SearchTerm OR [post_content] LIKE @SearchTerm OR [post_date] LIKE @SearchTerm";

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

    }
}
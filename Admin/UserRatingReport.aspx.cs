using System;
using System.Configuration;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;


namespace FYP_TravelPlanner
{
    public partial class UserRatingReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ratingData = GetRatingData();
                LoadRatingData();

                // Pass the data to JavaScript to update the pie chart
                var script = $"var ratingData = {ratingData};";
                ClientScript.RegisterStartupScript(this.GetType(), "setRatingData", script, true);
            }
        }

        private string GetRatingData()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int[] ratingsCount = new int[5];

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = "SELECT rating, COUNT(*) FROM Rating GROUP BY rating";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rating = reader.GetInt32(0);
                            int count = reader.GetInt32(1);

                            ratingsCount[rating - 1] = count;
                        }
                    }
                }
            }

            // Convert the ratings data array to a JavaScript array string
            return $"[{string.Join(",", ratingsCount)}]";
        }

        private void LoadRatingData()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) AS TotalCount,
                           AVG(CAST(Rating AS FLOAT)) AS AvgRating,
                           SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS FiveStarPercent
                    FROM Rating";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Populate the labels with data
                        lblTotalRating.Text = reader["TotalCount"].ToString();
                        lblAvgRating.Text = Convert.ToDouble(reader["AvgRating"]).ToString("0.0");
                        lblPercentage.Text = Convert.ToDouble(reader["FiveStarPercent"]).ToString("0.0") + "%";
                    }
                }
            }
        }

        //protected void btnGeneratePdf_Click(object sender, EventArgs e)
        //{
        //    // Retrieve the base64 chart image
        //    string base64ChartData = chartImage.Value;

        //    // Decode base64 to byte array
        //    byte[] chartImageBytes = Convert.FromBase64String(base64ChartData.Replace("data:image/png;base64,", ""));

        //    // Generate PDF
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        Document document = new Document(PageSize.A4);
        //        PdfWriter.GetInstance(document, ms);
        //        document.Open();

        //        // Add title or any text content
        //        document.Add(new Paragraph("User Ratings Report"));

        //        // Add chart image to PDF
        //        iTextSharp.text.Image chartImage = iTextSharp.text.Image.GetInstance(chartImageBytes);
        //        chartImage.ScaleToFit(400f, 300f);
        //        document.Add(chartImage);

        //        document.Close();

        //        // Output the PDF to the browser
        //        Response.ContentType = "application/pdf";
        //        Response.AddHeader("content-disposition", "attachment;filename=UserRatingsReport.pdf");
        //        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //        Response.BinaryWrite(ms.ToArray());
        //        Response.End();
        //    }
    }

}
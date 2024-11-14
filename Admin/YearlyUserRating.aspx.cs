using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;


namespace FYP_TravelPlanner
{
    public partial class YearlyUserRating : System.Web.UI.Page
    {
        protected string RatingDataJson = "[]"; 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRatingData(DateTime.Now.Year);
            }
        }
        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            // Load data for the selected month and year
            LoadRatingData(selectedYear);
        }

        private void LoadRatingData(int year)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            int[] ratingsCount = new int[5];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT rating, COUNT(*) 
                    FROM Rating 
                    WHERE YEAR(rating_date) = @Year
                    GROUP BY rating";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

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

                // Convert ratingsCount to JSON
                RatingDataJson = JsonConvert.SerializeObject(ratingsCount);
            }

            // Load summary information (total, avg, 5-star percentage)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string summaryQuery = @"
                    SELECT COUNT(*) AS TotalCount,
                           AVG(CAST(Rating AS FLOAT)) AS AvgRating,
                           SUM(CASE WHEN Rating = 5 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS FiveStarPercent
                    FROM Rating   
                    WHERE YEAR(rating_date) = @Year";

                using (SqlCommand cmd = new SqlCommand(summaryQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalRating.Text = reader["TotalCount"].ToString();

                        // Check for DBNull before converting AvgRating
                        lblAvgRating.Text = reader["AvgRating"] != DBNull.Value
                            ? Convert.ToDouble(reader["AvgRating"]).ToString("0.0")
                            : "0.0";

                        // Check for DBNull before converting FiveStarPercent
                        lblPercentage.Text = reader["FiveStarPercent"] != DBNull.Value
                            ? Convert.ToDouble(reader["FiveStarPercent"]).ToString("0.0") + "%"
                            : "0.0%";
                    }

                }
            }
        }

        private byte[] GenerateChartImage(int[] ratingData)
        {
            int width = 600;
            int height = 500;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                // Clear the bitmap with a solid white color
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.White);  // Clear the background to white

                    Brush[] brushes = {
                new SolidBrush(Color.Red),
                new SolidBrush(Color.Orange),
                new SolidBrush(Color.Yellow),
                new SolidBrush(Color.Green),
                new SolidBrush(Color.Blue)
            };
                    System.Drawing.Font font = new System.Drawing.Font("Arial", 14, FontStyle.Bold);

                    // Calculate total ratings for pie chart
                    int totalRatings = ratingData.Sum();

                    // Set the starting angle for the pie chart segments
                    float startAngle = 0;
                    string[] labels = { "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars" };

                    for (int i = 0; i < ratingData.Length; i++)
                    {
                        if (ratingData[i] == 0)
                            continue;
                        // Calculate the sweep angle for each rating segment
                        float sweepAngle = (totalRatings > 0) ? (float)ratingData[i] / totalRatings * 360 : 0;

                        // Draw each segment
                        g.FillPie(brushes[i], 50, 50, width - 100, height - 100, startAngle, sweepAngle);

                        // Calculate label position, adjusted farther from the pie center
                        float labelAngle = startAngle + sweepAngle / 2;
                        float labelDistance = 150; // Distance from the center to place the label
                        float labelX = (width / 2) + (float)Math.Cos(labelAngle * Math.PI / 180) * labelDistance;
                        float labelY = (height / 2) + (float)Math.Sin(labelAngle * Math.PI / 180) * labelDistance;

                        // Draw the label with larger font
                        g.DrawString($"{labels[i]}: {ratingData[i]}", font, Brushes.Black, labelX, labelY);

                        // Update the start angle for the next segment
                        startAngle += sweepAngle;
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }


        public void GenerateUserRatingsPDFWithChart(int[] ratingData)
        {
            byte[] chartImageBytes = GenerateChartImage(ratingData);

            Document pdfDoc = new Document(PageSize.A4);
            string pdfPath = Server.MapPath("~/AnnualUserRatingsReport.pdf");
            PdfWriter.GetInstance(pdfDoc, new FileStream(pdfPath, FileMode.Create));
            pdfDoc.Open();

            // Header table with logo
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;
            headerTable.HorizontalAlignment = Element.ALIGN_CENTER;

            string logoPath = Server.MapPath("~/img/logo.png");
            if (File.Exists(logoPath))
            {
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(140f, 140f);
                PdfPCell logoCell = new PdfPCell(logo)
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                headerTable.AddCell(logoCell);
            }

            pdfDoc.Add(headerTable);
            pdfDoc.Add(new Paragraph("Annual User Ratings Report", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD)));

            // Rating Count Table
            PdfPTable ratingCountTable = new PdfPTable(2);
            ratingCountTable.SpacingBefore = 20f;
            ratingCountTable.WidthPercentage = 100;

            ratingCountTable.AddCell("Rating");
            ratingCountTable.AddCell("Count");

            string[] labels = { "1 star", "2 stars", "3 stars", "4 stars", "5 stars" };
            for (int i = 0; i < ratingData.Length; i++)
            {
                ratingCountTable.AddCell(labels[i]);
                ratingCountTable.AddCell(ratingData[i].ToString());
            }

            pdfDoc.Add(new Paragraph("Rating Count", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD, BaseColor.BLUE)));
            pdfDoc.Add(ratingCountTable);

            // Rating Summary Table
            PdfPTable ratingSummaryTable = new PdfPTable(2);
            ratingSummaryTable.SpacingBefore = 20f;
            ratingSummaryTable.WidthPercentage = 100;

            ratingSummaryTable.AddCell("Metric");
            ratingSummaryTable.AddCell("Value");

            ratingSummaryTable.AddCell("Total Ratings:");
            ratingSummaryTable.AddCell(lblTotalRating.Text);

            ratingSummaryTable.AddCell("Average Rating:");
            ratingSummaryTable.AddCell(lblAvgRating.Text);

            ratingSummaryTable.AddCell("5-Star Percentage:");
            ratingSummaryTable.AddCell(lblPercentage.Text);

            pdfDoc.Add(new Paragraph("Rating Summary", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD, BaseColor.BLUE)));
            pdfDoc.Add(ratingSummaryTable);

            // Add chart image if it exists
            if (chartImageBytes != null && chartImageBytes.Length > 0)
            {
                Image chartImage = Image.GetInstance(chartImageBytes);
                chartImage.ScaleToFit(400f, 200f);
                chartImage.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(chartImage);
            }
            else
            {
                pdfDoc.Add(new Paragraph("Chart image could not be generated.", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));
            }

            pdfDoc.Close();
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            // Load data for the selected month and year
            LoadRatingData(selectedYear);

            int[] ratingData = JsonConvert.DeserializeObject<int[]>(RatingDataJson);

            if (ratingData == null || ratingData.Length == 0)
            {
                lblMessage.Text = "No rating data available to generate the report.";
                return;
            }

            GenerateUserRatingsPDFWithChart(ratingData);
            lblMessage.Text = "Annual User Ratings PDF has been generated successfully. <a href='/AnnualUserRatingsReport.pdf' target='_blank'>Download PDF</a>";
        }
    }
}
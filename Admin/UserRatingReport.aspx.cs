using System;
using System.Configuration;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Drawing;
using Image = iTextSharp.text.Image;
using System.Drawing.Imaging;
using System.Linq;


namespace FYP_TravelPlanner
{
    public partial class UserRatingReport : System.Web.UI.Page
    {
        protected string RatingDataJson = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetRatingData();
                System.Diagnostics.Debug.WriteLine("RatingDataJson after GetRatingData: " + RatingDataJson);
                LoadRatingData();


            }
        }

        private void GetRatingData()
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

            // Output ratingsCount to verify values
            System.Diagnostics.Debug.WriteLine("Ratings Count: " + string.Join(", ", ratingsCount));

            // Convert the ratings data array to a JavaScript array string
            RatingDataJson = JsonConvert.SerializeObject(ratingsCount);
            System.Diagnostics.Debug.WriteLine("RatingDataJson: " + RatingDataJson);

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
            string pdfPath = Server.MapPath("~/UserRatingsReport.pdf");
            PdfWriter.GetInstance(pdfDoc, new FileStream(pdfPath, FileMode.Create));
            pdfDoc.Open();
            // Create a table with a single column to stack the logo and company name vertically
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100; // Set the table width to full width for easier centering

            // Center the table horizontally on the page
            headerTable.HorizontalAlignment = Element.ALIGN_CENTER;

            // Add company logo to the first cell and center it
            string logoPath = Server.MapPath("~/img/logo.png");
            if (File.Exists(logoPath))
            {
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(140f, 140f); // Scale the logo as needed
                PdfPCell logoCell = new PdfPCell(logo);
                logoCell.Border = PdfPCell.NO_BORDER;
                logoCell.HorizontalAlignment = Element.ALIGN_CENTER; // Center-align the logo within the cell
                headerTable.AddCell(logoCell);
            }

          
            // Add the header table to the document
            pdfDoc.Add(headerTable);


            // Add some spacing after logo and name

            pdfDoc.Add(new Paragraph("User Ratings Report", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD, BaseColor.BLUE)));
            pdfDoc.Add(new Paragraph("Rating Summary:", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16, iTextSharp.text.Font.BOLD)));

            string[] labels = { "1 star", "2 stars", "3 stars", "4 stars", "5 stars" };
            for (int i = 0; i < ratingData.Length; i++)
            {
                
                    pdfDoc.Add(new Paragraph($"{labels[i]}: {ratingData[i]}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));
                
            }

            pdfDoc.Add(new Paragraph($"Total Ratings: {lblTotalRating.Text}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));
            pdfDoc.Add(new Paragraph($"Average Rating: {lblAvgRating.Text}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));
            pdfDoc.Add(new Paragraph($"5-Star Percentage: {lblPercentage.Text}", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));

            // Only add chart image if it exists
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
            GetRatingData(); 
            int[] ratingData = JsonConvert.DeserializeObject<int[]>(RatingDataJson);

            if (ratingData == null || ratingData.Length == 0)
            {
                lblMessage.Text = "No rating data available to generate the report.";
                return;
            }

            GenerateUserRatingsPDFWithChart(ratingData);
            lblMessage.Text = "User Ratings PDF has been generated successfully. <a href='/UserRatingsReport.pdf' target='_blank'>Download PDF</a>";
        }

    }

}
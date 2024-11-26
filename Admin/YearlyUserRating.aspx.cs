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
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
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
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.Clear(Color.White); // Clear the background to white

                    Brush[] brushes = {
                new SolidBrush(Color.FromArgb(244, 67, 54)),  // Red
                new SolidBrush(Color.FromArgb(255, 152, 0)),  // Orange
                new SolidBrush(Color.FromArgb(255, 235, 59)), // Yellow
                new SolidBrush(Color.FromArgb(76, 175, 80)),  // Green
                new SolidBrush(Color.FromArgb(33, 150, 243))  // Blue
            };

                    System.Drawing.Font titleFont = new System.Drawing.Font("Arial", 16, FontStyle.Bold);
                    System.Drawing.Font labelFont = new System.Drawing.Font("Arial", 12, FontStyle.Regular);

                    g.DrawString("User Ratings Distribution", titleFont, Brushes.Black, new PointF(width / 2 - 150, 20));

                    int totalRatings = ratingData.Sum();
                    float startAngle = 0;
                    string[] labels = { "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars" };

                    for (int i = 0; i < ratingData.Length; i++)
                    {
                        if (ratingData[i] == 0)
                            continue;

                        // Calculate sweep angle
                        float sweepAngle = totalRatings > 0 ? (float)ratingData[i] / totalRatings * 360 : 0;

                        // Draw pie segment
                        g.FillPie(brushes[i], 100, 100, width - 200, height - 200, startAngle, sweepAngle);
                        g.DrawPie(Pens.Black, 100, 100, width - 200, height - 200, startAngle, sweepAngle);

                        // Draw labels with percentages
                        float labelAngle = startAngle + sweepAngle / 2;
                        float labelDistance = 180; // Distance from the pie center
                        float labelX = (width / 2) + (float)Math.Cos(labelAngle * Math.PI / 180) * labelDistance;
                        float labelY = (height / 2) + (float)Math.Sin(labelAngle * Math.PI / 180) * labelDistance;

                        string label = $"{labels[i]}: {ratingData[i]} ({(float)ratingData[i] / totalRatings * 100:0.0}%)";
                        g.DrawString(label, labelFont, Brushes.Black, labelX - 50, labelY);

                        startAngle += sweepAngle;
                    }

                    // Add legend
                    float legendX = 516, legendY = 130;
                    for (int i = 0; i < brushes.Length; i++)
                    {
                        g.FillRectangle(brushes[i], legendX, legendY, 20, 20);
                        g.DrawRectangle(Pens.Black, legendX, legendY, 20, 20);
                        g.DrawString(labels[i], labelFont, Brushes.Black, legendX + 30, legendY);
                        legendY += 30;
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }


        public void GenerateUserRatingsPDFWithChart(int[] ratingData, int year)
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

            // Center-align title text with selected year
            string yearTitle = $"Annual User Ratings Report for {year}";
            Paragraph title = new Paragraph(yearTitle,
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(title);

            // Add current date and time
            Paragraph dateParagraph = new Paragraph("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11));
            dateParagraph.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(dateParagraph);
            pdfDoc.Add(new Paragraph(" "));

            // Create the data table
            PdfPTable dataTable = new PdfPTable(3);
            dataTable.WidthPercentage = 100;
            dataTable.SetWidths(new float[] { 1, 4, 2 }); // Column width proportions

            // Add column headers with horizontal line below
            AddTableCell(dataTable, "No", true, false, true);
            AddTableCell(dataTable, "Ratings", true, false, true);
            AddTableCell(dataTable, "Count of Rating", true, false, true);


            int totalRating = 0;
            string[] labels = { "1 Star", "2 Stars", "3 Stars", "4 Stars", "5 Stars" };
            for (int i = 0; i < ratingData.Length; i++)
            {
              int ratingCount = Convert.ToInt32(ratingData[i]);
                totalRating += ratingCount;
              
                AddTableCell(dataTable, (i + 1).ToString(), false, false, false);
                AddTableCell(dataTable, labels[i].ToString(), false, false, false);
                AddTableCell(dataTable, ratingCount.ToString(), false, false, false);
                
            }

            AddTableCell(dataTable, " ", false, true, false);
            AddTableCell(dataTable, "Total Count", true, true, false);
            AddTableCell(dataTable, totalRating.ToString(), true, true, false); // Total value with borders
           
            AddTableCell(dataTable, " ", false, false, true);
            AddTableCell(dataTable, "Average Rating", true, false, true);
            AddTableCell(dataTable, lblAvgRating.Text.ToString(), true, false, true); // Total value with borders

            pdfDoc.Add(dataTable);

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
                pdfDoc.Add(new Paragraph("Chart image could not be generated.",
                    new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12)));
            }

            pdfDoc.Close();
        }
        private void AddTableCell(PdfPTable table, string text, bool isHeader, bool drawTopBorder, bool drawBottomBorder)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text,
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, isHeader ? 12 : 10, isHeader ? iTextSharp.text.Font.BOLD : iTextSharp.text.Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;

            // Set border options
            cell.Border = PdfPCell.NO_BORDER;
            if (drawTopBorder) cell.Border |= PdfPCell.TOP_BORDER;
            if (drawBottomBorder) cell.Border |= PdfPCell.BOTTOM_BORDER;

            table.AddCell(cell);
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

            GenerateUserRatingsPDFWithChart(ratingData, selectedYear);
            lblMessage.Text = "Annual User Ratings PDF has been generated successfully. <a href='/AnnualUserRatingsReport.pdf' target='_blank'>Download PDF</a>";
        }
    }
}
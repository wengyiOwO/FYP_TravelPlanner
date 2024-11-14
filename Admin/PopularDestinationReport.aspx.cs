using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;
using System.Collections.Generic;

namespace FYP_TravelPlanner
{
    public partial class PopularDestinationReport : System.Web.UI.Page
    {
        protected string DestinationDataJson = "[]"; // JSON for chart data

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTopDestinationsJson();
                ClientScript.RegisterStartupScript(this.GetType(), "setDestinationData", $"var DestinationDataJson = {DestinationDataJson};", true);
            }
        }

        private void GetTopDestinationsJson()
        {
            DataTable destinations = new DataTable();
            destinations.Columns.Add("location_name", typeof(string));
            destinations.Columns.Add("visit_count", typeof(int));

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = @"
                SELECT TOP 8
                    l.place_name AS location_name,
                    COUNT(tp.plan_id) AS visit_count
                FROM 
                    Travel_Plan tp
                JOIN 
                    Area a ON tp.area_id = a.area_id
                JOIN 
                    Location l ON l.area_id = a.area_id
                GROUP BY 
                    l.place_name
                ORDER BY 
                    visit_count DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        destinations.Load(reader);
                    }
                }
            }

            DestinationDataJson = JsonConvert.SerializeObject(destinations);
        }

        private byte[] GenerateBarChartImage(DataTable destinationData)
        {
            int width = 800;
            int height = 400;
            int barWidth = 60;
            int spacing = 35;
            int labelOffset = 15;
            int maxLabelLength = 10; // Shorten max length to allow more lines
            int labelPadding = 8;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    // Determine max visit count for scaling
                    int maxVisits = destinationData.AsEnumerable()
                        .Max(row => Convert.ToInt32(row["visit_count"]));

                    // Define fonts and brushes
                    System.Drawing.Font font = new System.Drawing.Font("Arial", 12);
                    Brush brush = new SolidBrush(Color.Black);
                    Brush barBrush = new SolidBrush(Color.Blue);

                    // Draw bars
                    for (int i = 0; i < destinationData.Rows.Count; i++)
                    {
                        string locationName = destinationData.Rows[i]["location_name"].ToString();
                        int visitCount = Convert.ToInt32(destinationData.Rows[i]["visit_count"]);

                        // Calculate bar height
                        int barHeight = (int)((double)visitCount / maxVisits * (height - 100));

                        // Calculate positions
                        int x = i * (barWidth + spacing) + labelOffset;
                        int y = height - barHeight - 50;

                        // Draw bar
                        g.FillRectangle(barBrush, x, y, barWidth, barHeight);

                        // Draw visit count above bar
                        g.DrawString(visitCount.ToString(), font, brush, x + barWidth / 4, y - 20);

                        // Split label into up to four lines
                        List<string> labelLines = new List<string>();
                        while (locationName.Length > maxLabelLength && labelLines.Count < 3)
                        {
                            int splitIndex = locationName.LastIndexOf(' ', maxLabelLength);
                            if (splitIndex == -1) splitIndex = maxLabelLength;

                            labelLines.Add(locationName.Substring(0, splitIndex).Trim());
                            locationName = locationName.Substring(splitIndex).Trim();
                        }
                        labelLines.Add(locationName);

                        // Draw each line with appropriate spacing
                        for (int j = 0; j < labelLines.Count; j++)
                        {
                            g.DrawString(labelLines[j], font, brush, x, height - 55 + (j * 15) + labelPadding);
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private void GeneratePopularDestinationPDF(DataTable destinationData)
        {
            // Generate chart image bytes
            byte[] chartImageBytes = GenerateBarChartImage(destinationData);

            Document pdfDoc = new Document(PageSize.A4);
            string pdfPath = Server.MapPath("~/PopularDestinationReport.pdf");
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
            // Title
            pdfDoc.Add(new Paragraph("Overall Popular Destination Report", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD, BaseColor.BLACK)));
            pdfDoc.Add(new Paragraph(" ")); // Add space after title

            // Add data table
            PdfPTable pdfTable = new PdfPTable(2);
            pdfTable.AddCell("Location");
            pdfTable.AddCell("Visit Count");

            foreach (DataRow row in destinationData.Rows)
            {
                pdfTable.AddCell(row["location_name"].ToString());
                pdfTable.AddCell(row["visit_count"].ToString());
            }

            pdfDoc.Add(pdfTable);
            pdfDoc.Add(new Paragraph(" ")); // Add space after table

            // Add chart image
            if (chartImageBytes != null && chartImageBytes.Length > 0)
            {
                Image chartImage = Image.GetInstance(chartImageBytes);
                chartImage.ScaleToFit(500f, 300f); // Adjust image size
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
            // Load destination data
            DataTable destinationData = new DataTable();
            destinationData.Columns.Add("location_name", typeof(string));
            destinationData.Columns.Add("visit_count", typeof(int));

            GetTopDestinationsJson();
            destinationData = JsonConvert.DeserializeObject<DataTable>(DestinationDataJson);

            if (destinationData.Rows.Count == 0)
            {
                lblMessage.Text = "No data available to generate the report.";
                return;
            }

            // Generate PDF report
            GeneratePopularDestinationPDF(destinationData);
            lblMessage.Text = "Overall Popular Destination Report has been generated successfully. <a href='/PopularDestinationReport.pdf' target='_blank'>Download PDF</a>";
        }
    }
}

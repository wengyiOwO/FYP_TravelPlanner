using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;


namespace FYP_TravelPlanner.js.demo
{
    public partial class MonthlyPopularDestinations : System.Web.UI.Page
    {
        protected string DestinationDataJson = "[]"; // JSON for chart data

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            if (!IsPostBack)
            {
                // Get the current month and year
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Fetch popular destinations for the current month and year
                GetTopDestinationsJson(currentMonth, currentYear);
                ClientScript.RegisterStartupScript(this.GetType(), "setDestinationData", $"var DestinationDataJson = {DestinationDataJson};", true);
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedValue == "" || DropDownList2.SelectedValue == "")
            {
                lblMessage.Text = "Please select both Month and Year to generate the report.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
                return;
            }
            else
            {
                lblMessage.Visible = false;
            }
            int selectedMonth = int.Parse(DropDownList1.SelectedValue);
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            GetTopDestinationsJson(selectedMonth, selectedYear);
            ClientScript.RegisterStartupScript(this.GetType(), "setDestinationData", $"var DestinationDataJson = {DestinationDataJson};", true);

        }

        private void GetTopDestinationsJson(int month, int year)
        {
            DataTable destinations = new DataTable();
            destinations.Columns.Add("location_name", typeof(string));
            destinations.Columns.Add("visit_count", typeof(int));

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = @"
                SELECT TOP 8
    l.location_id,
    l.place_name AS location_name,
    COUNT(ta.location_id) AS visit_count
FROM 
    Travel_Activity ta
INNER JOIN 
    Daily_Itinerary di ON ta.itinerary_id = di.itinerary_id
INNER JOIN 
    Travel_Plan tp ON di.plan_id = tp.plan_id
INNER JOIN 
    Location l ON ta.location_id = l.location_id
WHERE 
    MONTH(tp.plan_date) = @Month and YEAR(tp.plan_date) = @Year 
GROUP BY 
    l.location_id, l.place_name, MONTH(tp.plan_date), YEAR(tp.plan_date)
ORDER BY 
    visit_count DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);
                    conn.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        destinations.Load(reader);
                    }
                }
            }

            DestinationDataJson = JsonConvert.SerializeObject(destinations);
        }
        private byte[] GenerateBarChartImage(DataTable destinationData)
        {
            int width = 950;
            int height = 750;
            int barWidth = 60;
            int spacing = 35;
            int labelOffset = 80;
            int maxLabelLength = 10;
            int labelPadding = 8;

            using (Bitmap bmp = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);

                    // Determine max visit count for scaling
                    int maxVisits = destinationData.AsEnumerable()
     .Select(row => Convert.ToInt32(row["visit_count"]))
     .DefaultIfEmpty(1) // Avoid divide by zero if dataset is empty
     .Max();

                    int numberOfGridlines = 5;
                    int gridlineSpacing = (height - 220) / numberOfGridlines;

                    System.Drawing.Font font = new System.Drawing.Font("Arial", 13);
                    System.Drawing.Font axisFont = new System.Drawing.Font("Arial", 18, FontStyle.Bold);
                    Brush brush = new SolidBrush(Color.Black);
                    Brush barBrush = new SolidBrush(Color.Blue);
                    Pen gridlinePen = new Pen(Color.LightGray, 1);

                    // Avoid duplicate labels 
                    HashSet<int> drawnLabels = new HashSet<int>();

                    for (int i = 0; i <= numberOfGridlines; i++)
                    {

                        int value = maxVisits * i / numberOfGridlines;

                        // Skip duplicate labels
                        if (!drawnLabels.Contains(value))
                        {
                            drawnLabels.Add(value);
                            int y = height - 145 - (i * gridlineSpacing);
                            g.DrawLine(gridlinePen, labelOffset, y, width - 50, y); // Draw gridline across the chart
                            g.DrawString(value.ToString(), font, brush, labelOffset - 40, y - 8);
                        }
                    }

                    // Draw Y-axis label (vertical, top-left corner)
                    g.RotateTransform(-90); // Rotate to draw vertically
                    g.DrawString("Number of Visits", axisFont, brush, -height / 2 - 50, 12);
                    g.RotateTransform(90); // Rotate back to original orientation

                    // Draw bars
                    for (int i = 0; i < destinationData.Rows.Count; i++)
                    {
                        string locationName = destinationData.Rows[i]["location_name"].ToString();
                        int visitCount = Convert.ToInt32(destinationData.Rows[i]["visit_count"]);

                        // Calculate bar height
                        int barHeight = (int)((double)visitCount / maxVisits * (height - 220));

                        // Calculate positions
                        int x = i * (barWidth + spacing) + labelOffset;
                        int y = height - barHeight - 145;

                        // Draw bar
                        g.FillRectangle(barBrush, x, y, barWidth, barHeight);

                        // Draw visit count above bar
                        g.DrawString(visitCount.ToString(), font, brush, x + barWidth / 4, y - 20);

                        // Split label into multiple lines if it's too long
                        List<string> labelLines = new List<string>();
                        while (locationName.Length > maxLabelLength && labelLines.Count < 4)
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
                            g.DrawString(labelLines[j], font, brush, x, height - 120 + (j * 15) + labelPadding);
                        }
                    }

                    // Draw X-axis label (horizontal, bottom center of the chart)
                    string xAxisLabel = "Top 8 Popular Destinations";
                    SizeF labelSize = g.MeasureString(xAxisLabel, axisFont);
                    float labelX = (width - labelSize.Width) / 2; // Center horizontally
                    float labelY = height - 30;
                    g.DrawString(xAxisLabel, axisFont, brush, labelX, labelY);

                }

                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        private void GeneratePopularDestinationPDF(DataTable destinationData, int month, int year)
        {
            // Generate chart image bytes
            byte[] chartImageBytes = GenerateBarChartImage(destinationData);

            Document pdfDoc = new Document(PageSize.A4);
            string pdfPath = Server.MapPath("~/MonthlyPopularDestinationReport.pdf");
            PdfWriter.GetInstance(pdfDoc, new FileStream(pdfPath, FileMode.Create));
            pdfDoc.Open();

            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;

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
            string monthYearTitle = $"Monthly Popular Destination Report for {new DateTime(year, month, 1):MMMM yyyy}";
            Paragraph title = new Paragraph(monthYearTitle,
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 20, iTextSharp.text.Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(title);

            // Add current date and time
            Paragraph dateParagraph = new Paragraph("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11));
            dateParagraph.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(dateParagraph);
            pdfDoc.Add(new Paragraph(" ")); // Add space after title

            // Create the data table
            PdfPTable dataTable = new PdfPTable(3);
            dataTable.WidthPercentage = 100;
            dataTable.SetWidths(new float[] { 1, 4, 2 }); // Column width proportions

            // Add column headers with horizontal line below
            AddTableCell(dataTable, "No", true, false, true);
            AddTableCell(dataTable, "Destinations", true, false, true);
            AddTableCell(dataTable, "Number of Visits", true, false, true);

            // Populate table rows without borders
            int totalVisits = 0;
            for (int i = 0; i < destinationData.Rows.Count; i++)
            {
                int visitCount = Convert.ToInt32(destinationData.Rows[i]["visit_count"]);
                totalVisits += visitCount;

                AddTableCell(dataTable, (i + 1).ToString(), false, false, false);
                AddTableCell(dataTable, destinationData.Rows[i]["location_name"].ToString(), false, false, false);
                AddTableCell(dataTable, visitCount.ToString(), false, false, false);
            }

            // Add total row with border line above and below
            AddTableCell(dataTable, " ", false, true, true);
            AddTableCell(dataTable, "Total", true, true, true);
            AddTableCell(dataTable, totalVisits.ToString(), true, true, true); // Total value with borders

            pdfDoc.Add(dataTable);

            // Add chart image
            if (chartImageBytes != null && chartImageBytes.Length > 0)
            {
                pdfDoc.Add(new Paragraph(" ")); // Add some space before the chart
                Image chartImage = Image.GetInstance(chartImageBytes);
                chartImage.ScaleToFit(500f, 300f);
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


        //add a cell to the table
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
            // Load destination data
            DataTable destinationData = new DataTable();
            destinationData.Columns.Add("location_name", typeof(string));
            destinationData.Columns.Add("visit_count", typeof(int));

            if (DropDownList1.SelectedValue == "" || DropDownList2.SelectedValue == "") { 
                lblMessage.Text = "Please select both Month and Year to generate the report.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;

                return;
            }
            else
            {
                lblMessage.Visible = false;
            }
            int selectedMonth = int.Parse(DropDownList1.SelectedValue);
            int selectedYear = int.Parse(DropDownList2.SelectedValue);

            GetTopDestinationsJson(selectedMonth, selectedYear);
            destinationData = JsonConvert.DeserializeObject<DataTable>(DestinationDataJson);

            if (destinationData.Rows.Count == 0)
            {
                lblMessage.Text = "No data available to generate the report.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;

                return;
            }
            else
            {
                lblMessage.Visible = false;
            }

            // Generate PDF report
            GeneratePopularDestinationPDF(destinationData, selectedMonth, selectedYear);
            lblMessage.Text = "Monthly Popular Destination Report has been generated successfully. <a href='/MonthlyPopularDestinationReport.pdf' target='_blank'>Download PDF</a>";
            lblMessage.Visible = true;
            lblMessage.ForeColor = Color.Green;

        }
    }
}

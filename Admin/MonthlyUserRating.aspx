<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyUserRating.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.MonthlyUserRating" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style type="text/css">
        .auto-style1 {
            width: 1241px;
        }

        .auto-style2 {
            width: 107px;
        }

        .auto-style4 {
            height: 29px;
        }

        .button-group {
            display: flex;
            justify-content: center;
            gap: 20px;
        }


        .summary-section {
            display: flex;
            flex-direction: column;
            gap: 15px;
            justify-content: flex-start;
        }
    </style>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.16.9/xlsx.full.min.js"></script>

    <script src="/js/MonthlyPieChart.js"></script>

    <script type="text/javascript">
        var ratingData = <%= RatingDataJson %>; 
        console.log("Rating Data:", ratingData); // Check data in console to verify

        function initializePieChart(data) {
            if (!data || data.length === 0) {
                console.warn("No rating data is available.");
                return; // Exit if there's no data to avoid creating an empty chart
            }

            const ctx = document.getElementById("myPieChart").getContext('2d');
            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ["1 star", "2 stars", "3 stars", "4 stars", "5 stars"],
                    datasets: [{
                        data: data,
                        backgroundColor: ['#e74a3b', '#f6c23e', '#36b9cc', '#1cc88a', '#4e73df'],
                        hoverBackgroundColor: ['#be2617', '#dda20a', '#2c9faf', '#17a673', '#2e59d9'],
                        hoverBorderColor: "rgba(234, 236, 244, 1)"
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        caretPadding: 10,
                    },
                    legend: {
                        display: false
                    },
                    cutoutPercentage: 80,
                },
            });
        }

        document.addEventListener("DOMContentLoaded", function () {
            // Initialize the chart only if there's rating data
            initializePieChart(ratingData);
        });
        async function exportToPDF() {
            // Check if rating data is available
            if (!ratingData || ratingData.length === 0) {
                console.error("No rating data available for PDF generation.");
                alert("No rating data available.");
                return;
            }


            // Initialize PDF document
            const { PDFDocument, rgb } = PDFLib;
            const pdfDoc = await PDFDocument.create();
            const page = pdfDoc.addPage([600, 700]);

            // Add report information to the PDF
            page.drawText("User Ratings Report", { x: 50, y: 650, size: 20, color: rgb(0, 0.53, 0.71) });

            // List each rating's count
            const ratingsListY = 550;
            page.drawText("Rating Counts:", { x: 50, y: ratingsListY, size: 16, color: rgb(0, 0, 0) });
            ["1 star", "2 stars", "3 stars", "4 stars", "5 stars"].forEach((label, index) => {
                page.drawText(`${label}: ${ratingData[index]}`, { x: 70, y: ratingsListY - 20 * (index + 1), size: 14 });
            });

            // Capture the chart as an image using html2canvas
            const chartCanvas = document.getElementById("myPieChart");
            if (chartCanvas) {
                try {
                    const chartImageData = await html2canvas(chartCanvas).then(canvas => canvas.toDataURL("image/png"));
                    const chartImageBytes = await pdfDoc.embedPng(chartImageData);
                    page.drawImage(chartImageBytes, { x: 50, y: 250, width: 500, height: 250 });
                } catch (error) {
                    console.error("Error capturing chart image:", error);
                }
            } else {
                console.error("Chart canvas element not found.");
            }
            // Calculate summary metrics
            const totalCount = ratingData.reduce((a, b) => a + b, 0);
            const avgRating = (ratingData.reduce((sum, value, index) => sum + value * (index + 1), 0) / totalCount).toFixed(1);
            const fiveStarPercentage = ((ratingData[4] / totalCount) * 100).toFixed(1) + "%";
            page.drawText(`Total Ratings: ${totalCount}`, { x: 50, y: 620, size: 14 });
            page.drawText(`Average Rating: ${avgRating}`, { x: 50, y: 600, size: 14 });
            page.drawText(`5-Star Percentage: ${fiveStarPercentage}`, { x: 50, y: 580, size: 14 });


            // Save and download the PDF
            try {
                const pdfBytes = await pdfDoc.save();
                const blob = new Blob([pdfBytes], { type: "application/pdf" });
                const url = URL.createObjectURL(blob);
                const link = document.createElement("a");
                link.href = url;
                link.download = "UserRatingsReport.pdf";
                link.click();
                URL.revokeObjectURL(url);
            } catch (error) {
                console.error("Error generating or downloading PDF:", error);
            }
        }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">User Ratings Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">
                    <button onclick="exportToPDF()" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" style="margin-right: 20px; width: 150px;">
                        <i
                            class="fas fa-download fa-sm text-white-50"></i>&nbsp;Generate Report</button>

                </td>
            </tr>
        </table>

        <div class="row justify-content-center">
            <div class="col-lg-12">
                <div class="button-group">
                    <asp:Button ID="Button2" CssClass="btn btn-success btn-lg" runat="server" Text="Overall" Width="99px" PostBackUrl="~/Admin/UserRatingReport.aspx" />
                    <asp:Button ID="Button3" CssClass="btn btn-success btn-lg" runat="server" Text="Monthly" Width="105px" Enabled="False" />
                    <asp:Button ID="Button4" CssClass="btn btn-success btn-lg" runat="server" Text="Yearly" Width="97px" PostBackUrl="~/Admin/YearlyUserRating.aspx" />
                </div>

                <div class="dropdown-list" style="margin-top: 20px; text-align: center;">
                    <h5>Select the Month & Year for the Monthly User Ratings Report</h5>
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                        <asp:ListItem>1</asp:ListItem>
                        <asp:ListItem>2</asp:ListItem>
                        <asp:ListItem>3</asp:ListItem>
                        <asp:ListItem>4</asp:ListItem>
                        <asp:ListItem>5</asp:ListItem>
                        <asp:ListItem>6</asp:ListItem>
                        <asp:ListItem>7</asp:ListItem>
                        <asp:ListItem>8</asp:ListItem>
                        <asp:ListItem>9</asp:ListItem>
                        <asp:ListItem>10</asp:ListItem>
                        <asp:ListItem>11</asp:ListItem>
                        <asp:ListItem>12</asp:ListItem>

                    </asp:DropDownList>
                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                        <asp:ListItem>2023</asp:ListItem>
                        <asp:ListItem>2024</asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- Pie Chart -->
                <div class="row justify-content-center" style="margin-top: 25px;">
                    <div class="col-lg-8">
                        <div class="card shadow mb-4">
                            <div class="card-header py-3">
                                <h6 class="m-0 font-weight-bold text-primary">Monthly User Rating Chart</h6>
                            </div>
                            <!-- Card Body -->
                            <div class="card-body">
                                <div class="chart-pie pt-4 pb-2">
                                    <canvas id="monthlyPieChart"></canvas>
                                </div>
                                <div class="mt-4 text-center small">
                                    <span class="mr-2">
                                        <i class="fas fa-circle text-primary"></i>5 stars
                                    </span>
                                    <span class="mr-2">
                                        <i class="fas fa-circle text-success"></i>4 stars
                                    </span>
                                    <span class="mr-2">
                                        <i class="fas fa-circle text-info"></i>3 stars
                                    </span>
                                    <span class="mr-2">
                                        <i class="fas fa-circle text-warning"></i>2 stars
                                    </span>
                                    <span class="mr-2">
                                        <i class="fas fa-circle text-danger"></i>1 star
                                    </span>
                                </div>
                            </div>
                        </div>

                    </div>
                    <!-- Summary Section -->
                    <div class="col-xl-3 col-lg-6 summary-section" style="margin-left: 15px; margin-bottom: 15px;">
                        <div class="card border-left-primary shadow h-100 py-2">
                            <div class="card-body">
                                <div class="row no-gutters align-items-center">
                                    <div class="col mr-2">
                                        <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">Total Ratings</div>
                                        <asp:Label ID="lblTotalRating" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                                    </div>
                                    <div class="col-auto">
                                        <i class="fas fa-clipboard-list fa-2x text-gray-300"></i>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card border-left-success shadow h-100 py-2">
                            <div class="card-body">
                                <div class="row no-gutters align-items-center">
                                    <div class="col mr-2">
                                        <div class="text-xs font-weight-bold text-success text-uppercase mb-1">Average Rating</div>
                                        <asp:Label ID="lblAvgRating" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                                    </div>
                                    <div class="col-auto">
                                        <i class="fas fa-star fa-2x text-gray-300"></i>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card border-left-info shadow h-100 py-2">
                            <div class="card-body">
                                <div class="row no-gutters align-items-center">
                                    <div class="col mr-2">
                                        <div class="text-xs font-weight-bold text-info text-uppercase mb-1">5-Star Percentage</div>
                                        <asp:Label ID="lblPercentage" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                                    </div>
                                    <div class="col-auto">
                                        <i class="fas fa-percentage fa-2x text-gray-300"></i>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>

        </div>
    </div>
   
</asp:Content>

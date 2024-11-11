<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserRatingReport.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.UserRatingReport" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


    <style type="text/css">
        .auto-style1 {
            width: 1241px;
        }

        .auto-style2 {
            width: 107px;
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

    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdf-lib/1.17.1/pdf-lib.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>

    <script src="/js/overallPieChart.js"></script>


<script type="text/javascript">
    var ratingData = <%= RatingDataJson %>; // Ensure this contains the rating data array
    console.log("Rating Data:", ratingData); // Check data in console to verify

    function initializePieChart() {
        if (!ratingData || ratingData.length === 0) {
            console.error("No valid rating data for chart creation.");
            return;
        }

        const ctx = document.getElementById("myPieChart").getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ["1 star", "2 stars", "3 stars", "4 stars", "5 stars"],
                datasets: [{
                    data: ratingData,
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
        initializePieChart(); // Initialize the chart if there's rating data
    });
</script>




</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">User Ratings Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">
                    <asp:HiddenField ID="chartImage" runat="server" />

                    <asp:Button ID="btnGenerate" runat="server" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" Text="Generate Report" style="margin-right: 20px; width: 150px;"  onClick="btnGenerate_Click"
                ></asp:Button>
                    <%--<button onclick="exportToPDF()" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" style="margin-right: 20px; width: 150px;">
                        <i class="fas fa-download fa-sm text-white-50"></i>&nbsp;Generate Report
                    </button>--%>
                                <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>

                </td>
            </tr>
        </table>
        <div class="row justify-content-center">
            <div class="col-lg-12">
                <div class="button-group">
                    <asp:Button ID="Button2" CssClass="btn btn-success btn-lg" runat="server" Text="Overall" Width="99px" Enabled="False" />
                    <asp:Button ID="Button3" CssClass="btn btn-success btn-lg" runat="server" Text="Monthly" Width="105px" PostBackUrl="~/Admin/MonthlyUserRating.aspx" />
                    <asp:Button ID="Button4" CssClass="btn btn-success btn-lg" runat="server" Text="Yearly" Width="97px" PostBackUrl="~/Admin/YearlyUserRating.aspx" />
                </div>

                <!-- Pie Chart -->
                <div class="row justify-content-center" style="margin-top: 25px;">
                    <div class="col-lg-8">
                        <div class="card shadow mb-4">
                            <div class="card-header py-3">
                                <h6 class="m-0 font-weight-bold text-primary">Overall User Rating Chart</h6>
                            </div>
                            <!-- Card Body -->
                            <div class="card-body">
                                <div class="chart-pie pt-4 pb-2">
                                    <canvas id="myPieChart"></canvas>
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
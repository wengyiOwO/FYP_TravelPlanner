<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script type="text/javascript">
        function populateChart(data) {
            const labels = data.map(item => item.Name);
            const visits = data.map(item => item.Visits);

            var ctx = document.getElementById('myBarChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Number of Visits',
                        data: visits,
                        backgroundColor: 'rgba(78, 115, 223, 0.5)',
                        borderColor: 'rgba(78, 115, 223, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Popular Destinations'
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Number of Visits'
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            display: false
                        }
                    }
                }
            });

            // Populate ranking list
            const topThree = data.slice(0, 3);
            const rankingList = document.getElementById("rankingList");
            rankingList.innerHTML = topThree.map((item, index) => `<li>${item.Name}</li>`).join('');
        }
   </script>
    <script src="/js/overallPieChart.js"></script>
    <script type="text/javascript">
        var ratingData = <%= RatingDataJson %>;


        function initializePieChart() {

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
            // Initialize the chart only if there's rating data
            initializePieChart();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Begin Page Content -->
    <div class="container-fluid">

        <!-- Content Row -->
        <div class="row">

            <!-- Number of Travel Plan (Monthly) Card Example -->
            <div class="col-xl-3 col-md-6 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                                    Number of Travel Plan (Monthly)
                                </div>
                                <asp:Label ID="lblMonthlyTP" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-file fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Number of Travel Plan (Annual) Card Example -->
            <div class="col-xl-3 col-md-6 mb-4">
                <div class="card border-left-success shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                                    Number of Travel Plan (Annual)
                                </div>
                                <asp:Label ID="lblAnnualTP" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-calendar fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Top 3 Popular Destinations Card Example -->
            <div class="col-xl-3 col-md-6 mb-4">
                <div class="card border-left-info shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
                                    Top 3 Popular Destinations
                                </div>
                                <div class="h5 mb-0 font-weight-bold text-gray-800">
                                    <div class="ranking-list">
                                        <ol id="rankingList"></ol>
                                    </div>
                                </div>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-clipboard-list fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Average Ratings Card Example -->
            <div class="col-xl-3 col-md-6 mb-4">
                <div class="card border-left-warning shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <div class="col mr-2">
                                <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                                    Average Ratings
                                </div>
                                <asp:Label ID="lblAvgRating" runat="server" CssClass="h5 mb-0 font-weight-bold text-gray-800" Visible="true"></asp:Label>
                            </div>
                            <div class="col-auto">
                                <i class="fas fa-star fa-2x text-gray-300"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Content Row -->
        <div class="row">

            <!-- Bar Chart -->
            <div class="col-xl-8 col-lg-7">
                <div class="card shadow mb-4">
                    <!-- Card Header - Dropdown -->
                    <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between">
                        <h6 class="m-0 font-weight-bold text-primary">Popular Destinations Overview</h6>
                        <div class="dropdown no-arrow">
                            <a class="dropdown-toggle" href="#" role="button" id="dropdownMenuLink"
                                data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class="fas fa-ellipsis-v fa-sm fa-fw text-gray-400"></i>
                            </a>
                            <div class="dropdown-menu dropdown-menu-right shadow animated--fade-in"
                                aria-labelledby="dropdownMenuLink">
                                <div class="dropdown-header">Action:</div>
                                <a class="dropdown-item" href="PopularDestinationReport.aspx">View Details</a>
                            </div>
                        </div>
                    </div>
                    <!-- Card Body -->
                    <div class="card-body">
                        <div class="chart-area" style="height: 500px;">
                            <canvas id="myBarChart" style="max-height: 100%;"></canvas>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Pie Chart -->
            <div class="col-xl-4 col-lg-5">
                <div class="card shadow mb-4">
                    <!-- Card Header - Dropdown -->
                    <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between">
                        <h6 class="m-0 font-weight-bold text-primary">User Ratings</h6>
                        <div class="dropdown no-arrow">
                            <a class="dropdown-toggle" href="#" role="button" id="dropdownMenuLink1"
                                data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class="fas fa-ellipsis-v fa-sm fa-fw text-gray-400"></i>
                            </a>
                            <div class="dropdown-menu dropdown-menu-right shadow animated--fade-in"
                                aria-labelledby="dropdownMenuLink">
                                <div class="dropdown-header">Action:</div>
                                <a class="dropdown-item" href="UserRatingReport.aspx">View Details</a>
                            </div>
                        </div>
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
        </div>

    </div>

    <!-- End of Main Content -->

</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YearlyPopularDestinations.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.js.demo.YearlyPopularDestinations" %>

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


        .ranking-list ol {
            padding-left: 20px;
            margin: 0;
            font-size: 16px;
        }

        .ranking-list li {
            margin-bottom: 10px;
        }

        .chart-container {
            padding: 20px;
        }

        .card-header {
            background-color: #f8f9fc;
            border-bottom: 1px solid #e3e6f0;
        }

        .card-title {
            margin-bottom: 0;
            font-weight: bold;
            color: #4e73df;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script type="text/javascript">
        var destinationData = <%= DestinationDataJson %>;
        console.log("Destination Data:", destinationData);
        function generateBarChart() {
            const ctx = document.getElementById("myBarChart").getContext("2d");

            const labels = destinationData.map(d => d.location_name);
            const dataCounts = destinationData.map(d => d.visit_count);

            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'Top Destinations',
                        data: dataCounts,
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

            const rankingList = document.getElementById("rankingList");
            const top8Destinations = destinationData.slice(0, 8); // Get top 8 destinations

            rankingList.innerHTML = top8Destinations
                .map((d, index) => `<li>${d.location_name} - ${d.visit_count} visits</li>`)
                .join('');
        }


        document.addEventListener("DOMContentLoaded", function () {
            generateBarChart();
        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">Popular Destinations Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">
                    <asp:Button ID="btnGenerate" runat="server" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" Text="Generate Report" Style="margin-right: 20px; width: 150px;" OnClick="btnGenerate_Click"></asp:Button>

                    <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>
                </td>
            </tr>
        </table>

        <div class="row justify-content-center">
            <div class="col-lg-12">
                <div class="button-group">
                    <asp:Button ID="Button2" CssClass="btn btn-success btn-lg" runat="server" Text="Overall" Width="99px" PostBackUrl="~/Admin/PopularDestinationReport.aspx" />
                    <asp:Button ID="Button3" CssClass="btn btn-success btn-lg" runat="server" Text="Monthly" Width="105px" PostBackUrl="~/Admin/MonthlyPopularDestinations.aspx" />
                    <asp:Button ID="Button4" CssClass="btn btn-success btn-lg" runat="server" Text="Yearly" Width="97px" Enabled="false" />
                </div>
                <div class="dropdown-list" style="margin-top: 20px; text-align: center;">
                    <h5>Select the Year for the Annual Popular Destinations Report</h5>

                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                        <asp:ListItem>2023</asp:ListItem>
                        <asp:ListItem>2024</asp:ListItem>
                        <asp:ListItem></asp:ListItem>
                    </asp:DropDownList>
                </div>


                <div class="row">
                    <!-- Bar Chart Section -->
                    <div class="col-xl-8 col-lg-7 chart-container">
                        <div class="card shadow mb-4">
                            <div class="card-header py-3">
                                <h6 class="card-title">Annual Popular Destinations</h6>
                            </div>
                            <div class="card-body">
                                <div class="chart-area" style="height: 500px;">
                                    <canvas id="myBarChart" style="max-height: 100%;"></canvas>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Top 8 Ranking Section -->
                    <div class="col-xl-4 col-lg-5 chart-container">
                        <div class="card shadow mb-4">
                            <div class="card-header py-3">
                                <h6 class="card-title">Top 8 Popular Destinations</h6>
                            </div>
                            <div class="card-body">
                                <div class="ranking-list">
                                    <ol id="rankingList"></ol>
                                    <!-- Dynamically populated -->
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

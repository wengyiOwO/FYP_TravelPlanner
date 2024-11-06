<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopularDestinationReport.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.PopularDestinationReport" %>

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
            margin-bottom: 20px;
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">Popular Destinations Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">

                    <a href="#" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" style="margin-right: 20px; width: 150px;"><i
                        class="fas fa-download fa-sm text-white-50"></i>&nbsp;Generate Report</a>
                </td>
            </tr>
        </table>

        <div class="button-group">
            <asp:Button ID="Button2" CssClass="btn btn-success btn-lg" runat="server" Text="Overall" Width="99px" Enabled="False" />
            <asp:Button ID="Button3" CssClass="btn btn-success btn-lg" runat="server" Text="Monthly" Width="105px" PostBackUrl="~/Admin/MonthlyPopularDestinations.aspx" />
            <asp:Button ID="Button4" CssClass="btn btn-success btn-lg" runat="server" Text="Yearly" Width="97px" PostBackUrl="~/Admin/YearlyPopularDestinations.aspx" />
        </div>

        <div class="row">
            <!-- Bar Chart Section -->
            <div class="col-xl-8 col-lg-7 chart-container">
                <div class="card shadow mb-4">
                    <div class="card-header py-3">
                        <h6 class="card-title">Overall Popular Destinations</h6>
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
                            <ol>
                                <li>Pulau Langkawi</li>
                                <li>Desaru Resort</li>
                                <li>Sunway Lagoon</li>
                                <li>Legoland</li>
                                <li>Pulau Redang</li>
                                <li>Escape Park</li>
                                <li>A'Famosa</li>
                                <li>KLCC</li>
                            </ol>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="/js/overallBarChart.js"></script>
</asp:Content>

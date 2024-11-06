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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">User Ratings Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">
                    <a href="#" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" style="margin-right: 20px; width: 150px;"><i
                        class="fas fa-download fa-sm text-white-50"></i>&nbsp;Generate Report</a>

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

                <div class="dropdown-list" style="margin-top: 20px; text-align:center;">
                    <h5>Select the Month & Year for the Monthly User Ratings Report</h5>
                    <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True">
                        <asp:ListItem>1</asp:ListItem>
                        <asp:ListItem>2</asp:ListItem>
                        <asp:ListItem>3</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True">
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
                                        <div class="h5 mb-0 font-weight-bold text-gray-800">115</div>
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
                                        <div class="h5 mb-0 font-weight-bold text-gray-800">4.2</div>
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
                                        <div class="h5 mb-0 font-weight-bold text-gray-800">47%</div>
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
    <script src="/js/overallPieChart.js"></script>
</asp:Content>

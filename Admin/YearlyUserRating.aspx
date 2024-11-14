<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="YearlyUserRating.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.YearlyUserRating" %>


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
        <script src="/js/YearlyPieChart.js"></script>

    <script type="text/javascript">
        var ratingData = <%= RatingDataJson %>; 
</script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/xlsx/0.16.9/xlsx.full.min.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <h1 class="h3 mb-2 text-gray-800">User Ratings Report</h1>

        <table style="width: 100%;">
            <tr>
                <td class="auto-style1">&nbsp;</td>
                <td class="auto-style2">

    <asp:Button ID="btnGenerate" runat="server" class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" Text="Generate Report" style="margin-right: 20px; width: 150px;"  onClick="btnGenerate_Click"
></asp:Button>
   
                <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>
                </td>
            </tr>
        </table>

        <div class="row justify-content-center">
            <div class="col-lg-12">
                <div class="button-group">
                    <asp:Button ID="Button2" CssClass="btn btn-success btn-lg" runat="server" Text="Overall" Width="99px" PostBackUrl="~/Admin/UserRatingReport.aspx" />
                    <asp:Button ID="Button3" CssClass="btn btn-success btn-lg" runat="server" Text="Monthly" Width="105px" PostBackUrl="~/Admin/MonthlyUserRating.aspx" />
                    <asp:Button ID="Button4" CssClass="btn btn-success btn-lg" runat="server" Text="Yearly" Width="97px" Enabled="False" />
                </div>
                <div class="dropdown-list" style="margin-top: 20px; text-align: center;">
                    <h5>Select the Year for the Annual User Ratings Report</h5>

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
                                <h6 class="m-0 font-weight-bold text-primary">Annual User Rating Chart</h6>
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
      <script type="text/javascript">
          function exportToExcel() {
              // Calculate total count, average rating, and 5-star percentage
              var totalCount = ratingData.reduce((a, b) => a + b, 0);
              var avgRating = (ratingData.reduce((sum, value, index) => sum + value * (index + 1), 0) / totalCount).toFixed(1);
              var fiveStarPercentage = ((ratingData[4] / totalCount) * 100).toFixed(1) + "%";

              // Prepare worksheet data
              const worksheetData = [
                  ["User Ratings Report"],
                  [],
                  ["Rating", "Count"],
                  ["1 star", ratingData[0]],
                  ["2 stars", ratingData[1]],
                  ["3 stars", ratingData[2]],
                  ["4 stars", ratingData[3]],
                  ["5 stars", ratingData[4]],
                  [],
                  ["Total Ratings", totalCount],
                  ["Average Rating", avgRating],
                  ["5-Star Percentage", fiveStarPercentage]
              ];

              // Create a workbook and worksheet
              const workbook = XLSX.utils.book_new();
              const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
              XLSX.utils.book_append_sheet(workbook, worksheet, "User Ratings");

              // Export the workbook to an Excel file
              XLSX.writeFile(workbook, "UserRatingsReport.xlsx");
          }
  </script>
</asp:Content>

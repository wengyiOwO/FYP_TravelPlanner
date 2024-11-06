<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTravelPlanHistory.aspx.cs" MasterPageFile="~/TakeMyTrip.Master" Inherits="FYP_TravelPlanner.Traveller.ViewTravelPlanHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 171px;
        }

        .auto-style4 {
            width: 199px;
            height: 39px;
        }

        .auto-style6 {
            width: 215px;
            height: 39px;
            text-align: center;
        }

        .auto-style7 {
            width: 272px;
        }

        .auto-style9 {
            width: 100%;
            margin: 0 auto;
            padding: 1.5rem;
            background-color: white;
        }

        .auto-style12 {
            width: 199px;
        }

        .auto-style13 {
            width: 321px;
        }

        .auto-style14 {
            height: 39px;
            width: 321px;
        }

        .auto-style15 {
            width: 336px;
        }

        .container {
            display: flex;
            flex-direction: column;
            height: 600px;
            background-color: white;
            overflow: hidden;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 20px;
        }

            .header h1 {
                margin: 0;
            }

        .card-body {
            width: 100%;
            max-width: 1200px;
            padding: 20px;
            height: calc(100% - 60px);
            overflow-y: auto;
        }

        .table-container {
            height: 100%;
            overflow-y: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th, td {
            padding: 10px;
            border: 1px solid #ddd;
            text-align: center;
        }

        .btn-view, .btn-delete {
            margin: 0 5px;
            padding: 5px 10px;
            font-size: 14px;
            color: #fff;
            text-decoration: none;
        }

        .btn-view {
            background-color: #007bff;
            border: none;
            cursor: pointer;
        }

            .btn-view:hover {
                background-color: #0056b3;
            }

        .btn-delete {
            background-color: #dc3545;
            border: none;
            cursor: pointer;
        }

            .btn-delete:hover {
                background-color: #c82333;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="auto-style9">
        <div class="container">
            <div class="header">
                <h1 class="h3 mb-2 text-gray-800">Travel Plan History</h1>
                <a href="CreateTravelPlan.aspx" class="btn btn-primary btn-lg" role="button">New Travel Plan</a>
            </div>
            <div class="card-body">
                <div class="table-container">
                    <div class="table-responsive">
                        <table class="table table-bordered" id="dataTable" cellspacing="0">
                            <thead>
                                <tr>
                                    <th class="auto-style6">Action</th>
                                    <th class="auto-style12">Date</th>
                                    <th class="auto-style12">Duration</th>
                                    <th class="auto-style15">Area</th>
                                    <th class="auto-style7">Budget(RM)</th>
                                    <th class="auto-style1">Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptTravelPlans" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <!-- Hidden field to hold plan_id -->
                                            <asp:HiddenField ID="hfPlanId" runat="server" Value='<%# Eval("plan_id") %>' />

                                            <td class="auto-style6">
                                                <a href='<%# Eval("plan_id", "TravelPlan.aspx?tp={0}") %>' class="btn-view">View</a>
                                                <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-delete"
                                                    OnClick="btnDelete_Click" OnClientClick="return confirm('Are you sure you want to delete this travel plan?');" />
                                            </td>
                                            <td><%# Eval("plan_date", "{0:dd-MM-yyyy}") %></td>
                                            <td><%# Eval("duration") %></td>
                                            <td><%# Eval("area_name") %></td>
                                            <td><%# Eval("budget") %></td>
                                            <td><%# Eval("status") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

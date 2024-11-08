<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/TakeMyTrip.Master" CodeBehind="Feedback.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.Feedback" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        .feedback-messages {
            display: flex;
            flex-direction: column;
            height: 600px;
            overflow: hidden;
            padding: 1rem;
            background-color: #f8f9fa;
            border-radius: 5px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }

        .feedback-message-left {
            display: flex;
            margin-bottom: 1rem;
            padding: 1rem;
            border-radius: 10px;
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

            .feedback-message-left:nth-child(even) {
                background-color: #f1f1f1;
            }

            .feedback-message-left img {
                margin-right: 1rem;
                border-radius: 50%;
                border: 2px solid #dee2e6;
            }

        .rating-stars i {
            font-size: 1.5rem;
            color: #FFD700;
            margin-right: 2px;
        }

        .filter-section {
            margin-top: 20px;
            margin-bottom: 20px;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .feedback-message-left:hover {
            background-color: #e9ecef;
            box-shadow: 0 6px 12px rgba(0,0,0,0.15);
            transition: all 0.3s ease;
        }

        .text-nowrap {
            white-space: nowrap;
        }

        @media (max-width: 768px) {
            .feedback-message-left {
                flex-direction: column;
                align-items: flex-start;
            }

                .feedback-message-left img {
                    margin-bottom: 0.5rem;
                }
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="content">
        <div class="card">
            <div class="row g-0">
                <div class="col-lg-11">
                    <div class="position-relative">
                        <div class="text-center mb-4">
                        <h4><asp:Label ID="lblOverallRating" runat="server" CssClass="font-weight-bold"></asp:Label></h4>
                        <div id="overallRatingStars" runat="server" class="rating-stars">
                        </div>
                    </div>
                        <div class="filter-section">
                            <label for="ratingFilter" class="mr-3">Filter by Rating:</label>
                            <asp:DropDownList ID="ratingFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ratingFilter_SelectedIndexChanged" CssClass="form-control w-auto">
                                <asp:ListItem Value="all" Text="All" />
                                <asp:ListItem Value="5" Text="5 Stars" />
                                <asp:ListItem Value="4" Text="4 Stars" />
                                <asp:ListItem Value="3" Text="3 Stars" />
                                <asp:ListItem Value="2" Text="2 Stars" />
                                <asp:ListItem Value="1" Text="1 Star" />
                            </asp:DropDownList>
                        </div>


                        <div class="feedback-messages">
                            <asp:Repeater ID="rptFeedback" runat="server">
                                <ItemTemplate>
                                    <div class="feedback-message-left">
                                        <img src="https://bootdey.com/img/Content/avatar/avatar3.png" alt='<%# Eval("account_name") %>' width="40" height="40">
                                        <div>
                                            <div class="font-weight-bold mb-1"><%# Eval("account_name") %></div>
                                            <div class="rating-stars">
                                                <%# GenerateStars((int)Eval("rating")) %>
                                            </div>
                                            <div><%# Eval("review") %></div>
                                            <div class="text-muted small text-nowrap mt-2"><%# Eval("rating_date", "{0:MM/dd/yyyy}") %></div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Label ID="lblMessage" runat="server" Text="No Reviews yet." CssClass="text-center text-muted mt-3" Visible="false"></asp:Label>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


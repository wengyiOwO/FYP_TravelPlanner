<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="Friends.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.Friends" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Friends - TakeMyTrip</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        body {
            margin-top: 0px;
            padding-top: 0px;
        }

        .content {
            padding-top: 0px; /* Ensuring no padding on top */
            min-height: 800px;
        }

        .card {
            min-height: 800px; /* Ensuring the card height fills the space */
            display: flex;
            flex-direction: column;
        }

        .row {
            flex-grow: 1;
        }

        .chat-online {
            color: #34ce57;
        }

        .chat-offline {
            color: #e4606d;
        }

        .chat-messages {
            display: flex;
            flex-direction: column;
            min-height: 500px;
            max-height: 500px;
            overflow-y: scroll;
        }

        .chat-message-left,
        .chat-message-right {
            display: flex;
            flex-shrink: 0;
        }

        .chat-message-left {
            margin-right: auto;
        }

        .chat-message-right {
            flex-direction: row-reverse;
            margin-left: auto;
        }

        .py-3 {
            padding-top: 1rem !important;
            padding-bottom: 1rem !important;
        }

        .px-4 {
            padding-right: 1.5rem !important;
            padding-left: 1.5rem !important;
        }

        .flex-grow-0 {
            flex-grow: 0 !important;
        }

        .border-top {
            border-top: 1px solid #dee2e6 !important;
        }

        .position-relative:hover .btn {
            opacity: 1.0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="card">
            <div class="row g-0">
                <!-- Friend List Section -->
                <div class="col-12 col-lg-5 col-xl-3 border-right">
                    <div class="px-4 d-none d-md-block">
                        <div class="d-flex align-items-center">
                            <div class="flex-grow-1">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control my-3" placeholder="Search..."></asp:TextBox>
                            </div>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary ml-2" Text="Search" OnClick="btnSearch_Click" />
                        </div>
                    </div>
                   <asp:Repeater ID="rptFriendsList" runat="server" OnItemCommand="rptFriendsList_ItemCommand">
    <ItemTemplate>
        <div class="d-flex align-items-start position-relative mb-3 ml-5"> <!-- Add margin-bottom and margin-left -->
            <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" OnDataBinding="imgProfile_DataBinding" />
            <div class="flex-grow-1 ml-3">
                <%# Eval("account_name") %>
            </div>
            <asp:LinkButton ID="btnSelectFriend" runat="server" 
                CommandName="SelectFriend" 
                CommandArgument='<%# Eval("account_id") %>' 
                CssClass="btn btn-link position-absolute w-100 h-100" 
                style="top: 0; left: 0; z-index: 1; opacity: 0;">
                Select
            </asp:LinkButton>
        </div>
    </ItemTemplate>
</asp:Repeater>
                </div>

                <!-- Friend Request Section -->
                <div class="col-12 col-lg-7 col-xl-9">
                    <div class="py-2 px-4 border-bottom d-none d-lg-block">
                        <div class="d-flex align-items-center py-1">
                            <div class="flex-grow-1 pl-3">
                                <strong>New Friend Requests</strong>
                            </div>
                            <div>
                                <a href="AddFriend.aspx" class="btn btn-primary btn-lg" role="button">Add New Friend</a>
                            </div>
                        </div>
                    </div>
                    <asp:Repeater ID="rptFriendRequests" runat="server" OnItemCommand="rptFriendRequests_ItemCommand">
                        <ItemTemplate>
                            <a href="#" class="list-group-item list-group-item-action border-0">
                                <div class="d-flex align-items-start">
                                    <asp:Image ID="imgProfile" runat="server" Width="40" Height="40" CssClass="rounded-circle mr-1" OnDataBinding="imgProfile_DataBinding" />
                                    <div class="flex-grow-1 ml-3">
                                        <%# Eval("account_name") %>
                                    </div>
                                    <asp:Button ID="btnAccept" runat="server" CssClass="btn btn-primary btn-lg mr-1 px-3" CommandName="Accept" CommandArgument='<%# Eval("account_id") %>' Text="Accept" />
                                    <asp:Button ID="btnReject" runat="server" CssClass="btn btn-danger btn-lg ml-2" CommandName="Reject" CommandArgument='<%# Eval("account_id") %>' Text="Reject" />
                                </div>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </main>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="AddFriend.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.AddFriend" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Add New Friend - TakeMyTrip</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        body {
            margin-top: 0px;
            padding-top: 0px;
        }

        .content {
            padding-top: 0px;
            min-height: 800px;
            display: flex;
            flex-direction: column;
        }

        .card {
            flex-grow: 1;
            display: flex;
            flex-direction: column;
            min-height: 800px;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="content">
        <div class="card">
            <div class="row g-0">
                <div class="col-12">
                    <div class="py-2 px-4 d-none d-lg-block">
                        <div class="d-flex align-items-center py-1 border-bottom">
                            <div class="flex-grow-1 pl-3">
                                <strong>Add New Friend</strong>
                            </div>
                            <div class="d-flex align-items-center">
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control mr-2" Placeholder="Enter account name..."></asp:TextBox>
                                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                            </div>
                            </div>
                            <!-- Repeater for Search Results -->
                            <div class="py-4 px-4">
                                <asp:Repeater ID="rptResults" runat="server" OnItemCommand="rptResults_ItemCommand">
                                    <ItemTemplate>
                                        <a href="#" class="list-group-item list-group-item-action border-0">
                                            <div class="d-flex align-items-start">
                                                <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle" Width="40" Height="40"
                                                    OnDataBinding="imgProfile_DataBinding" />
                                                <div class="flex-grow-1 ml-3">
                                                    <asp:Label ID="lblAccountName" runat="server" Text='<%# Eval("account_name") %>'></asp:Label>
                                                </div>
                                                <asp:Button ID="btnAddFriend" runat="server" CssClass="btn btn-primary btn-sm" Text="Add" CommandName="AddFriend" CommandArgument='<%# Eval("account_id") %>' />
                                            </div>
                                        </a>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Label ID="lblNoResults" runat="server" CssClass="text-muted mt-3" Text="No user found." Visible="false"></asp:Label>
                                </div>
                            </div>
                    </div>
            </div>
        </div>
    </main>
</asp:Content>

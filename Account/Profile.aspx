<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="FYP_TravelPlanner.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Profile with Followers and Cover Photo - Bootdey.com</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        body {
            padding-top: 20px;
            background-color: #f1f5f9;
        }

        .card {
            border: 0;
            border-radius: 0.5rem;
            box-shadow: 0 2px 4px rgba(0,0,20,.08), 0 1px 2px rgba(0,0,20,.08);
        }

        .card-body {
            padding: 0.5rem;
        }

        .avatar-xxl {
            height: 7.5rem;
            width: 7.5rem;
        }

        .avatar-sm {
            height: 2rem;
            width: 2rem;
        }

        .avatar-online::before {
            background-color: #198754;
        }

        .avatar-indicators::before {
            border: 2px solid #FFF;
            border-radius: 50%;
            height: 30%;
            width: 30%;
        }

        .rounded-circle {
            border-radius: 50% !important;
        }

        .shadow-none {
            box-shadow: none !important;
        }

        .py-6 {
            padding: 1.5rem !important;
        }

        .bg-gray-300 {
            background-color: #cbd5e1 !important;
        }

        .mt-n10 {
            margin-top: -3rem !important;
        }

        .mb-4 {
            margin-bottom: 1rem !important;
        }

        .mb-6 {
            margin-bottom: 1.5rem !important;
        }

        .mt-n7 {
            margin-top: -1.75rem !important;
        }

        .me-2 {
            margin-right: 0.5rem !important;
        }

        .post-card {
            position: relative;
            width: 100%;
            padding-top: 75%; 
            border-radius: 0.5rem;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

            .post-card img {
                position: absolute;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                object-fit: cover; 
                object-position: center; 
            }

            .post-card .card-body {
                position: absolute;
                bottom: 0;
                width: 100%;
                height: 30%;
                background-color: rgba(0, 0, 0, 0.6); 
                color: #fff;
                display: flex;
                align-items: center;
                justify-content: center;
                text-align: center;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="row align-items-center mt-4">
            <div class="col-12">
                <div class="pt-20 rounded-top" style="background: url(https://bootdey.com/image/480x480/00FFFF/000000) no-repeat; background-size: cover;">
                </div>
                <div class="card rounded-bottom smooth-shadow-sm">
                    <div class="d-flex align-items-center justify-content-between pt-4 pb-6 px-4">
                        <div class="d-flex align-items-center">
                            <asp:Image ID="imgProfile" runat="server" CssClass="avatar-xxl rounded-circle border border-2" />
                            <div class="lh-1 ms-3">
                                <asp:Label ID="lblAccountName" runat="server" CssClass="h2 mb-0"></asp:Label>
                            </div>
                        </div>
                        <div id="friendButtonContainer" runat="server" class="d-flex justify-content-end">
                            <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-outline-primary" Text="Edit Profile" OnClick="btnEdit_Click" Visible="false" />
                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary me-2" Text="Add Friend" OnClick="btnAdd_Click" Visible="false" />
                            <asp:Button ID="btnSent" runat="server" CssClass="btn btn-secondary me-2" Text="Friend Request Sent" Enabled="false" Visible="false" />
                            <asp:Button ID="btnAccept" runat="server" CssClass="btn btn-success me-2" Text="Accept" OnClick="btnAccept_Click" Visible="false" />
                            <asp:Button ID="btnReject" runat="server" CssClass="btn btn-danger me-2" Text="Reject" OnClick="btnReject_Click" Visible="false" />
                            <asp:Button ID="btnUnfriend" runat="server" CssClass="btn btn-warning" Text="Unfriend" OnClick="btnUnfriend_Click" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="py-6">
            <div class="row row-cols-1 row-cols-md-4 g-4">
                <asp:Repeater ID="PostsRepeater" runat="server">
                    <ItemTemplate>
                        <div class="col post-row">
                            <div class="post-card">
                                <a href="/Traveller/PostDetails.aspx?post_id=<%# Eval("post_id") %>">
                                    <img src='<%# ResolveUrl("~/Uploads/Images/") + Eval("post_id") + "_1.jpg" %>' alt="Post image" />
                                    <div class="card-body">
                                        <h5 class="card-title"><%# Eval("post_title") %></h5>
                                    </div>
                                </a>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    <script src="https://code.jquery.com/jquery-1.10.2.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>

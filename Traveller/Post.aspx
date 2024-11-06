<%@ Page Title="Posts" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="Post.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.Post" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .posts-container {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 10px;
        }

     
        .post-card {
            position: relative;
            width: 100%;
            padding-top: 75%;
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

        .card-body {
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

        .post-row {
            display: flex;
            justify-content: center;
            padding: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container my-2 posts-container">
        <div class="px-4 d-none d-md-block">
            <div class="d-flex align-items-center">
                <div class="flex-grow-1">
                    <input type="text" class="form-control my-3" placeholder="Search...">
                </div>
                <div class="input-group-append">
                    <button class="btn btn-primary" type="button">
                        <i class="fas fa-search fa-sm"></i>
                    </button>
                </div>
            </div>
        </div>

        <div class="row row-cols-1 row-cols-md-4 g-4">
            <asp:Repeater ID="PostsRepeater" runat="server">
                <ItemTemplate>
                    <div class="col post-row">
                        <div class="card post-card">
                            <a href="PostDetails.aspx?post_id=<%# Eval("post_id") %>">
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
</asp:Content>

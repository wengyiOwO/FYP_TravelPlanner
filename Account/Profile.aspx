<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="FYP_TravelPlanner.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Profile with Followers and Cover Photo - Bootdey.com</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/css/bootstrap.min.css" rel="stylesheet">
        <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">

    <style type="text/css">
        body {
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

        .card-title {
            max-width: 100%;
            word-wrap: break-word;
            white-space: normal;
            overflow-wrap: break-word;
        }

             .modal-dialog {
         height: 30vh; 
         max-height: 30vh;
     }

     .modal-content {
         height: 100%; 
         display: flex;
         flex-direction: column;
     }

     .modal-body {
         overflow-y: auto; 
         flex-grow: 1; 
     }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <asp:Panel ID="pnlDeletedMessage" runat="server" Visible="false" CssClass="alert alert-danger text-center my-5">
            <asp:Label ID="lblDeletedMessage" runat="server" Text="Invalid user"></asp:Label>
            <div class="mt-3">
                <a href="../Traveller/Friends.aspx" class="btn btn-primary">Return to Friend List</a>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlProfileDetails" runat="server" Visible="true">
            <div class="row align-items-center mt-4">
                <asp:Panel ID="successPanel" runat="server" CssClass="alert alert-success" Visible="false">
                    <strong>Success!</strong> Your account has been deleted. Redirecting to the login page...
                </asp:Panel>
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
                            <div class="d-flex justify-content-end mb-3">

                                <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-outline-danger btn-sm mr-2" Text="Delete Account" OnClientClick="showDeleteConfirmation(); return false;" />
                                <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-outline-primary d-flex justify-content-end" Text="Edit Profile" OnClick="btnEdit_Click" Visible="false" />
                            </div>

                            <asp:Panel ID="pnlConfirmDelete" runat="server" CssClass="modal" Style="display: none;">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title">Confirm Delete</h5>
                                            <button type="button" class="close" onclick="hideDeleteConfirmation();">&times;</button>
                                        </div>
                                        <div class="modal-body">
                                            <p>Are you sure you want to delete this account?</p>
                                        </div>
                                        <div class="modal-footer">
                                            <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" Text="Delete" OnClick="btnConfirmDelete_Click" />
                                            <button type="button" class="btn btn-secondary" onclick="hideDeleteConfirmation();">Cancel</button>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <div id="friendButtonContainer" runat="server" class="d-flex justify-content-end">
                                <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary me-2" Text="Add Friend" OnClick="btnAdd_Click" Visible="false" />
                                <asp:Button ID="btnSent" runat="server" CssClass="btn btn-secondary me-2" Text="Friend Request Sent" Enabled="false" Visible="false" />
                                <asp:Button ID="btnAccept" runat="server" CssClass="btn btn-success me-2" Text="Accept" OnClick="btnAccept_Click" Visible="false" />
                                <asp:Button ID="btnReject" runat="server" CssClass="btn btn-danger me-2" Text="Reject" OnClick="btnReject_Click" Visible="false" />
                                <asp:Button ID="btnUnfriend" runat="server" CssClass="btn btn-danger" Text="Unfriend" OnClick="btnUnfriend_Click" Visible="false" />
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
        </asp:Panel>
    </div>

    <script src="https://code.jquery.com/jquery-1.10.2.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.2.0/dist/js/bootstrap.bundle.min.js"></script>
     <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js"></script>
 <script>
     function showDeleteConfirmation() {
         document.getElementById('<%= pnlConfirmDelete.ClientID %>').style.display = 'block';
     }
     function hideDeleteConfirmation() {
         document.getElementById('<%= pnlConfirmDelete.ClientID %>').style.display = 'none';
     }
     </script>
</asp:Content>

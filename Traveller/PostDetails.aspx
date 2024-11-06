<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="PostDetails.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.PostDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        .image-container {
            width: 100%; 
            height: 600px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: #333; 
        }

        .image-container img,
        .image-container video {
            max-width: 100%;
            max-height: 100%;
            object-fit: contain;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-0">
        <div class="row no-gutters">
            <div class="col-md-12">
                <div class="card">
                    <div class="card-body">
                        <asp:Panel runat="server">
                            <div class="d-flex justify-content-end mb-3">
                                <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-outline-primary btn-sm mr-2" Text="Edit" OnClick="btnEdit_Click" />
                                <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-outline-danger btn-sm" Text="Delete" OnClick="btnDelete_Click" />
                            </div>
                        </asp:Panel>

                        <div class="d-flex align-items-center mb-3">
                            <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle" Width="50" Height="50" alt="User Profile" />
                            <div class="ml-3 ms-3">
                                <asp:Label ID="lblAuthorName" runat="server" CssClass="mb-0 h5" Text="Author"></asp:Label><br />
                                <asp:Label ID="lblPostDate" runat="server" CssClass="text-muted" Text="15/8/2024 20:30:03"></asp:Label>
                            </div>
                        </div>

                        <asp:Label ID="lblPostTitle" runat="server" CssClass="h6 mb-3" Text="Post Title"></asp:Label><br />
                        <asp:Literal ID="ltPostContent" runat="server"></asp:Literal>

                        <asp:Panel ID="postImagesPanel" runat="server">
                            <div id="postImagesCarousel" class="carousel slide" data-ride="carousel">
                                <div class="carousel-inner" runat="server" id="carouselInner">
                                    <!-- Literal control for video rendering -->
                                    <asp:Literal ID="videoLiteral" runat="server"></asp:Literal>
                                </div>

                                <!-- Conditional rendering for carousel controls -->
                                <asp:PlaceHolder ID="carouselControls" runat="server">
                                    <a class="carousel-control-prev" href="#postImagesCarousel" role="button" data-slide="prev">
                                        <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                                        <span class="sr-only">Previous</span>
                                    </a>
                                    <a class="carousel-control-next" href="#postImagesCarousel" role="button" data-slide="next">
                                        <span class="carousel-control-next-icon" aria-hidden="true"></span>
                                        <span class="sr-only">Next</span>
                                    </a>
                                </asp:PlaceHolder>
                            </div>
                        </asp:Panel>

                        <hr>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
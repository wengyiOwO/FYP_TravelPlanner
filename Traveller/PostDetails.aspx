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
                    <asp:Panel ID="pnlDeletedMessage" runat="server" Visible="false" CssClass="alert alert-danger text-center my-5">
                        <asp:Label ID="lblDeletedMessage" runat="server" Text="Invalid Post"></asp:Label>
                        <div class="mt-3">
                            <a href="Post.aspx" class="btn btn-primary">Return to Posts</a>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlPostDetails" runat="server" Visible="true">

                        <div class="card-body">
                            <asp:Panel ID="successPanel" runat="server" CssClass="alert alert-success" Visible="false">
                                <strong>Success!</strong> Your post has been deleted. Redirecting to the post page...
                            </asp:Panel>
                            <asp:Panel runat="server">
                                <div class="d-flex justify-content-end mb-3">
                                    <asp:Button ID="btnEdit" runat="server" CssClass="btn btn-outline-primary btn-sm mr-2" Text="Edit" OnClick="btnEdit_Click" />
                                    <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-outline-danger btn-sm" Text="Delete" OnClientClick="showDeleteConfirmation(); return false;" />
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlConfirmDelete" runat="server" CssClass="modal" Style="display: none;">
                                <div class="modal-dialog">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title">Confirm Delete</h5>
                                            <button type="button" class="close" onclick="hideDeleteConfirmation();">&times;</button>
                                        </div>
                                        <div class="modal-body">
                                            <p>Are you sure you want to delete this post?</p>
                                        </div>
                                        <div class="modal-footer">
                                            <asp:Button ID="btnConfirmDelete" runat="server" CssClass="btn btn-danger" Text="Delete" OnClick="btnConfirmDelete_Click" />
                                            <button type="button" class="btn btn-secondary" onclick="hideDeleteConfirmation();">Cancel</button>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>
                            <div class="d-flex align-items-center mb-3">
                                <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle" Width="50" Height="50" alt="User Profile" />
                                <div class="ml-3 ms-3">
                                    <asp:Label ID="lblAuthorName" runat="server" CssClass="mb-0 h5" Text="Author"></asp:Label><br />
                                    <asp:Label ID="lblPostDate" runat="server" CssClass="text-muted" Text="15/8/2024 20:30:03"></asp:Label>
                                </div>

                            </div>

                            <asp:Label ID="lblPostTitle" runat="server" CssClass="h4 mb-5" Text="Post Title"></asp:Label><br />
                            <asp:Literal ID="ltPostContent" runat="server"></asp:Literal>

                            <asp:Panel ID="postImagesPanel" runat="server" CssClass="mt-5">
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
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.3/dist/umd/popper.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function showDeleteConfirmation() {
            document.getElementById('<%= pnlConfirmDelete.ClientID %>').style.display = 'block';
        }
        function hideDeleteConfirmation() {
            document.getElementById('<%= pnlConfirmDelete.ClientID %>').style.display = 'none';
        }
    </script>
</asp:Content>

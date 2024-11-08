<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="EditProfile.aspx.cs" Inherits="FYP_TravelPlanner.EditProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Edit Profile - Account Details</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.1/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        body {
            margin-top: 20px;
            background-color: #f2f6fc;
            color: #69707a;
        }
        .img-account-profile {
            width: 150px;
            height: 150px;
            object-fit: cover;
            border-radius: 50%;
        }
        .card {
            box-shadow: 0 0.15rem 1.75rem 0 rgb(33 40 50 / 15%);
        }
        .card .card-header {
            font-weight: 500;
        }
        .form-control, .dataTable-input {
            width: 100%;
            padding: 0.875rem 1.125rem;
            font-size: 0.875rem;
            font-weight: 400;
            line-height: 1;
            color: #69707a;
            background-color: #fff;
            border: 1px solid #c5ccd6;
            border-radius: 0.35rem;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }
    </style>
    <script type="text/javascript">
        function previewProfileImage(input) {
            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    document.getElementById('<%= imgProfile.ClientID %>').src = e.target.result;
                }
                reader.readAsDataURL(input.files[0]);
            }
        }

        function refreshProfileImage() {
            let imgProfile = document.getElementById('<%= imgProfile.ClientID %>');
            imgProfile.src = imgProfile.src.split("?")[0] + "?t=" + new Date().getTime();
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-xl px-4 mt-4">
        <div class="row">
            <div class="col-xl-4">
                <div class="card mb-4 mb-xl-0">
                    <div class="card-header">Profile Picture</div>
                    <div class="card-body text-center">
                        <asp:Image ID="imgProfile" runat="server" CssClass="img-account-profile mb-2" />
                        <asp:FileUpload ID="ProfileImageUpload" runat="server" CssClass="form-control mb-2" OnChange="previewProfileImage(this)" />
                        <br />
                        <asp:Button ID="UploadProfileImage" runat="server" Text="Upload new image" CssClass="btn btn-primary" OnClick="UploadProfileImage_Click" />
                    </div>
                </div>
            </div>
            <div class="col-xl-8">
                <div class="card mb-4">
                    <div class="card-header">Account Details</div>
                    <div class="card-body">
                        <asp:Label ID="StatusMessage" runat="server" ForeColor="Red" />                        
                        <div class="mb-3">
                            <label class="small mb-1" for="inputName">Name</label>
                            <asp:TextBox ID="inputName" runat="server" CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <label class="small mb-1" for="inputEmailAddress">Email address</label>
                            <asp:TextBox ID="inputEmailAddress" runat="server" CssClass="form-control" TextMode="Email" Enabled="false" />
                        </div>
                        <div class="mb-3">
                            <label class="small mb-1" for="inputPhone">Phone number</label>
                            <asp:TextBox ID="inputPhone" runat="server" CssClass="form-control" TextMode="Phone" />
                        </div>

                        <asp:Button ID="SaveChanges" runat="server" Text="Save changes" CssClass="btn btn-primary" OnClick="SaveChanges_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="CreatePost.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.CreatePost" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
       #uploadWrapper {
    position: relative;
    width: 150px;
    height: 150px;
    background-color: #f0f0f0;
    border: 2px dashed #cccccc;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
}

#uploadWrapper::before {
    content: '+'; 
    font-size: 48px; 
    color: #999999; 
    position: absolute;
}

        #imageUpload {
            position: absolute;
            width: 100%;
            height: 100%;
            opacity: 0;
            cursor: pointer;
        }

        .image-preview-container {
            position: relative;
            width: 150px;
            height: 150px;
            margin-left: 20px;
            display: none;
        }

        #imagePreview {
            width: 100%;
            height: 100%;
            object-fit: cover;
            border: 1px solid #cccccc;
        }

        .delete-button {
            position: absolute;
            top: -10px;
            right: -10px;
            background-color: #ff6666;
            color: white;
            border-radius: 50%;
            border: none;
            width: 25px;
            height: 25px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            cursor: pointer;
        }

        .upload-section {
            display: flex;
            align-items: center;
        }

        .visibility-options {
            margin: 20px 0;
        }

            .visibility-options label {
                margin-right: 15px;
            }

        #previewContainer {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }

        .image-preview-container {
            position: relative;
            width: 150px;
            height: 150px;
            margin-left: 0;
            display: inline-block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-0">
        <div class="card">
            <div class="card-body">
                <h5 class="card-title"><strong>Create Post</strong></h5>
                <div class="mb-3 upload-section">
                    <div id="uploadWrapper">
    <asp:FileUpload ID="fileUpload" runat="server" accept="image/*" AllowMultiple="true" Style="opacity:0; width:100%; height:100%; position:absolute; cursor:pointer;" />
</div>
<div id="previewContainer" class="d-flex flex-wrap mt-3"></div>
                    <!-- Container for all image previews -->
                </div>
                <div class="mb-3">
                    <label for="postTitle" class="form-label">Title</label>
                    <asp:TextBox ID="txtPostTitle" runat="server" CssClass="form-control" placeholder="Enter title, you might get more likes~" />
                </div>

                <div class="mb-3">
                    <label for="postDescription" class="form-label">Description</label>
                    <asp:TextBox ID="txtPostContent" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Provide a detailed description to help more people see your post!" />
                </div>

                <div class="visibility-options">
                    <label>Who can view:</label>
                    <asp:RadioButtonList ID="rblPostPermission" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="Public" Text="Public" Selected="True" />
                        <asp:ListItem Value="Friend" Text="Friend" />
                        <asp:ListItem Value="Owner" Text="Owner Only" />
                    </asp:RadioButtonList>
                </div>

                <asp:Button ID="btnCreatePost" runat="server" CssClass="btn btn-primary" Text="Create Post" OnClick="btnCreatePost_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.getElementById('<%= fileUpload.ClientID %>').onchange = function (event) {
            const previewContainer = document.getElementById('previewContainer');
            previewContainer.innerHTML = ''; // Clear existing previews

            Array.from(event.target.files).forEach((file, index) => {
                const fileType = file.type.split('/')[0];
                const previewDiv = document.createElement('div');
                previewDiv.classList.add('image-preview-container');

                if (fileType === 'image') {
                    // Display image preview
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.classList.add('img-thumbnail');
                        img.style.width = '100%';
                        img.style.height = '100%';
                        img.style.objectFit = 'cover';

                        previewDiv.appendChild(img);
                    };
                    reader.readAsDataURL(file);
                } else if (fileType === 'video') {
                    // Display video preview
                    const video = document.createElement('video');
                    video.src = URL.createObjectURL(file);
                    video.classList.add('img-thumbnail');
                    video.controls = true;
                    video.style.width = '100%';
                    video.style.height = '100%';
                    video.style.objectFit = 'cover';

                    previewDiv.appendChild(video);
                }

                // Add delete button to remove file from preview and upload
                const deleteButton = document.createElement('button');
                deleteButton.classList.add('delete-button');
                deleteButton.innerHTML = 'x';
                deleteButton.onclick = function () {
                    previewDiv.remove();
                    const files = Array.from(event.target.files);
                    files.splice(index, 1);
                    event.target.files = new FileList(...files);
                };

                previewDiv.appendChild(deleteButton);
                previewContainer.appendChild(previewDiv);
            });
        };

        document.getElementById('deleteButton').onclick = function () {
            const previewContainer = document.getElementById('imagePreviewContainer');
            const imageUpload = document.getElementById('<%= fileUpload.ClientID %>');
            const preview = document.getElementById('imagePreview');

            imageUpload.value = ''; 
            preview.src = '';
            previewContainer.style.display = 'none';
        };
    </script>
</asp:Content>

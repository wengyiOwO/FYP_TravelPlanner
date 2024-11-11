<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="EditPost.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.EditPost" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        /* Same styles as CreatePost.aspx */
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

        .image-preview-container, .video-preview-container {
            position: relative;
            width: 150px;
            height: 150px;
            margin: 10px;
            display: inline-block;
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
            gap: 10px;
            flex-wrap: wrap;
        }

        .visibility-options {
            margin: 20px 0;
        }

        .visibility-options label {
            margin-right: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-0">
        <div class="card">
            <div class="card-body">
                <h5 class="card-title"><strong>Edit Post</strong></h5>

                <div class="mb-3 upload-section">
                    <div id="uploadWrapper">
                        <asp:FileUpload ID="fileUpload" runat="server" accept="image/*,video/*" AllowMultiple="true" Style="opacity:0; width:100%; height:100%; position:absolute; cursor:pointer;" />
                    </div>
                    <div id="previewContainer" class="d-flex flex-wrap mt-3">
                                            <asp:Literal ID="previewLiteral" runat="server"></asp:Literal>

                        <!-- Existing image and video previews will be added here -->
                    </div>
                </div>

                <div class="mb-3">
                    <label for="postTitle" class="form-label">Title</label>
                    <asp:TextBox ID="txtPostTitle" runat="server" CssClass="form-control" placeholder="Enter title" />
                </div>

                <div class="mb-3">
                    <label for="postDescription" class="form-label">Description</label>
                    <asp:TextBox ID="txtPostContent" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Enter description" />
                </div>

                <div class="visibility-options">
                    <label>Who can view:</label>
                    <asp:RadioButtonList ID="rblPostPermission" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="Public" Text="Public" />
                        <asp:ListItem Value="Friend" Text="Friend" />
                        <asp:ListItem Value="Owner" Text="Owner Only" />
                    </asp:RadioButtonList>
                </div>

                <asp:Button ID="btnUpdatePost" runat="server" CssClass="btn btn-primary" Text="Update Post" OnClick="btnUpdatePost_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.getElementById('<%= fileUpload.ClientID %>').onchange = function (event) {
            const previewContainer = document.getElementById('previewContainer');
            previewContainer.innerHTML = ''; // Clear existing previews

            Array.from(event.target.files).forEach((file) => {
                const fileType = file.type.split('/')[0];
                const previewDiv = document.createElement('div');
                previewDiv.classList.add(fileType === 'image' ? 'image-preview-container' : 'video-preview-container');

                if (fileType === 'image') {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.style.width = '100%';
                        img.style.height = '100%';
                        img.style.objectFit = 'cover';
                        previewDiv.appendChild(img);
                    };
                    reader.readAsDataURL(file);
                } else if (fileType === 'video') {
                    const video = document.createElement('video');
                    video.src = URL.createObjectURL(file);
                    video.controls = true;
                    video.style.width = '100%';
                    video.style.height = '100%';
                    video.style.objectFit = 'cover';
                    previewDiv.appendChild(video);
                }

                // Delete button to remove files
                const deleteButton = document.createElement('button');
                deleteButton.classList.add('delete-button');
                deleteButton.innerHTML = 'x';
                deleteButton.onclick = function () {
                    previewDiv.remove();
                };
                previewDiv.appendChild(deleteButton);

                previewContainer.appendChild(previewDiv);
            });
        };

        function removeFile(fileUrl) {
            // Use AJAX to notify server to delete the file or handle in backend after form submission
            const previewContainer = document.getElementById('previewContainer');
            const previewDiv = Array.from(previewContainer.children).find(div => {
                return div.querySelector("img[src='" + fileUrl + "'], video[src='" + fileUrl + "']");
            });
            if (previewDiv) previewDiv.remove();
        }
    </script>
</asp:Content>

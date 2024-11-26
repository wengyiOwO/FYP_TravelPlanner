<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.Chat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Chat - Take My Trip</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@4.5.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <style type="text/css">
        .content {
            min-height: 500px;
        }

       

        .chat-online {
            color: #34ce57;
        }

        .chat-offline {
            color: #e4606d;
        }

        .chat-messages {
            height: calc(100vh - 200px); /* Adjust based on header/footer size */
            overflow-y: auto;
        }

            /* Always scroll to bottom for chat messages */
            .chat-messages::-webkit-scrollbar {
                width: 8px;
            }

            .chat-messages::-webkit-scrollbar-thumb {
                background-color: #888;
                border-radius: 4px;
            }

                .chat-messages::-webkit-scrollbar-thumb:hover {
                    background: #555;
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

            .chat-message-left img,
            .chat-message-right img {
                max-width: 100%; /* Allow the image to scale responsively */
                max-height: 300px; /* Limit the maximum height of images */
                width: auto; /* Keep the image's aspect ratio */
                height: auto;
                object-fit: contain; /* Ensure the image fits within the container */
            }

            /* Video size */
            .chat-message-left video,
            .chat-message-right video {
                max-width: 100%; /* Allow the video to scale responsively */
                max-height: 300px; /* Limit the maximum height of videos */
                width: auto; /* Keep the video's aspect ratio */
                height: auto;
            }

        .text-right {
            text-align: right;
        }

        .bg-light {
            border-radius: 10px;
            padding: 8px;
        }

        .chat-section {
            display: flex;
            flex-direction: column;
            height: 100%;
        }

            .chat-section.visible {
                display: block;
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
            flex-shrink: 0;
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
                <!-- Friend List Section -->
                <div class="col-12 col-lg-5 col-xl-3 border-right">
                    <div class="px-4 d-none d-md-block friend-list">
                        <div class="d-flex align-items-center">
                            <div class="flex-grow-1">
                                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control my-3" placeholder="Search..."></asp:TextBox>
                            </div>
                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary ml-2" Text="Search" OnClick="btnSearch_Click" />
                        </div>
                    </div>
                    <asp:Repeater ID="rptFriendsList" runat="server" OnItemCommand="rptFriendsList_ItemCommand">
                        <ItemTemplate>
                            <div class="d-flex align-items-start position-relative mb-3 ml-5">
                                <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" ImageUrl='<%# ResolveUrl("~/Uploads/Profile/" + Eval("profile_image", "{0}")) %>' AlternateText="Profile Image" />
                                <div class="flex-grow-1 ml-3">
                                    <%# Eval("account_name") %>
                                </div>
                                <asp:LinkButton ID="btnSelectFriend" runat="server"
                                    CommandName="SelectFriend"
                                    CommandArgument='<%# Eval("account_id") %>'
                                    CssClass="btn btn-link position-absolute w-100 h-100"
                                    Style="top: 0; left: 0; z-index: 1; opacity: 0;">
                                    Select
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <!-- Chat Section -->
                <div class="col-12 col-lg-7 col-xl-9 chat-section<%= ViewState["SelectedFriendId"] != null ? " visible" : "" %>">
                    <asp:Panel ID="chatPanel" runat="server" CssClass="" Visible="false">

                        <div class="py-2 px-4 border-bottom d-none d-lg-block">
                            <div class="d-flex align-items-center py-1">
                                <!-- Selected Friend Image -->
                                <div class="position-relative">
                                    <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" ImageUrl='<%# Eval("profile_image", "{0}") %>' AlternateText="Profile Image" />
                                </div>
                                <div class="flex-grow-1 pl-3">
                                    <strong>
                                        <asp:Literal ID="litSelectedFriendName" runat="server"></asp:Literal></strong>
                                </div>
                            </div>
                        </div>
                        <div class="position-relative chat-messages p-4">
                            <asp:Repeater ID="rptChatMessages" runat="server" OnItemDataBound="rptChatMessages_ItemDataBound">
                                <ItemTemplate>
                                    <div class='<%# Eval("sender_id").ToString() == Session["account_id"].ToString() ? "chat-message-right" : "chat-message-left" %> pb-4'>
                                        <div>
                                            <asp:Image ID="imgMessageProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" />
                                            <div class="text-muted small text-nowrap mt-2">
                                                <asp:Literal ID="litTime" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <div class="flex-shrink-1 bg-light rounded py-2 px-3">
                                            <div class="font-weight-bold mb-1">
                                                <asp:Literal ID="litSender" runat="server"></asp:Literal>
                                            </div>
                                            <asp:PlaceHolder ID="phMessageContent" runat="server"></asp:PlaceHolder>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <div class="flex-grow-0 py-3 px-4 border-top">
                            <div class="input-group">
                                <!-- Text Input for Message -->
                                <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Type your message"></asp:TextBox>

                                <!-- File Upload Button -->
                                <asp:FileUpload ID="fileUpload" runat="server" accept="image/*,video/*" AllowMultiple="true" CssClass="btn btn-light" Style="cursor: pointer;" onchange="previewFiles(event)" />

                                <!-- Send Button -->
                                <asp:Button ID="btnSend" runat="server" CssClass="btn btn-primary" Text="Send" OnClick="btnSend_Click" />
                            </div>

                            <!-- Preview Container for Images and Videos -->
                            <div id="previewContainer" class="d-flex flex-wrap mt-3"></div>
                        </div>
                    </asp:Panel>
                </div>

            </div>
        </div>
    </main>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script>
        document.getElementById('<%= txtMessage.ClientID %>').addEventListener('input', function () {
            document.getElementById('<%= btnSend.ClientID %>').disabled = this.value.trim() === '';
        });

        let accountId = '<%= Session["account_id"] %>';
        let selectedFriendId = '<%= ViewState["SelectedFriendId"] ?? "" %>';

        function loadMessages() {
            if (!selectedFriendId) return;

            $.ajax({
                type: "POST",
                url: "Chat.aspx/GetChatMessages",
                data: JSON.stringify({ accountId: accountId, friendId: selectedFriendId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    const messages = JSON.parse(response.d);
                    const chatContainer = $(".chat-messages");
                    chatContainer.empty();

                    messages.forEach((msg) => {
                        const align = msg.sender_id === accountId ? "chat-message-right" : "chat-message-left";
                        const profileImg = msg.profile_image ? "../Uploads/Profile/" + msg.profile_image : "../Uploads/Profile/unknown.jpg";
                        const senderName = msg.sender_id === accountId ? "You" : msg.sender_name;
                        const messageTime = msg.formatted_time;

                        let messageContent = "";

                        if (msg.message_type === "text") {
                            messageContent = "<div>" + msg.chat_message + "</div>";
                        } else if (msg.message_type === "image") {
                            const imageUrl = "../Uploads/Chat/" + msg.chat_message;
                            messageContent = '<img src="' + imageUrl + '" class="img-fluid" alt="Image Message" />';
                        } else if (msg.message_type === "video") {
                            const videoUrl = "../Uploads/Chat/" + msg.chat_message;
                            messageContent = '<video src="' + videoUrl + '" class="w-100" controls></video>';
                        }

                        const messageHtml = `<div class="${align} pb-4">
                    <div>
                        <img src="${profileImg}" class="rounded-circle mr-1" style="width: 40px; height: 40px;" alt="${senderName}" />
                        <div class="text-muted small text-nowrap mt-2">${messageTime}</div>
                    </div>
                    <div class="flex-shrink-1 bg-light rounded py-2 px-3">
                        <div class="font-weight-bold mb-1">${senderName}</div>
                        ${messageContent}
                    </div>
                </div>`;

                        chatContainer.append(messageHtml);
                    });

                    // Scroll to bottom
                    chatContainer.scrollTop(chatContainer[0].scrollHeight);
                },
                error: function () {
                    console.error("Error loading messages");
                },
            });
        }


        // Poll for new messages every 1 seconds
        if (selectedFriendId) {
            setInterval(loadMessages, 1000);
        }

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
                        img.style.width = '100px';
                        img.style.height = '100px';
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
                    video.style.width = '100px';
                    video.style.height = '100px';
                    video.style.objectFit = 'cover';

                    previewDiv.appendChild(video);
                }

                // Add delete button to remove file from preview
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
    </script>
</asp:Content>

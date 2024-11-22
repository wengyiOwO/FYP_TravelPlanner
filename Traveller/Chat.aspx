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
            flex-grow: 1;
            overflow-y: auto; /* Scroll if content overflows */
            padding: 20px;
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
                order: -1;
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
                    <div class="px-4 d-none d-md-block">
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
                        <asp:Repeater ID="rptChatMessages" runat="server">
                            <ItemTemplate>
                                <div class='<%# Eval("sender_id").ToString() == Session["account_id"].ToString() ? "chat-message-right" : "chat-message-left" %> pb-4'>
                                    <div>
                                        <asp:Image ID="imgMessageProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" ImageUrl='<%# ResolveUrl("~/Uploads/Profile/" + Eval("profile_image", "{0}")) %>' AlternateText="Profile Image" />
                                        <div class="text-muted small text-nowrap mt-2"><%# Eval("chat_datetime", "{0:hh:mm tt}") %></div>
                                    </div>
                                    <div class="flex-shrink-1 bg-light rounded py-2 px-3">
                                        <div class="font-weight-bold mb-1">
                                            <%# Eval("sender_id").ToString() == Session["account_id"].ToString() ? "You" : Eval("sender_name") %>
                                        </div>
                                        <%# Eval("chat_message") %>
                                    </div>

                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="flex-grow-0 py-3 px-4 border-top">
                        <div class="input-group">
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Type your message"></asp:TextBox>
                            <asp:Button ID="btnSend" runat="server" CssClass="btn btn-primary" Text="Send" OnClick="btnSend_Click" />
                        </div>
                    </div>
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

                        const messageHtml = `<div class="${align} pb-4">
                        <div>
                            <img src="${profileImg}" class="rounded-circle mr-1" style="width: 40px; height: 40px;" alt="${senderName}" />
                            <div class="text-muted small text-nowrap mt-2">${messageTime}</div>
                        </div>
                        <div class="flex-shrink-1 bg-light rounded py-2 px-3">
                            <div class="font-weight-bold mb-1">
                                ${senderName}
                            </div>
                            <div>${msg.chat_message}</div>
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
    </script>
</asp:Content>

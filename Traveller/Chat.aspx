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
            flex-grow: 1; /* Expands to take up available space */
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
                                <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" OnDataBinding="imgProfile_DataBinding" />
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
                            <div class="position-relative">
                                <asp:Image ID="imgProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" OnDataBinding="imgProfile_DataBinding" />
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
                                <div class='<%# Eval("sender_id").ToString() == "AC0521" ? "chat-message-right" : "chat-message-left" %> pb-4'>
                                    <div>
                                        <asp:Image ID="imgMessageProfile" runat="server" CssClass="rounded-circle mr-1" Width="40" Height="40" OnDataBinding="imgProfile_DataBinding" />
                                        <div class="text-muted small text-nowrap mt-2"><%# Eval("chat_datetime", "{0:hh:mm tt}") %></div>
                                    </div>
                                    <div class="flex-shrink-1 bg-light rounded py-2 px-3">
                                        <div class="font-weight-bold mb-1">
                                            <%# Eval("sender_id").ToString() == "AC0521" ? "You" : Eval("sender_name") %>
                                        </div>
                                        <%# Eval("chat_message") %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="flex-grow-0 py-3 px-4 border-top">
                        <div class="input-group">
                            <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Type your message" AutoPostBack="true"></asp:TextBox>
                            <asp:Button ID="btnSend" runat="server" CssClass="btn btn-primary" Text="Send" OnClick="btnSend_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
    <script>
        document.getElementById('<%= txtMessage.ClientID %>').addEventListener('input', function () {
            document.getElementById('<%= btnSend.ClientID %>').disabled = this.value.trim() === '';
        });
    </script>
</asp:Content>

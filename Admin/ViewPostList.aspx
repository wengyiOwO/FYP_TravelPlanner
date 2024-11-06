<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewPostList.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.ViewPostList" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .gridview-style {
            width: 100%;
            border: 1px solid black;
            border-collapse: collapse;
            margin-top: 20px;
            margin-bottom: 20px;
        }

            .gridview-style th,
            .gridview-style td {
                border: 1px solid black;
                padding: 10px;
                text-align: center;
            }

            .gridview-style th {
                color: black;
            }

                .gridview-style th a {
                    color: black;
                    text-decoration: none;
                }

            .gridview-style td a {
                color: cornflowerblue;
                ;
            }

            .gridview-style tr {
                border: 1px solid black;
            }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <!-- Page Heading -->
        <h1 class="h3 mb-2 text-gray-800">Manage Posts</h1>


        <div class="card shadow mb-4">
            <div class="card-header py-3">
                <h3 class="m-0 font-weight-bold text-primary">Post List</h3>
            </div>
            <div class="col-lg-12">

                <p style="margin-left: 15px; margin-top: 20px;">
                    <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                    &nbsp;
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-success" OnClick="btnSearch_Click" />
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Visible="false"></asp:Label>

                </p>
                <div class="card-body">
                    <div class="table-responsive">
                        <asp:GridView ID="GridView1" runat="server" CssClass="gridview-style" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="post_id" DataSourceID="SqlDataSource1" OnRowUpdating="GridView_RowUpdating">
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="True" />

                                <asp:BoundField DataField="post_id" HeaderText="ID" ReadOnly="True" SortExpression="post_id" />
                                <asp:TemplateField HeaderText="Image">
                                    <ItemTemplate>
                                        <asp:Image ID="imgPost" runat="server"
                                            ImageUrl='<%# ResolveUrl("~/Uploads/Images/") + Eval("post_id") + "_1.jpg" %>'
                                            AlternateText="Post image" Width="100px" Height="100px" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" SortExpression="post_date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("post_date", "{0:dd/MM/yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("post_date", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox3"
                                            Display="Dynamic" ErrorMessage="Please Fill In Date!" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegExValidator3" runat="server" ControlToValidate="TextBox3"
                                            ValidationExpression="^(0[1-9]|[12][0-9]|3[01])/(0[1-9]|1[0-2])/([0-9]{4})$"
                                            ErrorMessage="Date must be in dd/MM/yyyy format." ForeColor="Red"></asp:RegularExpressionValidator>
                                        <asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="TextBox3"
                                            ErrorMessage="Date cannot exceeds today's date" ForeColor="Red" OnServerValidate="ValidateDate"></asp:CustomValidator>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Title" SortExpression="post_title">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("post_title") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2" Display="Dynamic" ErrorMessage="Please Fill In Title!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("post_title") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Content" SortExpression="post_content">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox6" runat="server" Text='<%# Bind("post_content") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TextBox6" Display="Dynamic" ErrorMessage="Please Fill In Content!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label6" runat="server" Text='<%# Bind("post_content") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" SortExpression="post_status">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox4" runat="server" Text='<%# Bind("post_status") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox4" Display="Dynamic" ErrorMessage="Please Fill In Status!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Bind("post_status") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Permission" SortExpression="post_permission">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlPermission" runat="server" SelectedValue='<%# Bind("post_permission") %>'>
                                            <asp:ListItem Text="Public" Value="Public"></asp:ListItem>
                                            <asp:ListItem Text="Friends" Value="Friends"></asp:ListItem>
                                            <asp:ListItem Text="Owner" Value="Owner"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddlPermission" Display="Dynamic" ErrorMessage="Please Select Permission!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("post_permission") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [post_id], [account_id], [post_date], [post_title], [post_content], [post_status], [post_permission] FROM [Posts] ORDER BY [post_id]" DeleteCommand="DELETE FROM [Posts] WHERE [post_id] = @post_id" UpdateCommand="UPDATE [Posts] SET [post_title] = @post_title,  [post_date] = @post_date , [post_content] = @post_content , [post_status] = @post_status, [post_permission] = @post_permission WHERE [post_id] = @post_id">
                            <DeleteParameters>
                                <asp:Parameter Name="post_id" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="post_title" />
                                <asp:Parameter Name="post_date" />
                                <asp:Parameter Name="post_content" />
                                <asp:Parameter Name="post_status" />
                                <asp:Parameter Name="post_permission" />
                                <asp:Parameter Name="post_id" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>


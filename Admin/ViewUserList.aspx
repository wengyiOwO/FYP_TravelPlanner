<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewUserList.aspx.cs" MasterPageFile="~/TravelPlanner_Admin.Master" Inherits="FYP_TravelPlanner.ViewUserList" %>

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

    <div class="auto-style9">

        <!-- Page Heading -->
        <h1 class="h3 mb-2 text-gray-800">Manage Users</h1>


        <div class="card shadow mb-4">
            <div class="card-header py-3">
                <h3 class="m-0 font-weight-bold text-primary">User List</h3>
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

                        <asp:GridView ID="GridView1" runat="server" CssClass="gridview-style" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="account_id" DataSourceID="SqlDataSource1" OnRowDeleting="GridView1_RowDeleting">
                            <Columns>
                                <asp:CommandField HeaderText="Action" ShowDeleteButton="True" ShowEditButton="True" />

                                <asp:BoundField DataField="account_id" HeaderText="ID" ReadOnly="True" SortExpression="account_id" />
                                <asp:TemplateField HeaderText="Name" SortExpression="account_name">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("account_name") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" Display="Dynamic" EnableViewState="False" ErrorMessage="Please Enter Name!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("account_name") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Phone Number" SortExpression="account_phoneNo">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("account_phoneNo") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox3" Display="Dynamic" ErrorMessage="Please Fill In Phone Number!" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="TextBox3" Display="Dynamic" ErrorMessage="Invalid phone number format(Eg.0102049999)" ForeColor="Red" ValidationExpression="^\d{10,11}$"></asp:RegularExpressionValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("account_phoneNo") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Email" SortExpression="account_email">
                                    <EditItemTemplate>
                                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("account_email") %>'></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2" Display="Dynamic" ErrorMessage="Please Fill In Email!" ForeColor="Red"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TextBox2" Display="Dynamic" ErrorMessage="Invalid Email Format!" ForeColor="Red" ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"></asp:RegularExpressionValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("account_email") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status" SortExpression="account_status">
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlStatus" runat="server" SelectedValue='<%# Bind("account_status") %>'>
                                            <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddlStatus" Display="Dynamic" ErrorMessage="Please Select Permission!" ForeColor="Red"></asp:RequiredFieldValidator>
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Bind("account_status") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT [account_id], [account_name], [account_phoneNo], [account_email], [account_status] FROM [Account] ORDER BY [account_id]" DeleteCommand="DELETE FROM [ACCOUNT] WHERE [account_id] = @account_id" UpdateCommand="UPDATE [Account] SET [account_name] = @account_name, [account_phoneNo] = @account_phoneNo, [account_email] = @account_email , [account_status] = @account_status WHERE [account_id] = @account_id
">
                            <DeleteParameters>
                                <asp:Parameter Name="account_id" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="account_name" />
                                <asp:Parameter Name="account_phoneNo" />
                                <asp:Parameter Name="account_email" />
                                <asp:Parameter Name="account_status" />
                                <asp:Parameter Name="account_id" />
                            </UpdateParameters>
                        </asp:SqlDataSource>
                    </div>
                </div>
            </div>
        </div>

    </div>



</asp:Content>


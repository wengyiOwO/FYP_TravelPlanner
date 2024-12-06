<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/TakeMyTrip_Anonymous.Master" CodeBehind="Login.aspx.cs" Inherits="FYP_TravelPlanner.Login" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    <div class="container">

        <!-- Outer Row -->
        <div class="row justify-content-center">

            <div class="col-xl-10 col-lg-12 col-md-9">

                <div class="card o-hidden border-0 shadow-lg my-5">
                    <div class="card-body p-0">
                        <!-- Nested Row within Card Body -->
                        <div class="row">
                            <div class="col-lg-6 d-none d-lg-block bg-login-image">
                                <img src="img/logo.png" width="520" />

                            </div>
                            <div class="col-lg-6">
                                <div class="p-5">
                                    <div class="text-center">
                                        <h1 class="h4 text-gray-900 mb-4">Welcome Back!</h1>
                                    </div>
                                    <div class="user">
                                        <div class="form-group">
                                            <asp:TextBox runat="server" type="email" class="form-control form-control-user"
                                                ID="inputEmail" aria-describedby="emailHelp"
                                                placeholder="Enter Email Address..." />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="inputEmail" ErrorMessage="Email is required" CssClass="text-danger" Display="Dynamic" />
                                            <asp:RegularExpressionValidator runat="server" ControlToValidate="inputEmail" ErrorMessage="Invalid email format (Eg. example@gmail.com)" CssClass="text-danger"
                                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" Display="Dynamic" />
                                        </div>
                                        <div class="form-group">
                                            <asp:TextBox runat="server" type="password" class="form-control form-control-user"
                                                ID="inputPassword" placeholder="Password" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="inputPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" />
                                            <asp:RegularExpressionValidator runat="server" ControlToValidate="inputPassword"
                                                ErrorMessage="Password must be at least 8 characters long, and include at least one uppercase letter, one lowercase letter, one number, and one special character."
                                                CssClass="text-danger"
                                                ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#+])[A-Za-z\d@$!%*?&#+]{8,16}$" Display="Dynamic" />
                                        </div>
                                        <div class="form-group">
                                            <div class="custom-control custom-checkbox small">
                                                <asp:CheckBox ID="customCheck" runat="server" />
                                                <label for="customCheck">Remember Me</label>
                                            </div>
                                        </div>
                                        <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-user btn-block" Text="Login" OnClick="btnLogin_Click" />

                                        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

                                        <hr>
                                    </div>

                                    <div class="text-center">
                                        <asp:HyperLink runat="server" CssClass="nav-link" Text="Forgot Password?" NavigateUrl="~/ForgotPassword.aspx" />
                                    </div>
                                    <div class="text-center">
                                        <asp:HyperLink runat="server" Text="Create an Account!" CssClass="nav-link" NavigateUrl="~/Register.aspx" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </div>

    </div>


</asp:Content>

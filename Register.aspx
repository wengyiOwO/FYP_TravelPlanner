<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/TakeMyTrip_Anonymous.Master" CodeBehind="Register.aspx.cs" Inherits="FYP_TravelPlanner.Register" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container">
        <div class="card o-hidden border-0 shadow-lg my-5">
            <div class="card-body p-0">
                <!-- Nested Row within Card Body -->
                <div class="row">
                    <div class="col-lg-5 d-none d-lg-block bg-register-image">
                        <img src="img/logo.png" width="520" />
                    </div>
                    <div class="col-lg-7">
                        <div class="p-5">
                            <div class="text-center">
                                <h1 class="h4 text-gray-900 mb-4">Create an Account!</h1>
                            </div>
                            <div class="user">
                                <div class="form-group row">
                                    <div class="col-sm-6 mb-3 mb-sm-0">
                                        <asp:TextBox runat="server" type="text" class="form-control form-control-user" ID="txtName" placeholder="Your Name" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="Name is required" CssClass="text-danger" />
                                    </div>
                                    <div class="col-sm-6">
                                        <asp:TextBox runat="server" type="text" class="form-control form-control-user" ID="txtPhoneNum" placeholder="0123456789" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPhoneNum" ErrorMessage="Phone number is required" CssClass="text-danger" />
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtPhoneNum" Display="Dynamic" ErrorMessage="Invalid phone number format(Eg.0102049999)" ForeColor="Red" ValidationExpression="^\d{10,11}$"></asp:RegularExpressionValidator>

                                    </div>
                                </div>
                                <div class="form-group">

                                    <div>
                                        <asp:TextBox runat="server" type="email" class="form-control form-control-user" ID="txtEmail" placeholder="Email Address" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required" CssClass="text-danger" />
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid email format" CssClass="text-danger"
                                            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" />
                                    </div>
                                </div>
                                <div class="form-group row">
                                    <div class="col-sm-6 mb-3 mb-sm-0 position-relative">
                                        <asp:TextBox runat="server" type="password" class="form-control form-control-user" ID="txtPassword" placeholder="Password" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" />
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPassword"
                                            ErrorMessage="Password must be at least 8 characters long, and include at least one uppercase letter, one lowercase letter, one number, and one special character."
                                            CssClass="text-danger"
                                            ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#+])[A-Za-z\d@$!%*?&#+]{8,}$" />
                                    </div>
                                    <div class="col-sm-6 position-relative">
                                        <asp:TextBox runat="server" type="password" class="form-control form-control-user" ID="txtRepeatPassword" placeholder="Repeat Password" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRepeatPassword" ErrorMessage="Please confirm your password" CssClass="text-danger" />
                                        <asp:CompareValidator runat="server" ControlToValidate="txtRepeatPassword" ControlToCompare="txtPassword" ErrorMessage="Passwords do not match" CssClass="text-danger" />

                                    </div>
                                </div>
                                <asp:Button runat="server" CssClass="btn btn-primary btn-user btn-block" Text="Register" ID="btnRegister" OnClick="btnRegister_Click" />
                                <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>
                                <asp:TextBox runat="server" type="hidden" ID="txtAccID" />
                                <hr>
                                <div class="text-center">
                                    <a class="small" href="ForgotPassword.aspx">Forgot Password?</a>
                                </div>
                                <div class="text-center">
                                    <a class="small" href="Login.aspx">Already have an account? Login!</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>





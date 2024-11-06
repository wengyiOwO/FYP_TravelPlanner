<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/TakeMyTrip_Anonymous.Master" CodeBehind="ForgotPassword.aspx.cs" Inherits="FYP_TravelPlanner.ForgotPassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <div class="container">

        <!-- Outer Row -->
        <div class="row justify-content-center">

            <div class="col-xl-10 col-lg-12 col-md-9">

                <div class="card o-hidden border-0 shadow-lg my-5">
                    <div class="card-body p-0">
                        <!-- Nested Row within Card Body -->
                        <div class="row">
                            <div class="col-lg-6 d-none d-lg-block bg-password-image">
                                <img src="img/logo.png" width="520" />
                            </div>
                            <div class="col-lg-6">
                                <div class="p-5">
                                    <div class="text-center">
                                        <h1 class="h4 text-gray-900 mb-2">Forgot Your Password?</h1>
                                        <p class="mb-4">
                                            We get it, stuff happens. Just enter your email address below
                                            and we'll send you a link to reset your password!
                                        </p>
                                    </div>
                                    <div class="user">
                                        <div class="form-group">
                                            <asp:TextBox runat="server" type="email" class="form-control form-control-user"
                                                ID="txtEmail" aria-describedby="emailHelp"
                                                placeholder="Enter Email Address..." />
                                        </div>
                                        <asp:Button ID="btnSendEmail" runat="server" Text="Send Email Link" CssClass="btn btn-primary btn-user btn-block" onClick="btnSendEmail_Click"/>
                                        <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>

                                    </div>
                                    <hr />
                                    <div class="text-center">
                                        <a class="small" href="Register.aspx">Create an Account!</a>
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

    </div>

</asp:Content>


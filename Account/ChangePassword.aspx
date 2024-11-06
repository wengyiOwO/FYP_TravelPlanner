<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/TakeMyTrip.Master" CodeBehind="ChangePassword.aspx.cs" Inherits="FYP_TravelPlanner.ChangePassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container d-flex justify-content-center align-items-center" style="min-height: 100vh; margin-left: auto; margin-right: auto;">

        <!-- Outer Row -->
        <div class="row justify-content-center w-100">
            <div class="col-xl-9 col-lg-12 col-md-9">
                <div class="card o-hidden border-0 shadow-lg my-5" style="margin-top: 50px;">
                    <div class="card-body p-0">
                        <!-- Nested Row within Card Body -->
                        <div class="row justify-content-center">
                            <div class="col-lg-6">
                                <div class="p-5">
                                    <div class="text-center">
                                        <h1 class="h4 text-gray-900 mb-4">Change Your Password</h1>
                                    </div>
                                    <div class="form-group">
                                        <label for="txtCurrentPassword" class="text-dark font-weight-bold">Current Password</label>
                                        <div class="input-group">
                                            <div class="input-group-prepend">
                                                <span class="input-group-text"><i class="fas fa-lock"></i></span>
                                            </div>
                                            <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" placeholder="Enter current password" CssClass="form-control form-control-user"></asp:TextBox>
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server" ControlToValidate="txtCurrentPassword" ErrorMessage="Current Password is required." CssClass="text-danger" />
                                    </div>
                                    <div class="form-group">
                                        <label for="txtNewPassword" class="text-dark font-weight-bold">New Password</label>
                                        <div class="input-group">
                                            <div class="input-group-prepend">
                                                <span class="input-group-text"><i class="fas fa-key"></i></span>
                                            </div>
                                            <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Enter new password" CssClass="form-control form-control-user"></asp:TextBox>
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ControlToValidate="txtNewPassword" ErrorMessage="New Password is required." CssClass="text-danger" />
                                    </div>
                                    <div class="form-group">
                                        <label for="txtConfirmNewPassword" class="text-dark font-weight-bold">Confirm New Password</label>
                                        <div class="input-group">
                                            <div class="input-group-prepend">
                                                <span class="input-group-text"><i class="fas fa-check"></i></span>
                                            </div>
                                            <asp:TextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password" placeholder="Confirm new password" CssClass="form-control form-control-user"></asp:TextBox>
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfvConfirmNewPassword" runat="server" ControlToValidate="txtConfirmNewPassword" ErrorMessage="Confirm New Password is required." CssClass="text-danger" />
                                        <asp:CompareValidator ID="cvNewPasswords" runat="server" ControlToValidate="txtConfirmNewPassword" ControlToCompare="txtNewPassword" ErrorMessage="Passwords do not match." CssClass="text-danger" />
                                    </div>
                                    <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn btn-primary btn-user btn-block" OnClick="btnChangePassword_Click" />
                                    <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>

                                    <hr />
                                    <div class="text-center">
                                        <a class="small text-primary" href="Profile.aspx">Back to Profile</a>
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

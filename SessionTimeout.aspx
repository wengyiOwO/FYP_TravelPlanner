<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionTimeout.aspx.cs" Inherits="FYP_TravelPlanner.SessionTimeout" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Session Timeout</title>
    <script type="text/javascript">
        setTimeout(function () {
            window.location.href = "Login.aspx";
        }, 5000); // Redirect after 5 seconds (adjust as needed)
    </script>
</head>
<body>
    <h2>Your session has expired due to inactivity.</h2>
    <p>You will be redirected to the login page shortly.</p>
</body>
</html>

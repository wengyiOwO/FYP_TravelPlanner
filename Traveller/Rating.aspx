<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rating.aspx.cs" MasterPageFile="~/TakeMyTrip.Master" Inherits="FYP_TravelPlanner.Traveller.Rating" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Include FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css">

    <style>
        .rating {
            display: flex;
            flex-direction: row-reverse;
            justify-content: left;
            position: relative;
        }

            .rating input {
                display: none;
            }

            .rating label {
                font-size: 2rem;
                color: #ccc;
                cursor: pointer;
            }

                .rating label i {
                    color: inherit;
                }

                .rating label:hover,
                .rating label:hover ~ label,
                .rating input:checked ~ label {
                    color: #FFD700;
                }

            .rating input:checked + label:hover,
            .rating input:checked + label:hover ~ label {
                color: #FFD700;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-5">
        <h2 class="mb-4 text-center">Rating and Reviews</h2>

        <!-- Question 1 -->
        <ol class="mb-4">
            <li>
                <h5>User Interface (UI) of this system is user-friendly</h5>
                <div class="rating">
                    <input type="radio" name="ui_rating" id="ui_rating_5" value="5"><label for="ui_rating_5"><i class="fas fa-star"></i></label>
                    <input type="radio" name="ui_rating" id="ui_rating_4" value="4"><label for="ui_rating_4"><i class="fas fa-star"></i></label>
                    <input type="radio" name="ui_rating" id="ui_rating_3" value="3"><label for="ui_rating_3"><i class="fas fa-star"></i></label>
                    <input type="radio" name="ui_rating" id="ui_rating_2" value="2"><label for="ui_rating_2"><i class="fas fa-star"></i></label>
                    <input type="radio" name="ui_rating" id="ui_rating_1" value="1"><label for="ui_rating_1"><i class="fas fa-star"></i></label>
                </div>
                <span id="ui_rating_error" class="text-danger" style="display: none;">Please select a rating.</span>

            </li>

            <!-- Question 2 -->
            <li>
                <h5>The travel plan generated meets my travel needs and preferences</h5>
                <div class="rating">
                    <input type="radio" name="plan_usefulness_rating" id="plan_usefulness_5" value="5"><label for="plan_usefulness_5"><i class="fas fa-star"></i></label>
                    <input type="radio" name="plan_usefulness_rating" id="plan_usefulness_4" value="4"><label for="plan_usefulness_4"><i class="fas fa-star"></i></label>
                    <input type="radio" name="plan_usefulness_rating" id="plan_usefulness_3" value="3"><label for="plan_usefulness_3"><i class="fas fa-star"></i></label>
                    <input type="radio" name="plan_usefulness_rating" id="plan_usefulness_2" value="2"><label for="plan_usefulness_2"><i class="fas fa-star"></i></label>
                    <input type="radio" name="plan_usefulness_rating" id="plan_usefulness_1" value="1"><label for="plan_usefulness_1"><i class="fas fa-star"></i></label>
                </div>
                <span id="plan_usefulness_error" class="text-danger" style="display: none;">Please select a rating.</span>

            </li>

            <!-- Question 3 -->
            <li>
                <h5>The features and functions provided by the system were easy to access and use</h5>
                <div class="rating">
                    <input type="radio" name="functionality_rating" id="functionality_5" value="5"><label for="functionality_5"><i class="fas fa-star"></i></label>
                    <input type="radio" name="functionality_rating" id="functionality_4" value="4"><label for="functionality_4"><i class="fas fa-star"></i></label>
                    <input type="radio" name="functionality_rating" id="functionality_3" value="3"><label for="functionality_3"><i class="fas fa-star"></i></label>
                    <input type="radio" name="functionality_rating" id="functionality_2" value="2"><label for="functionality_2"><i class="fas fa-star"></i></label>
                    <input type="radio" name="functionality_rating" id="functionality_1" value="1"><label for="functionality_1"><i class="fas fa-star"></i></label>
                </div>
                <span id="functionality_error" class="text-danger" style="display: none;">Please select a rating.</span>

            </li>

            <!-- Question 4 -->
            <li>
                <h5>Overall, I am satisfied with the system</h5>
                <div class="rating">
                    <input type="radio" name="satisfaction_rating" id="satisfaction_5" value="5"><label for="satisfaction_5"><i class="fas fa-star"></i></label>
                    <input type="radio" name="satisfaction_rating" id="satisfaction_4" value="4"><label for="satisfaction_4"><i class="fas fa-star"></i></label>
                    <input type="radio" name="satisfaction_rating" id="satisfaction_3" value="3"><label for="satisfaction_3"><i class="fas fa-star"></i></label>
                    <input type="radio" name="satisfaction_rating" id="satisfaction_2" value="2"><label for="satisfaction_2"><i class="fas fa-star"></i></label>
                    <input type="radio" name="satisfaction_rating" id="satisfaction_1" value="1"><label for="satisfaction_1"><i class="fas fa-star"></i></label>
                </div>
                <span id="satisfaction_error" class="text-danger" style="display: none;">Please select a rating.</span>

            </li>
            <!-- Question 5 -->

            <li>
                <h5>If you encountered any issues or have suggestions for improvement, please provide details below:</h5>

                <asp:TextBox runat="server" ID="txtReview" class="form-control mt-3" Rows="4" placeholder="Please provide any suggestions or feedback here..." TextMode="MultiLine"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvReview" runat="server" ControlToValidate="txtReview" ErrorMessage="Please provide feedback." ForeColor="Red" Display="Dynamic" />

            </li>
        </ol>
        <!-- Submit Button -->
        <div class="text-center" style="margin-bottom: 20px;">
            <asp:Button runat="server" ID="btnSubmit" Text="Submit" class="btn btn-primary" OnClick="btnSubmit_Click" OnClientClick="return validateForm();" />
            <br />
            <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.2/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <script>
        function validateForm() {
            let isValid = true;

            // Hide all error messages initially
            document.getElementById("ui_rating_error").style.display = "none";
            document.getElementById("plan_usefulness_error").style.display = "none";
            document.getElementById("functionality_error").style.display = "none";
            document.getElementById("satisfaction_error").style.display = "none";
            document.getElementById("review_error").style.display = "none";

            // Array of rating groups with their corresponding error messages
            const ratingGroups = [
                { name: 'ui_rating', errorId: 'ui_rating_error' },
                { name: 'plan_usefulness_rating', errorId: 'plan_usefulness_error' },
                { name: 'functionality_rating', errorId: 'functionality_error' },
                { name: 'satisfaction_rating', errorId: 'satisfaction_error' }
            ];

            // Validate each rating group
            ratingGroups.forEach(function (group) {
                const radios = document.getElementsByName(group.name);
                const groupValid = Array.from(radios).some(radio => radio.checked);
                if (!groupValid) {
                    document.getElementById(group.errorId).style.display = "block";
                    isValid = false;
                }
            });

            // Check if the review field is filled
            const review = document.getElementById("<%= txtReview.ClientID %>").value;
            if (review.trim() === '') {
                document.getElementById("review_error").style.display = "block";
                isValid = false;
            }

            // Return false if any validation fails
            return isValid;
        }
    </script>


</asp:Content>

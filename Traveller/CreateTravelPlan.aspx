<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="CreateTravelPlan.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.CreateTravelPlan" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Leaflet Map</title>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.min.js"></script>

    <style>
        html, body, .container-fluid, .row, .col-md-8 {
            height: 100%;
        }

        .card-body {
            overflow-y: auto;
            flex-grow: 1;
            max-height: calc(100% - 80px);
        }

        .card {
            padding: 15px;
        }

        .search-location-container {
            position: absolute;
            top: 20px;
            left: 50px;
            z-index: 1000;
            width: 250px;
            background-color: white;
            padding: 10px;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }

            .search-location-container .form-group {
                margin-bottom: 0;
            }

        .search-results-container {
            position: absolute;
            background-color: white;
            width: 100%;
            z-index: 1000;
            max-height: 200px;
            overflow-y: auto;
            border: 1px solid #ccc;
            border-radius: 0 0 5px 5px;
        }

        .search-result-item {
            padding: 10px;
            cursor: pointer;
        }

            .search-result-item:hover {
                background-color: #f0f0f0;
            }

        .ui-datepicker {
            font-size: 16px;
            background-color: white;
            border: 1px solid #ccc;
        }
    </style>



    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container-fluid p-0" style="height: 100vh;">
        <div class="row no-gutters" style="height: 100%;">
            <div class="col-md-4" style="height: 100%;">
                <div class="card bg-white shadow-sm border-0 h-100">
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title">Plan Your Trip</h5>
                        <div class="overflow-auto flex-grow-1">
                            <!-- This div will ensure scrolling -->

                            <!-- Area Selection -->
                            <div class="form-group">
                                <label for="ddlState">Select Area</label>
                                <asp:DropDownList ID="ddlState" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Select an area" Value="" />
                                    <asp:ListItem Text="Area 1" Value="1" />
                                    <asp:ListItem Text="Area 2" Value="2" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="ddlState"
                                    InitialValue=""
                                    ErrorMessage="Area is required"
                                    CssClass="text-danger" />
                            </div>

                            <!-- Start Date Selection -->
                            <div class="form-group">
                                <label for="txtStartDate">Start Date</label>
                                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" ReadOnly="True" />
                                <asp:Calendar ID="calendarStartDate" runat="server" OnSelectionChanged="calendarStartDate_SelectionChanged" />
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="txtStartDate"
                                    ErrorMessage="Start date is required"
                                    CssClass="text-danger" />
                                <asp:CustomValidator
                                    runat="server"
                                    ControlToValidate="txtStartDate"
                                    ErrorMessage="Start date must be today or later"
                                    CssClass="text-danger"
                                    OnServerValidate="ValidateStartDate" />
                            </div>

                            <!-- Duration Selection -->
                            <div class="form-group">
                                <label for="ddlDuration">Select Duration (Days)</label>
                                <asp:DropDownList ID="ddlDuration" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Select duration" Value="" />
                                    <asp:ListItem Text="1 day" Value="1" />
                                    <asp:ListItem Text="2 days" Value="2" />
                                    <asp:ListItem Text="3 days" Value="3" />
                                    <asp:ListItem Text="4 days" Value="4" />
                                    <asp:ListItem Text="5 days" Value="5" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="ddlDuration"
                                    InitialValue=""
                                    ErrorMessage="Duration is required"
                                    CssClass="text-danger" />
                            </div>

                            <!-- Budget Selection -->
                            <div class="form-group">
                                <label for="rblBudget">Select Budget (RM)</label>
                                <asp:RadioButtonList ID="rblBudget" runat="server" CssClass="form-check">
                                    <asp:ListItem Text="500" Value="500" />
                                    <asp:ListItem Text="1000" Value="1000" />
                                    <asp:ListItem Text="1500" Value="1500" />
                                    <asp:ListItem Text="2000" Value="2000" />
                                </asp:RadioButtonList>
                                <asp:RequiredFieldValidator
                                    runat="server"
                                    ControlToValidate="rblBudget"
                                    ErrorMessage="Budget is required"
                                    CssClass="text-danger" />
                            </div>

                            <!-- Activity Interest -->
                            <div class="form-group">
                                <label>Activity Interest</label><br />
                                <asp:CheckBoxList ID="cblActivities" runat="server" CssClass="form-check">
                                    <asp:ListItem Value="beaches">Beaches</asp:ListItem>
                                    <asp:ListItem Value="citySightseeing">City Sightseeing</asp:ListItem>
                                    <asp:ListItem Value="foodExploration">Food Exploration</asp:ListItem>
                                    <asp:ListItem Value="shopping">Shopping</asp:ListItem>
                                    <asp:ListItem Value="outdoorAdventures">Outdoor Adventures</asp:ListItem>
                                </asp:CheckBoxList>

                                <asp:CustomValidator
                                    runat="server"
                                    ID="CheckBoxRequired"
                                    EnableClientScript="true"
                                    OnServerValidate="CheckBoxRequired_ServerValidate"
                                    ErrorMessage="You must select at least one activity to proceed."
                                    CssClass="text-danger" />
                            </div>
                            <div class="form-group">
                                <label><b>You can add locations to your travel plan by searching for them in the search bar.</b></label>
                                <table class="table table-bordered" id="interestLocationsTable">
                                    <thead>
                                        <tr>
                                            <th>No. </th>
                                            <th>Location Name</th>
                                            <th>Area</th>
                                            <th>Actions</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                        <asp:Label ID="lblLocations" runat="server" CssClass="mt-3" Text="" />
                        <asp:Label ID="testError" runat="server" CssClass="mt-3" Text="" />
                        <asp:Button ID="btnPlan" runat="server" CssClass="btn btn-primary mt-3" Text="Plan" OnClick="btnPlan_Click" OnClientClick="prepareLocationsForSubmission()" />
                        <asp:HiddenField ID="hfSelectedLocations" runat="server" />

                    </div>
                </div>
            </div>
            <div class="col-md-8 p-0 position-relative" style="height: 100%;">
                <div id="map" style="height: 100%; width: 100%;"></div>
                <div class="search-location-container">
                    <div class="form-group">
                        <label for="search-input">Search Location</label>
                        <asp:TextBox ID="txtSearchLocation" runat="server" CssClass="form-control" placeholder="Enter a location" OnKeyUp="handleSearchInput()" />
                        <div id="search-results" class="search-results-container"></div>
                    </div>
                </div>
            </div>
        </div>
    </main>
    <script type="text/javascript">
        var map;
        var markers = [];
        var selectedLocations = [];
        var ddlStates = [];
        function initMap() {
            // zoom on KL
            map = L.map('map').setView([3.1390, 101.6869], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);
        }




        function searchLocations(query) {
            //languag english
            var url = 'https://nominatim.openstreetmap.org/search?format=json&accept-language=en&q=' + query;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    displayLocationResults(data);
                })
                .catch(error => {
                    console.error('Error fetching location data:', error);
                });
        }

        function displayLocationResults(locations) {
            var resultsContainer = document.getElementById('search-results');
            resultsContainer.innerHTML = '';

            locations.forEach(function (location) {
                var option = document.createElement('div');
                option.className = 'search-result-item';
                option.innerHTML = location.display_name;
                option.onclick = function () {
                    selectLocation(location);
                };
                resultsContainer.appendChild(option);
            });
        }

        function selectLocation(location) {
            markers.forEach(marker => map.removeLayer(marker));
            markers = [];

            var lat = location.lat;
            var lon = location.lon;

            var marker = L.marker([lat, lon]).addTo(map);
            marker.bindPopup("<b>" + location.display_name + "</b>").openPopup();
            map.setView([lat, lon], 13);

            markers.push(marker);

            addSelectedLocation(location);

            document.getElementById('<%= txtSearchLocation.ClientID %>').value = "";
            document.getElementById('search-results').innerHTML = "";
        }

        function handleSearchInput() {
            var query = document.getElementById('<%= txtSearchLocation.ClientID %>').value;
            if (query.length > 2) {
                searchLocations(query);
            } else {
                document.getElementById('search-results').innerHTML = '';
            }
        }


        $(document).ready(function () {
            $("#<%= txtStartDate.ClientID %>").datepicker({
                dateFormat: "dd-mm-yy",
                showAnim: "slideDown",  
                beforeShow: function (input, inst) {
                    $(inst.dpDiv).css({
                        'height': 'auto',    
                        'max-height': '300px', 
                        'overflow-y': 'auto'   
                    });
                }
            }).attr('readonly', true); 

            $("#<%= txtStartDate.ClientID %>").focus(function () {
                $(this).removeAttr('readonly'); 
                $(this).datepicker("show");
            }).blur(function () {
                $(this).attr('readonly', true);
            });
        });

        function initializeDDLStates() {
            $('#<%= ddlState.ClientID %> option').each(function () {
                var text = $(this).text().trim();
                var value = $(this).val();
                if (value) {
                    ddlStates.push({ text: text, value: value });
                }
            });
        }

        document.getElementById('<%= ddlState.ClientID %>').addEventListener("change", function () {
            var ddlState = document.getElementById('<%= ddlState.ClientID %>');
            var selectedState = ddlState.options[ddlState.selectedIndex].text;
            var selectedStateId = ddlState.value;

            selectedLocations = [];

            selectedArea = selectedState;
            selectedAreaId = selectedStateId;

            updateLocationsTable();
        });

        function addSelectedLocation(location) {
            var locationName = location.display_name.split(",")[0]; 
            var lat = location.lat;
            var lon = location.lon;
            var ddlState = document.getElementById('<%= ddlState.ClientID %>');
            var selectedState = ddlState.options[ddlState.selectedIndex].text;
            var selectedStateId = ddlState.value;

            var area = "Unsupported Area";
            var area_id = null;

            if (location.display_name.includes(selectedState)) {
                area = selectedState;
                area_id = selectedStateId;
            }

            if (area === "Unsupported Area") {
                alert("The location is not match with selected area");
                return;
            }

            var isDuplicate = selectedLocations.some(function (loc) {
                return loc.name === locationName;
            });

            if (isDuplicate) {
                alert("This location is already added.");
                return;
            }

            var locationObj = {
                name: locationName,
                area: area,
                area_id: area_id,
                lat: lat,
                lng: lon
            };
            selectedLocations.push(locationObj);

            updateLocationsTable();
        }

        function updateLocationsTable() {
            var tbody = document.querySelector("#interestLocationsTable tbody");
            tbody.innerHTML = ""; 

            selectedLocations.forEach((location, index) => {
                var row = document.createElement("tr");
                row.innerHTML = `
            <td>${index + 1}</td>
            <td>${location.name}</td>
            <td>${location.area}</td>
            <td><button class="btn btn-danger btn-sm" onclick="removeLocation(${index})">Remove</button></td>
        `;
                tbody.appendChild(row);
            });
        }

        // Remove a location from the array and update the table
        function removeLocation(index) {
            selectedLocations.splice(index, 1); // Remove from array
            updateLocationsTable(); // Refresh the table
        }

        function prepareLocationsForSubmission() {
            var hiddenField = document.getElementById('<%= hfSelectedLocations.ClientID %>');
            hiddenField.value = JSON.stringify(selectedLocations);
        }

        // Initialize ddlStates on page load
        $(document).ready(function () {
            initializeDDLStates();
        });


        window.onload = function () {
            initMap();
        }



    </script>
</asp:Content>

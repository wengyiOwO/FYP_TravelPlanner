<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="TravelPlan.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.TravelPlan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Travel Plan</title>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />

    <style>
        #map {
            height: 600px;
            width: 100%;
        }

        .table-container {
            max-height: 600px;
            overflow-y: auto;
        }

        .custom-div-icon {
            background-color: #007bff;
            border-radius: 50%;
            color: #fff;
            text-align: center;
            font-weight: bold;
            font-size: 16px;
            width: 24px;
            height: 24px;
        }

        .btn-delete {
            font-size: 16px;
            line-height: 1;
            width: 24px;
            height: 24px;
            text-align: center;
            border-radius: 50%;
            border: none;
            background-color: #dc3545;
            color: #fff;
            cursor: pointer;
            padding: 0;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-family: Arial, sans-serif;
        }

            .btn-delete:hover {
                background-color: #c82333;
            }

        .leaflet-routing-container {
            display: none;
        }

        .leaflet-routing-alt {
            display: none;
        }
    </style>
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet-routing-machine/3.2.12/leaflet-routing-machine.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>

    <script type="text/javascript">
        var map;
        var markers = [];
        var control;
        let locations = JSON.parse('<%= LocationsJson %>');
        let currentDay = 1;

        function initMap() {

            const allLocations = JSON.parse('<%= AllLocationsJson %>');
            map = L.map('map').setView([locations[0].lat, locations[0].lng], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            control = L.Routing.control({
                waypoints: locations.map(loc => L.latLng(loc.lat, loc.lng)),
                createMarker: function () { return null; },
                routeWhileDragging: false,
                show: false,
                addWaypoints: false,
                draggableWaypoints: false,
                routeLineOptions: {
                    styles: [{ color: '#ff0000', opacity: 0.8, weight: 5 }]
                }
            }).addTo(map);

            showDay(1);
        }

        function findNearbyLocation(lat, lng, event) {
            event.preventDefault(); // Prevents the default postback behavior

            var allLocations = JSON.parse('<%= AllLocationsJson %>');
            var selectedLocations = JSON.parse('<%= LocationsJson %>');

            // Get all locations within 2 km that are not already in the travel plan
            var nearbyLocations = allLocations.filter(function (location) {
                var distance = calculateDistance(lat, lng, location.lat, location.lng);

                // Check if location is nearby and not in the selected locations list
                var isSelected = selectedLocations.some(
                    selected => selected.lat === location.lat && selected.lng === location.lng
                );

                return distance <= 2 && !isSelected; // Exclude locations already in travel plan
            });

            // Display the nearby locations list
            displayNearbyLocationsList(nearbyLocations);
        }
        // Calculate the distance between two lat/lng points using Haversine formula
        function calculateDistance(lat1, lng1, lat2, lng2) {
            var R = 6371; // Radius of the Earth in km
            var dLat = degreesToRadians(lat2 - lat1);
            var dLng = degreesToRadians(lng2 - lng1);
            var a =
                Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(degreesToRadians(lat1)) * Math.cos(degreesToRadians(lat2)) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var distance = R * c; // Distance in km
            return distance;
        }

        // Convert degrees to radians
        function degreesToRadians(degrees) {
            return degrees * Math.PI / 180;
        }

        // Display the nearby locations on the map
        function displayNearbyLocationsList(locations) {
            var list = document.getElementById('nearbyLocationList');
            list.innerHTML = ''; // Clear previous entries

            if (locations.length === 0) {
                list.innerHTML = '<li class="list-group-item">No nearby locations found</li>';
            } else {
                locations.forEach(function (location) {
                    var listItem = document.createElement('li');
                    listItem.classList.add('list-group-item', 'd-flex', 'justify-content-between', 'align-items-center');
                    listItem.innerHTML = `<span><b>${location.name}</b><br>${location.address}</span>
                <button class="btn btn-sm btn-primary" onclick="addLocationToPlan(${location.lat}, ${location.lng}, '${location.name}')">Add</button>`;
                    list.appendChild(listItem);
                });
            }
        }

        function addLocationToPlan(lat, lng, name) {
            // Find the location object in AllLocationsJson to get its id
            const locationObj = JSON.parse('<%= AllLocationsJson %>').find(loc => loc.lat === lat && loc.lng === lng);
            if (!locationObj) {
                alert("Error: Location not found.");
                return;
            }

            const id = locationObj.id;

            // Add the new location to the markers array for tracking on the map
            var icon = L.divIcon({ html: `<div class="custom-div-icon">${markers.length + 1}</div>`, className: 'custom-div-icon' });
            var newMarker = L.marker([lat, lng], { icon: icon }).addTo(map);
            markers.push(newMarker);

            // Add the new location to the locations array for the current day, including the id
            locations.push({ id: id, lat: lat, lng: lng, name: name, day: currentDay });

            // Sort the locations array for the current day by latitude
            locations.sort((a, b) => a.lat - b.lat);

            // Update the table to reflect the sorted locations
            const tableBody = document.getElementById('locationTable');
            tableBody.innerHTML = '';  // Clear existing rows

            // Add the sorted locations back to the table
            locations.filter(loc => loc.day === currentDay).forEach((loc, index) => {
                const row = document.createElement('tr');
                row.innerHTML = `<td>${index + 1}</td><td>${loc.name}</td>
                         <td style="text-align: center;"><button class="btn btn-delete" onclick="deleteLocation(${loc.lat}, ${loc.lng}, ${loc.day}, this)">&times;</button></td>`;
                tableBody.appendChild(row);
            });


            // Update the route on the map to include the new location
            updateRoute();

            // Clear the nearby locations list
            document.getElementById('nearbyLocationList').innerHTML = '';
        }

        function showDay(day) {
            currentDay = day; // Update the current day

            // Filter locations for the specified day from the updated locations array
            const dayLocations = locations.filter(loc => loc.day === day);

            // Clear existing markers and update map route
            markers.forEach(marker => map.removeLayer(marker));
            markers = [];
            control.setWaypoints([]);

            // Add markers and display in table
            dayLocations.forEach((location, index) => {
                var icon = L.divIcon({ html: `<div class="custom-div-icon">${index + 1}</div>`, className: 'custom-div-icon' });
                var marker = L.marker([location.lat, location.lng], { icon: icon }).addTo(map);
                markers.push(marker);
                marker.bindPopup(`<b>${location.name}</b><br><button onclick="findNearbyLocation(${location.lat}, ${location.lng}, event)" class="btn btn-sm btn-primary mt-2">Find Nearby Location</button>`);
            });

            // Set waypoints for the routing control
            control.setWaypoints(dayLocations.map(loc => L.latLng(loc.lat, loc.lng)));

            // Update the table to display the day's locations
            updateTable(day);
        }

        function updateTable(day) {
            const tableBody = document.getElementById('locationTable');
            tableBody.innerHTML = ''; // Clear the table

            const dayLocations = locations.filter(loc => loc.day === day);

            dayLocations.forEach((location, index) => {
                var row = document.createElement('tr');
                row.innerHTML = `<td>${index + 1}</td><td>${location.name}</td>
                         <td style="text-align: center;"><button class="btn btn-delete" onclick="deleteLocation(${location.lat}, ${location.lng}, ${day}, this)">&times;</button></td>`;
                tableBody.appendChild(row);
            });
        }

        function focusLocation(lat, lng) {
            var location = L.latLng(lat, lng);
            map.setView(location, 15);
            markers.find(marker => marker.getLatLng().equals(location)).openPopup();
        }

        function deleteLocation(lat, lng, day, button) {
            // Remove the marker from the map and the markers array
            const markerIndex = markers.findIndex(marker => marker.getLatLng().equals([lat, lng]));
            if (markerIndex > -1) {
                map.removeLayer(markers[markerIndex]);
                markers.splice(markerIndex, 1);
            }

            // Remove the location from the locations array
            const locationIndex = locations.findIndex(loc => loc.lat === lat && loc.lng === lng && loc.day === day);
            if (locationIndex > -1) {
                locations.splice(locationIndex, 1);
            }

            // Remove the row from the table
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);

            // Update the route and markers on the map
            updateRoute();
            updateMarkerNumbers(); // Reorder marker numbers after deletion

            // Refresh the table to reflect the updated list for the current day
            updateTable(day);
        }

        function updateRoute() {
            // Set waypoints based on the updated markers array
            control.setWaypoints(markers.map(marker => marker.getLatLng()));
        }

        function updateMarkerNumbers() {
            markers.forEach((marker, i) => {
                var newIcon = L.divIcon({
                    html: '<div class="custom-div-icon">' + (i + 1) + '</div>',
                    className: 'custom-div-icon'
                });
                marker.setIcon(newIcon);
            });

            var rows = document.querySelectorAll('#locationTable tr');
            rows.forEach((row, i) => {
                row.cells[0].innerText = i + 1;
            });
        }

        function focusLocation(index) {
            var location = markers[index].getLatLng();
            map.setView(location, 15);
            markers[index].openPopup();
        }

        function saveTravelPlan() {
            // Serialize the current locations array to JSON
            const updatedLocationsJson = JSON.stringify(locations);

            // Use an AJAX call to send the updated locations to the server
            $.ajax({
                type: "POST",
                url: "TravelPlan.aspx/UpdateSelectedLocations",
                data: JSON.stringify({ updatedLocations: updatedLocationsJson }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    // After updating the session on the server, trigger the btnSave click
                    document.getElementById('<%= btnSave.ClientID %>').click();
                }
            });
        }

        function generatePDF() {
            const { jsPDF } = window.jspdf; // Get the jsPDF constructor
            const doc = new jsPDF();

            // Add header to the first page
            const header = 'Take My Trip';
            doc.setFontSize(20);
            doc.text(header, 20, 10); // Header at the top left

            // Footer text
            const footer = 'Thank you for using the Take My Trip\'s trip planning system';
            const footerY = 280; // Position for the footer (near bottom of the page)

            // Title for the document
            doc.setFontSize(16);
            doc.text('Travel Plan', 20, 20);

            let y = 30; // Starting y-position for the content

            // Iterate through each day of the itinerary
            const uniqueDays = Array.from(new Set(locations.map(loc => loc.day))); // Get all unique days

            uniqueDays.forEach(day => {
                // Add a header for the day
                doc.setFontSize(14);
                doc.text(`Day ${day} Itinerary`, 20, y);
                y += 10;

                // Create table headers
                doc.setFontSize(12);
                doc.text('No.', 20, y);
                doc.text('Location', 40, y);
                y += 10;

                // Add the locations for the specific day
                const dayLocations = locations.filter(loc => loc.day === day);

                dayLocations.forEach((location, index) => {
                    // Display each row
                    doc.text(`${index + 1}`, 20, y);
                    doc.text(location.name, 40, y);
                    y += 10;

                    // Add a page if content goes beyond the page limit
                    if (y > 270) { // If y goes beyond the page limit, add a new page
                        doc.addPage();
                        doc.setFontSize(20);
                        doc.text(header, 20, 10); // Re-add header
                        y = 30;
                    }
                });

                // Add a line separator between days
                doc.line(20, y, 190, y); // Draw a line
                y += 10; // Move down after the line
            });

            // Add footer
            doc.setFontSize(10);
            doc.text(footer, 20, footerY); // Footer near the bottom of the page

            // Open the PDF in a new tab
            doc.output('dataurlnewwindow');
        }

        window.addEventListener("load", initMap);
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container-fluid p-0">
        <div class="row no-gutters">
            <div class="col-md-4">
                <div class="card bg-white shadow-sm border-0">
                    <div class="card-body table-container">
                        <h5 class="card-title">Travel Plan</h5>
                        <!-- Day Tabs -->
                        <ul class="nav nav-tabs" id="dayTabs" role="tablist">
                            <% 
                                var locations = (List<Location>)Session["SelectedLocations"];
                                int maxDay = locations.Max(loc => loc.day);
                                for (int i = 1; i <= maxDay; i++)
                                {
                            %>
                            <li class="nav-item">
                                <a class="nav-link <% if (i == 1)
                                    { %>active<% } %>"
                                    id="day<%= i %>-tab" data-toggle="tab" href="#day<%= i %>"
                                    role="tab" aria-controls="day<%= i %>" aria-selected="<% if (i == 1)
                                    { %>true<% }
                                    else
                                    { %>false<% } %>"
                                    onclick="showDay(<%= i %>)">Day <%= i %></a>
                            </li>
                            <% } %>
                        </ul>
                        <!-- Table for Locations -->
                        <table class="table table-hover mt-3">
                            <thead>
                                <tr>
                                    <th>No.</th>
                                    <th>Location</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id="locationTable"></tbody>
                        </table>
                        <div class="nearby-locations mt-4">
                            <h5>Nearby Locations</h5>
                            <ul id="nearbyLocationList" class="list-group"></ul>
                        </div>


                        <div class="d-flex justify-content-between mt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save Plan" OnClick="btnSave_Click" OnClientClick="return saveTravelPlan();" CssClass="btn btn-primary" />
                            <asp:Button ID="btnPDF" runat="server" Text="Generate PDF" CssClass="btn btn-sm btn-primary shadow-sm" OnClientClick="generatePDF(); return false;" />
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-8">
                <div id="map" class="shadow-sm"></div>
            </div>
        </div>
        <div>
        </div>

    </main>
</asp:Content>

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
            display: none; /* Hides the routing instructions container */
        }

        .leaflet-routing-alt {
            display: none; /* Hides alternate routing instructions if present */
        }
    </style>
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet-routing-machine/3.2.12/leaflet-routing-machine.min.js"></script>
    <script type="text/javascript">
        var map;
        var markers = [];
        var control;

        function initMap() {

            const locations = JSON.parse('<%= LocationsJson %>');

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

        function findNearbyLocation(lat, lng) {

            alert("Finding nearby locations for: " + lat + ", " + lng);

        }

        function showDay(day) {
            const locations = JSON.parse('<%= LocationsJson %>');
            const dayLocations = locations.filter(loc => loc.day === day);

            markers.forEach(marker => map.removeLayer(marker));
            markers = [];
            control.setWaypoints([]);

            dayLocations.forEach((location, index) => {
                var icon = L.divIcon({ html: '<div class="custom-div-icon">' + (index + 1) + '</div>', className: 'custom-div-icon' });
                var marker = L.marker([location.lat, location.lng], { icon: icon }).addTo(map);
                markers.push(marker);
                marker.bindPopup("<b>" + location.name + "</b><br><button onclick='findNearbyLocation(" + location.lat + "," + location.lng + ")' class='btn btn-sm btn-primary mt-2'>Find Nearby Location</button>");
            });

            control.setWaypoints(dayLocations.map(loc => L.latLng(loc.lat, loc.lng)));
            updateTable(day);
        }

        function updateTable(day) {
            const tableBody = document.getElementById('locationTable');
            tableBody.innerHTML = '';

            const locations = JSON.parse('<%= LocationsJson %>');
            const dayLocations = locations.filter(loc => loc.day === day);

            dayLocations.forEach((location, index) => {
                var row = document.createElement('tr');
                row.innerHTML = `<td>${index + 1}</td><td><a href="#" onclick="focusLocation(${location.lat}, ${location.lng})">${location.name}</a></td>
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
            const markerIndex = markers.findIndex(marker => marker.getLatLng().equals([lat, lng]));
            if (markerIndex > -1) {
                map.removeLayer(markers[markerIndex]);
                markers.splice(markerIndex, 1);
            }

            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
            updateRoute();
        }

        function updateRoute() {
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
                                    { %>active<% } %>" id="day<%= i %>-tab" data-toggle="tab" href="#day<%= i %>"
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

                        <div class="d-flex justify-content-between mt-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save Plan" OnClick="btnSave_Click" CssClass="btn btn-primary" />
                            <asp:Button ID="btnPDF" runat="server" Text="Generate PDF" CssClass="btn btn-sm btn-primary shadow-sm" />
                            <asp:Label ID="lblMessage" runat="server" CssClass="text-small" Visible="true"></asp:Label>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-8">
                <div id="map" class="shadow-sm"></div>
            </div>
        </div>
    </main>
</asp:Content>

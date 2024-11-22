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
            map = L.map('map').setView([locations[0].lat, locations[0].lng], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            // Define the routing control
            control = L.Routing.control({
                waypoints: locations.map(loc => L.latLng(loc.lat, loc.lng)),
                createMarker: function () { return null; }, 
                routeWhileDragging: true,
                addWaypoints: false,
                draggableWaypoints: false
            }).addTo(map);

            control.on('routesfound', function (e) {
                console.log('Routes found:', e.routes); 
                e.routes.forEach(function (route) {
                    if (route._line) {
                        route._line.setStyle({ color: 'blue', weight: 5, opacity: 0.8 });
                    }
                });
            });

            control.getPlan().on('routefound', function (e) {
                console.log('Route found on plan:', e.routes); 
                e.routes.forEach(function (route) {
                    if (route._line) {
                        route._line.setStyle({ color: 'blue', weight: 5, opacity: 0.8 });
                    }
                });
            });

            showDay(1); 
        }

        function findNearbyLocation(lat, lng, event) {
            event.preventDefault(); 

            var allLocations = JSON.parse('<%= AllLocationsJson %>');
            var selectedLocations = locations;

            var nearbyLocations = allLocations.filter(function (location) {
                var distance = calculateDistance(lat, lng, location.lat, location.lng);

               
                var isSelected = selectedLocations.some(
                    selected => selected.lat === location.lat && selected.lng === location.lng
                );

                return distance <= 4 && !isSelected; 
            });

            displayNearbyLocationsList(nearbyLocations);
        }
        function calculateDistance(lat1, lng1, lat2, lng2) {
            var R = 6371; 
            var dLat = degreesToRadians(lat2 - lat1);
            var dLng = degreesToRadians(lng2 - lng1);
            var a =
                Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(degreesToRadians(lat1)) * Math.cos(degreesToRadians(lat2)) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var distance = R * c; 
            return distance;
        }

        function degreesToRadians(degrees) {
            return degrees * Math.PI / 180;
        }

        function displayNearbyLocationsList(locations) {
            var list = document.getElementById('nearbyLocationList');
            list.innerHTML = ''; 

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
            const locationObj = JSON.parse('<%= AllLocationsJson %>').find(loc => loc.lat === lat && loc.lng === lng);
            if (!locationObj) {
                alert("Error: Location not found.");
                return;
            }

            const id = locationObj.id;

            locations.push({ id: id, lat: lat, lng: lng, name: name, day: currentDay });

            locations.sort((a, b) => a.lat - b.lat);

            updateTable(currentDay);

            markers.forEach(marker => map.removeLayer(marker));
            markers = []; 

            locations.filter(loc => loc.day === currentDay).forEach((location, index) => {
                var icon = L.divIcon({ html: `<div class="custom-div-icon">${index + 1}</div>`, className: 'custom-div-icon' });
                var marker = L.marker([location.lat, location.lng], { icon: icon }).addTo(map);
                markers.push(marker);
                marker.bindPopup(`<b>${location.name}</b><br><button onclick="findNearbyLocation(${location.lat}, ${location.lng}, event)" class="btn btn-sm btn-primary mt-2">Find Nearby Location</button>`);
            });

            control.setWaypoints(locations.filter(loc => loc.day === currentDay).map(loc => L.latLng(loc.lat, loc.lng)));

            document.getElementById('nearbyLocationList').innerHTML = '';
        }



        function deleteLocation(lat, lng, day, button) {
            locations = locations.filter(location => !(location.lat === lat && location.lng === lng && location.day === day));

            // Remove the from the map
            markers = markers.filter(marker => {
                if (marker.getLatLng().lat === lat && marker.getLatLng().lng === lng) {
                    map.removeLayer(marker);
                    return false; // Remove this marker from the array
                }
                return true;
            });

            // Remove the row from the table
            var row = button.closest('tr');
            row.parentNode.removeChild(row);

            updateMarkerNumbers();
            updateTable(day);

            updateRoute();
        }


        function showDay(day) {
            currentDay = day;
            const dayLocations = locations.filter(loc => loc.day === day);

            markers.forEach(marker => map.removeLayer(marker));
            markers = [];
            control.setWaypoints([]);

            dayLocations.forEach((location, index) => {
                var icon = L.divIcon({ html: `<div class="custom-div-icon">${index + 1}</div>`, className: 'custom-div-icon' });
                var marker = L.marker([location.lat, location.lng], { icon: icon }).addTo(map);
                markers.push(marker);
                marker.bindPopup(`<b>${location.name}</b><br><button onclick="findNearbyLocation(${location.lat}, ${location.lng}, event)" class="btn btn-sm btn-primary mt-2">Find Nearby Location</button>`);
            });

            control.setWaypoints(dayLocations.map(loc => L.latLng(loc.lat, loc.lng)));
            updateTable(day);
        }

        function updateTable(day) {
            const tableBody = document.getElementById('locationTable');
            tableBody.innerHTML = ''; 

            const dayLocations = locations.filter(loc => loc.day === day);
            let totalDistance = 0;

            dayLocations.forEach((location, index) => {
                const distance = index > 0 ? calculateDistance(dayLocations[index - 1].lat, dayLocations[index - 1].lng, location.lat, location.lng) : 0;
                totalDistance += distance;

                // Convert distance to time in minutes
                const timeInMinutes = totalDistance * 60; 

                let timeDisplay = '';
                if (timeInMinutes >= 60) {
                    // Convert to hours and minutes
                    const hours = Math.floor(timeInMinutes / 60);
                    const minutes = Math.round(timeInMinutes % 60);
                    timeDisplay = `${hours} h${hours > 1 ? 's' : ''} ${minutes} min`;
                } else {
                    // If time is less than 60 minutes, display only minutes
                    timeDisplay = `${Math.round(timeInMinutes)} min`;
                }

                let row = document.createElement('tr');
                row.innerHTML = `
            <td>${index + 1}</td>
            <td>${location.name}</td>
            <td>${distance.toFixed(2)} km</td>
            <td>${timeDisplay}</td> <!-- Display time in formatted hours and minutes -->
            <td style="text-align: center;">
                <button class="btn btn-delete" onclick="deleteLocation(${location.lat}, ${location.lng}, ${day}, this)">&times;</button>
            </td>
        `;
                tableBody.appendChild(row);
            });
        }



        function calculateDistance(lat1, lng1, lat2, lng2) {
            var R = 6371; // Earth's radius in km
            var dLat = degreesToRadians(lat2 - lat1);
            var dLng = degreesToRadians(lng2 - lng1);
            var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(degreesToRadians(lat1)) * Math.cos(degreesToRadians(lat2)) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            return R * c; // Distance in km
        }

        function degreesToRadians(degrees) {
            return degrees * Math.PI / 180;
        }

        function formatTime(hours) {
            const totalMinutes = Math.floor(hours * 60);
            const h = Math.floor(totalMinutes / 60);
            const m = totalMinutes % 60;
            return `${h}:${m.toString().padStart(2, '0')}`;
        }

        function focusLocation(lat, lng) {
            var location = L.latLng(lat, lng);
            map.setView(location, 15);
            markers.find(marker => marker.getLatLng().equals(location)).openPopup();
        }
        function updateRoute() {
            // Set the waypoints for the routing control
            control.setWaypoints(markers.map(marker => marker.getLatLng()));

            // Update the route line color to blue
            control.getPlan().getRoute(0).setStyle({ color: 'blue' });
        }

        function updateMarkerNumbers() {
            markers.forEach((marker, i) => {
                var newIcon = L.divIcon({
                    html: `<div class="custom-div-icon">${i + 1}</div>`,
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

            const headerImage = '../img/logo.png';
            const imageWidth = 80; 
            const imageHeight = 40; 
            const imageX = (doc.internal.pageSize.width - imageWidth) / 2;
            const imageY = 10;

            doc.addImage(headerImage, 'PNG', imageX, imageY, imageWidth, imageHeight);

            // Footer text
            const footer = 'Thank you for using the Take My Trip\'s trip planning system';
            const footerY = 280; 

            // Title
            doc.setFontSize(16);
            doc.text('Travel Plan', 20, imageY + imageHeight + 10);  

            let y = imageY + imageHeight + 20; 

            // Iterate through each day of the itinerary
            const uniqueDays = Array.from(new Set(locations.map(loc => loc.day)));

            uniqueDays.forEach(day => {
                // Add a header for the day
                doc.setFontSize(14);
                doc.text(`Day ${day} Itinerary`, 20, y);
                y += 10;

                // Create table headers
                doc.setFontSize(12);
                doc.text('No.', 20, y);
                doc.text('Location', 40, y);
                doc.text('Distance', 100, y); 
                doc.text('Time', 140, y);    
                y += 10;

                // Add the locations for the specific day
                const dayLocations = locations.filter(loc => loc.day === day);

                dayLocations.forEach((location, index) => {
                    const distance = index > 0 ? calculateDistance(dayLocations[index - 1].lat, dayLocations[index - 1].lng, location.lat, location.lng) : 0;
                    const timeInMinutes = distance * 60;

                    let timeDisplay = '';
                    if (timeInMinutes >= 60) {
                        const hours = Math.floor(timeInMinutes / 60);
                        const minutes = Math.round(timeInMinutes % 60);
                        timeDisplay = `${hours} h${hours > 1 ? 's' : ''} ${minutes} min`;
                    } else {
                        timeDisplay = `${Math.round(timeInMinutes)} min`;
                    }

                    doc.text(`${index + 1}`, 20, y);
                    doc.text(location.name, 40, y);
                    doc.text(distance.toFixed(2) + ' km', 100, y);  
                    doc.text(timeDisplay, 140, y);                  
                    y += 10;

                    if (y > 270) { 
                        doc.addPage();
                        doc.setFontSize(16);
                        doc.text('Travel Plan', 20, 20);
                        doc.addImage(headerImage, 'PNG', imageX, imageY, imageWidth, imageHeight);
                        y = imageY + imageHeight + 10; 
                    }
                });

                doc.line(20, y, 190, y); 
                y += 10;
            });

            // Add footer
            doc.setFontSize(10);
            doc.text(footer, 20, footerY); 

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
                                    <th>Distance</th>
                                    <th>Time</th>
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
                                <asp:HiddenField ID="hiddenPlanId" runat="server" />
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

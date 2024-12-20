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

        .notification {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            background-color: #4CAF50;
            color: white;
            text-align: center;
            padding: 10px 0;
            font-size: 16px;
            z-index: 1000;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
        }
    </style>
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/leaflet-routing-machine/3.2.12/leaflet-routing-machine.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/2.5.1/jspdf.umd.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Sortable/1.14.0/Sortable.min.js"></script>

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

            control = L.Routing.control({
                waypoints: locations.map(loc => L.latLng(loc.lat, loc.lng)),
                createMarker: function () { return null; },
                routeWhileDragging: true,
                addWaypoints: false,
                draggableWaypoints: false
            }).addTo(map);

            showDay(1);
        }

        function showDay(day) {
            currentDay = day;
            const dayLocations = locations.filter(loc => loc.day === day);

            markers.forEach(marker => map.removeLayer(marker));
            markers = [];
            control.setWaypoints([]);

            dayLocations.forEach((location, index) => {
                var icon = L.divIcon({
                    html: `<div class="custom-div-icon">${index + 1}</div>`,
                    className: 'custom-div-icon'
                });
                var marker = L.marker([location.lat, location.lng], { icon: icon }).addTo(map);
                markers.push(marker);
                marker.bindPopup(`<b>${location.name}</b><br>
                <button onclick="findNearbyLocation(${location.lat}, ${location.lng}, event)" 
                class="btn btn-sm btn-primary mt-2">Find Nearby Location</button>`);
            });

            control.setWaypoints(dayLocations.map(loc => L.latLng(loc.lat, loc.lng)));
            updateTable(day);
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

        async function updateTable(day) {
            const tableBody = document.getElementById('locationTable');
            tableBody.innerHTML = '';  // Clear existing table content

            const dayLocations = locations.filter(loc => loc.day === day);
            const rowsData = []; 

            for (let index = 0; index < dayLocations.length; index++) {
                const location = dayLocations[index];
                let distance = 0;
                let duration = 0;

                // Calculate using OpenStreet routing API
                if (index > 0) {
                    const prevLocation = dayLocations[index - 1];
                    const route = await getRoute(prevLocation.lat, prevLocation.lng, location.lat, location.lng);

                    if (route) {
                        distance = route.distance / 1000;
                        duration = route.duration;
                    }
                }

                // Convert duration to hours and minutes
                const hours = Math.floor(duration / 3600);
                const minutes = Math.round((duration % 3600) / 60);
                const timeDisplay = duration > 0
                    ? (hours > 0
                        ? `${hours} h ${minutes} min`
                        : `${minutes} min`)
                    : '-';

                rowsData.push({
                    index: index + 1,
                    name: location.name,
                    distance: distance.toFixed(2),
                    timeDisplay: timeDisplay,
                    lat: location.lat,
                    lng: location.lng
                });
            }

            rowsData.forEach(rowData => {
                const row = document.createElement('tr');
                row.innerHTML = `
            <td>${rowData.index}</td>
            <td>${rowData.name}</td>
            <td>${rowData.distance} km</td>
            <td>${rowData.timeDisplay}</td>
            <td style="text-align: center;">
                <button class="btn btn-delete" onclick="deleteLocation(${rowData.lat}, ${rowData.lng}, ${day}, this)">&times;</button>
            </td>
        `;
                tableBody.appendChild(row);
            });
        }


        //get route with OpenStreet API
        async function getRoute(lat1, lng1, lat2, lng2) {
            const url = `https://router.project-osrm.org/route/v1/driving/${lng1},${lat1};
                            ${lng2},${lat2}?overview=false`;
            try {
                const data = await fetchWithRetry(url);
                if (data.routes && data.routes.length > 0) {
                    const route = data.routes[0];
                    return {
                        distance: route.distance,
                        duration: route.duration
                    };
                }
            } catch (error) {
                console.error('Failed to fetch route:', error);
            }
            return null;
        }

        async function fetchWithRetry(url, retries = 3, delay = 1000) {
            for (let i = 0; i < retries; i++) {
                try {
                    const response = await fetch(url);
                    if (response.ok) {
                        return await response.json();
                    }
                } catch (error) {
                    console.error(`Attempt ${i + 1} failed:`, error);
                }
                await new Promise(resolve => setTimeout(resolve, delay));
            }
            throw new Error('Failed to fetch after retries');
        }

        function calculateDistance(lat1, lng1, lat2, lng2) {
            var R = 6371;
            var dLat = degreesToRadians(lat2 - lat1);
            var dLng = degreesToRadians(lng2 - lng1);
            var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(degreesToRadians(lat1)) * Math.cos(degreesToRadians(lat2)) *
                Math.sin(dLng / 2) * Math.sin(dLng / 2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            return R * c;
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
            control.setWaypoints(markers.map(marker => marker.getLatLng()));

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
            const updatedLocationsJson = JSON.stringify(locations);

            return $.ajax({
                type: "POST",
                url: "TravelPlan.aspx/UpdateSelectedLocations",
                data: JSON.stringify({ updatedLocations: updatedLocationsJson }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (response) {
                    //
                },
                error: function (xhr, status, error) {
                    alert("Failed to save travel plan. Error: " + error);
                }
            }).then(function () {

                return true;
            }).catch(function () {
                return false;
            });
        }

        async function generatePDF() {
            const { jsPDF } = window.jspdf;
            const doc = new jsPDF();

            const headerImage = '../img/logo.png';
            const imageWidth = 80;
            const imageHeight = 40;
            const imageX = (doc.internal.pageSize.width - imageWidth) / 2;
            const imageY = 10;

            doc.addImage(headerImage, 'PNG', imageX, imageY, imageWidth, imageHeight);

            const footer = 'Thank you for using the Take My Trip\'s trip planning system';
            const footerY = 280;

            doc.setFontSize(16);
            doc.text('Travel Plan', 20, imageY + imageHeight + 10);

            let y = imageY + imageHeight + 20;

            const uniqueDays = Array.from(new Set(locations.map(loc => loc.day)));

            for (const day of uniqueDays) {
                doc.setFontSize(14);
                doc.text(`Day ${day} Itinerary`, 20, y);
                y += 10;

                doc.setFontSize(12);
                doc.text('No.', 20, y);
                doc.text('Location', 30, y);
                doc.text('Distance', 100, y);
                doc.text('Time', 140, y);
                y += 10;

                const dayLocations = locations.filter(loc => loc.day === day);

                const routePromises = [];

                for (let index = 0; index < dayLocations.length; index++) {
                    const location = dayLocations[index];
                    if (index > 0) {
                        const prevLocation = dayLocations[index - 1];
                        routePromises.push(getRoute(prevLocation.lat, prevLocation.lng, location.lat, location.lng));
                    }
                }

                const routes = await Promise.all(routePromises);

                for (let index = 0; index < dayLocations.length; index++) {
                    const location = dayLocations[index];
                    let distance = 0;
                    let duration = 0;

                    if (index > 0) {
                        const route = routes[index - 1];
                        if (route) {
                            distance = route.distance / 1000;
                            duration = route.duration;
                        }
                    }

                    const hours = Math.floor(duration / 3600);
                    const minutes = Math.round((duration % 3600) / 60);
                    const timeDisplay = duration > 0
                        ? (hours > 0
                            ? `${hours} h ${minutes} min`
                            : `${minutes} min`)
                        : '-';

                    doc.text(`${index + 1}`, 20, y);
                    doc.text(location.name, 30, y);
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
                }

                doc.line(20, y, 190, y);
                y += 10;
            }

            doc.setFontSize(10);
            doc.text(footer, 20, footerY);

            doc.output('dataurlnewwindow');
        }



        window.addEventListener("load", initMap);
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container-fluid p-0">
        <div id="notification" runat="server" class="notification" style="display: none;">
            <asp:Label ID="lblNotification" runat="server" Text=""></asp:Label>
        </div>

        <div class="row no-gutters">
            <div class="col-md-4">
                <div class="card bg-white shadow-sm border-0">
                    <div class="card-body table-container">
                        <h5 class="card-title">Travel Plan</h5>
                        <ul class="nav nav-tabs" id="dayTabs" role="tablist">
                            <% 
                                var locations = (List<Location>)Session["SelectedLocations"];

                                if (locations == null || locations.Count == 0)
                                {
                            %>
                            <li class="alert alert-danger" role="alert">Unable to display the plan, the location is empty.
                            </li>
                            <% 
                                }
                                else
                                {
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
                            <% 
                                    }
                                }
                            %>
                        </ul>
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
                            <asp:Button ID="btnSave" runat="server" Text="Save Plan" OnClientClick="return saveTravelPlan();" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                            <asp:Button ID="btnShare" runat="server" CssClass="btn btn-outline-primary btn-sm mr-2" Text="Share" OnClick="btnShare_Click" />
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
            <asp:Panel ID="pnlFriend" runat="server" CssClass="modal" Style="display: none;">
                <div class="modal-dialog modal-dialog-centered modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Share to?</h5>

                            <button type="button" class="close" onclick="hideFriendList();">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <!-- Friend List Section -->
                                <div class="col-12">
                                    <asp:Repeater ID="rptFriendsList" runat="server" OnItemCommand="rptFriendsList_ItemCommand">
                                        <ItemTemplate>
                                            <div class="d-flex align-items-center border-bottom py-2">
                                                <asp:Image
                                                    ID="imgProfile"
                                                    runat="server"
                                                    CssClass="rounded-circle mr-3"
                                                    Width="40"
                                                    Height="40"
                                                    ImageUrl='<%# "~/Uploads/Profile/" + Eval("profile_image") %>'
                                                    AlternateText="Profile Image" />
                                                <div class="flex-grow-1">
                                                    <span><%# Eval("account_name") %></span>
                                                </div>
                                                <asp:Button ID="btnSend" runat="server" CssClass="btn btn-primary btn-lg mr-1 px-3" CommandName="Send" CommandArgument='<%# Eval("account_id") %>' Text="Send" />
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
        <div>
        </div>

    </main>
    <script>
        function showFriendList() {
            document.getElementById('<%= pnlFriend.ClientID %>').style.display = 'block';
        }
        function hideFriendList() {
            document.getElementById('<%= pnlFriend.ClientID %>').style.display = 'none';
        }
    </script>
</asp:Content>

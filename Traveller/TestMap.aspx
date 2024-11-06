<%@ Page Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="TestMap.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.TestMap" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Leaflet Map Integration with Data Table</title>
    <!-- Leaflet CSS -->
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css" />
    <!-- Leaflet JavaScript -->
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <style>
        #map { height: 500px; width: 100%; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; }
        table, th, td { border: 1px solid black; }
        th, td { padding: 10px; text-align: left; }
    </style>
    <script type="text/javascript">
        var map;
        var locations = [
            { lat: 3.1390, lng: 101.6869, name: "Kuala Lumpur", info: "The capital city of Malaysia." },
            { lat: 3.1390, lng: 101.6959, name: "Petronas Towers", info: "Famous skyscrapers in Kuala Lumpur." },
            { lat: 3.1476, lng: 101.7110, name: "Batu Caves", info: "Popular tourist attraction in Malaysia." }
        ];

        function initMap() {
            map = L.map('map').setView([3.1390, 101.6869], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            locations.forEach(function (location) {
                L.marker([location.lat, location.lng]).addTo(map)
                    .bindPopup("<b>" + location.name + "</b><br>" + location.info);
            });

            map.on('moveend', updateTable);
            updateTable(); // Initial call to populate the table
        }

        function updateTable() {
            var bounds = map.getBounds();
            var visibleLocations = locations.filter(function (location) {
                return bounds.contains([location.lat, location.lng]);
            });

            var table = document.getElementById('locationTable');
            table.innerHTML = ''; // Clear the table

            // Add table header
            var header = table.createTHead();
            var headerRow = header.insertRow();
            headerRow.insertCell().textContent = 'Name';
            headerRow.insertCell().textContent = 'Info';

            // Add rows for each visible location
            var body = table.createTBody();
            visibleLocations.forEach(function (location) {
                var row = body.insertRow();
                row.insertCell().textContent = location.name;
                row.insertCell().textContent = location.info;
            });
        }
    </script>
</head>
<body onload="initMap()">
    <form id="form1" runat="server">
        <div>
            <h1>Leaflet Map Example - Multiple Points</h1>
            <div id="map"></div>
            <table id="locationTable"></table>
        </div>
    </form>
</body>
</html>

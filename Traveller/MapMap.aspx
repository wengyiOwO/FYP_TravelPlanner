<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="MapMap.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.MapMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <title>Leaflet Map Integration</title>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css" />
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
    <script type="text/javascript">
        var map;
        var markers = [];

        function initMap() {
            var locations = [
                { lat: 3.1390, lng: 101.6869, name: "Kuala Lumpur", info: "The capital city of Malaysia.", activity: "city" },
                { lat: 3.1390, lng: 101.6959, name: "Petronas Towers", info: "Famous skyscrapers in Kuala Lumpur.", activity: "chill" },
                { lat: 3.1476, lng: 101.7110, name: "Batu Caves", info: "Popular tourist attraction in Malaysia.", activity: "adventure" },
                { lat: 3.1520, lng: 101.7030, name: "KLCC Park", info: "Relaxing park near the Petronas Towers.", activity: "chill" },
                { lat: 3.1569, lng: 101.7123, name: "Pavilion Mall", info: "Luxury shopping mall.", activity: "chill" },
                { lat: 3.1412, lng: 101.6872, name: "Chinatown", info: "Famous street market in Kuala Lumpur.", activity: "chill" },
                { lat: 3.1300, lng: 101.6879, name: "Lake Gardens", info: "Beautiful gardens and parks.", activity: "chill" },
                { lat: 3.1487, lng: 101.7297, name: "Titiwangsa Park", info: "Large recreational park.", activity: "chill" },
                { lat: 3.1346, lng: 101.6865, name: "Central Market", info: "Cultural heritage site and shopping.", activity: "chill" },
                { lat: 3.1400, lng: 101.6880, name: "Aquaria KLCC", info: "State-of-the-art oceanarium.", activity: "chill" }
            ];

            map = L.map('map').setView([locations[0].lat, locations[0].lng], 13);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            locations.forEach(function (location) {
                var marker = L.marker([location.lat, location.lng]).addTo(map);
                marker.bindPopup("<b>" + location.name + "</b><br>" + location.info);
                markers.push(marker);
            });
        }

        function filterLocations() {
            markers.forEach(function (marker) {
                map.removeLayer(marker);
            });
            markers = [];

            var locations = [
                { lat: 3.1390, lng: 101.6869, name: "Kuala Lumpur", info: "The capital city of Malaysia.", activity: "city" },
                { lat: 3.1390, lng: 101.6959, name: "Petronas Towers", info: "Famous skyscrapers in Kuala Lumpur.", activity: "chill" },
                { lat: 3.1476, lng: 101.7110, name: "Batu Caves", info: "Popular tourist attraction in Malaysia.", activity: "adventure" },
                { lat: 3.1520, lng: 101.7030, name: "KLCC Park", info: "Relaxing park near the Petronas Towers.", activity: "chill" },
                { lat: 3.1569, lng: 101.7123, name: "Pavilion Mall", info: "Luxury shopping mall.", activity: "chill" },
                { lat: 3.1412, lng: 101.6872, name: "Chinatown", info: "Famous street market in Kuala Lumpur.", activity: "chill" },
                { lat: 3.1300, lng: 101.6879, name: "Lake Gardens", info: "Beautiful gardens and parks.", activity: "chill" },
                { lat: 3.1487, lng: 101.7297, name: "Titiwangsa Park", info: "Large recreational park.", activity: "chill" },
                { lat: 3.1346, lng: 101.6865, name: "Central Market", info: "Cultural heritage site and shopping.", activity: "chill" },
                { lat: 3.1400, lng: 101.6880, name: "Aquaria KLCC", info: "State-of-the-art oceanarium.", activity: "chill" }
            ];

            var baseLocation = locations[0];
            var nearbyLocations = locations.filter(function (location) {
                if (location.name === baseLocation.name) return false;

                var distance = Math.sqrt(
                    Math.pow(location.lat - baseLocation.lat, 2) +
                    Math.pow(location.lng - baseLocation.lng, 2)
                );
                location.distance = distance;

                return location.activity === "chill";
            });

            nearbyLocations.sort(function (a, b) {
                return a.distance - b.distance;
            });

            var recommendedLocations = nearbyLocations.slice(0, 5);

            recommendedLocations.forEach(function (location) {
                var marker = L.marker([location.lat, location.lng]).addTo(map);
                marker.bindPopup("<b>" + location.name + "</b><br>" + location.info).openPopup();
                markers.push(marker);
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<body onload="initMap()">
        <div>
            <h1>Leaflet Map Example - Filter "Chill" Locations</h1>
            <div id="map" style="height: 500px; width: 100%;"></div>
            <button type="button" onclick="filterLocations()">Show Nearest "Chill" Locations</button>
        </div>
</body>
</asp:Content>

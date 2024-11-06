<%@ Page Title="" Language="C#" MasterPageFile="~/TakeMyTrip.Master" AutoEventWireup="true" CodeBehind="ExploreMap.aspx.cs" Inherits="FYP_TravelPlanner.Traveller.ExploreMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Leaflet Map with Search Function</title>
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.7.1/dist/leaflet.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <style>
        .search-results-container {
            position: absolute;
            background-color: white;
            z-index: 1000;
            width: 100%;
            max-height: 200px;
            overflow-y: auto;
            border: 1px solid #ccc;
        }

        .search-result-item {
            padding: 10px;
            cursor: pointer;
        }

        .search-result-item:hover {
            background-color: #f0f0f0;
        }
    </style>
    <script src="https://unpkg.com/leaflet@1.7.1/dist/leaflet.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container-fluid p-0">
        <div class="card p-4 mx-0">
            <h5>Explore Map</h5>
            <!-- Search Input -->
            <div class="d-flex align-items-center mb-3">
                <div class="flex-grow-1">
                    <input type="text" id="search-input" class="form-control" placeholder="Search for a location..." onkeyup="handleSearchInput()">
                    <div id="search-results" class="search-results-container"></div>
                </div>
                <div class="input-group-append">
                    <button class="btn btn-primary" type="button">
                        <i class="fas fa-search fa-sm"></i>
                    </button>
                </div>
            </div>
            <!-- Map -->
            <div id="map" style="height: 600px; width: 100%;"></div>
        </div>
    </main>

    <script type="text/javascript">
        var map;
        var markers = [];

        // Initialize Leaflet Map
        function initMap() {
            map = L.map('map').setView([3.1390, 101.6869], 13);  // Default to Kuala Lumpur

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);
        }

        // Function to search locations using Nominatim API
        function searchLocations(query) {
            var url = 'https://nominatim.openstreetmap.org/search?format=json&q=' + query;

            fetch(url)
                .then(response => response.json())
                .then(data => {
                    displayLocationResults(data);
                })
                .catch(error => {
                    console.error('Error fetching location data:', error);
                });
        }

        // Function to display search results in the dropdown
        function displayLocationResults(locations) {
            var resultsContainer = document.getElementById('search-results');
            resultsContainer.innerHTML = '';  // Clear previous results

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

        // Function to mark selected location on the map
        function selectLocation(location) {
            // Clear previous markers
            markers.forEach(marker => map.removeLayer(marker));
            markers = [];

            var lat = location.lat;
            var lon = location.lon;

            var marker = L.marker([lat, lon]).addTo(map);
            marker.bindPopup("<b>" + location.display_name + "</b>").openPopup();
            map.setView([lat, lon], 13);

            markers.push(marker);
            document.getElementById('search-results').innerHTML = '';  // Clear search results
        }

        // Event handler for search input
        function handleSearchInput() {
            var query = document.getElementById('search-input').value;
            if (query.length > 2) {  // Start searching after 3 characters
                searchLocations(query);
            } else {
                document.getElementById('search-results').innerHTML = '';  // Clear search results
            }
        }

        window.onload = function () {
            initMap();
        }
    </script>
</asp:Content>

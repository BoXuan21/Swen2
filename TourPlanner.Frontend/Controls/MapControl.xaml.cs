using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using TourPlanner.Frontend.Models;

namespace TourPlanner.Frontend.Controls
{
    public partial class MapControl : UserControl
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "5b3ce3597851110001cf6248ff6cecb4e4444f4c82fe19ce2fc127a5";
        private const string API_BASE_URL = "https://api.openrouteservice.org/v2/directions/";
        private bool _isInitialized = false;
        
        public event EventHandler<RouteCalculatedEventArgs> RouteCalculated;
        public event EventHandler<LocationSelectedEventArgs> LocationSelected;

        public MapControl()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", API_KEY);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "TourPlanner/1.0");
            InitializeAsync();
        }

        private async void MapWebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                MapWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                MapWebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                await LoadMapAsync();
                _isInitialized = true;
            }
            else
            {
                MessageBox.Show($"Failed to initialize WebView2: {e.InitializationException?.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MapWebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Failed to load map", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InitializeAsync()
        {
            try
            {
                await MapWebView.EnsureCoreWebView2Async();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadMapAsync()
        {
            try
            {
                string html = GetMapHtml();
                MapWebView.CoreWebView2.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading map: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                await MapWebView.CoreWebView2.ExecuteScriptAsync(@"
                    document.addEventListener('click', function(e) {
                        if (e.target.classList.contains('location-marker')) {
                            const name = e.target.getAttribute('data-name');
                            const lat = parseFloat(e.target.getAttribute('data-lat'));
                            const lon = parseFloat(e.target.getAttribute('data-lon'));
                            window.chrome.webview.postMessage(JSON.stringify({
                                type: 'locationSelected',
                                data: { name, lat, lon }
                            }));
                        }
                    });
                ");
            }
        }

        private async void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = System.Text.Json.JsonSerializer.Deserialize<WebMessage>(e.WebMessageAsJson);
                
                if (message?.Type == "locationSelected" && message.Data != null)
                {
                    var locationData = System.Text.Json.JsonSerializer.Deserialize<LocationData>(message.Data.ToString());
                    if (locationData != null)
                    {
                        LocationSelected?.Invoke(this, new LocationSelectedEventArgs(locationData));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing web message: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task SetRouteAsync(string from, string to, string transportType)
        {
            try
            {
                var route = await CalculateRouteAsync(from, to, transportType);
                if (route != null)
                {
                    await MapWebView.CoreWebView2.ExecuteScriptAsync($@"
                        if (window.routeLayer) {{
                            map.removeLayer(window.routeLayer);
                        }}
                        if (window.markers) {{
                            window.markers.forEach(marker => map.removeLayer(marker));
                            window.markers = [];
                        }}
                        
                        const fromMarker = L.marker([{route.Features[0].Geometry.Coordinates[0][1]}, {route.Features[0].Geometry.Coordinates[0][0]}]).addTo(map);
                        const toMarker = L.marker([{route.Features[0].Geometry.Coordinates[route.Features[0].Geometry.Coordinates.Length - 1][1]}, {route.Features[0].Geometry.Coordinates[route.Features[0].Geometry.Coordinates.Length - 1][0]}]).addTo(map);
                        window.markers = [fromMarker, toMarker];
                        
                        const routeCoordinates = {System.Text.Json.JsonSerializer.Serialize(route.Features[0].Geometry.Coordinates)};
                        const routeLine = L.polyline(routeCoordinates.map(coord => [coord[1], coord[0]]), {{
                            color: 'blue',
                            weight: 3
                        }}).addTo(map);
                        window.routeLayer = routeLine;
                        
                        map.fitBounds(routeLine.getBounds(), {{ padding: [50, 50] }});
                        
                        window.chrome.webview.postMessage(JSON.stringify({{
                            type: 'routeCalculated',
                            data: {{
                                distance: {route.Features[0].Properties.Segments[0].Distance},
                                duration: {route.Features[0].Properties.Segments[0].Duration}
                            }}
                        }}));
                    ");

                    RouteCalculated?.Invoke(this, new RouteCalculatedEventArgs(
                        new RouteData
                        {
                            Distance = route.Features[0].Properties.Segments[0].Distance,
                            Duration = route.Features[0].Properties.Segments[0].Duration
                        }
                    ));
                }
            }
            catch (Exception)
            {
                // Silently handle the error
            }
        }

        private async Task<RouteResponse> CalculateRouteAsync(string from, string to, string transportType)
        {
            try
            {
                var fromCoords = await GeocodeLocationAsync(from);
                var toCoords = await GeocodeLocationAsync(to);

                if (fromCoords == null || toCoords == null)
                {
                    return null;
                }

                var request = new
                {
                    coordinates = new[]
                    {
                        new[] { fromCoords.Lon, fromCoords.Lat },
                        new[] { toCoords.Lon, toCoords.Lat }
                    },
                    format = "json"
                };

                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );

                var profile = GetOrsProfile(transportType);
                var response = await _httpClient.PostAsync($"{API_BASE_URL}{profile}", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<RouteResponse>(responseContent);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<Coordinates> GeocodeLocationAsync(string location)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(location)}&limit=1");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var results = System.Text.Json.JsonSerializer.Deserialize<GeocodingResult[]>(content);

                if (results != null && results.Length > 0)
                {
                    return new Coordinates
                    {
                        Lat = double.Parse(results[0].Lat),
                        Lon = double.Parse(results[0].Lon)
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetOrsProfile(string transportType)
        {
            return transportType?.ToLower() switch
            {
                "car" or "driving" => "driving-car",
                "bike" or "cycling" => "cycling-regular",
                "walk" or "walking" => "foot-walking",
                _ => "driving-car"
            };
        }

        private string GetMapHtml()
        {
            return @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Tour Map</title>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
                    <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
                    <style>
                        html, body { 
                            margin: 0; 
                            padding: 0; 
                            height: 100%; 
                            width: 100%; 
                            overflow: hidden;
                        }
                        #map { 
                            height: 100%; 
                            width: 100%; 
                            position: absolute;
                            top: 0;
                            left: 0;
                        }
                        .leaflet-container {
                            height: 100%;
                            width: 100%;
                        }
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        const map = L.map('map', {
                            zoomControl: true,
                            scrollWheelZoom: true,
                            doubleClickZoom: true,
                            dragging: true,
                            zoomSnap: 0.1
                        }).setView([48.210033, 16.363449], 13);
                        
                        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                            attribution: '© OpenStreetMap contributors',
                            maxZoom: 19,
                            minZoom: 2
                        }).addTo(map);
                        
                        window.markers = [];
                        window.routeLayer = null;

                        // Handle window resize
                        window.addEventListener('resize', function() {
                            map.invalidateSize();
                            if (window.routeLayer) {
                                map.fitBounds(window.routeLayer.getBounds(), { padding: [50, 50] });
                            }
                        });

                        // Initial size check
                        setTimeout(function() {
                            map.invalidateSize();
                        }, 100);
                    </script>
                </body>
                </html>
            ";
        }
    }

    public class WebMessage
    {
        public string Type { get; set; }
        public object Data { get; set; }
    }

    public class LocationData
    {
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class RouteData
    {
        public double Distance { get; set; }
        public double Duration { get; set; }
    }

    public class RouteCalculatedEventArgs : EventArgs
    {
        public RouteData RouteData { get; }

        public RouteCalculatedEventArgs(RouteData routeData)
        {
            RouteData = routeData;
        }
    }

    public class LocationSelectedEventArgs : EventArgs
    {
        public LocationData LocationData { get; }

        public LocationSelectedEventArgs(LocationData locationData)
        {
            LocationData = locationData;
        }
    }
} 
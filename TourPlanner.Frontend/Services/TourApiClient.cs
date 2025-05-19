using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TourPlanner.Frontend.Services
{
    public class TourApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public string ApiBaseUrl => _baseUrl;

        public TourApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromSeconds(5); // Increased timeout slightly

            // Get the API URL from config or use default if not found
            try
            {
                _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7022/api";
                Debug.WriteLine($"TourApiClient initialized with base URL from config: {_baseUrl}");
            }
            catch (Exception ex)
            {
                _baseUrl = "https://localhost:7022/api"; // Default fallback
                Debug.WriteLine($"Error reading config, using default URL: {_baseUrl}. Error: {ex.Message}");
            }
        }

        public async Task<List<JsonObject>> GetToursAsync()
        {
            Debug.WriteLine($"Attempting to fetch tours from {_baseUrl}/Tour");
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tour");
            
            Debug.WriteLine($"Response status: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var tours = await response.Content.ReadFromJsonAsync<List<JsonObject>>(options);
            Debug.WriteLine($"Successfully retrieved {tours.Count} tours from API");
            return tours;
        }

        public async Task<JsonObject> GetTourByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Tour/{id}");
            response.EnsureSuccessStatusCode();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return await response.Content.ReadFromJsonAsync<JsonObject>(options);
        }

        public async Task<JsonObject> CreateTourAsync(string name, string description, string from, string to, string transportType, float distance)
        {
            var tourId = Guid.NewGuid().ToString();
            
            var tour = new
            {
                Id = tourId,
                Name = name,
                Description = description ?? "",
                FromLocation = from,
                ToLocation = to,
                TransportType = transportType,
                Distance = distance,
                EstimatedTime = 0,
                RouteInformation = "",
                ListId = (string)null
            };
            
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/Tour", tour);
            response.EnsureSuccessStatusCode();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return await response.Content.ReadFromJsonAsync<JsonObject>(options);
        }

        public async Task UpdateTourAsync(string id, string name, string description, string from, string to, string transportType, float distance)
        {
            var tour = new
            {
                Id = id,
                Name = name,
                Description = description ?? "",
                FromLocation = from,
                ToLocation = to,
                TransportType = transportType,
                Distance = distance,
                EstimatedTime = 0,
                RouteInformation = "",
                ListId = (string)null
            };
            
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/Tour/{id}", tour);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTourAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Tour/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
} 
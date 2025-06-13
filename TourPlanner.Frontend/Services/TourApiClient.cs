using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TourPlanner.Frontend.Services
{
    public class TourApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public TourApiClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "https://localhost:7022/api";
                Debug.WriteLine($"TourApiClient initialized with base URL: {_baseUrl}");
            }
            catch (Exception ex)
            {
                _baseUrl = "https://localhost:7022/api";
                Debug.WriteLine($"Error reading config, using default URL: {_baseUrl}. Error: {ex.Message}");
            }
        }

        private async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        private async Task PutAsync(string endpoint, object data)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{endpoint}", data);
            response.EnsureSuccessStatusCode();
        }

        private async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();
        }

        // Tour endpoints
        public async Task<List<JsonObject>> GetToursAsync() => 
            await GetAsync<List<JsonObject>>("Tour");

        public async Task<JsonObject> GetTourByIdAsync(string id) => 
            await GetAsync<JsonObject>($"Tour/{id}");

        public async Task<JsonObject> CreateTourAsync(string name, string description, string from, string to, string transportType)
        {
            var tour = new
            {
                Name = name,
                Description = description ?? "",
                FromLocation = from,
                ToLocation = to,
                TransportType = transportType,
                RouteInformation = "",
                ListId = (string)null
            };
            
            return await PostAsync<JsonObject>("Tour", tour);
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
            
            await PutAsync($"Tour/{id}", tour);
        }

        public async Task DeleteTourAsync(string id) => 
            await DeleteAsync($"Tour/{id}");

        // Log endpoints
        public async Task<JsonArray> GetLogsByTourIdAsync(string tourId)
        {
            try
            {
                Debug.WriteLine($"Getting logs for tour {tourId} from {_baseUrl}/TourLog/tour/{tourId}");
                var response = await _httpClient.GetAsync($"{_baseUrl}/TourLog/tour/{tourId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Server error: {response.StatusCode}, Content: {errorContent}");
                    throw new HttpRequestException($"Server returned {response.StatusCode}: {errorContent}");
                }
                
                response.EnsureSuccessStatusCode();
                var logs = await response.Content.ReadFromJsonAsync<JsonArray>(_jsonOptions);
                Debug.WriteLine($"Received {logs?.Count ?? 0} logs from API");
                return logs ?? new JsonArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting logs: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<JsonObject> CreateLogAsync(string tourId, string comment, int difficulty, int rating, TimeSpan duration)
        {
            var log = new
            {
                Id = Guid.NewGuid().ToString(),
                Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Comment = comment,
                Difficulty = difficulty,
                Rating = rating,
                Duration = duration,
                TourId = tourId
            };
            
            Debug.WriteLine($"Creating log for tour {tourId}");
            return await PostAsync<JsonObject>("TourLog", log);
        }

        public async Task UpdateLogAsync(string id, string tourId, DateTime date, string comment, int difficulty, int rating, TimeSpan duration)
        {
            var log = new
            {
                Id = id,
                TourId = tourId,
                Date = date,
                Comment = comment,
                Difficulty = difficulty,
                Rating = rating,
                Duration = duration
            };
            
            await PutAsync($"TourLog/{id}", log);
        }

        public async Task DeleteLogAsync(string id) => 
            await DeleteAsync($"TourLog/{id}");
    }
} 
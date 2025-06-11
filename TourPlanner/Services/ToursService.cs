using System.Text.Json;
using TourPlanner.Models;

namespace TourPlanner.Services
{
    public class ToursService : IToursService
    {
        private const string ApiKey = "5b3ce3597851110001cf624844d7d66abc684a6688362166536896b6";
        private const string GeocodeBaseUrl = "https://api.openrouteservice.org/geocode/search";
        private const string DirectionsBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";
        private readonly HttpClient _httpClient;

        public ToursService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public float CalculateDistance(Tour tour)
        {
            return CalculateDistanceAndTimeAsync(tour).GetAwaiter().GetResult().distance;
        }

        public int CalculateEstimatedTime(Tour tour)
        {
            return (int)CalculateDistanceAndTimeAsync(tour).GetAwaiter().GetResult().duration;
        }

        public string[] ResolveCoords(Tour tour)
        {
            return ResolveCoordsAsync(tour).GetAwaiter().GetResult();
        }

        private async Task<string[]> ResolveCoordsAsync(Tour tour)
        {
            string fromUrl = $"{GeocodeBaseUrl}?api_key={ApiKey}&text={Uri.EscapeDataString(tour.FromLocation)}";
            string toUrl = $"{GeocodeBaseUrl}?api_key={ApiKey}&text={Uri.EscapeDataString(tour.ToLocation)}";

            var fromTask = _httpClient.GetStringAsync(fromUrl);
            var toTask = _httpClient.GetStringAsync(toUrl);

            await Task.WhenAll(fromTask, toTask);

            string fromCoord = ExtractCoords(fromTask.Result);
            string toCoord = ExtractCoords(toTask.Result);

            return new[] { fromCoord, toCoord };
        }

        private string ExtractCoords(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var coords = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("geometry")
                .GetProperty("coordinates");

            // Format: "longitude,latitude"
            return $"{coords[0]}, {coords[1]}";
        }

        private async Task<(float distance, double duration)> CalculateDistanceAndTimeAsync(Tour tour)
        {
            var coords = await ResolveCoordsAsync(tour);

            // Parse coordinates
            var fromParts = coords[0].Split(',', StringSplitOptions.TrimEntries);
            var toParts = coords[1].Split(',', StringSplitOptions.TrimEntries);

            string start = $"{fromParts[0]},{fromParts[1]}";
            string end = $"{toParts[0]},{toParts[1]}";

            string url = $"{DirectionsBaseUrl}?api_key={ApiKey}&start={start}&end={end}";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var summary = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary");

            float distance = (float)summary.GetProperty("distance").GetDouble(); // meters
            double duration = summary.GetProperty("duration").GetDouble(); // seconds

            return (distance, duration);
        }
    }
}

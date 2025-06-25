using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace TourPlanner.Frontend.Services
{
    public class ConfigurationService
    {
        private static ConfigurationService? _instance;
        private readonly Dictionary<string, JsonElement> _config;

        private ConfigurationService()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                string message = $"Configuration file not found at: {configPath}\nPlease ensure appsettings.json is present in the application directory.";
                MessageBox.Show(message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new FileNotFoundException(message, configPath);
            }

            try
            {
                string jsonContent = File.ReadAllText(configPath);
                _config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonContent)
                    ?? throw new InvalidOperationException("Failed to parse configuration file");
            }
            catch (JsonException ex)
            {
                string message = $"Error parsing configuration file: {ex.Message}";
                MessageBox.Show(message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                string message = $"Error reading configuration file: {ex.Message}";
                MessageBox.Show(message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        public static ConfigurationService Instance
        {
            get
            {
                _instance ??= new ConfigurationService();
                return _instance;
            }
        }

        public string GetOpenRouteServiceApiKey()
        {
            return GetConfigValue<string>("OpenRouteService", "ApiKey");
        }

        public string GetOpenRouteServiceBaseUrl()
        {
            return GetConfigValue<string>("OpenRouteService", "BaseUrl");
        }

        public string GetGeocodeUrl()
        {
            return GetConfigValue<string>("OpenRouteService", "GeocodeUrl");
        }

        public (double Lat, double Lon) GetMapDefaultCenter()
        {
            return (
                GetConfigValue<double>("Map", "DefaultCenter.Lat"),
                GetConfigValue<double>("Map", "DefaultCenter.Lon")
            );
        }

        public int GetMapDefaultZoom()
        {
            return GetConfigValue<int>("Map", "DefaultZoom");
        }

        public string GetMapTileServer()
        {
            return GetConfigValue<string>("Map", "TileServer");
        }

        private T GetConfigValue<T>(string section, string path)
        {
            try
            {
                if (!_config.ContainsKey(section))
                {
                    throw new KeyNotFoundException($"Configuration section '{section}' not found");
                }

                JsonElement element = _config[section];
                string[] parts = path.Split('.');
                
                // Navigate through nested properties
                foreach (var part in parts)
                {
                    element = element.GetProperty(part);
                }

                // Convert the JsonElement to the requested type
                return (T)Convert.ChangeType(GetElementValue(element, typeof(T)), typeof(T));
            }
            catch (Exception ex)
            {
                string message = $"Error reading configuration value {section}.{path}: {ex.Message}";
                MessageBox.Show(message, "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private static object GetElementValue(JsonElement element, Type targetType)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString()!,
                JsonValueKind.Number when targetType == typeof(int) => element.GetInt32(),
                JsonValueKind.Number when targetType == typeof(double) => element.GetDouble(),
                JsonValueKind.Number when targetType == typeof(float) => element.GetSingle(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => throw new InvalidOperationException($"Unsupported value kind: {element.ValueKind}")
            };
        }
    }
} 
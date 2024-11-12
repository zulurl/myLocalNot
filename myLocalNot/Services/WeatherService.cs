using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using myLocalNot.Models;

namespace myLocalNot.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = ""; // Substitua com sua chave de API válida
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather?q=Lisbon&appid=";

        public WeatherService()
        {
            _httpClient = new HttpClient();
            Debug.WriteLine("WeatherService initialized.");
        }

        public async Task<WeatherData> GetWeatherDataAsync()
        {
            try
            {
                Debug.WriteLine("Fetching weather data...");
                var response = await _httpClient.GetStringAsync($"{BaseUrl}{ApiKey}");
                Debug.WriteLine($"Received response: {response}");

                var weatherData = JsonSerializer.Deserialize<WeatherData>(response);
                Debug.WriteLine("Deserialized weather data successfully.");

                return weatherData;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching weather data: {ex.Message}");
                throw;
            }
        }
    }
}

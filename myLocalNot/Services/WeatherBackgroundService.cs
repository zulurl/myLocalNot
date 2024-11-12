using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using myLocalNot.Models;
using Microsoft.Maui.ApplicationModel; // Para MainThread

namespace myLocalNot.Services
{
    public class WeatherBackgroundService
    {
        private readonly WeatherService _weatherService;
        private Timer _timer;

        // Evento para notificar a UI sobre atualizações de clima
        public event Action<string, string> WeatherUpdated;

        public WeatherBackgroundService()
        {
            _weatherService = new WeatherService();
            Debug.WriteLine("WeatherBackgroundService initialized.");
        }

        public void Start()
        {
            Debug.WriteLine("Starting WeatherBackgroundService...");
            _timer = new Timer(async _ => await FetchWeatherData(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            Debug.WriteLine("Stopping WeatherBackgroundService...");
            _timer?.Dispose();
        }

        private async Task FetchWeatherData()
        {
            try
            {
                Debug.WriteLine("Fetching weather data...");
                var weatherData = await _weatherService.GetWeatherDataAsync();
                double tempInCelsius = weatherData.Main.Temp - 273.15;
                string temperature = $"{tempInCelsius:F1}°C";
                string conditions = weatherData.Weather[0].Description;

                // Dispara o evento e atualiza a UI na MainThread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeatherUpdated?.Invoke(temperature, conditions);
                    Debug.WriteLine($"Weather updated: {temperature}, {conditions}");
                });

                // Envia uma notificação na MainThread
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var notification = new NotificationRequest
                    {
                        NotificationId = 100,
                        Title = "Previsão do Tempo",
                        Description = $"Temperatura: {temperature}, Condições: {conditions}",
                        ReturningData = "Dados do clima",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = DateTime.Now.AddSeconds(5)
                        }
                    };
                    await LocalNotificationCenter.Current.Show(notification);
                    Debug.WriteLine("Notificação enviada com sucesso.");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar dados do clima: {ex.Message}");
            }
        }
    }
}

using Android.Content;
using Android.Util;
using AndroidX.Work;
using System;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using myLocalNot.Services;

namespace myLocalNot.Droid
{
    public class WeatherWorker : Worker
    {
        private readonly WeatherService _weatherService;

        public WeatherWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
            _weatherService = new WeatherService();
            Log.Debug("WeatherWorker", "WeatherWorker inicializado");
        }

        public override Result DoWork()
        {
            Log.Debug("WeatherWorker", "DoWork iniciado");

            // Executa a tarefa de buscar dados de clima
            Task.Run(async () => await FetchWeatherData()).Wait();
            return Result.InvokeSuccess();
        }

        private async Task FetchWeatherData()
        {
            try
            {
                Log.Debug("WeatherWorker", "Iniciando a busca de dados de clima...");
                
                var weatherData = await _weatherService.GetWeatherDataAsync();
                if (weatherData == null)
                {
                    Log.Error("WeatherWorker", "Nenhum dado de clima retornado pela API.");
                    return;
                }

                double tempInCelsius = weatherData.Main.Temp - 273.15;
                string message = $"Temperatura: {tempInCelsius:F1}°C, {weatherData.Weather[0].Description}";

                Log.Debug("WeatherWorker", $"Dados de clima recebidos: {message}");

                // Criar e exibir a notificação
                var notification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "Previsão do Tempo",
                    Description = message,
                    ReturningData = "Dados do clima"
                };

                Log.Debug("WeatherWorker", "Exibindo a notificação...");
                await LocalNotificationCenter.Current.Show(notification);
                Log.Debug("WeatherWorker", "Notificação exibida com sucesso.");
            }
            catch (Exception ex)
            {
                Log.Error("WeatherWorker", $"Erro ao buscar dados do clima: {ex.Message}");
            }
        }
    }
}

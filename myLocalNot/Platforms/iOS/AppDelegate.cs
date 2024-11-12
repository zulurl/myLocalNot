using Foundation;
using UIKit;
using UserNotifications;
using System;
using Plugin.LocalNotification;
using myLocalNot;
using myLocalNot.Services;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        // Configurar o Background Fetch com intervalo mínimo
        UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
        
        // Solicitar permissão para notificações locais
        RequestNotificationPermission();

        return base.FinishedLaunching(app, options);
    }

    private void RequestNotificationPermission()
    {
        UNUserNotificationCenter.Current.RequestAuthorization(
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Badge,
            (approved, err) =>
            {
                if (approved)
                {
                    Console.WriteLine("Permissão para notificações locais concedida.");
                }
                else
                {
                    Console.WriteLine("Permissão para notificações locais negada.");
                }
            });
    }

    public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
    {
        // Iniciar a tarefa de atualização em segundo plano
        Task.Run(async () =>
        {
            try
            {
                // Exemplo de código para obter dados do serviço de clima
                var weatherService = new WeatherService();
                var weatherData = await weatherService.GetWeatherDataAsync();

                if (weatherData != null)
                {
                    // Converte a temperatura e cria a mensagem da notificação
                    double tempInCelsius = weatherData.Main.Temp - 273.15;
                    string message = $"Temperatura: {tempInCelsius:F1}°C, {weatherData.Weather[0].Description}";

                    // Configura a notificação local
                    var notification = new NotificationRequest
                    {
                        NotificationId = 100,
                        Title = "Previsão do Tempo",
                        Description = message,
                        ReturningData = "Dados do clima",
                        
                    };

                    await LocalNotificationCenter.Current.Show(notification);

                    // Notifica o iOS que o fetch foi bem-sucedido
                    completionHandler(UIBackgroundFetchResult.NewData);
                }
                else
                {
                    // Nenhum dado novo
                    completionHandler(UIBackgroundFetchResult.NoData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar dados do clima: {ex.Message}");
                completionHandler(UIBackgroundFetchResult.Failed);
            }
        });
    }
}

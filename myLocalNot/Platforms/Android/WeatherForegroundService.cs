using Android.App;
using Android.Content;
using Android.OS;
using Plugin.LocalNotification;
using System.Threading.Tasks;
using myLocalNot.Services;

namespace myLocalNot.Droid
{
    [Service]
    public class WeatherForegroundService : Service
    {
        private Timer _timer;
        private readonly WeatherService _weatherService = new WeatherService();

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForeground(1001, CreateNotification("Iniciando serviço de clima..."));

            // Configurar o Timer para buscar dados de clima a cada intervalo
            _timer = new Timer(async _ => await FetchWeatherData(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _timer?.Dispose();
            base.OnDestroy();
        }

        private async Task FetchWeatherData()
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherDataAsync();
                double tempInCelsius = weatherData.Main.Temp - 273.15;
                string message = $"Temperatura: {tempInCelsius:F1}°C, {weatherData.Weather[0].Description}";

                // Atualizar notificação
                var notification = CreateNotification(message);
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.Notify(1001, notification);

                // Enviar uma notificação separada para o usuário
                var localNotification = new NotificationRequest
                {
                    NotificationId = 100,
                    Title = "Previsão do Tempo",
                    Description = message,
                    ReturningData = "Dados do clima"
                };
                await LocalNotificationCenter.Current.Show(localNotification);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("WeatherForegroundService", $"Erro ao buscar dados do clima: {ex.Message}");
            }
        }

        private Notification CreateNotification(string message)
        {
            var channelId = "weather_notification_channel";

            // Cria o canal de notificação (necessário para Android 8.0+)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(channelId, "Weather Updates", NotificationImportance.Default)
                {
                    Description = "Notificações de atualizações do tempo"
                };
                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }

            // Cria a notificação
            var notificationBuilder = new Notification.Builder(this, channelId)
                .SetContentTitle("Serviço de Clima")
                .SetContentText(message)
             //   .SetSmallIcon(Resource.Drawable.icon) // Substitua pelo ícone correto
                .SetOngoing(true);

            return notificationBuilder.Build();
        }
    }
}

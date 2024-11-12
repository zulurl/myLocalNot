using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Work;
using System;
using myLocalNot.Droid;
using Android.Provider;


namespace myLocalNot
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Iniciar o agendamento do WeatherWorker
            ScheduleWeatherWorker();
            CheckBatteryOptimization();
    

   
            // Iniciar o Foreground Service para notificações do clima
            var intent = new Intent(this, typeof(WeatherForegroundService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }
        }

        public void ScheduleWeatherWorker()
        {
            var workRequest = PeriodicWorkRequest.Builder
                .From<WeatherWorker>(TimeSpan.FromMinutes(1))
                .Build();

            WorkManager.Instance.EnqueueUniquePeriodicWork(
                "WeatherWorker",
                ExistingPeriodicWorkPolicy.Keep,
                workRequest);
        }

    private void CheckBatteryOptimization()
{
    var powerManager = (PowerManager)GetSystemService(PowerService);
    if (!powerManager.IsIgnoringBatteryOptimizations(PackageName))
    {
        // Mostrar uma explicação ao usuário e solicitar a permissão específica para o aplicativo
        var intent = new Intent();
        intent.SetAction(Settings.ActionRequestIgnoreBatteryOptimizations);
        intent.SetData(Android.Net.Uri.Parse($"package:{PackageName}"));
        StartActivity(intent);
    }
}


private void ShowBatteryOptimizationDialog()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Otimização de Bateria");
            builder.SetMessage("Para que o serviço de clima funcione corretamente, é necessário excluir o aplicativo da otimização de bateria.");
            builder.SetPositiveButton("OK", (sender, args) =>
            {
                // Redirecionar para configurações de otimização
                var intent = new Intent(Settings.ActionIgnoreBatteryOptimizationSettings);
                StartActivity(intent);
            });
            builder.SetNegativeButton("Cancelar", (sender, args) =>
            {
                // Usuário recusou a permissão; ação opcional
            });
            builder.Show();
        }

    }
}

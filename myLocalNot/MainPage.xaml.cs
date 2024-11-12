using System;
using System.Diagnostics;
using System.Threading.Tasks;
using myLocalNot.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Plugin.LocalNotification;

namespace myLocalNot
{
    public partial class MainPage : ContentPage
    {
        private WeatherBackgroundService _weatherService;

        public MainPage()
        {
            InitializeComponent();
            _weatherService = new WeatherBackgroundService();

            // Subscreve ao evento para atualizar a UI quando houver novos dados de clima
            _weatherService.WeatherUpdated += OnWeatherUpdated;
            Debug.WriteLine("MainPage initialized and event subscribed.");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckNotificationPermissionAsync();
            Debug.WriteLine("MainPage appeared and notification permissions checked.");
        }

        private async Task CheckNotificationPermissionAsync()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.Version.Major >= 13)
            {
                var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                }

                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Permissão de notificação não concedida.");
                    return;
                }
            }

            if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
        }

        private void StartServiceButton_Clicked(object sender, EventArgs e)
        {
            _weatherService.Start();
            ServiceStatusLabel.Text = "Serviço em execução";
            Debug.WriteLine("Serviço iniciado.");
        }

        private void StopServiceButton_Clicked(object sender, EventArgs e)
        {
            _weatherService.Stop();
            ServiceStatusLabel.Text = "Serviço parado";
            Debug.WriteLine("Serviço parado.");
        }

        // Método chamado quando o evento WeatherUpdated é disparado
        private void OnWeatherUpdated(string temperature, string conditions)
        {
            // Atualiza a interface com os novos dados de clima
            Debug.WriteLine($"Atualizando a UI: {temperature}, {conditions}");
            UpdateWeatherDisplay(temperature, conditions, DateTime.Now);
        }

        public void UpdateWeatherDisplay(string temperature, string conditions, DateTime lastUpdate)
        {
            TemperatureLabel.Text = $"Temperatura: {temperature}";
            ConditionsLabel.Text = $"Condições: {conditions}";
            LastUpdateLabel.Text = $"Última Atualização: {lastUpdate:HH:mm:ss}";
        }
    }
}

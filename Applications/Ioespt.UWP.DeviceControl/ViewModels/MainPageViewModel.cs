using Template10.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Ioespt.UWP.DeviceControl.Models;
using System;
using Ioespt.UWP.DeviceControl.Services.DataServices;
using Newtonsoft.Json;
using System.Net.Http;
using Windows.Devices.WiFi;
using Ioespt.UWP.Devices;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        RegisteredDevice _Value = null;
        public RegisteredDevice Value { get { return _Value; } set { Set(ref _Value, value); } }

        public ObservableCollection<RegisteredDevice> devices
        {
            get
            {
                return ((App)App.Current).devices;
            }
        }

        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                #region Design Time Info

                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Lamp 1",
                //    Status = DeviceStatus.Offline,
                //    ConnectedTo = "Adafruit IO (MQTT)"
                //});
                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Socket 1",
                //    Status = DeviceStatus.Online,
                //    ConnectedTo = "Amazon AWS"
                //});
                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-12" },
                //    GivenName = "Weather Station",
                //    Status = DeviceStatus.Online,
                //    ConnectedTo = "Azure IoT Hub"
                //});

                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Doorbell",
                //    Status = DeviceStatus.UnProvisioned,
                //    ConnectedTo = "N/A"
                //});

                #endregion /Design Time Info
            }
            this.PropertyChanged += MainPageViewModel_PropertyChanged; 
        }

        private void MainPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Value" && Value != null)
            {
                NavigationService.Navigate(typeof(Views.DetailPage), Value);
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            Value = null;
            if (suspensionState.Any())
            {
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            return Task.CompletedTask;
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), null);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public async void Search()
        {
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                Views.Busy.SetBusy(true, "WiFi searching ...");

                var firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                await firstAdapter.ScanAsync();

                var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower().StartsWith("ioespt-thing"));

                Views.Busy.SetBusy(true, $"WiFi found  {qualifyingWifi.Count()} devices...");


                foreach (WiFiAvailableNetwork deviceWifi in qualifyingWifi)
                {
                    Views.Busy.SetBusy(true, $"WiFi connecting to  {deviceWifi.Ssid}");

                    var connectionResult = await firstAdapter.ConnectAsync(deviceWifi, WiFiReconnectionKind.Automatic);

                    if (connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        try
                        {

                            HttpClient httpClient = new HttpClient();

                            var stringRes = await httpClient.GetStringAsync("http://192.168.4.1/");

                            var details = JsonConvert.DeserializeObject<DeviceDetails>(stringRes);

                            RegisteredDevice newRegisteredDevice = new RegisteredDevice()
                            {
                                Status = DeviceStatus.UnProvisioned,
                                ConnectedTo = "None",
                                GivenName = deviceWifi.Ssid,
                                ChipId = details.ChipId,
                                FirmwareName = details.FirmwareName,
                                FirmwareVersion = details.FirmwareVersion,
                                ModuleType = details.ModuleType
                            };

                            DataService db = new DataService();

                            db.InsertNewDevice(newRegisteredDevice);
                            ((App)App.Current).devices.Add(newRegisteredDevice);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
            }
            Views.Busy.SetBusy(false);
        }

    }
}


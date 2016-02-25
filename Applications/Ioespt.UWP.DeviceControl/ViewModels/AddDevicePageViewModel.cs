using Ioespt.UWP.DeviceControl.Models;
using Ioespt.UWP.DeviceControl.Services.DataServices;
using Ioespt.UWP.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    [PropertyChanged.ImplementPropertyChanged]
    class AddDevicePageViewModel : ViewModelBase
    {

        #region Properties

        public Visibility startPanelVisible { get; set; }

        public Visibility detailsPanelVisible { get; set; }

        public string SearchStatus { get; set; }

        public string DeviceName { get; set; }

        public string ConnectionStatus { get; set; }

        public string DetailsStatus { get; set; }


        #endregion /Properties

        public AddDevicePageViewModel()
        {
            startPanelVisible = Visibility.Visible;

            detailsPanelVisible = Visibility.Collapsed;

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                startPanelVisible = Visibility.Collapsed;
                detailsPanelVisible = Visibility.Visible;
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {

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

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public async void FindDevice()
        {
            startPanelVisible = Visibility.Collapsed;
            detailsPanelVisible = Visibility.Visible;

            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                var firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                SearchStatus = $"Scanning";

                await firstAdapter.ScanAsync();

                var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower().StartsWith("ioespt-thing"));

                SearchStatus = $"Found {qualifyingWifi.Count()} devices";

                foreach(WiFiAvailableNetwork deviceWifi in qualifyingWifi)
                {
                    DetailsStatus = "";

                    DeviceName = deviceWifi.Ssid;

                    ConnectionStatus = "Connecting...";

                    var connectionResult = await firstAdapter.ConnectAsync(deviceWifi, WiFiReconnectionKind.Automatic);

                    if(connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        ConnectionStatus = "Connected";
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

                            DetailsStatus = "Done";

                        }
                        catch(Exception)
                        {
                            DetailsStatus = "Failed :(";
                        }
                    }
                    else
                    {
                        ConnectionStatus = "Failed :(";
                    }
                }
            }
            else
            {
                SearchStatus = "No Devices Found";
            }
        }

        public void Done()
        {

        }

        public void ConnectWifi()
        {
            //if(selectedNetwork != null)
            //{
            //    HttpClient httpClient = new HttpClient();

            //    WifiConfig wifi = new WifiConfig()
            //    {
            //        ssid = ((WiFiAvailableNetwork)selectedNetwork).Ssid,
            //        password = password
            //    };

            //    StringContent sc = new StringContent(JsonConvert.SerializeObject(wifi));

            //    await httpClient.PostAsync("http://192.168.4.1/wifisettings", sc);
            //}
        }

    }
}

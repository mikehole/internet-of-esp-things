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

        public DeviceDetails device { get; set; }

        public ObservableCollection<WiFiAvailableNetwork> networks = new ObservableCollection<WiFiAvailableNetwork>();

        public object selectedNetwork;

        public string password { get; set; }
        

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

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

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

            Views.Busy.SetBusy(true, "Looking for device WiFi connection");

            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());
            if (result.Count >= 1)
            {
                var firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                Views.Busy.SetBusy(true, "Looking for device WiFi connection ... scanning... ");

                await firstAdapter.ScanAsync();

                var deviceWifi = firstAdapter.NetworkReport.AvailableNetworks.FirstOrDefault(N => N.Ssid.ToLower() == "ioespt-thing");

                if(deviceWifi != null)
                {
                    Views.Busy.SetBusy(true, "Looking for device WiFi connection ... connecting... ");

                    var connectionResult = await firstAdapter.ConnectAsync(deviceWifi, WiFiReconnectionKind.Automatic);

                    if(connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
                    {
                        Views.Busy.SetBusy(true, "Looking device WiFi connected ... looking for device... ");

                        try
                        {
                            HttpClient httpClient = new HttpClient();

                            var stringRes = await httpClient.GetStringAsync("http://192.168.4.1/");

                            device = JsonConvert.DeserializeObject<DeviceDetails>(stringRes);

                            Views.Busy.SetBusy(false);

                            networks.Clear();

                            foreach(var network in firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower() != "ioespt-thing"))
                            {
                                networks.Add(network);
                            }

                            selectedNetwork = networks.FirstOrDefault();

                        }
                        catch(Exception)
                        {
                            Views.Busy.SetBusy(false);

                            //Say something
                        }


                    }
                    else
                    {

                    }
                }

            }
            else
            {
                Views.Busy.SetBusy(false);

                ///TODO: Add message for no WiFi found
            }


        }

        public async void ConnectWifi()
        {
            if(selectedNetwork != null)
            {
                HttpClient httpClient = new HttpClient();

                WifiConfig wifi = new WifiConfig()
                {
                    ssid = ((WiFiAvailableNetwork)selectedNetwork).Ssid,
                    password = password
                };

                StringContent sc = new StringContent(JsonConvert.SerializeObject(wifi));

                await httpClient.PostAsync("http://192.168.4.1/wifisettings", sc);
            }
        }

    }
}

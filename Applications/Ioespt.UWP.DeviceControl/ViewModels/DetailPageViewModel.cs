using Ioespt.UWP.DeviceControl.Models;
using Ioespt.UWP.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Devices.WiFi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        public RegisteredDevice selectedDevice { get; set; }

        public Visibility showProvisioningDetails { get; set; }

        public Visibility showDeviceDetails { get; set; }

        public ObservableCollection<WiFiAvailableNetwork> networks { get; set; }

        public object selectedNetwork { get; set; }

        public string password { get; set; }

        private WiFiAvailableNetwork deviceNetwork;

        private WiFiAdapter firstAdapter = null;

        public DetailPageViewModel()
        {
            networks = new ObservableCollection<WiFiAvailableNetwork>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            selectedDevice = (suspensionState.ContainsKey(nameof(selectedDevice))) ? suspensionState[nameof(selectedDevice)] as RegisteredDevice : parameter as RegisteredDevice;

            //Set the state in accordance with the state of the device.
            if (selectedDevice.FirmwareName == "Unknown")
            {
                showProvisioningDetails = Visibility.Visible;
                showDeviceDetails = Visibility.Collapsed;

                GetNetworks();
            }
            else
            {
                showProvisioningDetails = Visibility.Collapsed;
                showDeviceDetails = Visibility.Visible;
            }

            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(selectedDevice)] = selectedDevice;
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            return Task.CompletedTask;
        }

        public async void GetNetworks()
        {
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

            if (result.Count >= 1)
            {
                firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                await firstAdapter.ScanAsync();

                deviceNetwork = firstAdapter.NetworkReport.AvailableNetworks.FirstOrDefault(N => N.Ssid == selectedDevice.GivenName); 

                var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => !N.Ssid.ToLower().StartsWith("ioespt-thing"));

                foreach(var network in qualifyingWifi)
                {
                    networks.Add(network);
                }

                selectedNetwork = networks.FirstOrDefault();
            }
        }

        public async void DoConnect()
        {
            if (selectedNetwork == null)
                return;

            Views.Busy.SetBusy(true, $"WiFi connecting to  {deviceNetwork.Ssid}");

            var connectionResult = await firstAdapter.ConnectAsync(deviceNetwork, WiFiReconnectionKind.Automatic);
            if (connectionResult.ConnectionStatus == WiFiConnectionStatus.Success)
            {
                try
                {
                    HttpClient httpClient = new HttpClient();
                    var stringRes = await httpClient.GetStringAsync("http://192.168.4.1/");
                    var details = JsonConvert.DeserializeObject<DeviceDetails>(stringRes);


                    selectedDevice.Status = DeviceStatus.Missing;
                    selectedDevice.ConnectedTo = "None";

                    selectedDevice.ChipId = details.ChipId;
                    selectedDevice.FirmwareName = details.FirmwareName;
                    selectedDevice.FirmwareVersion = details.FirmwareVersion;

                    selectedDevice.ModuleType = details.ModuleType;

                    WifiConfig wifi = new WifiConfig()
                    {
                        ssid = ((WiFiAvailableNetwork)selectedNetwork).Ssid,
                        password = password
                    };

                    StringContent sc = new StringContent(JsonConvert.SerializeObject(wifi));

                    await httpClient.PostAsync("http://192.168.4.1/wifisettings", sc);

                    //DataService db = new DataService();
                    //db.InsertNewDevice(newRegisteredDevice);
                }
                catch (Exception)
                {

                }
            }
            else
            {
            }
            Views.Busy.SetBusy(false);

            showProvisioningDetails = Visibility.Collapsed;
            showDeviceDetails = Visibility.Visible;
        }
    }
}

